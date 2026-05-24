using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Kampai.Common;
using Kampai.Main;
using Kampai.Splash;
using Object = UnityEngine.Object;

namespace Kampai.Util
{
    public static class KampaiResources
    {
        private sealed class AssetsCache
        {
            private readonly Dictionary<string, Dictionary<Type, Object>> _cache = new Dictionary<string, Dictionary<Type, Object>>(4096);

            public Object Get(string name, Type type)
            {
                if (string.IsNullOrEmpty(name)) return null;
                
                Dictionary<Type, Object> typeDict;
                if (!_cache.TryGetValue(name, out typeDict)) return null;
                
                Object obj;
                return typeDict.TryGetValue(type, out obj) ? obj : null;
            }

            public void Add(string name, Object obj, Type type)
            {
                if (obj == null || string.IsNullOrEmpty(name)) return;
                
                Dictionary<Type, Object> typeDict;
                if (!_cache.TryGetValue(name, out typeDict))
                {
                    typeDict = new Dictionary<Type, Object>(1);
                    _cache.Add(name, typeDict);
                }
                
                typeDict[type] = obj;
            }

            public void Clear()
            {
                foreach (Dictionary<Type, Object> typeDict in _cache.Values)
                {
                    foreach (Object obj in typeDict.Values)
                    {
                        if (obj != null && !(obj is GameObject) && !(obj is Component))
                        {
                            Resources.UnloadAsset(obj);
                        }
                    }
                }
                _cache.Clear();
            }
        }

        private static IManifestService _manifestService;
        private static ILocalContentService _localContentService;
        private static IKampaiLogger _logger;
        private static readonly AssetsCache _cachedObjects = new AssetsCache();

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_ANDROID
        private static Dictionary<string, string> _editorAssetPathMap;
        private static readonly object _initLock = new object();
        private static volatile bool _isInitialized = false;
        private static global::System.Threading.Thread _initThread;

