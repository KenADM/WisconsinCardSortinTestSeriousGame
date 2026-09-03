using System;
using UnityEngine;

[Serializable]

/// <summary>
/// Holds the defining characteristics (Color, Type, People) for a specific house.
/// </summary>
public class HouseCombination 
{
    [field: SerializeField] public int Color { get; private set; }
    [field: SerializeField] public int Type { get; private set; }
    [field: SerializeField] public int People { get; private set; }

    public HouseCombination(int colorNew, int typeNew, int peopleNew)
    {
        Color = colorNew;
        Type = typeNew;
        People = peopleNew;   
    }
}