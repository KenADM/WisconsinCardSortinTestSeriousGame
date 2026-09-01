using UnityEngine;

/// <summary>
/// Detects when a valid, grabbable house enters the tower's trigger zone.
/// Alerts the parent InverseTowerHandler to initiate the house placement sequence.
/// </summary>
public class AlertPlacement : MonoBehaviour
{
    [Header("Tower Reference")]
    [SerializeField] private InverseTowerHandler alignHouse;

    private void Start()
    {
        // Fallback: Automatically find the parent handler if not manually assigned in the Inspector
        if (alignHouse == null)
        {
            alignHouse = GetComponentInParent<InverseTowerHandler>();
            
            if (alignHouse == null)
            {
                Debug.LogWarning("AlertPlacement on " + gameObject.name + " could not find an InverseTowerHandler in its parents!");
            }
        }
    }

    /// <summary>
    /// Triggered automatically by Unity when a collider enters this trigger zone.
    /// Verifies if the object is a valid house before commanding the tower to place it.
    /// </summary>
    /// <param name="other">The collider that entered the drop zone.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Only trigger placement if the handler exists, the object is a house, and the house is currently grabbable (not already locked)
        if (alignHouse != null && other.gameObject.TryGetComponent<HouseFunction>(out HouseFunction houseChar) && houseChar.IsGrabbable())
        {   
            Debug.Log("Valid house placed in tower trigger zone.");
            alignHouse.PlaceHouse(other.transform);
        }
    }
}