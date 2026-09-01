using UnityEngine;
using TMPro;
using System;

/// <summary>
/// Gestisce il timer della modalità Arcade. Il tempo diminuisce costantemente, 
/// ma il giocatore può guadagnare secondi preziosi facendo le scelte giuste, 
/// o perderli facendo scelte sbagliate.
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

    // Evento globale che scatta quando il tempo arriva a zero
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
        // Supponiamo che il GameLogicManager lanci questi eventi quando valuta la casa
        GameLogicManager.OnCorrectPlacement += AddTime;
        GameLogicManager.OnWrongPlacement += RemoveTime;
        
        // Fai partire il timer quando finisce il tutorial o inizia il gioco
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

        // Riduci il tempo ogni frame
        currentTime -= Time.deltaTime;

        if (currentTime <= 0)
        {
            currentTime = 0;
            isTimerRunning = false;
            OnTimeOut?.Invoke(); // Avvisa il gioco che la partita è finita!
        }

        UpdateTimerUI();
    }

    /// <summary>
    /// Avvia il conto alla rovescia.
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
    /// Aggiunge secondi al timer (chiamato quando il giocatore indovina).
    /// </summary>
    private void AddTime()
    {
        if (!isTimerRunning) return;
        currentTime += bonusTime;
        
        // Feedback visivo opzionale: fai "saltare" il testo per far notare il bonus
        // (Si potrebbe aggiungere un'animazione qui)
    }

    /// <summary>
    /// Sottrae secondi al timer (chiamato quando il giocatore sbaglia).
    /// </summary>
    private void RemoveTime()
    {
        if (!isTimerRunning) return;
        currentTime -= penaltyTime;
    }

    /// <summary>
    /// Aggiorna il testo a schermo formattandolo come Minuti:Secondi.
    /// Se mancano meno di 10 secondi, il testo diventa rosso.
    /// </summary>
    private void UpdateTimerUI()
    {
        if (timerText == null) return;

        // Converte i float in minuti e secondi
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        // Effetto panico: cambia colore se il tempo sta per scadere
        if (currentTime <= 10f)
        {
            // Lampeggia leggermente tra rosso e bianco usando il tempo
            timerText.color = Color.Lerp(warningColor, normalColor, Mathf.PingPong(Time.time * 2f, 1f));
        }
        else
        {
            timerText.color = normalColor;
        }
    }
}