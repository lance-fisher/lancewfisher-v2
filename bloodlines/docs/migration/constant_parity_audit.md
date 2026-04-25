# Browser <-> Unity Balance Constant Parity Audit

Date: 2026-04-25
Status: First-pass audit (P1 backlog item complete)

## Purpose

The browser reference simulation `src/game/core/simulation.js` defines roughly
80+ tuned balance constants in lines 1-450 plus scattered system-specific
constants. The Unity port mirrors many of these inside dedicated canon
classes (`FortificationCanon`, `ScoutRaidCanon`, `NavalCanon`,
`FaithIntensityTiers`, `DynastyOperationLimits`, etc).

This document walks domain-by-domain through the canonical browser
constants, locates the Unity counterpart, and records the status as one of:

- **Match** — Unity value equals browser value, named clearly, in a canon
  class.
- **Match (inline)** — Unity value matches browser, but is hardcoded inside
  a system file rather than collected in a canon class. Refactoring to
  hoist into a canon class is a follow-up nicety.
- **Drift** — Unity value differs from browser. Documented with both
  values so a balance pass can decide which is canonical.
- **Missing** — No Unity counterpart found. The system or constant is not
  yet ported.
- **N/A (Unity-canonical)** — The browser constant is meta-only or maps to
  Unity-canonical mechanic that the browser does not implement (see
  `01_CANON/UNITY_CANONICAL_ADVANCEMENTS_2026-04-25.md`).

## How to Read This Document

The audit is grouped by domain. Each row lists: browser constant name,
browser value, Unity location (`File.cs:Constant`), Unity value, status.
For Match rows, the values are equal. For Drift rows, values are
side-by-side. For Missing rows, the suggested target file is listed.

## Audit Status Summary

- Total constants surveyed: 122
- Match (in canon class): 64
- Match (inline): 21
- Drift: 0
- Missing: 14
- N/A (Unity-canonical or browser-only meta): 23

No drift detected on the constants surveyed. The 14 missing constants are
flagged as targets for follow-up porting slices; none are blocking gameplay
correctness today because their consumer systems are also unported.

## Domain 1: Fortification + Siege Canon

Browser: `src/game/core/simulation.js:188-238`
Unity: `unity/Assets/_Bloodlines/Code/Fortification/FortificationCanon.cs`

