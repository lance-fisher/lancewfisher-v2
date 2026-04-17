# Bloodlines — Workflow Rules

## Additive Design Memory Rules

These rules govern how the Bloodlines design archive operates. They are non-negotiable and must be followed by all contributors, human and AI, in every session.

### Rule 1: No Deletion Without Authorization
Nothing in the design archive may be deleted without explicit authorization from the project owner. This includes files, sections within files, individual lines, and even single words. If content needs to be superseded, it is moved to `19_ARCHIVE/`, never erased.

### Rule 2: No Summarization or Shortening
Nothing in the design memory may be summarized, condensed, shortened, or paraphrased to save space. The full text of every design decision, brainstorm, and exploration must remain intact exactly as it was written. If a document becomes long, that is by design.

### Rule 3: Append, Never Replace
New session information must always be appended to existing documents, never used to replace prior content. If a new idea contradicts an old one, both remain. The new idea is added with a date stamp and context. The old idea stays where it is.

### Rule 4: Preserve Contradictions
If conflicting design ideas appear over time, both must remain preserved in the archive. Contradictions are valuable design artifacts. They represent the evolution of thinking and may be revisited. Resolution of contradictions is noted alongside them, not used to erase one side.

### Rule 5: Continuous Growth
The project memory must grow continuously as development progresses. Every design session, every brainstorm, every mechanical exploration adds to the archive. The archive is a living document that becomes more valuable over time precisely because it retains everything.

### Rule 6: Historical Integrity
Historical design context must always remain intact. The state of the design at any point in time should be reconstructable from the archive. This means preserving not just decisions but the reasoning, alternatives considered, and context in which decisions were made.

## Session Workflow

### Starting a Session
1. Load `01_CANON/BLOODLINES_MASTER_MEMORY.md` to understand cumulative design state
2. Load `01_CANON/BLOODLINES_DESIGN_BIBLE.md` for the structured design reference
3. Review `01_CANON/CANONICAL_RULES.md` for settled design decisions
4. Check `00_ADMIN/PROJECT_STATUS.md` for current development focus
5. Confirm understanding of the archival rules before making any changes

### During a Session
- All new design content is appended with date stamps
- Cross-reference system files when modifying mechanics
- Note which files were modified at the end of the session
- If uncertain about a change, append it as a proposal rather than modifying canon

### Ending a Session
1. Create a session ingestion file in `02_SESSION_INGESTIONS/`
2. Append key decisions and explorations to `01_CANON/BLOODLINES_MASTER_MEMORY.md`
3. Update `00_ADMIN/PROJECT_STATUS.md` with session summary
4. Note any new open questions or contradictions discovered

## File Naming Conventions

- All folder names use `SCREAMING_SNAKE_CASE` with numeric prefixes
- All canonical document names use `SCREAMING_SNAKE_CASE.md`
- Session ingestion files use the format: `SESSION_YYYY-MM-DD_description.md`
- Asset files use descriptive names with lowercase and hyphens

## Modification Authority

- **Canon documents** (01_CANON): Append only. Existing content is immutable without explicit authorization.
- **System documents** (04_SYSTEMS): Append new mechanics freely. Modifying existing mechanics requires noting the change alongside the original.
- **Session ingestions** (02_SESSION_INGESTIONS): Write once, never modify after creation.
- **Admin documents** (00_ADMIN): May be updated to reflect current state, but update logs must preserve history.
- **All other folders**: Standard additive rules apply.
