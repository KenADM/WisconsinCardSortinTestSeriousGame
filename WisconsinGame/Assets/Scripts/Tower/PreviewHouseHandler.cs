using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Tracks the crane's position in real-time to find the closest gameplay platform.
/// Commands the closest tower to display a "preview" (ghost) house, providing visual feedback for where the house will drop.
/// </summary>
public class PreviewHouseHandler : MonoBehaviour
{
    [Header("For Actual Game")]
    [SerializeField] private List<Transform> platforms = new List<Transform>();
    
    // The crane controlled by the player
    [SerializeField] private GameObject player; 
    
    [SerializeField] private GameObject spawningPlatform;

    private bool isActive;
    private Transform currentClosestPlatform = null;

    private void OnEnable()
    {
        Tutorial.StartGame += StartPreviewHandler;
    }

    private void OnDisable()
    {
        Tutorial.StartGame -= StartPreviewHandler;
    }

    private void Start()
    {
        isActive = false;
    }

    private void Update()
    {
        // Only run distance calculations if the main game is active
        if (isActive)
        {
            Transform closestPlatform = null;
            float minDistance = float.MaxValue;

            // Find the closest gameplay tower
            foreach (Transform tower in platforms)
            {
                float distance = Vector3.Distance(player.transform.position, tower.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPlatform = tower;
                }
            }

            // Check if the spawning platform is actually the closest point
            float spawnDistance = Vector3.Distance(player.transform.position, spawningPlatform.transform.position);
            if (spawnDistance < minDistance)
            {
                closestPlatform = spawningPlatform.transform;
            }

            // If the closest platform has changed this frame, update the visual previews
            if (closestPlatform != currentClosestPlatform)
            {
                // Hide the preview on the previous platform
                if (currentClosestPlatform != null && currentClosestPlatform != spawningPlatform.transform)
                {
                    if (currentClosestPlatform.TryGetComponent(out InverseTowerHandler oldTower))
                    {
                        oldTower.HidePreviewHouse();
                    }
                }

                currentClosestPlatform = closestPlatform;

                // Show the preview on the new closest platform
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

    /// <summary>
    /// Activates the preview tracking system once the main game begins.
    /// </summary>
    private void StartPreviewHandler()
    {   
        isActive = true;
    }
}