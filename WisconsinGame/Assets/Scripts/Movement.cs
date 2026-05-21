using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [Header("Impostazioni")]
    public float speed = 1f;
    public Transform vrHead; // La telecamera

    [Header("Input Movimento")]
    // serve per indicare quale tasto o joystick vogliamo usare per muoverci.
    public InputAction moveAction; 

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



        if (joystick.magnitude > 0.1f)
        {
            // vrHead.forward è un vettore che punta nella direzione in cui la testa sta guardando
            Vector3 forward = vrHead.forward;
            forward.y = 0; // per evitare di volare o sprofondare quando guardo su o giù
            forward.Normalize(); // rende il vettore lungo 1, così non influenza la velocità in orizzontale

            Vector3 right = vrHead.right;
            right.y = 0;
            right.Normalize();

            // Combina i due vettori, in base alla direzione della testa e a quanto spingo il joystick
            Vector3 movement = (forward * joystick.y) + (right * joystick.x);

            // Muovi il corpo nel mondo
            transform.Translate(movement * speed * Time.deltaTime, Space.World);
            // Time.deltaTime serve per rendere il movimento fluido e indipendente dal framerate (così va alla stessa velocità su computer potenti o meno)
            
        }
    }
}