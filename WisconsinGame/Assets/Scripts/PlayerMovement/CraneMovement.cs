using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;
using System.Collections;
using Unity.VisualScripting;

/// <summary>
/// Controls the physical movement, smooth sliding, and grabbing mechanics of the crane.
/// Responds to Player Input (Joystick/Keyboard) and automated cinematic commands from the Tutorial.
/// </summary>
public class CraneMovement : MonoBehaviour
{
    [Header("Settings")]
    public float rotationSpeed = 1f;
    public Transform playerHead; 
    private bool debugMode = false;

    [Header("Smooth Movement Settings")]
    public bool useSmoothMovement = true;
    public float movementSpeed = 5f; 
    
    private Vector3 targetPos;

    // not used in final version
    [Header("Movement Input")]
    public InputAction moveAction; 

    // not used in final version
    [Header("Actions Input")]
    public InputAction pickupAction; 
    
    // not used in final version
    [Header("Keyboard Fallback Settings")]
    public Key moveLeftKey = Key.A;
    public Key moveRightKey = Key.D;
    public Key keyboardPickupKey = Key.Space;
    private bool canTakeMoveInput = true;

    [Header("Sound")]
    [SerializeField] private CraneSoundSystem craneSoundManager;

    [Header("Piattaforme")]
    public List<Transform> platforms = new List<Transform>();

    [Header("Position for Spawn")]
    [SerializeField] private Transform spawnPosition = null;

    [Header("Tower Related Data")]
    public float offsetTowerDistance; 
    private bool isSomethingInHand = false;    
    
    // Movement State Flags
    private bool allowMoveLeft = false;
    private bool allowMoveRight = false;
    private bool allowGrab = false;
    
    // Animation related flags
    private bool isArmMoving = false; 
    private bool isCraneSliding = false; 

    // Events for Tutorial tracking 
    public static event Action OnHouseGrabbed;
    public static event Action OnHouseDropped;

    public GameObject hoveredHouse = null;
    private GameObject grabbedHouse = null;

    // game object related
    [SerializeField] private GameObject hook;
    [SerializeField] private GameObject arm;
    [SerializeField] private float maxRotation = 30;

    // useful constants
    private const float JOYSTICK_DEADZONE = 0.5f;
    private const float ARRIVAL_THRESHOLD = 0.001f;
    private const float ROTATION_MULTIPLIER = 50f; 

    private void Start()
    {
        moveAction.Enable();
        pickupAction.Enable(); 
        
        if (spawnPosition != null)
        {
            offsetTowerDistance = this.transform.localPosition.z - spawnPosition.localPosition.z;
        }
        
        targetPos = playerHead.position;
    }

    private void OnEnable()
    {
        Tutorial.EnableMoveDirection += EnableMoveDirection;
        Tutorial.EnableGrab += EnableGrab;
        GameLogicManager.EnableMoveDirection += EnableMoveDirection;
        GameLogicManager.EnableGrab += EnableGrab;
    }

    private void OnDisable()
    {
        Tutorial.EnableMoveDirection -= EnableMoveDirection;
        Tutorial.EnableGrab -= EnableGrab;
        GameLogicManager.EnableMoveDirection -= EnableMoveDirection;
        GameLogicManager.EnableGrab -= EnableGrab;        
    }

    private void Update()
    {
        // Listen for player input and translate it into crane movement commands.
        if(debugMode)
        {
            HandleInput();
        }

        // Handle the smooth movement of the crane towards its target position.
        HandleSmoothMovement();
    }
    
    void Awake()
    {
        debugMode = ExperimentSettings.debugMode;
    }

    /// <summary>
    /// Listens for VR Joystick or Keyboard inputs. Translates raw input into 
    /// crane movement commands while respecting tutorial movement locks.
    /// Only if debugMode is enabled, this method will be active. Otherwise, the movement is solely controlled by the buttons in the scene.
    /// </summary>
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
            bool pickupPressedThisFrame = pickupAction.WasPressedThisFrame() || (Keyboard.current != null && Keyboard.current[keyboardPickupKey].wasPressedThisFrame);

