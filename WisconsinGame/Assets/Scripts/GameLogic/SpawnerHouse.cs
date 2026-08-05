using UnityEngine;
using System.Collections.Generic;
using static CubeCombination;
using Serilog;

public class SpawnerHouse : MonoBehaviour

{
    [Header("Position for Spawn")]
    private float x;
    private float y;
    private float z;

    [SerializeField] private Transform spawnPosition = null;

    [Header("Colore")]
    public List<Material> colors = new List<Material>();
    private int colorNumber;
    /*
    0- blue
    1- green
    2- red
    3- yellow
    */

    [Header("Tipo di stanza")]
    public List<GameObject> rooms = new List<GameObject>();
    private int roomNumber;
    /*
    0- Cube
    1- Sphere
    2- Cilinder
    3- Capsule
    */

    [Header("Persone")]
    public int peopleNumberMin;
    public int peopleNumberMax; // Il numero di persone da spawnare

    private static Vector3 houseScale = new Vector3(2f, 2f, 2f);
    private static float houseHeight = 0.6f * houseScale.y; // Houses have a 0.6 height, so we multiply by the scale to get the actual height
    private List<CubeCombination> allCombinations = new List<CubeCombination>();


    void Awake()
    {
        colorNumber = colors.Count;
        roomNumber = rooms.Count;
        x = spawnPosition.position.x;
        y = spawnPosition.position.y;
        z = spawnPosition.position.z;

        FillCombinations();
        //Debug.Log("Total combinations generated: " + allCombinations.Count);
        //SpawnOne(0,0,1);

    }

    void OnEnable()
    {
        GameLogicManager.spawnHouse += Spawn;
        GameLogicManager.spawnStartingHouse += spawnStartingHouseHandler;
    }

    void OnDisable()
    {
        GameLogicManager.spawnHouse -= Spawn;
        GameLogicManager.spawnStartingHouse -= spawnStartingHouseHandler;
    }




    public void GeneratePeople(GameObject spawnedObject, int peopleCount)
    {
        //Debug.Log("Generating " + peopleCount );
        GameObject people = spawnedObject.transform.Find("People").gameObject;
        for (int i = 1; i <= peopleCount; i++)
        {
            people.transform.Find(i.ToString()).gameObject.SetActive(true);
        }
    }

    // Colors the inner walls of a house
    public void ColorRoom(GameObject spawnedObject, Material color)
    {   
        //Debug.Log("Coloring room with color: " + color.name);
        foreach (Transform child in spawnedObject.transform)
        {
            if(child.CompareTag("Wall"))
            {
                foreach (Transform inner in child)
                {
                if(inner.CompareTag("InnerWall"))
                {
                    Renderer rend = inner.GetComponent<Renderer>();
                    
                    if (rend != null) 
                    {
                        rend.material = color;
                    }
                }

                }
            }
        }

    }

    // filled the list of all possible combinations
    void FillCombinations()
    {
        for (int i = peopleNumberMin; i <= peopleNumberMax; i++) {
            for (int j = 0; j < colorNumber; j++) {
                for (int k = 0; k < roomNumber; k++) {
                    allCombinations.Add(new CubeCombination(j, k, i));
                    //Debug.Log("Combination added: People " + i + ", Color " + j + ", Room " + k);
                    
                }
            }
        }
    }

    // given a platform, the starting Houses are spawn
    private void spawnStartingHouseHandler(Transform platform)
    {
     
        float platformHeight = platform.Find("Platform").GetComponent<Transform>().localScale.y;

        
        TowerChar baseTowerChar = platform.GetComponent<TowerChar>();
        int colorIndex = baseTowerChar.GetColorTower();
        int typeIndex = baseTowerChar.GetTypeTower();
        int people = baseTowerChar.GetPeopleTower();

        GameObject spawnedHouse = SpawnOne(colorIndex, typeIndex, people, platform.transform.position.x + 4f, 1f, platform.transform.position.z - 5f);
        
        spawnedHouse.GetComponent<HouseFunctions>().MakeUnmovable();
        spawnedHouse.transform.SetParent(platform);
        spawnedHouse.name = "BaseHouse";
    }

    // Spawn a random house
    public void Spawn()
    {

        if (allCombinations.Count > 0)
        {   
            int index = Random.Range(0, allCombinations.Count);
            CubeCombination randomElement = allCombinations[index];
            allCombinations.RemoveAt(index);
            //Debug.Log("Spawning house with Color: " + randomElement.color + ", Type: " + randomElement.type + ", People: " + randomElement.people);
            SpawnOne(randomElement.color, randomElement.type, randomElement.people, x, y, z);
            //Debug.Log("Done");
        }else{
            Debug.Log("No more combinations available.");
            PlayerLog.CloseLogger();
        } 
    }

    // Spawn a specific house
    public GameObject SpawnOne(int colorIndex, int typeIndex, int people, float FixedX, float FixedY, float FixedZ)
    {
        Debug.Log(colorIndex + " " + typeIndex + " " + people);
        GameObject room =  rooms[typeIndex];
        Material color = colors[colorIndex];
        int numberOfPeople = people;
        allCombinations.RemoveAll(item => item.people == people && item.type == typeIndex && item.color == colorIndex);
        //spawn room
        GameObject spawnedObj = Instantiate(room, new Vector3(FixedX, FixedY, FixedZ), room.transform.rotation);
        spawnedObj.transform.localScale = houseScale;
        //color room
        ColorRoom(spawnedObj,color);
        //spawn people
        GeneratePeople(spawnedObj, numberOfPeople);

        spawnedObj.name = "NewHouse" + colorIndex + " "+ typeIndex + " "+ people + " ";

        HouseChar newHouse = spawnedObj.AddComponent<HouseChar>();
        newHouse.Initialize(colorIndex, typeIndex, people);
        return spawnedObj;
    }
    
    // not used
    public void SpawnTesting()
    {
        
        float dimensions = 0;
        int index = 0;
        foreach(CubeCombination Element in allCombinations)
        {

            GameObject room =  rooms[Element.type];
            Material color = colors[Element.color];
            int numberOfPeople = Element.people;

            //spawn room
            GameObject spawnedObj = Instantiate(room, new Vector3(0+ dimensions*index, 0, 0), Quaternion.identity);

            if (dimensions == 0)
            {
                dimensions = spawnedObj.transform.localScale.x;
            }
                
            //color room
            ColorRoom(spawnedObj,color);
            //spawn people
            GeneratePeople(spawnedObj, numberOfPeople);

            index += 1;
        }
    }

    void Update()
    {
        
    }
}