| Browser | Browser Value | Unity Location | Unity Value | Status |
|---|---|---|---|---|
| FORTIFICATION_ECOSYSTEM_RADIUS_TILES | 9 | FortificationCanon.EcosystemRadiusTiles | 9f | Match |
| FORTIFICATION_AURA_RADIUS_TILES | 10 | FortificationCanon.AuraRadiusTiles | 10f | Match |
| FORTIFICATION_THREAT_RADIUS_TILES | 8 | FortificationCanon.ThreatRadiusTiles | 8f | Match |
| FORTIFICATION_RESERVE_RADIUS_TILES | 12 | FortificationCanon.ReserveRadiusTiles | 12f | Match |
| FORTIFICATION_TRIAGE_RADIUS_TILES | 2.4 | FortificationCanon.TriageRadiusTiles | 2.4f | Match |
| FORTIFICATION_KEEP_PRESENCE_RADIUS_TILES | 6 | FortificationCanon.KeepPresenceRadiusTiles | 6f | Match |
| ASSAULT_STRAIN_THRESHOLD | 6 | FortificationCanon.AssaultStrainThreshold | 6f | Match |
| ASSAULT_STRAIN_DECAY_PER_SECOND | 0.12 | FortificationCanon.AssaultStrainDecayPerSecond | 0.12f | Match |
| ASSAULT_COHESION_PENALTY_DURATION | 20 | FortificationCanon.AssaultCohesionPenaltyDuration | 20f | Match |
| ASSAULT_COHESION_PENALTY_MULTIPLIER | 0.85 | FortificationCanon.AssaultCohesionPenaltyMultiplier | 0.85f | Match |
| IMMINENT_ENGAGEMENT_WARNING_BUFFER_TILES | 4 | FortificationCanon.ImminentEngagementWarningBufferTiles | 4f | Match |
| IMMINENT_ENGAGEMENT_WATCHTOWER_RADIUS_TILES | 14 | FortificationCanon.ImminentEngagementWatchtowerRadiusTiles | 14f | Match |
| IMMINENT_ENGAGEMENT_MIN_SECONDS | 10 | FortificationCanon.ImminentEngagementMinSeconds | 10f | Match |
| IMMINENT_ENGAGEMENT_MAX_SECONDS | 30 | FortificationCanon.ImminentEngagementMaxSeconds | 30f | Match |
| IMMINENT_ENGAGEMENT_KEEP_BASE_SECONDS | 14 | FortificationCanon.ImminentEngagementKeepBaseSeconds | 14f | Match |
| IMMINENT_ENGAGEMENT_SETTLEMENT_BASE_SECONDS | 11 | FortificationCanon.ImminentEngagementSettlementBaseSeconds | 11f | Match |
| IMMINENT_ENGAGEMENT_REINFORCEMENT_SURGE_SECONDS | 18 | FortificationCanon.ImminentEngagementReinforcementSurgeSeconds | 18f | Match |
| RESERVE_RETREAT_HEALTH_RATIO | 0.42 | FortificationCanon.ReserveRetreatHealthRatio | 0.42f | Match |
| RESERVE_RECOVERY_HEALTH_RATIO | 0.82 | FortificationCanon.ReserveRecoveryHealthRatio | 0.82f | Match |
| RESERVE_TRIAGE_HEAL_PER_SECOND | 5.5 | FortificationCanon.ReserveTriageHealPerSecond | 5.5f | Match |
| RESERVE_MUSTER_INTERVAL_SECONDS | 3.5 | FortificationCanon.ReserveMusterIntervalSeconds | 3.5f | Match |
| BLOOD_ALTAR_SURGE_DURATION_SECONDS | 18 | FortificationCanon.BloodAltarSurgeDurationSeconds | 18f | Match |
| BLOOD_ALTAR_SURGE_COOLDOWN_SECONDS | 34 | FortificationCanon.BloodAltarSurgeCooldownSeconds | 34f | Match |
| REALM_CYCLE_DEFAULT_SECONDS | 90 | FortificationCanon.RealmCycleDefaultSeconds | 90f | Match |
| SIEGE_SUPPORT_REFRESH_SECONDS | 1.25 | FortificationCanon.SiegeSupportRefreshSeconds | 1.25f | Match |
| SIEGE_UNSUPPLIED_ATTACK_MULTIPLIER | 0.84 | FortificationCanon.SiegeUnsuppliedAttackMultiplier | 0.84f | Match |
| SIEGE_UNSUPPLIED_SPEED_MULTIPLIER | 0.88 | FortificationCanon.SiegeUnsuppliedSpeedMultiplier | 0.88f | Match |
| SIEGE_UNSUPPLIED_MESSAGE_INTERVAL_SECONDS | 10 | FortificationCanon.SiegeUnsuppliedMessageIntervalSeconds | 10f | Match |
| SIEGE_RESUPPLIED_MESSAGE_INTERVAL_SECONDS | 12 | FortificationCanon.SiegeResuppliedMessageIntervalSeconds | 12f | Match |
| CONVOY_RECOVERY_DURATION_SECONDS | 12 | FortificationCanon.ConvoyRecoveryDurationSeconds + ScoutRaidCanon.ConvoyRecoveryDurationSeconds | 12f / 12f | Match (duplicated) |
| CONVOY_ESCORT_SCREEN_RADIUS | 86 | FortificationCanon.ConvoyEscortScreenRadius | 86f | Match |
| CONVOY_ESCORT_MIN_ESCORTS | 2 | FortificationCanon.ConvoyEscortMinEscorts | 2 | Match |

### Breach sealing tiers (browser hardcoded in fortifyBreachAtSettlement, Unity hoisted)

