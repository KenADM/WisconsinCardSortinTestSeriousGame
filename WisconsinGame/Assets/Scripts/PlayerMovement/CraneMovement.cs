using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;
using System.Collections; // <-- AGGIUNTO per usare IEnumerator

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
    
    // Flag per impedire input multipli mentre il braccio si muove
    private bool isArmMoving = false; 

    // -- Events for Tutorial tracking --
    public static event Action onHouseGrabbed;
    public static event Action onHouseDropped;

    public GameObject hoveredHouse = null;
    private GameObject grabbedHouse = null;
    [SerializeField] private GameObject hook;
    [SerializeField] private GameObject arm;

    [SerializeField] private float maxRotation = 30;

    // --- COSTANTI (Clean Code: niente numeri "magici" sparsi nel codice) ---
    private const float JOYSTICK_DEADZONE = 0.5f;
    private const float ARRIVAL_THRESHOLD = 0.001f;
    private const float MOVEMENT_THRESHOLD = 0.01f;
    private const float HOOK_Z_OFFSET = -0.04f;

    void Start()
    {
        moveAction.Enable();
        pickupAction.Enable(); 
        
        if (spawnPosition != null)
        {
            OffsetTowerDistance = this.transform.localPosition.z - spawnPosition.localPosition.z;
        }
        
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

        // Lettura Input
        if (Mathf.Abs(joystick.x) > JOYSTICK_DEADZONE)
        {
            inputX = joystick.x;
        }
        else if (Keyboard.current != null)
        {
            if (Keyboard.current[moveRightKey].isPressed) inputX = 1f;
            else if (Keyboard.current[moveLeftKey].isPressed) inputX = -1f;
        }

        // Verifica permessi di movimento
        bool canMove = (inputX > 0 && allowMoveRight) || (inputX < 0 && allowMoveLeft);

        if (canMove && Mathf.Abs(inputX) > JOYSTICK_DEADZONE)
        {
            if (isMoving)
            {
                MoveToClosest(inputX);
                isMoving = false; 
            }
        }
        else if (Mathf.Abs(inputX) < JOYSTICK_DEADZONE)
        {
            isMoving = true; 
        }

        // PICKUP LOGIC
        if (allowGrab)
        {       
            bool pickupPressedThisFrame = pickupAction.WasPressedThisFrame() || 
                                          (Keyboard.current != null && Keyboard.current[keyboardPickupKey].wasPressedThisFrame);

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
            if (Vector3.Distance(vrHead.position, targetPos) > ARRIVAL_THRESHOLD)
            {
                vrHead.position = Vector3.MoveTowards(vrHead.position, targetPos, movementSpeed * Time.deltaTime);
            }
        }
        else
        {
            vrHead.position = targetPos;
        }
    }
    
    private void MoveToClosest(float direction)
    {
        if (useSmoothMovement && Vector3.Distance(vrHead.position, targetPos) > MOVEMENT_THRESHOLD)
        {
            return;
        }

        bool isFacingFront = Mathf.Abs(Mathf.DeltaAngle(this.transform.eulerAngles.y, 0f)) < 1f;
        bool isFacingBack = Mathf.Abs(Mathf.DeltaAngle(this.transform.eulerAngles.y, 180f)) < 1f;
        
        if (isFacingFront) MoveToClosestHandler(direction);
        else if (isFacingBack) MoveToClosestHandler(-direction);
    }
    
    private void MoveToClosestHandler(float direction)
    {
        Vector3 spawnTarget = spawnPosition.position;
        Vector3 myPosition = targetPos; 
        Vector3 closest = myPosition; 
        
        // Trova la piattaforma più vicina
        foreach (Transform nextHop in platforms)
        {
            Vector3 targetNextPos = nextHop.position;
            
            if (IsCorrectDirection(myPosition.x, targetNextPos.x, direction))
            {
                if (Vector3.Distance(myPosition, targetNextPos) < Vector3.Distance(myPosition, closest) || Vector3.Distance(myPosition, closest) == 0f)
                {
                    closest = targetNextPos;
                }
            }          
        }

        // Controllo se il punto di spawn è in realtà la scelta migliore
        if (IsCorrectDirection(myPosition.x, spawnTarget.x, direction))
        {
            if (Vector3.Distance(myPosition, spawnTarget) < Vector3.Distance(myPosition, closest) || Vector3.Distance(myPosition, closest) == 0f)
            {   
                closest = spawnTarget;
            }
        }

        // Modifica solo la X del target, mantenendo Y e Z attuali
        targetPos = new Vector3(closest.x, targetPos.y, targetPos.z);
    }

    private bool IsCorrectDirection(float myX, float targetX, float direction)
    {
        if (direction > 0) return targetX > myX + MOVEMENT_THRESHOLD;
        if (direction < 0) return targetX < myX - MOVEMENT_THRESHOLD;
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
        // Blocca la richiesta se il braccio sta già scendendo/salendo o non hai i permessi
        if (!allowGrab || isArmMoving) return;

        // BLOCCO MOVIMENTO: Se la telecamera (vrHead) è ancora lontana dal punto di arrivo (targetPos),

        if (Vector3.Distance(vrHead.position, targetPos) > MOVEMENT_THRESHOLD) 
        {
            Debug.Log("Azione bloccata: La gru è in movimento!");
            return;
        }

        // 3. Esegui la presa o il rilascio
        if (!isSomethingInHand) 
        {
            StartCoroutine(CranePickupRoutine());
        }
        else 
        {
            CraneRelease();   
        }
    }
    private IEnumerator CranePickupRoutine()
    {
        isArmMoving = true;

        // Save Initial Rotation
        Quaternion startRotation = arm.transform.localRotation;
        
        
        Quaternion targetDownRotation = startRotation * Quaternion.Euler(maxRotation, 0, 0);

        // Rotate the arm until it reaches a house/it reaches something
        while (Quaternion.Angle(arm.transform.localRotation, targetDownRotation) > 0.1f && hoveredHouse == null)
        {
            arm.transform.localRotation = Quaternion.RotateTowards(arm.transform.localRotation, targetDownRotation, Rotationspeed * 50f * Time.deltaTime);
            yield return null;
        }

        // if a house is hovered, GRAB
        if (hoveredHouse != null && hoveredHouse.TryGetComponent(out HouseChar houseComp) && houseComp.isGrabbable())
        {
            isSomethingInHand = true;
            grabbedHouse = hoveredHouse; 
            
            grabbedHouse.transform.SetParent(hook.transform);
            Vector3 newPositionHouse = new Vector3(grabbedHouse.transform.localPosition.x, grabbedHouse.transform.localPosition.y, HOOK_Z_OFFSET);
            grabbedHouse.transform.localPosition = newPositionHouse; 
            
            houseComp.MakeUnmovable();
            onHouseGrabbed?.Invoke();

            // necessary pause for grab
            yield return new WaitForSeconds(0.2f);
        }

        // raise
        while (Quaternion.Angle(arm.transform.localRotation, startRotation) > 0.1f)
        {
            arm.transform.localRotation = Quaternion.RotateTowards(arm.transform.localRotation, startRotation, Rotationspeed * 50f * Time.deltaTime);
            yield return null;
        }

        // back to base rotation
        arm.transform.localRotation = startRotation;
        isArmMoving = false;
    }

    private void CraneRelease()
    {
        if (grabbedHouse == null) return;

        isSomethingInHand = false;
        grabbedHouse.transform.SetParent(null); 
        
        if (grabbedHouse.TryGetComponent(out HouseChar houseComp))
        {
            houseComp.Release();
        }
        
        grabbedHouse = null; 
        onHouseDropped?.Invoke();
    }

    public void MoveLeft() 
    { 
        if (allowMoveLeft) MoveToClosest(-1f);
    }

    public void MoveRight() 
    { 
        if (allowMoveRight) MoveToClosest(1f);
    }

    public void setSpawnPosition(GameObject spawnPos) 
    { 
        spawnPosition = spawnPos.transform; 
    }

    void EnableMoveDirection(bool left, bool right)
    {
        allowMoveLeft = left;
        allowMoveRight = right;
    }

    void EnableGrab(bool state)
    {
        allowGrab = state;
    }

    public void MoveToPositionSmoothly(Vector3 newTargetPos)
    {
        targetPos = newTargetPos;
    }

    public void SyncTargetPosition(Vector3 newPos)
    {
        targetPos = newPos;
        vrHead.position = newPos;
    }
}