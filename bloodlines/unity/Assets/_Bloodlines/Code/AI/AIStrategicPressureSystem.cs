using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Applies strategic timer pressure overrides to AI factions each frame.
    ///
    /// Ports the pressure-clamp and stage-gate block from ai.js updateEnemyAi
    /// (lines 1127-1241). Responsibilities:
    ///   1. Decrement per-frame countdown timers (attack, territory, raid).
    ///   2. Derive RivalryUnlocked / RaidPressureUnlocked from match stage and
    ///      world-pressure overrides.
    ///   3. Clamp attack/territory/raid timers downward when the player faction
    ///      has elevated world pressure (AI becomes more aggressive).
    ///   4. Floor territory/raid timers upward when rivalry/raid gates are closed
    ///      (AI holds back until stage gates open).
    ///
    /// Browser equivalents: ai.js lines 1127-1241 (timer clamp/floor block inside
    /// updateEnemyAi).
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

                em.SetComponentData(entities[i], s);
            }
            entities.Dispose();
            strategies.Dispose();
        }

        private static float math_min(float a, float b) => a < b ? a : b;
        private static float math_max(float a, float b) => a > b ? a : b;
    }
}
