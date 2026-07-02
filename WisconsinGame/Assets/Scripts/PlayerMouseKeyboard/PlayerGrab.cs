using UnityEngine;
using UnityEngine.InputSystem; 

public class PlayerGrab : MonoBehaviour
{
    [Header("Grab Settings")]
    public Camera playerCamera;
    public float grabRange = 3f;

    private GameObject heldObject;
    private Rigidbody heldObjectRb;
    
    // We will use this to remember how far away the object was when grabbed
    private float currentHoldDistance; 

    private void Update()
    {
        // Safety check for mouse
        if (Mouse.current == null) return;

        // Check if the Left Mouse Button was clicked this frame
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (heldObject == null)
            {
                TryGrab();
            }
            else
            {
                DropObject();
            }
        }

        // If holding an object, calculate the target position based on the camera's
        // forward direction and the exact distance it was grabbed at.
        if (heldObject != null)
        {
            Vector3 targetPosition = playerCamera.transform.position + playerCamera.transform.forward * currentHoldDistance;
            heldObject.transform.position = targetPosition;
        }
    }

private void TryGrab()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        
        if (Physics.Raycast(ray, out RaycastHit hit, grabRange))
        {
            // Check if the object we hit has the "Grabbable" tag BEFORE trying to grab it
            if (hit.collider.CompareTag("Grabbable"))
            {
                if (hit.collider.TryGetComponent(out Rigidbody rb))
                {
                    heldObject = hit.collider.gameObject;
                    heldObjectRb = rb;

                    // Calculate the distance between the camera and the object right now
                    currentHoldDistance = Vector3.Distance(playerCamera.transform.position, heldObject.transform.position);

                    // Turn off gravity and make it kinematic so it doesn't fall
                    heldObjectRb.useGravity = false;
                    heldObjectRb.isKinematic = true;
                    heldObject.tag = "Grabbed";
                }
            }
        }
    }

    private void DropObject()
    {
        // Turn physics back on
        heldObjectRb.useGravity = true;
        heldObjectRb.isKinematic = false;
        heldObject.tag = "Grabbable";
    

        // Clear our variables
        heldObject = null;
        heldObjectRb = null;
    }
}