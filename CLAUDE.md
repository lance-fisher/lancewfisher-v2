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

## Design Notes
- Dark aesthetic with gold accents (CSS custom properties)
- Sections: Hero, About, Now, Ventures, Projects, One-Three brand, Contact
- Responsive with mobile nav toggle
- Multiple sub-sites served from subdirectories
