import {
  acceptMarriage,
  attemptPlaceBuilding,
  chooseFaithCommitment,
  consolidateSuccessionCrisis,
  findOpenBuildingSite,
  getAvailablePopulation,
  getDivineRightDeclarationTerms,
  getDossierBackedSabotageProfile,
  getFactionSnapshot,
  getMatchProgressionSnapshot,
  getTrainableUnitIdsForBuilding,
  getMinorHousePressureOpportunityProfile,
  getWorldPressureConvergenceProfile,
  getWorldPressureLeaderProfile,
  getWorldPressureSourceBreakdown,
  getWorldPressureTargetProfile,
  getMarriageFaithCompatibilityProfile,
  getRealmConditionSnapshot,
  getSuccessionCrisisTerms,
  getSupplyWagonEscortCoverage,
  issueAttackCommand,
  issueGatherCommand,
  issueMoveCommand,
  issueRaidCommand,
  isSupplyWagonRecovering,
  performCovenantTestAction,
  promoteMemberToLesserHouse,
  proposeMarriage,
  queueProduction,
  startAssassinationOperation,
  startCounterIntelligenceOperation,
  startDivineRightDeclaration,
  startEspionageOperation,
  startHolyWarDeclaration,
  startMissionaryOperation,
  startRansomNegotiation,
  startRescueOperation,
  startSabotageOperation,
  getNonAggressionPactTerms,
  proposeNonAggressionPact,
} from "./simulation.js";

const AI_MARRIAGE_LEGITIMACY_DISTRESS_THRESHOLD = 50;

function findBuilding(state, factionId, typeId = null, options = {}) {
  const { completedOnly = false } = options;
  return state.buildings.find((building) =>
    building.factionId === factionId &&
    building.health > 0 &&
    (!completedOnly || building.completed) &&
    (typeId ? building.typeId === typeId : true),
  );
}

function getBuildingDef(state, typeId) {
  return state.content.byId.buildings[typeId];
}

function getUnitDef(state, typeId) {
  return state.content.byId.units[typeId];
}

function getFactionDisplayName(state, factionId) {
  const faction = state.factions[factionId];
  return state.content.byId.houses[faction?.houseId]?.name ?? factionId;
}

function areFactionsHostile(state, leftFactionId, rightFactionId) {
  if (!leftFactionId || !rightFactionId || leftFactionId === rightFactionId) {
    return false;
  }
  const left = state.factions[leftFactionId];
  const right = state.factions[rightFactionId];
  return Boolean((left?.hostileTo ?? []).includes(rightFactionId) || (right?.hostileTo ?? []).includes(leftFactionId));
}

function canAffordCost(resources, cost = {}) {
  return Object.entries(cost).every(([resourceId, amount]) => (resources[resourceId] ?? 0) >= amount);
}

function chooseBarracksUnit(state, factionId, barracksId, options = {}) {
  const { needsEscortMass = false, playerKeepFortified = false } = options;
  const faction = state.factions[factionId];
  if (!faction) {
    return null;
  }
  const availablePopulation = getAvailablePopulation(state, factionId);
  const trainableUnits = new Set(getTrainableUnitIdsForBuilding(state, barracksId));
  const canTrain = (unitId) => {
    const unitDef = trainableUnits.has(unitId) ? getUnitDef(state, unitId) : null;
    return Boolean(unitDef) &&
      availablePopulation >= (unitDef.populationCost ?? 0) &&
      canAffordCost(faction.resources, unitDef.cost);
  };

  const axemanAvailable = trainableUnits.has("axeman");
  const bloodLoad = faction.bloodProductionLoad ?? 0;
  const underBloodStrain = faction.houseId === "ironmark" && bloodLoad >= 8;
  if (faction.houseId === "ironmark" && axemanAvailable && needsEscortMass && !underBloodStrain && canTrain("axeman")) {
    return { unitId: "axeman", reason: "house_unit" };
  }
  if ((needsEscortMass || !playerKeepFortified) && canTrain("swordsman")) {
    return {
      unitId: "swordsman",
      reason: faction.houseId === "ironmark" && axemanAvailable && underBloodStrain ? "blood_strain" : "standard",
    };
  }
  if (canTrain("militia")) {
    return { unitId: "militia", reason: "fallback" };
  }
  return null;
}

function getCommittedFaithUnits(state, factionId, options = {}) {
  const faction = state.factions[factionId];
  const faithId = options.faithId ?? faction?.faith?.selectedFaithId ?? null;
  const doctrinePath = options.doctrinePath ?? faction?.faith?.doctrinePath ?? null;
  const stages = options.stages ?? null;
  return state.units.filter((unit) => {
    if (unit.factionId !== factionId || unit.health <= 0) {
      return false;
    }
    const unitDef = getUnitDef(state, unit.typeId);
    if (!unitDef?.faithId) {
      return false;
    }
    if (faithId && unitDef.faithId !== faithId) {
      return false;
    }
    if (doctrinePath && unitDef.doctrinePath && unitDef.doctrinePath !== doctrinePath) {
      return false;
    }
    if (stages && !stages.includes(unitDef.stage)) {
      return false;
    }
    return true;
  });
}

function getFaithSacredSite(state, faithId) {
  return (state.world.sacredSites ?? []).find((site) => site.faithId === faithId) ?? null;
}

function getFaithBuildAnchor(state, faithId, fallbackTileX, fallbackTileY, footprintPadding = 1) {
  const sacredSite = getFaithSacredSite(state, faithId);
  if (!sacredSite) {
    return { tileX: fallbackTileX, tileY: fallbackTileY };
  }
  return {
    tileX: Math.max(0, Math.floor(sacredSite.x) - footprintPadding),
    tileY: Math.max(0, Math.floor(sacredSite.y) - footprintPadding),
  };
}

function chooseFaithUnitForBuilding(state, factionId, buildingId, stages = []) {
  const faction = state.factions[factionId];
  if (!faction?.faith?.selectedFaithId) {
    return null;
  }
  const availablePopulation = getAvailablePopulation(state, factionId);
  const desiredStages = stages.length > 0 ? stages : [3, 4, 5];
  const candidates = getTrainableUnitIdsForBuilding(state, buildingId)
    .map((unitId) => ({ unitId, unitDef: getUnitDef(state, unitId) }))
    .filter(({ unitDef }) =>
      unitDef &&
      unitDef.faithId === faction.faith.selectedFaithId &&
      (!unitDef.doctrinePath || unitDef.doctrinePath === faction.faith.doctrinePath) &&
      desiredStages.includes(unitDef.stage) &&
      availablePopulation >= (unitDef.populationCost ?? 0) &&
      canAffordCost(faction.resources, unitDef.cost))
    .sort((left, right) =>
      (desiredStages.indexOf(left.unitDef.stage) - desiredStages.indexOf(right.unitDef.stage)) ||
      ((left.unitDef.populationCost ?? 0) - (right.unitDef.populationCost ?? 0)) ||
      left.unitId.localeCompare(right.unitId));
  return candidates[0]?.unitId ?? null;
}

function getIdleWorkers(state, factionId) {
  return state.units.filter((unit) =>
    unit.factionId === factionId &&
    unit.health > 0 &&
    state.content.byId.units[unit.typeId].role === "worker" &&
    (
      !unit.command ||
      (
        unit.command.type === "harass_retreat" &&
        (unit.command.resumeAt ?? 0) <= state.meta.elapsed
      )
    ),
  );
}

function getNearestNode(state, resourceType, fromX, fromY, factionId = null) {
  return state.world.resourceNodes
    .filter((node) =>
      node.type === resourceType &&
      node.amount > 0 &&
      (
        !factionId ||
        (node.harassedUntil ?? 0) <= state.meta.elapsed ||
        !node.harassedTargetFactionId ||
        node.harassedTargetFactionId !== factionId
      ))
    .sort((a, b) => {
      const ax = a.x * state.world.tileSize;
      const ay = a.y * state.world.tileSize;
      const bx = b.x * state.world.tileSize;
      const by = b.y * state.world.tileSize;
      return Math.hypot(fromX - ax, fromY - ay) - Math.hypot(fromX - bx, fromY - by);
    })[0] ?? null;
}

function getControlPointPosition(state, controlPoint) {
  return {
    x: controlPoint.x * state.world.tileSize,
    y: controlPoint.y * state.world.tileSize,
  };
}

function getBuildingCenter(state, building) {
  const def = getBuildingDef(state, building.typeId);
  return {
    x: (building.tileX + def.footprint.w / 2) * state.world.tileSize,
    y: (building.tileY + def.footprint.h / 2) * state.world.tileSize,
  };
}

function getCombatArmy(state, factionId) {
  return state.units.filter((unit) =>
    unit.factionId === factionId &&
    unit.health > 0 &&
    !["worker", "engineer-specialist", "support"].includes(state.content.byId.units[unit.typeId].role),
  );
}

function getEscortArmy(state, factionId) {
  return getCombatArmy(state, factionId).filter((unit) => !state.content.byId.units[unit.typeId]?.siegeClass);
}

function getSiegeArmy(state, factionId, siegeClass = null) {
  return getCombatArmy(state, factionId).filter((unit) => {
    const unitSiegeClass = state.content.byId.units[unit.typeId]?.siegeClass;
    return siegeClass ? unitSiegeClass === siegeClass : Boolean(unitSiegeClass);
  });
}

function getEngineerCorps(state, factionId) {
  return state.units.filter((unit) =>
    unit.factionId === factionId &&
    unit.health > 0 &&
    state.content.byId.units[unit.typeId].role === "engineer-specialist",
  );
}

function getSupplyWagons(state, factionId) {
  return state.units.filter((unit) =>
    unit.factionId === factionId &&
    unit.health > 0 &&
    state.content.byId.units[unit.typeId].role === "support",
  );
}

function isMovingLogisticsCarrier(state, unit) {
  return Boolean(state.content.byId.units[unit.typeId]?.movingLogisticsCarrier);
}

function isSupplyWagonInterdicted(state, unit) {
  return (unit?.logisticsInterdictedUntil ?? 0) > state.meta.elapsed;
}

function getLinkedSupplyCampForWagon(state, wagon) {
  if (!wagon || wagon.health <= 0 || !isMovingLogisticsCarrier(state, wagon)) {
    return null;
  }
  const wagonDef = getUnitDef(state, wagon.typeId);
  return state.buildings
    .filter((building) =>
      building.factionId === wagon.factionId &&
      building.health > 0 &&
      building.completed &&
      getBuildingDef(state, building.typeId)?.supportsSiegeLogistics &&
      (building.poisonedUntil ?? 0) <= state.meta.elapsed &&
      (building.raidedUntil ?? 0) <= state.meta.elapsed)
    .map((building) => {
      const buildingDef = getBuildingDef(state, building.typeId);
      const center = getBuildingCenter(state, building);
      const maxLinkRadius = Math.max(buildingDef.supplyLinkRadius ?? 0, wagonDef.campLinkRadius ?? 0);
      return {
        building,
        center,
        linked: Math.hypot(wagon.x - center.x, wagon.y - center.y) <= maxLinkRadius,
      };
    })
    .filter((entry) => entry.linked)
    .sort((left, right) =>
      Math.hypot(wagon.x - left.center.x, wagon.y - left.center.y) -
      Math.hypot(wagon.x - right.center.x, wagon.y - right.center.y))[0]?.building ?? null;
}

function getConvoyEscortPriority(state, wagon) {
  if (isSupplyWagonInterdicted(state, wagon)) {
    return 0;
  }
  if (isSupplyWagonRecovering(state, wagon)) {
    return 1;
  }
  return 2;
}

function getConvoyEscortFormationPoint(state, wagon, slotIndex = 0) {
  const linkedCamp = getLinkedSupplyCampForWagon(state, wagon);
  const linkedCampCenter = linkedCamp ? getBuildingCenter(state, linkedCamp) : null;
  const baseAngle = linkedCampCenter
    ? Math.atan2(wagon.y - linkedCampCenter.y, wagon.x - linkedCampCenter.x)
    : 0;
  const flankAngle = baseAngle + (slotIndex % 2 === 0 ? Math.PI / 2 : -Math.PI / 2);
  const radius = isSupplyWagonRecovering(state, wagon) || isSupplyWagonInterdicted(state, wagon)
    ? 44
    : 58;
  const recoveryBias = linkedCampCenter && (isSupplyWagonRecovering(state, wagon) || isSupplyWagonInterdicted(state, wagon))
    ? {
      x: (linkedCampCenter.x - wagon.x) * 0.22,
      y: (linkedCampCenter.y - wagon.y) * 0.22,
    }
    : { x: 0, y: 0 };
  return {
    x: wagon.x + Math.cos(flankAngle) * radius + recoveryBias.x,
    y: wagon.y + Math.sin(flankAngle) * radius + recoveryBias.y,
  };
}

function buildSupplyPatrolAssignments(state, factionId, supplyWagons, escortArmy) {
  const patrolWagons = [...supplyWagons]
    .filter((wagon) => wagon.health > 0)
    .sort((left, right) =>
      (getConvoyEscortPriority(state, left) - getConvoyEscortPriority(state, right)) ||
      (Math.hypot(left.x, left.y) - Math.hypot(right.x, right.y)));
  const patrolCapacity = Math.max(0, Math.min(Math.max(0, escortArmy.length - 3), patrolWagons.length * 2));
  if (patrolCapacity === 0 || patrolWagons.length === 0) {
    return { assignments: [], patrolCapacity };
  }
  const availableEscorts = escortArmy.filter((unit) =>
    unit.health > 0 && unit.factionId === factionId && unit.command?.type !== "raid");
  const assignments = [];
  const usedEscortIds = new Set();
  for (const slotIndex of [0, 1]) {
    for (const wagon of patrolWagons) {
      if (assignments.length >= patrolCapacity) {
        break;
      }
      const targetPoint = getConvoyEscortFormationPoint(state, wagon, slotIndex);
      const escort = availableEscorts
        .filter((unit) => !usedEscortIds.has(unit.id))
        .sort((left, right) =>
          Math.hypot(left.x - targetPoint.x, left.y - targetPoint.y) -
          Math.hypot(right.x - targetPoint.x, right.y - targetPoint.y))[0];
      if (!escort) {
        continue;
      }
      usedEscortIds.add(escort.id);
      assignments.push({
        unitId: escort.id,
        wagonId: wagon.id,
        slotIndex,
      });
    }
  }
  return { assignments, patrolCapacity };
}

function getStable(state, factionId, options = {}) {
  return findBuilding(state, factionId, "stable", options);
}

function getScoutRiders(state, factionId) {
  return state.units.filter((unit) =>
    unit.factionId === factionId &&
    unit.health > 0 &&
    (state.content.byId.units[unit.typeId]?.raidDurationSeconds ?? 0) > 0);
}

function isScoutHarassPositionSoft(state, raiderFactionId, targetFactionId, position, defenseRadius) {
  return !getCombatArmy(state, targetFactionId).some((unit) =>
    Math.hypot(unit.x - position.x, unit.y - position.y) <= defenseRadius &&
    unit.factionId !== raiderFactionId);
}

function getScoutRaidTargetPriority(typeId) {
  const priority = {
    supply_camp: 0,
    well: 1,
    iron_mine: 2,
    quarry: 3,
    farm: 4,
    lumber_camp: 5,
    mine_works: 6,
  };
  return priority[typeId] ?? 10;
}

function getScoutNodeHarassPriority(resourceType) {
  const priority = {
    iron: 0,
    gold: 1,
    stone: 2,
    wood: 3,
  };
  return priority[resourceType] ?? 10;
}

function pickScoutRaidTarget(state, factionId, anchor, targetFactionId = "player") {
  const scoutDef = getUnitDef(state, "scout_rider");
  const defenseRadius = scoutDef?.raidDefenseRadius ?? 136;
  const candidates = state.buildings
    .filter((building) =>
      building.factionId === targetFactionId &&
      building.health > 0 &&
      building.completed &&
      (building.raidedUntil ?? 0) <= state.meta.elapsed &&
      getBuildingDef(state, building.typeId)?.scoutRaidable)
    .map((building) => {
      const center = getBuildingCenter(state, building);
      const localControlPoint = state.world.controlPoints
        .filter((controlPoint) => controlPoint.ownerFactionId === targetFactionId)
        .map((controlPoint) => {
          const position = getControlPointPosition(state, controlPoint);
          return {
            controlPoint,
            distance: Math.hypot(center.x - position.x, center.y - position.y),
          };
        })
        .sort((left, right) => left.distance - right.distance)[0]?.controlPoint ?? null;
      return {
        building,
        center,
        priority: getScoutRaidTargetPriority(building.typeId),
        softTarget: isScoutHarassPositionSoft(state, factionId, building.factionId, center, defenseRadius),
        territoryLoyalty: localControlPoint?.loyalty ?? 100,
        distance: anchor ? Math.hypot(anchor.x - center.x, anchor.y - center.y) : 0,
      };
    });
  const softTargets = candidates.filter((candidate) => candidate.softTarget);
  const pool = softTargets.length > 0 ? softTargets : candidates;
  return pool
    .sort((left, right) =>
      (left.priority - right.priority) ||
      (left.territoryLoyalty - right.territoryLoyalty) ||
      (left.distance - right.distance))[0]?.building ?? null;
}

function pickScoutNodeHarassTarget(state, factionId, anchor, targetFactionId = "player") {
  const scoutDef = getUnitDef(state, "scout_rider");
  const defenseRadius = scoutDef?.raidDefenseRadius ?? 136;
  const candidates = (state.world.resourceNodes ?? [])
    .filter((resourceNode) => resourceNode.amount > 0)
    .map((resourceNode) => {
      const center = {
        x: resourceNode.x * state.world.tileSize,
        y: resourceNode.y * state.world.tileSize,
      };
      const pressuredWorkers = state.units.filter((unit) => {
        if (unit.health <= 0 || unit.factionId !== targetFactionId) {
          return false;
        }
        const unitDef = getUnitDef(state, unit.typeId);
        if (!unitDef || unitDef.role !== "worker") {
          return false;
        }
        if (unit.command?.type === "gather" && unit.command.nodeId === resourceNode.id) {
          return true;
        }
        return Math.hypot(unit.x - center.x, unit.y - center.y) <= 56;
      });
      if (pressuredWorkers.length === 0) {
        return null;
      }
      const localControlPoint = state.world.controlPoints
        .filter((controlPoint) => controlPoint.ownerFactionId === targetFactionId)
        .map((controlPoint) => {
          const position = getControlPointPosition(state, controlPoint);
          return {
            controlPoint,
            distance: Math.hypot(center.x - position.x, center.y - position.y),
          };
        })
        .sort((left, right) => left.distance - right.distance)[0]?.controlPoint ?? null;
      return {
        resourceNode,
        center,
        priority: getScoutNodeHarassPriority(resourceNode.type),
        workerCount: pressuredWorkers.length,
        softTarget: isScoutHarassPositionSoft(state, factionId, targetFactionId, center, defenseRadius),
        territoryLoyalty: localControlPoint?.loyalty ?? 100,
        distance: anchor ? Math.hypot(anchor.x - center.x, anchor.y - center.y) : 0,
      };
    })
    .filter(Boolean);
  const softTargets = candidates.filter((candidate) => candidate.softTarget);
  const pool = softTargets.length > 0 ? softTargets : candidates;
  return pool
    .sort((left, right) =>
      (right.workerCount - left.workerCount) ||
      (left.priority - right.priority) ||
      (left.territoryLoyalty - right.territoryLoyalty) ||
      (left.distance - right.distance))[0]?.resourceNode ?? null;
}

