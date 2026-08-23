using UnityEngine;
using System.Collections.Generic;
using System;

public class InverseTowerHandler : MonoBehaviour
{
    [SerializeField] private int blockCount;

    [SerializeField] private Transform housePreview; 
    [SerializeField] private TowerChar matchingChar;
    [SerializeField] private Transform trigger; 
    
    private static Vector3 houseScale = new Vector3(2f, 2f, 2f);
    // Altezza effettiva della casa (1.2 unità)
    private static float houseHeight = 0.6f * houseScale.y; 

    private bool isHousePlaced = false;
    private Transform placedHouse;
    
    [SerializeField] private Transform baseHouse;

    public static event Action<GameObject, int[]> checkHousePlacement;
    public static event Action<GameObject, int[]> checkHousePlacementTutorial;

    void Start()
    {
        blockCount = 0;
    }

    public void PlaceHouse(Transform house)
    {
        if (blockCount == 0)
        {
            Debug.Log("First House Placed");

            baseHouse = house;
            
            // FIX GALLEGGIAMENTO: Sottraiamo 0.6f (la metà dell'altezza) alla Y
            Vector3 correctPosition = new Vector3(housePreview.position.x, housePreview.position.y - 0.6f, housePreview.position.z);
            baseHouse.position = correctPosition;
            
            // FIX MATEMATICO: Invece di moltiplicare per 3 (che crea crescite esponenziali), sommiamo l'altezza
            housePreview.position = new Vector3(housePreview.position.x, housePreview.position.y + houseHeight, housePreview.position.z);
            trigger.position = new Vector3(trigger.position.x, trigger.position.y + houseHeight + 0.01f, trigger.position.z);
        }
        else
        {
            Debug.Log("Second House Placed");

            // Sposta la nuova casa esattamente dove si trovava la vecchia
            house.position = baseHouse.position;
            Destroy(baseHouse.gameObject);
            baseHouse = house;            
        }

        house.name = "PlacedHouse"; 
        house.transform.SetParent(this.transform);
        
        // CLEAN CODE FIX: Usiamo HouseChar per leggere i dati e fermare la gravità
        if (house.TryGetComponent(out HouseFunction houseChar))
        {
            houseChar.Grab(); // Ferma la gravità sfruttando il metodo che abbiamo creato prima!
            houseChar.MakeUnmovable(); // Impedisce che la gru la riprenda per sbaglio
            Debug.Log("5");
            checkHousePlacement?.Invoke(this.gameObject, new int[] { houseChar.GetColorHouse(), houseChar.GetTypeHouse(), houseChar.GetPeopleHouse() });
        }

        blockCount++;
    }

    public void ShowPreviewHouse()
    {
        housePreview.gameObject.SetActive(true);
    }

    public void HidePreviewHouse()
    {
        housePreview.gameObject.SetActive(false);
    }

    public int GetColorTower()
    {
        return matchingChar.GetColorTower();
    }
    
    public int GetTypeTower()
    {
        return matchingChar.GetTypeTower();
    }
    
    public int GetPeopleTower()
    {
        return matchingChar.GetPeopleTower();
    }
}