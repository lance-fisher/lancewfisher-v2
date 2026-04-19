# NEXT_SESSION_HANDOFF

Last updated: 2026-04-18 (Session 130: All 3 items complete. Victory Conditions System: VictoryStateComponent + VictoryConditionEvaluationSystem, 4-phase smoke PASS, contract revision 12. Tier 2 batch dynasty: 5 systems + smoke PASS, contract 11. AI strategic layer sub-slice 1: EnemyAIStrategySystem, 4-phase smoke PASS, contract 10. Next: Fortification + Siege system (codex/unity-fortification-siege branch has initial components.))
Previous entry: Last updated: 2026-04-18 (Session 129: AI strategic layer sub-slice 1 complete -- EnemyAIStrategySystem ISystem live, 4-phase smoke PASS, contract revision 10. Next sub-slices: supply chain/convoy mgmt (ai.js ~1100), siege staging. Also pending: Tier 2 batch systems (marriage, lesser house defection, minor house levies, renown scoring) and Victory Conditions system.)
Previous entry: Last updated: 2026-04-17 (state-snapshot-restore lane complete on master 2026-04-17: 3 sub-slices merged, BloodlinesSnapshotPayload/Writer/Restorer, 6-phase smoke, ProbeSnapshotIntegrity in bootstrap runtime smoke, all 10 gates green, contract Revision 5. 10 Claude Code skills merged to master. Concurrent session contract revision 5. Browser-to-Unity migration plan drafted and three Tier 1 slices landed on master the same day: Conviction scoring + bands + 4-phase governed validator, Dynasty core with eight-member template set + aging + heir succession + interregnum + 4-phase validator, Faith commitment + exposure threshold + five-tier intensity resolution + 4-phase validator. AI barracks observability and bootstrap runtime smoke startup timeout bump 120s to 240s also on master. Codex group-movement and combat-stances slice is now rebased, green, and pushed on `codex/unity-group-movement-and-stances` with all eight combat smoke phases passing, full governed gate chain green, `Assembly-CSharp.csproj` compile includes restored for the new runtime files, and governed wrapper hardening landed for bootstrap runtime, scene-shell, and graphics validations. Workspace `HANDOFF.md` archived to `HANDOFF_ARCHIVE_2026-04-17_session-125-attack-orders.md`; the browser-to-Unity migration plan at `docs/plans/2026-04-17-browser-to-unity-migration-plan.md` is now the authoritative forward-work map; Session 127: Unity passive target-acquisition throttling and sight-loss cleanup green on stacked branch `codex/unity-target-acquisition-los`; Session 126: Unity explicit attack orders and first attack-move command layer green on branch `codex/unity-attack-move`; all prior lanes preserved)
Previous author: Claude
Next recommended action: the migration plan at `docs/plans/2026-04-17-browser-to-unity-migration-plan.md` is the canonical forward-work map. Completed Tier 1 items: conviction/dynasty/faith, combat foundation, projectile combat, attack orders/attack-move, target-acquisition throttling, economy/AI lanes (starvation/cap-pressure/stability-surplus/loyalty/trickle/gather), state-snapshot-restore (complete 2026-04-17). Remaining Tier 1 items in suggested dependency order: fortification + siege + imminent-engagement (codex/unity-fortification-siege branch in flight; port fortification tiers and reserves; depends on dual-clock for stage-4 triggers), dual-clock + five-stage match progression + declaration seam, full enemy AI strategic layer port from `src/game/core/ai.js`.

## Active Owner Direction

- The active non-negotiable direction is recorded at `governance/OWNER_DIRECTION_2026-04-16_FULL_CANON_UNITY.md`.
- Bloodlines now proceeds under these hard rules:
  - full canonical design-bible realization stays in scope
  - Unity 6.3 LTS with DOTS / ECS is the shipping engine
  - the browser runtime is frozen as behavioral specification only
  - new implementation belongs in `unity/`
  - full commercial polish, Wwise audio, and Netcode for Entities scope remain in force unless Lance changes that direction later
  - MVP, phased-release, and scope-cut reasoning are stale and forbidden
- Any older browser follow-up tasks or phased-delivery language below are historical context only. Interpret them as Unity implementation targets, not as permission to add new browser systems.

## Project Completion Snapshot

