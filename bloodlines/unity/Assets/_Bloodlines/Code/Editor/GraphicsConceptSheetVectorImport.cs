using System;
using System.Collections.Generic;
using System.IO;
using Unity.VectorGraphics;
using UnityEditor;
using UnityEngine;
using UnityCamera = UnityEngine.Camera;
using UnityDebug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Process = System.Diagnostics.Process;
using ProcessStartInfo = System.Diagnostics.ProcessStartInfo;

namespace Bloodlines.EditorTools
{
    public static class GraphicsConceptSheetVectorImport
    {
        private const string SourceAssetFolder = "Assets/_Bloodlines/Art/Staging/ConceptSheets";
        private const string RasterizedAssetFolder = "Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized";

        private const string ShaderSupportFolder = "Assets/_Bloodlines/Shaders/Staging/VectorImport";
        private const string VectorShaderPath = ShaderSupportFolder + "/BloodlinesVectorImport.shader";
        private const string GradientShaderPath = ShaderSupportFolder + "/BloodlinesVectorGradientImport.shader";
        private const string DemultiplyShaderPath = ShaderSupportFolder + "/BloodlinesVectorDemultiply.shader";
        private const string ExpandEdgesShaderPath = ShaderSupportFolder + "/BloodlinesVectorExpandEdges.shader";
        private const string BlendMaxShaderPath = ShaderSupportFolder + "/BloodlinesVectorBlendMax.shader";
        private const string ToolScriptPath = "Assets/_Bloodlines/Code/Editor/GraphicsConceptSheetVectorImport.cs";
        private const string BrowserPathEnvironmentVariable = "BLOODLINES_VECTOR_BROWSER_PATH";

        private const int SvgWindowSize = 100;
        private const int MaxRasterDimension = 4096;
        private const int AntiAliasingSamples = 4;
        private const ushort GradientAtlasResolution = 256;
        private const float SvgPixelsPerUnit = 100.0f;
        private const float MaxCordDeviation = 0.25f;
        private const float MaxTangentAngleRadians = 0.17453292f;
        private const float SamplingStepSize = 0.01f;

        private static readonly string[] BrowserCandidates =
        {
            @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",
            @"C:\Program Files\Microsoft\Edge\Application\msedge.exe",
            @"C:\Program Files\Google\Chrome\Application\chrome.exe",
            @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe"
        };

        [MenuItem("Bloodlines/Graphics/Sync And Rasterize Concept Sheets")]
        public static void SyncAndRasterizeConceptSheets()
        {
            GraphicsConceptSheetSync.SyncConceptSheets();
            RasterizeConceptSheets();
        }

        [MenuItem("Bloodlines/Graphics/Rasterize Concept Sheets")]
        public static void RasterizeConceptSheets()
        {
            EnsureFolderChain(RasterizedAssetFolder);

            var sourceRoot = Path.Combine(Application.dataPath, "_Bloodlines", "Art", "Staging", "ConceptSheets");
            if (!Directory.Exists(sourceRoot))
            {
                UnityDebug.LogError("Bloodlines concept sheet staging root not found: " + sourceRoot);
                return;
            }

            var rasterizedRoot = Path.Combine(Application.dataPath, "_Bloodlines", "Art", "Staging", "ConceptSheetsRasterized");
            Directory.CreateDirectory(rasterizedRoot);

            var vectorShader = LoadRequiredShader(VectorShaderPath);

            var svgFiles = Directory.GetFiles(sourceRoot, "*.svg", SearchOption.TopDirectoryOnly);
            if (svgFiles.Length == 0)
            {
                UnityDebug.LogWarning("No staged SVG concept sheets found at " + sourceRoot);
                return;
            }

            var importedAssetPaths = new List<string>(svgFiles.Length);
            var generatedCount = 0;
            var skippedCount = 0;

            foreach (var sourceFile in svgFiles)
            {
                var outputFile = Path.Combine(rasterizedRoot, Path.GetFileNameWithoutExtension(sourceFile) + ".png");
                importedAssetPaths.Add(ToAssetPath(outputFile));

                if (!ShouldRasterize(sourceFile, outputFile))
                {
                    skippedCount++;
                    continue;
                }

                RasterizeSvgConceptSheet(sourceFile, outputFile, vectorShader);
                generatedCount++;
            }

            AssetDatabase.Refresh();
            foreach (var assetPath in importedAssetPaths)
            {
                ApplyRasterTextureImportSettings(assetPath);
            }

            AssetDatabase.SaveAssets();
            UnityDebug.Log(
                "Bloodlines concept sheet rasterization complete. Source: " + sourceRoot +
                " | Generated or updated: " + generatedCount +
                " | Skipped current: " + skippedCount);
        }

