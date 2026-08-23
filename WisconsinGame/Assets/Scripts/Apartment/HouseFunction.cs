using UnityEngine;
using System;

public class HouseFunction : MonoBehaviour
{
    [SerializeField] private int color;
    [SerializeField] private int type;
    [SerializeField] private int people;
    
    private Rigidbody rb;
    private Gravity myGravity; // Il riferimento privato per la gravità

    public bool isMovable = true;
    
    void Awake()
    {
        rb = this.gameObject.GetComponent<Rigidbody>();
        myGravity = this.gameObject.GetComponent<Gravity>(); 
    }
    
    public void Initialize(int colorNew,  int typeNew, int peopleNew)
    {
        this.people = peopleNew;
        this.type = typeNew;
        this.color = colorNew;
    }

    // --- GETTERS E SETTERS ---
    public int GetColorHouse() { return color; }
    public int GetTypeHouse() { return type; }
    public int GetPeopleHouse() { return people; }

    public void SetColorHouse(int color) { this.color = color; }
    public void SetTypeHouse(int type) { this.type = type; }
    public void SetPeopleHouse(int people) { this.people = people; }

    // --- LOGICA DI PRESA (GRAB) E RILASCIO ---
    
    public bool isGrabbable()
    {
        return isMovable;
    }

    public void MakeUnmovable()
    {
        isMovable = false;
    }

    public void Grab()
    {
        if (myGravity != null)
        {
            myGravity.StopGravity();
        }
    }

    public void Release()
    {
        if (myGravity != null)
        {
            myGravity.StartGravity();
        }
        
        // NOTA: Se vuoi che la casa possa essere ripresa dopo essere stata 
        // lasciata cadere nel vuoto (o su una piattaforma sbagliata), 
        // puoi de-commentare la riga qui sotto:
        // isMovable = true; 
    }
}