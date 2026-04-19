# 2026-04-18 Unity Fortification Siege Imminent Engagement Warnings

## Scope

Prompt-accurate Unity ECS port of `tickImminentEngagementWarnings` from
`src/game/core/simulation.js:11742-11873` on
`codex/unity-fortification-siege`.

This handoff supersedes the earlier first-pass note at
`docs/unity/session-handoffs/2026-04-18-unity-fortification-siege-sub3-imminent-engagement.md`.
That earlier pass widened into out-of-scope behavior. This slice brings the
implementation back to the requested scope and file layout.

## Delivered Files

- `unity/Assets/_Bloodlines/Code/Fortification/ImminentEngagementCanon.cs`
  - canonical posture profiles for `brace`, `steady`, and `counterstroke`
  - `GetPosture(string id)` defaults to `steady`
- `unity/Assets/_Bloodlines/Code/Components/ImminentEngagementComponent.cs`
  - prompt-accurate per-settlement warning-window state in
    `Bloodlines.Components`
  - old fortification-path component removed so the runtime has one canonical
    imminent-engagement component type
- `unity/Assets/_Bloodlines/Code/Fortification/ImminentEngagementWarningSystem.cs`
  - simplified per-settlement warning-window driver
  - profile activation uses `FortificationReserveComponent.ThreatActive`
  - warning seconds formula uses keep/settlement base plus watchtower and tier
    bonuses, clamped to `FortificationCanon` min and max
  - AI posture selection matches the prompt:
    `brace` for bloodline risk or reserve inferiority,
    `counterstroke` only when a commander is present and the hostile count is
    manageable, otherwise `steady`
  - expiry closes the window and marks `WindowConsumed`
  - threat dissipation reset preserves `SelectedResponseId`,
    `CommanderRecallIssuedAt`, and `BloodlineProtectionUntil`
  - `autoSortieOnExpiry` intentionally deferred with
    `// TODO: autoSortieOnExpiry -- deferred to sortie system lane.`
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesImminentEngagementSmokeValidation.cs`
  - static batch-entry smoke validator following the established project
    pattern
  - isolated ECS worlds for all four phases
  - writes `BLOODLINES_IMMINENT_ENGAGEMENT_SMOKE PASS|FAIL` artifact marker
- `scripts/Invoke-BloodlinesUnityImminentEngagementSmokeValidation.ps1`
  - self-contained wrapper pointing to
    `Bloodlines.EditorTools.BloodlinesImminentEngagementSmokeValidation.RunBatchBloodlinesImminentEngagementSmokeValidation`

## Browser References Ported

- `src/game/core/simulation.js:271-308`
  - `IMMINENT_ENGAGEMENT_POSTURES`
- `src/game/core/simulation.js:11448-11481`
  - `createDefaultImminentEngagementState`
- `src/game/core/simulation.js:11491-11493`
  - `getImminentEngagementPosture`
- `src/game/core/simulation.js:11506-11622`
  - `getSettlementImminentEngagementProfile`
- `src/game/core/simulation.js:11720-11728`
  - `chooseAiImminentEngagementResponse`
- `src/game/core/simulation.js:11730-11739`
  - `resetSettlementImminentEngagement`
- `src/game/core/simulation.js:11742-11873`
  - `tickImminentEngagementWarnings`

## Out Of Scope Kept Deferred

- `issueImminentEngagementCommanderRecall`
- `commitImminentEngagementReinforcements`
- reserve-heal and reserve-muster posture multipliers as FortificationReserve
  consumers
- HUD or player-facing notification strings
- `autoSortieOnExpiry` behavior beyond the TODO seam

## Validation

The required serial gate chain is green on `D:\BLFS\bloodlines`:

1. `dotnet build unity/Assembly-CSharp.csproj -nologo`
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
3. bootstrap runtime smoke
4. combat smoke
5. canonical scene-shell validation
6. fortification smoke
7. siege smoke
8. imminent engagement smoke
9. `node tests/data-validation.mjs`
10. `node tests/runtime-bridge.mjs`
11. `scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1`

Notable validation results:

- imminent smoke phase 1: tier-0 settlement stayed inactive
- imminent smoke phase 2: no-threat settlement stayed inactive
- imminent smoke phase 3: threat activation selected `brace` when
  `HostileCount=3` and `ReadyReserveCount=1`
- imminent smoke phase 4: expired active window closed and marked
  `WindowConsumed=True`

Because the checked-in bootstrap-runtime and scene-shell wrappers are still
path-pinned to `D:\ProjectsHome\Bloodlines`, those two validations were run
against `D:\BLFS\bloodlines` via inline path overrides during this slice. No
shared wrapper files were committed for that temporary adaptation.

## Branch State

- Branch: `codex/unity-fortification-siege`
- Contract revision: `15`
- Branch status: ready for merge coordination after prompt-accurate imminent
  engagement warning delivery

## Next Action

1. Merge `codex/unity-fortification-siege` to `master`.
2. After merge, retire or refresh the
   `fortification-siege-imminent-engagement` lane in the concurrent-session
   contract and continuity state.
3. Do not widen this branch into bootstrap integration, siege lifecycle,
   sortie execution, or unrelated AI work before merge coordination completes.