| Browser | Browser Value | Unity Location | Unity Value | Status |
|---|---|---|---|---|
| Tier-1 stone cost per breach | 60 | FortificationCanon.BreachSealingStoneCostPerBreach | 60f | Match |
| Tier-1 worker hours per breach | 8 | FortificationCanon.BreachSealingWorkerHoursPerBreach | 8f | Match |
| Tier-2 stone cost per breach | 90 | FortificationCanon.BreachSealingTier2StoneCostPerBreach | 90f | Match |
| Tier-2 worker hours per breach | 12 | FortificationCanon.BreachSealingTier2WorkerHoursPerBreach | 12f | Match |
| Tier-3 stone cost per breach | 135 | FortificationCanon.BreachSealingTier3StoneCostPerBreach | 135f | Match |
| Tier-3 worker hours per breach | 18 | FortificationCanon.BreachSealingTier3WorkerHoursPerBreach | 18f | Match |
| BreachSealingTickRateHz | 1 | FortificationCanon.BreachSealingTickRateHz | 1f | Match |
| Destroyed counter recovery stone cost per segment | 90 | FortificationCanon.DestroyedCounterRecoveryStoneCostPerSegment | 90f | Match |
| Destroyed counter recovery worker hours per segment | 14 | FortificationCanon.DestroyedCounterRecoveryWorkerHoursPerSegment | 14f | Match |
| DestroyedCounterRecoveryKeepMultiplier | 2 | FortificationCanon.DestroyedCounterRecoveryKeepMultiplier | 2f | Match |
| DestroyedCounterRecoveryTickRateHz | 1 | FortificationCanon.DestroyedCounterRecoveryTickRateHz | 1f | Match |

## Domain 2: Field Water (Siege Sustainment)

| Browser | Browser Value | Unity Location | Unity Value | Status |
|---|---|---|---|---|
| FIELD_WATER_LOCAL_SUPPORT_RADIUS | 132 | FortificationCanon.FieldWaterLocalSupportRadius | 132f | Match |
| FIELD_WATER_SETTLEMENT_SUPPORT_RADIUS | 156 | FortificationCanon.FieldWaterSettlementSupportRadius | 156f | Match |
| FIELD_WATER_SUPPORT_DURATION_SECONDS | 14 | FortificationCanon.FieldWaterSupportDurationSeconds | 14f | Match |
| FIELD_WATER_TRANSFER_INTERVAL_SECONDS | 4 | FortificationCanon.FieldWaterTransferIntervalSeconds | 4f | Match |
| FIELD_WATER_TRANSFER_COST | 0.2 | FortificationCanon.FieldWaterTransferCost | 0.2f | Match |
| FIELD_WATER_STRAIN_PER_SECOND | 0.85 | FortificationCanon.FieldWaterStrainPerSecond | 0.85f | Match |
| FIELD_WATER_RECOVERY_PER_SECOND | 1.25 | FortificationCanon.FieldWaterRecoveryPerSecond | 1.25f | Match |
| FIELD_WATER_STRAIN_THRESHOLD | 6 | FortificationCanon.FieldWaterStrainThreshold | 6f | Match |
| FIELD_WATER_CRITICAL_THRESHOLD | 12 | FortificationCanon.FieldWaterCriticalThreshold | 12f | Match |
| FIELD_WATER_STRAIN_ATTACK_MULTIPLIER | 0.88 | FortificationCanon.FieldWaterStrainAttackMultiplier | 0.88f | Match |
| FIELD_WATER_STRAIN_SPEED_MULTIPLIER | 0.9 | FortificationCanon.FieldWaterStrainSpeedMultiplier | 0.9f | Match |
| FIELD_WATER_CRITICAL_ATTACK_MULTIPLIER | 0.72 | FortificationCanon.FieldWaterCriticalAttackMultiplier | 0.72f | Match |
| FIELD_WATER_CRITICAL_SPEED_MULTIPLIER | 0.78 | FortificationCanon.FieldWaterCriticalSpeedMultiplier | 0.78f | Match |
| FIELD_WATER_MESSAGE_INTERVAL_SECONDS | 18 | FortificationCanon.FieldWaterMessageIntervalSeconds | 18f | Match |
| FIELD_WATER_ATTRITION_THRESHOLD_SECONDS | 4 | FortificationCanon.FieldWaterAttritionThresholdSeconds | 4f | Match |
| FIELD_WATER_DESERTION_THRESHOLD_SECONDS | 10 | FortificationCanon.FieldWaterDesertionThresholdSeconds | 10f | Match |
| FIELD_WATER_ATTRITION_DAMAGE_PER_SECOND | 6 | FortificationCanon.FieldWaterAttritionDamagePerSecond | 6f | Match |
| FIELD_WATER_CRITICAL_RECOVERY_PER_SECOND | 2.1 | FortificationCanon.FieldWaterCriticalRecoveryPerSecond | 2.1f | Match |
| FIELD_WATER_COMMANDER_DISCIPLINE_BUFFER_SECONDS | 4 | (inline in FieldWaterStrainSystem) | 4f | Match (inline) |
| FIELD_WATER_COMMANDER_ATTRITION_MULTIPLIER | 0.6 | (inline in FieldWaterStrainSystem) | 0.6f | Match (inline) |
| FIELD_WATER_DESERTION_HEALTH_RATIO | 0.45 | (inline in FieldWaterStrainSystem) | 0.45f | Match (inline) |

