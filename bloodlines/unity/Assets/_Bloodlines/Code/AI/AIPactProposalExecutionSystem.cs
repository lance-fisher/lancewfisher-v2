using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Consumes AICovertOpsComponent.LastFiredOp == CovertOpKind.PactProposal
    /// (written by AICovertOpsSystem in sub-slice 6) and executes the
    /// browser-side proposeNonAggressionPact path
    /// (simulation.js ~5185-5222). Browser dispatch site:
    /// ai.js updateEnemyAi pact block (~2643-2666).
    ///
    /// Browser proposeNonAggressionPact has no separate accept stage. The
    /// proposing faction's call instantly establishes the pact when terms
    /// pass. Unity matches that semantic: this system performs the
    /// terms check and writes the active pact entity in a single update.
    /// "Proposal" in the system name reflects the dispatch label
    /// (CovertOpKind.PactProposal) and the browser function name.
    ///
    /// Gates ported from getNonAggressionPactTerms (~5150-5183):
    ///   1. Source and target are kingdoms (FactionKind.Kingdom).
    ///   2. Source != target.
    ///   3. Source is hostile to target (HostilityComponent buffer match).
    ///   4. No existing PactComponent already links the two factions.
    ///   5. Source can afford 50 Influence + 80 Gold from
    ///      ResourceStockpileComponent.
    ///
    /// Gate intentionally deferred:
    ///   - Active holy war check. Unity has no holy-war system component
    ///     yet (browser getFaithHolyWarsState + getIncomingHolyWars).
    ///     When the holy-war lane lands the gate can be added without
    ///     reshaping this system.
    ///
    /// Effects on success:
    ///   - Deduct 50 Influence and 80 Gold from source ResourceStockpile.
    ///   - Remove HostilityComponent buffer entries both ways
    ///     (simulation.js removeMutualHostility).
    ///   - Create one PactComponent entity with FactionAId/FactionBId,
    ///     StartedAtInWorldDays = current, MinimumExpiresAtInWorldDays =
    ///     current + 180.
    ///
    /// Always clears LastFiredOp to None after processing regardless of
    /// outcome so one dispatch produces one execution attempt, matching
    /// the one-shot pattern shared with sub-slices 8, 9, 12, and 13.
    ///
    /// Browser reference: ai.js pact proposal block ~2643-2666 +
    /// simulation.js getNonAggressionPactTerms (~5150) and
    /// proposeNonAggressionPact (~5185); constants
    /// NON_AGGRESSION_PACT_INFLUENCE_COST = 50,
    /// NON_AGGRESSION_PACT_GOLD_COST = 80,
    /// NON_AGGRESSION_PACT_MINIMUM_DURATION_IN_WORLD_DAYS = 180
    /// at simulation.js ~5126-5128.
    ///
    /// Sub-slice 17 addition:
    ///   Narrative message push. After the pact entity is created, call
    ///     NarrativeMessageBridge.Push with the browser-parity line at
    ///     simulation.js:5216-5220: "<sourceFactionId> and
    ///     <targetFactionId> enter a non-aggression pact. Hostility ceases
    ///     for at least 180 in-world days." Tone is Good when the source
    ///     is "player", else Info.
    ///
    /// Deferred to later slices:
    ///   - Holy-war pact gate (waits on the holy-war lane).
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AICovertOpsSystem))]
    public partial struct AIPactProposalExecutionSystem : ISystem
    {
        public const float InfluenceCost = 50f;
        public const float GoldCost = 80f;
        public const float MinimumDurationInWorldDays = 180f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AICovertOpsComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;
            float inWorldDays = GetInWorldDays(em);

            var dispatchQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadWrite<AICovertOpsComponent>());
            var dispatchEntities = dispatchQuery.ToEntityArray(Allocator.Temp);
            dispatchQuery.Dispose();

            for (int i = 0; i < dispatchEntities.Length; i++)
            {
                var sourceEntity = dispatchEntities[i];
                var covert = em.GetComponentData<AICovertOpsComponent>(sourceEntity);
                if (covert.LastFiredOp != CovertOpKind.PactProposal)
                    continue;

                var sourceFaction = em.GetComponentData<FactionComponent>(sourceEntity);

                // Target is hardcoded as "player" matching the browser
                // ai.js dispatch (`proposeNonAggressionPact(state, "enemy", "player")`).
                var targetFactionId = new FixedString32Bytes("player");

                TryProposePact(em, sourceEntity, sourceFaction.FactionId,
                    targetFactionId, inWorldDays);

                covert.LastFiredOp = CovertOpKind.None;
                em.SetComponentData(sourceEntity, covert);
            }

            dispatchEntities.Dispose();
        }

        // ------------------------------------------------------------------ pact

        private static void TryProposePact(
            EntityManager em,
            Entity sourceEntity,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            float inWorldDays)
        {
            // Gate 1: same faction guard.
            if (sourceFactionId.Equals(targetFactionId)) return;

            // Gate 2: both kingdoms.
            if (!em.HasComponent<FactionKindComponent>(sourceEntity)) return;
            if (em.GetComponentData<FactionKindComponent>(sourceEntity).Kind != FactionKind.Kingdom)
                return;

            var targetEntity = FindFactionEntity(em, targetFactionId);
            if (targetEntity == Entity.Null) return;
            if (!em.HasComponent<FactionKindComponent>(targetEntity)) return;
            if (em.GetComponentData<FactionKindComponent>(targetEntity).Kind != FactionKind.Kingdom)
                return;

            // Gate 3: source must be hostile to target. Browser areHostile
            // checks both sides; we walk the source's HostilityComponent
            // buffer to match the existing Unity hostility surface.
            if (!IsHostile(em, sourceEntity, targetFactionId)) return;

            // Gate 4: no existing pact between the two factions.
            if (HasActivePact(em, sourceFactionId, targetFactionId)) return;

            // Gate 5: source can afford Influence + Gold cost.
            if (!em.HasComponent<ResourceStockpileComponent>(sourceEntity)) return;
            var resources = em.GetComponentData<ResourceStockpileComponent>(sourceEntity);
            if (resources.Influence < InfluenceCost) return;
            if (resources.Gold < GoldCost) return;

            // Effects: deduct cost, clear hostility both ways, create
            // pact entity.
            resources.Influence -= InfluenceCost;
            resources.Gold      -= GoldCost;
            em.SetComponentData(sourceEntity, resources);

            DropHostility(em, sourceEntity, targetFactionId);
            DropHostility(em, targetEntity, sourceFactionId);

            var pactEntity = em.CreateEntity(typeof(PactComponent));
            em.SetComponentData(pactEntity, new PactComponent
            {
                PactId                       = BuildPactId(sourceFactionId, targetFactionId, inWorldDays),
                FactionAId                   = sourceFactionId,
                FactionBId                   = targetFactionId,
                StartedAtInWorldDays         = inWorldDays,
                MinimumExpiresAtInWorldDays  = inWorldDays + MinimumDurationInWorldDays,
                Broken                       = false,
                BrokenByFactionId            = default,
            });

            // Narrative push (sub-slice 17). Browser pushMessage at
            // simulation.js:5216-5220.
            PushPactEnteredMessage(em, sourceFactionId, targetFactionId);
        }

        private static void PushPactEnteredMessage(
            EntityManager em,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId)
        {
            var message = new FixedString128Bytes();
            message.Append(sourceFactionId);
            message.Append((FixedString32Bytes)" and ");
            message.Append(targetFactionId);
            message.Append((FixedString64Bytes)" enter a non-aggression pact. Hostility ceases for at least ");
            message.Append((int)MinimumDurationInWorldDays);
            message.Append((FixedString32Bytes)" in-world days.");

            var tone = sourceFactionId.Equals(new FixedString32Bytes("player"))
                ? NarrativeMessageTone.Good
                : NarrativeMessageTone.Info;

            NarrativeMessageBridge.Push(em, message, tone);
        }

        // ------------------------------------------------------------------ helpers

        private static bool IsHostile(EntityManager em, Entity factionEntity, FixedString32Bytes targetFactionId)
        {
            if (!em.HasBuffer<HostilityComponent>(factionEntity)) return false;
            var buffer = em.GetBuffer<HostilityComponent>(factionEntity);
            for (int i = 0; i < buffer.Length; i++)
                if (buffer[i].HostileFactionId.Equals(targetFactionId)) return true;
            return false;
        }

        private static bool HasActivePact(
            EntityManager em,
            FixedString32Bytes a,
            FixedString32Bytes b)
        {
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<PactComponent>());
            if (q.IsEmpty) { q.Dispose(); return false; }

            var pacts = q.ToComponentDataArray<PactComponent>(Allocator.Temp);
            q.Dispose();

            bool found = false;
            for (int i = 0; i < pacts.Length; i++)
            {
                var p = pacts[i];
                if (p.Broken) continue;
                bool abMatch = p.FactionAId.Equals(a) && p.FactionBId.Equals(b);
                bool baMatch = p.FactionAId.Equals(b) && p.FactionBId.Equals(a);
                if (abMatch || baMatch) { found = true; break; }
            }
            pacts.Dispose();
            return found;
        }

        private static void DropHostility(
            EntityManager em,
            Entity sourceEntity,
            FixedString32Bytes targetFactionId)
        {
            if (!em.HasBuffer<HostilityComponent>(sourceEntity)) return;
            var buffer = em.GetBuffer<HostilityComponent>(sourceEntity);
            for (int i = buffer.Length - 1; i >= 0; i--)
                if (buffer[i].HostileFactionId.Equals(targetFactionId))
                    buffer.RemoveAt(i);
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

        private static FixedString64Bytes BuildPactId(
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            float inWorldDays)
        {
            var id = new FixedString64Bytes("nap-");
            id.Append(sourceFactionId);
            id.Append("-");
            id.Append(targetFactionId);
            id.Append("-d");
            id.Append((int)inWorldDays);
            return id;
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
