#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Debug;
using Bloodlines.Faith;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Core;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    public static class BloodlinesFaithExposureWalkerSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-faith-exposure-walker-smoke.log";

        [MenuItem("Bloodlines/Faith/Run Faith Exposure Walker Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchFaithExposureWalkerSmokeValidation() => RunInternal(batchMode: true);

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
                message = "Faith exposure walker smoke validation errored: " + e;
            }

            string artifact = "BLOODLINES_FAITH_EXPOSURE_WALKER_SMOKE " +
                              (success ? "PASS" : "FAIL") +
                              Environment.NewLine + message;
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
            ok &= RunExposureGainPhase(lines);
            ok &= RunWayshrineAmplificationPhase(lines);
            ok &= RunConstructionAndKingdomGatePhase(lines);
            ok &= RunMultiplierCapPhase(lines);
            report = lines.ToString();
            return ok;
        }

        private static bool RunExposureGainPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("faith-exposure-phase1");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedClock(entityManager);
            SeedFaction(entityManager, "player", FactionKind.Kingdom);
            SeedSacredSite(entityManager, "site_old_light", CovenantId.OldLight, new float3(0f, 0f, 0f), 120f, 10f);
            SeedUnit(entityManager, "player", new float3(40f, 0f, 0f));

            Tick(world);

            if (!debugScope.CommandSurface.TryDebugGetSacredSiteExposureSnapshot("player", "site_old_light", out var readout))
            {
                lines.AppendLine("Phase 1 FAIL: could not read sacred-site exposure snapshot.");
                return false;
            }

            var fields = ParseFields(readout);
            if (ReadFloat(fields, "Exposure") != 10f ||
                ReadField(fields, "Discovered") != "true" ||
                ReadFloat(fields, "Multiplier") != 1f)
            {
                lines.AppendLine("Phase 1 FAIL: expected base exposure 10, discovered=true, multiplier=1. Snapshot=" + readout);
                return false;
            }

            lines.AppendLine("Phase 1 PASS: a living kingdom unit inside the sacred-site radius gained 10 exposure and marked the faith discovered.");
            return true;
        }

        private static bool RunWayshrineAmplificationPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("faith-exposure-phase2");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedClock(entityManager);
            SeedFaction(entityManager, "player", FactionKind.Kingdom);
            SeedSacredSite(entityManager, "site_order", CovenantId.TheOrder, new float3(0f, 0f, 0f), 120f, 10f);
            SeedUnit(entityManager, "player", new float3(32f, 0f, 0f));
            SeedBuilding(entityManager, "player", "wayshrine", new float3(20f, 0f, 0f), underConstruction: false);

            Tick(world);

            if (!debugScope.CommandSurface.TryDebugGetSacredSiteExposureSnapshot("player", "site_order", out var readout))
            {
                lines.AppendLine("Phase 2 FAIL: could not read amplified sacred-site exposure snapshot.");
                return false;
            }

            var fields = ParseFields(readout);
            if (ReadFloat(fields, "Exposure") != 18f ||
                ReadFloat(fields, "Multiplier") != 1.8f ||
                ReadInt(fields, "ContributorCount") != 1)
            {
                lines.AppendLine("Phase 2 FAIL: expected exposure 18, multiplier 1.8, contributorCount 1. Snapshot=" + readout);
                return false;
            }

            lines.AppendLine("Phase 2 PASS: a completed wayshrine within aura radius amplified exposure gain from 10 to 18.");
            return true;
        }

        private static bool RunConstructionAndKingdomGatePhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("faith-exposure-phase3");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            var playerFaction = SeedFaction(entityManager, "player", FactionKind.Kingdom);
            var tribesFaction = SeedFaction(entityManager, "tribes", FactionKind.Tribes);
            SeedClock(entityManager);
            SeedSacredSite(entityManager, "site_wild", CovenantId.TheWild, new float3(0f, 0f, 0f), 120f, 10f);
            SeedUnit(entityManager, "player", new float3(32f, 0f, 0f));
            SeedUnit(entityManager, "tribes", new float3(16f, 0f, 0f));
            SeedBuilding(entityManager, "player", "wayshrine", new float3(12f, 0f, 0f), underConstruction: true);

            Tick(world);

            if (!debugScope.CommandSurface.TryDebugGetSacredSiteExposureSnapshot("player", "site_wild", out var readout))
            {
                lines.AppendLine("Phase 3 FAIL: could not read sacred-site snapshot with under-construction building.");
                return false;
            }

            var fields = ParseFields(readout);
            var tribesExposure = FaithScoring.GetExposure(
                entityManager.GetBuffer<FaithExposureElement>(tribesFaction),
                CovenantId.TheWild);
            var playerExposure = FaithScoring.GetExposure(
                entityManager.GetBuffer<FaithExposureElement>(playerFaction),
                CovenantId.TheWild);

            if (playerExposure != 10f ||
                ReadFloat(fields, "Multiplier") != 1f ||
                ReadInt(fields, "ContributorCount") != 0 ||
                tribesExposure != 0f)
            {
                lines.AppendLine("Phase 3 FAIL: under-construction shrine should not amplify and non-kingdom tribes should gain no exposure. Snapshot=" + readout);
                return false;
            }

            lines.AppendLine("Phase 3 PASS: under-construction shrines were ignored and non-kingdom factions stayed ineligible for sacred-site exposure.");
            return true;
        }

        private static bool RunMultiplierCapPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("faith-exposure-phase4");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedClock(entityManager);
            SeedFaction(entityManager, "player", FactionKind.Kingdom);
            SeedSacredSite(entityManager, "site_blood", CovenantId.BloodDominion, new float3(0f, 0f, 0f), 120f, 10f);
            SeedUnit(entityManager, "player", new float3(18f, 0f, 0f));
            SeedBuilding(entityManager, "player", "wayshrine", new float3(10f, 0f, 0f), underConstruction: false);
            SeedBuilding(entityManager, "player", "covenant_hall", new float3(16f, 0f, 0f), underConstruction: false);
            SeedBuilding(entityManager, "player", "grand_sanctuary", new float3(22f, 0f, 0f), underConstruction: false);
            SeedBuilding(entityManager, "player", "covenant_hall", new float3(28f, 0f, 0f), underConstruction: false);
            SeedBuilding(entityManager, "player", "wayshrine", new float3(34f, 0f, 0f), underConstruction: false);

            Tick(world);

            if (!debugScope.CommandSurface.TryDebugGetSacredSiteExposureSnapshot("player", "site_blood", out var readout))
            {
                lines.AppendLine("Phase 4 FAIL: could not read capped sacred-site exposure snapshot.");
                return false;
            }

            var fields = ParseFields(readout);
            if (ReadFloat(fields, "Exposure") != 40f ||
                ReadFloat(fields, "Multiplier") != 4f ||
                ReadInt(fields, "ContributorCount") != 4)
            {
                lines.AppendLine("Phase 4 FAIL: expected exposure 40, multiplier 4, contributorCount 4. Snapshot=" + readout);
                return false;
            }

            lines.AppendLine("Phase 4 PASS: stacked shrine/hall/sanctuary coverage clamped at the canonical 4x ceiling and max 4 contributors.");
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<FaithExposureWalkerSystem>());
            simulationGroup.SortSystems();
            return world;
        }

        private static void SeedClock(EntityManager entityManager)
        {
            var entity = entityManager.CreateEntity(typeof(DualClockComponent));
            entityManager.SetComponentData(entity, new DualClockComponent
            {
                InWorldDays = 0f,
                DaysPerRealSecond = 2f,
                DeclarationCount = 0,
            });
        }

        private static Entity SeedFaction(
            EntityManager entityManager,
            string factionId,
            FactionKind kind)
        {
            var entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(FaithStateComponent));
            entityManager.SetComponentData(entity, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            entityManager.SetComponentData(entity, new FactionKindComponent
            {
                Kind = kind,
            });
            entityManager.SetComponentData(entity, new FaithStateComponent
            {
                SelectedFaith = CovenantId.None,
                DoctrinePath = DoctrinePath.Unassigned,
                Intensity = 0f,
                Level = 0,
            });
            entityManager.AddBuffer<FaithExposureElement>(entity);
            return entity;
        }

        private static void SeedSacredSite(
            EntityManager entityManager,
            string siteId,
            CovenantId faith,
            float3 position,
            float radiusWorldUnits,
            float exposureRate)
        {
            var entity = entityManager.CreateEntity(
                typeof(SacredSiteExposureSourceComponent),
                typeof(PositionComponent));
            entityManager.SetComponentData(entity, new SacredSiteExposureSourceComponent
            {
                SiteId = new FixedString64Bytes(siteId),
                Faith = faith,
                RadiusWorldUnits = radiusWorldUnits,
                ExposureRate = exposureRate,
            });
            entityManager.SetComponentData(entity, new PositionComponent
            {
                Value = position,
            });
        }

        private static void SeedUnit(
            EntityManager entityManager,
            string factionId,
            float3 position)
        {
            var entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(PositionComponent),
                typeof(HealthComponent),
                typeof(UnitTypeComponent));
            entityManager.SetComponentData(entity, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            entityManager.SetComponentData(entity, new PositionComponent
            {
                Value = position,
            });
            entityManager.SetComponentData(entity, new HealthComponent
            {
                Current = 10f,
                Max = 10f,
            });
            entityManager.SetComponentData(entity, new UnitTypeComponent
            {
                TypeId = new FixedString64Bytes("militia"),
                Role = UnitRole.Melee,
                SiegeClass = SiegeClass.None,
                PopulationCost = 1,
                Stage = 2,
            });
        }

        private static void SeedBuilding(
            EntityManager entityManager,
            string factionId,
            string buildingTypeId,
            float3 position,
            bool underConstruction)
        {
            var entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(PositionComponent),
                typeof(HealthComponent),
                typeof(BuildingTypeComponent));
            entityManager.SetComponentData(entity, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            entityManager.SetComponentData(entity, new PositionComponent
            {
                Value = position,
            });
            entityManager.SetComponentData(entity, new HealthComponent
            {
                Current = 20f,
                Max = 20f,
            });
            entityManager.SetComponentData(entity, new BuildingTypeComponent
            {
                TypeId = new FixedString64Bytes(buildingTypeId),
                FortificationRole = FortificationRole.None,
                StructuralDamageMultiplier = 1f,
                PopulationCapBonus = 0,
                BlocksPassage = false,
                SupportsSiegePreparation = false,
                SupportsSiegeLogistics = false,
            });

            if (underConstruction)
            {
                entityManager.AddComponentData(entity, new ConstructionStateComponent
                {
                    RemainingSeconds = 10f,
                    TotalSeconds = 10f,
                    StartingHealth = 5f,
                });
            }
        }

        private static void Tick(World world)
        {
            world.SetTime(new TimeData(0d, 1f));
            world.Update();
        }

        private static Dictionary<string, string> ParseFields(string readout)
        {
            var fields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var parts = readout.Split('|');
            for (int i = 1; i < parts.Length; i++)
            {
                int separatorIndex = parts[i].IndexOf('=');
                if (separatorIndex <= 0)
                {
                    continue;
                }

                fields[parts[i][..separatorIndex]] = parts[i][(separatorIndex + 1)..];
            }

            return fields;
        }

        private static string ReadField(Dictionary<string, string> fields, string key)
        {
            return fields.TryGetValue(key, out var value) ? value : string.Empty;
        }

        private static float ReadFloat(Dictionary<string, string> fields, string key)
        {
            if (!fields.TryGetValue(key, out var value))
            {
                return float.NaN;
            }

            return float.Parse(value, CultureInfo.InvariantCulture);
        }

        private static int ReadInt(Dictionary<string, string> fields, string key)
        {
            if (!fields.TryGetValue(key, out var value))
            {
                return int.MinValue;
            }

            return int.Parse(value, CultureInfo.InvariantCulture);
        }

        private sealed class DebugCommandSurfaceScope : IDisposable
        {
            private readonly World previousDefaultWorld;
            private readonly GameObject hostObject;

            public DebugCommandSurfaceScope(World world)
            {
                previousDefaultWorld = World.DefaultGameObjectInjectionWorld;
                World.DefaultGameObjectInjectionWorld = world;
                hostObject = new GameObject("BloodlinesFaithExposureWalkerSmoke_CommandSurface")
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