function pickScoutConvoyHarassTarget(state, factionId, anchor, targetFactionId = "player") {
  const scoutDef = getUnitDef(state, "scout_rider");
  const defenseRadius = scoutDef?.raidDefenseRadius ?? 136;
  const candidates = state.units
    .filter((unit) =>
      unit.health > 0 &&
      unit.factionId === targetFactionId &&
      isMovingLogisticsCarrier(state, unit) &&
      !isSupplyWagonInterdicted(state, unit))
    .map((wagon) => {
      const wagonDef = getUnitDef(state, wagon.typeId);
      const linkedCamp = getLinkedSupplyCampForWagon(state, wagon);
      if (!wagonDef || !linkedCamp) {
        return null;
      }
      const supplyRadius = wagonDef.supplyRadius ?? 0;
      const nearbyEngines = getSiegeArmy(state, targetFactionId).filter((unit) =>
        Math.hypot(unit.x - wagon.x, unit.y - wagon.y) <= supplyRadius).length;
      const nearbyFieldUnits = getCombatArmy(state, targetFactionId).filter((unit) => {
        const unitDef = getUnitDef(state, unit.typeId);
        if (!unitDef || unitDef.siegeClass) {
          return false;
        }
        return Math.hypot(unit.x - wagon.x, unit.y - wagon.y) <= supplyRadius;
      }).length;
      const localControlPoint = state.world.controlPoints
        .filter((controlPoint) => controlPoint.ownerFactionId === targetFactionId)
        .map((controlPoint) => {
          const position = getControlPointPosition(state, controlPoint);
          return {
            controlPoint,
            distance: Math.hypot(wagon.x - position.x, wagon.y - position.y),
          };
        })
        .sort((left, right) => left.distance - right.distance)[0]?.controlPoint ?? null;
      const campCenter = getBuildingCenter(state, linkedCamp);
      return {
        wagon,
        priority: 0,
        activeSupportCount: nearbyEngines + nearbyFieldUnits,
        engineCount: nearbyEngines,
        fieldUnitCount: nearbyFieldUnits,
        distanceFromCamp: Math.hypot(wagon.x - campCenter.x, wagon.y - campCenter.y),
        softTarget: isScoutHarassPositionSoft(state, factionId, targetFactionId, { x: wagon.x, y: wagon.y }, defenseRadius),
        territoryLoyalty: localControlPoint?.loyalty ?? 100,
        distance: anchor ? Math.hypot(anchor.x - wagon.x, anchor.y - wagon.y) : 0,
      };
    })
    .filter(Boolean);
  const softTargets = candidates.filter((candidate) => candidate.softTarget);
  const pool = softTargets.length > 0 ? softTargets : candidates;
  return pool
    .sort((left, right) =>
      (right.activeSupportCount - left.activeSupportCount) ||
      (right.engineCount - left.engineCount) ||
      (right.fieldUnitCount - left.fieldUnitCount) ||
      (right.distanceFromCamp - left.distanceFromCamp) ||
      (left.territoryLoyalty - right.territoryLoyalty) ||
      (left.distance - right.distance))[0]?.wagon ?? null;
}

function pickScoutHarassTarget(state, factionId, anchor, targetFactionId = "player") {
  const convoy = pickScoutConvoyHarassTarget(state, factionId, anchor, targetFactionId);
  if (convoy) {
    return { targetType: "unit", target: convoy };
  }
  const resourceNode = pickScoutNodeHarassTarget(state, factionId, anchor, targetFactionId);
  if (resourceNode) {
    return { targetType: "resource", target: resourceNode };
  }
  const building = pickScoutRaidTarget(state, factionId, anchor, targetFactionId);
  return building ? { targetType: "building", target: building } : null;
}

function getScoutHarassSites(state, factionId) {
  const convoySites = state.units
    .filter((unit) =>
      unit.factionId === factionId &&
      unit.health > 0 &&
      isMovingLogisticsCarrier(state, unit) &&
      isSupplyWagonInterdicted(state, unit))
    .map((unit) => ({
      type: "unit",
      entity: unit,
      x: unit.x,
      y: unit.y,
      severity: 0,
    }));
  const nodeSites = (state.world.resourceNodes ?? [])
    .filter((resourceNode) =>
      (resourceNode.harassedUntil ?? 0) > state.meta.elapsed &&
      resourceNode.harassedTargetFactionId === factionId)
    .map((resourceNode) => ({
      type: "resource",
      entity: resourceNode,
      x: resourceNode.x * state.world.tileSize,
      y: resourceNode.y * state.world.tileSize,
      severity: 0,
    }));
  const buildingSites = state.buildings
    .filter((building) =>
      building.factionId === factionId &&
      building.completed &&
      building.health > 0 &&
      (building.raidedUntil ?? 0) > state.meta.elapsed)
    .map((building) => {
      const center = getBuildingCenter(state, building);
      return {
        type: "building",
        entity: building,
        x: center.x,
        y: center.y,
        severity: building.typeId === "supply_camp" ? 0 : 1,
      };
    });
  return [...convoySites, ...nodeSites, ...buildingSites]
    .sort((left, right) => left.severity - right.severity);
}

function getNearestHostileScoutRider(state, factionId, site, radius = 240) {
  return state.units
    .filter((unit) =>
      unit.health > 0 &&
      areFactionsHostile(state, factionId, unit.factionId) &&
      (getUnitDef(state, unit.typeId)?.raidDurationSeconds ?? 0) > 0)
    .map((unit) => ({
      unit,
      distance: Math.hypot(unit.x - site.x, unit.y - site.y),
    }))
    .filter((entry) => entry.distance <= radius)
    .sort((left, right) => left.distance - right.distance)[0]?.unit ?? null;
}

function getOperationalSiegeForce(state, factionId) {
  return [
    ...getEscortArmy(state, factionId),
    ...getSiegeArmy(state, factionId),
    ...getEngineerCorps(state, factionId),
    ...getSupplyWagons(state, factionId),
  ];
}

function getSupplyCamp(state, factionId, options = {}) {
  return findBuilding(state, factionId, "supply_camp", options);
}

function getSuppliedSiegeEngines(state, factionId) {
  return getSiegeArmy(state, factionId).filter((unit) => (unit.siegeSuppliedUntil ?? 0) > state.meta.elapsed);
}

function getPrimaryKeepSeat(state, factionId) {
  return (state.world.settlements ?? []).find(
    (settlement) => settlement.factionId === factionId && settlement.settlementClass === "primary_dynastic_keep",
  ) ?? null;
}

function ensureMinorHouseAi(faction) {
  faction.ai ??= {};
  faction.ai.defenseTimer ??= 0;
  faction.ai.regroupTimer ??= 0;
  faction.ai.claimAlertUntil ??= 0;
  faction.ai.localDefenseStatus ??= "forming";
  faction.ai.levyProgress ??= 0;
  faction.ai.levyStatus ??= "forming";
  faction.ai.levySecondsRequired ??= 0;
  faction.ai.levyUnitId ??= "militia";
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
  faction.ai.lastAlertSecond ??= -1;
  faction.ai.lastRallySecond ??= -1;
  faction.ai.lastRetakeSecond ??= -1;
  return faction.ai;
}

function getMinorHouseClaim(state, factionId) {
  return state.world.controlPoints.find((controlPoint) => controlPoint.id === `${factionId}-claim`)
    ?? state.world.controlPoints.find((controlPoint) => controlPoint.ownerFactionId === factionId)
    ?? null;
}

function getNearestHostileCombatUnit(state, factionId, anchor, radius = 220) {
  if (!anchor) return null;
  return state.units
    .filter((unit) => {
      if (unit.factionId === factionId || unit.health <= 0) return false;
      const unitDef = state.content.byId.units[unit.typeId];
      if (!unitDef || unitDef.role === "worker") return false;
      return Math.hypot(unit.x - anchor.x, unit.y - anchor.y) <= radius;
    })
    .sort((left, right) =>
      Math.hypot(left.x - anchor.x, left.y - anchor.y) - Math.hypot(right.x - anchor.x, right.y - anchor.y))[0]
    ?? null;
}

function hasSiegeUnit(state, factionId) {
  // Only actual siege engines count as "has siege capability" for keep-assault gating.
  // Mantlet (role siege-support) is protection, not attack capability.
  return getSiegeArmy(state, factionId).some((unit) => {
    const unitDef = state.content.byId.units[unit.typeId];
    return unitDef?.role === "siege-engine";
  });
}

// Session 12: Longer-siege AI adaptation, first canonical layer — relief-window
// awareness. Detects whether the target faction (typically "player") has combat
// reinforcements approaching the besieger's stage point. When relief is inbound,
// the AI holds its final assault commit until relief is either neutralized or
// absorbed into the defending keep. Canonical reference: master doctrine section X
// (defensive fortification must be a major path to power; relief armies must
// materially affect siege pacing).
function isReliefArmyApproaching(state, targetFactionId, siegeStagePoint, reliefRadius = 380) {
  const combatUnits = state.units.filter((unit) => {
    if (unit.factionId !== targetFactionId || unit.health <= 0) return false;
    const unitDef = state.content.byId.units[unit.typeId];
    if (!unitDef || unitDef.role === "worker") return false;
    return true;
  });
  // A relief army is 3 or more combat units within reliefRadius of the siege stage
  // point. Excludes units already very close to the stage point (treated as
  // already-engaged garrison rather than approaching relief).
  let approaching = 0;
  for (const unit of combatUnits) {
    const d = Math.hypot(unit.x - siegeStagePoint.x, unit.y - siegeStagePoint.y);
    if (d < reliefRadius && d > 90) {
      approaching += 1;
    }
  }
  return approaching >= 3;
}

function pickTerritoryTarget(state, factionId, anchor, options = {}) {
  const { targetedFactionId = null, pressureSourceId = "quiet" } = options;
  const availableTargets = state.world.controlPoints
    .filter((controlPoint) => controlPoint.ownerFactionId !== factionId);
  let sourceFocusedTargets = [];
  if (targetedFactionId && pressureSourceId === "offHomeHoldings") {
    sourceFocusedTargets = availableTargets.filter((controlPoint) =>
      controlPoint.ownerFactionId === targetedFactionId && (controlPoint.continentId ?? "home") !== "home");
  } else if (targetedFactionId && ["territoryExpansion", "darkExtremes", "greatReckoning"].includes(pressureSourceId)) {
    sourceFocusedTargets = availableTargets.filter((controlPoint) => controlPoint.ownerFactionId === targetedFactionId);
  }
  const candidateTargets = sourceFocusedTargets.length > 0
    ? sourceFocusedTargets
    : availableTargets;
  return candidateTargets
    .sort((left, right) => {
      const leftPoint = getControlPointPosition(state, left);
      const rightPoint = getControlPointPosition(state, right);
      const leftDistance = Math.hypot(anchor.x - leftPoint.x, anchor.y - leftPoint.y);
      const rightDistance = Math.hypot(anchor.x - rightPoint.x, anchor.y - rightPoint.y);
      const leftDarkExtremesBias = targetedFactionId &&
        pressureSourceId === "darkExtremes" &&
        left.ownerFactionId === targetedFactionId
        ? -((100 - (left.loyalty ?? 100)) * 4 + (left.controlState !== "stabilized" ? 90 : 0))
        : 0;
      const rightDarkExtremesBias = targetedFactionId &&
        pressureSourceId === "darkExtremes" &&
        right.ownerFactionId === targetedFactionId
        ? -((100 - (right.loyalty ?? 100)) * 4 + (right.controlState !== "stabilized" ? 90 : 0))
        : 0;
      const leftTerritoryExpansionBias = targetedFactionId &&
        pressureSourceId === "territoryExpansion" &&
        left.ownerFactionId === targetedFactionId
        ? -((100 - (left.loyalty ?? 100)) * 8 +
          (left.controlState !== "stabilized" ? 170 : 0) +
          (left.contested ? 140 : 0))
        : 0;
      const rightTerritoryExpansionBias = targetedFactionId &&
        pressureSourceId === "territoryExpansion" &&
        right.ownerFactionId === targetedFactionId
        ? -((100 - (right.loyalty ?? 100)) * 8 +
          (right.controlState !== "stabilized" ? 170 : 0) +
          (right.contested ? 140 : 0))
        : 0;
      const leftGreatReckoningBias = targetedFactionId &&
        pressureSourceId === "greatReckoning" &&
        left.ownerFactionId === targetedFactionId
        ? -((100 - (left.loyalty ?? 100)) * 14 +
          (left.controlState !== "stabilized" ? 240 : 0) +
          (left.contested ? 220 : 0))
        : 0;
      const rightGreatReckoningBias = targetedFactionId &&
        pressureSourceId === "greatReckoning" &&
        right.ownerFactionId === targetedFactionId
        ? -((100 - (right.loyalty ?? 100)) * 14 +
          (right.controlState !== "stabilized" ? 240 : 0) +
          (right.contested ? 220 : 0))
        : 0;
      const leftBias = (left.ownerFactionId === "player" ? -140 : left.contested ? -70 : 0) +
        leftDarkExtremesBias +
        leftTerritoryExpansionBias +
        leftGreatReckoningBias;
      const rightBias = (right.ownerFactionId === "player" ? -140 : right.contested ? -70 : 0) +
        rightDarkExtremesBias +
        rightTerritoryExpansionBias +
        rightGreatReckoningBias;
      return (leftDistance + leftBias) - (rightDistance + rightBias);
    })[0] ?? null;
}

function getWorldPressureRaidTarget(state, factionId, anchor) {
  if (!factionId || !anchor) {
    return null;
  }
  const pressureBreakdown = getWorldPressureSourceBreakdown(state, factionId);
  const offHomeFocused = pressureBreakdown.topSourceId === "offHomeHoldings";
  const territoryExpansionFocused = pressureBreakdown.topSourceId === "territoryExpansion";
  const greatReckoningFocused = pressureBreakdown.topSourceId === "greatReckoning";
  const ownedControlPoints = state.world.controlPoints
    .filter((controlPoint) => controlPoint.ownerFactionId === factionId);
  const prioritizedControlPoints = offHomeFocused
    ? ownedControlPoints.filter((controlPoint) => (controlPoint.continentId ?? "home") !== "home")
    : ownedControlPoints;
  const candidateControlPoints = prioritizedControlPoints.length > 0
    ? prioritizedControlPoints
    : ownedControlPoints;
  return candidateControlPoints
    .sort((left, right) => {
      const leftPoint = getControlPointPosition(state, left);
      const rightPoint = getControlPointPosition(state, right);
      const leftDistance = Math.hypot(anchor.x - leftPoint.x, anchor.y - leftPoint.y);
      const rightDistance = Math.hypot(anchor.x - rightPoint.x, anchor.y - rightPoint.y);
      const leftBias = (100 - (left.loyalty ?? 100)) * 2 +
        (left.controlState !== "stabilized" ? 30 : 0) +
        (territoryExpansionFocused
          ? (100 - (left.loyalty ?? 100)) * 6 + (left.contested ? 140 : 0) + (left.controlState !== "stabilized" ? 170 : 0)
          : 0) +
        (greatReckoningFocused
          ? (100 - (left.loyalty ?? 100)) * 14 + (left.contested ? 240 : 0) + (left.controlState !== "stabilized" ? 280 : 0)
          : 0) +
        (offHomeFocused && left.continentId && left.continentId !== "home" ? 320 : 0);
      const rightBias = (100 - (right.loyalty ?? 100)) * 2 +
        (right.controlState !== "stabilized" ? 30 : 0) +
        (territoryExpansionFocused
          ? (100 - (right.loyalty ?? 100)) * 6 + (right.contested ? 140 : 0) + (right.controlState !== "stabilized" ? 170 : 0)
          : 0) +
        (greatReckoningFocused
          ? (100 - (right.loyalty ?? 100)) * 14 + (right.contested ? 240 : 0) + (right.controlState !== "stabilized" ? 280 : 0)
          : 0) +
        (offHomeFocused && right.continentId && right.continentId !== "home" ? 320 : 0);
      return (leftDistance - leftBias) - (rightDistance - rightBias);
    })[0] ?? null;
}

function getLiveIntelligenceReport(faction, targetFactionId, elapsed, sourceType = null) {
  if (!faction?.dynasty) {
    return null;
  }
  return (faction.dynasty.intelligenceReports ?? []).find((report) =>
    report.targetFactionId === targetFactionId &&
    (report.expiresAt ?? 0) > elapsed &&
    (!sourceType || (report.sourceType ?? "espionage") === sourceType)) ?? null;
}

function addResourcePriority(priorities, resourceId) {
  if (!priorities.includes(resourceId)) {
    priorities.push(resourceId);
  }
}

function addShortfallPriorities(priorities, resources, cost = {}) {
  ["gold", "wood", "stone", "iron"].forEach((resourceId) => {
    if ((cost[resourceId] ?? 0) > 0 && (resources[resourceId] ?? 0) < cost[resourceId]) {
      addResourcePriority(priorities, resourceId);
    }
  });
}

function getEnemyGatherPriorities(state, resources, context) {
  const priorities = [];
  const ramDef = getUnitDef(state, "ram");
  const towerDef = getUnitDef(state, "siege_tower");
  const trebuchetDef = getUnitDef(state, "trebuchet");
  const engineerDef = getUnitDef(state, "siege_engineer");
  const supplyWagonDef = getUnitDef(state, "supply_wagon");

  if (context.playerKeepFortified) {
    if (!context.barracks) {
      addShortfallPriorities(priorities, resources, getBuildingDef(state, "barracks")?.cost);
    } else if (!context.quarry) {
      addShortfallPriorities(priorities, resources, getBuildingDef(state, "quarry")?.cost);
    } else if (!context.ironMine) {
      addShortfallPriorities(priorities, resources, getBuildingDef(state, "iron_mine")?.cost);
    } else if (!context.siegeWorkshop) {
      addShortfallPriorities(priorities, resources, getBuildingDef(state, "siege_workshop")?.cost);
    } else if (!context.supplyCamp) {
      addShortfallPriorities(priorities, resources, getBuildingDef(state, "supply_camp")?.cost);
    } else if (context.engineerCount < 1) {
      addShortfallPriorities(priorities, resources, engineerDef?.cost);
    } else if (context.supplyWagonCount < 1) {
      addShortfallPriorities(priorities, resources, supplyWagonDef?.cost);
    } else if (!context.stable) {
      addShortfallPriorities(priorities, resources, getBuildingDef(state, "stable")?.cost);
    } else if (!context.enemyHasSiege) {
      addShortfallPriorities(priorities, resources, {
        gold: Math.max(ramDef?.cost?.gold ?? 0, trebuchetDef?.cost?.gold ?? 0),
        wood: Math.max(ramDef?.cost?.wood ?? 0, towerDef?.cost?.wood ?? 0),
        stone: Math.max(towerDef?.cost?.stone ?? 0, trebuchetDef?.cost?.stone ?? 0),
        iron: Math.max(ramDef?.cost?.iron ?? 0, trebuchetDef?.cost?.iron ?? 0),
      });
    }
  }

  ["gold", "wood", "stone", "iron"].forEach((resourceId) => addResourcePriority(priorities, resourceId));
  return priorities;
}

function chooseGatherNode(state, worker, preferredResources) {
  for (const resourceType of preferredResources) {
    const node = getNearestNode(state, resourceType, worker.x, worker.y, worker.factionId);
    if (node) {
      return node;
    }
  }
  return getNearestNode(state, "gold", worker.x, worker.y, worker.factionId)
    ?? getNearestNode(state, "wood", worker.x, worker.y, worker.factionId);
}

