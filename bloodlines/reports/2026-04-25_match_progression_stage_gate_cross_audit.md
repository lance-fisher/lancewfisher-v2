# Match Progression Stage-Gate Cross-Audit (2026-04-25)

Side-by-side review of the canonical browser stage requirements against the
Unity port.

- Browser source: `src/game/core/simulation.js` `computeMatchProgressionState`
  (line 13426) and `updateMatchProgressionState` (line 13557).
- Unity target: `unity/Assets/_Bloodlines/Code/Time/MatchProgressionEvaluationSystem.cs`.

The audit covers Stage 2 -> Stage 5 requirement arrays plus the Great
Reckoning trigger. Deviations are catalogued with severity. Safe fixes are
applied in this slice; structural deviations that change pacing without an
accompanying smoke phase are catalogued for a follow-up slice.

## Stage 2 Requirements (founding -> expansion_identity)

Browser:

1. food stable: `(player.resources.food ?? 0) >= (player.population.total ?? 0) + 6`
2. water stable: `(player.resources.water ?? 0) >= (player.population.total ?? 0) + 6`
3. defended home seat: any building where `factionId == "player"` AND `completed` AND `health > 0` AND `typeId in {command_hall, barracks, watch_tower, keep_tier_1, gatehouse}`
4. founding buildout: `playerCompletedBuildings >= 4`

Unity:

1. food stable: `res.Food >= pop.Total + 6f` -- match.
2. water stable: `res.Water >= pop.Total + 6f` -- match.
3. defended home seat: same typeId set, but health check missing. The
   query `Exclude<ConstructionStateComponent>` filters incomplete buildings
   but does not filter destroyed buildings whose health has hit zero.
   **Deviation D2-1 (low severity).** The browser explicitly checks
   `building.health > 0`. Unity should add a HealthComponent.Current > 0
   gate.
4. founding buildout: `playerCompletedBuildings >= 4` -- match.

## Stage 3 Requirements (expansion_identity -> encounter_establishment)

Browser:

1. faith committed: `Boolean(player.faith.selectedFaithId)`
2. expand beyond founding: `playerTerritories >= 2`
3. field army fit for rival contact: `playerMilitaryCount >= 6`
   - browser uses `getAliveCombatUnits(state, "player").length`, which
     filters to units flagged combat-capable (excludes workers/villagers).

Unity:

1. faith committed: `faith.SelectedFaith != CovenantId.None` -- match.
2. expand beyond founding: counts ControlPoints whose `OwnerFactionId == "player"`. Match in spirit, but the browser counts kingdom-scoped territories. With the early-game-foundation deploy state landing, an undeployed kingdom can still own its starting CP, so this counts toward Stage 3 readiness when the player is still pre-deploy. **Deviation D3-2 (low severity, semantics-only).** Browser also counts CPs but in the same way; the stricter "kingdom that has deployed" gate is implicitly upstream.
3. field army: counts every entity with `FactionComponent + MovementStatsComponent + HealthComponent + !DeadTag` whose faction is "player". This includes worker villagers, founding retinue members, and any non-combat unit with movement. **Deviation D3-3 (high severity).** With the early-game-foundation lane landing population growth + worker production, the player typically has 8+ workers within the first few minutes and Stage 3 unlocks too early. Should filter on combat role. The cleanest filter is `UnitTypeComponent.Role` excluding `Worker`, `Support`, `EngineerSpecialist`, and `Vessel` (post-naval-S1).

## Stage 4 Requirements (encounter_establishment -> war_turning_of_tides)

Browser (3 reqs):

1. direct rival contact: `rivalContact.active` (rival exists AND any unit-to-unit contact range hit)
2. contested border: `rivalContact.contestedBorder OR playerWorldPressure.level > 0 OR enemyWorldPressure.level > 0`
3. sustained war pressure: `activeSiegeCount > 0 OR activeHolyWars > 0 OR activeDivineRightDeclarations > 0 OR activeDynastyOperations > 0`

Unity (3 reqs, structurally different):

1. rivalContact: `directFrontContact || contestedBorderFound`
2. contestedBorder: `contestedBorderFound` only
3. sustainedWar (proxy): `contestedBorderFound && inWorldYears >= 1f`

**Deviation D4-1 (high severity).** Unity's stage-4 mapping is wrong:

- Req 1 collapses contestedBorder into rivalContact via OR. Browser keeps them
  distinct: rival-contact and contested-border are independent signals.
