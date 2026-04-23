#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Debug;
using Bloodlines.Dynasties;
using Bloodlines.GameTime;
using Bloodlines.Systems;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Dedicated validator for the succession crisis slice.
    /// Phase 1: ruler death triggers a non-none crisis.
    /// Phase 2: opening loyalty shock applies to all owned control points.
    /// Phase 3: recovery progresses on whole in-world days and removes the crisis.
    /// Phase 4: catastrophic crisis drains legitimacy faster than minor crisis.
    /// </summary>
    public static class BloodlinesSuccessionCrisisSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-succession-crisis-smoke.log";

        [MenuItem("Bloodlines/Dynasty/Run Succession Crisis Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchSuccessionCrisisSmokeValidation() => RunInternal(batchMode: true);

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
                message = "Succession crisis smoke errored: " + exception;
            }

            string artifact = "BLOODLINES_SUCCESSION_CRISIS_SMOKE " + (success ? "PASS" : "FAIL") + Environment.NewLine + message;
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
            var sb = new System.Text.StringBuilder();
            bool ok = true;
            ok &= RunTriggerPhase(sb);
            ok &= RunLoyaltyShockPhase(sb);
            ok &= RunRecoveryPhase(sb);
            ok &= RunSeverityDrainPhase(sb);
            report = sb.ToString();
            return ok;
        }

        private static bool RunTriggerPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("succession-crisis-phase1");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 0f);
            SeedDynastyFaction(entityManager, "player", headAge: 38f, heirAge: 19f, rivalCount: 1);
            world.Update();

            if (!debugScope.CommandSurface.TryDebugFellDynastyMember("player", DynastyRole.HeadOfBloodline))
            {
                sb.AppendLine("Phase 1 FAIL: could not fell the player ruler.");
                return false;
            }

            Tick(world, 0f);

            if (!debugScope.CommandSurface.TryDebugGetSuccessionCrisis("player", out var crisis) ||
                crisis.CrisisSeverity == (byte)SuccessionCrisisSeverity.None)
            {
                sb.AppendLine("Phase 1 FAIL: ruler death did not create a succession crisis.");
                return false;
            }

            sb.AppendLine("Phase 1 PASS: crisis severity=" + ((SuccessionCrisisSeverity)crisis.CrisisSeverity) + ".");
            return true;
        }

        private static bool RunLoyaltyShockPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("succession-crisis-phase2");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 0f);
            SeedDynastyFaction(entityManager, "player", headAge: 38f, heirAge: 19f, rivalCount: 1);
            SeedOwnedControlPoint(entityManager, "player-cp-a", "player", 80f);
            SeedOwnedControlPoint(entityManager, "player-cp-b", "player", 75f);

            world.Update();
            debugScope.CommandSurface.TryDebugFellDynastyMember("player", DynastyRole.HeadOfBloodline);
            Tick(world, 0f);

            float loyaltyA = GetControlPointLoyalty(entityManager, "player-cp-a");
            float loyaltyB = GetControlPointLoyalty(entityManager, "player-cp-b");
            if (Math.Abs(loyaltyA - 75f) > 0.01f || Math.Abs(loyaltyB - 70f) > 0.01f)
            {
                sb.AppendLine($"Phase 2 FAIL: opening loyalty shock drifted cpA={loyaltyA:0.##} cpB={loyaltyB:0.##}.");
                return false;
            }

            sb.AppendLine($"Phase 2 PASS: opening loyalty shock applied cpA={loyaltyA:0.##} cpB={loyaltyB:0.##}.");
            return true;
        }

        private static bool RunRecoveryPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("succession-crisis-phase3");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 0f);
            SeedDynastyFaction(entityManager, "player", headAge: 38f, heirAge: 19f, rivalCount: 1, convictionBand: ConvictionBand.ApexMoral);
            world.Update();
            debugScope.CommandSurface.TryDebugFellDynastyMember("player", DynastyRole.HeadOfBloodline);
            Tick(world, 0f);

            if (!debugScope.CommandSurface.TryDebugGetSuccessionCrisis("player", out _))
            {
                sb.AppendLine("Phase 3 FAIL: no crisis existed before recovery.");
                return false;
            }

            Tick(world, 50f);

            if (debugScope.CommandSurface.TryDebugGetSuccessionCrisis("player", out _))
            {
                sb.AppendLine("Phase 3 FAIL: crisis still active after full recovery window.");
                return false;
            }

            sb.AppendLine("Phase 3 PASS: recovery removed the crisis after whole-day progress.");
            return true;
        }

        private static bool RunSeverityDrainPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("succession-crisis-phase4");
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 0f);
            Entity catastrophicFaction = SeedDynastyFaction(entityManager, "catastrophic", headAge: 38f, heirAge: 17f, rivalCount: 0, heirStatus: DynastyMemberStatus.Fallen);
            Entity minorFaction = SeedDynastyFaction(entityManager, "minor", headAge: 38f, heirAge: 30f, rivalCount: 1);
            world.Update();

            FellRuler(entityManager, catastrophicFaction);
            FellRuler(entityManager, minorFaction);
            Tick(world, 0f);
            Tick(world, 1f);

            float catastrophicLegitimacy = entityManager.GetComponentData<DynastyStateComponent>(catastrophicFaction).Legitimacy;
            float minorLegitimacy = entityManager.GetComponentData<DynastyStateComponent>(minorFaction).Legitimacy;
            if (!(catastrophicLegitimacy < minorLegitimacy))
            {
                sb.AppendLine(
                    $"Phase 4 FAIL: expected catastrophic drain to exceed minor drain. catastrophic={catastrophicLegitimacy:0.##} minor={minorLegitimacy:0.##}.");
                return false;
            }

            sb.AppendLine(
                $"Phase 4 PASS: catastrophic legitimacy={catastrophicLegitimacy:0.##} minor={minorLegitimacy:0.##}.");
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulation = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulation.AddSystemToUpdateList(world.GetOrCreateSystem<DynastySuccessionSystem>());
            simulation.AddSystemToUpdateList(world.GetOrCreateSystem<SuccessionCrisisEvaluationSystem>());
            simulation.AddSystemToUpdateList(world.GetOrCreateSystem<SuccessionCrisisRecoverySystem>());
            simulation.SortSystems();
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

        private static Entity SeedDynastyFaction(
            EntityManager entityManager,
            string factionId,
            float headAge,
            float heirAge,
            int rivalCount,
            ConvictionBand convictionBand = ConvictionBand.Neutral,
            DynastyMemberStatus heirStatus = DynastyMemberStatus.Active)
        {
            Entity faction = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(DynastyStateComponent),
                typeof(ConvictionComponent));
            entityManager.SetComponentData(faction, new FactionComponent { FactionId = factionId });
            entityManager.SetComponentData(faction, new DynastyStateComponent
            {
                ActiveMemberCap = 8,
                DormantReserve = 0,
                Legitimacy = 80f,
                LoyaltyPressure = 0f,
                Interregnum = false,
            });
            entityManager.SetComponentData(faction, new ConvictionComponent { Band = convictionBand });

            entityManager.AddBuffer<DynastyMemberRef>(faction);
            entityManager.AddBuffer<DynastyFallenLedger>(faction);

            AddMember(entityManager, faction, "head-" + factionId, "Ruling Seat", DynastyRole.HeadOfBloodline, DynastyMemberStatus.Ruling, headAge, 12f, order: 0);
            AddMember(entityManager, faction, "heir-" + factionId, "Young Heir", DynastyRole.HeirDesignate, heirStatus, heirAge, 9f, order: 1);
            for (int i = 0; i < rivalCount; i++)
            {
                AddMember(
                    entityManager,
                    faction,
                    $"rival-{factionId}-{i}",
                    $"Rival {i + 1}",
                    i == 0 ? DynastyRole.Commander : DynastyRole.Governor,
                    DynastyMemberStatus.Active,
                    29f + i,
                    15f + (i * 3f),
                    order: 2 + i);
            }

            return faction;
        }

        private static void AddMember(
            EntityManager entityManager,
            Entity factionEntity,
            string memberId,
            string title,
            DynastyRole role,
            DynastyMemberStatus status,
            float age,
            float renown,
            int order)
        {
            Entity member = entityManager.CreateEntity(typeof(DynastyMemberComponent));
            entityManager.SetComponentData(member, new DynastyMemberComponent
            {
                MemberId = memberId,
                Title = title,
                Role = role,
                Path = DynastyPath.Governance,
                AgeYears = age,
                Status = status,
                Renown = renown,
                Order = order,
                FallenAtWorldSeconds = 0f,
            });
            var roster = entityManager.GetBuffer<DynastyMemberRef>(factionEntity);
            roster.Add(new DynastyMemberRef { Member = member });
        }

        private static void SeedOwnedControlPoint(EntityManager entityManager, string controlPointId, string factionId, float loyalty)
        {
            Entity controlPoint = entityManager.CreateEntity(typeof(ControlPointComponent));
            entityManager.SetComponentData(controlPoint, new ControlPointComponent
            {
                ControlPointId = controlPointId,
                OwnerFactionId = factionId,
                CaptureFactionId = default,
                ContinentId = "west",
                ControlState = ControlState.Stabilized,
                IsContested = false,
                Loyalty = loyalty,
                CaptureProgress = 0f,
                SettlementClassId = "primary_dynastic_keep",
                FortificationTier = 2,
                RadiusTiles = 3f,
                CaptureTimeSeconds = 9f,
                GoldTrickle = 1f,
                FoodTrickle = 1f,
                WaterTrickle = 1f,
                WoodTrickle = 0f,
                StoneTrickle = 0f,
                IronTrickle = 0f,
                InfluenceTrickle = 0.2f,
            });
        }

        private static void FellRuler(EntityManager entityManager, Entity factionEntity)
        {
            var roster = entityManager.GetBuffer<DynastyMemberRef>(factionEntity);
            for (int i = 0; i < roster.Length; i++)
            {
                Entity memberEntity = roster[i].Member;
                var member = entityManager.GetComponentData<DynastyMemberComponent>(memberEntity);
                if (member.Status == DynastyMemberStatus.Ruling)
                {
                    member.Status = DynastyMemberStatus.Fallen;
                    entityManager.SetComponentData(memberEntity, member);
                    return;
                }
            }
        }

        private static float GetControlPointLoyalty(EntityManager entityManager, string controlPointId)
        {
            using var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<ControlPointComponent>());
            using NativeArray<ControlPointComponent> controlPoints =
                query.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);
            for (int i = 0; i < controlPoints.Length; i++)
            {
                if (controlPoints[i].ControlPointId.Equals(controlPointId))
                {
                    return controlPoints[i].Loyalty;
                }
            }

            return -1f;
        }

        private static void Tick(World world, float inWorldDays)
        {
            var entityManager = world.EntityManager;
            using var query = entityManager.CreateEntityQuery(ComponentType.ReadWrite<DualClockComponent>());
            var clock = query.GetSingleton<DualClockComponent>();
            clock.InWorldDays = inWorldDays;
            query.SetSingleton(clock);
            world.SetTime(new Unity.Core.TimeData(inWorldDays, 0.05f));
            world.Update();
        }
    }
}
#endif
