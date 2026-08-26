using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Rendering;

public class SpawnerHouse : MonoBehaviour
{
    private float x;
    private float y;
    private float z;

    [SerializeField] private Transform spawnPosition = null;

    public List<Material> colors = new List<Material>();
    private int colorNumber;

    public List<GameObject> rooms = new List<GameObject>();
    private int roomNumber;
    public int peopleNumberMin;
    public int peopleNumberMax; 
    [SerializeField] private int maxReshuffles = 2;
    private int reshuffles;
    [SerializeField] private GameObject player;

    private static Vector3 houseScale = new Vector3(2f, 2f, 2f);
    private static float houseHeight = 0.6f * houseScale.y; 
    
    private List<HouseCombination> allCombinations = new List<HouseCombination>();
    private List<HouseCombination> masterDeck = new List<HouseCombination>(); 
    
    public static event System.Action EndCausedByMaxReshuffles;
    void Awake()
    {
        maxReshuffles = ExperimentSettings.maxCombo;
        colorNumber = colors.Count;
        roomNumber = rooms.Count;
        x = spawnPosition.position.x;
        y = spawnPosition.position.y;
        z = spawnPosition.position.z;
        reshuffles = 0;
    }

    void Start()
    {
        FillCombinations();
    }

    void OnEnable()
    {
        GameLogicManager.SpawnHouse += Spawn;
        Tutorial.SpawnStartingHouse += SpawnStartingHouseHandler;
        GameLogicManager.SpawnStartingHouse += SpawnStartingHouseHandler;
    }

    void OnDisable()
    {
        GameLogicManager.SpawnHouse -= Spawn;
        Tutorial.SpawnStartingHouse -= SpawnStartingHouseHandler;
        GameLogicManager.SpawnStartingHouse -= SpawnStartingHouseHandler;
    }

    public void GeneratePeople(GameObject spawnedObject, int peopleCount)
    {
        if(spawnedObject.TryGetComponent(out CustomizeHouse customizer))
        {
            customizer.ShowPeople(peopleCount);
        }

    }

    public void ColorRoom(GameObject spawnedObject, Material color)
    {   
        if(spawnedObject.TryGetComponent(out CustomizeHouse customizer))
        {
            customizer.ColorHouse(color);
        }
    }

    // based on the original test
    // 60 cards where 3 combinations are duplicated
    void FillCombinations()
    {
        allCombinations.Clear();
        masterDeck.Clear();
        List<HouseCombination> tempDeck = new List<HouseCombination>();

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

        // remove 7
        for (int i = 0; i < 7; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, tempDeck.Count);
            tempDeck.RemoveAt(randomIndex);
        }

        // duplicate 3
        List<HouseCombination> duplicates = new List<HouseCombination>();
        for (int i = 0; i < 3; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, tempDeck.Count);
            HouseCombination original = tempDeck[randomIndex];
            duplicates.Add(new HouseCombination(original.type, original.color, original.people));
        }

        masterDeck.AddRange(tempDeck);
        masterDeck.AddRange(duplicates);
        allCombinations.AddRange(masterDeck);
    }

    void RemoveCombination(int colorIndex, int typeIndex, int people)
    {
        int index = allCombinations.FindIndex(item => item.people == people && item.type == typeIndex && item.color == colorIndex);

        if (index != -1)
        {
            allCombinations.RemoveAt(index);
        }
    }

    private void SpawnStartingHouseHandler(Transform platform)
    {       
        TowerChar baseTowerChar = platform.GetComponent<TowerChar>();

        int[] platformValues = new int[] { 
            baseTowerChar.TowerData.color, 
            baseTowerChar.TowerData.type, 
            baseTowerChar.TowerData.people 
        };
        
        GameObject spawnedHouse = SpawnOne(platformValues[0], platformValues[1], platformValues[2], platform.transform.position.x + 4f, 1f, platform.transform.position.z - 5f);
        
        spawnedHouse.GetComponent<HouseFunction>().MakeUnmovable();
        spawnedHouse.transform.SetParent(platform);
        spawnedHouse.name = "BaseHouse";
    }

    public void Spawn()
    {
        if (allCombinations.Count <= 0)
        {   
            if(reshuffles < maxReshuffles)
            {
                allCombinations.AddRange(masterDeck);   
            }
            else
            {
                EndCausedByMaxReshuffles?.Invoke();
                return;
            }
        }
        
        int index = UnityEngine.Random.Range(0, allCombinations.Count); 
        HouseCombination randomElement = allCombinations[index];

        RemoveCombination(randomElement.color, randomElement.type, randomElement.people);
        SpawnOne(randomElement.color, randomElement.type, randomElement.people, x, y, z);
    }

    public GameObject SpawnOne(int colorIndex, int typeIndex, int people, float FixedX, float FixedY, float FixedZ)
    {
        Debug.Log("combinations: " + allCombinations.Count);
        GameObject room = rooms[typeIndex];
        Material color = colors[colorIndex];
        
        GameObject spawnedObj = Instantiate(room, new Vector3(FixedX, FixedY, FixedZ), room.transform.rotation);
        spawnedObj.transform.localScale = houseScale;
        
        ColorRoom(spawnedObj, color);
        GeneratePeople(spawnedObj, people);

        spawnedObj.name = "New House " + colorIndex + " " + typeIndex + " " + people;

        spawnedObj.TryGetComponent<HouseFunction>(out HouseFunction newHouseFunction);
        if (newHouseFunction  != null)
        {
            Debug.Log("shilpe");
            newHouseFunction.Initialize(colorIndex, typeIndex, people);
        }

        spawnedObj.TryGetComponent<WallHiderDecider>(out WallHiderDecider newHouseHider);
        if(newHouseHider != null)
        {
            Debug.Log("shilpe");

            newHouseHider.AssignPlayer(player);
        }

        return spawnedObj;
    }

    public void SetMaxReshuffles(int reshufflesLimit)
    {
        this.maxReshuffles = reshufflesLimit;
        FillCombinations();
    }
}