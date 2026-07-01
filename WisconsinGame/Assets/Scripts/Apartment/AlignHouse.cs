using UnityEngine;
using System;


public class AlignHouse : MonoBehaviour
{
    public int blockCount;

    private Transform platformTransform;
    private BoxCollider platformCollider;

    public int colorIndex;
    public int typeIndex;
    public int people;


    void Start()
    {
        blockCount = 0; // Inizializziamo il contatore dei blocchi allineati a 0
        platformTransform = transform.Find("Trigger").GetComponent<Transform>(); // Otteniamo il Transform del trigger

        platformCollider = transform.Find("Trigger").GetComponent<BoxCollider>(); // Otteniamo il BoxCollider del trigger
    }

    public void OnTriggerAlert(Collider house) //aggiungere l'aquisizione di colorIndex, typeIndex e people dal prefab del blocco
    {
        Debug.Log("Triggered by: " + house.name);
        // Il blocco è grabbable E NON è attualmente in mano
        if(house.CompareTag("Grabbable") && house.transform.parent == null)
        {
            Rigidbody houseRigidbody = house.GetComponent<Rigidbody>();

            house.transform.rotation = transform.rotation;
            house.tag = "Aligned";

            houseRigidbody.isKinematic = true; // Disabilita la fisica per il blocco allineato
            houseRigidbody.useGravity = false; // Disabilita la gravità per il blocco allineato
            if(blockCount == 0)
            {
                
                platformTransform.position = new Vector3(platformTransform.position.x, platformTransform.position.y + platformTransform.localScale.y , platformTransform.position.z);
                Debug.Log("Platform Center: " + platformTransform.localScale.y);
                
            }
            blockCount++;


        }
    }
}