function getSiegeStagePoint(state, targetBuilding, sourceBuilding, distanceFromTarget = 220) {
  const targetCenter = getBuildingCenter(state, targetBuilding);
  const sourceCenter = getBuildingCenter(state, sourceBuilding);
  const dx = sourceCenter.x - targetCenter.x;
  const dy = sourceCenter.y - targetCenter.y;
  const length = Math.hypot(dx, dy) || 1;
  return {
    x: targetCenter.x + (dx / length) * distanceFromTarget,
    y: targetCenter.y + (dy / length) * distanceFromTarget,
  };
}

function areSiegeLinesFormed(units, stagePoint, radius) {
  return units.length > 0 && units.every((unit) => Math.hypot(unit.x - stagePoint.x, unit.y - stagePoint.y) <= radius);
}

export function updateEnemyAi(state, dt) {
  const enemy = state.factions.enemy;
  if (!enemy || state.meta.status !== "playing") {
    return;
  }

  enemy.ai.raidTimer ??= 18;
  enemy.ai.lastScoutRaidSecond ??= -1;
  enemy.ai.lastCounterRaidSecond ??= -1;
  enemy.ai.attackTimer -= dt;
  enemy.ai.buildTimer -= dt;
  enemy.ai.territoryTimer -= dt;
  enemy.ai.raidTimer -= dt;
  enemy.ai.lastHouseUnitSecond ??= -1;
  enemy.ai.lastPressureTerritorySecond ??= -1;
  enemy.ai.lastFaithBacklashSecond ??= -1;
  enemy.ai.lastCovertBacklashSecond ??= -1;
  enemy.ai.lastDarkBacklashSecond ??= -1;
  enemy.ai.lastCaptiveBacklashSecond ??= -1;
  enemy.ai.lastCovenantActionSecond ??= -1;
  enemy.ai.lastCovenantTerritorySecond ??= -1;
  enemy.ai.lastGovernanceTerritorySecond ??= -1;
  enemy.ai.lastFaithAdvanceSecond ??= -1;

  const hall = findBuilding(state, "enemy", "command_hall");
  const playerHall = findBuilding(state, "player", "command_hall");
  const snapshot = getFactionSnapshot(state, "enemy");
  const playerSnapshot = getFactionSnapshot(state, "player");
  const playerRealmSnapshot = getRealmConditionSnapshot(state, "player");
  const playerActiveCovenantTest = playerSnapshot.faith?.activeCovenantTest ?? null;
  const enemyActiveCovenantTest = snapshot.faith?.activeCovenantTest ?? null;
  const playerActiveTerritorialGovernanceRecognition = playerSnapshot.dynasty?.activeTerritorialGovernanceRecognition ?? null;
  const enemyActiveTerritorialGovernanceRecognition = snapshot.dynasty?.activeTerritorialGovernanceRecognition ?? null;
  const enemyFaithId = snapshot.faith?.selectedFaithId ?? null;
  const enemyDoctrinePath = snapshot.faith?.doctrinePath ?? null;
  const playerWorldPressure = getWorldPressureTargetProfile(state, "player");
  const playerWorldPressureLevel = playerWorldPressure.targeted ? playerWorldPressure.level : 0;
  const playerVerdantWardenDefenseCount = playerRealmSnapshot?.fortification?.verdantWardenCount ?? 0;
  const territoryExpansionSourceFocused = playerWorldPressure.targeted && playerWorldPressure.topSourceId === "territoryExpansion";
  const holyWarSourceFocused = playerWorldPressure.targeted && playerWorldPressure.topSourceId === "holyWar";
  const hostileOperationsSourceFocused = playerWorldPressure.targeted && playerWorldPressure.topSourceId === "hostileOperations";
  const darkExtremesSourceFocused = playerWorldPressure.targeted && playerWorldPressure.topSourceId === "darkExtremes";
  const captivesSourceFocused = playerWorldPressure.targeted && playerWorldPressure.topSourceId === "captives";
  const greatReckoningSourceFocused = playerWorldPressure.targeted && playerWorldPressure.topSourceId === "greatReckoning";
  const playerConvergencePressure = getWorldPressureConvergenceProfile(state, "player");
  const liveIntelOnPlayer = getLiveIntelligenceReport(enemy, "player", state.meta.elapsed);
  const liveCounterIntelDossierOnPlayer = getLiveIntelligenceReport(
    enemy,
    "player",
    state.meta.elapsed,
    "counter_intelligence",
  );
  const barracks = findBuilding(state, "enemy", "barracks");
  const quarry = findBuilding(state, "enemy", "quarry");
  const ironMine = findBuilding(state, "enemy", "iron_mine");
  const siegeWorkshop = findBuilding(state, "enemy", "siege_workshop");
  const siegeWorkshopCompleted = findBuilding(state, "enemy", "siege_workshop", { completedOnly: true });
  const supplyCamp = getSupplyCamp(state, "enemy");
  const supplyCampCompleted = getSupplyCamp(state, "enemy", { completedOnly: true });
  const stable = getStable(state, "enemy");
  const stableCompleted = getStable(state, "enemy", { completedOnly: true });
  const wayshrine = findBuilding(state, "enemy", "wayshrine");
  const wayshrineCompleted = findBuilding(state, "enemy", "wayshrine", { completedOnly: true });
  const covenantHall = findBuilding(state, "enemy", "covenant_hall");
  const covenantHallCompleted = findBuilding(state, "enemy", "covenant_hall", { completedOnly: true });
  const grandSanctuary = findBuilding(state, "enemy", "grand_sanctuary");
  const grandSanctuaryCompleted = findBuilding(state, "enemy", "grand_sanctuary", { completedOnly: true });
  const apexCovenant = findBuilding(state, "enemy", "apex_covenant");
  const apexCovenantCompleted = findBuilding(state, "enemy", "apex_covenant", { completedOnly: true });
  const houses = state.buildings.filter((building) => building.factionId === "enemy" && building.typeId === "dwelling" && building.health > 0);
  const farms = state.buildings.filter((building) => building.factionId === "enemy" && building.typeId === "farm" && building.health > 0);
  const wells = state.buildings.filter((building) => building.factionId === "enemy" && building.typeId === "well" && building.health > 0);
  const army = getCombatArmy(state, "enemy");
  const escortArmy = getEscortArmy(state, "enemy");
  const operationalSiegeForce = getOperationalSiegeForce(state, "enemy");
  const engineerCorps = getEngineerCorps(state, "enemy");
  const supplyWagons = getSupplyWagons(state, "enemy");
  const interdictedSupplyWagons = supplyWagons.filter((unit) => isSupplyWagonInterdicted(state, unit));
  const recoveringSupplyWagons = supplyWagons.filter((unit) => isSupplyWagonRecovering(state, unit));
  const screenedRecoveringSupplyWagons = recoveringSupplyWagons.filter((unit) =>
    getSupplyWagonEscortCoverage(state, "enemy", unit, escortArmy).screened);
  const convoyRecoveringUnscreenedCount = Math.max(0, recoveringSupplyWagons.length - screenedRecoveringSupplyWagons.length);
  const scoutRiders = getScoutRiders(state, "enemy");
  const enemyFaithUnits = getCommittedFaithUnits(state, "enemy", { faithId: enemyFaithId, doctrinePath: enemyDoctrinePath });
  const enemyStageThreeFaithUnits = getCommittedFaithUnits(state, "enemy", { faithId: enemyFaithId, doctrinePath: enemyDoctrinePath, stages: [3] });
  const enemyStageFourFaithUnits = getCommittedFaithUnits(state, "enemy", { faithId: enemyFaithId, doctrinePath: enemyDoctrinePath, stages: [4] });
  const enemyApexFaithUnits = state.units.filter((unit) =>
    unit.factionId === "enemy" &&
    unit.health > 0 &&
    getUnitDef(state, unit.typeId)?.role === "faith-apex");
  const activeHarassSites = getScoutHarassSites(state, "enemy");
  const playerHarassableConvoys = state.units.filter((unit) =>
    unit.factionId === "player" &&
    unit.health > 0 &&
    isMovingLogisticsCarrier(state, unit) &&
    !isSupplyWagonInterdicted(state, unit) &&
    Boolean(getLinkedSupplyCampForWagon(state, unit)));
  const playerRaidableStructures = state.buildings.filter((building) =>
    building.factionId === "player" &&
    building.health > 0 &&
    building.completed &&
    getBuildingDef(state, building.typeId)?.scoutRaidable);
  const playerHarassableNodes = (state.world.resourceNodes ?? []).filter((resourceNode) =>
    resourceNode.amount > 0 &&
    state.units.some((unit) =>
      unit.health > 0 &&
      unit.factionId === "player" &&
      getUnitDef(state, unit.typeId)?.role === "worker" &&
      (
        (unit.command?.type === "gather" && unit.command.nodeId === resourceNode.id) ||
        Math.hypot(unit.x - (resourceNode.x * state.world.tileSize), unit.y - (resourceNode.y * state.world.tileSize)) <= 56
      )));
  const playerKeep = getPrimaryKeepSeat(state, "player");
  const playerKeepFortified = (playerKeep?.fortificationTier ?? 0) > 0;
  const enemyHasSiege = hasSiegeUnit(state, "enemy");
  const suppliedSiegeArmy = getSuppliedSiegeEngines(state, "enemy");
  const idleWorkers = getIdleWorkers(state, "enemy");
  const gatherPriorities = getEnemyGatherPriorities(state, snapshot.resources, {
    barracks,
    quarry,
    ironMine,
      siegeWorkshop,
      supplyCamp,
      stable,
      engineerCount: engineerCorps.length,
      supplyWagonCount: supplyWagons.length,
      enemyHasSiege,
    playerKeepFortified,
  });
  const enemyHolyWarOnPlayer = (enemy.faith?.activeHolyWars ?? []).some((entry) =>
    entry.targetFactionId === "player" && (entry.expiresAt ?? 0) > state.meta.elapsed);
  const playerHolyWarOnEnemy = (state.factions.player.faith?.activeHolyWars ?? []).some((entry) =>
    entry.targetFactionId === "enemy" && (entry.expiresAt ?? 0) > state.meta.elapsed);
  const playerDivineRightActive = Boolean(
    state.factions.player.faith?.divineRightDeclaration &&
    (state.factions.player.faith.divineRightDeclaration.resolveAt ?? 0) > state.meta.elapsed &&
    !state.factions.player.faith.divineRightDeclaration.failedAt &&
    !state.factions.player.faith.divineRightDeclaration.completedAt,
  );
  const playerSuccessionCrisis = playerSnapshot.dynasty?.activeSuccessionCrisis ?? null;
  const enemySuccessionCrisis = snapshot.dynasty?.activeSuccessionCrisis ?? null;
  const matchProgression = getMatchProgressionSnapshot(state);
  const matchStageNumber = matchProgression.stageNumber ?? 1;
  const successionPressureOverride = Boolean(playerSuccessionCrisis);
  const rivalryPressureOverride = playerWorldPressureLevel > 0 ||
    darkExtremesSourceFocused ||
    greatReckoningSourceFocused ||
    playerConvergencePressure.active ||
    enemyHolyWarOnPlayer ||
    playerHolyWarOnEnemy ||
    successionPressureOverride;
  const raidPressureOverride = rivalryPressureOverride || hostileOperationsSourceFocused;
  const rivalryUnlocked = matchStageNumber >= 2 || rivalryPressureOverride;
  const raidPressureUnlocked = matchStageNumber >= 3 || raidPressureOverride;
  const enemyCovenantTargetPoint = enemyActiveCovenantTest?.targetControlPointId
    ? state.world.controlPoints.find((controlPoint) => controlPoint.id === enemyActiveCovenantTest.targetControlPointId) ?? null
    : null;
  const playerCovenantTargetsEnemy = Boolean(
    playerActiveCovenantTest &&
    (
      playerActiveCovenantTest.targetFactionId === "enemy" ||
      (
        playerActiveCovenantTest.targetControlPointId &&
        state.world.controlPoints.some((controlPoint) =>
          controlPoint.id === playerActiveCovenantTest.targetControlPointId &&
          controlPoint.ownerFactionId === "enemy")
      )
    ),
  );
  const playerGovernanceTargetPoint = playerActiveTerritorialGovernanceRecognition?.targetControlPointId
    ? state.world.controlPoints.find((controlPoint) =>
      controlPoint.id === playerActiveTerritorialGovernanceRecognition.targetControlPointId) ?? null
    : null;
  const enemyGovernanceTargetPoint = enemyActiveTerritorialGovernanceRecognition?.targetControlPointId
    ? state.world.controlPoints.find((controlPoint) =>
      controlPoint.id === enemyActiveTerritorialGovernanceRecognition.targetControlPointId) ?? null
    : null;

  if (enemyHolyWarOnPlayer || playerHolyWarOnEnemy) {
    enemy.ai.attackTimer = Math.min(enemy.ai.attackTimer, 10);
    enemy.ai.territoryTimer = Math.min(enemy.ai.territoryTimer, 8);
  }
  if (playerActiveTerritorialGovernanceRecognition) {
    const governanceVictoryPressure = Boolean(
      playerActiveTerritorialGovernanceRecognition.victoryReady && !playerActiveTerritorialGovernanceRecognition.completed,
    );
    const governanceAllianceThresholdPressure = !governanceVictoryPressure && Boolean(
      (playerActiveTerritorialGovernanceRecognition.integrationReady ||
        (playerActiveTerritorialGovernanceRecognition.populationAcceptancePct ?? 0) >=
          (playerActiveTerritorialGovernanceRecognition.populationAcceptanceAllianceThresholdPct ?? 60)) &&
      !playerActiveTerritorialGovernanceRecognition.completed,
    );
    enemy.ai.attackTimer = Math.min(enemy.ai.attackTimer, governanceVictoryPressure ? 3 : governanceAllianceThresholdPressure ? 4 : 5);
    enemy.ai.territoryTimer = Math.min(enemy.ai.territoryTimer, governanceVictoryPressure ? 2 : governanceAllianceThresholdPressure ? 3 : 4);
    enemy.ai.raidTimer = Math.min(enemy.ai.raidTimer ?? 95, governanceVictoryPressure ? 4 : governanceAllianceThresholdPressure ? 5 : 6);
    enemy.ai.assassinationTimer = Math.min(enemy.ai.assassinationTimer ?? 80, governanceVictoryPressure ? 3 : governanceAllianceThresholdPressure ? 4 : 6);
    enemy.ai.missionaryTimer = Math.min(enemy.ai.missionaryTimer ?? 70, governanceVictoryPressure ? 3 : governanceAllianceThresholdPressure ? 4 : 5);
    enemy.ai.holyWarTimer = Math.min(enemy.ai.holyWarTimer ?? 95, governanceVictoryPressure ? 4 : governanceAllianceThresholdPressure ? 5 : 6);
  }
  if (playerActiveCovenantTest) {
    enemy.ai.attackTimer = Math.min(enemy.ai.attackTimer, playerCovenantTargetsEnemy ? 4 : 6);
    enemy.ai.territoryTimer = Math.min(enemy.ai.territoryTimer, playerCovenantTargetsEnemy ? 4 : 6);
    enemy.ai.raidTimer = Math.min(enemy.ai.raidTimer ?? 95, playerCovenantTargetsEnemy ? 5 : 7);
    enemy.ai.holyWarTimer = Math.min(enemy.ai.holyWarTimer ?? 95, playerCovenantTargetsEnemy ? 6 : 12);
  }
  if (playerDivineRightActive) {
    enemy.ai.attackTimer = Math.min(enemy.ai.attackTimer, 5);
    enemy.ai.territoryTimer = Math.min(enemy.ai.territoryTimer, 4);
    enemy.ai.raidTimer = Math.min(enemy.ai.raidTimer ?? 95, 7);
  }
  if (playerSuccessionCrisis) {
    const highSeveritySuccession = ["major", "catastrophic"].includes(playerSuccessionCrisis.severityId);
    enemy.ai.attackTimer = Math.min(enemy.ai.attackTimer, highSeveritySuccession ? 5 : 7);
    enemy.ai.territoryTimer = Math.min(enemy.ai.territoryTimer, highSeveritySuccession ? 4 : 6);
    enemy.ai.marriageProposalTimer = Math.min(enemy.ai.marriageProposalTimer ?? 90, 18);
  }
  if (enemySuccessionCrisis) {
    const severeEnemyCrisis = ["major", "catastrophic"].includes(enemySuccessionCrisis.severityId);
    enemy.ai.attackTimer = Math.max(enemy.ai.attackTimer, severeEnemyCrisis ? 16 : 12);
    enemy.ai.territoryTimer = Math.max(enemy.ai.territoryTimer, severeEnemyCrisis ? 14 : 10);
    enemy.ai.marriageProposalTimer = Math.min(enemy.ai.marriageProposalTimer ?? 90, 14);
    enemy.ai.successionCrisisTimer ??= 12;
    enemy.ai.successionCrisisTimer -= dt;
    if (enemy.ai.successionCrisisTimer <= 0) {
      const successionTerms = getSuccessionCrisisTerms(state, "enemy");
      if (successionTerms.available) {
        consolidateSuccessionCrisis(state, "enemy");
        enemy.ai.successionCrisisTimer = 60;
      } else {
        enemy.ai.successionCrisisTimer = 18;
      }
    }
  } else {
    enemy.ai.successionCrisisTimer = 12;
  }
  if (enemyActiveCovenantTest) {
    enemy.ai.buildTimer = Math.min(enemy.ai.buildTimer, 2.5);
    enemy.ai.territoryTimer = Math.min(enemy.ai.territoryTimer, enemyCovenantTargetPoint ? 5 : 8);
    if (enemyActiveCovenantTest.targetFactionId === "player") {
      enemy.ai.attackTimer = Math.min(enemy.ai.attackTimer, 5);
      enemy.ai.raidTimer = Math.min(enemy.ai.raidTimer ?? 95, 6);
      enemy.ai.holyWarTimer = Math.min(enemy.ai.holyWarTimer ?? 95, 8);
    }
    if (enemyActiveCovenantTest.mandateId === "old_light_dark_purge") {
      enemy.ai.holyWarTimer = Math.min(enemy.ai.holyWarTimer ?? 95, 4);
    }
  }
  if (enemyActiveTerritorialGovernanceRecognition) {
    const enemyGovernanceVictoryPressure = Boolean(
      enemyActiveTerritorialGovernanceRecognition.victoryReady && !enemyActiveTerritorialGovernanceRecognition.completed,
    );
    const enemyGovernanceAllianceThresholdPressure = !enemyGovernanceVictoryPressure && Boolean(
      (enemyActiveTerritorialGovernanceRecognition.integrationReady ||
        (enemyActiveTerritorialGovernanceRecognition.populationAcceptancePct ?? 0) >=
          (enemyActiveTerritorialGovernanceRecognition.populationAcceptanceAllianceThresholdPct ?? 60)) &&
      !enemyActiveTerritorialGovernanceRecognition.completed,
    );
    enemy.ai.buildTimer = Math.min(enemy.ai.buildTimer, 2.5);
    enemy.ai.territoryTimer = Math.min(
      enemy.ai.territoryTimer,
      enemyGovernanceTargetPoint ? (enemyGovernanceVictoryPressure ? 3 : enemyGovernanceAllianceThresholdPressure ? 4 : 5) : 8,
    );
    enemy.ai.attackTimer = Math.max(enemy.ai.attackTimer, enemyGovernanceVictoryPressure ? 14 : enemyGovernanceAllianceThresholdPressure ? 13 : 12);
    enemy.ai.holyWarTimer = Math.max(enemy.ai.holyWarTimer ?? 95, enemyGovernanceVictoryPressure ? 24 : enemyGovernanceAllianceThresholdPressure ? 20 : 18);
  }

  if (playerWorldPressureLevel > 0) {
    enemy.ai.attackTimer = Math.min(enemy.ai.attackTimer, playerWorldPressureLevel >= 2 ? 8 : 14);
    enemy.ai.territoryTimer = Math.min(enemy.ai.territoryTimer, playerWorldPressureLevel >= 2 ? 6 : 10);
  }
  if (darkExtremesSourceFocused) {
    enemy.ai.attackTimer = Math.min(enemy.ai.attackTimer, 7);
    enemy.ai.territoryTimer = Math.min(enemy.ai.territoryTimer, 5);
  }
  if (greatReckoningSourceFocused) {
    enemy.ai.attackTimer = Math.min(enemy.ai.attackTimer, 6);
    enemy.ai.territoryTimer = Math.min(enemy.ai.territoryTimer, 4);
    enemy.ai.raidTimer = Math.min(enemy.ai.raidTimer ?? 95, 8);
  }
  if (playerConvergencePressure.active) {
    enemy.ai.attackTimer = Math.min(enemy.ai.attackTimer, playerConvergencePressure.attackTimerCap ?? enemy.ai.attackTimer);
    enemy.ai.territoryTimer = Math.min(enemy.ai.territoryTimer, playerConvergencePressure.territoryTimerCap ?? enemy.ai.territoryTimer);
  }
  if (!rivalryUnlocked) {
    enemy.ai.territoryTimer = Math.max(enemy.ai.territoryTimer, 24);
  } else if (!rivalryPressureOverride && matchStageNumber === 2) {
    enemy.ai.territoryTimer = Math.max(enemy.ai.territoryTimer, 16);
  }
  if (!raidPressureUnlocked) {
    enemy.ai.raidTimer = Math.max(enemy.ai.raidTimer, 20);
  }

  idleWorkers.forEach((worker, index) => {
    const rotatedPriorities = gatherPriorities
      .slice(index % gatherPriorities.length)
      .concat(gatherPriorities.slice(0, index % gatherPriorities.length));
    const node = chooseGatherNode(state, worker, rotatedPriorities);
    if (node) {
      issueGatherCommand(state, [worker.id], node.id);
    }
  });

  if (!enemy.faith.selectedFaithId) {
    const availableFaith = snapshot.faith.exposures
      .filter((faith) => faith.availableToCommit)
      .sort((left, right) => right.exposure - left.exposure)[0];
    if (availableFaith) {
      chooseFaithCommitment(state, "enemy", availableFaith.id);
    }
  }

  if (enemyActiveCovenantTest?.actionAvailable) {
    const currentSecond = Math.floor(state.meta.elapsed);
    if (enemy.ai.lastCovenantActionSecond !== currentSecond) {
      const result = performCovenantTestAction(state, "enemy");
      if (result.ok) {
        enemy.ai.lastCovenantActionSecond = currentSecond;
        enemy.ai.buildTimer = Math.min(enemy.ai.buildTimer, 2);
      }
    }
  }

  if (activeHarassSites.length > 0) {
    const responseSite = activeHarassSites[0];
    const localDefenders = army
      .filter((unit) => {
        const unitDef = getUnitDef(state, unit.typeId);
        if (!unitDef || unitDef.siegeClass) {
          return false;
        }
        return Math.hypot(unit.x - responseSite.x, unit.y - responseSite.y) <= 280;
      })
      .sort((left, right) =>
        Math.hypot(left.x - responseSite.x, left.y - responseSite.y) -
        Math.hypot(right.x - responseSite.x, right.y - responseSite.y))
      .slice(0, 2);
    const hostileScout = getNearestHostileScoutRider(state, "enemy", responseSite);
    if (localDefenders.length > 0) {
      if (hostileScout) {
        issueAttackCommand(state, localDefenders.map((unit) => unit.id), "unit", hostileScout.id);
      } else {
        issueMoveCommand(state, localDefenders.map((unit) => unit.id), responseSite.x, responseSite.y);
      }
      const currentSecond = Math.floor(state.meta.elapsed);
      if (enemy.ai.lastCounterRaidSecond !== currentSecond) {
        state.messages.unshift({
          id: `enemy-counter-raid-${currentSecond}`,
          text: responseSite.type === "resource"
            ? "Stonehelm throws local defenders toward the harried seam."
            : "Stonehelm rallies a local counter-raid around the struck logistics line.",
          tone: "warn",
          ttl: 7,
        });
        enemy.ai.lastCounterRaidSecond = currentSecond;
      }
    }
  }

  const availableScoutRaiders = scoutRiders.filter((unit) => !unit.command || unit.command.type === "move");
  if (
    raidPressureUnlocked &&
    enemy.ai.raidTimer <= 0 &&
    availableScoutRaiders.length > 0 &&
    (playerHarassableConvoys.length > 0 || playerRaidableStructures.length > 0 || playerHarassableNodes.length > 0)
  ) {
    const anchor = availableScoutRaiders[0]
      ? { x: availableScoutRaiders[0].x, y: availableScoutRaiders[0].y }
      : hall
        ? getBuildingCenter(state, hall)
        : null;
    const raidTarget = pickScoutHarassTarget(state, "enemy", anchor, "player");
    if (raidTarget) {
      const raidResult = issueRaidCommand(
        state,
        availableScoutRaiders.map((unit) => unit.id),
        raidTarget.targetType,
        raidTarget.target.id,
      );
      if (raidResult.ok) {
        const currentSecond = Math.floor(state.meta.elapsed);
        if (enemy.ai.lastScoutRaidSecond !== currentSecond) {
          state.messages.unshift({
            id: `enemy-scout-raid-${currentSecond}`,
            text: raidTarget.targetType === "resource"
              ? `Stonehelm scout riders break for the ${raidTarget.target.type} seam and nearby marches.`
              : raidTarget.targetType === "unit"
                ? `Stonehelm scout riders break for the ${getUnitDef(state, raidTarget.target.typeId).name} convoy and nearby marches.`
                : `Stonehelm scout riders break for ${getBuildingDef(state, raidTarget.target.typeId).name} and nearby marches.`,
            tone: "warn",
            ttl: 7,
          });
          enemy.ai.lastScoutRaidSecond = currentSecond;
        }
        const targetPressureCount = playerHarassableConvoys.length + playerRaidableStructures.length + playerHarassableNodes.length;
        enemy.ai.raidTimer = playerWorldPressureLevel >= 2 ? 8 : targetPressureCount >= 3 ? 10 : 14;
      } else {
        enemy.ai.raidTimer = 8;
      }
    } else {
      enemy.ai.raidTimer = 10;
    }
  } else if (enemy.ai.raidTimer <= 0) {
    enemy.ai.raidTimer = raidPressureUnlocked
      ? (playerHarassableConvoys.length > 0 || playerRaidableStructures.length > 0 || playerHarassableNodes.length > 0) ? 8 : 16
      : 20;
  }

  if (!hall || !playerHall) {
    return;
  }

  const hallTileX = hall.tileX + 2;
  const hallTileY = hall.tileY + 2;
  const siegeStagePoint = getSiegeStagePoint(state, playerHall, hall);
  const siegeStageTileX = Math.max(0, Math.floor(siegeStagePoint.x / state.world.tileSize) - 1);
  const siegeStageTileY = Math.max(0, Math.floor(siegeStagePoint.y / state.world.tileSize) - 1);
  const shrineAnchor = enemyFaithId
    ? getFaithBuildAnchor(state, enemyFaithId, hallTileX + 4, hallTileY - 3, 1)
    : null;
  const covenantAnchor = enemyFaithId
    ? getFaithBuildAnchor(state, enemyFaithId, hallTileX + 2, hallTileY - 2, 2)
    : null;
  const sanctuaryAnchor = enemyFaithId
    ? getFaithBuildAnchor(state, enemyFaithId, hallTileX, hallTileY - 3, 2)
    : null;

  if (enemy.ai.buildTimer <= 0) {
    const builder = state.units.find((unit) =>
      unit.factionId === "enemy" &&
      unit.health > 0 &&
      state.content.byId.units[unit.typeId].role === "worker",
    );

    if (builder) {
      if (!barracks && snapshot.resources.gold >= 85 && snapshot.resources.wood >= 95) {
        const site = findOpenBuildingSite(state, "barracks", hallTileX - 2, hallTileY + 4, 8);
        if (site) {
          attemptPlaceBuilding(state, "enemy", "barracks", site.tileX, site.tileY, builder.id);
        }
      } else if (
        enemyFaithId &&
        barracks &&
        !wayshrine &&
        canAffordCost(snapshot.resources, getBuildingDef(state, "wayshrine")?.cost)
      ) {
        const site = findOpenBuildingSite(
          state,
          "wayshrine",
          shrineAnchor?.tileX ?? (hallTileX + 4),
          shrineAnchor?.tileY ?? (hallTileY - 3),
          10,
        );
        if (site) {
          const placement = attemptPlaceBuilding(state, "enemy", "wayshrine", site.tileX, site.tileY, builder.id);
          if (placement.ok) {
            const currentSecond = Math.floor(state.meta.elapsed);
            if (enemy.ai.lastFaithAdvanceSecond !== currentSecond) {
              state.messages.unshift({
                id: `enemy-faith-wayshrine-${currentSecond}`,
                text: "Stonehelm raises a Wayshrine and begins consolidating covenant presence around the sacred site.",
                tone: "warn",
                ttl: 7,
              });
              enemy.ai.lastFaithAdvanceSecond = currentSecond;
            }
          }
        }
      } else if (playerKeepFortified && barracks && !quarry && snapshot.resources.gold >= 40 && snapshot.resources.wood >= 40) {
        const site = findOpenBuildingSite(state, "quarry", hallTileX + 3, hallTileY - 2, 10);
        if (site) {
          attemptPlaceBuilding(state, "enemy", "quarry", site.tileX, site.tileY, builder.id);
        }
      } else if (playerKeepFortified && barracks && !ironMine && snapshot.resources.gold >= 55 && snapshot.resources.wood >= 50) {
        const site = findOpenBuildingSite(state, "iron_mine", hallTileX + 5, hallTileY + 1, 10);
        if (site) {
          attemptPlaceBuilding(state, "enemy", "iron_mine", site.tileX, site.tileY, builder.id);
        }
      } else if (
        playerKeepFortified &&
        barracks &&
        quarry &&
        ironMine &&
        !siegeWorkshop &&
        canAffordCost(snapshot.resources, getBuildingDef(state, "siege_workshop")?.cost)
      ) {
        const site = findOpenBuildingSite(state, "siege_workshop", hallTileX - 5, hallTileY + 4, 10);
        if (site) {
          attemptPlaceBuilding(state, "enemy", "siege_workshop", site.tileX, site.tileY, builder.id);
        }
      } else if (
        enemyFaithId &&
        wayshrineCompleted &&
        !covenantHall &&
        (snapshot.faith.intensity ?? 0) >= 26 &&
        canAffordCost(snapshot.resources, getBuildingDef(state, "covenant_hall")?.cost)
      ) {
        const site = findOpenBuildingSite(
          state,
          "covenant_hall",
          covenantAnchor?.tileX ?? (hallTileX + 2),
          covenantAnchor?.tileY ?? (hallTileY - 2),
          12,
        );
        if (site) {
          const placement = attemptPlaceBuilding(state, "enemy", "covenant_hall", site.tileX, site.tileY, builder.id);
          if (placement.ok) {
            const currentSecond = Math.floor(state.meta.elapsed);
            if (enemy.ai.lastFaithAdvanceSecond !== currentSecond) {
              state.messages.unshift({
                id: `enemy-faith-hall-${currentSecond}`,
                text: "Stonehelm founds a Covenant Hall and begins mustering doctrine-bound warriors.",
                tone: "warn",
                ttl: 7,
              });
              enemy.ai.lastFaithAdvanceSecond = currentSecond;
            }
          }
        }
      } else if (
        enemyFaithId &&
        covenantHallCompleted &&
        !grandSanctuary &&
        (
          (snapshot.faith.intensity ?? 0) >= 48 ||
          Boolean(enemyActiveCovenantTest) ||
          Boolean(playerActiveCovenantTest) ||
          playerDivineRightActive
        ) &&
        canAffordCost(snapshot.resources, getBuildingDef(state, "grand_sanctuary")?.cost)
      ) {
        const site = findOpenBuildingSite(
          state,
          "grand_sanctuary",
          sanctuaryAnchor?.tileX ?? hallTileX,
          sanctuaryAnchor?.tileY ?? (hallTileY - 3),
          14,
        );
        if (site) {
          const placement = attemptPlaceBuilding(state, "enemy", "grand_sanctuary", site.tileX, site.tileY, builder.id);
          if (placement.ok) {
            const currentSecond = Math.floor(state.meta.elapsed);
            if (enemy.ai.lastFaithAdvanceSecond !== currentSecond) {
              state.messages.unshift({
                id: `enemy-faith-sanctuary-${currentSecond}`,
                text: "Stonehelm raises a Grand Sanctuary and opens the next covenant war tier.",
                tone: "warn",
                ttl: 7,
              });
              enemy.ai.lastFaithAdvanceSecond = currentSecond;
            }
          }
        }
      } else if (
        enemyFaithId &&
        enemy.faith?.covenantTestPassed &&
        grandSanctuaryCompleted &&
        !apexCovenant &&
        (snapshot.faith.intensity ?? 0) >= 80 &&
        canAffordCost(snapshot.resources, getBuildingDef(state, "apex_covenant")?.cost)
      ) {
        const site = findOpenBuildingSite(
          state,
          "apex_covenant",
          sanctuaryAnchor?.tileX ?? hallTileX,
          sanctuaryAnchor?.tileY ?? (hallTileY - 4),
          16,
        );
        if (site) {
          const placement = attemptPlaceBuilding(state, "enemy", "apex_covenant", site.tileX, site.tileY, builder.id);
          if (placement.ok) {
            const currentSecond = Math.floor(state.meta.elapsed);
            if (enemy.ai.lastFaithAdvanceSecond !== currentSecond) {
              state.messages.unshift({
                id: `enemy-faith-apex-${currentSecond}`,
                text: "Stonehelm opens an Apex Covenant seat after securing covenant recognition.",
                tone: "warn",
                ttl: 7,
              });
              enemy.ai.lastFaithAdvanceSecond = currentSecond;
            }
          }
        }
      } else if (
        playerKeepFortified &&
        siegeWorkshopCompleted &&
        !supplyCamp &&
        canAffordCost(snapshot.resources, getBuildingDef(state, "supply_camp")?.cost)
      ) {
        const site = findOpenBuildingSite(state, "supply_camp", siegeStageTileX, siegeStageTileY, 8);
        if (site) {
          attemptPlaceBuilding(state, "enemy", "supply_camp", site.tileX, site.tileY, builder.id);
        }
      } else if (
        playerKeepFortified &&
        supplyCampCompleted &&
        engineerCorps.length >= 1 &&
        supplyWagons.length >= 1 &&
        !stable &&
        canAffordCost(snapshot.resources, getBuildingDef(state, "stable")?.cost)
      ) {
        const site = findOpenBuildingSite(state, "stable", hallTileX + 5, hallTileY + 4, 10);
        if (site) {
          attemptPlaceBuilding(state, "enemy", "stable", site.tileX, site.tileY, builder.id);
        }
      } else if (snapshot.population.cap - snapshot.population.used - snapshot.population.reserved <= 1 && houses.length < 4) {
        const site = findOpenBuildingSite(state, "dwelling", hallTileX - 5, hallTileY - 1, 10);
        if (site) {
          attemptPlaceBuilding(state, "enemy", "dwelling", site.tileX, site.tileY, builder.id);
        }
      } else if (snapshot.resources.food < snapshot.population.total + 4 && farms.length < 3) {
        const site = findOpenBuildingSite(state, "farm", hallTileX + 3, hallTileY + 4, 10);
        if (site) {
          attemptPlaceBuilding(state, "enemy", "farm", site.tileX, site.tileY, builder.id);
        }
      } else if (snapshot.resources.water < snapshot.population.total + 4 && wells.length < 3) {
        const site = findOpenBuildingSite(state, "well", hallTileX + 1, hallTileY + 6, 10);
        if (site) {
          attemptPlaceBuilding(state, "enemy", "well", site.tileX, site.tileY, builder.id);
        }
      }
    }

    enemy.ai.buildTimer = playerKeepFortified ? 4 : 6;
  }

  if (
    hall &&
    hall.productionQueue.length === 0 &&
    getAvailablePopulation(state, "enemy") > 0 &&
    snapshot.resources.gold >= 45 &&
    idleWorkers.length < 5
  ) {
    queueProduction(state, hall.id, "villager");
  }

  if (barracks && barracks.productionQueue.length === 0 && getAvailablePopulation(state, "enemy") > 0) {
    const escortArmy = getEscortArmy(state, "enemy");
    const siegeArmy = getSiegeArmy(state, "enemy");
    const needsEscortMass = playerKeepFortified &&
      escortArmy.length < 4 + siegeArmy.length * 2 + Math.min(2, playerVerdantWardenDefenseCount);
    const barracksChoice = chooseBarracksUnit(state, "enemy", barracks.id, {
      needsEscortMass,
      playerKeepFortified,
    });
    if (barracksChoice) {
      const queued = queueProduction(state, barracks.id, barracksChoice.unitId);
      const currentSecond = Math.floor(state.meta.elapsed);
      if (queued.ok && enemy.ai.lastHouseUnitSecond !== currentSecond) {
        const enemyName = getFactionDisplayName(state, "enemy");
        const text = barracksChoice.reason === "house_unit"
          ? `${enemyName} spends blood to muster Axemen for the next assault.`
          : barracksChoice.reason === "blood_strain"
            ? `${enemyName} reins in Axemen levies while blood burden remains high, mustering Swordsmen instead.`
            : null;
        if (text) {
          state.messages.unshift({
            id: `enemy-house-unit-${currentSecond}`,
            text,
            tone: "warn",
            ttl: 7,
          });
          enemy.ai.lastHouseUnitSecond = currentSecond;
        }
      }
    }
  }

  const needsStageThreeFaith = Boolean(enemyFaithId) &&
    covenantHallCompleted &&
    enemyStageThreeFaithUnits.length < (enemyActiveCovenantTest ? 2 : 1);
  if (
    needsStageThreeFaith &&
    covenantHallCompleted.productionQueue.length === 0 &&
    getAvailablePopulation(state, "enemy") > 0
  ) {
    const faithUnitId = chooseFaithUnitForBuilding(state, "enemy", covenantHallCompleted.id, [3]);
    if (faithUnitId) {
      queueProduction(state, covenantHallCompleted.id, faithUnitId);
    }
  }

  const needsStageFourFaith = Boolean(enemyFaithId) &&
    grandSanctuaryCompleted &&
    (
      Boolean(enemyActiveCovenantTest) ||
      Boolean(playerActiveCovenantTest) ||
      playerDivineRightActive ||
      (snapshot.faith.intensity ?? 0) >= 60
    ) &&
    enemyStageFourFaithUnits.length < 1;
  if (
    needsStageFourFaith &&
    grandSanctuaryCompleted.productionQueue.length === 0 &&
    getAvailablePopulation(state, "enemy") > 0
  ) {
    const faithUnitId = chooseFaithUnitForBuilding(state, "enemy", grandSanctuaryCompleted.id, [4]);
    if (faithUnitId) {
      queueProduction(state, grandSanctuaryCompleted.id, faithUnitId);
    }
  }

  if (
    apexCovenantCompleted &&
    apexCovenantCompleted.productionQueue.length === 0 &&
    enemy.faith?.covenantTestPassed &&
    enemyApexFaithUnits.length === 0 &&
    getAvailablePopulation(state, "enemy") > 0
  ) {
    const apexUnitId = chooseFaithUnitForBuilding(state, "enemy", apexCovenantCompleted.id, [5]);
    if (apexUnitId) {
      queueProduction(state, apexCovenantCompleted.id, apexUnitId);
    }
  }

  if (raidPressureUnlocked && stableCompleted && stableCompleted.productionQueue.length === 0 && getAvailablePopulation(state, "enemy") > 0) {
    const scoutDef = getUnitDef(state, "scout_rider");
    const desiredScoutRiderCount = playerRaidableStructures.length > 0 || (playerSnapshot.territories?.count ?? 0) > 1 || playerWorldPressureLevel > 0
      ? 2
      : 1;
    if (
      scoutRiders.length < desiredScoutRiderCount &&
      canAffordCost(snapshot.resources, scoutDef.cost) &&
      getAvailablePopulation(state, "enemy") >= scoutDef.populationCost
    ) {
      queueProduction(state, stableCompleted.id, "scout_rider");
    }
  }

  if (siegeWorkshopCompleted && siegeWorkshopCompleted.productionQueue.length === 0 && getAvailablePopulation(state, "enemy") > 0 && playerKeepFortified) {
    const trebuchetUnits = getSiegeArmy(state, "enemy", "trebuchet");
    const ramUnits = getSiegeArmy(state, "enemy", "ram");
    const towerUnits = getSiegeArmy(state, "enemy", "siege_tower");
    const ballistaUnits = getSiegeArmy(state, "enemy", "ballista");
    const mantletUnits = getSiegeArmy(state, "enemy", "mantlet");
    const engineerDef = getUnitDef(state, "siege_engineer");
    const supplyWagonDef = getUnitDef(state, "supply_wagon");
    const trebuchetDef = getUnitDef(state, "trebuchet");
    const ramDef = getUnitDef(state, "ram");
    const towerDef = getUnitDef(state, "siege_tower");
    const ballistaDef = getUnitDef(state, "ballista");
    const mantletDef = getUnitDef(state, "mantlet");
    if (trebuchetUnits.length < 1 && canAffordCost(snapshot.resources, trebuchetDef.cost) && getAvailablePopulation(state, "enemy") >= trebuchetDef.populationCost) {
      queueProduction(state, siegeWorkshopCompleted.id, "trebuchet");
    } else if (engineerCorps.length < 1 && canAffordCost(snapshot.resources, engineerDef.cost) && getAvailablePopulation(state, "enemy") >= engineerDef.populationCost) {
      queueProduction(state, siegeWorkshopCompleted.id, "siege_engineer");
    } else if (supplyWagons.length < 1 && canAffordCost(snapshot.resources, supplyWagonDef.cost) && getAvailablePopulation(state, "enemy") >= supplyWagonDef.populationCost) {
      queueProduction(state, siegeWorkshopCompleted.id, "supply_wagon");
    } else if (mantletUnits.length < 1 && canAffordCost(snapshot.resources, mantletDef.cost) && getAvailablePopulation(state, "enemy") >= mantletDef.populationCost) {
      // Mantlet protects the siege line before ballista bolts start arriving.
      queueProduction(state, siegeWorkshopCompleted.id, "mantlet");
    } else if (ballistaUnits.length < 1 && canAffordCost(snapshot.resources, ballistaDef.cost) && getAvailablePopulation(state, "enemy") >= ballistaDef.populationCost) {
      // Ballista provides ranged anti-personnel pressure for the siege line.
      queueProduction(state, siegeWorkshopCompleted.id, "ballista");
    } else if (ramUnits.length < 1 && canAffordCost(snapshot.resources, ramDef.cost) && getAvailablePopulation(state, "enemy") >= ramDef.populationCost) {
      queueProduction(state, siegeWorkshopCompleted.id, "ram");
    } else if (towerUnits.length < 1 && canAffordCost(snapshot.resources, towerDef.cost) && getAvailablePopulation(state, "enemy") >= towerDef.populationCost) {
      queueProduction(state, siegeWorkshopCompleted.id, "siege_tower");
    }
  }

  if (rivalryUnlocked && enemy.ai.territoryTimer <= 0) {
    if (enemyActiveCovenantTest && enemyCovenantTargetPoint && army.length >= 2) {
      const destination = getControlPointPosition(state, enemyCovenantTargetPoint);
      issueMoveCommand(state, army.slice(0, Math.min(5, army.length)).map((unit) => unit.id), destination.x, destination.y);
      const currentSecond = Math.floor(state.meta.elapsed);
      if (enemy.ai.lastCovenantTerritorySecond !== currentSecond) {
        state.messages.unshift({
          id: `enemy-covenant-march-${currentSecond}`,
          text: `Stonehelm redirects its field host toward ${enemyCovenantTargetPoint.name} to satisfy ${enemyActiveCovenantTest.mandateLabel}.`,
          tone: "warn",
          ttl: 7,
        });
        enemy.ai.lastCovenantTerritorySecond = currentSecond;
      }
      enemy.ai.territoryTimer = enemyCovenantTargetPoint.ownerFactionId === "enemy" ? 12 : 8;
    } else if (playerActiveTerritorialGovernanceRecognition && playerGovernanceTargetPoint && army.length >= 2) {
      const destination = getControlPointPosition(state, playerGovernanceTargetPoint);
      issueMoveCommand(state, army.slice(0, Math.min(5, army.length)).map((unit) => unit.id), destination.x, destination.y);
      const currentSecond = Math.floor(state.meta.elapsed);
      if (enemy.ai.lastGovernanceTerritorySecond !== currentSecond) {
        state.messages.unshift({
          id: `enemy-governance-break-${currentSecond}`,
          text: `Stonehelm drives on ${playerGovernanceTargetPoint.name} to break ${playerSnapshot.displayName}'s Territorial Governance Recognition.`,
          tone: "warn",
          ttl: 7,
        });
        enemy.ai.lastGovernanceTerritorySecond = currentSecond;
      }
      enemy.ai.territoryTimer = playerGovernanceTargetPoint.ownerFactionId === "player" ? 6 : 10;
    } else if (enemyActiveTerritorialGovernanceRecognition && enemyGovernanceTargetPoint && army.length >= 2) {
      const destination = getControlPointPosition(state, enemyGovernanceTargetPoint);
      issueMoveCommand(state, army.slice(0, Math.min(4, army.length)).map((unit) => unit.id), destination.x, destination.y);
      const currentSecond = Math.floor(state.meta.elapsed);
      if (enemy.ai.lastGovernanceTerritorySecond !== currentSecond) {
        state.messages.unshift({
          id: `enemy-governance-hold-${currentSecond}`,
          text: `Stonehelm shifts its host toward ${enemyGovernanceTargetPoint.name} to preserve Territorial Governance Recognition.`,
          tone: "info",
          ttl: 7,
        });
        enemy.ai.lastGovernanceTerritorySecond = currentSecond;
      }
      enemy.ai.territoryTimer = 10;
    } else {
    const siegeInfrastructureActive = playerKeepFortified && (enemyHasSiege || siegeWorkshop || supplyCamp || ironMine || quarry);
    if (!siegeInfrastructureActive && army.length >= 2) {
      const targetPoint = pickTerritoryTarget(state, "enemy", getBuildingCenter(state, hall), {
        targetedFactionId: playerWorldPressure.targeted ? "player" : null,
        pressureSourceId: playerWorldPressure.targeted ? playerWorldPressure.topSourceId : "quiet",
      });
      if (targetPoint) {
        const destination = getControlPointPosition(state, targetPoint);
        issueMoveCommand(state, army.slice(0, Math.min(4, army.length)).map((unit) => unit.id), destination.x, destination.y);
        const currentSecond = Math.floor(state.meta.elapsed);
        const sourceAwareTerritoryExpansionPush = territoryExpansionSourceFocused &&
          targetPoint.ownerFactionId === "player";
        const sourceAwareOffHomePush = playerWorldPressure.targeted &&
          playerWorldPressure.topSourceId === "offHomeHoldings" &&
          targetPoint.ownerFactionId === "player" &&
          (targetPoint.continentId ?? "home") !== "home";
        const sourceAwareGreatReckoningPush = greatReckoningSourceFocused &&
          targetPoint.ownerFactionId === "player";
        const sourceAwareDarkExtremesPush = darkExtremesSourceFocused &&
          targetPoint.ownerFactionId === "player";
        if (sourceAwareOffHomePush && enemy.ai.lastPressureTerritorySecond !== currentSecond) {
          const enemyName = getFactionDisplayName(state, "enemy");
          const playerName = getFactionDisplayName(state, "player");
          state.messages.unshift({
            id: `enemy-pressure-front-${currentSecond}`,
            text: `${enemyName} drives on ${playerName}'s off-home marches, contesting continental overextension at its source.`,
            tone: "warn",
            ttl: 7,
          });
          enemy.ai.lastPressureTerritorySecond = currentSecond;
        } else if (sourceAwareGreatReckoningPush && enemy.ai.lastPressureTerritorySecond !== currentSecond) {
          const enemyName = getFactionDisplayName(state, "enemy");
          const playerName = getFactionDisplayName(state, "player");
          state.messages.unshift({
            id: `enemy-pressure-great-reckoning-${currentSecond}`,
            text: `${enemyName} drives on ${playerName}'s weakest marches as the Great Reckoning formalizes a coalition answer to overreach.`,
            tone: "warn",
            ttl: 7,
          });
          enemy.ai.lastPressureTerritorySecond = currentSecond;
        } else if (sourceAwareTerritoryExpansionPush && enemy.ai.lastPressureTerritorySecond !== currentSecond) {
          const enemyName = getFactionDisplayName(state, "enemy");
          const playerName = getFactionDisplayName(state, "player");
          state.messages.unshift({
            id: `enemy-pressure-territory-expansion-${currentSecond}`,
            text: `${enemyName} drives on ${playerName}'s stretched marches as territorial expansion turns breadth into a frontier liability.`,
            tone: "warn",
            ttl: 7,
          });
          enemy.ai.lastPressureTerritorySecond = currentSecond;
        } else if (sourceAwareDarkExtremesPush && enemy.ai.lastPressureTerritorySecond !== currentSecond) {
          const enemyName = getFactionDisplayName(state, "enemy");
          const playerName = getFactionDisplayName(state, "player");
          state.messages.unshift({
            id: `enemy-pressure-dark-extremes-${currentSecond}`,
            text: `${enemyName} drives on ${playerName}'s weakest marches as dark extremes turn world pressure into punitive war.`,
            tone: "warn",
            ttl: 7,
          });
          enemy.ai.lastPressureTerritorySecond = currentSecond;
        }
      }
    }

    enemy.ai.territoryTimer = matchStageNumber >= 3 ? 18 : 22;
    }
  } else if (!rivalryUnlocked && enemy.ai.territoryTimer <= 0) {
    enemy.ai.territoryTimer = 24;
  }

  if (enemy.ai.attackTimer <= 0) {
    const siegeArmy = getSiegeArmy(state, "enemy");
    const fieldWaterCriticalForce = operationalSiegeForce.filter((unit) =>
      unit.fieldWaterStatus === "critical" || (unit.fieldWaterStrain ?? 0) >= 12);
    const fieldWaterAttritionForce = operationalSiegeForce.filter((unit) => unit.fieldWaterAttritionActive);
    const fieldWaterDesertionRiskForce = operationalSiegeForce.filter((unit) => unit.fieldWaterDesertionRisk);
    const fieldWaterRegroupAnchor = supplyCampCompleted
      ? getBuildingCenter(state, supplyCampCompleted)
      : getBuildingCenter(state, hall);
    const engineeringReady = engineerCorps.length >= 1;
    const supplyLineReady = Boolean(supplyCampCompleted) && supplyWagons.length >= 1;
    const suppliedSiegeReady = siegeArmy.length > 0 && suppliedSiegeArmy.length >= 1;
    const siegeLinesFormed = areSiegeLinesFormed(siegeArmy, siegeStagePoint, 110) &&
      escortArmy.filter((unit) => Math.hypot(unit.x - siegeStagePoint.x, unit.y - siegeStagePoint.y) <= 150).length >= Math.min(3, escortArmy.length);
    const supportLinesFormed =
      engineerCorps.filter((unit) => Math.hypot(unit.x - siegeStagePoint.x, unit.y - siegeStagePoint.y) <= 150).length >= Math.min(1, engineerCorps.length || 1) &&
      supplyWagons.filter((unit) => Math.hypot(unit.x - siegeStagePoint.x, unit.y - siegeStagePoint.y) <= 150).length >= Math.min(1, supplyWagons.length || 1);
    const formalSiegeLinesFormed = siegeLinesFormed && supportLinesFormed;

    if (army.length >= 3) {
      const currentSecond = Math.floor(state.meta.elapsed);
      if (fieldWaterDesertionRiskForce.length > 0) {
        issueMoveCommand(state, operationalSiegeForce.map((unit) => unit.id), fieldWaterRegroupAnchor.x, fieldWaterRegroupAnchor.y);
        if (enemy.ai.lastFieldWaterDesertionSecond !== currentSecond) {
          state.messages.unshift({
            id: `enemy-field-water-breaking-${currentSecond}`,
            text: "Stonehelm abandons the assault and drags the column back before dehydration turns into desertion.",
            tone: "good",
            ttl: 7,
          });
          enemy.ai.lastFieldWaterDesertionSecond = currentSecond;
        }
      } else if (fieldWaterAttritionForce.length > 0) {
        issueMoveCommand(state, operationalSiegeForce.map((unit) => unit.id), fieldWaterRegroupAnchor.x, fieldWaterRegroupAnchor.y);
        if (enemy.ai.lastFieldWaterAttritionSecond !== currentSecond) {
          state.messages.unshift({
            id: `enemy-field-water-attrition-${currentSecond}`,
            text: "Stonehelm sees men dropping from thirst and recoils toward its water line before reforming.",
            tone: "good",
            ttl: 7,
          });
          enemy.ai.lastFieldWaterAttritionSecond = currentSecond;
        }
      } else if (fieldWaterCriticalForce.length > 0) {
        issueMoveCommand(state, operationalSiegeForce.map((unit) => unit.id), fieldWaterRegroupAnchor.x, fieldWaterRegroupAnchor.y);
        if (enemy.ai.lastFieldWaterDelaySecond !== currentSecond) {
          state.messages.unshift({
            id: `enemy-field-water-delay-${currentSecond}`,
            text: "Stonehelm pulls the assault column back toward its water anchors before renewing the march.",
            tone: "good",
            ttl: 7,
          });
          enemy.ai.lastFieldWaterDelaySecond = currentSecond;
        }
      } else if (playerKeepFortified && !enemyHasSiege) {
        if (enemy.ai.lastSiegeRefusalSecond !== currentSecond) {
          state.messages.unshift({
            id: `enemy-siege-refusal-${currentSecond}`,
            text: "Stonehelm scouts the Ironmark keep and refuses a direct assault without siege engines.",
            tone: "info",
            ttl: 7,
          });
          enemy.ai.lastSiegeRefusalSecond = currentSecond;
        }
      } else if (playerKeepFortified && !engineeringReady) {
        if (enemy.ai.lastEngineerDelaySecond !== currentSecond) {
          state.messages.unshift({
            id: `enemy-siege-engineer-${currentSecond}`,
            text: "Stonehelm halts the breach plan until engineer crews join the line.",
            tone: "info",
            ttl: 7,
          });
          enemy.ai.lastEngineerDelaySecond = currentSecond;
        }
      } else if (playerKeepFortified && !supplyCampCompleted) {
        if (enemy.ai.lastSupplyCampSecond !== currentSecond) {
          state.messages.unshift({
            id: `enemy-siege-camp-${currentSecond}`,
            text: "Stonehelm secures a forward supply camp before pressing the keep.",
            tone: "info",
            ttl: 7,
          });
          enemy.ai.lastSupplyCampSecond = currentSecond;
        }
      } else if (playerKeepFortified && !supplyLineReady) {
        if (enemy.ai.lastSupplyDelaySecond !== currentSecond) {
          state.messages.unshift({
            id: `enemy-siege-supply-${currentSecond}`,
            text: "Stonehelm delays the assault until wagons can sustain the siege line.",
            tone: "info",
            ttl: 7,
          });
          enemy.ai.lastSupplyDelaySecond = currentSecond;
        }
      } else if (playerKeepFortified && interdictedSupplyWagons.length > 0) {
        issueMoveCommand(state, operationalSiegeForce.map((unit) => unit.id), fieldWaterRegroupAnchor.x, fieldWaterRegroupAnchor.y);
        if (enemy.ai.lastSupplyScreenSecond !== currentSecond) {
          state.messages.unshift({
            id: `enemy-siege-screen-${currentSecond}`,
            text: "Stonehelm pulls the assault line back to screen its struck convoy before the breach can resume.",
            tone: "warn",
            ttl: 7,
          });
          enemy.ai.lastSupplyScreenSecond = currentSecond;
        }
      } else if (playerKeepFortified && recoveringSupplyWagons.length > 0 && convoyRecoveringUnscreenedCount > 0) {
        // Session 82: recovering wagons without escort screen. Pull assault line
        // back to regroup anchor until escorts close around the recovering convoy.
        issueMoveCommand(state, operationalSiegeForce.map((unit) => unit.id), fieldWaterRegroupAnchor.x, fieldWaterRegroupAnchor.y);
        if (enemy.ai.lastSupplyReconsolidationSecond !== currentSecond) {
          state.messages.unshift({
            id: `enemy-siege-reconsolidate-${currentSecond}`,
            text: "Stonehelm reforms wagon escort around the recovering convoy before the breach can resume.",
            tone: "warn",
            ttl: 7,
          });
          enemy.ai.lastSupplyReconsolidationSecond = currentSecond;
        }
      } else if (playerKeepFortified && recoveringSupplyWagons.length > 0 && convoyRecoveringUnscreenedCount === 0) {
        // Session 82: recovering wagons fully screened. Hold at siege stage point
        // while convoy recovery completes instead of pulling back to regroup.
        issueMoveCommand(state, operationalSiegeForce.map((unit) => unit.id), siegeStagePoint.x, siegeStagePoint.y);
        if (enemy.ai.lastSupplyReconsolidationSecond !== currentSecond) {
          state.messages.unshift({
            id: `enemy-siege-reconsolidate-${currentSecond}`,
            text: "Stonehelm holds the breach while the convoy recovers under escort screen.",
            tone: "warn",
            ttl: 7,
          });
          enemy.ai.lastSupplyReconsolidationSecond = currentSecond;
        }
      } else if (playerKeepFortified && !suppliedSiegeReady) {
        issueMoveCommand(state, operationalSiegeForce.map((unit) => unit.id), siegeStagePoint.x, siegeStagePoint.y);
        if (enemy.ai.lastSupplyStageSecond !== currentSecond) {
          state.messages.unshift({
            id: `enemy-siege-resupply-${currentSecond}`,
            text: "Stonehelm consolidates engineers and wagons to restore full siege readiness.",
            tone: "warn",
            ttl: 7,
          });
          enemy.ai.lastSupplyStageSecond = currentSecond;
        }
      } else if (playerKeepFortified && escortArmy.length < 3 + Math.min(2, playerVerdantWardenDefenseCount)) {
        if (enemy.ai.lastEscortDelaySecond !== currentSecond) {
          state.messages.unshift({
            id: `enemy-siege-escort-${currentSecond}`,
            text: playerVerdantWardenDefenseCount > 0
              ? "Stonehelm delays the assault until a heavier escort can break Hartvale wardens off the wall."
              : "Stonehelm delays the assault until a stronger escort screens the engines.",
            tone: "info",
            ttl: 7,
          });
          enemy.ai.lastEscortDelaySecond = currentSecond;
        }
      } else if (playerKeepFortified && !formalSiegeLinesFormed) {
        issueMoveCommand(state, operationalSiegeForce.map((unit) => unit.id), siegeStagePoint.x, siegeStagePoint.y);
        if (enemy.ai.lastSiegeStageSecond !== currentSecond) {
          state.messages.unshift({
            id: `enemy-siege-stage-${currentSecond}`,
            text: "Stonehelm forms siege lines with engines, engineers, and wagons outside the Ironmark keep.",
            tone: "warn",
            ttl: 7,
          });
          enemy.ai.lastSiegeStageSecond = currentSecond;
        }
      } else if (playerKeepFortified && isReliefArmyApproaching(state, "player", siegeStagePoint)) {
        // Session 12: relief-window awareness. A player relief column is
        // approaching the siege. Stonehelm holds the final breach commit,
        // pulls siege units slightly back to the stage, and recomposes.
        issueMoveCommand(state, operationalSiegeForce.map((unit) => unit.id), siegeStagePoint.x, siegeStagePoint.y);
        if (enemy.ai.lastReliefDelaySecond !== currentSecond) {
          state.messages.unshift({
            id: `enemy-relief-delay-${currentSecond}`,
            text: "Stonehelm spots a player relief column and holds the breach commit until the field is secured.",
            tone: "warn",
            ttl: 7,
          });
          enemy.ai.lastReliefDelaySecond = currentSecond;
        }
      } else if (playerKeepFortified && (enemy.cohesionPenaltyUntil ?? 0) > state.meta.elapsed) {
        // Session 14: post-repulse adjustment. If wave-spam denial has triggered
        // (assault cohesion strain threshold crossed), Stonehelm retreats to the
        // stage point, recomposes, and marks a re-attempt window. Canonical
        // reference: master doctrine section X — failed assaults must cost tempo,
        // not just lives. The AI is NOT allowed to blindly re-throw itself at the
        // keep during the cohesion penalty window.
        issueMoveCommand(state, operationalSiegeForce.map((unit) => unit.id), siegeStagePoint.x, siegeStagePoint.y);
        enemy.ai.postRepulseUntil = enemy.cohesionPenaltyUntil;
        enemy.ai.repeatedAssaultAttempts = (enemy.ai.repeatedAssaultAttempts ?? 0) + 1;
        if (enemy.ai.lastPostRepulseSecond !== currentSecond) {
          state.messages.unshift({
            id: `enemy-post-repulse-${currentSecond}`,
            text: "Stonehelm reels from the repulsed assault, pulls back to the siege stage, and reorganizes the line.",
            tone: "good",
            ttl: 7,
          });
          enemy.ai.lastPostRepulseSecond = currentSecond;
        }
      } else if (
        playerKeepFortified &&
        (enemy.ai.postRepulseUntil ?? 0) > 0 &&
        (enemy.ai.postRepulseUntil ?? 0) <= state.meta.elapsed &&
        (enemy.ai.repeatedAssaultAttempts ?? 0) > 0 &&
        (enemy.ai.repeatedAssaultAttempts ?? 0) < 4
      ) {
        // Session 16: repeated-assault window logic. After post-repulse cooldown
        // expires AND Stonehelm has been repulsed at least once, re-queue the
        // assault with a slight repositioning angle. Not blind re-commit —
        // a reconstituted push with canonical context ("renewed assault" tone
        // rather than initial advance). Cap at 4 attempts per match to prevent
        // infinite loops; after 4, AI escalates siege production instead.
        const offsetAngle = (enemy.ai.repeatedAssaultAttempts * 0.7) % (Math.PI * 2);
        const offsetRadius = 60;
        const offsetStage = {
          x: siegeStagePoint.x + Math.cos(offsetAngle) * offsetRadius,
          y: siegeStagePoint.y + Math.sin(offsetAngle) * offsetRadius,
        };
        issueAttackCommand(state, operationalSiegeForce.map((unit) => unit.id), "building", playerHall.id);
        if (enemy.ai.lastRepeatedAssaultSecond !== currentSecond) {
          state.messages.unshift({
            id: `enemy-renewed-assault-${currentSecond}`,
            text: `Stonehelm renews the assault from a new angle (attempt ${enemy.ai.repeatedAssaultAttempts}).`,
            tone: "warn",
            ttl: 7,
          });
          enemy.ai.lastRepeatedAssaultSecond = currentSecond;
        }
        // Clear the post-repulse marker so the branch doesn't loop every tick.
        enemy.ai.postRepulseUntil = 0;
      } else if (playerKeepFortified && suppliedSiegeReady && suppliedSiegeArmy.length === 0) {
        // Session 14: mid-siege supply-chain collapse. If the siege force was ready
        // but supplied engine count has dropped to zero (player destroyed camps or
        // wagons), pull back to stage and recompose rather than push hollow engines
        // into the breach.
        issueMoveCommand(state, operationalSiegeForce.map((unit) => unit.id), siegeStagePoint.x, siegeStagePoint.y);
        if (enemy.ai.lastSupplyCollapseSecond !== currentSecond) {
          state.messages.unshift({
            id: `enemy-supply-collapse-${currentSecond}`,
            text: "Stonehelm's siege supply has collapsed mid-approach. Engines fall back to the stage to re-link.",
            tone: "good",
            ttl: 7,
          });
          enemy.ai.lastSupplyCollapseSecond = currentSecond;
        }
      } else {
        issueAttackCommand(state, operationalSiegeForce.map((unit) => unit.id), "building", playerHall.id);
        if (playerKeepFortified && enemy.ai.lastSiegeAdvanceSecond !== currentSecond) {
          state.messages.unshift({
            id: `enemy-siege-advance-${currentSecond}`,
            text: "Stonehelm advances with engines, engineers, and a sustained supply line toward the Ironmark keep.",
            tone: "warn",
            ttl: 7,
          });
          enemy.ai.lastSiegeAdvanceSecond = currentSecond;
        } else if (enemy.ai.lastAlertSecond !== currentSecond) {
          state.messages.unshift({
            id: `enemy-wave-${currentSecond}`,
            text: "Stonehelm warband advancing on the Ironmark hall.",
            tone: "warn",
            ttl: 7,
          });
          enemy.ai.lastAlertSecond = currentSecond;
        }
      }
    }

    enemy.ai.attackTimer = playerKeepFortified
      ? fieldWaterDesertionRiskForce.length > 0
        ? 14
        : fieldWaterAttritionForce.length > 0
          ? 12
          : fieldWaterCriticalForce.length > 0
        ? 8
        : !enemyHasSiege
          ? 16
          : !engineeringReady || !supplyCampCompleted || !supplyLineReady
            ? 8
            : interdictedSupplyWagons.length > 0
              ? 6
            : recoveringSupplyWagons.length > 0
              ? convoyRecoveringUnscreenedCount > 0 ? 5 : 6
            : !suppliedSiegeReady
              ? 7
              : !formalSiegeLinesFormed
                ? 10
                : 30
      : 38;
  }

  // Session 29: AI naval reactivity. When the player has a live harbor or any vessel,
  // Stonehelm reacts by building its own harbor and a scout vessel. Canonical:
  // the world must not remain passive while the player establishes naval dominance.
  const playerHasHarbor = state.buildings.some(
    (b) => b.factionId === "player" && b.health > 0 && b.completed &&
      (b.typeId === "harbor_tier_1" || b.typeId === "harbor_tier_2"),
  );
  const playerHasVessel = state.units.some(
    (u) => u.factionId === "player" && u.health > 0 &&
      state.content.byId.units[u.typeId]?.role === "vessel",
  );
  const enemyHarborExists = state.buildings.some(
    (b) => b.factionId === "enemy" && b.health > 0 &&
      (b.typeId === "harbor_tier_1" || b.typeId === "harbor_tier_2"),
  );
  if ((playerHasHarbor || playerHasVessel) && !enemyHarborExists) {
    const harborDef = getBuildingDef(state, "harbor_tier_1");
    if (harborDef && canAffordCost(snapshot.resources, harborDef.cost)) {
      // Try to find a site near the canonical northeast coastal bay (tiles 64-71, 0-9).
      const site = findOpenBuildingSite(state, "harbor_tier_1", 62, 2, 10);
      if (site) {
        // Pick an idle worker (or any worker) to build.
        const worker = getIdleWorkers(state, "enemy")[0]
          ?? state.units.find((u) => u.factionId === "enemy" && state.content.byId.units[u.typeId]?.role === "worker");
        if (worker) {
          const placement = attemptPlaceBuilding(state, "enemy", "harbor_tier_1", site.tileX, site.tileY, worker.id);
          if (placement.ok) {
            const currentSecond = Math.floor(state.meta.elapsed);
            if (enemy.ai.lastNavalReactionSecond !== currentSecond) {
              state.messages.unshift({
                id: `enemy-naval-react-${currentSecond}`,
                text: "Stonehelm answers the player's naval moves by founding its own harbor on the northeast coast.",
                tone: "warn",
                ttl: 8,
              });
              enemy.ai.lastNavalReactionSecond = currentSecond;
            }
          }
        }
      }
    }
  }
  // Once Stonehelm has a completed harbor, queue at least one scout vessel.
  const enemyHarborCompleted = state.buildings.find(
    (b) => b.factionId === "enemy" && b.completed && b.health > 0 &&
      (b.typeId === "harbor_tier_1" || b.typeId === "harbor_tier_2"),
  );
  if (enemyHarborCompleted && enemyHarborCompleted.productionQueue.length === 0) {
    const enemyScouts = state.units.filter(
      (u) => u.factionId === "enemy" && u.health > 0 && u.typeId === "scout_vessel",
    );
    const enemyGalleys = state.units.filter(
      (u) => u.factionId === "enemy" && u.health > 0 && u.typeId === "war_galley",
    );
    const scoutDef = getUnitDef(state, "scout_vessel");
    const galleyDef = getUnitDef(state, "war_galley");
    if (enemyScouts.length < 1 && canAffordCost(snapshot.resources, scoutDef.cost) && getAvailablePopulation(state, "enemy") >= scoutDef.populationCost) {
      queueProduction(state, enemyHarborCompleted.id, "scout_vessel");
    } else if (enemyGalleys.length < 1 && (playerHasHarbor || playerHasVessel) && canAffordCost(snapshot.resources, galleyDef.cost) && getAvailablePopulation(state, "enemy") >= galleyDef.populationCost) {
      queueProduction(state, enemyHarborCompleted.id, "war_galley");
    }
  }

  // Session 23: Supply-protection patrols.
  // When Stonehelm has a completed supply camp + live wagons (live siege chain),
  // assign up to 2 combat units per wagon as escort patrol. Counters player
  // sabotage and supply raiding attempts. Canonical: a live siege line must be
  // actively defended, not passively assumed safe.
  if (supplyCampCompleted && supplyWagons.length > 0 && escortArmy.length >= 5) {
    const patrolCapacity = Math.min(escortArmy.length - 3, supplyWagons.length * 2);
    if (patrolCapacity > 0) {
      // Find escort candidates: combat units not already patrolling, not in the
      // operational siege force, not currently attacking.
      const existingPatrolIds = new Set((enemy.ai.supplyPatrolUnitIds ?? []).filter((id) => {
        const u = state.units.find((unit) => unit.id === id && unit.health > 0);
        return Boolean(u);
      }));
      const wagonCenter = supplyWagons.reduce(
        (acc, w) => ({ x: acc.x + w.x, y: acc.y + w.y }),
        { x: 0, y: 0 },
      );
      const wagonMeanX = wagonCenter.x / supplyWagons.length;
      const wagonMeanY = wagonCenter.y / supplyWagons.length;
      escortArmy
        .filter((unit) => existingPatrolIds.has(unit.id))
        .forEach((unit) => {
          if (unit.command?.type === "attack") {
            return;
          }
          if (Math.hypot(unit.x - wagonMeanX, unit.y - wagonMeanY) > 96) {
            issueMoveCommand(state, [unit.id], wagonMeanX, wagonMeanY);
          }
        });
      const currentPatrolCount = existingPatrolIds.size;
      if (currentPatrolCount < patrolCapacity) {
        const needed = patrolCapacity - currentPatrolCount;
        const candidates = escortArmy.filter((unit) => {
          if (existingPatrolIds.has(unit.id)) return false;
          if (unit.command?.type === "attack") return false;
          const ud = state.content.byId.units[unit.typeId];
          if (!ud || ud.role === "worker" || ud.siegeClass) return false;
          return true;
        }).slice(0, needed);
        candidates.forEach((unit) => {
          issueMoveCommand(state, [unit.id], wagonMeanX, wagonMeanY);
          existingPatrolIds.add(unit.id);
        });
        enemy.ai.supplyPatrolUnitIds = Array.from(existingPatrolIds);
        if (candidates.length > 0) {
          const currentSecond = Math.floor(state.meta.elapsed);
          if (enemy.ai.lastSupplyPatrolSecond !== currentSecond) {
            state.messages.unshift({
              id: `enemy-supply-patrol-${currentSecond}`,
              text: `Stonehelm assigns ${candidates.length} combat units to patrol the siege supply line.`,
              tone: "warn",
              ttl: 7,
            });
            enemy.ai.lastSupplyPatrolSecond = currentSecond;
          }
        }
      }
      const previousPatrolIds = new Set(enemy.ai.supplyPatrolUnitIds ?? []);
      // Session 82: clear stale escort-binding before reassignment so dead wagons
      // do not leave orphan bindings on escort units.
      escortArmy.forEach((unit) => {
        if (unit.escortAssignedWagonId) {
          const boundWagon = state.units.find((w) => w.id === unit.escortAssignedWagonId && w.health > 0);
          if (!boundWagon) {
            unit.escortAssignedWagonId = null;
          }
        }
      });
      const { assignments } = buildSupplyPatrolAssignments(state, "enemy", supplyWagons, escortArmy);
      enemy.ai.supplyPatrolAssignments = assignments.map((assignment) => ({
        unitId: assignment.unitId,
        wagonId: assignment.wagonId,
        slotIndex: assignment.slotIndex,
      }));
      enemy.ai.supplyPatrolUnitIds = assignments.map((assignment) => assignment.unitId);
      assignments.forEach((assignment) => {
        const patrolUnit = state.units.find((unit) => unit.id === assignment.unitId);
        const wagon = state.units.find((unit) => unit.id === assignment.wagonId);
        if (!patrolUnit || !wagon) {
          return;
        }
        // Session 82: write escort-binding on the unit so it persists across ticks
        // and survives snapshot round-trip. Escorts now carry their wagon assignment
        // as live unit state, not just ephemeral AI metadata.
        patrolUnit.escortAssignedWagonId = wagon.id;
        const scoutThreat = getNearestHostileScoutRider(
          state,
          patrolUnit.factionId,
          wagon,
          isSupplyWagonInterdicted(state, wagon) || isSupplyWagonRecovering(state, wagon) ? 220 : 164,
        );
        if (scoutThreat) {
          issueAttackCommand(state, [patrolUnit.id], "unit", scoutThreat.id);
          return;
        }
        const escortPoint = getConvoyEscortFormationPoint(state, wagon, assignment.slotIndex);
        const destinationDrift = patrolUnit.command?.type === "move"
          ? Math.hypot((patrolUnit.command.x ?? patrolUnit.x) - escortPoint.x, (patrolUnit.command.y ?? patrolUnit.y) - escortPoint.y)
          : Infinity;
        const leash = isSupplyWagonRecovering(state, wagon) ? 20 : 34;
        if (Math.hypot(patrolUnit.x - escortPoint.x, patrolUnit.y - escortPoint.y) > leash || destinationDrift > 18) {
          issueMoveCommand(state, [patrolUnit.id], escortPoint.x, escortPoint.y);
        }
      });
      // Session 82: stamp convoyReconsolidatedAt on recovering wagons that just
      // became fully screened. This provides a legible timestamp for when the
      // convoy reformed, which assault timing can read.
      supplyWagons.forEach((wagon) => {
        if (isSupplyWagonRecovering(state, wagon) &&
            getSupplyWagonEscortCoverage(state, "enemy", wagon, escortArmy).screened &&
            (wagon.convoyReconsolidatedAt ?? 0) === 0) {
          wagon.convoyReconsolidatedAt = state.meta.elapsed;
        }
        if (!isSupplyWagonRecovering(state, wagon) && !isSupplyWagonInterdicted(state, wagon)) {
          wagon.convoyReconsolidatedAt = 0;
        }
      });
      if (assignments.length > 0) {
        const currentSecond = Math.floor(state.meta.elapsed);
        const recoveringAssignments = assignments.filter((assignment) => {
          const wagon = state.units.find((unit) => unit.id === assignment.wagonId);
          return wagon && isSupplyWagonRecovering(state, wagon);
        }).length;
        const newlyAssignedCount = assignments.filter((assignment) => !previousPatrolIds.has(assignment.unitId)).length;
        if ((recoveringAssignments > 0 || newlyAssignedCount > 0 || previousPatrolIds.size !== assignments.length) && enemy.ai.lastSupplyPatrolSecond !== currentSecond) {
          state.messages.unshift({
            id: `enemy-supply-patrol-${currentSecond}`,
            text: recoveringAssignments > 0
              ? `Stonehelm reconsolidates ${assignments.length}/${patrolCapacity} escort units around its recovering wagons.`
              : `Stonehelm binds ${assignments.length}/${patrolCapacity} combat units directly to the siege wagons.`,
            tone: "warn",
            ttl: 7,
          });
          enemy.ai.lastSupplyPatrolSecond = currentSecond;
        }
      }
    }
  } else {
    // No live supply chain — clear patrol assignments.
    enemy.ai.supplyPatrolUnitIds = [];
    enemy.ai.supplyPatrolAssignments = [];
  }

  // Session 11: AI sabotage reciprocity. Stonehelm can run sabotage against the player
  // when it has budget and a spymaster-role bloodline member, just like the player can
  // against it. Cooldown prevents spam. Target selection prefers fortification/logistics
  // pressure points.
  enemy.ai.sabotageTimer = (enemy.ai.sabotageTimer ?? 45) - dt;
  if (hostileOperationsSourceFocused) {
    enemy.ai.sabotageTimer = Math.min(enemy.ai.sabotageTimer, 6);
  }
  if (playerWorldPressureLevel > 0) {
    enemy.ai.sabotageTimer = Math.min(enemy.ai.sabotageTimer, playerWorldPressureLevel >= 2 ? 8 : 16);
  }
  if (playerConvergencePressure.active) {
    enemy.ai.sabotageTimer = Math.min(enemy.ai.sabotageTimer, playerConvergencePressure.sabotageTimerCap ?? enemy.ai.sabotageTimer);
  }
  if (liveCounterIntelDossierOnPlayer) {
    enemy.ai.sabotageTimer = Math.min(
      enemy.ai.sabotageTimer,
      (liveCounterIntelDossierOnPlayer.interceptCount ?? 0) >= 2 ? 4 : 6,
    );
  }
  if (enemy.ai.sabotageTimer <= 0) {
    const budget = snapshot.resources;
    const hasBudget = (budget.gold ?? 0) >= 60 && (budget.influence ?? 0) >= 12;
    const target = pickAiSabotageTarget(state, liveCounterIntelDossierOnPlayer);
    if (hasBudget && target) {
      const result = startSabotageOperation(state, "enemy", target.subtype, "player", target.building.id, {
        intelligenceReportId: target.intelligenceReportId ?? null,
        intelligenceSupportBonus: target.intelligenceSupportBonus ?? 0,
        retaliationReason: target.retaliationReason ?? null,
        retaliationInterceptType: target.retaliationInterceptType ?? null,
      });
      if (result.ok) {
        const currentSecond = Math.floor(state.meta.elapsed);
        if (hostileOperationsSourceFocused && enemy.ai.lastCovertBacklashSecond !== currentSecond) {
          const enemyName = getFactionDisplayName(state, "enemy");
          const playerName = getFactionDisplayName(state, "player");
          state.messages.unshift({
            id: `enemy-covert-backlash-sabotage-${currentSecond}`,
            text: `${enemyName} answers ${playerName}'s hostile operations with retaliatory sabotage.`,
            tone: "warn",
            ttl: 7,
          });
          enemy.ai.lastCovertBacklashSecond = currentSecond;
        }
        enemy.ai.sabotageTimer = 85;
        return;
      }
    }
    enemy.ai.sabotageTimer = 25;
  }

  enemy.ai.counterIntelligenceTimer = (enemy.ai.counterIntelligenceTimer ?? 40) - dt;
  if (hostileOperationsSourceFocused) {
    enemy.ai.counterIntelligenceTimer = Math.min(enemy.ai.counterIntelligenceTimer, 4);
  }
  if (enemy.ai.counterIntelligenceTimer <= 0) {
    if (shouldAiRaiseCounterIntelligence(state)) {
      const result = startCounterIntelligenceOperation(state, "enemy");
      if (result.ok) {
        const currentSecond = Math.floor(state.meta.elapsed);
        if (hostileOperationsSourceFocused && enemy.ai.lastCovertBacklashSecond !== currentSecond) {
          const enemyName = getFactionDisplayName(state, "enemy");
          const playerName = getFactionDisplayName(state, "player");
          state.messages.unshift({
            id: `enemy-covert-backlash-watch-${currentSecond}`,
            text: `${enemyName} hardens the court as ${playerName}'s hostile operations drive world pressure.`,
            tone: "warn",
            ttl: 7,
          });
          enemy.ai.lastCovertBacklashSecond = currentSecond;
        }
        enemy.ai.counterIntelligenceTimer = 190;
        return;
      }
    }
    enemy.ai.counterIntelligenceTimer = 45;
  }

  enemy.ai.espionageTimer = (enemy.ai.espionageTimer ?? 55) - dt;
  if (playerWorldPressureLevel > 0) {
    enemy.ai.espionageTimer = Math.min(enemy.ai.espionageTimer, playerWorldPressureLevel >= 2 ? 10 : 18);
  }
  if (playerConvergencePressure.active) {
    enemy.ai.espionageTimer = Math.min(enemy.ai.espionageTimer, playerConvergencePressure.espionageTimerCap ?? enemy.ai.espionageTimer);
  }
  if (enemy.ai.espionageTimer <= 0) {
    const liveEspionage = (enemy.dynasty?.operations?.active ?? []).some((operation) =>
      operation.type === "espionage" && operation.targetFactionId === "player");
    if (!liveIntelOnPlayer && !liveEspionage) {
      const result = startEspionageOperation(state, "enemy", "player");
      if (result.ok) {
        enemy.ai.espionageTimer = 150;
        return;
      }
    }
    enemy.ai.espionageTimer = liveCounterIntelDossierOnPlayer ? 120 : liveIntelOnPlayer ? 90 : 30;
  }

  enemy.ai.assassinationTimer = (enemy.ai.assassinationTimer ?? 80) - dt;
  if (darkExtremesSourceFocused) {
    enemy.ai.assassinationTimer = Math.min(enemy.ai.assassinationTimer, 6);
  }
  if (playerWorldPressureLevel > 0) {
    enemy.ai.assassinationTimer = Math.min(enemy.ai.assassinationTimer, playerWorldPressureLevel >= 2 ? 12 : 22);
  }
  if (playerConvergencePressure.active) {
    enemy.ai.assassinationTimer = Math.min(enemy.ai.assassinationTimer, playerConvergencePressure.assassinationTimerCap ?? enemy.ai.assassinationTimer);
  }
  if (liveCounterIntelDossierOnPlayer) {
    enemy.ai.assassinationTimer = Math.min(
      enemy.ai.assassinationTimer,
      (liveCounterIntelDossierOnPlayer.interceptType ?? "") === "assassination" ? 4 : 6,
    );
  }
  if (enemy.ai.assassinationTimer <= 0) {
    const target = liveIntelOnPlayer ? pickAiAssassinationTarget(state) : null;
    if (liveIntelOnPlayer && target) {
      const result = startAssassinationOperation(state, "enemy", "player", target.id);
      if (result.ok) {
        const currentSecond = Math.floor(state.meta.elapsed);
        if (darkExtremesSourceFocused && enemy.ai.lastDarkBacklashSecond !== currentSecond) {
          const enemyName = getFactionDisplayName(state, "enemy");
          const playerName = getFactionDisplayName(state, "player");
          state.messages.unshift({
            id: `enemy-dark-backlash-assassination-${currentSecond}`,
            text: `${enemyName} answers ${playerName}'s dark extremes with a bloodline decapitation strike.`,
            tone: "warn",
            ttl: 7,
          });
          enemy.ai.lastDarkBacklashSecond = currentSecond;
        }
        enemy.ai.assassinationTimer = 180;
        return;
      }
    }
    enemy.ai.assassinationTimer = liveCounterIntelDossierOnPlayer ? 50 : liveIntelOnPlayer ? 60 : 35;
  }

  enemy.ai.missionaryTimer = (enemy.ai.missionaryTimer ?? 70) - dt;
  if (holyWarSourceFocused) {
    enemy.ai.missionaryTimer = Math.min(enemy.ai.missionaryTimer, 8);
  }
  if (playerDivineRightActive) {
    enemy.ai.missionaryTimer = Math.min(enemy.ai.missionaryTimer, 6);
  }
  if (playerConvergencePressure.active) {
    enemy.ai.missionaryTimer = Math.min(enemy.ai.missionaryTimer, playerConvergencePressure.missionaryTimerCap ?? enemy.ai.missionaryTimer);
  }
  if (enemy.ai.missionaryTimer <= 0) {
    const enemyFaithId = enemy.faith?.selectedFaithId ?? null;
    const playerFaithId = state.factions.player.faith?.selectedFaithId ?? null;
    const liveMissionary = (enemy.dynasty?.operations?.active ?? []).some((operation) =>
      operation.type === "missionary" && operation.targetFactionId === "player");
    const canPressureFaith = Boolean(enemyFaithId) &&
      enemyFaithId !== playerFaithId &&
      (enemy.faith?.intensity ?? 0) >= 12;
    if (canPressureFaith && !liveMissionary) {
      const result = startMissionaryOperation(state, "enemy", "player");
      if (result.ok) {
        const currentSecond = Math.floor(state.meta.elapsed);
        if (holyWarSourceFocused && enemy.ai.lastFaithBacklashSecond !== currentSecond) {
          const enemyName = getFactionDisplayName(state, "enemy");
          const playerName = getFactionDisplayName(state, "player");
          state.messages.unshift({
            id: `enemy-faith-backlash-missionary-${currentSecond}`,
            text: `${enemyName} answers ${playerName}'s holy war pressure with renewed missionary backlash.`,
            tone: "warn",
            ttl: 7,
          });
          enemy.ai.lastFaithBacklashSecond = currentSecond;
        }
        enemy.ai.missionaryTimer = 170;
        return;
      }
    }
    enemy.ai.missionaryTimer = canPressureFaith ? 55 : 35;
  }

  enemy.ai.holyWarTimer = (enemy.ai.holyWarTimer ?? 95) - dt;
  if (holyWarSourceFocused) {
    enemy.ai.holyWarTimer = Math.min(enemy.ai.holyWarTimer, 10);
  }
  if (playerDivineRightActive) {
    enemy.ai.holyWarTimer = Math.min(enemy.ai.holyWarTimer, 8);
  }
  if (playerWorldPressureLevel > 0) {
    enemy.ai.holyWarTimer = Math.min(enemy.ai.holyWarTimer, playerWorldPressureLevel >= 2 ? 14 : 24);
  }
  if (playerConvergencePressure.active) {
    enemy.ai.holyWarTimer = Math.min(enemy.ai.holyWarTimer, playerConvergencePressure.holyWarTimerCap ?? enemy.ai.holyWarTimer);
  }
  if (enemy.ai.holyWarTimer <= 0) {
    const faithCompatibility = getMarriageFaithCompatibilityProfile(state, "enemy", "player");
    const playerWeakestLoyalty = playerSnapshot.territories?.details?.reduce(
      (min, controlPoint) => Math.min(min, controlPoint.loyalty ?? 100),
      100,
    ) ?? 100;
    const activeHolyWar = (enemy.faith?.activeHolyWars ?? []).some((entry) =>
      entry.targetFactionId === "player" && (entry.expiresAt ?? 0) > state.meta.elapsed);
    const tensionSignals =
      Number((enemy.hostileTo ?? []).includes("player")) +
      Number(playerWeakestLoyalty < 68) +
      Number((playerSnapshot.dynasty?.legitimacy ?? 100) < 75) +
      Number(playerWorldPressureLevel > 0);
    const canDeclareHolyWar =
      Boolean(enemy.faith?.selectedFaithId) &&
      Boolean(state.factions.player.faith?.selectedFaithId) &&
      faithCompatibility.tier !== "harmonious" &&
      (enemy.faith?.intensity ?? 0) >= 18 &&
      tensionSignals > 0;
    if (!activeHolyWar && canDeclareHolyWar) {
      const result = startHolyWarDeclaration(state, "enemy", "player");
      if (result.ok) {
        const currentSecond = Math.floor(state.meta.elapsed);
        if (holyWarSourceFocused && enemy.ai.lastFaithBacklashSecond !== currentSecond) {
          const enemyName = getFactionDisplayName(state, "enemy");
          const playerName = getFactionDisplayName(state, "player");
          state.messages.unshift({
            id: `enemy-faith-backlash-holy-war-${currentSecond}`,
            text: `${enemyName} answers ${playerName}'s holy war pressure with a counter-holy-war declaration.`,
            tone: "warn",
            ttl: 7,
          });
          enemy.ai.lastFaithBacklashSecond = currentSecond;
        }
        enemy.ai.holyWarTimer = 240;
        return;
      }
    }
    enemy.ai.holyWarTimer = canDeclareHolyWar ? 75 : 45;
  }

  enemy.ai.divineRightTimer = (enemy.ai.divineRightTimer ?? 140) - dt;
  if (enemy.ai.divineRightTimer <= 0) {
    const divineRightTerms = getDivineRightDeclarationTerms(state, "enemy");
    if (divineRightTerms.available) {
      const result = startDivineRightDeclaration(state, "enemy");
      if (result.ok) {
        enemy.ai.divineRightTimer = 260;
        return;
      }
    }
    enemy.ai.divineRightTimer = divineRightTerms.available ? 120 : 55;
  }

  enemy.ai.captiveRecoveryTimer = (enemy.ai.captiveRecoveryTimer ?? 60) - dt;
  if (captivesSourceFocused) {
    enemy.ai.captiveRecoveryTimer = Math.min(enemy.ai.captiveRecoveryTimer, 6);
  }
  if (enemy.ai.captiveRecoveryTimer <= 0) {
    const captiveTarget = pickAiCaptiveRecoveryTarget(state);
    if (captiveTarget) {
      const highPriorityRecovery = ["head_of_bloodline", "heir_designate", "commander"].includes(captiveTarget.roleId);
      const prefersRescue = highPriorityRecovery || (enemy.hostileTo ?? []).includes("player");
      const recoveryAttempts = prefersRescue
        ? [
          { type: "rescue", run: () => startRescueOperation(state, "enemy", captiveTarget.id), cooldown: 170 },
          { type: "ransom", run: () => startRansomNegotiation(state, "enemy", captiveTarget.id), cooldown: 140 },
        ]
        : [
          { type: "ransom", run: () => startRansomNegotiation(state, "enemy", captiveTarget.id), cooldown: 140 },
          { type: "rescue", run: () => startRescueOperation(state, "enemy", captiveTarget.id), cooldown: 170 },
        ];
      for (const attempt of recoveryAttempts) {
        const result = attempt.run();
        if (result.ok) {
          const currentSecond = Math.floor(state.meta.elapsed);
          if (captivesSourceFocused && enemy.ai.lastCaptiveBacklashSecond !== currentSecond) {
            const enemyName = getFactionDisplayName(state, "enemy");
            const playerName = getFactionDisplayName(state, "player");
            state.messages.unshift({
              id: `enemy-captive-backlash-${attempt.type}-${currentSecond}`,
              text: attempt.type === "rescue"
                ? `${enemyName} answers ${playerName}'s captive-taking with a covert recovery strike for ${captiveTarget.title}.`
                : `${enemyName} answers ${playerName}'s captive-taking with immediate ransom terms for ${captiveTarget.title}.`,
              tone: "warn",
              ttl: 7,
            });
            enemy.ai.lastCaptiveBacklashSecond = currentSecond;
          }
          enemy.ai.captiveRecoveryTimer = attempt.cooldown;
          return;
        }
      }
      enemy.ai.captiveRecoveryTimer = 35;
      return;
    }
    enemy.ai.captiveRecoveryTimer = 55;
  }

  // Session 36: AI marriage proposal logic. Closes the marriage system reciprocity
  // gap from Sessions 33-34. Stonehelm proposes to Player when it is materially
  // pressured AND has not already sent a pending proposal AND no marriage between
  // the two factions is already in force. Canonical: marriage is a strategic
  // de-escalation tool, not chatter — only fired when the AI actually benefits.
  enemy.ai.marriageProposalTimer = (enemy.ai.marriageProposalTimer ?? 90) - dt;
  if (enemy.ai.marriageProposalTimer <= 0) {
    const result = tryAiMarriageProposal(state);
    if (result === "proposed") {
      enemy.ai.marriageProposalTimer = 240; // long cooldown after a real proposal
      return;
    }
    enemy.ai.marriageProposalTimer = 60; // short retry on declined gate
  }

  // Session 37: AI processes its OWN marriage inbox. Closes the marriage system
  // reciprocity gap fully — until now, player proposals to AI sat indefinitely.
  // Cooldown separate from proposal cooldown so accept can fire on the very
  // next AI tick after a proposal lands. Accept gate reuses the strategic
  // criteria from tryAiMarriageProposal so AI behavior is symmetric: AI
  // accepts what AI would have proposed.
  enemy.ai.marriageInboxTimer = (enemy.ai.marriageInboxTimer ?? 30) - dt;
  if (enemy.ai.marriageInboxTimer <= 0) {
    const accepted = tryAiAcceptIncomingMarriage(state);
    enemy.ai.marriageInboxTimer = accepted ? 180 : 30;
  }

  // Session 91: AI non-aggression pact proposal. Stonehelm proposes a pact to
  // the player when the enemy is under sufficient pressure that peace benefits
  // it: active succession crisis, very low army, or the player's governance
  // recognition is approaching victory. This creates interesting gameplay: the
  // enemy offers peace when the player is winning, giving a diplomatic choice.
  enemy.ai.pactProposalTimer = (enemy.ai.pactProposalTimer ?? 120) - dt;
  if (enemy.ai.pactProposalTimer <= 0) {
    const pactTerms = getNonAggressionPactTerms(state, "enemy", "player");
    if (pactTerms.available) {
      const enemyArmyCount = state.units.filter((u) =>
        u.factionId === "enemy" && u.health > 0 &&
        !["worker", "engineer-specialist", "support"].includes(getUnitDef(state, u.typeId)?.role)).length;
      const underSuccessionPressure = Boolean(enemySuccessionCrisis);
      const underArmyPressure = enemyArmyCount <= 3;
      const playerGovernanceNearVictory = Boolean(
        playerActiveTerritorialGovernanceRecognition?.recognized &&
        !playerActiveTerritorialGovernanceRecognition?.completed,
      );
      const shouldPropose = underSuccessionPressure || underArmyPressure || playerGovernanceNearVictory;
      if (shouldPropose) {
        const result = proposeNonAggressionPact(state, "enemy", "player");
        if (result.ok) {
          enemy.ai.pactProposalTimer = 300;
          return;
        }
      }
    }
    enemy.ai.pactProposalTimer = 90;
  }

  // Session 38: AI lesser-house promotion. Mirrors the player-side pipeline
  // from Session 35. Stonehelm's own dynasty is registered for candidates by
  // the same `tickLesserHouseCandidates` simulation tick. Here the AI consents
  // on the player's behalf — auto-promotes its first eligible candidate when
  // legitimacy is below 90 (canonical: legitimacy is the strategic motivator,
  // an already-secure dynasty has no urgency to expand cadet branches).
  enemy.ai.lesserHousePromotionTimer = (enemy.ai.lesserHousePromotionTimer ?? 60) - dt;
  if (enemy.ai.lesserHousePromotionTimer <= 0) {
    const promoted = tryAiPromoteLesserHouse(state);
    enemy.ai.lesserHousePromotionTimer = promoted ? 180 : 45;
  }
}

