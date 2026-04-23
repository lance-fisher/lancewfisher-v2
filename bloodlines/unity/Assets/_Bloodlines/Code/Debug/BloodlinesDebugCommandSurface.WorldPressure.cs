using Bloodlines.Components;
using Bloodlines.Dynasties;
using Bloodlines.Systems;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Debug
{
    public partial class BloodlinesDebugCommandSurface
    {
        public static bool TryDebugGetWorldPressure(string factionId, out WorldPressureComponent wp)
        {
            wp = default;
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null) return false;
            var em = world.EntityManager;
            var query = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<WorldPressureComponent>());
            using var entities = query.ToEntityArray(Unity.Collections.Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Unity.Collections.Allocator.Temp);
            using var wpData = query.ToComponentDataArray<WorldPressureComponent>(Unity.Collections.Allocator.Temp);
            query.Dispose();
            for (int i = 0; i < factions.Length; i++)
            {
                if (factions[i].FactionId == new Unity.Collections.FixedString32Bytes(factionId))
                {
                    wp = wpData[i];
                    return true;
                }
            }
            return false;
        }

        public static bool TryDebugGetWorldPressureCycleTracker(out WorldPressureCycleTrackerComponent tracker)
        {
            tracker = default;
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null) return false;
            var em = world.EntityManager;
            var query = em.CreateEntityQuery(ComponentType.ReadOnly<WorldPressureCycleTrackerComponent>());
            if (query.IsEmpty) { query.Dispose(); return false; }
            tracker = query.GetSingleton<WorldPressureCycleTrackerComponent>();
            query.Dispose();
            return true;
        }

        public static bool TryDebugGetGovernanceCoalitionState(out Unity.Collections.FixedString512Bytes readout)
        {
            readout = default;
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null) return false;
            var em = world.EntityManager;
            var query = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<TerritorialGovernanceRecognitionComponent>());
            using var factions = query.ToComponentDataArray<FactionComponent>(Unity.Collections.Allocator.Temp);
            using var recognitions = query.ToComponentDataArray<TerritorialGovernanceRecognitionComponent>(Unity.Collections.Allocator.Temp);
            query.Dispose();

            int bestIndex = -1;
            float bestAcceptance = float.MinValue;
            for (int i = 0; i < factions.Length; i++)
            {
                var recognition = recognitions[i];
                if (!recognition.Active && !recognition.Completed && recognition.WorldPressureContribution <= 0)
                {
                    continue;
                }

                if (recognition.PopulationAcceptancePct >= bestAcceptance)
                {
                    bestAcceptance = recognition.PopulationAcceptancePct;
                    bestIndex = i;
                }
            }

            if (bestIndex < 0)
            {
                return false;
            }

            var faction = factions[bestIndex];
            var selected = recognitions[bestIndex];
            readout = new Unity.Collections.FixedString512Bytes(
                $"Faction={faction.FactionId}" +
                $"|Active={(selected.Active ? "true" : "false")}" +
                $"|Recognized={(selected.RecognitionEstablished ? "true" : "false")}" +
                $"|Completed={(selected.Completed ? "true" : "false")}" +
                $"|Acceptance={(int)math.round(selected.PopulationAcceptancePct)}" +
                $"|Target={(int)math.round(selected.PopulationAcceptanceTargetPct)}" +
                $"|AllianceReady={(selected.AllianceThresholdReady ? "true" : "false")}" +
                $"|AlliancePressure={(selected.AlliancePressureActive ? "true" : "false")}" +
                $"|Hostiles={selected.AlliancePressureHostileCount}" +
                $"|Cycles={selected.AlliancePressureCycles}" +
                $"|Weakest={selected.WeakestControlPointId}" +
                $"|WeakestLoyalty={(int)math.round(selected.WeakestControlPointLoyalty)}" +
                $"|WorldPressure={selected.WorldPressureContribution}");
            return true;
        }

        public bool TryDebugGetTruebornRiseArc(out Unity.Collections.FixedString512Bytes readout)
        {
            readout = default;
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null) return false;

            var entityManager = world.EntityManager;
            using var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<TruebornRiseArcComponent>());
            if (query.IsEmpty)
            {
                return false;
            }

            Entity arcEntity = query.GetSingletonEntity();
            TruebornRiseArcComponent arc = entityManager.GetComponentData<TruebornRiseArcComponent>(arcEntity);
            var recognitionSlots = entityManager.GetBuffer<TruebornRiseFactionRecognitionSlotElement>(arcEntity);
            int recognizedCount = TruebornRecognitionUtility.CountRecognized(
                recognitionSlots,
                arc.RecognizedFactionsBitmask);

            readout = new Unity.Collections.FixedString512Bytes(
                $"Stage={arc.CurrentStage}" +
                $"|StageStarted={math.round(arc.StageStartedAtInWorldDays * 100f) / 100f}" +
                $"|GlobalPressure={math.round(arc.GlobalPressurePerDay * 100f) / 100f}" +
                $"|LoyaltyErosion={math.round(arc.LoyaltyErosionPerDay * 100f) / 100f}" +
                $"|Challenge={arc.ChallengeLevel}" +
                $"|UnchallengedCycles={arc.UnchallengedCycles}" +
                $"|UltimatumTarget={(arc.UltimatumTargetFactionId.Length == 0 ? "-" : arc.UltimatumTargetFactionId)}" +
                $"|UltimatumStage={arc.UltimatumStageNumber}" +
                $"|UltimatumDeadline={math.round(arc.UltimatumDeadlineInWorldDays * 100f) / 100f}" +
                $"|RecognizedCount={recognizedCount}" +
                $"|RecognizedMask={arc.RecognizedFactionsBitmask}");
            return true;
        }

        public bool TryDebugGetTruebornUltimatumState(
            out Unity.Collections.FixedString512Bytes readout)
        {
            readout = default;
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null) return false;

            var entityManager = world.EntityManager;
            using var arcQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<TruebornRiseArcComponent>());
            if (arcQuery.IsEmpty)
            {
                return false;
            }

            Entity arcEntity = arcQuery.GetSingletonEntity();
            TruebornRiseArcComponent arc = entityManager.GetComponentData<TruebornRiseArcComponent>(arcEntity);
            float currentInWorldDays = 0f;
            using (var clockQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<Bloodlines.GameTime.DualClockComponent>()))
            {
                if (!clockQuery.IsEmpty)
                {
                    currentInWorldDays = clockQuery.GetSingleton<Bloodlines.GameTime.DualClockComponent>().InWorldDays;
                }
            }

            bool active = arc.UltimatumTargetFactionId.Length > 0;
            bool expired = active && currentInWorldDays >= arc.UltimatumDeadlineInWorldDays;
            float daysRemaining = active
                ? math.max(0f, arc.UltimatumDeadlineInWorldDays - currentInWorldDays)
                : 0f;

            readout = new Unity.Collections.FixedString512Bytes(
                $"TruebornUltimatum|Active={(active ? "true" : "false")}" +
                $"|Target={(active ? arc.UltimatumTargetFactionId.ToString() : "-")}" +
                $"|MatchStage={arc.UltimatumStageNumber}" +
                $"|IssuedAt={math.round(arc.UltimatumIssuedAtInWorldDays * 100f) / 100f}" +
                $"|Deadline={math.round(arc.UltimatumDeadlineInWorldDays * 100f) / 100f}" +
                $"|DaysRemaining={math.round(daysRemaining * 100f) / 100f}" +
                $"|Expired={(expired ? "true" : "false")}" +
                $"|LoyaltyPressure={math.round(arc.UltimatumLoyaltyPressurePerDay * 100f) / 100f}" +
                $"|LegitimacyPressure={math.round(arc.UltimatumLegitimacyPressurePerDay * 100f) / 100f}");
            return true;
        }

        public bool TryDebugSetTruebornRecognition(string factionId, bool recognized)
        {
            if (string.IsNullOrWhiteSpace(factionId))
            {
                return false;
            }

            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null) return false;

            var entityManager = world.EntityManager;
            using var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<TruebornRiseArcComponent>());
            if (query.IsEmpty)
            {
                return false;
            }

            Entity arcEntity = query.GetSingletonEntity();
            TruebornRiseArcComponent arc = entityManager.GetComponentData<TruebornRiseArcComponent>(arcEntity);
            var recognitionSlots = entityManager.GetBuffer<TruebornRiseFactionRecognitionSlotElement>(arcEntity);
            var targetFactionId = new Unity.Collections.FixedString32Bytes(factionId);
            if (!TruebornRecognitionUtility.TrySetRecognition(
                    ref arc,
                    recognitionSlots,
                    targetFactionId,
                    recognized))
            {
                return false;
            }

            entityManager.SetComponentData(arcEntity, arc);
            return true;
        }

        public bool TryDebugRecognizeTrueborn(string factionId)
        {
            if (string.IsNullOrWhiteSpace(factionId))
            {
                return false;
            }

            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null) return false;

            var entityManager = world.EntityManager;
            Entity requestEntity = entityManager.CreateEntity(typeof(PlayerTruebornRecognitionRequestComponent));
            entityManager.SetComponentData(requestEntity, new PlayerTruebornRecognitionRequestComponent
            {
                SourceFactionId = new Unity.Collections.FixedString32Bytes(factionId),
            });
            return true;
        }

        public bool TryDebugGetTruebornRecognitionState(
            string factionId,
            out Unity.Collections.FixedString512Bytes readout)
        {
            readout = default;
            if (string.IsNullOrWhiteSpace(factionId))
            {
                return false;
            }

            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null) return false;

            var entityManager = world.EntityManager;
            using var arcQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<TruebornRiseArcComponent>());
            if (arcQuery.IsEmpty)
            {
                return false;
            }

            var targetFactionId = new Unity.Collections.FixedString32Bytes(factionId);
            Entity factionEntity = FindFactionEntity(entityManager, targetFactionId);
            if (factionEntity == Entity.Null)
            {
                return false;
            }

            Entity arcEntity = arcQuery.GetSingletonEntity();
            TruebornRiseArcComponent arc = entityManager.GetComponentData<TruebornRiseArcComponent>(arcEntity);
            var recognitionSlots = entityManager.GetBuffer<TruebornRiseFactionRecognitionSlotElement>(arcEntity);
            int slotIndex = TruebornRecognitionUtility.FindRecognitionSlot(recognitionSlots, targetFactionId);
            bool recognized = TruebornRecognitionUtility.IsRecognized(
                recognitionSlots,
                arc.RecognizedFactionsBitmask,
                targetFactionId);

            float legitimacy = entityManager.HasComponent<DynastyStateComponent>(factionEntity)
                ? entityManager.GetComponentData<DynastyStateComponent>(factionEntity).Legitimacy
                : 0f;
            float influence = 0f;
            float gold = 0f;
            if (entityManager.HasComponent<ResourceStockpileComponent>(factionEntity))
            {
                var resources = entityManager.GetComponentData<ResourceStockpileComponent>(factionEntity);
                influence = resources.Influence;
                gold = resources.Gold;
            }

            float renown = entityManager.HasComponent<DynastyRenownComponent>(factionEntity)
                ? entityManager.GetComponentData<DynastyRenownComponent>(factionEntity).RenownScore
                : 0f;

            int activeCooldownCount = 0;
            if (entityManager.HasBuffer<DynastyPoliticalEventComponent>(factionEntity))
            {
                var politicalEvents = entityManager.GetBuffer<DynastyPoliticalEventComponent>(factionEntity);
                for (int i = 0; i < politicalEvents.Length; i++)
                {
                    var eventType = politicalEvents[i].EventType;
                    if (eventType.Equals(DynastyPoliticalEventTypes.CovenantTestCooldown) ||
                        eventType.Equals(DynastyPoliticalEventTypes.DivineRightFailedCooldown))
                    {
                        activeCooldownCount++;
                    }
                }
            }

            readout = new Unity.Collections.FixedString512Bytes(
                $"TruebornRecognition|Faction={targetFactionId}" +
                $"|Stage={arc.CurrentStage}" +
                $"|Recognized={(recognized ? "true" : "false")}" +
                $"|Slot={slotIndex}" +
                $"|Mask={arc.RecognizedFactionsBitmask}" +
                $"|Legitimacy={(int)math.round(legitimacy)}" +
                $"|Influence={(int)math.round(influence)}" +
                $"|Gold={(int)math.round(gold)}" +
                $"|Renown={(int)math.round(renown)}" +
                $"|Cooldowns={activeCooldownCount}");
            return true;
        }
    }
}
