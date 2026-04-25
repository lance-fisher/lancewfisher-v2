# Unity Port Backlog

**Date:** 2026-04-25
**Author:** Claude Code (claude-sonnet-4-6)
**Purpose:** Prioritized backlog of Unity implementation items recovered from browser work and gap analysis. Each item is derived from browser-era design intelligence, canon documents, or gaps identified in the feature map.
**Status as of:** Branch `claude/unity-multiplayer-nfe-integration`

## 2026-04-25 Session Closure Summary

This session closed the following backlog items:

- P1 Balance Constant Parity Audit (commit 65c9e729) — 122 constants surveyed, 0 drift found.
- P2 Stonehelm Faction Bonuses Wired (commit afd64bba) — fortification cost discount + build speed bonus.
- P2 Stage Gate Cross-Audit (commits b755d8c8 + 921f9314) — D2-1 + D4-1 + D5-1 + D5-2 applied; D3-3 deferred fix also applied.
- P2 Unity-Canonical Advancements Canon Doc (commit b755d8c8).
- P2 Iron Mine Smelting Fuel Consumption (commit b755d8c8).
- P2 Holy War Runtime Effects (commit b755d8c8) — verified existing implementation, no code change.
- P3 Naval Layer slices S1, S2, S3, S4, S5 (commits d3657660, f41c820d, 38463327, 60e58e4d, 208b8fc3).
- P3 Neutral Faction AI — Tribes raid driver (commit ba63dbce). Browser ai.js:updateNeutralAi raid loop ported with world-pressure + Great Reckoning multipliers. Trueborn-City-style true-neutral driver still pending.

Refactor + housekeeping:
- GovernanceCanon hoist (d57ab8f3) — 17 territorial-governance constants.
- MatchProgressionCanon hoist (2ceee99b) — Great Reckoning + phase thresholds.
- Editor csproj re-canonicalization (a28ff9cb).
- Continuity files refreshed multiple times.

Remaining P1: Worker Slot Assignment HUD (UI work; needs in-game click panel), Multiplayer NfE Ghost prefabs (needs interactive Unity open).
Remaining naval slices: S6 (optional) AI naval dispatch.
Remaining P3 spec-blocked: Born of Sacrifice, Victory Conditions 4-6.
Remaining AI: dark-extremes Apex Cruel raid multiplier (depends on conviction-band aggregate); true-neutral negotiation-only driver.

---

## Priority Definitions

| Priority | Meaning |
|---|---|
| P1 | Blocks gameplay loop or core session milestone. Implement next available session. |
| P2 | High design value, clear path, no blockers. Implement soon. |
| P3 | Medium value, needs spec or minor dependencies first. |
| P4 | Valuable but deferred. Implement when surrounding systems are ready. |
| P5 | Long-term or architectural. Plan before touch. |

---

## Backlog

### P1: Worker Slot Assignment HUD

| Field | Value |
|---|---|
| **Priority** | P1 |
| **Target Area** | UI/HUD |
| **Recovered Feature** | Worker-slot production model (Unity-canonical, data in buildings.json). Player has no UI to interact with AssignedWorkers. |
| **Why It Matters** | WorkerSlotProductionSystem is live and consuming AssignedWorkers. Without a UI, the slot mechanic is invisible and the economy loop has a dead zone. |
| **Suggested Implementation Path** | Select WorkerSlotBuildingComponent-bearing building → bottom HUD panel → Worker assignment row showing "Assigned: X / Max: Y" with + and - buttons. Write-through to AssignedWorkers field. Uses existing Unity UI Toolkit. RA2 per-building slot model (not StarCraft global priority). |
| **Dependencies** | None. WorkerSlotBuildingComponent and WorkerSlotProductionSystem already exist. |
| **Risk** | Low. Pure additive UI layer. |
| **Status** | Not started |

---

### P1: Multiplayer NfE Package + Ghost Prefab Population

| Field | Value |
|---|---|
| **Priority** | P1 (listed as P1 in HANDOFF.md) |
| **Target Area** | Multiplayer/ |
| **Recovered Feature** | N/A (new Unity feature) |
| **Why It Matters** | This branch (`claude/unity-multiplayer-nfe-integration`) was created specifically for NfE. Foundation exists. Next slice is the package install. |
| **Suggested Implementation Path** | Install `com.unity.netcode` package → populate GhostPrefabArchetypeElement buffer for 3 archetypes (faction/control_point/unit) → prove 2-client local-loopback skirmish |
| **Dependencies** | Current branch uncommitted changes must be validated and committed first. |
| **Risk** | Medium. Package version compatibility needs testing. |
| **Status** | Foundation committed this branch. Next: package install. |

