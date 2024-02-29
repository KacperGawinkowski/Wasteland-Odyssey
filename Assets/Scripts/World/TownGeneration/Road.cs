using Unity.Mathematics;
using UnityEngine;

namespace World.TownGeneration
{
    public struct Road
    {
        public int2 point1;
        public int2 point2;

        public Road(int2 point1, int2 point2)
        {
            if (point1.x < point2.x || point1.y < point2.y)
            {
                this.point1 = point1;
                this.point2 = point2;
            }
            else
            {
                this.point1 = point2;
                this.point2 = point1;
            }
        }
    }
}