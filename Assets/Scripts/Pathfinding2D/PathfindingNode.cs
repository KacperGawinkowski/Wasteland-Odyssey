using UnityEngine;

namespace Pathfinding2D
{
    public class PathfindingNode
    {
        public float baseCost;

        public float d;
        public float h;
        public float f;

        public Vector2Int position;

        public PathfindingNode previousNode;
        public (PathfindingNode, float)[] pathfindingNodes;
    }
}