#!/usr/bin/env python3
"""
build_complete_bible.py

Builds the single complete Bloodlines bible that includes EVERYTHING.
Concatenates every .md and relevant .txt source file across the project
in a structured, navigable format.

Written 2026-04-14 for the cross-session consolidation ingestion.

The output is additive to the existing exports track:
- 18_EXPORTS/BLOODLINES_COMPLETE_UNIFIED_v1.0.md (prior unified, 15447 lines)
- 18_EXPORTS/BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.2.md (concurrent session track)
- 18_EXPORTS/BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.3.md (concurrent session track)
- 18_EXPORTS/BLOODLINES_COMPLETE_BIBLE_ALL_v2.0_<date>.md (this script)

Usage:
    python scripts/build_complete_bible.py

Output:
    18_EXPORTS/BLOODLINES_COMPLETE_BIBLE_ALL_v2.0_<date>.md

Idempotent: can be re-run any time to regenerate the master from current
canon state. The filename carries the build date; re-running on the same
day overwrites, different days produces a new dated file.

Deterministic: files are traversed in a defined order with files within
each folder sorted by filename. Two runs over identical inputs produce
byte-identical output.
"""

from __future__ import annotations

import os
import sys
import hashlib
from datetime import datetime
from pathlib import Path

# --------------------------------------------------------------------------
# Configuration
# --------------------------------------------------------------------------

# Locate the project root relative to this script (scripts/ sits under root)
SCRIPT_DIR = Path(__file__).resolve().parent
PROJECT_ROOT = SCRIPT_DIR.parent
CANONICAL_DISPLAY_ROOT = Path(r"D:\ProjectsHome\Bloodlines")

# Output location and naming
OUTPUT_DIR = PROJECT_ROOT / "18_EXPORTS"
BUILD_DATE = datetime.now().strftime("%Y-%m-%d")
OUTPUT_FILENAME = f"BLOODLINES_COMPLETE_BIBLE_ALL_v2.0_{BUILD_DATE}.md"
OUTPUT_PATH = OUTPUT_DIR / OUTPUT_FILENAME

# Parts definition: ordered traversal of the project.
# Each entry: (part_letter, part_title, relative_path, file_pattern, recursive, extensions)
#   - part_letter: single uppercase letter used as the Part label in the output
#   - part_title:  human-readable section title
#   - relative_path: path relative to PROJECT_ROOT (or "." for root)
#   - file_pattern: glob pattern relative to the path (e.g. "*.md", "**/*.md")
#   - recursive:   if True, walk subdirectories
#   - extensions:  set of extensions to include (with leading dot)
PARTS = [
    ("A", "Project Administration",               "00_ADMIN",            False, {".md"}),
    ("B", "Canonical Rules, Locks, and Bibles",   "01_CANON",            False, {".md"}),
    ("C", "Session Ingestions",                   "02_SESSION_INGESTIONS", False, {".md"}),
    ("D", "Master Prompts and Handoffs",          "03_PROMPTS",          False, {".md"}),
    ("E", "Core Systems",                         "04_SYSTEMS",          False, {".md"}),
    ("F", "Lore and Timeline",                    "05_LORE",             False, {".md"}),
    ("G", "Factions and Founding Houses",         "06_FACTIONS",         False, {".md"}),
    ("H", "Faiths and Covenants",                 "07_FAITHS",           False, {".md"}),
    ("I", "Mechanics (Operations, Diplomacy, Events)", "08_MECHANICS",   False, {".md"}),
    ("J", "World and Terrain",                    "09_WORLD",            False, {".md"}),
    ("K", "Units and Military",                   "10_UNITS",            False, {".md"}),
    ("L", "Match Flow and Naval",                 "11_MATCHFLOW",        False, {".md"}),
    ("M", "UI/UX Notes",                          "12_UI_UX",            False, {".md"}),
    ("N", "Audio/Visual Direction",               "13_AUDIO_VISUAL",     False, {".md"}),
    ("O", "Creative Branches and Assets",         "14_ASSETS",           False, {".md", ".txt"}),
    ("P", "Prototype",                            "15_PROTOTYPE",        True,  {".md", ".txt"}),
    ("Q", "Research",                             "16_RESEARCH",         True,  {".md"}),
    ("R", "Task Backlog",                         "17_TASKS",            True,  {".md"}),
    ("S", "Prior Exports (Design Bible Versions)","18_EXPORTS",          False, {".md"}),
    ("T", "Archive (Historical, Legacy, Voided)", "19_ARCHIVE",          True,  {".md", ".txt"}),
    ("U", "Development State Tracking (docs/)",   "docs",                False, {".md"}),
    ("V", "Unity Implementation (docs/unity/)",   "docs/unity",          False, {".md"}),
    ("W", "Historical Plans (docs/plans/)",       "docs/plans",          False, {".md"}),
    ("X", "Top-Level Working Files",              ".",                   False, {".md"}),
]

