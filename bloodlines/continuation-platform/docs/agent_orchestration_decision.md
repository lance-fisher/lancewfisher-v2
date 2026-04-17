# Agent Orchestration Decision

## Decision

Use a custom thin orchestrator over:

- direct filesystem scan
- SQLite continuity state
- direct Ollama REST calls
- optional OpenClaw bridge

## Why

- zero framework bloat
- easy to inspect and debug
- simple offline deployment
- no extra dependency stack required for the first slice
- matches the local-first requirement better than adding a full orchestration framework before the platform exists

## Evaluated Options

### LangGraph

- strong abstraction
- too much framework weight for the first slice
- unnecessary while only one mode is operational

### CrewAI

- agent-role abstractions are not the current bottleneck
- adds dependency and behavior overhead before continuity fundamentals are stable

### Autogen

- useful for multi-agent experiments
- poor fit for a first pass centered on continuity, not chat-to-chat delegation

### Custom Thin Orchestrator

- best fit for maintainability, observability, and scope control

## Re-Open Rule

Do not revisit this decision unless:

- the platform needs more than two truly distinct execution modes, or
- multi-step autonomous batch execution becomes the limiting factor
