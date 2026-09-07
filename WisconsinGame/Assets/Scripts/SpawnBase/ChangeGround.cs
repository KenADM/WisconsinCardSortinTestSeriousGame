using UnityEngine;

public class ChangeGround : MonoBehaviour
{
    /// <summary>
    /// changes the other gravity floor so that it sits on top of this object (based on trigger position)
    /// </summary>
    /// <param name="other">object which floor needs to vary</param>
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Gravity>(out Gravity houseGravity))
        {
            houseGravity.SetFloorHeight(this.transform.position.y);
        }
    }

    /// <summary>
    /// changes the other gravity floor so that it reverts to base height (0)
    /// </summary>
    /// <param name="other">object which floor needs to vary</param>
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<Gravity>(out Gravity houseGravity))
        {
            houseGravity.SetFloorHeight(0f);
        }
    }
}