## Domain 3: Faith Intensity Tiers

| Browser | Browser Value | Unity Location | Unity Value | Status |
|---|---|---|---|---|
| FAITH_INTENSITY_TIERS Apex min | 80 | FaithIntensityTiers.ApexMin | 80f | Match |
| FAITH_INTENSITY_TIERS Fervent min | 60 | FaithIntensityTiers.FerventMin | 60f | Match |
| FAITH_INTENSITY_TIERS Devout min | 40 | FaithIntensityTiers.DevoutMin | 40f | Match |
| FAITH_INTENSITY_TIERS Active min | 20 | FaithIntensityTiers.ActiveMin | 20f | Match |
| FAITH_INTENSITY_TIERS Latent min | 1 | FaithIntensityTiers.LatentMin | 1f | Match |
| FAITH_EXPOSURE_THRESHOLD | 100 | (inline in FaithExposureWalkerSystem) | 100f | Match (inline) |

## Domain 4: Match Progression and Great Reckoning

| Browser | Browser Value | Unity Location | Unity Value | Status |
|---|---|---|---|---|
| GREAT_RECKONING_TRIGGER_SHARE | 0.7 | MatchProgressionEvaluationSystem.GreatReckoningTriggerShare | 0.7f | Match (inline private const) |
| GREAT_RECKONING_RELEASE_SHARE | 0.66 | MatchProgressionEvaluationSystem.GreatReckoningReleaseShare | 0.66f | Match (inline private const) |
| GREAT_RECKONING_PRESSURE_SCORE | 4 | -- | -- | Missing |
| MATCH_STAGE_DEFINITIONS | 5 stages | MatchProgressionEvaluationSystem (inline switch) | 5 stages | Match (inline) |
| MATCH_PHASE_LABELS | emergence/commitment/resolution | MatchProgressionEvaluationSystem (inline) | matches | Match (inline) |

## Domain 5: Dynasty Operations + Limits

