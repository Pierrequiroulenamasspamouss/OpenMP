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
                // Simple parsing to find lastPlayedTime without full deserialization
                long serverTime = GetLastPlayedTime(serverData);
                long localTime = GetLastPlayedTime(localData);

                return (localTime > serverTime) ? localData : serverData;
            }
            catch (Exception e)
            {
                Debug.LogError("[OfflineMode] Error comparing saves: " + e.Message);
                return serverData;
            }
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
