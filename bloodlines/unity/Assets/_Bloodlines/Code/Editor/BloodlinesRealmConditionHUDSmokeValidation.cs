#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Conviction;
using Bloodlines.Debug;
using Bloodlines.Faith;
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
    /// Dedicated smoke validator for the first player-HUD realm-condition slice.
    /// Proves four phases through the public HUD debug readout:
    /// 1. Baseline stable realm: green population / food / water / loyalty, neutral conviction,
    ///    uncommitted faith, and a visible cycle-progress value.
    /// 2. Realm strain: cap pressure, food strain, water strain, and loyalty distress all surface red.
    /// 3. Conviction shift: a high stewardship/oathkeeping score resolves to Apex Moral.
    /// 4. Faith commitment: covenant, doctrine, intensity tier, and green faith band surface correctly.
    /// </summary>
    public static class BloodlinesRealmConditionHUDSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-realm-condition-hud-smoke.log";

        [MenuItem("Bloodlines/HUD/Run Realm Condition HUD Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchRealmConditionHUDSmokeValidation() => RunInternal(batchMode: true);

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
                message = "Realm condition HUD smoke validation errored: " + e;
            }

            var artifact = "BLOODLINES_REALM_CONDITION_HUD_SMOKE " +
                           (success ? "PASS" : "FAIL") +
                           Environment.NewLine + message;
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
            ok &= RunRealmStrainPhase(lines);
            ok &= RunConvictionPhase(lines);
            ok &= RunFaithPhase(lines);
            report = lines.ToString();
            return ok;
        }

        private static bool RunBaselinePhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("realm-hud-phase1");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedRealmCycleConfig(entityManager);
            SeedFaction(
                entityManager,
                "player",
                populationTotal: 20,
                populationCap: 40,
                food: 60f,
                water: 60f,
                loyaltyCurrent: 70f,
                cycleAccumulator: 45f,
                cycleCount: 2);

            TickOnce(world);

            if (!TryGetSnapshotFields(debugScope.CommandSurface, "player", out var fields, out var readout))
            {
                lines.AppendLine("Phase 1 FAIL: could not read realm HUD snapshot.");
                return false;
            }

            if (ReadField(fields, "PopulationBand") != "green" ||
                ReadField(fields, "FoodBand") != "green" ||
                ReadField(fields, "WaterBand") != "green" ||
                ReadField(fields, "LoyaltyBand") != "green" ||
                ReadField(fields, "ConvictionBand") != nameof(ConvictionBand.Neutral) ||
                ReadField(fields, "FaithCommitted") != "false" ||
                ReadField(fields, "FaithBand") != "red")
            {
                lines.AppendLine($"Phase 1 FAIL: unexpected baseline snapshot '{readout}'.");
                return false;
            }

            float cycleProgress = ParseFloat(fields, "CycleProgress");
            if (Mathf.Abs(cycleProgress - 0.5f) > 0.001f)
            {
                lines.AppendLine($"Phase 1 FAIL: expected CycleProgress=0.500, got {cycleProgress.ToString("0.000", CultureInfo.InvariantCulture)}.");
                return false;
            }

            lines.AppendLine("Phase 1 PASS: stable baseline surfaces green realm bands, neutral conviction, red uncommitted faith, CycleProgress=0.500.");
            return true;
        }

        private static bool RunRealmStrainPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("realm-hud-phase2");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedRealmCycleConfig(entityManager);
            SeedFaction(
                entityManager,
                "player",
                populationTotal: 19,
                populationCap: 20,
                food: 10f,
                water: 12f,
                loyaltyCurrent: 28f,
                cycleAccumulator: 81f,
                cycleCount: 5,
                foodStrainStreak: 2,
                waterStrainStreak: 1);

            TickOnce(world);

            if (!TryGetSnapshotFields(debugScope.CommandSurface, "player", out var fields, out var readout))
            {
                lines.AppendLine("Phase 2 FAIL: could not read strain HUD snapshot.");
                return false;
            }

            if (ReadField(fields, "PopulationBand") != "red" ||
                ReadField(fields, "FoodBand") != "red" ||
                ReadField(fields, "WaterBand") != "red" ||
                ReadField(fields, "LoyaltyBand") != "red" ||
                ReadField(fields, "FoodStrainStreak") != "2" ||
                ReadField(fields, "WaterStrainStreak") != "1")
            {
                lines.AppendLine($"Phase 2 FAIL: expected red strain snapshot, got '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 2 PASS: cap pressure, food strain, water strain, and loyalty distress all surface red with visible strain streaks.");
            return true;
        }

        private static bool RunConvictionPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("realm-hud-phase3");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedRealmCycleConfig(entityManager);
            SeedFaction(
                entityManager,
                "player",
                populationTotal: 18,
                populationCap: 36,
                food: 50f,
                water: 50f,
                loyaltyCurrent: 72f,
                cycleAccumulator: 18f,
                cycleCount: 3,
                stewardship: 55f,
                oathkeeping: 25f);

            TickOnce(world);

            if (!TryGetSnapshotFields(debugScope.CommandSurface, "player", out var fields, out var readout))
            {
                lines.AppendLine("Phase 3 FAIL: could not read conviction HUD snapshot.");
                return false;
            }

            if (ReadField(fields, "ConvictionBand") != nameof(ConvictionBand.ApexMoral) ||
                ReadField(fields, "ConvictionLabel") != "Apex Moral" ||
                ReadField(fields, "ConvictionColor") != "green")
            {
                lines.AppendLine($"Phase 3 FAIL: expected Apex Moral conviction readout, got '{readout}'.");
                return false;
            }

            float convictionScore = ParseFloat(fields, "ConvictionScore");
            if (convictionScore < 75f)
            {
                lines.AppendLine($"Phase 3 FAIL: expected conviction score >= 75, got {convictionScore.ToString("0.0", CultureInfo.InvariantCulture)}.");
                return false;
            }

            lines.AppendLine($"Phase 3 PASS: ConvictionBand=ApexMoral, ConvictionLabel=Apex Moral, ConvictionScore={convictionScore.ToString("0.0", CultureInfo.InvariantCulture)}.");
            return true;
        }

        private static bool RunFaithPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("realm-hud-phase4");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedRealmCycleConfig(entityManager);
            SeedFaction(
                entityManager,
                "player",
                populationTotal: 16,
                populationCap: 32,
                food: 45f,
                water: 45f,
                loyaltyCurrent: 74f,
                cycleAccumulator: 27f,
                cycleCount: 4,
                selectedFaith: CovenantId.OldLight,
                doctrinePath: DoctrinePath.Light,
                faithIntensity: 62f,
                faithLevel: 0);

            TickOnce(world);

            if (!TryGetSnapshotFields(debugScope.CommandSurface, "player", out var fields, out var readout))
            {
                lines.AppendLine("Phase 4 FAIL: could not read faith HUD snapshot.");
                return false;
            }

            if (ReadField(fields, "FaithId") != nameof(CovenantId.OldLight) ||
                ReadField(fields, "FaithLabel") != "Old Light" ||
                ReadField(fields, "FaithCommitted") != "true" ||
                ReadField(fields, "DoctrinePath") != nameof(DoctrinePath.Light) ||
                ReadField(fields, "DoctrineLabel") != "Light" ||
                ReadField(fields, "FaithBand") != "green" ||
                ReadField(fields, "FaithTier") != "Fervent" ||
                ReadField(fields, "FaithLevel") != "4")
            {
                lines.AppendLine($"Phase 4 FAIL: expected committed fervent faith readout, got '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 4 PASS: FaithId=OldLight, DoctrinePath=Light, FaithLevel=4, FaithTier=Fervent, FaithBand=green.");
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<RealmConditionCycleSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<StarvationResponseSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<CapPressureResponseSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<StabilitySurplusResponseSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<ConvictionScoringSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<FaithIntensityResolveSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<RealmConditionHUDSystem>());
            simulationGroup.SortSystems();
            return world;
        }

        private static void TickOnce(World world)
        {
            world.SetTime(new TimeData(0d, 0.05f));
            world.Update();
        }

        private static void SeedRealmCycleConfig(EntityManager entityManager)
        {
            var configEntity = entityManager.CreateEntity(typeof(RealmCycleConfig));
            entityManager.SetComponentData(configEntity, new RealmCycleConfig
            {
                CycleSeconds = 90f,
                FoodGreenRatio = 1.35f,
                FoodYellowRatio = 1.05f,
                WaterGreenRatio = 1.35f,
                WaterYellowRatio = 1.05f,
                LoyaltyGreenFloor = 62f,
                LoyaltyYellowFloor = 32f,
                PopulationGreenCapRatio = 0.75f,
                PopulationYellowCapRatio = 0.92f,
                FoodFamineConsecutiveCycles = 2,
                WaterCrisisConsecutiveCycles = 1,
                FaminePopulationDeclinePerCycle = 2,
                WaterCrisisOutmigrationPerCycle = 1,
                FamineLoyaltyDeltaPerCycle = -2,
                WaterCrisisLoyaltyDeltaPerCycle = -1,
                PopulationCapPressureRatio = 0.95f,
                CapPressureLoyaltyDeltaPerCycle = -1,
                StabilitySurplusFoodRatio = 1.75f,
                StabilitySurplusWaterRatio = 1.75f,
                StabilitySurplusLoyaltyDeltaPerCycle = 1,
                StabilitySurplusMaxLoyaltyToApply = 95f,
            });
        }

        private static void SeedFaction(
            EntityManager entityManager,
            string factionId,
            int populationTotal,
            int populationCap,
            float food,
            float water,
            float loyaltyCurrent,
            float cycleAccumulator,
            int cycleCount,
            int foodStrainStreak = 0,
            int waterStrainStreak = 0,
            float stewardship = 0f,
            float oathkeeping = 0f,
            CovenantId selectedFaith = CovenantId.None,
            DoctrinePath doctrinePath = DoctrinePath.Unassigned,
            float faithIntensity = 0f,
            int faithLevel = 0)
        {
            var entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(PopulationComponent),
                typeof(ResourceStockpileComponent),
                typeof(RealmConditionComponent),
                typeof(FactionLoyaltyComponent),
                typeof(FaithStateComponent),
                typeof(ConvictionComponent));

            entityManager.SetComponentData(entity, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            entityManager.SetComponentData(entity, new FactionKindComponent
            {
                Kind = FactionKind.Kingdom,
            });
            entityManager.SetComponentData(entity, new PopulationComponent
            {
                Total = populationTotal,
                Available = populationTotal,
                Cap = populationCap,
                BaseCap = populationCap,
                CapBonus = 0,
                GrowthAccumulator = 0f,
            });
            entityManager.SetComponentData(entity, new ResourceStockpileComponent
            {
                Food = food,
                Water = water,
                Gold = 100f,
                Wood = 50f,
                Stone = 50f,
                Iron = 25f,
                Influence = 20f,
            });
            entityManager.SetComponentData(entity, new RealmConditionComponent
            {
                CycleAccumulator = cycleAccumulator,
                CycleCount = cycleCount,
                FoodStrainStreak = foodStrainStreak,
                WaterStrainStreak = waterStrainStreak,
                AssaultFailureStrain = 0f,
                CohesionPenaltyUntil = 0d,
                LastStarvationResponseCycle = cycleCount,
                LastCapPressureResponseCycle = cycleCount,
                LastStabilitySurplusResponseCycle = cycleCount,
            });
            entityManager.SetComponentData(entity, new FactionLoyaltyComponent
            {
                Current = loyaltyCurrent,
                Max = 100f,
                Floor = 0f,
            });
            entityManager.SetComponentData(entity, new FaithStateComponent
            {
                SelectedFaith = selectedFaith,
                DoctrinePath = doctrinePath,
                Intensity = faithIntensity,
                Level = faithLevel,
            });
            entityManager.SetComponentData(entity, new ConvictionComponent
            {
                Stewardship = stewardship,
                Oathkeeping = oathkeeping,
                Ruthlessness = 0f,
                Desecration = 0f,
                Score = 0f,
                Band = ConvictionBand.Neutral,
            });
        }

        private static bool TryGetSnapshotFields(
            BloodlinesDebugCommandSurface commandSurface,
            string factionId,
            out Dictionary<string, string> fields,
            out string readout)
        {
            fields = new Dictionary<string, string>(StringComparer.Ordinal);
            if (!commandSurface.TryDebugGetRealmConditionHUDSnapshot(factionId, out readout))
            {
                return false;
            }

            var parts = readout.Split('|');
            if (parts.Length == 0 || !string.Equals(parts[0], "RealmHUD", StringComparison.Ordinal))
            {
                return false;
            }

            for (int i = 1; i < parts.Length; i++)
            {
                int equalsIndex = parts[i].IndexOf('=');
                if (equalsIndex <= 0)
                {
                    continue;
                }

                var key = parts[i].Substring(0, equalsIndex);
                var value = parts[i].Substring(equalsIndex + 1);
                fields[key] = value;
            }

            return true;
        }

        private static string ReadField(Dictionary<string, string> fields, string key)
        {
            if (!fields.TryGetValue(key, out var value))
            {
                throw new InvalidOperationException("Missing field: " + key);
            }

            return value;
        }

        private static float ParseFloat(Dictionary<string, string> fields, string key)
        {
            var value = ReadField(fields, key);
            if (!float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
            {
                throw new InvalidOperationException("Invalid float field: " + key);
            }

            return parsed;
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

                hostObject = new GameObject("BloodlinesRealmConditionHUDSmokeValidation_CommandSurface")
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
