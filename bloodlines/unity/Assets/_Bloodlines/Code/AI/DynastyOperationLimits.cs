using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Per-faction cap on simultaneously active dynasty operations plus
    /// the single canonical producer helper. Any system that starts a
    /// dynasty operation routes through BeginOperation so the active
    /// entity population is accounted under a single policy.
    ///
    /// Browser reference: simulation.js:17
    ///   const DYNASTY_OPERATION_ACTIVE_LIMIT = 6;
    /// plus the seven dispatch sites (missionary, holy war, counter-
    /// intelligence, espionage, assassination, sabotage, captive rescue/
    /// ransom) that all call
    ///   if ((operations.active ?? []).length >= DYNASTY_OPERATION_ACTIVE_LIMIT) return;
    /// before appending to `operations.active`.
    ///
    /// Unity departs from the browser array-slice-to-cap trim
    /// (`operations.active.slice(0, DYNASTY_OPERATION_ACTIVE_LIMIT)`)
    /// because an entity-per-operation model cannot silently drop entries
    /// without orphaning downstream state. Instead the gate is strict:
    /// callers must check HasCapacity before BeginOperation, and
    /// BeginOperation creates an entity unconditionally. Callers that
    /// skip the check may exceed the cap; that is intentionally kept as
    /// a caller contract so over-cap creation shows up as a bug rather
    /// than being silently trimmed.
    ///
    /// The helper is a static class with struct-style helpers so it can
    /// be called from system OnUpdate paths without needing an ECB or
    /// system instance.
    /// </summary>
    public static class DynastyOperationLimits
    {
        /// <summary>
        /// Browser-canonical per-faction cap on simultaneously active
        /// dynasty operations. simulation.js:17.
        /// </summary>
        public const int DYNASTY_OPERATION_ACTIVE_LIMIT = 6;

        /// <summary>
        /// Returns true when the faction may start at least one more
        /// dynasty operation: the count of DynastyOperationComponent
        /// entities with matching SourceFactionId and Active=true is
        /// strictly less than DYNASTY_OPERATION_ACTIVE_LIMIT.
        /// Inactive entities (Active=false) do not consume capacity and
        /// are skipped.
        ///
        /// The query scans all DynastyOperationComponent entities. Per-
        /// faction indexing is intentionally deferred until profiling
        /// shows it matters; a typical match holds a small number of
        /// operations (cap 6 per faction, with a small number of
        /// factions).
        /// </summary>
        public static bool HasCapacity(EntityManager em, FixedString32Bytes factionId)
        {
            return CountActiveForFaction(em, factionId) < DYNASTY_OPERATION_ACTIVE_LIMIT;
        }

        /// <summary>
        /// Returns the count of active DynastyOperationComponent entities
        /// whose SourceFactionId matches the given faction. Inactive
        /// entities are skipped.
        /// </summary>
        public static int CountActiveForFaction(EntityManager em, FixedString32Bytes factionId)
        {
            int count = 0;
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<DynastyOperationComponent>());
            using (var arr = q.ToComponentDataArray<DynastyOperationComponent>(Allocator.Temp))
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    var op = arr[i];
                    if (!op.Active) continue;
                    if (!op.SourceFactionId.Equals(factionId)) continue;
                    count++;
                }
            }
            q.Dispose();
            return count;
        }

        /// <summary>
        /// Creates a single DynastyOperationComponent entity with
        /// Active=true and StartedAtInWorldDays stamped from the
        /// DualClock singleton (0 if no clock is seeded). Callers are
        /// responsible for having already checked HasCapacity; the
        /// helper does not enforce the cap because strict enforcement
        /// belongs at the call site where the faction's other dispatch
        /// gates run (resource cost, terms availability, etc.).
        ///
        /// The created entity carries the target-faction and target-
        /// member fields so consumers can query the full operation
        /// shape without a second lookup. Callers that do not target
        /// a faction or a member pass default FixedStrings.
        ///
        /// Returns the created entity so the caller can attach per-kind
        /// component data (per-operation resolveAt, operator metadata,
        /// success scoring) in the same tick.
        /// </summary>
        public static Entity BeginOperation(
            EntityManager em,
            FixedString64Bytes operationId,
            FixedString32Bytes sourceFactionId,
            DynastyOperationKind kind,
            FixedString32Bytes targetFactionId = default,
            FixedString64Bytes targetMemberId = default)
        {
            var e = em.CreateEntity(typeof(DynastyOperationComponent));
            em.SetComponentData(e, new DynastyOperationComponent
            {
                OperationId          = operationId,
                SourceFactionId      = sourceFactionId,
                OperationKind        = kind,
                StartedAtInWorldDays = GetInWorldDays(em),
                TargetFactionId      = targetFactionId,
                TargetMemberId       = targetMemberId,
                Active               = true,
            });
            return e;
        }

        // ------------------------------------------------------------------ helpers

        private static float GetInWorldDays(EntityManager em)
        {
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<DualClockComponent>());
            if (q.IsEmpty) { q.Dispose(); return 0f; }
            float d = q.GetSingleton<DualClockComponent>().InWorldDays;
            q.Dispose();
            return d;
        }
    }
}
