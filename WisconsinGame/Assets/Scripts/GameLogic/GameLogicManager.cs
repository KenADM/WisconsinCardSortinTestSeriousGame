using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;
using System; 
using TMPro;

public class GameLogicManager : MonoBehaviour
{
    [Header("Data For Analysis")]
    public int combo = 0;
    public int maxCombo = 0;
    public int categoriesCompleted = 0;
    public int categoriesToComplete = 0;
    public int maxReshuffles = 0;
    public int rightChoices = 0; 
    
    private int numberOfTriedApartments;
    
    private bool timeMode = true;
    private bool debugMode = true;
    private List<int> conditionValues = new List<int>() {0, 1, 2};
    private int conditionNumber = 0;

    private bool isTutorial = true; 
    private string endReason = "Quit";
    
    [Header("Platforms")]
    [SerializeField] private List<Transform> platforms = new List<Transform>();

    [Header("Debug Displays")]
    [SerializeField] private GameObject debugConditionComboPanel;
    [SerializeField] private GameObject consolPanel;
    [SerializeField] private GameObject skipButton;

    [SerializeField] private TextMeshProUGUI displayCondition;
    [SerializeField] private TextMeshProUGUI displayCombo;

    [Header("Game Object Timer Display")]
    [SerializeField] private GameObject timerGameObject;

    [Header("End Game UI")]
    [SerializeField] private GameObject endGamePanel; 
    [SerializeField] private TextMeshProUGUI testoEndGamePanel;

    [Header("Panic Button")]
    [SerializeField] private Key ExitKey = Key.E;
    
    private float panicHoldTimer = 0f;
    private float requiredHoldTime = 3f;
    
    private bool isExiting = false;

    public static event Action<Transform> SpawnStartingHouse;
    public static event Action SpawnHouse;
    public static event Action EndGame;
    public static event Action<bool, bool> EnableMoveDirection;
    public static event Action<bool> EnableGrab;
    public static event Action TutorialSpawnNextHouse; 
    public static event Action OnCorrectPlacement; 
    public static event Action OnWrongPlacement; 

    void Awake()
    {
        combo = 0;
        maxCombo = ExperimentSettings.maxCombo;
        categoriesCompleted = 0;
        categoriesToComplete = ExperimentSettings.categoriesToComplete;
        maxReshuffles = ExperimentSettings.maxReshuffles;
        numberOfTriedApartments = 0;
        rightChoices = 0;
        timeMode = ExperimentSettings.timeMode;
        debugMode = ExperimentSettings.debugMode;
        
        DebugPanels(debugMode);
    }

    /// <summary>
    /// Function that activates or deactivates the debug panels based on the provided state.
    /// </summary>
    /// <param name="state">The state to set the debug panels to.</param>
    private void DebugPanels(bool state)
    {
        if (consolPanel != null) consolPanel.SetActive(state);
        if (debugConditionComboPanel != null) debugConditionComboPanel.SetActive(false); // activate only after tutorial ends
        if (skipButton != null) skipButton.SetActive(state);
    }
    
    private void Start()
    {
        if (endGamePanel != null) endGamePanel.SetActive(false);
        if (timerGameObject != null) timerGameObject.SetActive(timeMode);
    }

    private void OnEnable()
    {
        Tutorial.StartGame += StartGameHandler; 
        InverseTowerHandler.checkHousePlacement += CheckHousePlacementHandler;
        SpawnerHouse.EndCausedByMaxReshuffles += EndGameMaxReshuffles;
        ArcadeTimerManager.OnTimeOut += EndTimeOut;
    }

    private void OnDisable()
    {
        Tutorial.StartGame -= StartGameHandler;
        InverseTowerHandler.checkHousePlacement -= CheckHousePlacementHandler;
        SpawnerHouse.EndCausedByMaxReshuffles -= EndGameMaxReshuffles;
        ArcadeTimerManager.OnTimeOut -= EndTimeOut;
        
        // save only if the tutorial has been passed
        if (!isTutorial)
        {
            JsonTelemetryLogger.SaveAndClose(endReason, CalculatePoints(), rightChoices, numberOfTriedApartments, categoriesCompleted);
        }
    }

    private void Update()
    {
        if (isExiting) return;

        if (Keyboard.current != null && Keyboard.current[ExitKey].isPressed)
        {
            panicHoldTimer += Time.deltaTime;
            
            if (panicHoldTimer >= requiredHoldTime)
            {
                isExiting = true; 
                panicHoldTimer = 0f; 
                endReason = "PanicButton";
                ReturnToMenu();
            }
        }
        else
        {
            panicHoldTimer = 0f;
        }
    }

    /// <summary>
    /// Returns the menu screen, effectively ending the current game session and saving any necessary data.
    /// </summary>
    private void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    private void StartGameHandler()
    {
        if (debugConditionComboPanel != null) debugConditionComboPanel.SetActive(debugMode);
        isTutorial = false; 
        combo = 0;
        
        JsonTelemetryLogger.Initialize(ExperimentSettings.userCode, timeMode ? "TimeMode" : "CategoryMode");
        
        ChooseCondition();
        PrintCondition();
        SpawnStartingSetup();
    }