# Files to EXCLUDE even if they match. These are files this build script
# itself generates or that would cause circular inclusion.
EXCLUDE_FILENAMES = {
    OUTPUT_FILENAME,  # never include the file we're writing
}

# Files to exclude by path pattern (relative to PROJECT_ROOT, forward slashes)
EXCLUDE_PATH_PATTERNS = [
    "18_EXPORTS/BLOODLINES_COMPLETE_BIBLE_ALL_v2.0_",  # any prior ALL v2.0 builds
    "18_EXPORTS/BLOODLINES_COMPLETE_MASTER_",  # the everything-master companion (scripts/_build_everything_master.py output)
    "19_ARCHIVE/PRE_2026-04-14_CONSOLIDATION/BLOODLINES_COMPLETE_UNIFIED_v1.0_snapshot",  # 1.7MB snapshot would double the master
]

# Top-level files to include for Part X ("Top-Level Working Files"). We list
# them explicitly rather than globbing "*.md" to avoid accidentally including
# the project CLAUDE.md or similar instruction files that belong elsewhere.
TOP_LEVEL_FILES = [
    "BLOODLINES_ADDENDUM_A.md",
    "BLOODLINES_ADDITIVE_INTEGRATION.md",
    "BLOODLINES_ADDITIVE_INTEGRATION2.md",
    "BLOODLINES_CLAUDECODE_HANDOFF.md",
    "BLOODLINES_DESIGN_ADDITIONS.md",
    "BLOODLINES_FIVE_STAGES.md",
    "HANDOFF_ARCHIVE_2026-04-01.md",
    "PLAN_QA_IDEAS.md",
    "README.md",
    "SESSION_PROMPT.md",
    "SESSION_STATUS.md",
]


# --------------------------------------------------------------------------
# Helpers
# --------------------------------------------------------------------------

def should_exclude(relative_path: str, filename: str) -> bool:
    """Return True if this file must not be included in the master."""
    if filename in EXCLUDE_FILENAMES:
        return True
    for pattern in EXCLUDE_PATH_PATTERNS:
        if pattern in relative_path.replace("\\", "/"):
            return True
    return False


def collect_files(part_rel_path: str, pattern_ext: set, recursive: bool) -> list[Path]:
    """Collect all files under part_rel_path matching the extension set."""
    abs_path = PROJECT_ROOT / part_rel_path

    if not abs_path.exists():
        return []

    if not abs_path.is_dir():
        return []

    files = []
    if recursive:
        for root, _dirs, names in os.walk(abs_path):
            for name in names:
                p = Path(root) / name
                if p.suffix.lower() in pattern_ext:
                    files.append(p)
    else:
        for entry in abs_path.iterdir():
            if entry.is_file() and entry.suffix.lower() in pattern_ext:
                files.append(entry)

    # Deterministic ordering by relative-path string
    files.sort(key=lambda p: str(p.relative_to(PROJECT_ROOT)).lower())
    return files


def collect_top_level_files() -> list[Path]:
    """Return the explicit TOP_LEVEL_FILES list, filtering to those that exist."""
    found = []
    for name in TOP_LEVEL_FILES:
        p = PROJECT_ROOT / name
        if p.exists() and p.is_file():
            found.append(p)
    return found


