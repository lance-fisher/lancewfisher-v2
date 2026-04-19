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
    /// Bundled smoke validator covering Bundle 2 of the AI mechanical
    /// campaign: sub-slice 21 holy war declaration execution plus
    /// sub-slice 22 divine right declaration execution. Both sub-slices
    /// consume CovertOpKind dispatches written by AICovertOpsSystem
    /// (sub-slice 6) and route through the sub-slice 18 dynasty
    /// operations foundation (DynastyOperationLimits.HasCapacity /
    /// BeginOperation), mirroring the sub-slice 20 missionary execution
    /// pattern.
    ///
    /// Sub-slice 21 holy war phases:
    ///   PhaseHolyWarDispatchSuccess: enemy with faith committed +
    ///     intensity 30 + influence 50 + faith operator on roster,
    ///     target=player has different faith committed; LastFiredOp =
    ///     HolyWar; tick world; verify dispatch cleared, one HolyWar
    ///     DynastyOperationComponent created with player target,
    ///     DynastyOperationHolyWarComponent attached with sane
    ///     per-kind fields (ResolveAt = current+18, WarExpiresAt =
    ///     current+180, IntensityCost=18, EscrowCost.Influence=24,
    ///     OperatorMemberId resolved, light-doctrine pulses 0.9/1.2),
    ///     resources/intensity deducted, narrative pushed (Warn since
    ///     target=player).
    ///   PhaseHolyWarHarmoniousFaithBlocks: enemy + player share the
    ///     same (faith, doctrine); dispatch fails (harmonious tier
    ///     blocked); resources untouched; dispatch cleared.
    ///   PhaseHolyWarTargetNoFaithBlocks: target has no committed
    ///     faith; dispatch fails; resources untouched; dispatch cleared.
    ///   PhaseHolyWarInsufficientIntensityBlocks: enemy intensity 12 <
    ///     18 threshold; dispatch fails; intensity / influence
    ///     untouched.
    ///   PhaseHolyWarInsufficientResourcesBlocks: enemy influence 20 <
    ///     24 cost; dispatch fails; intensity / influence untouched.
    ///   PhaseHolyWarDarkPathPulses: dark-doctrine source produces
    ///     intensityPulse 1.2 and loyaltyPulse 1.8 per browser
    ///     simulation.js:10468-10469.
    ///
    /// Sub-slice 22 divine right phases:
    ///   PhaseDivineRightDispatchSuccess: enemy with faith committed +
    ///     intensity 90 + level 5; LastFiredOp = DivineRight; tick
    ///     world; verify dispatch cleared, one DivineRight
    ///     DynastyOperationComponent created with default target,
    ///     DynastyOperationDivineRightComponent attached with sane
    ///     per-kind fields (ResolveAt = current+180, SourceFaithId
    ///     mapped, DoctrinePath captured, recognition placeholders
    ///     default 0, structure placeholders default empty), narrative
    ///     pushed (Warn).
    ///   PhaseDivineRightInsufficientIntensityBlocks: enemy intensity
    ///     50 < 80 threshold; dispatch fails; dispatch cleared.
    ///   PhaseDivineRightInsufficientLevelBlocks: enemy level 4 < 5;
    ///     dispatch fails; dispatch cleared.
    ///   PhaseDivineRightExistingDeclarationBlocks: enemy already has
    ///     an active DivineRight DynastyOperationComponent; dispatch
    ///     fails (existing declaration gate); no second op created.
    ///
    /// Browser reference:
    ///   simulation.js startHolyWarDeclaration (~10565-10602),
    ///     getHolyWarDeclarationTerms (~10424-10471), HOLY_WAR_COST
    ///     (~9767), HOLY_WAR_INTENSITY_COST (~9774),
    ///     HOLY_WAR_DECLARATION_DURATION_SECONDS (~9771),
    ///     HOLY_WAR_DURATION_SECONDS (~9775).
    ///   simulation.js startDivineRightDeclaration (~10784-10835),
    ///     getDivineRightDeclarationTerms (~10604-10653),
    ///     DIVINE_RIGHT_INTENSITY_THRESHOLD (~9782),
    ///     DIVINE_RIGHT_DECLARATION_DURATION_SECONDS (~9779).
    ///   ai.js holy war dispatch (~2512-2551), divine right dispatch
    ///     (~2553-2564).
    ///
    /// Artifact: artifacts/unity-holy-war-and-divine-right-execution-smoke.log.
    /// </summary>
    public static class BloodlinesHolyWarAndDivineRightExecutionSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-holy-war-and-divine-right-execution-smoke.log";

        [UnityEditor.MenuItem("Bloodlines/AI/Run Holy War And Divine Right Execution Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchHolyWarAndDivineRightExecutionSmokeValidation() =>
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
                message = "Holy war and divine right execution smoke errored: " + e;
            }

            string artifact = "BLOODLINES_HOLY_WAR_AND_DIVINE_RIGHT_EXECUTION_SMOKE " +
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
            // Sub-slice 21 phases
            ok &= RunPhaseHolyWarDispatchSuccess(sb);
            ok &= RunPhaseHolyWarHarmoniousFaithBlocks(sb);
            ok &= RunPhaseHolyWarTargetNoFaithBlocks(sb);
            ok &= RunPhaseHolyWarInsufficientIntensityBlocks(sb);
            ok &= RunPhaseHolyWarInsufficientResourcesBlocks(sb);
            ok &= RunPhaseHolyWarDarkPathPulses(sb);
            // Sub-slice 22 phases
            ok &= RunPhaseDivineRightDispatchSuccess(sb);
            ok &= RunPhaseDivineRightInsufficientIntensityBlocks(sb);
            ok &= RunPhaseDivineRightInsufficientLevelBlocks(sb);
            ok &= RunPhaseDivineRightExistingDeclarationBlocks(sb);
            report = sb.ToString();
            return ok;
        }

        // ------------------------------------------------------------------ shared helpers

        private static SimulationSystemGroup SetupHolyWarSystems(World world)
        {
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var sg = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AIHolyWarExecutionSystem>());
            sg.SortSystems();
            return sg;
        }

        private static SimulationSystemGroup SetupDivineRightSystems(World world)
        {
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var sg = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AIDivineRightExecutionSystem>());
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

        private static (Entity enemy, Entity player) SeedHolyWarWorld(
            EntityManager em,
            float enemyIntensity,
            float enemyInfluence,
            CovenantId enemyFaith,
            DoctrinePath enemyDoctrine,
            CovenantId playerFaith,
            DoctrinePath playerDoctrine,
            CovertOpKind dispatched,
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
                DoctrinePath  = enemyDoctrine,
                Intensity     = enemyIntensity,
                Level         = 1,
            });
            em.SetComponentData(enemyEntity, new ResourceStockpileComponent
            {
                Influence = enemyInfluence,
            });

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
                DoctrinePath  = playerDoctrine,
                Intensity     = 20f,
                Level         = 1,
            });

            return (enemyEntity, playerEntity);
        }

        private static Entity SeedDivineRightFaction(
            EntityManager em,
            float intensity,
            int level,
            CovenantId faith,
            DoctrinePath doctrine,
            CovertOpKind dispatched)
        {
            var enemyEntity = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(AICovertOpsComponent),
                typeof(FaithStateComponent));
            em.SetComponentData(enemyEntity, new FactionComponent { FactionId = new FixedString32Bytes("enemy") });
            em.SetComponentData(enemyEntity, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(enemyEntity, new AICovertOpsComponent
            {
                LastFiredOp     = dispatched,
                LastFiredOpTime = 0f,
            });
            em.SetComponentData(enemyEntity, new FaithStateComponent
            {
                SelectedFaith = faith,
                DoctrinePath  = doctrine,
                Intensity     = intensity,
                Level         = level,
            });
            return enemyEntity;
        }

        private static int CountActiveOpsForKindAndFaction(
            EntityManager em,
            DynastyOperationKind kind,
            FixedString32Bytes factionId)
        {
            int count = 0;
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<DynastyOperationComponent>());
            using (var arr = q.ToComponentDataArray<DynastyOperationComponent>(Allocator.Temp))
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    var op = arr[i];
                    if (!op.Active) continue;
                    if (op.OperationKind != kind) continue;
                    if (!op.SourceFactionId.Equals(factionId)) continue;
                    count++;
                }
            }
            q.Dispose();
            return count;
        }

        private static bool TryGetSingleActiveOpForKindAndFaction(
            EntityManager em,
            DynastyOperationKind kind,
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
                if (ops[i].OperationKind != kind) continue;
                if (!ops[i].SourceFactionId.Equals(factionId)) continue;
                opEntity = entities[i];
                op = ops[i];
                found++;
            }
            entities.Dispose();
            ops.Dispose();
            return found == 1;
        }

        // ------------------------------------------------------------------ sub-slice 21 phases

        private static bool RunPhaseHolyWarDispatchSuccess(System.Text.StringBuilder sb)
        {
            using var world = new World("holy-war-dispatch-success");
            var em = world.EntityManager;
            SetupHolyWarSystems(world);
            SeedDualClock(em, inWorldDays: 50f);

            var (enemyEntity, _) = SeedHolyWarWorld(em,
                enemyIntensity:  30f,
                enemyInfluence:  50f,
                enemyFaith:      CovenantId.OldLight,
                enemyDoctrine:   DoctrinePath.Light,
                playerFaith:     CovenantId.TheWild,
                playerDoctrine:  DoctrinePath.Light,
                dispatched:      CovertOpKind.HolyWar);

            int messagesBefore = NarrativeMessageBridge.Count(em);
            world.Update();

            var covert = em.GetComponentData<AICovertOpsComponent>(enemyEntity);
            if (covert.LastFiredOp != CovertOpKind.None)
            {
                sb.AppendLine($"PhaseHolyWarDispatchSuccess FAIL: dispatch should be cleared; got {covert.LastFiredOp}");
                return false;
            }
            if (!TryGetSingleActiveOpForKindAndFaction(em,
                    DynastyOperationKind.HolyWar,
                    new FixedString32Bytes("enemy"),
                    out var opEntity,
                    out var op))
            {
                int n = CountActiveOpsForKindAndFaction(em, DynastyOperationKind.HolyWar, new FixedString32Bytes("enemy"));
                sb.AppendLine($"PhaseHolyWarDispatchSuccess FAIL: expected exactly 1 active holy-war op, got {n}");
                return false;
            }
            if (!op.TargetFactionId.Equals(new FixedString32Bytes("player")))
            {
                sb.AppendLine($"PhaseHolyWarDispatchSuccess FAIL: TargetFactionId expected 'player', got '{op.TargetFactionId}'");
                return false;
            }
            if (!em.HasComponent<DynastyOperationHolyWarComponent>(opEntity))
            {
                sb.AppendLine("PhaseHolyWarDispatchSuccess FAIL: DynastyOperationHolyWarComponent not attached");
                return false;
            }
            var perKind = em.GetComponentData<DynastyOperationHolyWarComponent>(opEntity);
            float expectedResolveAt = 50f + AIHolyWarExecutionSystem.HolyWarDeclarationDurationInWorldDays;
            float expectedWarExpires = 50f + AIHolyWarExecutionSystem.HolyWarDurationInWorldDays;
            if (perKind.ResolveAtInWorldDays < expectedResolveAt - 0.01f ||
                perKind.ResolveAtInWorldDays > expectedResolveAt + 0.01f)
            {
                sb.AppendLine($"PhaseHolyWarDispatchSuccess FAIL: ResolveAtInWorldDays expected {expectedResolveAt}, got {perKind.ResolveAtInWorldDays}");
                return false;
            }
            if (perKind.WarExpiresAtInWorldDays < expectedWarExpires - 0.01f ||
                perKind.WarExpiresAtInWorldDays > expectedWarExpires + 0.01f)
            {
                sb.AppendLine($"PhaseHolyWarDispatchSuccess FAIL: WarExpiresAtInWorldDays expected {expectedWarExpires}, got {perKind.WarExpiresAtInWorldDays}");
                return false;
            }
            if (!perKind.OperatorMemberId.Equals(new FixedString64Bytes("enemy-bloodline-il")))
            {
                sb.AppendLine($"PhaseHolyWarDispatchSuccess FAIL: OperatorMemberId mismatch, got '{perKind.OperatorMemberId}'");
                return false;
            }
            if (perKind.IntensityCost < 17.99f || perKind.IntensityCost > 18.01f)
            {
                sb.AppendLine($"PhaseHolyWarDispatchSuccess FAIL: IntensityCost expected 18, got {perKind.IntensityCost}");
                return false;
            }
            if (perKind.EscrowCost.Influence < 23.99f || perKind.EscrowCost.Influence > 24.01f)
            {
                sb.AppendLine($"PhaseHolyWarDispatchSuccess FAIL: EscrowCost.Influence expected 24, got {perKind.EscrowCost.Influence}");
                return false;
            }
            // Light-doctrine pulses: intensityPulse=0.9, loyaltyPulse=1.2.
            if (perKind.IntensityPulse < 0.89f || perKind.IntensityPulse > 0.91f)
            {
                sb.AppendLine($"PhaseHolyWarDispatchSuccess FAIL: IntensityPulse expected 0.9 (light doctrine), got {perKind.IntensityPulse}");
                return false;
            }
            if (perKind.LoyaltyPulse < 1.19f || perKind.LoyaltyPulse > 1.21f)
            {
                sb.AppendLine($"PhaseHolyWarDispatchSuccess FAIL: LoyaltyPulse expected 1.2 (light doctrine), got {perKind.LoyaltyPulse}");
                return false;
            }

            // Resources + intensity deducted.
            var resources = em.GetComponentData<ResourceStockpileComponent>(enemyEntity);
            if (resources.Influence < 25.99f || resources.Influence > 26.01f)
            {
                sb.AppendLine($"PhaseHolyWarDispatchSuccess FAIL: Influence expected 26 (50 - 24), got {resources.Influence}");
                return false;
            }
            var faith = em.GetComponentData<FaithStateComponent>(enemyEntity);
            if (faith.Intensity < 11.99f || faith.Intensity > 12.01f)
            {
                sb.AppendLine($"PhaseHolyWarDispatchSuccess FAIL: Intensity expected 12 (30 - 18), got {faith.Intensity}");
                return false;
            }

            int messagesAfter = NarrativeMessageBridge.Count(em);
            if (messagesAfter != messagesBefore + 1)
            {
                sb.AppendLine($"PhaseHolyWarDispatchSuccess FAIL: narrative count expected +1, got {messagesAfter - messagesBefore}");
                return false;
            }
            sb.AppendLine($"PhaseHolyWarDispatchSuccess PASS: op created, per-kind attached, influence 50->26, intensity 30->12, light pulses 0.9/1.2, narrative +1, dispatch cleared");
            return true;
        }

        private static bool RunPhaseHolyWarHarmoniousFaithBlocks(System.Text.StringBuilder sb)
        {
            using var world = new World("holy-war-harmonious-faith-blocks");
            var em = world.EntityManager;
            SetupHolyWarSystems(world);
            SeedDualClock(em, inWorldDays: 50f);

            // Identical (faith, doctrine) -> harmonious tier blocks.
            var (enemyEntity, _) = SeedHolyWarWorld(em,
                enemyIntensity:  30f,
                enemyInfluence:  50f,
                enemyFaith:      CovenantId.OldLight,
                enemyDoctrine:   DoctrinePath.Light,
                playerFaith:     CovenantId.OldLight,
                playerDoctrine:  DoctrinePath.Light,
                dispatched:      CovertOpKind.HolyWar);

            world.Update();

            var covert = em.GetComponentData<AICovertOpsComponent>(enemyEntity);
            if (covert.LastFiredOp != CovertOpKind.None)
            {
                sb.AppendLine($"PhaseHolyWarHarmoniousFaithBlocks FAIL: dispatch should be cleared; got {covert.LastFiredOp}");
                return false;
            }
            int active = CountActiveOpsForKindAndFaction(em, DynastyOperationKind.HolyWar, new FixedString32Bytes("enemy"));
            if (active != 0)
            {
                sb.AppendLine($"PhaseHolyWarHarmoniousFaithBlocks FAIL: expected 0 holy-war ops, got {active}");
                return false;
            }
            var resources = em.GetComponentData<ResourceStockpileComponent>(enemyEntity);
            if (resources.Influence < 49.99f || resources.Influence > 50.01f)
            {
                sb.AppendLine($"PhaseHolyWarHarmoniousFaithBlocks FAIL: Influence must not change; got {resources.Influence}");
                return false;
            }
            sb.AppendLine("PhaseHolyWarHarmoniousFaithBlocks PASS: harmonious tier blocks dispatch, resources untouched");
            return true;
        }

        private static bool RunPhaseHolyWarTargetNoFaithBlocks(System.Text.StringBuilder sb)
        {
            using var world = new World("holy-war-target-no-faith-blocks");
            var em = world.EntityManager;
            SetupHolyWarSystems(world);
            SeedDualClock(em, inWorldDays: 50f);

            var (enemyEntity, _) = SeedHolyWarWorld(em,
                enemyIntensity:  30f,
                enemyInfluence:  50f,
                enemyFaith:      CovenantId.OldLight,
                enemyDoctrine:   DoctrinePath.Light,
                playerFaith:     CovenantId.None,
                playerDoctrine:  DoctrinePath.Unassigned,
                dispatched:      CovertOpKind.HolyWar);

            world.Update();

            int active = CountActiveOpsForKindAndFaction(em, DynastyOperationKind.HolyWar, new FixedString32Bytes("enemy"));
            if (active != 0)
            {
                sb.AppendLine($"PhaseHolyWarTargetNoFaithBlocks FAIL: expected 0 holy-war ops, got {active}");
                return false;
            }
            var resources = em.GetComponentData<ResourceStockpileComponent>(enemyEntity);
            if (resources.Influence < 49.99f || resources.Influence > 50.01f)
            {
                sb.AppendLine($"PhaseHolyWarTargetNoFaithBlocks FAIL: Influence must not change; got {resources.Influence}");
                return false;
            }
            sb.AppendLine("PhaseHolyWarTargetNoFaithBlocks PASS: target no-faith blocks dispatch, resources untouched");
            return true;
        }

        private static bool RunPhaseHolyWarInsufficientIntensityBlocks(System.Text.StringBuilder sb)
        {
            using var world = new World("holy-war-insufficient-intensity-blocks");
            var em = world.EntityManager;
            SetupHolyWarSystems(world);
            SeedDualClock(em, inWorldDays: 50f);

            var (enemyEntity, _) = SeedHolyWarWorld(em,
                enemyIntensity:  12f,  // < 18 threshold
                enemyInfluence:  50f,
                enemyFaith:      CovenantId.OldLight,
                enemyDoctrine:   DoctrinePath.Light,
                playerFaith:     CovenantId.TheWild,
                playerDoctrine:  DoctrinePath.Light,
                dispatched:      CovertOpKind.HolyWar);

            world.Update();

            int active = CountActiveOpsForKindAndFaction(em, DynastyOperationKind.HolyWar, new FixedString32Bytes("enemy"));
            if (active != 0)
            {
                sb.AppendLine($"PhaseHolyWarInsufficientIntensityBlocks FAIL: expected 0 holy-war ops, got {active}");
                return false;
            }
            var resources = em.GetComponentData<ResourceStockpileComponent>(enemyEntity);
            if (resources.Influence < 49.99f || resources.Influence > 50.01f)
            {
                sb.AppendLine($"PhaseHolyWarInsufficientIntensityBlocks FAIL: Influence must not change; got {resources.Influence}");
                return false;
            }
            var faith = em.GetComponentData<FaithStateComponent>(enemyEntity);
            if (faith.Intensity < 11.99f || faith.Intensity > 12.01f)
            {
                sb.AppendLine($"PhaseHolyWarInsufficientIntensityBlocks FAIL: Intensity must not change; got {faith.Intensity}");
                return false;
            }
            sb.AppendLine("PhaseHolyWarInsufficientIntensityBlocks PASS: low intensity blocks dispatch, resources/intensity untouched");
            return true;
        }

        private static bool RunPhaseHolyWarInsufficientResourcesBlocks(System.Text.StringBuilder sb)
        {
            using var world = new World("holy-war-insufficient-resources-blocks");
            var em = world.EntityManager;
            SetupHolyWarSystems(world);
            SeedDualClock(em, inWorldDays: 50f);

            var (enemyEntity, _) = SeedHolyWarWorld(em,
                enemyIntensity:  30f,
                enemyInfluence:  20f,  // < 24 cost
                enemyFaith:      CovenantId.OldLight,
                enemyDoctrine:   DoctrinePath.Light,
                playerFaith:     CovenantId.TheWild,
                playerDoctrine:  DoctrinePath.Light,
                dispatched:      CovertOpKind.HolyWar);

            world.Update();

            int active = CountActiveOpsForKindAndFaction(em, DynastyOperationKind.HolyWar, new FixedString32Bytes("enemy"));
            if (active != 0)
            {
                sb.AppendLine($"PhaseHolyWarInsufficientResourcesBlocks FAIL: expected 0 holy-war ops, got {active}");
                return false;
            }
            var resources = em.GetComponentData<ResourceStockpileComponent>(enemyEntity);
            if (resources.Influence < 19.99f || resources.Influence > 20.01f)
            {
                sb.AppendLine($"PhaseHolyWarInsufficientResourcesBlocks FAIL: Influence must not change; got {resources.Influence}");
                return false;
            }
            sb.AppendLine("PhaseHolyWarInsufficientResourcesBlocks PASS: low influence blocks dispatch, resources untouched");
            return true;
        }

        private static bool RunPhaseHolyWarDarkPathPulses(System.Text.StringBuilder sb)
        {
            using var world = new World("holy-war-dark-path-pulses");
            var em = world.EntityManager;
            SetupHolyWarSystems(world);
            SeedDualClock(em, inWorldDays: 50f);

            var (enemyEntity, _) = SeedHolyWarWorld(em,
                enemyIntensity:  30f,
                enemyInfluence:  50f,
                enemyFaith:      CovenantId.BloodDominion,
                enemyDoctrine:   DoctrinePath.Dark,
                playerFaith:     CovenantId.TheWild,
                playerDoctrine:  DoctrinePath.Light,
                dispatched:      CovertOpKind.HolyWar);

            world.Update();

            if (!TryGetSingleActiveOpForKindAndFaction(em,
                    DynastyOperationKind.HolyWar,
                    new FixedString32Bytes("enemy"),
                    out var opEntity,
                    out var _))
            {
                sb.AppendLine("PhaseHolyWarDarkPathPulses FAIL: expected exactly 1 active holy-war op");
                return false;
            }
            var perKind = em.GetComponentData<DynastyOperationHolyWarComponent>(opEntity);
            // Dark-doctrine pulses: intensityPulse=1.2, loyaltyPulse=1.8.
            if (perKind.IntensityPulse < 1.19f || perKind.IntensityPulse > 1.21f)
            {
                sb.AppendLine($"PhaseHolyWarDarkPathPulses FAIL: IntensityPulse expected 1.2 (dark doctrine), got {perKind.IntensityPulse}");
                return false;
            }
            if (perKind.LoyaltyPulse < 1.79f || perKind.LoyaltyPulse > 1.81f)
            {
                sb.AppendLine($"PhaseHolyWarDarkPathPulses FAIL: LoyaltyPulse expected 1.8 (dark doctrine), got {perKind.LoyaltyPulse}");
                return false;
            }
            sb.AppendLine("PhaseHolyWarDarkPathPulses PASS: dark doctrine produces intensityPulse 1.2 and loyaltyPulse 1.8");
            return true;
        }

        // ------------------------------------------------------------------ sub-slice 22 phases

        private static bool RunPhaseDivineRightDispatchSuccess(System.Text.StringBuilder sb)
        {
            using var world = new World("divine-right-dispatch-success");
            var em = world.EntityManager;
            SetupDivineRightSystems(world);
            SeedDualClock(em, inWorldDays: 100f);

            var enemyEntity = SeedDivineRightFaction(em,
                intensity:  90f,
                level:      5,
                faith:      CovenantId.TheOrder,
                doctrine:   DoctrinePath.Light,
                dispatched: CovertOpKind.DivineRight);

            int messagesBefore = NarrativeMessageBridge.Count(em);
            world.Update();

            var covert = em.GetComponentData<AICovertOpsComponent>(enemyEntity);
            if (covert.LastFiredOp != CovertOpKind.None)
            {
                sb.AppendLine($"PhaseDivineRightDispatchSuccess FAIL: dispatch should be cleared; got {covert.LastFiredOp}");
                return false;
            }
            if (!TryGetSingleActiveOpForKindAndFaction(em,
                    DynastyOperationKind.DivineRight,
                    new FixedString32Bytes("enemy"),
                    out var opEntity,
                    out var op))
            {
                int n = CountActiveOpsForKindAndFaction(em, DynastyOperationKind.DivineRight, new FixedString32Bytes("enemy"));
                sb.AppendLine($"PhaseDivineRightDispatchSuccess FAIL: expected exactly 1 active divine-right op, got {n}");
                return false;
            }
            // Divine right has no specific target faction; default expected.
            if (!op.TargetFactionId.Equals(default(FixedString32Bytes)))
            {
                sb.AppendLine($"PhaseDivineRightDispatchSuccess FAIL: TargetFactionId expected default, got '{op.TargetFactionId}'");
                return false;
            }
            if (!em.HasComponent<DynastyOperationDivineRightComponent>(opEntity))
            {
                sb.AppendLine("PhaseDivineRightDispatchSuccess FAIL: DynastyOperationDivineRightComponent not attached");
                return false;
            }
            var perKind = em.GetComponentData<DynastyOperationDivineRightComponent>(opEntity);
            float expectedResolveAt = 100f + AIDivineRightExecutionSystem.DivineRightDeclarationDurationInWorldDays;
            if (perKind.ResolveAtInWorldDays < expectedResolveAt - 0.01f ||
                perKind.ResolveAtInWorldDays > expectedResolveAt + 0.01f)
            {
                sb.AppendLine($"PhaseDivineRightDispatchSuccess FAIL: ResolveAtInWorldDays expected {expectedResolveAt}, got {perKind.ResolveAtInWorldDays}");
                return false;
            }
            if (!perKind.SourceFaithId.Equals(new FixedString64Bytes("the_order")))
            {
                sb.AppendLine($"PhaseDivineRightDispatchSuccess FAIL: SourceFaithId expected 'the_order', got '{perKind.SourceFaithId}'");
                return false;
            }
            if (perKind.DoctrinePath != DoctrinePath.Light)
            {
                sb.AppendLine($"PhaseDivineRightDispatchSuccess FAIL: DoctrinePath expected Light, got {perKind.DoctrinePath}");
                return false;
            }
            // Recognition / structure placeholders default.
            if (perKind.RecognitionShare != 0f || perKind.RecognitionSharePct != 0f)
            {
                sb.AppendLine($"PhaseDivineRightDispatchSuccess FAIL: recognition placeholders expected 0, got {perKind.RecognitionShare}/{perKind.RecognitionSharePct}");
                return false;
            }
            if (!perKind.ActiveApexStructureId.Equals(default(FixedString64Bytes)) ||
                !perKind.ActiveApexStructureName.Equals(default(FixedString64Bytes)))
            {
                sb.AppendLine("PhaseDivineRightDispatchSuccess FAIL: apex structure placeholders should be default");
                return false;
            }

            int messagesAfter = NarrativeMessageBridge.Count(em);
            if (messagesAfter != messagesBefore + 1)
            {
                sb.AppendLine($"PhaseDivineRightDispatchSuccess FAIL: narrative count expected +1, got {messagesAfter - messagesBefore}");
                return false;
            }
            sb.AppendLine($"PhaseDivineRightDispatchSuccess PASS: op created (default target), per-kind attached with ResolveAt=280, SourceFaithId=the_order, narrative +1, dispatch cleared");
            return true;
        }

        private static bool RunPhaseDivineRightInsufficientIntensityBlocks(System.Text.StringBuilder sb)
        {
            using var world = new World("divine-right-insufficient-intensity-blocks");
            var em = world.EntityManager;
            SetupDivineRightSystems(world);
            SeedDualClock(em, inWorldDays: 100f);

            var enemyEntity = SeedDivineRightFaction(em,
                intensity:  50f,  // < 80 threshold
                level:      5,
                faith:      CovenantId.TheOrder,
                doctrine:   DoctrinePath.Light,
                dispatched: CovertOpKind.DivineRight);

            world.Update();

            var covert = em.GetComponentData<AICovertOpsComponent>(enemyEntity);
            if (covert.LastFiredOp != CovertOpKind.None)
            {
                sb.AppendLine($"PhaseDivineRightInsufficientIntensityBlocks FAIL: dispatch should be cleared; got {covert.LastFiredOp}");
                return false;
            }
            int active = CountActiveOpsForKindAndFaction(em, DynastyOperationKind.DivineRight, new FixedString32Bytes("enemy"));
            if (active != 0)
            {
                sb.AppendLine($"PhaseDivineRightInsufficientIntensityBlocks FAIL: expected 0 divine-right ops, got {active}");
                return false;
            }
            sb.AppendLine("PhaseDivineRightInsufficientIntensityBlocks PASS: low intensity blocks dispatch, dispatch cleared");
            return true;
        }

        private static bool RunPhaseDivineRightInsufficientLevelBlocks(System.Text.StringBuilder sb)
        {
            using var world = new World("divine-right-insufficient-level-blocks");
            var em = world.EntityManager;
            SetupDivineRightSystems(world);
            SeedDualClock(em, inWorldDays: 100f);

            var enemyEntity = SeedDivineRightFaction(em,
                intensity:  90f,
                level:      4,  // < 5 threshold
                faith:      CovenantId.TheOrder,
                doctrine:   DoctrinePath.Light,
                dispatched: CovertOpKind.DivineRight);

            world.Update();

            int active = CountActiveOpsForKindAndFaction(em, DynastyOperationKind.DivineRight, new FixedString32Bytes("enemy"));
            if (active != 0)
            {
                sb.AppendLine($"PhaseDivineRightInsufficientLevelBlocks FAIL: expected 0 divine-right ops, got {active}");
                return false;
            }
            sb.AppendLine("PhaseDivineRightInsufficientLevelBlocks PASS: low level blocks dispatch");
            return true;
        }

        private static bool RunPhaseDivineRightExistingDeclarationBlocks(System.Text.StringBuilder sb)
        {
            using var world = new World("divine-right-existing-declaration-blocks");
            var em = world.EntityManager;
            SetupDivineRightSystems(world);
            SeedDualClock(em, inWorldDays: 100f);

            var enemyEntity = SeedDivineRightFaction(em,
                intensity:  90f,
                level:      5,
                faith:      CovenantId.TheOrder,
                doctrine:   DoctrinePath.Light,
                dispatched: CovertOpKind.DivineRight);

            // Pre-seed an active divine right operation for enemy.
            var existing = em.CreateEntity(typeof(DynastyOperationComponent));
            em.SetComponentData(existing, new DynastyOperationComponent
            {
                OperationId          = new FixedString64Bytes("preexisting-divineright"),
                SourceFactionId      = new FixedString32Bytes("enemy"),
                OperationKind        = DynastyOperationKind.DivineRight,
                StartedAtInWorldDays = 50f,
                TargetFactionId      = default,
                TargetMemberId       = default,
                Active               = true,
            });

            world.Update();

            // Existing-declaration gate: dispatch should clear, but no
            // second op should be created. Total count remains 1.
            int active = CountActiveOpsForKindAndFaction(em, DynastyOperationKind.DivineRight, new FixedString32Bytes("enemy"));
            if (active != 1)
            {
                sb.AppendLine($"PhaseDivineRightExistingDeclarationBlocks FAIL: expected 1 active divine-right op (preexisting only), got {active}");
                return false;
            }
            var covert = em.GetComponentData<AICovertOpsComponent>(enemyEntity);
            if (covert.LastFiredOp != CovertOpKind.None)
            {
                sb.AppendLine($"PhaseDivineRightExistingDeclarationBlocks FAIL: dispatch should be cleared; got {covert.LastFiredOp}");
                return false;
            }
            sb.AppendLine("PhaseDivineRightExistingDeclarationBlocks PASS: existing declaration blocks new dispatch, dispatch still cleared");
            return true;
        }
    }
}
#endif
