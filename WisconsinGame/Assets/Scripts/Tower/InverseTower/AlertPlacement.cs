using UnityEngine;

public class AlertPlacement : MonoBehaviour
{
    // handles the placement of a house
    [SerializeField] private InverseTowerHandler alignHouse;

    void Start()
    {
        alignHouse = GetComponentInParent<InverseTowerHandler>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (alignHouse != null && other.gameObject.TryGetComponent<HouseChar>(out _)) 
        {
            alignHouse.PlaceHouse(other.transform);
        }

    }

}
