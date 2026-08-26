using UnityEngine;

public class VRSeatBeltWithFade : MonoBehaviour
{
    [Header("VR Rig References")]
    [Tooltip("Trascina qui la Main Camera (la testa del giocatore)")]
    public Transform vrCamera;

    [Header("Directional Limits (Metri)")]
    public float maxOffsetRight = 0.6f;
    public float maxOffsetLeft = 0.6f;
    public float maxOffsetForward = 0.6f;
    public float maxOffsetBackward = 0.5f; 

    [Header("Red Fade Effect")]
    [Tooltip("Trascina qui il Canvas Group dello schermo rosso")]
    public CanvasGroup redFadeCanvas;
    
    public float fadeSpeed = 10f;

    void LateUpdate()
    {
        if (vrCamera == null || redFadeCanvas == null) return;

        // 1. Legge la posizione fisica della tua testa nella stanza
        Vector3 localPos = vrCamera.localPosition;

        float ratioX = 0f;
        float ratioZ = 0f;

        // 2. Calcola la percentuale per l'asse X (Destra / Sinistra)
        if (localPos.x > 0)
        {
            ratioX = localPos.x / maxOffsetRight;
        }
        else if (localPos.x < 0)
        {
            ratioX = Mathf.Abs(localPos.x) / maxOffsetLeft;
        }

        // 3. Calcola la percentuale per l'asse Z (Avanti / Indietro)
        if (localPos.z > 0)
        {
            ratioZ = localPos.z / maxOffsetForward;
        }
        else if (localPos.z < 0)
        {
            ratioZ = Mathf.Abs(localPos.z) / maxOffsetBackward;
        }
            
        // 4. Prende la direzione in cui ti sei sporto di più (bloccando il valore massimo a 1)
        float targetAlpha = Mathf.Clamp01(Mathf.Max(ratioX, ratioZ));

        // 5. Applica l'opacità: più ti allontani dal centro verso uno dei 4 limiti, più diventa rosso
        redFadeCanvas.alpha = Mathf.MoveTowards(redFadeCanvas.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
    }
}