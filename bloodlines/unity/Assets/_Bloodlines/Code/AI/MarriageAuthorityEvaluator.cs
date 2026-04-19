using Bloodlines.Components;
using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Resolves the governance authority that ratifies a marriage accept for a
    /// target faction. Simplified port of simulation.js getMarriageAuthorityProfile
    /// (~6134). Unity has no explicit regency state component yet; the authority
    /// profile is derived from the dynasty member roster alone:
    ///
    ///   1. HeadOfBloodline with status Ruling -> HeadDirect (cost 0)
    ///   2. HeirDesignate present in any active status -> HeirRegency (cost 1)
    ///   3. Diplomat present in any active status -> EnvoyRegency (cost 2)
    ///   4. Otherwise -> None (accept is rejected)
    ///
    /// Backward-compatibility carve-out: if the faction entity carries no
    /// DynastyMemberRef buffer (which the sub-slice 11 accept-effects smoke
    /// and other synthetic test fixtures omit), default to HeadDirect with
    /// cost 0. Runtime bootstrapped factions always seed the canonical
    /// eight-member dynasty set via DynastyBootstrap.AttachDynasty so the
    /// strict path fires in real matches.
    ///
    /// Browser constants: MARRIAGE_REGENCY_LEGITIMACY_COSTS at simulation.js:6091.
    /// </summary>
    public static class MarriageAuthorityEvaluator
    {
        public const float HeadDirectLegitimacyCost = 0f;
        public const float HeirRegencyLegitimacyCost = 1f;
        public const float EnvoyRegencyLegitimacyCost = 2f;

        /// <summary>
        /// Resolve the authority terms for a faction. Returns true when the
        /// faction has at least one authority path (or has no dynasty roster
        /// seeded, in which case the backward-compatible HeadDirect default
        /// applies). Returns false only when the roster exists but yields no
        /// head-direct, heir, or envoy candidate; callers must reject the
        /// accept in that case.
        /// </summary>
        public static bool TryResolve(
            EntityManager em,
            Entity factionEntity,
            out MarriageAuthorityMode mode,
            out float legitimacyCost)
        {
            mode = MarriageAuthorityMode.HeadDirect;
            legitimacyCost = HeadDirectLegitimacyCost;

            if (factionEntity == Entity.Null)
                return false;

            if (!em.HasBuffer<DynastyMemberRef>(factionEntity))
            {
                // No roster seeded; synthetic tests and pre-bootstrap factions
                // keep the head-direct default so sub-slice 11 behavior is
                // preserved.
                return true;
            }

            bool hasHeir = false;
            bool hasEnvoy = false;

            var buffer = em.GetBuffer<DynastyMemberRef>(factionEntity);
            for (int i = 0; i < buffer.Length; i++)
            {
                var memberEntity = buffer[i].Member;
                if (memberEntity == Entity.Null) continue;
                if (!em.HasComponent<DynastyMemberComponent>(memberEntity)) continue;

                var member = em.GetComponentData<DynastyMemberComponent>(memberEntity);

                if (member.Role == DynastyRole.HeadOfBloodline &&
                    member.Status == DynastyMemberStatus.Ruling)
                {
                    mode = MarriageAuthorityMode.HeadDirect;
                    legitimacyCost = HeadDirectLegitimacyCost;
                    return true;
                }

                if (member.Role == DynastyRole.HeirDesignate &&
                    IsAvailable(member.Status))
                {
                    hasHeir = true;
                }
                else if (member.Role == DynastyRole.Diplomat &&
                         IsAvailable(member.Status))
                {
                    hasEnvoy = true;
                }
            }

            if (hasHeir)
            {
                mode = MarriageAuthorityMode.HeirRegency;
                legitimacyCost = HeirRegencyLegitimacyCost;
                return true;
            }

            if (hasEnvoy)
            {
                mode = MarriageAuthorityMode.EnvoyRegency;
                legitimacyCost = EnvoyRegencyLegitimacyCost;
                return true;
            }

            mode = MarriageAuthorityMode.None;
            legitimacyCost = 0f;
            return false;
        }

        private static bool IsAvailable(DynastyMemberStatus status)
        {
            return status == DynastyMemberStatus.Active
                || status == DynastyMemberStatus.Ruling;
        }
    }
}
