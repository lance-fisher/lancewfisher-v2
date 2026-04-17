using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    public static class GraphicsConceptSheetSync
    {
        private const string TargetAssetFolder = "Assets/_Bloodlines/Art/Staging/ConceptSheets";

        private static readonly string[] AllowedExtensions =
        {
            ".svg",
            ".png",
            ".jpg",
            ".jpeg",
            ".webp",
            ".html",
            ".md"
        };

        [MenuItem("Bloodlines/Graphics/Sync Concept Sheets")]
        public static void SyncConceptSheets()
        {
            var sourceRoot = GetExternalConceptSheetRoot();
            if (!Directory.Exists(sourceRoot))
            {
                UnityDebug.LogError("Bloodlines graphics concept source not found: " + sourceRoot);
                return;
            }

            EnsureFolderChain(TargetAssetFolder);

            var targetRoot = Path.Combine(Application.dataPath, "_Bloodlines", "Art", "Staging", "ConceptSheets");
            if (!Directory.Exists(targetRoot))
            {
                Directory.CreateDirectory(targetRoot);
            }

            var copiedCount = 0;
            foreach (var sourcePath in Directory.GetFiles(sourceRoot))
            {
                if (!IsAllowedExtension(Path.GetExtension(sourcePath)))
                {
                    continue;
                }

                var targetPath = Path.Combine(targetRoot, Path.GetFileName(sourcePath));
                if (!ShouldCopy(sourcePath, targetPath))
                {
                    continue;
                }

                File.Copy(sourcePath, targetPath, true);
                copiedCount++;
            }

            AssetDatabase.Refresh();
            UnityDebug.Log("Bloodlines graphics concept sync complete. Source: " + sourceRoot + " | Copied or updated: " + copiedCount);
        }

        [MenuItem("Bloodlines/Graphics/Open Concept Sheet Folder")]
        public static void OpenConceptSheetFolder()
        {
            EnsureFolderChain(TargetAssetFolder);
            var targetRoot = Path.Combine(Application.dataPath, "_Bloodlines", "Art", "Staging", "ConceptSheets");
            if (!Directory.Exists(targetRoot))
            {
                Directory.CreateDirectory(targetRoot);
            }

            EditorUtility.RevealInFinder(targetRoot);
        }

        private static string GetExternalConceptSheetRoot()
        {
            var projectRoot = Directory.GetParent(Application.dataPath);
            if (projectRoot == null)
            {
                throw new InvalidOperationException("Unable to resolve Unity project root.");
            }

            return Path.GetFullPath(Path.Combine(projectRoot.FullName, "..", "14_ASSETS", "GRAPHICS_PIPELINE", "02_FIRST_PASS_CONCEPT"));
        }

        private static bool IsAllowedExtension(string extension)
        {
            foreach (var allowed in AllowedExtensions)
            {
                if (string.Equals(extension, allowed, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ShouldCopy(string sourcePath, string targetPath)
        {
            if (!File.Exists(targetPath))
            {
                return true;
            }

            var sourceInfo = new FileInfo(sourcePath);
            var targetInfo = new FileInfo(targetPath);

            if (sourceInfo.Length != targetInfo.Length)
            {
                return true;
            }

            return sourceInfo.LastWriteTimeUtc > targetInfo.LastWriteTimeUtc;
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
