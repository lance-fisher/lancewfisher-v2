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
    /// Consumes AICovertOpsComponent.LastFiredOp == CovertOpKind.LesserHousePromotion
    /// (written by AICovertOpsSystem in sub-slice 6) and executes the browser
    /// tryAiPromoteLesserHouse behavior (ai.js ~2784-2801) plus the simulation-side
    /// promoteMemberToLesserHouse sink (simulation.js ~7184-7258) at the
    /// mechanical level Unity tracks.
    ///
    /// Strategic gates (browser parity):
    ///   - faction dynasty legitimacy &lt; 90 (consolidated houses do not need
    ///     cadet branches; LESSER_HOUSE_LEGITIMACY_CEILING_BLOCK = 90).
    ///   - existing lesser house count &lt; 3 (LESSER_HOUSE_PROMOTION_CAP).
    ///   - at least one eligible candidate exists.
    ///
    /// Eligibility criteria for a dynasty member (browser
    /// memberIsLesserHouseCandidate ~6469):
    ///   - Status == Active or Ruling.
    ///   - Role != HeadOfBloodline (head continues the main line).
    ///   - Path is one of Governance, MilitaryCommand, CovertOperations
    ///     (browser LESSER_HOUSE_QUALIFYING_PATHS).
    ///   - Renown &gt;= 30 (browser LESSER_HOUSE_RENOWN_THRESHOLD).
    ///   - Member is not already the founder of an existing
    ///     LesserHouseElement on this faction (cross-reference, since Unity
    ///     does not track a per-member FoundedLesserHouseId field on
    ///     DynastyMemberComponent).
    ///
    /// Browser also gates on member.promotionHistory.length &gt;= 1
    /// (LESSER_HOUSE_MIN_PROMOTIONS). Unity has no promotion-history field
    /// yet; this gate is intentionally deferred. Renown &gt;= 30 is a strong
    /// proxy because canonical bootstrap members start at 7-22 renown and
    /// only growth-driven members reach the threshold.
    ///
    /// Effects on success (browser parity):
    ///   - Append a new LesserHouseElement to the faction's buffer with
    ///     Loyalty = 75 (LESSER_HOUSE_INITIAL_LOYALTY) so the existing
    ///     LesserHouseLoyaltyDriftSystem picks it up immediately.
    ///   - Faction DynastyState legitimacy +3 clamped to 100
    ///     (LESSER_HOUSE_LEGITIMACY_BONUS).
    ///   - Stewardship conviction +2 via ConvictionScoring.ApplyEvent
    ///     (browser recordConvictionEvent("stewardship", 2,
    ///     "lesser_house_founded")).
    ///
    /// Always clears LastFiredOp to None after processing regardless of
    /// outcome so one dispatch produces one execution attempt, matching the
    /// browser single-fire-per-timer pattern shared with sub-slices 8 and 9.
    ///
    /// Browser reference: ai.js tryAiPromoteLesserHouse (~2784-2801),
    /// simulation.js promoteMemberToLesserHouse (~7184-7258),
    /// memberIsLesserHouseCandidate (~6469-6479), constants at simulation.js
    /// ~6444-6457 (LESSER_HOUSE_RENOWN_THRESHOLD = 30,
    /// LESSER_HOUSE_LEGITIMACY_BONUS = 3,
    /// LESSER_HOUSE_INITIAL_LOYALTY = 75, qualifying paths set).
    ///
    /// Sub-slice 17 addition:
    ///   Narrative message push. After the mechanical effects are applied,
    ///     call NarrativeMessageBridge.Push with the browser-parity founding
    ///     line: "<factionId> founds <lesserHouseName>, honoring
    ///     <founderTitle>." Tone is Good when the faction is "player", else
    ///     Info (matches browser simulation.js:7251-7255).
    ///
    /// Deferred to later slices:
    ///   - Per-member FoundedLesserHouseId field on DynastyMemberComponent
    ///     (cross-reference is the workaround for now).
    ///   - Promotion-history gate (LESSER_HOUSE_MIN_PROMOTIONS).
    ///   - Marital anchor and world-pressure cadet instability profiles
    ///     (browser computes these inline; Unity will layer them when the
    ///     marital-anchor and cadet world-pressure systems land).
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AICovertOpsSystem))]
    public partial struct AILesserHousePromotionSystem : ISystem
    {
        public const float LegitimacyCeilingBlock = 90f;
        public const int   LesserHouseCap = 3;
        public const float RenownThreshold = 30f;
        public const float LegitimacyBonus = 3f;
        public const float LegitimacyCeiling = 100f;
        public const float InitialLoyalty = 75f;
        public const float StewardshipBonus = 2f;

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
                var factionEntity = dispatchEntities[i];
                var covert = em.GetComponentData<AICovertOpsComponent>(factionEntity);
                if (covert.LastFiredOp != CovertOpKind.LesserHousePromotion)
                    continue;

                TryPromote(em, factionEntity, inWorldDays);

                covert.LastFiredOp = CovertOpKind.None;
                em.SetComponentData(factionEntity, covert);
            }

            dispatchEntities.Dispose();
        }

        // ------------------------------------------------------------------ promote

        private static void TryPromote(
            EntityManager em,
            Entity factionEntity,
            float inWorldDays)
        {
            // Gate 1: dynasty legitimacy below the consolidation ceiling.
            if (!em.HasComponent<DynastyStateComponent>(factionEntity)) return;
            var dynasty = em.GetComponentData<DynastyStateComponent>(factionEntity);
            if (dynasty.Legitimacy >= LegitimacyCeilingBlock) return;

            // Gate 2: existing lesser house count below the cap. Defected
            // houses still count toward the cap because the browser treats
            // dynasty.lesserHouses as the source of truth without filtering.
            if (!em.HasBuffer<LesserHouseElement>(factionEntity)) return;
            var lesserHouseBuffer = em.GetBuffer<LesserHouseElement>(factionEntity);
            if (lesserHouseBuffer.Length >= LesserHouseCap) return;

            // Gate 3: dynasty roster must be present.
            if (!em.HasBuffer<DynastyMemberRef>(factionEntity)) return;

            // Build the set of member ids already used as founders so we
            // skip them. Browser tracks this via member.foundedLesserHouseId
            // on the member object; Unity cross-references the buffer.
            var existingFounderIds = new NativeHashSet<FixedString64Bytes>(
                lesserHouseBuffer.Length, Allocator.Temp);
            for (int j = 0; j < lesserHouseBuffer.Length; j++)
                existingFounderIds.Add(lesserHouseBuffer[j].FounderMemberId);

            // Walk the roster in order and pick the first eligible member.
            var memberBuffer = em.GetBuffer<DynastyMemberRef>(factionEntity);
            FixedString64Bytes founderMemberId = default;
            FixedString64Bytes founderTitle = default;
            bool found = false;

            for (int k = 0; k < memberBuffer.Length; k++)
            {
                var memberEntity = memberBuffer[k].Member;
                if (memberEntity == Entity.Null) continue;
                if (!em.HasComponent<DynastyMemberComponent>(memberEntity)) continue;

                var member = em.GetComponentData<DynastyMemberComponent>(memberEntity);
                if (!IsEligible(member)) continue;
                if (existingFounderIds.Contains(member.MemberId)) continue;

                founderMemberId = member.MemberId;
                founderTitle    = member.Title;
                found = true;
                break;
            }

            existingFounderIds.Dispose();
            if (!found) return;

            // Append the new lesser-house element. LesserHouseLoyaltyDriftSystem
            // picks it up next in-world-day boundary.
            lesserHouseBuffer.Add(new LesserHouseElement
            {
                HouseId        = BuildLesserHouseId(founderMemberId),
                HouseName      = BuildLesserHouseName(founderTitle),
                FounderMemberId = founderMemberId,
                Loyalty        = InitialLoyalty,
                DailyLoyaltyDelta = 0f,
                LastDriftAppliedInWorldDays = inWorldDays,
                Defected       = false,
            });

            // Apply legitimacy +3 clamped to 100. Browser adjustLegitimacy
            // also clamps low at 0, but a +3 cannot go negative.
            dynasty.Legitimacy = math.min(LegitimacyCeiling, dynasty.Legitimacy + LegitimacyBonus);
            em.SetComponentData(factionEntity, dynasty);

            // Stewardship conviction +2.
            if (em.HasComponent<ConvictionComponent>(factionEntity))
            {
                var conviction = em.GetComponentData<ConvictionComponent>(factionEntity);
                ConvictionScoring.ApplyEvent(
                    ref conviction,
                    ConvictionBucket.Stewardship,
                    StewardshipBonus);
                em.SetComponentData(factionEntity, conviction);
            }

            // Narrative push (sub-slice 17). Browser pushMessage at
            // simulation.js:7251-7255.
            PushPromotionMessage(em, factionEntity, founderTitle);
        }

        private static void PushPromotionMessage(
            EntityManager em,
            Entity factionEntity,
            FixedString64Bytes founderTitle)
        {
            if (!em.HasComponent<FactionComponent>(factionEntity)) return;
            var factionId = em.GetComponentData<FactionComponent>(factionEntity).FactionId;

            // The new lesser-house element is the last entry just appended.
            var lesserHouseBuffer = em.GetBuffer<LesserHouseElement>(factionEntity);
            if (lesserHouseBuffer.Length == 0) return;
            var lesserHouseName = lesserHouseBuffer[lesserHouseBuffer.Length - 1].HouseName;

            var message = new FixedString128Bytes();
            message.Append(factionId);
            message.Append((FixedString32Bytes)" founds ");
            message.Append(lesserHouseName);
            message.Append((FixedString32Bytes)", honoring ");
            message.Append(founderTitle);
            message.Append((FixedString32Bytes)".");

            var tone = factionId.Equals(new FixedString32Bytes("player"))
                ? NarrativeMessageTone.Good
                : NarrativeMessageTone.Info;

            NarrativeMessageBridge.Push(em, message, tone);
        }

        // ------------------------------------------------------------------ helpers

        private static bool IsEligible(in DynastyMemberComponent member)
        {
            if (member.Role == DynastyRole.HeadOfBloodline) return false;
            if (member.Status != DynastyMemberStatus.Active &&
                member.Status != DynastyMemberStatus.Ruling) return false;
            if (member.Renown < RenownThreshold) return false;
            if (!IsQualifyingPath(member.Path)) return false;
            return true;
        }

        private static bool IsQualifyingPath(DynastyPath path)
        {
            return path == DynastyPath.Governance
                || path == DynastyPath.MilitaryCommand
                || path == DynastyPath.CovertOperations;
        }

        private static FixedString32Bytes BuildLesserHouseId(FixedString64Bytes founderMemberId)
        {
            var id = new FixedString32Bytes("lh-");
            for (int k = 0; k < founderMemberId.Length && id.Length < 28; k++)
                id.Append(founderMemberId[k]);
            return id;
        }

        private static FixedString64Bytes BuildLesserHouseName(FixedString64Bytes founderTitle)
        {
            var name = new FixedString64Bytes("Cadet of ");
            for (int k = 0; k < founderTitle.Length && name.Length < 60; k++)
                name.Append(founderTitle[k]);
            return name;
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
