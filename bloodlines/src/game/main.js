import { updateEnemyAi, updateNeutralAi } from "./core/ai.js";
import { loadGameContent } from "./core/data-loader.js";
import { createRenderer } from "./core/renderer.js";
import {
  attemptPlaceBuilding,
  chooseFaithCommitment,
  createSimulation,
  getFactionSnapshot,
  issueAttackCommand,
  issueGatherCommand,
  issueMoveCommand,
  pickWorldTarget,
  queueProduction,
  stepSimulation,
} from "./core/simulation.js";
import { clamp } from "./core/utils.js";

const refs = {
  startButton: document.getElementById("start-game"),
  launchScene: document.getElementById("launch-scene"),
  gameShell: document.getElementById("game-shell"),
  canvas: document.getElementById("game-canvas"),
  minimap: document.getElementById("minimap-canvas"),
  launchStatus: document.getElementById("launch-status"),
  resourceBar: document.getElementById("resource-bar"),
  selectionPanel: document.getElementById("selection-panel"),
  commandPanel: document.getElementById("command-panel"),
  dynastyPanel: document.getElementById("dynasty-panel"),
  faithPanel: document.getElementById("faith-panel"),
  debugOverlay: document.getElementById("debug-overlay"),
  messageLog: document.getElementById("message-log"),
  pauseToggle: document.getElementById("pause-toggle"),
  returnToMenu: document.getElementById("return-to-menu"),
};

const uiState = {
  camera: { x: 0, y: 0 },
  selectionIds: [],
  selectionBox: null,
  buildMode: null,
  dragging: false,
  dragStart: null,
  pointerWorld: { x: 0, y: 0 },
  paused: false,
};

let content = null;
let state = null;
let renderer = null;
let animationFrameId = 0;
let lastTimestamp = 0;

function setLaunchStatus(text, tone = "info") {
  refs.launchStatus.textContent = text;
  refs.launchStatus.className = "launch-status";
  if (tone === "loading") {
    refs.launchStatus.classList.add("launch-status--loading");
  } else if (tone === "error") {
    refs.launchStatus.classList.add("launch-status--error");
  }
}

function handleRuntimeFailure(error, returnToLaunch = false) {
  console.error(error);
  cancelAnimationFrame(animationFrameId);
  uiState.paused = true;

  if (returnToLaunch) {
    refs.gameShell.hidden = true;
    refs.launchScene.hidden = false;
    refs.startButton.disabled = false;
    setLaunchStatus(`Prototype failed: ${error.message}`, "error");
    return;
  }

  refs.debugOverlay.textContent = `Runtime failure:\n${error.message}`;
  refs.messageLog.innerHTML = `<div class="message-entry message-entry--warn">Prototype failed: ${error.message}</div>`;
}

function worldSize() {
  return {
    width: state.world.width * state.world.tileSize,
    height: state.world.height * state.world.tileSize,
  };
}

function getCanvasPoint(event) {
  const rect = refs.canvas.getBoundingClientRect();
  const scaleX = refs.canvas.width / rect.width;
  const scaleY = refs.canvas.height / rect.height;
  return {
    x: (event.clientX - rect.left) * scaleX,
    y: (event.clientY - rect.top) * scaleY,
  };
}

function screenToWorld(point) {
  return {
    x: point.x + uiState.camera.x,
    y: point.y + uiState.camera.y,
  };
}

function setSelection(ids) {
  uiState.selectionIds = [...new Set(ids)];
  renderPanels();
}

function getSelectedEntities() {
  const units = state.units.filter((unit) => uiState.selectionIds.includes(unit.id) && unit.factionId === "player" && unit.health > 0);
  const buildings = state.buildings.filter((building) => uiState.selectionIds.includes(building.id) && building.factionId === "player" && building.health > 0);
  return { units, buildings };
}

function getSelectedWorkers() {
  return getSelectedEntities().units.filter((unit) => content.byId.units[unit.typeId].role === "worker");
}

function getSelectedCombatUnits() {
  return getSelectedEntities().units.filter((unit) => content.byId.units[unit.typeId].role !== "worker");
}

