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

    [SerializeField] private Transform spawnPosition = null;

    [Header("Offset")]
    public float OffsetTowerDistance;

    private bool isInputActive = true;
    public bool pickupTriggered = false;

    // hover house ---
    public GameObject hoveredHouse = null;
    private GameObject grabbedHouse = null;
    [SerializeField] private GameObject hook;

    void Start()
    {
        moveAction.Enable();
        pickupAction.Enable(); 
    }

    void Update()
    {
        // --- MOVEMENT LOGIC ---
        float inputX = 0f;
        Vector2 joystick = moveAction.ReadValue<Vector2>();

        if (Mathf.Abs(joystick.x) > 0.5f)
        {
            inputX = joystick.x;
        }
        else if (Keyboard.current != null)
        {
            if (Keyboard.current[moveRightKey].isPressed) 
                inputX = 1f;
            else if (Keyboard.current[moveLeftKey].isPressed) 
                inputX = -1f;
        }

        if (Mathf.Abs(inputX) > 0.5f)
        {
            if (isInputActive)
            {
                MoveToClosest(inputX);
                isInputActive = false; 
            }
        }
        else
        {
            isInputActive = true; 
        }

        // --- PICKUP LOGIC ---
        bool pickupPressedThisFrame = pickupAction.WasPressedThisFrame() || (Keyboard.current != null && Keyboard.current[keyboardPickupKey].wasPressedThisFrame);

        if (pickupPressedThisFrame)
        {
            if (!pickupTriggered)
            {
                // if i have something in my hand and i trigger a pickup
                if (hoveredHouse != null && hoveredHouse.GetComponent<HouseChar>() != null && hoveredHouse.GetComponent<HouseChar>().isGrabbable())
                {
                    CranePickup();
                }
            }
            else
            {
                // if i have something in my hand and i click to pickup, i release it
                CraneRelease();
            }
        }
    }
    
    private void MoveToClosest(float direction) // da sistemare a CASA, visualizzare sempre il Preview
    {
        Vector3 spawn = spawnPosition.position;
        Vector3 myPosition = this.transform.position;
        Vector3 closest =  this.transform.position;
        Transform closestPlatform = null;

        foreach (Transform nextHop in platforms)
        {
            Vector3 targetPos = nextHop.position;
            targetPos.z += OffsetTowerDistance;
            nextHop.gameObject.GetComponent<InverseTowerHandler>().HidePreviewHouse();
            
            if (IsCorrectDirection(myPosition.x, targetPos.x, direction))
            {
                if ((Vector3.Distance(myPosition, targetPos) < Vector3.Distance(myPosition, closest) || Vector3.Distance(myPosition, closest) == 0f))
                {
                    closest = targetPos;
                    closestPlatform = nextHop;
                }
            }            
        }

        if(closestPlatform != null)
        {
            closestPlatform.gameObject.GetComponent<InverseTowerHandler>().ShowPreviewHouse();
        }

        Vector3 spawnTarget = spawn;
        spawnTarget.z += OffsetTowerDistance;

        if (IsCorrectDirection(myPosition.x, spawnTarget.x, direction))
        {
            if (Vector3.Distance(myPosition, spawnTarget) < Vector3.Distance(myPosition, closest) || Vector3.Distance(myPosition, closest) == 0f)
            {   
                if(closestPlatform != null)
                {
                    closestPlatform.gameObject.GetComponent<InverseTowerHandler>().HidePreviewHouse();
                }
                closest = spawnTarget;
            }
        }

        closest.y = myPosition.y; // Mantieni la stessa coordinata x del granchio
        vrHead.position = closest;
    }

    private bool IsCorrectDirection(float myX, float targetX, float direction)
    {
        if (direction > 0) return targetX > myX;
        else if (direction < 0) return targetX < myX;
        return false; 
    }

    public void HoverHouse(GameObject house)
    {
        // Se non abbiamo già una casa in mano, aggiorniamo l'hover
        if (!pickupTriggered) 
        {
            hoveredHouse = house;
        }
    }

    public void UnhoverHouse(GameObject house)
    {
        if (hoveredHouse == house)
        {
            hoveredHouse = null;
        }
    }

    private void CranePickup()
    {
        if (hoveredHouse != null)
        {
            pickupTriggered = true;
            grabbedHouse = hoveredHouse; // Salva la casa che stiamo tenendo
            
            grabbedHouse.transform.SetParent(hook.transform);
            Vector3 newPositionHouse = new Vector3(grabbedHouse.transform.localPosition.x, grabbedHouse.transform.localPosition.y, -0.04f);
            grabbedHouse.transform.localPosition = newPositionHouse; // Posiziona la casa in una posizione predefinita rispetto al gancio
            
            HouseChar houseComp = grabbedHouse.GetComponent<HouseChar>();
            if (houseComp != null) houseComp.MakeUnmovable();
        }
    }

    private void CraneRelease()
    {
        if (grabbedHouse != null)
        {
            pickupTriggered = false;

            grabbedHouse.transform.SetParent(null); 
            
            HouseChar houseComp = grabbedHouse.GetComponent<HouseChar>();
            if (houseComp != null) houseComp.Release();

            grabbedHouse = null; 
        }
    }
}