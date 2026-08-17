using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;

public class CraneMovement : MonoBehaviour
{
    [Header("Settings")]
    public float Rotationspeed = 1f;
    public Transform vrHead; // La telecamera

    [Header("Smooth Movement Settings")]
    [Tooltip("If false, the crane will instantly teleport (original behavior).")]
    public bool useSmoothMovement = true;
    [Tooltip("How fast the crane moves to the next platform (Units per second).")]
    public float movementSpeed = 5f; 
    
    // -- MODIFICATO: Ora traccia la posizione in 3D (X, Y, Z) --
    private Vector3 targetPos;

    [Header("Movement Input")]
    public InputAction moveAction; 

    [Header("Actions Input")]
    public InputAction pickupAction; 

    [Header("Keyboard Fallback Settings")]
    public Key moveLeftKey = Key.A;
    public Key moveRightKey = Key.D;
    public Key keyboardPickupKey = Key.Space;

    [Header("Piattaforme")]
    public List<Transform> platforms = new List<Transform>();

    [Header("Position for Spawn")]
    [SerializeField] private Transform spawnPosition = null;

    [Header("Offset")]
    public float OffsetTowerDistance;

    private bool isMoving = true;
    public bool isSomethingInHand = false;    
    
    // -- Directional Movement Flags --
    private bool allowMoveLeft = false;
    private bool allowMoveRight = false;
    private bool allowGrab = false;

    // -- Events for Tutorial tracking --
    public static event Action onHouseGrabbed;
    public static event Action onHouseDropped;

    public GameObject hoveredHouse = null;
    private GameObject grabbedHouse = null;
    [SerializeField] private GameObject hook;

    void Start()
    {
        moveAction.Enable();
        pickupAction.Enable(); 
        OffsetTowerDistance = this.transform.localPosition.z - spawnPosition.localPosition.z;
        
        // Initialize the target position to our full 3D starting position
        targetPos = vrHead.position;
    }

    void OnEnable()
    {
        Tutorial.enableMoveDirection += EnableMoveDirection;
        Tutorial.enableGrab += EnableGrab;
    }

    void OnDisable()
    {
        Tutorial.enableMoveDirection -= EnableMoveDirection;
        Tutorial.enableGrab -= EnableGrab;        
    }

    void Update()
    {
        HandleInput();
        HandleSmoothMovement();
    }

    private void HandleInput()
    {
        Vector2 joystick = moveAction.ReadValue<Vector2>();
        float inputX = 0f;

        if (Mathf.Abs(joystick.x) > 0.5f)
        {
            inputX = joystick.x;
        }
        else if (Keyboard.current != null)
        {
            if (Keyboard.current[moveRightKey].isPressed) inputX = 1f;
            else if (Keyboard.current[moveLeftKey].isPressed) inputX = -1f;
        }

        bool canMove = false;
        if (inputX > 0 && allowMoveRight) canMove = true;
        if (inputX < 0 && allowMoveLeft) canMove = true;

        if (canMove && Mathf.Abs(inputX) > 0.5f)
        {
            if (isMoving)
            {
                MoveToClosest(inputX);
                isMoving = false; 
            }
        }
        else if (Mathf.Abs(inputX) < 0.5f)
        {
            isMoving = true; 
        }

        // --- PICKUP LOGIC ---
        if (allowGrab)
        {       
            bool pickupPressedThisFrame = pickupAction.WasPressedThisFrame() || (Keyboard.current != null && Keyboard.current[keyboardPickupKey].wasPressedThisFrame);

            if (pickupPressedThisFrame)
            {
                TogglePickup();
            }
        }
    }

    private void HandleSmoothMovement()
    {
        if (useSmoothMovement)
        {
            // Vector3.Distance controlla se siamo arrivati a destinazione su tutti gli assi
            if (Vector3.Distance(vrHead.position, targetPos) > 0.001f)
            {
                // MoveTowards sposta fluidamente il Vector3 verso il target in 3D
                vrHead.position = Vector3.MoveTowards(vrHead.position, targetPos, movementSpeed * Time.deltaTime);
            }
        }
        else
        {
            // Instant teleportation (Fallback) in 3D
            vrHead.position = targetPos;
        }
    }
    
    private void MoveToClosest(float direction)
    {
        // --- NUOVO CONTROLLO: Blocca il movimento se la gru non è ancora arrivata al target ---
        if (useSmoothMovement && Vector3.Distance(vrHead.position, targetPos) > 0.01f)
        {
            return;
        }

        bool isFacingFront = Mathf.Abs(Mathf.DeltaAngle(this.transform.eulerAngles.y, 0f)) < 1f;
        bool isFacingBack = Mathf.Abs(Mathf.DeltaAngle(this.transform.eulerAngles.y, 180f)) < 1f;
        
        if(isFacingFront) MoveToClosestHandler(direction);
        else if(isFacingBack) MoveToClosestHandler(-direction);
    }
    
