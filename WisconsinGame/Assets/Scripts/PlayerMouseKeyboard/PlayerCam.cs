using UnityEngine;
using UnityEngine.InputSystem; // Added to use the New Input System

public class PlayerCam : MonoBehaviour
{
    public float sensX = 10f; // Note: You may need to lower your sensitivity values
    public float sensY = 10f; // because raw mouse deltas feel different in the new system!

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