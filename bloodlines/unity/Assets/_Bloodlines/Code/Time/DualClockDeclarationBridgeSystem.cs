using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Systems
{
    /// <summary>
    /// Observes control point ownership changes each frame and fires
    /// DeclareInWorldTimeRequest events when a capture completes, advancing
    /// the in-world clock by a canonical event delta.
    ///
    /// Browser equivalent sites:
    ///   declareInWorldTime(state, 14, `Captured ${controlPoint.name}`)   -- simulation.js:8155
    ///   declareInWorldTime(state, 28, `Cross-continental capture: ...`)  -- simulation.js:8159
    ///   declareInWorldTime(state, 30, `Marriage of ...`)                 -- simulation.js:7459
    ///
    /// Sub-slice 3 ports the capture event site only (14 in-world days per capture).
    /// Marriage and holy war declaration sites are wired in later dynasty/faith slices
    /// once those systems generate their own event signals.
    ///
    /// Runs after ControlPointCaptureSystem so this frame's ownership changes are visible.
    /// Runs before DualClockDeclarationSystem so declarations are processed this frame.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(ControlPointCaptureSystem))]
    [UpdateBefore(typeof(DualClockDeclarationSystem))]
    public partial struct DualClockDeclarationBridgeSystem : ISystem
    {
        // Tracks the owner of each CP from the previous frame to detect ownership transitions.
        private NativeHashMap<FixedString32Bytes, FixedString32Bytes> _previousCpOwners;

        private const float CaptureDaysDelta = 14f;

        public void OnCreate(ref SystemState state)
        {
            _previousCpOwners = new NativeHashMap<FixedString32Bytes, FixedString32Bytes>(32, Allocator.Persistent);
            state.RequireForUpdate<DualClockComponent>();
        }

        public void OnDestroy(ref SystemState state)
        {
            if (_previousCpOwners.IsCreated) _previousCpOwners.Dispose();
        }

        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.HasSingleton<DualClockComponent>()) return;

            var em = state.EntityManager;
            var clockEntity = SystemAPI.GetSingletonEntity<DualClockComponent>();

            if (!em.HasBuffer<DeclareInWorldTimeRequest>(clockEntity)) return;

            var cpQuery = em.CreateEntityQuery(ComponentType.ReadOnly<ControlPointComponent>());
            var cpData = cpQuery.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);
            cpQuery.Dispose();

            var buf = em.GetBuffer<DeclareInWorldTimeRequest>(clockEntity);

            for (int i = 0; i < cpData.Length; i++)
            {
                var cp = cpData[i];
                var cpId = cp.ControlPointId;
                var currentOwner = cp.OwnerFactionId;

                if (_previousCpOwners.TryGetValue(cpId, out var prevOwner))
                {
                    // Ownership changed and new owner is a real faction (not empty on game start).
                    if (prevOwner.Length > 0 && currentOwner != prevOwner)
                    {
                        buf.Add(new DeclareInWorldTimeRequest
                        {
                            DaysDelta = CaptureDaysDelta,
                            Reason = new FixedString64Bytes("CP captured"),
                        });
                    }
                }

                _previousCpOwners[cpId] = currentOwner;
            }

            cpData.Dispose();
        }
    }
}
