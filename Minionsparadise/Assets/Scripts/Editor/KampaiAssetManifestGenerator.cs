#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Kampai.Util.AssetEditor
{
    public class KampaiAssetManifestGenerator : AssetPostprocessor
    {
        [MenuItem("Kampai/Generate Asset Manifest")]
        public static void GenerateManifest()
        {
            Dictionary<string, string> map = new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase);
            string dataPath = Application.dataPath.Replace('\\', '/');
            string[] searchDirs = { "/content", "/Shader", "/Resources" };

            foreach (string searchDir in searchDirs)
            {
                string fullPath = dataPath + searchDir;
                if (!Directory.Exists(fullPath)) continue;

                string[] files = Directory.GetFiles(fullPath, "*.*", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    string normalizedFile = file.Replace('\\', '/');
                    if (normalizedFile.EndsWith(".meta")) continue;

                    string fileName = Path.GetFileNameWithoutExtension(normalizedFile);
                    if (!map.ContainsKey(fileName))
                    {
                        int assetsIdx = normalizedFile.IndexOf("/Assets/", System.StringComparison.OrdinalIgnoreCase);
                        if (assetsIdx >= 0)
                        {
                            string relativePath = normalizedFile.Substring(assetsIdx + 1);
                            map.Add(fileName, relativePath);
                        }
                    }
                }
            }

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(map, Newtonsoft.Json.Formatting.Indented);
            string resourcesPath = Path.Combine(Application.dataPath, "Resources");
            if (!Directory.Exists(resourcesPath)) Directory.CreateDirectory(resourcesPath);
            
            string manifestPath = Path.Combine(resourcesPath, "KampaiAssetManifest.json");
            File.WriteAllText(manifestPath, json);

            string binaryPath = Path.Combine(resourcesPath, "KampaiAssetManifest.bytes");
            using (FileStream fs = new FileStream(binaryPath, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter writer = new BinaryWriter(fs, System.Text.Encoding.UTF8))
                {
                    writer.Write(map.Count);
                    foreach (KeyValuePair<string, string> kvp in map)
                    {
                        writer.Write(kvp.Key);
                        writer.Write(kvp.Value ?? string.Empty);
                    }
                }
            }

            AssetDatabase.Refresh();

            Debug.Log("KampaiAssetManifest: Generated JSON manifest at " + manifestPath + " and binary manifest with " + map.Count + " entries at " + binaryPath);
        }

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            // Auto-regenerate if things change in key folders
            foreach (string asset in importedAssets)
            {
                if (asset.Contains("/content/") || asset.Contains("/Resources/") || asset.Contains("/Shader/"))
                {
                    GenerateManifest();
                    return;
                }
            }
        }
    }
}
#endif