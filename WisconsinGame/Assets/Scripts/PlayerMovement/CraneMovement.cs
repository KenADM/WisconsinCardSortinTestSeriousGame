using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class CraneMovement : MonoBehaviour
{
    [Header("Settings")]
    public float Rotationspeed = 1f;
    public Transform vrHead; // La telecamera

    [Header("Movement Input")]
    // serve per indicare quale tasto o joystick vogliamo usare per muoverci.
    public InputAction moveAction; 

    [Header("Actions Input")]
    // serve per indicare quale tasto usare per il pickup
    public InputAction pickupAction; 

    [Header("Keyboard Fallback Settings")]
    // Variables to customize keyboard controls in the Inspector
    public Key moveLeftKey = Key.A;
    public Key moveRightKey = Key.D;
    public Key keyboardPickupKey = Key.Space;

    [Header("Piattaforme")]
    public List<Transform> platforms = new List<Transform>();

    [Header("Position for Spawn")]
    public float x;
    public float y;
    public float z;  

    [Header("Offset")]
    public float OffsetTowerDistance;

    private bool isInputActive = true;
    private bool pickupTriggered = false;

    // hover house ---
    private GameObject hoveredHouse = null;
     [SerializeField] private GameObject hook;

    void Start()
    {
        moveAction.Enable();
        pickupAction.Enable(); 
    }

    void Update()
    {
        float inputX = 0f;

        // joystick input
        Vector2 joystick = moveAction.ReadValue<Vector2>();
        if (Mathf.Abs(joystick.x) > 0.5f)
        {
            inputX = joystick.x;
        }
        // Keyboard input (Customizable)
        else if (Keyboard.current != null)
        {
            // Read from the customizable public variables
            if (Keyboard.current[moveRightKey].isPressed) 
            {
                inputX = 1f;
            }
            else if (Keyboard.current[moveLeftKey].isPressed) 
            {
                inputX = -1f;
            }
        }

        // Apply Movement Logic
        if (Mathf.Abs(inputX) > 0.5f)
        {
            if (isInputActive)
            {
                MoveToClosest(inputX);
                isInputActive = false; // block further input until the joystick is released
            }
        }
        else
        {
            isInputActive = true; 
        }

        // PickUp Logic

        // VR input for pickup
        //bool pickupTriggered = pickupAction.WasPressedThisFrame(); 
        // Keyboard input for pickup 
        if (hoveredHouse != null)
        {
            if (!pickupTriggered && ((Keyboard.current != null && Keyboard.current[keyboardPickupKey].wasPressedThisFrame) || pickupAction.WasPressedThisFrame()))
            {
                pickupTriggered = true;
            }
            else if (pickupTriggered && ((Keyboard.current != null && Keyboard.current[keyboardPickupKey].wasPressedThisFrame) || pickupAction.WasPressedThisFrame()))
            {
                pickupTriggered = false;
            } 
        }


        // Se il bottone è stato premuto, chiama la funzione
        if (pickupTriggered)
        {
            CranePickup();
        }
        else if (!pickupTriggered)
        {
            CraneRelease();
        }

    }
    
    private void MoveToClosest(float direction)
    {
        Vector3 spawn = new Vector3(x, y, z);
        Vector3 myPosition = this.transform.position;
        
        Vector3 closest = this.transform.position;

        foreach (Transform nextHop in platforms)
        {
            // player position 
            Vector3 targetPos = nextHop.position;
            targetPos.z += OffsetTowerDistance;

            // check if targeted object is in the right direction
            if (IsCorrectDirection(myPosition.x, targetPos.x, direction))
            {
                if (Vector3.Distance(myPosition, targetPos) < Vector3.Distance(myPosition, closest) || Vector3.Distance(myPosition, closest) == 0f)
                {
                    closest = targetPos;
                }
            }
        }
        
        Vector3 spawnTarget = spawn;
        spawnTarget.z += OffsetTowerDistance;

        if (IsCorrectDirection(myPosition.x, spawnTarget.x, direction))
        {
            if (Vector3.Distance(myPosition, spawnTarget) < Vector3.Distance(myPosition, closest) || Vector3.Distance(myPosition, closest) == 0f)
            {
                closest = spawnTarget;
            }
        }

        // closest already has the offset applied from the loops above, so we just assign it
        vrHead.position = closest;
    }

    private bool IsCorrectDirection(float myX, float targetX, float direction)
    {
        if (direction > 0) // right
        {
            return targetX > myX;
        }
        else if (direction < 0) // left
        {
            return targetX < myX;
        }
        
        return false; 
    }

    public void HoverHouse(GameObject house)
    {
        Debug.Log("HoverHouse called with: " + house.name);
        hoveredHouse = house;
    }

    public void UnhoverHouse(GameObject house)
    {
        Debug.Log("UnhoverHouse called with: " + house.name);
        hoveredHouse = null;
    }

    private void CranePickup()
    {
        Debug.Log("Funzione CranePickup richiamata!");
        if (hoveredHouse != null)
        {
            Debug.Log("Hovered house is not null, Grabbing: " + hoveredHouse.name);
            hoveredHouse.transform.SetParent(hook.transform);
            hoveredHouse.transform.localPosition = Vector3.zero; // Reset position relative to the hook
            hoveredHouse.GetComponent<HouseChar>().MakeUnmovable(); // Make the house unmovable

        }
        else
        {
            Debug.Log("No hovered house to pick up.");
        }
    }

    private void CraneRelease()
    {
        Debug.Log("Funzione CraneRelease richiamata!");
        if (hoveredHouse != null)
        {
            Debug.Log("Hovered house is not null, Releasing: " + hoveredHouse.name);
            hoveredHouse.transform.SetParent(null); // Detach from the hook
            hoveredHouse.GetComponent<HouseChar>().Release(); // Make the house movable again
            hoveredHouse = null; // Clear the reference to the released house
        }
        else
        {
            Debug.Log("No hovered house to release.");
        }
    }

}