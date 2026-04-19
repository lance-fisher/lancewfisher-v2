# 2026-04-19 Unity Fortification Siege Breach Legibility Readout

## Scope

Prompt-accurate continuation of the Codex-owned fortification-siege lane on
`codex/unity-fortification-breach-legibility-readout`.

This slice starts from `origin/master` `374bb2e4` after Claude's
ai-strategic-layer sub-slice 18 dynasty-operations foundation landed and moved
the concurrent-session contract to revision `35`. The contract target for this
slice is revision `36`.

Sub-slice 5 resolved wall, gate, tower, keep, and `OpenBreachCount` state onto
`FortificationComponent`, and sub-slice 6 made breaches alter live assault
outcomes through `FieldWaterComponent` telemetry. Sub-slice 7 does not ship a
HUD. It ships the legibility seam a future HUD needs: one settlement-level
debug readout that packages breach counts, tier/frontage state, and aggregate
breach-assault telemetry without requiring a consumer to scan per-component
state directly.

## Delivered Files

- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Fortification.BreachReadout.cs`
  - new `SettlementBreachReadout` plain-data struct under debug-surface
    ownership
  - new `TryDebugGetSettlementBreachReadout(FixedString32Bytes settlementId,
    out SettlementBreachReadout readout)` seam on
    `BloodlinesDebugCommandSurface`
  - packages settlement id, owner faction id, open breach count, destroyed
    wall/tower/gate/keep counts, current tier, derived reserve frontage, and
    aggregate breach-assault state
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBreachLegibilityReadoutSmokeValidation.cs`
  - dedicated 5-phase validator covering intact baseline, single-breach
    pressure, three-breach capped scaling, missing-settlement false/default
    behavior, and mixed partial destruction with tier contraction
  - artifact marker:
    `BLOODLINES_BREACH_LEGIBILITY_READOUT_SMOKE PASS|FAIL`
- `scripts/Invoke-BloodlinesUnityBreachLegibilityReadoutSmokeValidation.ps1`
  - dedicated batch wrapper for the new validator

## Readout Design Notes

- `SettlementBreachReadout` keeps settlement-side fortification counts and
  breach-assault telemetry in one debug-owned surface so a future HUD can read
  one packaged struct per settlement instead of reaching into
  `FortificationComponent` and `FieldWaterComponent`.
- Reserve frontage is derived from the already-landed fortification reserve
  rule instead of becoming a second stored state value. The seam resolves the
  current settlement tier, counts linked live defenders inside reserve radius,
  and reduces frontage through the same tier-scaled rule the reserve system
  already uses.
- Aggregate breach-assault state is read from live `FieldWaterComponent`
  entries that currently target the settlement. This keeps the seam aligned
  with the runtime consumer from sub-slice 6 rather than recomputing attack and
  speed multipliers independently.
- No production HUD renderer ships in this slice. This is a foundation-only
  seam, matching the pattern Claude used in ai-strategic-layer sub-slice 18
  dynasty-operations foundation.

## Browser And Canon References

- `docs/unity/session-handoffs/2026-04-19-unity-fortification-siege-wall-segment-destruction-resolution.md`
  - fortification destruction counts and reserve-frontage contraction seam from
    sub-slice 5
- `docs/unity/session-handoffs/2026-04-19-unity-fortification-siege-breach-assault-pressure.md`
  - breach-assault telemetry surface from sub-slice 6
- `src/game/core/simulation.js:11227-11245`
  - fortification-tier advancement reference consumed indirectly through the
    current Unity fortification tier
- `src/game/core/simulation.js:11962-11974`
  - reserve frontage / desired-frontline frame mirrored by the current Unity
    reserve system

## Validation

The slice is green on `D:\BLBLR\bloodlines`:

1. `dotnet build unity/Assembly-CSharp.csproj -nologo`
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
3. bootstrap runtime smoke
4. combat smoke
5. canonical scene shells: Bootstrap + Gameplay
6. fortification smoke
7. siege smoke
8. `node tests/data-validation.mjs`
9. `node tests/runtime-bridge.mjs`
10. `scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1`
11. dedicated breach-legibility readout smoke

Dedicated breach-legibility readout smoke phases:

- Phase 1 PASS: intact fortified settlement returns zero breach counts, tier 1,
  reserve frontage 2, and baseline multipliers
- Phase 2 PASS: a single breach returns `OpenBreachCount = 1`,
  `BreachAssaultAdvantageActive = true`, `1.08x` aggregate attack, and `1.04x`
  aggregate speed
- Phase 3 PASS: three open breaches return capped aggregate multipliers
  `1.24x` attack and `1.12x` speed
- Phase 4 PASS: missing settlement id returns `false` and a default readout
- Phase 5 PASS: mixed wall/tower/gate destruction reports
  `walls=2, towers=1, gates=1, keeps=0`, `OpenBreachCount = 3`, tier 2, and
  reserve frontage 3

Validation artifacts:

- `artifacts/unity-bootstrap-runtime-smoke.log`
- `artifacts/unity-combat-smoke.log`
- `artifacts/unity-bootstrap-scene-validate.log`
- `artifacts/unity-gameplay-scene-validate.log`
- `artifacts/unity-fortification-smoke.log`
- `artifacts/unity-siege-smoke.log`
- `artifacts/unity-breach-legibility-readout-smoke.log`

Local csproj refresh note:

- `unity/Assembly-CSharp.csproj` and `unity/Assembly-CSharp-Editor.csproj` were
  regenerated locally in this worktree because the new `D:\BLBLR` checkout
  started without Unity's auto-generated project files. Both regenerated files
  now include the new breach-readout and validator entries. They remain
  gitignored and are not part of the commit.

## Branch State

- Branch: `codex/unity-fortification-breach-legibility-readout`
- Master base at start: `374bb2e4`
- Contract revision at handoff close: `36`
- Branch status: ready for push and merge coordination after revision-36
  breach-legibility readout delivery

## Next Action

1. Push `codex/unity-fortification-breach-legibility-readout`.
2. Merge it to `master` via the merge-temp ceremony.
3. Follow with the optional fortification sub-slice 8 breach sealing /
   recovery pass so defenders can spend stone and worker time to close
   `OpenBreachCount` back toward zero over time.
