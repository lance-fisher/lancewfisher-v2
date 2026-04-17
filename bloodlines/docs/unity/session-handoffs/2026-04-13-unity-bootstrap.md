# Bloodlines Unity Bootstrap Handoff

Date: 2026-04-13
Author: Codex

## Canonical Root Work Completed

The canonical Unity project path now exists:
- `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\deploy\bloodlines\unity\`

Project shell created:
- `Assets/`
- `Packages/`
- `ProjectSettings/`
- `Library/`
- `Temp/`
- `Logs/`
- `UserSettings/`

Added bootstrap scaffolding:
- `.gitignore`
- `.gitattributes`
- `README.md`
- `Packages/manifest.json`
- `Assets/_Bloodlines/` folder skeleton

Nested git repo initialized:
- `deploy/bloodlines/unity/.git/`

## Package Baseline Used

Versions were pulled from the installed Unity 6 editor metadata on this machine:
- `com.unity.addressables`: `2.9.1`
- `com.unity.burst`: `1.8.29`
- `com.unity.collections`: `6.4.0`
- `com.unity.entities`: `6.4.0`
- `com.unity.entities.graphics`: `6.4.0`
- `com.unity.feature.development`: `1.0.2`
- `com.unity.ide.rider`: `3.0.39`
- `com.unity.ide.visualstudio`: `2.0.27`
- `com.unity.inputsystem`: `1.19.0`
- `com.unity.mathematics`: `1.3.3`
- `com.unity.render-pipelines.universal`: `17.4.0`
- `com.unity.test-framework`: `1.6.0`
- `com.unity.timeline`: `1.8.12`
- `com.unity.ugui`: `2.0.0`

## What Was Verified

- Unity editor installed locally:
  - `C:\Program Files\Unity\Hub\Editor\6000.4.2f1`
- `ProjectSettings/ProjectVersion.txt` reports `6000.4.2f1`.
- `Packages/manifest.json` is now valid JSON without BOM.
- Unity batchmode can open the project shell after the manifest encoding fix.

## What Is Still Not Green

The project is not yet a fully verified U0 completion.

Observed issue:
- `Packages/packages-lock.json` still reflects built-in package state from the early bootstrap path.
- Batchmode logs showed built-in package registration, but did not clearly confirm custom dependency resolution for Entities, URP, Input System, and Addressables.

Practical interpretation:
- canonical Unity shell exists
- repo scaffolding exists
- package declaration exists
- package installation still needs confirmation, likely via interactive editor open or a longer package-resolution pass

## Why No Initial Commit Was Made

The initial nested-repo commit was intentionally deferred because package resolution has not been positively verified yet. Commit after the package state is confirmed so U0 is a real baseline instead of a half-resolved shell.

## Best Next Step

1. Open `deploy/bloodlines/unity/` in the Unity editor interactively.
2. Confirm Package Manager resolves:
   - Entities
   - Entities Graphics
   - Burst
   - Collections
   - Mathematics
   - Input System
   - URP
   - Addressables
3. Let Unity regenerate `packages-lock.json` if needed.
4. Verify `_Bloodlines` folders and project open cleanly.
5. Create the initial nested-repo commit.
6. Start U1 with `JsonContentImporter`.

## Supporting Logs

Canonical artifacts:
- `deploy/bloodlines/artifacts/unity-create.log`
- `deploy/bloodlines/artifacts/unity-open.log`
