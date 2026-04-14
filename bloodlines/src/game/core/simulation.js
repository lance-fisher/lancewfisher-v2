import { distance, moveToward, rectContainsPoint } from "./utils.js";

const MESSAGE_LIMIT = 6;
const GROWTH_INTERVAL_SECONDS = 18;
const CONTROL_POINT_CAPTURE_DECAY = 2.5;
const FAITH_EXPOSURE_THRESHOLD = 100;
const FAITH_INTENSITY_TIERS = [
  { level: 5, min: 80, label: "Apex" },
  { level: 4, min: 60, label: "Fervent" },
  { level: 3, min: 40, label: "Devout" },
  { level: 2, min: 20, label: "Active" },
  { level: 1, min: 1, label: "Latent" },
  { level: 0, min: 0, label: "Unawakened" },
];

function createEntityId(state, prefix) {
  state.counters[prefix] += 1;
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

function getFactionDisplayName(state, factionId) {
  return getFaction(state, factionId)?.displayName ?? factionId;
}

function getFactionPresentation(state, factionId) {
  return getFaction(state, factionId)?.presentation ?? { primaryColor: "#777777", accentColor: "#d0b188" };
}

function getFaith(content, faithId) {
  return content.byId.faiths[faithId];
}

function getConvictionBand(content, score) {
  return content.convictionStates.find((entry) => score >= entry.minScore) ?? content.convictionStates[content.convictionStates.length - 1];
}

function getFaithTier(intensity) {
  return FAITH_INTENSITY_TIERS.find((tier) => intensity >= tier.min) ?? FAITH_INTENSITY_TIERS[FAITH_INTENSITY_TIERS.length - 1];
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

function getAliveUnits(state, factionId) {
  return state.units.filter((unit) => unit.factionId === factionId && unit.health > 0);
}

function getAliveCombatUnits(state, factionId) {
  return getAliveUnits(state, factionId).filter((unit) => getUnitDef(state.content, unit.typeId).role !== "worker");
}

function getCompletedBuildings(state, factionId) {
  return state.buildings.filter((building) => building.factionId === factionId && building.completed && building.health > 0);
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

function getSacredSitePosition(state, sacredSite) {
  return {
    x: sacredSite.x * state.world.tileSize,
    y: sacredSite.y * state.world.tileSize,
  };
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
    members: templates.map((template, index) => ({
      id: `${factionId}-bloodline-${template.suffix}`,
      title: template.title,
      roleId: template.roleId,
      pathId: template.pathId,
      age: template.age,
      status: template.status,
      renown: template.renown,
      order: index,
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
  };
}

function createConvictionState(content) {
  const score = 0;
  const band = getConvictionBand(content, score);
  return {
    score,
    bandId: band.id,
    bandLabel: band.label,
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

function canAfford(faction, cost = {}) {
  return Object.entries(cost).every(([resourceId, amount]) => (faction.resources[resourceId] ?? 0) >= amount);
}

function spendResources(faction, cost = {}) {
  Object.entries(cost).forEach(([resourceId, amount]) => {
    faction.resources[resourceId] -= amount;
  });
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
    return def.dropoffResources?.includes(resourceType);
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

function applyDamage(state, entityType, entityId, amount, attackerFactionId) {
  const entity = getEntity(state, entityType, entityId);
  if (!entity || entity.health <= 0) {
    return;
  }

  entity.health -= amount;

  if (entity.health > 0) {
    return;
  }

  entity.health = 0;

  if (entityType === "unit") {
    const faction = getFaction(state, entity.factionId);
    const unitDef = getUnitDef(state.content, entity.typeId);
    faction.population.total = Math.max(0, faction.population.total - unitDef.populationCost);
    pushMessage(
      state,
      `${getFactionDisplayName(state, entity.factionId)} lost ${unitDef.name}.`,
      entity.factionId === "player" ? "warn" : "good",
    );
    return;
  }

  const faction = getFaction(state, entity.factionId);
  const buildingDef = getBuildingDef(state.content, entity.typeId);
  pushMessage(
    state,
    `${getFactionDisplayName(state, entity.factionId)} lost ${buildingDef.name}.`,
    entity.factionId === "player" ? "warn" : "good",
  );

  if (entity.typeId === "command_hall" && state.meta.status === "playing") {
    state.meta.status = entity.factionId === "player" ? "lost" : "won";
    state.meta.winnerId = attackerFactionId;
    pushMessage(
      state,
      entity.factionId === "player"
        ? "The Ironmark Command Hall has fallen. The frontier is lost."
        : "The Stonehelm Command Hall is broken. Ironmark holds the field.",
      entity.factionId === "player" ? "warn" : "good",
    );
  }
}

function spawnUnitAtBuilding(state, factionId, building, unitId) {
  const buildingDef = getBuildingDef(state.content, building.typeId);
  const center = getBuildingCenter(state, building, buildingDef);
  const offset = 22 + (state.counters.unit % 3) * 10;
  const nextUnitId = createEntityId(state, "unit");
  const unitDef = getUnitDef(state.content, unitId);

  state.units.push({
    id: nextUnitId,
    factionId,
    typeId: unitId,
    x: center.x + offset,
    y: center.y + offset,
    radius: unitDef.role === "worker" ? 10 : 12,
    health: unitDef.health,
    attackCooldownRemaining: 0,
    gatherProgress: 0,
    carrying: null,
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
    if (!building.completed || building.health <= 0) {
      return;
    }

    const def = getBuildingDef(state.content, building.typeId);
    if (!def.resourceTrickle) {
      return;
    }

    const faction = getFaction(state, building.factionId);
    Object.entries(def.resourceTrickle).forEach(([resourceId, amount]) => {
      faction.resources[resourceId] += amount * dt;
    });
  });
}

function tickPopulationGrowth(state, dt) {
  Object.values(state.factions).forEach((faction) => {
    const used = getUsedPopulation(state, faction.id);
    const availableCap = Math.max(0, faction.population.cap - used - faction.population.reserved);

    if (
      faction.population.total < faction.population.cap &&
      availableCap > 0 &&
      faction.resources.food >= faction.population.total + 1 &&
      faction.resources.water >= faction.population.total + 1
    ) {
      faction.population.growthProgress += dt;

      if (faction.population.growthProgress >= GROWTH_INTERVAL_SECONDS) {
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

function findNearestEnemyInRange(state, unit, range) {
  const enemyUnits = state.units.filter((candidate) =>
    candidate.health > 0 &&
    areHostile(state, unit.factionId, candidate.factionId),
  );
  const enemyBuildings = state.buildings.filter((building) =>
    building.health > 0 &&
    areHostile(state, unit.factionId, building.factionId),
  );

  let best = null;

  enemyUnits.forEach((enemy) => {
    const candidateDistance = distance(unit.x, unit.y, enemy.x, enemy.y);
    if (candidateDistance <= range && (!best || candidateDistance < best.distance)) {
      best = { type: "unit", target: enemy, distance: candidateDistance };
    }
  });

  enemyBuildings.forEach((building) => {
    const def = getBuildingDef(state.content, building.typeId);
    const center = getBuildingCenter(state, building, def);
    const candidateDistance = distance(unit.x, unit.y, center.x, center.y);
    if (candidateDistance <= range && (!best || candidateDistance < best.distance)) {
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
      Object.entries(controlPoint.resourceTrickle ?? {}).forEach(([resourceId, amount]) => {
        owningFaction.resources[resourceId] += amount * dt;
      });
      controlPoint.loyalty = Math.min(100, controlPoint.loyalty + dt * 0.4);
    }

    if (controlPoint.contested || activeClaimants.length === 0) {
      controlPoint.captureProgress = Math.max(0, controlPoint.captureProgress - dt * CONTROL_POINT_CAPTURE_DECAY);
      if (activeClaimants.length === 0) {
        controlPoint.captureFactionId = null;
      }
      return;
    }

    const claimantId = activeClaimants[0];
    if (controlPoint.ownerFactionId === claimantId) {
      controlPoint.captureFactionId = null;
      controlPoint.captureProgress = 0;
      controlPoint.loyalty = Math.min(100, controlPoint.loyalty + dt * 1.6);
      return;
    }

    if (controlPoint.captureFactionId && controlPoint.captureFactionId !== claimantId) {
      controlPoint.captureProgress = Math.max(0, controlPoint.captureProgress - dt * 6);
      if (controlPoint.captureProgress > 0) {
        return;
      }
    }

    controlPoint.captureFactionId = claimantId;
    controlPoint.captureProgress += dt;
    controlPoint.loyalty = Math.max(0, controlPoint.loyalty - dt * 4.5);

    if (controlPoint.captureProgress < controlPoint.captureTime) {
      return;
    }

    controlPoint.ownerFactionId = claimantId;
    controlPoint.captureFactionId = null;
    controlPoint.captureProgress = 0;
    controlPoint.loyalty = 34;
    pushMessage(
      state,
      `${getFactionDisplayName(state, claimantId)} seized ${controlPoint.name}.`,
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
      const previousExposure = faction.faith.exposure[sacredSite.faithId] ?? 0;
      const nextExposure = Math.min(100, previousExposure + sacredSite.exposureRate * dt);
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
        const nextIntensity = Math.min(100, faction.faith.intensity + dt * 0.25);
        const tier = getFaithTier(nextIntensity);
        faction.faith.intensity = nextIntensity;
        faction.faith.level = tier.level;
        faction.faith.tierLabel = tier.label;
      }
    });
  });
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

    applyDamage(state, projectile.targetType, projectile.targetId, projectile.damage, projectile.factionId);
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

    building.buildProgress += dt * unitDef.buildRate;
    if (building.buildProgress >= def.buildTime) {
      building.completed = true;
      building.buildProgress = def.buildTime;
      building.health = def.health;
      unit.command = null;
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

  if (unit.carrying) {
    const depositTarget = getNearestDropoff(state, unit.factionId, unit.carrying.type, unit.x, unit.y);
    if (!depositTarget) {
      return;
    }
    const depositPosition = getBuildingCenter(state, depositTarget, getBuildingDef(state.content, depositTarget.typeId));
    const next = moveToward(unit.x, unit.y, depositPosition.x, depositPosition.y, unitDef.speed, dt);
    unit.x = next.x;
    unit.y = next.y;

    if (next.arrived) {
      faction.resources[unit.carrying.type] += unit.carrying.amount;
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

function updateCombatUnit(state, unit, unitDef, dt) {
  if (unit.attackCooldownRemaining > 0) {
    unit.attackCooldownRemaining -= dt;
  }

  if (unit.command?.type === "move") {
    const next = moveToward(unit.x, unit.y, unit.command.x, unit.command.y, unitDef.speed, dt);
    unit.x = next.x;
    unit.y = next.y;
    if (next.arrived) {
      unit.command = null;
    }
  }

  if (unit.command?.type === "attack") {
    const target = getEntity(state, unit.command.targetType, unit.command.targetId);
    if (!target || target.health <= 0) {
      unit.command = null;
      return;
    }

    const targetPosition = unit.command.targetType === "unit"
      ? { x: target.x, y: target.y }
      : getBuildingCenter(state, target, getBuildingDef(state.content, target.typeId));
    const attackDistance = distance(unit.x, unit.y, targetPosition.x, targetPosition.y);

    if (attackDistance > unitDef.attackRange) {
      const next = moveToward(unit.x, unit.y, targetPosition.x, targetPosition.y, unitDef.speed, dt);
      unit.x = next.x;
      unit.y = next.y;
      return;
    }

    if (unit.attackCooldownRemaining > 0) {
      return;
    }

    unit.attackCooldownRemaining = unitDef.attackCooldown;
    if (unitDef.attackRange > 50) {
      state.projectiles.push({
        id: createEntityId(state, "projectile"),
        factionId: unit.factionId,
        x: unit.x,
        y: unit.y,
        targetType: unit.command.targetType,
        targetId: unit.command.targetId,
        damage: unitDef.attackDamage,
        speed: unitDef.projectileSpeed ?? 260,
      });
    } else {
      applyDamage(state, unit.command.targetType, unit.command.targetId, unitDef.attackDamage, unit.factionId);
    }
    return;
  }

  const targetInfo = findNearestEnemyInRange(state, unit, unitDef.sight);
  if (targetInfo) {
    unit.command = { type: "attack", targetType: targetInfo.type, targetId: targetInfo.target.id };
  }
}

function updateUnits(state, dt) {
  state.units.forEach((unit) => {
    if (unit.health <= 0) {
      return;
    }
    const unitDef = getUnitDef(state.content, unit.typeId);
    if (unitDef.role === "worker") {
      updateWorker(state, unit, unitDef, dt);
    } else {
      updateCombatUnit(state, unit, unitDef, dt);
    }
  });

  state.units = state.units.filter((unit) => unit.health > 0);
  state.buildings = state.buildings.filter((building) => building.health > 0);
}

export function createSimulation(content) {
  const map = content.map;
  const baselineResources = Object.fromEntries(content.resources.map((resource) => [resource.id, 0]));
  const state = {
    content,
    meta: {
      status: "playing",
      winnerId: null,
      elapsed: 0,
    },
    counters: {
      unit: 0,
      building: 0,
      projectile: 0,
    },
    world: {
      id: map.id,
      name: map.name,
      width: map.width,
      height: map.height,
      tileSize: map.tileSize,
      terrainPatches: map.terrainPatches,
      resourceNodes: map.resourceNodes.map((node) => ({ ...node })),
      controlPoints: (map.controlPoints ?? []).map((controlPoint) => ({
        ...controlPoint,
        ownerFactionId: controlPoint.ownerFactionId ?? null,
        captureFactionId: null,
        captureProgress: 0,
        contested: false,
        loyalty: controlPoint.ownerFactionId ? 50 : 18,
      })),
      sacredSites: (map.sacredSites ?? []).map((sacredSite) => ({ ...sacredSite })),
    },
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

  pushMessage(state, "Ironmark foothold established. Secure the frontier.", "good");
  if (state.factions.tribes) {
    pushMessage(state, "Frontier tribes hold the central marches. Expect raids and territorial fights.", "info");
  }
  return state;
}

export function stepSimulation(state, dt) {
  if (state.meta.status !== "playing") {
    updateMessages(state, dt);
    return state;
  }

  state.meta.elapsed += dt;
  recalculatePopulationCaps(state);
  tickPassiveResources(state, dt);
  tickPopulationGrowth(state, dt);
  updateControlPoints(state, dt);
  updateFaithExposure(state, dt);
  updateBuildings(state, dt);
  updateUnits(state, dt);
  updateProjectiles(state, dt);
  updateMessages(state, dt);
  return state;
}

export function getAvailablePopulation(state, factionId) {
  const faction = getFaction(state, factionId);
  return faction.population.total - getUsedPopulation(state, factionId) - faction.population.reserved;
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
  if (!canAfford(faction, unitDef.cost)) {
    return { ok: false, reason: "Not enough resources." };
  }

  if (getAvailablePopulation(state, building.factionId) < unitDef.populationCost) {
    return { ok: false, reason: "No available population." };
  }

  spendResources(faction, unitDef.cost);
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
  selectedUnits.forEach((unit, index) => {
    const offset = selectedUnits.length > 1
      ? {
          x: ((index % 4) - 1.5) * 16,
          y: (Math.floor(index / 4) - 0.5) * 16,
        }
      : { x: 0, y: 0 };
    unit.command = { type: "move", x: x + offset.x, y: y + offset.y };
  });
}

export function issueGatherCommand(state, workerIds, nodeId) {
  state.units
    .filter((unit) => workerIds.includes(unit.id) && unit.health > 0)
    .forEach((unit) => {
      unit.command = { type: "gather", nodeId };
      unit.carrying = null;
      unit.gatherProgress = 0;
    });
}

export function issueAttackCommand(state, unitIds, targetType, targetId) {
  state.units
    .filter((unit) => unitIds.includes(unit.id) && unit.health > 0)
    .forEach((unit) => {
      unit.command = { type: "attack", targetType, targetId };
    });
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
  if (!canAfford(faction, buildingDef.cost)) {
    return { ok: false, reason: "Not enough resources." };
  }

  spendResources(faction, buildingDef.cost);
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
  });

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
  const territories = state.world.controlPoints.filter((controlPoint) => controlPoint.ownerFactionId === factionId);
  const convictionBand = getConvictionBand(state.content, faction.conviction.score);
  const selectedFaith = faction.faith.selectedFaithId ? getFaith(state.content, faction.faith.selectedFaithId) : null;
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
    },
    dynasty: faction.dynasty
      ? {
          ...faction.dynasty,
          members: faction.dynasty.members.map((member) => ({
            ...member,
            role: state.content.byId.bloodlineRoles[member.roleId],
            path: state.content.byId.bloodlinePaths[member.pathId],
          })),
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
      exposures: state.content.faiths.map((faith) => ({
        id: faith.id,
        name: faith.name,
        covenantName: faith.covenantName,
        exposure: faction.faith.exposure[faith.id] ?? 0,
        availableToCommit: (faction.faith.exposure[faith.id] ?? 0) >= FAITH_EXPOSURE_THRESHOLD,
      })),
    },
  };
}

export function chooseFaithCommitment(state, factionId, faithId) {
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
  faction.faith.doctrinePath = "light";
  faction.faith.intensity = 20;
  faction.faith.level = tier.level;
  faction.faith.tierLabel = tier.label;
  if (!faction.faith.discoveredFaithIds.includes(faithId)) {
    faction.faith.discoveredFaithIds.push(faithId);
  }

  const faith = getFaith(state.content, faithId);
  pushMessage(
    state,
    `${getFactionDisplayName(state, factionId)} aligned with ${faith.name}.`,
    factionId === "player" ? "good" : "info",
  );
  return { ok: true };
}
