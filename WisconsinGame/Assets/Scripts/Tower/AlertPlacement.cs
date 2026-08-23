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
        Debug.Log("house placed");
        if (alignHouse != null && other.gameObject.TryGetComponent<HouseFunction>(out HouseFunction houseChar) && houseChar.isGrabbable())
        {   
            alignHouse.PlaceHouse(other.transform);
        }


    }

}
