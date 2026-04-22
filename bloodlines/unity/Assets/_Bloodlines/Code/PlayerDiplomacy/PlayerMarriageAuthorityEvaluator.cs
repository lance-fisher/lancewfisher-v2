using Bloodlines.Components;
using Unity.Entities;

namespace Bloodlines.PlayerDiplomacy
{
    public enum PlayerMarriageAuthorityMode : byte
    {
        None = 0,
        HeadDirect = 1,
        HeirRegency = 2,
        EnvoyRegency = 3,
    }

    public struct PlayerMarriageAuthorityProfile
    {
        public bool Available;
        public PlayerMarriageAuthorityMode Mode;
        public float LegitimacyCost;
    }

    public struct PlayerMarriageEnvoyProfile
    {
        public bool Available;
    }

    public struct PlayerMarriageGovernanceStatus
    {
        public PlayerMarriageAuthorityProfile Authority;
        public PlayerMarriageEnvoyProfile Envoy;
        public bool CanOfferMarriage;
    }

    /// <summary>
    /// Player-side mirror of the browser marriage governance helpers:
    /// getMarriageAuthorityProfile, getMarriageEnvoyProfile, and
    /// buildMarriageGovernanceStatus. Kept under the player-diplomacy lane so
    /// we do not write into the AI-owned authority evaluator.
    /// </summary>
    public static class PlayerMarriageAuthorityEvaluator
    {
        public const float HeadDirectLegitimacyCost = 0f;
        public const float HeirRegencyLegitimacyCost = 1f;
        public const float EnvoyRegencyLegitimacyCost = 2f;

        public static PlayerMarriageGovernanceStatus BuildGovernanceStatus(
            EntityManager entityManager,
            Entity factionEntity)
        {
            var authority = ResolveAuthority(entityManager, factionEntity);
            var envoy = ResolveEnvoy(entityManager, factionEntity);
            return new PlayerMarriageGovernanceStatus
            {
                Authority = authority,
                Envoy = envoy,
                CanOfferMarriage = authority.Available && envoy.Available,
            };
        }

        public static bool FactionAllowsPolygamy(EntityManager entityManager, Entity factionEntity)
        {
            if (factionEntity == Entity.Null ||
                !entityManager.HasComponent<FaithStateComponent>(factionEntity))
            {
                return false;
            }

            var faith = entityManager.GetComponentData<FaithStateComponent>(factionEntity);
            return faith.SelectedFaith == CovenantId.BloodDominion ||
                   faith.SelectedFaith == CovenantId.TheWild;
        }

        private static PlayerMarriageAuthorityProfile ResolveAuthority(
            EntityManager entityManager,
            Entity factionEntity)
        {
            if (factionEntity == Entity.Null ||
                !entityManager.HasBuffer<DynastyMemberRef>(factionEntity))
            {
                return default;
            }

            bool hasHeir = false;
            bool hasEnvoy = false;

            var members = entityManager.GetBuffer<DynastyMemberRef>(factionEntity);
            for (int i = 0; i < members.Length; i++)
            {
                var memberEntity = members[i].Member;
                if (memberEntity == Entity.Null ||
                    !entityManager.HasComponent<DynastyMemberComponent>(memberEntity))
                {
                    continue;
                }

                var member = entityManager.GetComponentData<DynastyMemberComponent>(memberEntity);
                if (member.Role == DynastyRole.HeadOfBloodline &&
                    member.Status == DynastyMemberStatus.Ruling)
                {
                    return new PlayerMarriageAuthorityProfile
                    {
                        Available = true,
                        Mode = PlayerMarriageAuthorityMode.HeadDirect,
                        LegitimacyCost = HeadDirectLegitimacyCost,
                    };
                }

                if (member.Role == DynastyRole.HeirDesignate && IsAvailable(member.Status))
                {
                    hasHeir = true;
                    continue;
                }

                if (member.Role == DynastyRole.Diplomat && IsAvailable(member.Status))
                {
                    hasEnvoy = true;
                }
            }

            if (hasHeir)
            {
                return new PlayerMarriageAuthorityProfile
                {
                    Available = true,
                    Mode = PlayerMarriageAuthorityMode.HeirRegency,
                    LegitimacyCost = HeirRegencyLegitimacyCost,
                };
            }

            if (hasEnvoy)
            {
                return new PlayerMarriageAuthorityProfile
                {
                    Available = true,
                    Mode = PlayerMarriageAuthorityMode.EnvoyRegency,
                    LegitimacyCost = EnvoyRegencyLegitimacyCost,
                };
            }

            return default;
        }

        private static PlayerMarriageEnvoyProfile ResolveEnvoy(
            EntityManager entityManager,
            Entity factionEntity)
        {
            if (factionEntity == Entity.Null ||
                !entityManager.HasBuffer<DynastyMemberRef>(factionEntity))
            {
                return default;
            }

            var members = entityManager.GetBuffer<DynastyMemberRef>(factionEntity);
            for (int i = 0; i < members.Length; i++)
            {
                var memberEntity = members[i].Member;
                if (memberEntity == Entity.Null ||
                    !entityManager.HasComponent<DynastyMemberComponent>(memberEntity))
                {
                    continue;
                }

                var member = entityManager.GetComponentData<DynastyMemberComponent>(memberEntity);
                if (member.Role == DynastyRole.Diplomat && IsAvailable(member.Status))
                {
                    return new PlayerMarriageEnvoyProfile
                    {
                        Available = true,
                    };
                }
            }

            return default;
        }

        private static bool IsAvailable(DynastyMemberStatus status)
        {
            return status == DynastyMemberStatus.Active ||
                   status == DynastyMemberStatus.Ruling;
        }
    }
}