export function updateMinorHouseAi(state, dt) {
  if (state.meta.status !== "playing") {
    return;
  }

  Object.values(state.factions)
    .filter((faction) => faction?.kind === "minor_house")
    .forEach((minor) => {
      const ai = ensureMinorHouseAi(minor);
      ai.defenseTimer = Math.max(0, (ai.defenseTimer ?? 0) - dt);
      ai.regroupTimer = Math.max(0, (ai.regroupTimer ?? 0) - dt);
      const pressureOpportunity = getMinorHousePressureOpportunityProfile(state, minor);
      ai.parentPressureLevel = pressureOpportunity.level ?? 0;
      ai.parentPressureStatus = pressureOpportunity.status ?? "quiet";
      ai.parentPressureLevyTempo = Math.round((pressureOpportunity.levyTempoMultiplier ?? 1) * 100) / 100;
      ai.parentPressureRetakeTempo = Math.round((pressureOpportunity.retakeTempoMultiplier ?? 1) * 100) / 100;
      ai.parentPressureRetinueBonus = pressureOpportunity.retinueCapBonus ?? 0;

      const claim = getMinorHouseClaim(state, minor.id);
      const claimPosition = claim ? getControlPointPosition(state, claim) : null;
      const combatUnits = getCombatArmy(state, minor.id);
      const founderUnitId = minor.dynasty?.attachments?.commanderUnitId ?? null;
      const founder = combatUnits.find((unit) => unit.id === founderUnitId) ?? combatUnits[0] ?? null;
      const rallyPoint = claimPosition ?? (founder ? { x: founder.x, y: founder.y } : null);

      if (!rallyPoint) {
        ai.localDefenseStatus = "shattered";
        return;
      }

      const threatRadius = (claim?.ownerFactionId === minor.id ? 220 : 300) + (pressureOpportunity.threatRadiusBonus ?? 0);
      const threat = getNearestHostileCombatUnit(state, minor.id, rallyPoint, threatRadius);
      const currentSecond = Math.floor(state.meta.elapsed);

      if (threat && combatUnits.length > 0 && ai.defenseTimer <= 0) {
        issueAttackCommand(state, combatUnits.map((unit) => unit.id), "unit", threat.id);
        ai.defenseTimer = Math.max(1, 2.5 / (pressureOpportunity.retakeTempoMultiplier ?? 1));
        ai.claimAlertUntil = state.meta.elapsed + 12;
        ai.localDefenseStatus = claim?.ownerFactionId === minor.id ? "defending" : "retaking";
        if (claim?.ownerFactionId === minor.id) {
          if (ai.lastAlertSecond !== currentSecond) {
            state.messages.unshift({
              id: `minor-house-alert-${minor.id}-${currentSecond}`,
              text: `${minor.displayName} rallies its retinue to defend the breakaway march.`,
              tone: "warn",
              ttl: 7,
            });
            ai.lastAlertSecond = currentSecond;
          }
        } else if (ai.lastRetakeSecond !== currentSecond) {
          state.messages.unshift({
            id: `minor-house-retake-${minor.id}-${currentSecond}`,
            text: `${minor.displayName} strikes back to retake its lost march.`,
            tone: "warn",
            ttl: 7,
          });
          ai.lastRetakeSecond = currentSecond;
        }
        return;
      }

      if (combatUnits.length === 0) {
        ai.localDefenseStatus = claim?.ownerFactionId === minor.id ? "disarmed" : "shattered";
        return;
      }

      const founderDistanceFromClaim = founder && claimPosition
        ? Math.hypot(founder.x - claimPosition.x, founder.y - claimPosition.y)
        : 0;
      const strayedUnitCount = combatUnits.filter((unit) =>
        Math.hypot(unit.x - rallyPoint.x, unit.y - rallyPoint.y) > 140).length;
      const needsRegroup = claim?.ownerFactionId !== minor.id || founderDistanceFromClaim > 115 || strayedUnitCount > 0;

      if (needsRegroup && ai.regroupTimer <= 0) {
        issueMoveCommand(state, combatUnits.map((unit) => unit.id), rallyPoint.x, rallyPoint.y);
        ai.regroupTimer = Math.max(2, 6 / (pressureOpportunity.retakeTempoMultiplier ?? 1));
        ai.localDefenseStatus = claim?.ownerFactionId === minor.id ? "holding" : "retaking";
        if (claim?.ownerFactionId === minor.id) {
          if (ai.lastRallySecond !== currentSecond) {
            state.messages.unshift({
              id: `minor-house-rally-${minor.id}-${currentSecond}`,
              text: `${minor.displayName} regroups around its claimed march.`,
              tone: "info",
              ttl: 6,
            });
            ai.lastRallySecond = currentSecond;
          }
        } else if (ai.lastRetakeSecond !== currentSecond) {
          state.messages.unshift({
            id: `minor-house-march-retake-${minor.id}-${currentSecond}`,
            text: `${minor.displayName} marches back toward its seized border claim.`,
            tone: "warn",
            ttl: 7,
          });
          ai.lastRetakeSecond = currentSecond;
        }
        return;
      }

      ai.localDefenseStatus = (ai.claimAlertUntil ?? 0) > state.meta.elapsed ? "alert" : "holding";
    });
}

