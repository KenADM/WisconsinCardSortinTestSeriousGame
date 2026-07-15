using UnityEngine;
using System.Collections.Generic;

public class DisableInternalView : MonoBehaviour
{
    // disable internal view of old apartments

    public void DisableView()
    {
        foreach(Transform house in this.transform)
        {
            if(house.CompareTag("Aligned")){
                
                WallHiderDecider whd = house.GetComponent<WallHiderDecider>();
                
                if(whd != null)
                {
                    whd.DeactivateHiding();
                }
            }
        }
    }
}
