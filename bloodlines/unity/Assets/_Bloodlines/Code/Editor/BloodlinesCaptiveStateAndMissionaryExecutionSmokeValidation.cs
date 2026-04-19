#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Bundled smoke validator covering Bundle 1 of the AI mechanical
    /// campaign: sub-slice 19 captive member state plus sub-slice 20
    /// missionary execution as the first consumer of the sub-slice 18
    /// dynasty operations foundation.
    ///
    /// Sub-slice 19 phases:
    ///   PhaseCaptureMember: capture a single member; verify the buffer
    ///     gets one Held entry with correct fields and a DualClock
    ///     timestamp.
    ///   PhaseMultipleCaptivesPerFaction: capture 3 members from
    ///     different origin factions; verify buffer length and
    ///     per-member lookup.
    ///   PhaseReleaseCaptive: capture, then ReleaseCaptive(Released);
    ///     verify status flipped and entry retained on the buffer.
    ///
    /// Sub-slice 20 phases:
    ///   PhaseMissionaryDispatchSuccess: enemy with faith committed +
    ///     intensity 30 + influence 50, target=player has one
    ///     ControlPointComponent + faith committed (different covenant);
    ///     enemy AICovertOpsComponent.LastFiredOp = Missionary; tick
    ///     the world; verify dispatch cleared, one Missionary
    ///     DynastyOperationComponent created with player target,
    ///     DynastyOperationMissionaryComponent attached with sane
    ///     per-kind fields, resources/intensity deducted, narrative
    ///     pushed with Warn tone (target == player).
    ///   PhaseMissionaryCapBlocks: 6 active missionary operations
    ///     pre-seeded for enemy; dispatch fails (HasCapacity=false);
    ///     resources untouched; dispatch still cleared (one-shot).
    ///   PhaseMissionaryNoFaithBlocks: enemy has no committed faith;
    ///     dispatch fails; resources untouched; dispatch cleared.
    ///   PhaseMissionaryInsufficientIntensityBlocks: enemy has faith
    ///     but intensity below 12; dispatch fails; resources untouched.
    ///   PhaseMissionaryInsufficientResourcesBlocks: enemy has faith
    ///     and intensity but influence below 14; dispatch fails;
    ///     intensity and resources untouched.
    ///
    /// Browser reference:
    ///   simulation.js transferMemberToCaptor (~4422-4453),
    ///     findCaptiveRecordByMemberId (~4068),
    ///     captives splice path (~4216-4226).
    ///   simulation.js startMissionaryOperation (~10523-10563),
    ///     getMissionaryTerms (~10362-10422), MISSIONARY_COST (~9766),
    ///     MISSIONARY_INTENSITY_COST (~9773),
    ///     MISSIONARY_DURATION_SECONDS (~9770).
    ///   ai.js missionary dispatch hook (~2469-2496).
    ///
    /// Artifact: artifacts/unity-captive-state-and-missionary-execution-smoke.log.
    /// </summary>
    public static class BloodlinesCaptiveStateAndMissionaryExecutionSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-captive-state-and-missionary-execution-smoke.log";

        [UnityEditor.MenuItem("Bloodlines/AI/Run Captive State And Missionary Execution Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchCaptiveStateAndMissionaryExecutionSmokeValidation() =>
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
                message = "Captive state and missionary execution smoke errored: " + e;
            }

            string artifact = "BLOODLINES_CAPTIVE_STATE_AND_MISSIONARY_EXECUTION_SMOKE " +
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
            // Sub-slice 19 phases
            ok &= RunPhaseCaptureMember(sb);
            ok &= RunPhaseMultipleCaptivesPerFaction(sb);
            ok &= RunPhaseReleaseCaptive(sb);
            // Sub-slice 20 phases
            ok &= RunPhaseMissionaryDispatchSuccess(sb);
            ok &= RunPhaseMissionaryCapBlocks(sb);
            ok &= RunPhaseMissionaryNoFaithBlocks(sb);
            ok &= RunPhaseMissionaryInsufficientIntensityBlocks(sb);
            ok &= RunPhaseMissionaryInsufficientResourcesBlocks(sb);
            report = sb.ToString();
            return ok;
        }

        // ------------------------------------------------------------------ shared helpers

        private static SimulationSystemGroup SetupMissionarySystems(World world)
        {
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var sg = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AIMissionaryExecutionSystem>());
            sg.SortSystems();
            return sg;
        }

        private static void SeedDualClock(EntityManager em, float inWorldDays)
        {
            var e = em.CreateEntity(typeof(DualClockComponent));
            em.SetComponentData(e, new DualClockComponent
            {
                InWorldDays       = inWorldDays,
                DaysPerRealSecond = 2f,
                DeclarationCount  = 0,
            });
        }

        private static Entity CreateFaction(EntityManager em, string factionId)
        {
            var e = em.CreateEntity(typeof(FactionComponent));
            em.SetComponentData(e, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            return e;
        }

        // ------------------------------------------------------------------ sub-slice 19 phases

        private static bool RunPhaseCaptureMember(System.Text.StringBuilder sb)
        {
            using var world = new World("captive-capture-member");
            var em = world.EntityManager;
            SeedDualClock(em, inWorldDays: 25f);
            CreateFaction(em, "enemy");
            CreateFaction(em, "player");

            int idx = CapturedMemberHelpers.CaptureMember(
                em,
                captorFactionId: new FixedString32Bytes("enemy"),
                memberId:        new FixedString64Bytes("player-bloodline-1"),
                memberTitle:     new FixedString64Bytes("Heir Designate"),
                originFactionId: new FixedString32Bytes("player"));

            if (idx != 0)
            {
                sb.AppendLine($"PhaseCaptureMember FAIL: expected buffer index 0, got {idx}");
                return false;
            }

            if (!CapturedMemberHelpers.TryGetCaptive(
                    em,
                    new FixedString32Bytes("enemy"),
                    new FixedString64Bytes("player-bloodline-1"),
                    out var captive,
                    out int lookupIdx))
            {
                sb.AppendLine("PhaseCaptureMember FAIL: TryGetCaptive could not find captured member");
                return false;
            }
            if (lookupIdx != 0)
            {
                sb.AppendLine($"PhaseCaptureMember FAIL: lookup index expected 0, got {lookupIdx}");
                return false;
            }
            if (captive.Status != CapturedMemberStatus.Held)
            {
                sb.AppendLine($"PhaseCaptureMember FAIL: expected Status=Held, got {captive.Status}");
                return false;
            }
            if (!captive.MemberTitle.Equals(new FixedString64Bytes("Heir Designate")))
            {
                sb.AppendLine($"PhaseCaptureMember FAIL: MemberTitle mismatch, got '{captive.MemberTitle}'");
                return false;
            }
            if (!captive.OriginFactionId.Equals(new FixedString32Bytes("player")))
            {
                sb.AppendLine($"PhaseCaptureMember FAIL: OriginFactionId mismatch, got '{captive.OriginFactionId}'");
                return false;
            }
            if (captive.CapturedAtInWorldDays < 24.99f || captive.CapturedAtInWorldDays > 25.01f)
            {
                sb.AppendLine($"PhaseCaptureMember FAIL: expected CapturedAtInWorldDays=25, got {captive.CapturedAtInWorldDays}");
                return false;
            }
            sb.AppendLine("PhaseCaptureMember PASS: 1 Held entry with correct fields and DualClock timestamp");
            return true;
        }

        private static bool RunPhaseMultipleCaptivesPerFaction(System.Text.StringBuilder sb)
        {
            using var world = new World("captive-multiple-per-faction");
            var em = world.EntityManager;
            SeedDualClock(em, inWorldDays: 10f);
            var enemyEntity = CreateFaction(em, "enemy");
            CreateFaction(em, "player");
            CreateFaction(em, "tribes");
            CreateFaction(em, "minor-house-aris");

            CapturedMemberHelpers.CaptureMember(em,
                new FixedString32Bytes("enemy"),
                new FixedString64Bytes("player-bloodline-1"),
                new FixedString64Bytes("Heir Designate"),
                new FixedString32Bytes("player"));
            CapturedMemberHelpers.CaptureMember(em,
                new FixedString32Bytes("enemy"),
                new FixedString64Bytes("tribes-warleader-3"),
                new FixedString64Bytes("Tribal Warleader"),
                new FixedString32Bytes("tribes"));
            CapturedMemberHelpers.CaptureMember(em,
                new FixedString32Bytes("enemy"),
                new FixedString64Bytes("minor-aris-cadet-2"),
                new FixedString64Bytes("Cadet of Aris"),
                new FixedString32Bytes("minor-house-aris"));

            if (!em.HasBuffer<CapturedMemberElement>(enemyEntity))
            {
                sb.AppendLine("PhaseMultipleCaptivesPerFaction FAIL: enemy entity missing CapturedMemberElement buffer");
                return false;
            }
            var buffer = em.GetBuffer<CapturedMemberElement>(enemyEntity);
            if (buffer.Length != 3)
            {
                sb.AppendLine($"PhaseMultipleCaptivesPerFaction FAIL: expected 3 entries, got {buffer.Length}");
                return false;
            }

            // Lookup each by member id
            string[] memberIds = { "player-bloodline-1", "tribes-warleader-3", "minor-aris-cadet-2" };
            string[] expectedOrigins = { "player", "tribes", "minor-house-aris" };
            for (int i = 0; i < memberIds.Length; i++)
            {
                if (!CapturedMemberHelpers.TryGetCaptive(em,
                        new FixedString32Bytes("enemy"),
                        new FixedString64Bytes(memberIds[i]),
                        out var entry,
                        out int _))
                {
                    sb.AppendLine($"PhaseMultipleCaptivesPerFaction FAIL: lookup failed for {memberIds[i]}");
                    return false;
                }
                if (!entry.OriginFactionId.Equals(new FixedString32Bytes(expectedOrigins[i])))
                {
                    sb.AppendLine($"PhaseMultipleCaptivesPerFaction FAIL: origin mismatch for {memberIds[i]}, got '{entry.OriginFactionId}'");
                    return false;
                }
            }
            sb.AppendLine("PhaseMultipleCaptivesPerFaction PASS: 3 captives, each found by member id");
            return true;
        }

        private static bool RunPhaseReleaseCaptive(System.Text.StringBuilder sb)
        {
            using var world = new World("captive-release");
            var em = world.EntityManager;
            SeedDualClock(em, inWorldDays: 5f);
            var enemyEntity = CreateFaction(em, "enemy");
            CreateFaction(em, "player");

            CapturedMemberHelpers.CaptureMember(em,
                new FixedString32Bytes("enemy"),
                new FixedString64Bytes("player-bloodline-1"),
                new FixedString64Bytes("Heir Designate"),
                new FixedString32Bytes("player"));

            bool released = CapturedMemberHelpers.ReleaseCaptive(em,
                new FixedString32Bytes("enemy"),
                new FixedString64Bytes("player-bloodline-1"),
                CapturedMemberStatus.Released);
            if (!released)
            {
                sb.AppendLine("PhaseReleaseCaptive FAIL: ReleaseCaptive returned false");
                return false;
            }

            // Buffer length must remain 1 (audit retention).
            var buffer = em.GetBuffer<CapturedMemberElement>(enemyEntity);
            if (buffer.Length != 1)
            {
                sb.AppendLine($"PhaseReleaseCaptive FAIL: buffer length expected 1 (audit retained), got {buffer.Length}");
                return false;
            }
            if (buffer[0].Status != CapturedMemberStatus.Released)
            {
                sb.AppendLine($"PhaseReleaseCaptive FAIL: expected Status=Released, got {buffer[0].Status}");
                return false;
            }
            sb.AppendLine("PhaseReleaseCaptive PASS: status flipped to Released, entry retained on buffer");
            return true;
        }

        // ------------------------------------------------------------------ sub-slice 20 helpers

        private static (Entity enemy, Entity player) SeedMissionaryWorld(
            EntityManager em,
            float enemyIntensity,
            float enemyInfluence,
            CovenantId enemyFaith,
            CovenantId playerFaith,
            CovertOpKind dispatched,
            bool seedTargetTerritory = true,
            bool seedFaithOperator = true)
        {
            var enemyEntity = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(AICovertOpsComponent),
                typeof(FaithStateComponent),
                typeof(ResourceStockpileComponent));
            em.SetComponentData(enemyEntity, new FactionComponent { FactionId = new FixedString32Bytes("enemy") });
            em.SetComponentData(enemyEntity, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(enemyEntity, new AICovertOpsComponent
            {
                LastFiredOp     = dispatched,
                LastFiredOpTime = 0f,
            });
            em.SetComponentData(enemyEntity, new FaithStateComponent
            {
                SelectedFaith = enemyFaith,
                DoctrinePath  = DoctrinePath.Light,
                Intensity     = enemyIntensity,
                Level         = 1,
            });
            em.SetComponentData(enemyEntity, new ResourceStockpileComponent
            {
                Influence = enemyInfluence,
            });

            // Faith operator member (optional; seeded by default).
            if (seedFaithOperator)
            {
                var operatorEntity = em.CreateEntity(typeof(DynastyMemberComponent));
                em.SetComponentData(operatorEntity, new DynastyMemberComponent
                {
                    MemberId = new FixedString64Bytes("enemy-bloodline-il"),
                    Title    = new FixedString64Bytes("Ideological Leader"),
                    Role     = DynastyRole.IdeologicalLeader,
                    Path     = DynastyPath.ReligiousLeadership,
                    Status   = DynastyMemberStatus.Active,
                    Renown   = 30f,
                });
                var roster = em.AddBuffer<DynastyMemberRef>(enemyEntity);
                roster.Add(new DynastyMemberRef { Member = operatorEntity });
            }

            var playerEntity = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(FaithStateComponent));
            em.SetComponentData(playerEntity, new FactionComponent { FactionId = new FixedString32Bytes("player") });
            em.SetComponentData(playerEntity, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(playerEntity, new FaithStateComponent
            {
                SelectedFaith = playerFaith,
                DoctrinePath  = DoctrinePath.Light,
                Intensity     = 20f,
                Level         = 1,
            });

            // Target territory (one ControlPointComponent owned by player).
            if (seedTargetTerritory)
            {
                var cpEntity = em.CreateEntity(typeof(ControlPointComponent));
                em.SetComponentData(cpEntity, new ControlPointComponent
                {
                    ControlPointId  = new FixedString32Bytes("cp-keep-player"),
                    OwnerFactionId  = new FixedString32Bytes("player"),
                    CaptureFactionId = default,
                    ContinentId     = new FixedString32Bytes("c0"),
                    ControlState    = ControlState.Stabilized,
                    Loyalty         = 80f,
                });
            }

            return (enemyEntity, playerEntity);
        }

        private static int CountActiveMissionaryOpsForFaction(EntityManager em, FixedString32Bytes factionId)
        {
            int count = 0;
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<DynastyOperationComponent>());
            using (var arr = q.ToComponentDataArray<DynastyOperationComponent>(Allocator.Temp))
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    var op = arr[i];
                    if (!op.Active) continue;
                    if (op.OperationKind != DynastyOperationKind.Missionary) continue;
                    if (!op.SourceFactionId.Equals(factionId)) continue;
                    count++;
                }
            }
            q.Dispose();
            return count;
        }

        private static bool TryGetSingleMissionaryOpForFaction(
            EntityManager em,
            FixedString32Bytes factionId,
            out Entity opEntity,
            out DynastyOperationComponent op)
        {
            opEntity = Entity.Null;
            op = default;
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<DynastyOperationComponent>());
            var entities = q.ToEntityArray(Allocator.Temp);
            var ops = q.ToComponentDataArray<DynastyOperationComponent>(Allocator.Temp);
            q.Dispose();

            int found = 0;
            for (int i = 0; i < entities.Length; i++)
            {
                if (!ops[i].Active) continue;
                if (ops[i].OperationKind != DynastyOperationKind.Missionary) continue;
                if (!ops[i].SourceFactionId.Equals(factionId)) continue;
                opEntity = entities[i];
                op = ops[i];
                found++;
            }
            entities.Dispose();
            ops.Dispose();
            return found == 1;
        }

        // ------------------------------------------------------------------ sub-slice 20 phases

        private static bool RunPhaseMissionaryDispatchSuccess(System.Text.StringBuilder sb)
        {
            using var world = new World("missionary-dispatch-success");
            var em = world.EntityManager;
            SetupMissionarySystems(world);
            SeedDualClock(em, inWorldDays: 40f);

            var (enemyEntity, _) = SeedMissionaryWorld(em,
                enemyIntensity: 30f,
                enemyInfluence: 50f,
                enemyFaith:     CovenantId.OldLight,
                playerFaith:    CovenantId.TheWild,
                dispatched:     CovertOpKind.Missionary);

            int messagesBefore = NarrativeMessageBridge.Count(em);
            world.Update();

            // Dispatch must clear.
            var covert = em.GetComponentData<AICovertOpsComponent>(enemyEntity);
            if (covert.LastFiredOp != CovertOpKind.None)
            {
                sb.AppendLine($"PhaseMissionaryDispatchSuccess FAIL: dispatch should be cleared; got {covert.LastFiredOp}");
                return false;
            }

            // One Missionary operation entity for enemy must exist.
            if (!TryGetSingleMissionaryOpForFaction(em,
                    new FixedString32Bytes("enemy"),
                    out var opEntity,
                    out var op))
            {
                int n = CountActiveMissionaryOpsForFaction(em, new FixedString32Bytes("enemy"));
                sb.AppendLine($"PhaseMissionaryDispatchSuccess FAIL: expected exactly 1 active missionary op for enemy, got {n}");
                return false;
            }
            if (!op.TargetFactionId.Equals(new FixedString32Bytes("player")))
            {
                sb.AppendLine($"PhaseMissionaryDispatchSuccess FAIL: TargetFactionId expected 'player', got '{op.TargetFactionId}'");
                return false;
            }

            // Per-kind component must be attached.
            if (!em.HasComponent<DynastyOperationMissionaryComponent>(opEntity))
            {
                sb.AppendLine("PhaseMissionaryDispatchSuccess FAIL: DynastyOperationMissionaryComponent not attached");
                return false;
            }
            var perKind = em.GetComponentData<DynastyOperationMissionaryComponent>(opEntity);
            float expectedResolveAt = 40f + AIMissionaryExecutionSystem.MissionaryDurationInWorldDays;
            if (perKind.ResolveAtInWorldDays < expectedResolveAt - 0.01f ||
                perKind.ResolveAtInWorldDays > expectedResolveAt + 0.01f)
            {
                sb.AppendLine($"PhaseMissionaryDispatchSuccess FAIL: ResolveAtInWorldDays expected {expectedResolveAt}, got {perKind.ResolveAtInWorldDays}");
                return false;
            }
            if (!perKind.OperatorMemberId.Equals(new FixedString64Bytes("enemy-bloodline-il")))
            {
                sb.AppendLine($"PhaseMissionaryDispatchSuccess FAIL: OperatorMemberId mismatch, got '{perKind.OperatorMemberId}'");
                return false;
            }
            if (perKind.IntensityCost < 11.99f || perKind.IntensityCost > 12.01f)
            {
                sb.AppendLine($"PhaseMissionaryDispatchSuccess FAIL: IntensityCost expected 12, got {perKind.IntensityCost}");
                return false;
            }
            if (perKind.EscrowCost.Influence < 13.99f || perKind.EscrowCost.Influence > 14.01f)
            {
                sb.AppendLine($"PhaseMissionaryDispatchSuccess FAIL: EscrowCost.Influence expected 14, got {perKind.EscrowCost.Influence}");
                return false;
            }

            // Resources + intensity deducted.
            var resources = em.GetComponentData<ResourceStockpileComponent>(enemyEntity);
            if (resources.Influence < 35.99f || resources.Influence > 36.01f)
            {
                sb.AppendLine($"PhaseMissionaryDispatchSuccess FAIL: Influence expected 36 (50 - 14), got {resources.Influence}");
                return false;
            }
            var faith = em.GetComponentData<FaithStateComponent>(enemyEntity);
            if (faith.Intensity < 17.99f || faith.Intensity > 18.01f)
            {
                sb.AppendLine($"PhaseMissionaryDispatchSuccess FAIL: Intensity expected 18 (30 - 12), got {faith.Intensity}");
                return false;
            }

            // Narrative message pushed.
            int messagesAfter = NarrativeMessageBridge.Count(em);
            if (messagesAfter != messagesBefore + 1)
            {
                sb.AppendLine($"PhaseMissionaryDispatchSuccess FAIL: narrative count expected +1, got {messagesAfter - messagesBefore}");
                return false;
            }
            sb.AppendLine($"PhaseMissionaryDispatchSuccess PASS: op created, per-kind attached, influence 50->36, intensity 30->18, narrative +1, dispatch cleared");
            return true;
        }

        private static bool RunPhaseMissionaryCapBlocks(System.Text.StringBuilder sb)
        {
            using var world = new World("missionary-cap-blocks");
            var em = world.EntityManager;
            SetupMissionarySystems(world);
            SeedDualClock(em, inWorldDays: 40f);

            var (enemyEntity, _) = SeedMissionaryWorld(em,
                enemyIntensity: 30f,
                enemyInfluence: 50f,
                enemyFaith:     CovenantId.OldLight,
                playerFaith:    CovenantId.TheWild,
                dispatched:     CovertOpKind.Missionary);

            // Pre-seed cap-worth of active operations for enemy.
            for (int i = 0; i < DynastyOperationLimits.DYNASTY_OPERATION_ACTIVE_LIMIT; i++)
            {
                var existing = em.CreateEntity(typeof(DynastyOperationComponent));
                em.SetComponentData(existing, new DynastyOperationComponent
                {
                    OperationId          = new FixedString64Bytes($"existing-op-{i}"),
                    SourceFactionId      = new FixedString32Bytes("enemy"),
                    OperationKind        = DynastyOperationKind.Missionary,
                    StartedAtInWorldDays = 5f,
                    TargetFactionId      = default,
                    TargetMemberId       = default,
                    Active               = true,
                });
            }

            world.Update();

            // Dispatch cleared (one-shot).
            var covert = em.GetComponentData<AICovertOpsComponent>(enemyEntity);
            if (covert.LastFiredOp != CovertOpKind.None)
            {
                sb.AppendLine($"PhaseMissionaryCapBlocks FAIL: dispatch should be cleared; got {covert.LastFiredOp}");
                return false;
            }

            // No new op was created; count remains at cap.
            int active = DynastyOperationLimits.CountActiveForFaction(em, new FixedString32Bytes("enemy"));
            if (active != DynastyOperationLimits.DYNASTY_OPERATION_ACTIVE_LIMIT)
            {
                sb.AppendLine($"PhaseMissionaryCapBlocks FAIL: expected count={DynastyOperationLimits.DYNASTY_OPERATION_ACTIVE_LIMIT} (no new op), got {active}");
                return false;
            }

            // Resources untouched.
            var resources = em.GetComponentData<ResourceStockpileComponent>(enemyEntity);
            if (resources.Influence < 49.99f || resources.Influence > 50.01f)
            {
                sb.AppendLine($"PhaseMissionaryCapBlocks FAIL: Influence must not change; got {resources.Influence}");
                return false;
            }
            var faith = em.GetComponentData<FaithStateComponent>(enemyEntity);
            if (faith.Intensity < 29.99f || faith.Intensity > 30.01f)
            {
                sb.AppendLine($"PhaseMissionaryCapBlocks FAIL: Intensity must not change; got {faith.Intensity}");
                return false;
            }
            sb.AppendLine($"PhaseMissionaryCapBlocks PASS: cap blocks creation, resources/intensity untouched, dispatch cleared");
            return true;
        }

        private static bool RunPhaseMissionaryNoFaithBlocks(System.Text.StringBuilder sb)
        {
            using var world = new World("missionary-no-faith-blocks");
            var em = world.EntityManager;
            SetupMissionarySystems(world);
            SeedDualClock(em, inWorldDays: 40f);

            var (enemyEntity, _) = SeedMissionaryWorld(em,
                enemyIntensity: 30f,
                enemyInfluence: 50f,
                enemyFaith:     CovenantId.None,
                playerFaith:    CovenantId.TheWild,
                dispatched:     CovertOpKind.Missionary);

            world.Update();

            var covert = em.GetComponentData<AICovertOpsComponent>(enemyEntity);
            if (covert.LastFiredOp != CovertOpKind.None)
            {
                sb.AppendLine($"PhaseMissionaryNoFaithBlocks FAIL: dispatch should be cleared; got {covert.LastFiredOp}");
                return false;
            }
            int active = CountActiveMissionaryOpsForFaction(em, new FixedString32Bytes("enemy"));
            if (active != 0)
            {
                sb.AppendLine($"PhaseMissionaryNoFaithBlocks FAIL: expected 0 missionary ops, got {active}");
                return false;
            }
            var resources = em.GetComponentData<ResourceStockpileComponent>(enemyEntity);
            if (resources.Influence < 49.99f || resources.Influence > 50.01f)
            {
                sb.AppendLine($"PhaseMissionaryNoFaithBlocks FAIL: Influence must not change; got {resources.Influence}");
                return false;
            }
            sb.AppendLine("PhaseMissionaryNoFaithBlocks PASS: no faith blocks dispatch, resources untouched, dispatch cleared");
            return true;
        }

        private static bool RunPhaseMissionaryInsufficientIntensityBlocks(System.Text.StringBuilder sb)
        {
            using var world = new World("missionary-insufficient-intensity-blocks");
            var em = world.EntityManager;
            SetupMissionarySystems(world);
            SeedDualClock(em, inWorldDays: 40f);

            var (enemyEntity, _) = SeedMissionaryWorld(em,
                enemyIntensity: 8f,   // < 12 threshold
                enemyInfluence: 50f,
                enemyFaith:     CovenantId.OldLight,
                playerFaith:    CovenantId.TheWild,
                dispatched:     CovertOpKind.Missionary);

            world.Update();

            int active = CountActiveMissionaryOpsForFaction(em, new FixedString32Bytes("enemy"));
            if (active != 0)
            {
                sb.AppendLine($"PhaseMissionaryInsufficientIntensityBlocks FAIL: expected 0 missionary ops, got {active}");
                return false;
            }
            var resources = em.GetComponentData<ResourceStockpileComponent>(enemyEntity);
            if (resources.Influence < 49.99f || resources.Influence > 50.01f)
            {
                sb.AppendLine($"PhaseMissionaryInsufficientIntensityBlocks FAIL: Influence must not change; got {resources.Influence}");
                return false;
            }
            var faith = em.GetComponentData<FaithStateComponent>(enemyEntity);
            if (faith.Intensity < 7.99f || faith.Intensity > 8.01f)
            {
                sb.AppendLine($"PhaseMissionaryInsufficientIntensityBlocks FAIL: Intensity must not change; got {faith.Intensity}");
                return false;
            }
            sb.AppendLine("PhaseMissionaryInsufficientIntensityBlocks PASS: low intensity blocks dispatch, resources/intensity untouched");
            return true;
        }

        private static bool RunPhaseMissionaryInsufficientResourcesBlocks(System.Text.StringBuilder sb)
        {
            using var world = new World("missionary-insufficient-resources-blocks");
            var em = world.EntityManager;
            SetupMissionarySystems(world);
            SeedDualClock(em, inWorldDays: 40f);

            var (enemyEntity, _) = SeedMissionaryWorld(em,
                enemyIntensity: 30f,
                enemyInfluence: 10f,  // < 14 cost
                enemyFaith:     CovenantId.OldLight,
                playerFaith:    CovenantId.TheWild,
                dispatched:     CovertOpKind.Missionary);

            world.Update();

            int active = CountActiveMissionaryOpsForFaction(em, new FixedString32Bytes("enemy"));
            if (active != 0)
            {
                sb.AppendLine($"PhaseMissionaryInsufficientResourcesBlocks FAIL: expected 0 missionary ops, got {active}");
                return false;
            }
            var resources = em.GetComponentData<ResourceStockpileComponent>(enemyEntity);
            if (resources.Influence < 9.99f || resources.Influence > 10.01f)
            {
                sb.AppendLine($"PhaseMissionaryInsufficientResourcesBlocks FAIL: Influence must not change; got {resources.Influence}");
                return false;
            }
            var faith = em.GetComponentData<FaithStateComponent>(enemyEntity);
            if (faith.Intensity < 29.99f || faith.Intensity > 30.01f)
            {
                sb.AppendLine($"PhaseMissionaryInsufficientResourcesBlocks FAIL: Intensity must not change; got {faith.Intensity}");
                return false;
            }
            sb.AppendLine("PhaseMissionaryInsufficientResourcesBlocks PASS: low influence blocks dispatch, resources/intensity untouched");
            return true;
        }
    }
}
#endif
