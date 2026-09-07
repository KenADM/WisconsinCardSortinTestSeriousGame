using UnityEngine;
using System;
using TMPro;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Manages the step-by-step tutorial sequence (Phases 0-10). 
/// Acts as the "Director" by locking/unlocking player controls, spawning specific practice houses, 
/// and handing over control to the GameLogicManager when the tutorial concludes.
/// </summary>
public class Tutorial : MonoBehaviour
{
    // --- EVENTS ---
    public static event Action<Transform> SpawnStartingHouse;
    public static event Action<int, int, int, float, float, float> SpawnSpecificHouse;
    public static event Action<bool, bool> EnableMoveDirection; 
    public static event Action<bool> EnableGrab; 
    public static event Action StartGame;

    [Header("For Tutorial")]
    [SerializeField] private GameObject spawningPlatformTutorial;
    [SerializeField] private GameObject placingPlatformTutorial;
    [SerializeField] private GameObject tutorialPlatforms;

    [Header("Tutorial Text Prompts (Fasi 0-10) and look at the buttons Prompt")]

    [SerializeField] private GameObject phase0Prompt; // benvenuto
    [SerializeField] private GameObject phase1Prompt; // vai a destra
    [SerializeField] private GameObject phase2Prompt; // 4 fondamenta
    [SerializeField] private GameObject phase3Prompt; // house description
    [SerializeField] private GameObject phase4Prompt; // vai a sinistra
    [SerializeField] private GameObject phase5Prompt; // porta su/giu
    [SerializeField] private GameObject phase6Prompt; // porta dall'altra parte
    [SerializeField] private GameObject phase7Prompt; // nube nera
    [SerializeField] private GameObject phase8Prompt; // riprova
    [SerializeField] private GameObject phase9Prompt; // nube bianca
    [SerializeField] private GameObject phase10Prompt; // Start gioco
    [SerializeField] private GameObject lookAtTheButtonsPrompt; 
    [SerializeField] private GameObject lookAtTimePrompt;

    [Header("Tutorial Triggers")]
    [SerializeField] private GameObject rightTriggerObj; 
    [SerializeField] private GameObject leftTriggerObj;  

    [Header("For Actual Game")]
    [SerializeField] private List<Transform> platformsGameplay = new List<Transform>();
    [SerializeField] private GameObject crane;
    [SerializeField] private GameObject spawningPlatformGameplay;

    private int phase = -1;
    private bool hasSpawnedSecondHouse = false;
    private bool timeMode;

    void Awake()
    {
        phase = -1;
        timeMode = ExperimentSettings.timeMode;
    }
    private void OnEnable()
    {
        StartingScreenManager.TutorialGame += TutorialGameHandler;
        wentRightTriggerTutorial.ReachedRight += ReachedRightTrigger;
        wentLeftTriggerTutorial.ReachedLeft += ReachedLeftTrigger; 
        CraneMovement.OnHouseGrabbed += OnCraneGrab;
        GameLogicManager.TutorialSpawnNextHouse += OnTutorialHousePlaced;
    }

    private void OnDisable()
    {
        StartingScreenManager.TutorialGame -= TutorialGameHandler;
        wentRightTriggerTutorial.ReachedRight -= ReachedRightTrigger;
        wentLeftTriggerTutorial.ReachedLeft -= ReachedLeftTrigger; 
        CraneMovement.OnHouseGrabbed -= OnCraneGrab;
        GameLogicManager.TutorialSpawnNextHouse -= OnTutorialHousePlaced;
    }

    private void Start()
    {
        SpawnStartingHouse?.Invoke(placingPlatformTutorial.transform);
        HideAllPrompts(); 

        if (rightTriggerObj != null) rightTriggerObj.SetActive(false);
        if (leftTriggerObj != null) leftTriggerObj.SetActive(true);
    }

    
    private void Update()
    {
        // Continuously check distance during Phase 9 to trigger the final UI prompt
        if (phase == 9)
        {
            float targetX = spawningPlatformGameplay.transform.position.x;

            if (Mathf.Abs(crane.transform.position.x - targetX) <= 0.05f)
            {
                Vector3 exactPos = new Vector3(targetX, crane.transform.position.y, crane.transform.position.z);
                crane.GetComponent<CraneMovement>().SyncTargetPosition(exactPos);

                SetPhase(10); 
            }
        }
    }


    /// <summary>
    /// Enables the initial welcome panel
    /// </summary>
    private void TutorialGameHandler()
    {
        if (ExperimentSettings.skipTutorial)
        {
            SkipTutorial();
        }
        else
        {
            // Start a coroutine to wait before moving the crane
            StartCoroutine(InitialCraneMoveRoutine());
        }
    }

