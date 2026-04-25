# LOCAL MODEL WORK QUEUE — Bloodlines Unity AI Strategic Layer
# Date authored: 2026-04-24
# Pre-researched by Claude Sonnet 4.6. All types verified in the codebase.
# Complete this queue in order. Never skip a sub-slice.

---

## How to use this queue

1. Read the sub-slice specification completely before writing any code.
2. Read the "Read these files first" files listed in each sub-slice.
3. Implement following the patterns in LOCAL_MODEL_STARTER_PROMPT.md.
4. Run validation commands after each sub-slice.
5. Fix any errors before committing.
6. Write handoff document, commit, push to master, then move to the next sub-slice.

The current CONTRACT REVISION is 139 (file: docs/unity/CONCURRENT_SESSION_CONTRACT.md).
Sub-slice 35 bumps it to 140. Each subsequent sub-slice increments by 1.

The last csproj anchor entries (add your new entries AFTER these lines):

Assembly-CSharp.csproj:
```xml
<Compile Include="Assets\_Bloodlines\Code\AI\AISuccessionCrisisConsolidationComponent.cs" />
<Compile Include="Assets\_Bloodlines\Code\AI\AISuccessionCrisisConsolidationSystem.cs" />
<Compile Include="Assets\_Bloodlines\Code\Debug\BloodlinesDebugCommandSurface.SuccessionCrisisConsolidation.cs" />
```

Assembly-CSharp-Editor.csproj:
```xml
<Compile Include="Assets\_Bloodlines\Code\Editor\BloodlinesSuccessionCrisisConsolidationSmokeValidation.cs" />
```

---

## Sub-slice 35: AI Faith Commitment Auto-Selection

**Branch:** `claude/unity-ai-faith-commitment`
**CONTRACT REVISION:** bump 139 → 140, Last Updated By: `local-model-2026-04-24`

### Purpose

When an AI faction has not yet committed to a covenant faith (FaithStateComponent.SelectedFaith == None), automatically select and commit to the highest-exposure available faith as soon as one reaches the threshold.

Browser equivalent: ai.js updateEnemyAi lines 1253-1260.
Simulation function: `chooseFaithCommitment` at simulation.js line 9694.

### Read these files first

- `unity/Assets/_Bloodlines/Code/Components/FaithComponent.cs` (FaithStateComponent, FaithExposureElement, CovenantId, DoctrinePath)
- `unity/Assets/_Bloodlines/Code/AI/AIMissionaryResolutionSystem.cs` (example of reading FaithExposureElement buffer)
- `unity/Assets/_Bloodlines/Code/AI/AISuccessionCrisisConsolidationSystem.cs` (example of overall system shape)

### Files to create

| File | Description |
|------|-------------|
| `unity/Assets/_Bloodlines/Code/AI/AIFaithCommitmentSystem.cs` | The system |
| `unity/Assets/_Bloodlines/Code/AI/AIFaithCommitmentSystem.cs.meta` | Meta (guid: `a2b3c4d5e6f7890112345678bcdef102`) |
| `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.FaithCommitment.cs` | Debug surface |
| `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.FaithCommitment.cs.meta` | Meta (guid: `b3c4d5e6f708901234567890cdef1203`) |
| `unity/Assets/_Bloodlines/Code/Editor/BloodlinesFaithCommitmentSmokeValidation.cs` | Smoke validator |
| `unity/Assets/_Bloodlines/Code/Editor/BloodlinesFaithCommitmentSmokeValidation.cs.meta` | Meta (guid: `c4d5e6f7089012345678901234ef2304`) |
| `scripts/Invoke-BloodlinesUnityFaithCommitmentSmokeValidation.ps1` | PS1 wrapper |

### System implementation

```
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(AISuccessionCrisisConsolidationSystem))]
public partial struct AIFaithCommitmentSystem : ISystem
```

**Usings:** `Bloodlines.Components`, `Bloodlines.Conviction`, `Unity.Collections`, `Unity.Entities`

**RequireForUpdate:** `FaithStateComponent`

**OnUpdate logic (per-frame):**

Query: factions with `FactionComponent` + `AIEconomyControllerComponent` + `FaithStateComponent` (AIEconomyControllerComponent is the AI-only gate; player factions do not have it).

