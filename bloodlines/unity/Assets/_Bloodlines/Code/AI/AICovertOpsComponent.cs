using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Holds timers and context flags for all AI covert/diplomatic operations.
    ///
    /// AICovertOpsSystem decrements the timers each frame, applies pressure caps
    /// from context flags, and writes LastFiredOp when a timer fires and its gate
    /// conditions are met. Actual operation execution (startAssassinationOperation,
    /// startMissionaryOperation, etc.) is deferred to a future integration pass;
    /// this component and system port the dispatch scheduling logic only.
    ///
    /// Browser reference: ai.js updateEnemyAi dynasty/covert ops block ~2419-2678.
    /// </summary>
    public struct AICovertOpsComponent : IComponentData
    {
        // ------------------------------------------------------------------ timers (canonical defaults from ai.js)
        public float AssassinationTimer;       // default 80s
        public float MissionaryTimer;          // default 70s
        public float HolyWarTimer;             // default 95s
        public float DivineRightTimer;         // default 140s
        public float CaptiveRecoveryTimer;     // default 60s
        public float MarriageProposalTimer;    // default 90s
        public float MarriageInboxTimer;       // default 30s
        public float PactProposalTimer;        // default 120s
        public float LesserHousePromotionTimer;// default 60s

        // ------------------------------------------------------------------ context flags: assassination (2419-2456)
        public bool  DarkExtremesSourceFocused;
        public int   PlayerWorldPressureLevel;  // 0, 1, or 2
        public bool  ConvergencePressureActive;
        public float ConvergenceAssassinationTimerCap;
        public bool  LiveCounterIntelOnPlayer;
        public bool  CounterIntelInterceptIsAssassination; // interceptType === "assassination"
        public bool  LiveIntelOnPlayer;         // required to pick a target

        // ------------------------------------------------------------------ context flags: missionary (2459-2496)
        public bool  HolyWarSourceFocused;
        public bool  PlayerDivineRightActive;
        public float ConvergenceMissionaryTimerCap;
        public bool  EnemyHasFaith;             // enemy.faith.selectedFaithId != null
        public bool  EnemyFaithDiffersFromPlayer;
        public float EnemyFaithIntensity;       // >= 12 required for canPressureFaith
        public bool  LiveMissionaryOnPlayer;    // already an active missionary op

        // ------------------------------------------------------------------ context flags: holy war (2499-2550)
        public float ConvergenceHolyWarTimerCap;
        public bool  FaithCompatibilityHarmonious;// tier === "harmonious" blocks holy war
        public int   TensionSignalCount;        // must be > 0 to declare
        public bool  ActiveHolyWarOnPlayer;     // blocks re-declaration
        // EnemyFaithIntensity >= 18 also required (reused from missionary block)
        public bool  PlayerHasFaith;            // state.factions.player.faith.selectedFaithId

        // ------------------------------------------------------------------ context flags: divine right (2553-2563)
        public bool  DivineRightAvailable;

        // ------------------------------------------------------------------ context flags: captive recovery (2566-2608)
        public bool  CaptivesSourceFocused;
        public bool  HasCaptiveTarget;
        public bool  HighPriorityCaptive;       // drives rescue-first vs ransom-first
        public bool  EnemyIsHostileToPlayer;    // also triggers rescue-first (hostileTo includes "player")

        // ------------------------------------------------------------------ context flags: marriage proposal (2616-2623)
        public bool  MarriageProposalGateMet;   // tryAiMarriageProposal would return "proposed"

        // ------------------------------------------------------------------ context flags: marriage inbox (2632-2635)
        public bool  MarriageInboxHasProposal;
        public bool  MarriageInboxAcceptGate;   // symmetric criteria to proposal

        // ------------------------------------------------------------------ context flags: non-aggression pact (2643-2665)
        public bool  PactTermsAvailable;
        public bool  EnemyUnderSuccessionPressure;
        public int   EnemyArmyCount;            // <= 3 triggers under-army pressure
        public bool  PlayerGovernanceNearVictory;

        // ------------------------------------------------------------------ context flags: lesser-house promotion (2674-2677)
        public bool  LesserHousePromotionAvailable;

        // ------------------------------------------------------------------ dispatch result (written by system)
        public CovertOpKind LastFiredOp;
        public float        LastFiredOpTime;    // elapsed seconds at last fire
    }

    /// <summary>
    /// Identifies which covert/diplomatic operation was dispatched last.
    /// </summary>
    public enum CovertOpKind : byte
    {
        None                   = 0,
        Assassination          = 1,
        Missionary             = 2,
        HolyWar                = 3,
        DivineRight            = 4,
        CaptiveRescue          = 5,
        CaptiveRansom          = 6,
        MarriageProposal       = 7,
        MarriageInboxAccept    = 8,
        PactProposal           = 9,
        LesserHousePromotion   = 10,
    }
}
