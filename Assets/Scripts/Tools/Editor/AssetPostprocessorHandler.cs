using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEditor.Presets;
using UnityEngine;

// stad https://docs.unity3d.com/6000.3/Documentation/Manual/DefaultPresetsByFolder.html

namespace PresetsPerFolder
{
    public class AssetPostprocessorHandler : AssetPostprocessor
    {
        void OnPreprocessAsset()
        {
            if (assetPath.StartsWith("Assets/") && !AssetDatabase.IsValidFolder(assetPath) && !assetPath.EndsWith(".cs") && !assetPath.EndsWith(".preset"))
            {
                var path = Path.GetDirectoryName(assetPath);
                ApplyPresetsFromFolderRecursively(path);
            }
        }

        void ApplyPresetsFromFolderRecursively(string folder)
        {
            var parentFolder = Path.GetDirectoryName(folder);
            if (!string.IsNullOrEmpty(parentFolder)) ApplyPresetsFromFolderRecursively(parentFolder);

            context.DependsOnCustomDependency($"PresetPostProcessor_{folder}");
            var presetPaths = Directory.EnumerateFiles(folder, "*.preset", SearchOption.TopDirectoryOnly).OrderBy(a => a);

            foreach (var presetPath in presetPaths)
            {
                var preset = AssetDatabase.LoadAssetAtPath<Preset>(presetPath);
                if (preset == null || preset.ApplyTo(assetImporter))
                {
                    context.DependsOnArtifact(presetPath);
                }
            }
        }

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths, bool didDomainReload)
        {
            if (didDomainReload)
            {
                var allPaths = AssetDatabase.FindAssets("glob:\"**.preset\"")
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .OrderBy(a => a)
                    .ToList();
                bool atLeastOnUpdate = false;
                string previousPath = string.Empty;
                Hash128 hash = new Hash128();
                for (var index = 0; index < allPaths.Count; index++)
                {
                    var path = allPaths[index];
                    var folder = Path.GetDirectoryName(path);
                    if (folder != previousPath)
                    {
                        if (previousPath != string.Empty)
                        {
                            AssetDatabase.RegisterCustomDependency($"PresetPostProcessor_{previousPath}", hash);
                            atLeastOnUpdate = true;
                        }
                        hash = new Hash128();
                        previousPath = folder;
                    }
                    hash.Append(path);
                    hash.Append(AssetDatabase.LoadAssetAtPath<Preset>(path).GetTargetFullTypeName());
                }
                if (previousPath != string.Empty)
                {
                    AssetDatabase.RegisterCustomDependency($"PresetPostProcessor_{previousPath}", hash);
                    atLeastOnUpdate = true;
                }
                if (atLeastOnUpdate)
                    AssetDatabase.Refresh();
            }
        }
    }
    public class UpdateFolderPresetDependency : AssetsModifiedProcessor
    {
        protected override void OnAssetsModified(string[] changedAssets, string[] addedAssets, string[] deletedAssets, AssetMoveInfo[] movedAssets)
        {
            HashSet<string> folders = new HashSet<string>();
            foreach (var asset in changedAssets)
            {
                if (asset.EndsWith(".preset"))
                {
                    folders.Add(Path.GetDirectoryName(asset));
                }
            }
            foreach (var asset in addedAssets)
            {
                if (asset.EndsWith(".preset"))
                {
                    folders.Add(Path.GetDirectoryName(asset));
                }
            }
            foreach (var asset in deletedAssets)
            {
                if (asset.EndsWith(".preset"))
                {
                    folders.Add(Path.GetDirectoryName(asset));
                }
            }
            foreach (var movedAsset in movedAssets)
            {
                if (movedAsset.destinationAssetPath.EndsWith(".preset"))
                {
                    folders.Add(Path.GetDirectoryName(movedAsset.destinationAssetPath));
                }
                if (movedAsset.sourceAssetPath.EndsWith(".preset"))
                {
                    folders.Add(Path.GetDirectoryName(movedAsset.sourceAssetPath));
                }
            }
            if (folders.Count != 0)
            {
                EditorApplication.delayCall += () =>
                {
                    DelayedDependencyRegistration(folders);
                };
            }
        }

        static void DelayedDependencyRegistration(HashSet<string> folders)
        {
            foreach (var folder in folders)
            {
                var presetPaths = AssetDatabase.FindAssets("glob:\"**.preset\"", new[] { folder })
                        .Select(AssetDatabase.GUIDToAssetPath)
                        .Where(presetPath => Path.GetDirectoryName(presetPath) == folder)
                        .OrderBy(a => a);
                Hash128 hash = new Hash128();
                foreach (var presetPath in presetPaths)
                {
                    hash.Append(presetPath);
                    hash.Append(AssetDatabase.LoadAssetAtPath<Preset>(presetPath).GetTargetFullTypeName());
                }
                AssetDatabase.RegisterCustomDependency($"PresetPostProcessor_{folder}", hash);
            }
            AssetDatabase.Refresh();
        }
    }
}