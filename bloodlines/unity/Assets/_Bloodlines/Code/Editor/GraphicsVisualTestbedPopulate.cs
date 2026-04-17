using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    public static class GraphicsVisualTestbedPopulate
    {
        private const string GeneratedRootName = "GENERATED_TESTBED_CONTENT";
        private const string MaterialRoot = "Assets/_Bloodlines/Materials/Staging/Testbeds";
        private const string SharedMaterialRoot = MaterialRoot + "/Shared";
        private const string DynastyMaterialRoot = MaterialRoot + "/Dynasties";
        private const string TerrainMaterialRoot = MaterialRoot + "/Terrain";
        private const string BoardMaterialRoot = MaterialRoot + "/Boards";

        private const string VisualReadabilityScenePath = "Assets/_Bloodlines/Scenes/Testbeds/VisualReadability/VisualReadability_Testbed.unity";
        private const string TerrainLookdevScenePath = "Assets/_Bloodlines/Scenes/Testbeds/TerrainLookdev/TerrainLookdev_Testbed.unity";
        private const string MaterialLookdevScenePath = "Assets/_Bloodlines/Scenes/Testbeds/MaterialLookdev/MaterialLookdev_Testbed.unity";
        private const string IconLegibilityScenePath = "Assets/_Bloodlines/Scenes/Testbeds/IconLegibility/IconLegibility_Testbed.unity";

        private static readonly string[] VisualHouseIds =
        {
            "trueborn",
            "highborne",
            "ironmark",
            "goldgrave",
            "stonehelm",
            "westland",
            "hartvale",
            "whitehall",
            "oldcrest"
        };

        [MenuItem("Bloodlines/Graphics/Populate Visual Testbed Scenes")]
        public static void PopulateVisualTestbedScenes()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            PopulateVisualTestbedScenesInternal();
        }

        public static void RunBatchPopulateVisualTestbedScenes()
        {
            PopulateVisualTestbedScenesInternal();
        }

        private static void PopulateVisualTestbedScenesInternal()
        {
            EnsureFolderChain(SharedMaterialRoot);
            EnsureFolderChain(DynastyMaterialRoot);
            EnsureFolderChain(TerrainMaterialRoot);
            EnsureFolderChain(BoardMaterialRoot);

            var houses = LoadHousePaletteMap();
            var sharedStone = GetOrCreateColorMaterial(SharedMaterialRoot + "/bl_testbed_shared_stone_placeholder_v001.mat", ParseHex("#8e8577"), 0.05f, 0.18f);
            var sharedTimber = GetOrCreateColorMaterial(SharedMaterialRoot + "/bl_testbed_shared_timber_placeholder_v001.mat", ParseHex("#7a5a36"), 0.02f, 0.12f);
            var sharedIron = GetOrCreateColorMaterial(SharedMaterialRoot + "/bl_testbed_shared_iron_placeholder_v001.mat", ParseHex("#5f646a"), 0.22f, 0.18f);
            var terrainGrass = GetOrCreateColorMaterial(TerrainMaterialRoot + "/bl_testbed_terrain_grass_placeholder_v001.mat", ParseHex("#778557"), 0.0f, 0.04f);
            var terrainMud = GetOrCreateColorMaterial(TerrainMaterialRoot + "/bl_testbed_terrain_mud_placeholder_v001.mat", ParseHex("#6c5642"), 0.0f, 0.03f);
            var terrainRoad = GetOrCreateColorMaterial(TerrainMaterialRoot + "/bl_testbed_terrain_road_placeholder_v001.mat", ParseHex("#85745d"), 0.0f, 0.02f);
            var terrainRiverbank = GetOrCreateColorMaterial(TerrainMaterialRoot + "/bl_testbed_terrain_riverbank_placeholder_v001.mat", ParseHex("#6f6859"), 0.0f, 0.04f);
            var terrainWater = GetOrCreateColorMaterial(TerrainMaterialRoot + "/bl_testbed_terrain_water_placeholder_v001.mat", ParseHex("#5c7b8c"), 0.0f, 0.05f);

            PopulateVisualReadabilityScene(houses, sharedStone, sharedTimber, sharedIron);
            PopulateTerrainLookdevScene(terrainGrass, terrainMud, terrainRoad, terrainRiverbank, terrainWater, sharedStone, sharedTimber);
            PopulateMaterialLookdevScene(houses, sharedStone, sharedTimber, sharedIron, terrainGrass, terrainMud, terrainRoad, terrainRiverbank);
            PopulateIconLegibilityScene();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            UnityDebug.Log("Bloodlines visual testbed population complete.");
        }

        private static void PopulateVisualReadabilityScene(Dictionary<string, HousePalette> houses, Material sharedStone, Material sharedTimber, Material sharedIron)
        {
            var scene = EditorSceneManager.OpenScene(VisualReadabilityScenePath, OpenSceneMode.Single);
            var generatedRoot = ResetGeneratedRoot();

            CreateTextLabel("VISUAL READABILITY", new Vector3(-14f, 0.1f, 17f), generatedRoot.transform, 1.1f, TextAnchor.MiddleLeft);
            CreateTextLabel("House and class placeholder lane", new Vector3(-14f, 0.1f, 15.6f), generatedRoot.transform, 0.55f, TextAnchor.MiddleLeft);
            CreateTextLabel("Panel order: Core Infantry / Ranged / Cavalry / Worker / Command", new Vector3(-14f, 0.1f, 14.1f), generatedRoot.transform, 0.45f, TextAnchor.MiddleLeft);

            for (var index = 0; index < VisualHouseIds.Length; index++)
            {
                var houseId = VisualHouseIds[index];
                if (!houses.TryGetValue(houseId, out var palette))
                {
                    continue;
                }

                var houseRoot = new GameObject("HOUSE_" + palette.Name.ToUpperInvariant());
                houseRoot.transform.SetParent(generatedRoot.transform, false);
                houseRoot.transform.position = GetGridAnchor(index, 3, new Vector3(-18f, 0f, 10f), 18f, 14f);

                CreateTextLabel(palette.Name, houseRoot.transform.position + new Vector3(0f, 0.1f, 5.8f), generatedRoot.transform, 0.55f, TextAnchor.MiddleCenter);

                var primaryMaterial = GetOrCreateColorMaterial(
                    DynastyMaterialRoot + "/bl_testbed_" + houseId + "_primary_placeholder_v001.mat",
                    palette.PrimaryColor,
                    0.03f,
                    0.12f);
                var accentMaterial = GetOrCreateColorMaterial(
                    DynastyMaterialRoot + "/bl_testbed_" + houseId + "_accent_placeholder_v001.mat",
                    palette.AccentColor,
                    0.02f,
                    0.10f);

                CreateInfantryMarker(houseRoot.transform.position + new Vector3(0f, 0.8f, 3.4f), primaryMaterial, accentMaterial, sharedIron, generatedRoot.transform);
                CreateRangedMarker(houseRoot.transform.position + new Vector3(0f, 0.75f, 1.7f), primaryMaterial, accentMaterial, sharedTimber, generatedRoot.transform);
                CreateCavalryMarker(houseRoot.transform.position + new Vector3(0f, 0.8f, 0f), primaryMaterial, accentMaterial, sharedTimber, sharedIron, generatedRoot.transform);
                CreateWorkerMarker(houseRoot.transform.position + new Vector3(0f, 0.7f, -1.7f), primaryMaterial, accentMaterial, sharedTimber, generatedRoot.transform);
                CreateCommandMarker(houseRoot.transform.position + new Vector3(0f, 0.9f, -3.4f), primaryMaterial, accentMaterial, sharedIron, generatedRoot.transform);
            }

            var boardRoot = new GameObject("REFERENCE_BOARDS");
            boardRoot.transform.SetParent(generatedRoot.transform, false);
            boardRoot.transform.position = new Vector3(34f, 0f, 0f);

            CreateBoardDisplay("SharedRosterBoard", "Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/bl_unit_shared_live_roster_sheet_a_first_pass_concept_v001.png", new Vector3(34f, 5f, 15f), new Vector2(6.4f, 3.8f), boardRoot.transform);
            for (var index = 0; index < VisualHouseIds.Length; index++)
            {
                var houseId = VisualHouseIds[index];
                var textureAssetPath = GetHouseBoardTextureAssetPath(houseId);
                if (string.IsNullOrEmpty(textureAssetPath))
                {
                    continue;
                }

                CreateBoardDisplay(
                    "HouseBoard_" + houseId,
                    textureAssetPath,
                    GetGridAnchor(index, 3, new Vector3(34f, 5f, 8f), 8.5f, 8f),
                    new Vector2(6.1f, 3.6f),
                    boardRoot.transform);
            }

            CreateTextLabel("Support Structure Board Wall", new Vector3(67f, 0.1f, 18.4f), generatedRoot.transform, 0.45f, TextAnchor.MiddleCenter);
            CreateBoardDisplay("MarketStorehouseGranaryBoard", "Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/bl_building_shared_market_storehouse_and_granary_sheet_a_first_pass_concept_v001.png", new Vector3(59f, 5f, 15f), new Vector2(6.2f, 3.6f), boardRoot.transform);
            CreateBoardDisplay("HousingTiersBoard", "Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/bl_building_shared_housing_tiers_sheet_a_first_pass_concept_v001.png", new Vector3(66.5f, 5f, 15f), new Vector2(6.2f, 3.6f), boardRoot.transform);
            CreateBoardDisplay("WellAndWaterSupportBoard", "Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/bl_building_shared_well_and_water_support_sheet_a_first_pass_concept_v001.png", new Vector3(74f, 5f, 15f), new Vector2(6.2f, 3.6f), boardRoot.transform);
            CreateBoardDisplay("DockFerryLandingBoard", "Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/bl_environment_shared_dock_ferry_and_landing_sheet_a_first_pass_concept_v001.png", new Vector3(62.8f, 5f, 7f), new Vector2(6.2f, 3.6f), boardRoot.transform);
            CreateBoardDisplay("CovenantSiteProgressionBoard", "Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/bl_building_shared_covenant_site_progression_sheet_a_first_pass_concept_v001.png", new Vector3(70.3f, 5f, 7f), new Vector2(6.2f, 3.6f), boardRoot.transform);

            EditorSceneManager.SaveScene(scene);
        }

        private static void PopulateTerrainLookdevScene(Material terrainGrass, Material terrainMud, Material terrainRoad, Material terrainRiverbank, Material terrainWater, Material sharedStone, Material sharedTimber)
        {
            var scene = EditorSceneManager.OpenScene(TerrainLookdevScenePath, OpenSceneMode.Single);
            var generatedRoot = ResetGeneratedRoot();

            CreateTextLabel("TERRAIN LOOKDEV", new Vector3(-14f, 0.1f, 17f), generatedRoot.transform, 1.1f, TextAnchor.MiddleLeft);
            CreateTextLabel("Surface, transition, and edge readability lane", new Vector3(-14f, 0.1f, 15.6f), generatedRoot.transform, 0.55f, TextAnchor.MiddleLeft);

            CreateTerrainStrip("GrassStrip", terrainGrass, new Vector3(-10f, 0.01f, 8f), new Vector3(6f, 1f, 3f), generatedRoot.transform);
            CreateTerrainStrip("MudStrip", terrainMud, new Vector3(-3f, 0.01f, 8f), new Vector3(6f, 1f, 3f), generatedRoot.transform);
            CreateTerrainStrip("RoadStrip", terrainRoad, new Vector3(4f, 0.01f, 8f), new Vector3(6f, 1f, 3f), generatedRoot.transform);
            CreateTerrainStrip("RiverbankStrip", terrainRiverbank, new Vector3(11f, 0.01f, 8f), new Vector3(6f, 1f, 3f), generatedRoot.transform);
            CreateTextLabel("Grass", new Vector3(-10f, 0.1f, 10.2f), generatedRoot.transform, 0.55f, TextAnchor.MiddleCenter);
            CreateTextLabel("Mud", new Vector3(-3f, 0.1f, 10.2f), generatedRoot.transform, 0.55f, TextAnchor.MiddleCenter);
            CreateTextLabel("Road", new Vector3(4f, 0.1f, 10.2f), generatedRoot.transform, 0.55f, TextAnchor.MiddleCenter);
            CreateTextLabel("Riverbank", new Vector3(11f, 0.1f, 10.2f), generatedRoot.transform, 0.55f, TextAnchor.MiddleCenter);

            CreateTerrainStrip("WaterBand", terrainWater, new Vector3(-8f, 0.01f, -1f), new Vector3(8f, 1f, 3f), generatedRoot.transform);
            CreateTerrainStrip("RoadEdge", terrainRoad, new Vector3(1f, 0.01f, -1f), new Vector3(6f, 1f, 2.4f), generatedRoot.transform);
            CreateTerrainStrip("MudEdge", terrainMud, new Vector3(7.2f, 0.01f, -1f), new Vector3(3.5f, 1f, 2.4f), generatedRoot.transform);
            CreateCliffRun(sharedStone, new Vector3(9.5f, 0f, -7f), generatedRoot.transform);
            CreateBridgeMarker(sharedTimber, sharedStone, new Vector3(-6.5f, 0.35f, -7.5f), generatedRoot.transform);

            CreateTextLabel("River and ford lane", new Vector3(-8f, 0.1f, 1.2f), generatedRoot.transform, 0.55f, TextAnchor.MiddleCenter);
            CreateTextLabel("Road and edge blend", new Vector3(3.8f, 0.1f, 1.2f), generatedRoot.transform, 0.55f, TextAnchor.MiddleCenter);
            CreateTextLabel("Cliff readability", new Vector3(10.8f, 0.1f, -3.6f), generatedRoot.transform, 0.55f, TextAnchor.MiddleCenter);

            var boardRoot = new GameObject("REFERENCE_BOARDS");
            boardRoot.transform.SetParent(generatedRoot.transform, false);
            CreateBoardDisplay("BiomeBoard", "Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/bl_terrain_shared_biomes_sheet_a_first_pass_concept_v001.png", new Vector3(19f, 4.8f, -4f), new Vector2(7.4f, 4.2f), boardRoot.transform);
            CreateBoardDisplay("TransitionBoard", "Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/bl_terrain_shared_cliff_and_shoreline_transition_sheet_a_first_pass_concept_v001.png", new Vector3(19f, 4.8f, 2f), new Vector2(7.4f, 4.2f), boardRoot.transform);
            CreateBoardDisplay("ResourceGroundBoard", "Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/bl_terrain_shared_resource_ground_and_edge_blend_sheet_a_first_pass_concept_v001.png", new Vector3(27f, 4.8f, -1f), new Vector2(7.4f, 4.2f), boardRoot.transform);
            CreateBoardDisplay("WaterInfrastructureBoard", "Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/bl_environment_shared_bridge_and_water_infrastructure_sheet_a_first_pass_concept_v001.png", new Vector3(27f, 4.8f, 5f), new Vector2(7.4f, 4.2f), boardRoot.transform);

            EditorSceneManager.SaveScene(scene);
        }

        private static void PopulateMaterialLookdevScene(Dictionary<string, HousePalette> houses, Material sharedStone, Material sharedTimber, Material sharedIron, Material terrainGrass, Material terrainMud, Material terrainRoad, Material terrainRiverbank)
        {
            var scene = EditorSceneManager.OpenScene(MaterialLookdevScenePath, OpenSceneMode.Single);
            var generatedRoot = ResetGeneratedRoot();

            CreateTextLabel("MATERIAL LOOKDEV", new Vector3(-14f, 0.1f, 17f), generatedRoot.transform, 1.1f, TextAnchor.MiddleLeft);
            CreateTextLabel("Shared and House value control lane", new Vector3(-14f, 0.1f, 15.6f), generatedRoot.transform, 0.55f, TextAnchor.MiddleLeft);

            var swatchStartX = -10f;
            var spacing = 4.2f;
            var sharedRowZ = 7.5f;

            CreateMaterialSwatch("SharedStone", sharedStone, new Vector3(swatchStartX, 0.9f, sharedRowZ), generatedRoot.transform);
            CreateMaterialSwatch("SharedTimber", sharedTimber, new Vector3(swatchStartX + spacing, 0.9f, sharedRowZ), generatedRoot.transform);
            CreateMaterialSwatch("SharedIron", sharedIron, new Vector3(swatchStartX + spacing * 2f, 0.9f, sharedRowZ), generatedRoot.transform);
            CreateMaterialSwatch("TerrainGrass", terrainGrass, new Vector3(swatchStartX + spacing * 3f, 0.9f, sharedRowZ), generatedRoot.transform);
            CreateMaterialSwatch("TerrainMud", terrainMud, new Vector3(swatchStartX + spacing * 4f, 0.9f, sharedRowZ), generatedRoot.transform);
            CreateMaterialSwatch("TerrainRoad", terrainRoad, new Vector3(swatchStartX + spacing * 5f, 0.9f, sharedRowZ), generatedRoot.transform);
            CreateMaterialSwatch("Riverbank", terrainRiverbank, new Vector3(swatchStartX + spacing * 6f, 0.9f, sharedRowZ), generatedRoot.transform);

            CreateTextLabel("Shared stone", new Vector3(swatchStartX, 0.1f, sharedRowZ - 2.3f), generatedRoot.transform, 0.4f, TextAnchor.MiddleCenter);
            CreateTextLabel("Shared timber", new Vector3(swatchStartX + spacing, 0.1f, sharedRowZ - 2.3f), generatedRoot.transform, 0.4f, TextAnchor.MiddleCenter);
            CreateTextLabel("Shared iron", new Vector3(swatchStartX + spacing * 2f, 0.1f, sharedRowZ - 2.3f), generatedRoot.transform, 0.4f, TextAnchor.MiddleCenter);
            CreateTextLabel("Grass", new Vector3(swatchStartX + spacing * 3f, 0.1f, sharedRowZ - 2.3f), generatedRoot.transform, 0.4f, TextAnchor.MiddleCenter);
            CreateTextLabel("Mud", new Vector3(swatchStartX + spacing * 4f, 0.1f, sharedRowZ - 2.3f), generatedRoot.transform, 0.4f, TextAnchor.MiddleCenter);
            CreateTextLabel("Road", new Vector3(swatchStartX + spacing * 5f, 0.1f, sharedRowZ - 2.3f), generatedRoot.transform, 0.4f, TextAnchor.MiddleCenter);
            CreateTextLabel("Riverbank", new Vector3(swatchStartX + spacing * 6f, 0.1f, sharedRowZ - 2.3f), generatedRoot.transform, 0.4f, TextAnchor.MiddleCenter);

            for (var index = 0; index < VisualHouseIds.Length; index++)
            {
                var houseId = VisualHouseIds[index];
                if (!houses.TryGetValue(houseId, out var palette))
                {
                    continue;
                }

                var primaryMaterial = GetOrCreateColorMaterial(
                    DynastyMaterialRoot + "/bl_testbed_" + houseId + "_primary_placeholder_v001.mat",
                    palette.PrimaryColor,
                    0.03f,
                    0.12f);
                var accentMaterial = GetOrCreateColorMaterial(
                    DynastyMaterialRoot + "/bl_testbed_" + houseId + "_accent_placeholder_v001.mat",
                    palette.AccentColor,
                    0.02f,
                    0.10f);

                var anchor = GetGridAnchor(index, 3, new Vector3(-10f, 0.9f, 0.5f), 12.5f, 8f);
                CreateMaterialSwatch("HousePrimary_" + palette.Name, primaryMaterial, anchor, generatedRoot.transform);
                CreateMaterialSwatch("HouseAccent_" + palette.Name, accentMaterial, anchor + new Vector3(2.7f, 0f, 0f), generatedRoot.transform);
                CreateTextLabel(palette.Name, anchor + new Vector3(1.35f, 0.1f, 2.5f), generatedRoot.transform, 0.45f, TextAnchor.MiddleCenter);
                CreateTextLabel("Primary", anchor + new Vector3(0f, 0.1f, -2.3f), generatedRoot.transform, 0.34f, TextAnchor.MiddleCenter);
                CreateTextLabel("Accent", anchor + new Vector3(2.7f, 0.1f, -2.3f), generatedRoot.transform, 0.34f, TextAnchor.MiddleCenter);
            }

            CreateTextLabel("Staged Board Wall", new Vector3(28.2f, 0.1f, 11.2f), generatedRoot.transform, 0.5f, TextAnchor.MiddleCenter);
            CreateBoardDisplay("BannerBoard", "Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/bl_icon_house_banner_system_sheet_a_first_pass_concept_v001.png", new Vector3(21f, 5f, 7f), new Vector2(6.3f, 3.8f), generatedRoot.transform);
            CreateBoardDisplay("EmblemBoard", "Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/bl_icon_house_founding_emblems_sheet_a_first_pass_concept_v001.png", new Vector3(28.2f, 5f, 7f), new Vector2(6.3f, 3.8f), generatedRoot.transform);
            CreateBoardDisplay("NeutralSettlementBoard", "Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/bl_building_shared_neutral_settlement_structures_sheet_a_first_pass_concept_v001.png", new Vector3(35.4f, 5f, 7f), new Vector2(6.3f, 3.8f), generatedRoot.transform);
            CreateBoardDisplay("FoundationBoards", "Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/bl_material_shared_foundation_boards_sheet_a_first_pass_concept_v001.png", new Vector3(21f, 5f, 1f), new Vector2(6.3f, 3.8f), generatedRoot.transform);
            CreateBoardDisplay("HouseTrimBoards", "Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/bl_material_house_trim_families_sheet_a_first_pass_concept_v001.png", new Vector3(28.2f, 5f, 1f), new Vector2(6.3f, 3.8f), generatedRoot.transform);
            CreateBoardDisplay("FaithStructureBoard", "Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/bl_building_shared_faith_structure_families_sheet_a_first_pass_concept_v001.png", new Vector3(35.4f, 5f, 1f), new Vector2(6.3f, 3.8f), generatedRoot.transform);
            CreateBoardDisplay("CivicSupportBoard", "Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/bl_building_shared_civic_support_variants_sheet_a_first_pass_concept_v001.png", new Vector3(28.2f, 5f, -5f), new Vector2(6.3f, 3.8f), generatedRoot.transform);
            CreateTextLabel("Settlement Variant Wall", new Vector3(50.2f, 0.1f, 11.2f), generatedRoot.transform, 0.5f, TextAnchor.MiddleCenter);
            CreateBoardDisplay("HouseOverlaySupportBoard", "Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/bl_building_shared_house_overlay_support_structures_sheet_a_first_pass_concept_v001.png", new Vector3(43f, 5f, 7f), new Vector2(6.3f, 3.8f), generatedRoot.transform);
            CreateBoardDisplay("MarketTradeYardBoard", "Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/bl_building_shared_market_and_trade_yard_variants_sheet_a_first_pass_concept_v001.png", new Vector3(50.2f, 5f, 7f), new Vector2(6.3f, 3.8f), generatedRoot.transform);
            CreateBoardDisplay("StorehouseGranaryVariantsBoard", "Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/bl_building_shared_storehouse_and_granary_variants_sheet_a_first_pass_concept_v001.png", new Vector3(57.4f, 5f, 7f), new Vector2(6.3f, 3.8f), generatedRoot.transform);
            CreateBoardDisplay("HousingClusterCourtyardBoard", "Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/bl_building_shared_housing_cluster_and_courtyard_variants_sheet_a_first_pass_concept_v001.png", new Vector3(46.6f, 5f, 1f), new Vector2(6.3f, 3.8f), generatedRoot.transform);
            CreateBoardDisplay("CovenantOverlayArchitectureBoard", "Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/bl_building_shared_covenant_overlay_architecture_variants_sheet_a_first_pass_concept_v001.png", new Vector3(53.8f, 5f, 1f), new Vector2(6.3f, 3.8f), generatedRoot.transform);

            EditorSceneManager.SaveScene(scene);
        }

        private static Vector3 GetGridAnchor(int index, int columns, Vector3 start, float spacingX, float spacingZ)
        {
            var column = index % columns;
            var row = index / columns;
            return new Vector3(
                start.x + column * spacingX,
                start.y,
                start.z - row * spacingZ);
        }

        private static string GetHouseBoardTextureAssetPath(string houseId)
        {
            return houseId switch
            {
                "trueborn" => "Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/bl_unit_trueborn_house_military_silhouettes_sheet_a_first_pass_concept_v001.png",
                "highborne" => "Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/bl_unit_highborne_house_military_silhouettes_sheet_a_first_pass_concept_v001.png",
                "ironmark" => "Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/bl_unit_ironmark_house_military_silhouettes_sheet_a_first_pass_concept_v001.png",
                "goldgrave" => "Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/bl_unit_goldgrave_house_military_silhouettes_sheet_a_first_pass_concept_v001.png",
                "stonehelm" => "Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/bl_unit_stonehelm_house_military_silhouettes_sheet_a_first_pass_concept_v001.png",
                "westland" => "Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/bl_unit_westland_house_military_silhouettes_sheet_a_first_pass_concept_v001.png",
                "hartvale" => "Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/bl_unit_hartvale_house_military_silhouettes_sheet_a_first_pass_concept_v001.png",
                "whitehall" => "Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/bl_unit_whitehall_house_military_silhouettes_sheet_a_first_pass_concept_v001.png",
                "oldcrest" => "Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/bl_unit_oldcrest_house_military_silhouettes_sheet_a_first_pass_concept_v001.png",
                _ => null
            };
        }

        private static void PopulateIconLegibilityScene()
        {
            var scene = EditorSceneManager.OpenScene(IconLegibilityScenePath, OpenSceneMode.Single);
            var generatedRoot = ResetGeneratedRoot();

            CreateTextLabel("ICON LEGIBILITY", new Vector3(-14f, 0.1f, 17f), generatedRoot.transform, 1.1f, TextAnchor.MiddleLeft);
            CreateTextLabel("Large, medium, and small board-size comparison lane", new Vector3(-14f, 0.1f, 15.6f), generatedRoot.transform, 0.55f, TextAnchor.MiddleLeft);

            CreateBackdrop("LightBackdrop", ParseHex("#d1ccc1"), new Vector3(-7.5f, 2.5f, 5f), new Vector2(10f, 7f), generatedRoot.transform);
            CreateBackdrop("DarkBackdrop", ParseHex("#34373d"), new Vector3(4.5f, 2.5f, 5f), new Vector2(10f, 7f), generatedRoot.transform);
            CreateBackdrop("TerrainBackdrop", ParseHex("#756858"), new Vector3(16.5f, 2.5f, 5f), new Vector2(10f, 7f), generatedRoot.transform);

            CreateIconBoardTriplet("ResourceCommand", "Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/bl_icon_shared_resource_command_sheet_a_first_pass_concept_v001.png", new Vector3(-7.5f, 4.2f, 5f), generatedRoot.transform);
            CreateIconBoardTriplet("FoundingEmblems", "Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/bl_icon_house_founding_emblems_sheet_a_first_pass_concept_v001.png", new Vector3(4.5f, 4.2f, 5f), generatedRoot.transform);
            CreateIconBoardTriplet("BannerSystem", "Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/bl_icon_house_banner_system_sheet_a_first_pass_concept_v001.png", new Vector3(16.5f, 4.2f, 5f), generatedRoot.transform);

            CreateTextLabel("Light background", new Vector3(-7.5f, 0.1f, 9.2f), generatedRoot.transform, 0.45f, TextAnchor.MiddleCenter);
            CreateTextLabel("Dark background", new Vector3(4.5f, 0.1f, 9.2f), generatedRoot.transform, 0.45f, TextAnchor.MiddleCenter);
            CreateTextLabel("Terrain-tone background", new Vector3(16.5f, 0.1f, 9.2f), generatedRoot.transform, 0.45f, TextAnchor.MiddleCenter);

            EditorSceneManager.SaveScene(scene);
        }

        private static GameObject ResetGeneratedRoot()
        {
            var activeScene = SceneManager.GetActiveScene();
            foreach (var rootObject in activeScene.GetRootGameObjects())
            {
                if (rootObject.name == GeneratedRootName)
                {
                    UnityEngine.Object.DestroyImmediate(rootObject);
                    break;
                }
            }

            var generatedRoot = new GameObject(GeneratedRootName);
            generatedRoot.transform.position = Vector3.zero;
            return generatedRoot;
        }

        private static void CreateInfantryMarker(Vector3 position, Material bodyMaterial, Material accentMaterial, Material weaponMaterial, Transform parent)
        {
            var root = new GameObject("CoreInfantryMarker");
            root.transform.SetParent(parent, false);
            root.transform.position = position;

            CreatePrimitiveChild(root.transform, PrimitiveType.Capsule, "Body", new Vector3(0f, 0f, 0f), new Vector3(0.75f, 1.2f, 0.75f), bodyMaterial);
            CreatePrimitiveChild(root.transform, PrimitiveType.Cylinder, "Shield", new Vector3(0.55f, 0.05f, 0f), new Vector3(0.2f, 0.7f, 0.6f), accentMaterial, new Vector3(90f, 0f, 0f));
            CreatePrimitiveChild(root.transform, PrimitiveType.Cube, "Weapon", new Vector3(-0.45f, 0.35f, 0f), new Vector3(0.12f, 1.2f, 0.12f), weaponMaterial);
        }

        private static void CreateRangedMarker(Vector3 position, Material bodyMaterial, Material accentMaterial, Material weaponMaterial, Transform parent)
        {
            var root = new GameObject("RangedMarker");
            root.transform.SetParent(parent, false);
            root.transform.position = position;

            CreatePrimitiveChild(root.transform, PrimitiveType.Capsule, "Body", new Vector3(0f, 0f, 0f), new Vector3(0.62f, 1.05f, 0.62f), bodyMaterial);
            CreatePrimitiveChild(root.transform, PrimitiveType.Cube, "Bow", new Vector3(0.55f, 0.25f, 0f), new Vector3(0.08f, 1.35f, 0.08f), weaponMaterial, new Vector3(0f, 0f, 15f));
            CreatePrimitiveChild(root.transform, PrimitiveType.Cube, "Quiver", new Vector3(-0.4f, 0.18f, -0.18f), new Vector3(0.18f, 0.55f, 0.18f), accentMaterial);
        }

        private static void CreateCavalryMarker(Vector3 position, Material riderMaterial, Material accentMaterial, Material mountMaterial, Material weaponMaterial, Transform parent)
        {
            var root = new GameObject("CavalryMarker");
            root.transform.SetParent(parent, false);
            root.transform.position = position;

            CreatePrimitiveChild(root.transform, PrimitiveType.Cube, "Mount", new Vector3(0f, -0.25f, 0f), new Vector3(1.8f, 0.9f, 0.85f), mountMaterial);
            CreatePrimitiveChild(root.transform, PrimitiveType.Capsule, "Rider", new Vector3(0.1f, 0.72f, 0f), new Vector3(0.46f, 0.8f, 0.46f), riderMaterial);
            CreatePrimitiveChild(root.transform, PrimitiveType.Cylinder, "Shield", new Vector3(0.8f, 0.35f, 0f), new Vector3(0.18f, 0.45f, 0.42f), accentMaterial, new Vector3(90f, 0f, 0f));
            CreatePrimitiveChild(root.transform, PrimitiveType.Cube, "Lance", new Vector3(-0.8f, 0.7f, 0f), new Vector3(1.8f, 0.08f, 0.08f), weaponMaterial);
        }

        private static void CreateWorkerMarker(Vector3 position, Material bodyMaterial, Material accentMaterial, Material toolMaterial, Transform parent)
        {
            var root = new GameObject("WorkerMarker");
            root.transform.SetParent(parent, false);
            root.transform.position = position;

            CreatePrimitiveChild(root.transform, PrimitiveType.Capsule, "Body", new Vector3(0f, 0f, 0f), new Vector3(0.58f, 0.9f, 0.58f), bodyMaterial);
            CreatePrimitiveChild(root.transform, PrimitiveType.Cube, "Pack", new Vector3(0f, -0.1f, -0.28f), new Vector3(0.32f, 0.36f, 0.2f), accentMaterial);
            CreatePrimitiveChild(root.transform, PrimitiveType.Cube, "Tool", new Vector3(0.55f, 0.15f, 0f), new Vector3(0.1f, 0.95f, 0.1f), toolMaterial, new Vector3(0f, 0f, 22f));
        }

        private static void CreateCommandMarker(Vector3 position, Material bodyMaterial, Material accentMaterial, Material weaponMaterial, Transform parent)
        {
            var root = new GameObject("CommandMarker");
            root.transform.SetParent(parent, false);
            root.transform.position = position;

            CreatePrimitiveChild(root.transform, PrimitiveType.Capsule, "Body", new Vector3(0f, 0f, 0f), new Vector3(0.85f, 1.35f, 0.85f), bodyMaterial);
            CreatePrimitiveChild(root.transform, PrimitiveType.Cube, "BannerPole", new Vector3(0.72f, 0.9f, 0f), new Vector3(0.08f, 2f, 0.08f), weaponMaterial);
            CreatePrimitiveChild(root.transform, PrimitiveType.Cube, "Banner", new Vector3(1.22f, 1.35f, 0f), new Vector3(0.85f, 0.52f, 0.08f), accentMaterial);
            CreatePrimitiveChild(root.transform, PrimitiveType.Cylinder, "Shield", new Vector3(-0.62f, 0.2f, 0f), new Vector3(0.22f, 0.72f, 0.62f), accentMaterial, new Vector3(90f, 0f, 0f));
        }

        private static void CreateTerrainStrip(string name, Material material, Vector3 position, Vector3 scale, Transform parent)
        {
            var plane = CreatePrimitiveChild(parent, PrimitiveType.Plane, name, position, scale, material);
            plane.transform.localScale = new Vector3(scale.x / 10f, 1f, scale.z / 10f);
        }

        private static void CreateCliffRun(Material cliffMaterial, Vector3 position, Transform parent)
        {
            var root = new GameObject("CliffRun");
            root.transform.SetParent(parent, false);
            root.transform.position = position;

            for (var i = 0; i < 4; i++)
            {
                CreatePrimitiveChild(root.transform, PrimitiveType.Cube, "CliffBlock_" + i, new Vector3(i * 1.8f, 0.9f + i * 0.18f, 0f), new Vector3(1.7f, 1.8f + i * 0.35f, 2.6f), cliffMaterial);
            }
        }

        private static void CreateBridgeMarker(Material timberMaterial, Material stoneMaterial, Vector3 position, Transform parent)
        {
            var root = new GameObject("BridgeMarker");
            root.transform.SetParent(parent, false);
            root.transform.position = position;

            CreatePrimitiveChild(root.transform, PrimitiveType.Cube, "Deck", new Vector3(0f, 0f, 0f), new Vector3(4.6f, 0.24f, 1.4f), timberMaterial);
            CreatePrimitiveChild(root.transform, PrimitiveType.Cube, "SupportLeft", new Vector3(-1.6f, -0.42f, 0f), new Vector3(0.5f, 0.8f, 1.0f), stoneMaterial);
            CreatePrimitiveChild(root.transform, PrimitiveType.Cube, "SupportRight", new Vector3(1.6f, -0.42f, 0f), new Vector3(0.5f, 0.8f, 1.0f), stoneMaterial);
        }

        private static void CreateMaterialSwatch(string name, Material material, Vector3 position, Transform parent)
        {
            var root = new GameObject(name);
            root.transform.SetParent(parent, false);
            root.transform.position = position;

            CreatePrimitiveChild(root.transform, PrimitiveType.Sphere, "Sphere", new Vector3(0f, 0f, 0f), new Vector3(1.2f, 1.2f, 1.2f), material);
            CreatePrimitiveChild(root.transform, PrimitiveType.Cube, "Block", new Vector3(0f, -1.1f, 0f), new Vector3(1.5f, 0.32f, 1.5f), material);
        }

        private static void CreateBackdrop(string name, Color color, Vector3 position, Vector2 size, Transform parent)
        {
            var material = GetOrCreateColorMaterial(
                BoardMaterialRoot + "/bl_testbed_" + name.ToLowerInvariant() + "_backdrop_placeholder_v001.mat",
                color,
                0.0f,
                0.0f);

            var backdrop = CreatePrimitiveChild(parent, PrimitiveType.Quad, name, position, new Vector3(size.x, size.y, 1f), material);
            backdrop.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }

        private static void CreateIconBoardTriplet(string name, string texturePath, Vector3 center, Transform parent)
        {
            CreateBoardDisplay(name + "_Large", texturePath, center + new Vector3(0f, 0f, -1.8f), new Vector2(4.3f, 2.6f), parent);
            CreateBoardDisplay(name + "_Medium", texturePath, center + new Vector3(0f, -1.8f, 0f), new Vector2(3.2f, 1.9f), parent);
            CreateBoardDisplay(name + "_Small", texturePath, center + new Vector3(0f, -3.1f, 1.5f), new Vector2(2.05f, 1.2f), parent);
        }

        private static void CreateBoardDisplay(string name, string textureAssetPath, Vector3 position, Vector2 size, Transform parent)
        {
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(textureAssetPath);
            if (texture == null)
            {
                UnityDebug.LogWarning("Bloodlines testbed board texture not found: " + textureAssetPath);
                return;
            }

            var materialPath = BoardMaterialRoot + "/bl_testbed_" + texture.name + "_board_placeholder_v001.mat";
            var material = GetOrCreateTextureMaterial(materialPath, texture);

            var board = CreatePrimitiveChild(parent, PrimitiveType.Quad, name, position, new Vector3(size.x, size.y, 1f), material);
            board.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }

        private static GameObject CreatePrimitiveChild(Transform parent, PrimitiveType primitiveType, string name, Vector3 localPosition, Vector3 localScale, Material material, Vector3? localEulerAngles = null)
        {
            var primitive = GameObject.CreatePrimitive(primitiveType);
            primitive.name = name;
            primitive.transform.SetParent(parent, false);
            primitive.transform.localPosition = localPosition;
            primitive.transform.localScale = localScale;
            if (localEulerAngles.HasValue)
            {
                primitive.transform.localRotation = Quaternion.Euler(localEulerAngles.Value);
            }

            var renderer = primitive.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = material;
            }

            return primitive;
        }

        private static void CreateTextLabel(string text, Vector3 position, Transform parent, float characterSize, TextAnchor anchor)
        {
            var label = new GameObject("LABEL_" + text.Replace(" ", "_").Replace("/", "_"));
            label.transform.SetParent(parent, false);
            label.transform.position = position;
            label.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

            var textMesh = label.AddComponent<TextMesh>();
            textMesh.text = text;
            textMesh.characterSize = characterSize;
            textMesh.anchor = anchor;
            textMesh.fontSize = 48;
            textMesh.color = new Color(0.1f, 0.1f, 0.1f, 1f);
        }

        private static Dictionary<string, HousePalette> LoadHousePaletteMap()
        {
            var paletteMap = new Dictionary<string, HousePalette>(StringComparer.OrdinalIgnoreCase);
            foreach (var record in LoadHouseRecords())
            {
                if (string.IsNullOrWhiteSpace(record.id) || string.IsNullOrWhiteSpace(record.name))
                {
                    continue;
                }

                paletteMap[record.id] = new HousePalette(record.name, ParseHex(record.primaryColor), ParseHex(record.accentColor));
            }

            return paletteMap;
        }

        private static HouseRecord[] LoadHouseRecords()
        {
            var projectRoot = Directory.GetParent(Application.dataPath);
            if (projectRoot == null)
            {
                throw new InvalidOperationException("Unable to resolve Unity project root.");
            }

            var housesPath = Path.GetFullPath(Path.Combine(projectRoot.FullName, "..", "data", "houses.json"));
            if (!File.Exists(housesPath))
            {
                throw new FileNotFoundException("Bloodlines house palette source not found.", housesPath);
            }

            var raw = File.ReadAllText(housesPath);
            var wrapped = "{\"items\":" + raw + "}";
            var wrapper = JsonUtility.FromJson<HouseRecordWrapper>(wrapped);
            return wrapper != null && wrapper.items != null ? wrapper.items : Array.Empty<HouseRecord>();
        }

        private static Material GetOrCreateColorMaterial(string assetPath, Color color, float metallic, float smoothness)
        {
            var material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
            if (material == null)
            {
                material = new Material(FindLitShader());
                AssetDatabase.CreateAsset(material, assetPath);
            }

            ApplyColor(material, color);
            ApplyFloat(material, "_Metallic", metallic);
            ApplyFloat(material, "_Smoothness", smoothness);
            ApplyFloat(material, "_WorkflowMode", 1f);
            EditorUtility.SetDirty(material);
            return material;
        }

        private static Material GetOrCreateTextureMaterial(string assetPath, Texture2D texture)
        {
            var material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
            if (material == null)
            {
                material = new Material(FindUnlitShader());
                AssetDatabase.CreateAsset(material, assetPath);
            }

            if (material.HasProperty("_BaseMap"))
            {
                material.SetTexture("_BaseMap", texture);
            }

            if (material.HasProperty("_MainTex"))
            {
                material.SetTexture("_MainTex", texture);
            }

            material.mainTexture = texture;
            EditorUtility.SetDirty(material);
            return material;
        }

        private static Shader FindLitShader()
        {
            return Shader.Find("Universal Render Pipeline/Lit")
                   ?? Shader.Find("Standard")
                   ?? Shader.Find("Diffuse");
        }

        private static Shader FindUnlitShader()
        {
            return Shader.Find("Universal Render Pipeline/Unlit")
                   ?? Shader.Find("Unlit/Texture")
                   ?? FindLitShader();
        }

        private static void ApplyColor(Material material, Color color)
        {
            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", color);
            }

            if (material.HasProperty("_Color"))
            {
                material.SetColor("_Color", color);
            }
        }

        private static void ApplyFloat(Material material, string propertyName, float value)
        {
            if (material.HasProperty(propertyName))
            {
                material.SetFloat(propertyName, value);
            }
        }

        private static Color ParseHex(string hex)
        {
            if (!string.IsNullOrWhiteSpace(hex) && ColorUtility.TryParseHtmlString(hex, out var color))
            {
                return color;
            }

            return new Color(0.7f, 0.7f, 0.7f, 1f);
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

        [Serializable]
        private sealed class HouseRecordWrapper
        {
            public HouseRecord[] items;
        }

        [Serializable]
        private sealed class HouseRecord
        {
            public string id;
            public string name;
            public string primaryColor;
            public string accentColor;
        }

        private readonly struct HousePalette
        {
            public HousePalette(string name, Color primaryColor, Color accentColor)
            {
                Name = name;
                PrimaryColor = primaryColor;
                AccentColor = accentColor;
            }

            public string Name { get; }
            public Color PrimaryColor { get; }
            public Color AccentColor { get; }
        }
    }
}
