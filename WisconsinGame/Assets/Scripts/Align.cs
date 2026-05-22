using UnityEngine;

public class Align : MonoBehaviour
{
    
    
    void OnTriggerStay(Collider other)
    { 
        // Il blocco è grabbable E NON è attualmente in mano
        if(other.CompareTag("Grabbable") && other.transform.parent == null)
        {
            //Debug.Log(transform.position);
            other.transform.rotation = transform.rotation;

            // Calcoliamo la posizione precisa sopra la base
            float targetY = transform.position.y + transform.localScale.y / 2 + other.transform.localScale.y / 2;
            Vector3 targetPosition = new Vector3(transform.position.x, targetY, transform.position.z);
            
            // Muoviamo il blocco 
            other.transform.position = targetPosition;

            //blocchiamo la caduta del nuovo blocco
            other.GetComponent<BlockGravity>().StopGravity();

            //Debug.Log(other.transform.position);

            other.gameObject.tag = "Aligned"; // Cambia il tag per indicare che è allineato, per evitare che non possa piu essere mosso

            //cambio collider, che viene spostato sopra il blocco, per permettere di allineare altri blocchi sopra
            BoxCollider triggerCollider = GetComponent<BoxCollider>();

            Debug.Log(GetComponent<BoxCollider>().center.y);
            //Debug.Log(triggerCollider.center.y);
            //Debug.Log(other.transform.localScale.y);

            GetComponent<BoxCollider>().center = new Vector3(triggerCollider.center.x, triggerCollider.center.y + 1, triggerCollider.center.z); // + 1 poichè il blocco è alto 1 unità, quindi spostiamo il collider di 1 unità verso l'alto per allineare il prossimo blocco sopra
            //Debug.Log(GetComponent<BoxCollider>().center.y);


            GetComponent<Spawning>().Spawn(); // Spawna un nuovo blocco sopra
            
        }
    }
}
//MODIFICO ALTEZZA DA RAGGIUNGERE