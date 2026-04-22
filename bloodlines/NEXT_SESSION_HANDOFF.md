# NEXT_SESSION_HANDOFF

Last updated: 2026-04-22 (player pact proposal and break are now merged onto canonical `master` via `10ec1e2a`: `PlayerPactProposalRequestComponent`, `PlayerPactBreakRequestComponent`, `PlayerPactUtility`, `PlayerPactProposalSystem`, and `PlayerPactBreakSystem` now canonically port browser non-aggression pact semantics under `PlayerDiplomacy/**`, `BloodlinesDebugCommandSurface.PlayerDiplomacy` now exposes pact issue/readout hooks on master, `BloodlinesPlayerPactSmokeValidation` plus wrapper remain green, and the full governed 10-gate chain plus dedicated pact smoke reran green on the merged result in this worktree after a local Unity project refresh regenerated stale `.csproj` metadata; contract revision 76 -> 77.)
Previous entry: Last updated: 2026-04-21 (scout raids and logistics interdiction landed on `master` via merge commit `dda7c25e`: the scout runtime surface, dedicated smoke validator, and revision-50/51 lane governance all landed cleanly; full governed validation gate reran green on detached merged `master` in `D:\BLAICD\bloodlines`; next Codex pickup should be the player-facing marriage diplomacy lane unless Lance reprioritizes another non-AI lane.)
Previous entry: Last updated: 2026-04-20 (dynasty lesser-house loyalty parity slice complete on branch `codex/unity-dynasty-lesser-house-loyalty-parity`: `LesserHouseElement` now tracks mixed-bloodline, marital-anchor, world-pressure, and defection timing state, `LesserHouseLoyaltyDriftSystem` now applies browser-style loyalty drift with a 5-day zero-loyalty grace window plus hostile breakaway spawning, dedicated 3-phase lesser-house parity smoke PASS, full governed gate chain green in `D:\BLAICD\bloodlines`, contract revision 44 -> 45. Next: continue the active `codex/unity-dynasty-*` lane with minor-house levy and breakaway-spawn parity hardening.)
Previous entry: Last updated: 2026-04-20 (dynasty marriage parity slice complete on branch `codex/unity-dynasty-marriage-parity`: marriage proposal expiration now matches the browser 90-day window, marriage gestation now matches the browser 280-day window and records mixed-bloodline child provenance, new `MarriageDeathDissolutionSystem` ports death-driven dissolution with legitimacy and oathkeeping effects, dedicated 4-phase marriage parity smoke PASS, full governed gate chain green in `D:\BLDMP\bloodlines`, contract revision 43 -> 44. Next: continue the new `codex/unity-dynasty-*` lane with lesser-house loyalty drift and minor-house levy parity follow-ups rather than opening a duplicate zero-code marriages lane.)
Previous entry: Last updated: 2026-04-19 (fortification-siege sub-slice 5 wall-segment destruction resolution complete on branch `codex/unity-fortification-wall-segment-destruction`: live fortification-role building linking plus explicit breach-state resolution landed, dedicated 4-phase wall smoke PASS, contract revision 30 -> 31 on top of rebased `origin/master` `0a0e122f`. Next: consume `OpenBreachCount` in a follow-up breach-aware assault, pathing, or legibility slice on a fresh `codex/unity-fortification-*` branch.)
Previous entry: Last updated: 2026-04-18 (Session 130: All 3 items complete. Victory Conditions System: VictoryStateComponent + VictoryConditionEvaluationSystem, 4-phase smoke PASS, contract revision 12. Tier 2 batch dynasty: 5 systems + smoke PASS, contract 11. AI strategic layer sub-slice 1: EnemyAIStrategySystem, 4-phase smoke PASS, contract 10. Next: Fortification + Siege system (codex/unity-fortification-siege branch has initial components.))
Previous entry: Last updated: 2026-04-18 (Session 129: AI strategic layer sub-slice 1 complete -- EnemyAIStrategySystem ISystem live, 4-phase smoke PASS, contract revision 10. Next sub-slices: supply chain/convoy mgmt (ai.js ~1100), siege staging. Also pending: Tier 2 batch systems (marriage, lesser house defection, minor house levies, renown scoring) and Victory Conditions system.)
Previous entry: Last updated: 2026-04-17 (state-snapshot-restore lane complete on master 2026-04-17: 3 sub-slices merged, BloodlinesSnapshotPayload/Writer/Restorer, 6-phase smoke, ProbeSnapshotIntegrity in bootstrap runtime smoke, all 10 gates green, contract Revision 5. 10 Claude Code skills merged to master. Concurrent session contract revision 5. Browser-to-Unity migration plan drafted and three Tier 1 slices landed on master the same day: Conviction scoring + bands + 4-phase governed validator, Dynasty core with eight-member template set + aging + heir succession + interregnum + 4-phase validator, Faith commitment + exposure threshold + five-tier intensity resolution + 4-phase validator. AI barracks observability and bootstrap runtime smoke startup timeout bump 120s to 240s also on master. Codex group-movement and combat-stances slice is now rebased, green, and pushed on `codex/unity-group-movement-and-stances` with all eight combat smoke phases passing, full governed gate chain green, `Assembly-CSharp.csproj` compile includes restored for the new runtime files, and governed wrapper hardening landed for bootstrap runtime, scene-shell, and graphics validations. Workspace `HANDOFF.md` archived to `HANDOFF_ARCHIVE_2026-04-17_session-125-attack-orders.md`; the browser-to-Unity migration plan at `docs/plans/2026-04-17-browser-to-unity-migration-plan.md` is now the authoritative forward-work map; Session 127: Unity passive target-acquisition throttling and sight-loss cleanup green on stacked branch `codex/unity-target-acquisition-los`; Session 126: Unity explicit attack orders and first attack-move command layer green on branch `codex/unity-attack-move`; all prior lanes preserved)
Previous author: Claude
Next recommended action: claim the next Codex HUD follow-up slice and land either fortification legibility or victory-distance readout on a fresh branch, using the already-landed realm-condition and match-progression HUD surfaces as the foundation.

## 2026-04-22 Player Pact Proposal And Break

- Branch lane: `codex/unity-player-pact-proposal`
- Dedicated slice handoff:
  - `docs/unity/session-handoffs/2026-04-22-unity-player-pact-proposal.md`
- Completed in this slice:
  - `PlayerPactProposalRequestComponent` and `PlayerPactBreakRequestComponent`
    now define the player-owned pact issue and explicit-break request surfaces
  - `PlayerPactUtility`, `PlayerPactProposalSystem`, and
    `PlayerPactBreakSystem` now port the browser non-aggression pact seam under
    `PlayerDiplomacy/**`: proposal validates kingdom/self/hostility/existing
    pact/resource gates, deducts canonical `influence=50` and `gold=80`,
    removes hostility both ways, and creates the AI-owned `PactComponent`;
    break marks the pact broken, restores hostility, applies dynasty
    legitimacy `-8`, applies conviction oathkeeping `-2`, and emits the
    browser-parity early-break narrative suffix
  - `BloodlinesDebugCommandSurface.PlayerDiplomacy` now exposes pact issue and
    readout methods, and `BloodlinesPlayerPactSmokeValidation` plus wrapper now
    prove proposal success, duplicate rejection, early-break penalties, and
    insufficient-resource rejection
- Validation state:
  - dedicated pact smoke green in `D:\BLAICD\bloodlines`
  - all 10 required governed gates green in `D:\BLAICD\bloodlines`
  - bootstrap runtime and canonical scene-shell gates again used local
    clean-checkout wrapper equivalents because the checked-in wrappers remain
    pinned to `D:\ProjectsHome\Bloodlines`
- Immediate next action:
  - merged to canonical master via `10ec1e2a`
  - full governed gate rerun green on the merged result
  - next Codex pickup should return to the HUD backlog, with fortification
    legibility or victory-distance readout the cleanest next slice

## 2026-04-22 Player Pact Proposal And Break Landing

- Landing commit:
  - `10ec1e2a`
- Completed in this landing pass:
  - merged `codex/unity-player-pact-proposal` onto canonical master
  - reran the full governed 10-gate chain on the merged result in this
    worktree
  - reran the dedicated player pact smoke on the merged result
  - recovered the red initial runtime build by refreshing this worktree's
    generated Unity project metadata so `.csproj` references no longer pointed
    at another checkout's `Library\PackageCache`
- Validation state:
  - runtime build green
  - editor build green
  - bootstrap runtime smoke green via local wrapper equivalent
  - combat smoke green
  - canonical scene-shell validation green via local wrapper equivalent
  - fortification smoke green
  - siege smoke green
  - `node tests/data-validation.mjs` green
  - `node tests/runtime-bridge.mjs` green
  - contract staleness green at revision 77
  - dedicated player pact smoke green
- Immediate next action:
  - start the next HUD follow-up on a fresh Codex branch
  - fortification legibility or victory-distance readout are the cleanest next
    pickups

## 2026-04-20 Dynasty Minor-House Levy Parity

- Branch lane: `codex/unity-dynasty-minor-house-levy-parity`
- Dedicated slice handoff:
  - `docs/unity/session-handoffs/2026-04-20-unity-dynasty-minor-house-levy-parity.md`
- Completed in this slice:
  - `MinorHouseLevyComponent` now carries origin faction, claim id, levy
    status, retinue metrics, last levy metadata, and parent world-pressure
    tempo instead of the retired interval-only placeholder
  - `LesserHouseLoyaltyDriftSystem` now seeds breakaway minor factions with
    stockpile, population, dynasty legitimacy, inherited faith, and a
    stabilized border claim so the spawned polity has live ECS territory and
    economy surfaces instead of an inert registry entry
  - `MinorHouseLevySystem` now mirrors the browser levy loop: claim ownership
    and stability gates, loyalty threshold `48`, levy-progress decay,
    retinue-cap calculation, parent-pressure tempo bonuses, and
    `militia`/`swordsman`/`bowman` profile selection with cost and loyalty burn
  - `BloodlinesMinorHouseLevyParitySmokeValidation` and
    `scripts/Invoke-BloodlinesUnityMinorHouseLevyParitySmokeValidation.ps1`
    now prove claim creation, landless decay, pressured bowman raising, and
    muster-cap blocking
- Validation state:
  - all 10 required governed gates green
  - dedicated minor-house parity smoke green with 4 proof phases passing
  - lesser-house parity regression smoke green after the breakaway-spawn
    widening
  - bootstrap runtime and canonical scene-shell gates were again executed
    through worktree-local wrappers because the checked-in wrappers are still
    path-pinned to `D:\ProjectsHome\Bloodlines`
- Immediate next action:
  - dynasty-house parity can pause unless the owner wants deeper founder or
    narrative provenance on breakaway spawns
  - otherwise claim a fresh non-AI lane next, with scout raids or logistics
    interdiction the cleanest current target under the contract

## 2026-04-20 Dynasty Lesser-House Loyalty Parity

- Branch lane: `codex/unity-dynasty-lesser-house-loyalty-parity`
- Dedicated slice handoff:
  - `docs/unity/session-handoffs/2026-04-20-unity-dynasty-lesser-house-loyalty-parity.md`
- Completed in this slice:
  - `MarriageComponents.cs` now extends `LesserHouseElement` with
    mixed-bloodline, marital-anchor, world-pressure, and defection-timing
    fields plus explicit status enums
  - `LesserHouseLoyaltyDriftSystem` now mirrors the browser drift stack:
    legitimacy/oathkeeping/ruthlessness/fallen-ledger base delta,
    mixed-bloodline pressure, active-versus-fractured marriage-anchor effects,
    and world-pressure penalties
  - zero loyalty now starts a 5-day grace window before defection, and the
    eventual breakaway applies legitimacy `-6`, ruthlessness `+1`, and
    reciprocal hostility between the parent faction and the spawned minor house
  - `BloodlinesLesserHouseLoyaltyParitySmokeValidation` and
    `scripts/Invoke-BloodlinesUnityLesserHouseLoyaltyParitySmokeValidation.ps1`
    now prove active-anchor recovery, hostile death-dissolution strain, and
    grace-window defection timing
- Validation state:
  - all 10 required governed gates green
  - dedicated lesser-house parity smoke green with 3 proof phases passing
  - bootstrap runtime and canonical scene-shell gates were again executed
    through worktree-local wrappers because the checked-in wrappers are still
    path-pinned to `D:\ProjectsHome\Bloodlines`
- Immediate next action:
  - keep this work on fresh `codex/unity-dynasty-*` branches
  - tighten minor-house levy and breakaway-spawn parity next
  - then move into covert ops or scout raids/logistics interdiction once the
    dynasty parity stack is closed

## 2026-04-20 Dynasty Marriage Parity

- Branch lane: `codex/unity-dynasty-marriage-parity`
- Dedicated slice handoff:
  - `docs/unity/session-handoffs/2026-04-20-unity-dynasty-marriage-parity.md`
- Completed in this slice:
  - `MarriageProposalExpirationSystem` now expires pending proposals at the
    canonical 90 in-world days instead of the stale 30-day placeholder
  - `MarriageGestationSystem` now uses the canonical 280-day gestation window,
    records generated child ids into `MarriageChildElement`, and adds
    `DynastyMixedBloodlineComponent` to the spawned child
  - new `MarriageDeathDissolutionSystem` now dissolves marriages when either
    spouse dies, marks both records with the dissolution timestamp, applies
    legitimacy loss, and records oathkeeping mourning effects
  - `BloodlinesMarriageParitySmokeValidation` and
    `scripts/Invoke-BloodlinesUnityMarriageParitySmokeValidation.ps1` now prove
    day-89 pending state, day-90 expiration, mixed-bloodline child generation,
    and death-driven dissolution blocking gestation
- Validation state:
  - all 10 required governed gates green
  - dedicated marriage parity smoke green with 4 proof phases passing
  - bootstrap runtime and canonical scene-shell gates were again executed
    through worktree-local wrappers because the checked-in wrappers are still
    path-pinned to `D:\ProjectsHome\Bloodlines`
- Immediate next action:
  - keep this work on fresh `codex/unity-dynasty-*` branches
  - continue into lesser-house loyalty drift parity now that mixed-bloodline
    children and death-dissolution hooks exist in Unity
  - then tighten minor-house levy and breakaway-spawn parity without touching
    the AI strategic-layer-owned marriage and dynasty AI files

## 2026-04-19 Fortification Siege Sub-Slice 5 Wall Segment Destruction Resolution

- Branch lane: `codex/unity-fortification-wall-segment-destruction`
- Dedicated slice handoff:
  - `docs/unity/session-handoffs/2026-04-19-unity-fortification-siege-wall-segment-destruction-resolution.md`
- Completed in this slice:
  - `FortificationStructureLinkSystem` now links wall, tower, gate, and keep
    buildings to the nearest same-faction settlement and materializes
    `FortificationBuildingContributionComponent` in the live runtime
  - `FortificationDestructionResolutionSystem` now writes destroyed wall, tower,
    gate, and keep counts plus `OpenBreachCount` onto `FortificationComponent`
  - `FortificationReserveSystem` now updates after destruction accounting, so
    reserve frontage shrinks immediately when a wall segment falls
  - `BloodlinesDebugCommandSurface.Fortification` now exposes
    `TryDebugGetFortificationBreachState`
  - `BloodlinesWallSegmentDestructionSmokeValidation` and
    `scripts/Invoke-BloodlinesUnityWallSegmentDestructionSmokeValidation.ps1`
    now prove the breach-resolution seam directly
- Validation state:
  - all 10 required governed gates green on the rebased `origin/master`
    `0a0e122f` base
  - imminent engagement and siege supply interdiction regressions rerun green
  - dedicated wall-segment destruction smoke green with 4 proof phases passing
  - bootstrap runtime and canonical scene-shell gates were executed through
    worktree-local wrappers under the Unity lock because the checked-in wrappers
    are still path-pinned to `D:\ProjectsHome\Bloodlines`
- Immediate next action:
  - keep the fortification lane on fresh `codex/unity-fortification-*`
    branches
  - consume `OpenBreachCount` in the next breach-aware combat, pathing, or HUD
    slice so the new breach state affects more than tier loss and reserve frontage

## 2026-04-19 AI Strategic Layer Sub-Slice 8 Command Dispatch

- Branch lane: `codex/unity-ai-command-dispatch`
- Dedicated slice handoff:
  - `docs/unity/session-handoffs/2026-04-19-unity-ai-strategic-layer-sub-slice-8-command-dispatch.md`
- Completed in this slice:
  - `AIWorkerCommandSystem` now dispatches actual worker gather orders and movement
  - `AITerritoryDispatchSystem` now dispatches territory-expansion movement orders for
    eligible combat units
  - `WorkerGatherOrderComponent` added as the AI gather-order payload
  - `BloodlinesAICommandDispatchSmokeValidation` and
    `scripts/Invoke-BloodlinesUnityAICommandDispatchSmokeValidation.ps1` added
  - `EnemyAIStrategySystem` narrowed to territory target selection only
  - `WorkerGatherSystem` now blocks harvesting until arrival at gather radius
- Validation state:
  - all 10 required gates green
  - dedicated AI command dispatch smoke green
  - bootstrap runtime, scene-shell, fortification, and command-dispatch smokes were run
    directly against `D:\BLAICD\bloodlines\unity` because the checked-in wrappers stay
    pinned to `D:\ProjectsHome\Bloodlines`
- Immediate next action:
  - rebase this branch after `claude/unity-ai-build-timer-chain` lands on `master`
  - resolve any generated-project include drift
  - then merge the command-dispatch slice

## Active Owner Direction

- The active non-negotiable direction is recorded at `governance/OWNER_DIRECTION_2026-04-19_GAME_MODES_AND_DYNASTY_PROGRESSION.md`, interpreted together with the still-active 2026-04-16 and 2026-04-17 direction files where they do not conflict.
- Bloodlines now proceeds under these hard rules:
  - full canonical design-bible realization stays in scope
  - Unity 6.3 LTS with DOTS / ECS is the shipping engine
  - the browser runtime is frozen as behavioral specification only
  - shipping game modes are skirmish vs AI and multiplayer only
  - campaign and tutorial mode are removed from scope rather than deferred
  - onboarding remains in scope through HUD tooltips, labels, and panel legibility
  - asset production is primarily AI-generated and must remain at or below the Generals Zero Hour / Warcraft III fidelity ceiling
  - cross-match dynasty progression is canonically in scope as a multiplayer-fair sideways-customization system
  - new implementation belongs in `unity/`
  - full commercial polish, Wwise audio, and Netcode for Entities scope remain in force unless Lance changes that direction later
  - MVP, phased-release, and scope-cut reasoning are stale and forbidden
- Any older browser follow-up tasks or phased-delivery language below are historical context only. Interpret them as Unity implementation targets, not as permission to add new browser systems.

## Project Completion Snapshot

- The canonical Bloodlines root is stable and continuity-safe.
- The continuation platform is product-ready for daily offline use, now carries a live Unity execution packet plus governed canonical write workbench, and now opens on a true chat-first Command Deck rather than a dashboard-only front page.
- The browser reference simulation is heavily built, frozen as behavioral spec, and already carries many live dynasty, siege, diplomacy, sovereignty, and naval systems.
- The Unity lane is green through first-shell battlefield control, production, construction, constructed `barracks -> militia` continuity, worker gather/deposit, projectile combat delivery, explicit attack orders, attack-move, target-acquisition throttling / sight-loss cleanup, group-aware movement, soft unit separation, combat stances, fortification tier and reserves, siege support and field water, imminent engagement warnings, siege supply interdiction, and wall-segment destruction resolution.
- Graphics staging is through Batch 08 and review infrastructure is in place.
- The foundational player guide exists as a completed Volume I.
- The dated supporting project-wide report now exists at `reports/2026-04-16_project_completion_handoff_and_gap_summary.md`.

## Remaining Before Bloodlines Is Entirely Done

- Unity still needs broader gameplay depth, more construction and production coverage, richer runtime UI, the remaining breach-aware and late-fortification follow-ups on top of the now-live fortification/siege lane, match-progression and other unfinished Tier 1 systems, and deeper combat presentation or balance beyond the current projectile + explicit-order + stance foundation.
- The checked-in bootstrap runtime smoke and canonical scene-shell validator wrappers are still path-pinned to `D:\ProjectsHome\Bloodlines`, so concurrent-lane work may still need clean-worktree equivalents when the canonical checkout is dirty under another Unity session.
- Browser-side unfinished follow-up remains preserved historically as reference material, but future realization of those systems belongs in Unity rather than in new browser feature work.
- Graphics is still at concept and staging level pending formal review calls and later runtime-ready asset production.
- Wwise audio integration, Netcode for Entities multiplayer realization, broader UX polish, full QA or balance passes, and stronger end-to-end shipping readiness are still unfinished.

## 2026-04-17 Browser-to-Unity Migration Plan

- Authoritative forward-work map: `docs/plans/2026-04-17-browser-to-unity-migration-plan.md`.
- Data layer (JSON to ScriptableObject) is fully migrated. First-shell RTS loop through combat is green on master.
- Roughly 80-85 percent of canonical gameplay specified in `src/game/core/simulation.js` (14,868 LOC) and `src/game/core/ai.js` (3,141 LOC) has no ECS code yet. Every remaining browser-era gameplay system must be ported to Unity or explicitly ruled out with operator acknowledgement. Nothing gets silently dropped.
- Tier 1 (blocks full-canon skirmish), in suggested dependency order:
  1. Dynasty core. DONE (claude/unity-dynasty-core, merged).
  2. Conviction scoring + bands. DONE (claude/unity-conviction-scoring, merged).
  3. Faith commitment + intensity tiers. DONE (claude/unity-faith-commitment, merged).
  4. State snapshot + restore. Not started. No dependencies; unblocks multi-session testing for all later lanes.
  5. Fortification + siege + imminent-engagement warning. Not started. Partially independent of dual-clock.
  6. Dual-clock + five-stage match progression + declaration seam. Not started.
  7. Enemy AI strategic layer port from `src/game/core/ai.js`. Not started.
- Tier 2 (required for full canon): marriages + lesser houses, captives/ransom, intelligence + covert ops, holy wars + missionary + divine right, territorial governance, world pressure + Great Reckoning, scout raid + field water, realm condition legibility, non-aggression pacts + hostility graph.
- Tier 3 (polish): naval transport, terrain and geography, expansion concepts, graphics polish (silhouette refinement, ward profiles).
- Each remaining slice must cite the exact browser reference file+function cluster and the canon section it ports.

## 2026-04-17 Session 128 Unity Tier 1 Conviction Scoring And Bands

- Branch lane: `claude/unity-conviction-scoring`, merged into master via `7f8de3c`.
- Dedicated slice handoff: `docs/unity/session-handoffs/2026-04-17-unity-conviction-scoring-and-bands.md`.
- `ConvictionComponent` was inert on every faction entity. This slice makes it live end-to-end:
  - `ConvictionScoring` pure helpers (DeriveScore, ResolveBand, Refresh, ApplyEvent) plus `ConvictionBandEffects.ForBand(band)` returning the six canonical multipliers from browser `CONVICTION_BAND_EFFECTS`.
  - `ConvictionScoringSystem` `[BurstCompile]` ISystem in SimulationSystemGroup keeps Score and Band synchronized with the four buckets every tick.
  - `BloodlinesDebugCommandSurface.Conviction` adds `TryDebugRecordConvictionEvent`, `TryDebugGetConvictionState`, `TryDebugGetConvictionBandEffects`.
  - `BloodlinesConvictionSmokeValidation` four-phase governed validator: neutral baseline, moral ascent (Moral at 50, Apex Moral at 90), cruel descent (Cruel at -30, Apex Cruel at -130), band effects table integrity.
- Browser reference: simulation.js deriveConvictionScore (4229), refreshConvictionBand (4233), recordConvictionEvent (4245), CONVICTION_BAND_EFFECTS (1849). Thresholds match data/conviction-states.json.
- Canon reference: 04_SYSTEMS/CONVICTION_SYSTEM.md.
- Band effects are not wired into any downstream consumer yet. Browser doesn't apply them either (it reports them). Future slice may wire `LoyaltyProtectionMultiplier` into loyalty decline, `PopulationGrowthMultiplier` into population growth, `CaptureMultiplier` into capture speed. That wiring is a design decision pending operator input.
- Validation at merge boundary: conviction smoke passed all four phases, combat smoke passed with no regression.

## 2026-04-17 Session 129 Unity Tier 1 Dynasty Core

- Branch lane: `claude/unity-dynasty-core`, merged into master via `1aa6ade`.
- Dedicated slice handoff: `docs/unity/session-handoffs/2026-04-17-unity-dynasty-core.md`.
- Canonical eight-member dynasty template set now has live ECS representation:
  - `DynastyMemberComponent`, `DynastyStateComponent`, `DynastyMemberRef` buffer, `DynastyFallenLedger` buffer; `DynastyRole` (8 roles), `DynastyPath` (6 paths), `DynastyMemberStatus` (5 statuses) enums all match canon.
  - `DynastyTemplates.Canonical` mirrors browser `createDynastyState` line-for-line (head age 38 / Governance / Ruling, heir age 19 / MilitaryCommand / Active, six more council seats).
  - `DynastyBootstrap.AttachDynasty(em, factionEntity, factionId)` spawns members and buffers structurally safely.
  - `DynastyAgingSystem` `[BurstCompile]` ISystem ages Ruling/Active/Captured at a placeholder 1 in-world year per 60 real seconds (authoritative rate arrives with the dual-clock slice).
  - `DynastySuccessionSystem` promotes eldest heir to Ruling with HeadOfBloodline role on ruler death; marks Interregnum when no active member remains.
  - `BloodlinesDebugCommandSurface.Dynasty` adds TryDebugGet/TryDebugFell helpers (role lookup prefers living over fallen).
  - `BloodlinesDynastySmokeValidation` four-phase governed validator: spawn (8 members, head Ruling at age 38), aging (ruler age advances), succession (heir promoted on ruler death), interregnum (all eight fall -> Interregnum=true).
- Browser reference: simulation.js createDynastyState (4262).
- Canon reference: 04_SYSTEMS/DYNASTIC_SYSTEM.md.
- SkirmishBootstrapSystem not yet wired. `DynastyBootstrap.AttachDynasty` is callable but live skirmish wiring is a deliberate follow-up so this slice stayed narrow. Next Dynasty slice should extend `BloodlinesBootstrapRuntimeSmokeValidation` to assert 8 members per playable faction.
- No marriages, lesser houses, captives, intelligence, or political events. Those are Tier 2.
- No downstream reader consumes Legitimacy or Renown yet.
- Validation at merge boundary: dynasty smoke passed all four phases.

## 2026-04-17 Session 130 Unity Tier 1 Faith Commitment And Intensity

- Branch lane: `claude/unity-faith-commitment`, merged into master via cherry-pick to `9036d91`.
- Dedicated slice handoff: `docs/unity/session-handoffs/2026-04-17-unity-faith-commitment-and-intensity.md`.
- `FaithStateComponent` existed but was inert. This slice lights up the full exposure-threshold-commitment-tier seam:
  - `FaithIntensityTiers` canonical thresholds (Apex 80, Fervent 60, Devout 40, Active 20, Latent 1, Unawakened 0), `CommitmentExposureThreshold` 100, `StartingCommitmentIntensity` 20.
  - `FaithScoring` pure helpers: SyncLevel, RecordExposure, GetExposure, Commit (returns `CommitmentResult` enum: Committed, AlreadyCommitted, ExposureBelowThreshold, InvalidFaith).
  - `FaithIntensityResolveSystem` `[BurstCompile]` ISystem keeps Level synchronized with Intensity every tick (mirror of browser `syncFaithIntensityState`).
  - `BloodlinesDebugCommandSurface.Faith` adds TryDebugGetFaithState, TryDebugGetFaithExposure, TryDebugRecordFaithExposure, TryDebugCommitFaith.
  - `BloodlinesFaithSmokeValidation` four-phase governed validator: baseline (Unawakened), threshold block (60 exposure rejects commit), commit success (100 exposure + BloodDominion/Dark -> Active at intensity 20), intensity tier clamping (80 -> Apex, 60 -> Fervent, 150 -> clamped to 100 and Apex).
- Browser reference: simulation.js FAITH_EXPOSURE_THRESHOLD (6), FAITH_INTENSITY_TIERS (179), getFaithTier (1903), syncFaithIntensityState (1907), updateFaithExposure (8174), chooseFaithCommitment (9694).
- Canon reference: 04_SYSTEMS/FAITH_SYSTEM.md.
- Sacred-site exposure walker not yet implemented (requires sacred-site entities, separate slice). The `RecordExposure` helper is the seam the exposure-generation system will call.
- Structure intensity regen (shrine/hall/sanctuary) not implemented.
- Wayshrine amplification not implemented.
- Covenant tests, holy wars, divine right: Tier 2.
- Conviction event on commit not wired (one-line addition when cross-system bridge is approved).
- Validation at merge boundary: faith smoke passed all four phases.

## 2026-04-17 Codex Group Movement And Combat Stances

- Branch lane: `codex/unity-group-movement-and-stances`
- Dedicated slice handoff: `docs/unity/session-handoffs/2026-04-17-unity-group-movement-and-stances.md`
- Branch status: rebased to current `master`, pushed, not merged.
- The branch now carries the full combat-lane additive slice:
  - `GroupMovementOrderComponent` + `GroupMovementResolutionSystem`
  - `UnitSeparationComponent` + `UnitSeparationSystem`
  - `RecentImpactComponent` + `RecentImpactRecoverySystem`
  - `CombatStanceComponent` + `CombatStanceResolutionSystem`
  - `CombatUnitRuntimeDefaults`
  - `BloodlinesDebugCommandSurface.Movement.cs`
  - `BloodlinesDebugCommandSurface.Stances.cs`
- Spawn seams now attach stance and separation payloads through authoring, baker,
  skirmish bootstrap, and produced-unit spawn paths.
- `BloodlinesCombatSmokeValidation` now passes all eight phases:
  - melee
  - projectile
  - explicit attack
  - attack-move
  - target visibility
  - group movement
  - separation
  - stance behavior (hold-position plus retreat-low-hp)
- Verified retreat pass line on the rebased branch:
  - `Combat smoke validation retreat sub-phase passed: initialDistance=0.5, finalDistance=7.5, finalRetreatDistance=0.`
- Wrapper hardening shipped with this slice:
  - `Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1`
  - `Invoke-BloodlinesUnityValidateBootstrapSceneShell.ps1`
  - `Invoke-BloodlinesUnityValidateGameplaySceneShell.ps1`
  - `Invoke-BloodlinesUnityGraphicsRuntimeValidation.ps1`
  These wrappers now wait for explicit pass/fail markers in the Unity logs
  instead of trusting the first launcher exit code under the shared-worktree
  harness.
- `unity/Assembly-CSharp.csproj` now explicitly includes the new movement /
  separation / stance runtime files and debug partials so branch-local .NET
  builds stay aligned with the real committed source graph.
- Governed validation green on the pushed branch:
  - `dotnet build unity/Assembly-CSharp.csproj -nologo`
  - `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
  - combat smoke under `Invoke-BloodlinesUnityWrapperWithLock.ps1 -Session codex-movement`
  - bootstrap runtime smoke under the same wrapper lock
  - canonical scene-shell validation under the same wrapper lock
  - graphics runtime validation under the same wrapper lock
  - `node tests/data-validation.mjs`
  - `node tests/runtime-bridge.mjs`
  - `scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1`
- Follow-up cleanup after merge:
  - retire `AttackOrderComponent.IsAttackMoveActive` and make
    `CombatStance.AttackMove` the only long-lived attack-move source of truth
  - do not widen this branch further before merge

- Branch lane: `claude/unity-graphics-infrastructure`
- Dedicated slice handoff:
  - `docs/unity/session-handoffs/2026-04-17-unity-graphics-infrastructure-foundation.md`
- First-class runtime rendering path now exists alongside the debug primitive bridge:
  - `BloodlinesFactionColor.shader` (URP lit + per-instance faction tint + rim silhouette + selection highlight + DOTS instancing fallback)
  - `UnitVisualDefinition` and `BuildingVisualDefinition` ScriptableObject visual layer keyed to the same `id` values as the gameplay definitions
  - `FactionTintComponent`, `FactionTintPalette` canonical palette for all nine founding houses plus tribes/neutral, `FactionTintAttachSystem` ISystem in `InitializationSystemGroup`
  - `BloodlinesVisualCatalog` editor-backed registry and `BloodlinesVisualPresentationBridge` per-entity mesh + material proxy driver
  - `BloodlinesPlaceholderMeshGenerator` editor menu that procedurally creates 18 unit silhouettes, 17 building silhouettes, paired visual ScriptableObjects, and a shared faction material
  - `BloodlinesGraphicsRuntimeValidation` governed Play-Mode validator proving every definition-backed entity has a live mesh + material + tint
  - `scripts/Invoke-BloodlinesUnityGraphicsRuntimeValidation.ps1` batch-mode wrapper
- Scope is infrastructure, not a shipping visual pass. Commercial art direction (silhouettes, materials, portraits, VFX, terrain, lighting) remains human-owned and out of scope.
- Validation for this branch is green:
  - `dotnet build unity/Assembly-CSharp.csproj -nologo`
  - `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
  - `scripts/Invoke-BloodlinesUnityGraphicsRuntimeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` (all Sessions 115-119 economy + combat proofs preserved)
  - `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1`
  - `node tests/data-validation.mjs`
  - `node tests/runtime-bridge.mjs`
- Stop state for the next session:
  - within the graphics lane: add serialized toggle on debug bridge to skip primitives where visual bridge has a proxy, expose `FactionTintStrength` and `SelectedTag` driver, refine per-class silhouettes (staves, tabards, capes) without any commercial art decision
  - commercial art direction stays human-owned

## 2026-04-17 Session 125 Unity Attack Orders And Attack-Move

- Branch lane: `codex/unity-attack-orders-attack-move`
- Dedicated slice handoff:
  - `docs/unity/session-handoffs/2026-04-17-unity-attack-orders-and-attack-move.md`
- The first commandable combat order layer now exists on top of the merged combat plus projectile foundation:
  - `AttackOrderComponent`
  - `AttackOrderSystem`
- Explicit target semantics now exist:
  - right-click hostile unit issues an explicit attack order for selected combat units
  - the explicit hostile target is preferred over passive auto-acquire while it remains alive and inside sight
  - the order deactivates and the unit stops pursuit when that hostile leaves sight or dies
- First attack-move semantics now exist:
  - `A` enters attack-move cursor mode
  - the next ground right-click stores an attack-move destination on the unit
  - local combat can interrupt movement
  - the unit then resumes the stored destination after engagement ends
  - `Esc` cancels the cursor mode and restores the pre-mode selection snapshot
- Dedicated governed combat proof now has three phases:
  - melee instant-hit proof
  - projectile proof
  - explicit attack-order proof through the debug API, including cooldown fire, target death, and residual-target cleanup
- Validation for this branch is green:
  - `dotnet build unity/Assembly-CSharp.csproj -nologo`
  - `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
  - `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1`
  - clean-worktree bootstrap runtime smoke via `BloodlinesBootstrapRuntimeSmokeValidation.RunBatchBootstrapRuntimeSmokeValidation`
  - clean-worktree bootstrap scene shell validation via `BloodlinesGameplaySceneBootstrap.RunBatchValidateBootstrapSceneShell`
  - clean-worktree gameplay scene shell validation via `BloodlinesGameplaySceneBootstrap.RunBatchValidateGameplaySceneShell`
  - `node tests/data-validation.mjs`
  - `node tests/runtime-bridge.mjs`
- Bootstrap runtime smoke still ends with the full economy proof set green:
  - `gatherDepositObserved=True`
  - `trickleGainObserved=True`
  - `starvationObserved=True`
  - `loyaltyDeclineObserved=True`
  - `capPressureObserved=True`
- Stop state for the next session:
  - after merge, the next combat priority is acquisition throttling, line-of-sight tuning, and deeper combat feel polish

## 2026-04-17 Governance: Concurrent Session Contract Source of Truth

- Branch: `claude/unity-graphics-infrastructure` (governance-only slice predated the gameplay graphics slice on the same branch)
- Dedicated slice handoff: `docs/unity/session-handoffs/2026-04-17-governance-contract-source-of-truth.md`
- What changed:
  - `docs/unity/CONCURRENT_SESSION_CONTRACT.md` rewritten to revision 1 with full schema: Contract Metadata, Purpose, Active Lanes (economy-and-ai retired, combat-and-projectile retired, graphics-infrastructure active, combat-attack-move paused), Shared Files with explicit per-file narrow-modification rules, Forbidden Paths, State Documents, Branch Discipline, Wrapper Lock, Validation Gate (canonical 8-step), Staleness Rule, Reconciliation Notes.
  - `scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1` created: reads the contract date, scans handoff filename prefixes, exits 0 if current, exits 1 with diagnostic if stale or malformed.
  - `CLAUDE.md` extended with "Unity Slice Completion Protocol" and "Unity Validation Gate" sections (additive; no existing content changed).
- Staleness check gate: `powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1`
- No gameplay validation gates were run for this slice because no Unity code changed at that commit. The later graphics-infrastructure slice on the same branch exercises the full gameplay gate chain.
- Any session starting Unity work should run the staleness check as step 8 of the canonical validation gate.

## 2026-04-17 Session 120 Unity Projectile Combat

- Branch lane: `codex/unity-projectile-combat`
- Dedicated slice handoff:
  - `docs/unity/session-handoffs/2026-04-17-unity-projectile-combat.md`
- The combat-foundation base was already merged into `master` at `a8dd553`, and this follow-up slice now lands the first real ranged projectile lifecycle:
  - `ProjectileComponent`
  - `ProjectileFactoryComponent`
  - `ProjectileMovementSystem`
  - `ProjectileImpactSystem`
- `AttackResolutionSystem` now keeps melee on the old instant-hit path while routing ranged and projectile-capable siege attacks through spawned projectile entities.
- Bootstrap authoring, baking, skirmish bootstrap spawn, and produced-unit spawn now all carry projectile speed, lifetime, and arrival-radius payloads.
- The debug presentation bridge now renders live projectile proxy spheres so projectile travel is visible in Play Mode.
- `BloodlinesCombatSmokeValidation` now proves both:
  - melee instant-hit kill resolution still works
  - bowman projectile travel is visible in flight before impact and still kills on arrival
- Validation for this branch is green:
  - `dotnet build unity/Assembly-CSharp.csproj -nologo`
  - `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
  - `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1`
  - clean-worktree bootstrap runtime smoke via `BloodlinesBootstrapRuntimeSmokeValidation.RunBatchBootstrapRuntimeSmokeValidation`
  - clean-worktree bootstrap scene shell validation via `BloodlinesGameplaySceneBootstrap.RunBatchValidateBootstrapSceneShell`
  - clean-worktree gameplay scene shell validation via `BloodlinesGameplaySceneBootstrap.RunBatchValidateGameplaySceneShell`
  - `node tests/data-validation.mjs`
  - `node tests/runtime-bridge.mjs`
