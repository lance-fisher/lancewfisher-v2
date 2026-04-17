# Bloodlines: Design Input Inbox

**Purpose:** A raw input file where the project owner can paste thoughts, answers, ideas, and design decisions for future processing. This file is intentionally unstructured. Its purpose is to capture input quickly without requiring the author to navigate the full project structure.

**How This File Works:**

1. Paste any design thoughts, answers to workbook questions, new ideas, or corrections here
2. Label each entry with a date and brief topic if possible (not required)
3. In a future Claude Code session, provide this instruction:
   > "Review `docs/INPUT_TO_APPLY.md` for new entries. Apply them to the appropriate project files."
4. The AI session will read the entries, identify where each piece of information belongs, and append it to the correct files with proper date stamps and cross-references
5. Applied entries are marked as `[APPLIED]` with the date and destination file, but never deleted

**Rules for AI Sessions Processing This File:**

1. Read all entries below the `## Inbox` heading
2. For each entry, determine which project files should be updated
3. Append the new content to the appropriate files with date stamps
4. If an entry settles an open question, update `01_CANON/CANONICAL_RULES.md`
5. If an entry contradicts prior content, preserve both with clear labeling
6. If an entry is ambiguous, flag it for clarification rather than guessing
7. After processing, mark each entry as `[APPLIED - date - destination files]`
8. Update `docs/CHANGE_LOG.md` with what was processed
9. Never delete entries from this file, even after processing

---

## Inbox

_(Paste new design input below this line. Each entry should be separated by a horizontal rule.)_

---
