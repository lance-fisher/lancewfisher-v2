const DATA_FILES = {
  houses: "../../../data/houses.json",
  resources: "../../../data/resources.json",
  units: "../../../data/units.json",
  buildings: "../../../data/buildings.json",
  faiths: "../../../data/faiths.json",
  convictionStates: "../../../data/conviction-states.json",
  bloodlineRoles: "../../../data/bloodline-roles.json",
  bloodlinePaths: "../../../data/bloodline-paths.json",
  victoryConditions: "../../../data/victory-conditions.json",
  map: "../../../data/maps/ironmark-frontier.json",
};

function indexById(items) {
  return Object.fromEntries(items.map((item) => [item.id, item]));
}

export async function loadGameContent() {
  const entries = await Promise.all(
    Object.entries(DATA_FILES).map(async ([key, path]) => {
      const url = new URL(path, import.meta.url);
      const response = await fetch(url);
      if (!response.ok) {
        throw new Error(`Failed to load ${url.pathname}: HTTP ${response.status}`);
      }
      return [key, await response.json()];
    }),
  );

  const content = Object.fromEntries(entries);
  content.byId = {
    houses: indexById(content.houses),
    units: indexById(content.units),
    buildings: indexById(content.buildings),
    resources: indexById(content.resources),
    faiths: indexById(content.faiths),
    convictionStates: indexById(content.convictionStates),
    bloodlineRoles: indexById(content.bloodlineRoles),
    bloodlinePaths: indexById(content.bloodlinePaths),
    victoryConditions: indexById(content.victoryConditions),
  };

  return content;
}