---

### P1: Balance Constant Parity Audit

| Field | Value |
|---|---|
| **Priority** | P1 |
| **Target Area** | All subsystems |
| **Recovered Feature** | 80+ tuned numeric constants from simulation.js (lines 1-450). |
| **Why It Matters** | These are battle-tested balance values from 96+ additive realization sessions. If Unity canon values drift from them without intent, balance tests will fail in unexpected ways. |
| **Suggested Implementation Path** | Create `docs/migration/constant_parity_audit.md`. For each browser constant, list: browser value, Unity canon file, Unity value, status (Match/Drift/Missing). Prioritize fortification constants, faith constants, legitimacy deltas. |
| **Dependencies** | None. Research task. |
| **Risk** | Low (research only). Drift found will be fixed in a follow-up. |
| **Status** | COMPLETE (2026-04-25). See `docs/migration/constant_parity_audit.md`. 122 constants surveyed across 14 domains. 64 Match (in canon class), 21 Match (inline), 0 Drift, 14 Missing (clustered around faction Legitimacy field deferred to dynasty-core lane, naval S4/S5, scout node harass, ledger history limits), 23 N/A. |

---

### P2: Stonehelm Faction Bonuses Wired

| Field | Value |
|---|---|
| **Priority** | P2 |
| **Target Area** | Construction/ + Fortification/ |
| **Recovered Feature** | `data/houses.json: Stonehelm.mechanics.fortificationCostMultiplier=0.80, fortificationBuildSpeedMultiplier=1.20` |
| **Why It Matters** | Faction differentiation is a core design pillar. Stonehelm's fortification identity is settled canon. Data exists but no system reads these multipliers. |
| **Suggested Implementation Path** | In `AdvanceFortificationTierSystem`, read `FactionHouseComponent.HouseId` → look up `HouseDefinition` via `BloodlinesDefinitions.Instance.GetHouseDefinition(houseId)` → multiply cost by `fortificationCostMultiplier` and build progress rate by `fortificationBuildSpeedMultiplier`. |
| **Dependencies** | `BloodlinesDefinitions` registry must be accessible from `AdvanceFortificationTierSystem`. Pattern exists in other systems. |
| **Risk** | Low. Additive. |
| **Status** | COMPLETE (2026-04-25, commit afd64bba). HouseDefinition fields + HouseMechanicsRecord JSON import + HouseMechanicsComponent + ConstructionSystem build-speed scale + DebugCommandSurface cost-discount scale all landed. Build speed applied per dt when BuildingTypeComponent.FortificationRole != None. Cost scaled at affordability check + SpendCost via integer round. |

---

### P2: Stage Gate Cross-Audit

| Field | Value |
|---|---|
| **Priority** | P2 |
| **Target Area** | Time/ |
| **Recovered Feature** | Browser `stageTwoRequirements`, `stageThreeRequirements`, `stageFourRequirements`, `stageFiveRequirements` arrays |
| **Why It Matters** | Stage transitions control faith unit unlocks, governance victory eligibility, Trueborn rise eligibility. Wrong gates create pacing problems. |
| **Suggested Implementation Path** | Side-by-side read of `simulation.js:updateMatchProgressionState` and `Time/MatchProgressionEvaluationSystem.cs`. Document any deviations. Fix deviations if found. |
| **Dependencies** | None. Diagnostic first, code only if deviation found. |
| **Risk** | Low (diagnostic). |
| **Status** | COMPLETE (2026-04-25, commit b755d8c8). Audit at reports/2026-04-25_match_progression_stage_gate_cross_audit.md. Applied: D2-1 defended-seat health gate, D4-1 sustainedWar canonical signals, contestedBorder world-pressure fallback, D5-1 split convergence/sovereignty, D5-2 enemy convergence + divine right. Deferred: D3-3 military combat-only filter, D4-1 rivalContact full 5-signal port, D4-2 perf rework. |

---

### P2: Unity-Canonical Advancements Canon Document

