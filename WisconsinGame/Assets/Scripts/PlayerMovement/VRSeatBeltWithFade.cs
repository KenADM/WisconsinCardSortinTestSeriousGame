using UnityEngine;
using UnityEngine.XR;
using System.Collections;

/// <summary>
/// Acts as a VR "seatbelt" by fading the player's vision to a given color if they physically lean 
/// or walk too far away from the center of the play area. 
/// Useful for keeping the player grounded and preventing them from clipping through virtual walls.
/// </summary>
public class VRSeatBeltWithFade : MonoBehaviour
{
    [Header("VR Rig References")]
    [Tooltip("Drag the Main Camera (player's head) here")]
    public Transform vrCamera;

    [Header("Directional Limits (Meters)")]
    public float maxOffsetRight = 0.6f;
    public float maxOffsetLeft = 0.6f;
    public float maxOffsetForward = 0.6f;
    public float maxOffsetBackward = 0.5f; 

    public float maxOffsetUp = 0.5f;
    public float maxOffsetDown = 1f;

    [Header("Color Fade Effect")]
    public CanvasGroup restrainCanvas;

    public float camX;
    public float camY;
    public float camZ;
    
    public float fadeSpeed = 10f;

    // Flag to prevent the seatbelt from engaging before the player puts the headset on
    private bool isOriginSet = false;

    private void Awake()
    {
        if (vrCamera == null)
        {
            Debug.LogError("VR Camera reference is missing. Please assign it in the inspector.");
        }

        if (restrainCanvas == null)
        {
            Debug.LogError("Restraining Canvas reference is missing. Please assign it in the inspector.");
        }
    }

    private void Start()
    {
        // Start checking for the headset proximity sensor
        if (ExperimentSettings.vrMode)
        {
            StartCoroutine(WaitForHeadsetToBeWorn());
        }
    }

    /// <summary>
    /// Coroutine that waits until the VR headset's internal proximity sensor detects a user's face.
    /// Once worn, it locks in the starting position for the seatbelt limits.
    /// </summary>
    private IEnumerator WaitForHeadsetToBeWorn()
    {
        bool headsetWorn = false;

        while (!headsetWorn)
        {
            // Get the VR Headset device
            InputDevice headDevice = InputDevices.GetDeviceAtXRNode(XRNode.Head);
            
            if (headDevice.isValid)
            {
                // Check if the proximity sensor detects a face
                if (headDevice.TryGetFeatureValue(CommonUsages.userPresence, out bool isPresent))
                {
                    headsetWorn = isPresent;
                }
                else
                {
                    // Fallback: If the headset lacks a proximity sensor, wait until it starts tracking (moves from origin)
                    if (vrCamera.localPosition != Vector3.zero)
                    {
                        headsetWorn = true;
                    }
                }
            }
            
            // Wait half a second before checking again to save performance
            yield return new WaitForSeconds(0.5f);
        }

        // Headset is worn! Set the center coordinates.
        camX = vrCamera.localPosition.x;
        camY = vrCamera.localPosition.y; 
        camZ = vrCamera.localPosition.z;

        isOriginSet = true;
    }

    /// <summary>
    /// Continuously checks the VR headset's local position. 
    /// Calculates how close the player is to the boundary and applies the color fade accordingly.
    /// Uses LateUpdate to ensure it calculates after all VR tracking updates have finished for the frame.
    /// </summary>
    private void LateUpdate()
    {
        // Do not calculate seatbelt fades until the player has actually put the headset on
        if (!isOriginSet || vrCamera == null || restrainCanvas == null) return;

        // Read the physical position of the player's head in the room
        Vector3 localPos = vrCamera.localPosition;

        float ratioX = 0f;
        float ratioZ = 0f;
        float ratioY = 0f;

        // Calculate the delta (distance) from the starting position
        float deltaX = localPos.x - camX;
        float deltaY = localPos.y - camY;
        float deltaZ = localPos.z - camZ;

        // Calculate the percentage for the X axis (Right / Left)
        if (deltaX > 0)
        {
            ratioX = deltaX / maxOffsetRight;
        }
        else if (deltaX < 0)
        {
            ratioX = Mathf.Abs(deltaX) / maxOffsetLeft;
        }

        // Calculate the percentage for the Z axis (Forward / Backward)
        if (deltaZ > 0)
        {
            ratioZ = deltaZ / maxOffsetForward;
        }
        else if (deltaZ < 0)
        {
            ratioZ = Mathf.Abs(deltaZ) / maxOffsetBackward;
        }
        
        // Calculate the percentage for the Y axis (Up / Down)
        if (deltaY > 0)
        {
            ratioY = deltaY / maxOffsetUp;
        }
        else if (deltaY < 0)
        {
            ratioY = Mathf.Abs(deltaY) / maxOffsetDown;
        }
        
        // Take the direction where the player leaned the most (clamped to a maximum of 1)
        float targetAlpha = Mathf.Clamp01(Mathf.Max(ratioX, ratioZ, ratioY));

        // Apply the opacity: the further away from the center towards one of the 6 limits, the more faded it becomes
        restrainCanvas.alpha = Mathf.MoveTowards(restrainCanvas.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
    }
}