using UnityEngine;
using UnityEngine.InputSystem;

public class HouseFunctions : MonoBehaviour
{
    private Rigidbody rb;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    void Awake()
    {
        rb = this.gameObject.GetComponent<Rigidbody>();
        grabInteractable = this.gameObject.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        
    }

    public void Grab( )
    {
        this.gameObject.tag = "Grabbed";

    }

    public void Release( ){

        this.gameObject.tag = "Grabbable";

    }

    public void MakeUnmovable(){
        this.gameObject.tag = "Aligned";
        if (rb != null)
        {
            rb.isKinematic = true; // Disabilita la fisica per il blocco allineato
            rb.useGravity = false; // Disabilita la gravità per il blocco allineato
        }

        if(grabInteractable != null)
        {
            grabInteractable.enabled = false;
        }

    }
}