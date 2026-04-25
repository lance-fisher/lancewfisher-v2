# LOCAL MODEL CONTINUITY PROMPT — Bloodlines Unity AI Strategic Layer
# Instructions: Fill in the [BRACKETED] sections with your session results, then
# deliver this entire file as the FIRST message of the next session.
# Do not modify the structure or the instructions above each section.

---

## Continuity Header

Date of this handoff: [YYYY-MM-DD]
Completed by: local-model-[YYYY-MM-DD]
Last completed sub-slice: Sub-slice [N] -- [NAME]
Next sub-slice to start: Sub-slice [N+1] -- [NAME]

---

## What Was Just Completed

[Write one paragraph describing what you implemented. Be specific: which files were
created, what system does, what the browser reference was.]

---

## Verification Results

- `dotnet build Assembly-CSharp.csproj`: [PASS / FAIL -- paste first error line if FAIL]
- `dotnet build Assembly-CSharp-Editor.csproj`: [PASS / FAIL]
- `node tests/data-validation.mjs`: [exit 0 / exit N]
- `node tests/runtime-bridge.mjs`: [exit 0 / exit N]
- `Invoke-BloodlinesUnityContractStalenessCheck.ps1`: [exit 0 / exit N, revision confirmed: NNN]

---

## Current Contract Revision

CONCURRENT_SESSION_CONTRACT.md is now at revision: [NNN]
Last Updated By field is now: local-model-[YYYY-MM-DD]

---

## Git State

Branch created: [claude/unity-ai-SLICE-NAME]
Commit hash pushed to master: [FULL 40-CHAR HASH]
Commit message first line: [PASTE FIRST LINE]

---

## Errors Encountered (if any)

[List any build errors you hit and how you resolved them. Format:
Error: <message>
Resolution: <what you changed>
Leave this section blank (write "None") if all builds passed first try.]

---

## Files Created This Session

[List every new file with a one-line description. Example:
- unity/Assets/_Bloodlines/Code/AI/AIFaithCommitmentSystem.cs -- one-shot faith auto-select system
- scripts/Invoke-BloodlinesUnityFaithCommitmentSmokeValidation.ps1 -- PS1 validator wrapper]

---

## Files Modified This Session

[List every modified file with what changed. Example:
- unity/Assembly-CSharp.csproj -- added 2 new Compile entries
- docs/unity/CONCURRENT_SESSION_CONTRACT.md -- bumped to revision 140]

---

## Next Action (Exact)

Implement Sub-slice [N+1]: [NAME] from LOCAL_MODEL_WORK_QUEUE.md.

1. Read all "Read these files first" entries listed under that sub-slice.
2. Create the files listed in the "Files to create" table.
3. Register them in Assembly-CSharp.csproj and Assembly-CSharp-Editor.csproj.
4. Bump CONCURRENT_SESSION_CONTRACT.md from revision [NNN] to [NNN+1].
5. Run all 5 validation commands. Fix any errors before proceeding.
6. Write handoff document at: docs/unity/session-handoffs/[DATE]-unity-ai-[SLICE-NAME].md
7. Commit and push to master using the commit message template in LOCAL_MODEL_WORK_QUEUE.md.

---

## Standing Instructions (do not skip these)

You are implementing Unity ECS code for Bloodlines. Your job is to continue the
work queue in LOCAL_MODEL_WORK_QUEUE.md.

- All code patterns are in LOCAL_MODEL_STARTER_PROMPT.md in this same directory.
- All type signatures and namespaces are pre-documented in the work queue.
- Do not invent types or fields. Only use types listed in the starter prompt or
  that you have verified by reading an existing file.
- Do not modify files outside the AI layer unless the sub-slice explicitly says to.
- Do not add features beyond the sub-slice specification.
- Run all 5 validation commands after each sub-slice and fix errors before committing.
- Bump the CONCURRENT_SESSION_CONTRACT.md revision for each sub-slice.
- Write a handoff document for each sub-slice before committing.
- Commit on a branch named `claude/unity-ai-<slice-name>` then push to master with:
    HASH=$(git rev-parse HEAD)
    git push origin $HASH:master

After completing the sub-slice listed in "Next Action", fill in a fresh copy of
this continuity prompt with your results and prepare it for the following session.
