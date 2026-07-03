using UnityEngine;

public class WallHiderDecider : MonoBehaviour
{
    private Transform north;
    private Transform south;
    private Transform west;
    private Transform east;
    //private Transform ceiling;
    private Transform floor;
    

    private Camera playerCamera;
    
    private bool wallHidingActive = true;
    
    void Start()
    {
        // Grabs the camera tagged as "MainCamera"
        playerCamera = Camera.main;
        north = transform.Find("North");
        south = transform.Find("South");
        west = transform.Find("West");
        east = transform.Find("East");
        //ceiling = transform.Find("Ceiling");
        floor = transform.Find("Floor");

    }

    public void DeactivateHiding(){
        wallHidingActive = false;
        ActivateAllWalls();
    }

    public void ActivateAllWalls(){
        foreach(Transform item in this.transform){
            if(item.CompareTag("Wall"))
            {
                item.gameObject.SetActive(true);
            }
        }
        
    }

    void HideCloserThing(Transform wallA, Transform wallB){
        float dist1 = Vector3.Distance(playerCamera.transform.position, wallA.position);
        float dist2 = Vector3.Distance(playerCamera.transform.position, wallB.position);
        
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

        float distOut = Vector3.Distance(playerCamera.transform.position, inner.position);
        float distIn = Vector3.Distance(playerCamera.transform.position, outer.position);

        if(distOut < distIn)
        {
            wall.gameObject.SetActive(false);
        }
    }
    
    void Update()
    {
        if(!wallHidingActive)
        {
            return;
        }

        HideCloserThing(north,south);
        HideCloserThing(west,east);

        /*
        float dist1 = Vector3.Distance(playerCamera.transform.position, floor.position);
        float dist2 = Vector3.Distance(playerCamera.transform.position, ceiling.position);
        if(dist1 < dist2)
        {
            ceiling.gameObject.SetActive(true);
        }
        else
        {
            ceiling.gameObject.SetActive(false);

        }*/
    }
}
