using UnityEngine;
using System.Collections.Generic;
using System;


public class InverseTowerHandler : MonoBehaviour
{
    // handles the raise and lower of the house stack and calls GameLogicHandler to check the placement attempt
    // FIX!! Devo premere piu volte x rilasciare la casa
    [SerializeField] private int blockCount;

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
    //----
    [SerializeField] private Transform baseHouse;

    //----

    public static event Action<GameObject, int[]> checkHousePlacement;
    public static event Action<GameObject, int[]> checkHousePlacementTutorial;

    // sistemo i nomi che alcuni son minuscoli
    void Start()
    {
        blockCount = 0;
    }


    // aggiungere controllo logico della scelta e spawn del prossimo
    public void PlaceHouse(Transform house)
    {
        if(blockCount == 0)
        {
            Debug.Log("firstHousePlaced");

            // place at the base
            baseHouse = house;
            baseHouse.position = housePreview.position;
            // raise preview
            housePreview.position = new Vector3(housePreview.position.x, housePreview.position.y * 3, housePreview.position.z);
            trigger.position = new Vector3(trigger.position.x, trigger.position.y * 3 + 0.01f, trigger.position.z);
        }
        else
        {
            Debug.Log("SecondHousePlaced");

            // move old top house to bottom
            house.position = baseHouse.position;
            Destroy(baseHouse.gameObject);
            baseHouse=house;            
        }

        house.name = "PlacedHouse"; 
        house.transform.SetParent(this.transform);
        HouseChar houseChar = house.GetComponent<HouseChar>();
        houseChar.MakeUnmovable();

        checkHousePlacement?.Invoke(this.gameObject , new int[] { houseChar.GetColorHouse(), houseChar.GetTypeHouse(), houseChar.GetPeopleHouse() });
        

        blockCount++;
    }
    public void ShowPreviewHouse()
    {
        // show the preview house in the preview position
        housePreview.gameObject.SetActive(true);
    }

    public void HidePreviewHouse()
    {
        // hide the preview house
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