        [MenuItem("Bloodlines/Graphics/Open Rasterized Concept Sheet Folder")]
        public static void OpenRasterizedConceptSheetFolder()
        {
            EnsureFolderChain(RasterizedAssetFolder);

            var rasterizedRoot = Path.Combine(Application.dataPath, "_Bloodlines", "Art", "Staging", "ConceptSheetsRasterized");
            Directory.CreateDirectory(rasterizedRoot);
            EditorUtility.RevealInFinder(rasterizedRoot);
        }

        public static void RunBatchSyncAndRasterizeConceptSheets()
        {
            SyncAndRasterizeConceptSheets();
        }

        public static void RunBatchRasterizeConceptSheets()
        {
            RasterizeConceptSheets();
        }

        private static void RasterizeSvgConceptSheet(string sourceFile, string outputFile, Shader vectorShader)
        {
            SVGParser.SceneInfo sceneInfo;
            using (var stream = new StreamReader(sourceFile))
            {
                sceneInfo = SVGParser.ImportSVG(stream, ViewportOptions.PreserveViewport, 0, 1, SvgWindowSize, SvgWindowSize);
            }

            if (sceneInfo.Scene == null || sceneInfo.Scene.Root == null)
            {
                throw new InvalidOperationException("Vector scene import returned an empty root for " + sourceFile);
            }

            var sceneRect = ResolveSceneRect(sceneInfo);
            var outputSize = ComputeOutputSize(sceneRect);
            if (TryBrowserRasterize(sourceFile, outputFile, outputSize, out var browserPath))
            {
                UnityDebug.Log(
                    "Bloodlines vector raster browser export | File: " + Path.GetFileName(sourceFile) +
                    " | Output: " + outputSize.x + "x" + outputSize.y +
                    " | Browser: " + browserPath);
                return;
            }

            var geometry = VectorUtils.TessellateScene(sceneInfo.Scene, CreateTessellationOptions(), sceneInfo.NodeOpacity);
            if (geometry == null || geometry.Count == 0)
            {
                throw new InvalidOperationException("Vector tessellation produced no geometry for " + sourceFile);
            }

            if (GeometryRequiresAtlas(geometry))
            {
                throw new NotSupportedException("Atlas-backed vector fills are not yet supported by the mesh raster path.");
            }

            var mesh = new Mesh
            {
                hideFlags = HideFlags.HideAndDontSave,
                name = Path.GetFileNameWithoutExtension(sourceFile) + "_mesh"
            };
            VectorUtils.FillMesh(mesh, geometry, SvgPixelsPerUnit, true);
            mesh.RecalculateBounds();

            Texture2D outputTexture = null;
            Material renderMaterial = null;

            try
            {
                renderMaterial = CreateRenderMaterial("Bloodlines/VectorImport/Vector", vectorShader);
                outputTexture = RenderMeshToTexture(mesh, outputSize.x, outputSize.y, renderMaterial);

                var centerColor = outputTexture.GetPixel(outputSize.x / 2, outputSize.y / 2);
                UnityDebug.Log(
                    "Bloodlines vector raster debug | File: " + Path.GetFileName(sourceFile) +
                    " | SceneRect: " + sceneRect +
                    " | GeometryCount: " + geometry.Count +
                    " | MeshVertices: " + mesh.vertexCount +
                    " | MeshTriangles: " + mesh.triangles.Length +
                    " | HasAtlas: False" +
                    " | Shader: " + renderMaterial.shader.name +
                    " | CenterPixel: " + centerColor);

                var pngBytes = outputTexture.EncodeToPNG();
                File.WriteAllBytes(outputFile, pngBytes);
            }
            finally
            {
                if (outputTexture != null)
                {
                    Object.DestroyImmediate(outputTexture);
                }

                if (renderMaterial != null)
                {
                    Object.DestroyImmediate(renderMaterial);
                }

                Object.DestroyImmediate(mesh);
            }
        }

