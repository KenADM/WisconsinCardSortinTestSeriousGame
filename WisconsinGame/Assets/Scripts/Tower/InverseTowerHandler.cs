using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections; 

/// <summary>
/// Manages the physical placement and replacement of houses on a specific tower platform.
/// Handles the cinematic sequence of dropping a new house and discarding the old one, 
/// as well as triggering the placement validation event for the GameLogicManager.
/// </summary>
public class InverseTowerHandler : MonoBehaviour
{
    [Header("Tower Settings")]
    [SerializeField] private int blockCount;
    [SerializeField] private Transform housePreview; 
    [SerializeField] private TowerChar matchingChar;
    [SerializeField] private Transform trigger; 
    
    [Header("Current State")]
    [SerializeField] private Transform baseHouse;

    private static Vector3 houseScale = new Vector3(2f, 2f, 2f);
    private static float houseHeight = 0.6f * houseScale.y - 0.05f;     
    
    // --- EVENTS ---
    // (Kept lowercase to safely match GameLogicManager.cs subscriptions)
    public static event Action<GameObject, int[]> checkHousePlacement;

    private void Start()
    {
        blockCount = 0;
    }

    /// <summary>
    /// Places a new house on the tower. If it is the first house, it snaps to the base position.
    /// If a house already exists, it initiates the replacement sequence to drop the new house and discard the old one.
    /// </summary>
    /// <param name="house">The transform of the house being placed by the crane.</param>
    public void PlaceHouse(Transform house)
    {
        house.name = "PlacedHouse"; 

        if (blockCount == 0)
        {
            baseHouse = house;
            
            Vector3 correctPosition = new Vector3(housePreview.position.x, housePreview.position.y - 0.6f, housePreview.position.z);
            baseHouse.position = correctPosition;
            
            // Shift the preview and trigger upwards for the next potential drop
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

    /// <summary>
    /// Extracts data from the placed house, locks its physical state, and fires 
    /// the validation event for the GameLogicManager (Referee) to evaluate.
    /// </summary>
    /// <param name="house">The transform of the placed house.</param>
    private void TriggerPlacementLogic(Transform house)
    {
        if (house.TryGetComponent(out HouseFunction houseChar))
        {
            houseChar.Grab(); 
            houseChar.MakeUnmovable();

            checkHousePlacement?.Invoke(this.gameObject, new int[] { 
                houseChar.HouseData.Color, 
                houseChar.HouseData.Type, 
                houseChar.HouseData.People 
            });
        }
    }

    /// <summary>
    /// Coroutine that handles the cinematic sequence of the new house falling into place 
    /// while the old house falls through the floor and is destroyed.
    /// </summary>
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

        // Wait until the new house physically reaches the floor
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

        // Wait for the old house to fall far enough out of sight
        yield return new WaitForSeconds(1.5f); 
        
        // Destroy the old house to free up memory
        if (oldHouse != null)
        {
            Destroy(oldHouse.gameObject);
        }
    }

    /// <summary>
    /// Displays the ghostly preview house, indicating this is the closest target.
    /// Called dynamically by the PreviewHouseHandler.
    /// </summary>
    public void ShowPreviewHouse()
    {
        housePreview.gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the ghostly preview house when the crane moves away.
    /// </summary>
    public void HidePreviewHouse()
    {
        housePreview.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// Retrieves the defining characteristics of this specific tower.
    /// </summary>
    public HouseCombination GetTowerData()
    {
        return matchingChar.TowerData;
    }
}