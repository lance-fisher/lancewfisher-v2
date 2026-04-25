# LOCAL MODEL STARTER PROMPT — Bloodlines Unity AI Strategic Layer
# Date queued: 2026-04-24
# Deliver this entire file as the first message to the local model.

---

You are implementing Unity ECS (Entity Component System) code for Bloodlines, a dynasty strategy game. Your job is to work through a queue of sub-slices listed in `LOCAL_MODEL_WORK_QUEUE.md` in this same directory. Each sub-slice is a small, self-contained feature. You must implement them one at a time, run the validation commands, and commit before moving to the next.

## Project Location

All paths are relative to the Bloodlines project root:
`D:\ProjectsHome\Bloodlines\`

Unity code is in:
`unity/Assets/_Bloodlines/Code/`

## The Only Coding Pattern You Will Use

Every sub-slice creates one or more of these file types. Follow this pattern exactly.

### 1. ISystem pattern (all new systems use this)

```csharp
using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.AI
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AICovertOpsSystem))]  // adjust as specified per sub-slice
    public partial struct AIMySomethingSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SomeRequiredComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            float dt = SystemAPI.Time.DeltaTime;
            var em  = state.EntityManager;

            foreach (var (comp, faction, entity) in SystemAPI
                .Query<RefRW<MyComponent>, RefRO<FactionComponent>>()
                .WithEntityAccess())
            {
                // do work
            }
        }
    }
}
```

### 2. IComponentData pattern

```csharp
using Unity.Entities;

namespace Bloodlines.AI
{
    public struct MyComponent : IComponentData
    {
        public float SomeTimer;
        public bool SomeFlag;
    }
}
```

### 3. ECB usage (when you need to add/remove components)

```csharp
var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
// ... queue changes via ecb.SetComponent, ecb.AddComponent, ecb.RemoveComponent
ecb.Playback(em);
ecb.Dispose();
```

### 4. Narrative message

```csharp
var msg = new FixedString128Bytes();
msg.Append(factionId);
msg.Append((FixedString32Bytes)" something happened.");
NarrativeMessageBridge.Push(em, msg, NarrativeMessageTone.Info);
// NarrativeMessageTone.Warn for threats to player
```

### 5. Conviction event

```csharp
using Bloodlines.Conviction;
// ...
if (em.HasComponent<ConvictionComponent>(entity))
{
    var conviction = em.GetComponentData<ConvictionComponent>(entity);
    ConvictionScoring.ApplyEvent(ref conviction, ConvictionBucket.Stewardship, 2f);
    em.SetComponentData(entity, conviction);
}
// Buckets: Stewardship, Oathkeeping, Ruthlessness, Desecration
```

### 6. Meta files (required for every .cs file)

```
fileFormatVersion: 2
guid: <16-character lowercase hex string, must be unique>
MonoImporter:
  externalObjects: {}
  serializedVersion: 2
  defaultReferences: []
  executionOrder: 0
  icon: {instanceID: 0}
  userData:
  assetBundleName:
  assetBundleVariant:
