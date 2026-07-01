using UnityEngine;
using System;

public class HouseChar : MonoBehaviour
{
    public int color;
    public int type;
    public int people;

    public void Initialize(int colorNew,  int typeNew, int peopleNew)
    {
        this.people = peopleNew;
        this.type = typeNew;
        this.color = colorNew;
    }

    public int GetColor()
    {
        return color;
    }
    public int GetTypeCube()
    {
        return type;
    }
    public int GetPeople()
    {
        return people;
    }

}
