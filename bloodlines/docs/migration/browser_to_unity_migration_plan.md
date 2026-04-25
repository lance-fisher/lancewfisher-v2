# Browser-to-Unity Migration Plan

**Date:** 2026-04-25
**Author:** Claude Code (claude-sonnet-4-6)
**Purpose:** Safe, additive migration plan for moving useful browser-era design and code into the Unity production path.
**Note:** This document replaces the stale `docs/unity/MIGRATION_PLAN.md` which was written on 2026-04-13 when "Unity direction is documented but not yet started." Unity is now in mid-early production with 30+ subsystem folders. That document is preserved as-is for historical reference; this document is the active migration plan.

---

## Migration Philosophy

The browser simulation is a frozen behavioral specification. It is not the implementation to continue. The goal of this migration plan is:

1. Confirm that Unity already implements what the browser demonstrated
2. Identify what the browser had that Unity does not yet have
3. Safely extend Unity with missing pieces, documented or coded
4. Never disrupt active Unity development
5. Never introduce browser-specific architecture into Unity

**Default posture:** Document and backlog rather than force code. Code only when the Unity architecture clearly supports it and the change is additive and compile-safe.

---

## Migration Tier Definitions

| Tier | Description | Action |
|---|---|---|
| T0 | Already fully implemented in Unity | Verify constants only. No code changes. |
| T1 | Unity is ahead of browser | Document Unity-canonical advancement. Freeze browser as reference. |
| T2 | High-value gap with clear Unity implementation path | Design the Unity slice, create components/systems. |
| T3 | Valuable but needs design confirmation first | Write design spec, backlog the implementation. |
| T4 | Browser-only artifact | Archive reference only. No Unity action. |

---

## T0: Already Fully Implemented (Verify Constants Only)

These systems are complete in Unity. The only remaining work is a constant-parity audit between the browser's numeric block and Unity's canon values.

| System | Unity Location | Audit Action |
|---|---|---|
| Faith (4 covenants, 5 tiers, covenant tests) | `Faith/` folder | Verify exposure threshold=100, test thresholds (80/180d/120d cooldown) |
| Conviction (4 buckets, 5 bands, score) | `Conviction/` folder | Verify band thresholds match browser exactly |
| Dynasty (succession chain, marriage, legitimacy, renown, aging) | `Dynasties/` folder | Verify RENOWN_CAP=100, marriage 90d/280d, legitimacy deltas match browser |
| Combat (melee, ranged, siege, commander aura) | `Combat/` folder | Verify commander aura radius base=126 |
| Fortification + reserves | `Fortification/` folder | Verify strain threshold=6, triage heal=5.5/s, retreat health=0.42, recovery=0.82 |
| Siege supply logistics | `Siege/` folder | Verify unsupplied attack multiplier=0.84, speed=0.88 |
| Scout raids | `Raids/` folder | Verify raid resource loss values (wood=14, food=10-12, gold=10, stone=12, iron=10) |
| Territory + loyalty | `TerritoryGovernance/` + `WorldPressure/` | Verify stabilized=72, governance victory=90, acceptance=65%/60% |
| Covert ops | `PlayerCovertOps/` + AI covert-op lanes | Verify ransom/rescue durations and costs |
| Match phases + dual clock | `Time/` folder | Verify 5-stage structure and gate requirements match browser |
| World pressure + Trueborn Rise | `WorldPressure/` folder | Verify trigger=0.70, release=0.66, pressure score=4 |
| Economy trickle | `Economy/` folder | Verify farm=0.5 food/s, well=0.45 water/s |
| Population growth | `Systems/` folder | Verify interval=18s, cost=1 food + 1 water per person |
| Save/Load | `SaveLoad/` folder | No audit needed |
| Realm conditions | `Economy/` folder | No audit needed |

**Recommended action:** Create a focused constant-parity audit script or document that lists all 80+ browser constants side-by-side with their Unity locations. This is a research task for one session, not a code change.

---

## T1: Unity-Canonical Advancements (Document Only)

These mechanics exist in Unity but NOT in the browser. The browser is frozen and was not updated to reflect them. They are canonical per owner direction and should be documented as such, not treated as Unity deviations.

| Mechanic | Unity Location | Canon Source | Documentation Needed |
|---|---|---|---|
| 5% active-duty labor contribution | `EarlyGameConstants.ProductivityTrainedActiveDuty=0.05f` | Owner direction 2026-04-25 | Add note to `01_CANON/` designating 5% as canonical starting value |
| 4-tier productivity model (civilian/untrained/reserve/active-duty) | `PopulationProductivitySystem` | Owner direction 2026-04-25 | Confirm canonical productivity weights in `01_CANON/` |
| Military draft slider (0-100% in 5% steps) | `MilitaryDraftSystem` | Owner direction 2026-04-25 | Confirm draft model canonical in `01_CANON/` |
| Cross-match dynasty XP progression (tiers 1-4, unit swaps) | `DynastyProgressionCanon` | Owner direction 2026-04-19 | Confirm tier XP thresholds canonical in `01_CANON/` |
| Worker-slot building production | `WorkerSlotProductionSystem` | Owner direction 2026-04-25 | Confirm slot model canonical in `01_CANON/`; note browser has slot data in JSON but no browser implementation |
| Trade routes (5 gold/route/day) | `TradeRouteEvaluationSystem` | Unity-canonical advancement | Document as Unity-canonical advancement. |

