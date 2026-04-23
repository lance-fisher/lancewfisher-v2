using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Faith
{
    /// <summary>
    /// Browser reference: simulation.js getFaithStructureIntensityRegenRate
    /// (823-827) and updateFaithStructureIntensity (8228-8238). Completed
    /// faith buildings contribute a per-second regen bonus that is capped and
    /// then folded into committed faction intensity on whole in-world days.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(Bloodlines.Systems.SkirmishBootstrapSystem))]
    [UpdateBefore(typeof(FaithIntensityResolveSystem))]
    public partial struct FaithStructureRegenSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DualClockComponent>();
            state.RequireForUpdate<FactionComponent>();
            state.RequireForUpdate<FaithStateComponent>();
            state.RequireForUpdate<BuildingTypeComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            var clock = SystemAPI.GetSingleton<DualClockComponent>();
            int currentDay = (int)math.floor(clock.InWorldDays);

            using var factionQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<FaithStateComponent>());
            using var buildingQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<BuildingTypeComponent>(),
                ComponentType.ReadOnly<HealthComponent>(),
                ComponentType.Exclude<ConstructionStateComponent>());

            using var factionEntities = factionQuery.ToEntityArray(Allocator.Temp);
            using var factionIds = factionQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var faithStates = factionQuery.ToComponentDataArray<FaithStateComponent>(Allocator.Temp);
            using var buildingFactions = buildingQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var buildingTypes = buildingQuery.ToComponentDataArray<BuildingTypeComponent>(Allocator.Temp);
            using var buildingHealths = buildingQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);

            for (int i = 0; i < factionEntities.Length; i++)
            {
                Entity factionEntity = factionEntities[i];
                var tracker = EnsureTracker(entityManager, factionEntity, currentDay);
                var faithState = faithStates[i];

                int buildingCount = 0;
                float regenRatePerSecond = 0f;
                if (faithState.SelectedFaith != CovenantId.None)
                {
                    regenRatePerSecond = ResolveRegenRate(
                        factionIds[i].FactionId,
                        buildingFactions,
                        buildingTypes,
                        buildingHealths,
                        out buildingCount);
                }

                int lastAppliedDay = tracker.Initialized != 0
                    ? (int)math.floor(tracker.LastAppliedInWorldDays)
                    : currentDay;
                int elapsedDays = math.max(0, currentDay - lastAppliedDay);

                tracker.CurrentRatePerSecond = regenRatePerSecond;
                tracker.FaithBuildingCount = buildingCount;
                tracker.LastAppliedIntensityDelta = 0f;
                tracker.Initialized = 1;

                if (elapsedDays > 0)
                {
                    tracker.LastAppliedInWorldDays = currentDay;
                }

                if (faithState.SelectedFaith != CovenantId.None && regenRatePerSecond > 0f && elapsedDays > 0)
                {
                    float intensityGain = FaithStructureRegenRules.ResolveIntensityGain(
                        regenRatePerSecond,
                        elapsedDays,
                        clock.DaysPerRealSecond);
                    if (intensityGain > 0f)
                    {
                        faithState.Intensity += intensityGain;
                        tracker.LastAppliedIntensityDelta = intensityGain;
                        entityManager.SetComponentData(factionEntity, faithState);
                    }
                }

                entityManager.SetComponentData(factionEntity, tracker);
            }
        }

        private static FaithStructureRegenComponent EnsureTracker(
            EntityManager entityManager,
            Entity factionEntity,
            int currentDay)
        {
            if (entityManager.HasComponent<FaithStructureRegenComponent>(factionEntity))
            {
                return entityManager.GetComponentData<FaithStructureRegenComponent>(factionEntity);
            }

            var tracker = new FaithStructureRegenComponent
            {
                LastAppliedInWorldDays = currentDay,
                CurrentRatePerSecond = 0f,
                LastAppliedIntensityDelta = 0f,
                FaithBuildingCount = 0,
                Initialized = 1,
            };
            entityManager.AddComponentData(factionEntity, tracker);
            return tracker;
        }

        private static float ResolveRegenRate(
            FixedString32Bytes factionId,
            NativeArray<FactionComponent> buildingFactions,
            NativeArray<BuildingTypeComponent> buildingTypes,
            NativeArray<HealthComponent> buildingHealths,
            out int buildingCount)
        {
            float total = 0f;
            buildingCount = 0;

            for (int i = 0; i < buildingTypes.Length; i++)
            {
                if (!buildingFactions[i].FactionId.Equals(factionId) ||
                    buildingHealths[i].Current <= 0f ||
                    !FaithStructureRegenRules.TryResolveRatePerSecond(buildingTypes[i], out float ratePerSecond))
                {
                    continue;
                }

                total += ratePerSecond;
                buildingCount++;
            }

            return math.min(FaithStructureRegenRules.MaxRatePerSecond, total);
        }
    }
}
