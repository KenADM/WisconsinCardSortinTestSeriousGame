using UnityEngine;

public class Gravity : MonoBehaviour
{
    [Header("Settings")]
    public float floorHeight = 0f;
    public float gravityForce = 9.8f;
    private bool gravityStopped = false;
    
    private Rigidbody rb;
    private float currentSpeed = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // deactivate gravity: this custom function will be the one used
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }
    }

    public void StopGravity()
    {
        gravityStopped = true;
        currentSpeed = 0f;
    }
    
    public void StartGravity()
    {
        gravityStopped = false;
    }
    
    void Update()
    {
        // if gravity is deactivated, do nothing
        if (gravityStopped) return;

        // if connected to gru, reset the velocity
        if (transform.parent != null)
        {
            currentSpeed = 0f;
            return;
        }

        // else fall from
        if (transform.position.y > floorHeight)
        {
            currentSpeed += gravityForce * Time.deltaTime;
            transform.position += Vector3.down * currentSpeed * Time.deltaTime;

            // if below/on the ground, stop
            if (transform.position.y <= floorHeight)
            {
                transform.position = new Vector3(transform.position.x, floorHeight, transform.position.z);
                currentSpeed = 0f;
            }
        }
    }
}