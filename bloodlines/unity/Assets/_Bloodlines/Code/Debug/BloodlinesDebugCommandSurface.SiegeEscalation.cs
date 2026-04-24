using Bloodlines.Components;
using Bloodlines.Siege;
using Unity.Entities;

namespace Bloodlines.Debug
{
    /// <summary>
    /// Debug command surface extension for the siege-escalation lane.
    /// Exposes settlement siege escalation state for smoke validators and tooling.
    /// </summary>
    public sealed partial class BloodlinesDebugCommandSurface
    {
        public bool TryDebugGetSiegeEscalation(
            string settlementId,
            out float durationDays,
            out byte escalationTier,
            out float starvationMultiplier,
            out float desertionThresholdPct,
            out float moralePenaltyPerDay)
        {
            durationDays = 0f;
            escalationTier = 0;
            starvationMultiplier = SiegeEscalationCanon.NormalStarvationMultiplier;
            desertionThresholdPct = SiegeEscalationCanon.NormalDesertionThresholdPct;
            moralePenaltyPerDay = SiegeEscalationCanon.NormalMoralePenaltyPerDay;

            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            Entity settlementEntity = FindSettlementEntity(entityManager, settlementId);
            if (settlementEntity == Entity.Null ||
                !entityManager.HasComponent<SiegeEscalationComponent>(settlementEntity))
            {
                return false;
            }

            var escalation = entityManager.GetComponentData<SiegeEscalationComponent>(settlementEntity);
            durationDays = escalation.SiegeDurationInWorldDays;
            escalationTier = escalation.EscalationTier;
            starvationMultiplier = escalation.StarvationMultiplier;
            desertionThresholdPct = escalation.DesertionThresholdPct;
            moralePenaltyPerDay = escalation.MoralePenaltyPerDay;
            return true;
        }

        public bool TryDebugGetFactionSiegeEscalationState(
            string factionId,
            out float starvationMultiplier)
        {
            starvationMultiplier = SiegeEscalationCanon.NormalStarvationMultiplier;

            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            var query = entityManager.CreateEntityQuery(
                Unity.Collections.ComponentType.ReadOnly<FactionComponent>(),
                Unity.Collections.ComponentType.ReadOnly<FactionSiegeEscalationStateComponent>());
            using var factionEntities = query.ToEntityArray(Unity.Collections.Allocator.Temp);
            using var factionComponents = query.ToComponentDataArray<FactionComponent>(Unity.Collections.Allocator.Temp);
            using var escalationStates = query.ToComponentDataArray<FactionSiegeEscalationStateComponent>(Unity.Collections.Allocator.Temp);
            query.Dispose();

            for (int i = 0; i < factionComponents.Length; i++)
            {
                if (factionComponents[i].FactionId.Equals(new Unity.Collections.FixedString32Bytes(factionId)))
                {
                    starvationMultiplier = escalationStates[i].StarvationMultiplier;
                    return true;
                }
            }

            return false;
        }
    }
}
