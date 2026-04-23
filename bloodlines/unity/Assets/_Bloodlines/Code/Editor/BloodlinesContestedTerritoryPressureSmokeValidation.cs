#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Debug;
using Bloodlines.GameTime;
using Bloodlines.Systems;
using Unity.Collections;
using Unity.Core;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Dedicated smoke validator for contested-territory pressure.
    /// Phase 1: two hostile factions inside a claim radius materialize a contested state.
    /// Phase 2: the contested point loses loyalty while an uncontested peer continues stabilizing.
    /// Phase 3: contested state clears once hostile units leave the claim radius.
    /// </summary>
    public static class BloodlinesContestedTerritoryPressureSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-contested-territory-pressure-smoke.log";

        [MenuItem("Bloodlines/WorldPressure/Run Contested Territory Pressure Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchContestedTerritoryPressureSmokeValidation() =>
            RunInternal(batchMode: true);

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
                message = "Contested territory pressure smoke errored: " + exception;
            }

            string artifact =
                "BLOODLINES_CONTESTED_TERRITORY_PRESSURE_SMOKE " +
                (success ? "PASS" : "FAIL") +
                Environment.NewLine +
                message;
            UnityDebug.Log(artifact);

            try
            {
                string fullPath = Path.GetFullPath(Path.Combine(Application.dataPath, ArtifactPath));
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
                File.WriteAllText(fullPath, artifact);
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
            ok &= RunDetectionPhase(lines);
            ok &= RunStabilityPhase(lines);
            ok &= RunClearPhase(lines);
            report = lines.ToString();
            return ok;
        }

        private static bool RunDetectionPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("contested-territory-detection");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 180f);
            Entity player = CreateFaction(entityManager, "player");
            CreateFaction(entityManager, "enemy");
            CreateFaction(entityManager, "rival");
            CreateFaction(entityManager, "ally");
            SetHostility(entityManager, player, "enemy", "rival");

            Entity controlPoint = CreateControlPoint(
                entityManager,
                "frontier",
                "player",
                80f,
                new float3(0f, 0f, 0f),
                radiusTiles: 6f);
            CreateCombatUnit(entityManager, "enemy", new float3(2f, 0f, 0f));
            CreateCombatUnit(entityManager, "rival", new float3(-2f, 0f, 1f));
            CreateCombatUnit(entityManager, "ally", new float3(1f, 0f, -1f));

            Tick(world, elapsedSeconds: 1d, deltaSeconds: 1f);

            if (!entityManager.HasComponent<ContestedTerritoryComponent>(controlPoint))
            {
                lines.AppendLine("Phase 1 FAIL: contested component was not created for a flanked frontier point.");
                return false;
            }

            var contested = entityManager.GetComponentData<ContestedTerritoryComponent>(controlPoint);
            if (contested.ContestingFactionCount != 2 ||
                contested.StabilityPenaltyPerDay <= 0f ||
                contested.LoyaltyVolatilityMultiplier <= 1f)
            {
                lines.AppendLine(
                    "Phase 1 FAIL: contested metrics were not populated correctly. " +
                    $"count={contested.ContestingFactionCount}, penalty={contested.StabilityPenaltyPerDay:0.00}, volatility={contested.LoyaltyVolatilityMultiplier:0.00}.");
                return false;
            }

            if (!debugScope.CommandSurface.TryDebugGetContestState("frontier", out var readout))
            {
                lines.AppendLine("Phase 1 FAIL: debug contest readout was unavailable.");
                return false;
            }

            string readoutText = readout.ToString();
            if (!readoutText.Contains("ControlPointId=frontier", StringComparison.Ordinal) ||
                !readoutText.Contains("ContestingFactionCount=2", StringComparison.Ordinal))
            {
                lines.AppendLine("Phase 1 FAIL: debug contest readout missed the expected frontier fields.");
                return false;
            }

            lines.AppendLine(
                "Phase 1 PASS: contested state materialized with count=" +
                contested.ContestingFactionCount +
                ", penalty=" +
                contested.StabilityPenaltyPerDay.ToString("0.00") +
                ", volatility=" +
                contested.LoyaltyVolatilityMultiplier.ToString("0.00") +
                ".");
            return true;
        }

        private static bool RunStabilityPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("contested-territory-stability");
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 240f);
            Entity player = CreateFaction(entityManager, "player");
            CreateFaction(entityManager, "enemy");
            CreateFaction(entityManager, "rival");
            SetHostility(entityManager, player, "enemy", "rival");

            Entity contestedPoint = CreateControlPoint(
                entityManager,
                "contested",
                "player",
                80f,
                new float3(0f, 0f, 0f),
                radiusTiles: 6f);
            Entity stablePoint = CreateControlPoint(
                entityManager,
                "stable",
                "player",
                80f,
                new float3(24f, 0f, 0f),
                radiusTiles: 6f);
            CreateCombatUnit(entityManager, "enemy", new float3(2f, 0f, 0f));
            CreateCombatUnit(entityManager, "rival", new float3(-2f, 0f, 0f));

            Tick(world, elapsedSeconds: 1d, deltaSeconds: 1f);

            float contestedLoyalty = entityManager.GetComponentData<ControlPointComponent>(contestedPoint).Loyalty;
            float stableLoyalty = entityManager.GetComponentData<ControlPointComponent>(stablePoint).Loyalty;
            if (!entityManager.HasComponent<ContestedTerritoryComponent>(contestedPoint) ||
                entityManager.HasComponent<ContestedTerritoryComponent>(stablePoint) ||
                contestedLoyalty >= stableLoyalty ||
                stableLoyalty <= 80f)
            {
                lines.AppendLine(
                    "Phase 2 FAIL: contested frontier pressure did not degrade loyalty against a stabilizing peer. " +
                    $"contested={contestedLoyalty:0.00}, stable={stableLoyalty:0.00}.");
                return false;
            }

            lines.AppendLine(
                "Phase 2 PASS: contested loyalty=" +
                contestedLoyalty.ToString("0.00") +
                " while stable peer rose to " +
                stableLoyalty.ToString("0.00") +
                ".");
            return true;
        }

        private static bool RunClearPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("contested-territory-clear");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 300f);
            Entity player = CreateFaction(entityManager, "player");
            CreateFaction(entityManager, "enemy");
            CreateFaction(entityManager, "rival");
            SetHostility(entityManager, player, "enemy", "rival");

            Entity controlPoint = CreateControlPoint(
                entityManager,
                "frontier",
                "player",
                80f,
                new float3(0f, 0f, 0f),
                radiusTiles: 6f);
            Entity enemyUnit = CreateCombatUnit(entityManager, "enemy", new float3(2f, 0f, 0f));
            Entity rivalUnit = CreateCombatUnit(entityManager, "rival", new float3(-2f, 0f, 0f));

            Tick(world, elapsedSeconds: 1d, deltaSeconds: 1f);

            if (!entityManager.HasComponent<ContestedTerritoryComponent>(controlPoint))
            {
                lines.AppendLine("Phase 3 FAIL: control point never entered the contested state.");
                return false;
            }

            MoveUnit(entityManager, enemyUnit, new float3(30f, 0f, 0f));
            MoveUnit(entityManager, rivalUnit, new float3(-30f, 0f, 0f));
            Tick(world, elapsedSeconds: 2d, deltaSeconds: 1f);

            if (entityManager.HasComponent<ContestedTerritoryComponent>(controlPoint) ||
                debugScope.CommandSurface.TryDebugGetContestState("frontier", out _))
            {
                lines.AppendLine("Phase 3 FAIL: contested state did not clear after hostile withdrawal.");
                return false;
            }

            lines.AppendLine("Phase 3 PASS: contested state cleared after hostile units withdrew.");
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<ContestedTerritoryEvaluationSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<ControlPointCaptureSystem>());
            simulationGroup.SortSystems();
            return world;
        }

        private static void SeedDualClock(EntityManager entityManager, float inWorldDays)
        {
            Entity clock = entityManager.CreateEntity(typeof(DualClockComponent));
            entityManager.SetComponentData(clock, new DualClockComponent
            {
                InWorldDays = inWorldDays,
                DaysPerRealSecond = 2f,
                DeclarationCount = 0,
            });
        }

        private static Entity CreateFaction(EntityManager entityManager, string factionId)
        {
            Entity faction = entityManager.CreateEntity(typeof(FactionComponent));
            entityManager.SetComponentData(faction, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            entityManager.AddBuffer<HostilityComponent>(faction);
            return faction;
        }

        private static void SetHostility(EntityManager entityManager, Entity factionEntity, params string[] hostileFactionIds)
        {
            DynamicBuffer<HostilityComponent> buffer = entityManager.GetBuffer<HostilityComponent>(factionEntity);
            buffer.Clear();
            for (int i = 0; i < hostileFactionIds.Length; i++)
            {
                buffer.Add(new HostilityComponent
                {
                    HostileFactionId = new FixedString32Bytes(hostileFactionIds[i]),
                });
            }
        }

        private static Entity CreateControlPoint(
            EntityManager entityManager,
            string controlPointId,
            string ownerFactionId,
            float loyalty,
            float3 position,
            float radiusTiles)
        {
            Entity controlPoint = entityManager.CreateEntity(
                typeof(ControlPointComponent),
                typeof(PositionComponent));
            entityManager.SetComponentData(controlPoint, new ControlPointComponent
            {
                ControlPointId = new FixedString32Bytes(controlPointId),
                OwnerFactionId = new FixedString32Bytes(ownerFactionId),
                CaptureFactionId = default,
                ContinentId = new FixedString32Bytes("frontier"),
                ControlState = ControlState.Stabilized,
                IsContested = false,
                Loyalty = loyalty,
                CaptureProgress = 0f,
                SettlementClassId = new FixedString32Bytes("border_settlement"),
                FortificationTier = 1,
                RadiusTiles = radiusTiles,
                CaptureTimeSeconds = 9f,
                GoldTrickle = 1f,
                FoodTrickle = 1f,
                WaterTrickle = 1f,
                WoodTrickle = 0f,
                StoneTrickle = 0f,
                IronTrickle = 0f,
                InfluenceTrickle = 0.2f,
            });
            entityManager.SetComponentData(controlPoint, new PositionComponent
            {
                Value = position,
            });
            return controlPoint;
        }

        private static Entity CreateCombatUnit(EntityManager entityManager, string factionId, float3 position)
        {
            Entity unit = entityManager.CreateEntity(
                typeof(UnitTypeComponent),
                typeof(PositionComponent),
                typeof(FactionComponent),
                typeof(HealthComponent));
            entityManager.SetComponentData(unit, new UnitTypeComponent
            {
                TypeId = new FixedString64Bytes("swordsman"),
                Role = UnitRole.Melee,
                SiegeClass = SiegeClass.None,
                PopulationCost = 1,
                Stage = 1,
            });
            entityManager.SetComponentData(unit, new PositionComponent
            {
                Value = position,
            });
            entityManager.SetComponentData(unit, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            entityManager.SetComponentData(unit, new HealthComponent
            {
                Current = 10f,
                Max = 10f,
            });
            return unit;
        }

        private static void MoveUnit(EntityManager entityManager, Entity unit, float3 position)
        {
            entityManager.SetComponentData(unit, new PositionComponent
            {
                Value = position,
            });
        }

        private static void Tick(World world, double elapsedSeconds, float deltaSeconds)
        {
            var entityManager = world.EntityManager;
            using var query = entityManager.CreateEntityQuery(ComponentType.ReadWrite<DualClockComponent>());
            var dualClock = query.GetSingleton<DualClockComponent>();
            dualClock.InWorldDays += dualClock.DaysPerRealSecond * deltaSeconds;
            query.SetSingleton(dualClock);

            world.SetTime(new TimeData(elapsedSeconds, deltaSeconds));
            world.Update();
        }
    }
}
#endif
