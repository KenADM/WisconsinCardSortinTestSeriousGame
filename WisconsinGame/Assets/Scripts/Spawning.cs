using UnityEngine;

public class Spawning : MonoBehaviour
{
    [Header("Prefab da Spawnare")]
    public GameObject prefab; // Il blocco da spawnare
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void Spawn()
    {
        Instantiate(prefab, new Vector3(0, 5, 0), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

