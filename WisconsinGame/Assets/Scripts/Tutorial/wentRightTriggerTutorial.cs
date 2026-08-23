using System;
using Serilog;
using UnityEngine;

public class wentRightTriggerTutorial : MonoBehaviour
{
    public static event Action reachedRight;

    void OnTriggerEnter(Collider other)
    {
        // Puoi aggiungere un controllo sul tag se vuoi che solo la gru lo attivi, es:
        // if (!other.CompareTag("Player")) return;
        
        
        reachedRight?.Invoke();
        
        // Disattiviamo il trigger subito dopo averlo toccato per evitare doppi input
        gameObject.SetActive(false); 
    }
}