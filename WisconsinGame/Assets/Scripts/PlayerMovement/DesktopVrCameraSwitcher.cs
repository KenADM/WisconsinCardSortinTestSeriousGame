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
    
    [Tooltip("Read-only view of the current mode. Set via ExperimentSettings at runtime.")]
    [SerializeField] private bool isDesktopMode = false;

    private void Awake()
    {
        isDesktopMode = !ExperimentSettings.vrMode;
    }
    
    private void Start()
    {
        if (isDesktopMode)
        {
            ActivateDesktop();
        }
        else
        {
            StartCoroutine(ActivateVR());
        }
    }

    /// <summary>
    /// Safely halts VR subsystems and enables the standard flat-screen desktop camera.
    /// </summary>
    private void ActivateDesktop()
    {
        // 1. Stop VR rendering and tracking (if it's running)
        if (XRGeneralSettings.Instance.Manager.isInitializationComplete)
        {
            XRGeneralSettings.Instance.Manager.StopSubsystems();
        }

        // 2. Turn OFF the VR rig
        vrCamera.SetActive(false);

        // 3. Turn ON the Desktop rig
        desktopCamera.SetActive(true);

        Debug.Log("Switched to Desktop Mode");
    }

    /// <summary>
    /// Initializes the XR subsystems if necessary, turns off the desktop rig, 
    /// and activates the VR headset tracking and rendering.
    /// </summary>
    private IEnumerator ActivateVR()
    {
        // 1. Initialize VR if it hasn't been initialized yet
        if (!XRGeneralSettings.Instance.Manager.isInitializationComplete)
        {
            yield return XRGeneralSettings.Instance.Manager.InitializeLoader();
        }

        // 2. Start VR rendering and tracking
        XRGeneralSettings.Instance.Manager.StartSubsystems();

        // 3. Turn OFF the Desktop rig
        desktopCamera.SetActive(false);

        // 4. Turn ON the VR rig
        vrCamera.SetActive(true);

        Debug.Log("Switched to VR Mode");
    }
}