using UnityEngine;

public class ManualGravity : MonoBehaviour
{
    public float gravityForce = 9.8f;
    public float floorHeight = 0f; // La Y del tuo pavimento
    private float currentVelocity = 0f;
    private Quaternion startRotation;

    void Start()
    {
        startRotation = transform.rotation;
    }

    void Update()
    {
        transform.rotation = startRotation;

        if (transform.parent != null) 
        {
            currentVelocity = 0f; 
            return; 
        }

        // Calcoliamo dove si trova la base del cubo (il fondo)
        float bottomEdge = transform.position.y - (transform.localScale.y / 2f);

        // Se il fondo è sopra il pavimento, cadiamo
        if (bottomEdge > floorHeight)
        {
            currentVelocity += gravityForce * Time.deltaTime;
            transform.Translate(Vector3.down * currentVelocity * Time.deltaTime, Space.World);
        }
        else
        {
            // Se siamo arrivati al pavimento, blocchiamo la posizione a filo
            currentVelocity = 0f;
            transform.position = new Vector3(transform.position.x, floorHeight + (transform.localScale.y / 2f), transform.position.z);
        }
    }
}