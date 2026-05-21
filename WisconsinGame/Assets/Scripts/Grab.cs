using UnityEngine;
using UnityEngine.InputSystem;

public class Grab : MonoBehaviour
{
    [Header("Tasto Presa")]
    public InputAction grabAction;

    private GameObject objectToGrab; // Il blocco che stiamo solo sfiorando
    private GameObject grabbedObject; // Il blocco che abbiamo effettivamente in mano

    void Start()
    {
        grabAction.Enable();
    }


    void Update()
    {
        // Legge quanto è premuto il tasto scelto da 0 a 1
        float grabValue = grabAction.ReadValue<float>();

        // Se premo il tasto, ho un oggetto a tiro, e non ho già le mani piene -> AFFERRA
        if (grabValue > 0.5f && grabbedObject == null && objectToGrab != null && objectToGrab.transform.parent == null && objectToGrab.CompareTag("Grabbable"))
        {
            GrabObject();
        }
        // Se rilascio il tasto e ho un oggetto in mano -> LASCIA
        else if (grabValue < 0.5f && grabbedObject != null)
        {
            ReleaseObject();
        }
    }

    // Questa funzione scatta automaticamente quando la mano "entra" in un oggetto
    void OnTriggerEnter(Collider other)
    {
        // Controlliamo se l'oggetto ha l'etichetta giusta
        if (other.CompareTag("Grabbable"))
        {
            objectToGrab = other.gameObject;
        }
    }

    // Questa funzione scatta quando la mano si allontana dall'oggetto
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Grabbable") && objectToGrab == other.gameObject)
        {
            objectToGrab = null;
        }
    }

    void GrabObject()
    {        
        // Facciamo diventare il blocco un "figlio" della mano, così si muove insieme a noi
        grabbedObject = objectToGrab;
        grabbedObject.transform.SetParent(transform);
    }

    void ReleaseObject()
    {
        // Togliamo la parentela. Il blocco resterà esattamente dove lo abbiamo lasciato. (sta all'oggerto decidere se cadere o restare sospeso)
        grabbedObject.transform.SetParent(null);
        grabbedObject = null;
    }
}