using UnityEngine;
using System;
using System.Collections.Generic;

public class PreviewHouseHandler : MonoBehaviour
{
    [Header("For Actual Game")]
    public List<Transform> platforms = new List<Transform>();
    [SerializeField] private GameObject player; // La gru
    public GameObject spawningPlatform;

    private bool isActive;
    
    private Transform currentClosestPlatform = null;

    void OnEnable()
    {
        Tutorial.StartGame += StartPreviewHandler;
    }

    void OnDisable()
    {
        Tutorial.StartGame -= StartPreviewHandler;
    }

    void Start()
    {
        isActive = false;
    }

    void Update()
    {
        if (isActive)
        {
            Transform closestPlatform = null;
            float minDistance = float.MaxValue;

            foreach (Transform tower in platforms)
            {
                float distance = Vector3.Distance(player.transform.position, tower.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPlatform = tower;
                }
            }

            float spawnDistance = Vector3.Distance(player.transform.position, spawningPlatform.transform.position);
            if (spawnDistance < minDistance)
            {
                closestPlatform = spawningPlatform.transform;
            }

            if (closestPlatform != currentClosestPlatform)
            {
                if (currentClosestPlatform != null && currentClosestPlatform != spawningPlatform.transform)
                {
                    if (currentClosestPlatform.TryGetComponent(out InverseTowerHandler oldTower))
                    {
                        oldTower.HidePreviewHouse();
                    }
                }

                currentClosestPlatform = closestPlatform;

                if (currentClosestPlatform != null && currentClosestPlatform != spawningPlatform.transform)
                {
                    if (currentClosestPlatform.TryGetComponent(out InverseTowerHandler newTower))
                    {
                        newTower.ShowPreviewHouse();
                    }
                }
            }
        }
    }

    void StartPreviewHandler()
    {   
        isActive = true;
    }
}