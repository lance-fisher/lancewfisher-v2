# Bloodlines Bible Server — Q&A Subsection + Ideas Inbox

## Context

The Bloodlines design bible viewer (`18_EXPORTS/bible_server.py`, port 8089) is currently a read-only document viewer. Two interactive features are needed:

1. **Q&A Subsection** — Bring back the question-driven design workflow as an integrated part of the bible. Questions from `INPUT_WORKBOOK.md` (30+ questions) and `OPEN_QUESTIONS.md` (87+ questions) should appear contextually within the bible, users answer directly in the UI, and answers get applied to the bible content.

2. **Ideas Inbox** — A large text input where free-form game ideas can be pasted, and the system intelligently categorizes them by bible section using keyword matching.

Both features integrate into the existing bible server (single `bible_server.py` file, inline HTML). No external dependencies.

---

## Architecture Decision

**Extend the bible server** rather than building a separate app. The bible_server.py already has the right aesthetic, the markdown content, and the section navigation. Adding tabs to the header ("Bible" | "Q&A" | "Ideas") keeps everything in one place.

**Three-view UI**: The existing bible content becomes the "Bible" view. Q&A becomes a second view showing questions grouped by bible section with answer textareas. Ideas becomes a third view with the input block and organization results.

**Why tabs over inline drawers**: Inline Q&A drawers per-section would bloat the bible view and mix reading with input. Separate tab views keep the bible clean for reading while giving Q&A and Ideas their own focused space. Questions still link back to their relevant bible section.

---

## Data Files (New)

```
18_EXPORTS/
  question_map.json    — Static mapping of all questions -> bible sections + keywords
  qa_responses.json    — Persisted answers (created on first save)
  ideas_inbox.json     — Persisted ideas (created on first submission)
```

### question_map.json structure
```json
{
  "section_keywords": {
    "section-1": {
      "title": "What Bloodlines Is",
      "keywords": ["identity", "genre", "scope", "competitive", "narrative"]
    }
  },
  "questions": [
    {
      "id": "Q1.1",
      "text": "Competitive vs. Narrative Focus",
      "description": "The game supports both multiplayer competition and long-form narrative play...",
      "options": ["(a) Primarily competitive RTS with narrative flavor", "(b) Primarily narrative..."],
      "severity": null,
      "source": "workbook",
      "bible_sections": ["section-1"],
      "application_note": "Updates Design Bible Section 1"
    },
    {
      "id": "W1",
      "text": "What map type?",
      "severity": "BLOCKING",
      "source": "open_questions",
      "bible_sections": ["section-17"],
      "workbook_ref": "Q10.1"
    }
  ]
}
```

---

## New Endpoints

| Endpoint | Method | Purpose |
|----------|--------|---------|
| `/questions` | GET | Returns question_map.json |
| `/qa` | GET | Returns saved responses from qa_responses.json |
| `/qa` | POST | Saves an answer `{questionId, answer, timestamp}` |
| `/qa/stage` | POST | Stages an answer for review before bible application |
| `/qa/approve` | POST | Approves a staged answer, appending it to the bible markdown |
| `/ideas` | GET | Returns saved ideas from ideas_inbox.json |
| `/ideas` | POST | Saves organized idea `{raw, organized, timestamp}` |
| `/ideas/apply` | POST | Appends an idea paragraph to the bible markdown |

---

## Implementation Plan

### Phase 1: Build question_map.json
**Files:** `18_EXPORTS/question_map.json` (new)
- Parse INPUT_WORKBOOK.md to extract all Q*.* questions with text, options, and application notes
- Parse OPEN_QUESTIONS.md to extract all W/M/C/F/FA/... questions with severity and workbook refs
- Map each question to bible section IDs (using the section heading slugs from the bible)
- Build keyword lists per section from bible section titles and content
- Output as structured JSON

### Phase 2: Backend endpoints in bible_server.py
**Files:** `18_EXPORTS/bible_server.py` (modify)
- Add `json` import
- Add helper functions: `load_json()`, `save_json()`, `append_to_bible()`
- Add `do_POST` method to Handler class
- Implement all 7 endpoints listed above
- `append_to_bible()` finds the target section heading in the bible markdown and inserts content before the next section, tagged with `[Q&A Response]` or `[Ideas Inbox]` + timestamp