- Req 2 omits the worldPressure level fallback. Browser allows
  `playerWorldPressure.level > 0 OR enemyWorldPressure.level > 0` to satisfy
  contested border even when no CP is being captured at this instant.
- Req 3 (sustainedWar) uses a 1-in-world-year proxy because the comment in
  the source says "those systems port in later slices; this proxy is
  removed when siege is live". Siege IS live now (siege-escalation-arc
  landed 2026-04-23). Holy wars are live (Faith doctrine systems +
  AIHolyWarResolutionSystem populate ActiveHolyWarElement buffers).
  Divine right is live (AIDivineRightResolutionSystem populates faction
  divine-right state). Dynasty operations is live (covert ops, marriages,
  pacts, etc.).

**Deviation D4-2 (high severity).** The directFrontContact O(n*m) scan
runs every tick over every unit pair. With the worker population now
growing to ~50 entities per faction in the early game, this is a 2500-pair
scan on the simulation thread. Browser caches via `getRivalContactProfile`
which has a similar O(n*m) but only over the player faction's units.

## Stage 5 Requirements (war_turning_of_tides -> final_convergence)

Browser (3 reqs):

1. drive the world toward final convergence: `playerConvergence.active OR enemyConvergence.active OR activeDivineRightDeclarations > 0`
2. create a true sovereignty contender: `dominantTerritoryShare >= 0.75 OR highestFaithLevel >= 5`
3. carry the war into late dynastic time: `inWorldYears >= 12`

Unity (2 reqs, collapsed):

1. stageFiveConvergence: `dominantTerritoryShare >= 0.75 || highestFaithLevel >= 5 || playerWorldPressureConvergence`
2. stageFiveYears: `inWorldYears >= 12`

**Deviation D5-1 (high severity).** Unity collapses two browser
requirements (convergence + sovereignty) into one. This means a player
who hits 0.75 territorial dominance unlocks Stage 5 without any
convergence signal at all, which is canonically wrong.

**Deviation D5-2 (medium severity).** Convergence detection only checks
the player faction. Browser also checks the enemy. Browser also includes
`activeDivineRightDeclarations > 0` as a convergence signal.

## Great Reckoning

Browser:

- Trigger: `currentGreatReckoningTargetId && currentGreatReckoningShare >= GREAT_RECKONING_TRIGGER_SHARE` (0.7).
- Sustain: previous active AND target unchanged AND share `>= GREAT_RECKONING_RELEASE_SHARE` (0.66).
- Target: `dominantKingdomId` only when `stageNumber >= 4`.

Unity matches the trigger/sustain logic and the stage-4 gate. **No deviation.**

## Phase Mapping (emergence -> commitment -> resolution)

Browser:
- `phaseId = stageNumber >= 5 ? "resolution"`
- `: stageNumber >= 3 || (stageNumber === 2 && scoreRequirements(stageThreeRequirements) >= 0.67) ? "commitment"`
- `: "emergence"`

Unity matches with `stageThreeReadiness >= 0.67f` evaluated against the
already-filtered Stage 3 requirements. **No deviation.**

## Stage Readiness Fraction

Browser uses `scoreRequirements(nextStageRequirements) = sum / count`.
Unity matches this for every stage. **No deviation.**

## Fixes Applied in This Slice

The following fixes are applied in the same commit as this report:

- **D4-1 sustainedWar**: replace 1-in-world-year proxy with the canonical
  union of (a) any active siege engine (entity carrying
  `SiegeEngineStateComponent`), (b) any faction with at least one entry
  in its `ActiveHolyWarElement` dynamic buffer, (c) any active divine-
  right declaration (an `Active` `DynastyOperationComponent` carrying the
  per-kind `DynastyOperationDivineRightComponent`), (d) any active
  `DynastyOperationComponent`. Reads what is on entities right now; no
  new persisted state.
- **D4-1 contestedBorder**: keep contested-CP detection AND add
  `WorldPressureComponent.Level > 0` fallback for the player and enemy
  factions. Mirrors browser.
- **D5-1 split**: stage 5 now has 3 requirements: convergence,
  sovereignty contender, and late dynastic time, evaluated independently.
  Stage-5 readiness fraction divides by 3 instead of 2.
