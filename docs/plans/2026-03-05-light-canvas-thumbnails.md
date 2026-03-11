# Light Theme Canvas Thumbnails — Design Doc

**Date**: 2026-03-05
**Status**: Approved

## Problem

All 10 canvas-rendered project thumbnails have hardcoded dark backgrounds. When the site is in light theme, dark thumbnails look out of place against the silver background.

## Approach: Hybrid Palette Swap

Swap background/chrome colors per theme while preserving semantic data colors (green=profit, red=loss, status badges).

### Palette

| Token | Dark | Light |
|-------|------|-------|
| bg gradient | `#0d0d0e → #080809` | `#e8e9eb → #dddee0` |
| panel fill | `rgba(accent, 0.025)` | `rgba(accent, 0.06)` |
| panel border | `rgba(accent, 0.05)` | `rgba(accent, 0.12)` |
| primary text | `rgba(240,235,224, X)` | `rgba(26,26,28, X)` |
| dim text | `rgba(240,235,224, 0.45)` | `rgba(26,26,28, 0.55)` |
| header text | `rgba(240,235,224, 0.75)` | `rgba(26,26,28, 0.85)` |
| accent label | `rgba(accent, 0.35)` | `rgba(accent, 0.5)` |
| corners | `rgba(accent, 0.15)` | `rgba(accent, 0.2)` |
| phone/device bg | `#1a1a1c` | `#f5f5f7` |

Semantic colors (unchanged): `#4ade80` (green), `#f87171` (red), `#fbbf24` (yellow), `#64748b` (slate).

### Implementation

1. Add `getCanvasTheme()` helper → reads `data-theme` attribute
2. Add `getCanvasPalette(accentR, accentG, accentB)` → returns color object
3. Modify `drawBg()` to accept palette and draw light/dark gradient
4. Update each of 10 canvas renderers to use palette tokens
5. Hook theme toggle to call `initProjectCanvases()` for re-render
6. Add CSS filter adjustments for 5 static image thumbnails
7. Update FTDA canvas (ftda/ftda.js) separately

### Static Images (CSS-only)

For the 5 `<img>` thumbnails, add light-theme CSS to slightly brighten:
```css
[data-theme="light"] .project-thumb-wrap img { filter: brightness(1.1) contrast(1.05); }
```

### Dynamic Toggle

In `setTheme()`, after setting/removing `data-theme`, call `initProjectCanvases()` and re-init FTDA canvas.
