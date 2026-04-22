# CURRENT_PROJECT_STATE

Last updated: 2026-04-21 (scout raids and logistics interdiction landed on `master` via merge commit `dda7c25e`: `ScoutRaidCommandComponent`, `BuildingRaidStateComponent`, `ScoutRaidCanon`, and `ScoutRaidResolutionSystem` now port building raids plus supply-wagon interdiction; trickle, worker drop-off, field-water, and siege-support consumers now respect active raid state; dedicated 4-phase scout smoke PASS; full governed validation chain rerun green on detached merged `master` in `D:\BLAICD\bloodlines`; contract revision 50 -> 51)
Previous entry: Last updated: 2026-04-19 (fortification-siege sub-slice 5 wall-segment destruction resolution complete on branch `codex/unity-fortification-wall-segment-destruction`: live fortification-role buildings now auto-link to their nearest same-faction settlement, destroyed wall/tower/gate/keep counts plus `OpenBreachCount` now resolve onto `FortificationComponent`, `FortificationReserveSystem` now runs after destruction accounting so breached walls reduce reserve frontage immediately, dedicated 4-phase wall-destruction smoke PASS, full governed gate chain rerun green on the rebased `origin/master` `0a0e122f` base, contract revision 30 -> 31)
Previous entry: Last updated: 2026-04-18 (Session 130: Victory Conditions System complete on branch codex/unity-fortification-siege: VictoryStateComponent singleton, VictoryConditionEvaluationSystem (CommandHallFall/TerritorialGovernance/DivinRight), 4-phase smoke PASS, all 8 gates green, contract revision 11 → 12. Tier 2 batch dynasty systems also complete: Tier 2 batch dynasty systems complete on branch codex/unity-fortification-siege: RenownAwardSystem (Commander priority routing, RENOWN_CAP=100), MarriageProposalExpirationSystem (30-day DualClock expiration), MarriageGestationSystem (60-day gestation, HeirDesignate child spawn), LesserHouseLoyaltyDriftSystem (DualClock-gated drift, deferred spawn NativeList structural-change fix, SpawnDefectedMinorFaction), MinorHouseLevySystem (0.001f interval floor, levy_militia unit spawn), MarriageComponents + RenownAwardRequestComponent, BloodlinesTier2BatchSmokeValidation 5-phase validator all green, MinorHouse=3 added to FactionKind enum, all 8 validation gates green, tier2 batch smoke PASS, contract revision 10 → 11; Session 129: AI strategic layer sub-slice 1 complete on branch codex/unity-fortification-siege: EnemyAIStrategySystem ISystem (territory expansion, scout/harass, world-pressure posture conditioning, reinforcement routing), AIStrategyComponent, AIStrategicPosture enum, BloodlinesAIStrategySmokeValidation 4-phase validator all green, BloodlinesDebugCommandSurface.AIStrategy.cs slimmed to debug-read-only API, SkirmishBootstrapSystem seeds AIStrategyComponent on non-player Kingdom factions, TickAIStrategyFactions MonoBehaviour tick removed from Update() in favor of ISystem, all 8 validation gates green, AI strategy smoke PASS, contract revision 9 → 10; Session 128: world-pressure escalation lane complete on branch codex/unity-world-pressure (merged); WorldPressureComponent, WorldPressureCycleTrackerComponent, WorldPressureEscalationSystem, 4-phase smoke validator all green, MatchProgressionEvaluationSystem stage-5 convergence signal wired, BloodlinesBootstrapRuntimeSmokeValidation ProbeWorldPressureIntegrity probe added, contract revision 8 → 9)
Previous entry: Last updated: 2026-04-17 (state-snapshot-restore lane complete: BloodlinesSnapshotPayload schema v1 with 11 list fields, BloodlinesSnapshotWriter.Capture keyed by FactionId excluding member entities, BloodlinesSnapshotRestorer.Apply restoring all faction-scoped components including dynasty member entities via DynastyMemberRef buffer, SnapshotVersionComponent, debug surface TryDebugGetSnapshotPayload/TryDebugCaptureSnapshot, 6-phase save-load smoke validator (empty world, single faction, full faction, round-trip deep-equal, conviction band after restore, dynasty members/legitimacy after restore), ProbeSnapshotIntegrity probe added to bootstrap runtime smoke (snapshotIntegrityChecked=True in pass line), all 10 validation gates green, all 3 sub-slices merged to master; 10 Claude Code skills merged to master: canon-enforcement, performance-and-scale, unity-ecs-discipline, slice-completion, unity-process-reset, balance-and-counterplay, rts-pattern-review, asset-pipeline-discipline, browser-reference-lookup, lane-claim; Codex Unity group-aware movement + soft unit separation + combat stances green on rebased branch `codex/unity-group-movement-and-stances` with `GroupMovementOrderComponent`, `GroupMovementResolutionSystem`, `UnitSeparationComponent`, `UnitSeparationSystem`, `RecentImpactComponent`, `RecentImpactRecoverySystem`, `CombatStanceComponent`, `CombatStanceResolutionSystem`, `CombatUnitRuntimeDefaults`, new debug partials `BloodlinesDebugCommandSurface.Movement.cs` and `.Stances.cs`, spawn-time stance and separation wiring through authoring/baker/bootstrap/production seams, dedicated combat smoke validator now proving eight phases including group-movement, separation, hold-position, and retreat-low-hp, governed gate chain rerun green on the rebased head with bootstrap runtime smoke, canonical scene-shell validation, graphics runtime validation, Node checks, and contract staleness check all preserved, plus wrapper hardening in the governed Unity runtime and scene validators so pass/fail log outcomes remain authoritative under the shared-worktree harness; Session 125: Unity explicit attack orders and first attack-move command layer green on branch `codex/unity-attack-orders-attack-move` with `AttackOrderComponent`, `AttackOrderSystem`, explicit-target preference inside `AutoAcquireTargetSystem`, attack-order cleanup in `DeathResolutionSystem`, debug right-click hostile attack orders plus `A` attack-move cursor mode and `Esc` cancel in `BloodlinesDebugCommandSurface.AttackOrders.cs`, dedicated combat smoke validator now proving melee + projectile + explicit attack-order phases, bootstrap runtime smoke preserved green with `gatherDepositObserved=True`, `trickleGainObserved=True`, `starvationObserved=True`, `loyaltyDeclineObserved=True`, and `capPressureObserved=True`, and both canonical scene-shell validators preserved green in the clean attack-order worktree through clone-local wrapper scripts because the checked-in runtime and scene validators remain pinned to the dirty canonical checkout; Session 120: Unity projectile combat green on branch `codex/unity-projectile-combat` with `ProjectileComponent` and `ProjectileFactoryComponent`, projectile movement and impact systems, ranged and projectile-siege attacks now spawning travel-time projectile entities instead of applying instant damage, bootstrap authoring and baker plus bootstrap and production spawn seams now carrying projectile payloads, debug projectile proxies visible in Play Mode, the dedicated combat smoke validator now proving both melee and projectile phases, bootstrap runtime smoke preserved green with `gatherDepositObserved=True`, `trickleGainObserved=True`, `starvationObserved=True`, `loyaltyDeclineObserved=True`, and `capPressureObserved=True`, and both canonical scene-shell validators preserved green in the clean projectile worktree; Session 119: Unity combat foundation green on branch `codex/unity-combat-foundation` with CombatStats/AttackTarget/Hostility runtime payloads, auto-acquire/attack/death systems, bootstrap + production combat-stat wiring, dedicated combat smoke validation, normalization of browser-spec combat distances into Unity world-space via `map.tileSize`, dedicated combat smoke pass, and bootstrap runtime smoke preserved green in both the worktree-equivalent batch pass and the unchanged governed wrapper. Codex originally labeled this slice Session 113 but it has been renumbered Session 119 during the master merge to avoid collision with continuation-platform Session 113; Session 118: Unity cap-pressure loyalty response green with PopulationCapPressureRatio (0.95) and CapPressureLoyaltyDeltaPerCycle (-1) added to RealmCycleConfig, new CapPressureResponseSystem running after RealmConditionCycleSystem with its own LastCapPressureResponseCycle marker, TryDebugForceCapPressureCycle debug API that clears starvation streaks so the proof is surgical, governed bootstrap runtime smoke proof (capPressureLoyaltyBeforeCycle=60.00 to capPressureLatestLoyalty=59.00, capPressureObserved=True), completing the canonical realm-condition effect trio of famine, water crisis, and cap pressure; Session 117: Unity faction loyalty + famine/water-crisis loyalty deltas green with new FactionLoyaltyComponent (Current/Max/Floor, seed default 70), FamineLoyaltyDeltaPerCycle (-4) and WaterCrisisLoyaltyDeltaPerCycle (-6) added to RealmCycleConfig, StarvationResponseSystem extended to apply loyalty deltas alongside population decline, TryDebugGetFactionLoyalty debug API, governed bootstrap runtime smoke proof (loyaltyPreviousValue=70.00 to loyaltyLatestValue=60.00, loyaltyDeclineObserved=True); Session 116: Unity starvation response green with LastStarvationResponseCycle marker on RealmConditionComponent, canonical foodFamineConsecutiveCycles/waterCrisisConsecutiveCycles/populationDecline/outmigration thresholds seeded on RealmCycleConfig, new StarvationResponseSystem applying famine population decline and water-crisis outmigration once per cycle, TryDebugForceStarvationCycle and TryDebugGetFactionPopulation debug APIs, governed bootstrap runtime smoke proof (starvationPreviousPopulation=14 to starvationLatestPopulation=12, starvationObserved=True), StartupTimeoutSeconds bumped 75 to 120 for the longer combined sequence; Session 115: Unity building resource trickle green with per-building ResourceTrickleBuildingComponent, per-dt ResourceTrickleBuildingSystem that ports browser tickPassiveResources into ECS, BuildingDefinition.resourceTrickle refactored to float ResourceTrickleFields so canonical farm food 0.5 and well water 0.45 survive JSON-to-ScriptableObject sync, seed fields propagated through authoring/baker/skirmish bootstrap and runtime construction paths, governed two-phase food-and-water trickle proof inside bootstrap runtime smoke (trickleGainObserved=True with food 39.70 to 43.77 and water 37.13 to 40.69), first concurrent-session contract and wrapper-lock helper landed so Claude (economy) and Codex (combat) can run parallel without colliding; Session 114: continuation platform governed task loop green with five new additions on the main Command Deck: persistent execution plans, `/plan` `/plans` `/show-plan` `/run-plan` `/archive-plan` command set, sequential step execution across governed local actions, persistent recent-run history, and natural-language model support for execution-plan staging; smoke coverage extended for plan create/show/run/archive; live `8067` server rebuilt onto the new plan-capable surface; Session 113: continuation platform governed local command runner green on the main Command Deck with `/run <command>`, policy-backed read-only inspection while locked, tier-gated validation and build execution, blocked direct file mutation and destructive/networked commands, live HTTP `/run` validation on `8067`, and refreshed continuation docs and state; Session 112: Unity worker gather-deposit primary economy loop green with WorkerGatherComponent, WorkerGatherSystem, nearest-command-hall drop-off targeting, all seven canonical primary resource routes into ResourceStockpileComponent, debug API for gather assignment and stockpile inspection, governed 5-worker gold-gather cycle proof in bootstrap runtime smoke artifact, 75-to-120-second startup timeout bump, and first origin push of Sessions 100-108 + full canonical corpus + Unity source + ScriptableObject data + continuation-platform source to lance-fisher/lancewfisher-v2 master; Session 111: finalization handoff and next-session continuation prompt refreshed so the current Session 110 command-deck and Unity-shipping state is the canonical re-entry path instead of the obsolete Session 104 prompt; Session 110: continuation platform chat-first Command Deck green with persistent offline back-and-forth Bloodlines agent conversation, slash-command local actions, local-model prompt handling, governed write-draft staging and apply flow, live HTTP validation, and `8067` rebuilt as the primary offline continuation surface; Session 109: continuation platform execution packet and governed write workbench green with dynamic current-canon ingestion, Unity shipping-lane execution view, safe project-file load and diff preview, real tier-gated project-artifact apply with stale-source protection and backups, live HTTP validation, and current Unity recommendation repair; Session 108: Unity first-shell production progress observability green with world-space production progress bar, debug API production progress getter, governed mid-production queue advancement proof carried into the bootstrap runtime smoke artifact, and spawn-floor and equality gates scoped to allow mid-production observation; Session 107: Unity first-shell construction progress observability and UI green with selection-aware construction-progress panel, world-space construction progress bar, debug API progress getter, and governed mid-construction advancement proof carried into the bootstrap runtime smoke artifact; Session 106: owner direction refresh saved with full-canon Unity 6.3 DOTS/ECS delivery, browser freeze, and no-MVP rule; Session 105: project-completion handoff refreshed with completed-vs-remaining summary; Session 104: Unity constructed production continuity green with worker-led barracks placement, militia training from the newly completed building, runtime-smoke validation, scene validation, and Node checks; Session 103: Unity first governed construction slice green with worker-led building placement, construction progression, dwelling completion, population-cap verification, scene validation, and Node checks; Session 102: Unity two-deep production queue tail-cancel validation green with governed rear-entry refund verification, surviving front-entry completion, scene validation, and Node checks; Session 101: Unity production queue cancel-and-refund slice green with queue-row UI, refund-safe payload metadata, governed cancel verification, scene validation, and Node checks; Session 100: Unity first governed production slice green with building selection, command-hall villager training, runtime-smoke validation, scene validation, JSON sync, and Node checks; Session 96: Naval world integration with vessel dispatch, fishing gather, naval combat, fire ship sacrifice, water-spawn, and 7 test assertions; Session 95: Trueborn recognition diplomacy with Rise pressure exemption, dynasty-panel UI, and 10 test assertions; Session 94: Trueborn Rise arc with three-stage escalation, loyalty/legitimacy pressure, challenge tracking, HUD exposure, save/restore, and 10 test assertions; Session 93: Trueborn City neutral-faction foundation with trade relationships, acceptance endorsement, save/restore, and 9 test assertions; Session 92: player-facing dynasty-panel diplomacy UI with propose and break pact action buttons, browser-verified; Session 91: AI non-aggression pact awareness with succession/army/governance pressure triggers and 4 test assertions; Session 90: non-aggression pact diplomacy system with alliance-threshold counterplay, save/restore, dynasty-panel, and 13 test assertions; Session 89: conviction, covenant, and tribal acceptance factors added to Territorial Governance population-acceptance model with dynasty-panel legibility and test coverage; Session 88: governance alliance-threshold coalition pressure stabilized with save/restore, snapshot exposure, dynasty-panel legibility, HUD world-pill exposure, and runtime bridge test coverage; prior unreported Session 89 code documented; continuation platform quality-of-life pass validated with persistent view state, in-view filtering, quick-jump controls, copy actions, toast feedback, and manual anchor clear; Bloodlines continuation platform thin slice running and validated; Unity bootstrap runtime smoke green with command-shell validation, control groups, framing, and battlefield HUD; Unity canonical scene-shell validation coverage added and batch-validated; Unity definition bindings repaired; Bootstrap scene canonical map assignment recovered; Unity drag-box selection shell added; Unity selection and formation-move shell preserved; Unity control-point capture foundation added; Unity scene-scaffold, battlefield camera, and debug shell added; Unity bootstrap and territory-yield foundation added; foundational player guide drafted; graphics lane Batch 08 settlement-variant wave integrated; Unity graphics staging advanced through Batch 08 with browser-first raster fallback; Unity graphics testbeds refreshed through Batch 08 and batch-review governance applied; Unity movement foundation added; non-canonical Unity template under `unity/My project` classified)

## 2026-04-20 Dynasty Marriage Parity

- Branch lane: `codex/unity-dynasty-marriage-parity`
- Dedicated slice handoff:
  - `docs/unity/session-handoffs/2026-04-20-unity-dynasty-marriage-parity.md`
- Canonical browser parity corrections landed on top of the retired Tier 2 dynasty
  foundation instead of re-porting the lane from zero:
  - `MarriageProposalExpirationSystem` now uses the canonical
    `MARRIAGE_PROPOSAL_EXPIRATION_IN_WORLD_DAYS = 90`
  - `MarriageGestationSystem` now uses the canonical
    `MARRIAGE_GESTATION_IN_WORLD_DAYS = 280`
  - gestation now records child provenance through `MarriageChildElement` on the
    marriage plus `DynastyMixedBloodlineComponent` on the spawned child
  - new `MarriageDeathDissolutionSystem` ports `dissolveMarriageFromDeath`,
    marking both marriage records dissolved, applying legitimacy loss, and
    recording oathkeeping mourning effects
- Dedicated validation landed:
  - `BloodlinesMarriageParitySmokeValidation`
  - `scripts/Invoke-BloodlinesUnityMarriageParitySmokeValidation.ps1`
  - proof phases cover pre-expiration pending state, day-90 expiration,
    mixed-bloodline child generation at day 280, and death-driven dissolution
    halting gestation while applying faction-side penalties
- Validation state:
  - `dotnet build unity/Assembly-CSharp.csproj -nologo`
  - `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
  - bootstrap runtime smoke
  - combat smoke
  - canonical scene-shell validation
  - fortification smoke
  - siege smoke
  - `node tests/data-validation.mjs`
  - `node tests/runtime-bridge.mjs`
  - contract staleness check
  - dedicated marriage parity smoke
  - all green in `D:\BLDMP\bloodlines`
  - bootstrap and canonical scene-shell validators again used worktree-local
    wrapper copies because the checked-in wrappers remain path-pinned to
    `D:\ProjectsHome\Bloodlines`

## 2026-04-19 Fortification Siege Sub-Slice 5 Wall Segment Destruction Resolution

- Branch lane: `codex/unity-fortification-wall-segment-destruction`
- Dedicated slice handoff:
  - `docs/unity/session-handoffs/2026-04-19-unity-fortification-siege-wall-segment-destruction-resolution.md`
- Fortification-role buildings now become live fortification contributors in the
  normal runtime instead of only inside isolated tests:
  - `FortificationStructureLinkSystem` links wall, tower, gate, and keep buildings
    to their nearest same-faction settlement inside the fortification ecosystem
    radius
  - the same system materializes `FortificationBuildingContributionComponent` from
    browser-equivalent fortification roles so `AdvanceFortificationTierSystem`
    finally sees real fortification structures in active world state
- Destroyed structural pieces now resolve into explicit breach state:
  - `FortificationDestructionResolutionSystem` writes
    `DestroyedWallSegmentCount`, `DestroyedTowerCount`, `DestroyedGateCount`,
    `DestroyedKeepCount`, and `OpenBreachCount` onto `FortificationComponent`
  - `BloodlinesDebugCommandSurface.Fortification` adds
    `TryDebugGetFortificationBreachState` so the new runtime state is observable
    without widening the root debug shell
- `FortificationReserveSystem` now runs after destruction accounting, so breached
  walls reduce reserve frontage and mustered defenders on the same update chain
  rather than one tick later.
- Dedicated validation landed:
  - `BloodlinesWallSegmentDestructionSmokeValidation`
  - `scripts/Invoke-BloodlinesUnityWallSegmentDestructionSmokeValidation.ps1`
  - proof phases cover linked-wall baseline, destroyed-wall breach creation, intact
    reserve frontage, and breached-wall reserve contraction
- Browser references ported:
  - `simulation.js` structural kill flow inside `applyDamage` (~7694-7755)
  - `nearestSettlementForBuilding` (~11182-11213)
  - `advanceFortificationTier` (~11227-11245)
- Validation state:
  - all 10 required governed gates green on the rebased `origin/master`
    `0a0e122f` base
  - fortification, siege, imminent engagement, and siege supply interdiction
    regressions rerun green on the same rebased branch
  - dedicated wall-segment destruction smoke green with all 4 phases passing
  - bootstrap runtime and canonical scene-shell gates were again executed through
    worktree-local wrappers under the Unity lock because the checked-in wrappers
    remain path-pinned to `D:\ProjectsHome\Bloodlines`
- Contract revision advanced from 30 to 31 for this slice.

## 2026-04-19 AI Strategic Layer Sub-Slice 8 Command Dispatch

- Branch lane: `codex/unity-ai-command-dispatch`
- Dedicated slice handoff:
  - `docs/unity/session-handoffs/2026-04-19-unity-ai-strategic-layer-sub-slice-8-command-dispatch.md`
- The AI strategic layer now issues real orders for two previously inert decision seams:
  - `AIWorkerCommandSystem` converts worker gather decisions into
    `WorkerGatherOrderComponent` plus `MoveCommandComponent` and flips workers from
    `Seeking` to `Gathering`
  - `AITerritoryDispatchSystem` converts expired territory-expansion timers into
    active `MoveCommandComponent` orders for non-worker, non-siege combat units
- `EnemyAIStrategySystem` now does decision-only territory target selection and leaves
  order issuance to the dedicated dispatcher.
- `WorkerGatherSystem` now requires workers to reach node radius before harvesting, so
  strategic dispatch cannot harvest from range.
- Validation for this slice is fully green:
  - `dotnet build unity/Assembly-CSharp.csproj -nologo`
  - `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
  - bootstrap runtime smoke via direct worktree execute-method run
  - `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1`
  - bootstrap + gameplay scene shell validation via direct worktree execute-method runs
  - fortification smoke via direct worktree execute-method run
  - `scripts/Invoke-BloodlinesUnitySiegeSmokeValidation.ps1`
  - `node tests/data-validation.mjs`
  - `node tests/runtime-bridge.mjs`
  - `scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1`
- Branch stop state:
  - green in clean worktree and ready for commit/push
  - should be rebased after Claude's sub-slice 7 build-timer-chain lands on `master`
  - keep the new systems anchored `[UpdateAfter(typeof(AICovertOpsSystem))]` on the
    revision-18 base until a later post-rebase ordering change is intentionally made

## Canonical Root

- Canonical session path: `D:\ProjectsHome\Bloodlines`
- Physical backing path: `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\bloodlines`
- Canonical status: active and authoritative

## Consolidation Status

The project is governed from one canonical root. Relevant outside Bloodlines sources were imported into `archive_preserved_sources/` and `governance/` so future sessions can continue from this folder without reconstructing context from the wider workspace.

An additional external doctrine ingestion was absorbed on 2026-04-14. The canonical root now preserves the original DOCX artifacts, the raw extraction appendix, the readable doctrine source, and the updated curated bible export that integrates that doctrine.

## What Exists In The Canonical Root

- Full numbered design corpus and archive from `00_ADMIN` through `19_ARCHIVE`
- Browser behavioral-spec runtime in `src/`, `data/`, `tests/`, `play.html`
- Unity shipping lane in `unity/` and `docs/unity/`
- Player-facing export lane in `18_EXPORTS/`, now including `18_EXPORTS/BLOODLINES_PLAYER_GUIDE_FOUNDATIONS.md`
- Imported preserved source bundles in `archive_preserved_sources/`
- Imported governance overlays in `governance/`
- Root continuity files and machine-readable state in `continuity/`

## 2026-04-17 Codex Unity Group Movement And Combat Stances

- Branch lane: `codex/unity-group-movement-and-stances`
- Dedicated slice handoff: `docs/unity/session-handoffs/2026-04-17-unity-group-movement-and-stances.md`
- The combat lane now has three additive runtime layers on top of the merged
  combat / projectile / attack-order foundation:
  - group-aware destination preservation through `GroupMovementOrderComponent`
    and `GroupMovementResolutionSystem`
  - soft same-faction overlap resolution through `UnitSeparationComponent`,
    `UnitSeparationSystem`, `RecentImpactComponent`, and
    `RecentImpactRecoverySystem`
  - explicit stance control through `CombatStanceComponent`,
    `CombatStanceRuntimeComponent`, `CombatStanceResolutionSystem`, and
    `CombatUnitRuntimeDefaults`
- Debug command surface extensions landed only through new partials:
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Movement.cs`
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Stances.cs`
- Spawn and production seams now attach stance and separation payloads additively
  through authoring, baker, bootstrap, and production paths without reshuffling
  existing component order.
- `BloodlinesCombatSmokeValidation` now proves eight combat phases green:
  - melee
  - projectile
  - explicit attack
  - attack-move
  - target visibility / sight loss
  - group movement
  - separation
  - stance behavior (hold-position plus retreat-on-low-hp)
- Verified retreat pass line on the rebased branch:
  - `Combat smoke validation retreat sub-phase passed: initialDistance=0.5, finalDistance=7.5, finalRetreatDistance=0.`
- Governed validation green on the rebased branch:
  - `dotnet build unity/Assembly-CSharp.csproj -nologo`
  - `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
  - `scripts/Invoke-BloodlinesUnityWrapperWithLock.ps1 -Session codex-movement -WrapperScript scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityWrapperWithLock.ps1 -Session codex-movement -WrapperScript scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityWrapperWithLock.ps1 -Session codex-movement -WrapperScript scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1`
  - `scripts/Invoke-BloodlinesUnityWrapperWithLock.ps1 -Session codex-movement -WrapperScript scripts/Invoke-BloodlinesUnityGraphicsRuntimeValidation.ps1`
  - `node tests/data-validation.mjs`
  - `node tests/runtime-bridge.mjs`
  - `scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1`
- Operational hardening landed alongside this slice:
  - `unity/Assembly-CSharp.csproj` now explicitly includes the new movement /
    stance / separation runtime files and debug partials so branch-local build
    state matches the actual committed file graph
  - the governed bootstrap-runtime, bootstrap-scene, gameplay-scene, and
    graphics-runtime wrapper scripts now wait for explicit log outcomes instead
    of trusting the first Unity launcher exit code under the shared-worktree
    harness
- Stop state:
  - branch is rebased, green, and should remain pending merge coordination only
  - next combat cleanup after merge should retire
    `AttackOrderComponent.IsAttackMoveActive` in favor of
    `CombatStance.AttackMove` as the sole long-lived attack-move source of truth

## 2026-04-17 Session 125 Unity Attack Orders And Attack-Move

- Branch lane: `codex/unity-attack-orders-attack-move`
- Dedicated slice handoff: `docs/unity/session-handoffs/2026-04-17-unity-attack-orders-and-attack-move.md`
- Explicit combat order payload and runtime now exist:
  - `AttackOrderComponent`
  - `AttackOrderSystem`
- `AutoAcquireTargetSystem` now prefers a live in-sight explicit hostile target from `AttackOrderComponent` before falling back to passive nearest-hostile acquire.
- `DeathResolutionSystem` now strips `AttackOrderComponent` from dying units so no residual order state survives death cleanup.
- A new debug-only combat partial now exists at:
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.AttackOrders.cs`
- That partial adds the first actual combat command bindings:
  - right-click hostile with selected combat units issues an explicit attack order
  - `A` enters attack-move cursor mode
  - the next ground right-click becomes the stored attack-move destination
  - `Esc` cancels attack-move mode and restores the pre-mode selection snapshot
- `BloodlinesCombatSmokeValidation` now has three green phases:
  - melee instant-hit proof
  - projectile proof
  - explicit attack-order proof through the debug API with cooldown fire, target death, and residual `AttackTargetComponent` cleanup all asserted
- Clean-worktree validation is green for this slice:
  - `dotnet build unity/Assembly-CSharp.csproj -nologo`
  - `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
  - `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1`
  - clean-worktree bootstrap runtime smoke via `BloodlinesBootstrapRuntimeSmokeValidation.RunBatchBootstrapRuntimeSmokeValidation`
  - clean-worktree bootstrap scene validation via `BloodlinesGameplaySceneBootstrap.RunBatchValidateBootstrapSceneShell`
  - clean-worktree gameplay scene validation via `BloodlinesGameplaySceneBootstrap.RunBatchValidateGameplaySceneShell`
  - `node tests/data-validation.mjs`
  - `node tests/runtime-bridge.mjs`
- Bootstrap runtime smoke remained green with the economy proof fields still present:
  - `gatherDepositObserved=True`
  - `trickleGainObserved=True`
  - `starvationObserved=True`
  - `loyaltyDeclineObserved=True`
  - `capPressureObserved=True`
- Validation-path note:
  - the checked-in bootstrap runtime smoke and canonical scene-shell wrappers are still pinned to `D:\ProjectsHome\Bloodlines`
  - because the canonical checkout remained dirty under the concurrent Claude lane, the same execute methods were rerun safely in the clean `D:\ao\bloodlines` worktree through clone-local temporary wrapper scripts under `scripts/Invoke-BloodlinesUnityWrapperWithLock.ps1`
- This branch should now stop at merge-coordination state. The next combat follow-up after merge is acquisition throttling, line-of-sight tuning, and deeper combat feel polish rather than widening this branch further.

## 2026-04-17 Session 120 Unity Projectile Combat

- Branch lane: `codex/unity-projectile-combat`
- Dedicated slice handoff: `docs/unity/session-handoffs/2026-04-17-unity-projectile-combat.md`
- The combat-foundation base is now merged into `master` at `a8dd553`, and this follow-up slice adds the first real ranged projectile lifecycle on top of it.
- New combat payloads and systems now exist:
  - `ProjectileComponent`
  - `ProjectileFactoryComponent`
  - `ProjectileMovementSystem`
  - `ProjectileImpactSystem`
- Canonical attack semantics are now split correctly:
  - melee paths remain instant-hit
  - `UnitRole.Ranged` attacks now spawn projectile entities
  - projectile-capable siege classes now use the same projectile-delivery path through runtime spawn wiring
- Bootstrap authoring, baking, runtime bootstrap spawning, and produced-unit spawning now all carry projectile speed, lifetime, and arrival-radius payloads.
- Debug presentation now shows live projectile proxy spheres so projectile motion is visible in Play Mode without adding any permanent art presentation.
- The dedicated combat smoke validator now has two green phases:
  - melee instant-hit proof remains green
  - bowman-vs-villager projectile proof now confirms an in-flight projectile exists before impact and then confirms the target dies on arrival
- Shared-file compatibility note:
  - `SkirmishBootstrapSystem` now attaches the Claude-lane AI economy controller reflectively when that component exists
  - this preserves buildability on current `master`, which references that AI seam before the AI component file itself is merged
- Clean-worktree validation is green for this slice:
  - `dotnet build unity/Assembly-CSharp.csproj -nologo`
  - `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
  - `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1`
  - clean-worktree bootstrap runtime smoke via `BloodlinesBootstrapRuntimeSmokeValidation.RunBatchBootstrapRuntimeSmokeValidation`
  - clean-worktree bootstrap scene validation via `BloodlinesGameplaySceneBootstrap.RunBatchValidateBootstrapSceneShell`
  - clean-worktree gameplay scene validation via `BloodlinesGameplaySceneBootstrap.RunBatchValidateGameplaySceneShell`
  - `node tests/data-validation.mjs`
  - `node tests/runtime-bridge.mjs`
- Bootstrap runtime smoke remained green with the economy proof fields still present:
  - `gatherDepositObserved=True`
  - `trickleGainObserved=True`
  - `starvationObserved=True`
  - `loyaltyDeclineObserved=True`
  - `capPressureObserved=True`
- This branch should now stop at merge-coordination state. The next combat follow-up after merge is explicit attack orders or attack-move, not more mid-slice expansion.

## 2026-04-17 Session 119 Unity Combat Foundation

- Branch lane: `codex/unity-combat-foundation`
- Dedicated slice handoff: `docs/unity/session-handoffs/2026-04-17-unity-combat-foundation.md`
- First real combat runtime now exists in Unity:
  - `CombatStatsComponent`
  - `AttackTargetComponent`
  - `HostilityComponent`
  - `AutoAcquireTargetSystem`
  - `AttackResolutionSystem`
  - `DeathResolutionSystem`
- Bootstrap authoring, baking, skirmish bootstrap, and produced-unit spawning now carry combat stats into live ECS units.
- `JsonContentImporter.cs` already preserved `attackDamage`, `attackRange`, `attackCooldown`, and `sight`, so no shared importer rewrite was required in this slice.
- Dedicated governed combat proof now exists at:
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesCombatSmokeValidation.cs`
  - `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1`
- Critical integration fix in this slice:
  - browser-spec `attackRange` and `sight` values are authored in pixel-scale terms
  - Unity battlefield units spawn in map tile/world coordinates
  - the authoring and baker seam now normalizes those combat distances by `map.tileSize` before storing runtime combat values
  - this keeps auto-acquire real without letting units aggro across most of `ironmark_frontier`
- Validation green for this branch:
  - `dotnet build unity/Assembly-CSharp.csproj -nologo`
  - `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
  - `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1`
  - worktree-equivalent batch run of `BloodlinesBootstrapRuntimeSmokeValidation.RunBatchBootstrapRuntimeSmokeValidation`
  - unchanged governed wrapper `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1`
  - `node tests/data-validation.mjs`
  - `node tests/runtime-bridge.mjs`
- Current combat-foundation proof line:
  - `Combat smoke validation passed: dead='enemy', survivorHealth=6/12, elapsedSeconds=1.2.`
- Session was originally numbered Session 113 on branch; renumbered Session 119 during the master merge to avoid collision with continuation-platform Session 113.

## 2026-04-17 Session 113 Continuation Platform Governed Local Command Runner

- `http://127.0.0.1:8067` now supports a real governed local execution loop on the main `Command Deck`, not only chat, retrieval, resume, and draft-write behavior.
- `continuation-platform/lib/core.py` now carries:
  - a command-runner policy loader with a safe built-in fallback
  - `/run <command>` handling for the conversation thread
  - policy-backed classification between read-only inspection and tier-gated validation or build work
  - blocked destructive filesystem commands, blocked direct file mutation, blocked git mutation, blocked package installation, and blocked networked commands
  - model-tool access to the same governed runner for natural-language turns that need real local validation
  - assistant-side handling for governed command refusals so the thread stays conversational instead of collapsing into transport errors
- New command-runner policy file now exists at `continuation-platform/config/command_runner_policy.json`.
- The Command Deck GUI now exposes local-runner policy details and ready-to-use governed command examples in the right rail, keeping the main screen centered on a Codex/Claude-style conversation workflow.
- `continuation-platform/tests/smoke_test.py` now proves:
  - governed read-only `/run` success
  - locked validation-command refusal
  - destructive-command blocking
- Live localhost validation is now green for:
  - `GET /api/agent-console`
  - `POST /api/agent-console/message` with `/help`
  - `POST /api/agent-console/message` with governed `/run Get-ChildItem continuation-platform\\config | Select-Object -ExpandProperty Name`
- Practical result: the offline Bloodlines continuation surface can now inspect files, query git state, run syntax checks, and run approved local validation commands from the same back-and-forth interface while still forcing canonical file edits through the governed draft and apply path.

## 2026-04-17 Session 114 Continuation Platform Governed Task Loop

- The continuation platform moved beyond single-turn `/run` and now carries a first governed multi-step task loop directly inside the main `Command Deck`.
- Five new additions are now live together:
  - persistent execution plans stored under `continuation-platform/state/agent_console_plans.json`
  - a plan command set: `/plan`, `/plans`, `/show-plan`, `/run-plan`, `/archive-plan`
  - sequential plan-step execution across governed actions such as `run_command`, `read_file`, `search_context`, `get_execution_packet`, `resume`, and `rescan`
  - persistent recent-run history stored under `continuation-platform/state/agent_console_runs.json`
  - local-model support for returning `execution_plan` mode from natural-language turns so the thread can stage a governed workflow instead of only answering or drafting writes
- `continuation-platform/lib/core.py` now carries the persistent plan and run state, the plan builder, safe fallback planning heuristics, plan execution, plan archiving, recent-run recording, and model-plan integration.
- `continuation-platform/server.py` now exposes:
  - `POST /api/agent-console/run-plan`
  - `POST /api/agent-console/archive-plan`
- `continuation-platform/static/index.html` and `static/app.js` now add Command Deck right-rail surfaces for:
  - active execution plans
  - recent governed runs
  - direct `Run Plan` and `Archive` actions
- `continuation-platform/tests/smoke_test.py` now proves:
  - governed plan creation
  - `/plans`
  - `/show-plan`
  - `/run-plan`
  - recent-run persistence after plan execution
  - `/archive-plan`
- Practical result: the offline Bloodlines continuation surface can now hold a multi-step local workflow across refreshes, run it under the same policy regime as `/run`, and keep execution history visible without leaving the main back-and-forth agent screen.

## 2026-04-16 Session 111 Finalization Handoff And Continuation Prompt Refresh

- A new current ready-to-paste continuation prompt now exists at `03_PROMPTS/CONTINUATION_PROMPT_2026-04-16_SESSION_111.md`.
- A new active finalization ladder now exists at `docs/plans/2026-04-16-bloodlines-finalization-execution-checklist.md`.
- The new prompt replaces the old Session 104 prompt as the active re-entry surface for future sessions.
- It is aligned to the real current state:
  - continuation-platform `Command Deck` first
  - `Execution` view second
  - Unity `Bootstrap.unity` manual verification next
  - then direct continuation into the next unblocked Unity finalization slices without stopping at another report-only checkpoint
- Root continuity now points at the new prompt so future sessions do not resume from the obsolete Session 104 continuity slice, and the new checklist gives those sessions an execution-grade definition of what still remains before honest finalization.

## 2026-04-16 Session 110 Continuation Platform Command Deck

- `http://127.0.0.1:8067` is no longer just a dashboard-first continuity viewer. It now opens on a chat-first `Command Deck` intended to act as the offline Bloodlines continuation surface.
- `continuation-platform/lib/core.py` now carries a persistent local console session, governed slash-command handling, local-model conversational turns, local tool-use budget, citation carry-through, governed write-draft staging, and one-click draft apply or dismiss flows.
- New live API surface now includes:
  - `GET /api/agent-console`
  - `POST /api/agent-console/message`
  - `POST /api/agent-console/reset`
  - `POST /api/agent-console/apply-draft`
  - `POST /api/agent-console/dismiss-draft`
