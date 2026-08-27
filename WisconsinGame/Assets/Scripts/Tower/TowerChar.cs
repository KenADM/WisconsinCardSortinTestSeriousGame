using System;
using UnityEngine;

public class TowerChar : MonoBehaviour
{
    [Header("Tower Data")]
    [field: SerializeField] public HouseCombination TowerData { get; private set; }
    [SerializeField] public GameObject HouseBaseSpawn;

    public void Initialize(int colorNew, int typeNew, int peopleNew)
    {
        TowerData = new HouseCombination(colorNew, typeNew, peopleNew);
    }
}