# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-14
Session: 10
Canonical root: `D:\ProjectsHome\Bloodlines`

## Scope Of This Addendum

Session 10 resolved the single-largest outstanding blocker on the project (Unity version alignment, unresolved since session 2) and advanced two browser-lane canonical mechanics that had been documented but not yet in runtime: sabotage operations and commander keep-presence sortie capability. This report records those three movements plus the ECS foundation authored alongside the version lock.

Sessions 4 through 9 remain authoritative for their dated slices. This report is additive.

## Top-Line Verdict

Unity production lane is unblocked. Browser reference simulation advanced two vectors simultaneously: Military (sabotage as first covert-operation type) and Dynastic (commander keep-presence sortie). Eight-layer attacker-side maturity in the browser siege model now exists: refusal → siege production → differentiated engines → siege lines → engineers → sustainment → supply-aware assault gating → sabotage-as-siege-accelerant.

## Unity Production Lane: UNBLOCKED

### Version alignment decision resolved

Lance authorized Option B. Unity 6.3 LTS (`6000.3.13f1`) is the locked editor for `unity/`.

- `unity/ProjectSettings/ProjectVersion.txt` updated from `6000.4.2f1` to `6000.3.13f1`. `m_EditorVersionWithRevision` marked `pending-first-open` until Lance opens the editor and Unity writes the full revision hash.
- `unity/Packages/manifest.json` adjusted to 6.3 LTS-compatible versions:
  - `com.unity.entities`: 6.4.0 → 1.4.0
  - `com.unity.collections`: 6.4.0 → 2.5.7
  - `com.unity.entities.graphics`: 6.4.0 → 1.4.0
  - `com.unity.render-pipelines.universal`: 17.4.0 → 17.3.0
  - `com.unity.inputsystem`: 1.19.0 → 1.11.2
  - `com.unity.addressables`: 2.9.1 → 2.5.0
  - Unchanged: burst 1.8.29, mathematics 1.3.3, test-framework 1.6.0, timeline 1.8.12, ugui 2.0.0, ide.rider 3.0.39, ide.visualstudio 2.0.27, feature.development 1.0.2
- On first open, Unity Package Manager may offer minor version adjustments from its current 6.3 LTS catalog. Accept LTS-compatible replacements. Do not downgrade below the floors above.
- `unity/README.md` Approved Toolchain section was rewritten to reflect the locked version. First-open workflow section added.
- `ENVIRONMENT_REPORT_2026-04-14.md` Section 4 has a new "Resolution (2026-04-14 Session 10)" block marking the decision locked.

### First ECS foundation authored

Fifteen canonical ECS component files authored under `unity/Assets/_Bloodlines/Code/Components/`:

- `PositionComponent.cs` — world-space position, `float3 Value`
- `FactionComponent.cs` + `FactionKindComponent.cs` — faction id (FixedString32) + enum kind (Kingdom / Tribes / Neutral)
- `HealthComponent.cs` + `DeadTag.cs` — current/max health + death finalization tag
- `UnitTypeComponent.cs` — type id, `UnitRole` enum (Worker, Melee, MeleeRecon, Ranged, UniqueMelee, LightCavalry, SiegeEngine, SiegeSupport, EngineerSpecialist, Support), `SiegeClass` enum (None, Ram, SiegeTower, Trebuchet, Ballista, Mantlet), population cost, stage
- `BuildingTypeComponent.cs` — type id, `FortificationRole` enum (None, Wall, Tower, Gate, Keep), structural damage multiplier, population cap bonus, blocks passage, supports siege preparation, supports siege logistics
- `ResourceNodeComponent.cs` — resource id + amount + initial amount
- `ControlPointComponent.cs` — control point id, owner, `ControlState` enum (Neutral, Occupied, Stabilized, Contested), loyalty, capture progress, settlement class, fort tier
- `SettlementComponent.cs` + `PrimaryKeepTag.cs` — settlement id, settlement class, fort tier + ceiling, primary-keep marker tag
- `CommanderComponent.cs` + `CommanderAtKeepTag.cs` — member id, role, renown, keep-presence tag (session 10 sortie capability)
- `BloodlineRoleComponent.cs` — `BloodlineMemberComponent` + `BloodlineRole` enum (9 canonical roles including Sorcerer) + `BloodlineStatus` enum + `BloodlinePath` enum (7 training paths)
- `FaithComponent.cs` — `FaithStateComponent` + `FaithExposureElement` buffer + `FaithWardedSettlementTag` + `CovenantId` enum (None, OldLight, BloodDominion, TheOrder, TheWild) + `DoctrinePath` enum (Light, Dark)
- `ConvictionComponent.cs` — four-bucket ledger + score + `ConvictionBand` enum (five canonical bands) — INDEPENDENT of faith per master doctrine section XX
- `RealmConditionComponent.cs` — `RealmConditionComponent` (per-faction realm cycle accumulator + strain streaks) + `RealmCycleConfig` (canonical thresholds singleton)
- `PopulationComponent.cs` — `PopulationComponent` + `ResourceStockpileComponent` (six primaries + influence)
- `SiegeComponent.cs` — `SiegeEngineStateComponent` (supply + engineer support timestamps) + `MantletCoverComponent` (cover radius + inbound ranged multiplier)

