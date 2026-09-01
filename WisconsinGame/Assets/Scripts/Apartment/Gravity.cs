using UnityEngine;

/// <summary>
/// Custom gravity script that handles accelerated falling logic, floor collision snapping,
/// and suspension handling when attached to the crane.
/// </summary>
public class Gravity : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] public float floorHeight = 0f;
    [SerializeField] public float gravityForce = 9.8f;
    
    private bool gravityStopped = false;
    private float currentSpeed = 0f;

    private void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        // Deactivate default physics gravity: this custom function will be the one used
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }
    }

    /// <summary>
    /// Stops the gravity simulation and resets the falling velocity.
    /// </summary>
    public void StopGravity()
    {
        gravityStopped = true;
        currentSpeed = 0f;
    }
    
    /// <summary>
    /// Resumes the gravity simulation.
    /// </summary>
    public void StartGravity()
    {
        gravityStopped = false;
    }
    
    private void Update()
    {
        // If gravity is deactivated, do nothing
        if (gravityStopped) return;

        // If connected to the crane, reset the velocity
        if (transform.parent != null)
        {
            currentSpeed = 0f;
            return;
        }

        // Else, fall from height
        if (transform.position.y > floorHeight)
        {
            currentSpeed += gravityForce * Time.deltaTime;
            transform.position += Vector3.down * currentSpeed * Time.deltaTime;

            // If below or on the ground, stop
            if (transform.position.y <= floorHeight)
            {
                transform.position = new Vector3(transform.position.x, floorHeight, transform.position.z);
                currentSpeed = 0f;
            }
        }
    }
    public void SetFloorHeight(float newHeight)
    {
        floorHeight = newHeight;
    }
}