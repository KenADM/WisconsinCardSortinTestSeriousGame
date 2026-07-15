using UnityEngine;
using UnityEngine.InputSystem;

public class HouseFunctions : MonoBehaviour
{
    // used to change state of a grabbed, ungrabbed and placed house

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
            rb.isKinematic = true; // Disable movement for aligned houses
            rb.useGravity = false; // Disable gravity for algned houses
        }

        if(grabInteractable != null)
        {
            grabInteractable.enabled = false;
        }

    }
}