function updateResourceBar() {
  refs.resourceBar.innerHTML = renderer
    .buildResourceBar(state)
    .map(
      ([label, value]) => `
        <div class="resource-pill">
          <span class="resource-label">${label}</span>
          <span class="resource-value">${value}</span>
        </div>
      `,
    )
    .join("");
}

function renderSelectionPanel() {
  const { units, buildings } = getSelectedEntities();
  if (units.length === 0 && buildings.length === 0) {
    refs.selectionPanel.innerHTML = `<p class="empty-state">Select units or a structure to inspect available commands.</p>`;
    return;
  }

  const rows = [];
  units.forEach((unit) => {
    const unitDef = content.byId.units[unit.typeId];
    rows.push(`
      <div class="selection-item">
        <span>${unitDef.name}</span>
        <span class="selection-meta">${Math.ceil(unit.health)}/${unitDef.health}</span>
      </div>
    `);
  });
  buildings.forEach((building) => {
    const buildingDef = content.byId.buildings[building.typeId];
    rows.push(`
      <div class="selection-item">
        <span>${buildingDef.name}</span>
        <span class="selection-meta">${building.completed ? "Online" : "Building"}</span>
      </div>
    `);
  });

  refs.selectionPanel.innerHTML = `<div class="selection-grid">${rows.join("")}</div>`;
}

function createActionButton({ label, detail, onClick, warning = false }) {
  const button = document.createElement("button");
  button.className = "action-btn";
  button.innerHTML = `<strong>${label}</strong><span class="${warning ? "warning-text" : ""}">${detail}</span>`;
  button.addEventListener("click", onClick);
  return button;
}

function renderCommandPanel() {
  refs.commandPanel.innerHTML = "";
  const { units, buildings } = getSelectedEntities();
  const workerSelection = getSelectedWorkers();

  if (uiState.buildMode) {
    refs.commandPanel.innerHTML = `
      <p class="command-meta">Placing <strong>${content.byId.buildings[uiState.buildMode].name}</strong>. Left click on the map to build. Right click to cancel.</p>
    `;
    return;
  }

  const actionGrid = document.createElement("div");
  actionGrid.className = "action-grid";

  if (workerSelection.length > 0 && workerSelection.length === units.length) {
    ["dwelling", "farm", "well", "lumber_camp", "mine_works", "barracks"].forEach((buildingId) => {
      const building = content.byId.buildings[buildingId];
      const costSummary = Object.entries(building.cost).map(([key, value]) => `${key} ${value}`).join(", ");
      actionGrid.appendChild(
        createActionButton({
          label: building.name,
          detail: costSummary,
          onClick: () => {
            uiState.buildMode = buildingId;
            renderCommandPanel();
          },
        }),
      );
    });
  }

  if (buildings.length === 1) {
    const building = buildings[0];
    const buildingDef = content.byId.buildings[building.typeId];
    (buildingDef.trainableUnits ?? []).forEach((unitId) => {
      const unitDef = content.byId.units[unitId];
      const costSummary = Object.entries(unitDef.cost).map(([key, value]) => `${key} ${value}`).join(", ");
      actionGrid.appendChild(
        createActionButton({
          label: `Train ${unitDef.name}`,
          detail: costSummary,
          onClick: () => {
            const result = queueProduction(state, building.id, unitId);
            if (!result.ok) {
              state.messages.unshift({
                id: `queue-fail-${Date.now()}`,
                text: result.reason,
                tone: "warn",
                ttl: 4,
              });
            }
            renderPanels();
          },
        }),
      );
    });
  }

  if (!actionGrid.hasChildNodes()) {
    refs.commandPanel.innerHTML = `<p class="empty-state">This selection has no direct build or training actions yet.</p>`;
    return;
  }

  refs.commandPanel.appendChild(actionGrid);
}

function renderDynastyPanel() {
  const player = getFactionSnapshot(state, "player");
  if (!player.dynasty) {
    refs.dynastyPanel.innerHTML = `<p class="empty-state">No active bloodline roster is present for this faction.</p>`;
    return;
  }

  const members = player.dynasty.members
    .slice(0, 6)
    .map((member) => `
      <div class="dynasty-member">
        <div class="member-title">
          <strong>${member.title}</strong>
          <span class="selection-meta">Age ${member.age}</span>
        </div>
        <div class="member-detail">${member.role.name} • ${member.path.name}</div>
        <div class="member-detail">Status: ${member.status} • Renown ${member.renown}</div>
      </div>
    `)
    .join("");

  refs.dynastyPanel.innerHTML = `
    <div class="subpanel-grid">
      <div class="member-detail">Legitimacy ${player.dynasty.legitimacy} • Active members ${player.dynasty.members.length}/${player.dynasty.activeMemberCap}</div>
      ${members}
    </div>
  `;
}

