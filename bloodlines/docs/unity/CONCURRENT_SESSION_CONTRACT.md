# Bloodlines Unity Concurrent Session Contract

## Contract Metadata

- Revision: 10
- Last Updated: 2026-04-18
- Last Updated By: codex-fortification-siege-2026-04-18
- Supersedes: revision 9 (fortification-siege lane advanced through sub-slice 2; siege support and field-water handoff landed on top of the rebased post-match-progression master)

## Purpose

This document is the single source of truth for Unity lane ownership, file-scope boundaries, shared-file rules, and forbidden paths across all concurrent Claude and Codex sessions working on the Bloodlines Unity project. Prompts and session handoffs do not hardcode lane data; they point here. Any session that needs to know what it may touch, what branches are in flight, or what the canonical validation gate is must read this document first. The contract is maintained across revisions so each bump constitutes an auditable record of how the concurrent session structure has evolved.

## Active Lanes

### Lane: economy-and-ai

- Status: retired
- Branch Prefix: `claude/unity-food-water-economy`, `claude/unity-enemy-ai-economic-base`
- Owner Agent: claude-code
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Economy/**`
  - `unity/Assets/_Bloodlines/Code/AI/**`
  - `unity/Assets/_Bloodlines/Code/Components/FactionLoyaltyComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.AI.cs`
- Owned Scripts:
  - None dedicated. The bootstrap runtime smoke (`scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1`) serves as the verification surface for this lane's work; it is a shared file governed by narrow-modification rules in the Shared Files section.
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-17-unity-building-resource-trickle-and-concurrent-session-contract.md`
  - `docs/unity/session-handoffs/2026-04-17-unity-starvation-response-and-famine-water-crisis.md`
  - `docs/unity/session-handoffs/2026-04-17-unity-faction-loyalty-and-famine-water-crisis-delta.md`
  - `docs/unity/session-handoffs/2026-04-17-unity-cap-pressure-response.md`
  - `docs/unity/session-handoffs/2026-04-17-unity-stability-surplus-loyalty-restoration.md`
  - `docs/unity/session-handoffs/2026-04-17-unity-loyalty-density-hud-readout.md`
  - `docs/unity/session-handoffs/2026-04-17-unity-enemy-ai-economic-base.md`
  - `docs/unity/session-handoffs/2026-04-17-unity-enemy-ai-construction-pass.md`
  - `docs/unity/session-handoffs/2026-04-17-unity-enemy-ai-militia-posture.md`
- Current Branch In Flight: none (merged into master via `aed6969` on 2026-04-17, Sessions 120-124)
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-17-unity-enemy-ai-militia-posture.md`

### Lane: combat-and-projectile

- Status: retired
- Branch Prefix: `codex/unity-combat-foundation`, `codex/unity-projectile-combat`
- Owner Agent: codex
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Combat/**`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesCombatSmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1`
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-17-unity-combat-foundation.md`
  - `docs/unity/session-handoffs/2026-04-17-unity-projectile-combat.md`
- Current Branch In Flight: none (`codex/unity-combat-foundation` merged at `a8dd553`; `codex/unity-projectile-combat` merged at `2ee8096`)
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-17-unity-projectile-combat.md`

### Lane: graphics-infrastructure

- Status: retired (first slice merged via `548d780`; follow-up polish items unstaffed, available to claim as new lane)
- Branch Prefix: `claude/unity-graphics-infrastructure`
- Owner Agent: claude-code
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Rendering/**`
  - `unity/Assets/_Bloodlines/Shaders/**`
  - `unity/Assets/_Bloodlines/Code/Components/FactionTintComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Definitions/BuildingVisualDefinition.cs`
  - `unity/Assets/_Bloodlines/Code/Definitions/UnitVisualDefinition.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlaceholderMeshGenerator.cs`
- Owned Scripts:
  - None yet defined. A dedicated graphics validation wrapper should be added as part of the first substantive implementation commit on this lane.
- Lane Authority Documents:
  - `docs/unity/VISUAL_ASSET_PIPELINE_2026-04-15.md`
  - `docs/unity/BLOODLINES_UNITY_6_3_VISUAL_IMPLEMENTATION_GUIDE_2026-04-16.md`
  - `docs/unity/BLOODLINES_VISUAL_TESTBED_PLAN_2026-04-16.md`
- Current Branch In Flight: `claude/unity-graphics-infrastructure`
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-17-governance-contract-source-of-truth.md`

### Lane: state-snapshot-restore

- Status: retired (all 3 sub-slices merged into master on 2026-04-17)
- Branch Prefix: `claude/unity-save-load-snapshot`
- Owner Agent: claude-code
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/SaveLoad/**`
  - `unity/Assets/_Bloodlines/Code/Components/SnapshotVersionComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.SaveLoad.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesSaveLoadSmokeValidation.cs`
  - `scripts/Invoke-BloodlinesUnitySaveLoadSmokeValidation.ps1`
- Shared-File Narrow Edits Applied:
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs` -- ProbeSnapshotIntegrity probe added after stabilitySurplus phase (sub-slice 3). snapshotIntegrityChecked=True in pass line.
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-17-unity-save-load-snapshot.md`
- Browser Reference: `src/game/core/simulation.js` `exportStateSnapshot` (13822), `restoreStateSnapshot` (13989)
- Current Branch In Flight: none (merged into master 2026-04-17)

### Lane: combat-attack-move

- Status: retired (merged into master via `5167a0b`)
- Branch Prefix: `codex/unity-attack-orders-attack-move`
- Owner Agent: codex
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Combat/**` (extends the combat-and-projectile foundation)
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesCombatSmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1`
- Lane Authority Documents:
  - `docs/unity/CODEX_NEXT_PROMPT_ATTACK_MOVE.md`
  - `docs/unity/session-handoffs/2026-04-17-unity-attack-orders-and-attack-move.md`
- Current Branch In Flight: `codex/unity-attack-move`
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-17-unity-attack-orders-and-attack-move.md`

### Lane: combat-acquisition-and-sight

- Status: retired (merged into master via `dc00fff`)
- Branch Prefix: `codex/unity-target-acquisition-los`
- Owner Agent: codex
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Combat/**`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesCombatSmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1`
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-17-unity-target-acquisition-throttling-and-sight-loss.md`
- Current Branch In Flight: none
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-17-unity-target-acquisition-throttling-and-sight-loss.md`

### Lane: ai-barracks-observability

- Status: retired (merged into master via `3101e98`)
- Branch Prefix: `claude/unity-ai-expansion-and-faith`
- Owner Agent: claude-code
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/AI/AIEconomyControllerComponent.cs` (additive fields: `TargetBarracksCount`, `ControlledBarracksCountCached`)
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.AI.cs` (additive `TryDebugGetAIBuildingCounts`)
- Current Branch In Flight: none

### Lane: conviction-scoring

- Status: retired (merged into master via `7f8de3c`)
- Branch Prefix: `claude/unity-conviction-scoring`
- Owner Agent: claude-code
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Conviction/**`
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Conviction.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesConvictionSmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityConvictionSmokeValidation.ps1`
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-17-unity-conviction-scoring-and-bands.md`
- Current Branch In Flight: none
- Follow-Up (pending operator design call): wire `ConvictionBandEffects.LoyaltyProtectionMultiplier` into loyalty decline, `PopulationGrowthMultiplier` into population growth, `CaptureMultiplier` into capture speed. Browser never applied these multipliers, so wiring is a design decision not an automatic port.

### Lane: dynasty-core

- Status: retired (merged into master via `1aa6ade`)
- Branch Prefix: `claude/unity-dynasty-core`
- Owner Agent: claude-code
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Dynasties/**`
  - `unity/Assets/_Bloodlines/Code/Components/DynastyMemberComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Dynasty.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesDynastySmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityDynastySmokeValidation.ps1`
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-17-unity-dynasty-core.md`
- Current Branch In Flight: none
- Follow-Up (next dynasty slice): wire `DynastyBootstrap.AttachDynasty` into `SkirmishBootstrapSystem`; extend `BloodlinesBootstrapRuntimeSmokeValidation` to assert 8 members per playable faction.

### Lane: faith-commitment

- Status: retired (merged into master via `9036d91`)
- Branch Prefix: `claude/unity-faith-commitment`
- Owner Agent: claude-code
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Faith/**`
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Faith.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesFaithSmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityFaithSmokeValidation.ps1`
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-17-unity-faith-commitment-and-intensity.md`
- Current Branch In Flight: none
- Follow-Up (later faith slices): sacred-site exposure walker, structure intensity regen, wayshrine amplification, covenant tests, holy wars, divine right, conviction event on commit.

### Lane: combat-group-movement-and-stances

- Status: retired (merged into master via `d9e58fc`; all 8 combat phases green)
- Branch Prefix: `codex/unity-group-movement-and-stances`
- Owner Agent: codex
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Combat/**` (group movement, combat stances, unit separation, recent impact recovery)
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Stances.cs`
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Movement.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesCombatSmokeValidation.cs` (phases 1-8: melee, projectile, explicit attack, attack-move, target-visibility, group-movement, separation, stance)
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1`
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-17-unity-group-movement-and-stances.md`
- Current Branch In Flight: none

### Lane: dual-clock-match-progression

- Status: retired (all 3 sub-slices merged into master on 2026-04-17)
- Branch Prefix: `claude/unity-match-progression`
- Owner Agent: claude-code
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Time/**`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesMatchProgressionSmokeValidation.cs`
  - `scripts/Invoke-BloodlinesUnityMatchProgressionSmokeValidation.ps1`
- Shared-File Narrow Edits Planned:
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs` -- match-progression probe to be added after save-load integrity phase (sub-slice 2 or 3)
  - `unity/Assets/_Bloodlines/Code/Systems/SkirmishBootstrapSystem.cs` -- seed DualClockComponent + MatchProgressionComponent singletons (sub-slice 1)
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-17-unity-dual-clock-match-progression.md` (written at slice close)
- Browser Reference:
  - `src/game/core/simulation.js` `tickDualClock` (13795), `updateMatchProgressionState` (13557), `computeMatchProgressionState` (13426), `getMatchProgressionSnapshot` (13650), `declareInWorldTime` (13800)
  - Stage definitions: `founding` (1), `expansion_identity` (2), `encounter_establishment` (3), `war_turning_of_tides` (4), `final_convergence` (5)
  - Phase labels: emergence, commitment, resolution
  - `DUAL_CLOCK_DEFAULT_DAYS_PER_REAL_SECOND = 2`
- Current Branch In Flight: `claude/unity-match-progression`
- Last Slice Handoff: none yet

### Lane: fortification-siege-imminent-engagement

- Status: active
- Branch Prefix: `codex/unity-fortification-siege`
- Owner Agent: codex
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Fortification/**`
  - `unity/Assets/_Bloodlines/Code/Siege/**`
  - `unity/Assets/_Bloodlines/Code/Components/FortificationComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Components/FortificationReserveComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Components/SiegeSupportComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Components/FieldWaterComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Components/SiegeSupplyTrainComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Fortification.cs`
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Siege.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesFortificationSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesSiegeSmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityFortificationSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnitySiegeSmokeValidation.ps1`
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-17-unity-fortification-siege-fortification-tier-and-reserves.md`
  - `docs/unity/session-handoffs/2026-04-18-unity-fortification-siege-siege-support-and-field-water.md`
- Current Branch In Flight: `codex/unity-fortification-siege`
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-18-unity-fortification-siege-siege-support-and-field-water.md`

## Next Unblocked Tier 1 Lanes (Unclaimed)

Forward work is prioritized in the browser-to-Unity migration plan at `docs/plans/2026-04-17-browser-to-unity-migration-plan.md`. The items below are unblocked and unclaimed. Any agent resuming a session may claim one by adding an entry under Active Lanes above, bumping Revision, and proceeding.

### Next Lane Candidate: ai-strategic-layer-port

- Suggested Branch: `claude/unity-ai-strategic-layer` or `codex/unity-ai-strategic-layer`.
- Target Paths: `unity/Assets/_Bloodlines/Code/AI/**` (extensive additions).
- Browser Reference: `src/game/core/ai.js` (3,141 LOC full strategic brain).
- Scope: port in per-stage slices. Each sub-slice gets its own handoff.

## Shared Files (Narrow Modification Rights for All Lanes)

The following files may be edited by any lane, but only via narrow, additive changes. No lane may rewrite, restructure, or behaviorally alter these files outside of its own seam.

- `unity/Assets/_Bloodlines/Code/Authoring/BloodlinesMapBootstrapAuthoring.cs`
  Rule: add new field readings from definition assets and seed payloads only. No behavioral rewrites. No moving or removing existing authoring logic.

- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesMapBootstrapBaker.cs`
  Rule: add new component baking calls for new seed element fields only. No behavioral rewrites. No removing existing baking logic.

- `unity/Assets/_Bloodlines/Code/Components/MapBootstrapComponents.cs`
  Rule: add new fields to seed element structs only. No removing or renaming existing fields. No restructuring the seed element layout.

- `unity/Assets/_Bloodlines/Code/Systems/SkirmishBootstrapSystem.cs`
  Rule: add new component attachments in `SpawnFactionEntity` and adjacent spawn helpers only. No behavioral rewrites. No removing or reordering existing spawn logic.

- `unity/Assets/_Bloodlines/Code/Systems/UnitProductionSystem.cs`
  Rule: add new component attachments on produced-unit spawn only. No behavioral rewrites. No removing existing component wiring.

- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugEntityPresentationBridge.cs`
  Rule: narrow fallback-check additions and new entity proxy types only. No rewrite. No behavioral change to existing proxy types. Both the combat lane and the economy/AI lane depend on this file.

- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.cs`
  Rule: add new debug API methods and HUD readout segments only. Do not remove or rename existing methods. Do not reorder the partial class split. When adding a substantial new command surface area, use a new partial file (e.g. `BloodlinesDebugCommandSurface.AI.cs`, `BloodlinesDebugCommandSurface.Combat.cs`).

- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs`
  Rule: narrow additions only. New fields in `RuntimeSmokeState`, new probe phases appended after existing phases, new diagnostic output fields. Do not remove or reorder existing probe phases. Do not change the success-line format of prior probes. All existing probes must remain green after each addition.

- `unity/Assets/_Bloodlines/Code/Editor/JsonContentImporter.cs`
  Rule: add new field mappings in importer record types only. No removing existing field mappings. No changing existing import logic.

- `unity/Assembly-CSharp.csproj`
  Rule: add new `<Compile Include="..." />` entries for new `.cs` files only. Do not remove existing references. Do not change project structure, target framework, or preprocessor defines.

- `unity/Assembly-CSharp-Editor.csproj`
  Rule: same as `Assembly-CSharp.csproj`. New editor-script registrations only.

## Forbidden Paths (All Lanes)

No lane may touch the following paths without explicit per-slice authorization from Lance. Each use requires a named justification in the slice handoff.

- `unity/Assets/_Bloodlines/Scenes/**` - Unity scene files (`.unity`). Structural scene changes may only occur via the governed scene shell tools (`scripts/Invoke-BloodlinesUnityCreateGameplaySceneShells.ps1`, etc.) with explicit authorization.
- `unity/ProjectSettings/**` - Unity project-wide settings. Changes affect all lanes and require explicit authorization.
- `unity/Packages/manifest.json`, `unity/Packages/packages-lock.json` - Package manifest changes require explicit authorization and justification for the new package.
- `src/**`, `play.html` - Browser runtime source. Frozen as behavioral specification. No new systems; only bug fixes and parity-instrumentation explicitly authorized.
- `data/*.json` - Canonical game-data files. These drive the JSON import pipeline. Changes require explicit authorization and re-import verification.
- `archive_preserved_sources/**`, `19_ARCHIVE/**`, `governance/**` - Preservation zones. Read-only without explicit authorization.

## State Documents (Append-Only At End Of Slice)

The following files are immutable mid-slice and append-only at end-of-slice (after rebase, before push):

- `CURRENT_PROJECT_STATE.md`
- `NEXT_SESSION_HANDOFF.md`
- `continuity/PROJECT_STATE.json`

Update these only after all validation gates are green. Never overwrite another lane's entries. Always rebase before updating to avoid divergent state.

## Branch Discipline

Every lane works on its own named branch following the prefix documented in the lane's subsection above. Before pushing, rebase the lane branch onto `origin/master` to keep the merge history linear. Push to the lane branch only. Never push directly to `master`. Merges to `master` are human-coordinated by Lance; neither agent auto-merges the other's branch. If two lanes have diverged significantly at merge time, produce a coordination note in a merge plan document before merging.

## Wrapper Lock

Every Unity batch invocation must go through `scripts/Invoke-BloodlinesUnityWrapperWithLock.ps1` with `-Session <lane-identifier>` matching the lane name. The lock file lives at `.unity-wrapper-lock` in the repo root. Before invoking any Unity wrapper: check whether the lock file exists; if it does and its timestamp is less than 15 minutes old and its session name is not yours, poll every 30 seconds up to 10 minutes. If the lock is older than 15 minutes, reclaim it by overwriting with your session name and current ISO UTC timestamp. Never run two Unity wrapper scripts in parallel from the same session. Delete the lock file after the wrapper completes (success or failure).

## Validation Gate (Canonical Order)

Every slice must pass all of the following before handoff. Run them serially because Unity holds a project lock.

1. `dotnet build unity/Assembly-CSharp.csproj -nologo` -- must exit 0 with 0 errors.
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` -- must exit 0 with 0 errors.
3. `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` -- must remain green; success line must carry all prior proof fields.
4. `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1` -- must remain green for all currently-governed combat phases. All lanes are responsible for not breaking the combat smoke.
5. `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` -- both Bootstrap and Gameplay scene shells must validate.
6. `scripts/Invoke-BloodlinesUnityFortificationSmokeValidation.ps1` -- must remain green for fortification tier and reserve-duty proof phases.
7. `scripts/Invoke-BloodlinesUnitySiegeSmokeValidation.ps1` -- must remain green for baseline, strain, recovery, and siege-support proof phases.
8. `node tests/data-validation.mjs` -- must exit 0.
9. `node tests/runtime-bridge.mjs` -- must exit 0.
10. `scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1` -- must exit 0; confirms this contract is not older than the newest session handoff.

## Staleness Rule

Any resume session that detects the Last Updated date of this document is older than the date of the latest file under `docs/unity/session-handoffs/` must STOP and surface the staleness before doing any other work. The correct action is to bump the Revision, set Last Updated to today, set Last Updated By to the current session identifier, and amend the affected lane subsections to reflect current reality. The `scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1` script automates this check and is part of the canonical validation gate. The contract revision must be bumped whenever a lane is created, renamed, retired, or has its owned paths changed.

## Reconciliation Notes

The following discrepancies were found between the pre-schema contract (revision 0), the per-slice handoffs, and `NEXT_SESSION_HANDOFF.md`. They are recorded for one revision cycle and will be cleared on the next revision if resolved.

1. **Economy branch name mismatch.** The pre-schema contract named the Claude economy branch `claude/unity-food-water-economy`. The actual branches used were `claude/unity-food-water-economy` (Sessions 113-116) and `claude/unity-enemy-ai-economic-base` (Sessions 120-124). Both are now merged. The lane is documented here as `economy-and-ai` to reflect the full scope it grew to cover.

2. **AI paths absent from pre-schema contract.** The pre-schema contract did not list `Code/AI/**` or `BloodlinesDebugCommandSurface.AI.cs` as Claude-owned. Per the Session 120-121 handoffs, Claude created `AIEconomyControllerComponent.cs` under `Code/AI/` and the partial class file `BloodlinesDebugCommandSurface.AI.cs` under `Code/Debug/`. These are now retroactively documented under the retired economy-and-ai lane.

3. **Two Codex combat branches not distinguished.** The pre-schema contract described a single combat-foundation lane on `codex/unity-combat-foundation`. In practice two branches were used: `codex/unity-combat-foundation` (merged) and `codex/unity-projectile-combat` (merged). Both are now documented under the retired combat-and-projectile lane.

4. **Combat smoke gate was lane-specific, not universal.** The pre-schema contract listed the combat smoke validator as optional for the economy lane. Per the handoff record, every economy-lane slice preserved the combat smoke as green once it existed. The canonical gate now mandates it for all lanes.

5. **`BloodlinesBootstrapRuntimeSmokeValidation.cs` ownership ambiguity.** The pre-schema contract listed this file as exclusively owned by the economy lane. Per the Session 113+ handoff record, all lane slices must keep it green and may add narrow diagnostic phases when their new systems require proof. It is now documented as a shared file, not exclusively owned.

6. **`BloodlinesDebugCommandSurface.cs` absent from shared-file list.** This file was implicitly economy-lane-owned in the pre-schema contract. The combat-attack-move lane will need to add debug command APIs for attack orders. It is now documented as a shared file with narrow-modification rules. No conflict has occurred yet.

7. **Stacked Codex combat follow-up.** Lance delegated merge coordination for `codex/unity-attack-move` separately to Claude while Codex continued onto the next combat opportunity. Revision 2 documents the stacked follow-up branch `codex/unity-target-acquisition-los` explicitly so the lane and wrapper-lock ownership remain unambiguous.