**Recommended action:** Create `01_CANON/UNITY_CANONICAL_ADVANCEMENTS_2026-04-25.md` listing these 6 mechanics as post-browser-freeze canon, with owner direction citations.

---

## T2: High-Value Gaps with Clear Unity Path (Implement)

These items are missing from Unity, valuable for the full shipping game, and have a clear DOTS/ECS implementation path that fits the existing architecture.

### T2-A: Worker Slot Assignment HUD (High Priority)

**What:** Player-facing UI panel for assigning workers to slot-based buildings (lumber camp, forager camp, small farm, mine works).

**Why:** The slot production system is implemented and canon. Without a UI surface, players have no way to interact with it. This is a day-1 gameplay loop hole.

**Implementation path:**
```
Unity location: unity/Assets/_Bloodlines/Code/UI/HUD/WorkerSlotAssignmentHUDSystem.cs
Approach:
  1. Extend building selection HUD to check for WorkerSlotBuildingComponent
  2. Show "Workers: X / Y" with + and - buttons
  3. + button: increment AssignedWorkers up to MaxWorkerSlots (if idle workers available)
  4. - button: decrement AssignedWorkers (releases to idle pool)
  5. WorkerSlotProductionSystem already picks up AssignedWorkers each tick
  
UI Toolkit approach recommended (matches Zero Hour fidelity era).
```

**Dependencies:** None. Component and system already exist.
**Risk:** Low. Pure additive UI layer.
**Estimated effort:** 1-2 session slices.

### T2-B: Stonehelm Faction Bonuses Wired to Systems

**What:** `HouseDefinition.fortificationCostMultiplier=0.80` and `fortificationBuildSpeedMultiplier=1.20` exist in data but no Unity system reads them.

**Why:** Faction differentiation is a core design pillar. Stonehelm's fortification identity is settled canon.

**Implementation path:**
```
1. Read FactionHouseComponent.HouseId in AdvanceFortificationTierSystem
2. Look up HouseDefinition via BloodlinesDefinitions registry
3. Apply fortificationCostMultiplier to resource cost calculation
4. Apply fortificationBuildSpeedMultiplier to construction progress rate
Same pattern for ConstructionSystem where applicable.
```

**Dependencies:** Requires `BloodlinesDefinitions` registry access from `AdvanceFortificationTierSystem`. This pattern already exists in other systems.
**Risk:** Low.
**Estimated effort:** Less than 1 session.

### T2-C: Match Stage Gate Requirement Cross-Audit

**What:** The browser has `stageTwoRequirements`, `stageThreeRequirements`, `stageFourRequirements`, `stageFiveRequirements` arrays with specific conditions. Unity's `MatchProgressionEvaluationSystem` may or may not use the same conditions.

**Why:** Stage transitions control when faith units unlock, when governance victory is checkable, when world pressure can trigger. Getting this wrong creates pacing problems that are hard to debug later.

**Implementation path:** Read `MatchProgressionEvaluationSystem.cs` and compare gate conditions to browser `updateMatchProgressionState` logic. Document any deviations. Fix deviations if needed.

**Dependencies:** None.
**Risk:** Low.
**Estimated effort:** Half a session.

### T2-D: Holy War Runtime Economic/Combat Effects

**What:** Browser's `tickFaithHolyWars` applies per-second economic and combat effects to belligerent factions during an active holy war. Unity has holy war declaration + dispatch + resolution but may not tick runtime effects.

**Why:** Without runtime effects, holy war is just a flag declaration. Players feel nothing during the holy war period.

**Implementation path:**
```
If HolyWarActiveComponent is present on two factions:
  - Apply per-second: combat attack bonus to belligerents vs. each other
  - Apply per-second: loyalty pressure on contested CPs
  - Apply conviction events on sustained holy war actions
May already be in FaithDoctrineSyncSystem or a similar system. Verify first.
```

**Dependencies:** Verify the gap exists before implementing. May already be covered.
**Risk:** Low if gap is confirmed.
**Estimated effort:** 0-1 session depending on gap size.

---

## T3: Needs Design Lock Before Implementation

These items cannot be implemented without an owner specification decision. The implementation path is clear, but the mechanics are not fully specified.

### T3-A: Born of Sacrifice (Medium-High Priority)

**What:** Population-constrained army lifecycle mechanic. Veterans retire; new wave is trained; institutional memory is inherited.

**Why:** Core dynasty-army circularity. Absence creates a flat military model where armies are disposable and there's no meaningful cost to losing veterans.

