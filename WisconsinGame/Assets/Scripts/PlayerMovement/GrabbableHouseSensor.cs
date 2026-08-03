using UnityEngine;

public class GrabbableHouseSensor : MonoBehaviour
{
    [SerializeField] private CraneMovement CraneManager;

    void Start()
    {
        alignHouse = GetComponentInParent<CraneMovement>();
    }

    void OnTriggerStay(Collider other)
    {
        //Debug.Log("AlertAlignment triggered by: " + other.name);
        if (CraneManager != null && other.GetComponent<HouseChar>().isGrabbable())
        {
            //Debug.Log("Calling OnTriggerAlert on AlignHouse");
            CraneManager.PlacedHouse(other);
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (CraneManager != null && other.GetComponent<HouseChar>().isGrabbable())
        {
            //Debug.Log("Calling RemovedHouse on AlignHouse");
            CraneManager.RemovedHouse(other);
        }
    }
}
