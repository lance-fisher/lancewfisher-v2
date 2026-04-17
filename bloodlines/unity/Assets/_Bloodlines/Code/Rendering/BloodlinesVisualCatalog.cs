using System.Collections.Generic;
using Bloodlines.DataDefinitions;
using UnityEngine;

namespace Bloodlines.Rendering
{
    /// <summary>
    /// Runtime lookup registry for UnitVisualDefinition and
    /// BuildingVisualDefinition assets. Populated from either
    /// AssetDatabase (editor / batch mode) or Resources (build),
    /// both via a single entry point so gameplay code never has
    /// to know which loader is active.
    /// </summary>
    public static class BloodlinesVisualCatalog
    {
        private static readonly Dictionary<string, UnitVisualDefinition> s_units =
            new Dictionary<string, UnitVisualDefinition>(System.StringComparer.OrdinalIgnoreCase);

        private static readonly Dictionary<string, BuildingVisualDefinition> s_buildings =
            new Dictionary<string, BuildingVisualDefinition>(System.StringComparer.OrdinalIgnoreCase);

        private static bool s_initialized;

        public static bool TryGetUnitVisual(string id, out UnitVisualDefinition definition)
        {
            EnsureInitialized();
            if (string.IsNullOrWhiteSpace(id))
            {
                definition = null;
                return false;
            }

            return s_units.TryGetValue(id, out definition) && definition != null;
        }

        public static bool TryGetBuildingVisual(string id, out BuildingVisualDefinition definition)
        {
            EnsureInitialized();
            if (string.IsNullOrWhiteSpace(id))
            {
                definition = null;
                return false;
            }

            return s_buildings.TryGetValue(id, out definition) && definition != null;
        }

        public static int UnitCount
        {
            get
            {
                EnsureInitialized();
                return s_units.Count;
            }
        }

        public static int BuildingCount
        {
            get
            {
                EnsureInitialized();
                return s_buildings.Count;
            }
        }

        public static void ForceRefresh()
        {
            s_initialized = false;
            s_units.Clear();
            s_buildings.Clear();
            EnsureInitialized();
        }

        private static void EnsureInitialized()
        {
            if (s_initialized)
            {
                return;
            }

            LoadAll();
            s_initialized = true;
        }

        private static void LoadAll()
        {
#if UNITY_EDITOR
            // Editor + batchmode: discover via AssetDatabase so the catalog
            // always sees the current set of placeholder definitions.
            foreach (var guid in UnityEditor.AssetDatabase.FindAssets("t:UnitVisualDefinition"))
            {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<UnitVisualDefinition>(path);
                if (asset != null && !string.IsNullOrWhiteSpace(asset.id))
                {
                    s_units[asset.id] = asset;
                }
            }

            foreach (var guid in UnityEditor.AssetDatabase.FindAssets("t:BuildingVisualDefinition"))
            {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<BuildingVisualDefinition>(path);
                if (asset != null && !string.IsNullOrWhiteSpace(asset.id))
                {
                    s_buildings[asset.id] = asset;
                }
            }
#else
            // Player / shipping: require a Resources/BloodlinesVisualBundle at
            // build time that bundles the placeholder and production visual
            // definitions. When the build pipeline lands this gets a loader.
#endif
        }
    }
}
