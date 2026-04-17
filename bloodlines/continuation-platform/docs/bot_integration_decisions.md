# Bot Integration Decisions

## Clawdbot

- Decision: `retire`
- Scope: Bloodlines continuation-platform role only
- Rationale: legacy gateway lineage, WSL-oriented install posture, no unique Bloodlines advantage over the newer local gateways, and no current runtime integration in the active workspace health surface

## Moltbot

- Decision: `retire`
- Scope: Bloodlines continuation-platform role only
- Rationale: currently offline, depends on WSL-friendly deployment patterns, and duplicates the same cross-channel assistant role without adding Bloodlines-specific value

## OpenClaw

- Decision: `keep_as_is_and_call_from_platform`
- Rationale: currently online, already routed to local Qwen models, exposes a local gateway surface, maintains its own sessions and workspace, and can serve as an optional external local-agent bridge without coupling Bloodlines continuity to its internal implementation

## Notes

- None of these decisions uninstall or delete existing bot infrastructure.
- These decisions only define the Bloodlines continuation-platform integration posture.
