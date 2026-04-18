using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Systems
{
    /// <summary>
    /// Computes per-faction world pressure scores and advances the streak/level state
    /// once per canonical realm cycle (90 seconds). Applies loyalty and legitimacy
    /// consequences when a faction's pressure level is non-zero.
    ///
    /// Browser equivalent: updateWorldPressureEscalation (simulation.js:13709).
    ///
    /// Score sources ported in this slice:
    ///   territoryExpansion = max(0, ownedTerritories - 2)  -- simulation.js:13193
    ///   greatReckoning     = 4 when faction is the Great Reckoning target  -- sim:13186
    ///
    /// Dominant leader: faction with score >= 4 AND score strictly > all others.
    /// Streak increments by 1 each cycle as dominant leader; decrements to 0 otherwise.
    /// Level thresholds: streak >= 6 -> 3, >= 3 -> 2, >= 1 -> 1, else 0.
    ///
    /// Consequences per cycle (simulation.js:applyWorldPressureConsequences:13695):
    ///   - lowest-loyalty owned CP loses [level] loyalty (clamped to 0)
    ///   - if level >= 2: DynastyStateComponent.Legitimacy -= (level - 1) (clamped to 0)
    ///
    /// Runs before MatchProgressionEvaluationSystem so stage 5 convergence uses
    /// current-frame WorldPressure values (one-cycle lag on GreatReckoning is acceptable).
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(MatchProgressionEvaluationSystem))]
    public partial struct WorldPressureEscalationSystem : ISystem
    {
        private const int GreatReckoningPressureScore = 4;
        private const float DefaultCycleSeconds = 90f;

        public void OnCreate(ref SystemState state)
        {
            if (!SystemAPI.HasSingleton<WorldPressureCycleTrackerComponent>())
            {
                var trackerEntity = state.EntityManager.CreateEntity(typeof(WorldPressureCycleTrackerComponent));
                state.EntityManager.SetName(trackerEntity, "WorldPressureCycleTracker");
                state.EntityManager.SetComponentData(trackerEntity, new WorldPressureCycleTrackerComponent
                {
                    Accumulator = 0f,
                    CycleSeconds = DefaultCycleSeconds,
                });
            }

            state.RequireForUpdate<WorldPressureCycleTrackerComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.HasSingleton<WorldPressureCycleTrackerComponent>()) return;

            var em = state.EntityManager;
            float dt = SystemAPI.Time.DeltaTime;

            var trackerEntity = SystemAPI.GetSingletonEntity<WorldPressureCycleTrackerComponent>();
            var tracker = em.GetComponentData<WorldPressureCycleTrackerComponent>(trackerEntity);
            tracker.Accumulator += dt;
            bool runCycleUpdate = tracker.Accumulator >= tracker.CycleSeconds;
            if (runCycleUpdate)
                tracker.Accumulator -= tracker.CycleSeconds;
            em.SetComponentData(trackerEntity, tracker);

            // Read GreatReckoning signals from previous frame (one cycle lag, acceptable).
            bool greatReckoningActive = false;
            FixedString32Bytes greatReckoningTargetId = default;
            if (SystemAPI.HasSingleton<MatchProgressionComponent>())
            {
                var mp = SystemAPI.GetSingleton<MatchProgressionComponent>();
                greatReckoningActive = mp.GreatReckoningActive;
                greatReckoningTargetId = mp.GreatReckoningTargetFactionId;
            }

            // Build territory counts per faction.
            var cpQuery = em.CreateEntityQuery(ComponentType.ReadOnly<ControlPointComponent>());
            var cpData = cpQuery.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);
            cpQuery.Dispose();

            var territoryMap = new NativeHashMap<FixedString32Bytes, int>(8, Allocator.Temp);
            for (int i = 0; i < cpData.Length; i++)
            {
                var ownerId = cpData[i].OwnerFactionId;
                if (ownerId.Length == 0) continue;
                if (!territoryMap.TryGetValue(ownerId, out int cnt)) cnt = 0;
                territoryMap[ownerId] = cnt + 1;
            }
            cpData.Dispose();

            // Query kingdom faction entities that have WorldPressureComponent.
            var factionQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<FactionKindComponent>(),
                ComponentType.ReadWrite<WorldPressureComponent>());
            var factionEntities = factionQuery.ToEntityArray(Allocator.Temp);
            var factionComps = factionQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            var factionKinds = factionQuery.ToComponentDataArray<FactionKindComponent>(Allocator.Temp);
            var wpData = factionQuery.ToComponentDataArray<WorldPressureComponent>(Allocator.Temp);
            factionQuery.Dispose();

            // First pass: compute scores for all kingdom factions.
            var scores = new NativeArray<int>(factionEntities.Length, Allocator.Temp);
            var expansionScores = new NativeArray<int>(factionEntities.Length, Allocator.Temp);
            var grScores = new NativeArray<int>(factionEntities.Length, Allocator.Temp);

            for (int i = 0; i < factionEntities.Length; i++)
            {
                if (factionKinds[i].Kind != FactionKind.Kingdom) continue;
                var factionId = factionComps[i].FactionId;
                int territories = territoryMap.TryGetValue(factionId, out int t) ? t : 0;
                int expansion = math.max(0, territories - 2);
                int gr = (greatReckoningActive && greatReckoningTargetId == factionId) ? GreatReckoningPressureScore : 0;
                expansionScores[i] = expansion;
                grScores[i] = gr;
                scores[i] = expansion + gr;
            }
            territoryMap.Dispose();

            // Find dominant leader: score >= 4 and strictly highest.
            int dominantIdx = -1;
            int topScore = 0;
            int secondScore = 0;
            for (int i = 0; i < factionEntities.Length; i++)
            {
                if (factionKinds[i].Kind != FactionKind.Kingdom) continue;
                int s = scores[i];
                if (s > topScore) { secondScore = topScore; topScore = s; dominantIdx = i; }
                else if (s > secondScore) { secondScore = s; }
            }
            if (topScore < GreatReckoningPressureScore || topScore <= secondScore) dominantIdx = -1;

            // Second pass: update WorldPressureComponent for each kingdom faction.
            for (int i = 0; i < factionEntities.Length; i++)
            {
                if (factionKinds[i].Kind != FactionKind.Kingdom) continue;
                var entity = factionEntities[i];
                var wp = wpData[i];
                bool isLeader = (i == dominantIdx);

                wp.Score = scores[i];
                wp.TerritoryExpansionScore = expansionScores[i];
                wp.GreatReckoningScore = grScores[i];
                wp.Targeted = isLeader && scores[i] > 0;

                if (runCycleUpdate)
                {
                    wp.Streak = isLeader ? (wp.Streak + 1) : math.max(0, wp.Streak - 1);
                    wp.Level = wp.Streak >= 6 ? 3 : wp.Streak >= 3 ? 2 : wp.Streak >= 1 ? 1 : 0;
                }

                wp.Label = wp.Level switch
                {
                    3 => new FixedString32Bytes("Convergence"),
                    2 => new FixedString32Bytes("Overwhelming"),
                    1 => new FixedString32Bytes("Gathering"),
                    _ => new FixedString32Bytes("Quiet"),
                };

                em.SetComponentData(entity, wp);

                // Apply consequences once per cycle when level > 0.
                if (runCycleUpdate && wp.Level > 0)
                    ApplyConsequences(em, factionComps[i].FactionId, wp.Level);
            }

            scores.Dispose();
            expansionScores.Dispose();
            grScores.Dispose();
            factionEntities.Dispose();
            factionComps.Dispose();
            factionKinds.Dispose();
            wpData.Dispose();
        }

        private static void ApplyConsequences(EntityManager em, FixedString32Bytes factionId, int level)
        {
            // Find the lowest-loyalty CP owned by this faction and drain [level] loyalty.
            var cpQuery = em.CreateEntityQuery(ComponentType.ReadWrite<ControlPointComponent>());
            var cpEntities = cpQuery.ToEntityArray(Allocator.Temp);
            var cpData = cpQuery.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);
            cpQuery.Dispose();

            int lowestLoyaltyIdx = -1;
            float lowestLoyalty = float.MaxValue;
            for (int i = 0; i < cpData.Length; i++)
            {
                if (cpData[i].OwnerFactionId == factionId && cpData[i].Loyalty < lowestLoyalty)
                {
                    lowestLoyalty = cpData[i].Loyalty;
                    lowestLoyaltyIdx = i;
                }
            }
            if (lowestLoyaltyIdx >= 0)
            {
                var cp = cpData[lowestLoyaltyIdx];
                cp.Loyalty = math.clamp(cp.Loyalty - level, 0f, 100f);
                em.SetComponentData(cpEntities[lowestLoyaltyIdx], cp);
            }
            cpEntities.Dispose();
            cpData.Dispose();

            // Legitimacy consequence: if level >= 2, drain (level - 1) from DynastyStateComponent.
            if (level >= 2)
            {
                var dynastyQuery = em.CreateEntityQuery(
                    ComponentType.ReadOnly<FactionComponent>(),
                    ComponentType.ReadWrite<DynastyStateComponent>());
                var dynastyEntities = dynastyQuery.ToEntityArray(Allocator.Temp);
                var dynastyFactions = dynastyQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
                var dynastyComps = dynastyQuery.ToComponentDataArray<DynastyStateComponent>(Allocator.Temp);
                dynastyQuery.Dispose();

                for (int i = 0; i < dynastyEntities.Length; i++)
                {
                    if (dynastyFactions[i].FactionId != factionId) continue;
                    var ds = dynastyComps[i];
                    ds.Legitimacy = math.clamp(ds.Legitimacy - (level - 1), 0f, 100f);
                    em.SetComponentData(dynastyEntities[i], ds);
                    break;
                }
                dynastyEntities.Dispose();
                dynastyFactions.Dispose();
                dynastyComps.Dispose();
            }
        }
    }
}
