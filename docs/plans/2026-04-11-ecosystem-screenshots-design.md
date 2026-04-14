# Ecosystem Device Showcase: Real Screenshots

## Summary
Replace the three hand-coded inline HTML device simulations in the home page ecosystem section with real browser screenshots of the actual product pages.

## Device Layout

| Slot | Product | Frame Type | Screenshot Source | Main Label | Subtitle |
|---|---|---|---|---|---|
| Left | Home Hub | Phone | `/home-hub/` overview tab | Network Control | Home Hub |
| Center | PIP | Laptop | `/pip/` main view | AI Workstation | Private Intelligence Platform |
| Right | Sovereign Signal | Phone | `/sovereign-signal/` messenger | Encrypted Comms | Sovereign Signal |

## Implementation

1. Capture screenshots at correct aspect ratios (375x812 phones, 1280x800 laptop)
2. Save to `deploy/thumbs/` as `screen-homehub.jpg`, `screen-pip.jpg`, `screen-signal.jpg`
3. Replace inline HTML in each `.device-screen` with `<img>` tag
4. Update device labels (main label + product name subtitle)
5. Update category labels above devices (Home Layer, AI Layer, Secure Layer)
6. Update ecosystem pill links to match new products
7. Verify both dark and light mode
8. Sync deploy to root

## Approach
Pure screenshot swap. CSS already supports `img` inside `.device-screen` with `object-fit: cover`.

## Files Modified
- `deploy/home.html` (ecosystem section)
- `deploy/thumbs/screen-homehub.jpg` (new)
- `deploy/thumbs/screen-pip.jpg` (new)
- `deploy/thumbs/screen-signal.jpg` (new)
