#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Conviction;
using Bloodlines.Debug;
using Bloodlines.Dynasties;
using Bloodlines.GameTime;
using Bloodlines.PlayerDiplomacy;
using Bloodlines.Systems;
using Unity.Collections;
using Unity.Core;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Dedicated smoke validator for player marriage acceptance. Exercises the
    /// public debug surface through the player-owned acceptance path:
    /// 1. Baseline: no pending proposals and no marriages.
    /// 2. Valid accept: incoming proposal creates primary + mirror marriage,
    ///    legitimacy +2 lands on both sides, hostility drops, oathkeeping +2
    ///    lands on both sides, and the 30-day declaration jump fires.
    /// 3. No pending proposal: accepting a bogus entity index does not create
    ///    a marriage or declaration.
    /// 4. Heir regency: target-side head fallen causes legitimacy cost 1,
    ///    so the target nets +1 legitimacy and Stewardship -1 before the +2.
    /// </summary>
    public static class BloodlinesPlayerMarriageAcceptanceSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-player-marriage-acceptance-smoke.log";

        [MenuItem("Bloodlines/Player Diplomacy/Run Player Marriage Acceptance Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchPlayerMarriageAcceptanceSmokeValidation() =>
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
                message = "Player marriage acceptance smoke validation errored: " + e;
            }

            var artifact = "BLOODLINES_PLAYER_MARRIAGE_ACCEPTANCE_SMOKE " +
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
            ok &= RunValidAcceptPhase(lines);
            ok &= RunNoPendingProposalPhase(lines);
            ok &= RunHeirRegencyCostPhase(lines);
            report = lines.ToString();
            return ok;
        }

        private static bool RunBaselinePhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-marriage-acceptance-phase1");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 18f);
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

            if (CountMarriages(entityManager) != 0)
            {
                lines.AppendLine("Phase 1 FAIL: expected no marriage entities on clean baseline.");
                return false;
            }

            lines.AppendLine("Phase 1 PASS: PendingProposalCount=0 and MarriageCount=0");
            return true;
        }

        private static bool RunValidAcceptPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-marriage-acceptance-phase2");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 20f);
            var playerFaction = SeedKingdomFaction(
                entityManager,
                "player",
                legitimacy: 80f,
                oathkeeping: 3f,
                hostileTo: "enemy");
            var enemyFaction = SeedKingdomFaction(
                entityManager,
                "enemy",
                legitimacy: 70f,
                oathkeeping: 1f,
                hostileTo: "player");
            var proposalEntity = SeedPendingProposal(
                entityManager,
                "enemy",
                "enemy-bloodline-heir",
                "player",
                "player-bloodline-heir");

            if (!debugScope.CommandSurface.TryDebugGetPlayerMarriageProposals("player", out var beforeReadout))
            {
                lines.AppendLine("Phase 2 FAIL: could not read pending proposals before accept.");
                return false;
            }

            if (!TryParseFirstProposalEntityIndex(beforeReadout, out var proposalIndex) ||
                proposalIndex != proposalEntity.Index)
            {
                lines.AppendLine($"Phase 2 FAIL: expected proposal entity index {proposalEntity.Index}, got '{beforeReadout}'.");
                return false;
            }

            if (!debugScope.CommandSurface.TryDebugIssuePlayerMarriageAccept(proposalIndex))
            {
                lines.AppendLine("Phase 2 FAIL: could not queue player marriage accept request.");
                return false;
            }

            TickOnce(world);

            if (!debugScope.CommandSurface.TryDebugGetPlayerMarriageProposals("player", out var afterReadout))
            {
                lines.AppendLine("Phase 2 FAIL: could not read proposal ledger after accept.");
                return false;
            }

            if (!afterReadout.Contains("PendingProposalCount=0", StringComparison.Ordinal))
            {
                lines.AppendLine($"Phase 2 FAIL: expected no pending proposals after accept, got '{afterReadout}'.");
                return false;
            }

            int marriageCount = CountMarriages(entityManager);
            int primaryCount = CountPrimaryMarriages(entityManager);
            if (marriageCount != 2 || primaryCount != 1)
            {
                lines.AppendLine($"Phase 2 FAIL: expected 2 marriage records with 1 primary, got marriageCount={marriageCount}, primaryCount={primaryCount}.");
                return false;
            }

            if (!TryGetPrimaryMarriage(entityManager, out var primaryMarriage))
            {
                lines.AppendLine("Phase 2 FAIL: primary marriage record not found after accept.");
                return false;
            }

            var playerDynasty = entityManager.GetComponentData<DynastyStateComponent>(playerFaction);
            var enemyDynasty = entityManager.GetComponentData<DynastyStateComponent>(enemyFaction);
            var playerConviction = entityManager.GetComponentData<ConvictionComponent>(playerFaction);
            var enemyConviction = entityManager.GetComponentData<ConvictionComponent>(enemyFaction);
            var dualClock = GetDualClock(entityManager);
            var acceptedProposal = entityManager.GetComponentData<MarriageProposalComponent>(proposalEntity);

            if (acceptedProposal.Status != MarriageProposalStatus.Accepted)
            {
                lines.AppendLine($"Phase 2 FAIL: proposal status expected Accepted, got {acceptedProposal.Status}.");
                return false;
            }

            if (!Approximately(playerDynasty.Legitimacy, 82f) ||
                !Approximately(enemyDynasty.Legitimacy, 72f))
            {
                lines.AppendLine(
                    $"Phase 2 FAIL: expected legitimacy 82/72 after accept, got player={playerDynasty.Legitimacy}, enemy={enemyDynasty.Legitimacy}.");
                return false;
            }

            if (!Approximately(playerConviction.Oathkeeping, 5f) ||
                !Approximately(enemyConviction.Oathkeeping, 3f))
            {
                lines.AppendLine(
                    $"Phase 2 FAIL: expected oathkeeping +2 both sides, got player={playerConviction.Oathkeeping}, enemy={enemyConviction.Oathkeeping}.");
                return false;
            }

            if (IsHostile(entityManager, playerFaction, "enemy") ||
                IsHostile(entityManager, enemyFaction, "player"))
            {
                lines.AppendLine("Phase 2 FAIL: hostility should be cleared both ways after accept.");
                return false;
            }

            if (!Approximately(dualClock.InWorldDays, 50f) ||
                dualClock.DeclarationCount != 1)
            {
                lines.AppendLine(
                    $"Phase 2 FAIL: expected dual clock 50 days with 1 declaration, got days={dualClock.InWorldDays}, declarations={dualClock.DeclarationCount}.");
                return false;
            }

            if (!primaryMarriage.IsPrimary ||
                !primaryMarriage.HeadFactionId.Equals(new FixedString32Bytes("enemy")) ||
                !primaryMarriage.SpouseFactionId.Equals(new FixedString32Bytes("player")) ||
                !Approximately(primaryMarriage.MarriedAtInWorldDays, 20f) ||
                !Approximately(primaryMarriage.ExpectedChildAtInWorldDays, 300f))
            {
                lines.AppendLine(
                    $"Phase 2 FAIL: primary marriage fields drifted: head={primaryMarriage.HeadFactionId}, spouse={primaryMarriage.SpouseFactionId}, marriedAt={primaryMarriage.MarriedAtInWorldDays}, expectedChildAt={primaryMarriage.ExpectedChildAtInWorldDays}.");
                return false;
            }

            lines.AppendLine(
                $"Phase 2 PASS: proposal accepted, marriageCount=2, legitimacy=82/72, oathkeeping=5/3, dualClockDays={dualClock.InWorldDays}");
            return true;
        }

        private static bool RunNoPendingProposalPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-marriage-acceptance-phase3");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 22f);
            SeedKingdomFaction(entityManager, "player");
            SeedKingdomFaction(entityManager, "enemy");

            if (!debugScope.CommandSurface.TryDebugIssuePlayerMarriageAccept(999999))
            {
                lines.AppendLine("Phase 3 FAIL: could not queue no-pending accept request.");
                return false;
            }

            TickOnce(world);

            var dualClock = GetDualClock(entityManager);
            if (CountMarriages(entityManager) != 0 || dualClock.DeclarationCount != 0)
            {
                lines.AppendLine(
                    $"Phase 3 FAIL: no-pending gate should leave state untouched, got marriageCount={CountMarriages(entityManager)}, declarations={dualClock.DeclarationCount}.");
                return false;
            }

            lines.AppendLine("Phase 3 PASS: bogus accept request created no marriage and no declaration");
            return true;
        }

        private static bool RunHeirRegencyCostPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-marriage-acceptance-phase4");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 24f);
            var playerFaction = SeedKingdomFaction(
                entityManager,
                "player",
                legitimacy: 80f,
                stewardship: 3f);
            SeedKingdomFaction(entityManager, "enemy", legitimacy: 70f);
            MarkMemberStatus(entityManager, playerFaction, "player-bloodline-head", DynastyMemberStatus.Fallen);
            var proposalEntity = SeedPendingProposal(
                entityManager,
                "enemy",
                "enemy-bloodline-heir",
                "player",
                "player-bloodline-heir");

            if (!debugScope.CommandSurface.TryDebugGetPlayerMarriageProposals("player", out var readout) ||
                !TryParseFirstProposalEntityIndex(readout, out var proposalIndex) ||
                proposalIndex != proposalEntity.Index)
            {
                lines.AppendLine($"Phase 4 FAIL: could not resolve heir-regency proposal entity index from '{readout}'.");
                return false;
            }

            if (!debugScope.CommandSurface.TryDebugIssuePlayerMarriageAccept(proposalIndex))
            {
                lines.AppendLine("Phase 4 FAIL: could not queue heir-regency accept request.");
                return false;
            }

            TickOnce(world);

            var playerDynasty = entityManager.GetComponentData<DynastyStateComponent>(playerFaction);
            var playerConviction = entityManager.GetComponentData<ConvictionComponent>(playerFaction);

            if (!Approximately(playerDynasty.Legitimacy, 81f))
            {
                lines.AppendLine(
                    $"Phase 4 FAIL: expected heir-regency net legitimacy 81 (80 - 1 + 2), got {playerDynasty.Legitimacy}.");
                return false;
            }

            if (!Approximately(playerConviction.Stewardship, 2f))
            {
                lines.AppendLine(
                    $"Phase 4 FAIL: expected Stewardship 2 after cost deduction, got {playerConviction.Stewardship}.");
                return false;
            }

            lines.AppendLine(
                $"Phase 4 PASS: heir-regency cost applied, legitimacy={playerDynasty.Legitimacy}, stewardship={playerConviction.Stewardship}");
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
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<PlayerMarriageAcceptSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<MarriageGestationSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<DualClockDeclarationSystem>());
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
            float legitimacy = 80f,
            float stewardship = 0f,
            float oathkeeping = 0f,
            string hostileTo = null)
        {
            var entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(ConvictionComponent),
                typeof(FaithStateComponent),
                typeof(HostilityComponent));
            entityManager.SetComponentData(entity, new FactionComponent { FactionId = factionId });
            entityManager.SetComponentData(entity, new FactionKindComponent { Kind = FactionKind.Kingdom });

            var conviction = new ConvictionComponent
            {
                Stewardship = stewardship,
                Oathkeeping = oathkeeping,
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

            var dynasty = entityManager.GetComponentData<DynastyStateComponent>(entity);
            dynasty.Legitimacy = legitimacy;
            entityManager.SetComponentData(entity, dynasty);

            if (!string.IsNullOrWhiteSpace(hostileTo))
            {
                entityManager.GetBuffer<HostilityComponent>(entity).Add(new HostilityComponent
                {
                    HostileFactionId = new FixedString32Bytes(hostileTo),
                });
            }

            return entity;
        }

        private static Entity SeedPendingProposal(
            EntityManager entityManager,
            string sourceFactionId,
            string sourceMemberId,
            string targetFactionId,
            string targetMemberId)
        {
            var proposalEntity = entityManager.CreateEntity(typeof(MarriageProposalComponent));
            entityManager.SetComponentData(proposalEntity, new MarriageProposalComponent
            {
                ProposalId = new FixedString64Bytes("incoming-player-marriage"),
                SourceFactionId = new FixedString32Bytes(sourceFactionId),
                SourceMemberId = new FixedString64Bytes(sourceMemberId),
                TargetFactionId = new FixedString32Bytes(targetFactionId),
                TargetMemberId = new FixedString64Bytes(targetMemberId),
                Status = MarriageProposalStatus.Pending,
                ProposedAtInWorldDays = 5f,
                ExpiresAtInWorldDays = 95f,
            });
            return proposalEntity;
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

        private static int CountMarriages(EntityManager entityManager)
        {
            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<MarriageComponent>());
            int count = query.CalculateEntityCount();
            query.Dispose();
            return count;
        }

        private static int CountPrimaryMarriages(EntityManager entityManager)
        {
            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<MarriageComponent>());
            if (query.IsEmpty)
            {
                query.Dispose();
                return 0;
            }

            using var marriages = query.ToComponentDataArray<MarriageComponent>(Allocator.Temp);
            query.Dispose();
            int count = 0;
            for (int i = 0; i < marriages.Length; i++)
            {
                if (marriages[i].IsPrimary)
                {
                    count++;
                }
            }

            return count;
        }

        private static bool TryGetPrimaryMarriage(
            EntityManager entityManager,
            out MarriageComponent marriage)
        {
            marriage = default;
            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<MarriageComponent>());
            if (query.IsEmpty)
            {
                query.Dispose();
                return false;
            }

            using var marriages = query.ToComponentDataArray<MarriageComponent>(Allocator.Temp);
            query.Dispose();
            for (int i = 0; i < marriages.Length; i++)
            {
                if (!marriages[i].IsPrimary)
                {
                    continue;
                }

                marriage = marriages[i];
                return true;
            }

            return false;
        }

        private static DualClockComponent GetDualClock(EntityManager entityManager)
        {
            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<DualClockComponent>());
            var dualClock = query.GetSingleton<DualClockComponent>();
            query.Dispose();
            return dualClock;
        }

        private static bool IsHostile(
            EntityManager entityManager,
            Entity factionEntity,
            string hostileFactionId)
        {
            if (factionEntity == Entity.Null ||
                !entityManager.HasBuffer<HostilityComponent>(factionEntity))
            {
                return false;
            }

            var hostility = entityManager.GetBuffer<HostilityComponent>(factionEntity);
            var hostileKey = new FixedString32Bytes(hostileFactionId);
            for (int i = 0; i < hostility.Length; i++)
            {
                if (hostility[i].HostileFactionId.Equals(hostileKey))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool TryParseFirstProposalEntityIndex(string readout, out int entityIndex)
        {
            entityIndex = -1;
            if (string.IsNullOrWhiteSpace(readout))
            {
                return false;
            }

            var marker = "EntityIndex=";
            int start = readout.IndexOf(marker, StringComparison.Ordinal);
            if (start < 0)
            {
                return false;
            }

            start += marker.Length;
            int end = readout.IndexOf('|', start);
            string token = end >= 0 ? readout[start..end] : readout[start..];
            return int.TryParse(token, out entityIndex);
        }

        private static bool Approximately(float actual, float expected)
        {
            return Mathf.Abs(actual - expected) <= 0.01f;
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

                hostObject = new GameObject("BloodlinesPlayerMarriageAcceptanceSmokeValidation_CommandSurface")
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
