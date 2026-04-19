#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.Conviction;
using Bloodlines.Dynasties;
using Bloodlines.GameTime;
using Bloodlines.Systems;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Smoke validator for the sub-slice 13 AI lesser-house promotion
    /// execution. Six phases cover every browser-side gate plus the
    /// success effects on the LesserHouseElement buffer, dynasty
    /// legitimacy, and conviction stewardship:
    ///
    ///   PhaseSuccessfulPromotion: legitimacy 60, no existing lesser
    ///     houses, eligible Governance member with Renown 35 -> new
    ///     LesserHouseElement appended, legitimacy 60 -> 63, stewardship
    ///     0 -> 2, dispatch cleared.
    ///   PhaseHighLegitimacyBlocks: legitimacy 90 (consolidation
    ///     ceiling) blocks promotion even with eligible candidate.
    ///   PhaseCapBlocks: 3 existing lesser houses blocks promotion even
    ///     with eligible candidate and low legitimacy.
    ///   PhaseHeadOfBloodlineRejected: only HeadOfBloodline meets renown
    ///     threshold; no other eligible member; promotion does not fire.
    ///   PhaseNonQualifyingPathRejected: only non-qualifying-path members
    ///     (Diplomat, Merchant) meet the renown threshold; no eligible
    ///     candidate; promotion does not fire.
    ///   PhaseNearCeilingPromotion: legitimacy starts at 89 (just below
    ///     the consolidation ceiling) with an eligible CovertOperations
    ///     candidate; +3 bonus lands at 92. The clamp-to-100 logic is
    ///     wired but unreachable in practice because the legitimacy
    ///     ceiling block at 90 fires first; this phase proves the
    ///     near-ceiling path executes cleanly.
    ///
    /// Browser reference:
    ///   ai.js tryAiPromoteLesserHouse (~2784-2801),
    ///   simulation.js promoteMemberToLesserHouse (~7184-7258),
    ///   memberIsLesserHouseCandidate (~6469-6479),
    ///   constants block (~6444-6457).
    /// Artifact: artifacts/unity-ai-lesser-house-promotion-smoke.log.
    /// </summary>
    public static class BloodlinesAILesserHousePromotionSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-ai-lesser-house-promotion-smoke.log";

        [UnityEditor.MenuItem("Bloodlines/AI/Run AI Lesser House Promotion Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchAILesserHousePromotionSmokeValidation() =>
            RunInternal(batchMode: true);

        private static void RunInternal(bool batchMode)
        {
            string message;
            bool success;
            try
            {
                success = RunAllPhases(out message);
            }
            catch (Exception e)
            {
                success = false;
                message = "AI lesser house promotion smoke errored: " + e;
            }

            string artifact = "BLOODLINES_AI_LESSER_HOUSE_PROMOTION_SMOKE " +
                              (success ? "PASS" : "FAIL") + "\n" + message;
            UnityDebug.Log(artifact);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ArtifactPath)!);
                File.WriteAllText(ArtifactPath, artifact);
            }
            catch (Exception) { }

            if (batchMode)
                EditorApplication.Exit(success ? 0 : 1);
        }

        private static bool RunAllPhases(out string report)
        {
            var sb = new System.Text.StringBuilder();
            bool ok = true;
            ok &= RunPhaseSuccessfulPromotion(sb);
            ok &= RunPhaseHighLegitimacyBlocks(sb);
            ok &= RunPhaseCapBlocks(sb);
            ok &= RunPhaseHeadOfBloodlineRejected(sb);
            ok &= RunPhaseNonQualifyingPathRejected(sb);
            ok &= RunPhaseNearCeilingPromotion(sb);
            report = sb.ToString();
            return ok;
        }

        // ------------------------------------------------------------------ setup

        private static SimulationSystemGroup SetupSimGroup(World world)
        {
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var sg = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AILesserHousePromotionSystem>());
            sg.SortSystems();
            return sg;
        }

        private static void SeedDualClock(EntityManager em, float inWorldDays = 50f)
        {
            var e = em.CreateEntity(typeof(DualClockComponent));
            em.SetComponentData(e, new DualClockComponent
            {
                InWorldDays       = inWorldDays,
                DaysPerRealSecond = 2f,
                DeclarationCount  = 0,
            });
        }

        private struct MemberSeed
        {
            public DynastyRole Role;
            public DynastyPath Path;
            public DynastyMemberStatus Status;
            public float Renown;
        }

        private static Entity SeedFactionWithRoster(
            EntityManager em,
            string factionId,
            float legitimacy,
            float stewardship,
            CovertOpKind dispatched,
            MemberSeed[] roster,
            int existingLesserHouses = 0)
        {
            var factionEntity = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(AICovertOpsComponent),
                typeof(DynastyStateComponent),
                typeof(ConvictionComponent),
                typeof(LesserHouseElement));
            em.SetComponentData(factionEntity, new FactionComponent { FactionId = factionId });
            em.SetComponentData(factionEntity, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(factionEntity, new AICovertOpsComponent
            {
                LastFiredOp     = dispatched,
                LastFiredOpTime = 0f,
            });
            em.SetComponentData(factionEntity, new DynastyStateComponent { Legitimacy = legitimacy });
            em.SetComponentData(factionEntity, new ConvictionComponent { Stewardship = stewardship });

            // Pre-seed existing lesser houses if requested (cap test).
            if (existingLesserHouses > 0)
            {
                var existingBuffer = em.GetBuffer<LesserHouseElement>(factionEntity);
                for (int i = 0; i < existingLesserHouses; i++)
                {
                    existingBuffer.Add(new LesserHouseElement
                    {
                        HouseId        = new FixedString32Bytes("lh-existing-" + i),
                        HouseName      = new FixedString64Bytes("Existing Cadet " + i),
                        FounderMemberId = new FixedString64Bytes("preexisting-" + i),
                        Loyalty        = 70f,
                        DailyLoyaltyDelta = 0f,
                        LastDriftAppliedInWorldDays = 0f,
                        Defected       = false,
                    });
                }
            }

            var memberBuffer = em.AddBuffer<DynastyMemberRef>(factionEntity);
            for (int i = 0; i < roster.Length; i++)
            {
                var memberEntity = em.CreateEntity(typeof(DynastyMemberComponent));
                em.SetComponentData(memberEntity, new DynastyMemberComponent
                {
                    MemberId = new FixedString64Bytes(factionId + "-bloodline-" + i),
                    Title    = new FixedString64Bytes("Member " + i),
                    Role     = roster[i].Role,
                    Path     = roster[i].Path,
                    Status   = roster[i].Status,
                    Renown   = roster[i].Renown,
                });
                memberBuffer.Add(new DynastyMemberRef { Member = memberEntity });
            }
            return factionEntity;
        }

        private static int CountLesserHouses(EntityManager em, Entity factionEntity)
        {
            if (!em.HasBuffer<LesserHouseElement>(factionEntity)) return 0;
            return em.GetBuffer<LesserHouseElement>(factionEntity).Length;
        }

        // ------------------------------------------------------------------ phases

        private static bool RunPhaseSuccessfulPromotion(System.Text.StringBuilder sb)
        {
            using var world = new World("lesser-house-promotion-success");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedDualClock(em);

            // Eligible: Governance, Active, Renown 35.
            var faction = SeedFactionWithRoster(em, "enemy", 60f, 0f,
                CovertOpKind.LesserHousePromotion, new[]
                {
                    new MemberSeed { Role = DynastyRole.HeadOfBloodline, Path = DynastyPath.Governance, Status = DynastyMemberStatus.Ruling, Renown = 50f },
                    new MemberSeed { Role = DynastyRole.Governor,        Path = DynastyPath.Governance, Status = DynastyMemberStatus.Active, Renown = 35f },
                });

            world.Update();

            int houses = CountLesserHouses(em, faction);
            float legitimacy = em.GetComponentData<DynastyStateComponent>(faction).Legitimacy;
            float stewardship = em.GetComponentData<ConvictionComponent>(faction).Stewardship;
            var covert = em.GetComponentData<AICovertOpsComponent>(faction);

            if (houses != 1)
            {
                sb.AppendLine($"PhaseSuccessfulPromotion FAIL: expected 1 lesser house, got {houses}");
                return false;
            }
            if (legitimacy < 62.99f || legitimacy > 63.01f)
            {
                sb.AppendLine($"PhaseSuccessfulPromotion FAIL: expected legitimacy 63 (60 + 3), got {legitimacy}");
                return false;
            }
            if (stewardship < 1.99f || stewardship > 2.01f)
            {
                sb.AppendLine($"PhaseSuccessfulPromotion FAIL: expected stewardship 2 (0 + 2), got {stewardship}");
                return false;
            }
            if (covert.LastFiredOp != CovertOpKind.None)
            {
                sb.AppendLine($"PhaseSuccessfulPromotion FAIL: dispatch should be cleared; got {covert.LastFiredOp}");
                return false;
            }
            sb.AppendLine($"PhaseSuccessfulPromotion PASS: 1 lesser house, legitimacy=63, stewardship=2, dispatch cleared");
            return true;
        }

        private static bool RunPhaseHighLegitimacyBlocks(System.Text.StringBuilder sb)
        {
            using var world = new World("lesser-house-promotion-legitimacy-block");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedDualClock(em);

            // Same eligible roster but legitimacy at the 90 ceiling.
            var faction = SeedFactionWithRoster(em, "enemy", 90f, 0f,
                CovertOpKind.LesserHousePromotion, new[]
                {
                    new MemberSeed { Role = DynastyRole.HeadOfBloodline, Path = DynastyPath.Governance, Status = DynastyMemberStatus.Ruling, Renown = 50f },
                    new MemberSeed { Role = DynastyRole.Governor,        Path = DynastyPath.Governance, Status = DynastyMemberStatus.Active, Renown = 35f },
                });

            world.Update();

            int houses = CountLesserHouses(em, faction);
            float legitimacy = em.GetComponentData<DynastyStateComponent>(faction).Legitimacy;
            var covert = em.GetComponentData<AICovertOpsComponent>(faction);

            if (houses != 0)
            {
                sb.AppendLine($"PhaseHighLegitimacyBlocks FAIL: expected 0 lesser houses, got {houses}");
                return false;
            }
            if (legitimacy < 89.99f || legitimacy > 90.01f)
            {
                sb.AppendLine($"PhaseHighLegitimacyBlocks FAIL: legitimacy must not change; got {legitimacy}");
                return false;
            }
            if (covert.LastFiredOp != CovertOpKind.None)
            {
                sb.AppendLine($"PhaseHighLegitimacyBlocks FAIL: dispatch should still clear; got {covert.LastFiredOp}");
                return false;
            }
            sb.AppendLine($"PhaseHighLegitimacyBlocks PASS: legitimacy >= 90 blocks promotion, dispatch cleared");
            return true;
        }

        private static bool RunPhaseCapBlocks(System.Text.StringBuilder sb)
        {
            using var world = new World("lesser-house-promotion-cap-block");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedDualClock(em);

            // 3 existing lesser houses (cap), low legitimacy, eligible candidate.
            var faction = SeedFactionWithRoster(em, "enemy", 50f, 0f,
                CovertOpKind.LesserHousePromotion, new[]
                {
                    new MemberSeed { Role = DynastyRole.Commander, Path = DynastyPath.MilitaryCommand, Status = DynastyMemberStatus.Active, Renown = 40f },
                },
                existingLesserHouses: 3);

            world.Update();

            int houses = CountLesserHouses(em, faction);
            float legitimacy = em.GetComponentData<DynastyStateComponent>(faction).Legitimacy;

            if (houses != 3)
            {
                sb.AppendLine($"PhaseCapBlocks FAIL: expected 3 lesser houses (cap), got {houses}");
                return false;
            }
            if (legitimacy < 49.99f || legitimacy > 50.01f)
            {
                sb.AppendLine($"PhaseCapBlocks FAIL: legitimacy must not change; got {legitimacy}");
                return false;
            }
            sb.AppendLine($"PhaseCapBlocks PASS: lesser house cap (3) blocks promotion");
            return true;
        }

        private static bool RunPhaseHeadOfBloodlineRejected(System.Text.StringBuilder sb)
        {
            using var world = new World("lesser-house-promotion-head-rejected");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedDualClock(em);

            // Only the head meets the renown threshold; head is excluded
            // because they continue the main line.
            var faction = SeedFactionWithRoster(em, "enemy", 60f, 0f,
                CovertOpKind.LesserHousePromotion, new[]
                {
                    new MemberSeed { Role = DynastyRole.HeadOfBloodline, Path = DynastyPath.Governance,      Status = DynastyMemberStatus.Ruling, Renown = 80f },
                    new MemberSeed { Role = DynastyRole.Commander,       Path = DynastyPath.MilitaryCommand, Status = DynastyMemberStatus.Active, Renown = 10f },
                });

            world.Update();

            int houses = CountLesserHouses(em, faction);
            float legitimacy = em.GetComponentData<DynastyStateComponent>(faction).Legitimacy;

            if (houses != 0)
            {
                sb.AppendLine($"PhaseHeadOfBloodlineRejected FAIL: head should not promote; got {houses} houses");
                return false;
            }
            if (legitimacy < 59.99f || legitimacy > 60.01f)
            {
                sb.AppendLine($"PhaseHeadOfBloodlineRejected FAIL: legitimacy must not change; got {legitimacy}");
                return false;
            }
            sb.AppendLine($"PhaseHeadOfBloodlineRejected PASS: head excluded, no other eligible candidate, no promotion");
            return true;
        }

        private static bool RunPhaseNonQualifyingPathRejected(System.Text.StringBuilder sb)
        {
            using var world = new World("lesser-house-promotion-path-rejected");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedDualClock(em);

            // Two members above renown threshold but on non-qualifying paths
            // (Diplomat, Merchant). Browser canon excludes these paths.
            var faction = SeedFactionWithRoster(em, "enemy", 60f, 0f,
                CovertOpKind.LesserHousePromotion, new[]
                {
                    new MemberSeed { Role = DynastyRole.Diplomat, Path = DynastyPath.Diplomacy,                Status = DynastyMemberStatus.Active, Renown = 40f },
                    new MemberSeed { Role = DynastyRole.Merchant, Path = DynastyPath.EconomicStewardshipTrade, Status = DynastyMemberStatus.Active, Renown = 50f },
                });

            world.Update();

            int houses = CountLesserHouses(em, faction);

            if (houses != 0)
            {
                sb.AppendLine($"PhaseNonQualifyingPathRejected FAIL: non-qualifying paths should not promote; got {houses} houses");
                return false;
            }
            sb.AppendLine($"PhaseNonQualifyingPathRejected PASS: Diplomacy and EconomicStewardshipTrade paths blocked");
            return true;
        }

        private static bool RunPhaseNearCeilingPromotion(System.Text.StringBuilder sb)
        {
            using var world = new World("lesser-house-promotion-clamp");
            var em = world.EntityManager;
            SetupSimGroup(world);
            SeedDualClock(em);

            // Legitimacy 99 with eligible candidate. +3 bonus must clamp at 100.
            // (Note 99 < 90 ceiling check is FALSE; need to test below ceiling.
            // Use 89 instead so promotion fires and bonus clamps at 92? No, we
            // want the clamp test. Use legitimacy 89 -> 89+3 = 92, no clamp.
            // For clamp test set initial 89.5 isn't useful either. The clamp
            // only matters above 97. But 97 is below 90 check? No 97 >= 90, so
            // promotion would not fire. The legitimacy ceiling block IS at 90,
            // so the clamp is unreachable in practice. Keep this phase to
            // document the clamp wiring is correct via direct math, but use
            // a starting value just below the ceiling and assert exact +3.)
            var faction = SeedFactionWithRoster(em, "enemy", 89f, 0f,
                CovertOpKind.LesserHousePromotion, new[]
                {
                    new MemberSeed { Role = DynastyRole.Spymaster, Path = DynastyPath.CovertOperations, Status = DynastyMemberStatus.Active, Renown = 40f },
                });

            world.Update();

            float legitimacy = em.GetComponentData<DynastyStateComponent>(faction).Legitimacy;
            int houses = CountLesserHouses(em, faction);

            if (houses != 1)
            {
                sb.AppendLine($"PhaseNearCeilingPromotion FAIL: expected 1 lesser house at legitimacy 89, got {houses}");
                return false;
            }
            if (legitimacy < 91.99f || legitimacy > 92.01f)
            {
                sb.AppendLine($"PhaseNearCeilingPromotion FAIL: expected legitimacy 92 (89 + 3), got {legitimacy}");
                return false;
            }
            sb.AppendLine($"PhaseNearCeilingPromotion PASS: CovertOperations path eligible, legitimacy 89 -> 92 (cap unreachable in practice; legitimacy ceiling at 90 blocks before clamp matters)");
            return true;
        }
    }
}
#endif