    /// <summary>
    /// Waits half a second for the game to settle before forcing the initial left movement.
    /// </summary>
    private IEnumerator InitialCraneMoveRoutine()
    {
        // Give the CraneMovement script time to finish its Start() and snap into place
        yield return new WaitForSeconds(0.5f);
        CraneMovement craneMovement = crane.GetComponent<CraneMovement>(); 
        EnableMoveDirection?.Invoke(true, false); 
        craneMovement.MoveLeft();
    }

    /// <summary>
    /// Resets the UI state to ensure overlapping tutorial panels do not occur during transitions.
    /// </summary>
    private void HideAllPrompts()
    {
        if (phase0Prompt) phase0Prompt.SetActive(false); 
        if (phase1Prompt) phase1Prompt.SetActive(false); 
        if (phase2Prompt) phase2Prompt.SetActive(false);
        if (phase3Prompt) phase3Prompt.SetActive(false);
        if (phase4Prompt) phase4Prompt.SetActive(false);
        if (phase5Prompt) phase5Prompt.SetActive(false);
        if (phase6Prompt) phase6Prompt.SetActive(false);
        if (phase7Prompt) phase7Prompt.SetActive(false);
        if (phase8Prompt) phase8Prompt.SetActive(false);
        if (phase9Prompt) phase9Prompt.SetActive(false);
        if (phase10Prompt) phase10Prompt.SetActive(false);
        if (lookAtTheButtonsPrompt) lookAtTheButtonsPrompt.SetActive(false);
        if (lookAtTimePrompt) lookAtTimePrompt.SetActive(false);
    }

    /// <summary>
    /// The core state machine of the tutorial. Enables/disables specific player controls 
    /// (grabbing, movement directions) and displays the relevant instructions for each step.
    /// </summary>
    private void SetPhase(int newPhase)
    {
        phase = newPhase;
        Debug.Log("Tutorial Phase Started: " + phase);

        HideAllPrompts(); 

        switch (phase)
        {
            case 0:
                EnableMoveDirection?.Invoke(false, false); 
                EnableGrab?.Invoke(false);
                if (phase0Prompt) phase0Prompt.SetActive(true); 
                break;
            case 1:
                EnableMoveDirection?.Invoke(false, true); 
                EnableGrab?.Invoke(false);
                if (phase1Prompt) phase1Prompt.SetActive(true); 
                if (lookAtTheButtonsPrompt) lookAtTheButtonsPrompt.SetActive(true);
                if (rightTriggerObj != null) rightTriggerObj.SetActive(true);
                break;

            case 2: 
                EnableMoveDirection?.Invoke(false, false); 
                EnableGrab?.Invoke(false);
                if (phase2Prompt) phase2Prompt.SetActive(true);
                break;

            case 3: 
                EnableMoveDirection?.Invoke(false, false); 
                EnableGrab?.Invoke(false);
                if (phase3Prompt) phase3Prompt.SetActive(true);
                break;

            case 4: 
                SpawnSpecificHouse?.Invoke(1, 1, 1, spawningPlatformTutorial.transform.position.x, spawningPlatformTutorial.transform.position.y + 3f, spawningPlatformTutorial.transform.position.z);
                EnableMoveDirection?.Invoke(true, true); 
                if (phase4Prompt) phase4Prompt.SetActive(true); 
                if (lookAtTheButtonsPrompt) lookAtTheButtonsPrompt.SetActive(true);
                if (leftTriggerObj != null) leftTriggerObj.SetActive(true);
                break;

            case 5: 
                EnableMoveDirection?.Invoke(true, true); 
                EnableGrab?.Invoke(true);
                if (phase5Prompt) phase5Prompt.SetActive(true); 
                if (lookAtTheButtonsPrompt) lookAtTheButtonsPrompt.SetActive(true);
                break;

            case 6: 
                if (phase6Prompt) phase6Prompt.SetActive(true); 
                break;

            case 7:
                if (phase7Prompt) phase7Prompt.SetActive(true); 
                break;

            case 8: 
                if (phase8Prompt) phase8Prompt.SetActive(true); 
                break;

            case 9:
                EnableMoveDirection?.Invoke(true, true); 
                EnableGrab?.Invoke(false); 
                if (phase9Prompt) phase9Prompt.SetActive(true); 
                break;

            case 10: 
                EnableMoveDirection?.Invoke(false, false);
                if(timeMode)
                {
                    if (lookAtTimePrompt) lookAtTimePrompt.SetActive(true);
                }
                else
                {
                    if (phase10Prompt) phase10Prompt.SetActive(true); 
                }
                break;
        }
    }

