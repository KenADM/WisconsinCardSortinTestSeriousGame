using UnityEngine;
using System.Collections.Generic;
using System; 
using TMPro;
using Serilog;
using static PlayerLog;

/// <summary>
/// Acts as the central "Referee" for the experiment. 
/// Evaluates player placements, manages the active psychological condition (Color, Type, People), 
/// and tracks clinical data metrics (combos, attempts, and overall score).
/// </summary>
public class GameLogicManager : MonoBehaviour
{
    [Header("Data For Analysis")]
    private int combo = 0;
    private int maxCombo = 5;
    private int categoriesCompleted = 0;
    private int categoriesToComplete = 9;
    private int maxReshuffles = 2;
    
    // Tracked to calculate the final Happiness Percentage metric at the end of the game
    private int numberOfTriedApartments;

    // 0: Color | 1: Type | 2: People
    private List<int> conditionValues = new List<int>() {0, 1, 2};
    private int conditionNumber = 0;

    // Prevents clinical data logging and end-game triggers during practice rounds
    private bool isTutorial = true; 
    
    [Header("Platforms")]
    [SerializeField] private List<Transform> platforms = new List<Transform>();

    [Header("Debug Displays")]
    [SerializeField] private TextMeshProUGUI displayCondition;
    [SerializeField] private TextMeshProUGUI displayCombo;

    [Header("End Game UI")]
    [SerializeField] private GameObject endGamePanel; 
    [SerializeField] private TextMeshProUGUI testoEndGamePanel;

    // --- EVENTS ---
    public static event Action<Transform> SpawnStartingHouse;
    public static event Action SpawnHouse;
    public static event Action EndGame;
    public static event Action<bool, bool> EnableMoveDirection;
    public static event Action<bool> EnableGrab;
    public static event Action TutorialSpawnNextHouse; 

    private void Awake()
    {
        combo = 0;
        maxCombo = ExperimentSettings.maxCombo;
        categoriesCompleted = 0;
        categoriesToComplete = ExperimentSettings.categoriesToComplete;
        maxReshuffles = ExperimentSettings.maxReshuffles;
        numberOfTriedApartments = 0;
    }
    
