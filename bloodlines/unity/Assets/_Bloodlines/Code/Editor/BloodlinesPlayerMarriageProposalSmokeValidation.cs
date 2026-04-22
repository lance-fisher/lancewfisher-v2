#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Conviction;
using Bloodlines.Debug;
using Bloodlines.Dynasties;
using Bloodlines.GameTime;
using Bloodlines.PlayerDiplomacy;
using Unity.Collections;
using Unity.Core;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Dedicated smoke validator for player marriage proposal execution.
    /// Proves four phases end-to-end through the public debug surface:
    /// 1. Baseline: no pending proposals.
    /// 2. Valid proposal: proposal entity is created and heir-regency cost
    ///    reduces source legitimacy by 1.
    /// 3. Duplicate proposal: second issue attempt does not create another
    ///    pending proposal.
    /// 4. Already-married gate: an active marriage blocks the proposal.
    /// </summary>
    public static class BloodlinesPlayerMarriageProposalSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-player-marriage-proposal-smoke.log";

        [MenuItem("Bloodlines/Player Diplomacy/Run Player Marriage Proposal Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchPlayerMarriageProposalSmokeValidation() =>
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
                message = "Player marriage proposal smoke validation errored: " + e;
            }

            var artifact = "BLOODLINES_PLAYER_MARRIAGE_PROPOSAL_SMOKE " +
                           (success ? "PASS" : "FAIL") + Environment.NewLine + message;
            UnityDebug.Log(artifact);
            try
            {
                var logPath = Path.GetFullPath(Path.Combine(Application.dataPath, ArtifactPath));
                Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
                File.WriteAllText(logPath, artifact);
            }
            catch
            {
            }

            if (batchMode)
            {
                EditorApplication.Exit(success ? 0 : 1);
            }
        }

        private static bool RunAllPhases(out string report)
        {
            var lines = new System.Text.StringBuilder();
            bool ok = true;
            ok &= RunBaselinePhase(lines);
            ok &= RunValidProposalPhase(lines);
            ok &= RunDuplicateGatePhase(lines);
            ok &= RunAlreadyMarriedPhase(lines);
            report = lines.ToString();
            return ok;
        }

        private static bool RunBaselinePhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-marriage-proposal-phase1");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 10f);
            SeedKingdomFaction(entityManager, "player");
            SeedKingdomFaction(entityManager, "enemy");
            TickOnce(world);

            if (!debugScope.CommandSurface.TryDebugGetPlayerMarriageProposals("player", out var readout))
            {
                lines.AppendLine("Phase 1 FAIL: could not read player marriage proposals.");
                return false;
            }

            if (!readout.Contains("PendingProposalCount=0", StringComparison.Ordinal))
            {
                lines.AppendLine($"Phase 1 FAIL: expected zero pending proposals, got '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 1 PASS: PendingProposalCount=0");
            return true;
        }

        private static bool RunValidProposalPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-marriage-proposal-phase2");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 12f);
            var playerFaction = SeedKingdomFaction(entityManager, "player", stewardship: 2f);
            SeedKingdomFaction(entityManager, "enemy");
            MarkMemberStatus(entityManager, playerFaction, "player-bloodline-head", DynastyMemberStatus.Fallen);

            float legitimacyBefore = entityManager.GetComponentData<DynastyStateComponent>(playerFaction).Legitimacy;

            if (!debugScope.CommandSurface.TryDebugIssuePlayerMarriageProposal("player", "enemy-bloodline-heir"))
            {
                lines.AppendLine("Phase 2 FAIL: could not queue player marriage proposal request.");
                return false;
            }

            TickOnce(world);

            if (!debugScope.CommandSurface.TryDebugGetPlayerMarriageProposals("player", out var readout))
            {
                lines.AppendLine("Phase 2 FAIL: could not read proposal ledger after request.");
                return false;
            }

            int proposalCount = CountPendingProposals(entityManager);
            var playerDynasty = entityManager.GetComponentData<DynastyStateComponent>(playerFaction);
            var playerConviction = entityManager.GetComponentData<ConvictionComponent>(playerFaction);
            if (proposalCount != 1 ||
                !readout.Contains("PendingProposalCount=1", StringComparison.Ordinal) ||
                !readout.Contains("SourceMemberId=player-bloodline-heir", StringComparison.Ordinal) ||
                !readout.Contains("TargetMemberId=enemy-bloodline-heir", StringComparison.Ordinal))
            {
                lines.AppendLine($"Phase 2 FAIL: expected one heir-to-heir pending proposal, got '{readout}'.");
                return false;
            }

            if (playerDynasty.Legitimacy != legitimacyBefore - 1f || playerConviction.Stewardship != 1f)
            {
                lines.AppendLine(
                    $"Phase 2 FAIL: expected heir-regency cost legitimacy {legitimacyBefore - 1f} and stewardship 1, " +
                    $"got legitimacy {playerDynasty.Legitimacy} and stewardship {playerConviction.Stewardship}.");
                return false;
            }

            lines.AppendLine(
                $"Phase 2 PASS: PendingProposalCount=1, source=player-bloodline-heir, target=enemy-bloodline-heir, legitimacy={playerDynasty.Legitimacy}");
            return true;
        }

        private static bool RunDuplicateGatePhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-marriage-proposal-phase3");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 14f);
            SeedKingdomFaction(entityManager, "player");
            SeedKingdomFaction(entityManager, "enemy");

            if (!debugScope.CommandSurface.TryDebugIssuePlayerMarriageProposal("player", "enemy-bloodline-heir"))
            {
                lines.AppendLine("Phase 3 FAIL: first proposal request could not be queued.");
                return false;
            }

            TickOnce(world);

            if (!debugScope.CommandSurface.TryDebugIssuePlayerMarriageProposal("player", "enemy-bloodline-heir"))
            {
                lines.AppendLine("Phase 3 FAIL: duplicate proposal request could not be queued for gate test.");
                return false;
            }

            TickOnce(world);

            int proposalCount = CountPendingProposals(entityManager);
            if (!debugScope.CommandSurface.TryDebugGetPlayerMarriageProposals("player", out var readout))
            {
                lines.AppendLine("Phase 3 FAIL: could not read proposals after duplicate request.");
                return false;
            }

            if (proposalCount != 1 || !readout.Contains("PendingProposalCount=1", StringComparison.Ordinal))
            {
                lines.AppendLine($"Phase 3 FAIL: duplicate gate drifted, expected 1 pending proposal, got '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 3 PASS: duplicate request preserved a single pending proposal");
            return true;
        }

        private static bool RunAlreadyMarriedPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-marriage-proposal-phase4");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 16f);
            SeedKingdomFaction(entityManager, "player");
            SeedKingdomFaction(entityManager, "enemy");
            SeedActiveMarriage(
                entityManager,
                "player",
                "player-bloodline-heir",
                "enemy",
                "enemy-bloodline-heir");

            if (!debugScope.CommandSurface.TryDebugIssuePlayerMarriageProposal("player", "enemy-bloodline-heir"))
            {
                lines.AppendLine("Phase 4 FAIL: already-married phase could not queue request.");
                return false;
            }

            TickOnce(world);

            int proposalCount = CountPendingProposals(entityManager);
            if (proposalCount != 0)
            {
                lines.AppendLine($"Phase 4 FAIL: expected active marriage to block proposal, got {proposalCount} pending proposals.");
                return false;
            }

            lines.AppendLine("Phase 4 PASS: active marriage blocked proposal creation");
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<MarriageDeathDissolutionSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<MarriageProposalExpirationSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<PlayerMarriageProposalSystem>());
            simulationGroup.SortSystems();
            return world;
        }

        private static void TickOnce(World world)
        {
            world.SetTime(new TimeData(0d, 0.05f));
            world.Update();
        }

        private static void SeedDualClock(EntityManager entityManager, float inWorldDays)
        {
            var clockEntity = entityManager.CreateEntity(typeof(DualClockComponent));
            entityManager.SetComponentData(clockEntity, new DualClockComponent
            {
                InWorldDays = inWorldDays,
                DaysPerRealSecond = 2f,
                DeclarationCount = 0,
            });
            entityManager.AddBuffer<DeclareInWorldTimeRequest>(clockEntity);
        }

        private static Entity SeedKingdomFaction(
            EntityManager entityManager,
            string factionId,
            float stewardship = 0f)
        {
            var entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(ConvictionComponent),
                typeof(FaithStateComponent));
            entityManager.SetComponentData(entity, new FactionComponent { FactionId = factionId });
            entityManager.SetComponentData(entity, new FactionKindComponent { Kind = FactionKind.Kingdom });

            var conviction = new ConvictionComponent
            {
                Stewardship = stewardship,
                Band = ConvictionBand.Neutral,
            };
            ConvictionScoring.Refresh(ref conviction);
            entityManager.SetComponentData(entity, conviction);
            entityManager.SetComponentData(entity, new FaithStateComponent
            {
                SelectedFaith = CovenantId.None,
                DoctrinePath = DoctrinePath.Unassigned,
                Intensity = 0f,
                Level = 0,
            });

            DynastyBootstrap.AttachDynasty(entityManager, entity, new FixedString32Bytes(factionId));
            return entity;
        }

        private static void MarkMemberStatus(
            EntityManager entityManager,
            Entity factionEntity,
            string memberId,
            DynastyMemberStatus status)
        {
            var memberEntity = FindMemberEntity(entityManager, factionEntity, memberId);
            if (memberEntity == Entity.Null)
            {
                throw new InvalidOperationException("Dynasty member not found for status mutation: " + memberId);
            }

            var member = entityManager.GetComponentData<DynastyMemberComponent>(memberEntity);
            member.Status = status;
            entityManager.SetComponentData(memberEntity, member);
        }

        private static Entity FindMemberEntity(
            EntityManager entityManager,
            Entity factionEntity,
            string memberId)
        {
            if (!entityManager.HasBuffer<DynastyMemberRef>(factionEntity))
            {
                return Entity.Null;
            }

            var members = entityManager.GetBuffer<DynastyMemberRef>(factionEntity);
            var memberKey = new FixedString64Bytes(memberId);
            for (int i = 0; i < members.Length; i++)
            {
                var memberEntity = members[i].Member;
                if (memberEntity == Entity.Null ||
                    !entityManager.HasComponent<DynastyMemberComponent>(memberEntity))
                {
                    continue;
                }

                var member = entityManager.GetComponentData<DynastyMemberComponent>(memberEntity);
                if (member.MemberId.Equals(memberKey))
                {
                    return memberEntity;
                }
            }

            return Entity.Null;
        }

        private static void SeedActiveMarriage(
            EntityManager entityManager,
            string headFactionId,
            string headMemberId,
            string spouseFactionId,
            string spouseMemberId)
        {
            var marriageEntity = entityManager.CreateEntity(typeof(MarriageComponent));
            entityManager.SetComponentData(marriageEntity, new MarriageComponent
            {
                MarriageId = new FixedString64Bytes("existing-marriage"),
                HeadFactionId = new FixedString32Bytes(headFactionId),
                HeadMemberId = new FixedString64Bytes(headMemberId),
                SpouseFactionId = new FixedString32Bytes(spouseFactionId),
                SpouseMemberId = new FixedString64Bytes(spouseMemberId),
                MarriedAtInWorldDays = 3f,
                ExpectedChildAtInWorldDays = 283f,
                IsPrimary = true,
                ChildGenerated = false,
                Dissolved = false,
            });
        }

        private static int CountPendingProposals(EntityManager entityManager)
        {
            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<MarriageProposalComponent>());
            if (query.IsEmpty)
            {
                query.Dispose();
                return 0;
            }

            using var proposals = query.ToComponentDataArray<MarriageProposalComponent>(Allocator.Temp);
            query.Dispose();
            int count = 0;
            for (int i = 0; i < proposals.Length; i++)
            {
                if (proposals[i].Status == MarriageProposalStatus.Pending)
                {
                    count++;
                }
            }

            return count;
        }

        private sealed class DebugCommandSurfaceScope : IDisposable
        {
            private readonly World previousDefaultWorld;
            private readonly GameObject hostObject;

            public BloodlinesDebugCommandSurface CommandSurface { get; }

            public DebugCommandSurfaceScope(World world)
            {
                previousDefaultWorld = World.DefaultGameObjectInjectionWorld;
                World.DefaultGameObjectInjectionWorld = world;

                hostObject = new GameObject("BloodlinesPlayerMarriageProposalSmokeValidation_CommandSurface")
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
                CommandSurface = hostObject.AddComponent<BloodlinesDebugCommandSurface>();
            }

            public void Dispose()
            {
                UnityEngine.Object.DestroyImmediate(hostObject);
                World.DefaultGameObjectInjectionWorld = previousDefaultWorld;
            }
        }
    }
}
#endif
