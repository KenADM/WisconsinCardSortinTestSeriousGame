using UnityEngine;
using UnityEngine.InputSystem;

public class HookTrigger : MonoBehaviour
{

    private GameObject objectToGrab; // Il blocco che stiamo solo sfiorando

    public Fish FishingRod;
    
    void OnTriggerEnter(Collider other)
    {
        // Controlliamo se l'oggetto ha l'etichetta giusta
        if (other.CompareTag("Grabbable"))
        {
            FishingRod.OnTriggerEnterOfBait(other.gameObject, transform.gameObject);
        }
    }
    
    // Questa funzione scatta quando la mano si allontana dall'oggetto
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Grabbable") && objectToGrab == other.gameObject)
        {
            FishingRod.OnTriggerExitOfBait();
        }
    }
}