function tryAiPromoteLesserHouse(state) {
  const enemy = state.factions.enemy;
  if (!enemy?.dynasty) return false;
  const candidates = enemy.dynasty.lesserHouseCandidates ?? [];
  if (candidates.length === 0) return false;
  // Strategic gate: only promote when legitimacy is below the consolidation
  // threshold. A secure house has no need to spawn cadet branches that dilute
  // the head's authority.
  const legitimacy = enemy.dynasty.legitimacy ?? 0;
  if (legitimacy >= 90) return false;
  // Cap total lesser houses so AI does not promote unboundedly.
  const existing = (enemy.dynasty.lesserHouses ?? []).length;
  if (existing >= 3) return false;
  // Promote the first eligible candidate.
  const firstId = candidates[0];
  const result = promoteMemberToLesserHouse(state, "enemy", firstId);
  return result.ok;
}

function getAiMarriageStrategicProfile(state) {
  const enemy = state.factions.enemy;
  const player = state.factions.player;
  const isHostile = (enemy.hostileTo ?? []).includes("player");
  const enemyPop = enemy.population?.total ?? 0;
  const playerPop = player.population?.total ?? 0;
  const populationDeficit = enemyPop > 0 && enemyPop < playerPop * 0.85;
  const legitimacyDistress = (enemy.dynasty?.legitimacy ?? 100) < AI_MARRIAGE_LEGITIMACY_DISTRESS_THRESHOLD;
  const ownSuccessionCrisis = (enemy.dynasty?.politicalEvents?.active ?? []).some((entry) => entry.type === "succession_crisis");
  const rivalSuccessionCrisis = (player.dynasty?.politicalEvents?.active ?? []).some((entry) => entry.type === "succession_crisis");
  const successionPressure = ownSuccessionCrisis || rivalSuccessionCrisis;
  const faithCompatibility = getMarriageFaithCompatibilityProfile(state, "enemy", "player");
  const faithBackedLegitimacySignal = legitimacyDistress && faithCompatibility.legitimacySignalAllowed;
  const signalCount = Number(isHostile) + Number(populationDeficit) + Number(faithBackedLegitimacySignal) + Number(successionPressure);
  const blockedByFaith = faithCompatibility.blocksWeakMatch && signalCount < 2;

  return {
    isHostile,
    populationDeficit,
    legitimacyDistress,
    ownSuccessionCrisis,
    rivalSuccessionCrisis,
    successionPressure,
    faithBackedLegitimacySignal,
    signalCount,
    blockedByFaith,
    willing: signalCount > 0 && !blockedByFaith,
    faithCompatibility,
    decision: blockedByFaith
      ? "blocked_by_faith"
      : faithBackedLegitimacySignal
        ? "faith_backed_legitimacy"
        : signalCount > 0
          ? "strategic_pressure"
          : "insufficient_pressure",
  };
}