- New local command loop now supports:
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
- `continuation-platform/static/index.html`, `static/app.js`, and `static/styles.css` now center the GUI around the conversation thread, composer, citations, suggested prompts, and governed draft rail while preserving the older execution, diff, telemetry, and handoff surfaces behind it.
- Natural-language turns now route through the local Ollama inventory and can answer Bloodlines continuity questions even when the output needs to fall back to a grounded continuity summary because the local model returns structurally weak JSON.
- The governed write loop is now available from both the explicit workbench and the conversation thread. Draft applies still use the same tier gate, stale-source protection, backup capture, and post-apply rescan behavior as the workbench path.
- Governed validation for this command-deck pass is green:
  - `python -B -m py_compile continuation-platform/lib/core.py continuation-platform/server.py continuation-platform/tests/smoke_test.py`
  - `node --check continuation-platform/static/app.js`
  - `python continuation-platform/tests/smoke_test.py`
  - live HTTP validation for `/`, `/api/agent-console`, and a natural-language `POST /api/agent-console/message`

## 2026-04-16 Session 109 Continuation Platform Execution Packet And Governed Write Workbench

- The continuation platform crossed from continuity-browser utility into a stronger production carry system for the Unity shipping lane.
- `continuation-platform/lib/core.py` now augments the old static canonical subset with current live sources:
  - owner direction
  - latest Unity handoff
  - latest project-gap report
  - latest continuation prompt
  - continuation-platform README
- The platform now generates `continuation-platform/state/execution_packet.json`, which carries the current Unity shipping-lane packet:
  - execution lane
  - recommended next step
  - project-work priority list
  - current verified Unity state
  - manual verification checklist
  - governed validation commands
  - canonical source spine
  - governed write targets
- `continuation-platform/static/index.html`, `static/app.js`, and `static/styles.css` now expose a real `Execution` view plus a governed write workbench that can load a project file, preview a diff, and apply a real canonical write after unlock.
- `continuation-platform/server.py` now exposes:
  - `GET /api/execution-packet`
  - `POST /api/project-file`
  - `POST /api/project-write/preview`
  - upgraded `POST /api/project-write` with stale-source hash protection
- `continuation-platform/tests/smoke_test.py` now proves:
  - execution-packet generation
  - project-file load
  - write preview
  - locked refusal
  - successful unlocked tier-3 project write into the canonical Bloodlines root under `test-results/`
- Governed validation:
  - `python -m compileall D:\ProjectsHome\Bloodlines\continuation-platform` passed
  - `node --check D:\ProjectsHome\Bloodlines\continuation-platform\static\app.js` passed
  - `python D:\ProjectsHome\Bloodlines\continuation-platform\tests\smoke_test.py` passed
  - live HTTP validation passed for `GET /api/bootstrap`, `GET /api/execution-packet`, `POST /api/project-file`, and `POST /api/project-write/preview`
- Latest validated continuation-platform state after this pass:
  - files mapped in governed source scope: `3182`
  - canonical subset documents ingested: `19`
  - discovered registry documents: `903`
  - conflict clusters: `158`
  - frontier artifacts: `22`
  - open tasks parsed: `10`
- Detailed report preserved at `reports/2026-04-16_continuation_platform_execution_packet_and_governed_write_workbench.md`.

## 2026-04-17 Session 115 Unity Building Resource Trickle + Concurrent Session Contract

- The Unity first shell now ports the canonical `tickPassiveResources` loop to ECS. Farms and wells and any future `resourceTrickle`-carrying building raise their owning faction's `ResourceStockpileComponent` by `rate * dt` on every simulation tick so long as they are alive and not under construction.
- `unity/Assets/_Bloodlines/Code/Definitions/BuildingDefinition.cs` and `unity/Assets/_Bloodlines/Code/Editor/JsonContentImporter.cs` corrected: `resourceTrickle` is now `ResourceTrickleFields` (float) instead of `ResourceAmountFields` (int). Canonical `farm.food = 0.5` and `well.water = 0.45` now survive round-trip through ScriptableObject sync; previously truncated to `0`.
- `unity/Assets/_Bloodlines/Code/Economy/ResourceTrickleBuildingComponent.cs` and `unity/Assets/_Bloodlines/Code/Economy/ResourceTrickleBuildingSystem.cs` are new. The system runs every tick before `PopulationGrowthSystem`, accumulates per-faction deltas into a `NativeParallelHashMap`, then applies them to each faction's `ResourceStockpileComponent` in a second pass to avoid structural-change hazards during iteration.
- Authoring, baker, skirmish bootstrap, and the runtime-construction path in `BloodlinesDebugCommandSurface.cs` now all attach the trickle component on spawn / completion when the source building definition carries any positive trickle rate.
- Governed validation extended: `BloodlinesBootstrapRuntimeSmokeValidation.cs` now snapshots controlled-faction food and water just before the gather probe (`ProbeResourceTrickleBaseline`), and after gather completes asserts both food and water rose by at least the configured minimum within the configured timeout (`ProbeResourceTrickleGain`). Final success diagnostics carries `trickleInitialFood`, `trickleLatestFood`, `trickleInitialWater`, `trickleLatestWater`, and `trickleGainObserved`.
- Governed validation:
  - `dotnet build unity/Assembly-CSharp.csproj -nologo` passed with 0 errors
  - `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` passed with 0 errors and 110 pre-existing warnings
  - `scripts/Invoke-BloodlinesUnitySyncJsonContent.ps1` passed (via wrapper lock). Farm and well assets now verified at `resourceTrickle.food: 0.5` and `resourceTrickle.water: 0.45`.
  - `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` passed (via wrapper lock). Success line ends with `trickleInitialFood=39.70`, `trickleLatestFood=43.77`, `trickleInitialWater=37.13`, `trickleLatestWater=40.69`, `trickleGainObserved=True` alongside preserved Session 112 gather proof and prior construction + production progress fields.
  - `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` passed.
  - `node tests/data-validation.mjs` passed.
  - `node tests/runtime-bridge.mjs` passed.
- First concurrent-session infrastructure landed: `docs/unity/CONCURRENT_SESSION_CONTRACT.md` defines lane ownership, file scope, wrapper-lock protocol, branch and merge discipline. `scripts/Invoke-BloodlinesUnityWrapperWithLock.ps1` enforces `.unity-wrapper-lock` serialization so Claude (economy) and Codex (combat) Unity sessions never collide on the project lock.
- Claude working on branch `claude/unity-food-water-economy`. Codex to work on `codex/unity-combat-foundation`.
- Detailed handoff preserved at `docs/unity/session-handoffs/2026-04-17-unity-building-resource-trickle-and-concurrent-session-contract.md`.

## 2026-04-17 Session 112 Unity Worker Gather-Deposit Primary Economy Loop

- The Unity first shell now carries the canonical RTS primary economy loop end to end.
- `unity/Assets/_Bloodlines/Code/Components/WorkerGatherComponent.cs` is new: adds `WorkerGatherPhase` (Idle, Seeking, Gathering, Returning, Depositing) and a `WorkerGatherComponent` storing assignment and carry state aligned with the browser runtime's gatherer shape.
- `unity/Assets/_Bloodlines/Code/Systems/WorkerGatherSystem.cs` is new: advances every controlled worker each simulation tick through Seeking -> Gathering -> Returning -> Depositing, moves via `MoveCommandComponent`, decrements the resource node's canonical amount, targets the nearest alive completed owned `command_hall` for drop-off, and deposits into all seven canonical ResourceStockpileComponent fields (gold, wood, stone, iron, food, water, influence).
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.cs` now exposes `TryDebugAssignSelectedWorkersToGatherResource`, `TryDebugGetFactionStockpile`, and `GetControlledWorkersWithActiveGatherCount` debug APIs so governed validators and Play Mode testing can exercise the loop without duplicating entity queries.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs` now runs a `ProbeWorkerGatherCycle` phase after the constructed `barracks -> militia` completion, records initial and latest faction gold, asserts a deposit delta above the configured minimum within a bounded cycle timeout, and carries `gatherResource`, `gatherAssigned`, `gatherAssignedWorkerCount`, `gatherInitialFactionGold`, `gatherLatestFactionGold`, and `gatherDepositObserved` into the final success line. `StartupTimeoutSeconds` raised from 75 to 120 to accommodate the longer combined sequence.
- Governed validation:
  - `dotnet build unity/Assembly-CSharp.csproj -nologo` passed with 0 warnings and 0 errors
  - `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` passed with 0 errors and 110 pre-existing warnings
  - `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` passed. Success line ends with `gatherResource='gold'`, `gatherAssigned=True`, `gatherAssignedWorkerCount=5`, `gatherInitialFactionGold=45.0`, `gatherLatestFactionGold=55.0`, `gatherDepositObserved=True` alongside preserved construction and production progress fields and the prior `buildings=11`, `units=18`, `controlledUnits=8`, `populationCap=24` totals.
  - `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` passed.
  - `node tests/data-validation.mjs` passed.
  - `node tests/runtime-bridge.mjs` passed.
- First public git push completed. Sessions 100-108, full canonical corpus, Unity source, ScriptableObject data, and continuation-platform source now live at origin `lance-fisher/lancewfisher-v2` master (commits `a72982c`, `087d7b0`, `f3dd374`, `de003c9`; Session 112 commit forthcoming).
- Detailed handoff preserved at `docs/unity/session-handoffs/2026-04-16-unity-worker-gather-deposit-primary-loop.md`.

## 2026-04-16 Session 108 Unity Production Progress Observability And World-Space Bar

- The Unity first-shell production lane now carries mid-production observability that mirrors the Session 107 construction-lane pattern.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugEntityPresentationBridge.cs` now renders a world-space production progress bar above every building with an active production queue, implemented via a pooled `ProductionProgressProxy` map with its own stale-proxy cleanup and a distinct blue fill that reads separately from the existing gold construction bar.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.cs` now exposes `TryDebugGetSelectedProductionProgress` so governed validators can sample the selected building's live queue head progress without duplicating entity queries.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs` now proves mid-production queue progress through a new `ProbeProductionProgress` phase between cancel-and-refund verification and the final spawn check, with initial and latest ratio recording, advancement assertion, and a `midProductionObservationWindow` bypass for the pre-existing spawn-floor and strict phase-equality gates so those gates no longer block mid-production observation while still strictly validating all other bootstrap counts.
- Governed validation:
  - `dotnet build unity/Assembly-CSharp.csproj -nologo` passed with 0 warnings and 0 errors
  - `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` passed with 0 errors and 110 pre-existing warnings
  - `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` passed. The success line now ends with `productionProgressInitialRatio=0.004`, `productionProgressLatestRatio=0.084`, `productionProgressAdvancementVerified=True`, `constructionProgressInitialRatio=0.001`, `constructionProgressLatestRatio=0.915`, and `constructionProgressAdvancementVerified=True` alongside the prior `11` total buildings, `18` total units, `8` controlled units, `populationCap=24`, and `constructedProductionUnitType='militia'` state.
  - `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` passed.
  - `node tests/data-validation.mjs` passed.
  - `node tests/runtime-bridge.mjs` passed.
- Detailed handoff preserved at `docs/unity/session-handoffs/2026-04-16-unity-production-progress-observability-and-world-space-bar.md`.

## 2026-04-16 Session 107 Unity Construction Progress Observability And UI

- The Unity first-shell construction lane gained mid-construction observability in the HUD, in the world, and through a governed debug API.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.cs` now renders a selection-aware construction-progress panel with percent complete, remaining and total seconds, a horizontal progress bar, and an integrity readout when a single under-construction controlled building is selected. Two new debug API entry points (`TryDebugSelectControlledConstructionSite`, `TryDebugGetSelectedConstructionProgress`) now expose mid-construction data to governed validators without duplicating entity queries.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugEntityPresentationBridge.cs` now carries a world-space construction progress bar above every under-construction building proxy via a dedicated `ConstructionProgressProxy` pool with its own stale-proxy cleanup.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs` now proves mid-construction progress through a new `ProbeConstructionProgress` phase between placement and completion, recording initial and latest ratios, asserting ratio in `[0, 1]`, asserting total seconds positive, asserting the observed building type matches expected, and asserting a minimum advancement ratio within a configurable wait window.
- Governed validation:
  - `dotnet build unity/Assembly-CSharp.csproj -nologo` passed with 0 warnings and 0 errors
  - `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` passed with 0 errors and 110 pre-existing warnings
  - `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` passed. The success line now ends with `constructionProgressInitialRatio=0.002`, `constructionProgressLatestRatio=0.916`, and `constructionProgressAdvancementVerified=True` alongside the prior `11` total buildings, `18` total units, `8` controlled units, `populationCap=24`, and `constructedProductionUnitType='militia'` state.
  - `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` passed.
  - `node tests/data-validation.mjs` passed.
  - `node tests/runtime-bridge.mjs` passed.
- Detailed handoff preserved at `docs/unity/session-handoffs/2026-04-16-unity-construction-progress-observability-and-ui.md`.

## 2026-04-16 Session 106 Owner Direction Refresh

- Lance Fisher issued a new non-negotiable direction that supersedes any older MVP, phased-release, descoping, or reduced-scope planning.
- The active direction is preserved at:
  - `governance/OWNER_DIRECTION_2026-04-16_FULL_CANON_UNITY.md`
- Current governing implementation posture:
  - ship the full canonical design-bible realization
  - use Unity 6.3 LTS with DOTS / ECS as the shipping engine
  - keep `unity/` as the only active Unity work target
  - treat the browser runtime as frozen behavioral specification only
  - treat all older browser-follow-up, MVP, or phased-delivery guidance as historical and superseded
- Full commercial polish remains in scope across art, audio, UX, AI depth, and multiplayer unless Lance changes that direction explicitly later.

## 2026-04-16 Session 105 Handoff Refresh

- The canonical handoff was refreshed so the next session can see one current completed-vs-remaining map instead of reconstructing overall project status from multiple session slices.
- A dated supporting report now exists at `reports/2026-04-16_project_completion_handoff_and_gap_summary.md`.
- As of this refresh:
  - the continuation platform is ready enough for daily offline continuity use
  - the browser reference simulation is materially deep and remains the authoritative behavioral and feel reference for Unity parity work
  - the Unity lane is green through constructed `barracks -> militia` continuity, but is still not a full gameplay runtime
  - graphics is staged through Batch 08 but still awaits formal review outcomes and later production-asset follow-through
- Bloodlines should still be treated as an in-progress full-canon build, not a finished game. The main remaining work is Unity gameplay expansion toward the complete design bible together with graphics approval and production, audio and UX realization, multiplayer realization, and broader ship-readiness work.

## 2026-04-16 Continuation Platform Quality-Of-Life Update

- The local continuation cockpit at `D:\ProjectsHome\Bloodlines\continuation-platform` kept its product-ready scope and added a daily-use ergonomics pass instead of changing architecture or governance posture.
- New operator-speed improvements now live in the GUI:
  - last active view persists across refreshes and relaunches
  - active-view filtering narrows list, table-row, detail, workspace, and timeline surfaces in place
  - quick-jump controls now move directly to Resume Anchor, Next Work, High Signal, Handoff Prompt, and Telemetry
  - direct copy actions now exist for the recommended next step plus the handoff prompt and handoff preview
  - manual resume-anchor overrides can now be cleared from the Dashboard without restarting the platform
  - rescan, resume, unlock, export, and copy flows now report through in-app toast feedback instead of blocking browser alerts
- Latest validated QoL-pass platform scan state:
  - files mapped in governed source scope: `3176`
  - canonical subset docs ingested: `14`
  - frontier artifacts detected: `22`
  - discovered registry documents: `894`
  - discovered conflict clusters: `158`
  - diff-watch conflict clusters: `23`
  - high-signal changed documents: `24`
  - open tasks parsed into task board: `13`
  - latest scan duration: `14.136s`
- Latest validated resume anchor after the QoL pass:
  - `manual_edit`
  - source path: `continuity/PROJECT_STATE.json`
- Validation remained green after the QoL pass:
  - `python -m compileall D:\ProjectsHome\Bloodlines\continuation-platform`
  - `node --check D:\ProjectsHome\Bloodlines\continuation-platform\static\app.js`
  - `python D:\ProjectsHome\Bloodlines\continuation-platform\tests\smoke_test.py`

## 2026-04-16 Continuation Platform Product-Ready Update

- The offline Bloodlines continuation platform at `D:\ProjectsHome\Bloodlines\continuation-platform` is now operating as a product-ready local continuation cockpit rather than only a thin vertical slice.
- The Windows-first launch path remains `D:\ProjectsHome\Bloodlines\continuation-platform\launch_windows.cmd`, serving the local GUI at `http://127.0.0.1:8067`.
- The GUI now exposes the full intended operator surface set at a baseline level:
  - Dashboard
  - Agent Workspace
  - Tasks
  - Memory
  - Diff Panel
  - Timeline
  - Handoff Builder
  - Telemetry
  - Config
- The runtime now includes:
  - governed Bloodlines-only scan scope with Unity cache and temp noise pruned out of continuity visibility
  - authority-scored canonical registry plus broader discovered registry
  - generated change report with high-signal changes, authoritative updates, and live conflict watch
  - richer telemetry drilldowns for task execution, retrieval provenance, write-gate outcomes, degraded modes, and scan history
  - a fuller handoff builder that packages continuity delta, open work, doctrine, canonical sources, and a frontier re-entry prompt
  - explicit resume-anchor selection when multiple recent candidates compete
  - locked-by-default write posture with refusal telemetry still enforced
- Product-ready baseline scan state before the QoL pass:
  - files mapped in governed source scope: `3179`
  - canonical subset docs ingested: `14`
  - discovered registry documents: `900`
  - discovered conflict clusters: `161`
  - diff-watch conflict clusters: `25`
  - current high-signal changed documents in generated change report: `21`
  - latest scan duration: `12.592s`
- Validation remains green for the product-ready pass:
  - `python -m compileall D:\ProjectsHome\Bloodlines\continuation-platform`
  - `node --check D:\ProjectsHome\Bloodlines\continuation-platform\static\app.js`
  - `python D:\ProjectsHome\Bloodlines\continuation-platform\tests\smoke_test.py`
  - live HTTP validation for `/api/bootstrap`, `/api/change-report`, `/api/telemetry-drilldown`, `/api/handoff-builder`, `/api/select-anchor`, `/api/agent/resume`, and locked `/api/project-write`
- The latest validated continuation recommendation from the local platform remains the Unity Bootstrap feel-verification lane:
  - `1. In Unity 6.3 LTS, open unity/Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity.`

## 2026-04-16 Session 100 Unity First Production Slice Update

- The Unity lane now has its first governed building-production slice instead of only movement and command-shell proof.
- `unity/Assets/_Bloodlines/Code/Definitions/UnitDefinition.cs` and `unity/Assets/_Bloodlines/Code/Editor/JsonContentImporter.cs` now preserve the canonical JSON production-gating fields the Unity lane needs:
  - `movementDomain`
  - `faithId`
  - `doctrinePath`
  - `ironmarkBloodPrice`
  - `bloodProductionLoadDelta`
- Added the first production runtime state in `unity/Assets/_Bloodlines/Code/Components/`:
  - `FactionHouseComponent.cs`
  - `ProductionComponents.cs`
- `SkirmishBootstrapSystem.cs` now seeds faction house identity and per-building production buffers for the first live training lane.
- `BloodlinesDebugCommandSurface.cs` now supports controlled-building selection, a production panel, gate evaluation, queue issuance, and runtime-smoke helpers for the first training seam.
- `BloodlinesDebugEntityPresentationBridge.cs` now highlights selected buildings as well as selected units in the first debug shell.
- `UnitProductionSystem.cs` is now live as the first ECS production resolver. It uses `EndSimulationEntityCommandBufferSystem` so trained units spawn without structural-change violations during iteration.
- `BloodlinesBootstrapRuntimeSmokeValidation.cs` now validates the first production seam end to end:
  - select a controlled `command_hall`
  - queue `villager`
  - wait for spawned unit-count increase
  - verify queue drain
  - verify controlled-unit growth
  - re-verify select-all after production
- Governing verification for this slice is green:
  - `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1`
  - `scripts/Invoke-BloodlinesUnitySyncJsonContent.ps1`
  - `node tests/data-validation.mjs`
  - `node tests/runtime-bridge.mjs`
- Latest governed runtime-smoke pass now proves:
  - factions `3`
  - buildings `9`
  - units `17`
  - resource nodes `13`
  - control points `4`
  - settlements `2`
  - controlled units `7`
  - control group 2 count `7`
  - successful `command_hall -> villager` production
- Detailed handoff for this slice: `docs/unity/session-handoffs/2026-04-16-unity-first-production-slice.md`.

## 2026-04-16 Session 104 Unity Constructed Production Continuity Update

- The Unity first-shell lane now proves that construction and production connect cleanly instead of stopping at `dwelling` completion.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs` now extends the governed Bootstrap runtime-smoke lane through:
  - worker-led `barracks` placement after the existing `dwelling` proof
  - barracks construction completion
  - controlled `barracks` selection after completion
  - `militia` queue issuance from the newly completed building
  - queue drain and final spawned-unit verification from that constructed production seat
- The validator was hardened for the longer governed sequence by:
  - lifting the batch timeout from `45s` to `75s`
  - fixing the older `command_hall -> villager` exact-count checkpoint so the later `barracks -> militia` phase no longer triggers a false mismatch
- Governing verification for this deeper slice is green:
  - `dotnet build unity/Assembly-CSharp.csproj -nologo`
  - `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
  - `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1`
  - `node tests/data-validation.mjs`
  - `node tests/runtime-bridge.mjs`
- The latest governed runtime-smoke pass now proves:
  - factions `3`
  - buildings `11`
  - units `18`
  - resource nodes `13`
  - control points `4`
  - settlements `2`
  - controlled units `8`
  - `command_hall -> villager` two-deep queue issue, rear-entry cancel, refund, and surviving front-entry completion
  - `dwelling` construction completion with `populationCap=24`
  - `barracks` construction completion plus `militia` training from the newly completed building
- Detailed handoff for this slice: `docs/unity/session-handoffs/2026-04-16-unity-constructed-barracks-production-continuity.md`.

## 2026-04-16 Session 103 Unity First Construction Slice Update

- The Unity first-shell lane now has its first governed construction slice instead of stopping at production and command proof.
- Added first construction runtime state in:
  - `unity/Assets/_Bloodlines/Code/Components/ConstructionComponents.cs`
  - `unity/Assets/_Bloodlines/Code/Systems/ConstructionSystem.cs`
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.cs` now supports:
  - worker-aware construction panel visibility
  - pending build-placement mode
  - right-click placement for supported structures
  - placement obstruction feedback
  - a debug construction hook for governed runtime smoke
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugEntityPresentationBridge.cs` now gives under-construction buildings a distinct provisional visual treatment, and `unity/Assets/_Bloodlines/Code/Systems/UnitProductionSystem.cs` now excludes under-construction buildings from active production.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs` now proves the first construction seam after the existing two-deep production proof:
  - worker-led `dwelling` placement
  - total building-count increase from `9` to `10`
  - construction-site completion
  - population-cap increase from `18` to `24`
- The compile surface was kept green additively by:
  - adding `ConstructionComponents.cs.meta`
  - adding `ConstructionSystem.cs.meta`
  - syncing the current generated `unity/Assembly-CSharp.csproj` include list so isolated `dotnet build` sees the two new construction runtime files until the next Unity project regeneration
- Governing verification for this slice is green:
  - `dotnet build unity/Assembly-CSharp.csproj -nologo`
  - `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
  - `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1`
  - `node tests/data-validation.mjs`
  - `node tests/runtime-bridge.mjs`
- The latest governed runtime-smoke pass now proves:
  - factions `3`
  - buildings `10`
  - units `17`
  - resource nodes `13`
  - control points `4`
  - settlements `2`
  - controlled units `7`
  - `command_hall -> villager` two-deep queue issue, rear-entry cancel, refund, and surviving front-entry completion
  - `dwelling` construction completion with `constructionSites=0` and `populationCap=24`
- Detailed handoff for this slice: `docs/unity/session-handoffs/2026-04-16-unity-first-construction-slice.md`.

## 2026-04-16 Session 102 Unity Two-Deep Queue Tail-Cancel Validation Update

- The Unity first production shell now has stronger governed queue-depth proof instead of only single-entry cancel coverage.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs` now proves the first production shell through a stricter sequence:
  - select a controlled `command_hall`
  - queue `villager`
  - queue a second `villager`
  - cancel queued entry index `1`
  - verify the rear-entry refund deltas against the stored queue payload while the surviving front entry remains queued
  - verify queue-depth reduction without canceling the remaining front item
  - allow the surviving front item to complete and re-verify final unit and controlled-unit totals
- Governing verification for the stronger queue-depth slice is green:
  - `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1`
  - `node tests/data-validation.mjs`
  - `node tests/runtime-bridge.mjs`
- The latest governed runtime-smoke pass remains green and now proves deeper queue semantics while still finishing at:
  - factions `3`
  - buildings `9`
  - units `17`
  - resource nodes `13`
  - control points `4`
  - settlements `2`
  - controlled units `7`
- Detailed handoff for this slice: `docs/unity/session-handoffs/2026-04-16-unity-two-deep-production-queue-tail-cancel-validation.md`.

## 2026-04-16 Session 101 Unity Production Queue Cancel-And-Refund Update

- The Unity first production shell now supports queue control instead of queue issuance only.
- `unity/Assets/_Bloodlines/Code/Components/ProductionComponents.cs` now preserves refund-safe queued metadata for the first production slice:
  - queued resource costs
  - queued Ironmark blood price
  - queued blood-production load delta
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.cs` now supports:
  - visible per-entry production queue rows on the controlled-building panel
  - cancel buttons for queued entries
  - resource and population refund when queued production is canceled
  - a debug cancel hook for governed runtime smoke coverage
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs` now proves the production shell in a stricter sequence:
  - select a controlled `command_hall`
  - queue `villager`
  - cancel the queued entry before completion
  - verify queue drain
  - verify queued refund deltas against the stored queue payload
  - re-queue `villager`
  - verify spawned unit-count increase and post-production controlled-unit growth
- Governing verification for the queue-control slice is green:
  - `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1`
  - `node tests/data-validation.mjs`
  - `node tests/runtime-bridge.mjs`
- The latest governed runtime-smoke pass now records `productionCancelVerified=True` in `artifacts/unity-bootstrap-runtime-smoke.log`.
- Detailed handoff for this slice: `docs/unity/session-handoffs/2026-04-16-unity-production-queue-cancel-and-refund.md`.

## 2026-04-16 Session 96 Naval World Integration Update

- Vessels now have a dedicated `updateVessel` tick path dispatched from `updateUnits`.
- Fishing boats auto-gather food when idle on water tiles at `gatherRate` per second.
- War galleys, fire ships, and capital ships resolve naval combat with attack range, damage, and cooldown.
- Fire ships implement `oneUseSacrifice`: detonate on first strike and self-destruct.
- `spawnUnitAtBuilding` now spawns vessels on the nearest water tile adjacent to the producing building.
- `findNearestWaterSpawnPosition` scans the building footprint ring for water tiles.
- Idle combat vessels auto-engage hostile units within 1.2x attack range.
- 7 new runtime bridge test assertions cover fishing gather, movement domain, naval combat, and save/restore.
- Session report: `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-16_SESSION_96.md`.

## 2026-04-16 Session 95 Trueborn Recognition Diplomacy Update

- Kingdoms can now formally recognize the Trueborn City's historical claim through `recognizeTruebornClaim(state, factionId)`.
- Recognition costs 40 influence + 60 gold + 5 legitimacy, grants +6 Trueborn standing, and reduces Rise pressure by 75% on all recognized territories.
- Dynasty panel shows "Recognize Trueborn Claim" action button when the Rise arc is active, with cost, bonus, and pressure reduction details.
- After recognition, the button changes to "Trueborn Claim Recognized" with an informational line.
- `getTruebornRecognitionTerms` validates availability against Rise stage, recognition status, and resources.
- 10 new runtime bridge test assertions cover terms availability, recognition execution, standing increase, legitimacy cost, flag persistence, duplicate rejection, and pressure reduction comparison.
- Session report: `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-16_SESSION_95.md`.

## 2026-04-16 Session 94 Trueborn Rise Arc Update

- The Trueborn Rise arc is now live as the canonical anti-snowball mechanic.
- Three-stage escalation: Claims (passive loyalty pressure), Recognition (active pressure + legitimacy drain), Restoration (maximum pressure).
- Activation requires 8 in-world years of low challenge from kingdoms (unchallenged threshold).
- Challenge level computed from kingdom territory count, Trueborn standing, and direct hostility.
- Stage 2 escalates 2 years after Stage 1; Stage 3 escalates 3 years after Stage 2.
- HUD world pill shows "Trueborn Rise Claims/Recognition/Restoration" when active.
- Snapshot exposes `truebornRiseStage`, `truebornRiseChallengeLevel`, `truebornRiseUnchallengedCycles`, `truebornRiseActivatedAtInWorldDays`.
- Save/restore preserves `riseArc` state on the Trueborn City faction.
- 10 new runtime bridge test assertions cover activation, three-stage escalation, loyalty/legitimacy pressure, snapshot exposure, save/restore, and message output.
- Session report: `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-16_SESSION_94.md`.

## 2026-04-16 Session 93 Trueborn City Neutral-Faction Foundation Update

- The Trueborn City now exists as a live `trueborn_city` kind faction in the simulation.
- Each realm cycle updates the city's trade standing with each kingdom based on conviction, legitimacy, hostility count, and active pacts.
- Standing feeds into the acceptance profile as `truebornEndorsement` (standing * 0.25, clamped [-3, +5]).
- The "World stance" dynasty-panel line now includes "Trueborn +/-N" when endorsement is non-zero.
- Save/restore preserves `tradeRelationships` on the Trueborn City faction.
- 9 new runtime bridge test assertions cover faction creation, standing growth, conviction effects, endorsement exposure, and save/restore.
- Session report: `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-16_SESSION_93.md`.

## 2026-04-16 Session 92 Player-Facing Pact Diplomacy UI Update

- The dynasty panel now exposes interactive action buttons for proposing and breaking non-aggression pacts.
- For each hostile kingdom, a "Propose Pact" button appears with real cost, availability, and minimum-duration information from `getNonAggressionPactTerms`.
- For each active pact, a "Break Pact" button appears with legitimacy and conviction cost warning.
- Buttons are disabled with explanatory text when conditions are not met (insufficient resources, active holy war, existing pact).
- Both buttons use the shared `createActionButton` pattern and call the same simulation functions the AI uses.
- Browser-verified: zero console errors, zero failed requests, Diplomacy section renders correctly.
- Session report: `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-16_SESSION_92.md`.

## 2026-04-16 Session 91 AI Non-Aggression Pact Awareness Update

- Stonehelm AI can now propose non-aggression pacts to the player when under sufficient pressure.
- Proposal triggers: active succession crisis, critically low army (3 or fewer combat units), or the player's Territorial Governance Recognition is established and approaching victory.
- Uses the same shared pact infrastructure from Session 90 (`getNonAggressionPactTerms`, `proposeNonAggressionPact`).
- Timer-based decision: 120-second initial, 90-second retry, 300-second cooldown after success.
- 4 new runtime bridge test assertions for AI pact proposal, record creation, hostility removal, and message-log output.
- Session report: `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-16_SESSION_91.md`.

## 2026-04-16 Session 90 Non-Aggression Pact Diplomacy Update

- Added the first non-aggression pact diplomacy system to Bloodlines, creating the missing diplomatic counterplay against alliance-threshold coalition pressure.
- `proposeNonAggressionPact(state, factionId, targetFactionId)`: Costs 50 influence + 80 gold, removes mutual hostility, creates pact records on both factions, publishes message.
- `breakNonAggressionPact(state, factionId, targetFactionId)`: Restores mutual hostility, costs 8 legitimacy and 2 conviction, publishes message.
- `getNonAggressionPactTerms(state, factionId, targetFactionId)`: Returns availability, cost, and conditions. Blocks on self-pacts, non-kingdoms, non-hostiles, duplicate pacts, and active holy wars.
- Active pacts reduce the hostile-kingdom count in the acceptance profile, which directly eases alliance-threshold acceptance-drag and realm-cycle coalition loyalty/legitimacy erosion.
- Factions now carry `diplomacy.nonAggressionPacts` array state, exported and restored through snapshot round-trip.
- Dynasty panel shows a "Diplomacy" section listing active pacts with target name and remaining minimum-duration days.
- 13 new runtime bridge test assertions cover proposal, hostility removal, cost deduction, duplicate rejection, save/restore, pact breaking, and message log.
- Session report: `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-16_SESSION_90.md`.

## 2026-04-16 Session 89 Conviction, Covenant, and Tribal Acceptance Factors Update

- Added three new interactive factors to the Territorial Governance population-acceptance model:
  - **Conviction modifier**: Apex Moral +4, Moral +2, Cruel -2, Apex Cruel -4. Moral dynasties are recognized by the world; cruel dynasties face rejection.
  - **Covenant endorsement**: Passed Covenant Test +3, Grand Sanctuary +2. Deep faith commitment endorses the dynasty's right to govern.
  - **Tribal friction**: Active tribal raiders impose up to -4 friction. The world's neutral stabilizing forces resist unchecked sovereignty until the frontier is pacified.
- Dynasty panel now shows a "World stance" detail line when any of the three factors are non-zero.
- All three factors propagated through the acceptance profile, governance profile, serialized recognition, and tick refresh.
- Runtime bridge test coverage: 8 new assertions for conviction (moral/cruel target shifts), covenant (endorsement value and flag), and tribal (friction value and target reduction).
- Session report: `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-16_SESSION_89.md`.

## 2026-04-16 Session 88 Alliance-Threshold Coalition Pressure Update

- Stabilized and documented the previously unreported alliance-threshold coalition pressure system (tagged as "Session 89" in code comments) by adding the missing save/restore, snapshot exposure, dynasty-panel legibility, HUD world-pill exposure, and runtime bridge test coverage.
- The alliance-threshold system creates a live counterforce during the 60 to 65% population-acceptance push: above 60% acceptance, each hostile kingdom slows the acceptance rise rate and, on each 90-second realm cycle, erodes the weakest governed march loyalty and drains legitimacy.
- `exportStateSnapshot` and `restoreStateSnapshot` now explicitly preserve `governanceAlliancePressureCycles`, `governanceAlliancePressureActive`, and `governanceAlliancePressureHostileCount`.
- `getRealmConditionSnapshot` now exposes the alliance-pressure state through the `worldPressure` block.
- The dynasty panel now shows a dedicated "Coalition pressure active" line when alliance-threshold pressure is live.
- The world pill in the HUD now shows "coalition N hostile" when governance alliance-threshold pressure is active.
- Runtime bridge test coverage added for: coalition loyalty erosion, legitimacy drain, active/cycle-counter state, snapshot exposure, save/restore round-trip, and below-threshold non-activation.
- Unity C# compilation verified: `dotnet build unity/Assembly-CSharp.csproj -nologo` passed with 0 warnings and 0 errors.
- Session report: `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-16_SESSION_88.md`.

## 2026-04-16 Bloodlines Continuation Platform Thin Slice

- A dedicated local continuation cockpit now exists at `continuation-platform/` inside the canonical Bloodlines root. This keeps the offline continuation lane inside Bloodlines governance instead of creating a second project root.
- Runtime choice for the first slice is Windows-native Python 3.14 with a localhost browser UI. Primary launch path: `continuation-platform/launch_windows.cmd`, which serves the UI at `http://127.0.0.1:8067`.
- The thin slice now scans the full Bloodlines tree for continuity awareness while ingesting a minimal canonical subset of 14 authoritative files into a local sqlite plus FTS continuity store.
- The first breadth pass beyond the thin subset is now live. The platform now builds a broader discovered-source registry across the Bloodlines text and project-source corpus, with heuristic authority scoring and surfaced conflict clusters for duplicate or competing continuity surfaces.
- Generated local state now includes:
  - `continuation-platform/state/source_map.json`
  - `continuation-platform/state/canonical_source_registry.json`
  - `continuation-platform/state/discovered_source_registry.json`
  - `continuation-platform/state/model_inventory.json`
  - `continuation-platform/state/resume_state.json`
  - `continuation-platform/state/telemetry.json`
  - `continuation-platform/state/operations_journal.jsonl`
  - `continuation-platform/state/tasks_board.json`
  - `continuation-platform/state/handoff_pack_preview.md`
- The live Ollama inventory is now surfaced and routed from actual local availability. Current thin-slice model inventory captured 8 installed models, with retrieval/classification assigned to `qwen-local:latest` and generation assigned to `qwen2.5-coder:14b`.
- `resume_last_good_state` now runs end to end. The latest validated run resolved `continuity/PROJECT_STATE.json` as the current continuity anchor, then produced a grounded next-step recommendation pointing back into the Unity scene/bootstrap verification lane.
- Latest validated expanded scan state:
  - 134514 Bloodlines files mapped
  - 14 canonical subset documents ingested
  - 5355 discovered registry documents scored
  - 40 conflict clusters surfaced