For each AI faction entity:
1. Skip if `FaithStateComponent.SelectedFaith != CovenantId.None` (already committed).
2. Skip if entity has no `FaithExposureElement` buffer.
3. Scan the `FaithExposureElement` buffer. Find the entry where `Discovered == true && Exposure >= 100f` with the highest `Exposure`. Call it `bestFaith`.
4. If no qualifying entry found, skip this faction.
5. Set `FaithStateComponent.SelectedFaith = bestFaith.Faith`.
6. Set `FaithStateComponent.DoctrinePath = DoctrinePath.Light`.
7. Set `FaithStateComponent.Intensity = 20f`.
8. Set `FaithStateComponent.Level = 1`.
9. Write the updated FaithStateComponent back with `em.SetComponentData`.
10. Conviction: if entity has `ConvictionComponent`, call `ConvictionScoring.ApplyEvent(ref conviction, ConvictionBucket.Oathkeeping, 2f)` then write back.
11. Narrative: push an Info-tone message with the faction's FactionId + " aligned with a covenant faith." using `NarrativeMessageBridge.Push`.

This is naturally one-shot per faction: once SelectedFaith is set it stays set and the gate at step 1 prevents re-entry.

**No new component needed.**

### Debug surface

File: `BloodlinesDebugCommandSurface.FaithCommitment.cs`

```csharp
namespace Bloodlines.Debug
{
    public partial class BloodlinesDebugCommandSurface
    {
        // Returns the FaithStateComponent for a faction by id, out param.
        // Returns false if entity not found or has no FaithStateComponent.
        public bool TryDebugGetFaithState(string factionId, out Bloodlines.Components.FaithStateComponent faith)
        {
            faith = default;
            using var q = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<Bloodlines.Components.FaithStateComponent>());
            using var entities = q.ToEntityArray(Allocator.Temp);
            using var factions = q.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var faithStates = q.ToComponentDataArray<Bloodlines.Components.FaithStateComponent>(Allocator.Temp);
            for (int i = 0; i < factions.Length; i++)
            {
                if (factions[i].FactionId == (FixedString32Bytes)factionId)
                {
                    faith = faithStates[i];
                    return true;
                }
            }
            return false;
        }
    }
}
```

### Smoke validator

File: `BloodlinesFaithCommitmentSmokeValidation.cs`

Three phases. Log artifact path: `../artifacts/unity-faith-commitment-smoke.log`
Marker: `BLOODLINES_FAITH_COMMITMENT_SMOKE PASS` on success, `BLOODLINES_FAITH_COMMITMENT_SMOKE FAIL` on any failure.

**Phase 1 -- CommitsWhenAvailable:**
- Create AI faction entity: `FactionComponent { FactionId = "test_enemy" }` + `AIEconomyControllerComponent` + `FaithStateComponent { SelectedFaith = CovenantId.None }`.
- Add `FaithExposureElement` buffer with one entry: `{ Faith = CovenantId.OldLight, Exposure = 110f, Discovered = true }`.
- Add `ConvictionComponent {}` (zero-initialized).
- Run `AIFaithCommitmentSystem.OnUpdate`.
- Assert: `FaithStateComponent.SelectedFaith == CovenantId.OldLight`.
- Assert: `FaithStateComponent.Intensity == 20f`.
- Assert: `FaithStateComponent.Level == 1`.
- Assert: `ConvictionComponent.Oathkeeping >= 2f`.
- Assert: at least one NarrativeMessageBufferElement was pushed.

**Phase 2 -- SkipsWhenExposureLow:**
- Same setup but `Exposure = 80f` (below threshold 100f).
- Run system.
- Assert: `FaithStateComponent.SelectedFaith == CovenantId.None` (not committed).

**Phase 3 -- SkipsWhenAlreadyCommitted:**
- Same setup as Phase 1 but FaithStateComponent already has `SelectedFaith = CovenantId.OldLight`.
- Record conviction before run.
- Run system.
- Assert: conviction did not change (no double-commit).

### Assembly registration

Assembly-CSharp.csproj (add after SuccessionCrisisConsolidation block):
```xml
<Compile Include="Assets\_Bloodlines\Code\AI\AIFaithCommitmentSystem.cs" />
<Compile Include="Assets\_Bloodlines\Code\Debug\BloodlinesDebugCommandSurface.FaithCommitment.cs" />
```

