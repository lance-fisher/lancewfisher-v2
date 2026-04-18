using Bloodlines.SaveLoad;
using Unity.Entities;

namespace Bloodlines.Debug
{
    /// <summary>
    /// Debug command surface extension for the save/load snapshot system.
    /// Gives the governed smoke validators a controlled way to capture and
    /// inspect snapshot payloads.
    ///
    /// Browser reference: simulation.js:13822 exportStateSnapshot,
    /// simulation.js:13989 restoreStateSnapshot.
    /// </summary>
    public sealed partial class BloodlinesDebugCommandSurface
    {
        public bool TryDebugCaptureSnapshot(out string json)
        {
            json = null;
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated)
            {
                return false;
            }

            BloodlinesSnapshotWriter.Capture(world.EntityManager, out var payload);
            json = BloodlinesSnapshotWriter.Serialize(payload);
            return json != null;
        }

        public bool TryDebugGetSnapshotPayload(out BloodlinesSnapshotPayload payload)
        {
            payload = null;
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated)
            {
                return false;
            }

            BloodlinesSnapshotWriter.Capture(world.EntityManager, out payload);
            return payload != null;
        }
    }
}
