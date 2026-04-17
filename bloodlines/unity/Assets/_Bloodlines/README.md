# _Bloodlines (Unity Asset Root)

The production-game Unity tree for Bloodlines. Every asset created for the Unity build lives under this folder.

## Approved Engine + Architecture

- **Engine:** Unity 6.3 LTS (`6000.3.13f1`) locked in `ProjectSettings/ProjectVersion.txt`
- **Core architecture:** DOTS / ECS (`com.unity.entities`), Burst, Collections, Unity Mathematics
- **Rendering:** URP 17.3.0
- **Input:** Input System 1.11.2
- **Data:** ScriptableObject definitions synced from the canonical JSON layer at `<repo>/data/` via `Assets/_Bloodlines/Code/Editor/JsonContentImporter.cs`
- **Docs:** This project's internal technical docs live under `Docs/`. The canonical design corpus remains above this folder in `01_CANON/`, `04_SYSTEMS/`, and `18_EXPORTS/`

Preserved non-canonical Unity templates still on disk:

- `<repo>/Bloodlines/`
- `<repo>/unity/My project/`

They remain preserved only. Do not extend them for active Bloodlines work.

## Structural Contract

```text
_Bloodlines/
  Art/        Characters, Units, Buildings, Environment, Terrain, FX, UI, Icons, Concepts
  Audio/      Music, SFX, Voice, Middleware
  Code/       Components, Systems, Aspects, Authoring, Baking, AI, Pathing,
              Economy, Population, Faith, Dynasties, Combat, Construction, Camera,
              Input, Networking, UI, SaveLoad, Utilities, Debug, Editor, Definitions
  Data/       ScriptableDefinitions, FactionDefinitions, UnitDefinitions,
              BuildingDefinitions, TerrainDefinitions, TechDefinitions,
              AudioDefinitions, DynastyDefinitions, FaithDefinitions,
              ResourceDefinitions, ConvictionDefinitions, BloodlineRoleDefinitions,
              BloodlinePathDefinitions, VictoryConditionDefinitions, MapDefinitions,
              SettlementClassDefinitions, RealmConditionDefinitions
  Prefabs/    Units, Buildings, Environment, UI
  Scenes/     Bootstrap, Sandbox, Gameplay, Testbeds, Strategic
  Materials/  URP materials
  Shaders/    URP shaders
  Animation/  clips, controllers
  Docs/       Technical, Gameplay, Systems, Decisions, UIUX, Continuity
```

## Preservation Non-Negotiables

1. Nothing relevant is deleted without explicit authorization.
2. Historical context stays preserved even when later canon supersedes it.
3. New implementation extends the preserved bible; it does not flatten it.
4. The ScriptableObject bridge at `Code/Editor/JsonContentImporter.cs` reads from `<repo>/data/` - do not sever that link.
5. This folder is the production Unity track. The browser prototype at `<repo>/play.html` and `<repo>/src/game/` remains the reference simulation in parallel.

## Direction-Of-Play Reminders

- **Close-in battlefield feel:** readable, responsive, base-building and unit-production flow similar in spirit to Command & Conquer: Generals / Zero Hour
- **Expanded command layer:** more menus, prompts, next-step guidance, dynasty decisions, and system-interplay visibility than classic C&C
- **Scale transition:** support zoom out into a broader theatre-of-war while preserving continuity with the battlefield view
- **Bloodline command presence:** top bloodline members must be visible in the UI, not bolted on later
