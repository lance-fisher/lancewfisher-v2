# Unity Player Succession Influence Landing

- Date: 2026-04-23
- Lane: `player-marriage-diplomacy`
- Branch: `codex/unity-player-succession-influence`
- Status: merged to canonical `master` and revalidated

## Goal

Land the validated player succession influence slice onto canonical `master`,
rerun the governed validation chain on the merged result, and leave the next
Codex pickup pointed at Priority 21 `codex/unity-siege-escalation-arc`.

## What Landed On Master

- `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/SuccessionPreferenceComponent.cs`,
  `PlayerSuccessionPreferenceRequestComponent.cs`, and
  `SuccessionPreferenceResolutionSystem.cs` now live on canonical `master`,
  so the player kingdom can pay 50 gold and 4 legitimacy to designate a
  preferred eligible heir for up to 365 in-world days before the existing
  succession cascade executes.
- `unity/Assets/_Bloodlines/Code/Dynasties/DynastySuccessionSystem.cs` now
  consumes a live preferred-heir designation before falling back to the
  default succession chain and clears stale or spent preferences without
  widening the broader dynasty lane.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.PlayerDiplomacy.cs`
  now exposes `TryDebugSetSuccessionPreference(...)` and
  `TryDebugGetSuccessionPreferenceState(...)` on the merged line.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerSuccessionInfluenceSmokeValidation.cs`
  plus
  `scripts/Invoke-BloodlinesUnityPlayerSuccessionInfluenceSmokeValidation.ps1`
  now live on canonical `master` as the dedicated proof surface.
- `unity/Assembly-CSharp.csproj` and `unity/Assembly-CSharp-Editor.csproj`
  retain the additive compile includes for the new runtime/editor files and
  the canonical
  `D:\ProjectsHome\Bloodlines\unity\Library\PackageCache` analyzer roots
  after the detached landing validation pass.

## Validation Proof On Merge Result

- Runtime build:
  - `Build succeeded.`
- Editor build:
  - `Build succeeded.` with existing repo-wide warnings only
- Bootstrap runtime:
  - `Bootstrap runtime smoke validation passed.`
- Combat smoke:
  - `Combat smoke validation passed.`
- Scene shells:
  - `Bootstrap scene shell validation passed.`
  - `Gameplay scene shell validation passed.`
  - `Bloodlines canonical Unity scene-shell validation sequence completed successfully.`
- Fortification smoke:
  - `Fortification smoke validation passed.`
- Siege smoke:
  - `Siege smoke validation passed.`
- Data validation:
  - `Bloodlines data validation passed.`
- Runtime bridge:
  - `Bloodlines runtime bridge validation passed.`
- Dedicated smoke on merge result:
  - `Player succession influence smoke validation passed.`

## Exact Next Action

1. Start the next fresh `codex/unity-siege-escalation-arc` branch from the
   updated canonical `master`.
2. Read the siege escalation seam in `src/game/core/simulation.js` and the
   current Unity `Siege/**` surfaces.
3. Port the next additive non-AI slice with its own dedicated smoke
   validator and matching PowerShell wrapper.
