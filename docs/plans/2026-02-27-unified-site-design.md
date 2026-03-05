# Unified Site Design — lancewfisher-v2 + one-three-net consolidation

**Date**: 2026-02-27
**Branch**: `feature/unified-site-v3`
**Status**: Approved, in progress

## Goal

Consolidate the best structural elements from one-three-net into the lancewfisher-v2 site to create a stronger personal brand page with clear revenue-generating CTAs. The Fisher One-Three shirt designs remain a separate page/project.

## Isolation Strategy

- All work happens on `feature/unified-site-v3` branch
- `master` branch is never touched
- one-three-net source project is never touched
- Local preview on `:8073` before anything ships
- User gives explicit "launch" command before merge

## What Gets Added (from one-three-net)

1. **Stats bar** — 4-stat credibility strip below hero (80k+ lines / 20+ projects / 6 languages / 15+ years leading)
2. **"Now" section** — Pulsing-dot current-work list showing active projects
3. **Ventures card format** — 3-card grid (NoCo / Fisher One-Three / Trading) with stat chips replacing the current ventures logos
4. **Project category filters** — Filter buttons (All / Apps / Trading / Games / Tools & AI) + show/hide toggle
5. **NoCo App Studio CTA** — Dedicated section with $2,500 setup / $199/mo pricing
6. **Fisher One-Three brand teaser** — Philosophy quote + stats + "See Collection" / "Shop on Etsy" buttons

## What Gets Added (new)

7. **Booking CTA** — "Book a Call" button using Calendly or Cal.com link (placeholder until booking URL ready)

## What Stays Separate

- **brand.html** (24-shirt gallery) — Lives as its own page at `/brand` or `/brand.html`, linked from the Ventures card and brand teaser
- Fisher One-Three is treated as a project in the portfolio, not merged content

## What Stays Unchanged

- FISHER typography hero
- FTDA / Playbook / Resume passkey-gated sections
- Architecture diagram
- Principles / Sovereignty sections
- All meta/SEO/structured data
- All existing sub-pages (harmony/, dashboard/, jumpquest/, noco/, concepts/)

## Bug Fixes Included

- `brand.html` 404 — create route or redirect
- `ftda/` 403 — ensure index.html loads
- `lancewfisher.com/lander` 403 — fix or remove redirect

## Design Aesthetic

- Matches existing lancewfisher-v2 dark theme (--bg: #0a0a0b, --warm-white, --gold-1776)
- Uses existing font stack (Cormorant Garamond, Inter)
- New sections styled inline to match surrounding code patterns
- No new CSS files or build steps
