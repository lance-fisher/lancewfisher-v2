# Bloodlines Unity Migration Plan

Author: Claude Code continuation package, 2026-04-13

This file mirrors and extends the Unity migration blueprint discovered in the canonical `deploy/bloodlines/` tree. It is the tracked continuation point for Codex and Claude sessions.

## 1. Baseline

Production direction:
- Unity 6 LTS with DOTS / Entities.

Reference simulation:
- `deploy/bloodlines/play.html`
- `deploy/bloodlines/src/game/`
- `deploy/bloodlines/data/`

Non-negotiable constraints:
- No scope reduction.
- Browser prototype remains the executable spec until Unity reaches parity.
- JSON gameplay data remains the balance source of truth.
- Ironmark remains the only settled prototype-playable house until a new house is explicitly advanced.
- Canonical working root remains `deploy/bloodlines/`.

## 2. Current Observed State

Recovered from the 2026-04-13 Bloodlines handoff:
- Browser prototype is playable and validated.
- Dynasty and Faith panels are live in the browser build.
- State analysis and web handoff were completed.
- Unity direction is documented, but Unity execution has not started.
- Local machine has Unity installed at `C:\Program Files\Unity\Hub\Editor\6000.4.2f1`.
- `deploy/bloodlines/unity/` does not exist yet.

## 3. Immediate Blockers

The previous session identified two blockers before U0 execution:

1. Git strategy for `deploy/bloodlines/unity/`.
2. Actual Unity project bootstrap.

Recommended default for early phases:
- Use a nested git repo inside `deploy/bloodlines/unity/` during U0 through U7.

Reason:
- `deploy/` is ignored by the portfolio repo.
- Unity asset churn should not pollute the portfolio history.
- Browser prototype and design archive can remain stable while the Unity repo iterates independently.

## 4. Engine and Package Baseline

Unity editor target:
- Unity 6 LTS, locally available editor: `6000.4.2f1`

Core packages to install during U0:
- `com.unity.entities`
- `com.unity.entities.graphics`
- `com.unity.burst`
- `com.unity.collections`
- `com.unity.mathematics`
- `com.unity.inputsystem`
- `com.unity.render-pipelines.universal`
- `com.unity.addressables`

Deferred package:
- `com.unity.netcode`

Editor targets:
- Linear color space
- New Input System only
- URP baseline
- Mono for iteration, IL2CPP for release

## 5. Project Location

Target project path:
- `deploy/bloodlines/unity/`

Intended Unity content root:
- `Assets/_Bloodlines/`

Top-level Unity layout:

```text
unity/
  Assets/
    _Bloodlines/
      Art/
      Audio/
      Code/
      Data/
      Prefabs/
      Scenes/
      Materials/
      Shaders/
      Animation/
      Docs/
  Packages/
  ProjectSettings/
  .gitignore
  .gitattributes
  README.md
```

## 6. Porting Principle

Unity is not a redesign pass.

Port in this order:
1. Match browser behavior.
2. Match browser data.
3. Match browser AI cadence.
4. Only then add capabilities the browser runtime could not carry cleanly.

That means the first true Unity-only unlock is pathing quality, not a rewrite of match structure or canon.

## 7. Milestone Sequence

U0 through U16 are defined in `PHASE_PLAN.md`.

Practical breakpoints:
- U0 to U3: project and control shell
- U4 to U8: browser parity
- U9: pathing milestone that justifies the engine move
- U10 onward: new Bloodlines depth layers that were blocked in browser runtime

## 8. Required Next Actions

1. Create `deploy/bloodlines/unity/` using the installed Unity 6 editor.
2. Initialize nested git repo and LFS rules inside that folder.
3. Commit initial project baseline.
4. Implement JSON importer before hand-authoring gameplay ScriptableObjects.
5. Keep every Unity-work session documented in `docs/unity/session-handoffs/`.

## 9. What Must Not Happen

- Do not move or deprecate the browser prototype.
- Do not author balance data only in Unity.
- Do not jump to multiplayer before parity and pathing.
- Do not invent new house identities for the eight reset houses.
- Do not treat missing systems as cut scope.
