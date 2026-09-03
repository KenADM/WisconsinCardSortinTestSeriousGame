using UnityEngine;
using System;
using TMPro;

/// <summary>
/// Manages the initial start screen UI, validating the user code 
/// before allowing the experiment or tutorial to begin.
/// </summary>
public class StartingScreenManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI testo;
    [SerializeField] private TMP_InputField input;
    [SerializeField] private GameObject startMenu;
    private string userCode;

    // --- EVENTS ---
    public static event Action TutorialGame;

    private void Start()
    {
        if (startMenu != null)
        {
            startMenu.SetActive(true);
        }
        
        if (input != null)
        {
            Debug.Log($"Loading user code: {input.text}");
            //input.text = ExperimentSettings.userCode;
        }else
        {
            Debug.LogWarning("Input field reference is missing in StartingScreenManager.");
        }
    }

    /// <summary>
    /// Checks if the user code input field contains valid text.
    /// </summary>
    /// <returns>True if the field is not empty or just blank spaces, otherwise false.</returns>
    private bool CheckInputField()
    {
        Debug.Log($"Checking input field: '{input.text}'");
        return !string.IsNullOrWhiteSpace(input.text);
    }

    /// <summary>
    /// Validates the user code and initiates the tutorial sequence.
    /// If the input is invalid or blank, it adds a warning "!" to the prompt text.
    /// </summary>
    public void StartTutorialHandler()
    {
        if (CheckInputField())
        {
            ExperimentSettings.userCode = input.text;
            TutorialGame?.Invoke();
            
            if (startMenu != null)
            {
                startMenu.SetActive(false);
            }
        }
        else
        {
            // Provides visual feedback to the user that the field cannot be empty
            if (testo != null)
            {
                testo.text += "!"; 
            }
        }
    }
}