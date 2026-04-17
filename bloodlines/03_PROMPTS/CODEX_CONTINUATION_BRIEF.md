# Bloodlines — Codex Continuation Brief (Claude/Codex Alternation)

**For Codex runs on the Bloodlines project.** Paste the section below the `---` as your first message to Codex when manually starting it, or configure it as the recurring prompt in any Codex scheduler you set up.

## Alternation Protocol Summary

- Claude fires every 5 hours at :12 past the hour (00:12, 05:12, 10:12, 15:12, 20:12) via the `bloodlines-claude-alternation` scheduled task.
- Codex should fire at staggered times (suggested: 02:42 AM, 07:42 AM, 12:42 PM, 05:42 PM, 10:42 PM) so the two agents alternate with ~2.5 hour gaps.
- Both agents read `NEXT_SESSION_HANDOFF.md` on start and update it on close. The handoff is the sole coordination surface.
- Both agents produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_N.md` on close with N = next unused number. The N suffix makes it clear which agent produced it (check the header of each report for "Author: Claude" or "Author: Codex").
- Both agents extend `00_ADMIN/CHANGE_LOG.md`, `CURRENT_PROJECT_STATE.md`, `continuity/PROJECT_STATE.json`, `tasks/todo.md` additively.

---

You are continuing Bloodlines, a grand dynastic civilizational RTS game, as part of a Claude/Codex alternation protocol. You are the Codex side of the alternation. Claude may have run the most recent session, or you may be picking up from an earlier Codex run. Read `NEXT_SESSION_HANDOFF.md` first to determine the state regardless of which agent ran last.

CANONICAL ROOT

`D:\ProjectsHome\Bloodlines` (resolves via junction to `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\bloodlines`). Do not create parallel roots. Do not treat deploy/bloodlines, frontier-bastion, or archive_preserved_sources/ as active roots.

READ BEFORE ANY EDIT

1. `AGENTS.md` — canonical entry for any AI agent on this project.
2. `README.md` — project overview.
3. `CLAUDE.md` — governance and preservation rules (applies to Codex equally).
4. `MASTER_PROJECT_INDEX.md`
5. `MASTER_BLOODLINES_CONTEXT.md` — especially the Session 9 addendum and any later addenda.
6. `CURRENT_PROJECT_STATE.md`
7. `NEXT_SESSION_HANDOFF.md` — the single authoritative next-action file.
8. `SOURCE_PROVENANCE_MAP.md`
9. `continuity/PROJECT_STATE.json`
10. `01_CANON/BLOODLINES_MASTER_DESIGN_DOCTRINE_2026-04-14.md` — the creator's full-scale preservation directive.
11. `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_9.md` — the full-realization state analysis.
12. `docs/BLOODLINES_SYSTEM_GAP_ANALYSIS_2026-04-14_SESSION_9.md`
13. `docs/plans/2026-04-14-session-9-full-realization-continuation-plan.md`
14. `docs/plans/2026-04-14-session-9-next-phase-execution-roadmap.md`
15. The most recent `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_*.md` — check whether the previous agent was Claude or Codex by reading the header.
16. `tasks/todo.md`
17. `03_PROMPTS/CONTINUATION_PROMPT_2026-04-14_SESSION_11_REVISED.md` — full preservation directive (canonical).

NON-NEGOTIABLE POSTURE (applies equally to Codex and Claude)

1. No scope reduction. Bloodlines stays at its full intended scale.
2. Additive canon only. New material goes into `02_SESSION_INGESTIONS/` with dated filenames, or appended as dated sections.
3. Preservation of prior session work. Every live system stays live.
4. Canonical depth over symbolic completion. No placeholder UI, no decorative dead surfaces.
5. Tests stay green before close.
6. Legibility follows depth within two sessions.
7. Unity lane respects its 6.3 LTS lock — browser simulation is the working spec.
8. No false simplification. No "good enough for now" without canonical follow-up logged.
9. No placeholder completion fraud. Report progress as DOCUMENTED / PARTIAL / LIVE / BLOCKED / VERIFIED.
10. Precision over reassurance. No em dashes.

CANONICAL INTERDEPENDENCY MANDATE

Every newly advanced system must interact with at least two already-live systems whenever the design logically permits. No isolated feature islands.

SIX VECTORS — ADVANCE ONE OR MORE PER SESSION

1. Civilizational depth (population, water, food, land, forestry, labor, logistics)
2. Dynastic depth (bloodline members as agents, succession, marriage, lesser houses)
3. Military depth (land and naval doctrine, commanders, fortification, siege, supply)
4. Faith and conviction depth (four covenants, Light/Dark paths, 5-tier ladders, zeal, apex)
5. World depth (continental geography, 5-stage match, multi-speed time, dual-clock)
6. Legibility depth (theatre zoom, 11-state dashboard, iconography, overlays)

WORK SELECTION

Priority: handoff → roadmap → todo → lagging vector. Pick by leverage, not ease.

WORK EXECUTION

Read surface. Identify canonical target. Implement first live canonical layer additively. Wire into simulation, legibility, AI awareness. Extend tests. Log follow-up layers.

VERIFICATION REQUIRED BEFORE CLOSE

```
cd D:/ProjectsHome/Bloodlines
node tests/data-validation.mjs
node tests/runtime-bridge.mjs
node --check src/game/main.js
node --check src/game/core/simulation.js
node --check src/game/core/renderer.js
node --check src/game/core/ai.js
node --check src/game/core/data-loader.js
node --check src/game/core/utils.js
```

Live runtime verification via `python -m http.server 8057 --directory D:/ProjectsHome/Bloodlines` and `http://localhost:8057/play.html`: launch transitions, resource bar, 11-pill dashboard, dynasty + faith panels, zero console errors, zero failed requests.

CLOSE PROTOCOL

Write a new `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_N.md` with **Author: Codex** in the header so Claude knows on the next fire. Update `00_ADMIN/CHANGE_LOG.md`, `CURRENT_PROJECT_STATE.md`, `NEXT_SESSION_HANDOFF.md`, `continuity/PROJECT_STATE.json`, `tasks/todo.md`. Reclassify items in the session 9 gap analysis that moved DOCUMENTED → PARTIAL → LIVE.

At close, state clearly: what moved to LIVE, what moved to PARTIAL, what remains blocked, what the next recommended action is, what canonical follow-up layers remain.

HANDOFF ALTERNATION NOTE

If the previous session report author is Claude, you are the Codex alternation fire. If the previous author is Codex, you are picking up from a prior Codex run (Claude's schedule may have been skipped due to usage limits or interruption). Either way, the handoff is the canonical source. Advance the project additively.

COMMUNICATION RULES

Direct. Precise. No hedging. No reassurance padding. Preserve the full vision. No em dashes.

Begin.