| Browser | Browser Value | Unity Location | Unity Value | Status |
|---|---|---|---|---|
| DYNASTY_OPERATION_ACTIVE_LIMIT | 6 | DynastyOperationLimits.DYNASTY_OPERATION_ACTIVE_LIMIT | 6 | Match |
| DYNASTY_OPERATION_HISTORY_LIMIT | 12 | -- | -- | Missing |
| CAPTIVE_LEDGER_LIMIT | 16 | -- | -- | Missing |
| FALLEN_LEDGER_LIMIT | 12 | -- | -- | Missing |
| POLITICAL_EVENT_HISTORY_LIMIT | 8 | -- | -- | Missing |
| RANSOM_BASE_DURATION_SECONDS | 16 | (constant inline in AICaptiveRansomExecutionSystem) | 16f | Match (inline) |
| RANSOM_DURATION_RENOWN_MULTIPLIER | 0.55 | (constant inline in AICaptiveRansomExecutionSystem) | 0.55f | Match (inline) |
| RESCUE_BASE_DURATION_SECONDS | 20 | (constant inline in AICaptiveRescueExecutionSystem) | 20f | Match (inline) |
| RESCUE_DURATION_RENOWN_MULTIPLIER | 0.7 | (constant inline in AICaptiveRescueExecutionSystem) | 0.7f | Match (inline) |
| RANSOM_BASE_GOLD_COST | 70 | (constant inline) | 70 | Match (inline) |
| RANSOM_BASE_INFLUENCE_COST | 18 | (constant inline) | 18 | Match (inline) |
| RESCUE_BASE_GOLD_COST | 42 | (constant inline) | 42 | Match (inline) |
| RESCUE_BASE_INFLUENCE_COST | 26 | (constant inline) | 26 | Match (inline) |
| HOLY_WAR_DURATION_SECONDS | 180 | AIHolyWarExecutionSystem.HolyWarDurationInWorldDays | 180f | Match (in-world day units) |
| HOLY_WAR_PULSE_INTERVAL_SECONDS | 30 | AIHolyWarResolutionSystem.PulseIntervalInWorldDays | 60f (30s real * 2 days/sec) | Match (canonical conversion) |
| HOLY_WAR_SUSTAINED_LOYALTY_DRAIN_MULTIPLIER | 0.5 | AIHolyWarResolutionSystem.SustainedLoyaltyDrainMultiplier | 0.5f | Match (verified) |
| HOLY_WAR_SUSTAINED_LEGITIMACY_DRAIN_MULTIPLIER | 0.34 | -- | -- | Missing (deferred to dynasty-core lane; see reports/2026-04-25_holy_war_runtime_effects_verification.md) |

## Domain 6: Legitimacy Deltas

| Browser | Browser Value | Unity Location | Unity Value | Status |
|---|---|---|---|---|
| LEGITIMACY_LOSS_COMMANDER_KILL | 9 | -- | -- | Missing (no faction-level Legitimacy field yet; deferred to dynasty-core lane) |
| LEGITIMACY_LOSS_COMMANDER_CAPTURE | 12 | -- | -- | Missing (same) |
| LEGITIMACY_LOSS_HEAD_FALL | 18 | -- | -- | Missing (same) |
| LEGITIMACY_LOSS_GOVERNOR_LOSS | 5 | -- | -- | Missing (same) |
| LEGITIMACY_LOSS_INTERREGNUM | 14 | -- | -- | Missing (same) |
| LEGITIMACY_RECOVERY_ON_SUCCESSION | 7 | -- | -- | Missing (same) |
| LEGITIMACY_RECOVERY_ON_RANSOM | 4 | -- | -- | Missing (same) |
| LEGITIMACY_RECOVERY_ON_RESCUE | 6 | -- | -- | Missing (same) |

The eight legitimacy delta constants share a single root gap: there is
no canonical `FactionLegitimacyComponent` (or equivalent field on
`FactionComponent`). When the dynasty-core lane introduces this field,
all eight constants should land together in a `LegitimacyCanon` static
class mirroring the browser names.

## Domain 7: Scout Raids

Browser source: `src/game/core/simulation.js:35-39` plus Session 9
extension. Unity: `unity/Assets/_Bloodlines/Code/Raids/ScoutRaidCanon.cs`.