function renderFaithPanel() {
  const player = getFactionSnapshot(state, "player");
  const actionGrid = document.createElement("div");
  actionGrid.className = "action-grid";

  const exposures = player.faith.exposures
    .map((faith) => `
      <div class="faith-track">
        <div class="faith-title">
          <strong>${faith.name}</strong>
          <span class="selection-meta">${Math.floor(faith.exposure)} / 100</span>
        </div>
        <div class="faith-detail">${faith.covenantName}</div>
        <div class="progress-bar"><div class="progress-fill" style="width: ${Math.min(100, faith.exposure)}%"></div></div>
      </div>
    `)
    .join("");

  if (!player.faith.selectedFaithId) {
    player.faith.exposures
      .filter((faith) => faith.availableToCommit)
      .forEach((faith) => {
        actionGrid.appendChild(
          createActionButton({
            label: `Embrace ${faith.name}`,
            detail: "Commit the dynasty to this covenant",
            onClick: () => {
              const result = chooseFaithCommitment(state, "player", faith.id);
              if (!result.ok) {
                state.messages.unshift({
                  id: `faith-fail-${Date.now()}`,
                  text: result.reason,
                  tone: "warn",
                  ttl: 4,
                });
              }
              renderPanels();
            },
          }),
        );
      });
  }

  refs.faithPanel.innerHTML = `
    <div class="subpanel-grid">
      <div class="faith-detail">Conviction: ${player.conviction.bandLabel}</div>
      <div class="faith-detail">${
        player.faith.selectedFaith
          ? `${player.faith.selectedFaith.name} • ${player.faith.tierLabel} intensity ${player.faith.intensity.toFixed(1)} • ${player.faith.doctrinePath} doctrine`
          : "No covenant chosen yet. Gain exposure by entering sacred sites and frontier holy ground."
      }</div>
      ${exposures}
    </div>
  `;

  if (actionGrid.hasChildNodes()) {
    refs.faithPanel.appendChild(actionGrid);
  }
}

function renderDebug() {
  const player = getFactionSnapshot(state, "player");
  const selection = getSelectedEntities();
  const territoryStatus = state.world.controlPoints
    .map((controlPoint) =>
      `${controlPoint.name}: ${controlPoint.contested ? "contested" : controlPoint.ownerFactionId ? state.factions[controlPoint.ownerFactionId].displayName : "neutral"}`,
    )
    .join(" | ");
  refs.debugOverlay.textContent = [
    `House: ${content.byId.houses[player.houseId].name}`,
    `Status: ${state.meta.status}`,
    `Selected units: ${selection.units.length}`,
    `Selected buildings: ${selection.buildings.length}`,
    `Population: total ${player.population.total}, used ${player.population.used}, reserved ${player.population.reserved}, available ${player.population.available}, cap ${player.population.cap}`,
    `Resources: gold ${Math.floor(player.resources.gold)}, wood ${Math.floor(player.resources.wood)}, influence ${player.resources.influence.toFixed(1)}, food ${player.resources.food.toFixed(1)}, water ${player.resources.water.toFixed(1)}`,
    `Territory: ${player.territories.count}/${state.world.controlPoints.length} held`,
    `Faith: ${player.faith.selectedFaith ? `${player.faith.selectedFaith.name} ${player.faith.tierLabel}` : "Unaligned"}`,
    `Conviction: ${player.conviction.bandLabel}`,
    `Control points: ${territoryStatus}`,
    `Build mode: ${uiState.buildMode ? content.byId.buildings[uiState.buildMode].name : "none"}`,
  ].join("\n");
}

function renderMessages() {
  refs.messageLog.innerHTML = state.messages
    .map((message) => `<div class="message-entry ${message.tone === "warn" ? "message-entry--warn" : message.tone === "good" ? "message-entry--good" : ""}">${message.text}</div>`)
    .join("");
}

