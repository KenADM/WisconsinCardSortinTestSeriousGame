using UnityEngine;

public class AlertAlignment : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("AlertAlignment triggered by: " + other.name);
        AlignHouse alignHouse = GetComponentInParent<AlignHouse>();
        if (alignHouse != null)
        {
            Debug.Log("Calling OnTriggerAlert on AlignHouse");
            alignHouse.OnTriggerAlert(other);
        }
    }
}
