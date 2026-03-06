# Light Theme Redesign: "Stone & Brass"

**Date:** 2026-03-05
**Branch:** `integration/master-merge`
**Status:** Approved

---

## Problem

The current light theme is washed out and nearly unreadable:
- Body text uses opacity tricks (50%) on a similar-toned background, making it invisible
- Gold accents were replaced with dead muted browns
- Hero has an awkward white rectangle from the script logo image
- Roman statue watermark at 0.15 opacity is completely invisible
- Overall brightness too high — feels bleached, not elegant

## Design Direction

**Stone & Brass** — Keep the warm stone palette, fix all contrast, add life with brass accents and a visible statue watermark. The light theme should feel like aged parchment with engraved illustrations — intentional, warm, lived-in.

## Color System

| Token | Current (broken) | New |
|-------|------------------|-----|
| `--bg` | `#e8e4df` | `#ddd8d2` (darker warm stone) |
| `--warm-white` (primary text) | `#2c2a28` at 50% opacity | `#1c1a18` full opacity |
| `--warm-white-dim` (secondary) | `rgba(44,42,40,0.50)` | `rgba(28,26,24,0.6)` |
| `--warm-white-ghost` | `rgba(44,42,40,0.15)` | `rgba(28,26,24,0.2)` |
| `--accent` | `rgba(44,42,40,0.45)` | `rgba(28,26,24,0.65)` |
| `--gold-1776` | `#6b6560` (dead brown) | `#8b7355` (warm brass) |
| `--gold-dim` | `rgba(90,82,74,0.25)` | `rgba(139,115,85,0.35)` |
| `--gold-ghost` | `rgba(90,82,74,0.06)` | `rgba(139,115,85,0.1)` |

**Principle:** No opacity tricks for text contrast. Use full-opacity dark colors with proper WCAG AA contrast ratios.

## Hero Section

- **Script logo**: `mix-blend-mode: multiply` + `filter: sepia(0.3) contrast(1.2) brightness(0.85)`. Opacity 0.6-0.7 (up from 0.45). Blends into stone as deliberate watermark.
- **Remove white rectangle**: Image container `background: transparent`.
- **LANCE F1SH3R lettermark**: `#1c1a18` with subtle brass-tinted text-shadow.
- **Numerals "1" and "3"**: `#8b7355` (brass).
- **Motto**: `rgba(28,26,24,0.5)` — visible but softer.

## Roman Statue Watermark

- **Opacity**: 0.35-0.45 (up from 0.15) — a real design feature like the ship on dark
- **CSS filter**: `sepia(0.2) contrast(0.9)` for brass/aged tint
- **Positioning**: Large, centered — engraved illustration on old paper
- **Interaction with text**: Text reads clearly over it due to dark text on lighter watermark

## Cards & Sections

- **Background**: `rgba(255,255,255,0.5)` (more opaque than current 0.35)
- **Border**: `1px solid rgba(139,115,85,0.18)` (brass-tinted, visible)
- **Shadow**: `0 2px 16px rgba(28,26,24,0.06)` (soft, warm)
- **Hover**: Border `rgba(139,115,85,0.35)`, shadow deepens, brass glow

## Creative Life Elements

- **Brass section dividers**: Thin `1px solid rgba(139,115,85,0.3)` between major sections
- **Stone texture**: Faint noise/grain CSS overlay for tactile depth
- **Section labels** (METHOD, NOW, etc.): Brass color `#8b7355` instead of faded gray
- **Gold flourish on hovers**: Card borders warm to brass glow

## Project Tiles

- Dark thumbnails stay dark (actual screenshots)
- Tile text: Full opacity dark
- Status badges: Brass border + brass text
- Tech stack: `rgba(28,26,24,0.55)` (secondary but readable)

## Concept Gallery

- Thumbnails stay dark
- Carousel arrows/scrollbar: Brass-tinted
- Labels: Full opacity text

## Navigation (scrolled)

- Background: `rgba(221,216,210,0.96)` with backdrop-blur (matches new `--bg`)
- Links: `#1c1a18`, hover: `#8b7355`

## Buttons

- Primary: `background: #4a4540`, `color: #e8e4df` (keep)
- Hover: Shift to `#8b7355` (brass)
- Theme toggle: Brass-tinted sun icon

## Additional Fix

- **Resume gate passkey**: Input seems functional (tested programmatically) but may have browser input issues. Will investigate and harden the input handling.

## Scope

- `styles.css` — rewrite all `[data-theme="light"]` overrides (~240 lines)
- `index.html` — possibly adjust statue SVG markup or inline styles
- `resume.html` — harden passkey input if needed
- No structural HTML changes expected
