using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Producer / mutator helpers for the captor-faction
    /// CapturedMemberElement buffer. Mirrors the browser
    /// transferMemberToCaptor flow (simulation.js:4422-4453) plus the
    /// release / ransom-offer mutation paths consumers in sub-slices
    /// 23/24 will use.
    ///
    /// The helpers are static so they can be called from system
    /// OnUpdate paths without needing an ECB or system instance, in
    /// the same style as DynastyOperationLimits. Style choice mirrors
    /// the existing ai-strategic-layer surface for consistency.
    ///
    /// All mutation goes through CaptureMember / ReleaseCaptive so the
    /// buffer remains the single source of truth for captive state on
    /// the captor faction. Lookup goes through TryGetCaptive so callers
    /// do not need to walk the buffer themselves.
    ///
    /// The captor-faction lookup walks the FactionComponent query and
    /// matches on FactionId. A small number of factions per match makes
    /// the linear scan acceptable; if a future profile shows this in
    /// hot paths a faction-id index is the upgrade path.
    ///
    /// Browser reference:
    ///   simulation.js transferMemberToCaptor (~4422-4453): build the
    ///     captive record and unshift onto faction.dynasty.captives,
    ///     trimming to CAPTIVE_LEDGER_LIMIT.
    ///   simulation.js findCaptiveRecordByMemberId (~4068),
    ///     findCaptiveRecordById (~4072): lookup by member id or
    ///     captive record id.
    ///   simulation.js captives splice path (~4216-4226): remove a
    ///     captive when rescue / ransom resolves.
    /// </summary>
    public static class CapturedMemberHelpers
    {
        /// <summary>
        /// Append a Held captive to the captor faction's
        /// CapturedMemberElement buffer. Lazy-creates the buffer on the
        /// captor faction entity if missing. CapturedAtInWorldDays is
        /// stamped from the DualClock singleton (0 if no clock seeded).
        /// RansomCost defaults to 0; rescue/ransom dispatch slices set
        /// the canonical value when they land.
        ///
        /// Returns the entity index (0-based) the new entry occupies on
        /// the buffer for callers that want to address the entry
        /// directly. Returns -1 if the captor faction entity is not
        /// found, mirroring the browser early-return in
        /// transferMemberToCaptor when captorFaction.dynasty is missing.
        /// </summary>
        public static int CaptureMember(
            EntityManager em,
            FixedString32Bytes captorFactionId,
            FixedString64Bytes memberId,
            FixedString64Bytes memberTitle,
            FixedString32Bytes originFactionId)
        {
            var captorEntity = FindFactionEntity(em, captorFactionId);
            if (captorEntity == Entity.Null) return -1;

            EnsureBuffer(em, captorEntity);
            var buffer = em.GetBuffer<CapturedMemberElement>(captorEntity);
            buffer.Add(new CapturedMemberElement
            {
                MemberId              = memberId,
                MemberTitle           = memberTitle,
                OriginFactionId       = originFactionId,
                CapturedAtInWorldDays = GetInWorldDays(em),
                RansomCost            = 0f,
                Status                = CapturedMemberStatus.Held,
            });
            return buffer.Length - 1;
        }

        /// <summary>
        /// Look up a captive on the captor's buffer by member id. Walks
        /// the buffer and returns the first matching entry along with
        /// its index. Returns false when the captor entity does not
        /// exist, when no buffer is attached, or when no matching entry
        /// is found.
        ///
        /// Mirrors browser findCaptiveRecordByMemberId at
        /// simulation.js:4068-4070, with the addition of the buffer
        /// index for callers that want to mutate in place via
        /// ReleaseCaptive.
        /// </summary>
        public static bool TryGetCaptive(
            EntityManager em,
            FixedString32Bytes captorFactionId,
            FixedString64Bytes memberId,
            out CapturedMemberElement element,
            out int index)
        {
            element = default;
            index = -1;

            var captorEntity = FindFactionEntity(em, captorFactionId);
            if (captorEntity == Entity.Null) return false;
            if (!em.HasBuffer<CapturedMemberElement>(captorEntity)) return false;

            var buffer = em.GetBuffer<CapturedMemberElement>(captorEntity);
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i].MemberId.Equals(memberId))
                {
                    element = buffer[i];
                    index = i;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Mutate the status of a captive in place. The entry is
        /// retained on the buffer (audit retention) regardless of the
        /// new status. Browser parity:
        ///   - Released maps to the splice path
        ///     (simulation.js:4225) on rescue/ransom completion.
        ///     Unity retains the entry with Status=Released so
        ///     downstream renown / dynasty arc consumers can query
        ///     historical captures.
        ///   - Executed maps to the future captor-side execution path
        ///     (browser does not currently ship this path; Unity ports
        ///     the enum value for forward compatibility with sub-slice
        ///     25 captive execution work).
        ///   - RansomOffered maps to the
        ///     startCaptiveRansomOperation flow (sub-slice 24).
        ///
        /// Returns true when the captive was found and updated, false
        /// when the captor entity, buffer, or matching entry does not
        /// exist.
        /// </summary>
        public static bool ReleaseCaptive(
            EntityManager em,
            FixedString32Bytes captorFactionId,
            FixedString64Bytes memberId,
            CapturedMemberStatus newStatus)
        {
            var captorEntity = FindFactionEntity(em, captorFactionId);
            if (captorEntity == Entity.Null) return false;
            if (!em.HasBuffer<CapturedMemberElement>(captorEntity)) return false;

            var buffer = em.GetBuffer<CapturedMemberElement>(captorEntity);
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i].MemberId.Equals(memberId))
                {
                    var entry = buffer[i];
                    entry.Status = newStatus;
                    buffer[i] = entry;
                    return true;
                }
            }
            return false;
        }

        // ------------------------------------------------------------------ helpers

        private static void EnsureBuffer(EntityManager em, Entity captorEntity)
        {
            if (!em.HasBuffer<CapturedMemberElement>(captorEntity))
                em.AddBuffer<CapturedMemberElement>(captorEntity);
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
