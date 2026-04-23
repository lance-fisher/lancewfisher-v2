#if UNITY_EDITOR
using System;
using System.Text;
using Bloodlines.Combat;
using Bloodlines.Components;
using Bloodlines.Debug;
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
    public static class BloodlinesCommanderAuraSmokeValidation
    {
        [MenuItem("Bloodlines/Validation/Run Commander Aura Smoke")]
        public static void RunMenu()
        {
            RunInternal(batchMode: false);
        }

        public static string RunBatchCommanderAuraSmokeValidation()
        {
            return RunInternal(batchMode: true);
        }

        private static string RunInternal(bool batchMode)
        {
            int exitCode = 0;
            try
            {
                string phase1 = RunInRangePhase();
                string phase2 = RunOutOfRangePhase();
                string phase3 = RunCommanderDeathPhase();
                string summary = $"BLOODLINES_COMMANDER_AURA_SMOKE PASS {phase1}; {phase2}; {phase3}";
                UnityDebug.Log(summary);
                return summary;
            }
            catch (Exception ex)
            {
                exitCode = 1;
                string summary = "BLOODLINES_COMMANDER_AURA_SMOKE FAIL " + ex;
                UnityDebug.LogError(summary);
                return summary;
            }
            finally
            {
                if (batchMode)
                {
                    EditorApplication.Exit(exitCode);
                }
            }
        }

        private static string RunInRangePhase()
        {
            using var world = CreateValidationWorld("CommanderAuraInRangePhase");
            var entityManager = world.EntityManager;

            SeedFaction(
                entityManager,
                "player",
                ConvictionBand.ApexCruel,
                CovenantId.TheWild,
                DoctrinePath.Light);

            Entity commander = SeedUnit(
                entityManager,
                "commander_unit",
                "player",
                new float3(0f, 0f, 0f),
                90f,
                16f,
                120f,
                6f,
                0.9f,
                addCommander: true,
                memberId: "commander-player",
                renown: 20f);
            Entity ally = SeedUnit(
                entityManager,
                "swordsman",
                "player",
                new float3(80f, 0f, 0f),
                23f,
                10f,
                100f,
                5f,
                1f);

            Tick(world, 0.25f);

            var allyCombat = entityManager.GetComponentData<CombatStatsComponent>(ally);
            var allyMove = entityManager.GetComponentData<MovementStatsComponent>(ally);
            var allyRuntime = entityManager.GetComponentData<CombatStanceRuntimeComponent>(ally);
            var recipient = entityManager.GetComponentData<CommanderAuraRecipientComponent>(ally);

            if (allyCombat.AttackDamage <= 10f ||
                allyCombat.Sight <= 100f ||
                allyMove.MaxSpeed <= 5f ||
                allyRuntime.IsRetreating)
            {
                throw new InvalidOperationException(
                    $"Phase 1 failed: in-range ally did not keep the expected aura buffs. attack={allyCombat.AttackDamage:0.000} sight={allyCombat.Sight:0.000} speed={allyMove.MaxSpeed:0.000} retreating={allyRuntime.IsRetreating}");
            }

            World previousDefault = World.DefaultGameObjectInjectionWorld;
            World.DefaultGameObjectInjectionWorld = world;
            try
            {
                var surface = new BloodlinesDebugCommandSurface();
                if (!surface.TryDebugGetCommanderAura("commander-player", out var aura) ||
                    aura.AuraRadius <= CommanderAuraCanon.BaseAuraRadius ||
                    aura.AttackBonus <= 0f ||
                    math.abs(aura.MoraleBonus - recipient.MoraleBonus) > 0.0001f)
                {
                    throw new InvalidOperationException(
                        $"Phase 1 failed: debug readout was unavailable or invalid. radius={aura.AuraRadius:0.000} attackBonus={aura.AttackBonus:0.000} morale={aura.MoraleBonus:0.000}");
                }

                return $"phase1Attack={allyCombat.AttackDamage:0.000},phase1Sight={allyCombat.Sight:0.000},phase1Speed={allyMove.MaxSpeed:0.000}";
            }
            finally
            {
                World.DefaultGameObjectInjectionWorld = previousDefault;
            }
        }

        private static string RunOutOfRangePhase()
        {
            using var world = CreateValidationWorld("CommanderAuraOutOfRangePhase");
            var entityManager = world.EntityManager;

            SeedFaction(
                entityManager,
                "player",
                ConvictionBand.ApexCruel,
                CovenantId.TheWild,
                DoctrinePath.Light);

            SeedUnit(
                entityManager,
                "commander_unit",
                "player",
                new float3(0f, 0f, 0f),
                90f,
                16f,
                120f,
                6f,
                0.9f,
                addCommander: true,
                memberId: "commander-player",
                renown: 20f);
            Entity distantAlly = SeedUnit(
                entityManager,
                "swordsman",
                "player",
                new float3(260f, 0f, 0f),
                23f,
                10f,
                100f,
                5f,
                1f);

            Tick(world, 0.25f);

            var allyCombat = entityManager.GetComponentData<CombatStatsComponent>(distantAlly);
            var allyMove = entityManager.GetComponentData<MovementStatsComponent>(distantAlly);
            var allyRuntime = entityManager.GetComponentData<CombatStanceRuntimeComponent>(distantAlly);

            if (entityManager.HasComponent<CommanderAuraRecipientComponent>(distantAlly) ||
                math.abs(allyCombat.AttackDamage - 10f) > 0.001f ||
                math.abs(allyCombat.Sight - 100f) > 0.001f ||
                math.abs(allyMove.MaxSpeed - 5f) > 0.001f ||
                !allyRuntime.IsRetreating)
            {
                throw new InvalidOperationException(
                    $"Phase 2 failed: out-of-range ally should remain unbuffed and retreat. attack={allyCombat.AttackDamage:0.000} sight={allyCombat.Sight:0.000} speed={allyMove.MaxSpeed:0.000} retreating={allyRuntime.IsRetreating}");
            }

            return "phase2OutOfRange=unbuffed";
        }

        private static string RunCommanderDeathPhase()
        {
            using var world = CreateValidationWorld("CommanderAuraDeathPhase");
            var entityManager = world.EntityManager;

            SeedFaction(
                entityManager,
                "player",
                ConvictionBand.ApexCruel,
                CovenantId.TheWild,
                DoctrinePath.Light);

            Entity commander = SeedUnit(
                entityManager,
                "commander_unit",
                "player",
                new float3(0f, 0f, 0f),
                90f,
                16f,
                120f,
                6f,
                0.9f,
                addCommander: true,
                memberId: "commander-player",
                renown: 20f);
            Entity ally = SeedUnit(
                entityManager,
                "swordsman",
                "player",
                new float3(80f, 0f, 0f),
                23f,
                10f,
                100f,
                5f,
                1f);

            Tick(world, 0.25f);

            entityManager.SetComponentData(commander, new HealthComponent { Current = 0f, Max = 100f });
            Tick(world, 0.25f);

            var allyCombat = entityManager.GetComponentData<CombatStatsComponent>(ally);
            var allyMove = entityManager.GetComponentData<MovementStatsComponent>(ally);
            var allyRuntime = entityManager.GetComponentData<CombatStanceRuntimeComponent>(ally);

            if (entityManager.HasComponent<CommanderAuraRecipientComponent>(ally) ||
                math.abs(allyCombat.AttackDamage - 10f) > 0.001f ||
                math.abs(allyCombat.Sight - 100f) > 0.001f ||
                math.abs(allyMove.MaxSpeed - 5f) > 0.001f ||
                !allyRuntime.IsRetreating)
            {
                throw new InvalidOperationException(
                    $"Phase 3 failed: aura did not clear after commander death. attack={allyCombat.AttackDamage:0.000} sight={allyCombat.Sight:0.000} speed={allyMove.MaxSpeed:0.000} retreating={allyRuntime.IsRetreating}");
            }

            return "phase3Death=cleared";
        }

        private static World CreateValidationWorld(string name)
        {
            var world = new World(name);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulation = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulation.AddSystemToUpdateList(world.GetOrCreateSystem<CommanderAuraSystem>());
            simulation.AddSystemToUpdateList(world.GetOrCreateSystem<CombatStanceResolutionSystem>());
            simulation.SortSystems();
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

        private static Entity SeedFaction(
            EntityManager entityManager,
            string factionId,
            ConvictionBand convictionBand,
            CovenantId covenantId,
            DoctrinePath doctrinePath)
        {
            var entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(ConvictionComponent),
                typeof(FaithStateComponent),
                typeof(ResourceStockpileComponent),
                typeof(PopulationComponent));

            entityManager.SetComponentData(entity, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            entityManager.SetComponentData(entity, new ConvictionComponent
            {
                Band = convictionBand,
            });
            entityManager.SetComponentData(entity, new FaithStateComponent
            {
                SelectedFaith = covenantId,
                DoctrinePath = doctrinePath,
                Intensity = 80f,
                Level = 5,
            });
            entityManager.SetComponentData(entity, new ResourceStockpileComponent());
            entityManager.SetComponentData(entity, new PopulationComponent
            {
                Total = 8,
                Available = 8,
                Cap = 16,
                BaseCap = 16,
                CapBonus = 0,
            });
            return entity;
        }

        private static Entity SeedUnit(
            EntityManager entityManager,
            string unitId,
            string factionId,
            float3 position,
            float currentHealth,
            float attackDamage,
            float sight,
            float maxSpeed,
            float attackCooldown,
            bool addCommander = false,
            string memberId = null,
            float renown = 0f)
        {
            var entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(PositionComponent),
                typeof(HealthComponent),
                typeof(UnitTypeComponent),
                typeof(CombatStatsComponent),
                typeof(MovementStatsComponent),
                typeof(MoveCommandComponent),
                typeof(CombatStanceComponent),
                typeof(CombatStanceRuntimeComponent));

            entityManager.SetComponentData(entity, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            entityManager.SetComponentData(entity, new PositionComponent { Value = position });
            entityManager.SetComponentData(entity, new HealthComponent
            {
                Current = currentHealth,
                Max = 100f,
            });
            entityManager.SetComponentData(entity, new UnitTypeComponent
            {
                TypeId = new FixedString64Bytes(unitId),
                Role = UnitRole.Melee,
                SiegeClass = SiegeClass.None,
                PopulationCost = 1,
                Stage = 2,
            });
            entityManager.SetComponentData(entity, new CombatStatsComponent
            {
                AttackDamage = attackDamage,
                AttackRange = 18f,
                AttackCooldown = attackCooldown,
                Sight = sight,
                CooldownRemaining = 0f,
                TargetAcquireIntervalSeconds = 0.25f,
                TargetSightGraceSeconds = 0.35f,
            });
            entityManager.SetComponentData(entity, new MovementStatsComponent
            {
                MaxSpeed = maxSpeed,
            });
            entityManager.SetComponentData(entity, new MoveCommandComponent
            {
                Destination = position,
                StoppingDistance = 0.5f,
                IsActive = false,
            });
            entityManager.SetComponentData(entity, new CombatStanceComponent
            {
                Stance = CombatStance.RetreatOnLowHealth,
                LowHealthRetreatThreshold = 0.25f,
            });
            entityManager.SetComponentData(entity, new CombatStanceRuntimeComponent
            {
                IsRetreating = false,
                SuspendAutoAcquireUntilMoveStops = false,
            });

            if (addCommander)
            {
                entityManager.AddComponentData(entity, new CommanderComponent
                {
                    MemberId = new FixedString64Bytes(memberId ?? "commander"),
                    Role = new FixedString32Bytes("commander"),
                    Renown = renown,
                });
            }

            return entity;
        }
    }
}
#endif
