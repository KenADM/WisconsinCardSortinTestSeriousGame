using UnityEngine;
using System.Collections.Generic;

public class Spawning : MonoBehaviour
{
    [Header("Prefab da Spawnare")]
    public GameObject prefab; // Il blocco da spawnare
    
    public List<Material> Materials = new List<Material>();
    
    void Start()
    {
        
    }

    public void Spawn()
    {
        GameObject spawnedObj = Instantiate(prefab, new Vector3(0.3f, 0.5f, 2f), Quaternion.identity);

        spawnedObj.GetComponent<Renderer>().material = Materials[Random.Range(0, Materials.Count)];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