function stampAiMarriageEvaluation(state, proposal, strategicProfile) {
  if (!proposal) return;
  proposal.aiEvaluation = {
    evaluatedAtInWorldDays: state.dualClock?.inWorldDays ?? 0,
    compatibilityTier: strategicProfile.faithCompatibility.tier,
    compatibilityLabel: strategicProfile.faithCompatibility.label,
    decision: strategicProfile.decision,
    signalCount: strategicProfile.signalCount,
    isHostile: strategicProfile.isHostile,
    populationDeficit: strategicProfile.populationDeficit,
    legitimacyDistress: strategicProfile.legitimacyDistress,
    ownSuccessionCrisis: strategicProfile.ownSuccessionCrisis,
    rivalSuccessionCrisis: strategicProfile.rivalSuccessionCrisis,
    faithBackedLegitimacySignal: strategicProfile.faithBackedLegitimacySignal,
    blockedByFaith: strategicProfile.blockedByFaith,
  };
}

function shouldAiRaiseCounterIntelligence(state) {
  const enemy = state.factions.enemy;
  const player = state.factions.player;
  if (!enemy?.dynasty || !player?.dynasty) {
    return false;
  }
  const activeWatch = (enemy.dynasty.counterIntelligence ?? []).some((entry) => (entry.expiresAt ?? 0) > state.meta.elapsed);
  const raisingWatch = (enemy.dynasty.operations?.active ?? []).some((operation) => operation.type === "counter_intelligence");
  if (activeWatch || raisingWatch) {
    return false;
  }
  const playerIntelOnEnemy = (player.dynasty.intelligenceReports ?? []).some((report) =>
    report.targetFactionId === "enemy" && (report.expiresAt ?? 0) > state.meta.elapsed);
  const activePlayerCovertPressure = (player.dynasty.operations?.active ?? []).some((operation) =>
    (operation.type === "espionage" || operation.type === "assassination") &&
    operation.targetFactionId === "enemy");
  const hostilitySignal = (enemy.hostileTo ?? []).includes("player");
  const legitimacyDistress = (enemy.dynasty.legitimacy ?? 100) < 78;
  return playerIntelOnEnemy || activePlayerCovertPressure || (hostilitySignal && legitimacyDistress);
}