        public static void InitializeAssetMap()
        {
            if (_isInitialized) return;

            lock (_initLock)
            {
                if (_isInitialized) return;

                // Load manifest first (fast path for all platforms)
                TextAsset manifest = Resources.Load<TextAsset>("KampaiAssetManifest");
                if (manifest == null)
                {
                    _isInitialized = true;
                    if (_logger != null) _logger.Warning("KampaiResources: KampaiAssetManifest not found in Resources. Build asset loading may fail.");
                    return;
                }

                _editorAssetPathMap = new Dictionary<string, string>(4096, StringComparer.OrdinalIgnoreCase);

                // TextAsset properties must be accessed on the main thread
                byte[] manifestBytes = manifest.bytes;
                string manifestText = manifest.text;

                _initThread = new global::System.Threading.Thread(delegate()
                {
                    System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
                    try
                    {
                        if (manifestBytes != null && manifestBytes.Length > 0 && manifestBytes[0] != '{')
                        {
                            ParseManifestBinary(manifestBytes);
                        }
                        else
                        {
                            ParseManifestFast(manifestText);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (_logger != null) _logger.Error(string.Format("KampaiResources: Failed to parse KampaiAssetManifest: {0}", ex.Message));
                    }
                    finally
                    {
                        _isInitialized = true;
                        sw.Stop();
                        if (_logger != null) _logger.Info(string.Format("KampaiResources: Background initialization of asset map completed with {0} entries in {1}ms", _editorAssetPathMap.Count, sw.ElapsedMilliseconds));
                    }
                });
                _initThread.Start();
            }
        }

        private static void EnsureAssetMapInitialized()
        {
            if (_isInitialized) return;

            lock (_initLock)
            {
                if (_isInitialized) return;

                if (_initThread != null && _initThread.IsAlive)
                {
                    _initThread.Join();
                }
                else
                {
                    InitializeAssetMap();
                    if (_initThread != null && _initThread.IsAlive)
                    {
                        _initThread.Join();
                    }
                }
            }
        }

        private static void ParseManifestBinary(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                using (BinaryReader reader = new BinaryReader(ms, System.Text.Encoding.UTF8))
                {
                    int count = reader.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        string key = reader.ReadString();
                        string val = reader.ReadString();
                        if (!_editorAssetPathMap.ContainsKey(key))
                        {
                            _editorAssetPathMap.Add(key, val);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Fast manual JSON parser for the asset manifest, avoids Newtonsoft.Json overhead.
        /// The manifest is a simple flat {"key":"value",...} dictionary.
        /// </summary>
        private static void ParseManifestFast(string json)
        {
            if (string.IsNullOrEmpty(json)) return;

            int addedCount = 0;
            int i = 0;
            int len = json.Length;

            // Skip to first '{'
            while (i < len && json[i] != '{') i++;
            i++; // skip '{'

            while (i < len)
            {
                // Skip whitespace and commas
                while (i < len && (json[i] == ' ' || json[i] == '\t' || json[i] == '\n' || json[i] == '\r' || json[i] == ',')) i++;
                
                if (i >= len || json[i] == '}') break;

                // Parse key
                if (json[i] != '"') break;
                i++; // skip opening quote
                int keyStart = i;
                while (i < len && json[i] != '"') i++;
                string key = json.Substring(keyStart, i - keyStart);
                i++; // skip closing quote

                // Skip colon and whitespace
                while (i < len && (json[i] == ' ' || json[i] == ':' || json[i] == '\t')) i++;

                // Parse value  
                if (i >= len || json[i] != '"') break;
                i++; // skip opening quote
                int valStart = i;
                while (i < len && json[i] != '"')
                {
                    if (json[i] == '\\') i++; // skip escaped char
                    i++;
                }
                string val = json.Substring(valStart, i - valStart);
                i++; // skip closing quote

                if (!_editorAssetPathMap.ContainsKey(key))
                {
                    _editorAssetPathMap.Add(key, val);
                    addedCount++;
                }
            }
            if (_logger != null) _logger.Info(string.Format("KampaiResources: Loaded {0} entries from KampaiAssetManifest", addedCount));
        }
#endif

        public static void SetManifestService(IManifestService service) 
        { 
            _manifestService = service; 
        }
        

        
        public static void SetLocalContentService(ILocalContentService service) 
        { 
            _localContentService = service; 
        }
        
        public static void SetLogger() 
        {
            _logger = Elevation.Logging.LogManager.GetClassLogger("KampaiResources") as IKampaiLogger;
        }

        public static void ClearCache() 
        { 
            _cachedObjects.Clear(); 
        }

        public static bool FileExists(string path)
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_ANDROID
            EnsureAssetMapInitialized();
            if (_editorAssetPathMap != null && _editorAssetPathMap.ContainsKey(Path.GetFileNameWithoutExtension(path)))
            {
                return true;
            }
#endif
            bool exists = false;
            if (_manifestService != null)
            {
                exists |= _manifestService.GetAssetLocation(path).Length > 0;
            }
            if (_localContentService != null)
            {
                exists |= _localContentService.IsLocalAsset(path);
            }
            return exists;
        }

        public static bool FileDownloaded(string path, DLCModel dlcModel)
        {
            return true;
        }

        public static bool IsAssetTierGated(string asset) 
        {
            return false;
        }

        public static T Load<T>(string path) where T : class 
        {
            return Load(path, typeof(T)) as T;
        }

        public static Object Load(string path) 
        {
            return Load(path, typeof(Object));
        }

		public static global::UnityEngine.Object Load(string path, global::System.Type type)
		{
			if (string.IsNullOrEmpty(path))
			{
				return null;
			}
            if (_logger != null) _logger.Debug(string.Format("KampaiResources.Load('{0}', type={1})", path, type != null ? type.Name : "null"));
            
            Object cached = _cachedObjects.Get(path, type);
            if (cached != null) return cached;

            TimeProfiler.StartAssetLoadSection(path);
            Object result = null;
            string resolvedPath;
            bool isEditorDatabasePath;

            if (TryGetLocalAssetPath(path, out resolvedPath, out isEditorDatabasePath))
            {
                if (_logger != null) _logger.Debug(string.Format("  - Local path found: '{0}' (isEditor={1})", resolvedPath, isEditorDatabasePath));
#if UNITY_EDITOR
                if (isEditorDatabasePath)
                {
                    result = UnityEditor.AssetDatabase.LoadAssetAtPath(resolvedPath, type);
                    if (result == null && _logger != null) _logger.Warning(string.Format("  - AssetDatabase failed to load at '{0}'", resolvedPath));
                }
#endif
                if (result == null)
                {
                    string resourcePath = resolvedPath;
                    if (resourcePath.StartsWith("Assets/Resources/", StringComparison.OrdinalIgnoreCase))
                    {
                        resourcePath = resourcePath.Substring(17);
                        resourcePath = Path.ChangeExtension(resourcePath, null);
                    }
                    else if (resourcePath.Contains("/Resources/"))
                    {
                        int resIndex = resourcePath.IndexOf("/Resources/", StringComparison.Ordinal) + 11;
                        resourcePath = Path.ChangeExtension(resourcePath.Substring(resIndex), null);
                    }
                    else
                    {
                         // If it doesn't contain /Resources/ but is being loaded from the map, 
                         // it might be a direct Resources.Load attempt by name
                         resourcePath = Path.ChangeExtension(resourcePath, null);
                    }
                    
                    result = Resources.Load(resourcePath, type);
                    if (result != null && _logger != null) _logger.Debug(string.Format("  - Resources.Load success: '{0}'", resourcePath));
                    else if (_logger != null) _logger.Warning(string.Format("  - Resources.Load FAILED: '{0}' (original path: '{1}')", resourcePath, path));
                }

                if (result != null)
                {
                    _cachedObjects.Add(path, result, type);
                    TimeProfiler.EndAssetLoadSection();
                    return result;
                }
            }
            else
            {
                if (_logger != null) _logger.Debug(string.Format("  - No local path for '{0}'", path));
            }

#if !(UNITY_EDITOR || UNITY_STANDALONE_WIN)
            if (_logger != null) _logger.Debug(string.Format("  - Assets are expected to be local or in Resources. Skipping bundle check for '{0}'", path));
#else
            if (_logger != null) _logger.Debug(string.Format("  - Skipping bundle check for '{0}' on Windows/Editor", path));
#endif

            TimeProfiler.EndAssetLoadSection();
            return result;
        }

        public static AsyncOperation LoadAsync(string path, IRoutineRunner routineRunner, Action<Object> onComplete = null)
        {
            return LoadAsync(path, typeof(Object), routineRunner, onComplete);
        }

        public static AsyncOperation LoadAsync(string path, Type type, IRoutineRunner routineRunner, Action<Object> onComplete = null)
        {
            Object cached = _cachedObjects.Get(path, type);
            if (cached != null)
            {
                if (onComplete != null) onComplete(cached);
                return null;
            }

            string resolvedPath;
            bool isEditorPath;

            if (TryGetLocalAssetPath(path, out resolvedPath, out isEditorPath))
            {
#if UNITY_EDITOR
                if (isEditorPath)
                {
                    Object editorObj = UnityEditor.AssetDatabase.LoadAssetAtPath(resolvedPath, type);
                    _cachedObjects.Add(path, editorObj, type);
                    if (onComplete != null) onComplete(editorObj);
                    return null;
                }
#endif
#if !UNITY_EDITOR
                // On build, Resources.LoadAsync expects path relative to Resources folder and NO extension
                if (resolvedPath.StartsWith("Assets/Resources/", StringComparison.OrdinalIgnoreCase))
                {
                    resolvedPath = resolvedPath.Substring(17);
                }
                resolvedPath = global::System.IO.Path.ChangeExtension(resolvedPath, null);
#endif
                ResourceRequest request = Resources.LoadAsync(resolvedPath, type);
                routineRunner.StartCoroutine(LoadAsyncWait(request, onComplete, path, type));
                return request;
            }

            if (onComplete != null) onComplete(null);
            return null;
        }

        private static IEnumerator LoadAsyncWait(AsyncOperation request, Action<Object> onComplete, string name, Type type)
        {
            if (request == null) yield break;
            yield return request;

            Object obj = null;
            
            ResourceRequest resReq = request as ResourceRequest;
            if (resReq != null)
            {
                obj = resReq.asset;
            }

            if (obj != null)
            {
                _cachedObjects.Add(name, obj, type);
                if (onComplete != null) onComplete(obj);
            }
        }

		public static bool TryGetLocalAssetPath(string path, out string resolvedPath, out bool isEditorPath)
		{
			resolvedPath = string.Empty;
			isEditorPath = false;
			if (string.IsNullOrEmpty(path))
			{
				return false;
			}
            EnsureAssetMapInitialized();
            string fileName = Path.GetFileNameWithoutExtension(path);
            
            string editorPath = null;
            if (_editorAssetPathMap != null)
            {
                if (_editorAssetPathMap.TryGetValue(fileName, out editorPath))
                {
                    // Direct match
                }
                else if (_editorAssetPathMap.TryGetValue(fileName + "_Phone", out editorPath))
                {
                    // Device suffix match
                    if (_logger != null) _logger.Debug(string.Format("KampaiResources: Resolved '{0}' to '{1}'", fileName, fileName + "_Phone"));
                }
                else if (_editorAssetPathMap.TryGetValue(fileName + "_Tablet", out editorPath))
                {
                    // Device suffix match
                    if (_logger != null) _logger.Debug(string.Format("KampaiResources: Resolved '{0}' to '{1}'", fileName, fileName + "_Tablet"));
                }
            }

            if (editorPath != null)
            {
#if UNITY_EDITOR
                resolvedPath = editorPath;
                isEditorPath = true;
                return true;
#else
                if (editorPath.StartsWith("Assets/Resources/", StringComparison.OrdinalIgnoreCase))
                {
                    resolvedPath = editorPath;
                    return true;
                }
                if (editorPath.Contains("/Resources/"))
                {
                    int resIndex = editorPath.IndexOf("/Resources/", StringComparison.Ordinal) + 11;
                    resolvedPath = Path.ChangeExtension(editorPath.Substring(resIndex), null);
                    return true;
                }
                // Fallback for Android naming
                resolvedPath = editorPath;
                return true;
#endif
            }
            
            string localKey = global::System.IO.Path.GetFileName(path);
            if (_localContentService != null && _localContentService.IsLocalAsset(localKey))
            {
                resolvedPath = _localContentService.GetAssetPath(localKey);
                return true;
            }

#if UNITY_ANDROID && !UNITY_EDITOR
            // On Android with bundles disabled, we might want to try Resources.Load directly as a fallback
            resolvedPath = path; 
            return true;
#endif
            return false;
        }


    }
}