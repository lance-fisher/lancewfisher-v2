using Bloodlines.AI;
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
    ///   Stage 2 (expansion_identity) -- food stable, water stable, defended seat
    ///     (live: HealthComponent.Current > 0), 4+ buildings.
    ///   Stage 3 (encounter_establishment) -- faith committed, 2+ territories,
    ///     6+ military (count includes any movement-capable unit; canonical filter
    ///     to combat-only is deferred to its own slice; see
    ///     reports/2026-04-25_match_progression_stage_gate_cross_audit.md D3-3).
    ///   Stage 4 (war_turning_of_tides) -- rival contact (direct front contact OR
    ///     contested border), contested border (active CP capture OR
    ///     WorldPressure.Level > 0 for player/enemy), sustained war (active siege
    ///     engine OR active holy war OR active divine-right declaration OR
    ///     active dynasty operation).
    ///   Stage 5 (final_convergence) -- 3 separate requirements per browser:
    ///     (1) convergence: any kingdom WorldPressure.Targeted && Level >= 3
    ///         (player or enemy) OR active divine-right declaration.
    ///     (2) sovereignty: dominantTerritoryShare >= 0.75 OR highestFaithLevel >= 5.
    ///     (3) late dynastic time: inWorldYears >= 12.
    ///
    /// Great Reckoning: triggers when dominant kingdom holds >= 70% of all kingdom
    /// territories. Releases when share falls below 66%.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(DualClockTickSystem))]
    public partial struct MatchProgressionEvaluationSystem : ISystem
    {
        // Constants hoisted to MatchProgressionCanon (see
        // docs/migration/constant_parity_audit.md 2026-04-25). Values unchanged
        // from browser canonical (simulation.js:404-405).
        private const float GreatReckoningTriggerShare = MatchProgressionCanon.GreatReckoningTriggerShare;
        private const float GreatReckoningReleaseShare = MatchProgressionCanon.GreatReckoningReleaseShare;

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
            // Browser parity: defended seat requires building.completed AND building.health > 0.
            // The Exclude<ConstructionStateComponent> filter handles the completed gate;
            // the HealthComponent.Current > 0 gate handles the health gate.
            var completedBuildingQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<BuildingTypeComponent>(),
                ComponentType.Exclude<ConstructionStateComponent>());
            var completedBuildingEntities = completedBuildingQuery.ToEntityArray(Allocator.Temp);
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
                bool isSeatType = typeId == new FixedString64Bytes("command_hall") ||
                    typeId == new FixedString64Bytes("barracks") ||
                    typeId == new FixedString64Bytes("watch_tower") ||
                    typeId == new FixedString64Bytes("keep_tier_1") ||
                    typeId == new FixedString64Bytes("gatehouse");
                if (!isSeatType) continue;
                var seatEntity = completedBuildingEntities[i];
                bool seatAlive = !em.HasComponent<HealthComponent>(seatEntity) ||
                    em.GetComponentData<HealthComponent>(seatEntity).Current > 0f;
                if (seatAlive) playerHasDefendedSeat = true;
            }
            completedBuildingEntities.Dispose();
            completedBuildingFactions.Dispose();
            completedBuildingTypes.Dispose();

            // --- Player living military units (combat-capable only) ---
            // Browser parity: getAliveCombatUnits filters role !== "worker" AND
            // !isSupportRole (engineer-specialist + support). The Stage 3 "field
            // army fit for rival contact" requirement uses this filter so worker
            // populations do not count as soldiers. Without this filter the
            // worker-slot economy lane would auto-unlock Stage 3 once 6 villagers
            // exist, which is canonically wrong.
            var militaryQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<MovementStatsComponent>(),
                ComponentType.ReadOnly<HealthComponent>(),
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.Exclude<DeadTag>());
            var militaryFactions = militaryQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            var militaryTypes = militaryQuery.ToComponentDataArray<UnitTypeComponent>(Allocator.Temp);
            militaryQuery.Dispose();

            int playerMilitaryCount = 0;
            for (int i = 0; i < militaryFactions.Length; i++)
            {
                if (militaryFactions[i].FactionId != playerFactionId) continue;
                var role = militaryTypes[i].Role;
                if (role == UnitRole.Worker ||
                    role == UnitRole.Support ||
                    role == UnitRole.EngineerSpecialist)
                {
                    continue;
                }
                playerMilitaryCount++;
            }
            militaryFactions.Dispose();
            militaryTypes.Dispose();

            // --- World pressure signals (browser getWorldPressureTargetProfile + getWorldPressureConvergenceProfile) ---
            // Stage 4 contestedBorder fallback: playerWorldPressure.level > 0 OR enemyWorldPressure.level > 0.
            // Stage 5 convergence signal: any kingdom with Targeted=true AND Level >= 3 satisfies playerConvergence
            // OR enemyConvergence (browser checks both factions' convergence profiles).
            bool playerWorldPressureLevelOver0 = false;
            bool enemyWorldPressureLevelOver0 = false;
            bool anyWorldPressureConvergence = false;
            var enemyFactionIdForPressure = new FixedString32Bytes("enemy");
            {
                var wpQ = em.CreateEntityQuery(
                    ComponentType.ReadOnly<FactionComponent>(),
                    ComponentType.ReadOnly<WorldPressureComponent>());
                var wpFactions = wpQ.ToComponentDataArray<FactionComponent>(Allocator.Temp);
                var wpComps = wpQ.ToComponentDataArray<WorldPressureComponent>(Allocator.Temp);
                wpQ.Dispose();
                for (int i = 0; i < wpFactions.Length; i++)
                {
                    var fid = wpFactions[i].FactionId;
                    var wp = wpComps[i];
                    if (fid == playerFactionId && wp.Targeted && wp.Level > 0) playerWorldPressureLevelOver0 = true;
                    if (fid == enemyFactionIdForPressure && wp.Targeted && wp.Level > 0) enemyWorldPressureLevelOver0 = true;
                    if (wp.Targeted && wp.Level >= 3) anyWorldPressureConvergence = true;
                }
                wpFactions.Dispose();
                wpComps.Dispose();
            }

            // --- Stage 4 rival contact signals (browser getRivalContactProfile) ---
            // directFrontContact: player unit within 220 units of any enemy unit.
            // contestedBorder: any CP owned by one faction and being captured by the other,
            //                  OR (per browser fallback) playerWorldPressure.level > 0,
            //                  OR enemyWorldPressure.level > 0.
            // sustainedWarActive: any active siege engine (entity with SiegeEngineStateComponent)
            //                     OR any kingdom with at least one ActiveHolyWarElement
            //                     OR any active divine-right declaration
            //                     (DynastyOperationDivineRightComponent on an Active operation)
            //                     OR any active dynasty operation
            //                     (DynastyOperationComponent.Active==true).
            //                     Replaces the old 1-in-world-year proxy now that the
            //                     supporting systems (siege, holy war, divine right,
            //                     dynasty ops) are live in the runtime.

            bool stageFourRivalContact;
            bool stageFourContestedBorder;
            bool stageFourSustainedWar;
            bool stageFiveConvergenceFromDivineRight = false;
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

                // sustainedWarActive: union of canonical signals (siege engines, holy wars,
                // divine right, dynasty ops). Each is a per-entity tag/buffer presence check.
                bool sustainedSiegeActive = false;
                {
                    var siegeQ = em.CreateEntityQuery(ComponentType.ReadOnly<SiegeEngineStateComponent>());
                    if (siegeQ.CalculateEntityCount() > 0) sustainedSiegeActive = true;
                    siegeQ.Dispose();
                }
                bool sustainedHolyWarActive = false;
                {
                    var hwQ = em.CreateEntityQuery(ComponentType.ReadOnly<ActiveHolyWarElement>());
                    using var hwEntities = hwQ.ToEntityArray(Allocator.Temp);
                    for (int i = 0; i < hwEntities.Length; i++)
                    {
                        var buf = em.GetBuffer<ActiveHolyWarElement>(hwEntities[i]);
                        if (buf.Length > 0) { sustainedHolyWarActive = true; break; }
                    }
                    hwQ.Dispose();
                }
                bool sustainedDivineRightActive = false;
                bool sustainedDynastyOpActive = false;
                {
                    var opQ = em.CreateEntityQuery(ComponentType.ReadOnly<DynastyOperationComponent>());
                    using var opData = opQ.ToComponentDataArray<DynastyOperationComponent>(Allocator.Temp);
                    using var opEntities = opQ.ToEntityArray(Allocator.Temp);
                    for (int i = 0; i < opData.Length; i++)
                    {
                        if (!opData[i].Active) continue;
                        sustainedDynastyOpActive = true;
                        if (em.HasComponent<DynastyOperationDivineRightComponent>(opEntities[i]))
                            sustainedDivineRightActive = true;
                    }
                    opQ.Dispose();
                }

                stageFourRivalContact = directFrontContact || contestedBorderFound;
                stageFourContestedBorder = contestedBorderFound ||
                    playerWorldPressureLevelOver0 || enemyWorldPressureLevelOver0;
                stageFourSustainedWar = sustainedSiegeActive || sustainedHolyWarActive ||
                    sustainedDivineRightActive || sustainedDynastyOpActive;

                // Track active divine-right declarations separately for the Stage 5
                // convergence requirement. Browser includes activeDivineRightDeclarations > 0
                // as a convergence signal in addition to player/enemy world pressure.
                stageFiveConvergenceFromDivineRight = sustainedDivineRightActive;
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

            // Stage 5 (browser computeMatchProgressionState canonical 3-req split):
            //   1. drive the world toward final convergence: any kingdom with WorldPressure
            //      Targeted && Level >= 3 (player or enemy) OR any active divine-right
            //      declaration.
            //   2. create a true sovereignty contender: dominant share >= 0.75 OR highest
            //      faith level >= 5.
            //   3. carry the war into late dynastic time: inWorldYears >= 12.
            // All three must be true. Browser does not collapse convergence into sovereignty.
            bool stageFiveConvergence = anyWorldPressureConvergence || stageFiveConvergenceFromDivineRight;
            bool stageFiveSovereignty = dominantTerritoryShare >= 0.75f || highestFaithLevel >= 5;
            bool stageFiveYears = inWorldYears >= 12f;
            bool stageFiveReady = stageFourReady && stageFiveConvergence &&
                stageFiveSovereignty && stageFiveYears;

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
                int met = BoolToInt(stageFiveConvergence) + BoolToInt(stageFiveSovereignty) +
                    BoolToInt(stageFiveYears);
                stageReadiness = met / 3f;
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
            else if (stageNumber >= 3 || (stageNumber == 2 && stageThreeReadiness >= MatchProgressionCanon.CommitmentPhaseStageThreeReadinessThreshold))
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