    private void MoveToClosestHandler(float direction)
    {
        Vector3 spawn = spawnPosition.position;
        
        // Calculate based on where we are GOING (targetPos)
        Vector3 myPosition = targetPos; 
        Vector3 closest = myPosition; 
        Transform closestPlatform = null;

        foreach (Transform nextHop in platforms)
        {
            Vector3 targetNextPos = nextHop.position;
            InverseTowerHandler towerHandler = nextHop.gameObject.GetComponent<InverseTowerHandler>();
            if (towerHandler != null) towerHandler.HidePreviewHouse();
            
            if (IsCorrectDirection(myPosition.x, targetNextPos.x, direction))
            {
                if ((Vector3.Distance(myPosition, targetNextPos) < Vector3.Distance(myPosition, closest) || Vector3.Distance(myPosition, closest) == 0f))
                {
                    closest = targetNextPos;
                    closestPlatform = nextHop;
                }
            }            
        }

        if(closestPlatform != null)
        {
            InverseTowerHandler closestTowerHandler = closestPlatform.gameObject.GetComponent<InverseTowerHandler>();
            if (closestTowerHandler != null) closestTowerHandler.ShowPreviewHouse();
        }

        Vector3 spawnTarget = spawn;
        if (IsCorrectDirection(myPosition.x, spawnTarget.x, direction))
        {
            if (Vector3.Distance(myPosition, spawnTarget) < Vector3.Distance(myPosition, closest) || Vector3.Distance(myPosition, closest) == 0f)
            {   
                if(closestPlatform != null)
                {
                    InverseTowerHandler closestTowerHandler = closestPlatform.gameObject.GetComponent<InverseTowerHandler>();
                    if (closestTowerHandler != null) closestTowerHandler.HidePreviewHouse();
                }
                closest = spawnTarget;
            }
        }

        // Durante il gameplay normale, cambiamo solo la X del target, mantenendo Y e Z attuali
        targetPos = new Vector3(closest.x, targetPos.y, targetPos.z);
    }

    private bool IsCorrectDirection(float myX, float targetX, float direction)
    {
        if (direction > 0) return targetX > myX + 0.01f;
        else if (direction < 0) return targetX < myX - 0.01f;
        return false; 
    }

    public void HoverHouse(GameObject house)
    {
        if (!isSomethingInHand) hoveredHouse = house;
    }

    public void UnhoverHouse(GameObject house)
    {
        if (hoveredHouse == house) hoveredHouse = null;
    }

    public void TogglePickup()
    {
        if(!allowGrab) return;

        // --- NUOVO CONTROLLO: Blocca la presa/rilascio se la gru si sta muovendo ---
        if (useSmoothMovement && Vector3.Distance(vrHead.position, targetPos) > 0.01f)
        {
            return;
        }

        if (!isSomethingInHand) CranePickup();
        else CraneRelease();
    }

    private void CranePickup()
    {
        if (hoveredHouse != null)
        {
            HouseChar houseComp = hoveredHouse.GetComponent<HouseChar>();
            if (houseComp != null && houseComp.isGrabbable())
            {
                isSomethingInHand = true;
                grabbedHouse = hoveredHouse; 
                
                grabbedHouse.transform.SetParent(hook.transform);
                Vector3 newPositionHouse = new Vector3(grabbedHouse.transform.localPosition.x, grabbedHouse.transform.localPosition.y, -0.04f);
                grabbedHouse.transform.localPosition = newPositionHouse; 
                
                houseComp.MakeUnmovable();
                
                onHouseGrabbed?.Invoke();
            }
        }
    }

    private void CraneRelease()
    {
        if (grabbedHouse != null)
        {
            isSomethingInHand = false;
            grabbedHouse.transform.SetParent(null); 
            
            HouseChar houseComp = grabbedHouse.GetComponent<HouseChar>();
            if (houseComp != null) houseComp.Release();
            grabbedHouse = null; 
            
            onHouseDropped?.Invoke();
        }
    }

    public void MoveLeft() { MoveToClosest(-1f); }
    public void MoveRight() { MoveToClosest(1f); }
    public void setSpawnPosition(GameObject spawnPos) { spawnPosition = spawnPos.transform; }

    void EnableMoveDirection(bool left, bool right)
    {
        allowMoveLeft = left;
        allowMoveRight = right;
    }

    void EnableGrab(bool state)
    {
        allowGrab = state;
    }

    // --- NUOVI METODI PER CONTROLLARE IL MOVIMENTO DALL'ESTERNO (Es. Tutorial) ---
    
    // Usa questo per dire alla gru di muoversi fluidamente verso un nuovo punto in 3D
    public void MoveToPositionSmoothly(Vector3 newTargetPos)
    {
        targetPos = newTargetPos;
    }

    // Usa questo se vuoi teletrasportare la gru istantaneamente
    public void SyncTargetPosition(Vector3 newPos)
    {
        targetPos = newPos;
        vrHead.position = newPos;
    }
}