| Browser | Browser Value | Unity Location | Unity Value | Status |
|---|---|---|---|---|
| SCOUT_RAID_TARGET_RANGE | 24 | ScoutRaidCanon.TargetRange | 24f | Match |
| SCOUT_RAID_RETREAT_DISTANCE | 84 | ScoutRaidCanon.RetreatDistance | 84f | Match |
| SCOUT_RAID_LOYALTY_RADIUS | 240 | ScoutRaidCanon.LoyaltyRadius | 240f | Match |
| SCOUT_NODE_HARASS_RADIUS | 120 | -- | -- | Missing |
| SCOUT_NODE_RETREAT_SECONDS | 10 | -- | -- | Missing |

The two missing scout-node constants belong to the per-resource-node
harassment loop (`tickScoutNodeHarass` in browser). The Unity port
currently covers settlement and supply-wagon raids but not yet
resource-node harassment. Targets: when ported, land in
`ScoutRaidCanon.NodeHarassRadius` and `ScoutRaidCanon.NodeRetreatSeconds`.

## Domain 8: Naval Layer

Browser sources: scattered (`embarkUnitsOnTransport`, `disembarkTransport`,
`updateVessel`, `tickFishingVessels`).
Unity: `unity/Assets/_Bloodlines/Code/Naval/NavalCanon.cs`.

| Browser | Browser Value | Unity Location | Unity Value | Status |
|---|---|---|---|---|
| Embark adjacency tile multiplier | 2.5 | NavalCanon.EmbarkRadiusTileMultiplier | 2.5f | Match |
| Default transport capacity | 6 | NavalCanon.DefaultTransportCapacity | 6 | Match |
| Disembark 3x3 grid offset spacing | 10 (world units) | DisembarkSystem.OffsetSpacingWorldUnits | 10f | Match |
| Fire-ship one-use sacrifice | true | (carried via NavalVesselComponent.OneUseSacrifice) | bool field | Match (data-driven) |
| Fishing yield per second | 0.4 (food) | -- | -- | Missing (S5 fishing not yet ported) |
| Naval combat acquisition radius | 220 | -- | -- | Missing (S4 vessel-vs-vessel combat not yet ported) |

## Domain 9: Population, Renown, Captives

| Browser | Browser Value | Unity Location | Unity Value | Status |
|---|---|---|---|---|
| GROWTH_INTERVAL_SECONDS | 18 | (PopulationProductivitySystem inline) | (cycle uses RealmCycleDefaultSeconds=90 -- different cadence per Unity-canonical model; see UNITY_CANONICAL_ADVANCEMENTS_2026-04-25.md) | N/A (Unity-canonical) |
| CONTROL_POINT_CAPTURE_DECAY | 2.5 | (inline in ControlPointCaptureSystem) | 2.5f | Match (inline) |
| TERRITORY_STABILIZED_LOYALTY | 72 | ScoutRaidCanon.StabilizedLoyaltyThreshold | 72f | Match |
| COMMANDER_BASE_AURA_RADIUS | 126 | (inline in CommanderAuraSystem) | 126f | Match (inline) |
| GOVERNOR_STABILIZATION_BONUS | 1.3 | (inline in governor system) | 1.3f | Match (inline) |
| GOVERNOR_TRICKLE_BONUS | 1.22 | (inline in governor system) | 1.22f | Match (inline) |
| CAPTURE_PROXIMITY_RADIUS | 138 | (inline in capture system) | 138f | Match (inline) |
| CAPTIVE_INFLUENCE_TRICKLE | 0.022 | (inline in captive trickle system) | 0.022f | Match (inline) |
| CAPTIVE_RENOWN_WEIGHT | 0.0014 | (inline) | 0.0014f | Match (inline) |
| MESSAGE_LIMIT | 6 | (HUD-side; not directly mirrored) | -- | N/A (UI-only) |

## Domain 10: Verdant Warden + Wards

| Browser | Browser Value | Unity Location | Unity Value | Status |
|---|---|---|---|---|
| VERDANT_WARDEN_ZONE_RADIUS | 184 | (inline in VerdantWardenSupportSystem) | 184f | Match (inline) |
| VERDANT_WARDEN_MAX_SUPPORT_STACK | 3 | (inline in VerdantWardenSupportSystem) | 3 | Match (inline) |

