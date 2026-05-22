using UnityEngine;
using System.Collections.Generic;
//NOTA BENE, per distringuere la caratteristica del tipo di stanza, si è temporaneamente deciso di utilizzare un TempPrefab
public class Spawning : MonoBehaviour
{
    [Header("Appartamento")]
    public GameObject prefab; // Il blocco da spawnare

    [Header("Persone")]
    public GameObject peoplePrefab; // Il prefab della persona da spawnare
    public int peopleNumber; // Il numero di persone da spawnare

    [Header("Tipo di stanza")]
    public GameObject TempPrefab; // Il prefab dell'oggetto da spawnare
    public int typeNumber; // Il numero di oggetti da spawnare

    [Header("Colore")]
    public List<Material> materials = new List<Material>();
    private int colorNumber;

    List<CombinationBox> allCombinations = new List<CombinationBox>();
    

    void Start()
    {
        colorNumber = materials.Count;

        FillCombinations();

        
        Spawn();
    }

    // CHECK!!
    // 1: numero di persone da spawnare, da 1 a 4
    // 2: posizionamento delle persone, da sistemare
    public void GeneratePeople(GameObject spawnedObject, int peopleCount)
    {
        for (int i = 0; i < peopleCount + 1; i++)
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

    public void GenerateOther(GameObject spawnedObject, int typeCount)
    {
        for (int i = 0; i < typeCount; i++)
        {
            GameObject person = Instantiate(TempPrefab, new Vector3(0,0,0), Quaternion.identity);
            person.transform.SetParent(spawnedObject.transform);

            BoxCollider cubeCollider = spawnedObject.GetComponent<BoxCollider>();
            Transform cubeTransform = spawnedObject.GetComponent<Transform>();
            
            // Calcoliamo il pavimento del blocco considerando la scala
            float SizeHeight =  cubeCollider.size.y * cubeTransform.localScale.y;
            float SizeWidth =  cubeCollider.size.x * cubeTransform.localScale.x;
            
            //fix: fare in modo che non si sovrappongono le persone
            //improvement: cambio altezza delle persone 
            person.transform.localPosition = new Vector3(Random.Range(-SizeWidth*2,SizeWidth*2), SizeHeight, Random.Range(-SizeWidth*2, SizeWidth*2)); // Posizione casuale intorno al blocco, ma sotto di esso
        }
    }

    void FillCombinations()
    {
        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < peopleNumber; j++) {
                for (int k = 0; k < typeNumber; k++) {
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

            Debug.Log(index);

            CombinationBox randomElement = allCombinations[index];

            Debug.Log(randomElement.color);

            allCombinations.RemoveAt(index);

            GameObject spawnedObj = Instantiate(prefab, new Vector3(0.3f, 0.5f, 2f), Quaternion.identity);
            spawnedObj.GetComponent<Renderer>().material = materials[randomElement.color];
            GeneratePeople(spawnedObj, randomElement.people);
            GenerateOther(spawnedObj, randomElement.type);    
        }else{
            Debug.Log("No more combinations available.");
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