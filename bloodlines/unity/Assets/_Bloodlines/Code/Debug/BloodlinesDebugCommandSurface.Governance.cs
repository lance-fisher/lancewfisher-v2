using Bloodlines.Components;
using Bloodlines.TerritoryGovernance;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Debug
{
    /// <summary>
    /// Territory-governance readouts for governor seat assignment and control-point
    /// specialization state.
    /// </summary>
    public sealed partial class BloodlinesDebugCommandSurface
    {
        public bool TryDebugGetGovernorSpecialization(
            string controlPointId,
            out GovernorSpecializationComponent specialization)
        {
            specialization = default;
            if (string.IsNullOrWhiteSpace(controlPointId))
            {
                return false;
            }

            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated)
            {
                return false;
            }

            var entityManager = world.EntityManager;
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<ControlPointComponent>(),
                ComponentType.ReadOnly<GovernorSpecializationComponent>());
            using var entities = query.ToEntityArray(Allocator.Temp);
            using var controlPoints = query.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                if (!controlPoints[i].ControlPointId.Equals(controlPointId))
                {
                    continue;
                }

                specialization = entityManager.GetComponentData<GovernorSpecializationComponent>(entities[i]);
                return true;
            }

            return false;
        }

        public bool TryDebugGetGovernorAssignments(
            string factionId,
            out FixedString512Bytes summary)
        {
            summary = default;
            if (string.IsNullOrWhiteSpace(factionId))
            {
                return false;
            }

            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated)
            {
                return false;
            }

            var entityManager = world.EntityManager;
            bool wroteAny = false;

            var controlPointQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<ControlPointComponent>(),
                ComponentType.ReadOnly<GovernorSeatAssignmentComponent>());
            using var controlPointEntities = controlPointQuery.ToEntityArray(Allocator.Temp);
            using var controlPoints = controlPointQuery.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);
            for (int i = 0; i < controlPointEntities.Length; i++)
            {
                if (controlPoints[i].OwnerFactionId.ToString() != factionId)
                {
                    continue;
                }

                if (wroteAny)
                {
                    summary.Append("|");
                }

                GovernorSeatAssignmentComponent assignment =
                    entityManager.GetComponentData<GovernorSeatAssignmentComponent>(controlPointEntities[i]);
                summary.Append("ControlPoint:");
                summary.Append(controlPoints[i].ControlPointId);
                summary.Append("=");
                summary.Append(assignment.GovernorMemberId);
                summary.Append("@");
                summary.Append(GovernorSpecializationCanon.ResolveSpecializationLabel(assignment.SpecializationId));
                wroteAny = true;
            }

            var settlementQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<SettlementComponent>(),
                ComponentType.ReadOnly<GovernorSeatAssignmentComponent>());
            using var settlementEntities = settlementQuery.ToEntityArray(Allocator.Temp);
            using var settlementFactions = settlementQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var settlements = settlementQuery.ToComponentDataArray<SettlementComponent>(Allocator.Temp);
            for (int i = 0; i < settlementEntities.Length; i++)
            {
                if (settlementFactions[i].FactionId.ToString() != factionId)
                {
                    continue;
                }

                if (wroteAny)
                {
                    summary.Append("|");
                }

                GovernorSeatAssignmentComponent assignment =
                    entityManager.GetComponentData<GovernorSeatAssignmentComponent>(settlementEntities[i]);
                summary.Append("Settlement:");
                summary.Append(settlements[i].SettlementId);
                summary.Append("=");
                summary.Append(assignment.GovernorMemberId);
                summary.Append("@");
                summary.Append(GovernorSpecializationCanon.ResolveSpecializationLabel(assignment.SpecializationId));
                wroteAny = true;
            }

            return wroteAny;
        }
    }
}
