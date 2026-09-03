using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Rendering;

/// <summary>
/// Manages the generation, customization, and spawning of houses. 
/// Builds and maintains a "deck" of house combinations mimicking the Wisconsin Card Sorting Test,
/// handling the logic for reshuffling and tracking maximum attempts.
/// </summary>
public class SpawnerHouse : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnPosition = null;
    [SerializeField] private GameObject player;

    [Header("House Characteristics")]
    [SerializeField] private List<Material> colors = new List<Material>();
    [SerializeField] private List<GameObject> rooms = new List<GameObject>();
    [SerializeField] private int peopleNumberMin;
    [SerializeField] private int peopleNumberMax; 
    
    [Header("Game Flow Constraints")]
    [SerializeField] private int maxReshuffles = 2;
    private bool timeMode ;

    private float x;
    private float y;
    private float z;

    private int colorNumber;
    private int roomNumber;
    private int reshuffles;

    private static Vector3 houseScale = new Vector3(2f, 2f, 2f);
    private static float houseHeight = 0.6f * houseScale.y; 
    
    private List<HouseCombination> allCombinations = new List<HouseCombination>();
    private List<HouseCombination> masterDeck = new List<HouseCombination>(); 
    
    // --- EVENTS ---
    public static event System.Action EndCausedByMaxReshuffles;

    private void Awake()
    {
        maxReshuffles = ExperimentSettings.maxCombo;
        colorNumber = colors.Count;
        roomNumber = rooms.Count;
        timeMode = ExperimentSettings.timeMode;

        if (spawnPosition != null)
        {
            x = spawnPosition.position.x;
            y = spawnPosition.position.y;
            z = spawnPosition.position.z;
        }

        reshuffles = 0;
    }

    private void Start()
    {
        FillCombinations();
    }

    private void OnEnable()
    {
        GameLogicManager.SpawnHouse += Spawn;
        Tutorial.SpawnStartingHouse += SpawnStartingHouseHandler;
        Tutorial.SpawnSpecificHouse += SpawnSpecificHouseHandler;
        GameLogicManager.SpawnStartingHouse += SpawnStartingHouseHandler;
    }

    private void OnDisable()
    {
        GameLogicManager.SpawnHouse -= Spawn;
        Tutorial.SpawnStartingHouse -= SpawnStartingHouseHandler;
        Tutorial.SpawnSpecificHouse -= SpawnSpecificHouseHandler;
        GameLogicManager.SpawnStartingHouse -= SpawnStartingHouseHandler;
    }

    /// <summary>
    /// Applies the correct number of people (characters) to a spawned house.
    /// </summary>
    public void GeneratePeople(GameObject spawnedObject, int peopleCount)
    {
        if (spawnedObject.TryGetComponent(out CustomizeHouse customizer))
        {
            customizer.ShowPeople(peopleCount);
        }
    }

    /// <summary>
    /// Applies the correct material color to a spawned house.
    /// </summary>
    public void ColorRoom(GameObject spawnedObject, Material color)
    {   
        if (spawnedObject.TryGetComponent(out CustomizeHouse customizer))
        {
            customizer.ColorHouse(color);
        }
    }

    /// <summary>
    /// Builds the master deck of house combinations based on the original psychological test constraints.
    /// Generates all permutations, removes 7 random cards, and duplicates 3 to create a specific 60-card deck.
    /// </summary>
    private void FillCombinations()
    {
        allCombinations.Clear();
        masterDeck.Clear();
        List<HouseCombination> tempDeck = new List<HouseCombination>();

        // 1. Generate all possible combinations
        for (int i = peopleNumberMin; i <= peopleNumberMax; i++)  
        {
            for (int j = 0; j < colorNumber; j++) 
            {
                for (int k = 0; k < roomNumber; k++) 
                {
                    tempDeck.Add(new HouseCombination(k, j, i));
                }
            }
        }

        // 2. Remove 7 random combinations
        for (int i = 0; i < 7; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, tempDeck.Count);
            tempDeck.RemoveAt(randomIndex);
        }

        // 3. Duplicate 3 random combinations
        List<HouseCombination> duplicates = new List<HouseCombination>();
        for (int i = 0; i < 3; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, tempDeck.Count);
            HouseCombination original = tempDeck[randomIndex];
            duplicates.Add(new HouseCombination(original.Type, original.Color, original.People));
        }

        // 4. Finalize the master deck
        masterDeck.AddRange(tempDeck);
        masterDeck.AddRange(duplicates);
        allCombinations.AddRange(masterDeck);
    }

    /// <summary>
    /// Removes a specific combination from the active deck so it cannot be drawn again.
    /// </summary>
    private void RemoveCombination(int colorIndex, int typeIndex, int people)
    {
        int index = allCombinations.FindIndex(item => item.People == people && item.Type == typeIndex && item.Color == colorIndex);

        if (index != -1)
        {
            allCombinations.RemoveAt(index);
        }
    }

    /// <summary>
    /// Spawns the static base house on a given tower platform based on the tower's defined characteristics.
    /// </summary>
    private void SpawnStartingHouseHandler(Transform platform)
    {       
        Debug.Log(platform.name);
        TowerChar baseTowerChar = platform.GetComponent<TowerChar>();
        
        if (baseTowerChar != null)
        {
            int[] platformValues = new int[] { 
                baseTowerChar.TowerData.Color, 
                baseTowerChar.TowerData.Type, 
                baseTowerChar.TowerData.People 
            };

            // Pass the position directly as a Vector3
            GameObject spawnedHouse = SpawnOne(
                platformValues[0], 
                platformValues[1], 
                platformValues[2], 
                baseTowerChar.HouseBaseSpawn.transform.position.x,
                baseTowerChar.HouseBaseSpawn.transform.position.y,
                baseTowerChar.HouseBaseSpawn.transform.position.z
            );
            
            spawnedHouse.GetComponent<HouseFunction>().MakeUnmovable();
            spawnedHouse.transform.SetParent(platform);
            spawnedHouse.name = "BaseHouse";
        }
    }

    /// <summary>
    /// Draws a random house combination from the active deck and spawns it at the default spawn point.
    /// Triggers a reshuffle if the deck is empty, or ends the game if max reshuffles are reached.
    /// </summary>
    public void Spawn()
    {
        if (allCombinations.Count <= 0)
        {   
            // if its in timeMode, reshuffles are always possible
            if (timeMode || reshuffles < maxReshuffles)
            {
                allCombinations.AddRange(masterDeck);   
                reshuffles++; // Increment added logically here, though original didn't explicitly increment, it represents standard reshuffle logic.
            }
            else
            {
                EndCausedByMaxReshuffles?.Invoke();
                return;
            }
        }
        
        int index = UnityEngine.Random.Range(0, allCombinations.Count); 
        HouseCombination randomElement = allCombinations[index];

        RemoveCombination(randomElement.Color, randomElement.Type, randomElement.People);
        SpawnOne(randomElement.Color, randomElement.Type, randomElement.People, x, y + 3f, z);
    }

    /// <summary>
    /// Instantiates and configures a single house object based on the provided combination parameters.
    /// </summary>
    private GameObject SpawnOne(int colorIndex, int typeIndex, int people, float fixedX, float fixedY, float fixedZ)
    {
        Debug.Log("combinations: " + allCombinations.Count);
        GameObject room = rooms[typeIndex];
        Material color = colors[colorIndex];
        
        GameObject spawnedObj = Instantiate(room, new Vector3(fixedX, fixedY, fixedZ), room.transform.rotation);
        spawnedObj.transform.localScale = houseScale;
        
        ColorRoom(spawnedObj, color);
        GeneratePeople(spawnedObj, people);

        spawnedObj.name = "New House " + colorIndex + " " + typeIndex + " " + people;

        if (spawnedObj.TryGetComponent<HouseFunction>(out HouseFunction newHouseFunction))
        {
            newHouseFunction.Initialize(colorIndex, typeIndex, people);
        }

        if (spawnedObj.TryGetComponent<WallHiderDecider>(out WallHiderDecider newHouseHider))
        {
            newHouseHider.AssignPlayer(player);
        }

        return spawnedObj;
    }

    /// <summary>
    /// Spawns a house with a specific combination at a specific location. Used primarily by the Tutorial system.
    /// </summary>
    private void SpawnSpecificHouseHandler(int colorIndex, int typeIndex, int people, float specificX, float specificY, float specificZ)
    {
        SpawnOne(colorIndex, typeIndex, people, specificX, specificY, specificZ);
    }
}