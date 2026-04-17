# Bloodlines Browser to ECS System Map

This file maps the current browser update loop to Unity ECS systems and execution order.

Reference browser files:
- `deploy/bloodlines/src/game/core/simulation.js`
- `deploy/bloodlines/src/game/core/ai.js`

## 1. Browser Loop Reference

The browser runtime currently advances by a mostly linear simulation order:

1. Population cap and derived faction state
2. Passive resource trickle
3. Population growth
4. Control point ownership and loyalty
5. Sacred site faith exposure and commitment availability
6. Building production and construction
7. Unit behavior, worker logic, combat logic
8. Projectile motion and impacts
9. AI updates
10. Message expiry and presentation cleanup

Unity should preserve this dependency order even if individual jobs run in parallel.

## 2. Proposed ECS Groups

Recommended high-level grouping:
- `BloodlinesInitializationGroup`
- `BloodlinesSimulationGroup`
- `BloodlinesAIGroup`
- `BloodlinesPresentationGroup`

Suggested order:
1. Initialization
2. Simulation
3. AI
4. Presentation

## 3. System Mapping

| Browser function / concern | Unity system |
|---|---|
| initial map bootstrap | `SkirmishBootstrapSystem` |
| population cap recalculation | `PopulationCapSystem` |
| passive farms and wells trickle | `PassiveResourceSystem` |
| population growth tick | `PopulationGrowthSystem` |
| control point contest and flip logic | `ControlPointSystem` |
| loyalty decay and stabilization | `ControlPointLoyaltySystem` |
| sacred site exposure | `FaithExposureSystem` |
| doctrine/intensity tier updates | `FaithIntensitySystem` |
| production queue progress | `BuildingProductionSystem` |
| construction progress | `ConstructionSystem` |
| worker command execution | `WorkerCommandSystem` |
| move command steering/pathing | `MoveCommandSystem` |
| target acquisition | `AutoAcquireSystem` |
| melee attack resolution | `MeleeCombatSystem` |
| ranged spawn | `RangedAttackSystem` |
| projectile movement and hit resolution | `ProjectileSystem` |
| enemy kingdom economy and attack timing | `EnemyKingdomAISystem` |
| neutral tribe raid timing | `NeutralRaidAISystem` |
| message TTL cleanup | `MessageDecaySystem` |

## 4. Critical Dependencies

Hard ordering requirements:
- `PopulationCapSystem` before `PopulationGrowthSystem`
- `ConstructionSystem` before `BuildingProductionSystem`
- `MoveCommandSystem` before combat range checks
- `AutoAcquireSystem` before `MeleeCombatSystem` and `RangedAttackSystem`
- `FaithExposureSystem` before any faith-commit UI sync
- `EnemyKingdomAISystem` after economy and production state are current
- `MessageDecaySystem` after gameplay systems can emit messages

## 5. Browser AI Translation

The current browser AI does four meaningful things that must survive the port:

Enemy kingdom:
- Assign idle workers to gold and wood.
- Build barracks, dwellings, farms, and wells based on resource and cap state.
- Queue villagers and combat units based on affordability and capacity.
- Periodically push armies to control points and eventually attack the player hall.

Neutral tribes:
- Fire a raid timer.
- Move raiders to the nearest non-tribe control point or command hall.
- Emit raid messages on trigger.

Unity AI parity is not achieved unless these behaviors are observable without player input.

## 6. Pathing Transition

Before U9:
- `MoveCommandSystem` may use simple steering that mirrors browser behavior.

At U9:
- Replace pure steering with grid plus waypoint pathing.
- Preserve the public command semantics so higher-level systems do not need to change.

## 7. Presentation Systems

Presentation is downstream of simulation. It should read, not own, gameplay truth.

Presentation concerns:
- HUD resource panels
- selection rings
- minimap
- dynasty panel
- faith panel
- message log
- control point state markers

These should hang off ECS data via UI Toolkit or GameObject presentation bridges, not duplicate gameplay state in MonoBehaviours.

## 8. Fortification and Siege Systems (2026-04-14)

Source: `01_CANON/DEFENSIVE_FORTIFICATION_DOCTRINE.md` (locked 2026-04-14). Defender specification: `04_SYSTEMS/FORTIFICATION_SYSTEM.md`. Attacker specification: `04_SYSTEMS/SIEGE_SYSTEM.md`.

### 8.1 Fortification systems (defender)

