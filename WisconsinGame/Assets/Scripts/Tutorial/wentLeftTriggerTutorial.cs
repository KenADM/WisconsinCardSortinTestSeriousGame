using System;
using UnityEngine;

public class wentLeftTriggerTutorial : MonoBehaviour
{
    public static event Action reachedLeft;

    void OnTriggerEnter(Collider other)
    {
        reachedLeft?.Invoke();
        
        // Disattiviamo il trigger subito dopo averlo toccato
        gameObject.SetActive(false); 
    }
}