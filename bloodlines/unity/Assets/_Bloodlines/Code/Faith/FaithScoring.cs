using Bloodlines.Components;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Faith
{
    /// <summary>
    /// Pure, deterministic helpers that mutate <see cref="FaithStateComponent"/>
    /// and its exposure buffer the same way the browser runtime does. No ECS
    /// system-level code here so the same helpers are reusable from debug
    /// surfaces, future AI decision paths, and the smoke validator.
    ///
    /// Browser reference: simulation.js updateFaithExposure (8174),
    /// chooseFaithCommitment (9694), syncFaithIntensityState (1907),
    /// updateFaithStructureIntensity (8226).
    /// </summary>
    public static class FaithScoring
    {
        public enum CommitmentResult : byte
        {
            Committed = 0,
            AlreadyCommitted = 1,
            ExposureBelowThreshold = 2,
            InvalidFaith = 3,
        }

        public static void SyncLevel(ref FaithStateComponent faith)
        {
            faith.Intensity = math.clamp(faith.Intensity, 0f, FaithIntensityTiers.IntensityMax);
            faith.Level = FaithIntensityTiers.ResolveLevel(faith.Intensity);
        }

        public static void RecordExposure(
            DynamicBuffer<FaithExposureElement> exposureBuffer,
            CovenantId faith,
            float amount)
        {
            if (faith == CovenantId.None || amount == 0f)
            {
                return;
            }

            for (int i = 0; i < exposureBuffer.Length; i++)
            {
                var element = exposureBuffer[i];
                if (element.Faith == faith)
                {
                    element.Exposure = math.clamp(
                        element.Exposure + amount,
                        0f,
                        FaithIntensityTiers.IntensityMax);
                    element.Discovered = element.Exposure > 0f || element.Discovered;
                    exposureBuffer[i] = element;
                    return;
                }
            }

            exposureBuffer.Add(new FaithExposureElement
            {
                Faith = faith,
                Exposure = math.clamp(amount, 0f, FaithIntensityTiers.IntensityMax),
                Discovered = amount > 0f,
            });
        }

        public static float GetExposure(
            DynamicBuffer<FaithExposureElement> exposureBuffer,
            CovenantId faith)
        {
            for (int i = 0; i < exposureBuffer.Length; i++)
            {
                if (exposureBuffer[i].Faith == faith)
                {
                    return exposureBuffer[i].Exposure;
                }
            }
            return 0f;
        }

        public static CommitmentResult Commit(
            ref FaithStateComponent faith,
            DynamicBuffer<FaithExposureElement> exposureBuffer,
            CovenantId target,
            DoctrinePath path)
        {
            if (target == CovenantId.None || path == DoctrinePath.Unassigned)
            {
                return CommitmentResult.InvalidFaith;
            }

            if (faith.SelectedFaith != CovenantId.None)
            {
                return CommitmentResult.AlreadyCommitted;
            }

            if (GetExposure(exposureBuffer, target) < FaithIntensityTiers.CommitmentExposureThreshold)
            {
                return CommitmentResult.ExposureBelowThreshold;
            }

            faith.SelectedFaith = target;
            faith.DoctrinePath = path;
            faith.Intensity = FaithIntensityTiers.StartingCommitmentIntensity;
            SyncLevel(ref faith);
            return CommitmentResult.Committed;
        }
    }
}
