# Art Review And Approval System

Every Bloodlines asset or batch review must use these outcome tags.

## Primary Outcome Tags

- `approved`
- `revise`
- `replace`
- `placeholder_only`
- `integration_ready`
- `not_integration_ready`

## Issue Tags

- `style_drift`
- `gameplay_readability_issue`
- `dynasty_confusion_issue`
- `texture_material_issue`
- `scale_issue`
- `silhouette_issue`

## Review Workflow

1. Confirm manifest entry and stage label.
2. Confirm House or universal brief used.
3. Review at gameplay-height scale first.
4. Review mid-distance identity second.
5. Review close material logic third.
6. Apply outcome and issue tags.
7. Record next action:
   - integrate
   - revise same family
   - replace from new brief
   - hold as placeholder

## Review Questions

- Can a player identify the class and side at gameplay height?
- Does the asset fit Bloodlines tone instead of drifting into generic fantasy?
- Does the House read correctly without flooding the asset in color?
- Does the material story make sense?
- Does the asset look structurally credible?
- Does any detail block selection, pathing, or terrain comprehension?
- Is the asset actually ready for Unity staging, or only visually promising?

## Batch Review Rules

- Review units by class and House together.
- Review buildings by settlement tier and function together.
- Review terrain in contiguous biome sets, not isolated tiles.
- Review icons at final in-game size, not only at export size.

## Fast Failure Rule

If an asset fails silhouette, scale, or dynasty recognition, do not spend time polishing texture.
It returns to revision immediately.
