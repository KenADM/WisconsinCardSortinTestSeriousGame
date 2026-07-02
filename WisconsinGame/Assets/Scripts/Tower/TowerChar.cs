using UnityEngine;
using System;

public class TowerChar: MonoBehaviour
{
    public int people;
    public int type;
    public int color;

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
