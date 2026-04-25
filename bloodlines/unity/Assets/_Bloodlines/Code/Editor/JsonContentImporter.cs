using System;
using System.Collections.Generic;
using System.IO;
using Bloodlines.Components;
using Bloodlines.DataDefinitions;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    public static class JsonContentImporter
    {
        private const string DataRoot = "Assets/_Bloodlines/Data";
        private const string FactionsFolder = DataRoot + "/FactionDefinitions";
        private const string ResourcesFolder = DataRoot + "/ResourceDefinitions";
        private const string UnitsFolder = DataRoot + "/UnitDefinitions";
        private const string BuildingsFolder = DataRoot + "/BuildingDefinitions";
        private const string FaithsFolder = DataRoot + "/FaithDefinitions";
        private const string ConvictionsFolder = DataRoot + "/ConvictionDefinitions";
        private const string RolesFolder = DataRoot + "/BloodlineRoleDefinitions";
        private const string PathsFolder = DataRoot + "/BloodlinePathDefinitions";
        private const string VictoryFolder = DataRoot + "/VictoryConditionDefinitions";
        private const string MapsFolder = DataRoot + "/MapDefinitions";
        private const string SettlementClassesFolder = DataRoot + "/SettlementClassDefinitions";
        private const string RealmConditionsFolder = DataRoot + "/RealmConditionDefinitions";
        private static readonly HashSet<string> BrokenBindingBackupPaths = new(StringComparer.OrdinalIgnoreCase);
        private static readonly List<string> RecreatedAssetPaths = new();
        private static string brokenBindingBackupRoot = string.Empty;

        [MenuItem("Bloodlines/Import/Sync JSON Content")]
        public static void ImportAll()
        {
            var jsonRoot = GetCanonicalJsonRoot();
            if (!Directory.Exists(jsonRoot))
            {
                UnityDebug.LogError("Bloodlines JSON root not found: " + jsonRoot);
                return;
            }

            ResetRepairSession();
            EnsureFolderChain(DataRoot);
            EnsureFolderChain(FactionsFolder);
            EnsureFolderChain(ResourcesFolder);
            EnsureFolderChain(UnitsFolder);
            EnsureFolderChain(BuildingsFolder);
            EnsureFolderChain(FaithsFolder);
            EnsureFolderChain(ConvictionsFolder);
            EnsureFolderChain(RolesFolder);
            EnsureFolderChain(PathsFolder);
            EnsureFolderChain(VictoryFolder);
            EnsureFolderChain(MapsFolder);
            EnsureFolderChain(SettlementClassesFolder);
            EnsureFolderChain(RealmConditionsFolder);

            AssetDatabase.StartAssetEditing();
            try
            {
                ImportHouses(Path.Combine(jsonRoot, "houses.json"));
                ImportResources(Path.Combine(jsonRoot, "resources.json"));
                ImportUnits(Path.Combine(jsonRoot, "units.json"));
                ImportBuildings(Path.Combine(jsonRoot, "buildings.json"));
                ImportFaiths(Path.Combine(jsonRoot, "faiths.json"));
                ImportConvictionStates(Path.Combine(jsonRoot, "conviction-states.json"));
                ImportBloodlineRoles(Path.Combine(jsonRoot, "bloodline-roles.json"));
                ImportBloodlinePaths(Path.Combine(jsonRoot, "bloodline-paths.json"));
                ImportVictoryConditions(Path.Combine(jsonRoot, "victory-conditions.json"));
                ImportSettlementClasses(Path.Combine(jsonRoot, "settlement-classes.json"));
                ImportRealmConditions(Path.Combine(jsonRoot, "realm-conditions.json"));
                ImportMaps(Path.Combine(jsonRoot, "maps", "ironmark-frontier.json"));
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            WriteRepairSummary();
            UnityDebug.Log("Bloodlines JSON content sync complete from " + jsonRoot);
            if (RecreatedAssetPaths.Count > 0)
            {
                UnityDebug.Log(
                    "Bloodlines JSON content sync repaired " + RecreatedAssetPaths.Count +
                    " broken generated definition assets. Backup root: " + brokenBindingBackupRoot);
            }
        }

        private static string GetCanonicalJsonRoot()
        {
            var projectRoot = Directory.GetParent(Application.dataPath);
            if (projectRoot == null)
            {
                throw new InvalidOperationException("Unable to resolve Unity project root.");
            }

            return Path.GetFullPath(Path.Combine(projectRoot.FullName, "..", "data"));
        }

        private static void ImportHouses(string filePath)
        {
            var items = LoadArray<HouseRecord>(filePath);
            foreach (var item in items)
            {
                var asset = LoadOrCreateAsset<HouseDefinition>(FactionsFolder, item.id);
                asset.id = item.id;
                asset.displayName = item.name;
                asset.status = item.status;
                asset.hairColor = item.hairColor;
                asset.primaryColor = item.primaryColor;
                asset.accentColor = item.accentColor;
                asset.symbol = item.symbol;
                asset.societalAdvantage = item.societalAdvantage;
                asset.requiredDisadvantage = item.requiredDisadvantage;
                asset.prototypePlayable = item.prototypePlayable;
                asset.notes = item.notes ?? Array.Empty<string>();
                EditorUtility.SetDirty(asset);
            }
        }

        private static void ImportResources(string filePath)
        {
            var items = LoadArray<ResourceRecord>(filePath);
            foreach (var item in items)
            {
                var asset = LoadOrCreateAsset<ResourceDefinition>(ResourcesFolder, item.id);
                asset.id = item.id;
                asset.displayName = item.name;
                asset.category = item.category;
                asset.enabledInPrototype = item.enabledInPrototype;
                asset.description = item.description;
                EditorUtility.SetDirty(asset);
            }
        }

        private static void ImportUnits(string filePath)
        {
            var items = LoadArray<UnitRecord>(filePath);
            foreach (var item in items)
            {
                var asset = LoadOrCreateAsset<UnitDefinition>(UnitsFolder, item.id);
                asset.id = item.id;
                asset.displayName = item.name;
                asset.stage = item.stage;
                asset.role = item.role;
                asset.siegeClass = item.siegeClass;
                asset.prototypeEnabled = item.prototypeEnabled;
                asset.populationCost = item.populationCost;
                asset.health = item.health;
                asset.speed = item.speed;
                asset.attackDamage = item.attackDamage;
                asset.attackRange = item.attackRange;
                asset.attackCooldown = item.attackCooldown;
                asset.sight = item.sight;
                asset.projectileSpeed = item.projectileSpeed;
                asset.movementDomain = item.movementDomain;
                asset.carryCapacity = item.carryCapacity;
                asset.gatherRate = item.gatherRate;
                asset.buildRate = item.buildRate;
                asset.structuralDamageMultiplier = item.structuralDamageMultiplier;
                asset.antiUnitDamageMultiplier = item.antiUnitDamageMultiplier;
                asset.cost = item.cost ?? new ResourceAmountFields();
                asset.house = item.house;
                asset.faithId = item.faithId;
                asset.doctrinePath = item.doctrinePath;
                asset.ironmarkBloodPrice = item.ironmarkBloodPrice;
                asset.bloodProductionLoadDelta = item.bloodProductionLoadDelta;
                asset.notes = item.notes ?? Array.Empty<string>();
                asset.separationRadius = ResolveUnitSeparationRadius(item.role, item.siegeClass, item.separationRadius);
                EditorUtility.SetDirty(asset);
            }
        }

        private static void ImportBuildings(string filePath)
        {
            var items = LoadArray<BuildingRecord>(filePath);
            foreach (var item in items)
            {
                var asset = LoadOrCreateAsset<BuildingDefinition>(BuildingsFolder, item.id);
                asset.id = item.id;
                asset.displayName = item.name;
                asset.prototypeEnabled = item.prototypeEnabled;
                asset.footprint = item.footprint ?? new Int2Data();
                asset.health = item.health;
                asset.buildTime = item.buildTime;
                asset.populationCapBonus = item.populationCapBonus;
                asset.dropoffResources = item.dropoffResources ?? Array.Empty<string>();
                asset.trainableUnits = item.trainableUnits ?? Array.Empty<string>();
                asset.cost = item.cost ?? new ResourceAmountFields();
                asset.resourceTrickle = item.resourceTrickle ?? new ResourceTrickleFields();
                asset.fortificationRole = item.fortificationRole;
                asset.fortificationTierContribution = item.fortificationTierContribution;
                asset.structuralDamageMultiplier = item.structuralDamageMultiplier;
                asset.armor = item.armor;
                asset.blocksPassage = item.blocksPassage;
                asset.sightBonusRadius = item.sightBonusRadius;
                asset.auraAttackMultiplier = item.auraAttackMultiplier;
                asset.auraRadius = item.auraRadius;
                asset.smeltingFuelResource = item.smeltingFuelResource;
                asset.smeltingFuelRatio = item.smeltingFuelRatio;
                asset.buildTier = item.buildTier;
                asset.maxWorkerSlots = item.maxWorkerSlots;
                asset.workerOutputPerSecond = item.workerOutputPerSecond ?? new ResourceTrickleFields();
                asset.waterPopulationSupport = item.waterPopulationSupport;
                EditorUtility.SetDirty(asset);
            }
        }

        private static void ImportFaiths(string filePath)
        {
            var items = LoadArray<FaithRecord>(filePath);
            foreach (var item in items)
            {
                var asset = LoadOrCreateAsset<FaithDefinition>(FaithsFolder, item.id);
                asset.id = item.id;
                asset.displayName = item.name;
                asset.covenantName = item.covenantName;
                asset.prototypeEnabled = item.prototypeEnabled;
                asset.alignmentPaths = item.alignmentPaths ?? Array.Empty<string>();
                asset.prototypeEffects = item.prototypeEffects ?? new DoctrineEffectSetData();
                EditorUtility.SetDirty(asset);
            }
        }

        private static void ImportConvictionStates(string filePath)
        {
            var items = LoadArray<ConvictionStateRecord>(filePath);
            foreach (var item in items)
            {
                var asset = LoadOrCreateAsset<ConvictionStateDefinition>(ConvictionsFolder, item.id);
                asset.id = item.id;
                asset.label = item.label;
                asset.minScore = item.minScore;
                EditorUtility.SetDirty(asset);
            }
        }

        private static void ImportBloodlineRoles(string filePath)
        {
            var items = LoadArray<BloodlineRoleRecord>(filePath);
            foreach (var item in items)
            {
                var asset = LoadOrCreateAsset<BloodlineRoleDefinition>(RolesFolder, item.id);
                asset.id = item.id;
                asset.displayName = item.name;
                asset.prototypeEnabled = item.prototypeEnabled;
                asset.pathAffinity = item.pathAffinity;
                EditorUtility.SetDirty(asset);
            }
        }

        private static void ImportBloodlinePaths(string filePath)
        {
            var items = LoadArray<BloodlinePathRecord>(filePath);
            foreach (var item in items)
            {
                var asset = LoadOrCreateAsset<BloodlinePathDefinition>(PathsFolder, item.id);
                asset.id = item.id;
                asset.displayName = item.name;
                asset.summary = item.summary;
                EditorUtility.SetDirty(asset);
            }
        }

        private static void ImportVictoryConditions(string filePath)
        {
            var items = LoadArray<VictoryConditionRecord>(filePath);
            foreach (var item in items)
            {
                var asset = LoadOrCreateAsset<VictoryConditionDefinition>(VictoryFolder, item.id);
                asset.id = item.id;
                asset.displayName = item.name;
                asset.status = item.status;
                asset.prototypeEnabled = item.prototypeEnabled;
                EditorUtility.SetDirty(asset);
            }
        }

        private static void ImportSettlementClasses(string filePath)
        {
            if (!File.Exists(filePath))
            {
                UnityDebug.LogWarning("settlement-classes.json not found at " + filePath);
                return;
            }

            var items = LoadArray<SettlementClassRecord>(filePath);
            foreach (var item in items)
            {
                var asset = LoadOrCreateAsset<SettlementClassDefinition>(SettlementClassesFolder, item.id);
                asset.id = item.id;
                asset.displayName = item.name;
                asset.defensiveCeiling = item.defensiveCeiling;
                asset.description = item.description;
                EditorUtility.SetDirty(asset);
            }
        }

        private static void ImportRealmConditions(string filePath)
        {
            if (!File.Exists(filePath))
            {
                UnityDebug.LogWarning("realm-conditions.json not found at " + filePath);
                return;
            }

            var record = LoadObject<RealmConditionRecord>(filePath);
            var asset = LoadOrCreateAsset<RealmConditionDefinition>(RealmConditionsFolder, "realm_conditions");
            asset.cycleSeconds = record.cycleSeconds;
            asset.thresholds = record.thresholds ?? new RealmConditionThresholdsData();
            asset.effects = record.effects ?? new RealmConditionEffectsData();
            asset.legibility = record.legibility ?? new RealmConditionLegibilityData();
            EditorUtility.SetDirty(asset);
        }

        private static void ImportMaps(string filePath)
        {
            var item = LoadObject<MapRecord>(filePath);
            var asset = LoadOrCreateAsset<MapDefinition>(MapsFolder, item.id);
            asset.id = item.id;
            asset.displayName = item.name;
            asset.tileSize = item.tileSize;
            asset.width = item.width;
            asset.height = item.height;
            asset.cameraStart = item.cameraStart ?? new Float2Data();
            asset.terrainPatches = item.terrainPatches ?? Array.Empty<TerrainPatchData>();
            asset.resourceNodes = item.resourceNodes ?? Array.Empty<ResourceNodeData>();
            asset.controlPoints = item.controlPoints ?? Array.Empty<ControlPointData>();
            asset.settlements = item.settlements ?? Array.Empty<SettlementSeedData>();
            asset.sacredSites = item.sacredSites ?? Array.Empty<SacredSiteData>();
            asset.factions = item.factions ?? Array.Empty<FactionSeedData>();
            EditorUtility.SetDirty(asset);
        }

        private static TAsset LoadOrCreateAsset<TAsset>(string folderPath, string fileName)
            where TAsset : ScriptableObject
        {
            var assetPath = folderPath + "/" + SanitizeFileName(fileName) + ".asset";
            var asset = AssetDatabase.LoadAssetAtPath<TAsset>(assetPath);
            if (asset != null)
            {
                return asset;
            }

            var mainAsset = AssetDatabase.LoadMainAssetAtPath(assetPath);
            if (mainAsset is TAsset directMainAsset)
            {
                return directMainAsset;
            }

            var absoluteAssetPath = GetAbsoluteAssetPath(assetPath);
            if (File.Exists(absoluteAssetPath))
            {
                asset = RecreateBrokenGeneratedAsset<TAsset>(assetPath, absoluteAssetPath);
                if (asset != null)
                {
                    return asset;
                }

                throw new InvalidOperationException(
                    "Unable to repair generated Bloodlines definition asset at " + assetPath);
            }

            asset = ScriptableObject.CreateInstance<TAsset>();
            AssetDatabase.CreateAsset(asset, assetPath);
            return asset;
        }

        private static void EnsureFolderChain(string assetFolderPath)
        {
            if (AssetDatabase.IsValidFolder(assetFolderPath))
            {
                return;
            }

            var parts = assetFolderPath.Split('/');
            var current = parts[0];
            for (var i = 1; i < parts.Length; i++)
            {
                var next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[i]);
                }

                current = next;
            }
        }

        private static T[] LoadArray<T>(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return JsonArrayUtility.FromJson<T>(json);
        }

        private static T LoadObject<T>(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<T>(json);
        }

        private static string SanitizeFileName(string value)
        {
            foreach (var invalidChar in Path.GetInvalidFileNameChars())
            {
                value = value.Replace(invalidChar, '_');
            }

            return value;
        }

        private static void ResetRepairSession()
        {
            BrokenBindingBackupPaths.Clear();
            RecreatedAssetPaths.Clear();
            brokenBindingBackupRoot = string.Empty;
        }

        private static TAsset RecreateBrokenGeneratedAsset<TAsset>(string assetPath, string absoluteAssetPath)
            where TAsset : ScriptableObject
        {
            BackupBrokenGeneratedAsset(assetPath, absoluteAssetPath);

            if (File.Exists(absoluteAssetPath))
            {
                File.Delete(absoluteAssetPath);
            }

            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);

            var recreatedAsset = ScriptableObject.CreateInstance<TAsset>();
            AssetDatabase.CreateAsset(recreatedAsset, assetPath);
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
            RecreatedAssetPaths.Add(assetPath);
            return recreatedAsset;
        }

        private static void BackupBrokenGeneratedAsset(string assetPath, string absoluteAssetPath)
        {
            if (!BrokenBindingBackupPaths.Add(assetPath))
            {
                return;
            }

            var backupRoot = GetBrokenBindingBackupRoot();
            var relativeAssetPath = assetPath.Replace('/', Path.DirectorySeparatorChar);
            var backupAssetPath = Path.Combine(backupRoot, relativeAssetPath);
            var backupDirectory = Path.GetDirectoryName(backupAssetPath);
            if (!string.IsNullOrEmpty(backupDirectory))
            {
                Directory.CreateDirectory(backupDirectory);
            }

            File.Copy(absoluteAssetPath, backupAssetPath, true);

            var absoluteMetaPath = absoluteAssetPath + ".meta";
            if (File.Exists(absoluteMetaPath))
            {
                File.Copy(absoluteMetaPath, backupAssetPath + ".meta", true);
            }
        }

        private static string GetBrokenBindingBackupRoot()
        {
            if (!string.IsNullOrEmpty(brokenBindingBackupRoot))
            {
                return brokenBindingBackupRoot;
            }

            var canonicalRoot = GetCanonicalBloodlinesRoot();
            var stamp = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
            brokenBindingBackupRoot = Path.Combine(
                canonicalRoot,
                "reports",
                "unity-definition-binding-repair",
                stamp);
            Directory.CreateDirectory(brokenBindingBackupRoot);
            return brokenBindingBackupRoot;
        }

        private static string GetCanonicalBloodlinesRoot()
        {
            var projectRoot = Directory.GetParent(Application.dataPath);
            if (projectRoot == null)
            {
                throw new InvalidOperationException("Unable to resolve Unity project root.");
            }

            var canonicalRoot = Directory.GetParent(projectRoot.FullName);
            if (canonicalRoot == null)
            {
                throw new InvalidOperationException("Unable to resolve Bloodlines canonical root.");
            }

            return canonicalRoot.FullName;
        }

        private static string GetAbsoluteAssetPath(string assetPath)
        {
            var projectRoot = Directory.GetParent(Application.dataPath);
            if (projectRoot == null)
            {
                throw new InvalidOperationException("Unable to resolve Unity project root.");
            }

            return Path.GetFullPath(Path.Combine(projectRoot.FullName, assetPath.Replace('/', Path.DirectorySeparatorChar)));
        }

        private static void WriteRepairSummary()
        {
            if (RecreatedAssetPaths.Count == 0)
            {
                return;
            }

            var backupRoot = GetBrokenBindingBackupRoot();
            var summaryPath = Path.Combine(backupRoot, "REPAIR_SUMMARY.txt");
            using var writer = new StreamWriter(summaryPath, false);
            writer.WriteLine("Bloodlines Unity definition binding repair");
            writer.WriteLine("Generated: " + DateTime.Now.ToString("O"));
            writer.WriteLine("Recreated generated assets: " + RecreatedAssetPaths.Count);
            writer.WriteLine();
            foreach (var assetPath in RecreatedAssetPaths)
            {
                writer.WriteLine(assetPath);
            }
        }

        [Serializable]
        private class HouseRecord
        {
            public string id;
            public string name;
            public string status;
            public string hairColor;
            public string primaryColor;
            public string accentColor;
            public string symbol;
            public string societalAdvantage;
            public string requiredDisadvantage;
            public bool prototypePlayable;
            public string[] notes;
        }

        [Serializable]
        private class ResourceRecord
        {
            public string id;
            public string name;
            public string category;
            public bool enabledInPrototype;
            public string description;
        }

        [Serializable]
        private class UnitRecord
        {
            public string id;
            public string name;
            public int stage;
            public string role;
            public string siegeClass;
            public bool prototypeEnabled;
            public int populationCost;
            public float health;
            public float speed;
            public float attackDamage;
            public float attackRange;
            public float attackCooldown;
            public float sight;
            public float projectileSpeed;
            public string movementDomain;
            public float carryCapacity;
            public float gatherRate;
            public float buildRate;
            public float structuralDamageMultiplier;
            public float antiUnitDamageMultiplier;
            public ResourceAmountFields cost;
            public string house;
            public string faithId;
            public string doctrinePath;
            public int ironmarkBloodPrice;
            public float bloodProductionLoadDelta;
            public string[] notes;
            public float separationRadius;
        }

        [Serializable]
        private class BuildingRecord
        {
            public string id;
            public string name;
            public bool prototypeEnabled;
            public Int2Data footprint;
            public float health;
            public float buildTime;
            public int populationCapBonus;
            public string[] dropoffResources;
            public string[] trainableUnits;
            public ResourceAmountFields cost;
            public ResourceTrickleFields resourceTrickle;
            public string fortificationRole;
            public int fortificationTierContribution;
            public float structuralDamageMultiplier;
            public float armor;
            public bool blocksPassage;
            public float sightBonusRadius;
            public float auraAttackMultiplier;
            public float auraRadius;
            public string smeltingFuelResource;
            public float smeltingFuelRatio;
            public int buildTier;
            public int maxWorkerSlots;
            public ResourceTrickleFields workerOutputPerSecond;
            public int waterPopulationSupport;
        }

        [Serializable]
        private class FaithRecord
        {
            public string id;
            public string name;
            public string covenantName;
            public bool prototypeEnabled;
            public string[] alignmentPaths;
            public DoctrineEffectSetData prototypeEffects;
        }

        [Serializable]
        private class ConvictionStateRecord
        {
            public string id;
            public string label;
            public int minScore;
        }

        [Serializable]
        private class BloodlineRoleRecord
        {
            public string id;
            public string name;
            public bool prototypeEnabled;
            public string pathAffinity;
        }

        [Serializable]
        private class BloodlinePathRecord
        {
            public string id;
            public string name;
            public string summary;
        }

        [Serializable]
        private class VictoryConditionRecord
        {
            public string id;
            public string name;
            public string status;
            public bool prototypeEnabled;
        }

        [Serializable]
        private class SettlementClassRecord
        {
            public string id;
            public string name;
            public int defensiveCeiling;
            public string description;
        }

        [Serializable]
        private class RealmConditionRecord
        {
            public float cycleSeconds;
            public RealmConditionThresholdsData thresholds;
            public RealmConditionEffectsData effects;
            public RealmConditionLegibilityData legibility;
        }

        [Serializable]
        private class MapRecord
        {
            public string id;
            public string name;
            public int tileSize;
            public int width;
            public int height;
            public Float2Data cameraStart;
            public TerrainPatchData[] terrainPatches;
            public ResourceNodeData[] resourceNodes;
            public ControlPointData[] controlPoints;
            public SettlementSeedData[] settlements;
            public SacredSiteData[] sacredSites;
            public FactionSeedData[] factions;
        }

        private static class JsonArrayUtility
        {
            [Serializable]
            private class Wrapper<T>
            {
                public T[] items;
            }

            public static T[] FromJson<T>(string json)
            {
                var wrapped = "{\"items\":" + json + "}";
                var result = JsonUtility.FromJson<Wrapper<T>>(wrapped);
                return result != null && result.items != null ? result.items : Array.Empty<T>();
            }
        }

        private static float ResolveUnitSeparationRadius(string role, string siegeClass, float authoredRadius)
        {
            return CombatUnitRuntimeDefaults.ResolveSeparationRadius(
                ResolveUnitRole(role),
                ResolveSiegeClass(siegeClass),
                authoredRadius);
        }

        private static UnitRole ResolveUnitRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                return UnitRole.Unknown;
            }

            if (string.Equals(role, "worker", StringComparison.OrdinalIgnoreCase))
            {
                return UnitRole.Worker;
            }

            if (string.Equals(role, "melee", StringComparison.OrdinalIgnoreCase))
            {
                return UnitRole.Melee;
            }

            if (string.Equals(role, "melee-recon", StringComparison.OrdinalIgnoreCase))
            {
                return UnitRole.MeleeRecon;
            }

            if (string.Equals(role, "ranged", StringComparison.OrdinalIgnoreCase))
            {
                return UnitRole.Ranged;
            }

            if (string.Equals(role, "unique-melee", StringComparison.OrdinalIgnoreCase))
            {
                return UnitRole.UniqueMelee;
            }

            if (string.Equals(role, "light-cavalry", StringComparison.OrdinalIgnoreCase))
            {
                return UnitRole.LightCavalry;
            }

            if (string.Equals(role, "siege-engine", StringComparison.OrdinalIgnoreCase))
            {
                return UnitRole.SiegeEngine;
            }

            if (string.Equals(role, "siege-support", StringComparison.OrdinalIgnoreCase))
            {
                return UnitRole.SiegeSupport;
            }

            if (string.Equals(role, "engineer-specialist", StringComparison.OrdinalIgnoreCase))
            {
                return UnitRole.EngineerSpecialist;
            }

            if (string.Equals(role, "support", StringComparison.OrdinalIgnoreCase))
            {
                return UnitRole.Support;
            }

            return UnitRole.Unknown;
        }

        private static SiegeClass ResolveSiegeClass(string siegeClass)
        {
            if (string.IsNullOrWhiteSpace(siegeClass))
            {
                return SiegeClass.None;
            }

            if (string.Equals(siegeClass, "ram", StringComparison.OrdinalIgnoreCase))
            {
                return SiegeClass.Ram;
            }

            if (string.Equals(siegeClass, "siege-tower", StringComparison.OrdinalIgnoreCase))
            {
                return SiegeClass.SiegeTower;
            }

            if (string.Equals(siegeClass, "trebuchet", StringComparison.OrdinalIgnoreCase))
            {
                return SiegeClass.Trebuchet;
            }

            if (string.Equals(siegeClass, "ballista", StringComparison.OrdinalIgnoreCase))
            {
                return SiegeClass.Ballista;
            }

            if (string.Equals(siegeClass, "mantlet", StringComparison.OrdinalIgnoreCase))
            {
                return SiegeClass.Mantlet;
            }

            return SiegeClass.None;
        }
    }
}
