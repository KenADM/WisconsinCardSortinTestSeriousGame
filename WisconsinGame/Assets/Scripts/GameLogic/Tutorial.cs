using UnityEngine;
using System;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel; // Pannello iniziale di benvenuto (Fase 0)
    private int numOfVariantPerCharacteristic = 4;
    string userCode = "testUser";

    // Questo è l'evento che "griderà" al resto del gioco che il tutorial è finito
    public static event Action<string, int> startGame;

    public SpawnerHouse spawnerHouse;

    [Header("For Tutorial")]
    [SerializeField] private GameObject spawningPlatformTutorial;
    [SerializeField] private GameObject placingPlatformTutorial;
    [SerializeField] private GameObject TutorialPlatforms;

    [Header("Tutorial Text Prompts (Fasi 1-9)")]
    [SerializeField] private GameObject phase0Prompt; // Muoviti a destra
    [SerializeField] private GameObject phase1Prompt; // Muoviti a destra
    [SerializeField] private GameObject phase2Prompt; // Mostra appartamento ideale e bottone
    [SerializeField] private GameObject phase3Prompt; // Muoviti a sinistra
    [SerializeField] private GameObject phase4Prompt; // Raccogli la prima casa
    [SerializeField] private GameObject phase5Prompt; // Rilascia la prima casa
    [SerializeField] private GameObject phase6Prompt; // Raccogli la seconda casa
    [SerializeField] private GameObject phase7Prompt; // Rilascia la seconda casa
    [SerializeField] private GameObject phase8Prompt; // Muoviti a sinistra finale
    [SerializeField] private GameObject phase9Prompt; // Bottone "Inizia Gioco"

    [Header("Tutorial Triggers")]
    [SerializeField] private GameObject rightTriggerObj; 
    [SerializeField] private GameObject leftTriggerObj;  

    public static event System.Action<Transform> spawnStartingHouse;
    public static event System.Action<bool, bool> enableMoveDirection; 
    public static event System.Action<bool> enableGrab; 

    [Header("For Actual Game")]
    public List<Transform> platformsGameplay = new List<Transform>();
    [SerializeField] private GameObject crane;
    public GameObject spawningPlatformGameplay;

    private int phase = 0;

    // Variabile per evitare lo spawn multiplo della seconda casa
    private bool hasSpawnedSecondHouse = false; 

    void OnEnable()
    {
        StartingScreenManager.tutorialGame += TutorialGameHandler;

        wentRightTriggerTutorial.reachedRight += ReachedRightTrigger;
        wentLeftTriggerTutorial.reachedLeft += ReachedLeftTrigger; 

        CraneMovement.onHouseGrabbed += OnCraneGrab;

        // Iscrizione all'evento di piazzamento corretto inviato dal Manager
        GameLogicManager.tutorialspawnNextHouse += OnTutorialHousePlaced;
    }

    void OnDisable()
    {
        StartingScreenManager.tutorialGame -= TutorialGameHandler;

        wentRightTriggerTutorial.reachedRight -= ReachedRightTrigger;
        wentLeftTriggerTutorial.reachedLeft -= ReachedLeftTrigger; 

        CraneMovement.onHouseGrabbed -= OnCraneGrab;

        // Disiscrizione all'evento
        GameLogicManager.tutorialspawnNextHouse -= OnTutorialHousePlaced;
    }

    void Start()
    {
        spawnStartingHouse?.Invoke(placingPlatformTutorial.transform);
        HideAllPrompts(); 

        if(rightTriggerObj != null) rightTriggerObj.SetActive(false);
        if(leftTriggerObj != null) leftTriggerObj.SetActive(false);
    }

    void Update()
    {
        if (phase == 8)
        {
            float targetX = spawningPlatformGameplay.transform.position.x;

            if (Mathf.Abs(crane.transform.position.x - targetX) <= 0.05f)
            {
                Vector3 exactPos = new Vector3(targetX, crane.transform.position.y, crane.transform.position.z);
                crane.GetComponent<CraneMovement>().SyncTargetPosition(exactPos);

                SetPhase(9); 
            }
        }
    }

    private void TutorialGameHandler(string codiceUtente, int numOfVariantPerCharacteristic)
    {
        userCode = codiceUtente;
        if (tutorialPanel) tutorialPanel.SetActive(true);
    }

    public void CloseTutorialPanel()
    {
        if (tutorialPanel) tutorialPanel.SetActive(false);
        SetPhase(1); 
    }

    // Spegne tutti i prompt di tutte le fasi
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
    }

    private void SetPhase(int newPhase)
    {
        phase = newPhase;
        Debug.Log("Tutorial Phase Started: " + phase);

        // at the start of all phases, hide all pannels
        HideAllPrompts(); 

        // activate right pannel
        switch (phase)
        {
            case 1:
                enableMoveDirection?.Invoke(false, true); 
                enableGrab?.Invoke(false);
                if (phase1Prompt) phase1Prompt.SetActive(true); 

                if(rightTriggerObj != null) rightTriggerObj.SetActive(true);
                break;

            case 2: 
                enableMoveDirection?.Invoke(false, false); 
                enableGrab?.Invoke(false);
                if (phase2Prompt) phase2Prompt.SetActive(true);
                break;

            case 3:
                spawnerHouse.SpawnOne(0, 0, 1, spawningPlatformTutorial.transform.position.x, spawningPlatformTutorial.transform.position.y, spawningPlatformTutorial.transform.position.z);

                enableMoveDirection?.Invoke(true, false); 
                if (phase3Prompt) phase3Prompt.SetActive(true); 

                if(leftTriggerObj != null) leftTriggerObj.SetActive(true);
                break;

            case 4:
                enableMoveDirection?.Invoke(true, true); 
                enableGrab?.Invoke(true);
                if (phase4Prompt) phase4Prompt.SetActive(true); 
                break;

            case 5:
                if (phase5Prompt) phase5Prompt.SetActive(true); 
                break;

            case 6:
                if (phase6Prompt) phase6Prompt.SetActive(true); 
                break;

            case 7:
                if (phase7Prompt) phase7Prompt.SetActive(true); 
                break;

            case 8: 
                enableMoveDirection?.Invoke(true, true); 
                enableGrab?.Invoke(false); 
                if (phase8Prompt) phase8Prompt.SetActive(true); 
                break;

            case 9: 
                enableMoveDirection?.Invoke(false, false); 
                if (phase9Prompt) phase9Prompt.SetActive(true); 
                break;
        }
    }

    // once it reaches the platform on the right
    private void ReachedRightTrigger()
    {
        if (phase == 1) SetPhase(2); 
    }

    // associated to a button to ack the preview base house
    public void ProceedToPhase3()
    {
        if (phase == 2)
        {
            SetPhase(3); 
        }
    }

    // once returned to the spawning platform
    public void ReachedLeftTrigger()
    {
        if (phase == 3) SetPhase(4); 
    }

    // grab in CraneMovement calls event that is handled by Tutorial 
    private void OnCraneGrab()
    {
        if (phase == 4) SetPhase(5);
        else if (phase == 6) SetPhase(7);
    }

    // InverseTowerHandler calls event during housePlacement and it is handled byt Tutorial if its in tutorial mode
    private void OnTutorialHousePlaced()
    {
        if (phase == 5)
        {
            // spawna la casa solo se è già stata piazzata
            if (!hasSpawnedSecondHouse)
            {
                hasSpawnedSecondHouse = true; 
                spawnerHouse.SpawnOne(1, 1, 1, spawningPlatformTutorial.transform.position.x, spawningPlatformTutorial.transform.position.y, spawningPlatformTutorial.transform.position.z);
            }
            SetPhase(6);
        }
        else if (phase == 7)
        {
            SetPhase(8);
        }
    }

    public void SkipTutorial()
    {
        HideAllPrompts(); 
        phase = 8;
        Debug.Log("Tutorial Phase Started: " + phase);
        enableMoveDirection?.Invoke(true, true); 
        enableGrab?.Invoke(false); 
        
    }

    public void StartGame()
    {
        HideAllPrompts();
        StartCoroutine(SmoothTransitionToGameplayZOnly());
    }

    // Coroutine for the final transition:
    // 1. Locks player controls
    // 2. Waits for the crane to physically reach the target destination
    // 3. Sets up the gameplay platforms and starts the actual game
    private IEnumerator SmoothTransitionToGameplayZOnly()
    {
        enableMoveDirection?.Invoke(false, false);
        enableGrab?.Invoke(false);

        //spawningPlatformTutorial.SetActive(false);
        //placingPlatformTutorial.SetActive(false);
        //TutorialPlatforms.SetActive(false);

        Vector3 targetPos = new Vector3(crane.transform.position.x, crane.transform.position.y, spawningPlatformGameplay.transform.position.z - 8);

        CraneMovement craneMovement = crane.GetComponent<CraneMovement>(); //add as variable
        craneMovement.MoveToPositionSmoothly(targetPos);

        while (Vector3.Distance(crane.transform.position, targetPos) > 0.05f)
        {
            yield return null; 
        }

        crane.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        craneMovement.SyncTargetPosition(targetPos);

        craneMovement.platforms.Clear();
        craneMovement.platforms = platformsGameplay;
        craneMovement.setSpawnPosition(spawningPlatformGameplay);

        enableMoveDirection?.Invoke(true, true);
        enableGrab?.Invoke(true);

        startGame?.Invoke(userCode, numOfVariantPerCharacteristic);
    }


}