```

Generate unique GUIDs by varying hex digits. Example: `a1b2c3d4e5f67890abcdef1234567890`

## Key Types Reference

```
FactionComponent.FactionId: FixedString32Bytes
DynastyMemberComponent.Role: DynastyRole (HeadOfBloodline, Spymaster, Diplomat, Merchant, ...)
DynastyMemberComponent.Status: DynastyMemberStatus (Active, Fallen, Captured)
DynastyMemberComponent.Renown: float
ResourceStockpileComponent.Gold: float
ResourceStockpileComponent.Influence: float
DynastyStateComponent.Legitimacy: float  (namespace: Bloodlines.Components)
ControlPointComponent.Loyalty: float
ControlPointComponent.OwnerFactionId: FixedString32Bytes
FaithStateComponent.SelectedFaith: CovenantId (None=0, OldLight=1, BloodDominion=2, TheOrder=3, TheWild=4)
FaithStateComponent.DoctrinePath: DoctrinePath (Unassigned=0, Light=1, Dark=2)
FaithStateComponent.Intensity: float
FaithExposureElement.Faith: CovenantId
FaithExposureElement.Exposure: float
FaithExposureElement.Discovered: bool
DualClockComponent.InWorldDays: float  (namespace: Bloodlines.GameTime)
SuccessionCrisisComponent.CrisisSeverity: byte  (namespace: Bloodlines.Dynasties)
ConvictionComponent.Stewardship/Oathkeeping/Ruthlessness/Desecration: float
```

## Assembly Registration

After creating each .cs file, you MUST add a `<Compile Include="..." />` entry to:

**For runtime code:**
`unity/Assembly-CSharp.csproj`

Find the block near the end where recent AI files are added:
```xml
<Compile Include="Assets\_Bloodlines\Code\AI\AISuccessionCrisisConsolidationSystem.cs" />
<Compile Include="Assets\_Bloodlines\Code\Debug\BloodlinesDebugCommandSurface.SuccessionCrisisConsolidation.cs" />
```
Add your new entries AFTER the last existing AI entry.

**For editor/validator code:**
`unity/Assembly-CSharp-Editor.csproj`

Find the block with existing smoke validators and add after:
```xml
<Compile Include="Assets\_Bloodlines\Code\Editor\BloodlinesSuccessionCrisisConsolidationSmokeValidation.cs" />
```

## Validation Commands (run after each sub-slice)

Run these in order from `D:\ProjectsHome\Bloodlines\`:

```
cd unity && dotnet build Assembly-CSharp.csproj -nologo
cd unity && dotnet build Assembly-CSharp-Editor.csproj -nologo
node tests/data-validation.mjs
node tests/runtime-bridge.mjs
powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1
```

All must exit 0 (or show "Build succeeded, 0 errors"). Fix errors before committing.

## CONCURRENT_SESSION_CONTRACT.md Update (required)

File: `docs/unity/CONCURRENT_SESSION_CONTRACT.md`

1. Bump `Revision:` by 1
2. Update `Last Updated:` to today
3. Update `Last Updated By:` to `local-model-YYYY-MM-DD`
4. Update `Supersedes:` line
5. Add new file paths to the ai-strategic-layer Owned Paths section
6. Add new session handoff doc path to Lane Authority Documents

## Session Handoff Document (required)

Create: `docs/unity/session-handoffs/YYYY-MM-DD-unity-ai-<slice-name>.md`

Content:
```markdown
# Session Handoff: Sub-slice N -- <Name>
## Goal
<one paragraph>
## Work Completed
### New Files
- list each file with brief description
### Modified Files
- list modifications
## Verification Results
- Build: 0 errors
- node tests: exit 0
- staleness check: exit 0
## Next Action
<what comes next>
## Browser Reference
<ai.js or simulation.js line references>
```

## Git Workflow

```bash
# Create branch from origin/master
git checkout -b claude/unity-ai-<slice-name>

# Stage only the files you created/modified for this slice
git add unity/Assets/_Bloodlines/Code/AI/MyNewFile.cs
git add ... (all files for this slice)

# Commit
git commit -m "ai-strategic-layer: add MyNewSystem (sub-slice N)\n\n<description>\n\nCo-Authored-By: Local Model <noreply@local>"

# Push branch
git push origin claude/unity-ai-<slice-name>

# Merge directly to master (do NOT create a pull request)
HASH=$(git rev-parse HEAD)
git push origin $HASH:master
```

## What NOT To Do

- Do not modify files outside the AI layer unless the sub-slice specification says to
- Do not add features beyond the sub-slice specification
- Do not modify CLAUDE.md, PROJECTS.json, or any governance file
- Do not run Unity Editor (use dotnet build only for compilation checks)
- Do not invent types or fields -- only use types listed in this document or that you have read from an existing file

## How To Start

1. Read `LOCAL_MODEL_WORK_QUEUE.md` to get the first sub-slice
2. Read any "read these files first" files listed in that sub-slice
3. Implement following the patterns above
4. Run validation commands
5. Fix any errors
6. Write handoff document
7. Commit and push to master
8. Read the next sub-slice and repeat
