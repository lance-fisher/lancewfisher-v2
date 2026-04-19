using Bloodlines.Components;
using Bloodlines.Conviction;
using Bloodlines.Dynasties;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.AI
{
    /// <summary>
    /// Applies the browser-side diplomatic and dynasty effects that sub-slice 9
    /// (AIMarriageInboxAcceptSystem) deferred. Sub-slice 9 creates primary and
    /// mirror MarriageComponent entities on accept; sub-slice 11 now reads the
    /// one-shot MarriageAcceptEffectsPendingTag on the primary record and
    /// applies:
    ///
    ///   Legitimacy +2 on both HeadFaction and SpouseFaction dynasties,
    ///     clamped to 100 (browser: Math.min(100, legitimacy + 2)).
    ///   Hostility drop: remove HostilityComponent buffer entries pointing
    ///     from head to spouse and from spouse to head (browser:
    ///     source.hostileTo.filter(id => id !== targetFactionId) both ways).
    ///   Tag removal so the effects apply exactly once per accepted marriage.
    ///   In-world time declaration: push a 30-day DeclareInWorldTimeRequest
    ///     onto the DualClock singleton buffer so the match-progression clock
    ///     advances to reflect the wedding ceremony (browser:
    ///     declareInWorldTime(state, 30, "Marriage of ..."); simulation.js
    ///     line 7459). DualClockDeclarationSystem consumes the request and
    ///     applies the jump.
    ///   Oathkeeping conviction +2 on both factions via
    ///     ConvictionScoring.ApplyEvent (browser:
    ///     recordConvictionEvent(state, source.id, "oathkeeping", 2, ...) and
    ///     the equivalent on target; simulation.js lines 7455-7456).
    ///     Score + band are refreshed in place by the helper.
    ///
    /// Sub-slice 12 additions:
    ///   Governance authority legitimacy cost on the target (spouse) side.
    ///     When the accept is ratified by a regency (heir or envoy), the
    ///     target dynasty pays a legitimacy penalty before the +2 bonus is
    ///     applied, matching browser applyMarriageGovernanceLegitimacyCost at
    ///     simulation.js:6232. Head-direct approval costs 0. Heir regency
    ///     costs 1. Envoy regency costs 2. The cost is read from the
    ///     MarriageAcceptanceTermsComponent attached to the primary record
    ///     by AIMarriageInboxAcceptSystem.
    ///   Stewardship conviction event (-cost) on the target when cost > 0.
    ///     ConvictionScoring.ApplyEvent clamps the bucket at zero, mirroring
    ///     the browser recordConvictionEvent("stewardship", -penalty) call
    ///     (simulation.js:6238-6244).
    ///
    /// Sub-slice 17 addition:
    ///   Narrative message push. After effects are applied, call
    ///     NarrativeMessageBridge.Push with the browser-parity ceremonial
    ///     line: "<headMemberTitle> of <headFactionId> weds
    ///     <spouseMemberTitle> of <spouseFactionId> <approvalText>." where
    ///     approvalText varies by authority mode. Tone is Good when the
    ///     head (source) faction is "player", else Info. Unity has no
    ///     faction display-name lookup yet, so FactionId is used where
    ///     the browser uses getFactionDisplayName; when a display-name
    ///     component lands in a later slice the substitution point is
    ///     isolated in BuildMarriageAcceptMessage.
    ///
    /// Browser reference: simulation.js acceptMarriage (~7388-7469), specifically
    /// the block starting at the legitimacy +2 increment and the hostileTo filter.
    /// Narrative push at simulation.js:7463. Authority cost source: simulation.js
    /// applyMarriageGovernanceLegitimacyCost (~6232) and
    /// getMarriageAcceptanceTerms (~6327).
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AIMarriageInboxAcceptSystem))]
    public partial struct AIMarriageAcceptEffectsSystem : ISystem
    {
        private const float LegitimacyBonus = 2f;
        private const float LegitimacyCeiling = 100f;
        private const float LegitimacyFloor = 0f;

        // Browser declareInWorldTime(state, 30, ...) on acceptMarriage
        // (simulation.js:7459). Units are in-world days.
        private const float MarriageAcceptInWorldDayJump = 30f;

        // Browser recordConvictionEvent(state, factionId, "oathkeeping", 2, ...)
        // on acceptMarriage (simulation.js:7455-7456).
        private const float MarriageAcceptOathkeepingBonus = 2f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MarriageAcceptEffectsPendingTag>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;

            var query = em.CreateEntityQuery(
                ComponentType.ReadOnly<MarriageComponent>(),
                ComponentType.ReadOnly<MarriageAcceptEffectsPendingTag>());
            var marriageEntities = query.ToEntityArray(Allocator.Temp);
            var marriages        = query.ToComponentDataArray<MarriageComponent>(Allocator.Temp);
            query.Dispose();

            for (int i = 0; i < marriageEntities.Length; i++)
            {
                var marriage = marriages[i];
                var marriageEntity = marriageEntities[i];

                // Browser order (simulation.js acceptMarriage ~7449-7458):
                //   1. applyMarriageGovernanceLegitimacyCost on the target
                //      (deducts cost, records Stewardship -cost event).
                //   2. Drop hostility both ways.
                //   3. Oathkeeping +2 on both sides.
                //   4. Legitimacy +2 on both sides, clamped to 100.
                //   5. declareInWorldTime(30).
                ApplyAuthorityLegitimacyCost(em, marriageEntity, marriage.SpouseFactionId);

                DropHostility(em, marriage.HeadFactionId, marriage.SpouseFactionId);
                DropHostility(em, marriage.SpouseFactionId, marriage.HeadFactionId);

                RecordOathkeepingBonus(em, marriage.HeadFactionId);
                RecordOathkeepingBonus(em, marriage.SpouseFactionId);

                ApplyLegitimacyBonus(em, marriage.HeadFactionId);
                ApplyLegitimacyBonus(em, marriage.SpouseFactionId);

                // Push a 30-day in-world time declaration so the match clock
                // reflects the wedding ceremony. DualClockDeclarationSystem
                // drains the buffer and applies the jump.
                EnqueueMarriageTimeDeclaration(em, marriage.MarriageId);

                // Narrative push (sub-slice 17). Browser pushMessage at
                // simulation.js:7463 composes the ceremonial line with an
                // approvalText suffix derived from targetAuthority. Unity
                // reads the authority mode + cost from
                // MarriageAcceptanceTermsComponent attached by
                // AIMarriageInboxAcceptSystem.
                PushMarriageAcceptMessage(em, marriageEntity, marriage);

                // Remove the tag so effects apply exactly once. The
                // MarriageAcceptanceTermsComponent remains on the marriage
                // entity as a durable record of how it was ratified.
                em.RemoveComponent<MarriageAcceptEffectsPendingTag>(marriageEntity);
            }

            marriageEntities.Dispose();
            marriages.Dispose();
        }

        // ------------------------------------------------------------------ effects

        private static void ApplyAuthorityLegitimacyCost(
            EntityManager em,
            Entity marriageEntity,
            FixedString32Bytes spouseFactionId)
        {
            if (!em.HasComponent<MarriageAcceptanceTermsComponent>(marriageEntity))
                return;

            var terms = em.GetComponentData<MarriageAcceptanceTermsComponent>(marriageEntity);
            float cost = terms.LegitimacyCost;
            if (cost <= 0f) return;

            var factionEntity = FindFactionEntity(em, spouseFactionId);
            if (factionEntity == Entity.Null) return;

            // Dynasty legitimacy -= cost, clamped [0, 100]. Mirrors browser
            // adjustLegitimacy at simulation.js:3033 (clamp both ends).
            if (em.HasComponent<DynastyStateComponent>(factionEntity))
            {
                var dynasty = em.GetComponentData<DynastyStateComponent>(factionEntity);
                dynasty.Legitimacy = math.clamp(
                    dynasty.Legitimacy - cost, LegitimacyFloor, LegitimacyCeiling);
                em.SetComponentData(factionEntity, dynasty);
            }

            // Stewardship -cost conviction event, clamped at 0 by
            // ConvictionScoring.ApplyEvent. Browser recordConvictionEvent
            // with "stewardship", -penalty at simulation.js:6238-6244.
            if (em.HasComponent<ConvictionComponent>(factionEntity))
            {
                var conviction = em.GetComponentData<ConvictionComponent>(factionEntity);
                ConvictionScoring.ApplyEvent(
                    ref conviction,
                    ConvictionBucket.Stewardship,
                    -cost);
                em.SetComponentData(factionEntity, conviction);
            }
        }

        private static void ApplyLegitimacyBonus(EntityManager em, FixedString32Bytes factionId)
        {
            var factionEntity = FindFactionEntity(em, factionId);
            if (factionEntity == Entity.Null) return;
            if (!em.HasComponent<DynastyStateComponent>(factionEntity)) return;

            var dynasty = em.GetComponentData<DynastyStateComponent>(factionEntity);
            dynasty.Legitimacy = math.min(LegitimacyCeiling, dynasty.Legitimacy + LegitimacyBonus);
            em.SetComponentData(factionEntity, dynasty);
        }

        private static void DropHostility(
            EntityManager em,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId)
        {
            var sourceEntity = FindFactionEntity(em, sourceFactionId);
            if (sourceEntity == Entity.Null) return;
            if (!em.HasBuffer<HostilityComponent>(sourceEntity)) return;

            var buffer = em.GetBuffer<HostilityComponent>(sourceEntity);
            for (int i = buffer.Length - 1; i >= 0; i--)
            {
                if (buffer[i].HostileFactionId.Equals(targetFactionId))
                    buffer.RemoveAt(i);
            }
        }

        // ------------------------------------------------------------------ conviction

        private static void RecordOathkeepingBonus(EntityManager em, FixedString32Bytes factionId)
        {
            var factionEntity = FindFactionEntity(em, factionId);
            if (factionEntity == Entity.Null) return;
            if (!em.HasComponent<ConvictionComponent>(factionEntity)) return;

            var conviction = em.GetComponentData<ConvictionComponent>(factionEntity);
            ConvictionScoring.ApplyEvent(
                ref conviction,
                ConvictionBucket.Oathkeeping,
                MarriageAcceptOathkeepingBonus);
            em.SetComponentData(factionEntity, conviction);
        }

        // ------------------------------------------------------------------ in-world time

        private static void EnqueueMarriageTimeDeclaration(
            EntityManager em, FixedString64Bytes marriageId)
        {
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<DualClockComponent>());
            if (q.IsEmpty) { q.Dispose(); return; }
            var clockEntity = q.GetSingletonEntity();
            q.Dispose();

            if (!em.HasBuffer<DeclareInWorldTimeRequest>(clockEntity)) return;

            var buffer = em.GetBuffer<DeclareInWorldTimeRequest>(clockEntity);
            var reason = new FixedString64Bytes("Marriage ");
            for (int k = 0; k < marriageId.Length && reason.Length < 58; k++)
                reason.Append(marriageId[k]);

            buffer.Add(new DeclareInWorldTimeRequest
            {
                DaysDelta = MarriageAcceptInWorldDayJump,
                Reason    = reason,
            });
        }

        // ------------------------------------------------------------------ narrative push

        private static void PushMarriageAcceptMessage(
            EntityManager em,
            Entity marriageEntity,
            MarriageComponent marriage)
        {
            // Look up member titles via the dynasty member buffer on each
            // faction entity. Falls back to the member id when no matching
            // DynastyMemberComponent entity exists (synthetic test fixtures
            // do not always seed full rosters).
            var headTitle   = ResolveMemberTitle(em, marriage.HeadFactionId,   marriage.HeadMemberId);
            var spouseTitle = ResolveMemberTitle(em, marriage.SpouseFactionId, marriage.SpouseMemberId);

            // Authority suffix matches the browser approvalText ternary at
            // simulation.js:7460-7462. Unity lacks authority title strings
            // so the suffix uses canonical role words: "head", "heir",
            // "envoy". HeadDirect costs 0, HeirRegency 1, EnvoyRegency 2.
            var authorityMode = MarriageAuthorityMode.HeadDirect;
            float cost = 0f;
            if (em.HasComponent<MarriageAcceptanceTermsComponent>(marriageEntity))
            {
                var terms = em.GetComponentData<MarriageAcceptanceTermsComponent>(marriageEntity);
                authorityMode = terms.AuthorityMode;
                cost = terms.LegitimacyCost;
            }

            var message = BuildMarriageAcceptMessage(
                headTitle, marriage.HeadFactionId,
                spouseTitle, marriage.SpouseFactionId,
                authorityMode, cost);

            // Browser tone at simulation.js:7466 is the constant "good". Per
            // sub-slice 17's tone-routing rule, player as the head (source)
            // promotes to Good, else Info, so routine AI-to-AI marriages do
            // not drown the HUD feed in Good-toned notifications.
            var tone = marriage.HeadFactionId.Equals(new FixedString32Bytes("player"))
                ? NarrativeMessageTone.Good
                : NarrativeMessageTone.Info;

            NarrativeMessageBridge.Push(em, message, tone);
        }

        private static FixedString128Bytes BuildMarriageAcceptMessage(
            FixedString64Bytes headTitle, FixedString32Bytes headFactionId,
            FixedString64Bytes spouseTitle, FixedString32Bytes spouseFactionId,
            MarriageAuthorityMode authorityMode, float cost)
        {
            var msg = new FixedString128Bytes();
            msg.Append(headTitle);
            msg.Append((FixedString32Bytes)" of ");
            msg.Append(headFactionId);
            msg.Append((FixedString32Bytes)" weds ");
            msg.Append(spouseTitle);
            msg.Append((FixedString32Bytes)" of ");
            msg.Append(spouseFactionId);
            msg.Append((FixedString32Bytes)" ");
            AppendApprovalSuffix(ref msg, authorityMode, cost);
            msg.Append((FixedString32Bytes)".");
            return msg;
        }

        private static void AppendApprovalSuffix(
            ref FixedString128Bytes msg, MarriageAuthorityMode mode, float cost)
        {
            switch (mode)
            {
                case MarriageAuthorityMode.HeadDirect:
                    msg.Append((FixedString32Bytes)"under head approval");
                    break;
                case MarriageAuthorityMode.HeirRegency:
                    msg.Append((FixedString64Bytes)"under heir regency (legitimacy -");
                    msg.Append((int)cost);
                    msg.Append((FixedString32Bytes)")");
                    break;
                case MarriageAuthorityMode.EnvoyRegency:
                    msg.Append((FixedString64Bytes)"under envoy regency (legitimacy -");
                    msg.Append((int)cost);
                    msg.Append((FixedString32Bytes)")");
                    break;
                default:
                    msg.Append((FixedString32Bytes)"under household approval");
                    break;
            }
        }

        private static FixedString64Bytes ResolveMemberTitle(
            EntityManager em,
            FixedString32Bytes factionId,
            FixedString64Bytes memberId)
        {
            var factionEntity = FindFactionEntity(em, factionId);
            if (factionEntity != Entity.Null && em.HasBuffer<DynastyMemberRef>(factionEntity))
            {
                var buffer = em.GetBuffer<DynastyMemberRef>(factionEntity);
                for (int k = 0; k < buffer.Length; k++)
                {
                    var memberEntity = buffer[k].Member;
                    if (memberEntity == Entity.Null) continue;
                    if (!em.HasComponent<DynastyMemberComponent>(memberEntity)) continue;
                    var member = em.GetComponentData<DynastyMemberComponent>(memberEntity);
                    if (member.MemberId.Equals(memberId)) return member.Title;
                }
            }
            // Fallback: use the memberId itself as a title stand-in when no
            // roster is available or the member cannot be located. Smoke
            // fixtures without full dynasty seeding still produce a readable
            // message. Direct FixedString64Bytes assignment preserves the
            // UTF-8 bytes as-is (byte-by-byte Append promotes through the
            // integer overload and serialises each byte as a decimal).
            return memberId;
        }

        // ------------------------------------------------------------------ faction lookup

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
    }
}
