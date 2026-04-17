import { readFile } from "node:fs/promises";
import path from "node:path";

const root = process.cwd();

async function readJson(relativePath) {
  const filePath = path.join(root, relativePath);
  return JSON.parse(await readFile(filePath, "utf8"));
}

function assert(condition, message) {
  if (!condition) {
    throw new Error(message);
  }
}

const [houses, resources, units, buildings, faiths, roles, paths, victoryConditions, settlementClasses, realmConditions, map] = await Promise.all([
  readJson("data/houses.json"),
  readJson("data/resources.json"),
  readJson("data/units.json"),
  readJson("data/buildings.json"),
  readJson("data/faiths.json"),
  readJson("data/bloodline-roles.json"),
  readJson("data/bloodline-paths.json"),
  readJson("data/victory-conditions.json"),
  readJson("data/settlement-classes.json"),
  readJson("data/realm-conditions.json"),
  readJson("data/maps/ironmark-frontier.json"),
]);

assert(houses.length === 9, "Expected 9 canonical houses.");
assert(houses.some((house) => house.id === "ironmark" && house.prototypePlayable), "Ironmark must be the playable prototype house.");
assert(houses.some((house) => house.id === "stonehelm" && house.prototypePlayable), "Session 11: Stonehelm must be prototype-playable.");
assert(houses.some((house) => house.id === "hartvale" && house.prototypePlayable), "Session 63: Hartvale must be prototype-playable.");
assert((houses.find((h) => h.id === "stonehelm")?.mechanics?.fortificationCostMultiplier ?? 1) < 1, "Stonehelm must expose fortification cost discount.");
assert((houses.find((h) => h.id === "stonehelm")?.mechanics?.fortificationBuildSpeedMultiplier ?? 1) > 1, "Stonehelm must expose fortification build-speed multiplier.");
assert(units.find((unit) => unit.id === "verdant_warden"), "Session 11: Hartvale Verdant Warden must exist in unit layer.");
assert(units.find((unit) => unit.id === "verdant_warden")?.prototypeEnabled, "Session 63: Verdant Warden must be prototype-enabled.");
assert(units.find((unit) => unit.id === "verdant_warden")?.house === "hartvale", "Verdant Warden must be marked as Hartvale unique.");
assert((units.find((unit) => unit.id === "verdant_warden")?.defenseRating ?? 0) === 5, "Verdant Warden must carry canonical Def 5.");
assert((units.find((unit) => unit.id === "verdant_warden")?.offenseRating ?? 0) === 4, "Verdant Warden must carry canonical Off 4.");
assert(units.find((unit) => unit.id === "axeman"), "Session 69: Ironmark Axeman must exist in unit layer.");
assert(units.find((unit) => unit.id === "axeman")?.prototypeEnabled, "Session 69: Ironmark Axeman must be prototype-enabled.");
assert(units.find((unit) => unit.id === "axeman")?.house === "ironmark", "Ironmark Axeman must be marked as Ironmark unique.");
assert((units.find((unit) => unit.id === "axeman")?.ironmarkBloodPrice ?? 0) >= 2, "Ironmark Axeman must levy at least 2 blood population when trained.");
assert((units.find((unit) => unit.id === "axeman")?.bloodProductionLoadDelta ?? 0) > 1.5, "Ironmark Axeman must add elevated blood-production load.");
assert(units.find((unit) => unit.id === "scout_rider"), "Session 79: Scout Rider must exist in unit layer.");
assert(units.find((unit) => unit.id === "scout_rider")?.prototypeEnabled, "Session 79: Scout Rider must be prototype-enabled.");
assert(units.find((unit) => unit.id === "scout_rider")?.role === "light-cavalry", "Scout Rider must expose light-cavalry role.");
assert((units.find((unit) => unit.id === "scout_rider")?.raidDurationSeconds ?? 0) > 0, "Scout Rider must expose raid duration.");
assert((units.find((unit) => unit.id === "scout_rider")?.raidCooldownSeconds ?? 0) > 0, "Scout Rider must expose raid cooldown.");
assert((units.find((unit) => unit.id === "scout_rider")?.nodeHarassDurationSeconds ?? 0) > 0, "Session 80: Scout Rider must expose direct resource-node harassment duration.");
assert((units.find((unit) => unit.id === "scout_rider")?.workerHarassRadius ?? 0) > 0, "Session 80: Scout Rider must expose worker-harassment radius.");
assert((units.find((unit) => unit.id === "scout_rider")?.workerRetreatSeconds ?? 0) > 0, "Session 80: Scout Rider must expose worker-retreat timing.");
assert(resources.some((resource) => resource.id === "iron"), "Iron must exist in the resource layer.");
assert(resources.some((resource) => resource.id === "influence"), "Influence must exist in the resource layer.");
assert(resources.filter((resource) => resource.category === "primary" && resource.enabledInPrototype).length === 6, "Expected six enabled primary resources.");
assert(faiths.length === 4, "Expected all four covenants.");
assert(settlementClasses.length === 6, "Expected six canonical settlement classes.");
assert(realmConditions.cycleSeconds === 90, "Expected the canonical 90-second realm cycle.");
assert((realmConditions.effects?.stabilitySurplus?.foodRatio ?? 0) >= 1.5, "Realm conditions must define a food-surplus stability threshold.");
assert((realmConditions.effects?.stabilitySurplus?.waterRatio ?? 0) >= 1.5, "Realm conditions must define a water-surplus stability threshold.");
assert((realmConditions.effects?.stabilitySurplus?.loyaltyDeltaPerCycle ?? 0) > 0, "Realm conditions must define positive loyalty reinforcement under surplus.");
faiths.forEach((faith) => {
  assert(faith.prototypeEffects?.light, `Expected light doctrine effects for ${faith.id}.`);
  assert(faith.prototypeEffects?.dark, `Expected dark doctrine effects for ${faith.id}.`);
});
assert(roles.length >= 8, "Expected expanded bloodline role definitions.");
assert(paths.length === 7, "Expected the seven bloodline training paths.");
assert(victoryConditions.filter((item) => item.status === "active-canon").length === 5, "Expected five active canon victory conditions.");

