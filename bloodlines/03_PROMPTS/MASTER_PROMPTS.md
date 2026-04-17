# Bloodlines — Master Prompts

A repository of important prompts used in the Bloodlines design process. Each prompt serves a specific purpose in the AI-assisted design workflow.

---

## Project Startup Prompt
**File:** `03_PROMPTS/PROJECT_STARTUP_PROMPT.md`
**Purpose:** Loaded at the beginning of every design session. Ensures the AI has full context, understands the archival rules, and maintains design continuity across sessions.

---

## Session Memory Ingestion Prompt
**File:** `03_PROMPTS/SESSION_MEMORY_INGESTION_PROMPT.md`
**Purpose:** Used to process raw session transcripts into structured memory entries. Extracts decisions, explorations, and open questions and appends them to the canonical memory files.

---

## Design Expansion Prompt
**File:** *(placeholder — to be created)*
**Purpose:** Used when expanding a specific design area in depth. Provides context on the target system, existing canon, and constraints, then guides a focused exploration of mechanical, narrative, or structural possibilities.

**Template:**
```
You are expanding the design of [SYSTEM/AREA] for the Bloodlines project.

Current canon for this area:
[paste relevant canon]

Constraints:
[paste any settled decisions that bound this exploration]

Open questions:
[paste relevant open questions]

Explore [specific aspect] in depth. Propose concrete mechanics, narrative hooks, or structural designs. Present trade-offs explicitly. Do not contradict settled canon.
```

---

## Canon Validation Prompt
**File:** *(placeholder — to be created)*
**Purpose:** Used to audit the current state of the design for internal consistency. Checks for contradictions, gaps, and unresolved dependencies across all canon documents.

**Template:**
```
You are performing a canon validation audit for the Bloodlines project.

Read the following documents in order:
1. 01_CANON/CANONICAL_RULES.md
2. 01_CANON/BLOODLINES_DESIGN_BIBLE.md
3. 01_CANON/BLOODLINES_MASTER_MEMORY.md

For each SETTLED element in Canonical Rules, verify that:
- It is reflected accurately in the Design Bible
- No subsequent Master Memory entry contradicts it without noting the contradiction
- All cross-references to other systems are consistent

Report:
- Confirmed consistent elements
- Contradictions found (with locations)
- Gaps (elements referenced but never defined)
- Recommendations for resolution
```

---

*New prompts are added to this index as they are created. Each entry includes the file location, purpose, and template or usage instructions.*
