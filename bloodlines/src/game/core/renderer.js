import { formatResourceValue } from "./utils.js";

function buildingRect(state, building, buildingDef, camera) {
  const size = state.world.tileSize;
  return {
    x: building.tileX * size - camera.x,
    y: building.tileY * size - camera.y,
    w: buildingDef.footprint.w * size,
    h: buildingDef.footprint.h * size,
  };
}

function drawHealthBar(context, x, y, width, ratio, goodColor, backColor) {
  context.fillStyle = backColor;
  context.fillRect(x, y, width, 4);
  context.fillStyle = goodColor;
  context.fillRect(x, y, width * Math.max(0, ratio), 4);
}

function getFactionPresentation(state, factionId) {
  return state.factions[factionId]?.presentation ?? { primaryColor: "#777777", accentColor: "#d0b188" };
}

function withAlpha(hexColor, alpha) {
  if (!hexColor?.startsWith("#") || (hexColor.length !== 7 && hexColor.length !== 4)) {
    return `rgba(208, 177, 136, ${alpha})`;
  }

  const normalized = hexColor.length === 4
    ? `#${hexColor[1]}${hexColor[1]}${hexColor[2]}${hexColor[2]}${hexColor[3]}${hexColor[3]}`
    : hexColor;
  const red = Number.parseInt(normalized.slice(1, 3), 16);
  const green = Number.parseInt(normalized.slice(3, 5), 16);
  const blue = Number.parseInt(normalized.slice(5, 7), 16);
  return `rgba(${red}, ${green}, ${blue}, ${alpha})`;
}

