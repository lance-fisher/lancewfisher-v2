# Gameplay Scenes

This folder is reserved for the canonical live battlefield and map-specific gameplay scenes.

## Immediate Next Scene

- `IronmarkFrontier.unity`

## Preferred Creation Path

- Run `Bloodlines -> Scenes -> Create Bootstrap And Gameplay Scene Shells` in the already-open Unity editor.
- Lock-free batch alternative: `scripts/Invoke-BloodlinesUnityCreateGameplaySceneShells.ps1`

## Required Relationship To Bootstrap

- `IronmarkFrontier.unity` is the first map-facing gameplay shell for the canonical `ironmark_frontier` data lane.
- The Bootstrap scene should be able to hand off into it without inventing a second map source or second spawn path.

## First Verification Target

The first playable version of `IronmarkFrontier.unity` should prove:

- map bootstrap authoring is wired
- spawned units exist in-world
- spawned buildings and control points are present
- the first battlefield camera shell can inspect the spawned ECS battlefield

Keep this folder dedicated to playable battlefield scenes. Do not use it as an archive or scratch area; use `Sandbox/` or `Testbeds/` for experimental scene work instead.