function renderPanels() {
  updateResourceBar();
  renderSelectionPanel();
  renderCommandPanel();
  renderDynastyPanel();
  renderFaithPanel();
  renderDebug();
  renderMessages();
}

function updateCamera(dt) {
  const speed = 520 * dt;
  if (pressedKeys.has("w") || pressedKeys.has("arrowup")) {
    uiState.camera.y -= speed;
  }
  if (pressedKeys.has("s") || pressedKeys.has("arrowdown")) {
    uiState.camera.y += speed;
  }
  if (pressedKeys.has("a") || pressedKeys.has("arrowleft")) {
    uiState.camera.x -= speed;
  }
  if (pressedKeys.has("d") || pressedKeys.has("arrowright")) {
    uiState.camera.x += speed;
  }

  const size = worldSize();
  uiState.camera.x = clamp(uiState.camera.x, 0, Math.max(0, size.width - refs.canvas.width));
  uiState.camera.y = clamp(uiState.camera.y, 0, Math.max(0, size.height - refs.canvas.height));
}

function performFrame(dt) {
  if (!uiState.paused) {
    updateCamera(dt);
    stepSimulation(state, dt);
    updateEnemyAi(state, dt);
    updateNeutralAi(state, dt);
  }

  renderer.render(state, uiState);
  renderPanels();
}

function animationLoop(timestamp) {
  try {
    const dt = Math.min((timestamp - lastTimestamp) / 1000 || 0, 0.05);
    lastTimestamp = timestamp;
    performFrame(dt);
    animationFrameId = requestAnimationFrame(animationLoop);
  } catch (error) {
    handleRuntimeFailure(error);
  }
}

function applyInitialCamera() {
  const start = content?.map?.cameraStart ?? { x: 0, y: 0 };
  uiState.camera.x = start.x;
  uiState.camera.y = start.y;
}

function bootImmediateFrame() {
  lastTimestamp = performance.now();
  performFrame(0);
}

async function startGame() {
  setLaunchStatus("Loading prototype data...", "loading");
  refs.startButton.disabled = true;
  if (!content) {
    content = await loadGameContent();
  }
  state = createSimulation(content);
  renderer = createRenderer(refs.canvas, refs.minimap);
  applyInitialCamera();
  uiState.selectionIds = [];
  uiState.selectionBox = null;
  uiState.buildMode = null;
  uiState.paused = false;
  refs.pauseToggle.textContent = "Pause";

  refs.launchScene.hidden = true;
  refs.gameShell.hidden = false;
  cancelAnimationFrame(animationFrameId);
  bootImmediateFrame();
  animationFrameId = requestAnimationFrame(animationLoop);
  refs.startButton.disabled = false;
  setLaunchStatus("");
}

function resetToMenu() {
  cancelAnimationFrame(animationFrameId);
  refs.gameShell.hidden = true;
  refs.launchScene.hidden = false;
  uiState.selectionIds = [];
  uiState.selectionBox = null;
  uiState.buildMode = null;
}

function handleLeftClick(event) {
  if (!state) {
    return;
  }

  const point = getCanvasPoint(event);
  const worldPoint = screenToWorld(point);
  uiState.pointerWorld = worldPoint;

  if (uiState.buildMode) {
    const worker = getSelectedWorkers()[0];
    if (!worker) {
      uiState.buildMode = null;
      renderCommandPanel();
      return;
    }

    const tileSize = state.world.tileSize;
    const tileX = Math.floor(worldPoint.x / tileSize);
    const tileY = Math.floor(worldPoint.y / tileSize);
    const result = attemptPlaceBuilding(state, "player", uiState.buildMode, tileX, tileY, worker.id);
    if (!result.ok) {
      state.messages.unshift({
        id: `build-fail-${Date.now()}`,
        text: result.reason,
        tone: "warn",
        ttl: 4,
      });
    } else {
      uiState.buildMode = null;
    }
    renderPanels();
    return;
  }

  const target = pickWorldTarget(state, worldPoint.x, worldPoint.y);
  if (!target) {
    setSelection([]);
    return;
  }

  if (target.type === "resource") {
    setSelection([]);
    return;
  }

  if (target.entity.factionId !== "player") {
    return;
  }

  setSelection([target.entity.id]);
}

