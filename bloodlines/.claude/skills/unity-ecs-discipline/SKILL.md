---
name: unity-ecs-discipline
description: Invoke when writing or editing C# code under unity/Assets/_Bloodlines/Code/, when creating a new ECS system, component, buffer, or authoring class, or when reviewing code that will run every frame in the ECS simulation group. Enforces Unity 6.3 DOTS/ECS idioms and prevents regression to MonoBehaviour or object-oriented patterns in runtime systems. Use before committing any runtime Unity code.
---

# Unity DOTS/ECS Discipline Skill (Bloodlines)

Bloodlines targets Unity 6.3 LTS DOTS/ECS. Runtime simulation must be ECS-native. This skill enforces that discipline when code is being written, reviewed, or refactored.

## Required patterns

### Components

- Runtime state lives on `IComponentData` structs. No class-based state on entities.
- Lists-of-things on an entity live on `IBufferElementData` structs accessed via `DynamicBuffer<T>`.
- Tag components (zero-field `IComponentData`) for binary state.
- Component fields must be unmanaged. Prefer `FixedString32Bytes` / `FixedString64Bytes` over `string`. No `List<T>`, no `Dictionary<T, U>`, no managed collections as component fields.
- Singleton-like state lives on a singleton entity (one entity per world carrying the component), not on the world or a static field.

### Systems

- Prefer `ISystem` + `[BurstCompile]` over `SystemBase` for stateless loops. Reserve `SystemBase` for systems that need managed world interop (e.g. managed authoring, `EntityManager.AddComponentObject`).
- Decorate with `[UpdateInGroup(typeof(SimulationSystemGroup))]` (or `InitializationSystemGroup` for once-per-bootstrap, or `PresentationSystemGroup` for non-deterministic render-adjacent work). Add `[UpdateBefore]` / `[UpdateAfter]` when ordering matters.
- `OnCreate` sets `state.RequireForUpdate<T>()` for the primary required component so the system skips work when nothing is present.
- `OnUpdate` reads `SystemAPI.Time.DeltaTime` once at top if dt is needed; guard against `dt <= 0f` early returns.
- Iterate with `foreach (var (...) in SystemAPI.Query<RefRO<T>, RefRW<U>>().WithEntityAccess())` for the new idiom. Avoid legacy `Entities.ForEach` — it is deprecated.
- Structural changes (AddComponent / RemoveComponent / DestroyEntity / CreateEntity) go through `EndSimulationEntityCommandBufferSystem.CreateCommandBuffer(state.WorldUnmanaged)`, never inline. Never invalidate a `BufferTypeHandle<T>` or query result with a structural change mid-iteration.
- No managed allocations in `OnUpdate`. No `new List<T>()`, `new Dictionary<T,U>()`, `new string(...)` per-frame. If you need a temporary collection, use `NativeArray<T>` / `NativeList<T>` / `NativeHashMap<T,U>` with `Allocator.Temp` and dispose with `using`.

### Authoring and Baking

- Authoring MonoBehaviours live under `Code/Authoring/`. They capture inspector-facing configuration.
- Bakers live under `Code/Editor/` or `Code/Baking/` and translate authoring → components via `IBaker<T>.AddComponent(...)`.
- Never create runtime simulation logic inside an authoring class or a MonoBehaviour. Authoring is a build-time transform, not a runtime system.

### Allowed MonoBehaviour usage

MonoBehaviours are permitted ONLY for:

- Authoring (inspector configuration that bakes into components).
- Presentation bridges that read ECS state and mirror into GameObject proxies (e.g. `BloodlinesDebugEntityPresentationBridge`, `BloodlinesVisualPresentationBridge`).
- Editor tooling and validators.

Any other MonoBehaviour in runtime simulation is a red flag. Flag and refactor.

## Red flags (reject or refactor)

- `[SerializeField] private List<X> foo` on a new runtime class not under `Code/Authoring/` or `Code/Debug/`.
- `MonoBehaviour.Update` or `MonoBehaviour.FixedUpdate` containing simulation logic.
- `GameObject.FindObjectOfType<T>()` or `FindFirstObjectByType<T>()` in runtime simulation code (ok in editor tooling, validators, and bridges).
- `static` mutable state outside `Burst`-compatible read-only lookup tables.
- `EntityManager.AddComponent` / `DestroyEntity` inside a `SystemAPI.Query` loop without an ECB.
- Reading a buffer handle before performing a structural change, then using the handle after. This throws `ObjectDisposedException: Attempted to access ... which has been invalidated by a structural change.` Collect entity IDs first, perform structural changes after the query loop ends, then re-fetch the buffer.
- Managed class allocations in `OnUpdate` or any per-frame path.
- Reflection (`Type.GetMethod`, `Activator.CreateInstance`) in runtime simulation.
- `async / await` in runtime simulation. ECS does not use Task-based concurrency.
- LINQ on ECS collections. `Where / Select / OrderBy` over `NativeArray` or `DynamicBuffer` defeats Burst.

## Required additions per new system

When a new runtime system is created:

1. Decide `ISystem` vs `SystemBase`. Default `ISystem`.
2. Decide update group. Default `SimulationSystemGroup`.
3. Add `[BurstCompile]` at class and method level where the body is Burst-compatible.
4. Add `state.RequireForUpdate<PrimaryComponent>()` in `OnCreate`.
5. Register the `.cs` file in `unity/Assembly-CSharp.csproj` under an appropriate `<ItemGroup>`. Unity auto-regenerates the csproj, but local `dotnet build` validation requires the entry.
6. Add the system to any isolated-world validator (`CreateValidationWorld`) that needs to exercise it.
7. Add a canonical `.meta` file for the new `.cs` file. Unity creates this automatically in editor mode, but if creating files outside the editor, ensure a `.meta` with a stable GUID exists before commit.

## Required additions per new component

1. Place under `Code/Components/<Subsystem>/` or a subsystem folder (`Code/Conviction/`, `Code/Dynasties/`, etc).
2. Use unmanaged types only. Enums as `byte` unless more are needed.
3. Document the browser reference in an XML doc comment: `/// <summary> ... Browser reference: simulation.js:<line> <function>. </summary>`.
4. If the component is per-faction, read `FactionComponent` alongside it. If per-unit, read `UnitTypeComponent`. Keep the entity shape explicit.

## Report format

When invoked on a code review, return:

- `ECS PASS`: "Code follows DOTS/ECS discipline. No red flags."
- `ECS VIOLATION`: "Line <N>: <violation>. Expected: <correct pattern>." One per violation.

## What this skill does NOT do

- Does not check canon alignment (that's canon-enforcement).
- Does not check performance beyond basic idioms (that's performance-and-scale).
- Does not run validators.
