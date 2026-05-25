using UnityEngine;
using System.Collections.Generic;
//NOTA BENE, per distringuere la caratteristica del tipo di stanza, si è temporaneamente deciso di utilizzare un TempPrefab
public class Spawning : MonoBehaviour
{
    [Header("Position for Spawn")]
    public float x;
    public float y;
    public float z;

    [Header("Persone")]
    public GameObject peoplePrefab; // Il prefab della persona da spawnare
    public int peopleNumberMin;
    public int peopleNumberMax; // Il numero di persone da spawnare

    [Header("Tipo di stanza")]
    public List<GameObject> rooms = new List<GameObject>();
    private int roomNumber;

    [Header("Colore")]
    public List<Material> colors = new List<Material>();
    private int colorNumber;

    private List<CombinationBox> allCombinations = new List<CombinationBox>();


    

    void Start()
    {
        colorNumber = colors.Count;
        roomNumber = rooms.Count;

        FillCombinations();
        //Spawn();
        SpawnTesting();
    }

    // CHECK!!
    // 2: posizionamento delle persone, da sistemare
    public void GeneratePeople(GameObject spawnedObject, int peopleCount)
    {
        for (int i = peopleNumberMin; i < peopleCount + 1; i++)
        {
            GameObject person = Instantiate(peoplePrefab, new Vector3(0,0,0), Quaternion.identity);
            person.transform.SetParent(spawnedObject.transform);

            BoxCollider cubeCollider = spawnedObject.GetComponent<BoxCollider>();
            Transform cubeTransform = spawnedObject.GetComponent<Transform>();
            
            // Calcoliamo il pavimento del blocco considerando la scala
            float SizeHeight =  cubeCollider.size.y * cubeTransform.localScale.y;
            float SizeWidth =  cubeCollider.size.x * cubeTransform.localScale.x;

            
            //fix: fare in modo che non si sovrappongono le persone
            //improvement: cambio altezza delle persone 
            person.transform.localPosition = new Vector3(Random.Range(-SizeWidth*2,SizeWidth*2), -SizeHeight*2, Random.Range(-SizeWidth*2, SizeWidth*2)); // Posizione casuale intorno al blocco, ma sotto di esso
        }
    }

    public void ColorRoom(GameObject spawnedObject, Material color)
    {   

        spawnedObject.GetComponent<Renderer>().material = color;

        // serce a scegliere i figli con Renderder
        Renderer[] allRenderers = spawnedObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer childRenderer in allRenderers)
        {
            childRenderer.material = color;
        }

    }

    void FillCombinations()
    {
        for (int i = peopleNumberMin; i <= peopleNumberMax; i++) {
            for (int j = 0; j < colorNumber; j++) {
                for (int k = 0; k < roomNumber; k++) {
                    allCombinations.Add(new CombinationBox(i, j, k));
                    Debug.Log(i);
                }
            }
        }
    }

    //fix: le coordinate dovranno essere parametri
    public void Spawn()
    {

        if (allCombinations.Count > 0)
        {   
            int index = Random.Range(0, allCombinations.Count);
            CombinationBox randomElement = allCombinations[index];
            allCombinations.RemoveAt(index);

            GameObject room =  rooms[randomElement.type];
            Material color = colors[randomElement.color];
            int numberOfPeople = randomElement.people;

            //spawn room
            GameObject spawnedObj = Instantiate(room, new Vector3(x, y, z), Quaternion.identity);
            //color room
            ColorRoom(spawnedObj,color);
            //spawn people
            GeneratePeople(spawnedObj, numberOfPeople);

        }else{
            Debug.Log("No more combinations available.");
        } 
    }

    public void SpawnTesting()
    {
        
        float dimensions = 0;
        int index = 0;
        foreach(CombinationBox Element in allCombinations)
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
    // Update is called once per frame
    void Update()
    {
        
    }
}

public class CombinationBox
{
    public int people;
    public int type;
    public int color;

    public CombinationBox(int people, int type, int color)
    {
        this.people = people;
        this.type = type;
        this.color = color;
    }
}