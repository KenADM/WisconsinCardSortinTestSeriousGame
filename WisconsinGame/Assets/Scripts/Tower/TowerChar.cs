using UnityEngine;
using System;

public class TowerChar: MonoBehaviour
{
    [SerializeField] private int people;
    [SerializeField] private int type;
    [SerializeField] private int color;

    public void Initialize(int colorNew,  int typeNew, int peopleNew)
    {
        this.people = peopleNew;
        this.type = typeNew;
        this.color = colorNew;
    }
    
    public int GetPeopleTower()
    {
        return this.people;
    }
    public int GetTypeTower()
    {
        return this.type;
    }
    public int GetColorTower()
    {
        return this.color;
    }
    
}
