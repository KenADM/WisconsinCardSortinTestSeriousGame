using UnityEngine;

public class AlertRaiseStack : MonoBehaviour
{
    [SerializeField] private AlignHouse alignHouse;

    void Start()
    {
        alignHouse = GetComponentInParent<AlignHouse>();
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("AlertAlignment triggered by: " + other.name);
        if (alignHouse != null && other.CompareTag("Player"))
        {
            //Debug.Log("Calling OnTriggerAlert on AlignHouse");
            alignHouse.raiseStack();
        }
    }

    void OnTriggerExit(Collider other)
    {
        //Debug.Log("AlertAlignment triggered by: " + other.name);
        if (alignHouse != null && other.CompareTag("Player"))
        {
            //Debug.Log("Calling OnTriggerAlert on AlignHouse");
            alignHouse.lowerStack();
        }
    }
}
