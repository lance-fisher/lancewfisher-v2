#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Dynasties;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Smoke validator for the Tier 2 batch dynasty systems.
    ///
    /// Phase 1: RenownAwardSystem routes award to Commander member.
    /// Phase 2: MarriageProposalExpirationSystem expires a stale proposal.
    /// Phase 3: MarriageGestationSystem generates a child after gestation window.
    /// Phase 4: LesserHouseLoyaltyDriftSystem triggers defection at zero loyalty.
    /// Phase 5: MinorHouseLevySystem spawns a levy unit after interval fires.
    ///
    /// Artifact: artifacts/unity-tier2-batch-smoke.log.
    /// </summary>
    public static class BloodlinesTier2BatchSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-tier2-batch-smoke.log";

        [MenuItem("Bloodlines/AI/Run Tier 2 Batch Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchTier2BatchSmokeValidation() => RunInternal(batchMode: true);

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
                message = "Tier 2 batch smoke errored: " + e;
            }

            string artifact = "BLOODLINES_TIER2_BATCH_SMOKE " + (success ? "PASS" : "FAIL") + "\n" + message;
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
            ok &= RunPhase1(sb);
            ok &= RunPhase2(sb);
            ok &= RunPhase3(sb);
            ok &= RunPhase4(sb);
            ok &= RunPhase5(sb);
            report = sb.ToString();
            return ok;
        }

        private static SimulationSystemGroup SetupSimGroup(World world)
        {
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var sg = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            return sg;
        }

        // --------- Phase 1: renown award routed to Commander ---------

        private static bool RunPhase1(System.Text.StringBuilder sb)
        {
            using var world = new World("tier2-phase1");
            var em = world.EntityManager;
            var sg = SetupSimGroup(world);
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<RenownAwardSystem>());

            // Faction entity with dynasty buffer.
            var faction = em.CreateEntity(typeof(FactionComponent), typeof(DynastyStateComponent));
            em.SetComponentData(faction, new FactionComponent { FactionId = "enemy" });
            em.SetComponentData(faction, new DynastyStateComponent { Legitimacy = 50f });
            em.AddBuffer<DynastyMemberRef>(faction);

            // Add a Commander member.
            var commander = em.CreateEntity(typeof(FactionComponent), typeof(DynastyMemberComponent));
            em.SetComponentData(commander, new FactionComponent { FactionId = "enemy" });
            em.SetComponentData(commander, new DynastyMemberComponent
            {
                MemberId = "cmdr-1",
                Title    = "Commander",
                Role     = DynastyRole.Commander,
                Path     = DynastyPath.MilitaryCommand,
                Status   = DynastyMemberStatus.Active,
                Renown   = 10f,
                Order    = 0,
                FallenAtWorldSeconds = -1f,
            });
            em.GetBuffer<DynastyMemberRef>(faction).Add(new DynastyMemberRef { Member = commander });

            // Renown award request.
            var req = em.CreateEntity(typeof(RenownAwardRequestComponent));
            em.SetComponentData(req, new RenownAwardRequestComponent
            {
                FactionId = "enemy",
                Amount    = 5f,
                Consumed  = false,
            });

            world.Update();

            var cmdrAfter = em.GetComponentData<DynastyMemberComponent>(commander);
            var reqAfter  = em.GetComponentData<RenownAwardRequestComponent>(req);

            if (System.Math.Abs(cmdrAfter.Renown - 15f) > 0.01f)
            {
                sb.AppendLine($"Phase 1 FAIL: expected Renown=15, got {cmdrAfter.Renown}."); return false;
            }
            if (!reqAfter.Consumed)
            {
                sb.AppendLine("Phase 1 FAIL: RenownAwardRequest not consumed."); return false;
            }

            sb.AppendLine($"Phase 1 PASS: Commander.Renown={cmdrAfter.Renown} Consumed={reqAfter.Consumed}.");
            return true;
        }

        // --------- Phase 2: proposal expiration ---------

        private static bool RunPhase2(System.Text.StringBuilder sb)
        {
            using var world = new World("tier2-phase2");
            var em = world.EntityManager;
            var sg = SetupSimGroup(world);
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<MarriageProposalExpirationSystem>());

            // DualClock at day 35 (past 30-day expiration).
            var clock = em.CreateEntity(typeof(DualClockComponent));
            em.SetComponentData(clock, new DualClockComponent { InWorldDays = 35f });

            // Pending proposal proposed at day 0.
            var proposal = em.CreateEntity(typeof(MarriageProposalComponent));
            em.SetComponentData(proposal, new MarriageProposalComponent
            {
                ProposalId            = "prop-1",
                SourceFactionId       = "enemy",
                TargetFactionId       = "player",
                Status                = MarriageProposalStatus.Pending,
                ProposedAtInWorldDays = 0f,
                ExpiresAtInWorldDays  = MarriageProposalExpirationSystem.ExpirationInWorldDays,
            });

            world.Update();

            var propAfter = em.GetComponentData<MarriageProposalComponent>(proposal);
            if (propAfter.Status != MarriageProposalStatus.Expired)
            {
                sb.AppendLine($"Phase 2 FAIL: expected Expired, got {propAfter.Status}."); return false;
            }

            sb.AppendLine($"Phase 2 PASS: proposal status={propAfter.Status} at day=35.");
            return true;
        }

        // --------- Phase 3: marriage gestation generates child ---------

        private static bool RunPhase3(System.Text.StringBuilder sb)
        {
            using var world = new World("tier2-phase3");
            var em = world.EntityManager;
            var sg = SetupSimGroup(world);
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<MarriageGestationSystem>());

            // DualClock at day 70 (past 60-day gestation).
            var clock = em.CreateEntity(typeof(DualClockComponent));
            em.SetComponentData(clock, new DualClockComponent { InWorldDays = 70f });

            // Faction entity with dynasty buffer.
            var faction = em.CreateEntity(typeof(FactionComponent), typeof(DynastyStateComponent));
            em.SetComponentData(faction, new FactionComponent { FactionId = "enemy" });
            em.SetComponentData(faction, new DynastyStateComponent { Legitimacy = 50f });
            em.AddBuffer<DynastyMemberRef>(faction);

            // Marriage entity -- head faction = "enemy".
            var marriage = em.CreateEntity(typeof(MarriageComponent));
            em.SetComponentData(marriage, new MarriageComponent
            {
                MarriageId                = "marriage-1",
                HeadFactionId             = "enemy",
                HeadMemberId              = "head-1",
                SpouseFactionId           = "player",
                SpouseMemberId            = "spouse-1",
                MarriedAtInWorldDays      = 0f,
                ExpectedChildAtInWorldDays = 60f,
                IsPrimary                 = true,
                ChildGenerated            = false,
                Dissolved                 = false,
            });

            int membersBefore = em.GetBuffer<DynastyMemberRef>(faction).Length;
            world.Update();
            int membersAfter = em.GetBuffer<DynastyMemberRef>(faction).Length;

            var marriageAfter = em.GetComponentData<MarriageComponent>(marriage);

            if (!marriageAfter.ChildGenerated)
            {
                sb.AppendLine("Phase 3 FAIL: ChildGenerated not set."); return false;
            }
            if (membersAfter <= membersBefore)
            {
                sb.AppendLine($"Phase 3 FAIL: expected new dynasty member, count {membersBefore} -> {membersAfter}."); return false;
            }

            sb.AppendLine($"Phase 3 PASS: ChildGenerated={marriageAfter.ChildGenerated} MemberCount={membersBefore}->{membersAfter}.");
            return true;
        }

        // --------- Phase 4: lesser house defection at zero loyalty ---------

        private static bool RunPhase4(System.Text.StringBuilder sb)
        {
            using var world = new World("tier2-phase4");
            var em = world.EntityManager;
            var sg = SetupSimGroup(world);
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<LesserHouseLoyaltyDriftSystem>());

            // DualClock at day 5; lesser house last drift applied at day 0.
            // The system will apply 5 days of drift on the first tick.
            var clock = em.CreateEntity(typeof(DualClockComponent));
            em.SetComponentData(clock, new DualClockComponent { InWorldDays = 5f });

            // Faction with a lesser house at loyalty = 2 and delta = -1.0/day.
            // After 5 days: loyalty = 2 + (-1 * 5) = -3 -> defection triggers.
            var faction = em.CreateEntity(typeof(FactionComponent), typeof(FactionKindComponent));
            em.SetComponentData(faction, new FactionComponent { FactionId = "enemy" });
            em.SetComponentData(faction, new FactionKindComponent { Kind = FactionKind.Kingdom });
            var buffer = em.AddBuffer<LesserHouseElement>(faction);
            buffer.Add(new LesserHouseElement
            {
                HouseId                       = "lh-1",
                HouseName                     = "Cadet House Thornwall",
                FounderMemberId               = "founder-1",
                Loyalty                       = 2f,
                DailyLoyaltyDelta             = -1f,
                LastDriftAppliedInWorldDays   = 0f,
                Defected                      = false,
            });

            world.Update();

            var bufAfter = em.GetBuffer<LesserHouseElement>(faction);
            if (bufAfter.Length == 0)
            {
                sb.AppendLine("Phase 4 FAIL: LesserHouseElement buffer lost."); return false;
            }

            var lhAfter = bufAfter[0];
            if (!lhAfter.Defected)
            {
                sb.AppendLine($"Phase 4 FAIL: expected Defected=true, got Loyalty={lhAfter.Loyalty}."); return false;
            }

            // Verify minor faction entity was spawned.
            var minorQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<FactionKindComponent>());
            var minorFactions = minorQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            var minorKinds    = minorQuery.ToComponentDataArray<FactionKindComponent>(Allocator.Temp);
            minorQuery.Dispose();

            bool minorSpawned = false;
            var expectedId    = new FixedString32Bytes("minor-lh-1");
            for (int i = 0; i < minorFactions.Length; i++)
            {
                if (minorFactions[i].FactionId.Equals(expectedId) &&
                    minorKinds[i].Kind == FactionKind.MinorHouse)
                {
                    minorSpawned = true;
                    break;
                }
            }

            minorFactions.Dispose();
            minorKinds.Dispose();

            if (!minorSpawned)
            {
                sb.AppendLine("Phase 4 FAIL: minor faction 'minor-lh-1' not spawned."); return false;
            }

            sb.AppendLine($"Phase 4 PASS: Defected={lhAfter.Defected} Loyalty={lhAfter.Loyalty} MinorFactionSpawned={minorSpawned}.");
            return true;
        }

        // --------- Phase 5: minor house levy spawns a unit ---------

        private static bool RunPhase5(System.Text.StringBuilder sb)
        {
            using var world = new World("tier2-phase5");
            var em = world.EntityManager;
            var sg = SetupSimGroup(world);
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<MinorHouseLevySystem>());

            // Minor house faction with levy interval = 0 (fire immediately).
            var faction = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(MinorHouseLevyComponent));
            em.SetComponentData(faction, new FactionComponent { FactionId = "minor-lh-1" });
            em.SetComponentData(faction, new FactionKindComponent { Kind = FactionKind.MinorHouse });
            em.SetComponentData(faction, new MinorHouseLevyComponent
            {
                LevyIntervalSeconds = 0f, // fire on first tick (floor = 0.001)
                LevyAccumulator     = 0f,
                LeviesIssued        = 0,
            });

            var unitQueryBefore = em.CreateEntityQuery(
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<FactionComponent>());
            int unitsBefore = unitQueryBefore.CalculateEntityCount();
            unitQueryBefore.Dispose();

            world.Update();

            var levy = em.GetComponentData<MinorHouseLevyComponent>(faction);
            var unitQueryAfter = em.CreateEntityQuery(
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<FactionComponent>());
            int unitsAfter = unitQueryAfter.CalculateEntityCount();
            unitQueryAfter.Dispose();

            if (levy.LeviesIssued == 0)
            {
                sb.AppendLine("Phase 5 FAIL: no levy issued after interval=0 tick."); return false;
            }
            if (unitsAfter <= unitsBefore)
            {
                sb.AppendLine($"Phase 5 FAIL: unit count did not increase ({unitsBefore} -> {unitsAfter})."); return false;
            }

            sb.AppendLine($"Phase 5 PASS: LeviesIssued={levy.LeviesIssued} UnitCount={unitsBefore}->{unitsAfter}.");
            return true;
        }
    }
}
#endif