Assembly-CSharp-Editor.csproj (add after BloodlinesSuccessionCrisisConsolidationSmokeValidation):
```xml
<Compile Include="Assets\_Bloodlines\Code\Editor\BloodlinesFaithCommitmentSmokeValidation.cs" />
```

### Handoff document

Path: `docs/unity/session-handoffs/2026-04-24-unity-ai-faith-commitment.md`
(Adjust date to actual date if different.)

### Commit message

```
ai-strategic-layer: add AIFaithCommitmentSystem (sub-slice 35)

Ports ai.js updateEnemyAi lines 1253-1260 (faith commitment auto-select).
AI factions with no committed faith auto-commit to highest-exposure
qualifying covenant (Exposure >= 100, Discovered). Doctrine: Light,
Intensity: 20, Oathkeeping +2. One-shot per faction via SelectedFaith gate.

Co-Authored-By: Local Model <noreply@local>
```

---

## Sub-slice 36: AI Succession Crisis Context Flag Refresher

**Branch:** `claude/unity-ai-succession-crisis-context`
**CONTRACT REVISION:** bump 140 → 141, Last Updated By: `local-model-2026-04-24`

### Purpose

AIStrategicPressureSystem reads `PlayerSuccessionCrisisActive`, `PlayerSuccessionCrisisHigh`, `EnemySuccessionCrisisActive`, and `EnemySuccessionCrisisSevere` from AIStrategyComponent to apply timer clamps (lines 1161-1185 in ai.js). Currently nothing writes these flags from real game state. This system does exactly that.

Browser equivalent: ai.js updateEnemyAi lines 1161-1185 (the two succession crisis blocks that read game state to determine timer clamps).

### Read these files first

- `unity/Assets/_Bloodlines/Code/AI/AIStrategyComponent.cs` (flag field names at lines 99-106)
- `unity/Assets/_Bloodlines/Code/AI/AISuccessionCrisisConsolidationSystem.cs` (SuccessionCrisisComponent usage pattern)
- `unity/Assets/_Bloodlines/Code/AI/AIStrategicPressureSystem.cs` (how the flags are consumed)

### Key types

```
SuccessionCrisisComponent.CrisisSeverity: byte  (namespace: Bloodlines.Dynasties)
  None = 0, Minor = 1, Moderate = 2, Major = 3, Catastrophic = 4
  Severity >= 3 counts as "high" / "severe" (Major or Catastrophic)

AIStrategyComponent (namespace: Bloodlines.AI)
  PlayerSuccessionCrisisActive: bool
  PlayerSuccessionCrisisHigh: bool
  EnemySuccessionCrisisActive: bool
  EnemySuccessionCrisisSevere: bool
```

### Files to create

| File | Description |
|------|-------------|
| `unity/Assets/_Bloodlines/Code/AI/AISuccessionCrisisContextSystem.cs` | The system |
| `unity/Assets/_Bloodlines/Code/AI/AISuccessionCrisisContextSystem.cs.meta` | Meta (guid: `d5e6f7089a1023456789012345f03405`) |
| `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.SuccessionCrisisContext.cs` | Debug surface |
| `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.SuccessionCrisisContext.cs.meta` | Meta (guid: `e6f7089ab1234567890123456g14506`) |
| `unity/Assets/_Bloodlines/Code/Editor/BloodlinesSuccessionCrisisContextSmokeValidation.cs` | Smoke validator |
| `unity/Assets/_Bloodlines/Code/Editor/BloodlinesSuccessionCrisisContextSmokeValidation.cs.meta` | Meta (guid: `f708901bc2345678901234567h25607`) |
| `scripts/Invoke-BloodlinesUnitySuccessionCrisisContextSmokeValidation.ps1` | PS1 wrapper |

### System implementation

```
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(AIFaithCommitmentSystem))]
public partial struct AISuccessionCrisisContextSystem : ISystem
```

**Usings:** `Bloodlines.Components`, `Bloodlines.Dynasties`, `Unity.Collections`, `Unity.Entities`

**RequireForUpdate:** `AIStrategyComponent`

