using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles the player's camera movement based on mouse input, allowing for smooth rotation and orientation control in a first-person perspective.
/// </summary>
public class PlayerCam : MonoBehaviour
{
    public float sensX = 10f; 
    public float sensY = 10f; 

    public Transform Orientation;

    float xRotation;
    float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Safety check to ensure a mouse is connected
        if (Mouse.current == null) return;

        // Get raw mouse delta (movement in pixels) from the new system
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        // Apply sensitivity and Time.deltaTime
        float mouseX = mouseDelta.x * Time.deltaTime * sensX;
        float mouseY = mouseDelta.y * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // rotate cam and orientation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        Orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}