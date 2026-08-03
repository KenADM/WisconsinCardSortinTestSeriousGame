using UnityEngine;
using System;

public class HouseChar : MonoBehaviour
{
    [SerializeField] private int color;
    [SerializeField] private int type;
    [SerializeField] private int people;

    private bool isMovable = true;

    private Rigidbody rb;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    void Awake()
    {
        rb = this.gameObject.GetComponent<Rigidbody>();
        grabInteractable = this.gameObject.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        
    }
    public void Initialize(int colorNew,  int typeNew, int peopleNew)
    {
        this.people = peopleNew;
        this.type = typeNew;
        this.color = colorNew;

    }

    public int GetColorHouse()
    {
        return color;
    }
    public int GetTypeHouse()
    {
        return type;
    }
    public int GetPeopleHouse()
    {
        return people;
    }

    public void MakeUnmovable()
    {
        isMovable = false;

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

    public void Release()
    {
        isMovable = true;

        if (rb != null)
        {
            rb.isKinematic = false; // Enable movement for released houses
            rb.useGravity = true; // Enable gravity for released houses
        }

        if(grabInteractable != null)
        {
            grabInteractable.enabled = true;
        }
    }

    public bool isGrabbable()
    {
        return isMovable;
    }

}