**OnUpdate logic:**

Step 1 -- Discover player succession crisis state:
- Query entities with `FactionComponent` + `SuccessionCrisisComponent`. Scan for player faction (FactionId == "player").
- `playerCrisisActive = player entity has SuccessionCrisisComponent && CrisisSeverity > 0`
- `playerCrisisHigh = playerCrisisActive && CrisisSeverity >= 3`
- If player entity found but has no SuccessionCrisisComponent: both false.

Step 2 -- For each AI faction:
- Query entities with `FactionComponent` + `AIEconomyControllerComponent` + `AIStrategyComponent`.
- For each AI faction entity:
  - Check if this entity has `SuccessionCrisisComponent`:
    - `enemyCrisisActive = hasCrisisComponent && CrisisSeverity > 0`
    - `enemyCrisisSevere = enemyCrisisActive && CrisisSeverity >= 3`
  - Else: both false.
  - Update AIStrategyComponent:
    - `s.PlayerSuccessionCrisisActive = playerCrisisActive`
    - `s.PlayerSuccessionCrisisHigh = playerCrisisHigh`
    - `s.EnemySuccessionCrisisActive = enemyCrisisActive`
    - `s.EnemySuccessionCrisisSevere = enemyCrisisSevere`
  - Write back with `em.SetComponentData`.

**No new component needed.**

### Debug surface

File: `BloodlinesDebugCommandSurface.SuccessionCrisisContext.cs`

Expose `TryDebugGetSuccessionCrisisContextFlags(string aiFactionId, out bool playerActive, out bool playerHigh, out bool enemyActive, out bool enemySevere)` that reads AIStrategyComponent for the named faction and extracts those four fields.

### Smoke validator

File: `BloodlinesSuccessionCrisisContextSmokeValidation.cs`

Three phases. Artifact: `../artifacts/unity-succession-crisis-context-smoke.log`
Marker: `BLOODLINES_SUCCESSION_CRISIS_CONTEXT_SMOKE PASS` / `FAIL`

**Phase 1 -- PlayerCrisisHighDetected:**
- Create player faction entity: `FactionComponent { FactionId = "player" }` + `SuccessionCrisisComponent { CrisisSeverity = 3 }` (Major).
- Create AI faction entity: `FactionComponent { FactionId = "test_enemy" }` + `AIEconomyControllerComponent` + `AIStrategyComponent {}`.
- Run `AISuccessionCrisisContextSystem.OnUpdate`.
- Assert: `PlayerSuccessionCrisisActive == true`.
- Assert: `PlayerSuccessionCrisisHigh == true`.
- Assert: `EnemySuccessionCrisisActive == false` (AI has no crisis component yet).

**Phase 2 -- EnemyCrisisSevereDetected:**
- Same AI faction entity, now also add `SuccessionCrisisComponent { CrisisSeverity = 4 }` (Catastrophic).
- Run system.
- Assert: `EnemySuccessionCrisisActive == true`.
- Assert: `EnemySuccessionCrisisSevere == true`.

**Phase 3 -- NoCrisis:**
- Entities with no SuccessionCrisisComponent on either.
- Run system.
- Assert: all four flags false.

### Assembly registration

Assembly-CSharp.csproj (add after AIFaithCommitmentSystem block):
```xml
<Compile Include="Assets\_Bloodlines\Code\AI\AISuccessionCrisisContextSystem.cs" />
<Compile Include="Assets\_Bloodlines\Code\Debug\BloodlinesDebugCommandSurface.SuccessionCrisisContext.cs" />
```

Assembly-CSharp-Editor.csproj:
```xml
<Compile Include="Assets\_Bloodlines\Code\Editor\BloodlinesSuccessionCrisisContextSmokeValidation.cs" />
```

### Commit message

```
ai-strategic-layer: add AISuccessionCrisisContextSystem (sub-slice 36)

Writes PlayerSuccessionCrisisActive/High and EnemySuccessionCrisisActive/Severe
flags into AIStrategyComponent from live SuccessionCrisisComponent data.
Feeds AIStrategicPressureSystem timer clamps (ai.js lines 1161-1185).

Co-Authored-By: Local Model <noreply@local>
```

---

## Sub-slice 37: AI Holy War Context Flag Refresher