### Phase 3: Q&A View UI
**Files:** `18_EXPORTS/bible_server.py` (modify, inline HTML)
- Add tab bar to header: "Bible" | "Q&A" | "Ideas" (gold underline on active tab)
- Q&A view layout:
  - Left: filter sidebar (by severity: BLOCKING/IMPORTANT/ENRICHING/FUTURE, by system area, answered/unanswered toggle)
  - Center: question cards grouped by bible section
  - Each card: question text, description, options (if any), severity badge, textarea for answer
  - "Save" auto-fires on input (debounced 2s) via POST `/qa`
  - "Stage for Bible" button per answered question -> moves answer to a review queue
  - Review queue panel at top of Q&A view: shows all staged answers with "Approve" (applies to bible) and "Edit" (returns to card) actions
  - Link icon on each card -> clicks through to the relevant bible section in the Bible tab
- Progress bar in header showing answered/total count
- Visual severity badges reusing existing bible color palette

### Phase 4: Ideas Inbox View UI
**Files:** `18_EXPORTS/bible_server.py` (modify, inline HTML)
- Ideas view layout:
  - Large textarea (min 400px height) with placeholder: "Paste or type game ideas here. Separate distinct ideas with blank lines."
  - "Organize" button below the textarea
  - Results panel: organized paragraphs grouped by bible section
    - Each paragraph in a card with section header
    - Dropdown to reassign section
    - "Uncategorized" bucket at bottom, with top 2-3 closest section suggestions as clickable pills per paragraph
  - "Save All" button -> POST `/ideas`
  - "Apply" button per paragraph -> POST `/ideas/apply`
  - History section: previously submitted ideas (collapsed, expandable)
- Client-side `organizeIdeas()` function:
  - Split on double-newline into paragraphs
  - Tokenize each paragraph (lowercase, strip punctuation)
  - Score against section keyword lists from question_map.json
  - Minimum 2 keyword matches to assign (else Uncategorized)
  - Return sorted by section order

### Phase 5: Polish
- Keyboard shortcuts: `1/2/3` to switch tabs
- Tab state persists in URL hash (`#bible`, `#qa`, `#ideas`)
- Auto-save indicator ("Saved" flash on successful POST)
- Q&A sidebar badge counts in Bible tab sidebar (how many unanswered per section)
- Mobile responsive: tabs stack, sidebar collapses

---

## Critical Files

| File | Action | Purpose |
|------|--------|---------|
| `18_EXPORTS/bible_server.py` | Modify | All backend endpoints + expanded inline HTML |
| `18_EXPORTS/question_map.json` | Create | Question-to-section mapping + section keywords |
| `18_EXPORTS/qa_responses.json` | Auto-created | Persisted Q&A answers |
| `18_EXPORTS/ideas_inbox.json` | Auto-created | Persisted idea submissions |
| `03_PROMPTS/INPUT_WORKBOOK.md` | Read-only | Source for workbook questions |
| `17_TASKS/OPEN_QUESTIONS.md` | Read-only | Source for open questions |
| `01_CANON/BLOODLINES_COMPLETE_DESIGN_BIBLE.md` | Append-only | Target for applied answers/ideas |
| `18_EXPORTS/workbook.html` | Reference | Existing Q&A patterns to carry forward |

---

## Verification

1. Start server: `python 18_EXPORTS/bible_server.py`
2. Open `http://localhost:8089` — bible view should work exactly as before
3. Click "Q&A" tab — questions load grouped by section, severity badges show
4. Answer a question, verify auto-save (check qa_responses.json on disk)
5. Click "Apply to Bible" — verify content appended to bible markdown
6. Switch to Bible tab — verify applied content renders in the correct section
7. Click "Ideas" tab — paste multi-paragraph text, click "Organize"
8. Verify paragraphs sort into correct sections based on keywords
9. Reassign a paragraph via dropdown, click "Save All"
10. Click "Apply" on a paragraph — verify appended to bible
11. Test filters in Q&A (severity, answered/unanswered)
12. Test on mobile viewport width
