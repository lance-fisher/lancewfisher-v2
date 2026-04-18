# Bloodlines Owner Direction - 2026-04-17

Owner: Lance Fisher
Supersedes: None. This file amends (does not replace) `governance/OWNER_DIRECTION_2026-04-16_FULL_CANON_UNITY.md` by clamping fidelity and naming the product quality metric.

This file records an active non-negotiable project direction for Bloodlines. It is additive governance. It supersedes any older prompt, handoff, roadmap, plan, recommendation, or canonical phrasing that implies AAA-fidelity art, PBR material authoring, HDRP rendering, cinematic animation, or graphical spectacle as a shipping requirement.

## Primary Product Quality Metric

Bloodlines succeeds on strategy depth, balance, replayability, and the availability of multiple viable playstyles. These are the shipping quality gates. Graphics, audio, VFX, and presentation serve these gates; they do not replace them.

Specifically:

- **Strategy depth.** Every system in the canon ships with its full mechanical interactions intact. No system is simplified for the sake of time. No mechanical interaction is cut because it is complex. The game must reward deep study, deep planning, and reading the opponent. Shallow play must lose to deep play, predictably.
- **Balance.** All nine founding houses, all four faiths, all doctrine paths, all unit roles, all build paths, all faith tiers, all conviction bands, and all victory paths are independently viable and mutually resolvable. No house is a joke. No victory path is a trap. No faith is a trap. No doctrine is a trap.
- **Replayability.** Canonical match structure supports distinct narratives across matches. Dynasty members, faith tiers, conviction bands, realm conditions, diplomacy events, and late-game arcs (Trueborn Rise, Great Reckoning, divine right declarations) combine to make no two matches alike.
- **Multiple playstyles.** The canon supports many viable paths to win. The authoritative list is `data/victory-conditions.json`, which currently enumerates five canonical victory paths (Military Conquest, Economic Dominance, Faith Divine Right, Territorial Governance, Alliance Victory) plus a non-victory dynastic prestige modifier, and may grow. Examples of strategic flavors the canon supports, non-exhaustively: pure offense / siege-focused pressure, pure defense / fortification-turtle, faith-aligned divine right conversion, economic dominance via trade and granary cap, territorial governance via settlement recognition, alliance-based power projection, dynasty prestige maximization, covert operations and sabotage dominance, mixed strategies that blend any of the above. The list is non-exhaustive. The canon and the five-stage match structure are the authoritative source of what paths are viable; new viable paths may emerge from mechanical interactions and must be preserved when observed.

If a design proposal reduces mechanical depth, compresses the victory-path set, forces players onto a single preferred playstyle, or trades gameplay systems for visual content, reject it.

## Graphics Fidelity Ceiling (Non-Negotiable)

The graphics fidelity ceiling is Command and Conquer Generals Zero Hour (2003) fused with Warcraft III (2002). Matching that era's polish is the goal. Exceeding it is out of scope.

"Full commercial polish" in prior owner direction refers to this era's commercial polish, not AAA 2020s fidelity. Age of Empires II Definitive, Warcraft III Reforged (base art, not remastered hero-unit art), Stronghold Crusader, and the Total War series pre-Attila are the visual benchmarks.

Concrete ceiling:

- URP Forward rendering only. No HDRP. No ray tracing. No volumetric effects.
- Diffuse + optional simple specular shaders. No PBR. No metalness / roughness / height / AO maps. Faction tint via per-instance color. Rim light for silhouette legibility. Selection highlight.
- Textures 256 to 1024 pixels, hand-painted style. No normal maps except where a specific canonical unit or building requires one for silhouette.
- Meshes 1k to 15k triangles. Silhouette distinction at RTS zoom is the quality metric.
- Skeletal animation with simple bone rigs (~10-30 bones). No motion capture. No facial animation. No advanced IK.
- Basic particle VFX. No GPU particle systems. No soft particles. No complex particle physics.
- Single directional sun + ambient. Cascaded shadow map at two levels. Optional point lights for torches and fire.
- Tile-based or simple heightmap terrain with splatted textures.
- UI: information-dense, clean, readable. Age of Empires / Warcraft III / Zero Hour density.

Out of scope autonomously:

- Final faction-identity art (art direction decisions stay human-owned even within this ceiling).
- PBR material authoring.
- Commercial texture pack integration designed for AAA pipelines.
- HDRP migration.
- Ray tracing adoption.
- Motion capture pipelines.
- Cinematic cutscene systems beyond simple in-engine fly-through.
- AAA animation blending networks (Animation Rigging, Kinematica, etc).
- Procedural material networks deeper than the current faction-tint shader already requires.

If an agent or contributor proposes work that exceeds this ceiling, the proposal is declined. The Graphics Fidelity Ceiling is not a placeholder for a future AAA pass. It is the shipping target.

## Audio Fidelity Ceiling

Same era as graphics. Wwise integration is in scope and canonical. Sound design fidelity matches early-2000s RTS sound: clear unit voice-overs with distinct faction accents, clean SFX for combat and economy, faction-distinct music per phase of the match. No AAA positional audio simulation. No occlusion ray-tracing. No procedural music generation.

## UX and Polish Bounded to the Era

Onboarding, tutorials, campaign, lobby, settings, HUD, and in-game panels all match the era's UX standards. Information density over visual spectacle. Every piece of information the player needs to play the canon at depth is surfaced through the UI. No reliance on third-party tools or external wikis to understand canonical mechanics.

## Interaction With Prior Owner Direction

The 2026-04-16 owner direction still applies:

- Unity 6.3 LTS with DOTS / ECS is the shipping engine.
- The browser runtime is frozen as behavioral specification.
- `unity/` is the only active Unity work target.
- Wwise audio and Netcode for Entities multiplayer remain in scope unless explicitly removed later.
- No MVP framing. No phased gameplay release. No scope-cutting of gameplay.

Where the 2026-04-16 direction uses the phrase "full commercial polish" or "full commercial-quality art," interpret it as "full polish for the Zero Hour / Warcraft III era fidelity ceiling defined in this document." Gameplay scope remains fully canonical; presentation scope is clamped.

## Authoritative References

- Victory paths canonical list: `data/victory-conditions.json`.
- Match structure: `02_SESSION_INGESTIONS/SESSION_2026-04-15_match-structure-time-system-multiplayer-doctrine*.md`.
- Canonical design bible: `18_EXPORTS/BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.4.md`.
- Continuity prompt enforcing this direction: `03_PROMPTS/BLOODLINES_UNITY_CONTINUITY_PROMPT_v3.md`.
- Prior owner direction (still active, interpreted through this amendment): `governance/OWNER_DIRECTION_2026-04-16_FULL_CANON_UNITY.md`.
