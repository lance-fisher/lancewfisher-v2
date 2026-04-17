#!/bin/bash
# Bloodlines Design Archive — Sync to lancewfisher-v2 website
# Run this after any design file updates to push changes to the web viewer
# Usage: bash scripts/sync-to-web.sh

SRC="D:/ProjectsHome/Bloodlines"
DST="D:/ProjectsHome/FisherSovereign/lancewfisher-v2/deploy/bloodlines"

echo "Syncing Bloodlines design files to web viewer..."

# Admin
cp "$SRC/00_ADMIN/PROJECT_STATUS.md" "$DST/00_ADMIN/"
cp "$SRC/00_ADMIN/CHANGE_LOG.md" "$DST/00_ADMIN/" 2>/dev/null || true
cp "$SRC/00_ADMIN/CURRENT_GAME_STATE.md" "$DST/00_ADMIN/" 2>/dev/null || true
cp "$SRC/00_ADMIN/DIRECTORY_MAP.md" "$DST/00_ADMIN/" 2>/dev/null || true
cp "$SRC/00_ADMIN/WORKFLOW_RULES.md" "$DST/00_ADMIN/" 2>/dev/null || true

# Canon (short bible with sections 16-24, append-only log, canon lock, design guide, defensive fortification doctrine)
cp "$SRC/01_CANON/BLOODLINES_DESIGN_BIBLE.md" "$DST/01_CANON/"
cp "$SRC/01_CANON/BLOODLINES_MASTER_MEMORY.md" "$DST/01_CANON/"
cp "$SRC/01_CANON/BLOODLINES_APPEND_ONLY_LOG.md" "$DST/01_CANON/" 2>/dev/null || true
cp "$SRC/01_CANON/BLOODLINES_STRUCTURE_INDEX.md" "$DST/01_CANON/" 2>/dev/null || true
cp "$SRC/01_CANON/CANONICAL_RULES.md" "$DST/01_CANON/"
cp "$SRC/01_CANON/CANON_LOCK.md" "$DST/01_CANON/" 2>/dev/null || true
cp "$SRC/01_CANON/DESIGN_GUIDE.md" "$DST/01_CANON/" 2>/dev/null || true
cp "$SRC/01_CANON/DEFENSIVE_FORTIFICATION_DOCTRINE.md" "$DST/01_CANON/" 2>/dev/null || true

# Core Systems (10 systems including Fortification and Siege added 2026-04-14)
cp "$SRC/04_SYSTEMS/SYSTEM_INDEX.md" "$DST/04_SYSTEMS/"
cp "$SRC/04_SYSTEMS/CONVICTION_SYSTEM.md" "$DST/04_SYSTEMS/"
cp "$SRC/04_SYSTEMS/FAITH_SYSTEM.md" "$DST/04_SYSTEMS/"
cp "$SRC/04_SYSTEMS/POPULATION_SYSTEM.md" "$DST/04_SYSTEMS/"
cp "$SRC/04_SYSTEMS/RESOURCE_SYSTEM.md" "$DST/04_SYSTEMS/"
cp "$SRC/04_SYSTEMS/TERRITORY_SYSTEM.md" "$DST/04_SYSTEMS/"
cp "$SRC/04_SYSTEMS/DYNASTIC_SYSTEM.md" "$DST/04_SYSTEMS/"
cp "$SRC/04_SYSTEMS/BORN_OF_SACRIFICE_SYSTEM.md" "$DST/04_SYSTEMS/"
cp "$SRC/04_SYSTEMS/FORTIFICATION_SYSTEM.md" "$DST/04_SYSTEMS/" 2>/dev/null || true
cp "$SRC/04_SYSTEMS/SIEGE_SYSTEM.md" "$DST/04_SYSTEMS/" 2>/dev/null || true

# Lore, Factions, Faiths
cp "$SRC/05_LORE/TIMELINE.md" "$DST/05_LORE/"
cp "$SRC/05_LORE/WORLD_HISTORY.md" "$DST/05_LORE/" 2>/dev/null || true
cp "$SRC/06_FACTIONS/FOUNDING_HOUSES.md" "$DST/06_FACTIONS/"
cp "$SRC/07_FAITHS/FOUR_ANCIENT_FAITHS.md" "$DST/07_FAITHS/"

# Mechanics
mkdir -p "$DST/08_MECHANICS/"
cp "$SRC/08_MECHANICS/MECHANICS_INDEX.md" "$DST/08_MECHANICS/" 2>/dev/null || true
cp "$SRC/08_MECHANICS/OPERATIONS_SYSTEM.md" "$DST/08_MECHANICS/"
cp "$SRC/08_MECHANICS/DIPLOMACY_SYSTEM.md" "$DST/08_MECHANICS/"
cp "$SRC/08_MECHANICS/POLITICAL_EVENTS.md" "$DST/08_MECHANICS/"

# World and Terrain
cp "$SRC/09_WORLD/WORLD_INDEX.md" "$DST/09_WORLD/"
cp "$SRC/09_WORLD/TERRAIN_SYSTEM.md" "$DST/09_WORLD/" 2>/dev/null || true

# Units
cp "$SRC/10_UNITS/UNIT_INDEX.md" "$DST/10_UNITS/"

# Match Flow
cp "$SRC/11_MATCHFLOW/MATCH_STRUCTURE.md" "$DST/11_MATCHFLOW/"
cp "$SRC/11_MATCHFLOW/POLITICAL_EVENTS.md" "$DST/11_MATCHFLOW/"
cp "$SRC/11_MATCHFLOW/NAVAL_SYSTEM.md" "$DST/11_MATCHFLOW/"

# UI and A/V
cp "$SRC/12_UI_UX/UI_NOTES.md" "$DST/12_UI_UX/"
cp "$SRC/13_AUDIO_VISUAL/AUDIO_VISUAL_DIRECTION.md" "$DST/13_AUDIO_VISUAL/"

# Exports (includes the v3.3 bible from the concurrent session and the ALL v2.0 master)
cp "$SRC/18_EXPORTS/BLOODLINES_COMPLETE_DESIGN_BIBLE.md" "$DST/18_EXPORTS/"
cp "$SRC/18_EXPORTS/BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.2.md" "$DST/18_EXPORTS/"
cp "$SRC/18_EXPORTS/BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.3.md" "$DST/18_EXPORTS/" 2>/dev/null || true
cp "$SRC/18_EXPORTS/BLOODLINES_COMPLETE_UNIFIED_v1.0.md" "$DST/18_EXPORTS/"
for allmaster in "$SRC/18_EXPORTS/"BLOODLINES_COMPLETE_BIBLE_ALL_v2.0_*.md; do
  [ -f "$allmaster" ] && cp "$allmaster" "$DST/18_EXPORTS/"
done

# Archive index (individual legacy files remain in source archive)
cp "$SRC/19_ARCHIVE/LEGACY_ARCHIVE_INDEX.md" "$DST/19_ARCHIVE/" 2>/dev/null || true

# Web viewer HTML — source of truth is scripts/bloodlines_viewer.html
# Update version badge, stats, and last-updated date in that file each session, then sync here.
cp "$SRC/scripts/bloodlines_viewer.html" "$DST/index.html"

echo "Done. Files synced to $DST"
echo "Upload deploy/bloodlines/ to Hostinger to publish."
echo "NOTE: Before running this script, update scripts/bloodlines_viewer.html with current version, stats, and last-updated date."