- The GUI now extends beyond the initial Dashboard and Agent Workspace into live Memory, Tasks, Timeline, Config, and Handoff Preview surfaces. The remaining GUI gap is the richer diff panel and fuller handoff-builder workflow.
- Write posture is read-only by default and session-memory-only. Validation confirmed the guarded project-write endpoint refuses locked writes with `tier_insufficient` until the session is explicitly unlocked at the required Boss tier.
- WSL is not required for the thin slice. The current validated daily-use path is Windows-first.

## 2026-04-16 Foundational Player Guide Draft Update

- Added `18_EXPORTS/BLOODLINES_PLAYER_GUIDE_FOUNDATIONS.md` as the first full player-facing Bloodlines manual draft.
- The new export is intentionally player-facing rather than developer-facing: it explains what Bloodlines is, why the game is not a simple RTS, how food, water, housing, loyalty, bloodline members, governance, warfare, and territorial command shape play, and why a Dynasty must be coherent before it can be dominant.
- The guide includes a dedicated `Faith and Conviction: The Soul of a Dynasty` section that keeps covenantal faith and lived conviction clearly separate while explaining how they reinforce or strain one another.
- The guide also includes a dedicated `The Living Realm: Population, Rule, and Territory` primer that ties population, food, water, housing, labor, bloodline offices, loyalty, governance, and territorial command into one player-facing strategic cycle.
- The guide now also includes a dedicated `Bloodline Members: The Pillars of Rule` section that explains succession, commanders, governors, envoys, ideological leaders, merchants, and spymasters in player-facing terms, including how death and capture reshape the realm.
- The guide now also includes a dedicated `War, Siege, and Tactical Consequence` section that explains campaign rhythm, tribal versus house conflict timing, morale, supply, command presence, siege preparation, fortification value, and why reckless assaults are strategically punished.
- The guide now also includes a dedicated `Governance, Loyalty, and Holding What You Take` section that explains why territory requires both military control and population loyalty, how provinces stabilize, why overexpansion creates administrative fracture, and how governance itself becomes a strategic weapon.
- The guide now also includes a dedicated onboarding layer explaining why population, economy, food, water, bloodline members, faith, conviction, tactical conduct, post-battle rulings, and sustainable expansion all matter, plus a separate post-battle consequence section focused on prisoners, civilians, captives, severity, mercy, and remembered rule.
- The guide includes full player primers for Trueborn, Highborne, Ironmark, Goldgrave, Stonehelm, Westland, Hartvale, Whitehall, and Oldcrest, followed by comparison, first-match guidance, common mistakes, and a roadmap for future guide volumes.
- Future documentation work should extend this player-guide export rather than re-deriving player-facing language directly from the design bible or raw player-manual ingestion files.

## 2026-04-16 Unity Movement Foundation Update

- Added the first reusable Unity ECS movement layer in `unity/Assets/_Bloodlines/Code/`:
  - `Components/MoveCommandComponent.cs`
  - `Components/MovementStatsComponent.cs`
  - `Pathing/UnitMovementSystem.cs`
  - `Pathing/PositionToLocalTransformSystem.cs`
- Verified Unity runtime C# compilation with:
  - `dotnet build unity/Assembly-CSharp.csproj -nologo`
  - `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
- Full Unity batch verification against `D:\ProjectsHome\Bloodlines\unity` is currently blocked only because the canonical project is already open in another Unity instance.
- Added a preservation notice for the second non-canonical template at `unity/My project/STUB_TEMPLATE_NOTICE.md`.
- Rewrote `unity/README.md`, `unity/Assets/_Bloodlines/README.md`, and `unity/Assets/_Bloodlines/Code/README.md` so the Unity lane documentation now matches the actual 6.3 LTS + DOTS state on disk.
- Detailed handoff for this increment: `docs/unity/session-handoffs/2026-04-16-unity-movement-foundation.md`.

## 2026-04-16 Unity Bootstrap And Territory Yield Foundation Update

- Added the first `MapDefinition` to ECS bootstrap bridge in `unity/Assets/_Bloodlines/Code/`:
  - `Authoring/BloodlinesMapBootstrapAuthoring.cs`
  - `Editor/BloodlinesMapBootstrapBaker.cs`
  - `Components/MapBootstrapComponents.cs`
  - `Systems/SkirmishBootstrapSystem.cs`
- Added the first territory-yield economy bridge in `unity/Assets/_Bloodlines/Code/Systems/ControlPointResourceTrickleSystem.cs`.
- Extended `ControlPointComponent` so live ECS control points now preserve continent identity and resource-trickle metadata instead of dropping it after bake time.
- Corrected a real Unity-lane map fidelity bug:
  - `ControlPointData.resourceTrickle` now uses float fields rather than integer `ResourceAmountFields`
  - `TerrainPatchData` now preserves `isCoastal` and `continentDivide`
  - `ControlPointData` now preserves `continentId`
- Patched `unity/Assets/_Bloodlines/Data/MapDefinitions/ironmark_frontier.asset` so the canonical map asset once again reflects the JSON control-point trickle decimals and continent metadata already present in `data/maps/ironmark-frontier.json`.
- Browser validation still passed after the map-definition and ECS changes:
  - `node tests/data-validation.mjs`
  - `node tests/runtime-bridge.mjs`
- Unity C# verification also passed for the new bootstrap slice and territory-yield system by using isolated Codex intermediate/output paths, because the canonical project was already open and holding the default `Temp\obj` outputs:
  - runtime assembly: 0 warnings, 0 errors
  - editor assembly: 0 errors, longstanding importer/editor warnings remain unchanged
- Detailed handoff for this increment: `docs/unity/session-handoffs/2026-04-16-unity-bootstrap-and-territory-yield.md`.

## 2026-04-16 Unity Scene Scaffold, Camera, And Debug Shell Update

- Added the first governed Unity scene-shell creation path:
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesGameplaySceneBootstrap.cs`
  - menu: `Bloodlines -> Scenes -> Create Bootstrap And Gameplay Scene Shells`
  - batch wrapper: `scripts/Invoke-BloodlinesUnityCreateGameplaySceneShells.ps1`
- This tool now creates the first intended scene shells without hand-authoring `.unity` YAML:
  - `Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity`
  - `Assets/_Bloodlines/Scenes/Gameplay/IronmarkFrontier.unity`
- Added the first battlefield camera shell at `unity/Assets/_Bloodlines/Code/Camera/BloodlinesBattlefieldCameraController.cs`.
  - supports keyboard pan, edge-scroll, middle-mouse drag pan, `Q` / `E` rotation, and mouse-wheel zoom
  - preserves map-bound and initial-focus configuration from the canonical `MapDefinition`
- Added the first visible Unity ECS debug presenter at `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugEntityPresentationBridge.cs`.
  - presents units, buildings, settlements, control points, and resource nodes as primitive markers
  - explicitly debug-only and not a substitute for the production render path
- Updated Unity docs and scene-folder guidance so the next execution step now points to the governed scene-shell tool rather than manual scene rediscovery.
- Unity C# verification for this slice passed by using isolated Codex intermediate/output paths:
  - runtime assembly: 0 warnings, 0 errors
  - editor assembly: 0 warnings, 0 errors
- Actual `.unity` scene files were not generated in this pass because the canonical project was already open in another Unity instance and the new tool was prepared for in-editor or future lock-free batch execution instead.
- Detailed handoff for this increment: `docs/unity/session-handoffs/2026-04-16-unity-scene-scaffold-camera-and-debug-shell.md`.

## 2026-04-16 Unity Control-Point Capture Foundation Update

- Added the first ECS ownership-and-capture bridge at `unity/Assets/_Bloodlines/Code/Systems/ControlPointCaptureSystem.cs`.
- Added the missing Unity asset metadata file `unity/Assets/_Bloodlines/Code/Systems/ControlPointCaptureSystem.cs.meta` so the new runtime slice has stable Unity identity on refresh.
- `ControlPointCaptureSystem` now provides the first narrow runtime parity for battlefield territory contention:
  - non-worker living units can claim or contest a point
  - contested or empty points decay capture progress
  - owned points stabilize over time
  - stabilized points fall back to occupied when loyalty is driven down
  - successful claimants flip ownership and reset loyalty into the occupied band
- Updated the territory-yield bridge so `ControlPointResourceTrickleSystem` only pays uncontested owned points after capture state is resolved for the frame.
- Unity C# verification still passed for the runtime assembly through isolated Codex intermediate/output paths after this slice landed:
  - runtime assembly: 0 warnings, 0 errors
  - editor assembly: 0 errors, longstanding `CS0649` warnings remain in existing editor/importer helpers
- Detailed handoff for this increment: `docs/unity/session-handoffs/2026-04-16-unity-control-point-capture-foundation.md`.

## 2026-04-16 Unity Selection And Formation-Move Shell Update

- Recovered and classified the already-present first interactive battlefield shell in `unity/Assets/_Bloodlines/Code/`:
  - `Debug/BloodlinesDebugCommandSurface.cs`
  - `Components/SelectionComponent.cs`
- Confirmed the governed scene-shell tool already wires `BloodlinesDebugCommandSurface` into generated scenes, so the intended first Play Mode shell is interactive rather than view-only.
- Extended `BloodlinesDebugCommandSurface` so multi-unit right-click commands now issue formation-aware move destinations instead of collapsing the whole selection onto one point.
- Current first-shell command behavior now includes:
  - left-click single select
  - `1` select all controlled units
  - `Escape` clear selection
  - right-click formation-aware move issuance
- Unity C# verification after this slice:
  - runtime assembly: 0 warnings, 0 errors
  - editor assembly: 0 errors, longstanding `CS0649` warnings remain in existing editor/importer helpers and currently total 105 warnings in the isolated editor build
- Detailed handoff for this increment: `docs/unity/session-handoffs/2026-04-16-unity-selection-and-formation-move-shell.md`.

## 2026-04-16 Unity Drag-Box Selection Shell Update

- Extended `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.cs` again so the first interactive battlefield shell now supports drag-box selection in addition to single-click selection and select-all.
- Added a simple on-screen selection rectangle overlay so the first shell communicates drag selection state visually instead of silently changing selection.
- The current first-shell interaction surface now includes:
  - left-click single select
  - left-drag box select
  - `1` select all controlled units
  - `Escape` clear selection
  - right-click formation-aware move issuance
- Unity C# verification after the drag-box extension:
  - runtime assembly: 0 warnings, 0 errors
  - editor assembly: 0 errors, longstanding `CS0649` warnings remain in existing editor/importer helpers and still total 105 warnings in the isolated editor build
- Detailed handoff for this increment remains `docs/unity/session-handoffs/2026-04-16-unity-selection-and-formation-move-shell.md`, now updated to include drag-box selection.

## 2026-04-16 Unity Definition Binding And Bootstrap Scene Recovery

- Ran the governed scene-shell generator and confirmed the first canonical Unity scenes now exist on disk:
  - `unity/Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity`
  - `unity/Assets/_Bloodlines/Scenes/Gameplay/IronmarkFrontier.unity`
- Diagnosed the real blocker behind failed Bootstrap map assignment: all 119 generated Unity definition assets under `unity/Assets/_Bloodlines/Data/` were serialized with `m_Script: {fileID: 0}`, so `MapDefinition` could not be resolved reliably in editor automation.
- Repaired the Unity data-definition structure by splitting each ScriptableObject definition into its own correctly named file:
  - `HouseDefinition.cs`
  - `ResourceDefinition.cs`
  - `UnitDefinition.cs`
  - `BuildingDefinition.cs`
  - `FaithDefinition.cs`
  - `ConvictionStateDefinition.cs`
  - `BloodlineRoleDefinition.cs`
  - `BloodlinePathDefinition.cs`
  - `VictoryConditionDefinition.cs`
  - `SettlementClassDefinition.cs`
  - `RealmConditionDefinition.cs`
  - `MapDefinition.cs`
- Preserved the pre-repair broken generated definition assets under `reports/unity-definition-binding-repair/2026-04-16-095158/` before recreating them in place through the governed JSON importer.
- Added the governed Unity JSON sync wrapper at `scripts/Invoke-BloodlinesUnitySyncJsonContent.ps1`.
- Updated `unity/Assets/_Bloodlines/Code/Editor/JsonContentImporter.cs` so broken generated definition assets are backed up and recreated in place when Unity cannot load them as the expected ScriptableObject type.
- Verified the repaired result:
  - 119 definition assets now carry valid `m_Script` references
  - 0 definition assets remain in the old `m_Script: {fileID: 0}` state
- Updated `unity/Assets/_Bloodlines/Code/Editor/BloodlinesGameplaySceneBootstrap.cs` so Bootstrap repair assigns the canonical map through a persistent asset-path reference and logs the resolved asset path cleanly.
- Added a governed Bootstrap-scene validation entry point:
  - menu: `Bloodlines -> Scenes -> Validate Bootstrap Scene Shell`
  - wrapper: `scripts/Invoke-BloodlinesUnityValidateBootstrapSceneShell.ps1`
- `unity/Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity` now stores the canonical `ironmark_frontier.asset` reference directly:
  - `map: {fileID: 11400000, guid: d1ad0843071350c45aa1a54a2bad5b84, type: 2}`
- Verification for this recovery block:
  - `node tests/data-validation.mjs` passed
  - `node tests/runtime-bridge.mjs` passed
  - `dotnet build unity/Assembly-CSharp.csproj -nologo` passed through isolated Codex intermediate/output paths with 0 warnings and 0 errors
  - `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` passed through isolated Codex intermediate/output paths with 0 errors and the same longstanding 105 `CS0649` warnings
- Detailed handoff for this increment: `docs/unity/session-handoffs/2026-04-16-unity-definition-binding-and-bootstrap-repair.md`.

## 2026-04-16 Unity Canonical Scene-Shell Validation Coverage Update

- Extended `unity/Assets/_Bloodlines/Code/Editor/BloodlinesGameplaySceneBootstrap.cs` so scene-shell validation now covers both canonical scenes through one shared structural validator.
- Added the first governed Gameplay scene validation menu path:
  - `Bloodlines -> Scenes -> Validate Gameplay Scene Shell`
- Added governed batch wrappers for the scene-validation lane:
  - `scripts/Invoke-BloodlinesUnityValidateBootstrapSceneShell.ps1`
  - `scripts/Invoke-BloodlinesUnityValidateGameplaySceneShell.ps1`
  - `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1`
- Hardened the Bootstrap and Gameplay validation wrappers so they automatically rerun once if the first Unity batch pass is consumed by script compilation or import work before the validator logs an explicit pass or fail line.
- Captured the current package-resolution nuance: `unity/Packages/manifest.json` still pins `com.unity.collections` to `2.5.7`, but `unity/Packages/packages-lock.json` and Unity batch logs currently resolve `com.unity.collections` as `2.6.5`. Preserve the current stable state and do not churn packages casually.
- Verified the canonical scene-shell lane in batch mode:
  - `Bootstrap.unity` passed structural validation against canonical map assignment, metadata root, camera shell, debug presentation bridge, command surface, and reference ground.
  - `IronmarkFrontier.unity` passed structural validation against gameplay-scene metadata, camera shell, debug presentation bridge, command surface, and reference ground while confirming that gameplay scenes do not carry the bootstrap authoring component.
  - `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` completed successfully and now serves as the governed preflight path for both scene shells together.
- Final verification for this slice:
  - `node tests/data-validation.mjs` passed
  - `node tests/runtime-bridge.mjs` passed
  - `dotnet build unity/Assembly-CSharp.csproj -nologo` passed through isolated Codex intermediate/output paths with 0 warnings and 0 errors
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` passed through isolated Codex intermediate/output paths with 0 errors and the same longstanding 105 `CS0649` warnings
- Detailed handoff for this increment: `docs/unity/session-handoffs/2026-04-16-unity-canonical-scene-shell-validation.md`.

## 2026-04-16 Unity Bootstrap Runtime Smoke And Command-Shell Ergonomics Update

- Hardened the governed Unity batch wrappers so `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1`, `scripts/Invoke-BloodlinesUnityValidateBootstrapSceneShell.ps1`, `scripts/Invoke-BloodlinesUnityValidateGameplaySceneShell.ps1`, and `scripts/Invoke-BloodlinesUnitySyncJsonContent.ps1` now wait on the actual Unity process instead of returning early. These wrappers should be run serially against the same Unity project, not in parallel.
- Extended `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs` so the governed runtime-smoke lane now logs richer diagnostics and validates:
  - live bootstrap spawning against canonical map counts
  - debug presentation bridge presence
  - command-shell select-all
  - control-group 2 save/recall
  - selection framing
- Fixed the remaining runtime blockers behind the first live Unity shell:
  - `unity/Assets/_Bloodlines/Code/Authoring/BloodlinesMapBootstrapAuthoring.cs` now injects the canonical bootstrap seed entity into Play Mode when the scene authoring anchor is present and the world needs runtime bootstrap data
  - authoring-side dynamic buffer writes are now safe across structural changes
  - `unity/Assets/_Bloodlines/Code/Systems/SkirmishBootstrapSystem.cs` now snapshots seed buffers before runtime spawning, preventing invalidated-buffer exceptions and runaway repeat-spawn behavior
  - `unity/Assets/_Bloodlines/Code/Pathing/PositionToLocalTransformSystem.cs` now runs in `SimulationSystemGroup` after `UnitMovementSystem`, removing the invalid Presentation ordering issue
- Extended `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.cs` with a compact battlefield HUD plus stronger RTS control ergonomics:
  - faction resource and population summary
  - selection composition summary
  - control-point ownership summary
  - control groups `2` through `5`
  - `Ctrl+2-5` save
  - `2-5` recall
  - `F` frame selection or controlled-force fallback
- Extended `unity/Assets/_Bloodlines/Code/Camera/BloodlinesBattlefieldCameraController.cs` with a public focus entry point so command-surface framing can reposition the battlefield camera coherently.
- Verified the governed runtime shell end to end:
  - `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` passed
  - pass log preserved in `artifacts/unity-bootstrap-runtime-smoke.log`
  - validated runtime counts: factions 3, buildings 9, units 16, resource nodes 13, control points 4, settlements 2
  - validated command-shell counts: controlled units 6, selection after select-all 6, control group 2 count 6, frame selection succeeded
- Final verification for this slice:
  - `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` passed again after the command-shell changes
  - `dotnet build unity/Assembly-CSharp.csproj -nologo` passed through isolated Codex intermediate/output paths with 0 warnings and 0 errors
  - `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` passed through isolated Codex intermediate/output paths with 0 errors and the same longstanding 105 `CS0649` warnings
- Detailed handoff for this increment: `docs/unity/session-handoffs/2026-04-16-unity-bootstrap-runtime-green-and-command-groups.md`.

## Imported Source Bundles Now Preserved In-Root

- 2026-03-22 early Bloodlines prompt bundle
- 2026-04-13 external-repo preconsolidation snapshot
- 2026-04-13 repo-root mirror preconsolidation snapshot
- 2026-04-14 full `deploy/bloodlines` compatibility copy
- 2026-04-14 full `frontier-bastion` predecessor root
- 2026-04-14 Bloodlines-specific parent-site preview surfaces
- 2026-04-14 external doctrine DOCX bundle from `D:\Lance\Downloads`

## Deploy Reconciliation Status

The active canonical tree now also directly includes the deploy-only delta:

- `88` files that existed only in `deploy/bloodlines` were copied into the active canonical tree
- the newer deploy `index.html` was promoted into the canonical root
- the thinner deploy-side single-root note was preserved as `docs/CONSOLIDATION_NOTE_2026-04-13_SINGLE_ROOT_deploy_variant_2026-04-14.md`

This means the canonical root now contains both:

- the full preserved deploy compatibility copy under `archive_preserved_sources/`
- the deploy-only and deploy-latest working-tree additions needed for forward work

## Active Implementation State

### Browser Reference Simulation (Frozen Behavioral Spec)

- Boots and is test-backed (`data-validation.mjs` + `runtime-bridge.mjs` both green).
- No new canonical systems belong here. Use this surface to validate feel, sequencing, pressure interaction, and AI behavior targets while implementing equivalent depth in Unity.
- Dynasty consequence cascade is live (commander capture/kill, captive ledger, heir succession, legitimacy delta on head fall, governor loss on territory flip).
- Doctrine path effects and conviction ledger are live at the current bridge depth.
- Stone + iron economy is live with the canonical smelting chain: iron production at `iron_mine` consumes wood at 0.5 ratio; if wood is insufficient, ore returns to the node and the worker stalls.
- Fortification building class is live: `wall_segment` (structural mult 0.2), `watch_tower` (0.15 plus sight aura plus attack aura), `gatehouse` (0.3, passable), `keep_tier_1` (0.1, canonical primary-keep foundation). Building completion advances the nearest settlement's `fortificationTier` up to its class ceiling.
- Settlement class + fortification tier metadata is live on control points and `state.world.settlements`. Supported classes: `border_settlement`, `military_fort`, `trade_town`, `regional_stronghold`, `primary_dynastic_keep`, `fortress_citadel`.
- Formal siege production infrastructure is now live: `siege_workshop` is buildable, Rams no longer come from Barracks, and siege preparation now has a dedicated production seat.
- Three siege engines are now live: `ram`, `siege_tower`, and `trebuchet`. Ram remains the gate-and-wall breach engine; Siege Tower now gives nearby allies better structural reach and pressure against defended walls; Trebuchet now delivers ranged bombardment against fortifications.
- Formal siege sustainment is now live: `supply_camp` is buildable as the forward logistics anchor, and the workshop now trains `siege_engineer` and `supply_wagon` specialist units alongside the engine roster.
- Engineer specialists are now live. Engineers are no longer abstract doctrine text: they provide nearby breach support, extend allied structural assault pressure, and repair damaged siege engines in the line.
- Siege supply continuity is now live. Supply Wagons linked to a live Supply Camp now resupply nearby siege engines with food, water, and wood-part throughput; unsupplied engines retain pressure but lose operational efficiency until the line is restored.
- Siege line interdiction is now structurally meaningful. Destroying or separating the camp-wagon-engine chain now cuts supply continuity and weakens formal assault output without needing a separate hardcoded sabotage script.
- Assault cohesion strain is live as wave-spam denial: combat units dying near hostile fortifications accrue strain; at threshold 6, the attacking faction takes a 20-second 0.85x cohesion attack penalty.
- Canonical 90-second realm condition cycle is live: famine after 2 consecutive sub-food cycles, water crisis after 1 sub-water cycle, cap pressure at 95 percent occupancy.
- `getRealmConditionSnapshot` is live and now surfaced into the HUD for the six most load-bearing pressure areas: population, food, water, loyalty, fortification, and army.
- Browser renderer legibility wave is live: stone and iron nodes have distinct silhouettes, fortification buildings have distinct wall/tower/gate/keep silhouettes, and Rams render as siege engines instead of generic unit markers.
- Browser command surface is live for the expanded economy, logistics, and fortification lane: worker build options include `quarry`, `iron_mine`, `siege_workshop`, `supply_camp`, `wall_segment`, `watch_tower`, `gatehouse`, and `keep_tier_1`; live workshop training UI surfaces the active siege roster with cost and population usage.
- Browser HUD legibility wave is live: resource bar now carries 10 pills (gold, food, water, wood, stone, iron, influence, available pop, total pop, territory), and the realm-condition bar now reflects the snapshot output each frame.
- Enemy AI now refuses direct keep assaults against fortified primary seats unless a siege-class unit is present.
- Enemy AI now advances from refusal into preparation. Stonehelm can add `quarry`, `iron_mine`, and `siege_workshop` when a fortified keep blocks the direct route, queue a first bombard engine from the workshop, and form siege lines before committing the assault.
- Enemy AI siege logistics are now live. Stonehelm can anchor a forward `supply_camp`, queue `siege_engineer` and `supply_wagon` support after the opening bombard engine, and delay the keep assault until engineers, wagons, and at least one supplied engine are present.
- Browser fortified-settlement reserve cycling is now live: wounded defenders at fortified seats fall back to the keep for triage while healthier reserves muster forward to the threatened section.
- Primary-keep reserve state is now surfaced into the HUD snapshot: ready reserves, recovering reserves, threat state, commander keep-presence, and bloodline-seat presence are visible in the fortification pill meta.
- Browser renderer now marks reserve-duty states on units so reserve musters and fallback cycles are legible in motion.
- Governor specialization is now live. Governor assignments can anchor either to frontier control points or to dynastic settlements, and specializations now resolve as `border`, `city`, or `keep` based on the governed anchor class.
- Governor rotation is now live. With no outer territory the governor defaults to keep stewardship; newly occupied marches pull the governor outward; fully stabilized marches allow rotation back to the keep.
- Faith-integrated fortification wards are now live at fortified seats: Old Light pyre wards, Blood Dominion blood-altar reserve/surge, Order edict wards, and Wild root wards all feed into defensive behavior.
- Local combat near fortified seats now reflects ward and keep-presence bonuses. Defenders can gain additional sight and damage leverage, and hostile combatants entering Wild-warded approaches are slowed.
- Blood Dominion reserve rites are now live as a sacrificial defensive expression: under active threat a fortified seat can trade population for a temporary defensive surge.
- Dynasty captivity operations are now live. Captured members no longer sit only as passive ledger state: the source dynasty can open negotiated ransom operations or covert rescue operations, and the captor can compel an immediate ransom exchange when the rival house can pay.
- Dynasty operations are now stateful and time-resolved. Active operations live under `dynasty.operations`, escrow their resource commitment up front, resolve inside simulation time, and write history entries for both the initiating house and the target house.
- Rescue resolution is now structural rather than arbitrary. Covert recovery projects power from the source dynasty's spymaster / envoy / commander against captive renown, captor fortification depth, active keep ward, and captor covert pressure, then resolves deterministically at the end of the operation window.
- The dynasty panel now exposes active captivity decisions directly: held captives can be ransomed out by the captor, captured bloodline members can be targeted with negotiated release or rescue-cell actions, and in-flight operation progress is visible while the mission resolves.
- Dynasty political events are now live as the first event architecture on the match-progression and dual-clock spine. Factions carry active and historical dynasty-event state, with the first runtime event now implemented as `Succession Crisis`.
- `Succession Crisis` now triggers from ruling bloodline collapse, applies immediate loyalty shock plus ongoing legitimacy, loyalty, economy, stabilization, growth, and combat penalties, escalates over time if unresolved, surfaces in the dynasty panel and debug lane, and survives restore.
- Stonehelm now reads player succession instability as live exploitable weakness, while enemy courts also slow and self-consolidate when their own succession crisis is active.
- `Covenant Test` is now live across all four covenants and both doctrine paths. Covenant structures now feed live faith intensity, active tests impose runtime strain, direct rites resolve with real cost where canon supports them, and passed tests now gate Apex Covenant access, late faith-unit access, and Divine Right ascent.
- Stonehelm now climbs the covenant structure ladder honestly, fields stage-3 or stage-4 or apex faith units through that live ladder, and reacts to player covenant pressure through sharper military and holy-war tempo.
- The first Stage 5 `Territorial Governance Recognition` layer is now live. Final Convergence kingdoms can enter and sustain a real recognition state built from loyal stabilized holdings, territory share, and no-active-holy-war pressure, with world-pressure consequence, AI backlash, dual-clock declaration output, dynasty-panel legibility, and restore continuity.
- Territorial Governance now reaches the first honest sovereignty-resolution seam. Multi-seat dynastic authorities seat across keeps or cities or frontier control points, recognized governance now requires seat coverage plus court-loyalty and anti-revolt stability plus no-incoming-holy-war pressure, world pressure escalates from recognition into held governance and imminent sovereignty victory, Stonehelm answers with emergency anti-sovereignty tempo, and snapshot restore now preserves both active hold state and completed territorial-governance victory metadata.
- Validation depth increased: runtime bridge now covers fortification-tier advancement, smelting stall/success, ram-vs-wall damage advantage, trebuchet wall pressure, siege-tower-supported assault pressure, famine trigger under starvation, and fortified-keep AI refusal.
- Runtime bridge now also covers reserve cycling, governor specialization rotation, Old Light fortification-ward sight behavior, AI siege-infrastructure buildout, workshop engine queuing, staged siege-line commitment, negotiated ransom recovery, covert rescue recovery, captor-side ransom demand resolution, engineer-assisted breach pressure, engineer repair throughput, supply-wagon sustainment, AI support-unit queueing, AI supply-camp buildout, and AI delay on unsupplied siege lines.

### 2026-04-14 Session 9 Additions

- `ballista` is now live as a siege-engine with ranged anti-personnel pressure (attack range 170, anti-unit multiplier 1.2) and moderate structural pressure (1.6). Trained at the Siege Workshop. Participates in siege supply continuity and unsupplied penalty mathematics.
- `mantlet` is now live as a siege-support class that reduces inbound ranged damage to friendly units inside a 92-unit cover radius (0.55x incoming ranged damage multiplier). No attack. Mobile. Trained at the Siege Workshop.
- Stonehelm AI siege-line queue order extended to: trebuchet â†’ engineer â†’ supply wagon â†’ mantlet â†’ ballista â†’ ram â†’ siege tower. Mantlet now protects the siege line before ballista bolts arrive.
- `hasSiegeUnit` gate for keep-assault refusal refined so that mantlet alone does not satisfy siege capability. A true engine (ram / siege_tower / trebuchet / ballista) is still required.
- `getFactionSiegeUnits` default filter is now role === "siege-engine" so mantlet is not counted as an attacking engine in `formalSiegeReady` or `readyForFormalAssault` logic.
- Full 11-state realm-condition dashboard now live in the HUD (Cycle, Pop, Food, Water, Loyalty, Fort, Army, Faith, Conviction, Logistics, World). All states driven by `getRealmConditionSnapshot`.
- `getRealmConditionSnapshot` now exports the full canonical 11-state shape: cycle, population, food, water, loyalty, fortification, military, faith (covenant + doctrine path + intensity + tier + discovered-count + band), conviction (score + bandId + bandLabel + four-bucket ledger + top bucket + band), logistics (supply camp count + wagon count + engineer count + engine count + supplied engines + unsupplied engines + engineer-supported engines + formal-siege-ready flag + band), and worldPressure (tribe activity + contested territories + active operations + held captives + fallen members + signals + band).
- `tests/data-validation.mjs` now asserts ballista and mantlet schema and workshop trainability.
- `tests/runtime-bridge.mjs` now asserts mantlet cover reduces inbound ranged damage against friendly units, ballista is trainable at the workshop, and the snapshot exposes all 11 canonical blocks with canonical band colors.

### 2026-04-14 Session 9 Analysis Surfaces

- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_9.md` â€” comprehensive state analysis written under the full-realization continuation directive. Preserves the full protected scope (civilizational, military, dynastic, faith-and-conviction, world, legibility) and documents where runtime, data, and canon each stand.
- `docs/BLOODLINES_SYSTEM_GAP_ANALYSIS_2026-04-14_SESSION_9.md` â€” system-by-system matrix under LIVE / PARTIAL / DATA-ONLY / DOCUMENTED / CANON-LOCKED / VOIDED / CANON-OPEN classification. Includes implementation-debt priority and drift watches.
- `docs/plans/2026-04-14-session-9-full-realization-continuation-plan.md` â€” six-vector growth model covering civilizational depth, dynastic depth, military depth, faith-and-conviction depth, world depth, and legibility depth. Defines non-negotiable session-level rules and exit criteria per vector.
- `docs/plans/2026-04-14-session-9-next-phase-execution-roadmap.md` â€” concrete ordered roadmap for sessions 9 through 30 plus.
- `MASTER_BLOODLINES_CONTEXT.md` â€” Session 9 addendum appended additively at end of file.

### Unity Production Project State (Environment-Aligned)

- Canonical Unity project: `<repo>/unity/`. Canonical editor is Unity 6.3 LTS `6000.3.13f1`.
- Stub Unity folder at `<repo>/Bloodlines/`: fresh URP 3D template without DOTS packages, preserved in place with `STUB_TEMPLATE_NOTICE.md` per preservation mandate.
- Additional preserved template project under `<repo>/unity/My project/`: default sample-scene Unity template, now labeled with `STUB_TEMPLATE_NOTICE.md` and not part of the production lane.
- DOTS/ECS package stack already installed in `<repo>/unity/Packages/manifest.json`: Entities 1.4.3, Burst 1.8.29, Collections 2.5.7, Mathematics 1.3.3, Entities.Graphics 1.4.16, URP 17.3.0, Input System 1.11.2, Addressables 2.5.0.
- `Assets/_Bloodlines/` tree matches the approved baseline with full subfolder structure for Art, Audio, Code, Data, Prefabs, Scenes, Materials, Shaders, Animation, and Docs.
- `BloodlinesDefinitions.cs` is extended with fortification, siege, settlement, and realm-condition canon fields.
- `JsonContentImporter.cs` is extended to import `settlement-classes.json` and `realm-conditions.json` and populate the new fields.
- `unity/README.md` is rewritten to reflect canonical root governance, approved toolchain, and direction-of-play non-negotiables (Generals/Zero Hour feel, theatre zoom, bloodline UI presence).
- Structural READMEs are seeded at `_Bloodlines/README.md`, `_Bloodlines/Code/README.md`, and `_Bloodlines/Data/README.md`.
- First full editor open and `Bloodlines -> Import -> Sync JSON Content` have already completed for the canonical Unity lane.
- The first additive movement foundation is now present: `MoveCommandComponent`, `MovementStatsComponent`, `Bloodlines.Pathing.UnitMovementSystem`, and `Bloodlines.Pathing.PositionToLocalTransformSystem`.
- Unity batchmode has also been executed successfully earlier for graphics-lane staging validation, but a fresh batch verification pass in this session could not take the project lock because another Unity instance was already open on the canonical project.

### Machine Environment

