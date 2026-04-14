import {
  attemptPlaceBuilding,
  chooseFaithCommitment,
  findOpenBuildingSite,
  getAvailablePopulation,
  getFactionSnapshot,
  issueAttackCommand,
  issueGatherCommand,
  issueMoveCommand,
  queueProduction,
} from "./simulation.js";

function findBuilding(state, factionId, typeId = null) {
  return state.buildings.find((building) =>
    building.factionId === factionId &&
    building.health > 0 &&
    (typeId ? building.typeId === typeId : true),
  );
}

function getIdleWorkers(state, factionId) {
  return state.units.filter((unit) =>
    unit.factionId === factionId &&
    unit.health > 0 &&
    state.content.byId.units[unit.typeId].role === "worker" &&
    !unit.command,
  );
}

function getNearestNode(state, resourceType, fromX, fromY) {
  return state.world.resourceNodes
    .filter((node) => node.type === resourceType && node.amount > 0)
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
  const def = state.content.byId.buildings[building.typeId];
  return {
    x: (building.tileX + def.footprint.w / 2) * state.world.tileSize,
    y: (building.tileY + def.footprint.h / 2) * state.world.tileSize,
  };
}

function getCombatArmy(state, factionId) {
  return state.units.filter((unit) =>
    unit.factionId === factionId &&
    unit.health > 0 &&
    state.content.byId.units[unit.typeId].role !== "worker",
  );
}

function pickTerritoryTarget(state, factionId, anchor) {
  return state.world.controlPoints
    .filter((controlPoint) => controlPoint.ownerFactionId !== factionId)
    .sort((left, right) => {
      const leftPoint = getControlPointPosition(state, left);
      const rightPoint = getControlPointPosition(state, right);
      const leftDistance = Math.hypot(anchor.x - leftPoint.x, anchor.y - leftPoint.y);
      const rightDistance = Math.hypot(anchor.x - rightPoint.x, anchor.y - rightPoint.y);
      const leftBias = left.ownerFactionId === "player" ? -140 : left.contested ? -70 : 0;
      const rightBias = right.ownerFactionId === "player" ? -140 : right.contested ? -70 : 0;
      return (leftDistance + leftBias) - (rightDistance + rightBias);
    })[0] ?? null;
}

export function updateEnemyAi(state, dt) {
  const enemy = state.factions.enemy;
  if (!enemy || state.meta.status !== "playing") {
    return;
  }

  enemy.ai.attackTimer -= dt;
  enemy.ai.buildTimer -= dt;
  enemy.ai.territoryTimer -= dt;

  const hall = findBuilding(state, "enemy", "command_hall");
  const playerHall = findBuilding(state, "player", "command_hall");
  if (!hall || !playerHall) {
    return;
  }

  const hallTileX = hall.tileX + 2;
  const hallTileY = hall.tileY + 2;
  const idleWorkers = getIdleWorkers(state, "enemy");
  idleWorkers.forEach((worker, index) => {
    const preferredType = index % 2 === 0 ? "gold" : "wood";
    const node = getNearestNode(state, preferredType, worker.x, worker.y) ?? getNearestNode(state, "gold", worker.x, worker.y);
    if (node) {
      issueGatherCommand(state, [worker.id], node.id);
    }
  });

  const snapshot = getFactionSnapshot(state, "enemy");
  if (!enemy.faith.selectedFaithId) {
    const availableFaith = snapshot.faith.exposures
      .filter((faith) => faith.availableToCommit)
      .sort((left, right) => right.exposure - left.exposure)[0];
    if (availableFaith) {
      chooseFaithCommitment(state, "enemy", availableFaith.id);
    }
  }
  const barracks = findBuilding(state, "enemy", "barracks");
  const houses = state.buildings.filter((building) => building.factionId === "enemy" && building.typeId === "dwelling" && building.health > 0);
  const farms = state.buildings.filter((building) => building.factionId === "enemy" && building.typeId === "farm" && building.health > 0);
  const wells = state.buildings.filter((building) => building.factionId === "enemy" && building.typeId === "well" && building.health > 0);

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

    enemy.ai.buildTimer = 6;
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
    if (snapshot.resources.gold >= 70 && snapshot.resources.wood >= 20) {
      queueProduction(state, barracks.id, "swordsman");
    } else if (snapshot.resources.gold >= 55 && snapshot.resources.wood >= 15) {
      queueProduction(state, barracks.id, "militia");
    }
  }

  const army = getCombatArmy(state, "enemy");

  if (enemy.ai.territoryTimer <= 0) {
    if (army.length >= 2) {
      const targetPoint = pickTerritoryTarget(state, "enemy", getBuildingCenter(state, hall));
      if (targetPoint) {
        const destination = getControlPointPosition(state, targetPoint);
        issueMoveCommand(state, army.slice(0, Math.min(4, army.length)).map((unit) => unit.id), destination.x, destination.y);
      }
    }

    enemy.ai.territoryTimer = 18;
  }

  if (enemy.ai.attackTimer <= 0) {

    if (army.length >= 3) {
      issueAttackCommand(state, army.map((unit) => unit.id), "building", playerHall.id);
      const currentSecond = Math.floor(state.meta.elapsed);
      if (enemy.ai.lastAlertSecond !== currentSecond) {
        state.messages.unshift({
          id: `enemy-wave-${currentSecond}`,
          text: "Stonehelm warband advancing on the Ironmark hall.",
          tone: "warn",
          ttl: 7,
        });
        enemy.ai.lastAlertSecond = currentSecond;
      }
    }

    enemy.ai.attackTimer = 38;
  }
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
  const contestedMarch = state.world.controlPoints
    .filter((controlPoint) => controlPoint.ownerFactionId !== "tribes")
    .sort((left, right) => {
      const leftPoint = getControlPointPosition(state, left);
      const rightPoint = getControlPointPosition(state, right);
      return Math.hypot(campCenter.x - leftPoint.x, campCenter.y - leftPoint.y) - Math.hypot(campCenter.x - rightPoint.x, campCenter.y - rightPoint.y);
    })[0];

  if (contestedMarch) {
    const destination = getControlPointPosition(state, contestedMarch);
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
    state.messages.unshift({
      id: `tribal-raid-${currentSecond}`,
      text: "Frontier tribes are raiding the marches.",
      tone: "warn",
      ttl: 7,
    });
    tribes.ai.lastAlertSecond = currentSecond;
  }

  tribes.ai.raidTimer = 30;
}
