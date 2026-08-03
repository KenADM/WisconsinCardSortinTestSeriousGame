using UnityEngine;

public class GrabbableHouseSensor : MonoBehaviour
{
    [SerializeField] private CraneMovement craneManager;

    void Start()
    {
        craneManager = GetComponentInParent<CraneMovement>();
    }

    void OnTriggerStay(Collider other)
    {
        //Debug.Log("AlertAlignment triggered by: " + other.name);
        if (craneManager != null && other.gameObject.TryGetComponent<HouseChar>(out _) && other.GetComponent<HouseChar>().isGrabbable())
        {
            //Debug.Log("Calling OnTriggerAlert on AlignHouse");
            craneManager.HoverHouse(other.gameObject);
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (craneManager != null && other.gameObject.TryGetComponent<HouseChar>(out _) && other.GetComponent<HouseChar>().isGrabbable())
        {
            //Debug.Log("Calling RemovedHouse on AlignHouse");
            craneManager.UnhoverHouse(other.gameObject);
        }
    }
}
