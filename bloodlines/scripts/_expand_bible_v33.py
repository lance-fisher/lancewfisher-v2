#!/usr/bin/env python3
"""
Expand the v3.3 bible with a Part XVII appendix containing full verbatim
content of every file touched during the 2026-04-14 session, plus a runtime
code implementation record and cross-session coordination note.

This script is idempotent: it rebuilds the Part XVII block each time it is
run by splitting the bible at the "End of Part XVI" marker and the
"End of Design Bible v3.3" marker and rewriting whatever lives between.
"""

from pathlib import Path
import re

ROOT = Path(__file__).resolve().parent.parent  # bloodlines/
BIBLE = ROOT / "18_EXPORTS" / "BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.3.md"

bible_text = BIBLE.read_text(encoding="utf-8")

# Find split points.
end_part_xvi_line = "*End of Part XVI*"
end_bible_line = "*End of Design Bible v3.3*"

# Take everything through the blank line after end-of-Part-XVI closer.
# We want to preserve the existing "---" separator that follows Part XVI.
part_xvi_index = bible_text.find(end_part_xvi_line)
assert part_xvi_index != -1, "End of Part XVI marker not found"

# Advance to end of the Part XVI closing block (next "---" separator).
tail = bible_text[part_xvi_index:]
first_sep = tail.find("\n---\n", 0)
assert first_sep != -1, "Separator after Part XVI not found"
pre_body_end = part_xvi_index + first_sep + len("\n---\n")
pre_body = bible_text[:pre_body_end]

# Find the end-of-bible closing block to re-attach.
end_bible_index = bible_text.find(end_bible_line, pre_body_end)
assert end_bible_index != -1, "End of Design Bible v3.3 marker not found"
# Extend backward to the "---" separator preceding it, so we re-attach the canonical closing.
closing_block_start = bible_text.rfind("\n---\n", pre_body_end, end_bible_index)
assert closing_block_start != -1, "Closing block separator not found"
# The re-attached tail starts at closing_block_start + 1 (drop the leading newline of separator to avoid double).
closing_block = bible_text[closing_block_start + 1:]


def read(path):
    return (ROOT / path).read_text(encoding="utf-8").rstrip() + "\n"


def extract_from(path, start_marker, end_marker=None):
    text = (ROOT / path).read_text(encoding="utf-8")
    idx = text.find(start_marker)
    if idx == -1:
        return f"*(Marker '{start_marker}' not found in {path})*\n"
    if end_marker:
        end = text.find(end_marker, idx)
        if end == -1:
            end = len(text)
        return text[idx:end].rstrip() + "\n"
    return text[idx:].rstrip() + "\n"


sections = []

sections.append(
    "\n## PART XVII — COMPLETE 2026-04-14 CANON APPENDIX (FULL VERBATIM)\n"
    "\n"
    "*This appendix is appended per Bloodlines additive archival rules. Where Part XVI "
    "(Sections 82-85) provided integration summaries, this Part XVII reproduces the FULL "
    "verbatim content of every canon file, system specification, documentation extension, "
    "and code-implementation record produced during the 2026-04-14 session. Nothing in this "
    "appendix contradicts or reduces Part XVI. It exists so that a single bible export "
    "contains every piece of design and implementation record from 2026-04-14, per operator "
    "direction.*\n"
    "\n"
    "*Coordination note: A parallel consolidation session (2026-04-14) is producing a "
    "separate master artifact at `18_EXPORTS/BLOODLINES_COMPLETE_BIBLE_ALL_v2.0_2026-04-14.md` "
    "that absorbs every canon directory, creative branch, archive, research file, and tasks "
    "entry into a single unified reference. This v3.3 bible and the ALL v2.0 master are "
    "complementary. Both are canonical reference surfaces.*\n"
)


def heading(letter, title, source_note):
    sections.append(
        "\n---\n\n"
        f"### Appendix XVII.{letter} — {title}\n\n"
        f"*Source: {source_note}*\n\n"
    )


heading("A", "Defensive Fortification Doctrine (Full Verbatim)",
        "`01_CANON/DEFENSIVE_FORTIFICATION_DOCTRINE.md`")
