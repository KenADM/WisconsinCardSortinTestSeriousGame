using UnityEngine;

/// <summary>
/// Forces position on assigned object.
/// Used to solve bug if disconnection of vr occur while in game.
/// </summary>
public class VRConnectionHandler : MonoBehaviour
{

    private void OnEnable()
    {
        // Subscribe to the absolute last moment before the frame is drawn
        Application.onBeforeRender += ForceHeightToZero;
    }

    private void OnDisable()
    {
        Application.onBeforeRender -= ForceHeightToZero;
    }

    private void LateUpdate()
    {
        // Also enforce it during standard physics/update cycles
        ForceHeightToZero();
    }

    private void ForceHeightToZero()
    {
        // Check if the VR system tried to push the height up
        if (transform.localPosition.y != 0f)
        {
            // Keep the X and Z exactly where the VR tracking put them, but crush Y to 0
            Vector3 lockedPosition = transform.localPosition;
            lockedPosition.y = 0f;
            transform.localPosition = lockedPosition;
        }
    }
}