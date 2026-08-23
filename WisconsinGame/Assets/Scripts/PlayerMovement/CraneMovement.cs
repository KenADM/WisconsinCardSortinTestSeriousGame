using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;
using System.Collections; 

public class CraneMovement : MonoBehaviour
{
    [Header("Settings")]
    public float rotationSpeed = 1f;
    public Transform playerHead; 

    [Header("Smooth Movement Settings")]
    public bool useSmoothMovement = true;
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
    public float offsetTowerDistance; 

    // Rinominato per chiarezza: impedisce input multipli dal joystick
    private bool canTakeMoveInput = true;
    public bool isSomethingInHand = false;    
    
    // -- Movement State Flags --
    private bool allowMoveLeft = false;
    private bool allowMoveRight = false;
    private bool allowGrab = false;
    
    private bool isArmMoving = false; 
    // NUOVO FLAG: Traccia se la gru sta scivolando verso una piattaforma
    private bool isCraneSliding = false; 

    // -- Events for Tutorial tracking --
    public static event Action onHouseGrabbed;
    public static event Action onHouseDropped;

    public GameObject hoveredHouse = null;
    private GameObject grabbedHouse = null;
    [SerializeField] private GameObject hook;
    [SerializeField] private GameObject arm;

    [SerializeField] private float maxRotation = 30;

    // --- COSTANTI ---
    private const float JOYSTICK_DEADZONE = 0.5f;
    private const float ARRIVAL_THRESHOLD = 0.001f;
    private const float ROTATION_MULTIPLIER = 50f; 

    void Start()
    {
        moveAction.Enable();
        pickupAction.Enable(); 
        
        if (spawnPosition != null)
        {
            offsetTowerDistance = this.transform.localPosition.z - spawnPosition.localPosition.z;
        }
        
        targetPos = playerHead.position;
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

        if (Mathf.Abs(joystick.x) > JOYSTICK_DEADZONE)
        {
            inputX = joystick.x;
        }
        else if (Keyboard.current != null)
        {
            if (Keyboard.current[moveRightKey].isPressed) inputX = 1f;
            else if (Keyboard.current[moveLeftKey].isPressed) inputX = -1f;
        }

        bool canMove = (inputX > 0 && allowMoveRight) || (inputX < 0 && allowMoveLeft);

        if (canMove && Mathf.Abs(inputX) > JOYSTICK_DEADZONE)
        {
            if (canTakeMoveInput)
            {
                MoveToClosest(inputX);
                canTakeMoveInput = false; 
            }
        }
        else if (Mathf.Abs(inputX) < JOYSTICK_DEADZONE)
        {
            canTakeMoveInput = true; 
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
            if (Vector3.Distance(playerHead.position, targetPos) > ARRIVAL_THRESHOLD)
            {
                // La gru è lontana dal target, quindi sta scivolando!
                isCraneSliding = true;
                playerHead.position = Vector3.MoveTowards(playerHead.position, targetPos, movementSpeed * Time.deltaTime);
            }
            else
            {
                // È arrivata a destinazione!
                isCraneSliding = false;
                playerHead.position = targetPos;
            }
        }
        else
        {
            isCraneSliding = false;
            playerHead.position = targetPos;
        }
    }
    
    private void MoveToClosest(float direction)
    {
        if (isCraneSliding) return; // Non calcolare un nuovo target se stiamo già scivolando

        bool isFacingFront = Mathf.Abs(Mathf.DeltaAngle(this.transform.eulerAngles.y, 0f)) < 1f;
        bool isFacingBack = Mathf.Abs(Mathf.DeltaAngle(this.transform.eulerAngles.y, 180f)) < 1f;
        
        if (isFacingFront) MoveToClosestHandler(direction);
        else if (isFacingBack) MoveToClosestHandler(-direction);
    }
    