| Field | Value |
|---|---|
| **Priority** | P2 |
| **Target Area** | `01_CANON/` |
| **Recovered Feature** | 5% active-duty labor, military draft, cross-match XP, worker slot model, trade routes. All Unity-canonical, not in browser. |
| **Why It Matters** | Future session agents will read the browser spec and not find these mechanics. Without a canon declaration document, they may incorrectly infer the browser is the ground truth for these areas. |
| **Suggested Implementation Path** | Create `01_CANON/UNITY_CANONICAL_ADVANCEMENTS_2026-04-25.md` listing: mechanic name, canon owner direction citation, Unity file location, what the browser says (nothing), confirmed status. |
| **Dependencies** | None. |
| **Risk** | None. Documentation only. |
| **Status** | COMPLETE (2026-04-25, commit b755d8c8). 01_CANON/UNITY_CANONICAL_ADVANCEMENTS_2026-04-25.md documents 5 mechanics with canon citations, Unity implementation pointers, and rationale. |

---

### P2: Iron Mine Smelting Fuel Consumption (COMPLETE 2026-04-25, commit b755d8c8)

Status: SmeltingComponent + MapBuildingSeedElement extension + WorkerGatherSystem deposit-time fuel deduction implemented.

| Field | Value |
|---|---|
| **Priority** | P2 |
| **Target Area** | Systems/ (WorkerSlotProductionSystem or WorkerGatherSystem) |
| **Recovered Feature** | Browser: iron mine consumes `smeltingFuelRatio=0.5` wood per iron gathered. Insufficient wood returns ore to node. |
| **Why It Matters** | Without smelting cost, iron production has no wood dependency. This breaks the intended resource interdependency (wood for construction AND smelting). |
| **Suggested Implementation Path** | In `WorkerGatherSystem` or `WorkerSlotProductionSystem`, when producing iron: check faction wood stockpile >= (iron produced × 0.5); if yes, deduct wood and produce iron; if no, produce at reduced rate or zero. |
| **Dependencies** | `ResourceStockpileComponent` must be accessible. |
| **Risk** | Low. Additive check. |
| **Status** | Not started |

---

### P2: Holy War Runtime Economic/Combat Effects (VERIFIED 2026-04-25, commit b755d8c8 — already implemented)

Status: AIHolyWarResolutionSystem.TickActiveHolyWars (Phase B) confirmed canonical. Verification at reports/2026-04-25_holy_war_runtime_effects_verification.md.

| Field | Value |
|---|---|
| **Priority** | P2 |
| **Target Area** | Faith/ |
| **Recovered Feature** | Browser `tickFaithHolyWars`: per-second combat bonuses and economic effects applied to belligerent factions during active holy war. |
| **Why It Matters** | Without runtime effects, holy war is a flag declaration with no gameplay feel during the active period. |
| **Suggested Implementation Path** | Verify first: does any existing system tick holy war runtime effects? If not, add a `HolyWarRuntimeEffectsSystem` that reads `HolyWarActiveComponent` and applies: attack bonus to belligerents vs. each other, loyalty pressure on contested CPs, conviction events on sustained actions. |
| **Dependencies** | Must verify gap before coding. May already be covered. |
| **Risk** | Low if gap is confirmed. Medium risk of duplicate logic if not verified first. |
| **Status** | Verify gap, then implement if missing |

---

### P3: Born of Sacrifice — Spec Lock First

| Field | Value |
|---|---|
| **Priority** | P3 (requires owner spec) |
| **Target Area** | Dynasties/ + Population/ |
| **Recovered Feature** | Design canon only. No browser code, no Unity code. Institutional memory lifecycle mechanic. |
| **Why It Matters** | Core dynasty-army circularity that makes losing veterans meaningful. Without it, military is a flat disposable resource. |
| **Suggested Implementation Path (when spec arrives)** | `BornOfSacrificeRequestComponent` (player intent) → `BornOfSacrificeResolutionSystem` (retire veterans, pay population cost, reduce active-duty count) → `InstitutionalMemoryComponent` on newly-trained squads (training cost reduction or unit-tier accelerator) → `BornOfSacrificeEffectsCanon` (faith variant multipliers). |
| **Dependencies** | Owner spec on: trigger, institutional memory numerics, faith variants, conviction cost, population/legitimacy cost, per-match limits. |
| **Risk** | High if specced incorrectly. Low if specced well. |
| **Status** | Pending owner spec |

