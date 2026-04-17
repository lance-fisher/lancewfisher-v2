# Ecosystem Audit Report

## Scope

Audit date: 2026-04-16

Audited from:

- `D:\ProjectsHome`
- `D:\ProjectsHome\Bloodlines`
- running local services on `localhost`

## Governance Findings

- Active workspace governance chain is present and readable through:
  - `D:\ProjectsHome\AGENTS.md`
  - `D:\ProjectsHome\RESUME.md`
  - `D:\ProjectsHome\CURRENT_STATE.json`
  - `D:\ProjectsHome\CLAUDE.md`
  - `D:\ProjectsHome\.governance\the-boss\*`
  - `D:\ProjectsHome\Bloodlines\AGENTS.md`
  - `D:\ProjectsHome\Bloodlines\CLAUDE.md`
- `UNIVERSAL_AI_COLLABORATION_MASTER_COMPLETE.md` was not found under `D:\ProjectsHome` or `C:\Users\lance` during discovery.
- For this slice, the active governing chain is therefore root workspace governance, Boss governance, and Bloodlines-local governance.

## Bloodlines Findings

- Canonical session root: `D:\ProjectsHome\Bloodlines`
- Physical backing path: `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\bloodlines`
- Preserved compatibility copy remains in the archive surfaces
- Bloodlines already contains strong continuity assets, including:
  - `CURRENT_PROJECT_STATE.md`
  - `NEXT_SESSION_HANDOFF.md`
  - `SOURCE_PROVENANCE_MAP.md`
  - `continuity/PROJECT_STATE.json`
  - `tasks/todo.md`

## Local Service Findings

- `project-hub` is online at `http://localhost:8090`
- `LocalClaude` is online at `http://localhost:3500`
- `Session Atlas` is online at `http://localhost:8092`
- `Auton` is online at `http://localhost:8095`
- `OpenClaw` is online at `http://127.0.0.1:18800`
- `Moltbot` is offline
- `WSL2 Ubuntu` is stopped

## Foundation Candidates

### Project Hub

Strengths:

- always-on dashboard model
- project cards and system-health views already exist
- Windows Terminal integration already exists

Weaknesses for this slice:

- protected global infrastructure path
- cross-project coupling risk
- single-file server makes Bloodlines-specific integration high-risk for a first pass

Decision:

- evaluate only, do not extend in slice 1

### LocalClaude

Strengths:

- local model provider abstractions already exist
- capability routing patterns already exist
- Fastify plus React architecture is proven locally

Weaknesses for this slice:

- separate application root and stack
- not Bloodlines-native
- would require more code movement or coupling than a contained slice

Decision:

- reuse patterns, do not couple directly in slice 1

## Bot Findings

- OpenClaw is the only currently online local assistant gateway with local Qwen routing
- Moltbot is offline and depends on WSL-centric deployment patterns
- Clawdbot is legacy relative to the newer gateways

See `bot_integration_decisions.md` for final decisions.

## Runtime Boundary

- Primary runtime chosen: Windows-native
- WSL status on audit: stopped
- Launch expectation for daily use: double-clickable Windows command path
