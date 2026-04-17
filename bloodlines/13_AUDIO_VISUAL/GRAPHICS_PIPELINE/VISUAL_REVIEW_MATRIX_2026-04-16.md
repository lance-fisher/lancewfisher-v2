# Visual Review Matrix

Established: 2026-04-16
Scope: additive graphics-lane review expansion for Bloodlines visual assets

## 1. Purpose

This matrix extends the original approval system into a more operational review surface for Unity staging and later runtime integration.

Use it for:

- concept sheets
- placeholder assets
- material boards
- icon sheets
- terrain strips
- building families
- unit families

## 2. Review Outcomes

### `approved`

The direction is accepted at its current review stage.

### `revise`

The asset family is viable but needs targeted changes.

### `replace`

The current pass does not support Bloodlines needs and should be superseded.

### `placeholder_only`

The asset may be used for temporary in-engine checks but not as settled direction.

### `integration_ready`

The asset is approved to leave staging and enter runtime-facing Unity folders.

### `not_integration_ready`

The asset is still restricted to staging, even if it is visually promising.

## 3. Issue Tags

### `style_drift`

The asset drifts away from Bloodlines tone or readability discipline.

### `dynasty_confusion`

The House read is too close to another House or too weak to register.

### `silhouette_issue`

The class or function cannot be read quickly at gameplay height.

### `material_issue`

The surface values, weathering, or texture treatment break the intended lane.

### `scale_issue`

The asset reads at the wrong physical or gameplay-relative size.

## 4. Core Review Questions

Every review should answer:

1. Can the asset be recognized at gameplay height
2. Can its gameplay role be inferred quickly
3. Does it stay inside Bloodlines tone
4. Does it preserve House or universal identity
5. Is it safe to keep in staging, or safe to integrate

## 5. Family-Specific Gates

### Units

Pass if:

- role reads before trim detail
- weapon and shield planes are clear
- House distinction is present but controlled
- elite or command detail does not create silhouette noise

Common failure tags:

- `silhouette_issue`
- `dynasty_confusion`
- `scale_issue`

### Buildings

Pass if:

- purpose reads from roofline, massing, frontage, or defensive form
- entrances and function markers are believable
- damage and upgrade states preserve family recognition
- the building does not collapse into generic medieval clutter

Common failure tags:

- `silhouette_issue`
- `material_issue`
- `style_drift`

### Terrain And Biomes

Pass if:

- roads, river edges, cliffs, and resource seams remain legible at play height
- clutter stays subordinate to units and buildings
- transitions are visible without becoming muddy

Common failure tags:

- `material_issue`
- `style_drift`
- `scale_issue`

### Icons And UI

Pass if:

- 32px and 64px reads are clean
- the icon is distinguishable from adjacent systems
- heraldic detail does not overwhelm the small-size read

Common failure tags:

- `silhouette_issue`
- `dynasty_confusion`
- `style_drift`

## 6. Unity Staging Gate

Before a visual asset is considered `integration_ready`, it must satisfy all of the following:

- manifest entry confirmed
- stage tag confirmed
- review note recorded
- Unity placement test completed in the correct visual testbed
- no unresolved `dynasty_confusion`, `silhouette_issue`, or `scale_issue`

If any of those remain open, the asset is `not_integration_ready`.

## 7. Suggested Review Record Shape

Use this structure in future reports or batch notes:

- asset id
- family
- current stage
- review outcome
- issue tags
- reviewer note
- next action
- Unity testbed used

## 8. Escalation Rule

If an asset family repeatedly fails for the same reason:

- preserve the failed versions
- do not overwrite them in place
- record the repeated failure mode explicitly
- tighten the next prompt or concept brief before generating another pass

## 9. What This Matrix Does Not Do

This matrix does not change:

- House canon
- gameplay stats
- building rosters
- unit rosters
- faith doctrine

It governs visual assessment only.
