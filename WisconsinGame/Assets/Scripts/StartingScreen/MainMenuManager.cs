using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Settings Displays (Text)")]
    public TMP_Text maxComboText;
    public TMP_Text categoriesText;
    public TMP_Text reshufflesText;
    public Toggle vrModeToggle; 

    [Header("Other Inputs")]
    public TMP_InputField userCodeInput;

    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject settingsPanel;
    private string gameSceneName = "MainGame";

    private int tempMaxCombo;
    private int tempCategories;
    private int tempReshuffles;
    
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        mainPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void OpenSettings()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);

        // Load the saved settings into our temporary variables
        tempMaxCombo = ExperimentSettings.maxCombo;
        tempCategories = ExperimentSettings.categoriesToComplete;
        tempReshuffles = ExperimentSettings.maxReshuffles;
        
        if (vrModeToggle != null) vrModeToggle.isOn = ExperimentSettings.vrMode;
        
        UpdateSettingsUI();
    }


    public void ChangeMaxCombo(int amount)
    {
        tempMaxCombo += amount;
        if (tempMaxCombo < 1) tempMaxCombo = 1; // Prevent going below 1
        UpdateSettingsUI();
    }

    public void ChangeCategories(int amount)
    {
        tempCategories += amount;
        if (tempCategories < 1) tempCategories = 1; 
        UpdateSettingsUI();
    }

    public void ChangeReshuffles(int amount)
    {
        tempReshuffles += amount;
        if (tempReshuffles < 0) tempReshuffles = 0; // Prevent negative reshuffles
        UpdateSettingsUI();
    }

    // --- WRAPPER METHODS FOR YOUR BUTTONS ---

    public void RaiseMaxCombo() { ChangeMaxCombo(1); }
    public void LowerMaxCombo() { ChangeMaxCombo(-1); }
    
    public void RaiseCategories() { ChangeCategories(1); }
    public void LowerCategories() { ChangeCategories(-1); }
    
    public void RaiseReshuffles() { ChangeReshuffles(1); }
    public void LowerReshuffles() { ChangeReshuffles(-1); }

    // Refreshes the text on screen so the player sees the numbers change
    private void UpdateSettingsUI()
    {
        if (maxComboText != null) maxComboText.text = tempMaxCombo.ToString();
        if (categoriesText != null) categoriesText.text = tempCategories.ToString();
        if (reshufflesText != null) reshufflesText.text = tempReshuffles.ToString();
    }

    public void SaveAndCloseSettings()
    {
        // Save everything to the global bridge
        ExperimentSettings.maxCombo = tempMaxCombo;
        ExperimentSettings.categoriesToComplete = tempCategories;
        ExperimentSettings.maxReshuffles = tempReshuffles;
        
        if (vrModeToggle != null) ExperimentSettings.vrMode = vrModeToggle.isOn;
        
        settingsPanel.SetActive(false);
        mainPanel.SetActive(true); // Bring the main menu back up
    }

    public void PlayGame()
    {
        if (userCodeInput != null)
        {
            ExperimentSettings.userCode = userCodeInput.text;
        }

        if (!string.IsNullOrWhiteSpace(ExperimentSettings.userCode))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            SceneManager.LoadScene(gameSceneName);
        }
        
    }
}