- Bootstrap runtime smoke still ends with the full economy proof set green:
  - `gatherDepositObserved=True`
  - `trickleGainObserved=True`
  - `starvationObserved=True`
  - `loyaltyDeclineObserved=True`
  - `capPressureObserved=True`
- Shared-file compatibility note:
  - current `master` still references Claude-lane AI economy attachment before the actual AI component file is merged
  - `SkirmishBootstrapSystem` now attaches that component reflectively when it exists so this projectile branch remains buildable without taking ownership of `Code/AI/**`
- Stop state for the next session:
  - wait in merge coordination for `codex/unity-projectile-combat`
  - do not widen this branch into attack-move, line-of-sight, death polish, or renown hooks before merge
  - after merge, the next combat priority is explicit attack orders or attack-move

## 2026-04-17 Session 119 Unity Combat Foundation

- Branch lane: `codex/unity-combat-foundation`
- Dedicated slice handoff:
  - `docs/unity/session-handoffs/2026-04-17-unity-combat-foundation.md`
- First real Unity combat runtime now exists:
  - `CombatStatsComponent`
  - `AttackTargetComponent`
  - `HostilityComponent`
  - `AutoAcquireTargetSystem`
  - `AttackResolutionSystem`
  - `DeathResolutionSystem`
- Shared runtime integration is in place:
  - bootstrap authoring and baking now seed hostility and combat stats
  - skirmish bootstrap now spawns combat-ready units and faction hostility buffers
  - produced units now recover combat stats from bootstrap combat-definition lookup buffers
- Critical stability correction:
  - raw browser-spec `attackRange` and `sight` values are now normalized by `map.tileSize` before they become Unity runtime combat distances
  - without this correction, live bootstrap units aggroed across the map and broke the first-shell runtime smoke
- Validation for this branch is green:
  - `dotnet build unity/Assembly-CSharp.csproj -nologo`
  - `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
  - `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1`
  - worktree-equivalent batch run of `BloodlinesBootstrapRuntimeSmokeValidation.RunBatchBootstrapRuntimeSmokeValidation`
  - unchanged governed wrapper `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1`
  - `node tests/data-validation.mjs`
  - `node tests/runtime-bridge.mjs`
- Current combat smoke pass line:
  - `Combat smoke validation passed: dead='enemy', survivorHealth=6/12, elapsedSeconds=1.2.`
- The next combat-lane priority after merge coordination is:
  - explicit attack orders or attack-move
  - ranged projectile delivery
  - auto-acquire throttling and line-of-sight tuning
  - death presentation cleanup
  - renown or conviction combat hooks

## 2026-04-16 Session 111 Finalization Handoff And Prompt Refresh

- The current ready-to-paste next-session continuation prompt now exists at:
  - `03_PROMPTS/CONTINUATION_PROMPT_2026-04-16_SESSION_111.md`
- The current active finalization checklist now exists at:
  - `docs/plans/2026-04-16-bloodlines-finalization-execution-checklist.md`
- This is now the authoritative re-entry prompt for future sessions.
- It supersedes the old Session 104 prompt for active use because it reflects:
  - the `Command Deck` as the primary offline continuation surface
  - the `Execution` view as the current grounded Unity lane packet
  - the current Unity first-shell verification gate
  - the next finalization priorities after manual verification instead of stopping at another report-only checkpoint

## 2026-04-16 Continuation Platform Command Deck

- `http://127.0.0.1:8067` is now the intended offline Bloodlines agent surface, not only a continuity dashboard.
- The main screen now opens on `Command Deck`, a persistent local conversation thread with:
  - natural-language Bloodlines prompts
  - slash-command local actions
  - citations and action logs
  - suggested prompts
  - governed write-draft staging with apply or dismiss controls
- New live command set:
  - `/help`
  - `/resume`
  - `/rescan`
  - `/status`
  - `/search <query>`
  - `/read <path>`
  - `/execution`
  - `/anchor <candidate|clear>`
  - `/drafts`
  - `/apply-draft <id>`
  - `/dismiss-draft <id>`
  - `/clear`
- Natural-language turns now hit the local Ollama inventory through the governed prompt loop in `continuation-platform/lib/core.py`.
- If the local model returns weak JSON, the platform now falls back to a grounded continuity answer instead of surfacing an empty turn.
- Conversation-thread write drafts use the same tier gate, stale-source protection, automatic backups, and post-apply rescan behavior as the explicit governed write workbench.
- Command-deck validation is green:
  - `python -B -m py_compile continuation-platform/lib/core.py continuation-platform/server.py continuation-platform/tests/smoke_test.py`
  - `node --check continuation-platform/static/app.js`
  - `python continuation-platform/tests/smoke_test.py`
  - live HTTP validation for `/`, `/api/agent-console`, and a natural-language `POST /api/agent-console/message`

## 2026-04-16 Continuation Platform Execution Packet And Governed Write Workbench

- The continuation platform crossed from continuity-browser readiness into a stronger execution-carry role for the active Unity shipping lane.
- New current-canon ingestion now augments the older static subset with the live owner direction, latest Unity handoff, latest project-gap report, latest continuation prompt, and continuation-platform README.
- A new governed artifact now exists at `continuation-platform/state/execution_packet.json`.
- The new `Execution` view now surfaces:
  - the active execution lane
  - the recommended next step
  - the project-work priority list
  - the current verified Unity state
  - the manual verification checklist
  - governed validation commands
  - the canonical source spine
  - governed write targets
- The governed write workbench now supports:
  - loading a real canonical project file
  - previewing a diff before write
  - applying a real canonical write after tier unlock
  - stale-source hash protection plus automatic backup capture
- Latest validated execution-packet state:
  - lane: `unity_shipping`
  - scene target: `unity/Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity`
  - recommended next step: run `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` if structural shell integrity should be re-confirmed before in-editor work
  - files mapped: `3182`
  - canonical subset documents: `19`
  - discovered registry documents: `903`
  - conflict clusters: `158`
  - frontier artifacts: `22`
  - open tasks parsed: `10`
- Governed validation passed for:
  - `python -m compileall D:\ProjectsHome\Bloodlines\continuation-platform`
  - `node --check D:\ProjectsHome\Bloodlines\continuation-platform\static\app.js`
  - `python D:\ProjectsHome\Bloodlines\continuation-platform\tests\smoke_test.py`
  - `GET /api/bootstrap`
  - `GET /api/execution-packet`
  - `POST /api/project-file`
  - `POST /api/project-write/preview`
- Detailed report preserved at `reports/2026-04-16_continuation_platform_execution_packet_and_governed_write_workbench.md`.

## 2026-04-16 Continuation Platform Quality-Of-Life Status

- The continuation platform preserved the same governed local architecture and added a contained operator-flow pass.
- New operator-speed behavior now live in the GUI:
  - last active view persists across refreshes and relaunches
  - active-view filtering narrows large list, table-row, detail, workspace, and timeline surfaces without leaving the current view
  - quick-jump controls move directly to Resume Anchor, Next Work, High Signal, Handoff Prompt, and Telemetry
  - direct copy actions exist for the recommended next step, handoff prompt, and handoff preview
  - manual resume-anchor overrides can now be cleared from the dashboard
  - rescan, resume, unlock, export, and copy flows now report through in-app toast feedback
- Latest validated QoL-pass platform scan state:
  - 3176 files mapped in the governed source scope
  - 14 canonical subset documents ingested
  - 22 frontier artifacts detected
  - 894 discovered registry documents scored
  - 158 conflict clusters discovered
  - 23 conflict clusters promoted into the live diff-watch surface
  - 24 current high-signal changed documents in the generated change report
  - 13 open tasks parsed into the task board
- Latest validated resume anchor after the QoL pass:
  - `manual_edit`
  - source path: `continuity/PROJECT_STATE.json`

## 2026-04-16 Continuation Platform Status

- The offline Bloodlines continuation platform is now in a product-ready local state at `D:\ProjectsHome\Bloodlines\continuation-platform`.
- Primary Windows launch path:
  - `D:\ProjectsHome\Bloodlines\continuation-platform\launch_windows.cmd`
  - local URL: `http://127.0.0.1:8067`
- Validated live capabilities now include:
  - governed Bloodlines source mapping with Unity cache and temp noise pruned out of continuity visibility
  - authority-scored canonical registry plus broader discovered registry
  - live Unity shipping-lane `Execution` view with current packet, canonical source spine, manual verification checklist, and governed validation commands
  - first-class Diff Panel with high-signal changes, authoritative updates, and live conflict watch
  - append-only operations journal plus a dedicated Telemetry panel with task, retrieval, write-gate, degraded-mode, and scan drilldowns
  - task-aware Ollama model inventory and routing visibility
  - one live agent mode: `resume_last_good_state`
  - explicit anchor-selection support when continuity health is `attention`
  - Dashboard, Execution, Agent Workspace, Tasks, Memory, Diff Panel, Timeline, Handoff Builder, Telemetry, and Config GUI surfaces
  - locked project-write gate with `tier_insufficient` refusal when the session remains read-only plus governed project-file load, diff preview, and tier-gated canonical write apply after unlock
- Product-ready baseline scan state before the QoL pass:
  - 3179 files mapped in the governed source scope
  - 14 canonical subset documents ingested
  - 900 discovered registry documents scored
  - 161 conflict clusters discovered
  - 25 conflict clusters promoted into the live diff-watch surface
  - 21 current high-signal changed documents in the generated change report
- Latest validated platform resume run re-confirmed the Unity scene/bootstrap lane as the grounded Bloodlines continuation path, and competing anchors now surface for explicit operator selection instead of silent auto-resolution.

## 2026-04-16 Graphics Lane Continuation Status

- The dedicated Bloodlines graphics lane is now grounded through eight additive first-pass concept batches under `D:\ProjectsHome\Bloodlines\14_ASSETS\GRAPHICS_PIPELINE\02_FIRST_PASS_CONCEPT`.
- Batch 07 completed settlement-support and water-support follow-ups:
  - market, storehouse, and granary family board
  - housing tiers board
  - well and water-support board
  - dock, ferry, and landing board
  - covenant-site progression board
- Batch 08 completed settlement-variant and covenant-overlay follow-ups:
  - House overlay support structures board
  - market and trade yard variants board
  - storehouse and granary variants board
  - housing cluster and courtyard variants board
  - covenant overlay architecture variants board
- All Batch 07 and Batch 08 SVG sources are mirrored into `D:\ProjectsHome\Bloodlines\unity\Assets\_Bloodlines\Art\Staging\ConceptSheets`.
- All Batch 07 and Batch 08 PNG review boards exist in `D:\ProjectsHome\Bloodlines\unity\Assets\_Bloodlines\Art\Staging\ConceptSheetsRasterized`.
- The approved browser-first raster path was used directly through local headless Edge export to keep Unity staging current while preserving the canonical editor state.
- `unity/Assets/_Bloodlines/Code/Editor/GraphicsVisualTestbedPopulate.cs` now contains:
  - the Batch 07 support-structure wall for `VisualReadability_Testbed.unity`
  - the Batch 08 settlement-variant wall for `MaterialLookdev_Testbed.unity`
- The governed populate path has now been rerun successfully, so those new board walls are present in the saved Unity testbed scenes on disk.
- A formal graphics review ledger and machine-readable review registry now exist at:
  - `13_AUDIO_VISUAL/GRAPHICS_PIPELINE/GRAPHICS_BATCH_REVIEW_LEDGER_2026-04-16.md`
  - `13_AUDIO_VISUAL/GRAPHICS_PIPELINE/MANIFESTS/concept_batch_review_registry.json`
- Latest graphics-lane reports:
  - `reports/2026-04-16_graphics_lane_batch_07_settlement_support_and_water_followups.md`
  - `reports/2026-04-16_graphics_lane_batch_08_settlement_variants_and_covenant_overlays.md`
  - `reports/2026-04-16_graphics_lane_batch_08_testbed_refresh_and_review_registry.md`

## Graphics Lane Next Action

1. Review Batches 01 through 08 together using `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/preview.html`, the Unity testbeds, and `13_AUDIO_VISUAL/GRAPHICS_PIPELINE/VISUAL_REVIEW_MATRIX_2026-04-16.md`.
2. Use `13_AUDIO_VISUAL/GRAPHICS_PIPELINE/GRAPHICS_BATCH_REVIEW_LEDGER_2026-04-16.md` and `13_AUDIO_VISUAL/GRAPHICS_PIPELINE/MANIFESTS/concept_batch_review_registry.json` to record the first formal `approved`, `revise`, or `replace` outcomes.
3. Prioritize formal review of:
  - the nine-House military wall
  - the Batch 07 support-structure wall
  - the Batch 08 settlement-variant and covenant-overlay wall
4. Continue the next graphics-only wave only after those review calls are made:
  - House-specific civic overlays
  - trade-hub upgrade-state boards
  - water-infrastructure replacement candidates
  - covenant-specific sacred-complex follow-ups

## Immediate Next Action Priority

1. Launch the continuation platform from `D:\ProjectsHome\Bloodlines\continuation-platform\launch_windows.cmd` and work from the `Command Deck` main screen at `http://127.0.0.1:8067`.
2. If a fresh frontier session must be opened, use `03_PROMPTS/CONTINUATION_PROMPT_2026-04-16_SESSION_111.md` as the canonical ready-to-paste resume prompt.
3. Use `docs/plans/2026-04-16-bloodlines-finalization-execution-checklist.md` as the authoritative ladder for what still remains between the current first-shell state and a genuinely finished Bloodlines production posture.
4. If Continuity Health shows `attention`, select the intended resume anchor in the Resume Anchor card or use `/anchor <candidate>`, then run `Resume Last Good State` or `/resume`. If the dashboard shows `ready`, run `Resume Last Good State` directly and confirm the anchor plus provenance set before starting new Bloodlines work.
5. Use the Command Deck first for local orientation:
   - `/status` for current posture
   - `/search <query>` for canon retrieval
   - `/read <path>` for local file loading
   - natural-language prompts for grounded Bloodlines continuation answers
6. Open the `Execution` view and confirm the live packet still resolves to the Unity shipping lane, `Bootstrap.unity`, the current canonical source spine, and the latest grounded recommendation before starting new work.
7. Follow the currently grounded project recommendation inside the Unity shipping lane:
   - run `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` if structural shell integrity should be re-confirmed before in-editor work
   - run `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` if a governed runtime preflight is needed; it now proves two-deep queue issue, rear-entry queue cancel-and-refund, surviving front-entry `command_hall -> villager` completion, `dwelling` construction completion to `24` population cap, and `barracks -> militia` continuity from a newly completed constructed production building, ending at `11` total buildings, `18` total units, and `8` controlled units. Do not run multiple Unity wrappers in parallel against the same project
   - run `scripts/Invoke-BloodlinesUnitySyncJsonContent.ps1` if the canonical JSON mirror should be refreshed before editor work
   - in Unity 6.3 editor, open `Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity` and confirm the canonical map reference remains assigned
   - enter Play Mode, verify live bootstrap entity creation plus debug presentation plus battlefield camera behavior
   - verify the debug command shell through unit select, building select, drag-box selection, `1` select-all, `Ctrl+2-5` save, `2-5` recall, `F` frame, clear-selection, formation-aware right-click move, the first production panel, queue cancel buttons, worker construction-panel visibility, build-placement feedback, and production access on newly completed constructed buildings
   - manually verify `command_hall -> villager` two-deep queue issue, rear-entry cancel, refund, and surviving front-entry completion feel even though the governed runtime smoke is already green for that slice
   - manually verify `dwelling` construction selection, pending-placement mode, obstruction feedback, right-click placement, completion timing, and final population-cap increase even though the governed runtime smoke is already green for that slice
   - manually verify `barracks` construction completion, post-completion selection, `militia` queue visibility, training completion, and post-training controlled-unit growth even though the governed runtime smoke is already green for that slice
   - verify the new `ControlPointCaptureSystem` by forcing uncontested capture, contested decay, and post-capture uncontested trickle behavior
   - continue directly into broader construction-roster verification, construction progress UI, deeper build-placement UX, broader production-roster verification, production from additional newly completed buildings, explicit attack orders, or deeper combat shell work after that first shell is confirmed
8. If continuity files need updating during the next slice, use the governed write workbench or Command Deck draft-apply flow after the required unlock tier instead of external scratch edits.
9. When handing work back to Claude Code or Codex, use the Handoff Builder view or `Export Handoff` action so the local continuity delta, doctrine, open work, and canonical pointers are packaged before frontier re-entry.

## Unity Lane Status Update (Session 112)

The Unity first-shell now carries the canonical RTS primary economy loop end to end.

- `Components/WorkerGatherComponent.cs` is new: adds `WorkerGatherPhase` (Idle, Seeking, Gathering, Returning, Depositing) and a `WorkerGatherComponent` that stores assigned node, assigned resource id, carry resource id, carry amount, carry capacity, gather rate, phase, gather radius, and deposit radius.
- `Systems/WorkerGatherSystem.cs` is new: advances every controlled worker each simulation tick through Seeking (move toward node), Gathering (decrement node amount, accrue carry at the unit's canonical gatherRate), Returning (move to the nearest alive, completed, owned `command_hall`), Depositing (accrue carry into the faction's `ResourceStockpileComponent` field matching the carry resource id), then resumes on the same node if it still has resources. Handles all seven canonical primaries: gold, wood, stone, iron, food, water, influence.
- `Debug/BloodlinesDebugCommandSurface.cs` extended with:
  - new `TryDebugAssignSelectedWorkersToGatherResource(resourceId)` debug API that filters the current selection to workers, finds the nearest resource node of the given type, and assigns each a fresh `WorkerGatherComponent` populated from the unit's canonical `carryCapacity` and `gatherRate`
  - new `TryDebugGetFactionStockpile` API exposing all seven primary resource pools for governed sampling
  - new `GetControlledWorkersWithActiveGatherCount` for diagnostic counts of non-idle gatherers on the controlled faction
  - new internal `TryFindNearestControlledResourceNode` helper
- `Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs` extended:
  - new state fields `gatherResourceId` (default "gold"), `gatherAssigned`, `gatherAssignedWorkerCount`, `gatherInitialFactionGold`, `gatherMinimumDepositAmount` (default 5), `gatherDepositObserved`, `gatherLatestFactionGold`, `gatherAssignedUtcTicks`, `gatherCycleTimeoutSeconds` (default 40)
  - new `ProbeWorkerGatherCycle` helper runs after constructed `barracks -> militia` completion, snapshots the controlled faction's gold, assigns controlled workers to gather gold, and waits for the stockpile to increase by at least the minimum threshold within the configured cycle timeout
  - final success diagnostics now carries `gatherResource`, `gatherAssigned`, `gatherAssignedWorkerCount`, `gatherInitialFactionGold`, `gatherLatestFactionGold`, `gatherDepositObserved`
  - `StartupTimeoutSeconds` raised from 75 to 120 to accommodate the longer governed sequence
- `unity/Assembly-CSharp.csproj` updated so the dotnet build path picks up the new component and system files without waiting for Unity's generator refresh.
- `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` passed. Success line ends with `gatherResource='gold'`, `gatherAssigned=True`, `gatherAssignedWorkerCount=5`, `gatherInitialFactionGold=45.0`, `gatherLatestFactionGold=55.0`, `gatherDepositObserved=True` alongside preserved production and construction progress fields.
- `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` passed.
- `node tests/data-validation.mjs` and `node tests/runtime-bridge.mjs` both passed.
- Sessions 100-108 + full canonical corpus + Unity source + ScriptableObject data + continuation-platform source now pushed to origin `lance-fisher/lancewfisher-v2` master (commits `a72982c`, `087d7b0`, `f3dd374`, `de003c9`, Session 112 commit forthcoming).
- Detailed handoff for this increment:
  - `docs/unity/session-handoffs/2026-04-16-unity-worker-gather-deposit-primary-loop.md`

## Unity Lane Status Update (Session 108)

The Unity first-shell production lane now carries mid-production observability that matches the Session 107 construction-lane pattern:

- `Debug/BloodlinesDebugEntityPresentationBridge.cs` now renders a world-space production progress bar:
  - new per-entity track + fill cubes cached in a `ProductionProgressProxy` map
  - new `SyncProductionProgressBar` pass invoked from `SyncBuildings` when a building is not under construction and has at least one active `ProductionQueueItemElement`; fill width tracks `1 - queue[0].RemainingSeconds / queue[0].TotalSeconds`
  - new `RemoveStaleProductionProgressProxies` pass mirrors the construction cleanup so completed or destroyed production buildings drop their bars
  - `ClearAllProxies` now also disposes outstanding production progress proxies across world rebinds
  - new serialized fields `presentProductionProgress`, `productionProgressTrackColor`, `productionProgressFillColor`, `productionProgressBarWidth`, `productionProgressBarHeight`, and `productionProgressBarVerticalOffset`; defaults use a distinct blue fill so production and construction bars read separately when both are visible
- `Debug/BloodlinesDebugCommandSurface.cs` now exposes `TryDebugGetSelectedProductionProgress(out ratio, out remaining, out total, out unitId, out buildingTypeId)` so governed validators can sample the selected building's live queue head progress without duplicating entity queries
- `Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs` now proves mid-production queue progress between cancel-and-refund verification and the final spawn check:
  - new state fields for initial sample ratio, remaining and total seconds, initial sample utc ticks, advancement-verified flag, latest sample ratio and remaining seconds, minimum advancement ratio default `0.08`, and advancement wait seconds default `1.25`
  - new `ProbeProductionProgress` helper mirrors `ProbeConstructionProgress`, re-selecting the controlled production building, asserting ratio in `[0, 1]`, total seconds positive, building type and unit type match expected, and advancement by at least the configured minimum within the configured wait window
  - the pre-existing spawn-floor gate and the strict phase-equality gate now carry a `midProductionObservationWindow` bypass so the probe can sample live progress before the queued villager spawns without tripping the floor or equality checks; other counts remain strictly validated
  - final success diagnostics now carries `productionProgressInitialRatio`, `productionProgressLatestRatio`, and `productionProgressAdvancementVerified` alongside the existing production and construction summaries
- `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` passed again after the production progress slice landed.
- `artifacts/unity-bootstrap-runtime-smoke.log` now ends with `productionProgressInitialRatio=0.004`, `productionProgressLatestRatio=0.084`, `productionProgressAdvancementVerified=True`, `constructionProgressInitialRatio=0.001`, `constructionProgressLatestRatio=0.915`, `constructionProgressAdvancementVerified=True`, `11` total buildings, `18` total units, `8` controlled units, `populationCap=24`, `constructedProductionBuildingType='barracks'`, and `constructedProductionUnitType='militia'`.
- `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` passed again.
- `node tests/data-validation.mjs` and `node tests/runtime-bridge.mjs` both passed.
- Detailed handoff for this increment:
  - `docs/unity/session-handoffs/2026-04-16-unity-production-progress-observability-and-world-space-bar.md`

## Unity Lane Status Update (Session 107)

The Unity first-shell construction lane is now deeper than the Session 104 snapshot:

- `Debug/BloodlinesDebugCommandSurface.cs` now renders a selection-aware construction-progress panel:
  - new `SelectedConstructionSiteSnapshot` + `TryGetSelectedConstructionSiteSnapshot` helper
  - new `DrawConstructionProgressPanel` showing display name, percent complete, remaining and total seconds, a horizontal progress bar, and an integrity readout
  - new `TryDebugSelectControlledConstructionSite(buildingTypeId)` and `TryDebugGetSelectedConstructionProgress(out ratio, out remaining, out total, out typeId)` debug API for governed validators
  - new `TrySelectControlledConstructionSite` internal selector that filters on `ConstructionStateComponent` presence so the panel targets active sites even when a completed building of the same type exists
  - new serialized fields `showConstructionProgressPanel`, `constructionProgressPanelSpacing`, `constructionProgressFillColor`, and `constructionProgressTrackColor` so the panel is toggleable and themable without code edits
- `Debug/BloodlinesDebugEntityPresentationBridge.cs` now renders a world-space construction progress bar:
  - new per-entity track + fill cubes cached in a `ConstructionProgressProxy` map
  - new `SyncConstructionProgressBar` pass invoked from `SyncBuildings` when a building has `ConstructionStateComponent`
  - new `RemoveStaleConstructionProgressProxies` pass mirroring the existing proxy cleanup so completed or destroyed construction sites drop their bars
  - `ClearAllProxies` now also disposes outstanding construction progress proxies across world rebinds
  - new serialized fields `presentConstructionProgress`, `constructionProgressTrackColor`, `constructionProgressFillColor`, `constructionProgressBarWidth`, `constructionProgressBarHeight`, `constructionProgressBarVerticalOffset`
- `Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs` now proves mid-construction progress between placement and completion:
  - new state fields for initial sample ratio, remaining and total seconds, initial sample utc ticks, advancement-verified flag, latest sample ratio and remaining seconds, minimum advancement ratio default `0.08`, and advancement wait seconds default `1.25`
  - new `ProbeConstructionProgress` helper selects the active site via the new debug API, asserts ratio in `[0, 1]`, asserts total seconds positive, asserts the observed building type matches expected, records the initial sample, then on subsequent probes asserts the ratio has advanced by the minimum margin within the wait window (or at least positively once the window has elapsed)
  - final success diagnostics now carries `constructionProgressInitialRatio`, `constructionProgressLatestRatio`, and `constructionProgressAdvancementVerified` alongside the existing construction summary
- `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` passed again after the progress-observability slice landed.
- `artifacts/unity-bootstrap-runtime-smoke.log` is now green for the stronger path, ending with `constructionProgressInitialRatio=0.002`, `constructionProgressLatestRatio=0.916`, `constructionProgressAdvancementVerified=True`, `11` total buildings, `18` total units, `8` controlled units, `populationCap=24`, `constructedProductionBuildingType='barracks'`, and `constructedProductionUnitType='militia'`.
- `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` passed again.
- `node tests/data-validation.mjs` and `node tests/runtime-bridge.mjs` both passed.
- Detailed handoff for this increment now includes:
  - `docs/unity/session-handoffs/2026-04-16-unity-construction-progress-observability-and-ui.md`

## Unity Lane Status Update (Session 104)

The Unity battlefield shell is now deeper than the Session 103 snapshot:
- `Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs` now proves constructed production continuity after the first `dwelling` slice:
  - worker-led `barracks` placement
  - barracks construction completion
  - controlled `barracks` selection after completion
  - `militia` queue issuance from the newly completed building
  - queue drain and final spawned-unit verification from that same production seat
- The validator timeout was raised from `45s` to `75s` so the longer governed sequence can finish in batch mode, and the older `command_hall -> villager` phase checks were gated correctly so later `barracks -> militia` growth does not trigger a false mismatch.
- `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` passed again after the constructed production continuity slice landed.
- `artifacts/unity-bootstrap-runtime-smoke.log` is now green for the stronger combined path, ending with `11` total buildings, `18` total units, `8` controlled units, `populationCap=24`, `constructedProductionBuildingType='barracks'`, and `constructedProductionUnitType='militia'`.
- `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` passed again after the validator extension landed.
- `node tests/data-validation.mjs` and `node tests/runtime-bridge.mjs` both passed after the constructed production continuity slice landed.
- Detailed handoff for this increment now includes:
  - `docs/unity/session-handoffs/2026-04-16-unity-constructed-barracks-production-continuity.md`
- Ready-to-paste next-session resume message now exists at:
  - `03_PROMPTS/CONTINUATION_PROMPT_2026-04-16_SESSION_104.md`

## Unity Lane Status Update (Session 100)

The Unity production lane is now deeper than the Session 99 snapshot:
- `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` now passes with first-production assertions, not just command-shell assertions.
- `UnitDefinition.cs` and `JsonContentImporter.cs` now preserve Unity-side production-gating fields from canonical JSON:
  - `movementDomain`
  - `faithId`
  - `doctrinePath`
  - `ironmarkBloodPrice`
  - `bloodProductionLoadDelta`
- The first production runtime layer now exists in code:
  - `Components/FactionHouseComponent.cs`
  - `Components/ProductionComponents.cs`
  - `Systems/UnitProductionSystem.cs`
- `SkirmishBootstrapSystem.cs` now seeds faction house identity plus per-building production queues.
- `BloodlinesDebugCommandSurface.cs` now supports controlled-building selection, a production panel, and queue issuance for the first training slice.
- `BloodlinesDebugEntityPresentationBridge.cs` now gives selected buildings a visible debug highlight.
- `UnitProductionSystem.cs` now resolves queued training through `EndSimulationEntityCommandBufferSystem` instead of invalid structural changes during iteration.
- The governed runtime-smoke lane now proves:
  - bootstrap spawning against canonical map counts
  - debug presentation bridge presence
  - command-shell select-all, control-group save or recall, and framing
  - controlled `command_hall` selection
  - `villager` production queue issuance
  - queue drain
  - post-production totals of `17` units and `7` controlled units
- `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` passed again after the production slice landed.
- `scripts/Invoke-BloodlinesUnitySyncJsonContent.ps1` passed after the new definition fields and importer work.
- `node tests/data-validation.mjs` and `node tests/runtime-bridge.mjs` both passed after the Unity production slice landed.
- Detailed handoff for this increment now includes:
  - `docs/unity/session-handoffs/2026-04-16-unity-first-production-slice.md`

## Unity Lane Status Update (Session 103)

The Unity battlefield shell is now deeper than the Session 102 snapshot:
- `Components/ConstructionComponents.cs` and `Systems/ConstructionSystem.cs` now provide the first ECS construction-progress lane for placed buildings.
- `Debug/BloodlinesDebugCommandSurface.cs` now exposes a worker-aware construction panel, pending-placement mode, right-click placement, obstruction checks, and a governed debug construction hook.
- `Debug/BloodlinesDebugEntityPresentationBridge.cs` now renders under-construction buildings distinctly, and `Systems/UnitProductionSystem.cs` now blocks under-construction production facilities from acting as live production seats.
- `Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs` now proves the first construction seam after the governed two-deep production path:
  - `dwelling` placement near a controlled worker
  - building-count increase from `9` to `10`
  - construction-site completion
  - final population-cap increase from `18` to `24`
- `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` passed again after the construction slice landed.
- `artifacts/unity-bootstrap-runtime-smoke.log` is now green for both the deeper production proof and the first construction proof, ending with `constructionPlaced=True`, `constructionSites=0`, `10` total buildings, `17` total units, `7` controlled units, and `populationCap=24`.
- `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` passed again after the construction slice landed.
- `node tests/data-validation.mjs` and `node tests/runtime-bridge.mjs` both passed after the construction slice landed.
- Detailed handoff for this increment now includes:
  - `docs/unity/session-handoffs/2026-04-16-unity-first-construction-slice.md`

## Unity Lane Status Update (Session 102)

The Unity production lane is now deeper than the Session 101 snapshot:
- `Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs` now proves the first production shell through a stronger queue-depth sequence:
  - controlled `command_hall` selection
  - first `villager` queue issuance
  - second `villager` queue issuance
  - rear-entry cancellation at queue index `1`
  - refund verification against the stored queue payload while the front entry remains active
  - surviving front-entry completion and final spawned-unit verification
- `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` passed again after the deeper queue-depth validation landed.
- `artifacts/unity-bootstrap-runtime-smoke.log` remains green and now corresponds to the stronger two-deep queue proof with `productionCancelVerified=True`, `17` total units, and `7` controlled units.
- `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` passed again after the deeper queue-depth validation landed.
- `node tests/data-validation.mjs` and `node tests/runtime-bridge.mjs` both passed after the deeper queue-depth validation landed.
- Detailed handoff for this increment now includes:
  - `docs/unity/session-handoffs/2026-04-16-unity-two-deep-production-queue-tail-cancel-validation.md`

## Unity Lane Status Update (Session 101)

The Unity production lane is now deeper than the Session 100 snapshot:
- `Components/ProductionComponents.cs` now stores refund-safe queued production metadata, including queued resource costs plus Ironmark blood-price and blood-load fields.
- `Debug/BloodlinesDebugCommandSurface.cs` now renders per-entry queue rows, exposes cancel buttons, refunds queued resource and population spend on cancellation, and exposes a debug cancel hook for governed validation.
- `Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs` now proves the full first queue-control sequence:
  - controlled `command_hall` selection
  - `villager` queue issuance
  - queued-entry cancellation before completion
  - refund-delta verification against the stored queue payload
  - final re-queue and successful spawned-unit completion
- `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` passed again after the queue-cancel slice landed.
- `artifacts/unity-bootstrap-runtime-smoke.log` now preserves `productionCancelVerified=True` in the successful governed runtime-smoke line.
- `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` passed again after the queue-cancel slice landed.
- `node tests/data-validation.mjs` and `node tests/runtime-bridge.mjs` both passed after the queue-cancel slice landed.
- Detailed handoff for this increment now includes:
  - `docs/unity/session-handoffs/2026-04-16-unity-production-queue-cancel-and-refund.md`

## Session 97 Next Action Priority

### Browser Lane (if continuing world depth)

1. Add AI harbor construction and naval unit production so Stonehelm can build harbors, train vessels, and contest water.
2. If the AI naval lane blocks, deepen trade-route naval interdiction, naval fog-of-war, or match-stage polish.
3. If all browser lanes are deprioritized, open the Unity Play Mode verification shell.

### Unity Lane (if pivoting to the production shell)

1. Open `Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity` in Unity 6.3 LTS.
2. Confirm the Bootstrap authoring object still references `Assets/_Bloodlines/Data/MapDefinitions/ironmark_frontier.asset`.
3. Enter Play Mode and verify that:
   - the map bootstrap authoring path creates live faction, building, unit, resource-node, settlement, and control-point entities
   - the debug presentation bridge renders visible battlefield markers
   - the battlefield camera controller pans, rotates, and zooms correctly
4. Verify that the debug command shell behaves correctly for:
   - left-click single select
   - left-drag box select
   - `1` select all controlled units
   - `Ctrl+2-5` save control groups
   - `2-5` recall control groups
   - `F` frame selection
   - `Escape` clear selection
   - right-click formation-aware move issuance
5. Verify that `ControlPointCaptureSystem` behaves correctly for:
   - uncontested neutral capture
   - contested progress decay
   - owner stabilization
   - post-capture uncontested resource trickle
6. Open `Assets/_Bloodlines/Scenes/Gameplay/IronmarkFrontier.unity` and confirm the generated shell is still intact after the definition-binding repair and validator-coverage pass.
7. Continue immediately into construction, production, explicit attack orders, or deeper combat shell work after that first live shell is confirmed.

### Sessions 88-96 Completed Work

- **Session 88**: Alliance-threshold coalition pressure is now LIVE with full save/restore, snapshot exposure, dynasty-panel legibility, world-pill HUD exposure, and runtime bridge test coverage. The previously unreported "Session 89" code has been documented and its continuity gaps repaired.
- **Session 89**: Three new interactive acceptance factors are now LIVE:
  - Conviction modifier: Apex Moral +4, Moral +2, Cruel -2, Apex Cruel -4.
  - Covenant endorsement: Passed Covenant Test +3, Grand Sanctuary +2.
  - Tribal friction: Active tribal raiders impose up to -4 friction (0.8 per raider, capped at -4).
  - Dynasty panel shows "World stance" detail line for the new factors.
  - 8 new runtime bridge test assertions cover conviction, covenant, and tribal effects.
- **Session 90**: Non-aggression pact diplomacy is now LIVE:
  - `proposeNonAggressionPact`: Costs 50 influence + 80 gold, removes mutual hostility, reduces alliance-threshold hostile count.
  - `breakNonAggressionPact`: Restores hostility, costs 8 legitimacy + 2 conviction.
  - Holy wars block pact formation. Minimum 180 in-world day duration.
  - Dynasty panel shows active pacts under a "Diplomacy" section.
  - Save/restore preserves pact state and de-hostilized status.
  - 13 new runtime bridge test assertions cover full pact lifecycle.
- **Session 91**: AI non-aggression pact awareness is now LIVE:
  - Stonehelm proposes pacts when under succession crisis, critically low army, or player governance near victory.
  - Timer-based: 120s initial, 90s retry, 300s cooldown after success.
  - 4 new runtime bridge test assertions for AI proposal, record creation, hostility removal, message output.
- **Session 92**: Player-facing pact diplomacy UI is now LIVE:
  - Dynasty panel shows "Diplomacy" section with propose buttons for hostile kingdoms and break buttons for active pacts.
  - Buttons use `getNonAggressionPactTerms`, `proposeNonAggressionPact`, and `breakNonAggressionPact` from the shared simulation layer.
  - Browser-verified: zero console errors, zero failed requests, Diplomacy section renders with correct availability and cost information.
- **Session 93**: Trueborn City neutral-faction foundation is now LIVE:
  - `trueborn_city` faction with trade-relationship tracking.
  - Standing updates each realm cycle from conviction (+1.5 Apex Moral to -1.5 Apex Cruel), legitimacy, hostility count, and active pacts.
  - Standing feeds into acceptance as `truebornEndorsement` (standing * 0.25, capped [-3, +5]).
  - Save/restore preserves trade relationships. Dynasty panel shows Trueborn endorsement in "World stance" line.
  - 9 new runtime bridge test assertions.
- **Session 94**: Trueborn Rise arc is now LIVE:
  - Three-stage escalation: Claims (passive loyalty -0.8/cycle), Recognition (loyalty -1.8 + legitimacy -0.6/cycle), Restoration (loyalty -3.2 + legitimacy -1.4/cycle).
  - Activation after 8 in-world years of low challenge from kingdoms.
  - Challenge level from territory count, standing, and hostility.
  - HUD world pill shows stage label when active.
  - Save/restore preserves full riseArc state.
  - 10 new runtime bridge test assertions.
