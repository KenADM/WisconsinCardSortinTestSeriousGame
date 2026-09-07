using UnityEngine;
using TMPro;
using System;

/// <summary>
/// Timer handler script, for when the game is in timer mode.
/// </summary>
public class ArcadeTimerManager : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private float startingTime = 60f;
    [SerializeField] private float bonusTime = 5f;
    [SerializeField] private float penaltyTime = 10f;

    [Header("UI References")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color warningColor = Color.red;

    private float currentTime;
    private bool isTimerRunning = false;
    public static event Action OnTimeOut;

    void Awake()
    {
        if (timerText == null)
        {
            Debug.LogError("Timer Text reference is missing. Please assign it in the inspector.");
        }

        startingTime = ExperimentSettings.time; 
    }
    void Start()
    {
        currentTime = startingTime;
        UpdateTimerUI();
    }

    private void OnEnable()
    {
        GameLogicManager.OnCorrectPlacement += AddTime;
        GameLogicManager.OnWrongPlacement += RemoveTime;
        Tutorial.StartGame += StartTimer;
    }

    private void OnDisable()
    {
        GameLogicManager.OnCorrectPlacement -= AddTime;
        GameLogicManager.OnWrongPlacement -= RemoveTime;
        Tutorial.StartGame -= StartTimer;
    }



    private void Update()
    {
        if (!isTimerRunning) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0)
        {
            currentTime = 0;
            isTimerRunning = false;
            OnTimeOut?.Invoke(); 
        }

        UpdateTimerUI();
    }

    /// <summary>
    /// Start Countdown
    /// </summary>
    private void StartTimer()
    {
        if (ExperimentSettings.timeMode)
        {
            currentTime = startingTime;
            isTimerRunning = true;
        }
    }

    /// <summary>
    /// add seconds to timer
    /// </summary>
    private void AddTime()
    {
        if (!isTimerRunning) return;
        currentTime += bonusTime;
        
    }

    /// <summary>
    /// remove time
    /// </summary>
    private void RemoveTime()
    {
        if (!isTimerRunning) return;
        currentTime -= penaltyTime;
    }

    /// <summary>
    /// Updates timer text
    /// </summary>
    private void UpdateTimerUI()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        // timer flash
        if (currentTime <= 10f)
        {
            timerText.color = Color.Lerp(warningColor, normalColor, Mathf.PingPong(Time.time * 2f, 1f));
        }
        else
        {
            timerText.color = normalColor;
        }
    }
}