sections.append(read("01_CANON/DEFENSIVE_FORTIFICATION_DOCTRINE.md"))

heading("B", "Fortification System Specification (Full Verbatim)",
        "`04_SYSTEMS/FORTIFICATION_SYSTEM.md`")
sections.append(read("04_SYSTEMS/FORTIFICATION_SYSTEM.md"))

heading("C", "Siege System Specification (Full Verbatim)",
        "`04_SYSTEMS/SIEGE_SYSTEM.md`")
sections.append(read("04_SYSTEMS/SIEGE_SYSTEM.md"))

heading("D", "System Index Current State (Full Verbatim)",
        "`04_SYSTEMS/SYSTEM_INDEX.md`")
sections.append(read("04_SYSTEMS/SYSTEM_INDEX.md"))

heading("E", "Territory System 2026-04-14 Addendum (Full Extract)",
        "`04_SYSTEMS/TERRITORY_SYSTEM.md` — 2026-04-14 section only")
sections.append(
    extract_from(
        "04_SYSTEMS/TERRITORY_SYSTEM.md",
        "### Design Content (Added 2026-04-14) — Defensive Fortification Doctrine Integration",
    )
)

heading("F", "Dynastic System 2026-04-14 Addendum (Full Extract)",
        "`04_SYSTEMS/DYNASTIC_SYSTEM.md` — 2026-04-14 section only")
sections.append(
    extract_from(
        "04_SYSTEMS/DYNASTIC_SYSTEM.md",
        "### 2026-04-14 — Defensive Fortification Doctrine Integration",
    )
)

heading("G", "Match Structure 2026-04-14 Section (Full Extract)",
        "`11_MATCHFLOW/MATCH_STRUCTURE.md` — 2026-04-14 section only")
sections.append(
    extract_from(
        "11_MATCHFLOW/MATCH_STRUCTURE.md",
        "## 2026-04-14 — Fortification and Siege Pacing (Defensive Fortification Doctrine Integration)",
        "*Match structure entries are appended",
    )
)

heading("H", "Unit Index 2026-04-14 Addendum (Full Extract)",
        "`10_UNITS/UNIT_INDEX.md` — 2026-04-14 section only")
sections.append(
    extract_from(
        "10_UNITS/UNIT_INDEX.md",
        "## 2026-04-14 — Fortification and Siege Unit Classes (Defensive Fortification Doctrine Integration)",
    )
)

heading("I", "Mechanics Index 2026-04-14 Extension (Full Extract)",
        "`08_MECHANICS/MECHANICS_INDEX.md` — 2026-04-14 section only")
sections.append(
    extract_from(
        "08_MECHANICS/MECHANICS_INDEX.md",
        "## 2026-04-14 — Fortification and Siege Mechanics (Defensive Fortification Doctrine Integration)",
    )
)

heading("J", "Canonical Rules: Fourteenth Session Canon (Full Extract)",
        "`01_CANON/CANONICAL_RULES.md` — 2026-04-14 Fourteenth Session Canon section only")
sections.append(
    extract_from(
        "01_CANON/CANONICAL_RULES.md",
        "## Fourteenth Session Canon — Defensive Fortification Doctrine — 2026-04-14",
    )
)

heading("K", "Canon Lock: Defensive Fortification Section (Full Extract)",
        "`01_CANON/CANON_LOCK.md` — Defensive Fortification section only")
sections.append(
    extract_from(
        "01_CANON/CANON_LOCK.md",
        "## Defensive Fortification — 2026-04-14",
    )
)

heading("L", "Current State Analysis 2026-04-14 Addendum (Full Extract)",
        "`docs/BLOODLINES_CURRENT_STATE_ANALYSIS.md` — Section 11 only")
sections.append(
    extract_from(
        "docs/BLOODLINES_CURRENT_STATE_ANALYSIS.md",
        "## 11. 2026-04-14 Integration Addendum — Dynasty Consequence Cascade and Fortification Doctrine",
    )
)

heading("M", "Definitive Decisions Register: Decision 21 (Full Extract)",
        "`docs/DEFINITIVE_DECISIONS_REGISTER.md` — Decision 21 and follow-on updates")
