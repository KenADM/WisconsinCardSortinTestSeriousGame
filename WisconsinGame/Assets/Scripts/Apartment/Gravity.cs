using UnityEngine;

public class Gravity : MonoBehaviour
{
    [Header("Settings")]
    public float floorHeight = 0f;
    public float gravityForce = 9.8f;
    
    [Header("LIVE DEBUG: Watch this!")]
    [Tooltip("If this is TRUE, another script (like HouseChar) told it to stop.")]
    public bool gravityStopped = false;
    
    private Rigidbody rb;
    public float currentSpeed = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
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
        // 1. Se la gravità è stata messa in pausa da un altro script, fermati qui.
        if (gravityStopped) return;

        // 2. Se è attaccata alla gru, resetta la velocità e non cadere.
        if (transform.parent != null)
        {
            currentSpeed = 0f;
            return;
        }

        // 3. Cadi SOLO se ti trovi più in alto del pavimento
        if (transform.position.y > floorHeight)
        {
            currentSpeed += gravityForce * Time.deltaTime;
            transform.position += Vector3.down * currentSpeed * Time.deltaTime;

            // 4. Se hai oltrepassato o toccato il pavimento, bloccati esattamente lì
            if (transform.position.y <= floorHeight)
            {
                transform.position = new Vector3(transform.position.x, floorHeight, transform.position.z);
                currentSpeed = 0f;
                // NOTA: Non impostiamo gravityStopped = true qui. 
                // Semplicemente smette di cadere perché Y non è più maggiore di floorHeight!
            }
        }
    }
}