Three canonical ECS systems authored under `unity/Assets/_Bloodlines/Code/Systems/`:

- `BloodlinesBootstrapSystem.cs` — one-shot singleton seeding of `RealmCycleConfig` with canonical defaults (90s cycle, famine/water/loyalty/population thresholds) if the importer has not already written them
- `RealmConditionCycleSystem.cs` — canonical 90-second realm cycle ISystem; advances accumulator, accrues famine + water-crisis streaks at cycle close, decays assault cohesion strain
- `PopulationGrowthSystem.cs` — canonical 18-second population growth gated by housing cap AND food AND water, consuming 1 food and 1 water per head

These are Burst-compatible `ISystem` structs (with Bootstrap as a `SystemBase` for the one-shot singleton creation). They are the reference pattern for every future Unity system. On Lance's first open of `unity/` under 6.3 LTS, these will compile against the DOTS 1.4 API.

### What Lance does on first open

1. Open `unity/` in Unity 6.3 LTS (6000.3.13f1).
2. Accept any LTS-compatible package version adjustments from the Package Manager.
3. Let Unity regenerate `Library/`. First compile includes the session 10 ECS layer.
4. Run `Bloodlines → Import → Sync JSON Content` to generate ScriptableObject assets.
5. Commit the generated `.asset` files.
6. Create `Scenes/Bootstrap/Bootstrap.unity`, seed with the authored systems and a subscene.
7. Open or create `Scenes/Gameplay/IronmarkFrontier.unity`.
8. Hit Play. Expect the bootstrap system to create the `RealmCycleConfig` singleton. Realm cycle and population growth will not yet produce visible output until authoring/baking and data-seeding scenes land (planned session 14 per the roadmap).

## Browser Reference Simulation Lane

### Sabotage operations — first covert-operation type live

Session 10 expanded `dynasty.operations` from captivity-only (ransom + rescue + demand) to also include sabotage against a rival faction's buildings. Canonical reference: master doctrine section VIII + `08_MECHANICS/OPERATIONS_SYSTEM.md`.

Four canonical sub-types live:

- **gate_opening** — targets a gatehouse. On success, target health drops to 20% of max and a 15-second exposed window is recorded. Breach-accelerant operation.
- **fire_raising** — targets any enemy building. On success, applies 8 damage-per-second burn for 10 seconds via the new `tickBuildingStatusEffects` loop.
- **supply_poisoning** — targets a supply camp (must expose `supportsSiegeLogistics`). On success, marks the camp `poisonedUntil = elapsed + 20`. Session 8's supply-wagon-to-camp linkage now excludes poisoned camps from the link filter, so the siege chain loses continuity for 20 seconds.
- **well_poisoning** — targets any settlement anchor. On success, adds two water-strain cycles to the target faction's `waterStrainStreak`, biasing the next realm cycle toward water crisis.

