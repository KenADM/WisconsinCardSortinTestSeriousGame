using UnityEngine;


public class StringGravity : MonoBehaviour
{
    // gravity for the string
    [Tooltip("RodTip")]
    public Transform rodTip;


    void Update()
    {
        Vector3 newPosition = rodTip.position;
        newPosition.x += 0.07f;
        transform.position = newPosition;
        transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
    }
   
}
