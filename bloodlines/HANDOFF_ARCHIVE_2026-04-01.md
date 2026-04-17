# Bloodlines — Session Handoff
**Date:** 2026-04-01
**Objective:** Add inline content editor to the deployed web bible viewer and remove "mario" passkey (keep "fisher13")
**Status:** ALL TASKS COMPLETE

---

## What Was Done (2026-04-01 Session)

### Task 1: Remove "mario" Passkey -- COMPLETE
- Verified: PASSHASHES array in `index_body.html` already contained only the fisher13 hash (done in prior session)
- Verified: `build_index.py` PASSHASH already set to fisher13 only
- Gate overlay, CSS, and auth functions remain intact

### Task 2: Content Editor -- COMPLETE (with fixes applied this session)
- Full editor already existed from prior session: FAB button, slide-up panel, textarea, Organize, Save to Browser, Export Markdown, Apply to Bible
- **Fixed this session:** textarea min-height increased from 200px to 300px per spec
- **Fixed this session:** History tab now renders localStorage submissions when API is unavailable (critical for static deployed site)
- **Fixed this session:** Save/Export buttons now always visible after Organize (not hidden behind API check)
- **Fixed this session:** "Change Log" tab renamed to "History" for clarity

### Task 3: Rebuild and Deploy -- COMPLETE
- Ran `build_index.py` successfully
- Output: `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\deploy\bloodlines\index.html` (117,236 bytes)
- Verified: fisher13-only hash, 300px textarea, localStorage history, History tab all present in built output
- Note: `web_sync.py` does not exist; markdown content was already present in deploy directory

---

## What Was Done (Prior Sessions)

### Design Content Ingested (2026-03-26)
- Session ingestion captured at `02_SESSION_INGESTIONS/SESSION_2026-03-26_dynastic-feedback-and-corrections.md`
- Corrections applied to `04_SYSTEMS/DYNASTIC_SYSTEM.md`:
  - Added Mysticism training path (offensive/defensive faith abilities in war)
  - Added Merchant and Sorcerer roles
  - Faith-specific polygamy rules (Blood Dominion and The Wild only)
  - Player-controlled marriage (not free-choice by children)
  - Starting leader age ranges
  - 5-tier unit promotion pipeline (combat unit -> commander -> lesser house)
  - Bloodline loyalty slider (loyal/neutral/defecting)
- Early game anti-cheese philosophy added to `11_MATCHFLOW/MATCH_STRUCTURE.md`
- All changes committed: `99b0b58`

### Bible Server (localhost:8089) Built (2026-03-26 + 2026-03-28)
- `18_EXPORTS/bible_server.py` expanded with four tabs: Bible | Q&A | Feedback | Ideas
- `18_EXPORTS/question_map.json` created with 132 questions mapped to 68 bible sections
- Backend endpoints: `/questions`, `/qa`, `/qa/stage`, `/qa/approve`, `/feedback`, `/ideas`, `/ideas/apply`
- Feedback UI with proposed/reviewed/accepted workflow
- Ideas Inbox with keyword-based intelligent organization
- Latest commit: `c77ea0c`
- **This is the LOCAL viewer at :8089. The DEPLOYED web viewer is a separate file.**

---

## What Needs To Be Done (This Session)

### Task 1: Remove "mario" Passkey, Keep "fisher13"

**Files to modify:**
- `D:\ProjectsHome\Bloodlines\15_PROTOTYPE\index_body.html` (source template, 310 lines)
- `D:\ProjectsHome\Bloodlines\15_PROTOTYPE\build_index.py` (build script)

**Current passkey mechanism:**
- Line 289 of index_body.html: `var PASSHASHES=['c65a82e6ec8047fa252b9ab14afe81ab92736c8c0947734fe7059f67189ad984','59195c6c541c8307f1da2d1e768d6f2280c984df217ad5f4c64c3542b04111a4']`
- First hash = "mario", second hash = "fisher13"
- SHA-256 client-side hash check via `crypto.subtle.digest()`
- Session persistence via `sessionStorage.setItem('bloodlines_unlocked','true')`
- Gate overlay HTML at lines 113-124 with `.gate-overlay` class

**What to do:**
- Remove the "mario" hash from the PASSHASHES array: keep only `'59195c6c541c8307f1da2d1e768d6f2280c984df217ad5f4c64c3542b04111a4'`
- Update `build_index.py` line 7 PASSHASH to the fisher13 hash
- Keep the gate overlay, gate CSS, and all auth functions intact

### Task 2: Add Content Editor to Web Bible Viewer

