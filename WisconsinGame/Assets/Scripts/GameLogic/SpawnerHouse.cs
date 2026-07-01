using UnityEngine;
using System.Collections.Generic;
using static CubeCombination;

public class SpawnerHouse : MonoBehaviour

{
    [Header("Position for Spawn")]
    public float x;
    public float y;
    public float z;

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
    public GameObject peoplePrefab; // Il prefab della persona da spawnare
    public int peopleNumberMin;
    public int peopleNumberMax; // Il numero di persone da spawnare

    private Vector3 scaleCube = new Vector3(2f, 2f, 2f);
    private Vector3 scalePeople = new Vector3(0.1f, 0.1f, 0.1f);


    private List<CubeCombination> allCombinations = new List<CubeCombination>();

    void Awake()
    {
        colorNumber = colors.Count;
        roomNumber = rooms.Count;

        FillCombinations();
        //Debug.Log("Total combinations generated: " + allCombinations.Count);
        //SpawnOne(0,0,1);

    }

    void OnEnable()
    {
        GameLogicManager.spawnHouse += Spawn;
        GameLogicManager.spawnStartingHouse += SpawnStartingCube;
    }

    void OnDisable()
    {
        GameLogicManager.spawnHouse -= Spawn;
        GameLogicManager.spawnStartingHouse -= SpawnStartingCube;
    }


    // 2: posizionamento delle persone, da sistemare
    public void GeneratePeople(GameObject spawnedObject, int peopleCount)
    {
        //Debug.Log("Generating " + peopleCount );
        for (int i = peopleNumberMin; i < peopleCount + 1; i++)
        {
            GameObject person = Instantiate(peoplePrefab, new Vector3(0,0,0), Quaternion.identity);
            person.transform.SetParent(spawnedObject.transform);
            
            person.transform.localPosition = new Vector3(Random.Range(-0.3f,0.3f), -0.45f, Random.Range(-0.3f, 0.3f));
            person.transform.localScale = scalePeople;

        }
    }


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

    private void SpawnStartingCube(Transform platform, int colorIndex, int typeIndex, int people)
    {
        //Debug.Log("yey");
        SpawnOne(colorIndex, typeIndex, people, platform.position.x, 1.2f, platform.position.z);

    }

    public void Spawn()
    {

        if (allCombinations.Count > 0)
        {   
            int index = Random.Range(0, allCombinations.Count);
            CubeCombination randomElement = allCombinations[index];
            allCombinations.RemoveAt(index);

            SpawnOne(randomElement.color, randomElement.type, randomElement.people, x, y, z);

        }else{
            Debug.Log("No more combinations available.");
            PlayerLogger.CloseLogger();
        } 
    }

    public void SpawnOne(int colorIndex, int typeIndex, int people, float FixedX, float FixedY, float FixedZ)
    {
        //Debug.Log(colorIndex + " " + typeIndex + " " + people);
        GameObject room =  rooms[typeIndex];
        Material color = colors[colorIndex];
        int numberOfPeople = people;
        allCombinations.RemoveAll(item => item.people == people && item.type == typeIndex && item.color == colorIndex);
        //spawn room
        GameObject spawnedObj = Instantiate(room, new Vector3(FixedX, FixedY, FixedZ), Quaternion.identity);
        spawnedObj.transform.localScale = scaleCube;
        //color room
        ColorRoom(spawnedObj,color);
        //spawn people
        GeneratePeople(spawnedObj, numberOfPeople);


        HouseChar newHouse = spawnedObj.AddComponent<HouseChar>();
        newHouse.Initialize(typeIndex, colorIndex, people);
    }

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
