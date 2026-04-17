# Codex Next Prompt: Unity Attack-Move and Explicit Attack Orders

Paste the following to Codex to start its next lane after the projectile
combat slice completes. Concurrent session contract:
`docs/unity/CONCURRENT_SESSION_CONTRACT.md`. Wrapper lock helper:
`scripts/Invoke-BloodlinesUnityWrapperWithLock.ps1`.

---

```
You are continuing Bloodlines Unity combat-lane work at
D:\ProjectsHome\Bloodlines. This is a new concurrent slice following
your projectile combat work. Claude is running an AI / economy polish
lane in parallel; the two must not collide. Read
docs/unity/CONCURRENT_SESSION_CONTRACT.md and
docs/unity/MERGE_PLAN_2026-04-17.md before starting.

ORIENT, DO NOT SUMMARIZE
- Read HANDOFF.md, NEXT_SESSION_HANDOFF.md (latest entry),
  continuity/PROJECT_STATE.json, and your own prior handoffs
  (combat foundation + projectile combat) under
  docs/unity/session-handoffs/.
- Read docs/unity/CONCURRENT_SESSION_CONTRACT.md for lane ownership,
  wrapper lock protocol, merge discipline.
- Read docs/unity/MERGE_PLAN_2026-04-17.md for the state of the two
  concurrent branches.

YOUR LANE: EXPLICIT ATTACK ORDERS + ATTACK-MOVE
Combat is alive (auto-acquire + projectile delivery) but the player
cannot command it yet. Two canonical commands must exist:

1. Attack order: right-click-on-hostile issues an attack that targets
   that specific hostile unit. Units chase while in sight, attack
   while in range, stop pursuing when out of sight radius.
2. Attack-move: a move command where the unit automatically engages
   any hostile it detects inside its acquire radius along the way.

REQUIRED DELIVERABLE FOR FIRST SLICE
1. Components:
   - unity/Assets/_Bloodlines/Code/Combat/AttackOrderComponent.cs
     ExplicitTargetEntity (nullable), IsAttackMoveDestination (bool),
     AttackMoveDestination (float3), IsActive (bool)
2. Systems:
   - unity/Assets/_Bloodlines/Code/Combat/AttackOrderSystem.cs
     handles conversion from AttackOrderComponent into the existing
     MoveCommandComponent + AttackTargetComponent flow. Runs
     before AutoAcquireTargetSystem.
   - Extend AutoAcquireTargetSystem: if a unit has an active
     AttackOrderComponent with an explicit target and that target
     is alive + in sight, prefer it over auto-acquired targets.
3. Debug input bindings on BloodlinesDebugCommandSurface:
   - right-click-on-hostile-unit while selection contains combat
     units issues an attack order on that target
   - 'A' keydown enters attack-move cursor mode; next right-click
     becomes an attack-move to that ground position for selected
     combat units. Esc cancels.
   - Add these handlers in a NEW partial class file
     BloodlinesDebugCommandSurface.AttackOrders.cs to stay out of
     the main command surface and Claude's AI partial class.
4. Extend BloodlinesCombatSmokeValidation with a third phase:
   - spawn a selectable player militia and a hostile enemy
   - issue an attack order via the new debug API
   - assert the attacker's CombatStatsComponent cooldown fires on
     the explicit target, target dies, attacker has no residual
     AttackTargetComponent
5. Per-slice handoff at
   docs/unity/session-handoffs/<YYYY-MM-DD>-unity-attack-orders-and-attack-move.md

FILE-SCOPE CONTRACT
You own exclusively:
  unity/Assets/_Bloodlines/Code/Combat/**
  unity/Assets/_Bloodlines/Code/Editor/BloodlinesCombatSmokeValidation.cs
  scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1
  unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.AttackOrders.cs (new partial file)
  docs/unity/session-handoffs/<date>-unity-attack-*.md

You may MODIFY but share narrowly with Claude:
  unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.cs
    (only if you need to add the `partial` keyword, already present)
  unity/Assembly-CSharp.csproj
  unity/Assembly-CSharp-Editor.csproj

You MUST NOT touch:
  unity/Assets/_Bloodlines/Code/AI/**                  (Claude lane)
  unity/Assets/_Bloodlines/Code/Economy/**             (Claude lane)
  unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.AI.cs
  unity/Assets/_Bloodlines/Code/Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs
    (Claude owns that file; combat proofs live in your dedicated
     combat smoke validator)
  CURRENT_PROJECT_STATE.md, NEXT_SESSION_HANDOFF.md,
  continuity/PROJECT_STATE.json
    (only update at the very end of your slice, after rebasing; if
     Claude has appended Sessions 120-124 already, add your session
     at Session 125 and DO NOT overwrite Claude's entries)

BRANCH AND COMMIT DISCIPLINE
- Create and stay on branch `codex/unity-attack-orders-attack-move`.
- Base from current master.
- Rebase onto origin/master before push.
- Commit frequently in small, logical slices.
- Do NOT push to master directly. Push to your branch.

UNITY WRAPPER LOCK
Use:
  powershell -ExecutionPolicy Bypass -File \
    scripts/Invoke-BloodlinesUnityWrapperWithLock.ps1 \
    -Session "codex" \
    -WrapperScript "scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1"
The lock enforces `.unity-wrapper-lock` so you never collide with Claude.

VALIDATION GATE
All six must be green before writing the handoff:
  1. dotnet build unity/Assembly-CSharp.csproj -nologo
  2. dotnet build unity/Assembly-CSharp-Editor.csproj -nologo
  3. scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1 (all phases)
  4. scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1
     (still green, must not break economy / AI / HUD work)
  5. scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1
  6. node tests/data-validation.mjs
  7. node tests/runtime-bridge.mjs

SCOPE DISCIPLINE
This slice is player-commandable combat. Out of scope:
- formation combat, stance changes (aggressive/defensive/hold)
- hotkey UX polish beyond 'A' + right-click
- death visuals beyond existing presentation bridge
- renown / conviction kill hooks
If you finish with session budget remaining, stop and wait for merge
coordination rather than expanding scope.

NON-NEGOTIABLE RULES (unchanged)
- No MVP framing. No phased release. Full canonical depth.
- Canon: 18_EXPORTS/BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.4.md.
- Browser src/game/*.js is frozen behavioral spec; do not extend.
- Do not commit secrets or .env files.
- Do not commit to master; push to your branch.

REPORT OUT WHEN DONE
- Branch name + HEAD SHA + commit list
- Path to per-slice handoff
- Tail of artifacts/unity-combat-smoke.log showing all phases pass
- Confirmation the bootstrap runtime smoke still ends with
  Claude's Session 120-124 proofs green:
    aiActivityObserved=True
    aiConstructionObserved=True
    stabilitySurplusObserved=True
    aiMilitaryOrdersIssued >= 1
  and all Session 115-118 economy proofs.
```
