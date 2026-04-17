# 2026-04-17 Governance: Concurrent Session Contract Source of Truth

## Goal

Make `docs/unity/CONCURRENT_SESSION_CONTRACT.md` the single authoritative source
of truth for Unity lane ownership, file-scope boundaries, shared-file rules,
and forbidden paths. Wire governance so every future session can check contract
currency automatically and knows exactly what it must update when a lane changes.

## What This Slice Did

This slice is a governance-only pass on the `claude/unity-graphics-infrastructure`
branch. It intentionally did not advance any gameplay lane (no combat, economy,
AI, or rendering code was committed).

### 1. Rewrote `docs/unity/CONCURRENT_SESSION_CONTRACT.md` (Revision 1)

The pre-schema document (revision 0) existed as a simple table with no schema.
This revision replaces it with the full canonical schema. All lane and path data
from the pre-schema document is preserved; coverage has been extended, not
regressed. The new document contains:

- Contract Metadata block (Revision, Last Updated, Last Updated By, Supersedes)
- Purpose statement
- Four lane subsections: `economy-and-ai` (retired), `combat-and-projectile`
  (retired), `graphics-infrastructure` (active), `combat-attack-move` (paused)
- Shared Files section listing eleven files with explicit narrow-modification rules
- Forbidden Paths section
- State Documents section (append-only discipline)
- Branch Discipline summary
- Wrapper Lock summary
- Validation Gate (canonical 8-step ordered list)
- Staleness Rule (one paragraph)
- Reconciliation Notes (six discrepancies found between revision 0 and handoff
  record; all addressed in revision 1)

### 2. Created `scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1`

New PowerShell script that:

- Reads the `Last Updated:` date from the contract metadata block.
- Enumerates `docs/unity/session-handoffs/*.md`.
- Extracts the date from each handoff filename prefix (YYYY-MM-DD).
- Compares the maximum handoff date to the contract Last Updated date.
- Exits 0 if the contract date is current or newer, or if no dated handoffs exist.
- Exits 1 with an explicit diagnostic message if the contract is older than the
  newest handoff, if the contract file is missing, or if the Revision field
  cannot be parsed as a positive integer.

How to run:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1
```

Expected output on a fresh checkout after this slice:

```
STALENESS CHECK PASSED: Contract revision=1, last-updated=2026-04-17 is current.
Latest handoff: 2026-04-17-governance-contract-source-of-truth.md (2026-04-17).
```

### 3. Updated `CLAUDE.md` (additive only)

Two new sections were appended after the "Current Direction" section:

- **Unity Slice Completion Protocol** -- six-step ordered checklist every slice
  must follow. Step 4 is the new contract-update gate: if a slice creates,
  renames, retires, or changes the owned paths of any lane, the session must
  bump the contract revision, set Last Updated, and run the staleness check
  before committing.
- **Unity Validation Gate** -- the canonical eight-step ordered command list.
  Step 8 is the new staleness check script. The gate closes with a pointer to
  the contract document.

No existing CLAUDE.md content was removed or altered.

## Reconciliation Notes Preserved in Contract

Six discrepancies were identified between the pre-schema contract (revision 0)
and the per-slice handoff record:

1. Economy branch name (`claude/unity-food-water-economy` vs the actual
   `claude/unity-enemy-ai-economic-base` used for Sessions 120-124).
2. AI lane paths (`Code/AI/**`, `BloodlinesDebugCommandSurface.AI.cs`) absent
   from the pre-schema contract.
3. Two Codex combat branches collapsed into one entry in the pre-schema contract.
4. Combat smoke gate was listed as lane-specific; now mandated for all lanes.
5. `BloodlinesBootstrapRuntimeSmokeValidation.cs` was listed as economy-lane-
   exclusive; now documented as a shared file.
6. `BloodlinesDebugCommandSurface.cs` absent from the shared-file list.

All six are addressed in contract revision 1.

## What Was Intentionally Not Done

- No combat, economy, AI, or rendering code was written or committed.
- The untracked graphics infrastructure files already on the
  `claude/unity-graphics-infrastructure` branch
  (`FactionTintComponent.cs`, `BuildingVisualDefinition.cs`,
  `UnitVisualDefinition.cs`, `BloodlinesPlaceholderMeshGenerator.cs`,
  `Code/Rendering/`, `Shaders/BloodlinesFactionColor.shader`) were not staged
  or committed. Those belong to the graphics lane's first implementation slice,
  not to this governance slice.
- The staleness check script was not run through the full Unity validation gate
  because this slice contains no Unity code changes. The script itself is pure
  PowerShell and does not require Unity.

## Verification

```powershell
# Confirm the contract file has the expected revision and schema sections
Select-String -Path docs/unity/CONCURRENT_SESSION_CONTRACT.md -Pattern "Revision: 1"
Select-String -Path docs/unity/CONCURRENT_SESSION_CONTRACT.md -Pattern "Staleness Rule"

# Run the staleness check
powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1

# Confirm CLAUDE.md received the new sections
Select-String -Path CLAUDE.md -Pattern "Unity Slice Completion Protocol"
Select-String -Path CLAUDE.md -Pattern "Unity Validation Gate"
```

## Next Action for the Graphics Infrastructure Lane

The `claude/unity-graphics-infrastructure` branch already has the following
untracked files staged in the working tree. The next implementation slice on
this lane should:

1. Review those files for correctness against the visual implementation guide.
2. Run `dotnet build unity/Assembly-CSharp.csproj -nologo` and
   `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` to confirm no
   compile errors.
3. Run the full validation gate (all 8 steps).
4. Commit, following the Unity Slice Completion Protocol from CLAUDE.md.
5. Because this will be the first committed slice on the graphics-infrastructure
   lane, the contract does NOT need a revision bump on that commit (the lane
   already exists in the contract). A bump is required only if the owned paths
   change from what is listed in contract revision 1.

## Next Action for the Combat-Attack-Move Lane (Codex)

The next Codex prompt lives at `docs/unity/CODEX_NEXT_PROMPT_ATTACK_MOVE.md`.
The lane is documented as `combat-attack-move` (paused) in contract revision 1.
When Codex starts that slice, it should create branch `codex/unity-attack-move`,
confirm the lane subsection in the contract is accurate, and proceed. No
contract revision bump is needed merely to activate the paused lane; a bump is
required if the owned paths or branch prefix change from what is listed.
