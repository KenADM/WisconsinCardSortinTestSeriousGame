using UnityEngine;
using System;
using TMPro;
using System.Collections;

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

    private IEnumerator Start()
    {   
        yield return null;
        if (!ExperimentSettings.enableUserCodeInGameInput) {
            StartTutorialDirectly();

        }
        else
        {
            if (startMenu != null)
            {
                startMenu.SetActive(true);
            }
            
            if (input != null)
            {
                input.text = ExperimentSettings.userCode;
                Debug.Log($"Loading user code: {input.text}");
            }else
            {
                Debug.LogWarning("Input field reference is missing in StartingScreenManager.");
            }   
        }


    


    }

    /// <summary>
    /// Checks if the user code input field contains valid text.
    /// </summary>
    /// <returns>True if the field is not empty or just blank spaces, otherwise false.</returns>
    private bool CheckUser()
    {
        Debug.Log($"Checking input field: '{ExperimentSettings.userCode}'");
        return !string.IsNullOrWhiteSpace(ExperimentSettings.userCode);
    }

    /// <summary>
    /// Validates the user code and initiates the tutorial sequence.
    /// If the input is invalid or blank, it adds a warning "!" to the prompt text.
    /// </summary>
    public void StartTutorialHandler()
    {
        ExperimentSettings.userCode = input.text;
        PlayerPrefs.SetString("UserCode", ExperimentSettings.userCode); 

        if (CheckUser())
        {
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

    /// <summary>
    /// Used when the ingame username insertion is disabled
    /// </summary>
    private void StartTutorialDirectly()
    {
        TutorialGame?.Invoke();
    }
}