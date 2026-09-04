using UnityEngine;

[CreateAssetMenu(fileName = "DefaultExperimentConfig", menuName = "Experiment/Base Config")]
public class ExperimentConfigSO : ScriptableObject
{
    [Header("Default Parameters")]
    public int defaultMaxCombo = 5;
    public int defaultCategoriesToComplete = 9;
    public int defaultMaxReshuffles = 2;
    public int defaultTime = 60;

    [Header("Default Flags")]
    public bool defaultVrMode = false;
    public bool defaultTimeMode = true;
    public bool defaultDebugMode = false;
    public bool defaultSkipTutorial = false;
    public bool defaultEnableUserCodeInGameInput = false;
}