using UnityEngine;

public static class ExperimentSettings
{
    // Live runtime values in memory
    public static int maxCombo;
    public static int categoriesToComplete;
    public static int maxReshuffles;
    public static int time;
    public static bool vrMode;
    public static bool timeMode;
    public static bool debugMode;
    public static bool skipTutorial;
    public static bool enableUserCodeInGameInput;
    public static string userCode = "";

    private static bool isInitialized = false;

    // Runs once when the application launches before any scene loads
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeApplicationLifecycle()
    {
        Application.quitting += OnApplicationQuitting;

        if (!isInitialized)
        {
            ResetToDefaults();
            isInitialized = true;
        }
    }

    /// <summary>
    /// Reads default values from the ScriptableObject in Resources and clears stale PlayerPrefs.
    /// </summary>
    public static void ResetToDefaults()
    {
        ExperimentConfigSO baseConfig = Resources.Load<ExperimentConfigSO>("DefaultExperimentConfig");

        if (baseConfig != null)
        {
            maxCombo = baseConfig.defaultMaxCombo;
            categoriesToComplete = baseConfig.defaultCategoriesToComplete;
            maxReshuffles = baseConfig.defaultMaxReshuffles;
            time = baseConfig.defaultTime;
            vrMode = baseConfig.defaultVrMode;
            timeMode = baseConfig.defaultTimeMode;
            debugMode = baseConfig.defaultDebugMode;
            skipTutorial = baseConfig.defaultSkipTutorial;
            enableUserCodeInGameInput = baseConfig.defaultEnableUserCodeInGameInput;
        }
        else
        {
            Debug.LogWarning("[ExperimentSettings] DefaultExperimentConfig not found in Resources! Using hardcoded fallbacks.");
            maxCombo = 5;
            categoriesToComplete = 9;
            maxReshuffles = 2;
            time = 60;
            vrMode = false;
            timeMode = true;
            debugMode = false;
            skipTutorial = false;
            enableUserCodeInGameInput = false;
        }

        userCode = "";

        // Purge old data from previous launches
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("[ExperimentSettings] Loaded baseline settings from ScriptableObject.");
    }

    private static void OnApplicationQuitting()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("[ExperimentSettings] App quitting: Settings wiped from storage.");
    }
}