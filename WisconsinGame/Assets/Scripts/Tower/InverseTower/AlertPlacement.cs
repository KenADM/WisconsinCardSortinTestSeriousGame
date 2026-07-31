using UnityEngine;

public class AlertPlacement : MonoBehaviour
{
    // handles the placement of a house
    [SerializeField] private InverseTowerHandler alignHouse;

    void Start()
    {
        alignHouse = GetComponentInParent<InverseTowerHandler>();
    }

    void OnTriggerStay(Collider other)
    {
        if (alignHouse != null && (other.CompareTag("Grabbable") || other.CompareTag("Aligned"))) // da cambiare
        {
            alignHouse.PlaceHouse(other.transform);
        }

    }

}
