# VERIFY_OFFLINE_WORK: Protocol For Returning Claude or Codex Sessions

## Purpose

The offline continuation platform applies small governed edits during Claude or Codex token droughts using local 7B and 14B models. Those edits are safe-by-construction (tier gate + backups + additive-only prompts), but they are NOT at frontier-model quality. Every applied change must be verified by a frontier-class agent (Claude or Codex) before being treated as canonical.

## Trigger

Execute this protocol when any of these is true:

1. `continuation-platform/state/offline_work_ledger.md` exists with entries whose "Verification status" is `pending`.
2. `git status` shows uncommitted changes in files that also appear in the ledger.
3. Lance says "verify the offline work" or similar.

## Protocol

### Step 1 — Read the ledger
`continuation-platform/state/offline_work_ledger.md` lists every applied draft with: timestamp, relative path, draft id, reason, source basis, backup path, post-write sha256, verification status.

### Step 2 — Cross-reference the journal
For each pending entry, find the matching `auto_continue` and `write_applied` events in `continuation-platform/state/operations_journal.jsonl`. These carry the full tool trace, model routing, prompt/output tokens, and any degraded-mode reasons.

### Step 3 — Diff each applied file against its backup
Backups sit under `continuation-platform/state/backups/`. For each ledger entry:
- Read the backup file
- Read the current live file
- Produce a unified diff
- Evaluate against these criteria:
  - Is the change scoped to exactly what the "Reason" says?
  - Does it preserve existing content (preservation-first mandate)?
  - Does it violate invariants (boot-order changes, multi-file side effects, new systems, doctrine contradictions)?
  - Does it align with `governance/OWNER_DIRECTION_2026-04-16_FULL_CANON_UNITY.md`?
  - For code: does it compile and follow existing style?
  - For canon: is it additive and cited?

### Step 4 — Mark each entry
Edit the ledger entry's "Verification status" line:
- `VERIFIED <YYYY-MM-DD> <agent>: <rationale>` — correct, stays
- `REVERTED <YYYY-MM-DD> <agent>: <rationale>` — wrong, reverted (restore from backup)
- `AMENDED <YYYY-MM-DD> <agent>: <rationale>` — partially correct, reshaped

### Step 5 — Summarize to Lance
- Count processed entries
- Count VERIFIED / REVERTED / AMENDED
- Any pattern worth flagging
- Anything needing Lance's decision

### Step 6 — Append closing block
```
---

## Verification pass closed <timestamp>
- Agent: <model id>
- Entries processed: <n>
- VERIFIED: <n>  REVERTED: <n>  AMENDED: <n>
```

The ledger is append-only. Do not delete prior entries.

## Non-Negotiables

- Do NOT treat any offline-applied change as canonical until verified.
- Do NOT bulk-verify without reading each diff.
- If any applied change touches `BloodlinesBootstrap.cs`, boot-order, `governance/`, or spans multiple files: default to REVERTED unless analysis clearly shows safety.
- Read diffs before marking. Signature implies responsibility.
