# lancewfisher-v2

Personal portfolio and brand site for Lance W. Fisher. Static HTML/CSS/JS, no build step.

## Stack
- **HTML/CSS/JS** — vanilla, no frameworks or bundlers
- **Fonts**: Google Fonts (Cormorant Garamond, Inter, JetBrains Mono)
- **Domain**: lancewfisher.com (canonical), one-three.net (brand variant)
- **Deploy**: Static files in `deploy/` directory

## How to Run
```
# Serve deploy/ for live preview (launch.json configured for port 8077):
python -m http.server 8077 --directory deploy

# Or serve root for development:
python -m http.server 8070 --directory .
```

## Source vs Deploy

**`deploy/` is the website and the authoritative source of truth for all content.** Operational rules for sync direction, dual-mode verification, and CSS conventions are in `.claude/rules/portfolio-website.md` and load automatically during website sessions.

## Bloodlines Content Rule

Bloodlines uses `D:/ProjectsHome/FisherSovereign/lancewfisher-v2/bloodlines/` as the main working root.

- `D:/ProjectsHome/Bloodlines/` is a compatibility alias to that root.
- `deploy/bloodlines/` is no longer a separate Bloodlines project root and should not be treated as one.
- Future Bloodlines canon, docs, tooling, browser runtime work, and Unity work should happen in `bloodlines/`.
- If Bloodlines content must be exposed under `deploy/`, treat that as a compatibility surface, not as the source of truth.

## Structure
```
index.html              — Main portfolio page (lancewfisher.com)
resume.html             — Resume page
one-pager.html          — One-page summary
404.html                — Custom 404 page
styles.css              — Extracted main CSS (from index.html)
main.js                 — Extracted main JS (from index.html)
concepts/               — Source concept HTML files (12 themes)
noco/                   — NoCo businesses directory (app.js, businesses.js, styles.css)
harmony/                — Serenity Medspa Demo embedded sub-page
dashboard/              — Dashboard sub-page
jumpquest/              — Jump Quest game sub-page
thumbs/                 — Thumbnail images
_originals/             — Original high-res assets
```
