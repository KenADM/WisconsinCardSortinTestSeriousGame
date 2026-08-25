using System;
using UnityEngine;

[Serializable]
public class HouseCombination 
{
    [field: SerializeField] public int color { get; private set; }
    [field: SerializeField] public int type { get; private set; }
    [field: SerializeField] public int people { get; private set; }

    public HouseCombination(int colorNew, int typeNew, int peopleNew)
    {
        color = colorNew;
        type = typeNew;
        people = peopleNew;   
    }
}