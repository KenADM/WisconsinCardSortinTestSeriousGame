using UnityEngine;

/// <summary>
/// Manages the audiovisual feedback (particle effects and sounds) when a house is placed on a tower.
/// Triggered by the GameLogicManager based on whether the placement matched the current sorting condition.
/// </summary>
public class FeedbackManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private ParticleSystem smoke;
    [SerializeField] private AudioSource sound;
    
    [Header("Feedback Settings")]
    [SerializeField] private Color positive = Color.white;
    [SerializeField] private Color negative = Color.black;
    [SerializeField] private AudioClip positiveSound;
    [SerializeField] private AudioClip negativeSound;

    /// <summary>
    /// Triggers the appropriate visual and audio feedback based on the result of a house placement.
    /// </summary>
    /// <param name="isCorrect">True if the placement matched the active sorting condition, false otherwise.</param>
    public void Feedback(bool isCorrect)
    {
        // Safely check if the particle system exists before attempting to stop or play it
        if (smoke != null)
        {   
            smoke.Stop(); // Moved inside the null check to prevent crashes!
            
            var smokeModule = smoke.main;
            smokeModule.startColor = isCorrect ? positive : negative;
            smoke.Play();

            // Safely play the audio
            if (sound != null)
            {
                if (isCorrect)
                {
                    sound.PlayOneShot(positiveSound, 0.25f);
                }
                else
                {
                    sound.PlayOneShot(negativeSound, 2.5f);
                }
            }
            else
            {
                Debug.LogWarning("AudioSource not assigned in FeedbackManager on " + gameObject.name);
            }
        }
        else
        {
            Debug.LogWarning("Smoke ParticleSystem not assigned in FeedbackManager on " + gameObject.name);
        }
    }
}