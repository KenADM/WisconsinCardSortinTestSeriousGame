using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Xml.Serialization;

public class CraneMovement : MonoBehaviour
{
    [Header("Settings")]
    public float Rotationspeed = 1f;
    public Transform vrHead; // La telecamera

    [Header("Movement Input")]
    // serve per indicare quale tasto o joystick vogliamo usare per muoverci.
    public InputAction moveAction; 

    [Header("Crane Down Input")]
    // serve per indicare quale tasto o joystick vogliamo usare per muoverci.
    public InputAction downAction; 
    
    [Header("Piattaforme")]
    public List<Transform> platforms = new List<Transform>();

    [Header("Position for Spawn")]
    public float x;
    public float y;
    public float z;  

    [Header("Offset")]

    public float OffsetTowerDistance;


    private bool isInputActive = true;
    void Start()
    {
        moveAction.Enable();
    }

    void Update()
    {
        // Il joystick legge un Vector2: 
        // il joystick invia segnali x e y (con valori da -1 a 1) in base a come si muove il joystick
        // joystick.x = quanto spingo a destra o sinistra
        // joystick.y = quanto spingo avanti o indietro

        Vector2 joystick = moveAction.ReadValue<Vector2>();
        if (Mathf.Abs(joystick.x) > 0.5f)
        {
            // Controlla se il movimento è già stato eseguito per questo "push"
            if (isInputActive)
            {
                MoveToClosest(joystick.x);
                isInputActive = false; // Blocca il movimento finché il joystick non torna al centro
            }
        }
        else
        {
            // Il joystick è tornato vicino al centro (zona morta).
            // Resettiamo la variabile così il giocatore può muoversi di nuovo.
            isInputActive = true;
        }

        Vector2 joystick2 = downAction.ReadValue<Vector2>();
        if (Mathf.Abs(joystick2.x) > 0.5f)
        {
            //DA IMPLEMENTARE
        }

    }
    
private void MoveToClosest(float direction)
{
    Vector3 spawn = new Vector3(x, y, z);
    Vector3 myPosition = this.transform.position;
    
    Vector3 closest = this.transform.position;

    foreach (Transform nextHop in platforms)
    {
        // player position 
        Vector3 targetPos = nextHop.position;
        targetPos.z += OffsetTowerDistance;

        // check if targeted onject is in the right direction
        if (IsCorrectDirection(myPosition.x, targetPos.x, direction))
        {
            if (Vector3.Distance(myPosition, targetPos) < Vector3.Distance(myPosition, closest) || Vector3.Distance(myPosition, closest) == 0f)
            {
                closest = targetPos;
            }
        }
    }
    
    Vector3 spawnTarget = spawn;
    spawnTarget.z += OffsetTowerDistance;

    if (IsCorrectDirection(myPosition.x, spawnTarget.x, direction))
    {
        if (Vector3.Distance(myPosition, spawnTarget) < Vector3.Distance(myPosition, closest) || Vector3.Distance(myPosition, closest) == 0f)
        {
            closest = spawnTarget;
        }
    }

    // closest already has the offset applied from the loops above, so we just assign it
    vrHead.position = closest;
    }

    private bool IsCorrectDirection(float myX, float targetX, float direction)
    {
        if (direction > 0) //right
        {
            return targetX > myX;
        }
        else if (direction < 0) // left
        {
            return targetX < myX;
        }
        
        return false; 
    }
}