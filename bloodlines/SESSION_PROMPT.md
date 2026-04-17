# Session Prompt — Bloodlines Web Bible Editor

Read `D:\ProjectsHome\Bloodlines\HANDOFF.md` first. It has the complete technical context.

## Task

Modify the Bloodlines web bible viewer (the one deployed to lancewfisher.com, NOT the localhost:8089 bible_server.py) to:

1. **Remove the passkey gate entirely.** The viewer currently requires entering "mario" to access. Remove the gate overlay HTML, all gate CSS, the PASSHASHES array, the sha256/submitPasskey/unlock JS functions, and make the app show immediately without authentication. Clean up the PASSHASH variable in build_index.py too.

2. **Add an inline content editor** so I can submit game design ideas directly from the web viewer without needing Claude Code. The editor should:
   - Have a floating "Add Content" button (bottom-right corner, matching the existing dark theme)
   - Open a slide-up panel with a large textarea for writing/pasting ideas
   - Include an "Organize" button that uses keyword matching to categorize each paragraph by bible section
   - Show the organized results grouped by section with the ability to reassign
   - "Save to Browser" stores in localStorage so ideas persist between visits
   - "Export Markdown" downloads a timestamped .md file that a future Claude Code session can ingest
   - History of previously saved submissions, viewable and clearable

   Port the keyword matching algorithm from `18_EXPORTS/bible_server.py` (the `organizeIdeas()` JS function). The section keywords data is in `18_EXPORTS/question_map.json` under `section_keywords`. Inline this data into the viewer's JS.

3. **Rebuild and verify.** Run `build_index.py` to generate the deployed `index.html`. Verify all existing functionality (sidebar nav, document loading, markdown rendering) still works alongside the new editor.

## Key Files

- Source template: `15_PROTOTYPE/index_body.html` (modify this)
- Build script: `15_PROTOTYPE/build_index.py` (modify this)
- Section keywords: `18_EXPORTS/question_map.json` (read section_keywords from this)
- Algorithm reference: `18_EXPORTS/bible_server.py` (port organizeIdeas() from this)
- Output: `FisherSovereign/lancewfisher-v2/bloodlines/index.html` (auto-generated)

## Constraints

- Do NOT modify any markdown content files. They are current and correct.
- Do NOT touch bible_server.py or the localhost:8089 system.
- Match the existing aesthetic: dark theme, ember/gold accents, Cinzel + Crimson Pro fonts.
- Keep the viewer as a static single-file HTML (no server required for the deployed version).
