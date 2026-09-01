using UnityEngine;

public class ChangeGround : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Gravity>(out Gravity houseGravity))
        {
            houseGravity.SetFloorHeight(this.transform.position.y);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<Gravity>(out Gravity houseGravity))
        {
            houseGravity.SetFloorHeight(0f);
        }
    }
}