- The canonical Bloodlines root is stable and continuity-safe.
- The continuation platform is product-ready for daily offline use, now carries a live Unity execution packet plus governed canonical write workbench, and now opens on a true chat-first Command Deck rather than a dashboard-only front page.
- The browser reference simulation is heavily built, frozen as behavioral spec, and already carries many live dynasty, siege, diplomacy, sovereignty, and naval systems.
- The Unity lane is green through first-shell battlefield control, production, construction, constructed `barracks -> militia` continuity, worker gather/deposit, projectile combat delivery, explicit attack orders, attack-move, target-acquisition throttling / sight-loss cleanup, group-aware movement, soft unit separation, and combat stances.
- Graphics staging is through Batch 08 and review infrastructure is in place.
- The foundational player guide exists as a completed Volume I.
- The dated supporting project-wide report now exists at `reports/2026-04-16_project_completion_handoff_and_gap_summary.md`.

## Remaining Before Bloodlines Is Entirely Done

- Unity still needs broader gameplay depth, more construction and production coverage, richer runtime UI, fortification / siege and match-progression Tier 1 systems, and deeper combat presentation or balance beyond the current projectile + explicit-order + stance foundation.
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
2. Faith Hall (L2) canonical covenant building — built on top of committed-covenant Wayshrine; unlocks L3 unit recruitment seat
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
- Commander keep-presence sortie is now live. `issueKeepSortie(state, factionId, settlementId)` grants defenders ×1.22 attack and +22 sight for 12 seconds with a 60-second cooldown. Requires commander at keep AND active threat.
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

All four covenants are still `prototypeEnabled: false` in `data/faiths.json`. Flip the flags, add per-covenant building progression (Wayshrine → Hall → Grand Sanctuary equivalents), add L3 faith unit roster (8 units: 2 per covenant per doctrine path). Extend `updateFaithExposure` and ward profiles to consume the new structures.

### Step 5 (Lance first-open Unity menu run)

Lance-gated. Run `Bloodlines → Import → Sync JSON Content` in Unity 6.3 LTS (6000.3.13f1) to generate ScriptableObject `.asset` files. Commit generated assets.

### Step 6 (Session-by-session roadmap)

See `docs/plans/2026-04-14-session-9-next-phase-execution-roadmap.md` for the full ordered sequence.

## Old Next Recommended Action (Session 11, now complete)

### Step 1 (Lance opens Unity for first time under 6.3 LTS)

Open `D:\ProjectsHome\Bloodlines\unity\` in Unity 6.3 LTS (`6000.3.13f1`). Accept any LTS-compatible package version adjustments the Package Manager offers. Let Unity regenerate `Library/`. Verify the authored session 10 ECS layer compiles cleanly.

Then run `Bloodlines → Import → Sync JSON Content`. Commit the generated ScriptableObject assets under `Assets/_Bloodlines/Data/*/`.

### Step 2 (Second playable house — Stonehelm)

Enable Stonehelm as `prototypePlayable: true` in `data/houses.json`. Add minimal URL-driven house-select to `play.html` so the skirmish can launch as Stonehelm via `play.html?house=stonehelm`. Give Stonehelm at least one distinguishing mechanical hook (fortification cost discount per `04_SYSTEMS/TERRITORY_SYSTEM.md`).

### Step 3 (Hartvale Verdant Warden entry)

Add the canonical Hartvale unique unit to `data/units.json`. Mark `house: "hartvale"`, `prototypeEnabled: false` until Hartvale becomes playable. Update `tests/data-validation.mjs` to assert the unit exists with canonical Off 4 / Def 5 profile.

### Step 4 (AI sabotage reciprocity)

Give Stonehelm AI the ability to run sabotage against player-owned targets. Spymaster-gated the same way, same success formula. Natural counter-pressure emerges when the player has exposed gates, supply camps, or wells.

### Step 5 (Longer-siege AI adaptation — continuing session 12 scope if time allows)

Relief-window awareness, repeated-assault window logic, supply-protection patrols, post-repulse tactical adjustment.

### Step 6 (Continued ECS foundation — session 14 scope if time allows)

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
- `docs/unity/CONCURRENT_SESSION_CONTRACT.md` -- revision 5 → 6, dual-clock-match-progression lane claimed as active

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
