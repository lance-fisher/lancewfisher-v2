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

function getSettlementClass(state, settlementClassId) {
  return state.content.byId.settlementClasses?.[settlementClassId] ?? null;
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
          : patch.type === "water"
            ? "rgba(28, 58, 88, 0.72)"
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
      } else if (node.type === "stone") {
        context.fillStyle = "#7b7f84";
        context.beginPath();
        context.arc(x, y, 15, 0, Math.PI * 2);
        context.fill();
        context.strokeStyle = "#464b53";
        context.lineWidth = 3;
        context.stroke();
        context.strokeStyle = "rgba(232, 236, 240, 0.38)";
        context.lineWidth = 1.5;
        context.beginPath();
        context.arc(x - 3, y - 2, 6, Math.PI * 1.1, Math.PI * 1.9);
        context.stroke();
      } else if (node.type === "iron") {
        context.fillStyle = "#6b3f1f";
        context.beginPath();
        context.arc(x, y, 15, 0, Math.PI * 2);
        context.fill();
        context.strokeStyle = "#b86a2a";
        context.lineWidth = 3;
        context.stroke();
        context.fillStyle = "#2d1a12";
        context.beginPath();
        context.arc(x, y, 6, 0, Math.PI * 2);
        context.fill();
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
      const settlementClass = getSettlementClass(state, controlPoint.settlementClass);
      const defensiveCeiling = settlementClass?.defensiveCeiling ?? 0;
      const outlineWidth = controlPoint.contested ? 3 : 1.75 + defensiveCeiling * 0.35;

      context.save();
      context.beginPath();
      context.fillStyle = ownerPresentation ? withAlpha(ownerPresentation.primaryColor, 0.22) : "rgba(216, 175, 102, 0.12)";
      context.strokeStyle = controlPoint.contested ? "#f09a53" : ownerPresentation?.accentColor ?? "#d8af66";
      context.lineWidth = outlineWidth;
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
            ? `${state.factions[controlPoint.ownerFactionId].displayName} ${controlPoint.controlState} ${Math.round(controlPoint.loyalty)}`
            : "Neutral territory",
        position.x - radius * 0.55,
        position.y + radius + 30,
      );
      if (controlPoint.governorMemberId) {
        context.fillStyle = ownerPresentation?.accentColor ?? "#d8af66";
        context.fillText("Governor seated", position.x - radius * 0.55, position.y + radius + 42);
      }
      context.font = "10px IBM Plex Sans";
      context.fillStyle = settlementClass ? "#d6c5a5" : "#b8ab95";
      context.fillText(
        `${settlementClass?.name ?? "Settlement"} • T${controlPoint.fortificationTier}/${defensiveCeiling}`,
        position.x - radius * 0.55,
        position.y + radius + (controlPoint.governorMemberId ? 54 : 42),
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
      context.save();
      context.globalAlpha = building.completed ? 0.96 : 0.52;

      if (buildingDef.typeId === "quarry" || building.typeId === "quarry") {
        context.fillStyle = "#787d84";
        context.fillRect(rect.x, rect.y + 4, rect.w, rect.h - 4);
        context.strokeStyle = "#434951";
        context.lineWidth = 2;
        context.strokeRect(rect.x, rect.y + 4, rect.w, rect.h - 4);
        context.fillStyle = "#cfd4db";
        context.fillRect(rect.x + 5, rect.y + 9, rect.w - 10, 5);
      } else if (building.typeId === "iron_mine") {
        context.fillStyle = "#4f3523";
        context.fillRect(rect.x, rect.y, rect.w, rect.h);
        context.strokeStyle = "#b86a2a";
        context.lineWidth = 2;
        context.strokeRect(rect.x, rect.y, rect.w, rect.h);
        context.fillStyle = "#211612";
        context.fillRect(rect.x + 6, rect.y + 6, rect.w - 12, rect.h - 12);
        context.fillStyle = presentation.accentColor;
        context.fillRect(rect.x + rect.w - 10, rect.y + 4, 4, rect.h - 8);
        } else if (building.typeId === "siege_workshop") {
          context.fillStyle = "#5b4634";
          context.fillRect(rect.x, rect.y + 6, rect.w, rect.h - 6);
          context.strokeStyle = "#c79258";
        context.lineWidth = 2;
        context.strokeRect(rect.x, rect.y + 6, rect.w, rect.h - 6);
        context.fillStyle = "#2e2219";
        context.fillRect(rect.x + 7, rect.y + 13, rect.w - 14, rect.h - 18);
        context.fillStyle = presentation.accentColor;
        context.fillRect(rect.x + 6, rect.y + 3, rect.w - 12, 4);
        context.strokeStyle = "#d9bb8e";
        context.lineWidth = 1.5;
        context.beginPath();
        context.moveTo(rect.x + rect.w - 10, rect.y + 8);
        context.lineTo(rect.x + rect.w - 10, rect.y - 8);
        context.lineTo(rect.x + rect.w - 2, rect.y - 4);
        context.stroke();
        context.fillStyle = "#d9bb8e";
        context.fillRect(rect.x + rect.w - 14, rect.y + 14, 6, rect.h - 18);
      } else if (building.typeId === "supply_camp") {
        context.fillStyle = "#65523d";
        context.fillRect(rect.x, rect.y + 8, rect.w, rect.h - 8);
        context.strokeStyle = "#d4a76c";
        context.lineWidth = 2;
        context.strokeRect(rect.x, rect.y + 8, rect.w, rect.h - 8);
        context.fillStyle = "#2d241c";
        context.fillRect(rect.x + 8, rect.y + 12, rect.w - 16, rect.h - 16);
        context.fillStyle = presentation.accentColor;
        context.beginPath();
        context.moveTo(rect.x + 7, rect.y + 8);
        context.lineTo(rect.x + rect.w * 0.45, rect.y - 2);
        context.lineTo(rect.x + rect.w * 0.45, rect.y + 8);
        context.closePath();
          context.fill();
          context.fillRect(rect.x + rect.w * 0.6, rect.y + 3, 4, rect.h - 2);
        } else if (building.typeId === "stable") {
          context.fillStyle = "#694a31";
          context.fillRect(rect.x, rect.y + 8, rect.w, rect.h - 8);
          context.strokeStyle = "#d5a36d";
          context.lineWidth = 2;
          context.strokeRect(rect.x, rect.y + 8, rect.w, rect.h - 8);
          context.fillStyle = "#3a2a1d";
          context.fillRect(rect.x + 6, rect.y + 14, rect.w - 12, rect.h - 18);
          context.fillStyle = presentation.accentColor;
          context.fillRect(rect.x + 5, rect.y + 5, rect.w - 10, 4);
          context.strokeStyle = "#f3e6d3";
          context.lineWidth = 1.5;
          context.beginPath();
          context.moveTo(rect.x + rect.w * 0.25, rect.y + 8);
          context.lineTo(rect.x + rect.w * 0.35, rect.y - 2);
          context.lineTo(rect.x + rect.w * 0.5, rect.y + 8);
          context.lineTo(rect.x + rect.w * 0.65, rect.y - 2);
          context.lineTo(rect.x + rect.w * 0.75, rect.y + 8);
          context.stroke();
          context.fillStyle = "#1f1a16";
          context.fillRect(rect.x + rect.w - 10, rect.y + 12, 4, rect.h - 16);
        } else if (buildingDef.fortificationRole === "wall") {
        context.fillStyle = "#69707a";
        context.fillRect(rect.x, rect.y + 6, rect.w, rect.h - 12);
        context.strokeStyle = "#404750";
        context.lineWidth = 2;
        context.strokeRect(rect.x, rect.y + 6, rect.w, rect.h - 12);
        context.fillStyle = presentation.accentColor;
        context.fillRect(rect.x, rect.y + 2, rect.w, 4);
        context.strokeStyle = "rgba(255, 255, 255, 0.16)";
        context.lineWidth = 1;
        for (let offset = rect.x + 8; offset < rect.x + rect.w; offset += 8) {
          context.beginPath();
          context.moveTo(offset, rect.y + 7);
          context.lineTo(offset, rect.y + rect.h - 7);
          context.stroke();
        }
      } else if (buildingDef.fortificationRole === "tower") {
        context.fillStyle = "#727985";
        context.fillRect(rect.x + rect.w * 0.25, rect.y + 8, rect.w * 0.5, rect.h - 8);
        context.strokeStyle = "#434951";
        context.lineWidth = 2;
        context.strokeRect(rect.x + rect.w * 0.25, rect.y + 8, rect.w * 0.5, rect.h - 8);
        context.beginPath();
        context.fillStyle = "#8b929d";
        context.arc(rect.x + rect.w / 2, rect.y + 10, rect.w * 0.28, 0, Math.PI * 2);
        context.fill();
        context.stroke();
        context.fillStyle = presentation.accentColor;
        context.fillRect(rect.x + rect.w * 0.44, rect.y + 2, rect.w * 0.12, 12);
      } else if (buildingDef.fortificationRole === "gate") {
        const flankWidth = rect.w * 0.28;
        context.fillStyle = "#6b717d";
        context.fillRect(rect.x, rect.y, flankWidth, rect.h);
        context.fillRect(rect.x + rect.w - flankWidth, rect.y, flankWidth, rect.h);
        context.fillRect(rect.x, rect.y, rect.w, 7);
        context.strokeStyle = "#424852";
        context.lineWidth = 2;
        context.strokeRect(rect.x, rect.y, flankWidth, rect.h);
        context.strokeRect(rect.x + rect.w - flankWidth, rect.y, flankWidth, rect.h);
        context.strokeRect(rect.x, rect.y, rect.w, 7);
        context.fillStyle = "rgba(22, 18, 15, 0.92)";
        context.fillRect(rect.x + flankWidth, rect.y + rect.h * 0.35, rect.w - flankWidth * 2, rect.h * 0.65);
        context.fillStyle = presentation.accentColor;
        context.fillRect(rect.x + 6, rect.y + 3, rect.w - 12, 3);
      } else if (buildingDef.fortificationRole === "keep") {
        context.fillStyle = "#646b75";
        context.fillRect(rect.x + 4, rect.y + 4, rect.w - 8, rect.h - 8);
        context.strokeStyle = "#3b414a";
        context.lineWidth = 2;
        context.strokeRect(rect.x + 4, rect.y + 4, rect.w - 8, rect.h - 8);
        context.fillStyle = "#7b828d";
        context.fillRect(rect.x + rect.w * 0.28, rect.y + rect.h * 0.28, rect.w * 0.44, rect.h * 0.44);
        context.strokeRect(rect.x + rect.w * 0.28, rect.y + rect.h * 0.28, rect.w * 0.44, rect.h * 0.44);
        context.fillStyle = presentation.accentColor;
        [
          [rect.x + 2, rect.y + 2],
          [rect.x + rect.w - 10, rect.y + 2],
          [rect.x + 2, rect.y + rect.h - 10],
          [rect.x + rect.w - 10, rect.y + rect.h - 10],
        ].forEach(([towerX, towerY]) => {
          context.fillRect(towerX, towerY, 8, 8);
        });
      } else if (buildingDef.navalRole === "harbor") {
        // Session 27: Harbor silhouette. Coastal stone base + wooden pier + moored vessel glyph.
        context.fillStyle = "#58504a";
        context.fillRect(rect.x + 2, rect.y + 6, rect.w - 4, rect.h - 10);
        context.strokeStyle = presentation.accentColor;
        context.lineWidth = 2;
        context.strokeRect(rect.x + 2, rect.y + 6, rect.w - 4, rect.h - 10);
        // Pier extending from the structure
        context.fillStyle = "#7a6547";
        context.fillRect(rect.x + rect.w * 0.30, rect.y + rect.h - 4, rect.w * 0.40, 8);
        // Moored vessel glyph
        context.fillStyle = presentation.primaryColor;
        context.beginPath();
        context.moveTo(rect.x + rect.w * 0.35, rect.y + rect.h + 2);
        context.lineTo(rect.x + rect.w * 0.65, rect.y + rect.h + 2);
        context.lineTo(rect.x + rect.w * 0.55, rect.y + rect.h + 10);
        context.lineTo(rect.x + rect.w * 0.45, rect.y + rect.h + 10);
        context.closePath();
        context.fill();
        context.strokeStyle = presentation.accentColor;
        context.lineWidth = 1;
        context.stroke();
        // Harbor flag
        context.fillStyle = presentation.accentColor;
        context.fillRect(rect.x + rect.w * 0.45, rect.y - 4, 3, 10);
        context.fillRect(rect.x + rect.w * 0.45 + 3, rect.y - 4, 6, 4);
      } else if (buildingDef.faithRole === "apex") {
        // Session 22: Apex Covenant silhouette. Most ornate covenant structure.
        context.fillStyle = "#15110a";
        context.fillRect(rect.x + 4, rect.y + 12, rect.w - 8, rect.h - 14);
        context.strokeStyle = presentation.accentColor;
        context.lineWidth = 4;
        context.strokeRect(rect.x + 4, rect.y + 12, rect.w - 8, rect.h - 14);
        // Towering central obelisk
        context.fillStyle = "#f0e0b0";
        context.fillRect(rect.x + rect.w * 0.42, rect.y, rect.w * 0.16, rect.h - 2);
        // Outer apex pillars (four)
        context.fillStyle = "#d2bc85";
        context.fillRect(rect.x + rect.w * 0.10, rect.y + 8, rect.w * 0.08, rect.h - 12);
        context.fillRect(rect.x + rect.w * 0.82, rect.y + 8, rect.w * 0.08, rect.h - 12);
        context.fillRect(rect.x + rect.w * 0.24, rect.y + 4, rect.w * 0.06, rect.h - 8);
        context.fillRect(rect.x + rect.w * 0.70, rect.y + 4, rect.w * 0.06, rect.h - 8);
        // Apex sigil at the crown
        context.fillStyle = presentation.accentColor;
        context.beginPath();
        context.arc(rect.x + rect.w / 2, rect.y + 6, 9, 0, Math.PI * 2);
        context.fill();
        // Inner sigil ring
        context.strokeStyle = withAlpha(presentation.accentColor, 0.6);
        context.lineWidth = 1;
        context.beginPath();
        context.arc(rect.x + rect.w / 2, rect.y + 6, 14, 0, Math.PI * 2);
        context.stroke();
        // Widest aura ring
        context.save();
        context.strokeStyle = withAlpha(presentation.accentColor, 0.22);
        context.lineWidth = 2;
        context.beginPath();
        context.arc(rect.x + rect.w / 2, rect.y + rect.h / 2, 82, 0, Math.PI * 2);
        context.stroke();
        context.restore();
      } else if (buildingDef.faithRole === "sanctuary") {
        // Session 21: Grand Sanctuary silhouette. Largest, most ornate covenant building.
        context.fillStyle = "#221f17";
        context.fillRect(rect.x + 4, rect.y + 10, rect.w - 8, rect.h - 12);
        context.strokeStyle = presentation.accentColor;
        context.lineWidth = 3;
        context.strokeRect(rect.x + 4, rect.y + 10, rect.w - 8, rect.h - 12);
        // Tall spire center
        context.fillStyle = "#e4d4a8";
        context.fillRect(rect.x + rect.w * 0.44, rect.y + 2, rect.w * 0.12, rect.h - 4);
        // Flanking pillars
        context.fillStyle = "#c9b68b";
        context.fillRect(rect.x + rect.w * 0.18, rect.y + 6, rect.w * 0.08, rect.h - 10);
        context.fillRect(rect.x + rect.w * 0.74, rect.y + 6, rect.w * 0.08, rect.h - 10);
        // Apex sigil atop the spire
        context.fillStyle = presentation.accentColor;
        context.beginPath();
        context.arc(rect.x + rect.w / 2, rect.y + 6, 7, 0, Math.PI * 2);
        context.fill();
        // Largest aura ring (64 tiles)
        context.save();
        context.strokeStyle = withAlpha(presentation.accentColor, 0.18);
        context.lineWidth = 1.5;
        context.beginPath();
        context.arc(rect.x + rect.w / 2, rect.y + rect.h / 2, 64, 0, Math.PI * 2);
        context.stroke();
        context.restore();
      } else if (buildingDef.faithRole === "hall") {
        // Session 17: Covenant Hall silhouette. Larger, more ornate than Wayshrine.
        context.fillStyle = "#2d2920";
        context.fillRect(rect.x + 4, rect.y + 8, rect.w - 8, rect.h - 10);
        context.strokeStyle = presentation.accentColor;
        context.lineWidth = 2;
        context.strokeRect(rect.x + 4, rect.y + 8, rect.w - 8, rect.h - 10);
        // Triple pillar facade
        context.fillStyle = "#d2c19a";
        context.fillRect(rect.x + rect.w * 0.22, rect.y + 4, rect.w * 0.08, rect.h - 6);
        context.fillRect(rect.x + rect.w * 0.46, rect.y + 4, rect.w * 0.08, rect.h - 6);
        context.fillRect(rect.x + rect.w * 0.70, rect.y + 4, rect.w * 0.08, rect.h - 6);
        // Central sigil at the apex
        context.fillStyle = presentation.accentColor;
        context.beginPath();
        context.arc(rect.x + rect.w / 2, rect.y + 8, 5, 0, Math.PI * 2);
        context.fill();
        // Extended aura ring (Hall radius)
        context.save();
        context.strokeStyle = withAlpha(presentation.accentColor, 0.15);
        context.lineWidth = 1;
        context.beginPath();
        context.arc(rect.x + rect.w / 2, rect.y + rect.h / 2, 48, 0, Math.PI * 2);
        context.stroke();
        context.restore();
      } else if (buildingDef.faithRole === "shrine") {
        // Session 13: Wayshrine silhouette. Canonical first-tier covenant structure.
        // Distinct from fortification buildings and economic dropoffs.
        context.fillStyle = "#3a342b";
        context.fillRect(rect.x + 4, rect.y + 6, rect.w - 8, rect.h - 8);
        context.strokeStyle = presentation.accentColor;
        context.lineWidth = 2;
        context.strokeRect(rect.x + 4, rect.y + 6, rect.w - 8, rect.h - 8);
        // Pillar silhouette
        context.fillStyle = "#c7b185";
        context.fillRect(rect.x + rect.w * 0.45, rect.y + 2, rect.w * 0.10, rect.h - 4);
        // Flame/sigil at the top
        context.fillStyle = presentation.accentColor;
        context.beginPath();
        context.arc(rect.x + rect.w / 2, rect.y + 6, 4, 0, Math.PI * 2);
        context.fill();
        // Faith exposure radius indicator (subtle, only when selected? always faint)
        context.save();
        context.strokeStyle = withAlpha(presentation.accentColor, 0.12);
        context.lineWidth = 1;
        context.beginPath();
        context.arc(rect.x + rect.w / 2, rect.y + rect.h / 2, 36, 0, Math.PI * 2);
        context.stroke();
        context.restore();
      } else {
        context.fillStyle = presentation.primaryColor;
        context.fillRect(rect.x, rect.y, rect.w, rect.h);
        context.strokeStyle = presentation.accentColor;
        context.lineWidth = 2;
        context.strokeRect(rect.x, rect.y, rect.w, rect.h);
        }

        if ((building.raidedUntil ?? 0) > (state.meta.elapsed ?? 0)) {
          context.strokeStyle = "rgba(230, 148, 78, 0.95)";
          context.lineWidth = 2;
          context.setLineDash([6, 4]);
          context.strokeRect(rect.x - 2, rect.y - 2, rect.w + 4, rect.h + 4);
          context.setLineDash([]);
          context.beginPath();
          context.moveTo(rect.x + 4, rect.y + rect.h - 2);
          context.lineTo(rect.x + rect.w - 4, rect.y + 4);
          context.stroke();
        }

        context.restore();
      drawHealthBar(context, rect.x, rect.y - 8, rect.w, building.health / buildingDef.health, "#88c08b", "rgba(0,0,0,0.45)");
      context.fillStyle = "#f3e6d3";
      context.font = "11px IBM Plex Sans";
      const labelY = buildingDef.fortificationRole || buildingDef.faithRole || building.typeId === "quarry" || building.typeId === "iron_mine" || building.typeId === "siege_workshop" || building.typeId === "supply_camp"
        ? rect.y + rect.h + 14
        : rect.y + 16;
      context.fillText(buildingDef.name, rect.x + 4, labelY);
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

      if (unit.commanderMemberId) {
        context.strokeStyle = withAlpha(presentation.accentColor, 0.8);
        context.lineWidth = 1.5;
        context.beginPath();
        context.arc(0, 0, unit.radius + 16, 0, Math.PI * 2);
        context.stroke();
      }

      if (unit.reserveDuty === "muster") {
        context.strokeStyle = "rgba(120, 203, 214, 0.88)";
        context.lineWidth = 1.5;
        context.setLineDash([5, 3]);
        context.beginPath();
        context.arc(0, 0, unit.radius + 10, 0, Math.PI * 2);
        context.stroke();
        context.setLineDash([]);
      } else if (unit.reserveDuty === "fallback" || unit.reserveDuty === "recovering") {
        context.strokeStyle = "rgba(244, 172, 94, 0.9)";
        context.lineWidth = 1.5;
        context.beginPath();
        context.arc(0, 0, unit.radius + 10, 0, Math.PI * 2);
        context.stroke();
      }

      context.fillStyle = presentation.primaryColor;
      context.strokeStyle = presentation.accentColor;
      context.lineWidth = 2;

      if (unitDef.role === "worker") {
        context.fillRect(-9, -9, 18, 18);
        context.strokeRect(-9, -9, 18, 18);
      } else if (unitDef.role === "engineer-specialist") {
        context.beginPath();
        context.moveTo(0, -12);
        context.lineTo(11, 0);
        context.lineTo(0, 12);
        context.lineTo(-11, 0);
        context.closePath();
        context.fill();
        context.stroke();
        context.strokeStyle = "#f3e6d3";
        context.lineWidth = 1.5;
        context.beginPath();
        context.moveTo(-6, 6);
        context.lineTo(6, -6);
        context.moveTo(-2, -8);
        context.lineTo(8, 2);
        context.stroke();
        } else if (unitDef.role === "support") {
          context.fillRect(-12, -8, 24, 16);
          context.strokeRect(-12, -8, 24, 16);
        context.fillStyle = withAlpha(presentation.accentColor, 0.38);
        context.fillRect(-6, -4, 12, 8);
        context.fillStyle = "#1f1a16";
        context.beginPath();
        context.arc(-8, 10, 4, 0, Math.PI * 2);
        context.arc(8, 10, 4, 0, Math.PI * 2);
        context.fill();
        context.strokeStyle = "#f3e6d3";
        context.lineWidth = 1;
        context.beginPath();
        context.moveTo(0, -8);
        context.lineTo(0, -14);
        context.lineTo(5, -11);
        context.stroke();
      } else if (unitDef.role === "vessel") {
        // Session 27: Vessel silhouette. Elongated hull + mast.
        context.fillStyle = "#6b543a";
        context.beginPath();
        context.moveTo(-16, 6);
        context.lineTo(16, 6);
        context.lineTo(12, 12);
        context.lineTo(-12, 12);
        context.closePath();
        context.fill();
        context.stroke();
        // Mast + sail
        context.strokeStyle = "#f3e6d3";
        context.lineWidth = 1;
        context.beginPath();
        context.moveTo(0, 6);
        context.lineTo(0, -16);
        context.stroke();
        context.fillStyle = withAlpha(presentation.accentColor, 0.7);
        context.beginPath();
        context.moveTo(0, -15);
        context.lineTo(10, -4);
        context.lineTo(0, -4);
        context.closePath();
        context.fill();
        // Water ripple indicator for legibility
        context.strokeStyle = withAlpha("#6fb0c4", 0.35);
        context.lineWidth = 1;
        context.beginPath();
        context.arc(0, 14, 14, 0.1 * Math.PI, 0.9 * Math.PI);
        context.stroke();
      } else if (unitDef.role === "siege-support" && unitDef.siegeClass === "mantlet") {
        // Mantlet silhouette: a wide shield-wall slab on low wheels with a cover aura.
        const coverRadius = unitDef.coverRadius ?? 0;
        if (coverRadius > 0) {
          context.save();
          context.fillStyle = withAlpha(presentation.accentColor, 0.08);
          context.strokeStyle = withAlpha(presentation.accentColor, 0.22);
          context.lineWidth = 1;
          context.beginPath();
          context.arc(0, 0, Math.min(coverRadius, 120) * 0.5, 0, Math.PI * 2);
          context.fill();
          context.stroke();
          context.restore();
        }
        context.fillRect(-14, -4, 28, 10);
        context.strokeRect(-14, -4, 28, 10);
        context.fillStyle = withAlpha(presentation.accentColor, 0.5);
        context.fillRect(-13, -3, 26, 4);
        context.strokeStyle = "#f3e6d3";
        context.lineWidth = 1;
        context.beginPath();
        context.moveTo(-10, -8);
        context.lineTo(-10, -4);
        context.moveTo(-4, -10);
        context.lineTo(-4, -4);
        context.moveTo(4, -10);
        context.lineTo(4, -4);
        context.moveTo(10, -8);
        context.lineTo(10, -4);
        context.stroke();
        context.fillStyle = "#1f1a16";
          context.beginPath();
          context.arc(-10, 9, 3, 0, Math.PI * 2);
          context.arc(10, 9, 3, 0, Math.PI * 2);
          context.fill();
        } else if (unitDef.role === "light-cavalry") {
          context.beginPath();
          context.moveTo(-12, 4);
          context.lineTo(-2, -8);
          context.lineTo(10, -2);
          context.lineTo(12, 6);
          context.lineTo(2, 10);
          context.lineTo(-8, 10);
          context.closePath();
          context.fill();
          context.stroke();
          context.strokeStyle = "#f3e6d3";
          context.lineWidth = 1.5;
          context.beginPath();
          context.moveTo(-2, -8);
          context.lineTo(6, -16);
          context.moveTo(4, -4);
          context.lineTo(10, -8);
          context.moveTo(-4, 10);
          context.lineTo(-8, 16);
          context.moveTo(4, 10);
          context.lineTo(8, 16);
          context.stroke();
        } else if (unitDef.role === "siege-engine") {
        if (unitDef.siegeClass === "siege_tower") {
          context.fillRect(-10, -16, 20, 28);
          context.strokeRect(-10, -16, 20, 28);
          context.fillStyle = presentation.accentColor;
          context.fillRect(-11, -20, 22, 6);
          context.fillStyle = withAlpha(presentation.accentColor, 0.35);
          context.fillRect(-5, -8, 10, 18);
          context.fillStyle = "#1f1a16";
          context.beginPath();
          context.arc(-9, 14, 4, 0, Math.PI * 2);
          context.arc(9, 14, 4, 0, Math.PI * 2);
          context.arc(-3, 14, 4, 0, Math.PI * 2);
          context.arc(3, 14, 4, 0, Math.PI * 2);
          context.fill();
        } else if (unitDef.siegeClass === "trebuchet") {
          context.beginPath();
          context.moveTo(-12, 10);
          context.lineTo(-2, -10);
          context.lineTo(8, 10);
          context.closePath();
          context.fill();
          context.stroke();
          context.beginPath();
          context.moveTo(-2, -10);
          context.lineTo(12, -18);
          context.stroke();
          context.fillStyle = presentation.accentColor;
          context.fillRect(10, -21, 6, 7);
          context.fillStyle = "#1f1a16";
          context.beginPath();
          context.arc(-9, 12, 4, 0, Math.PI * 2);
          context.arc(7, 12, 4, 0, Math.PI * 2);
          context.fill();
        } else if (unitDef.siegeClass === "ballista") {
          // Ballista silhouette: low frame with crossed rail and a projecting bolt channel.
          context.fillRect(-11, -4, 22, 10);
          context.strokeRect(-11, -4, 22, 10);
          context.strokeStyle = presentation.accentColor;
          context.lineWidth = 2;
          context.beginPath();
          context.moveTo(-10, -8);
          context.lineTo(10, -8);
          context.stroke();
          context.beginPath();
          context.moveTo(-8, -12);
          context.lineTo(8, -4);
          context.moveTo(8, -12);
          context.lineTo(-8, -4);
          context.stroke();
          context.lineWidth = 1.5;
          context.strokeStyle = "#f3e6d3";
          context.beginPath();
          context.moveTo(0, -6);
          context.lineTo(14, -6);
          context.stroke();
          context.fillStyle = "#1f1a16";
          context.beginPath();
          context.arc(-8, 8, 3, 0, Math.PI * 2);
          context.arc(8, 8, 3, 0, Math.PI * 2);
          context.fill();
        } else {
          context.fillRect(-13, -7, 26, 14);
          context.strokeRect(-13, -7, 26, 14);
          context.beginPath();
          context.moveTo(9, -7);
          context.lineTo(15, 0);
          context.lineTo(9, 7);
          context.closePath();
          context.fill();
          context.stroke();
          context.fillStyle = "#1f1a16";
          context.beginPath();
          context.arc(-7, 9, 4, 0, Math.PI * 2);
          context.arc(7, 9, 4, 0, Math.PI * 2);
          context.fill();
        }
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
      if (unit.commanderMemberId) {
        const faction = state.factions[unit.factionId];
        const commander = faction?.dynasty?.members?.find((member) => member.id === unit.commanderMemberId);
        if (commander) {
          context.fillStyle = "#f4ead8";
          context.font = "10px IBM Plex Sans";
          context.fillText(commander.title, -unit.radius - 6, unit.radius + 22);
        }
      }
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
          : patch.type === "water"
            ? "rgba(40, 80, 112, 0.85)"
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
        ["food", formatResourceValue(player.resources.food)],
        ["water", formatResourceValue(player.resources.water)],
        ["wood", formatResourceValue(player.resources.wood)],
        ["stone", formatResourceValue(player.resources.stone)],
        ["iron", formatResourceValue(player.resources.iron)],
        ["influence", formatResourceValue(player.resources.influence)],
        ["available pop", `${player.population.total - usedPopulation - player.population.reserved}`],
        ["total pop", `${player.population.total}/${player.population.cap}`],
        ["territory", `${territoryCount}/${state.world.controlPoints.length}`],
      ];
    },
  };
}
