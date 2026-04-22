#if UNITY_EDITOR
using System;
using System.Globalization;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Debug;
using Bloodlines.Dynasties;
using Bloodlines.GameTime;
using Bloodlines.Victory;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Dedicated validator for the dynasty renown/prestige read surface.
    /// Proves accumulation, decay, territory scaling, and peak tracking in an
    /// isolated world so it stays independent of the main bootstrap scene.
    /// </summary>
    public static class BloodlinesDynastyRenownSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-dynasty-renown-smoke.log";

        [MenuItem("Bloodlines/Dynasty/Run Dynasty Renown Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchDynastyRenownSmokeValidation() => RunInternal(batchMode: true);

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
                message = "Dynasty renown smoke validation errored: " + e;
            }

            string artifact = "BLOODLINES_DYNASTY_RENOWN_SMOKE " +
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
            ok &= RunAccumulationPhase(lines);
            ok &= RunDecayPhase(lines);
            ok &= RunTerritoryScalingPhase(lines);
            ok &= RunPeakTrackingPhase(lines);
            report = lines.ToString();
            return ok;
        }

        private static bool RunAccumulationPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("dynasty-renown-phase1");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 0f);
            SeedVictoryState(entityManager);
            var playerFaction = SeedDynastyFaction(entityManager, "player", faithIntensity: 72f, faithLevel: 4);
            SeedDynastyFaction(entityManager, "enemy");
            SeedControlPoint(entityManager, "cp-player-1", "player", 96f);
            SeedControlPoint(entityManager, "cp-player-2", "player", 93f);
            SeedControlPoint(entityManager, "cp-enemy-1", "enemy", 70f);

            TickToInWorldDays(world, entityManager, 0f);
            TickToInWorldDays(world, entityManager, 2f);

            if (!entityManager.HasComponent<DynastyRenownComponent>(playerFaction))
            {
                lines.AppendLine("Phase 1 FAIL: player faction has no DynastyRenownComponent after tick.");
                return false;
            }

            var renown = entityManager.GetComponentData<DynastyRenownComponent>(playerFaction);
            if (renown.RenownScore <= 0f)
            {
                lines.AppendLine("Phase 1 FAIL: expected renown accumulation to produce Score > 0.");
                return false;
            }

            if (!debugScope.CommandSurface.TryDebugGetDynastyRenown("player", out var readout))
            {
                lines.AppendLine("Phase 1 FAIL: debug readout could not resolve player dynasty renown.");
                return false;
            }

            lines.AppendLine(
                "Phase 1 PASS: accumulated dynasty renown score=" +
                renown.RenownScore.ToString("0.000", CultureInfo.InvariantCulture) +
                " readout=" + readout);
            return true;
        }

        private static bool RunDecayPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("dynasty-renown-phase2");
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 0f);
            SeedVictoryState(entityManager);
            var playerFaction = SeedDynastyFaction(entityManager, "player");
            SeedDynastyFaction(entityManager, "enemy");

            entityManager.AddComponentData(playerFaction, new DynastyRenownComponent
            {
                RenownScore = 10f,
                LastRenownUpdateInWorldDays = 0f,
                RenownDecayRate = 1f,
                PeakRenown = 10f,
                LastRulingMemberId = new FixedString64Bytes("player-head"),
                Initialized = 1,
            });

            TickToInWorldDays(world, entityManager, 3f);

            var renown = entityManager.GetComponentData<DynastyRenownComponent>(playerFaction);
            if (renown.RenownScore >= 10f)
            {
                lines.AppendLine("Phase 2 FAIL: expected decay to lower dynasty renown.");
                return false;
            }

            lines.AppendLine(
                "Phase 2 PASS: decay reduced score to " +
                renown.RenownScore.ToString("0.000", CultureInfo.InvariantCulture) + ".");
            return true;
        }

        private static bool RunTerritoryScalingPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("dynasty-renown-phase3");
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 0f);
            SeedVictoryState(entityManager);
            var playerFaction = SeedDynastyFaction(entityManager, "player");
            var enemyFaction = SeedDynastyFaction(entityManager, "enemy");
            SeedDynastyFaction(entityManager, "ally");

            SeedControlPoint(entityManager, "cp-player-1", "player", 95f);
            SeedControlPoint(entityManager, "cp-player-2", "player", 95f);
            SeedControlPoint(entityManager, "cp-player-3", "player", 95f);
            SeedControlPoint(entityManager, "cp-enemy-1", "enemy", 95f);

            TickToInWorldDays(world, entityManager, 0f);
            TickToInWorldDays(world, entityManager, 1f);

            float playerScore = entityManager.GetComponentData<DynastyRenownComponent>(playerFaction).RenownScore;
            float enemyScore = entityManager.GetComponentData<DynastyRenownComponent>(enemyFaction).RenownScore;

            if (playerScore <= enemyScore)
            {
                lines.AppendLine(
                    "Phase 3 FAIL: expected territory advantage to outscore the lower-hold faction. " +
                    "player=" + playerScore.ToString("0.000", CultureInfo.InvariantCulture) +
                    ", enemy=" + enemyScore.ToString("0.000", CultureInfo.InvariantCulture) + ".");
                return false;
            }

            lines.AppendLine(
                "Phase 3 PASS: territory scaling player=" +
                playerScore.ToString("0.000", CultureInfo.InvariantCulture) +
                " enemy=" + enemyScore.ToString("0.000", CultureInfo.InvariantCulture) + ".");
            return true;
        }

        private static bool RunPeakTrackingPhase(System.Text.StringBuilder lines)
        {
            using var world = CreateValidationWorld("dynasty-renown-phase4");
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 0f);
            SeedVictoryState(entityManager);
            var playerFaction = SeedDynastyFaction(entityManager, "player", faithIntensity: 80f, faithLevel: 5);
            SeedDynastyFaction(entityManager, "enemy");
            SeedControlPoint(entityManager, "cp-player-1", "player", 96f);
            SeedControlPoint(entityManager, "cp-player-2", "player", 96f);

            entityManager.AddComponentData(playerFaction, new DynastyRenownComponent
            {
                RenownScore = 4f,
                LastRenownUpdateInWorldDays = 0f,
                RenownDecayRate = 0.1f,
                PeakRenown = 4f,
                LastRulingMemberId = new FixedString64Bytes("player-head"),
                Initialized = 1,
            });

            TickToInWorldDays(world, entityManager, 0f);
            TickToInWorldDays(world, entityManager, 2f);

            var renown = entityManager.GetComponentData<DynastyRenownComponent>(playerFaction);
            if (renown.PeakRenown < renown.RenownScore || renown.PeakRenown <= 4f)
            {
                lines.AppendLine(
                    "Phase 4 FAIL: expected peak renown to track the new high-water mark. " +
                    "score=" + renown.RenownScore.ToString("0.000", CultureInfo.InvariantCulture) +
                    ", peak=" + renown.PeakRenown.ToString("0.000", CultureInfo.InvariantCulture) + ".");
                return false;
            }

            lines.AppendLine(
                "Phase 4 PASS: peak tracked at " +
                renown.PeakRenown.ToString("0.000", CultureInfo.InvariantCulture) + ".");
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<DynastyRenownAccumulationSystem>());
            simulationGroup.SortSystems();
            return world;
        }

        private static void TickToInWorldDays(World world, EntityManager entityManager, float inWorldDays)
        {
            using var query = entityManager.CreateEntityQuery(ComponentType.ReadWrite<DualClockComponent>());
            var dualClock = query.GetSingleton<DualClockComponent>();
            dualClock.InWorldDays = inWorldDays;
            query.SetSingleton(dualClock);
            world.SetTime(new Unity.Core.TimeData(inWorldDays, 0.05f));
            world.Update();
        }

        private static void SeedDualClock(EntityManager entityManager, float inWorldDays)
        {
            var dualClockEntity = entityManager.CreateEntity(typeof(DualClockComponent));
            entityManager.SetComponentData(dualClockEntity, new DualClockComponent
            {
                InWorldDays = inWorldDays,
                DaysPerRealSecond = 2f,
                DeclarationCount = 0,
            });
        }

        private static void SeedVictoryState(EntityManager entityManager)
        {
            var victoryEntity = entityManager.CreateEntity(typeof(VictoryStateComponent));
            entityManager.SetComponentData(victoryEntity, new VictoryStateComponent
            {
                Status = MatchStatus.Playing,
                VictoryType = VictoryConditionId.None,
                WinnerFactionId = default,
                VictoryReason = default,
                TerritorialGovernanceHoldSeconds = 0f,
            });
        }

        private static Entity SeedDynastyFaction(
            EntityManager entityManager,
            string factionId,
            float faithIntensity = 0f,
            int faithLevel = 0)
        {
            var entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            entityManager.AddComponentData(entity, new DynastyStateComponent
            {
                ActiveMemberCap = DynastyTemplates.InitialActiveMemberCap,
                DormantReserve = 0,
                Legitimacy = DynastyTemplates.InitialLegitimacy,
                LoyaltyPressure = 0f,
                Interregnum = false,
            });
            entityManager.AddBuffer<DynastyMemberRef>(entity);
            entityManager.AddComponentData(entity, new FaithStateComponent
            {
                SelectedFaith = faithIntensity > 0f ? CovenantId.TheOrder : CovenantId.None,
                DoctrinePath = DoctrinePath.Light,
                Intensity = faithIntensity,
                Level = faithLevel,
            });

            var headMember = entityManager.CreateEntity();
            entityManager.AddComponentData(headMember, new DynastyMemberComponent
            {
                MemberId = new FixedString64Bytes(factionId + "-head"),
                Title = new FixedString64Bytes("Head of Bloodline"),
                Role = DynastyRole.HeadOfBloodline,
                Path = DynastyPath.Governance,
                AgeYears = 38f,
                Status = DynastyMemberStatus.Ruling,
                Renown = 22f,
                Order = 0,
                FallenAtWorldSeconds = -1f,
            });
            entityManager.GetBuffer<DynastyMemberRef>(entity).Add(new DynastyMemberRef
            {
                Member = headMember,
            });

            return entity;
        }

        private static void SeedControlPoint(
            EntityManager entityManager,
            string controlPointId,
            string ownerFactionId,
            float loyalty)
        {
            var entity = entityManager.CreateEntity(typeof(ControlPointComponent));
            entityManager.SetComponentData(entity, new ControlPointComponent
            {
                ControlPointId = new FixedString32Bytes(controlPointId),
                OwnerFactionId = new FixedString32Bytes(ownerFactionId),
                CaptureFactionId = default,
                ContinentId = new FixedString32Bytes("home"),
                ControlState = ControlState.Stabilized,
                IsContested = false,
                Loyalty = loyalty,
                CaptureProgress = 0f,
                SettlementClassId = new FixedString32Bytes("march"),
                FortificationTier = 1,
                RadiusTiles = 6f,
                CaptureTimeSeconds = 10f,
                GoldTrickle = 0f,
                FoodTrickle = 0f,
                WaterTrickle = 0f,
                WoodTrickle = 0f,
                StoneTrickle = 0f,
                IronTrickle = 0f,
                InfluenceTrickle = 0f,
            });
        }
    }
}
#endif
