---
name: performance-and-scale
description: Invoke when writing or reviewing a new ECS system, a loop over entities, a buffer iteration, a per-frame update, or any code that runs inside SimulationSystemGroup. Checks for N-squared loops, per-frame managed allocations, structural changes in hot paths, missing Burst compatibility, and patterns that threaten the large-scale simulation Bloodlines targets. Use before committing any runtime code that will run per-tick.
---

# Performance and Scale Skill (Bloodlines)

Bloodlines targets large-scale simulation: many units, many buildings, many factions, long-form matches. Per-tick cost compounds. This skill catches the patterns that silently kill frame rate as scale grows.

## Scale targets (for context when deciding trade-offs)

- 1,000 to 5,000 active units per match at peak.
- 9 playable factions plus tribal and neutral kinds.
- 100+ buildings per faction at late stage.
- Match duration 30-120 minutes. Dynasty-clock spans much longer.

If a system costs 1ms per tick on a 60Hz schedule that is 6% of a 16.6ms frame. Ten such systems consume 60%. Budget accordingly.

## Red flags (rank-ordered by blast radius)

### 1. N-squared entity loops

Pattern:
```csharp
foreach (var a in SystemAPI.Query<RefRO<UnitComponent>>()) {
    foreach (var b in SystemAPI.Query<RefRO<UnitComponent>>()) {
        ...
    }
}
```

Blast radius: 1,000² = 1,000,000 inner iterations. At even 100ns per inner body that is 100ms per frame. Fatal at scale.

Fixes:
- Spatial hash / grid: bucket entities by tile position, check only nearby buckets. `NativeParallelMultiHashMap<int2, Entity>` keyed by tile.
- K-NN via sorted positions.
- Range-limited broadphase: only units within `attackRange * 1.5` are candidates; use a spatial query.
- Cached target: store the current target in a component, only re-scan on cooldown (see `CombatStatsComponent.TargetAcquireIntervalSeconds` pattern already in combat lane).

### 2. Managed allocations in OnUpdate

Pattern:
```csharp
public void OnUpdate(ref SystemState state) {
    var list = new List<Entity>();  // managed allocation
    var sb = new StringBuilder();   // managed allocation
    var copy = $"entity {e.Index}"; // boxed string per entity
}
```

Blast radius: GC pressure. Framerate spikes under Unity's generational GC on large games.

Fixes:
- `NativeList<Entity>` / `NativeArray<Entity>` with `Allocator.Temp` and `using` scope.
- `FixedString64Bytes` for strings.
- Move string formatting to debug-only paths gated by `#if UNITY_EDITOR`.

### 3. Structural changes inside query iteration

Pattern:
```csharp
foreach (var (unit, e) in SystemAPI.Query<RefRO<UnitComponent>>().WithEntityAccess()) {
    if (dead) EntityManager.DestroyEntity(e);  // invalidates handles
    EntityManager.AddComponent<TagX>(e);       // same problem
}
```

Blast radius: `ObjectDisposedException` or silent data corruption. Also prevents Burst compilation.

Fixes:
- Collect entity IDs in a `NativeList<Entity>` inside the query, perform structural changes after the loop ends.
- Or use `EndSimulationEntityCommandBufferSystem.CreateCommandBuffer(state.WorldUnmanaged)` and issue `ecb.DestroyEntity(e)` / `ecb.AddComponent<TagX>(e)`; it plays back after the system completes.

### 4. Missing Burst compatibility

Pattern:
```csharp
public partial struct MySystem : ISystem {
    public void OnUpdate(ref SystemState state) {  // no [BurstCompile]
        ...
    }
}
```

Blast radius: 5-10x slower for arithmetic-heavy loops.

Fixes:
- Add `[BurstCompile]` at class and method level.
- If the body uses managed types, isolate those into a non-Bursted helper and keep the hot path Bursted.
- Check the Burst inspector for "ABORT: cannot compile" messages.

### 5. Linear search through every faction / unit / building for lookup

Pattern:
```csharp
using var entities = query.ToEntityArray(Allocator.Temp);
for (int i = 0; i < entities.Length; i++) {
    if (factions[i].FactionId.ToString() == factionId) return entities[i];
}
```

This is fine for once-per-slice debug lookups but catastrophic if called per-entity in a hot system.

Fixes:
- Cache a `NativeParallelHashMap<FixedString32Bytes, Entity>` keyed by FactionId, built once in `OnCreate` or refreshed on structural-change events.
- For per-tick hot paths, store the target entity directly on the component rather than looking up by id.

### 6. Over-broad queries

Pattern:
```csharp
var query = em.CreateEntityQuery(ComponentType.ReadOnly<FactionComponent>());
```

If the actual need is "factions that have a dynasty", require the discriminating component too:
```csharp
var query = em.CreateEntityQuery(
    ComponentType.ReadOnly<FactionComponent>(),
    ComponentType.ReadOnly<DynastyStateComponent>());
```

Every extra unrelated entity you iterate over is a cost multiplier.

### 7. Serializing through string for inter-system communication

Using JSON or `ToString()` as the carrier between systems. Fine for save/load payloads and debug, never for runtime communication.

Fix: shared components.

### 8. Over-granular system update

Pattern: a system that runs every frame but only needs to run every second.

Fix: accumulate dt in a component field, early-return until threshold. See the target-acquisition throttling pattern in `AutoAcquireTargetSystem`.

## Allocator discipline

- `Allocator.Temp`: scoped to the current job or method. Auto-disposed at frame end if forgotten, but always `using`.
- `Allocator.TempJob`: up to 4 frames. For job inputs.
- `Allocator.Persistent`: survives until explicit dispose. System-lifetime caches only.
- Never `Allocator.None`.

## Burst compatibility cheat sheet

Burst-compatible:
- `int`, `float`, `bool`, `byte`, enum-as-byte, `float3`, `float4`, `quaternion`, `Entity`, `FixedString*`.
- `NativeArray`, `NativeList`, `NativeHashMap`, `DynamicBuffer`.
- `math.*` from `Unity.Mathematics`.

Burst-hostile:
- `string`, `List<T>`, `Dictionary<T,U>`, `StringBuilder`.
- Any managed object reference.
- Reflection.
- Exceptions (though Burst will compile `throw` paths, they are slow).

## Scale review checklist (run for every new system)

1. Worst-case entity count this system will iterate over?
2. Is the inner body O(1) with respect to that count?
3. Any nested loops? What is the multiplier?
4. Any per-entity managed allocations?
5. Any structural changes inside iteration?
6. Is `[BurstCompile]` present?
7. Does `state.RequireForUpdate<T>()` skip the system when nothing is present?
8. Is this system in the correct group ordered by `[UpdateBefore] / [UpdateAfter]` for data dependency?

## Report format

- `PERF PASS`: "No scale hazards found. Estimated worst-case cost: <N>ms at <count> entities."
- `PERF RISK <severity>`: "Line <N>: <issue>. At <scale target> this is <cost estimate>. Fix: <recommendation>."

## What this skill does NOT do

- Does not run Unity Profiler (that is manual).
- Does not validate canon compliance (canon-enforcement).
- Does not enforce ECS idioms beyond scale impact (unity-ecs-discipline).
