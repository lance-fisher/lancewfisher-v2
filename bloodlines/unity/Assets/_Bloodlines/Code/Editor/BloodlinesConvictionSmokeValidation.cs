#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Conviction;
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
    /// <summary>
    /// Governed conviction-scoring smoke validator. Runs in an isolated ECS world
    /// so it neither touches the Bootstrap scene nor conflicts with the
    /// combat/graphics smokes. Proves four things end-to-end:
    ///
    ///   1. A neutral faction with zero buckets resolves to the Neutral band.
    ///   2. A stewardship-heavy ledger rises through Moral into Apex Moral.
    ///   3. A ruthlessness+desecration ledger sinks through Cruel into Apex Cruel.
    ///   4. Band effect multipliers match the canonical browser table.
    ///
    /// Artifact: artifacts/unity-conviction-smoke.log.
    /// </summary>
    public static class BloodlinesConvictionSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-conviction-smoke.log";

        [MenuItem("Bloodlines/Conviction/Run Conviction Smoke Validation")]
        public static void RunInteractive()
        {
            RunInternal(batchMode: false);
        }

        public static void RunBatchConvictionSmokeValidation()
        {
            RunInternal(batchMode: true);
        }

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
                message = "Conviction smoke validation errored: " + e;
            }

            WriteResult(batchMode, success, message);
        }

        private static bool RunAllPhases(out string message)
        {
            if (!RunNeutralBaselinePhase(out string neutralMessage))
            {
                message = neutralMessage;
                return false;
            }

            if (!RunMoralAscentPhase(out string moralMessage))
            {
                message = moralMessage;
                return false;
            }

            if (!RunCruelDescentPhase(out string cruelMessage))
            {
                message = cruelMessage;
                return false;
            }

            if (!RunBandEffectsPhase(out string effectsMessage))
            {
                message = effectsMessage;
                return false;
            }

            if (!RunStarvationProtectionPhase(out string starvationMessage))
            {
                message = starvationMessage;
                return false;
            }

            if (!RunCapPressureProtectionPhase(out string capPressureMessage))
            {
                message = capPressureMessage;
                return false;
            }

            if (!RunCommanderCapturePhase(out string commanderCaptureMessage))
            {
                message = commanderCaptureMessage;
                return false;
            }

            message =
                "Conviction smoke validation passed: neutralPhase=True, moralAscentPhase=True, cruelDescentPhase=True, bandEffectsPhase=True, starvationProtectionPhase=True, capPressureProtectionPhase=True, commanderCapturePhase=True. " +
                neutralMessage + " " + moralMessage + " " + cruelMessage + " " + effectsMessage + " " + starvationMessage + " " + capPressureMessage + " " + commanderCaptureMessage;
            return true;
        }

        private static bool RunNeutralBaselinePhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesConvictionSmokeValidation_Neutral");
            var entityManager = world.EntityManager;
            var factionEntity = CreateFaction(entityManager, "player");

            TickOnce(world);

            var conviction = entityManager.GetComponentData<ConvictionComponent>(factionEntity);
            if (conviction.Band != ConvictionBand.Neutral || conviction.Score != 0f)
            {
                message =
                    "Conviction smoke validation failed: neutral baseline phase expected Neutral band with score 0. " +
                    "actualBand=" + conviction.Band + ", actualScore=" + conviction.Score + ".";
                return false;
            }

            message = "Neutral baseline: band=" + conviction.Band + ", score=" + conviction.Score + ".";
            return true;
        }

        private static bool RunMoralAscentPhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesConvictionSmokeValidation_Moral");
            using var commandSurfaceScope = new DebugCommandSurfaceScope(world);
            var commandSurface = commandSurfaceScope.CommandSurface;
            var entityManager = world.EntityManager;
            var factionEntity = CreateFaction(entityManager, "player");

            // 50 stewardship crosses into Moral (>= 25) but not Apex Moral.
            if (!commandSurface.TryDebugRecordConvictionEvent("player", ConvictionBucket.Stewardship, 50f))
            {
                message = "Conviction smoke validation failed: moral ascent phase could not record stewardship event.";
                return false;
            }

            var moralState = entityManager.GetComponentData<ConvictionComponent>(factionEntity);
            if (moralState.Band != ConvictionBand.Moral || moralState.Score != 50f)
            {
                message =
                    "Conviction smoke validation failed: moral ascent phase expected Moral at 50. " +
                    "actualBand=" + moralState.Band + ", actualScore=" + moralState.Score + ".";
                return false;
            }

            // +40 oathkeeping pushes score to 90, into Apex Moral (>= 75).
            commandSurface.TryDebugRecordConvictionEvent("player", ConvictionBucket.Oathkeeping, 40f);
            var apexState = entityManager.GetComponentData<ConvictionComponent>(factionEntity);
            if (apexState.Band != ConvictionBand.ApexMoral || apexState.Score != 90f)
            {
                message =
                    "Conviction smoke validation failed: moral ascent phase expected Apex Moral at 90. " +
                    "actualBand=" + apexState.Band + ", actualScore=" + apexState.Score + ".";
                return false;
            }

            message =
                "Moral ascent: moralBand=" + moralState.Band + "@" + moralState.Score +
                ", apexBand=" + apexState.Band + "@" + apexState.Score + ".";
            return true;
        }

        private static bool RunCruelDescentPhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesConvictionSmokeValidation_Cruel");
            using var commandSurfaceScope = new DebugCommandSurfaceScope(world);
            var commandSurface = commandSurfaceScope.CommandSurface;
            var entityManager = world.EntityManager;
            var factionEntity = CreateFaction(entityManager, "player");

            // -30 ruthlessness sinks score to -30, Cruel (>= -74).
            commandSurface.TryDebugRecordConvictionEvent("player", ConvictionBucket.Ruthlessness, 30f);
            var cruelState = entityManager.GetComponentData<ConvictionComponent>(factionEntity);
            if (cruelState.Band != ConvictionBand.Cruel || cruelState.Score != -30f)
            {
                message =
                    "Conviction smoke validation failed: cruel descent phase expected Cruel at -30. " +
                    "actualBand=" + cruelState.Band + ", actualScore=" + cruelState.Score + ".";
                return false;
            }

            // +100 desecration drops score to -130, Apex Cruel (< -74).
            commandSurface.TryDebugRecordConvictionEvent("player", ConvictionBucket.Desecration, 100f);
            var apexState = entityManager.GetComponentData<ConvictionComponent>(factionEntity);
            if (apexState.Band != ConvictionBand.ApexCruel || apexState.Score != -130f)
            {
                message =
                    "Conviction smoke validation failed: cruel descent phase expected Apex Cruel at -130. " +
                    "actualBand=" + apexState.Band + ", actualScore=" + apexState.Score + ".";
                return false;
            }

            message =
                "Cruel descent: cruelBand=" + cruelState.Band + "@" + cruelState.Score +
                ", apexBand=" + apexState.Band + "@" + apexState.Score + ".";
            return true;
        }

        private static bool RunBandEffectsPhase(out string message)
        {
            var apex = ConvictionBandEffects.ForBand(ConvictionBand.ApexMoral);
            if (apex.StabilizationMultiplier != 1.22f ||
                apex.LoyaltyProtectionMultiplier != 1.18f ||
                apex.CaptureMultiplier != 0.94f)
            {
                message =
                    "Conviction smoke validation failed: apex moral band effects drifted from canonical table. " +
                    "stabilization=" + apex.StabilizationMultiplier +
                    ", loyaltyProtection=" + apex.LoyaltyProtectionMultiplier +
                    ", capture=" + apex.CaptureMultiplier + ".";
                return false;
            }

            var apexCruel = ConvictionBandEffects.ForBand(ConvictionBand.ApexCruel);
            if (apexCruel.StabilizationMultiplier != 0.88f ||
                apexCruel.CaptureMultiplier != 1.22f ||
                apexCruel.AttackMultiplier != 1.12f)
            {
                message =
                    "Conviction smoke validation failed: apex cruel band effects drifted from canonical table. " +
                    "stabilization=" + apexCruel.StabilizationMultiplier +
                    ", capture=" + apexCruel.CaptureMultiplier +
                    ", attack=" + apexCruel.AttackMultiplier + ".";
                return false;
            }

            var neutral = ConvictionBandEffects.ForBand(ConvictionBand.Neutral);
            if (neutral.StabilizationMultiplier != 1f ||
                neutral.CaptureMultiplier != 1f)
            {
                message = "Conviction smoke validation failed: neutral band effects should all be 1. ";
                return false;
            }

            message =
                "Band effects: apexMoral.stabilization=" + apex.StabilizationMultiplier +
                ", apexCruel.capture=" + apexCruel.CaptureMultiplier +
                ", neutral.stabilization=" + neutral.StabilizationMultiplier + ".";
            return true;
        }

        private static bool RunStarvationProtectionPhase(out string message)
        {
            using var world = CreateStarvationValidationWorld("BloodlinesConvictionSmokeValidation_Starvation");
            var entityManager = world.EntityManager;

            CreateStarvationFaction(
                entityManager,
                factionId: "apex",
                band: ConvictionBand.ApexMoral,
                totalPopulation: 30,
                availablePopulation: 24,
                loyalty: 70f);
            CreateStarvationFaction(
                entityManager,
                factionId: "neutral",
                band: ConvictionBand.Neutral,
                totalPopulation: 30,
                availablePopulation: 24,
                loyalty: 70f);

            world.SetTime(new TimeData(0d, 0.05f));
            world.Update();

            if (!TryGetFactionState(entityManager, "apex", out var apexPopulation, out var apexLoyalty) ||
                !TryGetFactionState(entityManager, "neutral", out var neutralPopulation, out var neutralLoyalty))
            {
                message = "Conviction smoke validation failed: starvation protection phase could not resolve both faction states.";
                return false;
            }

            int apexPopulationLoss = 30 - apexPopulation.Total;
            int neutralPopulationLoss = 30 - neutralPopulation.Total;
            float apexLoyaltyLoss = 70f - apexLoyalty.Current;
            float neutralLoyaltyLoss = 70f - neutralLoyalty.Current;

            if (apexPopulationLoss >= neutralPopulationLoss)
            {
                message =
                    "Conviction smoke validation failed: Apex Moral famine protection did not reduce population loss. " +
                    "apexLoss=" + apexPopulationLoss + ", neutralLoss=" + neutralPopulationLoss + ".";
                return false;
            }

            if (apexLoyaltyLoss >= neutralLoyaltyLoss)
            {
                message =
                    "Conviction smoke validation failed: Apex Moral loyalty protection did not reduce loyalty loss. " +
                    "apexLoss=" + apexLoyaltyLoss + ", neutralLoss=" + neutralLoyaltyLoss + ".";
                return false;
            }

            message =
                "Starvation protection: apexPopulationLoss=" + apexPopulationLoss +
                ", neutralPopulationLoss=" + neutralPopulationLoss +
                ", apexLoyaltyLoss=" + apexLoyaltyLoss +
                ", neutralLoyaltyLoss=" + neutralLoyaltyLoss + ".";
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<ConvictionScoringSystem>());
            return world;
        }

        private static World CreateStarvationValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<StarvationResponseSystem>());

            var configEntity = world.EntityManager.CreateEntity();
            world.EntityManager.AddComponentData(configEntity, new RealmCycleConfig
            {
                FoodFamineConsecutiveCycles = 1,
                WaterCrisisConsecutiveCycles = 99,
                FaminePopulationDeclinePerCycle = 6,
                WaterCrisisOutmigrationPerCycle = 0,
                FamineLoyaltyDeltaPerCycle = -8,
                WaterCrisisLoyaltyDeltaPerCycle = 0,
            });

            return world;
        }

        private static World CreateCapPressureValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<CapPressureResponseSystem>());

            var configEntity = world.EntityManager.CreateEntity();
            world.EntityManager.AddComponentData(configEntity, new RealmCycleConfig
            {
                PopulationCapPressureRatio = 0.95f,
                CapPressureLoyaltyDeltaPerCycle = -10,
            });

            return world;
        }

        private static World CreateCommanderCaptureValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            world.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<DeathResolutionSystem>());
            return world;
        }

        private static void TickOnce(World world)
        {
            world.SetTime(new TimeData(0d, 0.05f));
            world.Update();
        }

        private static Entity CreateFaction(EntityManager entityManager, string factionId)
        {
            var entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, new FactionComponent { FactionId = factionId });
            entityManager.AddComponentData(entity, new ConvictionComponent
            {
                Band = ConvictionBand.Neutral,
            });
            return entity;
        }

        private static void CreateStarvationFaction(
            EntityManager entityManager,
            string factionId,
            ConvictionBand band,
            int totalPopulation,
            int availablePopulation,
            float loyalty)
        {
            var entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, new FactionComponent { FactionId = factionId });
            entityManager.AddComponentData(entity, new ConvictionComponent
            {
                Band = band,
            });
            entityManager.AddComponentData(entity, new PopulationComponent
            {
                Total = totalPopulation,
                Available = availablePopulation,
                Cap = totalPopulation + 10,
                BaseCap = totalPopulation + 10,
                CapBonus = 0,
                GrowthAccumulator = 0f,
            });
            entityManager.AddComponentData(entity, new ResourceStockpileComponent
            {
                Food = 0f,
                Water = 30f,
            });
            entityManager.AddComponentData(entity, new FactionLoyaltyComponent
            {
                Current = loyalty,
                Max = 100f,
                Floor = 0f,
            });
            entityManager.AddComponentData(entity, new RealmConditionComponent
            {
                CycleCount = 1,
                LastStarvationResponseCycle = 0,
                FoodStrainStreak = 1,
                WaterStrainStreak = 0,
            });
        }

        private static bool RunCapPressureProtectionPhase(out string message)
        {
            using var world = CreateCapPressureValidationWorld("BloodlinesConvictionSmokeValidation_CapPressure");
            var entityManager = world.EntityManager;

            CreateCapPressureFaction(entityManager, "apex", ConvictionBand.ApexMoral, 96, 100, 70f);
            CreateCapPressureFaction(entityManager, "neutral", ConvictionBand.Neutral, 96, 100, 70f);

            world.SetTime(new TimeData(0d, 0.05f));
            world.Update();

            if (!TryGetFactionState(entityManager, "apex", out _, out var apexLoyalty) ||
                !TryGetFactionState(entityManager, "neutral", out _, out var neutralLoyalty))
            {
                message = "Conviction smoke validation failed: cap-pressure phase could not resolve both faction states.";
                return false;
            }

            float apexLoyaltyLoss = 70f - apexLoyalty.Current;
            float neutralLoyaltyLoss = 70f - neutralLoyalty.Current;

            if (apexLoyaltyLoss >= neutralLoyaltyLoss)
            {
                message =
                    "Conviction smoke validation failed: Apex Moral cap-pressure protection did not reduce loyalty loss. " +
                    "apexLoss=" + apexLoyaltyLoss + ", neutralLoss=" + neutralLoyaltyLoss + ".";
                return false;
            }

            message =
                "Cap pressure protection: apexLoyaltyLoss=" + apexLoyaltyLoss +
                ", neutralLoyaltyLoss=" + neutralLoyaltyLoss + ".";
            return true;
        }

        private static bool RunCommanderCapturePhase(out string message)
        {
            using var world = CreateCommanderCaptureValidationWorld("BloodlinesConvictionSmokeValidation_CommanderCapture");
            var entityManager = world.EntityManager;

            var attackerFaction = CreateDynastyFaction(entityManager, "attacker", ConvictionBand.ApexCruel);
            var defenderFaction = CreateDynastyFaction(entityManager, "defender", ConvictionBand.Neutral);
            var defenderMember = CreateDynastyMember(
                entityManager,
                defenderFaction,
                "defender-commander",
                "War Captain",
                DynastyRole.Commander,
                DynastyMemberStatus.Active);

            float captureChance = CommanderCaptureUtility.ResolveCommanderCaptureChance(ConvictionBand.ApexCruel);
            if (captureChance <= 0f)
            {
                message = "Conviction smoke validation failed: Apex Cruel capture chance resolved to zero.";
                return false;
            }

            bool syntheticRollFound = false;
            for (int attackerIndex = 1; attackerIndex <= 32 && !syntheticRollFound; attackerIndex++)
            {
                for (int targetIndex = 1; targetIndex <= 32; targetIndex++)
                {
                    if (CommanderCaptureUtility.ShouldCaptureCommander(attackerIndex, targetIndex, captureChance))
                    {
                        syntheticRollFound = true;
                        break;
                    }
                }
            }

            if (!syntheticRollFound)
            {
                message = "Conviction smoke validation failed: synthetic commander-capture roll probe never succeeded for Apex Cruel chance.";
                return false;
            }

            var attackerEntity = entityManager.CreateEntity();
            entityManager.AddComponentData(attackerEntity, new FactionComponent { FactionId = "attacker" });

            var defenderEntity = entityManager.CreateEntity();
            entityManager.AddComponentData(defenderEntity, new FactionComponent { FactionId = "defender" });
            entityManager.AddComponentData(defenderEntity, new CommanderComponent
            {
                MemberId = "defender-commander",
                Role = "commander",
                Renown = 14f,
            });
            entityManager.AddComponentData(defenderEntity, new HealthComponent
            {
                Current = 0f,
                Max = 20f,
            });
            entityManager.AddComponentData(defenderEntity, new PendingCommanderCaptureComponent
            {
                CaptorFactionId = "attacker",
            });

            world.SetTime(new TimeData(0d, 0.05f));
            world.Update();

            if (!entityManager.HasBuffer<Bloodlines.AI.CapturedMemberElement>(attackerFaction))
            {
                message = "Conviction smoke validation failed: commander-capture phase produced no captive buffer on the attacker faction.";
                return false;
            }

            var captiveBuffer = entityManager.GetBuffer<Bloodlines.AI.CapturedMemberElement>(attackerFaction);
            if (captiveBuffer.Length != 1 ||
                !captiveBuffer[0].MemberId.Equals(new FixedString64Bytes("defender-commander")))
            {
                message =
                    "Conviction smoke validation failed: commander-capture phase produced the wrong captive ledger entry. " +
                    "count=" + captiveBuffer.Length + ".";
                return false;
            }

            var updatedMember = entityManager.GetComponentData<DynastyMemberComponent>(defenderMember);
            if (updatedMember.Status != DynastyMemberStatus.Captured)
            {
                message =
                    "Conviction smoke validation failed: commander-capture phase did not mark the dynasty member captured. " +
                    "status=" + updatedMember.Status + ".";
                return false;
            }

            message =
                "Commander capture: captiveMemberId=" + captiveBuffer[0].MemberId +
                ", status=" + updatedMember.Status +
                ", apexCruelChance=" + captureChance + ".";
            return true;
        }

        private static Entity CreateDynastyFaction(
            EntityManager entityManager,
            string factionId,
            ConvictionBand band)
        {
            var entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, new FactionComponent { FactionId = factionId });
            entityManager.AddComponentData(entity, new ConvictionComponent { Band = band });
            entityManager.AddComponentData(entity, new PopulationComponent
            {
                Total = 24,
                Available = 18,
                Cap = 32,
                BaseCap = 32,
                CapBonus = 0,
                GrowthAccumulator = 0f,
            });
            entityManager.AddBuffer<DynastyMemberRef>(entity);
            return entity;
        }

        private static Entity CreateDynastyMember(
            EntityManager entityManager,
            Entity factionEntity,
            string memberId,
            string title,
            DynastyRole role,
            DynastyMemberStatus status)
        {
            var memberEntity = entityManager.CreateEntity();
            entityManager.AddComponentData(memberEntity, new DynastyMemberComponent
            {
                MemberId = memberId,
                Title = title,
                Role = role,
                Path = DynastyPath.MilitaryCommand,
                AgeYears = 28f,
                Status = status,
                Renown = 14f,
                Order = 2,
                FallenAtWorldSeconds = -1f,
            });

            entityManager.GetBuffer<DynastyMemberRef>(factionEntity).Add(new DynastyMemberRef
            {
                Member = memberEntity,
            });

            return memberEntity;
        }

        private static void CreateCapPressureFaction(
            EntityManager entityManager,
            string factionId,
            ConvictionBand band,
            int totalPopulation,
            int cap,
            float loyalty)
        {
            var entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, new FactionComponent { FactionId = factionId });
            entityManager.AddComponentData(entity, new ConvictionComponent
            {
                Band = band,
            });
            entityManager.AddComponentData(entity, new PopulationComponent
            {
                Total = totalPopulation,
                Available = math.max(0, totalPopulation - 8),
                Cap = cap,
                BaseCap = cap,
                CapBonus = 0,
                GrowthAccumulator = 0f,
            });
            entityManager.AddComponentData(entity, new FactionLoyaltyComponent
            {
                Current = loyalty,
                Max = 100f,
                Floor = 0f,
            });
            entityManager.AddComponentData(entity, new RealmConditionComponent
            {
                CycleCount = 1,
                LastCapPressureResponseCycle = 0,
            });
        }

        private static bool TryGetFactionState(
            EntityManager entityManager,
            string factionId,
            out PopulationComponent population,
            out FactionLoyaltyComponent loyalty)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<PopulationComponent>(),
                ComponentType.ReadOnly<FactionLoyaltyComponent>());

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);

            var key = new FixedString32Bytes(factionId);
            for (int i = 0; i < entities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(key))
                {
                    continue;
                }

                population = entityManager.GetComponentData<PopulationComponent>(entities[i]);
                loyalty = entityManager.GetComponentData<FactionLoyaltyComponent>(entities[i]);
                return true;
            }

            population = default;
            loyalty = default;
            return false;
        }

        private static void WriteResult(bool batchMode, bool success, string message)
        {
            try
            {
                var logPath = Path.GetFullPath(Path.Combine(Application.dataPath, ArtifactPath));
                Directory.CreateDirectory(Path.GetDirectoryName(logPath));
                File.AppendAllText(logPath, message + Environment.NewLine);
            }
            catch
            {
            }

            UnityDebug.Log(message);
            if (batchMode)
            {
                EditorApplication.Exit(success ? 0 : 1);
            }
        }
    }

    internal sealed class DebugCommandSurfaceScope : IDisposable
    {
        private readonly World previousDefaultWorld;
        private readonly GameObject hostObject;

        public BloodlinesDebugCommandSurface CommandSurface { get; }

        public DebugCommandSurfaceScope(World world)
        {
            previousDefaultWorld = World.DefaultGameObjectInjectionWorld;
            World.DefaultGameObjectInjectionWorld = world;

            hostObject = new GameObject("BloodlinesConvictionSmokeValidation_CommandSurface")
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            CommandSurface = hostObject.AddComponent<BloodlinesDebugCommandSurface>();
        }

        public void Dispose()
        {
            if (hostObject != null)
            {
                UnityEngine.Object.DestroyImmediate(hostObject);
            }

            World.DefaultGameObjectInjectionWorld = previousDefaultWorld;
        }
    }
}
#endif
