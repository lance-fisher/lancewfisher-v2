using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Request marker to break an active non-aggression pact. Attached to
    /// a short-lived entity by callers that want to dissolve a pact (test
    /// harnesses, future player-UI break action, or future AI policy
    /// systems). PactBreakSystem consumes the request and destroys the
    /// entity after applying the break.
    ///
    /// The browser has no auto-break path: only explicit calls to
    /// breakNonAggressionPact (simulation.js ~5224) dissolve a pact. This
    /// request-component pattern mirrors that by requiring an explicit
    /// producer rather than polling a timer.
    ///
    /// Fields:
    ///   - PactId: identifies the PactComponent to break.
    ///   - RequestingFactionId: the faction initiating the break. This is
    ///     the faction that pays the legitimacy + oathkeeping penalty.
    ///
    /// Pairing: PactComponent entity matched by PactId. PactComponent.Broken
    /// is set, PactComponent.BrokenByFactionId is set to the requesting
    /// faction.
    /// </summary>
    public struct PactBreakRequestComponent : IComponentData
    {
        public FixedString64Bytes PactId;
        public FixedString32Bytes RequestingFactionId;
    }
}
