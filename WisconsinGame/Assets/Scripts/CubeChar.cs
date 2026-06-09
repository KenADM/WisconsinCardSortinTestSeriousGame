using UnityEngine;
using System;

namespace SpecialObjects
{
    public class CubeChar : MonoBehaviour
    {
        private CombinationBox CubeCombination;

        public void SetCombinationBox(int people, int type, int color)
        {
            CubeCombination = new CombinationBox(people, type, color);
        }

        public int GetPeople()
        {
            return CubeCombination.people;
        }

        public int GetTypeCube()
        {
            return CubeCombination.type;
        }

        public int GetColor()
        {
            return CubeCombination.color;
        }

    }
}