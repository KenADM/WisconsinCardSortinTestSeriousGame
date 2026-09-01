using UnityEngine;

/// <summary>
/// Acts as a VR "seatbelt" by fading the player's vision to red if they physically lean 
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

    [Header("Red Fade Effect")]
    [Tooltip("Drag the red screen Canvas Group here")]
    public CanvasGroup redFadeCanvas;
    
    public float fadeSpeed = 10f;
    public float ciao;

    /// <summary>
    /// Continuously checks the VR headset's local position. 
    /// Calculates how close the player is to the boundary and applies the red fade accordingly.
    /// Uses LateUpdate to ensure it calculates after all VR tracking updates have finished for the frame.
    /// </summary>
    private void LateUpdate()
    {
        if (vrCamera == null || redFadeCanvas == null) return;

        // 1. Read the physical position of the player's head in the room
        Vector3 localPos = vrCamera.localPosition;

        float ratioX = 0f;
        float ratioZ = 0f;

        // 2. Calculate the percentage for the X axis (Right / Left)
        if (localPos.x > 0)
        {
            ratioX = localPos.x / maxOffsetRight;
        }
        else if (localPos.x < 0)
        {
            ratioX = Mathf.Abs(localPos.x) / maxOffsetLeft;
        }

        // 3. Calculate the percentage for the Z axis (Forward / Backward)
        if (localPos.z > 0)
        {
            ratioZ = localPos.z / maxOffsetForward;
        }
        else if (localPos.z < 0)
        {
            ratioZ = Mathf.Abs(localPos.z) / maxOffsetBackward;
        }
            
        // 4. Take the direction where the player leaned the most (clamped to a maximum of 1)
        float targetAlpha = Mathf.Clamp01(Mathf.Max(ratioX, ratioZ));

        // 5. Apply the opacity: the further away from the center towards one of the 4 limits, the redder it becomes
        redFadeCanvas.alpha = Mathf.MoveTowards(redFadeCanvas.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
    }
}