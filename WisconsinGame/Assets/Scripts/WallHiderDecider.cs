using UnityEngine;

public class WallHiderDecider : MonoBehaviour
{
    public GameObject north;
    public GameObject south;
    public GameObject west;
    public GameObject east;
    public GameObject ceiling;
    public GameObject floor;
/*
    public WallHider northS;
    public WallHider southS;
    public WallHider westS;
    public WallHider eastS;
    public WallHider ceilingS;
    public WallHider floorS;
*/

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

        float distCeilingPlayer = Vector3.Distance(playerCamera.transform.position, ceiling.transform.position);
        float distFloorPlayer = Vector3.Distance(playerCamera.transform.position, floor.transform.position);
        

        if(distNorthPlayer < distSouthPlayer)
        {
            north.SetActive(false);
            south.SetActive(true);
        }
        else
        {
            north.SetActive(true);
            south.SetActive(false);

        }
        
        if(distWestPlayer < distEastPlayer)
        {
            west.SetActive(false);
            east.SetActive(true);
        }
        else
        {

            west.SetActive(true);
            east.SetActive(false);
        }

        if(distCeilingPlayer < distFloorPlayer)
        {
            ceiling.SetActive(true);
        }
        else
        {

            ceiling.SetActive(false);
        }



    }
}
