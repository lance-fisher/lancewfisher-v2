# Fisher Sovereign Integration -- Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Add Fisher Sovereign Systems identity section + article sub-page. Both dark/light themes verified.

**Architecture:** New section in index.html between stats strip and About. Article teaser card inside. Full editorial at fisher-sovereign.html. All CSS appended to styles.css using --fg/--gold variables. No build step.

**Design doc:** docs/plans/2026-03-13-fisher-sovereign-integration-design.md

---

## Task 1: Create git restore point

    git tag pre-fisher-sovereign
    git tag | grep pre-fisher-sovereign  # verify

Rollback: git checkout pre-fisher-sovereign -- index.html styles.css sitemap.xml && rm fisher-sovereign.html

---

## Task 2: Append CSS to styles.css

See full CSS block in design doc section "CSS Conventions". Append to end of styles.css.
Key classes: .fs-wordmark, .fs-glyph, .fs-wordmark-name, .fs-wordmark-rule, .fs-wordmark-entity,
.fs-headline, .fs-tagline, .fs-mission, .fs-ctas, .fs-teaser-card, .fs-teaser-info,
.fs-teaser-eyebrow, .fs-teaser-title, .fs-teaser-subtitle, .fs-teaser-quote, .fs-teaser-sublabel,
.fs-teaser-cta, .fs-teaser-deco, .fs-teaser-quote-mark, and all .fs-article-* classes.

Full CSS written in Task 2b below.

---

## Task 3: Add section HTML to index.html

Insertion point: after </div> closing .stats-strip, before existing spacer-lg (around line 138).

Section structure:
- div.section-label "Sovereign"
- div.fs-wordmark (SVG glyph + FISHER SOVEREIGN + SYSTEMS LLC)
- h2.fs-headline "The Modern Internet Was Not Built for You"
- p.fs-tagline "Building the Architecture for Independence"
- div.fs-mission (2 paragraphs)
- div.fs-ctas (The Full Case + See What We Build)
- div.fs-teaser-card (eyebrow + title + subtitle + blockquote + sublabel + CTA + deco)

Full HTML written in Task 3b below.

---

## Task 4: Add nav dot

Find nav dots block near bottom of index.html.
Insert between sec-about and sec-sovereignty dots:
  <div class="nav-dot" data-target="sec-fisher-sovereign"></div>

Commit: git add index.html styles.css && git commit -m "Add Fisher Sovereign section, teaser card, CSS"

---

## Task 5: Create fisher-sovereign.html

Full article sub-page. Loads styles.css (relative path). Theme toggle via lf-theme localStorage.
Structure: nav > header > article > end matter > footer > inline toggle JS.
Three pull quotes distributed through article body.
Ghost line "You are not the customer." behind hero title.

Commit: git add fisher-sovereign.html && git commit -m "Add Architect of Independence article sub-page"

---

## Task 6: Update sitemap.xml

Add before </urlset>:
  <url>
    <loc>https://lancewfisher.com/fisher-sovereign</loc>
    <changefreq>monthly</changefreq>
    <priority>0.8</priority>
  </url>

Commit: git add sitemap.xml && git commit -m "Add fisher-sovereign to sitemap"

---

## Task 7: Final verification

- Both pages: theme toggle switches dark/light, persists across pages via localStorage
- Mobile: teaser card collapses, article readable at 375px
- Check deploy/ sync: diff styles.css deploy/styles.css (note diff, do NOT run build-deploy.sh unless instructed)

---

## Rollback

    git checkout pre-fisher-sovereign -- index.html styles.css sitemap.xml
    rm fisher-sovereign.html
