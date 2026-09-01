using UnityEngine;

/// <summary>
/// Controls the individual state of a spawned house, including its defining characteristics (HouseData),
/// physical movement permissions (grabbable/locked), and gravity interactions when grabbed or released.
/// </summary>
public class HouseFunction : MonoBehaviour
{
    [Header("House Data")]
    [field: SerializeField] public HouseCombination HouseData { get; private set; } 
    private Gravity myGravity; 
    private bool isMovable = true;
    
    private void Awake()
    {
        myGravity = GetComponent<Gravity>(); 
    }
    
    /// <summary>
    /// Assigns the specific color, type, and people configuration to this house.
    /// </summary>
    /// <param name="colorNew">The color index.</param>
    /// <param name="typeNew">The architectural type index.</param>
    /// <param name="peopleNew">The number of occupants.</param>
    public void Initialize(int colorNew, int typeNew, int peopleNew)
    {
        HouseData = new HouseCombination(colorNew, typeNew, peopleNew);
    }

    /// <summary>
    /// Checks whether the house is currently free to be grabbed by the crane.
    /// </summary>
    public bool IsGrabbable() 
    { 
        return isMovable; 
    }

    /// <summary>
    /// Permanently locks the house in place (e.g., once placed onto a tower platform).
    /// </summary>
    public void MakeUnmovable() 
    { 
        isMovable = false; 
    }

    /// <summary>
    /// Halts gravity simulation when the crane picks up the house.
    /// </summary>
    public void Grab()
    {
        if (myGravity != null) myGravity.StopGravity();
    }

    /// <summary>
    /// Restarts gravity simulation when the house is released by the crane.
    /// </summary>
    public void Release()
    {
        if (myGravity != null) myGravity.StartGravity();
    }
}