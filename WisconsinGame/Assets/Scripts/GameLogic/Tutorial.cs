using UnityEngine;
using System;
using TMPro;
using System.Collections.Generic;
using System.Collections; // <-- REINSERITO PER IL VIAGGIO FLUIDO

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel;
    private int numOfVariantPerCharacteristic = 4;
    string userCode = "testUser";
    
    // Questo è l'evento che "griderà" al resto del gioco che il tutorial è finito
    public static event Action<string, int> startGame;

    public SpawnerHouse spawnerHouse;

    [Header("For Tutorial")]
    [SerializeField] private GameObject spawningPlatformTutorial;
    [SerializeField] private GameObject placingPlatformTutorial;
    [SerializeField] private GameObject TutorialPlatforms;

    [Header("Tutorial Text Prompts")]
    [SerializeField] private GameObject moveRightPrompt; 
    [SerializeField] private GameObject moveLeftPrompt;  
    [SerializeField] private GameObject pickupPrompt;    
    [SerializeField] private GameObject dropPrompt;      

    [Header("Tutorial Triggers")]
    [SerializeField] private GameObject rightTriggerObj; 
    [SerializeField] private GameObject leftTriggerObj;  

    [Header("UI Finale")]
    [SerializeField] private GameObject startGameButtonUI; 
    
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
        StartingScreenManager.TutorialGame += TutorialGameHandler;
        
        wentRightTriggerTutorial.reachedRight += ReachedRightTrigger;
        wentLeftTriggerTutorial.reachedLeft += ReachedLeftTrigger; 
        
        CraneMovement.onHouseGrabbed += OnCraneGrab;
        CraneMovement.onHouseDropped += OnCraneDrop;

        // Iscrizione all'evento di piazzamento corretto inviato dal Manager
        GameLogicManager.TutorialspawnNextHouse += OnTutorialHousePlaced;
    }

    void OnDisable()
    {
        StartingScreenManager.TutorialGame -= TutorialGameHandler;
        
        wentRightTriggerTutorial.reachedRight -= ReachedRightTrigger;
        wentLeftTriggerTutorial.reachedLeft -= ReachedLeftTrigger; 
        
        CraneMovement.onHouseGrabbed -= OnCraneGrab;
        CraneMovement.onHouseDropped -= OnCraneDrop;

        // Disiscrizione all'evento
        GameLogicManager.TutorialspawnNextHouse -= OnTutorialHousePlaced;
    }
    
    void Start()
    {
        spawnStartingHouse?.Invoke(placingPlatformTutorial.transform);
        HideAllPrompts(); 
        
        if(rightTriggerObj != null) rightTriggerObj.SetActive(false);
        if(leftTriggerObj != null) leftTriggerObj.SetActive(false);
        if(startGameButtonUI != null) startGameButtonUI.SetActive(false);
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
        tutorialPanel.SetActive(true);
    }

    public void CloseTutorialPanel()
    {
        tutorialPanel.SetActive(false);
        SetPhase(1); 
    }

    private void HideAllPrompts()
    {
        if (moveRightPrompt) moveRightPrompt.SetActive(false);
        if (moveLeftPrompt) moveLeftPrompt.SetActive(false);
        if (pickupPrompt) pickupPrompt.SetActive(false);
        if (dropPrompt) dropPrompt.SetActive(false);
    }

    private void SetPhase(int newPhase)
    {
        phase = newPhase;
        Debug.Log("Tutorial Phase Started: " + phase);

        HideAllPrompts(); 
    
        switch (phase)
        {
            case 1:
                enableMoveDirection?.Invoke(false, true); 
                enableGrab?.Invoke(false);
                if (moveRightPrompt) moveRightPrompt.SetActive(true); 
                
                if(rightTriggerObj != null) rightTriggerObj.SetActive(true);
                break;
                
            case 3:
                spawnerHouse.SpawnOne(0, 0, 1, spawningPlatformTutorial.transform.position.x, spawningPlatformTutorial.transform.position.y, spawningPlatformTutorial.transform.position.z);
                enableMoveDirection?.Invoke(true, false); 
                if (moveLeftPrompt) moveLeftPrompt.SetActive(true); 
                
                if(leftTriggerObj != null) leftTriggerObj.SetActive(true);
                break;

            case 4:
                enableMoveDirection?.Invoke(false, false); 
                enableGrab?.Invoke(true);
                if (pickupPrompt) pickupPrompt.SetActive(true); 
                break;
                
            case 5:
                if (dropPrompt) dropPrompt.SetActive(true); 
                break;

            case 6:
                if (pickupPrompt) pickupPrompt.SetActive(true); 
                break;

            case 7:
                if (dropPrompt) dropPrompt.SetActive(true); 
                break;

            case 8: 
                enableMoveDirection?.Invoke(true, false); 
                enableGrab?.Invoke(false); 
                if (moveLeftPrompt) moveLeftPrompt.SetActive(true); 
                break;
                
            case 9: 
                enableMoveDirection?.Invoke(false, false); 
                if (startGameButtonUI != null) startGameButtonUI.SetActive(true); 
                break;
        }
    }

    private void ReachedRightTrigger()
    {
        if (phase == 1) SetPhase(3); 
    }

    public void ReachedLeftTrigger()
    {
        if (phase == 3) SetPhase(4); 
    }

    private void OnCraneGrab()
    {
        if (phase == 4) SetPhase(5);
        else if (phase == 6) SetPhase(7);
    }

    private void OnCraneDrop()
    {
        // Lasciato vuoto intenzionalmente: gestiamo il drop tramite OnTutorialHousePlaced
    }

    private void OnTutorialHousePlaced()
    {
        if (phase == 5)
        {
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

    // --- MODIFICATO: Ora avvia la coroutine per il viaggio fluido ---
    public void StartGame()
    {
        if (startGameButtonUI != null) startGameButtonUI.SetActive(false);
        StartCoroutine(SmoothTransitionToGameplayZOnly());
    }

    // --- COROUTINE PER IL VIAGGIO AUTOMATICO FLUIDO (SOLO ASSE Z) ---
    private IEnumerator SmoothTransitionToGameplayZOnly()
    {
        // 1. Assicurati che i comandi siano bloccati durante il viaggio
        enableMoveDirection?.Invoke(false, false);
        enableGrab?.Invoke(false);

        // 2. Nascondi le piattaforme del tutorial
        spawningPlatformTutorial.SetActive(false);
        placingPlatformTutorial.SetActive(false);
        TutorialPlatforms.SetActive(false);

        // 3. Calcola il punto di arrivo (Mantiene la X attuale, cambia SOLO la Z)
        Vector3 targetPos = new Vector3(crane.transform.position.x, crane.transform.position.y, spawningPlatformGameplay.transform.position.z - 8);

        // 4. Ordiniamo a CraneMovement di andarci fluidamente usando il suo nuovo metodo
        CraneMovement craneMovement = crane.GetComponent<CraneMovement>();
        craneMovement.MoveToPositionSmoothly(targetPos);

        // 5. Aspettiamo finché la gru non è arrivata (con un margine d'errore di 0.05f)
        while (Vector3.Distance(crane.transform.position, targetPos) > 0.05f)
        {
            yield return null; // Aspetta il frame successivo
        }

        // 6. Arrivati! Raddrizziamo la rotazione e forziamo la posizione esatta finale per evitare micro-scivolamenti
        crane.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        craneMovement.SyncTargetPosition(targetPos);

        // 7. Imposta le nuove piattaforme di gioco reali
        craneMovement.platforms.Clear();
        craneMovement.platforms = platformsGameplay;
        craneMovement.setSpawnPosition(spawningPlatformGameplay);

        // 8. Riattiva tutti i controlli per permettere al giocatore di giocare liberamente
        enableMoveDirection?.Invoke(true, true);
        enableGrab?.Invoke(true);

        // 9. Lancia l'evento finale. Il GameLogicManager lo sentirà e genererà le prime case vere!
        startGame?.Invoke(userCode, numOfVariantPerCharacteristic);
    }
}