**Spec needed:**
- Trigger: player-initiated retirement vs. automatic on upgrade?
- Institutional memory mechanic: training cost reduction? unit-tier accelerator? renown meta-stat?
- Faith variants: which covenants grant what sacrifice bonuses?
- Cooldowns and per-match limits?
- Conviction implications (does sacrificing shift conviction bands)?
- Bloodline member involvement rules?

**Placeholder ECS surface when spec arrives:**
```csharp
BornOfSacrificeRequestComponent.cs   // player intent
BornOfSacrificeResolutionSystem.cs   // veteran retirement + population cost
InstitutionalMemoryComponent.cs       // on newly-trained units, carries forward bonuses
BornOfSacrificeEffectsCanon.cs       // numeric modifiers (faith variants, faith-conviction table)
```

### T3-B: Victory Conditions 4-6 (Medium Priority)

**What:** Economic Dominance, Alliance Victory, and potentially Military Conquest as a separate path from CommandHallFall.

**Why:** Full 6-condition victory set is canon. Currently only 3 of 6 are implemented.

**Spec needed:**
- Economic Dominance: what resource stockpile thresholds? sustained how long?
- Alliance Victory: which faction or combination? loyalty threshold?
- Does "military_conquest" differ from CommandHallFall in any way?

**Placeholder when spec arrives:** Add new `VictoryConditionId` enum values + evaluation paths in `VictoryConditionEvaluationSystem`.

### T3-C: Naval Layer (Medium Priority)

**What:** Embark/disembark for transport_ship (capacity 6), fire ship one-use detonation, naval combat lane, fishing gather.

**Why:** Naval integration is canon (Sessions 27/28, 96 in browser). Unit definitions are in data. No systems exist.

**Spec needed:**
- Embark/disembark UX: order on transport selected + shoreline right-click? or drag unit to transport icon?
- Fire ship detonation: player-triggered or auto-proximity?
- Naval combat: melee only, or ranged from ships?
- Fishing gather: passive trickle or worker-walk model?

**Placeholder when spec arrives:**
```csharp
NavalEmbarkRequestComponent.cs
NavalEmbarkSystem.cs
NavalDisembarkSystem.cs
FireShipDetonationSystem.cs
NavalCombatSystem.cs
```

---

## T4: Browser-Only Artifacts (Archive, No Unity Action)

| Item | Why Not Migrated |
|---|---|
| `main.js` game loop + UI shell | Browser DOM/canvas architecture |
| `renderer.js` Canvas2D layer | Unity has its own renderer |
| `data-loader.js` JSON fetch | Unity uses ScriptableObject registry |
| Browser `state.ui.*` fields | No equivalent in Unity ECS |
| `localStorage` usage | Not applicable |
| HTML/CSS/layout | Not applicable |
| React-style component patterns | Not applicable |

---

## Safe Migration Actions This Session

The following additive documentation artifacts are safe to create without Unity code changes:

1. `reports/browser_to_unity_inventory.md` ← already created
2. `reports/unity_current_state_inventory.md` ← already created
3. `reports/browser_to_unity_gap_analysis.md` ← already created
4. `docs/migration/browser_reference_extraction.md` ← already created
5. `docs/migration/browser_to_unity_migration_plan.md` ← this file
6. `docs/migration/browser_to_unity_feature_map.md` ← pending
7. `docs/migration/unity_port_backlog.md` ← pending

No Unity C# code changes are made in this migration session. All identified gaps are captured in documentation for future implementation sessions.

---

## Risks and Protections

| Risk | Protection |
|---|---|
| Constant drift between browser and Unity | Dedicated constant-parity audit session (T0 action) |
| Unity developer adds a system that duplicates a browser concept without knowing | This report package (session agents read it at start) |
| Born of Sacrifice spec ambiguity leads to wrong implementation | T3 gate: no code before spec is locked |
| Worker slot assignment UX mismatched to design intent | Confirm RA2 per-building model with owner before building UI |
| Naval layer coded against wrong UX model | T3 gate: no code before embark/disembark UX spec |
| AI behavioral drift between browser and Unity | Behavioral parity audit recommended before shipping AI AI to players |
| Balance constant drift discovered after shipping | Constant-parity audit now; regression tests later |

---

## Validation Steps After Any Unity Code Changes

Before committing any Unity code that touches this migration work:

1. `dotnet build unity/Assembly-CSharp.csproj -nologo` — 0 errors required
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` — 0 errors required
3. `powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1`
4. `powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1`
5. `powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1`
6. `node tests/data-validation.mjs`
7. `node tests/runtime-bridge.mjs`
8. `powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1`

If any smoke script fails, do not commit. Fix the failure first.

---

## Next Session Priorities

Recommended order for the next implementation session:

1. Worker Slot Assignment HUD (T2-A) — most impactful, no design lock needed, clear path
2. Stonehelm faction bonuses (T2-B) — small, additive, high faction-differentiation value
3. Stage gate cross-audit (T2-C) — diagnostic, low risk, high pacing importance
4. Holy war runtime effects (T2-D) — verify first, implement only if gap confirmed
5. Born of Sacrifice spec discussion with owner (T3-A) — prerequisite to any code
