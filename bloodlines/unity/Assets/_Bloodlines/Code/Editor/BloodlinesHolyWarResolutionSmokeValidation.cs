#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Smoke validator for sub-slice 26: AI holy war resolution effects.
    /// Covers AIHolyWarResolutionSystem Phase A (declaration landing)
    /// and Phase B (war-tick sustained effects, pulse, expiration).
    ///
    /// Browser reference:
    ///   simulation.js tickDynastyOperations holy_war branch (~5590-5645):
    ///     declaration landing, initial intensity/loyalty effects,
    ///     conviction events, finalize operation.
    ///   simulation.js tickFaithHolyWars (~4160-4215):
    ///     sustained loyalty drain, pulse at HOLY_WAR_PULSE_INTERVAL_SECONDS=30,
    ///     expiration pruning.
    ///   simulation.js createHolyWarEntry (~10505-10520):
    ///     holy war entry shape; ActiveHolyWarElement is the Unity equivalent.
    ///
    /// Phases:
    ///   PhaseDeclarationLanding: Operation past ResolveAtInWorldDays ->
    ///     op finalized (Active=false), ActiveHolyWarElement entry created on
    ///     source faction, source intensity boosted by max(3, pulse*2),
    ///     target control-point loyalty drained by max(2, loyaltyPulse),
    ///     conviction events applied, narrative pushed.
    ///   PhaseDeclarationVoidNoTarget: Target entity missing -> op finalized
    ///     (Active=false), no war entry created, void narrative pushed.
    ///   PhasePulseAndTick: Active war in buffer with LastPulseAt at day 0
    ///     and inWorldDays=100 (>= 0+60 interval) -> pulse fires; source
    ///     intensity += IntensityPulse, target control-point loyalty -= LoyaltyPulse.
    ///   PhaseWarExpiration: War entry past ExpiresAtInWorldDays -> entry
    ///     pruned from buffer, expiration narrative pushed for player-related war.
    ///
    /// Artifact: artifacts/unity-holy-war-resolution-smoke.log.
    /// </summary>
    public static class BloodlinesHolyWarResolutionSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-holy-war-resolution-smoke.log";

        [UnityEditor.MenuItem("Bloodlines/AI/Run Holy War Resolution Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchHolyWarResolutionSmokeValidation() =>
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
                message = "Holy war resolution smoke errored: " + e;
            }

            string artifact = "BLOODLINES_HOLY_WAR_RESOLUTION_SMOKE " +
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
            ok &= RunPhaseDeclarationLanding(sb);
            ok &= RunPhaseDeclarationVoidNoTarget(sb);
            ok &= RunPhasePulseAndTick(sb);
            ok &= RunPhaseWarExpiration(sb);
            report = sb.ToString();
            return ok;
        }

        // ------------------------------------------------------------------ shared helpers

        private static SimulationSystemGroup SetupResolutionSystems(World world)
        {
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var sg = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AIHolyWarResolutionSystem>());
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

        private static Entity CreateFactionWithFaith(
            EntityManager em,
            string factionId,
            CovenantId faith,
            DoctrinePath doctrine,
            float intensity)
        {
            var e = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(FaithStateComponent));
            em.SetComponentData(e, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            em.SetComponentData(e, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(e, new FaithStateComponent
            {
                SelectedFaith = faith,
                DoctrinePath  = doctrine,
                Intensity     = intensity,
                Level         = 1,
            });
            return e;
        }

        private static Entity CreateControlPoint(
            EntityManager em,
            string ownerFactionId,
            float loyalty)
        {
            var e = em.CreateEntity(typeof(ControlPointComponent));
            em.SetComponentData(e, new ControlPointComponent
            {
                ControlPointId = new FixedString32Bytes("cp-test"),
                OwnerFactionId = new FixedString32Bytes(ownerFactionId),
                Loyalty        = loyalty,
            });
            return e;
        }

        private static Entity CreateHolyWarOp(
            EntityManager em,
            string sourceFactionId,
            string targetFactionId,
            float resolveAtInWorldDays,
            float warExpiresAtInWorldDays,
            float intensityPulse,
            float loyaltyPulse)
        {
            var opEntity = em.CreateEntity(
                typeof(DynastyOperationComponent),
                typeof(DynastyOperationHolyWarComponent));
            em.SetComponentData(opEntity, new DynastyOperationComponent
            {
                OperationId          = new FixedString64Bytes("dynop-holywar-test"),
                SourceFactionId      = new FixedString32Bytes(sourceFactionId),
                TargetFactionId      = new FixedString32Bytes(targetFactionId),
                OperationKind        = DynastyOperationKind.HolyWar,
                StartedAtInWorldDays = 0f,
                Active               = true,
            });
            em.SetComponentData(opEntity, new DynastyOperationHolyWarComponent
            {
                ResolveAtInWorldDays    = resolveAtInWorldDays,
                WarExpiresAtInWorldDays = warExpiresAtInWorldDays,
                OperatorMemberId        = new FixedString64Bytes("op-il-1"),
                OperatorTitle           = new FixedString64Bytes("Ideological Leader"),
                IntensityPulse          = intensityPulse,
                LoyaltyPulse            = loyaltyPulse,
                CompatibilityLabel      = new FixedString64Bytes("discordant"),
                IntensityCost           = AIHolyWarExecutionSystem.HolyWarIntensityCost,
                EscrowCost              = new DynastyOperationEscrowCost
                {
                    Influence = AIHolyWarExecutionSystem.HolyWarInfluenceCost,
                },
            });
            return opEntity;
        }

        // ------------------------------------------------------------------ phases

        private static bool RunPhaseDeclarationLanding(System.Text.StringBuilder sb)
        {
            using var world = new World("holy-war-resolution-declaration-landing");
            var em = world.EntityManager;
            SetupResolutionSystems(world);
            SeedDualClock(em, inWorldDays: 100f);

            // Source faction: enemy, OldLight/Light, intensity=20.
            var sourceEntity = CreateFactionWithFaith(em, "enemy",
                CovenantId.OldLight, DoctrinePath.Light, 20f);

            // Target faction: player, TheWild/Light.
            var targetEntity = CreateFactionWithFaith(em, "player",
                CovenantId.TheWild, DoctrinePath.Light, 15f);

            // Target control point with loyalty=80.
            var cpEntity = CreateControlPoint(em, "player", 80f);

            // Op: resolveAt=50 (past, since inWorldDays=100), warExpires=230,
            // light pulses: intensityPulse=0.9, loyaltyPulse=1.2.
            var opEntity = CreateHolyWarOp(em,
                sourceFactionId:       "enemy",
                targetFactionId:       "player",
                resolveAtInWorldDays:  50f,
                warExpiresAtInWorldDays: 230f,
                intensityPulse:        0.9f,
                loyaltyPulse:          1.2f);

            int messagesBefore = NarrativeMessageBridge.Count(em);
            world.Update();

            // Verify op is finalized.
            var op = em.GetComponentData<DynastyOperationComponent>(opEntity);
            if (op.Active)
            {
                sb.AppendLine("PhaseDeclarationLanding FAIL: DynastyOperationComponent.Active should be false after resolution");
                return false;
            }

            // Verify ActiveHolyWarElement created on source entity.
            if (!em.HasBuffer<ActiveHolyWarElement>(sourceEntity))
            {
                sb.AppendLine("PhaseDeclarationLanding FAIL: ActiveHolyWarElement buffer not created on source faction");
                return false;
            }
            var warBuffer = em.GetBuffer<ActiveHolyWarElement>(sourceEntity, true);
            if (warBuffer.Length != 1)
            {
                sb.AppendLine($"PhaseDeclarationLanding FAIL: expected 1 ActiveHolyWarElement entry, got {warBuffer.Length}");
                return false;
            }
            var warEntry = warBuffer[0];
            if (!warEntry.TargetFactionId.Equals(new FixedString32Bytes("player")))
            {
                sb.AppendLine($"PhaseDeclarationLanding FAIL: war entry TargetFactionId expected 'player', got '{warEntry.TargetFactionId}'");
                return false;
            }
            if (warEntry.ExpiresAtInWorldDays < 229.99f || warEntry.ExpiresAtInWorldDays > 230.01f)
            {
                sb.AppendLine($"PhaseDeclarationLanding FAIL: ExpiresAtInWorldDays expected 230, got {warEntry.ExpiresAtInWorldDays}");
                return false;
            }

            // Verify source intensity boosted: 20 + max(3, 0.9*2) = 20 + 3 = 23.
            var sourceFaith = em.GetComponentData<FaithStateComponent>(sourceEntity);
            float expectedIntensity = 20f + math.max(3f, 0.9f * 2f); // = 23f
            if (sourceFaith.Intensity < expectedIntensity - 0.01f || sourceFaith.Intensity > expectedIntensity + 0.01f)
            {
                sb.AppendLine($"PhaseDeclarationLanding FAIL: source intensity expected {expectedIntensity}, got {sourceFaith.Intensity}");
                return false;
            }

            // Verify target control point loyalty drained: 80 - max(2, 1.2) = 78.
            var cp = em.GetComponentData<ControlPointComponent>(cpEntity);
            float expectedLoyalty = 80f - math.max(2f, 1.2f); // = 78f
            if (cp.Loyalty < expectedLoyalty - 0.01f || cp.Loyalty > expectedLoyalty + 0.01f)
            {
                sb.AppendLine($"PhaseDeclarationLanding FAIL: target CP loyalty expected {expectedLoyalty}, got {cp.Loyalty}");
                return false;
            }

            // Verify narrative pushed (+1 message; tone=Warn since target=player).
            int messagesAfter = NarrativeMessageBridge.Count(em);
            if (messagesAfter != messagesBefore + 1)
            {
                sb.AppendLine($"PhaseDeclarationLanding FAIL: narrative count expected +1, got {messagesAfter - messagesBefore}");
                return false;
            }

            sb.AppendLine($"PhaseDeclarationLanding PASS: op finalized, war entry created (expires=230), " +
                          $"source intensity 20->{sourceFaith.Intensity}, target CP loyalty 80->{cp.Loyalty}, narrative +1");
            return true;
        }

        private static bool RunPhaseDeclarationVoidNoTarget(System.Text.StringBuilder sb)
        {
            using var world = new World("holy-war-resolution-void-no-target");
            var em = world.EntityManager;
            SetupResolutionSystems(world);
            SeedDualClock(em, inWorldDays: 100f);

            // Source faction only; no target entity.
            var sourceEntity = CreateFactionWithFaith(em, "enemy",
                CovenantId.OldLight, DoctrinePath.Light, 20f);

            var opEntity = CreateHolyWarOp(em,
                sourceFactionId:       "enemy",
                targetFactionId:       "player",
                resolveAtInWorldDays:  50f,
                warExpiresAtInWorldDays: 230f,
                intensityPulse:        0.9f,
                loyaltyPulse:          1.2f);

            int messagesBefore = NarrativeMessageBridge.Count(em);
            world.Update();

            // Op should be finalized (Active=false) -- void branch.
            var op = em.GetComponentData<DynastyOperationComponent>(opEntity);
            if (op.Active)
            {
                sb.AppendLine("PhaseDeclarationVoidNoTarget FAIL: op.Active should be false (void finalization)");
                return false;
            }

            // No war entry should be created on source.
            bool hasBuffer = em.HasBuffer<ActiveHolyWarElement>(sourceEntity);
            int entryCount = hasBuffer ? em.GetBuffer<ActiveHolyWarElement>(sourceEntity, true).Length : 0;
            if (entryCount != 0)
            {
                sb.AppendLine($"PhaseDeclarationVoidNoTarget FAIL: expected 0 ActiveHolyWarElement entries, got {entryCount}");
                return false;
            }

            // Source intensity unchanged.
            var sourceFaith = em.GetComponentData<FaithStateComponent>(sourceEntity);
            if (sourceFaith.Intensity < 19.99f || sourceFaith.Intensity > 20.01f)
            {
                sb.AppendLine($"PhaseDeclarationVoidNoTarget FAIL: source intensity should be unchanged (20), got {sourceFaith.Intensity}");
                return false;
            }

            // Void narrative should be pushed.
            int messagesAfter = NarrativeMessageBridge.Count(em);
            if (messagesAfter != messagesBefore + 1)
            {
                sb.AppendLine($"PhaseDeclarationVoidNoTarget FAIL: void narrative expected +1, got {messagesAfter - messagesBefore}");
                return false;
            }

            sb.AppendLine("PhaseDeclarationVoidNoTarget PASS: op finalized (Active=false), no war entry, intensity unchanged, void narrative pushed");
            return true;
        }

        private static bool RunPhasePulseAndTick(System.Text.StringBuilder sb)
        {
            using var world = new World("holy-war-resolution-pulse-and-tick");
            var em = world.EntityManager;
            SetupResolutionSystems(world);
            SeedDualClock(em, inWorldDays: 100f);

            // Source faction with faith.
            var sourceEntity = CreateFactionWithFaith(em, "enemy",
                CovenantId.BloodDominion, DoctrinePath.Dark, 10f);

            // Target faction with control point.
            CreateFactionWithFaith(em, "player", CovenantId.TheWild, DoctrinePath.Light, 5f);
            var cpEntity = CreateControlPoint(em, "player", 70f);

            // Manually seed an ActiveHolyWarElement on the source entity.
            // LastPulseAtInWorldDays=0, ExpiresAt=300 (not expired at day 100).
            // With inWorldDays=100 >= 0 + PulseIntervalInWorldDays(60), pulse fires.
            var warBuffer = em.AddBuffer<ActiveHolyWarElement>(sourceEntity);
            warBuffer.Add(new ActiveHolyWarElement
            {
                Id                     = new FixedString64Bytes("dynop-holywar-test"),
                TargetFactionId        = new FixedString32Bytes("player"),
                FaithId                = CovenantId.BloodDominion,
                DocPath                = DoctrinePath.Dark,
                DeclaredAtInWorldDays  = 0f,
                LastPulseAtInWorldDays = 0f,     // 0 + 60 <= 100 -> pulse fires
                ExpiresAtInWorldDays   = 300f,   // not expired
                IntensityPulse         = 1.2f,   // dark-doctrine
                LoyaltyPulse           = 1.8f,
            });

            int messagesBefore = NarrativeMessageBridge.Count(em);
            world.Update();

            // Source intensity should increase by IntensityPulse (1.2): 10 -> 11.2.
            var sourceFaith = em.GetComponentData<FaithStateComponent>(sourceEntity);
            // Note: sustained drain doesn't affect source. Only pulse boost does.
            if (sourceFaith.Intensity < 11.1f || sourceFaith.Intensity > 11.3f)
            {
                sb.AppendLine($"PhasePulseAndTick FAIL: source intensity expected ~11.2 (10 + 1.2 pulse), got {sourceFaith.Intensity}");
                return false;
            }

            // Target control point loyalty drained by loyaltyPulse (1.8) on pulse:
            // 70 - 1.8 = 68.2, then minus sustained drain (dt*1.5*1.8, dt is small).
            var cp = em.GetComponentData<ControlPointComponent>(cpEntity);
            float loyaltyAfterPulse = 70f - 1.8f;  // 68.2
            if (cp.Loyalty > loyaltyAfterPulse + 0.01f)
            {
                sb.AppendLine($"PhasePulseAndTick FAIL: target CP loyalty expected <= {loyaltyAfterPulse + 0.01f} (pulse fired), got {cp.Loyalty}");
                return false;
            }

            // War entry should still be in buffer (not expired).
            var updatedBuffer = em.GetBuffer<ActiveHolyWarElement>(sourceEntity, true);
            if (updatedBuffer.Length != 1)
            {
                sb.AppendLine($"PhasePulseAndTick FAIL: expected 1 war entry in buffer (not expired), got {updatedBuffer.Length}");
                return false;
            }
            // LastPulseAtInWorldDays should be updated to 100.
            if (updatedBuffer[0].LastPulseAtInWorldDays < 99.99f ||
                updatedBuffer[0].LastPulseAtInWorldDays > 100.01f)
            {
                sb.AppendLine($"PhasePulseAndTick FAIL: LastPulseAtInWorldDays expected 100, got {updatedBuffer[0].LastPulseAtInWorldDays}");
                return false;
            }

            sb.AppendLine($"PhasePulseAndTick PASS: pulse fired, source intensity 10->{sourceFaith.Intensity}, " +
                          $"target CP loyalty 70->{cp.Loyalty}, LastPulseAt updated to 100, entry retained");
            return true;
        }

        private static bool RunPhaseWarExpiration(System.Text.StringBuilder sb)
        {
            using var world = new World("holy-war-resolution-expiration");
            var em = world.EntityManager;
            SetupResolutionSystems(world);
            SeedDualClock(em, inWorldDays: 250f);

            // Source faction: player (so expiration narrative fires).
            var sourceEntity = CreateFactionWithFaith(em, "player",
                CovenantId.OldLight, DoctrinePath.Light, 5f);

            // Target faction.
            CreateFactionWithFaith(em, "enemy", CovenantId.BloodDominion, DoctrinePath.Dark, 5f);

            // Seed war entry past ExpiresAtInWorldDays (100 < 250 = expired).
            var warBuffer = em.AddBuffer<ActiveHolyWarElement>(sourceEntity);
            warBuffer.Add(new ActiveHolyWarElement
            {
                Id                     = new FixedString64Bytes("dynop-holywar-expired"),
                TargetFactionId        = new FixedString32Bytes("enemy"),
                FaithId                = CovenantId.OldLight,
                DocPath                = DoctrinePath.Light,
                DeclaredAtInWorldDays  = 0f,
                LastPulseAtInWorldDays = 200f,
                ExpiresAtInWorldDays   = 100f,   // expired at day 100, inWorldDays=250
                IntensityPulse         = 0.9f,
                LoyaltyPulse           = 1.2f,
            });

            int messagesBefore = NarrativeMessageBridge.Count(em);
            world.Update();

            // Buffer should be empty (entry pruned).
            var updatedBuffer = em.GetBuffer<ActiveHolyWarElement>(sourceEntity, true);
            if (updatedBuffer.Length != 0)
            {
                sb.AppendLine($"PhaseWarExpiration FAIL: expected 0 entries after expiration, got {updatedBuffer.Length}");
                return false;
            }

            // Expiration narrative should be pushed (source=player -> tone=Info).
            int messagesAfter = NarrativeMessageBridge.Count(em);
            if (messagesAfter != messagesBefore + 1)
            {
                sb.AppendLine($"PhaseWarExpiration FAIL: expiration narrative expected +1, got {messagesAfter - messagesBefore}");
                return false;
            }

            sb.AppendLine("PhaseWarExpiration PASS: expired war entry pruned, expiration narrative pushed");
            return true;
        }
    }
}
#endif
