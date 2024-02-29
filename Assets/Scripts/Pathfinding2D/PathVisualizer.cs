using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding2D
{
    [RequireComponent(typeof(LineRenderer))]
    public class PathVisualizer : MonoBehaviour
    {
        [SerializeField] private PathfindingAgent2D m_PathfindingAgent2D;
        [SerializeField, HideInInspector] private LineRenderer m_LineRenderer;

        private void Start()
        {
            m_LineRenderer.enabled = false;

            m_PathfindingAgent2D.OnPathStart.AddListener(PathStart);
            m_PathfindingAgent2D.OnPathEnd.AddListener(PathEnd);
        }

        private void Update()
        {
            if (m_PathfindingAgent2D.path != null)
            {
                m_LineRenderer.positionCount = m_PathfindingAgent2D.path.Count + 2;

                m_LineRenderer.SetPosition(m_LineRenderer.positionCount-1, m_PathfindingAgent2D.transform.position);
                Vector2Int currentTarget = m_PathfindingAgent2D.CurrentTarget;
                m_LineRenderer.SetPosition(m_LineRenderer.positionCount-2, new Vector3(currentTarget.x + 0.5f, currentTarget.y + 0.5f));

                int i = m_LineRenderer.positionCount-3;

                foreach (Vector2Int point in m_PathfindingAgent2D.path)
                {
                    m_LineRenderer.SetPosition(i, new Vector3(point.x + 0.5f, point.y + 0.5f));
                    i--;
                }
            }
        }

        private void PathStart(Vector2Int _)
        {
            m_LineRenderer.enabled = true;
        }

        private void PathEnd(Vector2Int _)
        {
            m_LineRenderer.enabled = false;
        }

        private void OnDestroy()
        {
            m_PathfindingAgent2D.OnPathStart.RemoveListener(PathStart);
            m_PathfindingAgent2D.OnPathEnd.RemoveListener(PathEnd);
        }

        private void OnValidate()
        {
            if (!m_LineRenderer) m_LineRenderer = GetComponent<LineRenderer>();
        }
    }
}