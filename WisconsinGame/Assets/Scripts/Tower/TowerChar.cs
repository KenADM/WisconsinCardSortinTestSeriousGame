using System;
using UnityEngine;

/// <summary>
/// Holds the defining characteristics (Color, Type, People) for a specific tower platform.
/// </summary>
public class TowerChar : MonoBehaviour
{
    [Header("Tower Data")]
    [field: SerializeField] public HouseCombination TowerData { get; private set; }
    [SerializeField] public GameObject HouseBaseSpawn;

    public void Initialize(int colorNew, int typeNew, int peopleNew)
    {
        TowerData = new HouseCombination(colorNew, typeNew, peopleNew);
    }
    public override string ToString()
    {
        return $"Color: {TowerData.Color}, Type: {TowerData.Type}, People: {TowerData.People}";
    }
}