using UnityEngine;

public class FeedbackManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem smoke;
    [SerializeField] private AudioSource sound;
    
    [SerializeField] private Color positive = Color.white;
    [SerializeField] private Color negative = Color.black;
    [SerializeField] private AudioClip positiveSound;
    [SerializeField] private AudioClip negativeSound;
 

    public void Feedback(bool feedback)
    {
        if (smoke != null)
        {   
            var smokeModule = smoke.main;
            smokeModule.startColor = feedback ? positive : negative;
            smoke.Play();

            if(feedback)
            {
                sound.PlayOneShot(positiveSound, 1f);
            }
            else
            {
                sound.PlayOneShot(negativeSound, 1f);
            }

        }
        else
        {
            Debug.LogWarning("Negative particles not assigned in FeedbackManager on " + gameObject.name);
        }
    }
}
