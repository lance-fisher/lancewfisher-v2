using Bloodlines.AI;
using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Debug
{
    public sealed partial class BloodlinesDebugCommandSurface
    {
        /// <summary>
        /// Returns the FaithStateComponent for the given faction. Useful for
        /// smoke validators checking that commitment occurred correctly.
        /// </summary>
        public bool TryDebugGetFaithState(
            string factionId,
            out CovenantId selectedFaith,
            out DoctrinePath doctrinePath,
            out float intensity,
            out int level)
        {
            selectedFaith = CovenantId.None;
            doctrinePath  = DoctrinePath.Unassigned;
            intensity     = 0f;
            level         = 0;

            var em = World.DefaultGameObjectInjectionWorld.EntityManager;
            var q  = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<FaithStateComponent>());

            if (q.IsEmpty) { q.Dispose(); return false; }

            var entities    = q.ToEntityArray(Allocator.Temp);
            var factions    = q.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            var faithStates = q.ToComponentDataArray<FaithStateComponent>(Allocator.Temp);
            q.Dispose();

            var targetId = new FixedString32Bytes(factionId);
            bool found   = false;

            for (int i = 0; i < entities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(targetId)) continue;
                selectedFaith = faithStates[i].SelectedFaith;
                doctrinePath  = faithStates[i].DoctrinePath;
                intensity     = faithStates[i].Intensity;
                level         = faithStates[i].Level;
                found = true;
                break;
            }

            entities.Dispose();
            factions.Dispose();
            faithStates.Dispose();
            return found;
        }

        /// <summary>Batch entry point used by BloodlinesFaithCommitmentSmokeValidation.</summary>
        public static void RunBatchFaithCommitmentSmokeValidation()
        {
            var surface = new BloodlinesDebugCommandSurface();
            surface.TryDebugGetFaithState("enemy", out _, out _, out _, out _);
        }
    }
}
