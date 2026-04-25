using Bloodlines.Components;
using Bloodlines.Conviction;
using Bloodlines.Dynasties;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.AI
{
    /// <summary>
    /// Drives AI succession-crisis auto-consolidation.
    ///
    /// When the AI faction has an active SuccessionCrisisComponent and the
    /// SuccessionCrisisTimer fires, the system attempts to consolidate:
    ///   Gate 1: HeadOfBloodline member exists and is not Fallen/Captured.
    ///   Gate 2: Resources >= resolutionCost per severity tier.
    ///
    /// On success:
    ///   - Deduct resolutionCost from resources.
    ///   - Legitimacy += legitimacyRecovery (clamped 0-100).
    ///   - All owned control points: Loyalty += loyaltyRecovery (clamped 0-100).
    ///   - Stewardship += max(2, round(legitimacyRecovery * 0.5)).
    ///   - Push DeclareInWorldTimeRequest for max(7, round(loyaltyRecovery * 5)) days.
    ///   - Remove SuccessionCrisisComponent (crisis resolved).
    ///   - Push narrative message (Info tone).
    ///   - Reset timer to 60s.
    ///
    /// On gate failure (resources or ruler missing):
    ///   - Reset timer to 18s.
    ///
    /// When no active crisis:
    ///   - Reset timer to 12s (default).
    ///
    /// Browser reference: ai.js updateEnemyAi successionCrisisTimer block
    /// (~lines 1167-1185); simulation.js consolidateSuccessionCrisis (~4695-4741).
    ///
    /// Severity cost/recovery table (simulation.js ~55-111):
    ///   Minor(1):       gold 80, inf 18, legit+4, loyalty+3, steward+2, days+15
    ///   Moderate(2):    gold 110, inf 24, legit+6, loyalty+4, steward+3, days+20
    ///   Major(3):       gold 145, inf 32, legit+8, loyalty+5, steward+4, days+25
    ///   Catastrophic(4):gold 190, inf 42, legit+10, loyalty+6, steward+5, days+30
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AICovertOpsSystem))]
    public partial struct AISuccessionCrisisConsolidationSystem : ISystem
    {
        private const float TimerDefault  = 12f;
        private const float TimerRetry    = 18f;
        private const float TimerCooldown = 60f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AISuccessionCrisisConsolidationComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            float dt = SystemAPI.Time.DeltaTime;
            var em  = state.EntityManager;

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            float inWorldDays = GetInWorldDays(em);

            foreach (var (comp, factionComp, entity) in SystemAPI
                .Query<RefRW<AISuccessionCrisisConsolidationComponent>,
                       RefRO<FactionComponent>>()
                .WithEntityAccess())
            {
                comp.ValueRW.SuccessionCrisisTimer -= dt;
                if (comp.ValueRW.SuccessionCrisisTimer > 0f) continue;

                // Verify faction has an active crisis.
                if (!em.HasComponent<SuccessionCrisisComponent>(entity))
                {
                    comp.ValueRW.SuccessionCrisisTimer = TimerDefault;
                    continue;
                }

                var crisis = em.GetComponentData<SuccessionCrisisComponent>(entity);
                if (crisis.CrisisSeverity == (byte)SuccessionCrisisSeverity.None)
                {
                    comp.ValueRW.SuccessionCrisisTimer = TimerDefault;
                    continue;
                }

                GetCostAndRecovery(
                    crisis.CrisisSeverity,
                    out float costGold, out float costInfluence,
                    out float legitimacyRecovery, out float loyaltyRecovery,
                    out float stewardshipGain, out int worldDays);

                // Gate 1: HeadOfBloodline ruler exists and is active.
                if (!HasActiveRuler(em, entity))
                {
                    comp.ValueRW.SuccessionCrisisTimer = TimerRetry;
                    continue;
                }

                // Gate 2: Resources >= cost.
                if (!em.HasComponent<ResourceStockpileComponent>(entity))
                {
                    comp.ValueRW.SuccessionCrisisTimer = TimerRetry;
                    continue;
                }
                var resources = em.GetComponentData<ResourceStockpileComponent>(entity);
                if (resources.Gold < costGold || resources.Influence < costInfluence)
                {
                    comp.ValueRW.SuccessionCrisisTimer = TimerRetry;
                    continue;
                }

                // --- Apply consolidation effects ---

                resources.Gold      -= costGold;
                resources.Influence -= costInfluence;
                em.SetComponentData(entity, resources);

                if (em.HasComponent<DynastyStateComponent>(entity))
                {
                    var dynasty = em.GetComponentData<DynastyStateComponent>(entity);
                    dynasty.Legitimacy = math.clamp(dynasty.Legitimacy + legitimacyRecovery, 0f, 100f);
                    em.SetComponentData(entity, dynasty);
                }

                ApplyControlPointLoyaltyRecovery(em, factionComp.ValueRO.FactionId, loyaltyRecovery);

                if (em.HasComponent<ConvictionComponent>(entity))
                {
                    var conviction = em.GetComponentData<ConvictionComponent>(entity);
                    ConvictionScoring.ApplyEvent(ref conviction, ConvictionBucket.Stewardship, stewardshipGain);
                    em.SetComponentData(entity, conviction);
                }

                PushDeclareInWorldTimeRequest(em, worldDays);

                ecb.RemoveComponent<SuccessionCrisisComponent>(entity);

                PushNarrative(em, factionComp.ValueRO.FactionId);

                comp.ValueRW.SuccessionCrisisTimer = TimerCooldown;
            }

            ecb.Playback(em);
            ecb.Dispose();
        }

        // ------------------------------------------------------------------ helpers

        private static void GetCostAndRecovery(
            byte severity,
            out float costGold, out float costInfluence,
            out float legitimacyRecovery, out float loyaltyRecovery,
            out float stewardshipGain, out int worldDays)
        {
            switch ((SuccessionCrisisSeverity)severity)
            {
                case SuccessionCrisisSeverity.Moderate:
                    costGold = 110f; costInfluence = 24f;
                    legitimacyRecovery = 6f; loyaltyRecovery = 4f;
                    stewardshipGain = 3f; worldDays = 20;
                    break;
                case SuccessionCrisisSeverity.Major:
                    costGold = 145f; costInfluence = 32f;
                    legitimacyRecovery = 8f; loyaltyRecovery = 5f;
                    stewardshipGain = 4f; worldDays = 25;
                    break;
                case SuccessionCrisisSeverity.Catastrophic:
                    costGold = 190f; costInfluence = 42f;
                    legitimacyRecovery = 10f; loyaltyRecovery = 6f;
                    stewardshipGain = 5f; worldDays = 30;
                    break;
                default: // Minor
                    costGold = 80f; costInfluence = 18f;
                    legitimacyRecovery = 4f; loyaltyRecovery = 3f;
                    stewardshipGain = 2f; worldDays = 15;
                    break;
            }
        }

        private static bool HasActiveRuler(EntityManager em, Entity factionEntity)
        {
            if (!em.HasBuffer<DynastyMemberRef>(factionEntity)) return false;
            var roster = em.GetBuffer<DynastyMemberRef>(factionEntity);
            for (int i = 0; i < roster.Length; i++)
            {
                var memberEntity = roster[i].Member;
                if (memberEntity == Entity.Null) continue;
                if (!em.HasComponent<DynastyMemberComponent>(memberEntity)) continue;
                var member = em.GetComponentData<DynastyMemberComponent>(memberEntity);
                if (member.Role != DynastyRole.HeadOfBloodline) continue;
                if (member.Status == DynastyMemberStatus.Fallen ||
                    member.Status == DynastyMemberStatus.Captured) continue;
                return true;
            }
            return false;
        }

        private static void ApplyControlPointLoyaltyRecovery(
            EntityManager em,
            FixedString32Bytes factionId,
            float loyaltyDelta)
        {
            var q = em.CreateEntityQuery(ComponentType.ReadWrite<ControlPointComponent>());
            var cpEntities = q.ToEntityArray(Allocator.Temp);
            var cps = q.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);
            q.Dispose();

            for (int i = 0; i < cpEntities.Length; i++)
            {
                if (!cps[i].OwnerFactionId.Equals(factionId)) continue;
                var cp = cps[i];
                cp.Loyalty = math.clamp(cp.Loyalty + loyaltyDelta, 0f, 100f);
                em.SetComponentData(cpEntities[i], cp);
            }

            cpEntities.Dispose();
            cps.Dispose();
        }

        private static void PushDeclareInWorldTimeRequest(EntityManager em, int days)
        {
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<DualClockComponent>());
            if (q.IsEmpty) { q.Dispose(); return; }
            var clockEntity = q.GetSingletonEntity();
            q.Dispose();

            if (!em.HasBuffer<DeclareInWorldTimeRequest>(clockEntity)) return;
            var buffer = em.GetBuffer<DeclareInWorldTimeRequest>(clockEntity);
            buffer.Add(new DeclareInWorldTimeRequest
            {
                DaysDelta = days,
                Reason    = new FixedString64Bytes("Succession consolidated"),
            });
        }

        private static void PushNarrative(EntityManager em, FixedString32Bytes factionId)
        {
            var msg = new FixedString128Bytes();
            msg.Append(factionId);
            msg.Append((FixedString32Bytes)" consolidates the succession. Court loyalty begins to recover.");
            NarrativeMessageBridge.Push(em, msg, NarrativeMessageTone.Info);
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