const unitIds = new Set(units.map((unit) => unit.id));
const buildingIds = new Set(buildings.map((building) => building.id));
const resourceIds = new Set(resources.map((resource) => resource.id));

buildings.forEach((building) => {
  Object.keys(building.cost ?? {}).forEach((resourceId) => {
    assert(resourceIds.has(resourceId), `Unknown building cost resource: ${resourceId}`);
  });
  Object.keys(building.raidLoss ?? {}).forEach((resourceId) => {
    assert(resourceIds.has(resourceId), `Unknown building raid-loss resource: ${resourceId}`);
  });
  (building.trainableUnits ?? []).forEach((unitId) => {
    assert(unitIds.has(unitId), `Unknown trainable unit: ${unitId}`);
  });
});

units.forEach((unit) => {
  Object.keys(unit.cost ?? {}).forEach((resourceId) => {
    assert(resourceIds.has(resourceId), `Unknown unit cost resource: ${resourceId}`);
  });
});

assert(buildings.find((building) => building.id === "quarry")?.dropoffResources?.includes("stone"), "Quarry must drop off stone.");
assert(buildings.find((building) => building.id === "iron_mine")?.smeltingFuelResource === "wood", "Iron Mine must consume wood as smelting fuel.");
assert(buildings.find((building) => building.id === "barracks")?.trainableUnits?.includes("verdant_warden"), "Session 63: Barracks must expose Verdant Warden in the trainable roster.");
assert(buildings.find((building) => building.id === "stable"), "Session 79: Stable must exist in the building layer.");
assert(buildings.find((building) => building.id === "stable")?.trainableUnits?.includes("scout_rider"), "Session 79: Stable must train Scout Rider.");
assert(buildings.find((building) => building.id === "supply_camp")?.scoutRaidable === true, "Session 79: Supply Camp must expose scout-raidable logistics state.");
assert(buildings.find((building) => building.id === "well")?.scoutRaidable === true, "Session 79: Well must expose scout-raidable water-state.");
assert(buildings.find((building) => building.id === "siege_workshop")?.trainableUnits?.includes("ram"), "Siege Workshop must train Rams.");
assert(buildings.find((building) => building.id === "siege_workshop")?.trainableUnits?.includes("siege_tower"), "Siege Workshop must train Siege Towers.");
assert(buildings.find((building) => building.id === "siege_workshop")?.trainableUnits?.includes("trebuchet"), "Siege Workshop must train Trebuchets.");
assert(buildings.find((building) => building.id === "siege_workshop")?.trainableUnits?.includes("siege_engineer"), "Siege Workshop must train Siege Engineers.");
assert(buildings.find((building) => building.id === "siege_workshop")?.trainableUnits?.includes("supply_wagon"), "Siege Workshop must train Supply Wagons.");
assert(buildings.find((building) => building.id === "supply_camp")?.supportsSiegeLogistics === true, "Supply Camp must expose siege-logistics support.");
assert((buildings.find((building) => building.id === "well")?.armyWaterSupportRadius ?? 0) > 0, "Well must expose army water-support radius.");
assert((buildings.find((building) => building.id === "supply_camp")?.armyWaterSupportRadius ?? 0) > 0, "Supply Camp must expose army water-support radius.");
assert((buildings.find((building) => building.id === "well")?.armyWaterSupportDurationSeconds ?? 0) > 0, "Well must expose army water-support duration.");
assert((buildings.find((building) => building.id === "supply_camp")?.armyWaterSupportDurationSeconds ?? 0) > 0, "Supply Camp must expose army water-support duration.");
assert(buildings.find((building) => building.id === "barracks")?.trainableUnits?.includes("axeman"), "Session 69: Barracks must expose Axeman in the trainable roster.");