- **Session 95**: Trueborn recognition diplomacy is now LIVE:
  - `recognizeTruebornClaim`: 40 influence + 60 gold + 5 legitimacy. Grants +6 standing. Reduces Rise pressure 75%.
  - Dynasty panel shows "Recognize Trueborn Claim" action when Rise is active.
  - Recognized kingdoms receive 0.25x Rise loyalty/legitimacy pressure.
  - 10 new runtime bridge test assertions.
- **Session 96**: Naval world integration is now LIVE:
  - Vessels have a dedicated `updateVessel` tick path for movement, combat, and fishing gather.
  - Fishing boats auto-gather food on water tiles. War galleys, fire ships, capital ships resolve naval combat.
  - Fire ships detonate on first strike via `oneUseSacrifice`.
  - Vessels spawn on water tiles adjacent to producing harbors.
  - 7 new runtime bridge test assertions.
- All validation suites pass: data validation, runtime bridge (including 70 new assertions across Sessions 88-96), all syntax checks.

## Player Guide Status Update

- A new foundational player-facing manual now exists at `18_EXPORTS/BLOODLINES_PLAYER_GUIDE_FOUNDATIONS.md`.
- This file is the canonical starting point for future player-guide or player-facing copy work. Do not restart from raw ingestion or internal bible prose unless new canon must be reconciled first.
- The guide currently covers:
  - what Bloodlines is and how it differs from a simpler RTS
  - a dedicated "why these systems matter" onboarding layer
  - a dedicated living-realm primer covering population, food, water, housing, bloodline rule, governance, loyalty, and territorial command
  - a dedicated bloodline-members primer covering succession, commanders, governors, envoys, and bloodline risk
  - a dedicated warfare primer covering campaign rhythm, siege preparation, fortification value, and tactical consequence
  - a dedicated governance primer covering loyalty, stabilization, overexpansion, and holding territory through real rule
  - core principles of play
  - a dedicated Faith versus Conviction explanation
  - first-Dynasty guidance
  - full primers for Trueborn, Highborne, Ironmark, Goldgrave, Stonehelm, Westland, Hartvale, Whitehall, and Oldcrest
  - dynasty comparison, first-match guidance, post-battle consequence guidance, common mistakes, and future manual volumes
- If future documentation work is requested, the highest-value next additions are:
  - advanced economy guide
  - faith guide
  - conviction guide
  - faith and conviction interaction guide
  - bloodline member strategy guide
  - warfare doctrine guide
  - territorial governance guide
  - dynasty matchup guide

## Unity Lane Status Update (Session 99)

The Unity production lane is now deeper and better verified than the Session 98 snapshot:
- `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` is now green and preserved in `artifacts/unity-bootstrap-runtime-smoke.log`.
- The governed runtime-smoke lane now validates:
  - live bootstrap spawning against canonical map counts
  - debug presentation bridge presence
  - command-shell select-all
  - control-group 2 save/recall
  - selection framing
- The first live-shell runtime blockers were repaired:
  - `BloodlinesMapBootstrapAuthoring` now injects canonical bootstrap runtime data into Play Mode when needed
  - authoring-side buffer writes are safe across structural changes
  - `SkirmishBootstrapSystem` now snapshots seed buffers before entity creation
  - `PositionToLocalTransformSystem` now runs in `SimulationSystemGroup` after `UnitMovementSystem`
- `BloodlinesDebugCommandSurface` now extends beyond selection and move with:
  - a compact battlefield HUD
  - control groups `2` through `5`
  - `Ctrl+2-5` save
  - `2-5` recall
  - `F` frame selection or controlled-force fallback
- `BloodlinesBattlefieldCameraController` now exposes a focus method so the command shell can reframe the battlefield camera coherently.
- `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` still passes after the command-shell changes.
- `dotnet build unity/Assembly-CSharp.csproj -nologo` still passes with 0 warnings and 0 errors.
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` still passes with 0 errors and the same longstanding 105 `CS0649` warnings.
- Detailed session notes for this increment now include:
  - `docs/unity/session-handoffs/2026-04-16-unity-bootstrap-runtime-green-and-command-groups.md`
  - `docs/unity/session-handoffs/2026-04-16-unity-canonical-scene-shell-validation.md`

## Unity Lane Status Update (Session 98)

The Unity production lane is now materially deeper:
- Unity 6.3 LTS (6000.3.13f1) first-open completed via batch mode.
- `Bloodlines -> Import -> Sync JSON Content` executed successfully.
- 119 ScriptableObject assets generated across 12 categories (41 units, 23 buildings, 9 factions, 9 bloodline roles, 7 bloodline paths, 7 resources, 6 settlement classes, 6 victory conditions, 5 conviction states, 4 faiths, 1 map, 1 realm condition config).
- First movement foundation now exists in code:
  - `MoveCommandComponent`
  - `MovementStatsComponent`
  - `Bloodlines.Pathing.UnitMovementSystem`
  - `Bloodlines.Pathing.PositionToLocalTransformSystem`
- First map/bootstrap bridge now exists in code:
  - `BloodlinesMapBootstrapAuthoring`
  - `BloodlinesMapBootstrapBaker`
  - `MapBootstrapComponents`
  - `SkirmishBootstrapSystem`
- First governed scene-shell creation path now exists:
  - `BloodlinesGameplaySceneBootstrap`
  - menu: `Bloodlines -> Scenes -> Create Bootstrap And Gameplay Scene Shells`
  - batch wrapper: `scripts/Invoke-BloodlinesUnityCreateGameplaySceneShells.ps1`
- First battlefield camera shell now exists in code:
  - `BloodlinesBattlefieldCameraController`
- First visible ECS-shell debug presenter now exists in code:
  - `BloodlinesDebugEntityPresentationBridge`
- First interactive battlefield command shell now exists in code:
  - `BloodlinesDebugCommandSurface`
  - `SelectedTag`
- `BloodlinesDebugCommandSurface` already supports single-select, drag-box selection, select-all, clear-selection, and formation-aware right-click move issuance for the early ECS shell.
- First control-point ownership-and-capture foundation now exists in code:
  - `ControlPointCaptureSystem`
- Control-point runtime metadata now preserves continent and resource-trickle values, and `ControlPointResourceTrickleSystem` now provides the first ECS territory-yield lane once ownership exists.
- `ControlPointCaptureSystem` now resolves non-worker claimants, contested state, capture-progress decay, stabilization, ownership flips, and stabilized-versus-occupied transitions ahead of the territory-yield pass.
- `ControlPointData.resourceTrickle` now uses float fields in the Unity definitions, fixing the prior control-point trickle truncation bug in the Unity map lane.
- `ironmark_frontier.asset` was corrected to restore JSON-authored control-point trickle decimals and continent metadata.
- The real Unity data blocker was then diagnosed and repaired:
  - all 119 generated definition assets under `unity/Assets/_Bloodlines/Data/` had been serialized with `m_Script: {fileID: 0}`
  - each ScriptableObject definition now lives in its own correctly named file under `unity/Assets/_Bloodlines/Code/Definitions/`
  - `scripts/Invoke-BloodlinesUnitySyncJsonContent.ps1` now provides the governed batch JSON-sync path
  - `JsonContentImporter.cs` now preserves broken generated assets under `reports/unity-definition-binding-repair/` before recreating them in place
  - the latest preserved pre-repair backup root is `reports/unity-definition-binding-repair/2026-04-16-095158/`
- The repaired result is now confirmed:
  - 119 definition assets carry valid `m_Script` bindings
  - 0 definition assets remain in the old broken `m_Script: {fileID: 0}` state
- The canonical scene shells now exist on disk:
  - `unity/Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity`
  - `unity/Assets/_Bloodlines/Scenes/Gameplay/IronmarkFrontier.unity`
- `Bootstrap.unity` now carries the canonical map assignment directly:
  - `map: {fileID: 11400000, guid: d1ad0843071350c45aa1a54a2bad5b84, type: 2}`
- `BloodlinesGameplaySceneBootstrap.cs` now repairs Bootstrap map assignment by using a persistent asset-path reference and logs the resolved path cleanly in batch mode.
- `BloodlinesGameplaySceneBootstrap.cs` now also validates both canonical scene shells through governed menu and batch entry points:
  - `Bloodlines -> Scenes -> Validate Bootstrap Scene Shell`
  - `Bloodlines -> Scenes -> Validate Gameplay Scene Shell`
- Governed validation wrappers now exist for the scene-shell lane:
  - `scripts/Invoke-BloodlinesUnityValidateBootstrapSceneShell.ps1`
  - `scripts/Invoke-BloodlinesUnityValidateGameplaySceneShell.ps1`
  - `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1`
- The Bootstrap and Gameplay validation wrappers now rerun once automatically if the first Unity batch pass is consumed by editor compilation or import work before a validation outcome is logged.
- Both canonical scene shells have now passed structural batch validation:
  - `artifacts/unity-bootstrap-scene-validate.log`
  - `artifacts/unity-gameplay-scene-validate.log`
- Package-resolution note:
  - `unity/Packages/manifest.json` still pins `com.unity.collections` to `2.5.7`
  - `unity/Packages/packages-lock.json` and Unity batch logs currently resolve `com.unity.collections` as `2.6.5`
  - preserve the current stable state and do not churn packages casually without a deliberate package-alignment pass
- `dotnet build unity/Assembly-CSharp.csproj -nologo` passed through isolated Codex intermediate/output paths with 0 warnings and 0 errors.
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` passed through isolated Codex intermediate/output paths with 0 errors; longstanding `CS0649` warnings remain in existing editor/importer helpers and currently total 105 warnings.
- `unity/My project/` is now explicitly labeled as a preserved non-canonical Unity template via `unity/My project/STUB_TEMPLATE_NOTICE.md`.
- `node tests/data-validation.mjs` and `node tests/runtime-bridge.mjs` both passed again after the definition-binding repair and Bootstrap recovery work.
- `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` also completed successfully and now serves as the governed structural preflight for both canonical scenes together.
- Detailed session notes:
  - `docs/unity/session-handoffs/2026-04-16-unity-movement-foundation.md`
  - `docs/unity/session-handoffs/2026-04-16-unity-bootstrap-and-territory-yield.md`
  - `docs/unity/session-handoffs/2026-04-16-unity-scene-scaffold-camera-and-debug-shell.md`
  - `docs/unity/session-handoffs/2026-04-16-unity-control-point-capture-foundation.md`
  - `docs/unity/session-handoffs/2026-04-16-unity-selection-and-formation-move-shell.md`
  - `docs/unity/session-handoffs/2026-04-16-unity-definition-binding-and-bootstrap-repair.md`
  - `docs/unity/session-handoffs/2026-04-16-unity-canonical-scene-shell-validation.md`

## Graphics Lane Continuation Note

This note is additive and does not supersede the Session 88 runtime priority above.

If a future session is explicitly routed into the graphics lane, start here:

- `13_AUDIO_VISUAL/GRAPHICS_PIPELINE/README.md`
- `13_AUDIO_VISUAL/GRAPHICS_PIPELINE/BLOODLINES_VISUAL_PRODUCTION_BIBLE_2026-04-15.md`
- `13_AUDIO_VISUAL/GRAPHICS_PIPELINE/HOUSE_VISUAL_IDENTITY_PACKS_2026-04-15.md`
- `13_AUDIO_VISUAL/GRAPHICS_PIPELINE/UNIT_VISUAL_DIRECTION_PACKS_2026-04-16.md`
- `13_AUDIO_VISUAL/GRAPHICS_PIPELINE/BUILDING_FAMILY_DIRECTION_PACKS_2026-04-16.md`
- `13_AUDIO_VISUAL/GRAPHICS_PIPELINE/TERRAIN_AND_BIOME_DIRECTION_PACKS_2026-04-16.md`
- `13_AUDIO_VISUAL/GRAPHICS_PIPELINE/VISUAL_REVIEW_MATRIX_2026-04-16.md`
- `13_AUDIO_VISUAL/GRAPHICS_PIPELINE/MANIFESTS/`
- `03_PROMPTS/GRAPHICS_PIPELINE/`
- `14_ASSETS/GRAPHICS_PIPELINE/`
- `docs/unity/VISUAL_ASSET_PIPELINE_2026-04-15.md`
- `docs/unity/BLOODLINES_UNITY_6_3_VISUAL_IMPLEMENTATION_GUIDE_2026-04-16.md`
- `docs/unity/BLOODLINES_VISUAL_TESTBED_PLAN_2026-04-16.md`
- `reports/2026-04-15_graphics_lane_foundation.md`
- `reports/2026-04-15_graphics_lane_batch_01_first_pass_sheets.md`
- `reports/2026-04-16_graphics_lane_batch_02_environment_banners_portraits.md`
- `reports/2026-04-16_graphics_lane_batch_03_house_silhouette_sheets.md`
- `reports/2026-04-16_graphics_lane_batch_04_fortification_terrain_water_followups.md`
- `reports/2026-04-16_graphics_lane_batch_05_remaining_house_silhouette_sheets.md`
- `reports/2026-04-16_graphics_lane_batch_06_neutral_faith_civic_and_material_boards.md`
- `reports/2026-04-16_graphics_lane_unity_vector_browser_ingest.md`
- `reports/2026-04-16_graphics_lane_continuation_audit_and_unity_readiness.md`

Graphics-lane next steps only:

1. Review Batches 01 through 06 together through `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/preview.html` and the Unity-generated PNG boards under `unity/Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/`.
2. Tag the current House, building, terrain, environment, and material sheets `approved`, `revise`, or `replace` using `13_AUDIO_VISUAL/GRAPHICS_PIPELINE/VISUAL_REVIEW_MATRIX_2026-04-16.md`.
3. Use the already-populated Unity graphics testbeds to check:
   - nine-House battlefield differentiation in `VisualReadability_Testbed.unity`
   - terrain and bridge readability in `TerrainLookdev_Testbed.unity`
   - shared-versus-House material balance plus Batch 06 board wall in `MaterialLookdev_Testbed.unity`
   - icon contrast in `IconLegibility_Testbed.unity`
4. Build the next neutral and faith follow-up wave:
   - dynasty-adjacent replacement candidates for neutral settlements
   - more settled faith-structure variants
   - docks, ferries, and landing modular follow-up
5. Build the next civic-support follow-up wave:
   - granary variants
   - storehouse variants
   - market variants
   - housing and water-support follow-ups
6. Continue the material-board wave:
   - shield-face and heraldic restraint boards
   - leather-kit and siege-wood boards
   - terrain transition and road-family material boards
7. Keep all raw output in the graphics lane and Unity staging only. Use `Bloodlines -> Graphics -> Sync And Rasterize Concept Sheets` or `scripts/Invoke-BloodlinesUnityGraphicsRasterize.ps1`, and do not promote `.svg` or generated `.png` review sheets into runtime art folders until they are `integration_ready`.

Graphics-lane status update:

- Batch 01 is now complete under `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/` and mirrored into `unity/Assets/_Bloodlines/Art/Staging/ConceptSheets/`.
- Batch 02 is now also complete in that same folder and extends the preview surface with terrain, breach, banner, and portrait direction sheets.
- Batch 03 is now also complete in that same folder and adds House-specific military silhouette treatment boards for Ironmark, Stonehelm, Hartvale, and Trueborn.
- Batch 04 is now also complete in that same folder and adds fortification kit decomposition, cliff and shoreline transitions, resource-ground and edge blends, bridge or water-infrastructure sheets, and the first logistics or setpiece support sheet.
- Batch 05 is now also complete in that same folder and closes the first full nine-House silhouette-study lane by adding Highborne, Goldgrave, Westland, Whitehall, and Oldcrest.
- Batch 06 is now also complete in that same folder and adds neutral settlement structures, faith structure families, civic support variants, shared foundation material boards, and House trim-family control boards.
- Start graphics review with `preview.html` in that same folder.
- The House heraldry sheet is candidate-only and must not be treated as settled canon yet.
- The preserved Highborne silhouette filename on disk remains `bl_unit_highborne_house_military_silhouettes_sheet_a_first_pass_concept_v001.svg`; Unity staging and testbed tooling now map the canonical `highborne` House id to that preserved filename without renaming the source artifact.
- The current Unity project manifest intentionally does not include `com.unity.vectorgraphics`. A dedicated 2026-04-16 tooling pass confirmed the external package conflicts with Unity 6.3's built-in vector module surface in this project.
- A Unity editor helper now exists at `unity/Assets/_Bloodlines/Code/Editor/GraphicsConceptSheetSync.cs` for governed mirroring into `Assets/_Bloodlines/Art/Staging/ConceptSheets`.
- A Unity raster helper now also exists at `unity/Assets/_Bloodlines/Code/Editor/GraphicsConceptSheetVectorImport.cs` for governed PNG review-board generation into `Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized`.
- A Unity testbed bootstrap helper now also exists at `unity/Assets/_Bloodlines/Code/Editor/GraphicsVisualTestbedBootstrap.cs` for governed creation of visual-testbed scene shells under `Assets/_Bloodlines/Scenes/Testbeds/`.
- A Unity testbed population helper now also exists at `unity/Assets/_Bloodlines/Code/Editor/GraphicsVisualTestbedPopulate.cs` for governed generated-content population inside those testbed scenes.
- A CLI wrapper for that path now exists at `scripts/Invoke-BloodlinesUnityGraphicsPopulateTestbeds.ps1`.
- The preferred Unity staging path is now SVG source plus generated PNG review board, not SVG-only reference staging.
- Governed Unity raster refreshes have now been run successfully through Batch 06, so the House, fortification, terrain, bridge, logistics, neutral-settlement, faith-structure, civic-support, and material boards now all exist as generated PNG review surfaces as well as SVG source sheets.
- Family-level direction packs now exist for units, buildings, and terrain or biome work, plus a stricter issue-tagged review matrix and an expanded prompt-pack surface.
- Unity runtime-facing folder anchors are now explicit under `unity/Assets/_Bloodlines/Art/`, `Materials/`, `Prefabs/`, and `Scenes/Testbeds/`, but they remain mostly organizational until actual approved assets or scenes are inserted.
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` passed after the new testbed bootstrap helper was added. Existing `JsonContentImporter.cs` CS0649 warnings remain unchanged.
- The governed Unity testbed bootstrap has now been run successfully, and the four scene shells already exist under:
  - `unity/Assets/_Bloodlines/Scenes/Testbeds/VisualReadability/VisualReadability_Testbed.unity`
  - `unity/Assets/_Bloodlines/Scenes/Testbeds/TerrainLookdev/TerrainLookdev_Testbed.unity`
  - `unity/Assets/_Bloodlines/Scenes/Testbeds/MaterialLookdev/MaterialLookdev_Testbed.unity`
  - `unity/Assets/_Bloodlines/Scenes/Testbeds/IconLegibility/IconLegibility_Testbed.unity`
- The governed populate helper has now also been run successfully. Treat the four scenes as staging-only visual verification surfaces, not feature scenes, and rerun the helper only when refreshing the tool-owned `GENERATED_TESTBED_CONTENT` roots.

## 2026-04-15 Sessions 48-87 Refresh

### Current Status

- Session 48: `minor_house` AI is live. Breakaway houses now attack nearby hostile combatants threatening their claimed march, retake the march if seized, regroup toward the claim after pressure clears, and expose their current posture in the dynasty panel.
- Session 49: `restoreStateSnapshot` now rebuilds the live prefix-based counters used by runtime id creation, preventing post-restore id collisions for buildings and dynasty entities.
- Session 50: held breakaway marches now levy real local retinue by spending food and influence, consuming claim loyalty, obeying a local retinue cap, and persisting through snapshot restore.
- Session 51: mixed-bloodline children now feed live lesser-house instability. Active marriages soften that outside-house pull, and renewed hostility worsens it. The dynasty panel now surfaces mixed-bloodline drift for player cadet houses.
- Session 52: AI marriage proposal and acceptance logic now evaluates covenant and doctrine compatibility. Compatible faith can open a legitimacy-repair marriage path, fractured matches block weak one-signal diplomacy, and the dynasty panel now exposes covenant stance for pending offers.
- Session 53: real bloodline death now dissolves active marriages, applies legitimacy loss and oathkeeping mourning to both houses, stops further gestation on dissolved unions, and surfaces death-ended marriages in the dynasty panel.
- Session 54: owned marches, settlements, wells, supply camps, and linked wagons now sustain field armies; dehydration now slows armies, weakens attacks, surfaces in the logistics pill, survives restore, and can make Stonehelm delay an assault.
- Session 55: espionage now builds live rival-court intelligence reports, assassination now kills real bloodline members with legitimacy, succession, command, and hostility consequence, the dynasty panel surfaces covert pressure, and Stonehelm can retaliate through espionage and assassination.
- Session 56: missionary pressure and holy war declaration are now live faith operations. They consume faith intensity, change hostility, pressure rival territory or legitimacy, pulse ongoing zeal, surface through the faith panel, survive restore, and trigger Stonehelm faith reactivity.
- Session 57: marriage proposals and approvals now run through live household authority, require an offering envoy, apply legitimacy strain under regency, surface through the dynasty panel, survive restore, and now react to capture or succession disruption in the same dynastic role lane already used elsewhere.
- Session 58: prolonged critical dehydration now causes real field-water attrition and eventual desertion risk, commander presence buffers that collapse, the logistics surface exposes attrition and breaking-line pressure, Stonehelm recoils harder when its column is drying out, and restore now preserves the collapse timeline. Active holy war also now sustains legitimacy pressure alongside territorial pressure.
- Session 59: counter-intelligence is now live as a timed court-watch state. It lowers espionage and assassination success, adds bloodline-role shielding, records foiled covert operations, surfaces in the dynasty panel, survives restore with collision-safe ids, and now triggers Stonehelm defensive reciprocity once hostile intelligence pressure is live.
- Session 60: world pressure is now a live escalation lane. Dominant kingdoms accumulate pressure from expansion, holy war, captives, hostile operations, and dark extremes; the leader now suffers frontier-loyalty and legitimacy pressure, Stonehelm accelerates offensive tempo against that leader, tribes retarget raids toward it, the world pill exposes the state, and restore preserves the new pressure fields.
- Session 61: successful covert interceptions now create source-scoped counter-intelligence dossiers on hostile courts, preserve hostile-source watch history, surface dossier metadata in the dynasty panel, survive restore, and let Stonehelm retaliate through assassination without reopening redundant espionage first.
- Session 62: mixed-bloodline cadet branches now carry live marriage anchors that can be active, dissolved, strained, or fractured, materially changing lesser-house loyalty drift, dynasty-panel legibility, and restore continuity.
- Session 63: Hartvale is now prototype-playable through the existing house-select seam, Verdant Warden is live behind house-gated training, the command panel only surfaces units the active house can truly queue, and off-house unique-unit attempts now fail explicitly.
- Session 64: dominant world pressure now also worsens cadet-house loyalty drift, records severity on active lesser houses, surfaces cadet pressure through the dynasty panel and world pill, and survives restore.
- Session 65: hostile minor houses now exploit parent-realm world pressure through faster levy growth, higher retinue caps, sharper retake cadence, dynasty-panel legibility, world-pill legibility, and restore continuity.
- Session 66: counter-intelligence dossiers now drive sabotage target selection, sabotage subtype selection, sabotage-support bonus, dynasty-panel legibility, AI sabotage retaliation reuse, and restore continuity.
- Session 67: the player can now launch dossier-backed sabotage from the rival-court panel using live dossier target selection, shared sabotage terms, real dynasty operations, dynasty-panel legibility, and restore continuity.
- Session 68: `Convergence` world pressure now exposes a shared escalation profile, sharpens rival military, covert, and faith tempo beyond `Severe`, accelerates tribal raid cadence further, surfaces that pressure in the world pill, and survives restore.
- Session 69: Ironmark's `axeman` is now a live unique-unit lane. Barracks surfaces it only for Ironmark, queueing it consumes a heavier live blood levy and blood-production load than generic infantry, the command panel exposes that burden honestly, and restore preserves the queued unit plus resulting load.
- Session 70: Ironmark-controlled AI now recruits `axeman` through the same shared barracks gate, falls back to `swordsman` when blood-production burden is already high, surfaces both outcomes in the message log, and preserves queued Axemen through restore.
- Session 71: world pressure now resolves through a live source breakdown, the world pill exposes the leading source, tribes hard-prioritize off-home marches when continental overextension is the dominant source, and restore preserves that source identity.
- Session 72: Stonehelm now also reads that source breakdown for territorial pressure, redirects onto off-home marches when overextension is the dominant source, surfaces that redirect in the message log, and reapplies the same behavior after restore.
- Session 73: Stonehelm now also reads that source breakdown for faith backlash, compresses missionary and holy-war timing when active holy war is the dominant source, surfaces that retaliation in the message log, and preserves the timing behavior after restore.
- Session 74: Stonehelm now also reads that source breakdown for covert backlash, compresses counter-intelligence and sabotage timing when hostile operations are the dominant source, surfaces that retaliation in the message log, and preserves the timing behavior after restore.
- Session 75: Stonehelm now also reads that source breakdown for dark-extremes backlash, redirects punitive territorial pressure onto the weakest player-held marches, compresses assassination timing when live court intelligence exists, surfaces that retaliation in the message log, and preserves the behavior after restore.
- Session 76: Stonehelm now also reads that source breakdown for captive backlash, compresses captive-recovery timing when held captives are the dominant source, launches live rescue or ransom through the dynasty-operations lane, surfaces that retaliation in the message log, and preserves the behavior after restore.
- Session 77: tribes and Stonehelm now also read that source breakdown for broad territorial expansion, redirect live pressure onto the weakest stretched marches when territorial breadth itself is the dominant source, surface that backlash in the message log, expose breadth contribution through the world pill, and preserve the behavior after restore.
- Session 78: Hartvale `verdant_warden` now provides live settlement-defense and local-loyalty support, strengthens reserve recovery and muster, surfaces that support through the Loyalty and Fort dashboard pills, sharpens Stonehelm keep-assault caution, and preserves the support state through restore.
- Session 79: `scout_rider` is now live as a real stage-2 cavalry and infrastructure-raiding unit. `stable` is buildable, Scout Riders launch real raids against hostile support structures, raids cut live logistics and water support while depressing nearby hostile loyalty, Stonehelm now uses the same lane, and the command surface plus logistics pill expose the pressure honestly.
- Session 80: `scout_rider` now also harasses worked hostile resource seams, routes nearby workers to refuge, depresses local hostile march loyalty around the seam, exposes harried seams and routed workers in the logistics pill, triggers Stonehelm local counter-raids, and preserves the new harassment state through restore.
- Session 81: `scout_rider` now also intercepts moving hostile `supply_wagon`, strips convoy stores, forces convoy retreat, cuts already-live siege and field-water sustainment, shocks nearby hostile march loyalty, exposes convoy cuts in the logistics pill, triggers Stonehelm convoy targeting plus local counter-screen response, and preserves convoy-interdiction state through restore.
- Session 82: convoy escort discipline and post-interdiction reconsolidation are now live. Escort units carry persistent `escortAssignedWagonId` binding. AI now differentiates unscreened (pulls back) from screened (holds stage point) recovering convoys. `readyForFormalAssault` requires `convoyReconsolidated`. Logistics snapshot and pill expose escort discipline state. All new state survives snapshot restore.
- Session 83: the Session 82 match-structure, time-system, and multiplayer doctrine ingestion is now reflected in live runtime. Match progression now computes a readiness-gated five-stage state from real simulation conditions, the Cycle pill and debug surfaces expose stage or phase or year context, `dualClock` and `matchProgression` survive restore, and Stonehelm now respects stage-aware early-war territorial and cavalry restraint unless already-live escalation systems have honestly opened the war.
- Session 84: imminent-engagement warnings are now live around threatened dynastic keeps, with real reinforcement, posture, commander-recall, and emergency bloodline-guard responses. Stage 5 Divine Right declarations are also now live with apex-structure and recognition-share gating, rival AI coalition response, restore continuity, and real victory or failure resolution.
- Session 85: the first political-event architecture is now live through `Succession Crisis`. Ruling bloodline death can now trigger an active dynastic crisis with claim-strength weighting, severity escalation, real loyalty or legitimacy or economy or stabilization or combat penalties, a real `Consolidate Succession` action, rival AI aggression response, enemy self-resolution, and restore continuity.
- Session 86: `Covenant Test` is now live across the four covenants and both doctrine paths, with live runtime strain, direct rite resolution, apex-covenant gating, Stonehelm covenant ascent, UI legibility, and restore continuity. The same session also made the first `Territorial Governance Recognition` layer live with world-pressure consequence, coalition backlash, dynasty-panel legibility, and restore continuity.
- Session 87: Territorial Governance now reaches the first honest sovereignty-resolution seam. Multi-seat dynastic authorities seat across governed keeps or cities or frontier marches, recognition now requires seat coverage plus court-loyalty and anti-revolt stability plus no-incoming-holy-war pressure, world pressure escalates from recognition into held governance and imminent sovereignty victory, Stonehelm answers with emergency anti-sovereignty tempo, and restore continuity now preserves completed territorial-governance victory state.
- Validation is green across data validation, runtime bridge, and all required `node --check` passes.
- Browser lane verification for this pass confirmed `play.html` serves cleanly, still transitions from launch into shell, preserves the resource bar and 11-pill realm dashboard, populates the dynasty and faith panels, preserves pause-toggle behavior, and emits zero console errors or failed requests. Screenshots were preserved under `reports/session-86-governance-shell-smoke.png` and `reports/session-86-governance-briefing-smoke.png`.
- Session 87 browser follow-up also confirmed the authoritative HUD counts directly on the auto-start route: 10 resource pills, 11 realm dashboard pills, populated dynasty and faith panels, and zero console or request failures. Screenshot preserved under `reports/session-87-governance-victory-shell-smoke.png`.

### Where To Start Now

1. Open `D:\ProjectsHome\Bloodlines`.
2. Read the session-start file order from `AGENTS.md`.
3. Read `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_46.md` through `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_86.md`.
4. Read `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_87.md` and the refreshed root continuity files before choosing the next execution lane.
5. Open `src/game/core/ai.js`, `src/game/core/simulation.js`, `src/game/main.js`, and `tests/runtime-bridge.mjs`.

### Exact Next Work

- The canon-ingestion task is complete. Do not reopen it as pending.
- Continue the browser-lane implementation chain from the live progression plus declaration plus political-event plus sovereignty-resolution spine, not from the completed convoy-reconsolidation lane.
- Implement the next Stage 5 sovereignty follow-up on top of the now-live Divine Right plus Covenant Test plus Territorial Governance victory spine.
- Priority order:
  - implement alliance-threshold pressure and population-acceptance buildup,
  - if that lane blocks, deepen multi-kingdom coalition or neutral-power counterpressure around late sovereignty attempts.
- If that work blocks, pivot to deeper multi-kingdom world pressure, neutral-power stage presence, or naval-world integration.
- Extend runtime coverage for any new work that touches the verification suite.

### New Analysis Surfaces

- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_48.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_49.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_50.md`
- `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_51.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_51.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_80.md`
- `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_81.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_81.md`
- `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_82.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_82.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_83.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_84.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_85.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_86.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_87.md`
- `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_52.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_52.md`
- `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_53.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_53.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_54.md`
- `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_54.md`
- `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_55.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_55.md`
- `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_56.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_56.md`
- `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_57.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_57.md`
- `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_58.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_58.md`
- `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_59.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_59.md`
- `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_60.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_60.md`
- `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_61.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_61.md`
- `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_62.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_62.md`
- `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_63.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_63.md`
- `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_64.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_64.md`
- `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_65.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_65.md`
- `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_66.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_66.md`
- `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_67.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_67.md`
- `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_68.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_68.md`
- `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_69.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_69.md`
- `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_70.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_70.md`
- `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_71.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_71.md`
- `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_72.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_72.md`
- `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_73.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_73.md`
- `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_74.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_74.md`
- `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_75.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_75.md`
- `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_76.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_76.md`
- `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_77.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_77.md`
- `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_78.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_78.md`
- `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_79.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_79.md`
- `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_80.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_81.md`
- `03_PROMPTS/CONTINUATION_PROMPT_2026-04-15_SESSION_82.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_46.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_47.md`
- `HANDOFF.md`

Older handoff content below is preserved historically but superseded by this 2026-04-15 Sessions 48-87 refresh.

## Session 14 Next Action Priority

1. Longer-siege AI next layer: post-repulse adjustment (retreat + recompose after cohesion strain threshold hit)
2. Faith Hall (L2) canonical covenant building â€” built on top of committed-covenant Wayshrine; unlocks L3 unit recruitment seat
3. Conviction milestone powers per band
4. Save-state serialization primer
5. Dual-clock declaration seam minimal

## Where To Start

Open `D:\ProjectsHome\Bloodlines` and read, in order:

1. `AGENTS.md`
2. `README.md`
3. `CLAUDE.md`
4. `CURRENT_PROJECT_STATE.md`
5. `ENVIRONMENT_REPORT_2026-04-14.md`
6. `01_CANON/BLOODLINES_MASTER_DESIGN_DOCTRINE_2026-04-14.md`
7. `18_EXPORTS/BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.4.md`
8. `docs/plans/2026-04-14-fortification-siege-population-legibility-wave.md`
9. `SOURCE_PROVENANCE_MAP.md`
10. `tasks/todo.md`

## Current Status (End of 2026-04-14 Session 8)

### Browser Reference Simulation

- Dynasty cascade, commander aura, territory capture/stabilization, faith exposure, doctrine effects, conviction ledger: live.
- Stone + iron economy with smelting chain: live.
- Fortification building class (`wall_segment`, `watch_tower`, `gatehouse`, `keep_tier_1`) with settlement-tier advancement: live.
- Dedicated siege production infrastructure: live. `siege_workshop` is buildable and now trains the active siege-engine roster.
- Three siege engines are live: `ram`, `siege_tower`, and `trebuchet`.
- Siege Tower support logic is live: nearby allies can press defended walls with better reach and structural pressure while a tower is in support distance.
- Formal siege sustainment is now live: `supply_camp` is buildable, and `siege_engineer` plus `supply_wagon` are now part of the live workshop roster.
- Engineer specialist behavior is live: engineers now contribute breach support, extend nearby structural assault leverage, and repair damaged siege engines in the field.
- Siege supply continuity is live: wagons linked to a live camp now keep nearby engines supplied, and unsupplied engines lose operational efficiency until the chain is restored.
- Assault cohesion strain (wave-spam denial): live.
- Canonical 90-second realm cycle with famine, water crisis, and cap pressure: live.
- `getRealmConditionSnapshot`: live.
- Browser renderer legibility wave: live for stone nodes, iron nodes, fortification buildings, Rams, and settlement-class plus fortification-tier labels on control points.
- Browser HUD legibility wave: live for 10-pill resource bar and 6-pill realm-condition bar.
- Browser command panel wave: live for quarry, iron mine, siege workshop, supply camp, wall segment, watch tower, gatehouse, and inner keep build options; active siege engines plus engineer and wagon specialists now train through the workshop.
- Enemy AI keep-assault refusal: live. Stonehelm will not attack a fortified primary keep without siege support.
- Enemy AI siege preparation: live. Stonehelm can add quarry, iron mine, and siege workshop, queue a first trebuchet, and stage siege lines before the assault.
- Enemy AI siege sustainment: live. Stonehelm can place a forward supply camp, queue engineers and wagons after the opening bombard engine, and refuse to launch the keep assault until the siege train is actually supplied.
- Fortified-settlement reserve cycling: live. Wounded defenders now fall back to the keep for triage while healthy reserves muster forward.
- Primary-keep reserve readout: live through `getRealmConditionSnapshot` and the fortification HUD pill.
- Reserve-duty renderer markers: live for fallback/recovering and muster states.
- Governor specialization: live. Governors now resolve as `border`, `city`, or `keep` based on their current governed anchor, and can rotate between held marches and dynastic settlements.
- Faith-integrated keep wards: live. Old Light, Blood Dominion, Order, and Wild now alter defensive sight, reserve tempo, local combat leverage, or hostile approach speed.
- Blood Dominion reserve rites: live. Under active defensive pressure a fortified seat can burn population into a temporary defensive surge.
- Captured-member operations are now live. The source dynasty can negotiate release or launch covert rescue; the captor can compel an immediate ransom exchange when the rival house can satisfy the terms.
- `dynasty.operations` is now live. Active operations carry escrowed cost, resolve over simulation time, record history, and surface progress in the dynasty panel.
- Tests green: `node tests/data-validation.mjs`, `node tests/runtime-bridge.mjs`, plus `node --check` on `main.js`, `renderer.js`, `ai.js`, `simulation.js`, `data-loader.js`, and `utils.js`.

### Unity Production Project

- `<repo>/unity/` remains the canonical Unity project.
- DOTS/ECS packages, `_Bloodlines/` folder structure, importer extensions, and documentation baseline remain in place.
- `<repo>/Bloodlines/` stub template remains preserved with `STUB_TEMPLATE_NOTICE.md`.

### Open Blocker Before ECS Code

Unity version alignment is still unresolved. Installed editors include `6000.3.13f1` (Unity 6.3 LTS, approved) and `6000.4.2f1` (Unity 6.4). The `unity/` project currently targets 6.4. Approved architecture says 6.3 LTS. User direction is still needed before writing ECS code.

## What Changed In Session 9

- Full 11-state realm-condition dashboard is now live in the browser HUD (Cycle, Pop, Food, Water, Loyalty, Fort, Army, Faith, Conviction, Logistics, World).
- `ballista` and `mantlet` siege-support classes are now live in data, simulation, renderer, AI, and tests.
- Stonehelm AI now queues mantlet and ballista after the trebuchet/engineer/wagon stack.
- Mantlet cover reduces inbound ranged damage to nearby friendly units.
- Produced five session 9 deliverables: state analysis, gap analysis, continuation plan, execution roadmap, master context addendum.

## What Changed In Session 10

