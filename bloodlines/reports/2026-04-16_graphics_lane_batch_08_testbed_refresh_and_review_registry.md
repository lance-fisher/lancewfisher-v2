# Bloodlines Graphics Lane Batch 08 Testbed Refresh And Review Registry

Date: 2026-04-16
Scope: completed Unity testbed refresh for the latest graphics batches plus applied batch-review governance

## Purpose

This follow-up closes the remaining Unity graphics-lane gap from the earlier Batch 07 and Batch 08 staging passes.

It confirms that the governed Unity populate path was rerun successfully and that the latest staged boards are now present in the saved testbed scenes on disk.

It also adds the first explicit review ledger and machine-readable review registry for graphics batches 01 through 08.

## Unity Testbed Refresh Completed

The governed populate path was rerun successfully through:

- `scripts/Invoke-BloodlinesUnityGraphicsPopulateTestbeds.ps1`

The batch log now confirms:

- `Bloodlines visual testbed population complete.`

Saved scene verification now confirms:

- `unity/Assets/_Bloodlines/Scenes/Testbeds/VisualReadability/VisualReadability_Testbed.unity`
  - contains `Support Structure Board Wall`
  - contains `MarketStorehouseGranaryBoard`
  - contains `CovenantSiteProgressionBoard`
- `unity/Assets/_Bloodlines/Scenes/Testbeds/MaterialLookdev/MaterialLookdev_Testbed.unity`
  - contains `Settlement Variant Wall`
  - contains `HouseOverlaySupportBoard`
  - contains `MarketTradeYardBoard`
  - contains `CovenantOverlayArchitectureBoard`

## Review Governance Added

New review-governance surfaces now exist at:

- `13_AUDIO_VISUAL/GRAPHICS_PIPELINE/GRAPHICS_BATCH_REVIEW_LEDGER_2026-04-16.md`
- `13_AUDIO_VISUAL/GRAPHICS_PIPELINE/MANIFESTS/concept_batch_review_registry.json`

These surfaces record the current truth for Batches 01 through 08:

- all are preserved
- all remain `placeholder_only`
- all remain `not_integration_ready`
- Unity evidence and next review actions are now explicitly tracked

## Verification

- `dotnet build unity/Assembly-CSharp.csproj -nologo -p:BaseIntermediateOutputPath=unity/Temp/codex-runtime-obj-3/ -p:OutputPath=unity/Temp/codex-runtime-bin-3/`
  - passed with `0` warnings and `0` errors
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo -p:BaseIntermediateOutputPath=unity/Temp/codex-editor-obj-3/ -p:OutputPath=unity/Temp/codex-editor-bin-3/`
  - passed with `0` errors
  - longstanding editor/importer `CS0649` warnings remain
- `continuity/PROJECT_STATE.json` parses successfully after this follow-up
- Batch 08 raster outputs remain preserved at:
  - `artifacts/graphics-batch-08-browser-raster.log`

## What This Pass Left Untouched

This pass did not:

- promote any graphics batch into runtime-facing art folders
- alter gameplay scenes outside the governed testbeds
- force any approval verdict beyond `placeholder_only`
- modify lore canon, House canon, faith rules, economy rules, or render-pipeline choices

## Next Graphics-Lane Recommendations

- perform the first formal directed review pass across Batches 01 through 08 using the new review ledger and registry
- prioritize:
  - nine-House military wall review
  - support-structure role-read review
  - settlement-variant and covenant-overlay plausibility review
- only after that, generate the next graphics-only follow-up wave:
  - House-specific civic overlays
  - trade-hub upgrade-state boards
  - water-infrastructure replacement candidates
  - covenant-specific sacred-complex variants
