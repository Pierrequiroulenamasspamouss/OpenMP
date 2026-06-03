using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Kampai.Editor
{
    public class LocalizationSyncTool : EditorWindow
    {
        private Vector2 scrollPos;
        private List<string> results = new List<string>();

        [MenuItem("Kampai/Tools/Localization Sync Tool")]
        public static void ShowWindow()
        {
            GetWindow<LocalizationSyncTool>("Loc Sync");
        }

        private void OnGUI()
        {
            GUILayout.Label("Localization Sync Tool", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Check for Missing Keys"))
            {
                FindMissingKeys();
            }

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            foreach (var result in results)
            {
                EditorGUILayout.LabelField(result);
            }
            EditorGUILayout.EndScrollView();
        }

        private void FindMissingKeys()
        {
            results.Clear();
            string path = Path.Combine(Application.dataPath, "Resources/loc_text");
            if (!Directory.Exists(path))
            {
                results.Add("Error: Could not find Assets/Resources/loc_text");
                return;
            }

            string[] files = Directory.GetFiles(path, "*.json");
            if (files.Length == 0)
            {
                results.Add("No localization JSON files found.");
                return;
            }

            Dictionary<string, HashSet<string>> fileKeys = new Dictionary<string, HashSet<string>>();
            HashSet<string> allKeys = new HashSet<string>();

            foreach (string file in files)
            {
                // Skip -SOURCE files if they exist as they might be different
                if (file.EndsWith("-SOURCE.json")) continue;

                string content = File.ReadAllText(file);
                try
                {
                    var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                    if (data != null)
                    {
                        HashSet<string> keys = new HashSet<string>(data.Keys);
                        fileKeys.Add(Path.GetFileName(file), keys);
                        allKeys.UnionWith(keys);
                    }
                }
                catch (System.Exception ex)
                {
                    results.Add(string.Format("Error parsing {0}: {1}", Path.GetFileName(file), ex.Message));
                }
            }

            bool foundMissing = false;
            foreach (var entry in fileKeys)
            {
                List<string> missingInThisFile = new List<string>();
                foreach (string key in allKeys)
                {
                    if (!entry.Value.Contains(key))
                    {
                        missingInThisFile.Add(key);
                    }
                }

                if (missingInThisFile.Count > 0)
                {
                    foundMissing = true;
                    results.Add(string.Format("--- {0} is missing {1} keys ---", entry.Key, missingInThisFile.Count));
                    foreach (var mk in missingInThisFile)
                    {
                        results.Add(string.Format("  {0}", mk));
                    }
                }
            }

            if (!foundMissing)
            {
                results.Add("All localization files are in sync!");
            }
            
            Repaint();
        }
    }
}
