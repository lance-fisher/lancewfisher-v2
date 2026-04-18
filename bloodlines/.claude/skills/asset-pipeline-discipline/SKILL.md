---
name: asset-pipeline-discipline
description: Invoke when creating new Unity visual assets, generating placeholder meshes or materials, writing a new UnitVisualDefinition ScriptableObject, authoring shader logic, or committing any file under unity/Assets/_Bloodlines/Art/, unity/Assets/_Bloodlines/Shaders/, or unity/Assets/_Bloodlines/Code/Graphics/. Enforces the Zero Hour / Warcraft III fidelity ceiling per governance/OWNER_DIRECTION_2026-04-17_FIDELITY_AND_STRATEGY_DEPTH.md. Trigger phrases: "create asset", "add mesh", "new material", "generate placeholder", "visual definition", "add shader", "art commit", "texture", "new unit model".
---

# Asset Pipeline Discipline Skill (Bloodlines)

The fidelity ceiling for Bloodlines is Command and Conquer Generals Zero Hour (2003) and Warcraft III (2002). This skill enforces that ceiling at asset-creation and commit time.

**Authority:** `governance/OWNER_DIRECTION_2026-04-17_FIDELITY_AND_STRATEGY_DEPTH.md`. Read the relevant sections before making any judgment calls that go beyond the checklist below.

## Polygon budget check

| Asset type | Budget (triangles) | Hard cap |
|---|---|---|
| Infantry unit | 1,000 — 3,000 | 4,000 |
| Mounted unit | 2,000 — 4,000 | 5,000 |
| Vehicle / siege | 2,000 — 5,000 | 6,000 |
| Hero unit | 3,000 — 6,000 | 8,000 |
| Small building | 2,000 — 5,000 | 7,000 |
| Large building / fortification | 5,000 — 10,000 | 15,000 |
| Terrain tile / prop | 200 — 1,000 | 2,000 |

If a mesh exceeds the hard cap: reject. If it exceeds the target range but is under the hard cap: flag and ask whether the additional geometry is visible at RTS zoom (camera height 15-30 units above ground). If not visible at RTS zoom, reduce.

RTS zoom legibility is the only metric. Silhouette distinction at 30-unit camera height is more valuable than polygon detail.

## Texture budget check

| Texture type | Allowed resolution | Notes |
|---|---|---|
| Unit diffuse | 256x256 or 512x512 | Hand-painted style preferred |
| Hero / named unit diffuse | Up to 1024x1024 | Only if canonical |
| Building diffuse | 512x512 or 1024x1024 | One atlas per building set |
| Terrain splat map | 512x512 | One per terrain tile set |
| UI / portrait | 128x128 to 256x256 | Per-unit portrait |

**Texture types that are NOT allowed:**
- Normal maps (unless a specific canonical unit's art authority document explicitly requires one)
- Roughness maps
- Metalness / metallic maps
- Height / displacement maps
- AO (ambient occlusion) maps
- Emissive maps (replace with a simple additive color in the shader if needed for fire/glow)
- Opacity maps beyond a simple alpha channel on the diffuse

If a texture type not in the "allowed" list is being added: reject and provide the allowed alternative.

## Shader check

Allowed shader operations:
- Diffuse texture sample
- Simple specular (Blinn-Phong, single float intensity)
- Faction tint via per-instance `_FactionColor` property
- Simple rim light for silhouette legibility (single directional, single intensity)
- Alpha cutout for leaves, flags, banners
- Selection highlight / outline (simple color overlay)
- Basic additive blend for fire, magic, VFX particles

Not allowed in any Bloodlines shader:
- PBR workflows: `_Metallic`, `_Smoothness`, `_OcclusionMap`, `_BumpMap` (normal map)
- Subsurface scattering
- Anisotropic highlights
- Parallax occlusion / height-mapped displacement
- Screen-space effects applied per-object
- Shader Graph networks deeper than 3 additive nodes beyond what the faction-tint shader already defines
- HDRP material graphs

If a shader is found that uses any forbidden feature: reject, strip the feature, replace with the nearest allowed equivalent.

## Naming convention check

```
Assets/_Bloodlines/Art/Units/<FactionId>/<UnitType>/<AssetName>.<ext>
Assets/_Bloodlines/Art/Buildings/<FactionId>/<BuildingType>/<AssetName>.<ext>
Assets/_Bloodlines/Art/Terrain/<BiomeId>/<AssetName>.<ext>
Assets/_Bloodlines/Art/VFX/<CategoryId>/<AssetName>.<ext>
Assets/_Bloodlines/Art/UI/Portraits/<FactionId>/<UnitType>_portrait.<ext>
```

`<FactionId>` must match the `FactionId` field in the corresponding `FactionComponent` definition. Do not invent faction IDs.

## .meta file check

Every Unity asset requires a `.meta` file with a stable GUID. If creating files outside the Unity Editor:

1. Generate a GUID: `python -c "import uuid; print(str(uuid.uuid4()).replace('-',''))"` (32-char hex).
2. Write a minimal `.meta` alongside the asset:

```yaml
fileFormatVersion: 2
guid: <32-char-hex-guid>
NativeFormatImporter:
  externalObjects: {}
  mainObjectFileID: 0
  userData:
  assetBundleName:
  assetBundleAssetName:
```

Use `DefaultImporter` for `.fbx`, `TextureImporter` for image files (with appropriate `textureType: Sprite` or `textureType: Default`), `ModelImporter` for meshes. If in doubt, use `NativeFormatImporter`.

Never commit an asset without a `.meta`. Unity will generate a new GUID if one is missing, which will break all references to that asset in scenes and prefabs.

## UnitVisualDefinition pairing check

Every unit type that has a `UnitTypeComponent` entry in the ECS simulation must have a paired `UnitVisualDefinition` ScriptableObject under `Assets/_Bloodlines/Data/VisualDefinitions/Units/`. The visual definition must reference:
- At least one mesh asset at the correct path
- At least one material asset at the correct path
- A `FactionId` field matching the unit's faction

If a unit ECS type exists without a paired visual definition: flag as an art-gap item. It does not block the slice, but it must appear in the slice handoff's "Art Gaps" section.

## Art authority rule

Final faction-identity art (hero unit models, faction-defining buildings, canonical landmark structures) is human-owned. AI may generate:
- Geometric placeholder meshes (unit silhouette capsules, building boxes)
- Texture color fills (flat faction-color diffuse for placeholders)
- `.meta` files for generated assets
- `UnitVisualDefinition` ScriptableObjects pointing at existing or placeholder assets

AI may NOT make final visual identity decisions for named characters, named buildings, or faction emblems. If the task requires a final visual identity decision, flag it as "art authority — human review required" and use a placeholder.

## Report format

- `ASSET PASS`: "All assets within fidelity ceiling. Budget compliance: [units X tri, buildings Y tri]. Texture types: diffuse only. Shaders: [tint + specular]. .meta files: present. Naming: compliant."
- `ASSET VIOLATION`: "File: [path]. Issue: [polygon over budget / forbidden texture type / missing .meta / bad naming]. Required change: [specific action]."
- `ART AUTHORITY FLAG`: "Asset [name] requires human visual identity decision. Placeholder created at [path]. Flag for artist review."

## What this skill does NOT do

- Does not validate gameplay systems (canon-enforcement, balance-and-counterplay).
- Does not validate ECS code (unity-ecs-discipline).
- Does not generate final art. Placeholder geometry only.
- Does not make decisions about named character or faction-emblem visual design.
