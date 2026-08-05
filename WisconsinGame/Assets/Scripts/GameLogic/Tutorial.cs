using UnityEngine;
using System;
using TMPro;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel;

    private int numOfVariantPerCharacteristic = 4;
    string userCode = "testUser"; // Default user
    public static event Action<string, int> startGame;

    void OnEnable()
    {
        StartingScreenManager.TutorialGame += TutorialGameHandler;
    }

    void OnDisable()
    {
        StartingScreenManager.TutorialGame -= TutorialGameHandler;
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
        startGame?.Invoke(userCode, numOfVariantPerCharacteristic);
    }
}
