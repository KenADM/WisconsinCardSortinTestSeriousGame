using UnityEngine;

public class WallHider : MonoBehaviour
{
    public Camera playercamera;
    MeshRenderer myRenderer;
    Plane[] cameraFrustum;
    Bounds bounds;

    public bool hideAllowed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myRenderer = GetComponent<MeshRenderer>();
        bounds = GetComponent<Collider>().bounds;
        hideAllowed = true;
    }

    // Update is called once per frame
    void Update()
    {
        cameraFrustum = GeometryUtility.CalculateFrustumPlanes(playercamera);
        if(GeometryUtility.TestPlanesAABB(cameraFrustum, bounds))
        {
            myRenderer.enabled = false;
        }
        else
        {
            myRenderer.enabled = true;

        }
    }
}
