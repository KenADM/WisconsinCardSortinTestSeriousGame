using UnityEngine;
using System;

namespace SpecialObjects
{
    public class Align : MonoBehaviour
    {
        private int blockCount; // Contatore dei blocchi allineati
        private BoxCollider triggerCollider; // Riferimento al BoxCollider del trigger
        private Transform TriggerTransform; // Riferimento al Transform del trigger
        private float colliderX;
        private float colliderZ;
        private float top;

        // base to compare
        public int colorIndex;
        public int typeIndex;
        public int people;

        // events
        public static event Action<Align> startingConfiguration;
        public static event Action spawnNew;
        public static event Action<int, int, int, Collider> checkCondition;
        
        void Start()
        {
            blockCount = 0; // Inizializziamo il contatore dei blocchi allineati a 0
            top = 0;
            triggerCollider = GetComponent<BoxCollider>(); // Otteniamo il BoxCollider del trigger
            TriggerTransform = GetComponent<Transform>(); // Otteniamo il Transform del trigger
            //SpawningManager.SpawnOne(colorIndex, typeIndex, people, this.transform.position.x, this.transform.localScale.y+10, this.transform.localScale.z);
            
        }
        
        void OnEnable()
        {
            GameLogic.startStartingCubeSpawning += StartStartingCubeSpawningHandler;
        }

        void OnDisable()
        {
            GameLogic.startStartingCubeSpawning -= StartStartingCubeSpawningHandler;
        }

        void StartStartingCubeSpawningHandler()
        {
            startingConfiguration?.Invoke(this);
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
                Debug.Log("Block Height: " + blockHeight);

                //spostiamo il blocco in alto, considerando quanti blocchi sono già allineati
                if(top == 0)
                {
                    BoxCollider baseCollider = GetComponent<BoxCollider>();
                    Transform baseTransform = GetComponent<Transform>();
                    top = baseCollider.size.y * baseTransform.localScale.y; 

                    top = top + blockHeight/2; 
                }else{
                    top = top + blockHeight; 
                }
                Vector3 targetPosition = new Vector3(transform.position.x, top, transform.position.z);
                


                // Muoviamo il blocco 
                other.transform.position = targetPosition;

                // blocchiamo la caduta del nuovo blocco
                other.GetComponent<BlockGravity>().StopGravity();
                
                // Cambia il tag per indicare che è allineato, per evitare che non possa piu essere mosso
                other.gameObject.tag = "Aligned"; 


                //Debug.Log("Collider Center: " + triggerCollider.center);

                // sposto collider in alto, considerando i blocchi già allineati
                float heightToAdd = blockHeight / triggerCollider.transform.localScale.y; 
                triggerCollider.center = new Vector3(triggerCollider.center.x, triggerCollider.center.y + heightToAdd, triggerCollider.center.z);
                Debug.Log("Collider Center: " + triggerCollider.center);
                
                
                checkCondition?.Invoke(colorIndex, typeIndex, people, other);
                spawnNew?.Invoke();
                
            }
        }
    }
}