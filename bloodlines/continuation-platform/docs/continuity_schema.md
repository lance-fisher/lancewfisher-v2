# Continuity Schema

The thin slice stores continuity state in sqlite plus generated JSON artifacts.

## SQLite Tables

- `scans`: scan lifecycle records, timestamps, counts, status.
- `documents`: canonical subset registry entries with hashes, authority, and provenance.
- `chunks`: heading-aware text chunks derived from canonical docs.
- `chunks_fts`: FTS5 search index over chunk text when available.
- `artifacts`: detected frontier-session artifacts from Claude and Codex parity scanning.
- `journal`: append-only structured platform actions.
- `resume_candidates`: resolved last-good-state candidates.
- `write_events`: write posture attempts, approvals, refusals, and reason codes.

## Generated JSON and Text Assets

- `state/source_map.json`: full-tree source map, recent changes, frontier artifacts, duplicate-name conflicts.
- `state/canonical_source_registry.json`: authority-scored canonical subset registry.
- `state/model_inventory.json`: live Ollama inventory with routing roles.
- `state/resume_state.json`: last-good-state candidates and resolved anchor.
- `state/execution_packet.json`: current Unity shipping-lane execution packet built from the live handoff and continuity stack.
- `state/telemetry.json`: surfaced metrics and degraded-mode counts.
- `state/operations_journal.jsonl`: append-only durable event stream.
- `state/handoff_pack_preview.md`: frontier re-entry briefing preview.
- `state/scan_checkpoints.json`: per-file checkpoint map for incremental rescans.

## Resume Candidate Rules

Candidate order follows the Bloodlines continuation directive:

1. latest successful local agent action that passed doctrine check
2. latest ingested frontier artifact that passed doctrine check
3. latest ingested manual edit in the canonical subset

If the two most recent valid candidates come from different sources within a
15-minute window, the state is marked ambiguous and the UI surfaces the
competing anchors instead of auto-resolving.
