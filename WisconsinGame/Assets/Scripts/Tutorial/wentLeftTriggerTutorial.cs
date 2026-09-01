using UnityEngine;
using System;

/// <summary>
/// Detects when the crane reaches the left-side boundary during the tutorial 
/// and notifies the Tutorial state machine to advance to the next phase.
/// </summary>
public class wentLeftTriggerTutorial : MonoBehaviour
{
    // --- EVENTS ---
    public static event Action ReachedLeft;

    /// <summary>
    /// Triggered automatically by Unity when another collider enters this trigger zone.
    /// Fires the ReachedLeft event and immediately deactivates the object to prevent double-firing.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        ReachedLeft?.Invoke();
        
        // Deactivate immediately to prevent multiple rapid collisions from breaking the tutorial sequence
        gameObject.SetActive(false); 
    }
}