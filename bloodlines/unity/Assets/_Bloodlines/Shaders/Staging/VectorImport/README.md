# Vector Import Shader Support

This folder contains Bloodlines-local shader support for Unity 6.3 concept-sheet rasterization.

Purpose:

- keep the graphics lane independent of the legacy `com.unity.vectorgraphics` package
- allow SVG-authored concept sheets to rasterize into Unity staging without reintroducing the package conflict discovered on 2026-04-16
- support editor-only generation of PNG review boards from staged SVG sheets

Current execution model:

- first choice: headless browser raster export for full-fidelity SVG sheet capture
- fallback: Unity-local mesh rasterization using these staging shaders and the built-in vector module

These shaders are staging and tooling support, not approved runtime art shaders.
