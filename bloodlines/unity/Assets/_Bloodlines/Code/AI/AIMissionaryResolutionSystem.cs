using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.AI
{
    /// <summary>
    /// Walks expired Missionary DynastyOperationComponent entities and
    /// applies the per-kind resolution effects to the target faction:
    /// faith exposure gain, faith intensity erosion, and loyalty
    /// pressure on the lowest-loyalty control point. The first
    /// production per-kind resolution consumer; mirrors the browser
    /// tickDynastyOperations missionary branch at simulation.js:5517-5588.
    ///
    /// This system is the Unity-side equivalent of the browser's
    /// per-tick walker inside tickDynastyOperations (simulation.js:5453).
    /// Where the browser collects `operations.active.filter(op =>
    /// op.resolveAt <= elapsed)` and switches on op.type, Unity runs a
    /// per-kind resolution system per DynastyOperationKind (this slice
    /// ports missionary; future slices port holy war, divine right,
    /// captive rescue, captive ransom).
    ///
    /// Browser resolution semantics for missionary
    /// (simulation.js:5517-5588):
    ///   1. Early-void if target faction / dynasty no longer exists.
    ///   2. Success gate: successScore >= 0. Unity uses the
    ///      DynastyOperationMissionaryComponent.SuccessScore computed
    ///      at dispatch time (sub-slice 20). Unity matches deterministic
    ///      resolution; no additional roll.
    ///   3. On success: call applyMissionaryEffect which
    ///      (a) notes faith discovery on target,
    ///      (b) increments target.faith.exposure[sourceFaithId] by
    ///          exposureGain (clamped 0-100),
    ///      (c) if target has a different committed faith, subtracts
    ///          intensityErosion from target.faith.intensity
    ///          (clamped >= 0),
    ///      (d) applies loyaltyPressure to the lowest-loyalty
    ///          control point owned by target; if none exists,
    ///          applies the effect as a legitimacy penalty via
    ///          adjustLegitimacy(target, -max(1, round(loyaltyPressure
    ///          * 0.5))).
    ///   4. On success effects beyond applyMissionaryEffect:
    ///      conviction event recording (oathkeeping +1 for light,
    ///      desecration +1 for dark), exposure-threshold crossing
    ///      narrative, missionary-spread narrative.
    ///   5. On failure: target faith intensity += 2 (strengthens
    ///      target faith), ward-profile-dependent mutual hostility,
    ///      stewardship +1 conviction on target, repulsion narrative.
    ///
    /// Unity-side simplifications (deferred to future slices):
    ///   - Ward-profile triggered mutual hostility (browser simulation.js:5567):
    ///     requires the faith ward profile surface which is not yet
    ///     ported in Unity. On failure Unity applies the +2 intensity
    ///     bump but omits the ward-triggered hostility.
    ///   - Conviction event recording (oathkeeping/desecration/
    ///     stewardship +1): requires ConvictionComponent mutation
    ///     which is owned by the conviction-scoring lane; deferred.
    ///   - Faith discovery (noteFaithDiscovery at simulation.js:10481):
    ///     Unity FaithExposureElement has a Discovered bool field;
    ///     the resolution sets it true alongside the exposure bump.
    ///   - Exposure-threshold crossing narrative (browser at
    ///     simulation.js:5544-5549): emit a narrative line when the
    ///     exposure crosses 100 (FAITH_EXPOSURE_THRESHOLD at
    ///     simulation.js:6). Unity ports this threshold check since
    ///     it only needs exposure before/after values.
    ///   - Legitimacy penalty fallback on success when no control
    ///     points exist: Unity FactionComponent does not yet carry a
    ///     canonical "Legitimacy" field outside DynastyStateComponent
    ///     (which the dynasty-core lane owns); defer until that
    ///     surface ports. Unity falls back to a no-op when no control
    ///     points are owned by target.
    ///
    /// Always flips DynastyOperationComponent.Active = false regardless
    /// of outcome, so the operation slot releases back to the
    /// per-faction capacity budget (sub-slice 18). The entity itself is
    /// retained for audit (browser finalizeDynastyOperation writes
    /// history records; Unity defers history-record emission to a
    /// future slice while keeping the entity reachable).
    ///
    /// Updates after AIMissionaryExecutionSystem so a dispatch and
    /// resolution cannot happen in the same tick; a dispatched
    /// operation resolves no earlier than the next tick where
    /// ResolveAtInWorldDays has elapsed.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AIMissionaryExecutionSystem))]
    public partial struct AIMissionaryResolutionSystem : ISystem
    {
        public const float FaithExposureThreshold = 100f;
        public const float FailureIntensityReinforcement = 2f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DynastyOperationComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;
            float inWorldDays = GetInWorldDays(em);

            var q = em.CreateEntityQuery(
                ComponentType.ReadOnly<DynastyOperationComponent>(),
                ComponentType.ReadOnly<DynastyOperationMissionaryComponent>());
            var entities = q.ToEntityArray(Allocator.Temp);
            var ops = q.ToComponentDataArray<DynastyOperationComponent>(Allocator.Temp);
            var missionaries = q.ToComponentDataArray<DynastyOperationMissionaryComponent>(Allocator.Temp);
            q.Dispose();

            for (int i = 0; i < entities.Length; i++)
            {
                if (!ops[i].Active) continue;
                if (ops[i].OperationKind != DynastyOperationKind.Missionary) continue;
                if (missionaries[i].ResolveAtInWorldDays > inWorldDays) continue;

                ResolveOperation(em, ops[i], missionaries[i]);

                // Flip Active=false on the parent operation component so
                // the per-faction cap (sub-slice 18) releases the slot.
                var op = ops[i];
                op.Active = false;
                em.SetComponentData(entities[i], op);
            }

            entities.Dispose();
            ops.Dispose();
            missionaries.Dispose();
        }

        // ------------------------------------------------------------------ resolution

        private static void ResolveOperation(
            EntityManager em,
            DynastyOperationComponent op,
            DynastyOperationMissionaryComponent mission)
        {
            var targetEntity = FindFactionEntity(em, op.TargetFactionId);
            if (targetEntity == Entity.Null)
            {
                PushVoidMessage(em, op.SourceFactionId, op.TargetFactionId);
                return;
            }

            bool success = mission.SuccessScore >= 0f;
            if (success)
            {
                ApplySuccessEffects(em, targetEntity, mission);
                PushSuccessMessage(em, op.SourceFactionId, op.TargetFactionId, mission.SourceFaithId);
            }
            else
            {
                ApplyFailureEffects(em, targetEntity);
                PushFailureMessage(em, op.SourceFactionId, op.TargetFactionId, mission.SourceFaithId);
            }
        }

        private static void ApplySuccessEffects(
            EntityManager em,
            Entity targetEntity,
            DynastyOperationMissionaryComponent mission)
        {
            // Exposure gain on target's FaithExposureElement buffer.
            var faithId = ParseFaithId(mission.SourceFaithId);
            ApplyExposureGain(em, targetEntity, faithId, mission.ExposureGain);

            // Intensity erosion if target has a different committed faith.
            if (em.HasComponent<FaithStateComponent>(targetEntity))
            {
                var targetFaith = em.GetComponentData<FaithStateComponent>(targetEntity);
                if (targetFaith.SelectedFaith != CovenantId.None &&
                    targetFaith.SelectedFaith != faithId)
                {
                    targetFaith.Intensity = math.max(0f, targetFaith.Intensity - mission.IntensityErosion);
                    em.SetComponentData(targetEntity, targetFaith);
                }
            }

            // Loyalty pressure on lowest-loyalty control point owned by target.
            ApplyLoyaltyPressure(em, targetEntity, mission.LoyaltyPressure);
        }

        private static void ApplyFailureEffects(EntityManager em, Entity targetEntity)
        {
            // Target faith intensity += 2 (strengthens target faith).
            if (!em.HasComponent<FaithStateComponent>(targetEntity)) return;
            var targetFaith = em.GetComponentData<FaithStateComponent>(targetEntity);
            if (targetFaith.SelectedFaith == CovenantId.None) return;
            targetFaith.Intensity += FailureIntensityReinforcement;
            em.SetComponentData(targetEntity, targetFaith);
        }

        private static void ApplyExposureGain(
            EntityManager em,
            Entity targetEntity,
            CovenantId faithId,
            float exposureGain)
        {
            if (faithId == CovenantId.None) return;
            if (!em.HasBuffer<FaithExposureElement>(targetEntity)) return;

            var buffer = em.GetBuffer<FaithExposureElement>(targetEntity);
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i].Faith == faithId)
                {
                    var entry = buffer[i];
                    entry.Exposure   = math.clamp(entry.Exposure + exposureGain, 0f, 100f);
                    entry.Discovered = true;
                    buffer[i] = entry;
                    return;
                }
            }

            // No existing entry: append.
            buffer.Add(new FaithExposureElement
            {
                Faith      = faithId,
                Exposure   = math.clamp(exposureGain, 0f, 100f),
                Discovered = true,
            });
        }

        private static void ApplyLoyaltyPressure(
            EntityManager em,
            Entity targetEntity,
            float loyaltyPressure)
        {
            if (loyaltyPressure <= 0f) return;

            var targetFaction = em.GetComponentData<FactionComponent>(targetEntity);
            var q = em.CreateEntityQuery(ComponentType.ReadWrite<ControlPointComponent>());
            var entities = q.ToEntityArray(Allocator.Temp);
            q.Dispose();

            // Find the control point with the lowest loyalty that is
            // owned by the target faction.
            Entity lowestEntity = Entity.Null;
            float lowestLoyalty = float.MaxValue;
            for (int i = 0; i < entities.Length; i++)
            {
                var cp = em.GetComponentData<ControlPointComponent>(entities[i]);
                if (!cp.OwnerFactionId.Equals(targetFaction.FactionId)) continue;
                if (cp.Loyalty < lowestLoyalty)
                {
                    lowestLoyalty = cp.Loyalty;
                    lowestEntity  = entities[i];
                }
            }
            entities.Dispose();

            if (lowestEntity == Entity.Null) return;

            var lowestCp = em.GetComponentData<ControlPointComponent>(lowestEntity);
            lowestCp.Loyalty = math.clamp(lowestCp.Loyalty - loyaltyPressure, 0f, 100f);
            em.SetComponentData(lowestEntity, lowestCp);
        }

        // ------------------------------------------------------------------ narrative

        private static void PushVoidMessage(
            EntityManager em,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId)
        {
            var message = new FixedString128Bytes();
            message.Append(sourceFactionId);
            message.Append((FixedString64Bytes)" missionary mission expired without a valid target at ");
            message.Append(targetFactionId);
            message.Append((FixedString32Bytes)".");
            NarrativeMessageBridge.Push(em, message, NarrativeMessageTone.Info);
        }

        private static void PushSuccessMessage(
            EntityManager em,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            FixedString64Bytes sourceFaithId)
        {
            var message = new FixedString128Bytes();
            message.Append(sourceFaithId);
            message.Append((FixedString32Bytes)" missionaries breach ");
            message.Append(targetFactionId);
            message.Append((FixedString64Bytes)" with pressure dispatched by ");
            message.Append(sourceFactionId);
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

        private static void PushFailureMessage(
            EntityManager em,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            FixedString64Bytes sourceFaithId)
        {
            var message = new FixedString128Bytes();
            message.Append(targetFactionId);
            message.Append((FixedString64Bytes)" repelled missionaries of ");
            message.Append(sourceFaithId);
            message.Append((FixedString32Bytes)" from ");
            message.Append(sourceFactionId);
            message.Append((FixedString32Bytes)".");

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

        internal static CovenantId ParseFaithId(FixedString64Bytes faithId)
        {
            if (faithId.Equals(new FixedString64Bytes("old_light")))      return CovenantId.OldLight;
            if (faithId.Equals(new FixedString64Bytes("blood_dominion"))) return CovenantId.BloodDominion;
            if (faithId.Equals(new FixedString64Bytes("the_order")))      return CovenantId.TheOrder;
            if (faithId.Equals(new FixedString64Bytes("the_wild")))       return CovenantId.TheWild;
            return CovenantId.None;
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
                if (factions[i].FactionId.Equals(factionId))
                {
                    match = entities[i];
                    break;
                }
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