**Branch:** `claude/unity-ai-holy-war-context`
**CONTRACT REVISION:** bump 141 → 142, Last Updated By: `local-model-2026-04-24`

### Purpose

AIStrategicPressureSystem reads `AIStrategyComponent.HolyWarActive` and applies attack/territory timer caps when any holy war is active (ai.js lines 1129-1132). Currently nothing writes this flag from real game state. This system scans active DynastyOperationKind.HolyWar operations and sets the flag accordingly.

Browser equivalent: ai.js updateEnemyAi lines 1129-1132. Any active holy war (either direction) triggers these clamps.

### Read these files first

- `unity/Assets/_Bloodlines/Code/AI/DynastyOperationComponent.cs` (DynastyOperationKind enum + DynastyOperationComponent fields)
- `unity/Assets/_Bloodlines/Code/AI/AIStrategyComponent.cs` (HolyWarActive field at line 84)
- `unity/Assets/_Bloodlines/Code/AI/AIHolyWarExecutionSystem.cs` (example of querying DynastyOperationComponent)

### Key types

```
DynastyOperationComponent (namespace: Bloodlines.AI)
  OperationId: FixedString64Bytes
  SourceFactionId: FixedString32Bytes
  TargetFactionId: FixedString32Bytes
  OperationKind: DynastyOperationKind
  Active: bool

DynastyOperationKind.HolyWar = 2

AIStrategyComponent.HolyWarActive: bool  -- set true if ANY HolyWar op involving this faction is active
```

### Files to create

| File | Description |
|------|-------------|
| `unity/Assets/_Bloodlines/Code/AI/AIHolyWarContextSystem.cs` | The system |
| `unity/Assets/_Bloodlines/Code/AI/AIHolyWarContextSystem.cs.meta` | Meta (guid: `0809a1bc2d34567890123456789e4708`) |
| `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.HolyWarContext.cs` | Debug surface |
| `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.HolyWarContext.cs.meta` | Meta (guid: `1920b2cd3e45678901234567890f5809`) |
| `unity/Assets/_Bloodlines/Code/Editor/BloodlinesHolyWarContextSmokeValidation.cs` | Smoke validator |
| `unity/Assets/_Bloodlines/Code/Editor/BloodlinesHolyWarContextSmokeValidation.cs.meta` | Meta (guid: `2a31c3de4f5678901234567890106900`) |
| `scripts/Invoke-BloodlinesUnityHolyWarContextSmokeValidation.ps1` | PS1 wrapper |

### System implementation

```
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(AISuccessionCrisisContextSystem))]
public partial struct AIHolyWarContextSystem : ISystem
```

**Usings:** `Bloodlines.Components`, `Unity.Collections`, `Unity.Entities`

**RequireForUpdate:** `AIStrategyComponent`

**OnUpdate logic:**

Step 1 -- Collect all active HolyWar operation source and target faction IDs:
- Query all entities with `DynastyOperationComponent`.
- Filter: `OperationKind == DynastyOperationKind.HolyWar && Active == true`.
- Collect all `SourceFactionId` and `TargetFactionId` values into a `NativeHashSet<FixedString32Bytes>` (Allocator.Temp).

Step 2 -- Update each AI faction:
- Query entities with `FactionComponent` + `AIEconomyControllerComponent` + `AIStrategyComponent`.
- For each AI faction entity, check if `factionId` exists in the set (either as source or target).
- Set `AIStrategyComponent.HolyWarActive = involvedInHolyWar`.
- Write back.

Dispose the NativeHashSet after use.

**No new component needed.**

### Debug surface

File: `BloodlinesDebugCommandSurface.HolyWarContext.cs`

Expose `TryDebugGetHolyWarContextFlag(string aiFactionId, out bool holyWarActive)` that reads AIStrategyComponent.HolyWarActive for the named faction.

### Smoke validator

File: `BloodlinesHolyWarContextSmokeValidation.cs`

Two phases. Artifact: `../artifacts/unity-holy-war-context-smoke.log`
Marker: `BLOODLINES_HOLY_WAR_CONTEXT_SMOKE PASS` / `FAIL`

