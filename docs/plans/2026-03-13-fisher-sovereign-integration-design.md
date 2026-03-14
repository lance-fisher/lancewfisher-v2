# Fisher Sovereign Integration — Design Document
**Date:** 2026-03-13  
**Status:** Approved  
**Branch:** integration/master-merge

---

## Overview

Integrate Fisher Sovereign Systems, LLC as a prominent identity on the portfolio site alongside the personal brand. Add the article "Architect of Independence" as a featured, dedicated editorial sub-page. Preserve and verify the dark/light theme toggle. Create a git restore point before any changes.

---

## Goals

1. Feature Fisher Sovereign Systems as a business identity — not buried, not an afterthought
2. Present "Building the Architecture for Independence" as the company tagline with weight
3. Feature the article in a way that mirrors the FTDA treatment — elevated, not generic
4. Create a standalone editorial sub-page for the full article
5. Ensure both dark and light themes work throughout
6. Create a restore point before touching anything

---

## Language System (Locked)

| Element | Text |
|---|---|
| Section label | Sovereign |
| Section headline | The Modern Internet Was Not Built for You |
| Tagline | Building the Architecture for Independence |
| Mission para 1 | It was built around you. To observe, categorize, and convert your behavior into revenue. Every search, every click, every pause is a data point inside a system you did not design, cannot inspect, and rarely benefit from. You were never the customer. You were the inventory. |
| Mission para 2 | Fisher Sovereign Systems is the refusal. Encrypted. Local. Sovereign. Infrastructure engineered to answer to the person using it — not the company profiting from it. |
| Primary section CTA | The Full Case — links to fisher-sovereign.html |
| Secondary section CTA | See What We Build — anchors to #sec-projects |
| Teaser card eyebrow | Required Reading |
| Teaser card pull quote | People did not sign up to become datasets. They signed up to use services. The rest happened quietly behind the scenes. |
| Teaser card sub-label | The case for why this company exists — and the problem it was built to solve. |
| Teaser card CTA | Read the Brief — links to fisher-sovereign.html |
| Article sub-page ghost line | You are not the customer. |

---

## Architecture

### Files Modified
- index.html — new #sec-fisher-sovereign section, article teaser card, new nav dot
- styles.css — new section styles (both themes, --fg variable throughout)

### Files Created
- fisher-sovereign.html — standalone editorial article sub-page
- sitemap.xml update — add new page entry

### Restore Point
- git tag pre-fisher-sovereign created before any file is touched

---

## Main Page Changes

### New Section: #sec-fisher-sovereign

Position: Between the stats strip and the About section (after line ~138, before the spacer/thin-line leading to About).

Structure:
  section.section #sec-fisher-sovereign [data-reveal]
    div.section-label          "Sovereign"
    div.fs-wordmark            SVG glyph + "FISHER SOVEREIGN" + "SYSTEMS, LLC"
    div.fs-headline            "The Modern Internet Was Not Built for You"
    div.fs-tagline             "Building the Architecture for Independence"
    div.fs-mission             two mission paragraphs
    div.fs-ctas                "The Full Case" + "See What We Build"
    div.fs-teaser-card         article teaser card

### Logo placeholder SVG spec
- Minimal architectural mark: two vertical lines connected by a single horizontal crossbar
- "FISHER SOVEREIGN" in Cormorant Garamond, letter-spacing 0.25em, ~28-32px
- Thin 1px hairline rule below
- "SYSTEMS, LLC" in Inter, letter-spacing 0.3em, ~11px, 45% opacity
- SVG glyph sits left of text block
- Gold in dark mode, deep ink with gold accent in light mode
- Entire wordmark is a single div.fs-wordmark — swap SVG when real logo arrives

### Article Teaser Card spec
- Layout: info column left + large faded quotation mark right (~15% opacity, ~200px)
- Eyebrow: "Required Reading" in gold small-caps label style
- Title: "Architect of Independence" in Cormorant Garamond ~28px
- Sub-title: "Lance Fisher and the Quiet Rebellion Against the Surveillance Economy"
- Sub-label: "The case for why this company exists — and the problem it was built to solve."
- Pull quote: italic Cormorant, left gold border
- CTA: "Read the Brief" ghost button style

### Nav dot
Add sec-fisher-sovereign after sec-about in the nav dot strip (index.html lines ~1447-1457).

---

## Article Sub-Page: fisher-sovereign.html

### Purpose
Standalone editorial landing. Can be shared directly. Both themes. Minimal navigation.

### Structure
  head                       own title/meta, loads ../styles.css + ../fonts/fonts.css
  theme toggle (same pill)
  nav.fs-article-nav         wordmark left | back link right
  section.fs-article-hero    ghost line + title + byline
  article.fs-article-body    full article all sections
  section.fs-article-end     Fisher Sovereign identity block + CTAs
  footer                     thin copyright bar

### Hero detail
- Ghost line: "You are not the customer." — Cormorant ~72-96px, opacity ~10-12%, behind title
- Title: "Architect of Independence" — Cormorant display ~56-64px
- Sub-title: Lance Fisher and the Quiet Rebellion Against the Surveillance Economy
- Byline: By Editorial Staff · A Fisher Sovereign Publication
- Thin gold rule separating hero from body

### Body typography
- Body: Inter, ~17px, line-height 1.75, max-width 680px centered
- Section headers: Cormorant Garamond, letter-spacing 0.15em, ~22px
- Pull quotes (3 distributed):
  1. "People did not sign up to become datasets. They signed up to use services." — early
  2. "The safest data is data that was never centralized in the first place." — mid
  3. "This is one of the points Fisher returns to with unusual force — because it happened to him." — late
- Pull quote style: 2px left border in --gold, italic Cormorant ~22px, 8% bg tint

### End matter
- Fisher Sovereign wordmark (smaller)
- Tagline
- One sentence: "Fisher Sovereign Systems is being built to return control to the individual."
- CTAs: "Visit lancewfisher.com" (primary) + "Contact" (ghost, links to main page contact)

---

## CSS Conventions

- No hardcoded rgba(240, 235, 224, ...) — use rgba(var(--fg), 0.XX)
- Alpha: 0.85+ primary, 0.65-0.75 secondary, 0.45-0.55 muted
- Every new rule verified in both dark and light
- New classes prefixed fs- or fs-article- to avoid collision

---

## Theme Toggle Fix

Toggle code is correct in source. After implementation verify deploy/ is in sync:
  diff styles.css deploy/styles.css

---

## Restore / Rollback

  git checkout pre-fisher-sovereign    # full restore
  git diff pre-fisher-sovereign        # see what changed
