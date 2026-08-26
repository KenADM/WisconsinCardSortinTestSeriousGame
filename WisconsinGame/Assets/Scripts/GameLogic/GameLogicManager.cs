using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;
using Serilog;
using static PlayerLog;

public class GameLogicManager : MonoBehaviour
{
    [Header("Data For Analysis")]
    private int combo = 0;
    private int maxCombo = 5;
    private int categoriesCompleted = 0;
    private int categoriesToComplete = 9;
    private int maxReshuffles = 2;
    private int numberOfTriedApartments;

    // condition related data
    private List<int> conditionValues = new List<int>() {0, 1, 2};
    /*
        0: color
        1: type
        2: people
    */
    private int conditionNumber = 0;

    // starts at true, determines if in tutorial mode
    private bool isTutorial = true; 
    
    // platforms used in this phase
    public List<Transform> platforms = new List<Transform>();


    // debug stuff
    public TextMeshProUGUI displayCondition;
    public TextMeshProUGUI displayCombo;

    [Header("End Game UI")]
    [SerializeField] private GameObject endGamePanel; // End Screen
    [SerializeField] private TextMeshProUGUI testoEndGamePanel;

    public static event System.Action<Transform> SpawnStartingHouse;
    public static event System.Action SpawnHouse;
    public static event System.Action EndGame;
    public static event System.Action EnableMoveDirection;
    public static event System.Action EnableGrab;
    public static event System.Action TutorialSpawnNextHouse; 

    void Awake()
    {
        combo = 0;
        maxCombo = ExperimentSettings.maxCombo;
        categoriesCompleted = 0;
        categoriesToComplete = ExperimentSettings.categoriesToComplete;
        maxReshuffles = ExperimentSettings.maxReshuffles;
        numberOfTriedApartments = 0;

    }
    
    void Start()
    {
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(false);
        }
    }

    void OnEnable()
    {
        Tutorial.StartGame += StartGameHandler; 
        //StartingScreenManager.StartGame += StartGameHandler;
        InverseTowerHandler.checkHousePlacement += CheckHousePlacementHandler;
        SpawnerHouse.EndCausedByMaxReshuffles += EndGameMaxReshuffles;
        
    }

    void OnDisable()
    {
        Tutorial.StartGame -= StartGameHandler;
        //StartingScreenManager.StartGame -= StartGameHandler;
        InverseTowerHandler.checkHousePlacement -= CheckHousePlacementHandler;
        SpawnerHouse.EndCausedByMaxReshuffles -= EndGameMaxReshuffles;
        
        PlayerLog.CloseLogger();
    }

    // function called by starting game button
    private void StartGameHandler()
    {
        Debug.Log("Starting Game with user code: " + ExperimentSettings.userCode + " and number of variants per characteristic: ");
        
        isTutorial = false; 
        combo = 0;
        PlayerLog.Initialize(ExperimentSettings.userCode);
        
        ChooseCondition();
        PrintCondition();
        SpawnStartingSetup();
    }

    private void SpawnStartingSetup()
    {
        Debug.Log("Spawning starting cubes");
        for (int i = 0; i < platforms.Count; i++)
        {
            SpawnStartingHouse?.Invoke(platforms[i]);
        }
        SpawnHouse?.Invoke();
    }

    private void CheckHousePlacementHandler(GameObject platform, int[] houseValues)
    {
        TowerChar baseTowerChar = platform.GetComponent<TowerChar>();

        numberOfTriedApartments++;

        // access platform data
        int[] platformValues = new int[] { 
            baseTowerChar.TowerData.color, 
            baseTowerChar.TowerData.type, 
            baseTowerChar.TowerData.people 
        };
        
        FeedbackManager feedbackHandler = platform.GetComponent<FeedbackManager>();

        string condition = GetCondition();
        Debug.Log(condition);
        bool conditionMet = (platformValues[conditionNumber] == houseValues[conditionNumber]);
        Debug.Log(platformValues[conditionNumber]);
        Debug.Log(houseValues[conditionNumber]);
        string conditionResult = "";

        if (conditionMet)
        {
            Debug.Log("Condition met!");
            conditionResult = "Match";
            combo++;
            if(feedbackHandler != null)
            {
                feedbackHandler.Feedback(true);
            }
        }
        else
        {   
            Debug.Log("Condition not met!");
            conditionResult = "Not Match";
            combo = 0;
            if(feedbackHandler != null)
            {
                feedbackHandler.Feedback(false);
            }        
        }
        


        if (isTutorial)
        {
            TutorialSpawnNextHouse?.Invoke();
            return;
        }
        else
        {
            if (combo >= maxCombo)
            {
                Debug.Log("Change condition!");
                ChooseCondition();
                PrintCondition();
                combo = 0;
                PrintCombo();
                categoriesCompleted++;
            }
            PrintCombo();

            Log.Information("Check Condition\nCube:\tColor: {ColorNewCube} Type: {TypeNewCube} People: {PeopleNewCube}\nBase:\tColor: {ColorBase} Type: {TypeBase} People: {PeopleBase}\nCondition: {Condition}\nResult: {ConditionResult} ({RealInARow})\n", 
            houseValues[0], houseValues[1], houseValues[2], platformValues[0], platformValues[1], platformValues[2], condition, conditionResult, combo);

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
        Debug.Log(comboPrint);
        displayCombo.text = comboPrint;
    }

    private void ChooseCondition()
    {
        if(conditionValues.Count == 0)
        {
            conditionValues = new List<int>() {0, 1, 2};
        }

        int randomIndex = UnityEngine.Random.Range(0, conditionValues.Count);
        conditionNumber = conditionValues[randomIndex];

        switch (conditionNumber)
            {
                case 0: 
                    Debug.Log("Condition: Color!");
                    break;
                case 1: 
                    Debug.Log("Condition: Type!");
                    break;
                case 2: 
                    Debug.Log("Condition: People!");
                    break;
            }

        conditionValues.RemoveAt(randomIndex);
    }

    private string GetCondition()
    {
        string newCondition = "";
        switch (conditionNumber)
            {
                case 0:
                    newCondition = "Color";
                    break;
                case 1:
                    newCondition = "Type";
                    break;
                case 2:
                    newCondition = "People";
                    break;
            }
        return newCondition;
    }

    private void EndGameMaxReshuffles()
    {
        EndGamePhase();
    }
    private void EndGamePhase()
    {
        EnableMoveDirection?.Invoke();
        EnableGrab?.Invoke();
        Debug.Log("Tutte le case sono state spawnate. Mostro la schermata finale.");
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(true);
            
            testoEndGamePanel.text = "Percentuale Felicità Raggiunta: " + CalculatePoints().ToString("F0") + "%";
        }
    }

    private float CalculatePoints()
    {
        if (numberOfTriedApartments == 0) return 0f; 
        
        return ((categoriesCompleted * 5f) / numberOfTriedApartments) * 100f;
    }
    // function associated to the button to end game
    public void QuitGame()
    {
        Debug.Log("Uscita dal gioco...");
        
        // Save log
        PlayerLog.CloseLogger(); 
        
        // Close Application
        Application.Quit();
        // for testing
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}