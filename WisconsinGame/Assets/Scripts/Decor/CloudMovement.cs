using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    [Header("Movement Speed")]
    public float movementSpeed = 0.1f;

    void Update()
    {
        // This pushes the object forward along the Z-axis continuously 
        transform.Translate(0f, 0f, movementSpeed * Time.deltaTime);
    }
}