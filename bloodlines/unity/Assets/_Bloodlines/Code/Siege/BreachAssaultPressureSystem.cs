using System.Collections.Generic;
using Bloodlines.Components;
using Bloodlines.Fortification;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Siege
{
    /// <summary>
    /// Breach-aware assault-pressure seam for the fortification lane.
    /// Once a wall or gate is open, hostile units inside the breached settlement's
    /// threat envelope gain a bounded exploitation bonus. This is intentionally a
    /// coarse settlement-level read of OpenBreachCount until pathing and
    /// per-segment breach exploitation are ported in later slices.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(FortificationDestructionResolutionSystem))]
    [UpdateAfter(typeof(FieldWaterSupportScanSystem))]
    [UpdateBefore(typeof(FieldWaterStrainSystem))]
    public partial struct BreachAssaultPressureSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FieldWaterComponent>();
            state.RequireForUpdate<FortificationComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var breachedSettlements = new List<BreachedSettlementRecord>(8);
            foreach (var (settlement, fortification, faction, position) in
                SystemAPI.Query<
                    RefRO<SettlementComponent>,
                    RefRO<FortificationComponent>,
                    RefRO<FactionComponent>,
                    RefRO<PositionComponent>>())
            {
                if (fortification.ValueRO.OpenBreachCount <= 0)
                {
                    continue;
                }

                breachedSettlements.Add(new BreachedSettlementRecord
                {
                    SettlementId = settlement.ValueRO.SettlementId,
                    FactionId = faction.ValueRO.FactionId,
                    Position = position.ValueRO.Value,
                    ThreatRadiusTiles = fortification.ValueRO.ThreatRadiusTiles,
                    OpenBreachCount = fortification.ValueRO.OpenBreachCount,
                });
            }

            foreach (var (fieldWaterRw, faction, position, health) in
                SystemAPI.Query<
                    RefRW<FieldWaterComponent>,
                    RefRO<FactionComponent>,
                    RefRO<PositionComponent>,
                    RefRO<HealthComponent>>()
                .WithNone<DeadTag>())
            {
                ref var fieldWater = ref fieldWaterRw.ValueRW;
                ResetPressure(ref fieldWater);

                if (health.ValueRO.Current <= 0f)
                {
                    continue;
                }

                int breachedSettlementIndex = FindNearestBreachedSettlementIndex(
                    breachedSettlements,
                    faction.ValueRO.FactionId,
                    position.ValueRO.Value);
                if (breachedSettlementIndex < 0)
                {
                    continue;
                }

                var breachedSettlement = breachedSettlements[breachedSettlementIndex];
                fieldWater.BreachAssaultAdvantageActive = true;
                fieldWater.BreachOpenCount = breachedSettlement.OpenBreachCount;
                fieldWater.BreachTargetSettlementId = breachedSettlement.SettlementId;
                fieldWater.BreachAssaultAttackMultiplier =
                    SiegeSupportCanon.ResolveBreachAssaultAttackMultiplier(breachedSettlement.OpenBreachCount);
                fieldWater.BreachAssaultSpeedMultiplier =
                    SiegeSupportCanon.ResolveBreachAssaultSpeedMultiplier(breachedSettlement.OpenBreachCount);
            }
        }

        private static int FindNearestBreachedSettlementIndex(
            List<BreachedSettlementRecord> breachedSettlements,
            FixedString32Bytes unitFactionId,
            float3 unitPosition)
        {
            int bestIndex = -1;
            float bestDistanceSq = float.MaxValue;

            for (int i = 0; i < breachedSettlements.Count; i++)
            {
                var breachedSettlement = breachedSettlements[i];
                if (breachedSettlement.FactionId.Equals(unitFactionId))
                {
                    continue;
                }

                float distanceSq = math.distancesq(unitPosition, breachedSettlement.Position);
                float maxDistanceSq = breachedSettlement.ThreatRadiusTiles * breachedSettlement.ThreatRadiusTiles;
                if (distanceSq > maxDistanceSq)
                {
                    continue;
                }

                if (bestIndex >= 0 &&
                    distanceSq >= bestDistanceSq &&
                    breachedSettlement.OpenBreachCount <= breachedSettlements[bestIndex].OpenBreachCount)
                {
                    continue;
                }

                bestIndex = i;
                bestDistanceSq = distanceSq;
            }

            return bestIndex;
        }

        private static void ResetPressure(ref FieldWaterComponent fieldWater)
        {
            fieldWater.BreachAssaultAdvantageActive = false;
            fieldWater.BreachOpenCount = 0;
            fieldWater.BreachTargetSettlementId = default;
            fieldWater.BreachAssaultAttackMultiplier = 1f;
            fieldWater.BreachAssaultSpeedMultiplier = 1f;
        }

        private struct BreachedSettlementRecord
        {
            public FixedString64Bytes SettlementId;
            public FixedString32Bytes FactionId;
            public float3 Position;
            public float ThreatRadiusTiles;
            public int OpenBreachCount;
        }
    }
}