---

### P3: Victory Conditions 4-6 (Economic Dominance + Alliance Victory)

| Field | Value |
|---|---|
| **Priority** | P3 (requires owner decision on ship scope) |
| **Target Area** | Victory/ |
| **Recovered Feature** | `data/victory-conditions.json`: economic_dominance (disabled), alliance_victory (disabled). Currently `prototypeEnabled=false` in browser. |
| **Why It Matters** | Full 6-condition victory set provides multiple viable playstyles per canon design. Currently 3 of 6 implemented. |
| **Suggested Implementation Path** | Add `VictoryConditionId.EconomicDominance` and `AllianceVictory` to enum. Add evaluation paths in `VictoryConditionEvaluationSystem`. Define thresholds in canon first (resource stockpile amounts, sustained duration, ally scope). |
| **Dependencies** | Owner decision on: (1) ship with 3 or 6? (2) economic dominance thresholds? (3) alliance victory scope? |
| **Risk** | Low (additive evaluation paths) if spec is clear. |
| **Status** | Pending owner decision |

---

### P3: Neural Faction AI (Neutral Driver)

| Field | Value |
|---|---|
| **Priority** | P3 |
| **Target Area** | AI/ |
| **Recovered Feature** | Browser `ai.js: updateNeutralAi` — dedicated neutral faction AI driver separate from `updateEnemyAi`. Neutral factions behave differently: they don't attack, they negotiate, they respond to governance pressure. |
| **Why It Matters** | Neutral factions (including Trueborn City early game) need distinct AI behavior. If they use the enemy AI driver, they'll attack and ruin the early-game pacing. |
| **Suggested Implementation Path** | Add `AIFactionPostureComponent` (Neutral/Hostile/Coalition). `EnemyAIStrategySystem` reads posture to gate attack logic. Neutral posture: only governance pressure response, marriage proposals, pact acceptance. |
| **Dependencies** | Verify whether Unity currently has neutral posture gating in `EnemyAIStrategySystem`. |
| **Risk** | Medium. Behavior testing required. |
| **Status** | PARTIAL 2026-04-25 (commit ba63dbce). Tribes raid driver ported (TribesRaidStateComponent + TribesRaidSystem + smoke validator). Browser ai.js:updateNeutralAi raid loop landed: 30s timer, raiders dispatched to nearest non-tribes-owned CP, world-pressure + Great Reckoning multipliers. Deferred: dark-extremes multiplier (needs dynasty conviction-band aggregate), Trueborn-City-style true-neutral (negotiation-only) AI, world-pressure-leader-targeted raid prioritization. |

---

### P3: Naval Layer — Embark/Disembark + Combat

| Field | Value |
|---|---|
| **Priority** | P3 (requires UX spec) |
| **Target Area** | New: Naval/ (or extend Combat/) |
| **Recovered Feature** | Browser sessions 27/28, 96: full naval integration. 6 vessel types in units.json. transport_ship capacity=6. fire_ship oneUseSacrifice=true. |
| **Why It Matters** | Naval is a fully-settled mechanic. Vessel definitions exist. Without systems, naval units are data-only decorations. |
| **Suggested Implementation Path (when spec arrives)** | `NavalEmbarkRequestComponent` → `NavalEmbarkSystem` (load up to capacity=6) → `NavalDisembarkSystem` (on shoreline) → `FireShipDetonationSystem` → extend `Combat/` for naval vs. naval. |
| **Dependencies** | Owner spec on embark/disembark UX model. |
| **Risk** | Medium. New sub-system. |
| **Status** | PARTIALLY COMPLETE 2026-04-25. S1 embark (d3657660), S2 disembark + water-tile detection (f41c820d), S3 fire-ship detonation (38463327), S5 fishing gather (60e58e4d) all landed. Smoke validator runs 4 phases green. Remaining: S4 vessel-vs-vessel naval combat (separate acquisition + damage tables), S6 (optional) AI naval dispatch. |

---

### P4: Pathfinding Upgrade (Grid/Waypoint)