- Unity Hub plus Unity `6000.3.13f1` (6.3 LTS, approved) and Unity `6000.4.2f1` (6.4) installed
- Visual Studio 18 Community installed (VS 2022 legacy remnants in x86 path; functional via Unity's `ide.visualstudio` package)
- Blender 5.1, Git 2.46, GitHub Desktop, VS Code, .NET SDK 10.0.201 installed
- Wwise, Perforce, JetBrains Rider not installed (staged / acceptable)

See `ENVIRONMENT_REPORT_2026-04-14.md` at repo root for the full audit details.

## Primary Current Sources Of Truth

- Governance: `AGENTS.md`, `README.md`, `CLAUDE.md`
- Continuity: `NEXT_SESSION_HANDOFF.md`, `continuity/PROJECT_STATE.json`
- Local continuation platform: `continuation-platform/README.md`, `continuation-platform/docs/system_design.md`, `continuation-platform/state/resume_state.json`
- Design canon: `01_CANON/`, `18_EXPORTS/`, `docs/DEFINITIVE_DECISIONS_REGISTER.md`
- Current curated bible: `18_EXPORTS/BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.4.md`
- Current doctrine source: `01_CANON/BLOODLINES_MASTER_DESIGN_DOCTRINE_2026-04-14.md`
- Runtime reality: `docs/BLOODLINES_CURRENT_STATE_ANALYSIS.md`, `tasks/todo.md`
- Session 4 diagnosis: `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_4.md`
- Session 5 addendum: `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_5.md`
- Session 6 addendum: `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_6.md`
- Session 7 addendum: `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_7.md`
- Session 8 addendum: `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_8.md`
- Session 9 full-realization state analysis: `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_9.md`
- Session 9 gap analysis: `docs/BLOODLINES_SYSTEM_GAP_ANALYSIS_2026-04-14_SESSION_9.md`
- Session 9 continuation plan: `docs/plans/2026-04-14-session-9-full-realization-continuation-plan.md`
- Session 9 execution roadmap: `docs/plans/2026-04-14-session-9-next-phase-execution-roadmap.md`
- Session 10 state-of-game report: `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_10.md`
- Latest continuation report: `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_87.md`

### 2026-04-14 Session 10 Additions

- Unity version alignment decision resolved: Option B locked. Unity 6.3 LTS (`6000.3.13f1`) is the canonical editor for `unity/`. `ProjectVersion.txt` and `Packages/manifest.json` aligned.
- First ECS foundation authored: 15 canonical component files + 3 canonical systems (Bootstrap, RealmConditionCycle, PopulationGrowth) under `unity/Assets/_Bloodlines/Code/`. Unity batchmode has now opened successfully for graphics tooling verification; first wider compile-and-import pass in the editor remains pending.
- Sabotage operations are now live as first covert operation type. Four canonical sub-types: `gate_opening`, `fire_raising`, `supply_poisoning`, `well_poisoning`. Requires spymaster bloodline member. Escrowed cost. Success formula: spymaster renown + 45 vs fort tier Ã— 12 + ward active Ã— 15 + target spymaster Ã— 10. Counterplay on failure.
- Burn damage-over-time from `fire_raising` applies through a new `tickBuildingStatusEffects` loop. Poisoned supply camps are excluded from the wagon-link filter in `tickSiegeSupportLogistics`, interdicting siege supply until the window expires.
- Commander keep-presence sortie is now live. Requires commander at keep AND active threat AND cooldown elapsed. Duration 12s, cooldown 60s. Grants defenders Ã—1.22 attack and +22 sight during the active window.
- `getRealmConditionSnapshot` fortification block now exposes `commanderAtKeep`, `sortieActive`, `sortieCooldownRemaining`, `sortieReady`.
- Public API additions: `startSabotageOperation`, `issueKeepSortie`, `SORTIE_DURATION_SECONDS`, `SORTIE_COOLDOWN_SECONDS`.
- Tests extended: sabotage subtype validation, sabotage target validation, fire_raising escrow, sortie refusal path, snapshot fortification exposure.
- Provenance: `SOURCE_PROVENANCE_MAP.md`, `CONSOLIDATION_REPORT.md`

### 2026-04-15 Sessions 44 Through 51 Additions

- Session 44 made lesser-house defection materially world-facing: a defected cadet branch now spawns as a real `minor_house` faction with founder-copy dynasty state, hostility to the parent, and save/restore persistence.
- Session 45 anchored that breakaway branch physically: the new faction now spawns a founder militia near the parent command hall with commander attachment and consistent population bookkeeping.
- Session 46 deepened the civilizational loop: strong food and water surplus now reinforces loyalty across owned marches each canonical realm cycle, provided cap pressure is not active.
- Session 47 completed the first territorial expression of breakaway politics: a defected minor house now claims a stabilized border march with live food and influence trickle, visible through the world control layer and dynasty panel, and stable through snapshot restore.
- Session 48 activated the breakaway polity as an operational actor: minor-house AI now defends the claimed march, retakes it if seized, and regroups to the claim after pressure clears.
- Session 49 corrected restore-path id continuity: snapshot restore now rebuilds the live prefix-based counters used by the runtime, preventing post-restore id collision when new buildings or dynasty entities are created later in the match.
- Session 50 made the breakaway march materially generative: held minor-house territory now raises local militia, swordsmen, and archers by spending food and influence, reducing local loyalty, and growing a real retinue that persists through snapshot restore.
- Session 51 made mixed blood matter to cadet instability: marriage-born outside-house lineage now modifies lesser-house daily loyalty drift, active marriages soften that pull, renewed hostility to the outside house worsens it, and the dynasty panel surfaces the pressure.
- Runtime validation is green again after completing and correcting the previously unfinished Session 46 runtime-bridge branch.

### 2026-04-15 Session 52 Additions

- Session 52 made covenant and doctrine fit materially change AI marriage behavior. AI proposal and acceptance logic no longer reads only hostility and population deficit. It now also evaluates dynastic legitimacy distress and live faith compatibility.
- A new shared runtime helper, `getMarriageFaithCompatibilityProfile`, classifies unions as `unbound`, `harmonious`, `sectarian`, `strained`, or `fractured` from real faction covenant and doctrine state.
- Harmonious and sectarian matches can now open a legitimacy-repair marriage path when enemy legitimacy is weak enough.
- Fractured matches now block weak one-signal proposals and acceptances, so covenant rupture matters strategically instead of sitting in faith text.
- Pending marriage proposals now carry AI evaluation metadata for dynasty-panel legibility, and the dynasty panel now surfaces covenant stance for incoming and outgoing proposals plus the player proposal action.

### 2026-04-15 Session 53 Additions

- Session 53 made bloodline death terminate active marriages in live runtime. Real spouse death now dissolves the shared marriage record pair instead of leaving the union permanently active.
- Death-driven dissolution now stamps `dissolvedAt`, `dissolvedAtInWorldDays`, `dissolutionReason`, and deceased-spouse identity onto the existing marriage records so the change survives save and restore cleanly.
- Dissolution now affects two already-live dynastic systems immediately: both houses lose legitimacy and both record an oathkeeping mourning response in the conviction ledger.
- Marriage gestation now halts on dissolved unions, which means death no longer allows a marriage to continue generating children after the union is broken.
- Because lesser-house mixed-bloodline buffering already keys off active marriages, that buffering now naturally falls away once a union is dissolved.
- The dynasty panel now separates active marriages from marriages ended by death and exposes recent widowing with child count and dissolution timing.

### 2026-04-15 Session 54 Additions

- Session 54 made water infrastructure matter directly to campaigning armies. Owned marches, owned settlements, wells, supply camps, and camp-linked wagons now act as live hydration anchors for land forces.
- Field armies now accumulate dehydration strain when operating beyond those anchors. Strained and critical armies lose movement speed and attack output instead of carrying water only as passive resource text.
- The logistics block in `getRealmConditionSnapshot` now exposes hydrated, strained, critical, and anchor counts for field-water sustainment, and the 11-pill dashboard now surfaces that state in the logistics pill.
- Snapshot export and restore now preserve unit-level field-water state so long-form continuity does not discard army sustainment pressure mid-match.
- Stonehelm now recognizes critical dehydration in its assault column and regroups toward a live water anchor before renewing the march.

### 2026-04-15 Session 55 Additions

- Session 55 made covert operations bloodline-facing instead of building-only. Espionage and assassination now extend the already-live `dynasty.operations` lane beyond sabotage.
- Espionage now creates a live rival-court intelligence report carrying legitimacy, captive count, lesser-house count, and exposed rival bloodline members with location labels.
- Assassination now kills real bloodline members in runtime, clears commander or governor links when relevant, damages legitimacy, triggers succession ripple, and forces mutual hostility between the rival houses.
- The dynasty panel now surfaces intelligence reports, rival-court covert pressure, espionage launch actions, and assassination launch actions instead of hiding the covert lane in raw state.
- Stonehelm now runs espionage against the player and can escalate that live intelligence into assassination, so the covert lane is no longer player-only.
- Restore continuity now includes `dynastyIntel` id reconstruction, and runtime coverage proves fresh post-restore intelligence-report creation.

### 2026-04-15 Session 56 Additions

- Session 56 made faith commitment operational. Missionary pressure and holy war declaration now extend the live faith lane beyond passive exposure, doctrine effects, and keep wards.
- Missionary work now consumes real faith intensity, raises rival exposure to the attacking covenant, erodes incompatible rival covenant intensity, and pressures real march loyalty or dynastic legitimacy.
- Holy war declaration now creates a persistent active-war faith state, forces hostility, applies immediate rival pressure, and continues pulsing zeal and loyalty strain over time.
- The faith panel now exposes active faith operations, outbound holy wars, incoming holy wars, and player actions for missionary work and holy war declaration.
- Stonehelm now reacts through the same faith lane, launching missionary pressure and escalating into holy war when covenant fracture and dynastic pressure align.
- Restore continuity now includes `faithHolyWar` id reconstruction, and runtime coverage proves fresh post-restore holy-war creation.

### 2026-04-15 Session 57 Additions

- Session 57 made marriage authority explicitly dynastic. Proposals and approvals now route through live household authority instead of bypassing the ruling structure.
- The offering dynasty now requires a live diplomatic envoy to carry a marriage agreement, so role loss and capture materially disrupt the marriage lane.
- If the head of bloodline is unavailable, marriage governance now falls to heir or envoy regency, and that regency creates real legitimacy strain instead of acting as invisible fallback.
- Proposal records and accepted marriages now preserve governance provenance, including sanctioning authority, envoy, and approving authority, and that data survives restore.
- The dynasty panel now exposes the player court's marriage authority and envoy, shows sanction and approval provenance on pending and active unions, and surfaces target-household approval blockage.

### 2026-04-15 Session 58 Additions

- Session 58 advanced field-water pressure from tempo loss into live collapse. Unsupported armies now accumulate critical-duration state, begin taking health attrition under sustained dehydration, and can reach real desertion risk if the line remains dry.
- Commander presence now materially buffers that collapse, delaying attrition and desertion while reducing how quickly the line bleeds strength. Water pressure now interacts directly with the existing bloodline-command structure instead of sitting beside it.
- The logistics and military snapshot layers now expose attrition and desertion-risk counts, the 11-pill dashboard now surfaces those states directly, and the debug overlay exposes the expanded field-water profile for longer-run inspection.
- Stonehelm now reacts to dehydration collapse in a graded way: it regroups harder under active attrition and abandons an assault push before the line breaks into desertion.
- Verification also hardened the faith-war lane. Active holy war now sustains legitimacy pressure alongside territorial pressure, so ongoing holy-war consequence is no longer nullified by territorial restabilization.

### 2026-04-15 Session 59 Additions

- Session 59 made covert defense live. Courts can now raise a real counter-intelligence watch through the existing dynasty-operations lane instead of relying only on passive spymaster math.
- Active watch strength now reads real systems already in play: bloodline operator renown, keep fortification depth, ward backing, court loyalty, and dynastic legitimacy all affect defensive quality.
- Espionage and assassination no longer resolve against offense-only launch math. They now read live watch state at resolution, so defensive preparation can still matter after the offensive cell is already moving.
- Guarded bloodline roles now gain explicit assassination protection, which means head, heir, commander, spymaster, and governor safety are materially different under watch than in an open court.
- Successful interceptions now reinforce defending-house legitimacy, the dynasty panel exposes live watch state and rival-court protection, Stonehelm raises watch reactively, and restore continuity now rebuilds fresh `dynastyCounter` ids after snapshot round-trip.

### 2026-04-15 Session 60 Additions

- Session 60 made world pressure materially punitive instead of mostly observational. Kingdoms now accumulate live pressure score from territorial breadth, off-home holdings, active holy wars, held captives, hostile operations in flight, and dark-extremes descent.
- Realm cycles now promote a dominant-pressure leader through `Watchful`, `Severe`, and `Convergence` escalation levels instead of leaving world pressure as a flat signal count.
- Sustained pressure now damages the weakest owned march and, at severe pressure and above, also reduces dynastic legitimacy.
- Stonehelm now compresses attack, territory, sabotage, espionage, assassination, and holy-war timing against the world-pressure leader, and tribes now preferentially raid that dominant kingdom with faster cadence under stronger pressure.
- The 11-pill dashboard now exposes pressure label, score, streak, frontier-loyalty penalty, legitimacy pressure, and dominant-leader identity, and restore continuity now preserves the new pressure state through snapshot round-trip.

### 2026-04-15 Session 61 Additions

- Session 61 made successful covert interception actionable instead of purely defensive. Active counter-intelligence watch now records hostile source-faction interception history and turns a broken hostile operation into a live `Counter-intelligence dossier` on the attacking court.
- Intelligence reports now preserve source type, dossier label, intercepted operation type, and hostile network-hit count, which means a court can hold both standard espionage reporting and counter-intelligence-derived knowledge on the same rival without silent replacement.
- Stonehelm now reuses counter-intelligence dossiers as retaliatory knowledge, accelerating sabotage and assassination timing and striking back without first reopening redundant espionage.
- The dynasty panel now distinguishes a generic court report from a counter-intelligence dossier and surfaces intercepted operation type plus network-hit count directly in the existing intelligence-report lane.
- Restore continuity now preserves both the dossier and the source-scoped watch history that produced it.

### 2026-04-15 Session 62 Additions

- Session 62 made mixed-bloodline cadet pressure legible and stateful instead of leaving it as one blended drift number. Mixed-bloodline lesser houses now carry a real marriage-anchor profile derived from the parent house's relevant dynastic union with the outside house.
- Active marriage anchor now adds positive loyalty support, dissolved anchor now becomes a negative drift term, and renewed hostility now fractures that anchor into a harder cadet-house penalty.
- Existing branch children now strengthen an active anchor and deepen a dissolved one, so lineage carried through the marriage now materially affects cadet-house stability.
- The dynasty panel now surfaces mixed drift together with marriage-anchor house, status, pressure, and branch-child support, and restore continuity now preserves the new cadet-house fields through snapshot round-trip.

### 2026-04-15 Session 63 Additions

- Session 63 made Hartvale honestly playable instead of leaving it as a near-option in data only. Hartvale now reads as `prototypePlayable`, its Verdant Warden is prototype-enabled, and the shared barracks roster now includes that unique unit.
- Unique-unit access is no longer a raw building-data leak. Simulation now filters trainable units by house ownership and prototype status through a shared helper, and `queueProduction` rejects off-house unique-unit training with an explicit required-house reason.
- The command panel now reads the simulation-filtered roster, so Hartvale sees Verdant Warden in the live training surface while Ironmark and Stonehelm do not.
- Validation and runtime coverage now prove Hartvale can queue Verdant Warden and off-house factions cannot.

### 2026-04-15 Session 64 Additions

- Session 64 made dominant world pressure an internal dynastic threat instead of only an external realm burden. Overextended kingdoms now impose negative daily loyalty drift on their own active lesser houses.
- The new cadet-pressure seam reads the already-live world-pressure target state and severity level, then records branch-facing `worldPressureStatus`, `worldPressurePressure`, and `worldPressureLevel` on active lesser houses.
- The dynasty panel now surfaces branch-level world pressure, and the world pill now reports cadet drift plus pressured cadet count when the player is the dominant target.
- Snapshot continuity now preserves the new internal-pressure state through restore.

### 2026-04-15 Session 65 Additions

- Session 65 made dominant world pressure an external splinter accelerant instead of only an internal dynastic burden. Hostile breakaway minor houses now read live parent-realm world pressure and capitalize on overextension materially.
- Breakaway levy state now records `parentPressureLevel`, `parentPressureStatus`, `parentPressureLevyTempo`, `parentPressureRetakeTempo`, and `parentPressureRetinueBonus`, and levy growth now accelerates under parent overextension.
- Minor-house territorial AI now shortens regroup and retake cadence under parent pressure, so splinter opportunism affects live territorial behavior instead of sitting as a hidden modifier.
- The dynasty panel now surfaces hostile splinter parent-pressure severity, levy tempo, retake tempo, and pressure-driven cap bonus, and the world pill now reports pressured splinter count plus splinter tempo against the dominant target.
- Snapshot continuity now preserves the new splinter-opportunism state through restore.

### 2026-04-15 Session 66 Additions

- Session 66 made counter-intelligence dossiers materially useful beyond assassination reuse. Live dossiers now drive sabotage target selection, sabotage subtype selection, and sabotage support bonus.
- Sabotage operations now preserve `intelligenceReportId`, `intelligenceSupportBonus`, `retaliationReason`, and `retaliationInterceptType`, so dossier-backed retaliation stays legible and continuous through restore.
- Stonehelm sabotage now retaliates from live dossier knowledge instead of only the generic sabotage picker, and it still avoids reopening redundant espionage first.
- The dynasty panel now surfaces dossier-backed sabotage recommendation and support directly inside counter-intelligence report rows, and active sabotage operations now show dossier provenance.

### 2026-04-15 Session 67 Additions

- Session 67 made dossier-backed sabotage a real player decision instead of an AI-only privilege. A valid counter-intelligence dossier on a rival court now opens a live sabotage action in the rival-court panel.
- The new player action uses the shared dossier sabotage profile and shared sabotage terms. It surfaces real target, subtype, cost, duration, chance, and dossier support bonus instead of decorative copy.
- Player-launched dossier sabotage now preserves `intelligenceReportId` and `intelligenceSupportBonus` through the same dynasty-operation lane already used by AI retaliation and survives restore.
- Runtime verification now proves the player can raise counter-intelligence, intercept hostile espionage into a live dossier, launch dossier-backed sabotage, and preserve that operation through snapshot round-trip.

### 2026-04-15 Session 68 Additions

- Session 68 gave `Convergence` its own world-pressure runtime identity instead of treating it as only a redder form of `Severe`. A dominant realm at `Convergence` now exposes a shared escalation profile.
- That profile now sharpens rival attack, territory, sabotage, espionage, assassination, missionary, and holy-war tempo beyond the prior world-pressure branch.
- Frontier tribes now shorten raid cadence more aggressively under `Convergence`, and the raid message distinguishes true convergence pressure from lower escalation.
- The world pill now surfaces convergence sabotage, espionage, holy-war, and tribal-cadence pressure directly, and restore preserves that legibility through the same world-pressure state already kept in snapshot continuity.

### 2026-04-15 Session 69 Additions

- Session 69 advanced the house vector through Ironmark's dormant `axeman` lane. `axeman` is now a live prototype unit instead of disabled content.
- Barracks now includes `axeman` inside the shared roster, but simulation-side house gating remains authoritative, so only Ironmark can surface and queue it while Hartvale and Stonehelm stay locked out.
- Ironmark blood-production consequence is now unit-specific. Axeman training immediately consumes 2 living population and adds 3 blood-production load instead of using the lighter generic combat levy.
- The command panel now exposes blood levy and blood-production load honestly for trainable units, so the player can read the real cost of Ironmark escalation before queueing it.
- Runtime coverage now proves Ironmark-only access, elevated levy burden, snapshot exposure of the added blood-production load, and restore continuity of the queued Axeman plus resulting burden.

### 2026-04-15 Session 70 Additions

- Session 70 carried Ironmark's new house lane into AI awareness. Ironmark-controlled AI now recruits `axeman` through the same shared barracks gate the player already uses.
- AI now reads live Ironmark blood-production pressure before making that choice. When blood load is still below the growth-slowing threshold, Ironmark can spend blood to muster Axemen. When burden is already high, it falls back to Swordsmen instead of blindly deepening attritional strain.
- The message log now exposes both the blood-fueled Axeman muster and the burden-driven fallback, so the new AI behavior is visible through an already-live runtime surface.
- Runtime coverage now proves low-load Axeman recruitment, high-load fallback, off-house lockout, and restore continuity of the queued AI Axeman.

### 2026-04-15 Session 71 Additions

- Session 71 made world pressure name its cause instead of exposing only total score and severity. A new shared source-breakdown seam now resolves territorial expansion, off-home holdings, holy war, captives, hostile operations, and dark extremes into one live pressure profile.
- The world snapshot now exposes `topPressureSourceId`, `topPressureSourceLabel`, and `pressureSourceBreakdown`, so the reason a realm is under pressure is readable through existing state surfaces and stable through restore.
- Frontier tribes now react to that live cause. When off-home holdings are the dominant source, tribal raiders hard-prioritize off-home marches instead of merely converging on the pressured realm generically.
- The existing world pill and message log now surface that source-aware reaction directly, and runtime coverage proves off-home weighting, off-home targeting, legibility, and restore continuity.

### 2026-04-15 Session 72 Additions

- Session 72 carried the same source-aware world-pressure seam into rival-kingdom behavior. Stonehelm now reads the live pressure source and contests the cause of overextension instead of only accelerating generic territorial pressure.
- Enemy territorial target selection now hard-prioritizes player off-home marches when `offHomeHoldings` is the dominant world-pressure source, so continental breadth now draws direct hostile counterplay.
- The message log now exposes that redirect explicitly through an existing runtime surface, and restore-safe runtime coverage proves the same off-home counter-pressure reapplies after snapshot round-trip.

### 2026-04-15 Session 73 Additions

- Session 73 carried source-aware world pressure into the faith lane. Stonehelm now distinguishes holy-war-led pressure from generic pressure when deciding how quickly to answer through missionary and holy-war retaliation.
- Enemy missionary and holy-war timers now compress more sharply when `holyWar` is the dominant world-pressure source, and the message log surfaces both missionary backlash and counter-holy-war declaration explicitly.
- Restore-safe runtime coverage now proves holy-war source detection, sharper timer compression, live backlash launches, and preserved post-restore behavior.

### 2026-04-15 Session 74 Additions

- Session 74 carried source-aware world pressure into the covert lane. Stonehelm now distinguishes hostile-operations-led pressure from generic pressure when deciding how quickly to harden the court and retaliate through sabotage.
- Enemy counter-intelligence and sabotage timers now compress more sharply when `hostileOperations` is the dominant world-pressure source, and the message log surfaces both court hardening and retaliatory sabotage explicitly.
- Restore-safe runtime coverage now proves hostile-operations source detection, sharper covert-timer compression, live backlash launches, and preserved post-restore behavior.

### 2026-04-15 Session 75 Additions

- Session 75 carried source-aware world pressure into the dark-extremes lane. Stonehelm now distinguishes dark-extremes-led pressure from generic pressure when deciding how sharply to punish territory and bloodline leadership.
- Enemy attack, territory, and assassination timing now compress more sharply when `darkExtremes` is the dominant world-pressure source, and punitive territorial targeting now biases toward the weakest player-held march instead of only nearest hostile ground.
- Restore-safe runtime coverage now proves dark-extremes source detection, punitive weak-march targeting, live assassination backlash, and preserved post-restore behavior.

### 2026-04-15 Session 77 Additions

- Session 77 carried source-aware world pressure into broad territorial expansion. Tribes and Stonehelm now distinguish `territoryExpansion` from off-home breadth and generic pressure when choosing which marches to punish.
- Territorial-expansion backlash now targets stretched weak marches directly, using live loyalty, stabilization state, and contested status instead of only nearest-hostile movement.
- The world pill now surfaces explicit territorial-breadth contribution from the shared world-pressure source breakdown.
- Restore-safe runtime coverage now proves territory-expansion source detection, tribal and rival weakest-march targeting, message-log legibility, and preserved post-restore reapplication.

### 2026-04-15 Session 78 Additions

- Session 78 made Hartvale's already-live `verdant_warden` a true runtime support unit instead of only a house-gated roster entry.
- Verdant Warden presence now strengthens settlement defense through higher defender attack, faster keep-reserve healing, faster reserve muster, and a deeper desired frontline around the protected keep.
- The same support seam now strengthens local march stability through stronger loyalty recovery, softer loyalty erosion, and faster stabilization on supported territory.
- Stonehelm now recognizes Hartvale Warden-backed keeps as materially harder targets and delays an assault until escort mass is heavier, and the message log names that reason explicitly.
- The Loyalty and Fort dashboard pills now expose Verdant Warden supported-territory count, coverage, loyalty protection, loyalty growth, keep-defense count, attack bonus, reserve-heal bonus, and reserve-muster bonus.
- Restore-safe runtime coverage now proves Warden-backed reserve recovery, loyalty support, snapshot exposure, and post-restore reapplication.

### 2026-04-15 Session 79 Additions

- Session 79 made `scout_rider` live as the first honest stage-2 cavalry and infrastructure-raiding lane. Scout Riders now train from a real `stable`, carry live raid timing and loyalty-shock fields, and exist in runtime as more than dormant data.
- Scout Rider raids now disable hostile support infrastructure for a live duration, strip resource stores, shake nearby hostile march loyalty, and force retreat after the strike instead of acting as generic building attacks.
- Raided `supply_camp`, `well`, and other support structures now materially affect already-live systems: siege-logistics continuity, field-water sustainment, and gather-drop routing all degrade while the raid window is active.
- Stonehelm now builds `stable`, produces `scout_rider`, and launches the same raid lane through AI, so the new cavalry system is not player-only.
- The command surface and renderer now expose the cavalry lane honestly: `stable` is buildable, right-click raid orders are live for Scout Riders, raid cooldown is visible on the unit readout, raided buildings show active disable time and renderer overlay, and the logistics pill reports raid pressure directly.
- Browser verification also hardened the shell surface. `play.html` now declares an empty favicon so headless browser verification reaches zero failed requests.

### 2026-04-15 Session 80 Additions

- Session 80 carried `scout_rider` into direct economic warfare. Scout Riders now harry worked resource seams, route nearby hostile workers toward refuge, and reopen those seams only after the live harassment window clears.
- The same cavalry lane now depresses nearby hostile march loyalty around an active harried seam, so worker harassment touches territorial pressure instead of existing as isolated gather denial.
- Stonehelm now recognizes harried seams as live local pressure sites, redirects nearby defenders into a counter-raid response, and surfaces that response through the message log.
- The logistics pill now reports `nodes harried` and `workers routed`, and the new seam-harassment state survives snapshot restore together with the already-live cavalry cooldown lane.
- Scout Rider withdrawal is now honest hit-and-run behavior. A dedicated `raid_retreat` command prevents post-harass fallthrough into unintended immediate worker slaughter.

### 2026-04-15 Session 81 Additions

- Session 81 carried `scout_rider` into moving-logistics warfare. Scout Riders can now intercept hostile `supply_wagon` columns in motion instead of stopping at fixed structures and worked seams.
- Successful convoy strikes now impose a real interdiction window, strip live food, water, and wood stores, force the wagon into retreat toward a linked or fallback supply anchor, and shock nearby hostile march loyalty.
- Interdicted wagons now materially degrade already-live sustainment systems. Formal siege readiness now distinguishes active and interdicted wagons, and field-water support no longer counts a cut convoy as an active hydrator.
- Stonehelm now recognizes hostile convoys as live cavalry targets, treats struck wagons as pressure sites for local counter-screen response, and can pull the assault line back to protect a hit convoy before the breach resumes.
- The logistics pill now surfaces convoy cuts directly, and the new convoy-interdiction state survives snapshot restore with faction provenance intact.

### 2026-04-15 Session 82 Additions

- Session 82 made convoy escort discipline and post-interdiction reconsolidation live. Escort units now carry persistent `escortAssignedWagonId` binding. AI now differentiates between unscreened (pulls back to regroup) and screened (holds at siege stage point) recovering convoys. `readyForFormalAssault` now requires `convoyReconsolidated` rather than blanket zero-recovery. The logistics snapshot and dashboard pill now expose escorted wagon count, unscreened recovering count, and reconsolidation status. All new state survives snapshot export and restore.

### Immediate Continuation Direction

- Do not invent another non-settled house-specific unit lane. Hartvale is the only additional non-Ironmark house with a settled unique-unit seam already locked into canon.
- The cavalry and convoy escort chain is now live through escort discipline and reconsolidation readiness. Future deepening of this lane (player-side escort commands, multi-wagon formations) is optional.
- Session 82 doctrine ingestion is complete inside `02_SESSION_INGESTIONS/`, `11_MATCHFLOW/MATCH_STRUCTURE.md`, `01_CANON/CANON_LOCK.md`, and `04_SYSTEMS/SYSTEM_INDEX.md`.

### 2026-04-15 Session 83 Additions

- Session 83 made the first live match-progression layer real in browser runtime. The five-stage match now resolves from live founding, expansion, faith, territorial, military, rival-contact, and war-pressure conditions instead of remaining doctrine-only.
- `dualClock` and `matchProgression` now survive snapshot export and restore, and `getRealmConditionSnapshot` now exposes stage, phase, year, declaration count, readiness, and next-stage shortfalls through the existing dashboard lane.
- The Cycle pill and dashboard header now show stage and in-world time context, and the debug overlay now carries a dedicated match line for live inspection.
- Stonehelm now respects early-war stage gating for territorial rivalry and Scout Rider tempo while still allowing already-live escalation systems to override that restraint when the simulation is honestly in backlash or convergence.

### 2026-04-15 Session 84 Additions

- Session 84 made the first imminent-engagement warning layer live around threatened dynastic keeps. Hostile keep-approach windows now open real countdown state, surface watchtower coverage and hostile composition, and let the player commit reinforcements, change posture, recall commanders, and raise emergency bloodline guard.
- The same session made the first Stage 5 Divine Right declaration window live. Final Convergence kingdoms can now open a public declaration countdown gated by covenant commitment, Apex conviction, a living apex covenant structure, and global recognition share.
- Divine Right now contributes real world pressure, compresses rival coalition tempo, survives restore, and resolves into actual failure or victory instead of decorative doctrine text.
- The faith panel, cycle header, and debug overlay now surface Divine Right readiness plus active or incoming declaration windows, so the new late-game faith path is readable in the existing command surface.
- Match progression now treats an active Divine Right declaration as real sustained-war and final-convergence pressure, and the declaration-resolution seam no longer stalls after expiry.

### 2026-04-15 Session 85 Additions

- Session 85 made the first political-event architecture live through `Succession Crisis`. Dynasty state now carries active and historical political events instead of leaving that canonical lane as pure documentation.
- `Succession Crisis` now triggers from ruling bloodline death, reads interregnum depth, claimant pressure, and ruler maturity, applies immediate loyalty shock, then continues draining legitimacy and local loyalty while reducing resource trickle, population growth, stabilization, and battlefield attack output.
- The crisis now escalates over time if ignored, and the player can answer it through a real `Consolidate Succession` action that spends gold and influence, restores legitimacy and local loyalty, writes dual-clock declaration output, and moves the event into dynastic history.
- Stonehelm now exploits player succession instability through compressed military, territorial, and marriage tempo, while enemy courts also self-stabilize through the same consolidation seam when their own dynasty fractures.
- The dynasty panel, realm snapshot, and debug overlay now surface both player and rival succession-crisis state, and restore-safe runtime coverage now proves trigger, penalty, AI reaction, consolidation, and post-restore continuity.

### 2026-04-15 Session 86 Additions

- Session 86 made `Covenant Test` live as the second real political-event family on the new event spine. All four covenants and both doctrine paths now issue runtime mandates with real success or failure consequence instead of remaining canon-only.
- Completed covenant structures now feed live faith intensity, active Covenant Tests now impose real economy or stabilization or combat strain, and direct rite actions now resolve with real food or influence or population or legitimacy cost where canon supports them.
- `Apex Covenant` placement, stage-5 faith-unit recruitment, and the late-stage Divine Right ascent lane now require a passed Covenant Test instead of skipping the covenant-recognition seam.
- Stonehelm now climbs the covenant ladder through Wayshrine and Covenant Hall and Grand Sanctuary and Apex Covenant, recruits late faith units through that same live lane, and performs its own covenant rite when a direct mandate becomes actionable.
- The first live `Territorial Governance Recognition` layer is now in runtime. Final Convergence kingdoms can now trigger and sustain a recognition state from loyal stabilized holdings and real territory share, while world pressure and Stonehelm counterplay turn toward the weakest governed frontier.
- The dynasty panel, cycle header, debug overlay, and world-pressure detail lane now surface the governance-recognition state and outcome, and restore-safe runtime coverage now proves issuance, coalition backlash, establishment, collapse, and post-restore continuity.

### 2026-04-15 Graphics Lane Foundation

- A dedicated additive graphics lane now exists under `13_AUDIO_VISUAL/GRAPHICS_PIPELINE/`, `03_PROMPTS/GRAPHICS_PIPELINE/`, `14_ASSETS/GRAPHICS_PIPELINE/`, and the Unity staging surfaces under `unity/Assets/_Bloodlines/`.
- The lane includes a production-grade visual bible, House visual identity packs for all nine canonical founding Houses, a map and terrain visual doctrine, a production stage ladder, an approval and review system, and a machine-readable asset manifest suite for units, buildings, terrain and biomes, environment set pieces, and interface art.
- Unity-side visual ingestion and staging rules are now documented in `docs/unity/VISUAL_ASSET_PIPELINE_2026-04-15.md`, with matching local staging readmes under `unity/Assets/_Bloodlines/Docs/VisualProduction/`, `Art/Staging/`, `Materials/Staging/`, and `Prefabs/Staging/`.
- The graphics-lane report at `reports/2026-04-15_graphics_lane_foundation.md` records the audit, created folders, added docs, untouched workstreams, and graphics-only next steps.
- Audit result at lane creation time: the active canonical tree contained visual doctrine and Unity folder scaffolding but no governed active production art assets in the main Unity art, prefab, or material folders. Existing preserved image artifacts remain preserved and were not repurposed silently.
- This pass was non-destructive. Existing design work, lore work, systems work, runtime implementation, tests, and ongoing continuation files were preserved in place.

### 2026-04-15 Graphics Lane Batch 01

- The first actual staged visual assets now exist under `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/` as deterministic SVG concept sheets plus a local `preview.html`.
- Batch 01 covers a live-roster unit sheet, a core-structures building sheet, a shared resource and command marker sheet, and a founding-house heraldry candidate sheet. These are explicitly tagged `first_pass_concept` and `placeholder_only`; they are not approved-direction or final art.
- The same Batch 01 files were mirrored into `unity/Assets/_Bloodlines/Art/Staging/ConceptSheets/` so the Unity 6.3 LTS production track has an immediate, non-destructive visual-staging entry point.
- Because no built-in raster image generation tool was available in this session, the batch used SVG production sheets rather than claiming raster or 3D completion. This keeps the lane productive without inventing false-finish assets.
- The batch report lives at `reports/2026-04-15_graphics_lane_batch_01_first_pass_sheets.md`.

### 2026-04-16 Graphics Lane Batch 02

- Batch 02 extends the same staged visual lane under `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/` with four more deterministic SVG concept sheets: terrain and biome readability, fortification damage and breach states, House banner hierarchy, and bloodline-role portrait direction.
- The new banner sheet reuses canonical House names and palette direction from `data/houses.json`, and the portrait sheet reuses the current bloodline role set from `data/bloodline-roles.json`; neither source file was rewritten.
- The updated batch index and `preview.html` now surface all eight current concept sheets together.
- A Unity editor helper now exists at `unity/Assets/_Bloodlines/Code/Editor/GraphicsConceptSheetSync.cs`, adding `Bloodlines -> Graphics -> Sync Concept Sheets` as the governed mirror step into `unity/Assets/_Bloodlines/Art/Staging/ConceptSheets/`.
- Unity-side visual documentation now explicitly records that the current project manifest does not include `com.unity.vectorgraphics`, so mirrored `.svg` sheets remain staging review/reference material until raster export or explicit vector import approval.
- The Batch 02 report lives at `reports/2026-04-16_graphics_lane_batch_02_environment_banners_portraits.md`.
- This pass remained additive and non-destructive. Existing lore, systems, browser runtime, ECS work, tests, and continuation tracks were preserved in place.

### 2026-04-16 Graphics Lane Unity Vector Browser Ingest

- A dedicated approved Unity tooling pass is now complete for the staged SVG concept-sheet lane.
- `unity/Assets/_Bloodlines/Code/Editor/GraphicsConceptSheetVectorImport.cs` now supports `Bloodlines -> Graphics -> Sync And Rasterize Concept Sheets`, `Bloodlines -> Graphics -> Rasterize Concept Sheets`, and batch-safe execution methods for Unity command-line use.
- The preferred render path is now headless local browser export, which preserves the full sheet layout and text fidelity from the staged SVG boards. A Unity mesh raster fallback remains in place when browser export is unavailable.
- PNG review boards for all eight current concept sheets now exist under `unity/Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/` and remain staging-only review surfaces.
- `scripts/Invoke-BloodlinesUnityGraphicsRasterize.ps1` now provides a governed command-line entry point for future continuation sessions.
- The Unity-side pipeline now intentionally avoids the legacy external `com.unity.vectorgraphics` package because the dedicated tooling pass confirmed an assembly conflict against Unity 6.3's built-in vector module surface in this project.
- The Unity vector-ingest report lives at `reports/2026-04-16_graphics_lane_unity_vector_browser_ingest.md`.

### 2026-04-16 Graphics Lane Batch 03

- Batch 03 adds the first House-specific military silhouette treatment boards under `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/`.
- The four Houses covered in this pass are Ironmark, Stonehelm, Hartvale, and Trueborn, with Trueborn serving as the first settled-visual-only contrast sheet.
- These boards are explicitly framed as silhouette and material-direction studies, not gameplay-roster rewrites or new canon declarations.
- The batch updates the governed external review surface through `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/preview.html` and `INDEX.md`.
- The new source sheets were mirrored into `unity/Assets/_Bloodlines/Art/Staging/ConceptSheets/` for Unity-side continuation.
- A new report documents the pass at `reports/2026-04-16_graphics_lane_batch_03_house_silhouette_sheets.md`.
- The Unity raster lane remains available for these sheets, but a fresh batchmode pass was not forced during this step because the project was already open in another Unity instance and the graphics lane remained non-destructive.

### 2026-04-16 Graphics Lane Batch 04

- Batch 04 extends the graphics lane into fortification kit decomposition, cliff and shoreline transitions, resource-site ground wear, biome-edge blends, bridge or water-infrastructure readability, and the first logistics or setpiece prop lane.
- New source sheets now exist under `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/` for:
  - fortification kit decomposition
  - cliff and shoreline transitions
  - resource ground and edge blends
  - bridge and water infrastructure
  - logistics and setpieces
- The governed preview surface in `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/preview.html` and `INDEX.md` now includes Batch 04 alongside prior batches.
- The new source SVG files were mirrored into `unity/Assets/_Bloodlines/Art/Staging/ConceptSheets/`.
- The Unity 6.3 raster wrapper was then run successfully, producing PNG review boards for the new Batch 04 sheets under `unity/Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/`.
- Relevant raster validation logs now include `artifacts/unity-graphics-rasterize-batch-04.log`, `artifacts/unity-graphics-rasterize-batch-04b.log`, and `artifacts/unity-graphics-rasterize-batch-04c.log`.
- The Batch 04 report lives at `reports/2026-04-16_graphics_lane_batch_04_fortification_terrain_water_followups.md`.

### 2026-04-16 Graphics Lane Unity Runtime Readiness Continuation

- A new continuation audit confirmed the graphics lane is now strong on doctrine, manifests, House identity, concept-sheet staging, and Unity SVG-to-PNG review-board generation, but still partial on runtime-facing Unity grouping, scene-safe testbed planning, and family-level production references.
- New family-level graphics references now exist at:
  - `13_AUDIO_VISUAL/GRAPHICS_PIPELINE/UNIT_VISUAL_DIRECTION_PACKS_2026-04-16.md`
  - `13_AUDIO_VISUAL/GRAPHICS_PIPELINE/BUILDING_FAMILY_DIRECTION_PACKS_2026-04-16.md`
  - `13_AUDIO_VISUAL/GRAPHICS_PIPELINE/TERRAIN_AND_BIOME_DIRECTION_PACKS_2026-04-16.md`
  - `13_AUDIO_VISUAL/GRAPHICS_PIPELINE/VISUAL_REVIEW_MATRIX_2026-04-16.md`
- The prompt lane is now deeper for future production use through `03_PROMPTS/GRAPHICS_PIPELINE/EXPANDED_ASSET_PROMPT_PACKS_2026-04-16.md`.
- Unity 6.3 implementation guidance is now extended with:
  - `docs/unity/BLOODLINES_UNITY_6_3_VISUAL_IMPLEMENTATION_GUIDE_2026-04-16.md`
  - `docs/unity/BLOODLINES_VISUAL_TESTBED_PLAN_2026-04-16.md`
- A new Unity editor helper now exists at `unity/Assets/_Bloodlines/Code/Editor/GraphicsVisualTestbedBootstrap.cs`, adding `Bloodlines -> Graphics -> Create Visual Testbed Scene Shells` as the governed way to generate the first testbed scene shells under `unity/Assets/_Bloodlines/Scenes/Testbeds/`.
- A companion Unity editor helper now also exists at `unity/Assets/_Bloodlines/Code/Editor/GraphicsVisualTestbedPopulate.cs`, adding `Bloodlines -> Graphics -> Populate Visual Testbed Scenes` as the governed way to fill those scenes with generated placeholder comparisons and staging-board displays.
- A command-line wrapper for that populate step now exists at `scripts/Invoke-BloodlinesUnityGraphicsPopulateTestbeds.ps1`.
- Explicit runtime-facing Unity anchors now exist under:
  - `unity/Assets/_Bloodlines/Materials/Shared`, `Dynasties`, `Terrain`, `UI`, `FX`
  - `unity/Assets/_Bloodlines/Scenes/Testbeds/VisualReadability`, `TerrainLookdev`, `MaterialLookdev`, `IconLegibility`
  - approved runtime-facing art and prefab grouping subfolders under `unity/Assets/_Bloodlines/Art/` and `unity/Assets/_Bloodlines/Prefabs/`
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` was run after adding the new editor helper and passed with 0 errors. Existing `JsonContentImporter.cs` CS0649 warnings remain unchanged.
- The governed Unity batch bootstrap was then run successfully, creating:
  - `unity/Assets/_Bloodlines/Scenes/Testbeds/VisualReadability/VisualReadability_Testbed.unity`
  - `unity/Assets/_Bloodlines/Scenes/Testbeds/TerrainLookdev/TerrainLookdev_Testbed.unity`
  - `unity/Assets/_Bloodlines/Scenes/Testbeds/MaterialLookdev/MaterialLookdev_Testbed.unity`
  - `unity/Assets/_Bloodlines/Scenes/Testbeds/IconLegibility/IconLegibility_Testbed.unity`
- Relevant testbed bootstrap logs now include `artifacts/unity-graphics-testbed-bootstrap.log` and `artifacts/unity-graphics-testbed-bootstrap-pass2.log`.
- A first Unity batch attempt for the new testbed population step confirmed the execute method path but did not complete scene population before the editor pass ended.
- A follow-up pass was then blocked because another Unity instance already had `D:\ProjectsHome\Bloodlines\unity` open. The population tooling is ready, but the actual generated staging content still needs a lock-free Unity run.
- These additions remain organizational and documentation-facing only. No gameplay scenes, render-pipeline settings, ECS systems, canon files, runtime art assets, or approved production prefabs were overwritten.
- The continuation report for this pass lives at `reports/2026-04-16_graphics_lane_continuation_audit_and_unity_readiness.md`.

### 2026-04-16 Graphics Lane Batch 05

- Batch 05 completes the first full nine-House silhouette-study lane by adding Highborne, Goldgrave, Westland, Whitehall, and Oldcrest treatment boards under `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/`.
- The canonical file name for the Highborne sheet remains `bl_unit_highborne_house_military_silhouettes_sheet_a_first_pass_concept_v001.svg` for preservation reasons; the Unity tooling now maps the canonical `highborne` House id to that preserved filename without renaming or deleting the older artifact.
- The governed Unity raster lane was rerun successfully after the Batch 05 completion state was re-audited, so the full nine-House silhouette set now exists as generated PNG review boards under `unity/Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/`.
- The corresponding report remains `reports/2026-04-16_graphics_lane_batch_05_remaining_house_silhouette_sheets.md`.

### 2026-04-16 Graphics Lane Testbed Population Refresh

- The graphics-lane continuity audit confirmed that testbed population was no longer pending: the governed testbed scenes had already been populated on disk through the batch helper, and the stale continuity state was corrected rather than overwritten.
- `unity/Assets/_Bloodlines/Code/Editor/GraphicsVisualTestbedPopulate.cs` now drives a full nine-House readability grid in `VisualReadability_Testbed.unity` instead of the earlier partial-House layout.
- The governed testbed populate batch was rerun successfully and the log at `artifacts/unity-graphics-testbed-populate.log` confirms `Bloodlines visual testbed population complete.`
- The four governed testbed scenes now contain active tool-owned `GENERATED_TESTBED_CONTENT` roots and remain scene-safe staging surfaces only:
  - `unity/Assets/_Bloodlines/Scenes/Testbeds/VisualReadability/VisualReadability_Testbed.unity`
  - `unity/Assets/_Bloodlines/Scenes/Testbeds/TerrainLookdev/TerrainLookdev_Testbed.unity`
  - `unity/Assets/_Bloodlines/Scenes/Testbeds/MaterialLookdev/MaterialLookdev_Testbed.unity`
  - `unity/Assets/_Bloodlines/Scenes/Testbeds/IconLegibility/IconLegibility_Testbed.unity`

### 2026-04-16 Graphics Lane Batch 06

- Batch 06 extends the graphics lane into neutral settlement structures, faith structure families, civic support variants, shared foundation material boards, and House trim-family control boards.
- New Batch 06 source sheets now exist under `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/`:
  - `bl_building_shared_neutral_settlement_structures_sheet_a_first_pass_concept_v001.svg`
  - `bl_building_shared_faith_structure_families_sheet_a_first_pass_concept_v001.svg`
  - `bl_building_shared_civic_support_variants_sheet_a_first_pass_concept_v001.svg`
  - `bl_material_shared_foundation_boards_sheet_a_first_pass_concept_v001.svg`
  - `bl_material_house_trim_families_sheet_a_first_pass_concept_v001.svg`
- `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/INDEX.md` and `preview.html` now include all six current graphics batches.
- The five new source sheets were mirrored into `unity/Assets/_Bloodlines/Art/Staging/ConceptSheets/`.
- The governed Unity raster batch was rerun successfully and the log at `artifacts/unity-graphics-rasterize.log` confirms `Generated or updated: 5 | Skipped current: 22`.
- Batch 06 PNG review boards now exist under `unity/Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/`.
- `MaterialLookdev_Testbed.unity` now stages the new neutral-settlement, faith-structure, civic-support, shared-foundation, and House-trim boards through the governed populate helper.
- The report for this pass lives at `reports/2026-04-16_graphics_lane_batch_06_neutral_faith_civic_and_material_boards.md`.

### 2026-04-16 Graphics Lane Batch 07

- Batch 07 extends the graphics lane into market or storehouse or granary support structures, housing tiers, well and water-support infrastructure, dock or ferry or landing reads, and covenant-site progression.
- New Batch 07 source sheets now exist under `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/`:
  - `bl_building_shared_market_storehouse_and_granary_sheet_a_first_pass_concept_v001.svg`
  - `bl_building_shared_housing_tiers_sheet_a_first_pass_concept_v001.svg`
  - `bl_building_shared_well_and_water_support_sheet_a_first_pass_concept_v001.svg`
  - `bl_environment_shared_dock_ferry_and_landing_sheet_a_first_pass_concept_v001.svg`
  - `bl_building_shared_covenant_site_progression_sheet_a_first_pass_concept_v001.svg`
- `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/INDEX.md` and `preview.html` now include seven graphics batches.
- The five new source sheets were mirrored into `unity/Assets/_Bloodlines/Art/Staging/ConceptSheets/`.
- Another Unity instance had the canonical project open during this pass, so the governed Unity batch raster and populate wrappers could not take the project lock.
- The approved browser-first raster path was therefore used directly through local headless Edge export, and Batch 07 PNG review boards now also exist under `unity/Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/`.
- `unity/Assets/_Bloodlines/Code/Editor/GraphicsVisualTestbedPopulate.cs` now includes the support-structure board wall for `VisualReadability_Testbed.unity`, and the governed populate rerun has now applied it into the saved scene.
- The report for this pass lives at `reports/2026-04-16_graphics_lane_batch_07_settlement_support_and_water_followups.md`.

### 2026-04-16 Graphics Lane Batch 08

- Batch 08 extends the graphics lane into House overlay rules for support structures, deeper market and trade-yard variants, deeper storehouse and granary variants, denser housing cluster or courtyard compositions, and canonical covenant overlay architecture rules.
- New Batch 08 source sheets now exist under `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/`:
  - `bl_building_shared_house_overlay_support_structures_sheet_a_first_pass_concept_v001.svg`
  - `bl_building_shared_market_and_trade_yard_variants_sheet_a_first_pass_concept_v001.svg`
  - `bl_building_shared_storehouse_and_granary_variants_sheet_a_first_pass_concept_v001.svg`
  - `bl_building_shared_housing_cluster_and_courtyard_variants_sheet_a_first_pass_concept_v001.svg`
  - `bl_building_shared_covenant_overlay_architecture_variants_sheet_a_first_pass_concept_v001.svg`
- `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/INDEX.md` and `preview.html` now include eight graphics batches.
- The five new source sheets were mirrored into `unity/Assets/_Bloodlines/Art/Staging/ConceptSheets/`.
- The approved browser-first raster path was used again through local headless Edge export, and Batch 08 PNG review boards now also exist under `unity/Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/`.
- `unity/Assets/_Bloodlines/Code/Editor/GraphicsVisualTestbedPopulate.cs` now also includes the settlement-variant board wall for `MaterialLookdev_Testbed.unity`, and the governed populate rerun has now applied it into the saved scene.
- The report for this pass lives at `reports/2026-04-16_graphics_lane_batch_08_settlement_variants_and_covenant_overlays.md`.

### 2026-04-16 Graphics Lane Batch 08 Testbed Refresh And Review Registry

- The governed Unity populate path was rerun successfully through `scripts/Invoke-BloodlinesUnityGraphicsPopulateTestbeds.ps1`.
- `unity/Assets/_Bloodlines/Scenes/Testbeds/VisualReadability/VisualReadability_Testbed.unity` now contains the Batch 07 support-structure wall, including:
  - `MarketStorehouseGranaryBoard`
  - `HousingTiersBoard`
  - `WellAndWaterSupportBoard`
  - `DockFerryLandingBoard`
  - `CovenantSiteProgressionBoard`
- `unity/Assets/_Bloodlines/Scenes/Testbeds/MaterialLookdev/MaterialLookdev_Testbed.unity` now contains the Batch 08 settlement-variant wall, including:
  - `HouseOverlaySupportBoard`
  - `MarketTradeYardBoard`
  - `StorehouseGranaryVariantsBoard`
  - `HousingClusterCourtyardBoard`
  - `CovenantOverlayArchitectureBoard`
- A governed review ledger and machine-readable review registry now exist at:
  - `13_AUDIO_VISUAL/GRAPHICS_PIPELINE/GRAPHICS_BATCH_REVIEW_LEDGER_2026-04-16.md`
  - `13_AUDIO_VISUAL/GRAPHICS_PIPELINE/MANIFESTS/concept_batch_review_registry.json`
- These surfaces record the current truthful review state for Batches 01 through 08:
  - preserved
  - `placeholder_only`
  - `not_integration_ready`
  - Unity evidence and next action explicitly tracked
- The report for this follow-up lives at `reports/2026-04-16_graphics_lane_batch_08_testbed_refresh_and_review_registry.md`.

### Immediate Continuation Direction

- The next highest-leverage runtime follow-up is to deepen `Territorial Governance Recognition` into the first honest Territorial Governance victory-resolution seam by adding the missing governor-seat or Governor's House coverage, stronger no-war enforcement, anti-revolt validation, and final resolution logic.
- If the governor-coverage lane blocks cleanly, the next doctrine-to-runtime follow-up after that is alliance-threshold pressure and population-acceptance buildup as the parallel Stage 5 sovereignty path.
- If those lanes block, continue deeper world-depth follow-up through multi-kingdom pressure, neutral-power stage presence, or naval-world integration.

## Known Preserved Conflicts

- Older files still disagree on whether `deploy/bloodlines` or `bloodlines/` was canonical. Root governance resolves this in favor of `D:\ProjectsHome\Bloodlines`.
- `POLITICAL_EVENTS.md` exists in both `08_MECHANICS/` and `11_MATCHFLOW/`. Both are preserved.
- Prior house-profile material under voided CB004 remains preserved and non-canonical.
- Multiple design-bible and unified export snapshots remain preserved by design.

## What Remains Outside The Root

No confirmed Bloodlines-specific source root remains undocumented outside this root.

Compatibility and physical-backing paths still exist in the wider workspace, but they are governed by this root and are not to be treated as separate active project roots.

## Immediate Next Direction

- Continue the Unity production lane first. `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1`, `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1`, and `scripts/Invoke-BloodlinesUnitySyncJsonContent.ps1` are now all governed green entry points for the canonical scene and data lanes.
- Highest-leverage next action is now manual in-editor feel verification in `unity/Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity`, not more batch repair.
- Verify in Play Mode that:
  - the map bootstrap authoring path spawns live ECS faction, building, unit, resource-node, settlement, and control-point entities
  - the debug presentation bridge renders a visible first battlefield shell
  - the battlefield camera controller supports readable pan, rotate, zoom, and focus behavior
  - the debug command shell behaves correctly for unit select, building select, drag-box select, select-all, clear-selection, control groups, framing, formation-aware move, the first production panel, and queue-cancel buttons
  - `command_hall -> villager` two-deep queue issue, rear-entry cancel, refund, and surviving front-entry completion feel correct in-editor, not just in governed runtime smoke
  - control-point capture and uncontested territory trickle both resolve correctly in the same live shell
- After that first live shell is confirmed manually, deepen Unity through construction placement, broader production-roster verification, doctrine or governor yield-modifier parity, or richer production queue depth beyond the current two-deep rear-entry tail-cancel proof depending on whichever lane is least blocked.
- The first combat lane now exists; prioritize explicit attack orders or attack-move only after merge coordination and only as a follow-up to the current combat foundation rather than as a separate replacement lane.
- Treat the browser reference simulation as a frozen behavioral specification and feel reference only. Do not add new canonical systems there.
- If a task seems too large for one pass, choose the next concrete Unity 6.3 DOTS / ECS implementation slice that advances the full canonical vision. Do not reduce scope.
- Keep future design and implementation work aligned with the 2026-04-14 master doctrine, especially around population, water, logistics, naval strategy, and UI clarity.
- Keep future Bloodlines prompts, handoffs, research, and imported source material inside this root.
- Update continuity files at the end of each meaningful work block.

### 2026-04-17 Unity Attack Orders And Attack-Move (Session 126)

- The governed Unity combat command slice is now complete on `codex/unity-attack-move` and rebased onto current `master` at `548d7804ce55766420d75184385b3bedb739a3ee`.
- `unity/Assets/_Bloodlines/Code/Combat/AttackOrderComponent.cs` now carries the contract-shaped explicit-target and attack-move state fields while preserving compatibility with the older already-merged `IsAttackMoveDestination` naming.
- `unity/Assets/_Bloodlines/Code/Combat/AttackOrderResolutionSystem.cs` now resolves explicit attack orders and attack-move into the existing movement and attack-target flow before `AutoAcquireTargetSystem`, so commanded units chase out-of-range hostiles, fight when in range, and resume destination marching after local engagements.
- `unity/Assets/_Bloodlines/Code/Combat/AutoAcquireTargetSystem.cs` now honors active explicit targets first and otherwise preserves the prior nearest-hostile auto-acquire behavior.
- `unity/Assets/_Bloodlines/Code/Combat/DeathResolutionSystem.cs` now clears completed explicit orders on target death while leaving attack-move continuity intact.
- Because current `master` already contained the older merged `AttackOrderSystem`, that file was preserved in place and marked `[DisableAutoCreation]` so the rebased contract-shaped resolver is the only active attack-order system.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Combat.cs` now provides the governed debug APIs required by the contract without duplicating the already-merged `BloodlinesDebugCommandSurface.AttackOrders.cs` input surface.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesCombatSmokeValidation.cs` now proves four combat phases: melee, projectile, explicit attack order, and attack-move with a neutral decoy that must remain ignored.
- All required completion gates were rerun green on this branch tip:
  - `dotnet build unity/Assembly-CSharp.csproj -nologo`
  - `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
  - wrapper-locked combat smoke
  - wrapper-locked bootstrap runtime smoke
  - wrapper-locked canonical scene-shell validation
  - `node tests/data-validation.mjs`
  - `node tests/runtime-bridge.mjs`
- The new per-slice handoff lives at `docs/unity/session-handoffs/2026-04-17-unity-attack-orders-and-attack-move.md`.

### Immediate Next Direction Override (Session 126)

- Do not expand the attack-order branch further. This slice is intentionally stopped at merge coordination.
- The next action for this lane is merge coordination of `codex/unity-attack-move` onto current `master`, not stance or patrol expansion on the same branch.
- After merge coordination, any next combat follow-up should start from refreshed `master` and stay outside the out-of-scope list from the concurrent-session contract unless a new prompt changes that scope.

### 2026-04-17 Unity Target Acquisition Throttling And Sight Loss (Session 127)

- The next Codex combat follow-up is now complete on stacked branch `codex/unity-target-acquisition-los`, building directly on the finished `codex/unity-attack-move` slice while Claude handles merge coordination separately.
- `unity/Assets/_Bloodlines/Code/Combat/CombatStatsComponent.cs` now carries additive passive-target cadence and sight-retention state through `TargetAcquireIntervalSeconds`, `AcquireCooldownRemaining`, `TargetSightGraceSeconds`, and `TargetOutOfSightSeconds`.
- `unity/Assets/_Bloodlines/Code/Combat/AutoAcquireTargetSystem.cs` no longer scans every frame for units without a target. Passive auto-acquire now runs on a governed reacquire cadence while explicit attack orders remain untouched.
- `unity/Assets/_Bloodlines/Code/Combat/AttackResolutionSystem.cs` now clears stale passive or attack-move targets after a short sight-loss grace window, stops stale chase movement on target loss, and arms a reacquire cooldown before the next passive scan. Active explicit orders still preserve their commanded hostile beyond sight loss.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesCombatSmokeValidation.cs` now proves five combat phases: melee, projectile, explicit attack order, attack-move, and passive target-visibility plus reacquire throttling.
- All required completion gates were rerun green on this branch tip:
  - `dotnet build unity/Assembly-CSharp.csproj -nologo`
  - `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
  - wrapper-locked combat smoke
  - wrapper-locked bootstrap runtime smoke
  - wrapper-locked canonical scene-shell validation
  - `node tests/data-validation.mjs`
  - `node tests/runtime-bridge.mjs`
- The new per-slice handoff lives at `docs/unity/session-handoffs/2026-04-17-unity-target-acquisition-throttling-and-sight-loss.md`.
- The concurrent-session contract is now revision 2 so the stacked Codex combat lane is explicitly documented rather than inferred.

### Immediate Next Direction Override (Session 127)

- Claude or Lance should merge `codex/unity-attack-move` first.
- After that, rebase or merge `codex/unity-target-acquisition-los` onto the refreshed `master` tip.
- Do not widen the stacked target-acquisition branch into death presentation, renown or conviction kill hooks, or shared presentation work without a fresh contract lane assignment.

### 2026-04-18 Unity World Pressure Escalation (Session 128)

- `WorldPressureEscalationSystem` ported from `updateWorldPressureEscalation` / `applyWorldPressureConsequences` (simulation.js:13709, 13695) into Unity ECS on branch `claude/unity-world-pressure`.
- `WorldPressureComponent` (per-faction: Score, Streak, Level, Label, Targeted, TerritoryExpansionScore, GreatReckoningScore) and `WorldPressureCycleTrackerComponent` (singleton, 90s canonical cycle) live in `unity/Assets/_Bloodlines/Code/WorldPressure/`.
- Score/Targeted update every frame for HUD accuracy; Streak/Level/consequences gate behind 90-second `WorldPressureCycleTrackerComponent` accumulator. Dominant-leader check: score >= 4 AND strictly highest.
- Score sources ported: territoryExpansion = max(0, territories - 2), greatReckoning = 4 when GR target. Remaining sources deferred to later systems (divineRight, offHomeHoldings, holyWar, captives, hostileOps, darkExtremes).
- ApplyConsequences: lowest-loyalty CP loses [level] loyalty per cycle; DynastyStateComponent.Legitimacy -= (level - 1) when level >= 2.
- `MatchProgressionEvaluationSystem` stage 5 convergence now includes `playerWorldPressureConvergence` (Targeted && Level >= 3).
- `BloodlinesBootstrapRuntimeSmokeValidation` now includes `ProbeWorldPressureIntegrity` and reports `worldPressureChecked` in its diagnostics line.
- 4-phase smoke validator added: Phase 1 (tracker defaults), Phase 2 (score/expansion, no dominant), Phase 3 (dominant leader streak to Level 3), Phase 4 (loyalty drain consequence). All phases green.
- All 8 required gates green; contract bumped revision 8 -> 9, world-pressure-escalation lane retired.
- The per-slice handoff lives at `docs/unity/session-handoffs/2026-04-18-unity-world-pressure-escalation.md`.

### 2026-04-17 Unity Fortification Tier And Reserves (Fortification Lane Sub-slice 1, Codex)

- The first fortification lane sub-slice is now complete on `codex/unity-fortification-siege`; merged to master 2026-04-18.
- `unity/Assets/_Bloodlines/Code/Fortification/FortificationCanon.cs` now ports the browser fortification and siege constants block from `src/game/core/simulation.js:189-238`.
- `unity/Assets/_Bloodlines/Code/Components/FortificationComponent.cs` now carries settlement fortification tier / radius state plus a neutral faith-ward seam matching the browser ward-profile shape.
- `unity/Assets/_Bloodlines/Code/Components/FortificationReserveComponent.cs` now carries reserve muster / heal / threshold state and reserve-pool observability counts.
- `unity/Assets/_Bloodlines/Code/Fortification/AdvanceFortificationTierSystem.cs` now resolves fortification tier from completed linked fortification-contributing buildings and syncs the result onto `SettlementComponent`.
- `unity/Assets/_Bloodlines/Code/Fortification/FortificationReserveSystem.cs` now handles low-health retreat, triage healing, recovery-to-ready, and reserve muster-forward behavior for linked defenders in isolated ECS validation worlds.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Fortification.cs` now exposes governed tier, reserve-count, and force-muster debug APIs.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesFortificationSmokeValidation.cs` plus `scripts/Invoke-BloodlinesUnityFortificationSmokeValidation.ps1` now prove four phases: tier-0 baseline, tier advance, reserve muster after retreat, and reserve recovery back to ready.
- The per-slice handoff now lives at `docs/unity/session-handoffs/2026-04-17-unity-fortification-siege-fortification-tier-and-reserves.md`.

### 2026-04-18 Unity Siege Support And Field Water (Fortification Lane Sub-slice 2, Codex)

- The second fortification lane sub-slice is now complete on `codex/unity-fortification-siege`; merged to master 2026-04-18.
- `unity/Assets/_Bloodlines/Code/Siege/SiegeSupportCanon.cs` and `unity/Assets/_Bloodlines/Code/Siege/FieldWaterCanon.cs` now port the browser siege-support and field-water constants into governed Unity canon surfaces without retuning the canonical numbers.
- New runtime state now exists in:
  - `unity/Assets/_Bloodlines/Code/Components/SiegeSupportComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Components/FieldWaterComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Components/SiegeSupplyTrainComponent.cs`
- New siege systems now exist in:
  - `unity/Assets/_Bloodlines/Code/Siege/SiegeComponentInitializationSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Siege/SiegeSupportRefreshSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Siege/FieldWaterSupportScanSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Siege/FieldWaterStrainSystem.cs`
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Siege.cs` now exposes governed read surfaces for siege support, field-water state, and faction stockpiles.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesSiegeSmokeValidation.cs` plus `scripts/Invoke-BloodlinesUnitySiegeSmokeValidation.ps1` now prove four isolated ECS phases: baseline, strain, recovery, and siege-support cadence.
- The per-slice handoff now lives at `docs/unity/session-handoffs/2026-04-18-unity-fortification-siege-siege-support-and-field-water.md`.

### Immediate Next Direction (Post-merge, 2026-04-18)

- Sub-slice 3 (imminent-engagement warnings) is the remaining work on `codex/unity-fortification-siege`.
- Next implementation target: `tickImminentEngagementWarnings` from `src/game/core/simulation.js`.
- Keep bootstrap runtime smoke integration, siege operation lifecycle, and HUD strain readouts out of scope for the next pass unless Lance changes scope.
- Re-check `git branch --show-current` before any future commit or continuity edit. Branch drift was observed during wrapper execution in prior Codex sessions.

### 2026-04-18 Unity AI Strategic Layer Sub-Slice 2: Strategic Pressure System

- `AIStrategicPressureSystem` ported from ai.js `updateEnemyAi` lines 1127-1241 (timer-clamp and stage-gate block) into Unity ECS on branch `claude/unity-ai-strategic-pressure`.
- Ports the full timer-clamp logic: world pressure Level 1 caps AttackTimer<=14 and TerritoryTimer<=10; Level 2 caps AttackTimer<=8 and TerritoryTimer<=6; GreatReckoning (score>=4) caps AttackTimer<=6, TerritoryTimer<=4, RaidTimer<=8.
- Stage gates ported: RivalryUnlocked (stageNumber>=2 or wpLevel>0), RaidPressureUnlocked (stageNumber>=3 or wpLevel>0). Stage 1 floors TerritoryTimer>=24; stage 2 floors TerritoryTimer>=16; no pressure floors RaidTimer>=20.
- `AIStrategyComponent` extended with `RivalryUnlocked` and `RaidPressureUnlocked` bool fields.
- 4-phase smoke validator (`BloodlinesAIStrategicPressureSmokeValidation.cs`) and self-contained runner (`scripts/Invoke-BloodlinesUnityAIStrategicPressureSmokeValidation.ps1`) all green. Artifact: `artifacts/unity-ai-strategic-pressure-smoke.log`.
- All 10 validation gates green; contract bumped revision 13 -> 14. `victory-conditions` and `tier2-batch-dynasty-systems` lanes retired.
- The per-slice handoff lives at `docs/unity/session-handoffs/2026-04-18-unity-ai-strategic-layer-sub-slice-2-pressure.md`.

### 2026-04-18 Unity AI Strategic Layer Sub-Slice 3: Governance and Event-Context Pressure

- Governance/event-context timer clamp block from ai.js `updateEnemyAi` lines 1129-1215 ported into `AIStrategicPressureSystem` as the `ApplyGovernancePressure` static method.
- 20 flag fields added to `AIStrategyComponent` covering holy war, player governance recognition (victory/alliance pressure), player covenant test, player divine right, player/enemy succession crisis, enemy covenant test, enemy governance recognition. `BuildTimer` countdown field also added.
- `BloodlinesAIGovernancePressureSmokeValidation` 4-phase validator all green: Phase 1 holy war clamps, Phase 2 governance victory pressure, Phase 3 succession crisis player side, Phase 4 enemy governance floor boosts.
- All 10 validation gates green; contract bumped revision 14 -> 15.
- The per-slice handoff lives at `docs/unity/session-handoffs/2026-04-18-unity-ai-strategic-layer-sub-slice-3-governance.md`.

### 2026-04-18 Unity AI Strategic Layer Sub-Slice 4: Worker Gather Dispatch

- `AIWorkerGatherSystem` ported from ai.js idle-worker dispatch loop (lines 1243-1251), `getEnemyGatherPriorities` (885-922), `chooseGatherNode` (924-933) into Unity ECS on branch `claude/unity-ai-worker-gather`.
- Throttled by `WorkerGatherIntervalSeconds` accumulator on `AIStrategyComponent` (default 5s). Dispatch fires per faction when accumulator exceeds interval; resets to 0.
- Resource priority rotation: `workerIndex % 4` selects starting slot from `{gold, wood, stone, iron}`. Iterates from that slot forward (wrapping) until a non-depleted node is found.
- `PlayerKeepFortified` flag: when true, skips stone as a gather priority (mirrors ai.js fortified-keep defense posture block, lines 910-914).
- Nearest-node selection by Euclidean distance across all `ResourceNodeComponent` entities with `Amount > 0`.
- On dispatch: `WorkerGatherComponent.Phase = Seeking`, `AssignedNode`, `AssignedResourceId`, `CarryResourceId` set.
- `BloodlinesAIWorkerGatherSmokeValidation` 4-phase validator all green: Phase 1 idle->Seeking with gold node; Phase 2 all depleted stays Idle; Phase 3 Gathering not re-dispatched; Phase 4 two workers spread across gold and wood.
- All 10 validation gates green; contract bumped revision 15 -> 16.
- The per-slice handoff lives at `docs/unity/session-handoffs/2026-04-18-unity-ai-strategic-layer-sub-slice-4-worker-gather.md`.

### 2026-04-19 Unity AI Strategic Layer Sub-Slice 5: Siege Staging Orchestration

- `AISiegeOrchestrationSystem` + `AISiegeOrchestrationComponent` ported from ai.js attackTimer<=0 siege staging decision tree (lines ~1825-2090) on branch `claude/unity-ai-siege-staging`.
- `SiegeOrchestrationPhase` 17-value enum mirrors exact branch priority order from ai.js: Inactive, FieldWaterRetreat, SiegeRefusal, AwaitingEngineers, AwaitingSupplyCamp, AwaitingSupplyLine, SupplyInterdicted, SupplyRecoveringUnscreened, SupplyRecoveringScreened, AwaitingResupply, AwaitingEscort, StagingLines, ReliefHold, PostRepulse, RepeatedAssault, SupplyCollapse, Assaulting.
- `AISiegeOrchestrationComponent` holds all readiness/context flags: siege engine presence, engineering readiness, supply camp/line/supply collapse, field water crisis trio, escort threshold, formal siege line formation, relief army detection, cohesion penalty/post-repulse state.
- Pure state machine: reads `PlayerKeepFortified` from `AIStrategyComponent`, evaluates priority tree, writes `Phase`. Movement commands deferred to movement/combat systems.
- `BloodlinesAISiegeOrchestrationSmokeValidation` 6-phase validator all green: Phase 1 Inactive (keep not fortified); Phase 2 FieldWaterRetreat highest priority override; Phase 3 SiegeRefusal; Phase 4 StagingLines; Phase 5 Assaulting; Phase 6 PostRepulse cohesion penalty.
- All 10 validation gates green; contract bumped revision 16 -> 17.
- The per-slice handoff lives at `docs/unity/session-handoffs/2026-04-19-unity-ai-strategic-layer-sub-slice-5-siege-staging.md`.

### 2026-04-19 Unity AI Strategic Layer Sub-Slice 6: Dynasty Covert Ops Dispatch

- `AICovertOpsSystem` + `AICovertOpsComponent` ported from ai.js updateEnemyAi dynasty covert ops dispatch block (~lines 2419-2678) on branch `claude/unity-ai-dynasty-covert-ops`.
- Nine countdown timers: assassination (80s), missionary (70s), holy war (95s), divine right (140s), captive recovery (60s), marriage proposal (90s), marriage inbox (30s), pact proposal (120s), lesser-house promotion (60s). All defaults mirror ai.js canonical values.
- `CovertOpKind` 10-value enum: None, Assassination, Missionary, HolyWar, DivineRight, CaptiveRescue, CaptiveRansom, MarriageProposal, MarriageInboxAccept, PactProposal, LesserHousePromotion.
- `AICovertOpsSystem` three phases per entity per frame: TickTimers (decrement by deltaTime), ApplyPressureCaps (DarkExtremes/WorldPressureLevel/convergence/HolyWarFocused/DivineRight/CaptivesSourceFocused pressure-cap chain via math.min), TryFireOps (fire first expired timer whose gate conditions are met, write LastFiredOp, reset timer, return).
- Captive branch: HighPriorityCaptive or EnemyIsHostileToPlayer yields CaptiveRescue; otherwise CaptiveRansom. Pact branch: shouldPropose when succession pressure or army count <= 3 or governance near victory.
- `BloodlinesAICovertOpsSmokeValidation` 5-phase validator all green: Phase 1 Assassination, Phase 2 Missionary, Phase 3 HolyWar, Phase 4 PactProposal, Phase 5 CaptiveRescue.
- Note: Firing timers must be seeded at -1f (already expired) in test worlds; deltaTime=0 means 0.001f timers never expire.
- All 10 validation gates green; contract bumped revision 17 -> 18.
- The per-slice handoff lives at `docs/unity/session-handoffs/2026-04-19-unity-ai-strategic-layer-sub-slice-6-dynasty-covert-ops.md`.

### 2026-04-19 Unity AI Strategic Layer Sub-Slice 7: Build Order Priority Chain

- `AIBuildOrderSystem` + `AIBuildOrderComponent` ported from ai.js updateEnemyAi buildTimer<=0 block (~lines 1377-1573) on branch `claude/unity-ai-build-timer-chain`.
- 13-step build priority chain preserves ai.js if-else order exactly: (1) Barracks, (2) Wayshrine (faith faction + barracks), (3) Quarry (keep fortified + barracks), (4) IronMine (keep fortified + barracks), (5) SiegeWorkshop (keep fortified + barracks + quarry + iron mine), (6) CovenantHall (faith + wayshrine completed + intensity >= 26), (7) GrandSanctuary (faith + covenant hall completed + urgency gate: intensity >= 48 OR active covenant test OR player covenant active OR player divine right active), (8) ApexCovenant (faith + covenant test passed + grand sanctuary completed + intensity >= 80), (9) SupplyCamp (keep fortified + siege workshop completed), (10) Stable (keep fortified + supply camp completed + engineer corps + supply wagon), (11) Dwelling (population cap <= 1 + houses < 4), (12) Farm (food < total+4 + farms < 3), (13) Well (water < total+4 + wells < 3).
- `BuildOrderKind` 14-value enum: None, Barracks, Wayshrine, Quarry, IronMine, SiegeWorkshop, CovenantHall, GrandSanctuary, ApexCovenant, SupplyCamp, Stable, Dwelling, Farm, Well.
- Timer source: reads `AIStrategyComponent.BuildTimer` (decremented by `AIStrategicPressureSystem`). On fire, writes first matching branch to `NextBuildOp` and resets `BuildTimer = PlayerKeepFortified ? 4f : 6f` matching ai.js line 1573.
- Faith intensity thresholds canonical: 26 (covenant hall), 48 (grand sanctuary), 80 (apex covenant).
- Scheduling and decision logic only; actual `attemptPlaceBuilding` call is deferred to a later economy/placement integration pass. Same deferred pattern as sub-slice 6.
- `BloodlinesAIBuildOrderSmokeValidation` 5-phase validator all green: Phase 1 Barracks, Phase 2 Wayshrine, Phase 3 SiegeWorkshop, Phase 4 Dwelling, Phase 5 Farm.
- Timer seeding note: test worlds run world.Update() with deltaTime=0; AIStrategicPressureSystem applies a 0.016f floor; firing timers seeded at -1f become -1.016f, still <= 0 at the build-order check.
- All 10 validation gates green; contract bumped revision 18 -> 19.
- The per-slice handoff lives at `docs/unity/session-handoffs/2026-04-19-unity-ai-strategic-layer-sub-slice-7-build-order.md`.

### 2026-04-19 Unity AI Strategic Layer Sub-Slice 8: Marriage Proposal Execution

- `AIMarriageProposalExecutionSystem` ported from ai.js `tryAiMarriageProposal` (~2897-2944) on branch `claude/unity-ai-marriage-proposal-execution`.
- Consumes `AICovertOpsComponent.LastFiredOp == CovertOpKind.MarriageProposal` written by sub-slice 6 and runs the four browser abort gates in order: already-married between source and target factions (`MarriageComponent.Dissolved == false` match either direction); already-pending proposal from source to target (`MarriageProposalComponent.Status == Pending` match); no non-head active/ruling source-member candidate; no non-head active/ruling target-member candidate.
- On pass: creates a `MarriageProposalComponent` entity with ProposalId, source/target faction and member ids, `Status = Pending`, `ProposedAtInWorldDays = DualClockComponent.InWorldDays`, `ExpiresAtInWorldDays = now + 30` (pulls `ExpirationInWorldDays` from `MarriageProposalExpirationSystem` so the window stays synchronized).
- Target faction hardcoded to `"player"` matching the browser convention; multi-faction extension is reserved for a later slice.
- Always clears `LastFiredOp` back to `None` after processing (pass or fail), matching the browser's single-fire-per-timer semantic.
- Executes after `AICovertOpsSystem` in `SimulationSystemGroup`. Consumes signal in the same frame it was written to avoid multi-frame races.
- Cross-lane reads only: `MarriageComponent` / `MarriageProposalComponent` (tier2-batch-dynasty-systems lane, retired), `DynastyMemberComponent` / `DynastyMemberRef` (dynasty-core lane, retired), `DualClockComponent.InWorldDays` (dual-clock-match-progression lane, retired). No structural edits to those lanes' code.
- `BloodlinesAIMarriageProposalExecutionSmokeValidation` 5-phase validator all green: Phase 1 clean-path creation; Phase 2 blocked by prior marriage; Phase 3 blocked by pending proposal; Phase 4 blocked by missing target dynasty members; Phase 5 no-op when LastFiredOp is None.
- Strategic profile gating (faith hostility, population deficit, legitimacy distress, succession crises) is responsibility-split: sub-slice 6 gates dispatch via `MarriageStrategicProfileWilling`, sub-slice 8 gates execution structurally. Strategic profile port is a separate future slice.
- All 10 validation gates green; contract bumped revision 19 -> 20.
- The per-slice handoff lives at `docs/unity/session-handoffs/2026-04-19-unity-ai-strategic-layer-sub-slice-8-marriage-proposal-execution.md`.

### 2026-04-19 Unity AI Strategic Layer Sub-Slice 9: Marriage Inbox Accept

- `AIMarriageInboxAcceptSystem` ported from ai.js `tryAiAcceptIncomingMarriage` (~2880-2895) on branch `claude/unity-ai-marriage-inbox-accept`. Completes the mechanical marriage loop now that both proposal-side (sub-slice 8) and accept-side are Unity-resident.
- Consumes `AICovertOpsComponent.LastFiredOp == CovertOpKind.MarriageInboxAccept` written by sub-slice 6; scans `MarriageProposalComponent` entities for the first match with `Status == Pending`, `SourceFactionId == "player"`, `TargetFactionId == this AI faction`.
- On match: flips proposal `Status` to `Accepted`; creates primary + mirror `MarriageComponent` entities sharing `MarriageId` (primary = source-headed with `IsPrimary = true`; mirror = target-headed with `IsPrimary = false`). `MarriageGestationSystem` filters on `IsPrimary` so only one child spawns at 60 in-world days from the `GestationInWorldDays` constant.
- Always clears `LastFiredOp` to `None` after processing, matching the browser's single-fire-per-timer semantic.
- Source hardcoded to `"player"` matching the browser's inbox filter; multi-faction extension reserved alongside sub-slice 8's proposal-side extension.
- Executes after `AIMarriageProposalExecutionSystem` in `SimulationSystemGroup` so propose + accept cannot race on the same frame when both dispatch in the same tick.
- Cross-lane reads only: `MarriageProposalComponent` / `MarriageComponent` (tier2-batch-dynasty-systems lane, retired), `MarriageGestationSystem.GestationInWorldDays` constant, `DualClockComponent.InWorldDays`.
- Mechanical record creation only. Browser effects on accept that remain unported in this slice: governance authority legitimacy cost, hostility drop between factions, oathkeeping conviction events, legitimacy +2 both sides, 30-day declareInWorldTime jump, narrative message push. These remain for a dedicated effects slice.
- `BloodlinesAIMarriageInboxAcceptSmokeValidation` 4-phase validator all green: Phase 1 dispatch+pending->accepted creates primary+mirror; Phase 2 dispatch+no proposal no-op; Phase 3 dispatch+only-expired no-op with expired preserved; Phase 4 no-dispatch+pending no-op.
- All 10 validation gates green (required one bootstrap retry and one siege retry due to transient bee_backend/Library write-contention; both passed cleanly on second attempt). Contract bumped revision 20 -> 21.
- The per-slice handoff lives at `docs/unity/session-handoffs/2026-04-19-unity-ai-strategic-layer-sub-slice-9-marriage-inbox-accept.md`.

### 2026-04-19 Unity AI Strategic Layer Sub-Slice 8 Command Dispatch Rebase To Revision 22

- `codex/unity-ai-command-dispatch` is now rebased onto `origin/master`
  `00847e77ab8d7e085adcbf5de4ac10baa584bcba`, preserving Claude's merged
  ai-strategic-layer sub-slices 8 and 9 as the master-side base.
- `docs/unity/CONCURRENT_SESSION_CONTRACT.md` advanced from revision 21 to 22 under
  `codex-ai-command-dispatch-2026-04-19`, layering the command-dispatch ownership
  (`AIWorkerCommandSystem`, `AITerritoryDispatchSystem`, `WorkerGatherOrderComponent`,
  `BloodlinesAICommandDispatchSmokeValidation`, and `WorkerGatherSystem` arrival
  gating) on top of the rev-21 contract.
- Rebased validation is green on `D:\BLAICD\bloodlines`:
  - `dotnet build unity/Assembly-CSharp.csproj -nologo`
  - `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
  - bootstrap runtime smoke PASS via `artifacts/unity-bootstrap-runtime-smoke.log`
  - combat smoke PASS
  - Bootstrap scene shell PASS via `artifacts/unity-bootstrap-scene-batch-rev22.log`
  - Gameplay scene shell PASS via `artifacts/unity-gameplay-scene-batch-rev22.log`
  - fortification smoke PASS
  - siege smoke PASS
  - `node tests/data-validation.mjs` PASS
  - `node tests/runtime-bridge.mjs` PASS
  - contract staleness check PASS at revision 22
- Dedicated `BloodlinesAICommandDispatchSmokeValidation` rerun PASS on the rebased
  head via `artifacts/unity-ai-command-dispatch-batch-rev22.log`.
- Update-order discipline preserved: both new systems remain
  `[UpdateAfter(typeof(AICovertOpsSystem))]`.
- Rebasing produced branch head `34b8d694c4726345a63ce1577d63e34d4bdce5e1`; the
  slice is ready for force-with-lease push and merge to `master`.

### 2026-04-19 Unity Fortification Siege Sub-Slice 3 Rebase To Revision 23

- `codex/unity-fortification-siege` is now rebased onto `origin/master`
  `73a6f5435c047c303c256a822b5f785f0199d277`, which already includes the
  revision-22 Codex command-dispatch merge.
- `docs/unity/CONCURRENT_SESSION_CONTRACT.md` advanced from revision 22 to 23
  under `codex-fortification-siege-imminent-engagement-2026-04-19`, preserving
  the rev-22 ai-strategic-layer base and adding the imminent-engagement
  component, smoke validator, and wrapper ownership to the fortification lane.
- Full validation is green on `D:\BLFS\bloodlines`:
  - `dotnet build unity/Assembly-CSharp.csproj -nologo`
  - `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
  - bootstrap runtime smoke PASS via `artifacts/unity-bootstrap-runtime-smoke.log`
  - combat smoke PASS via `artifacts/unity-combat-smoke.log`
  - Bootstrap scene shell PASS via `artifacts/unity-bootstrap-scene-batch-rev23.log`
  - Gameplay scene shell PASS via `artifacts/unity-gameplay-scene-batch-rev23.log`
  - fortification smoke PASS via `artifacts/unity-fortification-smoke.log`
  - siege smoke PASS via `artifacts/unity-siege-smoke.log`
  - imminent engagement smoke PASS via `artifacts/unity-imminent-engagement-smoke.log`
  - `node tests/data-validation.mjs` PASS
  - `node tests/runtime-bridge.mjs` PASS
  - contract staleness check PASS at revision 23
- The authoritative handoff for this rebased slice is now
  `docs/unity/session-handoffs/2026-04-18-unity-fortification-siege-imminent-engagement-warnings.md`.
- Next action on this branch is operational, not implementation:
  force-push `codex/unity-fortification-siege`, merge it to `master`, then push
  `master` so the next Claude ai-strategic-layer session can start from
  revision 23.

### 2026-04-19 Unity AI Strategic Layer Sub-Slice 10: Marriage Strategic Profile

- `AIMarriageStrategicProfileSystem` ported from ai.js `getAiMarriageStrategicProfile` (~2803-2839) on branch `claude/unity-ai-marriage-strategic-profile`.
- Runs [UpdateInGroup(SimulationSystemGroup), UpdateBefore(AICovertOpsSystem)]. Each tick snapshots the player faction (population, selectedFaith, doctrinePath) once, then iterates every non-player faction with AICovertOpsComponent + AIStrategyComponent.
- Computes 4 strategic signals matching browser: isHostile (scan HostilityComponent buffer for "player"), populationDeficit (aiPop > 0 AND aiPop < playerPop * 0.85), legitimacyDistress gated by faith compatibility (<50 threshold, canonical AI_MARRIAGE_LEGITIMACY_DISTRESS_THRESHOLD), successionPressure (Enemy or Player SuccessionCrisisActive).
- Faith compatibility simplified from the browser's covenantName string grouping to SelectedFaith+DoctrinePath equality: unbound (either side uncommitted) is neutral, same covenant+same doctrine is harmonious, either-match is non-blocking, fully-different covenant+doctrine blocks weak matches. Mechanical intent preserved without requiring Unity-side covenant-name metadata.
- signalCount = isHostile + populationDeficit + faithBackedLegitimacySignal + successionPressure. blockedByFaith = compat.BlocksWeakMatch && signalCount < 2. willing = signalCount > 0 && !blockedByFaith.
- Writes `willing` to both `AICovertOpsComponent.MarriageProposalGateMet` and `MarriageInboxAcceptGate` (symmetric per ai.js:2630-2631 "accept gate reuses strategic criteria so AI behavior is symmetric"). AICovertOpsSystem was already reading these gates; this slice is the first to populate them based on live state.
- Target hardcoded to `"player"` matching browser signature; multi-faction extension reserved.
- Cross-lane reads only: PopulationComponent, FaithStateComponent, DynastyStateComponent, HostilityComponent buffer, AIStrategyComponent succession flags. No structural edits.
- `BloodlinesAIMarriageStrategicProfileSmokeValidation` 6-phase validator all green: Phase 1 no signals unbound faith -> not willing; Phase 2 hostility-only -> willing; Phase 3 population-deficit-only -> willing; Phase 4 legitimacy+harmonious-faith -> willing; Phase 5 succession-crisis-only -> willing; Phase 6 single-signal+incompatible-faith blocks weak match -> not willing.
- All 10 validation gates green. Contract bumped revision 23 -> 24.
- The per-slice handoff lives at `docs/unity/session-handoffs/2026-04-19-unity-ai-strategic-layer-sub-slice-10-marriage-strategic-profile.md`.

### 2026-04-19 Unity AI Strategic Layer Sub-Slice 11: Marriage Accept Effects

- `AIMarriageAcceptEffectsSystem` + `MarriageAcceptEffectsPendingTag` ported from simulation.js `acceptMarriage` (~7388-7469) post-record effects block on branch `claude/unity-ai-marriage-accept-effects`. Sub-slice 9 deferred these effects; sub-slice 11 now applies them via a tag-driven one-shot pipeline.
- `AIMarriageInboxAcceptSystem` (sub-slice 9) now attaches `MarriageAcceptEffectsPendingTag` to the primary marriage entity at creation. Mirror record remains untagged to prevent double application.
- `AIMarriageAcceptEffectsSystem` runs [UpdateInGroup(SimulationSystemGroup), UpdateAfter(AIMarriageInboxAcceptSystem)]. Queries MarriageComponent + MarriageAcceptEffectsPendingTag, applies four browser effects exactly once per accepted marriage, then removes the tag.
- Effect 1: Legitimacy +2 on both HeadFaction and SpouseFaction DynastyStateComponents, clamped to `min(100, legitimacy + 2)`.
- Effect 2: Hostility drop. Iterates HostilityComponent buffer back-to-front on both factions and removes entries where HostileFactionId matches the other side.
- Effect 3: 30-day DeclareInWorldTimeRequest push onto DualClock singleton buffer with reason `"Marriage <marriageId>"`. DualClockDeclarationSystem drains the buffer and applies the jump.
- Effect 4: Oathkeeping conviction +2 on both factions via `ConvictionScoring.ApplyEvent(ref conviction, ConvictionBucket.Oathkeeping, +2f)`. The helper refreshes ConvictionComponent.Score and Band in place.
- All effects use null-safe skip paths (HasComponent / HasBuffer checks) so bootstraps that don't seed every component don't throw.
- Cross-lane mutations are field-level only, no schema changes: DynastyStateComponent.Legitimacy (dynasty-core lane), HostilityComponent buffer entries (combat-and-projectile lane), ConvictionComponent via ConvictionScoring helper (conviction-scoring lane), DualClock singleton buffer push (dual-clock-match-progression lane). Reuses existing helpers and established buffer patterns.
- Deferred to future slices: governance-authority legitimacy cost on accept (requires `getMarriageAcceptanceTerms` port), narrative message push (no AI-to-UI message component wired yet).
- `BloodlinesAIMarriageAcceptEffectsSmokeValidation` 6-phase validator all green: Phase 1 full accept pipeline legitimacy +2 + hostility drop + tag removal; Phase 2 legitimacy ceiling clamped to 100; Phase 3 untagged marriage ignored; Phase 4 full end-to-end pipeline; Phase 5 DeclareInWorldTimeRequest enqueued with DaysDelta=30; Phase 6 oathkeeping +2 + score refresh.
- All 10 validation gates green. Contract bumped revision 24 -> 25.
- The per-slice handoff lives at `docs/unity/session-handoffs/2026-04-19-unity-ai-strategic-layer-sub-slice-11-marriage-accept-effects.md`.

### 2026-04-19 Unity AI Strategic Layer Sub-Slice 12: Marriage Acceptance Terms

- `MarriageAcceptanceTermsComponent` + `MarriageAuthorityEvaluator` + extensions to `AIMarriageInboxAcceptSystem` and `AIMarriageAcceptEffectsSystem` ported from simulation.js `getMarriageAcceptanceTerms` (~6327), `applyMarriageGovernanceLegitimacyCost` (~6232), and `getMarriageAuthorityProfile` (~6134) on branch `claude/unity-ai-marriage-acceptance-terms`. Sub-slice 11 explicitly deferred this; sub-slice 12 makes the AI marriage accept path browser-accurate end-to-end at the legitimacy/conviction level.
- `MarriageAuthorityEvaluator` resolves the target faction's authority by walking its `DynastyMemberRef` buffer in browser priority order: HeadDirect (head_of_bloodline + Ruling, cost 0), HeirRegency (heir_designate available, cost 1), EnvoyRegency (diplomat available, cost 2). Returns false (reject) only when the roster exists but yields none of those. Backward-compatibility default: when the faction has no `DynastyMemberRef` buffer at all, returns HeadDirect with cost 0 so sub-slice 11 synthetic test fixtures stay green.
- `AIMarriageInboxAcceptSystem` calls `MarriageAuthorityEvaluator.TryResolve` before `TryAcceptIncoming`. If false, it clears the dispatch and skips record creation, matching browser `getMarriageAcceptanceTerms` rejection. On success it attaches `MarriageAcceptanceTermsComponent { AuthorityMode, LegitimacyCost }` to the primary marriage entity alongside the existing pending tag.
- `AIMarriageAcceptEffectsSystem` reordered to match browser order (cost first, then hostility, then oathkeeping, then legitimacy +2, then declare time). New helper `ApplyAuthorityLegitimacyCost` reads the terms component, deducts cost from spouse `DynastyStateComponent.Legitimacy` clamped [0, 100], and records a Stewardship -cost conviction event via `ConvictionScoring.ApplyEvent` clamped at 0. Skips silently when terms component is absent so the sub-slice 11 phase-3 untagged synthetic marriage path still works.
- Cost-before-bonus arithmetic: net spouse legitimacy = min(100, max(0, L - cost) + 2). For cost 0 (head-direct) the result equals sub-slice 11's +2 baseline; for cost 1/2 the +2 bonus partially absorbs the cost when below the ceiling.
- `MarriageAcceptanceTermsComponent` persists on the primary marriage entity after the pending tag is removed, providing a durable provenance marker for downstream systems and HUD surfaces.
- Cross-lane reads only (no structural edits): `DynastyMemberRef` buffer + `DynastyMemberComponent.Role/Status` (dynasty-core lane). Cross-lane mutations are field-level only: `DynastyStateComponent.Legitimacy` clamped [0, 100], `ConvictionComponent.Stewardship` via the existing scoring helper.
- Deferred to future slices: narrative message push (no AI-to-UI message component wired yet, same blocker as sub-slice 11).
- `BloodlinesAIMarriageAcceptanceTermsSmokeValidation` 5-phase validator all green: PhaseHeadDirect cost 0 yields legitimacy 75->77 stewardship unchanged at 4; PhaseHeirRegency cost 1 yields 75->76 stewardship 4->3; PhaseEnvoyRegency cost 2 yields 75->75 stewardship 4->2; PhaseNoAuthority rejects accept and keeps proposal pending and dispatch cleared; PhaseTermsPersisted retains the terms component as provenance after effects pass.
- Sub-slice 11 regression smoke (`Invoke-BloodlinesUnityAIMarriageAcceptEffectsSmokeValidation.ps1`) re-run: all 6 phases PASS. Backward-compat default keeps the synthetic fixtures aligned with the original sub-slice 11 expectations.
- All 10 validation gates green. Contract bumped revision 25 -> 26.
- The per-slice handoff lives at `docs/unity/session-handoffs/2026-04-19-unity-ai-strategic-layer-sub-slice-12-marriage-acceptance-terms.md`.

### 2026-04-19 Unity Fortification Siege Sub-Slice 4: Camp Supply Interdiction

- `SiegeSupplyCampComponent`, `SiegeSupplyInterdictionCanon`, and `SiegeSupplyInterdictionSystem` port the browser convoy-interdiction seam on branch `codex/unity-fortification-siege-camp-supply-interdiction`.
- Logistics-support buildings now seed live camp stockpiles and raider-tracking state. Hostile raiders inside the supply-line radius drain camp stockpile, preserve attacker ownership, and mark camps non-operational below threshold.
- Supply wagons now carry additive interdiction and convoy-recovery state: `LogisticsInterdictedUntil`, `ConvoyRecoveryUntil`, `ConvoyReconsolidatedAt`, `InterdictedByFactionId`, `EscortCount`, `RequiredEscortCount`, and `EscortScreened`.
- `SiegeSupportRefreshSystem`, `FieldWaterSupportScanSystem`, and `FieldWaterStrainSystem` now respect interdicted and recovering wagons plus non-operational camps, so siege support and field-water sustainment fall off cleanly during interdiction instead of silently persisting.
- `BloodlinesDebugCommandSurface.Siege` now exposes dedicated wagon and camp logistics snapshots for debugger and smoke use.
- New dedicated validator `BloodlinesSiegeSupplyInterdictionSmokeValidation` is green across five phases: baseline readiness, active interdiction, unscreened recovery, screened recovery, and camp recovery back above the operational threshold.
- Full 10-gate validation is green on `D:\BLFS\bloodlines`:
  - `dotnet build unity/Assembly-CSharp.csproj -nologo`
  - `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
  - bootstrap runtime smoke PASS via `artifacts/unity-bootstrap-runtime-smoke.log`
  - combat smoke PASS via `artifacts/unity-combat-smoke.log`
  - Bootstrap scene shell PASS via `artifacts/unity-bootstrap-scene-batch-rev27-codex.log`
  - Gameplay scene shell PASS via `artifacts/unity-gameplay-scene-batch-rev27-codex.log`
  - fortification smoke PASS via `artifacts/unity-fortification-smoke.log`
  - siege smoke PASS via `artifacts/unity-siege-smoke.log`
- `node tests/data-validation.mjs` PASS
- `node tests/runtime-bridge.mjs` PASS
- contract staleness check PASS at revision 27
- Dedicated smoke PASS via `artifacts/unity-siege-supply-interdiction-smoke.log` with marker `BLOODLINES_SIEGE_SUPPLY_INTERDICTION_SMOKE PASS`.
- Contract advanced revision `26 -> 27` under `codex-fortification-siege-camp-supply-interdiction-2026-04-19`.
- The per-slice handoff lives at `docs/unity/session-handoffs/2026-04-19-unity-fortification-siege-camp-supply-interdiction.md`.

### 2026-04-19 Owner Direction Update: Game Modes And Dynasty Progression

- New owner direction file landed: `governance/OWNER_DIRECTION_2026-04-19_GAME_MODES_AND_DYNASTY_PROGRESSION.md`. Amends both 2026-04-16 and 2026-04-17 owner directions on three points without replacing them.
- Shipping game modes are now skirmish vs AI and multiplayer only. Story campaign and interactive tutorial mode are removed from scope. Older guidance that lists either as in-flight work is stale and should be treated as removed (not deferred). Onboarding remains in scope but lives in HUD tooltips and panel labels rather than a dedicated tutorial mode.
- Graphics fidelity ceiling stays at Generals Zero Hour (2003) / Warcraft III (2002) per the 2026-04-17 direction. Asset production is now explicitly AI-generated as the primary path, with delivered fidelity allowed to run somewhat below the ceiling. The 2026-04-17 "no PBR / no HDRP / no ray tracing / no AAA pipelines" rules remain in full effect.
- New canonical system in scope: a cross-match dynasty progression system. Top-performing dynasties (not strictly #1) accrue XP that unlocks tiers. Tier bonuses are sideways customization options (canonical example: swap one dynasty-specific special unit for another from the same house's progression options) rather than flat power upgrades, so non-#1 placements stay rewarding and multiplayer power gradients stay flat. Design surface and `data/` file location to be added when the design lands.
- `CLAUDE.md` updated to reference the new owner direction file in the Owner Direction Override section, amend the governing-implications list with the game-mode and progression bullets, and remove tutorial/campaign from the Current Direction polish list.
- `03_PROMPTS/BLOODLINES_UNITY_CONTINUITY_PROMPT_v3.md` Presentation block updated: UX line now reads "onboarding via HUD tooltips and panel labels, lobby, settings" with a note pointing to the 2026-04-19 owner direction; "tutorials" and "campaign framework" removed from the UX list.
- No code changes. No contract revision bump (the contract tracks Unity lane scope, not owner direction). No validation gate run required for governance-only edits.
- Implication for active and future Unity work: campaign-related backlog items are dropped without ceremony if any exist in older planning docs. Tutorial-mode design work is not pursued. The dynasty progression system is canonically in scope but not prioritized over the in-flight ai-strategic-layer or fortification-siege lanes; it gets its own slice plan when the design lands.

### 2026-04-19 Unity AI Strategic Layer Sub-Slice 13: Lesser-House Promotion Execution

- `AILesserHousePromotionSystem` ports ai.js `tryAiPromoteLesserHouse` (~2784-2801) plus the mechanical core of simulation.js `promoteMemberToLesserHouse` (~7184-7258) on branch `claude/unity-ai-lesser-house-promotion`. Sub-slice 6 already dispatches `CovertOpKind.LesserHousePromotion` into AICovertOpsComponent.LastFiredOp; sub-slice 13 consumes that dispatch and executes the founding effect.
- Runs `[UpdateInGroup(SimulationSystemGroup), UpdateAfter(AICovertOpsSystem)]`. Per faction with the matching dispatch: gates on `DynastyStateComponent.Legitimacy < 90` (consolidation ceiling) and `LesserHouseElement` buffer count `< 3` (cap), then walks the `DynastyMemberRef` buffer for the first eligible member and performs the founding write. Dispatch is unconditionally cleared, matching the one-shot pattern from sub-slices 8/9/12.
- Eligibility (browser memberIsLesserHouseCandidate parity): `Status == Active || Ruling`, `Role != HeadOfBloodline`, `Path` is one of `Governance`, `MilitaryCommand`, `CovertOperations`, `Renown >= 30`, MemberId not already a `FounderMemberId` of an existing LesserHouseElement on this faction. The browser's `member.foundedLesserHouseId` field is replaced by cross-referencing the buffer because Unity's DynastyMemberComponent does not have that field yet.
- On success appends a new `LesserHouseElement` with `Loyalty = 75` (canonical INITIAL_LOYALTY), `DailyLoyaltyDelta = 0` (recomputed by drift system), `LastDriftAppliedInWorldDays = currentInWorldDays` (so LesserHouseLoyaltyDriftSystem picks it up at the next in-world day boundary), `Defected = false`. Applies legitimacy `+3` clamped to `100` (LESSER_HOUSE_LEGITIMACY_BONUS) and records a Stewardship `+2` conviction event via `ConvictionScoring.ApplyEvent`.
- Cross-lane reads: DynastyMemberRef + DynastyMemberComponent.Role/Path/Status/Renown/MemberId/Title (dynasty-core, retired). Cross-lane mutations: DynastyStateComponent.Legitimacy clamp, LesserHouseElement buffer append (tier2-batch-dynasty-systems, retired), ConvictionComponent.Stewardship via the existing scoring helper (conviction-scoring, retired).
- Deferred: per-member FoundedLesserHouseId field on DynastyMemberComponent; promotion-history gate (LESSER_HOUSE_MIN_PROMOTIONS); marital-anchor and cadet world-pressure profiles on lesser-house records; narrative pushMessage (waits on the AI->UI message bridge, same blocker as sub-slices 11 and 12).
- `BloodlinesAILesserHousePromotionSmokeValidation` 6-phase validator all green: PhaseSuccessfulPromotion (legitimacy 60->63, stewardship 0->2, 1 lesser house created, dispatch cleared); PhaseHighLegitimacyBlocks (legitimacy 90 blocks); PhaseCapBlocks (3 existing lesser houses blocks); PhaseHeadOfBloodlineRejected (head excluded from candidate pool); PhaseNonQualifyingPathRejected (Diplomacy and EconomicStewardshipTrade paths blocked); PhaseNearCeilingPromotion (legitimacy 89 + 3 = 92, CovertOperations path eligible).
- All 10 validation gates green. Contract bumped revision 27 -> 28.
- Note on csproj refresh: the local Assembly-CSharp.csproj was missing entries for Codex's sub-slice 4 files (SiegeSupplyCampComponent.cs, SiegeSupplyInterdictionCanon.cs, SiegeSupplyInterdictionSystem.cs) and the editor csproj was missing BloodlinesSiegeSupplyInterdictionSmokeValidation.cs because Unity had not regenerated the project files in this worktree after the master-base switch. Both csproj files were updated as part of this slice; csproj remains gitignored and is not in the commit.
- The per-slice handoff lives at `docs/unity/session-handoffs/2026-04-19-unity-ai-strategic-layer-sub-slice-13-lesser-house-promotion.md`.

### 2026-04-19 Unity AI Strategic Layer Sub-Slice 14: Non-Aggression Pact Proposal Execution

- `AIPactProposalExecutionSystem` + new `PactComponent` port ai.js pact dispatch (~2643-2666) and simulation.js `getNonAggressionPactTerms` (~5150-5183) / `proposeNonAggressionPact` (~5185-5222) on branch `claude/unity-ai-pact-proposal-execution`. Sub-slice 6 already dispatched `CovertOpKind.PactProposal` into AICovertOpsComponent.LastFiredOp; sub-slice 14 consumes that dispatch and executes the pact semantics.
- Runs `[UpdateInGroup(SimulationSystemGroup), UpdateAfter(AICovertOpsSystem)]`. Per faction with the matching dispatch: gates on both FactionKind.Kingdom, source != target, source hostile to target via HostilityComponent buffer, no existing PactComponent entity between the two factions, source ResourceStockpileComponent.Influence >= 50 + Gold >= 80. Target is hardcoded `"player"` matching browser ai.js:2658 dispatch (`proposeNonAggressionPact(state, "enemy", "player")`).
- On success: deduct 50 Influence + 80 Gold from source ResourceStockpileComponent, remove HostilityComponent buffer entries both ways, create one PactComponent entity with FactionAId/FactionBId/StartedAtInWorldDays + MinimumExpiresAtInWorldDays = start + 180. Dispatch unconditionally cleared (one-shot pattern). Canonical constants: NON_AGGRESSION_PACT_INFLUENCE_COST = 50, NON_AGGRESSION_PACT_GOLD_COST = 80, NON_AGGRESSION_PACT_MINIMUM_DURATION_IN_WORLD_DAYS = 180.
- Deliberate departure from marriage primary+mirror pattern: pacts use a single PactComponent per bond rather than mirrored entities because pacts have no asymmetric downstream system analogous to MarriageGestationSystem. Queries that need per-faction enumeration scan all PactComponents and match either FactionAId or FactionBId.
- Cross-lane mutations: HostilityComponent buffer entries removed both ways (combat-and-projectile, retired), ResourceStockpileComponent.Influence/Gold on source (retired lanes). Field-level only; no schema changes.
- Deferred: holy-war pact gate (waits on holy-war lane; getNonAggressionPactTerms rejects pacts during active or incoming holy wars but Unity has no holy-war system yet); pact expiration and break system (browser breakNonAggressionPact ~5224 plus early-break legitimacy penalty); narrative pushMessage (waits on AI->UI message bridge, same blocker as sub-slices 11, 12, 13).
- `BloodlinesAIPactProposalExecutionSmokeValidation` 6-phase validator all green: PhaseSuccessfulPact (resources 200/100 -> 120/50, hostility cleared both ways, pact created with expiry 210 from start 30 + 180), PhaseHostilityRequired (no hostility blocks), PhaseAlreadyPactedRejected (pre-existing PactComponent blocks; resources untouched), PhaseInsufficientInfluence (30 < 50 blocks; gold untouched), PhaseInsufficientGold (60 < 80 blocks; influence untouched), PhaseTribeRejected (FactionKind.Tribes source blocked; browser requires both parties to be kingdoms).
- All 10 validation gates green. Contract bumped revision 28 -> 29.
- The per-slice handoff lives at `docs/unity/session-handoffs/2026-04-19-unity-ai-strategic-layer-sub-slice-14-pact-proposal-execution.md`.

### 2026-04-19 Unity AI Strategic Layer Sub-Slice 15: Pact Break And Expiration

- New `PactBreakRequestComponent` + `PactBreakSystem` port simulation.js `breakNonAggressionPact` (~5224-5257). Producer-driven semantic mirrors the browser: no auto-expiration; pacts past minimumExpiresAtInWorldDays remain active until explicitly broken. Request entity is destroyed after processing so idempotent re-requests do not double-apply the penalty.
- `PactBreakSystem` runs `[UpdateInGroup(SimulationSystemGroup), UpdateAfter(AIPactProposalExecutionSystem)]`. Per request: find matching PactComponent by PactId; short-circuit if not found or already broken; mark Broken=true and BrokenByFactionId=requesting; EnsureHostility idempotent-adds HostilityComponent entries both ways; breaker DynastyStateComponent.Legitimacy -= 8 clamped [0, 100] (NON_AGGRESSION_PACT_BREAK_LEGITIMACY_COST); breaker ConvictionComponent.Oathkeeping -= 2 via ConvictionScoring.ApplyEvent.
- Browser conviction.score -= 2 is mapped to the Oathkeeping bucket because Unity derives Score from bucket values (Score = Stewardship + Oathkeeping - Ruthlessness - Desecration). Oathkeeping is both architecturally correct (direct Score writes would be overwritten by Refresh) and semantically correct (pacts are oaths; breaking one stains oath-keeping). ConvictionScoring.ApplyEvent clamps the bucket at 0 so a faction with no remaining oath-keeping takes no additional penalty — matches the spirit of the canon.
- Penalty is unconditional regardless of early-break timing. Browser's earlyBreak flag affects messaging only; mechanical penalty applies identically whether the pact is broken before or after minimumExpiresAtInWorldDays.
- No target-side penalty. Only the breaker pays (legitimacy and oathkeeping). The target is put back into mutual hostility but its dynasty and conviction are untouched.
- Cross-lane mutations field-level only: PactComponent.Broken/BrokenByFactionId (own lane), HostilityComponent buffer entries idempotently added (combat-and-projectile, retired), DynastyStateComponent.Legitimacy clamp (dynasty-core, retired), ConvictionComponent.Oathkeeping via ConvictionScoring helper (conviction-scoring, retired).
- Deferred: narrative pushMessage (waits on AI->UI message bridge, same blocker as sub-slices 11-14).
- `BloodlinesPactBreakSmokeValidation` 5-phase validator all green: PhaseEarlyBreak (pact at day 30, current day 50 pre-minimum-expiry 210; enemy breaks -> Broken=true, BrokenByFactionId=enemy, hostility restored both ways, legitimacy 70->62, oathkeeping 5->3, request destroyed); PhaseLateBreak (current day 250 past minimum expiry 190; identical penalty -> browser parity on unconditional cost); PhaseIdempotentBreak (second break request on already-broken pact does not double-apply); PhaseNoPactNoOp (break request for missing PactId leaves state untouched; request still destroyed); PhaseLegitimacyClamp (breaker starts at Legitimacy 5 + Oathkeeping 0; both clamp to 0).
- All 10 validation gates green. Contract bumped revision 29 -> 30.
- The per-slice handoff lives at `docs/unity/session-handoffs/2026-04-19-unity-ai-strategic-layer-sub-slice-15-pact-break-expiration.md`.

### 2026-04-19 Unity AI Strategic Layer Sub-Slice 16: Narrative Message Bridge

- `NarrativeMessageComponents` + `NarrativeMessageBridge` port the browser `pushMessage` surface (simulation.js, many call sites) on branch `claude/unity-ai-narrative-message-bridge`. Rebased onto `origin/master` `40e80e03` after Codex's fortification-siege sub-slice 5 wall-segment destruction landed at revision 31.
- Sub-slices 11 through 15 all deferred their narrative ceremonial lines because no AI-to-UI message channel existed in Unity. Sub-slice 16 ports the minimal channel: `NarrativeMessageSingleton` tag entity carries a `NarrativeMessageElement` buffer of `{ Text: FixedString128Bytes, Tone: NarrativeMessageTone { Info, Good, Warn }, CreatedAtInWorldDays, Ttl }`. `NarrativeMessageBridge.Push(em, text, tone, ttlSeconds = 7f)` lazy-creates the singleton on first call and reads CreatedAtInWorldDays from the DualClockComponent singleton if seeded.
- PactBreakSystem (sub-slice 15) wired as the first consumer to prove the bridge end-to-end. After the mechanical break effects the system builds the browser ceremonial line with the same early-breach vs hostility-resumes suffix and routes tone via `breakerId == "player" ? Warn : Info`, then calls `NarrativeMessageBridge.Push`. Sub-slice 15 regression smoke all 5 phases remain PASS.
- Append-order buffer (browser uses `state.messages.unshift`; Unity consumers iterate forward or backward for preferred presentation order). No consumer/drain/TTL-eviction system in scope yet. Global scoping; per-faction scoping reserved for a future multiplayer slice without breaking the current API.
- Cross-lane reads only: `DualClockComponent.InWorldDays` (dual-clock-match-progression, retired). No structural edits.
- `BloodlinesNarrativeMessageBridgeSmokeValidation` 6-phase validator all green: PhaseLazySingleton (first push lazy-creates singleton + buffer), PhaseMultiplePushes (3 pushes accumulate in append order with correct tones), PhaseCreatedAtInWorldDays (entry stamped at day 42), PhasePactBreakEarlyMessage (early-breach text + Warn tone for player breaker), PhasePactBreakLateMessage (Hostility resumes text + Info tone for enemy breaker), PhaseNoMessageOnBadPact (missing pact short-circuits without pushing).
- All 10 validation gates green (siege smoke required one retry after a transient bee_backend cache-rebuild error referencing a stale path for Codex's FortificationStructureResolutionSystem.cs in Unity's Library cache; retried once after 15-second wait and passed cleanly). Contract bumped revision 31 -> 32.
- The per-slice handoff lives at `docs/unity/session-handoffs/2026-04-19-unity-ai-strategic-layer-sub-slice-16-narrative-message-bridge.md`.

### 2026-04-19 Unity AI Strategic Layer Sub-Slice 17: Narrative Back-Wire

- Back-wires `NarrativeMessageBridge.Push` into the three AI systems that deferred their browser ceremonial lines in earlier slices, on branch `claude/unity-ai-narrative-back-wire` from `origin/master` `8826d855` (revision 32). Systems: `AIMarriageAcceptEffectsSystem` (sub-slice 11+12 marriage accept line at simulation.js:7463), `AILesserHousePromotionSystem` (sub-slice 13 founding line at simulation.js:7251-7255), `AIPactProposalExecutionSystem` (sub-slice 14 pact entry line at simulation.js:5216-5220). Each wire-up is a small additive edit following the one-line-push pattern established by `PactBreakSystem` in sub-slice 16.
- Marriage accept push composes: `"<headTitle> of <headFactionId> weds <spouseTitle> of <spouseFactionId> <approvalText>."` Authority suffix: HeadDirect -> "under head approval"; HeirRegency -> "under heir regency (legitimacy -1)"; EnvoyRegency -> "under envoy regency (legitimacy -2)". Tones Good when HeadFactionId == "player", else Info. Browser tone at simulation.js:7466 is hardcoded "good"; Unity applies the source/target tone rule so routine AI-to-AI marriages do not flood Good-toned notifications. Member titles resolve via `DynastyMemberRef` buffer on the respective faction entity; fallback is the raw `MemberId` when no roster is seeded.
- Lesser-house founding push composes: `"<factionId> founds <lesserHouseName>, honoring <founderTitle>."` Tones Good when faction is "player", else Info (matches browser simulation.js:7254 ternary).
- Pact proposal push composes: `"<sourceFactionId> and <targetFactionId> enter a non-aggression pact. Hostility ceases for at least 180 in-world days."` Tones Good when source is "player", else Info. `AIPactProposalExecutionSystem` hardcodes `targetFactionId = "player"` per the browser ai.js dispatch, so the Good branch is defensively present but unreachable in the current system.
- Title fallback rewrite: `ResolveMemberTitle` now returns the FixedString64Bytes `MemberId` directly instead of walking byte-by-byte. `FixedString64Bytes.Append(byte)` promotes through the `Append(int)` overload and writes each byte as a decimal, which produced garbage output in the initial smoke run; direct FixedString assignment preserves the UTF-8 bytes verbatim. Also guards the PactBreakSystem pattern from the same issue in future wire-ups that want similar fallback paths.
- FactionId is used as a display-name stand-in since Unity has no faction display-name component yet. When such a component lands, the substitution points are isolated in `PushMarriageAcceptMessage`, `PushPromotionMessage`, and `PushPactEnteredMessage` and can be swapped in without reshaping the systems.
- Deferred: holy-war pact gate on the pact proposal system (waits on the holy-war lane); narrative TTL eviction system (walks the buffer each tick and removes entries whose `CreatedAtInWorldDays + Ttl` is past current); per-faction scoping of narrative messages (reserved for a future multiplayer slice); captive-recovery and captive-ransom execution systems (waits on a captive member buffer representation).
- `BloodlinesAINarrativeBackWireSmokeValidation` 6-phase validator all green: PhaseMarriageAcceptPlayerHeadDirect (player-source, Good tone, "weds"/"under head approval"), PhaseMarriageAcceptEnemyHeirRegency (enemy-source, Info tone, "heir regency"/"legitimacy -1"), PhaseMarriageAcceptEnemyEnvoyRegency (enemy-source, Info tone, "envoy regency"/"legitimacy -2"), PhaseLesserHousePromotionPlayer (player faction, Good tone, "founds"/"honoring"), PhaseLesserHousePromotionEnemy (enemy faction, Info tone), PhasePactProposalEnemy (enemy-source, Info tone, "enter a non-aggression pact"/"180 in-world days"). Sub-slice 11, 13, 14, 15, 16 regression smokes all re-run green, confirming the wire-ups are purely additive.
- All 10 validation gates green. Contract bumped revision 32 -> 33.
- The per-slice handoff lives at `docs/unity/session-handoffs/2026-04-19-unity-ai-strategic-layer-sub-slice-17-narrative-back-wire.md`.

### 2026-04-19 Unity Fortification Siege Sub-Slice 6: Breach Assault Pressure

- `BreachAssaultPressureSystem` now consumes `FortificationComponent.OpenBreachCount` after fortification destruction resolution and before `FieldWaterStrainSystem`, so hostile units inside a breached settlement's threat envelope gain a bounded exploitation bonus instead of breaches remaining legibility-only state.
- `FieldWaterComponent` now carries additive breach-pressure telemetry: `BreachAssaultAdvantageActive`, `BreachOpenCount`, `BreachTargetSettlementId`, `BreachAssaultAttackMultiplier`, and `BreachAssaultSpeedMultiplier`. The existing `TryDebugGetFieldWaterState` seam exposes these fields without widening the shared debug shell.
- `SiegeSupportCanon` now resolves breach exploitation at `+8%` attack and `+4%` speed per open breach, capped at three breaches. `FieldWaterStrainSystem` multiplies those bonuses alongside existing field-water and siege-support penalties, so the runtime attack/speed surface changes immediately when walls or gates fall.
- `BloodlinesSiegeSmokeValidation` now includes the new breach-pressure system in its validation world so owned siege smoke execution matches the live runtime order.
- New dedicated validator `BloodlinesBreachAssaultPressureSmokeValidation` plus wrapper `scripts/Invoke-BloodlinesUnityBreachAssaultPressureSmokeValidation.ps1` are green across four phases: intact-wall baseline, single-breach hostile bonus, settlement-owner exclusion, and multi-breach scaling with radius gating.
- Full 10-gate chain is green on the rebased `origin/master` `dfec72f5` base, and the dedicated breach smoke is also green via `artifacts/unity-breach-assault-pressure-smoke.log`.
- Contract bumped revision `33 -> 34` under `codex-fortification-breach-assault-pressure-2026-04-19`.
- Local csproj refresh note: the generated `unity/Assembly-CSharp.csproj` and `unity/Assembly-CSharp-Editor.csproj` were missing existing fortification-lane files after the master-base switch (`FortificationStructureResolutionSystem.cs`, `BloodlinesWallSegmentDestructionSmokeValidation.cs`) and were refreshed locally alongside the new breach-assault files. Both csproj files remain gitignored and are not part of the commit.
- The per-slice handoff lives at `docs/unity/session-handoffs/2026-04-19-unity-fortification-siege-breach-assault-pressure.md`.

### 2026-04-19 Unity AI Strategic Layer Sub-Slice 18: Dynasty Operations Foundation

- New `DynastyOperationComponent` + `DynastyOperationLimits` port the canonical dynasty-operation foundation that every browser dispatch site in `simulation.js` shares: `DYNASTY_OPERATION_ACTIVE_LIMIT = 6` at line 17 plus the seven dispatch sites (missionary ~10523, holy war ~10565, counter-intelligence ~10837, espionage ~10876, assassination ~10912, sabotage ~10952, captive rescue/ransom ~2566-2608 in ai.js) that gate on `(operations.active ?? []).length >= DYNASTY_OPERATION_ACTIVE_LIMIT` before appending. Branch: `claude/unity-ai-dynasty-operations-foundation` branched from `origin/master` `dfec72f5`; rebased onto `origin/master` `a2f5e6cd` after Codex's fortification sub-slice 6 breach assault pressure landed at revision 34.
- `DynastyOperationComponent` carries one entity per active operation with `OperationId` (FixedString64Bytes), `SourceFactionId` (FixedString32Bytes), `OperationKind` (new `DynastyOperationKind` enum: None, Missionary, HolyWar, DivineRight, CaptiveRescue, CaptiveRansom, LesserHousePromotion), `StartedAtInWorldDays` (float, stamped from DualClock), `TargetFactionId` (FixedString32Bytes, default when N/A), `TargetMemberId` (FixedString64Bytes, default when N/A), `Active` (bool). `Active=false` retains the entity for audit and excludes it from capacity counts.
- `DynastyOperationLimits` static helper exposes `DYNASTY_OPERATION_ACTIVE_LIMIT = 6`; `HasCapacity(em, factionId)` counts matching-source active entities and returns true strictly below cap; `CountActiveForFaction(em, factionId)` exposes the count for smoke assertions and future consumers; `BeginOperation(em, operationId, sourceFactionId, kind, targetFactionId, targetMemberId)` creates one entity with Active=true and the DualClock timestamp and returns the entity so callers can attach per-kind component data in the same tick.
- Deliberate departure from browser silent-trim (`operations.active.slice(0, DYNASTY_OPERATION_ACTIVE_LIMIT)`): Unity keeps the gate strict at the call site because an entity-per-operation model cannot silently drop entries without orphaning per-kind component data, and silent drop would mask over-cap bugs at the producer rather than surface them. Callers must check HasCapacity before BeginOperation; BeginOperation creates unconditionally.
- No system ships in sub-slice 18. The foundation ships standalone so each later dispatch slice (missionary execution, holy war execution, divine right declaration, captive rescue execution, captive ransom execution) can land as its own reviewable change and attach per-kind component structs (resolveAt, operatorId, successScore, sourceFaithId, etc.) to the entity created here. `LesserHousePromotion` is reserved in the enum for completeness; the sub-slice 13 promotion system does not currently route through the dynasty-operation cap because browser `promoteMemberToLesserHouse` does not call `getDynastyOperationsState`.
- Per-faction scan (not per-faction index) for HasCapacity: a typical match holds a small count of operations (cap 6 per faction, small number of factions), so linear scan is fine. If profiling shows this in hot paths, per-faction indexing or SharedComponent partitioning is the upgrade path.
- Cross-lane reads only: `DualClockComponent.InWorldDays` (dual-clock-match-progression, retired). No mutations, no new system.
- `BloodlinesDynastyOperationsSmokeValidation` 5-phase validator all green: PhaseBeginOperation (entity created with correct fields, DualClock stamp 12, HasCapacity=true at count 1); PhaseMultipleOperationsUnderCap (4 entities with 4 distinct kinds, HasCapacity=true at 4 < 6); PhaseCapReached (6 entities, HasCapacity=false); PhasePerFactionCap (enemy=5 + player=1, both below cap, per-faction scoping holds); PhaseInactiveExcluded (cap-worth of Active=false entities excluded, HasCapacity=true).
- All 10 validation gates green. Contract bumped revision 34 -> 35.
- The per-slice handoff lives at `docs/unity/session-handoffs/2026-04-19-unity-ai-strategic-layer-sub-slice-18-dynasty-operations-foundation.md`.

### 2026-04-19 Unity Fortification Siege Sub-Slice 7: Breach Legibility Readout

- New `SettlementBreachReadout` plus `BloodlinesDebugCommandSurface.Fortification.TryDebugGetSettlementBreachReadout` package settlement id, owner faction id, open breach count, destroyed wall/tower/gate/keep counts, current tier, derived reserve frontage, and aggregate breach-assault telemetry in one fortification-owned debug seam.
- The new seam deliberately ships without a production HUD consumer. This slice is the readout foundation only, matching the no-consumer foundation pattern used in Claude's ai-strategic-layer sub-slice 18.
- Reserve frontage is derived through the already-landed reserve-frontage rule instead of becoming a second stored state field. Aggregate breach-assault state is read from live `FieldWaterComponent` entries that currently target the settlement, so the readout stays aligned with sub-slice 6's runtime consumer rather than recomputing the attack/speed bonus separately.
- New dedicated validator `BloodlinesBreachLegibilityReadoutSmokeValidation` plus wrapper `scripts/Invoke-BloodlinesUnityBreachLegibilityReadoutSmokeValidation.ps1` are green across five phases: intact fortified settlement baseline, single-breach pressure activation, three-breach capped scaling, missing-settlement false/default behavior, and mixed partial destruction proving `walls=2`, `towers=1`, `gates=1`, `keeps=0`, `OpenBreachCount = 3`, tier 2, and reserve frontage 3.
- Full governed 10-gate chain is green on `D:\BLBLR\bloodlines`, plus the dedicated breach-legibility smoke. Contract bumped revision `35 -> 36`.
- Local csproj note: this new worktree began without Unity's gitignored `.csproj` files, so Unity regenerated `unity/Assembly-CSharp.csproj` and `unity/Assembly-CSharp-Editor.csproj` locally and both now include the new debug readout and validator entries. The regenerated csproj files remain gitignored and are not part of the commit.
- The per-slice handoff lives at `docs/unity/session-handoffs/2026-04-19-unity-fortification-siege-breach-legibility-readout.md`.

### 2026-04-19 Unity AI Strategic Layer Bundle 1: Captive State (Sub-Slice 19) and Missionary Execution (Sub-Slice 20)

- Bundle 1 of the AI mechanical campaign per the 2026-04-19 roadmap decision. Two logical sub-slices ship as one commit, one merge, and one contract revision bump on branch `claude/unity-ai-captive-state-and-missionary-execution`. Master base: `ef58ec4f` (after Codex fortification-siege sub-slice 7 breach legibility readout landed at revision 36).
- Sub-slice 19 captive member state: new `CapturedMemberElement` IBufferElementData with `[InternalBufferCapacity(8)]` carries `MemberId` (FixedString64Bytes), `MemberTitle` (FixedString64Bytes), `OriginFactionId` (FixedString32Bytes), `CapturedAtInWorldDays` (float, DualClock-stamped), `RansomCost` (float, default 0; sub-slice 24 ransom dispatch sets the canonical value), and `Status` (new `CapturedMemberStatus` enum: Held, RansomOffered, Released, Executed). The buffer attaches to the captor faction entity; sub-slices 23/24 captive rescue/ransom dispatch will consume it. New `CapturedMemberHelpers` static class exposes `CaptureMember` (lazy-creates buffer, appends Held entry, returns index), `TryGetCaptive` (lookup by member id with index), and `ReleaseCaptive` (mutates status in place, retains entry for audit). Browser parity: `transferMemberToCaptor` at simulation.js:4422-4453; `CAPTIVE_LEDGER_LIMIT = 16` trim deferred (Unity buffer accepts unconditionally; future retention pass can enforce a hard cap).
- Sub-slice 20 missionary execution: first production consumer of sub-slice 18 dynasty-operation foundation. New `DynastyOperationMissionaryComponent` per-kind component carries `ResolveAtInWorldDays` (current + 32f), `OperatorMemberId`, `OperatorTitle`, `SourceFaithId`, `ExposureGain`, `IntensityErosion`, `LoyaltyPressure`, `SuccessScore`, `ProjectedChance`, `IntensityCost` (12f), and `EscrowCost` (new `DynastyOperationEscrowCost` struct mirroring resource-stockpile fields with only Influence set to 14f). New `AIMissionaryExecutionSystem` ([UpdateInGroup(SimulationSystemGroup), UpdateAfter(AICovertOpsSystem)]) consumes `AICovertOpsComponent.LastFiredOp == CovertOpKind.Missionary`, gates on getMissionaryTerms parity (source != target, source FaithStateComponent.SelectedFaith != None, target faction exists, target has at least one ControlPointComponent owned by player, source has a faith operator on the DynastyMemberRef roster matching IdeologicalLeader/Spymaster/HeadOfBloodline/Diplomat with non-Fallen/non-Captured status, source ResourceStockpileComponent.Influence >= 14, source FaithStateComponent.Intensity >= 12), capacity-gates on DynastyOperationLimits.HasCapacity, deducts cost on success, computes per-kind terms from the simplified browser parity formula (offenseScore = operatorRenown + sourceIntensity*0.65 + dark/light bias; defenseScore = targetIntensity*0.55), calls DynastyOperationLimits.BeginOperation with DynastyOperationKind.Missionary + player target, attaches DynastyOperationMissionaryComponent, and pushes a NarrativeMessageBridge.Push line ("<source> dispatches missionaries of <faith> toward <target>.") with tone Warn when target is player and Info otherwise (browser tone routing simulation.js:10560-10561). Always clears LastFiredOp to None regardless of outcome (one-shot pattern).
- Browser duration translation: MISSIONARY_DURATION_SECONDS = 32 real seconds is reinterpreted as MissionaryDurationInWorldDays = 32f directly on the in-world timeline rather than translated through DualClock.DaysPerRealSecond. Future resolution slice can re-translate at runtime if the canonical clock rate shifts; data shape stays the same.
- Per-kind resolution intentionally deferred. Created operation entities sit Active=true with the per-kind component attached until a future slice walks expired DynastyOperationMissionaryComponent entries at ResolveAtInWorldDays, applies ExposureGain/IntensityErosion/LoyaltyPressure to the target faction, and flips Active=false.
- Defense-score simplification vs browser: Unity omits target-operator renown contribution and ward-profile bonus from the defense score because Unity has no symmetric target-operator gate yet and no faith-ward readout. Slight power adjustment vs browser; revisit when the resolution slice and ward-profile readout land.
- Faith operator role gate substitutes Spymaster for the canonical Sorcerer role (browser `getFaithOperatorMember` accepts `["ideological_leader", "sorcerer", "head_of_bloodline", "diplomat"]`); Unity has no Sorcerer DynastyRole yet.
- New `BloodlinesCaptiveStateAndMissionaryExecutionSmokeValidation` 8-phase validator all green: PhaseCaptureMember (1 Held entry with correct fields and DualClock timestamp); PhaseMultipleCaptivesPerFaction (3 captives from 3 origin factions, each found by member id); PhaseReleaseCaptive (status flipped to Released, entry retained on buffer for audit); PhaseMissionaryDispatchSuccess (op created, per-kind attached with ResolveAt=72, OperatorMemberId resolved, IntensityCost=12, EscrowCost.Influence=14, source Influence 50->36, source Intensity 30->18, narrative +1, dispatch cleared); PhaseMissionaryCapBlocks (cap-worth pre-seeded blocks new creation, resources/intensity untouched, dispatch still cleared one-shot); PhaseMissionaryNoFaithBlocks (CovenantId.None blocks); PhaseMissionaryInsufficientIntensityBlocks (intensity 8 < 12 blocks); PhaseMissionaryInsufficientResourcesBlocks (influence 10 < 14 blocks). Wrapper `scripts/Invoke-BloodlinesUnityCaptiveStateAndMissionaryExecutionSmokeValidation.ps1`.
- All 10 validation gates green. Contract bumped revision 36 -> 37.
- Local csproj note: this branch began with `unity/Assembly-CSharp.csproj` and `unity/Assembly-CSharp-Editor.csproj` carried from the prior `claude/unity-ai-dynasty-operations-foundation` worktree state, so the csproj also required adding Codex sub-slice 7 entries (`BloodlinesDebugCommandSurface.Fortification.BreachReadout.cs` and `BloodlinesBreachLegibilityReadoutSmokeValidation.cs`) for the dotnet-build gate to compile them. Unity's csproj auto-regeneration restores entries when the editor reloads. csproj files remain gitignored.
- The per-slice handoff lives at `docs/unity/session-handoffs/2026-04-19-unity-ai-strategic-layer-bundle-1-captive-state-and-missionary-execution.md`.

### 2026-04-19 Unity AI Strategic Layer Bundle 2: Holy War (Sub-Slice 21) and Divine Right (Sub-Slice 22) Execution

- Bundle 2 of the AI mechanical campaign: two faith-driven dispatch consumers ship as one commit, one merge, and one contract revision bump on branch `claude/unity-ai-holy-war-and-divine-right-execution`. Master base: `0b2fd111` (after Bundle 1 captive state + missionary execution landed at revision 37).
- Sub-slice 21 holy war declaration execution: second production consumer of the sub-slice 18 dynasty-operation foundation. New `DynastyOperationHolyWarComponent` per-kind component carries `ResolveAtInWorldDays` (current + 18f for declaration window), `WarExpiresAtInWorldDays` (current + 180f for full holy-war duration), `OperatorMemberId`, `OperatorTitle`, `IntensityPulse` (browser doctrine bias 1.2 dark / 0.9 light), `LoyaltyPulse` (1.8 dark / 1.2 light), `CompatibilityLabel` (simplified Unity-side derivation: identical-faith-different-doctrine -> "fractured", different-faith -> "discordant"), `IntensityCost` (18f), and `EscrowCost` (Influence = 24f). New `AIHolyWarExecutionSystem` ([UpdateInGroup(SimulationSystemGroup), UpdateAfter(AICovertOpsSystem)]) consumes `AICovertOpsComponent.LastFiredOp == CovertOpKind.HolyWar`, gates on getHolyWarDeclarationTerms parity (source != target, both factions have committed faith, simplified harmonious-tier check via identical (faith, doctrine), source has faith operator on roster, source ResourceStockpileComponent.Influence >= 24, source FaithStateComponent.Intensity >= 18), capacity-gates on DynastyOperationLimits.HasCapacity, deducts cost on success, computes per-kind doctrine-bias pulses, calls DynastyOperationLimits.BeginOperation with DynastyOperationKind.HolyWar + player target, attaches the per-kind component, and pushes `<source> sends a holy war declaration toward <target>.` with tone Warn when source or target is player and Info otherwise (browser tone routing simulation.js:10599 always-warn-on-player-involvement).
- Sub-slice 22 divine right declaration execution: third production consumer of the sub-slice 18 dynasty-operation foundation. New `DynastyOperationDivineRightComponent` per-kind component carries `ResolveAtInWorldDays` (current + 180f), `SourceFaithId` (CovenantId mapped to canonical string), `DoctrinePath` (captured at declaration), `RecognitionShare` and `RecognitionSharePct` (default 0; recognition system not yet ported), `ActiveApexStructureId` and `ActiveApexStructureName` (default empty; apex structure surface not yet ported). No EscrowCost or IntensityCost (browser divine right does NOT deduct intensity or charge escrow; the cost is the covenant-test prerequisite that gates getDivineRightDeclarationTerms). New `AIDivineRightExecutionSystem` (same group/order) consumes `AICovertOpsComponent.LastFiredOp == CovertOpKind.DivineRight`, gates on the simplified Unity-side parity set (source has committed faith, source FaithStateComponent.Intensity >= 80 DIVINE_RIGHT_INTENSITY_THRESHOLD, source FaithStateComponent.Level >= 5, no existing active DivineRight DynastyOperationComponent for this faction, DynastyOperationLimits.HasCapacity), calls DynastyOperationLimits.BeginOperation with DynastyOperationKind.DivineRight and default target (divine right has no specific target faction; affected factions are derived at resolution time), attaches the per-kind component, pushes `<source> declares Divine Right under <faith>. The spread window opens for 180 in-world days.` with tone Warn always (browser simulation.js:10832 always-warn).
- Both systems clear LastFiredOp to None unconditionally after processing (one-shot pattern shared with sub-slices 8/9/12/13/14/20).
- Browser duration translation continues the sub-slice 20 convention: HOLY_WAR_DECLARATION_DURATION_SECONDS = 18, HOLY_WAR_DURATION_SECONDS = 180, and DIVINE_RIGHT_DECLARATION_DURATION_SECONDS = 180 are reinterpreted as in-world-day numeric values directly rather than translated through DualClock.DaysPerRealSecond.
- Notable Unity-side departure (sub-slice 22): divine right routes through DynastyOperationLimits.BeginOperation with DynastyOperationKind.DivineRight even though browser does NOT gate divine right on DYNASTY_OPERATION_ACTIVE_LIMIT (startDivineRightDeclaration writes directly to faction.faith.divineRightDeclaration). Unity-side routing is for surface consistency with all other dispatch consumers; browser per-faction one-active-at-a-time semantic is preserved by the explicit existing-declaration gate before the capacity check.
- Sub-slice 21 compatibility-tier simplification: Unity uses identical-(faith, doctrine) equality instead of browser's getMarriageFaithCompatibilityProfile tier ladder which depends on covenant-name covariance not yet ported. Unity gate is strictly looser than browser (admits some "neutral" tiers browser would block); future tightening lands when the covenant compatibility surface ports.
- Sub-slice 22 deferred gates: covenant-test-passed gate (browser ensureFaithCovenantTestCompletionFromLegacyState + faithState.covenantTestPassed at simulation.js:10606/10614), cooldown gate (profile.cooldownRemaining at :10626), stage-ready gate (profile.stageReady Final Convergence threshold at :10629), active-apex-structure gate (profile.activeApexStructureReady at :10635), recognition-share gate (profile.recognitionReady at :10638), and faction-kind == kingdom gate (at :10607; defensible to add via FactionKindComponent in a hardening pass).
- Sub-slice 22 deferred effects: mutual hostility application against non-same-faith kingdoms (browser ensureMutualHostility at simulation.js:10819), AI timer cap propagation to candidate factions (browser attackTimer/territoryTimer/raidTimer/missionaryTimer/holyWarTimer caps at :10822-10826), conviction event recording (browser recordConvictionEvent at :10806; oathkeeping for light, desecration for dark, +3 either way), and resolution at ResolveAtInWorldDays (apex faith claim on success vs failDivineRightDeclaration cooldown + legitimacy penalty on failure at :10691). All wait on a future per-kind resolution slice.
- Per-kind resolution intentionally deferred for both sub-slices (matching the Bundle 1 pattern). Created operation entities sit Active=true with the per-kind component attached until a future slice walks expired entries at ResolveAtInWorldDays / WarExpiresAtInWorldDays, applies effects, and flips Active=false.
- New dedicated bundled validator `BloodlinesHolyWarAndDivineRightExecutionSmokeValidation` plus wrapper `scripts/Invoke-BloodlinesUnityHolyWarAndDivineRightExecutionSmokeValidation.ps1`. All 10 phases PASS:
  - PhaseHolyWarDispatchSuccess: op created, per-kind attached with ResolveAt=68, WarExpiresAt=230, IntensityCost=18, EscrowCost.Influence=24, OperatorMemberId resolved, light pulses 0.9/1.2, source Influence 50->26, source Intensity 30->12, narrative +1 Warn, dispatch cleared
  - PhaseHolyWarHarmoniousFaithBlocks: identical (faith, doctrine) blocks, resources untouched
  - PhaseHolyWarTargetNoFaithBlocks: target CovenantId.None blocks
  - PhaseHolyWarInsufficientIntensityBlocks: intensity 12 < 18 blocks
  - PhaseHolyWarInsufficientResourcesBlocks: influence 20 < 24 blocks
  - PhaseHolyWarDarkPathPulses: dark doctrine produces 1.2/1.8 pulses
  - PhaseDivineRightDispatchSuccess: op created with default target, per-kind attached with ResolveAt=280, SourceFaithId=the_order, DoctrinePath=Light, recognition/structure placeholders default, narrative +1 Warn, dispatch cleared
  - PhaseDivineRightInsufficientIntensityBlocks: intensity 50 < 80 blocks
  - PhaseDivineRightInsufficientLevelBlocks: level 4 < 5 blocks
  - PhaseDivineRightExistingDeclarationBlocks: preexisting active DivineRight op blocks new creation, dispatch still cleared
- All 10 governed validation gates green. Contract bumped revision 37 -> 38.
- Post-implementation diagnostic: initial bundled smoke run failed with `FixedString32Bytes: Truncation while copying ". The spread window opens for "` because the divine-right narrative line had a 31-character segment that overflowed FixedString32Bytes (29-byte cap after header). Fix: cast the segment to FixedString64Bytes. Smoke re-ran clean.
- The per-slice handoff lives at `docs/unity/session-handoffs/2026-04-19-unity-ai-strategic-layer-bundle-2-holy-war-and-divine-right-execution.md`.

### 2026-04-19 Unity Fortification Siege Sub-Slice 8: Breach Sealing Recovery

- New `BreachSealingProgressComponent` plus `BreachSealingSystem` give fortified
  settlements a defender-side recovery loop for `OpenBreachCount`. A settlement
  with open breaches now reserves `60` stone per breach, consumes `8` in-world
  worker-hours per breach at a 1 Hz tick, and reduces `OpenBreachCount` when the
  active breach finishes.
- Worker availability is resolved by scanning the owning faction's live idle
  workers (`UnitRole.Worker`, positive health, `WorkerGatherPhase.Idle`) instead
  of widening `FortificationReserveComponent`. This keeps the slice narrow and
  player/AI-neutral.
- `FortificationDestructionResolutionSystem` no longer rewrites
  `OpenBreachCount = destroyedWalls + destroyedGates` every frame. It still
  refreshes destroyed wall/tower/gate/keep counters, but now only adds
  newly-created breaches back into the live open-breach total so sealing can
  persist while destroyed structures remain unrepaired.
- New dedicated validator `BloodlinesBreachSealingRecoverySmokeValidation` plus
  wrapper `scripts/Invoke-BloodlinesUnityBreachSealingRecoverySmokeValidation.ps1`
  are green across six phases: half-window progress accumulation, first breach
  sealed, insufficient-stone block, zero-idle-worker block, intact-settlement
  no-op, and full three-breach sealing.
- Full governed 10-gate chain is green on `D:\BLBSR\bloodlines`, plus the
  dedicated breach-sealing smoke on the rebased `origin/master` `cec33509`
  base. Contract bumped revision `38 -> 39`.
- Local csproj note: the local gitignored `unity/Assembly-CSharp.csproj` and
  `unity/Assembly-CSharp-Editor.csproj` were refreshed so they included Claude
  Bundle 1 and Bundle 2 already-landed files plus the new breach-sealing files
  before the `dotnet build` gates ran.
- The per-slice handoff lives at
  `docs/unity/session-handoffs/2026-04-19-unity-fortification-siege-breach-sealing-recovery.md`.

### 2026-04-20 Unity AI Strategic Layer Bundle 3: Captive Rescue (Sub-Slice 23) and Captive Ransom (Sub-Slice 24) Execution

- Bundle 3 of the AI mechanical campaign: two captive-lifecycle dispatch consumers ship as one commit, one merge, and one contract revision bump on branch `claude/unity-ai-captive-rescue-and-ransom-execution`. Master base: `7821d74a` (rebased from original `cec33509` base after Codex fortification-siege sub-slice 8 breach sealing / recovery landed at revision 39 concurrently with Bundle 3's original revision-39 claim; Bundle 3 bumped to revision 40).
- Sub-slice 23 captive rescue execution: fourth production consumer of the sub-slice 18 dynasty-operation foundation and first production reader of the sub-slice 19 CapturedMemberElement buffer. New `DynastyOperationCaptiveRescueComponent` per-kind component carries `ResolveAtInWorldDays` (current + 20f for rescue duration), `CaptiveMemberId`, `CaptiveMemberTitle`, `CaptorFactionId`, `SpymasterMemberId` (priority [Spymaster, Diplomat, Merchant]), `DiplomatMemberId` (priority [Diplomat, Merchant, HeirDesignate]), `HoldingSettlementId/KeepTier/WardId` placeholders defaulting to empty/0 because the Unity holding-settlement and ward-profile surfaces are not yet ported, `SuccessScore` from simplified parity (power = 12 + spymaster.renown * 0.95 + diplomat.renown * 0.35; difficulty = 16 base), `ProjectedChance` clamped 0.12-0.88 from 0.5 + (successScore / 45), `IntensityCost` 0 (rescue does not deduct intensity), and `EscrowCost` with Gold=42 + Influence=26 (RESCUE_BASE_GOLD_COST and RESCUE_BASE_INFLUENCE_COST). New `AICaptiveRescueExecutionSystem` ([UpdateInGroup(SimulationSystemGroup), UpdateAfter(AICovertOpsSystem)]) consumes `AICovertOpsComponent.LastFiredOp == CovertOpKind.CaptiveRescue`, walks every faction's CapturedMemberElement buffer via a shared `TryPickCaptive` helper to find a Held captive belonging to source (simplified Unity port of browser pickAiCaptiveRecoveryTarget at ai.js:3011 which sorts by role priority then renown; Unity port returns the first match because CapturedMemberElement does not yet carry roleId/renown), gates on (captive picker success, no existing active dynasty operation for the captive's member id, source has Spymaster + Diplomat operatives on roster with non-Fallen/non-Captured status, source ResourceStockpileComponent.Gold >= 42, source ResourceStockpileComponent.Influence >= 26, DynastyOperationLimits.HasCapacity), deducts cost on success, computes simplified parity formula, calls `DynastyOperationLimits.BeginOperation` with `DynastyOperationKind.CaptiveRescue` + captor faction id as target + captive member id, attaches the per-kind component, and pushes a `NarrativeMessageBridge.Push` line ("<source> dispatches covert agents to recover <captive> from <captor>.") with tone Info when source is player and Good otherwise (browser tone routing simulation.js:11108).
- Sub-slice 24 captive ransom execution: fifth production consumer of the foundation and second production reader of the CapturedMemberElement buffer. New `DynastyOperationCaptiveRansomComponent` per-kind component carries `ResolveAtInWorldDays` (current + 16f for ransom duration), `CaptiveMemberId`, `CaptiveMemberTitle`, `CaptorFactionId`, `DiplomatMemberId` (priority [Diplomat, Merchant, HeirDesignate, HeadOfBloodline]), `MerchantMemberId` (priority [Merchant, Governor, HeadOfBloodline]), `ProjectedChance` hardcoded 1.0 (ransom is a paid transaction not a roll per browser simulation.js:4964), `IntensityCost` 0, and `EscrowCost` with Gold=70 + Influence=18 (RANSOM_BASE_GOLD_COST and RANSOM_BASE_INFLUENCE_COST). New `AICaptiveRansomExecutionSystem` (same group/order) reuses the sub-slice 23 captive picker and gate helpers via three `internal static` methods on AICaptiveRescueExecutionSystem (`TryPickCaptive`, `HasActiveOperationForMember`, `TryGetMemberByRolePriority`) so the two systems stay in sync without duplication. Gates (Diplomat + Merchant roles, Gold >= 70, Influence >= 18), deducts cost on success, calls BeginOperation with DynastyOperationKind.CaptiveRansom + captor faction id as target + captive member id, attaches the per-kind component, pushes "<source> opens ransom terms for <captive> with <captor>." with same tone routing.
- Both systems clear LastFiredOp to None unconditionally after processing (one-shot pattern shared with sub-slices 8/9/12/13/14/20/21/22).
- Browser duration translation continues the sub-slice 20 convention: RESCUE_BASE_DURATION_SECONDS=20, RANSOM_BASE_DURATION_SECONDS=16 reinterpreted as in-world-day numeric values directly. Renown-scaled duration adjustments deferred until CapturedMemberElement carries renown.
- Cost simplification: Unity uses canonical base constants without browser's renown/role/keepTier/ward scaling. A future captive-element extension can tighten cost calculation without reshaping the systems.
- Per-kind resolution intentionally deferred for both sub-slices following the Bundle 1/2 pattern. Created operation entities sit Active=true with the per-kind component attached until a future slice walks expired entries at ResolveAtInWorldDays, rolls against ProjectedChance for rescue (or unconditionally succeeds for ransom), applies effects, and flips Active=false.
- Parallel-revision resolution: Bundle 3 originally branched from revision-38 master (cec33509) and claimed revision 39 concurrently with Codex's fortification sub-slice 8. Codex landed first at revision 39 on master 7821d74a. Bundle 3 rebased onto 7821d74a and bumped from 39 to 40. All Bundle 3 validation gates re-run cleanly on the new base; Codex's sub-slice 8 BreachSealingSystem smoke also re-verified green alongside Bundle 3 (6 sealing phases all green).
- New dedicated bundled validator `BloodlinesCaptiveRescueAndRansomExecutionSmokeValidation` plus wrapper `scripts/Invoke-BloodlinesUnityCaptiveRescueAndRansomExecutionSmokeValidation.ps1`. All 9 phases PASS:
  - PhaseRescueDispatchSuccess: op created with player target + captive member id, per-kind attached with ResolveAt=80, EscrowCost.Gold=42, EscrowCost.Influence=26, OperatorMemberIds resolved, source Gold 100->58, source Influence 100->74, narrative +1 Good, dispatch cleared
  - PhaseRescueNoCaptiveBlocks: no Held captives blocks dispatch
  - PhaseRescueNoSpymasterBlocks: only Commander on roster blocks
  - PhaseRescueInsufficientGoldBlocks: gold 30 < 42 blocks
  - PhaseRescueInsufficientInfluenceBlocks: influence 20 < 26 blocks
  - PhaseRescueExistingOperationBlocks: preexisting active Ransom op for member id blocks new Rescue dispatch
  - PhaseRansomDispatchSuccess: op created, per-kind attached with ResolveAt=96, EscrowCost.Gold=70, EscrowCost.Influence=18, ProjectedChance=1.0, source Gold 100->30, source Influence 50->32, narrative +1 Good, dispatch cleared
  - PhaseRansomInsufficientGoldBlocks: gold 50 < 70 blocks
  - PhaseRansomNoMerchantBlocks: only Diplomat on roster blocks
- All 10 governed validation gates green on the rebased base. Contract bumped revision 39 -> 40.
- The per-slice handoff lives at `docs/unity/session-handoffs/2026-04-20-unity-ai-strategic-layer-bundle-3-captive-rescue-and-ransom-execution.md`.

## Codex Fortification Siege Sub-Slice 9: Destroyed Counter Recovery (2026-04-20)

### Status: COMPLETE on branch codex/unity-fortification-destroyed-counter-recovery

### What Was Done
- New `DestroyedCounterRecoveryProgressComponent` plus `DestroyedCounterRecoverySystem` extend the sub-slice 8 sealing loop into slow structural rebuild for `FortificationComponent.DestroyedWallCount`, `DestroyedTowerCount`, `DestroyedGateCount`, and `DestroyedKeepCount`.
- The recovery gate is explicit: the system only runs when `OpenBreachCount == 0`, so live breach sealing always finishes before destroyed counters begin to recover.
- Recovery priority is strict `Keep > Gate > Wall > Tower`. Standard rebuilds reserve `90` stone and `14` worker-hours at `1 Hz`; keep rebuilds apply a `2x` cost and `2x` worker-hour multiplier.
- Worker availability reuses the narrow live idle-worker scan from sub-slice 8 (`UnitRole.Worker`, positive health, `WorkerGatherPhase.Idle`) instead of widening `FortificationReserveComponent`.
- When a rebuild completes, the system decrements the chosen destroyed counter and restores the linked destroyed structure's `HealthComponent.Current` to max so `FortificationDestructionResolutionSystem` remains authoritative on subsequent ticks.
- New dedicated validator `BloodlinesDestroyedCounterRecoverySmokeValidation` plus wrapper `scripts/Invoke-BloodlinesUnityDestroyedCounterRecoverySmokeValidation.ps1` are green across six phases: post-sealing progress accumulation, first wall rebuild completion, keep-over-gate priority with `2x` keep multipliers, open-breach block, all-counters-zero no-op, and full keep->gate->wall->tower rebuild order.
- Full governed 10-gate chain is green on `D:\BLDCR\bloodlines`, plus the dedicated destroyed-counter recovery smoke on the rebased `origin/master` `e73933e4` base. Contract bumped revision `40 -> 41`.
- Local csproj note: the local gitignored `unity/Assembly-CSharp.csproj` and `unity/Assembly-CSharp-Editor.csproj` were refreshed so they included Claude Bundle 3's already-landed files plus the new destroyed-counter recovery files before the `dotnet build` gates ran.

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
- Contract staleness check: PASS at revision 40 before the revision-41 doc bump; re-run required after doc update
- Dedicated destroyed-counter recovery smoke: PASS via `artifacts/unity-destroyed-counter-recovery-smoke.log` with marker `BLOODLINES_DESTROYED_COUNTER_RECOVERY_SMOKE PASS`

### Recommended Next Fortification Follow-Up
1. Sealing-cost balance pass if the owner wants the new sealing and rebuild loops tuned before additional consumers land.
2. Breach-depth telemetry if the owner wants more explicit observability around sealing versus rebuild progress.

### 2026-04-20 Unity AI Strategic Layer Bundle 4: Missionary Resolution (Sub-Slice 25)

- Bundle 4 of the AI mechanical campaign: first production per-kind resolution consumer. Ships sub-slice 25 alone because per-kind resolution systems differ substantively across operation kinds. Branch: `claude/unity-ai-missionary-resolution-rebased`. Master base: `082699ab` (after Codex fortification-siege sub-slice 9 landed at revision 41; Bundle 4 cherry-picked onto rewritten master following upstream history rewrite).
- `AIMissionaryResolutionSystem` ([UpdateInGroup(SimulationSystemGroup), UpdateAfter(AIMissionaryExecutionSystem)]) walks every DynastyOperationComponent with OperationKind=Missionary and Active=true, filters by DynastyOperationMissionaryComponent.ResolveAtInWorldDays <= current DualClock.InWorldDays, applies canonical browser missionary resolution effects from simulation.js:5517-5588 + simulation.js:10473-10503.
- Success gate: SuccessScore >= 0. On success: ExposureGain applied to target's FaithExposureElement buffer (clamp [0,100]); IntensityErosion to target's FaithStateComponent.Intensity (clamped >=0) IFF target has different committed faith; LoyaltyPressure to lowest-loyalty ControlPointComponent owned by target (clamped [0,100]). On failure: +2 intensity reinforcement when target has committed faith. Always flips Active=false.
- First consumer of DynastyOperation* per-kind components reading rather than producing. Establishes resolution walker pattern future per-kind resolution systems (holy war, divine right, captive rescue, captive ransom) can reuse.
- 8-phase smoke PASS: PhaseNotExpiredSkips; PhaseExpiredSuccessApplies (exposure 10->30, intensity 50->45, loyalty 80->77, narrative +1); PhaseExpiredFailureStrengthens (intensity 50->52, exposure/loyalty unchanged); PhaseVoidOnMissingTarget; PhaseExposureAppendsNewEntry; PhaseExposureClampsAt100; PhaseLowestLoyaltyControlPointTargeted; PhaseIntensityErosionSkippedWhenSameFaith.
- Unity-side simplifications deferred: ward-profile-triggered mutual hostility on failure; conviction event recording; legitimacy-penalty fallback; exposure-threshold crossing narrative.
- All 10 governed validation gates green. Contract bumped revision 41 -> 42.
- Per-slice handoff: `docs/unity/session-handoffs/2026-04-20-unity-ai-strategic-layer-bundle-4-missionary-resolution.md`.

## Codex Fortification Siege Sub-Slice 10: Breach Depth Telemetry (2026-04-20)

### Status: COMPLETE on branch codex/unity-fortification-breach-depth-telemetry

### What Was Done
- Added `SettlementBreachTelemetry` plus `TryDebugGetSettlementBreachTelemetry` in `BloodlinesDebugCommandSurface.Fortification.BreachReadout.cs` so one structured debug seam now exposes settlement breach state, sealing eligibility/progress/costs, and destroyed-counter recovery eligibility/progress/costs without breaking the existing `SettlementBreachReadout` callers.
- Centralized the sealing and rebuild canon values in `FortificationCanon.cs` and updated both `BreachSealingSystem` and `DestroyedCounterRecoverySystem` to consume those shared constants instead of private copies.
- Extended `BloodlinesBreachLegibilityReadoutSmokeValidation` from 5 phases to 7 phases so the validator now proves mid-seal telemetry (`4/8` worker-hours, `60/60` stone, `0.5` progress) and mid-recovery telemetry on keep-priority rebuild (`14/28` worker-hours, `180/180` stone, `0.5` progress).
- Full governed 10-gate chain is green on `D:\BLBDT\bloodlines`, plus the dedicated breach-legibility readout smoke on top of revision-42 master-equivalent `575b824f`. Contract bumped revision `42 -> 43` to document the new fortification branch and handoff.
- Repo-state note: the current multi-day directive's Priority 2 description is stale. The repo already contains `MarriageComponents`, `MarriageProposalExpirationSystem`, `MarriageGestationSystem`, `LesserHouseLoyaltyDriftSystem`, and `MinorHouseLevySystem` from the retired `tier2-batch-dynasty-systems` lane, so no duplicate zero-code marriages lane was opened.

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
- Contract staleness check: PASS at revision 42 before the revision-43 doc bump; re-run required after doc update
- Breach legibility readout smoke: PASS via `artifacts/unity-breach-legibility-readout-smoke.log` with phases 6 and 7 covering sealing and recovery telemetry

### Recommended Next Fortification Follow-Up
1. Push and merge `codex/unity-fortification-breach-depth-telemetry`, then retire or pause the fortification lane unless Lance requests the optional sealing-cost balance pass.
2. If a dynasty follow-up is still desired, scope it as hardening or extension on top of the existing Tier 2 dynasties implementation instead of re-porting marriages, lesser houses, and minor houses from zero.

## Codex Dynasty Sub-Slice 2: Lesser-House Loyalty Parity (2026-04-20)

### Status: COMPLETE on branch codex/unity-dynasty-lesser-house-loyalty-parity

### What Was Done
- `MarriageComponents.cs` now expands `LesserHouseElement` with mixed-bloodline, marital-anchor, world-pressure, and defection-timing state plus the supporting `LesserHouseMaritalAnchorStatus` and `LesserHouseWorldPressureStatus` enums.
- `LesserHouseLoyaltyDriftSystem` now consumes the browser-style drift stack instead of the flat Tier 2 placeholder: legitimacy, oathkeeping, ruthlessness, and fallen-ledger pressure set the base daily delta; mixed-bloodline hostility, marriage-anchor recovery or fracture, and world pressure then push the final loyalty change.
- The system now starts a 5-day grace window the first day loyalty reaches zero, then defects only after the grace window expires. The resulting breakaway applies dynasty legitimacy `-6`, conviction ruthlessness `+1`, and establishes reciprocal hostility between the parent faction and the spawned minor house.
- The slice deliberately reuses the marriage-parity hooks that landed immediately before it: `DynastyMixedBloodlineComponent`, `MarriageChildElement`, and `MarriageDeathDissolutionSystem` are now live inputs to lesser-house loyalty drift rather than dead metadata.
- New dedicated validator `BloodlinesLesserHouseLoyaltyParitySmokeValidation` plus wrapper `scripts/Invoke-BloodlinesUnityLesserHouseLoyaltyParitySmokeValidation.ps1` are green across three phases: active-anchor recovery, hostile dissolution strain under overwhelming world pressure, and day-16 post-grace defection.
- Full governed validation chain is green on `D:\BLAICD\bloodlines`. Contract bumped revision `44 -> 45`.

### Gate Results
- `dotnet build unity/Assembly-CSharp.csproj -nologo`: PASS
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`: PASS
- Bootstrap runtime smoke: PASS via worktree-local wrapper copy under the Unity lock
- Combat smoke: PASS
- Scene shells: Bootstrap + Gameplay PASS via worktree-local wrapper copies under the Unity lock
- Fortification smoke: PASS
- Siege smoke: PASS
- `node tests/data-validation.mjs`: PASS
- `node tests/runtime-bridge.mjs`: PASS
- Contract staleness check: PASS at revision 45 after the continuity and contract updates
- Dedicated lesser-house loyalty parity smoke: PASS with three proof phases and marker `BLOODLINES_LESSER_HOUSE_LOYALTY_PARITY_SMOKE PASS`

### Recommended Next Dynasty Follow-Up
1. Tighten `MinorHouseLevySystem` parity next, especially territorial-levy timing and breakaway-spawn integration with the new lesser-house defection timestamps and hostility hooks.
2. After the dynasty parity stack closes, continue into covert-ops or scout-raid/logistics-interdiction follow-ups from the active directive.

## Codex Dynasty Sub-Slice 3: Minor-House Levy Parity (2026-04-20)

### Status: COMPLETE on branch codex/unity-dynasty-minor-house-levy-parity

### What Was Done
- `MarriageComponents.cs` now expands `MinorHouseLevyComponent` from the retired three-field placeholder into the runtime levy payload the browser loop expects: origin faction, claim id, levy status, retinue count and cap, last levy metadata, and parent-pressure tempo.
- `LesserHouseLoyaltyDriftSystem` now seeds a breakaway minor house with the live ECS surfaces it needs to function as more than a registry record: `ResourceStockpileComponent`, `PopulationComponent`, `DynastyStateComponent` at legitimacy `30`, inherited `FaithStateComponent` when the parent is committed, and a stabilized border-claim `ControlPointComponent` with loyalty `62`, food trickle `0.08`, and influence trickle `0.06`.
- `MinorHouseLevySystem` now ports the browser's territorial-levy cadence instead of the flat interval timer. The system now handles landless/dispossessed/contested/unsettled decay, canonical loyalty gate `48`, retinue-cap calculation from claim stability plus territory count, parent world-pressure tempo bonuses, and browser levy-profile selection for `militia`, `swordsman`, and `bowman`.
- Successful levies now spend food, influence, and claim loyalty and spawn a real ECS combat unit with position, transform, health, movement, combat stats, stance, and projectile data instead of the old minimal marker entity.
- New dedicated validator `BloodlinesMinorHouseLevyParitySmokeValidation` plus wrapper `scripts/Invoke-BloodlinesUnityMinorHouseLevyParitySmokeValidation.ps1` are green across four phases: defection claim spawn, landless decay, pressured bowman raise, and muster-cap blocking.
- The prior lesser-house parity validator was re-run green after the breakaway-spawn widening so the new claim and stockpile seeding did not regress day-16 defection behavior.
- Full governed validation chain is green on `D:\BLAICD\bloodlines`. Contract bumped revision `45 -> 46`.

### Gate Results
- `dotnet build unity/Assembly-CSharp.csproj -nologo`: PASS
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`: PASS
- Bootstrap runtime smoke: PASS via worktree-local wrapper copy under the Unity lock
- Combat smoke: PASS
- Scene shells: Bootstrap + Gameplay PASS via worktree-local wrapper copies under the Unity lock
- Fortification smoke: PASS
- Siege smoke: PASS
- `node tests/data-validation.mjs`: PASS
- `node tests/runtime-bridge.mjs`: PASS
- Contract staleness check: PASS at revision 46 after the continuity and contract updates
- Dedicated lesser-house loyalty parity regression smoke: PASS with marker `BLOODLINES_LESSER_HOUSE_LOYALTY_PARITY_SMOKE PASS`
- Dedicated minor-house levy parity smoke: PASS with marker `BLOODLINES_MINOR_HOUSE_LEVY_PARITY_SMOKE PASS`

### Recommended Next Follow-Up
1. The dynasty-house parity lane can pause cleanly. Under the current contract, scout raids or logistics interdiction are the safest next non-AI targets.
2. Optional dynasty follow-up only if Lance wants more parity before that: founder-member and ceremonial-message provenance on defected minor-house spawns.


### Gate Results
- `dotnet build unity/Assembly-CSharp.csproj -nologo`: PASS
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`: PASS with existing editor warnings only
- Bootstrap runtime smoke: PASS via worktree-local wrapper copy under the Unity lock
- Combat smoke: PASS
- Scene shells: Bootstrap + Gameplay PASS via worktree-local wrapper copies under the Unity lock
- Fortification smoke: PASS
- Siege smoke: PASS
- `node tests/data-validation.mjs`: PASS
- `node tests/runtime-bridge.mjs`: PASS
- Contract staleness check: PASS at revision `46` before the revision-47 doc bump; re-run required after the continuity and contract updates
- Dedicated breach sealing tier scaling smoke: PASS with marker `BLOODLINES_BREACH_SEALING_TIER_SCALING_SMOKE PASS`

### Recommended Next Fortification Follow-Up
1. Sub-slice 12 worker-locality gating so only idle workers local to the settlement's own control point can contribute to breach sealing.
2. Sub-slice 13 repair narrative after locality lands, so breach closure and destroyed-counter rebuilds emit the required info-tone messages without cross-settlement labor leakage.

## Codex Fortification Siege Sub-Slice 12: Sealing Worker Locality (2026-04-21)

### Status: COMPLETE on branch codex/unity-fortification-sealing-worker-locality

### What Was Done
- `BreachSealingSystem` now resolves each settlement's nearest same-owner control point from the settlement anchor position and only counts idle workers whose own nearest control point matches that anchor.
- The idle-worker scan now requires `PositionComponent`, resolves the worker's nearest `ControlPointComponent`, and rejects workers whose nearest control point is not owned by their faction.
- Same-faction workers near another settlement no longer contribute to breach closure here, which closes the cross-settlement labor-poaching gap without adding a new stored settlement-control-point foreign key.
- New dedicated validator `BloodlinesBreachSealingWorkerLocalitySmokeValidation` plus wrapper `scripts/Invoke-BloodlinesUnityBreachSealingWorkerLocalitySmokeValidation.ps1` prove local sealing, same-faction other-settlement blocking, no-workers blocking, and non-idle blocking.
- Full governed validation is green on `D:\BLF12\bloodlines`, with worktree-safe wrapper copies again used for the still-root-pinned bootstrap runtime and canonical scene-shell validators. Contract bumped revision `47 -> 48`.

### Gate Results
- `dotnet build unity/Assembly-CSharp.csproj -nologo`: PASS
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`: PASS with existing editor warnings only
- Bootstrap runtime smoke: PASS via worktree-local wrapper copy under the Unity lock
- Combat smoke: PASS
- Scene shells: Bootstrap + Gameplay PASS via worktree-local wrapper copies under the Unity lock
- Fortification smoke: PASS
- Siege smoke: PASS
- `node tests/data-validation.mjs`: PASS
- `node tests/runtime-bridge.mjs`: PASS
- Contract staleness check: PASS at revision `48` after the continuity and contract updates
- Dedicated breach sealing worker locality smoke: PASS with marker `BLOODLINES_BREACH_SEALING_WORKER_LOCALITY_SMOKE PASS`

### Validation Notes
- The first dedicated locality-smoke rerun after script compilation exited immediately with return code `1` before the validator executed. A single 10-second retry reached the batch method and passed cleanly, so the slice stays unblocked.

### Recommended Next Fortification Follow-Up
1. Sub-slice 13 repair narrative so breach closures and destroyed-counter rebuilds emit the required info-tone narrative pushes.
2. After the repair narrative lands, continue through any remaining fortification-siege follow-ups or hand the lane back for the next arc.

## Codex Fortification Siege Sub-Slice 13: Repair Narrative (2026-04-21)

### Status: COMPLETE on branch codex/unity-fortification-repair-narrative

### What Was Done
- `BreachSealingSystem` now pushes one info-tone narrative message through `NarrativeMessageBridge` each time a breach closes: `"<faction>'s masons seal a breach at <settlement>."`
- `DestroyedCounterRecoverySystem` now pushes one info-tone repair message each time a destroyed counter rebuild completes, with wall rebuilds matching the prompt-accurate `"<faction> rebuilds a wall at <settlement>."` line and the same pattern reused for other rebuilt fortification kinds.
- New dedicated validator `BloodlinesFortificationRepairNarrativeSmokeValidation` plus wrapper `scripts/Invoke-BloodlinesUnityFortificationRepairNarrativeSmokeValidation.ps1` prove single-breach closure, three-breach closure, and wall-rebuild narrative emission.
- Full governed validation is green on `D:\BLF13\bloodlines`, with worktree-safe wrapper copies still used for the root-pinned bootstrap runtime and canonical scene-shell validators. Contract bumped revision `48 -> 49`.

### Gate Results
- `dotnet build unity/Assembly-CSharp.csproj -nologo`: PASS
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`: PASS with existing editor warnings only
- Bootstrap runtime smoke: PASS via worktree-local wrapper copy under the Unity lock
- Combat smoke: PASS
- Scene shells: Bootstrap + Gameplay PASS via worktree-local wrapper copies under the Unity lock
- Fortification smoke: PASS
- Siege smoke: PASS
- `node tests/data-validation.mjs`: PASS
- `node tests/runtime-bridge.mjs`: PASS
- Contract staleness check: PASS at revision `49` after the continuity and contract updates
- Dedicated fortification repair narrative smoke: PASS with marker `BLOODLINES_FORTIFICATION_REPAIR_NARRATIVE_SMOKE PASS`

### Recommended Next Fortification Follow-Up
1. No queued fortification sub-slice remains after sub-slice 13.
2. Future fortification work now requires an operator-defined sub-slice 14 or a lane handoff to the next arc.

## Codex Fortification Siege Session Wrap 10 Through 13 (2026-04-21)

### Status: CURRENT QUEUE CLOSED

### What Was Done
- The current fortification-siege queue is now closed through sub-slice 13: breach-depth telemetry, sealing cost tier scaling, sealing worker locality, and repair narrative are all landed.
- The slice 12 worker-locality smoke-wrapper fixup was merged before sub-slice 13 so the locality validator is now stable on master instead of depending on the brittle one-off execute-method bridge.
- The fortification lane has no approved next slice. Further work now depends on a fresh operator-defined follow-up rather than an implied continuation.

### Recommended Next Follow-Up
1. Fetch `origin/master`, confirm contract revision `49`, and wait for Lance to define sub-slice 14 if fortification work should continue.
2. Otherwise claim a different approved Codex lane and leave fortification idle.

## 2026-04-21 Scout Raids And Logistics Interdiction Landing

- Branch lane: `codex/unity-scout-raids-logistics-interdiction`
- Merge commit: `dda7c25e`
- Dedicated slice handoffs:
  - `docs/unity/session-handoffs/2026-04-20-unity-scout-raids-and-logistics-interdiction.md`
  - `docs/unity/session-handoffs/2026-04-21-unity-scout-raids-logistics-interdiction-rebase-validation.md`
  - `docs/unity/session-handoffs/2026-04-21-unity-scout-raids-logistics-interdiction-landing.md`
- The landed scout foundation now ports the browser's direct non-AI raid seams:
  - `ScoutRaidCommandComponent`, `BuildingRaidStateComponent`, `ScoutRaidCanon`,
    and `ScoutRaidResolutionSystem` cover building harassment and supply-wagon
    interdiction.
  - `ResourceTrickleBuildingSystem` suppresses passive output from raided
    buildings.
  - `WorkerGatherSystem` reroutes return trips around raided drop-offs while
    keeping resource-acceptance gates intact.
  - `FieldWaterSupportScanSystem` and `SiegeSupportRefreshSystem` now suppress
    raided wells and supply camps until raid expiry.
  - `SkirmishBootstrapSystem` and the debug construction seam now attach
    `BuildingRaidStateComponent` so live runtime and smoke worlds stay aligned.
- The full governed validation chain reran green on detached merged `master`
  in `D:\BLAICD\bloodlines`:
  - `dotnet build unity/Assembly-CSharp.csproj -nologo`: `0 Error(s)`
  - `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`: `0 Error(s)`
  - bootstrap runtime smoke: `Bootstrap runtime smoke validation passed.`
  - combat smoke: all eight phases green
  - scene shells: Bootstrap + Gameplay green
  - fortification smoke: baseline/tier/reserve phases green
  - siege smoke: baseline/strain/recovery/support phases green
  - `node tests/data-validation.mjs`: PASS
  - `node tests/runtime-bridge.mjs`: PASS
  - contract staleness check: PASS
  - dedicated scout raid smoke: `BLOODLINES_SCOUT_RAID_AND_INTERDICTION_SMOKE PASS`

### Recommended Next Follow-Up
1. Under the current multi-day directive, claim the new player-facing marriage
   diplomacy lane next and start with the proposal-execution sub-slice.
2. If Lance reprioritizes raids before diplomacy, continue the scout lane on a
   fresh branch rather than reopening the merged landing branch.

## 2026-04-21 Player Marriage Diplomacy Sub-Slice 2A: Proposal Execution

- Branch lane: `codex/unity-player-marriage-proposal`
- Dedicated slice handoff:
  - `docs/unity/session-handoffs/2026-04-21-unity-player-marriage-proposal.md`
- The landed proposal-execution slice now ports the browser's player-side proposal seam under `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/`:
  - `PlayerMarriageProposalRequestComponent`, `PlayerMarriageAuthorityEvaluator`, and `PlayerMarriageProposalSystem`
    cover player-issued proposal requests, source-governance gating, active-marriage
    / polygamy checks, duplicate blocking, and source-side regency legitimacy cost.
  - `BloodlinesDebugCommandSurface.PlayerDiplomacy.cs` adds proposal issuance and
    readout helpers for governed smoke coverage and future player-surface work.
  - `BloodlinesPlayerMarriageProposalSmokeValidation` plus
    `scripts/Invoke-BloodlinesUnityPlayerMarriageProposalSmokeValidation.ps1`
    prove baseline, success, duplicate block, and already-married block.
- Full governed validation is green in `D:\BLM13\bloodlines\bloodlines`:
  - `dotnet build unity/Assembly-CSharp.csproj -nologo`: `Build succeeded.` / `0 Error(s)`
  - `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`: `0 Error(s)` with existing editor warnings
  - bootstrap runtime smoke: `Bootstrap runtime smoke validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity ...`
  - combat smoke: `Combat smoke validation passed: meleePhase=True, projectilePhase=True, explicitAttackPhase=True, attackMovePhase=True, targetVisibilityPhase=True, groupMovementPhase=True, separationPhase=True, stancePhase=True.`
  - scene shells: `Bootstrap scene shell validation passed ...` and `Gameplay scene shell validation passed ...`
  - fortification smoke: `Fortification smoke validation passed: baselinePhase=True, tierAdvancePhase=True, reserveMusterPhase=True, reserveRecoveryPhase=True.`
  - siege smoke: `Siege smoke validation passed: baselinePhase=True, strainPhase=True, recoveryPhase=True, supportPhase=True.`
  - `node tests/data-validation.mjs`: PASS
  - `node tests/runtime-bridge.mjs`: PASS
  - contract staleness check: PASS
  - dedicated player-marriage proposal smoke: `BLOODLINES_PLAYER_MARRIAGE_PROPOSAL_SMOKE PASS`

### Recommended Next Follow-Up
1. Start player-marriage sub-slice 2B on fresh branch `codex/unity-player-marriage-acceptance`.
2. Port `acceptMarriage` plus `getMarriageAcceptanceTerms`, including legitimacy deltas, hostility drop, oathkeeping gain, 30-day declaration jump, and gestation-ready `MarriageComponent` creation.

## 2026-04-21 Player Marriage Diplomacy Sub-Slice 2A Landing

- Merged branch: `codex/unity-player-marriage-proposal`
- Merge commit on `master`: `21550da3`
- Landing handoff:
  - `docs/unity/session-handoffs/2026-04-21-unity-player-marriage-proposal-landing.md`
- The player marriage proposal slice is now landed on `master`, not just
  validated on its feature branch:
  - `PlayerMarriageProposalRequestComponent`,
    `PlayerMarriageAuthorityEvaluator`, and `PlayerMarriageProposalSystem` are
    in the canonical runtime under
    `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/`.
  - `BloodlinesDebugCommandSurface.PlayerDiplomacy.cs` and
    `BloodlinesPlayerMarriageProposalSmokeValidation` are now part of the
    canonical debug and validation surfaces.
  - Merged `master` re-passed runtime build, editor build, bootstrap runtime,
    combat, scene-shell, fortification, siege, Node validation, and the
    dedicated proposal smoke in `D:\BLM13\bloodlines\bloodlines`.

### Recommended Next Follow-Up
1. Start sub-slice 2B from merged `master` on fresh branch `codex/unity-player-marriage-acceptance`.
2. Keep all work under `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/` plus a
   new acceptance debug/validator seam; do not reopen `unity/Assets/_Bloodlines/Code/AI/**`.

## 2026-04-21 Player Marriage Diplomacy Sub-Slice 2B: Acceptance And Effects

- Branch lane: `codex/unity-player-marriage-acceptance`
- Dedicated slice handoff:
  - `docs/unity/session-handoffs/2026-04-21-unity-player-marriage-acceptance.md`
- The player-side acceptance slice now ports the browser's incoming-marriage accept seam under `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/`:
  - `PlayerMarriageAcceptRequestComponent` and `PlayerMarriageAcceptSystem`
    cover pending-proposal lookup, source/target dynasty + member resolution,
    target-side authority validation, proposal acceptance, mirror marriage
    creation, legitimacy cost resolution, hostility drop, oathkeeping +2, and
    the 30-day `DeclareInWorldTimeRequest`.
  - `BloodlinesDebugCommandSurface.PlayerDiplomacy.cs` now exposes
    `TryDebugIssuePlayerMarriageAccept(proposalEntityIndex)` and includes
    `EntityIndex=...` in the proposal readout so the debug and validator seams
    can target the exact pending proposal entity.
  - `BloodlinesPlayerMarriageAcceptanceSmokeValidation` plus
    `scripts/Invoke-BloodlinesUnityPlayerMarriageAcceptanceSmokeValidation.ps1`
    prove baseline, valid acceptance, no-pending gate rejection, and
    heir-regency legitimacy-cost deduction.
- Full governed validation is green in `D:\BLM13\bloodlines\bloodlines`,
  using temporary worktree-local copies only for the still-root-pinned
  bootstrap-runtime and scene-shell wrapper scripts:
  - `dotnet build unity/Assembly-CSharp.csproj -nologo`: `Build succeeded.` / `0 Error(s)`
  - `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`: `0 Error(s)` with existing editor warnings
  - bootstrap runtime smoke:
    `Bootstrap runtime smoke validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity on map ironmark_frontier. ...`
  - combat smoke:
    `Combat smoke validation passed: meleePhase=True, projectilePhase=True, explicitAttackPhase=True, attackMovePhase=True, targetVisibilityPhase=True, groupMovementPhase=True, separationPhase=True, stancePhase=True.`
  - scene shells:
    `Bootstrap scene shell validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity with canonical map Assets/_Bloodlines/Data/MapDefinitions/ironmark_frontier.asset.`
    and
    `Gameplay scene shell validation passed for Assets/_Bloodlines/Scenes/Gameplay/IronmarkFrontier.unity.`
  - fortification smoke:
    `Fortification smoke validation passed: baselinePhase=True, tierAdvancePhase=True, reserveMusterPhase=True, reserveRecoveryPhase=True. ...`
  - siege smoke:
    `Siege smoke validation passed: baselinePhase=True, strainPhase=True, recoveryPhase=True, supportPhase=True. ...`
  - `node tests/data-validation.mjs`: PASS
  - `node tests/runtime-bridge.mjs`: PASS
  - dedicated player-marriage acceptance smoke:
    `BLOODLINES_PLAYER_MARRIAGE_ACCEPTANCE_SMOKE PASS`
    with
    `Phase 2 PASS: proposal accepted, marriageCount=2, legitimacy=82/72, oathkeeping=5/3, dualClockDays=50`
    and
    `Phase 4 PASS: heir-regency cost applied, legitimacy=81, stewardship=2`

### Recommended Next Follow-Up
1. Merge `codex/unity-player-marriage-acceptance` to `master` and rerun the governed gate on merged `master`.
2. After landing 2B, start sub-slice 2C on fresh branch `codex/unity-player-marriage-dissolution`.
3. Keep the player-marriage lane inside `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/` plus narrow continuity/contract updates; do not reopen `unity/Assets/_Bloodlines/Code/AI/**`.

## 2026-04-21 Player Marriage Diplomacy Sub-Slice 2B Landing

- Merged branch: `codex/unity-player-marriage-acceptance`
- Merge commit on `master`: `00223fa9`
- Landing handoff:
  - `docs/unity/session-handoffs/2026-04-21-unity-player-marriage-acceptance-landing.md`
- The player marriage acceptance slice is now canonical master content:
  - `PlayerMarriageAcceptRequestComponent` and `PlayerMarriageAcceptSystem`
    are now landed under `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/`.
  - `BloodlinesDebugCommandSurface.PlayerDiplomacy.cs` now carries the landed
    player-accept debug seam and proposal-entity index readout.
  - `BloodlinesPlayerMarriageAcceptanceSmokeValidation` and
    `scripts/Invoke-BloodlinesUnityPlayerMarriageAcceptanceSmokeValidation.ps1`
    are now part of the canonical validation surface on `master`.
- Merged `master` re-passed runtime build, editor build, bootstrap runtime,
  combat, scene shells, fortification, siege, `node tests/data-validation.mjs`,
  `node tests/runtime-bridge.mjs`, and the dedicated acceptance smoke in
  `D:\BLM13\bloodlines\bloodlines`.

### Recommended Next Follow-Up
1. Start sub-slice 2C from merged `master` on fresh branch `codex/unity-player-marriage-dissolution`.
2. Port death-driven marriage dissolution while keeping the marriage audit entity alive.
3. Keep all writes inside `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/` plus continuity/contract updates; do not reopen `unity/Assets/_Bloodlines/Code/AI/**`.

## 2026-04-21 Player Marriage Diplomacy Sub-Slice 2C: Dissolution Validation

- Branch lane: `codex/unity-player-marriage-dissolution`
- Dedicated slice handoff:
  - `docs/unity/session-handoffs/2026-04-21-unity-player-marriage-dissolution.md`
- Repo-reality correction for this slice:
  - the actual death-driven dissolution runtime was already landed on `master`
    under `unity/Assets/_Bloodlines/Code/Dynasties/MarriageDeathDissolutionSystem.cs`
    via the paused `dynasty-house-parity` lane
  - this player-marriage slice therefore closes the remaining marriage-diplomacy
    requirement by adding the dedicated player-facing proof surface instead of
    cloning a second dissolution system under `PlayerDiplomacy/`
- Added the dedicated player-marriage dissolution validator surface:
  - `BloodlinesPlayerMarriageDissolutionSmokeValidation`
  - `scripts/Invoke-BloodlinesUnityPlayerMarriageDissolutionSmokeValidation.ps1`
- The 3-phase validator proves:
  - accepted marriages stay active while both spouses live
  - ruler death dissolves both mirror records and promotes the heir through
    `DynastySuccessionSystem`
  - active marriages still gestate a child when no death intervenes
- Full governed validation is green in `D:\BLM13\bloodlines\bloodlines`,
  using temporary worktree-local copies only for the still-root-pinned
  bootstrap-runtime and scene-shell wrappers:
  - `dotnet build unity/Assembly-CSharp.csproj -nologo`: `Build succeeded.` / `0 Error(s)`
  - `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`: `0 Error(s)` with existing editor warnings
  - bootstrap runtime smoke:
    `Bootstrap runtime smoke validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity on map ironmark_frontier. ...`
  - combat smoke:
    `Combat smoke validation passed: meleePhase=True, projectilePhase=True, explicitAttackPhase=True, attackMovePhase=True, targetVisibilityPhase=True, groupMovementPhase=True, separationPhase=True, stancePhase=True.`
  - scene shells:
    `Bootstrap scene shell validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity with canonical map Assets/_Bloodlines/Data/MapDefinitions/ironmark_frontier.asset.`
    and
    `Gameplay scene shell validation passed for Assets/_Bloodlines/Scenes/Gameplay/IronmarkFrontier.unity.`
  - fortification smoke:
    `Fortification smoke validation passed: baselinePhase=True, tierAdvancePhase=True, reserveMusterPhase=True, reserveRecoveryPhase=True. ...`
  - siege smoke:
    `Siege smoke validation passed: baselinePhase=True, strainPhase=True, recoveryPhase=True, supportPhase=True. ...`
  - `node tests/data-validation.mjs`: PASS
  - `node tests/runtime-bridge.mjs`: PASS
  - dedicated player-marriage dissolution smoke:
    `BLOODLINES_PLAYER_MARRIAGE_DISSOLUTION_SMOKE PASS`
    with
    `Phase 1 PASS: alive marriage remained active with MarriageId=marriage-11210497115101494597108105118101451099711411410597`
    and
    `Phase 2 PASS: ruler death dissolved MarriageId=marriage-1121049711510150451141171081011144510010197116104 at day=50.00 and promoted player-bloodline-heir`
    and
    `Phase 3 PASS: live marriage gestated childId=child-marriage-11210497115101514510310111511697116105111110 for headFaction=enemy`

### Recommended Next Follow-Up
1. Merge `codex/unity-player-marriage-dissolution` to `master` and rerun the governed gate on merged `master`.
2. After landing 2C, move to Priority 3 on fresh branch `codex/unity-player-covert-ops-foundation`.
3. Keep the player-marriage lane additive only; do not reopen `unity/Assets/_Bloodlines/Code/Dynasties/MarriageDeathDissolutionSystem.cs` unless the contract is explicitly widened.

## 2026-04-21 Player Marriage Diplomacy Sub-Slice 2C Landing

- Merged branch: `codex/unity-player-marriage-dissolution`
- Merge commit on `master`: `f5bfef1d`
- Landing handoff:
  - `docs/unity/session-handoffs/2026-04-21-unity-player-marriage-dissolution-landing.md`
- The player-marriage diplomacy stack is now fully canonical on `master`:
  - proposal execution is landed
  - acceptance and effects are landed
  - the dedicated dissolution proof surface is landed and green against the
    already-mastered dynasty-parity dissolution runtime
- Merged `master` re-passed runtime build, editor build, bootstrap runtime,
  combat, scene shells, fortification, siege, `node tests/data-validation.mjs`,
  `node tests/runtime-bridge.mjs`, contract staleness, and the dedicated
  player-marriage dissolution smoke in `D:\BLM13\bloodlines\bloodlines`.

### Recommended Next Follow-Up
1. Start Priority 3 on fresh branch `codex/unity-player-covert-ops-foundation`.
2. Port `startEspionageOperation` under `unity/Assets/_Bloodlines/Code/PlayerCovertOps/`.
3. Keep `unity/Assets/_Bloodlines/Code/AI/**` untouched; Claude still owns the AI strategic lane.

## 2026-04-21 Player Covert Ops Sub-Slice 3A: Foundation

- Branch lane: `codex/unity-player-covert-ops-foundation`
- Dedicated slice handoff:
  - `docs/unity/session-handoffs/2026-04-21-unity-player-covert-ops-foundation.md`
- The player-side covert-ops foundation now ports the browser's espionage
  dispatch seam under `unity/Assets/_Bloodlines/Code/PlayerCovertOps/`:
  - `CovertOpKindPlayer`, `PlayerCovertOpsRequestComponent`,
    `PlayerCovertOpsResolutionComponent`, and `PlayerCovertOpsSystem` cover
    player-issued espionage requests, operator resolution
    (`Spymaster` -> `Diplomat` -> `Merchant`), canonical gold/influence cost
    deduction, active target/report dedupe, and the per-faction active-op cap
    combined with Claude's AI-side `DynastyOperationLimits`.
  - `BloodlinesDebugCommandSurface.PlayerCovertOps.cs` adds player espionage
    issuance and structured active-op readout.
  - `BloodlinesPlayerCovertOpsSmokeValidation` plus
    `scripts/Invoke-BloodlinesUnityPlayerCovertOpsSmokeValidation.ps1`
    prove baseline, successful dispatch, insufficient-influence rejection, and
    cap enforcement.
- Full governed validation is green in `D:\BLM13\bloodlines\bloodlines`,
  using temporary worktree-local copies only for the still-root-pinned
  bootstrap-runtime and canonical scene-shell wrappers:
  - `dotnet build unity/Assembly-CSharp.csproj -nologo`: `Build succeeded.` / `0 Error(s)`
  - `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`: `0 Error(s)` with existing editor warnings
  - bootstrap runtime smoke:
    `Bootstrap runtime smoke validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity on map ironmark_frontier. ...`
  - combat smoke:
    `Combat smoke validation passed: meleePhase=True, projectilePhase=True, explicitAttackPhase=True, attackMovePhase=True, targetVisibilityPhase=True, groupMovementPhase=True, separationPhase=True, stancePhase=True.`
  - scene shells:
    `Bootstrap scene shell validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity with canonical map Assets/_Bloodlines/Data/MapDefinitions/ironmark_frontier.asset.`
    and
    `Gameplay scene shell validation passed for Assets/_Bloodlines/Scenes/Gameplay/IronmarkFrontier.unity.`
  - fortification smoke:
    `Fortification smoke validation passed: baselinePhase=True, tierAdvancePhase=True, reserveMusterPhase=True, reserveRecoveryPhase=True. ...`
  - siege smoke:
    `Siege smoke validation passed: baselinePhase=True, strainPhase=True, recoveryPhase=True, supportPhase=True. ...`
  - `node tests/data-validation.mjs`: PASS
  - `node tests/runtime-bridge.mjs`: PASS
  - contract staleness check: PASS
  - dedicated player covert ops smoke:
    `BLOODLINES_PLAYER_COVERT_OPS_SMOKE PASS`
    with
    `Phase 2 PASS: espionage created, gold=155, influence=64, readout='ActivePlayerCovertOpCount=1`
    and
    `Phase 4 PASS: active espionage ops capped at 6 with readout 'ActivePlayerCovertOpCount=6`

### Recommended Next Follow-Up
1. Merge `codex/unity-player-covert-ops-foundation` to `master` and rerun the governed gate on merged `master`.
2. After landing 3A, start sub-slice 3B on fresh branch `codex/unity-player-assassination-sabotage`.
3. Keep all writes under `unity/Assets/_Bloodlines/Code/PlayerCovertOps/` plus narrow continuity/contract updates; do not reopen `unity/Assets/_Bloodlines/Code/AI/**`.

## 2026-04-21 Player Covert Ops Sub-Slice 3A Landing

- Merged branch: `codex/unity-player-covert-ops-foundation`
- Merge commit on `master`: `c18966d6`
- Landing handoff:
  - `docs/unity/session-handoffs/2026-04-21-unity-player-covert-ops-foundation-landing.md`
- The player covert ops foundation is now canonical `master` content:
  - `unity/Assets/_Bloodlines/Code/PlayerCovertOps/` now contains the landed
    player-owned covert-op enum, request component, resolution component, and
    espionage dispatch system.
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.PlayerCovertOps.cs`
    and
    `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerCovertOpsSmokeValidation.cs`
    are now part of the canonical debug and validation surfaces.
  - The lane still remains active for sub-slices 3B and 3C, but there is no
    follow-up branch currently in flight.
- Merged `master` re-passed runtime build, editor build, bootstrap runtime,
  combat, scene shells, fortification, siege, `node tests/data-validation.mjs`,
  `node tests/runtime-bridge.mjs`, contract staleness, and the dedicated player
  covert ops smoke in `D:\BLM13\bloodlines\bloodlines`.

### Recommended Next Follow-Up
1. Start sub-slice 3B from merged `master` on fresh branch `codex/unity-player-assassination-sabotage`.
2. Port `startAssassinationOperation` and `startSabotageOperation` under `unity/Assets/_Bloodlines/Code/PlayerCovertOps/`.
3. Extend the player covert ops smoke to prove assassination target validation and sabotage target validation without reopening `unity/Assets/_Bloodlines/Code/AI/**`.
