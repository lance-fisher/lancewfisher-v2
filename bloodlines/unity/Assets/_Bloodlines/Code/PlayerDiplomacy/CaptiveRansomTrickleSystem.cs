using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.Dynasties;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.PlayerDiplomacy
{
    /// <summary>
    /// Browser reference: simulation.js updateCaptiveRansomTrickle
    /// (4885-4903). Kingdom factions that currently hold captives earn passive
    /// influence from each held member plus an additive Unity-side dynasty
    /// renown drift sourced from the same renown-weighted signal.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    public partial struct CaptiveRansomTrickleSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DualClockComponent>();
            state.RequireForUpdate<FactionComponent>();
            state.RequireForUpdate<FactionKindComponent>();
            state.RequireForUpdate<ResourceStockpileComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            var clock = SystemAPI.GetSingleton<DualClockComponent>();
            int currentDay = (int)math.floor(clock.InWorldDays);

            using var factionQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<FactionKindComponent>(),
                ComponentType.ReadOnly<ResourceStockpileComponent>());
            using NativeArray<Entity> factionEntities = factionQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<FactionComponent> factions =
                factionQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using NativeArray<FactionKindComponent> factionKinds =
                factionQuery.ToComponentDataArray<FactionKindComponent>(Allocator.Temp);
            using var captiveLedgerQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<CapturedMemberElement>());
            using NativeArray<Entity> captiveLedgerEntities = captiveLedgerQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<FactionComponent> captiveLedgerFactions =
                captiveLedgerQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);

            using var ecb = new EntityCommandBuffer(Allocator.Temp);

            for (int i = 0; i < factionEntities.Length; i++)
            {
                Entity factionEntity = factionEntities[i];
                if (factionKinds[i].Kind != FactionKind.Kingdom ||
                    !entityManager.HasComponent<DynastyStateComponent>(factionEntity))
                {
                    continue;
                }

                bool hasTracker = entityManager.HasComponent<CaptiveRansomTrickleComponent>(factionEntity);
                var tracker = hasTracker
                    ? entityManager.GetComponentData<CaptiveRansomTrickleComponent>(factionEntity)
                    : new CaptiveRansomTrickleComponent
                    {
                        LastAppliedInWorldDays = currentDay,
                        Initialized = 1,
                    };

                ResolveRates(
                    entityManager,
                    factionEntity,
                    factionEntities,
                    factions,
                    captiveLedgerEntities,
                    captiveLedgerFactions,
                    out int heldCaptiveCount,
                    out float influenceRatePerSecond,
                    out float dynastyRenownRatePerSecond,
                    out float highestCaptiveRenown);

                int lastAppliedDay = tracker.Initialized != 0
                    ? (int)math.floor(tracker.LastAppliedInWorldDays)
                    : currentDay;
                int elapsedWholeDays = math.max(0, currentDay - lastAppliedDay);
                float elapsedRealSeconds =
                    CaptiveRansomTrickleRules.ResolveElapsedRealSeconds(elapsedWholeDays, clock.DaysPerRealSecond);

                tracker.CurrentInfluenceRatePerSecond = influenceRatePerSecond;
                tracker.CurrentDynastyRenownRatePerSecond = dynastyRenownRatePerSecond;
                tracker.LastAppliedInfluenceDelta = 0f;
                tracker.LastAppliedDynastyRenownDelta = 0f;
                tracker.HeldCaptiveCount = heldCaptiveCount;
                tracker.HighestCaptiveRenown = highestCaptiveRenown;
                tracker.Initialized = 1;

                if (elapsedWholeDays > 0)
                {
                    tracker.LastAppliedInWorldDays = currentDay;
                }

                if (elapsedRealSeconds > 0f &&
                    heldCaptiveCount > 0 &&
                    entityManager.HasComponent<ResourceStockpileComponent>(factionEntity))
                {
                    var stockpile = entityManager.GetComponentData<ResourceStockpileComponent>(factionEntity);
                    float influenceDelta = influenceRatePerSecond * elapsedRealSeconds;
                    stockpile.Influence += influenceDelta;
                    tracker.LastAppliedInfluenceDelta = influenceDelta;
                    entityManager.SetComponentData(factionEntity, stockpile);

                    if (entityManager.HasComponent<DynastyRenownComponent>(factionEntity))
                    {
                        float renownDelta = dynastyRenownRatePerSecond * elapsedRealSeconds;
                        var renown = entityManager.GetComponentData<DynastyRenownComponent>(factionEntity);
                        renown.RenownScore += renownDelta;
                        renown.PeakRenown = math.max(renown.PeakRenown, renown.RenownScore);
                        tracker.LastAppliedDynastyRenownDelta = renownDelta;
                        entityManager.SetComponentData(factionEntity, renown);
                    }
                }

                if (hasTracker)
                {
                    entityManager.SetComponentData(factionEntity, tracker);
                }
                else
                {
                    ecb.AddComponent(factionEntity, tracker);
                }
            }

            ecb.Playback(entityManager);
        }

        private static void ResolveRates(
            EntityManager entityManager,
            Entity captorFactionEntity,
            NativeArray<Entity> factionEntities,
            NativeArray<FactionComponent> factions,
            NativeArray<Entity> captiveLedgerEntities,
            NativeArray<FactionComponent> captiveLedgerFactions,
            out int heldCaptiveCount,
            out float influenceRatePerSecond,
            out float dynastyRenownRatePerSecond,
            out float highestCaptiveRenown)
        {
            heldCaptiveCount = 0;
            influenceRatePerSecond = 0f;
            dynastyRenownRatePerSecond = 0f;
            highestCaptiveRenown = 0f;

            FixedString32Bytes captorFactionId =
                entityManager.GetComponentData<FactionComponent>(captorFactionEntity).FactionId;
            Entity captiveLedgerEntity = FindCaptiveLedgerEntity(
                entityManager,
                captorFactionId,
                captiveLedgerEntities,
                captiveLedgerFactions);
            if (captiveLedgerEntity == Entity.Null ||
                !entityManager.HasBuffer<CapturedMemberElement>(captiveLedgerEntity))
            {
                return;
            }

            DynamicBuffer<CapturedMemberElement> captives = entityManager.GetBuffer<CapturedMemberElement>(captiveLedgerEntity);
            for (int i = 0; i < captives.Length; i++)
            {
                var captive = captives[i];
                if (!IsActivelyHeld(captive.Status))
                {
                    continue;
                }

                float captiveRenown = ResolveCaptiveRenown(entityManager, factionEntities, factions, captive);
                heldCaptiveCount++;
                highestCaptiveRenown = math.max(highestCaptiveRenown, captiveRenown);
                influenceRatePerSecond += CaptiveRansomTrickleRules.ResolveInfluenceRatePerSecond(captiveRenown);
                dynastyRenownRatePerSecond +=
                    CaptiveRansomTrickleRules.ResolveDynastyRenownRatePerSecond(captiveRenown);
            }
        }

        private static float ResolveCaptiveRenown(
            EntityManager entityManager,
            NativeArray<Entity> factionEntities,
            NativeArray<FactionComponent> factions,
            in CapturedMemberElement captive)
        {
            Entity sourceFaction = FindFactionEntity(factionEntities, factions, captive.OriginFactionId);
            if (sourceFaction == Entity.Null || !entityManager.HasBuffer<DynastyMemberRef>(sourceFaction))
            {
                return CaptiveRansomTrickleRules.DefaultCaptiveRenown;
            }

            DynamicBuffer<DynastyMemberRef> roster = entityManager.GetBuffer<DynastyMemberRef>(sourceFaction);
            for (int i = 0; i < roster.Length; i++)
            {
                Entity memberEntity = roster[i].Member;
                if (memberEntity == Entity.Null ||
                    !entityManager.HasComponent<DynastyMemberComponent>(memberEntity))
                {
                    continue;
                }

                var member = entityManager.GetComponentData<DynastyMemberComponent>(memberEntity);
                if (member.MemberId.Equals(captive.MemberId))
                {
                    return math.max(0f, member.Renown);
                }
            }

            return CaptiveRansomTrickleRules.DefaultCaptiveRenown;
        }

        private static Entity FindFactionEntity(
            NativeArray<Entity> factionEntities,
            NativeArray<FactionComponent> factions,
            FixedString32Bytes factionId)
        {
            for (int i = 0; i < factions.Length; i++)
            {
                if (factions[i].FactionId.Equals(factionId))
                {
                    return factionEntities[i];
                }
            }

            return Entity.Null;
        }

        private static Entity FindCaptiveLedgerEntity(
            EntityManager entityManager,
            FixedString32Bytes factionId,
            NativeArray<Entity> captiveLedgerEntities,
            NativeArray<FactionComponent> captiveLedgerFactions)
        {
            Entity fallback = Entity.Null;
            for (int i = 0; i < captiveLedgerEntities.Length; i++)
            {
                if (!captiveLedgerFactions[i].FactionId.Equals(factionId))
                {
                    continue;
                }

                Entity candidate = captiveLedgerEntities[i];
                if (entityManager.HasComponent<FactionKindComponent>(candidate) ||
                    entityManager.HasComponent<ResourceStockpileComponent>(candidate) ||
                    entityManager.HasComponent<DynastyStateComponent>(candidate))
                {
                    return candidate;
                }

                if (fallback == Entity.Null)
                {
                    fallback = candidate;
                }
            }

            return fallback;
        }

        private static bool IsActivelyHeld(CapturedMemberStatus status)
        {
            return status == CapturedMemberStatus.Held ||
                   status == CapturedMemberStatus.RansomOffered;
        }
    }
}
