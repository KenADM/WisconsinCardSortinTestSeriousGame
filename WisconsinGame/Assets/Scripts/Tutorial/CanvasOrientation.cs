using UnityEngine;

public class CanvasOrientation : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("Trascina qui la tua Main Camera (VR Head)")]
    public Transform vrCamera;

    [Header("Settings")]
    [Tooltip("Mantieni il Canvas dritto (non si inclina se guardi in alto/basso)")]
    public bool lockRotationUpright = true;

    void LateUpdate()
    {
        if (vrCamera == null) return;

        // Calcoliamo la direzione dalla telecamera verso il Canvas.
        // NOTA: Nei Canvas UI, calcoliamo la direzione "al contrario" 
        // per evitare che il testo risulti specchiato.
        Vector3 lookDirection = transform.position - vrCamera.position;
        
        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

            // Blocca gli assi X e Z per evitare che il Canvas si "sdrai" o si inclini lateralmente
            if (lockRotationUpright)
            {
                targetRotation.x = 0;
                targetRotation.z = 0;
            }

            // Applica la rotazione istantaneamente
            transform.rotation = targetRotation;
        }
    }
}