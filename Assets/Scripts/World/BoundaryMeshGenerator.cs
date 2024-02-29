using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryMeshGenerator : MonoBehaviour
{
    public float MaxX = 500f;
    public float MinX = -500f;
    public float MaxY = 500f;
    public float MinY = -500f;

    public float maxx = 500f;
    public float minx = 500f;
    public float maxy = 500f;
    public float miny = 500f;

    [SerializeField] private MeshFilter m_BoundaryMeshFilter;
    [SerializeField] private MeshCollider m_BoundaryMeshCollider;

    private void Start()
    {
        CreateMesh();
    }

    private void CreateMesh()
    {
        Mesh boundaryMesh = new Mesh();

        const float yOffset = 0.05f;

        Vector3[] newVertices =
        {
            new Vector3(MinX, yOffset, MaxY), // outer left top
            new Vector3(MaxX, yOffset, MaxY), // outer right top
            new Vector3(MinX, yOffset, MinY), // outer left bottom
            new Vector3(MaxX, yOffset, MinY), // outer right bottom
            new Vector3(minx, yOffset, maxy), // inner left top
            new Vector3(maxx, yOffset, maxy), // inner right top
            new Vector3(minx, yOffset, miny), // inner left bottom
            new Vector3(maxx, yOffset, miny) // inner right bottom
        };
        int[] newTriangles =
        {
            0, 1, 5,
            0, 5, 4,
            0, 4, 2,
            3, 7, 1,
            3, 6, 7,
            3, 2, 6,
            1, 7, 5,
            2, 4, 6
        };

        boundaryMesh.vertices = newVertices;
        //boundaryMesh.uv = newUV;
        boundaryMesh.triangles = newTriangles;

        m_BoundaryMeshFilter.mesh = boundaryMesh;

        m_BoundaryMeshCollider.sharedMesh = m_BoundaryMeshFilter.sharedMesh;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(new Vector3((maxx + minx) / 2f, 0f, (maxy + miny) / 2f), new Vector3(maxx - minx, 1f, maxy - miny));
    }
}