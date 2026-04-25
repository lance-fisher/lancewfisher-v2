using Bloodlines.Components;
using Bloodlines.Conviction;
using Bloodlines.GameTime;
using Bloodlines.TerritoryGovernance;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.AI
{
    /// <summary>
    /// Walks expired Assassination DynastyOperationComponent entities at
    /// ResolveAtInWorldDays and applies the canonical applyAssassinationEffect
    /// path from the browser (simulation.js:5390-5433, resolution at
    /// tickDynastyOperations ~5752-5830).
    ///
    /// Success path (SuccessScore >= 0):
    ///   - Void branch: target member no longer available (Fallen/Captured/
    ///     not on roster) -> finalize silently, no conviction effects.
    ///   - applyAssassinationEffect ports:
    ///       member.status = "fallen" -> DynastyMemberStatus.Fallen
    ///       member.fallenAt          -> FallenAtWorldSeconds = inWorldDays * 86400
    ///       conviction Ruthlessness+2 on source via ConvictionScoring.ApplyEvent
    ///       conviction Stewardship-1 on target if member was Governor
    ///       DynastyFallenLedger entry appended to target faction buffer
    ///       clearCommanderLinksFor / clearGovernorLinksFor (inline ECB removes)
    ///   - ensureMutualHostility between source and target factions
    ///   - Narrative push (browser simulation.js:5793-5800):
    ///       "[title] of [target] was assassinated by agents of [source]."
    ///       Tone: source==player -> Good, target==player -> Warn, else Info
    ///
    /// Failure path (SuccessScore < 0):
    ///   - Influence refund to target: max(8, round(escrowInfluence * 0.3))
    ///     (browser simulation.js:5804-5806)
    ///   - Conviction Stewardship+1 on target (simulation.js:5807-5810)
    ///   - Narrative push (simulation.js:5814-5820):
    ///       "[target] counter-intelligence intercepted an assassination cell
    ///       targeting [title]."
    ///       Tone: source==player -> Warn, target==player -> Good, else Info
    ///
    /// Deferred vs browser parity:
    ///   - Legitimacy loss on commander/governor kill: deferred (no canonical
    ///     Legitimacy field outside dynasty-core lane).
    ///   - applySuccessionRipple: deferred (complex succession logic; no
    ///     succession ripple system in this lane yet).
    ///   - tickMarriageDissolutionFromDeath: deferred (marriage dissolution
    ///     on death is handled by the dynasty-marriage-parity lane).
    ///   - recordCounterIntelligenceInterception on failure: deferred (the
    ///     counter-intel interception record lives in the player-covert-ops
    ///     lane; AI lane stores conviction effect only).
    ///
    /// Always flips DynastyOperationComponent.Active = false.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AIAssassinationExecutionSystem))]
    public partial struct AIAssassinationResolutionSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DynastyOperationComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;
            float inWorldDays = GetInWorldDays(em);
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            try
            {
                var q = em.CreateEntityQuery(
                    ComponentType.ReadOnly<DynastyOperationComponent>(),
                    ComponentType.ReadOnly<DynastyOperationAssassinationComponent>());
                var entities = q.ToEntityArray(Allocator.Temp);
                var ops      = q.ToComponentDataArray<DynastyOperationComponent>(Allocator.Temp);
                var assasns  = q.ToComponentDataArray<DynastyOperationAssassinationComponent>(Allocator.Temp);
                q.Dispose();

                for (int i = 0; i < entities.Length; i++)
                {
                    if (!ops[i].Active) continue;
                    if (ops[i].OperationKind != DynastyOperationKind.Assassination) continue;
                    if (assasns[i].ResolveAtInWorldDays > inWorldDays) continue;

                    ResolveOperation(em, ref ecb, ops[i], assasns[i], inWorldDays);

                    var op = ops[i];
                    op.Active = false;
                    ecb.SetComponent(entities[i], op);
                }

                entities.Dispose();
                ops.Dispose();
                assasns.Dispose();

                ecb.Playback(em);
            }
            finally
            {
                ecb.Dispose();
            }
        }

        // ------------------------------------------------------------------ resolution

        private static void ResolveOperation(
            EntityManager em,
            ref EntityCommandBuffer ecb,
            in DynastyOperationComponent op,
            in DynastyOperationAssassinationComponent assasn,
            float inWorldDays)
        {
            var sourceFactionEntity = FindFactionEntity(em, op.SourceFactionId);
            var targetFactionEntity = FindFactionEntity(em, assasn.TargetFactionId);

            if (targetFactionEntity == Entity.Null) return;

            bool targetMemberFound = TryResolveMemberEntity(
                em, targetFactionEntity, assasn.TargetMemberId,
                out var targetMemberEntity, out var targetMember);

            bool targetAvailable = targetMemberFound &&
                (targetMember.Status == DynastyMemberStatus.Active ||
                 targetMember.Status == DynastyMemberStatus.Ruling);

            if (!targetAvailable)
            {
                // Void: member gone before resolution; no effects
                return;
            }

            EnsureMutualHostility(em, op.SourceFactionId, assasn.TargetFactionId);

            if (assasn.SuccessScore >= 0f)
            {
                ApplyAssassinationEffect(em, ref ecb, sourceFactionEntity, targetFactionEntity,
                    targetMemberEntity, targetMember, op.SourceFactionId, assasn, inWorldDays);
                PushSuccessNarrative(em, op.SourceFactionId, assasn.TargetFactionId, assasn.TargetMemberTitle);
            }
            else
            {
                // Failure: refund influence to target, Stewardship+1 on target
                if (targetFactionEntity != Entity.Null &&
                    em.HasComponent<ResourceStockpileComponent>(targetFactionEntity))
                {
                    var stockpile = em.GetComponentData<ResourceStockpileComponent>(targetFactionEntity);
                    stockpile.Influence += math.max(8f, math.round(assasn.EscrowInfluence * 0.3f));
                    em.SetComponentData(targetFactionEntity, stockpile);
                }

                if (targetFactionEntity != Entity.Null &&
                    em.HasComponent<ConvictionComponent>(targetFactionEntity))
                {
                    var conviction = em.GetComponentData<ConvictionComponent>(targetFactionEntity);
                    ConvictionScoring.ApplyEvent(ref conviction, ConvictionBucket.Stewardship, 1f);
                    em.SetComponentData(targetFactionEntity, conviction);
                }

                PushFailureNarrative(em, op.SourceFactionId, assasn.TargetFactionId, assasn.TargetMemberTitle);
            }
        }

        private static void ApplyAssassinationEffect(
            EntityManager em,
            ref EntityCommandBuffer ecb,
            Entity sourceFactionEntity,
            Entity targetFactionEntity,
            Entity targetMemberEntity,
            DynastyMemberComponent targetMember,
            FixedString32Bytes sourceFactionId,
            in DynastyOperationAssassinationComponent assasn,
            float inWorldDays)
        {
            targetMember.Status = DynastyMemberStatus.Fallen;
            targetMember.FallenAtWorldSeconds = inWorldDays * 86400f;
            em.SetComponentData(targetMemberEntity, targetMember);

            // Legitimacy loss on commander/governor kill is deferred
            // (no canonical Legitimacy field outside dynasty-core lane).

            if (targetMember.Role == DynastyRole.Governor &&
                targetFactionEntity != Entity.Null &&
                em.HasComponent<ConvictionComponent>(targetFactionEntity))
            {
                var conviction = em.GetComponentData<ConvictionComponent>(targetFactionEntity);
                ConvictionScoring.ApplyEvent(ref conviction, ConvictionBucket.Stewardship, -1f);
                em.SetComponentData(targetFactionEntity, conviction);
            }

            if (sourceFactionEntity != Entity.Null &&
                em.HasComponent<ConvictionComponent>(sourceFactionEntity))
            {
                var conviction = em.GetComponentData<ConvictionComponent>(sourceFactionEntity);
                ConvictionScoring.ApplyEvent(ref conviction, ConvictionBucket.Ruthlessness, 2f);
                em.SetComponentData(sourceFactionEntity, conviction);
            }

            if (targetFactionEntity != Entity.Null &&
                em.HasBuffer<DynastyFallenLedger>(targetFactionEntity))
            {
                var fallen = em.GetBuffer<DynastyFallenLedger>(targetFactionEntity);
                fallen.Add(new DynastyFallenLedger
                {
                    MemberId             = targetMember.MemberId,
                    Title                = targetMember.Title,
                    Role                 = targetMember.Role,
                    FallenAtWorldSeconds = targetMember.FallenAtWorldSeconds,
                });
            }

            ClearCommanderLinks(em, ref ecb, assasn.TargetFactionId, targetMember.MemberId);
            ClearGovernorLinks(em, ref ecb, targetMember.MemberId);
        }

        // ------------------------------------------------------------------ link clearing

        private static void ClearCommanderLinks(
            EntityManager em,
            ref EntityCommandBuffer ecb,
            FixedString32Bytes factionId,
            FixedString64Bytes memberId)
        {
            var q = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<CommanderComponent>());
            if (q.IsEmpty) { q.Dispose(); return; }

            var entities   = q.ToEntityArray(Allocator.Temp);
            var factions   = q.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            var commanders = q.ToComponentDataArray<CommanderComponent>(Allocator.Temp);
            q.Dispose();

            for (int i = 0; i < entities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(factionId)) continue;
                if (!commanders[i].MemberId.Equals(memberId)) continue;

                ecb.RemoveComponent<CommanderComponent>(entities[i]);
                if (em.HasComponent<CommanderAuraComponent>(entities[i]))
                    ecb.RemoveComponent<CommanderAuraComponent>(entities[i]);
                if (em.HasComponent<CommanderAuraRecipientComponent>(entities[i]))
                    ecb.RemoveComponent<CommanderAuraRecipientComponent>(entities[i]);
                if (em.HasComponent<CommanderAtKeepTag>(entities[i]))
                    ecb.RemoveComponent<CommanderAtKeepTag>(entities[i]);
            }

            entities.Dispose();
            factions.Dispose();
            commanders.Dispose();
        }

        private static void ClearGovernorLinks(
            EntityManager em,
            ref EntityCommandBuffer ecb,
            FixedString64Bytes memberId)
        {
            var seatQ = em.CreateEntityQuery(
                ComponentType.ReadOnly<GovernorSeatAssignmentComponent>());
            if (!seatQ.IsEmpty)
            {
                var seatEntities = seatQ.ToEntityArray(Allocator.Temp);
                var seats        = seatQ.ToComponentDataArray<GovernorSeatAssignmentComponent>(Allocator.Temp);
                seatQ.Dispose();

                for (int i = 0; i < seatEntities.Length; i++)
                {
                    if (seats[i].GovernorMemberId.Equals(memberId))
                        ecb.RemoveComponent<GovernorSeatAssignmentComponent>(seatEntities[i]);
                }
                seatEntities.Dispose();
                seats.Dispose();
            }
            else
            {
                seatQ.Dispose();
            }

            var specQ = em.CreateEntityQuery(
                ComponentType.ReadOnly<GovernorSpecializationComponent>());
            if (!specQ.IsEmpty)
            {
                var specEntities = specQ.ToEntityArray(Allocator.Temp);
                var specs        = specQ.ToComponentDataArray<GovernorSpecializationComponent>(Allocator.Temp);
                specQ.Dispose();

                for (int i = 0; i < specEntities.Length; i++)
                {
                    if (specs[i].GovernorMemberId.Equals(memberId))
                        ecb.RemoveComponent<GovernorSpecializationComponent>(specEntities[i]);
                }
                specEntities.Dispose();
                specs.Dispose();
            }
            else
            {
                specQ.Dispose();
            }
        }

        // ------------------------------------------------------------------ hostility

        private static void EnsureMutualHostility(
            EntityManager em,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId)
        {
            var sourceEntity = FindFactionEntity(em, sourceFactionId);
            var targetEntity = FindFactionEntity(em, targetFactionId);
            if (sourceEntity == Entity.Null || targetEntity == Entity.Null) return;

            EnsureHostile(em, sourceEntity, targetFactionId);
            EnsureHostile(em, targetEntity, sourceFactionId);
        }

        private static void EnsureHostile(
            EntityManager em,
            Entity factionEntity,
            FixedString32Bytes hostileFactionId)
        {
            if (!em.HasBuffer<HostilityComponent>(factionEntity))
            {
                em.AddBuffer<HostilityComponent>(factionEntity);
            }

            var buffer = em.GetBuffer<HostilityComponent>(factionEntity);
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i].HostileFactionId.Equals(hostileFactionId)) return;
            }
            buffer.Add(new HostilityComponent { HostileFactionId = hostileFactionId });
        }

        // ------------------------------------------------------------------ narrative

        private static void PushSuccessNarrative(
            EntityManager em,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            FixedString64Bytes targetMemberTitle)
        {
            var message = new FixedString128Bytes();
            message.Append(targetMemberTitle);
            message.Append((FixedString32Bytes)" of ");
            message.Append(targetFactionId);
            message.Append((FixedString64Bytes)" was assassinated by agents of ");
            message.Append(sourceFactionId);
            message.Append((FixedString32Bytes)".");

            var playerId = new FixedString32Bytes("player");
            NarrativeMessageTone tone;
            if (sourceFactionId.Equals(playerId))
                tone = NarrativeMessageTone.Good;
            else if (targetFactionId.Equals(playerId))
                tone = NarrativeMessageTone.Warn;
            else
                tone = NarrativeMessageTone.Info;

            NarrativeMessageBridge.Push(em, message, tone);
        }

        private static void PushFailureNarrative(
            EntityManager em,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            FixedString64Bytes targetMemberTitle)
        {
            var message = new FixedString128Bytes();
            message.Append(targetFactionId);
            message.Append((FixedString64Bytes)" counter-intelligence intercepted an assassination cell targeting ");
            message.Append(targetMemberTitle);
            message.Append((FixedString32Bytes)".");

            var playerId = new FixedString32Bytes("player");
            NarrativeMessageTone tone;
            if (sourceFactionId.Equals(playerId))
                tone = NarrativeMessageTone.Warn;
            else if (targetFactionId.Equals(playerId))
                tone = NarrativeMessageTone.Good;
            else
                tone = NarrativeMessageTone.Info;

            NarrativeMessageBridge.Push(em, message, tone);
        }

        // ------------------------------------------------------------------ helpers

        private static bool TryResolveMemberEntity(
            EntityManager em,
            Entity factionEntity,
            FixedString64Bytes memberId,
            out Entity memberEntity,
            out DynastyMemberComponent member)
        {
            memberEntity = Entity.Null;
            member       = default;

            if (!em.HasBuffer<DynastyMemberRef>(factionEntity)) return false;
            var roster = em.GetBuffer<DynastyMemberRef>(factionEntity);

            for (int i = 0; i < roster.Length; i++)
            {
                var e = roster[i].Member;
                if (e == Entity.Null) continue;
                if (!em.HasComponent<DynastyMemberComponent>(e)) continue;

                var m = em.GetComponentData<DynastyMemberComponent>(e);
                if (!m.MemberId.Equals(memberId)) continue;

                memberEntity = e;
                member       = m;
                return true;
            }
            return false;
        }

        private static Entity FindFactionEntity(EntityManager em, FixedString32Bytes factionId)
        {
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<FactionComponent>());
            var entities = q.ToEntityArray(Allocator.Temp);
            var factions = q.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            q.Dispose();

            Entity match = Entity.Null;
            for (int i = 0; i < entities.Length; i++)
            {
                if (factions[i].FactionId.Equals(factionId)) { match = entities[i]; break; }
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