**Phase 1 -- HolyWarDetected:**
- Create AI faction entity: `FactionComponent { FactionId = "test_enemy" }` + `AIEconomyControllerComponent` + `AIStrategyComponent {}`.
- Create operation entity: `DynastyOperationComponent { OperationKind = DynastyOperationKind.HolyWar, SourceFactionId = "test_enemy", TargetFactionId = "player", Active = true }`.
- Run `AIHolyWarContextSystem.OnUpdate`.
- Assert: `AIStrategyComponent.HolyWarActive == true`.

**Phase 2 -- NoHolyWar:**
- Same AI faction entity. No operation entities, or only operations with Active=false.
- Run system.
- Assert: `AIStrategyComponent.HolyWarActive == false`.

### Assembly registration

Assembly-CSharp.csproj:
```xml
<Compile Include="Assets\_Bloodlines\Code\AI\AIHolyWarContextSystem.cs" />
<Compile Include="Assets\_Bloodlines\Code\Debug\BloodlinesDebugCommandSurface.HolyWarContext.cs" />
```

Assembly-CSharp-Editor.csproj:
```xml
<Compile Include="Assets\_Bloodlines\Code\Editor\BloodlinesHolyWarContextSmokeValidation.cs" />
```

### Commit message

```
ai-strategic-layer: add AIHolyWarContextSystem (sub-slice 37)

Writes AIStrategyComponent.HolyWarActive from live DynastyOperationKind.HolyWar
data. Any active holy war (source or target) involving the AI faction sets the
flag. Feeds AIStrategicPressureSystem clamps (ai.js lines 1129-1132).

Co-Authored-By: Local Model <noreply@local>
```

---

## Sub-slice 38: AI Player Divine Right Context Flag Refresher

**Branch:** `claude/unity-ai-player-divine-right-context`
**CONTRACT REVISION:** bump 142 → 143, Last Updated By: `local-model-2026-04-24`

### Purpose

AIStrategicPressureSystem reads `AIStrategyComponent.PlayerDivineRightActive` to apply timer clamps (ai.js lines 1156-1160). `AICovertOpsComponent.PlayerDivineRightActive` also blocks AI re-declaration of divine right when the player has one active. Both fields currently have no system writing them from real game state. This system detects an active DynastyOperationKind.DivineRight on the player faction and writes both flags.

Browser equivalent: ai.js updateEnemyAi lines 1156-1160.

### Read these files first

- `unity/Assets/_Bloodlines/Code/AI/DynastyOperationComponent.cs` (DynastyOperationKind enum, DynastyOperationComponent)
- `unity/Assets/_Bloodlines/Code/AI/AICovertOpsComponent.cs` (PlayerDivineRightActive field)
- `unity/Assets/_Bloodlines/Code/AI/AIStrategyComponent.cs` (PlayerDivineRightActive field at line 96)
- `unity/Assets/_Bloodlines/Code/AI/AIHolyWarContextSystem.cs` (same pattern you just wrote in sub-slice 37)

### Key types

```
DynastyOperationKind.DivineRight = 3

AIStrategyComponent.PlayerDivineRightActive: bool
AICovertOpsComponent.PlayerDivineRightActive: bool  -- line 41 in AICovertOpsComponent.cs

Gate: DynastyOperationComponent.OperationKind == DynastyOperationKind.DivineRight
      && SourceFactionId == "player"
      && Active == true
```

### Files to create

| File | Description |
|------|-------------|
| `unity/Assets/_Bloodlines/Code/AI/AIPlayerDivineRightContextSystem.cs` | The system |
| `unity/Assets/_Bloodlines/Code/AI/AIPlayerDivineRightContextSystem.cs.meta` | Meta (guid: `3b42d4ef506789012345678901217a01`) |
| `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.PlayerDivineRightContext.cs` | Debug surface |
| `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.PlayerDivineRightContext.cs.meta` | Meta (guid: `4c53e5f0617890123456789012328b02`) |
| `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerDivineRightContextSmokeValidation.cs` | Smoke validator |
| `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerDivineRightContextSmokeValidation.cs.meta` | Meta (guid: `5d64f6017289012345678901234390c3`) |
| `scripts/Invoke-BloodlinesUnityPlayerDivineRightContextSmokeValidation.ps1` | PS1 wrapper |

### System implementation

