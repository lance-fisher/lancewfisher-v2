using Bloodlines.Components;
using Bloodlines.Conviction;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.AI
{
    /// <summary>
    /// Walks expired CaptiveRescue DynastyOperationComponent entities at
    /// ResolveAtInWorldDays and applies the canonical browser rescue
    /// resolution effects from simulation.js:5861-5895.
    ///
    /// Success gate: DynastyOperationCaptiveRescueComponent.SuccessScore >= 0
    /// (deterministic; score computed at dispatch time by
    /// AICaptiveRescueExecutionSystem, sub-slice 23). On success:
    ///   - Calls CapturedMemberHelpers.ReleaseCaptive to flip the captive
    ///     Status -> Released on the captor faction's buffer.
    ///   - Records Stewardship +2 conviction event on the source faction
    ///     (browser simulation.js:5869 "Recovered by covert extraction").
    ///   - Pushes extraction narrative (Good if source==player,
    ///     Warn if target==player, Info otherwise).
    ///
    /// On failure (SuccessScore < 0):
    ///   - Returns max(6, round(EscrowCost.Influence * 0.2)) Influence to
    ///     the captor faction's ResourceStockpileComponent (browser
    ///     simulation.js:5884 captorFaction.resources.influence +=).
    ///   - Records Stewardship +1 conviction event on the CAPTOR faction
    ///     (browser simulation.js:5885 "Foiled rescue attempt").
    ///   - Pushes failed-rescue narrative (Warn if source==player,
    ///     Good if target==player, Info otherwise).
    ///
    /// Void path: if the captive is no longer found on the captor's buffer
    /// (e.g. already released by another path), finalizes silently.
    ///
    /// Always flips DynastyOperationComponent.Active = false regardless of
    /// outcome so the capacity slot releases back to the per-faction cap
    /// (sub-slice 18).
    ///
    /// Deferred:
    ///   - Legitimacy recovery on success (LEGITIMACY_RECOVERY_ON_RESCUE
    ///     at simulation.js:5868; adjustLegitimacy; no canonical Legitimacy
    ///     field outside dynasty-core lane).
    ///   - Captor-side spymaster-renown contribution to the failure
    ///     influence refund scaling (browser uses escrowCost.influence * 0.2;
    ///     Unity uses the same formula but without renown weighting since
    ///     CapturedMemberElement does not carry renown yet).
    ///
    /// Updates after AICaptiveRescueExecutionSystem so dispatch and
    /// resolution cannot happen in the same tick.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AICaptiveRescueExecutionSystem))]
    public partial struct AICaptiveRescueResolutionSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DynastyOperationComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;
            float inWorldDays = GetInWorldDays(em);
            TickRescueResolutions(em, inWorldDays);
        }

        private static void TickRescueResolutions(EntityManager em, float inWorldDays)
        {
            var q = em.CreateEntityQuery(
                ComponentType.ReadWrite<DynastyOperationComponent>(),
                ComponentType.ReadOnly<DynastyOperationCaptiveRescueComponent>());
            var entities = q.ToEntityArray(Allocator.Temp);
            var ops      = q.ToComponentDataArray<DynastyOperationComponent>(Allocator.Temp);
            var rescues  = q.ToComponentDataArray<DynastyOperationCaptiveRescueComponent>(Allocator.Temp);
            q.Dispose();

            for (int i = 0; i < entities.Length; i++)
            {
                if (!ops[i].Active) continue;
                if (ops[i].OperationKind != DynastyOperationKind.CaptiveRescue) continue;
                if (rescues[i].ResolveAtInWorldDays > inWorldDays) continue;

                ResolveOperation(em, ops[i], rescues[i]);

                var op = ops[i];
                op.Active = false;
                em.SetComponentData(entities[i], op);
            }

            entities.Dispose();
            ops.Dispose();
            rescues.Dispose();
        }

        private static void ResolveOperation(
            EntityManager em,
            DynastyOperationComponent op,
            DynastyOperationCaptiveRescueComponent rescue)
        {
            bool success = rescue.SuccessScore >= 0f;
            bool released = CapturedMemberHelpers.ReleaseCaptive(
                em,
                op.TargetFactionId,
                rescue.CaptiveMemberId,
                CapturedMemberStatus.Released);

            if (success)
            {
                if (!released)
                {
                    // Captive already gone; void path, no effects.
                    return;
                }

                RecordConvictionOnFaction(em, op.SourceFactionId, ConvictionBucket.Stewardship, 2f);
                PushSuccessMessage(em, op.SourceFactionId, op.TargetFactionId, rescue.CaptiveMemberTitle);
            }
            else
            {
                ReturnInfluenceToCaptor(em, op.TargetFactionId, rescue.EscrowCost.Influence);
                RecordConvictionOnFaction(em, op.TargetFactionId, ConvictionBucket.Stewardship, 1f);
                PushFailureMessage(em, op.SourceFactionId, op.TargetFactionId, rescue.CaptiveMemberTitle);
            }
        }

        private static void ReturnInfluenceToCaptor(
            EntityManager em,
            FixedString32Bytes captorFactionId,
            float escrowInfluence)
        {
            float refund = math.max(6f, math.round(escrowInfluence * 0.2f));
            var captorEntity = FindFactionEntity(em, captorFactionId);
            if (captorEntity == Entity.Null) return;
            if (!em.HasComponent<ResourceStockpileComponent>(captorEntity)) return;
            var stock = em.GetComponentData<ResourceStockpileComponent>(captorEntity);
            stock.Influence += refund;
            em.SetComponentData(captorEntity, stock);
        }

        private static void RecordConvictionOnFaction(
            EntityManager em,
            FixedString32Bytes factionId,
            ConvictionBucket bucket,
            float amount)
        {
            var entity = FindFactionEntity(em, factionId);
            if (entity == Entity.Null) return;
            if (!em.HasComponent<ConvictionComponent>(entity)) return;
            var conv = em.GetComponentData<ConvictionComponent>(entity);
            ConvictionScoring.ApplyEvent(ref conv, bucket, amount);
            em.SetComponentData(entity, conv);
        }

        private static void PushSuccessMessage(
            EntityManager em,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes captorFactionId,
            FixedString64Bytes memberTitle)
        {
            var message = new FixedString128Bytes();
            message.Append(memberTitle);
            message.Append((FixedString64Bytes)" has been extracted from ");
            message.Append(captorFactionId);
            message.Append((FixedString32Bytes)" custody.");

            var playerId = new FixedString32Bytes("player");
            NarrativeMessageTone tone;
            if (sourceFactionId.Equals(playerId))
                tone = NarrativeMessageTone.Good;
            else if (captorFactionId.Equals(playerId))
                tone = NarrativeMessageTone.Warn;
            else
                tone = NarrativeMessageTone.Info;

            NarrativeMessageBridge.Push(em, message, tone);
        }

        private static void PushFailureMessage(
            EntityManager em,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes captorFactionId,
            FixedString64Bytes memberTitle)
        {
            var message = new FixedString128Bytes();
            message.Append((FixedString64Bytes)"A rescue attempt for ");
            message.Append(memberTitle);
            message.Append((FixedString64Bytes)" failed inside ");
            message.Append(captorFactionId);
            message.Append((FixedString32Bytes)" territory.");

            var playerId = new FixedString32Bytes("player");
            NarrativeMessageTone tone;
            if (sourceFactionId.Equals(playerId))
                tone = NarrativeMessageTone.Warn;
            else if (captorFactionId.Equals(playerId))
                tone = NarrativeMessageTone.Good;
            else
                tone = NarrativeMessageTone.Info;

            NarrativeMessageBridge.Push(em, message, tone);
        }

        private static Entity FindFactionEntity(EntityManager em, FixedString32Bytes factionId)
        {
            var q        = em.CreateEntityQuery(ComponentType.ReadOnly<FactionComponent>());
            var entities = q.ToEntityArray(Allocator.Temp);
            var factions = q.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            q.Dispose();

            Entity match = Entity.Null;
            for (int i = 0; i < entities.Length; i++)
            {
                if (factions[i].FactionId.Equals(factionId))
                {
                    match = entities[i];
                    break;
                }
            }
            entities.Dispose();
            factions.Dispose();
            return match;
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
