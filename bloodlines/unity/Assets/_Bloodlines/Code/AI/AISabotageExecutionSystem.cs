using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.AI
{
    /// <summary>
    /// Consumes AICovertOpsComponent.LastFiredOp == CovertOpKind.Sabotage
    /// (written by AICovertOpsSystem) and executes the browser-side
    /// startSabotageOperation path (simulation.js:10952-10990).
    ///
    /// Browser dispatch site: ai.js ~2321-2370, source "enemy", target "player".
    /// Budget gate: gold >= 60 AND influence >= 12 (minimum across all subtypes).
    /// Target selection (pickAiSabotageTarget ~2946-2979): supply_camp first, then
    /// gatehouse, then well, then farm or barracks (fire_raising fallback).
    ///
    /// Subtype costs (SABOTAGE_COSTS, simulation.js:9739-9744):
    ///   gate_opening:    gold 60, influence 18, duration 28
    ///   fire_raising:    gold 40, influence 12, duration 24
    ///   supply_poisoning: gold 50, influence 15, duration 30
    ///   well_poisoning:  gold 70, influence 20, duration 32
    ///
    /// Contest formula (getSabotageTerms, simulation.js:9940-9942):
    ///   offenseScore = operatorRenown + 45 (base) + 0 (intelligenceSupportBonus, AI has no dossier)
    ///   defenseScore = fortTier * 12 + 0 (wardActive, not yet ported) + (10 if target spymaster present)
    ///   Unity simplifications (deferred): wardActive, intelligenceSupportBonus.
    ///
    /// Gates from getSabotageTerms (simulation.js:9900-9957):
    ///   1. source != target faction.
    ///   2. Target faction exists with at least one live building.
    ///   3. Source has a spymaster-class member (spymaster/diplomat/merchant).
    ///   4. Source resources meet subtype-specific cost.
    ///   5. DynastyOperationLimits.HasCapacity.
    ///
    /// Always clears LastFiredOp to None after processing.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AICovertOpsSystem))]
    public partial struct AISabotageExecutionSystem : ISystem
    {
        // Subtype costs (simulation.js SABOTAGE_COSTS)
        private const float GateOpeningGold      = 60f;
        private const float GateOpeningInfluence = 18f;
        private const float GateOpeningDuration  = 28f;

        private const float FireRaisingGold      = 40f;
        private const float FireRaisingInfluence = 12f;
        private const float FireRaisingDuration  = 24f;

        private const float SupplyPoisoningGold      = 50f;
        private const float SupplyPoisoningInfluence = 15f;
        private const float SupplyPoisoningDuration  = 30f;

        private const float WellPoisoningGold      = 70f;
        private const float WellPoisoningInfluence = 20f;
        private const float WellPoisoningDuration  = 32f;

        private const float BaseOffenseBonus = 45f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AICovertOpsComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;
            float inWorldDays = GetInWorldDays(em);

            var dispatchQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadWrite<AICovertOpsComponent>());
            var dispatchEntities = dispatchQuery.ToEntityArray(Allocator.Temp);
            dispatchQuery.Dispose();

            for (int i = 0; i < dispatchEntities.Length; i++)
            {
                var sourceEntity = dispatchEntities[i];
                var covert = em.GetComponentData<AICovertOpsComponent>(sourceEntity);
                if (covert.LastFiredOp != CovertOpKind.Sabotage)
                    continue;

                var sourceFaction = em.GetComponentData<FactionComponent>(sourceEntity);
                var targetFactionId = new FixedString32Bytes("player");

                TryDispatchSabotage(em, sourceEntity, sourceFaction.FactionId,
                    targetFactionId, inWorldDays);

                covert.LastFiredOp = CovertOpKind.None;
                em.SetComponentData(sourceEntity, covert);
            }

            dispatchEntities.Dispose();
        }

        // ------------------------------------------------------------------ dispatch

        private static void TryDispatchSabotage(
            EntityManager em,
            Entity sourceEntity,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            float inWorldDays)
        {
            if (sourceFactionId.Equals(targetFactionId)) return;

            var targetEntity = FindFactionEntity(em, targetFactionId);
            if (targetEntity == Entity.Null) return;

            if (!TryGetSpymasterOperator(em, sourceEntity,
                    out var operatorMemberId, out var operatorTitle, out var operatorRenown))
                return;

            if (!em.HasComponent<ResourceStockpileComponent>(sourceEntity)) return;
            var resources = em.GetComponentData<ResourceStockpileComponent>(sourceEntity);

            // Minimum budget gate (hasBudget check from ai.js 2343)
            if (resources.Gold < FireRaisingGold || resources.Influence < FireRaisingInfluence) return;

            if (!TryPickSabotageTarget(em, targetFactionId,
                    out var subtype, out var buildingTypeId, out var buildingEntityIndex))
                return;

            GetSubtypeCosts(subtype, out float goldCost, out float influenceCost, out float duration);

            if (resources.Gold < goldCost || resources.Influence < influenceCost) return;

            if (!DynastyOperationLimits.HasCapacity(em, sourceFactionId)) return;

            // ------------------------------------------------------------------ contest

            float targetSpymasterRenown = GetTargetSpymasterRenown(em, targetEntity);
            float keepTier = GetHighestKeepTier(em, targetFactionId);

            float offenseScore = operatorRenown + BaseOffenseBonus;
            float defenseScore = keepTier * 12f + (targetSpymasterRenown > 0f ? 10f : 0f);
            float successScore = offenseScore - defenseScore;
            float projectedChance = math.clamp(0.5f + successScore / 100f, 0.05f, 0.95f);

            // ------------------------------------------------------------------ effects

            resources.Gold      -= goldCost;
            resources.Influence -= influenceCost;
            em.SetComponentData(sourceEntity, resources);

            var operationId = BuildOperationId(sourceFactionId, targetFactionId, subtype, inWorldDays);
            var entity = DynastyOperationLimits.BeginOperation(
                em,
                operationId,
                sourceFactionId,
                DynastyOperationKind.Sabotage,
                targetFactionId,
                default);

            em.AddComponentData(entity, new DynastyOperationSabotageComponent
            {
                TargetFactionId            = targetFactionId,
                Subtype                    = subtype,
                TargetBuildingTypeId       = buildingTypeId,
                TargetBuildingEntityIndex  = buildingEntityIndex,
                OperatorMemberId           = operatorMemberId,
                OperatorTitle              = operatorTitle,
                ResolveAtInWorldDays       = inWorldDays + duration,
                SuccessScore               = successScore,
                ProjectedChance            = projectedChance,
                EscrowGold                 = goldCost,
                EscrowInfluence            = influenceCost,
            });

            PushDispatchMessage(em, sourceFactionId, targetFactionId, subtype);
        }

        // ------------------------------------------------------------------ target selection

        // Mirrors pickAiSabotageTarget (ai.js:2946-2979):
        //   supply_camp -> supply_poisoning
        //   gatehouse   -> gate_opening
        //   well        -> well_poisoning
        //   farm        -> fire_raising
        //   barracks    -> fire_raising
        private static bool TryPickSabotageTarget(
            EntityManager em,
            FixedString32Bytes targetFactionId,
            out FixedString32Bytes subtype,
            out FixedString64Bytes buildingTypeId,
            out int buildingEntityIndex)
        {
            subtype            = default;
            buildingTypeId     = default;
            buildingEntityIndex = -1;

            var q = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<BuildingTypeComponent>(),
                ComponentType.ReadOnly<HealthComponent>());
            if (q.IsEmpty) { q.Dispose(); return false; }

            var entities      = q.ToEntityArray(Allocator.Temp);
            var factions      = q.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            var buildingTypes = q.ToComponentDataArray<BuildingTypeComponent>(Allocator.Temp);
            var healths       = q.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            q.Dispose();

            // Priority order: supply_camp, gatehouse, well, farm/barracks
            int supplyCampIdx   = -1;
            int gatehouseIdx    = -1;
            int wellIdx         = -1;
            int farmIdx         = -1;
            int barracksIdx     = -1;

            var supplyCampId  = new FixedString64Bytes("supply_camp");
            var gatehouseId   = new FixedString64Bytes("gatehouse");
            var wellId        = new FixedString64Bytes("well");
            var farmId        = new FixedString64Bytes("farm");
            var barracksId    = new FixedString64Bytes("barracks");

            for (int i = 0; i < entities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(targetFactionId)) continue;
                if (healths[i].Current <= 0f) continue;

                var typeId = buildingTypes[i].TypeId;
                if (typeId.Equals(supplyCampId)  && supplyCampIdx  < 0) supplyCampIdx  = i;
                else if (typeId.Equals(gatehouseId)   && gatehouseIdx   < 0) gatehouseIdx   = i;
                else if (typeId.Equals(wellId)         && wellIdx         < 0) wellIdx         = i;
                else if (typeId.Equals(farmId)         && farmIdx         < 0) farmIdx         = i;
                else if (typeId.Equals(barracksId)     && barracksIdx     < 0) barracksIdx     = i;
            }

            int pickedIdx = -1;
            FixedString32Bytes pickedSubtype = default;
            FixedString64Bytes pickedTypeId  = default;

            if (supplyCampIdx >= 0)
            {
                pickedIdx     = supplyCampIdx;
                pickedSubtype = new FixedString32Bytes("supply_poisoning");
                pickedTypeId  = supplyCampId;
            }
            else if (gatehouseIdx >= 0)
            {
                pickedIdx     = gatehouseIdx;
                pickedSubtype = new FixedString32Bytes("gate_opening");
                pickedTypeId  = gatehouseId;
            }
            else if (wellIdx >= 0)
            {
                pickedIdx     = wellIdx;
                pickedSubtype = new FixedString32Bytes("well_poisoning");
                pickedTypeId  = wellId;
            }
            else if (farmIdx >= 0)
            {
                pickedIdx     = farmIdx;
                pickedSubtype = new FixedString32Bytes("fire_raising");
                pickedTypeId  = farmId;
            }
            else if (barracksIdx >= 0)
            {
                pickedIdx     = barracksIdx;
                pickedSubtype = new FixedString32Bytes("fire_raising");
                pickedTypeId  = barracksId;
            }

            bool found = pickedIdx >= 0;
            if (found)
            {
                subtype             = pickedSubtype;
                buildingTypeId      = pickedTypeId;
                buildingEntityIndex = entities[pickedIdx].Index;
            }

            entities.Dispose();
            factions.Dispose();
            buildingTypes.Dispose();
            healths.Dispose();
            return found;
        }

        private static void GetSubtypeCosts(
            FixedString32Bytes subtype,
            out float goldCost,
            out float influenceCost,
            out float duration)
        {
            if (subtype.Equals(new FixedString32Bytes("gate_opening")))
            {
                goldCost = GateOpeningGold; influenceCost = GateOpeningInfluence; duration = GateOpeningDuration;
            }
            else if (subtype.Equals(new FixedString32Bytes("supply_poisoning")))
            {
                goldCost = SupplyPoisoningGold; influenceCost = SupplyPoisoningInfluence; duration = SupplyPoisoningDuration;
            }
            else if (subtype.Equals(new FixedString32Bytes("well_poisoning")))
            {
                goldCost = WellPoisoningGold; influenceCost = WellPoisoningInfluence; duration = WellPoisoningDuration;
            }
            else
            {
                // fire_raising (and fallback)
                goldCost = FireRaisingGold; influenceCost = FireRaisingInfluence; duration = FireRaisingDuration;
            }
        }

        // ------------------------------------------------------------------ narrative

        private static void PushDispatchMessage(
            EntityManager em,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            FixedString32Bytes subtype)
        {
            var message = new FixedString128Bytes();
            message.Append(sourceFactionId);
            message.Append((FixedString32Bytes)" dispatches saboteurs to ");
            message.Append(targetFactionId);
            message.Append((FixedString32Bytes)": ");
            message.Append(subtype);
            message.Append((FixedString32Bytes)".");

            var playerId = new FixedString32Bytes("player");
            var tone = targetFactionId.Equals(playerId) ? NarrativeMessageTone.Warn : NarrativeMessageTone.Info;
            NarrativeMessageBridge.Push(em, message, tone);
        }

        // ------------------------------------------------------------------ helpers

        private static bool TryGetSpymasterOperator(
            EntityManager em,
            Entity factionEntity,
            out FixedString64Bytes operatorMemberId,
            out FixedString64Bytes operatorTitle,
            out float operatorRenown)
        {
            operatorMemberId = default;
            operatorTitle    = default;
            operatorRenown   = 0f;

            if (!em.HasBuffer<DynastyMemberRef>(factionEntity)) return false;
            var roster = em.GetBuffer<DynastyMemberRef>(factionEntity);

            for (int i = 0; i < roster.Length; i++)
            {
                var memberEntity = roster[i].Member;
                if (memberEntity == Entity.Null) continue;
                if (!em.HasComponent<DynastyMemberComponent>(memberEntity)) continue;

                var member = em.GetComponentData<DynastyMemberComponent>(memberEntity);
                if (member.Role != DynastyRole.Spymaster &&
                    member.Role != DynastyRole.Diplomat  &&
                    member.Role != DynastyRole.Merchant) continue;
                if (member.Status == DynastyMemberStatus.Fallen ||
                    member.Status == DynastyMemberStatus.Captured) continue;

                operatorMemberId = member.MemberId;
                operatorTitle    = member.Title;
                operatorRenown   = member.Renown;
                return true;
            }
            return false;
        }

        private static float GetTargetSpymasterRenown(EntityManager em, Entity targetEntity)
        {
            if (!em.HasBuffer<DynastyMemberRef>(targetEntity)) return 0f;
            var roster = em.GetBuffer<DynastyMemberRef>(targetEntity);

            for (int i = 0; i < roster.Length; i++)
            {
                var memberEntity = roster[i].Member;
                if (memberEntity == Entity.Null) continue;
                if (!em.HasComponent<DynastyMemberComponent>(memberEntity)) continue;

                var member = em.GetComponentData<DynastyMemberComponent>(memberEntity);
                if (member.Role != DynastyRole.Spymaster && member.Role != DynastyRole.Diplomat) continue;
                if (member.Status == DynastyMemberStatus.Fallen ||
                    member.Status == DynastyMemberStatus.Captured) continue;

                return member.Renown;
            }
            return 0f;
        }

        private static float GetHighestKeepTier(EntityManager em, FixedString32Bytes factionId)
        {
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<ControlPointComponent>());
            if (q.IsEmpty) { q.Dispose(); return 0f; }

            var cps = q.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);
            q.Dispose();

            float highest = 0f;
            for (int i = 0; i < cps.Length; i++)
            {
                if (!cps[i].OwnerFactionId.Equals(factionId)) continue;
                if (cps[i].FortificationTier > highest)
                    highest = cps[i].FortificationTier;
            }
            cps.Dispose();
            return highest;
        }

        private static Entity FindFactionEntity(EntityManager em, FixedString32Bytes factionId)
        {
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<FactionComponent>());
            var entities = q.ToEntityArray(Allocator.Temp);
            var factions = q.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            q.Dispose();

            Entity match = Entity.Null;
            for (int i = 0; i < entities.Length; i++)
            {
                if (factions[i].FactionId.Equals(factionId)) { match = entities[i]; break; }
            }
            entities.Dispose();
            factions.Dispose();
            return match;
        }

        private static FixedString64Bytes BuildOperationId(
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            FixedString32Bytes subtype,
            float inWorldDays)
        {
            var id = new FixedString64Bytes("dynop-sab-");
            id.Append(sourceFactionId);
            id.Append("-");
            id.Append(targetFactionId);
            id.Append("-");
            id.Append(subtype);
            id.Append("-d");
            id.Append((int)inWorldDays);
            return id;
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