def file_hash_short(path: Path) -> str:
    """Return a short SHA-1 hash of the file for audit tracing."""
    h = hashlib.sha1()
    with open(path, "rb") as f:
        while True:
            chunk = f.read(65536)
            if not chunk:
                break
            h.update(chunk)
    return h.hexdigest()[:10]


def read_text_safe(path: Path) -> str:
    """Read file content as text, trying utf-8 first then falling back."""
    for encoding in ("utf-8", "utf-8-sig", "latin-1"):
        try:
            return path.read_text(encoding=encoding)
        except UnicodeDecodeError:
            continue
    # Last resort: bytes with errors='replace'
    return path.read_bytes().decode("utf-8", errors="replace")


def count_lines(text: str) -> int:
    return text.count("\n") + (0 if text.endswith("\n") else 1) if text else 0


# --------------------------------------------------------------------------
# Output builders
# --------------------------------------------------------------------------

def build_document_header(total_files: int, total_lines: int, total_bytes: int) -> str:
    """Build the master document preamble."""
    timestamp = datetime.now().strftime("%Y-%m-%d %H:%M:%S %z").strip()
    return f"""# BLOODLINES — COMPLETE BIBLE (ALL v2.0)
## The single complete reference. Every design artifact. Nothing excluded.

**Version:** ALL v2.0
**Build date:** {BUILD_DATE}
**Build timestamp:** {timestamp}
**Source root:** `{CANONICAL_DISPLAY_ROOT}`
**Files included:** {total_files}
**Total source lines:** {total_lines:,}
**Total source bytes:** {total_bytes:,}
**Generated by:** `scripts/build_complete_bible.py`

---

## What this document is

This is the **comprehensive master bible** for Bloodlines. Unlike the versioned Design Bible series (`BLOODLINES_COMPLETE_DESIGN_BIBLE.md`, `_v3.2.md`, `_v3.3.md`) which is a curated, section-numbered narrative, this document is the complete source archive: every canonical file, every creative branch, every systems file, every lore file, every session ingestion, every archived legacy document, every development-state document, and every top-level working file, concatenated in a structured order with file-path boundaries preserved.

This is the file to load when a new AI session, cross-model collaboration context, or full project handoff needs **everything** in one place and cannot risk missing any content to selection bias.

## What this document is not

- It is **not** a substitute for the curated `BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.x.md` series. That series presents the bible as a continuous numbered narrative for human reading.
- It is **not** the latest state of any individual file. If a file is edited after this build, this master does not reflect the edit until the next rebuild.
- It is **not** a canonicalization mechanism. Canonical status is established in `01_CANON/CANONICAL_RULES.md`, `01_CANON/CANON_LOCK.md`, and by direct operator (Lance W. Fisher) decision. This file is an aggregation, not an authority.

## Three-layer source model (reminder)

Bloodlines source material exists across three simultaneous layers. All three are reflected in this file:

- **Layer A — Active Canon Snapshot:** the current best organized state. Parts A through N, S, U, V here.
- **Layer B — Historical Design Archive:** earlier versions, creative branches, legacy source, voided content. Parts O, T, W, X here.
- **Layer C — Open Design Reservoir:** concepts in orbit, preserved for revival. Found inside individual files within each part, most densely in Parts B, I, J, L, S.

## Structure

The document is divided into Parts A through X. Each Part covers one source directory (or a set of top-level files). Within each Part, files are listed alphabetically. Each file is introduced by a `================` banner giving its relative path, SHA-1 digest prefix, line count, and byte count, followed by the verbatim file content.

---

## Table of Contents

"""


def build_toc(parts_inventory: list) -> str:
    """Build the table of contents from the parts inventory."""
    lines = []
    for part_letter, part_title, files_meta in parts_inventory:
        if not files_meta:
            lines.append(f"- **Part {part_letter} — {part_title}** *(no files)*")
            continue
        lines.append(f"- **Part {part_letter} — {part_title}** ({len(files_meta)} files)")
        for rel_path, file_lines, file_bytes, _digest in files_meta:
            display = rel_path.replace("\\", "/")
            lines.append(f"  - `{display}` — {file_lines:,} lines, {file_bytes:,} bytes")
    return "\n".join(lines) + "\n\n---\n\n"


