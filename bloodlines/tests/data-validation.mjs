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

const [houses, resources, units, buildings, faiths, roles, paths, victoryConditions, map] = await Promise.all([
  readJson("data/houses.json"),
  readJson("data/resources.json"),
  readJson("data/units.json"),
  readJson("data/buildings.json"),
  readJson("data/faiths.json"),
  readJson("data/bloodline-roles.json"),
  readJson("data/bloodline-paths.json"),
  readJson("data/victory-conditions.json"),
  readJson("data/maps/ironmark-frontier.json"),
]);

assert(houses.length === 9, "Expected 9 canonical houses.");
assert(houses.some((house) => house.id === "ironmark" && house.prototypePlayable), "Ironmark must be the playable prototype house.");
assert(resources.some((resource) => resource.id === "iron"), "Iron must exist in the resource layer.");
assert(resources.some((resource) => resource.id === "influence"), "Influence must exist in the resource layer.");
assert(faiths.length === 4, "Expected all four covenants.");
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
  (building.trainableUnits ?? []).forEach((unitId) => {
    assert(unitIds.has(unitId), `Unknown trainable unit: ${unitId}`);
  });
});

units.forEach((unit) => {
  Object.keys(unit.cost ?? {}).forEach((resourceId) => {
    assert(resourceIds.has(resourceId), `Unknown unit cost resource: ${resourceId}`);
  });
});

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

(map.controlPoints ?? []).forEach((controlPoint) => {
  Object.keys(controlPoint.resourceTrickle ?? {}).forEach((resourceId) => {
    assert(resourceIds.has(resourceId), `Unknown control point resource: ${resourceId}`);
  });
});

(map.sacredSites ?? []).forEach((site) => {
  assert(faiths.some((faith) => faith.id === site.faithId), `Unknown sacred site faith: ${site.faithId}`);
});

console.log("Bloodlines data validation passed.");
