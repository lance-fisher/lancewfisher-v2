---
name: slice-completion
description: Invoke when completing a Unity implementation slice, wrapping up a sub-slice, or when ready to merge a lane branch to master. Runs the 10-step validation gate, authors the per-slice handoff, updates NEXT_SESSION_HANDOFF.md, PROJECT_STATE.json, and the concurrent session contract, then commits on the lane branch and merges to master. Trigger phrases: "complete slice", "wrap up", "finish slice", "ready to merge", "slice done", "all phases green".
---

# Slice Completion Skill (Bloodlines)

Run this ritual at the end of every Unity implementation sub-slice or slice before merging to master. Nothing merges without all gates green.

## Step 1: Confirm branch

```bash
git branch --show-current
```

If not on the correct lane branch, stop. Do not merge from the wrong branch. Switch explicitly.

## Step 2: Run the 10-step validation gate (in order, serially)

Unity holds a project lock. Do NOT run these in parallel.

```powershell
# 1. Runtime assembly
dotnet build unity/Assembly-CSharp.csproj -nologo
# Must exit 0 with 0 errors.

# 2. Editor assembly
dotnet build unity/Assembly-CSharp-Editor.csproj -nologo
# Must exit 0 with 0 errors.

# 3. Bootstrap runtime smoke
powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1
# Success line must carry all prior proof fields plus any new fields this slice added.

# 4. Combat smoke
powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1
# Melee and projectile phases both green.

# 5. Scene shells
powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1
# Both Bootstrap and Gameplay scene shells green.

# 6. Conviction smoke (if present)
powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityConvictionSmokeValidation.ps1

# 7. Dynasty smoke (if present)
powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityDynastySmokeValidation.ps1

# 8. Faith smoke (if present)
powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityFaithSmokeValidation.ps1

# 9. Data and runtime bridge
node tests/data-validation.mjs   # exit 0
node tests/runtime-bridge.mjs    # exit 0

# 10. Contract staleness check
powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1
# exit 0 required
```

For lane-specific smoke validators not listed above (e.g. `Invoke-BloodlinesUnitySaveLoadSmokeValidation.ps1`), insert them after step 8 and before step 9 data validation. All lane smokes must be green.

If any gate is red, stop. Do not proceed to commit or merge. Fix the failing gate first.

## Step 3: Author the per-slice handoff

Write to `docs/unity/session-handoffs/YYYY-MM-DD-unity-<lane>-<sub-slice>.md`.

Required sections:

```markdown
# Unity Slice Handoff: <lane> / <sub-slice>

**Date:** YYYY-MM-DD
**Lane:** <lane-name>
**Branch:** <lane-branch>
**Status:** Complete / In Progress

## Goal

One paragraph. What this sub-slice set out to deliver.

## Browser Reference

src/game/core/simulation.js:<line> <functionName>
(or ai.js if applicable)

## Canon Reference

<doc-path>:<section heading>

## Work Completed

Bullet list of what was created, modified, or extended. Include file paths.

## Scope Discipline

What was explicitly NOT done. Reference the kickoff prompt's scope-exclusion list.

## Verification Results

Each gate from the 10-step list with status (PASS / SKIP-not-applicable / FAIL-blocked).
Include the success line output from any smoke validator.

## Current Readiness

One sentence. "Merged to master. All gates green." or "Sub-slice N complete. Gate X still failing due to Y."

## Next Action

Exactly one sentence. The first action for the next session or sub-slice.
```

## Step 4: Update shared state files (append only)

### NEXT_SESSION_HANDOFF.md

Append a new entry at the bottom. Do NOT overwrite or replace any prior lane's entry.

```markdown
## <lane-name> (as of YYYY-MM-DD)

Latest sub-slice: <N>. Status: <Complete/In Progress>.
Handoff: docs/unity/session-handoffs/YYYY-MM-DD-unity-<lane>-<sub-slice>.md
Next: <one sentence next action>
```

### continuity/PROJECT_STATE.json

Update the relevant pointer field for this lane (e.g. `latest_unity_save_load_handoff`). Do not remove any other lane's fields.

### CONCURRENT_SESSION_CONTRACT.md

If this slice completes the lane (all sub-slices done and merged):
- Move the lane entry from Active Lanes to Completed Lanes.
- Bump Revision, update Last Updated to today, update Last Updated By.
- Run `scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1` again to confirm.

If the lane is still in progress (partial sub-slice completion):
- Update the lane's Status from `active` to `active` with a "Next Action" note.
- Still bump Revision and Last Updated.

## Step 5: Commit on lane branch

```bash
git add <specific files only — no git add -A>
git status  # verify nothing unintended is staged
git commit -m "$(cat <<'EOF'
<lane>: <sub-slice description>

<one or two sentence summary of what was implemented and verified>
EOF
)"
```

Never commit on master directly. Lane branch only.

## Step 6: Merge to master

```bash
git checkout master
git pull
git merge --no-ff <lane-branch> -m "Merge <lane-branch>: <sub-slice name>"
git push origin master
git push origin <lane-branch>
```

Verify master advanced:
```bash
git log --oneline origin/master -3
```

## What this skill does NOT do

- Does not fix failing gates (that's the job of the implementation pass that ran before this).
- Does not check canon alignment (canon-enforcement).
- Does not check performance (performance-and-scale).
- Does not create new lanes (lane-claim).
- Does not reset a stuck Unity process (unity-process-reset).
