using UnityEngine;
using System.Collections.Generic;
using System;


public class AlignHouse : MonoBehaviour
{
    // handles the raise and lower of the house stack and calls GameLogicHandler to check the placement attempt

    private int blockCount;

    // MANCA: controlli per gli errori
    [SerializeField] private Transform houseStack; // house stack gameobject, contains houseCollection
    [SerializeField] private Transform houseCollection; // gameObject containing the actual houses
    [SerializeField] private Transform housePreview; // house used as guide for placement
    [SerializeField] private TowerChar matchingChar;


    [SerializeField] private Transform trigger; // used for house positioning
    private static Vector3 houseScale = new Vector3(2f, 2f, 2f);
    private static float houseHeight = 0.6f * houseScale.y; // Houses have a 0.6 height, so we multiply by the scale to get the actual height

    private bool isHousePlaced = false;
    private Transform placedHouse;

    public static event Action<GameObject, int[]> checkHousePlacement;

    // sistemo i nomi che alcuni son minuscoli
    void Start()
    {
        blockCount = 0;
        /*
        trigger = transform.Find("Trigger").GetComponent<Transform>(); // Otteniamo il Transform del trigger
        houseStack = transform.Find("HouseStack").GetComponent<Transform>(); // Otteniamo il Transform del stack delle case
        houseCollection = houseStack.Find("HouseCollection").GetComponent<Transform>(); // Otteniamo il Transform del contenitore delle case nello stack
        housePreview = transform.Find("HousePreview").GetComponent<Transform>();
        matchingChar = GetComponent<TowerChar>();
        */
    }

    void SnapHouse(Transform house)
    {
        // house.transform.rotation = transform.rotation; // eliminato dato che le case non ruotano
        house.transform.position = new Vector3(trigger.position.x, trigger.position.y, trigger.position.z);
    }

    public void PlacedHouse(Collider house) 
    {

        if(house.CompareTag("Grabbable")){
            housePreview.gameObject.SetActive(false);
            SnapHouse(house.GetComponent<Transform>());
            isHousePlaced = true;
            placedHouse = house.GetComponent<Transform>();
        }

    }

    public void RemovedHouse(Collider house) 
    {

        if(house.CompareTag("Grabbed")){
            Debug.Log("rimosso");
            housePreview.gameObject.SetActive(true);
            isHousePlaced = false;
            placedHouse = null;
        }
    }
    public void RaiseStack()
    {
        Debug.Log("Raising stack");
        housePreview.gameObject.SetActive(true);

        houseCollection.position = new Vector3(houseCollection.position.x, houseCollection.position.y + (houseHeight/2*3), houseCollection.position.z);
    }

    void AddToStack(Transform house)
    {   
        Debug.Log("Adding house to stack");
        DisableInternalView();
        house.SetParent(houseCollection);
        placedHouse.GetComponent<HouseFunctions>().MakeUnmovable();
        house.gameObject.name = "PlacedHouse";
        HouseChar houseChar = house.GetComponent<HouseChar>();


        checkHousePlacement?.Invoke(this.gameObject , new int[] { houseChar.GetColorHouse(), houseChar.GetTypeHouse(), houseChar.GetPeopleHouse() });
        
        blockCount++;
        isHousePlaced = false;
    }

    public void LowerStack()
    {
        housePreview.gameObject.SetActive(false);

        if(isHousePlaced){
            Debug.Log("Adding placed house to stack");
            houseCollection.position = new Vector3(houseCollection.position.x, houseCollection.position.y - houseHeight/2, houseCollection.position.z);
            AddToStack(placedHouse);
            
        }else{
            Debug.Log("Lowering stack without adding placed house");
            houseCollection.position = new Vector3(houseCollection.position.x, houseCollection.position.y - (houseHeight/2*3), houseCollection.position.z);
        }


    }

    public void DisableInternalView()
    {   
        Debug.Log("disable internal view");

        foreach(Transform house in houseCollection)
        {
            Debug.Log("disable internal view house");

            if(house.CompareTag("Aligned")){
                WallHiderDecider whd = house.GetComponent<WallHiderDecider>();
                
                if(whd != null)
                {
                    whd.DeactivateHiding();
                }
            }
        }
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
