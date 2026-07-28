using System;
using System.IO;
using UnityEngine;

namespace Kampai.Util
{
    public static class OfflineModeUtility
    {
        private static string BasePath 
        {
            get 
            {
                // Use persistentDataPath for writeable storage on all platforms
                return Application.persistentDataPath;
            }
        }

        public static string PlayerSavePath
        {
            get { return Path.Combine(BasePath, "player_save.json"); }
        }

        public static string ConfigCachePath
        {
            get { return Path.Combine(BasePath, "config.json"); }
        }

        public static string DefinitionsCachePath
        {
            get { return Path.Combine(BasePath, "definitions.json"); }
        }

        public static void SaveLocal(string path, string data)
        {
            try
            {
                if (File.Exists(path))
                {
                    string oldPath = path + ".old";
                    if (File.Exists(oldPath)) File.Delete(oldPath);
                    File.Move(path, oldPath);
                }
                
                string directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(path, data);
            }
            catch (Exception e)
            {
                Debug.LogError("[OfflineMode] Failed to save " + path + ": " + e.Message);
            }
        }

        public static string LoadLocal(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    return File.ReadAllText(path);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[OfflineMode] Failed to load " + path + ": " + e.Message);
            }
            return null;
        }

        public static bool HasLocalSave()
        {
            return File.Exists(PlayerSavePath);
        }

        /// <summary>
        /// Compares two save data JSONs (server vs local) and returns the one with the latest timestamp.
        /// Assumes the JSON contains a field like "lastPlayedTime".
        /// </summary>
        public static string GetLatestSave(string serverData, string localData)
        {
            if (string.IsNullOrEmpty(serverData)) return localData;
            if (string.IsNullOrEmpty(localData)) return serverData;

            try
            {
                long serverTime = GetLastPlayedTime(serverData);
                long localTime = GetLastPlayedTime(localData);

                if (localTime > serverTime)
                {
                    Debug.LogErrorFormat("[OfflineMode] Local save is newer than server save ({0} > {1}). Using local save.", localTime, serverTime);
                    return localData;
                }
                else
                {
                    Debug.LogErrorFormat("[OfflineMode] Server save is newer or equal ({0} >= {1}). Using server save.", serverTime, localTime);
                    return serverData;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[OfflineMode] Error comparing saves: " + e.Message);
                return serverData;
            }
        }

        public static string MergeDashboardFlags(string targetSave, string serverSave)
        {
            if (string.IsNullOrEmpty(targetSave) || string.IsNullOrEmpty(serverSave))
                return targetSave;

            try
            {
                bool isEvent110000Disabled = IsSpecialEvent110000Ended(serverSave);

                // Update HasEnded specifically for Definition 110000
                if (isEvent110000Disabled)
                {
                    targetSave = SetSpecialEvent110000HasEnded(targetSave, true);
                }
                else
                {
                    targetSave = SetSpecialEvent110000HasEnded(targetSave, false);
                }

                Debug.LogErrorFormat("[WINTER_DEBUG] MergeDashboardFlags: Target save updated for Def 110000. IsDisabled={0}", isEvent110000Disabled);
                return targetSave;
            }
            catch (Exception e)
            {
                Debug.LogError("[OfflineMode] Error merging dashboard flags: " + e.Message);
                return targetSave;
            }
        }

        private static bool IsSpecialEvent110000Ended(string json)
        {
            if (string.IsNullOrEmpty(json)) return false;
            int idx = json.IndexOf("\"Definition\":110000");
            if (idx == -1) idx = json.IndexOf("\"Definition\": 110000");
            if (idx == -1) return false;

            int start = Math.Max(0, idx - 100);
            int len = Math.Min(json.Length - start, 300);
            string snippet = json.Substring(start, len);
            return snippet.Contains("\"HasEnded\":true") || snippet.Contains("\"HasEnded\": true") || snippet.Contains("\"HASENDED\":true") || snippet.Contains("\"HASENDED\": true");
        }

        private static string SetSpecialEvent110000HasEnded(string json, bool hasEnded)
        {
            if (string.IsNullOrEmpty(json)) return json;
            int idx = json.IndexOf("\"Definition\":110000");
            if (idx == -1) idx = json.IndexOf("\"Definition\": 110000");
            if (idx == -1)
            {
                string itemJson = "{\"$type\":\"Kampai.Game.SpecialEventItem, Assembly-CSharp\",\"Definition\":110000,\"ID\":99001100,\"HasEnded\":" + (hasEnded ? "true" : "false") + ",\"HASENDED\":" + (hasEnded ? "true" : "false") + "}";
                int instancesIdx = json.IndexOf("\"instances\":[");
                if (instancesIdx != -1)
                {
                    int insertPos = instancesIdx + "\"instances\":[".Length;
                    return json.Substring(0, insertPos) + itemJson + (json[insertPos] != ']' ? "," : "") + json.Substring(insertPos);
                }
                return json;
            }

            int start = Math.Max(0, idx - 100);
            int len = Math.Min(json.Length - start, 300);
            string snippet = json.Substring(start, len);
            
            string newSnippet = snippet;
            if (hasEnded)
            {
                newSnippet = newSnippet.Replace("\"HasEnded\":false", "\"HasEnded\":true")
                                       .Replace("\"HasEnded\": false", "\"HasEnded\":true")
                                       .Replace("\"HASENDED\":false", "\"HASENDED\":true")
                                       .Replace("\"HASENDED\": false", "\"HASENDED\":true");
            }
            else
            {
                newSnippet = newSnippet.Replace("\"HasEnded\":true", "\"HasEnded\":false")
                                       .Replace("\"HasEnded\": true", "\"HasEnded\":false")
                                       .Replace("\"HASENDED\":true", "\"HASENDED\":false")
                                       .Replace("\"HASENDED\": true", "\"HASENDED\":false");
            }
            return json.Substring(0, start) + newSnippet + json.Substring(start + len);
        }

        private static long GetLastPlayedTime(string json)
        {
            // We use a simple search to avoid dependency on specific JObject versions
            string search = "\"lastPlayedTime\":";
            int index = json.IndexOf(search);
            if (index == -1) return 0;
            
            int start = index + search.Length;
            int end = json.IndexOfAny(new char[] { ',', '}', ' ' }, start);
            if (end == -1) end = json.Length;
            
            string valueStr = json.Substring(start, end - start).Trim();
            long result;
            if (long.TryParse(valueStr, out result)) return result;
            return 0;
        }
    }
}