        private static Vector2Int ComputeOutputSize(Rect sceneRect)
        {
            var width = Mathf.Max(1, Mathf.CeilToInt(sceneRect.width));
            var height = Mathf.Max(1, Mathf.CeilToInt(sceneRect.height));

            var maxDimension = Mathf.Max(width, height);
            if (maxDimension > MaxRasterDimension)
            {
                var scale = (float)MaxRasterDimension / maxDimension;
                width = Mathf.Max(1, Mathf.RoundToInt(width * scale));
                height = Mathf.Max(1, Mathf.RoundToInt(height * scale));
            }

            return new Vector2Int(width, height);
        }

        private static Rect ResolveSceneRect(SVGParser.SceneInfo sceneInfo)
        {
            var sceneRect = sceneInfo.SceneViewport;
            if (sceneRect.width > Mathf.Epsilon && sceneRect.height > Mathf.Epsilon)
            {
                return sceneRect;
            }

            sceneRect = VectorUtils.ApproximateSceneNodeBounds(sceneInfo.Scene.Root);
            if (sceneRect.width <= Mathf.Epsilon || sceneRect.height <= Mathf.Epsilon)
            {
                throw new InvalidOperationException("Unable to resolve non-zero scene bounds for rasterization.");
            }

            return sceneRect;
        }

        private static VectorUtils.TessellationOptions CreateTessellationOptions()
        {
            var options = new VectorUtils.TessellationOptions
            {
                StepDistance = float.MaxValue,
                SamplingStepSize = SamplingStepSize
            };

            options.MaxCordDeviation = MaxCordDeviation;
            options.MaxTanAngleDeviation = MaxTangentAngleRadians;
            return options;
        }

        private static Texture2D RenderMeshToTexture(Mesh mesh, int width, int height, Material renderMaterial)
        {
            if (width <= 0 || height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Raster output dimensions must be greater than zero.");
            }

            var oldActive = RenderTexture.active;
            RenderTexture targetTexture = null;
            GameObject cameraObject = null;
            GameObject meshObject = null;

            try
            {
                var descriptor = new RenderTextureDescriptor(width, height, RenderTextureFormat.ARGB32, 0)
                {
                    msaaSamples = Mathf.Max(1, AntiAliasingSamples),
                    sRGB = QualitySettings.activeColorSpace == ColorSpace.Linear
                };

                targetTexture = RenderTexture.GetTemporary(descriptor);

                meshObject = new GameObject("BloodlinesVectorMeshPreview")
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
                var meshFilter = meshObject.AddComponent<MeshFilter>();
                meshFilter.sharedMesh = mesh;
                var meshRenderer = meshObject.AddComponent<MeshRenderer>();
                meshRenderer.sharedMaterial = renderMaterial;

                cameraObject = new GameObject("BloodlinesVectorPreviewCamera")
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
                var camera = cameraObject.AddComponent<UnityCamera>();
                camera.enabled = false;
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = Color.clear;
                camera.orthographic = true;
                camera.nearClipPlane = 0.01f;
                camera.farClipPlane = 100.0f;
                camera.allowMSAA = AntiAliasingSamples > 1;
                camera.targetTexture = targetTexture;

                var bounds = mesh.bounds;
                var aspect = (float)width / height;
                camera.orthographicSize = Mathf.Max(bounds.extents.y, bounds.extents.x / aspect);
                camera.transform.position = new Vector3(bounds.center.x, bounds.center.y, -10.0f);
                camera.transform.rotation = Quaternion.identity;
                camera.Render();

                RenderTexture.active = targetTexture;
                var copy = new Texture2D(width, height, TextureFormat.RGBA32, false)
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
                copy.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                copy.Apply();
                return copy;
            }
            finally
            {
                RenderTexture.active = oldActive;

                if (cameraObject != null)
                {
                    Object.DestroyImmediate(cameraObject);
                }

                if (meshObject != null)
                {
                    Object.DestroyImmediate(meshObject);
                }

                if (targetTexture != null)
                {
                    RenderTexture.ReleaseTemporary(targetTexture);
                }
            }
        }

