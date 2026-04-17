# Bloodlines — Project Startup Prompt

**Use this prompt at the beginning of every AI-assisted design session on the Bloodlines project.**

---

## Startup Instructions

When beginning work on the Bloodlines project, perform the following actions in order:

### 1. Load Master Memory
Read `01_CANON/BLOODLINES_MASTER_MEMORY.md` in full. This is the cumulative record of all design work done on the project. It contains every decision, exploration, and session summary in chronological order. Understanding this document is required before making any design contributions.

### 2. Load Design Bible
Read `01_CANON/BLOODLINES_DESIGN_BIBLE.md` in full. This is the structured game design document organized by domain. It provides the current state of each design area.

### 3. Review Canonical Rules
Read `01_CANON/CANONICAL_RULES.md`. This document tracks which design elements are settled canon, which are open for exploration, and which have been proposed but not confirmed. Do not contradict SETTLED elements without explicit authorization.

### 4. Understand Project Structure
Review `00_ADMIN/DIRECTORY_MAP.md` to understand where different types of content belong. Each folder has a specific purpose and content placed in the wrong location creates confusion.

### 5. Confirm Design Continuity
Before making any changes, confirm that you understand:
- The current state of the design (from Master Memory)
- What is settled versus open (from Canonical Rules)
- What the current development focus is (from `00_ADMIN/PROJECT_STATUS.md`)

### 6. Preserve Additive History
All work in this session must follow the additive archival rules:
- Append, never replace
- No deletion without authorization
- No summarization or shortening
- Contradictions are preserved, not erased
- Date stamp all new content

### 7. Append New Ideas
New design ideas are added to the appropriate documents. They do not overwrite prior content. If a new idea conflicts with an existing one, both remain in the archive with context explaining the evolution.

---

## Session End Protocol

At the end of the session:
1. Create a session ingestion file in `02_SESSION_INGESTIONS/` named `SESSION_YYYY-MM-DD_description.md`
2. Append a summary entry to `01_CANON/BLOODLINES_MASTER_MEMORY.md`
3. Update `01_CANON/CANONICAL_RULES.md` if any elements changed status
4. Update `00_ADMIN/PROJECT_STATUS.md` with the session's focus and outcomes
