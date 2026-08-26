using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections; 

public class InverseTowerHandler : MonoBehaviour
{
    [SerializeField] private int blockCount;

    [SerializeField] private Transform housePreview; 
    [SerializeField] private TowerChar matchingChar;
    [SerializeField] private Transform trigger; 
    
    private static Vector3 houseScale = new Vector3(2f, 2f, 2f);
    private static float houseHeight = 0.6f * houseScale.y - 0.05f;     
    
    [SerializeField] private Transform baseHouse;

    public static event Action<GameObject, int[]> checkHousePlacement;
    void Start()
    {
        blockCount = 0;
    }

    public void PlaceHouse(Transform house)
    {
        house.name = "PlacedHouse"; 

        if (blockCount == 0)
        {
            baseHouse = house;
            
            Vector3 correctPosition = new Vector3(housePreview.position.x, housePreview.position.y - 0.6f, housePreview.position.z);
            baseHouse.position = correctPosition;
            
            housePreview.position = new Vector3(housePreview.position.x, housePreview.position.y + houseHeight, housePreview.position.z);
            trigger.position = new Vector3(trigger.position.x, trigger.position.y + houseHeight, trigger.position.z);

            baseHouse.SetParent(this.transform);
            TriggerPlacementLogic(house);
        }
        else
        {
            StartCoroutine(ReplaceHouseSequence(baseHouse, house));
            baseHouse = house;            
        }

        blockCount++;
    }

    private void TriggerPlacementLogic(Transform house)
    {
        if (house.TryGetComponent(out HouseFunction houseChar))
        {
            houseChar.Grab(); 
            houseChar.MakeUnmovable();

            checkHousePlacement?.Invoke(this.gameObject, new int[] { 
                houseChar.HouseData.color, 
                houseChar.HouseData.type, 
                houseChar.HouseData.people 
            });
        }
    }

    private IEnumerator ReplaceHouseSequence(Transform oldHouse, Transform newHouse)
    {
        if (oldHouse != null)
        {
            oldHouse.SetParent(null); 
            if (oldHouse.TryGetComponent(out Gravity oldGravity))
            {
                oldGravity.floorHeight = -10f; 
                oldGravity.gravityForce *= 6f; 
                oldGravity.StartGravity();
            }
        }

        newHouse.SetParent(null); 
        
        newHouse.position = new Vector3(housePreview.position.x, newHouse.position.y, housePreview.position.z);

        if (newHouse.TryGetComponent(out Gravity newGravity))
        {
            newGravity.floorHeight = 0f; 
            newGravity.StartGravity();
        }

        while (newHouse != null && newHouse.position.y > 0.01f)
        {
            yield return null; 
        }

        if (newHouse != null)
        {
            newHouse.position = new Vector3(newHouse.position.x, 0f, newHouse.position.z); 
            newHouse.SetParent(this.transform); 
            
            TriggerPlacementLogic(newHouse);
        }

        // aspetta che la casa cada abbastanza
        yield return new WaitForSeconds(1.5f); 
        
        // la distrugge
        if (oldHouse != null)
        {
            Destroy(oldHouse.gameObject);
        }
    }

    public void ShowPreviewHouse()
    {
        housePreview.gameObject.SetActive(true);
    }

    public void HidePreviewHouse()
    {
        housePreview.gameObject.SetActive(false);
    }
    
    public HouseCombination GetTowerData()
    {
        return matchingChar.TowerData;
    }
}