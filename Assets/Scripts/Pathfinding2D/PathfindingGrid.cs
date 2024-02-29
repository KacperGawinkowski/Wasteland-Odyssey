using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Pathfinding2D
{
    public class PathfindingGrid : MonoBehaviour
    {
        //[SerializeField] private Tilemap m_Tilemap;

        //[SerializeField] private int m_MinX;
        //[SerializeField] private int m_MinY;
        //[SerializeField] private int m_MaxX;
        //[SerializeField] private int m_MaxY;

        public PathfindingNode[,] grid;

        private const float k_NormalCost = 1f;
        private const float k_DiagonalCost = 1.414f;

        private readonly List<Vector2Int> m_TempPath = new();
        private readonly HashSet<PathfindingNode> m_VisitedNodes = new();
        private readonly HashSet<PathfindingNode> m_PendingNodes = new();


        //void Awake()
        //{
        //    CreateGrid();
        //}

        public void FindPath(Vector2Int start, Vector2Int end, Queue<Vector2Int> path)
        {
            //Vector3Int start = m_Tilemap.WorldToCell(startPoint);
            //Vector3Int end = m_Tilemap.WorldToCell(endPoint);

            m_VisitedNodes.Clear();
            m_PendingNodes.Clear();

            PathfindingNode currentNode = grid[start.x, start.y];
            PathfindingNode endNode = grid[end.x, end.y];
            if (endNode == null || currentNode == null) return;

            m_PendingNodes.Add(currentNode);
            currentNode.previousNode = null;
            currentNode.d = 0;

            int debug = 0;
            while (m_PendingNodes.Count > 0 && debug < 10000)
            {
                debug++;

                currentNode = TakeBestPendingNode();
                m_PendingNodes.Remove(currentNode);
                m_VisitedNodes.Add(currentNode);

                if (currentNode == endNode)
                {
                    //print("path found: " + debug);
                    PathfindingNode n = currentNode;
                    m_TempPath.Clear();
                    int debug2 = 0;
                    while (n != null && debug2 < 10000)
                    {
                        debug2++;

                        m_TempPath.Add(n.position);
                        //print(n.position);
                        n = n.previousNode;
                    }
                    for (int i = m_TempPath.Count - 2; i >= 0; i--)
                    {
                        path.Enqueue(m_TempPath[i]);
                    }
                    return;
                }

                for (int i = 0; i < currentNode.pathfindingNodes.Length; i++)
                {
                    (PathfindingNode, float) item = currentNode.pathfindingNodes[i];
                    if (!m_VisitedNodes.Contains(item.Item1))
                    {
                        if (m_PendingNodes.Contains(item.Item1))
                        {
                            float newD = currentNode.d + item.Item2;
                            if (item.Item1.d > newD)
                            {
                                item.Item1.d = newD;
                                item.Item1.f = item.Item1.d + item.Item1.h;
                                item.Item1.previousNode = currentNode;
                            }
                        }
                        else
                        {
                            item.Item1.d = currentNode.d + item.Item2;
                            item.Item1.h = Vector2Int.Distance(item.Item1.position, endNode.position);
                            item.Item1.f = item.Item1.d + item.Item1.h;
                            item.Item1.previousNode = currentNode;
                            m_PendingNodes.Add(item.Item1);
                        }
                    }
                }
            }
            print("no path");
        }

        private PathfindingNode TakeBestPendingNode()
        {
            PathfindingNode node = m_PendingNodes.First();
            foreach (var item in m_PendingNodes)
            {
                if (item.f < node.f)
                {
                    node = item;
                }
            }
            return node;
        }

        public void CreateGrid(int sizeX, int sizeY)
        {
            grid = new PathfindingNode[sizeX, sizeY];

            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    PathfindingNode p = new();
                    p.position = new Vector2Int(x, y);
                    grid[x, y] = p;
                }
            }
        }

        public void BuildGrid()
        {
            List<(PathfindingNode, float)> pathfindingNodes = new(8);

            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    if (grid[x, y] != null)
                    {
                        pathfindingNodes.Clear();

                        AddGridNeighbor(x, y, x + 1, y, k_NormalCost, pathfindingNodes);
                        AddGridNeighbor(x, y, x - 1, y, k_NormalCost, pathfindingNodes);
                        AddGridNeighbor(x, y, x, y + 1, k_NormalCost, pathfindingNodes);
                        AddGridNeighbor(x, y, x, y - 1, k_NormalCost, pathfindingNodes);

                        AddGridNeighbor(x, y, x + 1, y + 1, k_DiagonalCost, pathfindingNodes);
                        AddGridNeighbor(x, y, x + 1, y - 1, k_DiagonalCost, pathfindingNodes);
                        AddGridNeighbor(x, y, x - 1, y + 1, k_DiagonalCost, pathfindingNodes);
                        AddGridNeighbor(x, y, x - 1, y - 1, k_DiagonalCost, pathfindingNodes);

                        grid[x, y].pathfindingNodes = pathfindingNodes.ToArray();
                    }
                }
            }
        }

        private void AddGridNeighbor(int x, int y, int nx, int ny, float cost, List<(PathfindingNode, float)> pathfindingNodes)
        {
            try
            {
                if (grid[nx, ny] != null && grid[nx, y] != null && grid[x, ny] != null)
                {
                    pathfindingNodes.Add((grid[nx, ny], grid[nx, ny].baseCost * cost));
                }
            }
            catch (System.Exception) { }
        }

        //private void OnDrawGizmosSelected()
        //{
        //    float cellSizeX = m_Tilemap.cellSize.x;
        //    float cellSizeY = m_Tilemap.cellSize.y;

        //    float anchorX = m_Tilemap.tileAnchor.x;
        //    float anchorY = m_Tilemap.tileAnchor.y;

        //    for (int y = m_MinY; y < m_MaxY; y++)
        //    {
        //        for (int x = m_MinX; x < m_MaxX; x++)
        //        {
        //            Vector3 pos = m_Tilemap.CellToWorld(new Vector3Int(x, y, 0));
        //            Gizmos.DrawWireCube(new Vector3(pos.x + anchorX, pos.y + anchorY, 0), new Vector3(cellSizeX, cellSizeY, 0));
        //        }
        //    }
        //}
    }
}