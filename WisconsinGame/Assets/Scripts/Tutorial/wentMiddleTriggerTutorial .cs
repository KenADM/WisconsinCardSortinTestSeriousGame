using UnityEngine;
using System;

/// <summary>
/// Detects when the crane reaches the middle during the tutorial 
/// and notifies the Tutorial state machine to advance to the next phase.
/// </summary>
public class wentMiddleTriggerTutorial  : MonoBehaviour
{
    // --- EVENTS ---
    public static event Action ReachedMiddle;

    /// <summary>
    /// Triggered automatically by Unity when another collider enters this trigger zone.
    /// Fires the ReachedLeft event and immediately deactivates the object to prevent double-firing.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        ReachedMiddle?.Invoke();
        
        // Deactivate immediately to prevent multiple rapid collisions from breaking the tutorial sequence
        gameObject.SetActive(false); 
    }
}

