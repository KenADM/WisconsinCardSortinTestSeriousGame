using UnityEngine;
using System;

public class CubeCombination : MonoBehaviour
{
    public int people;
    public int type;
    public int color;

    public CubeCombination(int typeNew,  int colorNew, int peopleNew)
    {
        this.people = peopleNew;
        this.type = typeNew;
        this.color = colorNew;   
    }

    public void Initialize(int typeNew,  int colorNew, int peopleNew)
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
