using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class GlobalMapGenerator
{
    private readonly MapGenTile[] m_MapGenTiles;
    private readonly int m_SizeX;
    private readonly int m_SizeY;

    private readonly HashSet<Vector2Int> m_UnusedPositions;
    public readonly HashSet<MapGenTile>[,] m_TileSetArray;

    private Tilemap m_Tilemap;

    public GlobalMapGenerator(int sizeX, int sizeY, MapGenTile[] mapGenTiles)
    {
        m_MapGenTiles = mapGenTiles;

        m_SizeX = sizeX;
        m_SizeY = sizeY;

        m_UnusedPositions = new HashSet<Vector2Int>(sizeX * sizeY);
        m_TileSetArray = new HashSet<MapGenTile>[sizeX, sizeY];
        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                m_TileSetArray[x, y] = new HashSet<MapGenTile>(m_MapGenTiles);
                m_UnusedPositions.Add(new Vector2Int(x, y));
            }
        }
    }


    public void Clear()
    {
        for (int y = 0; y < m_SizeY; y++)
        {
            for (int x = 0; x < m_SizeX; x++)
            {
                m_TileSetArray[x, y].UnionWith(m_MapGenTiles);
                m_UnusedPositions.Add(new Vector2Int(x, y));
            }
        }
    }

    public void Generate(Tilemap tilemap)
    {
        Stopwatch sw = new();
        sw.Start();

        m_Tilemap = tilemap;
        int restartCounter = 0;

        // refreshing tile possibilities
        while (m_UnusedPositions.Count > 0)
        {
            int minPosib = int.MaxValue;
            HashSet<Vector2Int> minPositions = new();
            foreach (Vector2Int item in m_UnusedPositions)
            {
                int c = m_TileSetArray[item.x, item.y].Count;
                if (c < minPosib)
                {
                    minPositions.Clear();
                    minPosib = c;
                }

                if (c == minPosib)
                {
                    minPositions.Add(item);
                }
            }

            Vector2Int randPos = minPositions.ElementAt(Random.Range(0, minPositions.Count));
            m_UnusedPositions.Remove(randPos);
            int randX = randPos.x;
            int randY = randPos.y;
            HashSet<MapGenTile> tile = m_TileSetArray[randX, randY];
            if (tile.Count > 0)
            {
                // MapGenTile b = tile.ElementAt(Random.Range(0, tile.Count));
                MapGenTile b = null;
                int weightSum = 0;
                foreach (MapGenTile t in tile)
                {
                    weightSum += t.weight;
                }

                int rand = Random.Range(0, weightSum);
                int currentSum = 0;
                foreach (MapGenTile t in tile)
                {
                    currentSum += t.weight;
                    if (currentSum > rand)
                    {
                        b = t;
                        break;
                    }
                }

                if (b != null)
                {
                    tile.Clear();
                    tile.Add(b);
                    m_Tilemap.SetTile(new Vector3Int(randX, randY), b.tile);
                    RefreshTiles(randX, randY);
                }
                else
                {
                    //Debug.Log("Amongus");
                }
            }
            else if (restartCounter < 100)
            {
                //Debug.Log("tragedia");
                restartCounter++;
                //m_WfcTilemap.ClearAllTiles();
                Clear();
            }
        }

        sw.Stop();
        Debug.Log($"Map generated in {sw.ElapsedMilliseconds}");
    }

    private void RefreshTiles(int x, int y)
    {
        Queue<Vector2Int> queue = new();

        Vector2Int starting = new(x, y);
        queue.Enqueue(starting);

        bool firstPass = true;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            if (RefreshTile(current.x, current.y) || firstPass)
            {
                firstPass = false;
                Vector2Int next = current + Vector2Int.up;
                if (next.y < m_TileSetArray.GetLength(1))
                {
                    queue.Enqueue(next);
                }

                next = current + Vector2Int.right;
                if (next.x < m_TileSetArray.GetLength(0))
                {
                    queue.Enqueue(next);
                }

                next = current + Vector2Int.down;
                if (next.y >= 0)
                {
                    queue.Enqueue(next);
                }

                next = current + Vector2Int.left;
                if (next.x >= 0)
                {
                    queue.Enqueue(next);
                }
            }
        }
    }

    /// <summary>
    /// Refreshes state of a tile
    /// </summary>
    /// <param name="x">x coordinate</param>
    /// <param name="y">y coordinate</param>
    /// <returns>true if tile state changed, false otherwise</returns>
    private bool RefreshTile(int x, int y)
    {
        HashSet<MapGenTile> currentTile = m_TileSetArray[x, y];

        if (currentTile.Count == 1) return false;

        int entrySize = currentTile.Count;

        if (y + 1 < m_TileSetArray.GetLength(1))
        {
            HashSet<MapGenTile> fromTop = new();
            foreach (MapGenTile item in m_TileSetArray[x, y + 1])
            {
                fromTop.UnionWith(item.bottomTiles);
            }

            currentTile.IntersectWith(fromTop);
        }

        if (y - 1 >= 0)
        {
            HashSet<MapGenTile> fromBottom = new();
            foreach (MapGenTile item in m_TileSetArray[x, y - 1])
            {
                fromBottom.UnionWith(item.topTiles);
            }

            currentTile.IntersectWith(fromBottom);
        }

        if (x - 1 >= 0)
        {
            HashSet<MapGenTile> fromLeft = new();
            foreach (MapGenTile item in m_TileSetArray[x - 1, y])
            {
                fromLeft.UnionWith(item.rightTiles);
            }

            currentTile.IntersectWith(fromLeft);
        }

        if (x + 1 < m_TileSetArray.GetLength(0))
        {
            HashSet<MapGenTile> fromRight = new();
            foreach (MapGenTile item in m_TileSetArray[x + 1, y])
            {
                fromRight.UnionWith(item.leftTiles);
            }

            currentTile.IntersectWith(fromRight);
        }

        if (currentTile.Count == 1)
        {
            m_Tilemap.SetTile(new Vector3Int(x, y), m_TileSetArray[x, y].First().tile);
            m_UnusedPositions.Remove(new Vector2Int(x, y));
        }

        return entrySize != currentTile.Count;
    }
}