using UnityEngine;
using UnityEngine.InputSystem;

public class Jetpack : MonoBehaviour
{
    
    public InputAction goUp;

    public BlockGravity GravityManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        goUp.Enable();       
    }

    // Update is called once per frame
    void Update()
    {
        float upValue = goUp.ReadValue<float>();

        if (upValue > 0.5f)
        {
            //Debug.Log("SU");
            GoUp();
            
        }
        else if (upValue < 0.5f )
        {
            //Debug.Log("GIU");
            Fall();
            
        }
    }

    private void GoUp(){
        GravityManager.StopGravity();
        Vector3 currentPosition = transform.position;
        currentPosition.y += 0.01f; 
        transform.position = currentPosition; 
    }

    private void Fall(){
        GravityManager.StartGravity();
    }
}
