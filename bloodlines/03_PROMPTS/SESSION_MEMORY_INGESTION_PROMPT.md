# Bloodlines — Session Memory Ingestion Prompt

**Use this prompt to process raw session output into structured, persistent memory.**

---

## Instructions

You are processing a design session transcript for the Bloodlines project. Your job is to extract all design-relevant content and append it to the project's persistent memory. Follow these steps:

### Step 1: Read the Session Transcript
Read the raw session data provided. Identify all design decisions, mechanical explorations, lore contributions, open questions, and creative ideas discussed.

### Step 2: Create a Session Ingestion File
Create a new file in `02_SESSION_INGESTIONS/` with the naming format:
```
SESSION_YYYY-MM-DD_brief-description.md
```

The ingestion file must contain:
- **Date and session context** (what was the focus of the session)
- **Key decisions made** (anything that should be tracked in Canonical Rules)
- **Design explorations** (ideas discussed, even if not finalized)
- **Mechanical proposals** (new system ideas or modifications to existing systems)
- **Lore contributions** (any world-building, history, or narrative content)
- **Open questions** (unresolved design questions surfaced during the session)
- **Contradictions noted** (any conflicts with existing canon, preserved with context)

### Step 3: Append to Master Memory
Add a dated entry to `01_CANON/BLOODLINES_MASTER_MEMORY.md` summarizing:
- What was discussed
- What was decided
- What remains open
- Which files were created or modified

**Do not summarize or shorten the ingestion file content for the Master Memory entry.** The Master Memory entry should capture the full substance of the session, not a condensed version.

### Step 4: Update Canonical Rules
If any design elements changed status (OPEN to PROPOSED, PROPOSED to SETTLED, etc.), update the relevant table in `01_CANON/CANONICAL_RULES.md`.

### Step 5: Update System Files
If specific game systems were discussed in detail, append the relevant content to the appropriate file in `04_SYSTEMS/`.

### Step 6: Update Project Status
Add a dated entry to `00_ADMIN/PROJECT_STATUS.md` reflecting the session's work.

---

## Critical Rules
- Never delete or modify existing content in any file
- Always append with date stamps
- Preserve the full text of design discussions, not summaries
- If something contradicts existing canon, note the contradiction explicitly and preserve both versions
