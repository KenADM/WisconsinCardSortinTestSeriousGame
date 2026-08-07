using Unity.XR.CoreUtils;
using UnityEngine;

public class wentLeftTriggerTutorial : MonoBehaviour
{
    [SerializeField] private GameObject wentRightTrigger;
    private bool triggered = false;
    void OnTriggerEnter(Collider other)
    {

        if (triggered) return; // Prevent multiple triggers

        if(wentRightTrigger != null)
        {
            wentRightTrigger.SetActive(true);
        }
        triggered = true;
    }
    void OnTriggerExit(Collider other)
    {
        triggered = false;
    }
    
}