function tryAiAcceptIncomingMarriage(state) {
  const enemy = state.factions.enemy;
  if (!enemy?.dynasty) return false;
  const inbox = enemy.dynasty.marriageProposalsIn ?? [];
  // Find the first pending proposal from the player faction.
  const proposal = inbox.find((p) => p.status === "pending" && p.sourceFactionId === "player");
  if (!proposal) return false;
  const strategicProfile = getAiMarriageStrategicProfile(state);
  if (!strategicProfile.willing) {
    stampAiMarriageEvaluation(state, proposal, strategicProfile);
    return false;
  }
  stampAiMarriageEvaluation(state, proposal, strategicProfile);
  const result = acceptMarriage(state, proposal.id);
  return result.ok;
}

function tryAiMarriageProposal(state) {
  const enemy = state.factions.enemy;
  const player = state.factions.player;
  if (!enemy?.dynasty || !player?.dynasty) return "skip";

  // Skip if any in-force marriage between the two factions already exists.
  const alreadyMarried = (enemy.dynasty.marriages ?? []).some(
    (m) => m.spouseFactionId === "player" && !m.dissolvedAt,
  );
  if (alreadyMarried) return "skip";

  // Skip if a pending proposal is already in player's inbox from enemy.
  const alreadyPending = (player.dynasty.marriageProposalsIn ?? []).some(
    (p) => p.sourceFactionId === "enemy" && p.status === "pending",
  );
  if (alreadyPending) return "skip";

  // Session 52: AI marriage pressure is now faith-aware. Hostility and population
  // deficit remain live strategic triggers, but committed covenant identity can
  // either open a legitimacy-repair path or harden a weak match into refusal.
  const strategicProfile = getAiMarriageStrategicProfile(state);
  if (!strategicProfile.willing) return "skip";

  // Find a non-head, available enemy member to send.
  const enemyCandidate = enemy.dynasty.members.find(
    (m) => m.roleId !== "head_of_bloodline" &&
      (m.status === "active" || m.status === "ruling") &&
      !m.capturedByFactionId,
  );
  if (!enemyCandidate) return "skip";

  // Find an available player member to receive.
  const playerCandidate = player.dynasty.members.find(
    (m) => m.roleId !== "head_of_bloodline" &&
      (m.status === "active" || m.status === "ruling") &&
      !m.capturedByFactionId,
  );
  if (!playerCandidate) return "skip";

  const result = proposeMarriage(state, "enemy", enemyCandidate.id, "player", playerCandidate.id);
  if (result.ok) {
    const proposal = player.dynasty.marriageProposalsIn.find((entry) => entry.id === result.proposalId)
      ?? enemy.dynasty.marriageProposalsOut.find((entry) => entry.id === result.proposalId)
      ?? null;
    stampAiMarriageEvaluation(state, proposal, strategicProfile);
  }
  return result.ok ? "proposed" : "skip";
}

