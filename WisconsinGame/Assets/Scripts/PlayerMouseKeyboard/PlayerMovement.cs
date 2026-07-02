using UnityEngine;
using UnityEngine.InputSystem; 

public class PlayerMovement : MonoBehaviour
{
    public Transform orientation;
    public float moveSpeed = 5f;

    private Rigidbody rb;
    private float horizontalInput;
    private float verticalInput;

    private void Start()
    {
        // Automatically grab the Rigidbody attached to the player
        rb = GetComponent<Rigidbody>();
        
        // Ensure rotation is frozen via code just in case it was missed in the Inspector
        rb.freezeRotation = true;
    }

    private void Update()
    {
        // Safety check to ensure a keyboard is connected
        if (Keyboard.current == null) return;

        // Get inputs in Update (which runs every frame)
        float w = Keyboard.current.wKey.ReadValue();
        float s = Keyboard.current.sKey.ReadValue();
        float a = Keyboard.current.aKey.ReadValue();
        float d = Keyboard.current.dKey.ReadValue();

        verticalInput = w - s;
        horizontalInput = d - a;
    }

    private void FixedUpdate()
    {
        // Move the player in FixedUpdate (which is meant for physics calculations)
        Vector3 moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        
        // Calculate the target velocity based on input and speed
        Vector3 targetVelocity = moveDirection.normalized * moveSpeed;
        
        // Keep the Rigidbody's current Y velocity so gravity still pulls the player down!
        targetVelocity.y = rb.linearVelocity.y;

        // Apply the velocity to the Rigidbody
        rb.linearVelocity = targetVelocity;
    }
}