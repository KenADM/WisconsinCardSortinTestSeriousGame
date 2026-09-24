using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.XR.Management;
using System.Net;

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
    // Cooldown variables
    private float pickupCooldown = 0.5f;
    private float lastPickupTime = -1f;
    
    private Vector3 targetPos;

    // not used in final version
    [Header("Movement Input")]
    [SerializeField] private InputAction moveAction; 

    // not used in final version
    [Header("Actions Input")]
    [SerializeField] private  InputAction pickupAction; 
    
    // not used in final version
    [Header("Keyboard Fallback Settings")]
    [SerializeField] private Key moveLeftKey = Key.A;
    [SerializeField] private Key moveRightKey = Key.D;
    [SerializeField] private Key keyboardPickupKey = Key.Space;
    private bool canTakeMoveInput = true;

    [Header("Sound")]
    [SerializeField] private CraneSoundSystem craneSoundManager;

    [Header("Piattaforme")]
    [SerializeField] private List<Transform> platforms = new List<Transform>();

    [Header("Position for Spawn")]
    [SerializeField] private Transform spawnPosition = null;
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

    private GameObject hoveredHouse = null;
    private GameObject grabbedHouse = null;

    // game object related
    [SerializeField] private GameObject hook;
    [SerializeField] private GameObject arm;
    [SerializeField] private float maxRotation = 30;

    // useful constants
    private const float JOYSTICK_DEADZONE = 0.5f;
    private const float ARRIVAL_THRESHOLD = 0.001f;
    private const float ROTATION_MULTIPLIER = 50f; 

    // audio
    [SerializeField] private AudioSource leftButton;
    [SerializeField] private AudioSource rightButton;
    [SerializeField] private AudioSource pickupButton;

    [SerializeField] private AudioClip clickSound;

    
    private void Start()
    {
        moveAction.Enable();
        pickupAction.Enable(); 
        
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
        // Listen for player input and translate it into crane movement commands only in debug mode.
        if(debugMode)
        {
            HandleInput();
        }
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
                if(inputX < 0)
                {
                    MoveLeft();
                }
                else
                {
                    MoveRight();
                }
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
    /// Start movement with animation
    /// </summary>
    private IEnumerator SlideCraneRoutine()
    {
        isCraneSliding = true;
        if (craneSoundManager != null) craneSoundManager.PlayMovementSound();

        if (useSmoothMovement)
        {
            while (Vector3.Distance(playerHead.position, targetPos) > ARRIVAL_THRESHOLD)
            {
                playerHead.position = Vector3.MoveTowards(playerHead.position, targetPos, movementSpeed * Time.deltaTime);
                yield return null;
            }
        }

        playerHead.position = targetPos;
        
        isCraneSliding = false;
        if (craneSoundManager != null) craneSoundManager.StopMovementSound();
    }
    
    /// <summary>
    /// Calculates the closest valid platform in the requested direction and sets it as the new target position.
    /// </summary>
    private void MoveToClosest(float direction)
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
            StartCoroutine(SlideCraneRoutine());
        }

        string directionString = direction > 0 ? "Right" : "Left";
        JsonTelemetryLogger.LogMovement(directionString, targetPos.x);
    }

    /// <summary>
    /// Checks if the target position is in the right direction of movement
    /// </summary>
    /// <param name="myX">my position</param>
    /// <param name="targetX">my target position</param>
    /// <param name="direction">-1 left, +1 right</param>
    /// <returns></returns>
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
        pickupButton.PlayOneShot(clickSound, 0.25f);

        if (!allowGrab || isArmMoving) return;

        if (Time.time < lastPickupTime + pickupCooldown) return;

        if (isCraneSliding) 
        {
            Debug.Log("Azione bloccata: La gru è in movimento!");
            return;
        }

        // Record the exact time the action was successfully triggered
        lastPickupTime = Time.time;

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

        // start and target rotation
        Quaternion startRotation = arm.transform.localRotation;
        Quaternion targetDownRotation = startRotation * Quaternion.Euler(maxRotation, 0, 0);

        // rotate until it reaches the target rotation OR until it hoveres a house
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
            // used for tutorial progress
            OnHouseGrabbed?.Invoke();

            int[] grabbedValues = new int[] { houseComp.HouseData.Color, houseComp.HouseData.Type, houseComp.HouseData.People };
            JsonTelemetryLogger.LogHouseInteraction(true, grabbedValues);

            yield return new WaitForSeconds(0.2f);
        }

        // back to position
        while (Quaternion.Angle(arm.transform.localRotation, startRotation) > 0.1f)
        {
            arm.transform.localRotation = Quaternion.RotateTowards(arm.transform.localRotation, startRotation, rotationSpeed * ROTATION_MULTIPLIER * Time.deltaTime);

            // fix house position and rotation   
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
            JsonTelemetryLogger.LogHouseInteraction(false, droppedValues);
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
        leftButton.PlayOneShot(clickSound, 0.25f);
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
        rightButton.PlayOneShot(clickSound, 0.25f);
        if (isArmMoving || isCraneSliding) return;
        Debug.Log("Right");
        if (allowMoveRight) MoveToClosest(1f);
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


    /// <summary>
    /// Set target position.
    /// Used by tutorial to slide to starting position
    /// Starts movement coroutine
    /// </summary>
    /// <param name="newTargetPos">The world position to move the crane to.</param>
    public void MoveToPositionSmoothly(Vector3 newTargetPos)
    {
        targetPos = newTargetPos;
        if (Vector3.Distance(playerHead.position, targetPos) > ARRIVAL_THRESHOLD)
        {
            StartCoroutine(SlideCraneRoutine());
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

    /// <summary>
    /// Clears Platforms
    /// </summary>
    public void ClearPlatforms()
    {
        platforms.Clear();
    }
    
    /// <summary>
    /// Populates plaform list
    /// </summary>
    /// <param name="newPlatforms">new platforms</param>
    public void PopulatePlatform(List<Transform> newPlatforms)
    {
        platforms = newPlatforms;
    } 

    /// <summary>
    /// Sets the spawn position.
    /// Used by Tutorial to inform the crane of the new current spawn platform.
    /// </summary>  
    public void SetSpawnPosition(GameObject spawnPos) 
    { 
        spawnPosition = spawnPos.transform; 
    }

}