Canonical shape:

- Costs: gate_opening 60g+18i, fire_raising 40g+12i, supply_poisoning 50g+15i, well_poisoning 70g+20i.
- Durations: gate_opening 28s, fire_raising 24s, supply_poisoning 30s, well_poisoning 32s.
- Requires spymaster-role bloodline member (or equivalent covert-ops path member) active in the source dynasty.
- Success formula: spymaster renown + 45 (offense) vs target fort tier × 12 + ward active × 15 + target spymaster presence × 10 (defense). Projected chance clamped to [0.05, 0.95].
- Escrow: cost is charged on start. On failure, cost is not refunded.
- Counterplay: failed sabotage records +1 stewardship conviction on the target faction and pushes a player-visible detection message.
- Conviction bookkeeping on success: supply_poisoning and well_poisoning record desecration; gate_opening and fire_raising record ruthlessness.

New public entry point: `export function startSabotageOperation(state, factionId, subtype, targetFactionId, targetBuildingId)`.

New support plumbing:

- `tickBuildingStatusEffects(state, dt)` called in `stepSimulation` between `updateBuildings` and `tickFortificationReserves`. Applies burn damage, cleans expired burn/poison/gate-exposed windows.
- `tickSiegeSupportLogistics` linkedCamp filter now excludes camps whose `poisonedUntil` is in the future. Sabotage-to-siege interplay emerges naturally: if a player (or AI, once AI sabotage lands) successfully poisons the enemy supply camp, the siege engines go unsupplied and lose operational efficiency per the session 8 mathematics.

### Commander keep-presence sortie — session 10 capability

Canonical reference: master doctrine section XII (commanders and generals must matter in both delegated and direct warfare) + `01_CANON/DEFENSIVE_FORTIFICATION_DOCTRINE.md` layered-defense pillar.

New mechanic:

- When the commander is at the primary keep AND the keep is under active threat AND the sortie cooldown is not active, the player can call a sortie.
- Duration: 12 seconds. Cooldown: 60 seconds.
- During an active sortie, defenders within the fortification local-combat profile gain ×1.22 attack and +22 sight on top of their ward-and-presence buffs.
- Sortie records a +1 oathkeeping conviction event when called.
- New public entry point: `export function issueKeepSortie(state, factionId, settlementId)`.
- Exports: `SORTIE_DURATION_SECONDS` and `SORTIE_COOLDOWN_SECONDS`.

### Snapshot extension

`getRealmConditionSnapshot`'s fortification block now exposes:

- `commanderAtKeep` — explicit commander-at-keep flag (mirrors `commanderPresent` for clearer downstream naming)
- `sortieActive` — true while a sortie window is open
- `sortieCooldownRemaining` — seconds until next sortie is callable
- `sortieReady` — composite flag (cooldown expired AND commander at keep AND threat present)

These support the dynasty panel and future keep-interior UI surfacing the sortie action to the player.

### Tests extended

`tests/runtime-bridge.mjs` session 10 additions:

- Sabotage rejects unknown sub-type.
- Sabotage `gate_opening` against a non-gate building rejects with canonical reason.
- Sabotage `fire_raising` against an enemy command hall succeeds and escrows both gold and influence.
- Sortie refuses when commander is not at the keep.
- Snapshot exposes all four new fortification fields (`commanderAtKeep`, `sortieActive`, `sortieReady`, `sortieCooldownRemaining`) and defaults `sortieActive` to false on a fresh simulation.

## Verification

```
node tests/data-validation.mjs       → Bloodlines data validation passed.
node tests/runtime-bridge.mjs        → Bloodlines runtime bridge validation passed.
node --check src/game/main.js                   → OK
node --check src/game/core/simulation.js        → OK
node --check src/game/core/renderer.js          → OK
node --check src/game/core/ai.js                → OK
node --check src/game/core/data-loader.js       → OK
node --check src/game/core/utils.js             → OK

Live browser at http://localhost:8057/play.html
  11 pressure pills render: CONFIRMED (Cycle, Pop, Food, Water, Loyalty, Fort, Army, Faith, Conviction, Logistics, World)
  Console errors: ZERO
  Failed network requests: ZERO
```

