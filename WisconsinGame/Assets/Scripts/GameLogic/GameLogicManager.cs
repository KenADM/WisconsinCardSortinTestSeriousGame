using UnityEngine;
using System.Collections.Generic;
using System;


public class GameLogicManager : MonoBehaviour
{



    private int combo = 0;
    public int maxCombo = 5;
    private List<int> conditionValues = new List<int>() {0, 1, 2};
    private int conditionNumber;
    public List<Transform> platforms = new List<Transform>();


    public static event Action<Transform, int, int, int> spawnStartingHouse;
    public static event Action spawnHouse;
    

    void OnEnable()
    {
        StartingScreenManager.startGame += StartGameHandler;
    }

    void OnDisable()
    {
        StartingScreenManager.startGame -= StartGameHandler;
    }

    private void StartGameHandler(string codiceUtente, int numOfVariantPerCharacteristic)
    {
        Debug.Log("Starting Game with user code: " + codiceUtente + " and number of variants per characteristic: " + numOfVariantPerCharacteristic);
        ChooseCondition();
        SpawnStartingCubes(numOfVariantPerCharacteristic);

    }

    private void SpawnStartingCubes(int numOfVariantPerCharacteristic)
    {
        Debug.Log("Spawning starting cubes");
        for (int i = 0; i < numOfVariantPerCharacteristic; i++)
        {
            Debug.Log("Spawning color: " + i + ", type: " + i + ", people: " + (i+1) + " at platform: " + platforms[i].name);
            spawnStartingHouse?.Invoke(platforms[i], i, i, i+1);
        }
        spawnHouse?.Invoke();
        
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
}
