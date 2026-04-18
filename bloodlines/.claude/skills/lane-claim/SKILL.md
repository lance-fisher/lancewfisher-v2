---
name: lane-claim
description: Invoke when starting a new Unity implementation lane, claiming an unblocked Tier 1 candidate from the concurrent session contract, or creating a new lane branch. Reads the contract, verifies the lane is unclaimed, creates the branch, adds the Active Lanes entry, bumps the contract revision, runs the staleness check, and commits the contract bump as the first commit on the lane branch. Trigger phrases: "claim lane X", "start lane X", "begin work on X lane", "I want to work on X", "pick up X lane".
---

# Lane Claim Skill (Bloodlines)

Every Unity implementation slice operates within a named lane. This skill performs the lane-claim ritual safely before any implementation work begins.

**Authority:** `docs/unity/CONCURRENT_SESSION_CONTRACT.md`. Read the current revision before making any changes.

## Step 1: Read the current contract

```bash
cat docs/unity/CONCURRENT_SESSION_CONTRACT.md
```

Note:
- The current Revision number
- The Last Updated date and Last Updated By
- Which lanes are currently Active
- Which lanes appear under "Next Unblocked Tier 1 Lanes"

## Step 2: Verify the target lane is claimable

The target lane must:
1. Appear under "Next Unblocked Tier 1 Lanes" (not Active, not Completed, not Paused).
2. Have no blocking dependency on another Active lane that has not yet merged to master.

If the lane is already Active with another agent listed as Owner: STOP. Do not claim a lane owned by another active session. Report the conflict to the operator.

If the lane is not in "Next Unblocked" at all: verify it exists in the contract. If it does not exist, it may need to be designed first. Do not invent lane scope — lanes come from the migration plan at `docs/plans/2026-04-17-browser-to-unity-migration-plan.md`.

## Step 3: Confirm agent identity and branch prefix

- **Claude Code sessions** use branch prefix: `claude/unity-<lane-slug>`
- **Codex sessions** use branch prefix: `codex/unity-<lane-slug>`

The `-Session` identifier for Last Updated By: `claude-<lane-slug>-<YYYY-MM-DD>` or `codex-<lane-slug>-<YYYY-MM-DD>`.

## Step 4: Create the lane branch

```bash
git checkout master
git pull
git checkout -b claude/unity-<lane-slug>
# or: git checkout -b codex/unity-<lane-slug>
```

Verify:
```bash
git branch --show-current
```

## Step 5: Update the contract

Edit `docs/unity/CONCURRENT_SESSION_CONTRACT.md`:

### 5a. Add entry to Active Lanes section

```markdown
### <Lane Name>

- **Status:** active
- **Owner Agent:** claude-code  (or: codex)
- **Branch Prefix:** claude/unity-<lane-slug>
- **Owned Paths:**
  - `unity/Assets/_Bloodlines/Code/<LaneFolder>/**`
  - `scripts/Invoke-BloodlinesUnity<LaneName>SmokeValidation.ps1`
  - (list any other exclusively owned paths)
- **Lane Authority Documents:**
  - `docs/unity/session-handoffs/YYYY-MM-DD-unity-<lane>-<sub-slice>.md` (will be created)
- **Shared-File Narrow Edits Authorized:**
  - (list any authorized shared-file narrow edits, or "none")
- **Next Action:** Begin sub-slice 1.
```

### 5b. Remove the entry from "Next Unblocked Tier 1 Lanes"

Find and delete or comment out the line that listed this lane under Next Unblocked. Do not delete the section header.

### 5c. Bump Revision and update header

At the top of the contract:

```markdown
**Revision:** <old + 1>
**Last Updated:** 2026-04-17  (today's date)
**Last Updated By:** claude-<lane-slug>-2026-04-17
```

### Collision handling (if another agent bumped simultaneously)

If you see a merge conflict on the Active Lanes or Revision line when you try to commit:
1. Keep BOTH active lane entries. Sort them alphabetically within the Active Lanes section.
2. Remove BOTH from the Next Unblocked section.
3. Take the higher Revision number.
4. Set Last Updated By to `claude-<lane1>-and-codex-<lane2>-<date>` to flag the simultaneous bump.
5. Never drop the other agent's entry.

## Step 6: Run the staleness check

```powershell
powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1
```

Must exit 0. If it exits non-zero: read the output, fix the staleness cause, re-run. Do not proceed to implementation with a stale contract.

## Step 7: Commit the contract bump as the first commit on the lane branch

```bash
git add docs/unity/CONCURRENT_SESSION_CONTRACT.md
git status  # verify only the contract is staged
git commit -m "$(cat <<'EOF'
Claim <lane-name> lane (claude/unity-<lane-slug>)

Adds lane entry to Active Lanes, bumps contract to rev <N>, removes
<lane-name> from Next Unblocked Tier 1 Lanes.
EOF
)"
```

This commit is the lane claim receipt. It must be the first commit on the lane branch.

## Step 8: Confirm and report

After the commit, report to the session:

```
Lane claimed: <lane-name>
Branch: claude/unity-<lane-slug> (or codex/...)
Contract: rev <N>, last updated 2026-04-17
Owned paths: [list]
Next action: [first sub-slice description from the migration plan or kickoff prompt]
```

Now proceed to the first sub-slice.

## What this skill does NOT do

- Does not design the lane scope — that comes from the migration plan.
- Does not implement any systems — it only claims the lane.
- Does not merge to master — that's the slice-completion skill.
- Does not resolve lane scope conflicts between competing designs (that requires operator input).
