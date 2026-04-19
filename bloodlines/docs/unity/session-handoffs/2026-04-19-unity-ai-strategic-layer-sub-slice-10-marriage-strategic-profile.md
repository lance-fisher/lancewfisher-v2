# Unity AI Strategic Layer Sub-Slice 10: Marriage Strategic Profile

**Date:** 2026-04-19
**Branch:** `claude/unity-ai-marriage-strategic-profile`
**Lane:** ai-strategic-layer
**Contract Revision:** 24

---

## Goal

Port the browser `getAiMarriageStrategicProfile` (ai.js ~2803-2839) and a
simplified `getMarriageFaithCompatibilityProfile` (simulation.js ~596-730)
to Unity DOTS/ECS. Sub-slices 6, 8, and 9 all reference the strategic
profile via `AICovertOpsComponent.MarriageProposalGateMet` and
`MarriageInboxAcceptGate` but never populate them, so the AI was dispatching
marriage ops unconditionally. This slice closes that gap by writing the
browser-accurate 4-signal willing decision into those two gates each frame.

With sub-slice 10 landed, the full AI marriage dispatch loop now gates on
browser-accurate strategic justification: hostility, population deficit,
faith-backed legitimacy signal, or succession pressure, modulated by
faith-compatibility tier.

---

## Work Completed

### New Files

- `unity/Assets/_Bloodlines/Code/AI/AIMarriageStrategicProfileSystem.cs`
  ISystem, `[UpdateInGroup(typeof(SimulationSystemGroup))]`,
  `[UpdateBefore(typeof(AICovertOpsSystem))]`. Per update:
  1. Resolves a "player" faction snapshot (PopulationComponent.Total,
     FaithStateComponent.SelectedFaith, DoctrinePath). If the player
     faction is missing, skips.
  2. Iterates every non-player faction with `FactionComponent`,
     `AICovertOpsComponent`, and `AIStrategyComponent`.
  3. For each AI faction, computes the 4 strategic signals from its own
     state plus the player snapshot:
     - `isHostile`: scan `HostilityComponent` buffer on AI entity for a
       "player" entry.
     - `populationDeficit`: `aiPop > 0 && aiPop < playerPop * 0.85`
       (matches browser `populationDeficit = enemyPop > 0 && enemyPop <
       playerPop * 0.85`).
     - `legitimacyDistress`: `legitimacy < 50` (matches browser
       `AI_MARRIAGE_LEGITIMACY_DISTRESS_THRESHOLD = 50`). Gated by
       `faithAllowsLegitimacySignal` to become `faithBackedLegitimacySignal`.
     - `successionPressure`: either `AIStrategyComponent.EnemySuccessionCrisisActive`
       (own crisis) or `PlayerSuccessionCrisisActive` (rival crisis).
  4. Computes simplified faith compatibility:
     - Either side unbound (SelectedFaith == None or DoctrinePath ==
       Unassigned) -> no legitimacy signal, does not block weak match.
     - Same covenant AND same doctrine -> harmonious, legitimacy signal
       allowed.
     - Same covenant XOR same doctrine -> sectarian/ecumenical, legitimacy
       signal allowed.
     - Neither matches -> incompatible, blocks weak match when signal count
       below 2.
  5. `signalCount = isHostile + populationDeficit + faithBackedLegitimacySignal + successionPressure`.
     `blockedByFaith = compat.BlocksWeakMatch && signalCount < 2`.
     `willing = signalCount > 0 && !blockedByFaith`.
  6. Writes `willing` to `AICovertOpsComponent.MarriageProposalGateMet`
     and `MarriageInboxAcceptGate` (both symmetric per browser comment at
     ai.js:2630-2631).

- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesAIMarriageStrategicProfileSmokeValidation.cs`
  Six-phase smoke validator. Each phase creates an isolated World with
  only `AIMarriageStrategicProfileSystem` in the sim group, seeds player
  faction with PopulationComponent + FaithStateComponent + DynastyStateComponent,
  and an AI faction with all relevant components + the signal state under
  test. After `world.Update()`, asserts the gate flags on
  `AICovertOpsComponent`.
  - Phase 1: No signals + unbound faith -> willing=false.
  - Phase 2: Hostility-only (isHostile=true) -> willing=true.
  - Phase 3: Population deficit only (50 < 85) -> willing=true.
  - Phase 4: Legitimacy 30 + harmonious faith (both OldLight+Light) ->
    faithBackedLegitimacySignal counts, willing=true.
  - Phase 5: EnemySuccessionCrisisActive=true -> willing=true.
  - Phase 6: Single hostile signal but incompatible faith (OldLight+Light vs
    BloodDominion+Dark) -> blockedByFaith fires since signalCount=1 < 2,
    willing=false.
  Artifact: `../artifacts/unity-ai-marriage-strategic-profile-smoke.log`
  Marker: `BLOODLINES_AI_MARRIAGE_STRATEGIC_PROFILE_SMOKE PASS`

- `scripts/Invoke-BloodlinesUnityAIMarriageStrategicProfileSmokeValidation.ps1`
  Standard PowerShell runner. Entry method:
  `Bloodlines.EditorTools.BloodlinesAIMarriageStrategicProfileSmokeValidation.RunBatchAIMarriageStrategicProfileSmokeValidation`.

### Shared-File Narrow Edits

- `unity/Assembly-CSharp.csproj` -- added `<Compile Include="..." />` entry
  for `AIMarriageStrategicProfileSystem.cs`.
- `unity/Assembly-CSharp-Editor.csproj` -- added `<Compile Include="..." />`
  entry for `BloodlinesAIMarriageStrategicProfileSmokeValidation.cs`.

### Cross-Lane Reads (no writes to another lane's code)

- Reads `PopulationComponent.Total` (economy-and-ai lane, retired) on both
  AI faction and player faction.
- Reads `FaithStateComponent.SelectedFaith` and `DoctrinePath`
  (faith-commitment lane, retired) on both sides.
- Reads `DynastyStateComponent.Legitimacy` (dynasty-core lane, retired) on
  AI faction.
- Reads `HostilityComponent` buffer (combat-and-projectile lane, retired)
  on AI faction.
- Reads `AIStrategyComponent.EnemySuccessionCrisisActive` and
  `PlayerSuccessionCrisisActive` (own lane).

No structural edits to any of those components or systems.

---

## Verification Results

All 10 validation gates green:

1. `dotnet build Assembly-CSharp.csproj -nologo` -- 0 errors PASS
2. `dotnet build Assembly-CSharp-Editor.csproj -nologo` -- 0 errors PASS
3. Bootstrap runtime smoke -- PASS
4. Combat smoke -- exit 0 PASS
5. Canonical scene shells (Bootstrap + Gameplay) -- both PASS
6. Fortification smoke -- PASS
7. Siege smoke -- exit 0 PASS
8. `node tests/data-validation.mjs` -- PASS
9. `node tests/runtime-bridge.mjs` -- PASS
10. Contract staleness check -- revision=23 current at gate time; bumped
    to 24 post-gate as part of handoff

AI marriage strategic profile smoke: all 6 phases PASS
- Phase 1 PASS: willing=False (no signals, unbound faith)
- Phase 2 PASS: willing=True (hostility-only)
- Phase 3 PASS: willing=True (population deficit only)
- Phase 4 PASS: willing=True (legitimacy distress + harmonious faith)
- Phase 5 PASS: willing=True (enemy succession crisis only)
- Phase 6 PASS: willing=False (single signal + incompatible faith blocks weak match)

---

## Key Design Notes

**Faith compatibility simplified.** The browser
`getMarriageFaithCompatibilityProfile` uses a three-axis tier system
(harmonious, sectarian, ecumenical, incompatible, unbound) derived from
covenantName grouping + doctrinePath strings. Unity's `FaithStateComponent`
only has `SelectedFaith` (CovenantId enum) and `DoctrinePath` (enum).
The port uses equality on both fields rather than covenantName string
grouping. This preserves the mechanical intent: identical covenant+doctrine
marriages are harmonious (legitimacy signal), either-field match is
non-blocking, fully-different covenant+doctrine blocks weak matches.

**Target hardcoded to "player".** Matches the browser convention where
`getAiMarriageStrategicProfile(state)` only reads `state.factions.enemy`
and `state.factions.player`. Multi-faction extension is reserved for a
later slice alongside the sub-slice 8/9 target extensions.

**Runs every frame.** The system recomputes from scratch each update
rather than tracking deltas. Population and legitimacy change across
ticks, and the system's work per tick is bounded (one faction-snapshot +
one per-AI-faction evaluation). Future optimization can throttle if
profiling shows it matters.

**Dispatch still gated by timer expiration.** `AICovertOpsSystem` only
reads `MarriageProposalGateMet` / `MarriageInboxAcceptGate` when
`MarriageProposalTimer` or `MarriageInboxTimer` has fired. So the
strategic profile filter adds gating but does not cause more frequent
dispatches.

**No evaluation stamping yet.** The browser `stampAiMarriageEvaluation`
writes an `aiEvaluation` object onto the proposal record so the UI can
show why the AI proposed or refused. This slice does not port that
bookkeeping; it can land as a follow-on enhancement if the match-review
UI needs it. The mechanical gate behavior (accept-or-reject) is complete
without it.

---

## Current Readiness

Branch `claude/unity-ai-marriage-strategic-profile` is ready to merge.
All gates green, contract at revision 24, continuity files updated.

---

## Next Action

1. Merge `claude/unity-ai-marriage-strategic-profile` into master.
2. Claim sub-slice 11 (marriage accept effects) on a new branch
   `claude/unity-ai-marriage-accept-effects`. Port the browser-side
   effects on accept that sub-slice 9 deferred: +2 legitimacy both sides,
   hostility drop both ways, oathkeeping conviction +2 both sides, 30-day
   `DeclareInWorldTimeRequest` jump.
3. Codex branches are landed (command-dispatch rev 22, fortification-siege
   imminent-engagement rev 23). Next Codex slice is unscoped; lane
   registry still lists fortification-siege-imminent-engagement as the
   active Codex lane with sub-slice 3 landed and sub-slice 4 open.
