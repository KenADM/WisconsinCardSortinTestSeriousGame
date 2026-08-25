using UnityEngine;

public class WallHiderDecider : MonoBehaviour
{
    // Script to hide the wall to allow vision of the interior (animal crossing style)

    [SerializeField] private Transform north;
    [SerializeField] private Transform south;
    [SerializeField] private Transform west;
    [SerializeField] private Transform east;
    [SerializeField] private Transform floor;
    

    private Camera playerCamera;
    
    private bool wallHidingActive = true;
    
    void Start()
    {        
        playerCamera = Camera.main; // uses tag, necessary because object dont exist from the start

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

    }
}
