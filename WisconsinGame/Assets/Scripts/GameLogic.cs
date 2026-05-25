using UnityEngine;
using System.Collections.Generic;
namespace SpecialObjects 
{
    public class GameLogic : MonoBehaviour
    {

        [Header("Condition Change Threshold")]
        public int matchMax;
        public List<int> condition;
        /* 
        1) n of people
        2) type of room
        3) color of room
        */
        private int matchedTime;
        

        // Called by a base, checks if the base and a given room match the current condition;
        public void CheckAlignment()
        {
            
        }

        void Start()
        {
            //avvio scelta condizioni

            //spawno le prime 4 case

            //spawno la prima casa
        }


    }
}