```
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(AIHolyWarContextSystem))]
public partial struct AIPlayerDivineRightContextSystem : ISystem
```

**Usings:** `Bloodlines.Components`, `Unity.Collections`, `Unity.Entities`

**RequireForUpdate:** `AIStrategyComponent`

**OnUpdate logic:**

Step 1 -- Detect player divine right:
- Query all entities with `DynastyOperationComponent`.
- `hasPlayerDivineRight = any entry where OperationKind == DivineRight && SourceFactionId == "player" && Active == true`
- No NativeHashSet needed (it is a single boolean).

Step 2 -- Update each AI faction:
- Query entities with `FactionComponent` + `AIEconomyControllerComponent` + `AIStrategyComponent`.
- For each AI faction entity:
  - Read current `AIStrategyComponent`, set `PlayerDivineRightActive = hasPlayerDivineRight`, write back.
  - If entity has `AICovertOpsComponent`:
    - Read current `AICovertOpsComponent`, set `PlayerDivineRightActive = hasPlayerDivineRight`, write back.

**No new component needed.**

### Debug surface

File: `BloodlinesDebugCommandSurface.PlayerDivineRightContext.cs`

Expose `TryDebugGetPlayerDivineRightFlags(string aiFactionId, out bool strategyFlag, out bool covertOpsFlag)` that reads both components on the named faction.

### Smoke validator

File: `BloodlinesPlayerDivineRightContextSmokeValidation.cs`

Two phases. Artifact: `../artifacts/unity-player-divine-right-context-smoke.log`
Marker: `BLOODLINES_PLAYER_DIVINE_RIGHT_CONTEXT_SMOKE PASS` / `FAIL`

**Phase 1 -- PlayerDivineRightDetected:**
- Create AI faction entity: `FactionComponent { FactionId = "test_enemy" }` + `AIEconomyControllerComponent` + `AIStrategyComponent {}` + `AICovertOpsComponent {}`.
- Create operation entity: `DynastyOperationComponent { OperationKind = DynastyOperationKind.DivineRight, SourceFactionId = "player", Active = true }`.
- Run `AIPlayerDivineRightContextSystem.OnUpdate`.
- Assert: `AIStrategyComponent.PlayerDivineRightActive == true`.
- Assert: `AICovertOpsComponent.PlayerDivineRightActive == true`.

**Phase 2 -- NoDivineRight:**
- Same AI faction entity. No operation entities (or only operations with Active=false or SourceFactionId != "player").
- Run system.
- Assert: both flags false.

### Assembly registration

Assembly-CSharp.csproj:
```xml
<Compile Include="Assets\_Bloodlines\Code\AI\AIPlayerDivineRightContextSystem.cs" />
<Compile Include="Assets\_Bloodlines\Code\Debug\BloodlinesDebugCommandSurface.PlayerDivineRightContext.cs" />
```

Assembly-CSharp-Editor.csproj:
```xml
<Compile Include="Assets\_Bloodlines\Code\Editor\BloodlinesPlayerDivineRightContextSmokeValidation.cs" />
```

### Commit message

```
ai-strategic-layer: add AIPlayerDivineRightContextSystem (sub-slice 38)

Writes PlayerDivineRightActive into both AIStrategyComponent and
AICovertOpsComponent when a player DivineRight operation is active.
Feeds AIStrategicPressureSystem clamps (ai.js lines 1156-1160) and
blocks AI re-declaration while player has one in flight.

Co-Authored-By: Local Model <noreply@local>
```

---

## Notes for the verifying session

When Claude or Codex resumes after this local model queue completes, check:

1. Run all 5 validation commands from project root. All must exit 0.
2. Confirm CONCURRENT_SESSION_CONTRACT.md is at revision 143 (or the correct count).
3. Confirm all 4 sub-slices are committed and merged to master.
4. Review the smoke validation log artifacts in `unity/artifacts/` for any FAIL markers.

The next candidates after sub-slice 38:
- AI Covenant Test Action Dispatch (requires CovenantTestComponent which does not yet exist in Unity; skip for now)
- AI Scout Raid Dispatch (requires unit-command dispatch integration; complex, defer)
- AI Governance Recognition Context Flag Refresher (TerritorialGovernanceComponent not yet defined in Unity; defer)
