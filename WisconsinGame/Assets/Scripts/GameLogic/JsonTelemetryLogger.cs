using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Serializable class representing the root structure of the JSON telemetry file.
/// </summary>
[Serializable]
public class TelemetryData
{
    public string username;
    public string gameMode;
    public DeviceData device;
    public GameConfigData gameConfig;
    public GameStateData finalGameState;
    
    // BACK TO A SINGLE LIST:
    public List<LogEvent> rawLogs = new List<LogEvent>();
}

/// <summary>
/// Serializable class for hardware profiling to monitor potential performance issues.
/// </summary>
[Serializable]
public class DeviceData
{
    public string deviceType;
    public string graphicsDeviceName;
    public string operatingSystem;
    public string processor;
    public int totalRamGB;
}

/// <summary>
/// Serializable class containing the initial rules and settings of the session.
/// </summary>
[Serializable]
public class GameConfigData
{
    public int maxCombo;
    public int categoriesToComplete;
    public int maxReshuffles;
    public bool isTimeMode;
    public float timeLimit;
    public bool isVrMode;
    public bool debugMode;
}

/// <summary>
/// Serializable class tracking the final clinical outcomes and metrics.
/// </summary>
[Serializable]
public class GameStateData
{
    public string endReason;
    public float totalScore;
    public int correctPlacements;
    public int totalAttempts;
    public int categoriesCompleted;
}

/// <summary>
/// Serializable class representing ANY action or event on the timeline.
/// </summary>
[Serializable]
public class LogEvent
{
    public float timeFromStart;
    public string eventType; // "Movement", "Grab", "Drop", "Placement"
    
    // Variables used by Movement/Interaction
    public string direction;
    public float positionX;
    
    // Variables used by Placements
    public string activeCondition;
    public bool isMatch;
    public int comboAfter;
    
    // Variables used by both (Interactions and Placements)
    public int[] houseValues; 
    public int[] towerTarget;
}


/// <summary>
/// Manages the collection and saving of clinical telemetry data into a JSON file.
/// </summary>
public static class JsonTelemetryLogger
{
    private static TelemetryData sessionData;
    private static float sessionStartTime;
    private static bool isTracking = false;


    /// <summary>
    /// Initializes a new telemetry session with the provided username and game mode.
    /// Sets up the device and game configuration data, and prepares the rawLogs list for event tracking.
    /// </summary>
    /// <param name="usernameNew">The username for the new telemetry session.</param>
    /// <param name="gameModeNew">The game mode for the new telemetry session, time Mode or max category mode.</param>
    public static void Initialize(string usernameNew, string gameModeNew)
    {
        sessionData = new TelemetryData
        {
            username = usernameNew,
            gameMode = gameModeNew,
            device = new DeviceData
            {
                deviceType = SystemInfo.deviceModel,
                graphicsDeviceName = SystemInfo.graphicsDeviceName,
                operatingSystem = SystemInfo.operatingSystem,
                processor = SystemInfo.processorType,
                totalRamGB = SystemInfo.systemMemorySize / 1024
            },
            gameConfig = new GameConfigData
            {                
                maxCombo = ExperimentSettings.maxCombo,
                categoriesToComplete = ExperimentSettings.categoriesToComplete,
                maxReshuffles = ExperimentSettings.maxReshuffles,
                isTimeMode = ExperimentSettings.timeMode,
                timeLimit = ExperimentSettings.time,
                isVrMode = ExperimentSettings.vrMode,
                debugMode = ExperimentSettings.debugMode,
            },
            finalGameState = new GameStateData(),

            // initialize the rawLogs list to ensure it's ready to store events
            rawLogs = new List<LogEvent>()
        };

        sessionStartTime = Time.time;
        isTracking = true;
        
        Debug.Log("[JsonTelemetryLogger] Session initialized for user: " + usernameNew);
    }

