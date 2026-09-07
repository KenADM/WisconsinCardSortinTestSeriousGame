using UnityEngine;

/// <summary>
/// Dynamically hides house walls that are closer to the player's camera viewpoint 
/// to maintain clear visibility of the interior (Animal Crossing-style camera culling).
/// </summary>
public class WallHiderDecider : MonoBehaviour
{
    [Header("Wall References")]
    [SerializeField] private Transform north;
    [SerializeField] private Transform south;
    [SerializeField] private Transform west;
    [SerializeField] private Transform east;    
    private GameObject player;
    
    public bool wallHidingActive = true;
    
    private void Awake()
    {
        ActivateAllWalls();
    }

    /// <summary>
    /// Assigns the tracking player object and enables wall hiding mechanics.
    /// </summary>
    public void AssignPlayer(GameObject currentPlayer)
    {
        this.player = currentPlayer;
        wallHidingActive = false;
    }

    /// <summary>
    /// Disables dynamic wall hiding and resets all walls to visible.
    /// </summary>
    public void DeactivateHiding()
    {
        wallHidingActive = false;
        ActivateAllWalls();
    }

    /// <summary>
    /// Forces all four house walls to be active and visible.
    /// </summary>
    public void ActivateAllWalls()
    {
        if (north != null) north.gameObject.SetActive(true);
        if (south != null) south.gameObject.SetActive(true);
        if (west != null) west.gameObject.SetActive(true);
        if (east != null) east.gameObject.SetActive(true);
    }

    /// <summary>
    /// Compares the distance of two opposing walls relative to the player, 
    /// keeping the further wall visible while checking the closer one for interior visibility.
    /// </summary>
    private void HideCloserThing(Transform wallA, Transform wallB)
    {
        if (player == null || wallA == null || wallB == null) return;

        float dist1 = Vector3.Distance(player.transform.position, wallA.position);
        float dist2 = Vector3.Distance(player.transform.position, wallB.position);
        
        if (dist1 < dist2)
        {
            wallB.gameObject.SetActive(true);
            DontHideIfInsideIsScene(wallA);
        }
        else
        {
            wallA.gameObject.SetActive(true);
            DontHideIfInsideIsScene(wallB);
        }
    }

    /// <summary>
    /// Checks the player's position relative to the interior and exterior meshes of a wall 
    /// to decide whether it should be culled from view.
    /// </summary>
    private void DontHideIfInsideIsScene(Transform wall)
    {
        if (player == null || wall == null) return;

        Transform inner = wall.Find("Wall");
        Transform outer = wall.Find("InnerWall");   

        if (inner == null || outer == null) return;

        float distOut = Vector3.Distance(player.transform.position, inner.position);
        float distIn = Vector3.Distance(player.transform.position, outer.position);

        if (distOut < distIn)
        {
            wall.gameObject.SetActive(false);
        }
    }
    
    private void Update()
    {
        if (wallHidingActive || player == null)
        {
            return;
        }

        HideCloserThing(north, south);
        HideCloserThing(west, east);
    }
}