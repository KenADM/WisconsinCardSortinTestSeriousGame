using UnityEngine;
using System.Collections.Generic;
using System;
using SpecialObjects;

public class GameLogic : MonoBehaviour
{

    public int numberOfCollumns = 4;
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

    public static event Action<bool> responsePopUp;


    private void Awake()
    {
        mySpawningManager = GetComponent<Spawning>();
    }

    private void OnEnable()
    {   
        Align.startingConfiguration += StartingConfigurationHandler;
        Align.spawnNew += SpawnNewHandler;
        Align.checkCondition += CheckConditionHandler;
    }

    private void OnDisable()
    {
        Align.startingConfiguration -= StartingConfigurationHandler;
        Align.spawnNew -= SpawnNewHandler;
        Align.checkCondition -= CheckConditionHandler;
    }
    
    private void StartingConfigurationHandler(Align align)
    {   
        mySpawningManager.SpawnOne(align.colorIndex, align.typeIndex, align.people, align.transform.position.x, align.transform.localScale.y+10, align.transform.localScale.z);
    }

    private void SpawnNewHandler()
    {   
        if(numberOfCollumns < 2) // spawn 1st block to fix only at the placement of the last base cube
        {
            mySpawningManager.Spawn();
        }else{
            numberOfCollumns--;
        }
        

    }



    private void CheckConditionHandler(int colorIndex, int typeIndex, int people, Collider collider)
    {
        if(numberOfCollumns > 0) // blocks the check of the base cube placements
        {
            return;    
        }
           

        bool conditionMet = false;

        switch (conditionNumber)
        {
            case 0: // same color
                conditionMet = (colorIndex == collider.GetComponent<CubeChar>().GetColor());
                break;
            case 1: // same type
                conditionMet = (typeIndex == collider.GetComponent<CubeChar>().GetTypeCube());
                break;
            case 2: // same number of people
                conditionMet = (people == collider.GetComponent<CubeChar>().GetPeople());
                break;
        }

        if (conditionMet)
        {
            Debug.Log("Condition met!");
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
            
            responsePopUp?.Invoke(false);
            realInARow = 0;
            Debug.Log("Condition not met!");
            
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
                case 0: // same color
                    Debug.Log("Condition: Color!");
                    break;
                case 1: // same type
                    Debug.Log("Condition: Type!");
                    break;
                case 2: // same number of people
                    Debug.Log("Condition: People!");
                    break;
            }

        conditionValues.RemoveAt(randomIndex);

    }
    void Start()
    {   
        ChooseCondition();
        

    }


}
