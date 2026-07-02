using UnityEngine;

public class AlertAlignment : MonoBehaviour
{
    private AlignHouse alignHouse;

    void Start()
    {
        alignHouse = GetComponentInParent<AlignHouse>();
    }

    void OnTriggerStay(Collider other)
    {
        //Debug.Log("AlertAlignment triggered by: " + other.name);
        if (alignHouse != null && other.CompareTag("Grabbable"))
        {
            //Debug.Log("Calling OnTriggerAlert on AlignHouse");
            alignHouse.PlacedHouse(other);
        }
    }

    void OnTriggerExit(Collider other)
    {
        //Debug.Log("CIAOOOO triggered by: " + other.name);
        if (alignHouse != null && other.CompareTag("Grabbed"))
        {
            //Debug.Log("Calling RemovedHouse on AlignHouse");
            alignHouse.RemovedHouse(other);
        }
    }
}
