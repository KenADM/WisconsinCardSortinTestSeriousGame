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
        bool pickupTriggered = pickupAction.WasPressedThisFrame(); 

        // Keyboard input for pickup (Customizable)
        if (Keyboard.current != null && Keyboard.current[keyboardPickupKey].wasPressedThisFrame)
        {
            pickupTriggered = true;
        }

        // Se il bottone è stato premuto, chiama la funzione
        if (pickupTriggered)
        {
            CranePickup();
        }
    }
    
    private void CranePickup()
    {
        Debug.Log("Funzione CranePickup richiamata!");

        
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
}