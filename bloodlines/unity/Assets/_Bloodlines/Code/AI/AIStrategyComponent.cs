using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Strategic posture the AI operates under, driven by WorldPressureComponent.Level.
    /// Browser equivalent: pressure-conditioned posture logic in ai.js territory/raid
    /// targeting (lines 817-949).
    /// </summary>
    public enum AIStrategicPosture : byte
    {
        Expand = 0,      // Level 0-1: press outward, claim uncontested CPs
        Consolidate = 1, // Level 2: hold gains, fortify existing holdings
        Defend = 2,      // Level 3 (targeted): call units home, halt expansion
    }

    /// <summary>
    /// Per-faction strategic AI state. Carries accumulators and cached decisions for
    /// territory expansion, scout/harass dispatch, world-pressure response, and
    /// reinforcement routing.
    ///
    /// Attached alongside AIEconomyControllerComponent on non-player Kingdom factions.
    /// Browser equivalent: the ai.js strategic brain tick (lines 747-949).
    /// </summary>
    public struct AIStrategyComponent : IComponentData
    {
        // --- territory expansion (ai.js pickTerritoryTarget ~line 747) ---
        public FixedString32Bytes ExpansionTargetCpId;
        public float ExpansionAccumulator;
        public float ExpansionIntervalSeconds;   // canonical default 8s
        public int ExpansionOrdersIssued;
        public int TerritoryCommandsIssued;

        // --- scout / harass (ai.js pickScoutHarassTarget ~line 412) ---
        public FixedString32Bytes HarassTargetCpId;
        public float ScoutHarassAccumulator;
        public float ScoutHarassIntervalSeconds; // canonical default 12s
        public int ScoutHarassOrdersIssued;

        // --- world pressure response (ai.js getWorldPressureRaidTarget ~line 817) ---
        public float WorldPressureResponseAccumulator;
        public float WorldPressureResponseIntervalSeconds; // canonical default 15s
        public int WorldPressureLevelCached;
        public bool IsWorldPressureTargetedCached;
        public AIStrategicPosture CurrentPosture;

        // --- reinforcement routing ---
        public float ReinforcementAccumulator;
        public float ReinforcementIntervalSeconds; // canonical default 10s
        public int ReinforcementOrdersIssued;

        // --- strategic timers (ai.js updateEnemyAi countdown timers, lines 960-963) ---
        // Each counts down each frame; action fires when <= 0 and timer resets.
        public float AttackTimer;           // canonical initial 20s
        public float TerritoryTimer;        // canonical initial 12s
        public float RaidTimer;             // canonical initial 18s
        public float MarriageProposalTimer; // canonical initial 90s
        public float HolyWarTimer;          // canonical initial 95s
        public float AssassinationTimer;    // canonical initial 80s
        public float MissionaryTimer;       // canonical initial 70s

        // --- match stage gates (ai.js lines 1103-1107) ---
        // RivalryUnlocked: stage >= 2 or rivalry pressure override is active.
        // RaidPressureUnlocked: stage >= 3 or raidPressureOverride is active.
        public bool RivalryUnlocked;
        public bool RaidPressureUnlocked;

        // --- build timer (ai.js updateEnemyAi, enemy.ai.buildTimer) ---
        public float BuildTimer;  // canonical initial 8s; accelerated by covenant/governance pressure

        // --- worker gather dispatch (ai.js idleWorkers dispatch loop, lines 1243-1251) ---
        public float WorkerGatherAccumulator;
        public float WorkerGatherIntervalSeconds; // canonical default 5s
        public int IdleWorkerCount;               // observability
        public int WorkersDispatched;             // running total
        public bool PlayerKeepFortified;          // drives gather priority (context.playerKeepFortified)

        // --- governance / event-context pressure flags (ai.js lines 1129-1215) ---
        // These are written by dynasty, faith, and governance systems (or seeded directly in tests).
        // AIStrategicPressureSystem reads them to apply the second layer of timer clamps.

        // Holy war (either direction): attackTimer<=10, territoryTimer<=8 (line 1129-1132).
        public bool HolyWarActive;

        // Player territorial governance recognition pressure (lines 1133-1149).
        public bool PlayerGovernanceActive;          // recognition is live
        public bool PlayerGovernanceVictoryPressure; // victoryReady && !completed
        public bool PlayerGovernanceAlliancePressure;// integrationReady / pct >= alliance threshold

        // Player active covenant test (lines 1150-1155).
        public bool PlayerCovenantActive;
        public bool PlayerCovenantTargetsEnemy;      // targetFactionId == "enemy" or CP owned by enemy

        // Player divine right declaration active (lines 1156-1160).
        public bool PlayerDivineRightActive;

        // Player succession crisis (lines 1161-1165).
        public bool PlayerSuccessionCrisisActive;
        public bool PlayerSuccessionCrisisHigh; // severityId "major" or "catastrophic"

        // Enemy succession crisis (lines 1167-1185).
        // Floors attackTimer/territoryTimer upward; drives marriageProposalTimer down.
        public bool EnemySuccessionCrisisActive;
        public bool EnemySuccessionCrisisSevere;     // severityId "major" or "catastrophic"
        public float EnemySuccessionCrisisAccumulator; // countdown for consolidation check

        // Enemy active covenant test (lines 1186-1197).
        public bool EnemyCovenantActive;
        public bool EnemyCovenantTargetsPlayer;      // targetFactionId == "player"
        public bool EnemyCovenantDarkPurgeMandate;   // mandateId == "old_light_dark_purge"
        public bool EnemyCovenantHasTargetPoint;     // enemyCovenantTargetPoint != null

        // Enemy territorial governance recognition (lines 1198-1215).
        // Floors attackTimer/holyWarTimer upward (enemy focuses on governance, not aggression).
        public bool EnemyGovernanceActive;
        public bool EnemyGovernanceVictoryPressure;  // victoryReady && !completed
        public bool EnemyGovernanceAlliancePressure; // integrationReady / pct >= threshold
        public bool EnemyGovernanceHasTargetPoint;   // enemyGovernanceTargetPoint != null
    }
}
