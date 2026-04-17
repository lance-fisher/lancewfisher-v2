import { updateEnemyAi, updateMinorHouseAi, updateNeutralAi } from "./core/ai.js";
import { loadGameContent } from "./core/data-loader.js";
import { createRenderer } from "./core/renderer.js";
import {
  acceptMarriage,
  attemptPlaceBuilding,
  chooseFaithCommitment,
  consolidateSuccessionCrisis,
  createSimulation,
  declineMarriage,
  demandCaptiveRansom,
  getMarriageAcceptanceTerms,
  getDossierBackedSabotageProfile,
  getFactionSnapshot,
  getAssassinationTerms,
  getCounterIntelligenceTerms,
  getDivineRightDeclarationTerms,
  getEspionageTerms,
  getMarriageFaithCompatibilityProfile,
  getMarriageGovernanceStatus,
  getMarriageProposalTerms,
  getHolyWarDeclarationTerms,
  getMissionaryTerms,
  getRealmConditionSnapshot,
  getSabotageOperationTerms,
  getSuccessionCrisisTerms,
  getTrainableUnitIdsForBuilding,
  commitImminentEngagementReinforcements,
  issueImminentEngagementCommanderRecall,
  issueAttackCommand,
  issueGatherCommand,
  issueRaidCommand,
  issueKeepSortie,
  issueMoveCommand,
  pickWorldTarget,
  performCovenantTestAction,
  promoteMemberToLesserHouse,
  protectImminentEngagementBloodline,
  proposeMarriage,
  queueProduction,
  setImminentEngagementPosture,
  startAssassinationOperation,
  startCounterIntelligenceOperation,
  startDivineRightDeclaration,
  startEspionageOperation,
  startHolyWarDeclaration,
  startMissionaryOperation,
  startRansomNegotiation,
  startRescueOperation,
  startSabotageOperation,
  stepSimulation,
  getNonAggressionPactTerms,
  proposeNonAggressionPact,
  breakNonAggressionPact,
  getTruebornRecognitionTerms,
  recognizeTruebornClaim,
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
  realmConditionBar: document.getElementById("realm-condition-bar"),
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

const WORKER_BUILD_ORDER = [
  "dwelling",
  "farm",
  "well",
  "lumber_camp",
  "mine_works",
  "quarry",
  "iron_mine",
  "barracks",
  "stable",
  "siege_workshop",
  "supply_camp",
  "wall_segment",
  "watch_tower",
  "gatehouse",
  "keep_tier_1",
  "wayshrine",
  "covenant_hall",
  "grand_sanctuary",
  "apex_covenant",
  "harbor_tier_1",
  "harbor_tier_2",
];

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

function getSelectedScoutRaiders() {
  return getSelectedCombatUnits().filter((unit) =>
    (content.byId.units[unit.typeId]?.raidDurationSeconds ?? 0) > 0);
}

function describeDoctrineEffects(effectSet) {
  if (!effectSet) {
    return "Doctrine effects dormant.";
  }

  return [
    effectSet.label,
    `Aura x${effectSet.auraAttackMultiplier.toFixed(2)}`,
    `Sight +${effectSet.auraSightBonus}`,
    `Stability x${effectSet.stabilizationMultiplier.toFixed(2)}`,
    `Capture x${effectSet.captureMultiplier.toFixed(2)}`,
    `Growth x${effectSet.populationGrowthMultiplier.toFixed(2)}`,
  ].join(" • ");
}

function describeMarriageFaithCompatibility(profile) {
  if (!profile) {
    return "Faith stance unreadable.";
  }

  switch (profile.tier) {
    case "harmonious":
      return `Faith stance: harmonious, ${profile.label}`;
    case "sectarian":
      return `Faith stance: sectarian, ${profile.label}`;
    case "strained":
      return `Faith stance: strained, ${profile.label}`;
    case "fractured":
      return `Faith stance: fractured, ${profile.label}`;
    default:
      return `Faith stance: unbound, ${profile.label}`;
  }
}

function describeAiMarriageEvaluation(evaluation) {
  if (!evaluation) {
    return null;
  }

  switch (evaluation.decision) {
    case "faith_backed_legitimacy":
      return "Enemy court is receptive, compatible covenant can repair dynastic legitimacy.";
    case "blocked_by_faith":
      return "Enemy court is resisting on covenant grounds, stronger pressure is required.";
    case "strategic_pressure":
      if (evaluation.ownSuccessionCrisis || evaluation.rivalSuccessionCrisis) {
        return "Enemy court reads succession instability into the match and is weighing marriage as a stabilizing compact or opportunistic bind.";
      }
      return "Enemy court is weighing the match as active strategic pressure relief.";
    case "insufficient_pressure":
      return "Enemy court sees no immediate strategic need for the match.";
    default:
      return null;
  }
}

function describeMarriageDissolution(state, faction, marriage) {
  if (!marriage?.dissolvedAt) {
    return null;
  }

  const headMember = faction?.dynasty?.members?.find((member) => member.id === marriage.headMemberId);
  const spouseFaction = state.factions[marriage.spouseFactionId];
  const spouseMember = spouseFaction?.dynasty?.members?.find((member) => member.id === marriage.spouseMemberId);
  const spouseHouse = state.content.byId.houses[marriage.spouseHouseId]?.name ?? "an allied house";
  const childCount = marriage.children?.length ?? 0;
  const childText = childCount === 1 ? "1 child recorded" : `${childCount} children recorded`;
  const dissolvedDay = Number.isFinite(marriage.dissolvedAtInWorldDays)
    ? `day ${Math.round(marriage.dissolvedAtInWorldDays)}`
    : "recently";
  const deceasedTitle = marriage.deceasedMemberId === headMember?.id
    ? headMember?.title
    : marriage.deceasedMemberId === spouseMember?.id
      ? spouseMember?.title
      : headMember?.status === "fallen"
        ? headMember.title
        : spouseMember?.status === "fallen"
          ? spouseMember.title
          : "a bloodline spouse";
  const reasonText = marriage.dissolutionReason === "death"
    ? `ended by the death of ${deceasedTitle}`
    : `ended for ${marriage.dissolutionReason ?? "unknown reasons"}`;

  return `${headMember?.title ?? "Bloodline member"}'s union with ${spouseHouse} ${reasonText} â€¢ ${childText} â€¢ ${dissolvedDay}`;
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

function renderRealmConditionBar() {
  const snapshot = getRealmConditionSnapshot(state, "player");
  if (!snapshot) {
    refs.realmConditionBar.innerHTML = "";
    return;
  }

  const cycleRemaining = Math.max(0, 100 - Math.round(snapshot.cycleProgress * 100));
  const cycleStageNumber = snapshot.cycle?.stageNumber ?? 1;
  const cycleStageLabel = snapshot.cycle?.stageLabel ?? "Founding";
  const cyclePhaseLabel = snapshot.cycle?.phaseLabel ?? "Emergence";
  const cycleInWorldYears = snapshot.dualClock?.inWorldYears ?? 0;
  const cycleDeclarations = snapshot.cycle?.declarationCount ?? 0;
  const nextStageShortfall = snapshot.cycle?.nextStageShortfalls?.[0] ?? null;
  const cycleGreatReckoningActive = snapshot.cycle?.greatReckoningActive ?? false;
  const cycleGreatReckoningTargeted = cycleGreatReckoningActive && snapshot.cycle?.greatReckoningTargetFactionId === "player";
  const cycleGreatReckoningShare = Math.round((snapshot.cycle?.greatReckoningShare ?? 0) * 100);
  const cycleGreatReckoningLabel = cycleGreatReckoningActive
    ? cycleGreatReckoningTargeted
      ? `Great Reckoning ${cycleGreatReckoningShare}%`
      : `Reckoning on ${snapshot.cycle?.greatReckoningTargetFactionName ?? "rival"}`
    : null;
  const degradedFieldWaterUnits = Math.max(
    0,
    (snapshot.logistics.fieldWaterStrainedUnits ?? 0) - (snapshot.logistics.fieldWaterCriticalUnits ?? 0),
  );
  const logisticsValue = (snapshot.logistics.fieldWaterDesertionRiskUnits ?? 0) > 0
    ? `${snapshot.logistics.fieldWaterDesertionRiskUnits} breaking`
    : (snapshot.logistics.fieldWaterAttritionUnits ?? 0) > 0
      ? `${snapshot.logistics.fieldWaterAttritionUnits} attriting`
    : snapshot.logistics.fieldWaterCriticalUnits > 0
        ? `${snapshot.logistics.fieldWaterCriticalUnits} critical`
    : (snapshot.logistics.interdictedSupplyWagonCount ?? 0) > 0
      ? `${snapshot.logistics.interdictedSupplyWagonCount} convoy cut`
    : (snapshot.logistics.convoyRecoveringCount ?? 0) > 0
      ? `${snapshot.logistics.convoyRecoveringCount} reforming`
    : (snapshot.logistics.harassedWorkerCount ?? 0) > 0
      ? `${snapshot.logistics.harassedWorkerCount} routed`
    : (snapshot.logistics.harassedResourceNodeCount ?? 0) > 0
      ? `${snapshot.logistics.harassedResourceNodeCount} node harried`
    : (snapshot.logistics.raidedSupplyCampCount ?? 0) > 0
      ? `${snapshot.logistics.raidedSupplyCampCount} camp raided`
      : (snapshot.logistics.raidedInfrastructureCount ?? 0) > 0
        ? `${snapshot.logistics.raidedInfrastructureCount} raided`
    : snapshot.logistics.engineCount > 0
      ? `${snapshot.logistics.suppliedEngines}/${snapshot.logistics.engineCount} supplied`
      : snapshot.logistics.fieldWaterUnitCount > 0
        ? `${snapshot.logistics.fieldWaterSupportedUnits}/${snapshot.logistics.fieldWaterUnitCount} hydrated`
        : snapshot.logistics.supplyCampCount > 0
          ? `${snapshot.logistics.supplyCampCount} camp${snapshot.logistics.supplyCampCount === 1 ? "" : "s"}`
          : "No siege line";
  const logisticsMetaParts = [];
  if (snapshot.logistics.supplyCampCount > 0 || snapshot.logistics.supplyWagonCount > 0 || snapshot.logistics.engineerCount > 0) {
    logisticsMetaParts.push(
      `Camps ${snapshot.logistics.supplyCampCount}`,
      `wagons ${snapshot.logistics.supplyWagonCount}`,
      `eng ${snapshot.logistics.engineerCount}`,
    );
  }
  if ((snapshot.logistics.fieldWaterUnitCount ?? 0) > 0) {
    logisticsMetaParts.push(`water ${snapshot.logistics.fieldWaterSupportedUnits}/${snapshot.logistics.fieldWaterUnitCount}`);
  }
  if (degradedFieldWaterUnits > 0) {
    logisticsMetaParts.push(`${degradedFieldWaterUnits} strained`);
  }
  if ((snapshot.logistics.fieldWaterCriticalUnits ?? 0) > 0) {
    logisticsMetaParts.push(`${snapshot.logistics.fieldWaterCriticalUnits} critical`);
  }
  if ((snapshot.logistics.fieldWaterAttritionUnits ?? 0) > 0) {
    logisticsMetaParts.push(`${snapshot.logistics.fieldWaterAttritionUnits} attriting`);
  }
  if ((snapshot.logistics.fieldWaterDesertionRiskUnits ?? 0) > 0) {
    logisticsMetaParts.push(`${snapshot.logistics.fieldWaterDesertionRiskUnits} desertion risk`);
  }
  if ((snapshot.logistics.fieldWaterSourceCount ?? 0) > 0) {
    logisticsMetaParts.push(`anchors ${snapshot.logistics.fieldWaterSourceCount}`);
  }
  if ((snapshot.logistics.interdictedSupplyWagonCount ?? 0) > 0) {
    logisticsMetaParts.push(`convoys cut ${snapshot.logistics.interdictedSupplyWagonCount}`);
  }
  if ((snapshot.logistics.convoyRecoveringCount ?? 0) > 0) {
    logisticsMetaParts.push(`reforming ${snapshot.logistics.convoyRecoveringCount}`);
  }
  if ((snapshot.logistics.supplyWagonCount ?? 0) > 0) {
    logisticsMetaParts.push(`screened ${snapshot.logistics.convoyScreenedCount ?? 0}/${snapshot.logistics.supplyWagonCount}`);
  }
  if ((snapshot.logistics.escortedSupplyWagonCount ?? 0) > 0) {
    logisticsMetaParts.push(`escorted ${snapshot.logistics.escortedSupplyWagonCount}/${snapshot.logistics.supplyWagonCount}`);
  }
  if ((snapshot.logistics.unscreenedRecoveringCount ?? 0) > 0) {
    logisticsMetaParts.push(`unscreened ${snapshot.logistics.unscreenedRecoveringCount}`);
  }
  if (snapshot.logistics.convoyReconsolidated === false) {
    logisticsMetaParts.push("reconsolidating");
  }
  if ((snapshot.logistics.harassedResourceNodeCount ?? 0) > 0) {
    logisticsMetaParts.push(`nodes harried ${snapshot.logistics.harassedResourceNodeCount}`);
  }
  if ((snapshot.logistics.harassedWorkerCount ?? 0) > 0) {
    logisticsMetaParts.push(`workers routed ${snapshot.logistics.harassedWorkerCount}`);
  }
  if ((snapshot.logistics.raidedInfrastructureCount ?? 0) > 0) {
    logisticsMetaParts.push(`raids ${snapshot.logistics.raidedInfrastructureCount}`);
  }
  if ((snapshot.logistics.raidedWaterAnchorCount ?? 0) > 0) {
    logisticsMetaParts.push(`water raids ${snapshot.logistics.raidedWaterAnchorCount}`);
  }
  if ((snapshot.logistics.raidedDropoffCount ?? 0) > 0) {
    logisticsMetaParts.push(`dropoffs cut ${snapshot.logistics.raidedDropoffCount}`);
  }
  if (snapshot.logistics.formalSiegeReady) {
    logisticsMetaParts.push("formal ready");
  }
  const logisticsMeta = logisticsMetaParts.length > 0
    ? logisticsMetaParts.join(" â€¢ ")
    : "No camp, no wagon, no water anchors";
  const worldValue = snapshot.worldPressure.greatReckoningActive
    ? snapshot.worldPressure.greatReckoningTargeted
      ? `Great Reckoning ${Math.round((snapshot.worldPressure.greatReckoningShare ?? 0) * 100)}%`
      : `Reckoning on ${snapshot.worldPressure.greatReckoningTargetFactionName ?? "rival"}`
    : snapshot.worldPressure.hostilePostRepulseActive
    ? `Rival repulsed ${Math.ceil(snapshot.worldPressure.hostilePostRepulseRemaining)}s`
    : snapshot.worldPressure.targetedByWorld
      ? `${snapshot.worldPressure.pressureLabel} pressure`
      : snapshot.worldPressure.signals === 0
        ? "Quiet"
        : `${snapshot.worldPressure.signals} signal${snapshot.worldPressure.signals === 1 ? "" : "s"}`;
  const worldMetaParts = [
    `Tribes ${snapshot.worldPressure.tribeUnitCount}`,
    `contested ${snapshot.worldPressure.contestedTerritories}`,
    `ops ${snapshot.worldPressure.activeOperations}`,
  ];
  if (snapshot.worldPressure.greatReckoningActive) {
    worldMetaParts.push(`reckoning ${Math.round((snapshot.worldPressure.greatReckoningShare ?? 0) * 100)}%`);
    if (snapshot.worldPressure.greatReckoningTargetFactionName) {
      worldMetaParts.push(`target ${snapshot.worldPressure.greatReckoningTargetFactionName}`);
    }
    worldMetaParts.push(`threshold ${Math.round((snapshot.worldPressure.greatReckoningThreshold ?? 0.7) * 100)}%`);
  }
  if (snapshot.worldPressure.targetedByWorld) {
    worldMetaParts.push(`score ${snapshot.worldPressure.score}`);
    worldMetaParts.push(`streak ${snapshot.worldPressure.pressureStreak}`);
    if (snapshot.worldPressure.topPressureSourceLabel && snapshot.worldPressure.topPressureSourceLabel !== "quiet") {
      worldMetaParts.push(`source ${snapshot.worldPressure.topPressureSourceLabel}`);
    }
    if ((snapshot.worldPressure.pressureSourceBreakdown?.territorialGovernanceRecognition ?? 0) > 0) {
      worldMetaParts.push(`governance +${snapshot.worldPressure.pressureSourceBreakdown.territorialGovernanceRecognition}`);
      if (snapshot.dynasty?.activeTerritorialGovernanceRecognition) {
        worldMetaParts.push(`accept ${snapshot.dynasty.activeTerritorialGovernanceRecognition.populationAcceptancePct ?? 0}%/${snapshot.dynasty.activeTerritorialGovernanceRecognition.populationAcceptanceThresholdPct ?? 65}%`);
      }
    }
    if (snapshot.worldPressure.governanceAlliancePressureActive) {
      worldMetaParts.push(`coalition ${snapshot.worldPressure.governanceAlliancePressureHostileCount ?? 0} hostile`);
    }
    if ((snapshot.worldPressure.truebornRiseStage ?? 0) > 0) {
      const riseLabels = { 1: "Claims", 2: "Recognition", 3: "Restoration" };
      worldMetaParts.push(`Trueborn Rise ${riseLabels[snapshot.worldPressure.truebornRiseStage] ?? `S${snapshot.worldPressure.truebornRiseStage}`}`);
    }
    if ((snapshot.worldPressure.pressureSourceBreakdown?.territoryExpansion ?? 0) > 0) {
      worldMetaParts.push(`breadth +${snapshot.worldPressure.pressureSourceBreakdown.territoryExpansion}`);
    }
    if (snapshot.worldPressure.frontierLoyaltyPenalty > 0) {
      worldMetaParts.push(`frontier -${snapshot.worldPressure.frontierLoyaltyPenalty} loyalty`);
    }
    if (snapshot.worldPressure.legitimacyPressure > 0) {
      worldMetaParts.push(`legit -${snapshot.worldPressure.legitimacyPressure}`);
    }
    if ((snapshot.worldPressure.cadetLoyaltyPenalty ?? 0) < 0) {
      worldMetaParts.push(`cadet drift ${snapshot.worldPressure.cadetLoyaltyPenalty}/day`);
    }
    if ((snapshot.worldPressure.pressuredLesserHouseCount ?? 0) > 0) {
      worldMetaParts.push(`cadets ${snapshot.worldPressure.pressuredLesserHouseCount}`);
    }
    if ((snapshot.worldPressure.splinterOpportunityCount ?? 0) > 0) {
      worldMetaParts.push(`splinters ${snapshot.worldPressure.splinterOpportunityCount}`);
      worldMetaParts.push(`splinter levy x${snapshot.worldPressure.splinterLevyTempo}`);
      worldMetaParts.push(`splinter retake x${snapshot.worldPressure.splinterRetakeTempo}`);
      if ((snapshot.worldPressure.splinterRetinueCapBonus ?? 0) > 0) {
        worldMetaParts.push(`splinter cap +${snapshot.worldPressure.splinterRetinueCapBonus}`);
      }
    }
    if (snapshot.worldPressure.convergenceActive) {
      if ((snapshot.worldPressure.rivalSabotageTimerCap ?? 0) > 0) {
        worldMetaParts.push(`convergence sabotage ${snapshot.worldPressure.rivalSabotageTimerCap}s`);
      }
      if ((snapshot.worldPressure.rivalEspionageTimerCap ?? 0) > 0) {
        worldMetaParts.push(`espionage ${snapshot.worldPressure.rivalEspionageTimerCap}s`);
      }
      if ((snapshot.worldPressure.rivalHolyWarTimerCap ?? 0) > 0) {
        worldMetaParts.push(`holy war ${snapshot.worldPressure.rivalHolyWarTimerCap}s`);
      }
      if ((snapshot.worldPressure.tribalRaidTimerMultiplier ?? 1) < 1) {
        worldMetaParts.push(`tribal cadence x${snapshot.worldPressure.tribalRaidTimerMultiplier}`);
      }
    }
  } else if (snapshot.worldPressure.leaderFactionName) {
    worldMetaParts.push(`leader ${snapshot.worldPressure.leaderFactionName}`);
    worldMetaParts.push(`score ${snapshot.worldPressure.leaderScore}`);
  }
  if (snapshot.worldPressure.heldCaptives > 0) {
    worldMetaParts.push(`held ${snapshot.worldPressure.heldCaptives}`);
  }
  if (snapshot.worldPressure.fallenMembers > 0) {
    worldMetaParts.push(`fallen ${snapshot.worldPressure.fallenMembers}`);
  }
  if (snapshot.worldPressure.hostileCohesionStrain > 0) {
    worldMetaParts.push(`rival strain ${snapshot.worldPressure.hostileCohesionStrain}`);
  }
  if (snapshot.worldPressure.offHomeContinentHoldings > 0) {
    worldMetaParts.push(`off-home holdings ${snapshot.worldPressure.offHomeContinentHoldings}`);
  }
  const worldMeta = worldMetaParts.join(" â€¢ ");
  // Canonical 11-state realm-condition dashboard per master doctrine section XVI.
  // Order: cycle, population, food, water, loyalty, fortification, army, faith, conviction, logistics, world-pressure.
  const entries = [
    {
      label: "Cycle",
      value: `S${cycleStageNumber}`,
      band: snapshot.cycle?.band ?? "green",
      meta: `Year ${cycleInWorldYears.toFixed(2)} | ${cyclePhaseLabel} | cycle #${snapshot.cycleCount} | declarations ${cycleDeclarations}${nextStageShortfall ? ` | next: ${nextStageShortfall}` : ` | ${cycleStageLabel}`}${cycleGreatReckoningLabel ? ` | ${cycleGreatReckoningLabel}` : ""}`,
    },
    {
      label: "Pop",
      value: `${snapshot.population.value}/${snapshot.population.cap}`,
      band: snapshot.population.bloodProductionActive ? "yellow" : snapshot.population.band,
      meta: snapshot.population.bloodProductionActive
        ? `${Math.round((snapshot.population.value / Math.max(1, snapshot.population.cap)) * 100)}% capacity • blood levy load ${snapshot.population.bloodProductionLoad} (growth slowed)`
        : snapshot.population.bloodProductionLoad > 0
          ? `${Math.round((snapshot.population.value / Math.max(1, snapshot.population.cap)) * 100)}% capacity • blood levy load ${snapshot.population.bloodProductionLoad}`
          : `${Math.round((snapshot.population.value / Math.max(1, snapshot.population.cap)) * 100)}% capacity`,
    },
    {
      label: "Food",
      value: `${snapshot.food.stock}/${snapshot.food.need}`,
      band: snapshot.food.band,
      meta: snapshot.food.famineStreak > 0 ? `Famine streak ${snapshot.food.famineStreak}` : "Stores holding",
    },
    {
      label: "Water",
      value: `${snapshot.water.stock}/${snapshot.water.need}`,
      band: snapshot.water.band,
      meta: snapshot.water.crisisStreak > 0 ? `Crisis streak ${snapshot.water.crisisStreak}` : "Supply holding",
    },
    {
      label: "Loyalty",
      value: `${snapshot.loyalty.average}`,
      band: snapshot.loyalty.band,
      meta: `Min ${snapshot.loyalty.min} across ${snapshot.loyalty.territoryCount} marches${snapshot.loyalty.verdantWardenSupportedTerritories > 0 ? ` • wardens guard ${snapshot.loyalty.verdantWardenSupportedTerritories}${snapshot.loyalty.verdantWardenPeakCoverage > 1 ? ` • peak ${snapshot.loyalty.verdantWardenPeakCoverage}` : ""} • loyalty +${snapshot.loyalty.verdantWardenLoyaltyGainBonus}% • shield +${snapshot.loyalty.verdantWardenProtectionBonus}%` : ""}`,
    },
    {
      label: "Fort",
      value: `${snapshot.fortification.tier}/${snapshot.fortification.ceiling}`,
      band: snapshot.fortification.band,
      meta: snapshot.fortification.primaryKeepName
        ? `${snapshot.fortification.primaryKeepName} • ${snapshot.fortification.readyReserves} ready • ${snapshot.fortification.recoveringReserves} recovering${snapshot.fortification.commanderPresent ? " • commander present" : ""}${snapshot.fortification.governorPresent ? " • governor present" : ""}${snapshot.fortification.bloodlinePresent ? " • bloodline seated" : ""}${snapshot.fortification.governorSpecializationLabel ? ` • ${snapshot.fortification.governorSpecializationLabel}` : ""}${snapshot.fortification.wardLabel ? ` • ${snapshot.fortification.wardLabel}` : ""}${snapshot.fortification.wardSurgeActive ? " • surge active" : ""}${snapshot.fortification.verdantWardenCount > 0 ? ` • wardens ${snapshot.fortification.verdantWardenCount} • attack +${snapshot.fortification.verdantWardenAttackBonus}% • heal +${snapshot.fortification.verdantWardenReserveHealBonus}% • muster +${snapshot.fortification.verdantWardenReserveMusterBonus}%` : ""}${snapshot.fortification.threatActive ? " • under threat" : ""}`
        : "No primary keep anchor",
    },
    {
      label: "Army",
      value: `${snapshot.military.count}`,
      band: snapshot.military.band ?? (snapshot.military.cohesionPenaltyActive
        ? "red"
        : snapshot.military.unsuppliedEngines > 0
          ? "yellow"
          : snapshot.military.assaultStrain >= 3
            ? "yellow"
            : "green"),
      meta: snapshot.military.cohesionPenaltyActive
        ? "Cohesion penalty active"
        : `Assault strain ${snapshot.military.assaultStrain}/6${snapshot.military.siegeEngineCount > 0 ? ` • ${snapshot.military.siegeEngineCount} engines` : ""}`,
    },
    {
      label: "Faith",
      value: snapshot.faith.selectedFaithName
        ? `${snapshot.faith.selectedFaithName}`
        : snapshot.faith.discoveredFaithCount > 0
          ? `${snapshot.faith.discoveredFaithCount} found`
          : "None",
      band: snapshot.faith.band,
      meta: snapshot.faith.selectedFaithName
        ? `${snapshot.faith.doctrinePath ?? "—"} • ${snapshot.faith.tierLabel ?? "Latent"} • intensity ${snapshot.faith.intensity}`
        : snapshot.faith.discoveredFaithCount > 0
          ? "Discovered, not committed"
          : "No covenant found",
    },
    {
      label: "Conviction",
      value: snapshot.conviction.bandLabel,
      band: snapshot.conviction.band,
      meta: (() => {
        const bandEffects = snapshot.conviction.bandEffects ?? {};
        const parts = [];
        parts.push(`Score ${snapshot.conviction.score}`);
        if (snapshot.conviction.topBucketKey) parts.push(`top ${snapshot.conviction.topBucketKey} (${snapshot.conviction.topBucketValue})`);
        const modifierBits = [];
        if (bandEffects.stabilizationMultiplier && bandEffects.stabilizationMultiplier !== 1) modifierBits.push(`stab x${bandEffects.stabilizationMultiplier}`);
        if (bandEffects.captureMultiplier && bandEffects.captureMultiplier !== 1) modifierBits.push(`capture x${bandEffects.captureMultiplier}`);
        if (bandEffects.populationGrowthMultiplier && bandEffects.populationGrowthMultiplier !== 1) modifierBits.push(`growth x${bandEffects.populationGrowthMultiplier}`);
        if (modifierBits.length > 0) parts.push(modifierBits.join(" "));
        return parts.join(" • ");
      })(),
    },
    {
      label: "Logistics",
      value: logisticsValue,
      band: snapshot.logistics.band,
      meta: logisticsMeta, /* legacy encoded branch retained only to avoid line-encoding patch churn
        ? `Camps ${snapshot.logistics.supplyCampCount} • wagons ${snapshot.logistics.supplyWagonCount} • eng ${snapshot.logistics.engineerCount}${snapshot.logistics.formalSiegeReady ? " • formal ready" : ""}`
      */
    },
    {
      label: "World",
      value: worldValue,
      band: snapshot.worldPressure.greatReckoningTargeted
        ? "red"
        : snapshot.worldPressure.hostilePostRepulseActive
          ? "green"
          : snapshot.worldPressure.band,
      meta: `Tribes ${snapshot.worldPressure.tribeUnitCount} • contested ${snapshot.worldPressure.contestedTerritories} • ops ${snapshot.worldPressure.activeOperations}${snapshot.worldPressure.heldCaptives > 0 ? ` • held ${snapshot.worldPressure.heldCaptives}` : ""}${snapshot.worldPressure.fallenMembers > 0 ? ` • fallen ${snapshot.worldPressure.fallenMembers}` : ""}${snapshot.worldPressure.hostileCohesionStrain > 0 ? ` • rival strain ${snapshot.worldPressure.hostileCohesionStrain}` : ""}${snapshot.worldPressure.offHomeContinentHoldings > 0 ? ` • off-home holdings ${snapshot.worldPressure.offHomeContinentHoldings}` : ""}`,
    },
  ];
  entries[entries.length - 1].meta = worldMeta;

  refs.realmConditionBar.innerHTML = `
    <div class="realm-cycle-meta">Realm dashboard • 11-state</div>
    ${entries.map((entry) => `
      <div class="pressure-pill ${entry.band}">
        <span class="pressure-label">${entry.label}</span>
        <span class="pressure-value"><span class="pressure-dot"></span>${entry.value}</span>
        <span class="pressure-meta">${entry.meta}</span>
      </div>
    `).join("")}
  `;
  const cycleMetaNode = refs.realmConditionBar.querySelector(".realm-cycle-meta");
  if (cycleMetaNode) {
    const fortImminent = snapshot.fortification.imminentEngagement;
    const activeTerritorialGovernanceRecognition = snapshot.dynasty.activeTerritorialGovernanceRecognition;
    const activeDivineRight = snapshot.faith.activeDivineRightDeclaration;
    const incomingDivineRight = (snapshot.faith.incomingDivineRightDeclarations ?? [])[0] ?? null;
    const fortImmediateLabel = fortImminent?.phase === "countdown"
      ? ` | Immediate ${Math.ceil(fortImminent.remainingSeconds ?? 0)}s ${fortImminent.selectedResponseLabel ?? "Steady Defense"}`
      : fortImminent?.phase === "engaged"
        ? ` | Engagement joined ${fortImminent.selectedResponseLabel ?? "Steady Defense"}`
        : "";
    const governanceLabel = formatGovernanceStatusSummary(activeTerritorialGovernanceRecognition);
    const divineRightLabel = activeDivineRight
      ? ` | Divine Right ${Math.ceil(activeDivineRight.remainingSeconds ?? 0)}s ${activeDivineRight.recognitionSharePct ?? 0}%/${activeDivineRight.requiredSharePct ?? 0}%`
      : incomingDivineRight
        ? ` | Rival Divine Right ${Math.ceil(incomingDivineRight.remainingSeconds ?? 0)}s ${incomingDivineRight.sourceFactionName ?? "Rival"}`
        : "";
    cycleMetaNode.textContent = `Realm dashboard | 11-state | Stage ${cycleStageNumber} ${cycleStageLabel} | ${cyclePhaseLabel} | Year ${cycleInWorldYears.toFixed(2)}${cycleGreatReckoningLabel ? ` | ${cycleGreatReckoningLabel}` : ""}${fortImmediateLabel}${governanceLabel}${divineRightLabel} | next cycle ${cycleRemaining}%`;
  }
}

function renderSelectionPanel() {
  const { units, buildings } = getSelectedEntities();
  const player = getFactionSnapshot(state, "player");
  if (units.length === 0 && buildings.length === 0) {
    refs.selectionPanel.innerHTML = `<p class="empty-state">Select units or a structure to inspect available commands.</p>`;
    return;
  }

  const rows = [];
  units.forEach((unit) => {
    const unitDef = content.byId.units[unit.typeId];
    const commander = unit.commanderMemberId
      ? player.dynasty?.members.find((member) => member.id === unit.commanderMemberId)
      : null;
    const raidMeta = (unitDef.raidDurationSeconds ?? 0) > 0
      ? ` â€¢ raid ${(unit.raidCooldownRemaining ?? 0) > 0 ? `${Math.ceil(unit.raidCooldownRemaining)}s` : "ready"}`
      : "";
    const supportMeta = unit.supportStatus ? ` • ${unit.supportStatus}` : "";
    rows.push(`
      <div class="selection-item">
        <span>${unitDef.name}${commander ? ` • ${commander.title}` : ""}</span>
        <span class="selection-meta">${Math.ceil(unit.health)}/${unitDef.health}${supportMeta}${raidMeta}</span>
      </div>
    `);
  });
  buildings.forEach((building) => {
    const buildingDef = content.byId.buildings[building.typeId];
    const raidRemaining = Math.max(0, (building.raidedUntil ?? 0) - state.meta.elapsed);
    rows.push(`
      <div class="selection-item">
        <span>${buildingDef.name}</span>
        <span class="selection-meta">${building.completed ? (raidRemaining > 0 ? `Raided ${Math.ceil(raidRemaining)}s` : "Online") : "Building"}</span>
      </div>
    `);
  });

  refs.selectionPanel.innerHTML = `<div class="selection-grid">${rows.join("")}</div>`;
}

function createActionButton({ label, detail, onClick, warning = false, disabled = false }) {
  const button = document.createElement("button");
  button.className = "action-btn";
  button.type = "button";
  button.disabled = disabled;
  button.innerHTML = `<strong>${label}</strong><span class="${warning ? "warning-text" : ""}">${detail}</span>`;
  if (!disabled) {
    button.addEventListener("click", onClick);
  }
  return button;
}

function formatCostSummary(cost = {}) {
  return Object.entries(cost).map(([key, value]) => `${key} ${value}`).join(", ");
}

function formatFaithOperationTerms(terms) {
  if (!terms?.available) {
    return terms?.reason ?? "Unavailable";
  }
  const details = [];
  if (terms.cost && Object.keys(terms.cost).length > 0) {
    details.push(formatCostSummary(terms.cost));
  }
  if (terms.intensityCost) {
    details.push(`intensity ${terms.intensityCost}`);
  }
  if (terms.duration) {
    details.push(formatOperationDuration(terms.duration));
  }
  if (typeof terms.projectedChance === "number") {
    details.push(`${Math.round(terms.projectedChance * 100)}% success`);
  }
  if (terms.compatibilityLabel) {
    details.push(terms.compatibilityLabel);
  }
  if (typeof terms.recognitionSharePct === "number" && typeof terms.requiredSharePct === "number") {
    details.push(`${terms.recognitionSharePct}% / ${terms.requiredSharePct}% recognition`);
  }
  if (terms.structureName) {
    details.push(terms.structureName);
  }
  return details.join(" | ");
}

function formatCovenantTestOutcome(outcome) {
  if (!outcome) {
    return "No prior Covenant Test outcome is recorded.";
  }
  return `Last Covenant Test ${outcome.outcome === "completed" ? "completed" : "failed"}: ${outcome.mandateLabel ?? "Unknown mandate"}${outcome.resolutionReason ? ` | ${outcome.resolutionReason}` : ""}.`;
}

function formatMarriageAuthoritySummary(authority) {
  if (!authority?.available) {
    return authority?.reason ?? "No household authority";
  }
  const details = [
    authority.title ?? "Unknown authority",
    authority.label ?? "Household approval",
  ];
  if (authority.legitimacyCost) {
    details.push(`legitimacy -${authority.legitimacyCost}`);
  }
  return details.join(" | ");
}

function formatMarriageEnvoySummary(envoy) {
  if (!envoy?.available) {
    return envoy?.reason ?? "No diplomatic envoy";
  }
  return envoy.title ?? "Diplomatic envoy";
}

function formatMarriageProposalTerms(terms, compatibility) {
  if (!terms?.available) {
    return terms?.reason ?? "Unavailable";
  }
  const details = [];
  if (compatibility?.tier) {
    details.push(compatibility.tier);
  }
  details.push(formatMarriageAuthoritySummary(terms.sourceAuthority));
  if (terms.sourceEnvoy?.available) {
    details.push(`Envoy ${terms.sourceEnvoy.title}`);
  }
  if (terms.targetAuthorityAvailable === false) {
    details.push(terms.targetAuthorityReason ?? "Target household cannot currently approve");
  }
  return details.join(" | ");
}

function formatMarriageAcceptanceTerms(terms, compatibility) {
  if (!terms?.available) {
    return terms?.reason ?? "Unavailable";
  }
  const details = [];
  if (compatibility?.tier) {
    details.push(compatibility.tier);
  }
  details.push(formatMarriageAuthoritySummary(terms.targetAuthority));
  return details.join(" | ");
}

function hasEnoughResources(resources, cost = {}) {
  return Object.entries(cost).every(([key, value]) => (resources[key] ?? 0) >= value);
}

function appendImminentEngagementPanel(container, snapshot) {
  const fortification = snapshot?.fortification;
  const imminent = fortification?.imminentEngagement;
  if (!imminent || imminent.phase === "idle") {
    return false;
  }

  const wrapper = document.createElement("div");
  wrapper.className = "subpanel-grid";

  const header = document.createElement("p");
  header.className = "command-meta";
  if (imminent.phase === "countdown") {
    header.innerHTML = `<strong>Immediate:</strong> ${imminent.battleTypeLabel ?? "Engagement"} at ${imminent.settlementName ?? "the keep"} in ${Math.ceil(imminent.remainingSeconds ?? 0)}s. Hostiles ${imminent.hostileCount ?? 0}${imminent.hostileSiegeCount > 0 ? ` | siege ${imminent.hostileSiegeCount}` : ""}${imminent.watchtowerCount > 0 ? ` | towers ${imminent.watchtowerCount}` : ""}${imminent.bloodlineAtRisk ? " | bloodline at risk" : ""}`;
  } else {
    header.innerHTML = `<strong>Engagement Joined:</strong> ${imminent.settlementName ?? "the keep"} is fighting under ${imminent.selectedResponseLabel ?? "Steady Defense"}.`;
  }
  wrapper.appendChild(header);

  if (imminent.phase === "countdown") {
    const postureRow = document.createElement("div");
    postureRow.className = "action-grid";
    [
      { id: "brace", label: "Brace", detail: "Reserve heal +20% | slower hostile approach" },
      { id: "steady", label: "Steady", detail: "Hold current defense plan" },
      { id: "counterstroke", label: "Counterstroke", detail: "Attack +10% | auto-sortie on expiry" },
    ].forEach((posture) => {
      postureRow.appendChild(
        createActionButton({
          label: imminent.selectedResponseId === posture.id ? `${posture.label} Active` : posture.label,
          detail: posture.detail,
          disabled: imminent.selectedResponseId === posture.id,
          warning: posture.id === "counterstroke" && imminent.bloodlineAtRisk,
          onClick: () => {
            const result = setImminentEngagementPosture(state, "player", imminent.settlementId, posture.id);
            if (!result.ok) {
              pushUiMessage(result.reason ?? "Could not change imminent-engagement posture.");
            }
            renderPanels();
          },
        }),
      );
    });
    wrapper.appendChild(postureRow);

    const actionRow = document.createElement("div");
    actionRow.className = "action-grid";
    actionRow.appendChild(
      createActionButton({
        label: imminent.reinforcementsCommitted ? "Reinforcements Committed" : "Commit Reinforcements",
        detail: imminent.reinforcementsCommitted
          ? `${Math.ceil(imminent.reinforcementsRemaining ?? 0)}s surge remaining`
          : `${imminent.readyReserveCount ?? 0} ready reserves | ${imminent.hostileCount ?? 0} hostile`,
        disabled: imminent.reinforcementsCommitted || (imminent.readyReserveCount ?? 0) <= 0,
        warning: !imminent.reinforcementsCommitted && (imminent.hostileCount ?? 0) > (imminent.readyReserveCount ?? 0),
        onClick: () => {
          const result = commitImminentEngagementReinforcements(state, "player", imminent.settlementId);
          if (!result.ok) {
            pushUiMessage(result.reason ?? "Could not commit reinforcements.");
          }
          renderPanels();
        },
      }),
    );
    actionRow.appendChild(
      createActionButton({
        label: fortification?.commanderAtKeep
          ? "Commander Present"
          : imminent.commanderRecallIssued
            ? "Commander Recalled"
            : "Recall Commander",
        detail: fortification?.commanderAtKeep
          ? "Commander already at threatened keep"
          : imminent.commanderRecallAvailable
            ? "Issue immediate recall to the keep"
            : "No recall path available",
        disabled: fortification?.commanderAtKeep || !imminent.commanderRecallAvailable || imminent.commanderRecallIssued,
        onClick: () => {
          const result = issueImminentEngagementCommanderRecall(state, "player", imminent.settlementId);
          if (!result.ok) {
            pushUiMessage(result.reason ?? "Could not recall the commander.");
          }
          renderPanels();
        },
      }),
    );
    actionRow.appendChild(
      createActionButton({
        label: imminent.bloodlineProtectionActive ? "Bloodline Guard Active" : "Protect Bloodline",
        detail: imminent.bloodlineProtectionActive
          ? `${Math.ceil(imminent.bloodlineProtectionRemaining ?? 0)}s emergency guard remaining`
          : imminent.bloodlineAtRisk
            ? "Harden the ruling seat against covert bloodline strikes"
            : "No seated bloodline members are under immediate threat",
        disabled: imminent.bloodlineProtectionActive || !imminent.bloodlineAtRisk,
        warning: imminent.bloodlineAtRisk,
        onClick: () => {
          const result = protectImminentEngagementBloodline(state, "player", imminent.settlementId);
          if (!result.ok) {
            pushUiMessage(result.reason ?? "Could not raise emergency bloodline guard.");
          }
          renderPanels();
        },
      }),
    );
    actionRow.appendChild(
      createActionButton({
        label: fortification?.sortieActive
          ? "Sortie Active"
          : fortification?.sortieCooldownRemaining > 0
            ? "Sortie Cooling"
            : fortification?.sortieReady
              ? "Call Sortie"
              : "Sortie Locked",
        detail: fortification?.sortieActive
          ? `${Math.ceil(fortification.sortieActiveRemaining ?? 0)}s remaining`
          : fortification?.sortieCooldownRemaining > 0
            ? `${Math.ceil(fortification.sortieCooldownRemaining ?? 0)}s until ready`
            : fortification?.sortieReady
              ? "12s keep burst under commander command"
              : fortification?.commanderAtKeep
                ? "Threat not yet at the keep"
                : "Commander must reach the keep",
        disabled: Boolean(fortification?.sortieActive || (fortification?.sortieCooldownRemaining ?? 0) > 0 || !fortification?.sortieReady),
        warning: fortification?.sortieReady && imminent.bloodlineAtRisk,
        onClick: () => {
          const result = issueKeepSortie(state, "player", imminent.settlementId);
          if (!result.ok) {
            pushUiMessage(result.reason ?? "Sortie could not be called.");
          }
          renderPanels();
        },
      }),
    );
    wrapper.appendChild(actionRow);
  }

  container.appendChild(wrapper);
  return true;
}

function renderCommandPanel() {
  refs.commandPanel.innerHTML = "";
  const { units, buildings } = getSelectedEntities();
  const workerSelection = getSelectedWorkers();
  const player = getFactionSnapshot(state, "player");
  const realmSnapshot = getRealmConditionSnapshot(state, "player");
  const hasImminentPanel = appendImminentEngagementPanel(refs.commandPanel, realmSnapshot);

  if (uiState.buildMode) {
    const buildMessage = document.createElement("p");
    buildMessage.className = "command-meta";
    buildMessage.innerHTML = `Placing <strong>${content.byId.buildings[uiState.buildMode].name}</strong>. Left click on the map to build. Right click to cancel.`;
    refs.commandPanel.appendChild(buildMessage);
    return;
  }

  const actionGrid = document.createElement("div");
  actionGrid.className = "action-grid";

  if (workerSelection.length > 0 && workerSelection.length === units.length) {
    WORKER_BUILD_ORDER.forEach((buildingId) => {
      const building = content.byId.buildings[buildingId];
      const affordable = hasEnoughResources(player.resources, building.cost);
      const costSummary = `${formatCostSummary(building.cost)}${affordable ? "" : " • insufficient resources"}`;
      actionGrid.appendChild(
        createActionButton({
          label: building.name,
          detail: costSummary,
          disabled: !affordable,
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
    getTrainableUnitIdsForBuilding(state, building.id).forEach((unitId) => {
      const unitDef = content.byId.units[unitId];
      const ironmarkBloodPrice = player.houseId === "ironmark" && unitDef.role !== "worker"
        ? Math.max(0, unitDef.ironmarkBloodPrice ?? 1)
        : 0;
      const bloodProductionLoadDelta = player.houseId === "ironmark" && unitDef.role !== "worker"
        ? Math.max(0, unitDef.bloodProductionLoadDelta ?? 1.5)
        : 0;
      const survivingPopulation = (player.population.total ?? 0) - ironmarkBloodPrice;
      const bloodLevyAffordable = ironmarkBloodPrice === 0
        || survivingPopulation >= ((player.population.used ?? 0) + (player.population.reserved ?? 0) + unitDef.populationCost);
      const affordable = hasEnoughResources(player.resources, unitDef.cost) &&
        player.population.available >= unitDef.populationCost &&
        bloodLevyAffordable;
      const costTerms = [formatCostSummary(unitDef.cost), `pop ${unitDef.populationCost}`];
      if (ironmarkBloodPrice > 0) {
        costTerms.push(`blood ${ironmarkBloodPrice}`);
      }
      if (bloodProductionLoadDelta > 0) {
        costTerms.push(`load +${bloodProductionLoadDelta}`);
      }
      if (!affordable) {
        costTerms.push("insufficient reserves");
      }
      const costSummary = `${formatCostSummary(unitDef.cost)} • pop ${unitDef.populationCost}${affordable ? "" : " • insufficient reserves"}`;
      const displayCostSummary = costTerms.join(" • ");
      actionGrid.appendChild(
        createActionButton({
          label: `Train ${unitDef.name}`,
          detail: displayCostSummary,
          disabled: !affordable,
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
    if (hasImminentPanel) {
      return;
    }
    refs.commandPanel.innerHTML = `<p class="empty-state">This selection has no direct build or training actions yet.</p>`;
    return;
  }

  refs.commandPanel.appendChild(actionGrid);
}

function buildDynastyDetailLine(text, strongPrefix = null) {
  const line = document.createElement("div");
  line.className = "member-detail";
  if (strongPrefix) {
    const strong = document.createElement("strong");
    strong.textContent = strongPrefix;
    line.appendChild(strong);
    line.appendChild(document.createTextNode(` ${text}`));
  } else {
    line.textContent = text;
  }
  return line;
}

function buildDynastyMemberCard(member) {
  const card = document.createElement("div");
  card.className = "dynasty-member";

  const header = document.createElement("div");
  header.className = "member-title";
  const nameNode = document.createElement("strong");
  nameNode.textContent = member.title;
  const ageNode = document.createElement("span");
  ageNode.className = "selection-meta";
  ageNode.textContent = `Age ${member.age}`;
  header.appendChild(nameNode);
  header.appendChild(ageNode);
  card.appendChild(header);

  const roleLine = document.createElement("div");
  roleLine.className = "member-detail";
  roleLine.textContent = `${member.role.name} • ${member.path.name}`;
  card.appendChild(roleLine);

  const statusLine = document.createElement("div");
  statusLine.className = "member-detail";
  statusLine.textContent = `Status: ${member.status} • Renown ${member.renown}`;
  card.appendChild(statusLine);

  return card;
}

function pushUiMessage(text, tone = "warn") {
  state.messages.unshift({
    id: `ui-msg-${Date.now()}`,
    text,
    tone,
    ttl: 4,
  });
  state.messages = state.messages.slice(0, 6);
}

function formatOperationDuration(seconds) {
  return `${Math.max(1, Math.round(seconds ?? 0))}s`;
}

function buildDynastyOperationText(operation) {
  const label = operation.type === "rescue" ? "Rescue cell" : "Ransom envoy";
  return `${label} for ${operation.memberTitle} â€¢ ${operation.targetFactionName} â€¢ ${Math.round((operation.progress ?? 0) * 100)}%`;
}

function buildDynastyOperationSummaryText(operation) {
  if (operation.type === "missionary") {
    return `Missionaries | ${operation.targetFactionName} | ${Math.round((operation.progress ?? 0) * 100)}%`;
  }
  if (operation.type === "holy_war") {
    return `Holy war declaration | ${operation.targetFactionName} | ${Math.round((operation.progress ?? 0) * 100)}%`;
  }
  if (operation.type === "counter_intelligence") {
    return `Counter-intelligence watch | ${Math.round((operation.progress ?? 0) * 100)}%`;
  }
  if (operation.type === "espionage") {
    return `Espionage web | ${operation.targetFactionName} | ${Math.round((operation.progress ?? 0) * 100)}%`;
  }
  if (operation.type === "assassination") {
    return `Assassination cell | ${operation.memberTitle} | ${operation.targetFactionName} | ${Math.round((operation.progress ?? 0) * 100)}%`;
  }
  if (operation.type === "sabotage") {
    const dossierText = operation.intelligenceReportId
      ? ` | dossier ${operation.retaliationInterceptType ?? "retaliation"} +${operation.intelligenceSupportBonus ?? 0}`
      : "";
    return `Sabotage, ${(operation.subtype ?? "unknown").replace(/_/g, " ")} | ${operation.targetFactionName}${dossierText} | ${Math.round((operation.progress ?? 0) * 100)}%`;
  }
  return buildDynastyOperationText(operation);
}

function getDynastyTargetPriority(roleId) {
  switch (roleId) {
    case "head_of_bloodline":
      return 0;
    case "heir_designate":
      return 1;
    case "commander":
      return 2;
    case "spymaster":
      return 3;
    case "governor":
      return 4;
    default:
      return 6;
  }
}

function getDynastyCovertTargets(rivalSnapshot, report = null) {
  if (report?.members?.length) {
    return report.members
      .slice()
      .sort((left, right) => {
        const leftPriority = getDynastyTargetPriority(left.roleId);
        const rightPriority = getDynastyTargetPriority(right.roleId);
        if (leftPriority !== rightPriority) {
          return leftPriority - rightPriority;
        }
        return (right.renown ?? 0) - (left.renown ?? 0);
      });
  }

  return (rivalSnapshot?.dynasty?.members ?? [])
    .filter((member) => (member.status === "active" || member.status === "ruling") && !member.capturedByFactionId)
    .sort((left, right) => {
      const leftPriority = getDynastyTargetPriority(left.roleId);
      const rightPriority = getDynastyTargetPriority(right.roleId);
      if (leftPriority !== rightPriority) {
        return leftPriority - rightPriority;
      }
      return (right.renown ?? 0) - (left.renown ?? 0);
    });
}

function renderDynastyPanel() {
  const player = getFactionSnapshot(state, "player");
  refs.dynastyPanel.replaceChildren();

  if (!player.dynasty) {
    const empty = document.createElement("p");
    empty.className = "empty-state";
    empty.textContent = "No active bloodline roster is present for this faction.";
    refs.dynastyPanel.appendChild(empty);
    return;
  }

  const grid = document.createElement("div");
  grid.className = "subpanel-grid";

  const livingMembers = player.dynasty.members.filter((member) => member.status !== "fallen" && member.status !== "captured");
  const capturedMembers = player.dynasty.members.filter((member) => member.status === "captured");
  const dynastyOperations = player.dynasty.operations?.active ?? [];
  const intelligenceReports = player.dynasty.intelligenceReports ?? [];
  const counterIntelligenceWatches = player.dynasty.counterIntelligence ?? [];
  const rivalDynasties = Object.values(state.factions)
    .filter((faction) => faction.id !== "player" && faction.dynasty && faction.kind !== "tribes")
    .map((faction) => getFactionSnapshot(state, faction.id));
  const activeSuccessionCrisis = player.dynasty.activeSuccessionCrisis ?? null;
  const successionCrisisTerms = activeSuccessionCrisis ? getSuccessionCrisisTerms(state, "player") : null;
  const activeTerritorialGovernanceRecognition = player.dynasty.activeTerritorialGovernanceRecognition ?? null;
  const lastTerritorialGovernanceOutcome = player.dynasty.lastTerritorialGovernanceOutcome ?? null;

  const legitimacyText = `Legitimacy ${Math.round(player.dynasty.legitimacy)} • Active members ${livingMembers.length}/${player.dynasty.activeMemberCap}${player.dynasty.interregnum ? " • INTERREGNUM" : ""}`;
  grid.appendChild(buildDynastyDetailLine(legitimacyText));

  const commanderText = player.dynasty.attachments.commanderMember && player.dynasty.attachments.commanderUnit
    ? `${player.dynasty.attachments.commanderMember.title} commands ${content.byId.units[player.dynasty.attachments.commanderUnit.typeId].name}`
    : "No active commander is attached to a field unit.";
  grid.appendChild(buildDynastyDetailLine(commanderText));

  const heirText = player.dynasty.attachments.heirMember
    ? `Heir designate: ${player.dynasty.attachments.heirMember.title}`
    : player.dynasty.interregnum
      ? "Interregnum: no heir acknowledged."
      : "Heir line holds the default succession chain.";
  grid.appendChild(buildDynastyDetailLine(heirText));

  const governorText = player.dynasty.attachments.governorAssignments[0]
    ? `${player.dynasty.attachments.governorAssignments[0].member.title} governs ${player.dynasty.attachments.governorAssignments[0].anchor?.name ?? "an assigned domain"}${player.dynasty.attachments.governorAssignments[0].specialization?.label ? ` • ${player.dynasty.attachments.governorAssignments[0].specialization.label}` : ""}`
    : "No governed march assignment is active.";
  grid.appendChild(buildDynastyDetailLine(governorText));
  const governanceSeatAssignments = player.dynasty.attachments.governorAssignments ?? [];
  if (governanceSeatAssignments.length > 1) {
    grid.appendChild(buildDynastyDetailLine(
      `Additional seats: ${governanceSeatAssignments.slice(1, 4).map((assignment) =>
        `${assignment.member.title} at ${assignment.anchor?.name ?? "an assigned domain"}${assignment.specialization?.label ? ` | ${assignment.specialization.label}` : ""}`).join(" | ")}${governanceSeatAssignments.length > 4 ? ` | +${governanceSeatAssignments.length - 4} more` : ""}`,
    ));
  }

  if (activeSuccessionCrisis) {
    grid.appendChild(buildDynastyDetailLine("", `Political event | Succession Crisis (${activeSuccessionCrisis.severityLabel})`));
    grid.appendChild(buildDynastyDetailLine(
      `${activeSuccessionCrisis.triggerReasons?.[0] ?? "The bloodline seat remains unstable."} | active ${Math.round(activeSuccessionCrisis.daysActive ?? 0)} in-world days | next escalation ${Math.ceil(activeSuccessionCrisis.nextEscalationInWorldDays ?? 0)} days`,
    ));
    grid.appendChild(buildDynastyDetailLine(
      `Claimants ${activeSuccessionCrisis.claimantCount ?? 0} | yield x${(activeSuccessionCrisis.resourceTrickleMultiplier ?? 1).toFixed(2)} | growth x${(activeSuccessionCrisis.populationGrowthMultiplier ?? 1).toFixed(2)} | stabilize x${(activeSuccessionCrisis.stabilizationMultiplier ?? 1).toFixed(2)} | attack x${(activeSuccessionCrisis.attackMultiplier ?? 1).toFixed(2)}`,
    ));
    const successionActionRow = document.createElement("div");
    successionActionRow.className = "action-grid";
    successionActionRow.appendChild(
      createActionButton({
        label: successionCrisisTerms?.available ? "Consolidate Succession" : "Succession Unsettled",
        detail: successionCrisisTerms?.active
          ? successionCrisisTerms.available
            ? `${formatCostSummary(successionCrisisTerms.cost)} | +${successionCrisisTerms.legitimacyRecovery} legitimacy | +${successionCrisisTerms.loyaltyRecovery} loyalty`
            : successionCrisisTerms.reason ?? "Succession cannot yet be consolidated."
          : "No active succession crisis.",
        warning: true,
        disabled: !successionCrisisTerms?.available,
        onClick: () => {
          const result = consolidateSuccessionCrisis(state, "player");
          if (!result.ok) {
            pushUiMessage(result.reason ?? "Succession could not be consolidated.");
          }
          renderPanels();
        },
      }),
    );
    grid.appendChild(successionActionRow);
  }

  if (activeTerritorialGovernanceRecognition) {
    grid.appendChild(buildDynastyDetailLine(
      "",
      `Late-game governance | Territorial Governance Recognition${activeTerritorialGovernanceRecognition.completed ? " (Victory)" : activeTerritorialGovernanceRecognition.recognized ? " (Held)" : ""}`,
    ));
    grid.appendChild(buildDynastyDetailLine(
      `${activeTerritorialGovernanceRecognition.territorySharePct ?? 0}% / ${activeTerritorialGovernanceRecognition.requiredSharePct ?? 0}% territory | loyal marches ${activeTerritorialGovernanceRecognition.loyalTerritoryCount ?? 0}/${activeTerritorialGovernanceRecognition.territoryCount ?? 0} | integrated marches ${activeTerritorialGovernanceRecognition.victoryLoyalTerritoryCount ?? 0}/${activeTerritorialGovernanceRecognition.territoryCount ?? 0} | weakest ${activeTerritorialGovernanceRecognition.targetControlPointName ?? activeTerritorialGovernanceRecognition.weakestTerritoryName ?? "march"} ${activeTerritorialGovernanceRecognition.weakestTerritoryLoyalty ?? 0} loyalty`,
    ));
    grid.appendChild(buildDynastyDetailLine(
      activeTerritorialGovernanceRecognition.completed
        ? `Victory established after ${Math.round(activeTerritorialGovernanceRecognition.victoryHoldSeconds ?? 0)}s of full sovereignty hold.`
        : activeTerritorialGovernanceRecognition.victoryReady
          ? `${Math.round(activeTerritorialGovernanceRecognition.victoryHoldSeconds ?? 0)}s / ${Math.round(activeTerritorialGovernanceRecognition.requiredVictorySeconds ?? 0)}s sovereignty hold | ${activeTerritorialGovernanceRecognition.statusText ?? "Hold every march in willing integration."}`
          : activeTerritorialGovernanceRecognition.integrationReady
            ? `Integrated marches hold, but population acceptance is still building. ${activeTerritorialGovernanceRecognition.statusText ?? "Keep prosperity and legitimacy rising until acceptance hardens."}`
          : activeTerritorialGovernanceRecognition.recognized
            ? `Recognition established after ${Math.round(activeTerritorialGovernanceRecognition.sustainedSeconds ?? 0)}s. Coalition counterpressure now focuses on the governed frontier.`
            : `${Math.round(activeTerritorialGovernanceRecognition.sustainedSeconds ?? 0)}s / ${Math.round(activeTerritorialGovernanceRecognition.requiredSustainSeconds ?? 0)}s sustained | ${activeTerritorialGovernanceRecognition.statusText ?? "Govern every held march into loyal stability."}`,
    ));
    grid.appendChild(buildDynastyDetailLine(
      `Acceptance ${activeTerritorialGovernanceRecognition.populationAcceptancePct ?? 0}% / ${activeTerritorialGovernanceRecognition.populationAcceptanceThresholdPct ?? 65}% | target ${activeTerritorialGovernanceRecognition.populationAcceptanceTargetPct ?? 0}% | tier ${(activeTerritorialGovernanceRecognition.populationAcceptanceTierLabel ?? "Quiet").toLowerCase()} | trend ${((activeTerritorialGovernanceRecognition.populationAcceptanceTrendPerSecond ?? 0) > 0 ? "+" : "")}${activeTerritorialGovernanceRecognition.populationAcceptanceTrendPerSecond ?? 0}/s${(activeTerritorialGovernanceRecognition.activeMarriageCount ?? 0) > 0 ? ` | marriages ${activeTerritorialGovernanceRecognition.activeMarriageCount}` : ""}${activeTerritorialGovernanceRecognition.marriageEnvoyAvailable ? " | envoy ready" : ""}`,
    ));
    const convictionMod = activeTerritorialGovernanceRecognition.convictionAcceptanceModifier ?? 0;
    const covenantEnd = activeTerritorialGovernanceRecognition.covenantEndorsement ?? 0;
    const tribalFric = activeTerritorialGovernanceRecognition.tribalFriction ?? 0;
    const acceptanceFactorParts = [];
    if (convictionMod !== 0) acceptanceFactorParts.push(`conviction ${convictionMod > 0 ? "+" : ""}${convictionMod}`);
    if (covenantEnd > 0) acceptanceFactorParts.push(`covenant +${covenantEnd}`);
    if (tribalFric > 0) acceptanceFactorParts.push(`tribal friction -${tribalFric}`);
    const truebornEnd = activeTerritorialGovernanceRecognition.truebornEndorsement ?? 0;
    if (truebornEnd !== 0) acceptanceFactorParts.push(`Trueborn ${truebornEnd > 0 ? "+" : ""}${truebornEnd}`);
    if (acceptanceFactorParts.length > 0) {
      grid.appendChild(buildDynastyDetailLine(`World stance: ${acceptanceFactorParts.join(" | ")}`));
    }
    grid.appendChild(buildDynastyDetailLine(
      `Seats ${activeTerritorialGovernanceRecognition.seatCoverageCount ?? 0}/${activeTerritorialGovernanceRecognition.requiredSeatCoverageCount ?? 0}${activeTerritorialGovernanceRecognition.primarySeatName ? ` | primary seat ${activeTerritorialGovernanceRecognition.primarySeatName} ${activeTerritorialGovernanceRecognition.primarySeatGoverned ? "governed" : "unseated"}` : ""} | court ${Math.round(activeTerritorialGovernanceRecognition.courtAverageLoyalty ?? 0)} / ${activeTerritorialGovernanceRecognition.courtLoyaltyThreshold ?? 0} loyalty | incoming holy wars ${activeTerritorialGovernanceRecognition.incomingHolyWarCount ?? 0}${(activeTerritorialGovernanceRecognition.raidedInfrastructureCount ?? 0) > 0 ? ` | raids ${activeTerritorialGovernanceRecognition.raidedInfrastructureCount}` : ""}${(activeTerritorialGovernanceRecognition.fieldWaterCriticalUnits ?? 0) > 0 ? ` | water critical ${activeTerritorialGovernanceRecognition.fieldWaterCriticalUnits}` : ""}${(activeTerritorialGovernanceRecognition.unsuppliedEngineCount ?? 0) > 0 ? ` | unsupplied engines ${activeTerritorialGovernanceRecognition.unsuppliedEngineCount}` : ""}`,
    ));
    if (activeTerritorialGovernanceRecognition.alliancePressureActive) {
      grid.appendChild(buildDynastyDetailLine(
        `Coalition pressure active | ${activeTerritorialGovernanceRecognition.alliancePressureHostileCount ?? 0} hostile courts contesting acceptance | drag ${activeTerritorialGovernanceRecognition.alliancePressureDrag ?? 0} | weakest governed march ${activeTerritorialGovernanceRecognition.weakestTerritoryName ?? "unknown"} (${activeTerritorialGovernanceRecognition.weakestTerritoryLoyalty ?? 0} loyalty)`,
      ));
    }
    if ((activeTerritorialGovernanceRecognition.shortfalls ?? []).length > 0) {
      grid.appendChild(buildDynastyDetailLine(
        `Shortfall: ${(activeTerritorialGovernanceRecognition.shortfalls ?? [])[0]}`,
      ));
    } else if ((activeTerritorialGovernanceRecognition.acceptanceShortfalls ?? []).length > 0) {
      grid.appendChild(buildDynastyDetailLine(
        `Acceptance shortfall: ${(activeTerritorialGovernanceRecognition.acceptanceShortfalls ?? [])[0]}`,
      ));
    } else if ((activeTerritorialGovernanceRecognition.victoryShortfalls ?? []).length > 0) {
      grid.appendChild(buildDynastyDetailLine(
        `Sovereignty shortfall: ${(activeTerritorialGovernanceRecognition.victoryShortfalls ?? [])[0]}`,
      ));
    }
  } else if (lastTerritorialGovernanceOutcome) {
    const governanceOutcomeLabel = lastTerritorialGovernanceOutcome.outcome === "recognized"
      ? "recognized"
      : lastTerritorialGovernanceOutcome.outcome === "broken_after_recognition"
        ? "collapsed after recognition"
        : lastTerritorialGovernanceOutcome.outcome === "completed"
          ? "completed"
        : "failed";
    grid.appendChild(buildDynastyDetailLine("", "Late-game governance"));
    grid.appendChild(buildDynastyDetailLine(
      `Last Territorial Governance outcome: ${governanceOutcomeLabel}${lastTerritorialGovernanceOutcome.reason ? ` | ${lastTerritorialGovernanceOutcome.reason}` : ""}`,
    ));
  }

  // Session 90/92: non-aggression pact diplomacy display with interactive actions.
  {
    const activePacts = (player.diplomacy?.nonAggressionPacts ?? []).filter((pact) => !pact.brokenAt);
    const hostileKingdoms = Object.values(state.factions).filter((candidate) =>
      candidate?.kind === "kingdom" &&
      candidate.id !== "player" &&
      (player.hostileTo ?? []).includes(candidate.id));
    const showDiplomacySection = activePacts.length > 0 || hostileKingdoms.length > 0;
    if (showDiplomacySection) {
      grid.appendChild(buildDynastyDetailLine("", "Diplomacy"));
      activePacts.forEach((pact) => {
        const targetFaction = state.factions[pact.targetFactionId];
        const targetName = targetFaction?.displayName ?? pact.targetFactionId;
        const daysRemaining = Math.max(0, Math.round((pact.minimumExpiresAtInWorldDays ?? 0) - (state.dualClock?.inWorldDays ?? 0)));
        grid.appendChild(buildDynastyDetailLine(
          `Non-aggression pact with ${targetName} | minimum ${daysRemaining > 0 ? `${daysRemaining} days remaining` : "expired, breakable"}`,
        ));
        const breakRow = document.createElement("div");
        breakRow.className = "member-detail";
        breakRow.appendChild(
          createActionButton({
            label: "Break Pact",
            detail: `Restore hostility with ${targetName} | costs 8 legitimacy, 2 conviction`,
            warning: true,
            disabled: false,
            onClick: () => {
              const result = breakNonAggressionPact(state, "player", pact.targetFactionId);
              if (!result.ok) {
                pushUiMessage(result.reason ?? "Pact could not be broken.");
              }
              renderPanels();
            },
          }),
        );
        grid.appendChild(breakRow);
      });
      hostileKingdoms.forEach((hostile) => {
        const terms = getNonAggressionPactTerms(state, "player", hostile.id);
        const proposeRow = document.createElement("div");
        proposeRow.className = "member-detail";
        proposeRow.appendChild(
          createActionButton({
            label: terms.available ? `Propose Pact with ${hostile.displayName ?? hostile.id}` : `Pact Unavailable (${hostile.displayName ?? hostile.id})`,
            detail: terms.available
              ? `${formatCostSummary(terms.cost)} | removes hostility for ${terms.minimumDurationInWorldDays} days`
              : terms.reason ?? "Cannot propose pact.",
            disabled: !terms.available,
            onClick: () => {
              const result = proposeNonAggressionPact(state, "player", hostile.id);
              if (!result.ok) {
                pushUiMessage(result.reason ?? "Pact proposal failed.");
              }
              renderPanels();
            },
          }),
        );
        grid.appendChild(proposeRow);
      });
      // Session 95: Trueborn recognition diplomacy action.
      const recognitionTerms = getTruebornRecognitionTerms(state, "player");
      if ((state.factions.trueborn_city?.riseArc?.stage ?? 0) >= 1) {
        const recognitionRow = document.createElement("div");
        recognitionRow.className = "member-detail";
        recognitionRow.appendChild(
          createActionButton({
            label: recognitionTerms.available
              ? "Recognize Trueborn Claim"
              : player.diplomacy?.truebornRecognition
                ? "Trueborn Claim Recognized"
                : "Recognition Unavailable",
            detail: recognitionTerms.available
              ? `${formatCostSummary(recognitionTerms.cost)} | +${recognitionTerms.standingBonus} standing | -${recognitionTerms.legitimacyCost} legitimacy | Rise pressure reduced 75%`
              : player.diplomacy?.truebornRecognition
                ? "Your dynasty has recognized the Trueborn historical claim. Rise pressure is reduced."
                : recognitionTerms.reason ?? "Cannot recognize the claim.",
            disabled: !recognitionTerms.available,
            onClick: () => {
              const result = recognizeTruebornClaim(state, "player");
              if (!result.ok) {
                pushUiMessage(result.reason ?? "Recognition failed.");
              }
              renderPanels();
            },
          }),
        );
        grid.appendChild(recognitionRow);
      }
    }
  }

  // Session 12: Commander keep-presence sortie UI.
  // Session 10 wired the sortie mechanic into the simulation (issueKeepSortie) and
  // exposed sortieReady/sortieActive/sortieCooldownRemaining on the snapshot. Until now
  // there was no UI button. This closes the legibility-follows-depth gap.
  const realmSnapshotForSortie = getRealmConditionSnapshot(state, "player");
  const fort = realmSnapshotForSortie?.fortification;
  if (fort && fort.primaryKeepName) {
    const primaryKeep = (state.world.settlements ?? []).find(
      (s) => s.factionId === "player" && s.settlementClass === "primary_dynastic_keep",
    );
    if (primaryKeep) {
      let sortieLabel;
      let sortieDetail;
      let sortieDisabled = false;
      let sortieWarning = false;
      if (fort.sortieActive) {
        // Session 41: surface remaining seconds + percentage of the burst window
        // so the player can read the active sortie's timing at a glance.
        const dur = fort.sortieDurationSeconds || 12;
        const remain = Math.max(0, fort.sortieActiveRemaining ?? 0);
        const elapsed = Math.max(0, dur - remain);
        const pct = Math.min(100, Math.max(0, Math.round((elapsed / dur) * 100)));
        sortieLabel = "Sortie Active";
        sortieDetail = `${Math.ceil(remain)}s of ${dur}s remaining at ${fort.primaryKeepName} (${pct}% of burst spent)`;
        sortieDisabled = true;
      } else if (fort.sortieCooldownRemaining > 0) {
        // Session 41: surface percentage recharged so the cooldown reads as a
        // gauge rather than a flat countdown. Combined with the per-frame
        // re-render in performFrame, this gives the player a smooth visual
        // tick-down on the sortie button between calls.
        const cd = fort.sortieCooldownSeconds || 60;
        const remain = Math.max(0, fort.sortieCooldownRemaining);
        const charged = Math.max(0, cd - remain);
        const pct = Math.min(100, Math.max(0, Math.round((charged / cd) * 100)));
        sortieLabel = "Sortie On Cooldown";
        sortieDetail = `${Math.ceil(remain)}s until ready • ${pct}% recharged`;
        sortieDisabled = true;
      } else if (!fort.commanderAtKeep) {
        sortieLabel = "Sortie Requires Commander";
        sortieDetail = "Commander must be present at the keep";
        sortieDisabled = true;
      } else if (!fort.threatActive) {
        sortieLabel = "Sortie (No Threat)";
        sortieDetail = "No enemy combatants at the keep yet";
        sortieDisabled = true;
      } else if (fort.sortieReady) {
        sortieLabel = "Call Sortie";
        sortieDetail = `12s attack +22% & sight +22 burst from ${fort.primaryKeepName}`;
      } else {
        sortieLabel = "Sortie Unavailable";
        sortieDetail = "Conditions not met";
        sortieDisabled = true;
        sortieWarning = true;
      }
      const sortieActionRow = document.createElement("div");
      sortieActionRow.className = "action-grid";
      sortieActionRow.appendChild(
        createActionButton({
          label: sortieLabel,
          detail: sortieDetail,
          disabled: sortieDisabled,
          warning: sortieWarning,
          onClick: () => {
            const result = issueKeepSortie(state, "player", primaryKeep.id);
            if (!result.ok) {
              pushUiMessage(result.reason ?? "Sortie could not be called.");
            }
            renderPanels();
          },
        }),
      );
      grid.appendChild(sortieActionRow);
    }
  }

  if (dynastyOperations.length > 0) {
    grid.appendChild(buildDynastyDetailLine("", `Active dynasty operations (${dynastyOperations.length})`));
    dynastyOperations.slice(0, 3).forEach((operation) => {
      grid.appendChild(buildDynastyDetailLine(buildDynastyOperationSummaryText(operation)));
    });
  }

  if (intelligenceReports.length > 0) {
    grid.appendChild(buildDynastyDetailLine("", `Intelligence reports (${intelligenceReports.length})`));
    intelligenceReports.slice(0, 3).forEach((report) => {
      const reportSummary = [
        `${report.reportLabel ?? "Court report"} on ${report.targetFactionName}`,
        `legitimacy ${report.targetLegitimacy}`,
        `captives ${report.targetCaptiveCount}`,
        `lesser houses ${report.targetLesserHouseCount}`,
      ];
      if ((report.sourceType ?? "espionage") === "counter_intelligence") {
        if (report.interceptType) {
          reportSummary.push(`intercepted ${report.interceptType}`);
        }
        if ((report.interceptCount ?? 0) > 0) {
          reportSummary.push(`network hits ${report.interceptCount}`);
        }
        const dossierSabotage = getDossierBackedSabotageProfile(state, "player", report.targetFactionId, report);
        if (dossierSabotage.available) {
          reportSummary.push(`retaliate ${dossierSabotage.subtype.replace(/_/g, " ")} -> ${dossierSabotage.targetBuildingName}`);
          reportSummary.push(`dossier +${dossierSabotage.intelligenceSupportBonus}`);
        }
      }
      reportSummary.push(`${Math.ceil(report.remainingSeconds ?? 0)}s left`);
      grid.appendChild(buildDynastyDetailLine(
        reportSummary.join(" | "),
      ));
      report.members.slice(0, 3).forEach((member) => {
        grid.appendChild(buildDynastyDetailLine(
          `${member.title} | ${member.role?.name ?? member.roleId} | ${member.locationLabel}`,
        ));
      });
    });
  }

  const counterIntelTerms = getCounterIntelligenceTerms(state, "player");
  if (counterIntelligenceWatches.length > 0) {
    grid.appendChild(buildDynastyDetailLine("", `Counter-intelligence watch (${counterIntelligenceWatches.length})`));
    counterIntelligenceWatches.slice(0, 2).forEach((entry) => {
      const guardedRoles = (entry.guardedRoles ?? []).slice(0, 3).join(", ");
      grid.appendChild(buildDynastyDetailLine(
        `${entry.operator?.title ?? entry.operatorTitle ?? "Court watcher"} | strength ${entry.watchStrength ?? 0} | ${Math.ceil(entry.remainingSeconds ?? 0)}s left | foiled ${entry.interceptions ?? 0}`,
      ));
      grid.appendChild(buildDynastyDetailLine(
        `Guarding ${guardedRoles || "core bloodline roles"} | ward ${entry.wardLabel ?? "none"}${entry.lastInterceptType ? ` | last ${entry.lastInterceptType} from ${entry.lastSourceFactionName ?? "unknown"}` : ""}`,
      ));
    });
  } else {
    grid.appendChild(buildDynastyDetailLine("Counter-intelligence idle, the bloodline court is not under an active defensive watch."));
  }

  const counterIntelActions = document.createElement("div");
  counterIntelActions.className = "action-grid";
  counterIntelActions.appendChild(
    createActionButton({
      label: "Raise Counter-Intelligence",
      detail: counterIntelTerms.available
        ? `${formatCostSummary(counterIntelTerms.cost)} | ${formatOperationDuration(counterIntelTerms.duration)} to raise | ${counterIntelTerms.watchDuration}s watch | strength ${counterIntelTerms.watchStrength}`
        : counterIntelTerms.reason ?? "Counter-intelligence unavailable",
      warning: counterIntelTerms.available && (counterIntelTerms.watchStrength ?? 0) < 18,
      disabled: !counterIntelTerms.available,
      onClick: () => {
        const result = startCounterIntelligenceOperation(state, "player");
        if (!result.ok) {
          pushUiMessage(result.reason ?? "Counter-intelligence could not be raised.");
        }
        renderPanels();
      },
    }),
  );
  grid.appendChild(counterIntelActions);

    if (rivalDynasties.length > 0) {
      grid.appendChild(buildDynastyDetailLine("", `Rival courts (${rivalDynasties.length})`));
      rivalDynasties.slice(0, 3).forEach((rival) => {
        const rivalReport = intelligenceReports.find((report) => report.targetFactionId === rival.id) ?? null;
        const dossierSabotage = rivalReport && (rivalReport.sourceType ?? "espionage") === "counter_intelligence"
          ? getDossierBackedSabotageProfile(state, "player", rival.id, rivalReport)
          : { available: false };
        const dossierSabotageTerms = dossierSabotage.available
          ? getSabotageOperationTerms(state, "player", dossierSabotage.subtype, rival.id, dossierSabotage.targetBuildingId, {
              intelligenceReportId: dossierSabotage.intelligenceReportId,
              intelligenceSupportBonus: dossierSabotage.intelligenceSupportBonus,
              retaliationReason: dossierSabotage.retaliationReason,
              retaliationInterceptType: dossierSabotage.interceptType ?? null,
            })
          : { available: false, reason: dossierSabotage.reason ?? "No dossier-backed sabotage line." };
        const activeMembers = (rival.dynasty.members ?? []).filter((member) =>
          member.status === "active" || member.status === "ruling").length;
        const rivalSuccessionCrisis = rival.dynasty.activeSuccessionCrisis ?? null;
        const rivalGovernanceRecognition = rival.dynasty.activeTerritorialGovernanceRecognition ?? null;
        grid.appendChild(buildDynastyDetailLine(
          `${rival.displayName} | legitimacy ${Math.round(rival.dynasty.legitimacy ?? 0)} | active bloodline ${activeMembers}${rivalSuccessionCrisis ? ` | succession ${rivalSuccessionCrisis.severityLabel.toLowerCase()}` : ""}${rivalGovernanceRecognition ? ` | governance ${rivalGovernanceRecognition.recognized ? "held" : `${Math.ceil(rivalGovernanceRecognition.remainingSeconds ?? 0)}s`}` : ""}`,
        ));
        if (rivalSuccessionCrisis) {
          grid.appendChild(buildDynastyDetailLine(
            `${rivalSuccessionCrisis.triggerReasons?.[0] ?? "The rival court remains unstable."} | claimants ${rivalSuccessionCrisis.claimantCount ?? 0} | active ${Math.round(rivalSuccessionCrisis.daysActive ?? 0)} days`,
          ));
        }
        if (rivalGovernanceRecognition) {
          grid.appendChild(buildDynastyDetailLine(
            `${rivalGovernanceRecognition.territorySharePct ?? 0}% / ${rivalGovernanceRecognition.requiredSharePct ?? 0}% territory | weakest ${rivalGovernanceRecognition.targetControlPointName ?? rivalGovernanceRecognition.weakestTerritoryName ?? "march"} ${rivalGovernanceRecognition.weakestTerritoryLoyalty ?? 0} loyalty | acceptance ${rivalGovernanceRecognition.populationAcceptancePct ?? 0}%/${rivalGovernanceRecognition.populationAcceptanceThresholdPct ?? 65}%`,
          ));
        }

      const espionage = getEspionageTerms(state, "player", rival.id);
      const covertActions = document.createElement("div");
      covertActions.className = "action-grid";
      covertActions.appendChild(
        createActionButton({
          label: "Infiltrate Court",
          detail: espionage.available
            ? `${formatCostSummary(espionage.cost)} | ${formatOperationDuration(espionage.duration)} | ${Math.round((espionage.projectedChance ?? 0) * 100)}%${espionage.counterIntelligenceActive ? ` | watch +${espionage.counterIntelligenceDefense ?? 0}` : ""}`
            : espionage.reason ?? "Espionage closed",
          warning: (espionage.projectedChance ?? 0) < 0.5,
          disabled: !espionage.available,
          onClick: () => {
            const result = startEspionageOperation(state, "player", rival.id);
            if (!result.ok) {
              pushUiMessage(result.reason ?? "Espionage failed to launch.");
            }
            renderPanels();
            },
          }),
        );
        if (dossierSabotage.available || rivalReport) {
          covertActions.appendChild(
            createActionButton({
              label: "Launch Dossier Sabotage",
              detail: dossierSabotageTerms.available
                ? `${formatCostSummary(dossierSabotageTerms.cost)} | ${formatOperationDuration(dossierSabotageTerms.duration)} | ${Math.round((dossierSabotageTerms.projectedChance ?? 0) * 100)}% | ${dossierSabotage.subtype.replace(/_/g, " ")} -> ${dossierSabotage.targetBuildingName} | dossier +${dossierSabotage.intelligenceSupportBonus}`
                : dossierSabotageTerms.reason ?? "Dossier sabotage closed",
              warning: (dossierSabotageTerms.projectedChance ?? 0) < 0.55,
              disabled: !dossierSabotageTerms.available,
              onClick: () => {
                const result = startSabotageOperation(state, "player", dossierSabotage.subtype, rival.id, dossierSabotage.targetBuildingId, {
                  intelligenceReportId: dossierSabotage.intelligenceReportId,
                  intelligenceSupportBonus: dossierSabotage.intelligenceSupportBonus,
                  retaliationReason: dossierSabotage.retaliationReason,
                  retaliationInterceptType: dossierSabotage.interceptType ?? null,
                });
                if (!result.ok) {
                  pushUiMessage(result.reason ?? "Dossier sabotage failed to launch.");
                }
                renderPanels();
              },
            }),
          );
        }
        grid.appendChild(covertActions);

      getDynastyCovertTargets(rival, rivalReport).slice(0, 3).forEach((target) => {
        const targetMemberId = target.memberId ?? target.id;
        const assassination = getAssassinationTerms(state, "player", rival.id, targetMemberId);
        const targetLocation = target.locationLabel ?? assassination.locationLabel ?? "inside the rival court";
        grid.appendChild(buildDynastyDetailLine(
          `${target.title} | ${target.role?.name ?? target.roleId} | ${targetLocation}${assassination.intelSupport ? " | intel-backed" : ""}${assassination.counterIntelligenceActive ? " | under watch" : ""}`,
        ));
        const assassinationRow = document.createElement("div");
        assassinationRow.className = "action-grid";
        assassinationRow.appendChild(
          createActionButton({
            label: "Assassinate",
            detail: assassination.available
              ? `${formatCostSummary(assassination.cost)} | ${formatOperationDuration(assassination.duration)} | ${Math.round((assassination.projectedChance ?? 0) * 100)}%${assassination.counterIntelligenceActive ? ` | watch +${assassination.counterIntelligenceDefense ?? 0}` : ""}${(assassination.bloodlineGuardBonus ?? 0) > 0 ? ` | bloodline shield +${assassination.bloodlineGuardBonus}` : ""}`
              : assassination.reason ?? "Assassination closed",
            warning: (assassination.projectedChance ?? 0) < 0.55,
            disabled: !assassination.available,
            onClick: () => {
              const result = startAssassinationOperation(state, "player", rival.id, targetMemberId);
              if (!result.ok) {
                pushUiMessage(result.reason ?? "Assassination failed to launch.");
              }
              renderPanels();
            },
          }),
        );
        grid.appendChild(assassinationRow);
      });
    });
  }

  const captives = player.dynasty.captives ?? [];
  if (captives.length > 0) {
    grid.appendChild(buildDynastyDetailLine("", `Captives held (${captives.length})`));
    captives.slice(0, 3).forEach((captive) => {
      const ransomDemand = captive.ransomDemand;
      const captiveText = `${captive.title} of ${captive.sourceFactionName} • ${captive.role?.name ?? captive.roleId} • renown ${captive.renown}`;
      grid.appendChild(buildDynastyDetailLine(captiveText));
      if (ransomDemand) {
        const captiveActions = document.createElement("div");
        captiveActions.className = "action-grid";
        captiveActions.appendChild(
          createActionButton({
            label: "Demand Ransom",
            detail: ransomDemand.available
              ? `${formatCostSummary(ransomDemand.cost)} â€¢ immediate release`
              : ransomDemand.reason ?? "Terms not yet payable",
            disabled: !ransomDemand.available,
            onClick: () => {
              const result = demandCaptiveRansom(state, "player", captive.id);
              if (!result.ok) {
                pushUiMessage(result.reason ?? "Ransom demand failed.");
              }
              renderPanels();
            },
          }),
        );
        grid.appendChild(captiveActions);
      }
    });
  }

  if (capturedMembers.length > 0) {
    grid.appendChild(buildDynastyDetailLine("", `Captured bloodline (${capturedMembers.length})`));
    capturedMembers.slice(0, 3).forEach((member) => {
      const ransom = member.recoveryOptions?.ransom;
      const rescue = member.recoveryOptions?.rescue;
      grid.appendChild(buildDynastyDetailLine(`${member.title} â€¢ held by ${member.capturedByName ?? "unknown enemy"} â€¢ ${member.role?.name ?? member.roleId}`));

      if (member.activeOperation) {
        grid.appendChild(buildDynastyDetailLine(buildDynastyOperationSummaryText(member.activeOperation)));
        return;
      }

      const rescueActions = document.createElement("div");
      rescueActions.className = "action-grid";
      rescueActions.appendChild(
        createActionButton({
          label: "Negotiate Release",
          detail: ransom?.available
            ? `${formatCostSummary(ransom.cost)} â€¢ ${formatOperationDuration(ransom.duration)}`
            : ransom?.reason ?? "Ransom closed",
          disabled: !ransom?.available,
          onClick: () => {
            const result = startRansomNegotiation(state, "player", member.id);
            if (!result.ok) {
              pushUiMessage(result.reason ?? "Ransom negotiation failed.");
            }
            renderPanels();
          },
        }),
      );
      rescueActions.appendChild(
        createActionButton({
          label: "Send Rescue Cell",
          detail: rescue?.available
            ? `${formatCostSummary(rescue.cost)} â€¢ ${formatOperationDuration(rescue.duration)} â€¢ ${Math.round((rescue.projectedChance ?? 0) * 100)}%`
            : rescue?.reason ?? "Rescue impossible",
          warning: (rescue?.projectedChance ?? 0) < 0.5,
          disabled: !rescue?.available,
          onClick: () => {
            const result = startRescueOperation(state, "player", member.id);
            if (!result.ok) {
              pushUiMessage(result.reason ?? "Rescue operation failed.");
            }
            renderPanels();
          },
        }),
      );
      grid.appendChild(rescueActions);
    });
  }

  const fallen = player.dynasty.attachments.fallenMembers ?? [];
  if (fallen.length > 0) {
    grid.appendChild(buildDynastyDetailLine("", `Fallen & captured (${fallen.length})`));
    fallen.slice(0, 3).forEach((entry) => {
      const disposition = entry.disposition === "captured"
        ? `captured by ${state.factions[entry.capturedByFactionId]?.displayName ?? "enemy"}`
        : entry.disposition === "displaced"
          ? "fled the frontier"
          : "slain in battle";
      grid.appendChild(buildDynastyDetailLine(`${entry.title} • ${disposition}`));
    });
  }

  // Session 34: Marriage UI panel. Surfaces active marriages (legitimacy + diplomatic
  // detente results), incoming proposals (player can accept), and a propose action
  // for each living player member who is eligible. Canonical legibility-follows-depth
  // closure for Session 33's marriage system.
  const playerMarriages = player.dynasty.marriages ?? [];
  const activePlayerMarriages = playerMarriages.filter((marriage) => !marriage.dissolvedAt);
  const dissolvedPlayerMarriages = playerMarriages
    .filter((marriage) => marriage.dissolvedAt && marriage.dissolutionReason === "death")
    .sort((a, b) => (b.dissolvedAtInWorldDays ?? 0) - (a.dissolvedAtInWorldDays ?? 0));
  const playerInbox = (player.dynasty.marriageProposalsIn ?? []).filter((p) => p.status === "pending");
  const playerOutbox = (player.dynasty.marriageProposalsOut ?? []).filter((p) => p.status === "pending");
  const playerMarriageGovernance = getMarriageGovernanceStatus(state, "player");

  if (playerMarriageGovernance) {
    grid.appendChild(buildDynastyDetailLine("", "Marriage governance"));
    grid.appendChild(buildDynastyDetailLine(`Household authority: ${formatMarriageAuthoritySummary(playerMarriageGovernance.authority)}`));
    grid.appendChild(buildDynastyDetailLine(`Offering envoy: ${formatMarriageEnvoySummary(playerMarriageGovernance.envoy)}`));
  }

  if (activePlayerMarriages.length > 0) {
    grid.appendChild(buildDynastyDetailLine("", `Active marriages (${activePlayerMarriages.length})`));
    activePlayerMarriages.slice(0, 4).forEach((m) => {
      const headMember = player.dynasty.members.find((mem) => mem.id === m.headMemberId);
      const spouseHouse = state.content.byId.houses[m.spouseHouseId]?.name ?? "an allied house";
      const childCount = m.children?.length ?? 0;
      const status = m.childGenerated
        ? childCount === 1 ? "1 child" : `${childCount} children`
        : "awaiting child";
      const text = `${headMember?.title ?? "Bloodline member"} wed to a ${spouseHouse} bloodline • ${status}`;
      grid.appendChild(buildDynastyDetailLine(text));
      if (m.governance?.sourceAuthority) {
        grid.appendChild(buildDynastyDetailLine(`Arranged by: ${formatMarriageAuthoritySummary(m.governance.sourceAuthority)}`));
      }
      if (m.governance?.acceptedByAuthority) {
        grid.appendChild(buildDynastyDetailLine(`Accepted by: ${formatMarriageAuthoritySummary(m.governance.acceptedByAuthority)}`));
      }
    });
  }

  if (dissolvedPlayerMarriages.length > 0) {
    grid.appendChild(buildDynastyDetailLine("", `Marriages ended by death (${dissolvedPlayerMarriages.length})`));
    dissolvedPlayerMarriages.slice(0, 3).forEach((marriage) => {
      const text = describeMarriageDissolution(state, player, marriage);
      if (text) {
        grid.appendChild(buildDynastyDetailLine(text));
      }
    });
  }

  if (playerInbox.length > 0) {
    grid.appendChild(buildDynastyDetailLine("", `Marriage proposals received (${playerInbox.length})`));
    playerInbox.slice(0, 3).forEach((proposal) => {
      const sourceFactionName = state.factions[proposal.sourceFactionId]?.displayName ?? proposal.sourceFactionId;
      const sourceMember = state.factions[proposal.sourceFactionId]?.dynasty?.members?.find((m) => m.id === proposal.sourceMemberId);
      const targetMember = player.dynasty.members.find((m) => m.id === proposal.targetMemberId);
      const compatibility = getMarriageFaithCompatibilityProfile(state, proposal.sourceFactionId, proposal.targetFactionId);
      const acceptanceTerms = getMarriageAcceptanceTerms(state, proposal.id);
      grid.appendChild(buildDynastyDetailLine(
        `${sourceMember?.title ?? "Foreign noble"} of ${sourceFactionName} proposes to ${targetMember?.title ?? "your bloodline member"}`,
      ));
      grid.appendChild(buildDynastyDetailLine(describeMarriageFaithCompatibility(compatibility)));
      if (proposal.governance?.sourceAuthority) {
        grid.appendChild(buildDynastyDetailLine(`Offering household: ${formatMarriageAuthoritySummary(proposal.governance.sourceAuthority)}`));
      }
      if (proposal.governance?.sourceEnvoy) {
        grid.appendChild(buildDynastyDetailLine(`Offering envoy: ${formatMarriageEnvoySummary(proposal.governance.sourceEnvoy)}`));
      }
      if (acceptanceTerms?.targetAuthority) {
        grid.appendChild(buildDynastyDetailLine(`Your approving authority: ${formatMarriageAuthoritySummary(acceptanceTerms.targetAuthority)}`));
      }
      const proposalActions = document.createElement("div");
      proposalActions.className = "action-grid";
      proposalActions.appendChild(
        createActionButton({
          label: "Accept Marriage",
          detail: `Detente + oathkeeping + dynastic legitimacy • ${formatMarriageAcceptanceTerms(acceptanceTerms, compatibility)}`,
          disabled: !acceptanceTerms?.available,
          onClick: () => {
            const result = acceptMarriage(state, proposal.id);
            if (!result.ok) {
              pushUiMessage(result.reason ?? "Marriage acceptance failed.");
            }
            renderPanels();
          },
        }),
      );
      // Session 39: Decline action surfaces the explicit decline path rather
      // than relying on natural expiration. Lifts the proposal off the inbox
      // immediately and frees both members.
      proposalActions.appendChild(
        createActionButton({
          label: "Decline",
          detail: "Frees both members; offer lapses now",
          onClick: () => {
            const result = declineMarriage(state, proposal.id);
            if (!result.ok) {
              pushUiMessage(result.reason ?? "Marriage decline failed.");
            }
            renderPanels();
          },
        }),
      );
      grid.appendChild(proposalActions);
    });
  }

  if (playerOutbox.length > 0) {
    grid.appendChild(buildDynastyDetailLine("", `Marriage offers sent (${playerOutbox.length})`));
    playerOutbox.slice(0, 3).forEach((proposal) => {
      const targetFactionName = state.factions[proposal.targetFactionId]?.displayName ?? proposal.targetFactionId;
      const sourceMember = player.dynasty.members.find((m) => m.id === proposal.sourceMemberId);
      const targetMember = state.factions[proposal.targetFactionId]?.dynasty?.members?.find((m) => m.id === proposal.targetMemberId);
      const compatibility = getMarriageFaithCompatibilityProfile(state, proposal.sourceFactionId, proposal.targetFactionId);
      const aiEvaluationText = describeAiMarriageEvaluation(proposal.aiEvaluation);
      grid.appendChild(buildDynastyDetailLine(
        `${sourceMember?.title ?? "Your noble"} awaits ${targetFactionName}'s answer regarding ${targetMember?.title ?? "their bloodline member"}.`,
      ));
      grid.appendChild(buildDynastyDetailLine(describeMarriageFaithCompatibility(compatibility)));
      if (proposal.governance?.sourceAuthority) {
        grid.appendChild(buildDynastyDetailLine(`Sanctioned by: ${formatMarriageAuthoritySummary(proposal.governance.sourceAuthority)}`));
      }
      if (proposal.governance?.sourceEnvoy) {
        grid.appendChild(buildDynastyDetailLine(`Carried by: ${formatMarriageEnvoySummary(proposal.governance.sourceEnvoy)}`));
      }
      if (proposal.governance?.targetAuthorityAvailable === false) {
        grid.appendChild(buildDynastyDetailLine(`Target approval blocked: ${proposal.governance?.targetAuthorityReason ?? "Target household cannot currently approve"}`));
      } else if (proposal.governance?.targetAuthorityPreview) {
        grid.appendChild(buildDynastyDetailLine(`Target household: ${formatMarriageAuthoritySummary(proposal.governance.targetAuthorityPreview)}`));
      }
      if (aiEvaluationText) {
        grid.appendChild(buildDynastyDetailLine(aiEvaluationText));
      }
    });
  }

  // Propose-marriage actions: player picks one eligible bloodline member, then
  // selects a hostile or neutral faction's available member. Surface a single
  // "Propose Marriage" button per eligible player member that targets the enemy
  // faction's first available member as the canonical first-layer convenience.
  // Future sessions can add a richer chooser. For now this is real diplomatic
  // action, not a placeholder: it actually triggers a proposal.
  const eligiblePlayerMembers = livingMembers.filter((m) => m.status === "active" || m.status === "ruling");
  const enemyFaction = state.factions.enemy;
  const enemyAvailable = enemyFaction?.dynasty?.members?.find((m) => m.status === "active" || m.status === "ruling");
  if (eligiblePlayerMembers.length > 0 && enemyAvailable && playerInbox.length === 0) {
    const proposeRow = document.createElement("div");
    proposeRow.className = "action-grid";
    const proposingMember = eligiblePlayerMembers.find((member) => member.roleId !== "head_of_bloodline") ?? eligiblePlayerMembers[0];
    const proposeCompatibility = getMarriageFaithCompatibilityProfile(state, "player", "enemy");
    const marriageTerms = getMarriageProposalTerms(state, "player", proposingMember.id, "enemy", enemyAvailable.id);
    proposeRow.appendChild(
      createActionButton({
        label: `Propose Marriage`,
        detail: `${proposingMember.title} \u2192 ${enemyAvailable.title} of ${enemyFaction.displayName} • ${formatMarriageProposalTerms(marriageTerms, proposeCompatibility)}`,
        disabled: !marriageTerms?.available,
        onClick: () => {
          const result = proposeMarriage(state, "player", proposingMember.id, "enemy", enemyAvailable.id);
          if (!result.ok) {
            pushUiMessage(result.reason ?? "Marriage proposal failed.");
          }
          renderPanels();
        },
      }),
    );
    grid.appendChild(proposeRow);
    grid.appendChild(buildDynastyDetailLine(describeMarriageFaithCompatibility(proposeCompatibility)));
    if (marriageTerms?.sourceAuthority) {
      grid.appendChild(buildDynastyDetailLine(`Offering household: ${formatMarriageAuthoritySummary(marriageTerms.sourceAuthority)}`));
    }
    if (marriageTerms?.sourceEnvoy) {
      grid.appendChild(buildDynastyDetailLine(`Offering envoy: ${formatMarriageEnvoySummary(marriageTerms.sourceEnvoy)}`));
    }
    if (marriageTerms?.targetAuthorityAvailable === false) {
      grid.appendChild(buildDynastyDetailLine(`Target approval blocked: ${marriageTerms.targetAuthorityReason ?? "Target household cannot currently approve"}`));
    }
  }

  // Session 35: Lesser houses promotion pipeline UI.
  // Surfaces (a) the active lesser-house roster (legitimacy + cadet branch
  // legibility) and (b) per-candidate promotion buttons. Promotion is gated by
  // simulation-side checks; the button calls promoteMemberToLesserHouse and
  // re-renders. Canonical preservation: every action wires a real simulation
  // function — no decorative buttons.
  const playerLesserHouses = player.dynasty.lesserHouses ?? [];
  const playerCandidates = player.dynasty.lesserHouseCandidates ?? [];

  if (playerLesserHouses.length > 0) {
    grid.appendChild(buildDynastyDetailLine("", `Lesser houses (${playerLesserHouses.length})`));
    playerLesserHouses.slice(0, 4).forEach((lh) => {
      // Session 44: surface defection status explicitly. A defected branch is no
      // longer a cadet — it is a hostile minor house on the world register.
      let text;
      const mixedHouseName = lh.mixedBloodlineHouseId
        ? state.content.byId.houses[lh.mixedBloodlineHouseId]?.name ?? lh.mixedBloodlineHouseId
        : null;
      const mixedPressureText = mixedHouseName
        ? ` | mixed with ${mixedHouseName} | mixed drift ${Math.round((lh.mixedBloodlinePressure ?? 0) * 100) / 100}/day`
        : "";
      const maritalAnchorHouseName = lh.maritalAnchorHouseId
        ? state.content.byId.houses[lh.maritalAnchorHouseId]?.name ?? lh.maritalAnchorHouseId
        : null;
      const maritalAnchorText = maritalAnchorHouseName
        ? ` | marriage anchor ${maritalAnchorHouseName} ${lh.maritalAnchorStatus ?? "none"} | anchor drift ${Math.round((lh.maritalAnchorPressure ?? 0) * 100) / 100}/day${(lh.maritalAnchorChildCount ?? 0) > 0 ? ` | branch children ${lh.maritalAnchorChildCount}` : ""}`
        : "";
      const worldPressureText = (lh.worldPressureLevel ?? 0) > 0 && (lh.worldPressurePressure ?? 0) < 0
        ? ` | world pressure ${lh.worldPressureStatus ?? "quiet"} | world drift ${Math.round((lh.worldPressurePressure ?? 0) * 100) / 100}/day`
        : "";
      if (lh.status === "defected") {
        text = `${lh.name} \u2014 DEFECTED \u2014 now hostile minor house (was founded by ${lh.founderTitle})`;
      } else {
        text = `${lh.name} • loyalty ${Math.round(lh.loyalty)} • founded by ${lh.founderTitle}${mixedPressureText}${maritalAnchorText}${worldPressureText}`;
      }
      grid.appendChild(buildDynastyDetailLine(text));
    });
  }

  // Session 44: surface defected-as-minor-faction entries in the world register
  // so the player sees that rival minor houses exist. Iterates state.factions
  // for entries of kind "minor_house" that originated from the player's dynasty.
  const playerBreakaways = Object.values(state.factions ?? {}).filter(
    (f) => f && f.kind === "minor_house" && f.originFactionId === "player",
  );
  if (playerBreakaways.length > 0) {
    grid.appendChild(buildDynastyDetailLine("", `Rival minor houses (${playerBreakaways.length})`));
    playerBreakaways.slice(0, 4).forEach((minor) => {
      const head = minor.dynasty?.members?.[0];
      const headText = head ? `led by ${head.title}` : "";
      const territoryCount = state.world.controlPoints.filter((cp) => cp.ownerFactionId === minor.id).length;
      const retinueCount = state.units.filter((unit) =>
        unit.factionId === minor.id &&
        unit.health > 0 &&
        state.content.byId.units[unit.typeId]?.role !== "worker").length;
      const defenseState = minor.ai?.localDefenseStatus ?? "forming";
      let defenseText = defenseState === "defending"
        ? "on alert"
        : defenseState === "retaking"
          ? "retaking march"
          : defenseState === "holding"
            ? "holding march"
            : defenseState;
      const levyProgress = minor.ai?.levySecondsRequired
        ? Math.min(100, Math.round(((minor.ai?.levyProgress ?? 0) / minor.ai.levySecondsRequired) * 100))
        : 0;
      const levyStatus = minor.ai?.levyStatus === "awaiting_food"
        ? "awaiting food"
        : minor.ai?.levyStatus === "awaiting_influence"
          ? "awaiting influence"
          : minor.ai?.levyStatus === "mustered"
            ? "retinue at local cap"
            : minor.ai?.levyStatus === "levying"
              ? `levy ${levyProgress}%`
              : minor.ai?.levyStatus === "raised"
                ? `raised ${(minor.ai?.lastLevyUnitId ?? "retinue").replace(/_/g, " ")}`
                : minor.ai?.levyStatus === "unsettled"
                  ? "levy stalled by low loyalty"
                  : minor.ai?.levyStatus === "contested"
                    ? "levy halted by contest"
                    : minor.ai?.levyStatus === "dispossessed"
                      ? "march lost"
                      : minor.ai?.levyStatus ?? "forming";
      defenseText = `${defenseText}, ${levyStatus}`;
      if ((minor.ai?.parentPressureLevel ?? 0) > 0) {
        defenseText += `, parent pressure ${minor.ai?.parentPressureStatus ?? "quiet"}, levy x${minor.ai?.parentPressureLevyTempo ?? 1}, retake x${minor.ai?.parentPressureRetakeTempo ?? 1}`;
        if ((minor.ai?.parentPressureRetinueBonus ?? 0) > 0) {
          defenseText += `, cap +${minor.ai.parentPressureRetinueBonus}`;
        }
      }
      grid.appendChild(buildDynastyDetailLine(`Retinue ${retinueCount} â€¢ ${defenseText}`));
      grid.appendChild(buildDynastyDetailLine(
        `${minor.displayName} \u2014 hostile minor house ${headText}${territoryCount > 0 ? ` • ${territoryCount} march${territoryCount === 1 ? "" : "es"}` : ""}`,
      ));
    });
  }

  if (playerCandidates.length > 0) {
    grid.appendChild(buildDynastyDetailLine("", `Lesser-house candidates (${playerCandidates.length})`));
    playerCandidates.slice(0, 3).forEach((memberId) => {
      const member = player.dynasty.members.find((m) => m.id === memberId);
      if (!member) return;
      grid.appendChild(buildDynastyDetailLine(
        `${member.title} \u2014 renown ${Math.round(member.renown ?? 0)} \u2014 ready for cadet branch`,
      ));
      const promoteRow = document.createElement("div");
      promoteRow.className = "action-grid";
      promoteRow.appendChild(
        createActionButton({
          label: "Promote to Lesser House",
          detail: "+legitimacy, +stewardship conviction, founds cadet branch",
          onClick: () => {
            const result = promoteMemberToLesserHouse(state, "player", member.id);
            if (!result.ok) {
              pushUiMessage(result.reason ?? "Lesser-house promotion failed.");
            }
            renderPanels();
          },
        }),
      );
      grid.appendChild(promoteRow);
    });
  }

  livingMembers.slice(0, 6).forEach((member) => {
    grid.appendChild(buildDynastyMemberCard(member));
  });

  refs.dynastyPanel.appendChild(grid);
}

function renderFaithPanel() {
  const player = getFactionSnapshot(state, "player");
  const realmSnapshot = getRealmConditionSnapshot(state, "player");
  const actionGrid = document.createElement("div");
  actionGrid.className = "action-grid";
  const rivalCourts = Object.values(state.factions)
    .filter((faction) => faction.id !== "player" && faction.kind !== "tribes" && faction.dynasty)
    .map((faction) => getFactionSnapshot(state, faction.id));
  const activeFaithOperations = (player.dynasty?.operations?.active ?? [])
    .filter((operation) => operation.type === "missionary" || operation.type === "holy_war");
  const activeHolyWars = player.faith.activeHolyWars ?? [];
  const incomingHolyWars = player.faith.incomingHolyWars ?? [];
  const activeCovenantTest = player.faith.activeCovenantTest ?? null;
  const incomingCovenantTests = player.faith.incomingCovenantTests ?? [];
  const lastCovenantTestOutcome = player.faith.lastCovenantTestOutcome ?? null;
  const activeDivineRightDeclaration = player.faith.activeDivineRightDeclaration ?? null;
  const incomingDivineRightDeclarations = player.faith.incomingDivineRightDeclarations ?? [];
  const lastDivineRightOutcome = player.faith.lastDivineRightOutcome ?? null;
  const divineRightTerms = player.faith.selectedFaithId
    ? getDivineRightDeclarationTerms(state, "player")
    : null;

  const postFaithFailure = (reason) => {
    state.messages.unshift({
      id: `faith-fail-${Date.now()}`,
      text: reason,
      tone: "warn",
      ttl: 4,
    });
  };

  const exposures = player.faith.exposures
    .map((faith) => `
      <div class="faith-track">
        <div class="faith-title">
          <strong>${faith.name}</strong>
          <span class="selection-meta">${Math.floor(faith.exposure)} / 100</span>
        </div>
        <div class="faith-detail">${faith.covenantName}</div>
        <div class="faith-detail">${describeDoctrineEffects(faith.prototypeEffects?.light)}</div>
        <div class="progress-bar"><div class="progress-fill" style="width: ${Math.min(100, faith.exposure)}%"></div></div>
      </div>
    `)
    .join("");

  const operationMarkup = activeFaithOperations.length > 0
    ? activeFaithOperations
      .map((operation) => `<div class="faith-detail">${buildDynastyOperationSummaryText(operation)}</div>`)
      .join("")
    : '<div class="faith-detail">No faith operations are currently in motion.</div>';

  const holyWarMarkup = activeHolyWars.length > 0
    ? activeHolyWars
      .map((entry) => `<div class="faith-detail">Holy war against ${entry.targetFactionName}, ${entry.faithName}, ${Math.round(entry.remainingSeconds)}s remaining.</div>`)
      .join("")
    : '<div class="faith-detail">No outbound holy war is currently active.</div>';

  const incomingHolyWarMarkup = incomingHolyWars.length > 0
    ? incomingHolyWars
      .map((entry) => `<div class="faith-detail">Incoming holy war from ${entry.sourceFactionName}, ${entry.faithName}, ${Math.round(entry.remainingSeconds)}s remaining.</div>`)
      .join("")
    : '<div class="faith-detail">No incoming holy war is currently pressing the realm.</div>';

  const divineRightReadinessMarkup = player.faith.selectedFaith
    ? `<div class="faith-detail">Divine Right readiness: ${player.faith.divineRightRecognitionSharePct ?? 0}% / ${player.faith.divineRightRequiredSharePct ?? 0}% recognition | ${player.faith.divineRightApexStructureName ? `apex ${player.faith.divineRightApexStructureName}` : "no apex covenant structure"} | ${player.faith.divineRightEligible ? "window ready" : "window sealed"}</div>`
    : "";

  const covenantTestStandingMarkup = player.faith.selectedFaith
    ? activeCovenantTest
      ? `<div class="faith-detail">Covenant Test active: ${activeCovenantTest.mandateLabel} | ${activeCovenantTest.progressPct ?? 0}% | ${Math.round(activeCovenantTest.remainingInWorldDays ?? 0)} in-world days remaining.</div>`
      : player.faith.covenantTestPassed
        ? '<div class="faith-detail">Covenant Test cleared. Apex covenant ascent is now open to this bloodline.</div>'
        : '<div class="faith-detail">Covenant Test not yet cleared. Reach apex intensity with a living Grand Sanctuary to draw the covenant mandate.</div>'
    : "";

  const activeCovenantTestMarkup = activeCovenantTest
    ? `
      <div class="faith-detail">${activeCovenantTest.summary}</div>
      <div class="faith-detail">${activeCovenantTest.objectiveText}</div>
      <div class="faith-detail">${activeCovenantTest.statusText ?? "Awaiting covenant response."}</div>
      ${activeCovenantTest.shortfalls?.length
        ? activeCovenantTest.shortfalls.map((entry) => `<div class="faith-detail">Shortfall: ${entry}</div>`).join("")
        : '<div class="faith-detail">No current shortfalls. The mandate can resolve immediately once the court acts.</div>'}
    `
    : '<div class="faith-detail">No active Covenant Test is currently pressing the court.</div>';

  const incomingCovenantTestMarkup = incomingCovenantTests.length > 0
    ? incomingCovenantTests
      .map((entry) => `<div class="faith-detail">${entry.sourceFactionName} faces ${entry.mandateLabel}, ${Math.round(entry.remainingInWorldDays ?? 0)} in-world days remaining.</div>`)
      .join("")
    : '<div class="faith-detail">No rival covenant tests are currently unfolding.</div>';

  const lastCovenantTestOutcomeMarkup = `<div class="faith-detail">${formatCovenantTestOutcome(lastCovenantTestOutcome)}</div>`;

  const activeDivineRightMarkup = activeDivineRightDeclaration
    ? `<div class="faith-detail">Divine Right declared under ${activeDivineRightDeclaration.faithName ?? activeDivineRightDeclaration.faithId}, ${Math.round(activeDivineRightDeclaration.remainingSeconds ?? 0)}s remaining, ${activeDivineRightDeclaration.recognitionSharePct ?? 0}% / ${activeDivineRightDeclaration.requiredSharePct ?? 0}% recognition${activeDivineRightDeclaration.structureName ? ` | ${activeDivineRightDeclaration.structureName}` : ""}.</div>`
    : '<div class="faith-detail">No Divine Right declaration is currently active.</div>';

  const incomingDivineRightMarkup = incomingDivineRightDeclarations.length > 0
    ? incomingDivineRightDeclarations
      .map((entry) => `<div class="faith-detail">Incoming Divine Right from ${entry.sourceFactionName}, ${entry.faithName}, ${Math.round(entry.remainingSeconds ?? 0)}s remaining, ${entry.recognitionSharePct ?? 0}% / ${entry.requiredSharePct ?? 0}% recognition.</div>`)
      .join("")
    : '<div class="faith-detail">No rival Divine Right declaration is currently unfolding.</div>';

  const lastDivineRightOutcomeMarkup = lastDivineRightOutcome
    ? `<div class="faith-detail">Last Divine Right ${lastDivineRightOutcome.outcome === "completed" ? "completed" : "failed"} under ${lastDivineRightOutcome.faithName ?? lastDivineRightOutcome.faithId}${lastDivineRightOutcome.reason ? ` | ${lastDivineRightOutcome.reason}` : ""}.</div>`
    : '<div class="faith-detail">No prior Divine Right outcome is recorded.</div>';

  if (!player.faith.selectedFaithId) {
    player.faith.exposures
      .filter((faith) => faith.availableToCommit)
      .forEach((faith) => {
        actionGrid.appendChild(
          createActionButton({
            label: `${faith.name} • Light`,
            detail: describeDoctrineEffects(faith.prototypeEffects?.light),
            onClick: () => {
              const result = chooseFaithCommitment(state, "player", faith.id, "light");
              if (!result.ok) {
                postFaithFailure(result.reason);
              }
              renderPanels();
            },
          }),
        );
        actionGrid.appendChild(
          createActionButton({
            label: `${faith.name} • Dark`,
            detail: describeDoctrineEffects(faith.prototypeEffects?.dark),
            warning: true,
            onClick: () => {
              const result = chooseFaithCommitment(state, "player", faith.id, "dark");
              if (!result.ok) {
                postFaithFailure(result.reason);
              }
              renderPanels();
            },
          }),
        );
      });
  } else {
    if (activeCovenantTest) {
      actionGrid.appendChild(
        createActionButton({
          label: activeCovenantTest.actionLabel ?? "Advance Covenant Test",
          detail: activeCovenantTest.actionAvailable
            ? activeCovenantTest.actionDetail ?? activeCovenantTest.statusText ?? activeCovenantTest.objectiveText
            : activeCovenantTest.shortfalls?.[0] ?? activeCovenantTest.statusText ?? activeCovenantTest.objectiveText,
          warning: activeCovenantTest.actionAvailable,
          disabled: !activeCovenantTest.actionAvailable,
          onClick: () => {
            const result = performCovenantTestAction(state, "player");
            if (!result.ok) {
              postFaithFailure(result.reason);
            }
            renderPanels();
          },
        }),
      );
    }
    actionGrid.appendChild(
      createActionButton({
        label: "Declare Divine Right",
        detail: formatFaithOperationTerms(divineRightTerms),
        warning: divineRightTerms?.available ?? false,
        disabled: !divineRightTerms?.available,
        onClick: () => {
          const result = startDivineRightDeclaration(state, "player");
          if (!result.ok) {
            postFaithFailure(result.reason);
          }
          renderPanels();
        },
      }),
    );
    rivalCourts.forEach((rival) => {
      const missionary = getMissionaryTerms(state, "player", rival.id);
      actionGrid.appendChild(
        createActionButton({
          label: `Missionaries, ${rival.displayName}`,
          detail: formatFaithOperationTerms(missionary),
          disabled: !missionary.available,
          onClick: () => {
            const result = startMissionaryOperation(state, "player", rival.id);
            if (!result.ok) {
              postFaithFailure(result.reason);
            }
            renderPanels();
          },
        }),
      );

      const holyWar = getHolyWarDeclarationTerms(state, "player", rival.id);
      actionGrid.appendChild(
        createActionButton({
          label: `Declare Holy War, ${rival.displayName}`,
          detail: formatFaithOperationTerms(holyWar),
          warning: holyWar.available,
          disabled: !holyWar.available,
          onClick: () => {
            const result = startHolyWarDeclaration(state, "player", rival.id);
            if (!result.ok) {
              postFaithFailure(result.reason);
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
      ${player.faith.selectedFaith ? `<div class="faith-detail">${describeDoctrineEffects(player.faith.doctrineEffects)}</div>` : ""}
      ${player.faith.selectedFaith && realmSnapshot?.fortification?.wardLabel ? `<div class="faith-detail">Keep ward: ${realmSnapshot.fortification.wardLabel}${realmSnapshot.fortification.wardSurgeActive ? " • sacrifice surge active" : ""}</div>` : ""}
      ${covenantTestStandingMarkup}
      <div class="faith-detail"><strong>Covenant Test</strong></div>
      ${activeCovenantTestMarkup}
      <div class="faith-detail"><strong>Incoming Covenant Tests</strong></div>
      ${incomingCovenantTestMarkup}
      <div class="faith-detail"><strong>Last Covenant Test Outcome</strong></div>
      ${lastCovenantTestOutcomeMarkup}
      ${divineRightReadinessMarkup}
      <div class="faith-detail"><strong>Divine Right</strong></div>
      ${activeDivineRightMarkup}
      <div class="faith-detail"><strong>Incoming Divine Right</strong></div>
      ${incomingDivineRightMarkup}
      <div class="faith-detail"><strong>Last declaration outcome</strong></div>
      ${lastDivineRightOutcomeMarkup}
      <div class="faith-detail"><strong>Faith operations</strong></div>
      ${operationMarkup}
      <div class="faith-detail"><strong>Declared holy wars</strong></div>
      ${holyWarMarkup}
      <div class="faith-detail"><strong>Incoming holy wars</strong></div>
      ${incomingHolyWarMarkup}
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
  const realmSnapshot = getRealmConditionSnapshot(state, "player");
  const territoryStatus = state.world.controlPoints
    .map((controlPoint) =>
      `${controlPoint.name}: ${controlPoint.contested ? "contested" : controlPoint.ownerFactionId ? `${state.factions[controlPoint.ownerFactionId].displayName} ${controlPoint.controlState}` : "neutral"}`,
    )
    .join(" | ");
  const captiveCount = player.dynasty?.captives?.length ?? 0;
  const operationCount = player.dynasty?.operations?.active?.length ?? 0;
  const fallenCount = player.dynasty?.attachments?.fallenMembers?.length ?? 0;
  const heirLabel = player.dynasty?.attachments?.heirMember?.title ?? (player.dynasty?.interregnum ? "interregnum" : "default chain");
  const successionCrisisLabel = player.dynasty?.activeSuccessionCrisis
    ? `${player.dynasty.activeSuccessionCrisis.severityLabel} succession crisis`
    : null;
  const activeTerritorialGovernanceRecognition = realmSnapshot?.dynasty?.activeTerritorialGovernanceRecognition ?? null;
  const activeCovenantTest = realmSnapshot?.faith?.activeCovenantTest ?? null;
  const activeDivineRight = realmSnapshot?.faith?.activeDivineRightDeclaration ?? null;
  const incomingDivineRight = (realmSnapshot?.faith?.incomingDivineRightDeclarations ?? [])[0] ?? null;
  const governanceDebugLabel = formatGovernanceStatusSummary(activeTerritorialGovernanceRecognition);
  const covenantTestDebugLabel = activeCovenantTest
    ? ` | Covenant Test ${activeCovenantTest.mandateLabel} ${Math.ceil(activeCovenantTest.remainingInWorldDays ?? 0)}d`
    : realmSnapshot?.faith?.covenantTestPassed
      ? " | Covenant Test cleared"
      : "";
  const divineRightDebugLabel = activeDivineRight
    ? ` | Divine Right ${Math.ceil(activeDivineRight.remainingSeconds ?? 0)}s ${activeDivineRight.recognitionSharePct ?? 0}%/${activeDivineRight.requiredSharePct ?? 0}%`
    : incomingDivineRight
      ? ` | Rival Divine Right ${Math.ceil(incomingDivineRight.remainingSeconds ?? 0)}s ${incomingDivineRight.sourceFactionName ?? "rival"}`
      : "";
  const matchDebugLine = realmSnapshot
    ? `Match: stage ${realmSnapshot.cycle?.stageNumber ?? 1} ${realmSnapshot.cycle?.phaseLabel ?? "Emergence"} | year ${(realmSnapshot.dualClock?.inWorldYears ?? 0).toFixed(2)} | declarations ${realmSnapshot.cycle?.declarationCount ?? 0}${realmSnapshot.cycle?.greatReckoningActive ? realmSnapshot.cycle?.greatReckoningTargetFactionId === "player" ? ` | Great Reckoning ${Math.round((realmSnapshot.cycle?.greatReckoningShare ?? 0) * 100)}%` : ` | Reckoning on ${realmSnapshot.cycle?.greatReckoningTargetFactionName ?? "rival"}` : ""}${realmSnapshot.fortification?.imminentEngagement?.phase === "countdown" ? ` | Immediate ${Math.ceil(realmSnapshot.fortification.imminentEngagement.remainingSeconds ?? 0)}s ${realmSnapshot.fortification.imminentEngagement.selectedResponseLabel ?? "Steady Defense"}` : realmSnapshot.fortification?.imminentEngagement?.phase === "engaged" ? ` | Engagement joined ${realmSnapshot.fortification.imminentEngagement.selectedResponseLabel ?? "Steady Defense"}` : ""}${governanceDebugLabel}${covenantTestDebugLabel}${divineRightDebugLabel}`
    : null;
  refs.debugOverlay.textContent = [
    `House: ${content.byId.houses[player.houseId].name}`,
    `Status: ${state.meta.status}`,
    `Selected units: ${selection.units.length}`,
    `Selected buildings: ${selection.buildings.length}`,
    `Population: total ${player.population.total}, used ${player.population.used}, reserved ${player.population.reserved}, available ${player.population.available}, cap ${player.population.cap}`,
    `Resources: gold ${Math.floor(player.resources.gold)}, food ${player.resources.food.toFixed(1)}, water ${player.resources.water.toFixed(1)}, wood ${Math.floor(player.resources.wood)}, stone ${Math.floor(player.resources.stone)}, iron ${Math.floor(player.resources.iron)}, influence ${player.resources.influence.toFixed(1)}`,
    `Territory: ${player.territories.count}/${state.world.controlPoints.length} held`,
    `Faith: ${player.faith.selectedFaith ? `${player.faith.selectedFaith.name} ${player.faith.tierLabel}` : "Unaligned"}`,
    `Conviction: ${player.conviction.bandLabel}`,
    `Ledger: ruth ${player.conviction.buckets.ruthlessness} • steward ${player.conviction.buckets.stewardship} • oath ${player.conviction.buckets.oathkeeping} • desecrate ${player.conviction.buckets.desecration}`,
    `Dynasty: legitimacy ${Math.round(player.dynasty?.legitimacy ?? 0)} • heir ${heirLabel} • captives ${captiveCount} • fallen ${fallenCount}${successionCrisisLabel ? ` • ${successionCrisisLabel}` : ""}`,
    realmSnapshot ? `Realm: cycle ${realmSnapshot.cycleCount} • food ${realmSnapshot.food.band} • water ${realmSnapshot.water.band} • fort ${realmSnapshot.fortification.tier}/${realmSnapshot.fortification.ceiling}${realmSnapshot.fortification.wardLabel ? ` • ${realmSnapshot.fortification.wardLabel}` : ""}` : null,
    `Siege: engines ${player.siege?.engineCount ?? 0} • supplied ${player.siege?.suppliedEngines ?? 0} • engineers ${player.siege?.engineerCount ?? 0} • wagons ${player.siege?.supplyWagonCount ?? 0} • camps ${player.siege?.supplyCampCount ?? 0}`,
    realmSnapshot ? `Field water: hydrated ${realmSnapshot.logistics.fieldWaterSupportedUnits}/${realmSnapshot.logistics.fieldWaterUnitCount} | strained ${realmSnapshot.logistics.fieldWaterStrainedUnits} | critical ${realmSnapshot.logistics.fieldWaterCriticalUnits} | attriting ${realmSnapshot.logistics.fieldWaterAttritionUnits ?? 0} | breaking ${realmSnapshot.logistics.fieldWaterDesertionRiskUnits ?? 0}` : null,
    `Control points: ${territoryStatus}`,
    `Build mode: ${uiState.buildMode ? content.byId.buildings[uiState.buildMode].name : "none"}`,
    matchDebugLine,
  ].filter(Boolean).join("\n");
}

function renderMessages() {
  refs.messageLog.innerHTML = state.messages
    .map((message) => `<div class="message-entry ${message.tone === "warn" ? "message-entry--warn" : message.tone === "good" ? "message-entry--good" : ""}">${message.text}</div>`)
    .join("");
}

function renderPanels() {
  updateResourceBar();
  renderRealmConditionBar();
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
    updateMinorHouseAi(state, dt);
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

// Session 11: URL-driven house-select. Supports ?house=stonehelm to swap the player
// and enemy house assignments. Stonehelm as second playable house honors Decision 4
// (house rollout) while keeping the map untouched. Further houses can slot in here as
// they become playable without changes to the map schema.
function applyHouseSelectOverride(contentRef) {
  const params = typeof window !== "undefined" ? new URLSearchParams(window.location.search) : null;
  if (typeof window !== "undefined") {
    window.__BLOODLINES_DBG = window.__BLOODLINES_DBG || {};
    window.__BLOODLINES_DBG.overrideCalled = true;
    window.__BLOODLINES_DBG.search = window.location.search;
  }
  if (!params) return contentRef;
  const requested = params.get("house");
  if (typeof window !== "undefined") {
    window.__BLOODLINES_DBG.requestedHouse = requested;
  }
  if (!requested) return contentRef;
  const targetHouse = contentRef.byId.houses[requested];
  if (typeof window !== "undefined") {
    window.__BLOODLINES_DBG.targetHouseFound = Boolean(targetHouse);
    window.__BLOODLINES_DBG.targetHouseName = targetHouse?.name;
    window.__BLOODLINES_DBG.targetHousePlayable = targetHouse?.prototypePlayable;
    window.__BLOODLINES_DBG.housesKeys = Object.keys(contentRef.byId?.houses ?? {});
  }
  if (!targetHouse) {
    setLaunchStatus(`Unknown house ${requested}; falling back to canonical Ironmark.`, "error");
    return contentRef;
  }
  if (!targetHouse.prototypePlayable) {
    setLaunchStatus(`${targetHouse.name} is not yet prototype-playable; falling back to Ironmark.`, "error");
    return contentRef;
  }
  const playerFaction = contentRef.map.factions.find((f) => f.id === "player");
  const enemyFaction = contentRef.map.factions.find((f) => f.id === "enemy");
  if (typeof window !== "undefined") {
    window.__BLOODLINES_DBG.playerFactionFound = Boolean(playerFaction);
    window.__BLOODLINES_DBG.enemyFactionFound = Boolean(enemyFaction);
    window.__BLOODLINES_DBG.playerHouseBefore = playerFaction?.houseId;
  }
  if (!playerFaction || !enemyFaction) return contentRef;
  if (playerFaction.houseId === requested) return contentRef;
  // Swap: give the player the requested house, demote the previous player house to enemy.
  const previousPlayerHouse = playerFaction.houseId;
  playerFaction.houseId = requested;
  enemyFaction.houseId = previousPlayerHouse;
  if (typeof window !== "undefined") {
    window.__BLOODLINES_DBG.swapped = true;
    window.__BLOODLINES_DBG.playerHouseAfter = playerFaction.houseId;
    window.__BLOODLINES_DBG.enemyHouseAfter = enemyFaction.houseId;
  }
  return contentRef;
}

async function startGame() {
  setLaunchStatus("Loading prototype data...", "loading");
  refs.startButton.disabled = true;
  if (!content) {
    content = await loadGameContent();
  }
  applyHouseSelectOverride(content);
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

function formatGovernanceStatusSummary(recognition) {
  if (!recognition) {
    return "";
  }
  const acceptanceLabel = typeof recognition.populationAcceptancePct === "number" &&
      typeof recognition.populationAcceptanceThresholdPct === "number"
    ? ` ${recognition.populationAcceptancePct}%/${recognition.populationAcceptanceThresholdPct}%`
    : "";
  if (recognition.completed) {
    return ` | Governance victory ${recognition.victoryLoyalTerritoryCount ?? 0}/${recognition.territoryCount ?? 0}${acceptanceLabel}`;
  }
  if (recognition.victoryReady) {
    return ` | Governance victory ${Math.ceil(recognition.victoryRemainingSeconds ?? 0)}s${acceptanceLabel}`;
  }
  if (recognition.integrationReady) {
    return ` | Governance acceptance ${recognition.populationAcceptancePct ?? 0}%/${recognition.populationAcceptanceThresholdPct ?? 65}%`;
  }
  if (recognition.recognized) {
    return ` | Governance held ${recognition.territorySharePct ?? 0}%/${recognition.requiredSharePct ?? 0}%${acceptanceLabel}`;
  }
  return ` | Governance ${Math.ceil(recognition.remainingSeconds ?? 0)}s ${recognition.territorySharePct ?? 0}%/${recognition.requiredSharePct ?? 0}%${acceptanceLabel}`;
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
  const scoutRaiders = getSelectedScoutRaiders();

  if (target?.type === "resource" && workers.length > 0) {
    issueGatherCommand(state, workers.map((unit) => unit.id), target.entity.id);
    return;
  }

  if (
    target?.type === "resource" &&
    scoutRaiders.length > 0 &&
    scoutRaiders.length === combatUnits.length
  ) {
    const result = issueRaidCommand(state, scoutRaiders.map((unit) => unit.id), target.type, target.entity.id);
    if (!result.ok) {
      state.messages.unshift({
        id: `raid-fail-${Date.now()}`,
        text: result.reason,
        tone: "warn",
        ttl: 4,
      });
    }
    return;
  }

  if (
    target?.type === "unit" &&
    target.entity.factionId !== "player" &&
    (
      content.byId.units[target.entity.typeId]?.role === "worker" ||
      content.byId.units[target.entity.typeId]?.movingLogisticsCarrier
    ) &&
    scoutRaiders.length > 0 &&
    scoutRaiders.length === combatUnits.length
  ) {
    const result = issueRaidCommand(state, scoutRaiders.map((unit) => unit.id), target.type, target.entity.id);
    if (!result.ok) {
      state.messages.unshift({
        id: `raid-fail-${Date.now()}`,
        text: result.reason,
        tone: "warn",
        ttl: 4,
      });
    }
    return;
  }

  if (
    target?.type === "building" &&
    target.entity.factionId !== "player" &&
    scoutRaiders.length > 0 &&
    scoutRaiders.length === combatUnits.length &&
    content.byId.buildings[target.entity.typeId]?.scoutRaidable
  ) {
    const result = issueRaidCommand(state, scoutRaiders.map((unit) => unit.id), target.type, target.entity.id);
    if (!result.ok) {
      state.messages.unshift({
        id: `raid-fail-${Date.now()}`,
        text: result.reason,
        tone: "warn",
        ttl: 4,
      });
    }
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