    private void Start()
    {
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(false);
        }
    }

    private void OnEnable()
    {
        Tutorial.StartGame += StartGameHandler; 
        InverseTowerHandler.checkHousePlacement += CheckHousePlacementHandler;
        SpawnerHouse.EndCausedByMaxReshuffles += EndGameMaxReshuffles;
    }

    private void OnDisable()
    {
        Tutorial.StartGame -= StartGameHandler;
        InverseTowerHandler.checkHousePlacement -= CheckHousePlacementHandler;
        SpawnerHouse.EndCausedByMaxReshuffles -= EndGameMaxReshuffles;
        
        PlayerLog.CloseLogger();
    }

    /// <summary>
    /// Initializes the clinical data collection phase, disabling tutorial protections, 
    /// setting the first sorting rule, and populating the gameplay towers.
    /// </summary>
    private void StartGameHandler()
    {
        Debug.Log("Starting Game with user code: " + ExperimentSettings.userCode);
        
        isTutorial = false; 
        combo = 0;
        PlayerLog.Initialize(ExperimentSettings.userCode);
        
        ChooseCondition();
        PrintCondition();
        SpawnStartingSetup();
    }

    /// <summary>
    /// Commands the Spawner Unit to build the unmovable base houses on all available platforms.
    /// </summary>
    private void SpawnStartingSetup()
    {
        for (int i = 0; i < platforms.Count; i++)
        {
            SpawnStartingHouse?.Invoke(platforms[i]);
        }
        SpawnHouse?.Invoke();
    }

    /// <summary>
    /// Evaluates the player's placement of a house on a given tower against the currently active sorting rule (Condition).
    /// Handles visual feedback, combo tracking, and logging the clinical result.
    /// </summary>
    private void CheckHousePlacementHandler(GameObject platform, int[] houseValues)
    {
        TowerChar baseTowerChar = platform.GetComponent<TowerChar>();
        numberOfTriedApartments++;

        int[] platformValues = new int[] { 
            baseTowerChar.TowerData.color, 
            baseTowerChar.TowerData.type, 
            baseTowerChar.TowerData.people 
        };
        
        FeedbackManager feedbackHandler = platform.GetComponent<FeedbackManager>();

        string condition = GetCondition();
        bool conditionMet = (platformValues[conditionNumber] == houseValues[conditionNumber]);
        string conditionResult = "";

        if (conditionMet)
        {
            conditionResult = "Match";
            combo++;
            if (feedbackHandler != null) feedbackHandler.Feedback(true);
        }
        else
        {   
            conditionResult = "Not Match";
            combo = 0;
            if (feedbackHandler != null) feedbackHandler.Feedback(false);
        }

        // if it's in tutorial mode, the choice is not registered 
        if (isTutorial)
        {
            TutorialSpawnNextHouse?.Invoke();
            return;
        }
        
        // Pass to next category if the combo max has been reached
        if (combo >= maxCombo)
        {
            ChooseCondition();
            PrintCondition();
            combo = 0;
            categoriesCompleted++;
        }
        
        PrintCombo();

        Log.Information("Check Condition\nCube:\tColor: {ColorNewCube} Type: {TypeNewCube} People: {PeopleNewCube}\nBase:\tColor: {ColorBase} Type: {TypeBase} People: {PeopleBase}\nCondition: {Condition}\nResult: {ConditionResult} ({RealInARow})\n", 
        houseValues[0], houseValues[1], houseValues[2], platformValues[0], platformValues[1], platformValues[2], condition, conditionResult, combo);

        // End game if the required number of categories has been completed, otherwise spawn a new house
        if (categoriesCompleted >= categoriesToComplete)
        {
            EndGame?.Invoke();
            EndGamePhase();
        }
        else
        {
            SpawnHouse?.Invoke();    
        }
    }

    private void PrintCondition()
    {
        string newCondition = GetCondition();
        Log.Information("New Condition: {Condition}!\n", newCondition); 
        displayCondition.text  = newCondition;
    }

    private void PrintCombo()
    {
        string comboPrint = "Combo " + combo;
        displayCombo.text = comboPrint;
    }

    /// <summary>
    /// Randomly selects a new sorting rule from the remaining pool.
    /// It refills the pool if the categories have not been completed enough time.
    /// </summary>
    private void ChooseCondition()
    {
        // Replenish the pool if all conditions have been exhausted
        if (conditionValues.Count == 0)
        {
            conditionValues = new List<int>() {0, 1, 2};
        }

        int randomIndex = UnityEngine.Random.Range(0, conditionValues.Count);
        conditionNumber = conditionValues[randomIndex];

        Debug.Log("New active condition: " + GetCondition());
        conditionValues.RemoveAt(randomIndex);
    }

    /// <summary>
    /// Converts the current condition integer into its human-readable string equivalent.
    /// </summary>
    private string GetCondition()
    {
        switch (conditionNumber)
        {
            case 0: return "Color";
            case 1: return "Type";
            case 2: return "People";
            default: return "Unknown";
        }
    }

    /// <summary>
    /// If the reshuffles have been maxed before completing enough categories, the experiment ends
    /// </summary>    
    private void EndGameMaxReshuffles()
    {
        Debug.Log("Experiment ended early due to maximum reshuffles.");
        EndGamePhase();
    }
    
    /// <summary>
    /// Locks player controls and displays the final performance metrics.
    /// </summary>
    private void EndGamePhase()
    {
        EnableMoveDirection?.Invoke(false, false);
        EnableGrab?.Invoke(false);
        
        
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(true);
            testoEndGamePanel.text = "Percentuale Felicità Raggiunta: " + CalculatePoints().ToString("F0") + "%";
        }
    }

    /// <summary>
    /// Calculates the player's overall performance score based on successful categories 
    /// relative to total attempts. Returns a percentage from 0 to 100.
    /// </summary>
    private float CalculatePoints()
    {
        if (numberOfTriedApartments == 0) return 0f; 
        
        return ((categoriesCompleted * 5f) / numberOfTriedApartments) * 100f;
    }

    /// <summary>
    /// Safely terminates the application, ensuring all clinical data is written to the log file before closing.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Exiting game and saving logs...");
        
        PlayerLog.CloseLogger(); 
        
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}