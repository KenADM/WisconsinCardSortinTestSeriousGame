using UnityEngine;

public class AlertRaiseStack : MonoBehaviour
{
    // trigger to raise and lower the house stack
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
            alignHouse.RaiseStack();
        }
    }

    void OnTriggerExit(Collider other)
    {
        //Debug.Log("AlertAlignment triggered by: " + other.name);
        if (alignHouse != null && other.CompareTag("Player"))
        {
            //Debug.Log("Calling OnTriggerAlert on AlignHouse");
            alignHouse.LowerStack();
        }
    }
}