- **FortificationTierSystem** — owns fortification-tier metadata on control points, settlements, and keeps. Reads construction and resource state; writes tier transitions when upgrade requirements are satisfied.
- **LayeredDefenseSystem** — manages outer works, inner ring, and final core state per settlement. Tracks breach state per layer.
- **WallSegmentDamageSystem** — segment-level structural damage for walls, gates, and towers. Reads damage events from combat resolution; writes repair events from engineer activity.
- **GarrisonReserveCycleSystem** — rotates garrison troops between active-defense and triage-recovery. Reads combat engagement state; writes unit command updates.
- **SignalNetworkSystem** — watchfire, bell, horn relay infrastructure. Reads threat detection events; writes cross-settlement alerts.
- **BloodlinePresenceBonusSystem** — applies defensive leverage bonuses when dynasty members are in the keep. Reads dynasty attachments from DynastyState; writes combat bonus modifiers to garrison entities.
- **FaithWardedSanctumSystem** — covenant-specific defensive effects from apex-tier faith buildings inside fortifications. Reads FaithState intensity and doctrine; writes defensive modifiers.
- **AssaultFailurePenaltySystem** — applies canonical wave-spam denial math after repulsed assaults. Reads assault resolution events; writes attacker morale, cohesion, and supply degradations.
- **FortificationRecoverySystem** — post-assault repair window and reinforcement arrival timing. Reads FortificationDamage state; writes repair progress and reinforcement events.

### 8.2 Siege systems (attacker)

- **SiegeEngineSystem** — owns siege engine entities (rams, siege towers, trebuchets, ballistae, mantlets, bombards). Reads build orders; writes siege engine state, damage output, and movement.
- **SiegeEngineerSystem** — specialist engineer population distinct from line infantry. Reads recruitment orders; writes engineering entity state, earthwork construction, mining/counter-mining progress.
- **SiegeSupplyLineSystem** — supply camps, supply wagons, foraging rights, rear-area security. Reads supply consumption events; writes siege force sustain state.
- **SiegeScoutingSystem** — intelligence on fortification weaknesses, garrison patterns, reserve locations, supply status. Reads scout unit observations; writes visible-intelligence state for AI and player.
- **BreachPlanningSystem** — target section selection, approach preparation, supporting fires, assault force staging, exploitation. Reads scouting intelligence; writes assault state machine transitions.
- **SabotageOperationSystem** — covert operations as siege enablers (gate-opening, fire-raising, supply poisoning, defector turning, counter-mining sabotage). Reads spymaster operations orders; writes defender weakness events.
- **MultiFrontCoordinationSystem** — diversionary assaults, multi-gate simultaneity, outside pressure. Reads army assignments; writes synchronized assault events.
- **IsolationSystem** — road interdiction, reinforcement interception, depot destruction, political isolation. Reads attacker blockade entities; writes defender relief-capability degradation.
- **FaithSiegeOperationSystem** — covenant-sanctioned siege operations at Fervent+ intensity. Reads attacker FaithState; writes siege force multipliers.

### 8.3 Cross-system dependencies

- **FortificationTierSystem ↔ DynastyState**: fortification tier modifies bloodline presence bonus magnitudes.
- **LayeredDefenseSystem ↔ TerritoryControlSystem**: layer breach does not immediately flip control; ownership change requires final core reduction.
- **AssaultFailurePenaltySystem ↔ ConvictionLedger**: failed assaults add desecration (wave-spam penalty) or ruthlessness (scale of attack).
- **SiegeEngineSystem ↔ SiegeSupplyLineSystem**: siege engines consume replacement parts and ammunition.
- **SabotageOperationSystem ↔ SpymasterState**: sabotage operations require spymaster availability and cost influence.
- **FaithSiegeOperationSystem ↔ FaithWardedSanctumSystem**: attacker faith powers face defender faith wards; contested resolution.
- **BloodlinePresenceBonusSystem ↔ 2026-04-14 Dynasty Consequence Cascade**: commander capture, governor loss, and head-of-bloodline fall update attachments which feed directly into fortification leverage.

### 8.4 Dependency ordering (canonical)

Within a tick, the canonical order is: scouting → breach planning → siege engine activity (damage to walls) → wall segment damage → assault resolution (combat contact) → garrison reserve cycling response → assault failure penalties (if repulsed) OR layer breach update (if successful) → fortification recovery → bloodline presence bonus refresh → signal network propagation → faith-warded sanctum effects refresh.

### 8.5 Implementation status

All fortification and siege systems are CANONICAL and IMPLEMENTATION PENDING as of 2026-04-14. The Dynasty Consequence Cascade (implemented 2026-04-14) provides the dynasty-side hooks that fortification bonuses will attach to. No fortification or siege code exists in the browser runtime yet. Unity migration plan phases U17-U22 cover the full rollout.

### 8.6 Non-negotiables

Implementation must honor the Defensive Fortification Doctrine's ten pillars (canonical lock 2026-04-14). No system may be implemented in a reduced form that contradicts any pillar. Where a pillar implies a mathematical model (assault failure penalties, attritional cost multiplier, tempo drag), the tuning values live in the data layer and can evolve; the existence of the mechanic cannot be reduced to zero without explicit authorization from Lance.
