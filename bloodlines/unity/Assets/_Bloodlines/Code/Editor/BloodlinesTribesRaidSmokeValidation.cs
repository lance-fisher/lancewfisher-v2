#if UNITY_EDITOR
using System;
using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Core;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Smoke validation for the tribes raid AI driver.
    ///
    /// Browser parity: src/game/core/ai.js updateNeutralAi (~3044-3141).
    /// The tribes faction maintains a raid timer; when it expires, raiders
    /// are dispatched to the nearest non-tribes-owned control point. The
    /// timer reset is shortened by world pressure level and Great Reckoning
    /// state.
    ///
    /// Phase 1 (raid dispatch): timer expires, raiders receive a move
    /// command toward the nearest contested control point.
    /// Phase 2 (timer reset): after dispatch, raid timer is reset to a
    /// non-zero value derived from the canonical baseline.
    /// </summary>
    public static class BloodlinesTribesRaidSmokeValidation
    {
        [MenuItem("Bloodlines/Validation/Run Tribes Raid Smoke Validation")]
        public static void RunTribesRaidSmokeValidation()
        {
            ExecuteValidation(exitOnComplete: false);
        }

        public static void RunBatchTribesRaidSmokeValidation()
        {
            ExecuteValidation(exitOnComplete: true);
        }

        private static void ExecuteValidation(bool exitOnComplete)
        {
            bool success = false;
            string message;

            try
            {
                success = RunValidation(out message);
                if (success)
                {
                    UnityDebug.Log(message);
                }
                else
                {
                    UnityDebug.LogError(message);
                }
            }
            catch (Exception exception)
            {
                message = "Tribes raid smoke validation failed with an exception: " + exception;
                UnityDebug.LogError(message);
            }

            if (exitOnComplete)
            {
                EditorApplication.Exit(success ? 0 : 1);
            }
        }

        private static bool RunValidation(out string message)
        {
            using var world = new World("BloodlinesTribesRaidSmokeValidation");
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simGroup.AddSystemToUpdateList(world.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>());
            simGroup.AddSystemToUpdateList(world.GetOrCreateSystem<TribesRaidSystem>());
            simGroup.SortSystems();
            var em = world.EntityManager;

            // Tribes faction with raid state ready to fire on first tick.
            var tribesFaction = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(TribesRaidStateComponent));
            em.SetComponentData(tribesFaction, new FactionComponent { FactionId = "tribes" });
            em.SetComponentData(tribesFaction, new FactionKindComponent { Kind = FactionKind.Tribes });
            em.SetComponentData(tribesFaction, new TribesRaidStateComponent
            {
                RaidTimerSeconds = 0.05f,
                BaseRaidIntervalSeconds = 30f,
            });

            // Three tribes raiders (Melee role, alive). Worker excluded by the
            // raid filter, so include one worker to prove it doesn't get
            // dispatched.
            Entity[] raiders = new Entity[3];
            float3 campCenter = new float3(20f, 0f, 20f);
            for (int i = 0; i < raiders.Length; i++)
            {
                raiders[i] = CreateRaider(em, "tribes", UnitRole.Melee,
                    new float3(campCenter.x + i, 0f, campCenter.z));
            }
            Entity workerRaider = CreateRaider(em, "tribes", UnitRole.Worker,
                new float3(campCenter.x + 5f, 0f, campCenter.z));

            // Player-owned control point at (10, 0, 10) — closest non-tribes CP.
            var cpEntity = em.CreateEntity(
                typeof(ControlPointComponent),
                typeof(PositionComponent));
            em.SetComponentData(cpEntity, new ControlPointComponent
            {
                ControlPointId = new FixedString32Bytes("cp_player_home"),
                OwnerFactionId = new FixedString32Bytes("player"),
                Loyalty = 100f,
            });
            em.SetComponentData(cpEntity, new PositionComponent { Value = new float3(10f, 0f, 10f) });

            // Decoy: a tribes-owned CP that should NOT be picked.
            var tribesCp = em.CreateEntity(
                typeof(ControlPointComponent),
                typeof(PositionComponent));
            em.SetComponentData(tribesCp, new ControlPointComponent
            {
                ControlPointId = new FixedString32Bytes("cp_tribes_camp"),
                OwnerFactionId = new FixedString32Bytes("tribes"),
                Loyalty = 100f,
            });
            em.SetComponentData(tribesCp, new PositionComponent { Value = new float3(22f, 0f, 22f) });

            // Tick once with dt > raidTimer to trigger dispatch.
            const float DtSeconds = 0.1f;
            world.SetTime(new TimeData(0d, DtSeconds));
            world.Update();

            // All Melee raiders should have an active MoveCommand pointing at
            // the player CP (10, 0, 10).
            int dispatchedCount = 0;
            for (int i = 0; i < raiders.Length; i++)
            {
                var move = em.GetComponentData<MoveCommandComponent>(raiders[i]);
                if (!move.IsActive) continue;
                if (math.distance(move.Destination, new float3(10f, 0f, 10f)) > 0.1f) continue;
                dispatchedCount++;
            }
            if (dispatchedCount != 3)
            {
                message = "Tribes raid smoke FAIL: expected 3 melee raiders dispatched to player CP, got " +
                    dispatchedCount + ".";
                return false;
            }

            // Worker raider should NOT have been dispatched.
            var workerMove = em.GetComponentData<MoveCommandComponent>(workerRaider);
            if (workerMove.IsActive && math.distance(workerMove.Destination, new float3(10f, 0f, 10f)) < 0.1f)
            {
                message = "Tribes raid smoke FAIL: worker tribes unit was dispatched as a raider (should be excluded).";
                return false;
            }

            // Raid timer should have been reset to baseline (subject to
            // multipliers; with no WP / Great Reckoning, multiplier=1 -> 30s).
            var raidState = em.GetComponentData<TribesRaidStateComponent>(tribesFaction);
            if (raidState.RaidTimerSeconds < 28f || raidState.RaidTimerSeconds > 32f)
            {
                message = "Tribes raid smoke FAIL: expected raidTimerSeconds reset near 30s, got " +
                    raidState.RaidTimerSeconds.ToString("0.##") + ".";
                return false;
            }

            // Phase 2: simulate Great Reckoning and verify multiplier kicks in.
            // Reset timer to fire again, set raid state.
            raidState.RaidTimerSeconds = 0.05f;
            em.SetComponentData(tribesFaction, raidState);

            var mpEntity = em.CreateEntity(typeof(MatchProgressionComponent));
            em.SetComponentData(mpEntity, new MatchProgressionComponent
            {
                StageNumber = 4,
                StageId = new FixedString32Bytes("war_turning_of_tides"),
                GreatReckoningActive = true,
                GreatReckoningTargetFactionId = new FixedString32Bytes("player"),
                GreatReckoningShare = 0.75f,
                GreatReckoningThreshold = 0.7f,
                DominantKingdomId = new FixedString32Bytes("player"),
                DominantTerritoryShare = 0.75f,
            });

            world.SetTime(new TimeData(1d, DtSeconds));
            world.Update();

            var raidStateAfter = em.GetComponentData<TribesRaidStateComponent>(tribesFaction);
            // 30 * 0.5 (Great Reckoning multiplier) = 15s
            if (raidStateAfter.RaidTimerSeconds < 13f || raidStateAfter.RaidTimerSeconds > 17f)
            {
                message = "Tribes raid smoke FAIL: expected Great Reckoning multiplier to drop interval to ~15s, got " +
                    raidStateAfter.RaidTimerSeconds.ToString("0.##") + ".";
                return false;
            }

            message =
                "Tribes raid smoke PASS: dispatchedCount=" + dispatchedCount +
                " workerExcluded=True" +
                " baseIntervalReset=" + raidState.RaidTimerSeconds.ToString("0.##") +
                " greatReckoningInterval=" + raidStateAfter.RaidTimerSeconds.ToString("0.##") +
                ".";
            return true;
        }

        private static Entity CreateRaider(EntityManager em, string factionId, UnitRole role, float3 position)
        {
            var entity = em.CreateEntity(
                typeof(FactionComponent),
                typeof(UnitTypeComponent),
                typeof(HealthComponent),
                typeof(PositionComponent),
                typeof(LocalTransform),
                typeof(MovementStatsComponent),
                typeof(MoveCommandComponent));
            em.SetComponentData(entity, new FactionComponent { FactionId = factionId });
            em.SetComponentData(entity, new UnitTypeComponent
            {
                TypeId = "tribal_raider",
                Role = role,
                SiegeClass = SiegeClass.None,
                PopulationCost = 1,
                Stage = 1,
            });
            em.SetComponentData(entity, new HealthComponent { Current = 70f, Max = 70f });
            em.SetComponentData(entity, new PositionComponent { Value = position });
            em.SetComponentData(entity, new LocalTransform
            {
                Position = position,
                Rotation = quaternion.identity,
                Scale = 1f,
            });
            em.SetComponentData(entity, new MovementStatsComponent { MaxSpeed = 60f });
            em.SetComponentData(entity, new MoveCommandComponent
            {
                Destination = position,
                StoppingDistance = 0.2f,
                IsActive = false,
            });
            return entity;
        }
    }
}
#endif
