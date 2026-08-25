using System;
using UnityEngine;

public class wentLeftTriggerTutorial : MonoBehaviour
{
    public static event Action ReachedLeft;

    void OnTriggerEnter(Collider other)
    {
        ReachedLeft?.Invoke();
        
        // Disattiviamo il trigger subito dopo averlo toccato
        gameObject.SetActive(false); 
    }
}