export function createRenderer(canvas, minimapCanvas) {
  const context = canvas.getContext("2d");
  const minimapContext = minimapCanvas.getContext("2d");

  function drawTerrain(state, camera) {
    context.fillStyle = "#18130f";
    context.fillRect(0, 0, canvas.width, canvas.height);

    context.strokeStyle = "rgba(216, 175, 102, 0.06)";
    context.lineWidth = 1;
    for (let x = 0; x <= state.world.width; x += 1) {
      const worldX = x * state.world.tileSize - camera.x;
      context.beginPath();
      context.moveTo(worldX, 0);
      context.lineTo(worldX, canvas.height);
      context.stroke();
    }
    for (let y = 0; y <= state.world.height; y += 1) {
      const worldY = y * state.world.tileSize - camera.y;
      context.beginPath();
      context.moveTo(0, worldY);
      context.lineTo(canvas.width, worldY);
      context.stroke();
    }

    state.world.terrainPatches.forEach((patch) => {
      const fill = patch.type === "forest"
        ? "rgba(39, 72, 49, 0.45)"
        : patch.type === "river"
          ? "rgba(33, 67, 84, 0.55)"
          : "rgba(101, 80, 54, 0.38)";
      context.fillStyle = fill;
      context.fillRect(
        patch.x * state.world.tileSize - camera.x,
        patch.y * state.world.tileSize - camera.y,
        patch.w * state.world.tileSize,
        patch.h * state.world.tileSize,
      );
    });
  }

  function drawResources(state, camera) {
    state.world.resourceNodes.forEach((node) => {
      if (node.amount <= 0) {
        return;
      }

      const x = node.x * state.world.tileSize - camera.x;
      const y = node.y * state.world.tileSize - camera.y;
      context.save();
      if (node.type === "gold") {
        context.fillStyle = "#d7a94d";
        context.beginPath();
        context.arc(x, y, 15, 0, Math.PI * 2);
        context.fill();
        context.strokeStyle = "#5f4716";
        context.lineWidth = 3;
        context.stroke();
      } else {
        context.fillStyle = "#2d5b35";
        context.fillRect(x - 10, y - 14, 20, 28);
        context.fillStyle = "#1b2816";
        context.fillRect(x - 4, y + 12, 8, 12);
      }
      context.restore();
    });
  }

  function drawControlPoints(state, camera) {
    state.world.controlPoints.forEach((controlPoint) => {
      const position = {
        x: controlPoint.x * state.world.tileSize - camera.x,
        y: controlPoint.y * state.world.tileSize - camera.y,
      };
      const radius = controlPoint.radiusTiles * state.world.tileSize;
      const ownerPresentation = controlPoint.ownerFactionId ? getFactionPresentation(state, controlPoint.ownerFactionId) : null;
      const claimantPresentation = controlPoint.captureFactionId ? getFactionPresentation(state, controlPoint.captureFactionId) : null;

      context.save();
      context.beginPath();
      context.fillStyle = ownerPresentation ? withAlpha(ownerPresentation.primaryColor, 0.22) : "rgba(216, 175, 102, 0.12)";
      context.strokeStyle = controlPoint.contested ? "#f09a53" : ownerPresentation?.accentColor ?? "#d8af66";
      context.lineWidth = controlPoint.contested ? 3 : 2;
      context.arc(position.x, position.y, radius, 0, Math.PI * 2);
      context.fill();
      context.stroke();

      context.fillStyle = ownerPresentation?.accentColor ?? "#e6c996";
      context.fillRect(position.x - 5, position.y - 28, 10, 24);
      context.beginPath();
      context.moveTo(position.x + 5, position.y - 28);
      context.lineTo(position.x + 18, position.y - 22);
      context.lineTo(position.x + 5, position.y - 16);
      context.closePath();
      context.fill();

      if (claimantPresentation && controlPoint.captureProgress > 0) {
        context.beginPath();
        context.strokeStyle = claimantPresentation.accentColor;
        context.lineWidth = 4;
        context.arc(
          position.x,
          position.y,
          radius + 6,
          -Math.PI / 2,
          -Math.PI / 2 + (Math.PI * 2 * Math.min(1, controlPoint.captureProgress / controlPoint.captureTime)),
        );
        context.stroke();
      }

      context.fillStyle = "#f4ead8";
      context.font = "11px IBM Plex Sans";
      context.fillText(controlPoint.name, position.x - radius * 0.55, position.y + radius + 16);
      context.font = "10px IBM Plex Sans";
      context.fillStyle = controlPoint.contested
        ? "#f0a35f"
        : ownerPresentation?.accentColor ?? "#c9b08a";
      context.fillText(
        controlPoint.contested
          ? "Contested"
          : controlPoint.ownerFactionId
            ? `${state.factions[controlPoint.ownerFactionId].displayName} ${Math.round(controlPoint.loyalty)}`
            : "Neutral territory",
        position.x - radius * 0.55,
        position.y + radius + 30,
      );
      context.restore();
    });
  }

  function drawSacredSites(state, camera) {
    state.world.sacredSites.forEach((site) => {
      const faith = state.content.byId.faiths[site.faithId];
      const x = site.x * state.world.tileSize - camera.x;
      const y = site.y * state.world.tileSize - camera.y;
      const radius = site.radiusTiles * state.world.tileSize;
      context.save();
      context.strokeStyle = "rgba(210, 180, 120, 0.7)";
      context.lineWidth = 1.5;
      context.setLineDash([6, 6]);
      context.beginPath();
      context.arc(x, y, radius, 0, Math.PI * 2);
      context.stroke();
      context.setLineDash([]);
      context.fillStyle = "rgba(244, 212, 138, 0.9)";
      context.fillRect(x - 6, y - 6, 12, 12);
      context.fillStyle = "#f0dfc3";
      context.font = "10px IBM Plex Sans";
      context.fillText(faith.name, x - radius * 0.45, y - radius - 6);
      context.restore();
    });
  }

  function drawBuildings(state, camera) {
    state.buildings.forEach((building) => {
      if (building.health <= 0) {
        return;
      }

      const presentation = getFactionPresentation(state, building.factionId);
      const buildingDef = state.content.byId.buildings[building.typeId];
      const rect = buildingRect(state, building, buildingDef, camera);
      context.fillStyle = presentation.primaryColor;
      context.globalAlpha = building.completed ? 0.95 : 0.45;
      context.fillRect(rect.x, rect.y, rect.w, rect.h);
      context.globalAlpha = 1;
      context.strokeStyle = presentation.accentColor;
      context.lineWidth = 2;
      context.strokeRect(rect.x, rect.y, rect.w, rect.h);
      drawHealthBar(context, rect.x, rect.y - 8, rect.w, building.health / buildingDef.health, "#88c08b", "rgba(0,0,0,0.45)");
      context.fillStyle = "#f3e6d3";
      context.font = "11px IBM Plex Sans";
      context.fillText(buildingDef.name, rect.x + 6, rect.y + 16);
    });
  }

  function drawUnits(state, camera, selectionIds) {
    state.units.forEach((unit) => {
      if (unit.health <= 0) {
        return;
      }

      const unitDef = state.content.byId.units[unit.typeId];
      const presentation = getFactionPresentation(state, unit.factionId);
      const drawX = unit.x - camera.x;
      const drawY = unit.y - camera.y;

      context.save();
      context.translate(drawX, drawY);

      if (selectionIds.includes(unit.id)) {
        context.strokeStyle = "#f4d48a";
        context.lineWidth = 2;
        context.beginPath();
        context.arc(0, 0, unit.radius + 5, 0, Math.PI * 2);
        context.stroke();
      }

      context.fillStyle = presentation.primaryColor;
      context.strokeStyle = presentation.accentColor;
      context.lineWidth = 2;

      if (unitDef.role === "worker") {
        context.fillRect(-9, -9, 18, 18);
        context.strokeRect(-9, -9, 18, 18);
      } else if (unitDef.role === "ranged") {
        context.beginPath();
        context.moveTo(0, -12);
        context.lineTo(11, 10);
        context.lineTo(-11, 10);
        context.closePath();
        context.fill();
        context.stroke();
      } else {
        context.beginPath();
        context.arc(0, 0, unit.radius, 0, Math.PI * 2);
        context.fill();
        context.stroke();
      }

      drawHealthBar(context, -unit.radius, -unit.radius - 10, unit.radius * 2, unit.health / unitDef.health, "#88c08b", "rgba(0,0,0,0.4)");
      context.restore();
    });
  }

  function drawProjectiles(state, camera) {
    state.projectiles.forEach((projectile) => {
      context.fillStyle = "#f0d38c";
      context.beginPath();
      context.arc(projectile.x - camera.x, projectile.y - camera.y, 3, 0, Math.PI * 2);
      context.fill();
    });
  }

  function drawSelectionBox(selectionBox) {
    if (!selectionBox) {
      return;
    }
    context.fillStyle = "rgba(243, 211, 140, 0.14)";
    context.strokeStyle = "rgba(243, 211, 140, 0.75)";
    context.lineWidth = 1;
    context.fillRect(selectionBox.x, selectionBox.y, selectionBox.w, selectionBox.h);
    context.strokeRect(selectionBox.x, selectionBox.y, selectionBox.w, selectionBox.h);
  }

  function drawMinimap(state, camera) {
    minimapContext.clearRect(0, 0, minimapCanvas.width, minimapCanvas.height);
    minimapContext.fillStyle = "#120f0d";
    minimapContext.fillRect(0, 0, minimapCanvas.width, minimapCanvas.height);

    const scaleX = minimapCanvas.width / (state.world.width * state.world.tileSize);
    const scaleY = minimapCanvas.height / (state.world.height * state.world.tileSize);

    state.world.terrainPatches.forEach((patch) => {
      minimapContext.fillStyle = patch.type === "forest"
        ? "rgba(45, 91, 53, 0.7)"
        : patch.type === "river"
          ? "rgba(50, 96, 119, 0.7)"
          : "rgba(115, 89, 58, 0.55)";
      minimapContext.fillRect(
        patch.x * state.world.tileSize * scaleX,
        patch.y * state.world.tileSize * scaleY,
        patch.w * state.world.tileSize * scaleX,
        patch.h * state.world.tileSize * scaleY,
      );
    });

    state.buildings.forEach((building) => {
      if (building.health <= 0) {
        return;
      }
      const presentation = getFactionPresentation(state, building.factionId);
      const def = state.content.byId.buildings[building.typeId];
      minimapContext.fillStyle = presentation.accentColor;
      minimapContext.fillRect(
        building.tileX * state.world.tileSize * scaleX,
        building.tileY * state.world.tileSize * scaleY,
        def.footprint.w * state.world.tileSize * scaleX,
        def.footprint.h * state.world.tileSize * scaleY,
      );
    });

    state.world.controlPoints.forEach((controlPoint) => {
      minimapContext.beginPath();
      minimapContext.fillStyle = controlPoint.ownerFactionId
        ? getFactionPresentation(state, controlPoint.ownerFactionId).primaryColor
        : "#d8af66";
      minimapContext.arc(
        controlPoint.x * state.world.tileSize * scaleX,
        controlPoint.y * state.world.tileSize * scaleY,
        4,
        0,
        Math.PI * 2,
      );
      minimapContext.fill();
    });

    state.units.forEach((unit) => {
      if (unit.health <= 0) {
        return;
      }
      const presentation = getFactionPresentation(state, unit.factionId);
      minimapContext.fillStyle = presentation.primaryColor;
      minimapContext.fillRect(unit.x * scaleX, unit.y * scaleY, 3, 3);
    });

    minimapContext.strokeStyle = "#f4d48a";
    minimapContext.lineWidth = 1;
    minimapContext.strokeRect(camera.x * scaleX, camera.y * scaleY, canvas.width * scaleX, canvas.height * scaleY);
  }

  return {
    render(state, uiState) {
      drawTerrain(state, uiState.camera);
      drawResources(state, uiState.camera);
      drawSacredSites(state, uiState.camera);
      drawControlPoints(state, uiState.camera);
      drawBuildings(state, uiState.camera);
      drawProjectiles(state, uiState.camera);
      drawUnits(state, uiState.camera, uiState.selectionIds);
      drawSelectionBox(uiState.selectionBox);
      drawMinimap(state, uiState.camera);
    },

    buildResourceBar(state) {
      const player = state.factions.player;
      const territoryCount = state.world.controlPoints.filter((controlPoint) => controlPoint.ownerFactionId === "player").length;
      const usedPopulation = state.units
        .filter((unit) => unit.factionId === "player" && unit.health > 0)
        .reduce((sum, unit) => sum + state.content.byId.units[unit.typeId].populationCost, 0);

      return [
        ["gold", formatResourceValue(player.resources.gold)],
        ["wood", formatResourceValue(player.resources.wood)],
        ["influence", formatResourceValue(player.resources.influence)],
        ["food", formatResourceValue(player.resources.food)],
        ["water", formatResourceValue(player.resources.water)],
        ["territory", `${territoryCount}/${state.world.controlPoints.length}`],
        ["available", `${player.population.total - usedPopulation - player.population.reserved}`],
        ["population", `${player.population.total}/${player.population.cap} pop`],
      ];
    },
  };
}
