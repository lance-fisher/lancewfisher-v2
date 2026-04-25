using Bloodlines.Components;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.AI
{
    /// <summary>
    /// Drives AI covert and diplomatic operation dispatch for the enemy faction.
    ///
    /// Each frame: decrements all operation timers by deltaTime, applies pressure-
    /// context caps, then fires the first timer that has expired and whose gate
    /// conditions are met.  Writing LastFiredOp records the intent; the actual
    /// startAssassinationOperation / startMissionaryOperation / etc. calls are
    /// deferred to a future integration pass once those subsystems are wired.
    ///
    /// Timer defaults (canonical from ai.js):
    ///   AssassinationTimer       80s
    ///   MissionaryTimer          70s
    ///   HolyWarTimer             95s
    ///   DivineRightTimer        140s
    ///   CaptiveRecoveryTimer     60s
    ///   MarriageProposalTimer    90s
    ///   MarriageInboxTimer       30s
    ///   PactProposalTimer       120s
    ///   LesserHousePromotion     60s
    ///
    /// Browser reference: ai.js updateEnemyAi dynasty/covert ops block ~2419-2678.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AISiegeOrchestrationSystem))]
    public partial struct AICovertOpsSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            float dt      = SystemAPI.Time.DeltaTime;
            float elapsed = (float)SystemAPI.Time.ElapsedTime;

            foreach (var ops in SystemAPI.Query<RefRW<AICovertOpsComponent>>())
            {
                ref var o = ref ops.ValueRW;
                TickTimers(ref o, dt);
                ApplyPressureCaps(ref o);
                TryFireOps(ref o, elapsed);
            }
        }

        // ------------------------------------------------------------------ tick all timers

        private static void TickTimers(ref AICovertOpsComponent o, float dt)
        {
            o.AssassinationTimer        -= dt;
            o.MissionaryTimer           -= dt;
            o.HolyWarTimer              -= dt;
            o.DivineRightTimer          -= dt;
            o.CaptiveRecoveryTimer      -= dt;
            o.MarriageProposalTimer     -= dt;
            o.MarriageInboxTimer        -= dt;
            o.PactProposalTimer         -= dt;
            o.LesserHousePromotionTimer -= dt;
            o.SabotageTimer             -= dt;
            o.EspionageTimer            -= dt;
            o.CounterIntelligenceTimer  -= dt;
        }

        // ------------------------------------------------------------------ apply pressure caps

        // Mirrors the Math.min clamp chains in ai.js before each timer-fire check.
        private static void ApplyPressureCaps(ref AICovertOpsComponent o)
        {
            // --- assassination caps (ai.js 2420-2434) ---
            if (o.DarkExtremesSourceFocused)
                o.AssassinationTimer = math.min(o.AssassinationTimer, 6f);
            if (o.PlayerWorldPressureLevel >= 2)
                o.AssassinationTimer = math.min(o.AssassinationTimer, 12f);
            else if (o.PlayerWorldPressureLevel > 0)
                o.AssassinationTimer = math.min(o.AssassinationTimer, 22f);
            if (o.ConvergencePressureActive && o.ConvergenceAssassinationTimerCap > 0f)
                o.AssassinationTimer = math.min(o.AssassinationTimer, o.ConvergenceAssassinationTimerCap);
            if (o.LiveCounterIntelOnPlayer)
                o.AssassinationTimer = math.min(o.AssassinationTimer,
                    o.CounterIntelInterceptIsAssassination ? 4f : 6f);

            // --- missionary caps (ai.js 2460-2467) ---
            if (o.HolyWarSourceFocused)
                o.MissionaryTimer = math.min(o.MissionaryTimer, 8f);
            if (o.PlayerDivineRightActive)
                o.MissionaryTimer = math.min(o.MissionaryTimer, 6f);
            if (o.ConvergencePressureActive && o.ConvergenceMissionaryTimerCap > 0f)
                o.MissionaryTimer = math.min(o.MissionaryTimer, o.ConvergenceMissionaryTimerCap);

            // --- holy war caps (ai.js 2500-2511) ---
            if (o.HolyWarSourceFocused)
                o.HolyWarTimer = math.min(o.HolyWarTimer, 10f);
            if (o.PlayerDivineRightActive)
                o.HolyWarTimer = math.min(o.HolyWarTimer, 8f);
            if (o.PlayerWorldPressureLevel >= 2)
                o.HolyWarTimer = math.min(o.HolyWarTimer, 14f);
            else if (o.PlayerWorldPressureLevel > 0)
                o.HolyWarTimer = math.min(o.HolyWarTimer, 24f);
            if (o.ConvergencePressureActive && o.ConvergenceHolyWarTimerCap > 0f)
                o.HolyWarTimer = math.min(o.HolyWarTimer, o.ConvergenceHolyWarTimerCap);

            // --- captive recovery cap (ai.js 2568) ---
            if (o.CaptivesSourceFocused)
                o.CaptiveRecoveryTimer = math.min(o.CaptiveRecoveryTimer, 6f);

            // --- espionage caps (ai.js 2400-2405) ---
            if (o.PlayerWorldPressureLevel >= 2)
                o.EspionageTimer = math.min(o.EspionageTimer, 10f);
            else if (o.PlayerWorldPressureLevel > 0)
                o.EspionageTimer = math.min(o.EspionageTimer, 18f);
            if (o.ConvergencePressureActive && o.ConvergenceEspionageTimerCap > 0f)
                o.EspionageTimer = math.min(o.EspionageTimer, o.ConvergenceEspionageTimerCap);

            // --- sabotage caps (ai.js 2326-2340) ---
            if (o.HostileOperationsSourceFocused)
                o.SabotageTimer = math.min(o.SabotageTimer, 6f);
            if (o.PlayerWorldPressureLevel >= 2)
                o.SabotageTimer = math.min(o.SabotageTimer, 8f);
            else if (o.PlayerWorldPressureLevel > 0)
                o.SabotageTimer = math.min(o.SabotageTimer, 16f);
            if (o.ConvergencePressureActive && o.ConvergenceSabotageTimerCap > 0f)
                o.SabotageTimer = math.min(o.SabotageTimer, o.ConvergenceSabotageTimerCap);
            if (o.LiveCounterIntelOnPlayer)
                o.SabotageTimer = math.min(o.SabotageTimer,
                    o.CounterIntelHighInterceptCount ? 4f : 6f);

            // --- counter-intelligence caps (ai.js 2372-2397) ---
            if (o.HostileOperationsSourceFocused)
                o.CounterIntelligenceTimer = math.min(o.CounterIntelligenceTimer, 4f);
        }

        // ------------------------------------------------------------------ fire operations (in ai.js sequence order)

        private static void TryFireOps(ref AICovertOpsComponent o, float elapsed)
        {
            // --- assassination (ai.js 2435-2457) ---
            if (o.AssassinationTimer <= 0f)
            {
                if (o.LiveIntelOnPlayer)
                {
                    o.LastFiredOp   = CovertOpKind.Assassination;
                    o.LastFiredOpTime = elapsed;
                    o.AssassinationTimer = 180f;
                    return;
                }
                // No intel: short retry
                o.AssassinationTimer = o.LiveCounterIntelOnPlayer ? 50f :
                                       o.LiveIntelOnPlayer        ? 60f : 35f;
            }

            // --- missionary (ai.js 2469-2496) ---
            if (o.MissionaryTimer <= 0f)
            {
                bool canPressureFaith = o.EnemyHasFaith &&
                                        o.EnemyFaithDiffersFromPlayer &&
                                        o.EnemyFaithIntensity >= 12f;
                if (canPressureFaith && !o.LiveMissionaryOnPlayer)
                {
                    o.LastFiredOp     = CovertOpKind.Missionary;
                    o.LastFiredOpTime = elapsed;
                    o.MissionaryTimer = 170f;
                    return;
                }
                o.MissionaryTimer = canPressureFaith ? 55f : 35f;
            }

            // --- holy war (ai.js 2512-2550) ---
            if (o.HolyWarTimer <= 0f)
            {
                bool canDeclareHolyWar =
                    o.EnemyHasFaith &&
                    o.PlayerHasFaith &&
                    !o.FaithCompatibilityHarmonious &&
                    o.EnemyFaithIntensity >= 18f &&
                    o.TensionSignalCount > 0;
                if (!o.ActiveHolyWarOnPlayer && canDeclareHolyWar)
                {
                    o.LastFiredOp     = CovertOpKind.HolyWar;
                    o.LastFiredOpTime = elapsed;
                    o.HolyWarTimer    = 240f;
                    return;
                }
                o.HolyWarTimer = canDeclareHolyWar ? 75f : 45f;
            }

            // --- divine right (ai.js 2554-2563) ---
            if (o.DivineRightTimer <= 0f)
            {
                if (o.DivineRightAvailable)
                {
                    o.LastFiredOp     = CovertOpKind.DivineRight;
                    o.LastFiredOpTime = elapsed;
                    o.DivineRightTimer = 260f;
                    return;
                }
                o.DivineRightTimer = o.DivineRightAvailable ? 120f : 55f;
            }

            // --- captive recovery (ai.js 2570-2608): rescue-first for high priority (ai.js 2573-2582) ---
            if (o.CaptiveRecoveryTimer <= 0f)
            {
                if (o.HasCaptiveTarget)
                {
                    bool prefersRescue = o.HighPriorityCaptive || o.EnemyIsHostileToPlayer;
                    o.LastFiredOp     = prefersRescue ? CovertOpKind.CaptiveRescue : CovertOpKind.CaptiveRansom;
                    o.LastFiredOpTime = elapsed;
                    o.CaptiveRecoveryTimer = prefersRescue ? 170f : 140f;
                    return;
                }
                o.CaptiveRecoveryTimer = 55f;
            }

            // --- marriage proposal (ai.js 2617-2623) ---
            if (o.MarriageProposalTimer <= 0f)
            {
                if (o.MarriageProposalGateMet)
                {
                    o.LastFiredOp        = CovertOpKind.MarriageProposal;
                    o.LastFiredOpTime    = elapsed;
                    o.MarriageProposalTimer = 240f;
                    return;
                }
                o.MarriageProposalTimer = 60f;
            }

            // --- marriage inbox (ai.js 2633-2635) ---
            if (o.MarriageInboxTimer <= 0f)
            {
                if (o.MarriageInboxHasProposal && o.MarriageInboxAcceptGate)
                {
                    o.LastFiredOp        = CovertOpKind.MarriageInboxAccept;
                    o.LastFiredOpTime    = elapsed;
                    o.MarriageInboxTimer = 180f;
                    return;
                }
                o.MarriageInboxTimer = 30f;
            }

            // --- non-aggression pact (ai.js 2644-2665) ---
            if (o.PactProposalTimer <= 0f)
            {
                if (o.PactTermsAvailable)
                {
                    bool shouldPropose =
                        o.EnemyUnderSuccessionPressure ||
                        o.EnemyArmyCount <= 3 ||
                        o.PlayerGovernanceNearVictory;
                    if (shouldPropose)
                    {
                        o.LastFiredOp        = CovertOpKind.PactProposal;
                        o.LastFiredOpTime    = elapsed;
                        o.PactProposalTimer  = 300f;
                        return;
                    }
                }
                o.PactProposalTimer = 90f;
            }

            // --- lesser-house promotion (ai.js 2675-2677) ---
            if (o.LesserHousePromotionTimer <= 0f)
            {
                if (o.LesserHousePromotionAvailable)
                {
                    o.LastFiredOp                = CovertOpKind.LesserHousePromotion;
                    o.LastFiredOpTime            = elapsed;
                    o.LesserHousePromotionTimer  = 180f;
                    return;
                }
                o.LesserHousePromotionTimer = 45f;
            }

            // --- sabotage (ai.js 2341-2370): fire unconditionally; execution system
            //     checks budget (gold>=60, influence>=12) and available target. ---
            if (o.SabotageTimer <= 0f)
            {
                o.LastFiredOp     = CovertOpKind.Sabotage;
                o.LastFiredOpTime = elapsed;
                o.SabotageTimer   = 85f;
            }

            // --- espionage (ai.js 2406-2417): gate on !liveIntelOnPlayer && !liveEspionage.
            //     on gate-blocked: retry at 90s (liveIntel) or 30s (default).
            //     execution system validates budget (gold>=45, influence>=16) and capacity. ---
            if (o.EspionageTimer <= 0f)
            {
                if (!o.LiveIntelOnPlayer && !o.HasActiveEspionageOnPlayer)
                {
                    o.LastFiredOp     = CovertOpKind.Espionage;
                    o.LastFiredOpTime = elapsed;
                    o.EspionageTimer  = 150f;
                }
                else
                {
                    o.EspionageTimer = o.LiveIntelOnPlayer ? 90f : 30f;
                }
            }

            // --- counter-intelligence (ai.js 2372-2397): suppress when watch already active.
            //     execution system validates budget (gold>=60, influence>=18) and capacity. ---
            if (o.CounterIntelligenceTimer <= 0f && !o.HasActiveCounterIntelligenceWatch)
            {
                o.LastFiredOp              = CovertOpKind.CounterIntelligence;
                o.LastFiredOpTime          = elapsed;
                o.CounterIntelligenceTimer = 190f;
            }
        }
    }
}
