using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.AI
{
    /// <summary>
    /// Resolves active divine right operations and fires the conviction event
    /// deferred by AIDivineRightExecutionSystem.
    ///
    /// Browser reference: tickFaithDivineRightDeclarations
    /// (simulation.js:10747-10782), failDivineRightDeclaration
    /// (simulation.js:10691-10713), completeDivineRightDeclaration
    /// (simulation.js:10715-10741), startDivineRightDeclaration conviction
    /// event (simulation.js:10806-10812).
    ///
    /// Resolution fires when a DynastyOperationComponent entity with
    /// OperationKind=DivineRight and Active=true is encountered each tick.
    ///
    /// Failure path (browser failDivineRightDeclaration): fires when the
    /// source faction loses its faith commitment (SelectedFaith==None or
    /// Intensity below DivineRightIntensityThreshold) during the declaration
    /// window. Effects:
    ///   1. Drain source FaithStateComponent.Intensity by
    ///      DivineRightFailureIntensityLoss=18 (browser simulation.js:9783 /
    ///      10705). Clamped to 0.
    ///   2. Fire conviction event on source: dark=Desecration+3 /
    ///      light=Oathkeeping+3 (deferred dispatch-time event at
    ///      simulation.js:10806).
    ///   3. Push failure narrative (browser simulation.js:10708-10712).
    ///   4. Flip DynastyOperationComponent.Active = false.
    ///
    /// Success path (browser completeDivineRightDeclaration): fires when
    /// inWorldDays >= ResolveAtInWorldDays and source still holds faith
    /// commitment. Effects:
    ///   1. Fire conviction event on source: dark=Desecration+3 /
    ///      light=Oathkeeping+3 (deferred dispatch-time event at
    ///      simulation.js:10806).
    ///   2. Push completion narrative (browser simulation.js:10736-10740).
    ///   3. Flip DynastyOperationComponent.Active = false.
    ///
    /// Unity-side deferred work (outside this slice scope):
    ///   - Legitimacy adjustment (+12 on success, -10 on failure): no
    ///     canonical Legitimacy surface outside dynasty-core lane.
    ///   - Game-over signaling (state.meta.status = "won"/"lost"):
    ///     no game-status component in this lane.
    ///   - Cooldown surface (divine-right cooldown prevents immediate
    ///     re-dispatch for 75 real-seconds = 150 in-world days; deferred
    ///     per AIDivineRightExecutionSystem scope note).
    ///   - Recognition share check: global apex-faith share calculator
    ///     not yet ported; resolution treats inWorldDays >= ResolveAt as
    ///     the sole success condition.
    ///   - Apex structure check: apex structure surface not yet ported.
    ///   - ensureMutualHostility at declaration time: hostility component
    ///     pair not yet ported.
    ///
    /// Updates after AIDivineRightExecutionSystem so a same-tick dispatch
    /// cannot immediately resolve.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AIDivineRightExecutionSystem))]
    public partial struct AIDivineRightResolutionSystem : ISystem
    {
        /// Browser DIVINE_RIGHT_FAILURE_INTENSITY_LOSS at simulation.js:9783.
        public const float DivineRightFailureIntensityLoss = 18f;

        /// Browser DIVINE_RIGHT_INTENSITY_THRESHOLD at simulation.js:9782.
        public const float DivineRightIntensityThreshold = 80f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DualClockComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;
            float inWorldDays = GetInWorldDays(em);

            TickDivineRightResolutions(em, inWorldDays);
        }

        // ------------------------------------------------------------------ resolution

        private static void TickDivineRightResolutions(EntityManager em, float inWorldDays)
        {
            var q = em.CreateEntityQuery(
                ComponentType.ReadWrite<DynastyOperationComponent>(),
                ComponentType.ReadOnly<DynastyOperationDivineRightComponent>());
            var entities = q.ToEntityArray(Allocator.Temp);
            var ops      = q.ToComponentDataArray<DynastyOperationComponent>(Allocator.Temp);
            var drs      = q.ToComponentDataArray<DynastyOperationDivineRightComponent>(Allocator.Temp);
            q.Dispose();

            for (int i = 0; i < entities.Length; i++)
            {
                if (!ops[i].Active) continue;

                var op = ops[i];
                var dr = drs[i];

                var sourceEntity = FindFactionEntity(em, op.SourceFactionId);
                if (sourceEntity == Entity.Null)
                {
                    op.Active = false;
                    em.SetComponentData(entities[i], op);
                    continue;
                }

                bool faithLost = false;
                if (!em.HasComponent<FaithStateComponent>(sourceEntity))
                {
                    faithLost = true;
                }
                else
                {
                    var sourceFaith = em.GetComponentData<FaithStateComponent>(sourceEntity);
                    if (sourceFaith.SelectedFaith == CovenantId.None ||
                        sourceFaith.Intensity < DivineRightIntensityThreshold)
                    {
                        faithLost = true;
                    }
                }

                if (faithLost)
                {
                    ApplyFailureEffects(em, sourceEntity, op.SourceFactionId, dr.DoctrinePath);
                    op.Active = false;
                    em.SetComponentData(entities[i], op);
                    continue;
                }

                if (inWorldDays >= dr.ResolveAtInWorldDays)
                {
                    ApplySuccessEffects(em, sourceEntity, op.SourceFactionId, dr.DoctrinePath, dr.SourceFaithId);
                    op.Active = false;
                    em.SetComponentData(entities[i], op);
                }
            }

            entities.Dispose();
            ops.Dispose();
            drs.Dispose();
        }

        // ------------------------------------------------------------------ failure

        private static void ApplyFailureEffects(
            EntityManager em,
            Entity sourceEntity,
            FixedString32Bytes sourceFactionId,
            DoctrinePath doctrinePath)
        {
            // Drain intensity by DIVINE_RIGHT_FAILURE_INTENSITY_LOSS (browser :10705).
            if (em.HasComponent<FaithStateComponent>(sourceEntity))
            {
                var faith = em.GetComponentData<FaithStateComponent>(sourceEntity);
                faith.Intensity = math.max(0f, faith.Intensity - DivineRightFailureIntensityLoss);
                em.SetComponentData(sourceEntity, faith);
            }

            // Deferred dispatch-time conviction event (browser :10806; +3 either path).
            RecordConvictionEvent(em, sourceEntity, doctrinePath, 3f);

            // Browser simulation.js:10708-10712.
            var message = new FixedString128Bytes();
            message.Append(sourceFactionId);
            message.Append((FixedString64Bytes)" loses its Divine Right claim.");
            var playerId = new FixedString32Bytes("player");
            NarrativeMessageBridge.Push(
                em,
                message,
                sourceFactionId.Equals(playerId) ? NarrativeMessageTone.Warn : NarrativeMessageTone.Good);
        }

        // ------------------------------------------------------------------ success

        private static void ApplySuccessEffects(
            EntityManager em,
            Entity sourceEntity,
            FixedString32Bytes sourceFactionId,
            DoctrinePath doctrinePath,
            FixedString64Bytes sourceFaithId)
        {
            // Deferred dispatch-time conviction event (browser :10806; +3 either path).
            RecordConvictionEvent(em, sourceEntity, doctrinePath, 3f);

            // Browser simulation.js:10736-10740.
            var message = new FixedString128Bytes();
            message.Append(sourceFactionId);
            message.Append((FixedString64Bytes)" completes the Divine Right spread window under ");
            message.Append(sourceFaithId);
            message.Append((FixedString32Bytes)". Sovereignty is recognized.");
            var playerId = new FixedString32Bytes("player");
            NarrativeMessageBridge.Push(
                em,
                message,
                sourceFactionId.Equals(playerId) ? NarrativeMessageTone.Good : NarrativeMessageTone.Warn);
        }

        // ------------------------------------------------------------------ helpers

        private static void RecordConvictionEvent(
            EntityManager em,
            Entity sourceEntity,
            DoctrinePath doctrinePath,
            float amount)
        {
            // Browser simulation.js:10806-10812: dark=desecration, light=oathkeeping, +3.
            if (!em.HasComponent<ConvictionComponent>(sourceEntity)) return;
            var conv = em.GetComponentData<ConvictionComponent>(sourceEntity);
            if (doctrinePath == DoctrinePath.Dark)
                conv.Desecration += amount;
            else
                conv.Oathkeeping += amount;
            em.SetComponentData(sourceEntity, conv);
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
