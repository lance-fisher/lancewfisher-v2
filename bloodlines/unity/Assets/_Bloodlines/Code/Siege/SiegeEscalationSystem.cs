using System.Collections.Generic;
using Bloodlines.Components;
using Bloodlines.Fortification;
using Bloodlines.GameTime;
using Bloodlines.Systems;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Siege
{
    /// <summary>
    /// Per simulation frame: for each fortified settlement with ThreatActive, increments
    /// SiegeDurationInWorldDays, advances EscalationTier at canonical thresholds, writes
    /// FactionSiegeEscalationStateComponent onto the owning faction entity (read by
    /// StarvationResponseSystem), and applies accumulated MoralePenaltyPerDay to faction
    /// loyalty.
    ///
    /// SiegeEscalationComponent is attached lazily when ThreatActive becomes true and
    /// removed when the threat clears. Duration accumulates continuously so tier advance
    /// happens naturally as in-world days accumulate.
    ///
    /// Browser equivalent: absent (tickSiegeEscalation not in simulation.js).
    /// Implemented from canonical siege doctrine.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(FortificationReserveSystem))]
    [UpdateBefore(typeof(StarvationResponseSystem))]
    public partial struct SiegeEscalationSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DualClockComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.TryGetSingleton(out DualClockComponent clock))
            {
                return;
            }

            float currentInWorldDays = clock.InWorldDays;
            var em = state.EntityManager;

            // Resolve all faction entities so we can write aggregate escalation state.
            var factionQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadWrite<FactionLoyaltyComponent>());
            using var factionEntities = factionQuery.ToEntityArray(Allocator.Temp);
            using var factionIds = factionQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var factionLoyalties = factionQuery.ToComponentDataArray<FactionLoyaltyComponent>(Allocator.Temp);
            factionQuery.Dispose();

            // Per-faction accumulators: worst starvation multiplier and total morale penalty this frame.
            var factionMaxMult = new NativeArray<float>(factionEntities.Length, Allocator.Temp);
            var factionPenaltyAccum = new NativeArray<float>(factionEntities.Length, Allocator.Temp);
            for (int i = 0; i < factionEntities.Length; i++)
            {
                factionMaxMult[i] = SiegeEscalationCanon.NormalStarvationMultiplier;
                factionPenaltyAccum[i] = 0f;
            }

            // Collect settlement-tier entities with fortification reserve state.
            var settlementQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FortificationReserveComponent>(),
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<SettlementComponent>());
            using var settlementEntities = settlementQuery.ToEntityArray(Allocator.Temp);
            using var reserves = settlementQuery.ToComponentDataArray<FortificationReserveComponent>(Allocator.Temp);
            using var settlementFactions = settlementQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            settlementQuery.Dispose();

            var toAdd = new List<(Entity entity, SiegeEscalationComponent comp)>(4);
            var toRemove = new List<Entity>(4);

            for (int i = 0; i < settlementEntities.Length; i++)
            {
                Entity settlementEntity = settlementEntities[i];
                bool siegeActive = reserves[i].ThreatActive;
                bool hasComp = em.HasComponent<SiegeEscalationComponent>(settlementEntity);

                if (!siegeActive)
                {
                    if (hasComp)
                    {
                        toRemove.Add(settlementEntity);
                    }

                    continue;
                }

                var ownerFactionId = settlementFactions[i].FactionId;
                SiegeEscalationComponent escalation = hasComp
                    ? em.GetComponentData<SiegeEscalationComponent>(settlementEntity)
                    : SiegeEscalationCanon.BuildComponent(0f, currentInWorldDays, ownerFactionId);

                float elapsed = math.max(0f, currentInWorldDays - escalation.LastTickInWorldDays);
                escalation.SiegeDurationInWorldDays += elapsed;
                escalation.LastTickInWorldDays = currentInWorldDays;
                escalation.OwnerFactionId = ownerFactionId;

                byte newTier = SiegeEscalationCanon.ResolveTier(escalation.SiegeDurationInWorldDays);
                escalation.EscalationTier = newTier;
                escalation.StarvationMultiplier = SiegeEscalationCanon.ResolveStarvationMultiplier(newTier);
                escalation.DesertionThresholdPct = SiegeEscalationCanon.ResolveDesertionThresholdPct(newTier);
                escalation.MoralePenaltyPerDay = SiegeEscalationCanon.ResolveMoralePenaltyPerDay(newTier);

                if (hasComp)
                {
                    em.SetComponentData(settlementEntity, escalation);
                }
                else
                {
                    toAdd.Add((settlementEntity, escalation));
                }

                int factionIndex = FindFactionIndex(factionIds, ownerFactionId);
                if (factionIndex >= 0)
                {
                    if (escalation.StarvationMultiplier > factionMaxMult[factionIndex])
                    {
                        factionMaxMult[factionIndex] = escalation.StarvationMultiplier;
                    }

                    factionPenaltyAccum[factionIndex] += escalation.MoralePenaltyPerDay * elapsed;
                }
            }

            for (int i = 0; i < toAdd.Count; i++)
            {
                em.AddComponentData(toAdd[i].entity, toAdd[i].comp);
            }

            for (int i = 0; i < toRemove.Count; i++)
            {
                em.RemoveComponent<SiegeEscalationComponent>(toRemove[i]);
            }

            // Write aggregate siege escalation state to faction entities and apply morale penalty.
            for (int i = 0; i < factionEntities.Length; i++)
            {
                Entity factionEntity = factionEntities[i];
                float mult = factionMaxMult[i];
                float penalty = factionPenaltyAccum[i];

                var escalationState = new FactionSiegeEscalationStateComponent
                {
                    StarvationMultiplier = mult,
                };

                if (em.HasComponent<FactionSiegeEscalationStateComponent>(factionEntity))
                {
                    em.SetComponentData(factionEntity, escalationState);
                }
                else
                {
                    em.AddComponentData(factionEntity, escalationState);
                }

                if (penalty > 0f)
                {
                    var loyalty = factionLoyalties[i];
                    float max = loyalty.Max > 0f ? loyalty.Max : 100f;
                    loyalty.Current = math.clamp(loyalty.Current - penalty, loyalty.Floor, max);
                    em.SetComponentData(factionEntity, loyalty);
                }
            }

            factionMaxMult.Dispose();
            factionPenaltyAccum.Dispose();
        }

        private static int FindFactionIndex(
            NativeArray<FactionComponent> factions,
            FixedString32Bytes factionId)
        {
            for (int i = 0; i < factions.Length; i++)
            {
                if (factions[i].FactionId.Equals(factionId))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
