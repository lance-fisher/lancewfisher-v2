# Concept Sheet Rasterized

This folder holds Unity-generated PNG mirrors of the canonical graphics-lane SVG concept sheets.

Rules:

- The raw `.svg` sheets in `Assets/_Bloodlines/Art/Staging/ConceptSheets/` remain the source of truth inside Unity staging.
- Do not hand-edit the `.png` files in this folder.
- Refresh this folder through:
  - `Bloodlines -> Graphics -> Sync And Rasterize Concept Sheets`
  - `Bloodlines -> Graphics -> Rasterize Concept Sheets`
- The preferred renderer is headless local browser export for full SVG fidelity, including text and layout.
- If browser export is unavailable, the tooling falls back to Unity mesh rasterization through the built-in vector module.
- Keep all generated review boards in staging until a later approval step promotes any derived asset into runtime-facing folders.
