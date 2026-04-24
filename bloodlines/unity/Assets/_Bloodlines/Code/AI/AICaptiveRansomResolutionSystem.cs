using Bloodlines.Components;
using Bloodlines.Conviction;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Walks expired CaptiveRansom DynastyOperationComponent entities at
    /// ResolveAtInWorldDays and applies the canonical browser ransom
    /// resolution effects from simulation.js:5838-5858.
    ///
    /// Ransom is deterministic (ProjectedChance = 1.0 per browser
    /// simulation.js:4964). Resolution fires only on the expiry tick;
    /// there is no failure roll. On resolution:
    ///   - Returns EscrowCost resources to the captor faction's
    ///     ResourceStockpileComponent (browser grantResources at
    ///     simulation.js:5840).
    ///   - Calls CapturedMemberHelpers.ReleaseCaptive to flip the captive
    ///     Status -> Released on the captor faction's buffer.
    ///   - Records Oathkeeping +1 conviction event on the SOURCE faction
    ///     (browser simulation.js:5846 "Ransomed X home").
    ///   - Records Stewardship +1 conviction event on the CAPTOR faction
    ///     (browser simulation.js:5847 "Extracted ransom for X").
    ///   - Pushes ransom narrative (Good if source==player, Info otherwise;
    ///     browser simulation.js:5851).
    ///
    /// Void path: if the captive is no longer found on the captor's buffer,
    /// the resource refund and conviction events are still applied (the
    /// captor is still owed the ransom) but no narrative push occurs.
    ///
    /// Always flips DynastyOperationComponent.Active = false regardless of
    /// outcome so the capacity slot releases back to the per-faction cap
    /// (sub-slice 18).
    ///
    /// Deferred:
    ///   - Legitimacy recovery on resolution (LEGITIMACY_RECOVERY_ON_RANSOM
    ///     at simulation.js:5845; adjustLegitimacy; no canonical Legitimacy
    ///     field outside dynasty-core lane).
    ///   - Return of the member to the source faction's dynasty roster
    ///     (browser releaseCapturedMember re-integrates the member; Unity
    ///     defers roster mutation until DynastyMemberComponent gains a
    ///     captivity-state field and the release flow is formalized).
    ///
    /// Updates after AICaptiveRansomExecutionSystem so dispatch and
    /// resolution cannot happen in the same tick.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AICaptiveRansomExecutionSystem))]
    public partial struct AICaptiveRansomResolutionSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DynastyOperationComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;
            float inWorldDays = GetInWorldDays(em);
            TickRansomResolutions(em, inWorldDays);
        }

        private static void TickRansomResolutions(EntityManager em, float inWorldDays)
        {
            var q = em.CreateEntityQuery(
                ComponentType.ReadWrite<DynastyOperationComponent>(),
                ComponentType.ReadOnly<DynastyOperationCaptiveRansomComponent>());
            var entities = q.ToEntityArray(Allocator.Temp);
            var ops      = q.ToComponentDataArray<DynastyOperationComponent>(Allocator.Temp);
            var ransoms  = q.ToComponentDataArray<DynastyOperationCaptiveRansomComponent>(Allocator.Temp);
            q.Dispose();

            for (int i = 0; i < entities.Length; i++)
            {
                if (!ops[i].Active) continue;
                if (ops[i].OperationKind != DynastyOperationKind.CaptiveRansom) continue;
                if (ransoms[i].ResolveAtInWorldDays > inWorldDays) continue;

                ResolveOperation(em, ops[i], ransoms[i]);

                var op = ops[i];
                op.Active = false;
                em.SetComponentData(entities[i], op);
            }

            entities.Dispose();
            ops.Dispose();
            ransoms.Dispose();
        }

        private static void ResolveOperation(
            EntityManager em,
            DynastyOperationComponent op,
            DynastyOperationCaptiveRansomComponent ransom)
        {
            // Captor always receives escrow resources (browser grantResources at :5840).
            GrantResourcesToCaptor(em, op.TargetFactionId, ransom.EscrowCost);

            bool released = CapturedMemberHelpers.ReleaseCaptive(
                em,
                op.TargetFactionId,
                ransom.CaptiveMemberId,
                CapturedMemberStatus.Released);

            // Conviction events fire regardless of released status (escrow already paid).
            RecordConvictionOnFaction(em, op.SourceFactionId, ConvictionBucket.Oathkeeping, 1f);
            RecordConvictionOnFaction(em, op.TargetFactionId, ConvictionBucket.Stewardship, 1f);

            if (released)
                PushRansomMessage(em, op.SourceFactionId, op.TargetFactionId, ransom.CaptiveMemberTitle);
        }

        private static void GrantResourcesToCaptor(
            EntityManager em,
            FixedString32Bytes captorFactionId,
            DynastyOperationEscrowCost escrow)
        {
            var captorEntity = FindFactionEntity(em, captorFactionId);
            if (captorEntity == Entity.Null) return;
            if (!em.HasComponent<ResourceStockpileComponent>(captorEntity)) return;
            var stock = em.GetComponentData<ResourceStockpileComponent>(captorEntity);
            stock.Gold      += escrow.Gold;
            stock.Influence += escrow.Influence;
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

        private static void PushRansomMessage(
            EntityManager em,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes captorFactionId,
            FixedString64Bytes memberTitle)
        {
            var message = new FixedString128Bytes();
            message.Append(memberTitle);
            message.Append((FixedString64Bytes)" returns to ");
            message.Append(sourceFactionId);
            message.Append((FixedString64Bytes)" under ransom terms.");

            var playerId = new FixedString32Bytes("player");
            NarrativeMessageTone tone = sourceFactionId.Equals(playerId)
                ? NarrativeMessageTone.Good
                : NarrativeMessageTone.Info;

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
