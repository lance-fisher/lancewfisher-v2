# Bloodlines Unity Visual Testbed Plan

Established: 2026-04-16
Scope: scene-safe visual verification lanes for Unity 6.3 LTS

## 1. Purpose

This document defines the first governed Unity testbed structure for Bloodlines visual work.

It exists to let art, materials, icons, and terrain be checked in-engine without colliding with:

- gameplay bootstrap work
- map authoring work
- ECS simulation work
- UI implementation work
- future production scenes

## 2. Testbed Rule

Testbeds are visual verification surfaces, not feature scenes.

They may contain:

- placeholder meshes
- approved review boards
- material lookdev samples
- controlled lighting rigs
- fixed comparison cameras
- reference prefabs for scale comparison

They must not become:

- gameplay maps
- long-lived feature prototypes
- dumping grounds for unrelated experiments

## 3. Required Testbed Lanes

### `Assets/_Bloodlines/Scenes/Testbeds/VisualReadability/`

Purpose:

- compare unit class recognition at gameplay height
- compare House distinction at identical camera and light settings
- check keep, tower, wall, gate, and siege silhouette separation
- verify banner visibility without clutter drift

Recommended first scene:

- `VisualReadability_Testbed.unity`

Recommended contents:

- one neutral terrain pad
- fixed gameplay-height camera
- one comparison strip for infantry, ranged, cavalry, worker, and command units
- one comparison strip for keep, barracks, market, tower, wall, gate, and siege workshop forms

### `Assets/_Bloodlines/Scenes/Testbeds/TerrainLookdev/`

Purpose:

- compare ground materials at gameplay height
- validate road, ford, shoreline, and cliff readability
- check resource-node ground treatment discipline
- measure prop clutter at tactical density

Recommended first scene:

- `TerrainLookdev_Testbed.unity`

Recommended contents:

- flat terrain plane with modular sectors
- one river edge strip
- one cliff strip
- one road and path strip
- one resource-ground strip for stone, iron, timber, and field use

### `Assets/_Bloodlines/Scenes/Testbeds/MaterialLookdev/`

Purpose:

- compare House trim values
- compare shared stone, wood, iron, leather, and cloth families
- keep weathering, soot, mud, and wear within readability discipline
- prevent value drift between environment and unit surfaces

Recommended first scene:

- `MaterialLookdev_Testbed.unity`

Recommended contents:

- calibrated neutral lighting rig
- turntable or static comparison wall
- material spheres or simple boards
- small building-fragment and shield-fragment references

### `Assets/_Bloodlines/Scenes/Testbeds/IconLegibility/`

Purpose:

- compare icon clarity at command-bar size
- check resource, alert, dynasty, and faith symbol separation
- verify minimap and command marker contrast
- detect dynasty confusion before UI implementation spreads it

Recommended first scene:

- `IconLegibility_Testbed.unity`

Recommended contents:

- fixed UI canvas
- small, medium, and large icon comparison rows
- contrast backgrounds for light, dark, and terrain-toned conditions

## 4. Safe Scene Rules

- keep each testbed self-contained
- do not add gameplay bootstrap dependencies unless the test explicitly requires them
- do not modify production scenes to host visual experiments
- keep scene names stable once created
- put temporary objects under clear parents such as `READABILITY_PROBES` or `TERRAIN_STRIPS`

## 4A. Bootstrap Helper

An additive Unity editor helper now exists at:

- `unity/Assets/_Bloodlines/Code/Editor/GraphicsVisualTestbedBootstrap.cs`

Menu entry:

- `Bloodlines -> Graphics -> Create Visual Testbed Scene Shells`

Batch entry:

- `Bloodlines.EditorTools.GraphicsVisualTestbedBootstrap.RunBatchCreateVisualTestbedSceneShells`

This helper is intended to create scene shells only. It must not overwrite existing testbed scenes.

## 4B. Population Helper

An additive Unity editor helper now also exists at:

- `unity/Assets/_Bloodlines/Code/Editor/GraphicsVisualTestbedPopulate.cs`

Menu entry:

- `Bloodlines -> Graphics -> Populate Visual Testbed Scenes`

Batch entry:

- `Bloodlines.EditorTools.GraphicsVisualTestbedPopulate.RunBatchPopulateVisualTestbedScenes`

CLI wrapper:

- `scripts/Invoke-BloodlinesUnityGraphicsPopulateTestbeds.ps1`

This helper is intended to populate only the tool-owned `GENERATED_TESTBED_CONTENT` root inside each testbed scene so repeat runs do not overwrite unrelated hand-placed work.

Current governed population status:

- the helper has been run successfully in Unity 6.3 batch mode
- all four testbed scenes now contain generated placeholder content
- `VisualReadability_Testbed.unity` now carries a full nine-House comparison grid plus staged House board wall and the Batch 07 support-structure wall
- `TerrainLookdev_Testbed.unity` now carries surface strips, cliff and bridge markers, and staged terrain-board references
- `MaterialLookdev_Testbed.unity` now carries shared and House swatches plus staged banner, heraldry, neutral-settlement, faith-structure, civic-support, material-board, and Batch 08 settlement-variant references
- `IconLegibility_Testbed.unity` now carries staged icon-sheet triplets on controlled light, dark, and terrain-tone backdrops
- Batch 07 and Batch 08 board-wall helper extensions have now been applied successfully into the saved testbed scenes

## 5. Camera Rules

All testbeds should preserve at least one fixed camera representing likely RTS play height.

Recommended checks:

- gameplay-height wide shot
- slightly closer identification shot
- icon or UI close shot only for interface-specific assets

Do not review a unit or building family only from hero-camera distance.

## 6. Lighting Rules

- use one neutral daylight rig as the baseline comparison light
- use a second overcast or dusk rig only as a stress test
- do not judge first-pass readability from extreme cinematic light
- treat fire, emissive, and ward light as restrained secondary passes

## 7. Asset Insertion Rules

Before an asset enters a testbed:

1. it must have a manifest entry
2. it must carry a stage tag
3. it must have a review owner or review note
4. it must stay in staging or be clearly marked as approved runtime art

If an asset is still under visual uncertainty, keep it in staging folders and instantiate it from there.

## 8. Expected Outputs

Each testbed pass should answer concrete questions:

- can this be identified at gameplay height
- can this be confused with another House or structure family
- is the scale truthful against surrounding assets
- do materials collapse into noise
- is the terrain or icon too busy for RTS use

## 9. What This Plan Leaves Untouched

This plan does not:

- alter render pipeline choices
- add packages
- modify importer logic
- create production gameplay scenes
- change canon, House identity, or system rules

It only establishes a safe Unity 6.3 visual verification lane.
