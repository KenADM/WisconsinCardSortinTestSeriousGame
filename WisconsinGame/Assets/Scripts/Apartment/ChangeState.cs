using UnityEngine;
using UnityEngine.InputSystem;

public class ChangeState : MonoBehaviour
{
    public void Grab( )
    {
        // Cambia lo stato del blocco in "grabbed"
        this.gameObject.tag = "Grabbed";

    }

    public void Release( ){
        this.gameObject.tag = "Grabbable";

    }
}