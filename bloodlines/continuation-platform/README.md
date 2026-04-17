# Bloodlines Continuation Platform

Local offline continuation cockpit for `D:\ProjectsHome\Bloodlines`.

This slice is intentionally contained inside the canonical Bloodlines root so it inherits Bloodlines governance, continuity, and source boundaries without creating a second project root.

## What The Platform Does

- Scans the governed Bloodlines source scope, while pruning Unity cache and temp noise so the continuity layer tracks meaningful project state instead of editor churn
- Builds an authority-scored canonical registry, a broader discovered registry, and local FTS5 retrieval over the authoritative subset
- Detects file changes across frontier-session artifacts, authoritative continuity files, code, prompts, reports, and manual edits
- Tracks last-good-state candidates, surfaces ambiguity, and lets the operator explicitly select the resume anchor when multiple candidates compete
- Builds a live Unity execution packet from the current owner direction, handoff, latest Unity lane handoff, and canonical continuity files
- Surfaces a Windows-first local GUI with Dashboard, Agent Workspace, Execution, Tasks, Memory, Diff Panel, Timeline, Handoff Builder, Telemetry, and Config views
- Opens on a chat-first Command Deck that supports back-and-forth Bloodlines prompts, slash commands, local-model reasoning, citations, and governed write-draft staging
- Routes local tasks across the actual Ollama inventory on this machine and makes those routing decisions visible in the UI
- Keeps project-artifact writes locked unless the session unlocks at the required Boss tier, while still allowing governed file load and write preview
- Applies real project-artifact writes inside the Bloodlines root after unlock, with tier checks, stale-source protection, and automatic backup capture

## Runtime

- Primary runtime: Windows-native Python 3.14
- GUI: local browser UI served by the Python server
- Network posture: localhost only
- WSL posture: not required for the thin slice

## Launch

```powershell
cd D:\ProjectsHome\Bloodlines\continuation-platform
python server.py --open
```

Or run:

```powershell
D:\ProjectsHome\Bloodlines\continuation-platform\launch_windows.cmd
```

Default URL:

- `http://127.0.0.1:8067`

## Key Files

- `server.py`: local HTTP server and API surface
- `lib/core.py`: scan, registry, retrieval, routing, continuity, and agent logic
- `config/`: scan, routing, doctrine, and tier-gate configuration
- `state/`: generated registry, journal, source-map, telemetry, and continuity outputs
- `docs/`: audit, design, decisions, degraded modes, and validation notes

## Product-Ready Operator Flow

1. Launch `launch_windows.cmd`
2. Work from the Command Deck on the main screen for the primary loop:
   - type natural-language prompts for local Bloodlines reasoning
   - use slash commands like `/resume`, `/status`, `/search`, `/read`, `/drafts`, and `/apply-draft`
   - review citations, actions taken, and any governed write drafts in the right rail
3. If continuity health is `attention`, select the intended resume anchor in the Resume Anchor card or with `/anchor <candidate>`
4. Run `Resume Last Good State` from the hero bar, the Command Deck, or `/resume`
5. Open the Execution view to confirm the current Unity shipping-lane packet, scene target, validation commands, and in-editor verification checklist
6. Use the governed write workbench or draft-apply flow when canonical project files need to be updated after the session is unlocked at the required tier
7. Use Diff Panel, Telemetry, and Tasks to decide the next approved move
8. Use Handoff Builder when packaging continuity back to Claude Code or Codex

## Daily-Use Quality Of Life

- The UI remembers the last active view across refreshes and relaunches.
- The operator filter now scopes to the active view, so large task, diff, memory, timeline, and telemetry surfaces can be narrowed in place.
- Quick-jump controls move directly to Resume Anchor, Next Work, High Signal, Handoff Prompt, and Telemetry.
- Handoff Prompt, Handoff Preview, and the current recommended next step can be copied directly from the UI.
- Manual resume-anchor overrides can now be cleared from the Dashboard without restarting the session.
- Rescan, resume, unlock, export, and copy actions now report through in-app toast feedback instead of blocking browser alerts.
- The Command Deck is now the default main screen, giving the platform a Codex/Claude-style local conversation loop for Bloodlines continuation.
- Natural-language turns can call the local model, while slash commands hit governed local actions directly.
- Draft file updates can now be staged from the conversation thread and applied later through the same tier gate as the workbench.

## Execution And Writes

- The Execution view is the current canonical operator surface for the active Unity lane.
- The execution packet is generated into `state/execution_packet.json` during each rescan.
- The governed write workbench can:
  - load any text project file inside `D:\ProjectsHome\Bloodlines`
  - preview a write with required tier, hashes, added or removed line counts, and unified diff preview
  - apply the write after unlock with backup capture under `state/backups/`
- The write workbench is not a sandbox-only preview. It targets the real canonical Bloodlines root and uses the same tier gate as the API.

## Scope Boundary

This app is Bloodlines-only. It never scans outside `D:\ProjectsHome\Bloodlines`, and it treats cross-project contamination as a hard failure.
