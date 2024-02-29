using UnityEngine;

namespace World.TownGeneration
{
    [CreateAssetMenu(fileName = "HouseConfig", menuName = "WorldGeneration/House", order = 1)]
    public class 
        HouseConfig : ScriptableObject
    {
        public HouseController[] houseControllers;
        public GameObject[] interiorVariants;
        
        public bool Left;
        public bool Right;
        public bool Top;
        public bool TopLeft;
        public bool TopRight;
    }
}