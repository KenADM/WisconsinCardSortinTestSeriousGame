using UnityEngine;
using UnityEngine.InputSystem; // Added to use the New Input System

public class PlayerMovement : MonoBehaviour
{
    public Transform orientation;
    public float moveSpeed = 5f;

    private void Update()
    {
        // Safety check to ensure a keyboard is connected
        if (Keyboard.current == null) return;

        // Read the value of WASD keys (Returns 1 if pressed, 0 if not pressed)
        float w = Keyboard.current.wKey.ReadValue();
        float s = Keyboard.current.sKey.ReadValue();
        float a = Keyboard.current.aKey.ReadValue();
        float d = Keyboard.current.dKey.ReadValue();

        // Calculate axis values (-1 to 1)
        float verticalInput = w - s;
        float horizontalInput = d - a;

        // Calculate direction based on where the player is facing
        Vector3 moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        
        // Move the player
        transform.position += moveDirection.normalized * moveSpeed * Time.deltaTime;
    }
}