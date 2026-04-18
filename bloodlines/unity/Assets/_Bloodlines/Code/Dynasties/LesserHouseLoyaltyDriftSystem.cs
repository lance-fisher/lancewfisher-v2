using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Dynasties
{
    /// <summary>
    /// Applies daily loyalty drift to every active lesser house in each
    /// faction's LesserHouseElement buffer, gated by DualClockComponent in-world
    /// days so exactly one drift is applied per in-world day elapsed.
    /// When loyalty drops to or below zero, marks the house as Defected and
    /// spawns a new minor-house faction entity.
    ///
    /// Structural change safety: buffer writes are completed BEFORE any entity
    /// creation so DynamicBuffer references remain valid throughout the loop.
    ///
    /// Browser reference: simulation.js tickLesserHouseLoyaltyDrift (~6631),
    /// spawnDefectedMinorFaction (~6851).
    /// Canonical base daily loyalty delta = -0.5.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct LesserHouseLoyaltyDriftSystem : ISystem
    {
        private const float BaseDailyDelta = -0.5f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<LesserHouseElement>();
            state.RequireForUpdate<DualClockComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;

            float inWorldDays = GetInWorldDays(em);

            var factionQuery    = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                typeof(LesserHouseElement));
            var factionEntities = factionQuery.ToEntityArray(Allocator.Temp);
            var factionComps    = factionQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            factionQuery.Dispose();

            // Collect defection events so we can spawn after all buffer writes.
            var defections = new NativeList<LesserHouseElement>(4, Allocator.Temp);

            for (int i = 0; i < factionEntities.Length; i++)
            {
                var buffer = em.GetBuffer<LesserHouseElement>(factionEntities[i]);
                for (int j = 0; j < buffer.Length; j++)
                {
                    var lh = buffer[j];
                    if (lh.Defected)
                        continue;

                    float daysSinceLast = inWorldDays - lh.LastDriftAppliedInWorldDays;
                    if (daysSinceLast < 1f)
                        continue;

                    float daysToApply = UnityEngine.Mathf.Floor(daysSinceLast);
                    float delta       = (lh.DailyLoyaltyDelta != 0f ? lh.DailyLoyaltyDelta : BaseDailyDelta) * daysToApply;
                    lh.Loyalty                       += delta;
                    lh.LastDriftAppliedInWorldDays    = inWorldDays - (daysSinceLast - daysToApply);

                    if (lh.Loyalty <= 0f)
                    {
                        lh.Loyalty  = 0f;
                        lh.Defected = true;
                        defections.Add(lh);
                    }

                    buffer[j] = lh;  // all buffer writes complete before entity creation
                }
            }

            factionEntities.Dispose();
            factionComps.Dispose();

            // Now safe to spawn: all DynamicBuffer references have been released.
            for (int d = 0; d < defections.Length; d++)
                SpawnDefectedMinorFaction(em, defections[d]);

            defections.Dispose();
        }

        private static void SpawnDefectedMinorFaction(EntityManager em, LesserHouseElement lh)
        {
            var minorFactionId = new FixedString32Bytes("minor-");
            minorFactionId.Append(lh.HouseId);

            var existingQuery = em.CreateEntityQuery(ComponentType.ReadOnly<FactionComponent>());
            var existing      = existingQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            existingQuery.Dispose();

            for (int k = 0; k < existing.Length; k++)
            {
                if (existing[k].FactionId.Equals(minorFactionId))
                {
                    existing.Dispose();
                    return;
                }
            }

            existing.Dispose();

            var minorEntity = em.CreateEntity();
            em.AddComponentData(minorEntity, new FactionComponent { FactionId = minorFactionId });
            em.AddComponentData(minorEntity, new FactionKindComponent { Kind = FactionKind.MinorHouse });
            em.AddComponentData(minorEntity, new AIEconomyControllerComponent { Enabled = true });
            em.AddComponentData(minorEntity, new MinorHouseLevyComponent
            {
                LevyIntervalSeconds = 60f,
                LevyAccumulator     = 0f,
                LeviesIssued        = 0,
            });
        }

        private static float GetInWorldDays(EntityManager em)
        {
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<DualClockComponent>());
            if (q.IsEmpty) { q.Dispose(); return 0f; }
            float d = q.GetSingleton<DualClockComponent>().InWorldDays;
            q.Dispose();
            return d;
        }
    }
}
