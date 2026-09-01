using UnityEngine;

/// <summary>
/// Simulates gravity for the crane's string by constantly aligning it with the rod tip 
/// and forcing it to hang straight down.
/// </summary>
public class StringGravity : MonoBehaviour
{
    [Header("References")]
    [Tooltip("RodTip")]
    public Transform rodTip;

    /// <summary>
    /// Updates the string's position and rotation every frame to follow the rod tip 
    /// with a slight X-axis offset, keeping it pointing strictly downwards.
    /// </summary>
    private void Update()
    {
        Vector3 newPosition = rodTip.position;
        newPosition.x += 0.07f;
        transform.position = newPosition;
        transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
    }
}