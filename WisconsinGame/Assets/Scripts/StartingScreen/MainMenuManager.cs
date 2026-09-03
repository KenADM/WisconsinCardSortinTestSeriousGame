using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the Main Menu UI, allowing the researcher/player to adjust experiment settings 
/// (combo targets, categories, reshuffles, time limits, VR mode) before loading the main simulation scene.
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    [Header("Settings Displays (Text)")]
    [SerializeField] private TMP_Text maxComboText;
    [SerializeField] private TMP_Text categoriesText;
    [SerializeField] private TMP_Text reshufflesText;
    [SerializeField] private TMP_Text timeText;
    
    [Header("Toggles")]
    [SerializeField] private Toggle vrModeToggle; 
    [SerializeField] private Toggle timeModeToggle; 
    [SerializeField] private Toggle debugModeToggle; 

    [Header("Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Mode Inputs")]
    [SerializeField] private GameObject MaxTimeInputField;
    [SerializeField] private GameObject MaxReshufflesInputField;
    [SerializeField] private GameObject MaxCategoriesInputField;

    [Header("Scene Routing")]
    [SerializeField] private string gameSceneName = "MainGame";

    // Temporary variables to hold settings while the user adjusts them before saving
    private int tempMaxCombo;
    private int tempCategories;
    private int tempReshuffles;
    
    private void Awake()
    {
        // Initialize temporary settings to current global settings
        tempMaxCombo = ExperimentSettings.maxCombo;
        tempCategories = ExperimentSettings.categoriesToComplete;
        tempReshuffles = ExperimentSettings.maxReshuffles;
        if (vrModeToggle != null) vrModeToggle.isOn = ExperimentSettings.vrMode;
        if (timeModeToggle != null) timeModeToggle.isOn = ExperimentSettings.timeMode;
        if (timeModeToggle.isOn)
        {
            TimeMode();
        }
        else
        {
            CategoryMode();
        }
    }
    
    private void Start()
    {
        // Ensure the cursor is visible and unlocked for menu navigation
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        if (mainPanel != null) mainPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }
    
    private void Update()
    {
        if (timeModeToggle != null && timeModeToggle.isOn)
        {
            TimeMode();
        }
        else
        {
            CategoryMode();
        }
    }

    /// <summary>
    /// Swaps the UI to the settings panel and pre-loads the current global experiment settings.
    /// </summary>
    public void OpenSettings()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);

        tempMaxCombo = ExperimentSettings.maxCombo;
        tempCategories = ExperimentSettings.categoriesToComplete;
        tempReshuffles = ExperimentSettings.maxReshuffles;
        
        if (vrModeToggle != null) vrModeToggle.isOn = ExperimentSettings.vrMode;
        if (timeModeToggle != null) timeModeToggle.isOn = ExperimentSettings.timeMode;
        if (debugModeToggle != null) debugModeToggle.isOn = ExperimentSettings.debugMode;

        UpdateSettingsUI();
    }

    /// <summary>
    /// Core calculation method to safely adjust the temporary Max Combo setting.
    /// </summary>
    private void ChangeMaxCombo(int amount)
    {
        tempMaxCombo += amount;
        if (tempMaxCombo < 1) tempMaxCombo = 1; // Prevent going below 1
        UpdateSettingsUI();
    }

    /// <summary>
    /// Core calculation method to safely adjust the temporary Categories setting.
    /// </summary>
    private void ChangeCategories(int amount)
    {
        tempCategories += amount;
        if (tempCategories < 1) tempCategories = 1; 
        UpdateSettingsUI();
    }

    /// <summary>
    /// Core calculation method to safely adjust the temporary Reshuffles setting.
    /// </summary>
    private void ChangeReshuffles(int amount)
    {
        tempReshuffles += amount;
        if (tempReshuffles < 0) tempReshuffles = 0; // Prevent negative reshuffles
        UpdateSettingsUI();
    }

    /// <summary>
    /// Core calculation method to safely adjust the starting time for the Arcade Mode.
    /// </summary>
    private void ChangeTimeAmmount(int amount) // Kept original spelling to preserve Inspector button assignments
    {
        ExperimentSettings.time += amount;
        if (ExperimentSettings.time < 0) ExperimentSettings.time = 0; // Prevent going below 0 seconds
        UpdateSettingsUI();
    }

    // --- WRAPPER METHODS FOR UI BUTTONS ---

    public void RaiseMaxCombo() { ChangeMaxCombo(1); }
    public void LowerMaxCombo() { ChangeMaxCombo(-1); }
    
    public void RaiseCategories() { ChangeCategories(1); }
    public void LowerCategories() { ChangeCategories(-1); }
    
    public void RaiseReshuffles() { ChangeReshuffles(1); }
    public void LowerReshuffles() { ChangeReshuffles(-1); }

    public void RaiseTime() { ChangeTimeAmmount(30); }
    public void LowerTime() { ChangeTimeAmmount(-30); }

    /// <summary>
    /// Refreshes the text on screen so the player sees the numbers change dynamically.
    /// </summary>
    private void UpdateSettingsUI()
    {
        if (maxComboText != null) maxComboText.text = tempMaxCombo.ToString();
        if (categoriesText != null) categoriesText.text = tempCategories.ToString();
        if (reshufflesText != null) reshufflesText.text = tempReshuffles.ToString();
        if (timeText != null) timeText.text = ExperimentSettings.time.ToString(); 
    }

    /// <summary>
    /// Commits the temporary settings to the global ExperimentSettings bridge and returns to the main menu.
    /// </summary>
    public void SaveAndCloseSettings()
    {
        ExperimentSettings.maxCombo = tempMaxCombo;
        ExperimentSettings.categoriesToComplete = tempCategories;
        ExperimentSettings.maxReshuffles = tempReshuffles;
        
        if (vrModeToggle != null) ExperimentSettings.vrMode = vrModeToggle.isOn;
        if (timeModeToggle != null) ExperimentSettings.timeMode = timeModeToggle.isOn; 
        if (debugModeToggle != null) ExperimentSettings.debugMode = debugModeToggle.isOn;
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (mainPanel != null) mainPanel.SetActive(true); 
    }

    /// <summary>
    /// Locks the cursor and transitions the player into the main gameplay scene.
    /// </summary>
    public void PlayGame()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene(gameSceneName);
    }

    public void TimeCateogryToggleChange()
    {
        ExperimentSettings.timeMode = timeModeToggle.isOn;
        Debug.Log("Time Mode Toggle Changed: " + ExperimentSettings.timeMode);
    }
    
    public void TimeMode()
    {
        if(MaxTimeInputField != null) MaxTimeInputField.SetActive(true);
        if(MaxReshufflesInputField != null) MaxReshufflesInputField.SetActive(false);
        if(MaxCategoriesInputField != null) MaxCategoriesInputField.SetActive(false);
    }
    
    public void CategoryMode()
    {
        if(MaxTimeInputField != null) MaxTimeInputField.SetActive(false);
        if(MaxReshufflesInputField != null) MaxReshufflesInputField.SetActive(true);
        if(MaxCategoriesInputField != null) MaxCategoriesInputField.SetActive(true);
    }

    /// <summary>
    /// Quits the application entirely. If running in the Unity Editor, it safely stops Play Mode.
    /// </summary>
    public void QuitApplication()
    {
        Debug.Log("Quitting Application");
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}