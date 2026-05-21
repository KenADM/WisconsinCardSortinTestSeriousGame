using UnityEngine;
using UnityEngine.InputSystem; // This is required to read OpenXR inputs

public class CustomVRInput : MonoBehaviour
{
    [Header("Controller Inputs")]
    // This creates an empty slot in the Unity Editor where you can plug in any button
    public InputActionProperty triggerButton;
    public InputActionProperty primaryButton; // Usually the 'A' or 'X' button

    void Update()
    {
        // 1. READ A FLOAT (0.0 to 1.0)
        // Perfect for triggers, so you know exactly how far down it is squeezed
        float triggerValue = triggerButton.action.ReadValue<float>();
        
        if (triggerValue > 0.5f)
        {
            Debug.Log("Trigger is squeezed past halfway! Value: " + triggerValue);
        }

        // 2. READ A BUTTON PRESS (True or False)
        // WasPressedThisFrame() ensures this only triggers once per click, perfect for jumping or firing
        if (primaryButton.action.WasPressedThisFrame())
        {
            Debug.Log("Primary button was clicked!");
        }
    }
}