// Session 13: Faith prototype enablement + Wayshrine canonical shrine building.
faiths.forEach((faith) => {
  assert(faith.prototypeEnabled === true, `Session 13: ${faith.id} must be prototypeEnabled for faith prototype enablement.`);
});
assert(buildings.find((b) => b.id === "wayshrine"), "Wayshrine canonical shrine building must exist.");
// Session 17: Covenant Hall L2.
assert(buildings.find((b) => b.id === "covenant_hall"), "Covenant Hall canonical L2 building must exist.");
assert(buildings.find((b) => b.id === "covenant_hall")?.faithTier === 2, "Covenant Hall must be faithTier 2.");
assert(buildings.find((b) => b.id === "covenant_hall")?.requiresFaithCommitment === true, "Covenant Hall must require faith commitment.");
assert((buildings.find((b) => b.id === "covenant_hall")?.faithExposureAmplification ?? 0) > (buildings.find((b) => b.id === "wayshrine")?.faithExposureAmplification ?? 0), "Covenant Hall amp must exceed Wayshrine amp.");

// Session 20: canonical 8-unit L3 faith roster (2 per covenant per doctrine path).
const expectedL3FaithUnits = [
  ["flame_warden", "old_light", "light"],
  ["judgment_pyre", "old_light", "dark"],
  ["consecrated_blade", "blood_dominion", "light"],
  ["crimson_reaver", "blood_dominion", "dark"],
  ["mandate_sentinel", "the_order", "light"],
  ["edict_enforcer", "the_order", "dark"],
  ["root_warden", "the_wild", "light"],
  ["predator_stalker", "the_wild", "dark"],
];
expectedL3FaithUnits.forEach(([unitId, faithId, doctrine]) => {
  const def = units.find((u) => u.id === unitId);
  assert(def, `Session 20: L3 faith unit ${unitId} must exist.`);
  assert(def.faithId === faithId, `${unitId} must bind to faith ${faithId}.`);
  assert(def.doctrinePath === doctrine, `${unitId} must bind to doctrine ${doctrine}.`);
  assert(def.role === "faith-unit", `${unitId} must be role faith-unit.`);
  assert(def.prototypeEnabled, `${unitId} must be prototypeEnabled.`);
});
const hallTrainable = buildings.find((b) => b.id === "covenant_hall")?.trainableUnits ?? [];
expectedL3FaithUnits.forEach(([unitId]) => {
  assert(hallTrainable.includes(unitId), `Covenant Hall must train ${unitId}.`);
});

