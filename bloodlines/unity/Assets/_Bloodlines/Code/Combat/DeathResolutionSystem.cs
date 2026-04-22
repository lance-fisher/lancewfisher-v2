using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Systems
{
    /// <summary>
    /// First death finalization pass. Marks dead entities, clears hostile references, and
    /// releases the faction population slot consumed by dead units.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AttackResolutionSystem))]
    [UpdateAfter(typeof(ProjectileImpactSystem))]
    public partial struct DeathResolutionSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<HealthComponent>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
            var attackTargetQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<AttackTargetComponent>());

            using var attackTargetEntities = attackTargetQuery.ToEntityArray(Allocator.Temp);
            using var attackTargets = attackTargetQuery.ToComponentDataArray<AttackTargetComponent>(Allocator.Temp);
            EnsurePendingCaptureBuffers(entityManager);

            foreach (var (health, entity) in SystemAPI.Query<RefRO<HealthComponent>>()
                .WithNone<DeadTag>()
                .WithEntityAccess())
            {
                if (health.ValueRO.Current > 0f)
                {
                    continue;
                }

                TryCaptureCommander(ref state, entity);
                ecb.AddComponent<DeadTag>(entity);
                if (entityManager.HasComponent<AttackTargetComponent>(entity))
                {
                    ecb.RemoveComponent<AttackTargetComponent>(entity);
                }

                if (entityManager.HasComponent<AttackOrderComponent>(entity))
                {
                    ecb.RemoveComponent<AttackOrderComponent>(entity);
                }

                if (entityManager.HasComponent<MoveCommandComponent>(entity))
                {
                    var moveCommand = entityManager.GetComponentData<MoveCommandComponent>(entity);
                    moveCommand.IsActive = false;
                    entityManager.SetComponentData(entity, moveCommand);
                }

                for (int i = 0; i < attackTargets.Length; i++)
                {
                    if (attackTargets[i].TargetEntity == entity)
                    {
                        ecb.RemoveComponent<AttackTargetComponent>(attackTargetEntities[i]);
                        ClearCompletedExplicitAttackOrder(entityManager, attackTargetEntities[i], entity);
                    }
                }

                if (entityManager.HasComponent<UnitTypeComponent>(entity) &&
                    entityManager.HasComponent<FactionComponent>(entity))
                {
                    var unitType = entityManager.GetComponentData<UnitTypeComponent>(entity);
                    if (unitType.PopulationCost > 0 &&
                        TryFindFactionPopulation(
                            entityManager,
                            entityManager.GetComponentData<FactionComponent>(entity).FactionId,
                            out var population,
                            out var factionEntity))
                    {
                        population.Available = math.min(population.Cap, population.Available + unitType.PopulationCost);
                        entityManager.SetComponentData(factionEntity, population);
                    }
                }
            }
        }

        private static bool TryFindFactionPopulation(
            EntityManager entityManager,
            FixedString32Bytes factionId,
            out PopulationComponent population,
            out Entity factionEntity)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<PopulationComponent>());

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var populations = query.ToComponentDataArray<PopulationComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(factionId))
                {
                    continue;
                }

                population = populations[i];
                factionEntity = entities[i];
                return true;
            }

            population = default;
            factionEntity = Entity.Null;
            return false;
        }

        private static void ClearCompletedExplicitAttackOrder(
            EntityManager entityManager,
            Entity attackerEntity,
            Entity defeatedTarget)
        {
            if (!entityManager.HasComponent<AttackOrderComponent>(attackerEntity))
            {
                return;
            }

            var attackOrder = entityManager.GetComponentData<AttackOrderComponent>(attackerEntity);
            if (!attackOrder.IsActive ||
                attackOrder.IsAttackMoveActive ||
                attackOrder.ExplicitTargetEntity != defeatedTarget)
            {
                return;
            }

            attackOrder.ExplicitTargetEntity = Entity.Null;
            attackOrder.AttackMoveDestination = float3.zero;
            attackOrder.IsAttackMoveActive = false;
            attackOrder.IsActive = false;
            entityManager.SetComponentData(attackerEntity, attackOrder);

            if (!entityManager.HasComponent<MoveCommandComponent>(attackerEntity))
            {
                return;
            }

            var moveCommand = entityManager.GetComponentData<MoveCommandComponent>(attackerEntity);
            moveCommand.IsActive = false;

            if (entityManager.HasComponent<PositionComponent>(attackerEntity))
            {
                moveCommand.Destination = entityManager.GetComponentData<PositionComponent>(attackerEntity).Value;
            }

            entityManager.SetComponentData(attackerEntity, moveCommand);
        }

        private static void EnsurePendingCaptureBuffers(EntityManager entityManager)
        {
            var pendingCaptureQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<PendingCommanderCaptureComponent>());

            using var pendingCaptures = pendingCaptureQuery.ToComponentDataArray<PendingCommanderCaptureComponent>(Allocator.Temp);
            for (int i = 0; i < pendingCaptures.Length; i++)
            {
                if (!TryFindFactionEntity(entityManager, pendingCaptures[i].CaptorFactionId, out var factionEntity))
                {
                    continue;
                }

                if (!entityManager.HasBuffer<Bloodlines.AI.CapturedMemberElement>(factionEntity))
                {
                    entityManager.AddBuffer<Bloodlines.AI.CapturedMemberElement>(factionEntity);
                }
            }
        }

        private static void TryCaptureCommander(ref SystemState state, Entity defeatedEntity)
        {
            var entityManager = state.EntityManager;
            if (!entityManager.HasComponent<PendingCommanderCaptureComponent>(defeatedEntity) ||
                !entityManager.HasComponent<CommanderComponent>(defeatedEntity) ||
                !entityManager.HasComponent<FactionComponent>(defeatedEntity))
            {
                return;
            }

            var pendingCapture = entityManager.GetComponentData<PendingCommanderCaptureComponent>(defeatedEntity);
            var defeatedFaction = entityManager.GetComponentData<FactionComponent>(defeatedEntity).FactionId;
            var commander = entityManager.GetComponentData<CommanderComponent>(defeatedEntity);

            if (!TryFindDynastyMember(
                    entityManager,
                    defeatedFaction,
                    commander.MemberId,
                    out var memberEntity,
                    out var member))
            {
                return;
            }

            if (member.Status == DynastyMemberStatus.Captured ||
                member.Status == DynastyMemberStatus.Fallen)
            {
                return;
            }

            if (!TryFindFactionEntity(entityManager, pendingCapture.CaptorFactionId, out var captorFactionEntity) ||
                !entityManager.HasBuffer<Bloodlines.AI.CapturedMemberElement>(captorFactionEntity))
            {
                return;
            }

            var captiveBuffer = entityManager.GetBuffer<Bloodlines.AI.CapturedMemberElement>(captorFactionEntity);
            captiveBuffer.Add(new Bloodlines.AI.CapturedMemberElement
            {
                MemberId = member.MemberId,
                MemberTitle = member.Title,
                OriginFactionId = defeatedFaction,
                CapturedAtInWorldDays = GetInWorldDays(entityManager),
                RansomCost = 0f,
                Status = Bloodlines.AI.CapturedMemberStatus.Held,
            });

            member.Status = DynastyMemberStatus.Captured;
            entityManager.SetComponentData(memberEntity, member);
            entityManager.RemoveComponent<PendingCommanderCaptureComponent>(defeatedEntity);
        }

        private static bool TryFindDynastyMember(
            EntityManager entityManager,
            FixedString32Bytes factionId,
            FixedString64Bytes memberId,
            out Entity memberEntity,
            out DynastyMemberComponent member)
        {
            if (!TryFindFactionEntity(entityManager, factionId, out var factionEntity) ||
                !entityManager.HasBuffer<DynastyMemberRef>(factionEntity))
            {
                memberEntity = Entity.Null;
                member = default;
                return false;
            }

            var roster = entityManager.GetBuffer<DynastyMemberRef>(factionEntity);
            for (int j = 0; j < roster.Length; j++)
            {
                var candidateEntity = roster[j].Member;
                if (!entityManager.Exists(candidateEntity) ||
                    !entityManager.HasComponent<DynastyMemberComponent>(candidateEntity))
                {
                    continue;
                }

                var candidate = entityManager.GetComponentData<DynastyMemberComponent>(candidateEntity);
                if (!candidate.MemberId.Equals(memberId))
                {
                    continue;
                }

                memberEntity = candidateEntity;
                member = candidate;
                return true;
            }

            memberEntity = Entity.Null;
            member = default;
            return false;
        }

        private static bool TryFindFactionEntity(
            EntityManager entityManager,
            FixedString32Bytes factionId,
            out Entity factionEntity)
        {
            var factionQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<PopulationComponent>());
            using var factionEntities = factionQuery.ToEntityArray(Allocator.Temp);
            using var factionIds = factionQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);

            for (int i = 0; i < factionEntities.Length; i++)
            {
                if (!factionIds[i].FactionId.Equals(factionId))
                {
                    continue;
                }

                factionEntity = factionEntities[i];
                return true;
            }

            factionEntity = Entity.Null;
            return false;
        }

        private static float GetInWorldDays(EntityManager entityManager)
        {
            var clockQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<DualClockComponent>());
            if (clockQuery.IsEmpty)
            {
                return 0f;
            }

            return clockQuery.GetSingleton<DualClockComponent>().InWorldDays;
        }
    }
}
