using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class CraneMovement : MonoBehaviour
// sistemare il codice
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
        OffsetTowerDistance = this.transform.localPosition.z - spawnPosition.localPosition.z;
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
            {
                inputX = 1f;
            }
            else if (Keyboard.current[moveLeftKey].isPressed ) 
            {
                inputX = -1f;
            }
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
            TogglePickup();
        }
    }
    
    private void MoveToClosest(float direction)
    {
        bool isFacingFront = Mathf.Abs(Mathf.DeltaAngle(this.transform.eulerAngles.y, 0f)) < 1f;
        bool isFacingBack = Mathf.Abs(Mathf.DeltaAngle(this.transform.eulerAngles.y, 180f)) < 1f;
        if(isFacingFront)
        {
            MoveToClosestHandler(direction);
        }
        else if(isFacingBack)
        {
            MoveToClosestHandler(-direction);
        }
    }
    private void MoveToClosestHandler(float direction) // da sistemare a CASA, visualizzare sempre il Preview
    {
        Vector3 spawn = spawnPosition.position;
        Vector3 myPosition = this.transform.position;
        Vector3 closest = this.transform.position;
        Transform closestPlatform = null;

        foreach (Transform nextHop in platforms)
        {
            Vector3 targetPos = nextHop.position;
            
            // Cache the component to avoid calling GetComponent multiple times
            InverseTowerHandler towerHandler = nextHop.gameObject.GetComponent<InverseTowerHandler>();
            
            if (towerHandler != null)
            {
                towerHandler.HidePreviewHouse();
            }
            
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
            InverseTowerHandler closestTowerHandler = closestPlatform.gameObject.GetComponent<InverseTowerHandler>();
            if (closestTowerHandler != null)
            {
                closestTowerHandler.ShowPreviewHouse();
            }
        }

        Vector3 spawnTarget = spawn;

        if (IsCorrectDirection(myPosition.x, spawnTarget.x, direction))
        {
            if (Vector3.Distance(myPosition, spawnTarget) < Vector3.Distance(myPosition, closest) || Vector3.Distance(myPosition, closest) == 0f)
            {   
                if(closestPlatform != null)
                {
                    InverseTowerHandler closestTowerHandler = closestPlatform.gameObject.GetComponent<InverseTowerHandler>();
                    if (closestTowerHandler != null)
                    {
                        closestTowerHandler.HidePreviewHouse();
                    }
                }
                closest = spawnTarget;
            }
        }

        // Modify ONLY the X value of the vrHead, keeping its original Y and Z
        Vector3 newHeadPos = vrHead.position;
        newHeadPos.x = closest.x;
        vrHead.position = newHeadPos;
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

    // --- NEW PUBLIC FUNCTION FOR VR BUTTONS ---
    public void TogglePickup()
    {
        if (!pickupTriggered)
        {
            CranePickup();
        }
        else
        {
            CraneRelease();
        }
    }

    private void CranePickup()
    {
        // All validity checks are now centralized here
        if (hoveredHouse != null)
        {
            HouseChar houseComp = hoveredHouse.GetComponent<HouseChar>();
            
            if (houseComp != null && houseComp.isGrabbable())
            {
                pickupTriggered = true;
                grabbedHouse = hoveredHouse; // Salva la casa che stiamo tenendo
                
                grabbedHouse.transform.SetParent(hook.transform);
                Vector3 newPositionHouse = new Vector3(grabbedHouse.transform.localPosition.x, grabbedHouse.transform.localPosition.y, -0.04f);
                grabbedHouse.transform.localPosition = newPositionHouse; // Posiziona la casa in una posizione predefinita rispetto al gancio
                
                houseComp.MakeUnmovable();
            }
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

    // --- PUBLIC MOVEMENT FUNCTIONS ---
    public void MoveLeft()
    {
        MoveToClosest(-1f);
    }
    
    public void MoveRight()
    {
        MoveToClosest(1f);
    }

    public void setSpawnPosition(GameObject spawnPos)
    {
        spawnPosition = spawnPos.transform;
    }
}