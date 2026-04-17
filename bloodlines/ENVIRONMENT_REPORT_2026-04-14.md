# ENVIRONMENT_REPORT — 2026-04-14

Dated audit of the local machine state, installed toolchain, Bloodlines project layout, package alignment, and readiness for Unity 6.3 LTS + DOTS/ECS production implementation.

---

## 1. Machine Discovery

### Host

- OS: Windows 11 Pro 10.0.26200
- User: lance
- Canonical project path: `D:\ProjectsHome\Bloodlines` → junction to `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\bloodlines`

### Engine Toolchain

| Tool | Status | Path | Version |
|---|---|---|---|
| Unity Hub | INSTALLED | `C:\Program Files\Unity Hub\Unity Hub.exe` | present |
| Unity Editor 6000.3.13f1 (Unity 6.3 LTS) | INSTALLED | `C:\Program Files\Unity\Hub\Editor\6000.3.13f1` | APPROVED VERSION |
| Unity Editor 6000.4.2f1 (Unity 6.4) | INSTALLED | `C:\Program Files\Unity\Hub\Editor\6000.4.2f1` | newer, currently active for `unity/` project |

### IDE

| Tool | Status | Path | Notes |
|---|---|---|---|
| Visual Studio (18 / Community) | INSTALLED | `C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\devenv.exe` | Newer than VS 2022; VS 2022 directory exists in `(x86)` path as legacy artifact. For Unity integration purposes `com.unity.ide.visualstudio 2.0.27` will handle it. |
| Visual Studio 2022 (v17) | NOT FOUND at standard full-install path | — | Only remnant directory at `C:\Program Files (x86)\Microsoft Visual Studio\2022\` (no `devenv.exe`). The approved target was VS 2022; the installed VS is newer (18). |
| JetBrains Rider | NOT INSTALLED | — | Not required; `com.unity.ide.rider 3.0.39` already in manifest if it's ever added. |
| VS Code | INSTALLED | `C:\Users\lance\AppData\Local\Programs\Microsoft VS Code\` | Usable for editing; not preferred for Unity C# debugging. |
| .NET SDK | INSTALLED | `C:\Program Files\dotnet\sdk\10.0.201` | Modern .NET present; runtimes 6/7/8/10 all installed. |

### 3D / Content Tools

| Tool | Status | Path |
|---|---|---|
| Blender | INSTALLED | `C:\Program Files\Blender Foundation\Blender 5.1` |

### Version Control

| Tool | Status | Path | Version |
|---|---|---|---|
| Git | INSTALLED | `C:\Program Files\Git` | 2.46.0.windows.1 |
| GitHub Desktop | INSTALLED | `C:\Users\lance\AppData\Local\GitHubDesktop\` | present |
| Perforce / Helix | NOT INSTALLED | — | Not required for current stage; Git is adequate. |

### Audio / Future Middleware

| Tool | Status | Path | Notes |
|---|---|---|---|
| Wwise | NOT INSTALLED | — | Reserved. `Assets/_Bloodlines/Audio/Middleware/` structure created for future integration. |

---

## 2. Project Discovery

### Two Unity-Like Folders Found

| Path | Role | Manifest Contents | Unity Target | Verdict |
|---|---|---|---|---|
| `<repo>/unity/` | **TRUE canonical Unity project** | Entities 6.4.0, Burst 1.8.29, Collections 6.4.0, Mathematics 1.3.3, Entities.Graphics 6.4.0, URP 17.4.0, Input System 1.19.0, Addressables 2.9.1, Timeline 1.8.12, Test Framework 1.6.0, UGUI 2.0.0, VS/Rider IDE packages, Development feature | 6000.4.2f1 | AUTHORITATIVE |
| `<repo>/Bloodlines/` | Fresh Unity 6.4 URP 3D template stub | Default URP/2D packages only — NO Entities/Burst/Collections/Mathematics | 6000.4.2f1 | NON-CANONICAL STUB — preserved, labeled with `STUB_TEMPLATE_NOTICE.md` |

### Existing Content in `<repo>/unity/`

**Already present (partial but solid):**

- `Assets/_Bloodlines/Code/Definitions/BloodlinesDefinitions.cs` — ScriptableObject definitions for Houses, Resources, Units, Buildings, Faiths, ConvictionStates, BloodlineRoles, BloodlinePaths, VictoryConditions, Maps. Well-structured namespace `Bloodlines.DataDefinitions`.
- `Assets/_Bloodlines/Code/Editor/JsonContentImporter.cs` — editor tool that reads `<repo>/data/*.json` and generates the corresponding ScriptableObject assets under `Assets/_Bloodlines/Data/*`. Invokable via Unity menu **Bloodlines → Import → Sync JSON Content**.
- URP project-level assets at `Assets/DefaultVolumeProfile.asset`, `Assets/UniversalRenderPipelineGlobalSettings.asset`.
- 7 baseline asset folders: `Art/`, `Audio/`, `Code/`, `Data/`, `Docs/`, `Prefabs/`, `Scenes/` — all empty or near-empty.
- Solution file: `unity.slnx`.
- Generated csproj files: `Assembly-CSharp.csproj`, `Assembly-CSharp-Editor.csproj`.
- `README.md` that previously pointed at the stale `deploy\bloodlines\` canonical path (now updated).

**NOT yet present:**

- No scenes in `Assets/_Bloodlines/Scenes/`
- No prefabs in `Assets/_Bloodlines/Prefabs/`
- No generated ScriptableObject `.asset` files in `Assets/_Bloodlines/Data/*` (the importer has not been run; they are generated on demand)
- No ECS Components, Systems, Aspects, Authoring, Baking code
- No AI, Pathing, Economy, Population, Faith, Dynasty, Combat, Construction, Camera, Input, Networking, UI, SaveLoad code in the C# tree

### Reference Simulation (Preserved)

The browser prototype at `<repo>/play.html` + `<repo>/src/game/` + `<repo>/data/` is the **reference simulation**. Every canon system has a working JS implementation. As of 2026-04-14 the browser prototype has:

- Dynasty consequence cascade (live)
- Commander aura + capture/succession (live)
- Territory capture + stabilization + governor (live)
- Faith exposure + doctrine effects (live)
- Conviction scoring (tracked)
- **Stone + iron resources with smelting chain** (live as of today's wave)
- **Fortification buildings (wall, tower, gatehouse, keep) with structural damage multipliers** (live)
- **First siege unit (Battering Ram) with anti-structural and anti-unit multipliers** (live)
- **Settlement class + fortification tier metadata** (live)
- **Canonical 90-second realm condition cycle with famine + water crisis + cap pressure** (live)
- **Assault cohesion strain (wave-spam denial)** (live)
- `getRealmConditionSnapshot` helper (live, ready for UI surfacing in next slice)

---

## 3. Structure Changes Made

### Created Full `Assets/_Bloodlines/` Approved Tree

Under `<repo>/unity/Assets/_Bloodlines/`, created the approved baseline subdirectory structure:

- `Art/` → Characters, Units, Buildings, Environment, Terrain, FX, UI, Icons, Concepts
- `Audio/` → Music, SFX, Voice, Middleware
- `Code/` → Components (existing), Systems (existing), Definitions (existing), Editor (existing) + new: Aspects, Authoring, Baking, AI, Pathing, Economy, Population, Faith, Dynasties, Combat, Construction, Camera, Input, Networking, UI, SaveLoad, Utilities, Debug
- `Data/` → ScriptableDefinitions, FactionDefinitions, UnitDefinitions, BuildingDefinitions, TerrainDefinitions, TechDefinitions, AudioDefinitions, DynastyDefinitions, FaithDefinitions, ResourceDefinitions, ConvictionDefinitions, BloodlineRoleDefinitions, BloodlinePathDefinitions, VictoryConditionDefinitions, MapDefinitions, SettlementClassDefinitions, RealmConditionDefinitions
- `Prefabs/` → Units, Buildings, Environment, UI
- `Scenes/` → Bootstrap, Sandbox, Gameplay, Testbeds, Strategic
- `Materials/`, `Shaders/`, `Animation/`
- `Docs/` → Technical, Gameplay, Systems, Decisions, UIUX, Continuity

### Documentation Seeded

- `<repo>/unity/Assets/_Bloodlines/README.md` — master contract for the asset tree
- `<repo>/unity/Assets/_Bloodlines/Code/README.md` — per-folder code responsibilities + namespace conventions
- `<repo>/unity/Assets/_Bloodlines/Data/README.md` — JSON sync direction + per-folder data sources

### Unity Root README Updated

`<repo>/unity/README.md` rewritten to reflect:
- Canonical root governance (`D:\ProjectsHome\Bloodlines` via junction)
- Approved toolchain versions
- Approved architecture (DOTS/ECS/URP/Input System)
- Reference simulation location
- Non-negotiable direction-of-play (Generals/Zero Hour feel + theatre zoom + bloodline UI)

Old README pointed at `deploy\bloodlines\` as canonical; that statement is preserved historically but superseded.

### ScriptableObject Definitions Extended

`<repo>/unity/Assets/_Bloodlines/Code/Definitions/BloodlinesDefinitions.cs` extended to cover:

- **UnitDefinition:** `siegeClass`, `structuralDamageMultiplier`, `antiUnitDamageMultiplier` fields (siege canon)
- **BuildingDefinition:** `fortificationRole`, `fortificationTierContribution`, `structuralDamageMultiplier`, `armor`, `blocksPassage`, `sightBonusRadius`, `auraAttackMultiplier`, `auraRadius`, `smeltingFuelResource`, `smeltingFuelRatio` fields (fortification + smelting canon)
- **ControlPointData:** `settlementClass` field
- **SettlementSeedData:** new class for map.settlements entries
- **MapDefinition:** new `settlements` array
- **New definitions:** `SettlementClassDefinition`, `RealmConditionDefinition` (+ supporting `RealmConditionThresholdsData`, `RealmConditionFamineData`, `RealmConditionWaterCrisisData`, `RealmConditionCapPressureData`, `RealmConditionEffectsData`, `RealmConditionLegibilityData`)

### JSON Importer Extended

`<repo>/unity/Assets/_Bloodlines/Code/Editor/JsonContentImporter.cs` extended to:

- Import `settlement-classes.json` → `SettlementClassDefinitions/*.asset`
- Import `realm-conditions.json` → `RealmConditionDefinitions/realm_conditions.asset`
- Populate the new fortification/siege/smelting/settlement fields on Unit/Building/ControlPoint/Map definitions

### Stub Folder Preserved With Notice

`<repo>/Bloodlines/STUB_TEMPLATE_NOTICE.md` created. The stub URP template folder is preserved intact (not deleted) per the preservation mandate, but clearly labeled with identifying context, evidence, and instructions to avoid accidentally treating it as canonical.

---

## 4. Package / Engine Status

### Installed in `<repo>/unity/Packages/manifest.json`

- com.unity.addressables 2.9.1
- com.unity.burst 1.8.29
- com.unity.collections 6.4.0
- com.unity.entities 6.4.0
- com.unity.entities.graphics 6.4.0
- com.unity.feature.development 1.0.2
- com.unity.ide.rider 3.0.39
- com.unity.ide.visualstudio 2.0.27
- com.unity.inputsystem 1.19.0
- com.unity.mathematics 1.3.3
- com.unity.render-pipelines.universal 17.4.0
- com.unity.test-framework 1.6.0
- com.unity.timeline 1.8.12
- com.unity.ugui 2.0.0

### Package Alignment Against Approved Architecture

| Requirement | Status |
|---|---|
| Entities (ECS) | ✓ com.unity.entities 6.4.0 |
| Burst | ✓ com.unity.burst 1.8.29 |
| Collections | ✓ com.unity.collections 6.4.0 |
| Mathematics | ✓ com.unity.mathematics 1.3.3 |
| Entities.Graphics | ✓ com.unity.entities.graphics 6.4.0 |
| Rendering (URP) | ✓ com.unity.render-pipelines.universal 17.4.0 |
| Input System | ✓ com.unity.inputsystem 1.19.0 |
| Addressables | ✓ com.unity.addressables 2.9.1 |
| Test Framework | ✓ com.unity.test-framework 1.6.0 |
| Netcode for Entities | ✗ NOT IN MANIFEST — staged, defer until multiplayer lane activates |

### Unity Version Alignment Decision Point

**Issue:** The approved architecture specifies Unity 6.3 LTS. The installed editors include `6000.3.13f1` (which IS Unity 6.3 LTS) AND `6000.4.2f1` (Unity 6.4). The canonical `unity/` project currently targets Unity 6.4 (`ProjectVersion.txt` says `6000.4.2f1`).

**Options:**

A. **Accept the drift to Unity 6.4** and update the approved architecture statement. Unity 6.4 is a superset; DOTS/ECS packages at 6.4.0 align. Risk: 6.4 is newer than LTS, so it carries more churn than 6.3 LTS.

B. **Downgrade `unity/` to 6000.3.13f1 (6.3 LTS)** for strict LTS compliance. Risk: Packages installed at `6.4.0` may not be bit-for-bit compatible with 6.3 LTS; the `packages-lock.json` will resolve differently; the Library cache regenerates.

C. **Dual-editor approach.** Keep both editors installed. Use 6.3 LTS for production builds and 6.4 for preview/experiments. Requires careful project switching.

**Recommendation:** Option B (downgrade to 6.3 LTS) is most aligned with the user's explicit approval. The DOTS/ECS packages at `6.4.0` will need version adjustment to their 6.3 LTS equivalents (Entities 1.3.x / 1.4.x range under the 6.3 LTS manifold). Execute this as a dedicated pass before writing significant ECS code, so the code compiles under the target.

**Awaiting your direction on Option A vs B vs C.** This is the one open decision that blocks starting real ECS code.

### Resolution (2026-04-14 Session 10): OPTION B LOCKED

Lance authorized Option B. Unity 6.3 LTS (`6000.3.13f1`) is the locked editor version for `unity/`.

Actions performed in Session 10:

- `unity/ProjectSettings/ProjectVersion.txt` updated from `6000.4.2f1` to `6000.3.13f1`. `m_EditorVersionWithRevision` marked `pending-first-open` until Lance opens the editor and Unity writes the full revision hash.
- `unity/Packages/manifest.json` package versions adjusted to 6.3 LTS-compatible targets:
  - `com.unity.entities`: `6.4.0` → `1.4.0`
  - `com.unity.collections`: `6.4.0` → `2.5.7`
  - `com.unity.entities.graphics`: `6.4.0` → `1.4.0`
  - `com.unity.render-pipelines.universal`: `17.4.0` → `17.3.0`
  - `com.unity.inputsystem`: `1.19.0` → `1.11.2`
  - `com.unity.addressables`: `2.9.1` → `2.5.0`
  - `com.unity.burst` (`1.8.29`), `com.unity.mathematics` (`1.3.3`), `com.unity.test-framework` (`1.6.0`), `com.unity.timeline` (`1.8.12`), `com.unity.ugui` (`2.0.0`), `com.unity.ide.rider` (`3.0.39`), `com.unity.ide.visualstudio` (`2.0.27`), `com.unity.feature.development` (`1.0.2`) retained at current-known-stable values.
- On first open, Unity Package Manager may adjust any version that does not match its exact 6.3 LTS catalog string. Accept LTS-compatible replacements. Do not downgrade below the floors listed above.
- Session 10 also authored the first ECS component layer (15 components) and the first ECS systems (Bootstrap, RealmConditionCycle, PopulationGrowth). See `unity/Assets/_Bloodlines/Code/Components/` and `unity/Assets/_Bloodlines/Code/Systems/`.

ECS code lane is now UNBLOCKED. Next action: Lance opens Unity, accepts package resolutions, runs `Bloodlines → Import → Sync JSON Content`, commits generated ScriptableObjects.

### IDE Alignment

- VS 2022 is not installed at a full-install path. VS 18 (Community) IS installed. Unity's `ide.visualstudio` package handles both. Treat VS 18 as the working IDE unless you explicitly want to install VS 2022.
- VS Code can be used for C# editing but is not a full Unity debugger.
- JetBrains Rider not installed; `ide.rider 3.0.39` is in the manifest but irrelevant without the IDE.

---

## 5. Risks and Concerns

1. **Unity 6.3 LTS vs 6.4 drift (BLOCKER for production code).** See section 4. Must be resolved before real ECS code is written.
2. **Netcode for Entities not installed.** Deferred per directive; ensure structure preserves the `Networking/` folder and any future code writes no hardcoded single-player assumptions that will become painful to unwind.
3. **VS 2022 not installed (approved-preferred IDE).** VS 18 works as a superset; monitor for any Unity-integration friction.
4. **Wwise not installed (staged target).** `Audio/Middleware/` folder reserved. Ensure all near-term audio authoring stays engine-native (Unity AudioClip) so future Wwise adoption doesn't require rework.
5. **`Bloodlines/` stub folder at repo root.** Preserved per governance, but new sessions could accidentally open it as the primary project. Mitigation: `STUB_TEMPLATE_NOTICE.md` present; also the `unity/` README is now explicit about canonical location.
6. **Browser prototype is advancing ahead of Unity.** The JS reference simulation now has fortification + siege + realm cycle + smelting live; the Unity project does not yet have ECS implementations for those. This is acceptable as long as the JS simulation is treated as the spec — but it's a growing debt between the two surfaces.
7. **Library/ caches for both Unity projects are large and version-bound.** If Unity version changes, Library regenerates. This is non-destructive but can take minutes.

---

## 6. Current Readiness State

### Ready Now

- **Environment discovery:** complete.
- **Project discovery:** complete. Canonical Unity project identified at `<repo>/unity/`.
- **Structural baseline:** created, matches approved layout.
- **ScriptableObject layer:** extended to cover all current canon (fortification/siege/settlement/realm conditions included).
- **JSON importer:** ready to sync canonical data into Unity on first `Bloodlines → Import → Sync JSON Content` invocation.
- **Reference simulation:** live, spec-complete for the lanes built so far.
- **Canonical design corpus:** fully preserved at `01_CANON/`, `04_SYSTEMS/`, `05_LORE/`, `06_FACTIONS/`, `07_FAITHS/`, etc.

### Ready Pending Decision

- **ECS system creation** — blocked only by the Unity version alignment decision (6.3 LTS vs 6.4).
- **First simulation scene with unit spawning** — can begin immediately after Unity version is locked.
- **JSON → ScriptableObject first full sync** — ready now; requires Unity to be opened once to run the importer menu action.

### Not Ready Yet (Next Waves)

- **Production-grade battlefield camera with zoom transition** — needs Camera/ code, which waits on ECS lane.
- **Fog-of-war scaling from battlefield to theatre** — needs Rendering + AI awareness layers; large scope, stage-appropriate for later.
- **Bloodline top-member HUD presence** — needs UI/ code and ScriptableObject linkage to dynasty state; structure is in place, implementation pending.
- **Wwise integration** — deferred; no blocker.
- **Netcode for Entities** — deferred; no blocker.

---

## 7. Next Execution Targets (When Unity Version Is Locked)

**Immediate (Session N+1):**

1. Lock Unity version (6.3 LTS strongly recommended per user directive; adjust `unity/ProjectSettings/ProjectVersion.txt` and re-resolve manifest to 6.3 LTS package versions).
2. Open the project in Unity once; run **Bloodlines → Import → Sync JSON Content** to generate all ScriptableObject `.asset` files under `Assets/_Bloodlines/Data/*/`.
3. Commit the generated `.asset` files so they become tracked ground truth for the Unity side.

**Session N+2 (ECS Foundation):**

4. Write the first `IComponentData` structs for: `Position`, `Faction`, `Health`, `UnitTypeId`, `BuildingTypeId`, `ResourceNode`, `ControlPoint`, `Settlement` with `FortificationTier`. Place in `Assets/_Bloodlines/Code/Components/`.
5. Write the first `ISystem` implementations: `ResourceAccumulationSystem`, `PopulationGrowthSystem`, `UnitMovementSystem`. Place in `Assets/_Bloodlines/Code/Systems/`.
6. Write the first `Baker<T>` + `Authoring` components to convert scene-placed objects into entities. Place in `Assets/_Bloodlines/Code/Authoring/` and `Assets/_Bloodlines/Code/Baking/`.
7. Create `Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity` that initializes the subscene + spawn configuration.
8. Create `Assets/_Bloodlines/Scenes/Gameplay/IronmarkFrontier.unity` as the first playable scene seeded from `MapDefinitions/ironmark_frontier.asset`.

**Session N+3 (Battlefield Feel):**

9. Battlefield camera with pan (WASD/arrows), zoom (scroll), rotate (optional middle-drag). Target feel: Generals/Zero Hour.
10. Selection (click, drag-box) using Input System 1.19 + screen-to-world conversion.
11. Right-click command dispatch (move, attack, gather, build).
12. Construction preview / placement with stone + iron cost gating for fortification buildings.

**Session N+4 (Dynasty + Bloodline UI Presence):**

13. Dynasty component + system that mirrors the browser prototype's cascade (head → heir → commander → governor chain).
14. HUD with persistent top-bloodline-member panel — canonical requirement.
15. Realm condition dashboard surfacing 11 canonical pressure states.

**Session N+5 (Theatre Zoom):**

16. Strategic-zoom camera mode: zoom-out transition from battlefield to larger map view.
17. Fog-of-war scaling appropriate to each zoom level.
18. Continuity between battlefield and strategic views.

---

## 8. Continuity

This report is the 2026-04-14 environment audit snapshot. Its companion continuity files to update:

- `CURRENT_PROJECT_STATE.md` — reflect the normalized Unity layout + new canon fields + pending version decision
- `NEXT_SESSION_HANDOFF.md` — next recommended action becomes "lock Unity version, run JSON sync, then start ECS foundation"
- `continuity/PROJECT_STATE.json` — primary_next_focus refreshed
- `00_ADMIN/CHANGE_LOG.md` — dated entry for this wave (already covered by the preceding browser-runtime wave + this environment-alignment wave)
- `tasks/todo.md` — mark environment-alignment items complete; add the Unity ECS foundation tasks listed above

Do not overwrite or reduce existing continuity. Append.