// Session 21: Grand Sanctuary L3 + canonical L4 faith unit roster.
assert(buildings.find((b) => b.id === "grand_sanctuary"), "Grand Sanctuary L3 covenant building must exist.");
assert(buildings.find((b) => b.id === "grand_sanctuary")?.faithTier === 3, "Grand Sanctuary must be faithTier 3.");
assert(buildings.find((b) => b.id === "grand_sanctuary")?.requiresFaithCommitment === true, "Grand Sanctuary must require faith commitment.");
assert((buildings.find((b) => b.id === "grand_sanctuary")?.faithExposureAmplification ?? 0) > (buildings.find((b) => b.id === "covenant_hall")?.faithExposureAmplification ?? 0), "Sanctuary amp must exceed Hall amp.");
const expectedL4FaithUnits = [
  ["flame_herald", "old_light", "light"],
  ["pyre_sovereign", "old_light", "dark"],
  ["blood_consort", "blood_dominion", "light"],
  ["crimson_exarch", "blood_dominion", "dark"],
  ["mandate_paladin", "the_order", "light"],
  ["edict_inquisitor", "the_order", "dark"],
  ["elder_grove_keeper", "the_wild", "light"],
  ["predator_ascendant", "the_wild", "dark"],
];
expectedL4FaithUnits.forEach(([unitId, faithId, doctrine]) => {
  const def = units.find((u) => u.id === unitId);
  assert(def, `Session 21: L4 faith unit ${unitId} must exist.`);
  assert(def.stage === 4, `${unitId} must be stage 4.`);
  assert(def.faithId === faithId, `${unitId} must bind to faith ${faithId}.`);
  assert(def.doctrinePath === doctrine, `${unitId} must bind to doctrine ${doctrine}.`);
});
const sanctuaryTrainable = buildings.find((b) => b.id === "grand_sanctuary")?.trainableUnits ?? [];
expectedL4FaithUnits.forEach(([unitId]) => {
  assert(sanctuaryTrainable.includes(unitId), `Grand Sanctuary must train ${unitId}.`);
});

// Session 22: L4 Apex covenant structure + canonical L5 apex unit roster.
const apex = buildings.find((b) => b.id === "apex_covenant");
assert(apex, "Apex Covenant L4 building must exist.");
assert(apex.faithTier === 4, "Apex Covenant must be faithTier 4.");
assert(apex.requiresFaithCommitment === true, "Apex Covenant must require faith commitment.");
assert((apex.requiresFaithIntensity ?? 0) >= 80, "Apex Covenant must require intensity >= 80 (Fervent).");
assert((apex.faithExposureAmplification ?? 0) > (buildings.find((b) => b.id === "grand_sanctuary")?.faithExposureAmplification ?? 0), "Apex amp must exceed Sanctuary amp.");
const expectedL5ApexUnits = [
  ["the_unbroken", "old_light"],
  ["the_sacrificed", "blood_dominion"],
  ["the_mandate", "the_order"],
  ["the_first_wild", "the_wild"],
];
expectedL5ApexUnits.forEach(([unitId, faithId]) => {
  const def = units.find((u) => u.id === unitId);
  assert(def, `Session 22: L5 apex unit ${unitId} must exist.`);
  assert(def.stage === 5, `${unitId} must be stage 5.`);
  assert(def.role === "faith-apex", `${unitId} must be role faith-apex.`);
  assert(def.faithId === faithId, `${unitId} must bind to faith ${faithId}.`);
  assert(def.prototypeEnabled, `${unitId} must be prototypeEnabled.`);
});
const apexTrainable = apex.trainableUnits ?? [];
expectedL5ApexUnits.forEach(([unitId]) => {
  assert(apexTrainable.includes(unitId), `Apex Covenant must train ${unitId}.`);
});

// Session 27: Naval foundation first canonical layer.
assert(buildings.find((b) => b.id === "harbor_tier_1"), "Harbor canonical L1 building must exist.");
assert(buildings.find((b) => b.id === "harbor_tier_1")?.navalRole === "harbor", "Harbor must expose navalRole harbor.");
assert(buildings.find((b) => b.id === "harbor_tier_1")?.requiresCoastalAdjacency === true, "Harbor must require coastal adjacency.");
const expectedVessels = ["fishing_boat", "scout_vessel", "war_galley"];
expectedVessels.forEach((unitId) => {
  const u = units.find((unit) => unit.id === unitId);
  assert(u, `Session 27: vessel ${unitId} must exist.`);
  assert(u.role === "vessel", `${unitId} must be role vessel.`);
  assert(u.movementDomain === "water", `${unitId} must have movementDomain water.`);
  assert(u.prototypeEnabled, `${unitId} must be prototypeEnabled.`);
});
const harborTrainable = buildings.find((b) => b.id === "harbor_tier_1")?.trainableUnits ?? [];
expectedVessels.forEach((unitId) => {
  assert(harborTrainable.includes(unitId), `Harbor must train ${unitId}.`);
});
// Canonical: map must include at least one water patch for naval foundation.
const waterPatches = (map.terrainPatches ?? []).filter((p) => p.type === "water");
assert(waterPatches.length >= 1, "Canonical map must include at least one water terrain patch.");

