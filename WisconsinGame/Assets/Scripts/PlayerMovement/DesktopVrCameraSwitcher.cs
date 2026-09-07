using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Management;
using System.Collections;

/// <summary>
/// Handles the initialization and switching between the Desktop camera and the VR camera rigs 
/// based on the global experiment settings.
/// </summary>
public class DesktopvrCameraSwitcher : MonoBehaviour
{
    [Header("Rigs")]
    [SerializeField] private GameObject vrCamera;
    [SerializeField] private GameObject desktopCamera;

    private void Start()
    {
        if (ExperimentSettings.vrMode)
        {
            StartCoroutine(ActivateVR());
        }
        else
        {
            ActivateDesktop();
            
        }
    }

    /// <summary>
    /// Safely halts VR subsystems and enables the standard flat-screen desktop camera.
    /// </summary>
    private void ActivateDesktop()
    {
        // Stop VR rendering and tracking (if it's running)
        if (XRGeneralSettings.Instance.Manager.isInitializationComplete)
        {
            XRGeneralSettings.Instance.Manager.StopSubsystems();
        }

        vrCamera.SetActive(false);
        desktopCamera.SetActive(true);

        Debug.Log("Switched to Desktop Mode");
    }

    /// <summary>
    /// Initializes the XR subsystems if necessary, turns off the desktop rig, 
    /// and activates the VR headset tracking and rendering.
    /// </summary>
    private IEnumerator ActivateVR()
    {
        // Initialize VR if it hasn't been initialized yet
        if (!XRGeneralSettings.Instance.Manager.isInitializationComplete)
        {
            yield return XRGeneralSettings.Instance.Manager.InitializeLoader();
        }

        // Start VR rendering and tracking
        XRGeneralSettings.Instance.Manager.StartSubsystems();

        desktopCamera.SetActive(false);
        vrCamera.SetActive(true);

        Debug.Log("Switched to VR Mode");
    }
}