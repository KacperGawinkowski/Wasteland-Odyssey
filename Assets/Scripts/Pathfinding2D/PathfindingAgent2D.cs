using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using World;

namespace Pathfinding2D
{
    public class PathfindingAgent2D : MonoBehaviour
    {
        [SerializeField] private Tilemap m_Tilemap;

        private PathfindingGrid m_PathfindingGrid;

        public Vector2Int CurrentTarget { get; private set; }
        public Vector2Int CurrentTile { get; private set; }

        public Queue<Vector2Int> path = new();

        [SerializeField] private float m_MoveSpeed = 1f;
        [SerializeField] private float m_MoveProgress;
        [SerializeField] private float m_CurrentSpeed;
        private bool m_TileChanged;

        public UnityEvent<Vector2Int> OnTileCzanged;
        public UnityEvent<Vector2Int> OnPathStart;
        public UnityEvent<Vector2Int> OnPathEnd;

        public bool forceStop;

        // Start is called before the first frame update
        void Start()
        {
            m_PathfindingGrid = FindObjectOfType<PathfindingGrid>();

            if (m_PathfindingGrid == null)
            {
                Debug.LogError("Pathfinding grid not found");
                return;
            }

            if (SaveSystem.saveContent.playerGlobalMapPosition != null)
            {
                Vector3Int pos = new(SaveSystem.saveContent.playerGlobalMapPosition.Value.x, SaveSystem.saveContent.playerGlobalMapPosition.Value.y);
                transform.position = m_Tilemap.CellToWorld(pos) + m_Tilemap.tileAnchor;
                CurrentTile = new Vector2Int(pos.x, pos.y);
                CurrentTarget = new Vector2Int(pos.x, pos.y);
            }
            else
            {
                Vector3Int cellPos = m_Tilemap.WorldToCell(transform.position);
                CurrentTile = new Vector2Int(cellPos.x, cellPos.y);
                CurrentTarget = new Vector2Int(cellPos.x, cellPos.y);
            }

            OnPathEnd.Invoke(CurrentTile);

            SaveSystem.OnUpdateSaveContent += SaveLocation;

            // m_CurrentSpeed = m_PathfindingGrid.grid[m_CurrentTile.x, m_CurrentTile.y].baseCost;
        }

        // Update is called once per frame
        void Update()
        {
            if (forceStop) return;

            if (CurrentTarget != CurrentTile)
            {
                //print(m_MoveProgress);

                m_MoveProgress += (Time.deltaTime / m_CurrentSpeed) * m_MoveSpeed;

                Vector2 point = Vector2.Lerp(
                    m_Tilemap.CellToWorld(new Vector3Int(CurrentTile.x, CurrentTile.y)),
                    m_Tilemap.CellToWorld(new Vector3Int(CurrentTarget.x, CurrentTarget.y)),
                    m_MoveProgress);

                transform.position = point + new Vector2(m_Tilemap.tileAnchor.x, m_Tilemap.tileAnchor.y);

                while ((m_MoveProgress >= 1) || (!m_TileChanged && m_MoveProgress >= 0.5f))
                {
                    if (!m_TileChanged && m_MoveProgress >= 0.5f)
                    {
                        m_CurrentSpeed = m_PathfindingGrid.grid[CurrentTarget.x, CurrentTarget.y].baseCost * Vector2Int.Distance(CurrentTile, CurrentTarget);
                        m_TileChanged = true;
                        OnTileCzanged.Invoke(CurrentTarget);
                    }

                    if (m_MoveProgress >= 1)
                    {
                        m_MoveProgress -= 1;
                        CurrentTile = CurrentTarget;
                        m_TileChanged = false;
                        if (path.Count > 0)
                        {
                            CurrentTarget = path.Dequeue();
                            m_CurrentSpeed = m_PathfindingGrid.grid[CurrentTile.x, CurrentTile.y].baseCost * Vector2Int.Distance(CurrentTile, CurrentTarget);
                        }
                        else
                        {
                            OnPathEnd.Invoke(CurrentTile);
                        }
                    }
                }
            }
            else
            {
                if (path.Count > 0)
                {
                    CurrentTarget = path.Dequeue();
                    m_CurrentSpeed = m_PathfindingGrid.grid[CurrentTile.x, CurrentTile.y].baseCost * Vector2Int.Distance(CurrentTile, CurrentTarget);
                    OnPathStart.Invoke(CurrentTile);
                }
            }
        }

        public void FindPath(Vector2Int destination)
        {
            path.Clear();
            m_PathfindingGrid.FindPath(CurrentTarget, destination, path);
        }

        private void OnDestroy()
        {
            SaveSystem.OnUpdateSaveContent -= SaveLocation;
        }

        private void SaveLocation()
        {
            SaveSystem.saveContent.playerGlobalMapPosition = new Vector2Int(CurrentTile.x, CurrentTile.y);
        }
    }

    //private void OnDrawGizmos()
    ////private void OnDrawGizmosSelected()
    //{
    //    for (int i = 0; i < m_Path.Count; i++)
    //    {
    //        Gizmos.DrawWireSphere(m_Path[i], 0.5f);
    //    }
    //}
}