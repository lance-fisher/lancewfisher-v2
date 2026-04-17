# 2026-04-17 Unity Dynasty Core (Tier 1 Migration Slice)

## Goal

Second Tier 1 slice from the 2026-04-17 browser-to-Unity migration plan. Ports
the canonical eight-member dynasty template set and the core lifecycle fields
(roles, paths, age, status, renown, fallen ledger) plus succession behavior
from the browser runtime into ECS. Every non-tribe faction now spawns a live
dynasty that ages, supports promotion of heirs on ruler death, and enters
interregnum when the bloodline is fully extinguished.

## Browser Reference

- `src/game/core/simulation.js:4262` — `createDynastyState(content, factionSetup, factionId)`:
  authoritative template list. Eight members with canonical roles, paths,
  titles, ages, statuses, and starting renown.
- `src/game/core/simulation.js:4278` — dynasty state shape: activeMemberCap,
  dormantReserve, legitimacy, loyaltyPressure, interregnum, attachments
  (including fallenMembers ledger).

## Canon Reference

- `04_SYSTEMS/DYNASTIC_SYSTEM.md` — canonical dynasty design, role chain,
  succession semantics.

## Work Completed

### Components (new + extended)
- `unity/Assets/_Bloodlines/Code/Components/DynastyMemberComponent.cs`:
  - `DynastyRole` enum (HeadOfBloodline, HeirDesignate, Commander, Governor,
    Diplomat, IdeologicalLeader, Merchant, Spymaster) — canonical order
    matters for succession.
  - `DynastyPath` enum (Governance, MilitaryCommand, Diplomacy,
    ReligiousLeadership, EconomicStewardshipTrade, CovertOperations).
  - `DynastyMemberStatus` enum (Active, Ruling, Dormant, Fallen, Captured).
  - `DynastyMemberComponent` — per-member IComponentData: MemberId, Title,
    Role, Path, AgeYears, Status, Renown, Order, FallenAtWorldSeconds.
  - `DynastyStateComponent` — per-faction: ActiveMemberCap, DormantReserve,
    Legitimacy, LoyaltyPressure, Interregnum.
  - `DynastyMemberRef` IBufferElementData — faction-to-member reference buffer.
  - `DynastyFallenLedger` IBufferElementData — ordered fallen-member ledger.

### Dynasty logic (new folder)
- `unity/Assets/_Bloodlines/Code/Dynasties/DynastyTemplates.cs`: static
  eight-member template array mirroring `createDynastyState` line-for-line,
  plus `InitialActiveMemberCap` (20) and `InitialLegitimacy` (58) constants.
- `unity/Assets/_Bloodlines/Code/Dynasties/DynastyBootstrap.cs`:
  `AttachDynasty(EntityManager, Entity, FixedString32Bytes)` shared spawning
  helper. Materializes all eight member entities, writes the member-ref
  buffer, writes the empty fallen-ledger buffer, and commits the initial
  dynasty state. Structurally safe (caches member entities before retrieving
  buffers).
- `unity/Assets/_Bloodlines/Code/Dynasties/DynastyAgingSystem.cs`:
  `[BurstCompile]` `ISystem` in SimulationSystemGroup. Advances age on
  Ruling, Active, and Captured members; Fallen and Dormant are held. Uses a
  placeholder rate of one in-world year per sixty real seconds; the
  authoritative rate will replace this when the dual-clock slice lands.
- `unity/Assets/_Bloodlines/Code/Dynasties/DynastySuccessionSystem.cs`:
  `ISystem` runs after aging. If no member carries Ruling status but an
  active heir exists, promotes the lowest-Order active member to Ruling
  with HeadOfBloodline role. If no active member remains, marks the faction
  Interregnum=true.

### Debug command surface (new partial)
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Dynasty.cs`:
  - `TryDebugGetDynastyMemberCount(factionId)`
  - `TryDebugGetDynastyState(factionId, out DynastyStateComponent)`
  - `TryDebugGetDynastyMember(factionId, DynastyRole, out DynastyMemberComponent)` —
    prefers living members (Ruling, Active, Captured) over Fallen so callers
    get the current holder of a role, not a historical one.
  - `TryDebugFellDynastyMember(factionId, DynastyRole)` — marks the target
    member Fallen, stamps the fall time, appends to the fallen ledger.

### Governed smoke validator (new editor)
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesDynastySmokeValidation.cs`:
  editor-only `[MenuItem]` + `RunBatchDynastySmokeValidation` entry. Four
  phases in isolated ECS worlds:
  1. **Spawn phase** — attaches dynasty to a faction, asserts 8 canonical
     members exist, asserts the head is Ruling at age 38 on the Governance
     path, asserts Legitimacy=58 and ActiveMemberCap=20.
  2. **Aging phase** — ticks 120 seconds, asserts the ruling head's age
     advanced (2 in-world years at the placeholder rate).
  3. **Succession phase** — fells the ruler, ticks, asserts the eldest heir
     is now Ruling at HeadOfBloodline role, asserts dynasty is not in
     Interregnum.
  4. **Interregnum phase** — fells the current ruler repeatedly (succession
     keeps promoting the next heir to HeadOfBloodline), asserts all eight
     members fall and the dynasty enters Interregnum=true.
  Writes `artifacts/unity-dynasty-smoke.log`.

### Wrapper (new)
- `scripts/Invoke-BloodlinesUnityDynastySmokeValidation.ps1`:
  batch-mode wrapper calling
  `Bloodlines.EditorTools.BloodlinesDynastySmokeValidation.RunBatchDynastySmokeValidation`.

## Scope Discipline

- No changes to the Bootstrap scene or SkirmishBootstrapSystem yet.
  `DynastyBootstrap.AttachDynasty` is callable by any spawn path but has
  not been wired into the live skirmish yet; that wiring is a follow-up
  slice so this one stays narrow and reviewable.
- No marriages, lesser houses, captives, intelligence operations, or
  political events. Those are separate Tier 2 slices per the migration
  plan.
- Aging rate is a placeholder until the dual-clock slice; documented in
  the aging system's docstring.
- No downstream consumer reads dynasty state yet (no legitimacy applied
  to loyalty, no renown applied to combat). Consumer wiring is the next
  slice.

## Verification

- `Invoke-BloodlinesUnityDynastySmokeValidation.ps1` — passed:
  `Dynasty smoke validation passed: spawnPhase=True, agingPhase=True, successionPhase=True, interregnumPhase=True. Spawn: memberCount=8, headAge=38, legitimacy=58. Aging: initialAge=38, finalAge=40, delta=2. Succession: newRulerTitle=Eldest Heir, age=19. Interregnum: felledThroughChain=8, interregnum=True.`
- Additional governance gates (bootstrap, combat, graphics, scene shells,
  data-validation, runtime-bridge, staleness) to run at the merge boundary.

## Next Action (Within Migration Plan)

After this slice merges:

1. Wire `DynastyBootstrap.AttachDynasty` into `SkirmishBootstrapSystem` so
   the live skirmish carries a live dynasty. Extend the bootstrap runtime
   smoke to assert 8 members per playable faction.
2. Begin the faith commitment slice (Tier 1 item 3): `chooseFaithCommitment`,
   `updateFaithExposure`, `updateFaithStructureIntensity`. The dynasty head
   is the member on whom faith commitment is canonically anchored, so
   porting dynasty first unblocks this correctly.
3. Begin the state snapshot / restore slice (Tier 1 item 5). Dynasty and
   conviction state both need to round-trip.

## Branch

- Branch: `claude/unity-dynasty-core`
- Base: master tip after Conviction slice merge.