## Gap Analysis Reclassification

Session 10 moves the following items in the session 9 gap analysis matrix:

| System | Previous | Current |
|---|---|---|
| Sabotage (gate opening, fire raising, supply poisoning, well poisoning) | DOCUMENTED | LIVE (browser reference simulation) |
| Covert operations expansion in `dynasty.operations` | DOCUMENTED | PARTIAL (sabotage live; assassination, espionage still DOCUMENTED) |
| Commander keep-presence expansion beyond reserve layer | DOCUMENTED | LIVE (sortie capability with duration + cooldown + snapshot surface) |
| Unity version alignment decision | OPEN BLOCKER | RESOLVED (Option B, Unity 6.3 LTS locked) |
| Unity ECS Components (Position, Faction, Health, UnitType, BuildingType, ResourceNode, ControlPoint, Settlement, Commander, Bloodline, Faith, Conviction, Population, Resources, Siege, RealmCondition) | PENDING | AUTHORED (15 component files, awaits first Unity open) |
| Unity ECS Systems (RealmConditionCycle, PopulationGrowth, Bootstrap) | PENDING | AUTHORED (3 system files, awaits first Unity open) |

## Top Remaining Structural Deficits After Session 10

1. **Unity first-open verification.** ECS code is authored but has not yet been compiled by Unity. Session 13 scope is to open Unity, accept package resolutions, run JSON sync, commit ScriptableObjects, and verify the authored systems compile cleanly.
2. **Longer-siege AI.** Stonehelm still doesn't adapt to relief-window pressure, doesn't handle post-repulse recovery, and doesn't run supply-protection patrols.
3. **Second playable house.** Still Ironmark-only. Session 11 scope.
4. **Faith prototype enablement.** `faiths.json` still has all four covenants `prototypeEnabled: false`. No faith-specific unit rosters (L3, L4, L5). No apex structures.
5. **Dual-clock declaration seam.** No post-battle in-world time declaration yet.
6. **Continental / naval foundation.** Still single-continent. No water tiles. No harbor. No vessel classes in data.
7. **Ironmark Blood Production loop depth.** Still partial. Canonical Ironmark identity mechanic.
8. **Hartvale Verdant Warden.** Still absent from `data/units.json`.
9. **AI sabotage.** Stonehelm does not yet run sabotage against the player. The player can sabotage; the AI cannot. Reciprocity pending.

## Drift Watches

- Sabotage cost/duration tuning should be revisited once the full operations system lands. Current numbers are canonical starting points, not final balance.
- Sortie duration (12s) and cooldown (60s) are first-pass. Canonical leverage should be reassessed after the AI can threaten a keep long enough for sortie timing to matter.
- Unity package version drift: Unity's 6.3 LTS catalog may differ by a patch version from what's written in the manifest. If Unity refuses to resolve on first open, check the Package Manager's suggested versions and update `manifest.json` accordingly.

## Session 11 Next Action

Per the session 9 execution roadmap, session 11 advances:

1. Enable Stonehelm as second playable house with minimal URL-driven house-select (`play.html?house=stonehelm`).
2. Add Hartvale Verdant Warden to `data/units.json` (house-specific, Off 4 / Def 5).
3. Optionally, add AI sabotage reciprocity so Stonehelm can run sabotage against the player.

## Preservation Statement

No canonical system was reduced, substituted, or sidelined. All nine founding houses, four faiths, six vessel classes, continental world architecture, five-stage match structure, dual-clock architecture, Decision 21 fortification doctrine, multi-dimensional land and naval doctrine, and every other preserved pillar remain protected. Session 10 advanced from PARTIAL / DOCUMENTED toward LIVE. Nothing regressed.
