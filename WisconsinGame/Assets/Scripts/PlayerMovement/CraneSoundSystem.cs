using UnityEngine;

public class CraneSoundSystem : MonoBehaviour
{
    [SerializeField] private AudioClip craneSoundUp;
    [SerializeField] private AudioClip movementSound;

    private AudioSource armAudioSource;
    private AudioSource movementAudioSource;

    void Awake()
    {
        armAudioSource = gameObject.AddComponent<AudioSource>();
        armAudioSource.clip = craneSoundUp;
        armAudioSource.loop = true;
        armAudioSource.volume = 0.25f;
        armAudioSource.playOnAwake = false;

        movementAudioSource = gameObject.AddComponent<AudioSource>();
        movementAudioSource.clip = movementSound;
        movementAudioSource.loop = true;
        movementAudioSource.volume = 0.25f;

        movementAudioSource.playOnAwake = false;
    }

    public void PlayArmSound()
    {
        if (armAudioSource != null && !armAudioSource.isPlaying)
        {
            armAudioSource.Play();
        }
    }

    public void StopArmSound()
    {
        if (armAudioSource != null && armAudioSource.isPlaying)
        {
            armAudioSource.Stop();
        }
    }

    public void PlayMovementSound()
    {
        if (movementAudioSource != null && !movementAudioSource.isPlaying)
        {
            movementAudioSource.Play();
        }
    }

    public void StopMovementSound()
    {
        if (movementAudioSource != null && movementAudioSource.isPlaying)
        {
            movementAudioSource.Stop();
        }
    }
}