// Session 32: Continental architecture — secondary continent on the map.
const offHomeControlPoints = (map.controlPoints ?? []).filter((cp) => cp.continentId && cp.continentId !== "home");
assert(offHomeControlPoints.length >= 1, "Session 32: map must include at least one secondary-continent control point.");
const cliffsong = (map.controlPoints ?? []).find((cp) => cp.id === "cliffsong_outpost");
assert(cliffsong, "Cliffsong Outpost (canonical secondary-continent CP) must exist.");
assert(cliffsong.continentId === "south", "Cliffsong must be on the south continent.");
const continentDivides = (map.terrainPatches ?? []).filter((p) => p.continentDivide === "south");
assert(continentDivides.length >= 3, "South continent must be water-isolated by at least 3 divide patches.");

// Session 28: Complete canonical 6-class vessel roster + Harbor L2.
const expectedAllVessels = [
  ["fishing_boat", "fishing"],
  ["scout_vessel", "scout"],
  ["war_galley", "war_galley"],
  ["transport_ship", "transport"],
  ["fire_ship", "fire_ship"],
  ["capital_ship", "capital_ship"],
];
expectedAllVessels.forEach(([unitId, vesselClass]) => {
  const u = units.find((x) => x.id === unitId);
  assert(u, `Session 28: canonical vessel ${unitId} must exist.`);
  assert(u.vesselClass === vesselClass, `${unitId} vesselClass must be ${vesselClass}.`);
  assert(u.movementDomain === "water", `${unitId} must have movementDomain water.`);
});
assert(units.find((u) => u.id === "fire_ship")?.oneUseSacrifice === true, "Fire Ship must be one-use sacrifice per canon.");
assert((units.find((u) => u.id === "transport_ship")?.transportCapacity ?? 0) > 0, "Transport Ship must expose transport capacity.");
assert(buildings.find((b) => b.id === "harbor_tier_2"), "Harbor L2 (Great Harbor) must exist.");
assert(buildings.find((b) => b.id === "harbor_tier_2")?.navalTier === 2, "Harbor L2 must be navalTier 2.");
const harbor2Trainables = buildings.find((b) => b.id === "harbor_tier_2")?.trainableUnits ?? [];
assert(harbor2Trainables.includes("fire_ship"), "Harbor L2 must train Fire Ship.");
assert(harbor2Trainables.includes("capital_ship"), "Harbor L2 must train Capital Ship.");
assert(buildings.find((b) => b.id === "wayshrine")?.faithRole === "shrine", "Wayshrine must expose faithRole shrine.");
assert((buildings.find((b) => b.id === "wayshrine")?.faithExposureAmplification ?? 0) > 1, "Wayshrine must amplify faith exposure.");
assert((buildings.find((b) => b.id === "wayshrine")?.faithExposureRadius ?? 0) > 0, "Wayshrine must expose faith exposure radius.");
assert(!(buildings.find((building) => building.id === "barracks")?.trainableUnits ?? []).includes("ram"), "Barracks should no longer train Rams once the Siege Workshop is live.");
assert(buildings.find((building) => building.id === "wall_segment")?.fortificationRole === "wall", "Wall Segment must expose fortification role.");
assert(buildings.find((building) => building.id === "watch_tower")?.fortificationRole === "tower", "Watch Tower must expose fortification role.");
assert(buildings.find((building) => building.id === "gatehouse")?.fortificationRole === "gate", "Gatehouse must expose fortification role.");
assert(buildings.find((building) => building.id === "keep_tier_1")?.fortificationRole === "keep", "Inner Keep must expose fortification role.");
assert(units.find((unit) => unit.id === "ram")?.siegeClass === "ram", "Ram must expose siege class.");
assert(units.find((unit) => unit.id === "siege_tower")?.siegeClass === "siege_tower", "Siege Tower must expose siege class.");
assert((units.find((unit) => unit.id === "siege_tower")?.alliedStructuralSupportMultiplier ?? 0) > 1, "Siege Tower must grant allied assault support.");
assert(units.find((unit) => unit.id === "trebuchet")?.siegeClass === "trebuchet", "Trebuchet must expose siege class.");
assert((units.find((unit) => unit.id === "trebuchet")?.attackRange ?? 0) >= 200, "Trebuchet must expose long-range bombardment.");
assert(units.find((unit) => unit.id === "siege_engineer")?.role === "engineer-specialist", "Siege Engineer must expose engineer-specialist role.");
assert((units.find((unit) => unit.id === "siege_engineer")?.siegeRepairPerSecond ?? 0) > 0, "Siege Engineer must expose siege repair throughput.");
assert(units.find((unit) => unit.id === "supply_wagon")?.role === "support", "Supply Wagon must expose support role.");
assert((units.find((unit) => unit.id === "supply_wagon")?.supplyRadius ?? 0) > 0, "Supply Wagon must expose supply radius.");
assert(units.find((unit) => unit.id === "supply_wagon")?.movingLogisticsCarrier === true, "Session 81: Supply Wagon must expose moving logistics-carrier state.");
assert((units.find((unit) => unit.id === "supply_wagon")?.raidInterdictionSeconds ?? 0) > 0, "Session 81: Supply Wagon must expose raid interdiction timing.");
assert((units.find((unit) => unit.id === "supply_wagon")?.raidLoss?.food ?? 0) > 0, "Session 81: Supply Wagon must expose convoy raid-loss payload.");
assert((units.find((unit) => unit.id === "ram")?.structuralDamageMultiplier ?? 0) > 1, "Ram must carry structural damage bonus.");

