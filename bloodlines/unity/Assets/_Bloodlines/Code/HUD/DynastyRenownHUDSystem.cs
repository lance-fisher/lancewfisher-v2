using Bloodlines.Components;
using Bloodlines.Dynasties;
using Bloodlines.GameTime;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.HUD
{
    /// <summary>
    /// Projects the dynasty renown/prestige runtime into a faction-scoped HUD read
    /// model with ranking, ruler identity, and succession status labels.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(DynastyRenownAccumulationSystem))]
    public partial struct DynastyRenownHUDSystem : ISystem
    {
        private const float RefreshCadenceInWorldDays = 0.25f;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DualClockComponent>();
            state.RequireForUpdate<DynastyRenownComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            float currentInWorldDays = SystemAPI.GetSingleton<DualClockComponent>().InWorldDays;

            using var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (faction, renown, entity) in SystemAPI.Query<
                         RefRO<FactionComponent>,
                         RefRO<DynastyRenownComponent>>()
                     .WithAll<DynastyStateComponent>()
                     .WithNone<DynastyRenownHUDComponent, DynastyRenownHUDRefreshComponent>()
                     .WithEntityAccess())
            {
                ecb.AddComponent(entity, new DynastyRenownHUDComponent
                {
                    FactionId = faction.ValueRO.FactionId,
                    RenownScore = renown.ValueRO.RenownScore,
                    PeakRenown = renown.ValueRO.PeakRenown,
                    ScoreToPeakRatio = 0f,
                    RenownRank = 0,
                    IsLeadingDynasty = false,
                    RulerMemberId = default,
                    RulerTitle = default,
                    Legitimacy = 0f,
                    Interregnum = false,
                    StatusLabel = default,
                    RenownBandLabel = default,
                    RenownBandColor = default,
                });
                ecb.AddComponent(entity, new DynastyRenownHUDRefreshComponent
                {
                    LastRefreshInWorldDays = float.NaN,
                });
            }

            ecb.Playback(entityManager);

            using var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<DynastyRenownComponent>(),
                ComponentType.ReadOnly<DynastyStateComponent>(),
                ComponentType.ReadWrite<DynastyRenownHUDComponent>(),
                ComponentType.ReadWrite<DynastyRenownHUDRefreshComponent>());

            using NativeArray<Entity> entities = query.ToEntityArray(Allocator.Temp);
            using NativeArray<FactionComponent> factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using NativeArray<DynastyRenownComponent> renowns = query.ToComponentDataArray<DynastyRenownComponent>(Allocator.Temp);
            using NativeArray<DynastyStateComponent> dynasties = query.ToComponentDataArray<DynastyStateComponent>(Allocator.Temp);
            using NativeArray<DynastyRenownHUDRefreshComponent> refreshes =
                query.ToComponentDataArray<DynastyRenownHUDRefreshComponent>(Allocator.Temp);

            if (entities.Length == 0)
            {
                return;
            }

            bool shouldRefresh = false;
            for (int i = 0; i < refreshes.Length; i++)
            {
                float lastRefresh = refreshes[i].LastRefreshInWorldDays;
                if (float.IsNaN(lastRefresh) || currentInWorldDays - lastRefresh >= RefreshCadenceInWorldDays)
                {
                    shouldRefresh = true;
                    break;
                }
            }

            if (!shouldRefresh)
            {
                return;
            }

            for (int i = 0; i < entities.Length; i++)
            {
                ResolveRankAndLeadership(renowns, i, out int rank, out bool isLeading);
                ResolveRulerIdentity(entityManager, entities[i], out var rulerMemberId, out var rulerTitle);
                ResolveBand(renowns[i].RenownScore, out var bandLabel, out var bandColor);

                float peak = math.max(0f, renowns[i].PeakRenown);
                float ratio = peak > 0f
                    ? math.saturate(renowns[i].RenownScore / peak)
                    : 0f;

                entityManager.SetComponentData(entities[i], new DynastyRenownHUDComponent
                {
                    FactionId = factions[i].FactionId,
                    RenownScore = renowns[i].RenownScore,
                    PeakRenown = renowns[i].PeakRenown,
                    ScoreToPeakRatio = ratio,
                    RenownRank = rank,
                    IsLeadingDynasty = isLeading,
                    RulerMemberId = rulerMemberId,
                    RulerTitle = rulerTitle,
                    Legitimacy = dynasties[i].Legitimacy,
                    Interregnum = dynasties[i].Interregnum,
                    StatusLabel = dynasties[i].Interregnum
                        ? new FixedString32Bytes("interregnum")
                        : new FixedString32Bytes("stable"),
                    RenownBandLabel = bandLabel,
                    RenownBandColor = bandColor,
                });

                entityManager.SetComponentData(entities[i], new DynastyRenownHUDRefreshComponent
                {
                    LastRefreshInWorldDays = currentInWorldDays,
                });
            }
        }

        private static void ResolveRankAndLeadership(
            NativeArray<DynastyRenownComponent> renowns,
            int index,
            out int rank,
            out bool isLeading)
        {
            rank = 1;
            isLeading = true;

            float score = renowns[index].RenownScore;
            float peak = renowns[index].PeakRenown;
            for (int i = 0; i < renowns.Length; i++)
            {
                if (i == index)
                {
                    continue;
                }

                float otherScore = renowns[i].RenownScore;
                float otherPeak = renowns[i].PeakRenown;
                bool outranks =
                    otherScore > score + 0.0001f ||
                    (math.abs(otherScore - score) <= 0.0001f &&
                     (otherPeak > peak + 0.0001f ||
                      (math.abs(otherPeak - peak) <= 0.0001f && i < index)));
                if (!outranks)
                {
                    continue;
                }

                rank++;
                isLeading = false;
            }
        }

        private static void ResolveRulerIdentity(
            EntityManager entityManager,
            Entity factionEntity,
            out FixedString64Bytes memberId,
            out FixedString64Bytes title)
        {
            memberId = default;
            title = default;

            if (!entityManager.HasBuffer<DynastyMemberRef>(factionEntity))
            {
                return;
            }

            DynamicBuffer<DynastyMemberRef> roster = entityManager.GetBuffer<DynastyMemberRef>(factionEntity);
            for (int i = 0; i < roster.Length; i++)
            {
                Entity memberEntity = roster[i].Member;
                if (memberEntity == Entity.Null ||
                    !entityManager.HasComponent<DynastyMemberComponent>(memberEntity))
                {
                    continue;
                }

                DynastyMemberComponent member = entityManager.GetComponentData<DynastyMemberComponent>(memberEntity);
                if (member.Status != DynastyMemberStatus.Ruling)
                {
                    continue;
                }

                memberId = member.MemberId;
                title = member.Title;
                return;
            }
        }

        private static void ResolveBand(
            float score,
            out FixedString32Bytes label,
            out FixedString32Bytes color)
        {
            if (score >= 45f)
            {
                label = new FixedString32Bytes("legendary");
                color = new FixedString32Bytes("gold");
                return;
            }

            if (score >= 25f)
            {
                label = new FixedString32Bytes("ascendant");
                color = new FixedString32Bytes("green");
                return;
            }

            if (score >= 10f)
            {
                label = new FixedString32Bytes("rising");
                color = new FixedString32Bytes("yellow");
                return;
            }

            label = new FixedString32Bytes("obscure");
            color = new FixedString32Bytes("red");
        }
    }
}
