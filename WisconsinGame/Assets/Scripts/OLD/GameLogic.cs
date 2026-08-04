using UnityEngine;
using System.Collections.Generic;
using System;
using Serilog;
using static PlayerLogger;


public class GameLogic : MonoBehaviour
{

    public int numberOfCollumns = 4;
    private bool startSpawn = false;
    public int rightComboInARow = 1;
    private int realInARow = 0;
    private List<int> conditionValues = new List<int>() {0, 1, 2};
    private int conditionNumber;
    /*
    Value that determines the condition to be tested between 2 blocks.
    0: same color
    1: same type
    2: same number of people
    */

    private Spawning mySpawningManager;

    public static event System.Action<bool> responsePopUp;
    public static event System.Action startStartingCubeSpawning;


    private void Awake()
    {
        mySpawningManager = GetComponent<Spawning>();
        
    }

    private void OnEnable()
    {   
        StartingMenuManager.startGame += StartGameHandler;
        Align.startingConfiguration += StartingConfigurationHandler;
        Align.spawnNew += SpawnNewHandler;
        Align.checkCondition += CheckConditionHandler;
    }

    private void OnDisable()
    {
        StartingMenuManager.startGame -= StartGameHandler;
        Align.startingConfiguration -= StartingConfigurationHandler;
        Align.spawnNew -= SpawnNewHandler;
        Align.checkCondition -= CheckConditionHandler;
        PlayerLogger.CloseLogger();

    }
    
    private void StartGameHandler(string codiceUtente)
    {
        PlayerLogger.Initialize(codiceUtente);
    
        ChooseCondition();

        startStartingCubeSpawning?.Invoke();
    } 

    private void StartingConfigurationHandler(Align align)
    {   
        mySpawningManager.SpawnOne(align.colorIndex, align.typeIndex, align.people, align.transform.position.x, align.transform.localScale.y+10, align.transform.localScale.z);
    }

    private void SpawnNewHandler()
    {   
        if(numberOfCollumns < 2) 
        {
            startSpawn = true;
            mySpawningManager.Spawn();
        }else{
            numberOfCollumns--;
        }
        

    }

    private void CheckConditionHandler(int colorBase, int typeBase, int peopleBase, Collider collider)
    {
        if(!startSpawn) // blocks the check of the base cube placements
        {
            return;    
        }
        
        int colorNewCube = collider.GetComponent<CubeChar>().GetColor();
        int typeNewCube = collider.GetComponent<CubeChar>().GetTypeCube();
        int peopleNewCube = collider.GetComponent<CubeChar>().GetPeople();
        string condition = "";
        bool conditionMet = false;

        switch (conditionNumber)
        {
            case 0: // same color
                conditionMet = (colorBase == colorNewCube);
                condition = "color";
                break;
            case 1: // same type
                conditionMet = (typeBase == typeNewCube);
                condition = "type";
                break;
            case 2: // same number of people
                conditionMet = (peopleBase == peopleNewCube);
                condition = "people";
                break;
        }

        string conditionResult = "";
        if (conditionMet)
        {

            Debug.Log("Condition met!");

            conditionResult = "Match";

            responsePopUp?.Invoke(true);
            realInARow++;
            if (realInARow >= rightComboInARow)
            {
                Debug.Log("Change condition!");
                ChooseCondition();
                realInARow = 0;
            }

        }
        else
        {   
            conditionResult = "Not Match";
            responsePopUp?.Invoke(false);
            realInARow = 0;
            Debug.Log("Condition not met!");
            
        }

        
        Log.Information("Check Condition\nCube:\tColor: {ColorNewCube} Type: {TypeNewCube} People: {PeopleNewCube}\nBase:\tColor: {ColorBase} Type: {TypeBase} People: {PeopleBase}\nCondition: {Condition}\nResult: {ConditionResult} ({RealInARow})\n", 
        colorNewCube, typeNewCube, peopleNewCube, colorBase, typeBase, peopleBase, condition, conditionResult, realInARow);

        if(conditionMet && realInARow == 0)
        {
            string newCondition = GetCondition();
            Log.Information("New Condition: {Condition}!\n", condition);    
        }
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
                    newCondition = "Color";
                    break;
                case 2:
                    newCondition = "Color";
                    break;
            }
        return newCondition;
    }

    void Start()
    {   
        

    }


}
