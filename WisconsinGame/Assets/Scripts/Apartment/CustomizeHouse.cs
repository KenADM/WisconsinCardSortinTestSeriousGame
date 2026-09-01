using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Handles the visual customization of a spawned house, including coloring its inner walls 
/// and displaying the correct number of occupant characters.
/// </summary>
public class CustomizeHouse : MonoBehaviour
{
    [Header("Customization References")]
    [SerializeField] private List<Transform> innerWalls;
    [SerializeField] private List<Transform> people;

    /// <summary>
    /// Applies the specified material color to all inner walls of the house.
    /// </summary>
    /// <param name="color">The material to apply.</param>
    public void ColorHouse(Material color)
    {
        foreach (Transform wall in innerWalls)
        {
            if (wall != null && wall.TryGetComponent(out Renderer rend))
            {
                rend.material = color;
            }
        }
    }

    /// <summary>
    /// Activates the corresponding number of character GameObjects inside the house.
    /// </summary>
    /// <param name="numberOfPeople">The target number of people to display.</param>
    public void ShowPeople(int numberOfPeople)
    {
        for (int i = 0; i < numberOfPeople && i < people.Count; i++)
        {
            if (people[i] != null)
            {
                people[i].gameObject.SetActive(true);
            }
        }
    }
}