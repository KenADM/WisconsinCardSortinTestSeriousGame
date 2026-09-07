using UnityEngine;

/// <summary>
/// Acts as a sensor for the crane's hook, detecting when a valid house is within grabbing range.
/// Notifies the CraneMovement script to hover or unhover the detected house.
/// </summary>
public class GrabbableHouseSensor : MonoBehaviour
{
    [Header("Manager Reference")]
    [SerializeField] private CraneMovement craneManager;

    private void Start()
    {
        if (craneManager == null)
        {
            craneManager = GetComponentInParent<CraneMovement>();
            
            if (craneManager == null)
            {
                Debug.LogWarning("GrabbableHouseSensor on " + gameObject.name + " could not find a CraneMovement in its parents!");
            }
        }
    }

    /// <summary>
    /// Triggered continuously while another collider remains inside this trigger zone.
    /// If the object is a valid, grabbable house, it tells the crane to 'hover' it.
    /// </summary>
    /// <param name="other">The collider currently inside the sensor zone.</param>
    private void OnTriggerStay(Collider other)
    {
        // Check if the manager exists, if the object is a house, and if the house is allowed to be grabbed
        if (craneManager != null && other.gameObject.TryGetComponent<HouseFunction>(out HouseFunction houseFunction) && houseFunction.IsGrabbable())
        {
            //Debug.Log("AlertAlignment triggered by: " + other.name);
            craneManager.HoverHouse(other.gameObject);
        }
    }

    /// <summary>
    /// Triggered automatically by Unity when a collider leaves this trigger zone.
    /// Tells the crane to stop hovering the house if it was the one currently being tracked.
    /// </summary>
    /// <param name="other">The collider that exited the sensor zone.</param>
    private void OnTriggerExit(Collider other)
    {
        if (craneManager != null && other.gameObject.TryGetComponent<HouseFunction>(out _))
        {
            Debug.Log("Calling RemovedHouse on AlignHouse");
            craneManager.UnhoverHouse(other.gameObject);
        }
    }
}