using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.TerritoryGovernance
{
    public static class GovernorSpecializationCanon
    {
        public const float GovernorTrickleBaseBonus = 1.22f;
        public const float GovernorStabilizationBaseBonus = 1.3f;

        public static GovernorSpecializationComponent DefaultProfile => new()
        {
            GovernorMemberId = default,
            SpecializationId = GovernorSpecializationId.None,
            ResourceTrickleMultiplier = 1f,
            StabilizationMultiplier = 1f,
            CaptureResistanceBonus = 1f,
            LoyaltyProtectionMultiplier = 1f,
            ReserveRegenMultiplier = 1f,
            HealRegenMultiplier = 1f,
        };

        public static GovernorSpecializationId ResolveSpecialization(FixedString32Bytes settlementClassId)
        {
            if (settlementClassId.Equals(new FixedString32Bytes("border_settlement")) ||
                settlementClassId.Equals(new FixedString32Bytes("military_fort")))
            {
                return GovernorSpecializationId.BorderMarshal;
            }

            if (settlementClassId.Equals(new FixedString32Bytes("trade_town")) ||
                settlementClassId.Equals(new FixedString32Bytes("regional_stronghold")))
            {
                return GovernorSpecializationId.CivicSteward;
            }

            if (settlementClassId.Equals(new FixedString32Bytes("primary_dynastic_keep")) ||
                settlementClassId.Equals(new FixedString32Bytes("fortress_citadel")))
            {
                return GovernorSpecializationId.KeepCastellan;
            }

            return GovernorSpecializationId.None;
        }

        public static FixedString32Bytes ResolveSpecializationLabel(GovernorSpecializationId specializationId)
        {
            return specializationId switch
            {
                GovernorSpecializationId.BorderMarshal => new FixedString32Bytes("Border Marshal"),
                GovernorSpecializationId.CivicSteward => new FixedString32Bytes("Civic Steward"),
                GovernorSpecializationId.KeepCastellan => new FixedString32Bytes("Keep Castellan"),
                _ => new FixedString32Bytes("General Stewardship"),
            };
        }

        public static GovernorSpecializationComponent GetProfile(
            GovernorSpecializationId specializationId,
            FixedString64Bytes governorMemberId)
        {
            GovernorSpecializationComponent profile = DefaultProfile;
            profile.GovernorMemberId = governorMemberId;
            profile.SpecializationId = specializationId;

            switch (specializationId)
            {
                case GovernorSpecializationId.BorderMarshal:
                    profile.ResourceTrickleMultiplier = 1.08f;
                    profile.StabilizationMultiplier = 1.20f;
                    profile.CaptureResistanceBonus = 1.16f;
                    profile.LoyaltyProtectionMultiplier = 1.08f;
                    profile.ReserveRegenMultiplier = 1.06f;
                    profile.HealRegenMultiplier = 1.03f;
                    break;
                case GovernorSpecializationId.CivicSteward:
                    profile.ResourceTrickleMultiplier = 1.18f;
                    profile.StabilizationMultiplier = 1.14f;
                    profile.CaptureResistanceBonus = 1.08f;
                    profile.LoyaltyProtectionMultiplier = 1.18f;
                    profile.ReserveRegenMultiplier = 1f;
                    profile.HealRegenMultiplier = 1f;
                    break;
                case GovernorSpecializationId.KeepCastellan:
                    profile.ResourceTrickleMultiplier = 1.05f;
                    profile.StabilizationMultiplier = 1.12f;
                    profile.CaptureResistanceBonus = 1.20f;
                    profile.LoyaltyProtectionMultiplier = 1.24f;
                    profile.ReserveRegenMultiplier = 1.18f;
                    profile.HealRegenMultiplier = 1.18f;
                    break;
            }

            return profile;
        }

        public static float GetGovernanceSeatMemberScore(
            DynastyRole role,
            float renown,
            GovernorSpecializationId specializationId,
            GovernanceAnchorType anchorType)
        {
            float specializationScore = role switch
            {
                DynastyRole.Governor => specializationId switch
                {
                    GovernorSpecializationId.KeepCastellan => 6f,
                    GovernorSpecializationId.CivicSteward => 5f,
                    GovernorSpecializationId.BorderMarshal => 4f,
                    _ => 0f,
                },
                DynastyRole.HeadOfBloodline => specializationId switch
                {
                    GovernorSpecializationId.KeepCastellan => 5f,
                    GovernorSpecializationId.CivicSteward => 4f,
                    GovernorSpecializationId.BorderMarshal => 3f,
                    _ => 0f,
                },
                DynastyRole.HeirDesignate => specializationId switch
                {
                    GovernorSpecializationId.KeepCastellan => 3f,
                    GovernorSpecializationId.CivicSteward => 2f,
                    GovernorSpecializationId.BorderMarshal => 5f,
                    _ => 0f,
                },
                DynastyRole.Diplomat => specializationId switch
                {
                    GovernorSpecializationId.KeepCastellan => 3f,
                    GovernorSpecializationId.CivicSteward => 5f,
                    GovernorSpecializationId.BorderMarshal => 2f,
                    _ => 0f,
                },
                DynastyRole.Merchant => specializationId switch
                {
                    GovernorSpecializationId.KeepCastellan => 2f,
                    GovernorSpecializationId.CivicSteward => 4f,
                    GovernorSpecializationId.BorderMarshal => 3f,
                    _ => 0f,
                },
                _ => 0f,
            };

            float anchorBias = anchorType == GovernanceAnchorType.Settlement ? 0.6f : 0f;
            return specializationScore + anchorBias + (renown * 0.01f);
        }

        public static float ApplyLoyaltyProtection(float loyaltyDelta, float protectionMultiplier)
        {
            if (loyaltyDelta >= 0f)
            {
                return loyaltyDelta;
            }

            return loyaltyDelta / math.max(1f, protectionMultiplier);
        }

        public static GovernorSpecializationComponent GetSettlementProfile(
            EntityManager entityManager,
            Entity settlementEntity)
        {
            if (!entityManager.HasComponent<GovernorSeatAssignmentComponent>(settlementEntity))
            {
                return DefaultProfile;
            }

            GovernorSeatAssignmentComponent assignment =
                entityManager.GetComponentData<GovernorSeatAssignmentComponent>(settlementEntity);
            return GetProfile(assignment.SpecializationId, assignment.GovernorMemberId);
        }

        public static GovernorSpecializationComponent GetControlPointProfile(
            EntityManager entityManager,
            Entity controlPointEntity)
        {
            return entityManager.HasComponent<GovernorSpecializationComponent>(controlPointEntity)
                ? entityManager.GetComponentData<GovernorSpecializationComponent>(controlPointEntity)
                : DefaultProfile;
        }
    }
}
