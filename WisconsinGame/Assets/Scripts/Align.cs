using UnityEngine;

public class Align : MonoBehaviour
{
    void OnTriggerStay(Collider other)
    { 
        // 1. Il blocco è grabbable E NON è attualmente in mano
        if(other.CompareTag("Grabbable") && other.transform.parent == null)
        {
            // 2. Allineiamo la rotazione della base
            other.transform.rotation = transform.rotation;

            // 3. Calcoliamo la posizione precisa sopra la base
            float targetY = transform.position.y + (transform.localScale.y / 2f) + (other.transform.localScale.y / 2f) + 0.01f;
            Vector3 targetPosition = new Vector3(transform.position.x, targetY, transform.position.z);
            
            // 4. Muoviamo il blocco (Lerp per la fluidità)
            other.transform.position = Vector3.Lerp(other.transform.position, targetPosition, Time.deltaTime * 10f);

            other.gameObject.tag = "Aligned"; // Cambia il tag per indicare che è allineato (opzionale, dipende da come vuoi gestire la logica del gioco)
        }
    }
}