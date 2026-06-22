/*using UnityEngine;

public class WallHiderDecider : MonoBehaviour
{
    public GameObject north;
    public GameObject south;
    public GameObject west;
    public GameObject east;
    public Camera playerCamera;
    
    
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float distNorthPlayer = Vector3.Distance(playerCamera.transform.position, north.transform.position);
        float distSouthPlayer = Vector3.Distance(playerCamera.transform.position, south.transform.position);
        float distWestPlayer = Vector3.Distance(playerCamera.transform.position, west.transform.position);
        float distEastPlayer = Vector3.Distance(playerCamera.transform.position, east.transform.position);

        if(distNorthPlayer < distSouthPlayer)
        {
            north.WallHider.hideAllowed = true;
            south.WallHider.hideAllowed = false;
        }
        else
        {
            north.WallHider.hideAllowed = false;
            south.WallHider.hideAllowed = true;
        }
        
        if(distWestPlayer < distEastPlayer)
        {
            west.WallHider.hideAllowed = true;
            east.WallHider.hideAllowed = false;
        }
        else
        {
            west.WallHider.hideAllowed = false;
            east.WallHider.hideAllowed = true;
        }



    }
}*/