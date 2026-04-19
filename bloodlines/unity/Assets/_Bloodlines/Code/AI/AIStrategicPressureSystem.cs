using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Applies strategic timer pressure overrides to AI factions each frame.
    ///
    /// Ports the pressure-clamp, stage-gate, and governance/event-context clamp
    /// block from ai.js updateEnemyAi (lines 1127-1241 and 1129-1215).
    /// Responsibilities:
    ///   1. Decrement per-frame countdown timers (attack, territory, raid, build).
    ///   2. Derive RivalryUnlocked / RaidPressureUnlocked from match stage and
    ///      world-pressure overrides.
    ///   3. Clamp timers downward via world-pressure level and GreatReckoning focus.
    ///   4. Floor territory/raid timers upward when stage gates are closed.
    ///   5. Apply governance / event-context timer clamps:
    ///        holy war, player governance recognition, player covenant test,
    ///        player divine right, player/enemy succession crisis,
    ///        enemy covenant test, enemy governance recognition.
    ///
    /// Browser equivalents:
    ///   Lines 1127-1241 (timer clamp/floor, stage gates, WP clamps).
    ///   Lines 1129-1215 (governance/event-context clamps).
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(EnemyAIStrategySystem))]
    public partial struct AIStrategicPressureSystem : ISystem
    {
        // Canonical timer ceilings enforced when stage gates are closed.
        private const float TerritoryTimerRivalryFloor    = 24f; // stage 1: hold back hard
        private const float TerritoryTimerStage2Floor     = 16f; // stage 2, no pressure: moderate
        private const float RaidTimerNoPressureFloor      = 20f; // no raid-pressure: hold raids back

        // World-pressure-driven attack/territory clamps (ai.js ~1218-1219).
        private const float AttackTimerPressureLevel1Cap  = 14f;
        private const float AttackTimerPressureLevel2Cap  =  8f;
        private const float TerritoryTimerPressureLevel1Cap = 10f;
        private const float TerritoryTimerPressureLevel2Cap =  6f;

        // Great-reckoning-source clamps (ai.js ~1227-1229).
        private const float AttackTimerGreatReckoningCap    =  6f;
        private const float TerritoryTimerGreatReckoningCap =  4f;
        private const float RaidTimerGreatReckoningCap      =  8f;
        private const int   GreatReckoningPressureScore     =  4;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AIStrategyComponent>();
            state.RequireForUpdate<MatchProgressionComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;
            float dt = SystemAPI.Time.DeltaTime;
            if (dt <= 0f)
                dt = 0.016f;

            int stageNumber = SystemAPI.GetSingleton<MatchProgressionComponent>().StageNumber;

            // Read player faction world pressure (read-only, no structural change).
            int playerWpLevel = 0;
            bool playerGreatReckoningFocused = false;
            var wpQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<WorldPressureComponent>());
            var wpFactions = wpQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            var wpComps    = wpQuery.ToComponentDataArray<WorldPressureComponent>(Allocator.Temp);
            wpQuery.Dispose();
            for (int k = 0; k < wpFactions.Length; k++)
            {
                if (wpFactions[k].FactionId == "player")
                {
                    playerWpLevel = wpComps[k].Level;
                    playerGreatReckoningFocused = wpComps[k].GreatReckoningScore >= GreatReckoningPressureScore
                                                 && wpComps[k].Level >= 2;
                    break;
                }
            }
            wpFactions.Dispose();
            wpComps.Dispose();

            // Stage gate booleans shared across all AI factions.
            bool rivalryPressureOverride = playerWpLevel > 0;
            bool raidPressureOverride    = rivalryPressureOverride;
            bool rivalryUnlocked         = stageNumber >= 2 || rivalryPressureOverride;
            bool raidPressureUnlocked    = stageNumber >= 3 || raidPressureOverride;

            // Update each AI faction.
            var factionQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<AIEconomyControllerComponent>(),
                typeof(AIStrategyComponent));
            var entities   = factionQuery.ToEntityArray(Allocator.Temp);
            var strategies = factionQuery.ToComponentDataArray<AIStrategyComponent>(Allocator.Temp);
            factionQuery.Dispose();

            for (int i = 0; i < entities.Length; i++)
            {
                var s = strategies[i];

                // 1. Decrement countdown timers.
                s.AttackTimer    -= dt;
                s.TerritoryTimer -= dt;
                s.RaidTimer      -= dt;
                s.BuildTimer     -= dt;

                // 2. Write derived stage gates.
                s.RivalryUnlocked      = rivalryUnlocked;
                s.RaidPressureUnlocked = raidPressureUnlocked;

                // 3. World-pressure downward clamps (makes AI more aggressive).
                if (playerWpLevel > 0)
                {
                    float attackCap    = playerWpLevel >= 2 ? AttackTimerPressureLevel2Cap    : AttackTimerPressureLevel1Cap;
                    float territoryCap = playerWpLevel >= 2 ? TerritoryTimerPressureLevel2Cap : TerritoryTimerPressureLevel1Cap;
                    s.AttackTimer    = math_min(s.AttackTimer,    attackCap);
                    s.TerritoryTimer = math_min(s.TerritoryTimer, territoryCap);
                }
                if (playerGreatReckoningFocused)
                {
                    s.AttackTimer    = math_min(s.AttackTimer,    AttackTimerGreatReckoningCap);
                    s.TerritoryTimer = math_min(s.TerritoryTimer, TerritoryTimerGreatReckoningCap);
                    s.RaidTimer      = math_min(s.RaidTimer,      RaidTimerGreatReckoningCap);
                }

                // 4. Stage-gate upward floors (prevents AI from acting before gates open).
                if (!rivalryUnlocked)
                {
                    s.TerritoryTimer = math_max(s.TerritoryTimer, TerritoryTimerRivalryFloor);
                }
                else if (!rivalryPressureOverride && stageNumber == 2)
                {
                    s.TerritoryTimer = math_max(s.TerritoryTimer, TerritoryTimerStage2Floor);
                }
                if (!raidPressureUnlocked)
                {
                    s.RaidTimer = math_max(s.RaidTimer, RaidTimerNoPressureFloor);
                }

                // 5. Governance / event-context clamps (ai.js lines 1129-1215).
                ApplyGovernancePressure(ref s, dt);

                em.SetComponentData(entities[i], s);
            }
            entities.Dispose();
            strategies.Dispose();
        }

        // ------------------------------------------------------------------ governance

        // Governance/event-context timer constants (ai.js lines 1129-1215).
        // Each constant is the cap or floor applied under the named condition.

        // Holy war (either direction) -- lines 1129-1132.
        private const float HolyWarAttackCap    = 10f;
        private const float HolyWarTerritoryCap =  8f;

        // Player governance recognition -- lines 1143-1148.
        private const float GovVictoryAttackCap    = 3f;
        private const float GovAllianceAttackCap   = 4f;
        private const float GovActiveAttackCap     = 5f;
        private const float GovVictoryTerritoryCap = 2f;
        private const float GovAllianceTerritoryCap= 3f;
        private const float GovActiveTerritoryCap  = 4f;
        private const float GovVictoryRaidCap      = 4f;
        private const float GovAllianceRaidCap     = 5f;
        private const float GovActiveRaidCap       = 6f;
        private const float GovVictoryAssassinCap  = 3f;
        private const float GovAllianceAssassinCap = 4f;
        private const float GovActiveAssassinCap   = 6f;
        private const float GovVictoryMissionaryCap= 3f;
        private const float GovAllianceMissionaryCap=4f;
        private const float GovActiveMissionaryCap = 5f;
        private const float GovVictoryHolyWarCap   = 4f;
        private const float GovAllianceHolyWarCap  = 5f;
        private const float GovActiveHolyWarCap    = 6f;

        // Player covenant test -- lines 1151-1154.
        private const float CovenantTargetsAttackCap    = 4f;
        private const float CovenantActiveAttackCap     = 6f;
        private const float CovenantTargetsTerritoryCap = 4f;
        private const float CovenantActiveTerritoryCap  = 6f;
        private const float CovenantTargetsRaidCap      = 5f;
        private const float CovenantActiveRaidCap       = 7f;
        private const float CovenantTargetsHolyWarCap   = 6f;
        private const float CovenantActiveHolyWarCap    = 12f;

        // Player divine right -- lines 1157-1159.
        private const float DivineRightAttackCap    = 5f;
        private const float DivineRightTerritoryCap = 4f;
        private const float DivineRightRaidCap      = 7f;

        // Player succession crisis -- lines 1163-1165.
        private const float PlayerCrisisHighAttackCap    = 5f;
        private const float PlayerCrisisNormAttackCap    = 7f;
        private const float PlayerCrisisHighTerritoryCap = 4f;
        private const float PlayerCrisisNormTerritoryCap = 6f;
        private const float PlayerCrisisMarriageCap      = 18f;

        // Enemy succession crisis -- lines 1169-1184.
        private const float EnemyCrisisSevereAttackFloor  = 16f;
        private const float EnemyCrisisNormAttackFloor    = 12f;
        private const float EnemyCrisisSevereTerritoryFloor=14f;
        private const float EnemyCrisisNormTerritoryFloor  =10f;
        private const float EnemyCrisisMarriageCap        = 14f;
        private const float EnemyCrisisConsolidateReset   = 60f;
        private const float EnemyCrisisRetryDelay         = 18f;
        private const float EnemyCrisisInitialDelay       = 12f;

        // Enemy covenant test -- lines 1186-1196.
        private const float EnemyCovenantBuildCap          = 2.5f;
        private const float EnemyCovenantWithTargetTerrCap = 5f;
        private const float EnemyCovenantNoTargetTerrCap   = 8f;
        private const float EnemyCovenantPlayerAttackCap   = 5f;
        private const float EnemyCovenantPlayerRaidCap     = 6f;
        private const float EnemyCovenantPlayerHolyWarCap  = 8f;
        private const float EnemyCovenantDarkPurgeHolyWarCap=4f;

        // Enemy governance recognition -- lines 1198-1214.
        // Territory caps depend on whether a target control point exists.
        private const float EnemyGovVictoryTerrWithTargetCap   = 3f;
        private const float EnemyGovAllianceTerrWithTargetCap  = 4f;
        private const float EnemyGovActiveTerrWithTargetCap    = 5f;
        private const float EnemyGovNoTargetTerrCap            = 8f;
        private const float EnemyGovBuildCap                   = 2.5f;
        // Enemy governance floors attack/holyWar UPWARD (AI pulls back to consolidate).
        private const float EnemyGovVictoryAttackFloor         = 14f;
        private const float EnemyGovAllianceAttackFloor        = 13f;
        private const float EnemyGovActiveAttackFloor          = 12f;
        private const float EnemyGovVictoryHolyWarFloor        = 24f;
        private const float EnemyGovAllianceHolyWarFloor       = 20f;
        private const float EnemyGovActiveHolyWarFloor         = 18f;

        /// <summary>
        /// Applies the governance/event-context timer clamp block from ai.js
        /// updateEnemyAi lines 1129-1215. Reads flags written by dynasty, faith,
        /// and governance systems (or seeded directly in tests).
        /// </summary>
        private static void ApplyGovernancePressure(ref AIStrategyComponent s, float dt)
        {
            // --- holy war (either direction) (lines 1129-1132) ---
            if (s.HolyWarActive)
            {
                s.AttackTimer    = math_min(s.AttackTimer,    HolyWarAttackCap);
                s.TerritoryTimer = math_min(s.TerritoryTimer, HolyWarTerritoryCap);
            }

            // --- player territorial governance recognition (lines 1133-1149) ---
            if (s.PlayerGovernanceActive)
            {
                float attackCap = s.PlayerGovernanceVictoryPressure  ? GovVictoryAttackCap
                                : s.PlayerGovernanceAlliancePressure ? GovAllianceAttackCap
                                :                                       GovActiveAttackCap;
                float terrCap   = s.PlayerGovernanceVictoryPressure  ? GovVictoryTerritoryCap
                                : s.PlayerGovernanceAlliancePressure ? GovAllianceTerritoryCap
                                :                                       GovActiveTerritoryCap;
                float raidCap   = s.PlayerGovernanceVictoryPressure  ? GovVictoryRaidCap
                                : s.PlayerGovernanceAlliancePressure ? GovAllianceRaidCap
                                :                                       GovActiveRaidCap;
                float assassinCap = s.PlayerGovernanceVictoryPressure  ? GovVictoryAssassinCap
                                  : s.PlayerGovernanceAlliancePressure ? GovAllianceAssassinCap
                                  :                                       GovActiveAssassinCap;
                float missionaryCap = s.PlayerGovernanceVictoryPressure  ? GovVictoryMissionaryCap
                                    : s.PlayerGovernanceAlliancePressure ? GovAllianceMissionaryCap
                                    :                                       GovActiveMissionaryCap;
                float holyWarCap = s.PlayerGovernanceVictoryPressure  ? GovVictoryHolyWarCap
                                 : s.PlayerGovernanceAlliancePressure ? GovAllianceHolyWarCap
                                 :                                       GovActiveHolyWarCap;
                s.AttackTimer       = math_min(s.AttackTimer,       attackCap);
                s.TerritoryTimer    = math_min(s.TerritoryTimer,    terrCap);
                s.RaidTimer         = math_min(s.RaidTimer,         raidCap);
                s.AssassinationTimer= math_min(s.AssassinationTimer,assassinCap);
                s.MissionaryTimer   = math_min(s.MissionaryTimer,   missionaryCap);
                s.HolyWarTimer      = math_min(s.HolyWarTimer,      holyWarCap);
            }

            // --- player covenant test (lines 1150-1155) ---
            if (s.PlayerCovenantActive)
            {
                s.AttackTimer    = math_min(s.AttackTimer,    s.PlayerCovenantTargetsEnemy ? CovenantTargetsAttackCap    : CovenantActiveAttackCap);
                s.TerritoryTimer = math_min(s.TerritoryTimer, s.PlayerCovenantTargetsEnemy ? CovenantTargetsTerritoryCap : CovenantActiveTerritoryCap);
                s.RaidTimer      = math_min(s.RaidTimer,      s.PlayerCovenantTargetsEnemy ? CovenantTargetsRaidCap      : CovenantActiveRaidCap);
                s.HolyWarTimer   = math_min(s.HolyWarTimer,   s.PlayerCovenantTargetsEnemy ? CovenantTargetsHolyWarCap   : CovenantActiveHolyWarCap);
            }

            // --- player divine right (lines 1156-1160) ---
            if (s.PlayerDivineRightActive)
            {
                s.AttackTimer    = math_min(s.AttackTimer,    DivineRightAttackCap);
                s.TerritoryTimer = math_min(s.TerritoryTimer, DivineRightTerritoryCap);
                s.RaidTimer      = math_min(s.RaidTimer,      DivineRightRaidCap);
            }

            // --- player succession crisis (lines 1161-1165) ---
            if (s.PlayerSuccessionCrisisActive)
            {
                s.AttackTimer          = math_min(s.AttackTimer,          s.PlayerSuccessionCrisisHigh ? PlayerCrisisHighAttackCap    : PlayerCrisisNormAttackCap);
                s.TerritoryTimer       = math_min(s.TerritoryTimer,       s.PlayerSuccessionCrisisHigh ? PlayerCrisisHighTerritoryCap : PlayerCrisisNormTerritoryCap);
                s.MarriageProposalTimer= math_min(s.MarriageProposalTimer, PlayerCrisisMarriageCap);
            }

            // --- enemy succession crisis (lines 1167-1185) ---
            if (s.EnemySuccessionCrisisActive)
            {
                // Floors push timers UPWARD: enemy is distracted, pulls back.
                s.AttackTimer          = math_max(s.AttackTimer,          s.EnemySuccessionCrisisSevere ? EnemyCrisisSevereAttackFloor   : EnemyCrisisNormAttackFloor);
                s.TerritoryTimer       = math_max(s.TerritoryTimer,       s.EnemySuccessionCrisisSevere ? EnemyCrisisSevereTerritoryFloor : EnemyCrisisNormTerritoryFloor);
                s.MarriageProposalTimer= math_min(s.MarriageProposalTimer, EnemyCrisisMarriageCap);
                // Consolidation accumulator tick (consolidateSuccessionCrisis call
                // is deferred until dynasty system is ported; this tracks the timer).
                s.EnemySuccessionCrisisAccumulator -= dt;
                if (s.EnemySuccessionCrisisAccumulator <= 0f)
                    s.EnemySuccessionCrisisAccumulator = EnemyCrisisRetryDelay;
            }
            else
            {
                s.EnemySuccessionCrisisAccumulator = EnemyCrisisInitialDelay;
            }

            // --- enemy covenant test (lines 1186-1197) ---
            if (s.EnemyCovenantActive)
            {
                s.BuildTimer     = math_min(s.BuildTimer,     EnemyCovenantBuildCap);
                s.TerritoryTimer = math_min(s.TerritoryTimer, s.EnemyCovenantHasTargetPoint ? EnemyCovenantWithTargetTerrCap : EnemyCovenantNoTargetTerrCap);
                if (s.EnemyCovenantTargetsPlayer)
                {
                    s.AttackTimer  = math_min(s.AttackTimer,  EnemyCovenantPlayerAttackCap);
                    s.RaidTimer    = math_min(s.RaidTimer,     EnemyCovenantPlayerRaidCap);
                    s.HolyWarTimer = math_min(s.HolyWarTimer,  EnemyCovenantPlayerHolyWarCap);
                }
                if (s.EnemyCovenantDarkPurgeMandate)
                    s.HolyWarTimer = math_min(s.HolyWarTimer, EnemyCovenantDarkPurgeHolyWarCap);
            }

            // --- enemy governance recognition (lines 1198-1215) ---
            if (s.EnemyGovernanceActive)
            {
                s.BuildTimer = math_min(s.BuildTimer, EnemyGovBuildCap);
                float terrCap = s.EnemyGovernanceHasTargetPoint
                    ? (s.EnemyGovernanceVictoryPressure  ? EnemyGovVictoryTerrWithTargetCap
                     : s.EnemyGovernanceAlliancePressure ? EnemyGovAllianceTerrWithTargetCap
                     :                                     EnemyGovActiveTerrWithTargetCap)
                    : EnemyGovNoTargetTerrCap;
                s.TerritoryTimer = math_min(s.TerritoryTimer, terrCap);
                // Attack and HolyWar are floored UPWARD: enemy holds back aggression.
                float attackFloor = s.EnemyGovernanceVictoryPressure  ? EnemyGovVictoryAttackFloor
                                  : s.EnemyGovernanceAlliancePressure ? EnemyGovAllianceAttackFloor
                                  :                                     EnemyGovActiveAttackFloor;
                float holyWarFloor= s.EnemyGovernanceVictoryPressure  ? EnemyGovVictoryHolyWarFloor
                                  : s.EnemyGovernanceAlliancePressure ? EnemyGovAllianceHolyWarFloor
                                  :                                     EnemyGovActiveHolyWarFloor;
                s.AttackTimer    = math_max(s.AttackTimer,    attackFloor);
                s.HolyWarTimer   = math_max(s.HolyWarTimer,   holyWarFloor);
            }
        }

        private static float math_min(float a, float b) => a < b ? a : b;
        private static float math_max(float a, float b) => a > b ? a : b;
    }
}
