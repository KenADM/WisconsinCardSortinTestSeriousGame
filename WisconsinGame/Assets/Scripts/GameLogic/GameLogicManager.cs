using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;
using Serilog;
using static PlayerLog;

public class GameLogicManager : MonoBehaviour
{
    [Header("Data For Analysis")]
    [SerializeField] private int combo = 0;
    [SerializeField] private int maxCombo = 5;
    [SerializeField] private int categoriesCompleted = 0;

    private List<int> conditionValues = new List<int>() {0, 1, 2};
    private int conditionNumber;
    public List<Transform> platforms = new List<Transform>();

    private bool isTutorial = true; // Inizia in modalità tutorial

    public TextMeshProUGUI displayCondition;
    public TextMeshProUGUI displayCombo;

    public static event System.Action<Transform> spawnStartingHouse;
    public static event System.Action spawnHouse;
    
    // Evento per dire al tutorial di generare la seconda casa
    public static event System.Action TutorialspawnNextHouse; 
    
    void OnEnable()
    {
        // Qui il Manager si mette "in ascolto" del bottone StartGame del Tutorial
        Tutorial.startGame += StartGameHandler; 
        StartingScreenManager.startGame += StartGameHandler;
        InverseTowerHandler.checkHousePlacement += CheckHousePlacementHandler;
    }

    void OnDisable()
    {
        Tutorial.startGame -= StartGameHandler;
        StartingScreenManager.startGame -= StartGameHandler;
        InverseTowerHandler.checkHousePlacement -= CheckHousePlacementHandler;
        PlayerLog.CloseLogger();
    }

    // Questa funzione viene scatenata dall'evento quando premi il bottone StartGame
    private void StartGameHandler(string codiceUtente, int numOfVariantPerCharacteristic)
    {
        Debug.Log("Starting Game with user code: " + codiceUtente + " and number of variants per characteristic: " + numOfVariantPerCharacteristic);
        
        // IL TUTORIAL FINISCE QUI UFFICIALMENTE
        isTutorial = false; 
        
        PlayerLog.Initialize(codiceUtente);
        
        ChooseCondition();
        PrintCondition();
        SpawnStartingSetup(numOfVariantPerCharacteristic);
    }

    private void SpawnStartingSetup(int numOfVariantPerCharacteristic)
    {
        Debug.Log("Spawning starting cubes");
        for (int i = 0; i < numOfVariantPerCharacteristic; i++)
        {
            spawnStartingHouse?.Invoke(platforms[i]);
        }
        spawnHouse?.Invoke();
    }

    private void CheckHousePlacementHandler(GameObject platform, int[] houseValues)
    {

        // --- DA QUI IN GIÙ AVVIENE SOLO SE ISTUTORIAL È FALSE (GIOCO VERO) ---

        Debug.Log("check");
        TowerChar baseTowerChar = platform.GetComponent<TowerChar>();
        int[] platformValues = new int[] { baseTowerChar.GetColorTower(), baseTowerChar.GetTypeTower(), baseTowerChar.GetPeopleTower() };

        FeedbackManager feedbackHandler = platform.GetComponent<FeedbackManager>();

        string condition = GetCondition();
        bool conditionMet = (platformValues[conditionNumber] == houseValues[conditionNumber]);
        
        string conditionResult = "";

        if (conditionMet)
        {
            Debug.Log("Condition met!");
            conditionResult = "Match";
            combo++;
            feedbackHandler.Feedback(true);
        }
        else
        {   
            Debug.Log("Condition not met!");
            conditionResult = "Not Match";
            combo = 0;
            feedbackHandler.Feedback(false);
        }
        

        if (combo >= maxCombo)
        {
            Debug.Log("Change condition!");
            ChooseCondition();
            PrintCondition();
            combo = 0;
            PrintCombo();
            categoriesCompleted++;
        }

        if (isTutorial)
        {
            TutorialspawnNextHouse?.Invoke();
            return;
        }
        else
        {
            PrintCombo();

            Log.Information("Check Condition\nCube:\tColor: {ColorNewCube} Type: {TypeNewCube} People: {PeopleNewCube}\nBase:\tColor: {ColorBase} Type: {TypeBase} People: {PeopleBase}\nCondition: {Condition}\nResult: {ConditionResult} ({RealInARow})\n", 
            houseValues[0], houseValues[1], houseValues[2], platformValues[0], platformValues[1], platformValues[2], condition, conditionResult, combo);

            spawnHouse?.Invoke();    

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
            conditionValues = new List<int>() {0, 1, 2};

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
}