# Validation Notes

Validation targets for this slice:

1. Launch the server on Windows with no external dependency beyond local Python and local Ollama.
2. Confirm `/api/bootstrap` returns:
   - model inventory
   - source map
   - canonical source registry
   - resume candidates
3. Confirm `/api/agent/resume` returns a grounded response with provenance.
4. Confirm the GUI loads the Dashboard and Agent Workspace.
5. Confirm write endpoints refuse while the session remains locked.

Post-run observations will be appended here by the platform bootstrap.

## 2026-04-16 Observations

- `python -m compileall D:\ProjectsHome\Bloodlines\continuation-platform`: passed
- `python tests\smoke_test.py`: passed
  - bootstrap completed against the live Bloodlines tree
  - resume mode completed with grounded provenance
  - locked project-write probe refused with `tier_insufficient`
- HTTP validation against a live `python server.py` process: passed
  - `GET /api/bootstrap`: returned continuity health, source map, registry, model inventory, and telemetry
  - `POST /api/agent/resume`: returned `resume_last_good_state`
  - `POST /api/project-write`: refused while locked with `tier_insufficient`
- Latest validated startup scan:
  - files mapped: 134514
  - canonical subset docs ingested: 14
  - local models inventoried: 8
  - runtime: Windows-native Python 3.14
  - WSL required for slice: no

## 2026-04-16 Expanded Registry Pass

- `python tests\smoke_test.py`: passed again after broadening the discovered registry and GUI/API surfaces.
- Latest validated expanded scan:
  - files mapped: 134514
  - canonical subset docs ingested: 14
  - discovered registry documents: 5355
  - conflict clusters surfaced: 40
  - open tasks parsed into task board: 12
- Live HTTP validation also passed for the new breadth endpoints:
  - `GET /api/discovered-registry`
  - `GET /api/tasks-board`
  - `GET /api/handoff-preview`
- `resume_last_good_state` now preserves continuity by overriding stale model drift with the current project-work priority extracted from `NEXT_SESSION_HANDOFF.md`.

## 2026-04-16 Product-Ready Pass

- `python -m compileall D:\ProjectsHome\Bloodlines\continuation-platform`: passed again after the final product pass.
- `node --check D:\ProjectsHome\Bloodlines\continuation-platform\static\app.js`: passed.
- `python tests\smoke_test.py`: passed again after:
  - adding the diff-awareness surface and generated `change_report.json`
  - adding telemetry drilldowns and recent write or scan history exposure
  - replacing preview-only handoff export with a fuller handoff builder and frontier re-entry prompt
  - adding explicit resume-anchor selection when continuity health is `attention`
  - pruning Unity cache and temp noise from the governed scan scope
- Latest validated product-ready scan:
  - files mapped in governed source scope: 3179
  - canonical subset docs ingested: 14
  - discovered registry documents: 900
  - discovered conflict clusters: 161
  - promoted diff-watch conflict clusters: 25
  - current high-signal changed documents: 21
  - open tasks parsed into task board: 10
  - latest scan duration: 12.592s
- Live HTTP validation passed for the product-ready endpoints and flows:
  - `GET /api/bootstrap`
  - `GET /api/change-report`
  - `GET /api/telemetry-drilldown`
  - `GET /api/handoff-builder`
  - `POST /api/select-anchor`
  - `POST /api/agent/resume`
  - locked `POST /api/project-write`
- Latest validated resume result after anchor-selection handling:
  - recommended next step: `1. In Unity 6.3 LTS, open unity/Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity.`

## 2026-04-16 Quality-Of-Life Pass

- `python -m compileall D:\ProjectsHome\Bloodlines\continuation-platform`: passed again after the operator-flow QoL pass.
- `node --check D:\ProjectsHome\Bloodlines\continuation-platform\static\app.js`: passed again after the UI ergonomics changes.
- `python tests\smoke_test.py`: passed again after:
  - adding persistent active-view state across refreshes
  - adding active-view filtering for list, table-row, detail-line, workspace-card, and timeline surfaces
  - adding quick-jump controls for Resume Anchor, Next Work, High Signal, Handoff Prompt, and Telemetry
  - adding direct copy actions for the recommended next step plus handoff prompt and preview
  - adding dashboard-side clearing of manual resume-anchor override
  - replacing blocking browser alerts with in-app toast feedback for rescan, resume, unlock, export, and copy flows
- Latest validated QoL-pass scan:
  - files mapped in governed source scope: 3176
  - canonical subset docs ingested: 14
  - detected frontier artifacts: 22
  - discovered registry documents: 894
  - discovered conflict clusters: 158
  - promoted diff-watch conflict clusters: 23
  - current high-signal changed documents: 24
  - open tasks parsed into task board: 13
  - latest scan duration: 14.136s
- Smoke validation re-confirmed:
  - `GET /api/bootstrap`
  - `POST /api/agent/resume`
  - `GET /api/discovered-registry`
  - `GET /api/tasks-board`
  - `GET /api/change-report`
  - `GET /api/telemetry-drilldown`
  - `GET /api/handoff-builder`
  - `GET /api/handoff-preview`
  - locked `POST /api/project-write`
- Latest validated resume anchor after the QoL pass:
  - `manual_edit`
  - source path: `continuity/PROJECT_STATE.json`
- Latest validated continuation recommendation remains:
  - `1. In Unity 6.3 LTS, open unity/Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity.`

## 2026-04-16 Execution Packet And Governed Write Workbench

- `python -m compileall D:\ProjectsHome\Bloodlines\continuation-platform`: passed after dynamic canonical subset augmentation, execution-packet generation, and the governed write workbench landed.
- `node --check D:\ProjectsHome\Bloodlines\continuation-platform\static\app.js`: passed after adding the Execution view and the write-workbench UI.
- `python tests\smoke_test.py`: passed after:
  - expanding canonical subset ingestion from a static list into a live current subset that now includes the owner direction, latest Unity handoff, latest project-gap report, latest continuation prompt, and continuation-platform README
  - generating `state/execution_packet.json` with the current Unity shipping-lane next step, verification commands, manual checklist, canonical sources, and governed write targets
  - adding governed project-file load plus write preview and apply coverage
  - proving an unlocked tier-3 project write can update a real file under `test-results/` instead of only refusing writes in a sandbox posture
- Latest validated platform state after the execution-packet pass:
  - files mapped in governed source scope: `3182`
  - canonical subset docs ingested: `19`
  - detected frontier artifacts: `22`
  - discovered registry documents: `903`
  - conflict clusters surfaced: `158`
  - current high-signal changed documents: `16`
  - open tasks parsed into task board: `10`
- Live HTTP validation passed for the new production endpoints and flows:
  - `GET /api/bootstrap`
  - `GET /api/execution-packet`
  - `POST /api/project-file`
  - `POST /api/project-write/preview`
- Latest validated execution-packet lane:
  - `unity_shipping`
- Latest validated continuation recommendation after the task-priority parser fix:
  - `run scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1 if structural shell integrity should be re-confirmed before in-editor work`
