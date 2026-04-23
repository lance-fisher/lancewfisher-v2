# 2026-04-23 Unity Player Captive Ransom Trickle

## Slice Summary

- Lane: `player-diplomacy`
- Branch: `codex/unity-player-captive-ransom-trickle`
- Browser references / search terms used:
  - `updateCaptiveRansomTrickle`
  - `CAPTIVE_INFLUENCE_TRICKLE`
  - `CAPTIVE_RENOWN_WEIGHT`
  - `getCapturedMemberRansomTerms`
  - `startCaptiveRansomOperation`

## What Landed

- Added `CaptiveRansomTrickleComponent` and `CaptiveRansomTrickleSystem` under
  `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/` so kingdom faction roots now
  cache held-captive count, highest captive renown, current influence /
  dynasty-renown rates, and last-applied deltas, then pay the browser-aligned
  passive captive trickle on whole in-world day boundaries using
  `DualClockComponent.DaysPerRealSecond`.
- The runtime resolves captive renown from the origin faction dynasty roster
  and applies two additive outputs to the captor faction root:
  - influence using the browser `CAPTIVE_INFLUENCE_TRICKLE +
    captiveRenown * CAPTIVE_RENOWN_WEIGHT` seam
  - additive Unity-side dynasty renown drift using the same renown-weighted
    signal so notable captives surface in the already-landed prestige ledger
- The runtime now prefers a faction-root captive ledger when one exists and
  otherwise falls back to the currently-buffered captive-holder entity with the
  same `FactionId`. This preserves compatibility with current capture helper
  behavior without editing the AI-owned `Code/AI/**` lane.
- Added `TryDebugGetCaptiveTrickle(...)` to
  `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.PlayerDiplomacy.cs`
  so the seam is inspectable from the existing player-diplomacy debug surface.
- Added `BloodlinesPlayerCaptiveRansomTrickleSmokeValidation` plus
  `scripts/Invoke-BloodlinesUnityPlayerCaptiveRansomTrickleSmokeValidation.ps1`
  to prove:
  - a higher-renown captive yields a larger influence and dynasty-renown delta
    than a low-renown captive
  - factions with no held captives receive zero passive trickle
- `unity/Assembly-CSharp.csproj` and `unity/Assembly-CSharp-Editor.csproj` now
  explicitly include the new runtime and editor files for this slice.

## Validation

- Dedicated captive ransom trickle smoke: PASS
  - `Player captive ransom trickle smoke validation passed.`
- Runtime build: PASS
  - `Build succeeded.`
- Editor build: PASS
  - `Build succeeded.`
- Governed full validation chain: PASS
  - `Bootstrap runtime smoke validation passed.`
  - `Combat smoke validation passed.`
  - `Bloodlines canonical Unity scene-shell validation sequence completed successfully.`
  - `Fortification smoke validation passed.`
  - `Siege smoke validation passed.`
  - `Bloodlines data validation passed.`
  - `Bloodlines runtime bridge validation passed.`
  - `STALENESS CHECK PASSED: Contract revision=101, last-updated=2026-04-23 is current.`

## Notes

- `D:\ProjectsHome\Bloodlines\03_PROMPTS\CODEX_MULTI_DAY_DIRECTIVE_2026-04-23.md`
  was absent in the canonical root during this slice. The active priority stack
  came from
  `D:\ProjectsHome\Bloodlines\03_PROMPTS\CODEX_MULTI_DAY_DIRECTIVE_2026-04-24.md`,
  which explicitly supersedes the missing 2026-04-23 directive.
- This worktree required a local `unity/Library` junction back to the canonical
  `D:\ProjectsHome\Bloodlines\unity\Library` surface so the governed `.csproj`
  build gates could resolve `Library\ScriptAssemblies`.
- The prior local library directory was preserved as
  `unity/Library.pre-junction-20260423-player-captive-ransom-trickle` and must
  remain unstaged.

## Exact Next Action

1. Claim Priority 11 `covert-ops-resolution-effects` from
   `D:\ProjectsHome\Bloodlines\03_PROMPTS\CODEX_MULTI_DAY_DIRECTIVE_2026-04-24.md`.
2. Open `codex/unity-covert-ops-resolution-effects` from updated `master`.
3. Port assassination / sabotage / espionage resolution effects plus a
   dedicated smoke validator and rerun the governed 10-gate chain.