    /// <summary>
    /// Spawns the initial setup of houses on the platforms at the start of the game or tutorial.
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
    /// Handles the logic for checking the placement of a house on a platform. 
    /// It compares the characteristics of the placed house with the target platform's characteristics based on the current condition (Color, Type, or People). 
    /// It updates the combo count, categories completed, and triggers feedback events accordingly. 
    /// If the game is in tutorial mode, it simply spawns the next house without affecting the game state.
    /// </summary>
    /// <param name="platform">The platform on which the house is placed.</param>
    /// <param name="houseValues">The values associated with the placed house.</param>
    private void CheckHousePlacementHandler(GameObject platform, int[] houseValues)
    {
        TowerChar baseTowerChar = platform.GetComponent<TowerChar>();
        numberOfTriedApartments++;

        int[] platformValues = new int[] { 
            baseTowerChar.TowerData.Color, 
            baseTowerChar.TowerData.Type, 
            baseTowerChar.TowerData.People 
        };
        
        FeedbackManager feedbackHandler = platform.GetComponent<FeedbackManager>();

        string condition = GetCondition();
        bool conditionMet = (platformValues[conditionNumber] == houseValues[conditionNumber]);

        // check if the condition is met and update combo and categories accordingly
        if (conditionMet)
        {
            combo++;
            if (!isTutorial) 
            {
                rightChoices++;
                OnCorrectPlacement?.Invoke();
            }
            if (feedbackHandler != null) feedbackHandler.Feedback(true);
        }
        else
        {   
            combo = 0;
            if (!isTutorial) OnWrongPlacement?.Invoke();
            if (feedbackHandler != null) feedbackHandler.Feedback(false);
        }

        if (isTutorial)
        {
            TutorialSpawnNextHouse?.Invoke();
            return;
        }
        
        // check if the combo has reached the maxCombo threshold, and if so, choose a new condition and reset the combo
        if (combo >= maxCombo)
        {
            ChooseCondition();
            PrintCondition();
            combo = 0;
            categoriesCompleted++;
        }
        
        PrintCombo();

        JsonTelemetryLogger.LogPlacement(GetCondition(), conditionMet, combo, houseValues, platformValues, platform.transform.position.x);

        // Check if the game has reached its end conditions: max categories completed (used in category mode only).
        if (!timeMode && categoriesCompleted >= categoriesToComplete)
        {
            endReason = "MaxCategories";
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
        displayCondition.text  = GetCondition();
    }

    private void PrintCombo()
    {
        displayCombo.text = "Combo " + combo;
    }

    /// <summary>
    /// Chooses a new condition for the game (Color, Type, or People) randomly from the available conditions.
    /// Refills the conditionValues list if all conditions have been used and max reshuffles have not been reached.
    /// </summary>
    private void ChooseCondition()
    {
        if (conditionValues.Count == 0)
        {
            conditionValues = new List<int>() {0, 1, 2};
        }

        int randomIndex = UnityEngine.Random.Range(0, conditionValues.Count);
        conditionNumber = conditionValues[randomIndex];
        conditionValues.RemoveAt(randomIndex);
    }

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
    /// Handles the end of the game due to a timeout (only used in time mode).
    /// </summary>
    private void EndTimeOut()
    {
        endReason = "TimeOut";
        EndGamePhase();
    }
    /// <summary>
    /// Handles the end of the game due to a max reshuffles limit (only used in category mode).
    /// </summary>
    private void EndGameMaxReshuffles()
    {
        endReason = "MaxReshuffles";
        EndGamePhase();
    }
    

    /// <summary>
    /// Handles the end of the game and calculates the final score based on the game mode.
    /// </summary>
    private void EndGamePhase()
    {
        EnableMoveDirection?.Invoke(false, false);
        EnableGrab?.Invoke(false);
        
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(true);
            if(timeMode){
                testoEndGamePanel.text = "Paga Giornaliera: " + CalculatePoints() + "€";
            }
            else
            {
                testoEndGamePanel.text = "Percentuale Felicità Raggiunta: " + CalculatePoints() + "%";    
            }
        }
    }

    /// <summary>
    /// Calculates the final score based on the game mode. 
    /// In time mode, the score is calculated based on the number of right choices and wrong choices. 
    /// In category mode, the score is calculated based on the number of categories completed and the total number of tried apartments.
    /// </summary>
    private float CalculatePoints()
    {
        if(timeMode)
        {
            return ((rightChoices * 100) - ((numberOfTriedApartments - rightChoices) * 25));
        }
        else
        {
            if (numberOfTriedApartments == 0) return 0f; 
            return ((categoriesCompleted * maxCombo) / numberOfTriedApartments) * 100f;
        }

    }

    /// <summary>
    /// Handles the game quit action, setting the end reason to "UI_Quit" and returning to the menu.
    /// Associated with button in final prompt.
    /// </summary>
    public void QuitGame()
    {
        endReason = "UI_Quit";
        ReturnToMenu();
    }
}