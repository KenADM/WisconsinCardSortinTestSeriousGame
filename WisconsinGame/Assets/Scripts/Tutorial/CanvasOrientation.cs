using UnityEngine;

public class CanvasOrientation : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("VR headset")]
    public Transform vrCamera;

    [Header("Settings")]
    [Tooltip("Lock vertival rotation")]
    public bool lockRotationUpright = true;


    /// <summary>
    /// Adjust rotation for canvases to face the player.
    /// LateUpdate waits for camera to finish the movement before calculating the orientation.
    /// </summary>
    void LateUpdate()
    {
        if (vrCamera == null) return;

        // Calculate direction
        Vector3 lookDirection = transform.position - vrCamera.position;

        // Flatten the Y axis to keep it upright BEFORE making the Quaternion if lockRotationUprights is active
        if (lockRotationUpright)
        {
            lookDirection.y = 0;
        }

        // Now apply the rotation safely
        if (lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }
}