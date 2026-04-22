#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Conviction;
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
    /// Dedicated smoke validator for the remaining player-marriage dissolution
    /// proof surface. It intentionally reuses the already-landed
    /// MarriageDeathDissolutionSystem from the retired dynasty parity lane
    /// rather than duplicating that runtime under PlayerDiplomacy.
    ///
    /// Proves:
    /// 1. an accepted marriage remains active while both members are alive;
    /// 2. ruler death triggers succession and dissolves the mirrored marriage;
    /// 3. active marriages still gestate a child when no death intervenes.
    /// </summary>
    public static class BloodlinesPlayerMarriageDissolutionSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-player-marriage-dissolution-smoke.log";

        [MenuItem("Bloodlines/Player Diplomacy/Run Player Marriage Dissolution Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchPlayerMarriageDissolutionSmokeValidation() =>
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
                message = "Player marriage dissolution smoke validation errored: " + e;
            }

            var artifact = "BLOODLINES_PLAYER_MARRIAGE_DISSOLUTION_SMOKE " +
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
            ok &= RunAliveMarriagePhase(lines);
            ok &= RunRulerDeathSuccessionPhase(lines);
            ok &= RunGestationSurvivesPhase(lines);
            report = lines.ToString();
            return ok;
        }

        private static bool RunAliveMarriagePhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-marriage-dissolution-phase1");
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 20f);
            SeedKingdomFaction(entityManager, "player", legitimacy: 80f);
            SeedKingdomFaction(entityManager, "enemy", legitimacy: 70f);

            if (!CreateAcceptedMarriage(
                    world,
                    entityManager,
                    "phase1-alive-marriage",
                    "enemy",
                    "enemy-bloodline-heir",
                    "player",
                    "player-bloodline-heir",
                    out var marriageId,
                    out var failure))
            {
                lines.AppendLine("Phase 1 FAIL: " + failure);
                return false;
            }

            TickOnce(world);

            if (!TryGetMarriagePair(entityManager, marriageId, out _, out var primary, out _, out var mirror))
            {
                lines.AppendLine("Phase 1 FAIL: accepted marriage pair was not found after baseline tick.");
                return false;
            }

            if (primary.Dissolved || mirror.Dissolved || CountDissolvedMarriages(entityManager) != 0)
            {
                lines.AppendLine("Phase 1 FAIL: alive spouses should not trigger dissolution.");
                return false;
            }

            lines.AppendLine($"Phase 1 PASS: alive marriage remained active with MarriageId={marriageId}");
            return true;
        }

        private static bool RunRulerDeathSuccessionPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-marriage-dissolution-phase2");
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 20f);
            var playerFaction = SeedKingdomFaction(entityManager, "player", legitimacy: 80f, oathkeeping: 2f);
            var enemyFaction = SeedKingdomFaction(entityManager, "enemy", legitimacy: 70f, oathkeeping: 0f);

            if (!CreateAcceptedMarriage(
                    world,
                    entityManager,
                    "phase2-ruler-death",
                    "enemy",
                    "enemy-bloodline-heir",
                    "player",
                    "player-bloodline-head",
                    out var marriageId,
                    out var failure))
            {
                lines.AppendLine("Phase 2 FAIL: " + failure);
                return false;
            }

            var playerDynastyBeforeDissolution = entityManager.GetComponentData<DynastyStateComponent>(playerFaction);
            var enemyDynastyBeforeDissolution = entityManager.GetComponentData<DynastyStateComponent>(enemyFaction);
            var playerConvictionBeforeDissolution = entityManager.GetComponentData<ConvictionComponent>(playerFaction);
            var enemyConvictionBeforeDissolution = entityManager.GetComponentData<ConvictionComponent>(enemyFaction);
            var deathTickDays = GetDualClock(entityManager).InWorldDays;

            MarkMemberStatus(entityManager, playerFaction, "player-bloodline-head", DynastyMemberStatus.Fallen);
            TickOnce(world);

            if (!TryGetMarriagePair(entityManager, marriageId, out _, out var primary, out _, out var mirror))
            {
                lines.AppendLine("Phase 2 FAIL: dissolved marriage pair could not be found.");
                return false;
            }

            if (!primary.Dissolved || !mirror.Dissolved)
            {
                lines.AppendLine("Phase 2 FAIL: ruler death did not dissolve both marriage records.");
                return false;
            }

            if (!Approximately(primary.DissolvedAtInWorldDays, deathTickDays) ||
                !Approximately(mirror.DissolvedAtInWorldDays, deathTickDays))
            {
                lines.AppendLine(
                    $"Phase 2 FAIL: dissolution day drifted. expected={deathTickDays}, primary={primary.DissolvedAtInWorldDays}, mirror={mirror.DissolvedAtInWorldDays}.");
                return false;
            }

            var playerDynastyAfterDissolution = entityManager.GetComponentData<DynastyStateComponent>(playerFaction);
            var enemyDynastyAfterDissolution = entityManager.GetComponentData<DynastyStateComponent>(enemyFaction);
            var playerConvictionAfterDissolution = entityManager.GetComponentData<ConvictionComponent>(playerFaction);
            var enemyConvictionAfterDissolution = entityManager.GetComponentData<ConvictionComponent>(enemyFaction);

            if (!Approximately(playerDynastyAfterDissolution.Legitimacy, playerDynastyBeforeDissolution.Legitimacy - MarriageDeathDissolutionSystem.LegitimacyLoss) ||
                !Approximately(enemyDynastyAfterDissolution.Legitimacy, enemyDynastyBeforeDissolution.Legitimacy - MarriageDeathDissolutionSystem.LegitimacyLoss))
            {
                lines.AppendLine(
                    $"Phase 2 FAIL: legitimacy loss drifted. player={playerDynastyAfterDissolution.Legitimacy}, enemy={enemyDynastyAfterDissolution.Legitimacy}.");
                return false;
            }

            if (!Approximately(playerConvictionAfterDissolution.Oathkeeping, playerConvictionBeforeDissolution.Oathkeeping + MarriageDeathDissolutionSystem.OathkeepingGain) ||
                !Approximately(enemyConvictionAfterDissolution.Oathkeeping, enemyConvictionBeforeDissolution.Oathkeeping + MarriageDeathDissolutionSystem.OathkeepingGain))
            {
                lines.AppendLine(
                    $"Phase 2 FAIL: oathkeeping mourning drifted. player={playerConvictionAfterDissolution.Oathkeeping}, enemy={enemyConvictionAfterDissolution.Oathkeeping}.");
                return false;
            }

            var successorEntity = FindMemberEntity(entityManager, playerFaction, new FixedString64Bytes("player-bloodline-heir"));
            if (successorEntity == Entity.Null)
            {
                lines.AppendLine("Phase 2 FAIL: player successor entity not found after ruler death.");
                return false;
            }

            var successor = entityManager.GetComponentData<DynastyMemberComponent>(successorEntity);
            if (successor.Status != DynastyMemberStatus.Ruling ||
                successor.Role != DynastyRole.HeadOfBloodline)
            {
                lines.AppendLine(
                    $"Phase 2 FAIL: succession did not promote the heir. status={successor.Status}, role={successor.Role}.");
                return false;
            }

            lines.AppendLine(
                $"Phase 2 PASS: ruler death dissolved MarriageId={marriageId} at day={deathTickDays:0.00} and promoted player-bloodline-heir");
            return true;
        }

        private static bool RunGestationSurvivesPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-marriage-dissolution-phase3");
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 0f);
            SeedKingdomFaction(entityManager, "player", legitimacy: 80f);
            SeedKingdomFaction(entityManager, "enemy", legitimacy: 70f);

            if (!CreateAcceptedMarriage(
                    world,
                    entityManager,
                    "phase3-gestation",
                    "enemy",
                    "enemy-bloodline-heir",
                    "player",
                    "player-bloodline-heir",
                    out var marriageId,
                    out var failure))
            {
                lines.AppendLine("Phase 3 FAIL: " + failure);
                return false;
            }

            if (!TryGetMarriagePair(entityManager, marriageId, out var primaryEntity, out var primary, out _, out _))
            {
                lines.AppendLine("Phase 3 FAIL: primary marriage record not found before gestation advance.");
                return false;
            }

            SetInWorldDays(entityManager, primary.ExpectedChildAtInWorldDays);
            TickOnce(world);

            if (!TryGetMarriagePair(entityManager, marriageId, out primaryEntity, out primary, out _, out _))
            {
                lines.AppendLine("Phase 3 FAIL: primary marriage record not found after gestation advance.");
                return false;
            }

            if (primary.Dissolved)
            {
                lines.AppendLine("Phase 3 FAIL: active gestation marriage should not be dissolved.");
                return false;
            }

            if (!primary.ChildGenerated)
            {
                lines.AppendLine("Phase 3 FAIL: gestation did not generate a child on the live marriage.");
                return false;
            }

            if (!entityManager.HasBuffer<MarriageChildElement>(primaryEntity))
            {
                lines.AppendLine("Phase 3 FAIL: gestation did not attach a marriage child buffer.");
                return false;
            }

            var children = entityManager.GetBuffer<MarriageChildElement>(primaryEntity);
            if (children.Length != 1)
            {
                lines.AppendLine($"Phase 3 FAIL: expected exactly 1 child id, saw {children.Length}.");
                return false;
            }

            var headFaction = FindFactionEntity(entityManager, primary.HeadFactionId);
            var childEntity = FindMemberEntity(entityManager, headFaction, children[0].ChildMemberId);
            if (childEntity == Entity.Null)
            {
                lines.AppendLine("Phase 3 FAIL: gestated child was not added to the head dynasty roster.");
                return false;
            }

            var child = entityManager.GetComponentData<DynastyMemberComponent>(childEntity);
            if (child.Role != DynastyRole.HeirDesignate ||
                child.Status != DynastyMemberStatus.Active)
            {
                lines.AppendLine(
                    $"Phase 3 FAIL: gestated child role/status drifted. role={child.Role}, status={child.Status}.");
                return false;
            }

            lines.AppendLine(
                $"Phase 3 PASS: live marriage gestated childId={children[0].ChildMemberId} for headFaction={primary.HeadFactionId}");
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<DynastyAgingSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<DynastySuccessionSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<MarriageDeathDissolutionSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<MarriageProposalExpirationSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<PlayerMarriageProposalSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<PlayerMarriageAcceptSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<MarriageGestationSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<DualClockDeclarationSystem>());
            simulationGroup.SortSystems();
            return world;
        }

        private static bool CreateAcceptedMarriage(
            World world,
            EntityManager entityManager,
            string proposalId,
            string sourceFactionId,
            string sourceMemberId,
            string targetFactionId,
            string targetMemberId,
            out FixedString64Bytes marriageId,
            out string failure)
        {
            failure = string.Empty;
            marriageId = BuildMarriageId(proposalId);
            var proposalEntity = SeedPendingProposal(
                entityManager,
                proposalId,
                sourceFactionId,
                sourceMemberId,
                targetFactionId,
                targetMemberId);

            var requestEntity = entityManager.CreateEntity(typeof(PlayerMarriageAcceptRequestComponent));
            entityManager.SetComponentData(requestEntity, new PlayerMarriageAcceptRequestComponent
            {
                ProposalEntityIndex = proposalEntity.Index,
            });

            TickOnce(world);

            if (!TryGetMarriagePair(entityManager, marriageId, out _, out _, out _, out _))
            {
                failure = "accept request did not create the mirrored marriage pair.";
                return false;
            }

            var acceptedProposal = entityManager.GetComponentData<MarriageProposalComponent>(proposalEntity);
            if (acceptedProposal.Status != MarriageProposalStatus.Accepted)
            {
                failure = $"proposal status expected Accepted, got {acceptedProposal.Status}.";
                return false;
            }

            return true;
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
            string proposalId,
            string sourceFactionId,
            string sourceMemberId,
            string targetFactionId,
            string targetMemberId)
        {
            var proposalEntity = entityManager.CreateEntity(typeof(MarriageProposalComponent));
            entityManager.SetComponentData(proposalEntity, new MarriageProposalComponent
            {
                ProposalId = new FixedString64Bytes(proposalId),
                SourceFactionId = new FixedString32Bytes(sourceFactionId),
                SourceMemberId = new FixedString64Bytes(sourceMemberId),
                TargetFactionId = new FixedString32Bytes(targetFactionId),
                TargetMemberId = new FixedString64Bytes(targetMemberId),
                Status = MarriageProposalStatus.Pending,
                ProposedAtInWorldDays = 0f,
                ExpiresAtInWorldDays = 90f,
            });
            return proposalEntity;
        }

        private static void MarkMemberStatus(
            EntityManager entityManager,
            Entity factionEntity,
            string memberId,
            DynastyMemberStatus status)
        {
            var memberEntity = FindMemberEntity(entityManager, factionEntity, new FixedString64Bytes(memberId));
            if (memberEntity == Entity.Null)
            {
                throw new InvalidOperationException("Dynasty member not found for status mutation: " + memberId);
            }

            var member = entityManager.GetComponentData<DynastyMemberComponent>(memberEntity);
            member.Status = status;
            entityManager.SetComponentData(memberEntity, member);
        }

        private static bool TryGetMarriagePair(
            EntityManager entityManager,
            FixedString64Bytes marriageId,
            out Entity primaryEntity,
            out MarriageComponent primary,
            out Entity mirrorEntity,
            out MarriageComponent mirror)
        {
            primaryEntity = Entity.Null;
            mirrorEntity = Entity.Null;
            primary = default;
            mirror = default;

            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<MarriageComponent>());
            if (query.IsEmpty)
            {
                query.Dispose();
                return false;
            }

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var marriages = query.ToComponentDataArray<MarriageComponent>(Allocator.Temp);
            query.Dispose();
            for (int i = 0; i < marriages.Length; i++)
            {
                if (!marriages[i].MarriageId.Equals(marriageId))
                {
                    continue;
                }

                if (marriages[i].IsPrimary)
                {
                    primaryEntity = entities[i];
                    primary = marriages[i];
                }
                else
                {
                    mirrorEntity = entities[i];
                    mirror = marriages[i];
                }
            }

            return primaryEntity != Entity.Null && mirrorEntity != Entity.Null;
        }

        private static int CountDissolvedMarriages(EntityManager entityManager)
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
                if (marriages[i].Dissolved)
                {
                    count++;
                }
            }

            return count;
        }

        private static Entity FindFactionEntity(
            EntityManager entityManager,
            FixedString32Bytes factionId)
        {
            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<FactionComponent>());
            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            query.Dispose();
            for (int i = 0; i < entities.Length; i++)
            {
                if (factions[i].FactionId.Equals(factionId))
                {
                    return entities[i];
                }
            }

            return Entity.Null;
        }

        private static Entity FindMemberEntity(
            EntityManager entityManager,
            Entity factionEntity,
            FixedString64Bytes memberId)
        {
            if (factionEntity == Entity.Null ||
                !entityManager.HasBuffer<DynastyMemberRef>(factionEntity))
            {
                return Entity.Null;
            }

            var members = entityManager.GetBuffer<DynastyMemberRef>(factionEntity);
            for (int i = 0; i < members.Length; i++)
            {
                var memberEntity = members[i].Member;
                if (memberEntity == Entity.Null ||
                    !entityManager.HasComponent<DynastyMemberComponent>(memberEntity))
                {
                    continue;
                }

                var member = entityManager.GetComponentData<DynastyMemberComponent>(memberEntity);
                if (member.MemberId.Equals(memberId))
                {
                    return memberEntity;
                }
            }

            return Entity.Null;
        }

        private static DualClockComponent GetDualClock(EntityManager entityManager)
        {
            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<DualClockComponent>());
            var dualClock = query.GetSingleton<DualClockComponent>();
            query.Dispose();
            return dualClock;
        }

        private static void SetInWorldDays(EntityManager entityManager, float inWorldDays)
        {
            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<DualClockComponent>());
            var clockEntity = query.GetSingletonEntity();
            var dualClock = entityManager.GetComponentData<DualClockComponent>(clockEntity);
            dualClock.InWorldDays = inWorldDays;
            entityManager.SetComponentData(clockEntity, dualClock);
            query.Dispose();
        }

        private static FixedString64Bytes BuildMarriageId(string proposalId)
        {
            var proposalKey = new FixedString64Bytes(proposalId);
            var marriageId = new FixedString64Bytes("marriage-");
            for (int i = 0; i < proposalKey.Length && marriageId.Length < 58; i++)
            {
                marriageId.Append(proposalKey[i]);
            }

            return marriageId;
        }

        private static bool Approximately(float actual, float expected)
        {
            return Mathf.Abs(actual - expected) <= 0.01f;
        }
    }
}
#endif
