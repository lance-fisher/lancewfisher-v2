using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Canonical dynasty operation kinds that share the single active-cap
    /// budget defined in DynastyOperationLimits. The browser stores these
    /// as string `type` values on the operation object; Unity uses an
    /// enum so queries and gates can switch without string comparisons.
    ///
    /// The set mirrors the canonical dispatch sites in the browser
    /// simulation.js that call through getDynastyOperationsState /
    /// DYNASTY_OPERATION_ACTIVE_LIMIT:
    ///   - Missionary: startMissionaryOperation (~10523)
    ///   - HolyWar:    startHolyWarDeclaration  (~10565)
    ///   - DivineRight: declaration flow creates an operation slot in the
    ///                 same dynasty-operation active list (divine right
    ///                 attack/territory/raid/missionary/holyWar timer caps
    ///                 are the same dispatch surface at ~10820-10827)
    ///   - CaptiveRescue:  startCaptiveRescueOperation dispatch site
    ///                     (ai.js dynasty/covert ops dispatch ~2566-2608)
    ///   - CaptiveRansom:  startCaptiveRansomOperation dispatch site
    ///                     (ai.js dynasty/covert ops dispatch ~2566-2608)
    ///   - Assassination: startAssassinationOperation (~10912);
    ///                    ai.js dispatch at ~2435-2457 (source "enemy",
    ///                    target "player", target member selected via
    ///                    pickAiAssassinationTarget priority order).
    ///   - LesserHousePromotion: included for enum completeness so later
    ///                           slices can reuse the operation slot
    ///                           surface if they choose. The sub-slice 13
    ///                           promotion system currently runs without
    ///                           a dynasty-operation slot because the
    ///                           browser does not gate promotion through
    ///                           DYNASTY_OPERATION_ACTIVE_LIMIT; a future
    ///                           slice may promote promotion to a gated
    ///                           operation and this enum value is ready.
    ///
    /// None is the default value so newly default-constructed components
    /// do not match any specific kind by accident.
    /// </summary>
    public enum DynastyOperationKind : byte
    {
        None                 = 0,
        Missionary           = 1,
        HolyWar              = 2,
        DivineRight          = 3,
        CaptiveRescue        = 4,
        CaptiveRansom        = 5,
        LesserHousePromotion = 6,
        Assassination        = 7,
        Sabotage             = 8,
        CounterIntelligence  = 9,
        Espionage            = 10,
    }

    /// <summary>
    /// One entity per active dynasty operation. The browser stores the
    /// active operations as an array on each faction's dynasty state
    /// (`faction.dynasty.operations.active`) and gates new operation
    /// creation on `(operations.active ?? []).length >=
    /// DYNASTY_OPERATION_ACTIVE_LIMIT`. Unity collapses that per-faction
    /// array into per-faction-scoped entities: the gate counts active
    /// entities whose SourceFactionId matches the caller.
    ///
    /// The shape carries only the fields future slices need to find,
    /// gate on, and later resolve the operation. Per-kind fields
    /// (resolveAt, operatorId, successScore, etc.) are intentionally
    /// out of scope for sub-slice 18; later slices attach per-kind
    /// component structs when they port each dispatch surface.
    ///
    /// Active is carried explicitly (rather than relying on entity
    /// destruction) so a future resolution system can flip Active=false
    /// when an operation completes, keeping the entity around for audit
    /// and retrospective queries without consuming a slot against the
    /// cap. The HasCapacity helper skips Active=false entities.
    ///
    /// TargetFactionId and TargetMemberId are optional. When the
    /// operation kind does not target a faction or a member (e.g. a
    /// counter-intelligence operation that targets the source's own
    /// court), callers pass default FixedStrings. Consumers read the
    /// kind first and ignore target fields when they are not meaningful.
    ///
    /// Browser reference: simulation.js createEntityId("dynastyOperation")
    /// at the top of every start* function (~10537, 10579, 10849, 10888,
    /// 10924, 10963) plus the identical `operations.active.unshift(op)` +
    /// `operations.active = operations.active.slice(0, DYNASTY_OPERATION_ACTIVE_LIMIT)`
    /// block.
    /// </summary>
    public struct DynastyOperationComponent : IComponentData
    {
        public FixedString64Bytes OperationId;
        public FixedString32Bytes SourceFactionId;
        public DynastyOperationKind OperationKind;
        public float StartedAtInWorldDays;
        public FixedString32Bytes TargetFactionId;
        public FixedString64Bytes TargetMemberId;
        public bool Active;
    }
}