sections.append(
    extract_from(
        "docs/DEFINITIVE_DECISIONS_REGISTER.md",
        "## Decision 21: Defensive Fortification Doctrine Lock (2026-04-14)",
    )
)

heading("N", "Unity Phase Plan: U17 through U22 (Full Extract)",
        "`docs/unity/PHASE_PLAN.md` — U17 through U22 sections only")
sections.append(
    extract_from(
        "docs/unity/PHASE_PLAN.md",
        "## U17. Fortification Foundation",
        "## Session Discipline",
    )
)

heading("O", "Unity System Map: Section 8 (Full Extract)",
        "`docs/unity/SYSTEM_MAP.md` — Section 8 only")
sections.append(
    extract_from(
        "docs/unity/SYSTEM_MAP.md",
        "## 8. Fortification and Siege Systems (2026-04-14)",
    )
)

heading("P", "Runtime Code Implementation Notes (Dynasty Consequence Cascade)",
        "authored for this bible appendix; records code changes landed 2026-04-14")
sections.append(
    "This appendix records the canonical code changes that landed in the browser runtime "
    "during the 2026-04-14 session. These changes collectively implement the Dynasty "
    "Consequence Cascade per canonical design.\n"
    "\n"
    "#### P.1 Faith Data File (data/faiths.json)\n"
    "\n"
    "All four faiths now carry `prototypeEffects` blocks with Light and Dark doctrine path "
    "effect sets. Each doctrine path defines: `label`, `auraAttackMultiplier`, "
    "`auraRadiusBonus`, `auraSightBonus`, `stabilizationMultiplier`, `captureMultiplier`, "
    "and `populationGrowthMultiplier`. Light doctrines favor resilience and recovery "
    "(higher stabilization, higher population growth). Dark doctrines favor lethality and "
    "deterrence (higher attack multiplier, higher capture rate, lower stabilization).\n"
    "\n"
    "Canonical doctrine labels:\n"
    "\n"
    "- Old Light: Vigilant Flame (light) / Judgment By Fire (dark)\n"
    "- Blood Dominion: Consecrated Blood Oath (light) / Crimson Exaction (dark)\n"
    "- The Order: Mandate Of Stewardship (light) / Edict Of Submission (dark)\n"
    "- The Wild: Rootbound Renewal (light) / Predator's Claim (dark)\n"
    "\n"
    "#### P.2 Simulation Core (src/game/core/simulation.js)\n"
    "\n"
    "Canonical constants added: CAPTURE_PROXIMITY_RADIUS (138 world units), "
    "CAPTIVE_INFLUENCE_TRICKLE (0.022), CAPTIVE_RENOWN_WEIGHT (0.0014), "
    "CAPTIVE_LEDGER_LIMIT (16), FALLEN_LEDGER_LIMIT (12), "
    "LEGITIMACY_LOSS_COMMANDER_KILL (9), LEGITIMACY_LOSS_COMMANDER_CAPTURE (12), "
    "LEGITIMACY_LOSS_HEAD_FALL (18), LEGITIMACY_LOSS_GOVERNOR_LOSS (5), "
    "LEGITIMACY_LOSS_INTERREGNUM (14), LEGITIMACY_RECOVERY_ON_SUCCESSION (7), "
    "SUCCESSION_ROLE_CHAIN, DOCTRINE_DEFAULTS, COMMANDER_BASE_AURA_RADIUS (126), "
    "GOVERNOR_STABILIZATION_BONUS (1.3), GOVERNOR_TRICKLE_BONUS (1.22), "
    "TERRITORY_STABILIZED_LOYALTY (72), FAITH_EXPOSURE_THRESHOLD (100), "
    "FAITH_INTENSITY_TIERS (Unawakened/Latent/Active/Devout/Fervent/Apex).\n"
    "\n"
    "Canonical functions added: isMemberAvailable, findAvailableSuccessor, logPromotion, "
    "adjustLegitimacy, appendFallenLedger, findNearestHostileCombatantFaction, "
    "transferMemberToCaptor, clearCommanderLinksFor, clearGovernorLinksFor, "
    "applySuccessionRipple, backfillHeir, handleCommanderFall, handleGovernorLoss, "
    "restoreDisplacedMembers, updateCaptiveRansomTrickle, finalizeUnitDeaths, "
    "finalizeBuildingDeaths, getFaithDoctrineEffects, getConvictionBand, "
    "refreshConvictionBand, recordConvictionEvent, deriveConvictionScore, "
    "syncCommanderAssignment, syncGovernorAssignments, syncDynastyAssignments, "
    "getCommanderAuraProfile.\n"
    "\n"
    "applyDamage was refactored to a pure damage function: it reduces health, clamps to "
    "zero, and records killedByFactionId on the entity. Death effects are deferred to "
    "finalizeUnitDeaths and finalizeBuildingDeaths, which run after updateUnits inside "
    "stepSimulation.\n"
    "\n"
    "stepSimulation order added: updateCaptiveRansomTrickle(state, dt) and "
    "restoreDisplacedMembers(state) after updateProjectiles and before updateMessages.\n"
    "\n"
    "updateControlPoints now calls handleGovernorLoss before ownership flips, capturing "
    "or displacing the previous governor per the capture proximity rule.\n"
    "\n"
    "createDynastyState now includes: interregnum (false), captives ([]), "
    "attachments.heirMemberId (null), attachments.fallenMembers ([]), "
    "attachments.capturedMembers ({}). Each dynasty member carries capturedByFactionId, "
    "fallenAt, and promotionHistory fields.\n"
    "\n"
    "getFactionSnapshot now exposes captives (with role and path references), "
    "attachments.fallenMembers, attachments.capturedMembers, attachments.heirMember, "
    "and interregnum on the dynasty projection. Member records include capturedByName.\n"
    "\n"
    "Ironmark Blood Levy enforcement in queueProduction requires Ironmark training of "
    "non-worker units to have a living population buffer of 1; on successful levy, "
    "population decrements by 1 and a ruthlessness conviction event is recorded.\n"
    "\n"
    "#### P.3 Presentation Layer (src/game/core/renderer.js)\n"
    "\n"
    "Commander aura ring drawn around units with commanderMemberId. Commander title "
    "rendered below the unit. Control point display includes controlState (occupied or "
    "stabilized) and 'Governor seated' marker when a governor is assigned. Alpha helper "
    "withAlpha coerces hex colors to rgba with alpha for overlays.\n"
    "\n"
    "#### P.4 UI Layer (src/game/main.js)\n"
    "\n"
    "Dynasty panel rebuilt using DOM-safe methods (document.createElement, textContent, "
    "appendChild) to satisfy the security reminder hook. The panel shows: legitimacy, "
    "active member count, interregnum flag, commander attachment, heir line, governor "
    "assignment, held captives block (top 3 with source faction, role, renown), fallen "
    "and captured history block (top 3 with disposition), and living members (top 6).\n"
    "\n"
    "Debug overlay extended with: 'Dynasty: legitimacy N, heir X, captives N, fallen N' "
    "line and the conviction ledger line 'Ledger: ruth N, steward N, oath N, desecrate N'.\n"
    "\n"
    "Doctrine selection UI surfaces Light and Dark doctrine commitment buttons per faith "
    "at the commit threshold (100 exposure) with doctrine effect summary displayed for "
    "each option.\n"
    "\n"
    "#### P.5 Tests (tests/)\n"
    "\n"
    "tests/data-validation.mjs extended with prototypeEffects.light and prototypeEffects.dark "
    "assertion for every faith.\n"
    "\n"
    "tests/runtime-bridge.mjs added (NEW). Validates:\n"
    "\n"
    "- Commander attachment to field unit on simulation start\n"
    "- Doctrine commitment success path and conviction event recording (dark doctrine adds desecration)\n"
    "- Territory occupied-to-stabilized transition over simulation time (305 ticks at 0.2s)\n"
    "- Governor assignment to the held march\n"
    "- Conviction oathkeeping and stewardship accrual from stabilization\n"
    "- Commander capture scenario: placing an enemy combatant near the commander, forcing the commander unit to zero health, stepping the simulation, and asserting the captured status, captor captive ledger, legitimacy drop, and captor ruthlessness accrual\n"
    "- Ransom influence trickle accrual over simulated time\n"
    "- Head-of-bloodline succession: forcing the head into a combat unit and killing it with no captor present, asserting the head is marked fallen, a successor is promoted to head with status 'ruling'\n"
    "\n"
    "All tests pass. All runtime modules pass node --check.\n"
)

