#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Conviction;
using Bloodlines.Debug;
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
    /// Dedicated smoke validator for the consolidated battlefield command deck summary.
    /// Proves the live Unity command surface now binds the match, realm, faith/conviction,
    /// and victory HUD read-models into a single player-facing summary seam.
    /// </summary>
    public static class BloodlinesBattlefieldCommandDeckSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-battlefield-command-deck-smoke.log";

        [MenuItem("Bloodlines/HUD/Run Battlefield Command Deck Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchBattlefieldCommandDeckSmokeValidation() => RunInternal(batchMode: true);

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
                message = "Battlefield command deck smoke validation errored: " + e;
            }

            var artifact = "BLOODLINES_BATTLEFIELD_COMMAND_DECK_SMOKE " +
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
            ok &= RunBaselineLeaderPhase(lines);
            ok &= RunGreatReckoningPhase(lines);
            report = lines.ToString();
            return ok;
        }

        private static bool RunBaselineLeaderPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("battlefield-command-deck-phase1");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedRealmCycleConfig(entityManager);
            var player = SeedFaction(
                entityManager,
                "player",
                populationTotal: 22,
                populationCap: 40,
                food: 60f,
                water: 58f,
                loyaltyCurrent: 75f,
                cycleAccumulator: 45f,
                cycleCount: 4,
                stewardship: 55f,
                oathkeeping: 24f,
                selectedFaith: CovenantId.OldLight,
                doctrinePath: DoctrinePath.Light,
                faithIntensity: 62f,
                faithLevel: 0);
            var enemy = SeedFaction(
                entityManager,
                "enemy",
                populationTotal: 19,
                populationCap: 36,
                food: 42f,
                water: 40f,
                loyaltyCurrent: 58f,
                cycleAccumulator: 45f,
                cycleCount: 4);
            SeedVictoryReadout(entityManager, player, ("TerritorialGovernance", 0.67f, true));
            SeedVictoryReadout(entityManager, enemy, ("DivineRight", 0.45f, true));
            SeedMatchProgression(
                entityManager,
                stageNumber: 3,
                phaseLabel: "Commitment",
                inWorldYears: 1.24f,
                declarationCount: 2,
                dominantLeaderFactionId: "player",
                dominantLeaderTerritoryShare: 0.67f);
            SeedWorldPressure(entityManager, "player", level: 1, label: "rising", score: 4, targeted: true);

            TickOnce(world);

            if (!TryGetSummaryFields(debugScope.CommandSurface, out var fields, out var readout))
            {
                lines.AppendLine("Phase 1 FAIL: could not read command deck summary.");
                return false;
            }

            if (ReadField(fields, "FactionId") != "player" ||
                ReadField(fields, "PhaseLabel") != "Commitment" ||
                ReadField(fields, "WorldPressureLabel") != "rising" ||
                ReadField(fields, "PopulationBand") != "green" ||
                ReadField(fields, "FoodBand") != "green" ||
                ReadField(fields, "WaterBand") != "green" ||
                ReadField(fields, "LoyaltyBand") != "green" ||
                ReadField(fields, "ConvictionLabel") != "Apex Moral" ||
                ReadField(fields, "FaithLabel") != "Old Light" ||
                ReadField(fields, "FaithTier") != "Fervent" ||
                ReadField(fields, "PlayerVictoryRank") != "1" ||
                ReadField(fields, "TopFactionId") != "player")
            {
                lines.AppendLine($"Phase 1 FAIL: unexpected baseline summary '{readout}'.");
                return false;
            }

            lines.AppendLine("Phase 1 PASS: command deck binds rising pressure, stable realm bands, Apex Moral conviction, Old Light Fervent faith, and player-led victory rank 1.");
            return true;
        }

        private static bool RunGreatReckoningPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("battlefield-command-deck-phase2");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedRealmCycleConfig(entityManager);
            var player = SeedFaction(
                entityManager,
                "player",
                populationTotal: 18,
                populationCap: 20,
                food: 12f,
                water: 11f,
                loyaltyCurrent: 29f,
                cycleAccumulator: 70f,
                cycleCount: 8);
            var enemy = SeedFaction(
                entityManager,
                "enemy",
                populationTotal: 28,
                populationCap: 36,
                food: 74f,
                water: 70f,
                loyaltyCurrent: 71f,
                cycleAccumulator: 70f,
                cycleCount: 8);
            SeedVictoryReadout(entityManager, player, ("TerritorialGovernance", 0.41f, true));
            SeedVictoryReadout(entityManager, enemy, ("DivineRight", 0.81f, true));
            SeedMatchProgression(
                entityManager,
                stageNumber: 5,
                phaseLabel: "Resolution",
                inWorldYears: 1.92f,
                declarationCount: 4,
                dominantLeaderFactionId: "enemy",
                dominantLeaderTerritoryShare: 0.81f,
                greatReckoningActive: true,
                greatReckoningTargetFactionId: "enemy",
                greatReckoningShare: 0.81f);
            SeedWorldPressure(entityManager, "enemy", level: 3, label: "convergence", score: 8, targeted: true);

            TickOnce(world);

            if (!TryGetSummaryFields(debugScope.CommandSurface, out var fields, out var readout))
            {
                lines.AppendLine("Phase 2 FAIL: could not read Great Reckoning command deck summary.");
                return false;
            }

            if (ReadField(fields, "GreatReckoningActive") != "true" ||
                ReadField(fields, "GreatReckoningTargetFactionId") != "enemy" ||
                ReadField(fields, "WorldPressureLabel") != "convergence" ||
                ReadField(fields, "PopulationBand") != "yellow" ||
                ReadField(fields, "FoodBand") != "red" ||
                ReadField(fields, "WaterBand") != "red" ||
                ReadField(fields, "LoyaltyBand") != "red" ||
                ReadField(fields, "PlayerVictoryRank") != "2" ||
                ReadField(fields, "TopFactionId") != "enemy" ||
                ReadField(fields, "TopVictoryConditionId") != "DivineRight")
            {
                lines.AppendLine($"Phase 2 FAIL: unexpected Great Reckoning summary '{readout}'.");
                return false;
            }

            float reckoningShare = ParseFloat(fields, "GreatReckoningShare");
            if (Mathf.Abs(reckoningShare - 0.81f) > 0.001f)
            {
                lines.AppendLine($"Phase 2 FAIL: expected GreatReckoningShare=0.810, got {reckoningShare.ToString("0.000", CultureInfo.InvariantCulture)}.");
                return false;
            }

            lines.AppendLine("Phase 2 PASS: command deck surfaces convergence pressure, Great Reckoning on enemy at 81%, yellow cap pressure plus red supply/loyalty strain, and player victory rank 2.");
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            var presentationGroup = world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<RealmConditionCycleSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<StarvationResponseSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<CapPressureResponseSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<StabilitySurplusResponseSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<ConvictionScoringSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<FaithIntensityResolveSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<RealmConditionHUDSystem>());
            presentationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<MatchProgressionHUDSystem>());
            presentationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<VictoryLeaderboardHUDSystem>());
            simulationGroup.SortSystems();
            presentationGroup.SortSystems();
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

        private static Entity SeedFaction(
            EntityManager entityManager,
            string factionId,
            int populationTotal,
            int populationCap,
            float food,
            float water,
            float loyaltyCurrent,
            float cycleAccumulator,
            int cycleCount,
            float stewardship = 0f,
            float oathkeeping = 0f,
            CovenantId selectedFaith = CovenantId.None,
            DoctrinePath doctrinePath = DoctrinePath.Unassigned,
            float faithIntensity = 0f,
            int faithLevel = 0)
        {
            var entity = entityManager.CreateEntity(
                typeof(FactionComponent),
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
            entityManager.SetComponentData(entity, new PopulationComponent
            {
                Total = populationTotal,
                Cap = populationCap,
                Available = populationCap - populationTotal,
            });
            entityManager.SetComponentData(entity, new ResourceStockpileComponent
            {
                Food = food,
                Water = water,
            });
            entityManager.SetComponentData(entity, new RealmConditionComponent
            {
                CycleAccumulator = cycleAccumulator,
                CycleCount = cycleCount,
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
            });
            entityManager.AddBuffer<VictoryConditionReadoutComponent>(entity);
            return entity;
        }

        private static void SeedVictoryReadout(
            EntityManager entityManager,
            Entity factionEntity,
            params (string ConditionId, float ProgressPct, bool IsLeading)[] entries)
        {
            var buffer = entityManager.GetBuffer<VictoryConditionReadoutComponent>(factionEntity);
            buffer.Clear();
            foreach (var entry in entries)
            {
                buffer.Add(new VictoryConditionReadoutComponent
                {
                    ConditionId = new FixedString32Bytes(entry.ConditionId),
                    ProgressPct = entry.ProgressPct,
                    IsLeading = entry.IsLeading,
                    TimeRemainingEstimateInWorldDays = float.NaN,
                });
            }
        }

        private static void SeedMatchProgression(
            EntityManager entityManager,
            int stageNumber,
            string phaseLabel,
            float inWorldYears,
            int declarationCount,
            string dominantLeaderFactionId,
            float dominantLeaderTerritoryShare,
            bool greatReckoningActive = false,
            string greatReckoningTargetFactionId = "",
            float greatReckoningShare = 0f)
        {
            var entity = entityManager.CreateEntity(typeof(MatchProgressionComponent));
            entityManager.SetComponentData(entity, new MatchProgressionComponent
            {
                StageNumber = stageNumber,
                StageId = new FixedString32Bytes("stage_" + stageNumber),
                StageLabel = new FixedString64Bytes("Stage " + stageNumber),
                PhaseId = new FixedString32Bytes(phaseLabel.ToLowerInvariant()),
                PhaseLabel = new FixedString32Bytes(phaseLabel),
                StageReadiness = 0.5f,
                InWorldYears = inWorldYears,
                InWorldDays = inWorldYears * 365f,
                DeclarationCount = declarationCount,
                DominantKingdomId = new FixedString32Bytes(dominantLeaderFactionId),
                DominantTerritoryShare = dominantLeaderTerritoryShare,
                GreatReckoningActive = greatReckoningActive,
                GreatReckoningTargetFactionId = new FixedString32Bytes(greatReckoningTargetFactionId),
                GreatReckoningShare = greatReckoningShare,
            });
        }

        private static void SeedWorldPressure(
            EntityManager entityManager,
            string factionId,
            int level,
            string label,
            int score,
            bool targeted)
        {
            var entity = entityManager.CreateEntity(typeof(FactionComponent), typeof(WorldPressureComponent));
            entityManager.SetComponentData(entity, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            entityManager.SetComponentData(entity, new WorldPressureComponent
            {
                Level = level,
                Label = new FixedString32Bytes(label),
                Score = score,
                Targeted = targeted,
            });
        }

        private static bool TryGetSummaryFields(
            BloodlinesDebugCommandSurface commandSurface,
            out Dictionary<string, string> fields,
            out string readout)
        {
            fields = new Dictionary<string, string>(StringComparer.Ordinal);
            if (!commandSurface.TryDebugGetBattlefieldCommandDeckSummary("player", out readout))
            {
                return false;
            }

            var parts = readout.Split('|');
            if (parts.Length == 0 || !string.Equals(parts[0], "CommandDeckSummary", StringComparison.Ordinal))
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

                fields[parts[i].Substring(0, equalsIndex)] = parts[i].Substring(equalsIndex + 1);
            }

            return fields.Count > 0;
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

                hostObject = new GameObject("BloodlinesBattlefieldCommandDeckSmokeValidation_CommandSurface")
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
