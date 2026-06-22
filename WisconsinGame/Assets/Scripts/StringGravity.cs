using UnityEngine;

public class StringGravity : MonoBehaviour
{

    [Tooltip("RodTip")]
    public Transform rodTip;

    void Update()
    {
        transform.position = rodTip.position;
        transform.rotation = Quaternion.identity; 
    }
    
}
