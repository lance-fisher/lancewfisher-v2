using Bloodlines.Components;
using Bloodlines.Conviction;
using Bloodlines.GameTime;
using Bloodlines.Raids;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.AI
{
    /// <summary>
    /// Walks expired Sabotage DynastyOperationComponent entities at
    /// ResolveAtInWorldDays and applies the canonical tickDynastyOperations
    /// sabotage branch (simulation.js:5464-5514).
    ///
    /// Void path: target building no longer exists or has health <= 0 ->
    ///   finalize silently, no conviction effects.
    ///
    /// Success path (SuccessScore >= 0):
    ///   - applySabotageEffect ports (simulation.js:10992-11024):
    ///       gate_opening: health clamped to 20% of max (health floor ratio),
    ///         PlayerSabotageEffectComponent or BuildingRaidStateComponent not written
    ///         (AI lane writes to RealmConditionComponent + BuildingRaidStateComponent
    ///          for non-gate subtypes; gate_opening skips structural effect in AI lane
    ///          as no fortification breach system is wired here yet -- deferred).
    ///       fire_raising: no tick-based burn applied in AI lane (burn ticking lives
    ///         in SabotageResolutionSystem / PlayerCovertOps lane); AI lane just
    ///         confirms success and fires conviction (deferred: burn effect).
    ///       supply_poisoning: BuildingRaidStateComponent.RaidedUntil extended
    ///         (halts trickle production for the poison duration).
    ///       well_poisoning: RealmConditionComponent.WaterStrainStreak += 2.
    ///   - Conviction: Desecration+2 on source for poisoning subtypes;
    ///                 Ruthlessness+2 on source for gate_opening / fire_raising.
    ///   - Narrative: "[source] sabotage succeeds: [subtype] against [target]."
    ///
    /// Failure path (SuccessScore < 0):
    ///   - Conviction Stewardship+1 on target (simulation.js:5497-5502).
    ///   - Narrative: "A sabotage attempt against [target] was detected and foiled."
    ///
    /// Deferred vs browser parity:
    ///   - gate_opening structural health damage: HealthComponent not updated in AI
    ///     lane (building damage from gate sabotage belongs to fortification lane).
    ///   - fire_raising burn ticking: tick-damage loop lives in PlayerCovertOps lane.
    ///   - recordCounterIntelligenceInterception: PlayerCovertOps lane only.
    ///
    /// Always flips DynastyOperationComponent.Active = false.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AISabotageExecutionSystem))]
    public partial struct AISabotageResolutionSystem : ISystem
    {
        private const float PoisonDurationInWorldDays = 20f / 86400f;
        private const int   WellPoisoningWaterStrainGain = 2;
        private const float SabotageConvictionGain = 2f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DynastyOperationComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;
            float inWorldDays = GetInWorldDays(em);
            double elapsed = SystemAPI.Time.ElapsedTime;
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            try
            {
                var q = em.CreateEntityQuery(
                    ComponentType.ReadOnly<DynastyOperationComponent>(),
                    ComponentType.ReadOnly<DynastyOperationSabotageComponent>());
                var entities  = q.ToEntityArray(Allocator.Temp);
                var ops       = q.ToComponentDataArray<DynastyOperationComponent>(Allocator.Temp);
                var sabotages = q.ToComponentDataArray<DynastyOperationSabotageComponent>(Allocator.Temp);
                q.Dispose();

                for (int i = 0; i < entities.Length; i++)
                {
                    if (!ops[i].Active) continue;
                    if (ops[i].OperationKind != DynastyOperationKind.Sabotage) continue;
                    if (sabotages[i].ResolveAtInWorldDays > inWorldDays) continue;

                    ResolveOperation(em, ref ecb, ops[i], sabotages[i], elapsed);

                    var op = ops[i];
                    op.Active = false;
                    ecb.SetComponent(entities[i], op);
                }

                entities.Dispose();
                ops.Dispose();
                sabotages.Dispose();

                ecb.Playback(em);
            }
            finally
            {
                ecb.Dispose();
            }
        }

        // ------------------------------------------------------------------ resolution

        private static void ResolveOperation(
            EntityManager em,
            ref EntityCommandBuffer ecb,
            in DynastyOperationComponent op,
            in DynastyOperationSabotageComponent sab,
            double elapsed)
        {
            var sourceFactionEntity = FindFactionEntity(em, op.SourceFactionId);
            var targetFactionEntity = FindFactionEntity(em, sab.TargetFactionId);

            // Find the target building by stored entity index
            var buildingEntity = FindBuildingByIndex(em, sab.TargetBuildingEntityIndex);
            bool buildingAlive = buildingEntity != Entity.Null &&
                                 em.HasComponent<HealthComponent>(buildingEntity) &&
                                 em.GetComponentData<HealthComponent>(buildingEntity).Current > 0f;

            if (!buildingAlive)
            {
                // Void: building destroyed before resolution
                return;
            }

            if (sab.SuccessScore >= 0f)
            {
                ApplySabotageEffect(em, buildingEntity, sourceFactionEntity, targetFactionEntity,
                    sab.Subtype, elapsed);
                ApplySuccessConviction(em, sourceFactionEntity, sab.Subtype);
                PushSuccessNarrative(em, op.SourceFactionId, sab.TargetFactionId, sab.Subtype);
            }
            else
            {
                if (targetFactionEntity != Entity.Null &&
                    em.HasComponent<ConvictionComponent>(targetFactionEntity))
                {
                    var conviction = em.GetComponentData<ConvictionComponent>(targetFactionEntity);
                    ConvictionScoring.ApplyEvent(ref conviction, ConvictionBucket.Stewardship, 1f);
                    em.SetComponentData(targetFactionEntity, conviction);
                }

                PushFailureNarrative(em, op.SourceFactionId, sab.TargetFactionId);
            }
        }

        private static void ApplySabotageEffect(
            EntityManager em,
            Entity buildingEntity,
            Entity sourceFactionEntity,
            Entity targetFactionEntity,
            FixedString32Bytes subtype,
            double elapsed)
        {
            var supplyPoisoning = new FixedString32Bytes("supply_poisoning");
            var wellPoisoning   = new FixedString32Bytes("well_poisoning");

            if (subtype.Equals(supplyPoisoning))
            {
                // Halt production: extend BuildingRaidStateComponent (mirrors SabotageResolutionSystem.UpsertRaidState)
                double poisonedUntil = elapsed + 20d;
                var raidState = em.HasComponent<BuildingRaidStateComponent>(buildingEntity)
                    ? em.GetComponentData<BuildingRaidStateComponent>(buildingEntity)
                    : default;

                raidState.RaidedUntil = math.max(raidState.RaidedUntil, poisonedUntil);
                raidState.LastRaidedAt = math.max(raidState.LastRaidedAt, poisonedUntil - 20d);
                if (sourceFactionEntity != Entity.Null &&
                    em.HasComponent<FactionComponent>(sourceFactionEntity))
                {
                    raidState.RaidedByFactionId =
                        em.GetComponentData<FactionComponent>(sourceFactionEntity).FactionId;
                }

                if (em.HasComponent<BuildingRaidStateComponent>(buildingEntity))
                    em.SetComponentData(buildingEntity, raidState);
                else
                    em.AddComponentData(buildingEntity, raidState);
                return;
            }

            if (subtype.Equals(wellPoisoning))
            {
                if (targetFactionEntity != Entity.Null &&
                    em.HasComponent<RealmConditionComponent>(targetFactionEntity))
                {
                    var realm = em.GetComponentData<RealmConditionComponent>(targetFactionEntity);
                    realm.WaterStrainStreak += WellPoisoningWaterStrainGain;
                    em.SetComponentData(targetFactionEntity, realm);
                }
                return;
            }

            // gate_opening and fire_raising: structural + burn effects deferred to
            // fortification and PlayerCovertOps lanes respectively. Conviction and
            // narrative are applied by the caller.
        }

        // ------------------------------------------------------------------ conviction

        private static void ApplySuccessConviction(
            EntityManager em,
            Entity sourceFactionEntity,
            FixedString32Bytes subtype)
        {
            if (sourceFactionEntity == Entity.Null) return;
            if (!em.HasComponent<ConvictionComponent>(sourceFactionEntity)) return;

            var bucket = IsDesecrationSubtype(subtype)
                ? ConvictionBucket.Desecration
                : ConvictionBucket.Ruthlessness;

            var conviction = em.GetComponentData<ConvictionComponent>(sourceFactionEntity);
            ConvictionScoring.ApplyEvent(ref conviction, bucket, SabotageConvictionGain);
            em.SetComponentData(sourceFactionEntity, conviction);
        }

        private static bool IsDesecrationSubtype(FixedString32Bytes subtype)
        {
            return subtype.Equals(new FixedString32Bytes("well_poisoning")) ||
                   subtype.Equals(new FixedString32Bytes("supply_poisoning"));
        }

        // ------------------------------------------------------------------ narrative

        private static void PushSuccessNarrative(
            EntityManager em,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            FixedString32Bytes subtype)
        {
            var message = new FixedString128Bytes();
            message.Append(sourceFactionId);
            message.Append((FixedString32Bytes)" sabotage succeeds: ");
            message.Append(subtype);
            message.Append((FixedString32Bytes)" against ");
            message.Append(targetFactionId);
            message.Append((FixedString32Bytes)".");

            var playerId = new FixedString32Bytes("player");
            NarrativeMessageTone tone;
            if (sourceFactionId.Equals(playerId))
                tone = NarrativeMessageTone.Good;
            else if (targetFactionId.Equals(playerId))
                tone = NarrativeMessageTone.Warn;
            else
                tone = NarrativeMessageTone.Info;

            NarrativeMessageBridge.Push(em, message, tone);
        }

        private static void PushFailureNarrative(
            EntityManager em,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId)
        {
            var message = new FixedString128Bytes();
            message.Append((FixedString64Bytes)"A sabotage attempt against ");
            message.Append(targetFactionId);
            message.Append((FixedString64Bytes)" was detected and foiled.");

            var playerId = new FixedString32Bytes("player");
            NarrativeMessageTone tone;
            if (sourceFactionId.Equals(playerId))
                tone = NarrativeMessageTone.Warn;
            else if (targetFactionId.Equals(playerId))
                tone = NarrativeMessageTone.Good;
            else
                tone = NarrativeMessageTone.Info;

            NarrativeMessageBridge.Push(em, message, tone);
        }

        // ------------------------------------------------------------------ helpers

        private static Entity FindBuildingByIndex(EntityManager em, int entityIndex)
        {
            if (entityIndex < 0) return Entity.Null;

            var q = em.CreateEntityQuery(
                ComponentType.ReadOnly<BuildingTypeComponent>(),
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<HealthComponent>());
            if (q.IsEmpty) { q.Dispose(); return Entity.Null; }

            var entities = q.ToEntityArray(Allocator.Temp);
            q.Dispose();

            Entity match = Entity.Null;
            for (int i = 0; i < entities.Length; i++)
            {
                if (entities[i].Index == entityIndex) { match = entities[i]; break; }
            }
            entities.Dispose();
            return match;
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
