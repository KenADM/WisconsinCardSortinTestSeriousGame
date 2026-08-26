using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class CustomizeHouse : MonoBehaviour
{
    [SerializeField] private List<Transform> innerWalls;

    [SerializeField] private List<Transform> people;


    public void ColorHouse(Material color)
    {
        foreach (Transform wall in innerWalls)
        {
            if(wall.TryGetComponent(out Renderer rend)){
                rend.material = color;
            }
        }
        
    }

    public void ShowPeople(int numberOfPeople)
    {
        for(int i = 0; i < numberOfPeople && i <= people.Count; i++)
        {
            people[i].gameObject.SetActive(true);
        }
    }
}