- Unity version alignment decision RESOLVED. Option B locked: Unity 6.3 LTS (`6000.3.13f1`). `ProjectSettings/ProjectVersion.txt` and `Packages/manifest.json` aligned to 6.3 LTS-compatible package versions (Entities 1.4.0, Collections 2.5.7, Entities.Graphics 1.4.0, URP 17.3.0, Input System 1.11.2, Addressables 2.5.0).
- First ECS foundation authored: 15 canonical component files and 3 canonical systems under `unity/Assets/_Bloodlines/Code/`. Components cover position, faction, health, unit type (with role + siege class enums), building type (with fort role enum), resource node, control point (with control state enum), settlement (with primary keep tag), commander (with keep-presence tag), bloodline member (9-role enum + status enum + 7-path enum), faith (covenant id + doctrine path + exposure buffer + warded tag), conviction (4-bucket + 5-band enum), realm condition + cycle config, population + resource stockpile, siege engine state + mantlet cover. Systems cover Bootstrap (one-shot RealmCycleConfig seeding), RealmConditionCycle (90-second canonical cycle), PopulationGrowth (18-second gated growth).
- Sabotage operations are now live as the first covert operation type in `dynasty.operations`. Four canonical sub-types (gate_opening, fire_raising, supply_poisoning, well_poisoning). Spymaster-gated. Canonical cost, duration, success formula, counterplay. Burn DOT through a new `tickBuildingStatusEffects` loop. Poisoned supply camps break the supply chain for their window.
- Commander keep-presence sortie is now live. `issueKeepSortie(state, factionId, settlementId)` grants defenders Ã—1.22 attack and +22 sight for 12 seconds with a 60-second cooldown. Requires commander at keep AND active threat.
- `getRealmConditionSnapshot` fortification block now exposes `commanderAtKeep`, `sortieActive`, `sortieCooldownRemaining`, `sortieReady`.
- Tests extended: sabotage subtype validation, target validation, fire_raising escrow, sortie refusal path, snapshot exposure of new fortification fields.

## Next Recommended Action (Session 12)

### Step 0 (CLAUDE/CODEX ALTERNATION CONTEXT)