            if (pickupPressedThisFrame)
            {
                TogglePickup();
            }
        }
    }

    /// <summary>
    /// Handles the actual physical sliding of the crane towards its target position.
    /// Can toggle between smooth interpolation and instant teleportation.
    /// </summary>
    private void HandleSmoothMovement()
    {
        // if i want to use smooth movements
        if (useSmoothMovement)
        {
            // if the crane is not at the target position, move it closer
            if (Vector3.Distance(playerHead.position, targetPos) > ARRIVAL_THRESHOLD)
            {
                isCraneSliding = true;
                playerHead.position = Vector3.MoveTowards(playerHead.position, targetPos, movementSpeed * Time.deltaTime);
            }
            else
            {
                isCraneSliding = false;
                playerHead.position = targetPos;
                if (craneSoundManager != null) craneSoundManager.StopMovementSound();
            }
        }
        else
        {
            //teleport
            isCraneSliding = false;
            playerHead.position = targetPos;
            if (craneSoundManager != null) craneSoundManager.StopMovementSound();
        }
    }
    
    private void MoveToClosest(float direction)
    {
        Debug.Log("moving");

        string directionString = direction > 0 ? "Right" : "Left";
        JsonTelemetryLogger.LogMovement(directionString, playerHead.position.x);

        bool isFacingFront = Mathf.Abs(Mathf.DeltaAngle(this.transform.eulerAngles.y, 0f)) < 1f;
        bool isFacingBack = Mathf.Abs(Mathf.DeltaAngle(this.transform.eulerAngles.y, 180f)) < 1f;

        if (isFacingFront) MoveToClosestHandler(direction);
        else if (isFacingBack) MoveToClosestHandler(-direction);
    }
    
    /// <summary>
    /// Calculates the closest valid platform in the requested direction and sets it as the new target position.
    /// </summary>
    private void MoveToClosestHandler(float direction)
    {
        Vector3 myPosition = targetPos; 
        Vector3 closest = myPosition; 
        float minDistance = Mathf.Infinity; 
        
        // find the closest platform in the correct direction
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

        // checks if the spawning platform is closer
        if (spawnPosition != null && IsCorrectDirection(myPosition.x, spawnPosition.position.x, direction))
        {
            float dist = Vector3.Distance(myPosition, spawnPosition.position);
            if (dist < minDistance)
            {
                closest = spawnPosition.position;
            }
        }


        // move to closer target
        targetPos = new Vector3(closest.x, targetPos.y, targetPos.z);
        
        if (Vector3.Distance(playerHead.position, targetPos) > ARRIVAL_THRESHOLD)
        {
            isCraneSliding = true;
            if (craneSoundManager != null) craneSoundManager.PlayMovementSound();
        }
    }

    private bool IsCorrectDirection(float myX, float targetX, float direction)
    {
        if (direction > 0) return targetX > myX + ARRIVAL_THRESHOLD;
        if (direction < 0) return targetX < myX - ARRIVAL_THRESHOLD;
        return false; 
    }

    /// <summary>
    /// If GrabbableHouseSensor detects a valid house, this method is called to set it as the currently hovered house.
    /// </summary>
    public void HoverHouse(GameObject house)
    {
        if (!isSomethingInHand) hoveredHouse = house;
    }

    /// <summary>
    /// If GrabbableHouseSensor detects the exit of a valid house in his sensor, it removes it from current hovered house.
    /// </summary>
    public void UnhoverHouse(GameObject house)
    {
        if (hoveredHouse == house) hoveredHouse = null;
    }

    /// <summary>
    /// Blocks Pickup during any movement.
    /// </summary>
    public void TogglePickup()
    {
        if (!allowGrab || isArmMoving) return;

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

    /// <summary>
    /// Crane arm, locking onto a house, and pulling it back up.
    /// </summary>
    private IEnumerator CranePickupRoutine()
    {
        isArmMoving = true;
        if (craneSoundManager != null) craneSoundManager.PlayArmSound();

        Quaternion startRotation = arm.transform.localRotation;
        Quaternion targetDownRotation = startRotation * Quaternion.Euler(maxRotation, 0, 0);

        while (Quaternion.Angle(arm.transform.localRotation, targetDownRotation) > 0.1f && hoveredHouse == null)
        {
            arm.transform.localRotation = Quaternion.RotateTowards(arm.transform.localRotation, targetDownRotation, rotationSpeed * ROTATION_MULTIPLIER * Time.deltaTime);
            yield return null;
        }

        float lockedWorldZ = 0f;
        Quaternion lockedRotation = Quaternion.identity;

        if (hoveredHouse != null && hoveredHouse.TryGetComponent(out HouseFunction houseComp) && houseComp.IsGrabbable())
        {
            isSomethingInHand = true;
            grabbedHouse = hoveredHouse; 
            
            lockedWorldZ = grabbedHouse.transform.position.z;
            lockedRotation = grabbedHouse.transform.rotation;

            grabbedHouse.transform.SetParent(hook.transform, true);
            
            houseComp.Grab();
            OnHouseGrabbed?.Invoke();

            int[] grabbedValues = new int[] { houseComp.HouseData.Color, houseComp.HouseData.Type, houseComp.HouseData.People };
            JsonTelemetryLogger.LogHouseInteraction(true, grabbedValues, playerHead.position.x);

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
        if (craneSoundManager != null) craneSoundManager.StopArmSound();
    }

    /// <summary>
    /// Drops the currently held house and fires the global drop event.
    /// </summary>
    private void CraneRelease()
    {
        if (grabbedHouse == null) return;

        isSomethingInHand = false;
        grabbedHouse.transform.SetParent(null); 
        
        if (grabbedHouse.TryGetComponent(out HouseFunction houseComp))
        {
            int[] droppedValues = new int[] { houseComp.HouseData.Color, houseComp.HouseData.Type, houseComp.HouseData.People };
            JsonTelemetryLogger.LogHouseInteraction(false, droppedValues, playerHead.position.x);
            houseComp.Release();
        }
        
        grabbedHouse = null; 
        OnHouseDropped?.Invoke();
    }

    /// <summary>
    /// Triggers a smooth crane movement to the left.
    /// Used by a movement button ingame.
    /// </summary>   
    public void MoveLeft() 
    { 
        if (isArmMoving || isCraneSliding) return;
        Debug.Log("Left");
        if (allowMoveLeft) MoveToClosest(-1f);
    }

    /// <summary>
    /// Triggers a smooth crane movement to the right.
    /// Used by a movement button ingame.
    /// </summary>   
    public void MoveRight() 
    { 
        if (isArmMoving || isCraneSliding) return;
        Debug.Log("Right");
        if (allowMoveRight) MoveToClosest(1f);
    }

    /// <summary>
    /// Sets the spawn position.
    /// Used by Tutorial to inform the crane of the new current spawn platform.
    /// </summary>  
    public void SetSpawnPosition(GameObject spawnPos) 
    { 
        spawnPosition = spawnPos.transform; 
    }

    /// <summary>
    /// Enables or disables the crane's ability to move left or right based on tutorial progression and at the end of the game.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    private void EnableMoveDirection(bool left, bool right)
    {
        allowMoveLeft = left;
        allowMoveRight = right;
    }

    /// <summary>
    /// Enables or disables the crane's ability to grab houses based on tutorial progression and at the end of the game.
    /// </summary>
    /// <param name="state"></param>
    private void EnableGrab(bool state)
    {
        allowGrab = state;
    }


    // function used by tutorial to move the crane to a specific position without player input.

    /// <summary>
    /// Set target position.
    /// Play movement crane sound if moving.
    /// set isCraneSliding to true if moving.
    /// </summary>
    /// <param name="newTargetPos">The world position to move the crane to.</param>
    public void MoveToPositionSmoothly(Vector3 newTargetPos)
    {
        targetPos = newTargetPos;
        if (Vector3.Distance(playerHead.position, targetPos) > ARRIVAL_THRESHOLD)
        {
            isCraneSliding = true;
            if (craneSoundManager != null) craneSoundManager.PlayMovementSound();
        }
    }

    /// <summary>
    /// Instantly teleports the crane to a specific position and halts any sliding momentum.
    /// </summary>
    /// <param name="newPos">The new position to teleport the crane to.</param>
    public void SyncTargetPosition(Vector3 newPos)
    {
        targetPos = newPos;
        playerHead.position = newPos;
        isCraneSliding = false;
        if (craneSoundManager != null) craneSoundManager.StopMovementSound();
    }
}