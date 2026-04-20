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
    /// Bundled smoke validator covering Bundle 3 of the AI mechanical
    /// campaign: sub-slice 23 captive rescue execution plus sub-slice
    /// 24 captive ransom execution. Both are first production
    /// consumers of the sub-slice 19 CapturedMemberElement buffer plus
    /// sub-slice 18 dynasty operations foundation. Both follow the
    /// sub-slice 20/21/22 dispatch pattern (consume CovertOpKind,
    /// gate, attach per-kind component, push narrative, clear
    /// LastFiredOp unconditionally).
    ///
    /// Sub-slice 23 captive rescue phases:
    ///   PhaseRescueDispatchSuccess: enemy faction has a Held captive
    ///     belonging to it (recorded on the player faction's
    ///     CapturedMemberElement buffer) plus full Spymaster + Diplomat
    ///     roster, gold 100, influence 100; LastFiredOp = CaptiveRescue;
    ///     tick world; verify dispatch cleared, one CaptiveRescue
    ///     DynastyOperationComponent created with player target +
    ///     captive member id, DynastyOperationCaptiveRescueComponent
    ///     attached with sane per-kind fields (ResolveAt = current+20,
    ///     EscrowCost.Gold=42, EscrowCost.Influence=26, OperatorMemberIds
    ///     resolved, SuccessScore + ProjectedChance computed),
    ///     resources deducted, narrative pushed (Good since
    ///     source != player).
    ///   PhaseRescueNoCaptiveBlocks: enemy has no Held captives
    ///     anywhere; dispatch fails (captive picker returns null);
    ///     resources untouched; dispatch cleared.
    ///   PhaseRescueNoSpymasterBlocks: enemy roster has only a
    ///     Commander (no Spymaster/Diplomat/Merchant); dispatch fails;
    ///     resources untouched.
    ///   PhaseRescueInsufficientGoldBlocks: enemy has gold 30 (< 42);
    ///     dispatch fails; resources untouched.
    ///   PhaseRescueInsufficientInfluenceBlocks: enemy has influence
    ///     20 (< 26); dispatch fails; resources untouched.
    ///   PhaseRescueExistingOperationBlocks: enemy already has an
    ///     active dynasty operation targeting the same member id;
    ///     dispatch fails (existing-operation gate); only the
    ///     preexisting op remains.
    ///
    /// Sub-slice 24 captive ransom phases:
    ///   PhaseRansomDispatchSuccess: enemy has a Held captive plus
    ///     full Diplomat + Merchant roster, gold 100, influence 50;
    ///     LastFiredOp = CaptiveRansom; tick world; verify dispatch
    ///     cleared, one CaptiveRansom DynastyOperationComponent
    ///     created, DynastyOperationCaptiveRansomComponent attached
    ///     with ResolveAt = current+16, EscrowCost.Gold=70,
    ///     EscrowCost.Influence=18, ProjectedChance=1.0, narrative
    ///     pushed (Good).
    ///   PhaseRansomInsufficientGoldBlocks: enemy has gold 50 (< 70);
    ///     dispatch fails; resources untouched.
    ///   PhaseRansomNoMerchantBlocks: enemy roster has only a
    ///     Diplomat (no Merchant/Governor/HeadOfBloodline); dispatch
    ///     fails.
    ///
    /// Browser reference:
    ///   simulation.js startRescueOperation (~11067-11111),
    ///     getCapturedMemberRescueTerms (~4968-5028), RESCUE_BASE_*
    ///     constants (~29-34).
    ///   simulation.js startRansomNegotiation (~11026-11065),
    ///     getCapturedMemberRansomTerms (~4929-4966), RANSOM_BASE_*
    ///     constants (~27-32).
    ///   ai.js captive recovery dispatch hook (~2566-2607).
    ///
    /// Artifact: artifacts/unity-captive-rescue-and-ransom-execution-smoke.log.
    /// </summary>
    public static class BloodlinesCaptiveRescueAndRansomExecutionSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-captive-rescue-and-ransom-execution-smoke.log";

        [UnityEditor.MenuItem("Bloodlines/AI/Run Captive Rescue And Ransom Execution Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchCaptiveRescueAndRansomExecutionSmokeValidation() =>
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
                message = "Captive rescue and ransom execution smoke errored: " + e;
            }

            string artifact = "BLOODLINES_CAPTIVE_RESCUE_AND_RANSOM_EXECUTION_SMOKE " +
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
            // Sub-slice 23 phases
            ok &= RunPhaseRescueDispatchSuccess(sb);
            ok &= RunPhaseRescueNoCaptiveBlocks(sb);
            ok &= RunPhaseRescueNoSpymasterBlocks(sb);
            ok &= RunPhaseRescueInsufficientGoldBlocks(sb);
            ok &= RunPhaseRescueInsufficientInfluenceBlocks(sb);
            ok &= RunPhaseRescueExistingOperationBlocks(sb);
            // Sub-slice 24 phases
            ok &= RunPhaseRansomDispatchSuccess(sb);
            ok &= RunPhaseRansomInsufficientGoldBlocks(sb);
            ok &= RunPhaseRansomNoMerchantBlocks(sb);
            report = sb.ToString();
            return ok;
        }

        // ------------------------------------------------------------------ shared helpers

        private static SimulationSystemGroup SetupRescueSystems(World world)
        {
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var sg = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AICaptiveRescueExecutionSystem>());
            sg.SortSystems();
            return sg;
        }

        private static SimulationSystemGroup SetupRansomSystems(World world)
        {
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var sg = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AICaptiveRansomExecutionSystem>());
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

        private struct MemberSeed
        {
            public DynastyRole Role;
            public DynastyMemberStatus Status;
            public float Renown;
            public string IdSuffix;
        }

        // Seed: source faction (e.g. "enemy") with a roster, gold, and
        // influence, plus AICovertOpsComponent dispatched.
        private static Entity SeedSourceFaction(
            EntityManager em,
            string sourceFactionId,
            float gold,
            float influence,
            CovertOpKind dispatched,
            MemberSeed[] roster)
        {
            var entity = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(AICovertOpsComponent),
                typeof(ResourceStockpileComponent));
            em.SetComponentData(entity, new FactionComponent { FactionId = new FixedString32Bytes(sourceFactionId) });
            em.SetComponentData(entity, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(entity, new AICovertOpsComponent
            {
                LastFiredOp     = dispatched,
                LastFiredOpTime = 0f,
            });
            em.SetComponentData(entity, new ResourceStockpileComponent
            {
                Gold      = gold,
                Influence = influence,
            });

            var memberBuffer = em.AddBuffer<DynastyMemberRef>(entity);
            for (int i = 0; i < roster.Length; i++)
            {
                var memberEntity = em.CreateEntity(typeof(DynastyMemberComponent));
                em.SetComponentData(memberEntity, new DynastyMemberComponent
                {
                    MemberId = new FixedString64Bytes(sourceFactionId + "-" + roster[i].IdSuffix),
                    Title    = new FixedString64Bytes(roster[i].Role.ToString()),
                    Role     = roster[i].Role,
                    Path     = DynastyPath.Governance,
                    Status   = roster[i].Status,
                    Renown   = roster[i].Renown,
                });
                memberBuffer.Add(new DynastyMemberRef { Member = memberEntity });
            }
            return entity;
        }

        // Seed: captor faction with one Held captive belonging to source.
        private static Entity SeedCaptorWithHeldCaptive(
            EntityManager em,
            string captorFactionId,
            string captiveOriginFactionId,
            string captiveMemberId,
            string captiveMemberTitle)
        {
            var entity = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent));
            em.SetComponentData(entity, new FactionComponent { FactionId = new FixedString32Bytes(captorFactionId) });
            em.SetComponentData(entity, new FactionKindComponent { Kind = FactionKind.Kingdom });

            var buffer = em.AddBuffer<CapturedMemberElement>(entity);
            buffer.Add(new CapturedMemberElement
            {
                MemberId              = new FixedString64Bytes(captiveMemberId),
                MemberTitle           = new FixedString64Bytes(captiveMemberTitle),
                OriginFactionId       = new FixedString32Bytes(captiveOriginFactionId),
                CapturedAtInWorldDays = 5f,
                RansomCost            = 0f,
                Status                = CapturedMemberStatus.Held,
            });
            return entity;
        }

        private static Entity SeedCaptorBare(EntityManager em, string captorFactionId)
        {
            var entity = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent));
            em.SetComponentData(entity, new FactionComponent { FactionId = new FixedString32Bytes(captorFactionId) });
            em.SetComponentData(entity, new FactionKindComponent { Kind = FactionKind.Kingdom });
            return entity;
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

        // ------------------------------------------------------------------ sub-slice 23 phases

        private static MemberSeed[] FullRescueRoster() => new[]
        {
            new MemberSeed { Role = DynastyRole.HeadOfBloodline, Status = DynastyMemberStatus.Ruling, Renown = 30f, IdSuffix = "head" },
            new MemberSeed { Role = DynastyRole.Spymaster,       Status = DynastyMemberStatus.Active, Renown = 25f, IdSuffix = "spy"  },
            new MemberSeed { Role = DynastyRole.Diplomat,        Status = DynastyMemberStatus.Active, Renown = 20f, IdSuffix = "dip"  },
        };

        private static bool RunPhaseRescueDispatchSuccess(System.Text.StringBuilder sb)
        {
            using var world = new World("rescue-dispatch-success");
            var em = world.EntityManager;
            SetupRescueSystems(world);
            SeedDualClock(em, inWorldDays: 60f);

            var enemyEntity = SeedSourceFaction(em, "enemy",
                gold:        100f,
                influence:   100f,
                dispatched:  CovertOpKind.CaptiveRescue,
                roster:      FullRescueRoster());
            SeedCaptorWithHeldCaptive(em, "player",
                captiveOriginFactionId: "enemy",
                captiveMemberId:        "enemy-bloodline-heir",
                captiveMemberTitle:     "Heir Designate");

            int messagesBefore = NarrativeMessageBridge.Count(em);
            world.Update();

            var covert = em.GetComponentData<AICovertOpsComponent>(enemyEntity);
            if (covert.LastFiredOp != CovertOpKind.None)
            {
                sb.AppendLine($"PhaseRescueDispatchSuccess FAIL: dispatch should be cleared; got {covert.LastFiredOp}");
                return false;
            }
            if (!TryGetSingleActiveOpForKindAndFaction(em,
                    DynastyOperationKind.CaptiveRescue,
                    new FixedString32Bytes("enemy"),
                    out var opEntity,
                    out var op))
            {
                int n = CountActiveOpsForKindAndFaction(em, DynastyOperationKind.CaptiveRescue, new FixedString32Bytes("enemy"));
                sb.AppendLine($"PhaseRescueDispatchSuccess FAIL: expected exactly 1 active rescue op, got {n}");
                return false;
            }
            if (!op.TargetFactionId.Equals(new FixedString32Bytes("player")))
            {
                sb.AppendLine($"PhaseRescueDispatchSuccess FAIL: TargetFactionId expected 'player', got '{op.TargetFactionId}'");
                return false;
            }
            if (!op.TargetMemberId.Equals(new FixedString64Bytes("enemy-bloodline-heir")))
            {
                sb.AppendLine($"PhaseRescueDispatchSuccess FAIL: TargetMemberId expected 'enemy-bloodline-heir', got '{op.TargetMemberId}'");
                return false;
            }
            if (!em.HasComponent<DynastyOperationCaptiveRescueComponent>(opEntity))
            {
                sb.AppendLine("PhaseRescueDispatchSuccess FAIL: DynastyOperationCaptiveRescueComponent not attached");
                return false;
            }
            var perKind = em.GetComponentData<DynastyOperationCaptiveRescueComponent>(opEntity);
            float expectedResolveAt = 60f + AICaptiveRescueExecutionSystem.RescueBaseDurationInWorldDays;
            if (perKind.ResolveAtInWorldDays < expectedResolveAt - 0.01f ||
                perKind.ResolveAtInWorldDays > expectedResolveAt + 0.01f)
            {
                sb.AppendLine($"PhaseRescueDispatchSuccess FAIL: ResolveAtInWorldDays expected {expectedResolveAt}, got {perKind.ResolveAtInWorldDays}");
                return false;
            }
            if (perKind.EscrowCost.Gold < 41.99f || perKind.EscrowCost.Gold > 42.01f)
            {
                sb.AppendLine($"PhaseRescueDispatchSuccess FAIL: EscrowCost.Gold expected 42, got {perKind.EscrowCost.Gold}");
                return false;
            }
            if (perKind.EscrowCost.Influence < 25.99f || perKind.EscrowCost.Influence > 26.01f)
            {
                sb.AppendLine($"PhaseRescueDispatchSuccess FAIL: EscrowCost.Influence expected 26, got {perKind.EscrowCost.Influence}");
                return false;
            }
            if (!perKind.SpymasterMemberId.Equals(new FixedString64Bytes("enemy-spy")))
            {
                sb.AppendLine($"PhaseRescueDispatchSuccess FAIL: SpymasterMemberId expected 'enemy-spy', got '{perKind.SpymasterMemberId}'");
                return false;
            }
            if (!perKind.DiplomatMemberId.Equals(new FixedString64Bytes("enemy-dip")))
            {
                sb.AppendLine($"PhaseRescueDispatchSuccess FAIL: DiplomatMemberId expected 'enemy-dip', got '{perKind.DiplomatMemberId}'");
                return false;
            }
            if (!perKind.CaptorFactionId.Equals(new FixedString32Bytes("player")))
            {
                sb.AppendLine($"PhaseRescueDispatchSuccess FAIL: CaptorFactionId expected 'player', got '{perKind.CaptorFactionId}'");
                return false;
            }

            // Resources deducted.
            var resources = em.GetComponentData<ResourceStockpileComponent>(enemyEntity);
            if (resources.Gold < 57.99f || resources.Gold > 58.01f)
            {
                sb.AppendLine($"PhaseRescueDispatchSuccess FAIL: Gold expected 58 (100 - 42), got {resources.Gold}");
                return false;
            }
            if (resources.Influence < 73.99f || resources.Influence > 74.01f)
            {
                sb.AppendLine($"PhaseRescueDispatchSuccess FAIL: Influence expected 74 (100 - 26), got {resources.Influence}");
                return false;
            }

            int messagesAfter = NarrativeMessageBridge.Count(em);
            if (messagesAfter != messagesBefore + 1)
            {
                sb.AppendLine($"PhaseRescueDispatchSuccess FAIL: narrative count expected +1, got {messagesAfter - messagesBefore}");
                return false;
            }
            sb.AppendLine($"PhaseRescueDispatchSuccess PASS: op created (target=player, member=enemy-bloodline-heir), per-kind attached with ResolveAt=80, gold 100->58, influence 100->74, narrative +1, dispatch cleared");
            return true;
        }

        private static bool RunPhaseRescueNoCaptiveBlocks(System.Text.StringBuilder sb)
        {
            using var world = new World("rescue-no-captive-blocks");
            var em = world.EntityManager;
            SetupRescueSystems(world);
            SeedDualClock(em, inWorldDays: 60f);

            var enemyEntity = SeedSourceFaction(em, "enemy",
                gold:        100f,
                influence:   100f,
                dispatched:  CovertOpKind.CaptiveRescue,
                roster:      FullRescueRoster());
            // No captor with a Held captive belonging to enemy.
            SeedCaptorBare(em, "player");

            world.Update();

            var covert = em.GetComponentData<AICovertOpsComponent>(enemyEntity);
            if (covert.LastFiredOp != CovertOpKind.None)
            {
                sb.AppendLine($"PhaseRescueNoCaptiveBlocks FAIL: dispatch should be cleared; got {covert.LastFiredOp}");
                return false;
            }
            int active = CountActiveOpsForKindAndFaction(em, DynastyOperationKind.CaptiveRescue, new FixedString32Bytes("enemy"));
            if (active != 0)
            {
                sb.AppendLine($"PhaseRescueNoCaptiveBlocks FAIL: expected 0 rescue ops, got {active}");
                return false;
            }
            var resources = em.GetComponentData<ResourceStockpileComponent>(enemyEntity);
            if (resources.Gold < 99.99f || resources.Gold > 100.01f)
            {
                sb.AppendLine($"PhaseRescueNoCaptiveBlocks FAIL: Gold must not change; got {resources.Gold}");
                return false;
            }
            sb.AppendLine("PhaseRescueNoCaptiveBlocks PASS: no captive blocks dispatch, resources untouched, dispatch cleared");
            return true;
        }

        private static bool RunPhaseRescueNoSpymasterBlocks(System.Text.StringBuilder sb)
        {
            using var world = new World("rescue-no-spymaster-blocks");
            var em = world.EntityManager;
            SetupRescueSystems(world);
            SeedDualClock(em, inWorldDays: 60f);

            var enemyEntity = SeedSourceFaction(em, "enemy",
                gold:        100f,
                influence:   100f,
                dispatched:  CovertOpKind.CaptiveRescue,
                roster:      new[]
                {
                    new MemberSeed { Role = DynastyRole.Commander, Status = DynastyMemberStatus.Active, Renown = 30f, IdSuffix = "cmd" },
                });
            SeedCaptorWithHeldCaptive(em, "player",
                captiveOriginFactionId: "enemy",
                captiveMemberId:        "enemy-bloodline-heir",
                captiveMemberTitle:     "Heir Designate");

            world.Update();

            int active = CountActiveOpsForKindAndFaction(em, DynastyOperationKind.CaptiveRescue, new FixedString32Bytes("enemy"));
            if (active != 0)
            {
                sb.AppendLine($"PhaseRescueNoSpymasterBlocks FAIL: expected 0 rescue ops, got {active}");
                return false;
            }
            var resources = em.GetComponentData<ResourceStockpileComponent>(enemyEntity);
            if (resources.Gold < 99.99f || resources.Gold > 100.01f)
            {
                sb.AppendLine($"PhaseRescueNoSpymasterBlocks FAIL: Gold must not change; got {resources.Gold}");
                return false;
            }
            sb.AppendLine("PhaseRescueNoSpymasterBlocks PASS: no spymaster/diplomat/merchant blocks dispatch, resources untouched");
            return true;
        }

        private static bool RunPhaseRescueInsufficientGoldBlocks(System.Text.StringBuilder sb)
        {
            using var world = new World("rescue-insufficient-gold-blocks");
            var em = world.EntityManager;
            SetupRescueSystems(world);
            SeedDualClock(em, inWorldDays: 60f);

            var enemyEntity = SeedSourceFaction(em, "enemy",
                gold:        30f,  // < 42
                influence:   100f,
                dispatched:  CovertOpKind.CaptiveRescue,
                roster:      FullRescueRoster());
            SeedCaptorWithHeldCaptive(em, "player",
                captiveOriginFactionId: "enemy",
                captiveMemberId:        "enemy-bloodline-heir",
                captiveMemberTitle:     "Heir Designate");

            world.Update();

            int active = CountActiveOpsForKindAndFaction(em, DynastyOperationKind.CaptiveRescue, new FixedString32Bytes("enemy"));
            if (active != 0)
            {
                sb.AppendLine($"PhaseRescueInsufficientGoldBlocks FAIL: expected 0 rescue ops, got {active}");
                return false;
            }
            var resources = em.GetComponentData<ResourceStockpileComponent>(enemyEntity);
            if (resources.Gold < 29.99f || resources.Gold > 30.01f)
            {
                sb.AppendLine($"PhaseRescueInsufficientGoldBlocks FAIL: Gold must not change; got {resources.Gold}");
                return false;
            }
            sb.AppendLine("PhaseRescueInsufficientGoldBlocks PASS: low gold blocks dispatch, resources untouched");
            return true;
        }

        private static bool RunPhaseRescueInsufficientInfluenceBlocks(System.Text.StringBuilder sb)
        {
            using var world = new World("rescue-insufficient-influence-blocks");
            var em = world.EntityManager;
            SetupRescueSystems(world);
            SeedDualClock(em, inWorldDays: 60f);

            var enemyEntity = SeedSourceFaction(em, "enemy",
                gold:        100f,
                influence:   20f,  // < 26
                dispatched:  CovertOpKind.CaptiveRescue,
                roster:      FullRescueRoster());
            SeedCaptorWithHeldCaptive(em, "player",
                captiveOriginFactionId: "enemy",
                captiveMemberId:        "enemy-bloodline-heir",
                captiveMemberTitle:     "Heir Designate");

            world.Update();

            int active = CountActiveOpsForKindAndFaction(em, DynastyOperationKind.CaptiveRescue, new FixedString32Bytes("enemy"));
            if (active != 0)
            {
                sb.AppendLine($"PhaseRescueInsufficientInfluenceBlocks FAIL: expected 0 rescue ops, got {active}");
                return false;
            }
            var resources = em.GetComponentData<ResourceStockpileComponent>(enemyEntity);
            if (resources.Gold < 99.99f || resources.Gold > 100.01f)
            {
                sb.AppendLine($"PhaseRescueInsufficientInfluenceBlocks FAIL: Gold must not change; got {resources.Gold}");
                return false;
            }
            if (resources.Influence < 19.99f || resources.Influence > 20.01f)
            {
                sb.AppendLine($"PhaseRescueInsufficientInfluenceBlocks FAIL: Influence must not change; got {resources.Influence}");
                return false;
            }
            sb.AppendLine("PhaseRescueInsufficientInfluenceBlocks PASS: low influence blocks dispatch, resources untouched");
            return true;
        }

        private static bool RunPhaseRescueExistingOperationBlocks(System.Text.StringBuilder sb)
        {
            using var world = new World("rescue-existing-operation-blocks");
            var em = world.EntityManager;
            SetupRescueSystems(world);
            SeedDualClock(em, inWorldDays: 60f);

            var enemyEntity = SeedSourceFaction(em, "enemy",
                gold:        100f,
                influence:   100f,
                dispatched:  CovertOpKind.CaptiveRescue,
                roster:      FullRescueRoster());
            SeedCaptorWithHeldCaptive(em, "player",
                captiveOriginFactionId: "enemy",
                captiveMemberId:        "enemy-bloodline-heir",
                captiveMemberTitle:     "Heir Designate");

            // Pre-seed an active dynasty operation for this member.
            var existing = em.CreateEntity(typeof(DynastyOperationComponent));
            em.SetComponentData(existing, new DynastyOperationComponent
            {
                OperationId          = new FixedString64Bytes("preexisting-rescue"),
                SourceFactionId      = new FixedString32Bytes("enemy"),
                OperationKind        = DynastyOperationKind.CaptiveRansom,
                StartedAtInWorldDays = 50f,
                TargetFactionId      = new FixedString32Bytes("player"),
                TargetMemberId       = new FixedString64Bytes("enemy-bloodline-heir"),
                Active               = true,
            });

            world.Update();

            // Existing-operation gate: the rescue dispatch should not
            // create a second op. Total rescue ops still 0; total ops
            // for member still 1 (the preexisting ransom).
            int rescueCount = CountActiveOpsForKindAndFaction(em, DynastyOperationKind.CaptiveRescue, new FixedString32Bytes("enemy"));
            if (rescueCount != 0)
            {
                sb.AppendLine($"PhaseRescueExistingOperationBlocks FAIL: expected 0 rescue ops (existing op blocks), got {rescueCount}");
                return false;
            }
            int ransomCount = CountActiveOpsForKindAndFaction(em, DynastyOperationKind.CaptiveRansom, new FixedString32Bytes("enemy"));
            if (ransomCount != 1)
            {
                sb.AppendLine($"PhaseRescueExistingOperationBlocks FAIL: expected 1 active ransom op (preexisting), got {ransomCount}");
                return false;
            }
            var resources = em.GetComponentData<ResourceStockpileComponent>(enemyEntity);
            if (resources.Gold < 99.99f || resources.Gold > 100.01f)
            {
                sb.AppendLine($"PhaseRescueExistingOperationBlocks FAIL: Gold must not change; got {resources.Gold}");
                return false;
            }
            sb.AppendLine("PhaseRescueExistingOperationBlocks PASS: existing operation for member blocks rescue, resources untouched");
            return true;
        }

        // ------------------------------------------------------------------ sub-slice 24 phases

        private static MemberSeed[] FullRansomRoster() => new[]
        {
            new MemberSeed { Role = DynastyRole.HeadOfBloodline, Status = DynastyMemberStatus.Ruling, Renown = 30f, IdSuffix = "head" },
            new MemberSeed { Role = DynastyRole.Diplomat,        Status = DynastyMemberStatus.Active, Renown = 25f, IdSuffix = "dip" },
            new MemberSeed { Role = DynastyRole.Merchant,        Status = DynastyMemberStatus.Active, Renown = 20f, IdSuffix = "mer" },
        };

        private static bool RunPhaseRansomDispatchSuccess(System.Text.StringBuilder sb)
        {
            using var world = new World("ransom-dispatch-success");
            var em = world.EntityManager;
            SetupRansomSystems(world);
            SeedDualClock(em, inWorldDays: 80f);

            var enemyEntity = SeedSourceFaction(em, "enemy",
                gold:        100f,
                influence:   50f,
                dispatched:  CovertOpKind.CaptiveRansom,
                roster:      FullRansomRoster());
            SeedCaptorWithHeldCaptive(em, "player",
                captiveOriginFactionId: "enemy",
                captiveMemberId:        "enemy-bloodline-heir",
                captiveMemberTitle:     "Heir Designate");

            int messagesBefore = NarrativeMessageBridge.Count(em);
            world.Update();

            var covert = em.GetComponentData<AICovertOpsComponent>(enemyEntity);
            if (covert.LastFiredOp != CovertOpKind.None)
            {
                sb.AppendLine($"PhaseRansomDispatchSuccess FAIL: dispatch should be cleared; got {covert.LastFiredOp}");
                return false;
            }
            if (!TryGetSingleActiveOpForKindAndFaction(em,
                    DynastyOperationKind.CaptiveRansom,
                    new FixedString32Bytes("enemy"),
                    out var opEntity,
                    out var op))
            {
                int n = CountActiveOpsForKindAndFaction(em, DynastyOperationKind.CaptiveRansom, new FixedString32Bytes("enemy"));
                sb.AppendLine($"PhaseRansomDispatchSuccess FAIL: expected exactly 1 active ransom op, got {n}");
                return false;
            }
            if (!op.TargetFactionId.Equals(new FixedString32Bytes("player")) ||
                !op.TargetMemberId.Equals(new FixedString64Bytes("enemy-bloodline-heir")))
            {
                sb.AppendLine($"PhaseRansomDispatchSuccess FAIL: target/member mismatch; got '{op.TargetFactionId}'/'{op.TargetMemberId}'");
                return false;
            }
            if (!em.HasComponent<DynastyOperationCaptiveRansomComponent>(opEntity))
            {
                sb.AppendLine("PhaseRansomDispatchSuccess FAIL: DynastyOperationCaptiveRansomComponent not attached");
                return false;
            }
            var perKind = em.GetComponentData<DynastyOperationCaptiveRansomComponent>(opEntity);
            float expectedResolveAt = 80f + AICaptiveRansomExecutionSystem.RansomBaseDurationInWorldDays;
            if (perKind.ResolveAtInWorldDays < expectedResolveAt - 0.01f ||
                perKind.ResolveAtInWorldDays > expectedResolveAt + 0.01f)
            {
                sb.AppendLine($"PhaseRansomDispatchSuccess FAIL: ResolveAtInWorldDays expected {expectedResolveAt}, got {perKind.ResolveAtInWorldDays}");
                return false;
            }
            if (perKind.EscrowCost.Gold < 69.99f || perKind.EscrowCost.Gold > 70.01f)
            {
                sb.AppendLine($"PhaseRansomDispatchSuccess FAIL: EscrowCost.Gold expected 70, got {perKind.EscrowCost.Gold}");
                return false;
            }
            if (perKind.EscrowCost.Influence < 17.99f || perKind.EscrowCost.Influence > 18.01f)
            {
                sb.AppendLine($"PhaseRansomDispatchSuccess FAIL: EscrowCost.Influence expected 18, got {perKind.EscrowCost.Influence}");
                return false;
            }
            if (perKind.ProjectedChance < 0.99f || perKind.ProjectedChance > 1.01f)
            {
                sb.AppendLine($"PhaseRansomDispatchSuccess FAIL: ProjectedChance expected 1.0, got {perKind.ProjectedChance}");
                return false;
            }
            if (!perKind.DiplomatMemberId.Equals(new FixedString64Bytes("enemy-dip")))
            {
                sb.AppendLine($"PhaseRansomDispatchSuccess FAIL: DiplomatMemberId expected 'enemy-dip', got '{perKind.DiplomatMemberId}'");
                return false;
            }
            if (!perKind.MerchantMemberId.Equals(new FixedString64Bytes("enemy-mer")))
            {
                sb.AppendLine($"PhaseRansomDispatchSuccess FAIL: MerchantMemberId expected 'enemy-mer', got '{perKind.MerchantMemberId}'");
                return false;
            }

            var resources = em.GetComponentData<ResourceStockpileComponent>(enemyEntity);
            if (resources.Gold < 29.99f || resources.Gold > 30.01f)
            {
                sb.AppendLine($"PhaseRansomDispatchSuccess FAIL: Gold expected 30 (100 - 70), got {resources.Gold}");
                return false;
            }
            if (resources.Influence < 31.99f || resources.Influence > 32.01f)
            {
                sb.AppendLine($"PhaseRansomDispatchSuccess FAIL: Influence expected 32 (50 - 18), got {resources.Influence}");
                return false;
            }
            int messagesAfter = NarrativeMessageBridge.Count(em);
            if (messagesAfter != messagesBefore + 1)
            {
                sb.AppendLine($"PhaseRansomDispatchSuccess FAIL: narrative count expected +1, got {messagesAfter - messagesBefore}");
                return false;
            }
            sb.AppendLine($"PhaseRansomDispatchSuccess PASS: op created, per-kind attached with ResolveAt=96, gold 100->30, influence 50->32, ProjectedChance=1.0, narrative +1, dispatch cleared");
            return true;
        }

        private static bool RunPhaseRansomInsufficientGoldBlocks(System.Text.StringBuilder sb)
        {
            using var world = new World("ransom-insufficient-gold-blocks");
            var em = world.EntityManager;
            SetupRansomSystems(world);
            SeedDualClock(em, inWorldDays: 80f);

            var enemyEntity = SeedSourceFaction(em, "enemy",
                gold:        50f,  // < 70
                influence:   50f,
                dispatched:  CovertOpKind.CaptiveRansom,
                roster:      FullRansomRoster());
            SeedCaptorWithHeldCaptive(em, "player",
                captiveOriginFactionId: "enemy",
                captiveMemberId:        "enemy-bloodline-heir",
                captiveMemberTitle:     "Heir Designate");

            world.Update();

            int active = CountActiveOpsForKindAndFaction(em, DynastyOperationKind.CaptiveRansom, new FixedString32Bytes("enemy"));
            if (active != 0)
            {
                sb.AppendLine($"PhaseRansomInsufficientGoldBlocks FAIL: expected 0 ransom ops, got {active}");
                return false;
            }
            var resources = em.GetComponentData<ResourceStockpileComponent>(enemyEntity);
            if (resources.Gold < 49.99f || resources.Gold > 50.01f)
            {
                sb.AppendLine($"PhaseRansomInsufficientGoldBlocks FAIL: Gold must not change; got {resources.Gold}");
                return false;
            }
            sb.AppendLine("PhaseRansomInsufficientGoldBlocks PASS: low gold blocks dispatch, resources untouched");
            return true;
        }

        private static bool RunPhaseRansomNoMerchantBlocks(System.Text.StringBuilder sb)
        {
            using var world = new World("ransom-no-merchant-blocks");
            var em = world.EntityManager;
            SetupRansomSystems(world);
            SeedDualClock(em, inWorldDays: 80f);

            var enemyEntity = SeedSourceFaction(em, "enemy",
                gold:        100f,
                influence:   50f,
                dispatched:  CovertOpKind.CaptiveRansom,
                roster:      new[]
                {
                    new MemberSeed { Role = DynastyRole.Diplomat, Status = DynastyMemberStatus.Active, Renown = 25f, IdSuffix = "dip" },
                });
            SeedCaptorWithHeldCaptive(em, "player",
                captiveOriginFactionId: "enemy",
                captiveMemberId:        "enemy-bloodline-heir",
                captiveMemberTitle:     "Heir Designate");

            world.Update();

            int active = CountActiveOpsForKindAndFaction(em, DynastyOperationKind.CaptiveRansom, new FixedString32Bytes("enemy"));
            if (active != 0)
            {
                sb.AppendLine($"PhaseRansomNoMerchantBlocks FAIL: expected 0 ransom ops, got {active}");
                return false;
            }
            var resources = em.GetComponentData<ResourceStockpileComponent>(enemyEntity);
            if (resources.Gold < 99.99f || resources.Gold > 100.01f)
            {
                sb.AppendLine($"PhaseRansomNoMerchantBlocks FAIL: Gold must not change; got {resources.Gold}");
                return false;
            }
            sb.AppendLine("PhaseRansomNoMerchantBlocks PASS: no merchant blocks dispatch, resources untouched");
            return true;
        }
    }
}
#endif