const pressedKeys = new Set();

window.addEventListener("keydown", (event) => {
  pressedKeys.add(event.key.toLowerCase());
});

window.addEventListener("keyup", (event) => {
  pressedKeys.delete(event.key.toLowerCase());
});

refs.canvas.addEventListener("mousedown", (event) => {
  if (event.button !== 0 || !state) {
    return;
  }

  const point = getCanvasPoint(event);
  uiState.dragging = true;
  uiState.dragStart = point;
  uiState.selectionBox = { x: point.x, y: point.y, w: 0, h: 0 };
});

refs.canvas.addEventListener("mousemove", (event) => {
  if (!state) {
    return;
  }

  const point = getCanvasPoint(event);
  uiState.pointerWorld = screenToWorld(point);

  if (!uiState.dragging || !uiState.dragStart) {
    return;
  }

  uiState.selectionBox = {
    x: Math.min(uiState.dragStart.x, point.x),
    y: Math.min(uiState.dragStart.y, point.y),
    w: Math.abs(uiState.dragStart.x - point.x),
    h: Math.abs(uiState.dragStart.y - point.y),
  };
});

window.addEventListener("mouseup", (event) => {
  if (event.button !== 0 || !state || !uiState.dragging || !uiState.selectionBox) {
    return;
  }

  const box = uiState.selectionBox;
  const wasClick = box.w < 6 && box.h < 6;
  uiState.dragging = false;
  uiState.dragStart = null;
  uiState.selectionBox = null;

  if (wasClick) {
    handleLeftClick(event);
    return;
  }

  const worldBox = {
    x: box.x + uiState.camera.x,
    y: box.y + uiState.camera.y,
    w: box.w,
    h: box.h,
  };

  const selected = state.units
    .filter((unit) => unit.factionId === "player" && unit.health > 0)
    .filter((unit) =>
      unit.x >= worldBox.x &&
      unit.x <= worldBox.x + worldBox.w &&
      unit.y >= worldBox.y &&
      unit.y <= worldBox.y + worldBox.h,
    )
    .map((unit) => unit.id);

  setSelection(selected);
});

refs.canvas.addEventListener("contextmenu", (event) => {
  event.preventDefault();
  if (!state) {
    return;
  }

  if (uiState.buildMode) {
    uiState.buildMode = null;
    renderCommandPanel();
    return;
  }

  const point = getCanvasPoint(event);
  const worldPoint = screenToWorld(point);
  const target = pickWorldTarget(state, worldPoint.x, worldPoint.y);
  const workers = getSelectedWorkers();
  const combatUnits = getSelectedCombatUnits();

  if (target?.type === "resource" && workers.length > 0) {
    issueGatherCommand(state, workers.map((unit) => unit.id), target.entity.id);
    return;
  }

  if (target && target.type !== "resource" && target.entity.factionId !== "player" && combatUnits.length > 0) {
    issueAttackCommand(state, combatUnits.map((unit) => unit.id), target.type, target.entity.id);
    return;
  }

  const allUnits = getSelectedEntities().units;
  if (allUnits.length > 0) {
    issueMoveCommand(state, allUnits.map((unit) => unit.id), worldPoint.x, worldPoint.y);
  }
});

refs.startButton.addEventListener("click", () => {
  startGame().catch((error) => {
    refs.startButton.disabled = false;
    setLaunchStatus(`Failed to load prototype: ${error.message}`, "error");
  });
});

refs.pauseToggle.addEventListener("click", () => {
  uiState.paused = !uiState.paused;
  refs.pauseToggle.textContent = uiState.paused ? "Resume" : "Pause";
});

refs.returnToMenu.addEventListener("click", resetToMenu);

window.addEventListener("error", (event) => {
  if (event.error) {
    handleRuntimeFailure(event.error, !state);
  }
});

window.addEventListener("unhandledrejection", (event) => {
  const reason = event.reason instanceof Error ? event.reason : new Error(String(event.reason));
  handleRuntimeFailure(reason, !state);
});

const query = new URLSearchParams(window.location.search);
const shouldAutoStart = query.get("briefing") !== "1";

if (shouldAutoStart) {
  startGame().catch((error) => {
    handleRuntimeFailure(error, true);
  });
}
