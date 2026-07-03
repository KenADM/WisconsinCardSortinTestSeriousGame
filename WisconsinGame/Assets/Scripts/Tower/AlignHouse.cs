using UnityEngine;
using System.Collections.Generic;
using System;


public class AlignHouse : MonoBehaviour
{
    private int blockCount;

    private Transform trigger;
    private Transform houseStack;
    private Transform HouseCollection;
    private Transform housePreview;
    
    private static Vector3 houseScale = new Vector3(2f, 2f, 2f);
    private static float houseHeight = 0.6f * houseScale.y; // Houses have a 0.6 height, so we multiply by the scale to get the actual height

    private bool isHousePlaced = false;
    private Transform placedHouse;



    private int color;
    private int type;
    private int people;

    public static event Action<int[], int[]> checkHousePlacement;

    // sistemo i nomi che alcuni son minuscoli
    void Start()
    {
        blockCount = 0; // Inizializziamo il contatore dei blocchi allineati a 0
        trigger = transform.Find("Trigger").GetComponent<Transform>(); // Otteniamo il Transform del trigger
        houseStack = transform.Find("HouseStack").GetComponent<Transform>(); // Otteniamo il Transform del stack delle case
        HouseCollection = houseStack.Find("HouseCollection").GetComponent<Transform>(); // Otteniamo il Transform del contenitore delle case nello stack
        housePreview = transform.Find("HousePreview").GetComponent<Transform>();
        TowerChar towerChar = GetComponent<TowerChar>();
        if (towerChar != null)
        {
            color = towerChar.GetColorTower();
            type = towerChar.GetTypeTower();
            people = towerChar.GetPeopleTower();
        }
        else
        {
            Debug.LogError("TowerChar component not found on the GameObject.");
        }
    }

    void snapHouse(Transform house)
    {
        house.transform.rotation = transform.rotation;
        house.transform.position = new Vector3(trigger.position.x, trigger.position.y, trigger.position.z);
    }

    public void PlacedHouse(Collider house) 
    {

        if(house.CompareTag("Grabbable")){
            housePreview.gameObject.SetActive(false);
            snapHouse(house.GetComponent<Transform>());
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
    public void raiseStack()
    {
        Debug.Log("Raising stack");
        housePreview.gameObject.SetActive(true);

        HouseCollection.position = new Vector3(HouseCollection.position.x, HouseCollection.position.y + (houseHeight/2*3), HouseCollection.position.z);
    }

    void AddToStack(Transform house)
    {   
        Debug.Log("Adding house to stack");
        DisableInternalView();
        house.SetParent(HouseCollection);
        placedHouse.GetComponent<HouseFunctions>().MakeUnmovable();
        house.gameObject.name = "PlacedHouse";
        HouseChar houseChar = house.GetComponent<HouseChar>();


        checkHousePlacement?.Invoke(new int[] { this.color, this.type, this.people }, new int[] { houseChar.GetColorHouse(), houseChar.GetTypeHouse(), houseChar.GetPeopleHouse() });
        
        blockCount++;
        isHousePlaced = false;
    }

    public void lowerStack()
    {
        housePreview.gameObject.SetActive(false);

        if(isHousePlaced){
            Debug.Log("Adding placed house to stack");
            HouseCollection.position = new Vector3(HouseCollection.position.x, HouseCollection.position.y - houseHeight/2, HouseCollection.position.z);
            AddToStack(placedHouse);
            
        }else{
            Debug.Log("Lowering stack without adding placed house");
            HouseCollection.position = new Vector3(HouseCollection.position.x, HouseCollection.position.y - (houseHeight/2*3), HouseCollection.position.z);
        }


    }

    public void DisableInternalView()
    {   
        Debug.Log("disable internal view");

        foreach(Transform house in HouseCollection)
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
        return this.color;
    }
    public int GetTypeTower()
    {
        return this.type;
    }
    public int GetPeopleTower()
    {
        return this.people;
    }
    
}
