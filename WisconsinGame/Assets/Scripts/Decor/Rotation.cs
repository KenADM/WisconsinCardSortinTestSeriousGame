using UnityEngine;

public class Rotation : MonoBehaviour
{
    [Header("Rotation Speed")]
    public float rotationSpeed = 50f;

    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}