    /// <summary>
    /// Logs a movement event, capturing the direction and current X position of the player.
    /// </summary>
    /// <param name="direction">The direction of the movement.</param>
    /// <param name="currentPosX">The current X position of the player.</param>
    public static void LogMovement(string direction, float currentPosX)
    {
        if (!isTracking) return;

        sessionData.rawLogs.Add(new LogEvent
        {
            timeFromStart = Time.time - sessionStartTime,
            eventType = "Movement",
            direction = direction,
            positionX = currentPosX
        });
    }

    /// <summary>
    /// Logs a house interaction event, capturing whether it was a grab or drop, the house values, and the current X position of the player.
    /// </summary>
    /// <param name="isGrab">Indicates whether the action was a grab (true) or a drop (false).</param>
    /// <param name="houseValues">The values associated with the house being interacted with.</param>
    /// <param name="currentPosX">The current X position of the player.</param>

    public static void LogHouseInteraction(bool isGrab, int[] houseValues, float currentPosX)
    {
        if (!isTracking) return;

        sessionData.rawLogs.Add(new LogEvent
        {
            timeFromStart = Time.time - sessionStartTime,
            eventType = isGrab ? "Grab" : "Drop",
            positionX = currentPosX,
            houseValues = houseValues
        });
    }

    /// <summary>
    /// Logs a house placement event, capturing the active condition, whether it was a match, the combo after placement, the house values, and the target tower values.
    /// These logs can be used to analyze player performance and decision-making during the game, like the real wisconsin test.
    /// </summary>
    /// <param name="activeCondition">The active condition during the placement.</param>    
    /// <param name="isMatch">Indicates whether the placement was a match (true) or not (false).</param>
    /// <param name="combo">The combo after the placement.</param>
    /// <param name="houseValues">The values associated with the house being placed.</param>
    /// <param name="baseValues">The values associated with the target tower.</param>
    public static void LogPlacement(string activeCondition, bool isMatch, int combo, int[] houseValues, int[] baseValues)
    {
        if (!isTracking) return;

        sessionData.rawLogs.Add(new LogEvent
        {
            timeFromStart = Time.time - sessionStartTime,
            eventType = "Placement",
            activeCondition = activeCondition,
            isMatch = isMatch,
            comboAfter = combo,
            houseValues = houseValues,
            towerTarget = baseValues
        });
    }


    /// <summary>
    /// Saves the current telemetry session data to a JSON file and resets the tracking state.
    /// The file is saved in a "PlayerLog" folder within the persistent data path, with a filename that includes the username and a timestamp.
    /// </summary>
    /// <param name="endReason">The reason for ending the session.</param>
    /// <param name="finalScore">The final score of the session.</param>
    /// <param name="correctChoices">The number of correct choices made during the session.</param>
    /// <param name="totalAttempts">The total number of attempts made during the session.</param>
    /// <param name="categoriesCompleted">The number of categories completed during the session.</param>
    public static void SaveAndClose(string endReason, float finalScore, int correctChoices, int totalAttempts, int categoriesCompleted)
    {
        if (!isTracking || sessionData == null) return;

        sessionData.finalGameState.endReason = endReason;
        sessionData.finalGameState.totalScore = finalScore;
        sessionData.finalGameState.correctPlacements = correctChoices;
        sessionData.finalGameState.totalAttempts = totalAttempts;
        sessionData.finalGameState.categoriesCompleted = categoriesCompleted;

        string jsonData = JsonUtility.ToJson(sessionData, true);

        string folderName = "PlayerLog";
        
        // determines the directory path based on the platform, using standard path
        string directoryPath = Path.Combine(Application.persistentDataPath, folderName);

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = $"Telemetry_{sessionData.username}_{timestamp}.json";
        string filePath = Path.Combine(directoryPath, fileName);

        File.WriteAllText(filePath, jsonData);

        Debug.Log("Player logs saved in: " + filePath);
        
        isTracking = false;
        sessionData = null; 
    }
}