#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Debug;
using Bloodlines.Dynasties;
using Bloodlines.Faith;
using Bloodlines.GameTime;
using Bloodlines.HUD;
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
    /// Dedicated smoke validator for the political-state HUD panel projections.
    /// Phase 1: succession crisis HUD severity mirrors crisis pressure.
    /// Phase 2: political events tray sorts and caps active events.
    /// Phase 3: covenant test HUD reports phase, progress, and cooldown.
    /// Phase 4: Trueborn rise HUD mirrors stage and recognition state.
    /// </summary>
    public static class BloodlinesPoliticalStateHUDSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-political-state-hud-smoke.log";

        [MenuItem("Bloodlines/HUD/Run Political State HUD Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchPoliticalStateHUDSmokeValidation() => RunInternal(batchMode: true);

        private static void RunInternal(bool batchMode)
        {
            string message;
            bool success;
            try
            {
                success = RunAllPhases(out message);
            }
            catch (Exception exception)
            {
                success = false;
                message = "Political state HUD smoke validation errored: " + exception;
            }

            string artifact = "BLOODLINES_POLITICAL_STATE_HUD_SMOKE " +
                              (success ? "PASS" : "FAIL") +
                              Environment.NewLine +
                              message;
            UnityDebug.Log(artifact);
            try
            {
                string logPath = Path.GetFullPath(Path.Combine(Application.dataPath, ArtifactPath));
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
            ok &= RunSuccessionCrisisPhase(lines);
            ok &= RunPoliticalEventsTrayPhase(lines);
            ok &= RunCovenantTestPhase(lines);
            ok &= RunTruebornRisePhase(lines);
            report = lines.ToString();
            return ok;
        }

        private static bool RunSuccessionCrisisPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("political-state-hud-phase1");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 120f);
            SeedTruebornArc(entityManager);
            Entity player = SeedKingdom(entityManager, "player");
            entityManager.AddComponentData(player, new SuccessionCrisisComponent
            {
                CrisisSeverity = (byte)SuccessionCrisisSeverity.Major,
                CrisisStartedAtInWorldDays = 102f,
                RecoveryProgressPct = 0.35f,
                ResourceTrickleFactor = 0.8f,
                LoyaltyShockApplied = true,
                LegitimacyDrainRatePerDay = 1.25f,
                LoyaltyDrainRatePerDay = 0.75f,
                LastDailyTickInWorldDays = 119f,
                RecoveryRatePerDay = 0.02f,
            });

            Tick(world, 120f);

            if (!debugScope.CommandSurface.TryDebugGetSuccessionCrisisHUDSnapshot("player", out var readout))
            {
                lines.AppendLine("Phase 1 FAIL: could not read succession crisis HUD snapshot.");
                return false;
            }

            var fields = ParseFields(readout);
            if (ReadField(fields, "CrisisActive") != "true" ||
                ReadField(fields, "Severity") != nameof(SuccessionCrisisSeverity.Major) ||
                ReadField(fields, "SeverityColor") != "red" ||
                Math.Abs(ParseFloat(fields, "RecoveryProgressPct") - 0.35f) > 0.001f ||
                Math.Abs(ParseFloat(fields, "ResourceTrickleFactor") - 0.8f) > 0.001f)
            {
                lines.AppendLine($"Phase 1 FAIL: unexpected succession crisis HUD snapshot '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 1 PASS: succession crisis HUD exposes major severity, red urgency, and recovery progress.");
            return true;
        }

        private static bool RunPoliticalEventsTrayPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("political-state-hud-phase2");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 200f);
            SeedTruebornArc(entityManager);
            Entity player = SeedKingdom(entityManager, "player");
            var events = entityManager.AddBuffer<DynastyPoliticalEventComponent>(player);
            events.Add(new DynastyPoliticalEventComponent
            {
                EventType = new FixedString32Bytes("CustomWarning"),
                ExpiresAtInWorldDays = 203f,
                ResourceTrickleFactor = 1f,
                AttackMultiplier = 1f,
                StabilizationMultiplier = 1f,
            });
            events.Add(new DynastyPoliticalEventComponent
            {
                EventType = DynastyPoliticalEventTypes.CovenantTestCooldown,
                ExpiresAtInWorldDays = 205f,
                ResourceTrickleFactor = 0.95f,
                AttackMultiplier = 1f,
                StabilizationMultiplier = 1f,
            });
            events.Add(new DynastyPoliticalEventComponent
            {
                EventType = DynastyPoliticalEventTypes.DivineRightFailedCooldown,
                ExpiresAtInWorldDays = 208f,
                ResourceTrickleFactor = 1f,
                AttackMultiplier = 1f,
                StabilizationMultiplier = 1f,
            });
            events.Add(new DynastyPoliticalEventComponent
            {
                EventType = DynastyPoliticalEventTypes.SuccessionShock,
                ExpiresAtInWorldDays = 212f,
                ResourceTrickleFactor = 0.8f,
                AttackMultiplier = 0.9f,
                StabilizationMultiplier = 0.8f,
            });
            events.Add(new DynastyPoliticalEventComponent
            {
                EventType = new FixedString32Bytes("LongFuse"),
                ExpiresAtInWorldDays = 250f,
                ResourceTrickleFactor = 1f,
                AttackMultiplier = 1f,
                StabilizationMultiplier = 1f,
            });
            events.Add(new DynastyPoliticalEventComponent
            {
                EventType = new FixedString32Bytes("ExpiredEvent"),
                ExpiresAtInWorldDays = 180f,
                ResourceTrickleFactor = 1f,
                AttackMultiplier = 1f,
                StabilizationMultiplier = 1f,
            });

            Tick(world, 200f);

            if (!debugScope.CommandSurface.TryDebugGetPoliticalEventsTraySnapshot("player", out var readout))
            {
                lines.AppendLine("Phase 2 FAIL: could not read political events tray HUD snapshot.");
                return false;
            }

            string[] rows = readout.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            var header = ParseFields(rows[0]);
            var firstEntry = rows.Length > 1 ? ParseFields(rows[1]) : new Dictionary<string, string>(StringComparer.Ordinal);
            var fourthEntry = rows.Length > 4 ? ParseFields(rows[4]) : new Dictionary<string, string>(StringComparer.Ordinal);
            if (ReadField(header, "ActiveEventCount") != "5" ||
                ReadField(header, "DisplayedCount") != "4" ||
                ReadField(firstEntry, "EventType") != "CustomWarning" ||
                Math.Abs(ParseFloat(firstEntry, "RemainingInWorldDays") - 3f) > 0.001f ||
                ReadField(fourthEntry, "EventType") != "SuccessionShock")
            {
                lines.AppendLine($"Phase 2 FAIL: unexpected political events tray snapshot '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 2 PASS: political events tray counts only active events, sorts by expiry, and caps to four rows.");
            return true;
        }

        private static bool RunCovenantTestPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("political-state-hud-phase3");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 260f);
            SeedTruebornArc(entityManager);
            Entity player = SeedKingdom(entityManager, "player");
            entityManager.AddComponentData(player, new FaithStateComponent
            {
                SelectedFaith = CovenantId.BloodDominion,
                DoctrinePath = DoctrinePath.Light,
                Intensity = 84f,
                Level = 4,
            });
            entityManager.AddComponentData(player, new CovenantTestStateComponent
            {
                IntensityThresholdMetAtInWorldDays = 80f,
                TestPhase = CovenantTestPhase.Failed,
                TestStartedAtInWorldDays = 210f,
                LastFailedAtInWorldDays = 240f,
                SuccessCount = 1,
            });

            var events = EnsurePoliticalEvents(entityManager, player);
            events.Add(new DynastyPoliticalEventComponent
            {
                EventType = DynastyPoliticalEventTypes.CovenantTestCooldown,
                ExpiresAtInWorldDays = 300f,
                ResourceTrickleFactor = 0.92f,
                AttackMultiplier = 0.95f,
                StabilizationMultiplier = 0.9f,
            });

            Tick(world, 260f);

            if (!debugScope.CommandSurface.TryDebugGetCovenantTestProgressHUDSnapshot("player", out var readout))
            {
                lines.AppendLine("Phase 3 FAIL: could not read covenant test progress HUD snapshot.");
                return false;
            }

            var fields = ParseFields(readout);
            if (ReadField(fields, "FaithId") != nameof(CovenantId.BloodDominion) ||
                ReadField(fields, "TestPhase") != nameof(CovenantTestPhase.Failed) ||
                ReadField(fields, "PhaseLabel") != "Failed" ||
                Math.Abs(ParseFloat(fields, "ProgressPct") - 1f) > 0.001f ||
                Math.Abs(ParseFloat(fields, "CooldownRemainingInWorldDays") - 40f) > 0.001f ||
                ReadField(fields, "SuccessCount") != "1")
            {
                lines.AppendLine($"Phase 3 FAIL: unexpected covenant test HUD snapshot '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 3 PASS: covenant test HUD shows failed state with full qualification progress and remaining cooldown.");
            return true;
        }

        private static bool RunTruebornRisePhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("political-state-hud-phase4");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 4200f);
            Entity player = SeedKingdom(entityManager, "player");
            SeedKingdom(entityManager, "enemy");
            Entity arcEntity = SeedTruebornArc(entityManager);
            DynamicBuffer<TruebornRiseFactionRecognitionSlotElement> recognitionSlots =
                entityManager.GetBuffer<TruebornRiseFactionRecognitionSlotElement>(arcEntity);
            recognitionSlots.Add(new TruebornRiseFactionRecognitionSlotElement
            {
                FactionId = new FixedString32Bytes("player"),
            });
            recognitionSlots.Add(new TruebornRiseFactionRecognitionSlotElement
            {
                FactionId = new FixedString32Bytes("enemy"),
            });

            entityManager.SetComponentData(arcEntity, new TruebornRiseArcComponent
            {
                CurrentStage = 2,
                StageStartedAtInWorldDays = 4000f,
                GlobalPressurePerDay = 0.6f,
                LoyaltyErosionPerDay = 1.8f,
                RecognizedFactionsBitmask = 1UL,
                ChallengeLevel = 2,
                UnchallengedCycles = 4,
            });

            Tick(world, 4200f);

            if (!debugScope.CommandSurface.TryDebugGetTruebornRiseHUDSnapshot("player", out var readout))
            {
                lines.AppendLine("Phase 4 FAIL: could not read Trueborn rise HUD snapshot.");
                return false;
            }

            var fields = ParseFields(readout);
            if (ReadField(fields, "CurrentStage") != "2" ||
                ReadField(fields, "StageLabel") != "Escalation" ||
                ReadField(fields, "Recognized") != "true" ||
                ReadField(fields, "RecognizedFactionCount") != "1" ||
                ReadField(fields, "ChallengeLevel") != "2" ||
                Math.Abs(ParseFloat(fields, "GlobalPressurePerDay") - 0.6f) > 0.001f ||
                Math.Abs(ParseFloat(fields, "LoyaltyErosionPerDay") - 1.8f) > 0.001f)
            {
                lines.AppendLine($"Phase 4 FAIL: unexpected Trueborn rise HUD snapshot '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 4 PASS: Trueborn rise HUD mirrors the shared escalation stage and per-faction recognition state.");
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var presentation = world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            presentation.AddSystemToUpdateList(world.GetOrCreateSystem<SuccessionCrisisHUDSystem>());
            presentation.AddSystemToUpdateList(world.GetOrCreateSystem<PoliticalEventsTrayHUDSystem>());
            presentation.AddSystemToUpdateList(world.GetOrCreateSystem<CovenantTestProgressHUDSystem>());
            presentation.AddSystemToUpdateList(world.GetOrCreateSystem<TruebornRiseHUDSystem>());
            presentation.SortSystems();
            return world;
        }

        private static void SeedDualClock(EntityManager entityManager, float inWorldDays)
        {
            Entity entity = entityManager.CreateEntity(typeof(DualClockComponent));
            entityManager.SetComponentData(entity, new DualClockComponent
            {
                InWorldDays = inWorldDays,
                DaysPerRealSecond = 2f,
                DeclarationCount = 0,
            });
        }

        private static Entity SeedKingdom(EntityManager entityManager, string factionId)
        {
            Entity entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent));
            entityManager.SetComponentData(entity, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            entityManager.SetComponentData(entity, new FactionKindComponent
            {
                Kind = FactionKind.Kingdom,
            });
            return entity;
        }

        private static Entity SeedTruebornArc(EntityManager entityManager)
        {
            Entity entity = entityManager.CreateEntity(typeof(TruebornRiseArcComponent));
            entityManager.SetComponentData(entity, new TruebornRiseArcComponent
            {
                CurrentStage = 0,
                StageStartedAtInWorldDays = 0f,
                GlobalPressurePerDay = 0f,
                LoyaltyErosionPerDay = 0f,
                RecognizedFactionsBitmask = 0UL,
                ChallengeLevel = 0,
                UnchallengedCycles = 0,
            });
            entityManager.AddBuffer<TruebornRiseFactionRecognitionSlotElement>(entity);
            return entity;
        }

        private static DynamicBuffer<DynastyPoliticalEventComponent> EnsurePoliticalEvents(
            EntityManager entityManager,
            Entity factionEntity)
        {
            if (!entityManager.HasBuffer<DynastyPoliticalEventComponent>(factionEntity))
            {
                entityManager.AddBuffer<DynastyPoliticalEventComponent>(factionEntity);
            }

            return entityManager.GetBuffer<DynastyPoliticalEventComponent>(factionEntity);
        }

        private static void Tick(World world, float inWorldDays)
        {
            EntityManager entityManager = world.EntityManager;
            using var query = entityManager.CreateEntityQuery(ComponentType.ReadWrite<DualClockComponent>());
            var dualClock = query.GetSingleton<DualClockComponent>();
            dualClock.InWorldDays = inWorldDays;
            query.SetSingleton(dualClock);
            world.SetTime(new TimeData(inWorldDays, 0.05f));
            world.Update();
        }

        private static Dictionary<string, string> ParseFields(string readout)
        {
            var fields = new Dictionary<string, string>(StringComparer.Ordinal);
            string[] parts = readout.Split('|');
            for (int i = 1; i < parts.Length; i++)
            {
                int separatorIndex = parts[i].IndexOf('=');
                if (separatorIndex <= 0)
                {
                    continue;
                }

                fields[parts[i].Substring(0, separatorIndex)] = parts[i].Substring(separatorIndex + 1);
            }

            return fields;
        }

        private static string ReadField(Dictionary<string, string> fields, string key)
        {
            return fields.TryGetValue(key, out string value) ? value : string.Empty;
        }

        private static float ParseFloat(Dictionary<string, string> fields, string key)
        {
            string value = ReadField(fields, key);
            if (string.Equals(value, "NaN", StringComparison.OrdinalIgnoreCase))
            {
                return float.NaN;
            }

            return float.Parse(value, CultureInfo.InvariantCulture);
        }

        private sealed class DebugCommandSurfaceScope : IDisposable
        {
            private readonly World previousDefaultWorld;
            private readonly GameObject hostObject;

            public DebugCommandSurfaceScope(World world)
            {
                previousDefaultWorld = World.DefaultGameObjectInjectionWorld;
                World.DefaultGameObjectInjectionWorld = world;
                hostObject = new GameObject("BloodlinesPoliticalStateHUDSmokeValidation_CommandSurface")
                {
                    hideFlags = HideFlags.HideAndDontSave,
                };
                CommandSurface = hostObject.AddComponent<BloodlinesDebugCommandSurface>();
            }

            public BloodlinesDebugCommandSurface CommandSurface { get; }

            public void Dispose()
            {
                UnityEngine.Object.DestroyImmediate(hostObject);
                World.DefaultGameObjectInjectionWorld = previousDefaultWorld;
            }
        }
    }
}
#endif