// Session 9: Ballista + Mantlet siege-support classes.
assert(units.find((unit) => unit.id === "ballista"), "Ballista must exist in the unit layer.");
assert(units.find((unit) => unit.id === "ballista")?.siegeClass === "ballista", "Ballista must expose siege class.");
assert(units.find((unit) => unit.id === "ballista")?.role === "siege-engine", "Ballista must be siege-engine role.");
assert((units.find((unit) => unit.id === "ballista")?.attackRange ?? 0) >= 120, "Ballista must have meaningful ranged reach.");
assert((units.find((unit) => unit.id === "ballista")?.antiUnitDamageMultiplier ?? 0) >= 1, "Ballista must retain anti-personnel effectiveness.");
assert(buildings.find((building) => building.id === "siege_workshop")?.trainableUnits?.includes("ballista"), "Siege Workshop must train Ballista.");

assert(units.find((unit) => unit.id === "mantlet"), "Mantlet must exist in the unit layer.");
assert(units.find((unit) => unit.id === "mantlet")?.siegeClass === "mantlet", "Mantlet must expose siege class.");
assert(units.find((unit) => unit.id === "mantlet")?.role === "siege-support", "Mantlet must be siege-support role (not an attacking engine).");
assert((units.find((unit) => unit.id === "mantlet")?.coverRadius ?? 0) > 0, "Mantlet must expose cover radius.");
assert((units.find((unit) => unit.id === "mantlet")?.coverInboundRangedMultiplier ?? 1) < 1, "Mantlet must reduce inbound ranged damage.");
assert((units.find((unit) => unit.id === "mantlet")?.attackDamage ?? 0) === 0, "Mantlet must not attack.");
assert(buildings.find((building) => building.id === "siege_workshop")?.trainableUnits?.includes("mantlet"), "Siege Workshop must train Mantlet.");

map.factions.forEach((faction) => {
  faction.buildings.forEach((building) => {
    assert(buildingIds.has(building.typeId), `Map references unknown building type: ${building.typeId}`);
  });
  faction.units.forEach((unit) => {
    assert(unitIds.has(unit.typeId), `Map references unknown unit type: ${unit.typeId}`);
  });
});

(roles ?? []).forEach((role) => {
  if (role.pathAffinity) {
    assert(paths.some((path) => path.id === role.pathAffinity), `Unknown role path affinity: ${role.pathAffinity}`);
  }
});

assert((map.controlPoints ?? []).length >= 3, "Expected at least three frontier control points on the prototype map.");
assert(map.factions.some((faction) => faction.id === "tribes"), "Prototype map should include neutral frontier tribes.");
assert((map.sacredSites ?? []).length === 4, "Prototype map should include one sacred exposure site for each covenant.");
assert((map.settlements ?? []).length >= 2, "Expected primary dynastic keep settlement seeds.");

(map.controlPoints ?? []).forEach((controlPoint) => {
  Object.keys(controlPoint.resourceTrickle ?? {}).forEach((resourceId) => {
    assert(resourceIds.has(resourceId), `Unknown control point resource: ${resourceId}`);
  });
  assert(controlPoint.settlementClass, `Control point ${controlPoint.id} must define settlement class.`);
  assert(settlementClasses.some((klass) => klass.id === controlPoint.settlementClass), `Unknown settlement class on ${controlPoint.id}.`);
});

(map.sacredSites ?? []).forEach((site) => {
  assert(faiths.some((faith) => faith.id === site.faithId), `Unknown sacred site faith: ${site.faithId}`);
});

console.log("Bloodlines data validation passed.");
