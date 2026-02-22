# lancewfisher-v2

Personal portfolio and brand site for Lance W. Fisher. Static HTML/CSS/JS, no build step.

## Stack
- **HTML/CSS/JS** — vanilla, no frameworks or bundlers
- **Fonts**: Google Fonts (Cormorant Garamond, Inter, JetBrains Mono)
- **Domain**: lancewfisher.com (canonical), one-three.net (brand variant)
- **Deploy**: Static files in `deploy/` directory

## How to Run
```
python -m http.server 8070 --directory .
```
Then open http://localhost:8070

## Structure
```
index.html              — Main portfolio page (lancewfisher.com)
resume.html             — Resume page
one-pager.html          — One-page summary
404.html                — Custom 404 page
deploy/                 — Production-ready copy of all assets
  brand/                — Fisher One-Three brand variant (one-three.net)
    index.html          — Brand landing page
    style.css           — Dark gold aesthetic
    brand.css           — Brand-specific styles
  noco/                 — NoCo businesses directory sub-app
  concepts/             — 12 concept/design explorations
  ftda/                 — FTDA sub-app (config.js, ftda.js, ftda.css)
concepts/               — Source concept HTML files (12 themes)
noco/                   — NoCo businesses directory (app.js, businesses.js, styles.css)
harmony/                — Harmony Medspa embedded sub-page
dashboard/              — Dashboard sub-page
jumpquest/              — Jump Quest game sub-page
thumbs/                 — Thumbnail images
_originals/             — Original high-res assets
```

## Design Notes
- Dark aesthetic with gold accents (CSS custom properties)
- Sections: Hero, About, Now, Ventures, Projects, One-Three brand, Contact
- Responsive with mobile nav toggle
- Multiple sub-sites served from subdirectories
