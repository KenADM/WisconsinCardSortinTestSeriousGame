using UnityEngine;
using System;

public class HouseChar : MonoBehaviour
{
    private int color;
    private int type;
    private int people;

    public void Initialize(int colorNew,  int typeNew, int peopleNew)
    {
        this.people = peopleNew;
        this.type = typeNew;
        this.color = colorNew;
    }

    public int GetColorHouse()
    {
        return color;
    }
    public int GetTypeHouse()
    {
        return type;
    }
    public int GetPeopleHouse()
    {
        return people;
    }

}
