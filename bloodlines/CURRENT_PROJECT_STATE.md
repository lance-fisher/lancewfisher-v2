# CURRENT_PROJECT_STATE

Last updated: 2026-04-17 (Session 113: Unity combat foundation green on branch `codex/unity-combat-foundation` with CombatStats/AttackTarget/Hostility runtime payloads, auto-acquire/attack/death systems, bootstrap + production combat-stat wiring, dedicated combat smoke validation, normalization of browser-spec combat distances into Unity world-space via `map.tileSize`, dedicated combat smoke pass, and bootstrap runtime smoke preserved green in both the worktree-equivalent batch pass and the unchanged governed wrapper; Session 112: Unity worker gather-deposit primary economy loop green with WorkerGatherComponent, WorkerGatherSystem, nearest-command-hall drop-off targeting, all seven canonical primary resource routes into ResourceStockpileComponent, debug API for gather assignment and stockpile inspection, governed 5-worker gold-gather cycle proof in bootstrap runtime smoke artifact, 75-to-120-second startup timeout bump, and first origin push of Sessions 100-108 + full canonical corpus + Unity source + ScriptableObject data + continuation-platform source to lance-fisher/lancewfisher-v2 master; Session 111: finalization handoff and next-session continuation prompt refreshed so the current Session 110 command-deck and Unity-shipping state is the canonical re-entry path instead of the obsolete Session 104 prompt; Session 110: continuation platform chat-first Command Deck green with persistent offline back-and-forth Bloodlines agent conversation, slash-command local actions, local-model prompt handling, governed write-draft staging and apply flow, live HTTP validation, and `8067` rebuilt as the primary offline continuation surface; Session 109: continuation platform execution packet and governed write workbench green with dynamic current-canon ingestion, Unity shipping-lane execution view, safe project-file load and diff preview, real tier-gated project-artifact apply with stale-source protection and backups, live HTTP validation, and current Unity recommendation repair; Session 108: Unity first-shell production progress observability green with world-space production progress bar, debug API production progress getter, governed mid-production queue advancement proof carried into the bootstrap runtime smoke artifact, and spawn-floor and equality gates scoped to allow mid-production observation; Session 107: Unity first-shell construction progress observability and UI green with selection-aware construction-progress panel, world-space construction progress bar, debug API progress getter, and governed mid-construction advancement proof carried into the bootstrap runtime smoke artifact; Session 106: owner direction refresh saved with full-canon Unity 6.3 DOTS/ECS delivery, browser freeze, and no-MVP rule; Session 105: project-completion handoff refreshed with completed-vs-remaining summary; Session 104: Unity constructed production continuity green with worker-led barracks placement, militia training from the newly completed building, runtime-smoke validation, scene validation, and Node checks; Session 103: Unity first governed construction slice green with worker-led building placement, construction progression, dwelling completion, population-cap verification, scene validation, and Node checks; Session 102: Unity two-deep production queue tail-cancel validation green with governed rear-entry refund verification, surviving front-entry completion, scene validation, and Node checks; Session 101: Unity production queue cancel-and-refund slice green with queue-row UI, refund-safe payload metadata, governed cancel verification, scene validation, and Node checks; Session 100: Unity first governed production slice green with building selection, command-hall villager training, runtime-smoke validation, scene validation, JSON sync, and Node checks; Session 96: Naval world integration with vessel dispatch, fishing gather, naval combat, fire ship sacrifice, water-spawn, and 7 test assertions; Session 95: Trueborn recognition diplomacy with Rise pressure exemption, dynasty-panel UI, and 10 test assertions; Session 94: Trueborn Rise arc with three-stage escalation, loyalty/legitimacy pressure, challenge tracking, HUD exposure, save/restore, and 10 test assertions; Session 93: Trueborn City neutral-faction foundation with trade relationships, acceptance endorsement, save/restore, and 9 test assertions; Session 92: player-facing dynasty-panel diplomacy UI with propose and break pact action buttons, browser-verified; Session 91: AI non-aggression pact awareness with succession/army/governance pressure triggers and 4 test assertions; Session 90: non-aggression pact diplomacy system with alliance-threshold counterplay, save/restore, dynasty-panel, and 13 test assertions; Session 89: conviction, covenant, and tribal acceptance factors added to Territorial Governance population-acceptance model with dynasty-panel legibility and test coverage; Session 88: governance alliance-threshold coalition pressure stabilized with save/restore, snapshot exposure, dynasty-panel legibility, HUD world-pill exposure, and runtime bridge test coverage; prior unreported Session 89 code documented; continuation platform quality-of-life pass validated with persistent view state, in-view filtering, quick-jump controls, copy actions, toast feedback, and manual anchor clear; Bloodlines continuation platform thin slice running and validated; Unity bootstrap runtime smoke green with command-shell validation, control groups, framing, and battlefield HUD; Unity canonical scene-shell validation coverage added and batch-validated; Unity definition bindings repaired; Bootstrap scene canonical map assignment recovered; Unity drag-box selection shell added; Unity selection and formation-move shell preserved; Unity control-point capture foundation added; Unity scene-scaffold, battlefield camera, and debug shell added; Unity bootstrap and territory-yield foundation added; foundational player guide drafted; graphics lane Batch 08 settlement-variant wave integrated; Unity graphics staging advanced through Batch 08 with browser-first raster fallback; Unity graphics testbeds refreshed through Batch 08 and batch-review governance applied; Unity movement foundation added; non-canonical Unity template under `unity/My project` classified)

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

## 2026-04-17 Session 113 Unity Combat Foundation

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
