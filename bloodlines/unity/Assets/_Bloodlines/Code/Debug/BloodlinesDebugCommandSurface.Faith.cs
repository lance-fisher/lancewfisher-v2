using Bloodlines.Components;
using Bloodlines.Faith;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Debug
{
    /// <summary>
    /// Debug command surface extension for the faith lane. Lets governed smoke
    /// validators and future UI tooling record exposure, force commitment, and
    /// inspect commitment state on a faction without bypassing the canonical
    /// helpers.
    ///
    /// Browser reference: simulation.js chooseFaithCommitment (9694) and
    /// updateFaithExposure (8174).
    /// </summary>
    public sealed partial class BloodlinesDebugCommandSurface
    {
        public bool TryDebugGetFaithState(string factionId, out FaithStateComponent state)
        {
            state = default;
            var factionEntity = FindFactionEntityByFaith(factionId, out var entityManager);
            if (factionEntity == Entity.Null ||
                !entityManager.HasComponent<FaithStateComponent>(factionEntity))
            {
                return false;
            }

            state = entityManager.GetComponentData<FaithStateComponent>(factionEntity);
            return true;
        }

        public bool TryDebugGetFaithExposure(string factionId, CovenantId target, out float exposure)
        {
            exposure = 0f;
            var factionEntity = FindFactionEntityByFaith(factionId, out var entityManager);
            if (factionEntity == Entity.Null ||
                !entityManager.HasBuffer<FaithExposureElement>(factionEntity))
            {
                return false;
            }

            var buffer = entityManager.GetBuffer<FaithExposureElement>(factionEntity);
            exposure = FaithScoring.GetExposure(buffer, target);
            return true;
        }

        public bool TryDebugRecordFaithExposure(string factionId, CovenantId target, float amount)
        {
            var factionEntity = FindFactionEntityByFaith(factionId, out var entityManager);
            if (factionEntity == Entity.Null)
            {
                return false;
            }

            if (!entityManager.HasBuffer<FaithExposureElement>(factionEntity))
            {
                entityManager.AddBuffer<FaithExposureElement>(factionEntity);
            }

            var buffer = entityManager.GetBuffer<FaithExposureElement>(factionEntity);
            FaithScoring.RecordExposure(buffer, target, amount);
            return true;
        }

        public FaithScoring.CommitmentResult TryDebugCommitFaith(
            string factionId,
            CovenantId target,
            DoctrinePath path)
        {
            var factionEntity = FindFactionEntityByFaith(factionId, out var entityManager);
            if (factionEntity == Entity.Null ||
                !entityManager.HasComponent<FaithStateComponent>(factionEntity))
            {
                return FaithScoring.CommitmentResult.InvalidFaith;
            }

            if (!entityManager.HasBuffer<FaithExposureElement>(factionEntity))
            {
                entityManager.AddBuffer<FaithExposureElement>(factionEntity);
            }

            var faithState = entityManager.GetComponentData<FaithStateComponent>(factionEntity);
            var buffer = entityManager.GetBuffer<FaithExposureElement>(factionEntity);
            var result = FaithScoring.Commit(ref faithState, buffer, target, path);
            entityManager.SetComponentData(factionEntity, faithState);
            return result;
        }

        public bool TryDebugTriggerCovenantTest(string factionId)
        {
            var factionEntity = FindFactionEntityByFaith(factionId, out var entityManager);
            if (factionEntity == Entity.Null)
            {
                return false;
            }

            var requestEntity = entityManager.CreateEntity(typeof(PlayerCovenantTestRequestComponent));
            entityManager.SetComponentData(requestEntity, new PlayerCovenantTestRequestComponent
            {
                SourceFactionId = new FixedString32Bytes(factionId),
            });
            return true;
        }

        public bool TryDebugQueueCovenantTestDispatch(string factionId)
        {
            var factionEntity = FindFactionEntityByFaith(factionId, out var entityManager);
            if (factionEntity == Entity.Null)
            {
                return false;
            }

            if (!entityManager.HasComponent<PlayerCovenantTestDispatchStateComponent>(factionEntity))
            {
                entityManager.AddComponentData(
                    factionEntity,
                    PlayerCovenantTestDispatchStateComponent.CreateDefault(new FixedString32Bytes(factionId)));
            }

            var dispatchState = entityManager.GetComponentData<PlayerCovenantTestDispatchStateComponent>(factionEntity);
            dispatchState.RequestQueued = true;
            entityManager.SetComponentData(factionEntity, dispatchState);
            return true;
        }

        public bool TryDebugGetCovenantTestDispatchState(string factionId, out string readout)
        {
            readout = string.Empty;
            var factionEntity = FindFactionEntityByFaith(factionId, out var entityManager);
            if (factionEntity == Entity.Null ||
                !entityManager.HasComponent<PlayerCovenantTestDispatchStateComponent>(factionEntity))
            {
                return false;
            }

            var state = entityManager.GetComponentData<PlayerCovenantTestDispatchStateComponent>(factionEntity);
            readout =
                "CovenantTestDispatch|FactionId=" + factionId +
                "|FaithId=" + state.FaithId +
                "|DoctrinePath=" + state.DoctrinePath +
                "|Phase=" + state.TestPhase +
                "|ActionAvailable=" + state.ActionAvailable +
                "|CanAfford=" + state.CanAfford +
                "|RequestQueued=" + state.RequestQueued +
                "|RequestPending=" + state.RequestPending +
                "|FoodCost=" + state.FoodCost +
                "|InfluenceCost=" + state.InfluenceCost +
                "|PopulationCost=" + state.PopulationCost +
                "|LegitimacyCost=" + state.LegitimacyCost +
                "|ActionLabel=" + state.ActionLabel +
                "|ActionDetail=" + state.ActionDetail +
                "|BlockReason=" + state.BlockReason;
            return true;
        }

        public bool TryDebugGetCovenantTestState(string factionId, out string readout)
        {
            readout = string.Empty;
            var factionEntity = FindFactionEntityByFaith(factionId, out var entityManager);
            if (factionEntity == Entity.Null ||
                !entityManager.HasComponent<CovenantTestStateComponent>(factionEntity))
            {
                return false;
            }

            var state = entityManager.GetComponentData<CovenantTestStateComponent>(factionEntity);
            readout =
                "CovenantTestState|FactionId=" + factionId +
                "|Phase=" + state.TestPhase +
                "|IntensityThresholdMetAtInWorldDays=" + state.IntensityThresholdMetAtInWorldDays +
                "|TestStartedAtInWorldDays=" + state.TestStartedAtInWorldDays +
                "|LastFailedAtInWorldDays=" + state.LastFailedAtInWorldDays +
                "|SuccessCount=" + state.SuccessCount;
            return true;
        }

        public bool TryDebugSetFaithIntensity(string factionId, float value)
        {
            var factionEntity = FindFactionEntityByFaith(factionId, out var entityManager);
            if (factionEntity == Entity.Null ||
                !entityManager.HasComponent<FaithStateComponent>(factionEntity))
            {
                return false;
            }

            var faithState = entityManager.GetComponentData<FaithStateComponent>(factionEntity);
            faithState.Intensity = math.clamp(value, 0f, FaithIntensityTiers.IntensityMax);
            FaithScoring.SyncLevel(ref faithState);
            entityManager.SetComponentData(factionEntity, faithState);
            return true;
        }

        private static Entity FindFactionEntityByFaith(string factionId, out EntityManager entityManager)
        {
            entityManager = default;
            if (string.IsNullOrWhiteSpace(factionId))
            {
                return Entity.Null;
            }

            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated)
            {
                return Entity.Null;
            }

            entityManager = world.EntityManager;
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<FaithStateComponent>());
            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            for (int i = 0; i < entities.Length; i++)
            {
                if (factions[i].FactionId.ToString() == factionId)
                {
                    return entities[i];
                }
            }

            return Entity.Null;
        }
    }
}
