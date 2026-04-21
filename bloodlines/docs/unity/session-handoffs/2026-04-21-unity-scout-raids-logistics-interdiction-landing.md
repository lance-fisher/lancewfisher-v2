# 2026-04-21 Unity Scout Raids And Logistics Interdiction Landing

## Goal

Land the rebased `codex/unity-scout-raids-logistics-interdiction` branch onto
`master`, rerun the governed validation gate on the merged master commit, and
update continuity so the next session resumes from the next unclaimed Codex
lane instead of re-merging the scout slice.

## Merge State

- Merge commit on `master`: `dda7c25e`
- Merged branch: `codex/unity-scout-raids-logistics-interdiction`
- Contract revision advanced: `50 -> 51`

## Browser References

- `src/game/core/simulation.js`
  - `SCOUT_RAID_TARGET_RANGE` (35)
  - `SCOUT_RAID_RETREAT_DISTANCE` (36)
  - `SCOUT_RAID_LOYALTY_RADIUS` (37)
  - `isBuildingUnderScoutRaid` (2046)
  - `getRaidRetreatCommand` (2349)
  - `executeScoutRaid` (2362)
  - `executeScoutLogisticsInterdiction` (2515)

## Work Completed

- Merged the rebased scout-raids lane to `master` with `git merge --no-ff`.
- Verified that the merged master content matches the scout branch content for
  the Bloodlines runtime, validation wrappers, and lane docs.
- Re-ran the full governed validation gate against detached `master`
  (`dda7c25e`) in `D:\BLAICD\bloodlines`, which already carried the Unity
  generated project metadata needed for truthful `dotnet build` and batch-mode
  validation.
- Updated continuity surfaces and the concurrent-session contract so the scout
  lane is now represented as landed-on-master with no branch currently in
  flight.

## Validation Proof

1. `dotnet build unity/Assembly-CSharp.csproj -nologo`
   - `Build succeeded.`
   - `0 Warning(s)`
   - `0 Error(s)`
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
   - `Build succeeded.`
   - `113 Warning(s)`
   - `0 Error(s)`
3. Bootstrap runtime smoke
   - `Bootstrap runtime smoke validation passed.`
4. Combat smoke
   - `Combat smoke validation passed: meleePhase=True, projectilePhase=True, explicitAttackPhase=True, attackMovePhase=True, targetVisibilityPhase=True, groupMovementPhase=True, separationPhase=True, stancePhase=True.`
5. Scene shells
   - `Bootstrap scene shell validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity with canonical map Assets/_Bloodlines/Data/MapDefinitions/ironmark_frontier.asset.`
   - `Gameplay scene shell validation passed for Assets/_Bloodlines/Scenes/Gameplay/IronmarkFrontier.unity.`
6. Fortification smoke
   - `Fortification smoke validation passed: baselinePhase=True, tierAdvancePhase=True, reserveMusterPhase=True, reserveRecoveryPhase=True. Baseline: tier=0, ceiling=3. TierAdvance: tier=1, ceiling=3, contributionApplied=1. ReserveMuster: retreatDuty=Fallback, reserveDuty=Muster, committed=1. ReserveRecovery: duty=Ready, healthRatio=0.95, readyReserveCount=1.`
7. Siege smoke
   - `Siege smoke validation passed: baselinePhase=True, strainPhase=True, recoveryPhase=True, supportPhase=True. Baseline: strain=0, attrition=False, supportRefreshCount=0. Strain: status=Strained, strain=6.25, attackMultiplier=0.88, speedMultiplier=0.90. Recovery: supported=True, strain=3.88, supportRefreshCount=51. Support: engineerSupport=True, supplySupport=True, refreshCount=3, suppliedUntil=11.20.`
8. `node tests/data-validation.mjs`
   - `Bloodlines data validation passed.`
9. `node tests/runtime-bridge.mjs`
   - `Bloodlines runtime bridge validation passed.`
10. `scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1`
   - `STALENESS CHECK PASSED: Contract revision=50, last-updated=2026-04-21 is current. Latest handoff: 2026-04-21-unity-fortification-repair-narrative.md (2026-04-21).`
11. Dedicated scout raid smoke
   - `BLOODLINES_SCOUT_RAID_AND_INTERDICTION_SMOKE PASS`
   - `PhaseFarmRaid PASS: food=38.00 influence=18.00 loyalty=72.00 raidedUntil=25.00 retreatX=12.00.`
   - `PhaseWellRaidBlocksFieldWater PASS: support resumed after expiry; refreshCount=1 suppliedUntil=42.00 water=7.80.`
   - `PhaseDropoffReroute PASS: worker routed to clear hall at (42.00,0.00).`
   - `PhaseSupplyWagonInterdiction PASS: interdictedUntil=19.00 recoveryUntil=31.00 wagonMove=(160.00,100.00).`

## Unity-Side Simplifications Deferred

- The checked-in bootstrap-runtime and canonical scene-shell wrappers are still
  path-pinned to `D:\ProjectsHome\Bloodlines\unity`; clean worktrees still need
  temporary local copies for truthful validation.
- The Unity-generated `.csproj` files remain gitignored and worktree-local.
  The merge worktree did not have its own generated project surface, so the
  post-merge validation rerun was executed from detached `master` in the scout
  worktree where the generated files already existed.

## Exact Next Action

1. Claim the unimplemented player-facing marriage diplomacy lane on a fresh
   `codex/unity-player-marriage-proposal` branch.
2. Add a new `PlayerDiplomacy` lane entry to the concurrent-session contract
   before writing any runtime code.
3. Port the browser `proposeMarriage` gate chain first, then land the dedicated
   four-phase proposal smoke validator before moving on to acceptance.
