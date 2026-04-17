# System Design

## Goal

Build a local continuation cockpit that can reopen Bloodlines after a frontier-model session ends, detect what changed, recover the latest authoritative state, and propose the next grounded action without cross-project contamination.

## Current Delivery Boundary

Implemented now:

- Canonical subset ingestion
- Dynamic canonical subset augmentation for the current owner direction, latest Unity handoff, latest project-gap summary, and latest continuation prompt
- Authority-scored source registry
- SQLite continuity store with FTS5 retrieval
- Last-good-state resolution
- Command Deck main-screen conversation surface with persistent local chat session
- Slash-command local actions for `/resume`, `/rescan`, `/status`, `/search`, `/read`, `/anchor`, and governed draft handling
- Local-model conversational turns with citations, tool-use budget, and write-draft staging
- Resume agent mode: `resume_last_good_state`
- Unity execution-packet generation from the live handoff and continuity stack
- Dashboard, Agent Workspace, Execution, Tasks, Memory, Diff Panel, Timeline, Handoff Builder, Telemetry, and Config surfaces
- Governed project-file load and write preview
- Governed project-artifact apply after unlock with tier checks, stale-source protection, and automatic backups
- Windows-first launch path
- Read-only default posture with hashed tier-gate verification until the operator unlocks the session

Deferred to later breadth phases:

- Dense vector retrieval
- Batch execution across multiple agent modes
- More autonomous multi-step execution beyond the current governed operator flow

## Runtime Decision

- Primary runtime: Windows-native Python 3.14
- Reason: Python is already dominant in local infrastructure, sqlite3 and FTS5 ship in stdlib, Windows launch is straightforward, and WSL is currently stopped on this machine
- GUI delivery: local browser app served over localhost by the Python server

## Foundation Decision

The existing `project-hub` dashboard at `http://localhost:8090` was evaluated first and rejected as the initial code foundation for this slice.

Why it was not extended in the first pass:

- `project-hub` is a protected global infrastructure path
- it is cross-project by design, while this platform must remain Bloodlines-specialized
- its main server is a large single-file Python surface, which raises coupling risk for first-pass Bloodlines-specific work

What is reused anyway:

- local-only launch posture
- Python stdlib server pattern
- dashboard-oriented operating model

## High-Level Architecture

1. `server.py`
   Routes API and serves the GUI.
2. `lib/core.py`
   Handles scanning, indexing, retrieval, routing, last-good-state resolution, telemetry, and optional model calls.
3. `config/`
   Declares subset scope, doctrine, scan policy, routing policy, and hashed tier gates.
4. `state/`
   Stores generated registry, journal, source map, telemetry, and sqlite state.
5. `static/`
   Holds the browser UI.

## Data Flow

1. Startup initializes sqlite schema and runtime session state.
2. Startup scan loads the canonical subset and writes:
   - source map
   - canonical source registry
   - model inventory
   - resume candidates
   - execution packet
   - telemetry snapshot
3. GUI loads dashboard status from `/api/bootstrap`.
4. Command Deck loads persistent conversation state from `/api/agent-console`.
5. Command Deck posts either slash commands or natural-language turns into `/api/agent-console/message`.
6. Natural-language turns build a context pack from:
   - latest continuity files
   - high-authority registry items
   - retrieved supporting chunks
7. The local model returns either a final answer, a governed write draft, or one intermediate tool request.
8. The server executes at most one local tool step per loop turn until the conversation reaches a grounded response.
9. Execution view reads the generated Unity execution packet and current write posture.
10. If the operator unlocks the session, the write workbench or staged conversation drafts can apply real canonical file updates through the same platform.

## Write Posture

Platform-state writes are allowed inside `continuation-platform/state/` so the system can maintain continuity.

Bloodlines project-artifact writes are locked by default. Unlock is session-memory-only and does not survive restart.

Once unlocked, project-artifact writes are no longer preview-only:

- reads are allowed for text files inside the canonical Bloodlines root
- previews show required tier, current and proposed hashes, and unified diff output
- applies write the real project file, not a mirror, and create automatic backups under `state/backups/`
- stale-source protection prevents blind overwrite when the target changed after preview
- Command Deck draft applies use the same tier gate, stale-source protection, and backup flow as the explicit workbench

## Failure Strategy

The platform fails into browseable degraded modes:

- no Ollama: continuity browsing still works
- sqlite or FTS trouble: registry falls back to file-level browsing
- write gate locked: write endpoints refuse with `tier_insufficient`
