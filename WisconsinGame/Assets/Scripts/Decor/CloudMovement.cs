using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    [Header("Movement Speed")]
    public float movementSpeed = 0.1f;

    void Update()
    {
        transform.Translate(0f, 0f, movementSpeed * Time.deltaTime);
    }
}