function pickAiSabotageTarget(state, dossier = null) {
  if (dossier) {
    const dossierProfile = getDossierBackedSabotageProfile(state, "enemy", "player", dossier);
    if (dossierProfile.available) {
      const targetBuilding = state.buildings.find((building) => building.id === dossierProfile.targetBuildingId) ?? null;
      if (targetBuilding) {
        return {
          subtype: dossierProfile.subtype,
          building: targetBuilding,
          intelligenceReportId: dossierProfile.intelligenceReportId,
          intelligenceSupportBonus: dossierProfile.intelligenceSupportBonus,
          retaliationReason: dossierProfile.retaliationReason,
          retaliationInterceptType: dossierProfile.interceptType ?? null,
        };
      }
    }
  }

  const playerBuildings = state.buildings.filter(
    (b) => b.factionId === "player" && b.health > 0 && b.completed,
  );
  if (playerBuildings.length === 0) return null;
  const supplyCamp = playerBuildings.find((b) => b.typeId === "supply_camp");
  if (supplyCamp) return { subtype: "supply_poisoning", building: supplyCamp };
  const gatehouse = playerBuildings.find((b) => b.typeId === "gatehouse");
  if (gatehouse) return { subtype: "gate_opening", building: gatehouse };
  const well = playerBuildings.find((b) => b.typeId === "well");
  if (well) return { subtype: "well_poisoning", building: well };
  const farm = playerBuildings.find((b) => b.typeId === "farm");
  if (farm) return { subtype: "fire_raising", building: farm };
  const barracks = playerBuildings.find((b) => b.typeId === "barracks");
  if (barracks) return { subtype: "fire_raising", building: barracks };
  return null;
}

function pickAiAssassinationTarget(state) {
  const player = state.factions.player;
  if (!player?.dynasty) return null;

  const members = (player.dynasty.members ?? []).filter((member) =>
    (member.status === "active" || member.status === "ruling") &&
    !member.capturedByFactionId);
  if (members.length === 0) return null;

  const rolePriority = {
    commander: 0,
    heir_designate: 1,
    head_of_bloodline: 2,
    governor: 3,
    spymaster: 4,
    steward: 5,
  };

  return members
    .slice()
    .sort((left, right) => {
      const leftPriority = rolePriority[left.roleId] ?? 8;
      const rightPriority = rolePriority[right.roleId] ?? 8;
      if (leftPriority !== rightPriority) {
        return leftPriority - rightPriority;
      }
      return (right.renown ?? 0) - (left.renown ?? 0);
    })[0] ?? null;
}

function pickAiCaptiveRecoveryTarget(state) {
  const enemy = state.factions.enemy;
  if (!enemy?.dynasty) return null;

  const capturedMembers = (enemy.dynasty.members ?? []).filter((member) =>
    member.status === "captured" && member.capturedByFactionId === "player");
  if (capturedMembers.length === 0) {
    return null;
  }

  const rolePriority = {
    head_of_bloodline: 0,
    heir_designate: 1,
    commander: 2,
    governor: 3,
    spymaster: 4,
    steward: 5,
    diplomat: 6,
    merchant: 7,
  };

  return capturedMembers
    .slice()
    .sort((left, right) => {
      const leftPriority = rolePriority[left.roleId] ?? 8;
      const rightPriority = rolePriority[right.roleId] ?? 8;
      if (leftPriority !== rightPriority) {
        return leftPriority - rightPriority;
      }
      return (right.renown ?? 0) - (left.renown ?? 0);
    })[0] ?? null;
}

export function updateNeutralAi(state, dt) {
  const tribes = state.factions.tribes;
  if (!tribes?.ai || state.meta.status !== "playing") {
    return;
  }

  tribes.ai.raidTimer -= dt;
  if (tribes.ai.raidTimer > 0) {
    return;
  }

  const raiders = state.units.filter((unit) =>
    unit.factionId === "tribes" &&
    unit.health > 0 &&
    state.content.byId.units[unit.typeId].role !== "worker",
  );
  if (raiders.length === 0) {
    tribes.ai.raidTimer = 24;
    return;
  }

  const camp = findBuilding(state, "tribes", "tribal_camp");
  const campCenter = camp ? getBuildingCenter(state, camp) : { x: state.world.width * state.world.tileSize / 2, y: state.world.height * state.world.tileSize / 2 };
  const worldPressureLeader = getWorldPressureLeaderProfile(state);
  const worldPressureConvergence = worldPressureLeader.factionId
    ? getWorldPressureConvergenceProfile(state, worldPressureLeader.factionId)
    : { active: false, tribalRaidTimerMultiplier: 1 };
  const worldPressureSources = worldPressureLeader.factionId
    ? getWorldPressureSourceBreakdown(state, worldPressureLeader.factionId)
    : { topSourceId: "quiet", topSourceLabel: "quiet" };
  const pressureTargetMarch = getWorldPressureRaidTarget(state, worldPressureLeader.factionId, campCenter);
  const contestedMarch = state.world.controlPoints
    .filter((controlPoint) => controlPoint.ownerFactionId !== "tribes")
    .sort((left, right) => {
      const leftPoint = getControlPointPosition(state, left);
      const rightPoint = getControlPointPosition(state, right);
      return Math.hypot(campCenter.x - leftPoint.x, campCenter.y - leftPoint.y) - Math.hypot(campCenter.x - rightPoint.x, campCenter.y - rightPoint.y);
    })[0];
  const raidTarget = pressureTargetMarch ?? contestedMarch ?? null;

  if (raidTarget) {
    const destination = getControlPointPosition(state, raidTarget);
    issueMoveCommand(state, raiders.map((unit) => unit.id), destination.x, destination.y);
  } else {
    const enemyHall = findBuilding(state, "player", "command_hall") ?? findBuilding(state, "enemy", "command_hall");
    if (enemyHall) {
      const destination = getBuildingCenter(state, enemyHall);
      issueMoveCommand(state, raiders.map((unit) => unit.id), destination.x, destination.y);
    }
  }

  const currentSecond = Math.floor(state.meta.elapsed);
  if (tribes.ai.lastAlertSecond !== currentSecond) {
    const pressureTargetFaction = worldPressureLeader.factionId
      ? state.factions[worldPressureLeader.factionId]
      : null;
    const pressureTargetName = pressureTargetFaction
      ? state.content.byId.houses[pressureTargetFaction.houseId]?.name ?? pressureTargetFaction.id
      : null;
    const raidText = pressureTargetMarch && pressureTargetName && worldPressureSources.topSourceId === "greatReckoning"
      ? `Frontier tribes surge on ${pressureTargetName} marches as the Great Reckoning turns broad response into live frontier war.`
      : pressureTargetMarch && pressureTargetName && worldPressureSources.topSourceId === "offHomeHoldings"
      ? `Frontier tribes strike ${pressureTargetName}'s off-home marches as continental overextension sharpens world pressure.`
      : pressureTargetMarch && pressureTargetName && worldPressureSources.topSourceId === "territoryExpansion"
        ? `Frontier tribes strike ${pressureTargetName}'s stretched marches as territorial expansion sharpens world pressure.`
      : pressureTargetMarch && pressureTargetName && worldPressureConvergence.active
        ? `Frontier tribes surge on ${pressureTargetName} marches as world pressure reaches convergence.`
        : pressureTargetMarch && pressureTargetName
          ? `Frontier tribes converge on ${pressureTargetName} marches as world pressure sharpens.`
          : "Frontier tribes are raiding the marches.";
    state.messages.unshift({
      id: `tribal-raid-${currentSecond}`,
      text: raidText,
      tone: "warn",
      ttl: 7,
    });
    tribes.ai.lastAlertSecond = currentSecond;
  }

  // Session 19: Dark-extremes world pressure — tribes raid more aggressively
  // when a kingdom faction has sustained Apex Cruel (darkExtremesActive true).
  // Canonical: the world reacts to dynastic atrocity.
  const darkExtremesKingdoms = Object.values(state.factions).filter(
    (f) => f.kind === "kingdom" && f.darkExtremesActive,
  );
  const darkExtremesMultiplier = darkExtremesKingdoms.length > 0 ? 0.6 : 1;
  const worldPressureMultiplier = worldPressureConvergence.active
    ? worldPressureConvergence.tribalRaidTimerMultiplier ?? 0.3
    : worldPressureLeader.level >= 3
      ? 0.45
      : worldPressureLeader.level >= 2
        ? 0.6
        : worldPressureLeader.level >= 1
          ? 0.75
          : 1;
  const greatReckoningMultiplier = worldPressureSources.topSourceId === "greatReckoning" ? 0.5 : 1;
  tribes.ai.raidTimer = 30 * Math.min(darkExtremesMultiplier, worldPressureMultiplier, greatReckoningMultiplier);
}
