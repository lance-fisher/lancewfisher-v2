import { clamp, distance, moveToward, rectContainsPoint } from "./utils.js";

const MESSAGE_LIMIT = 6;
const GROWTH_INTERVAL_SECONDS = 18;
const CONTROL_POINT_CAPTURE_DECAY = 2.5;
const FAITH_EXPOSURE_THRESHOLD = 100;
const TERRITORY_STABILIZED_LOYALTY = 72;
const COMMANDER_BASE_AURA_RADIUS = 126;
const GOVERNOR_STABILIZATION_BONUS = 1.3;
const GOVERNOR_TRICKLE_BONUS = 1.22;
const CAPTURE_PROXIMITY_RADIUS = 138;
const CAPTIVE_INFLUENCE_TRICKLE = 0.022;
const CAPTIVE_RENOWN_WEIGHT = 0.0014;
const CAPTIVE_LEDGER_LIMIT = 16;
const FALLEN_LEDGER_LIMIT = 12;
const DYNASTY_OPERATION_HISTORY_LIMIT = 12;
const DYNASTY_OPERATION_ACTIVE_LIMIT = 6;
const LEGITIMACY_LOSS_COMMANDER_KILL = 9;
const LEGITIMACY_LOSS_COMMANDER_CAPTURE = 12;
const LEGITIMACY_LOSS_HEAD_FALL = 18;
const LEGITIMACY_LOSS_GOVERNOR_LOSS = 5;
const LEGITIMACY_LOSS_INTERREGNUM = 14;
const LEGITIMACY_RECOVERY_ON_SUCCESSION = 7;
const LEGITIMACY_RECOVERY_ON_RANSOM = 4;
const LEGITIMACY_RECOVERY_ON_RESCUE = 6;
const POLITICAL_EVENT_HISTORY_LIMIT = 8;
const RANSOM_BASE_DURATION_SECONDS = 16;
const RANSOM_DURATION_RENOWN_MULTIPLIER = 0.55;
const RESCUE_BASE_DURATION_SECONDS = 20;
const RESCUE_DURATION_RENOWN_MULTIPLIER = 0.7;
const RANSOM_BASE_GOLD_COST = 70;
const RANSOM_BASE_INFLUENCE_COST = 18;
const RESCUE_BASE_GOLD_COST = 42;
const RESCUE_BASE_INFLUENCE_COST = 26;
const SCOUT_RAID_TARGET_RANGE = 24;
const SCOUT_RAID_RETREAT_DISTANCE = 84;
const SCOUT_RAID_LOYALTY_RADIUS = 240;
const SCOUT_NODE_HARASS_RADIUS = 120;
const SCOUT_NODE_RETREAT_SECONDS = 10;
const SUCCESSION_ROLE_CHAIN = [
  "head_of_bloodline",
  "heir_designate",
  "commander",
  "governor",
  "diplomat",
  "ideological_leader",
  "merchant",
  "spymaster",
];
const SUCCESSION_CRISIS_ADULT_AGE = 18;
const SUCCESSION_CRISIS_MATURE_AGE = 21;
const SUCCESSION_CRISIS_ESCALATION_IN_WORLD_DAYS = 120;
const SUCCESSION_CRISIS_CLAIM_GAP_THRESHOLD = 4;
const SUCCESSION_CRISIS_SEVERITY_ORDER = ["minor", "moderate", "major", "catastrophic"];
const SUCCESSION_CRISIS_SEVERITY_PROFILES = {
  minor: {
    id: "minor",
    label: "Minor",
    loyaltyShock: 3,
    loyaltyDrainPerDay: 0.12,
    legitimacyDrainPerDay: 0.08,
    resourceTrickleMultiplier: 0.96,
    populationGrowthMultiplier: 0.94,
    stabilizationMultiplier: 0.94,
    attackMultiplier: 0.97,
    resolutionCost: { gold: 80, influence: 18 },
    legitimacyRecovery: 4,
    loyaltyRecovery: 3,
  },
  moderate: {
    id: "moderate",
    label: "Moderate",
    loyaltyShock: 5,
    loyaltyDrainPerDay: 0.2,
    legitimacyDrainPerDay: 0.14,
    resourceTrickleMultiplier: 0.9,
    populationGrowthMultiplier: 0.86,
    stabilizationMultiplier: 0.88,
    attackMultiplier: 0.94,
    resolutionCost: { gold: 110, influence: 24 },
    legitimacyRecovery: 6,
    loyaltyRecovery: 4,
  },
  major: {
    id: "major",
    label: "Major",
    loyaltyShock: 7,
    loyaltyDrainPerDay: 0.34,
    legitimacyDrainPerDay: 0.24,
    resourceTrickleMultiplier: 0.84,
    populationGrowthMultiplier: 0.78,
    stabilizationMultiplier: 0.8,
    attackMultiplier: 0.9,
    resolutionCost: { gold: 145, influence: 32 },
    legitimacyRecovery: 8,
    loyaltyRecovery: 5,
  },
  catastrophic: {
    id: "catastrophic",
    label: "Catastrophic",
    loyaltyShock: 10,
    loyaltyDrainPerDay: 0.5,
    legitimacyDrainPerDay: 0.34,
    resourceTrickleMultiplier: 0.74,
    populationGrowthMultiplier: 0.68,
    stabilizationMultiplier: 0.72,
    attackMultiplier: 0.84,
    resolutionCost: { gold: 190, influence: 42 },
    legitimacyRecovery: 10,
    loyaltyRecovery: 6,
  },
};
const DEFAULT_POLITICAL_EVENT_EFFECTS = {
  resourceTrickleMultiplier: 1,
  populationGrowthMultiplier: 1,
  stabilizationMultiplier: 1,
  attackMultiplier: 1,
};
const COVENANT_TEST_INTENSITY_THRESHOLD = 80;
const COVENANT_TEST_DURATION_IN_WORLD_DAYS = 180;
const COVENANT_TEST_RETRY_COOLDOWN_IN_WORLD_DAYS = 120;
const COVENANT_TEST_FAILURE_INTENSITY_LOSS = 20;
const COVENANT_TEST_FAILURE_LEGITIMACY_LOSS = 8;
const COVENANT_TEST_FAILURE_LOYALTY_SHOCK = 6;
const COVENANT_TEST_SUCCESS_INTENSITY_FLOOR = 82;
const COVENANT_TEST_SUCCESS_LEGITIMACY_BONUS = 8;
const COVENANT_TEST_MAX_FAITH_BUILDING_REGEN_PER_SECOND = 1.4;
const COVENANT_TEST_ACTIVE_EFFECTS = {
  resourceTrickleMultiplier: 0.94,
  populationGrowthMultiplier: 0.96,
  stabilizationMultiplier: 0.9,
  attackMultiplier: 0.95,
};
const COVENANT_TEST_BLOOD_DOMINION_LIGHT_RITE_COST = {
  food: 45,
  influence: 18,
};
const COVENANT_TEST_BLOOD_DOMINION_DARK_BINDING_COST = {
  influence: 20,
};
const COVENANT_TEST_BLOOD_DOMINION_DARK_POPULATION_COST = 3;
const COVENANT_TEST_BLOOD_DOMINION_DARK_LEGITIMACY_COST = 6;
const TERRITORIAL_GOVERNANCE_MIN_STAGE = 5;
const TERRITORIAL_GOVERNANCE_MIN_TERRITORY_SHARE = 0.35;
const TERRITORIAL_GOVERNANCE_LOYALTY_THRESHOLD = 80;
const TERRITORIAL_GOVERNANCE_VICTORY_LOYALTY_THRESHOLD = 90;
const TERRITORIAL_GOVERNANCE_BREAK_LOYALTY_THRESHOLD = 65;
const TERRITORIAL_GOVERNANCE_COURT_LOYALTY_THRESHOLD = 72;
const TERRITORIAL_GOVERNANCE_LESSER_HOUSE_LOYALTY_THRESHOLD = 25;
const TERRITORIAL_GOVERNANCE_SUSTAIN_SECONDS = 90;
const TERRITORIAL_GOVERNANCE_VICTORY_SECONDS = 120;
const TERRITORIAL_GOVERNANCE_WORLD_PRESSURE_SCORE = 3;
const TERRITORIAL_GOVERNANCE_RECOGNIZED_WORLD_PRESSURE_SCORE = 5;
const TERRITORIAL_GOVERNANCE_THRESHOLD_WORLD_PRESSURE_SCORE = 6;
const TERRITORIAL_GOVERNANCE_VICTORY_WORLD_PRESSURE_SCORE = 7;
const TERRITORIAL_GOVERNANCE_ACCEPTANCE_THRESHOLD_PCT = 65;
const TERRITORIAL_GOVERNANCE_ACCEPTANCE_ALLIANCE_THRESHOLD_PCT = 60;
// Session 89: coalition loyalty pressure on governed frontier marches.
// Once population acceptance crosses the alliance threshold (60%), every rival
// kingdom that is hostile to the governance leader applies coordinated frontier
// pressure: loyalty erosion on the weakest governed march each realm cycle,
// legitimacy strain, and acceptance-rise drag. The values are deliberately
// moderate: the counter-pressure makes the final 60->65% push feel contested,
// not impossible. Two hostile kingdoms produce -3 loyalty per cycle on the
// weakest march, which is comparable to a single water crisis but focused on
// one territory instead of all.
const GOVERNANCE_ALLIANCE_LOYALTY_PRESSURE_BASE = -1.5;
const GOVERNANCE_ALLIANCE_LEGITIMACY_PRESSURE_PER_CYCLE = 0.8;
const GOVERNANCE_ALLIANCE_ACCEPTANCE_DRAG_PER_HOSTILE = 0.04;
const DOCTRINE_DEFAULTS = {
  label: "Unsworn",
  auraAttackMultiplier: 1,
  auraRadiusBonus: 0,
  auraSightBonus: 0,
  stabilizationMultiplier: 1,
  captureMultiplier: 1,
  populationGrowthMultiplier: 1,
};
const FAITH_INTENSITY_TIERS = [
  { level: 5, min: 80, label: "Apex" },
  { level: 4, min: 60, label: "Fervent" },
  { level: 3, min: 40, label: "Devout" },
  { level: 2, min: 20, label: "Active" },
  { level: 1, min: 1, label: "Latent" },
  { level: 0, min: 0, label: "Unawakened" },
];

// Fortification / siege canon constants (2026-04-14 doctrine lane).
const FORTIFICATION_ECOSYSTEM_RADIUS_TILES = 9;
const ASSAULT_STRAIN_THRESHOLD = 6;
const ASSAULT_STRAIN_DECAY_PER_SECOND = 0.12;
const ASSAULT_COHESION_PENALTY_DURATION = 20;
const ASSAULT_COHESION_PENALTY_MULTIPLIER = 0.85;
const FORTIFICATION_AURA_RADIUS_TILES = 10;
const FORTIFICATION_THREAT_RADIUS_TILES = 8;
const FORTIFICATION_RESERVE_RADIUS_TILES = 12;
const FORTIFICATION_TRIAGE_RADIUS_TILES = 2.4;
const FORTIFICATION_KEEP_PRESENCE_RADIUS_TILES = 6;
const IMMINENT_ENGAGEMENT_WARNING_BUFFER_TILES = 4;
const IMMINENT_ENGAGEMENT_WATCHTOWER_RADIUS_TILES = 14;
const IMMINENT_ENGAGEMENT_MIN_SECONDS = 10;
const IMMINENT_ENGAGEMENT_MAX_SECONDS = 30;
const IMMINENT_ENGAGEMENT_KEEP_BASE_SECONDS = 14;
const IMMINENT_ENGAGEMENT_SETTLEMENT_BASE_SECONDS = 11;
const IMMINENT_ENGAGEMENT_REINFORCEMENT_SURGE_SECONDS = 18;
const RESERVE_RETREAT_HEALTH_RATIO = 0.42;
const RESERVE_RECOVERY_HEALTH_RATIO = 0.82;
const RESERVE_TRIAGE_HEAL_PER_SECOND = 5.5;
const RESERVE_MUSTER_INTERVAL_SECONDS = 3.5;
const BLOOD_ALTAR_SURGE_DURATION_SECONDS = 18;
const BLOOD_ALTAR_SURGE_COOLDOWN_SECONDS = 34;
const REALM_CYCLE_DEFAULT_SECONDS = 90;
const SIEGE_SUPPORT_REFRESH_SECONDS = 1.25;
const SIEGE_UNSUPPLIED_ATTACK_MULTIPLIER = 0.84;
const SIEGE_UNSUPPLIED_SPEED_MULTIPLIER = 0.88;
const SIEGE_UNSUPPLIED_MESSAGE_INTERVAL_SECONDS = 10;
const SIEGE_RESUPPLIED_MESSAGE_INTERVAL_SECONDS = 12;
const CONVOY_RECOVERY_DURATION_SECONDS = 12;
const CONVOY_ESCORT_SCREEN_RADIUS = 86;
const CONVOY_ESCORT_MIN_ESCORTS = 2;
const FIELD_WATER_LOCAL_SUPPORT_RADIUS = 132;
const FIELD_WATER_SETTLEMENT_SUPPORT_RADIUS = 156;
const FIELD_WATER_SUPPORT_DURATION_SECONDS = 14;
const FIELD_WATER_TRANSFER_INTERVAL_SECONDS = 4;
const FIELD_WATER_TRANSFER_COST = 0.2;
const FIELD_WATER_STRAIN_PER_SECOND = 0.85;
const FIELD_WATER_RECOVERY_PER_SECOND = 1.25;
const FIELD_WATER_STRAIN_THRESHOLD = 6;
const FIELD_WATER_CRITICAL_THRESHOLD = 12;
const FIELD_WATER_STRAIN_ATTACK_MULTIPLIER = 0.88;
const FIELD_WATER_STRAIN_SPEED_MULTIPLIER = 0.9;
const FIELD_WATER_CRITICAL_ATTACK_MULTIPLIER = 0.72;
const FIELD_WATER_CRITICAL_SPEED_MULTIPLIER = 0.78;
const FIELD_WATER_MESSAGE_INTERVAL_SECONDS = 18;
const FIELD_WATER_ATTRITION_THRESHOLD_SECONDS = 4;
const FIELD_WATER_DESERTION_THRESHOLD_SECONDS = 10;
const FIELD_WATER_ATTRITION_DAMAGE_PER_SECOND = 6;
const FIELD_WATER_CRITICAL_RECOVERY_PER_SECOND = 2.1;
const FIELD_WATER_COMMANDER_DISCIPLINE_BUFFER_SECONDS = 4;
const FIELD_WATER_COMMANDER_ATTRITION_MULTIPLIER = 0.6;
const FIELD_WATER_DESERTION_HEALTH_RATIO = 0.45;
const VERDANT_WARDEN_ZONE_RADIUS = 184;
const VERDANT_WARDEN_MAX_SUPPORT_STACK = 3;
const MINOR_HOUSE_LEVY_MIN_LOYALTY = 48;
const MINOR_HOUSE_LEVY_PROGRESS_DECAY_PER_SECOND = 0.6;

const MINOR_HOUSE_LEVY_PROFILES = {
  militia: {
    unitId: "militia",
    foodCost: 6,
    influenceCost: 5,
    loyaltyCost: 3,
    secondsRequired: 22,
  },
  swordsman: {
    unitId: "swordsman",
    foodCost: 8,
    influenceCost: 7,
    loyaltyCost: 4,
    secondsRequired: 28,
  },
  bowman: {
    unitId: "bowman",
    foodCost: 7,
    influenceCost: 8,
    loyaltyCost: 4,
    secondsRequired: 26,
  },
};

const IMMINENT_ENGAGEMENT_POSTURES = {
  brace: {
    id: "brace",
    label: "Brace Defenses",
    reserveHealMultiplier: 1.2,
    reserveMusterMultiplier: 1.16,
    desiredFrontlineBonus: 0,
    retreatThresholdBonus: 0.05,
    defenderAttackMultiplier: 0.98,
    defenderSightBonus: 8,
    enemyApproachMultiplier: 0.92,
    autoSortieOnExpiry: false,
  },
  steady: {
    id: "steady",
    label: "Steady Defense",
    reserveHealMultiplier: 1,
    reserveMusterMultiplier: 1,
    desiredFrontlineBonus: 0,
    retreatThresholdBonus: 0,
    defenderAttackMultiplier: 1,
    defenderSightBonus: 0,
    enemyApproachMultiplier: 1,
    autoSortieOnExpiry: false,
  },
  counterstroke: {
    id: "counterstroke",
    label: "Counterstroke",
    reserveHealMultiplier: 0.96,
    reserveMusterMultiplier: 1.1,
    desiredFrontlineBonus: 1,
    retreatThresholdBonus: -0.02,
    defenderAttackMultiplier: 1.1,
    defenderSightBonus: 12,
    enemyApproachMultiplier: 1,
    autoSortieOnExpiry: true,
  },
};

const DEFAULT_GOVERNOR_SPECIALIZATION = {
  id: "general",
  label: "General Stewardship",
  trickleMultiplier: 1,
  stabilizationMultiplier: 1,
  captureResistanceMultiplier: 1,
  loyaltyProtectionMultiplier: 1,
  reserveMusterMultiplier: 1,
  reserveHealMultiplier: 1,
};

const GOVERNOR_SPECIALIZATION_PROFILES = {
  border: {
    id: "border",
    label: "Border Marshal",
    trickleMultiplier: 1.08,
    stabilizationMultiplier: 1.2,
    captureResistanceMultiplier: 1.16,
    loyaltyProtectionMultiplier: 1.08,
    reserveMusterMultiplier: 1.06,
    reserveHealMultiplier: 1.03,
  },
  city: {
    id: "city",
    label: "Civic Steward",
    trickleMultiplier: 1.18,
    stabilizationMultiplier: 1.14,
    captureResistanceMultiplier: 1.08,
    loyaltyProtectionMultiplier: 1.18,
    reserveMusterMultiplier: 1,
    reserveHealMultiplier: 1,
  },
  keep: {
    id: "keep",
    label: "Keep Castellan",
    trickleMultiplier: 1.05,
    stabilizationMultiplier: 1.12,
    captureResistanceMultiplier: 1.2,
    loyaltyProtectionMultiplier: 1.24,
    reserveMusterMultiplier: 1.18,
    reserveHealMultiplier: 1.18,
  },
};

const GOVERNANCE_SEAT_MEMBER_PRIORITY = [
  "governor",
  "head_of_bloodline",
  "heir_designate",
  "diplomat",
  "merchant",
];

const GOVERNANCE_SEAT_ROLE_PROFILES = {
  governor: {
    keep: 6,
    city: 5,
    border: 4,
  },
  head_of_bloodline: {
    keep: 5,
    city: 4,
    border: 3,
  },
  heir_designate: {
    keep: 3,
    city: 2,
    border: 5,
  },
  diplomat: {
    keep: 3,
    city: 5,
    border: 2,
  },
  merchant: {
    keep: 2,
    city: 4,
    border: 3,
  },
};

const MATCH_STAGE_DEFINITIONS = [
  { number: 1, id: "founding", label: "Founding" },
  { number: 2, id: "expansion_identity", label: "Expansion and Identity" },
  { number: 3, id: "encounter_establishment", label: "Encounter and Establishment" },
  { number: 4, id: "war_turning_of_tides", label: "War and the Turning of Tides" },
  { number: 5, id: "final_convergence", label: "Final Convergence" },
];

const MATCH_PHASE_LABELS = {
  emergence: "Emergence",
  commitment: "Commitment",
  resolution: "Resolution",
};

const GREAT_RECKONING_TRIGGER_SHARE = 0.7;
const GREAT_RECKONING_RELEASE_SHARE = 0.66;
const GREAT_RECKONING_PRESSURE_SCORE = 4;

const DEFAULT_FORTIFICATION_WARD = {
  id: "unwarded",
  label: "Unwarded Keep",
  sightBonus: 0,
  defenderAttackMultiplier: 1,
  reserveHealMultiplier: 1,
  reserveMusterMultiplier: 1,
  loyaltyProtectionMultiplier: 1,
  enemySpeedMultiplier: 1,
  surgeActive: false,
};

const DEFAULT_VERDANT_WARDEN_SUPPORT = {
  count: 0,
  cappedCount: 0,
  defenderAttackMultiplier: 1,
  reserveHealMultiplier: 1,
  reserveMusterMultiplier: 1,
  loyaltyProtectionMultiplier: 1,
  loyaltyGainMultiplier: 1,
  stabilizationMultiplier: 1,
  desiredFrontlineBonus: 0,
};

const CAPTIVE_ROLE_OPERATION_MULTIPLIERS = {
  head_of_bloodline: 1.95,
  heir_designate: 1.72,
  commander: 1.42,
  governor: 1.22,
  diplomat: 1.16,
  ideological_leader: 1.18,
  merchant: 1.14,
  spymaster: 1.28,
};

const WARD_RESCUE_DIFFICULTY = {
  unwarded: 0,
  old_light: 2,
  blood_dominion: 3,
  the_order: 3,
  the_wild: 2.5,
};

function createEntityId(state, prefix) {
  state.counters[prefix] = (state.counters[prefix] ?? 0) + 1;
  return `${prefix}-${state.counters[prefix]}`;
}

function getBuildingDef(content, typeId) {
  return content.byId.buildings[typeId];
}

function getUnitDef(content, typeId) {
  return content.byId.units[typeId];
}

function getHouse(content, houseId) {
  return content.byId.houses[houseId];
}

function getFaction(state, factionId) {
  return state.factions[factionId];
}

function getMinorHouseClaim(state, factionId) {
  return state.world.controlPoints.find((controlPoint) => controlPoint.id === `${factionId}-claim`)
    ?? state.world.controlPoints.find((controlPoint) => controlPoint.ownerFactionId === factionId)
    ?? null;
}

function getMinorHouseCombatUnits(state, factionId) {
  return state.units.filter((unit) => {
    if (unit.factionId !== factionId || unit.health <= 0) {
      return false;
    }
    const unitDef = getUnitDef(state.content, unit.typeId);
    return unitDef && !["worker", "engineer-specialist", "support"].includes(unitDef.role);
  });
}

function ensureMinorHouseLevyState(faction) {
  faction.ai ??= {};
  faction.ai.levyProgress ??= 0;
  faction.ai.levyStatus ??= "forming";
  faction.ai.levySecondsRequired ??= MINOR_HOUSE_LEVY_PROFILES.militia.secondsRequired;
  faction.ai.levyUnitId ??= MINOR_HOUSE_LEVY_PROFILES.militia.unitId;
  faction.ai.retinueCap ??= 1;
  faction.ai.retinueCount ??= 0;
  faction.ai.lastLevyAt ??= null;
  faction.ai.lastLevyUnitId ??= null;
  faction.ai.parentPressureLevel ??= 0;
  faction.ai.parentPressureStatus ??= "quiet";
  faction.ai.parentPressureLevyTempo ??= 1;
  faction.ai.parentPressureRetakeTempo ??= 1;
  faction.ai.parentPressureRetinueBonus ??= 0;
  faction.ai.lastParentPressureStatus ??= "quiet";
  return faction.ai;
}

function getMinorHouseRetinueCap(claim, territoryCount = 1, pressureBonus = 0) {
  let cap = 1;
  if (claim?.ownerFactionId && claim.controlState === "stabilized" && (claim.loyalty ?? 0) >= 55) {
    cap += 1;
  }
  if (claim?.ownerFactionId && claim.controlState === "stabilized" && (claim.loyalty ?? 0) >= 72) {
    cap += 1;
  }
  cap += Math.max(0, territoryCount - 1);
  cap += Math.max(0, pressureBonus);
  return Math.max(1, Math.min(6, cap));
}

function pickMinorHouseLevyProfile(claim, currentRetinue) {
  const loyalty = claim?.loyalty ?? 0;
  if (loyalty >= 80 && currentRetinue >= 2) {
    return MINOR_HOUSE_LEVY_PROFILES.bowman;
  }
  if (loyalty >= 68) {
    return MINOR_HOUSE_LEVY_PROFILES.swordsman;
  }
  return MINOR_HOUSE_LEVY_PROFILES.militia;
}

function spawnMinorHouseRetinueUnit(state, factionId, claim, unitId) {
  const unitDef = getUnitDef(state.content, unitId);
  if (!unitDef || !claim) {
    return null;
  }
  const claimPosition = getControlPointPosition(state, claim);
  const offsetSeed = (state.counters.unit ?? 0) + 1;
  const angle = (offsetSeed % 8) * (Math.PI / 4);
  const offset = 30 + (offsetSeed % 3) * 12;
  const unitIdValue = createEntityId(state, "unit");
  state.units.push({
    id: unitIdValue,
    factionId,
    typeId: unitId,
    x: claimPosition.x + Math.cos(angle) * offset,
    y: claimPosition.y + Math.sin(angle) * offset,
    radius: unitDef.role === "worker" ? 10 : 12,
    health: unitDef.health,
    attackCooldownRemaining: 0,
    gatherProgress: 0,
    carrying: null,
    commanderMemberId: null,
    reserveDuty: null,
    reserveSettlementId: null,
    supportStatus: null,
    engineerSupportUntil: 0,
    siegeSuppliedUntil: 0,
    logisticsInterdictedUntil: 0,
    convoyRecoveryUntil: 0,
    interdictedByFactionId: null,
    escortAssignedWagonId: null,
    convoyReconsolidatedAt: 0,
    lastSupplyTransferAt: -999,
    command: null,
  });
  return unitIdValue;
}

function getFactionDisplayName(state, factionId) {
  return getFaction(state, factionId)?.displayName ?? factionId;
}

function getFactionPresentation(state, factionId) {
  return getFaction(state, factionId)?.presentation ?? { primaryColor: "#777777", accentColor: "#d0b188" };
}

function getFaith(content, faithId) {
  return content.byId.faiths[faithId];
}

function getFaithDoctrineEffects(state, factionId) {
  const faction = getFaction(state, factionId);
  if (!faction?.faith?.selectedFaithId || !faction.faith.doctrinePath) {
    return DOCTRINE_DEFAULTS;
  }

  const faith = getFaith(state.content, faction.faith.selectedFaithId);
  return faith?.prototypeEffects?.[faction.faith.doctrinePath] ?? DOCTRINE_DEFAULTS;
}

// Session 52: shared covenant-compatibility profile for cross-dynasty marriage.
// Canonical target: faith does not sit beside dynasty politics as isolated flavor.
// Marriage between houses must increasingly reckon with committed covenant identity
// and doctrine divergence. This helper is shared by AI and UI so proposal logic and
// player legibility read from the same live source of truth.
export function getMarriageFaithCompatibilityProfile(state, sourceFactionId, targetFactionId) {
  const sourceFaction = getFaction(state, sourceFactionId);
  const targetFaction = getFaction(state, targetFactionId);
  const sourceFaithId = sourceFaction?.faith?.selectedFaithId ?? null;
  const targetFaithId = targetFaction?.faith?.selectedFaithId ?? null;
  const sourceDoctrinePath = sourceFaction?.faith?.doctrinePath ?? null;
  const targetDoctrinePath = targetFaction?.faith?.doctrinePath ?? null;
  const sourceFaith = sourceFaithId ? getFaith(state.content, sourceFaithId) : null;
  const targetFaith = targetFaithId ? getFaith(state.content, targetFaithId) : null;

  const baseProfile = {
    sourceFactionId,
    targetFactionId,
    sourceFaithId,
    targetFaithId,
    sourceFaithName: sourceFaith?.name ?? null,
    targetFaithName: targetFaith?.name ?? null,
    sourceCovenantName: sourceFaith?.covenantName ?? null,
    targetCovenantName: targetFaith?.covenantName ?? null,
    sourceDoctrinePath,
    targetDoctrinePath,
    sameFaithId: false,
    sameCovenant: false,
    sameDoctrine: false,
    tier: "unbound",
    label: "One or both courts remain unbound by covenant.",
    score: 0,
    legitimacySignalAllowed: false,
    blocksWeakMatch: false,
  };

  if (!sourceFaith || !targetFaith || !sourceDoctrinePath || !targetDoctrinePath) {
    return baseProfile;
  }

  const sameFaithId = sourceFaithId === targetFaithId;
  const sameCovenant = Boolean(sourceFaith.covenantName) && sourceFaith.covenantName === targetFaith.covenantName;
  const sameDoctrine = sourceDoctrinePath === targetDoctrinePath;

  if (sameFaithId && sameDoctrine) {
    return {
      ...baseProfile,
      sameFaithId,
      sameCovenant,
      sameDoctrine,
      tier: "harmonious",
      label: `Shared ${sourceFaith.name} ${sourceDoctrinePath} doctrine.`,
      score: 1,
      legitimacySignalAllowed: true,
      blocksWeakMatch: false,
    };
  }

  if (sameCovenant) {
    return {
      ...baseProfile,
      sameFaithId,
      sameCovenant,
      sameDoctrine,
      tier: "sectarian",
      label: `Shared ${sourceFaith.covenantName}, but doctrine splits ${sourceDoctrinePath} and ${targetDoctrinePath}.`,
      score: 0.45,
      legitimacySignalAllowed: true,
      blocksWeakMatch: false,
    };
  }

  if (sameDoctrine) {
    return {
      ...baseProfile,
      sameFaithId,
      sameCovenant,
      sameDoctrine,
      tier: "strained",
      label: `Cross-covenant match, but both courts follow ${sourceDoctrinePath} doctrine.`,
      score: -0.35,
      legitimacySignalAllowed: false,
      blocksWeakMatch: false,
    };
  }

  return {
    ...baseProfile,
    sameFaithId,
    sameCovenant,
    sameDoctrine,
    tier: "fractured",
    label: `Covenant fracture, ${sourceFaith.name} ${sourceDoctrinePath} doctrine against ${targetFaith.name} ${targetDoctrinePath}.`,
    score: -1,
    legitimacySignalAllowed: false,
    blocksWeakMatch: true,
  };
}

function getFaithOperatorMember(faction) {
  return getDynastyMemberByRole(faction, ["ideological_leader", "sorcerer", "head_of_bloodline", "diplomat"]);
}

function getFaithHolyWarsState(faction) {
  if (!faction?.faith) {
    return [];
  }
  faction.faith.activeHolyWars = faction.faith.activeHolyWars ?? [];
  return faction.faith.activeHolyWars;
}

function getActiveHolyWar(faction, targetFactionId, elapsed) {
  return getFaithHolyWarsState(faction).find((entry) =>
    entry.targetFactionId === targetFactionId &&
    (entry.expiresAt ?? 0) > elapsed) ?? null;
}

function getIncomingHolyWars(state, targetFactionId, elapsed) {
  return Object.values(state.factions).flatMap((faction) =>
    getFaithHolyWarsState(faction)
      .filter((entry) => entry.targetFactionId === targetFactionId && (entry.expiresAt ?? 0) > elapsed)
      .map((entry) => ({
        ...entry,
        sourceFactionId: faction.id,
      })));
}

function getFaithDivineRightDeclarationState(faction) {
  if (!faction?.faith) {
    return null;
  }
  faction.faith.divineRightDeclaration = faction.faith.divineRightDeclaration ?? null;
  faction.faith.lastDivineRightOutcome = faction.faith.lastDivineRightOutcome ?? null;
  faction.faith.divineRightCooldownUntil = faction.faith.divineRightCooldownUntil ?? 0;
  return faction.faith.divineRightDeclaration;
}

function getActiveDivineRightDeclaration(faction, elapsed) {
  const declaration = getFaithDivineRightDeclarationState(faction);
  if (!declaration) {
    return null;
  }
  return (declaration.resolveAt ?? 0) > elapsed && !declaration.failedAt && !declaration.completedAt
    ? declaration
    : null;
}

function getIncomingDivineRightDeclarations(state, factionId, elapsed) {
  return Object.values(state.factions).flatMap((faction) => {
    if (!faction || faction.id === factionId) {
      return [];
    }
    const declaration = getActiveDivineRightDeclaration(faction, elapsed);
    return declaration
      ? [{
        ...declaration,
        sourceFactionId: faction.id,
      }]
      : [];
  });
}

function getActiveFaithApexStructure(state, factionId) {
  return state.buildings.find((building) => {
    if (building.factionId !== factionId || !building.completed || building.health <= 0) {
      return false;
    }
    const buildingDef = getBuildingDef(state.content, building.typeId);
    return buildingDef?.faithRole === "apex";
  }) ?? null;
}

function getFaithCovenantTestProgressState(faction) {
  if (!faction?.faith) {
    return null;
  }
  faction.faith.covenantTestPassed = faction.faith.covenantTestPassed ?? false;
  faction.faith.lastCovenantTestOutcome = faction.faith.lastCovenantTestOutcome ?? null;
  faction.faith.covenantTestCooldownUntilInWorldDays = faction.faith.covenantTestCooldownUntilInWorldDays ?? 0;
  return faction.faith;
}

function ensureFaithCovenantTestCompletionFromLegacyState(state, factionId) {
  const faction = getFaction(state, factionId);
  const faith = getFaithCovenantTestProgressState(faction);
  if (!faith || faith.covenantTestPassed) {
    return faith;
  }
  const hasApexStructure = Boolean(getActiveFaithApexStructure(state, factionId));
  const hasApexUnit = state.units.some((unit) =>
    unit.factionId === factionId &&
    unit.health > 0 &&
    getUnitDef(state.content, unit.typeId)?.role === "faith-apex");
  const hasDivineRightTrail = Boolean(
    getActiveDivineRightDeclaration(faction, state.meta.elapsed) ||
    faith.lastDivineRightOutcome,
  );
  if (hasApexStructure || hasApexUnit || hasDivineRightTrail) {
    faith.covenantTestPassed = true;
    faith.lastCovenantTestOutcome = faith.lastCovenantTestOutcome ?? {
      outcome: "completed",
      mandateId: "legacy_migration",
      mandateLabel: "Legacy Covenant Recognition",
      resolvedAt: state.meta.elapsed,
      resolvedAtInWorldDays: state.dualClock?.inWorldDays ?? 0,
      resolutionReason: "legacy apex progression restored",
    };
  }
  return faith;
}

function getCompletedFaithBuildings(state, factionId, allowedRoles = null) {
  return state.buildings.filter((building) => {
    if (building.factionId !== factionId || !building.completed || building.health <= 0) {
      return false;
    }
    const buildingDef = getBuildingDef(state.content, building.typeId);
    if (!buildingDef?.faithRole) {
      return false;
    }
    return !allowedRoles || allowedRoles.includes(buildingDef.faithRole);
  });
}

function hasCompletedFaithBuildingOfType(state, factionId, typeId) {
  return state.buildings.some((building) =>
    building.factionId === factionId &&
    building.typeId === typeId &&
    building.completed &&
    building.health > 0);
}

function getFaithStructureIntensityRegenRate(state, factionId) {
  const total = getCompletedFaithBuildings(state, factionId)
    .reduce((sum, building) =>
      sum + (getBuildingDef(state.content, building.typeId)?.faithIntensityRegenBonus ?? 0), 0);
  return Math.min(COVENANT_TEST_MAX_FAITH_BUILDING_REGEN_PER_SECOND, total);
}

function getFaithSacredSite(state, faithId) {
  return (state.world.sacredSites ?? []).find((sacredSite) => sacredSite.faithId === faithId) ?? null;
}

function getClosestOwnedControlPointToPosition(state, factionId, position) {
  if (!position) {
    return null;
  }
  return state.world.controlPoints
    .filter((controlPoint) => controlPoint.ownerFactionId === factionId)
    .map((controlPoint) => ({
      controlPoint,
      distance: distance(
        controlPoint.x * state.world.tileSize,
        controlPoint.y * state.world.tileSize,
        position.x,
        position.y,
      ),
    }))
    .sort((left, right) => left.distance - right.distance)[0]?.controlPoint ?? null;
}

function getClosestExternalControlPointToPosition(state, factionId, position) {
  if (!position) {
    return null;
  }
  return state.world.controlPoints
    .filter((controlPoint) => controlPoint.ownerFactionId !== factionId)
    .map((controlPoint) => ({
      controlPoint,
      distance: distance(
        controlPoint.x * state.world.tileSize,
        controlPoint.y * state.world.tileSize,
        position.x,
        position.y,
      ),
    }))
    .sort((left, right) => left.distance - right.distance)[0]?.controlPoint ?? null;
}

function getFactionFaithUnitRoster(state, factionId, options = {}) {
  const {
    faithId = null,
    doctrinePath = null,
    stages = null,
  } = options;
  return state.units.filter((unit) => {
    if (unit.factionId !== factionId || unit.health <= 0) {
      return false;
    }
    const unitDef = getUnitDef(state.content, unit.typeId);
    if (!unitDef?.faithId) {
      return false;
    }
    if (faithId && unitDef.faithId !== faithId) {
      return false;
    }
    if (doctrinePath && unitDef.doctrinePath !== doctrinePath) {
      return false;
    }
    if (stages && !stages.includes(unitDef.stage)) {
      return false;
    }
    return true;
  });
}

function getFaithBuildingCoverageAt(state, factionId, position, allowedRoles = null, radiusPadding = 0) {
  if (!position) {
    return [];
  }
  return getCompletedFaithBuildings(state, factionId, allowedRoles).filter((building) => {
    const buildingDef = getBuildingDef(state.content, building.typeId);
    const center = getBuildingCenter(state, building, buildingDef);
    const radius = (buildingDef.faithExposureRadius ?? 0) + radiusPadding;
    return distance(center.x, center.y, position.x, position.y) <= radius;
  });
}

function getFaithRecognitionShareProfile(state, faithId) {
  let recognizedPopulation = 0;
  let totalPopulation = 0;
  Object.values(state.factions).forEach((faction) => {
    const population = Math.max(0, faction?.population?.total ?? 0);
    if (population <= 0) {
      return;
    }
    totalPopulation += population;
    const committed = faction.faith?.selectedFaithId === faithId;
    const intensityRatio = clamp((faction.faith?.intensity ?? 0) / 100, 0, 1);
    const exposureRatio = clamp((faction.faith?.exposure?.[faithId] ?? 0) / 100, 0, 1);
    const contributionRatio = committed
      ? Math.min(1, 0.75 + intensityRatio * 0.25)
      : exposureRatio * 0.25;
    recognizedPopulation += population * contributionRatio;
  });
  const share = totalPopulation > 0 ? recognizedPopulation / totalPopulation : 0;
  return {
    faithId,
    recognizedPopulation,
    totalPopulation,
    share,
    sharePct: Math.round(share * 1000) / 10,
  };
}

function getDivineRightDeclarationProfile(state, factionId) {
  const faction = getFaction(state, factionId);
  const selectedFaithId = faction?.faith?.selectedFaithId ?? null;
  const selectedFaith = selectedFaithId ? getFaith(state.content, selectedFaithId) : null;
  const matchProgression = getMatchProgressionSnapshot(state);
  const stageReady = (matchProgression.stageNumber ?? 1) >= DIVINE_RIGHT_DECLARATION_MIN_STAGE;
  const activeApexStructure = selectedFaithId ? getActiveFaithApexStructure(state, factionId) : null;
  const recognition = selectedFaithId
    ? getFaithRecognitionShareProfile(state, selectedFaithId)
    : { recognizedPopulation: 0, totalPopulation: 0, share: 0, sharePct: 0 };
  const cooldownRemaining = Math.max(0, (faction?.faith?.divineRightCooldownUntil ?? 0) - state.meta.elapsed);
  return {
    factionId,
    selectedFaithId,
    selectedFaithName: selectedFaith?.name ?? null,
    doctrinePath: faction?.faith?.doctrinePath ?? null,
    stageNumber: matchProgression.stageNumber ?? 1,
    stageReady,
    intensity: faction?.faith?.intensity ?? 0,
    level: faction?.faith?.level ?? 0,
    intensityReady: (faction?.faith?.intensity ?? 0) >= DIVINE_RIGHT_INTENSITY_THRESHOLD && (faction?.faith?.level ?? 0) >= 5,
    activeApexStructureId: activeApexStructure?.id ?? null,
    activeApexStructureName: activeApexStructure ? getBuildingDef(state.content, activeApexStructure.typeId)?.name ?? activeApexStructure.typeId : null,
    activeApexStructureReady: Boolean(activeApexStructure),
    recognitionShare: recognition.share,
    recognitionSharePct: recognition.sharePct,
    recognizedPopulation: Math.round(recognition.recognizedPopulation * 10) / 10,
    totalPopulation: Math.round(recognition.totalPopulation * 10) / 10,
    requiredShare: DIVINE_RIGHT_GLOBAL_SHARE_THRESHOLD,
    requiredSharePct: Math.round(DIVINE_RIGHT_GLOBAL_SHARE_THRESHOLD * 1000) / 10,
    recognitionReady: recognition.share >= DIVINE_RIGHT_GLOBAL_SHARE_THRESHOLD,
    cooldownRemaining,
  };
}

function getDynastyTerritorialGovernanceState(faction) {
  if (!faction?.dynasty) {
    return null;
  }
  faction.dynasty.territorialGovernanceRecognition = faction.dynasty.territorialGovernanceRecognition ?? null;
  faction.dynasty.lastTerritorialGovernanceOutcome = faction.dynasty.lastTerritorialGovernanceOutcome ?? null;
  return faction.dynasty;
}

function getActiveTerritorialGovernanceRecognition(faction) {
  const dynasty = getDynastyTerritorialGovernanceState(faction);
  const recognition = dynasty?.territorialGovernanceRecognition ?? null;
  return recognition && !recognition.failedAt ? recognition : null;
}

function getTerritorialGovernanceActiveMarriageCount(faction) {
  return (faction?.dynasty?.marriages ?? []).filter((marriage) => !marriage.dissolvedAt).length;
}

function getTerritorialGovernanceAcceptanceTier(acceptancePct, thresholdPct = TERRITORIAL_GOVERNANCE_ACCEPTANCE_THRESHOLD_PCT) {
  const allianceThresholdPct = Math.min(thresholdPct, TERRITORIAL_GOVERNANCE_ACCEPTANCE_ALLIANCE_THRESHOLD_PCT);
  if ((acceptancePct ?? 0) >= thresholdPct) {
    return { id: "accepted", label: "Accepted" };
  }
  if ((acceptancePct ?? 0) >= allianceThresholdPct) {
    return { id: "threshold", label: "Threshold" };
  }
  if ((acceptancePct ?? 0) >= 50) {
    return { id: "forming", label: "Forming" };
  }
  return { id: "quiet", label: "Quiet" };
}

function getTerritorialGovernanceAcceptanceProfile(state, factionId, context = {}) {
  const faction = context.faction ?? getFaction(state, factionId);
  const recognition = context.recognition ?? getActiveTerritorialGovernanceRecognition(faction);
  const territories = context.territories ?? state.world.controlPoints.filter((controlPoint) => controlPoint.ownerFactionId === factionId);
  const loyalTerritories = context.loyalTerritories ?? [];
  const victoryLoyalTerritories = context.victoryLoyalTerritories ?? [];
  const contestedTerritories = context.contestedTerritories ?? [];
  const activeHolyWars = context.activeHolyWars ?? [];
  const incomingHolyWars = context.incomingHolyWars ?? [];
  const courtLoyalty = context.courtLoyalty ?? getDynastyCourtLoyaltyProfile(state, factionId);
  const activeLesserHouses = context.activeLesserHouses ?? [];
  const stressedLesserHouses = context.stressedLesserHouses ?? [];
  const defectedBranches = context.defectedBranches ?? [];
  const territoryShare = context.territoryShare ?? 0;
  const totalTerritoryCount = context.totalTerritoryCount ?? Math.max(1, state.world.controlPoints.length);

  const population = Math.max(1, faction?.population?.total ?? 1);
  const foodRatio = (faction?.resources?.food ?? 0) / population;
  const waterRatio = (faction?.resources?.water ?? 0) / population;
  const completedBuildingCount = getCompletedBuildingCount(state, factionId);
  const infrastructurePerTerritory = completedBuildingCount / Math.max(1, territories.length || totalTerritoryCount);
  const raidedInfrastructureCount = state.buildings.filter((building) =>
    building.factionId === factionId &&
    building.completed &&
    building.health > 0 &&
    isBuildingUnderScoutRaid(state, building)).length;
  const harassedWorkerCount = state.units.filter((unit) =>
    unit.factionId === factionId &&
    unit.health > 0 &&
    getUnitDef(state.content, unit.typeId)?.role === "worker" &&
    unit.command?.type === "harass_retreat").length;
  const fieldWaterState = getFactionFieldWaterState(state, factionId);
  const siegeState = getFactionSiegeState(state, factionId);
  const activeMarriageCount = getTerritorialGovernanceActiveMarriageCount(faction);
  const marriageEnvoyAvailable = getMarriageEnvoyProfile(faction).available;
  const marriageAuthorityAvailable = getMarriageAuthorityProfile(faction).available;
  const legitimacy = clamp(faction?.dynasty?.legitimacy ?? 0, 0, 100);
  const faithIntensity = clamp(faction?.faith?.intensity ?? 0, 0, 100);
  const recentFallenCount = Math.min(5, faction?.dynasty?.attachments?.fallenMembers?.length ?? 0);
  const hostileKingdomCount = Object.values(state.factions).filter((candidate) =>
    candidate?.kind === "kingdom" &&
    candidate.id !== factionId &&
    areHostile(state, factionId, candidate.id)).length;
  const worldPressureLevel = faction?.worldPressureLevel ?? 0;
  const worldPressureScore = faction?.worldPressureScore ?? 0;

  const loyalShare = territories.length > 0 ? loyalTerritories.length / territories.length : 0;
  const integratedShare = territories.length > 0 ? victoryLoyalTerritories.length / territories.length : 0;
  const territoryShareNormalized = clamp(territoryShare / Math.max(0.0001, TERRITORIAL_GOVERNANCE_MIN_TERRITORY_SHARE), 0, 1);
  const foodSupport = clamp((foodRatio - 0.85) / 0.55, 0, 1) * 6;
  const waterSupport = clamp((waterRatio - 0.85) / 0.55, 0, 1) * 6;
  const infrastructureSupport = clamp(infrastructurePerTerritory / 3, 0, 1) * 6;
  const legitimacySupport = clamp(legitimacy / 100, 0, 1) * 6;
  const courtSupport = clamp((courtLoyalty.averageLoyalty ?? 0) / 100, 0, 1) * 6;
  const cadetSupport = Math.max(0, 6 - (stressedLesserHouses.length * 2) - (defectedBranches.length * 3));
  const faithSupport = faction?.faith?.selectedFaithId
    ? faithIntensity >= 25 && faithIntensity <= 80
      ? 2
      : 1
    : 0;
  const accordSupport = Math.min(4, activeMarriageCount * 2) +
    (marriageEnvoyAvailable ? 2 : 0) +
    (marriageAuthorityAvailable ? 1 : 0) +
    faithSupport;
  // Session 89: conviction-based acceptance modifier. Canon: Apex Moral dynasties
  // are recognized as protectors and neutral communities offer tribute; cruel
  // dynasties face rejection and resistance from the broader world.
  const convictionBandId = faction?.conviction?.bandId ?? "neutral";
  const convictionAcceptanceModifier =
    convictionBandId === "apex_moral" ? 4 :
    convictionBandId === "moral" ? 2 :
    convictionBandId === "cruel" ? -2 :
    convictionBandId === "apex_cruel" ? -4 :
    0;
  // Session 89: covenant commitment acceptance support. A passed Covenant Test
  // and a living Grand Sanctuary both represent deep faith endorsement of the
  // dynasty's right to govern, strengthening population acceptance.
  const covenantTestPassed = faction?.faith?.covenantTestPassed ?? false;
  const hasGrandSanctuary = hasCompletedFaithBuildingOfType(state, factionId, "grand_sanctuary");
  const covenantEndorsement = (covenantTestPassed ? 3 : 0) + (hasGrandSanctuary ? 2 : 0);
  // Session 89: tribal-backed acceptance friction. Active tribal raiders near
  // governed territory represent the world's neutral resistance to any single
  // dynasty claiming sovereignty. Canon: the Trueborn city and broader world
  // stabilizing forces should resist unchecked expansion.
  const activeTribeUnits = state.units.filter(
    (u) => u.factionId === "tribes" && u.health > 0).length;
  const tribalFriction = Math.min(4, activeTribeUnits * 0.8);
  // Session 93: Trueborn City endorsement. Positive standing with the neutral
  // city boosts acceptance; negative standing drags it. This is the canonical
  // neutral-power arbitration layer: the world's oldest authority either endorses
  // or resists a dynasty's claim to sovereignty.
  const truebornCity = state.factions.trueborn_city ?? null;
  const truebornStanding = truebornCity?.tradeRelationships?.[factionId] ?? 0;
  const truebornEndorsement = clamp(truebornStanding * 0.25, -3, 5);
  const territorySupport = (loyalShare * 8) + (integratedShare * 10) + (territoryShareNormalized * 5);
  const prosperitySupport = foodSupport + waterSupport + infrastructureSupport;
  const dynastySupport = legitimacySupport + courtSupport + cadetSupport;
  const integrationMomentum = territories.length > 0 && victoryLoyalTerritories.length === territories.length ? 6 : 0;
  const logisticsPenalty = Math.min(
    8,
    (raidedInfrastructureCount * 2) +
      (Math.min(3, harassedWorkerCount) * 0.75) +
      ((fieldWaterState.criticalUnits ?? 0) * 1.5) +
      ((fieldWaterState.attritionUnits ?? 0) * 1.25) +
      ((siegeState.unsuppliedEngines ?? 0) * 1.5),
  );
  const conflictPenalty = Math.min(
    14,
    (activeHolyWars.length * 4) +
      (incomingHolyWars.length * 4) +
      (contestedTerritories.length * 4) +
      (hostileKingdomCount * 1.5) +
      (worldPressureLevel * 1.5),
  );
  const dynasticPenalty = (recentFallenCount * 1.5) +
    (getActiveSuccessionCrisis(faction) ? 6 : 0) +
    (faction?.dynasty?.interregnum ? 8 : 0);
  const targetAcceptancePct = clamp(
    14 + territorySupport + prosperitySupport + dynastySupport + accordSupport + integrationMomentum + convictionAcceptanceModifier + covenantEndorsement + truebornEndorsement - logisticsPenalty - conflictPenalty - dynasticPenalty - tribalFriction,
    0,
    100,
  );
  const acceptanceThresholdPct = recognition?.populationAcceptanceThresholdPct ?? TERRITORIAL_GOVERNANCE_ACCEPTANCE_THRESHOLD_PCT;
  const allianceThresholdPct = recognition?.populationAcceptanceAllianceThresholdPct
    ?? Math.min(acceptanceThresholdPct, TERRITORIAL_GOVERNANCE_ACCEPTANCE_ALLIANCE_THRESHOLD_PCT);
  const seededAcceptancePct = clamp(targetAcceptancePct - 10, 40, Math.min(62, targetAcceptancePct));
  const currentAcceptancePct = clamp(recognition?.populationAcceptancePct ?? seededAcceptancePct, 0, 100);
  const acceptanceTier = getTerritorialGovernanceAcceptanceTier(currentAcceptancePct, acceptanceThresholdPct);
  const growthMomentum =
    (prosperitySupport >= 10 ? 0.15 : 0) +
    (dynastySupport >= 12 ? 0.1 : 0) +
    (activeMarriageCount > 0 ? 0.1 : 0) +
    (marriageEnvoyAvailable ? 0.05 : 0);
  // Session 89: coalition acceptance-drag. When the governance leader's
  // acceptance is at or above the alliance threshold (60%), every hostile
  // kingdom slows the acceptance rise rate. This makes the 60->65% push
  // materially harder when the governance leader has more hostile neighbors,
  // encouraging diplomacy and marriage to reduce hostility count.
  const atOrAboveAllianceThreshold = currentAcceptancePct >= allianceThresholdPct;
  const allianceDrag = atOrAboveAllianceThreshold
    ? hostileKingdomCount * GOVERNANCE_ALLIANCE_ACCEPTANCE_DRAG_PER_HOSTILE
    : 0;
  const decayPressure =
    (worldPressureLevel >= 2 ? 0.1 : 0) +
    (raidedInfrastructureCount > 0 ? 0.1 : 0) +
    (contestedTerritories.length > 0 ? 0.08 : 0) +
    allianceDrag;
  const riseRate = clamp(0.35 + growthMomentum - decayPressure, 0.2, 0.95);
  const fallRate = clamp(0.55 + (conflictPenalty * 0.03) + (logisticsPenalty * 0.02) + (dynasticPenalty * 0.04), 0.45, 1.25);
  const trendPerSecond = targetAcceptancePct > currentAcceptancePct + 0.05
    ? riseRate
    : targetAcceptancePct < currentAcceptancePct - 0.05
      ? -fallRate
      : 0;

  return {
    populationAcceptancePct: Math.round(currentAcceptancePct * 10) / 10,
    populationAcceptanceSeedPct: Math.round(seededAcceptancePct * 10) / 10,
    populationAcceptanceTargetPct: Math.round(targetAcceptancePct * 10) / 10,
    populationAcceptanceThresholdPct: acceptanceThresholdPct,
    populationAcceptanceAllianceThresholdPct: allianceThresholdPct,
    populationAcceptanceGapPct: Math.round(Math.max(0, acceptanceThresholdPct - currentAcceptancePct) * 10) / 10,
    populationAcceptanceReady: currentAcceptancePct >= acceptanceThresholdPct,
    allianceThresholdReady: currentAcceptancePct >= allianceThresholdPct,
    alliancePressureActive: atOrAboveAllianceThreshold && hostileKingdomCount > 0,
    alliancePressureHostileCount: atOrAboveAllianceThreshold ? hostileKingdomCount : 0,
    alliancePressureDrag: Math.round(allianceDrag * 1000) / 1000,
    populationAcceptanceTierId: acceptanceTier.id,
    populationAcceptanceTierLabel: acceptanceTier.label,
    populationAcceptanceTrendPerSecond: Math.round(trendPerSecond * 10) / 10,
    populationAcceptanceRiseRate: Math.round(riseRate * 10) / 10,
    populationAcceptanceFallRate: Math.round(fallRate * 10) / 10,
    foodRatio: Math.round(foodRatio * 10) / 10,
    waterRatio: Math.round(waterRatio * 10) / 10,
    completedBuildingCount,
    infrastructurePerTerritory: Math.round(infrastructurePerTerritory * 10) / 10,
    raidedInfrastructureCount,
    harassedWorkerCount,
    fieldWaterStrainedUnits: fieldWaterState.strainedUnits ?? 0,
    fieldWaterCriticalUnits: fieldWaterState.criticalUnits ?? 0,
    unsuppliedEngineCount: siegeState.unsuppliedEngines ?? 0,
    activeMarriageCount,
    marriageEnvoyAvailable,
    marriageAuthorityAvailable,
    activeHolyWarCount: activeHolyWars.length,
    hostileKingdomCount,
    worldPressureLevel,
    worldPressureScore,
    legitimacy: Math.round(legitimacy * 10) / 10,
    // Session 89: conviction, covenant, and tribal acceptance factors.
    convictionBandId,
    convictionAcceptanceModifier,
    covenantTestPassed,
    hasGrandSanctuary,
    covenantEndorsement,
    activeTribeUnits,
    tribalFriction: Math.round(tribalFriction * 10) / 10,
    // Session 93: Trueborn City endorsement.
    truebornStanding: Math.round(truebornStanding * 10) / 10,
    truebornEndorsement: Math.round(truebornEndorsement * 10) / 10,
  };
}

function getTerritorialGovernanceProfile(state, factionId) {
  const faction = getFaction(state, factionId);
  const recognition = getActiveTerritorialGovernanceRecognition(faction);
  const territories = state.world.controlPoints.filter((controlPoint) => controlPoint.ownerFactionId === factionId);
  const primarySeat = getPrimaryKeepSeat(state, factionId);
  const totalTerritoryCount = Math.max(1, state.world.controlPoints.length);
  const loyalTerritories = territories.filter((controlPoint) =>
    (controlPoint.loyalty ?? 0) >= TERRITORIAL_GOVERNANCE_LOYALTY_THRESHOLD &&
    controlPoint.controlState === "stabilized" &&
    !controlPoint.contested &&
    (controlPoint.captureProgress ?? 0) <= 0);
  const victoryLoyalTerritories = territories.filter((controlPoint) =>
    (controlPoint.loyalty ?? 0) >= TERRITORIAL_GOVERNANCE_VICTORY_LOYALTY_THRESHOLD &&
    controlPoint.controlState === "stabilized" &&
    !controlPoint.contested &&
    (controlPoint.captureProgress ?? 0) <= 0 &&
    Boolean(controlPoint.governorMemberId));
  const contestedTerritories = territories.filter((controlPoint) =>
    controlPoint.contested || (controlPoint.captureProgress ?? 0) > 0);
  const unstableTerritories = territories.filter((controlPoint) => controlPoint.controlState !== "stabilized");
  const unseatedTerritories = territories.filter((controlPoint) => !controlPoint.governorMemberId);
  const weakestTerritory = territories
    .slice()
    .sort((left, right) =>
      (left.loyalty ?? 0) - (right.loyalty ?? 0) ||
      Number(left.controlState !== "stabilized") - Number(right.controlState !== "stabilized") ||
      Number(Boolean(left.contested)) - Number(Boolean(right.contested)))[0] ?? null;
  const territoryShare = territories.length / totalTerritoryCount;
  const matchProgression = getMatchProgressionSnapshot(state);
  const activeHolyWars = (faction?.faith?.activeHolyWars ?? [])
    .filter((entry) => (entry.expiresAt ?? 0) > state.meta.elapsed);
  const incomingHolyWars = getIncomingHolyWars(state, factionId, state.meta.elapsed);
  const courtLoyalty = getDynastyCourtLoyaltyProfile(state, factionId);
  const activeLesserHouses = (faction?.dynasty?.lesserHouses ?? []).filter((lesserHouse) => lesserHouse.status === "active");
  const stressedLesserHouses = activeLesserHouses.filter((lesserHouse) =>
    (lesserHouse.loyalty ?? 100) < TERRITORIAL_GOVERNANCE_LESSER_HOUSE_LOYALTY_THRESHOLD);
  const defectedBranches = Object.values(state.factions).filter((candidate) =>
    candidate?.kind === "minor_house" &&
    candidate.originFactionId === factionId);
  const stageReady = (matchProgression.stageNumber ?? 1) >= TERRITORIAL_GOVERNANCE_MIN_STAGE;
  const shareReady = territoryShare >= TERRITORIAL_GOVERNANCE_MIN_TERRITORY_SHARE;
  const allHeldTerritoriesLoyal = territories.length > 0 && loyalTerritories.length === territories.length;
  const allHeldTerritoriesIntegrated = territories.length > 0 && victoryLoyalTerritories.length === territories.length;
  const noHeldTerritoryBelowBreakFloor = territories.every((controlPoint) =>
    (controlPoint.loyalty ?? 0) >= TERRITORIAL_GOVERNANCE_BREAK_LOYALTY_THRESHOLD);
  const territorySeatCoverageCount = territories.filter((controlPoint) => Boolean(controlPoint.governorMemberId)).length;
  const primarySeatGoverned = Boolean(primarySeat?.governorMemberId);
  const requiredSeatCoverageCount = territories.length + Number(Boolean(primarySeat));
  const seatCoverageCount = territorySeatCoverageCount + Number(primarySeatGoverned);
  const seatCoverageReady = territories.length > 0 &&
    territorySeatCoverageCount === territories.length &&
    (!primarySeat || primarySeatGoverned);
  const warStateClear = activeHolyWars.length === 0 && incomingHolyWars.length === 0;
  const antiRevoltReady = !faction?.dynasty?.interregnum &&
    !getActiveSuccessionCrisis(faction) &&
    stressedLesserHouses.length === 0 &&
    defectedBranches.length === 0 &&
    (courtLoyalty.averageLoyalty ?? 0) >= TERRITORIAL_GOVERNANCE_COURT_LOYALTY_THRESHOLD;
  const acceptanceProfile = getTerritorialGovernanceAcceptanceProfile(state, factionId, {
    faction,
    recognition,
    territories,
    loyalTerritories,
    victoryLoyalTerritories,
    contestedTerritories,
    activeHolyWars,
    incomingHolyWars,
    courtLoyalty,
    activeLesserHouses,
    stressedLesserHouses,
    defectedBranches,
    territoryShare,
    totalTerritoryCount,
  });
  const integrationReady = stageReady &&
    shareReady &&
    allHeldTerritoriesIntegrated &&
    noHeldTerritoryBelowBreakFloor &&
    contestedTerritories.length === 0 &&
    warStateClear &&
    seatCoverageReady &&
    antiRevoltReady;
  return {
    stageNumber: matchProgression.stageNumber ?? 1,
    stageReady,
    territoryCount: territories.length,
    totalTerritoryCount,
    territoryShare,
    territorySharePct: Math.round(territoryShare * 1000) / 10,
    shareReady,
    requiredShare: TERRITORIAL_GOVERNANCE_MIN_TERRITORY_SHARE,
    requiredSharePct: Math.round(TERRITORIAL_GOVERNANCE_MIN_TERRITORY_SHARE * 1000) / 10,
    loyalTerritoryCount: loyalTerritories.length,
    victoryLoyalTerritoryCount: victoryLoyalTerritories.length,
    loyalThreshold: TERRITORIAL_GOVERNANCE_LOYALTY_THRESHOLD,
    victoryLoyalThreshold: TERRITORIAL_GOVERNANCE_VICTORY_LOYALTY_THRESHOLD,
    breakThreshold: TERRITORIAL_GOVERNANCE_BREAK_LOYALTY_THRESHOLD,
    contestedTerritoryCount: contestedTerritories.length,
    unstableTerritoryCount: unstableTerritories.length,
    holyWarCount: activeHolyWars.length,
    incomingHolyWarCount: incomingHolyWars.length,
    warStateClear,
    allHeldTerritoriesLoyal,
    allHeldTerritoriesIntegrated,
    noHeldTerritoryBelowBreakFloor,
    courtAverageLoyalty: Math.round((courtLoyalty.averageLoyalty ?? 0) * 10) / 10,
    courtWeakestLoyalty: Math.round((courtLoyalty.weakestLoyalty ?? 0) * 10) / 10,
    courtLoyaltyThreshold: TERRITORIAL_GOVERNANCE_COURT_LOYALTY_THRESHOLD,
    activeLesserHouseCount: activeLesserHouses.length,
    stressedLesserHouseCount: stressedLesserHouses.length,
    stressedLesserHouseThreshold: TERRITORIAL_GOVERNANCE_LESSER_HOUSE_LOYALTY_THRESHOLD,
    stressedLesserHouseNames: stressedLesserHouses.map((lesserHouse) => lesserHouse.name),
    defectedBranchCount: defectedBranches.length,
    defectedBranchNames: defectedBranches.map((branch) => branch.displayName ?? branch.id),
    successionCrisisActive: Boolean(getActiveSuccessionCrisis(faction)),
    interregnum: Boolean(faction?.dynasty?.interregnum),
    primarySeatId: primarySeat?.id ?? null,
    primarySeatName: primarySeat?.name ?? null,
    primarySeatGoverned,
    seatCoverageCount,
    requiredSeatCoverageCount,
    territorySeatCoverageCount,
    unseatedTerritoryCount: unseatedTerritories.length,
    unseatedTerritoryName: unseatedTerritories[0]?.name ?? null,
    seatCoverageReady,
    antiRevoltReady,
    triggerReady: stageReady && shareReady && allHeldTerritoriesLoyal,
    holdReady: stageReady &&
      shareReady &&
      allHeldTerritoriesLoyal &&
      noHeldTerritoryBelowBreakFloor &&
      contestedTerritories.length === 0 &&
      warStateClear &&
      seatCoverageReady &&
      antiRevoltReady,
    integrationReady,
    victoryReady: integrationReady && acceptanceProfile.populationAcceptanceReady,
    weakestTerritoryId: weakestTerritory?.id ?? null,
    weakestTerritoryName: weakestTerritory?.name ?? null,
    weakestTerritoryLoyalty: Math.round(weakestTerritory?.loyalty ?? 0),
    weakestTerritoryControlState: weakestTerritory?.controlState ?? null,
    weakestTerritoryContested: Boolean(weakestTerritory?.contested),
    ...acceptanceProfile,
  };
}

function buildTerritorialGovernanceShortfalls(profile) {
  const shortfalls = [];
  if (!profile.stageReady) shortfalls.push("Final Convergence must be active.");
  if (!profile.shareReady) shortfalls.push(`Hold at least ${profile.requiredSharePct}% of total territory.`);
  if (!profile.allHeldTerritoriesLoyal) shortfalls.push(`Raise every held territory to ${profile.loyalThreshold}+ loyalty under stabilized rule.`);
  if (!profile.seatCoverageReady) shortfalls.push(`Seat governance authorities across every held march${profile.primarySeatName ? ` and ${profile.primarySeatName}` : ""}.`);
  if (profile.contestedTerritoryCount > 0) shortfalls.push("Remove active contest from every held territory.");
  if (!profile.noHeldTerritoryBelowBreakFloor) shortfalls.push(`No held territory may fall below ${profile.breakThreshold} loyalty during the sustained window.`);
  if (!profile.warStateClear) shortfalls.push("End all incoming and outgoing holy-war state during the sustained window.");
  if (!profile.antiRevoltReady) {
    if (profile.successionCrisisActive) shortfalls.push("Resolve the active succession crisis before governance can be recognized.");
    else if (profile.interregnum) shortfalls.push("End the interregnum before governance can be recognized.");
    else if (profile.stressedLesserHouseCount > 0) shortfalls.push(`Recover cadet loyalty above ${profile.stressedLesserHouseThreshold} before governance can be recognized.`);
    else if (profile.defectedBranchCount > 0) shortfalls.push("Suppress or reconcile active defected branches before governance can be recognized.");
    else shortfalls.push(`Raise court loyalty to ${profile.courtLoyaltyThreshold}+ before governance can be recognized.`);
  }
  return shortfalls;
}

function buildTerritorialGovernanceAcceptanceShortfalls(profile) {
  const shortfalls = [];
  if (profile.populationAcceptanceReady) {
    return shortfalls;
  }
  if ((profile.foodRatio ?? 0) < 1.05 || (profile.waterRatio ?? 0) < 1.05) {
    shortfalls.push("Restore food and water surplus so governed populations feel protected.");
  }
  if ((profile.raidedInfrastructureCount ?? 0) > 0 ||
      (profile.harassedWorkerCount ?? 0) > 0 ||
      (profile.fieldWaterCriticalUnits ?? 0) > 0 ||
      (profile.unsuppliedEngineCount ?? 0) > 0) {
    shortfalls.push("Recover raided infrastructure and field logistics so population acceptance can keep rising.");
  }
  if ((profile.legitimacy ?? 0) < 75 || (profile.courtAverageLoyalty ?? 0) < (profile.courtLoyaltyThreshold ?? TERRITORIAL_GOVERNANCE_COURT_LOYALTY_THRESHOLD)) {
    shortfalls.push("Raise court loyalty and bloodline legitimacy so governed populations trust the ruling line.");
  }
  if (profile.successionCrisisActive || profile.interregnum || (profile.stressedLesserHouseCount ?? 0) > 0 || (profile.defectedBranchCount ?? 0) > 0) {
    shortfalls.push("Stabilize cadet and succession politics so recognition looks durable beyond the court.");
  }
  if ((profile.activeMarriageCount ?? 0) === 0 && !profile.marriageEnvoyAvailable) {
    shortfalls.push("Open dynastic diplomatic channels so acceptance spreads beyond direct rule.");
  }
  if ((profile.activeHolyWarCount ?? 0) > 0 || (profile.incomingHolyWarCount ?? 0) > 0) {
    shortfalls.push("End holy-war pressure before population acceptance can harden.");
  }
  if ((profile.contestedTerritoryCount ?? 0) > 0 || (profile.worldPressureLevel ?? 0) >= 2 || (profile.hostileKingdomCount ?? 0) > 0) {
    shortfalls.push("Reduce frontier and coalition pressure so governed populations keep accepting the realm.");
  }
  if (shortfalls.length === 0) {
    shortfalls.push(`Raise population acceptance to ${profile.populationAcceptanceThresholdPct}% through prosperity, legitimacy, and stable governed marches.`);
  }
  return [...new Set(shortfalls)];
}

function buildTerritorialGovernanceVictoryShortfalls(profile) {
  const shortfalls = [];
  if (!profile.holdReady) {
    shortfalls.push(...buildTerritorialGovernanceShortfalls(profile));
  }
  if (!profile.integrationReady) shortfalls.push(`Raise every held territory to ${profile.victoryLoyalThreshold}+ loyalty with a seated governor.`);
  if (!profile.populationAcceptanceReady) shortfalls.push(...buildTerritorialGovernanceAcceptanceShortfalls(profile));
  return [...new Set(shortfalls)];
}

function getTerritorialGovernanceWorldPressureContribution(faction) {
  const recognition = getActiveTerritorialGovernanceRecognition(faction);
  if (!recognition) {
    return 0;
  }
  if (recognition.victoryReady || recognition.completedAt) {
    return TERRITORIAL_GOVERNANCE_VICTORY_WORLD_PRESSURE_SCORE;
  }
  if (recognition.integrationReady || (recognition.populationAcceptancePct ?? 0) >= (recognition.populationAcceptanceAllianceThresholdPct ?? TERRITORIAL_GOVERNANCE_ACCEPTANCE_ALLIANCE_THRESHOLD_PCT)) {
    return TERRITORIAL_GOVERNANCE_THRESHOLD_WORLD_PRESSURE_SCORE;
  }
  if (recognition.recognizedAt) {
    return TERRITORIAL_GOVERNANCE_RECOGNIZED_WORLD_PRESSURE_SCORE;
  }
  return TERRITORIAL_GOVERNANCE_WORLD_PRESSURE_SCORE;
}

function serializeTerritorialGovernanceRecognition(state, factionId, recognition) {
  if (!recognition) {
    return null;
  }
  const profile = getTerritorialGovernanceProfile(state, factionId);
  const shortfalls = buildTerritorialGovernanceShortfalls(profile);
  const acceptanceShortfalls = buildTerritorialGovernanceAcceptanceShortfalls(profile);
  const victoryShortfalls = buildTerritorialGovernanceVictoryShortfalls(profile);
  const requiredSustainSeconds = recognition.requiredSustainSeconds ?? TERRITORIAL_GOVERNANCE_SUSTAIN_SECONDS;
  const sustainedSeconds = Math.max(0, recognition.sustainedSeconds ?? 0);
  const requiredVictorySeconds = recognition.requiredVictorySeconds ?? TERRITORIAL_GOVERNANCE_VICTORY_SECONDS;
  const victoryHoldSeconds = Math.max(0, recognition.victoryHoldSeconds ?? 0);
  return {
    ...recognition,
    territoryShare: profile.territoryShare,
    territorySharePct: profile.territorySharePct,
    territoryCount: profile.territoryCount,
    totalTerritoryCount: profile.totalTerritoryCount,
    loyalTerritoryCount: profile.loyalTerritoryCount,
    victoryLoyalTerritoryCount: profile.victoryLoyalTerritoryCount,
    contestedTerritoryCount: profile.contestedTerritoryCount,
    unstableTerritoryCount: profile.unstableTerritoryCount,
    courtAverageLoyalty: profile.courtAverageLoyalty,
    courtWeakestLoyalty: profile.courtWeakestLoyalty,
    courtLoyaltyThreshold: profile.courtLoyaltyThreshold,
    seatCoverageCount: profile.seatCoverageCount,
    requiredSeatCoverageCount: profile.requiredSeatCoverageCount,
    territorySeatCoverageCount: profile.territorySeatCoverageCount,
    primarySeatId: profile.primarySeatId,
    primarySeatName: profile.primarySeatName,
    primarySeatGoverned: profile.primarySeatGoverned,
    activeLesserHouseCount: profile.activeLesserHouseCount,
    stressedLesserHouseCount: profile.stressedLesserHouseCount,
    defectedBranchCount: profile.defectedBranchCount,
    activeHolyWarCount: profile.activeHolyWarCount,
    incomingHolyWarCount: profile.incomingHolyWarCount,
    weakestTerritoryId: profile.weakestTerritoryId,
    weakestTerritoryName: profile.weakestTerritoryName,
    weakestTerritoryLoyalty: profile.weakestTerritoryLoyalty,
    weakestTerritoryControlState: profile.weakestTerritoryControlState,
    weakestTerritoryContested: profile.weakestTerritoryContested,
    requiredShare: profile.requiredShare,
    requiredSharePct: profile.requiredSharePct,
    requiredSustainSeconds,
    sustainedSeconds: Math.round(sustainedSeconds * 10) / 10,
    sustainedProgressPct: Math.min(100, Math.round((sustainedSeconds / Math.max(1, requiredSustainSeconds)) * 100)),
    remainingSeconds: Math.max(0, Math.round((requiredSustainSeconds - sustainedSeconds) * 10) / 10),
    requiredVictorySeconds,
    victoryHoldSeconds: Math.round(victoryHoldSeconds * 10) / 10,
    victoryProgressPct: Math.min(100, Math.round((victoryHoldSeconds / Math.max(1, requiredVictorySeconds)) * 100)),
    victoryRemainingSeconds: Math.max(0, Math.round((requiredVictorySeconds - victoryHoldSeconds) * 10) / 10),
    integrationReady: profile.integrationReady,
    populationAcceptancePct: profile.populationAcceptancePct,
    populationAcceptanceTargetPct: profile.populationAcceptanceTargetPct,
    populationAcceptanceThresholdPct: profile.populationAcceptanceThresholdPct,
    populationAcceptanceAllianceThresholdPct: profile.populationAcceptanceAllianceThresholdPct,
    populationAcceptanceGapPct: profile.populationAcceptanceGapPct,
    populationAcceptanceReady: profile.populationAcceptanceReady,
    populationAcceptanceTierId: profile.populationAcceptanceTierId,
    populationAcceptanceTierLabel: profile.populationAcceptanceTierLabel,
    populationAcceptanceTrendPerSecond: profile.populationAcceptanceTrendPerSecond,
    populationAcceptanceRiseRate: profile.populationAcceptanceRiseRate,
    populationAcceptanceFallRate: profile.populationAcceptanceFallRate,
    activeMarriageCount: profile.activeMarriageCount,
    marriageEnvoyAvailable: profile.marriageEnvoyAvailable,
    marriageAuthorityAvailable: profile.marriageAuthorityAvailable,
    legitimacy: profile.legitimacy,
    foodRatio: profile.foodRatio,
    waterRatio: profile.waterRatio,
    completedBuildingCount: profile.completedBuildingCount,
    infrastructurePerTerritory: profile.infrastructurePerTerritory,
    raidedInfrastructureCount: profile.raidedInfrastructureCount,
    harassedWorkerCount: profile.harassedWorkerCount,
    fieldWaterStrainedUnits: profile.fieldWaterStrainedUnits,
    fieldWaterCriticalUnits: profile.fieldWaterCriticalUnits,
    unsuppliedEngineCount: profile.unsuppliedEngineCount,
    hostileKingdomCount: profile.hostileKingdomCount,
    worldPressureLevel: profile.worldPressureLevel,
    alliancePressureActive: profile.alliancePressureActive,
    alliancePressureHostileCount: profile.alliancePressureHostileCount,
    alliancePressureDrag: profile.alliancePressureDrag,
    // Session 89: conviction, covenant, and tribal acceptance factors.
    convictionBandId: profile.convictionBandId,
    convictionAcceptanceModifier: profile.convictionAcceptanceModifier,
    covenantTestPassed: profile.covenantTestPassed,
    hasGrandSanctuary: profile.hasGrandSanctuary,
    covenantEndorsement: profile.covenantEndorsement,
    activeTribeUnits: profile.activeTribeUnits,
    tribalFriction: profile.tribalFriction,
    // Session 93: Trueborn City endorsement.
    truebornStanding: profile.truebornStanding,
    truebornEndorsement: profile.truebornEndorsement,
    shortfalls,
    acceptanceShortfalls,
    victoryShortfalls,
    holdReady: profile.holdReady,
    victoryReady: profile.victoryReady,
    recognized: Boolean(recognition.recognizedAt),
    completed: Boolean(recognition.completedAt),
    statusText: recognition.recognizedAt
      ? recognition.completedAt
        ? `Territorial Governance victory secured at ${profile.territorySharePct}% territory share with every held march at ${profile.victoryLoyalThreshold}+ loyalty and ${profile.populationAcceptancePct}% population acceptance.`
        : profile.victoryReady
          ? `${profile.victoryLoyalTerritoryCount}/${profile.territoryCount} integrated marches | acceptance ${profile.populationAcceptancePct}%/${profile.populationAcceptanceThresholdPct}% | ${Math.round(victoryHoldSeconds)}s/${requiredVictorySeconds}s sovereignty hold | seats ${profile.seatCoverageCount}/${profile.requiredSeatCoverageCount}.`
          : profile.integrationReady
            ? `Recognition holds at ${profile.victoryLoyalTerritoryCount}/${profile.territoryCount} integrated marches with ${profile.populationAcceptancePct}%/${profile.populationAcceptanceThresholdPct}% acceptance. ${acceptanceShortfalls[0] ?? "Keep prosperity, legitimacy, and diplomacy rising."}`
            : `Recognition holds at ${profile.loyalTerritoryCount}/${profile.territoryCount} loyal territories and ${profile.territorySharePct}% territory share. ${victoryShortfalls[0] ?? "Deepen integration to full sovereignty."}`
      : `${profile.loyalTerritoryCount}/${profile.territoryCount} loyal territories | ${Math.round(sustainedSeconds)}s/${requiredSustainSeconds}s held | acceptance ${profile.populationAcceptancePct}%/${profile.populationAcceptanceThresholdPct}% | seats ${profile.seatCoverageCount}/${profile.requiredSeatCoverageCount} | weakest ${profile.weakestTerritoryName ?? "march"} ${profile.weakestTerritoryLoyalty}.`,
  };
}

function shouldIssueTerritorialGovernanceRecognition(state, factionId) {
  const faction = getFaction(state, factionId);
  if (!faction?.dynasty || faction.kind !== "kingdom") {
    return false;
  }
  if (getActiveTerritorialGovernanceRecognition(faction)) {
    return false;
  }
  return getTerritorialGovernanceProfile(state, factionId).triggerReady;
}

function issueTerritorialGovernanceRecognition(state, factionId) {
  const faction = getFaction(state, factionId);
  const dynasty = getDynastyTerritorialGovernanceState(faction);
  const profile = getTerritorialGovernanceProfile(state, factionId);
  if (!dynasty || !profile.triggerReady) {
    return null;
  }
  const recognition = {
    id: createEntityId(state, "territorialGovernance"),
    type: "territorial_governance_recognition",
    label: "Territorial Governance Recognition",
    startedAt: state.meta.elapsed,
    startedAtInWorldDays: state.dualClock?.inWorldDays ?? 0,
    requiredSustainSeconds: TERRITORIAL_GOVERNANCE_SUSTAIN_SECONDS,
    sustainedSeconds: 0,
    requiredVictorySeconds: TERRITORIAL_GOVERNANCE_VICTORY_SECONDS,
    victoryHoldSeconds: 0,
    territoryShare: profile.territoryShare,
    territorySharePct: profile.territorySharePct,
    territoryCount: profile.territoryCount,
    loyalThreshold: profile.loyalThreshold,
    victoryLoyalThreshold: profile.victoryLoyalThreshold,
    breakThreshold: profile.breakThreshold,
    targetControlPointId: profile.weakestTerritoryId,
    targetControlPointName: profile.weakestTerritoryName,
    populationAcceptancePct: profile.populationAcceptanceSeedPct,
    populationAcceptanceTargetPct: profile.populationAcceptanceTargetPct,
    populationAcceptanceThresholdPct: profile.populationAcceptanceThresholdPct,
    populationAcceptanceAllianceThresholdPct: profile.populationAcceptanceAllianceThresholdPct,
    populationAcceptanceTierId: profile.populationAcceptanceTierId,
    populationAcceptanceTierLabel: profile.populationAcceptanceTierLabel,
    populationAcceptanceReady: profile.populationAcceptanceReady,
    integrationReady: profile.integrationReady,
    victoryReady: profile.victoryReady,
    lastAcceptanceTierId: profile.populationAcceptanceTierId,
    recognizedAt: null,
    recognizedAtInWorldDays: null,
    completedAt: null,
    completedAtInWorldDays: null,
    lastReminderAtInWorldDays: state.dualClock?.inWorldDays ?? 0,
    failedAt: null,
  };
  dynasty.territorialGovernanceRecognition = recognition;
  pushMessage(
    state,
    `${getFactionDisplayName(state, factionId)} reaches Territorial Governance Recognition. Coalition pressure turns toward its weakest loyal marches.`,
    factionId === "player" ? "warn" : "info",
  );
  declareInWorldTime(state, 104, `Territorial Governance Recognition: ${getFactionDisplayName(state, factionId)}`);
  return recognition;
}

function breakTerritorialGovernanceRecognition(state, factionId, recognition, reason) {
  const faction = getFaction(state, factionId);
  const dynasty = getDynastyTerritorialGovernanceState(faction);
  if (!dynasty || !recognition) {
    return;
  }
  dynasty.lastTerritorialGovernanceOutcome = {
    outcome: recognition.recognizedAt ? "broken_after_recognition" : "broken",
    reason,
    startedAt: recognition.startedAt,
    startedAtInWorldDays: recognition.startedAtInWorldDays,
    endedAt: state.meta.elapsed,
    endedAtInWorldDays: state.dualClock?.inWorldDays ?? 0,
    sustainedSeconds: Math.round((recognition.sustainedSeconds ?? 0) * 10) / 10,
    victoryHoldSeconds: Math.round((recognition.victoryHoldSeconds ?? 0) * 10) / 10,
  };
  dynasty.territorialGovernanceRecognition = null;
  pushMessage(
    state,
    `${getFactionDisplayName(state, factionId)} loses Territorial Governance Recognition: ${reason}.`,
    factionId === "player" ? "warn" : "good",
  );
  declareInWorldTime(state, 112, `Territorial Governance broken: ${getFactionDisplayName(state, factionId)}`);
}

function establishTerritorialGovernanceRecognition(state, factionId, recognition) {
  const faction = getFaction(state, factionId);
  const dynasty = getDynastyTerritorialGovernanceState(faction);
  if (!dynasty || !recognition || recognition.recognizedAt) {
    return;
  }
  recognition.recognizedAt = state.meta.elapsed;
  recognition.recognizedAtInWorldDays = state.dualClock?.inWorldDays ?? 0;
  recognition.victoryHoldSeconds = 0;
  dynasty.lastTerritorialGovernanceOutcome = {
    outcome: "recognized",
    startedAt: recognition.startedAt,
    startedAtInWorldDays: recognition.startedAtInWorldDays,
    recognizedAt: recognition.recognizedAt,
    recognizedAtInWorldDays: recognition.recognizedAtInWorldDays,
    sustainedSeconds: Math.round((recognition.sustainedSeconds ?? 0) * 10) / 10,
  };
  pushMessage(
    state,
    `${getFactionDisplayName(state, factionId)} holds Territorial Governance Recognition. Rival coalitions intensify pressure on the governed frontier.`,
    factionId === "player" ? "good" : "warn",
  );
  declareInWorldTime(state, 128, `Territorial Governance held: ${getFactionDisplayName(state, factionId)}`);
}

function completeTerritorialGovernanceRecognition(state, factionId, recognition) {
  const faction = getFaction(state, factionId);
  const dynasty = getDynastyTerritorialGovernanceState(faction);
  if (!dynasty || !recognition || recognition.completedAt || state.meta.status !== "playing") {
    return;
  }
  recognition.completedAt = state.meta.elapsed;
  recognition.completedAtInWorldDays = state.dualClock?.inWorldDays ?? 0;
  dynasty.lastTerritorialGovernanceOutcome = {
    outcome: "completed",
    startedAt: recognition.startedAt,
    startedAtInWorldDays: recognition.startedAtInWorldDays,
    recognizedAt: recognition.recognizedAt,
    recognizedAtInWorldDays: recognition.recognizedAtInWorldDays,
    completedAt: recognition.completedAt,
    completedAtInWorldDays: recognition.completedAtInWorldDays,
    sustainedSeconds: Math.round((recognition.sustainedSeconds ?? 0) * 10) / 10,
    victoryHoldSeconds: Math.round((recognition.victoryHoldSeconds ?? 0) * 10) / 10,
  };
  declareInWorldTime(
    state,
    144,
    `Territorial Governance recognized: ${getFactionDisplayName(state, factionId)}`,
  );
  state.meta.status = faction.id === "player" ? "won" : "lost";
  state.meta.winnerId = faction.id;
  state.meta.victoryType = "territorial_governance";
  state.meta.victoryReason = `${getFactionDisplayName(state, faction.id)} secured Territorial Governance through loyal integrated marches.`;
  pushMessage(
    state,
    `${getFactionDisplayName(state, faction.id)} secures Territorial Governance. The realm's loyal marches accept dynastic sovereignty in full.`,
    faction.id === "player" ? "good" : "warn",
  );
}

function announceTerritorialGovernanceAcceptanceTierShift(state, factionId, previousTierId, nextProfile) {
  const nextTierId = nextProfile.populationAcceptanceTierId;
  if (!nextTierId || previousTierId === nextTierId) {
    return;
  }
  if (nextTierId === "threshold") {
    pushMessage(
      state,
      `${getFactionDisplayName(state, factionId)} nears the 65% population-acceptance threshold. Rival courts harden alliance-threshold pressure.`,
      factionId === "player" ? "warn" : "info",
    );
    return;
  }
  if (nextTierId === "accepted") {
    pushMessage(
      state,
      `${getFactionDisplayName(state, factionId)} reaches ${nextProfile.populationAcceptanceThresholdPct}% population acceptance. Full sovereignty can now mature if integration holds.`,
      factionId === "player" ? "good" : "warn",
    );
    return;
  }
  if (previousTierId === "accepted" && nextTierId !== "accepted") {
    pushMessage(
      state,
      `${getFactionDisplayName(state, factionId)} falls back below the ${nextProfile.populationAcceptanceThresholdPct}% population-acceptance threshold. Sovereignty cannot mature until it recovers.`,
      factionId === "player" ? "warn" : "good",
    );
  }
}

function tickTerritorialGovernanceRecognition(state, dt) {
  const currentInWorldDays = state.dualClock?.inWorldDays ?? 0;
  const currentDay = Math.floor(currentInWorldDays);
  Object.values(state.factions).forEach((faction) => {
    if (!faction?.dynasty || faction.kind !== "kingdom") {
      return;
    }
    if (shouldIssueTerritorialGovernanceRecognition(state, faction.id)) {
      issueTerritorialGovernanceRecognition(state, faction.id);
    }
    const recognition = getActiveTerritorialGovernanceRecognition(faction);
    if (!recognition) {
      return;
    }
    const initialProfile = getTerritorialGovernanceProfile(state, faction.id);
    recognition.populationAcceptanceThresholdPct = initialProfile.populationAcceptanceThresholdPct;
    recognition.populationAcceptanceAllianceThresholdPct = initialProfile.populationAcceptanceAllianceThresholdPct;
    recognition.populationAcceptanceTargetPct = initialProfile.populationAcceptanceTargetPct;
    const currentAcceptance = clamp(
      recognition.populationAcceptancePct ?? initialProfile.populationAcceptancePct,
      0,
      100,
    );
    const targetAcceptance = initialProfile.populationAcceptanceTargetPct ?? currentAcceptance;
    const acceptanceDelta = targetAcceptance - currentAcceptance;
    const acceptanceRate = acceptanceDelta >= 0
      ? initialProfile.populationAcceptanceRiseRate ?? 0
      : initialProfile.populationAcceptanceFallRate ?? 0;
    recognition.populationAcceptancePct = Math.round(clamp(
      currentAcceptance + (Math.sign(acceptanceDelta) * Math.min(Math.abs(acceptanceDelta), acceptanceRate * dt)),
      0,
      100,
    ) * 10) / 10;
    const profile = getTerritorialGovernanceProfile(state, faction.id);
    recognition.territoryShare = profile.territoryShare;
    recognition.territorySharePct = profile.territorySharePct;
    recognition.territoryCount = profile.territoryCount;
    recognition.targetControlPointId = profile.weakestTerritoryId;
    recognition.targetControlPointName = profile.weakestTerritoryName;
    recognition.populationAcceptancePct = profile.populationAcceptancePct;
    recognition.populationAcceptanceTargetPct = profile.populationAcceptanceTargetPct;
    recognition.populationAcceptanceThresholdPct = profile.populationAcceptanceThresholdPct;
    recognition.populationAcceptanceAllianceThresholdPct = profile.populationAcceptanceAllianceThresholdPct;
    recognition.populationAcceptanceTierId = profile.populationAcceptanceTierId;
    recognition.populationAcceptanceTierLabel = profile.populationAcceptanceTierLabel;
    recognition.populationAcceptanceReady = profile.populationAcceptanceReady;
    recognition.integrationReady = profile.integrationReady;
    recognition.victoryReady = profile.victoryReady;
    announceTerritorialGovernanceAcceptanceTierShift(
      state,
      faction.id,
      recognition.lastAcceptanceTierId ?? initialProfile.populationAcceptanceTierId,
      profile,
    );
    recognition.lastAcceptanceTierId = profile.populationAcceptanceTierId;
    if (!profile.triggerReady) {
      breakTerritorialGovernanceRecognition(
        state,
        faction.id,
        recognition,
        `the weakest held march fell to ${profile.weakestTerritoryLoyalty} loyalty or the realm dropped below ${profile.requiredSharePct}% territory share`,
      );
      return;
    }
    if (profile.holdReady) {
      recognition.sustainedSeconds = Math.min(
        recognition.requiredSustainSeconds ?? TERRITORIAL_GOVERNANCE_SUSTAIN_SECONDS,
        (recognition.sustainedSeconds ?? 0) + dt,
      );
      if (!recognition.recognizedAt &&
        (recognition.sustainedSeconds ?? 0) >= (recognition.requiredSustainSeconds ?? TERRITORIAL_GOVERNANCE_SUSTAIN_SECONDS)) {
        establishTerritorialGovernanceRecognition(state, faction.id, recognition);
      }
    } else if (!recognition.recognizedAt) {
      recognition.sustainedSeconds = 0;
    } else if (recognition.recognizedAt) {
      breakTerritorialGovernanceRecognition(
        state,
        faction.id,
        recognition,
        buildTerritorialGovernanceShortfalls(profile)[0] ?? "the sustained governance hold collapsed",
      );
      return;
    }

    if (recognition.recognizedAt && !recognition.completedAt) {
      if (profile.victoryReady) {
        recognition.victoryHoldSeconds = Math.min(
          recognition.requiredVictorySeconds ?? TERRITORIAL_GOVERNANCE_VICTORY_SECONDS,
          (recognition.victoryHoldSeconds ?? 0) + dt,
        );
        if ((recognition.victoryHoldSeconds ?? 0) >= (recognition.requiredVictorySeconds ?? TERRITORIAL_GOVERNANCE_VICTORY_SECONDS)) {
          completeTerritorialGovernanceRecognition(state, faction.id, recognition);
          return;
        }
      } else {
        recognition.victoryHoldSeconds = 0;
      }
    }

    const lastReminderDay = Math.floor(recognition.lastReminderAtInWorldDays ?? recognition.startedAtInWorldDays ?? currentInWorldDays);
    if ((currentDay - lastReminderDay) >= 45) {
      recognition.lastReminderAtInWorldDays = currentDay;
      pushMessage(
        state,
        `${getFactionDisplayName(state, faction.id)} still holds every governed march near the Territorial Governance threshold.`,
        faction.id === "player" ? "warn" : "info",
      );
    }
  });
}

function getLowestLoyaltyControlPoint(state, factionId) {
  return state.world.controlPoints
    .filter((controlPoint) => controlPoint.ownerFactionId === factionId)
    .sort((left, right) => left.loyalty - right.loyalty)[0] ?? null;
}

function getConvictionBand(content, score) {
  return content.convictionStates.find((entry) => score >= entry.minScore) ?? content.convictionStates[content.convictionStates.length - 1];
}

// Session 18: Conviction milestone powers per band.
// Each canonical band unlocks a distinct passive modifier on already-live systems.
// Master doctrine section XX. Keeps conviction INDEPENDENT of faith — these modifiers
// stack multiplicatively with doctrine effects, never replace them.
const CONVICTION_BAND_EFFECTS = {
  apex_moral: {
    label: "Apex Moral",
    stabilizationMultiplier: 1.22,
    reserveHealMultiplier: 1.12,
    loyaltyProtectionMultiplier: 1.18,
    captureMultiplier: 0.94,
    populationGrowthMultiplier: 1.08,
    attackMultiplier: 1,
  },
  moral: {
    label: "Moral",
    stabilizationMultiplier: 1.08,
    reserveHealMultiplier: 1.04,
    loyaltyProtectionMultiplier: 1.08,
    captureMultiplier: 0.98,
    populationGrowthMultiplier: 1.03,
    attackMultiplier: 1,
  },
  neutral: {
    label: "Neutral",
    stabilizationMultiplier: 1,
    reserveHealMultiplier: 1,
    loyaltyProtectionMultiplier: 1,
    captureMultiplier: 1,
    populationGrowthMultiplier: 1,
    attackMultiplier: 1,
  },
  cruel: {
    label: "Cruel",
    stabilizationMultiplier: 0.96,
    reserveHealMultiplier: 0.98,
    loyaltyProtectionMultiplier: 0.94,
    captureMultiplier: 1.08,
    populationGrowthMultiplier: 0.97,
    attackMultiplier: 1.04,
  },
  apex_cruel: {
    label: "Apex Cruel",
    stabilizationMultiplier: 0.88,
    reserveHealMultiplier: 0.92,
    loyaltyProtectionMultiplier: 0.82,
    captureMultiplier: 1.22,
    populationGrowthMultiplier: 0.92,
    attackMultiplier: 1.12,
  },
};

function getConvictionBandEffects(state, factionId) {
  const faction = getFaction(state, factionId);
  const bandId = faction?.conviction?.bandId ?? "neutral";
  return CONVICTION_BAND_EFFECTS[bandId] ?? CONVICTION_BAND_EFFECTS.neutral;
}

function getFaithTier(intensity) {
  return FAITH_INTENSITY_TIERS.find((tier) => intensity >= tier.min) ?? FAITH_INTENSITY_TIERS[FAITH_INTENSITY_TIERS.length - 1];
}

function syncFaithIntensityState(faction) {
  if (!faction?.faith) {
    return null;
  }
  const intensity = clamp(faction.faith.intensity ?? 0, 0, 100);
  const tier = getFaithTier(intensity);
  faction.faith.intensity = intensity;
  faction.faith.level = tier.level;
  faction.faith.tierLabel = tier.label;
  return tier;
}

function noteFaithDiscovery(faction, faithId) {
  if (!faction?.faith || !faithId) {
    return;
  }
  faction.faith.discoveredFaithIds = faction.faith.discoveredFaithIds ?? [];
  if (!faction.faith.discoveredFaithIds.includes(faithId)) {
    faction.faith.discoveredFaithIds.push(faithId);
  }
}

function getBuildingWorldRect(state, building, buildingDef) {
  const size = state.world.tileSize;
  return {
    x: building.tileX * size,
    y: building.tileY * size,
    w: buildingDef.footprint.w * size,
    h: buildingDef.footprint.h * size,
  };
}

function getBuildingCenter(state, building, buildingDef) {
  const rect = getBuildingWorldRect(state, building, buildingDef);
  return {
    x: rect.x + rect.w / 2,
    y: rect.y + rect.h / 2,
  };
}

function getEntity(state, entityType, entityId) {
  if (entityType === "unit") {
    return state.units.find((unit) => unit.id === entityId);
  }
  if (entityType === "building") {
    return state.buildings.find((building) => building.id === entityId);
  }
  return null;
}

function clearFortificationDuty(unit) {
  unit.reserveDuty = null;
  unit.reserveSettlementId = null;
}

function isSupportRole(unitDef) {
  return unitDef?.role === "engineer-specialist" || unitDef?.role === "support";
}

function getAliveUnits(state, factionId) {
  return state.units.filter((unit) => unit.factionId === factionId && unit.health > 0);
}

function getAliveCombatUnits(state, factionId) {
  return getAliveUnits(state, factionId).filter((unit) => {
    const unitDef = getUnitDef(state.content, unit.typeId);
    return unitDef.role !== "worker" && !isSupportRole(unitDef);
  });
}

function getCompletedBuildings(state, factionId) {
  return state.buildings.filter((building) => building.factionId === factionId && building.completed && building.health > 0);
}

function getSupplyCampBuildings(state, factionId) {
  return getCompletedBuildings(state, factionId).filter((building) =>
    getBuildingDef(state.content, building.typeId)?.supportsSiegeLogistics &&
    !isBuildingUnderScoutRaid(state, building));
}

function isScoutRiderUnitDef(unitDef) {
  return unitDef?.role === "light-cavalry" && (unitDef?.raidDurationSeconds ?? 0) > 0;
}

function isMovingLogisticsCarrierUnitDef(unitDef) {
  return unitDef?.role === "support" && unitDef?.movingLogisticsCarrier === true;
}

function isScoutRaidableUnitDef(unitDef) {
  return unitDef?.role === "worker" || isMovingLogisticsCarrierUnitDef(unitDef);
}

function isSupplyWagonInterdicted(state, unit) {
  return Boolean(unit) && (unit.logisticsInterdictedUntil ?? 0) > (state.meta.elapsed ?? 0);
}

export function isSupplyWagonRecovering(state, unit) {
  return Boolean(unit) &&
    !isSupplyWagonInterdicted(state, unit) &&
    (unit.convoyRecoveryUntil ?? 0) > (state.meta.elapsed ?? 0);
}

export function getSupplyWagonEscortCoverage(state, factionId, wagon, escortUnits = null) {
  if (!wagon || wagon.health <= 0 || !isMovingLogisticsCarrierUnitDef(getUnitDef(state.content, wagon.typeId))) {
    return {
      escortCount: 0,
      requiredEscortCount: CONVOY_ESCORT_MIN_ESCORTS,
      screenRadius: CONVOY_ESCORT_SCREEN_RADIUS,
      screened: false,
      escorts: [],
    };
  }
  const escortPool = Array.isArray(escortUnits)
    ? escortUnits
    : state.units.filter((unit) => {
      if (unit.health <= 0 || unit.factionId !== (factionId ?? wagon.factionId) || unit.id === wagon.id) {
        return false;
      }
      const unitDef = getUnitDef(state.content, unit.typeId);
      return Boolean(unitDef) &&
        unitDef.role !== "worker" &&
        unitDef.role !== "engineer-specialist" &&
        unitDef.role !== "support" &&
        !unitDef.siegeClass;
    });
  const escorts = escortPool.filter((unit) =>
    distance(unit.x, unit.y, wagon.x, wagon.y) <= CONVOY_ESCORT_SCREEN_RADIUS);
  const requiredEscortCount = isSupplyWagonRecovering(state, wagon)
    ? CONVOY_ESCORT_MIN_ESCORTS
    : 1;
  return {
    escortCount: escorts.length,
    requiredEscortCount,
    screenRadius: CONVOY_ESCORT_SCREEN_RADIUS,
    screened: escorts.length >= requiredEscortCount,
    escorts,
  };
}

function isBuildingUnderScoutRaid(state, building) {
  return Boolean(building) && (building.raidedUntil ?? 0) > (state.meta.elapsed ?? 0);
}

function isResourceNodeUnderScoutHarassment(state, resourceNode, factionId = null) {
  if (!resourceNode || (resourceNode.harassedUntil ?? 0) <= (state.meta.elapsed ?? 0)) {
    return false;
  }
  if (!factionId) {
    return true;
  }
  return !resourceNode.harassedTargetFactionId || resourceNode.harassedTargetFactionId === factionId;
}

function getResourceNodePosition(state, resourceNode) {
  if (!resourceNode) {
    return null;
  }
  return {
    x: resourceNode.x * state.world.tileSize,
    y: resourceNode.y * state.world.tileSize,
  };
}

function getRaidTargetPosition(state, targetType, target) {
  if (!target) {
    return null;
  }
  if (targetType === "building") {
    return getBuildingCenter(state, target, getBuildingDef(state.content, target.typeId));
  }
  if (targetType === "resource") {
    return getResourceNodePosition(state, target);
  }
  if (targetType === "unit") {
    return { x: target.x, y: target.y };
  }
  return null;
}

function getNearestHostileOwnedControlPointToPosition(state, raiderFactionId, x, y, maxRadius = SCOUT_RAID_LOYALTY_RADIUS) {
  return state.world.controlPoints
    .filter((controlPoint) =>
      controlPoint.ownerFactionId &&
      controlPoint.ownerFactionId !== raiderFactionId &&
      areHostile(state, raiderFactionId, controlPoint.ownerFactionId))
    .map((controlPoint) => {
      const position = getControlPointPosition(state, controlPoint);
      return {
        controlPoint,
        distance: distance(x, y, position.x, position.y),
      };
    })
    .filter((entry) => entry.distance <= maxRadius)
    .sort((left, right) => left.distance - right.distance)[0]?.controlPoint ?? null;
}

function getScoutResourceHarassTargetFactionId(state, resourceNode, raiderFactionId) {
  if (!resourceNode || !raiderFactionId) {
    return null;
  }
  if (
    isResourceNodeUnderScoutHarassment(state, resourceNode) &&
    resourceNode.harassedTargetFactionId &&
    areHostile(state, raiderFactionId, resourceNode.harassedTargetFactionId)
  ) {
    return resourceNode.harassedTargetFactionId;
  }
  const nodePosition = getResourceNodePosition(state, resourceNode);
  if (!nodePosition) {
    return null;
  }
  const worker = state.units
    .filter((candidate) => {
      if (candidate.health <= 0 || candidate.factionId === raiderFactionId) {
        return false;
      }
      if (!areHostile(state, raiderFactionId, candidate.factionId)) {
        return false;
      }
      const unitDef = getUnitDef(state.content, candidate.typeId);
      if (!unitDef || unitDef.role !== "worker") {
        return false;
      }
      if (candidate.command?.type === "gather" && candidate.command.nodeId === resourceNode.id) {
        return true;
      }
      if (candidate.command?.type === "harass_retreat" && candidate.command.nodeId === resourceNode.id) {
        return true;
      }
      return distance(candidate.x, candidate.y, nodePosition.x, nodePosition.y) <= SCOUT_NODE_HARASS_RADIUS + 24;
    })
    .sort((left, right) =>
      distance(left.x, left.y, nodePosition.x, nodePosition.y) -
      distance(right.x, right.y, nodePosition.x, nodePosition.y))[0];
  if (worker) {
    return worker.factionId;
  }
  return getNearestHostileOwnedControlPointToPosition(
    state,
    raiderFactionId,
    nodePosition.x,
    nodePosition.y,
    SCOUT_RAID_LOYALTY_RADIUS,
  )?.ownerFactionId ?? null;
}

function getLinkedSupplyCampForWagon(state, wagon, supplyCamps = null) {
  if (!wagon || wagon.health <= 0) {
    return null;
  }
  const wagonDef = getUnitDef(state.content, wagon.typeId);
  if (!isMovingLogisticsCarrierUnitDef(wagonDef)) {
    return null;
  }
  const linkedSupplyCamps = (supplyCamps ?? getSupplyCampBuildings(state, wagon.factionId))
    .filter((camp) => {
      if ((camp.poisonedUntil ?? 0) > state.meta.elapsed) {
        return false;
      }
      if (isBuildingUnderScoutRaid(state, camp)) {
        return false;
      }
      const campDef = getBuildingDef(state.content, camp.typeId);
      const campCenter = getBuildingCenter(state, camp, campDef);
      const maxLinkRadius = Math.max(campDef.supplyLinkRadius ?? 0, wagonDef.campLinkRadius ?? 0);
      return distance(wagon.x, wagon.y, campCenter.x, campCenter.y) <= maxLinkRadius;
    })
    .sort((left, right) => {
      const leftCenter = getBuildingCenter(state, left, getBuildingDef(state.content, left.typeId));
      const rightCenter = getBuildingCenter(state, right, getBuildingDef(state.content, right.typeId));
      return distance(wagon.x, wagon.y, leftCenter.x, leftCenter.y) - distance(wagon.x, wagon.y, rightCenter.x, rightCenter.y);
    });
  return linkedSupplyCamps[0] ?? null;
}

function getNearestFriendlyRefugePosition(state, factionId, fromX, fromY) {
  const buildingTarget = getCompletedBuildings(state, factionId)
    .filter((building) => building.health > 0 && !isBuildingUnderScoutRaid(state, building))
    .map((building) => {
      const buildingDef = getBuildingDef(state.content, building.typeId);
      const center = getBuildingCenter(state, building, buildingDef);
      return {
        x: center.x,
        y: center.y,
        distance: distance(fromX, fromY, center.x, center.y),
      };
    })
    .sort((left, right) => left.distance - right.distance)[0];
  if (buildingTarget) {
    return { x: buildingTarget.x, y: buildingTarget.y };
  }
  const controlPointTarget = state.world.controlPoints
    .filter((controlPoint) => controlPoint.ownerFactionId === factionId)
    .map((controlPoint) => {
      const position = getControlPointPosition(state, controlPoint);
      return {
        x: position.x,
        y: position.y,
        distance: distance(fromX, fromY, position.x, position.y),
      };
    })
    .sort((left, right) => left.distance - right.distance)[0];
  return controlPointTarget ? { x: controlPointTarget.x, y: controlPointTarget.y } : null;
}

function routeWorkerToHarassRetreat(state, worker, resourceNode, attackerFactionId, retreatSeconds) {
  const resourceNodePosition = resourceNode ? getResourceNodePosition(state, resourceNode) : { x: worker.x, y: worker.y };
  const fallbackRetreat = getRaidRetreatCommand(state, worker, resourceNodePosition);
  const refuge = getNearestFriendlyRefugePosition(state, worker.factionId, worker.x, worker.y) ?? {
    x: fallbackRetreat.x,
    y: fallbackRetreat.y,
  };
  if (worker.carrying && resourceNode && worker.carrying.type === resourceNode.type) {
    resourceNode.amount += worker.carrying.amount;
  }
  clearFortificationDuty(worker);
  worker.carrying = null;
  worker.gatherProgress = 0;
  worker.command = {
    type: "harass_retreat",
    x: refuge.x,
    y: refuge.y,
    nodeId: resourceNode?.id ?? null,
    resumeAt: state.meta.elapsed + retreatSeconds,
    harassedByFactionId: attackerFactionId ?? null,
  };
}

function routeWorkersFromHarassedNode(state, targetFactionId, attackerFactionId, resourceNode, retreatSeconds, harassRadius) {
  const nodePosition = getResourceNodePosition(state, resourceNode);
  if (!nodePosition) {
    return 0;
  }
  const impactedWorkers = state.units.filter((worker) => {
    if (worker.health <= 0 || worker.factionId !== targetFactionId) {
      return false;
    }
    const unitDef = getUnitDef(state.content, worker.typeId);
    if (!unitDef || unitDef.role !== "worker") {
      return false;
    }
    if (worker.command?.type === "gather" && worker.command.nodeId === resourceNode.id) {
      return true;
    }
    if (worker.command?.type === "harass_retreat" && worker.command.nodeId === resourceNode.id) {
      return true;
    }
    return distance(worker.x, worker.y, nodePosition.x, nodePosition.y) <= harassRadius;
  });
  impactedWorkers.forEach((worker) => {
    routeWorkerToHarassRetreat(state, worker, resourceNode, attackerFactionId, retreatSeconds);
  });
  return impactedWorkers.length;
}

function isScoutRaidTargetDefended(state, raiderFactionId, targetFactionId, targetPosition, defenseRadius) {
  if (!targetPosition || !raiderFactionId || !targetFactionId) {
    return false;
  }
  return state.units.some((candidate) => {
    if (candidate.health <= 0 || candidate.factionId !== targetFactionId) {
      return false;
    }
    if (!areHostile(state, raiderFactionId, candidate.factionId)) {
      return false;
    }
    const unitDef = getUnitDef(state.content, candidate.typeId);
    if (
      !unitDef ||
      unitDef.role === "worker" ||
      unitDef.role === "support" ||
      unitDef.role === "engineer-specialist" ||
      unitDef.siegeClass
    ) {
      return false;
    }
    return distance(candidate.x, candidate.y, targetPosition.x, targetPosition.y) <= defenseRadius;
  });
}

function applyResourceLoss(resources, loss = {}) {
  const applied = {};
  Object.entries(loss).forEach(([resourceId, amount]) => {
    if (!Number.isFinite(amount) || amount <= 0) {
      return;
    }
    const available = resources[resourceId] ?? 0;
    const drained = Math.min(available, amount);
    resources[resourceId] = Math.max(0, available - amount);
    applied[resourceId] = Math.round(drained * 10) / 10;
  });
  return applied;
}

function formatResourceLossSummary(loss) {
  const entries = Object.entries(loss ?? {}).filter(([, amount]) => amount > 0);
  if (entries.length === 0) {
    return "no stores";
  }
  return entries
    .map(([resourceId, amount]) => `${Math.round(amount * 10) / 10} ${resourceId}`)
    .join(", ");
}

function getNearestOwnedControlPointToPosition(state, factionId, x, y, maxRadius = SCOUT_RAID_LOYALTY_RADIUS) {
  return state.world.controlPoints
    .filter((controlPoint) => controlPoint.ownerFactionId === factionId)
    .map((controlPoint) => {
      const position = getControlPointPosition(state, controlPoint);
      return {
        controlPoint,
        distance: distance(x, y, position.x, position.y),
      };
    })
    .filter((entry) => entry.distance <= maxRadius)
    .sort((left, right) => left.distance - right.distance)[0]?.controlPoint ?? null;
}

function applyLocalRaidLoyaltyShock(state, factionId, x, y, loyaltyDamage) {
  if (!factionId || !Number.isFinite(loyaltyDamage) || loyaltyDamage <= 0) {
    return null;
  }
  const controlPoint = getNearestOwnedControlPointToPosition(state, factionId, x, y);
  if (!controlPoint) {
    return null;
  }
  const governorProfile = getGovernorProfileForControlPoint(state, controlPoint);
  const verdantWardenSupport = getVerdantWardenControlPointSupportProfile(state, controlPoint);
  const adjustedDelta = getProtectedLoyaltyDelta(
    -loyaltyDamage,
    governorProfile.loyaltyProtectionMultiplier * (verdantWardenSupport.loyaltyProtectionMultiplier ?? 1),
  );
  controlPoint.loyalty = Math.max(0, Math.min(100, controlPoint.loyalty + adjustedDelta));
  if (controlPoint.controlState === "stabilized" && controlPoint.loyalty < TERRITORY_STABILIZED_LOYALTY) {
    controlPoint.controlState = "occupied";
  }
  return {
    controlPoint,
    delta: adjustedDelta,
  };
}

function getRaidRetreatCommand(state, unit, targetPosition) {
  const dx = unit.x - targetPosition.x;
  const dy = unit.y - targetPosition.y;
  const length = Math.hypot(dx, dy) || 1;
  const worldWidth = state.world.width * state.world.tileSize;
  const worldHeight = state.world.height * state.world.tileSize;
  return {
    type: "raid_retreat",
    x: clamp(unit.x + (dx / length) * SCOUT_RAID_RETREAT_DISTANCE, 0, worldWidth),
    y: clamp(unit.y + (dy / length) * SCOUT_RAID_RETREAT_DISTANCE, 0, worldHeight),
  };
}

function executeScoutRaid(state, unit, unitDef, building) {
  const buildingDef = getBuildingDef(state.content, building.typeId);
  const targetPosition = getRaidTargetPosition(state, "building", building);
  const targetFaction = getFaction(state, building.factionId);
  if (!buildingDef?.scoutRaidable || !targetPosition || !targetFaction) {
    unit.command = null;
    return false;
  }

  const raidDuration = Math.max(8, unitDef.raidDurationSeconds ?? 20);
  building.raidedUntil = Math.max(building.raidedUntil ?? 0, state.meta.elapsed + raidDuration);
  const resourceLoss = applyResourceLoss(targetFaction.resources, buildingDef.raidLoss ?? {});
  const loyaltyShock = applyLocalRaidLoyaltyShock(
    state,
    building.factionId,
    targetPosition.x,
    targetPosition.y,
    unitDef.raidLoyaltyDamage ?? 0,
  );
  unit.command = getRaidRetreatCommand(state, unit, targetPosition);
  const loyaltyText = loyaltyShock?.controlPoint
    ? ` ${loyaltyShock.controlPoint.name} loyalty shaken.`
    : "";
  pushMessage(
    state,
    `${getFactionDisplayName(state, unit.factionId)} ${unitDef.name} raids ${getFactionDisplayName(state, building.factionId)} ${buildingDef.name}, stripping ${formatResourceLossSummary(resourceLoss)}.${loyaltyText}`,
    building.factionId === "player" ? "warn" : unit.factionId === "player" ? "good" : "info",
  );
  return true;
}

function executeScoutResourceHarass(state, unit, unitDef, resourceNode) {
  const nodePosition = getResourceNodePosition(state, resourceNode);
  const targetFactionId = getScoutResourceHarassTargetFactionId(state, resourceNode, unit.factionId);
  if (!nodePosition || !targetFactionId) {
    unit.command = null;
    return false;
  }
  const harassDuration = Math.max(8, unitDef.nodeHarassDurationSeconds ?? Math.max(12, (unitDef.raidDurationSeconds ?? 0) - 4));
  const retreatSeconds = Math.max(6, unitDef.workerRetreatSeconds ?? Math.min(harassDuration, SCOUT_NODE_RETREAT_SECONDS));
  resourceNode.harassedUntil = Math.max(resourceNode.harassedUntil ?? 0, state.meta.elapsed + harassDuration);
  resourceNode.harassedTargetFactionId = targetFactionId;
  resourceNode.harassedByFactionId = unit.factionId;
  const routedWorkers = routeWorkersFromHarassedNode(
    state,
    targetFactionId,
    unit.factionId,
    resourceNode,
    retreatSeconds,
    unitDef.workerHarassRadius ?? SCOUT_NODE_HARASS_RADIUS,
  );
  const loyaltyShock = applyLocalRaidLoyaltyShock(
    state,
    targetFactionId,
    nodePosition.x,
    nodePosition.y,
    Math.max(3, Math.round((unitDef.raidLoyaltyDamage ?? 0) * 0.5)),
  );
  unit.command = getRaidRetreatCommand(state, unit, nodePosition);
  const workerText = routedWorkers > 0
    ? `routing ${routedWorkers} worker${routedWorkers === 1 ? "" : "s"}`
    : "denying the seam";
  const loyaltyText = loyaltyShock?.controlPoint
    ? ` ${loyaltyShock.controlPoint.name} loyalty shaken.`
    : "";
  pushMessage(
    state,
    `${getFactionDisplayName(state, unit.factionId)} ${unitDef.name} harasses ${getFactionDisplayName(state, targetFactionId)} ${resourceNode.type} node, ${workerText}.${loyaltyText}`,
    targetFactionId === "player" ? "warn" : unit.factionId === "player" ? "good" : "info",
  );
  return true;
}

function executeScoutWorkerHarass(state, unit, unitDef, targetWorker) {
  const workerDef = getUnitDef(state.content, targetWorker.typeId);
  if (!workerDef || workerDef.role !== "worker") {
    unit.command = null;
    return false;
  }
  const sourceNode = targetWorker.command?.nodeId
    ? state.world.resourceNodes.find((resourceNode) => resourceNode.id === targetWorker.command.nodeId)
    : null;
  if (sourceNode) {
    return executeScoutResourceHarass(state, unit, unitDef, sourceNode);
  }
  const retreatSeconds = Math.max(6, unitDef.workerRetreatSeconds ?? SCOUT_NODE_RETREAT_SECONDS);
  routeWorkerToHarassRetreat(state, targetWorker, null, unit.factionId, retreatSeconds);
  const loyaltyShock = applyLocalRaidLoyaltyShock(
    state,
    targetWorker.factionId,
    targetWorker.x,
    targetWorker.y,
    Math.max(2, Math.round((unitDef.raidLoyaltyDamage ?? 0) * 0.33)),
  );
  unit.command = getRaidRetreatCommand(state, unit, { x: targetWorker.x, y: targetWorker.y });
  const loyaltyText = loyaltyShock?.controlPoint
    ? ` ${loyaltyShock.controlPoint.name} loyalty shaken.`
    : "";
  pushMessage(
    state,
    `${getFactionDisplayName(state, unit.factionId)} ${unitDef.name} scatters ${getFactionDisplayName(state, targetWorker.factionId)} workers from the line.${loyaltyText}`,
    targetWorker.factionId === "player" ? "warn" : unit.factionId === "player" ? "good" : "info",
  );
  return true;
}

function getSupplyWagonRetreatPosition(state, wagon, threatPosition) {
  const linkedCamp = getLinkedSupplyCampForWagon(state, wagon);
  if (linkedCamp) {
    const campDef = getBuildingDef(state.content, linkedCamp.typeId);
    return getBuildingCenter(state, linkedCamp, campDef);
  }
  const fallbackCamp = getSupplyCampBuildings(state, wagon.factionId)
    .map((camp) => {
      const campDef = getBuildingDef(state.content, camp.typeId);
      const center = getBuildingCenter(state, camp, campDef);
      return {
        x: center.x,
        y: center.y,
        distance: distance(wagon.x, wagon.y, center.x, center.y),
      };
    })
    .sort((left, right) => left.distance - right.distance)[0];
  if (fallbackCamp) {
    return { x: fallbackCamp.x, y: fallbackCamp.y };
  }
  const fallbackRetreat = getRaidRetreatCommand(state, wagon, threatPosition ?? { x: wagon.x, y: wagon.y });
  return { x: fallbackRetreat.x, y: fallbackRetreat.y };
}

function getSupplyWagonInterdictionImpactProfile(state, wagon, wagonDef) {
  const supplyRadius = wagonDef.supplyRadius ?? 0;
  const engineCount = getFactionSiegeUnits(
    state,
    wagon.factionId,
    (candidate, candidateDef) => Boolean(candidateDef?.siegeClass) && candidateDef.role === "siege-engine",
  ).filter((engine) => distance(wagon.x, wagon.y, engine.x, engine.y) <= supplyRadius).length;
  const fieldArmyCount = getFieldArmyUnits(state, wagon.factionId).filter((candidate) => {
    if (candidate.id === wagon.id) {
      return false;
    }
    const candidateDef = getUnitDef(state.content, candidate.typeId);
    if (!candidateDef || candidateDef.siegeClass || candidateDef.role === "support") {
      return false;
    }
    return distance(wagon.x, wagon.y, candidate.x, candidate.y) <= supplyRadius;
  }).length;
  return {
    engineCount,
    fieldArmyCount,
  };
}

function executeScoutLogisticsInterdiction(state, unit, unitDef, targetUnit) {
  const targetDef = getUnitDef(state.content, targetUnit.typeId);
  if (!isMovingLogisticsCarrierUnitDef(targetDef)) {
    unit.command = null;
    return false;
  }
  const targetPosition = { x: targetUnit.x, y: targetUnit.y };
  const targetFaction = getFaction(state, targetUnit.factionId);
  if (!targetFaction) {
    unit.command = null;
    return false;
  }
  const interdictionDuration = Math.max(
    10,
    targetDef.raidInterdictionSeconds ?? Math.max(12, (unitDef.raidDurationSeconds ?? 0) - 6),
  );
  targetUnit.logisticsInterdictedUntil = Math.max(
    targetUnit.logisticsInterdictedUntil ?? 0,
    state.meta.elapsed + interdictionDuration,
  );
  targetUnit.convoyRecoveryUntil = Math.max(
    targetUnit.convoyRecoveryUntil ?? 0,
    state.meta.elapsed + interdictionDuration + CONVOY_RECOVERY_DURATION_SECONDS,
  );
  targetUnit.interdictedByFactionId = unit.factionId;
  const retreatPosition = getSupplyWagonRetreatPosition(state, targetUnit, targetPosition);
  targetUnit.command = {
    type: "move",
    x: retreatPosition.x,
    y: retreatPosition.y,
  };
  const resourceLoss = applyResourceLoss(targetFaction.resources, targetDef.raidLoss ?? {
    food: 8,
    water: 8,
    wood: 6,
  });
  const impactProfile = getSupplyWagonInterdictionImpactProfile(state, targetUnit, targetDef);
  const loyaltyShock = applyLocalRaidLoyaltyShock(
    state,
    targetUnit.factionId,
    targetPosition.x,
    targetPosition.y,
    Math.max(3, Math.round((unitDef.raidLoyaltyDamage ?? 0) * 0.5)),
  );
  unit.command = getRaidRetreatCommand(state, unit, targetPosition);
  const convoyImpactText = [
    impactProfile.engineCount > 0 ? `${impactProfile.engineCount} engine${impactProfile.engineCount === 1 ? "" : "s"}` : null,
    impactProfile.fieldArmyCount > 0 ? `${impactProfile.fieldArmyCount} field unit${impactProfile.fieldArmyCount === 1 ? "" : "s"}` : null,
  ].filter(Boolean).join(", ");
  const convoyText = convoyImpactText ? `, disrupting ${convoyImpactText}` : "";
  const loyaltyText = loyaltyShock?.controlPoint
    ? ` ${loyaltyShock.controlPoint.name} loyalty shaken.`
    : "";
  pushMessage(
    state,
    `${getFactionDisplayName(state, unit.factionId)} ${unitDef.name} cuts ${getFactionDisplayName(state, targetUnit.factionId)} ${targetDef.name}, stripping ${formatResourceLossSummary(resourceLoss)}${convoyText}.${loyaltyText}`,
    targetUnit.factionId === "player" ? "warn" : unit.factionId === "player" ? "good" : "info",
  );
  return true;
}

function getFactionSiegeUnits(state, factionId, predicate = null) {
  return getAliveUnits(state, factionId).filter((unit) => {
    const unitDef = getUnitDef(state.content, unit.typeId);
    if (!unitDef?.prototypeEnabled) {
      return false;
    }
    if (predicate) {
      return predicate(unit, unitDef);
    }
    // Default: only siege-engine role units (ram, siege_tower, trebuchet, ballista).
    // Siege-support (mantlet) and support (supply_wagon) are fetched separately.
    return Boolean(unitDef?.siegeClass) && unitDef.role === "siege-engine";
  });
}

function getFactionEngineerUnits(state, factionId) {
  return getAliveUnits(state, factionId).filter((unit) => {
    const unitDef = getUnitDef(state.content, unit.typeId);
    return Boolean(unitDef?.prototypeEnabled) && unitDef.role === "engineer-specialist";
  });
}

function getFactionSupplyWagons(state, factionId) {
  return getAliveUnits(state, factionId).filter((unit) => {
    const unitDef = getUnitDef(state.content, unit.typeId);
    return Boolean(unitDef?.prototypeEnabled) && unitDef.role === "support";
  });
}

function getFactionMantlets(state, factionId) {
  return getAliveUnits(state, factionId).filter((unit) => {
    const unitDef = getUnitDef(state.content, unit.typeId);
    return Boolean(unitDef?.prototypeEnabled) && unitDef.role === "siege-support" && unitDef.siegeClass === "mantlet";
  });
}

function getFactionSiegeState(state, factionId) {
  const siegeEngines = getFactionSiegeUnits(state, factionId);
  const engineers = getFactionEngineerUnits(state, factionId);
  const supplyWagons = getFactionSupplyWagons(state, factionId);
  const activeSupplyWagons = supplyWagons.filter((unit) => !isSupplyWagonInterdicted(state, unit));
  const recoveringSupplyWagons = activeSupplyWagons.filter((unit) => isSupplyWagonRecovering(state, unit));
  const screenedSupplyWagons = activeSupplyWagons.filter((unit) =>
    getSupplyWagonEscortCoverage(state, factionId, unit).screened);
  const escortedSupplyWagons = supplyWagons.filter((unit) =>
    !isSupplyWagonInterdicted(state, unit) && getSupplyWagonEscortCoverage(state, factionId, unit).screened);
  const supplyCamps = getSupplyCampBuildings(state, factionId);
  const elapsed = state.meta.elapsed ?? 0;
  const suppliedEngines = siegeEngines.filter((unit) => (unit.siegeSuppliedUntil ?? 0) > elapsed).length;
  const engineerSupportedEngines = siegeEngines.filter((unit) => (unit.engineerSupportUntil ?? 0) > elapsed).length;
  // Session 82: convoy reconsolidation readiness. Recovering wagons that are
  // screened by escorts count as reconsolidated. Assault readiness now requires
  // all recovering wagons to be screened before the breach can resume, so
  // unescorted recovering convoys hold the assault line back.
  const unscreenedRecoveringCount = Math.max(0, recoveringSupplyWagons.length - screenedSupplyWagons.length);
  const convoyReconsolidated = recoveringSupplyWagons.length === 0 ||
    (recoveringSupplyWagons.length > 0 && unscreenedRecoveringCount === 0);
  return {
    engines: siegeEngines,
    engineers,
    supplyWagons,
    activeSupplyWagons,
    recoveringSupplyWagons,
    screenedSupplyWagons,
    escortedSupplyWagons,
    supplyCamps,
    engineCount: siegeEngines.length,
    engineerCount: engineers.length,
    supplyWagonCount: supplyWagons.length,
    activeSupplyWagonCount: activeSupplyWagons.length,
    escortedSupplyWagonCount: escortedSupplyWagons.length,
    interdictedSupplyWagonCount: Math.max(0, supplyWagons.length - activeSupplyWagons.length),
    convoyRecoveringCount: recoveringSupplyWagons.length,
    convoyScreenedCount: screenedSupplyWagons.length,
    unscreenedRecoveringCount,
    convoyReconsolidated,
    supplyCampCount: supplyCamps.length,
    suppliedEngines,
    engineerSupportedEngines,
    unsuppliedEngines: Math.max(0, siegeEngines.length - suppliedEngines),
    readyForFormalAssault:
      siegeEngines.length > 0 &&
      engineers.length > 0 &&
      activeSupplyWagons.length > 0 &&
      convoyReconsolidated &&
      supplyCamps.length > 0 &&
      suppliedEngines > 0,
  };
}

function getFactionSiegeLogisticsState(faction) {
  faction.siegeLogistics = faction.siegeLogistics ?? {
    lastUnsuppliedMessageAt: -999,
    lastResuppliedMessageAt: -999,
  };
  return faction.siegeLogistics;
}

function ensureFieldWaterState(unit) {
  unit.fieldWaterSuppliedUntil = unit.fieldWaterSuppliedUntil ?? 0;
  unit.lastFieldWaterTransferAt = unit.lastFieldWaterTransferAt ?? -999;
  unit.fieldWaterStrain = unit.fieldWaterStrain ?? 0;
  unit.fieldWaterStatus = unit.fieldWaterStatus ?? "steady";
  unit.fieldWaterCriticalDuration = unit.fieldWaterCriticalDuration ?? 0;
  unit.fieldWaterAttritionActive = unit.fieldWaterAttritionActive ?? false;
  unit.fieldWaterDesertionRisk = unit.fieldWaterDesertionRisk ?? false;
}

function ensureFieldWaterAlertState(faction) {
  faction.fieldWaterAlerts = {
    lastCriticalMessageAt: -999,
    lastAttritionMessageAt: -999,
    lastDesertionMessageAt: -999,
    lastRecoveredMessageAt: -999,
    ...(faction.fieldWaterAlerts ?? {}),
  };
  return faction.fieldWaterAlerts;
}

function isFieldArmyUnit(unitDef) {
  return Boolean(unitDef?.prototypeEnabled) && unitDef.role !== "worker" && unitDef.movementDomain !== "water";
}

function getFieldArmyUnits(state, factionId) {
  return getAliveUnits(state, factionId).filter((unit) => isFieldArmyUnit(getUnitDef(state.content, unit.typeId)));
}

function getFieldWaterDisciplineProfile(state, unit) {
  const commanderAura = getCommanderAuraProfile(state, unit);
  return {
    commanderAura,
    attritionThresholdSeconds: FIELD_WATER_ATTRITION_THRESHOLD_SECONDS +
      (commanderAura ? FIELD_WATER_COMMANDER_DISCIPLINE_BUFFER_SECONDS : 0),
    desertionThresholdSeconds: FIELD_WATER_DESERTION_THRESHOLD_SECONDS +
      (commanderAura ? FIELD_WATER_COMMANDER_DISCIPLINE_BUFFER_SECONDS : 0),
    attritionDamagePerSecond: FIELD_WATER_ATTRITION_DAMAGE_PER_SECOND *
      (commanderAura ? FIELD_WATER_COMMANDER_ATTRITION_MULTIPLIER : 1),
  };
}

function getFieldWaterSourceCount(state, factionId) {
  const buildingSources = getCompletedBuildings(state, factionId).filter((building) =>
    (getBuildingDef(state.content, building.typeId)?.armyWaterSupportRadius ?? 0) > 0 &&
    !isBuildingUnderScoutRaid(state, building)).length;
  const controlPointSources = state.world.controlPoints.filter((controlPoint) => controlPoint.ownerFactionId === factionId).length;
  const settlementSources = (state.world.settlements ?? []).filter((settlement) => settlement.factionId === factionId).length;
  return buildingSources + controlPointSources + settlementSources;
}

function getFactionFieldWaterState(state, factionId) {
  const elapsed = state.meta.elapsed ?? 0;
  const units = getFieldArmyUnits(state, factionId);
  units.forEach((unit) => ensureFieldWaterState(unit));
  const supportedUnits = units.filter((unit) => (unit.fieldWaterSuppliedUntil ?? 0) > elapsed).length;
  const strainedUnits = units.filter((unit) => (unit.fieldWaterStrain ?? 0) >= FIELD_WATER_STRAIN_THRESHOLD).length;
  const criticalUnits = units.filter((unit) => (unit.fieldWaterStrain ?? 0) >= FIELD_WATER_CRITICAL_THRESHOLD).length;
  const attritionUnits = units.filter((unit) => unit.fieldWaterAttritionActive).length;
  const desertionRiskUnits = units.filter((unit) => unit.fieldWaterDesertionRisk).length;
  return {
    units,
    unitCount: units.length,
    supportedUnits,
    strainedUnits,
    criticalUnits,
    attritionUnits,
    desertionRiskUnits,
    sourceCount: getFieldWaterSourceCount(state, factionId),
  };
}

function getFieldWaterOperationalProfile(unit) {
  const strain = unit.fieldWaterStrain ?? 0;
  if (strain >= FIELD_WATER_CRITICAL_THRESHOLD) {
    return {
      speedMultiplier: FIELD_WATER_CRITICAL_SPEED_MULTIPLIER,
      attackMultiplier: FIELD_WATER_CRITICAL_ATTACK_MULTIPLIER,
      band: "critical",
    };
  }
  if (strain >= FIELD_WATER_STRAIN_THRESHOLD) {
    return {
      speedMultiplier: FIELD_WATER_STRAIN_SPEED_MULTIPLIER,
      attackMultiplier: FIELD_WATER_STRAIN_ATTACK_MULTIPLIER,
      band: "strained",
    };
  }
  return {
    speedMultiplier: 1,
    attackMultiplier: 1,
    band: "steady",
  };
}

function getFieldWaterSupportProfile(state, factionId, unit) {
  let durationSeconds = 0;

  const ownedControlPoints = state.world.controlPoints.filter((controlPoint) => controlPoint.ownerFactionId === factionId);
  for (const controlPoint of ownedControlPoints) {
    const position = getControlPointPosition(state, controlPoint);
    const radius = Math.max(FIELD_WATER_LOCAL_SUPPORT_RADIUS, (controlPoint.radiusTiles ?? 3) * state.world.tileSize * 0.7);
    if (distance(unit.x, unit.y, position.x, position.y) <= radius) {
      durationSeconds = Math.max(durationSeconds, FIELD_WATER_SUPPORT_DURATION_SECONDS);
      break;
    }
  }

  const ownedSettlements = (state.world.settlements ?? []).filter((settlement) => settlement.factionId === factionId);
  for (const settlement of ownedSettlements) {
    const position = getSettlementPosition(state, settlement);
    if (distance(unit.x, unit.y, position.x, position.y) <= FIELD_WATER_SETTLEMENT_SUPPORT_RADIUS) {
      durationSeconds = Math.max(durationSeconds, FIELD_WATER_SUPPORT_DURATION_SECONDS);
      break;
    }
  }

  const waterSupportBuildings = getCompletedBuildings(state, factionId).filter((building) =>
    (getBuildingDef(state.content, building.typeId)?.armyWaterSupportRadius ?? 0) > 0 &&
    !isBuildingUnderScoutRaid(state, building));
  for (const building of waterSupportBuildings) {
    const buildingDef = getBuildingDef(state.content, building.typeId);
    const center = getBuildingCenter(state, building, buildingDef);
    if (distance(unit.x, unit.y, center.x, center.y) <= (buildingDef.armyWaterSupportRadius ?? 0)) {
      durationSeconds = Math.max(durationSeconds, buildingDef.armyWaterSupportDurationSeconds ?? FIELD_WATER_SUPPORT_DURATION_SECONDS);
    }
  }

  const supplyWagons = getFactionSupplyWagons(state, factionId);
  const supplyCamps = getSupplyCampBuildings(state, factionId);
  for (const wagon of supplyWagons) {
    const wagonDef = getUnitDef(state.content, wagon.typeId);
    if (isSupplyWagonInterdicted(state, wagon)) {
      continue;
    }
    const linkedCamp = getLinkedSupplyCampForWagon(state, wagon, supplyCamps);
    if (!linkedCamp) {
      continue;
    }
    if (distance(wagon.x, wagon.y, unit.x, unit.y) <= (wagonDef.supplyRadius ?? 0)) {
      durationSeconds = Math.max(durationSeconds, wagonDef.supplyDurationSeconds ?? FIELD_WATER_SUPPORT_DURATION_SECONDS);
    }
  }

  return {
    available: durationSeconds > 0,
    durationSeconds,
  };
}

function refreshFieldWaterSupport(state, faction, unit, durationSeconds) {
  ensureFieldWaterState(unit);
  if ((state.meta.elapsed - (unit.lastFieldWaterTransferAt ?? -999)) >= FIELD_WATER_TRANSFER_INTERVAL_SECONDS) {
    const transferCost = { water: FIELD_WATER_TRANSFER_COST };
    if (!canAffordCost(faction.resources, transferCost)) {
      return false;
    }
    applyCost(faction.resources, transferCost);
    unit.lastFieldWaterTransferAt = state.meta.elapsed;
  }
  unit.fieldWaterSuppliedUntil = Math.max(
    unit.fieldWaterSuppliedUntil ?? 0,
    state.meta.elapsed + Math.max(FIELD_WATER_SUPPORT_DURATION_SECONDS, durationSeconds),
  );
  return true;
}

function getUsedPopulation(state, factionId) {
  return getAliveUnits(state, factionId).reduce((sum, unit) => sum + getUnitDef(state.content, unit.typeId).populationCost, 0);
}

function areHostile(state, factionId, otherFactionId) {
  if (!factionId || !otherFactionId || factionId === otherFactionId) {
    return false;
  }

  const faction = getFaction(state, factionId);
  const otherFaction = getFaction(state, otherFactionId);
  if (!faction || !otherFaction) {
    return false;
  }

  return (faction.hostileTo ?? []).includes(otherFactionId) || (otherFaction.hostileTo ?? []).includes(factionId);
}

function getControlPointPosition(state, controlPoint) {
  return {
    x: controlPoint.x * state.world.tileSize,
    y: controlPoint.y * state.world.tileSize,
  };
}

function getSettlementPosition(state, settlement) {
  return {
    x: settlement.x * state.world.tileSize,
    y: settlement.y * state.world.tileSize,
  };
}

function getSettlementById(state, settlementId) {
  return (state.world.settlements ?? []).find((settlement) => settlement.id === settlementId) ?? null;
}

function isVerdantWardenUnit(unit) {
  return unit?.typeId === "verdant_warden";
}

function getVerdantWardenSupportProfile(count) {
  const safeCount = Math.max(0, Math.floor(count ?? 0));
  const cappedCount = Math.min(VERDANT_WARDEN_MAX_SUPPORT_STACK, safeCount);
  if (cappedCount <= 0) {
    return DEFAULT_VERDANT_WARDEN_SUPPORT;
  }
  return {
    count: safeCount,
    cappedCount,
    defenderAttackMultiplier: 1 + cappedCount * 0.06,
    reserveHealMultiplier: 1 + cappedCount * 0.08,
    reserveMusterMultiplier: 1 + cappedCount * 0.1,
    loyaltyProtectionMultiplier: 1 + cappedCount * 0.18,
    loyaltyGainMultiplier: 1 + cappedCount * 0.14,
    stabilizationMultiplier: 1 + cappedCount * 0.18,
    desiredFrontlineBonus: cappedCount > 0 ? 1 : 0,
  };
}

function getVerdantWardenZoneSupportProfile(state, factionId, x, y, radius = VERDANT_WARDEN_ZONE_RADIUS) {
  if (!factionId || !Number.isFinite(x) || !Number.isFinite(y) || radius <= 0) {
    return DEFAULT_VERDANT_WARDEN_SUPPORT;
  }
  const wardenCount = state.units.reduce((count, unit) => {
    if (
      unit.health <= 0 ||
      unit.factionId !== factionId ||
      !isVerdantWardenUnit(unit) ||
      distance(unit.x, unit.y, x, y) > radius
    ) {
      return count;
    }
    return count + 1;
  }, 0);
  return getVerdantWardenSupportProfile(wardenCount);
}

function getVerdantWardenControlPointSupportProfile(state, controlPoint) {
  if (!controlPoint?.ownerFactionId) {
    return DEFAULT_VERDANT_WARDEN_SUPPORT;
  }
  const position = getControlPointPosition(state, controlPoint);
  const radius = Math.max(
    VERDANT_WARDEN_ZONE_RADIUS * 0.78,
    ((controlPoint.radiusTiles ?? 3) * state.world.tileSize) + 44,
  );
  return getVerdantWardenZoneSupportProfile(state, controlPoint.ownerFactionId, position.x, position.y, radius);
}

function getGovernorSpecializationIdForSettlementClass(settlementClassId) {
  if (["border_settlement", "military_fort"].includes(settlementClassId)) {
    return "border";
  }
  if (["trade_town", "regional_stronghold"].includes(settlementClassId)) {
    return "city";
  }
  if (["primary_dynastic_keep", "fortress_citadel"].includes(settlementClassId)) {
    return "keep";
  }
  return DEFAULT_GOVERNOR_SPECIALIZATION.id;
}

function getGovernorSpecializationProfile(specializationId) {
  return GOVERNOR_SPECIALIZATION_PROFILES[specializationId] ?? DEFAULT_GOVERNOR_SPECIALIZATION;
}

function getGovernorAssignmentAnchor(state, assignment) {
  if (!assignment) {
    return null;
  }
  if (assignment.anchorType === "settlement") {
    return getSettlementById(state, assignment.anchorId);
  }
  return state.world.controlPoints.find((controlPoint) => controlPoint.id === assignment.anchorId) ?? null;
}

function getSacredSitePosition(state, sacredSite) {
  return {
    x: sacredSite.x * state.world.tileSize,
    y: sacredSite.y * state.world.tileSize,
  };
}

function getDynastyMember(faction, memberId) {
  return faction?.dynasty?.members?.find((member) => member.id === memberId) ?? null;
}

function isMemberAvailable(member) {
  if (!member) {
    return false;
  }
  return member.status !== "fallen" &&
    member.status !== "captured" &&
    member.status !== "dead" &&
    member.status !== "displaced";
}

function getDynastyMemberByRole(faction, roleIds) {
  const members = faction?.dynasty?.members ?? [];
  for (const roleId of roleIds) {
    const member = members.find((candidate) => candidate.roleId === roleId && isMemberAvailable(candidate));
    if (member) {
      return member;
    }
  }
  return null;
}

function getGovernanceSeatMembers(faction) {
  const members = faction?.dynasty?.members ?? [];
  return members
    .filter((member) =>
      isMemberAvailable(member) &&
      GOVERNANCE_SEAT_MEMBER_PRIORITY.includes(member.roleId))
    .sort((left, right) =>
      GOVERNANCE_SEAT_MEMBER_PRIORITY.indexOf(left.roleId) - GOVERNANCE_SEAT_MEMBER_PRIORITY.indexOf(right.roleId) ||
      (right.renown ?? 0) - (left.renown ?? 0) ||
      left.id.localeCompare(right.id));
}

function getGovernanceSeatMemberScore(member, specializationId, anchorType) {
  const roleProfile = GOVERNANCE_SEAT_ROLE_PROFILES[member?.roleId] ?? {};
  const specializationScore = roleProfile[specializationId] ?? 0;
  const anchorBias = anchorType === "settlement" ? 0.6 : 0;
  return specializationScore + anchorBias + ((member?.renown ?? 0) * 0.01);
}

function findAvailableSuccessor(faction, excludeIds = new Set()) {
  const members = faction?.dynasty?.members ?? [];
  for (const roleId of SUCCESSION_ROLE_CHAIN) {
    const candidate = members.find((member) =>
      member.roleId === roleId &&
      !excludeIds.has(member.id) &&
      isMemberAvailable(member),
    );
    if (candidate) {
      return candidate;
    }
  }
  return members.find((member) => !excludeIds.has(member.id) && isMemberAvailable(member)) ?? null;
}

function logPromotion(member, fromRoleId, toRoleId, elapsed) {
  member.promotionHistory = member.promotionHistory ?? [];
  member.promotionHistory.unshift({
    fromRoleId,
    toRoleId,
    at: elapsed,
  });
  member.promotionHistory = member.promotionHistory.slice(0, 6);
}

function adjustLegitimacy(faction, delta) {
  if (!faction?.dynasty) {
    return;
  }
  faction.dynasty.legitimacy = Math.max(0, Math.min(100, faction.dynasty.legitimacy + delta));
}

function appendFallenLedger(faction, entry) {
  if (!faction?.dynasty?.attachments) {
    return;
  }
  faction.dynasty.attachments.fallenMembers = [
    entry,
    ...(faction.dynasty.attachments.fallenMembers ?? []),
  ].slice(0, FALLEN_LEDGER_LIMIT);
}

// Session 40: Direct battlefield-hero renown award hook.
// Canonical: glory is earned in the field, not granted by edit. The active
// commander (or, fallback, the marshal, head of bloodline, or anyone on the
// military_command path) accrues renown when their faction wins a battlefield
// event. Worker kills do NOT count — canonical "glory does not come from
// killing peasants". Renown is capped at 100 to keep the lesser-house
// pipeline (S35) from being trivially bypassed by farming, and to leave
// headroom for design balance later.
export const RENOWN_CAP = 100;
export const RENOWN_AWARD_COMBAT_KILL = 1;
export const RENOWN_AWARD_FORTIFICATION_KILL = 2;

function findRenownAwardRecipient(faction) {
  if (!faction?.dynasty?.members) return null;
  // Order of preference: active commander > marshal/captain > any
  // military_command path member > head of bloodline. Mirrors how the realm
  // would actually credit the win.
  const members = faction.dynasty.members;
  const byRolePriority = ["commander", "head_of_bloodline"];
  for (const roleId of byRolePriority) {
    const m = members.find((mem) => mem.roleId === roleId && isMemberAvailable(mem));
    if (m) return m;
  }
  // Fallback: any military_command path member.
  const milMember = members.find((mem) => mem.pathId === "military_command" && isMemberAvailable(mem));
  if (milMember) return milMember;
  // Last resort: any available member.
  return members.find(isMemberAvailable) ?? null;
}

function awardRenownToFaction(state, factionId, amount, reason) {
  const faction = getFaction(state, factionId);
  if (!faction) return null;
  const recipient = findRenownAwardRecipient(faction);
  if (!recipient) return null;
  const previous = recipient.renown ?? 0;
  recipient.renown = Math.min(RENOWN_CAP, previous + amount);
  if (recipient.renown === previous) return null;
  // Renown ledger is a lightweight per-member append-only trace so the UI and
  // tests can verify award provenance without scanning the global message log.
  recipient.renownLedger = recipient.renownLedger ?? [];
  recipient.renownLedger.unshift({
    amount: recipient.renown - previous,
    reason,
    atInWorldDays: state.dualClock?.inWorldDays ?? 0,
  });
  recipient.renownLedger = recipient.renownLedger.slice(0, 12);
  return recipient;
}

function getDynastyOperationsState(faction) {
  if (!faction?.dynasty) {
    return null;
  }
  faction.dynasty.operations = faction.dynasty.operations ?? { active: [], history: [] };
  faction.dynasty.operations.active = faction.dynasty.operations.active ?? [];
  faction.dynasty.operations.history = faction.dynasty.operations.history ?? [];
  return faction.dynasty.operations;
}

function getDynastyIntelligenceReports(faction) {
  if (!faction?.dynasty) {
    return [];
  }
  faction.dynasty.intelligenceReports = faction.dynasty.intelligenceReports ?? [];
  return faction.dynasty.intelligenceReports;
}

function getDynastyCounterIntelligenceState(faction) {
  if (!faction?.dynasty) {
    return [];
  }
  faction.dynasty.counterIntelligence = faction.dynasty.counterIntelligence ?? [];
  return faction.dynasty.counterIntelligence;
}

function getDynastyPoliticalEventState(faction) {
  if (!faction?.dynasty) {
    return null;
  }
  faction.dynasty.politicalEvents = faction.dynasty.politicalEvents ?? { active: [], history: [] };
  faction.dynasty.politicalEvents.active = faction.dynasty.politicalEvents.active ?? [];
  faction.dynasty.politicalEvents.history = faction.dynasty.politicalEvents.history ?? [];
  return faction.dynasty.politicalEvents;
}

function getActiveDynastyPoliticalEvent(faction, type) {
  return getDynastyPoliticalEventState(faction)?.active?.find((entry) => entry.type === type) ?? null;
}

function pushDynastyPoliticalEventHistory(faction, entry) {
  const politicalEvents = getDynastyPoliticalEventState(faction);
  if (!politicalEvents) {
    return;
  }
  politicalEvents.history = [entry, ...politicalEvents.history].slice(0, POLITICAL_EVENT_HISTORY_LIMIT);
}

function getSuccessionCrisisSeverityProfile(severityId) {
  return SUCCESSION_CRISIS_SEVERITY_PROFILES[severityId] ?? SUCCESSION_CRISIS_SEVERITY_PROFILES.minor;
}

function getSuccessorRoleClaimWeight(roleId) {
  switch (roleId) {
    case "head_of_bloodline":
      return 12;
    case "heir_designate":
      return 10;
    case "commander":
      return 8;
    case "governor":
      return 7;
    case "diplomat":
      return 6;
    case "ideological_leader":
      return 5;
    case "merchant":
      return 4;
    case "spymaster":
      return 4;
    default:
      return 3;
  }
}

function getSuccessionClaimStrength(member, { ruling = false } = {}) {
  if (!member || !isMemberAvailable(member)) {
    return -Infinity;
  }
  const maturityScore = member.age >= SUCCESSION_CRISIS_MATURE_AGE
    ? 5
    : member.age >= SUCCESSION_CRISIS_ADULT_AGE
      ? 3
      : 0;
  return getSuccessorRoleClaimWeight(member.roleId) + maturityScore + Math.round((member.renown ?? 0) / 3) + (ruling ? 3 : 0);
}

function getMoreSevereSuccessionCrisisId(leftId, rightId) {
  const leftIndex = SUCCESSION_CRISIS_SEVERITY_ORDER.indexOf(leftId);
  const rightIndex = SUCCESSION_CRISIS_SEVERITY_ORDER.indexOf(rightId);
  if (leftIndex === -1) {
    return rightId;
  }
  if (rightIndex === -1) {
    return leftId;
  }
  return rightIndex > leftIndex ? rightId : leftId;
}

function buildSuccessionCrisisTriggerProfile(state, factionId, fallenMember) {
  const faction = getFaction(state, factionId);
  if (!faction?.dynasty) {
    return {
      shouldTrigger: false,
      severityId: null,
      currentRuler: null,
      claimantProfiles: [],
      triggerReasons: [],
      interregnum: false,
      youngRuler: false,
      underageRuler: false,
    };
  }

  const currentRuler = getDynastyMemberByRole(faction, ["head_of_bloodline"]);
  const interregnum = Boolean(faction.dynasty.interregnum) || !currentRuler;
  const youngRuler = Boolean(currentRuler) && (currentRuler.age ?? 0) < SUCCESSION_CRISIS_MATURE_AGE;
  const underageRuler = Boolean(currentRuler) && (currentRuler.age ?? 0) < SUCCESSION_CRISIS_ADULT_AGE;
  const rulerClaimStrength = getSuccessionClaimStrength(currentRuler, { ruling: true });
  const claimantProfiles = (faction.dynasty.members ?? [])
    .filter((member) =>
      member.id !== fallenMember?.id &&
      member.id !== currentRuler?.id &&
      isMemberAvailable(member) &&
      (member.age ?? 0) >= SUCCESSION_CRISIS_ADULT_AGE)
    .map((member) => ({
      id: member.id,
      title: member.title,
      roleId: member.roleId,
      age: member.age,
      renown: member.renown ?? 0,
      claimStrength: getSuccessionClaimStrength(member),
    }))
    .filter((member) =>
      !currentRuler ||
      member.claimStrength >= (rulerClaimStrength - SUCCESSION_CRISIS_CLAIM_GAP_THRESHOLD))
    .sort((left, right) =>
      (right.claimStrength - left.claimStrength) ||
      ((right.renown ?? 0) - (left.renown ?? 0)));

  const triggerReasons = [];
  let severityId = null;

  if (interregnum) {
    severityId = "catastrophic";
    triggerReasons.push("No ruling claimant sits the dynastic seat.");
  } else {
    if (underageRuler) {
      severityId = getMoreSevereSuccessionCrisisId(severityId, "major");
      triggerReasons.push(`${currentRuler.title} is below the adult succession threshold.`);
    } else if (youngRuler) {
      severityId = getMoreSevereSuccessionCrisisId(severityId, "moderate");
      triggerReasons.push(`${currentRuler.title} ascends before reaching full dynastic maturity.`);
    }

    if (claimantProfiles.length >= 2) {
      severityId = getMoreSevereSuccessionCrisisId(severityId, "major");
      triggerReasons.push(`${claimantProfiles.length} rival claimants remain close enough to contest the seat.`);
    } else if (claimantProfiles.length === 1) {
      severityId = getMoreSevereSuccessionCrisisId(severityId, "minor");
      triggerReasons.push(`${claimantProfiles[0].title} holds a viable rival claim.`);
    }
  }

  return {
    shouldTrigger: Boolean(severityId),
    severityId,
    currentRuler,
    claimantProfiles,
    triggerReasons,
    interregnum,
    youngRuler,
    underageRuler,
  };
}

function serializeSuccessionCrisisEvent(state, factionId, event) {
  if (!event) {
    return null;
  }
  const faction = getFaction(state, factionId);
  const severity = getSuccessionCrisisSeverityProfile(event.severityId);
  const currentInWorldDays = state.dualClock?.inWorldDays ?? 0;
  const nextEscalationAtInWorldDays = Math.floor(event.lastEscalatedAtInWorldDays ?? event.startedAtInWorldDays ?? currentInWorldDays)
    + SUCCESSION_CRISIS_ESCALATION_IN_WORLD_DAYS;
  return {
    ...event,
    severityLabel: severity.label,
    resourceTrickleMultiplier: severity.resourceTrickleMultiplier,
    populationGrowthMultiplier: severity.populationGrowthMultiplier,
    stabilizationMultiplier: severity.stabilizationMultiplier,
    attackMultiplier: severity.attackMultiplier,
    legitimacyDrainPerDay: severity.legitimacyDrainPerDay,
    loyaltyDrainPerDay: severity.loyaltyDrainPerDay,
    currentRuler: event.currentRulerId ? getDynastyMember(faction, event.currentRulerId) : null,
    fallenMember: event.fallenMemberId ? getDynastyMember(faction, event.fallenMemberId) : null,
    claimantMembers: (event.claimantIds ?? [])
      .map((memberId) => getDynastyMember(faction, memberId))
      .filter(Boolean),
    claimantCount: event.claimantIds?.length ?? 0,
    daysActive: Math.max(0, (state.dualClock?.inWorldDays ?? 0) - (event.startedAtInWorldDays ?? currentInWorldDays)),
    nextEscalationInWorldDays: Math.max(0, nextEscalationAtInWorldDays - currentInWorldDays),
  };
}

function getActiveSuccessionCrisis(faction) {
  return getActiveDynastyPoliticalEvent(faction, "succession_crisis");
}

function getActiveCovenantTest(faction) {
  return getActiveDynastyPoliticalEvent(faction, "covenant_test");
}

function pickCovenantTestTargetFactionId(state, factionId) {
  const faction = getFaction(state, factionId);
  return Object.values(state.factions)
    .filter((candidate) => candidate?.kind === "kingdom" && candidate.id !== factionId)
    .sort((left, right) =>
      (Number((faction?.hostileTo ?? []).includes(right.id)) - Number((faction?.hostileTo ?? []).includes(left.id))) ||
      ((right.population?.total ?? 0) - (left.population?.total ?? 0)) ||
      ((right.resources?.gold ?? 0) - (left.resources?.gold ?? 0)))[0]?.id ?? null;
}

function buildCovenantTestEvent(state, factionId) {
  const faction = getFaction(state, factionId);
  const faithState = ensureFaithCovenantTestCompletionFromLegacyState(state, factionId);
  if (!faction?.dynasty || !faithState?.selectedFaithId) {
    return null;
  }
  const primarySeat = getPrimaryKeepSeat(state, factionId);
  const primarySeatPosition = primarySeat ? getSettlementPosition(state, primarySeat) : null;
  const selectedFaith = getFaith(state.content, faithState.selectedFaithId);
  const doctrinePath = faithState.doctrinePath ?? "light";
  const targetFactionId = pickCovenantTestTargetFactionId(state, factionId);
  const targetFactionName = targetFactionId ? getFactionDisplayName(state, targetFactionId) : null;
  const targetControlPoint = getClosestExternalControlPointToPosition(state, factionId, primarySeatPosition);
  const wildSacredSite = getFaithSacredSite(state, faithState.selectedFaithId);
  const wildSacredSitePosition = wildSacredSite ? getSacredSitePosition(state, wildSacredSite) : null;
  const wildAnchor = wildSacredSitePosition ? getClosestOwnedControlPointToPosition(state, factionId, wildSacredSitePosition) : null;
  const weakestOwnedTerritory = state.world.controlPoints
    .filter((controlPoint) => controlPoint.ownerFactionId === factionId)
    .sort((left, right) => (left.loyalty ?? 100) - (right.loyalty ?? 100))[0] ?? null;
  const resolveByInWorldDays = (state.dualClock?.inWorldDays ?? 0) + COVENANT_TEST_DURATION_IN_WORLD_DAYS;
  const resolveAt = state.meta.elapsed + (
    COVENANT_TEST_DURATION_IN_WORLD_DAYS /
    (state.dualClock?.daysPerRealSecond ?? DUAL_CLOCK_DEFAULT_DAYS_PER_REAL_SECOND)
  );

  let mandateId = `${faithState.selectedFaithId}_${doctrinePath}_covenant_test`;
  let mandateLabel = `${selectedFaith?.name ?? faithState.selectedFaithId} Covenant Test`;
  let summary = `Demonstrate full commitment to ${selectedFaith?.covenantName ?? faithState.selectedFaithId}.`;
  let objectiveText = summary;
  let actionId = null;
  let actionLabel = null;
  let targetControlPointId = targetControlPoint?.id ?? null;
  let targetControlPointName = targetControlPoint?.name ?? null;
  let targetSacredSiteId = null;
  let targetSacredSiteName = null;
  let requiredLoyalty = null;

  switch (`${faithState.selectedFaithId}:${doctrinePath}`) {
    case "old_light:light":
      mandateId = "old_light_light_memorial";
      mandateLabel = "Act of Memorial";
      requiredLoyalty = Math.max(72, Math.round((weakestOwnedTerritory?.loyalty ?? 64) + 8));
      targetControlPointId = weakestOwnedTerritory?.id ?? null;
      targetControlPointName = weakestOwnedTerritory?.name ?? "a held march";
      summary = "The Old Light demands living memory in a stabilized march.";
      objectiveText = `Stabilize ${targetControlPointName} to ${requiredLoyalty}+ loyalty under living covenant order.`;
      break;
    case "old_light:dark":
      mandateId = "old_light_dark_purge";
      mandateLabel = "Purge Mandate";
      summary = "The Old Light demands a declared purge against a rival court.";
      objectiveText = targetFactionName
        ? `Declare holy war on ${targetFactionName} and drive their weakest loyalty below 40.`
        : "Declare holy war on a rival court and break their weakest march below 40 loyalty.";
      break;
    case "blood_dominion:light":
      mandateId = "blood_dominion_light_ceremony";
      mandateLabel = "Shared Covenant Ceremony";
      summary = "The Red Covenant demands a witnessed renewal across the army.";
      objectiveText = "Field a committed host, stock 45 food and 18 influence, then conduct the covenant rite.";
      actionId = "conduct_shared_covenant";
      actionLabel = "Conduct Covenant Rite";
      break;
    case "blood_dominion:dark":
      mandateId = "blood_dominion_dark_binding";
      mandateLabel = "Binding of Cost";
      summary = "The Red Covenant demands a binding paid in living strength.";
      objectiveText = `Offer a binding sacrifice worth ${COVENANT_TEST_BLOOD_DOMINION_DARK_POPULATION_COST} population, ${COVENANT_TEST_BLOOD_DOMINION_DARK_BINDING_COST.influence} influence, and ${COVENANT_TEST_BLOOD_DOMINION_DARK_LEGITIMACY_COST} legitimacy.`;
      actionId = "offer_binding_sacrifice";
      actionLabel = "Offer Binding Sacrifice";
      break;
    case "the_order:light":
      mandateId = "the_order_light_codification";
      mandateLabel = "Mandate Codification";
      summary = "The Order demands codified governance across every held march.";
      objectiveText = "Stabilize every held territory, keep court loyalty at 68+, and seat governors across the realm.";
      break;
    case "the_order:dark":
      mandateId = "the_order_dark_expansion";
      mandateLabel = "Dominion Expansion";
      requiredLoyalty = 55;
      summary = "The Order demands a new march be folded into Dominion rule.";
      objectiveText = targetControlPointName
        ? `Take ${targetControlPointName} and hold it at ${requiredLoyalty}+ loyalty under enforcement.`
        : `Take a rival march and hold it at ${requiredLoyalty}+ loyalty under enforcement.`;
      break;
    case "the_wild:light":
      mandateId = "the_wild_light_grove_renewal";
      mandateLabel = "Grove Renewal";
      requiredLoyalty = Math.max(66, Math.round((wildAnchor?.loyalty ?? 60) + 6));
      targetSacredSiteId = wildSacredSite?.id ?? null;
      targetSacredSiteName = wildSacredSite?.name ?? "the covenant grove";
      targetControlPointId = wildAnchor?.id ?? null;
      targetControlPointName = wildAnchor?.name ?? "the nearest march";
      summary = "The Wild demands renewal around a living sacred grove.";
      objectiveText = `Project faith structure coverage over ${targetSacredSiteName} and hold ${targetControlPointName} at ${requiredLoyalty}+ loyalty.`;
      break;
    case "the_wild:dark":
      mandateId = "the_wild_dark_predator_hunt";
      requiredLoyalty = 45;
      mandateLabel = "Predator Hunt";
      summary = "The Wild demands a predator force descend on a rival frontier.";
      objectiveText = targetControlPointName
        ? `Drive a Predator Ascendant onto ${targetControlPointName} and collapse the march below ${requiredLoyalty} loyalty or seize it outright.`
        : `Drive a Predator Ascendant into a rival march and collapse it below ${requiredLoyalty} loyalty or seize it outright.`;
      break;
    default:
      break;
  }

  return {
    id: createEntityId(state, "dynastyEvent"),
    type: "covenant_test",
    faithId: faithState.selectedFaithId,
    faithName: selectedFaith?.name ?? faithState.selectedFaithId,
    covenantName: selectedFaith?.covenantName ?? selectedFaith?.name ?? faithState.selectedFaithId,
    doctrinePath,
    mandateId,
    mandateLabel,
    summary,
    objectiveText,
    actionId,
    actionLabel,
    targetFactionId,
    targetFactionName,
    targetControlPointId,
    targetControlPointName,
    targetSacredSiteId,
    targetSacredSiteName,
    requiredLoyalty,
    startedAt: state.meta.elapsed,
    startedAtInWorldDays: state.dualClock?.inWorldDays ?? 0,
    resolveByInWorldDays,
    resolveAt,
    lastReminderAtInWorldDays: state.dualClock?.inWorldDays ?? 0,
  };
}

function buildCovenantTestEvaluation(conditions, shortfalls, extra = {}) {
  const total = Math.max(1, conditions.length);
  const satisfied = conditions.filter(Boolean).length;
  return {
    completed: satisfied >= total,
    progressPct: Math.round((satisfied / total) * 100),
    shortfalls,
    ...extra,
  };
}

function evaluateCovenantTestProgress(state, factionId, event) {
  const faction = getFaction(state, factionId);
  if (!faction?.dynasty || !event) {
    return buildCovenantTestEvaluation([false], ["Covenant test no longer has a living court."]);
  }
  const ownedTerritories = state.world.controlPoints.filter((controlPoint) => controlPoint.ownerFactionId === factionId);
  const averageLoyalty = ownedTerritories.length > 0
    ? ownedTerritories.reduce((sum, controlPoint) => sum + (controlPoint.loyalty ?? 50), 0) / ownedTerritories.length
    : 0;
  const governorAssignments = faction.dynasty.attachments?.governorAssignments ?? [];
  const stageThreeAndFourFaithUnits = getFactionFaithUnitRoster(state, factionId, {
    faithId: event.faithId,
    doctrinePath: event.doctrinePath,
    stages: [3, 4],
  });
  const stageFourFaithUnits = getFactionFaithUnitRoster(state, factionId, {
    faithId: event.faithId,
    doctrinePath: event.doctrinePath,
    stages: [4],
  });
  const combatUnitCount = state.units.filter((unit) =>
    unit.factionId === factionId &&
    unit.health > 0 &&
    !["worker", "engineer-specialist", "support"].includes(getUnitDef(state.content, unit.typeId)?.role)).length;
  const hasHallOrSanctuary = hasCompletedFaithBuildingOfType(state, factionId, "covenant_hall") ||
    hasCompletedFaithBuildingOfType(state, factionId, "grand_sanctuary");
  const hasSanctuary = hasCompletedFaithBuildingOfType(state, factionId, "grand_sanctuary");
  const targetFaction = event.targetFactionId ? getFaction(state, event.targetFactionId) : null;
  const targetControlPoint = event.targetControlPointId
    ? state.world.controlPoints.find((controlPoint) => controlPoint.id === event.targetControlPointId) ?? null
    : null;
  const targetSacredSite = event.targetSacredSiteId
    ? (state.world.sacredSites ?? []).find((sacredSite) => sacredSite.id === event.targetSacredSiteId) ?? null
    : null;
  const targetSacredSitePosition = targetSacredSite ? getSacredSitePosition(state, targetSacredSite) : null;
  const targetHolyWarActive = event.targetFactionId
    ? Boolean(getActiveHolyWar(faction, event.targetFactionId, state.meta.elapsed))
    : false;
  const targetWeakestLoyalty = event.targetFactionId
    ? (getDynastyCourtLoyaltyProfile(state, event.targetFactionId).weakestLoyalty ?? 100)
    : 100;
  const targetGroveCoverage = targetSacredSitePosition
    ? getFaithBuildingCoverageAt(state, factionId, targetSacredSitePosition, ["shrine", "hall", "sanctuary"], 40)
    : [];

  switch (event.mandateId) {
    case "old_light_light_memorial": {
      const shortfalls = [];
      if (!hasSanctuary) shortfalls.push("Raise a living Grand Sanctuary.");
      if (!targetControlPoint || targetControlPoint.ownerFactionId !== factionId) shortfalls.push(`Hold ${event.targetControlPointName ?? "the designated march"}.`);
      if (targetControlPoint && targetControlPoint.controlState !== "stabilized") shortfalls.push(`${event.targetControlPointName ?? "The march"} must be stabilized.`);
      if (targetControlPoint && (targetControlPoint.loyalty ?? 0) < (event.requiredLoyalty ?? 72)) shortfalls.push(`Lift ${event.targetControlPointName ?? "the march"} to ${event.requiredLoyalty ?? 72}+ loyalty.`);
      return buildCovenantTestEvaluation([
        hasSanctuary,
        Boolean(targetControlPoint && targetControlPoint.ownerFactionId === factionId),
        Boolean(targetControlPoint && targetControlPoint.controlState === "stabilized"),
        Boolean(targetControlPoint && (targetControlPoint.loyalty ?? 0) >= (event.requiredLoyalty ?? 72)),
      ], shortfalls, {
        statusText: targetControlPoint
          ? `${targetControlPoint.name} at ${Math.round(targetControlPoint.loyalty ?? 0)} loyalty, ${targetControlPoint.controlState}.`
          : "No designated memorial march remains available.",
      });
    }
    case "old_light_dark_purge": {
      const shortfalls = [];
      if (!hasSanctuary) shortfalls.push("Raise a living Grand Sanctuary.");
      if (!targetHolyWarActive) shortfalls.push(`Declare holy war on ${event.targetFactionName ?? "the designated rival"}.`);
      if (targetWeakestLoyalty >= 40) shortfalls.push(`Break ${event.targetFactionName ?? "the rival court"} below 40 weakest loyalty.`);
      if (stageFourFaithUnits.length === 0) shortfalls.push("Field a Stage 4 covenant vanguard.");
      return buildCovenantTestEvaluation([
        hasSanctuary,
        targetHolyWarActive,
        targetWeakestLoyalty < 40,
        stageFourFaithUnits.length > 0,
      ], shortfalls, {
        statusText: `${event.targetFactionName ?? "Rival"} weakest loyalty ${Math.round(targetWeakestLoyalty)}${targetHolyWarActive ? " under holy war" : ""}.`,
      });
    }
    case "blood_dominion_light_ceremony": {
      const shortfalls = [];
      if (!hasHallOrSanctuary) shortfalls.push("Raise a Covenant Hall or Grand Sanctuary.");
      if (combatUnitCount < 6) shortfalls.push("Field at least 6 combat units.");
      if (stageThreeAndFourFaithUnits.length < 2) shortfalls.push("Keep at least 2 covenant faith units alive.");
      if ((faction.resources.food ?? 0) < COVENANT_TEST_BLOOD_DOMINION_LIGHT_RITE_COST.food) shortfalls.push(`Store ${COVENANT_TEST_BLOOD_DOMINION_LIGHT_RITE_COST.food} food for the rite.`);
      if ((faction.resources.influence ?? 0) < COVENANT_TEST_BLOOD_DOMINION_LIGHT_RITE_COST.influence) shortfalls.push(`Store ${COVENANT_TEST_BLOOD_DOMINION_LIGHT_RITE_COST.influence} influence for the rite.`);
      const actionReady = shortfalls.length === 0 && !event.ritualCompletedAt;
      return buildCovenantTestEvaluation([
        hasHallOrSanctuary,
        combatUnitCount >= 6,
        stageThreeAndFourFaithUnits.length >= 2,
        (faction.resources.food ?? 0) >= COVENANT_TEST_BLOOD_DOMINION_LIGHT_RITE_COST.food,
        (faction.resources.influence ?? 0) >= COVENANT_TEST_BLOOD_DOMINION_LIGHT_RITE_COST.influence,
        Boolean(event.ritualCompletedAt),
      ], shortfalls, {
        actionAvailable: actionReady,
        actionLabel: event.actionLabel,
        actionDetail: `Spend ${COVENANT_TEST_BLOOD_DOMINION_LIGHT_RITE_COST.food} food and ${COVENANT_TEST_BLOOD_DOMINION_LIGHT_RITE_COST.influence} influence to bind the host.`,
        statusText: event.ritualCompletedAt
          ? "The shared covenant ceremony has been witnessed."
          : `${combatUnitCount} combat units, ${stageThreeAndFourFaithUnits.length} covenant units, ${Math.floor(faction.resources.food ?? 0)} food in store.`,
      });
    }
    case "blood_dominion_dark_binding": {
      const shortfalls = [];
      if (!hasSanctuary) shortfalls.push("Raise a living Grand Sanctuary.");
      if ((faction.population?.total ?? 0) < COVENANT_TEST_BLOOD_DOMINION_DARK_POPULATION_COST + 1) shortfalls.push(`Keep more than ${COVENANT_TEST_BLOOD_DOMINION_DARK_POPULATION_COST} population alive to pay the binding.`);
      if ((faction.resources.influence ?? 0) < COVENANT_TEST_BLOOD_DOMINION_DARK_BINDING_COST.influence) shortfalls.push(`Store ${COVENANT_TEST_BLOOD_DOMINION_DARK_BINDING_COST.influence} influence for the binding.`);
      const actionReady = shortfalls.length === 0 && !event.bindingCompletedAt;
      return buildCovenantTestEvaluation([
        hasSanctuary,
        (faction.population?.total ?? 0) >= COVENANT_TEST_BLOOD_DOMINION_DARK_POPULATION_COST + 1,
        (faction.resources.influence ?? 0) >= COVENANT_TEST_BLOOD_DOMINION_DARK_BINDING_COST.influence,
        Boolean(event.bindingCompletedAt),
      ], shortfalls, {
        actionAvailable: actionReady,
        actionLabel: event.actionLabel,
        actionDetail: `Spend ${COVENANT_TEST_BLOOD_DOMINION_DARK_POPULATION_COST} population, ${COVENANT_TEST_BLOOD_DOMINION_DARK_BINDING_COST.influence} influence, and ${COVENANT_TEST_BLOOD_DOMINION_DARK_LEGITIMACY_COST} legitimacy.`,
        statusText: event.bindingCompletedAt
          ? "The binding has been paid in living cost."
          : `${Math.floor(faction.population?.total ?? 0)} population, ${Math.floor(faction.resources.influence ?? 0)} influence, ${Math.round(faction.dynasty?.legitimacy ?? 0)} legitimacy.`,
      });
    }
    case "the_order_light_codification": {
      const allStabilized = ownedTerritories.length > 0 && ownedTerritories.every((controlPoint) => controlPoint.controlState === "stabilized");
      const requiredGovernors = Math.min(2, ownedTerritories.length);
      const shortfalls = [];
      if (ownedTerritories.length < 2) shortfalls.push("Hold at least 2 territories.");
      if (!allStabilized) shortfalls.push("Every held march must be stabilized.");
      if (averageLoyalty < 68) shortfalls.push("Raise average realm loyalty to 68+.");
      if (governorAssignments.length < requiredGovernors) shortfalls.push("Seat governors across the codified realm.");
      return buildCovenantTestEvaluation([
        ownedTerritories.length >= 2,
        allStabilized,
        averageLoyalty >= 68,
        governorAssignments.length >= requiredGovernors,
      ], shortfalls, {
        statusText: `${ownedTerritories.length} territories, ${Math.round(averageLoyalty)} average loyalty, ${governorAssignments.length} governors seated.`,
      });
    }
    case "the_order_dark_expansion": {
      const shortfalls = [];
      if (!targetControlPoint || targetControlPoint.ownerFactionId !== factionId) shortfalls.push(`Take ${event.targetControlPointName ?? "the designated march"}.`);
      if (targetControlPoint && targetControlPoint.controlState !== "stabilized") shortfalls.push(`${event.targetControlPointName ?? "The march"} must be stabilized under Dominion.`);
      if (targetControlPoint && (targetControlPoint.loyalty ?? 0) < (event.requiredLoyalty ?? 55)) shortfalls.push(`Hold ${event.targetControlPointName ?? "the march"} at ${event.requiredLoyalty ?? 55}+ loyalty.`);
      if (stageFourFaithUnits.length === 0) shortfalls.push("Field a Dominion enforcement cadre.");
      return buildCovenantTestEvaluation([
        Boolean(targetControlPoint && targetControlPoint.ownerFactionId === factionId),
        Boolean(targetControlPoint && targetControlPoint.controlState === "stabilized"),
        Boolean(targetControlPoint && (targetControlPoint.loyalty ?? 0) >= (event.requiredLoyalty ?? 55)),
        stageFourFaithUnits.length > 0,
      ], shortfalls, {
        statusText: targetControlPoint
          ? `${targetControlPoint.name} at ${Math.round(targetControlPoint.loyalty ?? 0)} loyalty, ${targetControlPoint.controlState}.`
          : "No designated Dominion march remains available.",
      });
    }
    case "the_wild_light_grove_renewal": {
      const anchor = targetSacredSitePosition
        ? getClosestOwnedControlPointToPosition(state, factionId, targetSacredSitePosition)
        : null;
      const shortfalls = [];
      if (targetGroveCoverage.length === 0) shortfalls.push(`Project covenant structures over ${event.targetSacredSiteName ?? "the sacred grove"}.`);
      if (!anchor || anchor.id !== event.targetControlPointId) shortfalls.push(`Hold ${event.targetControlPointName ?? "the grove march"}.`);
      if (anchor && anchor.controlState !== "stabilized") shortfalls.push(`${event.targetControlPointName ?? "The grove march"} must be stabilized.`);
      if (anchor && (anchor.loyalty ?? 0) < (event.requiredLoyalty ?? 66)) shortfalls.push(`Lift ${event.targetControlPointName ?? "the grove march"} to ${event.requiredLoyalty ?? 66}+ loyalty.`);
      return buildCovenantTestEvaluation([
        targetGroveCoverage.length > 0,
        Boolean(anchor && anchor.id === event.targetControlPointId),
        Boolean(anchor && anchor.controlState === "stabilized"),
        Boolean(anchor && (anchor.loyalty ?? 0) >= (event.requiredLoyalty ?? 66)),
      ], shortfalls, {
        statusText: anchor
          ? `${event.targetSacredSiteName ?? "Sacred grove"} anchored by ${anchor.name} at ${Math.round(anchor.loyalty ?? 0)} loyalty.`
          : "The sacred grove is not yet anchored by the dynasty.",
      });
    }
    case "the_wild_dark_predator_hunt": {
      const shortfalls = [];
      if (stageFourFaithUnits.length === 0) shortfalls.push("Field a Predator Ascendant.");
      if (!targetControlPoint) shortfalls.push("No rival frontier remains marked for the hunt.");
      if (targetControlPoint && targetControlPoint.ownerFactionId !== factionId && (targetControlPoint.loyalty ?? 100) >= (event.requiredLoyalty ?? 45)) shortfalls.push(`Drive ${event.targetControlPointName ?? "the marked frontier"} below ${event.requiredLoyalty ?? 45} loyalty or seize it.`);
      const predatorAtHunt = stageFourFaithUnits.some((unit) => {
        if (!targetControlPoint) {
          return false;
        }
        const position = getControlPointPosition(state, targetControlPoint);
        const radius = ((targetControlPoint.radiusTiles ?? 3) * state.world.tileSize) + 30;
        return distance(unit.x, unit.y, position.x, position.y) <= radius;
      });
      if (!predatorAtHunt) shortfalls.push(`Drive a Predator Ascendant into ${event.targetControlPointName ?? "the marked frontier"}.`);
      return buildCovenantTestEvaluation([
        stageFourFaithUnits.length > 0,
        Boolean(targetControlPoint && (
          targetControlPoint.ownerFactionId === factionId ||
          (targetControlPoint.loyalty ?? 100) < (event.requiredLoyalty ?? 45)
        )),
        predatorAtHunt,
      ], shortfalls, {
        statusText: targetControlPoint
          ? `${targetControlPoint.name} at ${Math.round(targetControlPoint.loyalty ?? 0)} loyalty${predatorAtHunt ? " with predator force engaged" : ""}.`
          : "No hunt frontier remains marked.",
      });
    }
    default:
      return buildCovenantTestEvaluation([false], ["No live covenant test evaluator exists for this doctrine."], {
        statusText: "Doctrine mandate not yet resolved in simulation.",
      });
  }
}

function serializeCovenantTestEvent(state, factionId, event) {
  if (!event) {
    return null;
  }
  const evaluation = evaluateCovenantTestProgress(state, factionId, event);
  return {
    ...event,
    remainingInWorldDays: Math.max(0, Math.round(((event.resolveByInWorldDays ?? 0) - (state.dualClock?.inWorldDays ?? 0)) * 10) / 10),
    remainingSeconds: Math.max(0, Math.round(((event.resolveAt ?? 0) - state.meta.elapsed) * 10) / 10),
    progressPct: evaluation.progressPct,
    completed: evaluation.completed,
    shortfalls: [...(evaluation.shortfalls ?? [])],
    statusText: evaluation.statusText ?? event.objectiveText,
    actionAvailable: evaluation.actionAvailable ?? false,
    actionLabel: evaluation.actionLabel ?? event.actionLabel ?? null,
    actionDetail: evaluation.actionDetail ?? null,
  };
}

function shouldIssueCovenantTest(state, factionId) {
  const faction = getFaction(state, factionId);
  const faithState = ensureFaithCovenantTestCompletionFromLegacyState(state, factionId);
  if (!faction?.dynasty || !faithState?.selectedFaithId) {
    return false;
  }
  if (getActiveCovenantTest(faction) || faithState.covenantTestPassed) {
    return false;
  }
  if ((faithState.intensity ?? 0) < COVENANT_TEST_INTENSITY_THRESHOLD) {
    return false;
  }
  if (!hasCompletedFaithBuildingOfType(state, factionId, "grand_sanctuary")) {
    return false;
  }
  const currentInWorldDays = state.dualClock?.inWorldDays ?? 0;
  return (faithState.covenantTestCooldownUntilInWorldDays ?? 0) <= currentInWorldDays;
}

function issueCovenantTest(state, factionId) {
  const faction = getFaction(state, factionId);
  if (!faction?.dynasty) {
    return null;
  }
  const event = buildCovenantTestEvent(state, factionId);
  if (!event) {
    return null;
  }
  const politicalEvents = getDynastyPoliticalEventState(faction);
  politicalEvents.active = [event, ...(politicalEvents.active ?? [])].slice(0, 4);
  pushMessage(
    state,
    `${getFactionDisplayName(state, factionId)} faces the ${event.covenantName}'s covenant test: ${event.mandateLabel}.`,
    factionId === "player" ? "warn" : "info",
  );
  declareInWorldTime(state, 96, `Covenant Test issued: ${getFactionDisplayName(state, factionId)} under ${event.faithName}`);
  return event;
}

function getCovenantTestConvictionRewardBucket(event) {
  switch (event?.mandateId) {
    case "old_light_light_memorial":
    case "the_order_light_codification":
    case "the_wild_light_grove_renewal":
      return "stewardship";
    case "blood_dominion_light_ceremony":
      return "oathkeeping";
    case "old_light_dark_purge":
    case "the_wild_dark_predator_hunt":
      return "desecration";
    case "blood_dominion_dark_binding":
    case "the_order_dark_expansion":
      return "ruthlessness";
    default:
      return "oathkeeping";
  }
}

function completeCovenantTest(state, factionId, event, resolutionReason) {
  const faction = getFaction(state, factionId);
  const faithState = ensureFaithCovenantTestCompletionFromLegacyState(state, factionId);
  if (!faction?.dynasty || !event || !faithState) {
    return null;
  }
  const resolved = resolveDynastyPoliticalEvent(state, factionId, event, "completed", resolutionReason);
  faithState.covenantTestPassed = true;
  faithState.covenantTestCooldownUntilInWorldDays = 0;
  faithState.lastCovenantTestOutcome = {
    outcome: "completed",
    mandateId: event.mandateId,
    mandateLabel: event.mandateLabel,
    resolvedAt: state.meta.elapsed,
    resolvedAtInWorldDays: state.dualClock?.inWorldDays ?? 0,
    resolutionReason,
  };
  faction.faith.intensity = Math.max(faction.faith.intensity ?? 0, COVENANT_TEST_SUCCESS_INTENSITY_FLOOR);
  syncFaithIntensityState(faction);
  adjustLegitimacy(faction, COVENANT_TEST_SUCCESS_LEGITIMACY_BONUS);
  recordConvictionEvent(state, factionId, getCovenantTestConvictionRewardBucket(event), 3, `Completed ${event.mandateLabel}`);
  pushMessage(
    state,
    `${getFactionDisplayName(state, factionId)} completes the ${event.mandateLabel} and secures covenant recognition.`,
    factionId === "player" ? "good" : "warn",
  );
  declareInWorldTime(state, 112, `Covenant Test completed: ${getFactionDisplayName(state, factionId)} under ${event.faithName}`);
  return resolved;
}

function failCovenantTest(state, factionId, event, resolutionReason) {
  const faction = getFaction(state, factionId);
  const faithState = ensureFaithCovenantTestCompletionFromLegacyState(state, factionId);
  if (!faction?.dynasty || !event || !faithState) {
    return null;
  }
  const resolved = resolveDynastyPoliticalEvent(state, factionId, event, "failed", resolutionReason);
  faithState.covenantTestPassed = false;
  faithState.covenantTestCooldownUntilInWorldDays = (state.dualClock?.inWorldDays ?? 0) + COVENANT_TEST_RETRY_COOLDOWN_IN_WORLD_DAYS;
  faithState.lastCovenantTestOutcome = {
    outcome: "failed",
    mandateId: event.mandateId,
    mandateLabel: event.mandateLabel,
    resolvedAt: state.meta.elapsed,
    resolvedAtInWorldDays: state.dualClock?.inWorldDays ?? 0,
    resolutionReason,
  };
  spendFaithIntensity(faction, COVENANT_TEST_FAILURE_INTENSITY_LOSS);
  adjustLegitimacy(faction, -COVENANT_TEST_FAILURE_LEGITIMACY_LOSS);
  applyControlPointLoyaltyDelta(state, factionId, -COVENANT_TEST_FAILURE_LOYALTY_SHOCK);
  pushMessage(
    state,
    `${getFactionDisplayName(state, factionId)} fails the ${event.mandateLabel}; covenant confidence fractures across the realm.`,
    factionId === "player" ? "warn" : "good",
  );
  declareInWorldTime(state, 112, `Covenant Test failed: ${getFactionDisplayName(state, factionId)} under ${event.faithName}`);
  return resolved;
}

function getIncomingCovenantTests(state, factionId) {
  return Object.values(state.factions).flatMap((faction) => {
    if (!faction || faction.id === factionId) {
      return [];
    }
    const event = getActiveCovenantTest(faction);
    return event
      ? [{
        ...serializeCovenantTestEvent(state, faction.id, event),
        sourceFactionId: faction.id,
      }]
      : [];
  });
}

function getFactionPoliticalEventEffects(state, factionId) {
  const faction = getFaction(state, factionId);
  const activeSuccessionCrisis = getActiveSuccessionCrisis(faction);
  const activeCovenantTest = getActiveCovenantTest(faction);
  const effects = { ...DEFAULT_POLITICAL_EVENT_EFFECTS };
  if (activeSuccessionCrisis) {
    const severity = getSuccessionCrisisSeverityProfile(activeSuccessionCrisis.severityId);
    effects.resourceTrickleMultiplier *= severity.resourceTrickleMultiplier;
    effects.populationGrowthMultiplier *= severity.populationGrowthMultiplier;
    effects.stabilizationMultiplier *= severity.stabilizationMultiplier;
    effects.attackMultiplier *= severity.attackMultiplier;
  }
  if (activeCovenantTest) {
    effects.resourceTrickleMultiplier *= COVENANT_TEST_ACTIVE_EFFECTS.resourceTrickleMultiplier;
    effects.populationGrowthMultiplier *= COVENANT_TEST_ACTIVE_EFFECTS.populationGrowthMultiplier;
    effects.stabilizationMultiplier *= COVENANT_TEST_ACTIVE_EFFECTS.stabilizationMultiplier;
    effects.attackMultiplier *= COVENANT_TEST_ACTIVE_EFFECTS.attackMultiplier;
  }
  return effects;
}

function startSuccessionCrisis(state, factionId, fallenMember) {
  const faction = getFaction(state, factionId);
  if (!faction?.dynasty) {
    return null;
  }

  const profile = buildSuccessionCrisisTriggerProfile(state, factionId, fallenMember);
  if (!profile.shouldTrigger) {
    return null;
  }

  const politicalEvents = getDynastyPoliticalEventState(faction);
  const existing = getActiveSuccessionCrisis(faction);
  const severity = getSuccessionCrisisSeverityProfile(profile.severityId);
  if (existing) {
    existing.severityId = getMoreSevereSuccessionCrisisId(existing.severityId, profile.severityId);
    existing.currentRulerId = profile.currentRuler?.id ?? null;
    existing.currentRulerTitle = profile.currentRuler?.title ?? null;
    existing.claimantIds = profile.claimantProfiles.map((entry) => entry.id);
    existing.claimantTitles = profile.claimantProfiles.map((entry) => entry.title);
    existing.triggerReasons = [...profile.triggerReasons];
    existing.interregnum = profile.interregnum;
    existing.youngRuler = profile.youngRuler;
    existing.underageRuler = profile.underageRuler;
    existing.lastEscalatedAtInWorldDays = state.dualClock?.inWorldDays ?? 0;
    return existing;
  }

  const crisis = {
    id: createEntityId(state, "dynastyEvent"),
    type: "succession_crisis",
    severityId: profile.severityId,
    startedAt: state.meta.elapsed,
    startedAtInWorldDays: state.dualClock?.inWorldDays ?? 0,
    lastEffectTickInWorldDays: Math.floor(state.dualClock?.inWorldDays ?? 0),
    lastEscalatedAtInWorldDays: state.dualClock?.inWorldDays ?? 0,
    lastReminderAtInWorldDays: state.dualClock?.inWorldDays ?? 0,
    fallenMemberId: fallenMember?.id ?? null,
    fallenMemberTitle: fallenMember?.title ?? null,
    currentRulerId: profile.currentRuler?.id ?? null,
    currentRulerTitle: profile.currentRuler?.title ?? null,
    claimantIds: profile.claimantProfiles.map((entry) => entry.id),
    claimantTitles: profile.claimantProfiles.map((entry) => entry.title),
    triggerReasons: [...profile.triggerReasons],
    interregnum: profile.interregnum,
    youngRuler: profile.youngRuler,
    underageRuler: profile.underageRuler,
  };
  politicalEvents.active = [crisis, ...(politicalEvents.active ?? [])].slice(0, 4);

  applyControlPointLoyaltyDelta(state, factionId, -severity.loyaltyShock);
  pushMessage(
    state,
    `${getFactionDisplayName(state, factionId)} enters a ${severity.label.toLowerCase()} succession crisis as ${crisis.currentRulerTitle ?? "the dynastic seat"} comes under dispute.`,
    factionId === "player" ? "warn" : "info",
  );
  return crisis;
}

function resolveDynastyPoliticalEvent(state, factionId, event, outcome, resolutionReason) {
  const faction = getFaction(state, factionId);
  if (!faction?.dynasty || !event) {
    return null;
  }
  const politicalEvents = getDynastyPoliticalEventState(faction);
  politicalEvents.active = (politicalEvents.active ?? []).filter((entry) => entry.id !== event.id);
  const resolved = {
    ...event,
    outcome,
    resolvedAt: state.meta.elapsed,
    resolvedAtInWorldDays: state.dualClock?.inWorldDays ?? 0,
    resolutionReason,
  };
  pushDynastyPoliticalEventHistory(faction, resolved);
  return resolved;
}

function escalateSuccessionCrisis(state, factionId, event) {
  if (!event) {
    return null;
  }
  const currentIndex = SUCCESSION_CRISIS_SEVERITY_ORDER.indexOf(event.severityId);
  if (currentIndex === -1 || currentIndex >= SUCCESSION_CRISIS_SEVERITY_ORDER.length - 1) {
    return null;
  }
  event.severityId = SUCCESSION_CRISIS_SEVERITY_ORDER[currentIndex + 1];
  event.lastEscalatedAtInWorldDays = state.dualClock?.inWorldDays ?? 0;
  const severity = getSuccessionCrisisSeverityProfile(event.severityId);
  pushMessage(
    state,
    `${getFactionDisplayName(state, factionId)} slips deeper into ${severity.label.toLowerCase()} succession crisis as the court remains unsettled.`,
    factionId === "player" ? "warn" : "info",
  );
  return event;
}

function tickDynastyPoliticalEvents(state) {
  const currentInWorldDays = state.dualClock?.inWorldDays ?? 0;
  const currentDay = Math.floor(currentInWorldDays);
  Object.values(state.factions).forEach((faction) => {
    ensureFaithCovenantTestCompletionFromLegacyState(state, faction.id);
    if (shouldIssueCovenantTest(state, faction.id)) {
      issueCovenantTest(state, faction.id);
    }

    const crisis = getActiveSuccessionCrisis(faction);
    if (crisis) {
      const severity = getSuccessionCrisisSeverityProfile(crisis.severityId);
      const lastTickDay = Math.floor(crisis.lastEffectTickInWorldDays ?? crisis.startedAtInWorldDays ?? currentInWorldDays);
      const elapsedDays = currentDay - lastTickDay;
      if (elapsedDays > 0) {
        adjustLegitimacy(faction, -(severity.legitimacyDrainPerDay * elapsedDays));
        applyControlPointLoyaltyDelta(state, faction.id, -(severity.loyaltyDrainPerDay * elapsedDays));
        crisis.lastEffectTickInWorldDays = currentDay;
      }

      const lastEscalatedDay = Math.floor(crisis.lastEscalatedAtInWorldDays ?? crisis.startedAtInWorldDays ?? currentInWorldDays);
      if ((currentDay - lastEscalatedDay) >= SUCCESSION_CRISIS_ESCALATION_IN_WORLD_DAYS) {
        escalateSuccessionCrisis(state, faction.id, crisis);
      }

      const lastReminderDay = Math.floor(crisis.lastReminderAtInWorldDays ?? crisis.startedAtInWorldDays ?? currentInWorldDays);
      if ((currentDay - lastReminderDay) >= 45) {
        crisis.lastReminderAtInWorldDays = currentDay;
        pushMessage(
          state,
          `${getFactionDisplayName(state, faction.id)} still struggles to settle the succession.`,
          faction.id === "player" ? "warn" : "info",
        );
      }
    }

    const covenantTest = getActiveCovenantTest(faction);
    if (covenantTest) {
      const evaluation = evaluateCovenantTestProgress(state, faction.id, covenantTest);
      if (evaluation.completed) {
        completeCovenantTest(
          state,
          faction.id,
          covenantTest,
          evaluation.actionAvailable ? "ritual action pending final witness" : "mandate satisfied",
        );
        return;
      }
      if ((covenantTest.resolveByInWorldDays ?? currentInWorldDays) <= currentInWorldDays) {
        failCovenantTest(state, faction.id, covenantTest, "the covenant deadline elapsed");
        return;
      }
      const lastReminderDay = Math.floor(covenantTest.lastReminderAtInWorldDays ?? covenantTest.startedAtInWorldDays ?? currentInWorldDays);
      if ((currentDay - lastReminderDay) >= 45) {
        covenantTest.lastReminderAtInWorldDays = currentDay;
        pushMessage(
          state,
          `${getFactionDisplayName(state, faction.id)} remains under covenant test pressure: ${covenantTest.mandateLabel}.`,
          faction.id === "player" ? "warn" : "info",
        );
      }
      if ((evaluation.shortfalls ?? []).length === 0 && !evaluation.actionAvailable) {
        completeCovenantTest(state, faction.id, covenantTest, "mandate conditions fully satisfied");
      }
    }
  });
}

function pushDynastyOperationHistory(faction, entry) {
  const operations = getDynastyOperationsState(faction);
  if (!operations) {
    return;
  }
  operations.history = [entry, ...operations.history].slice(0, DYNASTY_OPERATION_HISTORY_LIMIT);
}

function canAffordCost(resources, cost = {}) {
  return Object.entries(cost).every(([resourceId, amount]) => (resources[resourceId] ?? 0) >= amount);
}

function applyCost(resources, cost = {}) {
  Object.entries(cost).forEach(([resourceId, amount]) => {
    resources[resourceId] = Math.max(0, (resources[resourceId] ?? 0) - amount);
  });
}

function spendFaithIntensity(faction, amount) {
  if (!faction?.faith) {
    return;
  }
  faction.faith.intensity = Math.max(0, (faction.faith.intensity ?? 0) - amount);
  syncFaithIntensityState(faction);
}

function grantResources(resources, gain = {}) {
  Object.entries(gain).forEach(([resourceId, amount]) => {
    resources[resourceId] = (resources[resourceId] ?? 0) + amount;
  });
}

function getOperationProgress(operation, elapsed) {
  const startedAt = operation.startedAt ?? elapsed;
  const resolveAt = operation.resolveAt ?? elapsed;
  const duration = Math.max(0.001, resolveAt - startedAt);
  return Math.max(0, Math.min(1, (elapsed - startedAt) / duration));
}

function getCaptiveRoleOperationMultiplier(roleId) {
  return CAPTIVE_ROLE_OPERATION_MULTIPLIERS[roleId] ?? 1;
}

function getRoleRenownBonus(member, divisor, cap = 0.2) {
  if (!member || !isMemberAvailable(member)) {
    return 0;
  }
  return Math.min(cap, (member.renown ?? 0) / divisor);
}

function findCaptiveRecordByMemberId(faction, memberId) {
  return (faction?.dynasty?.captives ?? []).find((captive) => captive.memberId === memberId) ?? null;
}

function findCaptiveRecordById(faction, captiveId) {
  return (faction?.dynasty?.captives ?? []).find((captive) => captive.id === captiveId) ?? null;
}

function getActiveDynastyOperationForMember(faction, memberId) {
  return (faction?.dynasty?.operations?.active ?? []).find((operation) => operation.memberId === memberId) ?? null;
}

function getActiveDynastyOperationByType(faction, type) {
  return (faction?.dynasty?.operations?.active ?? []).find((operation) => operation.type === type) ?? null;
}

function getActiveDynastyOperationForTargetFaction(faction, type, targetFactionId) {
  return (faction?.dynasty?.operations?.active ?? []).find((operation) =>
    operation.type === type &&
    operation.targetFactionId === targetFactionId) ?? null;
}

function getActiveDynastyOperationForTargetMember(faction, type, targetFactionId, targetMemberId) {
  return (faction?.dynasty?.operations?.active ?? []).find((operation) =>
    operation.type === type &&
    operation.targetFactionId === targetFactionId &&
    operation.targetMemberId === targetMemberId) ?? null;
}

function getActiveIntelligenceReport(faction, targetFactionId, elapsed) {
  return (faction?.dynasty?.intelligenceReports ?? []).find((report) =>
    report.targetFactionId === targetFactionId && (report.expiresAt ?? 0) > elapsed) ?? null;
}

function getActiveCounterIntelligence(faction, elapsed) {
  return getDynastyCounterIntelligenceState(faction).find((entry) => (entry.expiresAt ?? 0) > elapsed) ?? null;
}

function tickDynastyIntelligenceReports(state) {
  Object.values(state.factions).forEach((faction) => {
    if (!faction?.dynasty) {
      return;
    }
    faction.dynasty.intelligenceReports = (faction.dynasty.intelligenceReports ?? []).filter((report) =>
      (report.expiresAt ?? 0) > state.meta.elapsed);
  });
}

function tickDynastyCounterIntelligence(state) {
  Object.values(state.factions).forEach((faction) => {
    if (!faction?.dynasty) {
      return;
    }
    const active = getDynastyCounterIntelligenceState(faction);
    const expired = active.filter((entry) => (entry.expiresAt ?? 0) <= state.meta.elapsed);
    faction.dynasty.counterIntelligence = active.filter((entry) => (entry.expiresAt ?? 0) > state.meta.elapsed);
    if (faction.id === "player" && expired.length > 0) {
      pushMessage(state, "Your counter-intelligence watch around the bloodline court has lapsed.", "warn");
    }
  });
}

function getDynastyCourtLoyaltyProfile(state, factionId) {
  const territories = state.world.controlPoints.filter((controlPoint) => controlPoint.ownerFactionId === factionId);
  if (territories.length === 0) {
    return { averageLoyalty: 50, weakestLoyalty: 50, territoryCount: 0 };
  }
  const totalLoyalty = territories.reduce((sum, controlPoint) => sum + (controlPoint.loyalty ?? 50), 0);
  return {
    averageLoyalty: totalLoyalty / territories.length,
    weakestLoyalty: territories.reduce((min, controlPoint) => Math.min(min, controlPoint.loyalty ?? 50), 100),
    territoryCount: territories.length,
  };
}

function getCounterIntelligenceRoleGuardBonus(roleId) {
  switch (roleId) {
    case "head_of_bloodline":
      return 10;
    case "heir_designate":
      return 8;
    case "commander":
      return 8;
    case "spymaster":
      return 7;
    case "governor":
      return 6;
    default:
      return 0;
  }
}

function tickFaithHolyWars(state, dt) {
  Object.values(state.factions).forEach((faction) => {
    if (!faction?.faith) {
      return;
    }

    const retained = [];
    getFaithHolyWarsState(faction).forEach((entry) => {
      if ((entry.expiresAt ?? 0) <= state.meta.elapsed) {
        if (faction.id === "player" || entry.targetFactionId === "player") {
          pushMessage(
            state,
            `${getFactionDisplayName(state, faction.id)} holy war against ${getFactionDisplayName(state, entry.targetFactionId)} has run out of declared fervor.`,
            faction.id === "player" ? "info" : entry.targetFactionId === "player" ? "good" : "info",
          );
        }
        return;
      }

      const targetFaction = getFaction(state, entry.targetFactionId);
      if (!targetFaction?.faith) {
        return;
      }

      adjustLegitimacy(
        targetFaction,
        -((entry.loyaltyPulse ?? 0) * HOLY_WAR_SUSTAINED_LEGITIMACY_DRAIN_MULTIPLIER * dt),
      );
      const targetPoint = getLowestLoyaltyControlPoint(state, entry.targetFactionId);
      if (targetPoint) {
        targetPoint.loyalty = clamp(
          targetPoint.loyalty - ((entry.loyaltyPulse ?? 0) * HOLY_WAR_SUSTAINED_LOYALTY_DRAIN_MULTIPLIER * dt),
          0,
          100,
        );
      }

      if ((entry.lastPulseAt ?? entry.declaredAt ?? 0) + HOLY_WAR_PULSE_INTERVAL_SECONDS <= state.meta.elapsed) {
        entry.lastPulseAt = state.meta.elapsed;
        faction.faith.intensity = (faction.faith.intensity ?? 0) + (entry.intensityPulse ?? 0);
        syncFaithIntensityState(faction);
        if (targetPoint) {
          targetPoint.loyalty = clamp(targetPoint.loyalty - (entry.loyaltyPulse ?? 0), 0, 100);
        } else {
          adjustLegitimacy(targetFaction, -Math.max(1, Math.round((entry.loyaltyPulse ?? 1) * 0.4)));
        }
      }

      retained.push(entry);
    });

    faction.faith.activeHolyWars = retained.slice(0, 6);
  });
}

function removeCaptiveRecord(faction, memberId, captiveId = null) {
  if (!faction?.dynasty?.captives) {
    return null;
  }
  const index = faction.dynasty.captives.findIndex((candidate) =>
    (captiveId && candidate.id === captiveId) || candidate.memberId === memberId,
  );
  if (index === -1) {
    return null;
  }
  const [removed] = faction.dynasty.captives.splice(index, 1);
  return removed ?? null;
}

function deriveConvictionScore(buckets) {
  return (buckets.stewardship + buckets.oathkeeping) - (buckets.ruthlessness + buckets.desecration);
}

function refreshConvictionBand(state, factionId) {
  const faction = getFaction(state, factionId);
  if (!faction?.conviction) {
    return;
  }

  faction.conviction.score = deriveConvictionScore(faction.conviction.buckets);
  const band = getConvictionBand(state.content, faction.conviction.score);
  faction.conviction.bandId = band.id;
  faction.conviction.bandLabel = band.label;
}

function recordConvictionEvent(state, factionId, bucket, amount, reason) {
  const faction = getFaction(state, factionId);
  if (!faction?.conviction?.buckets || !Object.hasOwn(faction.conviction.buckets, bucket)) {
    return;
  }

  faction.conviction.buckets[bucket] += amount;
  faction.conviction.eventLedger.unshift({
    id: `${Date.now()}-${Math.random().toString(16).slice(2, 6)}`,
    bucket,
    amount,
    reason,
  });
  faction.conviction.eventLedger = faction.conviction.eventLedger.slice(0, 10);
  refreshConvictionBand(state, factionId);
}

function createDynastyState(content, factionSetup, factionId) {
  if (factionSetup.kind === "tribe") {
    return null;
  }

  const templates = [
    { suffix: "head", title: "Head of Bloodline", roleId: "head_of_bloodline", pathId: "governance", age: 38, status: "ruling", renown: 22 },
    { suffix: "heir", title: "Eldest Heir", roleId: "heir_designate", pathId: "military_command", age: 19, status: "active", renown: 12 },
    { suffix: "marshal", title: "War Captain", roleId: "commander", pathId: "military_command", age: 28, status: "active", renown: 14 },
    { suffix: "governor", title: "March Governor", roleId: "governor", pathId: "governance", age: 31, status: "active", renown: 10 },
    { suffix: "envoy", title: "House Envoy", roleId: "diplomat", pathId: "diplomacy", age: 27, status: "active", renown: 9 },
    { suffix: "priest", title: "Covenant Speaker", roleId: "ideological_leader", pathId: "religious_leadership", age: 24, status: "active", renown: 8 },
    { suffix: "factor", title: "Trade Factor", roleId: "merchant", pathId: "economic_stewardship_trade", age: 30, status: "active", renown: 8 },
    { suffix: "shadow", title: "House Shadow", roleId: "spymaster", pathId: "covert_operations", age: 26, status: "active", renown: 7 },
  ];

  return {
    activeMemberCap: 20,
    dormantReserve: 0,
    legitimacy: 58,
    loyaltyPressure: 0,
    interregnum: false,
    attachments: {
      commanderMemberId: null,
      commanderUnitId: null,
      heirMemberId: null,
      governorAssignments: [],
      fallenMembers: [],
      capturedMembers: {},
    },
    captives: [],
    operations: {
      active: [],
      history: [],
    },
    politicalEvents: {
      active: [],
      history: [],
    },
    intelligenceReports: [],
    counterIntelligence: [],
    // Session 33: Marriage state. Canonical master doctrine section XVIII.
    // marriages: array of { id, headMemberId, spouseMemberId, spouseFactionId,
    //                       spouseHouseId, marriedAtInWorldDays, children:[ids] }
    marriages: [],
    marriageProposalsIn: [],
    marriageProposalsOut: [],
    // Session 35: Lesser houses promotion pipeline (Vector 2 continuation).
    // Canonical master doctrine: war heroes who accumulate sufficient renown and
    // serve on the military command path earn the right to a cadet branch.
    // lesserHouses: array of { id, name, founderMemberId, foundedAtInWorldDays,
    //                          parentHouseId, loyalty, status }
    // lesserHouseCandidates: array of memberIds auto-flagged as eligible.
    // Promotion is player-consented (canonical: new houses are a diplomatic act).
    lesserHouses: [],
    lesserHouseCandidates: [],
    members: templates.map((template, index) => ({
      id: `${factionId}-bloodline-${template.suffix}`,
      title: template.title,
      roleId: template.roleId,
      pathId: template.pathId,
      age: template.age,
      status: template.status,
      renown: template.renown,
      order: index,
      capturedByFactionId: null,
      fallenAt: null,
      promotionHistory: [],
    })),
  };
}

function createFaithState(content, factionSetup) {
  const exposure = Object.fromEntries(content.faiths.map((faith) => [faith.id, 0]));
  const selectedFaithId = factionSetup.startingFaithId ?? null;
  const startingIntensity = selectedFaithId ? 25 : 0;
  const tier = getFaithTier(startingIntensity);

  return {
    selectedFaithId,
    doctrinePath: selectedFaithId ? (factionSetup.startingDoctrinePath ?? "light") : null,
    intensity: startingIntensity,
    level: tier.level,
    tierLabel: tier.label,
    exposure,
    discoveredFaithIds: selectedFaithId ? [selectedFaithId] : [],
    activeHolyWars: [],
    covenantTestPassed: false,
    lastCovenantTestOutcome: null,
    covenantTestCooldownUntilInWorldDays: 0,
    divineRightDeclaration: null,
    lastDivineRightOutcome: null,
    divineRightCooldownUntil: 0,
  };
}

function createConvictionState(content) {
  const score = 0;
  const band = getConvictionBand(content, score);
  return {
    score,
    bandId: band.id,
    bandLabel: band.label,
    buckets: {
      ruthlessness: 0,
      stewardship: 0,
      oathkeeping: 0,
      desecration: 0,
    },
    eventLedger: [],
  };
}

function pushMessage(state, text, tone = "info") {
  state.messages.unshift({
    id: `${Date.now()}-${Math.random().toString(16).slice(2, 8)}`,
    text,
    tone,
    ttl: 8,
  });
  state.messages = state.messages.slice(0, MESSAGE_LIMIT);
}

function updateMessages(state, dt) {
  state.messages = state.messages
    .map((message) => ({ ...message, ttl: message.ttl - dt }))
    .filter((message) => message.ttl > 0);
}

function findNearestHostileCombatantFaction(state, point, victimFactionId) {
  let best = null;
  state.units.forEach((candidate) => {
    if (candidate.health <= 0) {
      return;
    }
    if (candidate.factionId === victimFactionId) {
      return;
    }
    if (!areHostile(state, candidate.factionId, victimFactionId)) {
      return;
    }
    const candidateDef = getUnitDef(state.content, candidate.typeId);
    if (candidateDef.role === "worker") {
      return;
    }
    const candidateDistance = distance(point.x, point.y, candidate.x, candidate.y);
    if (candidateDistance > CAPTURE_PROXIMITY_RADIUS) {
      return;
    }
    const candidateFaction = getFaction(state, candidate.factionId);
    if (!candidateFaction || candidateFaction.kind !== "kingdom") {
      return;
    }
    if (!best || candidateDistance < best.distance) {
      best = { factionId: candidate.factionId, distance: candidateDistance };
    }
  });
  return best;
}

function transferMemberToCaptor(state, member, victimFactionId, captorFactionId, reason) {
  const captorFaction = getFaction(state, captorFactionId);
  if (!captorFaction?.dynasty) {
    return null;
  }

  const elapsed = state.meta.elapsed;
  const captive = {
    id: `captive-${member.id}-${Math.floor(elapsed * 10)}`,
    memberId: member.id,
    sourceFactionId: victimFactionId,
    sourceFactionName: getFactionDisplayName(state, victimFactionId),
    title: member.title,
    roleId: member.roleId,
    pathId: member.pathId,
    renown: member.renown,
    age: member.age,
    capturedAt: elapsed,
    reason,
  };

  captorFaction.dynasty.captives = [captive, ...captorFaction.dynasty.captives].slice(0, CAPTIVE_LEDGER_LIMIT);

  const victimFaction = getFaction(state, victimFactionId);
  if (victimFaction?.dynasty?.attachments) {
    victimFaction.dynasty.attachments.capturedMembers = {
      ...(victimFaction.dynasty.attachments.capturedMembers ?? {}),
      [member.id]: captorFactionId,
    };
  }

  return captive;
}

function clearCommanderLinksFor(state, factionId, memberId) {
  const faction = getFaction(state, factionId);
  if (!faction?.dynasty?.attachments) {
    return;
  }

  if (faction.dynasty.attachments.commanderMemberId === memberId) {
    faction.dynasty.attachments.commanderMemberId = null;
    faction.dynasty.attachments.commanderUnitId = null;
  }

  state.units.forEach((unit) => {
    if (unit.factionId === factionId && unit.commanderMemberId === memberId) {
      unit.commanderMemberId = null;
    }
  });
}

function clearGovernorLinksFor(state, factionId, memberId) {
  state.world.controlPoints.forEach((controlPoint) => {
    if (controlPoint.ownerFactionId === factionId && controlPoint.governorMemberId === memberId) {
      controlPoint.governorMemberId = null;
    }
  });
  (state.world.settlements ?? []).forEach((settlement) => {
    if (settlement.factionId === factionId && settlement.governorMemberId === memberId) {
      settlement.governorMemberId = null;
    }
  });

  const faction = getFaction(state, factionId);
  if (faction?.dynasty?.attachments) {
    faction.dynasty.attachments.governorAssignments = (faction.dynasty.attachments.governorAssignments ?? [])
      .filter((assignment) => assignment.memberId !== memberId);
  }
}

function getGovernorProfileForControlPoint(state, controlPoint) {
  if (!controlPoint?.governorMemberId) {
    return DEFAULT_GOVERNOR_SPECIALIZATION;
  }
  return getGovernorSpecializationProfile(
    getGovernorSpecializationIdForSettlementClass(controlPoint.settlementClass),
  );
}

function getGovernorProfileForSettlement(state, settlement) {
  if (!settlement?.governorMemberId) {
    return DEFAULT_GOVERNOR_SPECIALIZATION;
  }
  return getGovernorSpecializationProfile(
    getGovernorSpecializationIdForSettlementClass(settlement.settlementClass),
  );
}

function getProtectedLoyaltyDelta(delta, protectionMultiplier = 1) {
  if (delta >= 0) {
    return delta;
  }
  return delta / Math.max(1, protectionMultiplier);
}

function applyControlPointLoyaltyDelta(state, factionId, loyaltyDelta, options = {}) {
  if (!Number.isFinite(loyaltyDelta) || loyaltyDelta === 0) {
    return 0;
  }
  const minLoyalty = options.minLoyalty ?? 0;
  const maxLoyalty = options.maxLoyalty ?? 100;
  let affected = 0;
  for (const cp of state.world.controlPoints) {
    if (cp.ownerFactionId !== factionId) {
      continue;
    }
    const governorProfile = getGovernorProfileForControlPoint(state, cp);
    const verdantWardenSupport = getVerdantWardenControlPointSupportProfile(state, cp);
    const adjustedDelta = loyaltyDelta >= 0
      ? loyaltyDelta * (verdantWardenSupport.loyaltyGainMultiplier ?? 1)
      : getProtectedLoyaltyDelta(
          loyaltyDelta,
          governorProfile.loyaltyProtectionMultiplier * (verdantWardenSupport.loyaltyProtectionMultiplier ?? 1),
        );
    cp.loyalty = Math.max(
      minLoyalty,
      Math.min(
        maxLoyalty,
        cp.loyalty + adjustedDelta,
      ),
    );
    affected += 1;
  }
  return affected;
}

function applySuccessionRipple(state, factionId, member) {
  const faction = getFaction(state, factionId);
  if (!faction?.dynasty) {
    return;
  }

  if (member.roleId === "head_of_bloodline") {
    adjustLegitimacy(faction, -LEGITIMACY_LOSS_HEAD_FALL);
    recordConvictionEvent(
      state,
      factionId,
      "oathkeeping",
      2,
      `House ${getFactionDisplayName(state, factionId)} mourns ${member.title}`,
    );
  }

  const surviving = faction.dynasty.members.filter(isMemberAvailable);
  if (surviving.length === 0) {
    faction.dynasty.interregnum = true;
    adjustLegitimacy(faction, -LEGITIMACY_LOSS_INTERREGNUM);
    pushMessage(
      state,
      `${getFactionDisplayName(state, factionId)} has no living bloodline to govern. Interregnum begins.`,
      factionId === "player" ? "warn" : "info",
    );
    return;
  }

  if (member.roleId === "head_of_bloodline") {
    const heir = findAvailableSuccessor(faction, new Set([member.id]));
    if (heir) {
      const previousRole = heir.roleId;
      logPromotion(heir, previousRole, "head_of_bloodline", state.meta.elapsed);
      heir.roleId = "head_of_bloodline";
      heir.status = "ruling";
      heir.renown = Math.min(60, heir.renown + 6);
      faction.dynasty.attachments.heirMemberId = null;
      adjustLegitimacy(faction, LEGITIMACY_RECOVERY_ON_SUCCESSION);
      recordConvictionEvent(state, factionId, "stewardship", 2, `Elevated ${heir.title} to lead the bloodline`);
      pushMessage(
        state,
        `${heir.title} ascends as the new head of ${getFactionDisplayName(state, factionId)}.`,
        factionId === "player" ? "good" : "info",
      );
      backfillHeir(state, factionId, heir.id);
    } else {
      faction.dynasty.interregnum = true;
      pushMessage(
        state,
        `${getFactionDisplayName(state, factionId)} has no heir able to claim the ruling seat.`,
        factionId === "player" ? "warn" : "info",
      );
    }
  }

  if (member.roleId === "heir_designate") {
    backfillHeir(state, factionId, null);
  }

  if (member.roleId === "head_of_bloodline") {
    startSuccessionCrisis(state, factionId, member);
  }
}

function backfillHeir(state, factionId, newRulerId) {
  const faction = getFaction(state, factionId);
  if (!faction?.dynasty) {
    return;
  }

  const excludeIds = new Set();
  if (newRulerId) {
    excludeIds.add(newRulerId);
  }

  const existingHeir = faction.dynasty.members.find((member) => member.roleId === "heir_designate" && isMemberAvailable(member));
  if (existingHeir) {
    faction.dynasty.attachments.heirMemberId = existingHeir.id;
    return;
  }

  const candidate = findAvailableSuccessor(faction, excludeIds);
  if (!candidate) {
    faction.dynasty.attachments.heirMemberId = null;
    return;
  }

  const previousRole = candidate.roleId;
  logPromotion(candidate, previousRole, "heir_designate", state.meta.elapsed);
  candidate.roleId = "heir_designate";
  if (candidate.status === "commanding" || candidate.status === "governing") {
    candidate.status = "active";
  }
  faction.dynasty.attachments.heirMemberId = candidate.id;
  pushMessage(
    state,
    `${candidate.title} is now named heir to ${getFactionDisplayName(state, factionId)}.`,
    factionId === "player" ? "good" : "info",
  );
}

export function getSuccessionCrisisTerms(state, factionId) {
  const faction = getFaction(state, factionId);
  if (!faction?.dynasty) {
    return {
      active: false,
      available: false,
      reason: "Faction has no dynastic event state.",
    };
  }

  const crisis = getActiveSuccessionCrisis(faction);
  if (!crisis) {
    return {
      active: false,
      available: false,
      reason: "No active succession crisis.",
    };
  }

  const severity = getSuccessionCrisisSeverityProfile(crisis.severityId);
  const ruler = getDynastyMemberByRole(faction, ["head_of_bloodline"]);
  if (!ruler) {
    return {
      active: true,
      available: false,
      reason: "No ruling claimant sits the dynastic seat.",
      crisis: serializeSuccessionCrisisEvent(state, factionId, crisis),
      cost: severity.resolutionCost,
    };
  }

  const available = canAffordCost(faction.resources, severity.resolutionCost);
  return {
    active: true,
    available,
    reason: available ? null : "Need more gold and influence to consolidate the court.",
    cost: severity.resolutionCost,
    legitimacyRecovery: severity.legitimacyRecovery,
    loyaltyRecovery: severity.loyaltyRecovery,
    crisis: serializeSuccessionCrisisEvent(state, factionId, crisis),
    rulerTitle: ruler.title,
  };
}

export function consolidateSuccessionCrisis(state, factionId) {
  const faction = getFaction(state, factionId);
  const terms = getSuccessionCrisisTerms(state, factionId);
  if (!faction?.dynasty) {
    return { ok: false, reason: "Faction has no dynasty." };
  }
  if (!terms.active) {
    return { ok: false, reason: terms.reason ?? "No active succession crisis." };
  }
  if (!terms.available) {
    return { ok: false, reason: terms.reason ?? "Succession cannot yet be consolidated." };
  }

  applyCost(faction.resources, terms.cost);
  adjustLegitimacy(faction, terms.legitimacyRecovery ?? 0);
  applyControlPointLoyaltyDelta(state, factionId, terms.loyaltyRecovery ?? 0);
  recordConvictionEvent(
    state,
    factionId,
    "stewardship",
    Math.max(2, Math.round((terms.legitimacyRecovery ?? 4) * 0.5)),
    `Consolidated the succession of ${getFactionDisplayName(state, factionId)}`,
  );
  declareInWorldTime(
    state,
    Math.max(7, Math.round((terms.loyaltyRecovery ?? 3) * 5)),
    `${getFactionDisplayName(state, factionId)} consolidates the succession`,
  );

  const resolved = resolveDynastyPoliticalEvent(
    state,
    factionId,
    getActiveSuccessionCrisis(faction),
    "resolved",
    `${terms.rulerTitle ?? "The ruling claimant"} consolidated the court.`,
  );
  pushMessage(
    state,
    `${terms.rulerTitle ?? "The ruling claimant"} consolidates the succession of ${getFactionDisplayName(state, factionId)}. Court loyalty begins to recover.`,
    factionId === "player" ? "good" : "info",
  );

  return {
    ok: true,
    eventId: resolved?.id ?? null,
  };
}

function handleCommanderFall(state, unit, attackerFactionId) {
  const faction = getFaction(state, unit.factionId);
  if (!faction?.dynasty || !unit.commanderMemberId) {
    return;
  }
  const member = getDynastyMember(faction, unit.commanderMemberId);
  if (!member) {
    return;
  }

  const deathPoint = { x: unit.x, y: unit.y };
  const captorInfo = attackerFactionId && getFaction(state, attackerFactionId)?.kind === "kingdom"
    ? findNearestHostileCombatantFaction(state, deathPoint, unit.factionId)
    : null;

  if (captorInfo) {
    const captor = captorInfo.factionId;
    member.status = "captured";
    member.capturedByFactionId = captor;
    member.fallenAt = state.meta.elapsed;
    transferMemberToCaptor(state, member, unit.factionId, captor, `Taken captive near ${Math.round(unit.x)}, ${Math.round(unit.y)}`);
    adjustLegitimacy(faction, -LEGITIMACY_LOSS_COMMANDER_CAPTURE);
    recordConvictionEvent(state, captor, "ruthlessness", 2, `Captured ${member.title}`);
    recordConvictionEvent(state, captor, "stewardship", 1, `Holds ${member.title} for ransom`);
    pushMessage(
      state,
      `${getFactionDisplayName(state, captor)} captured ${member.title}.`,
      unit.factionId === "player" ? "warn" : "good",
    );
    appendFallenLedger(faction, {
      memberId: member.id,
      title: member.title,
      roleId: member.roleId,
      disposition: "captured",
      capturedByFactionId: captor,
      at: state.meta.elapsed,
    });
  } else {
    member.status = "fallen";
    member.fallenAt = state.meta.elapsed;
    adjustLegitimacy(faction, -LEGITIMACY_LOSS_COMMANDER_KILL);
    if (attackerFactionId && getFaction(state, attackerFactionId)) {
      recordConvictionEvent(state, attackerFactionId, "ruthlessness", 3, `Slew ${member.title}`);
    }
    pushMessage(
      state,
      `${getFactionDisplayName(state, unit.factionId)} lost ${member.title} in battle.`,
      unit.factionId === "player" ? "warn" : "good",
    );
    appendFallenLedger(faction, {
      memberId: member.id,
      title: member.title,
      roleId: member.roleId,
      disposition: "fallen",
      attackerFactionId: attackerFactionId ?? null,
      at: state.meta.elapsed,
    });
  }

  clearCommanderLinksFor(state, unit.factionId, member.id);
  clearGovernorLinksFor(state, unit.factionId, member.id);
  applySuccessionRipple(state, unit.factionId, member);
}

function handleGovernorLoss(state, previousOwnerId, governorMemberId, newOwnerId, position) {
  const faction = getFaction(state, previousOwnerId);
  if (!faction?.dynasty || !governorMemberId) {
    return;
  }

  const member = getDynastyMember(faction, governorMemberId);
  if (!member || !isMemberAvailable(member)) {
    return;
  }

  const captorInfo = newOwnerId && getFaction(state, newOwnerId)?.kind === "kingdom"
    ? findNearestHostileCombatantFaction(state, position, previousOwnerId)
    : null;

  clearGovernorLinksFor(state, previousOwnerId, governorMemberId);

  if (captorInfo && captorInfo.factionId === newOwnerId) {
    member.status = "captured";
    member.capturedByFactionId = newOwnerId;
    member.fallenAt = state.meta.elapsed;
    transferMemberToCaptor(state, member, previousOwnerId, newOwnerId, "Seized during territorial loss");
    adjustLegitimacy(faction, -LEGITIMACY_LOSS_COMMANDER_CAPTURE);
    recordConvictionEvent(state, newOwnerId, "ruthlessness", 2, `Captured ${member.title}`);
    pushMessage(
      state,
      `${getFactionDisplayName(state, newOwnerId)} captured ${member.title} as the march fell.`,
      previousOwnerId === "player" ? "warn" : "good",
    );
    appendFallenLedger(faction, {
      memberId: member.id,
      title: member.title,
      roleId: member.roleId,
      disposition: "captured",
      capturedByFactionId: newOwnerId,
      at: state.meta.elapsed,
    });
    clearCommanderLinksFor(state, previousOwnerId, member.id);
    applySuccessionRipple(state, previousOwnerId, member);
    return;
  }

  member.status = "displaced";
  adjustLegitimacy(faction, -LEGITIMACY_LOSS_GOVERNOR_LOSS);
  recordConvictionEvent(state, previousOwnerId, "stewardship", -1, `Lost stewardship of a march`);
  pushMessage(
    state,
    `${member.title} fled the frontier as ${getFactionDisplayName(state, previousOwnerId)} lost the march.`,
    previousOwnerId === "player" ? "warn" : "info",
  );
  appendFallenLedger(faction, {
    memberId: member.id,
    title: member.title,
    roleId: member.roleId,
    disposition: "displaced",
    at: state.meta.elapsed,
  });
}

function restoreDisplacedMembers(state) {
  Object.values(state.factions).forEach((faction) => {
    if (faction.kind !== "kingdom" || !faction.dynasty) {
      return;
    }

    const heldTerritory = state.world.controlPoints.some((controlPoint) => controlPoint.ownerFactionId === faction.id);
    if (!heldTerritory) {
      return;
    }

    faction.dynasty.members.forEach((member) => {
      if (member.status === "displaced") {
        member.status = "active";
      }
    });
  });
}

function updateCaptiveRansomTrickle(state, dt) {
  Object.values(state.factions).forEach((faction) => {
    if (faction.kind !== "kingdom" || !faction.dynasty) {
      return;
    }

    const captives = faction.dynasty.captives ?? [];
    if (captives.length === 0) {
      return;
    }

    const trickle = captives.reduce((sum, captive) => {
      const renownComponent = (captive.renown ?? 6) * CAPTIVE_RENOWN_WEIGHT;
      return sum + CAPTIVE_INFLUENCE_TRICKLE + renownComponent;
    }, 0);

    faction.resources.influence += trickle * dt;
  });
}

function getCaptiveHoldingSettlement(state, captorFactionId) {
  const primarySeat = getPrimaryKeepSeat(state, captorFactionId);
  if (primarySeat) {
    return primarySeat;
  }
  let best = null;
  for (const settlement of state.world.settlements ?? []) {
    if (settlement.factionId !== captorFactionId) {
      continue;
    }
    if (!best || (settlement.fortificationTier ?? 0) > (best.fortificationTier ?? 0)) {
      best = settlement;
    }
  }
  return best;
}

function getResourceShortfallReason(resources, cost = {}) {
  const missing = Object.entries(cost)
    .filter(([resourceId, amount]) => (resources[resourceId] ?? 0) < amount)
    .map(([resourceId, amount]) => `${resourceId} ${amount}`);
  return missing.length > 0 ? `Need ${missing.join(", ")}` : null;
}

function getCapturedMemberRansomTerms(state, sourceFactionId, memberId) {
  const sourceFaction = getFaction(state, sourceFactionId);
  const member = getDynastyMember(sourceFaction, memberId);
  if (!sourceFaction?.dynasty || !member || member.status !== "captured" || !member.capturedByFactionId) {
    return { available: false, reason: "Member is not currently captive." };
  }
  if (getActiveDynastyOperationForMember(sourceFaction, memberId)) {
    return { available: false, reason: "A dynasty operation is already underway for this member." };
  }

  const captorFaction = getFaction(state, member.capturedByFactionId);
  const envoy = getDynastyMemberByRole(sourceFaction, ["diplomat", "merchant", "heir_designate", "head_of_bloodline"]);
  const merchant = getDynastyMemberByRole(sourceFaction, ["merchant", "governor", "head_of_bloodline"]);
  const captorEnvoy = getDynastyMemberByRole(captorFaction, ["diplomat", "head_of_bloodline", "heir_designate"]);
  const roleMultiplier = getCaptiveRoleOperationMultiplier(member.roleId);
  const envoyDiscount = 1 - getRoleRenownBonus(envoy, 90, 0.18);
  const merchantDiscount = 1 - getRoleRenownBonus(merchant, 110, 0.14);
  const captorPremium = 1 + getRoleRenownBonus(captorEnvoy, 130, 0.12);
  const cost = {
    gold: Math.max(35, Math.round((RANSOM_BASE_GOLD_COST + member.renown * 8) * roleMultiplier * merchantDiscount * captorPremium)),
    influence: Math.max(12, Math.round((RANSOM_BASE_INFLUENCE_COST + member.renown * 1.35) * roleMultiplier * envoyDiscount * captorPremium)),
  };
  const duration = Math.max(
    8,
    (RANSOM_BASE_DURATION_SECONDS + member.renown * RANSOM_DURATION_RENOWN_MULTIPLIER) * Math.max(0.74, envoyDiscount) * captorPremium,
  );
  const reason = getResourceShortfallReason(sourceFaction.resources, cost);
  return {
    available: !reason,
    reason,
    cost,
    duration,
    captorFactionId: member.capturedByFactionId,
    diplomatId: envoy?.id ?? null,
    merchantId: merchant?.id ?? null,
    projectedChance: 1,
  };
}

function getCapturedMemberRescueTerms(state, sourceFactionId, memberId) {
  const sourceFaction = getFaction(state, sourceFactionId);
  const member = getDynastyMember(sourceFaction, memberId);
  if (!sourceFaction?.dynasty || !member || member.status !== "captured" || !member.capturedByFactionId) {
    return { available: false, reason: "Member is not currently captive." };
  }
  if (getActiveDynastyOperationForMember(sourceFaction, memberId)) {
    return { available: false, reason: "A dynasty operation is already underway for this member." };
  }

  const captorFaction = getFaction(state, member.capturedByFactionId);
  const holdingSettlement = getCaptiveHoldingSettlement(state, member.capturedByFactionId);
  const wardProfile = holdingSettlement ? getFortificationFaithWardProfile(state, holdingSettlement) : DEFAULT_FORTIFICATION_WARD;
  const keepTier = holdingSettlement?.fortificationTier ?? 0;
  const spymaster = getDynastyMemberByRole(sourceFaction, ["spymaster", "diplomat", "merchant"]);
  const envoy = getDynastyMemberByRole(sourceFaction, ["diplomat", "merchant", "heir_designate"]);
  const commander = getDynastyMemberByRole(sourceFaction, ["commander", "heir_designate", "head_of_bloodline"]);
  const captorSpymaster = getDynastyMemberByRole(captorFaction, ["spymaster", "diplomat"]);
  const roleMultiplier = getCaptiveRoleOperationMultiplier(member.roleId);
  const power =
    12 +
    (spymaster?.renown ?? 0) * 0.95 +
    (envoy?.renown ?? 0) * 0.35 +
    (commander?.renown ?? 0) * 0.22;
  const difficulty =
    16 +
    member.renown * roleMultiplier * 0.65 +
    keepTier * 4.8 +
    (WARD_RESCUE_DIFFICULTY[wardProfile.id] ?? 0) +
    (captorSpymaster?.renown ?? 0) * 0.42;
  const projectedChance = Math.max(0.12, Math.min(0.88, 0.5 + ((power - difficulty) / 45)));
  const cost = {
    gold: Math.max(24, Math.round((RESCUE_BASE_GOLD_COST + member.renown * 4) * (1 + keepTier * 0.06))),
    influence: Math.max(16, Math.round((RESCUE_BASE_INFLUENCE_COST + member.renown * 1.4) * roleMultiplier)),
  };
  const duration = Math.max(
    10,
    RESCUE_BASE_DURATION_SECONDS +
      member.renown * RESCUE_DURATION_RENOWN_MULTIPLIER +
      keepTier * 3 -
      (spymaster?.renown ?? 0) * 0.14 -
      (envoy?.renown ?? 0) * 0.1,
  );
  const reason = getResourceShortfallReason(sourceFaction.resources, cost);
  return {
    available: !reason,
    reason,
    cost,
    duration,
    captorFactionId: member.capturedByFactionId,
    spymasterId: spymaster?.id ?? null,
    diplomatId: envoy?.id ?? null,
    holdingSettlementId: holdingSettlement?.id ?? null,
    holdingSettlementName: holdingSettlement?.name ?? getFactionDisplayName(state, member.capturedByFactionId),
    keepTier,
    wardId: wardProfile.id,
    wardLabel: wardProfile.label,
    successScore: power - difficulty,
    projectedChance,
  };
}

function getHeldCaptiveRansomTerms(state, captorFactionId, captiveId) {
  const captorFaction = getFaction(state, captorFactionId);
  const captive = findCaptiveRecordById(captorFaction, captiveId);
  if (!captorFaction?.dynasty || !captive) {
    return { available: false, reason: "Captive record not found." };
  }
  const terms = getCapturedMemberRansomTerms(state, captive.sourceFactionId, captive.memberId);
  return {
    ...terms,
    captiveId,
    memberId: captive.memberId,
    sourceFactionId: captive.sourceFactionId,
    sourceFactionName: captive.sourceFactionName ?? getFactionDisplayName(state, captive.sourceFactionId),
  };
}

function normalizeReleasedMemberRole(faction, member) {
  if (!faction?.dynasty || !member) {
    return;
  }
  const rulingHead = faction.dynasty.members.find((candidate) =>
    candidate.id !== member.id &&
    candidate.roleId === "head_of_bloodline" &&
    candidate.status === "ruling",
  );
  if (member.roleId === "head_of_bloodline" && rulingHead) {
    member.roleId = member.promotionHistory?.[0]?.fromRoleId ?? "governor";
  }

  const standingHeir = faction.dynasty.members.find((candidate) =>
    candidate.id !== member.id &&
    candidate.roleId === "heir_designate" &&
    isMemberAvailable(candidate),
  );
  if (member.roleId === "heir_designate" && standingHeir) {
    member.roleId = member.promotionHistory?.[0]?.fromRoleId ?? "diplomat";
  }
}

function releaseCapturedMember(state, sourceFactionId, memberId, releaseContext = {}) {
  const sourceFaction = getFaction(state, sourceFactionId);
  const member = getDynastyMember(sourceFaction, memberId);
  if (!sourceFaction?.dynasty || !member) {
    return { ok: false, reason: "Member not found." };
  }

  const captorFactionId =
    releaseContext.captorFactionId ??
    member.capturedByFactionId ??
    sourceFaction.dynasty.attachments.capturedMembers?.[member.id] ??
    null;
  const captorFaction = captorFactionId ? getFaction(state, captorFactionId) : null;
  removeCaptiveRecord(captorFaction, member.id, releaseContext.captiveId ?? null);

  if (sourceFaction.dynasty.attachments?.capturedMembers) {
    delete sourceFaction.dynasty.attachments.capturedMembers[member.id];
  }

  member.capturedByFactionId = null;
  member.fallenAt = null;
  normalizeReleasedMemberRole(sourceFaction, member);
  member.status = member.roleId === "head_of_bloodline" ? "ruling" : "active";

  if (member.roleId === "heir_designate") {
    sourceFaction.dynasty.attachments.heirMemberId = member.id;
  } else if (sourceFaction.dynasty.attachments.heirMemberId === member.id) {
    sourceFaction.dynasty.attachments.heirMemberId = null;
    backfillHeir(state, sourceFactionId, null);
  }

  return {
    ok: true,
    member,
    captorFactionId,
  };
}

function ensureMutualHostility(state, sourceFactionId, targetFactionId) {
  const sourceFaction = getFaction(state, sourceFactionId);
  const targetFaction = getFaction(state, targetFactionId);
  if (!sourceFaction || !targetFaction || sourceFactionId === targetFactionId) {
    return;
  }
  sourceFaction.hostileTo = sourceFaction.hostileTo ?? [];
  targetFaction.hostileTo = targetFaction.hostileTo ?? [];
  if (!sourceFaction.hostileTo.includes(targetFactionId)) {
    sourceFaction.hostileTo.push(targetFactionId);
  }
  if (!targetFaction.hostileTo.includes(sourceFactionId)) {
    targetFaction.hostileTo.push(sourceFactionId);
  }
}

// Session 90: non-aggression pact diplomacy. Allows a kingdom to propose
// a pact that removes mutual hostility, easing the governance alliance-threshold
// acceptance-drag. Canon: sovereignty requires statecraft, not just conquest.
const NON_AGGRESSION_PACT_INFLUENCE_COST = 50;
const NON_AGGRESSION_PACT_GOLD_COST = 80;
const NON_AGGRESSION_PACT_MINIMUM_DURATION_IN_WORLD_DAYS = 180;
const NON_AGGRESSION_PACT_BREAK_LEGITIMACY_COST = 8;

function removeMutualHostility(state, factionIdA, factionIdB) {
  const factionA = getFaction(state, factionIdA);
  const factionB = getFaction(state, factionIdB);
  if (!factionA || !factionB) return;
  factionA.hostileTo = (factionA.hostileTo ?? []).filter((id) => id !== factionIdB);
  factionB.hostileTo = (factionB.hostileTo ?? []).filter((id) => id !== factionIdA);
}

function getActivePact(faction, targetFactionId) {
  return (faction?.diplomacy?.nonAggressionPacts ?? []).find(
    (pact) => pact.targetFactionId === targetFactionId && !pact.brokenAt,
  ) ?? null;
}

function getActivePactWithFaction(state, factionId, targetFactionId) {
  const faction = getFaction(state, factionId);
  return getActivePact(faction, targetFactionId);
}

export function getNonAggressionPactTerms(state, factionId, targetFactionId) {
  const faction = getFaction(state, factionId);
  const targetFaction = getFaction(state, targetFactionId);
  if (!faction || !targetFaction || faction.kind !== "kingdom" || targetFaction.kind !== "kingdom") {
    return { available: false, reason: "Both parties must be kingdoms." };
  }
  if (factionId === targetFactionId) {
    return { available: false, reason: "Cannot form a pact with yourself." };
  }
  if (!areHostile(state, factionId, targetFactionId)) {
    return { available: false, reason: "Already at peace." };
  }
  if (getActivePact(faction, targetFactionId)) {
    return { available: false, reason: "Pact already active." };
  }
  const activeHolyWars = getFaithHolyWarsState(faction)
    .filter((hw) => hw.targetFactionId === targetFactionId && (hw.expiresAt ?? 0) > state.meta.elapsed);
  const incomingHolyWars = getIncomingHolyWars(state, factionId, state.meta.elapsed)
    .filter((hw) => hw.factionId === targetFactionId || hw.targetFactionId === factionId);
  if (activeHolyWars.length > 0 || incomingHolyWars.length > 0) {
    return { available: false, reason: "Cannot form a pact during an active holy war." };
  }
  const cost = { influence: NON_AGGRESSION_PACT_INFLUENCE_COST, gold: NON_AGGRESSION_PACT_GOLD_COST };
  if (!canAffordCost(faction.resources, cost)) {
    return { available: false, reason: `Requires ${cost.influence} influence and ${cost.gold} gold.`, cost };
  }
  return {
    available: true,
    cost,
    minimumDurationInWorldDays: NON_AGGRESSION_PACT_MINIMUM_DURATION_IN_WORLD_DAYS,
    targetFactionId,
    targetFactionName: getFactionDisplayName(state, targetFactionId),
  };
}

export function proposeNonAggressionPact(state, factionId, targetFactionId) {
  const terms = getNonAggressionPactTerms(state, factionId, targetFactionId);
  if (!terms.available) {
    return { ok: false, reason: terms.reason };
  }
  const faction = getFaction(state, factionId);
  const targetFaction = getFaction(state, targetFactionId);
  applyCost(faction.resources, terms.cost);
  removeMutualHostility(state, factionId, targetFactionId);
  const currentInWorldDays = state.dualClock?.inWorldDays ?? 0;
  const pactRecord = {
    id: `nap-${factionId}-${targetFactionId}-${Math.round(state.meta.elapsed * 1000)}`,
    factionId,
    targetFactionId,
    startedAt: state.meta.elapsed,
    startedAtInWorldDays: currentInWorldDays,
    minimumExpiresAtInWorldDays: currentInWorldDays + NON_AGGRESSION_PACT_MINIMUM_DURATION_IN_WORLD_DAYS,
    brokenAt: null,
    brokenByFactionId: null,
  };
  faction.diplomacy = faction.diplomacy ?? {};
  faction.diplomacy.nonAggressionPacts = faction.diplomacy.nonAggressionPacts ?? [];
  faction.diplomacy.nonAggressionPacts.push(pactRecord);
  targetFaction.diplomacy = targetFaction.diplomacy ?? {};
  targetFaction.diplomacy.nonAggressionPacts = targetFaction.diplomacy.nonAggressionPacts ?? [];
  targetFaction.diplomacy.nonAggressionPacts.push({
    ...pactRecord,
    id: `nap-${targetFactionId}-${factionId}-${Math.round(state.meta.elapsed * 1000)}`,
    factionId: targetFactionId,
    targetFactionId: factionId,
  });
  pushMessage(
    state,
    `${getFactionDisplayName(state, factionId)} and ${getFactionDisplayName(state, targetFactionId)} enter a non-aggression pact. Hostility ceases for at least ${NON_AGGRESSION_PACT_MINIMUM_DURATION_IN_WORLD_DAYS} in-world days.`,
    factionId === "player" ? "good" : "info",
  );
  return { ok: true, pactId: pactRecord.id };
}

export function breakNonAggressionPact(state, factionId, targetFactionId) {
  const faction = getFaction(state, factionId);
  const targetFaction = getFaction(state, targetFactionId);
  if (!faction || !targetFaction) {
    return { ok: false, reason: "Invalid factions." };
  }
  const pact = getActivePact(faction, targetFactionId);
  if (!pact) {
    return { ok: false, reason: "No active pact to break." };
  }
  const currentInWorldDays = state.dualClock?.inWorldDays ?? 0;
  const minimumExpiry = pact.minimumExpiresAtInWorldDays ?? 0;
  const earlyBreak = currentInWorldDays < minimumExpiry;
  pact.brokenAt = state.meta.elapsed;
  pact.brokenByFactionId = factionId;
  const targetPact = getActivePact(targetFaction, factionId);
  if (targetPact) {
    targetPact.brokenAt = state.meta.elapsed;
    targetPact.brokenByFactionId = factionId;
  }
  ensureMutualHostility(state, factionId, targetFactionId);
  if (faction.dynasty) {
    faction.dynasty.legitimacy = Math.max(0, (faction.dynasty.legitimacy ?? 0) - NON_AGGRESSION_PACT_BREAK_LEGITIMACY_COST);
  }
  if (faction.conviction) {
    faction.conviction.score = clamp((faction.conviction.score ?? 0) - 2, -100, 100);
  }
  pushMessage(
    state,
    `${getFactionDisplayName(state, factionId)} breaks the non-aggression pact with ${getFactionDisplayName(state, targetFactionId)}.${earlyBreak ? " The early breach damages legitimacy and conviction." : " Hostility resumes."}`,
    factionId === "player" ? "warn" : "info",
  );
  return { ok: true, earlyBreak };
}

function getDynastyMemberLocationProfile(state, factionId, memberId) {
  const faction = getFaction(state, factionId);
  const member = getDynastyMember(faction, memberId);
  if (!faction?.dynasty || !member) {
    return {
      label: "location unknown",
      exposureBonus: 0,
      keepTier: 0,
      wardProfile: DEFAULT_FORTIFICATION_WARD,
    };
  }

  const commanderUnit = faction.dynasty.attachments.commanderMemberId === member.id && faction.dynasty.attachments.commanderUnitId
    ? getEntity(state, "unit", faction.dynasty.attachments.commanderUnitId)
    : null;
  if (commanderUnit?.health > 0) {
    return {
      label: "commanding in the field",
      exposureBonus: 14,
      keepTier: 0,
      wardProfile: DEFAULT_FORTIFICATION_WARD,
    };
  }

  const governorAssignment = (faction.dynasty.attachments.governorAssignments ?? []).find(
    (assignment) => assignment.memberId === member.id,
  );
  if (governorAssignment) {
    const anchor = getGovernorAssignmentAnchor(state, governorAssignment);
    const settlementAnchor = governorAssignment.anchorType === "settlement"
      ? getSettlementById(state, governorAssignment.anchorId)
      : null;
    return {
      label: anchor?.name ? `governing ${anchor.name}` : "governing a march",
      exposureBonus: governorAssignment.anchorType === "controlPoint" ? 6 : 4,
      keepTier: settlementAnchor?.fortificationTier ?? anchor?.fortificationTier ?? 0,
      wardProfile: settlementAnchor ? getFortificationFaithWardProfile(state, settlementAnchor) : DEFAULT_FORTIFICATION_WARD,
    };
  }

  const primarySeat = getPrimaryKeepSeat(state, factionId);
  const wardProfile = primarySeat ? getFortificationFaithWardProfile(state, primarySeat) : DEFAULT_FORTIFICATION_WARD;
  const bloodlineProtectionActive = Boolean(
    primarySeat &&
    (ensureImminentEngagementState(primarySeat).bloodlineProtectionUntil ?? 0) > state.meta.elapsed,
  );
  const seatName = primarySeat?.name ?? `${getFactionDisplayName(state, factionId)} court`;
  if (member.roleId === "spymaster") {
    return {
      label: bloodlineProtectionActive ? `moving through ${seatName} under emergency guard` : `moving through ${seatName}`,
      exposureBonus: 2 - (bloodlineProtectionActive ? 6 : 0),
      keepTier: (primarySeat?.fortificationTier ?? 0) + (bloodlineProtectionActive ? 1 : 0),
      wardProfile,
      bloodlineProtectionBonus: bloodlineProtectionActive ? 10 : 0,
    };
  }
  return {
    label: bloodlineProtectionActive ? `inside ${seatName} under emergency bloodline guard` : `inside ${seatName}`,
    exposureBonus: (member.roleId === "head_of_bloodline" ? -6 : -2) - (bloodlineProtectionActive ? 8 : 0),
    keepTier: (primarySeat?.fortificationTier ?? 0) + (bloodlineProtectionActive ? 1 : 0),
    wardProfile,
    bloodlineProtectionBonus: bloodlineProtectionActive ? 10 : 0,
  };
}

function createDynastyIntelligenceReport(state, sourceFactionId, targetFactionId, options = {}) {
  const targetFaction = getFaction(state, targetFactionId);
  if (!targetFaction?.dynasty) {
    return null;
  }
  const activeMembers = targetFaction.dynasty.members
    .filter((member) => isMemberAvailable(member))
    .sort((left, right) => {
      const priority = (member) => (
        member.roleId === "head_of_bloodline" ? 0
          : member.roleId === "heir_designate" ? 1
            : member.roleId === "commander" ? 2
              : member.roleId === "spymaster" ? 3
                : member.roleId === "governor" ? 4
                  : 5
      );
      return priority(left) - priority(right);
    })
    .slice(0, 6)
    .map((member) => {
      const locationProfile = getDynastyMemberLocationProfile(state, targetFactionId, member.id);
      return {
        memberId: member.id,
        title: member.title,
        roleId: member.roleId,
        status: member.status,
        renown: member.renown ?? 0,
        locationLabel: locationProfile.label,
        exposureBonus: locationProfile.exposureBonus,
      };
    });

  return {
    id: createEntityId(state, "dynastyIntel"),
    sourceFactionId,
    targetFactionId,
    sourceType: options.sourceType ?? "espionage",
    reportLabel: options.reportLabel ?? ((options.sourceType ?? "espionage") === "counter_intelligence"
      ? "Counter-intelligence dossier"
      : "Court report"),
    interceptType: options.interceptType ?? null,
    interceptCount: options.interceptCount ?? 0,
    createdAt: state.meta.elapsed,
    expiresAt: state.meta.elapsed + (options.duration ?? INTELLIGENCE_REPORT_DURATION_SECONDS),
    targetLegitimacy: Math.round(targetFaction.dynasty.legitimacy ?? 0),
    targetActiveOperations: targetFaction.dynasty.operations?.active?.length ?? 0,
    targetCaptiveCount: targetFaction.dynasty.captives?.length ?? 0,
    targetLesserHouseCount: targetFaction.dynasty.lesserHouses?.length ?? 0,
    members: activeMembers,
  };
}

function storeDynastyIntelligenceReport(faction, report) {
  if (!faction?.dynasty || !report) {
    return null;
  }
  const reports = getDynastyIntelligenceReports(faction).filter((candidate) =>
    !(
      candidate.targetFactionId === report.targetFactionId &&
      (candidate.sourceType ?? "espionage") === (report.sourceType ?? "espionage")
    ));
  reports.unshift(report);
  faction.dynasty.intelligenceReports = reports.slice(0, 6);
  return report;
}

function applyAssassinationEffect(state, sourceFactionId, targetFactionId, targetMemberId) {
  const targetFaction = getFaction(state, targetFactionId);
  const member = getDynastyMember(targetFaction, targetMemberId);
  if (!targetFaction?.dynasty || !member || !isMemberAvailable(member)) {
    return { ok: false, reason: "Target member is no longer available." };
  }

  member.status = "fallen";
  member.fallenAt = state.meta.elapsed;
  member.capturedByFactionId = null;
  if (member.roleId === "commander") {
    adjustLegitimacy(targetFaction, -LEGITIMACY_LOSS_COMMANDER_KILL);
  }
  if (member.roleId === "governor") {
    adjustLegitimacy(targetFaction, -LEGITIMACY_LOSS_GOVERNOR_LOSS);
    recordConvictionEvent(state, targetFactionId, "stewardship", -1, `Lost ${member.title} to assassination`);
  }
  if (sourceFactionId) {
    recordConvictionEvent(state, sourceFactionId, "ruthlessness", 2, `Assassinated ${member.title}`);
  }
  appendFallenLedger(targetFaction, {
    memberId: member.id,
    title: member.title,
    roleId: member.roleId,
    disposition: "fallen",
    attackerFactionId: sourceFactionId ?? null,
    cause: "assassination",
    at: state.meta.elapsed,
  });
  clearCommanderLinksFor(state, targetFactionId, member.id);
  clearGovernorLinksFor(state, targetFactionId, member.id);
  applySuccessionRipple(state, targetFactionId, member);
  return { ok: true, member };
}

function finalizeDynastyOperation(state, sourceFactionId, operation, outcome) {
  const sourceFaction = getFaction(state, sourceFactionId);
  const operations = getDynastyOperationsState(sourceFaction);
  if (!operations) {
    return;
  }
  operations.active = operations.active.filter((candidate) => candidate.id !== operation.id);

  const entry = {
    ...operation,
    status: outcome.status,
    completedAt: state.meta.elapsed,
    outcomeText: outcome.text,
    perspective: "source",
  };
  pushDynastyOperationHistory(sourceFaction, entry);

  if (operation.targetFactionId) {
    const targetFaction = getFaction(state, operation.targetFactionId);
    if (targetFaction?.dynasty) {
      pushDynastyOperationHistory(targetFaction, {
        ...entry,
        perspective: "target",
      });
    }
  }
}

function tickDynastyOperations(state) {
  Object.values(state.factions).forEach((faction) => {
    const operations = getDynastyOperationsState(faction);
    if (!operations || operations.active.length === 0) {
      return;
    }

    const ready = operations.active.filter((operation) => (operation.resolveAt ?? 0) <= state.meta.elapsed);
    ready.forEach((operation) => {
      // Sabotage operations resolve against a target building, not a captive member.
      // Dispatch here before the captivity-state validation below.
      if (operation.type === "sabotage") {
        const targetBuilding = state.buildings.find((b) => b.id === operation.targetBuildingId);
        if (!targetBuilding || targetBuilding.health <= 0) {
          finalizeDynastyOperation(state, faction.id, operation, {
            status: "void",
            text: `Sabotage target was destroyed before the operation resolved.`,
          });
          return;
        }
        const success = (operation.successScore ?? -999) >= 0;
        if (success) {
          applySabotageEffect(state, operation);
          const convictionBucket =
            operation.subtype === "well_poisoning" || operation.subtype === "supply_poisoning"
              ? "desecration"
              : "ruthlessness";
          recordConvictionEvent(
            state,
            faction.id,
            convictionBucket,
            2,
            `Ran ${operation.subtype} against ${getFactionDisplayName(state, operation.targetFactionId)}`,
          );
          pushMessage(
            state,
            `${getFactionDisplayName(state, faction.id)} sabotage succeeds: ${operation.subtype.replace(/_/g, " ")} against ${getFactionDisplayName(state, operation.targetFactionId)}.`,
            faction.id === "player" ? "good" : operation.targetFactionId === "player" ? "warn" : "info",
          );
          finalizeDynastyOperation(state, faction.id, operation, {
            status: "completed",
            text: `${operation.subtype.replace(/_/g, " ")} succeeded against ${getFactionDisplayName(state, operation.targetFactionId)}.`,
          });
        } else {
          recordConvictionEvent(
            state,
            operation.targetFactionId,
            "stewardship",
            1,
            `Detected sabotage attempt from ${getFactionDisplayName(state, faction.id)}`,
          );
          pushMessage(
            state,
            `A sabotage attempt against ${getFactionDisplayName(state, operation.targetFactionId)} was detected and foiled.`,
            faction.id === "player" ? "warn" : operation.targetFactionId === "player" ? "good" : "info",
          );
          finalizeDynastyOperation(state, faction.id, operation, {
            status: "failed",
            text: `${operation.subtype.replace(/_/g, " ")} attempt was foiled.`,
          });
        }
        return;
      }

      if (operation.type === "missionary") {
        const targetFaction = getFaction(state, operation.targetFactionId);
        if (!targetFaction?.faith || !targetFaction?.dynasty) {
          finalizeDynastyOperation(state, faction.id, operation, {
            status: "void",
            text: "Target court no longer exists for missionary resolution.",
          });
          return;
        }

        const success = (operation.successScore ?? -999) >= 0;
        if (success) {
          const result = applyMissionaryEffect(state, operation);
          if (!result.ok) {
            finalizeDynastyOperation(state, faction.id, operation, {
              status: "void",
              text: result.reason ?? "Missionary target was lost before resolution.",
            });
            return;
          }
          recordConvictionEvent(
            state,
            faction.id,
            faction.faith?.doctrinePath === "dark" ? "desecration" : "oathkeeping",
            1,
            `Sent missionaries into ${getFactionDisplayName(state, operation.targetFactionId)}`,
          );
          if ((result.exposureBefore ?? 0) < FAITH_EXPOSURE_THRESHOLD && (result.exposureAfter ?? 0) >= FAITH_EXPOSURE_THRESHOLD) {
            pushMessage(
              state,
              `${getFactionDisplayName(state, operation.targetFactionId)} now has enough exposure to embrace ${operation.sourceFaithName}.`,
              operation.targetFactionId === "player" ? "warn" : faction.id === "player" ? "good" : "info",
            );
          }
          pushMessage(
            state,
            `${operation.sourceFaithName} missionaries breach ${getFactionDisplayName(state, operation.targetFactionId)}${result.pressurePointName ? ` through pressure at ${result.pressurePointName}` : ""}.`,
            faction.id === "player" ? "good" : operation.targetFactionId === "player" ? "warn" : "info",
          );
          finalizeDynastyOperation(state, faction.id, operation, {
            status: "completed",
            text: `Missionaries spread ${operation.sourceFaithName} inside ${getFactionDisplayName(state, operation.targetFactionId)}.`,
          });
        } else {
          const targetSeat = getPrimaryKeepSeat(state, operation.targetFactionId);
          const wardProfile = targetSeat ? getFortificationFaithWardProfile(state, targetSeat) : DEFAULT_FORTIFICATION_WARD;
          if (targetFaction.faith?.selectedFaithId) {
            targetFaction.faith.intensity = (targetFaction.faith.intensity ?? 0) + 2;
            syncFaithIntensityState(targetFaction);
          }
          if (wardProfile.id !== "none" || targetFaction.faith?.selectedFaithId === "the_order") {
            ensureMutualHostility(state, faction.id, operation.targetFactionId);
          }
          recordConvictionEvent(
            state,
            operation.targetFactionId,
            "stewardship",
            1,
            `Repelled missionaries from ${getFactionDisplayName(state, faction.id)}`,
          );
          pushMessage(
            state,
            `${getFactionDisplayName(state, operation.targetFactionId)} exposed and repelled missionaries sent by ${getFactionDisplayName(state, faction.id)}.`,
            faction.id === "player" ? "warn" : operation.targetFactionId === "player" ? "good" : "info",
          );
          finalizeDynastyOperation(state, faction.id, operation, {
            status: "failed",
            text: `Missionary work was repelled by ${getFactionDisplayName(state, operation.targetFactionId)}.`,
          });
        }
        return;
      }

      if (operation.type === "holy_war") {
        const targetFaction = getFaction(state, operation.targetFactionId);
        if (!targetFaction?.faith || !targetFaction?.dynasty) {
          finalizeDynastyOperation(state, faction.id, operation, {
            status: "void",
            text: "Target court no longer exists for holy war declaration.",
          });
          return;
        }
        ensureMutualHostility(state, faction.id, operation.targetFactionId);
        const entry = createHolyWarEntry(state, faction.id, operation.targetFactionId, operation);
        if (!entry) {
          finalizeDynastyOperation(state, faction.id, operation, {
            status: "void",
            text: "Faith commitment was lost before holy war declaration could land.",
          });
          return;
        }
        const wars = getFaithHolyWarsState(faction).filter((candidate) => candidate.targetFactionId !== operation.targetFactionId);
        wars.unshift(entry);
        faction.faith.activeHolyWars = wars.slice(0, 6);
        faction.faith.intensity = (faction.faith.intensity ?? 0) + Math.max(3, (entry.intensityPulse ?? 1) * 2);
        syncFaithIntensityState(faction);
        const pressurePoint = getLowestLoyaltyControlPoint(state, operation.targetFactionId);
        if (pressurePoint) {
          pressurePoint.loyalty = clamp(pressurePoint.loyalty - Math.max(2, entry.loyaltyPulse ?? 1), 0, 100);
        } else {
          adjustLegitimacy(targetFaction, -3);
        }
        recordConvictionEvent(
          state,
          faction.id,
          faction.faith?.doctrinePath === "dark" ? "desecration" : "oathkeeping",
          2,
          `Declared holy war on ${getFactionDisplayName(state, operation.targetFactionId)}`,
        );
        recordConvictionEvent(
          state,
          operation.targetFactionId,
          "stewardship",
          -1,
          `Came under holy war declaration from ${getFactionDisplayName(state, faction.id)}`,
        );
        declareInWorldTime(
          state,
          21,
          `Holy war declared: ${getFactionDisplayName(state, faction.id)} against ${getFactionDisplayName(state, operation.targetFactionId)}`,
        );
        pushMessage(
          state,
          `${getFactionDisplayName(state, faction.id)} declares holy war on ${getFactionDisplayName(state, operation.targetFactionId)}.`,
          faction.id === "player" ? "warn" : operation.targetFactionId === "player" ? "warn" : "info",
        );
        finalizeDynastyOperation(state, faction.id, operation, {
          status: "completed",
          text: `Holy war was declared against ${getFactionDisplayName(state, operation.targetFactionId)}.`,
        });
        return;
      }

      if (operation.type === "counter_intelligence") {
        const watch = createCounterIntelligenceWatch(state, faction.id, operation);
        if (!watch) {
          finalizeDynastyOperation(state, faction.id, operation, {
            status: "void",
            text: "Counter-intelligence watch could not be established.",
          });
          return;
        }
        const watches = getDynastyCounterIntelligenceState(faction)
          .filter((entry) => (entry.expiresAt ?? 0) > state.meta.elapsed);
        watches.unshift(watch);
        faction.dynasty.counterIntelligence = watches.slice(0, 2);
        recordConvictionEvent(
          state,
          faction.id,
          "stewardship",
          1,
          "Raised counter-intelligence watch around the bloodline court",
        );
        pushMessage(
          state,
          `${getFactionDisplayName(state, faction.id)} raises a counter-intelligence watch around the bloodline court.`,
          faction.id === "player" ? "good" : "info",
        );
        finalizeDynastyOperation(state, faction.id, operation, {
          status: "completed",
          text: "Counter-intelligence watch established around the bloodline court.",
        });
        return;
      }

      if (operation.type === "espionage") {
        const targetFaction = getFaction(state, operation.targetFactionId);
        if (!targetFaction?.dynasty) {
          finalizeDynastyOperation(state, faction.id, operation, {
            status: "void",
            text: "Target court no longer exists for espionage resolution.",
          });
          return;
        }

        const contest = getEspionageContest(state, faction.id, operation.targetFactionId);
        const success = contest.successScore >= 0;
        if (success) {
          const report = createDynastyIntelligenceReport(state, faction.id, operation.targetFactionId);
          if (!report) {
            finalizeDynastyOperation(state, faction.id, operation, {
              status: "void",
              text: "Target court no longer exposes usable intelligence.",
            });
            return;
          }
          storeDynastyIntelligenceReport(faction, report);
          recordConvictionEvent(
            state,
            faction.id,
            "stewardship",
            1,
            `Built an espionage report on ${getFactionDisplayName(state, operation.targetFactionId)}`,
          );
          pushMessage(
            state,
            `${getFactionDisplayName(state, faction.id)} establishes a covert intelligence report inside ${getFactionDisplayName(state, operation.targetFactionId)}.`,
            faction.id === "player" ? "good" : operation.targetFactionId === "player" ? "warn" : "info",
          );
          finalizeDynastyOperation(state, faction.id, operation, {
            status: "completed",
            text: `Espionage succeeded against ${getFactionDisplayName(state, operation.targetFactionId)}.`,
          });
        } else {
          const interception = recordCounterIntelligenceInterception(
            state,
            operation.targetFactionId,
            faction.id,
            "espionage",
          );
          ensureMutualHostility(state, faction.id, operation.targetFactionId);
          recordConvictionEvent(
            state,
            operation.targetFactionId,
            "stewardship",
            1,
            `Exposed espionage agents from ${getFactionDisplayName(state, faction.id)}`,
          );
          pushMessage(
            state,
            interception
              ? `${getFactionDisplayName(state, operation.targetFactionId)} counter-intelligence exposed an espionage web traced back to ${getFactionDisplayName(state, faction.id)}.`
              : `${getFactionDisplayName(state, operation.targetFactionId)} exposed an espionage web traced back to ${getFactionDisplayName(state, faction.id)}.`,
            faction.id === "player" ? "warn" : operation.targetFactionId === "player" ? "good" : "info",
          );
          finalizeDynastyOperation(state, faction.id, operation, {
            status: "failed",
            text: interception
              ? `Espionage was broken by ${getFactionDisplayName(state, operation.targetFactionId)} counter-intelligence.`
              : `Espionage was exposed by ${getFactionDisplayName(state, operation.targetFactionId)}.`,
          });
        }
        return;
      }

      if (operation.type === "assassination") {
        const targetFaction = getFaction(state, operation.targetFactionId);
        const targetMember = getDynastyMember(targetFaction, operation.targetMemberId ?? operation.memberId);
        if (!targetFaction?.dynasty || !targetMember || !isMemberAvailable(targetMember)) {
          finalizeDynastyOperation(state, faction.id, operation, {
            status: "void",
            text: `${operation.memberTitle} was no longer available when the assassination cell moved.`,
          });
          return;
        }

        ensureMutualHostility(state, faction.id, operation.targetFactionId);
        const contest = getAssassinationContest(
          state,
          faction.id,
          operation.targetFactionId,
          operation.targetMemberId ?? operation.memberId,
        );
        const success = contest.successScore >= 0;
        if (success) {
          const result = applyAssassinationEffect(
            state,
            faction.id,
            operation.targetFactionId,
            operation.targetMemberId ?? operation.memberId,
          );
          if (!result.ok) {
            finalizeDynastyOperation(state, faction.id, operation, {
              status: "void",
              text: result.reason ?? "Assassination target was lost before resolution.",
            });
            return;
          }
          tickMarriageDissolutionFromDeath(state);
          pushMessage(
            state,
            `${operation.memberTitle} of ${getFactionDisplayName(state, operation.targetFactionId)} was assassinated by agents of ${getFactionDisplayName(state, faction.id)}.`,
            faction.id === "player" ? "good" : operation.targetFactionId === "player" ? "warn" : "info",
          );
          finalizeDynastyOperation(state, faction.id, operation, {
            status: "completed",
            text: `${operation.memberTitle} was assassinated inside ${getFactionDisplayName(state, operation.targetFactionId)}.`,
          });
        } else {
          const interception = recordCounterIntelligenceInterception(
            state,
            operation.targetFactionId,
            faction.id,
            "assassination",
            operation.targetMemberId ?? operation.memberId,
          );
          const targetResources = targetFaction.resources ?? {};
          targetResources.influence = (targetResources.influence ?? 0) + Math.max(8, Math.round((operation.escrowCost?.influence ?? 0) * 0.3));
          recordConvictionEvent(
            state,
            operation.targetFactionId,
            "stewardship",
            1,
            `Foiled assassination against ${operation.memberTitle}`,
          );
          pushMessage(
            state,
            interception
              ? `${getFactionDisplayName(state, operation.targetFactionId)} counter-intelligence intercepted an assassination cell targeting ${operation.memberTitle}.`
              : `${getFactionDisplayName(state, operation.targetFactionId)} uncovered and broke an assassination cell targeting ${operation.memberTitle}.`,
            faction.id === "player" ? "warn" : operation.targetFactionId === "player" ? "good" : "info",
          );
          finalizeDynastyOperation(state, faction.id, operation, {
            status: "failed",
            text: interception
              ? `Counter-intelligence broke the assassination against ${operation.memberTitle}.`
              : `Assassination failed against ${operation.memberTitle}.`,
          });
        }
        return;
      }

      const member = getDynastyMember(faction, operation.memberId);
      if (!member || member.status !== "captured" || member.capturedByFactionId !== operation.targetFactionId) {
        finalizeDynastyOperation(state, faction.id, operation, {
          status: "void",
          text: `${operation.memberTitle} is no longer in the expected captivity state.`,
        });
        return;
      }

      if (operation.type === "ransom") {
        const captorFaction = getFaction(state, operation.targetFactionId);
        grantResources(captorFaction.resources, operation.escrowCost ?? {});
        const release = releaseCapturedMember(state, faction.id, operation.memberId, {
          captorFactionId: operation.targetFactionId,
        });
        if (release.ok) {
          adjustLegitimacy(faction, LEGITIMACY_RECOVERY_ON_RANSOM);
          recordConvictionEvent(state, faction.id, "oathkeeping", 1, `Ransomed ${operation.memberTitle} home`);
          recordConvictionEvent(state, operation.targetFactionId, "stewardship", 1, `Extracted ransom for ${operation.memberTitle}`);
          pushMessage(
            state,
            `${operation.memberTitle} returns to ${getFactionDisplayName(state, faction.id)} under ransom terms.`,
            faction.id === "player" ? "good" : operation.targetFactionId === "player" ? "info" : "info",
          );
          finalizeDynastyOperation(state, faction.id, operation, {
            status: "completed",
            text: `${operation.memberTitle} returned through negotiated ransom.`,
          });
        }
        return;
      }

      if (operation.type === "rescue") {
        const success = (operation.successScore ?? -999) >= 0;
        if (success) {
          const release = releaseCapturedMember(state, faction.id, operation.memberId, {
            captorFactionId: operation.targetFactionId,
          });
          if (release.ok) {
            adjustLegitimacy(faction, LEGITIMACY_RECOVERY_ON_RESCUE);
            recordConvictionEvent(state, faction.id, "stewardship", 2, `Recovered ${operation.memberTitle} by covert extraction`);
            pushMessage(
              state,
              `${operation.memberTitle} has been extracted from ${getFactionDisplayName(state, operation.targetFactionId)} custody.`,
              faction.id === "player" ? "good" : operation.targetFactionId === "player" ? "warn" : "info",
            );
            finalizeDynastyOperation(state, faction.id, operation, {
              status: "completed",
              text: `${operation.memberTitle} was recovered by covert rescue.`,
            });
          }
          return;
        }

        const captorFaction = getFaction(state, operation.targetFactionId);
        captorFaction.resources.influence += Math.max(6, Math.round((operation.escrowCost?.influence ?? 0) * 0.2));
        recordConvictionEvent(state, operation.targetFactionId, "stewardship", 1, `Foiled rescue attempt against ${operation.memberTitle}`);
        pushMessage(
          state,
          `A rescue attempt for ${operation.memberTitle} failed inside ${getFactionDisplayName(state, operation.targetFactionId)} territory.`,
          faction.id === "player" ? "warn" : operation.targetFactionId === "player" ? "good" : "info",
        );
        finalizeDynastyOperation(state, faction.id, operation, {
          status: "failed",
          text: `${operation.memberTitle} remained captive after a failed rescue attempt.`,
        });
      }
    });
  });
}

function clearCommanderAssignment(state, factionId) {
  state.units.forEach((unit) => {
    if (unit.factionId === factionId && unit.commanderMemberId) {
      unit.commanderMemberId = null;
    }
  });

  const faction = getFaction(state, factionId);
  if (faction?.dynasty?.attachments) {
    faction.dynasty.attachments.commanderMemberId = null;
    faction.dynasty.attachments.commanderUnitId = null;
  }
}

function syncGovernorAssignments(state, factionId) {
  const faction = getFaction(state, factionId);
  if (!faction?.dynasty?.attachments) {
    return;
  }

  faction.dynasty.attachments.governorAssignments = [];
  state.world.controlPoints.forEach((controlPoint) => {
    if (controlPoint.ownerFactionId === factionId) {
      controlPoint.governorMemberId = null;
    }
  });
  (state.world.settlements ?? []).forEach((settlement) => {
    if (settlement.factionId === factionId) {
      settlement.governorMemberId = null;
    }
  });

  const governanceSeatMembers = getGovernanceSeatMembers(faction);
  if (governanceSeatMembers.length === 0) {
    return;
  }

  const ownedPoints = state.world.controlPoints.filter((controlPoint) => controlPoint.ownerFactionId === factionId);
  const candidates = [];
  ownedPoints.forEach((controlPoint) => {
    const specializationId = getGovernorSpecializationIdForSettlementClass(controlPoint.settlementClass);
    const urgency = controlPoint.controlState !== "stabilized"
      ? 120 - controlPoint.loyalty
      : 74 - controlPoint.loyalty * 0.35;
    const classBias = specializationId === "border" ? 8 : specializationId === "city" ? 6 : 4;
    candidates.push({
      anchorType: "controlPoint",
      anchorId: controlPoint.id,
      specializationId,
      priority: urgency + classBias,
      loyalty: controlPoint.loyalty,
    });
  });

  (state.world.settlements ?? [])
    .filter((settlement) => settlement.factionId === factionId)
    .forEach((settlement) => {
      const specializationId = getGovernorSpecializationIdForSettlementClass(settlement.settlementClass);
      const defenseState = getSettlementDefenseState(state, settlement);
      const noTerritoryBonus = ownedPoints.length === 0 ? 40 : 0;
      const keepThreatBonus = defenseState.enemyCombatants.length > 0 ? 36 : 0;
      const fortPriority = 40 +
        (settlement.fortificationTier ?? 0) * 6 +
        (settlement.settlementClass === "primary_dynastic_keep" ? 10 : 0) +
        noTerritoryBonus +
        keepThreatBonus;
      candidates.push({
        anchorType: "settlement",
        anchorId: settlement.id,
        specializationId,
        priority: fortPriority,
        loyalty: 100,
      });
    });

  const availableMembers = [...governanceSeatMembers];
  candidates
    .sort((left, right) => right.priority - left.priority || left.loyalty - right.loyalty)
    .forEach((candidate) => {
      if (availableMembers.length === 0) {
        return;
      }
      const memberIndex = availableMembers
        .map((member, index) => ({
          index,
          score: getGovernanceSeatMemberScore(member, candidate.specializationId, candidate.anchorType),
        }))
        .sort((left, right) => right.score - left.score || left.index - right.index)[0]?.index ?? -1;
      if (memberIndex < 0) {
        return;
      }
      const [member] = availableMembers.splice(memberIndex, 1);
      const anchor = getGovernorAssignmentAnchor(state, candidate);
      if (!anchor) {
        return;
      }
      anchor.governorMemberId = member.id;
      faction.dynasty.attachments.governorAssignments.push({
        memberId: member.id,
        anchorType: candidate.anchorType,
        anchorId: candidate.anchorId,
        specializationId: candidate.specializationId,
      });
    });
}

function syncCommanderAssignment(state, factionId) {
  const faction = getFaction(state, factionId);
  if (!faction?.dynasty?.attachments) {
    return;
  }

  const previousUnit = faction.dynasty.attachments.commanderUnitId
    ? getEntity(state, "unit", faction.dynasty.attachments.commanderUnitId)
    : null;
  if (previousUnit && previousUnit.health > 0) {
    previousUnit.commanderMemberId = faction.dynasty.attachments.commanderMemberId;
    return;
  }

  clearCommanderAssignment(state, factionId);
  const member = getDynastyMemberByRole(faction, ["commander", "heir_designate", "head_of_bloodline"]);
  const unit = getAliveCombatUnits(state, factionId)[0];
  if (!member || !unit) {
    return;
  }

  faction.dynasty.attachments.commanderMemberId = member.id;
  faction.dynasty.attachments.commanderUnitId = unit.id;
  unit.commanderMemberId = member.id;
  if (member.status === "active") {
    member.status = "commanding";
  }
}

function syncDynastyAssignments(state) {
  Object.values(state.factions).forEach((faction) => {
    if (faction.kind !== "kingdom" || !faction.dynasty) {
      return;
    }

    syncCommanderAssignment(state, faction.id);
    syncGovernorAssignments(state, faction.id);
  });
}

function getCommanderAuraProfile(state, unit) {
  const faction = getFaction(state, unit.factionId);
  if (!faction?.dynasty?.attachments?.commanderUnitId) {
    return null;
  }

  const commanderUnit = getEntity(state, "unit", faction.dynasty.attachments.commanderUnitId);
  const commanderMember = getDynastyMember(faction, faction.dynasty.attachments.commanderMemberId);
  if (!commanderUnit || commanderUnit.health <= 0 || !commanderMember) {
    return null;
  }

  const doctrine = getFaithDoctrineEffects(state, unit.factionId);
  const auraRadius = COMMANDER_BASE_AURA_RADIUS + doctrine.auraRadiusBonus + commanderMember.renown;
  if (distance(unit.x, unit.y, commanderUnit.x, commanderUnit.y) > auraRadius) {
    return null;
  }

  return {
    member: commanderMember,
    commanderUnit,
    auraRadius,
    attackMultiplier: Math.min(1.35, (1 + commanderMember.renown * 0.006) * doctrine.auraAttackMultiplier),
    sightBonus: doctrine.auraSightBonus + Math.round(commanderMember.renown / 2),
  };
}

function canAfford(faction, cost = {}) {
  return Object.entries(cost).every(([resourceId, amount]) => (faction.resources[resourceId] ?? 0) >= amount);
}

function spendResources(faction, cost = {}) {
  Object.entries(cost).forEach(([resourceId, amount]) => {
    faction.resources[resourceId] -= amount;
  });
}

// Session 33: Marriage system first canonical layer.
// Canonical master doctrine section XVIII: marriage matters in governance,
// diplomacy, succession, and dynastic continuity. Polygamy is canonically
// restricted to Blood Dominion and Wild covenants only.
const POLYGAMY_FAITHS = new Set(["blood_dominion", "the_wild"]);
const MARRIAGE_GESTATION_IN_WORLD_DAYS = 280;
const MARRIAGE_DISSOLUTION_LEGITIMACY_LOSS = 2;
const MARRIAGE_DISSOLUTION_OATHKEEPING_GAIN = 1;
const MARRIAGE_REGENCY_LEGITIMACY_COSTS = {
  head_direct: 0,
  heir_regency: 1,
  envoy_regency: 2,
};

function ensureMarriageState(faction) {
  if (!faction.dynasty) return null;
  faction.dynasty.marriages = faction.dynasty.marriages ?? [];
  faction.dynasty.marriageProposalsIn = faction.dynasty.marriageProposalsIn ?? [];
  faction.dynasty.marriageProposalsOut = faction.dynasty.marriageProposalsOut ?? [];
  return faction.dynasty;
}

function serializeMarriageAuthorityProfile(profile) {
  if (!profile) {
    return null;
  }
  return {
    available: profile.available,
    memberId: profile.member?.id ?? null,
    title: profile.member?.title ?? null,
    roleId: profile.member?.roleId ?? null,
    mode: profile.mode ?? null,
    label: profile.label ?? null,
    legitimacyCost: profile.legitimacyCost ?? 0,
    reason: profile.reason ?? null,
  };
}

function serializeMarriageEnvoyProfile(profile) {
  if (!profile) {
    return null;
  }
  return {
    available: profile.available,
    memberId: profile.member?.id ?? null,
    title: profile.member?.title ?? null,
    roleId: profile.member?.roleId ?? null,
    reason: profile.reason ?? null,
  };
}

function getMarriageAuthorityProfile(faction) {
  if (!faction?.dynasty) {
    return {
      available: false,
      mode: null,
      label: null,
      legitimacyCost: 0,
      member: null,
      reason: "Faction has no dynasty to authorize marriage.",
    };
  }

  const head = getDynastyMemberByRole(faction, ["head_of_bloodline"]);
  if (head?.status === "ruling") {
    return {
      available: true,
      mode: "head_direct",
      label: "Head approval",
      legitimacyCost: MARRIAGE_REGENCY_LEGITIMACY_COSTS.head_direct,
      member: head,
      reason: null,
    };
  }

  const heir = getDynastyMemberByRole(faction, ["heir_designate"]);
  if (heir) {
    return {
      available: true,
      mode: "heir_regency",
      label: "Heir regency",
      legitimacyCost: MARRIAGE_REGENCY_LEGITIMACY_COSTS.heir_regency,
      member: heir,
      reason: null,
    };
  }

  const envoy = getDynastyMemberByRole(faction, ["diplomat"]);
  if (envoy) {
    return {
      available: true,
      mode: "envoy_regency",
      label: "Envoy regency",
      legitimacyCost: MARRIAGE_REGENCY_LEGITIMACY_COSTS.envoy_regency,
      member: envoy,
      reason: null,
    };
  }

  return {
    available: false,
    mode: null,
    label: null,
    legitimacyCost: 0,
    member: null,
    reason: `${faction.displayName} has no head, heir, or envoy able to sanction a marriage.`,
  };
}

function getMarriageEnvoyProfile(faction) {
  if (!faction?.dynasty) {
    return {
      available: false,
      member: null,
      reason: "Faction has no dynasty to dispatch a marriage envoy.",
    };
  }

  const envoy = getDynastyMemberByRole(faction, ["diplomat"]);
  if (envoy) {
    return {
      available: true,
      member: envoy,
      reason: null,
    };
  }

  return {
    available: false,
    member: null,
    reason: `${faction.displayName} has no committed diplomatic envoy able to carry a marriage agreement.`,
  };
}

function buildMarriageGovernanceStatus(faction) {
  const authority = getMarriageAuthorityProfile(faction);
  const envoy = getMarriageEnvoyProfile(faction);
  return {
    authority: serializeMarriageAuthorityProfile(authority),
    envoy: serializeMarriageEnvoyProfile(envoy),
    canOfferMarriage: authority.available && envoy.available,
    offerReason: !authority.available
      ? authority.reason
      : !envoy.available
        ? envoy.reason
        : null,
  };
}

function applyMarriageGovernanceLegitimacyCost(state, faction, authorityProfile, contextLabel) {
  const penalty = authorityProfile?.legitimacyCost ?? 0;
  if (!faction?.dynasty || penalty <= 0) {
    return;
  }
  adjustLegitimacy(faction, -penalty);
  recordConvictionEvent(
    state,
    faction.id,
    "stewardship",
    -penalty,
    `${authorityProfile.title ?? "Acting authority"} governed ${contextLabel} under regency`,
  );
}

function getMarriageProposalContext(state, sourceFactionId, sourceMemberId, targetFactionId, targetMemberId) {
  const source = getFaction(state, sourceFactionId);
  const target = getFaction(state, targetFactionId);
  if (!source?.dynasty || !target?.dynasty) {
    return { ok: false, reason: "Both factions must have dynasties." };
  }
  if (sourceFactionId === targetFactionId) {
    return { ok: false, reason: "Cross-dynasty marriage requires a different faction." };
  }
  const sourceMember = getDynastyMember(source, sourceMemberId);
  const targetMember = getDynastyMember(target, targetMemberId);
  if (!sourceMember || !targetMember) {
    return { ok: false, reason: "Invalid bloodline member." };
  }
  if (sourceMember.status !== "active" && sourceMember.status !== "ruling") {
    return { ok: false, reason: "Source member is not available to marry." };
  }
  if (targetMember.status !== "active" && targetMember.status !== "ruling") {
    return { ok: false, reason: "Target member is not available to marry." };
  }
  if (memberHasActiveMarriage(source, sourceMemberId) && !factionAllowsPolygamy(source)) {
    return { ok: false, reason: "Source faction's covenant does not permit polygamy." };
  }
  if (memberHasActiveMarriage(target, targetMemberId) && !factionAllowsPolygamy(target)) {
    return { ok: false, reason: "Target faction's covenant does not permit polygamy." };
  }
  return { ok: true, source, target, sourceMember, targetMember };
}

function findMarriageProposal(state, proposalId) {
  for (const fid of Object.keys(state.factions)) {
    const faction = state.factions[fid];
    const inbox = faction.dynasty?.marriageProposalsIn ?? [];
    const found = inbox.find((proposal) => proposal.id === proposalId);
    if (found) {
      return { proposal: found, targetFaction: faction };
    }
  }
  return { proposal: null, targetFaction: null };
}

export function getMarriageGovernanceStatus(state, factionId) {
  const faction = getFaction(state, factionId);
  if (!faction?.dynasty) {
    return null;
  }
  return buildMarriageGovernanceStatus(faction);
}

export function getMarriageProposalTerms(state, sourceFactionId, sourceMemberId, targetFactionId, targetMemberId) {
  const context = getMarriageProposalContext(state, sourceFactionId, sourceMemberId, targetFactionId, targetMemberId);
  if (!context.ok) {
    return { available: false, reason: context.reason };
  }

  const sourceGovernance = buildMarriageGovernanceStatus(context.source);
  if (!sourceGovernance.canOfferMarriage) {
    return {
      available: false,
      reason: sourceGovernance.offerReason,
      sourceAuthority: sourceGovernance.authority,
      sourceEnvoy: sourceGovernance.envoy,
      targetAuthority: buildMarriageGovernanceStatus(context.target).authority,
    };
  }

  const targetGovernance = buildMarriageGovernanceStatus(context.target);
  return {
    available: true,
    sourceAuthority: sourceGovernance.authority,
    sourceEnvoy: sourceGovernance.envoy,
    targetAuthority: targetGovernance.authority,
    targetAuthorityAvailable: targetGovernance.authority?.available ?? false,
    targetAuthorityReason: targetGovernance.authority?.reason ?? null,
    legitimacyCost: sourceGovernance.authority?.legitimacyCost ?? 0,
    sourceMemberId,
    targetMemberId,
  };
}

export function getMarriageAcceptanceTerms(state, proposalId) {
  const { proposal, targetFaction } = findMarriageProposal(state, proposalId);
  if (!proposal || !targetFaction) {
    return { available: false, reason: "Marriage proposal not found." };
  }
  if (proposal.status !== "pending") {
    return { available: false, reason: "Proposal already resolved." };
  }

  const sourceFaction = getFaction(state, proposal.sourceFactionId);
  if (!sourceFaction?.dynasty) {
    return { available: false, reason: "Source faction unavailable." };
  }

  const sourceMember = getDynastyMember(sourceFaction, proposal.sourceMemberId);
  const targetMember = getDynastyMember(targetFaction, proposal.targetMemberId);
  if (!sourceMember || !targetMember) {
    return { available: false, reason: "Bloodline members no longer available." };
  }

  const targetGovernance = buildMarriageGovernanceStatus(targetFaction);
  if (!(targetGovernance.authority?.available)) {
    return {
      available: false,
      reason: targetGovernance.authority?.reason ?? "Target household has no authority able to approve the marriage.",
      targetAuthority: targetGovernance.authority,
    };
  }

  return {
    available: true,
    targetAuthority: targetGovernance.authority,
    legitimacyCost: targetGovernance.authority?.legitimacyCost ?? 0,
  };
}

function getMarriageMirrorRecords(state, marriageId) {
  return Object.values(state.factions).flatMap((faction) =>
    (faction?.dynasty?.marriages ?? []).filter((marriage) => marriage.id === marriageId),
  );
}

function getMarriageParticipants(state, marriage) {
  const headFaction = getFaction(state, marriage.headFactionId);
  const spouseFaction = getFaction(state, marriage.spouseFactionId);
  const headMember = headFaction ? getDynastyMember(headFaction, marriage.headMemberId) : null;
  const spouseMember = spouseFaction ? getDynastyMember(spouseFaction, marriage.spouseMemberId) : null;
  return {
    headFaction,
    spouseFaction,
    headMember,
    spouseMember,
  };
}

function dissolveMarriageFromDeath(state, marriage, participants, deceasedParticipants) {
  if (!marriage?.id || marriage.dissolvedAt || deceasedParticipants.length === 0) {
    return false;
  }

  const dissolvedAt = state.meta.elapsed;
  const dissolvedAtInWorldDays = state.dualClock?.inWorldDays ?? 0;
  const deceasedMemberIds = deceasedParticipants.map(({ member }) => member?.id).filter(Boolean);
  const deceasedMemberId = deceasedMemberIds[0] ?? null;
  const deceasedTitles = deceasedParticipants.map(({ member }) => member?.title).filter(Boolean);
  const mirrorRecords = getMarriageMirrorRecords(state, marriage.id);

  mirrorRecords.forEach((record) => {
    record.dissolvedAt = dissolvedAt;
    record.dissolvedAtInWorldDays = dissolvedAtInWorldDays;
    record.dissolutionReason = "death";
    record.deceasedMemberId = deceasedMemberId;
    record.deceasedMemberIds = [...deceasedMemberIds];
  });

  const affectedFactions = [participants.headFaction, participants.spouseFaction]
    .filter((faction, index, collection) =>
      faction?.dynasty && collection.findIndex((candidate) => candidate?.id === faction.id) === index)
    .map((faction) => faction);

  affectedFactions.forEach((faction) => {
    adjustLegitimacy(faction, -MARRIAGE_DISSOLUTION_LEGITIMACY_LOSS);
    recordConvictionEvent(
      state,
      faction.id,
      "oathkeeping",
      MARRIAGE_DISSOLUTION_OATHKEEPING_GAIN,
      `Mourned a marriage dissolved by death`,
    );
  });

  const headTitle = participants.headMember?.title ?? "Bloodline member";
  const spouseTitle = participants.spouseMember?.title ?? "bloodline spouse";
  const deceasedLabel = deceasedTitles.length > 1
    ? deceasedTitles.join(" and ")
    : deceasedTitles[0] ?? "a bloodline spouse";
  pushMessage(
    state,
    `Death dissolves the marriage of ${headTitle} and ${spouseTitle}. ${deceasedLabel} is mourned across both houses.`,
    affectedFactions.some((faction) => faction.id === "player") ? "warn" : "info",
  );
  return true;
}

function factionAllowsPolygamy(faction) {
  return POLYGAMY_FAITHS.has(faction.faith?.selectedFaithId ?? "");
}

// Session 35: Lesser houses promotion pipeline.
// Canonical thresholds (tunable by playtest, preserved as constants so design
// intent is legible in the source):
// - LESSER_HOUSE_RENOWN_THRESHOLD: member must reach this renown to be flagged
// - LESSER_HOUSE_MIN_PROMOTIONS:   member must have at least this many promotion
//                                   history entries (proves they climbed)
// - LESSER_HOUSE_LEGITIMACY_BONUS: founding a lesser house adds this to the
//                                   parent house's legitimacy (capped at 100)
// - LESSER_HOUSE_INITIAL_LOYALTY:  starting loyalty of the cadet branch to parent
export const LESSER_HOUSE_RENOWN_THRESHOLD = 30;
export const LESSER_HOUSE_MIN_PROMOTIONS = 1;
export const LESSER_HOUSE_LEGITIMACY_BONUS = 3;
export const LESSER_HOUSE_INITIAL_LOYALTY = 75;

// Qualifying paths for lesser-house founding. Canonically restricted to members
// whose service shaped the realm: war command, covert operations, governance.
// Priests and merchants hold influence but do not found cadet branches without
// additional mechanisms (deferred for later sessions).
const LESSER_HOUSE_QUALIFYING_PATHS = new Set([
  "military_command",
  "covert_operations",
  "governance",
]);

function ensureLesserHouseState(faction) {
  if (!faction.dynasty) return;
  if (!Array.isArray(faction.dynasty.lesserHouses)) {
    faction.dynasty.lesserHouses = [];
  }
  if (!Array.isArray(faction.dynasty.lesserHouseCandidates)) {
    faction.dynasty.lesserHouseCandidates = [];
  }
}

function memberIsLesserHouseCandidate(faction, member) {
  if (!isMemberAvailable(member)) return false;
  // Head of bloodline continues the main line, not a cadet branch.
  if (member.roleId === "head_of_bloodline") return false;
  // Already founded a lesser house.
  if (member.foundedLesserHouseId) return false;
  if ((member.renown ?? 0) < LESSER_HOUSE_RENOWN_THRESHOLD) return false;
  if ((member.promotionHistory?.length ?? 0) < LESSER_HOUSE_MIN_PROMOTIONS) return false;
  if (!LESSER_HOUSE_QUALIFYING_PATHS.has(member.pathId)) return false;
  return true;
}

function tickLesserHouseCandidates(state) {
  for (const fid of Object.keys(state.factions)) {
    const faction = state.factions[fid];
    if (!faction.dynasty) continue;
    ensureLesserHouseState(faction);
    const existing = new Set(faction.dynasty.lesserHouseCandidates);
    const current = faction.dynasty.members.filter((m) => memberIsLesserHouseCandidate(faction, m));
    const currentIds = new Set(current.map((m) => m.id));

    // Add newly-eligible members; announce once.
    for (const m of current) {
      if (!existing.has(m.id)) {
        faction.dynasty.lesserHouseCandidates.push(m.id);
        pushMessage(
          state,
          `${m.title} of ${faction.displayName} has earned the right to a cadet house.`,
          faction.id === "player" ? "good" : "info",
        );
      }
    }
    // Drop candidates who are no longer eligible (fell, got captured, promoted
    // past the pipeline, etc.) so the list does not accrete stale ids.
    faction.dynasty.lesserHouseCandidates = faction.dynasty.lesserHouseCandidates.filter((id) =>
      currentIds.has(id),
    );
  }
}

// Session 42: Lesser-house loyalty drift.
// Canonical: cadet branches are not inert. Their loyalty drifts each in-world day
// based on the parent faction's current standing. Inputs are pulled from live
// state we already track so this introduces NO new state surface beyond
// per-day timestamps on each lesser house.
//
// Inputs (per in-world day, applied additively):
//   +0.30 if parent dynasty legitimacy >= 75 (the parent looks strong and just)
//   +0.15 if parent dynasty legitimacy >= 50 (modestly strong)
//   -0.40 if parent dynasty legitimacy <  30 (weak — defection becomes thinkable)
//   +0.20 per oathkeeping band step above neutral (parent is a faithful liege)
//   -0.20 per ruthlessness band step above neutral (parent is feared, not loved)
//   -0.50 if parent has any fallen members in the last 10 ledger entries within
//          the last 10 in-world days (the realm is bleeding)
// Loyalty is clamped to [0, 100]. Future sessions wire defection events to the
// 0-loyalty boundary; this session just establishes the drift signal.
export const LESSER_HOUSE_LOYALTY_MIN = 0;
export const LESSER_HOUSE_LOYALTY_MAX = 100;

function computeLesserHouseDailyLoyaltyDelta(state, parentFaction) {
  let delta = 0;
  const legit = parentFaction.dynasty?.legitimacy ?? 50;
  if (legit >= 75) delta += 0.30;
  else if (legit >= 50) delta += 0.15;
  if (legit < 30) delta -= 0.40;

  const buckets = parentFaction.conviction?.buckets ?? {};
  const oathkeeping = buckets.oathkeeping ?? 0;
  const ruthlessness = buckets.ruthlessness ?? 0;
  // Each 5-point bucket step is one band step in the canonical conviction system.
  delta += Math.floor(oathkeeping / 5) * 0.20;
  delta -= Math.floor(ruthlessness / 5) * 0.20;

  // Recent fallen ledger entries — proxy for "the realm is bleeding".
  const fallenRecent = (parentFaction.dynasty?.attachments?.fallenMembers ?? []).slice(0, 10);
  if (fallenRecent.length > 0) {
    delta -= 0.50;
  }
  return delta;
}

function getLesserHouseMixedBloodlineHouseId(parentFaction, lesserHouse) {
  const mixed = lesserHouse?.mixedBloodline
    ?? getDynastyMember(parentFaction, lesserHouse?.founderMemberId)?.mixedBloodline
    ?? null;
  const spouseHouseId = mixed?.spouseHouseId ?? null;
  if (!spouseHouseId || spouseHouseId === parentFaction.houseId) {
    return null;
  }
  return spouseHouseId;
}

function factionHasActiveMarriageWithHouse(faction, houseId) {
  return (faction?.dynasty?.marriages ?? []).some(
    (marriage) => !marriage.dissolvedAt && marriage.spouseHouseId === houseId,
  );
}

function getRelevantMarriageWithHouse(faction, houseId) {
  const matches = (faction?.dynasty?.marriages ?? []).filter((marriage) => marriage.spouseHouseId === houseId);
  const active = matches
    .filter((marriage) => !marriage.dissolvedAt)
    .sort((left, right) => (right.marriedAtInWorldDays ?? 0) - (left.marriedAtInWorldDays ?? 0))[0];
  if (active) {
    return active;
  }
  return matches.sort((left, right) =>
    (right.dissolvedAtInWorldDays ?? right.marriedAtInWorldDays ?? 0)
      - (left.dissolvedAtInWorldDays ?? left.marriedAtInWorldDays ?? 0))[0]
    ?? null;
}

function getLesserHouseMaritalAnchorProfile(state, parentFaction, lesserHouse) {
  const mixedHouseId = getLesserHouseMixedBloodlineHouseId(parentFaction, lesserHouse);
  if (!mixedHouseId) {
    return {
      houseId: null,
      marriageId: null,
      status: "none",
      delta: 0,
      childCount: 0,
    };
  }

  const marriage = getRelevantMarriageWithHouse(parentFaction, mixedHouseId);
  const childCount = marriage?.children?.length ?? 0;
  let delta = 0;
  let status = marriage ? (marriage.dissolvedAt ? "dissolved" : "active") : "none";

  if (marriage && !marriage.dissolvedAt) {
    delta += 0.15;
    delta += Math.min(0.12, childCount * 0.06);
  } else if (marriage?.dissolvedAt) {
    delta -= 0.12;
    delta -= Math.min(0.12, childCount * 0.04);
  }

  const relatedFactionIds = Object.values(state.factions)
    .filter((faction) => faction?.houseId === mixedHouseId)
    .map((faction) => faction.id);
  const hostile = relatedFactionIds.some((factionId) => (parentFaction.hostileTo ?? []).includes(factionId));
  if (hostile) {
    delta -= 0.35;
    status = marriage && !marriage.dissolvedAt ? "strained" : "fractured";
  }

  return {
    houseId: mixedHouseId,
    marriageId: marriage?.id ?? null,
    status,
    delta,
    childCount,
  };
}

function computeLesserHouseMixedBloodlineDelta(state, parentFaction, lesserHouse) {
  if (!getLesserHouseMixedBloodlineHouseId(parentFaction, lesserHouse)) {
    return 0;
  }
  return -0.25;
}

function tickLesserHouseLoyaltyDrift(state) {
  const days = state.dualClock?.inWorldDays ?? 0;
  for (const fid of Object.keys(state.factions)) {
    const f = state.factions[fid];
    const lhs = f.dynasty?.lesserHouses ?? [];
    if (lhs.length === 0) continue;
    const baseDailyDelta = computeLesserHouseDailyLoyaltyDelta(state, f);
    const cadetWorldPressure = getWorldPressureCadetInstabilityProfile(state, f);
    const previousCadetWorldPressureStatus = f.dynasty?.lastCadetWorldPressureStatus ?? "quiet";
    if (cadetWorldPressure.stressedCount > 0 && previousCadetWorldPressureStatus !== cadetWorldPressure.status) {
      if (cadetWorldPressure.level > 0) {
        pushMessage(
          state,
          `${f.displayName}'s cadet houses strain under ${cadetWorldPressure.status} world pressure.`,
          f.id === "player" ? "warn" : "info",
        );
      } else if (previousCadetWorldPressureStatus !== "quiet") {
        pushMessage(
          state,
          `Cadet pressure eases inside ${f.displayName} as world attention slackens.`,
          f.id === "player" ? "good" : "info",
        );
      }
    }
    f.dynasty.lastCadetWorldPressureStatus = cadetWorldPressure.status;
    lhs.forEach((lh) => {
      if (lh.status !== "active") return;
      const mixedBloodlineHouseId = getLesserHouseMixedBloodlineHouseId(f, lh);
      const mixedBloodlineDelta = computeLesserHouseMixedBloodlineDelta(state, f, lh);
      const maritalAnchor = getLesserHouseMaritalAnchorProfile(state, f, lh);
      const dailyDelta = baseDailyDelta + mixedBloodlineDelta + maritalAnchor.delta + cadetWorldPressure.delta;
      lh.mixedBloodlineHouseId = mixedBloodlineHouseId;
      lh.mixedBloodlinePressure = mixedBloodlineDelta;
      lh.maritalAnchorHouseId = maritalAnchor.houseId;
      lh.maritalAnchorMarriageId = maritalAnchor.marriageId;
      lh.maritalAnchorStatus = maritalAnchor.status;
      lh.maritalAnchorPressure = maritalAnchor.delta;
      lh.maritalAnchorChildCount = maritalAnchor.childCount;
      lh.worldPressureStatus = cadetWorldPressure.status;
      lh.worldPressurePressure = cadetWorldPressure.delta;
      lh.worldPressureLevel = cadetWorldPressure.level;
      lh.currentDailyLoyaltyDelta = dailyDelta;
      const previousAnchorStatus = lh.lastMaritalAnchorStatus ?? maritalAnchor.status;
      if (maritalAnchor.houseId && previousAnchorStatus !== maritalAnchor.status) {
        const houseName = state.content.byId.houses[maritalAnchor.houseId]?.name ?? maritalAnchor.houseId;
        const anchorMessage = maritalAnchor.status === "active"
          ? `${lh.name} is steadied by an active marriage anchor into ${houseName}.`
          : maritalAnchor.status === "strained"
            ? `${lh.name}'s marriage anchor into ${houseName} is straining under renewed hostility.`
            : maritalAnchor.status === "dissolved"
              ? `${lh.name} loses its marriage anchor into ${houseName} after the household union breaks.`
              : `${lh.name}'s mixed-bloodline tie to ${houseName} has fractured into open branch pressure.`;
        pushMessage(state, anchorMessage, f.id === "player" ? "warn" : "info");
      }
      lh.lastMaritalAnchorStatus = maritalAnchor.status;
      // Drift snapshot is taken on whole in-world days to keep the mechanic
      // legible (per-day deltas in the design comments above) and so tests can
      // assert idempotency between sub-day ticks.
      const lastDay = Math.floor(lh.lastLoyaltyTickInWorldDays ?? lh.foundedAtInWorldDays ?? days);
      const currentDay = Math.floor(days);
      const elapsedDays = currentDay - lastDay;
      let newLoyalty = lh.loyalty ?? LESSER_HOUSE_INITIAL_LOYALTY;
      if (elapsedDays > 0) {
        newLoyalty = Math.max(
          LESSER_HOUSE_LOYALTY_MIN,
          Math.min(LESSER_HOUSE_LOYALTY_MAX, newLoyalty + dailyDelta * elapsedDays),
        );
        lh.loyalty = newLoyalty;
        lh.lastLoyaltyTickInWorldDays = currentDay;
      }
      // Optional canonical message at the 25 / 10 / 0 thresholds, fired ONCE per
      // crossing so a lesser house drifting toward defection produces a visible
      // event in the message log without spamming.
      const announcedThresholds = lh.announcedLoyaltyThresholds ?? [];
      const crossings = [];
      if (newLoyalty <= 25 && !announcedThresholds.includes(25)) crossings.push(25);
      if (newLoyalty <= 10 && !announcedThresholds.includes(10)) crossings.push(10);
      if (newLoyalty <= 0 && !announcedThresholds.includes(0)) crossings.push(0);
      crossings.forEach((threshold) => {
        announcedThresholds.push(threshold);
        const msg = threshold === 0
          ? `${lh.name} stands ready to break with ${f.displayName}.`
          : threshold === 10
            ? `${lh.name}'s loyalty to ${f.displayName} is dangerously thin.`
            : `${lh.name}'s loyalty to ${f.displayName} is wavering.`;
        pushMessage(state, msg, f.id === "player" ? "warn" : "info");
      });
      lh.announcedLoyaltyThresholds = announcedThresholds;

      // Session 43: defection event hook. When loyalty falls to (or below) the
      // canonical breaking point AND the loyalty-bleed is sustained (we use a
      // 5-day grace period after the 0-crossing message so the player has at
      // least one in-world day to react with corrective action), the cadet
      // branch defects. Canonical effects: status flips to "defected",
      // departedAtInWorldDays recorded, parent loses legitimacy LESSER_HOUSE_DEFECTION_LEGITIMACY_PENALTY.
      if (lh.status === "active" && newLoyalty <= LESSER_HOUSE_DEFECTION_THRESHOLD) {
        lh.brokeAtLoyaltyZeroInWorldDays = lh.brokeAtLoyaltyZeroInWorldDays ?? currentDay;
        const graceElapsed = currentDay - lh.brokeAtLoyaltyZeroInWorldDays;
        if (graceElapsed >= LESSER_HOUSE_DEFECTION_GRACE_DAYS) {
          lh.status = "defected";
          lh.departedAtInWorldDays = currentDay;
          adjustLegitimacy(f, -LESSER_HOUSE_DEFECTION_LEGITIMACY_PENALTY);
          recordConvictionEvent(state, f.id, "ruthlessness", 1, "lesser_house_defected");
          pushMessage(
            state,
            `${lh.name} has broken with ${f.displayName} and gone its own way.`,
            f.id === "player" ? "warn" : "info",
          );
          // Session 44: spawn the defected branch as a real minor faction so the
          // consequence is visible on the world register, not just in the parent's
          // internal bookkeeping. The new faction is hostile to parent (canonical:
          // departed with grievance), has a seed dynasty built from the founder,
          // and carries the founding faith commitment forward. Canonical later
          // layers (territory, units, AI, diplomacy) remain documented.
          spawnDefectedMinorFaction(state, lh, f);
        }
      } else if (lh.status === "active" && newLoyalty > LESSER_HOUSE_DEFECTION_THRESHOLD && lh.brokeAtLoyaltyZeroInWorldDays != null) {
        // Recovered above the threshold during grace window — clear the flag.
        lh.brokeAtLoyaltyZeroInWorldDays = null;
      }
    });
  }
}

// Canonical thresholds for defection. Held as constants (not magic numbers in
// the tick) so the design is legible from the source and tunable in one place.
export const LESSER_HOUSE_DEFECTION_THRESHOLD = 0;
export const LESSER_HOUSE_DEFECTION_GRACE_DAYS = 5;
export const LESSER_HOUSE_DEFECTION_LEGITIMACY_PENALTY = 6;

// Session 44: Defected-branch-as-minor-faction spawn.
// When a lesser house defects (S43), it becomes an actual entry in state.factions
// rather than sitting inert in the parent's lesserHouses array. First canonical
// layer focuses on the REGISTRY: the new faction exists, carries its identity,
// references its founder, inherits the parent's faith commitment (canonical:
// the founder's faith formed under the parent), and starts hostile to parent.
//
// Intentionally NOT in this first layer (documented as follow-ups):
//   - AI activation (the minor faction acts autonomously)
//   - Marriage candidacy (minor is eligible as a marriage target)
//   - Trade and diplomacy participation
//   - Full population and economy stack
// The spawn must leave existing AI, UI, tick, and snapshot paths intact. Any
// function that iterates factions must continue to work even when a minor
// house has no AI, no buildings, no units, and an empty economy.
function getNearestControlPointContinentId(state, anchorX, anchorY) {
  let best = null;
  for (const cp of state.world.controlPoints ?? []) {
    const d = distance(cp.x, cp.y, anchorX, anchorY);
    if (!best || d < best.distance) {
      best = { distance: d, continentId: cp.continentId ?? null };
    }
  }
  return best?.continentId ?? "home";
}

function findDefectedMinorClaimPosition(state, anchorX, anchorY) {
  const offsets = [
    [5.5, 5.5],
    [-5.5, 5.5],
    [5.5, -5.5],
    [-5.5, -5.5],
    [7.5, 0],
    [-7.5, 0],
    [0, 7.5],
    [0, -7.5],
  ];
  let best = null;
  offsets.forEach(([dx, dy]) => {
    const x = clamp(anchorX + dx, 4, state.world.width - 4);
    const y = clamp(anchorY + dy, 4, state.world.height - 4);
    const nearestDistance = (state.world.controlPoints ?? []).reduce(
      (min, cp) => Math.min(min, distance(cp.x, cp.y, x, y)),
      Number.POSITIVE_INFINITY,
    );
    if (!best || nearestDistance > best.nearestDistance) {
      best = { x, y, nearestDistance };
    }
  });
  return best ?? {
    x: clamp(anchorX + 5.5, 4, state.world.width - 4),
    y: clamp(anchorY + 5.5, 4, state.world.height - 4),
  };
}

function spawnDefectedMinorTerritoryClaim(state, minor, lesserHouse, anchorX, anchorY) {
  const claimId = `${minor.id}-claim`;
  const existing = (state.world.controlPoints ?? []).find((cp) => cp.id === claimId);
  if (existing) {
    lesserHouse.defectedTerritoryId = claimId;
    return existing;
  }
  const claimPosition = findDefectedMinorClaimPosition(state, anchorX, anchorY);
  const claim = {
    id: claimId,
    name: `${lesserHouse.name} March`,
    x: claimPosition.x,
    y: claimPosition.y,
    radiusTiles: 2.8,
    captureTime: 9,
    resourceTrickle: {
      food: 0.08,
      influence: 0.06,
    },
    settlementClass: "border_settlement",
    continentId: getNearestControlPointContinentId(state, claimPosition.x, claimPosition.y),
    ownerFactionId: minor.id,
    captureFactionId: null,
    captureProgress: 0,
    contested: false,
    controlState: "stabilized",
    governorMemberId: null,
    loyalty: 62,
    fortificationTier: 0,
  };
  state.world.controlPoints.push(claim);
  lesserHouse.defectedTerritoryId = claimId;
  return claim;
}

function spawnDefectedMinorFaction(state, lesserHouse, parentFaction) {
  if (!lesserHouse || !parentFaction) return null;
  const minorId = `minor-${lesserHouse.id}`;
  // Idempotency guard: if this minor faction somehow already exists (defensive
  // replay scenarios), skip to avoid double-spawn.
  if (state.factions[minorId]) return state.factions[minorId];

  // Seed a minimal dynasty for the minor house. The founder record is COPIED
  // (not moved) so the parent's member register, renownLedger, and any
  // outstanding references remain intact. The copy is then repurposed as the
  // new house's head of bloodline.
  const founderOrig = getDynastyMember(parentFaction, lesserHouse.founderMemberId);
  const founderCopy = founderOrig
    ? {
        id: `${founderOrig.id}-minor-head`,
        title: `${founderOrig.title} of ${lesserHouse.name}`,
        roleId: "head_of_bloodline",
        pathId: founderOrig.pathId ?? "governance",
        age: founderOrig.age ?? 30,
        status: "ruling",
        renown: Math.max(12, founderOrig.renown ?? 12),
        order: 0,
        capturedByFactionId: null,
        fallenAt: null,
        promotionHistory: [],
        originFactionId: parentFaction.id,
        originMemberId: founderOrig.id,
      }
    : null;

  const minorDynasty = {
    activeMemberCap: 4,
    dormantReserve: 0,
    legitimacy: 30, // low: minor houses are new and unestablished.
    loyaltyPressure: 0,
    interregnum: false,
    attachments: {
      commanderMemberId: null,
      commanderUnitId: null,
      heirMemberId: null,
      governorAssignments: [],
      fallenMembers: [],
      capturedMembers: {},
    },
    captives: [],
    operations: { active: [], history: [] },
    intelligenceReports: [],
    marriages: [],
    marriageProposalsIn: [],
    marriageProposalsOut: [],
    lesserHouses: [],
    lesserHouseCandidates: [],
    members: founderCopy ? [founderCopy] : [],
  };

  // Presentation: preserve parent accent, desaturate primary so the minor is
  // visually related but distinct. No hard dependency on a house registry
  // entry (minor houses canonically exist off the main registry until they
  // establish themselves).
  const basePrimary = parentFaction.presentation?.primaryColor ?? "#777777";
  const baseAccent = parentFaction.presentation?.accentColor ?? "#d0b188";

  const minor = {
    id: minorId,
    houseId: null,
    displayName: lesserHouse.name,
    kind: "minor_house",
    hostileTo: [parentFaction.id],
    presentation: { primaryColor: basePrimary, accentColor: baseAccent },
    resources: {
      gold: 0,
      wood: 0,
      stone: 0,
      food: 0,
      water: 0,
      influence: 0,
      faith: 0,
      iron: 0,
      command: 0,
      morale: 0,
    },
    dynasty: minorDynasty,
    faith: {
      // Inherit parent's faith commitment (canonical: the founder worshipped
      // under the parent's covenant). Fresh exposure map.
      selectedFaithId: parentFaction.faith?.selectedFaithId ?? null,
      doctrinePath: parentFaction.faith?.doctrinePath ?? null,
      intensity: 20,
      level: 1,
      tierLabel: parentFaction.faith?.tierLabel ?? "Nascent",
      exposure: {},
      discoveredFaithIds: parentFaction.faith?.selectedFaithId
        ? [parentFaction.faith.selectedFaithId]
        : [],
      activeHolyWars: [],
      covenantTestPassed: parentFaction.faith?.covenantTestPassed ?? false,
      lastCovenantTestOutcome: parentFaction.faith?.lastCovenantTestOutcome
        ? { ...parentFaction.faith.lastCovenantTestOutcome }
        : null,
      covenantTestCooldownUntilInWorldDays: parentFaction.faith?.covenantTestCooldownUntilInWorldDays ?? 0,
      divineRightDeclaration: null,
      lastDivineRightOutcome: null,
      divineRightCooldownUntil: 0,
    },
    conviction: {
      score: 0,
      bandId: "neutral",
      bandLabel: "Neutral",
      buckets: { ruthlessness: 0, stewardship: 0, oathkeeping: 0, desecration: 0 },
      eventLedger: [
        {
          id: `${Date.now()}-${Math.random().toString(16).slice(2, 6)}`,
          bucket: "ruthlessness",
          amount: 0,
          reason: "minor_house_founded_from_defection",
        },
      ],
    },
    population: { total: 0, cap: 0, baseCap: 0, reserved: 0, growthProgress: 0 },
    ai: null, // documented follow-up: minor house AI.
    siegeLogistics: {
      lastUnsuppliedMessageAt: -999,
      lastResuppliedMessageAt: -999,
    },
    // Session 44 provenance: clear back-reference to the parent and the lesser-house
    // record so save/restore and legibility paths can reconstruct the defection
    // history of the realm.
    originFactionId: parentFaction.id,
    originLesserHouseId: lesserHouse.id,
    foundedAtInWorldDays: state.dualClock?.inWorldDays ?? 0,
  };
  state.factions[minorId] = minor;

  // Mirror the relationship back on the lesserHouse record for UI / snapshot.
  lesserHouse.defectedAsFactionId = minorId;

  // Session 45: spawn the founder as a physical commander unit on the map. The
  // minor faction stops being purely a registry entry and becomes tangible: a
  // unit appears near the parent's command hall (canonical: the founder walks
  // out of the parent's seat with their banner). Unit type is militia — the
  // first canonical layer; richer retinues and unique units are deferred.
  let claimAnchor = null;
  const parentHall = state.buildings.find(
    (b) => b.factionId === parentFaction.id && b.typeId === "command_hall" && b.health > 0,
  );
  if (parentHall) {
    const hallDef = getBuildingDef(state.content, parentHall.typeId);
    const center = getBuildingCenter(state, parentHall, hallDef);
    claimAnchor = {
      x: center.x / state.world.tileSize,
      y: center.y / state.world.tileSize,
    };
    const founderUnitId = createEntityId(state, "unit");
    const militiaDef = getUnitDef(state.content, "militia");
    if (militiaDef) {
      const founderUnit = {
        id: founderUnitId,
        factionId: minorId,
        typeId: "militia",
        x: center.x + 64,
        y: center.y + 64,
        radius: 12,
        health: militiaDef.health,
        attackCooldownRemaining: 0,
        gatherProgress: 0,
        carrying: null,
        commanderMemberId: founderCopy ? founderCopy.id : null,
        reserveDuty: null,
        reserveSettlementId: null,
        supportStatus: null,
        engineerSupportUntil: 0,
        siegeSuppliedUntil: 0,
        logisticsInterdictedUntil: 0,
        convoyRecoveryUntil: 0,
        interdictedByFactionId: null,
        escortAssignedWagonId: null,
        convoyReconsolidatedAt: 0,
        lastSupplyTransferAt: -999,
        command: null,
      };
      state.units.push(founderUnit);
      claimAnchor = {
        x: founderUnit.x / state.world.tileSize,
        y: founderUnit.y / state.world.tileSize,
      };
      // Minor's dynasty attachments point at the commander unit for future legibility.
      minor.dynasty.attachments.commanderMemberId = founderCopy ? founderCopy.id : null;
      minor.dynasty.attachments.commanderUnitId = founderUnitId;
      minor.population.total = 1;
      minor.population.cap = 1;
    }
  }

  // Session 47: the breakaway branch now claims an actual march on the world
  // map. This makes the minor house a territorial actor, not only a registry
  // entry plus militia token, and immediately threads it into loyalty, resource
  // trickle, territory count, renderer, and save/restore systems.
  if (claimAnchor) {
    spawnDefectedMinorTerritoryClaim(state, minor, lesserHouse, claimAnchor.x, claimAnchor.y);
  }

  pushMessage(
    state,
    `${lesserHouse.name} raises its own banner as a minor house, hostile to ${parentFaction.displayName}.`,
    parentFaction.id === "player" ? "warn" : "info",
  );
  return minor;
}

function tickMinorHouseTerritorialLevies(state, dt) {
  Object.values(state.factions)
    .filter((faction) => faction?.kind === "minor_house")
    .forEach((minor) => {
      const ai = ensureMinorHouseLevyState(minor);
      const claim = getMinorHouseClaim(state, minor.id);
      const territories = state.world.controlPoints.filter((controlPoint) => controlPoint.ownerFactionId === minor.id);
      const retinue = getMinorHouseCombatUnits(state, minor.id);
      const pressureOpportunity = getMinorHousePressureOpportunityProfile(state, minor);
      const roundedLevyTempo = Math.round((pressureOpportunity.levyTempoMultiplier ?? 1) * 100) / 100;
      const roundedRetakeTempo = Math.round((pressureOpportunity.retakeTempoMultiplier ?? 1) * 100) / 100;

      ai.parentPressureLevel = pressureOpportunity.level ?? 0;
      ai.parentPressureStatus = pressureOpportunity.status ?? "quiet";
      ai.parentPressureLevyTempo = roundedLevyTempo;
      ai.parentPressureRetakeTempo = roundedRetakeTempo;
      ai.parentPressureRetinueBonus = pressureOpportunity.retinueCapBonus ?? 0;

      const previousPressureStatus = ai.lastParentPressureStatus ?? "quiet";
      if ((pressureOpportunity.status ?? "quiet") !== previousPressureStatus) {
        if (pressureOpportunity.pressured) {
          pushMessage(
            state,
            `${minor.displayName} exploits ${pressureOpportunity.parentFactionName}'s ${pressureOpportunity.status} world pressure, accelerating splinter levies and retake tempo.`,
            minor.originFactionId === "player" ? "warn" : "info",
          );
        } else if (previousPressureStatus !== "quiet") {
          pushMessage(
            state,
            `${minor.displayName} loses some of its splinter momentum as pressure on ${pressureOpportunity.parentFactionName ?? "its parent realm"} eases.`,
            minor.originFactionId === "player" ? "info" : "warn",
          );
        }
        ai.lastParentPressureStatus = pressureOpportunity.status ?? "quiet";
      }

      ai.retinueCount = retinue.length;
      ai.retinueCap = getMinorHouseRetinueCap(claim, territories.length, pressureOpportunity.retinueCapBonus ?? 0);

      if (!claim) {
        ai.levyStatus = "landless";
        ai.levyProgress = Math.max(0, ai.levyProgress - dt * MINOR_HOUSE_LEVY_PROGRESS_DECAY_PER_SECOND);
        return;
      }

      if (claim.ownerFactionId !== minor.id) {
        ai.levyStatus = "dispossessed";
        ai.levyProgress = Math.max(0, ai.levyProgress - dt * MINOR_HOUSE_LEVY_PROGRESS_DECAY_PER_SECOND);
        return;
      }

      if (claim.contested) {
        ai.levyStatus = "contested";
        ai.levyProgress = Math.max(0, ai.levyProgress - dt * MINOR_HOUSE_LEVY_PROGRESS_DECAY_PER_SECOND);
        return;
      }

      if ((claim.loyalty ?? 0) < MINOR_HOUSE_LEVY_MIN_LOYALTY) {
        ai.levyStatus = "unsettled";
        ai.levyProgress = Math.max(0, ai.levyProgress - dt * MINOR_HOUSE_LEVY_PROGRESS_DECAY_PER_SECOND);
        return;
      }

      if (retinue.length >= ai.retinueCap) {
        ai.levyStatus = "mustered";
        ai.levyProgress = 0;
        return;
      }

      const levyProfile = pickMinorHouseLevyProfile(claim, retinue.length);
      ai.levyUnitId = levyProfile.unitId;
      ai.levySecondsRequired = levyProfile.secondsRequired;

      const hasSupplies = (minor.resources.food ?? 0) >= levyProfile.foodCost;
      const hasInfluence = (minor.resources.influence ?? 0) >= levyProfile.influenceCost;
      if (!hasSupplies || !hasInfluence) {
        ai.levyStatus = !hasSupplies ? "awaiting_food" : "awaiting_influence";
        ai.levyProgress = Math.max(0, ai.levyProgress - dt * MINOR_HOUSE_LEVY_PROGRESS_DECAY_PER_SECOND * 0.4);
        return;
      }

      const loyaltyTempo = 1 + Math.max(0, (claim.loyalty ?? 0) - MINOR_HOUSE_LEVY_MIN_LOYALTY) / 80;
      ai.levyProgress += dt * loyaltyTempo * roundedLevyTempo;
      ai.levyStatus = "levying";

      if (ai.levyProgress < levyProfile.secondsRequired) {
        return;
      }

      minor.resources.food = Math.max(0, (minor.resources.food ?? 0) - levyProfile.foodCost);
      minor.resources.influence = Math.max(0, (minor.resources.influence ?? 0) - levyProfile.influenceCost);
      claim.loyalty = Math.max(0, (claim.loyalty ?? 0) - levyProfile.loyaltyCost);

      const newUnitId = spawnMinorHouseRetinueUnit(state, minor.id, claim, levyProfile.unitId);
      if (!newUnitId) {
        ai.levyStatus = "stalled";
        ai.levyProgress = 0;
        return;
      }

      const populationCost = getUnitDef(state.content, levyProfile.unitId)?.populationCost ?? 1;
      minor.population.total = Math.max(minor.population.total ?? 0, retinue.length) + populationCost;
      minor.population.baseCap = Math.max(minor.population.baseCap ?? 0, minor.population.total);
      minor.population.cap = Math.max(minor.population.cap ?? 0, minor.population.total);
      ai.lastLevyAt = state.meta.elapsed;
      ai.lastLevyUnitId = levyProfile.unitId;
      ai.levyStatus = "raised";
      ai.levyProgress = 0;

      pushMessage(
        state,
        `${minor.displayName} raises ${getUnitDef(state.content, levyProfile.unitId).name} from ${claim.name}, spending food and influence to deepen its retinue.`,
        minor.originFactionId === "player" ? "warn" : "info",
      );
    });
}

function deriveLesserHouseName(member, parentFaction) {
  // Canonical naming: "House of {member title suffix}". Kept simple and
  // deterministic so tests can verify without locale concerns.
  const base = member.title.replace(/^(Eldest |The |House )/i, "").trim();
  return `House of ${base} (${parentFaction.displayName} cadet)`;
}

export function promoteMemberToLesserHouse(state, factionId, memberId) {
  const faction = getFaction(state, factionId);
  if (!faction?.dynasty) {
    return { ok: false, reason: "Faction has no dynasty." };
  }
  ensureLesserHouseState(faction);
  const member = getDynastyMember(faction, memberId);
  if (!member) {
    return { ok: false, reason: "Member not found." };
  }
  if (!memberIsLesserHouseCandidate(faction, member)) {
    return { ok: false, reason: "Member is not an eligible lesser-house founder." };
  }

  const days = state.dualClock?.inWorldDays ?? 0;
  const lesserHouse = {
    id: createEntityId(state, "lesser-house"),
    name: deriveLesserHouseName(member, faction),
    founderMemberId: member.id,
    founderTitle: member.title,
    foundedAtInWorldDays: days,
    parentFactionId: faction.id,
    parentHouseId: faction.houseId,
    loyalty: LESSER_HOUSE_INITIAL_LOYALTY,
    status: "active",
    mixedBloodline: member.mixedBloodline ?? null,
    mixedBloodlineHouseId: member.mixedBloodline?.spouseHouseId ?? null,
    mixedBloodlinePressure: 0,
    maritalAnchorHouseId: null,
    maritalAnchorMarriageId: null,
    maritalAnchorStatus: "none",
    maritalAnchorPressure: 0,
    maritalAnchorChildCount: 0,
    lastMaritalAnchorStatus: "none",
    worldPressureStatus: "quiet",
    worldPressurePressure: 0,
    worldPressureLevel: 0,
    currentDailyLoyaltyDelta: 0,
  };
  const maritalAnchor = getLesserHouseMaritalAnchorProfile(state, faction, lesserHouse);
  lesserHouse.maritalAnchorHouseId = maritalAnchor.houseId;
  lesserHouse.maritalAnchorMarriageId = maritalAnchor.marriageId;
  lesserHouse.maritalAnchorStatus = maritalAnchor.status;
  lesserHouse.maritalAnchorPressure = maritalAnchor.delta;
  lesserHouse.maritalAnchorChildCount = maritalAnchor.childCount;
  lesserHouse.lastMaritalAnchorStatus = maritalAnchor.status;
  faction.dynasty.lesserHouses.push(lesserHouse);
  const cadetWorldPressure = getWorldPressureCadetInstabilityProfile(state, faction);
  lesserHouse.worldPressureStatus = cadetWorldPressure.status;
  lesserHouse.worldPressurePressure = cadetWorldPressure.delta;
  lesserHouse.worldPressureLevel = cadetWorldPressure.level;

  // Mark founder so the pipeline does not re-nominate them and the UI can show
  // their cadet affiliation.
  member.foundedLesserHouseId = lesserHouse.id;
  member.foundedLesserHouseAtInWorldDays = days;

  // Canonical rewards: parent house gains legitimacy, conviction ledger records
  // the stewardship act (governance of a proliferating realm).
  adjustLegitimacy(faction, LESSER_HOUSE_LEGITIMACY_BONUS);
  recordConvictionEvent(state, faction.id, "stewardship", 2, "lesser_house_founded");

  // Drop the member from the active candidate list.
  faction.dynasty.lesserHouseCandidates = faction.dynasty.lesserHouseCandidates.filter(
    (id) => id !== member.id,
  );

  pushMessage(
    state,
    `${faction.displayName} founds ${lesserHouse.name}, honoring ${member.title}.`,
    faction.id === "player" ? "good" : "info",
  );

  return { ok: true, lesserHouseId: lesserHouse.id };
}

function memberHasActiveMarriage(faction, memberId) {
  return (faction.dynasty?.marriages ?? []).some(
    (m) => (m.headMemberId === memberId || m.spouseMemberId === memberId) && !m.dissolvedAt,
  );
}

// Session 39: Marriage proposal expiration timer. Canonical: a marriage offer
// is a binding diplomatic act; if neither side responds within a generous
// window it lapses (pending → expired) and frees both members for new
// arrangements. 90 in-world days mirrors the canonical wedding-window of 30
// days at 3x for "decision pending" — long enough to weigh strategic value,
// short enough to prevent forever-stale offers cluttering the diplomatic register.
export const MARRIAGE_PROPOSAL_EXPIRATION_IN_WORLD_DAYS = 90;

function tickMarriageProposalExpiration(state) {
  const days = state.dualClock?.inWorldDays ?? 0;
  for (const fid of Object.keys(state.factions)) {
    const f = state.factions[fid];
    if (!f.dynasty) continue;
    const expireOnList = (list) => {
      if (!Array.isArray(list)) return;
      list.forEach((p) => {
        if (p.status !== "pending") return;
        const proposedDay = p.proposedAtInWorldDays ?? 0;
        if (days - proposedDay < MARRIAGE_PROPOSAL_EXPIRATION_IN_WORLD_DAYS) return;
        p.status = "expired";
        p.expiredAtInWorldDays = days;
        // Push canonical message ONCE per expiration event. To avoid duplicate
        // messages (the same proposal lives in both source.out and target.in
        // arrays) only emit when we touch the source-side record. We detect
        // that by checking whether this faction is the proposal source.
        if (p.sourceFactionId === fid) {
          const sourceName = getFactionDisplayName(state, p.sourceFactionId);
          const targetName = getFactionDisplayName(state, p.targetFactionId);
          pushMessage(
            state,
            `Marriage offer from ${sourceName} to ${targetName} lapsed.`,
            "info",
          );
        }
      });
    };
    expireOnList(f.dynasty.marriageProposalsIn);
    expireOnList(f.dynasty.marriageProposalsOut);
  }
}

export function declineMarriage(state, proposalId) {
  // Player or AI explicit decline path. Target inbox holds the proposal record;
  // we mark it status "declined" on both the inbox and the corresponding
  // source-side outbox record. Frees both members for new arrangements.
  let proposal = null;
  for (const fid of Object.keys(state.factions)) {
    const f = state.factions[fid];
    const inbox = f.dynasty?.marriageProposalsIn ?? [];
    const found = inbox.find((p) => p.id === proposalId);
    if (found) {
      proposal = found;
      break;
    }
  }
  if (!proposal) return { ok: false, reason: "Marriage proposal not found." };
  if (proposal.status !== "pending") return { ok: false, reason: "Proposal already resolved." };
  proposal.status = "declined";
  proposal.declinedAtInWorldDays = state.dualClock?.inWorldDays ?? 0;
  // Mirror the status on the source's outbox record (same id).
  const source = getFaction(state, proposal.sourceFactionId);
  const outboxMatch = (source?.dynasty?.marriageProposalsOut ?? []).find((p) => p.id === proposalId);
  if (outboxMatch) {
    outboxMatch.status = "declined";
    outboxMatch.declinedAtInWorldDays = proposal.declinedAtInWorldDays;
  }
  pushMessage(
    state,
    `${getFactionDisplayName(state, proposal.targetFactionId)} declines the marriage offer from ${getFactionDisplayName(state, proposal.sourceFactionId)}.`,
    "info",
  );
  return { ok: true };
}

export function proposeMarriage(state, sourceFactionId, sourceMemberId, targetFactionId, targetMemberId) {
  const context = getMarriageProposalContext(state, sourceFactionId, sourceMemberId, targetFactionId, targetMemberId);
  if (!context.ok) {
    return { ok: false, reason: context.reason };
  }
  const terms = getMarriageProposalTerms(state, sourceFactionId, sourceMemberId, targetFactionId, targetMemberId);
  if (!terms.available) {
    return { ok: false, reason: terms.reason };
  }

  const { source, target, sourceMember, targetMember } = context;
  ensureMarriageState(source);
  ensureMarriageState(target);
  const proposalId = createEntityId(state, "marriage-proposal");
  const proposal = {
    id: proposalId,
    sourceFactionId,
    sourceMemberId,
    targetFactionId,
    targetMemberId,
    proposedAt: state.meta.elapsed,
    proposedAtInWorldDays: state.dualClock?.inWorldDays ?? 0,
    status: "pending",
    governance: {
      sourceAuthority: terms.sourceAuthority,
      sourceEnvoy: terms.sourceEnvoy,
      targetAuthorityPreview: terms.targetAuthority,
      targetAuthorityAvailable: terms.targetAuthorityAvailable,
      targetAuthorityReason: terms.targetAuthorityReason,
    },
  };
  source.dynasty.marriageProposalsOut.unshift(proposal);
  target.dynasty.marriageProposalsIn.unshift(proposal);
  applyMarriageGovernanceLegitimacyCost(state, source, terms.sourceAuthority, `a marriage proposal toward ${target.displayName}`);
  const sanctionText = terms.sourceAuthority?.mode === "head_direct"
    ? `sanctioned by ${terms.sourceAuthority?.title ?? "the household head"}`
    : `sanctioned through ${terms.sourceAuthority?.title ?? "acting household authority"} (${terms.sourceAuthority?.label ?? "regency"}, legitimacy -${terms.legitimacyCost})`;
  const envoyText = terms.sourceEnvoy?.title
    ? ` and carried by ${terms.sourceEnvoy.title}`
    : "";
  pushMessage(
    state,
    `${getFactionDisplayName(state, sourceFactionId)} proposes marriage between ${sourceMember.title} and ${targetMember.title} of ${getFactionDisplayName(state, targetFactionId)}, ${sanctionText}${envoyText}.`,
    sourceFactionId === "player" || targetFactionId === "player" ? "info" : "info",
  );
  return { ok: true, proposalId };
}

export function acceptMarriage(state, proposalId) {
  const terms = getMarriageAcceptanceTerms(state, proposalId);
  if (!terms.available) {
    return { ok: false, reason: terms.reason };
  }
  // Find the proposal (canonical: target must accept; we look in their inbox).
  const { proposal, targetFaction: target } = findMarriageProposal(state, proposalId);
  if (!proposal || !target) return { ok: false, reason: "Marriage proposal not found." };
  const source = getFaction(state, proposal.sourceFactionId);
  if (!source?.dynasty) return { ok: false, reason: "Source faction unavailable." };
  const sourceMember = getDynastyMember(source, proposal.sourceMemberId);
  const targetMember = getDynastyMember(target, proposal.targetMemberId);
  if (!sourceMember || !targetMember) return { ok: false, reason: "Bloodline members no longer available." };

  proposal.status = "accepted";
  proposal.acceptedAt = state.meta.elapsed;
  proposal.acceptedAtInWorldDays = state.dualClock?.inWorldDays ?? 0;
  proposal.governance = {
    ...(proposal.governance ?? {}),
    acceptedByAuthority: terms.targetAuthority,
  };

  // Source faction holds the canonical "primary" marriage record; child generation
  // happens only on this record. Target gets a mirror record marked as such so it
  // never duplicates child generation. Both record arrays are independent.
  const marriedAtDays = state.dualClock?.inWorldDays ?? 0;
  const marriageId = createEntityId(state, "marriage");
  const sourceRecord = {
    id: marriageId,
    proposalId,
    headMemberId: proposal.sourceMemberId,
    headFactionId: proposal.sourceFactionId,
    spouseMemberId: proposal.targetMemberId,
    spouseFactionId: proposal.targetFactionId,
    spouseHouseId: target.houseId,
    marriedAtInWorldDays: marriedAtDays,
    children: [],
    expectedChildAtInWorldDays: marriedAtDays + MARRIAGE_GESTATION_IN_WORLD_DAYS,
    childGenerated: false,
    isPrimaryRecord: true,
    governance: proposal.governance,
  };
  const targetRecord = {
    id: marriageId,
    proposalId,
    headMemberId: proposal.targetMemberId,
    headFactionId: proposal.targetFactionId,
    spouseMemberId: proposal.sourceMemberId,
    spouseFactionId: proposal.sourceFactionId,
    spouseHouseId: source.houseId,
    marriedAtInWorldDays: marriedAtDays,
    children: [],
    expectedChildAtInWorldDays: marriedAtDays + MARRIAGE_GESTATION_IN_WORLD_DAYS,
    childGenerated: false,
    isPrimaryRecord: false,
    governance: proposal.governance,
  };
  ensureMarriageState(source);
  ensureMarriageState(target);
  source.dynasty.marriages.unshift(sourceRecord);
  target.dynasty.marriages.unshift(targetRecord);
  applyMarriageGovernanceLegitimacyCost(state, target, terms.targetAuthority, `marriage acceptance with ${source.displayName}`);

  // Canonical: marriage shifts diplomatic state. Both sides drop hostility toward each
  // other if they were hostile; oathkeeping conviction event for both.
  source.hostileTo = (source.hostileTo ?? []).filter((id) => id !== proposal.targetFactionId);
  target.hostileTo = (target.hostileTo ?? []).filter((id) => id !== proposal.sourceFactionId);
  recordConvictionEvent(state, source.id, "oathkeeping", 2, `Married ${sourceMember.title} into ${target.displayName}`);
  recordConvictionEvent(state, target.id, "oathkeeping", 2, `Married ${targetMember.title} into ${source.displayName}`);
  if (source.dynasty) source.dynasty.legitimacy = Math.min(100, (source.dynasty.legitimacy ?? 0) + 2);
  if (target.dynasty) target.dynasty.legitimacy = Math.min(100, (target.dynasty.legitimacy ?? 0) + 2);
  declareInWorldTime(state, 30, `Marriage of ${sourceMember.title} and ${targetMember.title}`);
  const approvalText = terms.targetAuthority?.mode === "head_direct"
    ? `under ${terms.targetAuthority?.title ?? "household"} approval`
    : `under ${terms.targetAuthority?.title ?? "acting authority"} (${terms.targetAuthority?.label ?? "regency"}, legitimacy -${terms.legitimacyCost})`;
  pushMessage(
    state,
    `${sourceMember.title} of ${source.displayName} weds ${targetMember.title} of ${target.displayName} ${approvalText}.`,
    "good",
  );
  return { ok: true, marriageId: sourceRecord.id };
}

function tickMarriageDissolutionFromDeath(state) {
  const processedMarriageIds = new Set();
  for (const faction of Object.values(state.factions)) {
    const marriages = faction?.dynasty?.marriages ?? [];
    for (const marriage of marriages) {
      if (!marriage.isPrimaryRecord || marriage.dissolvedAt || processedMarriageIds.has(marriage.id)) {
        continue;
      }

      const participants = getMarriageParticipants(state, marriage);
      const deceasedParticipants = [
        { faction: participants.headFaction, member: participants.headMember },
        { faction: participants.spouseFaction, member: participants.spouseMember },
      ].filter(({ member }) => member?.status === "fallen");

      if (deceasedParticipants.length === 0) {
        continue;
      }

      dissolveMarriageFromDeath(state, marriage, participants, deceasedParticipants);
      processedMarriageIds.add(marriage.id);
    }
  }
}

function tickMarriageGestation(state) {
  const days = state.dualClock?.inWorldDays ?? 0;
  for (const fid of Object.keys(state.factions)) {
    const f = state.factions[fid];
    if (!f.dynasty?.marriages) continue;
    f.dynasty.marriages.forEach((m) => {
      if (m.childGenerated) return;
      if (m.dissolvedAt) return;
      if (!m.isPrimaryRecord) return; // Only primary record generates children to prevent duplication.
      if (days < (m.expectedChildAtInWorldDays ?? Infinity)) return;
      // Child generation: a new dynasty member is added to the head faction.
      // The child carries mixed bloodline metadata (sourceHouseId + spouseHouseId).
      const childId = createEntityId(state, "bloodline-child");
      const headMember = getDynastyMember(f, m.headMemberId);
      const childTitle = `Child of ${headMember?.title ?? "Bloodline"}`;
      const child = {
        id: childId,
        title: childTitle,
        roleId: "heir_designate",
        pathId: "governance",
        age: 0,
        status: "active",
        renown: 4,
        mixedBloodline: { headHouseId: f.houseId, spouseHouseId: m.spouseHouseId },
      };
      f.dynasty.members.push(child);
      m.children.push(childId);
      m.childGenerated = true;
      pushMessage(
        state,
        `A child is born into ${f.displayName}: ${childTitle}.`,
        f.id === "player" ? "good" : "info",
      );
    });
  }
}

// Session 30: Transport embark/disembark runtime.
// Canonical master doctrine section XV: transport vessels carry land units across
// water. Each Transport Ship stores an `embarkedUnitIds` array up to its
// `transportCapacity`. Embarked units are simulation-inactive: they do not
// render, do not gather, do not attack, but persist health and state.
// Disembark ejects them onto the nearest land tile adjacent to the transport.
export function embarkUnitsOnTransport(state, transportUnitId, unitIds) {
  const transport = state.units.find((u) => u.id === transportUnitId && u.health > 0);
  if (!transport) return { ok: false, reason: "Transport not found." };
  const transportDef = getUnitDef(state.content, transport.typeId);
  if (!transportDef || (transportDef.transportCapacity ?? 0) <= 0) {
    return { ok: false, reason: "Unit is not a transport." };
  }
  transport.embarkedUnitIds = transport.embarkedUnitIds ?? [];
  const capacity = transportDef.transportCapacity;
  const remaining = capacity - transport.embarkedUnitIds.length;
  if (remaining <= 0) return { ok: false, reason: "Transport is full." };
  const candidates = state.units.filter((u) => {
    if (!unitIds.includes(u.id) || u.health <= 0) return false;
    if (u.factionId !== transport.factionId) return false;
    if (u.embarkedInTransportId) return false;
    const ud = getUnitDef(state.content, u.typeId);
    // Only land-domain units can embark on transport. Vessels don't board other vessels.
    if (ud?.movementDomain === "water") return false;
    // Must be close enough to the transport (within 2 tiles).
    return distance(u.x, u.y, transport.x, transport.y) <= state.world.tileSize * 2.5;
  }).slice(0, remaining);
  candidates.forEach((u) => {
    transport.embarkedUnitIds.push(u.id);
    u.embarkedInTransportId = transport.id;
    u.command = null;
  });
  if (candidates.length === 0) {
    return { ok: false, reason: "No eligible land units adjacent to transport." };
  }
  pushMessage(
    state,
    `${candidates.length} units embark on ${transportDef.name}.`,
    transport.factionId === "player" ? "info" : "info",
  );
  return { ok: true, embarkedCount: candidates.length };
}

export function disembarkTransport(state, transportUnitId) {
  const transport = state.units.find((u) => u.id === transportUnitId && u.health > 0);
  if (!transport) return { ok: false, reason: "Transport not found." };
  if (!Array.isArray(transport.embarkedUnitIds) || transport.embarkedUnitIds.length === 0) {
    return { ok: false, reason: "Transport is empty." };
  }
  // Find adjacent land tile: scan a ring around the transport's tile.
  const tileSize = state.world.tileSize;
  const tx = Math.floor(transport.x / tileSize);
  const ty = Math.floor(transport.y / tileSize);
  let landTile = null;
  for (let dx = -1; dx <= 1 && !landTile; dx += 1) {
    for (let dy = -1; dy <= 1; dy += 1) {
      if (dx === 0 && dy === 0) continue;
      const candX = tx + dx;
      const candY = ty + dy;
      if (!isWaterTileAt(state, candX, candY)) {
        landTile = { x: candX, y: candY };
        break;
      }
    }
  }
  if (!landTile) {
    return { ok: false, reason: "No adjacent land tile to disembark onto." };
  }
  const disembarkX = (landTile.x + 0.5) * tileSize;
  const disembarkY = (landTile.y + 0.5) * tileSize;
  const disembarkedIds = [...transport.embarkedUnitIds];
  disembarkedIds.forEach((unitId, index) => {
    const u = state.units.find((unit) => unit.id === unitId);
    if (!u) return;
    const offset = {
      x: ((index % 3) - 1) * 10,
      y: (Math.floor(index / 3) - 1) * 10,
    };
    u.x = disembarkX + offset.x;
    u.y = disembarkY + offset.y;
    u.embarkedInTransportId = null;
    u.command = null;
  });
  transport.embarkedUnitIds = [];
  pushMessage(
    state,
    `${disembarkedIds.length} units disembark from ${getUnitDef(state.content, transport.typeId).name}.`,
    transport.factionId === "player" ? "good" : "info",
  );
  return { ok: true, disembarkedCount: disembarkedIds.length };
}

// Session 27: terrain-water detection. Canonical: "river" and "water" terrain types
// are navigable by vessels (movementDomain === "water") and rejected by land units.
function isWaterTileAt(state, tileX, tileY) {
  const patches = state.world.terrainPatches ?? [];
  for (const p of patches) {
    if (p.type !== "water" && p.type !== "river") continue;
    if (tileX >= p.x && tileX < p.x + p.w && tileY >= p.y && tileY < p.y + p.h) {
      return true;
    }
  }
  return false;
}

function isFootprintAdjacentToWater(state, tileX, tileY, footprint) {
  const w = footprint.w;
  const h = footprint.h;
  // Check a 1-tile ring around the footprint for any water tile.
  for (let dx = -1; dx <= w; dx += 1) {
    for (let dy = -1; dy <= h; dy += 1) {
      // Skip interior cells (only edges matter for adjacency).
      if (dx >= 0 && dx < w && dy >= 0 && dy < h) continue;
      if (isWaterTileAt(state, tileX + dx, tileY + dy)) return true;
    }
  }
  return false;
}

function insideWorld(state, tileX, tileY, footprint) {
  return tileX >= 0 &&
    tileY >= 0 &&
    tileX + footprint.w <= state.world.width &&
    tileY + footprint.h <= state.world.height;
}

function intersectsAnyBuilding(state, tileX, tileY, footprint, ignoreBuildingId = null) {
  const rect = { x: tileX, y: tileY, w: footprint.w, h: footprint.h };
  return state.buildings.some((building) => {
    if (building.id === ignoreBuildingId || building.health <= 0) {
      return false;
    }

    const def = getBuildingDef(state.content, building.typeId);
    const other = { x: building.tileX, y: building.tileY, w: def.footprint.w, h: def.footprint.h };
    return !(
      rect.x + rect.w <= other.x ||
      rect.x >= other.x + other.w ||
      rect.y + rect.h <= other.y ||
      rect.y >= other.y + other.h
    );
  });
}

function getNearestDropoff(state, factionId, resourceType, fromX, fromY) {
  const candidates = getCompletedBuildings(state, factionId).filter((building) => {
    const def = getBuildingDef(state.content, building.typeId);
    return def.dropoffResources?.includes(resourceType) && !isBuildingUnderScoutRaid(state, building);
  });

  return candidates.reduce((best, candidate) => {
    const def = getBuildingDef(state.content, candidate.typeId);
    const center = getBuildingCenter(state, candidate, def);
    const candidateDistance = distance(fromX, fromY, center.x, center.y);
    if (!best || candidateDistance < best.distance) {
      return { building: candidate, distance: candidateDistance };
    }
    return best;
  }, null)?.building ?? null;
}

function applyDamage(state, entityType, entityId, amount, attackerFactionId, attackerContext = null) {
  const entity = getEntity(state, entityType, entityId);
  if (!entity || entity.health <= 0) {
    return;
  }

  // Siege-vs-structural canon: wall/tower/gate/keep all have structural multipliers.
  // Line infantry attacks a wall at 0.2x. A ram's 3.5x structural bonus counters for 0.7x.
  // A ram attacking a unit applies 0.4x anti-unit — rams are canonically weak without escort.
  let finalDamage = amount;
  if (entityType === "building") {
    const buildingDef = getBuildingDef(state.content, entity.typeId);
    const structuralMult = buildingDef?.structuralDamageMultiplier ?? 1;
    const attackerStructural = attackerContext?.structuralDamageMultiplier ?? 1;
    finalDamage = amount * structuralMult * attackerStructural;
  } else if (entityType === "unit") {
    const attackerAntiUnit = attackerContext?.antiUnitDamageMultiplier ?? 1;
    finalDamage = amount * attackerAntiUnit;
  }

  entity.health -= finalDamage;

  if (entity.health > 0) {
    return;
  }

  entity.health = 0;
  entity.killedByFactionId = attackerFactionId ?? entity.killedByFactionId ?? null;

  // Canonical assault-failure cohesion strain: if an attacking combat unit dies near a
  // hostile fortification, the attacking faction accumulates strain. This is the
  // wave-spam-denial pillar of the 2026-04-14 fortification doctrine.
  if (entityType === "unit" && attackerFactionId && attackerFactionId !== entity.factionId) {
    const unitDef = getUnitDef(state.content, entity.typeId);
    const isCombatUnit = unitDef && unitDef.role !== "worker";
    if (isCombatUnit && isUnitNearHostileFortification(state, entity, attackerFactionId)) {
      const assaultFaction = getFaction(state, entity.factionId);
      if (assaultFaction) {
        assaultFaction.assaultFailureStrain = (assaultFaction.assaultFailureStrain ?? 0) + 1;
      }
    }
  }

  // Session 40: Battlefield-hero renown award. Confirmed kills credited to the
  // attacker faction's heroic recipient (commander → head → military path →
  // any). Worker kills do NOT grant renown (canonical: peasant deaths are not
  // glory). Building kills only count for fortification-roled buildings,
  // matching the canonical fortification doctrine that defines what is worth
  // being remembered as a battlefield achievement.
  if (attackerFactionId && attackerFactionId !== entity.factionId) {
    if (entityType === "unit") {
      const killedDef = getUnitDef(state.content, entity.typeId);
      if (killedDef && killedDef.role !== "worker") {
        awardRenownToFaction(state, attackerFactionId, RENOWN_AWARD_COMBAT_KILL, `kill_${killedDef.id ?? entity.typeId}`);
      }
    } else if (entityType === "building") {
      const killedDef = getBuildingDef(state.content, entity.typeId);
      if (killedDef?.fortificationRole) {
        awardRenownToFaction(state, attackerFactionId, RENOWN_AWARD_FORTIFICATION_KILL, `fortkill_${killedDef.id ?? entity.typeId}`);
      }
    }
  }
}

function isUnitNearHostileFortification(state, unit, defenderFactionId) {
  const tileSize = state.world.tileSize;
  const radius = FORTIFICATION_AURA_RADIUS_TILES * tileSize;
  for (const building of state.buildings) {
    if (building.factionId !== defenderFactionId || !building.completed || building.health <= 0) {
      continue;
    }
    const def = getBuildingDef(state.content, building.typeId);
    if (!def?.fortificationRole) {
      continue;
    }
    const center = getBuildingCenter(state, building, def);
    if (distance(unit.x, unit.y, center.x, center.y) <= radius) {
      return true;
    }
  }
  return false;
}

function finalizeUnitDeaths(state) {
  state.units.forEach((unit) => {
    if (unit.health > 0 || unit.deathFinalized) {
      return;
    }
    unit.deathFinalized = true;

    const faction = getFaction(state, unit.factionId);
    const unitDef = getUnitDef(state.content, unit.typeId);
    if (faction?.population) {
      faction.population.total = Math.max(0, faction.population.total - unitDef.populationCost);
    }

    if (unit.commanderMemberId && faction?.dynasty) {
      handleCommanderFall(state, unit, unit.killedByFactionId ?? null);
    }

    const deathMessage = unit.deathReason === "field_water_desertion"
      ? `${getFactionDisplayName(state, unit.factionId)} lost ${unitDef.name} to dehydration desertion.`
      : unit.deathReason === "field_water_attrition"
        ? `${getFactionDisplayName(state, unit.factionId)} lost ${unitDef.name} to dehydration attrition.`
        : `${getFactionDisplayName(state, unit.factionId)} lost ${unitDef.name}.`;
    pushMessage(
      state,
      deathMessage,
      unit.factionId === "player" ? "warn" : "good",
    );
  });
}

function finalizeBuildingDeaths(state) {
  state.buildings.forEach((building) => {
    if (building.health > 0 || building.deathFinalized) {
      return;
    }
    building.deathFinalized = true;

    const buildingDef = getBuildingDef(state.content, building.typeId);
    pushMessage(
      state,
      `${getFactionDisplayName(state, building.factionId)} lost ${buildingDef.name}.`,
      building.factionId === "player" ? "warn" : "good",
    );

    if (building.typeId === "command_hall" && state.meta.status === "playing") {
      const attackerFactionId = building.killedByFactionId ?? null;
      state.meta.status = building.factionId === "player" ? "lost" : "won";
      state.meta.winnerId = attackerFactionId;
      state.meta.victoryType = "command_hall_fall";
      state.meta.victoryReason = attackerFactionId
        ? `${getFactionDisplayName(state, attackerFactionId)} destroyed ${getFactionDisplayName(state, building.factionId)}'s Command Hall.`
        : `${getFactionDisplayName(state, building.factionId)} lost its Command Hall.`;
      pushMessage(
        state,
        building.factionId === "player"
          ? "The Ironmark Command Hall has fallen. The frontier is lost."
          : "The Stonehelm Command Hall is broken. Ironmark holds the field.",
        building.factionId === "player" ? "warn" : "good",
      );
      if (attackerFactionId) {
        recordConvictionEvent(state, attackerFactionId, "ruthlessness", 5, "Destroyed a ruling hall");
      }
    }
  });
}

// Session 96: find the nearest water tile adjacent to a building for vessel spawning.
function findNearestWaterSpawnPosition(state, building, buildingDef) {
  const tileSize = state.world.tileSize ?? 32;
  const footprint = buildingDef.footprint ?? { w: 2, h: 2 };
  for (let dx = -1; dx <= footprint.w; dx += 1) {
    for (let dy = -1; dy <= footprint.h; dy += 1) {
      if (dx >= 0 && dx < footprint.w && dy >= 0 && dy < footprint.h) continue;
      const checkX = building.tileX + dx;
      const checkY = building.tileY + dy;
      if (isWaterTileAt(state, checkX, checkY)) {
        return { x: (checkX + 0.5) * tileSize, y: (checkY + 0.5) * tileSize };
      }
    }
  }
  return null;
}

function spawnUnitAtBuilding(state, factionId, building, unitId) {
  const buildingDef = getBuildingDef(state.content, building.typeId);
  const center = getBuildingCenter(state, building, buildingDef);
  const offset = 22 + (state.counters.unit % 3) * 10;
  const nextUnitId = createEntityId(state, "unit");
  const unitDef = getUnitDef(state.content, unitId);

  // Session 96: vessels spawn on the nearest water tile adjacent to the producing building.
  let spawnX = center.x + offset;
  let spawnY = center.y + offset;
  if (unitDef.role === "vessel") {
    const waterSpawn = findNearestWaterSpawnPosition(state, building, buildingDef);
    if (waterSpawn) {
      spawnX = waterSpawn.x + (state.counters.unit % 3) * 6;
      spawnY = waterSpawn.y + (state.counters.unit % 3) * 6;
    }
  }

  state.units.push({
    id: nextUnitId,
    factionId,
    typeId: unitId,
    x: spawnX,
    y: spawnY,
    radius: unitDef.role === "worker" ? 10 : 12,
    health: unitDef.health,
    attackCooldownRemaining: 0,
    gatherProgress: 0,
    carrying: null,
    commanderMemberId: null,
    reserveDuty: null,
    reserveSettlementId: null,
    supportStatus: null,
    engineerSupportUntil: 0,
    siegeSuppliedUntil: 0,
    raidCooldownRemaining: 0,
    logisticsInterdictedUntil: 0,
    convoyRecoveryUntil: 0,
    interdictedByFactionId: null,
    escortAssignedWagonId: null,
    convoyReconsolidatedAt: 0,
    lastSupplyTransferAt: -999,
    command: null,
  });

  return nextUnitId;
}

function recalculatePopulationCaps(state) {
  Object.values(state.factions).forEach((faction) => {
    const buildingCap = getCompletedBuildings(state, faction.id).reduce((sum, building) => {
      const def = getBuildingDef(state.content, building.typeId);
      return sum + (def.populationCapBonus ?? 0);
    }, 0);

    faction.population.cap = faction.population.baseCap + buildingCap;
  });
}

function tickPassiveResources(state, dt) {
  state.buildings.forEach((building) => {
    if (!building.completed || building.health <= 0 || isBuildingUnderScoutRaid(state, building)) {
      return;
    }

    const def = getBuildingDef(state.content, building.typeId);
    if (!def.resourceTrickle) {
      return;
    }

    const faction = getFaction(state, building.factionId);
    const politicalEffects = getFactionPoliticalEventEffects(state, building.factionId);
    Object.entries(def.resourceTrickle).forEach(([resourceId, amount]) => {
      faction.resources[resourceId] += amount * dt * (politicalEffects.resourceTrickleMultiplier ?? 1);
    });
  });
}

function tickPopulationGrowth(state, dt) {
  Object.values(state.factions).forEach((faction) => {
    const used = getUsedPopulation(state, faction.id);
    const availableCap = Math.max(0, faction.population.cap - used - faction.population.reserved);
    const doctrine = getFaithDoctrineEffects(state, faction.id);
    const conviction = getConvictionBandEffects(state, faction.id);
    const politicalEffects = getFactionPoliticalEventEffects(state, faction.id);
    // Session 15: Ironmark Blood Production deepening. Sustained blood levy load
    // depresses growth tempo (canonical cost of attritional war). Below threshold 8,
    // no effect. Above 8, growth interval stretches up to 1.8x at load 14+.
    const bloodLoad = faction.bloodProductionLoad ?? 0;
    const bloodGrowthPenalty = faction.houseId === "ironmark" && bloodLoad > 8
      ? Math.min(1.8, 1 + (bloodLoad - 8) * 0.1)
      : 1;
    // Session 18: conviction band also modifies growth (Apex Moral canonically
    // favors prosperity; Apex Cruel canonically erodes it).
    const growthThreshold = (GROWTH_INTERVAL_SECONDS * bloodGrowthPenalty) / Math.max(
      0.5,
      doctrine.populationGrowthMultiplier *
        conviction.populationGrowthMultiplier *
        (politicalEffects.populationGrowthMultiplier ?? 1),
    );

    if (
      faction.population.total < faction.population.cap &&
      availableCap > 0 &&
      faction.resources.food >= faction.population.total + 1 &&
      faction.resources.water >= faction.population.total + 1
    ) {
      faction.population.growthProgress += dt;

      if (faction.population.growthProgress >= growthThreshold) {
        faction.population.growthProgress = 0;
        faction.population.total += 1;
        faction.resources.food -= 1;
        faction.resources.water -= 1;
        pushMessage(
          state,
          `${getHouse(state.content, faction.houseId).name} population grew to ${faction.population.total}.`,
          faction.id === "player" ? "good" : "info",
        );
      }
    } else {
      faction.population.growthProgress = Math.max(0, faction.population.growthProgress - dt * 0.5);
    }
  });
}

function findNearestEnemyInRange(state, unit, range, extraSightBonus = 0, options = {}) {
  const includeBuildings = options.includeBuildings ?? true;
  const includeWorkers = options.includeWorkers ?? true;
  const includeMovingLogisticsCarriers = options.includeMovingLogisticsCarriers ?? true;
  const effectiveRange = range + extraSightBonus;
  const enemyUnits = state.units.filter((candidate) =>
    candidate.health > 0 &&
    areHostile(state, unit.factionId, candidate.factionId),
  );
  const enemyBuildings = includeBuildings
    ? state.buildings.filter((building) =>
    building.health > 0 &&
    areHostile(state, unit.factionId, building.factionId),
  )
    : [];

  let best = null;

  enemyUnits.forEach((enemy) => {
    const enemyDef = getUnitDef(state.content, enemy.typeId);
    if (!includeWorkers && enemyDef?.role === "worker") {
      return;
    }
    if (!includeMovingLogisticsCarriers && isMovingLogisticsCarrierUnitDef(enemyDef)) {
      return;
    }
    const candidateDistance = distance(unit.x, unit.y, enemy.x, enemy.y);
    if (candidateDistance <= effectiveRange && (!best || candidateDistance < best.distance)) {
      best = { type: "unit", target: enemy, distance: candidateDistance };
    }
  });

  enemyBuildings.forEach((building) => {
    const def = getBuildingDef(state.content, building.typeId);
    const center = getBuildingCenter(state, building, def);
    const candidateDistance = distance(unit.x, unit.y, center.x, center.y);
    if (candidateDistance <= effectiveRange && (!best || candidateDistance < best.distance)) {
      best = { type: "building", target: building, distance: candidateDistance };
    }
  });

  return best;
}

function updateControlPoints(state, dt) {
  state.world.controlPoints.forEach((controlPoint) => {
    const position = getControlPointPosition(state, controlPoint);
    const radius = controlPoint.radiusTiles * state.world.tileSize;
    const nearbyCombatants = state.units.filter((unit) =>
      unit.health > 0 &&
      getUnitDef(state.content, unit.typeId).role !== "worker" &&
      distance(unit.x, unit.y, position.x, position.y) <= radius,
    );
    const presentFactionIds = [...new Set(nearbyCombatants.map((unit) => unit.factionId))];
    const activeClaimants = presentFactionIds.filter((factionId) =>
      !controlPoint.ownerFactionId ||
      factionId === controlPoint.ownerFactionId ||
      areHostile(state, factionId, controlPoint.ownerFactionId),
    );

    controlPoint.contested = activeClaimants.length > 1;

    if (controlPoint.ownerFactionId && !controlPoint.contested) {
      const owningFaction = getFaction(state, controlPoint.ownerFactionId);
      const doctrine = getFaithDoctrineEffects(state, controlPoint.ownerFactionId);
      const politicalEffects = getFactionPoliticalEventEffects(state, controlPoint.ownerFactionId);
      const governorProfile = getGovernorProfileForControlPoint(state, controlPoint);
      const verdantWardenSupport = getVerdantWardenControlPointSupportProfile(state, controlPoint);
      const governorBonus = controlPoint.governorMemberId
        ? GOVERNOR_TRICKLE_BONUS * governorProfile.trickleMultiplier
        : 1;
      const territoryYield = controlPoint.controlState === "stabilized" ? 1 : 0.42;
      Object.entries(controlPoint.resourceTrickle ?? {}).forEach(([resourceId, amount]) => {
        owningFaction.resources[resourceId] += amount * dt * territoryYield * governorBonus * (politicalEffects.resourceTrickleMultiplier ?? 1);
      });
      const stabilizationRate = 0.4 *
        doctrine.stabilizationMultiplier *
        (politicalEffects.stabilizationMultiplier ?? 1) *
        (controlPoint.governorMemberId
          ? GOVERNOR_STABILIZATION_BONUS * governorProfile.stabilizationMultiplier
          : 1) *
        (verdantWardenSupport.stabilizationMultiplier ?? 1);
      controlPoint.loyalty = Math.min(100, controlPoint.loyalty + dt * stabilizationRate);
      if (controlPoint.controlState === "occupied" && controlPoint.loyalty >= TERRITORY_STABILIZED_LOYALTY) {
        controlPoint.controlState = "stabilized";
        recordConvictionEvent(state, controlPoint.ownerFactionId, "oathkeeping", 3, `Stabilized ${controlPoint.name}`);
        recordConvictionEvent(state, controlPoint.ownerFactionId, "stewardship", 2, `Governed ${controlPoint.name}`);
        pushMessage(
          state,
          `${getFactionDisplayName(state, controlPoint.ownerFactionId)} stabilized rule at ${controlPoint.name}.`,
          controlPoint.ownerFactionId === "player" ? "good" : "info",
        );
      }
    }

    if (controlPoint.contested || activeClaimants.length === 0) {
      const governorProfile = getGovernorProfileForControlPoint(state, controlPoint);
      controlPoint.captureProgress = Math.max(
        0,
        controlPoint.captureProgress - dt * CONTROL_POINT_CAPTURE_DECAY * governorProfile.captureResistanceMultiplier,
      );
      if (activeClaimants.length === 0) {
        controlPoint.captureFactionId = null;
      }
      return;
    }

    const claimantId = activeClaimants[0];
    if (controlPoint.ownerFactionId === claimantId) {
      const doctrine = getFaithDoctrineEffects(state, claimantId);
      const conviction = getConvictionBandEffects(state, claimantId);
      const politicalEffects = getFactionPoliticalEventEffects(state, claimantId);
      const commanderPresent = nearbyCombatants.some((unit) => unit.factionId === claimantId && unit.commanderMemberId);
      const governorProfile = getGovernorProfileForControlPoint(state, controlPoint);
      const verdantWardenSupport = getVerdantWardenControlPointSupportProfile(state, controlPoint);
      const stabilizationRate = 1.6 * doctrine.stabilizationMultiplier * conviction.stabilizationMultiplier *
        (politicalEffects.stabilizationMultiplier ?? 1) *
        (controlPoint.governorMemberId ? GOVERNOR_STABILIZATION_BONUS * governorProfile.stabilizationMultiplier : 1) *
        (commanderPresent ? 1.1 : 1) *
        (verdantWardenSupport.stabilizationMultiplier ?? 1);
      controlPoint.captureFactionId = null;
      controlPoint.captureProgress = 0;
      controlPoint.loyalty = Math.min(100, controlPoint.loyalty + dt * stabilizationRate);
      if (controlPoint.controlState === "occupied" && controlPoint.loyalty >= TERRITORY_STABILIZED_LOYALTY) {
        controlPoint.controlState = "stabilized";
        recordConvictionEvent(state, claimantId, "oathkeeping", 3, `Stabilized ${controlPoint.name}`);
        recordConvictionEvent(state, claimantId, "stewardship", 2, `Governed ${controlPoint.name}`);
        pushMessage(
          state,
          `${getFactionDisplayName(state, claimantId)} stabilized rule at ${controlPoint.name}.`,
          claimantId === "player" ? "good" : claimantId === "enemy" ? "warn" : "info",
        );
      }
      return;
    }

    if (controlPoint.captureFactionId && controlPoint.captureFactionId !== claimantId) {
      controlPoint.captureProgress = Math.max(0, controlPoint.captureProgress - dt * 6);
      if (controlPoint.captureProgress > 0) {
        return;
      }
    }

    controlPoint.captureFactionId = claimantId;
    const doctrine = getFaithDoctrineEffects(state, claimantId);
    const conviction = getConvictionBandEffects(state, claimantId);
    const commanderPresent = nearbyCombatants.some((unit) => unit.factionId === claimantId && unit.commanderMemberId);
    const defendingGovernorProfile = getGovernorProfileForControlPoint(state, controlPoint);
    controlPoint.captureProgress += dt * doctrine.captureMultiplier * conviction.captureMultiplier * (commanderPresent ? 1.15 : 1) / defendingGovernorProfile.captureResistanceMultiplier;
    controlPoint.loyalty = Math.max(0, controlPoint.loyalty - dt * 4.5);

    if (controlPoint.captureProgress < controlPoint.captureTime) {
      return;
    }

    const previousOwnerId = controlPoint.ownerFactionId;
    const previousGovernorMemberId = controlPoint.governorMemberId;
    if (previousOwnerId && previousGovernorMemberId) {
      handleGovernorLoss(state, previousOwnerId, previousGovernorMemberId, claimantId, position);
    }
    controlPoint.ownerFactionId = claimantId;
    controlPoint.captureFactionId = null;
    controlPoint.captureProgress = 0;
    controlPoint.loyalty = 34;
    controlPoint.controlState = "occupied";
    controlPoint.governorMemberId = null;
    recordConvictionEvent(state, claimantId, doctrine.captureMultiplier > 1.05 ? "ruthlessness" : "oathkeeping", doctrine.captureMultiplier > 1.05 ? 2 : 1, `Occupied ${controlPoint.name}`);
    // Session 26: canonical in-world time declaration on territorial capture.
    // Border takeovers compress time in-world far beyond the real-time capture action.
    declareInWorldTime(state, 14, `Captured ${controlPoint.name}`);
    // Session 32: Continental capture is canonically larger time-jump (cross-water
    // campaign requires sustained logistics). Master doctrine section XIV.
    if (controlPoint.continentId && controlPoint.continentId !== "home") {
      declareInWorldTime(state, 28, `Cross-continental capture: ${controlPoint.name}`);
      pushMessage(
        state,
        `${getFactionDisplayName(state, claimantId)} establishes a foothold on the ${controlPoint.continentId} continent at ${controlPoint.name}.`,
        claimantId === "player" ? "good" : claimantId === "enemy" ? "warn" : "info",
      );
    }
    pushMessage(
      state,
      `${getFactionDisplayName(state, claimantId)} occupied ${controlPoint.name}${previousOwnerId ? ` from ${getFactionDisplayName(state, previousOwnerId)}` : ""}.`,
      claimantId === "player" ? "good" : claimantId === "enemy" ? "warn" : "info",
    );
  });
}

function updateFaithExposure(state, dt) {
  state.world.sacredSites.forEach((sacredSite) => {
    const position = getSacredSitePosition(state, sacredSite);
    const radius = sacredSite.radiusTiles * state.world.tileSize;
    const presentFactionIds = [...new Set(state.units
      .filter((unit) => unit.health > 0 && distance(unit.x, unit.y, position.x, position.y) <= radius)
      .map((unit) => unit.factionId))]
      .filter((factionId) => state.factions[factionId]?.kind === "kingdom");

    presentFactionIds.forEach((factionId) => {
      const faction = getFaction(state, factionId);
      // Session 13: Wayshrine amplification of faith exposure.
      // A faction's own Wayshrines within faithExposureRadius of the sacred site
      // amplify that faction's exposure gain at this site by their faithExposureAmplification.
      // Multiple shrines multiply (but capped at 3x to prevent runaway).
      const wayshrineMultiplier = getWayshrineExposureMultiplierAt(state, factionId, position);
      const previousExposure = faction.faith.exposure[sacredSite.faithId] ?? 0;
      const nextExposure = Math.min(100, previousExposure + sacredSite.exposureRate * dt * wayshrineMultiplier);
      faction.faith.exposure[sacredSite.faithId] = nextExposure;

      if (!faction.faith.discoveredFaithIds.includes(sacredSite.faithId)) {
        faction.faith.discoveredFaithIds.push(sacredSite.faithId);
        pushMessage(
          state,
          `${getFactionDisplayName(state, factionId)} discovered ${sacredSite.name}.`,
          factionId === "player" ? "good" : "info",
        );
      }

      if (previousExposure < FAITH_EXPOSURE_THRESHOLD && nextExposure >= FAITH_EXPOSURE_THRESHOLD) {
        const faith = getFaith(state.content, sacredSite.faithId);
        pushMessage(
          state,
          `${getFactionDisplayName(state, factionId)} has enough exposure to embrace ${faith.name}.`,
          factionId === "player" ? "good" : "info",
        );
      }

      if (faction.faith.selectedFaithId === sacredSite.faithId) {
        // Session 13: Wayshrines belonging to the committed covenant also boost
        // ongoing faith intensity regen, strengthening ward profiles canonically.
        const intensityBonus = wayshrineMultiplier > 1 ? (wayshrineMultiplier - 1) * 0.18 : 0;
        const nextIntensity = Math.min(100, faction.faith.intensity + dt * (0.25 + intensityBonus));
        const tier = getFaithTier(nextIntensity);
        faction.faith.intensity = nextIntensity;
        faction.faith.level = tier.level;
        faction.faith.tierLabel = tier.label;
      }
    });
  });
}

function updateFaithStructureIntensity(state, dt) {
  Object.keys(state.factions).forEach((factionId) => {
    const faction = getFaction(state, factionId);
    if (!faction?.faith?.selectedFaithId) {
      return;
    }
    const regenRate = getFaithStructureIntensityRegenRate(state, factionId);
    if (regenRate <= 0) {
      return;
    }
    faction.faith.intensity = Math.min(100, (faction.faith.intensity ?? 0) + regenRate * dt);
    syncFaithIntensityState(faction);
  });
}

// Session 13 + 17: Faith exposure multiplier at a sacred-site position.
// Counts the faction's completed faith buildings (shrine, hall, and future sanctuary
// tiers) within their own faithExposureRadius of the sacred site position.
// Returns a cumulative multiplier capped at 4.0 (upper ceiling raised in Session 17
// to accommodate Hall contribution; canonical diminishing returns still apply).
function getWayshrineExposureMultiplierAt(state, factionId, sacredSitePosition) {
  let cumulative = 1;
  let contributingCount = 0;
  for (const building of state.buildings) {
    if (building.factionId !== factionId || building.health <= 0 || !building.completed) continue;
    const def = getBuildingDef(state.content, building.typeId);
    if (!def || !def.faithRole) continue;
    if (def.faithRole !== "shrine" && def.faithRole !== "hall" && def.faithRole !== "sanctuary") continue;
    const center = getBuildingCenter(state, building, def);
    const radius = def.faithExposureRadius ?? 180;
    if (distance(center.x, center.y, sacredSitePosition.x, sacredSitePosition.y) <= radius) {
      const amp = def.faithExposureAmplification ?? 1;
      cumulative *= amp;
      contributingCount += 1;
      if (contributingCount >= 4) break;
    }
  }
  return Math.min(4, cumulative);
}

function updateProjectiles(state, dt) {
  state.projectiles = state.projectiles.filter((projectile) => {
    const target = getEntity(state, projectile.targetType, projectile.targetId);
    if (!target || target.health <= 0) {
      return false;
    }

    const targetPosition = projectile.targetType === "unit"
      ? { x: target.x, y: target.y }
      : getBuildingCenter(state, target, getBuildingDef(state.content, target.typeId));

    const next = moveToward(projectile.x, projectile.y, targetPosition.x, targetPosition.y, projectile.speed, dt);
    projectile.x = next.x;
    projectile.y = next.y;

    if (!next.arrived) {
      return true;
    }

    // Mantlet cover: friendly mantlets near the target reduce inbound ranged damage.
    // Applies only to unit targets; building targets take full projectile damage.
    let inboundDamage = projectile.damage;
    if (projectile.targetType === "unit") {
      const coverMultiplier = getIncomingRangedCoverMultiplier(state, target);
      inboundDamage = projectile.damage * coverMultiplier;
    }

    applyDamage(
      state,
      projectile.targetType,
      projectile.targetId,
      inboundDamage,
      projectile.factionId,
      projectile.attackerContext ?? null,
    );
    return false;
  });
}

function updateBuildings(state, dt) {
  state.buildings.forEach((building) => {
    if (building.health <= 0) {
      return;
    }

    if (!building.completed) {
      const def = getBuildingDef(state.content, building.typeId);
      building.health = Math.max(1, (building.buildProgress / def.buildTime) * def.health);
      return;
    }

    if (building.productionQueue.length === 0) {
      return;
    }

    const queueItem = building.productionQueue[0];
    queueItem.remaining -= dt;
    if (queueItem.remaining > 0) {
      return;
    }

    const faction = getFaction(state, building.factionId);
    faction.population.reserved = Math.max(0, faction.population.reserved - queueItem.populationCost);
    spawnUnitAtBuilding(state, building.factionId, building, queueItem.unitId);
    building.productionQueue.shift();

    if (building.factionId === "player") {
      pushMessage(state, `${getUnitDef(state.content, queueItem.unitId).name} trained.`, "good");
    }
  });
}

function updateWorker(state, unit, unitDef, dt) {
  const faction = getFaction(state, unit.factionId);
  const shortagePenalty = faction.resources.food < faction.population.total || faction.resources.water < faction.population.total ? 0.75 : 1;

  if (unit.command?.type === "harass_retreat") {
    const resourceNode = unit.command.nodeId
      ? state.world.resourceNodes.find((node) => node.id === unit.command.nodeId)
      : null;
    const nodeStillHarassed = resourceNode
      ? isResourceNodeUnderScoutHarassment(state, resourceNode, unit.factionId)
      : false;
    if (resourceNode && resourceNode.amount > 0 && !nodeStillHarassed && (unit.command.resumeAt ?? 0) <= state.meta.elapsed) {
      unit.command = { type: "gather", nodeId: resourceNode.id };
      unit.gatherProgress = 0;
      return;
    }
    const next = moveToward(unit.x, unit.y, unit.command.x, unit.command.y, unitDef.speed, dt);
    unit.x = next.x;
    unit.y = next.y;
    if (!next.arrived) {
      return;
    }
    if (!resourceNode || resourceNode.amount <= 0) {
      unit.command = null;
    }
    return;
  }

  if (unit.command?.type === "move") {
    const next = moveToward(unit.x, unit.y, unit.command.x, unit.command.y, unitDef.speed, dt);
    unit.x = next.x;
    unit.y = next.y;
    if (next.arrived) {
      unit.command = null;
    }
    return;
  }

  if (unit.command?.type === "build") {
    const building = getEntity(state, "building", unit.command.buildingId);
    if (!building || building.health <= 0) {
      unit.command = null;
      return;
    }

    const def = getBuildingDef(state.content, building.typeId);
    const center = getBuildingCenter(state, building, def);
    const next = moveToward(unit.x, unit.y, center.x, center.y, unitDef.speed, dt);
    unit.x = next.x;
    unit.y = next.y;

    if (!next.arrived) {
      return;
    }

    // Session 11: Stonehelm (and any future house with fortificationBuildSpeedMultiplier)
    // builds fortification-role structures faster. Non-fortification buildings are unaffected.
    const ownerFaction = getFaction(state, building.factionId);
    const ownerHouse = ownerFaction?.houseId ? getHouse(state.content, ownerFaction.houseId) : null;
    const buildSpeedMult =
      def.fortificationRole && ownerHouse?.mechanics?.fortificationBuildSpeedMultiplier
        ? ownerHouse.mechanics.fortificationBuildSpeedMultiplier
        : 1;
    building.buildProgress += dt * unitDef.buildRate * buildSpeedMult;
    if (building.buildProgress >= def.buildTime) {
      building.completed = true;
      building.buildProgress = def.buildTime;
      building.health = def.health;
      unit.command = null;
      if (["dwelling", "farm", "well"].includes(building.typeId)) {
        recordConvictionEvent(state, building.factionId, "stewardship", 1, `Completed ${def.name}`);
      }
      if (def.fortificationRole) {
        advanceFortificationTier(state, building, def);
        recordConvictionEvent(state, building.factionId, "stewardship", 2, `Raised ${def.name}`);
      }
      if (building.factionId === "player") {
        pushMessage(state, `${def.name} completed.`, "good");
      }
    }
    return;
  }

  if (unit.command?.type !== "gather") {
    return;
  }

  const node = state.world.resourceNodes.find((resourceNode) => resourceNode.id === unit.command.nodeId);
  if (!node || node.amount <= 0) {
    unit.command = null;
    unit.carrying = null;
    return;
  }

  if (isResourceNodeUnderScoutHarassment(state, node, unit.factionId)) {
    routeWorkerToHarassRetreat(
      state,
      unit,
      node,
      node.harassedByFactionId ?? null,
      Math.max(4, (node.harassedUntil ?? state.meta.elapsed) - state.meta.elapsed),
    );
    return;
  }

  if (unit.carrying) {
    const depositTarget = getNearestDropoff(state, unit.factionId, unit.carrying.type, unit.x, unit.y);
    if (!depositTarget) {
      return;
    }
    const depositDef = getBuildingDef(state.content, depositTarget.typeId);
    const depositPosition = getBuildingCenter(state, depositTarget, depositDef);
    const next = moveToward(unit.x, unit.y, depositPosition.x, depositPosition.y, unitDef.speed, dt);
    unit.x = next.x;
    unit.y = next.y;

    if (next.arrived) {
      const carrying = unit.carrying;
      // Canonical smelting chain: iron mine converts delivered ore into iron by burning wood.
      if (depositDef.smeltingFuelResource && depositDef.smeltingFuelRatio) {
        const fuelNeeded = carrying.amount * depositDef.smeltingFuelRatio;
        const fuelStock = faction.resources[depositDef.smeltingFuelResource] ?? 0;
        if (fuelStock < fuelNeeded) {
          // Return ore to source node; stall worker until fuel available.
          const sourceNode = state.world.resourceNodes.find((n) => n.id === unit.command.nodeId);
          if (sourceNode) {
            sourceNode.amount += carrying.amount;
          }
          unit.carrying = null;
          unit.gatherProgress = 0;
          if (!faction.smeltingStallMessageAt || state.meta.elapsed - faction.smeltingStallMessageAt > 8) {
            faction.smeltingStallMessageAt = state.meta.elapsed;
            if (unit.factionId === "player") {
              pushMessage(state, `${depositDef.name} cannot smelt: insufficient ${depositDef.smeltingFuelResource}.`, "warn");
            }
          }
          return;
        }
        faction.resources[depositDef.smeltingFuelResource] -= fuelNeeded;
      }
      faction.resources[carrying.type] = (faction.resources[carrying.type] ?? 0) + carrying.amount;
      unit.carrying = null;
      unit.gatherProgress = 0;
    }
    return;
  }

  const nodeX = node.x * state.world.tileSize;
  const nodeY = node.y * state.world.tileSize;
  const next = moveToward(unit.x, unit.y, nodeX, nodeY, unitDef.speed, dt);
  unit.x = next.x;
  unit.y = next.y;

  if (!next.arrived) {
    return;
  }

  unit.gatherProgress += dt * unitDef.gatherRate * shortagePenalty;
  if (unit.gatherProgress < 1.25) {
    return;
  }

  const gathered = Math.min(unitDef.carryCapacity, node.amount);
  node.amount -= gathered;
  unit.carrying = { type: node.type, amount: gathered };
  unit.gatherProgress = 0;
}

function updateSupportUnit(state, unit, unitDef, dt) {
  const hostileSlowMultiplier = getHostileFortificationApproachSpeedMultiplier(state, unit);
  const fieldWaterOperationalProfile = getFieldWaterOperationalProfile(unit);
  const effectiveSpeed = unitDef.speed * hostileSlowMultiplier * (fieldWaterOperationalProfile.speedMultiplier ?? 1);
  const targetPosition = getSupportCommandTargetPosition(state, unit.command);

  if (!targetPosition) {
    if (unit.command?.type === "attack") {
      unit.command = null;
    }
    return;
  }

  let desiredDistance = 0;
  if (unit.command?.type === "attack") {
    desiredDistance = unitDef.role === "support"
      ? Math.max(32, (unitDef.supplyRadius ?? 96) * 0.45)
      : Math.max(28, (unitDef.engineerSupportRadius ?? 72) * 0.45);
  }

  const currentDistance = distance(unit.x, unit.y, targetPosition.x, targetPosition.y);
  if (currentDistance <= desiredDistance) {
    if (unit.command?.type !== "attack") {
      unit.command = null;
    }
    return;
  }

  const next = moveToward(unit.x, unit.y, targetPosition.x, targetPosition.y, effectiveSpeed, dt);
  unit.x = next.x;
  unit.y = next.y;
  if (next.arrived && unit.command?.type !== "attack") {
    unit.command = null;
  }
}

function updateCombatUnit(state, unit, unitDef, dt) {
  unit.raidCooldownRemaining = Math.max(0, (unit.raidCooldownRemaining ?? 0) - dt);
  const aura = getCommanderAuraProfile(state, unit);
  const fortificationProfile = getFriendlyFortificationCombatProfile(state, unit);
  const hostileSlowMultiplier = getHostileFortificationApproachSpeedMultiplier(state, unit);
  const siegeOperationalProfile = getSiegeOperationalProfile(state, unit);
  const fieldWaterOperationalProfile = getFieldWaterOperationalProfile(unit);
  const effectiveSpeed = unitDef.speed *
    hostileSlowMultiplier *
    (siegeOperationalProfile.speedMultiplier ?? 1) *
    (fieldWaterOperationalProfile.speedMultiplier ?? 1);
  if (unit.attackCooldownRemaining > 0) {
    unit.attackCooldownRemaining -= dt;
  }

  if (unit.command?.type === "fallback") {
    const next = moveToward(unit.x, unit.y, unit.command.x, unit.command.y, effectiveSpeed, dt);
    unit.x = next.x;
    unit.y = next.y;
    if (next.arrived) {
      unit.command = null;
      unit.reserveDuty = "recovering";
    }
    return;
  }

  if (unit.command?.type === "muster") {
    const next = moveToward(unit.x, unit.y, unit.command.x, unit.command.y, effectiveSpeed, dt);
    unit.x = next.x;
    unit.y = next.y;
    if (!next.arrived) {
      return;
    }
    unit.command = null;
    unit.reserveDuty = "engaged";
  }

  if (unit.command?.type === "move") {
    const next = moveToward(unit.x, unit.y, unit.command.x, unit.command.y, effectiveSpeed, dt);
    unit.x = next.x;
    unit.y = next.y;
    if (next.arrived) {
      unit.command = null;
    }
  }

  if (unit.command?.type === "raid_retreat") {
    const next = moveToward(unit.x, unit.y, unit.command.x, unit.command.y, effectiveSpeed, dt);
    unit.x = next.x;
    unit.y = next.y;
    if (next.arrived) {
      unit.command = null;
    }
    return;
  }

  if (unit.command?.type === "raid") {
    const target = unit.command.targetType === "resource"
      ? state.world.resourceNodes.find((resourceNode) => resourceNode.id === unit.command.targetId)
      : getEntity(state, unit.command.targetType, unit.command.targetId);
    if (
      !target ||
      ((unit.command.targetType === "building" || unit.command.targetType === "unit") && target.health <= 0) ||
      (unit.command.targetType !== "resource" && target.factionId === unit.factionId)
    ) {
      unit.command = null;
      return;
    }
    if (!isScoutRiderUnitDef(unitDef)) {
      unit.command = null;
      return;
    }

    if (
      unit.command.targetType === "building" &&
      !getBuildingDef(state.content, target.typeId)?.scoutRaidable
    ) {
      unit.command = null;
      return;
    }
    if (
      unit.command.targetType === "unit" &&
      !isScoutRaidableUnitDef(getUnitDef(state.content, target.typeId))
    ) {
      unit.command = null;
      return;
    }
    if (
      unit.command.targetType === "resource" &&
      !getScoutResourceHarassTargetFactionId(state, target, unit.factionId)
    ) {
      unit.command = null;
      return;
    }

    const targetPosition = getRaidTargetPosition(state, unit.command.targetType, target);
    if (!targetPosition) {
      unit.command = null;
      return;
    }

    if (distance(unit.x, unit.y, targetPosition.x, targetPosition.y) > Math.max(unitDef.attackRange ?? 0, SCOUT_RAID_TARGET_RANGE)) {
      const next = moveToward(unit.x, unit.y, targetPosition.x, targetPosition.y, effectiveSpeed, dt);
      unit.x = next.x;
      unit.y = next.y;
      return;
    }

    const targetFactionId = unit.command.targetType === "resource"
      ? getScoutResourceHarassTargetFactionId(state, target, unit.factionId)
      : target.factionId;
    const raidDefenseRadius =
      unit.command.targetType === "unit" && isMovingLogisticsCarrierUnitDef(getUnitDef(state.content, target.typeId))
        ? Math.max(72, Math.round((unitDef.raidDefenseRadius ?? 0) * 0.55))
        : (unitDef.raidDefenseRadius ?? 0);
    if (isScoutRaidTargetDefended(state, unit.factionId, targetFactionId, targetPosition, raidDefenseRadius)) {
      unit.command = getRaidRetreatCommand(state, unit, targetPosition);
      unit.raidCooldownRemaining = Math.max(unit.raidCooldownRemaining ?? 0, 2);
      return;
    }

    if (
      (unit.command.targetType === "building" && isBuildingUnderScoutRaid(state, target)) ||
      (unit.command.targetType === "resource" && isResourceNodeUnderScoutHarassment(state, target)) ||
      (unit.command.targetType === "unit" && isSupplyWagonInterdicted(state, target))
    ) {
      unit.command = getRaidRetreatCommand(state, unit, targetPosition);
      return;
    }

    if ((unit.raidCooldownRemaining ?? 0) > 0) {
      return;
    }

    unit.raidCooldownRemaining = Math.max(4, unitDef.raidCooldownSeconds ?? 12);
    if (unit.command.targetType === "building") {
      executeScoutRaid(state, unit, unitDef, target);
    } else if (unit.command.targetType === "resource") {
      executeScoutResourceHarass(state, unit, unitDef, target);
    } else {
      const targetDef = getUnitDef(state.content, target.typeId);
      if (targetDef?.role === "worker") {
        executeScoutWorkerHarass(state, unit, unitDef, target);
      } else {
        executeScoutLogisticsInterdiction(state, unit, unitDef, target);
      }
    }
    return;
  }

  if (unit.command?.type === "attack") {
    const target = getEntity(state, unit.command.targetType, unit.command.targetId);
    if (!target || target.health <= 0) {
      unit.command = null;
      return;
    }

    const siegeSupportProfile = getFriendlySiegeSupportProfile(state, unit, unit.command.targetType);
    const targetPosition = unit.command.targetType === "unit"
      ? { x: target.x, y: target.y }
      : getBuildingCenter(state, target, getBuildingDef(state.content, target.typeId));
    const attackDistance = distance(unit.x, unit.y, targetPosition.x, targetPosition.y);
    const effectiveAttackRange = unitDef.attackRange + (
      unit.command.targetType === "building"
        ? (siegeSupportProfile?.attackRangeBonus ?? 0)
        : 0
    );

    if (attackDistance > effectiveAttackRange) {
      const next = moveToward(unit.x, unit.y, targetPosition.x, targetPosition.y, effectiveSpeed, dt);
      unit.x = next.x;
      unit.y = next.y;
      return;
    }

    if (unit.attackCooldownRemaining > 0) {
      return;
    }

    unit.attackCooldownRemaining = unitDef.attackCooldown / (aura?.attackMultiplier ?? 1);
    const cohesionMultiplier = getAssaultCohesionMultiplier(state, unit.factionId);
    const structuralSupportMultiplier = unit.command.targetType === "building"
      ? (siegeSupportProfile?.structuralSupportMultiplier ?? 1)
      : 1;
    const damage = unitDef.attackDamage *
      (aura?.attackMultiplier ?? 1) *
      cohesionMultiplier *
      (fortificationProfile?.attackMultiplier ?? 1) *
      (siegeOperationalProfile.attackMultiplier ?? 1) *
      (fieldWaterOperationalProfile.attackMultiplier ?? 1) *
      (getFactionPoliticalEventEffects(state, unit.factionId).attackMultiplier ?? 1) *
      structuralSupportMultiplier;
    const attackerContext = {
      structuralDamageMultiplier: unitDef.structuralDamageMultiplier,
      antiUnitDamageMultiplier: unitDef.antiUnitDamageMultiplier,
    };
    if (unitDef.attackRange > 50) {
      state.projectiles.push({
        id: createEntityId(state, "projectile"),
        factionId: unit.factionId,
        x: unit.x,
        y: unit.y,
        targetType: unit.command.targetType,
        targetId: unit.command.targetId,
        damage,
        speed: unitDef.projectileSpeed ?? 260,
        attackerContext,
      });
    } else {
      applyDamage(state, unit.command.targetType, unit.command.targetId, damage, unit.factionId, attackerContext);
    }
    // Session 31: Fire Ship one-use sacrifice combat. The vessel detonates on its
    // first strike: target takes the full canonical damage AND the Fire Ship itself
    // is destroyed. Canonical master doctrine section XV.
    if (unitDef.oneUseSacrifice) {
      unit.health = 0;
      pushMessage(
        state,
        `${unitDef.name} detonates in a sacrificial strike.`,
        unit.factionId === "player" ? "warn" : "good",
      );
    }
    return;
  }

  const targetInfo = findNearestEnemyInRange(
    state,
    unit,
    unitDef.sight,
    (aura?.sightBonus ?? 0) + (fortificationProfile?.sightBonus ?? 0),
    isScoutRiderUnitDef(unitDef) && (unit.raidCooldownRemaining ?? 0) > 0
      ? { includeBuildings: false, includeWorkers: false, includeMovingLogisticsCarriers: false }
      : undefined,
  );
  if (targetInfo) {
    unit.command = { type: "attack", targetType: targetInfo.type, targetId: targetInfo.target.id };
  }
}

// Session 96: vessel update tick. Handles movement, fishing gather, and naval combat.
function updateVessel(state, unit, unitDef, dt) {
  if (unit.attackCooldownRemaining > 0) {
    unit.attackCooldownRemaining -= dt;
  }

  // Fishing boat: gather food from water.
  if (unitDef.vesselClass === "fishing") {
    if (!unit.command || unit.command.type === "idle") {
      // Auto-fish when idle on water.
      const tileSize = state.world.tileSize ?? 32;
      const tileX = Math.floor(unit.x / tileSize);
      const tileY = Math.floor(unit.y / tileSize);
      if (isWaterTileAt(state, tileX, tileY)) {
        const faction = getFaction(state, unit.factionId);
        const fishYield = (unitDef.gatherRate ?? 0.8) * dt;
        faction.resources.food = (faction.resources.food ?? 0) + fishYield;
      }
      return;
    }
  }

  // Movement command.
  if (unit.command?.type === "move") {
    const next = moveToward(unit.x, unit.y, unit.command.x, unit.command.y, unitDef.speed, dt);
    unit.x = next.x;
    unit.y = next.y;
    if (next.arrived) {
      unit.command = null;
    }
    return;
  }

  // Attack command (war galley, fire ship, capital ship).
  if (unit.command?.type === "attack") {
    const target = state.units.find((candidate) =>
      candidate.id === unit.command.targetId && candidate.health > 0);
    if (!target) {
      unit.command = null;
      return;
    }
    const dist = distance(unit.x, unit.y, target.x, target.y);
    const attackRange = unitDef.attackRange ?? 80;
    if (dist > attackRange) {
      const next = moveToward(unit.x, unit.y, target.x, target.y, unitDef.speed, dt);
      unit.x = next.x;
      unit.y = next.y;
      return;
    }
    if (unit.attackCooldownRemaining <= 0) {
      const damage = unitDef.attackDamage ?? unitDef.attack ?? 5;
      target.health -= damage;
      unit.attackCooldownRemaining = unitDef.attackCooldown ?? unitDef.attackSpeed ?? 1.5;
      // Session 96: Fire Ship sacrifice mechanic. oneUseSacrifice vessels
      // detonate on first strike and are destroyed.
      if (unitDef.oneUseSacrifice) {
        unit.health = 0;
        unit.command = null;
        return;
      }
      if (target.health <= 0) {
        unit.command = null;
      }
    }
    return;
  }

  // Auto-aggression: vessels engage hostile vessels in range.
  if (!unit.command) {
    const attackRange = unitDef.attackRange ?? 80;
    if ((unitDef.attack ?? 0) > 0) {
      const nearestHostile = state.units
        .filter((candidate) =>
          candidate.id !== unit.id &&
          candidate.health > 0 &&
          areHostile(state, unit.factionId, candidate.factionId) &&
          distance(unit.x, unit.y, candidate.x, candidate.y) <= attackRange * 1.2)
        .sort((a, b) => distance(unit.x, unit.y, a.x, a.y) - distance(unit.x, unit.y, b.x, b.y))[0];
      if (nearestHostile) {
        unit.command = { type: "attack", targetType: "unit", targetId: nearestHostile.id };
      }
    }
  }
}

function updateUnits(state, dt) {
  state.units.forEach((unit) => {
    if (unit.health <= 0) {
      return;
    }
    // Session 30: embarked units are simulation-inactive.
    // They do not gather, attack, or move while aboard a transport.
    if (unit.embarkedInTransportId) {
      return;
    }
    const unitDef = getUnitDef(state.content, unit.typeId);
    if (unitDef.role === "worker") {
      updateWorker(state, unit, unitDef, dt);
    } else if (unitDef.role === "vessel") {
      updateVessel(state, unit, unitDef, dt);
    } else if (isSupportRole(unitDef)) {
      updateSupportUnit(state, unit, unitDef, dt);
    } else {
      updateCombatUnit(state, unit, unitDef, dt);
    }
  });

  finalizeUnitDeaths(state);
  finalizeBuildingDeaths(state);
  state.units = state.units.filter((unit) => unit.health > 0);
  state.buildings = state.buildings.filter((building) => building.health > 0);
  syncDynastyAssignments(state);
}

export function createSimulation(content) {
  const map = content.map;
  const baselineResources = Object.fromEntries(content.resources.map((resource) => [resource.id, 0]));
  const state = {
    content,
    meta: {
      status: "playing",
      winnerId: null,
      victoryType: null,
      victoryReason: null,
      elapsed: 0,
    },
    counters: {
      unit: 0,
      building: 0,
      projectile: 0,
      dynastyOperation: 0,
    },
    world: {
      id: map.id,
      name: map.name,
      width: map.width,
      height: map.height,
      tileSize: map.tileSize,
      terrainPatches: map.terrainPatches,
      resourceNodes: map.resourceNodes.map((node) => ({
        ...node,
        harassedUntil: 0,
        harassedTargetFactionId: null,
        harassedByFactionId: null,
      })),
      controlPoints: (map.controlPoints ?? []).map((controlPoint) => ({
        ...controlPoint,
        ownerFactionId: controlPoint.ownerFactionId ?? null,
        captureFactionId: null,
        captureProgress: 0,
        contested: false,
        controlState: controlPoint.ownerFactionId ? "stabilized" : "neutral",
        governorMemberId: null,
        loyalty: controlPoint.ownerFactionId ? 50 : 18,
        settlementClass: controlPoint.settlementClass ?? "border_settlement",
        fortificationTier: 0,
      })),
      settlements: (map.settlements ?? []).map((settlement) => ({
        ...settlement,
        fortificationTier: 0,
        governorMemberId: null,
        reserveState: {
          lastCommitAt: -999,
          threatActive: false,
          lastMessageAt: -999,
        },
        imminentEngagement: createDefaultImminentEngagementState(settlement.id, settlement.name ?? settlement.id),
      })),
      sacredSites: (map.sacredSites ?? []).map((sacredSite) => ({ ...sacredSite })),
    },
    realmConditions: content.realmConditions ?? { cycleSeconds: REALM_CYCLE_DEFAULT_SECONDS, thresholds: {}, effects: {}, legibility: {} },
    realmCycleAccumulator: 0,
    realmCycleCount: 0,
    factions: {},
    units: [],
    buildings: [],
    projectiles: [],
    messages: [],
  };

  map.factions.forEach((factionSetup) => {
    const house = factionSetup.houseId ? getHouse(content, factionSetup.houseId) : null;
    state.factions[factionSetup.id] = {
      id: factionSetup.id,
      houseId: factionSetup.houseId,
      displayName: factionSetup.displayName ?? house?.name ?? factionSetup.id,
      kind: factionSetup.kind ?? "kingdom",
      hostileTo: [...(factionSetup.hostileTo ?? [])],
      presentation: house
        ? { primaryColor: house.primaryColor, accentColor: house.accentColor }
        : factionSetup.presentation ?? { primaryColor: "#777777", accentColor: "#d0b188" },
      resources: {
        ...baselineResources,
        ...factionSetup.startingResources,
      },
      dynasty: createDynastyState(content, factionSetup, factionSetup.id),
      faith: createFaithState(content, factionSetup),
      conviction: createConvictionState(content),
      population: {
        total: factionSetup.population.total,
        cap: factionSetup.population.cap,
        baseCap: 0,
        reserved: factionSetup.population.reserved,
        growthProgress: 0,
      },
      ai: factionSetup.aiProfile === "enemy-kingdom"
        ? {
            attackTimer: 30,
            buildTimer: 4,
            territoryTimer: 10,
            lastAlertSecond: -1,
          }
        : factionSetup.aiProfile === "neutral-tribes"
          ? {
              raidTimer: 22,
              lastAlertSecond: -1,
            }
        : null,
      siegeLogistics: {
        lastUnsuppliedMessageAt: -999,
        lastResuppliedMessageAt: -999,
      },
      worldPressureScore: 0,
      worldPressureStreak: 0,
      worldPressureLevel: 0,
    };

    factionSetup.buildings.forEach((building) => {
      const buildingDef = getBuildingDef(content, building.typeId);
      state.buildings.push({
        id: building.id,
        factionId: factionSetup.id,
        typeId: building.typeId,
        tileX: building.x,
        tileY: building.y,
        buildProgress: building.completed ? buildingDef.buildTime : 0,
        completed: building.completed,
        health: building.completed ? buildingDef.health : 1,
        productionQueue: [],
        raidedUntil: 0,
      });
    });

    factionSetup.units.forEach((unit) => {
      const unitDef = getUnitDef(content, unit.typeId);
      state.units.push({
        id: unit.id,
        factionId: factionSetup.id,
        typeId: unit.typeId,
        x: unit.x * map.tileSize,
        y: unit.y * map.tileSize,
        radius: unitDef.role === "worker" ? 10 : 12,
        health: unitDef.health,
        attackCooldownRemaining: 0,
        gatherProgress: 0,
        carrying: null,
        commanderMemberId: null,
        reserveDuty: null,
        reserveSettlementId: null,
        supportStatus: null,
        engineerSupportUntil: 0,
        siegeSuppliedUntil: 0,
        raidCooldownRemaining: 0,
        logisticsInterdictedUntil: 0,
        convoyRecoveryUntil: 0,
        interdictedByFactionId: null,
        escortAssignedWagonId: null,
        convoyReconsolidatedAt: 0,
        lastSupplyTransferAt: -999,
        command: null,
      });
    });
  });

  Object.values(state.factions).forEach((faction) => {
    const buildingCap = getCompletedBuildings(state, faction.id).reduce((sum, building) => {
      const def = getBuildingDef(content, building.typeId);
      return sum + (def.populationCapBonus ?? 0);
    }, 0);
    faction.population.baseCap = faction.population.cap - buildingCap;
  });

  // Session 93: Trueborn City neutral faction. Canon: the oldest authority in the
  // world, a neutral stabilizing force. Endorses moral dynasties, rejects cruel
  // ones, and materially affects population acceptance through trade relationship.
  state.factions.trueborn_city = {
    id: "trueborn_city",
    houseId: "trueborn",
    displayName: "Trueborn City",
    kind: "trueborn_city",
    hostileTo: [],
    presentation: { primaryColor: "#c4a35a", accentColor: "#f0ebe0" },
    resources: { gold: 500 },
    population: { total: 0, cap: 0, baseCap: 0, reserved: 0, growthProgress: 0 },
    dynasty: null,
    faith: null,
    conviction: null,
    ai: null,
    tradeRelationships: {},
  };

  pushMessage(state, "Ironmark foothold established. Secure the frontier.", "good");
  pushMessage(state, "The Trueborn City watches from its neutral seat. Earn its endorsement through moral governance.", "info");
  if (state.factions.tribes) {
    pushMessage(state, "Frontier tribes hold the central marches. Expect raids and territorial fights.", "info");
  }
  syncDynastyAssignments(state);
  updateMatchProgressionState(state, { announce: false });
  return state;
}

export function stepSimulation(state, dt) {
  if (state.meta.status !== "playing") {
    updateMessages(state, dt);
    return state;
  }

  state.meta.elapsed += dt;
  tickDualClock(state, dt);
  recalculatePopulationCaps(state);
  tickPassiveResources(state, dt);
  tickPopulationGrowth(state, dt);
  updateControlPoints(state, dt);
  updateFaithExposure(state, dt);
  updateFaithStructureIntensity(state, dt);
  updateBuildings(state, dt);
  tickBuildingStatusEffects(state, dt);
  tickResourceNodeHarassmentState(state);
  tickWorkerHarassRecovery(state);
  tickSupplyWagonInterdictionState(state);
  tickImminentEngagementWarnings(state, dt);
  tickFortificationReserves(state, dt);
  tickSiegeSupportLogistics(state, dt);
  tickFieldWaterLogistics(state, dt);
  updateUnits(state, dt);
  updateProjectiles(state, dt);
  updateCaptiveRansomTrickle(state, dt);
  tickDynastyIntelligenceReports(state);
  tickDynastyCounterIntelligence(state);
  tickMarriageDissolutionFromDeath(state);
  tickDynastyOperations(state);
  tickDynastyPoliticalEvents(state);
  tickFaithHolyWars(state, dt);
  tickFaithDivineRightDeclarations(state);
  tickTerritorialGovernanceRecognition(state, dt);
  tickMarriageGestation(state);
  tickMarriageProposalExpiration(state);
  tickLesserHouseCandidates(state);
  tickLesserHouseLoyaltyDrift(state);
  tickMinorHouseTerritorialLevies(state, dt);
  tickAssaultCohesion(state, dt);
  tickRealmConditionCycle(state, dt);
  updateMatchProgressionState(state);
  restoreDisplacedMembers(state);
  updateMessages(state, dt);
  return state;
}

export function getAvailablePopulation(state, factionId) {
  const faction = getFaction(state, factionId);
  return faction.population.total - getUsedPopulation(state, factionId) - faction.population.reserved;
}

function isUnitPrototypeTrainableForFaction(content, faction, unitDef) {
  if (!unitDef?.prototypeEnabled) {
    return false;
  }
  if (unitDef.house && faction?.houseId !== unitDef.house) {
    return false;
  }
  return true;
}

export function getTrainableUnitIdsForBuilding(state, buildingId) {
  const building = getEntity(state, "building", buildingId);
  if (!building || !building.completed || building.health <= 0) {
    return [];
  }
  const buildingDef = getBuildingDef(state.content, building.typeId);
  const faction = getFaction(state, building.factionId);
  return (buildingDef.trainableUnits ?? []).filter((unitId) =>
    isUnitPrototypeTrainableForFaction(state.content, faction, getUnitDef(state.content, unitId)));
}

export function queueProduction(state, buildingId, unitId) {
  const building = getEntity(state, "building", buildingId);
  if (!building || !building.completed || building.health <= 0) {
    return { ok: false, reason: "Building is unavailable." };
  }

  const buildingDef = getBuildingDef(state.content, building.typeId);
  if (!buildingDef.trainableUnits?.includes(unitId)) {
    return { ok: false, reason: "Unit cannot be trained here." };
  }

  const faction = getFaction(state, building.factionId);
  const unitDef = getUnitDef(state.content, unitId);
  if (!isUnitPrototypeTrainableForFaction(state.content, faction, unitDef)) {
    const requiredHouseName = unitDef?.house ? getHouse(state.content, unitDef.house)?.name ?? unitDef.house : null;
    return {
      ok: false,
      reason: requiredHouseName
        ? `${unitDef.name} is reserved to ${requiredHouseName}.`
        : `${unitDef?.name ?? unitId} is not enabled in the prototype.`,
    };
  }
  if (!canAfford(faction, unitDef.cost)) {
    return { ok: false, reason: "Not enough resources." };
  }

  if (getAvailablePopulation(state, building.factionId) < unitDef.populationCost) {
    return { ok: false, reason: "No available population." };
  }

  // Session 20: faith-unit training guards. A unit tagged with faithId + doctrinePath
  // can only be recruited by a faction committed to that exact covenant + path.
  if (unitDef.faithId) {
    if (faction.faith?.selectedFaithId !== unitDef.faithId) {
      return { ok: false, reason: `This unit requires commitment to ${unitDef.faithId.replace(/_/g, " ")}.` };
    }
    if (unitDef.doctrinePath && faction.faith?.doctrinePath !== unitDef.doctrinePath) {
      return { ok: false, reason: `This unit requires ${unitDef.doctrinePath}-path doctrine.` };
    }
    ensureFaithCovenantTestCompletionFromLegacyState(state, building.factionId);
    if (unitDef.stage >= 5 && !(faction.faith?.covenantTestPassed ?? false)) {
      return { ok: false, reason: "The Covenant Test must be completed before apex covenant units can be raised." };
    }
  }

  const bloodPrice = faction.houseId === "ironmark" && unitDef.role !== "worker"
    ? Math.max(0, unitDef.ironmarkBloodPrice ?? 1)
    : 0;
  const bloodProductionLoadDelta = faction.houseId === "ironmark" && unitDef.role !== "worker"
    ? Math.max(0, unitDef.bloodProductionLoadDelta ?? 1.5)
    : 0;
  if (bloodPrice > 0 && faction.population.total - bloodPrice < getUsedPopulation(state, building.factionId) + faction.population.reserved + unitDef.populationCost) {
    return { ok: false, reason: "Ironmark blood levy requires more living population." };
  }

  spendResources(faction, unitDef.cost);
  if (bloodPrice > 0) {
    faction.population.total = Math.max(0, faction.population.total - bloodPrice);
    recordConvictionEvent(state, building.factionId, "ruthlessness", 1, `Paid Ironmark blood levy for ${unitDef.name}`);
    // Session 15: track sustained blood levy load for growth-tempo depression
    // and canonical conviction drift downstream.
    faction.bloodProductionLoad = (faction.bloodProductionLoad ?? 0) + bloodProductionLoadDelta;
    if (faction.bloodProductionLoad > 12 && (faction.bloodProductionLastDriftAt ?? -999) + 40 < state.meta.elapsed) {
      // Sustained-war conviction drift: extra ruthlessness event beyond the
      // per-unit one, reflecting canonical master doctrine section IV/VIII costs.
      recordConvictionEvent(state, building.factionId, "ruthlessness", 2, `Sustained Ironmark blood levy under attritional war`);
      faction.bloodProductionLastDriftAt = state.meta.elapsed;
    }
    pushMessage(
      state,
      `${getFactionDisplayName(state, building.factionId)} paid a blood levy to raise ${unitDef.name}.`,
      building.factionId === "player" ? "warn" : "info",
    );
  }
  faction.population.reserved += unitDef.populationCost;
  building.productionQueue.push({
    unitId,
    remaining: 9 + unitDef.populationCost * 2,
    populationCost: unitDef.populationCost,
  });
  return { ok: true };
}

export function issueMoveCommand(state, unitIds, x, y) {
  const selectedUnits = state.units.filter((unit) => unitIds.includes(unit.id) && unit.health > 0);
  const tileSize = state.world.tileSize;
  selectedUnits.forEach((unit, index) => {
    const unitDef = getUnitDef(state.content, unit.typeId);
    // Session 27: movement domain gate. Vessels (water domain) cannot be ordered
    // onto land; land units (default domain) cannot be ordered onto water.
    // Canonical: ships do not walk; infantry do not swim.
    const destTileX = Math.floor(x / tileSize);
    const destTileY = Math.floor(y / tileSize);
    const destIsWater = isWaterTileAt(state, destTileX, destTileY);
    if (unitDef?.movementDomain === "water" && !destIsWater) {
      // Vessel ordered onto land; ignore the command.
      return;
    }
    if (unitDef?.movementDomain !== "water" && destIsWater) {
      // Land unit ordered onto water; ignore (no transport mechanic yet).
      return;
    }
    const offset = selectedUnits.length > 1
      ? {
          x: ((index % 4) - 1.5) * 16,
          y: (Math.floor(index / 4) - 0.5) * 16,
        }
      : { x: 0, y: 0 };
    clearFortificationDuty(unit);
    unit.command = { type: "move", x: x + offset.x, y: y + offset.y };
  });
}

export function issueGatherCommand(state, workerIds, nodeId) {
  state.units
    .filter((unit) => workerIds.includes(unit.id) && unit.health > 0)
    .forEach((unit) => {
      clearFortificationDuty(unit);
      unit.command = { type: "gather", nodeId };
      unit.carrying = null;
      unit.gatherProgress = 0;
    });
}

export function issueAttackCommand(state, unitIds, targetType, targetId) {
  state.units
    .filter((unit) => unitIds.includes(unit.id) && unit.health > 0)
    .forEach((unit) => {
      clearFortificationDuty(unit);
      unit.command = { type: "attack", targetType, targetId };
    });
}

export function issueRaidCommand(state, unitIds, targetType, targetId) {
  if (!["building", "resource", "unit"].includes(targetType)) {
    return { ok: false, reason: "Scout riders can only raid exposed infrastructure, moving logistics carriers, workers, or resource seams in the live browser lane." };
  }
  const target = targetType === "resource"
    ? state.world.resourceNodes.find((resourceNode) => resourceNode.id === targetId)
    : getEntity(state, targetType, targetId);
  if (!target || ((targetType === "building" || targetType === "unit") && target.health <= 0)) {
    return { ok: false, reason: "Raid target is unavailable." };
  }
  if (targetType === "building" && !getBuildingDef(state.content, target.typeId)?.scoutRaidable) {
    return { ok: false, reason: "This structure cannot be raided by scout riders." };
  }
  if (targetType === "unit" && !isScoutRaidableUnitDef(getUnitDef(state.content, target.typeId))) {
    return { ok: false, reason: "Scout riders only harass hostile workers or exposed moving logistics carriers directly." };
  }
  const assignedUnits = state.units
    .filter((unit) => unitIds.includes(unit.id) && unit.health > 0)
    .filter((unit) => isScoutRiderUnitDef(getUnitDef(state.content, unit.typeId)));
  if (assignedUnits.length === 0) {
    return { ok: false, reason: "Select scout riders to launch a raid." };
  }
  const targetFactionId = targetType === "resource"
    ? getScoutResourceHarassTargetFactionId(state, target, assignedUnits[0].factionId)
    : target.factionId;
  if (!targetFactionId || !areHostile(state, assignedUnits[0].factionId, targetFactionId)) {
    return { ok: false, reason: "Scout riders can only raid hostile infrastructure." };
  }
  assignedUnits.forEach((unit) => {
    clearFortificationDuty(unit);
    unit.command = { type: "raid", targetType, targetId };
  });
  return { ok: true };
}

// Session 11: House mechanics. Applies house-specific cost modifiers to building construction.
// Stonehelm's canonical fortification cost discount (20%) applies to fortification-role
// buildings. Future houses register here. Ironmark has no cost modifier — its canonical
// asymmetry is Blood Production on unit training, not building cost.
function getEffectiveBuildingCost(state, faction, buildingDef) {
  const house = faction.houseId ? getHouse(state.content, faction.houseId) : null;
  const mechanics = house?.mechanics ?? null;
  if (!mechanics) return buildingDef.cost ?? {};
  const cost = { ...(buildingDef.cost ?? {}) };
  // Stonehelm: fortification-role buildings cost less.
  if (buildingDef.fortificationRole && mechanics.fortificationCostMultiplier) {
    for (const key of Object.keys(cost)) {
      cost[key] = Math.round(cost[key] * mechanics.fortificationCostMultiplier);
    }
  }
  return cost;
}

function getEffectiveBuildTime(state, faction, buildingDef) {
  const house = faction.houseId ? getHouse(state.content, faction.houseId) : null;
  const mechanics = house?.mechanics ?? null;
  const baseTime = buildingDef.buildTime ?? 0;
  if (!mechanics) return baseTime;
  // Stonehelm: fortifications build faster (canonical defensive-tendency profile).
  if (buildingDef.fortificationRole && mechanics.fortificationBuildSpeedMultiplier) {
    return baseTime / mechanics.fortificationBuildSpeedMultiplier;
  }
  return baseTime;
}

export function attemptPlaceBuilding(state, factionId, buildingTypeId, tileX, tileY, workerId) {
  const buildingDef = getBuildingDef(state.content, buildingTypeId);
  const faction = getFaction(state, factionId);
  const worker = getEntity(state, "unit", workerId);

  if (!worker || worker.health <= 0) {
    return { ok: false, reason: "Select a living Worker." };
  }
  if (!insideWorld(state, tileX, tileY, buildingDef.footprint)) {
    return { ok: false, reason: "Out of bounds." };
  }
  if (intersectsAnyBuilding(state, tileX, tileY, buildingDef.footprint)) {
    return { ok: false, reason: "Space is occupied." };
  }
  // Session 27: Harbor coastal adjacency gate. Canonical: harbors must border water.
  if (buildingDef.requiresCoastalAdjacency && !isFootprintAdjacentToWater(state, tileX, tileY, buildingDef.footprint)) {
    return { ok: false, reason: "Harbor must be placed adjacent to a water tile." };
  }
  const effectiveCost = getEffectiveBuildingCost(state, faction, buildingDef);
  if (!canAfford(faction, effectiveCost)) {
    return { ok: false, reason: "Not enough resources." };
  }

  // Session 17: Covenant Hall (and any future covenant building with
  // requiresFaithCommitment) cannot be placed unless the faction has committed to
  // a covenant. Canonical gate per master doctrine section XIX.
  if (buildingDef.requiresFaithCommitment && !faction.faith?.selectedFaithId) {
    return { ok: false, reason: "A covenant must be chosen before raising this covenant structure." };
  }
  // Session 22: Apex Covenant additionally requires sustained faith intensity
  // (Fervent tier 80+). Canonical anti-snowball + late-game divergence lock.
  if (buildingDef.requiresFaithIntensity && (faction.faith?.intensity ?? 0) < buildingDef.requiresFaithIntensity) {
    return { ok: false, reason: `Apex covenant requires faith intensity >= ${buildingDef.requiresFaithIntensity}.` };
  }
  if (buildingDef.faithRole === "apex") {
    ensureFaithCovenantTestCompletionFromLegacyState(state, factionId);
    if (!(faction.faith?.covenantTestPassed ?? false)) {
      return { ok: false, reason: "The Covenant Test must be completed before an Apex covenant can be raised." };
    }
  }

  spendResources(faction, effectiveCost);
  const buildingId = createEntityId(state, "building");
  state.buildings.push({
    id: buildingId,
    factionId,
    typeId: buildingTypeId,
    tileX,
    tileY,
    buildProgress: 0,
    completed: false,
    health: 1,
    productionQueue: [],
    raidedUntil: 0,
  });

  clearFortificationDuty(worker);
  worker.command = { type: "build", buildingId };
  return { ok: true, buildingId };
}

export function pickWorldTarget(state, worldX, worldY) {
  const unit = [...state.units]
    .reverse()
    .find((candidate) => candidate.health > 0 && distance(worldX, worldY, candidate.x, candidate.y) <= candidate.radius + 4);
  if (unit) {
    return { type: "unit", entity: unit };
  }

  const building = [...state.buildings].reverse().find((candidate) => {
    if (candidate.health <= 0) {
      return false;
    }
    const rect = getBuildingWorldRect(state, candidate, getBuildingDef(state.content, candidate.typeId));
    return rectContainsPoint(rect, worldX, worldY);
  });
  if (building) {
    return { type: "building", entity: building };
  }

  const node = state.world.resourceNodes.find((resourceNode) => {
    const nodeX = resourceNode.x * state.world.tileSize;
    const nodeY = resourceNode.y * state.world.tileSize;
    return resourceNode.amount > 0 && distance(worldX, worldY, nodeX, nodeY) <= 20;
  });
  if (node) {
    return { type: "resource", entity: node };
  }

  return null;
}

export function findOpenBuildingSite(state, buildingTypeId, anchorTileX, anchorTileY, searchRadius = 8) {
  const def = getBuildingDef(state.content, buildingTypeId);
  for (let radius = 0; radius <= searchRadius; radius += 1) {
    for (let offsetY = -radius; offsetY <= radius; offsetY += 1) {
      for (let offsetX = -radius; offsetX <= radius; offsetX += 1) {
        const tileX = anchorTileX + offsetX;
        const tileY = anchorTileY + offsetY;
        if (insideWorld(state, tileX, tileY, def.footprint) && !intersectsAnyBuilding(state, tileX, tileY, def.footprint)) {
          return { tileX, tileY };
        }
      }
    }
  }
  return null;
}

export function getFactionSnapshot(state, factionId) {
  const faction = getFaction(state, factionId);
  ensureFaithCovenantTestCompletionFromLegacyState(state, factionId);
  const territories = state.world.controlPoints.filter((controlPoint) => controlPoint.ownerFactionId === factionId);
  const convictionBand = getConvictionBand(state.content, faction.conviction.score);
  const selectedFaith = faction.faith.selectedFaithId ? getFaith(state.content, faction.faith.selectedFaithId) : null;
  const doctrineEffects = getFaithDoctrineEffects(state, factionId);
  const activeDynastyOperations = faction.dynasty?.operations?.active ?? [];
  const activeHolyWars = getFaithHolyWarsState(faction)
    .filter((entry) => (entry.expiresAt ?? 0) > state.meta.elapsed);
  const incomingHolyWars = getIncomingHolyWars(state, factionId, state.meta.elapsed);
  const activeDivineRightDeclaration = getActiveDivineRightDeclaration(faction, state.meta.elapsed);
  const incomingDivineRightDeclarations = getIncomingDivineRightDeclarations(state, factionId, state.meta.elapsed);
  const divineRightProfile = getDivineRightDeclarationProfile(state, factionId);
  const siegeState = getFactionSiegeState(state, factionId);
  const dynastyPoliticalEvents = getDynastyPoliticalEventState(faction);
  const activeSuccessionCrisis = getActiveSuccessionCrisis(faction);
  const activeCovenantTest = getActiveCovenantTest(faction);
  const activeTerritorialGovernanceRecognition = getActiveTerritorialGovernanceRecognition(faction);
  const incomingCovenantTests = getIncomingCovenantTests(state, factionId);
  return {
    ...faction,
    displayName: getFactionDisplayName(state, factionId),
    presentation: getFactionPresentation(state, factionId),
    population: {
      ...faction.population,
      used: getUsedPopulation(state, factionId),
      available: getAvailablePopulation(state, factionId),
    },
    territories: {
      count: territories.length,
      names: territories.map((controlPoint) => controlPoint.name),
      details: territories.map((controlPoint) => ({
        id: controlPoint.id,
        name: controlPoint.name,
        loyalty: controlPoint.loyalty,
        controlState: controlPoint.controlState,
        governorMemberId: controlPoint.governorMemberId,
      })),
    },
    dynasty: faction.dynasty
      ? {
          ...faction.dynasty,
          interregnum: faction.dynasty.interregnum ?? false,
          attachments: {
            ...faction.dynasty.attachments,
            governorAssignments: (faction.dynasty.attachments.governorAssignments ?? []).map((assignment) => ({
              ...assignment,
              member: getDynastyMember(faction, assignment.memberId),
              specialization: getGovernorSpecializationProfile(assignment.specializationId),
              controlPoint: assignment.anchorType === "controlPoint"
                ? state.world.controlPoints.find((controlPoint) => controlPoint.id === assignment.anchorId) ?? null
                : null,
              settlement: assignment.anchorType === "settlement"
                ? getSettlementById(state, assignment.anchorId)
                : null,
              anchor: getGovernorAssignmentAnchor(state, assignment),
            })),
            commanderMember: getDynastyMember(faction, faction.dynasty.attachments.commanderMemberId),
            commanderUnit: faction.dynasty.attachments.commanderUnitId
              ? getEntity(state, "unit", faction.dynasty.attachments.commanderUnitId)
              : null,
            heirMember: getDynastyMember(faction, faction.dynasty.attachments.heirMemberId),
            fallenMembers: (faction.dynasty.attachments.fallenMembers ?? []).slice(0, FALLEN_LEDGER_LIMIT),
            capturedMembers: faction.dynasty.attachments.capturedMembers ?? {},
          },
          captives: (faction.dynasty.captives ?? []).map((captive) => ({
            ...captive,
            role: state.content.byId.bloodlineRoles[captive.roleId],
            path: state.content.byId.bloodlinePaths[captive.pathId],
            sourceFactionName: captive.sourceFactionName ?? getFactionDisplayName(state, captive.sourceFactionId),
            ransomDemand: getHeldCaptiveRansomTerms(state, factionId, captive.id),
          })),
          operations: {
            active: activeDynastyOperations.map((operation) => ({
              ...operation,
              targetFactionName: operation.targetFactionId ? getFactionDisplayName(state, operation.targetFactionId) : null,
              progress: getOperationProgress(operation, state.meta.elapsed),
            })),
            history: (faction.dynasty.operations?.history ?? []).slice(0, 5).map((operation) => ({
              ...operation,
              targetFactionName: operation.targetFactionId ? getFactionDisplayName(state, operation.targetFactionId) : null,
            })),
          },
          intelligenceReports: (faction.dynasty.intelligenceReports ?? []).map((report) => ({
            ...report,
            targetFactionName: getFactionDisplayName(state, report.targetFactionId),
            sourceFactionName: report.sourceFactionId ? getFactionDisplayName(state, report.sourceFactionId) : null,
            remainingSeconds: Math.max(0, (report.expiresAt ?? 0) - state.meta.elapsed),
            members: (report.members ?? []).map((member) => ({
              ...member,
              role: state.content.byId.bloodlineRoles[member.roleId] ?? null,
            })),
          })),
          counterIntelligence: getDynastyCounterIntelligenceState(faction).map((entry) => ({
            ...entry,
            remainingSeconds: Math.max(0, (entry.expiresAt ?? 0) - state.meta.elapsed),
            operator: getDynastyMember(faction, entry.operatorId),
            lastSourceFactionName: entry.lastSourceFactionId ? getFactionDisplayName(state, entry.lastSourceFactionId) : null,
            lastTargetMemberTitle: entry.lastTargetMemberId ? getDynastyMember(faction, entry.lastTargetMemberId)?.title ?? null : null,
          })),
          politicalEvents: {
            active: (dynastyPoliticalEvents?.active ?? []).map((entry) =>
              entry.type === "succession_crisis"
                ? serializeSuccessionCrisisEvent(state, factionId, entry)
                : entry.type === "covenant_test"
                  ? serializeCovenantTestEvent(state, factionId, entry)
                : { ...entry }),
            history: (dynastyPoliticalEvents?.history ?? []).slice(0, 4).map((entry) =>
              entry.type === "succession_crisis"
                ? serializeSuccessionCrisisEvent(state, factionId, entry)
                : entry.type === "covenant_test"
                  ? serializeCovenantTestEvent(state, factionId, entry)
                : { ...entry }),
          },
          activeSuccessionCrisis: activeSuccessionCrisis
            ? serializeSuccessionCrisisEvent(state, factionId, activeSuccessionCrisis)
            : null,
          activeCovenantTest: activeCovenantTest
            ? serializeCovenantTestEvent(state, factionId, activeCovenantTest)
            : null,
          activeTerritorialGovernanceRecognition: activeTerritorialGovernanceRecognition
            ? serializeTerritorialGovernanceRecognition(state, factionId, activeTerritorialGovernanceRecognition)
            : null,
          lastTerritorialGovernanceOutcome: faction.dynasty.lastTerritorialGovernanceOutcome
            ? { ...faction.dynasty.lastTerritorialGovernanceOutcome }
            : null,
          members: faction.dynasty.members.map((member) => ({
            ...member,
            role: state.content.byId.bloodlineRoles[member.roleId],
            path: state.content.byId.bloodlinePaths[member.pathId],
            capturedByName: member.capturedByFactionId ? getFactionDisplayName(state, member.capturedByFactionId) : null,
            activeOperation: activeDynastyOperations.find((operation) => operation.memberId === member.id)
              ? {
                  ...activeDynastyOperations.find((operation) => operation.memberId === member.id),
                  targetFactionName: getFactionDisplayName(
                    state,
                    activeDynastyOperations.find((operation) => operation.memberId === member.id).targetFactionId,
                  ),
                  progress: getOperationProgress(
                    activeDynastyOperations.find((operation) => operation.memberId === member.id),
                    state.meta.elapsed,
                  ),
                }
              : null,
            recoveryOptions: member.status === "captured"
              ? {
                  ransom: getCapturedMemberRansomTerms(state, factionId, member.id),
                  rescue: getCapturedMemberRescueTerms(state, factionId, member.id),
                }
              : null,
          })),
          marriageGovernance: buildMarriageGovernanceStatus(faction),
        }
      : null,
    conviction: {
      ...faction.conviction,
      bandId: convictionBand.id,
      bandLabel: convictionBand.label,
    },
    faith: {
      ...faction.faith,
      selectedFaith,
      doctrineEffects,
      activeHolyWars: activeHolyWars.map((entry) => ({
        ...entry,
        targetFactionName: getFactionDisplayName(state, entry.targetFactionId),
        faithName: getFaith(state.content, entry.faithId)?.name ?? entry.faithId,
        remainingSeconds: Math.max(0, (entry.expiresAt ?? 0) - state.meta.elapsed),
      })),
      incomingHolyWars: incomingHolyWars.map((entry) => ({
        ...entry,
        sourceFactionName: getFactionDisplayName(state, entry.sourceFactionId),
        faithName: getFaith(state.content, entry.faithId)?.name ?? entry.faithId,
        remainingSeconds: Math.max(0, (entry.expiresAt ?? 0) - state.meta.elapsed),
      })),
      activeDivineRightDeclaration: activeDivineRightDeclaration
        ? {
          ...activeDivineRightDeclaration,
          remainingSeconds: Math.max(0, (activeDivineRightDeclaration.resolveAt ?? 0) - state.meta.elapsed),
        }
        : null,
      incomingDivineRightDeclarations: incomingDivineRightDeclarations.map((entry) => ({
        ...entry,
        sourceFactionName: getFactionDisplayName(state, entry.sourceFactionId),
        remainingSeconds: Math.max(0, (entry.resolveAt ?? 0) - state.meta.elapsed),
      })),
      lastDivineRightOutcome: faction.faith?.lastDivineRightOutcome ? { ...faction.faith.lastDivineRightOutcome } : null,
      covenantTestPassed: Boolean(faction.faith?.covenantTestPassed),
      activeCovenantTest: activeCovenantTest
        ? serializeCovenantTestEvent(state, factionId, activeCovenantTest)
        : null,
      incomingCovenantTests: incomingCovenantTests.map((entry) => ({
        ...entry,
        sourceFactionName: getFactionDisplayName(state, entry.sourceFactionId),
      })),
      lastCovenantTestOutcome: faction.faith?.lastCovenantTestOutcome ? { ...faction.faith.lastCovenantTestOutcome } : null,
      divineRightRecognitionSharePct: divineRightProfile.recognitionSharePct,
      divineRightRequiredSharePct: divineRightProfile.requiredSharePct,
      divineRightApexStructureName: divineRightProfile.activeApexStructureName,
      divineRightEligible: getDivineRightDeclarationTerms(state, factionId).available,
      exposures: state.content.faiths.map((faith) => ({
        id: faith.id,
        name: faith.name,
        covenantName: faith.covenantName,
        exposure: faction.faith.exposure[faith.id] ?? 0,
        availableToCommit: (faction.faith.exposure[faith.id] ?? 0) >= FAITH_EXPOSURE_THRESHOLD,
        prototypeEffects: faith.prototypeEffects ?? null,
      })),
    },
    siege: {
      engineCount: siegeState.engineCount,
      engineerCount: siegeState.engineerCount,
      supplyWagonCount: siegeState.supplyWagonCount,
      supplyCampCount: siegeState.supplyCampCount,
      suppliedEngines: siegeState.suppliedEngines,
      engineerSupportedEngines: siegeState.engineerSupportedEngines,
      unsuppliedEngines: siegeState.unsuppliedEngines,
      readyForFormalAssault: siegeState.readyForFormalAssault,
    },
  };
}

export function chooseFaithCommitment(state, factionId, faithId, doctrinePath = "light") {
  const faction = getFaction(state, factionId);
  if (!faction || faction.kind !== "kingdom") {
    return { ok: false, reason: "Faction cannot commit to a covenant." };
  }
  if (faction.faith.selectedFaithId) {
    return { ok: false, reason: "Covenant already chosen." };
  }
  if ((faction.faith.exposure[faithId] ?? 0) < FAITH_EXPOSURE_THRESHOLD) {
    return { ok: false, reason: "Not enough covenant exposure yet." };
  }

  const tier = getFaithTier(20);
  faction.faith.selectedFaithId = faithId;
  faction.faith.doctrinePath = doctrinePath;
  faction.faith.intensity = 20;
  faction.faith.level = tier.level;
  faction.faith.tierLabel = tier.label;
  if (!faction.faith.discoveredFaithIds.includes(faithId)) {
    faction.faith.discoveredFaithIds.push(faithId);
  }

  const faith = getFaith(state.content, faithId);
  recordConvictionEvent(state, factionId, doctrinePath === "dark" ? "desecration" : "oathkeeping", 2, `Committed to ${faith.name} via ${doctrinePath} doctrine`);
  pushMessage(
    state,
    `${getFactionDisplayName(state, factionId)} aligned with ${faith.name} through ${doctrinePath} doctrine.`,
    factionId === "player" ? "good" : "info",
  );
  return { ok: true };
}

// --------------------------------------------------------------------------
// Session 10: Sabotage operations — covert operations system first expansion.
// Canonical reference: master doctrine section VIII (supply disruption and
// negative upkeep) + 08_MECHANICS/OPERATIONS_SYSTEM.md. Builds on the
// dynasty.operations architecture landed in Session 7 (ransom + rescue).
//
// Four canonical sub-types:
//   gate_opening    — targets a gatehouse; on success severely weakens it
//   fire_raising    — targets any enemy building; on success applies burn DOT
//   supply_poisoning — targets a supply_camp; on success disables supply
//   well_poisoning  — targets any settlement; on success adds water strain
// --------------------------------------------------------------------------

const SABOTAGE_COSTS = {
  gate_opening: { gold: 60, influence: 18 },
  fire_raising: { gold: 40, influence: 12 },
  supply_poisoning: { gold: 50, influence: 15 },
  well_poisoning: { gold: 70, influence: 20 },
};

const SABOTAGE_DURATIONS = {
  gate_opening: 28,
  fire_raising: 24,
  supply_poisoning: 30,
  well_poisoning: 32,
};

const COUNTER_INTELLIGENCE_COST = { gold: 60, influence: 18 };
const COUNTER_INTELLIGENCE_DURATION_SECONDS = 18;
const COUNTER_INTELLIGENCE_WATCH_DURATION_SECONDS = 150;
const COUNTER_INTELLIGENCE_GUARDED_ROLES = [
  "head_of_bloodline",
  "heir_designate",
  "commander",
  "spymaster",
  "governor",
];

const ESPIONAGE_COST = { gold: 45, influence: 16 };
const ASSASSINATION_COST = { gold: 85, influence: 28 };
const MISSIONARY_COST = { influence: 14 };
const HOLY_WAR_COST = { influence: 24 };
const ESPIONAGE_DURATION_SECONDS = 30;
const ASSASSINATION_DURATION_SECONDS = 34;
const MISSIONARY_DURATION_SECONDS = 32;
const HOLY_WAR_DECLARATION_DURATION_SECONDS = 18;
const INTELLIGENCE_REPORT_DURATION_SECONDS = 120;
const MISSIONARY_INTENSITY_COST = 12;
const HOLY_WAR_INTENSITY_COST = 18;
const HOLY_WAR_DURATION_SECONDS = 180;
const HOLY_WAR_PULSE_INTERVAL_SECONDS = 30;
const HOLY_WAR_SUSTAINED_LOYALTY_DRAIN_MULTIPLIER = 1.5;
const HOLY_WAR_SUSTAINED_LEGITIMACY_DRAIN_MULTIPLIER = 0.12;
const DIVINE_RIGHT_DECLARATION_DURATION_SECONDS = 180;
const DIVINE_RIGHT_GLOBAL_SHARE_THRESHOLD = 0.5;
const DIVINE_RIGHT_DECLARATION_MIN_STAGE = 5;
const DIVINE_RIGHT_INTENSITY_THRESHOLD = 80;
const DIVINE_RIGHT_FAILURE_INTENSITY_LOSS = 18;
const DIVINE_RIGHT_FAILURE_LEGITIMACY_LOSS = 10;
const DIVINE_RIGHT_RELAUNCH_COOLDOWN_SECONDS = 75;
const DIVINE_RIGHT_WORLD_PRESSURE_SCORE = 3;
const DIVINE_RIGHT_ATTACK_TIMER_CAP = 5;
const DIVINE_RIGHT_TERRITORY_TIMER_CAP = 4;
const DIVINE_RIGHT_RAID_TIMER_CAP = 7;
const DIVINE_RIGHT_MISSIONARY_TIMER_CAP = 6;
const DIVINE_RIGHT_HOLY_WAR_TIMER_CAP = 8;

const SABOTAGE_VALID_SUBTYPES = new Set(Object.keys(SABOTAGE_COSTS));

function validateSabotageTarget(subtype, buildingDef) {
  if (!buildingDef) return { ok: false, reason: "Unknown target building." };
  switch (subtype) {
    case "gate_opening":
      return buildingDef.fortificationRole === "gate"
        ? { ok: true }
        : { ok: false, reason: "Gate-opening must target a gatehouse." };
    case "fire_raising":
      return { ok: true };
    case "supply_poisoning":
      return buildingDef.supportsSiegeLogistics === true || buildingDef.id === "supply_camp"
        ? { ok: true }
        : { ok: false, reason: "Supply poisoning must target a supply camp." };
    case "well_poisoning":
      // Any enemy settlement anchor works (command hall, keep, or supply camp acts as anchor).
      return { ok: true };
    default:
      return { ok: false, reason: `Unknown sabotage type: ${subtype}` };
  }
}

export function getDossierBackedSabotageProfile(state, factionId, targetFactionId, reportOrId) {
  const faction = getFaction(state, factionId);
  const report = typeof reportOrId === "string"
    ? (faction?.dynasty?.intelligenceReports ?? []).find((entry) => entry.id === reportOrId) ?? null
    : reportOrId;
  if (!faction?.dynasty || !report || (report.sourceType ?? "espionage") !== "counter_intelligence") {
    return { available: false, reason: "No live counter-intelligence dossier is available." };
  }
  if (report.targetFactionId !== targetFactionId) {
    return { available: false, reason: "Dossier target does not match the requested hostile court." };
  }
  if ((report.expiresAt ?? 0) <= state.meta.elapsed) {
    return { available: false, reason: "Counter-intelligence dossier has expired." };
  }

  const hostileBuildings = state.buildings.filter((building) =>
    building.factionId === targetFactionId &&
    building.health > 0 &&
    building.completed);
  if (hostileBuildings.length === 0) {
    return { available: false, reason: "No completed hostile buildings are available for dossier-backed sabotage." };
  }

  const findTarget = (...typeIds) => hostileBuildings.find((building) => typeIds.includes(building.typeId)) ?? null;
  let subtype = "fire_raising";
  let targetBuilding = null;
  let retaliationReason = "Dossier-backed retaliation targets the rival court's live infrastructure.";
  const interceptType = report.interceptType ?? null;

  switch (interceptType) {
    case "assassination":
      targetBuilding = findTarget("command_hall", "gatehouse", "barracks");
      subtype = targetBuilding?.typeId === "gatehouse" ? "gate_opening" : "fire_raising";
      retaliationReason = "Dossier-backed retaliation answers an intercepted assassination by striking the rival court seat and command surface.";
      break;
    case "espionage":
      targetBuilding = findTarget("command_hall", "barracks", "farm");
      subtype = "fire_raising";
      retaliationReason = "Dossier-backed retaliation answers intercepted espionage by burning the rival court's working infrastructure.";
      break;
    default:
      targetBuilding = findTarget("supply_camp", "gatehouse", "well", "farm", "barracks");
      subtype = targetBuilding?.typeId === "supply_camp"
        ? "supply_poisoning"
        : targetBuilding?.typeId === "gatehouse"
          ? "gate_opening"
          : targetBuilding?.typeId === "well"
            ? "well_poisoning"
            : "fire_raising";
      retaliationReason = "Dossier-backed retaliation exploits the rival court's exposed infrastructure through sabotage.";
      break;
  }

  if (!targetBuilding) {
    return { available: false, reason: "No dossier-backed sabotage target could be resolved." };
  }

  const buildingDef = getBuildingDef(state.content, targetBuilding.typeId);
  const targetCheck = validateSabotageTarget(subtype, buildingDef);
  if (!targetCheck.ok) {
    return { available: false, reason: targetCheck.reason };
  }

  const intelligenceSupportBonus = Math.max(
    4,
    Math.min(
      14,
      (interceptType === "assassination" ? 8 : 6) + Math.max(0, ((report.interceptCount ?? 1) - 1) * 2),
    ),
  );

  return {
    available: true,
    intelligenceReportId: report.id,
    interceptType,
    subtype,
    targetBuildingId: targetBuilding.id,
    targetBuildingName: buildingDef?.name ?? targetBuilding.typeId,
    targetBuildingTypeId: targetBuilding.typeId,
    intelligenceSupportBonus,
    retaliationReason,
  };
}

function getSabotageTerms(state, factionId, subtype, targetFactionId, targetBuildingId, options = {}) {
  if (!SABOTAGE_VALID_SUBTYPES.has(subtype)) {
    return { available: false, reason: `Unknown sabotage type: ${subtype}` };
  }
  const faction = getFaction(state, factionId);
  const targetFaction = getFaction(state, targetFactionId);
  if (!faction?.dynasty || !targetFaction) {
    return { available: false, reason: "Invalid factions for sabotage." };
  }
  if (factionId === targetFactionId) {
    return { available: false, reason: "Cannot sabotage your own holdings." };
  }
  const targetBuilding = state.buildings.find((b) => b.id === targetBuildingId);
  if (!targetBuilding || targetBuilding.factionId !== targetFactionId || targetBuilding.health <= 0) {
    return { available: false, reason: "Sabotage target is invalid." };
  }
  const buildingDef = getBuildingDef(state.content, targetBuilding.typeId);
  const targetCheck = validateSabotageTarget(subtype, buildingDef);
  if (!targetCheck.ok) {
    return { available: false, reason: targetCheck.reason };
  }
  const spymaster = getDynastyMemberByRole(faction, ["spymaster", "diplomat", "covert_operations"]);
  if (!spymaster) {
    return { available: false, reason: "A Spymaster is required to run sabotage operations." };
  }
  const cost = SABOTAGE_COSTS[subtype];
  if (!hasResourcesAvailable(faction.resources, cost)) {
    return { available: false, reason: "Insufficient resources for sabotage." };
  }
  const duration = SABOTAGE_DURATIONS[subtype];
  const intelligenceSupportBonus = Math.max(0, options.intelligenceSupportBonus ?? 0);

  // Success formula: spymaster renown + base offense 45 vs fort tier × 12 + ward active 15
  // + defender spymaster 10 + canonical floor. Keeps sabotage a real commitment.
  const nearestSettlement = findNearestSettlementByBuilding(state, targetBuilding);
  const fortTier = nearestSettlement?.fortificationTier ?? 0;
  const defenseProfile = nearestSettlement ? getSettlementDefenseState(state, nearestSettlement) : null;
  const wardActive = defenseProfile?.wardProfile && defenseProfile.wardProfile.id !== "none" ? 15 : 0;
  const targetSpymaster = getDynastyMemberByRole(targetFaction, ["spymaster", "diplomat"]);
  const defenseScore = fortTier * 12 + wardActive + (targetSpymaster ? 10 : 0);
  const offenseScore = (spymaster.renown ?? 20) + 45 + intelligenceSupportBonus;
  const successScore = offenseScore - defenseScore;
  const projectedChance = Math.max(0.05, Math.min(0.95, 0.5 + successScore / 100));

  return {
    available: true,
    cost,
    duration,
    spymasterId: spymaster.id,
    successScore,
    projectedChance,
    targetBuildingName: buildingDef.name,
    intelligenceReportId: options.intelligenceReportId ?? null,
    intelligenceSupportBonus,
    retaliationReason: options.retaliationReason ?? null,
    retaliationInterceptType: options.retaliationInterceptType ?? null,
  };
}

export function getSabotageOperationTerms(state, factionId, subtype, targetFactionId, targetBuildingId, options = {}) {
  return getSabotageTerms(state, factionId, subtype, targetFactionId, targetBuildingId, options);
}

function hasResourcesAvailable(resources, cost) {
  return Object.entries(cost ?? {}).every(
    ([key, value]) => (resources[key] ?? 0) >= value,
  );
}

function findNearestSettlementByBuilding(state, building) {
  const settlements = state.world.settlements ?? [];
  if (settlements.length === 0 || !building) return null;
  const def = getBuildingDef(state.content, building.typeId);
  const center = getBuildingCenter(state, building, def);
  let best = null;
  let bestDist = Infinity;
  for (const s of settlements) {
    if (s.factionId !== building.factionId) continue;
    const d = distance(center.x, center.y, s.x * state.world.tileSize, s.y * state.world.tileSize);
    if (d < bestDist) {
      best = s;
      bestDist = d;
    }
  }
  return best;
}

function buildCounterIntelligenceTerms(state, factionId) {
  const faction = getFaction(state, factionId);
  if (!faction?.dynasty) {
    return null;
  }
  const operator = getDynastyMemberByRole(faction, ["spymaster", "diplomat", "head_of_bloodline"]);
  if (!operator) {
    return null;
  }
  const primarySeat = getPrimaryKeepSeat(state, factionId);
  const wardProfile = primarySeat ? getFortificationFaithWardProfile(state, primarySeat) : DEFAULT_FORTIFICATION_WARD;
  const courtLoyalty = getDynastyCourtLoyaltyProfile(state, factionId);
  const legitimacy = faction.dynasty.legitimacy ?? 0;
  const loyaltySupport = Math.max(0, (courtLoyalty.averageLoyalty - 50) * 0.08);
  const legitimacySupport = Math.max(0, (legitimacy - 55) * 0.05);
  const instabilityPenalty = Math.max(0, (58 - courtLoyalty.weakestLoyalty) * 0.12);
  const watchStrength = Math.max(
    8,
    Math.round(
      (operator.renown ?? 8) * 0.5 +
      10 +
      (primarySeat?.fortificationTier ?? 0) * 3 +
      (wardProfile.id !== "none" ? 5 : 0) +
      loyaltySupport +
      legitimacySupport -
      instabilityPenalty,
    ),
  );
  return {
    cost: COUNTER_INTELLIGENCE_COST,
    duration: COUNTER_INTELLIGENCE_DURATION_SECONDS,
    watchDuration: COUNTER_INTELLIGENCE_WATCH_DURATION_SECONDS,
    operatorId: operator.id,
    operatorTitle: operator.title,
    wardLabel: wardProfile.label,
    watchStrength,
    averageLoyalty: courtLoyalty.averageLoyalty,
    weakestLoyalty: courtLoyalty.weakestLoyalty,
    guardedRoles: COUNTER_INTELLIGENCE_GUARDED_ROLES.slice(),
  };
}

function getDynastyCounterIntelligenceProfile(state, targetFactionId, sourceFactionId = null, targetMemberId = null) {
  const targetFaction = getFaction(state, targetFactionId);
  if (!targetFaction?.dynasty) {
    return {
      active: false,
      entry: null,
      totalBonus: 0,
      roleGuardBonus: 0,
      hostilitySupport: 0,
      instabilityPenalty: 0,
      averageLoyalty: 50,
      weakestLoyalty: 50,
    };
  }

  const entry = getActiveCounterIntelligence(targetFaction, state.meta.elapsed);
  if (!entry) {
    return {
      active: false,
      entry: null,
      totalBonus: 0,
      roleGuardBonus: 0,
      hostilitySupport: 0,
      instabilityPenalty: 0,
      averageLoyalty: 50,
      weakestLoyalty: 50,
    };
  }

  const loyaltyProfile = getDynastyCourtLoyaltyProfile(state, targetFactionId);
  const legitimacy = targetFaction.dynasty.legitimacy ?? 0;
  const targetMember = targetMemberId ? getDynastyMember(targetFaction, targetMemberId) : null;
  const roleGuardBonus = targetMember ? getCounterIntelligenceRoleGuardBonus(targetMember.roleId) : 0;
  const hostilitySupport = sourceFactionId && (targetFaction.hostileTo ?? []).includes(sourceFactionId) ? 4 : 0;
  const loyaltySupport = Math.max(0, (loyaltyProfile.averageLoyalty - 50) * 0.06);
  const legitimacySupport = Math.max(0, (legitimacy - 55) * 0.04);
  const instabilityPenalty = Math.max(0, (58 - loyaltyProfile.weakestLoyalty) * 0.14);
  const totalBonus = Math.max(
    0,
    Math.round(
      (entry.watchStrength ?? 0) +
      loyaltySupport +
      legitimacySupport +
      hostilitySupport +
      roleGuardBonus -
      instabilityPenalty,
    ),
  );

  return {
    active: true,
    entry,
    roleGuardBonus,
    hostilitySupport,
    instabilityPenalty,
    averageLoyalty: loyaltyProfile.averageLoyalty,
    weakestLoyalty: loyaltyProfile.weakestLoyalty,
    totalBonus,
  };
}

function recordCounterIntelligenceInterception(state, targetFactionId, sourceFactionId, type, targetMemberId = null) {
  const targetFaction = getFaction(state, targetFactionId);
  const entry = getActiveCounterIntelligence(targetFaction, state.meta.elapsed);
  if (!entry) {
    return null;
  }
  entry.sourceInterceptions = entry.sourceInterceptions ?? [];
  entry.interceptions = (entry.interceptions ?? 0) + 1;
  entry.lastInterceptAt = state.meta.elapsed;
  entry.lastSourceFactionId = sourceFactionId ?? null;
  entry.lastTargetMemberId = targetMemberId ?? null;
  entry.lastInterceptType = type;
  const sourceInterception = sourceFactionId
    ? entry.sourceInterceptions.find((candidate) => candidate.sourceFactionId === sourceFactionId) ?? null
    : null;
  const sourceEntry = sourceInterception ?? (sourceFactionId
    ? {
        sourceFactionId,
        interceptions: 0,
        foiledEspionage: 0,
        foiledAssassinations: 0,
        lastInterceptAt: null,
        lastInterceptType: null,
        lastTargetMemberId: null,
        lastDossierAt: null,
      }
    : null);
  if (sourceEntry && !sourceInterception) {
    entry.sourceInterceptions.unshift(sourceEntry);
  }
  if (sourceEntry) {
    sourceEntry.interceptions = (sourceEntry.interceptions ?? 0) + 1;
    sourceEntry.lastInterceptAt = state.meta.elapsed;
    sourceEntry.lastInterceptType = type;
    sourceEntry.lastTargetMemberId = targetMemberId ?? null;
  }
  if (type === "assassination") {
    entry.foiledAssassinations = (entry.foiledAssassinations ?? 0) + 1;
    if (sourceEntry) {
      sourceEntry.foiledAssassinations = (sourceEntry.foiledAssassinations ?? 0) + 1;
    }
    adjustLegitimacy(targetFaction, 2);
  } else if (type === "espionage") {
    entry.foiledEspionage = (entry.foiledEspionage ?? 0) + 1;
    if (sourceEntry) {
      sourceEntry.foiledEspionage = (sourceEntry.foiledEspionage ?? 0) + 1;
    }
    adjustLegitimacy(targetFaction, 1);
  }
  const dossier = sourceFactionId
    ? createDynastyIntelligenceReport(state, targetFactionId, sourceFactionId, {
        sourceType: "counter_intelligence",
        reportLabel: "Counter-intelligence dossier",
        interceptType: type,
        interceptCount: sourceEntry?.interceptions ?? 1,
      })
    : null;
  if (dossier) {
    storeDynastyIntelligenceReport(targetFaction, dossier);
    if (sourceEntry) {
      sourceEntry.lastDossierAt = state.meta.elapsed;
    }
  }
  return {
    ...entry,
    dossierId: dossier?.id ?? null,
    dossierIssued: Boolean(dossier),
  };
}

function createCounterIntelligenceWatch(state, factionId, operation) {
  const faction = getFaction(state, factionId);
  if (!faction?.dynasty) {
    return null;
  }
  return {
    id: createEntityId(state, "dynastyCounter"),
    activatedAt: state.meta.elapsed,
    expiresAt: state.meta.elapsed + (operation.watchDuration ?? COUNTER_INTELLIGENCE_WATCH_DURATION_SECONDS),
    operatorId: operation.operatorId,
    operatorTitle: operation.operatorTitle,
    watchStrength: operation.watchStrength ?? 0,
    wardLabel: operation.wardLabel ?? DEFAULT_FORTIFICATION_WARD.label,
    guardedRoles: (operation.guardedRoles ?? COUNTER_INTELLIGENCE_GUARDED_ROLES).slice(),
    averageLoyalty: operation.averageLoyalty ?? 50,
    weakestLoyalty: operation.weakestLoyalty ?? 50,
    interceptions: 0,
    foiledEspionage: 0,
    foiledAssassinations: 0,
    lastInterceptAt: null,
    lastInterceptType: null,
    lastSourceFactionId: null,
    lastTargetMemberId: null,
    sourceInterceptions: [],
  };
}

function getEspionageContest(state, factionId, targetFactionId) {
  const faction = getFaction(state, factionId);
  const targetFaction = getFaction(state, targetFactionId);
  const spymaster = getDynastyMemberByRole(faction, ["spymaster", "diplomat", "merchant"]);
  const targetSpymaster = getDynastyMemberByRole(targetFaction, ["spymaster", "diplomat"]);
  const targetSeat = getPrimaryKeepSeat(state, targetFactionId);
  const wardProfile = targetSeat ? getFortificationFaithWardProfile(state, targetSeat) : DEFAULT_FORTIFICATION_WARD;
  const wardDefense = wardProfile.id !== "none" ? 8 : 0;
  const counterIntel = getDynastyCounterIntelligenceProfile(state, targetFactionId, factionId);
  const offenseScore = (spymaster?.renown ?? 0) + 32;
  const defenseScore =
    (targetSpymaster?.renown ?? 0) * 0.55 +
    (targetSeat?.fortificationTier ?? 0) * 6 +
    wardDefense +
    counterIntel.totalBonus;
  const successScore = offenseScore - defenseScore;
  const projectedChance = Math.max(0.08, Math.min(0.92, 0.5 + successScore / 100));
  return {
    spymasterId: spymaster?.id ?? null,
    successScore,
    projectedChance,
    counterIntelligenceActive: counterIntel.active,
    counterIntelligenceDefense: counterIntel.totalBonus,
    counterIntelligenceIntercepts: counterIntel.entry?.interceptions ?? 0,
  };
}

function getAssassinationContest(state, factionId, targetFactionId, targetMemberId) {
  const faction = getFaction(state, factionId);
  const targetFaction = getFaction(state, targetFactionId);
  const targetMember = getDynastyMember(targetFaction, targetMemberId);
  const spymaster = getDynastyMemberByRole(faction, ["spymaster", "diplomat", "merchant"]);
  const activeIntel = getActiveIntelligenceReport(faction, targetFactionId, state.meta.elapsed);
  const locationProfile = getDynastyMemberLocationProfile(state, targetFactionId, targetMemberId);
  const targetSpymaster = getDynastyMemberByRole(targetFaction, ["spymaster", "diplomat"]);
  const wardDefense = locationProfile.wardProfile?.id !== "none" ? 12 : 0;
  const intelBonus = activeIntel ? 12 : 0;
  const counterIntel = getDynastyCounterIntelligenceProfile(state, targetFactionId, factionId, targetMemberId);
  const offenseScore = (spymaster?.renown ?? 0) + 36 + intelBonus + locationProfile.exposureBonus;
  const defenseScore =
    (targetSpymaster?.renown ?? 0) * 0.55 +
    (locationProfile.keepTier ?? 0) * 7 +
    wardDefense +
    (locationProfile.bloodlineProtectionBonus ?? 0) +
    (targetMember?.roleId === "head_of_bloodline" ? 8 : 0) +
    counterIntel.totalBonus;
  const successScore = offenseScore - defenseScore;
  const projectedChance = Math.max(0.06, Math.min(0.9, 0.5 + successScore / 100));
  return {
    spymasterId: spymaster?.id ?? null,
    targetMemberTitle: targetMember?.title ?? "Unknown target",
    locationLabel: locationProfile.label,
    intelSupport: Boolean(activeIntel),
    successScore,
    projectedChance,
    counterIntelligenceActive: counterIntel.active,
    counterIntelligenceDefense: counterIntel.totalBonus,
    bloodlineGuardBonus: counterIntel.roleGuardBonus,
  };
}

export function getEspionageTerms(state, factionId, targetFactionId) {
  const faction = getFaction(state, factionId);
  const targetFaction = getFaction(state, targetFactionId);
  if (!faction?.dynasty || !targetFaction?.dynasty) {
    return { available: false, reason: "Invalid factions for espionage." };
  }
  if (factionId === targetFactionId) {
    return { available: false, reason: "Cannot infiltrate your own court." };
  }
  if (getActiveDynastyOperationForTargetFaction(faction, "espionage", targetFactionId)) {
    return { available: false, reason: "An espionage web is already active against this court." };
  }
  if (getActiveIntelligenceReport(faction, targetFactionId, state.meta.elapsed)) {
    return { available: false, reason: "This court is already under live surveillance." };
  }
  const spymaster = getDynastyMemberByRole(faction, ["spymaster", "diplomat", "merchant"]);
  if (!spymaster) {
    return { available: false, reason: "A Spymaster is required to run espionage." };
  }
  if (!hasResourcesAvailable(faction.resources, ESPIONAGE_COST)) {
    return { available: false, reason: "Insufficient resources for espionage." };
  }
  const contest = getEspionageContest(state, factionId, targetFactionId);
  return {
    available: true,
    cost: ESPIONAGE_COST,
    duration: ESPIONAGE_DURATION_SECONDS,
    reportDuration: INTELLIGENCE_REPORT_DURATION_SECONDS,
    spymasterId: spymaster.id,
    successScore: contest.successScore,
    projectedChance: contest.projectedChance,
    counterIntelligenceActive: contest.counterIntelligenceActive,
    counterIntelligenceDefense: contest.counterIntelligenceDefense,
  };
}

export function getAssassinationTerms(state, factionId, targetFactionId, targetMemberId) {
  const faction = getFaction(state, factionId);
  const targetFaction = getFaction(state, targetFactionId);
  if (!faction?.dynasty || !targetFaction?.dynasty) {
    return { available: false, reason: "Invalid factions for assassination." };
  }
  if (factionId === targetFactionId) {
    return { available: false, reason: "Cannot target your own bloodline." };
  }
  const targetMember = getDynastyMember(targetFaction, targetMemberId);
  if (!targetMember || !isMemberAvailable(targetMember)) {
    return { available: false, reason: "Target member is not available." };
  }
  if (getActiveDynastyOperationForTargetMember(faction, "assassination", targetFactionId, targetMemberId)) {
    return { available: false, reason: "An assassination cell is already moving on this target." };
  }
  const spymaster = getDynastyMemberByRole(faction, ["spymaster", "diplomat", "merchant"]);
  if (!spymaster) {
    return { available: false, reason: "A Spymaster is required to order an assassination." };
  }
  if (!hasResourcesAvailable(faction.resources, ASSASSINATION_COST)) {
    return { available: false, reason: "Insufficient resources for assassination." };
  }
  const contest = getAssassinationContest(state, factionId, targetFactionId, targetMemberId);
  return {
    available: true,
    cost: ASSASSINATION_COST,
    duration: ASSASSINATION_DURATION_SECONDS,
    spymasterId: spymaster.id,
    targetMemberTitle: contest.targetMemberTitle,
    locationLabel: contest.locationLabel,
    intelSupport: contest.intelSupport,
    successScore: contest.successScore,
    projectedChance: contest.projectedChance,
    counterIntelligenceActive: contest.counterIntelligenceActive,
    counterIntelligenceDefense: contest.counterIntelligenceDefense,
    bloodlineGuardBonus: contest.bloodlineGuardBonus,
  };
}

export function getCounterIntelligenceTerms(state, factionId) {
  const faction = getFaction(state, factionId);
  if (!faction?.dynasty) {
    return { available: false, reason: "Invalid faction for counter-intelligence." };
  }
  if (getActiveDynastyOperationByType(faction, "counter_intelligence")) {
    return { available: false, reason: "A counter-intelligence watch is already being raised." };
  }
  const activeWatch = getActiveCounterIntelligence(faction, state.meta.elapsed);
  if (activeWatch) {
    return {
      available: false,
      reason: `Counter-intelligence watch already active for ${Math.ceil((activeWatch.expiresAt ?? state.meta.elapsed) - state.meta.elapsed)}s.`,
      remainingSeconds: Math.max(0, (activeWatch.expiresAt ?? state.meta.elapsed) - state.meta.elapsed),
    };
  }
  const terms = buildCounterIntelligenceTerms(state, factionId);
  if (!terms) {
    return { available: false, reason: "A Spymaster, Diplomat, or ruling head is required to raise counter-intelligence." };
  }
  if (!hasResourcesAvailable(faction.resources, COUNTER_INTELLIGENCE_COST)) {
    return { available: false, reason: "Insufficient resources for counter-intelligence." };
  }
  return {
    available: true,
    cost: COUNTER_INTELLIGENCE_COST,
    duration: COUNTER_INTELLIGENCE_DURATION_SECONDS,
    watchDuration: COUNTER_INTELLIGENCE_WATCH_DURATION_SECONDS,
    operatorId: terms.operatorId,
    operatorTitle: terms.operatorTitle,
    wardLabel: terms.wardLabel,
    watchStrength: terms.watchStrength,
    averageLoyalty: terms.averageLoyalty,
    weakestLoyalty: terms.weakestLoyalty,
    guardedRoles: terms.guardedRoles,
  };
}

export function getMissionaryTerms(state, factionId, targetFactionId) {
  const faction = getFaction(state, factionId);
  const targetFaction = getFaction(state, targetFactionId);
  if (!faction?.dynasty || !targetFaction?.dynasty) {
    return { available: false, reason: "Invalid factions for missionary work." };
  }
  if (factionId === targetFactionId) {
    return { available: false, reason: "Cannot send missionaries into your own court." };
  }
  if (!faction.faith?.selectedFaithId) {
    return { available: false, reason: "A committed covenant is required to dispatch missionaries." };
  }
  if (targetFaction.faith?.selectedFaithId === faction.faith.selectedFaithId) {
    return { available: false, reason: "Target court already follows this covenant." };
  }
  if (getActiveDynastyOperationForTargetFaction(faction, "missionary", targetFactionId)) {
    return { available: false, reason: "Missionaries are already working inside this court." };
  }
  const operator = getFaithOperatorMember(faction);
  if (!operator) {
    return { available: false, reason: "An Ideological Leader or equivalent bloodline voice is required." };
  }
  if (!hasResourcesAvailable(faction.resources, MISSIONARY_COST)) {
    return { available: false, reason: "Insufficient influence for missionary work." };
  }
  if ((faction.faith.intensity ?? 0) < MISSIONARY_INTENSITY_COST) {
    return { available: false, reason: `Faith intensity must be at least ${MISSIONARY_INTENSITY_COST}.` };
  }
  const targetOperator = getFaithOperatorMember(targetFaction);
  const targetSeat = getPrimaryKeepSeat(state, targetFactionId);
  const wardProfile = targetSeat ? getFortificationFaithWardProfile(state, targetSeat) : DEFAULT_FORTIFICATION_WARD;
  const sourceIntensity = faction.faith.intensity ?? 0;
  const targetIntensity = targetFaction.faith.intensity ?? 0;
  const offenseScore = (operator.renown ?? 10) + sourceIntensity * 0.65 + (faction.faith.doctrinePath === "dark" ? 8 : 4);
  const defenseScore = (targetOperator?.renown ?? 0) * 0.6 + targetIntensity * 0.55 + (wardProfile.id !== "none" ? 10 : 0);
  const successScore = offenseScore - defenseScore;
  const projectedChance = Math.max(0.1, Math.min(0.9, 0.5 + successScore / 100));
  const exposureGain = Math.max(12, Math.round(14 + Math.max(0, successScore) * 0.22));
  const intensityErosion = targetFaction.faith?.selectedFaithId && targetFaction.faith.selectedFaithId !== faction.faith.selectedFaithId
    ? Math.max(4, Math.round(5 + Math.max(0, sourceIntensity - targetIntensity) / 12) + (faction.faith.doctrinePath === "dark" ? 2 : 0))
    : 0;
  const loyaltyPressure = targetFaction.faith?.selectedFaithId && targetFaction.faith.selectedFaithId !== faction.faith.selectedFaithId
    ? (faction.faith.doctrinePath === "dark" ? 4 : 2)
    : 1;
  return {
    available: true,
    cost: MISSIONARY_COST,
    intensityCost: MISSIONARY_INTENSITY_COST,
    duration: MISSIONARY_DURATION_SECONDS,
    operatorId: operator.id,
    operatorTitle: operator.title,
    sourceFaithId: faction.faith.selectedFaithId,
    sourceFaithName: getFaith(state.content, faction.faith.selectedFaithId)?.name ?? faction.faith.selectedFaithId,
    wardLabel: wardProfile.label,
    exposureGain,
    intensityErosion,
    loyaltyPressure,
    successScore,
    projectedChance,
  };
}

export function getHolyWarDeclarationTerms(state, factionId, targetFactionId) {
  const faction = getFaction(state, factionId);
  const targetFaction = getFaction(state, targetFactionId);
  if (!faction?.dynasty || !targetFaction?.dynasty) {
    return { available: false, reason: "Invalid factions for holy war declaration." };
  }
  if (factionId === targetFactionId) {
    return { available: false, reason: "Cannot declare holy war on your own court." };
  }
  if (!faction.faith?.selectedFaithId) {
    return { available: false, reason: "A committed covenant is required to declare holy war." };
  }
  if (!targetFaction.faith?.selectedFaithId) {
    return { available: false, reason: "Target court must first bind itself to a covenant." };
  }
  const compatibility = getMarriageFaithCompatibilityProfile(state, factionId, targetFactionId);
  if (compatibility.tier === "harmonious") {
    return { available: false, reason: "Holy war requires a real covenant fracture, not aligned doctrine." };
  }
  if (getActiveDynastyOperationForTargetFaction(faction, "holy_war", targetFactionId)) {
    return { available: false, reason: "A holy war declaration is already being carried to this court." };
  }
  if (getActiveHolyWar(faction, targetFactionId, state.meta.elapsed)) {
    return { available: false, reason: "This holy war is already active." };
  }
  const operator = getFaithOperatorMember(faction);
  if (!operator) {
    return { available: false, reason: "An Ideological Leader or equivalent bloodline voice is required." };
  }
  if (!hasResourcesAvailable(faction.resources, HOLY_WAR_COST)) {
    return { available: false, reason: "Insufficient influence for holy war declaration." };
  }
  if ((faction.faith.intensity ?? 0) < HOLY_WAR_INTENSITY_COST) {
    return { available: false, reason: `Faith intensity must be at least ${HOLY_WAR_INTENSITY_COST}.` };
  }
  return {
    available: true,
    cost: HOLY_WAR_COST,
    intensityCost: HOLY_WAR_INTENSITY_COST,
    duration: HOLY_WAR_DECLARATION_DURATION_SECONDS,
    warDuration: HOLY_WAR_DURATION_SECONDS,
    operatorId: operator.id,
    operatorTitle: operator.title,
    compatibilityLabel: compatibility.label,
    intensityPulse: faction.faith.doctrinePath === "dark" ? 1.2 : 0.9,
    loyaltyPulse: faction.faith.doctrinePath === "dark" ? 1.8 : 1.2,
  };
}

function applyMissionaryEffect(state, operation) {
  const sourceFaction = getFaction(state, operation.sourceFactionId);
  const targetFaction = getFaction(state, operation.targetFactionId);
  if (!sourceFaction?.faith?.selectedFaithId || !targetFaction?.faith) {
    return { ok: false, reason: "Missionary target no longer exists." };
  }

  const sourceFaithId = sourceFaction.faith.selectedFaithId;
  noteFaithDiscovery(targetFaction, sourceFaithId);
  const previousExposure = targetFaction.faith.exposure[sourceFaithId] ?? 0;
  targetFaction.faith.exposure[sourceFaithId] = clamp(previousExposure + (operation.exposureGain ?? 0), 0, 100);

  if (targetFaction.faith.selectedFaithId && targetFaction.faith.selectedFaithId !== sourceFaithId) {
    targetFaction.faith.intensity = Math.max(0, (targetFaction.faith.intensity ?? 0) - (operation.intensityErosion ?? 0));
    syncFaithIntensityState(targetFaction);
  }

  const pressurePoint = getLowestLoyaltyControlPoint(state, operation.targetFactionId);
  if (pressurePoint) {
    pressurePoint.loyalty = clamp(pressurePoint.loyalty - (operation.loyaltyPressure ?? 0), 0, 100);
  } else {
    adjustLegitimacy(targetFaction, -Math.max(1, Math.round((operation.loyaltyPressure ?? 1) * 0.5)));
  }

  return {
    ok: true,
    exposureBefore: previousExposure,
    exposureAfter: targetFaction.faith.exposure[sourceFaithId] ?? 0,
    pressurePointName: pressurePoint?.name ?? null,
  };
}

function createHolyWarEntry(state, sourceFactionId, targetFactionId, operation) {
  const sourceFaction = getFaction(state, sourceFactionId);
  if (!sourceFaction?.faith?.selectedFaithId) {
    return null;
  }
  return {
    id: createEntityId(state, "faithHolyWar"),
    targetFactionId,
    faithId: sourceFaction.faith.selectedFaithId,
    doctrinePath: sourceFaction.faith.doctrinePath ?? "light",
    declaredAt: state.meta.elapsed,
    lastPulseAt: state.meta.elapsed,
    expiresAt: state.meta.elapsed + (operation.warDuration ?? HOLY_WAR_DURATION_SECONDS),
    intensityPulse: operation.intensityPulse ?? 1,
    loyaltyPulse: operation.loyaltyPulse ?? 1,
  };
}

export function startMissionaryOperation(state, factionId, targetFactionId) {
  const faction = getFaction(state, factionId);
  const terms = getMissionaryTerms(state, factionId, targetFactionId);
  if (!terms.available) {
    return { ok: false, reason: terms.reason };
  }
  const operations = getDynastyOperationsState(faction);
  if ((operations.active ?? []).length >= DYNASTY_OPERATION_ACTIVE_LIMIT) {
    return { ok: false, reason: "Too many dynasty operations are already active." };
  }

  applyCost(faction.resources, terms.cost);
  spendFaithIntensity(faction, terms.intensityCost);
  const operation = {
    id: createEntityId(state, "dynastyOperation"),
    type: "missionary",
    sourceFactionId: factionId,
    targetFactionId,
    sourceFaithId: terms.sourceFaithId,
    sourceFaithName: terms.sourceFaithName,
    startedAt: state.meta.elapsed,
    resolveAt: state.meta.elapsed + terms.duration,
    operatorId: terms.operatorId,
    operatorTitle: terms.operatorTitle,
    exposureGain: terms.exposureGain,
    intensityErosion: terms.intensityErosion,
    loyaltyPressure: terms.loyaltyPressure,
    successScore: terms.successScore,
    projectedChance: terms.projectedChance,
    intensityCost: terms.intensityCost,
    escrowCost: terms.cost,
  };
  operations.active.unshift(operation);
  operations.active = operations.active.slice(0, DYNASTY_OPERATION_ACTIVE_LIMIT);
  pushMessage(
    state,
    `${getFactionDisplayName(state, factionId)} dispatches missionaries of ${terms.sourceFaithName} toward ${getFactionDisplayName(state, targetFactionId)}.`,
    factionId === "player" ? "info" : targetFactionId === "player" ? "warn" : "info",
  );
  return { ok: true, operation };
}

export function startHolyWarDeclaration(state, factionId, targetFactionId) {
  const faction = getFaction(state, factionId);
  const terms = getHolyWarDeclarationTerms(state, factionId, targetFactionId);
  if (!terms.available) {
    return { ok: false, reason: terms.reason };
  }
  const operations = getDynastyOperationsState(faction);
  if ((operations.active ?? []).length >= DYNASTY_OPERATION_ACTIVE_LIMIT) {
    return { ok: false, reason: "Too many dynasty operations are already active." };
  }

  applyCost(faction.resources, terms.cost);
  spendFaithIntensity(faction, terms.intensityCost);
  const operation = {
    id: createEntityId(state, "dynastyOperation"),
    type: "holy_war",
    sourceFactionId: factionId,
    targetFactionId,
    startedAt: state.meta.elapsed,
    resolveAt: state.meta.elapsed + terms.duration,
    operatorId: terms.operatorId,
    operatorTitle: terms.operatorTitle,
    compatibilityLabel: terms.compatibilityLabel,
    warDuration: terms.warDuration,
    intensityPulse: terms.intensityPulse,
    loyaltyPulse: terms.loyaltyPulse,
    intensityCost: terms.intensityCost,
    escrowCost: terms.cost,
  };
  operations.active.unshift(operation);
  operations.active = operations.active.slice(0, DYNASTY_OPERATION_ACTIVE_LIMIT);
  pushMessage(
    state,
    `${getFactionDisplayName(state, factionId)} sends a holy war declaration toward ${getFactionDisplayName(state, targetFactionId)}.`,
    factionId === "player" ? "warn" : targetFactionId === "player" ? "warn" : "info",
  );
  return { ok: true, operation };
}

export function getDivineRightDeclarationTerms(state, factionId) {
  const faction = getFaction(state, factionId);
  const faithState = ensureFaithCovenantTestCompletionFromLegacyState(state, factionId);
  if (!faction?.dynasty || faction.kind !== "kingdom") {
    return { available: false, reason: "Only a ruling dynasty may declare Divine Right." };
  }
  const profile = getDivineRightDeclarationProfile(state, factionId);
  if (!profile.selectedFaithId) {
    return { available: false, reason: "A committed covenant is required before Divine Right can be declared." };
  }
  if (!faithState?.covenantTestPassed) {
    const activeCovenantTest = getActiveCovenantTest(faction);
    return {
      available: false,
      reason: activeCovenantTest
        ? `Covenant Test remains active: ${activeCovenantTest.mandateLabel}.`
        : "The Covenant Test must be completed before Divine Right can be declared.",
    };
  }
  if (getActiveDivineRightDeclaration(faction, state.meta.elapsed)) {
    return { available: false, reason: "A Divine Right declaration is already in progress." };
  }
  if (profile.cooldownRemaining > 0) {
    return { available: false, reason: `The court is still recovering from a failed declaration (${Math.ceil(profile.cooldownRemaining)}s).` };
  }
  if (!profile.stageReady) {
    return { available: false, reason: "Final Convergence is required before the declaration window can open." };
  }
  if (!profile.intensityReady) {
    return { available: false, reason: `Faith intensity must be Apex (${DIVINE_RIGHT_INTENSITY_THRESHOLD}+) with Level 5 covenant conviction.` };
  }
  if (!profile.activeApexStructureReady) {
    return { available: false, reason: "A living Apex covenant structure is required to support the declaration." };
  }
  if (!profile.recognitionReady) {
    return {
      available: false,
      reason: `Global recognition share must reach ${profile.requiredSharePct}% (${profile.recognitionSharePct}% currently).`,
    };
  }
  return {
    available: true,
    duration: DIVINE_RIGHT_DECLARATION_DURATION_SECONDS,
    recognitionShare: profile.recognitionShare,
    recognitionSharePct: profile.recognitionSharePct,
    requiredShare: profile.requiredShare,
    requiredSharePct: profile.requiredSharePct,
    structureName: profile.activeApexStructureName,
    faithName: profile.selectedFaithName,
  };
}

export function performCovenantTestAction(state, factionId) {
  const faction = getFaction(state, factionId);
  const event = getActiveCovenantTest(faction);
  if (!faction?.dynasty || !event) {
    return { ok: false, reason: "No active Covenant Test currently demands action." };
  }
  const evaluation = evaluateCovenantTestProgress(state, factionId, event);
  if (!evaluation.actionAvailable) {
    return { ok: false, reason: evaluation.shortfalls?.[0] ?? "The court has not met the rite conditions yet." };
  }

  if (event.mandateId === "blood_dominion_light_ceremony") {
    applyCost(faction.resources, COVENANT_TEST_BLOOD_DOMINION_LIGHT_RITE_COST);
    event.ritualCompletedAt = state.meta.elapsed;
    event.ritualCompletedAtInWorldDays = state.dualClock?.inWorldDays ?? 0;
    recordConvictionEvent(state, factionId, "oathkeeping", 2, "Conducted the Shared Covenant Ceremony");
  } else if (event.mandateId === "blood_dominion_dark_binding") {
    applyCost(faction.resources, COVENANT_TEST_BLOOD_DOMINION_DARK_BINDING_COST);
    faction.population.total = Math.max(0, (faction.population.total ?? 0) - COVENANT_TEST_BLOOD_DOMINION_DARK_POPULATION_COST);
    adjustLegitimacy(faction, -COVENANT_TEST_BLOOD_DOMINION_DARK_LEGITIMACY_COST);
    event.bindingCompletedAt = state.meta.elapsed;
    event.bindingCompletedAtInWorldDays = state.dualClock?.inWorldDays ?? 0;
    recordConvictionEvent(state, factionId, "ruthlessness", 3, "Paid the Binding of Cost");
  } else {
    return { ok: false, reason: "This Covenant Test currently resolves through state changes, not a direct rite action." };
  }

  const postAction = evaluateCovenantTestProgress(state, factionId, event);
  if (postAction.completed) {
    const resolved = completeCovenantTest(state, factionId, event, "the required covenant rite was performed");
    return { ok: true, resolved };
  }
  return { ok: true, event: serializeCovenantTestEvent(state, factionId, event) };
}

function failDivineRightDeclaration(state, faction, declaration, reason) {
  if (!faction?.faith || !declaration) {
    return;
  }
  faction.faith.divineRightDeclaration = null;
  faction.faith.divineRightCooldownUntil = state.meta.elapsed + DIVINE_RIGHT_RELAUNCH_COOLDOWN_SECONDS;
  faction.faith.lastDivineRightOutcome = {
    id: declaration.id,
    outcome: "failed",
    reason,
    faithId: declaration.faithId,
    faithName: declaration.faithName,
    failedAt: state.meta.elapsed,
  };
  faction.faith.intensity = Math.max(0, (faction.faith.intensity ?? 0) - DIVINE_RIGHT_FAILURE_INTENSITY_LOSS);
  syncFaithIntensityState(faction);
  adjustLegitimacy(faction, -DIVINE_RIGHT_FAILURE_LEGITIMACY_LOSS);
  pushMessage(
    state,
    `${getFactionDisplayName(state, faction.id)} loses its Divine Right claim: ${reason}.`,
    faction.id === "player" ? "warn" : "good",
  );
}

function completeDivineRightDeclaration(state, faction, declaration) {
  if (!faction?.faith || !declaration || state.meta.status !== "playing") {
    return;
  }
  faction.faith.divineRightDeclaration = {
    ...declaration,
    completedAt: state.meta.elapsed,
  };
  faction.faith.lastDivineRightOutcome = {
    id: declaration.id,
    outcome: "completed",
    faithId: declaration.faithId,
    faithName: declaration.faithName,
    completedAt: state.meta.elapsed,
  };
  adjustLegitimacy(faction, 12);
  declareInWorldTime(
    state,
    240,
    `Divine Right recognized: ${getFactionDisplayName(state, faction.id)} under ${declaration.faithName ?? declaration.faithId}`,
  );
  state.meta.status = faction.id === "player" ? "won" : "lost";
  state.meta.winnerId = faction.id;
  state.meta.victoryType = "divine_right";
  state.meta.victoryReason = `${getFactionDisplayName(state, faction.id)} completed the Divine Right spread window under ${declaration.faithName ?? declaration.faithId}.`;
  pushMessage(
    state,
    `${getFactionDisplayName(state, faction.id)} completes the Divine Right spread window under ${declaration.faithName ?? declaration.faithId}. Sovereignty is recognized.`,
    faction.id === "player" ? "good" : "warn",
  );
}

function tickFaithDivineRightDeclarations(state) {
  Object.values(state.factions).forEach((faction) => {
    const declaration = getFaithDivineRightDeclarationState(faction);
    if (!declaration || declaration.failedAt || declaration.completedAt) {
      return;
    }
    const profile = getDivineRightDeclarationProfile(state, faction.id);
    declaration.recognitionShare = profile.recognitionShare;
    declaration.recognitionSharePct = profile.recognitionSharePct;
    declaration.requiredShare = profile.requiredShare;
    declaration.requiredSharePct = profile.requiredSharePct;
    declaration.remainingSeconds = Math.max(0, (declaration.resolveAt ?? 0) - state.meta.elapsed);
    declaration.structureName = profile.activeApexStructureName;
    declaration.structureId = profile.activeApexStructureId;

    if (!profile.selectedFaithId) {
      failDivineRightDeclaration(state, faction, declaration, "the covenant bond was broken");
      return;
    }
    if (!profile.activeApexStructureReady) {
      failDivineRightDeclaration(state, faction, declaration, "the apex covenant structure was lost");
      return;
    }
    if (!profile.intensityReady) {
      failDivineRightDeclaration(state, faction, declaration, "faith intensity fell below Apex");
      return;
    }
    if (!profile.recognitionReady) {
      failDivineRightDeclaration(state, faction, declaration, "global recognition share collapsed below threshold");
      return;
    }
    if ((declaration.resolveAt ?? 0) <= state.meta.elapsed) {
      completeDivineRightDeclaration(state, faction, declaration);
    }
  });
}

export function startDivineRightDeclaration(state, factionId) {
  const faction = getFaction(state, factionId);
  const terms = getDivineRightDeclarationTerms(state, factionId);
  if (!terms.available) {
    return { ok: false, reason: terms.reason };
  }
  const declaration = {
    id: createEntityId(state, "divineRight"),
    faithId: faction.faith.selectedFaithId,
    faithName: terms.faithName ?? faction.faith.selectedFaithId,
    doctrinePath: faction.faith.doctrinePath ?? "light",
    declaredAt: state.meta.elapsed,
    resolveAt: state.meta.elapsed + DIVINE_RIGHT_DECLARATION_DURATION_SECONDS,
    recognitionShare: terms.recognitionShare ?? 0,
    recognitionSharePct: terms.recognitionSharePct ?? 0,
    requiredShare: terms.requiredShare ?? DIVINE_RIGHT_GLOBAL_SHARE_THRESHOLD,
    requiredSharePct: terms.requiredSharePct ?? Math.round(DIVINE_RIGHT_GLOBAL_SHARE_THRESHOLD * 1000) / 10,
    structureId: getDivineRightDeclarationProfile(state, factionId).activeApexStructureId,
    structureName: terms.structureName ?? null,
  };
  faction.faith.divineRightDeclaration = declaration;
  faction.faith.lastDivineRightOutcome = null;
  recordConvictionEvent(
    state,
    factionId,
    faction.faith.doctrinePath === "dark" ? "desecration" : "oathkeeping",
    3,
    `Declared Divine Right under ${declaration.faithName}`,
  );
  Object.values(state.factions)
    .filter((candidate) => candidate?.kind === "kingdom" && candidate.id !== factionId)
    .forEach((candidate) => {
      const sameFaith = candidate.faith?.selectedFaithId === declaration.faithId &&
        candidate.faith?.doctrinePath === declaration.doctrinePath;
      if (!sameFaith) {
        ensureMutualHostility(state, factionId, candidate.id);
      }
      if (candidate.ai) {
        candidate.ai.attackTimer = Math.min(candidate.ai.attackTimer ?? 999, DIVINE_RIGHT_ATTACK_TIMER_CAP);
        candidate.ai.territoryTimer = Math.min(candidate.ai.territoryTimer ?? 999, DIVINE_RIGHT_TERRITORY_TIMER_CAP);
        candidate.ai.raidTimer = Math.min(candidate.ai.raidTimer ?? 999, DIVINE_RIGHT_RAID_TIMER_CAP);
        candidate.ai.missionaryTimer = Math.min(candidate.ai.missionaryTimer ?? 999, DIVINE_RIGHT_MISSIONARY_TIMER_CAP);
        candidate.ai.holyWarTimer = Math.min(candidate.ai.holyWarTimer ?? 999, DIVINE_RIGHT_HOLY_WAR_TIMER_CAP);
      }
    });
  pushMessage(
    state,
    `${getFactionDisplayName(state, factionId)} declares Divine Right under ${declaration.faithName}. The spread window opens for ${DIVINE_RIGHT_DECLARATION_DURATION_SECONDS}s at ${terms.recognitionSharePct}% recognition.`,
    factionId === "player" ? "warn" : "warn",
  );
  return { ok: true, declaration };
}

export function startCounterIntelligenceOperation(state, factionId) {
  const faction = getFaction(state, factionId);
  const terms = getCounterIntelligenceTerms(state, factionId);
  if (!terms.available) {
    return { ok: false, reason: terms.reason };
  }
  const operations = getDynastyOperationsState(faction);
  if ((operations.active ?? []).length >= DYNASTY_OPERATION_ACTIVE_LIMIT) {
    return { ok: false, reason: "Too many dynasty operations are already active." };
  }

  applyCost(faction.resources, terms.cost);
  const operation = {
    id: createEntityId(state, "dynastyOperation"),
    type: "counter_intelligence",
    sourceFactionId: factionId,
    startedAt: state.meta.elapsed,
    resolveAt: state.meta.elapsed + terms.duration,
    operatorId: terms.operatorId,
    operatorTitle: terms.operatorTitle,
    watchDuration: terms.watchDuration,
    watchStrength: terms.watchStrength,
    wardLabel: terms.wardLabel,
    guardedRoles: terms.guardedRoles,
    averageLoyalty: terms.averageLoyalty,
    weakestLoyalty: terms.weakestLoyalty,
    escrowCost: terms.cost,
  };
  operations.active.unshift(operation);
  operations.active = operations.active.slice(0, DYNASTY_OPERATION_ACTIVE_LIMIT);

  pushMessage(
    state,
    `${getFactionDisplayName(state, factionId)} mobilizes counter-intelligence around the bloodline court.`,
    factionId === "player" ? "info" : "warn",
  );
  return { ok: true, operation };
}

export function startEspionageOperation(state, factionId, targetFactionId) {
  const faction = getFaction(state, factionId);
  const terms = getEspionageTerms(state, factionId, targetFactionId);
  if (!terms.available) {
    return { ok: false, reason: terms.reason };
  }
  const operations = getDynastyOperationsState(faction);
  if ((operations.active ?? []).length >= DYNASTY_OPERATION_ACTIVE_LIMIT) {
    return { ok: false, reason: "Too many dynasty operations are already active." };
  }

  applyCost(faction.resources, terms.cost);
  const operation = {
    id: createEntityId(state, "dynastyOperation"),
    type: "espionage",
    sourceFactionId: factionId,
    targetFactionId,
    startedAt: state.meta.elapsed,
    resolveAt: state.meta.elapsed + terms.duration,
    spymasterId: terms.spymasterId,
    reportDuration: terms.reportDuration,
    successScore: terms.successScore,
    projectedChance: terms.projectedChance,
    escrowCost: terms.cost,
  };
  operations.active.unshift(operation);
  operations.active = operations.active.slice(0, DYNASTY_OPERATION_ACTIVE_LIMIT);

  pushMessage(
    state,
    `${getFactionDisplayName(state, factionId)} dispatches an espionage web toward ${getFactionDisplayName(state, targetFactionId)}.`,
    factionId === "player" ? "info" : targetFactionId === "player" ? "warn" : "info",
  );
  return { ok: true, operation };
}

export function startAssassinationOperation(state, factionId, targetFactionId, targetMemberId) {
  const faction = getFaction(state, factionId);
  const terms = getAssassinationTerms(state, factionId, targetFactionId, targetMemberId);
  if (!terms.available) {
    return { ok: false, reason: terms.reason };
  }
  const operations = getDynastyOperationsState(faction);
  if ((operations.active ?? []).length >= DYNASTY_OPERATION_ACTIVE_LIMIT) {
    return { ok: false, reason: "Too many dynasty operations are already active." };
  }

  applyCost(faction.resources, terms.cost);
  const operation = {
    id: createEntityId(state, "dynastyOperation"),
    type: "assassination",
    sourceFactionId: factionId,
    targetFactionId,
    targetMemberId,
    memberId: targetMemberId,
    memberTitle: terms.targetMemberTitle,
    startedAt: state.meta.elapsed,
    resolveAt: state.meta.elapsed + terms.duration,
    spymasterId: terms.spymasterId,
    locationLabel: terms.locationLabel,
    intelSupport: terms.intelSupport,
    successScore: terms.successScore,
    projectedChance: terms.projectedChance,
    escrowCost: terms.cost,
  };
  operations.active.unshift(operation);
  operations.active = operations.active.slice(0, DYNASTY_OPERATION_ACTIVE_LIMIT);

  pushMessage(
    state,
    `${getFactionDisplayName(state, factionId)} sets an assassination cell on ${terms.targetMemberTitle} inside ${getFactionDisplayName(state, targetFactionId)}.`,
    factionId === "player" ? "info" : targetFactionId === "player" ? "warn" : "info",
  );
  return { ok: true, operation };
}

export function startSabotageOperation(state, factionId, subtype, targetFactionId, targetBuildingId, options = {}) {
  const faction = getFaction(state, factionId);
  const terms = getSabotageTerms(state, factionId, subtype, targetFactionId, targetBuildingId, options);
  if (!terms.available) {
    return { ok: false, reason: terms.reason };
  }
  const operations = getDynastyOperationsState(faction);
  if ((operations.active ?? []).length >= DYNASTY_OPERATION_ACTIVE_LIMIT) {
    return { ok: false, reason: "Too many dynasty operations are already active." };
  }
  applyCost(faction.resources, terms.cost);
  const operation = {
    id: createEntityId(state, "dynastyOperation"),
    type: "sabotage",
    subtype,
    sourceFactionId: factionId,
    targetFactionId,
    targetBuildingId,
    targetBuildingName: terms.targetBuildingName,
    startedAt: state.meta.elapsed,
    resolveAt: state.meta.elapsed + terms.duration,
    spymasterId: terms.spymasterId,
    successScore: terms.successScore,
    projectedChance: terms.projectedChance,
    escrowCost: terms.cost,
    intelligenceReportId: terms.intelligenceReportId ?? null,
    intelligenceSupportBonus: terms.intelligenceSupportBonus ?? 0,
    retaliationReason: terms.retaliationReason ?? null,
    retaliationInterceptType: terms.retaliationInterceptType ?? null,
  };
  operations.active.unshift(operation);
  operations.active = operations.active.slice(0, DYNASTY_OPERATION_ACTIVE_LIMIT);
  pushMessage(
    state,
    `${getFactionDisplayName(state, factionId)} dispatches saboteurs to ${getFactionDisplayName(state, targetFactionId)} for ${subtype.replace(/_/g, " ")}${terms.intelligenceReportId ? ", guided by a counter-intelligence dossier" : ""}.`,
    factionId === "player" ? "info" : "info",
  );
  return { ok: true, operation };
}

function applySabotageEffect(state, operation) {
  const building = state.buildings.find((b) => b.id === operation.targetBuildingId);
  const targetFaction = getFaction(state, operation.targetFactionId);
  if (!targetFaction) return;

  switch (operation.subtype) {
    case "gate_opening": {
      if (!building || building.health <= 0) return;
      const def = getBuildingDef(state.content, building.typeId);
      const floor = (def?.health ?? building.health) * 0.2;
      building.health = Math.min(building.health, floor);
      // Canonical window: gate stays exposed; defenders must rebuild.
      building.sabotageGateExposedUntil = state.meta.elapsed + 15;
      return;
    }
    case "fire_raising": {
      if (!building || building.health <= 0) return;
      building.burnExpiresAt = state.meta.elapsed + 10;
      building.burnDamagePerSecond = 8;
      return;
    }
    case "supply_poisoning": {
      if (!building) return;
      building.poisonedUntil = state.meta.elapsed + 20;
      return;
    }
    case "well_poisoning": {
      // Add two water-strain cycles to target faction (canonical water-crisis bias).
      targetFaction.waterStrainStreak = (targetFaction.waterStrainStreak ?? 0) + 2;
      return;
    }
  }
}

export function startRansomNegotiation(state, factionId, memberId) {
  const faction = getFaction(state, factionId);
  if (!faction?.dynasty) {
    return { ok: false, reason: "Faction cannot run dynasty operations." };
  }
  const member = getDynastyMember(faction, memberId);
  const terms = getCapturedMemberRansomTerms(state, factionId, memberId);
  if (!member || !terms.available) {
    return { ok: false, reason: terms.reason ?? "Ransom negotiation is unavailable." };
  }

  const operations = getDynastyOperationsState(faction);
  if ((operations.active ?? []).length >= DYNASTY_OPERATION_ACTIVE_LIMIT) {
    return { ok: false, reason: "Too many dynasty operations are already active." };
  }

  applyCost(faction.resources, terms.cost);
  const operation = {
    id: createEntityId(state, "dynastyOperation"),
    type: "ransom",
    memberId,
    memberTitle: member.title,
    sourceFactionId: factionId,
    targetFactionId: terms.captorFactionId,
    startedAt: state.meta.elapsed,
    resolveAt: state.meta.elapsed + terms.duration,
    diplomatId: terms.diplomatId,
    merchantId: terms.merchantId,
    escrowCost: terms.cost,
  };
  operations.active.unshift(operation);
  operations.active = operations.active.slice(0, DYNASTY_OPERATION_ACTIVE_LIMIT);

  pushMessage(
    state,
    `${getFactionDisplayName(state, factionId)} opens ransom terms for ${member.title} with ${getFactionDisplayName(state, terms.captorFactionId)}.`,
    factionId === "player" ? "info" : "good",
  );
  return { ok: true, operation };
}

export function startRescueOperation(state, factionId, memberId) {
  const faction = getFaction(state, factionId);
  if (!faction?.dynasty) {
    return { ok: false, reason: "Faction cannot run dynasty operations." };
  }
  const member = getDynastyMember(faction, memberId);
  const terms = getCapturedMemberRescueTerms(state, factionId, memberId);
  if (!member || !terms.available) {
    return { ok: false, reason: terms.reason ?? "Rescue operation is unavailable." };
  }

  const operations = getDynastyOperationsState(faction);
  if ((operations.active ?? []).length >= DYNASTY_OPERATION_ACTIVE_LIMIT) {
    return { ok: false, reason: "Too many dynasty operations are already active." };
  }

  applyCost(faction.resources, terms.cost);
  const operation = {
    id: createEntityId(state, "dynastyOperation"),
    type: "rescue",
    memberId,
    memberTitle: member.title,
    sourceFactionId: factionId,
    targetFactionId: terms.captorFactionId,
    startedAt: state.meta.elapsed,
    resolveAt: state.meta.elapsed + terms.duration,
    spymasterId: terms.spymasterId,
    diplomatId: terms.diplomatId,
    holdingSettlementId: terms.holdingSettlementId,
    keepTier: terms.keepTier,
    wardId: terms.wardId,
    successScore: terms.successScore,
    projectedChance: terms.projectedChance,
    escrowCost: terms.cost,
  };
  operations.active.unshift(operation);
  operations.active = operations.active.slice(0, DYNASTY_OPERATION_ACTIVE_LIMIT);

  pushMessage(
    state,
    `${getFactionDisplayName(state, factionId)} dispatches covert agents to recover ${member.title} from ${getFactionDisplayName(state, terms.captorFactionId)}.`,
    factionId === "player" ? "info" : "good",
  );
  return { ok: true, operation };
}

export function demandCaptiveRansom(state, factionId, captiveId) {
  const faction = getFaction(state, factionId);
  if (!faction?.dynasty) {
    return { ok: false, reason: "Faction cannot compel ransom." };
  }

  const captive = findCaptiveRecordById(faction, captiveId);
  const terms = getHeldCaptiveRansomTerms(state, factionId, captiveId);
  if (!captive || !terms.available) {
    return { ok: false, reason: terms.reason ?? "Ransom demand cannot be executed." };
  }

  const sourceFaction = getFaction(state, captive.sourceFactionId);
  applyCost(sourceFaction.resources, terms.cost);
  grantResources(faction.resources, terms.cost);
  const release = releaseCapturedMember(state, captive.sourceFactionId, captive.memberId, {
    captorFactionId: factionId,
    captiveId,
  });
  if (!release.ok) {
    return { ok: false, reason: release.reason ?? "Release failed." };
  }

  adjustLegitimacy(sourceFaction, LEGITIMACY_RECOVERY_ON_RANSOM);
  recordConvictionEvent(state, captive.sourceFactionId, "oathkeeping", 1, `Paid ransom to recover ${captive.title}`);
  recordConvictionEvent(state, factionId, "stewardship", 1, `Compelled ransom for ${captive.title}`);
  pushDynastyOperationHistory(sourceFaction, {
    id: `history-${captiveId}-${Math.floor(state.meta.elapsed * 10)}`,
    type: "ransom",
    memberId: captive.memberId,
    memberTitle: captive.title,
    sourceFactionId: captive.sourceFactionId,
    targetFactionId: factionId,
    status: "completed",
    completedAt: state.meta.elapsed,
    outcomeText: `${captive.title} returned under compelled ransom terms.`,
    perspective: "source",
  });
  pushDynastyOperationHistory(faction, {
    id: `history-${captiveId}-${Math.floor(state.meta.elapsed * 10)}`,
    type: "ransom",
    memberId: captive.memberId,
    memberTitle: captive.title,
    sourceFactionId: captive.sourceFactionId,
    targetFactionId: factionId,
    status: "completed",
    completedAt: state.meta.elapsed,
    outcomeText: `${captive.title} released for immediate ransom.`,
    perspective: "target",
  });
  pushMessage(
    state,
    `${getFactionDisplayName(state, factionId)} extracts ransom and returns ${captive.title} to ${terms.sourceFactionName}.`,
    factionId === "player" ? "good" : captive.sourceFactionId === "player" ? "warn" : "info",
  );
  return { ok: true };
}

// --------------------------------------------------------------------------
// 2026-04-14 Fortification / Siege / Realm Cycle extensions
// These functions implement the canonical lane declared in
// 01_CANON/DEFENSIVE_FORTIFICATION_DOCTRINE.md and the Master Doctrine
// 2026-04-14, specifically:
//   - settlement class + fortification tier tracking
//   - assault-failure cohesion strain (wave-spam denial)
//   - canonical 90-second realm cycle with famine + water crisis
// Nothing above these helpers was removed; this block is strictly additive.
// --------------------------------------------------------------------------

function nearestSettlementForBuilding(state, building) {
  const def = getBuildingDef(state.content, building.typeId);
  const center = getBuildingCenter(state, building, def);
  const tileSize = state.world.tileSize;
  const radius = FORTIFICATION_ECOSYSTEM_RADIUS_TILES * tileSize;
  let best = null;
  // Prefer same-faction settlement seats first.
  for (const settlement of state.world.settlements ?? []) {
    if (settlement.factionId && settlement.factionId !== building.factionId) {
      continue;
    }
    const sx = settlement.x * tileSize;
    const sy = settlement.y * tileSize;
    const d = distance(center.x, center.y, sx, sy);
    if (d <= radius && (!best || d < best.distance)) {
      best = { kind: "settlement", target: settlement, distance: d };
    }
  }
  if (best) return best;
  // Fall back to owned control points within the ecosystem radius.
  for (const cp of state.world.controlPoints) {
    if (cp.ownerFactionId && cp.ownerFactionId !== building.factionId) {
      continue;
    }
    const cx = cp.x * tileSize;
    const cy = cp.y * tileSize;
    const d = distance(center.x, center.y, cx, cy);
    if (d <= radius && (!best || d < best.distance)) {
      best = { kind: "controlPoint", target: cp, distance: d };
    }
  }
  return best;
}

function settlementClassOf(state, anchor) {
  if (!anchor) return null;
  if (anchor.kind === "settlement") {
    return state.content.byId.settlementClasses?.[anchor.target.settlementClass] ?? null;
  }
  if (anchor.kind === "controlPoint") {
    return state.content.byId.settlementClasses?.[anchor.target.settlementClass] ?? null;
  }
  return null;
}

function advanceFortificationTier(state, building, buildingDef) {
  const anchor = nearestSettlementForBuilding(state, building);
  if (!anchor) return;
  const klass = settlementClassOf(state, anchor);
  if (!klass) return;
  const ceiling = klass.defensiveCeiling ?? 1;
  const contribution = buildingDef.fortificationTierContribution ?? 1;
  const prior = anchor.target.fortificationTier ?? 0;
  const next = Math.min(ceiling, prior + contribution);
  anchor.target.fortificationTier = next;
  if (next > prior && building.factionId === "player") {
    const name = anchor.target.name ?? anchor.target.id;
    pushMessage(
      state,
      `${name} advances to fortification tier ${next} of ${ceiling}.`,
      "good",
    );
  }
}

function getPrimaryKeepSeat(state, factionId) {
  return (state.world.settlements ?? []).find(
    (settlement) => settlement.factionId === factionId && settlement.settlementClass === "primary_dynastic_keep",
  ) ?? null;
}

function getFortificationFaithWardProfile(state, settlement) {
  if (!settlement?.factionId || (settlement.fortificationTier ?? 0) <= 0) {
    return DEFAULT_FORTIFICATION_WARD;
  }

  const faction = getFaction(state, settlement.factionId);
  if (!faction?.faith?.selectedFaithId) {
    return DEFAULT_FORTIFICATION_WARD;
  }

  const doctrinePath = faction.faith.doctrinePath ?? "light";
  const intensityScale = Math.min(1.22, 1 + (faction.faith.intensity ?? 0) / 200);
  const surgeActive = (faction.bloodAltarSurgeUntil ?? 0) > state.meta.elapsed;

  if (faction.faith.selectedFaithId === "old_light") {
    return {
      id: "old_light",
      label: doctrinePath === "light" ? "Pyre Ward" : "Judgment Pyre",
      sightBonus: Math.round((doctrinePath === "light" ? 22 : 16) * intensityScale),
      defenderAttackMultiplier: doctrinePath === "light" ? 1.03 : 1.08,
      reserveHealMultiplier: doctrinePath === "light" ? 1.15 : 1.08,
      reserveMusterMultiplier: doctrinePath === "light" ? 1.08 : 1.1,
      loyaltyProtectionMultiplier: doctrinePath === "light" ? 1.18 : 1.08,
      enemySpeedMultiplier: 1,
      surgeActive: false,
    };
  }

  if (faction.faith.selectedFaithId === "blood_dominion") {
    return {
      id: "blood_dominion",
      label: surgeActive ? "Blood-Altar Surge" : "Blood-Altar Reserve",
      sightBonus: 0,
      defenderAttackMultiplier: surgeActive
        ? (doctrinePath === "light" ? 1.12 : 1.2)
        : (doctrinePath === "light" ? 1.04 : 1.09),
      reserveHealMultiplier: surgeActive ? 1.16 : 1.05,
      reserveMusterMultiplier: surgeActive ? 1.22 : 1.08,
      loyaltyProtectionMultiplier: doctrinePath === "light" ? 1.06 : 1,
      enemySpeedMultiplier: 1,
      surgeActive,
    };
  }

  if (faction.faith.selectedFaithId === "the_order") {
    return {
      id: "the_order",
      label: doctrinePath === "light" ? "Edict Ward" : "Submission Edict",
      sightBonus: Math.round((doctrinePath === "light" ? 12 : 8) * intensityScale),
      defenderAttackMultiplier: doctrinePath === "light" ? 1.04 : 1.08,
      reserveHealMultiplier: doctrinePath === "light" ? 1.1 : 1.06,
      reserveMusterMultiplier: doctrinePath === "light" ? 1.18 : 1.14,
      loyaltyProtectionMultiplier: doctrinePath === "light" ? 1.26 : 1.18,
      enemySpeedMultiplier: 1,
      surgeActive: false,
    };
  }

  if (faction.faith.selectedFaithId === "the_wild") {
    return {
      id: "the_wild",
      label: doctrinePath === "light" ? "Root Ward" : "Predator Root Ward",
      sightBonus: Math.round((doctrinePath === "light" ? 10 : 6) * intensityScale),
      defenderAttackMultiplier: doctrinePath === "light" ? 1.04 : 1.1,
      reserveHealMultiplier: doctrinePath === "light" ? 1.06 : 1.02,
      reserveMusterMultiplier: doctrinePath === "light" ? 1.08 : 1.12,
      loyaltyProtectionMultiplier: doctrinePath === "light" ? 1.1 : 1.02,
      enemySpeedMultiplier: doctrinePath === "light" ? 0.88 : 0.82,
      surgeActive: false,
    };
  }

  return DEFAULT_FORTIFICATION_WARD;
}

function maybeTriggerBloodAltarSurge(state, settlement, defenseState) {
  if (!settlement?.factionId || defenseState.enemyCombatants.length === 0) {
    return;
  }

  const faction = getFaction(state, settlement.factionId);
  if (faction?.faith?.selectedFaithId !== "blood_dominion") {
    return;
  }
  if ((faction.bloodAltarSurgeUntil ?? 0) > state.meta.elapsed || (faction.bloodAltarSurgeCooldownUntil ?? 0) > state.meta.elapsed) {
    return;
  }

  const survivingPopulationFloor = getUsedPopulation(state, faction.id) + faction.population.reserved + 1;
  if ((faction.population.total ?? 0) <= survivingPopulationFloor) {
    return;
  }

  faction.population.total = Math.max(0, faction.population.total - 1);
  faction.bloodAltarSurgeUntil = state.meta.elapsed + BLOOD_ALTAR_SURGE_DURATION_SECONDS;
  faction.bloodAltarSurgeCooldownUntil = state.meta.elapsed + BLOOD_ALTAR_SURGE_COOLDOWN_SECONDS;
  recordConvictionEvent(
    state,
    faction.id,
    faction.faith.doctrinePath === "dark" ? "ruthlessness" : "oathkeeping",
    1,
    `${settlement.name} enacted a blood-altar reserve rite`,
  );
  pushMessage(
    state,
    `${settlement.name} burns blood in reserve rites, surging the defense.`,
    faction.id === "player" ? "warn" : "info",
  );
}

function getKeepPresenceProfile(state, settlement) {
  const faction = getFaction(state, settlement.factionId);
  if (!faction?.dynasty) {
    return { commanderPresent: false, bloodlinePresent: false, governorPresent: false };
  }

  const position = getSettlementPosition(state, settlement);
  const radius = FORTIFICATION_KEEP_PRESENCE_RADIUS_TILES * state.world.tileSize;
  const commanderUnit = faction.dynasty.attachments.commanderUnitId
    ? getEntity(state, "unit", faction.dynasty.attachments.commanderUnitId)
    : null;
  const commanderMember = getDynastyMember(faction, faction.dynasty.attachments.commanderMemberId);
  const commanderPresent = Boolean(
    commanderUnit &&
    commanderUnit.health > 0 &&
    distance(commanderUnit.x, commanderUnit.y, position.x, position.y) <= radius,
  );
  const rulingHead = (faction.dynasty.members ?? []).find((member) =>
    member.roleId === "head_of_bloodline" && member.status === "ruling",
  ) ?? null;
  const dynasticSeatPresence = Boolean(
    settlement.settlementClass === "primary_dynastic_keep" &&
    rulingHead &&
    rulingHead.status === "ruling",
  );
  const commandingBloodlinePresence = Boolean(
    commanderPresent &&
    commanderMember &&
    ["head_of_bloodline", "heir_designate", "commander"].includes(commanderMember.roleId),
  );
  const governorPresent = Boolean(settlement.governorMemberId && getDynastyMember(faction, settlement.governorMemberId));

  return {
    commanderPresent,
    governorPresent,
    bloodlinePresent: dynasticSeatPresence || commandingBloodlinePresence,
  };
}

function getSettlementLocalLoyaltyProfile(state, settlement) {
  if (!settlement?.factionId) {
    return { average: 50, min: 50, territoryCount: 0 };
  }
  const position = getSettlementPosition(state, settlement);
  const radius = FORTIFICATION_ECOSYSTEM_RADIUS_TILES * state.world.tileSize * 1.35;
  const nearbyTerritories = state.world.controlPoints.filter((controlPoint) =>
    controlPoint.ownerFactionId === settlement.factionId &&
    distance(position.x, position.y, controlPoint.x * state.world.tileSize, controlPoint.y * state.world.tileSize) <= radius);
  const territories = nearbyTerritories.length > 0
    ? nearbyTerritories
    : state.world.controlPoints.filter((controlPoint) => controlPoint.ownerFactionId === settlement.factionId);
  if (territories.length === 0) {
    return { average: 50, min: 50, territoryCount: 0 };
  }
  const average = territories.reduce((sum, controlPoint) => sum + (controlPoint.loyalty ?? 50), 0) / territories.length;
  const min = territories.reduce((lowest, controlPoint) => Math.min(lowest, controlPoint.loyalty ?? 50), 100);
  return {
    average,
    min,
    territoryCount: territories.length,
  };
}

function getSettlementWatchtowerCount(state, settlement, radius) {
  if (!settlement?.factionId) {
    return 0;
  }
  const position = getSettlementPosition(state, settlement);
  return state.buildings.reduce((count, building) => {
    if (
      building.factionId !== settlement.factionId ||
      building.typeId !== "watch_tower" ||
      !building.completed ||
      building.health <= 0
    ) {
      return count;
    }
    const buildingCenter = getBuildingCenter(state, building, getBuildingDef(state.content, building.typeId));
    if (distance(position.x, position.y, buildingCenter.x, buildingCenter.y) > radius) {
      return count;
    }
    return count + 1;
  }, 0);
}

function createDefaultImminentEngagementState(settlementId = null, settlementName = null) {
  return {
    active: false,
    windowConsumed: false,
    settlementId,
    settlementName,
    battleTypeId: null,
    battleTypeLabel: null,
    hostileFactionId: null,
    hostileFactionName: null,
    hostileCount: 0,
    hostileSiegeCount: 0,
    hostileScoutCount: 0,
    watchtowerCount: 0,
    warningRadius: 0,
    totalSeconds: 0,
    expiresAt: 0,
    remainingSeconds: 0,
    localLoyalty: 50,
    localLoyaltyMin: 50,
    commanderPresent: false,
    commanderRecallAvailable: false,
    commanderRecallIssuedAt: -999,
    governorPresent: false,
    bloodlineAtRisk: false,
    bloodlineProtectionUntil: 0,
    selectedResponseId: "steady",
    selectedResponseLabel: IMMINENT_ENGAGEMENT_POSTURES.steady.label,
    reinforcementsCommittedUntil: 0,
    startedAt: 0,
    engagedAt: 0,
    lastActivationAt: -999,
  };
}

function ensureImminentEngagementState(settlement) {
  settlement.imminentEngagement = {
    ...createDefaultImminentEngagementState(settlement?.id ?? null, settlement?.name ?? null),
    ...(settlement.imminentEngagement ?? {}),
  };
  return settlement.imminentEngagement;
}

function getImminentEngagementPosture(postureId) {
  return IMMINENT_ENGAGEMENT_POSTURES[postureId] ?? IMMINENT_ENGAGEMENT_POSTURES.steady;
}

function getSettlementImminentEngagementDirective(settlement, elapsed) {
  const engagement = ensureImminentEngagementState(settlement);
  const posture = getImminentEngagementPosture(engagement.selectedResponseId);
  return {
    engagement,
    posture,
    active: Boolean(engagement.active || engagement.windowConsumed),
    reinforcementSurgeActive: (engagement.reinforcementsCommittedUntil ?? 0) > elapsed,
  };
}

function getSettlementImminentEngagementProfile(state, settlement, defenseState = null) {
  if (!settlement?.factionId || (settlement.fortificationTier ?? 0) <= 0) {
    return {
      active: false,
      battleTypeId: null,
      battleTypeLabel: null,
      hostileCount: 0,
      hostileSiegeCount: 0,
      hostileScoutCount: 0,
      warningRadius: 0,
      warningSeconds: IMMINENT_ENGAGEMENT_MIN_SECONDS,
      watchtowerCount: 0,
      localLoyalty: 50,
      localLoyaltyMin: 50,
      commanderPresent: false,
      governorPresent: false,
      bloodlineAtRisk: false,
      hostileFactionId: null,
      hostileFactionName: null,
      readyReserves: 0,
      recoveringReserves: 0,
    };
  }

  const resolvedDefenseState = defenseState ?? getSettlementDefenseState(state, settlement);
  const loyalty = getSettlementLocalLoyaltyProfile(state, settlement);
  const watchtowerRadius = Math.max(
    resolvedDefenseState.reserveRadius,
    IMMINENT_ENGAGEMENT_WATCHTOWER_RADIUS_TILES * state.world.tileSize,
  );
  const watchtowerCount = getSettlementWatchtowerCount(state, settlement, watchtowerRadius);
  const warningRadius = resolvedDefenseState.threatRadius + (
    IMMINENT_ENGAGEMENT_WARNING_BUFFER_TILES +
    watchtowerCount * 1.25 +
    Math.max(0, (settlement.fortificationTier ?? 0) - 1) * 0.6
  ) * state.world.tileSize;
  const approachingEnemyCombatants = state.units.filter((unit) => {
    if (
      unit.health <= 0 ||
      !areHostile(state, settlement.factionId, unit.factionId)
    ) {
      return false;
    }
    const unitDef = getUnitDef(state.content, unit.typeId);
    if (unitDef.role === "worker") {
      return false;
    }
    return distance(unit.x, unit.y, resolvedDefenseState.position.x, resolvedDefenseState.position.y) <= warningRadius;
  });
  const hostileCounts = approachingEnemyCombatants.reduce((map, unit) => {
    map.set(unit.factionId, (map.get(unit.factionId) ?? 0) + 1);
    return map;
  }, new Map());
  const hostileLeaderEntry = [...hostileCounts.entries()].sort((left, right) => right[1] - left[1])[0] ?? null;
  const hostileFactionId = hostileLeaderEntry?.[0] ?? null;
  const hostileSiegeCount = approachingEnemyCombatants.filter((unit) =>
    Boolean(getUnitDef(state.content, unit.typeId)?.siegeClass)).length;
  const hostileScoutCount = approachingEnemyCombatants.filter((unit) =>
    isScoutRiderUnitDef(getUnitDef(state.content, unit.typeId))).length;
  const nearestApproachDistance = approachingEnemyCombatants.reduce((best, unit) => {
    const d = distance(unit.x, unit.y, resolvedDefenseState.position.x, resolvedDefenseState.position.y);
    return Math.min(best, d);
  }, Number.POSITIVE_INFINITY);
  const commanderMember = settlement.factionId
    ? getDynastyMember(getFaction(state, settlement.factionId), getFaction(state, settlement.factionId)?.dynasty?.attachments?.commanderMemberId)
    : null;
  const loyaltyBonus = loyalty.average >= 72 ? 3 : loyalty.average >= 58 ? 2 : loyalty.average >= 44 ? 1 : 0;
  const commanderBonus = resolvedDefenseState.keepPresence.commanderPresent
    ? 2 + Math.min(2, Math.floor((commanderMember?.renown ?? 0) / 18))
    : 0;
  const baseWarningSeconds = settlement.settlementClass === "primary_dynastic_keep"
    ? IMMINENT_ENGAGEMENT_KEEP_BASE_SECONDS
    : IMMINENT_ENGAGEMENT_SETTLEMENT_BASE_SECONDS;
  const warningSeconds = clamp(
    baseWarningSeconds +
      watchtowerCount * 2 +
      Math.max(0, (settlement.fortificationTier ?? 0) - 1) * 1.5 +
      loyaltyBonus +
      commanderBonus +
      (resolvedDefenseState.keepPresence.governorPresent ? 1 : 0) -
      hostileScoutCount * 1.5 -
      hostileSiegeCount,
    IMMINENT_ENGAGEMENT_MIN_SECONDS,
    IMMINENT_ENGAGEMENT_MAX_SECONDS,
  );
  const threatAlreadyActive = resolvedDefenseState.enemyCombatants.length > 0;
  const approachPressureActive = approachingEnemyCombatants.length > 0 && (
    hostileSiegeCount > 0 ||
    approachingEnemyCombatants.length >= 2 ||
    nearestApproachDistance <= resolvedDefenseState.threatRadius + state.world.tileSize * 1.5
  );

  return {
    active: threatAlreadyActive || approachPressureActive,
    battleTypeId: settlement.settlementClass === "primary_dynastic_keep" ? "dynastic_keep_assault" : "settlement_engagement",
    battleTypeLabel: settlement.settlementClass === "primary_dynastic_keep" ? "Dynastic Keep Assault" : "Settlement Engagement",
    hostileFactionId,
    hostileFactionName: hostileFactionId ? getFactionDisplayName(state, hostileFactionId) : null,
    hostileCount: approachingEnemyCombatants.length,
    hostileSiegeCount,
    hostileScoutCount,
    warningRadius,
    warningSeconds,
    watchtowerCount,
    localLoyalty: loyalty.average,
    localLoyaltyMin: loyalty.min,
    commanderPresent: resolvedDefenseState.keepPresence.commanderPresent,
    commanderRecallAvailable: Boolean(
      getFaction(state, settlement.factionId)?.dynasty?.attachments?.commanderUnitId &&
      !resolvedDefenseState.keepPresence.commanderPresent
    ),
    governorPresent: resolvedDefenseState.keepPresence.governorPresent,
    bloodlineAtRisk: settlement.settlementClass === "primary_dynastic_keep" && resolvedDefenseState.keepPresence.bloodlinePresent,
    readyReserves: resolvedDefenseState.readyReserves,
    recoveringReserves: resolvedDefenseState.recoveringReserves,
  };
}

function getSettlementDefenseState(state, settlement) {
  const position = getSettlementPosition(state, settlement);
  const tileSize = state.world.tileSize;
  const threatRadius = (FORTIFICATION_THREAT_RADIUS_TILES + Math.max(0, (settlement.fortificationTier ?? 0) - 1) * 0.75) * tileSize;
  const reserveRadius = (FORTIFICATION_RESERVE_RADIUS_TILES + (settlement.fortificationTier ?? 0) * 0.5) * tileSize;
  const triageRadius = FORTIFICATION_TRIAGE_RADIUS_TILES * tileSize;
  const governorProfile = getGovernorProfileForSettlement(state, settlement);
  const wardProfile = getFortificationFaithWardProfile(state, settlement);
  const friendlyCombatants = state.units.filter((unit) =>
    unit.health > 0 &&
    unit.factionId === settlement.factionId &&
    getUnitDef(state.content, unit.typeId).role !== "worker" &&
    distance(unit.x, unit.y, position.x, position.y) <= reserveRadius,
  );
  const enemyCombatants = state.units.filter((unit) =>
    unit.health > 0 &&
    getUnitDef(state.content, unit.typeId).role !== "worker" &&
    areHostile(state, settlement.factionId, unit.factionId) &&
    distance(unit.x, unit.y, position.x, position.y) <= threatRadius,
  );
  const readyReserves = friendlyCombatants.filter((unit) => {
    const unitDef = getUnitDef(state.content, unit.typeId);
    const ratio = unit.health / unitDef.health;
    return ratio >= RESERVE_RECOVERY_HEALTH_RATIO &&
      distance(unit.x, unit.y, position.x, position.y) <= triageRadius * 1.7 &&
      unit.command?.type !== "attack" &&
      unit.command?.type !== "muster";
  }).length;
  const recoveringReserves = friendlyCombatants.filter((unit) =>
    unit.reserveDuty === "fallback" || unit.reserveDuty === "recovering",
  ).length;
  const verdantWardenCount = friendlyCombatants.filter((unit) =>
    isVerdantWardenUnit(unit) &&
    distance(unit.x, unit.y, position.x, position.y) <= VERDANT_WARDEN_ZONE_RADIUS).length;
  const verdantWardenSupport = getVerdantWardenSupportProfile(verdantWardenCount);

  return {
    position,
    threatRadius,
    reserveRadius,
    triageRadius,
    friendlyCombatants,
    enemyCombatants,
    readyReserves,
    recoveringReserves,
    governorProfile,
    wardProfile,
    verdantWardenSupport,
    keepPresence: getKeepPresenceProfile(state, settlement),
  };
}

function getSettlementThreatCenter(defenseState) {
  if ((defenseState?.enemyCombatants?.length ?? 0) === 0) {
    return null;
  }
  const enemyCenter = defenseState.enemyCombatants.reduce(
    (sum, unit) => ({ x: sum.x + unit.x, y: sum.y + unit.y }),
    { x: 0, y: 0 },
  );
  enemyCenter.x /= defenseState.enemyCombatants.length;
  enemyCenter.y /= defenseState.enemyCombatants.length;
  return enemyCenter;
}

function commitSettlementReadyReserves(state, settlement, defenseState, commitCount) {
  if ((defenseState?.enemyCombatants?.length ?? 0) === 0 || commitCount <= 0) {
    return { committedCount: 0, enemyCenter: null };
  }
  const enemyCenter = getSettlementThreatCenter(defenseState);
  if (!enemyCenter) {
    return { committedCount: 0, enemyCenter: null };
  }
  const reserveCandidates = defenseState.friendlyCombatants.filter((unit) => {
    const unitDef = getUnitDef(state.content, unit.typeId);
    const ratio = unit.health / unitDef.health;
    return ratio >= RESERVE_RECOVERY_HEALTH_RATIO &&
      distance(unit.x, unit.y, defenseState.position.x, defenseState.position.y) <= defenseState.reserveRadius * 0.55 &&
      distance(unit.x, unit.y, enemyCenter.x, enemyCenter.y) > defenseState.threatRadius * 0.55 &&
      unit.command?.type !== "attack" &&
      unit.command?.type !== "muster" &&
      unit.command?.type !== "fallback";
  }).sort((left, right) => right.health - left.health);

  const committed = reserveCandidates.slice(0, Math.max(1, commitCount));
  committed.forEach((unit) => {
    unit.command = { type: "muster", x: enemyCenter.x, y: enemyCenter.y };
    unit.reserveDuty = "muster";
    unit.reserveSettlementId = settlement.id;
  });
  if (committed.length > 0) {
    settlement.reserveState.lastCommitAt = state.meta.elapsed;
  }
  return { committedCount: committed.length, enemyCenter };
}

function chooseAiImminentEngagementResponse(profile) {
  if (profile.bloodlineAtRisk || profile.hostileCount > Math.max(1, profile.readyReserves + 1)) {
    return "brace";
  }
  if (profile.commanderPresent && profile.hostileCount <= Math.max(3, profile.readyReserves + 1)) {
    return "counterstroke";
  }
  return "steady";
}

function resetSettlementImminentEngagement(settlement) {
  const prior = ensureImminentEngagementState(settlement);
  settlement.imminentEngagement = {
    ...createDefaultImminentEngagementState(settlement?.id ?? null, settlement?.name ?? null),
    selectedResponseId: prior.selectedResponseId ?? "steady",
    selectedResponseLabel: getImminentEngagementPosture(prior.selectedResponseId).label,
    commanderRecallIssuedAt: prior.commanderRecallIssuedAt ?? -999,
    bloodlineProtectionUntil: prior.bloodlineProtectionUntil ?? 0,
  };
  return settlement.imminentEngagement;
}

function tickImminentEngagementWarnings(state, dt) {
  for (const settlement of state.world.settlements ?? []) {
    if (!settlement.factionId || (settlement.fortificationTier ?? 0) <= 0) {
      continue;
    }
    settlement.reserveState = settlement.reserveState ?? {
      lastCommitAt: -999,
      threatActive: false,
      lastMessageAt: -999,
    };
    const engagement = ensureImminentEngagementState(settlement);
    const defenseState = getSettlementDefenseState(state, settlement);
    const profile = getSettlementImminentEngagementProfile(state, settlement, defenseState);

    if (!profile.active) {
      if (engagement.active) {
        if (settlement.factionId === "player") {
          pushMessage(
            state,
            `${settlement.name} immediate engagement warning breaks before contact fully commits.`,
            "good",
          );
        }
        resetSettlementImminentEngagement(settlement);
      } else if (engagement.windowConsumed) {
        resetSettlementImminentEngagement(settlement);
      }
      continue;
    }

    if (!engagement.active && !engagement.windowConsumed) {
      const preferredResponseId = settlement.factionId === "player"
        ? (engagement.selectedResponseId ?? "steady")
        : chooseAiImminentEngagementResponse(profile);
      const posture = getImminentEngagementPosture(preferredResponseId);
      settlement.imminentEngagement = {
        ...engagement,
        active: true,
        windowConsumed: false,
        settlementId: settlement.id,
        settlementName: settlement.name ?? settlement.id,
        battleTypeId: profile.battleTypeId,
        battleTypeLabel: profile.battleTypeLabel,
        hostileFactionId: profile.hostileFactionId,
        hostileFactionName: profile.hostileFactionName,
        hostileCount: profile.hostileCount,
        hostileSiegeCount: profile.hostileSiegeCount,
        hostileScoutCount: profile.hostileScoutCount,
        watchtowerCount: profile.watchtowerCount,
        warningRadius: profile.warningRadius,
        totalSeconds: profile.warningSeconds,
        expiresAt: state.meta.elapsed + profile.warningSeconds,
        remainingSeconds: profile.warningSeconds,
        localLoyalty: profile.localLoyalty,
        localLoyaltyMin: profile.localLoyaltyMin,
        commanderPresent: profile.commanderPresent,
        commanderRecallAvailable: profile.commanderRecallAvailable,
        governorPresent: profile.governorPresent,
        bloodlineAtRisk: profile.bloodlineAtRisk,
        selectedResponseId: preferredResponseId,
        selectedResponseLabel: posture.label,
        startedAt: state.meta.elapsed,
        engagedAt: 0,
        lastActivationAt: state.meta.elapsed,
      };
      if (settlement.factionId === "player") {
        const hostileLabel = profile.hostileFactionName ?? "hostile forces";
        pushMessage(
          state,
          `${settlement.name} immediate alert: ${hostileLabel} will force a ${profile.battleTypeLabel.toLowerCase()} in ${Math.ceil(profile.warningSeconds)}s.${profile.bloodlineAtRisk ? " Bloodline seat at risk." : ""}`,
          "warn",
        );
      }
    }

    const liveEngagement = ensureImminentEngagementState(settlement);
    liveEngagement.battleTypeId = profile.battleTypeId;
    liveEngagement.battleTypeLabel = profile.battleTypeLabel;
    liveEngagement.hostileFactionId = profile.hostileFactionId;
    liveEngagement.hostileFactionName = profile.hostileFactionName;
    liveEngagement.hostileCount = profile.hostileCount;
    liveEngagement.hostileSiegeCount = profile.hostileSiegeCount;
    liveEngagement.hostileScoutCount = profile.hostileScoutCount;
    liveEngagement.watchtowerCount = profile.watchtowerCount;
    liveEngagement.warningRadius = profile.warningRadius;
    liveEngagement.localLoyalty = profile.localLoyalty;
    liveEngagement.localLoyaltyMin = profile.localLoyaltyMin;
    liveEngagement.commanderPresent = profile.commanderPresent;
    liveEngagement.commanderRecallAvailable = profile.commanderRecallAvailable;
    liveEngagement.governorPresent = profile.governorPresent;
    liveEngagement.bloodlineAtRisk = profile.bloodlineAtRisk;
    liveEngagement.remainingSeconds = Math.max(0, (liveEngagement.expiresAt ?? 0) - state.meta.elapsed);

    if (settlement.factionId !== "player" && liveEngagement.active) {
      const desiredResponseId = chooseAiImminentEngagementResponse(profile);
      if (liveEngagement.selectedResponseId !== desiredResponseId) {
        liveEngagement.selectedResponseId = desiredResponseId;
        liveEngagement.selectedResponseLabel = getImminentEngagementPosture(desiredResponseId).label;
      }
      if (
        profile.commanderRecallAvailable &&
        (state.meta.elapsed - (liveEngagement.commanderRecallIssuedAt ?? -999)) >= 10
      ) {
        issueImminentEngagementCommanderRecall(state, settlement.factionId, settlement.id);
      }
      if (
        (profile.hostileCount > Math.max(1, profile.readyReserves) || profile.bloodlineAtRisk) &&
        (liveEngagement.reinforcementsCommittedUntil ?? 0) <= state.meta.elapsed
      ) {
        commitImminentEngagementReinforcements(state, settlement.factionId, settlement.id);
      }
    }

    if (liveEngagement.active && liveEngagement.remainingSeconds <= 0) {
      liveEngagement.active = false;
      liveEngagement.windowConsumed = true;
      liveEngagement.engagedAt = state.meta.elapsed;
      liveEngagement.remainingSeconds = 0;
      const posture = getImminentEngagementPosture(liveEngagement.selectedResponseId);
      if (posture.autoSortieOnExpiry && settlement.settlementClass === "primary_dynastic_keep") {
        issueKeepSortie(state, settlement.factionId, settlement.id);
      }
      if (settlement.factionId === "player") {
        pushMessage(
          state,
          `${settlement.name} engagement window closes. Battle proceeds under ${posture.label.toLowerCase()}.`,
          "warn",
        );
      }
    }
  }
}

function tickFortificationReserves(state, dt) {
  for (const settlement of state.world.settlements ?? []) {
    if (!settlement.factionId || (settlement.fortificationTier ?? 0) <= 0) {
      continue;
    }

    settlement.reserveState = settlement.reserveState ?? {
      lastCommitAt: -999,
      threatActive: false,
      lastMessageAt: -999,
    };

    const defenseState = getSettlementDefenseState(state, settlement);
    const {
      position,
      threatRadius,
      reserveRadius,
      triageRadius,
      friendlyCombatants,
      enemyCombatants,
      keepPresence,
      governorProfile,
    } = defenseState;
    maybeTriggerBloodAltarSurge(state, settlement, defenseState);
    const wardProfile = getFortificationFaithWardProfile(state, settlement);
    const imminentDirective = getSettlementImminentEngagementDirective(settlement, state.meta.elapsed);

    const healRate = RESERVE_TRIAGE_HEAL_PER_SECOND +
      (settlement.fortificationTier ?? 0) * 0.8 +
      (keepPresence.commanderPresent ? 1.5 : 0) +
      (keepPresence.bloodlinePresent ? 1.2 : 0);
    const effectiveHealRate = healRate *
      governorProfile.reserveHealMultiplier *
      wardProfile.reserveHealMultiplier *
      imminentDirective.posture.reserveHealMultiplier *
      (imminentDirective.reinforcementSurgeActive ? 1.14 : 1) *
      (defenseState.verdantWardenSupport.reserveHealMultiplier ?? 1) *
      (keepPresence.governorPresent ? 1.05 : 1);

    friendlyCombatants.forEach((unit) => {
      const unitDef = getUnitDef(state.content, unit.typeId);
      if (
        distance(unit.x, unit.y, position.x, position.y) <= triageRadius &&
        (unit.reserveDuty === "fallback" || unit.reserveDuty === "recovering" || enemyCombatants.length === 0)
      ) {
        unit.health = Math.min(unitDef.health, unit.health + effectiveHealRate * dt);
        if ((unit.health / unitDef.health) >= RESERVE_RECOVERY_HEALTH_RATIO && unit.reserveDuty === "recovering") {
          unit.reserveDuty = "ready";
          unit.reserveSettlementId = settlement.id;
        }
      }
    });

    settlement.reserveState.threatActive = enemyCombatants.length > 0;
    if (enemyCombatants.length === 0) {
      continue;
    }

    const retreatThreshold = RESERVE_RETREAT_HEALTH_RATIO +
      (keepPresence.commanderPresent ? 0.05 : 0) +
      (keepPresence.bloodlinePresent ? 0.03 : 0) +
      imminentDirective.posture.retreatThresholdBonus +
      (wardProfile.id === "blood_dominion" && wardProfile.surgeActive ? 0.02 : 0);
    const enemyCenter = getSettlementThreatCenter(defenseState);

    const retreatingUnits = friendlyCombatants
      .filter((unit) => {
        const unitDef = getUnitDef(state.content, unit.typeId);
        return (unit.health / unitDef.health) <= retreatThreshold &&
          distance(unit.x, unit.y, enemyCenter.x, enemyCenter.y) <= threatRadius * 0.75 &&
          unit.command?.type !== "fallback";
      })
      .sort((left, right) => left.health - right.health);

    retreatingUnits
      .slice(0, Math.max(1, settlement.fortificationTier ?? 1))
      .forEach((unit) => {
        unit.command = { type: "fallback", x: position.x, y: position.y };
        unit.reserveDuty = "fallback";
        unit.reserveSettlementId = settlement.id;
      });

    const engagedCount = friendlyCombatants.filter((unit) =>
      distance(unit.x, unit.y, enemyCenter.x, enemyCenter.y) <= threatRadius * 0.65 ||
      unit.command?.type === "attack" ||
      unit.reserveDuty === "engaged",
    ).length;
    const desiredFrontline = Math.min(
      friendlyCombatants.length,
      1 +
        (settlement.fortificationTier ?? 0) +
        (keepPresence.commanderPresent ? 1 : 0) +
        (keepPresence.bloodlinePresent ? 1 : 0) +
        (governorProfile.id === "keep" ? 1 : 0) +
        imminentDirective.posture.desiredFrontlineBonus +
        (imminentDirective.reinforcementSurgeActive ? 1 : 0) +
        (defenseState.verdantWardenSupport.desiredFrontlineBonus ?? 0) +
        (wardProfile.surgeActive ? 1 : 0),
    );
    const neededReserves = Math.max(0, desiredFrontline - engagedCount);
    if (neededReserves <= 0) {
      continue;
    }

    const musterInterval = RESERVE_MUSTER_INTERVAL_SECONDS / Math.max(
      1,
      (1 + (keepPresence.commanderPresent ? 0.25 : 0) + (keepPresence.bloodlinePresent ? 0.2 : 0)) *
        governorProfile.reserveMusterMultiplier *
        wardProfile.reserveMusterMultiplier *
        imminentDirective.posture.reserveMusterMultiplier *
        (imminentDirective.reinforcementSurgeActive ? 1.18 : 1) *
        (defenseState.verdantWardenSupport.reserveMusterMultiplier ?? 1),
    );
    if ((state.meta.elapsed - settlement.reserveState.lastCommitAt) < musterInterval) {
      continue;
    }

    const committed = commitSettlementReadyReserves(
      state,
      settlement,
      defenseState,
      Math.max(1, neededReserves),
    );

    if (committed.committedCount > 0) {
      if (
        settlement.factionId === "player" &&
        (state.meta.elapsed - settlement.reserveState.lastMessageAt) > 10
      ) {
        settlement.reserveState.lastMessageAt = state.meta.elapsed;
        pushMessage(
          state,
          `${settlement.name} cycles fresh reserves to the wall while wounded defenders fall back for triage.`,
          "info",
        );
      }
    }
  }
}

function getLocalFortifiedSettlementForUnit(state, factionId, x, y) {
  let best = null;
  for (const settlement of state.world.settlements ?? []) {
    if (settlement.factionId !== factionId || (settlement.fortificationTier ?? 0) <= 0) {
      continue;
    }
    const defenseState = getSettlementDefenseState(state, settlement);
    const d = distance(x, y, defenseState.position.x, defenseState.position.y);
    if (d <= defenseState.reserveRadius && (!best || d < best.distance)) {
      best = { settlement, defenseState, distance: d };
    }
  }
  return best;
}

function getFriendlyFortificationCombatProfile(state, unit) {
  const local = getLocalFortifiedSettlementForUnit(state, unit.factionId, unit.x, unit.y);
  if (!local) {
    return null;
  }

  const { settlement, defenseState } = local;
  const imminentDirective = getSettlementImminentEngagementDirective(settlement, state.meta.elapsed);
  const keepPresenceAttackMultiplier =
    (defenseState.keepPresence.commanderPresent ? 1.08 : 1) *
    (defenseState.keepPresence.bloodlinePresent ? 1.04 : 1) *
    (defenseState.keepPresence.governorPresent && defenseState.governorProfile.id === "keep" ? 1.04 : 1);

  // Session 10: Commander keep-presence sortie. When a commander-led sortie is active,
  // nearby defenders temporarily gain attack and sight leverage.
  const sortieActive = (settlement.sortieActiveUntil ?? 0) > state.meta.elapsed;
  const sortieAttackMultiplier = sortieActive ? 1.22 : 1;
  const sortieSightBonus = sortieActive ? 22 : 0;

  return {
    settlement,
    defenseState,
    sortieActive,
    sightBonus:
      defenseState.wardProfile.sightBonus +
      (defenseState.keepPresence.commanderPresent ? 12 : 0) +
      (defenseState.keepPresence.bloodlinePresent ? 6 : 0) +
      imminentDirective.posture.defenderSightBonus +
      sortieSightBonus,
    attackMultiplier:
      defenseState.wardProfile.defenderAttackMultiplier *
      (defenseState.verdantWardenSupport.defenderAttackMultiplier ?? 1) *
      keepPresenceAttackMultiplier *
      imminentDirective.posture.defenderAttackMultiplier *
      sortieAttackMultiplier,
  };
}

// Session 10: Commander keep-presence sortie.
// The canonical commander-at-keep expansion. Requires commander present, under threat,
// and cooldown not active. On invoke: all nearby defenders gain a 12-second sortie buff
// (attack ×1.22, sight +22) via the defensive combat profile read path.
export const SORTIE_DURATION_SECONDS = 12;
export const SORTIE_COOLDOWN_SECONDS = 60;

export function setImminentEngagementPosture(state, factionId, settlementId, postureId) {
  const settlement = (state.world.settlements ?? []).find(
    (entry) => entry.id === settlementId && entry.factionId === factionId,
  );
  if (!settlement) {
    return { ok: false, reason: "Settlement not found for imminent engagement posture." };
  }
  const engagement = ensureImminentEngagementState(settlement);
  if (!engagement.active) {
    return { ok: false, reason: "No active imminent engagement window at this settlement." };
  }
  const posture = IMMINENT_ENGAGEMENT_POSTURES[postureId];
  if (!posture) {
    return { ok: false, reason: "Unknown imminent engagement posture." };
  }
  engagement.selectedResponseId = posture.id;
  engagement.selectedResponseLabel = posture.label;
  if (factionId === "player") {
    pushMessage(
      state,
      `${settlement.name} posture shifts to ${posture.label.toLowerCase()}.`,
      "info",
    );
  }
  return { ok: true };
}

export function issueImminentEngagementCommanderRecall(state, factionId, settlementId) {
  const settlement = (state.world.settlements ?? []).find(
    (entry) => entry.id === settlementId && entry.factionId === factionId,
  );
  if (!settlement) {
    return { ok: false, reason: "Settlement not found for commander recall." };
  }
  const faction = getFaction(state, factionId);
  const commanderUnitId = faction?.dynasty?.attachments?.commanderUnitId;
  const commanderUnit = commanderUnitId ? getEntity(state, "unit", commanderUnitId) : null;
  if (!commanderUnit || commanderUnit.health <= 0) {
    return { ok: false, reason: "No living commander unit is available to recall." };
  }
  const defenseState = getSettlementDefenseState(state, settlement);
  if (defenseState.keepPresence.commanderPresent) {
    return { ok: false, reason: "Commander is already present at the settlement." };
  }
  issueMoveCommand(state, [commanderUnit.id], defenseState.position.x, defenseState.position.y);
  const engagement = ensureImminentEngagementState(settlement);
  engagement.commanderRecallIssuedAt = state.meta.elapsed;
  if (factionId === "player") {
    pushMessage(
      state,
      `${settlement.name} recalls the commander to the threatened seat.`,
      "warn",
    );
  }
  return { ok: true };
}

export function commitImminentEngagementReinforcements(state, factionId, settlementId) {
  const settlement = (state.world.settlements ?? []).find(
    (entry) => entry.id === settlementId && entry.factionId === factionId,
  );
  if (!settlement) {
    return { ok: false, reason: "Settlement not found for reinforcement commitment." };
  }
  const engagement = ensureImminentEngagementState(settlement);
  if (!engagement.active) {
    return { ok: false, reason: "No active imminent engagement window at this settlement." };
  }
  if ((engagement.reinforcementsCommittedUntil ?? 0) > state.meta.elapsed) {
    return {
      ok: false,
      reason: `Reinforcements already committed for ${Math.ceil(engagement.reinforcementsCommittedUntil - state.meta.elapsed)}s.`,
    };
  }
  const defenseState = getSettlementDefenseState(state, settlement);
  if ((defenseState.enemyCombatants?.length ?? 0) === 0) {
    return { ok: false, reason: "No hostile pressure is active at the settlement." };
  }
  const desiredCommit = Math.max(
    1,
    Math.min(
      defenseState.readyReserves,
      Math.ceil(((engagement.hostileCount ?? 0) + (engagement.bloodlineAtRisk ? 2 : 0)) / 2),
    ),
  );
  const commit = commitSettlementReadyReserves(state, settlement, defenseState, desiredCommit);
  if (commit.committedCount <= 0) {
    return { ok: false, reason: "No ready reserves can be committed right now." };
  }
  engagement.reinforcementsCommittedUntil = state.meta.elapsed + IMMINENT_ENGAGEMENT_REINFORCEMENT_SURGE_SECONDS;
  recordConvictionEvent(state, factionId, "oathkeeping", 1, `Committed keep reinforcements to ${settlement.name}`);
  if (factionId === "player") {
    pushMessage(
      state,
      `${settlement.name} commits ${commit.committedCount} ready reserve${commit.committedCount === 1 ? "" : "s"} into the engagement corridor.`,
      "warn",
    );
  }
  return { ok: true, committedCount: commit.committedCount };
}

export function protectImminentEngagementBloodline(state, factionId, settlementId) {
  const settlement = (state.world.settlements ?? []).find(
    (entry) => entry.id === settlementId && entry.factionId === factionId,
  );
  if (!settlement) {
    return { ok: false, reason: "Settlement not found for bloodline protection." };
  }
  const engagement = ensureImminentEngagementState(settlement);
  if (!engagement.active) {
    return { ok: false, reason: "No active imminent engagement window at this settlement." };
  }
  if (!engagement.bloodlineAtRisk) {
    return { ok: false, reason: "No seated bloodline members are at immediate risk here." };
  }
  if ((engagement.bloodlineProtectionUntil ?? 0) > state.meta.elapsed) {
    return {
      ok: false,
      reason: `Bloodline guard already active for ${Math.ceil(engagement.bloodlineProtectionUntil - state.meta.elapsed)}s.`,
    };
  }
  engagement.bloodlineProtectionUntil = state.meta.elapsed + Math.max(24, (engagement.remainingSeconds ?? 0) + 10);
  recordConvictionEvent(state, factionId, "stewardship", 1, `Raised emergency bloodline guard at ${settlement.name}`);
  pushMessage(
    state,
    `${settlement.name} raises an emergency bloodline guard around the ruling seat.`,
    factionId === "player" ? "warn" : "info",
  );
  return { ok: true };
}

export function issueKeepSortie(state, factionId, settlementId) {
  const settlement = (state.world.settlements ?? []).find(
    (s) => s.id === settlementId && s.factionId === factionId,
  );
  if (!settlement) {
    return { ok: false, reason: "Settlement not found for sortie." };
  }
  const defenseState = getSettlementDefenseState(state, settlement);
  if (!defenseState?.keepPresence?.commanderPresent) {
    return { ok: false, reason: "Commander must be present at the keep to sortie." };
  }
  if (!(defenseState.enemyCombatants?.length > 0)) {
    return { ok: false, reason: "No active threat at the keep." };
  }
  if ((settlement.sortieCooldownUntil ?? 0) > state.meta.elapsed) {
    return {
      ok: false,
      reason: `Sortie on cooldown for ${Math.ceil((settlement.sortieCooldownUntil - state.meta.elapsed))}s.`,
    };
  }
  settlement.sortieActiveUntil = state.meta.elapsed + SORTIE_DURATION_SECONDS;
  settlement.sortieCooldownUntil = state.meta.elapsed + SORTIE_COOLDOWN_SECONDS;
  recordConvictionEvent(state, factionId, "oathkeeping", 1, `Commander sortie called from ${settlement.name}`);
  pushMessage(
    state,
    `${getFactionDisplayName(state, factionId)} commander calls a sortie from ${settlement.name}.`,
    factionId === "player" ? "good" : "info",
  );
  return { ok: true };
}

function getSiegeOperationalProfile(state, unit) {
  const unitDef = getUnitDef(state.content, unit.typeId);
  const supplied = (unit.siegeSuppliedUntil ?? 0) > state.meta.elapsed;
  const engineerSupported = (unit.engineerSupportUntil ?? 0) > state.meta.elapsed;
  const isSiegeEngine = Boolean(unitDef?.siegeClass);

  return {
    supplied,
    engineerSupported,
    attackMultiplier:
      (isSiegeEngine && !supplied ? SIEGE_UNSUPPLIED_ATTACK_MULTIPLIER : 1) *
      (isSiegeEngine && engineerSupported ? 1.1 : 1),
    speedMultiplier:
      isSiegeEngine && !supplied
        ? SIEGE_UNSUPPLIED_SPEED_MULTIPLIER
        : 1,
  };
}

function getFriendlySiegeSupportProfile(state, unit, targetType) {
  if (targetType !== "building") {
    return null;
  }

  let structuralSupportMultiplier = 1;
  let attackRangeBonus = 0;
  state.units.forEach((candidate) => {
    if (candidate.id === unit.id || candidate.factionId !== unit.factionId || candidate.health <= 0) {
      return;
    }

    const candidateDef = getUnitDef(state.content, candidate.typeId);
    const supportRadius =
      candidateDef.siegeClass === "siege_tower"
        ? candidateDef.assaultSupportRadius ?? 0
        : candidateDef.role === "engineer-specialist"
          ? candidateDef.engineerSupportRadius ?? 0
          : 0;
    if (supportRadius <= 0 || distance(unit.x, unit.y, candidate.x, candidate.y) > supportRadius) {
      return;
    }
    structuralSupportMultiplier = Math.max(
      structuralSupportMultiplier,
      candidateDef.alliedStructuralSupportMultiplier ?? 1,
    );
    attackRangeBonus = Math.max(
      attackRangeBonus,
      candidateDef.alliedAttackRangeBonus ?? 0,
    );
  });

  if (structuralSupportMultiplier === 1 && attackRangeBonus === 0) {
    return null;
  }

  return {
    structuralSupportMultiplier,
    attackRangeBonus,
  };
}

// Mantlet cover: friendly mantlets within coverRadius of a target unit reduce
// inbound ranged damage. This is the canonical "mobile cover" siege-support pillar
// (Session 9 addition). Returns a multiplier in [0.0, 1.0] applied to inbound ranged damage.
function getIncomingRangedCoverMultiplier(state, targetUnit) {
  if (!targetUnit || targetUnit.health <= 0) {
    return 1;
  }
  let bestMultiplier = 1;
  const mantlets = getFactionMantlets(state, targetUnit.factionId);
  for (const mantlet of mantlets) {
    const mantletDef = getUnitDef(state.content, mantlet.typeId);
    const coverRadius = mantletDef.coverRadius ?? 0;
    if (coverRadius <= 0) continue;
    if (distance(mantlet.x, mantlet.y, targetUnit.x, targetUnit.y) > coverRadius) continue;
    const reduction = mantletDef.coverInboundRangedMultiplier ?? 1;
    if (reduction < bestMultiplier) {
      bestMultiplier = reduction;
    }
  }
  return bestMultiplier;
}

function getSupportCommandTargetPosition(state, command) {
  if (!command) {
    return null;
  }
  if (command.type === "move" || command.type === "raid_retreat" || command.type === "fallback" || command.type === "muster") {
    return { x: command.x, y: command.y };
  }
  if (command.type === "attack") {
    const target = getEntity(state, command.targetType, command.targetId);
    if (!target || target.health <= 0) {
      return null;
    }
    return command.targetType === "unit"
      ? { x: target.x, y: target.y }
      : getBuildingCenter(state, target, getBuildingDef(state.content, target.typeId));
  }
  return null;
}

// Session 10: processes per-tick status effects applied by sabotage operations.
// Burn DOT from fire_raising applies damage over its remaining window.
// Supply poisoning and gate-open exposure are checked at read sites (this tick
// just clears expired flags so state stays clean).
function tickBuildingStatusEffects(state, dt) {
  const elapsed = state.meta.elapsed;
  state.buildings.forEach((building) => {
    if (building.health <= 0) return;
    if (building.burnExpiresAt && elapsed < building.burnExpiresAt && (building.burnDamagePerSecond ?? 0) > 0) {
      const tickDamage = building.burnDamagePerSecond * dt;
      applyDamage(state, "building", building.id, tickDamage, null, null);
      return;
    }
    if (building.burnExpiresAt && elapsed >= building.burnExpiresAt) {
      building.burnExpiresAt = 0;
      building.burnDamagePerSecond = 0;
    }
    if (building.poisonedUntil && elapsed >= building.poisonedUntil) {
      building.poisonedUntil = 0;
    }
    if (building.sabotageGateExposedUntil && elapsed >= building.sabotageGateExposedUntil) {
      building.sabotageGateExposedUntil = 0;
    }
    if (building.raidedUntil && elapsed >= building.raidedUntil) {
      building.raidedUntil = 0;
    }
  });
}

function tickResourceNodeHarassmentState(state) {
  const elapsed = state.meta.elapsed;
  (state.world.resourceNodes ?? []).forEach((resourceNode) => {
    if ((resourceNode.harassedUntil ?? 0) > 0 && elapsed >= (resourceNode.harassedUntil ?? 0)) {
      const recoveringFactionId = resourceNode.harassedTargetFactionId ?? null;
      resourceNode.harassedUntil = 0;
      resourceNode.harassedTargetFactionId = null;
      resourceNode.harassedByFactionId = null;
      state.units.forEach((unit) => {
        if (
          unit.health > 0 &&
          unit.command?.type === "harass_retreat" &&
          unit.command.nodeId === resourceNode.id &&
          (!recoveringFactionId || unit.factionId === recoveringFactionId) &&
          (unit.command.resumeAt ?? 0) <= elapsed
        ) {
          unit.command = resourceNode.amount > 0
            ? { type: "gather", nodeId: resourceNode.id }
            : null;
          unit.gatherProgress = 0;
        }
      });
    }
  });
}

function tickWorkerHarassRecovery(state) {
  state.units.forEach((unit) => {
    if (unit.health <= 0 || unit.command?.type !== "harass_retreat") {
      return;
    }
    const unitDef = getUnitDef(state.content, unit.typeId);
    if (!unitDef || unitDef.role !== "worker") {
      return;
    }
    const resourceNode = unit.command.nodeId
      ? state.world.resourceNodes.find((node) => node.id === unit.command.nodeId)
      : null;
    const nodeStillHarassed = resourceNode
      ? isResourceNodeUnderScoutHarassment(state, resourceNode, unit.factionId)
      : false;
    if (resourceNode && resourceNode.amount > 0 && !nodeStillHarassed && (unit.command.resumeAt ?? 0) <= state.meta.elapsed) {
      unit.command = { type: "gather", nodeId: resourceNode.id };
      unit.gatherProgress = 0;
      return;
    }
    if (!resourceNode || resourceNode.amount <= 0) {
      unit.command = null;
    }
  });
}

function tickSupplyWagonInterdictionState(state) {
  const elapsed = state.meta.elapsed ?? 0;
  state.units.forEach((unit) => {
    const unitDef = getUnitDef(state.content, unit.typeId);
    if (!isMovingLogisticsCarrierUnitDef(unitDef)) {
      return;
    }
    if ((unit.logisticsInterdictedUntil ?? 0) > 0 && elapsed >= (unit.logisticsInterdictedUntil ?? 0)) {
      unit.logisticsInterdictedUntil = 0;
      unit.interdictedByFactionId = null;
    }
    if ((unit.convoyRecoveryUntil ?? 0) > 0 && elapsed >= (unit.convoyRecoveryUntil ?? 0)) {
      unit.convoyRecoveryUntil = 0;
    }
  });
}

function tickSiegeSupportLogistics(state, dt) {
  state.units.forEach((unit) => {
    const unitDef = getUnitDef(state.content, unit.typeId);
    if (unit.health > 0 && isSupportRole(unitDef)) {
      unit.supportStatus = "idle";
    }
  });

  for (const factionId of Object.keys(state.factions)) {
    const faction = getFaction(state, factionId);
    if (!faction || faction.kind !== "kingdom") {
      continue;
    }

    const siegeState = getFactionSiegeState(state, factionId);
    if (siegeState.engineCount === 0) {
      continue;
    }

    siegeState.engineers.forEach((engineer) => {
      const engineerDef = getUnitDef(state.content, engineer.typeId);
      const supportRadius = engineerDef.engineerSupportRadius ?? 0;
      const nearbyEngines = siegeState.engines.filter((engine) =>
        distance(engineer.x, engineer.y, engine.x, engine.y) <= supportRadius,
      );
      if (nearbyEngines.length === 0) {
        const targetPosition = getSupportCommandTargetPosition(state, engineer.command);
        engineer.supportStatus = targetPosition ? "advance" : "idle";
        return;
      }

      engineer.supportStatus = "earthworks";
      nearbyEngines.forEach((engine) => {
        const engineDef = getUnitDef(state.content, engine.typeId);
        engine.engineerSupportUntil = Math.max(engine.engineerSupportUntil ?? 0, state.meta.elapsed + SIEGE_SUPPORT_REFRESH_SECONDS);
        if (engine.health < engineDef.health) {
          engine.health = Math.min(
            engineDef.health,
            engine.health + (engineerDef.siegeRepairPerSecond ?? 0) * dt,
          );
          engineer.supportStatus = "repair";
        }
      });
    });

    siegeState.supplyWagons.forEach((wagon) => {
      const wagonDef = getUnitDef(state.content, wagon.typeId);
      if (isSupplyWagonInterdicted(state, wagon)) {
        wagon.supportStatus = "interdicted";
        return;
      }
      const linkedCamp = getLinkedSupplyCampForWagon(state, wagon, siegeState.supplyCamps);
      if (!linkedCamp) {
        wagon.supportStatus = "cut-off";
        return;
      }
      const escortCoverage = getSupplyWagonEscortCoverage(state, factionId, wagon);
      if (isSupplyWagonRecovering(state, wagon)) {
        wagon.supportStatus = escortCoverage.screened ? "screened" : "reforming";
        return;
      }

      const nearbyEngines = siegeState.engines.filter((engine) =>
        distance(wagon.x, wagon.y, engine.x, engine.y) <= (wagonDef.supplyRadius ?? 0),
      );
      if (nearbyEngines.length === 0) {
        const targetPosition = getSupportCommandTargetPosition(state, wagon.command);
        wagon.supportStatus = escortCoverage.screened
          ? "screened"
          : targetPosition ? "advance" : "idle";
        return;
      }

      if ((state.meta.elapsed - (wagon.lastSupplyTransferAt ?? -999)) >= (wagonDef.supplyTransferSeconds ?? 3)) {
        const transferCost = Object.fromEntries(
          Object.entries(wagonDef.supplyCostPerTransfer ?? {}).map(([resourceId, amount]) => [
            resourceId,
            amount * Math.max(1, Math.min(2, nearbyEngines.length)),
          ]),
        );
        if (canAffordCost(faction.resources, transferCost)) {
          applyCost(faction.resources, transferCost);
          nearbyEngines.forEach((engine) => {
            engine.siegeSuppliedUntil = Math.max(
              engine.siegeSuppliedUntil ?? 0,
              state.meta.elapsed + (wagonDef.supplyDurationSeconds ?? 10),
            );
          });
          wagon.lastSupplyTransferAt = state.meta.elapsed;
          wagon.supportStatus = "supplying";
        } else {
          wagon.supportStatus = "starved";
        }
      } else {
        wagon.supportStatus = "supplying";
      }
    });

    if (factionId !== "player") {
      continue;
    }

    const logistics = getFactionSiegeLogisticsState(faction);
    if (
      siegeState.unsuppliedEngines > 0 &&
      (state.meta.elapsed - (logistics.lastUnsuppliedMessageAt ?? -999)) >= SIEGE_UNSUPPLIED_MESSAGE_INTERVAL_SECONDS
    ) {
      logistics.lastUnsuppliedMessageAt = state.meta.elapsed;
      pushMessage(
        state,
        `Ironmark siege engines are outrunning supply continuity. Secure a live camp and wagon route.`,
        "warn",
      );
    } else if (
      siegeState.engineCount > 0 &&
      siegeState.unsuppliedEngines === 0 &&
      (state.meta.elapsed - (logistics.lastResuppliedMessageAt ?? -999)) >= SIEGE_RESUPPLIED_MESSAGE_INTERVAL_SECONDS
    ) {
      logistics.lastResuppliedMessageAt = state.meta.elapsed;
      pushMessage(
        state,
        `Ironmark siege train resupplied. Engineers and wagons keep the line operational.`,
        "good",
      );
    }
  }
}

function tickFieldWaterLogistics(state, dt) {
  for (const factionId of Object.keys(state.factions)) {
    const faction = getFaction(state, factionId);
    if (!faction || faction.kind !== "kingdom") {
      continue;
    }

    const fieldWaterState = getFactionFieldWaterState(state, factionId);
    fieldWaterState.units.forEach((unit) => {
      ensureFieldWaterState(unit);
      const supportProfile = getFieldWaterSupportProfile(state, factionId, unit);
      if (supportProfile.available) {
        refreshFieldWaterSupport(state, faction, unit, supportProfile.durationSeconds);
      }

      if ((unit.fieldWaterSuppliedUntil ?? 0) > state.meta.elapsed) {
        unit.fieldWaterStrain = Math.max(0, (unit.fieldWaterStrain ?? 0) - dt * FIELD_WATER_RECOVERY_PER_SECOND);
      } else {
        unit.fieldWaterStrain = Math.min(
          FIELD_WATER_CRITICAL_THRESHOLD * 2,
          (unit.fieldWaterStrain ?? 0) + dt * FIELD_WATER_STRAIN_PER_SECOND,
        );
      }

      if ((unit.fieldWaterStrain ?? 0) >= FIELD_WATER_CRITICAL_THRESHOLD) {
        unit.fieldWaterStatus = "critical";
      } else if ((unit.fieldWaterStrain ?? 0) >= FIELD_WATER_STRAIN_THRESHOLD) {
        unit.fieldWaterStatus = "strained";
      } else if ((unit.fieldWaterSuppliedUntil ?? 0) > state.meta.elapsed) {
        unit.fieldWaterStatus = (unit.fieldWaterStrain ?? 0) > 0.2 ? "recovering" : "steady";
      } else {
        unit.fieldWaterStatus = "dry";
      }

      const disciplineProfile = getFieldWaterDisciplineProfile(state, unit);
      if (unit.fieldWaterStatus === "critical") {
        unit.fieldWaterCriticalDuration = Math.min(
          disciplineProfile.desertionThresholdSeconds + 18,
          (unit.fieldWaterCriticalDuration ?? 0) + dt,
        );
      } else {
        unit.fieldWaterCriticalDuration = Math.max(
          0,
          (unit.fieldWaterCriticalDuration ?? 0) - dt * FIELD_WATER_CRITICAL_RECOVERY_PER_SECOND,
        );
      }

      unit.fieldWaterAttritionActive =
        unit.fieldWaterStatus === "critical" &&
        (unit.fieldWaterCriticalDuration ?? 0) >= disciplineProfile.attritionThresholdSeconds;
      unit.fieldWaterDesertionRisk =
        unit.fieldWaterStatus === "critical" &&
        (unit.fieldWaterCriticalDuration ?? 0) >= disciplineProfile.desertionThresholdSeconds;

      if (unit.fieldWaterAttritionActive && unit.health > 0) {
        const wasAlive = unit.health > 0;
        applyDamage(state, "unit", unit.id, dt * disciplineProfile.attritionDamagePerSecond, null, null);
        if (wasAlive && unit.health <= 0) {
          unit.deathReason = "field_water_attrition";
        }
      }

      if (unit.health > 0 && unit.fieldWaterDesertionRisk) {
        const unitDef = getUnitDef(state.content, unit.typeId);
        const desertionHealthFloor = (unitDef?.health ?? unit.health) * FIELD_WATER_DESERTION_HEALTH_RATIO;
        if (unit.health <= desertionHealthFloor) {
          unit.health = 0;
          unit.killedByFactionId = null;
          unit.deathReason = "field_water_desertion";
        }
      }
    });

    if (factionId !== "player") {
      continue;
    }

    const alerts = ensureFieldWaterAlertState(faction);
    const criticalUnits = fieldWaterState.units.filter((unit) => unit.fieldWaterStatus === "critical").length;
    const strainedUnits = fieldWaterState.units.filter((unit) =>
      unit.fieldWaterStatus === "critical" || unit.fieldWaterStatus === "strained").length;
    const attritionUnits = fieldWaterState.units.filter((unit) => unit.fieldWaterAttritionActive).length;
    const desertionRiskUnits = fieldWaterState.units.filter((unit) => unit.fieldWaterDesertionRisk).length;
    if (
      desertionRiskUnits > 0 &&
      (state.meta.elapsed - (alerts.lastDesertionMessageAt ?? -999)) >= FIELD_WATER_MESSAGE_INTERVAL_SECONDS
    ) {
      alerts.lastDesertionMessageAt = state.meta.elapsed;
      pushMessage(
        state,
        `Ironmark field armies are starting to break for water. ${desertionRiskUnits} unit${desertionRiskUnits === 1 ? "" : "s"} face desertion if the line stays dry.`,
        "warn",
      );
    } else if (
      attritionUnits > 0 &&
      (state.meta.elapsed - (alerts.lastAttritionMessageAt ?? -999)) >= FIELD_WATER_MESSAGE_INTERVAL_SECONDS
    ) {
      alerts.lastAttritionMessageAt = state.meta.elapsed;
      pushMessage(
        state,
        `Critical dehydration is bleeding Ironmark's line. ${attritionUnits} unit${attritionUnits === 1 ? "" : "s"} are taking field-water attrition.`,
        "warn",
      );
    } else if (
      criticalUnits > 0 &&
      (state.meta.elapsed - (alerts.lastCriticalMessageAt ?? -999)) >= FIELD_WATER_MESSAGE_INTERVAL_SECONDS
    ) {
      alerts.lastCriticalMessageAt = state.meta.elapsed;
      pushMessage(
        state,
        `Ironmark field armies are losing water discipline. ${criticalUnits} unit${criticalUnits === 1 ? "" : "s"} are critically dehydrated.`,
        "warn",
      );
    } else if (
      strainedUnits === 0 &&
      fieldWaterState.unitCount > 0 &&
      attritionUnits === 0 &&
      desertionRiskUnits === 0 &&
      (state.meta.elapsed - (alerts.lastRecoveredMessageAt ?? -999)) >= FIELD_WATER_MESSAGE_INTERVAL_SECONDS
    ) {
      alerts.lastRecoveredMessageAt = state.meta.elapsed;
      pushMessage(
        state,
        `Ironmark field armies have re-established water support.`,
        "good",
      );
    }
  }
}

function getHostileFortificationApproachSpeedMultiplier(state, unit) {
  let speedMultiplier = 1;
  for (const settlement of state.world.settlements ?? []) {
    if (!settlement.factionId || !areHostile(state, unit.factionId, settlement.factionId) || (settlement.fortificationTier ?? 0) <= 0) {
      continue;
    }
    const defenseState = getSettlementDefenseState(state, settlement);
    if (distance(unit.x, unit.y, defenseState.position.x, defenseState.position.y) <= defenseState.threatRadius) {
      const imminentDirective = getSettlementImminentEngagementDirective(settlement, state.meta.elapsed);
      speedMultiplier = Math.min(
        speedMultiplier,
        defenseState.wardProfile.enemySpeedMultiplier ?? 1,
        imminentDirective.posture.enemyApproachMultiplier ?? 1,
      );
    }
  }
  return speedMultiplier;
}

function getAssaultCohesionMultiplier(state, factionId) {
  const faction = getFaction(state, factionId);
  if (!faction) return 1;
  if ((faction.cohesionPenaltyUntil ?? 0) > state.meta.elapsed) {
    return ASSAULT_COHESION_PENALTY_MULTIPLIER;
  }
  return 1;
}

function tickAssaultCohesion(state, dt) {
  for (const factionId of Object.keys(state.factions)) {
    const faction = state.factions[factionId];
    const strain = faction.assaultFailureStrain ?? 0;
    if (strain > 0) {
      faction.assaultFailureStrain = Math.max(0, strain - dt * ASSAULT_STRAIN_DECAY_PER_SECOND);
    }
    if ((faction.assaultFailureStrain ?? 0) >= ASSAULT_STRAIN_THRESHOLD) {
      const stillActive = (faction.cohesionPenaltyUntil ?? 0) > state.meta.elapsed;
      if (!stillActive) {
        faction.cohesionPenaltyUntil = state.meta.elapsed + ASSAULT_COHESION_PENALTY_DURATION;
        faction.assaultFailureStrain = 0;
        recordConvictionEvent(state, factionId, "desecration", 1, "Assault force shaken by fortification repulsion");
        pushMessage(
          state,
          `${getFactionDisplayName(state, factionId)} assault force shaken — cohesion falters.`,
          factionId === "player" ? "warn" : "good",
        );
      }
    }
  }
}

function tickRealmConditionCycle(state, dt) {
  const rc = state.realmConditions;
  const cycleSeconds = rc?.cycleSeconds ?? REALM_CYCLE_DEFAULT_SECONDS;
  state.realmCycleAccumulator = (state.realmCycleAccumulator ?? 0) + dt;
  if (state.realmCycleAccumulator < cycleSeconds) return;
  state.realmCycleAccumulator -= cycleSeconds;
  state.realmCycleCount = (state.realmCycleCount ?? 0) + 1;

  const thresholds = rc?.thresholds ?? {};
  const effects = rc?.effects ?? {};

  for (const factionId of Object.keys(state.factions)) {
    const faction = state.factions[factionId];
    if (faction.kind !== "kingdom") continue;

    // Food strain detection
    const foodNeeded = Math.max(1, faction.population.total);
    const foodStock = faction.resources.food ?? 0;
    const underFoodStrain = foodStock < foodNeeded * (thresholds.foodStrainRatio ?? 1);
    if (underFoodStrain) {
      faction.foodStrainStreak = (faction.foodStrainStreak ?? 0) + 1;
    } else {
      faction.foodStrainStreak = 0;
    }
    if (faction.foodStrainStreak >= (thresholds.foodFamineConsecutiveCycles ?? 2)) {
      applyFamine(state, faction, effects.famine ?? {});
    }

    // Water strain detection
    const waterNeeded = Math.max(1, faction.population.total);
    const waterStock = faction.resources.water ?? 0;
    const underWaterStrain = waterStock < waterNeeded * (thresholds.waterStrainRatio ?? 1);
    if (underWaterStrain) {
      faction.waterStrainStreak = (faction.waterStrainStreak ?? 0) + 1;
    } else {
      faction.waterStrainStreak = 0;
    }
    if (faction.waterStrainStreak >= (thresholds.waterCrisisConsecutiveCycles ?? 1)) {
      applyWaterCrisis(state, faction, effects.waterCrisis ?? {});
    }

    // Housing/cap pressure (canon: surplus can't be housed → loyalty erodes)
    const capRatio = thresholds.populationCapPressureRatio ?? 0.95;
    const underCapPressure = faction.population.total >= faction.population.cap * capRatio && faction.population.cap > 0;
    if (underCapPressure) {
      applyCapPressure(state, faction, effects.capPressure ?? {});
    }

    // Session 46: strong food + water surplus reinforces territorial loyalty.
    // Canonical civilizational principle: a realm that materially overfeeds and
    // overwaters its people should feel more governable than one merely avoiding
    // famine. This hooks civilizational depth back into the live territory layer
    // without inventing a new UI surface, because the loyalty pill and march
    // labels already read from the same state.
    if (!underFoodStrain && !underWaterStrain && !underCapPressure) {
      applyCivilizationalStability(state, faction, effects.stabilitySurplus ?? {});
    }

    // Session 89: governance alliance-threshold coalition loyalty pressure.
    // When the governance leader's population acceptance is at or above the
    // alliance threshold, hostile kingdoms coordinate frontier pressure:
    // loyalty erosion on the weakest governed march and legitimacy strain.
    // This creates a live counterforce that makes the 60->65% acceptance
    // push feel contested, especially with multiple hostile neighbors.
    const activeGovernanceRecognition = getActiveTerritorialGovernanceRecognition(faction);
    if (activeGovernanceRecognition) {
      const governanceAcceptance = activeGovernanceRecognition.populationAcceptancePct ?? 0;
      const governanceAllianceThreshold = activeGovernanceRecognition.populationAcceptanceAllianceThresholdPct
        ?? TERRITORIAL_GOVERNANCE_ACCEPTANCE_ALLIANCE_THRESHOLD_PCT;
      if (governanceAcceptance >= governanceAllianceThreshold) {
        const hostileKingdoms = Object.values(state.factions).filter((candidate) =>
          candidate?.kind === "kingdom" &&
          candidate.id !== factionId &&
          areHostile(state, factionId, candidate.id));
        const hostileCount = hostileKingdoms.length;
        if (hostileCount > 0) {
          const weakestGovernedMarch = state.world.controlPoints
            .filter((cp) => cp.ownerFactionId === factionId && cp.controlState === "stabilized")
            .sort((a, b) => (a.loyalty ?? 0) - (b.loyalty ?? 0))[0];
          if (weakestGovernedMarch) {
            const loyaltyPressure = GOVERNANCE_ALLIANCE_LOYALTY_PRESSURE_BASE * hostileCount;
            weakestGovernedMarch.loyalty = Math.max(0, (weakestGovernedMarch.loyalty ?? 0) + loyaltyPressure);
          }
          const legitimacyPressure = GOVERNANCE_ALLIANCE_LEGITIMACY_PRESSURE_PER_CYCLE * hostileCount;
          if (faction.dynasty) {
            faction.dynasty.legitimacy = Math.max(0, (faction.dynasty.legitimacy ?? 0) - legitimacyPressure);
          }
          faction.governanceAlliancePressureCycles = (faction.governanceAlliancePressureCycles ?? 0) + 1;
          faction.governanceAlliancePressureHostileCount = hostileCount;
          faction.governanceAlliancePressureActive = true;
          faction.governanceAlliancePressureWeakestMarchId = weakestGovernedMarch?.id ?? null;
          faction.governanceAlliancePressureWeakestMarchName = weakestGovernedMarch?.name ?? null;
          faction.governanceAlliancePressureWeakestMarchLoyalty = Math.round(weakestGovernedMarch?.loyalty ?? 0);
        }
      } else {
        faction.governanceAlliancePressureActive = false;
      }
    } else {
      faction.governanceAlliancePressureActive = false;
    }

    // Session 15: Ironmark Blood Production load decays each realm cycle.
    // Canonical canonical war-end recovery: the dynasty can step back from
    // sustained attritional levy and let the load dissipate if training stops.
    if ((faction.bloodProductionLoad ?? 0) > 0) {
      faction.bloodProductionLoad = Math.max(0, faction.bloodProductionLoad - 2.5);
    }

    // Session 19: Dark-extremes world pressure trigger.
    // Canonical: a faction sitting at Apex Cruel for sustained realm cycles
    // attracts a world-reaction. First layer: accumulate a darkExtremesStreak
    // per realm cycle spent at apex_cruel. On 3+ consecutive apex_cruel cycles,
    // mark darkExtremesActive true. This feeds tribe AI reaction below and
    // exposes in the world-pressure snapshot.
    if (faction.conviction?.bandId === "apex_cruel") {
      faction.darkExtremesStreak = (faction.darkExtremesStreak ?? 0) + 1;
    } else {
      faction.darkExtremesStreak = Math.max(0, (faction.darkExtremesStreak ?? 0) - 1);
    }
    const darkExtremesThreshold = 3;
    const wasActive = faction.darkExtremesActive ?? false;
    faction.darkExtremesActive = (faction.darkExtremesStreak ?? 0) >= darkExtremesThreshold;
    if (faction.darkExtremesActive && !wasActive) {
      pushMessage(
        state,
        `The world takes notice of ${getFactionDisplayName(state, factionId)}'s Apex Cruel descent. Tribal pressure intensifies.`,
        factionId === "player" ? "warn" : "info",
      );
    }

    // Session 93: Trueborn City trade-relationship tick. The city tracks each
    // kingdom's standing based on conviction, legitimacy, and hostility record.
    // Moral kingdoms gain standing, cruel kingdoms lose it. Standing feeds into
    // the acceptance profile as Trueborn endorsement.
    if (faction.kind === "kingdom" && state.factions.trueborn_city) {
      const city = state.factions.trueborn_city;
      city.tradeRelationships = city.tradeRelationships ?? {};
      const currentStanding = city.tradeRelationships[factionId] ?? 0;
      const convictionBand = faction.conviction?.bandId ?? "neutral";
      const convictionDelta =
        convictionBand === "apex_moral" ? 1.5 :
        convictionBand === "moral" ? 0.8 :
        convictionBand === "cruel" ? -0.8 :
        convictionBand === "apex_cruel" ? -1.5 :
        0.1;
      const legitimacyBonus = (faction.dynasty?.legitimacy ?? 0) >= 70 ? 0.3 : 0;
      const hostilityPenalty = (faction.hostileTo ?? []).length > 2 ? -0.4 : 0;
      const pactBonus = (faction.diplomacy?.nonAggressionPacts ?? []).some((p) => !p.brokenAt) ? 0.2 : 0;
      const standingDelta = convictionDelta + legitimacyBonus + hostilityPenalty + pactBonus;
      city.tradeRelationships[factionId] = clamp(currentStanding + standingDelta, -20, 20);
    }
  }

  // Keep stage-structure events current before realm-cycle escalation logic reads
  // them as world-pressure sources.
  // Session 94: Trueborn Rise arc tick after trade-relationship updates.
  tickTruebornRiseArc(state);

  updateMatchProgressionState(state, { announce: false, preserveAnnouncements: true });
  updateWorldPressureEscalation(state);
}

// Session 94: Trueborn Rise arc. Canon: if no dynasty has conquered or
// meaningfully challenged the Trueborn City by a defined match threshold,
// the city activates as a major NPC faction pursuing its own reconquest.
// Three stages: claims (passive pressure), recognition demands (active pressure),
// full assertion (military + legitimacy reconquest).
const TRUEBORN_RISE_UNCHALLENGED_THRESHOLD_DAYS = 365 * 8;
const TRUEBORN_RISE_STAGE_2_DELAY_DAYS = 365 * 2;
const TRUEBORN_RISE_STAGE_3_DELAY_DAYS = 365 * 3;
const TRUEBORN_RISE_STAGE_1_LOYALTY_PRESSURE = -0.8;
const TRUEBORN_RISE_STAGE_2_LOYALTY_PRESSURE = -1.8;
const TRUEBORN_RISE_STAGE_3_LOYALTY_PRESSURE = -3.2;
const TRUEBORN_RISE_STAGE_2_LEGITIMACY_PRESSURE = 0.6;
const TRUEBORN_RISE_STAGE_3_LEGITIMACY_PRESSURE = 1.4;
const TRUEBORN_RISE_CHALLENGE_THRESHOLD = 5;

function getTruebornChallengeLevel(state) {
  const city = state.factions.trueborn_city;
  if (!city) return 0;
  let challenge = 0;
  Object.values(state.factions).forEach((faction) => {
    if (faction.kind !== "kingdom") return;
    const standing = city.tradeRelationships?.[faction.id] ?? 0;
    if (standing <= -5) challenge += 2;
    const territories = state.world.controlPoints.filter((cp) => cp.ownerFactionId === faction.id).length;
    if (territories >= 3) challenge += 1;
    if ((faction.hostileTo ?? []).includes("trueborn_city")) challenge += 3;
  });
  return challenge;
}

function tickTruebornRiseArc(state) {
  const city = state.factions.trueborn_city;
  if (!city) return;
  const currentDays = state.dualClock?.inWorldDays ?? 0;
  city.riseArc = city.riseArc ?? {
    stage: 0,
    activatedAt: 0,
    activatedAtInWorldDays: 0,
    lastEscalationAtInWorldDays: 0,
    challengeLevel: 0,
    unchallengedCycles: 0,
  };
  const arc = city.riseArc;
  const challengeLevel = getTruebornChallengeLevel(state);
  arc.challengeLevel = challengeLevel;

  if (arc.stage === 0) {
    if (challengeLevel < TRUEBORN_RISE_CHALLENGE_THRESHOLD) {
      arc.unchallengedCycles += 1;
    } else {
      arc.unchallengedCycles = Math.max(0, arc.unchallengedCycles - 2);
    }
    if (currentDays >= TRUEBORN_RISE_UNCHALLENGED_THRESHOLD_DAYS && arc.unchallengedCycles >= 3) {
      arc.stage = 1;
      arc.activatedAt = state.meta.elapsed;
      arc.activatedAtInWorldDays = currentDays;
      arc.lastEscalationAtInWorldDays = currentDays;
      pushMessage(
        state,
        "The Trueborn City raises its banner. The ancient authority asserts formal governance claims over territories within its sphere. Legitimacy pressure begins.",
        "warn",
      );
    }
    return;
  }

  // Session 95: recognized kingdoms receive reduced Rise pressure.
  const isRecognized = (fId) => state.factions[fId]?.diplomacy?.truebornRecognition ?? false;
  const recognizedPressureMultiplier = 0.25;

  if (arc.stage === 1) {
    state.world.controlPoints.forEach((cp) => {
      if (cp.ownerFactionId && state.factions[cp.ownerFactionId]?.kind === "kingdom") {
        const mult = isRecognized(cp.ownerFactionId) ? recognizedPressureMultiplier : 1;
        cp.loyalty = Math.max(0, (cp.loyalty ?? 0) + TRUEBORN_RISE_STAGE_1_LOYALTY_PRESSURE * mult);
      }
    });
    if (currentDays >= arc.lastEscalationAtInWorldDays + TRUEBORN_RISE_STAGE_2_DELAY_DAYS) {
      arc.stage = 2;
      arc.lastEscalationAtInWorldDays = currentDays;
      pushMessage(
        state,
        "The Trueborn City escalates. Recognition missions travel to every court. Refusing the Trueborn claim accelerates loyalty erosion in peripheral territories.",
        "warn",
      );
    }
    return;
  }

  if (arc.stage === 2) {
    state.world.controlPoints.forEach((cp) => {
      if (cp.ownerFactionId && state.factions[cp.ownerFactionId]?.kind === "kingdom") {
        const mult = isRecognized(cp.ownerFactionId) ? recognizedPressureMultiplier : 1;
        cp.loyalty = Math.max(0, (cp.loyalty ?? 0) + TRUEBORN_RISE_STAGE_2_LOYALTY_PRESSURE * mult);
        const faction = state.factions[cp.ownerFactionId];
        if (faction?.dynasty) {
          faction.dynasty.legitimacy = Math.max(0, (faction.dynasty.legitimacy ?? 0) - TRUEBORN_RISE_STAGE_2_LEGITIMACY_PRESSURE * mult);
        }
      }
    });
    if (currentDays >= arc.lastEscalationAtInWorldDays + TRUEBORN_RISE_STAGE_3_DELAY_DAYS) {
      arc.stage = 3;
      arc.lastEscalationAtInWorldDays = currentDays;
      pushMessage(
        state,
        "The Trueborn City enters full assertion. Military forces mobilize. The Restoration begins. Every kingdom faces accelerating legitimacy doubt and territorial pressure.",
        "warn",
      );
    }
    return;
  }

  if (arc.stage === 3) {
    state.world.controlPoints.forEach((cp) => {
      if (cp.ownerFactionId && state.factions[cp.ownerFactionId]?.kind === "kingdom") {
        const mult = isRecognized(cp.ownerFactionId) ? recognizedPressureMultiplier : 1;
        cp.loyalty = Math.max(0, (cp.loyalty ?? 0) + TRUEBORN_RISE_STAGE_3_LOYALTY_PRESSURE * mult);
        const faction = state.factions[cp.ownerFactionId];
        if (faction?.dynasty) {
          faction.dynasty.legitimacy = Math.max(0, (faction.dynasty.legitimacy ?? 0) - TRUEBORN_RISE_STAGE_3_LEGITIMACY_PRESSURE * mult);
        }
      }
    });
  }
}

// Session 95: Trueborn recognition diplomacy. A kingdom can formally recognize
// the Trueborn City's historical claim, gaining standing and exemption from
// Rise pressure at the cost of legitimacy and conviction.
const TRUEBORN_RECOGNITION_INFLUENCE_COST = 40;
const TRUEBORN_RECOGNITION_GOLD_COST = 60;
const TRUEBORN_RECOGNITION_STANDING_BONUS = 6;
const TRUEBORN_RECOGNITION_LEGITIMACY_COST = 5;

export function getTruebornRecognitionTerms(state, factionId) {
  const faction = getFaction(state, factionId);
  const city = state.factions.trueborn_city;
  if (!faction || !city || faction.kind !== "kingdom") {
    return { available: false, reason: "Must be a kingdom with a live Trueborn City." };
  }
  const arc = city.riseArc ?? {};
  if ((arc.stage ?? 0) < 1) {
    return { available: false, reason: "The Trueborn City has not yet raised its banner." };
  }
  const alreadyRecognized = (faction.diplomacy?.truebornRecognition ?? false);
  if (alreadyRecognized) {
    return { available: false, reason: "Already recognized the Trueborn claim." };
  }
  const cost = { influence: TRUEBORN_RECOGNITION_INFLUENCE_COST, gold: TRUEBORN_RECOGNITION_GOLD_COST };
  if (!canAffordCost(faction.resources, cost)) {
    return { available: false, reason: `Requires ${cost.influence} influence and ${cost.gold} gold.`, cost };
  }
  return {
    available: true,
    cost,
    standingBonus: TRUEBORN_RECOGNITION_STANDING_BONUS,
    legitimacyCost: TRUEBORN_RECOGNITION_LEGITIMACY_COST,
    riseStage: arc.stage,
  };
}

export function recognizeTruebornClaim(state, factionId) {
  const terms = getTruebornRecognitionTerms(state, factionId);
  if (!terms.available) {
    return { ok: false, reason: terms.reason };
  }
  const faction = getFaction(state, factionId);
  const city = state.factions.trueborn_city;
  applyCost(faction.resources, terms.cost);
  if (faction.dynasty) {
    faction.dynasty.legitimacy = Math.max(0, (faction.dynasty.legitimacy ?? 0) - TRUEBORN_RECOGNITION_LEGITIMACY_COST);
  }
  faction.diplomacy = faction.diplomacy ?? {};
  faction.diplomacy.truebornRecognition = true;
  faction.diplomacy.truebornRecognitionAtInWorldDays = state.dualClock?.inWorldDays ?? 0;
  city.tradeRelationships = city.tradeRelationships ?? {};
  city.tradeRelationships[factionId] = clamp(
    (city.tradeRelationships[factionId] ?? 0) + TRUEBORN_RECOGNITION_STANDING_BONUS,
    -20,
    20,
  );
  pushMessage(
    state,
    `${getFactionDisplayName(state, factionId)} formally recognizes the Trueborn City's historical claim. Standing improves. Rise pressure eases on recognized territories.`,
    factionId === "player" ? "good" : "info",
  );
  return { ok: true };
}

function applyFamine(state, faction, effects) {
  const decline = effects.populationDeclinePerCycle ?? 1;
  const loyaltyDelta = effects.loyaltyDeltaPerCycle ?? -4;
  if (faction.population.total > 0) {
    faction.population.total = Math.max(0, faction.population.total - decline);
  }
  applyControlPointLoyaltyDelta(state, faction.id, loyaltyDelta);
  pushMessage(
    state,
    `${getFactionDisplayName(state, faction.id)} suffers famine — population and loyalty decline.`,
    faction.id === "player" ? "warn" : "info",
  );
  recordConvictionEvent(state, faction.id, "desecration", 1, "Famine unaddressed");
}

function applyWaterCrisis(state, faction, effects) {
  const outmigration = effects.outmigrationPerCycle ?? 1;
  const loyaltyDelta = effects.loyaltyDeltaPerCycle ?? -6;
  if (faction.population.total > 0) {
    faction.population.total = Math.max(0, faction.population.total - outmigration);
  }
  applyControlPointLoyaltyDelta(state, faction.id, loyaltyDelta);
  pushMessage(
    state,
    `${getFactionDisplayName(state, faction.id)} suffers a water crisis — people outmigrating.`,
    faction.id === "player" ? "warn" : "info",
  );
}

function applyCivilizationalStability(state, faction, effects) {
  const ownedTerritory = state.world.controlPoints.filter((cp) => cp.ownerFactionId === faction.id);
  if (ownedTerritory.length === 0) {
    return false;
  }
  const population = Math.max(1, faction.population.total);
  const foodRatio = (faction.resources.food ?? 0) / population;
  const waterRatio = (faction.resources.water ?? 0) / population;
  const requiredFoodRatio = effects.foodRatio ?? 1.75;
  const requiredWaterRatio = effects.waterRatio ?? 1.75;
  if (foodRatio < requiredFoodRatio || waterRatio < requiredWaterRatio) {
    return false;
  }
  applyControlPointLoyaltyDelta(state, faction.id, effects.loyaltyDeltaPerCycle ?? 1, {
    maxLoyalty: effects.maxLoyaltyToApply ?? 95,
  });
  return true;
}

function applyCapPressure(state, faction, effects) {
  applyControlPointLoyaltyDelta(state, faction.id, effects.loyaltyDeltaPerCycle ?? -1);
}

function getWorldPressureLevelLabel(level) {
  switch (level) {
    case 3:
      return "Convergence";
    case 2:
      return "Severe";
    case 1:
      return "Watchful";
    default:
      return "Quiet";
  }
}

export function getWorldPressureSourceBreakdown(state, factionId) {
  const faction = getFaction(state, factionId);
  const activeGreatReckoning = Boolean(state.matchProgression?.greatReckoningActive);
  const greatReckoningTargetFactionId = state.matchProgression?.greatReckoningTargetFactionId ?? null;
  if (!faction || faction.kind !== "kingdom") {
    return {
      total: 0,
      topSourceId: "quiet",
      topSourceLabel: "quiet",
      sources: {
        greatReckoning: 0,
        divineRightDeclaration: 0,
        territorialGovernanceRecognition: 0,
        territoryExpansion: 0,
        offHomeHoldings: 0,
        holyWar: 0,
        captives: 0,
        hostileOperations: 0,
        darkExtremes: 0,
      },
    };
  }
  const ownedTerritories = state.world.controlPoints.filter((controlPoint) => controlPoint.ownerFactionId === factionId);
  const sourceValues = {
    greatReckoning: activeGreatReckoning && greatReckoningTargetFactionId === factionId
      ? GREAT_RECKONING_PRESSURE_SCORE
      : 0,
    divineRightDeclaration: getActiveDivineRightDeclaration(faction, state.meta.elapsed)
      ? DIVINE_RIGHT_WORLD_PRESSURE_SCORE
      : 0,
    territorialGovernanceRecognition: getTerritorialGovernanceWorldPressureContribution(faction),
    territoryExpansion: Math.max(0, ownedTerritories.length - 2),
    offHomeHoldings: ownedTerritories.filter((controlPoint) => controlPoint.continentId && controlPoint.continentId !== "home").length * 2,
    holyWar: (faction.faith?.activeHolyWars ?? []).filter((entry) => (entry.expiresAt ?? 0) > state.meta.elapsed).length * 2,
    captives: faction.dynasty?.captives?.length ?? 0,
    hostileOperations: Math.min(2, (faction.dynasty?.operations?.active ?? []).filter((operation) =>
      operation.type === "espionage" ||
      operation.type === "assassination" ||
      operation.type === "holy_war").length),
    darkExtremes: faction.darkExtremesActive ? 3 : 0,
  };
  const sourcePriority = [
    ["greatReckoning", "great reckoning"],
    ["divineRightDeclaration", "divine right declaration"],
    ["territorialGovernanceRecognition", "territorial governance recognition"],
    ["offHomeHoldings", "off-home holdings"],
    ["darkExtremes", "dark extremes"],
    ["holyWar", "holy war"],
    ["territoryExpansion", "territory expansion"],
    ["hostileOperations", "hostile operations"],
    ["captives", "captives"],
  ];
  const topSource = sourcePriority
    .map(([id, label]) => ({ id, label, value: sourceValues[id] ?? 0 }))
    .sort((left, right) => (right.value - left.value))[0];
  return {
    total: Object.values(sourceValues).reduce((sum, value) => sum + value, 0),
    topSourceId: (topSource?.value ?? 0) > 0 ? topSource.id : "quiet",
    topSourceLabel: (topSource?.value ?? 0) > 0 ? topSource.label : "quiet",
    sources: sourceValues,
  };
}

function calculateWorldPressureScore(state, factionId) {
  return getWorldPressureSourceBreakdown(state, factionId).total;
}

export function getWorldPressureLeaderProfile(state) {
  const kingdoms = Object.values(state.factions)
    .filter((faction) => faction?.kind === "kingdom")
    .map((faction) => ({
      factionId: faction.id,
      score: faction.worldPressureScore ?? 0,
      level: faction.worldPressureLevel ?? 0,
      streak: faction.worldPressureStreak ?? 0,
    }))
    .sort((left, right) =>
      (right.level - left.level) ||
      (right.score - left.score) ||
      (right.streak - left.streak));
  const leader = kingdoms[0] ?? null;
  return {
    factionId: leader && (leader.level ?? 0) > 0 ? leader.factionId : null,
    score: leader?.score ?? 0,
    level: leader?.level ?? 0,
    streak: leader?.streak ?? 0,
  };
}

export function getWorldPressureTargetProfile(state, factionId) {
  const faction = getFaction(state, factionId);
  const leader = getWorldPressureLeaderProfile(state);
  const level = faction?.worldPressureLevel ?? 0;
  const sourceBreakdown = getWorldPressureSourceBreakdown(state, factionId);
  return {
    score: faction?.worldPressureScore ?? 0,
    streak: faction?.worldPressureStreak ?? 0,
    level,
    label: getWorldPressureLevelLabel(level),
    topSourceId: sourceBreakdown.topSourceId,
    topSourceLabel: sourceBreakdown.topSourceLabel,
    sources: sourceBreakdown.sources,
    targeted: Boolean(leader.factionId) && leader.factionId === factionId && level > 0,
    leaderFactionId: leader.factionId,
    leaderScore: leader.score,
    leaderLevel: leader.level,
    leaderStreak: leader.streak,
  };
}

export function getWorldPressureConvergenceProfile(state, factionId) {
  const pressureProfile = getWorldPressureTargetProfile(state, factionId);
  const score = pressureProfile.score ?? 0;
  const active = pressureProfile.targeted && (pressureProfile.level ?? 0) >= 3;
  if (!active) {
    return {
      active: false,
      level: pressureProfile.level ?? 0,
      status: (pressureProfile.label ?? "Quiet").toLowerCase(),
      score,
      attackTimerCap: null,
      territoryTimerCap: null,
      sabotageTimerCap: null,
      espionageTimerCap: null,
      assassinationTimerCap: null,
      missionaryTimerCap: null,
      holyWarTimerCap: null,
      tribalRaidTimerMultiplier: 1,
    };
  }

  const scoreBonus = Math.max(0, Math.min(2, score - 6));
  return {
    active: true,
    level: pressureProfile.level ?? 0,
    status: "convergence",
    score,
    attackTimerCap: Math.max(4, 5 - scoreBonus),
    territoryTimerCap: Math.max(3, 4 - scoreBonus),
    sabotageTimerCap: Math.max(3, 4 - scoreBonus),
    espionageTimerCap: Math.max(4, 6 - scoreBonus),
    assassinationTimerCap: Math.max(5, 8 - scoreBonus),
    missionaryTimerCap: Math.max(5, 9 - scoreBonus),
    holyWarTimerCap: Math.max(6, 10 - scoreBonus),
    tribalRaidTimerMultiplier: Math.max(0.2, 0.3 - (scoreBonus * 0.02)),
  };
}

function getWorldPressureCadetInstabilityProfile(state, factionOrId) {
  const faction = typeof factionOrId === "string" ? getFaction(state, factionOrId) : factionOrId;
  const activeLesserHouses = (faction?.dynasty?.lesserHouses ?? []).filter((lesserHouse) => lesserHouse.status === "active");
  const pressureProfile = faction ? getWorldPressureTargetProfile(state, faction.id) : null;
  if (!faction || activeLesserHouses.length === 0 || !pressureProfile?.targeted || (pressureProfile.level ?? 0) <= 0) {
    return {
      level: 0,
      status: "quiet",
      delta: 0,
      score: pressureProfile?.score ?? 0,
      stressedCount: activeLesserHouses.length,
    };
  }
  const scoreBonus = Math.max(0, Math.min(0.12, ((pressureProfile.score ?? 0) - 4) * 0.03));
  const delta = -(((pressureProfile.level ?? 0) * 0.12) + scoreBonus);
  return {
    level: pressureProfile.level,
    status: (pressureProfile.label ?? "Quiet").toLowerCase(),
    delta,
    score: pressureProfile.score ?? 0,
    stressedCount: activeLesserHouses.length,
  };
}

function getCompletedBuildingCount(state, factionId) {
  return state.buildings.filter((building) =>
    building.factionId === factionId &&
    building.completed &&
    building.health > 0).length;
}

function getKingdomTerritoryCount(state, factionId) {
  return state.world.controlPoints.filter((controlPoint) => controlPoint.ownerFactionId === factionId).length;
}

function getRivalContactProfile(state, leftFactionId, rightFactionId) {
  const leftFaction = getFaction(state, leftFactionId);
  const rightFaction = getFaction(state, rightFactionId);
  if (!leftFaction || !rightFaction) {
    return {
      active: false,
      signals: 0,
      directFrontContact: false,
      contestedBorder: false,
      hostileOperations: false,
      captivePressure: false,
      holyWar: false,
    };
  }

  const leftCombat = getAliveCombatUnits(state, leftFactionId);
  const rightCombat = getAliveCombatUnits(state, rightFactionId);
  const directFrontContact = leftCombat.some((leftUnit) =>
    rightCombat.some((rightUnit) => distance(leftUnit.x, leftUnit.y, rightUnit.x, rightUnit.y) <= 220));
  const contestedBorder = state.world.controlPoints.some((controlPoint) => (
    (controlPoint.ownerFactionId === leftFactionId && controlPoint.captureFactionId === rightFactionId) ||
    (controlPoint.ownerFactionId === rightFactionId && controlPoint.captureFactionId === leftFactionId)
  ));
  const hostileOperations = Object.values(state.factions).some((faction) =>
    (faction.dynasty?.operations?.active ?? []).some((operation) =>
      (operation.factionId === leftFactionId && operation.targetFactionId === rightFactionId) ||
      (operation.factionId === rightFactionId && operation.targetFactionId === leftFactionId)));
  const captivePressure = [...(leftFaction.dynasty?.members ?? []), ...(rightFaction.dynasty?.members ?? [])]
    .some((member) =>
      member.capturedByFactionId === leftFactionId || member.capturedByFactionId === rightFactionId);
  const holyWar = getIncomingHolyWars(state, leftFactionId, state.meta.elapsed).some((entry) => entry.sourceFactionId === rightFactionId) ||
    getIncomingHolyWars(state, rightFactionId, state.meta.elapsed).some((entry) => entry.sourceFactionId === leftFactionId);
  const signals = Number(directFrontContact) + Number(contestedBorder) + Number(hostileOperations) + Number(captivePressure) + Number(holyWar);
  return {
    active: signals > 0,
    signals,
    directFrontContact,
    contestedBorder,
    hostileOperations,
    captivePressure,
    holyWar,
  };
}

function getMatchStageDefinition(stageNumber) {
  return MATCH_STAGE_DEFINITIONS.find((definition) => definition.number === stageNumber)
    ?? MATCH_STAGE_DEFINITIONS[0];
}

function ensureMatchProgressionState(state) {
  if (!state.matchProgression) {
    state.matchProgression = {
      stageNumber: 1,
      stageId: "founding",
      stageLabel: "Founding",
      phaseId: "emergence",
      phaseLabel: "Emergence",
      stageReadiness: 0,
      nextStageId: "expansion_identity",
      nextStageLabel: "Expansion and Identity",
      nextStageShortfalls: [],
      declarationCount: 0,
      inWorldDays: 0,
      inWorldYears: 0,
      rivalContactActive: false,
      sustainedWarActive: false,
      lastAnnouncedStageId: "founding",
      dominantKingdomId: null,
      dominantTerritoryShare: 0,
      greatReckoningActive: false,
      greatReckoningTargetFactionId: null,
      greatReckoningShare: 0,
      greatReckoningThreshold: GREAT_RECKONING_TRIGGER_SHARE,
      greatReckoningTriggeredAtInWorldDays: 0,
      lastAnnouncedGreatReckoningTargetId: null,
      lastReleasedGreatReckoningTargetId: null,
    };
  }
  return state.matchProgression;
}

function computeMatchProgressionState(state) {
  const player = getFaction(state, "player");
  const playerTerritories = getKingdomTerritoryCount(state, "player");
  const playerCompletedBuildings = getCompletedBuildingCount(state, "player");
  const playerMilitaryCount = getAliveCombatUnits(state, "player").length;
  const playerFoodStable = (player?.resources?.food ?? 0) >= (player?.population?.total ?? 0) + 6;
  const playerWaterStable = (player?.resources?.water ?? 0) >= (player?.population?.total ?? 0) + 6;
  const defendedHome = state.buildings.some((building) =>
    building.factionId === "player" &&
    building.completed &&
    building.health > 0 &&
    ["command_hall", "barracks", "watch_tower", "keep_tier_1", "gatehouse"].includes(building.typeId));
  const faithCommitted = Boolean(player?.faith?.selectedFaithId);
  const playerWorldPressure = getWorldPressureTargetProfile(state, "player");
  const enemyWorldPressure = getWorldPressureTargetProfile(state, "enemy");
  const playerConvergence = getWorldPressureConvergenceProfile(state, "player");
  const enemyConvergence = getWorldPressureConvergenceProfile(state, "enemy");
  const rivalContact = getRivalContactProfile(state, "player", "enemy");
  const activeSiegeCount = getFactionSiegeState(state, "player").engineCount + getFactionSiegeState(state, "enemy").engineCount;
  const activeHolyWars = getIncomingHolyWars(state, "player", state.meta.elapsed).length +
    getIncomingHolyWars(state, "enemy", state.meta.elapsed).length;
  const activeDivineRightDeclarations = Object.values(state.factions)
    .filter((faction) => faction?.kind === "kingdom")
    .filter((faction) => Boolean(getActiveDivineRightDeclaration(faction, state.meta.elapsed))).length;
  const activeDynastyOperations = Object.values(state.factions)
    .flatMap((faction) => faction.dynasty?.operations?.active ?? [])
    .filter((operation) =>
      operation.targetFactionId === "player" || operation.targetFactionId === "enemy").length;
  const kingdomTerritoryCounts = Object.values(state.factions)
    .filter((faction) => faction?.kind === "kingdom")
    .map((faction) => getKingdomTerritoryCount(state, faction.id));
  const dominantKingdomTerritoryCount = kingdomTerritoryCounts.length > 0 ? Math.max(...kingdomTerritoryCounts) : 0;
  const totalKingdomTerritories = kingdomTerritoryCounts.reduce((sum, territoryCount) => sum + territoryCount, 0);
  const dominantTerritoryShare = totalKingdomTerritories > 0 ? dominantKingdomTerritoryCount / totalKingdomTerritories : 0;
  const highestFaithLevel = Math.max(...Object.values(state.factions)
    .filter((faction) => faction?.kind === "kingdom")
    .map((faction) => faction.faith?.level ?? 0), 0);
  const inWorldDays = state.dualClock?.inWorldDays ?? 0;
  const inWorldYears = inWorldDays / 365;

  const stageTwoRequirements = [
    { label: "stabilize food with surplus", complete: playerFoodStable },
    { label: "stabilize water with surplus", complete: playerWaterStable },
    { label: "raise a defended home seat", complete: defendedHome },
    { label: "complete the founding buildout", complete: playerCompletedBuildings >= 4 },
  ];
  const stageThreeRequirements = [
    { label: "bind the dynasty to a faith covenant", complete: faithCommitted },
    { label: "expand beyond the founding march", complete: playerTerritories >= 2 },
    { label: "raise a field army fit for rival contact", complete: playerMilitaryCount >= 6 },
  ];
  const stageFourRequirements = [
    { label: "make direct rival contact", complete: rivalContact.active },
    { label: "contest a live border", complete: rivalContact.contestedBorder || playerWorldPressure.level > 0 || enemyWorldPressure.level > 0 },
    { label: "open sustained war pressure", complete: activeSiegeCount > 0 || activeHolyWars > 0 || activeDivineRightDeclarations > 0 || activeDynastyOperations > 0 },
  ];
  const stageFiveRequirements = [
    { label: "drive the world toward final convergence", complete: playerConvergence.active || enemyConvergence.active || activeDivineRightDeclarations > 0 },
    { label: "create a true sovereignty contender", complete: dominantTerritoryShare >= 0.75 || highestFaithLevel >= 5 },
    { label: "carry the war into late dynastic time", complete: inWorldYears >= 12 },
  ];

  const scoreRequirements = (requirements) =>
    requirements.reduce((sum, requirement) => sum + Number(requirement.complete), 0) / Math.max(1, requirements.length);

  const stageTwoReady = stageTwoRequirements.every((requirement) => requirement.complete);
  const stageThreeReady = stageTwoReady && stageThreeRequirements.every((requirement) => requirement.complete);
  const stageFourReady = stageThreeReady && stageFourRequirements.every((requirement) => requirement.complete);
  const stageFiveReady = stageFourReady && stageFiveRequirements.every((requirement) => requirement.complete);

  const stageNumber = stageFiveReady
    ? 5
    : stageFourReady
      ? 4
      : stageThreeReady
        ? 3
        : stageTwoReady
          ? 2
          : 1;
  const stageDefinition = getMatchStageDefinition(stageNumber);
  const phaseId = stageNumber >= 5
    ? "resolution"
    : stageNumber >= 3 || (stageNumber === 2 && scoreRequirements(stageThreeRequirements) >= 0.67)
      ? "commitment"
      : "emergence";
  const nextStageNumber = Math.min(5, stageNumber + 1);
  const nextStageDefinition = getMatchStageDefinition(nextStageNumber);
  const nextStageShortfalls = stageNumber === 1
    ? stageTwoRequirements.filter((requirement) => !requirement.complete).map((requirement) => requirement.label)
    : stageNumber === 2
      ? stageThreeRequirements.filter((requirement) => !requirement.complete).map((requirement) => requirement.label)
      : stageNumber === 3
        ? stageFourRequirements.filter((requirement) => !requirement.complete).map((requirement) => requirement.label)
        : stageNumber === 4
          ? stageFiveRequirements.filter((requirement) => !requirement.complete).map((requirement) => requirement.label)
          : [];
  const stageReadiness = stageNumber === 1
    ? scoreRequirements(stageTwoRequirements)
    : stageNumber === 2
      ? scoreRequirements(stageThreeRequirements)
      : stageNumber === 3
        ? scoreRequirements(stageFourRequirements)
        : stageNumber === 4
          ? scoreRequirements(stageFiveRequirements)
          : 1;

  return {
    stageNumber,
    stageId: stageDefinition.id,
    stageLabel: stageDefinition.label,
    phaseId,
    phaseLabel: MATCH_PHASE_LABELS[phaseId] ?? "Emergence",
    stageReadiness,
    nextStageId: stageNumber >= 5 ? null : nextStageDefinition.id,
    nextStageLabel: stageNumber >= 5 ? null : nextStageDefinition.label,
    nextStageShortfalls,
    declarationCount: state.dualClock?.declarations?.length ?? 0,
    inWorldDays,
    inWorldYears,
    rivalContactActive: rivalContact.active,
    rivalContactSignals: rivalContact.signals,
    sustainedWarActive: stageFourRequirements.some((requirement) => requirement.complete),
    dominantKingdomId: kingdomTerritoryCounts.length > 0
      ? Object.values(state.factions)
        .filter((faction) => faction?.kind === "kingdom")
        .sort((left, right) => getKingdomTerritoryCount(state, right.id) - getKingdomTerritoryCount(state, left.id))[0]?.id ?? null
      : null,
    dominantTerritoryShare,
  };
}

function updateMatchProgressionState(state, options = {}) {
  const { announce = true, preserveAnnouncements = false } = options;
  const progression = ensureMatchProgressionState(state);
  const previousStageId = progression.stageId;
  const previousGreatReckoningActive = progression.greatReckoningActive ?? false;
  const previousGreatReckoningTargetId = progression.greatReckoningTargetFactionId ?? null;
  const computed = computeMatchProgressionState(state);
  Object.assign(progression, computed);

  const currentGreatReckoningTargetId = progression.stageNumber >= 4
    ? progression.dominantKingdomId ?? null
    : null;
  const currentGreatReckoningShare = currentGreatReckoningTargetId
    ? progression.dominantTerritoryShare ?? 0
    : 0;
  const sustainGreatReckoning = previousGreatReckoningActive &&
    previousGreatReckoningTargetId === currentGreatReckoningTargetId &&
    currentGreatReckoningShare >= GREAT_RECKONING_RELEASE_SHARE;
  const triggerGreatReckoning = Boolean(currentGreatReckoningTargetId) &&
    currentGreatReckoningShare >= GREAT_RECKONING_TRIGGER_SHARE;

  progression.greatReckoningActive = triggerGreatReckoning || sustainGreatReckoning;
  progression.greatReckoningTargetFactionId = progression.greatReckoningActive
    ? currentGreatReckoningTargetId
    : null;
  progression.greatReckoningShare = progression.greatReckoningActive
    ? currentGreatReckoningShare
    : 0;
  progression.greatReckoningThreshold = GREAT_RECKONING_TRIGGER_SHARE;
  progression.greatReckoningTriggeredAtInWorldDays = progression.greatReckoningActive
    ? previousGreatReckoningActive && previousGreatReckoningTargetId === currentGreatReckoningTargetId
      ? progression.greatReckoningTriggeredAtInWorldDays ?? progression.inWorldDays
      : progression.inWorldDays
    : 0;

  if (preserveAnnouncements) {
    progression.lastAnnouncedStageId = progression.lastAnnouncedStageId ?? progression.stageId;
    return progression;
  }

  if (!announce) {
    progression.lastAnnouncedStageId = progression.stageId;
    return progression;
  }

  if (previousStageId && previousStageId !== progression.stageId && progression.lastAnnouncedStageId !== progression.stageId) {
    pushMessage(
      state,
      `Match enters Stage ${progression.stageNumber}: ${progression.stageLabel}.`,
      progression.stageNumber >= 4 ? "warn" : "info",
    );
    progression.lastAnnouncedStageId = progression.stageId;
  }

  if (progression.greatReckoningActive &&
    progression.greatReckoningTargetFactionId &&
    progression.lastAnnouncedGreatReckoningTargetId !== progression.greatReckoningTargetFactionId) {
    const targetName = getFactionDisplayName(state, progression.greatReckoningTargetFactionId);
    pushMessage(
      state,
      `The Great Reckoning turns on ${targetName} as it crosses ${Math.round(GREAT_RECKONING_TRIGGER_SHARE * 100)}% of the world. Rival powers and the Trueborn City are authorized to answer its claim.`,
      progression.greatReckoningTargetFactionId === "player" ? "warn" : "info",
    );
    progression.lastAnnouncedGreatReckoningTargetId = progression.greatReckoningTargetFactionId;
    progression.lastReleasedGreatReckoningTargetId = null;
  } else if (!progression.greatReckoningActive &&
    previousGreatReckoningActive &&
    previousGreatReckoningTargetId &&
    progression.lastReleasedGreatReckoningTargetId !== previousGreatReckoningTargetId) {
    const targetName = getFactionDisplayName(state, previousGreatReckoningTargetId);
    pushMessage(
      state,
      `Great Reckoning pressure slackens around ${targetName} as its territorial share falls back below coalition threshold.`,
      previousGreatReckoningTargetId === "player" ? "good" : "info",
    );
    progression.lastReleasedGreatReckoningTargetId = previousGreatReckoningTargetId;
    progression.lastAnnouncedGreatReckoningTargetId = null;
  }

  return progression;
}

function getGreatReckoningProfile(state) {
  const progression = ensureMatchProgressionState(state);
  return {
    active: Boolean(progression.greatReckoningActive),
    targetFactionId: progression.greatReckoningTargetFactionId ?? null,
    territoryShare: progression.greatReckoningShare ?? 0,
    threshold: progression.greatReckoningThreshold ?? GREAT_RECKONING_TRIGGER_SHARE,
    triggeredAtInWorldDays: progression.greatReckoningTriggeredAtInWorldDays ?? 0,
  };
}

export function getMatchProgressionSnapshot(state) {
  const progression = state.matchProgression
    ? updateMatchProgressionState(state, { announce: false, preserveAnnouncements: true })
    : updateMatchProgressionState(state, { announce: false });
  return {
    ...progression,
    nextStageShortfalls: [...(progression.nextStageShortfalls ?? [])],
  };
}

export function getMinorHousePressureOpportunityProfile(state, factionOrId) {
  const minor = typeof factionOrId === "string" ? getFaction(state, factionOrId) : factionOrId;
  const parentFaction = minor?.originFactionId ? getFaction(state, minor.originFactionId) : null;
  const pressureProfile = parentFaction ? getWorldPressureTargetProfile(state, parentFaction.id) : null;
  if (!minor || minor.kind !== "minor_house" || !parentFaction || !pressureProfile?.targeted || (pressureProfile.level ?? 0) <= 0) {
    return {
      pressured: false,
      parentFactionId: parentFaction?.id ?? minor?.originFactionId ?? null,
      parentFactionName: parentFaction?.displayName ?? null,
      level: 0,
      status: "quiet",
      score: pressureProfile?.score ?? 0,
      levyTempoMultiplier: 1,
      retakeTempoMultiplier: 1,
      retinueCapBonus: 0,
      threatRadiusBonus: 0,
    };
  }

  const level = pressureProfile.level ?? 0;
  const scoreBonus = Math.max(0, Math.min(0.18, ((pressureProfile.score ?? 0) - 4) * 0.03));
  return {
    pressured: true,
    parentFactionId: parentFaction.id,
    parentFactionName: parentFaction.displayName,
    level,
    status: (pressureProfile.label ?? "Quiet").toLowerCase(),
    score: pressureProfile.score ?? 0,
    levyTempoMultiplier: 1 + (level * 0.22) + scoreBonus,
    retakeTempoMultiplier: 1 + (level * 0.28) + scoreBonus,
    retinueCapBonus: level >= 3 ? 2 : level >= 2 ? 1 : 0,
    threatRadiusBonus: level * 30,
  };
}

function applyWorldPressureConsequences(state, faction) {
  const level = faction.worldPressureLevel ?? 0;
  if (level <= 0) {
    return;
  }
  const pressurePoint = getLowestLoyaltyControlPoint(state, faction.id);
  if (pressurePoint) {
    pressurePoint.loyalty = clamp(pressurePoint.loyalty - level, 0, 100);
  }
  if (level >= 2) {
    adjustLegitimacy(faction, -(level - 1));
  }
}

function updateWorldPressureEscalation(state) {
  const kingdoms = Object.values(state.factions).filter((faction) => faction?.kind === "kingdom");
  const scored = kingdoms
    .map((faction) => ({
      faction,
      score: calculateWorldPressureScore(state, faction.id),
    }))
    .sort((left, right) => right.score - left.score);
  const leader = scored[0] ?? null;
  const secondScore = scored[1]?.score ?? 0;
  const dominantLeaderId = leader && leader.score >= 4 && leader.score > secondScore
    ? leader.faction.id
    : null;

  kingdoms.forEach((faction) => {
    faction.worldPressureScore = scored.find((entry) => entry.faction.id === faction.id)?.score ?? 0;
    const previousLevel = faction.worldPressureLevel ?? 0;
    if (dominantLeaderId === faction.id) {
      faction.worldPressureStreak = (faction.worldPressureStreak ?? 0) + 1;
    } else {
      faction.worldPressureStreak = Math.max(0, (faction.worldPressureStreak ?? 0) - 1);
    }

    const streak = faction.worldPressureStreak ?? 0;
    faction.worldPressureLevel = streak >= 6
      ? 3
      : streak >= 3
        ? 2
        : streak >= 1
          ? 1
          : 0;

    if (faction.worldPressureLevel > 0) {
      applyWorldPressureConsequences(state, faction);
    }

    if (faction.worldPressureLevel > previousLevel) {
      const label = getWorldPressureLevelLabel(faction.worldPressureLevel);
      pushMessage(
        state,
        `${getFactionDisplayName(state, faction.id)} draws ${label.toLowerCase()} world pressure as its realm overextends.`,
        faction.id === "player" ? "warn" : "info",
      );
    } else if (previousLevel > 0 && faction.worldPressureLevel === 0) {
      pushMessage(
        state,
        `World pressure eases around ${getFactionDisplayName(state, faction.id)} as its dominance slackens.`,
        faction.id === "player" ? "good" : "info",
      );
    }
  });
}

function legibilityBand(value, greenFloor, yellowFloor) {
  if (value >= greenFloor) return "green";
  if (value >= yellowFloor) return "yellow";
  return "red";
}

// Session 26: Dual-clock declaration seam (first canonical layer).
// Canonical master doctrine section XIII ("Determined Time System") requires that
// every major simulation event carry a declared in-world time delta so the Dynastic
// Clock can advance alongside the Battlefield Clock.
//
// First layer: each stepSimulation tick advances state.dualClock.inWorldDays by
// a canonical ratio (1 real second = 2 in-world days by default, matching the
// "3 to 6 months per skirmish" canon band from doctrine section XIV). Events
// (battles ending, control points flipping, sieges breaking) declare additional
// in-world time jumps on top of the baseline tick.
//
// Later sessions can layer: explicit Declaration Phase between battles, Events
// Queue during commitment windows, and full Phase Entry at stage boundaries.

const DUAL_CLOCK_DEFAULT_DAYS_PER_REAL_SECOND = 2;

function ensureDualClockState(state) {
  if (!state.dualClock) {
    state.dualClock = {
      inWorldDays: 0,
      daysPerRealSecond: DUAL_CLOCK_DEFAULT_DAYS_PER_REAL_SECOND,
      declarations: [],
    };
  }
  return state.dualClock;
}

function tickDualClock(state, dt) {
  const clock = ensureDualClockState(state);
  clock.inWorldDays += dt * clock.daysPerRealSecond;
}

export function declareInWorldTime(state, daysDelta, reason) {
  const clock = ensureDualClockState(state);
  if (typeof daysDelta !== "number" || daysDelta <= 0) return null;
  clock.inWorldDays += daysDelta;
  const declaration = {
    atRealElapsed: state.meta.elapsed,
    atInWorldDays: clock.inWorldDays,
    daysDelta,
    reason: reason ?? "unspecified",
  };
  clock.declarations.unshift(declaration);
  // Bound log to last 32 declarations so we don't grow unbounded.
  clock.declarations = clock.declarations.slice(0, 32);
  return declaration;
}

// Session 24: Save-state serialization primer.
// Exports a deterministic structured snapshot of the full live simulation state
// so that future sessions can implement true round-trip save/resume.
// Canonical Decision 16 (save/resume for long matches) depends on a stable
// snapshot shape. This primer establishes the shape. Restore is NOT yet wired;
// consuming this snapshot to rebuild state is a later session.
export function exportStateSnapshot(state) {
  if (!state) return null;
  const version = 1;
  const shallowCopyMap = (obj) => {
    if (!obj || typeof obj !== "object") return obj;
    return JSON.parse(JSON.stringify(obj));
  };
  const factions = {};
  for (const factionId of Object.keys(state.factions)) {
    const f = state.factions[factionId];
    factions[factionId] = {
      id: f.id,
      houseId: f.houseId,
      displayName: f.displayName,
      kind: f.kind,
      hostileTo: Array.isArray(f.hostileTo) ? [...f.hostileTo] : [],
      resources: { ...(f.resources ?? {}) },
      population: { ...(f.population ?? {}) },
      faith: shallowCopyMap(f.faith),
      conviction: shallowCopyMap(f.conviction),
      dynasty: shallowCopyMap(f.dynasty),
      ai: shallowCopyMap(f.ai),
      fieldWaterAlerts: shallowCopyMap(f.fieldWaterAlerts),
      assaultFailureStrain: f.assaultFailureStrain ?? 0,
      cohesionPenaltyUntil: f.cohesionPenaltyUntil ?? 0,
      foodStrainStreak: f.foodStrainStreak ?? 0,
      waterStrainStreak: f.waterStrainStreak ?? 0,
      bloodProductionLoad: f.bloodProductionLoad ?? 0,
      darkExtremesStreak: f.darkExtremesStreak ?? 0,
      darkExtremesActive: f.darkExtremesActive ?? false,
      worldPressureScore: f.worldPressureScore ?? 0,
      worldPressureStreak: f.worldPressureStreak ?? 0,
      worldPressureLevel: f.worldPressureLevel ?? 0,
      // Session 88: governance alliance-threshold coalition pressure state.
      governanceAlliancePressureCycles: f.governanceAlliancePressureCycles ?? 0,
      governanceAlliancePressureActive: f.governanceAlliancePressureActive ?? false,
      governanceAlliancePressureHostileCount: f.governanceAlliancePressureHostileCount ?? 0,
      // Session 90: non-aggression pact diplomacy state.
      diplomacy: shallowCopyMap(f.diplomacy ?? null),
      // Session 93: Trueborn City trade-relationship state.
      tradeRelationships: shallowCopyMap(f.tradeRelationships ?? null),
      // Session 94: Trueborn Rise arc state.
      riseArc: shallowCopyMap(f.riseArc ?? null),
      // Session 44: minor-house provenance so defection history survives save/resume.
      originFactionId: f.originFactionId ?? null,
      originLesserHouseId: f.originLesserHouseId ?? null,
      foundedAtInWorldDays: f.foundedAtInWorldDays ?? null,
    };
  }
  return {
    version,
    exportedAt: state.meta?.elapsed ?? 0,
    status: state.meta?.status ?? "unknown",
    winnerId: state.meta?.winnerId ?? null,
    victoryType: state.meta?.victoryType ?? null,
    victoryReason: state.meta?.victoryReason ?? null,
    realmCycleAccumulator: state.realmCycleAccumulator ?? 0,
    realmCycleCount: state.realmCycleCount ?? 0,
    dualClock: {
      inWorldDays: state.dualClock?.inWorldDays ?? 0,
      daysPerRealSecond: state.dualClock?.daysPerRealSecond ?? DUAL_CLOCK_DEFAULT_DAYS_PER_REAL_SECOND,
      declarations: shallowCopyMap(state.dualClock?.declarations ?? []),
    },
    matchProgression: shallowCopyMap(state.matchProgression ?? null),
    world: {
      width: state.world.width,
      height: state.world.height,
      tileSize: state.world.tileSize,
      controlPoints: (state.world.controlPoints ?? []).map((cp) => ({
        id: cp.id,
        name: cp.name,
        x: cp.x,
        y: cp.y,
        radiusTiles: cp.radiusTiles,
        captureTime: cp.captureTime,
        resourceTrickle: shallowCopyMap(cp.resourceTrickle),
        settlementClass: cp.settlementClass ?? "border_settlement",
        continentId: cp.continentId ?? null,
        ownerFactionId: cp.ownerFactionId,
        loyalty: cp.loyalty,
        captureProgress: cp.captureProgress,
        captureFactionId: cp.captureFactionId,
        contested: cp.contested ?? false,
        controlState: cp.controlState,
        fortificationTier: cp.fortificationTier,
        governorMemberId: cp.governorMemberId ?? null,
      })),
      settlements: (state.world.settlements ?? []).map((s) => ({
        id: s.id,
        factionId: s.factionId,
        settlementClass: s.settlementClass,
        fortificationTier: s.fortificationTier,
        sortieActiveUntil: s.sortieActiveUntil ?? 0,
        sortieCooldownUntil: s.sortieCooldownUntil ?? 0,
        reserveState: shallowCopyMap(s.reserveState ?? null),
        imminentEngagement: shallowCopyMap(s.imminentEngagement ?? null),
      })),
      resourceNodes: (state.world.resourceNodes ?? []).map((n) => ({
        id: n.id,
        type: n.type,
        x: n.x,
        y: n.y,
        amount: n.amount,
        harassedUntil: n.harassedUntil ?? 0,
        harassedTargetFactionId: n.harassedTargetFactionId ?? null,
        harassedByFactionId: n.harassedByFactionId ?? null,
      })),
    },
    factions,
    units: state.units.map((u) => ({
      id: u.id,
      typeId: u.typeId,
      factionId: u.factionId,
      x: Math.round(u.x * 10) / 10,
      y: Math.round(u.y * 10) / 10,
      health: Math.round(u.health * 10) / 10,
      commanderMemberId: u.commanderMemberId ?? null,
      siegeSuppliedUntil: u.siegeSuppliedUntil ?? 0,
      engineerSupportUntil: u.engineerSupportUntil ?? 0,
      logisticsInterdictedUntil: u.logisticsInterdictedUntil ?? 0,
      convoyRecoveryUntil: u.convoyRecoveryUntil ?? 0,
      interdictedByFactionId: u.interdictedByFactionId ?? null,
      escortAssignedWagonId: u.escortAssignedWagonId ?? null,
      convoyReconsolidatedAt: u.convoyReconsolidatedAt ?? 0,
      fieldWaterSuppliedUntil: u.fieldWaterSuppliedUntil ?? 0,
      lastFieldWaterTransferAt: u.lastFieldWaterTransferAt ?? -999,
      fieldWaterStrain: Math.round((u.fieldWaterStrain ?? 0) * 10) / 10,
      fieldWaterStatus: u.fieldWaterStatus ?? "steady",
      fieldWaterCriticalDuration: Math.round((u.fieldWaterCriticalDuration ?? 0) * 10) / 10,
      fieldWaterAttritionActive: Boolean(u.fieldWaterAttritionActive),
      fieldWaterDesertionRisk: Boolean(u.fieldWaterDesertionRisk),
      raidCooldownRemaining: Math.round((u.raidCooldownRemaining ?? 0) * 10) / 10,
      reserveDuty: u.reserveDuty ?? null,
      reserveSettlementId: u.reserveSettlementId ?? null,
      command: shallowCopyMap(u.command),
    })),
    buildings: state.buildings.map((b) => ({
      id: b.id,
      typeId: b.typeId,
      factionId: b.factionId,
      tileX: b.tileX,
      tileY: b.tileY,
      buildProgress: Math.round((b.buildProgress ?? 0) * 10) / 10,
      completed: b.completed,
      health: Math.round(b.health * 10) / 10,
      productionQueue: (b.productionQueue ?? []).map((q) => ({
        unitId: q.unitId,
        remaining: Math.round((q.remaining ?? 0) * 10) / 10,
        populationCost: q.populationCost,
      })),
      burnExpiresAt: b.burnExpiresAt ?? 0,
      burnDamagePerSecond: b.burnDamagePerSecond ?? 0,
      poisonedUntil: b.poisonedUntil ?? 0,
      raidedUntil: b.raidedUntil ?? 0,
      sabotageGateExposedUntil: b.sabotageGateExposedUntil ?? 0,
    })),
    projectileCount: state.projectiles?.length ?? 0,
  };
}

// Session 25: Restore state from snapshot. Consumes exportStateSnapshot output
// plus the content bundle and rebuilds a live simulation state. Canonical
// Decision 16 (save/resume) round trip now complete at the state-shape layer.
// Limitations documented: projectiles reset (deliberately — they are ephemeral
// combat resolution artifacts and do not need to persist); entity id counters
// advance based on the highest id found so new entities do not collide; the
// "state.entityIdCounter" is rebuilt from max existing id.
export function restoreStateSnapshot(content, snapshot) {
  if (!snapshot || snapshot.version !== 1) {
    return { ok: false, reason: "Unsupported snapshot version." };
  }
  // Rebuild a fresh state and overwrite from snapshot.
  const state = createSimulation(content);
  state.meta.elapsed = snapshot.exportedAt ?? 0;
  state.meta.status = snapshot.status ?? "playing";
  state.meta.winnerId = snapshot.winnerId ?? null;
  state.meta.victoryType = snapshot.victoryType ?? null;
  state.meta.victoryReason = snapshot.victoryReason ?? null;
  state.realmCycleAccumulator = snapshot.realmCycleAccumulator ?? 0;
  state.realmCycleCount = snapshot.realmCycleCount ?? 0;
  state.dualClock = {
    inWorldDays: snapshot.dualClock?.inWorldDays ?? 0,
    daysPerRealSecond: snapshot.dualClock?.daysPerRealSecond ?? DUAL_CLOCK_DEFAULT_DAYS_PER_REAL_SECOND,
    declarations: Array.isArray(snapshot.dualClock?.declarations) ? [...snapshot.dualClock.declarations] : [],
  };
  if (snapshot.matchProgression) {
    state.matchProgression = {
      ...snapshot.matchProgression,
      nextStageShortfalls: [...(snapshot.matchProgression.nextStageShortfalls ?? [])],
    };
  }

  // Restore world.controlPoints
  if (Array.isArray(snapshot.world?.controlPoints)) {
    snapshot.world.controlPoints.forEach((snapCp) => {
      let cp = state.world.controlPoints.find((c) => c.id === snapCp.id);
      if (!cp) {
        cp = {
          id: snapCp.id,
          name: snapCp.name ?? snapCp.id,
          x: snapCp.x ?? 0,
          y: snapCp.y ?? 0,
          radiusTiles: snapCp.radiusTiles ?? 3,
          captureTime: snapCp.captureTime ?? 10,
          resourceTrickle: { ...(snapCp.resourceTrickle ?? {}) },
          settlementClass: snapCp.settlementClass ?? "border_settlement",
          continentId: snapCp.continentId ?? null,
          ownerFactionId: null,
          loyalty: 18,
          captureProgress: 0,
          captureFactionId: null,
          contested: false,
          controlState: "neutral",
          fortificationTier: 0,
          governorMemberId: null,
        };
        state.world.controlPoints.push(cp);
      }
      cp.name = snapCp.name ?? cp.name;
      cp.x = snapCp.x ?? cp.x;
      cp.y = snapCp.y ?? cp.y;
      cp.radiusTiles = snapCp.radiusTiles ?? cp.radiusTiles;
      cp.captureTime = snapCp.captureTime ?? cp.captureTime;
      cp.resourceTrickle = { ...(snapCp.resourceTrickle ?? cp.resourceTrickle ?? {}) };
      cp.settlementClass = snapCp.settlementClass ?? cp.settlementClass ?? "border_settlement";
      cp.continentId = snapCp.continentId ?? cp.continentId ?? null;
      cp.ownerFactionId = snapCp.ownerFactionId;
      cp.loyalty = snapCp.loyalty;
      cp.captureProgress = snapCp.captureProgress;
      cp.captureFactionId = snapCp.captureFactionId;
      cp.contested = snapCp.contested ?? false;
      cp.controlState = snapCp.controlState;
      cp.fortificationTier = snapCp.fortificationTier;
      cp.governorMemberId = snapCp.governorMemberId;
    });
  }

  // Restore settlements (fort tier + sortie timers)
  if (Array.isArray(snapshot.world?.settlements)) {
    snapshot.world.settlements.forEach((snapS) => {
      const s = state.world.settlements.find((x) => x.id === snapS.id);
      if (!s) return;
      s.fortificationTier = snapS.fortificationTier;
      s.sortieActiveUntil = snapS.sortieActiveUntil ?? 0;
      s.sortieCooldownUntil = snapS.sortieCooldownUntil ?? 0;
      s.reserveState = {
        lastCommitAt: -999,
        threatActive: false,
        lastMessageAt: -999,
        ...(snapS.reserveState ?? {}),
      };
      s.imminentEngagement = {
        ...createDefaultImminentEngagementState(s.id, s.name ?? s.id),
        ...(snapS.imminentEngagement ?? {}),
      };
    });
  }

  // Restore resource node amounts
  if (Array.isArray(snapshot.world?.resourceNodes)) {
    snapshot.world.resourceNodes.forEach((snapN) => {
      let n = state.world.resourceNodes.find((x) => x.id === snapN.id);
      if (!n) {
        n = {
          id: snapN.id,
          type: snapN.type,
          x: snapN.x,
          y: snapN.y,
          amount: snapN.amount,
          harassedUntil: 0,
          harassedTargetFactionId: null,
          harassedByFactionId: null,
        };
        state.world.resourceNodes.push(n);
      }
      n.amount = snapN.amount;
      n.harassedUntil = snapN.harassedUntil ?? 0;
      n.harassedTargetFactionId = snapN.harassedTargetFactionId ?? null;
      n.harassedByFactionId = snapN.harassedByFactionId ?? null;
    });
  }

  // Restore factions (overwrite the freshly-created ones field-by-field)
  if (snapshot.factions) {
    for (const factionId of Object.keys(snapshot.factions)) {
      const snapF = snapshot.factions[factionId];
      // Session 44: if the snapshot has a faction that createSimulation did not
      // rebuild (typically a minor_house that spawned mid-match from a lesser-
      // house defection), reconstruct a minimal shell for it so the field
      // assignments below can land. Without this the minor faction is silently
      // lost on reload.
      if (!state.factions[factionId]) {
        state.factions[factionId] = {
          id: factionId,
          houseId: snapF.houseId,
          displayName: snapF.displayName,
          kind: snapF.kind ?? "minor_house",
          hostileTo: Array.isArray(snapF.hostileTo) ? [...snapF.hostileTo] : [],
          presentation: snapF.presentation ?? { primaryColor: "#777777", accentColor: "#d0b188" },
          resources: {},
          dynasty: null,
          faith: null,
          conviction: null,
          population: { total: 0, cap: 0, baseCap: 0, reserved: 0, growthProgress: 0 },
          ai: null,
          fieldWaterAlerts: { lastCriticalMessageAt: -999, lastRecoveredMessageAt: -999 },
          siegeLogistics: { lastUnsuppliedMessageAt: -999, lastResuppliedMessageAt: -999 },
          worldPressureScore: 0,
          worldPressureStreak: 0,
          worldPressureLevel: 0,
        };
      }
      const f = state.factions[factionId];
      f.houseId = snapF.houseId;
      f.displayName = snapF.displayName;
      f.kind = snapF.kind ?? f.kind;
      f.hostileTo = Array.isArray(snapF.hostileTo) ? [...snapF.hostileTo] : [];
      f.resources = { ...snapF.resources };
      f.population = { ...snapF.population };
      if (snapF.faith) f.faith = snapF.faith;
      if (snapF.conviction) f.conviction = snapF.conviction;
      if (snapF.dynasty) f.dynasty = snapF.dynasty;
      if (snapF.ai) f.ai = snapF.ai;
      f.fieldWaterAlerts = {
        ...(f.fieldWaterAlerts ?? { lastCriticalMessageAt: -999, lastRecoveredMessageAt: -999 }),
        ...(snapF.fieldWaterAlerts ?? {}),
      };
      f.assaultFailureStrain = snapF.assaultFailureStrain ?? 0;
      f.cohesionPenaltyUntil = snapF.cohesionPenaltyUntil ?? 0;
      f.foodStrainStreak = snapF.foodStrainStreak ?? 0;
      f.waterStrainStreak = snapF.waterStrainStreak ?? 0;
      f.bloodProductionLoad = snapF.bloodProductionLoad ?? 0;
      f.darkExtremesStreak = snapF.darkExtremesStreak ?? 0;
      f.darkExtremesActive = snapF.darkExtremesActive ?? false;
      f.worldPressureScore = snapF.worldPressureScore ?? 0;
      f.worldPressureStreak = snapF.worldPressureStreak ?? 0;
      f.worldPressureLevel = snapF.worldPressureLevel ?? 0;
      // Session 88: governance alliance-threshold coalition pressure restore.
      f.governanceAlliancePressureCycles = snapF.governanceAlliancePressureCycles ?? 0;
      f.governanceAlliancePressureActive = snapF.governanceAlliancePressureActive ?? false;
      f.governanceAlliancePressureHostileCount = snapF.governanceAlliancePressureHostileCount ?? 0;
      // Session 90: non-aggression pact diplomacy restore.
      if (snapF.diplomacy) {
        f.diplomacy = JSON.parse(JSON.stringify(snapF.diplomacy));
      }
      // Session 93: Trueborn City trade-relationship restore.
      if (snapF.tradeRelationships) {
        f.tradeRelationships = JSON.parse(JSON.stringify(snapF.tradeRelationships));
      }
      // Session 94: Trueborn Rise arc restore.
      if (snapF.riseArc) {
        f.riseArc = JSON.parse(JSON.stringify(snapF.riseArc));
      }
      f.originFactionId = snapF.originFactionId ?? null;
      f.originLesserHouseId = snapF.originLesserHouseId ?? null;
      f.foundedAtInWorldDays = snapF.foundedAtInWorldDays ?? null;
    }
  }

  // Replace units array from snapshot
  state.units = (snapshot.units ?? []).map((snapU) => {
    const unitDef = getUnitDef(content, snapU.typeId);
    return {
      id: snapU.id,
      typeId: snapU.typeId,
      factionId: snapU.factionId,
      x: snapU.x,
      y: snapU.y,
      radius: unitDef?.role === "worker" ? 10 : 12,
      health: snapU.health,
      attackCooldownRemaining: 0,
      gatherProgress: 0,
      carrying: null,
      commanderMemberId: snapU.commanderMemberId,
      reserveDuty: snapU.reserveDuty,
      reserveSettlementId: snapU.reserveSettlementId,
      supportStatus: null,
      engineerSupportUntil: snapU.engineerSupportUntil ?? 0,
      siegeSuppliedUntil: snapU.siegeSuppliedUntil ?? 0,
      logisticsInterdictedUntil: snapU.logisticsInterdictedUntil ?? 0,
      convoyRecoveryUntil: snapU.convoyRecoveryUntil ?? 0,
      interdictedByFactionId: snapU.interdictedByFactionId ?? null,
      escortAssignedWagonId: snapU.escortAssignedWagonId ?? null,
      convoyReconsolidatedAt: snapU.convoyReconsolidatedAt ?? 0,
      fieldWaterSuppliedUntil: snapU.fieldWaterSuppliedUntil ?? 0,
      lastFieldWaterTransferAt: snapU.lastFieldWaterTransferAt ?? -999,
      fieldWaterStrain: snapU.fieldWaterStrain ?? 0,
      fieldWaterStatus: snapU.fieldWaterStatus ?? "steady",
      fieldWaterCriticalDuration: snapU.fieldWaterCriticalDuration ?? 0,
      fieldWaterAttritionActive: snapU.fieldWaterAttritionActive ?? false,
      fieldWaterDesertionRisk: snapU.fieldWaterDesertionRisk ?? false,
      raidCooldownRemaining: snapU.raidCooldownRemaining ?? 0,
      lastSupplyTransferAt: -999,
      command: snapU.command ?? null,
    };
  });

  // Replace buildings array from snapshot
  state.buildings = (snapshot.buildings ?? []).map((snapB) => {
    return {
      id: snapB.id,
      typeId: snapB.typeId,
      factionId: snapB.factionId,
      tileX: snapB.tileX,
      tileY: snapB.tileY,
      buildProgress: snapB.buildProgress,
      completed: snapB.completed,
      health: snapB.health,
      productionQueue: (snapB.productionQueue ?? []).slice(),
      burnExpiresAt: snapB.burnExpiresAt ?? 0,
      burnDamagePerSecond: snapB.burnDamagePerSecond ?? 0,
      poisonedUntil: snapB.poisonedUntil ?? 0,
      raidedUntil: snapB.raidedUntil ?? 0,
      sabotageGateExposedUntil: snapB.sabotageGateExposedUntil ?? 0,
    };
  });

  // Reset projectiles (ephemeral)
  state.projectiles = [];

  const getPrefixMax = (ids, prefix) => {
    const escapedPrefix = prefix.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
    const pattern = new RegExp(`^${escapedPrefix}-(\\d+)$`);
    return ids.reduce((max, id) => {
      const match = String(id).match(pattern);
      return match ? Math.max(max, Number(match[1])) : max;
    }, 0);
  };

  const dynastyIds = Object.values(state.factions).flatMap((faction) => {
    const dynasty = faction.dynasty ?? {};
    return [
      ...(dynasty.members ?? []).map((member) => member.id),
      ...(dynasty.lesserHouses ?? []).map((lesserHouse) => lesserHouse.id),
      ...(dynasty.marriageProposalsIn ?? []).map((proposal) => proposal.id),
      ...(dynasty.marriageProposalsOut ?? []).map((proposal) => proposal.id),
      ...(dynasty.marriages ?? []).map((marriage) => marriage.id),
      ...(dynasty.intelligenceReports ?? []).map((report) => report.id),
      ...(dynasty.counterIntelligence ?? []).map((entry) => entry.id),
      ...((dynasty.politicalEvents?.active ?? []).map((entry) => entry.id)),
      ...((dynasty.politicalEvents?.history ?? []).map((entry) => entry.id)),
      ...((dynasty.operations?.active ?? []).map((operation) => operation.id)),
      ...((dynasty.operations?.history ?? []).map((operation) => operation.id)),
    ];
  });
  const faithIds = Object.values(state.factions).flatMap((faction) =>
    (faction.faith?.activeHolyWars ?? []).map((entry) => entry.id));

  state.counters = {
    ...(state.counters ?? {}),
    unit: getPrefixMax(state.units.map((unit) => unit.id), "unit"),
    building: getPrefixMax(state.buildings.map((building) => building.id), "building"),
    projectile: 0,
    dynastyOperation: getPrefixMax(dynastyIds, "dynastyOperation"),
    dynastyIntel: getPrefixMax(dynastyIds, "dynastyIntel"),
    dynastyCounter: getPrefixMax(dynastyIds, "dynastyCounter"),
    dynastyEvent: getPrefixMax(dynastyIds, "dynastyEvent"),
    faithHolyWar: getPrefixMax(faithIds, "faithHolyWar"),
    "lesser-house": getPrefixMax(dynastyIds, "lesser-house"),
    "marriage-proposal": getPrefixMax(dynastyIds, "marriage-proposal"),
    marriage: getPrefixMax(dynastyIds, "marriage"),
    "bloodline-child": getPrefixMax(dynastyIds, "bloodline-child"),
  };

  updateMatchProgressionState(state, { announce: false, preserveAnnouncements: true });

  return { ok: true, state };
}

export function getRealmConditionSnapshot(state, factionId) {
  const faction = getFaction(state, factionId);
  ensureFaithCovenantTestCompletionFromLegacyState(state, factionId);
  if (!faction) return null;
  const legibility = state.realmConditions?.legibility ?? {};
  const territories = state.world.controlPoints.filter((cp) => cp.ownerFactionId === factionId);
  const siegeState = getFactionSiegeState(state, factionId);
  const loyaltyAvg = territories.length > 0
    ? territories.reduce((sum, cp) => sum + cp.loyalty, 0) / territories.length
    : 50;
  const loyaltyMin = territories.length > 0
    ? territories.reduce((min, cp) => Math.min(min, cp.loyalty), 100)
    : 50;
  const verdantWardenTerritoryProfiles = territories.map((controlPoint) =>
    getVerdantWardenControlPointSupportProfile(state, controlPoint));
  const verdantWardenSupportedTerritories = verdantWardenTerritoryProfiles.filter((profile) => profile.count > 0).length;
  const verdantWardenPeakCoverage = verdantWardenTerritoryProfiles.reduce(
    (max, profile) => Math.max(max, profile.count ?? 0),
    0,
  );
  const verdantWardenPeakLoyaltyGainMultiplier = verdantWardenTerritoryProfiles.reduce(
    (max, profile) => Math.max(max, profile.loyaltyGainMultiplier ?? 1),
    1,
  );
  const verdantWardenPeakProtectionMultiplier = verdantWardenTerritoryProfiles.reduce(
    (max, profile) => Math.max(max, profile.loyaltyProtectionMultiplier ?? 1),
    1,
  );
  const foodStock = faction.resources.food ?? 0;
  const waterStock = faction.resources.water ?? 0;
  const fieldWaterState = getFactionFieldWaterState(state, factionId);
  const raidedInfrastructure = state.buildings.filter((building) =>
    building.factionId === factionId &&
    building.completed &&
    building.health > 0 &&
    isBuildingUnderScoutRaid(state, building));
  const harassedResourceNodes = (state.world.resourceNodes ?? []).filter((resourceNode) =>
    isResourceNodeUnderScoutHarassment(state, resourceNode, factionId));
  const harassedWorkerCount = state.units.filter((unit) =>
    unit.factionId === factionId &&
    unit.health > 0 &&
    getUnitDef(state.content, unit.typeId)?.role === "worker" &&
    unit.command?.type === "harass_retreat").length;
  const interdictedSupplyWagons = siegeState.supplyWagons.filter((unit) => isSupplyWagonInterdicted(state, unit));
  const raidedSupplyCampCount = raidedInfrastructure.filter((building) => building.typeId === "supply_camp").length;
  const raidedWaterAnchorCount = raidedInfrastructure.filter((building) =>
    (getBuildingDef(state.content, building.typeId)?.armyWaterSupportRadius ?? 0) > 0).length;
  const raidedDropoffCount = raidedInfrastructure.filter((building) =>
    (getBuildingDef(state.content, building.typeId)?.dropoffResources?.length ?? 0) > 0).length;
  const pop = faction.population.total;
  const cap = faction.population.cap;
  const foodRatio = pop > 0 ? foodStock / pop : 99;
  const waterRatio = pop > 0 ? waterStock / pop : 99;
  const capRatio = cap > 0 ? pop / cap : 0;

  // Primary keep fortification tier (the bloodline seat).
  const primarySeat = (state.world.settlements ?? []).find(
    (s) => s.factionId === factionId && s.settlementClass === "primary_dynastic_keep",
  );
  const primaryKeepTier = primarySeat?.fortificationTier ?? 0;
  const primaryKeepCeiling = primarySeat
    ? state.content.byId.settlementClasses?.[primarySeat.settlementClass]?.defensiveCeiling ?? 0
    : 0;
  const primaryDefense = primarySeat ? getSettlementDefenseState(state, primarySeat) : null;
  const primaryImminentEngagement = primarySeat ? ensureImminentEngagementState(primarySeat) : null;
  const primaryImminentPhase = primaryImminentEngagement
    ? primaryImminentEngagement.active
      ? "countdown"
      : primaryImminentEngagement.windowConsumed
        ? "engaged"
        : "idle"
    : "idle";
  const primaryImminentPosture = getImminentEngagementPosture(primaryImminentEngagement?.selectedResponseId);

  const militaryCount = state.units.filter(
    (u) => u.factionId === factionId && u.health > 0 && state.content.byId.units[u.typeId]?.role !== "worker",
  ).length;

  // Faith block (canonical pressure state 8). Surface covenant, doctrine path, intensity tier.
  const faithState = faction.faith ?? null;
  const selectedFaith = faithState?.selectedFaithId ? getFaith(state.content, faithState.selectedFaithId) : null;
  const discoveredFaithCount = faithState?.discoveredFaithIds?.length ?? 0;
  const faithIntensity = Math.round(faithState?.intensity ?? 0);
  const faithBand = selectedFaith
    ? (faithIntensity >= 60 ? "green" : faithIntensity >= 25 ? "yellow" : "yellow")
    : (discoveredFaithCount > 0 ? "yellow" : "red");
  const activeDivineRightDeclaration = getActiveDivineRightDeclaration(faction, state.meta.elapsed);
  const incomingDivineRightDeclarations = getIncomingDivineRightDeclarations(state, factionId, state.meta.elapsed);
  const divineRightProfile = getDivineRightDeclarationProfile(state, factionId);
  const activeSuccessionCrisis = getActiveSuccessionCrisis(faction);
  const activeCovenantTest = getActiveCovenantTest(faction);
  const activeTerritorialGovernanceRecognition = getActiveTerritorialGovernanceRecognition(faction);
  const incomingCovenantTests = getIncomingCovenantTests(state, factionId);

  // Conviction block (canonical pressure state 9). Four-bucket ledger with derived band.
  const convictionState = faction.conviction ?? null;
  const convictionBuckets = convictionState?.buckets ?? {};
  const convictionScore = convictionState?.score ?? 0;
  let topBucket = null;
  for (const [key, value] of Object.entries(convictionBuckets)) {
    if (!topBucket || value > topBucket.value) {
      topBucket = { key, value };
    }
  }
  const convictionBandId = convictionState?.bandId ?? "neutral";
  const convictionBandLabel = convictionState?.bandLabel ?? "Neutral";
  const convictionBand = convictionBandId === "apex_moral" || convictionBandId === "moral"
    ? "green"
    : convictionBandId === "neutral"
      ? "yellow"
      : "red";

  // Logistics block (canonical pressure state 10). Supply chain integrity.
  const logisticsBand = fieldWaterState.criticalUnits > 0
    ? "red"
    : fieldWaterState.attritionUnits > 0 || fieldWaterState.desertionRiskUnits > 0
      ? "red"
    : interdictedSupplyWagons.length > 0 && (siegeState.engineCount > 0 || fieldWaterState.unitCount > 0)
      ? "red"
    : harassedWorkerCount > 0
      ? "red"
    : raidedSupplyCampCount > 0 && siegeState.engineCount > 0
      ? "red"
    : interdictedSupplyWagons.length > 0
      ? "yellow"
    : siegeState.convoyRecoveringCount > 0
      ? "yellow"
    : harassedResourceNodes.length > 0
      ? "yellow"
    : raidedInfrastructure.length > 0
      ? "yellow"
    : fieldWaterState.strainedUnits > 0
      ? "yellow"
      : siegeState.engineCount === 0
        ? "green"
        : siegeState.unsuppliedEngines === 0 && siegeState.supplyWagonCount > 0
          ? "green"
          : siegeState.unsuppliedEngines > 0 && siegeState.suppliedEngines > 0
            ? "yellow"
            : "red";

  // World-pressure block (canonical pressure state 11). Tribes, contested territories, active operations.
  const tribeUnitCount = state.units.filter(
    (u) => u.factionId === "tribes" && u.health > 0,
  ).length;
  const contestedTerritories = state.world.controlPoints.filter(
    (cp) => cp.ownerFactionId !== factionId && (cp.captureProgress ?? 0) > 0,
  ).length;
  const activeOperations = faction.dynasty?.operations?.active?.length ?? 0;
  const heldCaptives = faction.dynasty?.captives?.length ?? 0;
  const fallenMembers = faction.dynasty?.attachments?.fallenMembers?.length ?? 0;
  const worldPressureProfile = getWorldPressureTargetProfile(state, factionId);
  const convergenceProfile = getWorldPressureConvergenceProfile(state, factionId);
  const cadetWorldPressureProfile = getWorldPressureCadetInstabilityProfile(state, faction);
  const matchProgression = getMatchProgressionSnapshot(state);
  const greatReckoning = getGreatReckoningProfile(state);
  const greatReckoningTargeted = greatReckoning.active && greatReckoning.targetFactionId === factionId;
  const greatReckoningTargetName = greatReckoning.targetFactionId
    ? getFactionDisplayName(state, greatReckoning.targetFactionId)
    : null;
  const splinterOpportunityProfiles = Object.values(state.factions)
    .filter((entry) => entry?.kind === "minor_house" && entry.originFactionId === factionId)
    .map((entry) => getMinorHousePressureOpportunityProfile(state, entry))
    .filter((profile) => profile.pressured);
  const splinterOpportunityLeader = splinterOpportunityProfiles
    .sort((left, right) =>
      (right.level - left.level) ||
      (right.retakeTempoMultiplier - left.retakeTempoMultiplier) ||
      (right.levyTempoMultiplier - left.levyTempoMultiplier))[0] ?? null;
  const worldPressureSignals =
    tribeUnitCount +
    contestedTerritories +
    activeOperations +
    (worldPressureProfile.level * 2) +
    Number(Boolean(activeTerritorialGovernanceRecognition)) +
    Number(Boolean(activeDivineRightDeclaration)) +
    Number(faction.darkExtremesActive ?? false) +
    (greatReckoning.active ? 2 : 0);
  const worldPressureBand = greatReckoningTargeted
    ? "red"
    : worldPressureProfile.targeted
    ? worldPressureProfile.level >= 2
      ? "red"
      : "yellow"
    : worldPressureSignals === 0
      ? "green"
      : worldPressureSignals <= 4
        ? "yellow"
        : "red";

  // Session 14: expose hostile post-repulse legibility so the player sees when
  // a recent defensive success has actually cost the attacker tempo. Canonical
  // master doctrine section X — failed assaults must visibly cost the attacker.
  const hostileFactions = Object.values(state.factions).filter(
    (f) => f.kind === "kingdom" && f.id !== factionId,
  );
  let hostilePostRepulseActive = false;
  let hostilePostRepulseRemaining = 0;
  let hostileCohesionStrain = 0;
  hostileFactions.forEach((hostile) => {
    if ((hostile.cohesionPenaltyUntil ?? 0) > state.meta.elapsed) {
      hostilePostRepulseActive = true;
      hostilePostRepulseRemaining = Math.max(
        hostilePostRepulseRemaining,
        hostile.cohesionPenaltyUntil - state.meta.elapsed,
      );
    }
    hostileCohesionStrain = Math.max(hostileCohesionStrain, hostile.assaultFailureStrain ?? 0);
  });

  const dualClock = state.dualClock ?? { inWorldDays: 0, declarations: [] };
  return {
    cycleCount: state.realmCycleCount ?? 0,
    cycleProgress: (state.realmCycleAccumulator ?? 0) / (state.realmConditions?.cycleSeconds ?? REALM_CYCLE_DEFAULT_SECONDS),
    cycle: {
      count: state.realmCycleCount ?? 0,
      progress: (state.realmCycleAccumulator ?? 0) / (state.realmConditions?.cycleSeconds ?? REALM_CYCLE_DEFAULT_SECONDS),
      band: greatReckoning.active ? (greatReckoningTargeted ? "red" : "yellow") : matchProgression.stageNumber >= 4 ? "yellow" : "green",
      stageId: matchProgression.stageId,
      stageNumber: matchProgression.stageNumber,
      stageLabel: matchProgression.stageLabel,
      phaseId: matchProgression.phaseId,
      phaseLabel: matchProgression.phaseLabel,
      stageReadiness: Math.round((matchProgression.stageReadiness ?? 0) * 100) / 100,
      nextStageId: matchProgression.nextStageId,
      nextStageLabel: matchProgression.nextStageLabel,
      nextStageShortfalls: [...(matchProgression.nextStageShortfalls ?? [])],
      declarationCount: matchProgression.declarationCount ?? 0,
      greatReckoningActive: greatReckoning.active,
      greatReckoningTargetFactionId: greatReckoning.targetFactionId,
      greatReckoningTargetFactionName: greatReckoningTargetName,
      greatReckoningShare: Math.round((greatReckoning.territoryShare ?? 0) * 1000) / 1000,
    },
    matchProgression,
    // Session 26: dual-clock declaration layer.
    dualClock: {
      inWorldDays: Math.round(dualClock.inWorldDays * 10) / 10,
      inWorldYears: Math.round((dualClock.inWorldDays / 365) * 100) / 100,
      recentDeclarations: (dualClock.declarations ?? []).slice(0, 5),
    },
    population: {
      value: pop,
      cap,
      band: capRatio <= (legibility.populationGreenCapRatio ?? 0.75)
        ? "green"
        : capRatio <= (legibility.populationYellowCapRatio ?? 0.92)
          ? "yellow"
          : "red",
      // Session 15: Ironmark Blood Production load for legibility.
      // Other houses surface 0; Ironmark surfaces real accumulated load.
      bloodProductionLoad: Math.round((faction.bloodProductionLoad ?? 0) * 10) / 10,
      bloodProductionActive: faction.houseId === "ironmark" && (faction.bloodProductionLoad ?? 0) > 8,
    },
    food: {
      stock: Math.floor(foodStock),
      need: pop,
      ratio: foodRatio,
      band: legibilityBand(foodRatio, legibility.foodGreenRatio ?? 1.35, legibility.foodYellowRatio ?? 1.05),
      famineStreak: faction.foodStrainStreak ?? 0,
    },
    water: {
      stock: Math.floor(waterStock),
      need: pop,
      ratio: waterRatio,
      band: legibilityBand(waterRatio, legibility.waterGreenRatio ?? 1.35, legibility.waterYellowRatio ?? 1.05),
      crisisStreak: faction.waterStrainStreak ?? 0,
    },
    loyalty: {
      average: Math.round(loyaltyAvg),
      min: Math.round(loyaltyMin),
      band: legibilityBand(loyaltyAvg, legibility.loyaltyGreenFloor ?? 62, legibility.loyaltyYellowFloor ?? 32),
      territoryCount: territories.length,
      verdantWardenSupportedTerritories,
      verdantWardenPeakCoverage,
      verdantWardenLoyaltyGainBonus: Math.round((verdantWardenPeakLoyaltyGainMultiplier - 1) * 100),
      verdantWardenProtectionBonus: Math.round((verdantWardenPeakProtectionMultiplier - 1) * 100),
    },
    fortification: {
      primaryKeepName: primarySeat?.name ?? null,
      tier: primaryKeepTier,
      ceiling: primaryKeepCeiling,
      threatActive: primaryDefense?.enemyCombatants.length > 0,
      readyReserves: primaryDefense?.readyReserves ?? 0,
      recoveringReserves: primaryDefense?.recoveringReserves ?? 0,
      commanderPresent: primaryDefense?.keepPresence.commanderPresent ?? false,
      commanderAtKeep: primaryDefense?.keepPresence.commanderPresent ?? false,
      sortieActive: primarySeat
        ? (primarySeat.sortieActiveUntil ?? 0) > state.meta.elapsed
        : false,
      // Session 41: expose the active-window remaining seconds so the UI can show
      // "burst ends in Ns" without hardcoding SORTIE_DURATION_SECONDS.
      sortieActiveRemaining: primarySeat
        ? Math.max(0, (primarySeat.sortieActiveUntil ?? 0) - state.meta.elapsed)
        : 0,
      sortieCooldownRemaining: primarySeat
        ? Math.max(0, (primarySeat.sortieCooldownUntil ?? 0) - state.meta.elapsed)
        : 0,
      // Session 41: surface canonical durations so the UI can render progress
      // percentages and bars without import coupling. Values come from the
      // SORTIE_DURATION_SECONDS / SORTIE_COOLDOWN_SECONDS constants.
      sortieDurationSeconds: SORTIE_DURATION_SECONDS,
      sortieCooldownSeconds: SORTIE_COOLDOWN_SECONDS,
      sortieReady: primarySeat
        ? (primarySeat.sortieCooldownUntil ?? 0) <= state.meta.elapsed &&
          (primaryDefense?.keepPresence?.commanderPresent ?? false) &&
          ((primaryDefense?.enemyCombatants?.length ?? 0) > 0)
        : false,
      governorPresent: primaryDefense?.keepPresence.governorPresent ?? false,
      bloodlinePresent: primaryDefense?.keepPresence.bloodlinePresent ?? false,
      governorSpecialization: primaryDefense?.governorProfile?.id ?? null,
      governorSpecializationLabel: primaryDefense?.governorProfile?.label ?? null,
      wardId: primaryDefense?.wardProfile?.id ?? null,
      wardLabel: primaryDefense?.wardProfile?.label ?? null,
      wardSurgeActive: primaryDefense?.wardProfile?.surgeActive ?? false,
      verdantWardenCount: primaryDefense?.verdantWardenSupport?.count ?? 0,
      verdantWardenAttackBonus: Math.round((((primaryDefense?.verdantWardenSupport?.defenderAttackMultiplier) ?? 1) - 1) * 100),
      verdantWardenReserveHealBonus: Math.round((((primaryDefense?.verdantWardenSupport?.reserveHealMultiplier) ?? 1) - 1) * 100),
      verdantWardenReserveMusterBonus: Math.round((((primaryDefense?.verdantWardenSupport?.reserveMusterMultiplier) ?? 1) - 1) * 100),
      imminentEngagement: primaryImminentEngagement
        ? {
          active: Boolean(primaryImminentEngagement.active),
          phase: primaryImminentPhase,
          phaseLabel: primaryImminentPhase === "countdown"
            ? "Immediate Window"
            : primaryImminentPhase === "engaged"
              ? "Battle Joined"
              : "Idle",
          settlementId: primarySeat?.id ?? null,
          settlementName: primarySeat?.name ?? null,
          battleTypeId: primaryImminentEngagement.battleTypeId ?? null,
          battleTypeLabel: primaryImminentEngagement.battleTypeLabel ?? null,
          hostileFactionId: primaryImminentEngagement.hostileFactionId ?? null,
          hostileFactionName: primaryImminentEngagement.hostileFactionName ?? null,
          hostileCount: primaryImminentEngagement.hostileCount ?? 0,
          hostileSiegeCount: primaryImminentEngagement.hostileSiegeCount ?? 0,
          hostileScoutCount: primaryImminentEngagement.hostileScoutCount ?? 0,
          totalSeconds: Math.round((primaryImminentEngagement.totalSeconds ?? 0) * 10) / 10,
          remainingSeconds: Math.round((primaryImminentEngagement.remainingSeconds ?? 0) * 10) / 10,
          watchtowerCount: primaryImminentEngagement.watchtowerCount ?? 0,
          localLoyalty: Math.round(primaryImminentEngagement.localLoyalty ?? 50),
          localLoyaltyMin: Math.round(primaryImminentEngagement.localLoyaltyMin ?? 50),
          commanderPresent: primaryImminentEngagement.commanderPresent ?? false,
          commanderRecallAvailable: primaryImminentEngagement.commanderRecallAvailable ?? false,
          commanderRecallIssued: (primaryImminentEngagement.commanderRecallIssuedAt ?? -999) > 0,
          governorPresent: primaryImminentEngagement.governorPresent ?? false,
          bloodlineAtRisk: primaryImminentEngagement.bloodlineAtRisk ?? false,
          bloodlineProtectionActive: (primaryImminentEngagement.bloodlineProtectionUntil ?? 0) > state.meta.elapsed,
          bloodlineProtectionRemaining: Math.max(0, (primaryImminentEngagement.bloodlineProtectionUntil ?? 0) - state.meta.elapsed),
          selectedResponseId: primaryImminentEngagement.selectedResponseId ?? "steady",
          selectedResponseLabel: primaryImminentEngagement.selectedResponseLabel ?? primaryImminentPosture.label,
          reinforcementsCommitted: (primaryImminentEngagement.reinforcementsCommittedUntil ?? 0) > state.meta.elapsed,
          reinforcementsRemaining: Math.max(0, (primaryImminentEngagement.reinforcementsCommittedUntil ?? 0) - state.meta.elapsed),
          readyReserveCount: primaryDefense?.readyReserves ?? 0,
          recoveringReserveCount: primaryDefense?.recoveringReserves ?? 0,
        }
        : null,
      band: primaryKeepCeiling === 0
        ? "red"
        : primaryKeepTier >= primaryKeepCeiling * 0.75
          ? "green"
          : primaryKeepTier > 0
            ? "yellow"
            : "red",
    },
    military: {
      count: militaryCount,
      assaultStrain: Math.round((faction.assaultFailureStrain ?? 0) * 10) / 10,
      cohesionPenaltyActive: (faction.cohesionPenaltyUntil ?? 0) > state.meta.elapsed,
      siegeEngineCount: siegeState.engineCount,
      engineerCount: siegeState.engineerCount,
      supplyWagonCount: siegeState.supplyWagonCount,
      supplyCampCount: siegeState.supplyCampCount,
      suppliedEngines: siegeState.suppliedEngines,
        engineerSupportedEngines: siegeState.engineerSupportedEngines,
        unsuppliedEngines: siegeState.unsuppliedEngines,
        fieldWaterSupportedUnits: fieldWaterState.supportedUnits,
        fieldWaterStrainedUnits: fieldWaterState.strainedUnits,
        fieldWaterCriticalUnits: fieldWaterState.criticalUnits,
        fieldWaterAttritionUnits: fieldWaterState.attritionUnits,
        fieldWaterDesertionRiskUnits: fieldWaterState.desertionRiskUnits,
        formalSiegeReady: siegeState.readyForFormalAssault,
        band: militaryCount === 0
          ? "red"
          : ((faction.cohesionPenaltyUntil ?? 0) > state.meta.elapsed)
            ? "red"
          : fieldWaterState.desertionRiskUnits > 0 || fieldWaterState.attritionUnits > 0 || fieldWaterState.criticalUnits > 0
              ? "red"
            : siegeState.unsuppliedEngines > 0
              ? "yellow"
              : fieldWaterState.strainedUnits > 0
                ? "yellow"
            : (faction.assaultFailureStrain ?? 0) >= 3
              ? "yellow"
              : "green",
    },
    faith: {
      selectedFaithId: faithState?.selectedFaithId ?? null,
      selectedFaithName: selectedFaith?.name ?? null,
      doctrinePath: faithState?.doctrinePath ?? null,
      intensity: faithIntensity,
      level: faithState?.level ?? 0,
      tierLabel: faithState?.tierLabel ?? null,
      discoveredFaithCount,
      covenantTestPassed: Boolean(faithState?.covenantTestPassed),
      divineRightEligible: getDivineRightDeclarationTerms(state, factionId).available,
      divineRightRecognitionSharePct: divineRightProfile.recognitionSharePct,
      divineRightRequiredSharePct: divineRightProfile.requiredSharePct,
      divineRightApexStructureName: divineRightProfile.activeApexStructureName,
      activeCovenantTest: activeCovenantTest
        ? serializeCovenantTestEvent(state, factionId, activeCovenantTest)
        : null,
      incomingCovenantTests: incomingCovenantTests.map((entry) => ({
        ...entry,
        sourceFactionName: getFactionDisplayName(state, entry.sourceFactionId),
      })),
      lastCovenantTestOutcome: faithState?.lastCovenantTestOutcome ? { ...faithState.lastCovenantTestOutcome } : null,
      activeDivineRightDeclaration: activeDivineRightDeclaration
        ? {
          ...activeDivineRightDeclaration,
          remainingSeconds: Math.max(0, (activeDivineRightDeclaration.resolveAt ?? 0) - state.meta.elapsed),
        }
        : null,
      incomingDivineRightDeclarations: incomingDivineRightDeclarations.map((entry) => ({
        ...entry,
        sourceFactionName: getFactionDisplayName(state, entry.sourceFactionId),
        remainingSeconds: Math.max(0, (entry.resolveAt ?? 0) - state.meta.elapsed),
      })),
      lastDivineRightOutcome: faithState?.lastDivineRightOutcome ? { ...faithState.lastDivineRightOutcome } : null,
      band: faithBand,
    },
    dynasty: {
      legitimacy: Math.round((faction.dynasty?.legitimacy ?? 0) * 10) / 10,
      interregnum: faction.dynasty?.interregnum ?? false,
      activePoliticalEventCount: faction.dynasty?.politicalEvents?.active?.length ?? 0,
      successionCrisisActive: Boolean(activeSuccessionCrisis),
      covenantTestActive: Boolean(activeCovenantTest),
      territorialGovernanceRecognitionActive: Boolean(activeTerritorialGovernanceRecognition),
      successionCrisis: activeSuccessionCrisis
        ? serializeSuccessionCrisisEvent(state, factionId, activeSuccessionCrisis)
        : null,
      covenantTest: activeCovenantTest
        ? serializeCovenantTestEvent(state, factionId, activeCovenantTest)
        : null,
      activeTerritorialGovernanceRecognition: activeTerritorialGovernanceRecognition
        ? serializeTerritorialGovernanceRecognition(state, factionId, activeTerritorialGovernanceRecognition)
        : null,
      lastTerritorialGovernanceOutcome: faction.dynasty?.lastTerritorialGovernanceOutcome
        ? { ...faction.dynasty.lastTerritorialGovernanceOutcome }
        : null,
      politicalEffects: getFactionPoliticalEventEffects(state, factionId),
    },
    conviction: {
      score: Math.round(convictionScore * 10) / 10,
      bandId: convictionBandId,
      bandLabel: convictionBandLabel,
      buckets: {
        ruthlessness: Math.round((convictionBuckets.ruthlessness ?? 0) * 10) / 10,
        stewardship: Math.round((convictionBuckets.stewardship ?? 0) * 10) / 10,
        oathkeeping: Math.round((convictionBuckets.oathkeeping ?? 0) * 10) / 10,
        desecration: Math.round((convictionBuckets.desecration ?? 0) * 10) / 10,
      },
      topBucketKey: topBucket?.key ?? null,
      topBucketValue: topBucket ? Math.round(topBucket.value * 10) / 10 : 0,
      band: convictionBand,
      // Session 18: canonical conviction band effects surfaced for legibility.
      bandEffects: CONVICTION_BAND_EFFECTS[convictionBandId] ?? CONVICTION_BAND_EFFECTS.neutral,
    },
    logistics: {
      supplyCampCount: siegeState.supplyCampCount,
      supplyWagonCount: siegeState.supplyWagonCount,
      engineerCount: siegeState.engineerCount,
      engineCount: siegeState.engineCount,
      suppliedEngines: siegeState.suppliedEngines,
      unsuppliedEngines: siegeState.unsuppliedEngines,
      engineerSupportedEngines: siegeState.engineerSupportedEngines,
      fieldWaterUnitCount: fieldWaterState.unitCount,
      fieldWaterSupportedUnits: fieldWaterState.supportedUnits,
      fieldWaterStrainedUnits: fieldWaterState.strainedUnits,
      fieldWaterCriticalUnits: fieldWaterState.criticalUnits,
      fieldWaterAttritionUnits: fieldWaterState.attritionUnits,
      fieldWaterDesertionRiskUnits: fieldWaterState.desertionRiskUnits,
      fieldWaterSourceCount: fieldWaterState.sourceCount,
      interdictedSupplyWagonCount: interdictedSupplyWagons.length,
      convoyRecoveringCount: siegeState.convoyRecoveringCount,
      convoyScreenedCount: siegeState.convoyScreenedCount,
      escortedSupplyWagonCount: siegeState.escortedSupplyWagonCount,
      unscreenedRecoveringCount: siegeState.unscreenedRecoveringCount,
      convoyReconsolidated: siegeState.convoyReconsolidated,
      harassedResourceNodeCount: harassedResourceNodes.length,
      harassedWorkerCount,
      raidedInfrastructureCount: raidedInfrastructure.length,
      raidedSupplyCampCount,
      raidedWaterAnchorCount,
      raidedDropoffCount,
      formalSiegeReady: siegeState.readyForFormalAssault,
      band: logisticsBand,
    },
    worldPressure: {
      tribeUnitCount,
      contestedTerritories,
      activeOperations,
      heldCaptives,
      fallenMembers,
      signals: worldPressureSignals,
      band: worldPressureBand,
      score: worldPressureProfile.score,
      pressureLevel: worldPressureProfile.level,
      pressureLabel: worldPressureProfile.label,
      pressureStreak: worldPressureProfile.streak,
      topPressureSourceId: worldPressureProfile.topSourceId,
      topPressureSourceLabel: worldPressureProfile.topSourceLabel,
      pressureSourceBreakdown: { ...(worldPressureProfile.sources ?? {}) },
      territorialGovernanceRecognitionActive: Boolean(activeTerritorialGovernanceRecognition),
      divineRightDeclarationActive: Boolean(activeDivineRightDeclaration),
      incomingDivineRightDeclarationCount: incomingDivineRightDeclarations.length,
      targetedByWorld: worldPressureProfile.targeted,
      leaderFactionId: worldPressureProfile.leaderFactionId,
      leaderFactionName: worldPressureProfile.leaderFactionId ? getFactionDisplayName(state, worldPressureProfile.leaderFactionId) : null,
      leaderScore: worldPressureProfile.leaderScore,
      leaderLevel: worldPressureProfile.leaderLevel,
      leaderStreak: worldPressureProfile.leaderStreak,
      convergenceActive: convergenceProfile.active,
      rivalAttackTimerCap: convergenceProfile.attackTimerCap,
      rivalTerritoryTimerCap: convergenceProfile.territoryTimerCap,
      rivalSabotageTimerCap: convergenceProfile.sabotageTimerCap,
      rivalEspionageTimerCap: convergenceProfile.espionageTimerCap,
      rivalAssassinationTimerCap: convergenceProfile.assassinationTimerCap,
      rivalMissionaryTimerCap: convergenceProfile.missionaryTimerCap,
      rivalHolyWarTimerCap: convergenceProfile.holyWarTimerCap,
      tribalRaidTimerMultiplier: Math.round((convergenceProfile.tribalRaidTimerMultiplier ?? 1) * 100) / 100,
      frontierLoyaltyPenalty: worldPressureProfile.targeted ? worldPressureProfile.level : 0,
      legitimacyPressure: worldPressureProfile.targeted ? Math.max(0, worldPressureProfile.level - 1) : 0,
      greatReckoningActive: greatReckoning.active,
      greatReckoningTargeted,
      greatReckoningTargetFactionId: greatReckoning.targetFactionId,
      greatReckoningTargetFactionName: greatReckoningTargetName,
      greatReckoningShare: Math.round((greatReckoning.territoryShare ?? 0) * 1000) / 1000,
      greatReckoningThreshold: greatReckoning.threshold,
      cadetLoyaltyPenalty: Math.round((cadetWorldPressureProfile.delta ?? 0) * 100) / 100,
      cadetPressureStatus: cadetWorldPressureProfile.status,
      pressuredLesserHouseCount: cadetWorldPressureProfile.level > 0 ? cadetWorldPressureProfile.stressedCount : 0,
      splinterOpportunityCount: splinterOpportunityProfiles.length,
      splinterPressureStatus: splinterOpportunityLeader?.status ?? "quiet",
      splinterLevyTempo: Math.round((splinterOpportunityLeader?.levyTempoMultiplier ?? 1) * 100) / 100,
      splinterRetakeTempo: Math.round((splinterOpportunityLeader?.retakeTempoMultiplier ?? 1) * 100) / 100,
      splinterRetinueCapBonus: splinterOpportunityLeader?.retinueCapBonus ?? 0,
      hostilePostRepulseActive,
      hostilePostRepulseRemaining: Math.round(hostilePostRepulseRemaining * 10) / 10,
      hostileCohesionStrain: Math.round(hostileCohesionStrain * 10) / 10,
      // Session 19: dark-extremes world pressure — true when the snapshot faction
      // has been at Apex Cruel for 3+ consecutive realm cycles.
      darkExtremesActive: faction.darkExtremesActive ?? false,
      darkExtremesStreak: faction.darkExtremesStreak ?? 0,
      // Session 32: continental holdings legibility.
      continentalHoldings: state.world.controlPoints
        .filter((cp) => cp.ownerFactionId === factionId && cp.continentId)
        .reduce((acc, cp) => {
          const key = cp.continentId;
          acc[key] = (acc[key] ?? 0) + 1;
          return acc;
        }, {}),
      offHomeContinentHoldings: state.world.controlPoints.filter(
        (cp) => cp.ownerFactionId === factionId && cp.continentId && cp.continentId !== "home",
      ).length,
      // Session 88: governance alliance-threshold coalition pressure snapshot.
      governanceAlliancePressureActive: faction.governanceAlliancePressureActive ?? false,
      governanceAlliancePressureCycles: faction.governanceAlliancePressureCycles ?? 0,
      governanceAlliancePressureHostileCount: faction.governanceAlliancePressureHostileCount ?? 0,
      governanceAlliancePressureWeakestMarchName: faction.governanceAlliancePressureWeakestMarchName ?? null,
      governanceAlliancePressureWeakestMarchLoyalty: faction.governanceAlliancePressureWeakestMarchLoyalty ?? 0,
      // Session 94: Trueborn Rise arc state for HUD and dynasty panel.
      truebornRiseStage: state.factions.trueborn_city?.riseArc?.stage ?? 0,
      truebornRiseChallengeLevel: state.factions.trueborn_city?.riseArc?.challengeLevel ?? 0,
      truebornRiseUnchallengedCycles: state.factions.trueborn_city?.riseArc?.unchallengedCycles ?? 0,
      truebornRiseActivatedAtInWorldDays: state.factions.trueborn_city?.riseArc?.activatedAtInWorldDays ?? 0,
    },
  };
}