        private static Material CreateRenderMaterial(string preferredShaderName, Shader fallbackShader)
        {
            var shader = Shader.Find(preferredShaderName) ?? fallbackShader;
            if (shader == null)
            {
                throw new InvalidOperationException("Cannot create a render material without a compiled shader.");
            }

            return new Material(shader)
            {
                hideFlags = HideFlags.HideAndDontSave
            };
        }

        private static bool TryBrowserRasterize(string sourceFile, string outputFile, Vector2Int outputSize, out string browserPath)
        {
            browserPath = FindAvailableBrowser();
            if (string.IsNullOrEmpty(browserPath))
            {
                return false;
            }

            var projectRoot = Directory.GetParent(Application.dataPath);
            if (projectRoot == null)
            {
                return false;
            }

            var tempRoot = Path.Combine(projectRoot.FullName, "Temp", "BloodlinesGraphicsBrowserRaster");
            Directory.CreateDirectory(tempRoot);

            var profilePath = Path.Combine(tempRoot, "browser-profile");
            Directory.CreateDirectory(profilePath);

            var wrapperPath = Path.Combine(tempRoot, Path.GetFileNameWithoutExtension(sourceFile) + ".html");
            var sourceUri = new Uri(sourceFile).AbsoluteUri;
            var wrapperHtml =
                "<!DOCTYPE html><html><head><meta charset=\"utf-8\">" +
                "<style>html,body{margin:0;padding:0;overflow:hidden;background:transparent;width:" + outputSize.x + "px;height:" + outputSize.y + "px;}" +
                "img{display:block;width:" + outputSize.x + "px;height:" + outputSize.y + "px;}</style></head>" +
                "<body><img src=\"" + sourceUri + "\" alt=\"\" /></body></html>";
            File.WriteAllText(wrapperPath, wrapperHtml);

            var startInfo = new ProcessStartInfo
            {
                FileName = browserPath,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };
            startInfo.ArgumentList.Add("--headless=new");
            startInfo.ArgumentList.Add("--disable-gpu");
            startInfo.ArgumentList.Add("--hide-scrollbars");
            startInfo.ArgumentList.Add("--no-first-run");
            startInfo.ArgumentList.Add("--no-default-browser-check");
            startInfo.ArgumentList.Add("--run-all-compositor-stages-before-draw");
            startInfo.ArgumentList.Add("--virtual-time-budget=2000");
            startInfo.ArgumentList.Add("--force-device-scale-factor=1");
            startInfo.ArgumentList.Add("--user-data-dir=" + profilePath);
            startInfo.ArgumentList.Add("--screenshot=" + outputFile);
            startInfo.ArgumentList.Add("--window-size=" + outputSize.x + "," + outputSize.y);
            startInfo.ArgumentList.Add(new Uri(wrapperPath).AbsoluteUri);

            using var process = Process.Start(startInfo);
            if (process == null)
            {
                return false;
            }

            process.WaitForExit();
            return File.Exists(outputFile) && new FileInfo(outputFile).Length > 0;
        }

        private static Shader LoadRequiredShader(string assetPath)
        {
            var shader = AssetDatabase.LoadAssetAtPath<Shader>(assetPath);
            if (shader == null)
            {
                throw new InvalidOperationException("Required Bloodlines vector support shader not found: " + assetPath);
            }

            return shader;
        }