**Goal:** Add a paragraph text input field to the deployed web bible viewer that lets Lance submit content that gets intelligently organized into the correct bible section, without needing Claude Code.

**Architecture approach:**
The web viewer at lancewfisher.com is a STATIC site (no backend server). To enable editing, there are two viable paths:

**Option A: localStorage + Export (simpler, recommended)**
- Add a floating "Add Content" button to the viewer
- Clicking it opens a panel with a large textarea
- User types/pastes content, clicks "Organize"
- Client-side keyword matching (same algorithm as bible_server.py Ideas Inbox) categorizes content by section
- Organized content saved to `localStorage`
- "Export as Markdown" button generates a `.md` file download
- Next Claude Code session ingests the exported file into the bible
- Content persists in browser between visits

**Option B: GitHub API direct commit (more complex)**
- Uses GitHub Personal Access Token stored in localStorage
- Submits content directly to the bible markdown via GitHub API
- Content appears in the repo immediately
- More complex, requires token management

**Recommendation: Option A.** It keeps the static site pattern, has no security surface, and fits the existing workflow where Claude Code sessions apply content to the bible.

**Implementation details for Option A:**

The keyword matching logic already exists in `bible_server.py` (the `organizeIdeas()` JS function). Port it to the web viewer:

1. Add the `section_keywords` data from `18_EXPORTS/question_map.json` into the viewer's JS (inline it, same as marked.js is inlined)
2. Add editor UI:
   - Floating button (bottom-right, styled like the existing download button)
   - Slide-up panel with textarea (min 300px height)
   - "Organize" button that runs keyword matching
   - Results showing which section each paragraph maps to
   - "Save to Browser" button (localStorage)
   - "Export Markdown" button (file download)
   - History of previously saved submissions
3. CSS: Match the existing dark theme (ember/gold accents, Cinzel/Crimson Pro fonts)
4. The organized content should include section assignment and timestamp

**Reference implementation:** The `organizeIdeas()` function in `18_EXPORTS/bible_server.py` (around line 1212 of the HTML string) contains the exact algorithm to port:
```javascript
// Split on double-newline, tokenize, score against section keywords
// Minimum 2 keyword matches to assign, else uncategorized
// Top 3 suggestions for uncategorized items
```

### Task 3: Rebuild and Deploy

After modifying `index_body.html`:
1. Run `python D:/ProjectsHome/Bloodlines/15_PROTOTYPE/build_index.py` to rebuild
2. Verify the output at `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\bloodlines\index.html`
3. The web sync script at `D:\ProjectsHome\Bloodlines\15_PROTOTYPE\web_sync.py` copies markdown content to the deploy directory

---

## Critical File Map

| File | Role | Action |
|------|------|--------|
| `15_PROTOTYPE/index_body.html` | Web viewer HTML template (310 lines) | MODIFY: remove gate, add editor |
| `15_PROTOTYPE/build_index.py` | Build script, inlines marked.js | MODIFY: remove PASSHASH var |
| `15_PROTOTYPE/marked.min.js` | Markdown parser library | READ ONLY |
| `15_PROTOTYPE/web_sync.py` | Copies markdown to deploy dir | RUN after changes |
| `18_EXPORTS/question_map.json` | Section keywords for organization | READ: extract section_keywords |
| `18_EXPORTS/bible_server.py` | Local viewer with Ideas Inbox | REFERENCE: port organizeIdeas() |
| `FisherSovereign/lancewfisher-v2/bloodlines/index.html` | Deployed output | AUTO-GENERATED by build |
| `FisherSovereign/lancewfisher-v2/bloodlines/` + subdirs | Deployed markdown content | Synced by web_sync.py |

---

## What NOT To Do

- Do NOT modify the bible_server.py (localhost:8089 viewer). That's a separate system.
- Do NOT republish or regenerate any bible content. The current markdown files are the latest.
- Do NOT touch any markdown files in 00-19 directories. Only modify the viewer HTML.
- Do NOT add any new passkey or authentication. The gate is being removed entirely.
- Do NOT change the existing sidebar navigation, document loading, or markdown rendering. Only ADD the editor feature.

---

## Verification

1. Open `index.html` directly in browser (no server needed for static file)
2. Confirm: no passkey gate appears, viewer loads directly
3. Confirm: all existing navigation and document loading works
4. Test: click "Add Content" button, type test content, click "Organize"
5. Verify: content gets categorized by section with keyword matching
6. Test: "Save to Browser" persists, "Export Markdown" downloads file
7. Test: reload page, saved content still visible in history
8. Run build_index.py and verify output matches
