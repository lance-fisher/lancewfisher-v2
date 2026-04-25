using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Per-kind component attached to a DynastyOperationComponent entity
    /// whose OperationKind == DynastyOperationKind.Sabotage.
    ///
    /// Browser reference: operation object created by startSabotageOperation
    /// (simulation.js:10952-10990):
    ///   type: "sabotage", subtype, sourceFactionId, targetFactionId,
    ///   targetBuildingId, targetBuildingName, startedAt, resolveAt,
    ///   spymasterId, successScore, projectedChance, escrowCost.
    ///
    /// Unity simplifications vs browser:
    ///   - intelligenceReportId / retaliationReason / retaliationInterceptType:
    ///     not stored; AI dispatch lane does not consume dossier-backed paths.
    ///   - intelligenceSupportBonus: always 0 for AI dispatch (no dossier); omitted.
    ///   - escrowCost: split into EscrowGold / EscrowInfluence (subtype-specific).
    ///   - TargetBuildingEntityIndex: int entity index used at resolution time to
    ///     find the building entity (mirrors TargetEntityIndex in PlayerCovertOps).
    /// </summary>
    public struct DynastyOperationSabotageComponent : IComponentData
    {
        public FixedString32Bytes TargetFactionId;
        public FixedString32Bytes Subtype;             // "gate_opening", "fire_raising", "supply_poisoning", "well_poisoning"
        public FixedString64Bytes TargetBuildingTypeId;
        public int                TargetBuildingEntityIndex;
        public FixedString64Bytes OperatorMemberId;
        public FixedString64Bytes OperatorTitle;
        public float              ResolveAtInWorldDays;
        public float              SuccessScore;
        public float              ProjectedChance;
        public float              EscrowGold;
        public float              EscrowInfluence;
    }
}