    // --- FUNCTION FOR IN GAME BUTTONS ---

    /// <summary>
    /// Closes the initial welcome panel and begins the first interactive movement phase.
    /// </summary>
    public void CloseTutorialPanel()
    {
        if (phase0Prompt) phase0Prompt.SetActive(false);
        SetPhase(1); 
    }
    /// <summary>
    /// Triggered when the crane physically touches the right-side boundary box.
    /// </summary>
    private void ReachedRightTrigger()
    {
        if (phase == 1) 
        {
            if (leftTriggerObj != null) leftTriggerObj.SetActive(true);
            SetPhase(2); 
        }
    }

    /// <summary>
    /// Advances the UI after the player acknowledges the ideal apartment preview.
    /// </summary>
    public void ProceedToPhase3()
    {
        if (phase == 2) SetPhase(3);
    }

    /// <summary>
    /// Moves from the extra explanation panel back into active player control.
    /// </summary>
    public void ProceedToPhase4()
    {
        if (phase == 3) SetPhase(4); 
    }

    /// <summary>
    /// Triggered when the crane returns to the left-side spawning platform.
    /// </summary>
    public void ReachedLeftTrigger()
    {
        if (phase == -1) SetPhase(0);
        if (phase == 4) SetPhase(5); 
    }

    /// <summary>
    /// Listens to the CraneMovement script to advance the tutorial immediately upon grabbing a house.
    /// </summary>
    private void OnCraneGrab()
    {
        if (phase == 5) SetPhase(6);
        else if (phase == 7) SetPhase(8);
    }

    /// <summary>
    /// Bypasses all remaining tutorial steps and prepares the crane for gameplay transition.
    /// </summary>
    public void SkipTutorial()
    {
        HideAllPrompts(); 
        phase = 9;
        EnableMoveDirection?.Invoke(true, true); 
        EnableGrab?.Invoke(false); 
        
    }

    /// <summary>
    /// close look at time prompt and show phase 10 prompt.
    /// </summary>
    public void TimeModePromptButton()
    {
        if (lookAtTimePrompt) lookAtTimePrompt.SetActive(false);
        if (phase10Prompt) phase10Prompt.SetActive(true);
    }
    /// <summary>
    /// Triggers the final automated sequence to bridge the tutorial environment and the main gameplay environment.
    /// </summary>
    public void StartGameHandler()
    {
        HideAllPrompts();
        StartCoroutine(SmoothTransitionToGameplay());
    }

    /// <summary>
    /// Listens to the GameLogicManager to confirm a successful practice placement.
    /// </summary>
    private void OnTutorialHousePlaced()
    {
        if (phase == 6)
        {
            // Ensures the second tutorial house is only generated once per session
            if (!hasSpawnedSecondHouse)
            {
                hasSpawnedSecondHouse = true;
                SpawnSpecificHouse?.Invoke(0, 3, 1, spawningPlatformTutorial.transform.position.x, spawningPlatformTutorial.transform.position.y + 3f, spawningPlatformTutorial.transform.position.z);
            }
            SetPhase(7);
        }
        else if (phase == 8)
        {
            SetPhase(9);
        }
    }

    /// <summary>
    /// Automates crane movement toward the spawning gameplay platform.
    /// Upon arrival, it swaps the active platform lists to enable correct crane movement and fires the global StartGame event.
    /// </summary>
    private IEnumerator SmoothTransitionToGameplay()
    {
        // Lock controls during cinematic movement
        EnableMoveDirection?.Invoke(false, false);
        EnableGrab?.Invoke(false);

        // Define target (matching target platform's X/Z but retaining crane's current Y height)
        Vector3 targetPos = new Vector3(
            spawningPlatformGameplay.transform.position.x, 
            crane.transform.position.y, 
            spawningPlatformGameplay.transform.position.z - 8f
        );

        CraneMovement craneMovement = crane.GetComponent<CraneMovement>(); 
        craneMovement.MoveToPositionSmoothly(targetPos);

        // Wait until the physical position matches the target
        while (Vector3.Distance(crane.transform.position, targetPos) > 0.05f)
        {
            yield return null; 
        }

        // Finalize position and swap operational data to gameplay mode
        craneMovement.SyncTargetPosition(targetPos);
        craneMovement.platforms.Clear();
        craneMovement.platforms = platformsGameplay;
        craneMovement.SetSpawnPosition(spawningPlatformGameplay);

        // Restore controls and wake up the GameLogicManager
        EnableMoveDirection?.Invoke(true, true);
        EnableGrab?.Invoke(true);
        StartGame?.Invoke();
    }
}