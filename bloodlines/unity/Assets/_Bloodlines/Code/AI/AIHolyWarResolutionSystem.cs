using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.AI
{
    /// <summary>
    /// Resolves active holy war operations in two phases each tick.
    ///
    /// Phase A -- Declaration resolution (browser tickDynastyOperations
    /// holy_war branch at simulation.js:5590-5645): fires once per
    /// DynastyOperationHolyWarComponent entity when
    /// DynastyOperationComponent.Active == true and
    /// InWorldDays >= ResolveAtInWorldDays. On resolution:
    ///   1. Void if source faction has lost its faith commitment
    ///      (FaithStateComponent.SelectedFaith == None) -- browser gate:
    ///      `if (!entry) { finalize(void) }` after createHolyWarEntry
    ///      returns null when sourceFaction.faith.selectedFaithId is absent.
    ///   2. Void if target faction no longer exists or has lost faith --
    ///      browser gate: `if (!targetFaction?.faith || !targetFaction?.dynasty)`.
    ///   3. On success: create ActiveHolyWarElement buffer entry on source
    ///      faction entity (browser faction.faith.activeHolyWars.unshift,
    ///      capped at 6). Boost source intensity by math.max(3,
    ///      intensityPulse * 2) (browser simulation.js:5611). Drain target
    ///      lowest-loyalty control point by math.max(2, loyaltyPulse)
    ///      (browser simulation.js:5615-5619). Apply conviction event to
    ///      source (dark=Desecration+2 / light=Oathkeeping+2) and to
    ///      target (Stewardship-1) if ConvictionComponent is present.
    ///      Push narrative message.
    ///   4. Flip DynastyOperationComponent.Active = false (browser
    ///      finalizeDynastyOperation with status "completed" or "void").
    ///
    /// Phase B -- War-tick sustained effects (browser tickFaithHolyWars
    /// at simulation.js:4160-4215): fires every tick for each faction
    /// entity that has an ActiveHolyWarElement buffer.
    ///   - If entry.ExpiresAtInWorldDays <= inWorldDays: prune entry from
    ///     buffer; push expiration narrative if player-related (browser
    ///     faction.id==player || entry.targetFactionId==player).
    ///   - Else: drain target lowest control point Loyalty by
    ///     loyaltyPulse * SustainedLoyaltyDrainMultiplier * dt
    ///     (browser simulation.js:4191; dt in real seconds, matching
    ///     SystemAPI.Time.DeltaTime). Legitimacy drain deferred (no
    ///     canonical Legitimacy field outside dynasty-core lane).
    ///   - Pulse check at PulseIntervalInWorldDays (60 in-world days =
    ///     30 real seconds at canonical DaysPerRealSecond=2; browser
    ///     HOLY_WAR_PULSE_INTERVAL_SECONDS=30 at simulation.js:9776):
    ///     boost source intensity by IntensityPulse (browser:4199), drain
    ///     target control point Loyalty by LoyaltyPulse (browser:4202) or
    ///     FactionLoyaltyComponent if no control points; update
    ///     LastPulseAtInWorldDays.
    ///
    /// Unity-side deferred work (outside this slice scope):
    ///   - Legitimacy drain during sustained phase (adjustLegitimacy):
    ///     browser simulation.js:4186; Unity defers until a canonical
    ///     Legitimacy surface ports outside the dynasty-core lane.
    ///   - ensureMutualHostility at declaration resolution (browser
    ///     simulation.js:5600): requires a hostility component pair not
    ///     yet ported.
    ///   - Conviction event recording deferred if ConvictionComponent is
    ///     absent; the guard is already in place.
    ///   - Full faith-compatibility parity (covenant-name covariance):
    ///     inherited from sub-slice 21 simplification.
    ///
    /// Updates after AIHolyWarExecutionSystem so a dispatch and Phase A
    /// resolution cannot happen in the same tick.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AIHolyWarExecutionSystem))]
    public partial struct AIHolyWarResolutionSystem : ISystem
    {
        /// In-world-days equivalent of browser HOLY_WAR_PULSE_INTERVAL_SECONDS=30
        /// at canonical DaysPerRealSecond=2: 30 * 2 = 60.
        public const float PulseIntervalInWorldDays        = 60f;

        /// Browser HOLY_WAR_SUSTAINED_LOYALTY_DRAIN_MULTIPLIER at simulation.js:9777.
        public const float SustainedLoyaltyDrainMultiplier = 1.5f;

        /// Maximum concurrent holy wars per faction (browser .slice(0, 6)).
        public const int MaxActiveHolyWarsPerFaction = 6;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DualClockComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em          = state.EntityManager;
            float inWorldDays = GetInWorldDays(em);
            float dt          = SystemAPI.Time.DeltaTime;

            TickDeclarationResolutions(em, inWorldDays);
            TickActiveHolyWars(em, inWorldDays, dt);
        }

        // ------------------------------------------------------------------ Phase A

        private static void TickDeclarationResolutions(EntityManager em, float inWorldDays)
        {
            var q = em.CreateEntityQuery(
                ComponentType.ReadOnly<DynastyOperationComponent>(),
                ComponentType.ReadOnly<DynastyOperationHolyWarComponent>());
            var entities  = q.ToEntityArray(Allocator.Temp);
            var ops       = q.ToComponentDataArray<DynastyOperationComponent>(Allocator.Temp);
            var holyWars  = q.ToComponentDataArray<DynastyOperationHolyWarComponent>(Allocator.Temp);
            q.Dispose();

            for (int i = 0; i < entities.Length; i++)
            {
                if (!ops[i].Active) continue;
                if (ops[i].OperationKind != DynastyOperationKind.HolyWar) continue;
                if (holyWars[i].ResolveAtInWorldDays > inWorldDays) continue;

                ResolveDeclaration(em, entities[i], ops[i], holyWars[i], inWorldDays);

                var op = ops[i];
                op.Active = false;
                em.SetComponentData(entities[i], op);
            }

            entities.Dispose();
            ops.Dispose();
            holyWars.Dispose();
        }

        private static void ResolveDeclaration(
            EntityManager em,
            Entity opEntity,
            DynastyOperationComponent op,
            DynastyOperationHolyWarComponent hw,
            float inWorldDays)
        {
            // Source void check: source must still hold a committed faith.
            var sourceEntity = FindFactionEntity(em, op.SourceFactionId);
            if (sourceEntity == Entity.Null) return;
            if (!em.HasComponent<FaithStateComponent>(sourceEntity)) return;
            var sourceFaith = em.GetComponentData<FaithStateComponent>(sourceEntity);
            if (sourceFaith.SelectedFaith == CovenantId.None)
            {
                PushVoidMessage(em, op.SourceFactionId, op.TargetFactionId, "Faith commitment was lost before holy war declaration could land.");
                return;
            }

            // Target void check: target must still exist and have faith.
            var targetEntity = FindFactionEntity(em, op.TargetFactionId);
            if (targetEntity == Entity.Null)
            {
                PushVoidMessage(em, op.SourceFactionId, op.TargetFactionId, "Target court no longer exists for holy war declaration.");
                return;
            }
            if (!em.HasComponent<FaithStateComponent>(targetEntity))
            {
                PushVoidMessage(em, op.SourceFactionId, op.TargetFactionId, "Target court no longer exists for holy war declaration.");
                return;
            }

            // Create ActiveHolyWarElement on source faction entity.
            if (!em.HasBuffer<ActiveHolyWarElement>(sourceEntity))
                em.AddBuffer<ActiveHolyWarElement>(sourceEntity);

            var warBuffer = em.GetBuffer<ActiveHolyWarElement>(sourceEntity);

            // Deduplicate: remove any existing entry for the same target.
            for (int j = 0; j < warBuffer.Length; j++)
            {
                if (warBuffer[j].TargetFactionId.Equals(op.TargetFactionId))
                {
                    warBuffer.RemoveAt(j);
                    break;
                }
            }

            var warEntry = new ActiveHolyWarElement
            {
                Id                    = op.OperationId,
                TargetFactionId       = op.TargetFactionId,
                FaithId               = sourceFaith.SelectedFaith,
                DocPath               = sourceFaith.DoctrinePath,
                DeclaredAtInWorldDays = inWorldDays,
                LastPulseAtInWorldDays = inWorldDays,
                ExpiresAtInWorldDays  = hw.WarExpiresAtInWorldDays,
                IntensityPulse        = hw.IntensityPulse,
                LoyaltyPulse          = hw.LoyaltyPulse,
            };

            // Insert at front; cap at MaxActiveHolyWarsPerFaction (browser .slice(0, 6)).
            warBuffer.Insert(0, warEntry);
            while (warBuffer.Length > MaxActiveHolyWarsPerFaction)
                warBuffer.RemoveAt(warBuffer.Length - 1);

            // Initial intensity boost to source: max(3, intensityPulse * 2).
            sourceFaith.Intensity += math.max(3f, hw.IntensityPulse * 2f);
            em.SetComponentData(sourceEntity, sourceFaith);

            // Initial loyalty drain to target lowest control point.
            ApplyInitialLoyaltyDrain(em, targetEntity, op.TargetFactionId, hw.LoyaltyPulse);

            // Conviction events: source and target if ConvictionComponent present.
            ApplyConvictionEvents(em, sourceEntity, targetEntity, sourceFaith.DoctrinePath);

            PushDeclarationLandedMessage(em, op.SourceFactionId, op.TargetFactionId);
        }

        private static void ApplyInitialLoyaltyDrain(
            EntityManager em,
            Entity targetEntity,
            FixedString32Bytes targetFactionId,
            float loyaltyPulse)
        {
            // Browser: pressurePoint.loyalty = clamp(p.loyalty - max(2, loyaltyPulse), 0, 100)
            // or adjustLegitimacy(target, -3) if no control point.
            float drain = math.max(2f, loyaltyPulse);

            var targetFaction = em.GetComponentData<FactionComponent>(targetEntity);
            var cpQuery = em.CreateEntityQuery(ComponentType.ReadWrite<ControlPointComponent>());
            var cpEntities = cpQuery.ToEntityArray(Allocator.Temp);
            cpQuery.Dispose();

            Entity lowestEntity = Entity.Null;
            float lowestLoyalty = float.MaxValue;
            for (int i = 0; i < cpEntities.Length; i++)
            {
                var cp = em.GetComponentData<ControlPointComponent>(cpEntities[i]);
                if (!cp.OwnerFactionId.Equals(targetFaction.FactionId)) continue;
                if (cp.Loyalty < lowestLoyalty)
                {
                    lowestLoyalty = cp.Loyalty;
                    lowestEntity  = cpEntities[i];
                }
            }
            cpEntities.Dispose();

            if (lowestEntity != Entity.Null)
            {
                var lowestCp = em.GetComponentData<ControlPointComponent>(lowestEntity);
                lowestCp.Loyalty = math.clamp(lowestCp.Loyalty - drain, 0f, 100f);
                em.SetComponentData(lowestEntity, lowestCp);
            }
            else if (em.HasComponent<FactionLoyaltyComponent>(targetEntity))
            {
                // Fallback: drain FactionLoyaltyComponent when no control points exist.
                var loyalty = em.GetComponentData<FactionLoyaltyComponent>(targetEntity);
                loyalty.Current = math.clamp(loyalty.Current - drain, loyalty.Floor, loyalty.Max);
                em.SetComponentData(targetEntity, loyalty);
            }
        }

        private static void ApplyConvictionEvents(
            EntityManager em,
            Entity sourceEntity,
            Entity targetEntity,
            DoctrinePath sourceDoctrine)
        {
            // Source: dark=Desecration+2 / light=Oathkeeping+2
            // Browser simulation.js:5622-5630 recordConvictionEvent(source, dark?"desecration":"oathkeeping", 2)
            if (em.HasComponent<ConvictionComponent>(sourceEntity))
            {
                var conv = em.GetComponentData<ConvictionComponent>(sourceEntity);
                if (sourceDoctrine == DoctrinePath.Dark)
                    conv.Desecration += 2f;
                else
                    conv.Oathkeeping += 2f;
                em.SetComponentData(sourceEntity, conv);
            }

            // Target: Stewardship-1 (browser simulation.js:5631-5635)
            if (em.HasComponent<ConvictionComponent>(targetEntity))
            {
                var conv = em.GetComponentData<ConvictionComponent>(targetEntity);
                conv.Stewardship -= 1f;
                em.SetComponentData(targetEntity, conv);
            }
        }

        // ------------------------------------------------------------------ Phase B

        private static void TickActiveHolyWars(EntityManager em, float inWorldDays, float dt)
        {
            var factionQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>());
            var factionEntities = factionQuery.ToEntityArray(Allocator.Temp);
            factionQuery.Dispose();

            for (int i = 0; i < factionEntities.Length; i++)
            {
                var sourceEntity = factionEntities[i];
                if (!em.HasBuffer<ActiveHolyWarElement>(sourceEntity)) continue;

                var sourceFaction = em.GetComponentData<FactionComponent>(sourceEntity);
                var warBuffer = em.GetBuffer<ActiveHolyWarElement>(sourceEntity);

                // Process entries from back to front so RemoveAt indices stay valid.
                for (int j = warBuffer.Length - 1; j >= 0; j--)
                {
                    var entry = warBuffer[j];

                    if (entry.ExpiresAtInWorldDays <= inWorldDays)
                    {
                        // War expired: prune and notify if player-related.
                        warBuffer.RemoveAt(j);
                        var playerId = new FixedString32Bytes("player");
                        if (sourceFaction.FactionId.Equals(playerId) || entry.TargetFactionId.Equals(playerId))
                            PushExpirationMessage(em, sourceFaction.FactionId, entry.TargetFactionId);
                        continue;
                    }

                    var targetEntity = FindFactionEntity(em, entry.TargetFactionId);
                    if (targetEntity == Entity.Null) continue;

                    // Sustained loyalty drain per frame (browser simulation.js:4191).
                    ApplySustainedLoyaltyDrain(em, targetEntity, entry.TargetFactionId,
                        entry.LoyaltyPulse * SustainedLoyaltyDrainMultiplier * dt);

                    // Pulse check: fire when interval has elapsed.
                    if (inWorldDays >= entry.LastPulseAtInWorldDays + PulseIntervalInWorldDays)
                    {
                        ApplyPulseEffects(em, sourceEntity, targetEntity, entry.TargetFactionId,
                            entry.IntensityPulse, entry.LoyaltyPulse);

                        entry.LastPulseAtInWorldDays = inWorldDays;
                        warBuffer[j] = entry;
                    }
                }
            }

            factionEntities.Dispose();
        }

        private static void ApplySustainedLoyaltyDrain(
            EntityManager em,
            Entity targetEntity,
            FixedString32Bytes targetFactionId,
            float drain)
        {
            if (drain <= 0f) return;

            var targetFaction = em.GetComponentData<FactionComponent>(targetEntity);
            var cpQuery = em.CreateEntityQuery(ComponentType.ReadWrite<ControlPointComponent>());
            var cpEntities = cpQuery.ToEntityArray(Allocator.Temp);
            cpQuery.Dispose();

            Entity lowestEntity = Entity.Null;
            float lowestLoyalty = float.MaxValue;
            for (int i = 0; i < cpEntities.Length; i++)
            {
                var cp = em.GetComponentData<ControlPointComponent>(cpEntities[i]);
                if (!cp.OwnerFactionId.Equals(targetFaction.FactionId)) continue;
                if (cp.Loyalty < lowestLoyalty)
                {
                    lowestLoyalty = cp.Loyalty;
                    lowestEntity  = cpEntities[i];
                }
            }
            cpEntities.Dispose();

            if (lowestEntity != Entity.Null)
            {
                var lowestCp = em.GetComponentData<ControlPointComponent>(lowestEntity);
                lowestCp.Loyalty = math.clamp(lowestCp.Loyalty - drain, 0f, 100f);
                em.SetComponentData(lowestEntity, lowestCp);
            }
        }

        private static void ApplyPulseEffects(
            EntityManager em,
            Entity sourceEntity,
            Entity targetEntity,
            FixedString32Bytes targetFactionId,
            float intensityPulse,
            float loyaltyPulse)
        {
            // Source intensity boost (browser simulation.js:4199).
            if (em.HasComponent<FaithStateComponent>(sourceEntity))
            {
                var faith = em.GetComponentData<FaithStateComponent>(sourceEntity);
                faith.Intensity += intensityPulse;
                em.SetComponentData(sourceEntity, faith);
            }

            // Target control-point loyalty drain (browser simulation.js:4202).
            var targetFaction = em.GetComponentData<FactionComponent>(targetEntity);
            var cpQuery = em.CreateEntityQuery(ComponentType.ReadWrite<ControlPointComponent>());
            var cpEntities = cpQuery.ToEntityArray(Allocator.Temp);
            cpQuery.Dispose();

            Entity lowestEntity = Entity.Null;
            float lowestLoyalty = float.MaxValue;
            for (int i = 0; i < cpEntities.Length; i++)
            {
                var cp = em.GetComponentData<ControlPointComponent>(cpEntities[i]);
                if (!cp.OwnerFactionId.Equals(targetFaction.FactionId)) continue;
                if (cp.Loyalty < lowestLoyalty)
                {
                    lowestLoyalty = cp.Loyalty;
                    lowestEntity  = cpEntities[i];
                }
            }
            cpEntities.Dispose();

            if (lowestEntity != Entity.Null)
            {
                var lowestCp = em.GetComponentData<ControlPointComponent>(lowestEntity);
                lowestCp.Loyalty = math.clamp(lowestCp.Loyalty - loyaltyPulse, 0f, 100f);
                em.SetComponentData(lowestEntity, lowestCp);
            }
            else if (em.HasComponent<FactionLoyaltyComponent>(targetEntity))
            {
                // Browser fallback: adjustLegitimacy(target, -max(1, round(loyaltyPulse * 0.4))).
                // Unity maps to FactionLoyaltyComponent.Current as closest available surface.
                var loyalty = em.GetComponentData<FactionLoyaltyComponent>(targetEntity);
                float penalty = math.max(1f, math.round(loyaltyPulse * 0.4f));
                loyalty.Current = math.clamp(loyalty.Current - penalty, loyalty.Floor, loyalty.Max);
                em.SetComponentData(targetEntity, loyalty);
            }
        }

        // ------------------------------------------------------------------ narrative

        private static void PushDeclarationLandedMessage(
            EntityManager em,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId)
        {
            // Browser simulation.js:5647-5651.
            var message = new FixedString128Bytes();
            message.Append(sourceFactionId);
            message.Append((FixedString64Bytes)" declares holy war on ");
            message.Append(targetFactionId);
            message.Append((FixedString32Bytes)".");

            var playerId = new FixedString32Bytes("player");
            NarrativeMessageTone tone;
            if (sourceFactionId.Equals(playerId))
                tone = NarrativeMessageTone.Warn;
            else if (targetFactionId.Equals(playerId))
                tone = NarrativeMessageTone.Warn;
            else
                tone = NarrativeMessageTone.Info;

            NarrativeMessageBridge.Push(em, message, tone);
        }

        private static void PushVoidMessage(
            EntityManager em,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            string reason)
        {
            var message = new FixedString128Bytes();
            message.Append(sourceFactionId);
            message.Append((FixedString64Bytes)" holy war against ");
            message.Append(targetFactionId);
            message.Append((FixedString32Bytes)" voided.");
            NarrativeMessageBridge.Push(em, message, NarrativeMessageTone.Info);
        }

        private static void PushExpirationMessage(
            EntityManager em,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId)
        {
            // Browser simulation.js:4172-4177: only pushes for player-related wars.
            var message = new FixedString128Bytes();
            message.Append(sourceFactionId);
            message.Append((FixedString64Bytes)" holy war against ");
            message.Append(targetFactionId);
            message.Append((FixedString64Bytes)" has run out of declared fervor.");

            var playerId = new FixedString32Bytes("player");
            NarrativeMessageTone tone;
            if (sourceFactionId.Equals(playerId))
                tone = NarrativeMessageTone.Info;
            else if (targetFactionId.Equals(playerId))
                tone = NarrativeMessageTone.Good;
            else
                tone = NarrativeMessageTone.Info;

            NarrativeMessageBridge.Push(em, message, tone);
        }

        // ------------------------------------------------------------------ helpers

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
