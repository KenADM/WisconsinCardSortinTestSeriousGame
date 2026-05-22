using UnityEngine;

public class Align : MonoBehaviour
{
    private int blockCount; // Contatore dei blocchi allineati
    private BoxCollider triggerCollider; // Riferimento al BoxCollider del trigger
    private float colliderX;
    private float colliderZ;
    
    [Header("Spawning")]
    public Spawning Manager;
    
    void Start()
    {
        blockCount = 0; // Inizializziamo il contatore dei blocchi allineati a 0
        triggerCollider = GetComponent<BoxCollider>(); // Otteniamo il BoxCollider del trigger
    }

    void OnTriggerStay(Collider other)
    { 
        // Il blocco è grabbable E NON è attualmente in mano
        if(other.CompareTag("Grabbable") && other.transform.parent == null)
        {
            BoxCollider cubeCollider = other.GetComponent<BoxCollider>();
            Transform cubeTransform = other.GetComponent<Transform>();

            // Debug.Log(transform.position);
            other.transform.rotation = transform.rotation;

            // Incrementa il contatore dei blocchi allineati
            blockCount++; 

            // Calcoliamo l'altezza del blocco considerando la scala
            float blockHeight =  cubeCollider.size.y * cubeTransform.localScale.y; 

            //spostiamo il blocco in alto, considerando quanti blocchi sono già allineati
            float targetY = transform.position.y + blockHeight * blockCount; 
            Vector3 targetPosition = new Vector3(transform.position.x, targetY, transform.position.z);
            
            // Muoviamo il blocco 
            other.transform.position = targetPosition;

            // blocchiamo la caduta del nuovo blocco
            other.GetComponent<BlockGravity>().StopGravity();
            
            // Cambia il tag per indicare che è allineato, per evitare che non possa piu essere mosso
            other.gameObject.tag = "Aligned"; 

            // cambio collider, che viene spostato sopra il blocco, per permettere di allineare altri blocchi sopra
            triggerCollider.center = new Vector3(triggerCollider.center.x, triggerCollider.center.y + cubeCollider.size.y, triggerCollider.center.z); // + 1 poichè il blocco è alto 1 unità, quindi spostiamo il collider di 1 unità verso l'alto per allineare il prossimo blocco sopra

            // Spawna un nuovo blocco
            Manager.Spawn(); 
            
        }
    }
}