    private void MoveToClosestHandler(float direction)
    {
        Vector3 myPosition = targetPos; 
        Vector3 closest = myPosition; 
        float minDistance = Mathf.Infinity; 
        
        foreach (Transform nextHop in platforms)
        {
            if (IsCorrectDirection(myPosition.x, nextHop.position.x, direction))
            {
                float dist = Vector3.Distance(myPosition, nextHop.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closest = nextHop.position;
                }
            }          
        }

        if (spawnPosition != null && IsCorrectDirection(myPosition.x, spawnPosition.position.x, direction))
        {
            float dist = Vector3.Distance(myPosition, spawnPosition.position);
            if (dist < minDistance)
            {
                closest = spawnPosition.position;
            }
        }

        targetPos = new Vector3(closest.x, targetPos.y, targetPos.z);
        
        // Se il nuovo target è diverso dalla posizione attuale, iniziamo a scivolare
        if (Vector3.Distance(playerHead.position, targetPos) > ARRIVAL_THRESHOLD)
        {
            isCraneSliding = true;
        }
    }

    private bool IsCorrectDirection(float myX, float targetX, float direction)
    {
        // Rimosso MOVEMENT_THRESHOLD qui, ora ci affidiamo ad ARRIVAL_THRESHOLD in alto
        if (direction > 0) return targetX > myX + ARRIVAL_THRESHOLD;
        if (direction < 0) return targetX < myX - ARRIVAL_THRESHOLD;
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
        if (!allowGrab || isArmMoving) return;

        // MOLTO PIÙ PULITO! Controlla solo il flag invece di fare calcoli matematici
        if (isCraneSliding) 
        {
            Debug.Log("Azione bloccata: La gru è in movimento!");
            return;
        }

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

        Quaternion startRotation = arm.transform.localRotation;
        Quaternion targetDownRotation = startRotation * Quaternion.Euler(maxRotation, 0, 0);

        while (Quaternion.Angle(arm.transform.localRotation, targetDownRotation) > 0.1f && hoveredHouse == null)
        {
            arm.transform.localRotation = Quaternion.RotateTowards(arm.transform.localRotation, targetDownRotation, rotationSpeed * ROTATION_MULTIPLIER * Time.deltaTime);
            yield return null;
        }

        float lockedWorldZ = 0f;
        Quaternion lockedRotation = Quaternion.identity;

        if (hoveredHouse != null && hoveredHouse.TryGetComponent(out HouseFunction houseComp) && houseComp.isGrabbable())
        {
            isSomethingInHand = true;
            grabbedHouse = hoveredHouse; 
            
            lockedWorldZ = grabbedHouse.transform.position.z;
            lockedRotation = grabbedHouse.transform.rotation;

            grabbedHouse.transform.SetParent(hook.transform, true);
            
            houseComp.Grab();
            onHouseGrabbed?.Invoke();

            yield return new WaitForSeconds(0.2f);
        }

        while (Quaternion.Angle(arm.transform.localRotation, startRotation) > 0.1f)
        {
            arm.transform.localRotation = Quaternion.RotateTowards(arm.transform.localRotation, startRotation, rotationSpeed * ROTATION_MULTIPLIER * Time.deltaTime);
            
            if (grabbedHouse != null)
            {
                grabbedHouse.transform.position = new Vector3(grabbedHouse.transform.position.x, grabbedHouse.transform.position.y, lockedWorldZ);
                grabbedHouse.transform.rotation = lockedRotation;
            }

            yield return null;
        }

        arm.transform.localRotation = startRotation;

        if (grabbedHouse != null)
        {
            grabbedHouse.transform.position = new Vector3(grabbedHouse.transform.position.x, grabbedHouse.transform.position.y, lockedWorldZ);
            grabbedHouse.transform.rotation = lockedRotation;
        }

        isArmMoving = false;
    }

    private void CraneRelease()
    {
        if (grabbedHouse == null) return;

        isSomethingInHand = false;
        grabbedHouse.transform.SetParent(null); 
        
        if (grabbedHouse.TryGetComponent(out HouseFunction houseComp))
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

    public void SetSpawnPosition(GameObject spawnPos) 
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
        if (Vector3.Distance(playerHead.position, targetPos) > ARRIVAL_THRESHOLD) isCraneSliding = true;
    }

    public void SyncTargetPosition(Vector3 newPos)
    {
        targetPos = newPos;
        playerHead.position = newPos;
        isCraneSliding = false;
    }
}