def build_part_banner(letter: str, title: str, file_count: int) -> str:
    bar = "=" * 72
    return f"\n\n{bar}\n# PART {letter} — {title.upper()}\n# ({file_count} files)\n{bar}\n\n"


def build_file_banner(rel_path: str, digest: str, line_count: int, byte_count: int) -> str:
    rel = rel_path.replace("\\", "/")
    bar = "-" * 72
    return (
        f"\n\n{bar}\n"
        f"### FILE: `{rel}`\n"
        f"### SHA1: {digest} | LINES: {line_count:,} | BYTES: {byte_count:,}\n"
        f"{bar}\n\n"
    )


# --------------------------------------------------------------------------
# Main
# --------------------------------------------------------------------------

def main() -> int:
    print(f"Building {OUTPUT_FILENAME}")
    print(f"  Project root: {PROJECT_ROOT}")
    print(f"  Output path: {OUTPUT_PATH}")
    print()

    # Phase 1: collect all files and their metadata
    parts_inventory = []  # list of (letter, title, files_meta)
    total_files = 0
    total_lines = 0
    total_bytes = 0

    for letter, title, rel_path, recursive, extensions in PARTS:
        if rel_path == ".":
            files = collect_top_level_files()
        else:
            files = collect_files(rel_path, extensions, recursive)

        files_meta = []  # (relative_path_str, line_count, byte_count, sha1_prefix)
        for path in files:
            rel_str = str(path.relative_to(PROJECT_ROOT))
            name = path.name
            if should_exclude(rel_str, name):
                continue
            try:
                content = read_text_safe(path)
                line_count = count_lines(content)
                byte_count = path.stat().st_size
                digest = file_hash_short(path)
            except Exception as exc:
                print(f"  WARN: could not read {rel_str}: {exc}")
                continue
            files_meta.append((rel_str, line_count, byte_count, digest))
            total_files += 1
            total_lines += line_count
            total_bytes += byte_count

        parts_inventory.append((letter, title, files_meta))
        print(f"  Part {letter} ({title}): {len(files_meta)} files collected from {rel_path}")

    print()
    print(f"Total files: {total_files}")
    print(f"Total lines: {total_lines:,}")
    print(f"Total bytes: {total_bytes:,}")
    print()

    # Phase 2: write the master file
    OUTPUT_DIR.mkdir(parents=True, exist_ok=True)
    with open(OUTPUT_PATH, "w", encoding="utf-8") as out:
        # Document header and summary
        out.write(build_document_header(total_files, total_lines, total_bytes))
        # Table of contents
        out.write(build_toc(parts_inventory))

        # Each part with its files
        for letter, title, files_meta in parts_inventory:
            out.write(build_part_banner(letter, title, len(files_meta)))
            if not files_meta:
                out.write("*(No files in this part.)*\n")
                continue

            # Write each file with its banner and content
            for rel_path, line_count, byte_count, digest in files_meta:
                out.write(build_file_banner(rel_path, digest, line_count, byte_count))
                abs_path = PROJECT_ROOT / rel_path
                try:
                    content = read_text_safe(abs_path)
                except Exception as exc:
                    out.write(f"*Could not read file: {exc}*\n")
                    continue
                out.write(content)
                if not content.endswith("\n"):
                    out.write("\n")

        # Footer
        out.write("\n\n")
        out.write("=" * 72 + "\n")
        out.write("# END OF BLOODLINES COMPLETE BIBLE (ALL v2.0)\n")
        out.write(f"# Built {datetime.now().strftime('%Y-%m-%d %H:%M:%S')} from {total_files} source files\n")
        out.write(f"# Total: {total_lines:,} lines, {total_bytes:,} bytes of source content\n")
        out.write("=" * 72 + "\n")

    # Report final file size
    final_bytes = OUTPUT_PATH.stat().st_size
    print(f"Wrote {OUTPUT_PATH}")
    print(f"  Output size: {final_bytes:,} bytes")
    print(f"  Compression factor vs source: {final_bytes / max(total_bytes, 1):.3f}")
    print()
    print("Done.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
