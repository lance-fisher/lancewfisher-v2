Operate only in D:\ProjectsHome\Bloodlines.

Bloodlines is a large-scale dynastic RTS. No scope reduction, no decorative dead UI, no canon compression, no architecture replacement. Preserve all live systems and continue converting canon into runtime reality.

Before editing, read in order:
1. AGENTS.md
2. README.md
3. CLAUDE.md
4. MASTER_PROJECT_INDEX.md
5. MASTER_BLOODLINES_CONTEXT.md
6. CURRENT_PROJECT_STATE.md
7. NEXT_SESSION_HANDOFF.md
8. SOURCE_PROVENANCE_MAP.md
9. continuity/PROJECT_STATE.json
10. 01_CANON/BLOODLINES_MASTER_DESIGN_DOCTRINE_2026-04-14.md
11. docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_9.md
12. docs/BLOODLINES_SYSTEM_GAP_ANALYSIS_2026-04-14_SESSION_9.md
13. docs/plans/2026-04-14-session-9-full-realization-continuation-plan.md
14. docs/plans/2026-04-14-session-9-next-phase-execution-roadmap.md
15. docs/unity/session-handoffs/2026-04-16-unity-constructed-barracks-production-continuity.md
16. tasks/todo.md
17. HANDOFF.md

Current live state to preserve:
- Session 100: first governed Unity production slice is live with controlled `command_hall` selection and `villager` training.
- Session 101: queue cancel and refund are live and governed-runtime verified.
- Session 102: two-deep queue issue, rear-entry cancel, refund, and surviving front-entry completion are governed-runtime verified.
- Session 103: first worker-led construction slice is live with `dwelling` placement, ECS construction progression, completion, and `populationCap=24`.
- Session 104: constructed production continuity is governed-runtime verified, including worker-led `barracks` placement, barracks completion, controlled post-completion selection, `militia` queue issuance, queue drain, and final totals of `11` buildings, `18` units, and `8` controlled units.
- Canonical scene shells are structurally valid and the machine-readable continuity file parses cleanly.

Next required action:
Open the Unity battlefield shell in the real editor and manually verify the full Session 104 command, construction, and constructed-production flow in Play Mode.

Requirements for that work:
- Use Unity 6.3 LTS `6000.3.13f1`.
- If a governed preflight is needed, run `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` first and `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` second. Do not run Unity wrappers in parallel against the same project.
- Open `unity/Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity`.
- Manually verify unit select, building select, drag-box selection, `1` select-all, `Ctrl+2-5` save, `2-5` recall, `F` frame, clear-selection, formation-aware move, and camera feel.
- Manually verify `command_hall -> villager` queue-row visibility, rear-entry cancel, refund, and surviving front-entry completion feel.
- Manually verify worker construction-panel visibility, pending placement, obstruction rejection, `dwelling` placement, completion timing, and final population-cap increase.
- Manually verify `barracks` completion, post-completion selection, `militia` queue visibility, training completion, and controlled-unit growth feel.
- Manually verify control-point capture, contested decay, stabilization, and uncontested trickle.
- After manual verification, continue directly into broader construction-roster verification, construction progress UI, deeper build-placement UX, broader production-roster verification, or production from additional newly completed buildings. Do not prioritize attack-move until a real combat lane exists.

Before closing, run:
- node tests/data-validation.mjs
- node tests/runtime-bridge.mjs
- dotnet build unity/Assembly-CSharp.csproj -nologo
- dotnet build unity/Assembly-CSharp-Editor.csproj -nologo
- powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1
- powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1

Then verify `Bootstrap.unity` in Play Mode as far as the environment allows.

Before ending, update additively:
- CURRENT_PROJECT_STATE.md
- NEXT_SESSION_HANDOFF.md
- HANDOFF.md
- continuity/PROJECT_STATE.json
- unity/README.md
- unity/Assets/_Bloodlines/Code/README.md
- docs/unity/session-handoffs/2026-04-16-unity-constructed-barracks-production-continuity.md or the next dated Unity handoff if a new slice lands
