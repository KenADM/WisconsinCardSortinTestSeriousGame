using UnityEngine;

public class WallHiderDecider : MonoBehaviour
{
    // Script to hide the wall to allow vision of the interior (animal crossing style)

    [SerializeField] private Transform north;
    [SerializeField] private Transform south;
    [SerializeField] private Transform west;
    [SerializeField] private Transform east;
    [SerializeField] private Transform floor;
    

    private GameObject player;
    
    public bool wallHidingActive = true;
    
    void Awake()
    {
        ActivateAllWalls();
    }
    public void AssignPlayer(GameObject currentPlayer)
    {
        this.player = currentPlayer;
        wallHidingActive = false;
    }

    public void DeactivateHiding(){
        wallHidingActive = false;
        ActivateAllWalls();
    }

    public void ActivateAllWalls(){
        north.gameObject.SetActive(true);
        south.gameObject.SetActive(true);
        west.gameObject.SetActive(true);
        east.gameObject.SetActive(true);
        
    }

    void HideCloserThing(Transform wallA, Transform wallB){
        float dist1 = Vector3.Distance(player.transform.position, wallA.position);
        float dist2 = Vector3.Distance(player.transform.position, wallB.position);
        
        if(dist1 < dist2)
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

    void DontHideIfInsideIsScene(Transform wall){
        Transform inner = wall.Find("Wall");
        Transform outer = wall.Find("InnerWall");   

        float distOut = Vector3.Distance(player.transform.position, inner.position);
        float distIn = Vector3.Distance(player.transform.position, outer.position);

        if(distOut < distIn)
        {
            wall.gameObject.SetActive(false);
        }
    }
    
    void Update()
    {
        if(wallHidingActive)
        {
            return;
        }

        HideCloserThing(north,south);
        HideCloserThing(west,east);

    }
}