heading("Q", "Change Log Entry (Full Verbatim)",
        "`00_ADMIN/CHANGE_LOG.md` — 2026-04-14 entry only")
sections.append(
    extract_from(
        "00_ADMIN/CHANGE_LOG.md",
        "## 2026-04-14: Dynasty Consequence Cascade + Defensive Fortification Doctrine",
    )
)

heading("R", "Tasks: Current Todo State (Full Verbatim)",
        "`tasks/todo.md`")
sections.append(read("tasks/todo.md"))

heading("S", "Tasks: Lessons Recorded (Full Verbatim)",
        "`tasks/lessons.md`")
sections.append(read("tasks/lessons.md"))

sections.append(
    "\n---\n\n"
    "### Appendix XVII.T — Cross-Session Coordination Note\n"
    "\n"
    "A parallel consolidation session (2026-04-14) has been running alongside the session "
    "that produced Part XVI and this Part XVII appendix. The parallel session produced:\n"
    "\n"
    "- Updated `01_CANON/BLOODLINES_DESIGN_BIBLE.md` (working bible; Sections 16-24 added "
    "in consolidation covering Siege, Fortification, Naval, Diplomacy, Operations, "
    "Political Events, Terrain, Governance Framework, and a Development State pointer).\n"
    "- Updated `01_CANON/BLOODLINES_MASTER_MEMORY.md` (cumulative memory; Sections 92 and 93 "
    "added covering the 2026-04-14 Fortification Doctrine and the consolidation session).\n"
    "- Updated `01_CANON/BLOODLINES_APPEND_ONLY_LOG.md` (append-only log; new entries for "
    "2026-04-14).\n"
    "- Pre-consolidation snapshots archived to `19_ARCHIVE/PRE_2026-04-14_CONSOLIDATION/`.\n"
    "- A forthcoming comprehensive master at "
    "`18_EXPORTS/BLOODLINES_COMPLETE_BIBLE_ALL_v2.0_2026-04-14.md` that absorbs every "
    "canon directory, creative branch, archive, research, task, export, docs, and top-level "
    "working file.\n"
    "- Updated viewer at `scripts/bloodlines_viewer.html` and "
    "`deploy/bloodlines/scripts/bloodlines_viewer.html`.\n"
    "- Updated web sync at `scripts/sync-to-web.sh`.\n"
    "\n"
    "The v3.3 bible (this document) and the ALL v2.0 master (produced by the consolidation "
    "session) are complementary canonical reference surfaces. Both exist additively. Prior "
    "v3.2 bible, v1.0 unified, and pre-2026-04-14 canon files are preserved in 18_EXPORTS/ "
    "and 19_ARCHIVE/ per additive-only archival rules. No session during 2026-04-14 has "
    "deleted, summarized, or reduced any prior canon content.\n"
    "\n"
    "---\n"
    "\n"
    "*End of Part XVII*\n"
    "*Appendix XVII incorporates verbatim content from 17 source files produced or extended "
    "during the 2026-04-14 session, plus a cross-session coordination note. This part "
    "ensures the bible contains the full record of all 2026-04-14 work for cross-session "
    "and cross-model continuity.*\n"
)

full_part_xvii = "".join(sections)

# Compose final bible.
# pre_body ends with "\n---\n" after the Part XVI closing block.
# Then Part XVII. Then the closing_block which starts with "---\n" (we dropped the
# leading newline to avoid a doubled blank).
final = pre_body + full_part_xvii + "\n" + closing_block

BIBLE.write_text(final, encoding="utf-8")

new_lines = final.count("\n")
new_bytes = len(final.encode("utf-8"))
print(f"Bible rewritten: {new_lines} lines, {new_bytes} bytes")
