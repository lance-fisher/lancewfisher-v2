# 2026-04-21 Unity Fortification Repair Narrative

## Scope

Prompt-accurate continuation of the Codex-owned fortification-siege lane on
`codex/unity-fortification-repair-narrative`.

This slice started from `origin/master` `6932a5dd` at contract revision `48`
after the worker-locality smoke-wrapper fixup had landed. The goal was to
push info-tone narrative messages whenever a breach closed or a destroyed
fortification counter rebuilt.

## Delivered Files

- `unity/Assets/_Bloodlines/Code/Fortification/BreachSealingSystem.cs`
  - breach closure now routes through `NarrativeMessageBridge.Push` and emits
    `"<faction>'s masons seal a breach at <settlement>."` each time a breach
    seals
- `unity/Assets/_Bloodlines/Code/Fortification/DestroyedCounterRecoverySystem.cs`
  - destroyed-counter rebuild completion now pushes an info-tone repair
    message through `NarrativeMessageBridge.Push`
  - wall rebuilds emit the prompt-accurate
    `"<faction> rebuilds a wall at <settlement>."` text, while other counter
    kinds reuse the same pattern with the rebuilt structure label
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesFortificationRepairNarrativeSmokeValidation.cs`
  - new dedicated validator with three phases:
    single breach close, three breach closes, and wall rebuild
- `scripts/Invoke-BloodlinesUnityFortificationRepairNarrativeSmokeValidation.ps1`
  - dedicated batch wrapper for the repair-narrative validator

## Design Notes

- The slice intentionally reuses the already-landed AI-owned
  `NarrativeMessageBridge` instead of adding a fortification-local buffer or a
  new HUD surface.
- Breach messages fire at the exact point where `OpenBreachCount` decrements,
  so one message maps to one closed breach and the three-breach smoke proves
  that repeated completion emits repeated messages instead of one summary line.
- Destroyed-counter rebuild messages fire only after the linked dead structure
  has been restored to full health and the destroyed counter has decremented,
  so the narrative line tracks a real rebuild completion rather than reserved
  stone or partial worker-hour progress.

## Validation

The slice is green on `D:\BLF13\bloodlines`:

1. `dotnet build unity/Assembly-CSharp.csproj -nologo`
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
3. bootstrap runtime smoke via a worktree-safe wrapper copy
4. combat smoke
5. bootstrap scene-shell validation via a worktree-safe wrapper copy
6. gameplay scene-shell validation via a worktree-safe wrapper copy
7. fortification smoke
8. siege smoke
9. `node tests/data-validation.mjs`
10. `node tests/runtime-bridge.mjs`
11. `scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1`
12. fortification repair narrative smoke

Dedicated repair-narrative smoke phases:

- Phase 1 PASS: one breach closure emits exactly one info-tone
  `"player's masons seal a breach at keep_alpha."`
- Phase 2 PASS: three closures emit three identical info-tone breach messages
- Phase 3 PASS: one wall rebuild emits exactly one info-tone
  `"player rebuilds a wall at keep_alpha."`

Validation artifacts:

- `artifacts/unity-bootstrap-runtime-smoke.log`
- `artifacts/unity-combat-smoke.log`
- `artifacts/unity-bootstrap-scene-validate.log`
- `artifacts/unity-gameplay-scene-validate.log`
- `artifacts/unity-fortification-smoke.log`
- `artifacts/unity-siege-smoke.log`
- `artifacts/unity-fortification-repair-narrative-smoke.log`

## Current Readiness

This slice is complete and ready for push, merge-temp coordination, and
session wrap. No further fortification-siege slice is currently queued.

## Next Action

1. Push `codex/unity-fortification-repair-narrative`.
2. Merge it to `master` via the normal merge-temp ceremony.
3. Record the fortification-siege session wrap through sub-slice 13 and hand
   the next Codex pickup to either an operator-defined fortification
   sub-slice 14 or a different approved arc.
