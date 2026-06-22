using UnityEngine;

public class WallHiderDecider : MonoBehaviour
{
    public GameObject north;
    public GameObject south;
    public GameObject west;
    public GameObject east;

    public WallHider northS;
    public WallHider southS;
    public WallHider westS;
    public WallHider eastS;
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
            northS.hideAllowed = true;
            southS.hideAllowed = false;
        }
        else
        {
            northS.hideAllowed = false;
            southS.hideAllowed = true;
        }
        
        if(distWestPlayer < distEastPlayer)
        {
            westS.hideAllowed = true;
            eastS.hideAllowed = false;
        }
        else
        {
            westS.hideAllowed = false;
            eastS.hideAllowed = true;
        }



    }
}
