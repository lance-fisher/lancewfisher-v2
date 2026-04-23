# 2026-04-23 Unity Player Succession Influence

## Slice Summary

- Lane: `player-diplomacy`
- Branch: `codex/unity-player-succession-influence`
- Browser references / search terms used:
  - `PlayerSuccessionPreference`
  - `appointHeir`
  - `getSuccessionPreferenceState`
  - `preferredHeirId`
  - `successionOverride`
  - `applySuccessionRipple`
  - `backfillHeir`

## What Landed

- Added `SuccessionPreferenceComponent`,
  `PlayerSuccessionPreferenceRequestComponent`, and
  `SuccessionPreferenceResolutionSystem` under
  `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/` so kingdom faction roots can
  store a paid preferred-heir designation, consume a one-shot player request,
  deduct succession-planning costs, and prune stale or expired designations.
- Extended `DynastySuccessionSystem` with a narrow additive override: when the
  ruling head falls, the existing succession seam now checks for a live,
  eligible preferred heir on the faction root before falling back to the
  default ordered active-member chain, then consumes the preference after use.
- Extended
  `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.PlayerDiplomacy.cs`
  with `TryDebugSetSuccessionPreference(...)` and
  `TryDebugGetSuccessionPreferenceState(...)` so the seam is inspectable and
  queueable from the existing player-diplomacy debug surface.
- Added `BloodlinesPlayerSuccessionInfluenceSmokeValidation` plus
  `scripts/Invoke-BloodlinesUnityPlayerSuccessionInfluenceSmokeValidation.ps1`
  to prove:
  - designation deducts the expected gold and legitimacy costs and writes the
    faction-root preference component
  - a valid preferred heir overrides default succession order on ruler death
  - an invalid preferred heir is discarded and succession falls back to the
    default heir-designate path
- `unity/Assembly-CSharp.csproj` and `unity/Assembly-CSharp-Editor.csproj` now
  explicitly include the new runtime and editor files for this slice.

## Validation

- Dedicated player succession influence smoke: PASS
  - `Player succession influence smoke validation passed.`
- Runtime build: PASS
  - `Build succeeded.`
- Editor build: PASS
  - `Build succeeded.` (existing repo-wide warnings only)
- Governed full validation chain: PASS
  - `Bootstrap runtime smoke validation passed.`
  - `Combat smoke validation passed.`
  - `Bloodlines canonical Unity scene-shell validation sequence completed successfully.`
  - `Fortification smoke validation passed.`
  - `Siege smoke validation passed.`
  - `Bloodlines data validation passed.`
  - `Bloodlines runtime bridge validation passed.`
  - `STALENESS CHECK PASSED: Contract revision=117, last-updated=2026-04-23 is current.`

## Notes

- No direct browser-side `PlayerSuccessionPreference` helper exists in
  `src/game/core/simulation.js`; this slice therefore implements the action from
  the active 2026-04-24 Codex directive using the already-landed succession
  cascade (`applySuccessionRipple` / `backfillHeir`) as the behavioral anchor.
- The current implementation makes two explicit canon-adjacent assumptions in
  that absence:
  - preferred-heir designation costs `50` gold and `4` legitimacy
  - a designation naturally expires after `365` in-world days if unused
- This worktree required a local `unity/Library` junction back to the canonical
  `D:\ProjectsHome\Bloodlines\unity\Library` surface so the governed `.csproj`
  build gates could resolve `Library\ScriptAssemblies`.

## Exact Next Action

1. Claim Priority 21 `siege-escalation-arc` from
   `D:\ProjectsHome\Bloodlines\03_PROMPTS\CODEX_MULTI_DAY_DIRECTIVE_2026-04-24.md`.
2. Open `codex/unity-siege-escalation-arc` from refreshed canonical `master`
   after this slice lands.
3. Port the prolonged-siege escalation state, dedicated smoke validator, and
   rerun the governed validation chain.
