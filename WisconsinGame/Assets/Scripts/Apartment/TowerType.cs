using UnityEngine;
using System;

public class TowerType : MonoBehaviour
{
    public int people;
    public int type;
    public int color;

    public TowerType(int color, int type, int people)
    {
        this.people = people;
        this.type = type;
        this.color = color;
    }
    
}
