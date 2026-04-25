using Bloodlines.Components;
using Bloodlines.Conviction;
using Bloodlines.GameTime;
using Bloodlines.PlayerCovertOps;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Resolves matured DynastyOperationEspionageComponent operations.
    ///
    /// Resolution fires when ResolveAtInWorldDays <= inWorldDays.
    ///
    /// Void path: target faction missing -> silently deactivate op.
    ///
    /// Success path (SuccessScore >= 0):
    ///   Creates an IntelligenceReportElement on the source faction entity via
    ///   PlayerCounterIntelligenceSystem.TryCreateIntelligenceReport + StoreIntelligenceReport.
    ///   Report expires at esp.ReportExpiresAtInWorldDays.
    ///   Stewardship +1 on the source faction.
    ///
    /// Failure path (SuccessScore < 0):
    ///   RecordCounterIntelligenceInterception on target faction.
    ///   EnsureMutualHostility between source and target.
    ///   Stewardship +1 on the target faction (they caught the spy).
    ///
    /// Browser reference: resolveEspionageOperation, simulation.js ~10920-10970.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AIEspionageExecutionSystem))]
    public partial struct AIEspionageResolutionSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DynastyOperationEspionageComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;
            float inWorldDays = GetInWorldDays(em);
            double elapsed    = SystemAPI.Time.ElapsedTime;

            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            var q = em.CreateEntityQuery(
                ComponentType.ReadOnly<DynastyOperationComponent>(),
                ComponentType.ReadOnly<DynastyOperationEspionageComponent>());
            var opEntities = q.ToEntityArray(Allocator.Temp);
            q.Dispose();

            for (int i = 0; i < opEntities.Length; i++)
            {
                var opEntity = opEntities[i];
                var op  = em.GetComponentData<DynastyOperationComponent>(opEntity);
                var esp = em.GetComponentData<DynastyOperationEspionageComponent>(opEntity);

                if (!op.Active) continue;
                if (esp.ResolveAtInWorldDays > inWorldDays) continue;

                Resolve(em, ecb, opEntity, op, esp, inWorldDays, (float)elapsed);
            }

            opEntities.Dispose();
            ecb.Playback(em);
            ecb.Dispose();
        }

        // ------------------------------------------------------------------ resolution

        private static void Resolve(
            EntityManager em,
            EntityCommandBuffer ecb,
            Entity opEntity,
            DynastyOperationComponent op,
            DynastyOperationEspionageComponent esp,
            float inWorldDays,
            float elapsed)
        {
            var sourceEntity = FindFactionEntity(em, esp.SourceFactionId);
            var targetEntity = FindFactionEntity(em, esp.TargetFactionId);

            // Deactivate op regardless of outcome.
            var finalOp = op;
            finalOp.Active = false;
            ecb.SetComponent(opEntity, finalOp);

            if (sourceEntity == Entity.Null) return;

            if (esp.SuccessScore >= 0f)
                ResolveSuccess(em, opEntity, esp, sourceEntity, inWorldDays);
            else
                ResolveFailure(em, esp, targetEntity, inWorldDays);
        }

        private static void ResolveSuccess(
            EntityManager em,
            Entity opEntity,
            DynastyOperationEspionageComponent esp,
            Entity sourceEntity,
            float inWorldDays)
        {
            if (PlayerCounterIntelligenceSystem.TryCreateIntelligenceReport(
                    em,
                    esp.SourceFactionId,
                    esp.TargetFactionId,
                    new FixedString32Bytes("espionage"),
                    new FixedString64Bytes("Court report"),
                    default,
                    0,
                    inWorldDays,
                    esp.ReportExpiresAtInWorldDays,
                    opEntity.Index,
                    out var report))
            {
                PlayerCounterIntelligenceSystem.StoreIntelligenceReport(em, sourceEntity, report);
            }

            ApplyStewardship(em, sourceEntity, 1f);

            var playerId = new FixedString32Bytes("player");
            var tone = esp.TargetFactionId.Equals(playerId)
                ? NarrativeMessageTone.Warn
                : NarrativeMessageTone.Info;
            var msg = new FixedString128Bytes();
            msg.Append(esp.SourceFactionId);
            msg.Append((FixedString32Bytes)" espionage succeeded: court report on ");
            msg.Append(esp.TargetFactionId);
            msg.Append((FixedString32Bytes)".");
            NarrativeMessageBridge.Push(em, msg, tone);
        }

        private static void ResolveFailure(
            EntityManager em,
            DynastyOperationEspionageComponent esp,
            Entity targetEntity,
            float inWorldDays)
        {
            PlayerCounterIntelligenceSystem.RecordCounterIntelligenceInterception(
                em,
                esp.TargetFactionId,
                esp.SourceFactionId,
                new FixedString32Bytes("espionage"),
                default,
                inWorldDays);

            PlayerCounterIntelligenceSystem.EnsureMutualHostility(em, esp.SourceFactionId, esp.TargetFactionId);

            if (targetEntity != Entity.Null)
                ApplyStewardship(em, targetEntity, 1f);

            var playerId = new FixedString32Bytes("player");
            var tone = esp.TargetFactionId.Equals(playerId)
                ? NarrativeMessageTone.Info
                : NarrativeMessageTone.Warn;
            var msg = new FixedString128Bytes();
            msg.Append(esp.SourceFactionId);
            msg.Append((FixedString32Bytes)" espionage agent caught by ");
            msg.Append(esp.TargetFactionId);
            msg.Append((FixedString32Bytes)".");
            NarrativeMessageBridge.Push(em, msg, tone);
        }

        // ------------------------------------------------------------------ helpers

        private static void ApplyStewardship(EntityManager em, Entity factionEntity, float gain)
        {
            if (!em.HasComponent<ConvictionComponent>(factionEntity)) return;
            var conviction = em.GetComponentData<ConvictionComponent>(factionEntity);
            ConvictionScoring.ApplyEvent(ref conviction, ConvictionBucket.Stewardship, gain);
            em.SetComponentData(factionEntity, conviction);
        }

        private static Entity FindFactionEntity(EntityManager em, FixedString32Bytes factionId)
        {
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<FactionComponent>());
            var entities = q.ToEntityArray(Allocator.Temp);
            var factions = q.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            q.Dispose();

            Entity result = Entity.Null;
            for (int i = 0; i < entities.Length; i++)
            {
                if (factions[i].FactionId.Equals(factionId))
                {
                    result = entities[i];
                    break;
                }
            }
            entities.Dispose();
            factions.Dispose();
            return result;
        }

        private static float GetInWorldDays(EntityManager em)
        {
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<DualClockComponent>());
            if (q.IsEmpty) { q.Dispose(); return 0f; }
            float d = q.GetSingleton<DualClockComponent>().InWorldDays;
            q.Dispose();
            return d;
        }
    }
}