| Field | Value |
|---|---|
| **Priority** | P4 |
| **Target Area** | Pathing/ |
| **Recovered Feature** | Browser uses simple vector movement. Unity currently has only `PositionToLocalTransformSystem` + `UnitMovementSystem` (simple steering). |
| **Why It Matters** | Simple steering will break at Bloodlines unit counts (100+ units per faction). Required for any meaningful scale. |
| **Suggested Implementation Path** | Grid-based pathfinding with DOTS-compatible burst-compiled A* or flow field. Was designated U9 target in `docs/unity/MIGRATION_PLAN.md`. Consider Unity DOTS Navigation or custom flow field. |
| **Dependencies** | Map tile system must be settled. Requires planning session before touch. |
| **Risk** | High. Architectural change to movement model. |
| **Status** | U9 target — not yet started |

---

### P4: AI Behavioral Parity Audit

| Field | Value |
|---|---|
| **Priority** | P4 |
| **Target Area** | AI/ |
| **Recovered Feature** | Browser `ai.js: updateEnemyAi` (~1700 LOC monolithic function with all enemy AI logic). Unity has 14+ per-operation lanes. |
| **Why It Matters** | Browser AI was tuned across 96 sessions. Unity AI is structurally different. Without a parity audit, we can't know if Unity AI produces equivalent strategic behavior. |
| **Suggested Implementation Path** | Scenario-driven comparison: define 5-10 scenario starting states. Run each in browser, record AI decisions at T=60s, T=120s, T=300s. Compare to Unity AI decisions from same states. Document gaps. Fix missing decision paths. |
| **Dependencies** | Unity must reach a playable skirmish state first. |
| **Risk** | Low (research only). Fixes based on findings will have varied risk. |
| **Status** | Not started. Deferred until Unity reaches playable skirmish. |

---

### P4: Graphics Asset Production

| Field | Value |
|---|---|
| **Priority** | P4 |
| **Target Area** | Art/ |
| **Recovered Feature** | 37 SVG concept sheets in graphics pipeline. 0 approved assets. 0 production-candidate assets. |
| **Why It Matters** | Shipping target requires visual assets. Current state is concept-pass only. |
| **Suggested Implementation Path** | Per graphics pipeline plan: select approved concepts, produce AI-generated assets at Zero Hour / Warcraft III fidelity ceiling, import to Unity, bind to visual definitions. |
| **Dependencies** | Requires asset pipeline execution session. Graphics lane plan is documented. |
| **Risk** | Medium. Art direction approval needed before production. |
| **Status** | Pipeline ready. Execution not started. |

---

### P5: Divine Right + Holy War Runtime Ticking Audit

| Field | Value |
|---|---|
| **Priority** | P5 |
| **Target Area** | Faith/ + AI/ |
| **Recovered Feature** | Browser `tickFaithDivineRightDeclarations`, `tickFaithHolyWars`: per-second runtime effects during active declarations. |
| **Why It Matters** | These special states should have measurable gameplay impact during the active period, not just at declaration and resolution. |
| **Suggested Implementation Path** | Verify both runtime-ticking systems exist. If not, add them as part of a faith-effects review session. |
| **Dependencies** | Existing faith system must be stable. |
| **Risk** | Low if systems already exist. Medium if they need to be added (overlap risk with existing systems). |
| **Status** | Pending verification |

---

## Quick Reference Summary

| Priority | Item | Effort |
|---|---|---|
| P1 | Worker Slot Assignment HUD | 1-2 sessions |
| P1 | NfE Package Install + Ghost Prefabs | 1-2 sessions |
| P1 | Balance Constant Parity Audit | 1 session (research) |
| P2 | Stonehelm Faction Bonuses Wired | < 1 session |
| P2 | Stage Gate Cross-Audit | < 1 session |
| P2 | Unity-Canonical Advancements Canon Doc | < 1 session |
| P2 | Iron Mine Smelting Fuel | < 1 session |
| P2 | Holy War Runtime Effects (verify first) | 0-1 session |
| P3 | Born of Sacrifice (spec → impl) | 2-3 sessions after spec |
| P3 | Victory Conditions 4-6 | 1-2 sessions after spec |
| P3 | Neutral Faction AI (verify first) | 1 session |
| P3 | Naval Layer | 2-3 sessions after spec |
| P4 | Pathfinding Upgrade | Planning session + 3-5 implementation sessions |
| P4 | AI Behavioral Parity Audit | 1-2 sessions (research) |
| P4 | Graphics Asset Production | Graphics pipeline execution sessions |
| P5 | Divine Right + Holy War Runtime Audit | Fold into faith review session |
