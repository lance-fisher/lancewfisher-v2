#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Debug;
using Bloodlines.Dynasties;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Smoke validator for the minor-house levy parity slice.
    ///
    /// Phase 1: defection spawns a live minor faction claim with levy provenance.
    /// Phase 2: landless minor houses decay levy progress instead of spawning.
    /// Phase 3: low-loyalty claims stay unsettled, decay progress, and surface the
    ///          same status through the debug readout.
    /// Phase 4: pressured stabilized claims raise the correct levy profile and spend costs.
    /// Phase 5: retinue cap blocks over-mustering and clears progress.
    /// </summary>
    public static class BloodlinesMinorHouseLevyParitySmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-minor-house-levy-parity-smoke.log";
        private const float UnsetInWorldDays = -1f;

        [MenuItem("Bloodlines/AI/Run Minor House Levy Parity Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchMinorHouseLevyParitySmokeValidation() => RunInternal(batchMode: true);

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
                message = "Minor-house levy parity smoke errored: " + e;
            }

            string artifact = "BLOODLINES_MINOR_HOUSE_LEVY_PARITY_SMOKE " + (success ? "PASS" : "FAIL") + "\n" + message;
            UnityDebug.Log(artifact);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ArtifactPath)!);
                File.WriteAllText(ArtifactPath, artifact);
            }
            catch (Exception)
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
            ok &= RunPhaseDefectionClaimSpawn(sb);
            ok &= RunPhaseLandlessDecay(sb);
            ok &= RunPhaseUnsettledClaimGate(sb);
            ok &= RunPhasePressuredBowmanRaise(sb);
            ok &= RunPhaseMusterCapBlock(sb);
            report = sb.ToString();
            return ok;
        }

        private static SimulationSystemGroup SetupSimGroup(World world)
        {
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulation = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            return simulation;
        }

        private static bool RunPhaseDefectionClaimSpawn(System.Text.StringBuilder sb)
        {
            using var world = new World("minor-house-phase1");
            var em = world.EntityManager;
            var simulation = SetupSimGroup(world);
            simulation.AddSystemToUpdateList(world.GetOrCreateSystem<LesserHouseLoyaltyDriftSystem>());

            var clock = em.CreateEntity(typeof(DualClockComponent));
            em.SetComponentData(clock, new DualClockComponent { InWorldDays = 16f });

            var parent = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(DynastyStateComponent));
            em.SetComponentData(parent, new FactionComponent { FactionId = new FixedString32Bytes("player") });
            em.SetComponentData(parent, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(parent, new DynastyStateComponent { Legitimacy = 20f });

            var lesserHouses = em.AddBuffer<LesserHouseElement>(parent);
            lesserHouses.Add(new LesserHouseElement
            {
                HouseId = new FixedString32Bytes("cadet-ashfall"),
                HouseName = new FixedString64Bytes("Cadet Ashfall"),
                FounderMemberId = new FixedString64Bytes("founder-1"),
                Loyalty = 0f,
                DailyLoyaltyDelta = -0.5f,
                LastDriftAppliedInWorldDays = 15f,
                Defected = false,
                MixedBloodlineHouseId = default,
                MixedBloodlinePressure = 0f,
                MaritalAnchorHouseId = default,
                MaritalAnchorMarriageId = default,
                MaritalAnchorStatus = LesserHouseMaritalAnchorStatus.None,
                LastMaritalAnchorStatus = LesserHouseMaritalAnchorStatus.None,
                MaritalAnchorPressure = 0f,
                MaritalAnchorChildCount = 0,
                WorldPressureStatus = LesserHouseWorldPressureStatus.Quiet,
                WorldPressurePressure = 0f,
                WorldPressureLevel = 0,
                CurrentDailyLoyaltyDelta = 0f,
                BrokeAtLoyaltyZeroInWorldDays = 10f,
                DepartedAtInWorldDays = UnsetInWorldDays,
            });

            var anchor = em.CreateEntity(typeof(PositionComponent), typeof(LocalTransform), typeof(ControlPointComponent));
            em.SetComponentData(anchor, new PositionComponent { Value = new float3(120f, 0f, 88f) });
            em.SetComponentData(anchor, new LocalTransform
            {
                Position = new float3(120f, 0f, 88f),
                Rotation = quaternion.identity,
                Scale = 1f,
            });
            em.SetComponentData(anchor, new ControlPointComponent
            {
                ControlPointId = new FixedString32Bytes("player-keep"),
                OwnerFactionId = new FixedString32Bytes("player"),
                CaptureFactionId = default,
                ContinentId = new FixedString32Bytes("west"),
                ControlState = ControlState.Stabilized,
                IsContested = false,
                Loyalty = 80f,
                CaptureProgress = 0f,
                SettlementClassId = new FixedString32Bytes("primary_dynastic_keep"),
                FortificationTier = 2,
                RadiusTiles = 3f,
                CaptureTimeSeconds = 9f,
                GoldTrickle = 0f,
                FoodTrickle = 0.1f,
                WaterTrickle = 0f,
                WoodTrickle = 0f,
                StoneTrickle = 0f,
                IronTrickle = 0f,
                InfluenceTrickle = 0.05f,
            });

            world.Update();

            Entity minorFaction = FindFactionEntity(em, "minor-cadet-ashfall");
            if (minorFaction == Entity.Null)
            {
                sb.AppendLine("PhaseDefectionClaimSpawn FAIL: minor faction 'minor-cadet-ashfall' was not spawned.");
                return false;
            }

            if (!em.HasComponent<MinorHouseLevyComponent>(minorFaction) ||
                !em.HasComponent<ResourceStockpileComponent>(minorFaction) ||
                !em.HasComponent<PopulationComponent>(minorFaction) ||
                !em.HasComponent<DynastyStateComponent>(minorFaction))
            {
                sb.AppendLine("PhaseDefectionClaimSpawn FAIL: spawned minor faction is missing levy, stockpile, population, or dynasty state.");
                return false;
            }

            var levy = em.GetComponentData<MinorHouseLevyComponent>(minorFaction);
            if (!levy.OriginFactionId.Equals(new FixedString32Bytes("player")) ||
                !levy.ClaimControlPointId.Equals(new FixedString32Bytes("minor-cadet-ashfall-claim")))
            {
                sb.AppendLine($"PhaseDefectionClaimSpawn FAIL: levy provenance mismatch (origin='{levy.OriginFactionId}', claim='{levy.ClaimControlPointId}').");
                return false;
            }

            Entity claimEntity = FindControlPointEntity(em, "minor-cadet-ashfall-claim");
            if (claimEntity == Entity.Null)
            {
                sb.AppendLine("PhaseDefectionClaimSpawn FAIL: defected claim control point was not created.");
                return false;
            }

            var claim = em.GetComponentData<ControlPointComponent>(claimEntity);
            if (!claim.OwnerFactionId.Equals(new FixedString32Bytes("minor-cadet-ashfall")) ||
                claim.ControlState != ControlState.Stabilized ||
                math.abs(claim.Loyalty - 62f) > 0.01f ||
                math.abs(claim.FoodTrickle - 0.08f) > 0.001f ||
                math.abs(claim.InfluenceTrickle - 0.06f) > 0.001f ||
                !claim.ContinentId.Equals(new FixedString32Bytes("west")))
            {
                sb.AppendLine(
                    $"PhaseDefectionClaimSpawn FAIL: claim mismatch owner='{claim.OwnerFactionId}' state={claim.ControlState} loyalty={claim.Loyalty} food={claim.FoodTrickle} influence={claim.InfluenceTrickle} continent='{claim.ContinentId}'.");
                return false;
            }

            sb.AppendLine("PhaseDefectionClaimSpawn PASS: defected minor faction arrives with levy provenance, economy scaffolding, and a stabilized border claim.");
            return true;
        }

        private static bool RunPhaseLandlessDecay(System.Text.StringBuilder sb)
        {
            using var world = new World("minor-house-phase2");
            var em = world.EntityManager;
            var simulation = SetupSimGroup(world);
            simulation.AddSystemToUpdateList(world.GetOrCreateSystem<MinorHouseLevySystem>());

            var minor = CreateMinorFaction(
                em,
                factionId: "minor-landless",
                originFactionId: "player",
                claimId: "missing-claim",
                levyAccumulator: 5f,
                levySecondsRequired: 22f,
                food: 20f,
                influence: 20f);

            world.Update();

            var levy = em.GetComponentData<MinorHouseLevyComponent>(minor);
            if (levy.LevyStatus != MinorHouseLevyStatus.Landless ||
                !(levy.LevyAccumulator < 5f) ||
                levy.LeviesIssued != 0)
            {
                sb.AppendLine(
                    $"PhaseLandlessDecay FAIL: status={levy.LevyStatus} accumulator={levy.LevyAccumulator} levies={levy.LeviesIssued}.");
                return false;
            }

            sb.AppendLine($"PhaseLandlessDecay PASS: status={levy.LevyStatus} accumulator decayed to {levy.LevyAccumulator:0.00}.");
            return true;
        }

        private static bool RunPhaseUnsettledClaimGate(System.Text.StringBuilder sb)
        {
            using var world = new World("minor-house-phase3");
            var em = world.EntityManager;
            var simulation = SetupSimGroup(world);
            simulation.AddSystemToUpdateList(world.GetOrCreateSystem<MinorHouseLevySystem>());

            var minor = CreateMinorFaction(
                em,
                factionId: "minor-unsettled",
                originFactionId: "player",
                claimId: "minor-unsettled-claim",
                levyAccumulator: 4f,
                levySecondsRequired: 22f,
                food: 20f,
                influence: 20f);

            CreateClaim(
                em,
                controlPointId: "minor-unsettled-claim",
                ownerFactionId: "minor-unsettled",
                loyalty: 47f,
                controlState: ControlState.Stabilized,
                isContested: false,
                position: new float3(44f, 0f, 44f));

            world.Update();

            var levy = em.GetComponentData<MinorHouseLevyComponent>(minor);
            if (levy.LevyStatus != MinorHouseLevyStatus.Unsettled ||
                !(levy.LevyAccumulator < 4f) ||
                levy.LeviesIssued != 0)
            {
                sb.AppendLine(
                    $"PhaseUnsettledClaimGate FAIL: status={levy.LevyStatus} accumulator={levy.LevyAccumulator} levies={levy.LeviesIssued}.");
                return false;
            }

            World previousDefault = World.DefaultGameObjectInjectionWorld;
            World.DefaultGameObjectInjectionWorld = world;
            try
            {
                var surface = new BloodlinesDebugCommandSurface();
                if (!surface.TryDebugGetMinorHouseLevyState("minor-unsettled", out var readout))
                {
                    sb.AppendLine("PhaseUnsettledClaimGate FAIL: debug surface did not resolve the unsettled minor-house levy state.");
                    return false;
                }

                string summary = readout.ToString();
                if (!summary.Contains("Status=Unsettled", StringComparison.Ordinal) ||
                    !summary.Contains("Claim=minor-unsettled-claim", StringComparison.Ordinal))
                {
                    sb.AppendLine($"PhaseUnsettledClaimGate FAIL: debug summary mismatch '{summary}'.");
                    return false;
                }

                sb.AppendLine(
                    $"PhaseUnsettledClaimGate PASS: low-loyalty claim blocks levying and debug readout reports '{summary}'.");
                return true;
            }
            finally
            {
                World.DefaultGameObjectInjectionWorld = previousDefault;
            }
        }

        private static bool RunPhasePressuredBowmanRaise(System.Text.StringBuilder sb)
        {
            using var world = new World("minor-house-phase4");
            var em = world.EntityManager;
            var simulation = SetupSimGroup(world);
            simulation.AddSystemToUpdateList(world.GetOrCreateSystem<MinorHouseLevySystem>());

            var clock = em.CreateEntity(typeof(DualClockComponent));
            em.SetComponentData(clock, new DualClockComponent { InWorldDays = 72f });

            var parent = em.CreateEntity(typeof(FactionComponent), typeof(WorldPressureComponent));
            em.SetComponentData(parent, new FactionComponent { FactionId = new FixedString32Bytes("player") });
            em.SetComponentData(parent, new WorldPressureComponent
            {
                Score = 8,
                Level = 2,
                Label = new FixedString32Bytes("Overwhelming"),
                Targeted = true,
            });

            var minor = CreateMinorFaction(
                em,
                factionId: "minor-pressure",
                originFactionId: "player",
                claimId: "minor-pressure-claim",
                levyAccumulator: 25.99f,
                levySecondsRequired: 22f,
                food: 20f,
                influence: 20f,
                populationTotal: 2,
                populationCap: 2);

            CreateClaim(
                em,
                controlPointId: "minor-pressure-claim",
                ownerFactionId: "minor-pressure",
                loyalty: 80f,
                controlState: ControlState.Stabilized,
                isContested: false,
                position: new float3(64f, 0f, 64f));

            CreateRetinueUnit(em, "minor-pressure", "militia", UnitRole.Melee, 80f, new float3(60f, 0f, 64f));
            CreateRetinueUnit(em, "minor-pressure", "swordsman", UnitRole.MeleeRecon, 100f, new float3(68f, 0f, 64f));

            world.Update();

            var levy = em.GetComponentData<MinorHouseLevyComponent>(minor);
            var resources = em.GetComponentData<ResourceStockpileComponent>(minor);
            var population = em.GetComponentData<PopulationComponent>(minor);
            Entity claimEntity = FindControlPointEntity(em, "minor-pressure-claim");
            var claim = em.GetComponentData<ControlPointComponent>(claimEntity);
            int bowmen = CountFactionUnitsByType(em, "minor-pressure", "bowman");
            int totalRetinue = CountFactionUnits(em, "minor-pressure");

            bool ok =
                levy.LevyStatus == MinorHouseLevyStatus.Raised &&
                levy.LeviesIssued == 1 &&
                levy.ParentPressureLevel == 2 &&
                levy.ParentPressureStatus == LesserHouseWorldPressureStatus.Overwhelming &&
                math.abs(levy.ParentPressureLevyTempo - 1.56f) < 0.01f &&
                levy.RetinueCap == 4 &&
                levy.RetinueCount == 3 &&
                levy.LastLevyUnitId.Equals(new FixedString64Bytes("bowman")) &&
                math.abs(resources.Food - 13f) < 0.01f &&
                math.abs(resources.Influence - 12f) < 0.01f &&
                math.abs(claim.Loyalty - 76f) < 0.01f &&
                population.Total == 3 &&
                population.Cap == 3 &&
                bowmen == 1 &&
                totalRetinue == 3;

            if (!ok)
            {
                sb.AppendLine(
                    $"PhasePressuredBowmanRaise FAIL: status={levy.LevyStatus} levies={levy.LeviesIssued} pressureLevel={levy.ParentPressureLevel} tempo={levy.ParentPressureLevyTempo} cap={levy.RetinueCap} retinue={levy.RetinueCount} lastUnit='{levy.LastLevyUnitId}' food={resources.Food} influence={resources.Influence} claimLoyalty={claim.Loyalty} population={population.Total}/{population.Cap} bowmen={bowmen} totalRetinue={totalRetinue}.");
                return false;
            }

            sb.AppendLine("PhasePressuredBowmanRaise PASS: pressured stabilized claim raises a bowman, spends food/influence, and updates retinue state.");
            return true;
        }

        private static bool RunPhaseMusterCapBlock(System.Text.StringBuilder sb)
        {
            using var world = new World("minor-house-phase5");
            var em = world.EntityManager;
            var simulation = SetupSimGroup(world);
            simulation.AddSystemToUpdateList(world.GetOrCreateSystem<MinorHouseLevySystem>());

            var minor = CreateMinorFaction(
                em,
                factionId: "minor-mustered",
                originFactionId: "player",
                claimId: "minor-mustered-claim",
                levyAccumulator: 27.99f,
                levySecondsRequired: 22f,
                food: 20f,
                influence: 20f,
                populationTotal: 3,
                populationCap: 3);

            CreateClaim(
                em,
                controlPointId: "minor-mustered-claim",
                ownerFactionId: "minor-mustered",
                loyalty: 72f,
                controlState: ControlState.Stabilized,
                isContested: false,
                position: new float3(32f, 0f, 96f));

            CreateRetinueUnit(em, "minor-mustered", "militia", UnitRole.Melee, 80f, new float3(30f, 0f, 96f));
            CreateRetinueUnit(em, "minor-mustered", "swordsman", UnitRole.MeleeRecon, 100f, new float3(34f, 0f, 96f));
            CreateRetinueUnit(em, "minor-mustered", "swordsman", UnitRole.MeleeRecon, 100f, new float3(36f, 0f, 96f));

            world.Update();

            var levy = em.GetComponentData<MinorHouseLevyComponent>(minor);
            int retinueCount = CountFactionUnits(em, "minor-mustered");

            if (levy.LevyStatus != MinorHouseLevyStatus.Mustered ||
                levy.RetinueCap != 3 ||
                levy.LeviesIssued != 0 ||
                levy.LevyAccumulator != 0f ||
                retinueCount != 3)
            {
                sb.AppendLine(
                    $"PhaseMusterCapBlock FAIL: status={levy.LevyStatus} cap={levy.RetinueCap} levies={levy.LeviesIssued} accumulator={levy.LevyAccumulator} retinue={retinueCount}.");
                return false;
            }

            sb.AppendLine("PhaseMusterCapBlock PASS: cap-hitting minor house stops levying and clears progress.");
            return true;
        }

        private static Entity CreateMinorFaction(
            EntityManager em,
            string factionId,
            string originFactionId,
            string claimId,
            float levyAccumulator,
            float levySecondsRequired,
            float food,
            float influence,
            int populationTotal = 0,
            int populationCap = 0)
        {
            var faction = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(ResourceStockpileComponent),
                typeof(PopulationComponent),
                typeof(MinorHouseLevyComponent));

            em.SetComponentData(faction, new FactionComponent { FactionId = new FixedString32Bytes(factionId) });
            em.SetComponentData(faction, new FactionKindComponent { Kind = FactionKind.MinorHouse });
            em.SetComponentData(faction, new ResourceStockpileComponent
            {
                Food = food,
                Influence = influence,
            });
            em.SetComponentData(faction, new PopulationComponent
            {
                Total = populationTotal,
                Cap = populationCap,
                BaseCap = populationCap,
                Available = math.max(0, populationCap - populationTotal),
            });
            em.SetComponentData(faction, new MinorHouseLevyComponent
            {
                OriginFactionId = new FixedString32Bytes(originFactionId),
                ClaimControlPointId = new FixedString32Bytes(claimId),
                LevyAccumulator = levyAccumulator,
                LevyIntervalSeconds = levySecondsRequired,
                LeviesIssued = 0,
                LevyStatus = MinorHouseLevyStatus.Forming,
                LevyUnitId = new FixedString64Bytes("militia"),
                RetinueCap = 1,
                RetinueCount = 0,
                LastLevyAtInWorldDays = UnsetInWorldDays,
                LastLevyUnitId = default,
                ParentPressureLevel = 0,
                ParentPressureStatus = LesserHouseWorldPressureStatus.Quiet,
                ParentPressureLevyTempo = 1f,
                ParentPressureRetakeTempo = 1f,
                ParentPressureRetinueBonus = 0,
            });
            return faction;
        }

        private static Entity CreateClaim(
            EntityManager em,
            string controlPointId,
            string ownerFactionId,
            float loyalty,
            ControlState controlState,
            bool isContested,
            float3 position)
        {
            var claim = em.CreateEntity(typeof(PositionComponent), typeof(LocalTransform), typeof(ControlPointComponent));
            em.SetComponentData(claim, new PositionComponent { Value = position });
            em.SetComponentData(claim, new LocalTransform
            {
                Position = position,
                Rotation = quaternion.identity,
                Scale = 1f,
            });
            em.SetComponentData(claim, new ControlPointComponent
            {
                ControlPointId = new FixedString32Bytes(controlPointId),
                OwnerFactionId = new FixedString32Bytes(ownerFactionId),
                CaptureFactionId = default,
                ContinentId = new FixedString32Bytes("home"),
                ControlState = controlState,
                IsContested = isContested,
                Loyalty = loyalty,
                CaptureProgress = 0f,
                SettlementClassId = new FixedString32Bytes("border_settlement"),
                FortificationTier = 0,
                RadiusTiles = 2.8f,
                CaptureTimeSeconds = 9f,
                GoldTrickle = 0f,
                FoodTrickle = 0.08f,
                WaterTrickle = 0f,
                WoodTrickle = 0f,
                StoneTrickle = 0f,
                IronTrickle = 0f,
                InfluenceTrickle = 0.06f,
            });
            return claim;
        }

        private static Entity CreateRetinueUnit(
            EntityManager em,
            string factionId,
            string unitId,
            UnitRole role,
            float maxHealth,
            float3 position)
        {
            var unit = em.CreateEntity(
                typeof(FactionComponent),
                typeof(UnitTypeComponent),
                typeof(HealthComponent),
                typeof(PositionComponent),
                typeof(LocalTransform));

            em.SetComponentData(unit, new FactionComponent { FactionId = new FixedString32Bytes(factionId) });
            em.SetComponentData(unit, new UnitTypeComponent
            {
                TypeId = new FixedString64Bytes(unitId),
                Role = role,
                SiegeClass = SiegeClass.None,
                PopulationCost = 1,
                Stage = 1,
            });
            em.SetComponentData(unit, new HealthComponent
            {
                Current = maxHealth,
                Max = maxHealth,
            });
            em.SetComponentData(unit, new PositionComponent { Value = position });
            em.SetComponentData(unit, new LocalTransform
            {
                Position = position,
                Rotation = quaternion.identity,
                Scale = 1f,
            });

            return unit;
        }

        private static Entity FindFactionEntity(EntityManager em, string factionId)
        {
            var query = em.CreateEntityQuery(ComponentType.ReadOnly<FactionComponent>());
            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            query.Dispose();

            var target = new FixedString32Bytes(factionId);
            for (int i = 0; i < factions.Length; i++)
            {
                if (factions[i].FactionId.Equals(target))
                {
                    return entities[i];
                }
            }

            return Entity.Null;
        }

        private static Entity FindControlPointEntity(EntityManager em, string controlPointId)
        {
            var query = em.CreateEntityQuery(ComponentType.ReadOnly<ControlPointComponent>());
            using var entities = query.ToEntityArray(Allocator.Temp);
            using var controlPoints = query.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);
            query.Dispose();

            var target = new FixedString32Bytes(controlPointId);
            for (int i = 0; i < controlPoints.Length; i++)
            {
                if (controlPoints[i].ControlPointId.Equals(target))
                {
                    return entities[i];
                }
            }

            return Entity.Null;
        }

        private static int CountFactionUnits(EntityManager em, string factionId)
        {
            var query = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<HealthComponent>());
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var health = query.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            query.Dispose();

            var target = new FixedString32Bytes(factionId);
            int count = 0;
            for (int i = 0; i < factions.Length; i++)
            {
                if (factions[i].FactionId.Equals(target) && health[i].Current > 0f)
                {
                    count += 1;
                }
            }

            return count;
        }

        private static int CountFactionUnitsByType(EntityManager em, string factionId, string unitId)
        {
            var query = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<HealthComponent>());
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var unitTypes = query.ToComponentDataArray<UnitTypeComponent>(Allocator.Temp);
            using var health = query.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            query.Dispose();

            var targetFaction = new FixedString32Bytes(factionId);
            var targetUnit = new FixedString64Bytes(unitId);
            int count = 0;
            for (int i = 0; i < factions.Length; i++)
            {
                if (factions[i].FactionId.Equals(targetFaction) &&
                    health[i].Current > 0f &&
                    unitTypes[i].TypeId.Equals(targetUnit))
                {
                    count += 1;
                }
            }

            return count;
        }
    }
}
#endif
