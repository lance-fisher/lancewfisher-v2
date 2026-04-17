# Asset Manifest Schema

The graphics manifests in `MANIFESTS/` are machine-readable planning surfaces for the Bloodlines graphics lane.

## Required Fields

Each manifest entry must include:

- `assetId`
- `assetName`
- `gameplayFunction`
- `dynastyUsage`
- `visualRole`
- `silhouetteNotes`
- `materialNotes`
- `colorNotes`
- `scaleNotes`
- `destructionState`
- `upgradeStates`
- `animationRelevance`
- `iconNeeded`
- `priorityLevel`
- `placeholderStatus`
- `conceptStatus`
- `finalStatus`
- `relatedPrefabCategory`
- `relatedTextureMaterialFamily`
- `generationPromptNotes`
- `unityImportNotes`

## Shared Enums

### `priorityLevel`

- `critical`
- `high`
- `medium`
- `low`

### `placeholderStatus`

- `required_now`
- `planned`
- `optional`
- `not_applicable`

### `conceptStatus`

- `brief_ready`
- `concept_needed`
- `concept_in_progress`
- `direction_approved`
- `not_applicable`

### `finalStatus`

- `not_started`
- `blocked_by_direction`
- `candidate_pending_review`
- `integration_ready`
- `final_approved`

### `iconNeeded`

- `true`
- `false`
- `shared_icon_family`

## Manifest Discipline

- A manifest entry is a production family, not a one-off note.
- Future sessions may extend entries with `variants`, `houseOverrides`, or `faithOverrides`.
- Non-canon gameplay specifics must be flagged in notes instead of being silently assumed.
- If a family is reserved for future canon, its status remains planned and its notes must say so explicitly.