This project now runs under an overnight Claude/Codex alternation. If you are reading this on a scheduled fire:
- Check the previous Session N state-of-game report's `Author:` header to know which agent ran last.
- Write your session report with the correct author tag so the next fire can see the chain.
- Claude scheduled fires: 00:12, 05:12, 10:12, 15:12, 20:12 local via `bloodlines-claude-alternation`.
- Codex fires: staggered between Claude fires (manual or via user's own Task Scheduler).

### Step 1 (Verify house-select visually)

A Session 11 browser ES module cache issue prevented live visual confirmation of the URL-driven house-select swap (`play.html?house=stonehelm`). The disk fix is correct (confirmed by served-file inspection). Action: hard-reload the preview with Ctrl+Shift+R or use a fresh browser profile. Confirm `House: Stonehelm` in debug overlay when `?house=stonehelm` is in the URL.

### Step 2 (Commander sortie UI surface)

Session 10 made sortie live in simulation but exposed it only through `getRealmConditionSnapshot`. The player has no UI button to invoke it. Add an action button in the fort pill tooltip or dynasty panel that calls `issueKeepSortie(state, "player", primaryKeep.id)` when sortieReady is true. Satisfies legibility-follows-depth.

### Step 3 (Longer-siege AI adaptation)

Stonehelm AI currently prepares and commits to formal siege but does not adapt to mid-siege pressure. Add: relief-window awareness (delay assault if player army approaching), repeated-assault window logic (after repulse, retreat to resupply before next attempt), supply-protection patrols, post-repulse tactical adjustment.

### Step 4 (Faith prototype enablement)

All four covenants are still `prototypeEnabled: false` in `data/faiths.json`. Flip the flags, add per-covenant building progression (Wayshrine â†’ Hall â†’ Grand Sanctuary equivalents), add L3 faith unit roster (8 units: 2 per covenant per doctrine path). Extend `updateFaithExposure` and ward profiles to consume the new structures.

### Step 5 (Lance first-open Unity menu run)

Lance-gated. Run `Bloodlines â†’ Import â†’ Sync JSON Content` in Unity 6.3 LTS (6000.3.13f1) to generate ScriptableObject `.asset` files. Commit generated assets.

### Step 6 (Session-by-session roadmap)

See `docs/plans/2026-04-14-session-9-next-phase-execution-roadmap.md` for the full ordered sequence.

## Old Next Recommended Action (Session 11, now complete)

### Step 1 (Lance opens Unity for first time under 6.3 LTS)

Open `D:\ProjectsHome\Bloodlines\unity\` in Unity 6.3 LTS (`6000.3.13f1`). Accept any LTS-compatible package version adjustments the Package Manager offers. Let Unity regenerate `Library/`. Verify the authored session 10 ECS layer compiles cleanly.

Then run `Bloodlines â†’ Import â†’ Sync JSON Content`. Commit the generated ScriptableObject assets under `Assets/_Bloodlines/Data/*/`.

### Step 2 (Second playable house â€” Stonehelm)

Enable Stonehelm as `prototypePlayable: true` in `data/houses.json`. Add minimal URL-driven house-select to `play.html` so the skirmish can launch as Stonehelm via `play.html?house=stonehelm`. Give Stonehelm at least one distinguishing mechanical hook (fortification cost discount per `04_SYSTEMS/TERRITORY_SYSTEM.md`).

### Step 3 (Hartvale Verdant Warden entry)

Add the canonical Hartvale unique unit to `data/units.json`. Mark `house: "hartvale"`, `prototypeEnabled: false` until Hartvale becomes playable. Update `tests/data-validation.mjs` to assert the unit exists with canonical Off 4 / Def 5 profile.

### Step 4 (AI sabotage reciprocity)

Give Stonehelm AI the ability to run sabotage against player-owned targets. Spymaster-gated the same way, same success formula. Natural counter-pressure emerges when the player has exposed gates, supply camps, or wells.

### Step 5 (Longer-siege AI adaptation â€” continuing session 12 scope if time allows)

Relief-window awareness, repeated-assault window logic, supply-protection patrols, post-repulse tactical adjustment.

### Step 6 (Continued ECS foundation â€” session 14 scope if time allows)

Once session 11 verifies Unity open, continue authoring: UnitMovementSystem, UnitGatherSystem, SmeltingFuelSystem, BuildingProductionSystem, ConstructionSystem, CombatTargetingSystem, CombatDamageSystem, AssaultCohesionSystem, TerritoryCaptureSystem, FaithExposureSystem, DynastyCascadeSystem. Plus authoring + baking for scene-to-entity conversion.

### Full session-by-session roadmap

See `docs/plans/2026-04-14-session-9-next-phase-execution-roadmap.md` for ordered sequencing through session 30+. Session 10 completed roadmap items from session 10 (sabotage + commander sortie) and session 13 (Unity version decision + first ECS components), accelerating the timeline by two sessions of roadmap work.

## New Analysis Surfaces

- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_4.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_5.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_6.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_7.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_8.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_9.md` (full-realization state analysis)
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_10.md` (Unity 6.3 LTS lock + ECS foundation + sabotage + sortie)
- `docs/BLOODLINES_SYSTEM_GAP_ANALYSIS_2026-04-14_SESSION_9.md`
- `docs/plans/2026-04-14-session-9-full-realization-continuation-plan.md`
- `docs/plans/2026-04-14-session-9-next-phase-execution-roadmap.md`

Session 4 is the broad diagnosis. Session 5 is the governor/ward addendum. Session 6 is the siege-preparation addendum. Session 7 is the captivity-operations addendum. Session 8 is the siege-sustainment addendum. Session 9 is the full-realization state analysis and introduces the six-vector growth model that governs all subsequent sessions. Read all six plus the gap analysis, continuation plan, and execution roadmap before choosing the next browser or Unity execution wave.

## Do Not Regress

- Do not reintroduce `deploy/bloodlines` as an active root.
- Do not create a new external Bloodlines folder.
- Do not delete preserved source bundles, including the `Bloodlines/` stub.
- Do not replace large design files with short summaries.
- Do not ignore the continuity layer added at the root.
- Do not drift away from the 2026-04-14 master doctrine when making design calls.
- Do not reduce the canonical scope.
- Do not abandon the browser reference simulation. It remains the frozen working spec and feel reference, but no new canonical systems belong there.
- Do not open `<repo>/Bloodlines/` as the primary Unity project. Open `<repo>/unity/`.

## Verification Commands

### Browser reference simulation

```powershell
python -m http.server 8057 --directory D:/ProjectsHome/Bloodlines
```

Open:

- `http://localhost:8057/`
- `http://localhost:8057/play.html`

Validation:

```powershell
Set-Location D:/ProjectsHome/Bloodlines
node tests/data-validation.mjs
node tests/runtime-bridge.mjs
node --check src/game/main.js
node --check src/game/core/simulation.js
node --check src/game/core/renderer.js
node --check src/game/core/ai.js
node --check src/game/core/data-loader.js
node --check src/game/core/utils.js
```

### Unity production project

```powershell
# Open canonical Unity project in approved version once the version decision is locked:
& "C:\Program Files\Unity\Hub\Editor\6000.3.13f1\Editor\Unity.exe" -projectPath "D:/ProjectsHome/Bloodlines/unity"

# After first open, run menu: Bloodlines -> Import -> Sync JSON Content
```

## Important Files For The Next Session

- `CURRENT_PROJECT_STATE.md`
- `ENVIRONMENT_REPORT_2026-04-14.md`
- `docs/plans/2026-04-14-fortification-siege-population-legibility-wave.md`
- `unity/README.md`
- `unity/Assets/_Bloodlines/README.md`
- `unity/Assets/_Bloodlines/Code/README.md`
- `unity/Assets/_Bloodlines/Data/README.md`
- `unity/Assets/_Bloodlines/Code/Definitions/BloodlinesDefinitions.cs`
- `unity/Assets/_Bloodlines/Code/Editor/JsonContentImporter.cs`
- `Bloodlines/STUB_TEMPLATE_NOTICE.md`
- `tasks/todo.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_4.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_5.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_6.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_7.md`
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_8.md`
- `01_CANON/DEFENSIVE_FORTIFICATION_DOCTRINE.md`
- `04_SYSTEMS/FORTIFICATION_SYSTEM.md`
- `04_SYSTEMS/SIEGE_SYSTEM.md`
- `03_PROMPTS/CONTINUATION_PROMPT_2026-04-14_SESSION_9.md`

## Session 126 Unity Attack Orders And Attack-Move

- Status: green on `codex/unity-attack-move` at `7759f84e1c00eeb8a1baf329ac33b38d0e074cbc`, rebased onto `master` head `548d7804ce55766420d75184385b3bedb739a3ee`.
- Delivered:
  - contract-shaped `AttackOrderComponent`
  - new `AttackOrderResolutionSystem` before `AutoAcquireTargetSystem`
  - minimal explicit-target preservation inside `AutoAcquireTargetSystem`
  - explicit-order cleanup inside `DeathResolutionSystem`
  - new governed debug API partial `BloodlinesDebugCommandSurface.Combat.cs`
  - four-phase combat smoke proof including explicit attack and attack-move
- Important integration note: current `master` already contains the older `AttackOrderSystem` and `BloodlinesDebugCommandSurface.AttackOrders.cs` from the previous merged attack-order lane. This slice composes with that state additively by:
  - keeping `AttackOrders.cs` as the input owner
  - adding only the governed API surface in `Combat.cs`
  - marking `AttackOrderSystem` as `[DisableAutoCreation]` so it does not compete with `AttackOrderResolutionSystem`
- Validation status:
  - `dotnet build unity/Assembly-CSharp.csproj -nologo` green
  - `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` green
  - wrapper-locked `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1` green with:
    - `explicitAttackPhase=True`
    - `attackMovePhase=True`
  - wrapper-locked `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` green with:
    - `aiActivityObserved=True`
    - `aiConstructionObserved=True`
    - `stabilitySurplusObserved=True`
    - `capPressureObserved=True`
    - `loyaltyDeclineObserved=True`
    - `starvationObserved=True`
    - `trickleGainObserved=True`
    - `gatherDepositObserved=True`
  - wrapper-locked `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` green
  - `node tests/data-validation.mjs` green
  - `node tests/runtime-bridge.mjs` green
- Handoff doc: `docs/unity/session-handoffs/2026-04-17-unity-attack-orders-and-attack-move.md`

## Next Session Instruction

- Stop at merge coordination for `codex/unity-attack-move`.
- Do not extend this branch into stance systems, patrol, guard, follow, AoE combat, HUD cursor polish, or AI consumption of attack orders.
- If the branch merges cleanly, the next combat continuation should start from updated `master`, not from this branch tip.

## Session 127 Unity Target Acquisition Throttling And Sight Loss

- Status: green on stacked branch `codex/unity-target-acquisition-los`, built above `codex/unity-attack-move`.
- Contract lane: `combat-acquisition-and-sight` in contract Revision 2.
- Delivered:
  - additive passive-target cadence and sight-retention state inside `CombatStatsComponent`
  - throttled passive scans in `AutoAcquireTargetSystem`
  - stale passive-target cleanup, chase-stop behavior, and reacquire cooldown arming in `AttackResolutionSystem`
  - fifth governed combat smoke proof for sight-loss cleanup plus delayed replacement acquire
- Validation status:
  - `dotnet build unity/Assembly-CSharp.csproj -nologo` green
  - `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` green
  - wrapper-locked `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1` green with:
    - `explicitAttackPhase=True`
    - `attackMovePhase=True`
    - `targetVisibilityPhase=True`
  - wrapper-locked `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` green with:
    - `aiActivityObserved=True`
    - `aiConstructionObserved=True`
    - `stabilitySurplusObserved=True`
    - `capPressureObserved=True`
    - `loyaltyDeclineObserved=True`
    - `starvationObserved=True`
    - `trickleGainObserved=True`
    - `gatherDepositObserved=True`
  - wrapper-locked `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` green
  - `node tests/data-validation.mjs` green
  - `node tests/runtime-bridge.mjs` green
- Handoff doc: `docs/unity/session-handoffs/2026-04-17-unity-target-acquisition-throttling-and-sight-loss.md`

## Next Session Instruction (Session 127)

- Claude or Lance should merge `codex/unity-attack-move` first.
- Then rebase or merge `codex/unity-target-acquisition-los` onto the refreshed `master` tip.
- Do not widen the stacked target-acquisition branch into death presentation, renown or conviction kill hooks, or shared presentation work without a fresh contract lane assignment.

## Dual-Clock + Match-Progression Lane: Sub-Slice 1 Complete (2026-04-17)

### Slice
Lane: `dual-clock-match-progression`, Branch: `claude/unity-match-progression`

### Work Completed
- `unity/Assets/_Bloodlines/Code/Time/DualClockComponent.cs` -- singleton IComponentData: InWorldDays, DaysPerRealSecond (default 2), DeclarationCount
- `unity/Assets/_Bloodlines/Code/Time/MatchProgressionComponent.cs` -- singleton IComponentData: five-stage state, phase, readiness, Great Reckoning, dominant faction
- `unity/Assets/_Bloodlines/Code/Time/DualClockTickSystem.cs` -- ISystem [BurstCompile]; creates singleton on OnCreate; ticks InWorldDays each frame
- `unity/Assets/_Bloodlines/Code/Time/MatchProgressionEvaluationSystem.cs` -- ISystem; evaluates all five stages from live ECS state; fully implements stages 1-3; stages 4-5 war signals are placeholder false pending declaration-seam port
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.MatchProgression.cs` -- TryDebugGetDualClock, TryDebugGetMatchProgression
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesMatchProgressionSmokeValidation.cs` -- 4-phase validator (DualClock defaults, tick arithmetic, MatchProgression defaults, stage advance)
- `scripts/Invoke-BloodlinesUnityMatchProgressionSmokeValidation.ps1` -- PS1 wrapper
- `docs/unity/CONCURRENT_SESSION_CONTRACT.md` -- revision 5 â†’ 6, dual-clock-match-progression lane claimed as active

### Namespace Note
`Bloodlines.GameTime` (not `Bloodlines.Time`) to avoid ambiguity with `UnityEngine.Time`.

### Validation Results
- dotnet build Assembly-CSharp.csproj: 0 errors 0 warnings
- dotnet build Assembly-CSharp-Editor.csproj: 0 errors (pre-existing CS0649 warnings only)
- node tests/data-validation.mjs: passed
- node tests/runtime-bridge.mjs: passed
- Invoke-BloodlinesUnityContractStalenessCheck.ps1: PASSED revision=6
- Bootstrap + combat smokes: not re-run in batch (Unity not available); builds passing is the gate for sub-slice 1

### Next Action (Sub-Slice 2)
Wire the declaration seam: implement `DualClockDeclarationSystem` that lets in-world events declare time jumps via a `DeclareInWorldTimeRequest` buffer element (browser: `declareInWorldTime` at 13800). Port `rivalContactActive`, `sustainedWarActive`, `contestedBorder` signals by querying ControlPoint proximity between player and enemy factions. Update stage 4 requirements from placeholder false to live signals.

## Dual-Clock + Match-Progression Lane: Sub-Slice 3 Complete (2026-04-17)

### Slice
Lane: `dual-clock-match-progression`, Branch: `claude/unity-match-progression`

### Work Completed
- `unity/Assets/_Bloodlines/Code/Time/DualClockDeclarationBridgeSystem.cs` -- observes CP ownership changes, fires 14-day declaration on each capture (browser: declareInWorldTime:8155)
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs` -- narrow additions: `matchProgressionChecked` field, `ProbeMatchProgressionIntegrity` method (asserts DualClock and MatchProgression singletons present + sane), `matchProgressionChecked=True` in pass-line diagnostics

### Gate Results
- dotnet build Assembly-CSharp.csproj: 0 errors, 0 warnings
- dotnet build Assembly-CSharp-Editor.csproj: 0 errors
- data-validation.mjs: passed
- runtime-bridge.mjs: passed
- contract staleness check: PASSED revision=6

### Lane Status After Sub-Slice 3
- DualClockComponent singleton: ticks each frame, holds DeclarationCount
- DeclareInWorldTimeRequest buffer: any system can push time jumps
- DualClockDeclarationSystem: processes buffer each frame
- DualClockDeclarationBridgeSystem: fires capture events -> 14 in-world days per capture
- MatchProgressionEvaluationSystem: stages 1-3 fully live; stage 4 live via contestedBorder + unit proximity; stage 5 pending world-pressure port
- BootstrapRuntimeSmoke: matchProgressionChecked=True probe confirms singletons always present in live bootstrap world

### Next Lane Action
Retire dual-clock-match-progression lane to master once sub-slice 3 is merged. Future extension points: wiring `declareInWorldTime` at dynasty operations, marriage events, and holy war sites; replacing `sustainedWarActive` proxy with real siege engine count once Codex fortification-siege lane is merged.

---

## 2026-04-18 World Pressure Escalation Lane (Session 128)

### Status: COMPLETE and MERGED to master

### What Was Done
- Ported `updateWorldPressureEscalation` / `applyWorldPressureConsequences` (simulation.js:13709, 13695) into Unity ECS.
- `WorldPressureEscalationSystem` in `unity/Assets/_Bloodlines/Code/WorldPressure/` runs in SimulationSystemGroup before MatchProgressionEvaluationSystem.
- Score sources active: territoryExpansion (max(0, territories-2)), greatReckoning (4 when GR target).
- Stage 5 convergence now live: MatchProgressionEvaluationSystem includes `playerWorldPressureConvergence` (Targeted && Level >= 3).
- Bootstrap smoke now probes WorldPressure integrity (`worldPressureChecked=True`).
- 4-phase smoke validator all green; contract revision 8 -> 9, lane retired.

### Gate Results
- dotnet build Assembly-CSharp.csproj: 0 errors
- dotnet build Assembly-CSharp-Editor.csproj: 0 errors
- Bootstrap runtime smoke: PASS (worldPressureChecked=True)
- Combat smoke: exit 0
- Scene shells: Bootstrap + Gameplay green
- data-validation.mjs: passed
- runtime-bridge.mjs: passed
- Contract staleness check: PASSED revision=9
- World pressure smoke: Phase 1-4 PASS

### Next Unclaimed Tier 1 Lanes
See `docs/unity/CONCURRENT_SESSION_CONTRACT.md` "Next Unblocked Tier 1 Lanes" section. Top candidates:
1. fortification-siege-sub-slice-3-imminent-engagement-warnings (continuing `codex/unity-fortification-siege`)
2. ai-strategic-layer-sub-slice-2-supply-chain

---

## Session 128 Unity Fortification Tier And Reserves (Codex)

- Status: green on `codex/unity-fortification-siege`; merged to master 2026-04-18.
- Delivered:
  - `FortificationCanon.cs` direct port of the browser fortification / siege constants block
  - `FortificationComponent.cs` fortification tier / radius state plus neutral fortification faith-ward seam
  - `FortificationReserveComponent.cs` reserve threshold / heal / count state
  - `AdvanceFortificationTierSystem.cs` deterministic fortification-tier advancement from completed linked buildings
  - `FortificationReserveSystem.cs` retreat, triage heal, recovery, and muster-forward reserve cycling
  - `BloodlinesDebugCommandSurface.Fortification.cs` with tier, reserve-count, and force-muster APIs
  - `BloodlinesFortificationSmokeValidation.cs` and `scripts/Invoke-BloodlinesUnityFortificationSmokeValidation.ps1`
- Handoff doc: `docs/unity/session-handoffs/2026-04-17-unity-fortification-siege-fortification-tier-and-reserves.md`

## Session 129 Unity Siege Support And Field Water (Codex)

- Status: green on `codex/unity-fortification-siege`; merged to master 2026-04-18.
- Delivered:
  - `SiegeSupportCanon.cs` and `FieldWaterCanon.cs` for the canonical siege-support and field-water constants
  - `SiegeSupportComponent.cs`, `FieldWaterComponent.cs`, and `SiegeSupplyTrainComponent.cs`
  - `SiegeComponentInitializationSystem.cs`, `SiegeSupportRefreshSystem.cs`, `FieldWaterSupportScanSystem.cs`, and `FieldWaterStrainSystem.cs`
  - `BloodlinesDebugCommandSurface.Siege.cs`
  - `BloodlinesSiegeSmokeValidation.cs` and `scripts/Invoke-BloodlinesUnitySiegeSmokeValidation.ps1`
- Handoff doc: `docs/unity/session-handoffs/2026-04-18-unity-fortification-siege-siege-support-and-field-water.md`

## Next Sub-Slice Instruction (Session 130 merge complete)

- Sub-slice 3 (imminent-engagement warnings) is the remaining work on `codex/unity-fortification-siege`.
- Target browser reference: `src/game/core/simulation.js` `tickImminentEngagementWarnings`.
- Keep scope discipline: no bootstrap runtime smoke integration, no siege-operation lifecycle, no combat-file edits.
- Re-check `git branch --show-current` before any continuity update or commit.

## AI Strategic Layer Sub-Slice 2: Strategic Pressure System (Claude, 2026-04-18)

### Status: COMPLETE on branch claude/unity-ai-strategic-pressure

### What Was Done
- Ported ai.js `updateEnemyAi` timer-clamp and stage-gate block (lines 1127-1241) into `AIStrategicPressureSystem`.
- Extended `AIStrategyComponent` with `RivalryUnlocked` and `RaidPressureUnlocked` fields.
- 4-phase smoke validator all green (phase 1: stage lock / territory floor; phase 2: rivalry+raid unlock; phase 3: WP Level 2 clamps; phase 4: Great Reckoning clamps).
- Contract bumped to revision 14. Retired victory-conditions and tier2-batch-dynasty-systems lanes.

### Gate Results
- dotnet build Assembly-CSharp.csproj: 0 errors
- dotnet build Assembly-CSharp-Editor.csproj: 0 errors
- Bootstrap runtime smoke: PASS
- Combat smoke: exit 0
- Scene shells: Bootstrap + Gameplay green
- Fortification smoke: PASS
- Siege smoke: exit 0
- data-validation.mjs: PASS
- runtime-bridge.mjs: PASS
- Contract staleness check: PASSED revision=14
- AI strategic pressure smoke: Phase 1-4 PASS

### Next Unclaimed Lanes
1. fortification-siege-sub-slice-3-imminent-engagement-warnings (branch codex/unity-fortification-siege, Codex prompt ready)
2. ai-strategic-layer-sub-slice-3-supply-chain

## AI Strategic Layer Sub-Slice 3: Governance and Event-Context Pressure (Claude, 2026-04-18)

### Status: COMPLETE on branch claude/unity-ai-governance-pressure

### What Was Done
- Ported ai.js governance/event-context timer clamp block (lines 1129-1215) into `AIStrategicPressureSystem.ApplyGovernancePressure`.
- Extended `AIStrategyComponent` with 20 flag fields and `BuildTimer`.
- 4-phase smoke validator all green. Contract revision 14 -> 15.

### Gate Results
- dotnet build Assembly-CSharp.csproj: 0 errors
- dotnet build Assembly-CSharp-Editor.csproj: 0 errors
- Bootstrap runtime smoke: PASS
- Combat smoke: exit 0
- Scene shells: Bootstrap + Gameplay green
- AI strategic pressure smoke (prior sub-slice): PASS
- AI governance pressure smoke: Phase 1-4 PASS
- data-validation.mjs: PASS
- runtime-bridge.mjs: PASS
- Contract staleness check: PASSED revision=15

### Next Unclaimed Lanes
1. ai-strategic-layer-sub-slice-4-siege-staging
2. fortification-siege-sub-slice-3-imminent-engagement-warnings (Codex in progress)

## AI Strategic Layer Sub-Slice 4: Worker Gather Dispatch (Claude, 2026-04-18)

### Status: COMPLETE on branch claude/unity-ai-worker-gather

### What Was Done
- Ported ai.js idle-worker dispatch loop (1243-1251), getEnemyGatherPriorities (885-922), chooseGatherNode (924-933) into `AIWorkerGatherSystem`.
- Throttled by WorkerGatherIntervalSeconds accumulator (default 5s). Index-modulo resource priority rotation. PlayerKeepFortified skips stone.
- Nearest-node selection by Euclidean distance. Sets Phase=Seeking, AssignedNode, AssignedResourceId, CarryResourceId on dispatch.
- 4-phase smoke validator all green. Contract revision 15 -> 16.

### Gate Results
- dotnet build Assembly-CSharp.csproj: 0 errors
- dotnet build Assembly-CSharp-Editor.csproj: 0 errors
- Bootstrap runtime smoke: PASS
- Combat smoke: exit 0
- Scene shells: Bootstrap + Gameplay green
- AI governance pressure smoke (prior sub-slice): PASS
- AI worker gather smoke: Phase 1-4 PASS
- data-validation.mjs: PASS
- runtime-bridge.mjs: PASS
- Contract staleness check: PASSED revision=16

### Next Unclaimed Lanes
1. ai-strategic-layer-sub-slice-5-siege-staging (new branch claude/unity-ai-siege-staging)
2. ai-strategic-layer-sub-slice-6-dynasty-covert-ops (new branch, after sub-slice 5)
3. fortification-siege-sub-slice-3-imminent-engagement-warnings (Codex in progress, do not claim)

## AI Strategic Layer Sub-Slice 5: Siege Staging Orchestration (Claude, 2026-04-19)

### Status: COMPLETE on branch claude/unity-ai-siege-staging

### What Was Done
- Ported ai.js attackTimer<=0 siege staging decision tree (~1825-2090) into `AISiegeOrchestrationSystem` + `AISiegeOrchestrationComponent`.
- `SiegeOrchestrationPhase` 17-value enum matches exact branch priority order.
- `AISiegeOrchestrationComponent` holds all readiness/context flags (field water, siege engine presence, supply chain, escort, post-repulse, etc.).
- Pure state machine: reads PlayerKeepFortified from AIStrategyComponent, evaluates priority tree, writes Phase.
- 6-phase smoke validator all green. Contract revision 16 -> 17.

### Gate Results
- dotnet build Assembly-CSharp.csproj: 0 errors
- dotnet build Assembly-CSharp-Editor.csproj: 0 errors
- Bootstrap runtime smoke: PASS (worldPressureChecked=True and all prior fields)
- Combat smoke: exit 0
- Scene shells: Bootstrap + Gameplay green
- AI governance pressure smoke: PASS
- AI worker gather smoke: PASS
- AI siege orchestration smoke: Phase 1-6 PASS
- data-validation.mjs: PASS
- runtime-bridge.mjs: PASS
- Contract staleness check: PASSED revision=17

### Next Unclaimed Lanes
1. ai-strategic-layer-sub-slice-6-dynasty-covert-ops (new branch claude/unity-ai-dynasty-covert-ops; ai.js ~2681)
2. fortification-siege-sub-slice-3-imminent-engagement-warnings (Codex in progress, do not claim)

## AI Strategic Layer Sub-Slice 6: Dynasty Covert Ops Dispatch (Claude, 2026-04-19)

### Status: COMPLETE on branch claude/unity-ai-dynasty-covert-ops

### What Was Done
- Ported ai.js updateEnemyAi dynasty covert ops dispatch block (~2419-2678) into `AICovertOpsSystem` + `AICovertOpsComponent`.
- Nine countdown timers with canonical ai.js defaults. `CovertOpKind` 10-value enum.
- Three phases per entity: TickTimers, ApplyPressureCaps (DarkExtremes/WorldPressureLevel/convergence/HolyWarFocused/DivineRight/CaptivesFocused caps via math.min), TryFireOps (fires first expired timer with gate met, writes LastFiredOp, returns).
- Captive branch: rescue-first for HighPriorityCaptive or EnemyIsHostileToPlayer; ransom-first otherwise.
- Pact branch: shouldPropose when succession pressure or army <= 3 or governance near victory.
- 5-phase smoke validator all green. Contract revision 17 -> 18.
- Key lesson: firing timers in smoke tests must be -1f (already expired); 0.001f never fires with deltaTime=0.

### Gate Results
- dotnet build Assembly-CSharp.csproj: 0 errors
- dotnet build Assembly-CSharp-Editor.csproj: 0 errors
- Bootstrap runtime smoke: PASS
- Combat smoke: exit 0
- Scene shells: Bootstrap + Gameplay green
- Fortification smoke: PASS
- Siege smoke: exit 0
- AI covert ops smoke: Phase 1-5 PASS
- data-validation.mjs: PASS
- runtime-bridge.mjs: PASS
- Contract staleness check: PASSED revision=18

### Next Unclaimed Lanes
1. ai-strategic-layer-sub-slice-7-build-timer-chain (new branch claude/unity-ai-build-timer-chain; ai.js building construction/upgrade timer block ~lines 1060-1100)
2. fortification-siege-sub-slice-3-imminent-engagement-warnings (Codex in progress, do not claim)

## AI Strategic Layer Sub-Slice 7: Build Order Priority Chain (Claude, 2026-04-19)

### Status: COMPLETE on branch claude/unity-ai-build-timer-chain

### What Was Done
- Ported ai.js updateEnemyAi buildTimer<=0 block (~lines 1377-1573) into `AIBuildOrderSystem` + `AIBuildOrderComponent`.
- 13-step build priority chain preserves ai.js if-else order exactly: Barracks, Wayshrine, Quarry, IronMine, SiegeWorkshop, CovenantHall, GrandSanctuary, ApexCovenant, SupplyCamp, Stable, Dwelling, Farm, Well.
- `BuildOrderKind` 14-value enum (None plus 13 branches).
- Timer source: reads `AIStrategyComponent.BuildTimer` (decremented by `AIStrategicPressureSystem` with 0.016f floor). On fire, writes first matching branch to `NextBuildOp` and resets `BuildTimer = PlayerKeepFortified ? 4f : 6f` per ai.js line 1573.
- Faith intensity thresholds canonical: 26 (covenant hall), 48 (grand sanctuary), 80 (apex covenant). Grand sanctuary urgency also fires on active covenant test, player covenant active, or player divine right active.
- Scheduling and decision only; actual `attemptPlaceBuilding` deferred to later economy/placement integration pass.
- 5-phase smoke validator all green (Barracks, Wayshrine, SiegeWorkshop, Dwelling, Farm). Contract revision 18 -> 19.
- Firing timers seeded at -1f in smoke tests (same pattern as sub-slice 6).

### Gate Results
- dotnet build Assembly-CSharp.csproj: 0 errors
- dotnet build Assembly-CSharp-Editor.csproj: 0 errors
- Bootstrap runtime smoke: PASS
- Combat smoke: exit 0
- Scene shells: Bootstrap + Gameplay green
- Fortification smoke: PASS
- Siege smoke: exit 0
- AI build order smoke: Phase 1-5 PASS
- data-validation.mjs: PASS
- runtime-bridge.mjs: PASS
- Contract staleness check: PASSED revision=19

### Next Unclaimed Lanes
1. ai-strategic-layer-sub-slice-8-marriage-proposal-execution (new branch claude/unity-ai-marriage-proposal-execution; ai.js ~2580-2620 plus simulation proposeMarriage ~7340 and acceptMarriage ~7388)
2. fortification-siege-sub-slice-3-imminent-engagement-warnings (Codex in progress on codex/unity-fortification-siege, do not claim)
3. codex/unity-ai-command-dispatch rebase pending on Codex side once master advances to revision 19.

## AI Strategic Layer Sub-Slice 8: Marriage Proposal Execution (Claude, 2026-04-19)

### Status: COMPLETE on branch claude/unity-ai-marriage-proposal-execution

### What Was Done
- Ported ai.js `tryAiMarriageProposal` (~2897-2944) into `AIMarriageProposalExecutionSystem` as the execution half of the covert-ops marriage path.
- Consumes `AICovertOpsComponent.LastFiredOp == CovertOpKind.MarriageProposal` written by sub-slice 6; always clears back to `None` after processing (pass or fail) to match browser single-fire-per-timer semantic.
- Four abort gates in browser order: already-married, already-pending, no source candidate, no target candidate. Candidate selection walks `DynastyMemberRef` buffer, filters to non-head active/ruling members, picks first match.
- Creates `MarriageProposalComponent` entity with `ProposedAtInWorldDays = DualClockComponent.InWorldDays` and `ExpiresAtInWorldDays = now + 30`. Expiration window pulls from `MarriageProposalExpirationSystem.ExpirationInWorldDays` so it stays in sync.
- Target faction hardcoded to `"player"` matching browser. Multi-faction extension reserved for later slice.
- Strategic profile gating (faith hostility, population deficit, legitimacy distress, succession crises) is responsibility-split: sub-slice 6 gates dispatch via `MarriageStrategicProfileWilling`, this slice gates execution structurally.
- Cross-lane reads only (MarriageComponent, MarriageProposalComponent, DynastyMemberComponent, DualClockComponent); no structural edits to retired lanes.
- 5-phase smoke validator all green (clean creation, already-married block, already-pending block, missing target dynasty, no-dispatch no-op). Contract revision 19 -> 20.

### Gate Results
- dotnet build Assembly-CSharp.csproj: 0 errors
- dotnet build Assembly-CSharp-Editor.csproj: 0 errors
- Bootstrap runtime smoke: PASS
- Combat smoke: exit 0
- Scene shells: Bootstrap + Gameplay green
- Fortification smoke: PASS
- Siege smoke: exit 0
- AI marriage proposal execution smoke: Phase 1-5 PASS
- data-validation.mjs: PASS
- runtime-bridge.mjs: PASS
- Contract staleness check: PASSED revision=20 (bumped post-gate)

### Next Unclaimed Lanes
1. ai-strategic-layer-sub-slice-9-marriage-inbox-accept (new branch claude/unity-ai-marriage-inbox-accept; ai.js tryAiAcceptIncomingMarriage ~2880-2895; simulation acceptMarriage ~7388)
2. fortification-siege-sub-slice-3-imminent-engagement-warnings (Codex in progress on codex/unity-fortification-siege; do not claim)
3. codex/unity-ai-command-dispatch rebase pending on Codex side; master now at revision 20 so Codex should bump to 21 on rebase.

## AI Strategic Layer Sub-Slice 9: Marriage Inbox Accept (Claude, 2026-04-19)

### Status: COMPLETE on branch claude/unity-ai-marriage-inbox-accept

### What Was Done
- Ported ai.js `tryAiAcceptIncomingMarriage` (~2880-2895) into `AIMarriageInboxAcceptSystem` as the execution half of the covert-ops marriage inbox path. With sub-slice 8 already landed, the marriage loop is now mechanically end-to-end in Unity (dispatch, proposal, accept, expiration, gestation).
- Consumes `AICovertOpsComponent.LastFiredOp == CovertOpKind.MarriageInboxAccept` written by sub-slice 6. Scans `MarriageProposalComponent` entities for the first `Status == Pending` match where source is `"player"` and target is this AI faction.
- On match: flips proposal status to `Accepted`, creates primary + mirror `MarriageComponent` entities sharing a `MarriageId` derived from the proposal id. Primary (source-headed) drives child generation via existing `MarriageGestationSystem`; mirror (target-headed) exists for symmetric enumeration only.
- Gestation window pulled from `MarriageGestationSystem.GestationInWorldDays` so the two systems stay in sync on the 60-day canon.
- Always clears `LastFiredOp` back to `None` after processing (match or no match). Matches browser single-fire-per-timer semantic.
- Source hardcoded to `"player"` matching browser inbox filter; multi-faction extension reserved for future slice.
- `[UpdateAfter(AIMarriageProposalExecutionSystem)]` so propose + accept cannot race on the same frame.
- Cross-lane reads only (MarriageProposalComponent, MarriageComponent, MarriageGestationSystem constant, DualClockComponent).
- Deferred: browser-side effects on accept (governance authority cost, hostility drop, conviction event recording, legitimacy +2, declareInWorldTime 30-day jump, narrative message push). These remain for a dedicated effects slice.
- 4-phase smoke validator all green (dispatch+pending creates primary+mirror, dispatch+no-proposal no-op, dispatch+only-expired no-op, no-dispatch+pending no-op). Contract revision 20 -> 21.

### Gate Results
- dotnet build Assembly-CSharp.csproj: 0 errors
- dotnet build Assembly-CSharp-Editor.csproj: 0 errors
- Bootstrap runtime smoke: PASS (required one retry after transient bee_backend state-file contention)
- Combat smoke: exit 0
- Scene shells: Bootstrap + Gameplay green
- Fortification smoke: PASS
- Siege smoke: exit 0 PASS (required one retry after transient Library write-contention from back-to-back Unity launches)
- AI marriage inbox accept smoke: Phase 1-4 PASS
- data-validation.mjs: PASS
- runtime-bridge.mjs: PASS
- Contract staleness check: PASSED (bumped to revision 21 post-gate)

### Next Unclaimed Lanes
1. ai-strategic-layer-sub-slice-10-marriage-strategic-profile (new branch claude/unity-ai-marriage-strategic-profile; ai.js getAiMarriageStrategicProfile ~2730-2857; gates both proposal and accept paths on faith-aware legitimacy repair vs. hardened refusal, population deficit, hostility, succession signals).
2. fortification-siege-sub-slice-3-imminent-engagement-warnings (Codex in progress on codex/unity-fortification-siege; do not claim).
3. codex/unity-ai-command-dispatch rebase pending on Codex side; master now at revision 21 so Codex should bump to 22 on rebase. codex/unity-fortification-siege will bump to 23.

## 2026-04-19 Codex Command Dispatch Rebase To Revision 22

- Status: rebased green on `codex/unity-ai-command-dispatch` at
  `34b8d694c4726345a63ce1577d63e34d4bdce5e1`.
- Base: `origin/master` `00847e77ab8d7e085adcbf5de4ac10baa584bcba`
  (contract revision 21, Claude sub-slice 9 merged).
- Contract updated 21 -> 22 under `codex-ai-command-dispatch-2026-04-19`.
- Validation on `D:\BLAICD\bloodlines` is fully green:
  - both `dotnet build` gates PASS
  - bootstrap runtime smoke PASS via `artifacts/unity-bootstrap-runtime-smoke.log`
  - combat smoke PASS
  - Bootstrap and Gameplay scene shells PASS via
    `artifacts/unity-bootstrap-scene-batch-rev22.log` and
    `artifacts/unity-gameplay-scene-batch-rev22.log`
  - fortification smoke PASS
  - siege smoke PASS
  - `node tests/data-validation.mjs` PASS
  - `node tests/runtime-bridge.mjs` PASS
  - contract staleness check PASS at revision 22
  - dedicated AI command dispatch smoke PASS via
    `artifacts/unity-ai-command-dispatch-batch-rev22.log`
- Note: checked-in bootstrap, scene-shell, fortification, and command-dispatch
  wrappers are still pinned to `D:\ProjectsHome\Bloodlines`, so the rebased worktree
  used direct worktree-local execute-method validation for those gates.
- Next recommended action:
  1. Force-push `codex/unity-ai-command-dispatch`.
  2. Merge it to `master` and push `master`.
  3. Rebase `codex/unity-fortification-siege` onto the new revision-22 master base,
     bump the contract to 23, rerun the full gate chain plus imminent-engagement
     smoke, and merge that lane next.

## 2026-04-19 Codex Fortification Siege Rebase To Revision 23

- Status: rebased green on `codex/unity-fortification-siege`.
- Base: `origin/master` `73a6f5435c047c303c256a822b5f785f0199d277`
  (revision 22 with Codex command-dispatch already merged).
- Contract updated `22 -> 23` under
  `codex-fortification-siege-imminent-engagement-2026-04-19`.
- Validation on `D:\BLFS\bloodlines` is fully green:
  - both `dotnet build` gates PASS
  - bootstrap runtime smoke PASS via `artifacts/unity-bootstrap-runtime-smoke.log`
  - combat smoke PASS via `artifacts/unity-combat-smoke.log`
  - Bootstrap and Gameplay scene shells PASS via
    `artifacts/unity-bootstrap-scene-batch-rev23.log` and
    `artifacts/unity-gameplay-scene-batch-rev23.log`
  - fortification smoke PASS via `artifacts/unity-fortification-smoke.log`
  - siege smoke PASS via `artifacts/unity-siege-smoke.log`
  - imminent engagement smoke PASS via
    `artifacts/unity-imminent-engagement-smoke.log`
  - `node tests/data-validation.mjs` PASS
  - `node tests/runtime-bridge.mjs` PASS
  - contract staleness check PASS at revision 23
- Branch authority handoff:
  `docs/unity/session-handoffs/2026-04-18-unity-fortification-siege-imminent-engagement-warnings.md`
- Next action:
  1. Force-push `codex/unity-fortification-siege`.
  2. Merge it to `master` and push `master`.
  3. Start the next ai-strategic-layer lane from revision 23 on a new Claude branch.

## AI Strategic Layer Sub-Slice 10: Marriage Strategic Profile (Claude, 2026-04-19)

### Status: COMPLETE on branch claude/unity-ai-marriage-strategic-profile

### What Was Done
- Ported ai.js `getAiMarriageStrategicProfile` (~2803-2839) into new `AIMarriageStrategicProfileSystem`. Runs before `AICovertOpsSystem`, closes the gap where sub-slices 6/8/9 referenced `MarriageProposalGateMet` and `MarriageInboxAcceptGate` but nothing populated them.
- 4 strategic signals per browser: isHostile (HostilityComponent buffer), populationDeficit (<85% of player), legitimacyDistress gated by faith compat (<50), successionPressure (either side active). signalCount + blockedByFaith -> willing.
- Faith compatibility simplified to SelectedFaith+DoctrinePath equality (harmonious / either-match / incompatible / unbound); preserves mechanical intent without browser covenantName metadata.
- Writes `willing` to both gate flags symmetrically per ai.js comment that accept gate reuses proposal strategic criteria.
- Target hardcoded to `"player"` matching browser; multi-faction extension reserved.
- Cross-lane reads only; no structural edits to retired lanes.
- 6-phase smoke validator covers no-signal, each individual signal path, harmonious-faith legitimacy path, and incompatible-faith-blocks-weak-match path. Contract revision 23 -> 24.

### Gate Results
- dotnet build Assembly-CSharp.csproj: 0 errors
- dotnet build Assembly-CSharp-Editor.csproj: 0 errors
- Bootstrap runtime smoke: PASS
- Combat smoke: exit 0
- Scene shells: Bootstrap + Gameplay green
- Fortification smoke: PASS
- Siege smoke: exit 0
- AI marriage strategic profile smoke: Phase 1-6 PASS
- data-validation.mjs: PASS
- runtime-bridge.mjs: PASS
- Contract staleness check: PASSED (bumped to revision 24 post-gate)

### Next Unclaimed Lanes
1. ai-strategic-layer-sub-slice-11-marriage-accept-effects (new branch claude/unity-ai-marriage-accept-effects; simulation.js acceptMarriage ~7388-7469 effects block: +2 legitimacy both sides, hostility drop both ways, oathkeeping conviction +2, 30-day declareInWorldTime jump).
2. fortification-siege sub-slice 4+ (Codex owns; lane still active after sub-slice 3 imminent engagement landed at rev 23).

## AI Strategic Layer Sub-Slice 11: Marriage Accept Effects (Claude, 2026-04-19)

### Status: COMPLETE on branch claude/unity-ai-marriage-accept-effects

### What Was Done
- New `AIMarriageAcceptEffectsSystem` + `MarriageAcceptEffectsPendingTag`. Ports simulation.js `acceptMarriage` (~7388-7469) post-record effects block.
- `AIMarriageInboxAcceptSystem` (sub-slice 9) now attaches the pending tag to the primary marriage entity at creation. Mirror record untagged.
- Effects applied exactly once per marriage: legitimacy +2 clamped 100 both dynasties, hostility drop both ways (HostilityComponent buffer), 30-day DeclareInWorldTimeRequest push onto DualClock singleton, oathkeeping +2 on both factions via `ConvictionScoring.ApplyEvent`.
- Null-safe: each effect checks component/buffer presence before mutating.
- Deferred: governance authority legitimacy cost (needs getMarriageAcceptanceTerms port), narrative message push (needs UI message component).
- 6-phase smoke validator all green. Contract revision 24 -> 25.

### Gate Results
- dotnet build Assembly-CSharp.csproj: 0 errors
- dotnet build Assembly-CSharp-Editor.csproj: 0 errors
- Bootstrap runtime smoke: PASS
- Combat smoke: exit 0
- Scene shells: Bootstrap + Gameplay green
- Fortification smoke: PASS
- Siege smoke: exit 0
- AI marriage accept effects smoke: Phase 1-6 PASS
- data-validation.mjs: PASS
- runtime-bridge.mjs: PASS
- Contract staleness check: PASSED (bumped to revision 25 post-gate)

### Next Unclaimed Lanes
1. ai-strategic-layer-sub-slice-12-marriage-acceptance-terms (new branch claude/unity-ai-marriage-acceptance-terms; ports `getMarriageAcceptanceTerms` and governance-authority legitimacy cost on accept).
2. fortification-siege sub-slice 4+ (Codex owns; lane still active after sub-slice 3 imminent engagement landed at rev 23).

## AI Strategic Layer Sub-Slice 12: Marriage Acceptance Terms (Claude, 2026-04-19)

### Status: COMPLETE on branch claude/unity-ai-marriage-acceptance-terms

### What Was Done
- New `MarriageAcceptanceTermsComponent` (AuthorityMode + LegitimacyCost) and `MarriageAuthorityEvaluator` static helper. Ports simulation.js `getMarriageAuthorityProfile` (~6134), `getMarriageAcceptanceTerms` (~6327), `applyMarriageGovernanceLegitimacyCost` (~6232), and `MARRIAGE_REGENCY_LEGITIMACY_COSTS` (~6091).
- `MarriageAuthorityEvaluator` walks the target faction's `DynastyMemberRef` buffer in browser priority order: HeadDirect (head_of_bloodline + Ruling, cost 0), HeirRegency (heir_designate available, cost 1), EnvoyRegency (diplomat available, cost 2). Returns false (reject) when a roster exists but yields none of those. Backward-compatible default: empty roster -> HeadDirect cost 0.
- `AIMarriageInboxAcceptSystem` resolves the authority before `TryAcceptIncoming`. On reject, dispatch is cleared and accept short-circuits (matches browser `getMarriageAcceptanceTerms` rejection). On success the resolved terms are attached to the primary marriage entity as `MarriageAcceptanceTermsComponent` alongside the pending tag.
- `AIMarriageAcceptEffectsSystem` reordered to browser order (cost first, hostility, oathkeeping, legitimacy +2, declare time). New helper `ApplyAuthorityLegitimacyCost` reads the terms component, deducts cost from spouse `DynastyStateComponent.Legitimacy` clamped [0, 100], and records a Stewardship -cost conviction event via `ConvictionScoring.ApplyEvent`. Skips silently when terms are absent (preserves sub-slice 11 phase-3 untagged synthetic test).
- Cost-before-bonus net spouse legitimacy = min(100, max(0, L - cost) + 2). Head-direct (cost 0) yields the same +2 baseline as sub-slice 11; regency costs partially absorb the bonus when below the ceiling.
- Terms component persists after the pending tag is removed, providing a durable provenance marker for downstream HUD or audit systems.
- Cross-lane reads only: `DynastyMemberRef`, `DynastyMemberComponent.Role/Status`. Cross-lane mutations are field-level only: `DynastyStateComponent.Legitimacy` clamped [0, 100], `ConvictionComponent.Stewardship` via the existing scoring helper.
- Deferred: narrative message push (still no AI->UI message component).
- 5-phase dedicated smoke validator all green. Sub-slice 11 regression smoke also re-run green. Contract revision 25 -> 26.

### Gate Results
- dotnet build Assembly-CSharp.csproj: 0 errors
- dotnet build Assembly-CSharp-Editor.csproj: 0 errors
- Bootstrap runtime smoke: PASS
- Combat smoke: exit 0
- Scene shells: Bootstrap + Gameplay green
- Fortification smoke: PASS
- Siege smoke: exit 0
- AI marriage accept effects smoke (sub-slice 11 regression): Phase 1-6 PASS
- AI marriage acceptance terms smoke (sub-slice 12): all 5 phases PASS (head-direct, heir-regency, envoy-regency, no-authority, terms-persisted)
- data-validation.mjs: PASS
- runtime-bridge.mjs: PASS
- Contract staleness check: PASSED (bumped to revision 26 post-gate)

### Next Unclaimed Lanes
1. ai-strategic-layer-sub-slice-13-narrative-message-bridge (designs an AI->UI message channel so `acceptMarriage` ceremonial pushMessage and other AI events can surface to the player; deferred since sub-slice 11).
2. fortification-siege sub-slice 4+ (Codex owns; lane still active).

## 2026-04-19 Codex Fortification Siege Sub-Slice 4: Camp Supply Interdiction

### Status: COMPLETE on branch codex/unity-fortification-siege-camp-supply-interdiction

### What Was Done
- Chosen scope: siege camp supply interdiction. This was the highest-value bounded fortification follow-up that stayed entirely inside Codex-owned siege and fortification surfaces while feeding the already-landed AI siege orchestration phases.
- Added `SiegeSupplyCampComponent` for live logistics stockpile state on logistics-support buildings and `SiegeSupplyInterdictionCanon` for browser-derived convoy and raid constants.
- Added `SiegeSupplyInterdictionSystem` to scan raiders and escorts, drain contested camp stockpiles, mark wagons interdicted and recovering, record convoy screening and reconsolidation, apply direct resource loss on wagon hits, and write `InterdictedWagonCount`, `RecoveringWagonCount`, and `ConvoyRecoveringUnscreenedCount` into `AISiegeOrchestrationComponent` without editing Claude-owned AI files.
- Extended `SiegeSupplyTrainComponent` with additive interdiction and recovery fields; extended `SiegeSupportStatus` with `Interdicted`, `RecoveringUnscreened`, and `RecoveringScreened`.
- Updated `SiegeComponentInitializationSystem`, `SiegeSupportRefreshSystem`, `FieldWaterSupportScanSystem`, and `FieldWaterStrainSystem` so camps and wagons stop providing siege or field-water sustainment while the logistics chain is interdicted or recovering.
- Added debug helpers to `BloodlinesDebugCommandSurface.Siege` for wagon and camp logistics state.
- Added dedicated validator `BloodlinesSiegeSupplyInterdictionSmokeValidation` plus wrapper `scripts/Invoke-BloodlinesUnitySiegeSupplyInterdictionSmokeValidation.ps1`.
- Important validation note: the checked-in bootstrap-runtime and scene-shell wrappers are still pinned to `D:\ProjectsHome\Bloodlines`, so the worktree at `D:\BLFS\bloodlines` used worktree-local direct execute-method validation for those gates rather than committing shared wrapper churn.

### Gate Results
- `dotnet build unity/Assembly-CSharp.csproj -nologo`: PASS
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`: PASS
- Bootstrap runtime smoke: PASS via `artifacts/unity-bootstrap-runtime-smoke.log`
- Combat smoke: PASS via `artifacts/unity-combat-smoke.log`
- Bootstrap scene shell: PASS via `artifacts/unity-bootstrap-scene-batch-rev27-codex.log`
- Gameplay scene shell: PASS via `artifacts/unity-gameplay-scene-batch-rev27-codex.log`
- Fortification smoke: PASS via `artifacts/unity-fortification-smoke.log`
- Siege smoke: PASS via `artifacts/unity-siege-smoke.log`
- `node tests/data-validation.mjs`: PASS
- `node tests/runtime-bridge.mjs`: PASS
- Contract staleness check: PASS at revision 27
- Dedicated siege supply interdiction smoke: PASS via `artifacts/unity-siege-supply-interdiction-smoke.log`
- Dedicated smoke marker: `BLOODLINES_SIEGE_SUPPLY_INTERDICTION_SMOKE PASS`

### Continuity Updates
- Contract bumped `26 -> 27` under `codex-fortification-siege-camp-supply-interdiction-2026-04-19`.
- New slice handoff: `docs/unity/session-handoffs/2026-04-19-unity-fortification-siege-camp-supply-interdiction.md`.
- `CURRENT_PROJECT_STATE.md`, `NEXT_SESSION_HANDOFF.md`, and `continuity/PROJECT_STATE.json` updated additively with the new fortification entry while preserving Claude sub-slice 12.

### Recommended Next Fortification Follow-Up
1. Wall-segment destruction resolution.
2. Gate breach mechanics.
3. Casualty accumulation and morale collapse.

## Owner Direction Update: Game Modes And Dynasty Progression (Lance, 2026-04-19)

### Status: LANDED in governance/

### What Changed
- New owner direction file: `governance/OWNER_DIRECTION_2026-04-19_GAME_MODES_AND_DYNASTY_PROGRESSION.md`. Amends 2026-04-16 and 2026-04-17 owner directions on three points; the rest of those documents stays active.
- Shipping game modes are now skirmish vs AI and multiplayer only. Story campaign and interactive tutorial mode removed from scope (not deferred). Onboarding lives in HUD tooltips and panel labels.
- Asset production is primarily AI-generated. Delivered fidelity may run somewhat below the Generals Zero Hour / Warcraft III ceiling but never above it. All "no PBR / no HDRP / no ray tracing / no AAA pipelines" rules from 2026-04-17 remain in full effect.
- New canonical system in scope: a cross-match dynasty progression system. Top-performing dynasties (not strictly #1) accrue XP that unlocks tiers. Tier bonuses are sideways customization options (canonical example: swap a dynasty-specific special unit for another from the same house's progression options) so non-#1 placements stay rewarding. Design and `data/` surface to be added when the design lands.
- `CLAUDE.md` Owner Direction Override section updated to reference the new file. Governing-implications list amended for game modes and progression. Current Direction polish list no longer mentions tutorials or campaign.
- `03_PROMPTS/BLOODLINES_UNITY_CONTINUITY_PROMPT_v3.md` Presentation block UX line updated to drop tutorials and campaign framework, route onboarding through HUD tooltips, and reference the 2026-04-19 owner direction.

### Implication For Sub-Slice 13 And Beyond
- The narrative message bridge candidate is unchanged in priority. It remains the highest-leverage cleanup item once a UI consumer is in scope.
- The dynasty progression system is canonically in scope but should not be prioritized ahead of the in-flight ai-strategic-layer and fortification-siege lanes. It gets its own slice plan and `data/` file when the design lands.
- Any planning doc, prompt, or backlog item that lists campaign or tutorial work as in-flight should be treated as stale.

### Gate Results
- N/A. Governance-only edits. No code changed. No validator run required.
- Contract revision unchanged (the contract tracks Unity lane scope, not owner direction).

## AI Strategic Layer Sub-Slice 13: Lesser-House Promotion Execution (Claude, 2026-04-19)

### Status: COMPLETE on branch claude/unity-ai-lesser-house-promotion

### What Was Done
- New `AILesserHousePromotionSystem` ports ai.js `tryAiPromoteLesserHouse` (~2784-2801) plus the mechanical core of simulation.js `promoteMemberToLesserHouse` (~7184-7258). Sub-slice 6 already dispatched `CovertOpKind.LesserHousePromotion` into AICovertOpsComponent; sub-slice 13 consumes it and executes the founding effect.
- Strategic gates ported: faction `DynastyStateComponent.Legitimacy < 90` (consolidation ceiling), `LesserHouseElement` buffer count `< 3` (cap), `DynastyMemberRef` buffer present.
- Eligibility gates (browser memberIsLesserHouseCandidate parity): `Status == Active || Ruling`, `Role != HeadOfBloodline`, `Path` is `Governance`, `MilitaryCommand`, or `CovertOperations`, `Renown >= 30`, MemberId not already a `FounderMemberId` of an existing LesserHouseElement on this faction. The browser's `member.foundedLesserHouseId` field is replaced by cross-referencing the buffer.
- On success: append new `LesserHouseElement` at Loyalty=75, +3 dynasty legitimacy clamped to 100, +2 Stewardship conviction event via `ConvictionScoring.ApplyEvent`. Dispatch unconditionally cleared (one-shot pattern from sub-slices 8/9/12). New buffer entry initialized with `LastDriftAppliedInWorldDays = currentInWorldDays` so the existing `LesserHouseLoyaltyDriftSystem` picks it up at the next in-world day boundary.
- Cross-lane reads only (DynastyMemberRef, DynastyMemberComponent fields). Cross-lane mutations: DynastyState.Legitimacy clamp, LesserHouseElement append, Conviction.Stewardship via existing helper.
- Deferred: per-member FoundedLesserHouseId field on DynastyMemberComponent; promotion-history gate; marital-anchor + cadet world-pressure profiles; narrative pushMessage (still waits on AI->UI message bridge).
- 6-phase dedicated smoke validator all green. Contract revision 27 -> 28.
- csproj refresh note: the local csproj was missing Codex's sub-slice 4 files (SiegeSupplyCampComponent, SiegeSupplyInterdictionCanon, SiegeSupplyInterdictionSystem, BloodlinesSiegeSupplyInterdictionSmokeValidation). Both csproj files updated; csproj is gitignored and not committed.

### Gate Results
- dotnet build Assembly-CSharp.csproj: 0 errors
- dotnet build Assembly-CSharp-Editor.csproj: 0 errors
- Bootstrap runtime smoke: PASS
- Combat smoke: exit 0
- Scene shells: Bootstrap + Gameplay green
- Fortification smoke: PASS
- Siege smoke: exit 0
- AI lesser house promotion smoke (sub-slice 13): all 6 phases PASS
- data-validation.mjs: PASS
- runtime-bridge.mjs: PASS
- Contract staleness check: PASSED at revision 28

### Next Unclaimed Lanes
1. ai-strategic-layer-sub-slice-14-narrative-message-bridge (designs an AI->UI message channel; deferred since sub-slices 11, 12, 13).
2. ai-strategic-layer-sub-slice-15-pact-proposal-execution (mirrors marriage-proposal pattern from sub-slice 8; ports `proposeNonAggressionPact` from ai.js ~2643-2665).
3. fortification-siege wall-segment destruction resolution (Codex owns; recommended next Codex slice per sub-slice 4 handoff).

## AI Strategic Layer Sub-Slice 14: Non-Aggression Pact Proposal Execution (Claude, 2026-04-19)

### Status: COMPLETE on branch claude/unity-ai-pact-proposal-execution

### What Was Done
- New `PactComponent` (one entity per active pact, symmetric FactionAId/FactionBId; no primary/mirror split since pacts have no asymmetric downstream system) and new `AIPactProposalExecutionSystem` port ai.js pact dispatch (~2643-2666) and simulation.js `getNonAggressionPactTerms` (~5150-5183) / `proposeNonAggressionPact` (~5185-5222).
- Consumes `AICovertOpsComponent.LastFiredOp == CovertOpKind.PactProposal` from sub-slice 6. Gates on: both kingdoms (FactionKind.Kingdom), source != target, source hostile to target via HostilityComponent buffer, no existing PactComponent between them, source resources >= 50 Influence + 80 Gold.
- On success: deduct cost from source ResourceStockpileComponent, remove HostilityComponent entries both ways, create one PactComponent entity with MinimumExpiresAtInWorldDays = start + 180. Dispatch unconditionally cleared (one-shot pattern from sub-slices 8/9/12/13). Target hardcoded as "player" matching browser ai.js:2658.
- Deferred: holy-war pact gate (waits on holy-war lane); pact expiration / break system; narrative pushMessage (same message-bridge blocker as sub-slices 11, 12, 13).
- 6-phase dedicated smoke validator all green. Contract revision 28 -> 29.

### Gate Results
- dotnet build Assembly-CSharp.csproj: 0 errors
- dotnet build Assembly-CSharp-Editor.csproj: 0 errors
- Bootstrap runtime smoke: PASS
- Combat smoke: exit 0
- Scene shells: Bootstrap + Gameplay green
- Fortification smoke: PASS
- Siege smoke: exit 0
- AI pact proposal execution smoke (sub-slice 14): all 6 phases PASS
- data-validation.mjs: PASS
- runtime-bridge.mjs: PASS
- Contract staleness check: PASSED at revision 29

### Next Unclaimed Lanes
1. ai-strategic-layer-sub-slice-15-pact-break-and-expiration (ports `breakNonAggressionPact` from simulation.js ~5224 + early-break legitimacy penalty; extends PactComponent.Broken/BrokenByFactionId).
2. ai-strategic-layer narrative message bridge (still deferred across sub-slices 11-14).
3. Other CovertOpKind executions (assassination, missionary, holy war, divine right, captive rescue, captive ransom).
4. fortification-siege wall-segment destruction resolution (Codex owns).

## AI Strategic Layer Sub-Slice 15: Pact Break And Expiration (Claude, 2026-04-19)

### Status: COMPLETE on branch claude/unity-ai-pact-break-expiration

### What Was Done
- New `PactBreakRequestComponent` (request marker: PactId + RequestingFactionId) and `PactBreakSystem` port simulation.js `breakNonAggressionPact` (~5224-5257). Request-entity pattern mirrors browser's explicit-only break semantic; no auto-expiration.
- Per request: find matching PactComponent, short-circuit if missing or already broken; mark Broken=true + BrokenByFactionId; re-establish hostility both ways idempotently; apply breaker legitimacy -8 clamped [0, 100]; apply breaker Oathkeeping -2 via ConvictionScoring.ApplyEvent (bucket-layer mapping of browser's direct conviction.score -= 2). Request entity destroyed after processing.
- Penalty is unconditional regardless of early-break timing (browser earlyBreak flag affects only messaging). No target-side penalty.
- 5-phase dedicated smoke validator all green. Contract revision 29 -> 30.

### Gate Results
- dotnet build Assembly-CSharp.csproj: 0 errors
- dotnet build Assembly-CSharp-Editor.csproj: 0 errors
- Bootstrap runtime smoke: PASS
- Combat smoke: exit 0
- Scene shells: Bootstrap + Gameplay green
- Fortification smoke: PASS
- Siege smoke: exit 0
- Pact break smoke (sub-slice 15): all 5 phases PASS
- data-validation.mjs: PASS
- runtime-bridge.mjs: PASS
- Contract staleness check: PASSED at revision 30

### Next Unclaimed Lanes
1. ai-strategic-layer-sub-slice-16-captive-recovery-execution (ports CovertOpKind.CaptiveRescue / CaptiveRansom consumers from ai.js ~2566-2608).
2. ai-strategic-layer narrative message bridge (still deferred across sub-slices 11-15).
3. Other CovertOpKind executions (assassination, missionary, holy war, divine right).
4. fortification-siege wall-segment destruction resolution (Codex owns).

## AI Strategic Layer Sub-Slice 16: Narrative Message Bridge (Claude, 2026-04-19)

### Status: COMPLETE on branch claude/unity-ai-narrative-message-bridge (rebased onto origin/master 40e80e03 after Codex's fortification-siege sub-slice 5 landed at revision 31)

### What Was Done
- New `NarrativeMessageComponents` (NarrativeMessageSingleton tag + NarrativeMessageElement buffer { Text: FixedString128Bytes, Tone: { Info, Good, Warn }, CreatedAtInWorldDays, Ttl }) and `NarrativeMessageBridge` (static Push / Count helpers with lazy-singleton creation). Ports the browser `pushMessage` surface from simulation.js (many call sites).
- PactBreakSystem (sub-slice 15) wired as first consumer: after mechanical break effects, builds the browser ceremonial line with the same early-breach vs hostility-resumes suffix and routes tone via breaker-is-player -> Warn else Info.
- Append-order buffer (browser uses unshift; Unity consumers iterate per their preference). No consumer/drain/TTL-eviction system yet. Global scoping; per-faction scoping reserved for a future multiplayer slice.
- 6-phase dedicated smoke validator all green. Sub-slice 15 regression smoke all 5 phases still PASS.
- Contract revision 31 -> 32.

### Gate Results
- dotnet build Assembly-CSharp.csproj: 0 errors
- dotnet build Assembly-CSharp-Editor.csproj: 0 errors
- Bootstrap runtime smoke: PASS
- Combat smoke: exit 0
- Scene shells: Bootstrap + Gameplay green
- Fortification smoke: PASS
- Siege smoke: exit 0 (passed on retry after transient bee_backend cache-rebuild error referencing a stale FortificationStructureResolutionSystem.cs path in Unity Library; retried once per protocol and passed cleanly)
- Pact break smoke (sub-slice 15 regression): all 5 phases PASS
- Narrative message bridge smoke (sub-slice 16): all 6 phases PASS
- data-validation.mjs: PASS
- runtime-bridge.mjs: PASS
- Contract staleness check: PASSED at revision 32

### Next Unclaimed Lanes
1. ai-strategic-layer-sub-slice-17-back-wire-narrative-pushes (add NarrativeMessageBridge.Push calls to AIMarriageAcceptEffectsSystem, AIMarriageInboxAcceptSystem, AILesserHousePromotionSystem, AIPactProposalExecutionSystem so the ceremonial lines deferred by sub-slices 11, 12, 13, 14 now surface; one-line additive edits following the PactBreakSystem pattern from sub-slice 16).
2. ai-strategic-layer-sub-slice-18-dynasty-operations-foundation (new DynastyOperationComponent entity shape + DYNASTY_OPERATION_ACTIVE_LIMIT gate; unblocks missionary, holy war, divine right, captive rescue, and ransom execution sub-slices).
3. ai-strategic-layer-sub-slice-19-captive-member-state (port faction.captives + CapturedMemberRecord shape; needed for captive rescue/ransom execution on top of sub-slice 18 foundation).
4. fortification-siege breach-aware follow-up (Codex owns; use FortificationComponent.OpenBreachCount in breach-aware assault, pathing, or HUD/legibility logic on a fresh `codex/unity-fortification-*` branch).

## AI Strategic Layer Sub-Slice 17: Narrative Back-Wire (Claude, 2026-04-19)

### Status: COMPLETE on branch claude/unity-ai-narrative-back-wire (branched from origin/master 8826d855 after sub-slice 16 narrative message bridge landed at revision 32)

### What Was Done
- Back-wires `NarrativeMessageBridge.Push` into the three AI systems that deferred their browser ceremonial lines in earlier slices: `AIMarriageAcceptEffectsSystem` (marriage accept line at simulation.js:7463, sub-slice 11+12), `AILesserHousePromotionSystem` (founding line at simulation.js:7251-7255, sub-slice 13), `AIPactProposalExecutionSystem` (pact entry line at simulation.js:5216-5220, sub-slice 14). Each wire-up is a small additive edit following the PactBreakSystem pattern from sub-slice 16.
- Marriage accept push reads `MarriageAcceptanceTermsComponent.AuthorityMode` + `LegitimacyCost` for the approval suffix (HeadDirect -> "under head approval"; HeirRegency -> "under heir regency (legitimacy -1)"; EnvoyRegency -> "under envoy regency (legitimacy -2)"). Member titles resolve via `DynastyMemberRef` with a direct FixedString64Bytes `MemberId` fallback. Player as head promotes to Good tone, else Info.
- Lesser-house push composes `"<factionId> founds <lesserHouseName>, honoring <founderTitle>."` with Good tone for player faction, else Info.
- Pact proposal push composes `"<sourceFactionId> and <targetFactionId> enter a non-aggression pact. Hostility ceases for at least 180 in-world days."` with Good tone for player source, else Info. Target is hardcoded to "player" in the system, so the Good branch is defensive for future extension.
- Title fallback rewrite: `ResolveMemberTitle` returns the raw MemberId directly. Byte-by-byte FixedString.Append promotes through the int overload and writes decimal digits rather than characters; direct FixedString assignment preserves the UTF-8 bytes verbatim.
- `BloodlinesAINarrativeBackWireSmokeValidation` 6-phase validator all green. Sub-slice 11, 13, 14, 15, 16 regression smokes all re-run green.
- Contract revision 32 -> 33.

### Gate Results
- dotnet build Assembly-CSharp.csproj: 0 errors
- dotnet build Assembly-CSharp-Editor.csproj: 0 errors
- Bootstrap runtime smoke: PASS
- Combat smoke: 8 phases PASS
- Scene shells: Bootstrap + Gameplay green
- Fortification smoke: PASS
- Siege smoke: PASS
- AI narrative back-wire smoke (sub-slice 17): all 6 phases PASS
- AI marriage accept effects regression (sub-slice 11): PASS
- AI lesser-house promotion regression (sub-slice 13): PASS
- AI pact proposal execution regression (sub-slice 14): PASS
- Pact break regression (sub-slice 15): PASS
- Narrative message bridge regression (sub-slice 16): PASS
- data-validation.mjs: PASS
- runtime-bridge.mjs: PASS
- Contract staleness check: PASSED at revision 33

### Next Unclaimed Lanes
1. ai-strategic-layer-sub-slice-18-dynasty-operations-foundation (new DynastyOperationComponent entity shape + DYNASTY_OPERATION_ACTIVE_LIMIT gate; unblocks missionary, holy war, divine right, captive rescue, and ransom execution sub-slices).
2. ai-strategic-layer-sub-slice-19-captive-member-state (port faction.captives + CapturedMemberRecord shape; needed for captive rescue/ransom execution on top of sub-slice 18 foundation).
3. ai-strategic-layer narrative TTL eviction system (walks the NarrativeMessageElement buffer each tick and removes entries whose CreatedAtInWorldDays + Ttl is past current; small self-contained slice that bounds the buffer before a UI consumer lands).
4. fortification-siege follow-up (Codex has `codex/unity-fortification-breach-assault-pressure` in flight locally but not yet on origin/master; breach-aware assault, pathing, or HUD/legibility is still unclaimed on a fresh `codex/unity-fortification-*` branch).

## Codex Fortification Siege Sub-Slice 6: Breach Assault Pressure (2026-04-19)

### Status: COMPLETE on rebased branch codex/unity-fortification-breach-assault-pressure

### What Was Done
- Chosen scope: breach-aware assault pressure. This was the highest-leverage `OpenBreachCount` consumer that stayed inside Codex-owned fortification/siege surfaces. Pathing was deferred because the active contract does not give this lane ownership of `unity/Assets/_Bloodlines/Code/Pathing/**`, and debug-only legibility was lower leverage than making breaches alter live assault outcomes.
- New `BreachAssaultPressureSystem` runs after `FortificationDestructionResolutionSystem` and before `FieldWaterStrainSystem`. It scans breached settlements and grants hostile units inside the breached settlement threat radius a bounded exploitation bonus, while resetting the state cleanly when no breach applies.
- `FieldWaterComponent` now carries additive breach telemetry (`BreachAssaultAdvantageActive`, `BreachOpenCount`, `BreachTargetSettlementId`, `BreachAssaultAttackMultiplier`, `BreachAssaultSpeedMultiplier`). `FieldWaterStrainSystem` now multiplies breach pressure into the existing attack/speed resolution path, so the change affects live unit performance without touching combat-lane files.
- `SiegeSupportCanon` now defines the bounded breach bonus: `+8%` attack and `+4%` speed per open breach, capped at 3 breaches.
- `BloodlinesSiegeSmokeValidation` now includes the new breach-pressure system in its validation-world setup so the owned siege smoke matches live runtime ordering.
- New dedicated validator `BloodlinesBreachAssaultPressureSmokeValidation` plus wrapper `scripts/Invoke-BloodlinesUnityBreachAssaultPressureSmokeValidation.ps1`. All 4 phases PASS:
  - Phase 1 intact wall -> no assault bonus
  - Phase 2 one breach -> hostile attacker gets `1.08x` attack and `1.04x` speed
  - Phase 3 settlement owner excluded
  - Phase 4 two breaches -> near attacker scales to `1.16x` attack while far attacker stays baseline
- Local csproj refresh note: after rebasing onto `dfec72f5`, the generated csproj files were missing the already-landed fortification files `FortificationStructureResolutionSystem.cs` and `BloodlinesWallSegmentDestructionSmokeValidation.cs`, plus the new breach-assault files. Both generated csproj files were refreshed locally so the governed `dotnet build` gates could run. The csproj files remain gitignored and are not part of the commit.
- Contract revision advanced `33 -> 34`. New per-slice handoff: `docs/unity/session-handoffs/2026-04-19-unity-fortification-siege-breach-assault-pressure.md`.

### Gate Results
- `dotnet build unity/Assembly-CSharp.csproj -nologo`: PASS
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`: PASS
- Bootstrap runtime smoke: PASS
- Combat smoke: PASS
- Scene shells: Bootstrap + Gameplay PASS
- Fortification smoke: PASS
- Siege smoke: PASS
- `node tests/data-validation.mjs`: PASS
- `node tests/runtime-bridge.mjs`: PASS
- Contract staleness check: PASS at revision 34
- Dedicated breach assault pressure smoke: PASS via `artifacts/unity-breach-assault-pressure-smoke.log` with marker `BLOODLINES_BREACH_ASSAULT_PRESSURE_SMOKE PASS`

### Recommended Next Fortification Follow-Up
1. Breach HUD / legibility summary on the fortification debug surface so the new assault-pressure state is readable per settlement without inspecting unit telemetry.
2. Coordinated breach-aware pathing / exploitation after pathing ownership is explicitly claimed or split in the contract.
3. Breach sealing, assault failure, or recovery follow-up so defenders can contest the new exploit window over time instead of only suffering the static bonus.
## AI Strategic Layer Sub-Slice 18: Dynasty Operations Foundation (Claude, 2026-04-19)

### Status: COMPLETE on branch claude/unity-ai-dynasty-operations-foundation (branched from origin/master dfec72f5, rebased onto origin/master a2f5e6cd after Codex fortification sub-slice 6 breach assault pressure landed at revision 34)

### What Was Done
- New `DynastyOperationComponent` + `DynastyOperationLimits` port the canonical dynasty-operation foundation that every browser dispatch site in simulation.js shares: `DYNASTY_OPERATION_ACTIVE_LIMIT = 6` (line 17) plus the seven dispatch sites (missionary ~10523, holy war ~10565, counter-intelligence ~10837, espionage ~10876, assassination ~10912, sabotage ~10952, captive rescue/ransom ~2566-2608 in ai.js) that gate on `(operations.active ?? []).length >= DYNASTY_OPERATION_ACTIVE_LIMIT`.
- `DynastyOperationComponent` carries one entity per active operation with OperationId (FixedString64Bytes), SourceFactionId (FixedString32Bytes), OperationKind (new `DynastyOperationKind` enum: None, Missionary, HolyWar, DivineRight, CaptiveRescue, CaptiveRansom, LesserHousePromotion), StartedAtInWorldDays (stamped from DualClock), TargetFactionId (default when N/A), TargetMemberId (default when N/A), Active (false retains for audit without consuming capacity).
- `DynastyOperationLimits` static helper exposes the cap constant, `HasCapacity(em, factionId)` (strict < cap, skips inactive), `CountActiveForFaction(em, factionId)`, and `BeginOperation(em, operationId, sourceFactionId, kind, targetFactionId, targetMemberId)` (creates entity with Active=true + DualClock-stamped StartedAtInWorldDays).
- Deliberate departure from browser silent-trim: Unity keeps the cap gate strict at the call site because entity-per-operation cannot silently drop entries without orphaning per-kind data, and silent drop masks over-cap bugs at the producer.
- No system ships in this slice. The foundation ships standalone so each later dispatch slice (missionary, holy war, divine right, captive rescue, captive ransom) attaches its own per-kind component struct to the entity created by BeginOperation. LesserHousePromotion reserved in enum for completeness since browser `promoteMemberToLesserHouse` does not route through `getDynastyOperationsState`.
- Cross-lane reads only: `DualClockComponent.InWorldDays` (dual-clock-match-progression, retired). No mutations.
- `BloodlinesDynastyOperationsSmokeValidation` 5-phase validator all green.
- Contract revision 34 -> 35.

### Gate Results
- dotnet build Assembly-CSharp.csproj: 0 errors
- dotnet build Assembly-CSharp-Editor.csproj: 0 errors
- Bootstrap runtime smoke: PASS
- Combat smoke: exit 0
- Scene shells: Bootstrap + Gameplay green
- Fortification smoke: PASS
- Siege smoke: exit 0 (now includes Codex's breach-pressure system on the rebased base; no regression)
- Dynasty operations smoke (sub-slice 18): all 5 phases PASS
- data-validation.mjs: PASS
- runtime-bridge.mjs: PASS
- Contract staleness check: PASSED at revision 35

### Next Unclaimed Lanes
1. ai-strategic-layer-sub-slice-19-captive-member-state (port faction.captives array + CapturedMemberRecord shape as a Unity buffer on the faction entity; together with sub-slice 18 unblocks captive rescue/ransom execution slices).
2. ai-strategic-layer missionary execution (first consumer of the dynasty operations foundation; port `startMissionaryOperation` at simulation.js:10523 with per-kind resolveAt, operatorId, sourceFaithId, exposureGain, loyaltyPressure fields on a new DynastyOperationMissionaryComponent struct attached to the entity created by BeginOperation).
3. ai-strategic-layer narrative TTL eviction system (walks the NarrativeMessageElement buffer each tick and removes entries whose CreatedAtInWorldDays + Ttl is past current; small self-contained slice that bounds the buffer before a UI consumer lands).
4. fortification-siege follow-up: breach HUD / legibility summary on the fortification debug surface; breach-aware pathing after pathing ownership is split; breach sealing / assault failure / recovery follow-up (per Codex sub-slice 6 handoff recommendations).

## Codex Fortification Siege Sub-Slice 7: Breach Legibility Readout (2026-04-19)

### Status: COMPLETE on branch codex/unity-fortification-breach-legibility-readout (branched from origin/master 374bb2e4 after Claude's ai-strategic-layer sub-slice 18 moved the contract to revision 35)

### What Was Done
- New `SettlementBreachReadout` plain-data struct under `unity/Assets/_Bloodlines/Code/Debug/` plus `TryDebugGetSettlementBreachReadout(FixedString32Bytes settlementId, out SettlementBreachReadout readout)` on `BloodlinesDebugCommandSurface`. The seam packages settlement id, owner faction id, open breach count, destroyed wall/tower/gate/keep counts, current tier, derived reserve frontage, and aggregate breach-assault state without a HUD consumer needing to query `FortificationComponent` and `FieldWaterComponent` directly.
- Reserve frontage is derived from the already-landed fortification reserve-frontage rule rather than stored separately. Aggregate breach-assault telemetry is reduced from live `FieldWaterComponent` entries that currently target the settlement, so the seam stays aligned with sub-slice 6's runtime consumer.
- No production HUD renderer ships in this slice. The seam is a foundation only, mirroring the no-consumer foundation pattern Claude used in ai-strategic-layer sub-slice 18 dynasty operations foundation.
- New dedicated validator `BloodlinesBreachLegibilityReadoutSmokeValidation` plus wrapper `scripts/Invoke-BloodlinesUnityBreachLegibilityReadoutSmokeValidation.ps1`. All 5 phases PASS:
  - Phase 1 intact fortified settlement baseline: zero breach counts, tier 1, frontage 2, baseline multipliers
  - Phase 2 single breach: `OpenBreachCount = 1`, active pressure true, aggregate attack `1.08x`, aggregate speed `1.04x`
  - Phase 3 three breaches: capped aggregate attack `1.24x`, capped aggregate speed `1.12x`
  - Phase 4 missing settlement: `false` + default readout
  - Phase 5 partial destruction variety: `walls=2`, `towers=1`, `gates=1`, `keeps=0`, `OpenBreachCount = 3`, tier 2, frontage 3
- Local csproj note: this fresh worktree initially had no Unity-generated `unity/Assembly-CSharp.csproj` or `unity/Assembly-CSharp-Editor.csproj`. Unity regenerated both locally, and both include the new breach-readout and validator entries. The csproj files remain gitignored and are not part of the commit.
- Contract revision advanced `35 -> 36`. New per-slice handoff: `docs/unity/session-handoffs/2026-04-19-unity-fortification-siege-breach-legibility-readout.md`.

### Gate Results
- `dotnet build unity/Assembly-CSharp.csproj -nologo`: PASS
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`: PASS with existing editor warnings only
- Bootstrap runtime smoke: PASS via `artifacts/unity-bootstrap-runtime-smoke.log`
- Combat smoke: PASS via `artifacts/unity-combat-smoke.log`
- Scene shells: Bootstrap + Gameplay PASS via `artifacts/unity-bootstrap-scene-validate.log` and `artifacts/unity-gameplay-scene-validate.log`
- Fortification smoke: PASS via `artifacts/unity-fortification-smoke.log`
- Siege smoke: PASS via `artifacts/unity-siege-smoke.log`
- `node tests/data-validation.mjs`: PASS
- `node tests/runtime-bridge.mjs`: PASS
- Contract staleness check: PASS at revision 36
- Dedicated breach legibility readout smoke: PASS via `artifacts/unity-breach-legibility-readout-smoke.log` with marker `BLOODLINES_BREACH_LEGIBILITY_READOUT_SMOKE PASS`

### Recommended Next Fortification Follow-Up
1. Optional fortification sub-slice 8 breach sealing / recovery: defenders restore `OpenBreachCount` toward zero over time through a new `BreachSealingSystem` that spends stone and worker time, so breach pressure becomes a contestable window instead of a static post-destruction state.
2. Coordinated breach-aware pathing / exploit routing after pathing ownership is explicitly claimed or split in the contract.
3. Production HUD consumer of `SettlementBreachReadout` after the sealing/recovery rule or pathing follow-up decides which breach signals should be rendered first.

## Unity AI Strategic Layer Bundle 1: Captive State (Sub-Slice 19) and Missionary Execution (Sub-Slice 20) Landed (2026-04-19)

Branch: `claude/unity-ai-captive-state-and-missionary-execution`. Master base: `ef58ec4f` (after Codex fortification-siege sub-slice 7 breach legibility readout landed at revision 36). Bundle 1 of the AI mechanical campaign per the 2026-04-19 roadmap decision: two logical sub-slices ship as one commit, one merge, and one contract revision bump.

### What Was Done
- **Sub-slice 19 captive member state**: new `CapturedMemberElement` IBufferElementData with `[InternalBufferCapacity(8)]` carries `MemberId`, `MemberTitle`, `OriginFactionId`, `CapturedAtInWorldDays` (DualClock-stamped), `RansomCost`, and `Status` (new `CapturedMemberStatus` enum: Held, RansomOffered, Released, Executed). Buffer attaches to the captor faction entity. Sub-slices 23/24 captive rescue/ransom dispatch will consume it. New `CapturedMemberHelpers` static class exposes `CaptureMember` (lazy-creates buffer, appends Held entry, returns buffer index), `TryGetCaptive` (lookup by member id with index), and `ReleaseCaptive` (mutates status in place, retains entry for audit). No consumer system ships in this slice; the data shape lands ahead of dispatch.
- **Sub-slice 20 missionary execution**: first production consumer of sub-slice 18 dynasty operations foundation. New `DynastyOperationMissionaryComponent` per-kind component carries `ResolveAtInWorldDays` (current + 32f), `OperatorMemberId`, `OperatorTitle`, `SourceFaithId`, `ExposureGain`, `IntensityErosion`, `LoyaltyPressure`, `SuccessScore`, `ProjectedChance`, `IntensityCost` (12f), and `EscrowCost` (new `DynastyOperationEscrowCost` struct mirroring resource-stockpile fields with only Influence set to 14f). New `AIMissionaryExecutionSystem` consumes `AICovertOpsComponent.LastFiredOp == CovertOpKind.Missionary`, gates on getMissionaryTerms parity, capacity-gates on `DynastyOperationLimits.HasCapacity`, deducts cost on success, computes per-kind terms from the simplified browser parity formula, calls `DynastyOperationLimits.BeginOperation` with `DynastyOperationKind.Missionary` + player target, attaches `DynastyOperationMissionaryComponent`, and pushes a `NarrativeMessageBridge.Push` line ("<source> dispatches missionaries of <faith> toward <target>.") with tone Warn when target is player and Info otherwise (browser tone routing simulation.js:10560-10561). Always clears LastFiredOp to None regardless of outcome (one-shot pattern shared with sub-slices 8/9/12/13/14).
- Browser duration translation: `MISSIONARY_DURATION_SECONDS = 32` real seconds is reinterpreted as `MissionaryDurationInWorldDays = 32f` directly on the in-world timeline rather than translated through `DualClock.DaysPerRealSecond`. Future resolution slice can re-translate at runtime if the canonical clock rate shifts; data shape stays the same.
- Per-kind resolution intentionally deferred. Created operation entities sit Active=true with the per-kind component attached until a future slice walks expired entries at `ResolveAtInWorldDays`, applies `ExposureGain`/`IntensityErosion`/`LoyaltyPressure` to the target faction, and flips Active=false.
- Defense-score simplification vs browser: Unity omits target-operator renown contribution and ward-profile bonus from the defense score because Unity has no symmetric target-operator gate yet and no faith-ward readout. Slight power adjustment vs browser; revisit when the resolution slice and ward-profile readout land.
- Faith operator role gate substitutes Spymaster for the canonical Sorcerer role (browser `getFaithOperatorMember` accepts `["ideological_leader", "sorcerer", "head_of_bloodline", "diplomat"]`); Unity has no Sorcerer DynastyRole yet.
- New dedicated bundled validator `BloodlinesCaptiveStateAndMissionaryExecutionSmokeValidation` plus wrapper `scripts/Invoke-BloodlinesUnityCaptiveStateAndMissionaryExecutionSmokeValidation.ps1`. All 8 phases PASS:
  - PhaseCaptureMember: 1 Held entry with correct fields and DualClock timestamp
  - PhaseMultipleCaptivesPerFaction: 3 captives from 3 origin factions, each found by member id
  - PhaseReleaseCaptive: status flipped to Released, entry retained on buffer for audit
  - PhaseMissionaryDispatchSuccess: op created with player target, per-kind attached with ResolveAt=72, OperatorMemberId resolved, IntensityCost=12, EscrowCost.Influence=14, source Influence 50->36, source Intensity 30->18, narrative +1, dispatch cleared
  - PhaseMissionaryCapBlocks: cap-worth pre-seeded blocks new creation, resources/intensity untouched, dispatch still cleared
  - PhaseMissionaryNoFaithBlocks: CovenantId.None blocks dispatch
  - PhaseMissionaryInsufficientIntensityBlocks: intensity 8 < 12 blocks
  - PhaseMissionaryInsufficientResourcesBlocks: influence 10 < 14 blocks
- Local csproj note: branch began with `unity/Assembly-CSharp.csproj` and `unity/Assembly-CSharp-Editor.csproj` carried from the prior `claude/unity-ai-dynasty-operations-foundation` worktree state, so the csproj also required adding Codex sub-slice 7 entries (`BloodlinesDebugCommandSurface.Fortification.BreachReadout.cs` and `BloodlinesBreachLegibilityReadoutSmokeValidation.cs`) for the dotnet-build gate to compile them. Unity's csproj auto-regeneration restores entries when the editor reloads. csproj files remain gitignored.
- Contract revision advanced 36 -> 37. New per-slice handoff: `docs/unity/session-handoffs/2026-04-19-unity-ai-strategic-layer-bundle-1-captive-state-and-missionary-execution.md`.

### Gate Results
- `dotnet build unity/Assembly-CSharp.csproj -nologo`: PASS
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`: PASS with existing editor warnings only
- Bootstrap runtime smoke: PASS via `artifacts/unity-bootstrap-runtime-smoke.log`
- Combat smoke: PASS via `artifacts/unity-combat-smoke.log` (all 8 phases)
- Scene shells: Bootstrap + Gameplay PASS via `artifacts/unity-bootstrap-scene-validate.log` and `artifacts/unity-gameplay-scene-validate.log`
- Fortification smoke: PASS via `artifacts/unity-fortification-smoke.log`
- Siege smoke: PASS via `artifacts/unity-siege-smoke.log`
- `node tests/data-validation.mjs`: PASS
- `node tests/runtime-bridge.mjs`: PASS
- Contract staleness check: PASS at revision 37
- Dedicated bundled smoke: PASS via `artifacts/unity-captive-state-and-missionary-execution-smoke.log` with marker `BLOODLINES_CAPTIVE_STATE_AND_MISSIONARY_EXECUTION_SMOKE PASS`

### Recommended Next AI Strategic Layer Slices (Bundle 2 candidates)
1. Holy war declaration execution (sub-slice 21): port `startHolyWarDeclaration` at simulation.js:10565 with a `DynastyOperationHolyWarComponent` attached to the entity created by `DynastyOperationLimits.BeginOperation`. Consumes `AICovertOpsComponent.LastFiredOp == CovertOpKind.HolyWar`. Same one-shot pattern.
2. Divine right declaration execution (sub-slice 22): port the divine right declaration flow with a `DynastyOperationDivineRightComponent`. Consumes `AICovertOpsComponent.LastFiredOp == CovertOpKind.DivineRight`. Bundle with sub-slice 21 if scope allows; both are faith-driven dispatch consumers of the sub-slice 18 foundation.
3. Captive rescue and ransom execution (sub-slices 23/24): consume `CovertOpKind.CaptiveRescue` / `CovertOpKind.CaptiveRansom` and produce per-kind components plus mutations on the sub-slice 19 `CapturedMemberElement` buffer (rescue removes Held captives via state flip; ransom flips to RansomOffered then Released with gold cost).
4. Per-kind resolution system: walk expired `DynastyOperationMissionaryComponent` entries at `ResolveAtInWorldDays`, apply `ExposureGain` to target's `FaithExposureElement` buffer, apply `IntensityErosion` to target's `FaithStateComponent.Intensity`, apply `LoyaltyPressure` to lowest-loyalty `ControlPointComponent` owned by target, flip Active=false. Generalizes once additional per-kind components land.

## Unity AI Strategic Layer Bundle 2: Holy War (Sub-Slice 21) and Divine Right (Sub-Slice 22) Execution Landed (2026-04-19)

Branch: `claude/unity-ai-holy-war-and-divine-right-execution`. Master base: `0b2fd111` (after Bundle 1 captive state + missionary execution landed at revision 37). Bundle 2 of the AI mechanical campaign: two faith-driven dispatch consumers ship as one commit, one merge, and one contract revision bump.

### What Was Done
- **Sub-slice 21 holy war declaration execution**: second production consumer of the sub-slice 18 dynasty-operation foundation. New `DynastyOperationHolyWarComponent` per-kind component carries `ResolveAtInWorldDays` (current + 18f), `WarExpiresAtInWorldDays` (current + 180f), `OperatorMemberId`, `OperatorTitle`, `IntensityPulse` (browser doctrine bias 1.2 dark / 0.9 light), `LoyaltyPulse` (1.8 dark / 1.2 light), `CompatibilityLabel` (simplified Unity-side derivation), `IntensityCost` (18f), and `EscrowCost` (Influence = 24f). New `AIHolyWarExecutionSystem` consumes `AICovertOpsComponent.LastFiredOp == CovertOpKind.HolyWar`, gates on getHolyWarDeclarationTerms parity, capacity-gates on `DynastyOperationLimits.HasCapacity`, deducts cost on success, calls `BeginOperation` with `DynastyOperationKind.HolyWar` + player target, attaches the per-kind component, and pushes a narrative line with `Warn` tone when source or target is player.
- **Sub-slice 22 divine right declaration execution**: third production consumer. New `DynastyOperationDivineRightComponent` per-kind component carries `ResolveAtInWorldDays` (current + 180f), `SourceFaithId` (CovenantId mapped to canonical string), `DoctrinePath`, `RecognitionShare`/`RecognitionSharePct` placeholders (default 0; recognition system not yet ported), `ActiveApexStructureId`/`ActiveApexStructureName` placeholders (default empty; apex structure system not yet ported). No EscrowCost or IntensityCost (browser divine right does NOT deduct intensity or charge an escrow cost; the cost is the covenant-test prerequisite). New `AIDivineRightExecutionSystem` consumes `AICovertOpsComponent.LastFiredOp == CovertOpKind.DivineRight`, gates on the simplified Unity-side parity set (committed faith, intensity >= 80, level >= 5, no existing active DivineRight op for this faction, capacity), calls `BeginOperation` with `DynastyOperationKind.DivineRight` and default target (no specific target faction), attaches the per-kind component, pushes a narrative line with `Warn` tone always (browser always-warn).
- Both systems clear LastFiredOp to None unconditionally after processing (one-shot pattern shared with sub-slices 8/9/12/13/14/20).
- Browser duration translation continues the sub-slice 20 convention: `HOLY_WAR_DECLARATION_DURATION_SECONDS = 18`, `HOLY_WAR_DURATION_SECONDS = 180`, and `DIVINE_RIGHT_DECLARATION_DURATION_SECONDS = 180` are reinterpreted as in-world-day numeric values directly.
- Notable Unity-side departure (sub-slice 22): divine right routes through `DynastyOperationLimits.BeginOperation` with `DynastyOperationKind.DivineRight` even though browser does NOT gate divine right on `DYNASTY_OPERATION_ACTIVE_LIMIT`. Routing is for surface consistency with all other dispatch consumers; browser per-faction one-active-at-a-time semantic is preserved by the explicit existing-declaration gate before the capacity check.
- Sub-slice 21 compatibility-tier simplification: Unity uses identical-(faith, doctrine) equality instead of browser's `getMarriageFaithCompatibilityProfile` tier ladder. Unity gate is strictly looser than browser; future tightening lands when the covenant compatibility surface ports.
- Sub-slice 22 deferred gates: covenant-test-passed, cooldown, stage-ready (Final Convergence threshold), active-apex-structure, recognition-share, faction-kind == kingdom. All wait on their respective surface ports.
- Sub-slice 22 deferred effects: mutual hostility application, AI timer cap propagation, conviction event recording, resolution at ResolveAtInWorldDays. All wait on a future per-kind resolution slice.
- Per-kind resolution intentionally deferred for both sub-slices (matching the Bundle 1 pattern). Created operation entities sit Active=true with the per-kind component attached until a future slice walks expired entries, applies effects, and flips Active=false.
- New dedicated bundled validator `BloodlinesHolyWarAndDivineRightExecutionSmokeValidation` plus wrapper `scripts/Invoke-BloodlinesUnityHolyWarAndDivineRightExecutionSmokeValidation.ps1`. All 10 phases PASS:
  - PhaseHolyWarDispatchSuccess: op created, per-kind attached with ResolveAt=68, WarExpiresAt=230, IntensityCost=18, EscrowCost.Influence=24, OperatorMemberId resolved, light pulses 0.9/1.2, source Influence 50->26, source Intensity 30->12, narrative +1 Warn, dispatch cleared
  - PhaseHolyWarHarmoniousFaithBlocks: identical (faith, doctrine) blocks
  - PhaseHolyWarTargetNoFaithBlocks: target CovenantId.None blocks
  - PhaseHolyWarInsufficientIntensityBlocks: intensity 12 < 18 blocks
  - PhaseHolyWarInsufficientResourcesBlocks: influence 20 < 24 blocks
  - PhaseHolyWarDarkPathPulses: dark doctrine produces 1.2/1.8 pulses
  - PhaseDivineRightDispatchSuccess: op created with default target, per-kind attached with ResolveAt=280, SourceFaithId=the_order, DoctrinePath=Light, recognition/structure placeholders default, narrative +1 Warn, dispatch cleared
  - PhaseDivineRightInsufficientIntensityBlocks: intensity 50 < 80 blocks
  - PhaseDivineRightInsufficientLevelBlocks: level 4 < 5 blocks
  - PhaseDivineRightExistingDeclarationBlocks: preexisting active DivineRight op blocks new creation
- Post-implementation diagnostic note: initial bundled smoke run failed with `FixedString32Bytes: Truncation while copying ". The spread window opens for "`. The divine-right narrative line had a 31-character string segment that overflowed FixedString32Bytes (29-byte cap after header). Fixed by casting the segment to FixedString64Bytes. Smoke re-ran clean.
- Contract revision advanced 37 -> 38. New per-slice handoff: `docs/unity/session-handoffs/2026-04-19-unity-ai-strategic-layer-bundle-2-holy-war-and-divine-right-execution.md`.

### Gate Results
- `dotnet build unity/Assembly-CSharp.csproj -nologo`: PASS
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`: PASS with existing editor warnings only
- Bootstrap runtime smoke: PASS via `artifacts/unity-bootstrap-runtime-smoke.log`
- Combat smoke: PASS via `artifacts/unity-combat-smoke.log`
- Scene shells: Bootstrap + Gameplay PASS
- Fortification smoke: PASS via `artifacts/unity-fortification-smoke.log`
- Siege smoke: PASS via `artifacts/unity-siege-smoke.log`
- `node tests/data-validation.mjs`: PASS
- `node tests/runtime-bridge.mjs`: PASS
- Contract staleness check: PASS at revision 38
- Dedicated bundled smoke: PASS via `artifacts/unity-holy-war-and-divine-right-execution-smoke.log` with marker `BLOODLINES_HOLY_WAR_AND_DIVINE_RIGHT_EXECUTION_SMOKE PASS`

### Recommended Next AI Strategic Layer Slices (Bundle 3 candidates)
1. Captive rescue execution (sub-slice 23) + captive ransom execution (sub-slice 24): consume `CovertOpKind.CaptiveRescue` / `CovertOpKind.CaptiveRansom`, produce per-kind components (`DynastyOperationCaptiveRescueComponent`, `DynastyOperationCaptiveRansomComponent`), and mutate the sub-slice 19 `CapturedMemberElement` buffer (rescue removes Held captives via Released status flip; ransom flips to RansomOffered then Released with gold cost on the home faction). Bundle as Bundle 3 following the same ceremony.
2. Per-kind resolution system: walk expired `DynastyOperationMissionaryComponent`, `DynastyOperationHolyWarComponent`, and `DynastyOperationDivineRightComponent` entries at their ResolveAtInWorldDays / WarExpiresAtInWorldDays boundaries, apply per-kind effects, flip Active=false. Could ship as a single resolution slice or split per-kind.
3. Divine right side-effect resolution (mutual hostility application against non-same-faith kingdoms, AI timer cap propagation, conviction event recording). Currently deferred from sub-slice 22.
## Codex Fortification Siege Sub-Slice 8: Breach Sealing Recovery (2026-04-19)

### Status: COMPLETE on branch codex/unity-fortification-breach-sealing-recovery

### What Was Done
- New `BreachSealingProgressComponent` plus `BreachSealingSystem` give defenders an explicit recovery loop for `FortificationComponent.OpenBreachCount`. At `1 Hz`, a breached settlement can reserve `60` stone for the active breach, accumulate `8` in-world worker-hours, and then reduce `OpenBreachCount` by one when the breach seals.
- Worker availability is resolved by scanning the owning faction's live idle workers (`UnitRole.Worker`, positive health, `WorkerGatherPhase.Idle`) instead of adding a new cached field to `FortificationReserveComponent`. This keeps the foundation slice narrow and player/AI-neutral.
- `FortificationDestructionResolutionSystem` was narrowed so sealing actually persists. Destroyed wall, tower, gate, and keep counts still refresh every tick, but `OpenBreachCount` now only increases when new walls or gates are destroyed. Previously it was recomputed from destroyed counters every frame, which would have overwritten any sealing progress immediately.
- No destroyed-structure repair ships here. Downstream consumers continue to derive assault-pressure and readout changes from `OpenBreachCount` only.
- New dedicated validator `BloodlinesBreachSealingRecoverySmokeValidation` plus wrapper `scripts/Invoke-BloodlinesUnityBreachSealingRecoverySmokeValidation.ps1`. All 6 phases PASS:
  - half-window progress accumulates to `4.00` worker-hours while breaches stay open
  - first breach seals cleanly with stone `200 -> 140`
  - insufficient stone blocks progress
  - zero idle workers block progress
  - intact settlement remains a no-op with no progress component attached
  - three full windows clear three open breaches and consume `180` stone
- Bundle 2 landed on `master` while this slice was in flight, so the fortification branch was rebased onto `origin/master` `cec33509` before close.
- Contract revision advanced `38 -> 39`. New per-slice handoff: `docs/unity/session-handoffs/2026-04-19-unity-fortification-siege-breach-sealing-recovery.md`.

### Gate Results
- `dotnet build unity/Assembly-CSharp.csproj -nologo`: PASS
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`: PASS
- Bootstrap runtime smoke: PASS via `artifacts/unity-bootstrap-runtime-smoke.log`
- Combat smoke: PASS via `artifacts/unity-combat-smoke.log`
- Scene shells: Bootstrap + Gameplay PASS via `artifacts/unity-bootstrap-scene-validate.log` and `artifacts/unity-gameplay-scene-validate.log`
- Fortification smoke: PASS via `artifacts/unity-fortification-smoke.log`
- Siege smoke: PASS via `artifacts/unity-siege-smoke.log`
- `node tests/data-validation.mjs`: PASS
- `node tests/runtime-bridge.mjs`: PASS
- Contract staleness check: PASS at revision 39
- Dedicated breach sealing recovery smoke: PASS via `artifacts/unity-breach-sealing-recovery-smoke.log` with marker `BLOODLINES_BREACH_SEALING_RECOVERY_SMOKE PASS`

### Recommended Next Fortification Follow-Up
1. Destroyed-counter recovery if the owner wants actual wall or gate repair to follow sealed breaches.
2. Sealing-cost or worker-throughput balance if the owner wants the new loop tuned before more consumers land.
3. A production HUD consumer of the breach readout after the owner decides which post-sealing signals should be rendered first.

## Unity AI Strategic Layer Bundle 3: Captive Rescue (Sub-Slice 23) and Captive Ransom (Sub-Slice 24) Execution Landed (2026-04-20)

Branch: `claude/unity-ai-captive-rescue-and-ransom-execution`. Master base: `7821d74a` (rebased from original `cec33509` base after Codex fortification-siege sub-slice 8 breach sealing / recovery landed at revision 39 concurrently with Bundle 3's original revision-39 claim; Bundle 3 bumped to revision 40). Bundle 3 of the AI mechanical campaign: two captive-lifecycle dispatch consumers ship as one commit, one merge, and one contract revision bump.

### What Was Done
- **Sub-slice 23 captive rescue execution**: fourth production consumer of the sub-slice 18 dynasty-operation foundation and first production reader of the sub-slice 19 `CapturedMemberElement` buffer. New `DynastyOperationCaptiveRescueComponent` per-kind component carries rescue-specific duration (20f), operator metadata (Spymaster + Diplomat member ids resolved by role priority), holding-settlement/keep-tier/ward placeholders (surfaces not yet ported), `SuccessScore` from simplified parity formula, `ProjectedChance` clamped 0.12-0.88, and `EscrowCost` (Gold=42, Influence=26). New `AICaptiveRescueExecutionSystem` consumes `AICovertOpsComponent.LastFiredOp == CovertOpKind.CaptiveRescue`, picks a Held captive belonging to source via the shared `TryPickCaptive` helper, gates on operative availability + cost + capacity + no-existing-op-for-member, deducts cost, attaches the per-kind component, and pushes a narrative line with Good tone (Info when source is player).
- **Sub-slice 24 captive ransom execution**: fifth production consumer and second production reader of the captive buffer. New `DynastyOperationCaptiveRansomComponent` carries ransom-specific duration (16f), operator metadata (Diplomat + Merchant), `ProjectedChance` hardcoded 1.0 (ransom is a paid transaction, not a roll), and `EscrowCost` (Gold=70, Influence=18). New `AICaptiveRansomExecutionSystem` reuses the sub-slice 23 captive picker and gate helpers via three `internal static` methods on AICaptiveRescueExecutionSystem (`TryPickCaptive`, `HasActiveOperationForMember`, `TryGetMemberByRolePriority`), keeping the two systems in sync without duplication.
- Both systems clear LastFiredOp to None unconditionally after processing (one-shot pattern shared with sub-slices 8/9/12/13/14/20/21/22).
- Browser duration translation continues the sub-slice 20 convention: RESCUE_BASE_DURATION_SECONDS=20, RANSOM_BASE_DURATION_SECONDS=16 reinterpreted as in-world-day numeric values directly.
- Cost simplification: Unity uses canonical base constants without browser's renown/role/keepTier/ward scaling because CapturedMemberElement does not yet carry renown or roleId.
- Per-kind resolution intentionally deferred for both sub-slices. Created entities sit Active=true until a future slice walks expired entries at ResolveAtInWorldDays, rolls against ProjectedChance for rescue (or unconditionally succeeds for ransom), applies effects (captive Status flip to Released via CapturedMemberHelpers.ReleaseCaptive, conviction event recording, source-faction member roster restoration), and flips Active=false.
- Parallel-revision resolution: Bundle 3 originally branched from revision-38 master (cec33509) and claimed revision 39 concurrently with Codex's fortification sub-slice 8. Codex landed first at revision 39 on master 7821d74a. Bundle 3 rebased onto 7821d74a and bumped from 39 to 40. All Bundle 3 validation gates re-run cleanly on the new base; Codex's sub-slice 8 BreachSealingSystem smoke also re-verified green alongside Bundle 3 (6 sealing phases all green).
- New dedicated bundled validator `BloodlinesCaptiveRescueAndRansomExecutionSmokeValidation` plus wrapper `scripts/Invoke-BloodlinesUnityCaptiveRescueAndRansomExecutionSmokeValidation.ps1`. All 9 phases PASS.
- Contract revision advanced 39 -> 40. New per-slice handoff: `docs/unity/session-handoffs/2026-04-20-unity-ai-strategic-layer-bundle-3-captive-rescue-and-ransom-execution.md`.

### Gate Results
- `dotnet build unity/Assembly-CSharp.csproj -nologo`: PASS
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`: PASS
- Bootstrap runtime smoke: PASS
- Combat smoke: PASS (all 8 phases)
- Scene shells: Bootstrap + Gameplay PASS
- Fortification smoke: PASS
- Siege smoke: PASS
- `node tests/data-validation.mjs`: PASS
- `node tests/runtime-bridge.mjs`: PASS
- Contract staleness check: PASS at revision 40
- Dedicated bundled smoke: PASS via `artifacts/unity-captive-rescue-and-ransom-execution-smoke.log` with marker `BLOODLINES_CAPTIVE_RESCUE_AND_RANSOM_EXECUTION_SMOKE PASS`
- Codex sub-slice 8 breach-sealing smoke: re-verified PASS via `artifacts/unity-breach-sealing-recovery-smoke.log` confirming no regression from Bundle 3

### Recommended Next AI Strategic Layer Slices (Bundle 4 candidates)
1. Per-kind resolution system (the first multi-kind resolution consumer): walk expired per-kind components at their resolve boundaries, apply effects, flip Active=false. Missionary is simplest (exposure + intensity + loyalty); holy war requires faction.faith.activeHolyWars materialization + war-tick pulses; divine right requires apex faith claim evaluation; captive rescue/ransom require captive status flips plus member roster restoration. Ship as one walker or split per-kind.
2. CapturedMemberElement extension (roleId + renown fields): adds the canonical browser fields to enable renown-scaled cost, role-priority captive picker, and simplified parity formula tightening.
3. Divine right side-effect resolution (sub-slice 22 deferrals): mutual hostility application against non-same-faith kingdoms, AI timer cap propagation to candidate factions, conviction event recording.

## Codex Fortification Siege Sub-Slice 9: Destroyed Counter Recovery (2026-04-20)

This foundation slice is complete on `codex/unity-fortification-destroyed-counter-recovery` and updates the fortification lane to contract revision `41`.

### What Landed
- `DestroyedCounterRecoveryProgressComponent` plus `DestroyedCounterRecoverySystem` now restore destroyed wall, tower, gate, and keep counters over in-world time after sub-slice 8 has already driven `OpenBreachCount` to zero.
- Recovery is intentionally ordered `Keep > Gate > Wall > Tower`, uses `90` stone plus `14` worker-hours per standard segment, and applies a `2x` multiplier for keep rebuilds.
- The runtime system heals the linked destroyed structure back to full health on completion so `FortificationDestructionResolutionSystem` stays authoritative for the fortification counters.
- Dedicated validator `BloodlinesDestroyedCounterRecoverySmokeValidation` plus wrapper `scripts/Invoke-BloodlinesUnityDestroyedCounterRecoverySmokeValidation.ps1` are green.
- The full governed 10-gate chain is green in the clean merge worktree, plus the dedicated destroyed-counter smoke.

### Validation Notes
- The checked-in bootstrap runtime and canonical scene-shell wrappers still pin `D:\ProjectsHome\Bloodlines\unity`, so this session used temporary worktree-safe wrapper copies for those gates while preserving the same pass markers.
- Local gitignored Unity-generated csproj files were refreshed before the `dotnet build` gates so they included Claude Bundle 3's already-landed files plus the new destroyed-counter recovery files.

### Next Fortification Scope
1. Sealing-cost balance pass.
2. Breach-depth telemetry.

## Unity AI Strategic Layer Bundle 4: Missionary Resolution (Sub-Slice 25) Landed (2026-04-20)

Branch: `claude/unity-ai-missionary-resolution-rebased`. Master base: `082699ab` (after Codex fortification-siege sub-slice 9 destroyed-counter recovery landed at revision 41; Bundle 4 cherry-picked onto rewritten master following upstream history rewrite that orphaned the original `claude/unity-ai-missionary-resolution` branch). Bundle 4 ships sub-slice 25 alone.

### What Was Done
- **Sub-slice 25 missionary resolution**: first production per-kind resolution consumer. New `AIMissionaryResolutionSystem` walks expired Missionary DynastyOperationComponent entries and applies canonical browser effects (ExposureGain to target's FaithExposureElement; IntensityErosion to target's Intensity IFF different faith; LoyaltyPressure to lowest-loyalty ControlPointComponent; +2 reinforcement on failure when target has committed faith). Always flips Active=false. Narrative push with success/failure/void variants.
- First consumer of DynastyOperation* per-kind components reading rather than producing. Establishes the resolution walker pattern for future per-kind resolution systems.
- New dedicated validator `BloodlinesMissionaryResolutionSmokeValidation` (8 phases) plus wrapper. All pass.
- Contract revision advanced 41 -> 42.

### Gate Results
- All 10 governed validation gates green.
- Dedicated smoke: PASS via `artifacts/unity-missionary-resolution-smoke.log`.

### Recommended Next AI Strategic Layer Slices (Bundle 5 candidates)
1. Holy war + divine right resolution (sub-slices 26 + 27) as Bundle 5.
2. Captive rescue + ransom resolution (sub-slices 28 + 29) as Bundle 6.
3. CapturedMemberElement extension (roleId + renown fields) to enable renown-scaled cost and role-priority captive picker.

## Codex Fortification Siege Sub-Slice 10: Breach Depth Telemetry (2026-04-20)

This observability follow-up is complete on `codex/unity-fortification-breach-depth-telemetry`.

### What Landed
- `SettlementBreachTelemetry` and `TryDebugGetSettlementBreachTelemetry` were added to the fortification debug surface so one settlement-level read gives:
  - the existing breach readout
  - sealing eligibility, tracked state, worker-hours, stone cost, and normalized progress
  - destroyed-counter recovery eligibility, tracked state, target counter, worker-hours, stone cost, and normalized progress
- `FortificationCanon` now owns the sealing and rebuild tick/cost constants; `BreachSealingSystem` and `DestroyedCounterRecoverySystem` were updated to read those shared values.
- `BloodlinesBreachLegibilityReadoutSmokeValidation` now covers 7 phases, including half-window sealing telemetry and half-window keep-recovery telemetry.
- All 10 governed validation gates are green in `D:\BLBDT\bloodlines`, plus the breach legibility readout smoke.

### Immediate Next Actions
1. Push `codex/unity-fortification-breach-depth-telemetry`.
2. Merge it to `master` via the normal merge-temp ceremony.
3. After merge, either pause or retire the fortification lane unless Lance explicitly wants the optional sealing-cost balance pass.

### Important Repo-Reality Note
- The current multi-day directive's Priority 2 description is stale against the repository. The repo already contains Tier 2 dynasties foundation code in:
  - `unity/Assets/_Bloodlines/Code/Dynasties/MarriageComponents.cs`
  - `unity/Assets/_Bloodlines/Code/Dynasties/MarriageProposalExpirationSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Dynasties/MarriageGestationSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Dynasties/LesserHouseLoyaltyDriftSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Dynasties/MinorHouseLevySystem.cs`
- Do not open a duplicate zero-code marriages lane. If Lance still wants more dynasty work, scope the next lane as hardening or extension on top of those existing surfaces after checking the retired `tier2-batch-dynasty-systems` handoff.

## Codex Fortification Siege Sub-Slice 11: Sealing Cost Tier Scaling (2026-04-21)

### Status: COMPLETE on branch codex/unity-fortification-sealing-cost-tier-scaling

### What Landed
- `FortificationCanon` now carries the tier-aware breach-sealing cost curve: Tier 1 `60` stone and `8` worker-hours, Tier 2 `90` stone and `12` worker-hours, Tier 3 `135` stone and `18` worker-hours.
- `BreachSealingSystem` now resolves required stone and labor from `FortificationComponent.Tier`, so stronger settlements cost more to close after a breach opens.
- `SettlementBreachTelemetry` and the existing breach-legibility smoke now read those tier-aware values, and the new `BloodlinesBreachSealingTierScalingSmokeValidation` proves all three tiers plus a mixed-tier portfolio in one dedicated validator.
- The full governed chain is green in `D:\BLF11\bloodlines`, with worktree-local wrapper copies used for the still-root-pinned bootstrap runtime and scene-shell gates. Contract revision advanced `46 -> 47`.

### Immediate Next Fortification Action
1. Merge `codex/unity-fortification-sealing-cost-tier-scaling`.
2. Fetch `origin/master` again after the merge lands.
3. Continue on sub-slice 12 with `codex/unity-fortification-sealing-worker-locality`.

### Validation Notes
- `dotnet build` runtime and editor gates: PASS
- Bootstrap runtime smoke: PASS via worktree-local wrapper copy
- Combat smoke: PASS
- Scene shells: Bootstrap + Gameplay PASS via worktree-local wrapper copies
- Fortification smoke: PASS
- Siege smoke: PASS
- `node tests/data-validation.mjs`: PASS
- `node tests/runtime-bridge.mjs`: PASS
- Contract staleness check: PASS at revision `46` before the revision-47 contract update
- Dedicated tier-scaling smoke: PASS with marker `BLOODLINES_BREACH_SEALING_TIER_SCALING_SMOKE PASS`

## Codex Fortification Siege Sub-Slice 12: Sealing Worker Locality (2026-04-21)

### Status: COMPLETE on branch codex/unity-fortification-sealing-worker-locality

### What Landed
- `BreachSealingSystem` now ties sealing labor to the settlement's own control-point footprint instead of a faction-wide idle-worker pool. The system resolves the settlement's nearest same-owner control point, resolves each idle worker's nearest control point, and only spends labor when those anchors match.
- The worker scan now requires `PositionComponent` and owner match on the worker's nearest `ControlPointComponent`, so same-faction workers parked near another settlement no longer poach this settlement's breach closure.
- New dedicated validator `BloodlinesBreachSealingWorkerLocalitySmokeValidation` plus wrapper prove local sealing, same-faction other-settlement blocking, no-workers blocking, and non-idle blocking.
- Full governed validation is green on `D:\BLF12\bloodlines`, again using worktree-local wrapper copies for the path-pinned bootstrap runtime and scene-shell gates. Contract revision advanced `47 -> 48`.

### Immediate Next Fortification Action
1. Merge `codex/unity-fortification-sealing-worker-locality`.
2. Fetch `origin/master` again after the merge lands.
3. Continue on sub-slice 13 with `codex/unity-fortification-repair-narrative`.

### Validation Notes
- `dotnet build` runtime and editor gates: PASS
- Bootstrap runtime smoke: PASS via worktree-local wrapper copy
- Combat smoke: PASS
- Scene shells: Bootstrap + Gameplay PASS via worktree-local wrapper copies
- Fortification smoke: PASS
- Siege smoke: PASS
- `node tests/data-validation.mjs`: PASS
- `node tests/runtime-bridge.mjs`: PASS
- Contract staleness check: PASS at revision `48` after the contract and continuity updates
- Dedicated worker-locality smoke: PASS with marker `BLOODLINES_BREACH_SEALING_WORKER_LOCALITY_SMOKE PASS`
- The first post-compile locality-smoke rerun exited immediately with return code `1` before the validator executed. One 10-second retry passed cleanly, so no block remains.

## Codex Fortification Siege Sub-Slice 13: Repair Narrative (2026-04-21)

### Status: COMPLETE on branch codex/unity-fortification-repair-narrative

### What Landed
- `BreachSealingSystem` now pushes one info-tone message through `NarrativeMessageBridge` whenever a breach closes.
- `DestroyedCounterRecoverySystem` now pushes one info-tone repair message whenever a destroyed counter rebuild completes.
- New dedicated validator `BloodlinesFortificationRepairNarrativeSmokeValidation` plus wrapper `scripts/Invoke-BloodlinesUnityFortificationRepairNarrativeSmokeValidation.ps1` prove single-breach close, three-breach close, and wall rebuild narrative emission.
- Full governed validation is green on `D:\BLF13\bloodlines`, with worktree-safe wrapper copies still used for the root-pinned bootstrap runtime and scene-shell validators. Contract revision advanced `48 -> 49`.

### Immediate Next Action
1. Merge `codex/unity-fortification-repair-narrative`.
2. Regenerate the root `HANDOFF.md` for a post-fortification lane handoff.
3. Stop unless Lance explicitly defines a new fortification sub-slice 14.

### Validation Notes
- `dotnet build` runtime and editor gates: PASS
- Bootstrap runtime smoke: PASS via worktree-local wrapper copy
- Combat smoke: PASS
- Scene shells: Bootstrap + Gameplay PASS via worktree-local wrapper copies
- Fortification smoke: PASS
- Siege smoke: PASS
- `node tests/data-validation.mjs`: PASS
- `node tests/runtime-bridge.mjs`: PASS
- Contract staleness check: PASS at revision `49` after the continuity and contract updates
- Dedicated fortification repair narrative smoke: PASS with marker `BLOODLINES_FORTIFICATION_REPAIR_NARRATIVE_SMOKE PASS`

## Codex Fortification Siege Session Wrap 10 Through 13 (2026-04-21)

### Status: FORTIFICATION QUEUE PAUSED

### Next Recommended Pickup
1. Fetch `origin/master` and confirm `docs/unity/CONCURRENT_SESSION_CONTRACT.md` is at revision `49`.
2. There is no approved fortification sub-slice 14. Do not extend the lane without fresh operator direction.
3. If Lance does not define a new fortification follow-up, claim a different approved Codex lane instead.

### Context Notes
- The checked-in bootstrap runtime and canonical scene-shell wrappers still pin `D:\ProjectsHome\Bloodlines\unity`. Clean worktree validation should keep using temporary worktree-safe wrapper copies.
- `unity/ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json` still dirties during Unity validation and should remain unstaged.

## Codex Scout Raids And Logistics Interdiction Landing (2026-04-21)

Branch landed: `codex/unity-scout-raids-logistics-interdiction`

### What Landed
- `ScoutRaidCommandComponent`, `BuildingRaidStateComponent`, `ScoutRaidCanon`,
  and `ScoutRaidResolutionSystem` are now on `master`.
- Building raids now suppress passive trickle, apply stockpile loss and local
  loyalty shock, and force raider retreat.
- Supply-wagon interdiction now applies stockpile loss, interdiction and convoy
  recovery timers, support-status downgrade, and wagon retreat toward the
  nearest operational allied camp.
- `WorkerGatherSystem`, `FieldWaterSupportScanSystem`, and
  `SiegeSupportRefreshSystem` all now respect active raid state.
- Dedicated validation is landed through
  `BloodlinesScoutRaidAndInterdictionSmokeValidation` and
  `scripts/Invoke-BloodlinesUnityScoutRaidAndInterdictionSmokeValidation.ps1`.

### Validation State
- Merge commit: `dda7c25e`
- Full governed gate rerun green on detached merged `master` in
  `D:\BLAICD\bloodlines`
- Dedicated scout smoke green with marker
  `BLOODLINES_SCOUT_RAID_AND_INTERDICTION_SMOKE PASS`

### Immediate Next Action
1. Claim the player-facing marriage diplomacy lane next.
2. Start with proposal execution under
   `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/` on a fresh
   `codex/unity-player-marriage-proposal` branch.
3. Before coding, add the new lane to
   `docs/unity/CONCURRENT_SESSION_CONTRACT.md` and re-read the browser
   references for `proposeMarriage`, `getMarriageAuthorityProfile`, and
   `getMarriageAcceptanceTerms`.

## Codex Player Marriage Diplomacy Sub-Slice 2A: Proposal Execution (2026-04-21)

### Status: COMPLETE on branch `codex/unity-player-marriage-proposal`

### What Was Done
- Claimed the new `player-marriage-diplomacy` lane in `docs/unity/CONCURRENT_SESSION_CONTRACT.md` without reopening the paused `dynasty-house-parity` lane and without writing into Claude's AI-owned `unity/Assets/_Bloodlines/Code/AI/**` path.
- Added `PlayerMarriageProposalRequestComponent`, `PlayerMarriageAuthorityEvaluator`, and `PlayerMarriageProposalSystem` under `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/`.
- Ported the browser proposal gate chain: dynasty presence, same-faction rejection, candidate availability, active-marriage + non-polygamy rejection, source authority + envoy governance requirement, duplicate pending-pair block, 90-day expiration seed, and source-side regency legitimacy cost.
- Added `BloodlinesDebugCommandSurface.PlayerDiplomacy.cs` with `TryDebugIssuePlayerMarriageProposal` and `TryDebugGetPlayerMarriageProposals`.
- Added `BloodlinesPlayerMarriageProposalSmokeValidation` plus `scripts/Invoke-BloodlinesUnityPlayerMarriageProposalSmokeValidation.ps1`.
- Ran the full governed 10-step gate green in `D:\BLM13\bloodlines\bloodlines`, using temporary worktree-local copies only for the still-root-pinned bootstrap-runtime and scene-shell wrapper scripts.

### Validation Proof Lines
- Runtime build: `Build succeeded.` / `0 Error(s)`
- Editor build: `0 Error(s)` with existing warnings
- Bootstrap runtime smoke: `Bootstrap runtime smoke validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity on map ironmark_frontier. ...`
- Combat smoke: `Combat smoke validation passed: meleePhase=True, projectilePhase=True, explicitAttackPhase=True, attackMovePhase=True, targetVisibilityPhase=True, groupMovementPhase=True, separationPhase=True, stancePhase=True.`
- Bootstrap scene shell: `Bootstrap scene shell validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity with canonical map Assets/_Bloodlines/Data/MapDefinitions/ironmark_frontier.asset.`
- Gameplay scene shell: `Gameplay scene shell validation passed for Assets/_Bloodlines/Scenes/Gameplay/IronmarkFrontier.unity.`
- Fortification smoke: `Fortification smoke validation passed: baselinePhase=True, tierAdvancePhase=True, reserveMusterPhase=True, reserveRecoveryPhase=True.`
- Siege smoke: `Siege smoke validation passed: baselinePhase=True, strainPhase=True, recoveryPhase=True, supportPhase=True. ...`
- Data validation: `Bloodlines data validation passed.`
- Runtime bridge: `Bloodlines runtime bridge validation passed.`
- Contract staleness: `STALENESS CHECK PASSED: Contract revision=52, last-updated=2026-04-21 is current. ...`
- Dedicated smoke: `BLOODLINES_PLAYER_MARRIAGE_PROPOSAL_SMOKE PASS`

### Exact Next Action
1. Start sub-slice 2B on fresh branch `codex/unity-player-marriage-acceptance`.
2. Port `acceptMarriage` and `getMarriageAcceptanceTerms` into `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/`.
3. Add a dedicated acceptance smoke proving marriage creation, legitimacy delta, no-pending-proposal rejection, and heir-regency cost deduction.

## Codex Player Marriage Diplomacy Sub-Slice 2A Landing (2026-04-21)

### Status: MERGED to `master` via `21550da3`

### What Landed
- `codex/unity-player-marriage-proposal` is now merged, so proposal execution
  is part of canonical `master` rather than a pending feature branch.
- `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/` now contains the landed
  proposal request, authority evaluation, and execution systems.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.PlayerDiplomacy.cs`
  and `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerMarriageProposalSmokeValidation.cs`
  are now canonical validation/debug surfaces on `master`.
- Merged `master` re-passed runtime build, editor build, bootstrap runtime,
  combat, scene shells, fortification, siege, `node tests/data-validation.mjs`,
  `node tests/runtime-bridge.mjs`, and the dedicated proposal smoke.

### Immediate Next Action
1. Create fresh branch `codex/unity-player-marriage-acceptance` from current `master`.
2. Port `acceptMarriage` and `getMarriageAcceptanceTerms` under `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/`.
3. Add the dedicated acceptance smoke and wrapper before attempting any later marriage-diplomacy slice.

### Context Notes
- The checked-in bootstrap-runtime and canonical scene-shell wrappers are still
  pinned to `D:\ProjectsHome\Bloodlines`; continue using temporary worktree-safe
  copies when validating a clean worktree.
- `unity/ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json`
  still dirties during Unity validation and should remain unstaged.

## Codex Player Marriage Diplomacy Sub-Slice 2B: Acceptance And Effects (2026-04-21)

### Status: COMPLETE on branch `codex/unity-player-marriage-acceptance`

### What Was Done
- Added `PlayerMarriageAcceptRequestComponent` and `PlayerMarriageAcceptSystem`
  under `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/`.
- Ported the player-facing `acceptMarriage` execution path:
  pending-proposal lookup, source/target dynasty and member resolution,
  target-side authority validation, primary + mirror marriage creation,
  target legitimacy-cost deduction, hostility drop, oathkeeping +2, legitimacy
  +2 on both sides, and the 30-day declaration jump.
- Extended
  `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.PlayerDiplomacy.cs`
  with `TryDebugIssuePlayerMarriageAccept(proposalEntityIndex)` and proposal
  `EntityIndex=...` readout support.
- Added
  `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerMarriageAcceptanceSmokeValidation.cs`
  plus
  `scripts/Invoke-BloodlinesUnityPlayerMarriageAcceptanceSmokeValidation.ps1`.
- Full governed validation is green in `D:\BLM13\bloodlines\bloodlines` with
  temporary worktree-local copies used only for the still-root-pinned
  bootstrap-runtime and canonical scene-shell wrappers.

### Validation Proof
- Runtime build: `Build succeeded.` / `0 Error(s)`
- Editor build: `0 Error(s)` with existing warnings
- Bootstrap runtime smoke:
  `Bootstrap runtime smoke validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity on map ironmark_frontier. ...`
- Combat smoke:
  `Combat smoke validation passed: meleePhase=True, projectilePhase=True, explicitAttackPhase=True, attackMovePhase=True, targetVisibilityPhase=True, groupMovementPhase=True, separationPhase=True, stancePhase=True.`
- Bootstrap scene shell:
  `Bootstrap scene shell validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity with canonical map Assets/_Bloodlines/Data/MapDefinitions/ironmark_frontier.asset.`
- Gameplay scene shell:
  `Gameplay scene shell validation passed for Assets/_Bloodlines/Scenes/Gameplay/IronmarkFrontier.unity.`
- Fortification smoke:
  `Fortification smoke validation passed: baselinePhase=True, tierAdvancePhase=True, reserveMusterPhase=True, reserveRecoveryPhase=True. ...`
- Siege smoke:
  `Siege smoke validation passed: baselinePhase=True, strainPhase=True, recoveryPhase=True, supportPhase=True. ...`
- Data validation: `Bloodlines data validation passed.`
- Runtime bridge: `Bloodlines runtime bridge validation passed.`
- Dedicated smoke:
  `BLOODLINES_PLAYER_MARRIAGE_ACCEPTANCE_SMOKE PASS`
  with
  `Phase 2 PASS: proposal accepted, marriageCount=2, legitimacy=82/72, oathkeeping=5/3, dualClockDays=50`
  and
  `Phase 4 PASS: heir-regency cost applied, legitimacy=81, stewardship=2`

### Immediate Next Action
1. Stage the acceptance slice files plus continuity updates and commit them on `codex/unity-player-marriage-acceptance`.
2. Push the branch, merge it to `master`, and rerun the full governed validation gate on merged `master`.
3. After the landing continuity pass, begin sub-slice 2C on fresh branch `codex/unity-player-marriage-dissolution`.

### Context Notes
- The checked-in bootstrap-runtime and canonical scene-shell wrappers are still
  pinned to `D:\ProjectsHome\Bloodlines`; continue using temporary worktree-safe
  copies when validating this clean worktree.
- `unity/ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json`
  still dirties during Unity validation and should remain unstaged.

## Codex Player Marriage Diplomacy Sub-Slice 2B Landing (2026-04-21)

### Status: MERGED to `master` via `00223fa9`

### What Landed
- `codex/unity-player-marriage-acceptance` is now merged, so player-side
  marriage acceptance is part of canonical `master`.
- `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/` now contains the landed
  acceptance request and execution systems alongside the previously landed
  proposal path.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.PlayerDiplomacy.cs`
  and
  `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerMarriageAcceptanceSmokeValidation.cs`
  are now canonical debug and validation surfaces on `master`.
- Merged `master` re-passed runtime build, editor build, bootstrap runtime,
  combat, scene shells, fortification, siege, `node tests/data-validation.mjs`,
  `node tests/runtime-bridge.mjs`, and the dedicated acceptance smoke.

### Immediate Next Action
1. Create fresh branch `codex/unity-player-marriage-dissolution` from current `master`.
2. Port the browser's death-driven marriage dissolution behavior under the player-marriage lane.
3. Add the dedicated dissolution smoke before attempting any later player-marriage or covert-ops slice.

### Context Notes
- The checked-in bootstrap-runtime and canonical scene-shell wrappers are still
  pinned to `D:\ProjectsHome\Bloodlines`; continue using temporary worktree-safe
  copies when validating this clean worktree.
- `unity/ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json`
  still dirties during Unity validation and should remain unstaged.

## Codex Player Marriage Diplomacy Sub-Slice 2C: Dissolution Validation (2026-04-21)

### Status: COMPLETE on branch `codex/unity-player-marriage-dissolution`

### What Was Done
- Added `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerMarriageDissolutionSmokeValidation.cs`.
- Added `scripts/Invoke-BloodlinesUnityPlayerMarriageDissolutionSmokeValidation.ps1`.
- Closed the remaining player-marriage proof gap without duplicating runtime:
  the validator intentionally reuses the already-landed
  `unity/Assets/_Bloodlines/Code/Dynasties/MarriageDeathDissolutionSystem.cs`
  from the paused `dynasty-house-parity` lane.
- The dedicated 3-phase smoke now proves:
  - accepted marriages remain active while both spouses live
  - ruler death dissolves both mirror records and promotes the heir through
    `DynastySuccessionSystem`
  - active marriages still gestate a child when no death intervenes
- Full governed validation is green in `D:\BLM13\bloodlines\bloodlines`, with
  temporary worktree-local copies used only for the still-root-pinned
  bootstrap-runtime and canonical scene-shell wrappers.

### Validation Proof
- Runtime build: `Build succeeded.` / `0 Error(s)`
- Editor build: `0 Error(s)` with existing warnings
- Bootstrap runtime smoke:
  `Bootstrap runtime smoke validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity on map ironmark_frontier. ...`
- Combat smoke:
  `Combat smoke validation passed: meleePhase=True, projectilePhase=True, explicitAttackPhase=True, attackMovePhase=True, targetVisibilityPhase=True, groupMovementPhase=True, separationPhase=True, stancePhase=True.`
- Bootstrap scene shell:
  `Bootstrap scene shell validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity with canonical map Assets/_Bloodlines/Data/MapDefinitions/ironmark_frontier.asset.`
- Gameplay scene shell:
  `Gameplay scene shell validation passed for Assets/_Bloodlines/Scenes/Gameplay/IronmarkFrontier.unity.`
- Fortification smoke:
  `Fortification smoke validation passed: baselinePhase=True, tierAdvancePhase=True, reserveMusterPhase=True, reserveRecoveryPhase=True. ...`
- Siege smoke:
  `Siege smoke validation passed: baselinePhase=True, strainPhase=True, recoveryPhase=True, supportPhase=True. ...`
- Data validation: `Bloodlines data validation passed.`
- Runtime bridge: `Bloodlines runtime bridge validation passed.`
- Dedicated smoke:
  `BLOODLINES_PLAYER_MARRIAGE_DISSOLUTION_SMOKE PASS`
  with
  `Phase 1 PASS: alive marriage remained active with MarriageId=marriage-11210497115101494597108105118101451099711411410597`
  and
  `Phase 2 PASS: ruler death dissolved MarriageId=marriage-1121049711510150451141171081011144510010197116104 at day=50.00 and promoted player-bloodline-heir`
  and
  `Phase 3 PASS: live marriage gestated childId=child-marriage-11210497115101514510310111511697116105111110 for headFaction=enemy`

### Immediate Next Action
1. Stage the dissolution validator, wrapper, and continuity updates and commit them on `codex/unity-player-marriage-dissolution`.
2. Push the branch, merge it to `master`, and rerun the full governed validation gate on merged `master`.
3. After the landing continuity pass, start Priority 3 on fresh branch `codex/unity-player-covert-ops-foundation`.

### Context Notes
- The checked-in bootstrap-runtime and canonical scene-shell wrappers are still
  pinned to `D:\ProjectsHome\Bloodlines`; continue using temporary worktree-safe
  copies when validating this clean worktree.
- `unity/ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json`
  still dirties during Unity validation and should remain unstaged.
- Node validations for this worktree must run from `D:\BLM13\bloodlines\bloodlines`
  rather than the outer worktree root `D:\BLM13\bloodlines`.

## Codex Player Marriage Diplomacy Sub-Slice 2C Landing (2026-04-21)

### Status: MERGED to `master` via `f5bfef1d`

### What Landed
- `codex/unity-player-marriage-dissolution` is now merged, so the
  player-marriage diplomacy stack is fully landed on canonical `master`.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerMarriageDissolutionSmokeValidation.cs`
  and
  `scripts/Invoke-BloodlinesUnityPlayerMarriageDissolutionSmokeValidation.ps1`
  are now part of the canonical player-diplomacy validation surface.
- The dedicated dissolution proof still reuses the already-landed
  `MarriageDeathDissolutionSystem` from the paused dynasty-house-parity lane;
  no duplicate runtime was introduced under `PlayerDiplomacy/`.
- Merged `master` re-passed runtime build, editor build, bootstrap runtime,
  combat, scene shells, fortification, siege, `node tests/data-validation.mjs`,
  `node tests/runtime-bridge.mjs`, and the dedicated dissolution smoke.

### Immediate Next Action
1. Create fresh branch `codex/unity-player-covert-ops-foundation` from current `master`.
2. Port the browser's `startEspionageOperation` gate chain and operation creation under `unity/Assets/_Bloodlines/Code/PlayerCovertOps/`.
3. Add the dedicated player covert ops smoke and wrapper before starting assassination or sabotage follow-ups.

### Context Notes
- The checked-in bootstrap-runtime and canonical scene-shell wrappers are still
  pinned to `D:\ProjectsHome\Bloodlines`; continue using temporary worktree-safe
  copies when validating this clean worktree.
- `unity/ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json`
  still dirties during Unity validation and should remain unstaged.
- Node validations for this worktree must run from `D:\BLM13\bloodlines\bloodlines`
  rather than the outer worktree root `D:\BLM13\bloodlines`.

## Codex Player Covert Ops Sub-Slice 3A: Foundation (2026-04-21)

### Status: COMPLETE on branch `codex/unity-player-covert-ops-foundation`

### What Was Done
- Added `unity/Assets/_Bloodlines/Code/PlayerCovertOps/CovertOpKindPlayer.cs`.
- Added `unity/Assets/_Bloodlines/Code/PlayerCovertOps/PlayerCovertOpsRequestComponent.cs`.
- Added `unity/Assets/_Bloodlines/Code/PlayerCovertOps/PlayerCovertOpsResolutionComponent.cs`.
- Added `unity/Assets/_Bloodlines/Code/PlayerCovertOps/PlayerCovertOpsSystem.cs`.
- Added `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.PlayerCovertOps.cs`.
- Added `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerCovertOpsSmokeValidation.cs`.
- Added `scripts/Invoke-BloodlinesUnityPlayerCovertOpsSmokeValidation.ps1`.
- Ported the browser's player-facing espionage dispatch seam:
  source/target faction validation, active-op and live-report dedupe, operator
  resolution from the dynasty roster, canonical espionage cost deduction, live
  player-op entity creation, and the combined active-op cap using Claude's
  `DynastyOperationLimits` read-only helper plus player-owned operation
  entities.
- Full governed validation is green in `D:\BLM13\bloodlines\bloodlines` with
  temporary worktree-local copies used only for the still-root-pinned
  bootstrap-runtime and canonical scene-shell wrappers.

### Validation Proof
- Runtime build: `Build succeeded.` / `0 Error(s)`
- Editor build: `0 Error(s)` with existing warnings
- Bootstrap runtime smoke:
  `Bootstrap runtime smoke validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity on map ironmark_frontier. ...`
- Combat smoke:
  `Combat smoke validation passed: meleePhase=True, projectilePhase=True, explicitAttackPhase=True, attackMovePhase=True, targetVisibilityPhase=True, groupMovementPhase=True, separationPhase=True, stancePhase=True.`
- Bootstrap scene shell:
  `Bootstrap scene shell validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity with canonical map Assets/_Bloodlines/Data/MapDefinitions/ironmark_frontier.asset.`
- Gameplay scene shell:
  `Gameplay scene shell validation passed for Assets/_Bloodlines/Scenes/Gameplay/IronmarkFrontier.unity.`
- Fortification smoke:
  `Fortification smoke validation passed: baselinePhase=True, tierAdvancePhase=True, reserveMusterPhase=True, reserveRecoveryPhase=True. ...`
- Siege smoke:
  `Siege smoke validation passed: baselinePhase=True, strainPhase=True, recoveryPhase=True, supportPhase=True. ...`
- Data validation: `Bloodlines data validation passed.`
- Runtime bridge: `Bloodlines runtime bridge validation passed.`
- Dedicated smoke:
  `BLOODLINES_PLAYER_COVERT_OPS_SMOKE PASS`
  with
  `Phase 2 PASS: espionage created, gold=155, influence=64, readout='ActivePlayerCovertOpCount=1`
  and
  `Phase 4 PASS: active espionage ops capped at 6 with readout 'ActivePlayerCovertOpCount=6`

### Immediate Next Action
1. Stage the player covert ops foundation files plus continuity/contract updates and commit them on `codex/unity-player-covert-ops-foundation`.
2. Push the branch, merge it to `master`, and rerun the full governed validation gate on merged `master`.
3. After the landing continuity pass, start sub-slice 3B on fresh branch `codex/unity-player-assassination-sabotage`.

### Context Notes
- The checked-in bootstrap-runtime and canonical scene-shell wrappers are still
  pinned to `D:\ProjectsHome\Bloodlines`; continue using temporary worktree-safe
  copies when validating this clean worktree.
- `unity/ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json`
  still dirties during Unity validation and should remain unstaged.
- Node validations for this worktree must run from `D:\BLM13\bloodlines\bloodlines`
  rather than the outer worktree root `D:\BLM13\bloodlines`.
- The player covert ops lane intentionally keeps live operation entities under
  `PlayerCovertOps/**` instead of widening Claude's AI-owned
  `DynastyOperationComponent` graph.

## Codex Player Covert Ops Sub-Slice 3A Landing (2026-04-21)

### Status: MERGED to `master` via `c18966d6`

### What Landed
- `codex/unity-player-covert-ops-foundation` is now merged, so player-facing
  espionage dispatch is part of canonical `master`.
- `unity/Assets/_Bloodlines/Code/PlayerCovertOps/` now contains the landed
  covert-op enum, request component, resolution component, and espionage
  dispatch system.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.PlayerCovertOps.cs`
  and
  `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerCovertOpsSmokeValidation.cs`
  are now canonical debug and validation surfaces on `master`.
- Merged `master` re-passed runtime build, editor build, bootstrap runtime,
  combat, scene shells, fortification, siege, `node tests/data-validation.mjs`,
  `node tests/runtime-bridge.mjs`, contract staleness, and the dedicated player
  covert ops smoke.

### Immediate Next Action
1. Create fresh branch `codex/unity-player-assassination-sabotage` from current `master`.
2. Port the browser's `startAssassinationOperation` and `startSabotageOperation` under `unity/Assets/_Bloodlines/Code/PlayerCovertOps/`.
3. Extend the dedicated player covert ops smoke and wrapper before attempting any later counter-intelligence follow-up.

### Context Notes
- The checked-in bootstrap-runtime and canonical scene-shell wrappers are still
  pinned to `D:\ProjectsHome\Bloodlines`; continue using temporary worktree-safe
  copies when validating this clean worktree.
- `unity/ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json`
  still dirties during Unity validation and should remain unstaged.
- Node validations for this worktree must run from `D:\BLM13\bloodlines\bloodlines`
  rather than the outer worktree root `D:\BLM13\bloodlines`.
- The player covert ops lane remains active for 3B and 3C, but there is no
  current feature branch in flight after this landing pass.

## Codex Player Covert Ops Sub-Slice 3B: Assassination And Sabotage (2026-04-21)

### Status: COMPLETE on branch `codex/unity-player-assassination-sabotage`

### What Was Done
- Extended `unity/Assets/_Bloodlines/Code/PlayerCovertOps/PlayerCovertOpsRequestComponent.cs`
  with subtype input so one request path can carry sabotage subtype selection.
- Extended `unity/Assets/_Bloodlines/Code/PlayerCovertOps/PlayerCovertOpsResolutionComponent.cs`
  with subtype, target label, location label, intelligence-support, and
  defense telemetry fields.
- Reworked `unity/Assets/_Bloodlines/Code/PlayerCovertOps/PlayerCovertOpsSystem.cs`
  so it now dispatches player espionage, assassination, and sabotage from one
  lane-local system while preserving the browser active-op cap against the
  shared `DynastyOperationLimits` helper.
- Added assassination dispatch gates:
  live enemy dynasty member target, duplicate-target blocking, canonical
  `gold=85` / `influence=28` cost deduction, operator selection
  `Spymaster` -> `Diplomat` -> `Merchant`, and live op creation with location
  and projected-chance telemetry.
- Added sabotage dispatch gates:
  canonical subtype legality checks from `BuildingTypeComponent`,
  live-building validation, subtype-specific canonical costs and durations, and
  live op creation with subtype plus target-building telemetry.
- Extended `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.PlayerCovertOps.cs`
  with `TryDebugIssuePlayerAssassination(...)`,
  `TryDebugIssuePlayerSabotage(...)`, and richer structured readout fields.
- Extended `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerCovertOpsSmokeValidation.cs`
  from 4 phases to 6 so it now proves assassination target validation and
  sabotage target validation in addition to the existing 3A coverage.
- Recorded the branch-side contract and continuity pass:
  - `docs/unity/CONCURRENT_SESSION_CONTRACT.md` revision `61 -> 62`
  - `docs/unity/session-handoffs/2026-04-21-unity-player-assassination-sabotage.md`
  - `CURRENT_PROJECT_STATE.md`
  - `NEXT_SESSION_HANDOFF.md`
  - `continuity/PROJECT_STATE.json`

### Validation Proof
- Runtime build: `Build succeeded.` / `0 Error(s)`
- Editor build: `113 Warning(s)` / `0 Error(s)`
- Bootstrap runtime smoke:
  `Bootstrap runtime smoke validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity on map ironmark_frontier. ...`
- Combat smoke:
  `Combat smoke validation passed: meleePhase=True, projectilePhase=True, explicitAttackPhase=True, attackMovePhase=True, targetVisibilityPhase=True, groupMovementPhase=True, separationPhase=True, stancePhase=True.`
- Bootstrap scene shell:
  `Bootstrap scene shell validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity with canonical map Assets/_Bloodlines/Data/MapDefinitions/ironmark_frontier.asset.`
- Gameplay scene shell:
  `Gameplay scene shell validation passed for Assets/_Bloodlines/Scenes/Gameplay/IronmarkFrontier.unity.`
- Fortification smoke:
  `Fortification smoke validation passed: baselinePhase=True, tierAdvancePhase=True, reserveMusterPhase=True, reserveRecoveryPhase=True. ...`
- Siege smoke:
  `Siege smoke validation passed: baselinePhase=True, strainPhase=True, recoveryPhase=True, supportPhase=True. ...`
- Data validation: `Bloodlines data validation passed.`
- Runtime bridge: `Bloodlines runtime bridge validation passed.`
- Dedicated smoke:
  `BLOODLINES_PLAYER_COVERT_OPS_SMOKE PASS`
  with
  `Phase 5 PASS: assassination targeted memberId=enemy-bloodline-marshal, title=War Captain, gold=215, influence=152.`
  and
  `Phase 6 PASS: sabotage targeted entityIndex=22, subtype=gate_opening, gold=200, influence=102.`

### Immediate Next Action
1. Stage the player covert ops 3B files plus continuity/contract updates and commit them on `codex/unity-player-assassination-sabotage`.
2. Push the branch, merge it to `master`, and rerun the full governed validation gate on merged `master`.
3. After the landing continuity pass, start sub-slice 3C on a fresh player counter-intelligence branch.

### Context Notes
- The checked-in bootstrap-runtime and canonical scene-shell wrappers are still
  pinned to `D:\ProjectsHome\Bloodlines`; continue using temporary worktree-safe
  copies when validating this clean worktree.
- `unity/ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json`
  still dirties during Unity validation and should remain unstaged.
- Node validations for this worktree must run from `D:\BLM13\bloodlines\bloodlines`
  rather than the outer worktree root `D:\BLM13\bloodlines`.
- Counter-intelligence defense, dossiers, retaliation metadata, and
  intelligence-report buffers remain deferred to sub-slice 3C.

## Codex Player Covert Ops Sub-Slice 3B Landing (2026-04-21)

### Status: MERGED to `master` via `2892c583`

### What Landed
- `codex/unity-player-assassination-sabotage` is now merged, so player-facing
  assassination and sabotage dispatch are part of canonical `master`.
- `unity/Assets/_Bloodlines/Code/PlayerCovertOps/PlayerCovertOpsSystem.cs`
  now lands the player-side assassination and sabotage dispatch seams
  alongside the previously landed espionage path.
- `unity/Assets/_Bloodlines/Code/PlayerCovertOps/PlayerCovertOpsRequestComponent.cs`
  and
  `unity/Assets/_Bloodlines/Code/PlayerCovertOps/PlayerCovertOpsResolutionComponent.cs`
  now carry subtype and structured covert-op telemetry on `master`.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.PlayerCovertOps.cs`
  now exposes player assassination and sabotage issuance plus richer structured
  readout.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerCovertOpsSmokeValidation.cs`
  now includes the 3B assassination-target and sabotage-target phases on
  canonical `master`.
- Merged `master` re-passed runtime build, editor build, bootstrap runtime,
  combat, scene shells, fortification, siege, `node tests/data-validation.mjs`,
  `node tests/runtime-bridge.mjs`, and the extended dedicated player covert ops
  smoke.

### Immediate Next Action
1. Create fresh branch `codex/unity-player-counter-intelligence` from current `master`.
2. Port `tickDynastyCounterIntelligence` and `tickDynastyIntelligenceReports` under `unity/Assets/_Bloodlines/Code/PlayerCovertOps/`.
3. Extend the dedicated player covert ops smoke with counter-intelligence and intelligence-report assertions.

### Context Notes
- The checked-in bootstrap-runtime and canonical scene-shell wrappers are still
  pinned to `D:\ProjectsHome\Bloodlines`; continue using temporary worktree-safe
  copies when validating this clean worktree.
- `unity/ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json`
  still dirties during Unity validation and should remain unstaged.
- Node validations for this worktree must run from `D:\BLM13\bloodlines\bloodlines`
  rather than the outer worktree root `D:\BLM13\bloodlines`.
- Counter-intelligence watch, dossiers, retaliation metadata, and explicit
  bloodline-guard defense remain the next open player-covert-ops slice.

## Codex Player Covert Ops Sub-Slice 3C: Counter-Intelligence And Intelligence Reports (2026-04-21)

### Status: COMPLETE on branch `codex/unity-player-counter-intelligence`

### What Was Done
- Added `unity/Assets/_Bloodlines/Code/PlayerCovertOps/IntelligenceReportElement.cs`,
  `PlayerCounterIntelligenceComponent.cs`, and
  `PlayerCounterIntelligenceSystem.cs` to land player-owned report/watch state,
  expiry, dossier interception, and defended-op resolution.
- Extended
  `unity/Assets/_Bloodlines/Code/PlayerCovertOps/PlayerCovertOpsResolutionComponent.cs`
  with watch duration, watch strength, ward label, guarded-role summary, and
  loyalty telemetry.
- Reworked
  `unity/Assets/_Bloodlines/Code/PlayerCovertOps/PlayerCovertOpsSystem.cs`
  so it now dispatches player counter-intelligence with canonical
  `gold=60` / `influence=18`, 18-second activation delay, 150-second watch
  duration, operator order `Spymaster -> Diplomat -> HeadOfBloodline`, and
  defended espionage / assassination projected-chance penalties.
- Extended
  `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.PlayerCovertOps.cs`
  with `TryDebugIssuePlayerCounterIntelligence(...)`,
  `TryDebugGetPlayerCounterIntelligence(...)`,
  `TryDebugGetIntelligenceReports(...)`, and richer structured watch/readout
  fields.
- Added dedicated validator +
  wrapper:
  `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerCounterIntelligenceSmokeValidation.cs`
  and
  `scripts/Invoke-BloodlinesUnityPlayerCounterIntelligenceSmokeValidation.ps1`.
- Fixed the slice-discovered faction-root bug by preferring the actual faction
  entity over same-faction settlement entities when binding player covert-op
  watches and reports.
- Removed invalid `UpdateBefore(...)` attributes from
  `PlayerCounterIntelligenceSystem` after Unity correctly warned they targeted
  systems outside the same update group.
- Recorded the branch-side contract and continuity pass:
  - `docs/unity/CONCURRENT_SESSION_CONTRACT.md` revision `63 -> 64`
  - `docs/unity/session-handoffs/2026-04-21-unity-player-counter-intelligence.md`
  - `CURRENT_PROJECT_STATE.md`
  - `NEXT_SESSION_HANDOFF.md`
  - `continuity/PROJECT_STATE.json`

### Validation Proof
- Runtime build: `Build succeeded.` / `0 Error(s)`
- Editor build: `113 Warning(s)` / `0 Error(s)`
- Bootstrap runtime smoke:
  `Bootstrap runtime smoke validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity on map ironmark_frontier. ...`
- Combat smoke:
  `Combat smoke validation passed: meleePhase=True, projectilePhase=True, explicitAttackPhase=True, attackMovePhase=True, targetVisibilityPhase=True, groupMovementPhase=True, separationPhase=True, stancePhase=True.`
- Bootstrap scene shell:
  `Bootstrap scene shell validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity with canonical map Assets/_Bloodlines/Data/MapDefinitions/ironmark_frontier.asset.`
- Gameplay scene shell:
  `Gameplay scene shell validation passed for Assets/_Bloodlines/Scenes/Gameplay/IronmarkFrontier.unity.`
- Fortification smoke:
  `Fortification smoke validation passed: baselinePhase=True, tierAdvancePhase=True, reserveMusterPhase=True, reserveRecoveryPhase=True. ...`
- Siege smoke:
  `Siege smoke validation passed: baselinePhase=True, strainPhase=True, recoveryPhase=True, supportPhase=True. ...`
- Data validation: `Bloodlines data validation passed.`
- Runtime bridge: `Bloodlines runtime bridge validation passed.`
- Contract staleness before branch continuity:
  `STALENESS CHECK PASSED: Contract revision=63, last-updated=2026-04-21 is current. Latest handoff: 2026-04-21-unity-fortification-repair-narrative.md (2026-04-21).`
- Dedicated smoke:
  `BLOODLINES_PLAYER_COUNTER_INTELLIGENCE_SMOKE PASS`
  with
  `Phase 2 PASS: watchId=dynastyCounter-player-player-2073627-25, strength=34, opId=player-counter-intel-player-to-player-25, lapsed cleanly.`
  and
  `Phase 4 PASS: baselineChance=0.651, defendedChance=0.331, watchId=dynastyCounter-player-player-2764827-25, legitimacy 70->71.`

### Immediate Next Action
1. Stage the player covert ops 3C files plus continuity/contract updates and commit them on `codex/unity-player-counter-intelligence`.
2. Push the branch, merge it to `master`, and rerun the full governed validation gate on merged `master`.
3. After the landing continuity pass, close the player covert ops lane and claim the player HUD / realm-condition legibility lane.

### Context Notes
- The checked-in bootstrap-runtime and canonical scene-shell wrappers are still
  pinned to `D:\ProjectsHome\Bloodlines`; continue using temporary worktree-safe
  copies when validating this clean worktree.
- `unity/ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json`
  still dirties during Unity validation and should remain unstaged.
- Node validations for this worktree must run from `D:\BLM13\bloodlines\bloodlines`
  rather than the outer worktree root `D:\BLM13\bloodlines`.
- Snapshot/save-load integration for retained report/watch state remains
  deferred, and sabotage resolution still remains outside the scope of 3C.

## Codex Player Covert Ops Sub-Slice 3C Landing (2026-04-21)

### Status: MERGED to `master` via `661fea5b`

### What Landed
- `codex/unity-player-counter-intelligence` is now merged, so the player covert
  ops lane is complete through its planned directive scope.
- `unity/Assets/_Bloodlines/Code/PlayerCovertOps/IntelligenceReportElement.cs`,
  `PlayerCounterIntelligenceComponent.cs`, and
  `PlayerCounterIntelligenceSystem.cs` now canonically land player-owned
  report/watch state, dossier interception, and defended-op resolution.
- `unity/Assets/_Bloodlines/Code/PlayerCovertOps/PlayerCovertOpsSystem.cs`
  now canonically dispatches player counter-intelligence alongside espionage,
  assassination, and sabotage.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.PlayerCovertOps.cs`
  now canonically exposes counter-intelligence issue/readout and
  intelligence-report readout.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerCounterIntelligenceSmokeValidation.cs`
  plus
  `scripts/Invoke-BloodlinesUnityPlayerCounterIntelligenceSmokeValidation.ps1`
  now prove the 3C watch/report lifecycle on `master`.
- The slice-discovered faction-root bug is fixed on `master`, so player
  covert-op watches and reports bind to the owning faction root rather than a
  same-faction settlement shell.
- Merged `master` re-passed runtime build, editor build, bootstrap runtime,
  combat, scene shells, fortification, siege, `node tests/data-validation.mjs`,
  `node tests/runtime-bridge.mjs`, contract staleness, and the dedicated player
  counter-intelligence smoke.

### Immediate Next Action
1. Claim the player HUD / realm-condition legibility lane in `docs/unity/CONCURRENT_SESSION_CONTRACT.md`.
2. Create a fresh Codex branch for that HUD lane from current `master`.
3. Keep `unity/Assets/_Bloodlines/Code/AI/**` read-only and continue using worktree-local wrapper copies for the still-root-pinned bootstrap-runtime and scene-shell validators.

### Context Notes
- The checked-in bootstrap-runtime and canonical scene-shell wrappers are still
  pinned to `D:\ProjectsHome\Bloodlines`; continue using temporary worktree-safe
  copies when validating this clean worktree.
- `unity/ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json`
  still dirties during Unity validation and should remain unstaged.
- Node validations for this worktree must run from `D:\BLM13\bloodlines\bloodlines`
  rather than the outer worktree root `D:\BLM13\bloodlines`.
- Snapshot/save-load integration for retained report/watch state remains
  deferred, and sabotage resolution still remains outside the landed
  player-covert-ops scope.

## Codex Player HUD Realm-Condition Legibility Slice 1 (2026-04-21)

### Status: VALIDATED on branch `codex/unity-player-hud-realm-condition-legibility`

### What Landed On Branch
- `unity/Assets/_Bloodlines/Code/HUD/RealmConditionHUDComponent.cs` and
  `RealmConditionHUDSystem.cs` now project the canonical player-facing realm
  block from existing ECS state:
  - cycle count / progress
  - population pressure
  - food / water ratios and strain streaks
  - loyalty band
  - conviction band / label / color
  - faith covenant / doctrine / intensity / tier / band
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.HUD.cs`
  now exposes `TryDebugGetRealmConditionHUDSnapshot(...)` as a structured
  `RealmHUD|Key=Value|...` readout for smoke assertions.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesRealmConditionHUDSmokeValidation.cs`
  plus
  `scripts/Invoke-BloodlinesUnityRealmConditionHUDSmokeValidation.ps1`
  now prove four phases:
  1. stable baseline
  2. red realm strain
  3. Apex Moral conviction shift
  4. committed Fervent faith
- Narrow shared-file edits were applied to
  `unity/Assembly-CSharp.csproj`
  and
  `unity/Assembly-CSharp-Editor.csproj`
  so the new HUD files compile in the worktree.

### Validation Proof
- Runtime build: `Build succeeded.` / `0 Error(s)`
- Editor build: `113 Warning(s)` / `0 Error(s)`
- Dedicated HUD smoke:
  - `BLOODLINES_REALM_CONDITION_HUD_SMOKE PASS`
  - `Phase 1 PASS: stable baseline surfaces green realm bands, neutral conviction, red uncommitted faith, CycleProgress=0.500.`
  - `Phase 2 PASS: cap pressure, food strain, water strain, and loyalty distress all surface red with visible strain streaks.`
  - `Phase 3 PASS: ConvictionBand=ApexMoral, ConvictionLabel=Apex Moral, ConvictionScore=80.0.`
  - `Phase 4 PASS: FaithId=OldLight, DoctrinePath=Light, FaithLevel=4, FaithTier=Fervent, FaithBand=green.`
- Bootstrap runtime:
  - `Bootstrap runtime smoke validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity on map ironmark_frontier. Counts: factions=3, buildings=13, units=19, resourceNodes=13, controlPoints=4, settlements=2. ...`
- Combat:
  - `Combat smoke validation passed: meleePhase=True, projectilePhase=True, explicitAttackPhase=True, attackMovePhase=True, targetVisibilityPhase=True, groupMovementPhase=True, separationPhase=True, stancePhase=True.`
- Scene shells:
  - `Bootstrap scene shell validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity with canonical map Assets/_Bloodlines/Data/MapDefinitions/ironmark_frontier.asset.`
  - `Gameplay scene shell validation passed for Assets/_Bloodlines/Scenes/Gameplay/IronmarkFrontier.unity.`
- Fortification:
  - `Fortification smoke validation passed: baselinePhase=True, tierAdvancePhase=True, reserveMusterPhase=True, reserveRecoveryPhase=True. ...`
- Siege:
  - `Siege smoke validation passed: baselinePhase=True, strainPhase=True, recoveryPhase=True, supportPhase=True. ...`
- Data validation:
  - `Bloodlines data validation passed.`
- Runtime bridge:
  - `Bloodlines runtime bridge validation passed.`

### Immediate Next Action
1. Stage the HUD slice files plus continuity/contract updates and commit them on `codex/unity-player-hud-realm-condition-legibility`.
2. Push the branch, merge it to `master`, and rerun the full governed validation gate on merged `master`.
3. After the landing continuity pass, continue the lane with the match progression HUD slice.

### Context Notes
- The checked-in bootstrap-runtime and canonical scene-shell wrappers are still
  pinned to `D:\ProjectsHome\Bloodlines`; continue using the worktree-local
  copies under `artifacts/validation-temp/scripts` for those two validators.
- `unity/ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json`
  still dirties during Unity validation and should remain unstaged.
- The first HUD slice is intentionally a read-model + proof seam only; the
  on-screen player HUD surface and the broader match/fortification/world/victory
  HUD blocks remain follow-up work inside this same lane.

## Codex Player HUD Realm-Condition Legibility Slice 1 Landing (2026-04-21)

### Status: MERGED to `master` via `dfcbcec9`

### What Landed
- `codex/unity-player-hud-realm-condition-legibility` is now merged, so the
  first player-HUD slice is canonical on `master`.
- `unity/Assets/_Bloodlines/Code/HUD/RealmConditionHUDComponent.cs` and
  `RealmConditionHUDSystem.cs` now canonically project cycle, population,
  food, water, loyalty, conviction, and faith into a per-faction HUD read-model.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.HUD.cs`
  now canonically exposes the parseable `RealmHUD|Key=Value|...` readout.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesRealmConditionHUDSmokeValidation.cs`
  plus
  `scripts/Invoke-BloodlinesUnityRealmConditionHUDSmokeValidation.ps1`
  now canonically prove the first HUD slice on `master`.
- Merged `master` re-passed runtime build, editor build, bootstrap runtime,
  combat, canonical scene shells, fortification, siege,
  `node tests/data-validation.mjs`, `node tests/runtime-bridge.mjs`, contract
  staleness, and the dedicated realm-condition HUD smoke.

### Immediate Next Action
1. Create a fresh Codex branch from current `master` for the next player-HUD slice.
2. Implement `MatchProgressionHUDComponent`, `MatchProgressionHUDSystem`, and `TryDebugGetMatchHUDSnapshot`.
3. Keep `unity/Assets/_Bloodlines/Code/AI/**` read-only and keep using the worktree-local bootstrap-runtime and scene-shell wrapper copies.

### Context Notes
- The player HUD lane remains active, but there is currently no HUD branch in flight.
- The first HUD slice is still intentionally a read-model + proof seam only; the
  on-screen player HUD surface and the broader match/fortification/world/victory
  HUD blocks remain follow-up work inside this same lane.

## Codex Player HUD Match Progression Slice (2026-04-21)

### Status: VALIDATED on branch `codex/unity-player-hud-match-progression`

### What Landed On Branch
- `unity/Assets/_Bloodlines/Code/HUD/MatchProgressionHUDComponent.cs` and
  `MatchProgressionHUDSystem.cs` now project the browser's
  `getMatchProgressionSnapshot` shape into a Unity ECS HUD read-model:
  - stage number / id / label
  - phase id / label
  - readiness and next-stage label
  - in-world days / years and declaration count
  - dominant-leader faction telemetry
  - Great Reckoning target telemetry
  - resolved world-pressure level / label / score / targeted flag
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.HUD.cs`
  now exposes `TryDebugGetMatchHUDSnapshot(...)` as a structured
  `MatchHUD|Key=Value|...` readout for smoke assertions.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesMatchProgressionHUDSmokeValidation.cs`
  plus
  `scripts/Invoke-BloodlinesUnityMatchProgressionHUDSmokeValidation.ps1`
  now prove four phases:
  1. founding baseline
  2. Stage 4 commitment projection
  3. dominant-leader pressure
  4. Great Reckoning convergence
- Narrow shared-file edits were applied to
  `unity/Assembly-CSharp.csproj`
  and
  `unity/Assembly-CSharp-Editor.csproj`
  so the new match-HUD files compile in the clean worktree.

### Validation Proof
- Runtime build: `Build succeeded.` / `0 Error(s)`
- Editor build: `113 Warning(s)` / `0 Error(s)`
- Dedicated HUD smoke:
  - `BLOODLINES_MATCH_PROGRESSION_HUD_SMOKE PASS`
  - `Phase 1 PASS: founding stage surfaces emergence phase, quiet world pressure, and InWorldDays=12.0.`
  - `Phase 2 PASS: stage-4 commitment snapshot surfaces readiness, next-stage label, and declaration count.`
  - `Phase 3 PASS: dominant leader 'player' surfaces overwhelming world pressure level 2 and score 6.`
  - `Phase 4 PASS: Great Reckoning target 'enemy' surfaces convergence pressure level 3 at share 0.720.`
- Bootstrap runtime:
  - `Bootstrap runtime smoke validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity on map ironmark_frontier. Counts: factions=3, buildings=13, units=19, resourceNodes=13, controlPoints=4, settlements=2. ...`
- Combat:
  - `Combat smoke validation passed: meleePhase=True, projectilePhase=True, explicitAttackPhase=True, attackMovePhase=True, targetVisibilityPhase=True, groupMovementPhase=True, separationPhase=True, stancePhase=True.`
- Scene shells:
  - `Bootstrap scene shell validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity with canonical map Assets/_Bloodlines/Data/MapDefinitions/ironmark_frontier.asset.`
  - `Gameplay scene shell validation passed for Assets/_Bloodlines/Scenes/Gameplay/IronmarkFrontier.unity.`
- Fortification:
  - `Fortification smoke validation passed: baselinePhase=True, tierAdvancePhase=True, reserveMusterPhase=True, reserveRecoveryPhase=True. ...`
- Siege:
  - `Siege smoke validation passed: baselinePhase=True, strainPhase=True, recoveryPhase=True, supportPhase=True. ...`
- Data validation:
  - `Bloodlines data validation passed.`
- Runtime bridge:
  - `Bloodlines runtime bridge validation passed.`

### Immediate Next Action
1. Stage the match-progression HUD slice files plus continuity/contract updates and commit them on `codex/unity-player-hud-match-progression`.
2. Push the branch, merge it to `master`, and rerun the full governed validation gate on merged `master`.
3. After the landing continuity pass, continue the lane with the fortification-legibility or victory-readout HUD follow-up slice.

### Context Notes
- The checked-in bootstrap-runtime and canonical scene-shell wrappers are still
  pinned to `D:\ProjectsHome\Bloodlines`; continue using the worktree-local
  copies under `artifacts/validation-temp/scripts` for those two validators in
  clean worktrees.
- `unity/ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json`
  still dirties during Unity validation and should remain unstaged.
- This slice is still intentionally a read-model + proof seam only; the
  on-screen HUD and the remaining fortification / victory readouts are follow-up
  work inside the same player-HUD lane.

## Codex Player HUD Match Progression Slice Landing (2026-04-21)

### Status: MERGED to `master` via `ed22484c`

### What Landed
- `codex/unity-player-hud-match-progression` is now merged, so the second
  player-HUD slice is canonical on `master`.
- `unity/Assets/_Bloodlines/Code/HUD/MatchProgressionHUDComponent.cs` and
  `MatchProgressionHUDSystem.cs` now canonically project stage, phase,
  readiness, next-stage, declaration count, in-world time, dominant-leader
  telemetry, Great Reckoning telemetry, and resolved world-pressure state into
  a singleton HUD read-model.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.HUD.cs`
  now canonically exposes the parseable `MatchHUD|Key=Value|...` readout via
  `TryDebugGetMatchHUDSnapshot(...)`.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesMatchProgressionHUDSmokeValidation.cs`
  plus
  `scripts/Invoke-BloodlinesUnityMatchProgressionHUDSmokeValidation.ps1`
  now canonically prove the match-progression HUD slice on `master`.
- Merged `master` re-passed runtime build, editor build, the dedicated
  match-progression HUD smoke, bootstrap runtime, combat, canonical scene
  shells, fortification, siege, `node tests/data-validation.mjs`,
  `node tests/runtime-bridge.mjs`, and contract staleness.

### Immediate Next Action
1. Create a fresh Codex branch from current `master` for the next player-HUD slice.
2. Implement either the fortification-legibility HUD readout or the victory-distance readout next.
3. Keep `unity/Assets/_Bloodlines/Code/AI/**` read-only and keep using the worktree-local bootstrap-runtime and scene-shell wrapper copies.

### Context Notes
- The player HUD lane remains active, but there is currently no HUD branch in flight.
- The merged-master worktree now has expected local churn in
  `unity/Assembly-CSharp.csproj`,
  `unity/Assembly-CSharp-Editor.csproj`,
  and
  `unity/ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json`
  from worktree-specific analyzer-path regeneration and Unity validation; leave
  those files unstaged unless a slice explicitly needs to own them.
- The on-screen HUD and the remaining fortification / victory readouts are
  still follow-up work inside this same lane.

## Codex Player Diplomacy Holy War And Divine Right Slice (2026-04-22)

### Status: VALIDATED on branch `codex/unity-player-holy-war-divine-right`

### What Landed On Branch
- `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/PlayerHolyWarDeclarationRequestComponent.cs`
  and
  `PlayerDivineRightDeclarationRequestComponent.cs`
  now provide the player-owned request surfaces for issuing faith declarations.
- `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/PlayerFaithDeclarationUtility.cs`
  now centralizes faction-root preference, kingdom gating, active-operation
  scans, faith-operator selection, and declaration narrative helpers for the
  `PlayerDiplomacy/**` lane.
- `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/PlayerHolyWarDeclarationSystem.cs`
  now ports player-side holy-war declaration dispatch using the browser
  `getHolyWarDeclarationTerms` / `startHolyWarDeclaration` parity subset:
  source and target kingdom validation, committed faith validation, harmony
  rejection for identical `(faith, doctrine)`, duplicate-active-op rejection,
  canonical `influence=24` and `intensity=18` deduction, active-cap
  enforcement through read-only `DynastyOperationLimits`, AI-owned
  `DynastyOperationComponent` + `DynastyOperationHolyWarComponent` creation,
  and narrative emission through `NarrativeMessageBridge`.
- `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/PlayerDivineRightDeclarationSystem.cs`
  now ports player-side divine-right declaration dispatch using the browser
  `getDivineRightDeclarationTerms` / `startDivineRightDeclaration` parity
  subset: committed faith, `intensity >= 80`, `level >= 5`, no existing active
  declaration for the same faction, active-cap enforcement, AI-owned
  `DynastyOperationComponent` + `DynastyOperationDivineRightComponent`
  creation, and declaration narrative emission.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.PlayerDiplomacy.cs`
  now exposes:
  - `TryDebugIssuePlayerHolyWarDeclaration(...)`
  - `TryDebugIssuePlayerDivineRightDeclaration(...)`
  - `TryDebugGetPlayerFaithDeclarationOperations(...)`
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerHolyWarDivineRightSmokeValidation.cs`
  plus
  `scripts/Invoke-BloodlinesUnityPlayerHolyWarDivineRightSmokeValidation.ps1`
  now prove four phases:
  1. holy-war success against an incompatible-faith kingdom
  2. holy-war rejection for harmonious same-faith/same-doctrine targets
  3. divine-right success at intensity 85 and level 5
  4. divine-right rejection below the threshold
- Narrow shared-file edits were applied to
  `unity/Assembly-CSharp.csproj`
  and
  `unity/Assembly-CSharp-Editor.csproj`
  so the generated local projects compile the new player-diplomacy files.

### Validation Proof
- Runtime build: `Build succeeded.` / `0 Error(s)`
- Editor build: `Build succeeded.` / `0 Error(s)`
- Bootstrap runtime: `Bootstrap runtime smoke validation passed.`
- Combat smoke: Unity exit code `0`
- Scene shells: bootstrap and gameplay scene shell validation both passed
- Fortification smoke: `Fortification smoke validation passed.`
- Siege smoke: Unity exit code `0`
- Data validation: `Bloodlines data validation passed.`
- Runtime bridge: `Bloodlines runtime bridge validation passed.`
- Contract staleness: `STALENESS CHECK PASSED: Contract revision=71, last-updated=2026-04-22 is current.`
- Dedicated smoke: `Player holy war/divine right smoke validation passed.`

### Immediate Next Action
1. Stage the player-faith declaration slice files plus continuity/contract updates and commit them on `codex/unity-player-holy-war-divine-right`.
2. Push the branch to `origin`, merge it to `master`, and rerun the full governed validation gate on merged `master`.
3. After the landing continuity pass, continue the next directive item from the current player-facing priority stack on a fresh Codex branch.

### Context Notes
- `unity/Assets/_Bloodlines/Code/AI/**` remained read-only throughout the slice;
  the new systems only reuse AI-owned dynasty-operation payload shapes.
- `unity/ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json`
  still dirties during Unity validation and should remain unstaged for this slice.
- Holy-war compatibility is still a simplified identical `(faith, doctrine)`
  gate rather than the browser's fuller compatibility ladder because the
  covenant covariance surface is not yet ported.

## Codex Player Diplomacy Missionary Dispatch Slice (2026-04-22)

### Status: VALIDATED on branch `codex/unity-player-missionary-dispatch`

### What Landed On Branch
- `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/PlayerMissionaryDispatchRequestComponent.cs`
  now provides the player-owned request surface for missionary dispatch.
- `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/PlayerMissionaryDispatchSystem.cs`
  now ports player-side missionary dispatch using the browser
  `getMissionaryTerms` / `startMissionaryOperation` parity subset: source and
  target validation, committed faith validation, same-faith rejection,
  duplicate-active-op rejection, canonical `influence=14` and `intensity=12`
  deduction, active-cap enforcement through read-only `DynastyOperationLimits`,
  AI-owned `DynastyOperationComponent` + `DynastyOperationMissionaryComponent`
  creation, and missionary narrative emission through `NarrativeMessageBridge`.
- `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/PlayerFaithDeclarationUtility.cs`
  now exposes a renown-returning faith-operator lookup overload and a
  missionary narrative helper so the player-faith slices reuse one utility
  surface.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.PlayerDiplomacy.cs`
  now exposes:
  - `TryDebugIssuePlayerMissionaryDispatch(...)`
  - missionary payload fields in `TryDebugGetPlayerFaithDeclarationOperations(...)`
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerMissionaryDispatchSmokeValidation.cs`
  plus
  `scripts/Invoke-BloodlinesUnityPlayerMissionaryDispatchSmokeValidation.ps1`
  now prove three phases:
  1. missionary success creates the AI-owned operation payload and deducts canonical influence/intensity
  2. insufficient influence blocks creation and preserves stockpile
  3. active-cap at six blocks dispatch
- Narrow shared-file edits were applied to
  `unity/Assembly-CSharp.csproj`
  and
  `unity/Assembly-CSharp-Editor.csproj`
  so the initialized local projects compile the new player-diplomacy files.

### Validation Proof
- Runtime build: `Build succeeded.` / `0 Error(s)`
- Editor build: `Build succeeded.` / `0 Error(s)`
- Bootstrap runtime: `Bootstrap runtime smoke validation passed.`
- Combat smoke: Unity exit code `0`
- Scene shells: bootstrap and gameplay scene shell validation both passed
- Fortification smoke: `Fortification smoke validation passed.`
- Siege smoke: Unity exit code `0`
- Data validation: `Bloodlines data validation passed.`
- Runtime bridge: `Bloodlines runtime bridge validation passed.`
- Contract staleness: `STALENESS CHECK PASSED: Contract revision=72, last-updated=2026-04-22 is current.`
- Dedicated smoke: `Player missionary dispatch smoke validation passed.`

### Immediate Next Action
1. Stage the player missionary dispatch slice files plus continuity/contract updates and commit them on `codex/unity-player-missionary-dispatch`.
2. Push the branch to `origin`, merge it to `master`, and rerun the full governed validation gate on merged `master`.
3. After the landing continuity pass, continue the next directive item on a fresh Codex branch: conviction-band wiring.

### Context Notes
- `unity/Assets/_Bloodlines/Code/AI/**` remained read-only throughout the slice;
  the new system only reuses the AI-owned missionary payload shape.
- `unity/ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json`
  still dirties during Unity validation and should remain unstaged for this slice.
- Missionary defense still uses the same simplified parity already used by the
  AI missionary dispatch slice; target-operator renown and ward bonuses remain
  deferred.

## Codex Conviction Band Wiring Checkpoint (2026-04-22)

### Branch

- `codex/unity-conviction-band-wiring`

### What Changed So Far

- `unity/Assets/_Bloodlines/Code/Economy/StarvationResponseSystem.cs`
  now reads `ConvictionComponent` and applies starvation-side conviction protection:
  negative loyalty deltas are divided by `max(1, LoyaltyProtectionMultiplier)` and famine population decline is reduced
  by `PopulationGrowthMultiplier` with a minimum decline floor of one.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesConvictionSmokeValidation.cs`
  now includes a fifth starvation-protection phase proving an `ApexMoral` faction loses less loyalty and population
  than a `Neutral` faction under identical famine conditions.
- `docs/unity/CONCURRENT_SESSION_CONTRACT.md`
  bumped revision `72 -> 73` and claims the new `conviction-band-wiring` lane with this branch as the active owner.

### Validation Completed

- Runtime build: `Build succeeded.` / `0 Error(s)`
- Editor build: `Build succeeded.` / `0 Error(s)`
- Conviction smoke: `Conviction smoke validation passed.`

### Immediate Next Action

1. Resolve the combat-side `CaptureMultiplier` seam before touching `unity/Assets/_Bloodlines/Code/Combat/AttackResolutionSystem.cs`.
2. The core decision is whether conviction-based capture should apply only to commander / bloodline-backed combatants
   (the only units with stable member identity today) or whether a broader unit-to-dynasty captive bridge must land
   first.
3. After that decision, wire the combat hook, extend the smoke surface, run the full 10-gate chain, and then land the
   slice.

### Context Notes

- `CapturedMemberElement` requires `MemberId` and `MemberTitle`. Those exist on `DynastyMemberComponent` and
  `CommanderComponent`, not on ordinary militia / worker units.
- The existing branch still has the usual unstaged Unity churn at
  `unity/ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json`; keep it out of any commit.

## Codex Conviction Band Wiring Completion (2026-04-22)

### Branch

- `codex/unity-conviction-band-wiring-finish`

### What Landed On Branch

- `unity/Assets/_Bloodlines/Code/Economy/StarvationResponseSystem.cs`
  retains the checkpointed conviction protection for negative starvation loyalty loss and famine population decline.
- `unity/Assets/_Bloodlines/Code/Economy/CapPressureResponseSystem.cs`
  now applies the same loyalty-protection multiplier to negative cap-pressure loyalty loss.
- `unity/Assets/_Bloodlines/Code/Combat/PendingCommanderCaptureComponent.cs`
  introduces a narrow commander-only conviction capture helper and pending-capture marker.
- `unity/Assets/_Bloodlines/Code/Combat/AttackResolutionSystem.cs`
  and
  `unity/Assets/_Bloodlines/Code/Combat/ProjectileImpactSystem.cs`
  now mark lethal commander defeats for capture using the attacker's faction conviction band.
- `unity/Assets/_Bloodlines/Code/Combat/DeathResolutionSystem.cs`
  now resolves pending commander capture before death cleanup, writes `CapturedMemberElement` to the captor faction
  root, and marks the matching dynasty member `Captured`.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesConvictionSmokeValidation.cs`
  now proves starvation protection, cap-pressure protection, and deterministic commander capture in dedicated ECS
  worlds.

### Validation Proof

- Runtime build: `Build succeeded.` / `0 Error(s)`
- Editor build: `Build succeeded.` / `0 Error(s)` with existing repo-wide warnings only
- Bootstrap runtime: `Bootstrap runtime smoke validation passed.`
- Combat smoke: Unity exit code `0`
- Scene shells: bootstrap and gameplay scene shell validation both passed
- Fortification smoke: `Fortification smoke validation passed.`
- Siege smoke: Unity exit code `0`
- Data validation: `Bloodlines data validation passed.`
- Runtime bridge: `Bloodlines runtime bridge validation passed.`
- Contract staleness: `STALENESS CHECK PASSED: Contract revision=74, last-updated=2026-04-22 is current.`
- Dedicated smoke: `Conviction smoke validation passed.`

### Immediate Next Action

1. Stage the conviction-band wiring slice files plus continuity/contract updates and commit them on `codex/unity-conviction-band-wiring-finish`.
2. Push the branch to `origin`, merge it to `master`, and rerun the full governed validation gate on merged `master`.
3. After the landing continuity pass, continue the next Codex player-facing follow-up slice: fortification legibility or victory-distance HUD readout.

### Context Notes

- The conviction `CaptureMultiplier` is intentionally implemented only for commanders / bloodline-backed combatants in
  this slice because generic combat units still lack captive-ready dynasty identity.
- `unity/ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json`
  still dirties during Unity validation and should remain unstaged for this slice.

## Codex Player HUD Fortification Legibility Slice (2026-04-22)

### Branch

- `codex/unity-player-hud-fortification`

### What Landed On Branch

- `unity/Assets/_Bloodlines/Code/HUD/FortificationHUDComponent.cs`
  and
  `unity/Assets/_Bloodlines/Code/HUD/FortificationHUDSystem.cs`
  now project fortification tier, breach count, reserve frontage, mustered defenders, ready / recovering reserve
  counts, threat state, and sealing / recovery progress into a player-facing HUD read-model.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.HUD.cs`
  now exposes a parseable `FortificationHUD|Key=Value|...` settlement readout for command-surface and smoke usage.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesFortificationHUDSmokeValidation.cs`
  plus
  `scripts/Invoke-BloodlinesUnityFortificationHUDSmokeValidation.ps1`
  now prove baseline reserve state, active breach / threat projection, and sealing / recovery progress transitions in a
  dedicated ECS validation world.
- `unity/Assembly-CSharp.csproj`
  and
  `unity/Assembly-CSharp-Editor.csproj`
  were refreshed so the initialized local project metadata includes the new runtime and editor slice files.

### Validation Proof

- Runtime build: `Build succeeded.` / `0 Error(s)`
- Editor build: `Build succeeded.` / `0 Error(s)`
- Bootstrap runtime: `Bootstrap runtime smoke validation passed.`
- Combat smoke: Unity exit code `0`
- Scene shells: bootstrap and gameplay scene shell validation both passed
- Fortification smoke: `Fortification smoke validation passed.`
- Siege smoke: Unity exit code `0`
- Data validation: `Bloodlines data validation passed.`
- Runtime bridge: `Bloodlines runtime bridge validation passed.`
- Contract staleness: `STALENESS CHECK PASSED: Contract revision=78, last-updated=2026-04-22 is current.`
- Dedicated smoke: `Fortification HUD smoke validation passed.`

### Immediate Next Action

1. Stage only the fortification HUD slice files plus continuity / contract updates and commit them on `codex/unity-player-hud-fortification`.
2. Push the branch to `origin`.
3. After the landing continuity pass, continue the next player-HUD follow-up slice: victory-distance readout.

### Context Notes

- `unity/Assets/_Bloodlines/Code/AI/**` remained untouched; this slice only reads shared fortification and reserve state.
- `unity/ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json`
  still dirties during Unity validation and should remain unstaged for this slice.

## Codex Player HUD Fortification Legibility Rerun (2026-04-22)

### Branch

- `codex/unity-player-hud-fortification-legibility-rerun`

### What Reran On Branch

- The preserved fortification HUD slice was replayed onto the current canonical
  `origin/master` baseline without widening the scope:
  `unity/Assets/_Bloodlines/Code/HUD/FortificationHUDComponent.cs`,
  `FortificationHUDSystem.cs`,
  `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.HUD.cs`,
  `unity/Assets/_Bloodlines/Code/Editor/BloodlinesFortificationHUDSmokeValidation.cs`,
  and
  `scripts/Invoke-BloodlinesUnityFortificationHUDSmokeValidation.ps1`.
- `unity/Assembly-CSharp.csproj`
  and
  `unity/Assembly-CSharp-Editor.csproj`
  were regenerated locally after Unity refreshed stale analyzer references that
  still pointed at another worktree's `Library\PackageCache`.
- Worktree-local wrapper copies under `artifacts/local-wrappers/` were used only
  for the still-root-pinned bootstrap-runtime and canonical scene-shell
  validators; all other governed wrappers ran from `scripts/` under the Unity
  lock.

### Validation Proof

- Runtime build: `Build succeeded.` / `0 Error(s)`
- Editor build: `Build succeeded.` / `0 Error(s)` with existing repo-wide warnings only
- Bootstrap runtime: `Bootstrap runtime smoke validation passed.`
- Combat smoke: Unity exit code `0`
- Scene shells: bootstrap and gameplay scene shell validation both passed
- Fortification smoke: `Fortification smoke validation passed.`
- Siege smoke: Unity exit code `0`
- Data validation: `Bloodlines data validation passed.`
- Runtime bridge: `Bloodlines runtime bridge validation passed.`
- Contract staleness: `STALENESS CHECK PASSED: Contract revision=78, last-updated=2026-04-22 is current.`
- Dedicated smoke: `Fortification HUD smoke validation passed.`

### Immediate Next Action

1. Stage only the fortification HUD rerun slice files plus continuity / contract
   updates and commit them on `codex/unity-player-hud-fortification-legibility-rerun`.
2. Push the branch to `origin`.
3. Continue the next HUD follow-up slice on a fresh Codex branch: victory-distance
   readout.

### Context Notes

- `unity/Assets/_Bloodlines/Code/AI/**` remained untouched; this rerun only
  consumes the shared fortification, reserve, and repair-progress read seams.
- `unity/ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json`
  still dirties during Unity validation and should remain unstaged.

## Codex Player HUD Victory Distance Readout (2026-04-22)

### Branch

- `codex/unity-hud-victory-distance-readout`

### What Landed On Branch

- `unity/Assets/_Bloodlines/Code/HUD/VictoryConditionReadoutComponent.cs`
  and
  `VictoryConditionReadoutSystem.cs`
  now attach a per-faction HUD-owned DynamicBuffer read-model for
  `TerritorialGovernance`, `DivineRight`, and `CommandHallFall`.
- The readout system refreshes on an in-world-day cadence, uses the canonical
  thresholds already defined in the retired victory lane, marks the leading
  faction for each condition, and exposes an ETA only when the current ECS state
  can compute one safely.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.HUD.cs`
  now exposes a parseable multi-line
  `VictoryReadout|FactionId=...|ConditionId=...|ProgressPct=...|IsLeading=...|TimeRemainingEstimateInWorldDays=...`
  seam for smoke and later HUD panel wiring.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesVictoryReadoutSmokeValidation.cs`
  plus
  `scripts/Invoke-BloodlinesUnityVictoryReadoutSmokeValidation.ps1`
  now prove majority-loyal Territorial Governance progress, positive Divine
  Right progress from high intensity, terminal Command Hall Fall completion, and
  per-condition leader flags.
- `unity/Assembly-CSharp.csproj`
  and
  `unity/Assembly-CSharp-Editor.csproj`
  were locally repaired so stale analyzer/source-generator paths no longer point
  at another Codex worktree's `unity/Library/PackageCache`.

### Validation Proof

- Dedicated smoke:
  - `Victory readout smoke validation passed.`
- Runtime build:
  - `Build succeeded.`
  - `0 Error(s)`
- Editor build:
  - `Build succeeded.`
  - `0 Error(s)` with existing editor warnings only

### Immediate Next Action

1. Run the full governed 10-gate chain with the updated contract and continuity
   files.
2. Stage only the victory-readout slice files plus continuity / contract
   updates, commit them on `codex/unity-hud-victory-distance-readout`, and push
   to `origin`.
3. Continue the HUD lane with the victory-condition leaderboard panel or the
   next player-diplomacy captive dispatch slice.

### Context Notes

- `unity/Assets/_Bloodlines/Code/AI/**` remained untouched; the slice only reads
  retired victory definitions plus shared faction, control-point, building, and
  dual-clock state.
- `unity/ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json`
  still dirties during Unity validation and should remain unstaged.

## Codex Player Captive Rescue Dispatch (2026-04-22)

### Branch

- `codex/unity-player-captive-rescue`

### What Landed On Branch

- `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/PlayerCaptiveRescueRequestComponent.cs`,
  `PlayerCaptiveDispatchUtility.cs`,
  and
  `PlayerCaptiveRescueDispatchSystem.cs`
  now port the browser's player-side captive rescue dispatch seam under the
  existing `PlayerDiplomacy/**` lane without widening `AI/**`.
- The player rescue flow now validates kingdom state, verifies the requested
  dynasty member is currently `Captured`, resolves the holding faction from
  the AI-owned `CapturedMemberElement` buffers, rejects duplicate active rescue
  ops, reuses the AI rescue operator priorities, deducts canonical
  `gold=42` / `influence=26`, creates
  `DynastyOperationComponent` plus
  `DynastyOperationCaptiveRescueComponent`,
  and emits a rescue narrative line.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.PlayerDiplomacy.cs`
  now exposes `TryDebugDispatchCaptiveRescue(...)`.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerCaptiveRescueSmokeValidation.cs`
  plus
  `scripts/Invoke-BloodlinesUnityPlayerCaptiveRescueSmokeValidation.ps1`
  now prove three phases: rescue success, no-operator rejection, and
  missing-captive rejection.
- `unity/Assembly-CSharp.csproj`
  and
  `unity/Assembly-CSharp-Editor.csproj`
  now include the new player-diplomacy runtime and smoke files for this local
  initialized Unity project.

### Validation Proof

- Runtime build:
  - `Build succeeded.`
  - `0 Error(s)`
- Editor build:
  - `Build succeeded.`
  - `0 Error(s)` with existing repo-wide warnings only
- Bootstrap runtime:
  - `Bootstrap runtime smoke validation passed.`
- Combat smoke:
  - `Unity exited with code 0`
- Scene shells:
  - `Bootstrap scene shell validation passed.`
  - `Gameplay scene shell validation passed.`
- Fortification smoke:
  - `Fortification smoke validation passed.`
- Siege smoke:
  - `Unity exited with code 0`
- Data validation:
  - `Bloodlines data validation passed.`
- Runtime bridge:
  - `Bloodlines runtime bridge validation passed.`
- Contract staleness:
  - `STALENESS CHECK PASSED: Contract revision=80, last-updated=2026-04-22 is current.`
- Dedicated smoke:
  - `Player captive rescue smoke validation passed.`

### Immediate Next Action

1. Stage the player captive rescue slice files plus contract and continuity
   updates, commit them on `codex/unity-player-captive-rescue`, and push to
   `origin`.
2. Continue the next player-diplomacy captive follow-up on a fresh branch:
   player captive ransom dispatch.

### Context Notes

- `unity/Assets/_Bloodlines/Code/AI/**` remained untouched; this slice only
  reads captive holding buffers, rescue operation payloads, and rescue helper
  constants.
- `unity/ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json`
  still dirties during Unity validation and should remain unstaged.
