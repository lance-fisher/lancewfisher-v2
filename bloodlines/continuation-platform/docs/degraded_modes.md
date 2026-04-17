# Degraded Modes

## Ollama Unreachable

- effect: agent execution disabled
- retrieval: stays available from sqlite state
- GUI: dashboard banner plus recovery hint
- code: `ollama_unreachable`

## SQLite Or FTS Failure

- effect: retrieval falls back to file-level registry browsing
- writes: project-artifact writes stay blocked
- GUI: dashboard banner plus rebuild hint
- code: `sqlite_unavailable`

## Scan Interrupted

- effect: next scan resumes from stored checkpoint
- GUI: checkpoint boundary shown
- code: `scan_interrupted`

## Write Gate Locked

- effect: write endpoints refuse
- telemetry: increment refusal count
- GUI: visible lock state
- code: `tier_insufficient`

## Optional Bridge Unreachable

- effect: OpenClaw integration disabled, core platform still works
- code: `openclaw_unreachable`
