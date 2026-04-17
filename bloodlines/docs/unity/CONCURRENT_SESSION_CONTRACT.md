# Concurrent Session Contract for Bloodlines Unity Work

This file is the coordination protocol for concurrent Claude and Codex sessions
operating on the Bloodlines Unity project. Keep it authoritative; edit only
when the protocol itself changes.

## Lane Ownership

| Session | Branch | Lane | Exclusive File Scope |
|---|---|---|---|
| Codex  | `codex/unity-combat-foundation`   | Combat foundation   | `unity/Assets/_Bloodlines/Code/Combat/**`, `unity/Assets/_Bloodlines/Code/Editor/BloodlinesCombatSmokeValidation.cs`, `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1`, `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Combat.cs` |
| Claude | `claude/unity-food-water-economy` | Food/water economy  | `unity/Assets/_Bloodlines/Code/Economy/**`, `unity/Assets/_Bloodlines/Code/Components/FoodWaterConsumptionComponent.cs`, `unity/Assets/_Bloodlines/Code/Components/ResourceProductionBuildingComponent.cs`, `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs` |

Shared-but-surgical files (either session may edit, but keep edits narrow and
additive; rebase/merge carefully at hand-off):

- `unity/Assets/_Bloodlines/Code/Authoring/BloodlinesMapBootstrapAuthoring.cs`
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesMapBootstrapBaker.cs`
- `unity/Assets/_Bloodlines/Code/Systems/SkirmishBootstrapSystem.cs`
- `unity/Assets/_Bloodlines/Code/Systems/UnitProductionSystem.cs`
- `unity/Assets/_Bloodlines/Code/Editor/JsonContentImporter.cs`
- `unity/Assembly-CSharp.csproj`
- Continuity files (`CURRENT_PROJECT_STATE.md`, `NEXT_SESSION_HANDOFF.md`,
  `continuity/PROJECT_STATE.json`) — update only at end of slice, rebase first.

## Unity Wrapper Lock

Unity batch mode holds a project lock. Only one session may run
`scripts\Invoke-BloodlinesUnity*.ps1` or any Unity.exe invocation at a time.

Lock file path: `D:\ProjectsHome\Bloodlines\.unity-wrapper-lock`

Lock file format (UTF-8, no BOM):

```
<session-name> <iso-utc-timestamp> <wrapper-script-path>
```

Protocol:

1. Before running any Unity wrapper, check if the lock file exists.
2. If it exists and its timestamp is less than 15 minutes old, and its
   `<session-name>` is not yours, wait. Poll every 30 seconds up to 10 minutes.
3. If it exists and its timestamp is older than 15 minutes, it is stale.
   Reclaim it by overwriting with your own session name and current timestamp.
4. If absent, create it with your session name, current ISO UTC timestamp,
   and the wrapper path you are about to run.
5. Run the wrapper.
6. On success or failure, delete the lock file.

Do not run multiple Unity wrappers in parallel. Do not skip the lock.

## Branch and Merge Discipline

- Both sessions base their work on current `master`.
- Both commit on their own branch.
- Neither pushes to master directly.
- Before pushing, rebase onto `origin/master` to keep merges linear.
- Merges to master are human-coordinated. Neither session auto-merges the other.

## Validation Gates

Each slice is considered done only when all of these are green for the
session's branch:

1. `dotnet build unity/Assembly-CSharp.csproj -nologo` — 0 errors.
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` — 0 errors.
3. `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` —
   still green (no lane may break the shared first shell).
4. The lane's own smoke validator:
   - Combat lane: `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1`
   - Economy lane: bootstrap smoke covers this.
5. `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1`.
6. `node tests/data-validation.mjs`.
7. `node tests/runtime-bridge.mjs`.

## Handoff Discipline

- Each session writes its own per-slice handoff at
  `docs/unity/session-handoffs/<YYYY-MM-DD>-unity-<lane>-<slice>.md`.
- Do not collide filenames.
- Update `CURRENT_PROJECT_STATE.md`, `NEXT_SESSION_HANDOFF.md`, and
  `continuity/PROJECT_STATE.json` only at the end of a slice, after rebasing,
  and only append — never overwrite the other session's entries.

## Non-Negotiables (unchanged from root governance)

- Full canonical depth. No MVP. No phased release.
- Browser runtime is frozen behavioral spec; do not extend.
- No secrets or `.env` committed.
- No `git push --force`; no rewriting pushed commits.
- No skipping hooks with `--no-verify`.
