using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;
using Serilog;
using static PlayerLog;

public class GameLogicManager : MonoBehaviour
{


    private int combo = 0;
    public int maxCombo = 5;
    private List<int> conditionValues = new List<int>() {0, 1, 2};
    private int conditionNumber;
    public List<Transform> platforms = new List<Transform>();

    public TextMeshProUGUI displayCondition;
    public TextMeshProUGUI displayCombo;

    public static event Action<Transform> spawnStartingHouse;
    public static event Action spawnHouse;
    

    void OnEnable()
    {
        StartingScreenManager.startGame += StartGameHandler;
        AlignHouse.checkHousePlacement += CheckHousePlacementHandler;
    }

    void OnDisable()
    {
        StartingScreenManager.startGame -= StartGameHandler;
        AlignHouse.checkHousePlacement -= CheckHousePlacementHandler;
        PlayerLog.CloseLogger();
    }

    private void StartGameHandler(string codiceUtente, int numOfVariantPerCharacteristic)
    {
        Debug.Log("Starting Game with user code: " + codiceUtente + " and number of variants per characteristic: " + numOfVariantPerCharacteristic);
        
        PlayerLog.Initialize(codiceUtente);
        
        ChooseCondition();
        PrintCondition();
        SpawnStartingCubes(numOfVariantPerCharacteristic);

    }

    private void SpawnStartingCubes(int numOfVariantPerCharacteristic)
    {
        Debug.Log("Spawning starting cubes");
        for (int i = 0; i < numOfVariantPerCharacteristic; i++)
        {
            spawnStartingHouse?.Invoke(platforms[i]);
            platforms[i].Find("HouseStack").Find("HouseCollection").Find("Ceiling").gameObject.SetActive(true);
        }
        spawnHouse?.Invoke();
        
    }

    private void CheckHousePlacementHandler(int[] platformValues, int[] houseValues)
    {

        string condition = "";
        bool conditionMet = false;
        Debug.Log("Checking house placement");    
        switch (conditionNumber)
        {
            case 0: // same color
                conditionMet = (platformValues[0] == houseValues[0]);
                condition = "color";
                break;
            case 1: // same type
                conditionMet = (platformValues[1] == houseValues[1]);
                condition = "type";
                break;
            case 2: // same number of people
                conditionMet = (platformValues[2] == houseValues[2]);
                condition = "people";
                break;
        }

        string conditionResult = "";

        if (conditionMet)
        {

            Debug.Log("Condition met!");
            conditionResult = "Match";
            combo++;
        }
        else
        {   
            Debug.Log("Condition not met!");
            conditionResult = "Not Match";
            combo = 0;
            
        }
        PrintCombo();
        Log.Information("Check Condition\nCube:\tColor: {ColorNewCube} Type: {TypeNewCube} People: {PeopleNewCube}\nBase:\tColor: {ColorBase} Type: {TypeBase} People: {PeopleBase}\nCondition: {Condition}\nResult: {ConditionResult} ({RealInARow})\n", 
        houseValues[0], houseValues[1], houseValues[2], platformValues[0], platformValues[1], platformValues[2], condition, conditionResult, combo);

        if (combo >= maxCombo)
        {
            Debug.Log("Change condition!");
            ChooseCondition();
            PrintCondition();
            combo = 0;
            PrintCombo();
        }

        spawnHouse?.Invoke();
    }

    private void PrintCondition(){
        string newCondition = GetCondition();
        Log.Information("New Condition: {Condition}!\n", newCondition); 
        displayCondition.text  = newCondition;
    }

    private void PrintCombo(){
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
