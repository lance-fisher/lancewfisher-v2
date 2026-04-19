using Bloodlines.Components;
using Bloodlines.Conviction;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.AI
{
    /// <summary>
    /// Consumes PactBreakRequestComponent entities and breaks the matching
    /// PactComponent. Ports simulation.js breakNonAggressionPact
    /// (~5224-5257).
    ///
    /// Browser canon has no AI auto-break path: pacts dissolve only on
    /// explicit call. This system mirrors that by requiring an external
    /// producer (test harness, future player-UI break action, or future
    /// AI policy system) to attach a PactBreakRequestComponent. There is
    /// no expiration timer; a pact past its MinimumExpiresAtInWorldDays
    /// remains active until explicitly broken, exactly as in the browser.
    ///
    /// Browser effects on break (simulation.js:5237-5250), all applied
    /// regardless of early-break status:
    ///   1. pact.brokenAt + brokenByFactionId are set on both sides.
    ///   2. ensureMutualHostility adds HostilityComponent entries between
    ///      the two factions if they are not already present.
    ///   3. faction.dynasty.legitimacy -= 8 clamped to [0, 100] on the
    ///      breaker (NON_AGGRESSION_PACT_BREAK_LEGITIMACY_COST = 8 at
    ///      simulation.js:5129).
    ///   4. faction.conviction.score -= 2 on the breaker, clamped to
    ///      [-100, 100]. Unity's conviction score is derived from bucket
    ///      values via ConvictionScoring.DeriveScore (Score =
    ///      Stewardship + Oathkeeping - Ruthlessness - Desecration), so
    ///      we write the penalty at the bucket layer: Oathkeeping -= 2.
    ///      This maps the browser's direct score decrement onto the
    ///      architecturally correct surface. Oathkeeping is the natural
    ///      bucket for pact-breaking: pacts are oaths, and breaking one
    ///      stains the oath-keeping ledger. ConvictionScoring.ApplyEvent
    ///      clamps the bucket at zero, so a faction with no remaining
    ///      oath-keeping to lose takes no additional penalty (a faction
    ///      already marked as an oath-breaker gains nothing from
    ///      breaking another pact, which matches the spirit of the
    ///      canon).
    ///   5. pushMessage: deferred until the AI-to-UI message bridge.
    ///
    /// The `earlyBreak` boolean that the browser returns only affects
    /// messaging ("The early breach damages legitimacy and conviction.");
    /// the mechanical penalty is identical regardless of timing. Unity
    /// matches that exactly: penalty applies always.
    ///
    /// Request entity is destroyed after processing so the same break
    /// request does not double-apply.
    ///
    /// Browser reference: simulation.js breakNonAggressionPact (~5224)
    /// and NON_AGGRESSION_PACT_BREAK_LEGITIMACY_COST constant (~5129).
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AIPactProposalExecutionSystem))]
    public partial struct PactBreakSystem : ISystem
    {
        public const float LegitimacyCost = 8f;
        public const float OathkeepingPenalty = 2f;
        public const float LegitimacyFloor = 0f;
        public const float LegitimacyCeiling = 100f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PactBreakRequestComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;

            var requestQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<PactBreakRequestComponent>());
            var requestEntities = requestQuery.ToEntityArray(Allocator.Temp);
            var requests        = requestQuery.ToComponentDataArray<PactBreakRequestComponent>(Allocator.Temp);
            requestQuery.Dispose();

            for (int i = 0; i < requestEntities.Length; i++)
            {
                var req = requests[i];
                ApplyBreak(em, req.PactId, req.RequestingFactionId);
                em.DestroyEntity(requestEntities[i]);
            }

            requestEntities.Dispose();
            requests.Dispose();
        }

        // ------------------------------------------------------------------ break

        private static void ApplyBreak(
            EntityManager em,
            FixedString64Bytes pactId,
            FixedString32Bytes requestingFactionId)
        {
            if (!TryFindPact(em, pactId, out Entity pactEntity, out PactComponent pact))
                return;
            if (pact.Broken)
                return;

            // Mark the pact broken.
            pact.Broken            = true;
            pact.BrokenByFactionId = requestingFactionId;
            em.SetComponentData(pactEntity, pact);

            // Re-establish mutual hostility between the two factions.
            var otherFactionId = pact.FactionAId.Equals(requestingFactionId)
                ? pact.FactionBId
                : pact.FactionAId;

            EnsureHostility(em, requestingFactionId, otherFactionId);
            EnsureHostility(em, otherFactionId, requestingFactionId);

            // Apply legitimacy + oathkeeping penalty on the breaker.
            var breakerEntity = FindFactionEntity(em, requestingFactionId);
            if (breakerEntity == Entity.Null) return;

            if (em.HasComponent<DynastyStateComponent>(breakerEntity))
            {
                var dynasty = em.GetComponentData<DynastyStateComponent>(breakerEntity);
                dynasty.Legitimacy = math.clamp(
                    dynasty.Legitimacy - LegitimacyCost, LegitimacyFloor, LegitimacyCeiling);
                em.SetComponentData(breakerEntity, dynasty);
            }

            if (em.HasComponent<ConvictionComponent>(breakerEntity))
            {
                var conviction = em.GetComponentData<ConvictionComponent>(breakerEntity);
                ConvictionScoring.ApplyEvent(
                    ref conviction,
                    ConvictionBucket.Oathkeeping,
                    -OathkeepingPenalty);
                em.SetComponentData(breakerEntity, conviction);
            }
        }

        // ------------------------------------------------------------------ helpers

        private static bool TryFindPact(
            EntityManager em,
            FixedString64Bytes pactId,
            out Entity pactEntity,
            out PactComponent pact)
        {
            pactEntity = Entity.Null;
            pact = default;

            var q = em.CreateEntityQuery(ComponentType.ReadOnly<PactComponent>());
            if (q.IsEmpty) { q.Dispose(); return false; }

            var entities = q.ToEntityArray(Allocator.Temp);
            var pacts    = q.ToComponentDataArray<PactComponent>(Allocator.Temp);
            q.Dispose();

            bool found = false;
            for (int i = 0; i < entities.Length; i++)
            {
                if (pacts[i].PactId.Equals(pactId))
                {
                    pactEntity = entities[i];
                    pact       = pacts[i];
                    found      = true;
                    break;
                }
            }
            entities.Dispose();
            pacts.Dispose();
            return found;
        }

        private static void EnsureHostility(
            EntityManager em,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId)
        {
            var sourceEntity = FindFactionEntity(em, sourceFactionId);
            if (sourceEntity == Entity.Null) return;
            if (!em.HasBuffer<HostilityComponent>(sourceEntity))
            {
                em.AddBuffer<HostilityComponent>(sourceEntity);
            }

            var buffer = em.GetBuffer<HostilityComponent>(sourceEntity);
            for (int i = 0; i < buffer.Length; i++)
                if (buffer[i].HostileFactionId.Equals(targetFactionId)) return;

            buffer.Add(new HostilityComponent { HostileFactionId = targetFactionId });
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
    }
}
