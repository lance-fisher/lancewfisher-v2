Operate only in `D:\ProjectsHome\Bloodlines`.

Bloodlines remains a full-scale canonical dynastic RTS. No scope reduction, no browser-lane expansion, no alternate Unity target, no MVP framing, no destruction of preserved material. Preserve every live lane and continue converting canon into the real Unity shipping runtime.

Before editing, read in order:
1. `AGENTS.md`
2. `README.md`
3. `CLAUDE.md`
4. `MASTER_PROJECT_INDEX.md`
5. `MASTER_BLOODLINES_CONTEXT.md`
6. `CURRENT_PROJECT_STATE.md`
7. `NEXT_SESSION_HANDOFF.md`
8. `SOURCE_PROVENANCE_MAP.md`
9. `continuity/PROJECT_STATE.json`
10. `governance/OWNER_DIRECTION_2026-04-16_FULL_CANON_UNITY.md`
11. `reports/2026-04-16_project_completion_handoff_and_gap_summary.md`
12. `reports/2026-04-16_continuation_platform_execution_packet_and_governed_write_workbench.md`
13. `docs/plans/2026-04-16-bloodlines-finalization-execution-checklist.md`
14. `docs/unity/session-handoffs/2026-04-16-unity-production-progress-observability-and-world-space-bar.md`
15. `tasks/todo.md`
16. `HANDOFF.md`

Current live state to preserve:
- `D:\ProjectsHome\Bloodlines\continuation-platform\launch_windows.cmd` and `http://127.0.0.1:8067` are now the primary offline Bloodlines continuation surface.
- The continuation platform is chat-first through `Command Deck`, but it also carries the governed `Execution` view, canonical source spine, handoff builder, telemetry, and governed write flow.
- The current execution packet resolves to the Unity shipping lane and `unity/Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity`.
- Unity is green through first-shell selection, drag-box, control groups, framing, formation-aware move, control-point capture, production queue issue/cancel/refund, worker-led construction, constructed `barracks -> militia` continuity, construction progress observability, and production progress observability.
- Browser runtime remains frozen as behavioral specification only.
- Graphics staging is through Batch 08, but runtime-ready production assets, audio, multiplayer, and full end-to-end shipping polish are still unfinished.

Required continuation sequence:
1. Launch `D:\ProjectsHome\Bloodlines\continuation-platform\launch_windows.cmd`.
2. Work from `Command Deck` first:
   - `/status`
   - `/resume`
   - `/execution`
   - `/read NEXT_SESSION_HANDOFF.md`
   - use grounded natural-language prompts only after the anchor and execution packet are confirmed
3. Open the `Execution` view and confirm:
   - lane is `unity_shipping`
   - scene target is `unity/Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity`
   - the canonical source spine is current
   - the recommended next step still matches the live Unity lane
4. If a governed preflight is needed, run serially and never in parallel:
   - `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1`
   - `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1`
   - `scripts/Invoke-BloodlinesUnitySyncJsonContent.ps1` only if the JSON mirror must be refreshed before editor work
5. In Unity 6.3 LTS `6000.3.13f1`, open `unity/Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity`.
6. In Play Mode, manually verify the full current first shell:
   - unit select
   - building select
   - drag-box selection
   - `1` select-all
   - `Ctrl+2-5` save
   - `2-5` recall
   - `F` frame
   - clear-selection
   - formation-aware move
   - battlefield camera feel
   - control-point capture, contested decay, stabilization, and uncontested trickle
   - `command_hall -> villager` two-deep queue issue, rear-entry cancel, refund, and surviving front-entry completion feel
   - worker construction-panel visibility, pending placement, obstruction rejection, `dwelling` placement, completion timing, and final population-cap increase
   - `barracks` completion, post-completion selection, `militia` queue visibility, training completion, and controlled-unit growth feel
7. After that manual verification, do not stop. Continue directly into the next highest-value unblocked Unity finalization slices in this order:
   - broader construction roster and deeper build-placement UX
   - broader production roster and production from additional newly completed buildings
   - deeper runtime UI for economy, logistics, and realm-state legibility
   - real combat lane and only then attack-move or battlefield abilities
   - stronger Unity AI behavior and parity-driven migration of canon load-bearing systems
   - asset, audio, multiplayer, polish, and QA follow-up once the core gameplay runtime is stable enough to absorb them

The governing checklist for what still separates Bloodlines from a truly finished state is:
- `docs/plans/2026-04-16-bloodlines-finalization-execution-checklist.md`

Definition of done for the next session:
- not another report-only stop
- not just a platform check
- at least one additional real Unity shipping-lane improvement beyond manual verification if the environment allows it
- continuity updated so the next session can continue immediately without reconstructing context

Before closing, run as many of these as the environment allows:
- `node tests/data-validation.mjs`
- `node tests/runtime-bridge.mjs`
- `dotnet build unity/Assembly-CSharp.csproj -nologo`
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
- `powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1`
- `powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1`

Before ending, update additively:
- `CURRENT_PROJECT_STATE.md`
- `NEXT_SESSION_HANDOFF.md`
- `HANDOFF.md`
- `continuity/PROJECT_STATE.json`
- `unity/README.md`
- `unity/Assets/_Bloodlines/Code/README.md`
- the next dated Unity handoff under `docs/unity/session-handoffs/`

This file is the current ready-to-paste next-session continuation prompt.