        private static bool ShouldRasterize(string sourceFile, string outputFile)
        {
            if (!File.Exists(outputFile))
            {
                return true;
            }

            var outputWriteTime = File.GetLastWriteTimeUtc(outputFile);
            if (File.GetLastWriteTimeUtc(sourceFile) > outputWriteTime)
            {
                return true;
            }

            return
                IsSupportAssetNewer(VectorShaderPath, outputWriteTime) ||
                IsSupportAssetNewer(GradientShaderPath, outputWriteTime) ||
                IsSupportAssetNewer(DemultiplyShaderPath, outputWriteTime) ||
                IsSupportAssetNewer(ExpandEdgesShaderPath, outputWriteTime) ||
                IsSupportAssetNewer(BlendMaxShaderPath, outputWriteTime) ||
                IsSupportAssetNewer(ToolScriptPath, outputWriteTime);
        }

        private static bool GeometryRequiresAtlas(IReadOnlyList<VectorUtils.Geometry> geometry)
        {
            for (var i = 0; i < geometry.Count; i++)
            {
                if (geometry[i].UVs != null && geometry[i].UVs.Length > 0)
                {
                    return true;
                }
            }

            return false;
        }

        private static string FindAvailableBrowser()
        {
            var overrideBrowser = Environment.GetEnvironmentVariable(BrowserPathEnvironmentVariable);
            if (!string.IsNullOrWhiteSpace(overrideBrowser) && File.Exists(overrideBrowser))
            {
                return overrideBrowser;
            }

            for (var i = 0; i < BrowserCandidates.Length; i++)
            {
                if (File.Exists(BrowserCandidates[i]))
                {
                    return BrowserCandidates[i];
                }
            }

            return null;
        }

        private static bool IsSupportAssetNewer(string assetPath, DateTime outputWriteTime)
        {
            var fullPath = Path.GetFullPath(Path.Combine(Directory.GetParent(Application.dataPath).FullName, assetPath));
            return File.Exists(fullPath) && File.GetLastWriteTimeUtc(fullPath) > outputWriteTime;
        }

        private static void ApplyRasterTextureImportSettings(string assetPath)
        {
            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null)
            {
                return;
            }

            var changed = false;

            if (importer.textureType != TextureImporterType.Default)
            {
                importer.textureType = TextureImporterType.Default;
                changed = true;
            }

            if (importer.alphaIsTransparency != true)
            {
                importer.alphaIsTransparency = true;
                changed = true;
            }

            if (importer.mipmapEnabled)
            {
                importer.mipmapEnabled = false;
                changed = true;
            }

            if (importer.wrapMode != TextureWrapMode.Clamp)
            {
                importer.wrapMode = TextureWrapMode.Clamp;
                changed = true;
            }

            if (importer.filterMode != FilterMode.Bilinear)
            {
                importer.filterMode = FilterMode.Bilinear;
                changed = true;
            }

            if (importer.textureCompression != TextureImporterCompression.Uncompressed)
            {
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                changed = true;
            }

            if (importer.maxTextureSize != MaxRasterDimension)
            {
                importer.maxTextureSize = MaxRasterDimension;
                changed = true;
            }

            if (importer.npotScale != TextureImporterNPOTScale.None)
            {
                importer.npotScale = TextureImporterNPOTScale.None;
                changed = true;
            }

            if (importer.isReadable)
            {
                importer.isReadable = false;
                changed = true;
            }

            if (changed)
            {
                importer.SaveAndReimport();
            }
        }

        private static string ToAssetPath(string fullPath)
        {
            var normalizedFullPath = fullPath.Replace('\\', '/');
            var normalizedAssetsPath = Application.dataPath.Replace('\\', '/');
            if (!normalizedFullPath.StartsWith(normalizedAssetsPath, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Path is outside the Unity Assets root: " + fullPath);
            }

            return "Assets" + normalizedFullPath.Substring(normalizedAssetsPath.Length);
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
    }
}
