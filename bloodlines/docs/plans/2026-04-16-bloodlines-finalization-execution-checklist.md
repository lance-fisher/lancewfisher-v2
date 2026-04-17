# 2026-04-16 Bloodlines Finalization Execution Checklist

## Purpose

This is the active execution checklist for pushing Bloodlines from the current preserved multi-lane state toward a genuinely finished shipping posture.

It does not replace canon, handoffs, or subsystem reports.
It exists to give the next session an execution-grade ladder:

1. what must be verified first
2. what must be expanded next
3. what still separates the project from an honestly final version

## Current Reality

- The continuation platform is operational and now strong enough to carry continuity session by session through `Command Deck` plus the governed `Execution` view.
- The browser runtime is frozen as behavioral specification only.
- Unity is the shipping engine and the only active implementation lane.
- Unity currently has a real first shell, but not a finished game:
  - movement
  - camera
  - selection
  - drag-box
  - control groups
  - formation-aware move
  - control-point capture and trickle
  - queue issue / cancel / refund
  - worker-led construction
  - constructed `barracks -> militia` continuity
  - construction and production progress observability

## Finalization Order

### 1. Verify The Existing First Shell In Real Play Mode

This is the first hard gate because the current shell is governed-runtime green but still not fully human-verified in-editor.

Required checks:

- `Bootstrap.unity` opens cleanly in Unity 6.3 LTS `6000.3.13f1`
- unit selection feels correct
- building selection feels correct
- drag-box selection feels correct
- `1` select-all works
- `Ctrl+2-5` save and `2-5` recall work
- `F` frame works
- formation-aware move feels legible
- camera pan / zoom / rotate / framing feel acceptable
- control-point capture, contested decay, stabilization, and uncontested trickle behave correctly
- `command_hall -> villager` two-deep queue issue / rear cancel / refund / surviving front completion all feel correct
- worker construction panel, pending placement, obstruction rejection, `dwelling` placement, completion timing, and population-cap increase all feel correct
- `barracks` completion, post-completion selection, `militia` queue visibility, training completion, and controlled-unit growth all feel correct

Exit condition:

- a dated Unity handoff records real Play Mode results, not only batch validation

### 2. Deepen The Battlefield Runtime Beyond The First Shell

Once the first shell is manually verified, the next work must stay in Unity and continue real gameplay depth.

Priority order:

1. broader construction roster
2. deeper build-placement UX
3. broader production roster
4. production from additional newly completed buildings
5. richer economy, logistics, and realm-state runtime UI
6. real combat lane
7. attack-move and battlefield abilities only after combat exists

Exit condition:

- at least one additional real Unity gameplay slice lands beyond the current first-shell seam

### 3. Migrate More Of What Makes Bloodlines Distinct Into The Ship Lane

Bloodlines is not "done" when the battlefield shell merely feels like a generic RTS.

The Unity ship lane still needs additive realization of:

- House identity and House-specific roster depth
- bloodline command presence
- dynasty consequence visibility
- faith and conviction realization
- diplomacy and operations follow-up
- world-pressure and late-stage consequence structure
- stronger AI reaction loops

Exit condition:

- Unity carries materially more of the canon differentiators rather than only tactical scaffolding

### 4. Convert Staging Into Production Assets And Production UX

The project is not final while visual and audio lanes remain mostly concept-stage.

Still required:

- formal review outcomes for graphics batches 01 through 08
- approved production directions from those reviews
- runtime-ready units, buildings, terrain, icons, portraits, materials, and prefabs
- Wwise integration
- music, SFX, ambience, and interface sound
- stronger onboarding, HUD, menus, and runtime legibility

Exit condition:

- staging work begins turning into runtime-ready content instead of remaining mostly review material

### 5. Finish Shipping Readiness

Still required before honest finalization:

- structured playtest loops
- performance profiling and budgets
- balance iteration
- save/resume posture for the ship lane
- release packaging discipline
- multiplayer architecture and implementation if it remains in scope

Exit condition:

- the project can be described as production-ready rather than continuity-safe and technically advancing

## Immediate Next Session Definition Of Success

The next session should not stop after producing another report.

Minimum acceptable success:

1. use the continuation platform and confirm the live anchor plus execution packet
2. manually verify the current Unity first shell in Play Mode as far as the environment allows
3. if that verification is successful, land at least one additional real Unity gameplay improvement
4. update continuity so the next session resumes from the new real state

## Anti-Regression Rules

- Do not reopen browser feature growth.
- Do not replace Unity with another runtime.
- Do not reframe the project into MVP scope.
- Do not treat platform continuity work as a substitute for gameplay progress now that the continuation platform is already strong enough.
- Do not claim finalization until Unity gameplay breadth, content, UX, audio, and QA maturity have materially advanced beyond the current shell.
