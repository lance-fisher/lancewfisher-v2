using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Systems
{
    /// <summary>
    /// Evaluates the five-stage match-progression state each simulation tick and
    /// writes the result to the MatchProgressionComponent singleton.
    /// Browser equivalent: updateMatchProgressionState / computeMatchProgressionState
    /// (simulation.js:13557, 13426). Runs after DualClockTickSystem so InWorldDays
    /// is already advanced for this frame.
    ///
    /// Stage evaluation (canonical browser spec):
    ///   Stage 1 (founding) -- default.
    ///   Stage 2 (expansion_identity) -- food stable, water stable, defended seat, 4+ buildings.
    ///   Stage 3 (encounter_establishment) -- faith committed, 2+ territories, 6+ military.
    ///   Stage 4 (war_turning_of_tides) -- rival contact + contested border + sustained war.
    ///     [Stage 4 war signals remain false until the declaration-seam sub-slice is ported.]
    ///   Stage 5 (final_convergence) -- convergence active OR dominant share >= 0.75,
    ///     highest faith level >= 5, in-world years >= 12.
    ///     Convergence active = player WorldPressureComponent.Targeted && Level >= 3.
    ///
    /// Great Reckoning: triggers when dominant kingdom holds >= 70% of all kingdom
    /// territories. Releases when share falls below 66%.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(DualClockTickSystem))]
    public partial struct MatchProgressionEvaluationSystem : ISystem
    {
        private const float GreatReckoningTriggerShare = 0.7f;
        private const float GreatReckoningReleaseShare = 0.66f;

        public void OnCreate(ref SystemState state)
        {
            if (!SystemAPI.HasSingleton<MatchProgressionComponent>())
            {
                var entity = state.EntityManager.CreateEntity(typeof(MatchProgressionComponent));
                state.EntityManager.SetName(entity, "MatchProgression");
                state.EntityManager.SetComponentData(entity, new MatchProgressionComponent
                {
                    StageNumber = 1,
                    StageId = new FixedString32Bytes("founding"),
                    StageLabel = new FixedString64Bytes("Founding"),
                    PhaseId = new FixedString32Bytes("emergence"),
                    PhaseLabel = new FixedString32Bytes("Emergence"),
                    StageReadiness = 0f,
                    NextStageId = new FixedString32Bytes("expansion_identity"),
                    NextStageLabel = new FixedString64Bytes("Expansion and Identity"),
                    InWorldDays = 0f,
                    InWorldYears = 0f,
                    DeclarationCount = 0,
                    RivalContactActive = false,
                    SustainedWarActive = false,
                    GreatReckoningActive = false,
                    GreatReckoningTargetFactionId = default,
                    GreatReckoningShare = 0f,
                    GreatReckoningThreshold = GreatReckoningTriggerShare,
                    DominantKingdomId = default,
                    DominantTerritoryShare = 0f,
                });
            }
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;

            if (!SystemAPI.HasSingleton<DualClockComponent>() ||
                !SystemAPI.HasSingleton<MatchProgressionComponent>())
                return;

            var clock = SystemAPI.GetSingleton<DualClockComponent>();
            float inWorldDays = clock.InWorldDays;
            float inWorldYears = inWorldDays / 365f;

            // --- Territory counts per faction (for dominant-share computation) ---
            var cpQuery = em.CreateEntityQuery(ComponentType.ReadOnly<ControlPointComponent>());
            var cpData = cpQuery.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);
            cpQuery.Dispose();

            var territoryMap = new NativeHashMap<FixedString32Bytes, int>(8, Allocator.Temp);
            for (int i = 0; i < cpData.Length; i++)
            {
                var ownerId = cpData[i].OwnerFactionId;
                if (ownerId.Length == 0) continue;
                if (!territoryMap.TryGetValue(ownerId, out int count)) count = 0;
                territoryMap[ownerId] = count + 1;
            }
            cpData.Dispose();

            // --- Faction-level data: resources, faith, per-kingdom territory counts ---
            bool playerFoodStable = false;
            bool playerWaterStable = false;
            bool playerFaithCommitted = false;
            FixedString32Bytes dominantKingdomId = default;
            int dominantTerritoryCount = 0;
            int kingdomTerritoryTotal = 0;
            int highestFaithLevel = 0;

            var factionQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<FactionKindComponent>());
            var factionEntities = factionQuery.ToEntityArray(Allocator.Temp);
            var factionComps = factionQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            var factionKinds = factionQuery.ToComponentDataArray<FactionKindComponent>(Allocator.Temp);
            factionQuery.Dispose();

            var playerFactionId = new FixedString32Bytes("player");

            for (int i = 0; i < factionEntities.Length; i++)
            {
                var entity = factionEntities[i];
                var factionId = factionComps[i].FactionId;
                if (factionKinds[i].Kind != FactionKind.Kingdom) continue;

                int tc = territoryMap.TryGetValue(factionId, out int tv) ? tv : 0;
                kingdomTerritoryTotal += tc;
                if (tc > dominantTerritoryCount)
                {
                    dominantTerritoryCount = tc;
                    dominantKingdomId = factionId;
                }

                if (em.HasComponent<FaithStateComponent>(entity))
                {
                    var faith = em.GetComponentData<FaithStateComponent>(entity);
                    if (faith.Level > highestFaithLevel) highestFaithLevel = faith.Level;
                    if (factionId == playerFactionId)
                        playerFaithCommitted = faith.SelectedFaith != CovenantId.None;
                }

                if (factionId == playerFactionId &&
                    em.HasComponent<ResourceStockpileComponent>(entity) &&
                    em.HasComponent<PopulationComponent>(entity))
                {
                    var res = em.GetComponentData<ResourceStockpileComponent>(entity);
                    var pop = em.GetComponentData<PopulationComponent>(entity);
                    float surplus = pop.Total + 6f;
                    playerFoodStable = res.Food >= surplus;
                    playerWaterStable = res.Water >= surplus;
                }
            }

            factionEntities.Dispose();
            factionComps.Dispose();
            factionKinds.Dispose();
            territoryMap.Dispose();

            float dominantTerritoryShare = kingdomTerritoryTotal > 0
                ? (float)dominantTerritoryCount / kingdomTerritoryTotal
                : 0f;
            int playerKingdomTerritories = 0;
            {
                var q2 = em.CreateEntityQuery(ComponentType.ReadOnly<ControlPointComponent>());
                var cp2 = q2.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);
                q2.Dispose();
                for (int i = 0; i < cp2.Length; i++)
                    if (cp2[i].OwnerFactionId == playerFactionId) playerKingdomTerritories++;
                cp2.Dispose();
            }

            // --- Player completed buildings (no ConstructionStateComponent) ---
            var completedBuildingQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<BuildingTypeComponent>(),
                ComponentType.Exclude<ConstructionStateComponent>());
            var completedBuildingFactions = completedBuildingQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            var completedBuildingTypes = completedBuildingQuery.ToComponentDataArray<BuildingTypeComponent>(Allocator.Temp);
            completedBuildingQuery.Dispose();

            int playerCompletedBuildings = 0;
            bool playerHasDefendedSeat = false;
            for (int i = 0; i < completedBuildingFactions.Length; i++)
            {
                if (completedBuildingFactions[i].FactionId != playerFactionId) continue;
                playerCompletedBuildings++;
                var typeId = completedBuildingTypes[i].TypeId;
                if (typeId == new FixedString64Bytes("command_hall") ||
                    typeId == new FixedString64Bytes("barracks") ||
                    typeId == new FixedString64Bytes("watch_tower") ||
                    typeId == new FixedString64Bytes("keep_tier_1") ||
                    typeId == new FixedString64Bytes("gatehouse"))
                {
                    playerHasDefendedSeat = true;
                }
            }
            completedBuildingFactions.Dispose();
            completedBuildingTypes.Dispose();

            // --- Player living military units ---
            var militaryQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<MovementStatsComponent>(),
                ComponentType.ReadOnly<HealthComponent>(),
                ComponentType.Exclude<DeadTag>());
            var militaryFactions = militaryQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            militaryQuery.Dispose();

            int playerMilitaryCount = 0;
            for (int i = 0; i < militaryFactions.Length; i++)
                if (militaryFactions[i].FactionId == playerFactionId) playerMilitaryCount++;
            militaryFactions.Dispose();

            // --- Stage 5 world-pressure convergence signal (browser getWorldPressureConvergenceProfile) ---
            // Active when the player faction has Targeted=true and Level >= 3.
            bool playerWorldPressureConvergence = false;
            {
                var wpQ = em.CreateEntityQuery(
                    ComponentType.ReadOnly<FactionComponent>(),
                    ComponentType.ReadOnly<WorldPressureComponent>());
                var wpFactions = wpQ.ToComponentDataArray<FactionComponent>(Allocator.Temp);
                var wpComps = wpQ.ToComponentDataArray<WorldPressureComponent>(Allocator.Temp);
                wpQ.Dispose();
                for (int i = 0; i < wpFactions.Length; i++)
                    if (wpFactions[i].FactionId == playerFactionId && wpComps[i].Targeted && wpComps[i].Level >= 3)
                        playerWorldPressureConvergence = true;
                wpFactions.Dispose();
                wpComps.Dispose();
            }

            // --- Stage 4 rival contact signals (browser getRivalContactProfile) ---
            // directFrontContact: player unit within 220 units of any enemy unit.
            // contestedBorder: any CP owned by one faction and being captured by the other.
            // sustainedWarActive: contestedBorder active AND in-world years >= 1 (proxy;
            //   browser checks active siege engines + holy wars + dynasty ops -- those
            //   systems port in later slices; this proxy is removed when siege is live).

            bool stageFourRivalContact;
            bool stageFourContestedBorder;
            bool stageFourSustainedWar;
            {
                var enemyFactionId = new FixedString32Bytes("enemy");
                const float ContactDistanceSq = 220f * 220f;

                // directFrontContact: O(n*m) position scan, acceptable for <=200 units.
                var unitQuery = em.CreateEntityQuery(
                    ComponentType.ReadOnly<FactionComponent>(),
                    ComponentType.ReadOnly<PositionComponent>(),
                    ComponentType.ReadOnly<MovementStatsComponent>(),
                    ComponentType.Exclude<DeadTag>());
                var unitFactions = unitQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
                var unitPositions = unitQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
                unitQuery.Dispose();

                bool directFrontContact = false;
                for (int i = 0; i < unitFactions.Length && !directFrontContact; i++)
                {
                    if (unitFactions[i].FactionId != playerFactionId) continue;
                    for (int j = 0; j < unitFactions.Length && !directFrontContact; j++)
                    {
                        if (unitFactions[j].FactionId != enemyFactionId) continue;
                        float distSq = math.distancesq(unitPositions[i].Value, unitPositions[j].Value);
                        if (distSq <= ContactDistanceSq) directFrontContact = true;
                    }
                }
                unitFactions.Dispose();
                unitPositions.Dispose();

                // contestedBorder: CP owned by player being captured by enemy, or vice versa.
                var cpContestedQuery = em.CreateEntityQuery(ComponentType.ReadOnly<ControlPointComponent>());
                var cpContestedData = cpContestedQuery.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);
                cpContestedQuery.Dispose();

                bool contestedBorderFound = false;
                for (int i = 0; i < cpContestedData.Length; i++)
                {
                    var cp = cpContestedData[i];
                    if ((cp.OwnerFactionId == playerFactionId && cp.CaptureFactionId == enemyFactionId) ||
                        (cp.OwnerFactionId == enemyFactionId && cp.CaptureFactionId == playerFactionId))
                    {
                        contestedBorderFound = true;
                        break;
                    }
                }
                cpContestedData.Dispose();

                stageFourRivalContact = directFrontContact || contestedBorderFound;
                stageFourContestedBorder = contestedBorderFound;
                // sustainedWarActive proxy: contested border present AND enough in-world time
                // has elapsed to represent a prolonged conflict (>= 1 in-world year).
                stageFourSustainedWar = contestedBorderFound && inWorldYears >= 1f;
            }

            // --- Stage requirement evaluation (browser computeMatchProgressionState) ---

            // Stage 2: food stable, water stable, defended seat, 4+ buildings.
            bool stageTwoFoodStable = playerFoodStable;
            bool stageTwoWaterStable = playerWaterStable;
            bool stageTwoDefendedHome = playerHasDefendedSeat;
            bool stageTwoFoundingBuildout = playerCompletedBuildings >= 4;
            bool stageTwoReady = stageTwoFoodStable && stageTwoWaterStable &&
                                 stageTwoDefendedHome && stageTwoFoundingBuildout;

            // Stage 3: faith committed, 2+ territories, 6+ military.
            bool stageThreeFaithCommitted = playerFaithCommitted;
            bool stageThreeExpanded = playerKingdomTerritories >= 2;
            bool stageThreeArmed = playerMilitaryCount >= 6;
            bool stageThreeReady = stageTwoReady && stageThreeFaithCommitted &&
                                   stageThreeExpanded && stageThreeArmed;

            // Stage 4: rival contact + contested border + sustained war.
            bool stageFourReady = stageThreeReady && stageFourRivalContact &&
                                  stageFourContestedBorder && stageFourSustainedWar;

            // Stage 5: dominant share >= 0.75 OR highest faith >= 5 OR world pressure convergence active.
            bool stageFiveConvergence = dominantTerritoryShare >= 0.75f || highestFaithLevel >= 5 || playerWorldPressureConvergence;
            bool stageFiveYears = inWorldYears >= 12f;
            bool stageFiveReady = stageFourReady && stageFiveConvergence && stageFiveYears;

            int stageNumber = stageFiveReady ? 5
                : stageFourReady ? 4
                : stageThreeReady ? 3
                : stageTwoReady ? 2
                : 1;

            // Stage readiness (fraction of next-stage requirements met, browser spec).
            float stageReadiness;
            if (stageNumber == 1)
            {
                int met = BoolToInt(stageTwoFoodStable) + BoolToInt(stageTwoWaterStable) +
                          BoolToInt(stageTwoDefendedHome) + BoolToInt(stageTwoFoundingBuildout);
                stageReadiness = met / 4f;
            }
            else if (stageNumber == 2)
            {
                int met = BoolToInt(stageThreeFaithCommitted) + BoolToInt(stageThreeExpanded) +
                          BoolToInt(stageThreeArmed);
                stageReadiness = met / 3f;
            }
            else if (stageNumber == 3)
            {
                int met = BoolToInt(stageFourRivalContact) + BoolToInt(stageFourContestedBorder) +
                          BoolToInt(stageFourSustainedWar);
                stageReadiness = met / 3f;
            }
            else if (stageNumber == 4)
            {
                int met = BoolToInt(stageFiveConvergence) + BoolToInt(stageFiveYears);
                stageReadiness = met / 2f;
            }
            else
            {
                stageReadiness = 1f;
            }

            // Phase (browser: emergence -> commitment -> resolution).
            float stageThreeReadiness = (BoolToInt(stageThreeFaithCommitted) +
                BoolToInt(stageThreeExpanded) + BoolToInt(stageThreeArmed)) / 3f;
            FixedString32Bytes phaseId;
            FixedString32Bytes phaseLabel;
            if (stageNumber >= 5) { phaseId = new FixedString32Bytes("resolution"); phaseLabel = new FixedString32Bytes("Resolution"); }
            else if (stageNumber >= 3 || (stageNumber == 2 && stageThreeReadiness >= 0.67f))
            { phaseId = new FixedString32Bytes("commitment"); phaseLabel = new FixedString32Bytes("Commitment"); }
            else { phaseId = new FixedString32Bytes("emergence"); phaseLabel = new FixedString32Bytes("Emergence"); }

            // Stage descriptors.
            FixedString32Bytes stageId;
            FixedString64Bytes stageLabel;
            FixedString32Bytes nextStageId;
            FixedString64Bytes nextStageLabel;
            if (stageNumber == 1)
            { stageId = new FixedString32Bytes("founding"); stageLabel = new FixedString64Bytes("Founding"); nextStageId = new FixedString32Bytes("expansion_identity"); nextStageLabel = new FixedString64Bytes("Expansion and Identity"); }
            else if (stageNumber == 2)
            { stageId = new FixedString32Bytes("expansion_identity"); stageLabel = new FixedString64Bytes("Expansion and Identity"); nextStageId = new FixedString32Bytes("encounter_establishment"); nextStageLabel = new FixedString64Bytes("Encounter and Establishment"); }
            else if (stageNumber == 3)
            { stageId = new FixedString32Bytes("encounter_establishment"); stageLabel = new FixedString64Bytes("Encounter and Establishment"); nextStageId = new FixedString32Bytes("war_turning_of_tides"); nextStageLabel = new FixedString64Bytes("War and the Turning of Tides"); }
            else if (stageNumber == 4)
            { stageId = new FixedString32Bytes("war_turning_of_tides"); stageLabel = new FixedString64Bytes("War and the Turning of Tides"); nextStageId = new FixedString32Bytes("final_convergence"); nextStageLabel = new FixedString64Bytes("Final Convergence"); }
            else
            { stageId = new FixedString32Bytes("final_convergence"); stageLabel = new FixedString64Bytes("Final Convergence"); nextStageId = default; nextStageLabel = default; }

            // --- Great Reckoning ---
            var prev = SystemAPI.GetSingleton<MatchProgressionComponent>();
            bool prevGreatReckoningActive = prev.GreatReckoningActive;
            FixedString32Bytes prevGreatReckoningTargetId = prev.GreatReckoningTargetFactionId;

            FixedString32Bytes greatReckoningTargetId = stageNumber >= 4 ? dominantKingdomId : default;
            float greatReckoningShare = greatReckoningTargetId.Length > 0 ? dominantTerritoryShare : 0f;
            bool sustainReckoning = prevGreatReckoningActive &&
                prevGreatReckoningTargetId == greatReckoningTargetId &&
                greatReckoningShare >= GreatReckoningReleaseShare;
            bool triggerReckoning = greatReckoningTargetId.Length > 0 &&
                greatReckoningShare >= GreatReckoningTriggerShare;
            bool greatReckoningActive = triggerReckoning || sustainReckoning;

            var mp = new MatchProgressionComponent
            {
                StageNumber = stageNumber,
                StageId = stageId,
                StageLabel = stageLabel,
                PhaseId = phaseId,
                PhaseLabel = phaseLabel,
                StageReadiness = stageReadiness,
                NextStageId = nextStageId,
                NextStageLabel = nextStageLabel,
                InWorldDays = inWorldDays,
                InWorldYears = inWorldYears,
                DeclarationCount = clock.DeclarationCount,
                RivalContactActive = stageFourRivalContact,
                SustainedWarActive = stageFourSustainedWar,
                GreatReckoningActive = greatReckoningActive,
                GreatReckoningTargetFactionId = greatReckoningActive ? greatReckoningTargetId : default,
                GreatReckoningShare = greatReckoningActive ? greatReckoningShare : 0f,
                GreatReckoningThreshold = GreatReckoningTriggerShare,
                DominantKingdomId = dominantKingdomId,
                DominantTerritoryShare = dominantTerritoryShare,
            };

            var mpEntity = SystemAPI.GetSingletonEntity<MatchProgressionComponent>();
            em.SetComponentData(mpEntity, mp);
        }

        private static int BoolToInt(bool b) => b ? 1 : 0;
    }
}
