using UnityEngine;

/// <summary>
/// Dynamically creates and manages the audio sources for the crane's mechanical sounds.
/// Handles the looping playback for both the arm movement (up/down) and the track movement (left/right).
/// </summary>
public class CraneSoundSystem : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip craneSoundUp;
    [SerializeField] private AudioClip movementSound;

    private AudioSource armAudioSource;
    private AudioSource movementAudioSource;

    private void Awake()
    {
        // Dynamically configure the AudioSource for the arm movement
        armAudioSource = gameObject.AddComponent<AudioSource>();
        armAudioSource.clip = craneSoundUp;
        armAudioSource.loop = true;
        armAudioSource.volume = 0.25f;
        armAudioSource.playOnAwake = false;

        // Dynamically configure the AudioSource for the lateral track movement
        movementAudioSource = gameObject.AddComponent<AudioSource>();
        movementAudioSource.clip = movementSound;
        movementAudioSource.loop = true;
        movementAudioSource.volume = 0.25f;
        movementAudioSource.playOnAwake = false;
    }

    /// <summary>
    /// Starts playing the mechanical sound for the crane's arm lowering and rising.
    /// </summary>
    public void PlayArmSound()
    {
        if (armAudioSource != null && !armAudioSource.isPlaying)
        {
            armAudioSource.Play();
        }
    }

    /// <summary>
    /// Stops the arm movement sound.
    /// </summary>
    public void StopArmSound()
    {
        if (armAudioSource != null && armAudioSource.isPlaying)
        {
            armAudioSource.Stop();
        }
    }

    /// <summary>
    /// Starts playing the sliding sound when the crane moves along the horizontal tracks.
    /// </summary>
    public void PlayMovementSound()
    {
        if (movementAudioSource != null && !movementAudioSource.isPlaying)
        {
            movementAudioSource.Play();
        }
    }

    /// <summary>
    /// Stops the sliding movement sound.
    /// </summary>
    public void StopMovementSound()
    {
        if (movementAudioSource != null && movementAudioSource.isPlaying)
        {
            movementAudioSource.Stop();
        }
    }
}