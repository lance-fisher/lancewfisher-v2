using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Fortification
{
    public enum ImminentEngagementPostureId : byte
    {
        Steady = 0,
        Brace = 1,
        Counterstroke = 2,
    }

    /// <summary>
    /// Canonical imminent-engagement posture payload materialized on fortified settlements
    /// while their warning window is active.
    /// </summary>
    public struct ImminentEngagementPostureComponent : IComponentData
    {
        public byte PostureId;
        public float ReserveHealMultiplier;
        public float MusteringSpeedMultiplier;
        public float FrontlineBonusMultiplier;
        public float RetreatThresholdMultiplier;
        public float DesiredFrontlineBonus;
        public float RetreatThresholdOffset;
        public bool AutoSortieOnExpiry;
    }

    public static class ImminentEngagementPostureUtility
    {
        public static readonly FixedString32Bytes SteadyResponseId = new("steady");
        public static readonly FixedString32Bytes BraceResponseId = new("brace");
        public static readonly FixedString32Bytes CounterstrokeResponseId = new("counterstroke");
        public static readonly FixedString64Bytes SteadyLabel = new(ImminentEngagementCanon.Steady.Label);
        public static readonly FixedString64Bytes BraceLabel = new(ImminentEngagementCanon.Brace.Label);
        public static readonly FixedString64Bytes CounterstrokeLabel = new(ImminentEngagementCanon.Counterstroke.Label);

        public static bool IsValidPostureId(byte postureId)
        {
            return postureId <= (byte)ImminentEngagementPostureId.Counterstroke;
        }

        public static byte ResolvePostureId(FixedString32Bytes responseId)
        {
            if (responseId.Equals(BraceResponseId))
            {
                return (byte)ImminentEngagementPostureId.Brace;
            }

            if (responseId.Equals(CounterstrokeResponseId))
            {
                return (byte)ImminentEngagementPostureId.Counterstroke;
            }

            return (byte)ImminentEngagementPostureId.Steady;
        }

        public static FixedString32Bytes ResolveResponseId(byte postureId)
        {
            return postureId switch
            {
                (byte)ImminentEngagementPostureId.Brace => BraceResponseId,
                (byte)ImminentEngagementPostureId.Counterstroke => CounterstrokeResponseId,
                _ => SteadyResponseId,
            };
        }

        public static FixedString64Bytes ResolveResponseLabel(byte postureId)
        {
            return postureId switch
            {
                (byte)ImminentEngagementPostureId.Brace => BraceLabel,
                (byte)ImminentEngagementPostureId.Counterstroke => CounterstrokeLabel,
                _ => SteadyLabel,
            };
        }

        public static ImminentEngagementPostureComponent ResolveComponent(FixedString32Bytes responseId)
        {
            return ResolveComponent(ResolvePostureId(responseId));
        }

        public static ImminentEngagementPostureComponent ResolveComponent(byte postureId)
        {
            return postureId switch
            {
                (byte)ImminentEngagementPostureId.Brace => new ImminentEngagementPostureComponent
                {
                    PostureId = postureId,
                    ReserveHealMultiplier = ImminentEngagementCanon.Brace.ReserveHealMultiplier,
                    MusteringSpeedMultiplier = ImminentEngagementCanon.Brace.ReserveMusterMultiplier,
                    FrontlineBonusMultiplier = ImminentEngagementCanon.Brace.DefenderAttackMultiplier,
                    RetreatThresholdMultiplier = 1.05f,
                    DesiredFrontlineBonus = ImminentEngagementCanon.Brace.DesiredFrontlineBonus,
                    RetreatThresholdOffset = ImminentEngagementCanon.Brace.RetreatThresholdBonus,
                    AutoSortieOnExpiry = ImminentEngagementCanon.Brace.AutoSortieOnExpiry,
                },
                (byte)ImminentEngagementPostureId.Counterstroke => new ImminentEngagementPostureComponent
                {
                    PostureId = postureId,
                    ReserveHealMultiplier = ImminentEngagementCanon.Counterstroke.ReserveHealMultiplier,
                    MusteringSpeedMultiplier = ImminentEngagementCanon.Counterstroke.ReserveMusterMultiplier,
                    FrontlineBonusMultiplier = ImminentEngagementCanon.Counterstroke.DefenderAttackMultiplier,
                    RetreatThresholdMultiplier = 0.98f,
                    DesiredFrontlineBonus = ImminentEngagementCanon.Counterstroke.DesiredFrontlineBonus,
                    RetreatThresholdOffset = ImminentEngagementCanon.Counterstroke.RetreatThresholdBonus,
                    AutoSortieOnExpiry = ImminentEngagementCanon.Counterstroke.AutoSortieOnExpiry,
                },
                _ => new ImminentEngagementPostureComponent
                {
                    PostureId = (byte)ImminentEngagementPostureId.Steady,
                    ReserveHealMultiplier = ImminentEngagementCanon.Steady.ReserveHealMultiplier,
                    MusteringSpeedMultiplier = ImminentEngagementCanon.Steady.ReserveMusterMultiplier,
                    FrontlineBonusMultiplier = ImminentEngagementCanon.Steady.DefenderAttackMultiplier,
                    RetreatThresholdMultiplier = 1f,
                    DesiredFrontlineBonus = ImminentEngagementCanon.Steady.DesiredFrontlineBonus,
                    RetreatThresholdOffset = ImminentEngagementCanon.Steady.RetreatThresholdBonus,
                    AutoSortieOnExpiry = ImminentEngagementCanon.Steady.AutoSortieOnExpiry,
                },
            };
        }

        public static bool TryGetSettlementPosture(
            EntityManager entityManager,
            Entity settlementEntity,
            out ImminentEngagementPostureComponent posture)
        {
            posture = ResolveComponent((byte)ImminentEngagementPostureId.Steady);
            if (settlementEntity == Entity.Null ||
                !entityManager.Exists(settlementEntity) ||
                !entityManager.HasComponent<ImminentEngagementPostureComponent>(settlementEntity))
            {
                return false;
            }

            posture = entityManager.GetComponentData<ImminentEngagementPostureComponent>(settlementEntity);
            return true;
        }

        public static bool TryGetFrontlineCombatantPosture(
            EntityManager entityManager,
            Entity combatantEntity,
            out ImminentEngagementPostureComponent posture)
        {
            posture = ResolveComponent((byte)ImminentEngagementPostureId.Steady);
            if (combatantEntity == Entity.Null ||
                !entityManager.Exists(combatantEntity) ||
                !entityManager.HasComponent<FortificationSettlementLinkComponent>(combatantEntity) ||
                !entityManager.HasComponent<FortificationReserveAssignmentComponent>(combatantEntity))
            {
                return false;
            }

            var assignment = entityManager.GetComponentData<FortificationReserveAssignmentComponent>(combatantEntity);
            if (assignment.Duty != ReserveDutyState.Engaged &&
                assignment.Duty != ReserveDutyState.Muster)
            {
                return false;
            }

            var link = entityManager.GetComponentData<FortificationSettlementLinkComponent>(combatantEntity);
            if (!TryGetSettlementPosture(entityManager, link.SettlementEntity, out posture) ||
                !entityManager.HasComponent<ImminentEngagementComponent>(link.SettlementEntity))
            {
                return false;
            }

            return entityManager.GetComponentData<ImminentEngagementComponent>(link.SettlementEntity).Active;
        }

        public static bool TryGetRetreatPosture(
            EntityManager entityManager,
            Entity combatantEntity,
            out ImminentEngagementPostureComponent posture)
        {
            posture = ResolveComponent((byte)ImminentEngagementPostureId.Steady);
            if (combatantEntity == Entity.Null ||
                !entityManager.Exists(combatantEntity) ||
                !entityManager.HasComponent<FortificationSettlementLinkComponent>(combatantEntity))
            {
                return false;
            }

            var link = entityManager.GetComponentData<FortificationSettlementLinkComponent>(combatantEntity);
            if (!TryGetSettlementPosture(entityManager, link.SettlementEntity, out posture) ||
                !entityManager.HasComponent<ImminentEngagementComponent>(link.SettlementEntity))
            {
                return false;
            }

            return entityManager.GetComponentData<ImminentEngagementComponent>(link.SettlementEntity).Active;
        }

        public static float ApplyRetreatThreshold(
            float baseThreshold,
            in ImminentEngagementPostureComponent posture)
        {
            return math.saturate(baseThreshold + posture.RetreatThresholdOffset);
        }
    }
}
