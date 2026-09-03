using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Management;
using System.Collections;
using System.IO;

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
    [SerializeField] private TMP_Text userCodeText;
    
    [Header("Toggles")]
    [SerializeField] private Toggle vrModeToggle; 
    [SerializeField] private Toggle timeModeToggle; 
    [SerializeField] private Toggle debugModeToggle; 
    [SerializeField] private Toggle skipTutorialToggle;

    [Header("Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Mode Inputs")]
    [SerializeField] private GameObject maxTimeInputField;
    [SerializeField] private GameObject maxReshufflesInputField;
    [SerializeField] private GameObject maxCategoriesInputField;

    [Header("Scene Routing")]
    [SerializeField] private string gameSceneName = "MainGame";

    private void Awake()
    {

    }
    
    private void Start()
    {
        if (PlayerPrefs.HasKey("MaxCombo")) ExperimentSettings.maxCombo = PlayerPrefs.GetInt("MaxCombo");
        if (PlayerPrefs.HasKey("CategoriesToComplete")) ExperimentSettings.categoriesToComplete = PlayerPrefs.GetInt("CategoriesToComplete");
        if (PlayerPrefs.HasKey("MaxReshuffles")) ExperimentSettings.maxReshuffles = PlayerPrefs.GetInt("MaxReshuffles");
        if (PlayerPrefs.HasKey("Time")) ExperimentSettings.time = PlayerPrefs.GetInt("Time");
        if (PlayerPrefs.HasKey("vrMode")) ExperimentSettings.vrMode = PlayerPrefs.GetInt("vrMode") == 1;
        if (PlayerPrefs.HasKey("TimeMode")) ExperimentSettings.timeMode = PlayerPrefs.GetInt("TimeMode") == 1;  
        if (PlayerPrefs.HasKey("DebugMode")) ExperimentSettings.debugMode = PlayerPrefs.GetInt("DebugMode") == 1;
        if (PlayerPrefs.HasKey("SkipTutorial")) ExperimentSettings.skipTutorial = PlayerPrefs.GetInt("SkipTutorial") == 1;

        // 2. Connect Toggles to Listeners via code (Better than using the Inspector)
        if (vrModeToggle != null) 
        {
            vrModeToggle.isOn = ExperimentSettings.vrMode;
            vrModeToggle.onValueChanged.AddListener(OnVRModeToggled);
        }
        if (timeModeToggle != null) 
        {
            timeModeToggle.isOn = ExperimentSettings.timeMode;
            timeModeToggle.onValueChanged.AddListener(OnTimeModeToggled);
        }
        if (debugModeToggle != null) 
        {
            debugModeToggle.isOn = ExperimentSettings.debugMode;
            debugModeToggle.onValueChanged.AddListener(OnDebugModeToggled);
        }
        if (skipTutorialToggle != null)
        {
            skipTutorialToggle.isOn = ExperimentSettings.skipTutorial;
            skipTutorialToggle.onValueChanged.AddListener(OnSkipTutorialToggled);
        }

        // Initialize panels and text
        if (mainPanel != null) mainPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        
        TimeModeSettings(ExperimentSettings.timeMode); // Initialize correct panels

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        ExperimentSettings.userCode = ""; 

        SaveAndRefreshUI();
        StartCoroutine(StartXRSafely());
    }

    private IEnumerator StartXRSafely()
    {
        if (XRGeneralSettings.Instance != null && XRGeneralSettings.Instance.Manager != null)
        {
            if (!XRGeneralSettings.Instance.Manager.isInitializationComplete)
            {
                yield return XRGeneralSettings.Instance.Manager.InitializeLoader();
            }

            if (XRGeneralSettings.Instance.Manager.activeLoader != null)
            {
                XRGeneralSettings.Instance.Manager.StartSubsystems();
                Debug.Log("[MainMenu] VR Driver inizializzati e avviati con successo.");
            }
            else
            {
                Debug.LogWarning("[MainMenu] Impossibile avviare il VR. Passaggio forzato alla visuale Desktop.");
                if (vrModeToggle != null) vrModeToggle.isOn = false;
            }
        }
    }

    private void StopXR()
    {
        if (XRGeneralSettings.Instance != null && XRGeneralSettings.Instance.Manager != null)
        {
            if (XRGeneralSettings.Instance.Manager.isInitializationComplete)
            {
                XRGeneralSettings.Instance.Manager.StopSubsystems();
                XRGeneralSettings.Instance.Manager.DeinitializeLoader();
            }
        }
    }

    public void OpenSettings()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        ExperimentSettings.userCode = userCodeText != null ? userCodeText.text : "";
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (mainPanel != null) mainPanel.SetActive(true); 
    }

    // --- DIRECT MODIFICATION METHODS ---

    private void ChangeMaxCombo(int amount)
    {
        ExperimentSettings.maxCombo += amount;
        if (ExperimentSettings.maxCombo < 1) ExperimentSettings.maxCombo = 1; 
        SaveAndRefreshUI();
    }

    private void ChangeCategories(int amount)
    {
        ExperimentSettings.categoriesToComplete += amount;
        if (ExperimentSettings.categoriesToComplete < 1) ExperimentSettings.categoriesToComplete = 1; 
        SaveAndRefreshUI();
    }

    private void ChangeReshuffles(int amount)
    {
        ExperimentSettings.maxReshuffles += amount;
        if (ExperimentSettings.maxReshuffles < 0) ExperimentSettings.maxReshuffles = 0; 
        SaveAndRefreshUI();
    }

    private void ChangeTimeAmmount(int amount) 
    {
        ExperimentSettings.time += amount;
        if (ExperimentSettings.time < 0) ExperimentSettings.time = 0; 
        SaveAndRefreshUI();
    }

    public void RaiseMaxCombo() { ChangeMaxCombo(1); }
    public void LowerMaxCombo() { ChangeMaxCombo(-1); }
    
    public void RaiseCategories() { ChangeCategories(1); }
    public void LowerCategories() { ChangeCategories(-1); }
    
    public void RaiseReshuffles() { ChangeReshuffles(1); }
    public void LowerReshuffles() { ChangeReshuffles(-1); }

    public void RaiseTime() { ChangeTimeAmmount(30); }
    public void LowerTime() { ChangeTimeAmmount(-30); }

    // --- TOGGLE LISTENERS ---

    private void OnVRModeToggled(bool isOn)
    {
        ExperimentSettings.vrMode = isOn;
        SaveAndRefreshUI();
    }

    private void OnTimeModeToggled(bool isOn)
    {
        ExperimentSettings.timeMode = isOn;
        TimeModeSettings(isOn); 
        SaveAndRefreshUI();
    }

    private void OnDebugModeToggled(bool isOn)
    {
        ExperimentSettings.debugMode = isOn;
        SaveAndRefreshUI();
    }

    private void OnSkipTutorialToggled(bool isOn)
    {
        ExperimentSettings.skipTutorial = isOn;
        SaveAndRefreshUI();
    }

    // --- CORE SAVING & UI REFRESH ---

    /// <summary>
    /// Immediately writes everything to PlayerPrefs and updates text.
    /// </summary>
    private void SaveAndRefreshUI()
    {
        // Update Text
        if (maxComboText != null) maxComboText.text = ExperimentSettings.maxCombo.ToString();
        if (categoriesText != null) categoriesText.text = ExperimentSettings.categoriesToComplete.ToString();
        if (reshufflesText != null) reshufflesText.text = ExperimentSettings.maxReshuffles.ToString();
        if (timeText != null) timeText.text = ExperimentSettings.time.ToString(); 
        
        // Save to Disk instantly
        PlayerPrefs.SetInt("MaxCombo", ExperimentSettings.maxCombo);
        PlayerPrefs.SetInt("CategoriesToComplete", ExperimentSettings.categoriesToComplete);
        PlayerPrefs.SetInt("MaxReshuffles", ExperimentSettings.maxReshuffles);
        PlayerPrefs.SetInt("Time", ExperimentSettings.time);
        PlayerPrefs.SetInt("vrMode", ExperimentSettings.vrMode ? 1 : 0);
        PlayerPrefs.SetInt("TimeMode", ExperimentSettings.timeMode ? 1 : 0);
        PlayerPrefs.SetInt("DebugMode", ExperimentSettings.debugMode ? 1 : 0);
        PlayerPrefs.SetInt("SkipTutorial", ExperimentSettings.skipTutorial ? 1 : 0);
        PlayerPrefs.Save(); // Force write to disk
    }

    public void TimeModeSettings(bool isTimeMode)
    {
        if(maxTimeInputField != null) maxTimeInputField.SetActive(isTimeMode);
        if(maxReshufflesInputField != null) maxReshufflesInputField.SetActive(!isTimeMode);
        if(maxCategoriesInputField != null) maxCategoriesInputField.SetActive(!isTimeMode);
    }

    public void WisconsinMode()
    {
        ExperimentSettings.maxCombo = 5;
        ExperimentSettings.categoriesToComplete = 9;
        ExperimentSettings.maxReshuffles = 2;
        ExperimentSettings.vrMode = true;
        ExperimentSettings.timeMode = false;
        ExperimentSettings.debugMode = false;
        ExperimentSettings.skipTutorial = false;

        // Visually update the toggles. Changing `.isOn` automatically triggers the listeners we set up!
        if (vrModeToggle != null) vrModeToggle.isOn = true;
        if (timeModeToggle != null) timeModeToggle.isOn = false;
        if (debugModeToggle != null) debugModeToggle.isOn = false;
        if (skipTutorialToggle != null) skipTutorialToggle.isOn = false;
        if (skipTutorialToggle != null) skipTutorialToggle.isOn = false;

        SaveAndRefreshUI();
    }

    // --- SYSTEM ROUTING ---

    public void PlayGame()
    {
        if (!ExperimentSettings.vrMode)
        {
            StopXR();
        }

        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitApplication()
    {
        Debug.Log("Quitting Application");
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void OpenLogLocation()
    {
        string folderName = "PlayerLog";
        string logDirectory = Path.Combine(Application.persistentDataPath, folderName);
        Debug.Log("Opening log directory: " + logDirectory);
        Application.OpenURL(logDirectory);
    }
}