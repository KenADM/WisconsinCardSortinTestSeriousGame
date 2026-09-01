using UnityEngine;
using System;

/// <summary>
/// Detects when the crane reaches the right-side boundary during the tutorial 
/// and notifies the Tutorial state machine to advance to the next phase.
/// </summary>
public class wentRightTriggerTutorial : MonoBehaviour
{
    // --- EVENTS ---
    public static event Action ReachedRight;

    /// <summary>
    /// Triggered automatically by Unity when another collider enters this trigger zone.
    /// Fires the ReachedRight event and immediately deactivates the object to prevent double-firing.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        ReachedRight?.Invoke();
        
        // Deactivate immediately to prevent multiple rapid collisions from breaking the tutorial sequence
        gameObject.SetActive(false); 
    }
}