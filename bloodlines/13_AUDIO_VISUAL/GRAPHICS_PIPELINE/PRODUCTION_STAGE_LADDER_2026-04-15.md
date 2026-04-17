# Production Stage Ladder

Bloodlines graphics production uses the following non-fake stage ladder:

1. `placeholder`
2. `first_pass_concept`
3. `approved_direction`
4. `production_candidate`
5. `in_engine_test_asset`
6. `refinement_candidate`
7. `near_final`
8. `final`

## Stage Rules

### 1. Placeholder

- Purpose: prove function, scale, and readability
- Allowed quality: crude but honest
- Must include: manifest link, correct footprint or silhouette class, stage label

### 2. First Pass Concept

- Purpose: establish visual direction options
- Must include: prompt source, variant labels, review tags
- Not integration-ready

### 3. Approved Direction

- Purpose: direction locked for the family
- Must include: approval notes, House or universal lane confirmation, negative drift notes

### 4. Production Candidate

- Purpose: first serious execution against approved direction
- Must include: material family notes, destruction-state notes where relevant, Unity import notes

### 5. In-Engine Test Asset

- Purpose: gameplay-height verification in actual runtime conditions
- Must include: scale verification, readability check, icon link if applicable

### 6. Refinement Candidate

- Purpose: correct issues found in engine or review
- Must include: change list against prior candidate

### 7. Near Final

- Purpose: stable asset awaiting final polish, optimization, and package signoff
- Must include: final review checklist pass except ship packaging

### 8. Final

- Purpose: approved runtime asset
- Must include: integration-ready status, prefab path, material path, manifest final status update

## Advancement Gates

An asset may advance only when:

- the previous stage exists
- review tags are attached
- the manifest entry is updated
- the file naming convention is correct
- the asset remains in the correct staging lane

No asset becomes `final` directly from concept art.
