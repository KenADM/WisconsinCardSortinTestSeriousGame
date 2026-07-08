using UnityEngine;
using System;

public class HouseChar : MonoBehaviour
{
    [SerializeField] private int color;
    [SerializeField] private int type;
    [SerializeField] private int people;

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
