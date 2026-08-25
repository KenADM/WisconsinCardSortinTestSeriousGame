using UnityEngine;

public class HouseFunction : MonoBehaviour
{
    [Header("House Data")]
    [field: SerializeField] public HouseCombination HouseData { get; private set; } 
    
    private Rigidbody rb;
    private Gravity myGravity; 
    private bool isMovable = true;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        myGravity = GetComponent<Gravity>(); 
    }
    
    public void Initialize(int colorNew, int typeNew, int peopleNew)
    {
        HouseData = new HouseCombination(colorNew, typeNew, peopleNew);
    }

    public bool isGrabbable() { return isMovable; }
    public void MakeUnmovable() { isMovable = false; }

    public void Grab()
    {
        if (myGravity != null) myGravity.StopGravity();
    }

    public void Release()
    {
        if (myGravity != null) myGravity.StartGravity();
    }
}