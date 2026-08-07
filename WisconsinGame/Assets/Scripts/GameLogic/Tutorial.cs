using UnityEngine;
using System;
using TMPro;
using System.Collections.Generic;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel;
    private int numOfVariantPerCharacteristic = 4;
    string userCode = "testUser"; // Default user
    public static event Action<string, int> startGame;

    public SpawnerHouse spawnerHouse;

    [Header("For Tutorial")]
    [SerializeField] private GameObject spawningPlatformTutorial;
    [SerializeField] private GameObject placingPlatformTutorial;

    [SerializeField] private GameObject TutorialPlatforms;
    
    public static event System.Action<Transform> spawnStartingHouse;


    [Header("For Actual Game")]

    public List<Transform> platformsGameplay = new List<Transform>();


    [SerializeField] private GameObject crane;
    public GameObject spawningPlatformGameplay;

    private int currentTutorialHousePlaced = 0;

    void OnEnable()
    {
        StartingScreenManager.TutorialGame += TutorialGameHandler;
        wentRightTriggerTutorial.spawnTutorial += TutorialPlacement;
    }

    void OnDisable()
    {
        StartingScreenManager.TutorialGame -= TutorialGameHandler;
        wentRightTriggerTutorial.spawnTutorial -= TutorialPlacement;
    }

    void Start()
    {
        spawnStartingHouse?.Invoke(placingPlatformTutorial.transform);

        // tutorialPanel.SetActive(true);
        // spawningPlatformTutorial.SetActive(true);
        // placingPlatformTutorial.SetActive(true);
        // finalizeTutorial();
    }
    public void StartGame()
    {
        startGame?.Invoke(userCode, numOfVariantPerCharacteristic);

    }

    private void TutorialGameHandler(string codiceUtente, int numOfVariantPerCharacteristic)
    {
        userCode = codiceUtente;
        tutorialPanel.SetActive(true);
    }

    public void CloseTutorialPanel()
    {
        tutorialPanel.SetActive(false);
        // spiega movement

        // spiego pickup e placement sbagliato
        // spawnerHouse.SpawnOne(0, 0, 1, spawningPlatformTutorial.transform.position.x, spawningPlatformTutorial.transform.position.y, spawningPlatformTutorial.transform.position.z);
        
        // spiego pickup e placement corretto
        

        //startGame?.Invoke(userCode, numOfVariantPerCharacteristic);
        //startTutorialGame?.Invoke(userCode, numOfVariantPerCharacteristic);
    }

    private void TutorialPlacement()
    {
        if(currentTutorialHousePlaced == 0)
        {
            spawnerHouse.SpawnOne(0, 0, 1, spawningPlatformTutorial.transform.position.x, spawningPlatformTutorial.transform.position.y, spawningPlatformTutorial.transform.position.z);
            currentTutorialHousePlaced++;
        }
        else if(currentTutorialHousePlaced == 1)
        {
            spawnerHouse.SpawnOne(1, 1, 1, spawningPlatformTutorial.transform.position.x, spawningPlatformTutorial.transform.position.y, spawningPlatformTutorial.transform.position.z);
            currentTutorialHousePlaced++;

        }else if(currentTutorialHousePlaced == 2)
        {
            finalizeTutorial();
        }
    }

    private void finalizeTutorial()
    {
        // disattivo il tutorial e attivo il gioco vero
        spawningPlatformTutorial.SetActive(false);
        placingPlatformTutorial.SetActive(false);

        crane.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        crane.transform.position = new Vector3(spawningPlatformGameplay.transform.position.x, crane.transform.position.y, spawningPlatformGameplay.transform.position.z - 8);
        CraneMovement craneMovement = crane.GetComponent<CraneMovement>();
        craneMovement.platforms.Clear();
        craneMovement.platforms = platformsGameplay;
        craneMovement.setSpawnPosition(spawningPlatformGameplay);

        startGame?.Invoke(userCode, numOfVariantPerCharacteristic);
        TutorialPlatforms.SetActive(false);
    }



}