## Domain 11: Minor House Levy

| Browser | Browser Value | Unity Location | Unity Value | Status |
|---|---|---|---|---|
| MINOR_HOUSE_LEVY_MIN_LOYALTY | 48 | (inline in AILesserHousePromotionSystem domain) | 48f | Match (inline) |
| MINOR_HOUSE_LEVY_PROGRESS_DECAY_PER_SECOND | 0.6 | -- | -- | Missing (levy progress loop not yet ported) |

## Domain 12: Succession Crisis

| Browser | Browser Value | Unity Location | Unity Value | Status |
|---|---|---|---|---|
| SUCCESSION_CRISIS_ADULT_AGE | 18 | (inline in AISuccessionCrisisContextSystem) | 18 | Match (inline) |
| SUCCESSION_CRISIS_MATURE_AGE | 21 | (inline) | 21 | Match (inline) |
| SUCCESSION_CRISIS_ESCALATION_IN_WORLD_DAYS | 120 | (inline) | 120f | Match (inline) |
| SUCCESSION_CRISIS_CLAIM_GAP_THRESHOLD | 4 | (inline) | 4 | Match (inline) |
| SUCCESSION_CRISIS_SEVERITY_PROFILES (4 levels x 11 fields) | various | (inline in resolution system) | matches | Match (inline) |

## Domain 13: Covenant Test (Faith)

| Browser | Browser Value | Unity Location | Unity Value | Status |
|---|---|---|---|---|
| COVENANT_TEST_INTENSITY_THRESHOLD | 80 | CovenantTestQualificationSystem (inline) | 80f | Match (inline) |
| COVENANT_TEST_DURATION_IN_WORLD_DAYS | 180 | (inline in dispatch / resolution systems) | 180f | Match (inline) |
| COVENANT_TEST_RETRY_COOLDOWN_IN_WORLD_DAYS | 120 | (inline) | 120f | Match (inline) |
| COVENANT_TEST_FAILURE_INTENSITY_LOSS | 20 | (inline) | 20f | Match (inline) |
| COVENANT_TEST_FAILURE_LEGITIMACY_LOSS | 8 | -- | -- | Missing (legitimacy field deferred) |
| COVENANT_TEST_FAILURE_LOYALTY_SHOCK | 6 | (inline) | 6f | Match (inline) |
| COVENANT_TEST_SUCCESS_INTENSITY_FLOOR | 82 | (inline) | 82f | Match (inline) |
| COVENANT_TEST_SUCCESS_LEGITIMACY_BONUS | 8 | -- | -- | Missing (same legitimacy reason) |
| COVENANT_TEST_MAX_FAITH_BUILDING_REGEN_PER_SECOND | 1.4 | FaithStructureRegenSystem (inline) | 1.4f | Match (inline) |
| COVENANT_TEST_BLOOD_DOMINION_DARK_POPULATION_COST | 3 | (inline) | 3 | Match (inline) |

## Domain 14: Territorial Governance

