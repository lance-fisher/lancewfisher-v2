using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.AI
{
    /// <summary>
    /// Drives the tribes faction's frontier raid loop. Each tick decrements the
    /// raid timer; when it expires, the system collects all living tribes
    /// raider units (any non-worker, non-support, alive faction member with a
    /// MoveCommandComponent), picks the nearest non-tribes-owned contested
    /// control point as the raid target (falling back to the nearest enemy or
    /// player command_hall building), and issues a synchronized move command.
    ///
    /// Browser parity: src/game/core/ai.js updateNeutralAi (~3044-3141).
    /// Specifically:
    /// - line 3050 raidTimer decrement
    /// - lines 3055-3059 raider filter (role !== "worker")
    /// - lines 3075-3081 contested-march nearest-pick
    /// - line 3086 issueMoveCommand to raiders
    /// - line 3140 raidTimer reset (base 30 * world-pressure / Great Reckoning
    ///   / dark-extremes multipliers)
    ///
    /// Deferred until the supporting world state ports:
    /// - Great Reckoning multiplier (0.5) — depends on
    ///   MatchProgressionComponent.GreatReckoningActive (live in Unity).
    /// - World-pressure leader convergence multiplier — depends on
    ///   WorldPressureComponent (live in Unity).
    /// - Dark-extremes Apex Cruel multiplier (0.6) — depends on a dynasty
    ///   conviction-band aggregate that is not yet a single component query.
    ///
    /// This first slice applies the world-pressure-leader multiplier (player
    /// or enemy WorldPressure.Targeted with Level &gt;= 1, 2, or 3) plus the
    /// Great Reckoning multiplier. Dark-extremes deferred.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct TribesRaidSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TribesRaidStateComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            float dt = SystemAPI.Time.DeltaTime;
            if (dt <= 0f) return;

            var em = state.EntityManager;
            var tribesFactionId = new FixedString32Bytes("tribes");

            var tribesFactionEntity = ResolveTribesFactionEntity(em, tribesFactionId);
            if (tribesFactionEntity == Entity.Null) return;

            var raidState = em.GetComponentData<TribesRaidStateComponent>(tribesFactionEntity);
            raidState.RaidTimerSeconds = math.max(0f, raidState.RaidTimerSeconds - dt);
            if (raidState.RaidTimerSeconds > 0f)
            {
                em.SetComponentData(tribesFactionEntity, raidState);
                return;
            }

            // Collect alive tribes raiders (non-worker, non-support, has movement).
            var raiderQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<HealthComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadWrite<MoveCommandComponent>(),
                ComponentType.Exclude<DeadTag>());
            using var raiderEntities = raiderQuery.ToEntityArray(Allocator.Temp);
            using var raiderFactions = raiderQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var raiderTypes = raiderQuery.ToComponentDataArray<UnitTypeComponent>(Allocator.Temp);
            using var raiderHealth = raiderQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);

            int raiderCount = 0;
            float3 campCenter = default;
            for (int i = 0; i < raiderEntities.Length; i++)
            {
                if (!raiderFactions[i].FactionId.Equals(tribesFactionId)) continue;
                if (raiderHealth[i].Current <= 0f) continue;
                var role = raiderTypes[i].Role;
                if (role == UnitRole.Worker || role == UnitRole.Support || role == UnitRole.EngineerSpecialist) continue;
                raiderCount++;
                if (raiderCount == 1)
                {
                    campCenter = em.GetComponentData<PositionComponent>(raiderEntities[i]).Value;
                }
            }
            raiderQuery.Dispose();

            if (raiderCount == 0)
            {
                // No raiders alive — short cooldown so we re-check soon.
                raidState.RaidTimerSeconds = 24f;
                em.SetComponentData(tribesFactionEntity, raidState);
                return;
            }

            // Pick raid target: nearest non-tribes-owned control point.
            float3 target;
            bool foundTarget = TryPickRaidTarget(em, tribesFactionId, campCenter, out target);
            if (!foundTarget)
            {
                // Fallback: nearest player or enemy command_hall.
                if (!TryPickFallbackTarget(em, campCenter, out target))
                {
                    raidState.RaidTimerSeconds = ResolveNextRaidInterval(em, raidState.BaseRaidIntervalSeconds);
                    em.SetComponentData(tribesFactionEntity, raidState);
                    return;
                }
            }

            // Issue move commands to all raiders.
            var raiderQuery2 = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<HealthComponent>(),
                ComponentType.ReadWrite<MoveCommandComponent>(),
                ComponentType.Exclude<DeadTag>());
            using var entities2 = raiderQuery2.ToEntityArray(Allocator.Temp);
            using var factions2 = raiderQuery2.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var types2 = raiderQuery2.ToComponentDataArray<UnitTypeComponent>(Allocator.Temp);
            using var health2 = raiderQuery2.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            for (int i = 0; i < entities2.Length; i++)
            {
                if (!factions2[i].FactionId.Equals(tribesFactionId)) continue;
                if (health2[i].Current <= 0f) continue;
                var role = types2[i].Role;
                if (role == UnitRole.Worker || role == UnitRole.Support || role == UnitRole.EngineerSpecialist) continue;
                em.SetComponentData(entities2[i], new MoveCommandComponent
                {
                    Destination = target,
                    StoppingDistance = 0.5f,
                    IsActive = true,
                });
            }
            raiderQuery2.Dispose();

            raidState.RaidTimerSeconds = ResolveNextRaidInterval(em, raidState.BaseRaidIntervalSeconds);
            em.SetComponentData(tribesFactionEntity, raidState);
        }

        private static Entity ResolveTribesFactionEntity(EntityManager em, FixedString32Bytes tribesFactionId)
        {
            var query = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<TribesRaidStateComponent>());
            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            for (int i = 0; i < entities.Length; i++)
            {
                if (factions[i].FactionId.Equals(tribesFactionId))
                {
                    query.Dispose();
                    return entities[i];
                }
            }
            query.Dispose();
            return Entity.Null;
        }

        private static bool TryPickRaidTarget(
            EntityManager em,
            FixedString32Bytes tribesFactionId,
            float3 campCenter,
            out float3 target)
        {
            target = default;
            var cpQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<ControlPointComponent>(),
                ComponentType.ReadOnly<PositionComponent>());
            using var cpData = cpQuery.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);
            using var cpPositions = cpQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            cpQuery.Dispose();

            float bestDistSq = float.MaxValue;
            bool found = false;
            for (int i = 0; i < cpData.Length; i++)
            {
                if (cpData[i].OwnerFactionId.Equals(tribesFactionId)) continue;
                float distSq = math.distancesq(campCenter, cpPositions[i].Value);
                if (distSq < bestDistSq)
                {
                    bestDistSq = distSq;
                    target = cpPositions[i].Value;
                    found = true;
                }
            }
            return found;
        }

        private static bool TryPickFallbackTarget(EntityManager em, float3 campCenter, out float3 target)
        {
            target = default;
            var commandHallId = new FixedString64Bytes("command_hall");
            var query = em.CreateEntityQuery(
                ComponentType.ReadOnly<BuildingTypeComponent>(),
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<HealthComponent>(),
                ComponentType.ReadOnly<PositionComponent>());
            using var entities = query.ToEntityArray(Allocator.Temp);
            using var types = query.ToComponentDataArray<BuildingTypeComponent>(Allocator.Temp);
            using var health = query.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            using var positions = query.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            query.Dispose();

            float bestDistSq = float.MaxValue;
            bool found = false;
            for (int i = 0; i < entities.Length; i++)
            {
                if (!types[i].TypeId.Equals(commandHallId)) continue;
                if (health[i].Current <= 0f) continue;
                float distSq = math.distancesq(campCenter, positions[i].Value);
                if (distSq < bestDistSq)
                {
                    bestDistSq = distSq;
                    target = positions[i].Value;
                    found = true;
                }
            }
            return found;
        }

        private static float ResolveNextRaidInterval(EntityManager em, float baseIntervalSeconds)
        {
            float baseInterval = baseIntervalSeconds > 0f ? baseIntervalSeconds : 30f;

            // Browser parity (ai.js:3130-3140): world-pressure multiplier on top
            // raid timer.
            float worldPressureMultiplier = ResolveWorldPressureMultiplier(em);
            float greatReckoningMultiplier = ResolveGreatReckoningMultiplier(em);

            return baseInterval * math.min(worldPressureMultiplier, greatReckoningMultiplier);
        }

        private static float ResolveWorldPressureMultiplier(EntityManager em)
        {
            // Find the highest WorldPressure.Level among Targeted factions and
            // map to the canonical multiplier (browser ai.js:3132-3138).
            var query = em.CreateEntityQuery(
                ComponentType.ReadOnly<WorldPressureComponent>());
            using var comps = query.ToComponentDataArray<WorldPressureComponent>(Allocator.Temp);
            query.Dispose();

            int highestLevel = 0;
            for (int i = 0; i < comps.Length; i++)
            {
                if (comps[i].Targeted && comps[i].Level > highestLevel)
                {
                    highestLevel = comps[i].Level;
                }
            }

            if (highestLevel >= 3) return 0.45f;
            if (highestLevel >= 2) return 0.6f;
            if (highestLevel >= 1) return 0.75f;
            return 1f;
        }

        private static float ResolveGreatReckoningMultiplier(EntityManager em)
        {
            var query = em.CreateEntityQuery(
                ComponentType.ReadOnly<MatchProgressionComponent>());
            if (query.CalculateEntityCount() == 0)
            {
                query.Dispose();
                return 1f;
            }
            using var comps = query.ToComponentDataArray<MatchProgressionComponent>(Allocator.Temp);
            query.Dispose();
            return comps[0].GreatReckoningActive ? 0.5f : 1f;
        }
    }
}
