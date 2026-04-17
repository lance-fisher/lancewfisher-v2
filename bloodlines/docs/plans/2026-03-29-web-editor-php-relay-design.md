# Bloodlines Web Editor: PHP Relay Design

**Date:** 2026-03-29
**Status:** Approved

## Summary

Add server-side content submission to the Bloodlines web bible viewer at lancewfisher.com. Users write ideas in the existing editor panel, organize them by bible section via keyword matching, then click "Apply to Bible" to push content directly to the deployed markdown files via a PHP endpoint on Hostinger. A changelog enables reverting any change.

## Architecture

```
[Browser: Bible Viewer]  --POST JSON-->  [Hostinger: api/submit.php]  --appends-->  [Markdown files]
                                                                      --appends-->  [_ideas_inbox.md]
                                                                      --writes-->   [_changelog.json]
```

## PHP Backend: `api/submit.php`

Single PHP file, three actions via `action` parameter:

### `apply`
- Validates fisher13 passkey hash (SHA-256, sent with every request)
- For each idea: appends a marked block to the target markdown file
- Adds entry to `_ideas_inbox.md` organized by section heading
- Writes entry to `_changelog.json` with unique ID, timestamp, section, target file, content
- Appended block format:

```markdown

---

> **Web Editor** | 2026-03-29 14:32 | Cavalry System

Idea text here...
```

### `changelog`
- Returns full `_changelog.json` as JSON response

### `revert`
- Given a changelog entry ID:
  - Reads the target markdown file
  - Finds and removes the exact appended block (matched by unique ID marker)
  - Marks entry as reverted in `_changelog.json`
  - Removes entry from `_ideas_inbox.md`

### Security
- Every request must include the fisher13 SHA-256 hash
- PHP validates before any write operation

## Section-to-File Mapping

| Sections | Target File |
|---|---|
| 1-2, 4, 69 | `01_CANON/BLOODLINES_MASTER_MEMORY.md` |
| 3 | `05_LORE/TIMELINE.md` |
| 5-7, 19, 53-61 | `06_FACTIONS/FOUNDING_HOUSES.md` |
| 8-9, 16-18, 20 | `09_WORLD/WORLD_INDEX.md` |
| 10-15 | `04_SYSTEMS/DYNASTIC_SYSTEM.md` |
| 21-27, 68 | `07_FAITHS/FOUR_ANCIENT_FAITHS.md` |
| 28-31 | `04_SYSTEMS/CONVICTION_SYSTEM.md` |
| 32-34, 36-38 | `10_UNITS/UNIT_INDEX.md` |
| 35 | `04_SYSTEMS/BORN_OF_SACRIFICE_SYSTEM.md` |
| 39, 45-46, 48-52 | `11_MATCHFLOW/MATCH_STRUCTURE.md` |
| 40-44 | `04_SYSTEMS/RESOURCE_SYSTEM.md` |
| 47 | `11_MATCHFLOW/POLITICAL_EVENTS.md` |
| 63-67 | `08_MECHANICS/OPERATIONS_SYSTEM.md` |
| uncategorized | `_ideas_inbox.md` only (no file append) |

## Viewer JS Changes

- **Write tab**: Organize button stays. After organizing, "Apply to Bible" button POSTs to PHP endpoint. Replaces Save to Browser / Export Markdown when endpoint is reachable.
- **Change Log tab**: Replaces History tab. Fetches changelog from PHP, displays entries with Revert buttons.
- **Results tab**: Removed (redundant).
- **Offline fallback**: If PHP endpoint unreachable, falls back to localStorage + Export Markdown behavior.
- **Two tabs total**: Write, Change Log.

## File Inventory

| File | Location | New/Modified |
|---|---|---|
| `api/submit.php` | Hostinger `bloodlines/api/` | NEW |
| `api/.htaccess` | Hostinger `bloodlines/api/` | NEW (CORS headers) |
| `_ideas_inbox.md` | Hostinger `bloodlines/` | NEW (created by PHP) |
| `_changelog.json` | Hostinger `bloodlines/` | NEW (created by PHP) |
| `15_PROTOTYPE/index_body.html` | Local source template | MODIFIED |
| `15_PROTOTYPE/build_index.py` | Local build script | UNCHANGED |

## Revert Mechanism

Each appended block includes a hidden HTML comment with the changelog entry ID:
```markdown
<!-- idea:a1b2c3d4 -->
```
This allows the PHP revert action to find and remove the exact block reliably, even if surrounding content has changed.
