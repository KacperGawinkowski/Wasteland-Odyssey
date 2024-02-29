// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using UnityEditor;
// using UnityEngine;
// using UnityEngine.InputSystem;
// using UnityEngine.Tilemaps;
// using Random = UnityEngine.Random;
//
// public class GlobalMapGenerator : MonoBehaviour
// {
//     public MapGenTile[] mapGenTiles;
//
//     public Tilemap tilemap;
//
//     [SerializeField] private int m_SizeX;
//     [SerializeField] private int m_SizeY;
//
//     private HashSet<Vector2Int> m_UnusedPositions;
//     private HashSet<MapGenTile>[,] m_TileSetArray;
//
//     // Start is called before the first frame update
//     void Start()
//     {
//         // StartCoroutine(GenerateWFC());
//         GenerateWFC();
//     }
//
//     // Update is called once per frame
//     // void Update()
//     // {
//     // }
//
//
//     [ContextMenu("GenerateWFC")]
//     public void GenerateWFC()
//     {
//         tilemap.ClearAllTiles();
//
//
//         m_UnusedPositions = new HashSet<Vector2Int>(m_SizeX * m_SizeY);
//
//         m_TileSetArray = new HashSet<MapGenTile>[m_SizeX, m_SizeY];
//         for (int y = 0; y < m_SizeY; y++)
//         {
//             for (int x = 0; x < m_SizeX; x++)
//             {
//                 m_TileSetArray[x, y] = new HashSet<MapGenTile>(mapGenTiles);
//                 m_UnusedPositions.Add(new Vector2Int(x, y));
//             }
//         }
//
//         // selecting random tile
//         // int randX = Random.Range(0, m_SizeX);
//         // int randY = Random.Range(0, m_SizeY);
//         // HashSet<MapGenTile> tile = tileSetArray[randX, randY];
//         // MapGenTile b = tile.ElementAt(Random.Range(0, tile.Count));
//         // tile.Clear();
//         // tile.Add(b);
//
//         int randX;
//         int randY;
//
//         // refreshing tile possibilities
//         do
//         {
//             // await Task.Delay(10);
//             // while (!Keyboard.current.spaceKey.wasPressedThisFrame)
//             // {
//             //     await Task.Delay(1);
//             // }
//
//             Vector2Int randPos = m_UnusedPositions.ElementAt(Random.Range(0, m_UnusedPositions.Count));
//             m_UnusedPositions.Remove(randPos);
//             // randX = Random.Range(0, m_SizeX);
//             // randY = Random.Range(0, m_SizeY);
//             randX = randPos.x;
//             randY = randPos.y;
//             HashSet<MapGenTile> tile = m_TileSetArray[randX, randY];
//             if (tile.Count > 0)
//             {
//                 MapGenTile b = tile.ElementAt(Random.Range(0, tile.Count));
//                 tile.Clear();
//                 tile.Add(b);
//                 // await Task.Delay(1000);
//                 tilemap.SetTile(new Vector3Int(randX, randY), tile.First().tile);
//             }
//             else
//             {
//                 print($"{randPos} {tile}");
//                 throw new Exception("wtf wtf");
//             }
//         } while (RefreshTiles(randX, randY) != 0 && m_UnusedPositions.Count > 0);
//         // RefreshTiles(tileSetArray, randX, randY);
//         // RefreshTiles(tileSetArray, randX, randY);
//         // RefreshTiles(tileSetArray, randX, randY);
//
//
//         // drawing
//         // tilemap.ClearAllTiles();
//         // for (int y = 0; y < m_SizeY; y++)
//         // {
//         //     for (int x = 0; x < m_SizeX; x++)
//         //     {
//         //         print($"x: {x}  y:{y}        {tileSetArray[x, y].Count}");
//         //         if (tileSetArray[x, y].Count == 1)
//         //         {
//         //             tilemap.SetTile(new Vector3Int(x, y), tileSetArray[x, y].First().tile);
//         //         }
//         //     }
//         // }
//     }
//
//     private int RefreshTiles(int x, int y)
//     {
//         int tilesLeft = 0;
//
//         HashSet<Vector2Int> visitedTiles = new();
//         Queue<Vector2Int> queue = new();
//
//         Vector2Int starting = new(x, y);
//         visitedTiles.Add(starting);
//         queue.Enqueue(starting);
//
//         while (queue.Count > 0)
//         {
//             Vector2Int current = queue.Dequeue();
//
//             Vector2Int next = current + Vector2Int.up;
//             if (next.y < m_TileSetArray.GetLength(1) && !visitedTiles.Contains(next))
//             {
//                 queue.Enqueue(next);
//                 visitedTiles.Add(next);
//             }
//             
//             next = current + Vector2Int.right;
//             if (next.x < m_TileSetArray.GetLength(0) && !visitedTiles.Contains(next))
//             {
//                 queue.Enqueue(next);
//                 visitedTiles.Add(next);
//             }
//
//             next = current + Vector2Int.down;
//             if (next.y >= 0 && !visitedTiles.Contains(next))
//             {
//                 queue.Enqueue(next);
//                 visitedTiles.Add(next);
//             }
//
//             next = current + Vector2Int.left;
//             if (next.x >= 0 && !visitedTiles.Contains(next))
//             {
//                 queue.Enqueue(next);
//                 visitedTiles.Add(next);
//             }
//
//             HashSet<MapGenTile> tile = m_TileSetArray[current.x, current.y];
//             if (tile.Count > 1)
//             {
//                 RefreshTile(current.x, current.y);
//                 tilesLeft++;
//             }
//         }
//
//         return tilesLeft;
//     }
//
//     private void RefreshTile(int x, int y)
//     {
//         HashSet<MapGenTile> currentTile = m_TileSetArray[x, y];
//
//         if (y + 1 < m_TileSetArray.GetLength(1))
//         {
//             HashSet<MapGenTile> fromTop = new();
//             foreach (MapGenTile item in m_TileSetArray[x, y + 1])
//             {
//                 fromTop.UnionWith(item.bottomTiles);
//             }
//
//             currentTile.IntersectWith(fromTop);
//         }
//
//         if (y - 1 >= 0)
//         {
//             HashSet<MapGenTile> fromBottom = new();
//             foreach (MapGenTile item in m_TileSetArray[x, y - 1])
//             {
//                 fromBottom.UnionWith(item.topTiles);
//             }
//
//             currentTile.IntersectWith(fromBottom);
//         }
//
//         if (x - 1 >= 0)
//         {
//             HashSet<MapGenTile> fromLeft = new();
//             foreach (MapGenTile item in m_TileSetArray[x - 1, y])
//             {
//                 fromLeft.UnionWith(item.rightTiles);
//             }
//
//             currentTile.IntersectWith(fromLeft);
//         }
//
//         if (x + 1 < m_TileSetArray.GetLength(0))
//         {
//             HashSet<MapGenTile> fromRight = new();
//             foreach (MapGenTile item in m_TileSetArray[x + 1, y])
//             {
//                 fromRight.UnionWith(item.leftTiles);
//             }
//
//             currentTile.IntersectWith(fromRight);
//         }
//
//         if (currentTile.Count == 1)
//         {
//             // await Task.Delay(100);
//             tilemap.SetTile(new Vector3Int(x, y), m_TileSetArray[x, y].First().tile);
//             m_UnusedPositions.Remove(new Vector2Int(x, y));
//         }
//         if (currentTile.Count == 0)
//         {
//             print($"TRAGEDIA ({x}, {y})");
//         }
//     }
// }