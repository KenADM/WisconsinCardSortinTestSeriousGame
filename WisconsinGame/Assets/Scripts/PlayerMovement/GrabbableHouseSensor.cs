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
        if (craneManager != null && other.gameObject.TryGetComponent<HouseFunction>(out HouseFunction houseFunction) && houseFunction.isGrabbable())
        {
            Debug.Log("AlertAlignment triggered by: " + other.name);
            craneManager.HoverHouse(other.gameObject);
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (craneManager != null && other.gameObject.TryGetComponent<HouseFunction>(out _))
        {
            Debug.Log("Calling RemovedHouse on AlignHouse");
            craneManager.UnhoverHouse(other.gameObject);
        }
    }
}
