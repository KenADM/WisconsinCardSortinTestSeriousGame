using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Management;
using System.Collections;

public class DesktopvrCameraSwitcher : MonoBehaviour
{
    [Header("Rigs")]
    public GameObject vrCamera;
    public GameObject desktopCamera;

    [Header("Input (Press to Switch)")]
    public bool isDesktopMode = false; 
    void Start()
    {
        // Set the initial state when the game starts
        if (isDesktopMode)
        {
            ActivateDesktop();
        }
        else
        {
            ActivateVR();
        }
    }

    void Update()
    {

    }

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