using Unity.XR.CoreUtils;
using System;
using UnityEngine;

public class wentRightTriggerTutorial : MonoBehaviour
{
    public static event Action spawnTutorial;
    private bool triggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return; // Prevent multiple triggers
        spawnTutorial?.Invoke();
        //this.gameObject.SetActive(false);               
        triggered = true;

    }
    void OnTriggerExit(Collider other)
    {
        triggered = false;
    }
}
