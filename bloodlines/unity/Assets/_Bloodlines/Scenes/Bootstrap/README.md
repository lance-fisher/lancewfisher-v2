# Bootstrap Scenes

This folder is reserved for the canonical Unity bootstrap entry scenes.

## Immediate Next Scene

- `Bootstrap.unity`

## Preferred Creation Path

- Run `Bloodlines -> Scenes -> Create Bootstrap And Gameplay Scene Shells` in the already-open Unity editor.
- Lock-free batch alternative: `scripts/Invoke-BloodlinesUnityCreateGameplaySceneShells.ps1`

## Required Contents For `Bootstrap.unity`

1. A GameObject with `BloodlinesMapBootstrapAuthoring`.
2. `Map` assigned to `Assets/_Bloodlines/Data/MapDefinitions/ironmark_frontier.asset`.
3. Default spawn toggles left enabled for the first verification pass unless a narrower debug case is needed.

## Expected Runtime Outcome

On Play Mode entry, the bake/bootstrap path should create the first live ECS shell for:

- factions
- buildings
- units
- resource nodes
- settlements
- control points

Do not place long-term gameplay cameras or UI in this scene unless they are specifically part of the bootstrap shell. The purpose of this folder is entry and handoff into gameplay scenes, not final content accumulation.