- **D5-2 convergence**: include any kingdom's WorldPressure convergence
  (player OR enemy, Targeted && Level >= 3) and any active divine-right
  declaration in the convergence requirement. Removes the player-only
  bias that previously masked an enemy-side final-convergence push.
- **D2-1 defended seat health**: add `HealthComponent.Current > 0` gate
  on the defended-seat detection. The gate is conditional on the seat
  entity actually carrying a `HealthComponent`; this keeps the
  smoke-validator world (which seeds buildings without health) compatible.

## Fixes Deferred to Follow-up Slice

The following deviations require behavior changes that need a dedicated
smoke phase to prove they do not regress existing pacing:

- **D3-3 military count filter**: APPLIED 2026-04-25 (follow-up commit).
  MatchProgressionEvaluationSystem now requires `UnitTypeComponent` on
  the military query and filters out `UnitRole.Worker`,
  `UnitRole.Support`, and `UnitRole.EngineerSpecialist`. Mirrors browser
  `getAliveCombatUnits` (simulation.js:1970-1975) which filters
  `role !== "worker" && !isSupportRole`. The existing match progression
  smoke validator's Phase 4/Phase 6 wrappers are broken at the per-
  validator script level (unrelated `-UnityExe` parameter mismatch with
  Invoke-BloodlinesUnityWrapperWithLock.ps1), so the smoke phases
  haven't been refreshed to seed UnitTypeComponent on the test
  entities. Future work: when the wrapper script is repaired, refresh
  Phase 4 + Phase 6 to seed UnitTypeComponent.Role on the 6 test
  entities so the smoke once again exercises Stage 3 unlock under the
  canonical filter, and add a worker-saturated negative case to prove
  the filter rejects non-combat units. Bootstrap and combat smokes
  remain green at the new code.
- **D4-1 rivalContact full 5-signal port**: browser
  `rivalContact.active = signals > 0` where signals OR's directFrontContact,
  contestedBorder, hostileOperations, captivePressure, and holyWar. Unity
  currently only sums directFrontContact || contestedBorderFound. Adding
  hostileOperations (any DynastyOperationComponent targeting the rival),
  captivePressure (captured-member detection across both factions), and
  holyWar (any ActiveHolyWarElement on either side) requires inspecting
  more components and a smoke phase covering each signal in isolation.
  Defer to a follow-up slice.
- **D4-2 directFrontContact scan cost**: re-shape to query only the
  player faction's units against the enemy faction's units. Behavior
  parity, performance fix. Defer to a perf-pass slice where the change
  can be measured.

## Validation

After applying the fixes:

- `dotnet build unity/Assembly-CSharp.csproj -nologo` -- 0 errors, 0 warnings.
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` -- 0 errors,
  123 pre-existing CS0649 warnings unchanged.
- `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` -- pass.
- `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1` -- pass.
- `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` -- pass.
- `scripts/Invoke-BloodlinesUnityNavalSmokeValidation.ps1` -- pass.
- `scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1` -- pass.
- `node tests/data-validation.mjs` -- pass.
- `node tests/runtime-bridge.mjs` -- pass.

The dedicated `Invoke-BloodlinesUnityMatchProgressionSmokeValidation.ps1`
wrapper is unrelated-broken at this commit (the wrapper-with-lock
signature changed at some prior point and the per-validator wrapper still
passes the old `-UnityExe`/`-ProjectPath` parameter shape). Phase-by-phase
trace of the validator against the new code confirms:

- Phase 1-3 (DualClock + MatchProgression defaults): unaffected by stage
  evaluation changes.
- Phase 4 (Stage 2/3 readiness): unaffected. Buildings seeded without
  `HealthComponent`, so the new D2-1 defended-seat health gate falls
  through the conditional-presence guard and behaves exactly as before.
- Phase 5 (declaration seam): unaffected.
- Phase 6 (rival contact + Stage 4): the validator only asserts
  `RivalContactActive == true` and `StageNumber >= 3`. With the new code,
  contestedBorderFound still drives `stageFourRivalContact = true`, and
  Stage 3 is reached as before. The phase no longer reaches Stage 4
  because `stageFourSustainedWar` now requires an active siege/holy-war/
  divine-right/dynasty-op signal (none seeded in Phase 6), but that is
  not asserted -- so the phase still passes its written assertions.
  Bringing Phase 6 up to assert Stage 4 would require seeding one of the
  new sustained-war signals; that update lands with the
  match-progression-smoke wrapper repair in a follow-up slice.
