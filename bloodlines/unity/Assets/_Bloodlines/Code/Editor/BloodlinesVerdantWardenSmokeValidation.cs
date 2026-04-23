#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Debug;
using Bloodlines.Faith;
using Bloodlines.Fortification;
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
    public static class BloodlinesVerdantWardenSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-verdant-warden-smoke.log";

        [MenuItem("Bloodlines/Faith/Run Verdant Warden Smoke Validation")]
        public static void RunInteractive()
        {
            RunInternal(batchMode: false);
        }

        public static void RunBatchVerdantWardenSmokeValidation()
        {
            RunInternal(batchMode: true);
        }

        private static void RunInternal(bool batchMode)
        {
            string details;
            bool success;
            try
            {
                success = RunAllPhases(out details);
            }
            catch (Exception ex)
            {
                success = false;
                details = "Verdant Warden smoke errored: " + ex;
            }

            WriteResult(batchMode, success, details);
        }

        private static bool RunAllPhases(out string details)
        {
            if (!RunCoveragePhase(out string coverageDetails))
            {
                details = coverageDetails;
                return false;
            }

            if (!RunStackCapPhase(out string stackCapDetails))
            {
                details = stackCapDetails;
                return false;
            }

            details = coverageDetails + "; " + stackCapDetails;
            return true;
        }

        private static bool RunCoveragePhase(out string details)
        {
            using var world = CreateValidationWorld(
                "BloodlinesVerdantWarden_Coverage",
                includeCapture: true,
                includeReserve: true,
                includeAttack: false);
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            CreateClock(entityManager);
            CreateMapConfig(entityManager);
            CreateFaction(entityManager, "player");

            Entity coveredControlPoint = CreateControlPoint(
                entityManager,
                "cp_covered",
                "player",
                new float3(0f, 0f, 0f));
            Entity uncoveredControlPoint = CreateControlPoint(
                entityManager,
                "cp_uncovered",
                "player",
                new float3(320f, 0f, 0f));

            CreateClaimUnit(entityManager, "player", new float3(0f, 0f, 0f));
            CreateClaimUnit(entityManager, "player", new float3(320f, 0f, 0f));
            CreateVerdantWarden(entityManager, "player", new float3(50f, 0f, 0f));

            Entity coveredSettlement = CreateSettlement(
                entityManager,
                "covered_keep",
                "player",
                new float3(0f, 0f, 60f),
                threatActive: false);
            Entity uncoveredSettlement = CreateSettlement(
                entityManager,
                "uncovered_keep",
                "player",
                new float3(320f, 0f, 60f),
                threatActive: false);

            Entity coveredReserve = CreateReserveDefender(
                entityManager,
                coveredSettlement,
                "covered_keep",
                "player",
                new float3(0.8f, 0f, 60f),
                currentHealth: 1f,
                maxHealth: 10f,
                ReserveDutyState.Fallback);
            Entity uncoveredReserve = CreateReserveDefender(
                entityManager,
                uncoveredSettlement,
                "uncovered_keep",
                "player",
                new float3(320.8f, 0f, 60f),
                currentHealth: 1f,
                maxHealth: 10f,
                ReserveDutyState.Fallback);

            Tick(world, 1f);

            var coveredControl = entityManager.GetComponentData<ControlPointComponent>(coveredControlPoint);
            var uncoveredControl = entityManager.GetComponentData<ControlPointComponent>(uncoveredControlPoint);
            float coveredReserveHealth = entityManager.GetComponentData<HealthComponent>(coveredReserve).Current;
            float uncoveredReserveHealth = entityManager.GetComponentData<HealthComponent>(uncoveredReserve).Current;

            if (coveredControl.VerdantWardenCount != 1 ||
                coveredControl.VerdantWardenCappedCount != 1 ||
                coveredControl.Loyalty <= uncoveredControl.Loyalty + 0.25f ||
                coveredReserveHealth <= uncoveredReserveHealth + 0.05f)
            {
                details =
                    "Coverage phase failed: live support did not project stronger control-point loyalty and reserve healing. " +
                    "coveredLoyalty=" + coveredControl.Loyalty.ToString("0.000") +
                    ", uncoveredLoyalty=" + uncoveredControl.Loyalty.ToString("0.000") +
                    ", coveredReserveHealth=" + coveredReserveHealth.ToString("0.000") +
                    ", uncoveredReserveHealth=" + uncoveredReserveHealth.ToString("0.000") +
                    ", coveredCount=" + coveredControl.VerdantWardenCount +
                    ", coveredCapped=" + coveredControl.VerdantWardenCappedCount + ".";
                return false;
            }

            if (!debugScope.CommandSurface.TryDebugGetVerdantWardenCoverage("cp_covered", out string readout) ||
                !readout.Contains("|Count=1|", StringComparison.Ordinal) ||
                !readout.Contains("|CappedCount=1|", StringComparison.Ordinal))
            {
                details =
                    "Coverage phase failed: debug coverage readout was unavailable or missing the expected single-stack state. " +
                    "readout=" + readout + ".";
                return false;
            }

            details =
                "coveragePhase: coveredLoyalty=" + coveredControl.Loyalty.ToString("0.000") +
                ", uncoveredLoyalty=" + uncoveredControl.Loyalty.ToString("0.000") +
                ", coveredReserveHealth=" + coveredReserveHealth.ToString("0.000") +
                ", uncoveredReserveHealth=" + uncoveredReserveHealth.ToString("0.000") + ".";
            return true;
        }

        private static bool RunStackCapPhase(out string details)
        {
            using var world = CreateValidationWorld(
                "BloodlinesVerdantWarden_StackCap",
                includeCapture: false,
                includeReserve: false,
                includeAttack: true);
            var entityManager = world.EntityManager;

            CreateClock(entityManager);
            CreateMapConfig(entityManager);
            CreateFaction(entityManager, "player", hostileTo: "enemy");
            CreateFaction(entityManager, "enemy", hostileTo: "player");

            Entity cappedControlPoint = CreateControlPoint(
                entityManager,
                "cp_cap",
                "player",
                new float3(0f, 0f, 0f));
            Entity cappedSettlement = CreateSettlement(
                entityManager,
                "cap_keep",
                "player",
                new float3(0f, 0f, 0f),
                threatActive: false);
            Entity baselineSettlement = CreateSettlement(
                entityManager,
                "steady_keep",
                "player",
                new float3(400f, 0f, 0f),
                threatActive: false);

            CreateVerdantWarden(entityManager, "player", new float3(30f, 0f, 0f));
            CreateVerdantWarden(entityManager, "player", new float3(60f, 0f, 0f));
            CreateVerdantWarden(entityManager, "player", new float3(90f, 0f, 0f));
            CreateVerdantWarden(entityManager, "player", new float3(120f, 0f, 0f));

            Entity cappedTarget = CreateCombatTarget(entityManager, "enemy", new float3(5f, 0f, 0f), 40f);
            Entity baselineTarget = CreateCombatTarget(entityManager, "enemy", new float3(405f, 0f, 0f), 40f);

            CreateFrontlineDefender(
                entityManager,
                cappedSettlement,
                "cap_keep",
                "player",
                new float3(1f, 0f, 0f),
                cappedTarget);
            CreateFrontlineDefender(
                entityManager,
                baselineSettlement,
                "steady_keep",
                "player",
                new float3(401f, 0f, 0f),
                baselineTarget);

            Tick(world, 0.1f);

            var controlPoint = entityManager.GetComponentData<ControlPointComponent>(cappedControlPoint);
            var fortification = entityManager.GetComponentData<FortificationComponent>(cappedSettlement);
            float cappedDamage = 40f - entityManager.GetComponentData<HealthComponent>(cappedTarget).Current;
            float baselineDamage = 40f - entityManager.GetComponentData<HealthComponent>(baselineTarget).Current;

            if (controlPoint.VerdantWardenCount != 4 ||
                controlPoint.VerdantWardenCappedCount != 3 ||
                fortification.VerdantWardenCount != 4 ||
                fortification.VerdantWardenCappedCount != 3 ||
                math.abs(fortification.VerdantWardenDefenderAttackMultiplier - 1.18f) > 0.0001f ||
                math.abs(fortification.VerdantWardenReserveHealMultiplier - 1.24f) > 0.0001f ||
                math.abs(fortification.VerdantWardenReserveMusterMultiplier - 1.30f) > 0.0001f ||
                math.abs(cappedDamage - 11.8f) > 0.05f ||
                math.abs(baselineDamage - 10f) > 0.05f)
            {
                details =
                    "Stack-cap phase failed: stored coverage or frontline damage exceeded the canonical three-stack cap. " +
                    "controlCount=" + controlPoint.VerdantWardenCount +
                    ", controlCapped=" + controlPoint.VerdantWardenCappedCount +
                    ", fortCount=" + fortification.VerdantWardenCount +
                    ", fortCapped=" + fortification.VerdantWardenCappedCount +
                    ", attackMultiplier=" + fortification.VerdantWardenDefenderAttackMultiplier.ToString("0.000") +
                    ", healMultiplier=" + fortification.VerdantWardenReserveHealMultiplier.ToString("0.000") +
                    ", musterMultiplier=" + fortification.VerdantWardenReserveMusterMultiplier.ToString("0.000") +
                    ", cappedDamage=" + cappedDamage.ToString("0.000") +
                    ", baselineDamage=" + baselineDamage.ToString("0.000") + ".";
                return false;
            }

            details =
                "stackCapPhase: controlCount=" + controlPoint.VerdantWardenCount +
                ", controlCapped=" + controlPoint.VerdantWardenCappedCount +
                ", attackMultiplier=" + fortification.VerdantWardenDefenderAttackMultiplier.ToString("0.000") +
                ", cappedDamage=" + cappedDamage.ToString("0.000") +
                ", baselineDamage=" + baselineDamage.ToString("0.000") + ".";
            return true;
        }

        private static World CreateValidationWorld(
            string worldName,
            bool includeCapture,
            bool includeReserve,
            bool includeAttack)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();

            if (includeAttack)
            {
                var endSimulation = world.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
                simulationGroup.AddSystemToUpdateList(endSimulation);
            }

            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<VerdantWardenSupportSystem>());

            if (includeCapture)
            {
                simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<ControlPointCaptureSystem>());
            }

            if (includeReserve)
            {
                simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<FortificationReserveSystem>());
            }

            if (includeAttack)
            {
                simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<AttackResolutionSystem>());
            }

            simulationGroup.SortSystems();
            return world;
        }

        private static void Tick(World world, float deltaTime)
        {
            if (deltaTime <= 0f)
            {
                return;
            }

            double nextElapsed = world.Time.ElapsedTime + deltaTime;
            world.SetTime(new TimeData(nextElapsed, deltaTime));
            world.Update();
        }

        private static void CreateClock(EntityManager entityManager)
        {
            Entity entity = entityManager.CreateEntity(typeof(DualClockComponent));
            entityManager.SetComponentData(entity, new DualClockComponent
            {
                InWorldDays = 12f,
                DaysPerRealSecond = 2f,
                DeclarationCount = 0,
            });
        }

        private static void CreateMapConfig(EntityManager entityManager)
        {
            Entity entity = entityManager.CreateEntity(typeof(MapBootstrapConfigComponent));
            entityManager.SetComponentData(entity, new MapBootstrapConfigComponent
            {
                MapId = new FixedString64Bytes("verdant_smoke"),
                MapDisplayName = new FixedString128Bytes("Verdant Smoke"),
                TileSize = 32,
                Width = 128,
                Height = 128,
                SpawnFactions = false,
                SpawnBuildings = false,
                SpawnUnits = false,
                SpawnResourceNodes = false,
                SpawnControlPoints = false,
                SpawnSettlements = false,
            });
        }

        private static Entity CreateFaction(
            EntityManager entityManager,
            string factionId,
            string hostileTo = null)
        {
            Entity entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(HostilityComponent));

            entityManager.SetComponentData(entity, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });

            if (!string.IsNullOrWhiteSpace(hostileTo))
            {
                var hostilityBuffer = entityManager.GetBuffer<HostilityComponent>(entity);
                hostilityBuffer.Add(new HostilityComponent
                {
                    HostileFactionId = new FixedString32Bytes(hostileTo),
                });
            }

            return entity;
        }

        private static Entity CreateControlPoint(
            EntityManager entityManager,
            string controlPointId,
            string ownerFactionId,
            float3 position)
        {
            Entity entity = entityManager.CreateEntity(
                typeof(ControlPointComponent),
                typeof(PositionComponent));

            entityManager.SetComponentData(entity, new ControlPointComponent
            {
                ControlPointId = new FixedString32Bytes(controlPointId),
                OwnerFactionId = new FixedString32Bytes(ownerFactionId),
                CaptureFactionId = default,
                ContinentId = new FixedString32Bytes("heartlands"),
                ControlState = ControlState.Occupied,
                IsContested = false,
                Loyalty = 40f,
                CaptureProgress = 0f,
                SettlementClassId = new FixedString32Bytes("trade_town"),
                FortificationTier = 1,
                RadiusTiles = 6f,
                CaptureTimeSeconds = 5f,
            });
            entityManager.SetComponentData(entity, new PositionComponent
            {
                Value = position,
            });

            return entity;
        }

        private static Entity CreateSettlement(
            EntityManager entityManager,
            string settlementId,
            string factionId,
            float3 position,
            bool threatActive)
        {
            Entity entity = entityManager.CreateEntity(
                typeof(SettlementComponent),
                typeof(FactionComponent),
                typeof(FortificationComponent),
                typeof(FortificationReserveComponent),
                typeof(PositionComponent));

            entityManager.SetComponentData(entity, new SettlementComponent
            {
                SettlementId = new FixedString64Bytes(settlementId),
                SettlementClassId = new FixedString32Bytes("trade_town"),
                FortificationTier = 1,
                FortificationCeiling = 3,
            });
            entityManager.SetComponentData(entity, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            entityManager.SetComponentData(entity, new PositionComponent
            {
                Value = position,
            });
            entityManager.SetComponentData(entity, new FortificationComponent
            {
                SettlementId = new FixedString64Bytes(settlementId),
                Tier = 1,
                Ceiling = 3,
                ThreatRadiusTiles = FortificationCanon.ThreatRadiusTiles,
                ReserveRadiusTiles = FortificationCanon.ReserveRadiusTiles,
                KeepPresenceRadiusTiles = FortificationCanon.KeepPresenceRadiusTiles,
                FaithWardId = new FixedString32Bytes("unwarded"),
                FaithWardDefenderAttackMultiplier = 1f,
                FaithWardReserveHealMultiplier = 1f,
                FaithWardReserveMusterMultiplier = 1f,
                FaithWardLoyaltyProtectionMultiplier = 1f,
                FaithWardEnemySpeedMultiplier = 1f,
                FaithWardSurgeActive = false,
            });
            entityManager.SetComponentData(entity, new FortificationReserveComponent
            {
                MusterIntervalSeconds = FortificationCanon.ReserveMusterIntervalSeconds,
                ReserveHealPerSecond = FortificationCanon.ReserveTriageHealPerSecond,
                RetreatHealthRatio = FortificationCanon.ReserveRetreatHealthRatio,
                RecoveryHealthRatio = FortificationCanon.ReserveRecoveryHealthRatio,
                TriageRadiusTiles = FortificationCanon.TriageRadiusTiles,
                LastCommitAt = -999d,
                ThreatActive = threatActive,
                ReadyReserveCount = 1,
            });

            return entity;
        }

        private static Entity CreateClaimUnit(
            EntityManager entityManager,
            string factionId,
            float3 position)
        {
            Entity entity = entityManager.CreateEntity(
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

            return entity;
        }

        private static Entity CreateVerdantWarden(
            EntityManager entityManager,
            string factionId,
            float3 position)
        {
            Entity entity = entityManager.CreateEntity(
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
                Current = 12f,
                Max = 12f,
            });
            entityManager.SetComponentData(entity, new UnitTypeComponent
            {
                TypeId = VerdantWardenRules.UnitTypeId,
                Role = UnitRole.Support,
                SiegeClass = SiegeClass.None,
                PopulationCost = 1,
                Stage = 2,
            });

            return entity;
        }

        private static Entity CreateReserveDefender(
            EntityManager entityManager,
            Entity settlementEntity,
            string settlementId,
            string factionId,
            float3 position,
            float currentHealth,
            float maxHealth,
            ReserveDutyState duty)
        {
            Entity entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(PositionComponent),
                typeof(HealthComponent),
                typeof(FortificationCombatantTag),
                typeof(FortificationSettlementLinkComponent),
                typeof(FortificationReserveAssignmentComponent));

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
                Current = currentHealth,
                Max = maxHealth,
            });
            entityManager.SetComponentData(entity, new FortificationSettlementLinkComponent
            {
                SettlementEntity = settlementEntity,
                SettlementId = new FixedString64Bytes(settlementId),
            });
            entityManager.SetComponentData(entity, new FortificationReserveAssignmentComponent
            {
                Duty = duty,
            });

            return entity;
        }

        private static Entity CreateFrontlineDefender(
            EntityManager entityManager,
            Entity settlementEntity,
            string settlementId,
            string factionId,
            float3 position,
            Entity target)
        {
            Entity entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(PositionComponent),
                typeof(HealthComponent),
                typeof(UnitTypeComponent),
                typeof(CombatStatsComponent),
                typeof(MoveCommandComponent),
                typeof(AttackTargetComponent),
                typeof(FortificationCombatantTag),
                typeof(FortificationSettlementLinkComponent),
                typeof(FortificationReserveAssignmentComponent));

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
            entityManager.SetComponentData(entity, new UnitTypeComponent
            {
                TypeId = new FixedString64Bytes("militia"),
                Role = UnitRole.Melee,
                SiegeClass = SiegeClass.None,
                PopulationCost = 1,
                Stage = 2,
            });
            entityManager.SetComponentData(entity, new CombatStatsComponent
            {
                AttackDamage = 10f,
                AttackRange = 8f,
                AttackCooldown = 0.5f,
                Sight = 20f,
                CooldownRemaining = 0f,
                TargetAcquireIntervalSeconds = 0.25f,
                TargetSightGraceSeconds = 0.35f,
            });
            entityManager.SetComponentData(entity, new MoveCommandComponent
            {
                Destination = position,
                StoppingDistance = 0.5f,
                IsActive = false,
            });
            entityManager.SetComponentData(entity, new AttackTargetComponent
            {
                TargetEntity = target,
                EngagementRange = 8f,
            });
            entityManager.SetComponentData(entity, new FortificationSettlementLinkComponent
            {
                SettlementEntity = settlementEntity,
                SettlementId = new FixedString64Bytes(settlementId),
            });
            entityManager.SetComponentData(entity, new FortificationReserveAssignmentComponent
            {
                Duty = ReserveDutyState.Engaged,
            });

            return entity;
        }

        private static Entity CreateCombatTarget(
            EntityManager entityManager,
            string factionId,
            float3 position,
            float health)
        {
            Entity entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(PositionComponent),
                typeof(HealthComponent));

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
                Current = health,
                Max = health,
            });

            return entity;
        }

        private static void WriteResult(bool batchMode, bool success, string details)
        {
            string summary =
                "BLOODLINES_VERDANT_WARDEN_SMOKE " +
                (success ? "PASS " : "FAIL ") +
                details;

            try
            {
                string logPath = Path.GetFullPath(Path.Combine(Application.dataPath, ArtifactPath));
                Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
                File.WriteAllText(logPath, summary);
            }
            catch
            {
            }

            if (success)
            {
                UnityDebug.Log(summary);
            }
            else
            {
                UnityDebug.LogError(summary);
            }

            if (batchMode)
            {
                EditorApplication.Exit(success ? 0 : 1);
            }
        }

        private sealed class DebugCommandSurfaceScope : IDisposable
        {
            private readonly World previousDefaultWorld;
            private readonly GameObject hostObject;

            public DebugCommandSurfaceScope(World world)
            {
                previousDefaultWorld = World.DefaultGameObjectInjectionWorld;
                World.DefaultGameObjectInjectionWorld = world;
                hostObject = new GameObject("BloodlinesVerdantWardenSmoke_CommandSurface")
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
