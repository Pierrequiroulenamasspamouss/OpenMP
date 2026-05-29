using Discord.Sdk;
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

public class DiscordController : MonoBehaviour
{
    private const ulong applicationId = 1493210561616543777UL;

    [SerializeField] private float presenceUpdateInterval = 5f;
    [SerializeField] private int playerLevel = 0;

    private Discord.Sdk.Client client;
    private bool initialized;

    private static ulong gameStartTimeMs;
    private static MethodInfo runCallbacksStatic;

    private float nextUpdateUnscaled;
    private static Type contextType;
    private static Type playerServiceType;
    private static Type staticItemType;
    private static Type mgrViewType;
    private static object levelEnum;
    private static MethodInfo getQuantityMethod;

    public static DiscordController Instance { get; private set; }

    public int PlayerLevel => playerLevel;

    public void SetPlayerLevel(int level)
    {
        if (level < 0) level = 0;
        if (playerLevel == level) return;
        playerLevel = level;
        RefreshPresence();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        Initialize();
    }

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        Initialize();

        if (!initialized || client == null)
            return;

        TryRunCallbacksIfExposed();

        if (Time.unscaledTime >= nextUpdateUnscaled)
        {
            nextUpdateUnscaled = Time.unscaledTime + Mathf.Max(1f, presenceUpdateInterval);
            RefreshPresence();
        }
    }

    private void OnDestroy()
    {
        Shutdown();

        if (Instance == this)
            Instance = null;
    }

    private void OnApplicationQuit()
    {
        Shutdown();
    }

    private void Initialize()
    {
        if (initialized || client != null)
            return;

        if (!IsDiscordRunning())
            return;

        try
        {
            client = new Discord.Sdk.Client();
            client.AddLogCallback(OnDiscordLog, LoggingSeverity.Error);
            client.SetStatusChangedCallback(OnStatusChanged);
            client.SetApplicationId(applicationId);

            try
            {
                int pid = System.Diagnostics.Process.GetCurrentProcess().Id;
                client.SetGameWindowPid(pid);
            }
            catch { }

            if (gameStartTimeMs == 0UL)
                gameStartTimeMs = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            CacheOptionalRunCallbacks();

            nextUpdateUnscaled = Time.unscaledTime + Mathf.Max(1f, presenceUpdateInterval);
            initialized = true;

            TryReadPlayerLevelFromService();
            RefreshPresence();
        }
        catch
        {
            client = null;
            initialized = false;
        }
    }

    private void Shutdown()
    {
        if (client != null)
        {
            try { client.ClearRichPresence(); } catch { }
            TryRunCallbacksIfExposed();
            try { client.Disconnect(); } catch { }
            TryRunCallbacksIfExposed();
            try { client.Dispose(); } catch { }
            client = null;
        }

        initialized = false;
    }

    private void RefreshPresence()
    {
        if (client == null || !initialized)
            return;

        TryReadPlayerLevelFromService();
        UpdatePresence(BuildStateText(), "default_icon", BuildDetailsText(), gameStartTimeMs);
    }

    private string BuildStateText()
    {
        return "Unity " + Application.unityVersion;
    }

    private void TryReadPlayerLevelFromService()
    {
        try
        {
            if (contextType == null)
                contextType = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).FirstOrDefault(t => t.FullName == "strange.extensions.context.impl.Context");
            if (contextType == null) return;

            var firstContextField = contextType.GetField("firstContext", BindingFlags.Public | BindingFlags.Static);
            if (firstContextField == null) return;

            var firstContext = firstContextField.GetValue(null);
            if (firstContext == null) return;

            var injectionBinderProp = firstContext.GetType().GetProperty("injectionBinder", BindingFlags.Public | BindingFlags.Instance);
            if (injectionBinderProp == null) return;

            var injectionBinder = injectionBinderProp.GetValue(firstContext);
            if (injectionBinder == null) return;

            if (playerServiceType == null)
                playerServiceType = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).FirstOrDefault(t => t.FullName == "Kampai.Game.IPlayerService");
            if (playerServiceType == null) return;

            var getInstanceMethod = injectionBinder.GetType().GetMethod("GetInstance", new Type[] { typeof(Type) });
            object playerService = null;
            if (getInstanceMethod != null)
                playerService = getInstanceMethod.Invoke(injectionBinder, new object[] { playerServiceType });
            if (playerService == null) return;

            if (staticItemType == null)
                staticItemType = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).FirstOrDefault(t => t.FullName == "Kampai.Game.StaticItem");
            if (staticItemType == null) return;

            if (levelEnum == null)
                levelEnum = Enum.Parse(staticItemType, "LEVEL_ID");

            if (getQuantityMethod == null)
                getQuantityMethod = playerServiceType.GetMethod("GetQuantity", new Type[] { staticItemType });
            if (getQuantityMethod == null) return;

            var result = getQuantityMethod.Invoke(playerService, new object[] { levelEnum });
            if (result == null) return;

            int level = Convert.ToInt32(result);
            if (level >= 0 && playerLevel != level)
                playerLevel = level;
        }
        catch { }
    }

    private string BuildDetailsText()
    {
        try
        {
            if (mgrViewType == null)
            {
                mgrViewType = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.FullName == "Kampai.Game.Mignette.View.MignetteManagerView");
            }

            if (mgrViewType != null)
            {
                var getIsPlayingMethod = mgrViewType.GetMethod("GetIsPlaying", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
                if (getIsPlayingMethod != null)
                {
                    var isPlayingObj = getIsPlayingMethod.Invoke(null, null);
                    bool isPlaying = isPlayingObj is bool b && b;

                    if (isPlaying)
                    {
                        var findMethod = typeof(UnityEngine.Object).GetMethod("FindObjectOfType", new Type[] { typeof(Type) });

                        var managerTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
                            .Where(t => t != mgrViewType && mgrViewType.IsAssignableFrom(t) && !t.IsAbstract);

                        foreach (var mt in managerTypes)
                        {
                            try
                            {
                                var instance = findMethod.Invoke(null, new object[] { mt });
                                if (instance != null)
                                {
                                    string typeName = mt.Name;
                                    string baseName = typeName.Replace("MignetteManagerView", "").Replace("MignetteManager", "").Trim();
                                    baseName = Regex.Replace(baseName, "([a-z])([A-Z])", "$1 $2").Trim();
                                    if (!string.IsNullOrEmpty(baseName))
                                        return "Playing - " + baseName;
                                }
                            }
                            catch { }
                        }
                    }
                }
            }
        }
        catch { }

        return "In Game - Level " + playerLevel;
    }

    private void UpdatePresence(string state, string imageKey, string details, ulong startTimestampMs)
    {
        if (client == null)
            return;

        if (startTimestampMs == 0UL)
        {
            startTimestampMs = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            gameStartTimeMs = startTimestampMs;
        }

        Activity activity = new Activity();
        activity.SetType(ActivityTypes.Playing);
        activity.SetState(state);
        activity.SetDetails(details);

        ActivityAssets assets = new ActivityAssets();
        assets.SetLargeImage(imageKey);
        assets.SetLargeText("Unity Game");
        activity.SetAssets(assets);

        ActivityTimestamps timestamps = new ActivityTimestamps();
        timestamps.SetStart(startTimestampMs);
        activity.SetTimestamps(timestamps);

        client.UpdateRichPresence(activity, OnUpdateRichPresence);
    }

    private bool hasLoggedUpdateError = false;

    private void OnUpdateRichPresence(ClientResult result)
    {
        if (!result.Successful())
        {
            if (!hasLoggedUpdateError)
            {
                Debug.LogWarning("[DiscordController] Failed to update rich presence: " + result.Error() + " (Only logged once to prevent spam)");
                hasLoggedUpdateError = true;
            }
        }
        else
        {
            hasLoggedUpdateError = false;
        }
    }

    private void OnDiscordLog(string message, LoggingSeverity severity) { }

    private void OnStatusChanged(Discord.Sdk.Client.Status status, Discord.Sdk.Client.Error error, int errorCode)
    {
        if (error != Discord.Sdk.Client.Error.None)
            Debug.LogWarning("[DiscordController] Error: " + error + " (" + errorCode + ")");
    }

    private bool IsDiscordRunning()
    {
        try
        {
            return System.Diagnostics.Process.GetProcessesByName("Discord").Any();
        }
        catch
        {
            return false;
        }
    }

    private static void CacheOptionalRunCallbacks()
    {
        if (runCallbacksStatic != null)
            return;

        string[] candidateTypeNames =
        {
            "discordpp",
            "discordpp.discordpp",
            "Discord.Sdk.discordpp",
            "Discord.Sdk.NativeMethods"
        };

        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var tn in candidateTypeNames)
            {
                try
                {
                    var t = asm.GetType(tn, false);
                    if (t == null) continue;

                    var mi = t.GetMethod("RunCallbacks",
                        BindingFlags.Public | BindingFlags.Static,
                        null, Type.EmptyTypes, null);

                    if (mi != null)
                    {
                        runCallbacksStatic = mi;
                        return;
                    }
                }
                catch { }
            }
        }
    }

    private static void TryRunCallbacksIfExposed()
    {
        if (runCallbacksStatic == null)
            return;

        try
        {
            runCallbacksStatic.Invoke(null, null);
        }
        catch { }
    }
}