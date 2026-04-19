using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// One entity per active non-aggression pact between two factions.
    /// Created by AIPactProposalExecutionSystem when an AI faction's
    /// CovertOpsComponent.LastFiredOp == CovertOpKind.PactProposal fires
    /// and the browser-side gates pass.
    ///
    /// Pacts are symmetric: a single PactComponent represents the bond
    /// between FactionAId and FactionBId. Queries that need to find a
    /// faction's pacts scan all PactComponent entities and match either
    /// FactionAId or FactionBId. This is a deliberate departure from the
    /// marriage primary+mirror pattern (sub-slice 9): marriages need a
    /// primary record because MarriageGestationSystem must spawn exactly
    /// one child per marriage; pacts have no analogous asymmetric
    /// downstream system.
    ///
    /// Browser reference: simulation.js proposeNonAggressionPact (~5185)
    /// creates two faction.diplomacy.nonAggressionPacts records (one per
    /// side) with mirrored fields. Unity collapses that into a single
    /// entity because both sides have identical fields.
    ///
    /// Constants (browser):
    ///   NON_AGGRESSION_PACT_INFLUENCE_COST = 50
    ///   NON_AGGRESSION_PACT_GOLD_COST = 80
    ///   NON_AGGRESSION_PACT_MINIMUM_DURATION_IN_WORLD_DAYS = 180
    /// </summary>
    public struct PactComponent : IComponentData
    {
        public FixedString64Bytes PactId;
        public FixedString32Bytes FactionAId;
        public FixedString32Bytes FactionBId;
        public float StartedAtInWorldDays;
        public float MinimumExpiresAtInWorldDays;
        public bool  Broken;
        public FixedString32Bytes BrokenByFactionId;
    }
}