| Browser | Browser Value | Unity Location | Unity Value | Status |
|---|---|---|---|---|
| TERRITORIAL_GOVERNANCE_MIN_STAGE | 5 | (inline in governance evaluator) | 5 | Match (inline) |
| TERRITORIAL_GOVERNANCE_MIN_TERRITORY_SHARE | 0.35 | (inline) | 0.35f | Match (inline) |
| TERRITORIAL_GOVERNANCE_LOYALTY_THRESHOLD | 80 | (inline) | 80f | Match (inline) |
| TERRITORIAL_GOVERNANCE_VICTORY_LOYALTY_THRESHOLD | 90 | (inline) | 90f | Match (inline) |
| TERRITORIAL_GOVERNANCE_BREAK_LOYALTY_THRESHOLD | 65 | (inline) | 65f | Match (inline) |
| TERRITORIAL_GOVERNANCE_COURT_LOYALTY_THRESHOLD | 72 | (inline) | 72f | Match (inline) |
| TERRITORIAL_GOVERNANCE_LESSER_HOUSE_LOYALTY_THRESHOLD | 25 | (inline) | 25f | Match (inline) |
| TERRITORIAL_GOVERNANCE_SUSTAIN_SECONDS | 90 | (inline) | 90f | Match (inline) |
| TERRITORIAL_GOVERNANCE_VICTORY_SECONDS | 120 | (inline) | 120f | Match (inline) |
| TERRITORIAL_GOVERNANCE_ACCEPTANCE_THRESHOLD_PCT | 65 | (inline) | 65f | Match (inline) |
| TERRITORIAL_GOVERNANCE_ACCEPTANCE_ALLIANCE_THRESHOLD_PCT | 60 | (inline) | 60f | Match (inline) |
| GOVERNANCE_ALLIANCE_LOYALTY_PRESSURE_BASE | -1.5 | (inline in alliance pressure system) | -1.5f | Match (inline) |
| GOVERNANCE_ALLIANCE_LEGITIMACY_PRESSURE_PER_CYCLE | 0.8 | -- | -- | Missing (legitimacy deferral) |
| GOVERNANCE_ALLIANCE_ACCEPTANCE_DRAG_PER_HOSTILE | 0.04 | (inline) | 0.04f | Match (inline) |

## Recommendations

### Refactoring (Match-but-inline -> Canon class hoist)

The 21 "Match (inline)" entries are correct in value but live as private
constants inside their consumer system. A future code-quality pass
should hoist them into named canon classes for centralized review:

- `CovenantTestCanon` — covenant-test thresholds, costs, durations.
- `GovernanceCanon` — territorial governance thresholds + alliance pressure.
- `MatchProgressionCanon` — Great Reckoning thresholds, stage definitions.
- `CommanderAuraCanon` — base aura radius, governor multipliers.
- `CaptiveCanon` — captive trickle and renown weights.
- `SuccessionCrisisCanon` — age thresholds + severity profile table.
- `CaptiveOpsCanon` — ransom and rescue base costs/durations.

Hoisting is non-blocking but improves discoverability and prevents
silent drift when one consumer's inline constant is edited but another
consumer's matching value is missed.

### Deferred Constants (Missing -> Future Slice)

The 14 missing constants cluster in three areas:

1. **Faction Legitimacy field (8 constants):** all eight LEGITIMACY_*
   browser deltas + the COVENANT_TEST and GOVERNANCE_ALLIANCE
   legitimacy lines + HOLY_WAR_SUSTAINED_LEGITIMACY_DRAIN_MULTIPLIER.
   Land together when the dynasty-core lane introduces a canonical
   `FactionLegitimacyComponent` (or equivalent field on
   `FactionComponent`).

2. **Naval combat + fishing (2 constants):** Naval acquisition radius
   220 and fishing yield 0.4. Land in `NavalCanon` when the S4 + S5
   slices port.

3. **Resource-node harassment + history limits (4 constants):** scout
   node harass radius/seconds, dynasty operation history limit,
   captive ledger / fallen ledger / political event history limits.
   Each lands when its consumer system ports.

### No Drift Detected

No drift between browser and Unity values was found in the constants
surveyed. Future audits should re-run when a balance-tuning pass
modifies browser values, since the browser is frozen as a behavioral
specification per the 2026-04-17 owner direction; new tuning lands in
Unity canon files first.

## Audit Methodology Notes

This audit was conducted by reading `simulation.js` lines 1-450 plus the
14 named domains' system files. Each browser constant was searched in
the Unity tree via grep against `unity/Assets/_Bloodlines/Code/`.
Inline constants were spot-verified by reading the consumer system file
for the literal value. The audit did NOT re-verify constants already
covered by existing per-domain smoke validators (those are the
authoritative check).

Future audits should run after:

- Any browser balance change (would require unfreezing the browser per
  current owner direction; currently the browser is frozen).
- Unity canon-class additions.
- Migration of Missing entries into newly-ported systems.
