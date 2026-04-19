using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.AI
{
    /// <summary>
    /// Consumes AICovertOpsComponent.LastFiredOp == CovertOpKind.HolyWar
    /// (written by AICovertOpsSystem in sub-slice 6) and executes the
    /// browser-side startHolyWarDeclaration path
    /// (simulation.js:10565-10602). Second consumer of the sub-slice 18
    /// dynasty-operation foundation, mirroring sub-slice 20's missionary
    /// pattern.
    ///
    /// Browser dispatch site: ai.js holy war dispatch ~2512-2551,
    /// hardcoded to source "enemy" / target "player".
    ///
    /// Gates ported from getHolyWarDeclarationTerms (~10424-10471):
    ///   1. Source != target.
    ///   2. Source has a committed faith
    ///      (FaithStateComponent.SelectedFaith != CovenantId.None).
    ///   3. Target faction exists and has a committed faith. Browser
    ///      `if (!targetFaction.faith?.selectedFaithId) return ...`.
    ///   4. Faith compatibility tier is not "harmonious". Unity's
    ///      simplified port treats compatibility as harmonious only
    ///      when source and target have identical (faith, doctrine);
    ///      browser uses the broader getMarriageFaithCompatibilityProfile
    ///      tier ladder which depends on covenant-name covariance not
    ///      yet ported. The simplified gate is strictly looser than
    ///      browser (it lets some "neutral" tiers through that browser
    ///      would block); future tightening lands when the covenant
    ///      compatibility surface ports.
    ///   5. Source has a faith operator on the dynasty roster
    ///      (DynastyMemberComponent with one of IdeologicalLeader,
    ///      Spymaster, HeadOfBloodline, Diplomat with non-Fallen,
    ///      non-Captured status). Reuses the same role gate as
    ///      sub-slice 20 missionary; Spymaster substitutes for the
    ///      canonical Sorcerer role until DynastyRole gains a Sorcerer
    ///      entry.
    ///   6. Source ResourceStockpileComponent.Influence >= 24
    ///      (HOLY_WAR_COST.influence at simulation.js:9767).
    ///   7. Source FaithStateComponent.Intensity >= 18
    ///      (HOLY_WAR_INTENSITY_COST at simulation.js:9774).
    ///
    /// Capacity gate: DynastyOperationLimits.HasCapacity must return
    /// true for the source faction (sub-slice 18). The cap is six per
    /// faction (DYNASTY_OPERATION_ACTIVE_LIMIT at simulation.js:17).
    ///
    /// Effects on success:
    ///   - Deduct 24 Influence from ResourceStockpileComponent.
    ///   - Deduct 18 Intensity from FaithStateComponent (clamped >= 0).
    ///   - Compute per-kind terms (IntensityPulse / LoyaltyPulse from
    ///     doctrine bias; CompatibilityLabel from the simplified
    ///     compatibility tier).
    ///   - Call DynastyOperationLimits.BeginOperation with
    ///     DynastyOperationKind.HolyWar and the player target;
    ///     attach DynastyOperationHolyWarComponent to the created
    ///     entity carrying the per-kind fields (declaration window
    ///     end + war duration end + operator + pulses + cost).
    ///   - Push narrative message via NarrativeMessageBridge.Push
    ///     mirroring simulation.js:10596-10600 ("sends a holy war
    ///     declaration toward Y"). Tone routing per browser:
    ///       sourceFactionId == "player" -> Warn
    ///       targetFactionId == "player" -> Warn
    ///       else                        -> Info
    ///     Browser uses warn for both source==player and target==player
    ///     because holy war is an escalation either way; Unity matches.
    ///
    /// Always clears LastFiredOp to None after processing regardless of
    /// outcome so one dispatch produces one execution attempt, matching
    /// the one-shot pattern shared with sub-slices 8/9/12/13/14/20.
    ///
    /// Browser duration translation: HOLY_WAR_DECLARATION_DURATION_SECONDS
    /// = 18 and HOLY_WAR_DURATION_SECONDS = 180 are real seconds. Unity
    /// stores ResolveAtInWorldDays = current + 18 and
    /// WarExpiresAtInWorldDays = current + 180 (using browser numeric
    /// values directly on the in-world timeline). Future resolution
    /// slice can translate via DualClock.DaysPerRealSecond if the
    /// canonical clock rate ever shifts; data shape stays the same.
    ///
    /// Deferred to later slices:
    ///   - Per-operation resolution at ResolveAtInWorldDays (materialize
    ///     the holy-war entry on the source faction's faith state via
    ///     browser createHolyWarEntry at simulation.js:10505).
    ///   - War-tick effects at WarExpiresAtInWorldDays boundaries
    ///     (apply intensityPulse / loyaltyPulse to the target faction
    ///     until the war expires).
    ///   - Full faith-compatibility tier evaluation
    ///     (getMarriageFaithCompatibilityProfile parity with covenant-name
    ///     covariance; Unity uses simplified faith+doctrine equality).
    ///   - Sorcerer DynastyRole canonical mapping (Spymaster currently
    ///     stands in).
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AICovertOpsSystem))]
    public partial struct AIHolyWarExecutionSystem : ISystem
    {
        public const float HolyWarInfluenceCost              = 24f;
        public const float HolyWarIntensityCost              = 18f;
        public const float HolyWarDeclarationDurationInWorldDays = 18f;
        public const float HolyWarDurationInWorldDays        = 180f;

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
                if (covert.LastFiredOp != CovertOpKind.HolyWar)
                    continue;

                var sourceFaction = em.GetComponentData<FactionComponent>(sourceEntity);

                // Target is hardcoded as "player" matching the browser
                // ai.js dispatch (`startHolyWarDeclaration(state, "enemy", "player")`).
                var targetFactionId = new FixedString32Bytes("player");

                TryDispatchHolyWar(em, sourceEntity, sourceFaction.FactionId,
                    targetFactionId, inWorldDays);

                covert.LastFiredOp = CovertOpKind.None;
                em.SetComponentData(sourceEntity, covert);
            }

            dispatchEntities.Dispose();
        }

        // ------------------------------------------------------------------ dispatch

        private static void TryDispatchHolyWar(
            EntityManager em,
            Entity sourceEntity,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            float inWorldDays)
        {
            // Gate 1: same faction guard.
            if (sourceFactionId.Equals(targetFactionId)) return;

            // Gate 2: source must have a committed faith.
            if (!em.HasComponent<FaithStateComponent>(sourceEntity)) return;
            var sourceFaith = em.GetComponentData<FaithStateComponent>(sourceEntity);
            if (sourceFaith.SelectedFaith == CovenantId.None) return;

            // Gate 3: target faction must exist and have a committed faith.
            var targetEntity = FindFactionEntity(em, targetFactionId);
            if (targetEntity == Entity.Null) return;
            if (!em.HasComponent<FaithStateComponent>(targetEntity)) return;
            var targetFaith = em.GetComponentData<FaithStateComponent>(targetEntity);
            if (targetFaith.SelectedFaith == CovenantId.None) return;

            // Gate 4: simplified faith-compatibility check. Browser blocks
            // when tier == "harmonious"; Unity's simplification treats
            // identical (faith, doctrine) as harmonious. A future port of
            // getMarriageFaithCompatibilityProfile can tighten this gate.
            bool harmonious =
                sourceFaith.SelectedFaith == targetFaith.SelectedFaith &&
                sourceFaith.DoctrinePath  == targetFaith.DoctrinePath;
            if (harmonious) return;

            // Gate 5: source must have a faith operator member.
            if (!TryGetFaithOperator(em, sourceEntity,
                    out var operatorMemberId, out var operatorTitle, out var _))
                return;

            // Gate 6: source must afford the influence cost.
            if (!em.HasComponent<ResourceStockpileComponent>(sourceEntity)) return;
            var resources = em.GetComponentData<ResourceStockpileComponent>(sourceEntity);
            if (resources.Influence < HolyWarInfluenceCost) return;

            // Gate 7: source faith intensity must meet the threshold.
            if (sourceFaith.Intensity < HolyWarIntensityCost) return;

            // Capacity gate (sub-slice 18).
            if (!DynastyOperationLimits.HasCapacity(em, sourceFactionId)) return;

            // ------------------------------------------------------------------ effects

            resources.Influence -= HolyWarInfluenceCost;
            em.SetComponentData(sourceEntity, resources);

            sourceFaith.Intensity = math.max(0f, sourceFaith.Intensity - HolyWarIntensityCost);
            em.SetComponentData(sourceEntity, sourceFaith);

            // Per-kind terms: doctrine bias mirrors browser
            // simulation.js:10468-10469 (intensityPulse 1.2 dark / 0.9
            // light; loyaltyPulse 1.8 dark / 1.2 light).
            bool isDarkPath = sourceFaith.DoctrinePath == DoctrinePath.Dark;
            float intensityPulse = isDarkPath ? 1.2f : 0.9f;
            float loyaltyPulse   = isDarkPath ? 1.8f : 1.2f;

            // Compatibility label: simplified Unity-side derivation.
            // Browser uses tier label from getMarriageFaithCompatibilityProfile.
            var compatibilityLabel = DeriveCompatibilityLabel(sourceFaith, targetFaith);

            var operationId = BuildOperationId(sourceFactionId, targetFactionId, inWorldDays);
            var entity = DynastyOperationLimits.BeginOperation(
                em,
                operationId,
                sourceFactionId,
                DynastyOperationKind.HolyWar,
                targetFactionId,
                default);

            em.AddComponentData(entity, new DynastyOperationHolyWarComponent
            {
                ResolveAtInWorldDays    = inWorldDays + HolyWarDeclarationDurationInWorldDays,
                WarExpiresAtInWorldDays = inWorldDays + HolyWarDurationInWorldDays,
                OperatorMemberId        = operatorMemberId,
                OperatorTitle           = operatorTitle,
                IntensityPulse          = intensityPulse,
                LoyaltyPulse            = loyaltyPulse,
                CompatibilityLabel      = compatibilityLabel,
                IntensityCost           = HolyWarIntensityCost,
                EscrowCost              = new DynastyOperationEscrowCost { Influence = HolyWarInfluenceCost },
            });

            PushHolyWarDeclarationMessage(em, sourceFactionId, targetFactionId);
        }

        // ------------------------------------------------------------------ narrative

        private static void PushHolyWarDeclarationMessage(
            EntityManager em,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId)
        {
            var message = new FixedString128Bytes();
            message.Append(sourceFactionId);
            message.Append((FixedString64Bytes)" sends a holy war declaration toward ");
            message.Append(targetFactionId);
            message.Append((FixedString32Bytes)".");

            // Browser tone routing simulation.js:10599: source==player
            // -> warn, target==player -> warn, else -> info.
            var playerId = new FixedString32Bytes("player");
            NarrativeMessageTone tone;
            if (sourceFactionId.Equals(playerId) || targetFactionId.Equals(playerId))
                tone = NarrativeMessageTone.Warn;
            else
                tone = NarrativeMessageTone.Info;

            NarrativeMessageBridge.Push(em, message, tone);
        }

        // ------------------------------------------------------------------ helpers

        private static FixedString64Bytes DeriveCompatibilityLabel(
            FaithStateComponent source,
            FaithStateComponent target)
        {
            // Simplified Unity-side compatibility tier label. Browser
            // ladder lives in getMarriageFaithCompatibilityProfile and
            // uses covenant-name covariance not yet ported; this Unity
            // derivation maps the visible cases:
            //   - same faith, same doctrine: "harmonious" (gated out
            //     above; never reaches this code path)
            //   - same faith, different doctrine: "fractured"
            //   - different faith: "discordant"
            if (source.SelectedFaith == target.SelectedFaith)
                return new FixedString64Bytes("fractured");
            return new FixedString64Bytes("discordant");
        }

        private static bool TryGetFaithOperator(
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
                if (!IsFaithOperatorRole(member.Role)) continue;
                if (member.Status == DynastyMemberStatus.Fallen ||
                    member.Status == DynastyMemberStatus.Captured) continue;

                operatorMemberId = member.MemberId;
                operatorTitle    = member.Title;
                operatorRenown   = member.Renown;
                return true;
            }
            return false;
        }

        private static bool IsFaithOperatorRole(DynastyRole role)
        {
            // Browser order: ideological_leader, sorcerer, head_of_bloodline, diplomat.
            // Unity has no Sorcerer role; Spymaster stands in.
            return role == DynastyRole.IdeologicalLeader ||
                   role == DynastyRole.Spymaster ||
                   role == DynastyRole.HeadOfBloodline ||
                   role == DynastyRole.Diplomat;
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

        private static FixedString64Bytes BuildOperationId(
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            float inWorldDays)
        {
            var id = new FixedString64Bytes("dynop-holywar-");
            id.Append(sourceFactionId);
            id.Append("-");
            id.Append(targetFactionId);
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
