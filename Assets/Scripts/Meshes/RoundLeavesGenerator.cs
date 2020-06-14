using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates round foliage.
/// </summary>
public class RoundLeavesGenerator : FoliageGenerator
{
    //Golden ratio
    private const float PHI = 1.61803399f;

    public RoundLeavesGenerator(ProceduralTree tree, Mesh mesh, float scale) : base(tree, mesh, scale)
    {
    }

    /// <summary>
    /// Generates a basic, randomly rotated icosahedron.
    /// </summary>
    private void GenerateIcosahedron()
    {
        var vectors = new Vector3[] {
            new Vector3(-1f, PHI, 0f),
            new Vector3(1f, PHI, 0f),
            new Vector3(-1f, -PHI, 0f),
            new Vector3(1f, -PHI, 0f),

            new Vector3(0f, -1f, PHI),
            new Vector3(0f, 1f, PHI),
            new Vector3(0f, -1f, -PHI),
            new Vector3(0f, 1f, -PHI),

            new Vector3(PHI, 0f, -1f),
            new Vector3(PHI, 0f, 1f),
            new Vector3(-PHI, 0f, -1f),
            new Vector3(-PHI, 0f, 1f)
        };

        var rotation = Random.rotation;
        foreach (Vector3 v in vectors) AddVertex(rotation * v.normalized, color);

        // 5 faces around point 0
        AddTriangle(0, 11, 5);
        AddTriangle(0, 5, 1);
        AddTriangle(0, 1, 7);
        AddTriangle(0, 7, 10);
        AddTriangle(0, 10, 11);

        // 5 adjacent faces 
        AddTriangle(1, 5, 9);
        AddTriangle(5, 11, 4);
        AddTriangle(11, 10, 2);
        AddTriangle(10, 7, 6);
        AddTriangle(7, 1, 8);

        // 5 faces around point 3
        AddTriangle(3, 9, 4);
        AddTriangle(3, 4, 2);
        AddTriangle(3, 2, 6);
        AddTriangle(3, 6, 8);
        AddTriangle(3, 8, 9);

        // 5 adjacent faces 
        AddTriangle(4, 9, 5);
        AddTriangle(2, 4, 11);
        AddTriangle(6, 2, 10);
        AddTriangle(8, 6, 7);
        AddTriangle(9, 8, 1);
    }

    /// <summary>
    /// Subdivides every triangle into four new ones.
    /// </summary>
    private void Subdivide()
    {
        int n = triangles.Count;
        for (int i = 0; i < n; i += 3)
        {
            int a = FindOrCreateMiddle(triangles[i], triangles[i + 1], true);
            int b = FindOrCreateMiddle(triangles[i + 1], triangles[i + 2], true);
            int c = FindOrCreateMiddle(triangles[i + 2], triangles[i], true);

            AddTriangle(c, triangles[i], a);
            AddTriangle(a, triangles[i + 1], b);
            AddTriangle(b, triangles[i + 2], c);
            AddTriangle(a, b, c);
        }

        triangles.RemoveRange(0, n);
    }

    private float GetRadius(Vector3 v)
    {
        return Mathf.Lerp(width, height, Mathf.Abs(v.y) / v.magnitude);
    }

    /// <summary>
    /// Adjusts the radius of the vertices of the icosahedron.
    /// </summary>
    private void AdjustRadius()
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            Vector3 v = vertices[i];
            float radius = GetRadius(v);
            vertices[i] = v * radius;
        }
    }

    /// <summary>
    /// Returns indices of every vertex connected to k.
    /// </summary>
    /// <param name="k">Index of the vertex</param>
    /// <returns>IEnumerable of k's neighbors</returns>
    private IEnumerable<int> FindNeighbors(int k)
    {
        for (int i = 0; i < triangles.Count; i += 3)
        {
            if (triangles[i] != k &&
                triangles[i + 1] != k &&
                triangles[i + 2] != k) continue;

            for (int j = i; j < i + 3; j++)
            {
                int other = triangles[j];
                if (other != k) yield return other;
            }
        }
    }

    /// <summary>
    /// Displaces vertices randomly while preserving the shape of the figure.
    /// </summary>
    private void DisplaceVertices()
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            Vector3 v = vertices[i];

            float minDistance = float.MaxValue;
            foreach (int j in FindNeighbors(i))
            {
                Vector3 neighbor = vertices[j];
                float distance = Vector3.Distance(v, neighbor);
                if (distance < minDistance) minDistance = distance;
            }

            float magnitude = Random.value * (minDistance / 3);

            float oldRadius = GetRadius(v);

            Vector3 displacement = Random.insideUnitSphere * magnitude;
            vertices[i] = (v + displacement).normalized * oldRadius;

            float minRadius = oldRadius * 0.8f;
            if (vertices[i].magnitude < minRadius)
            {
                vertices[i] = vertices[i].normalized * minRadius;
            }
        }
    }

    public override void GenerateMesh()
    {
        GenerateIcosahedron();
        if (Mathf.Min(width, height) >= 3 || Mathf.Abs(width - height) >= 1)
        {
            Subdivide();
        }
        AdjustRadius();
        DisplaceVertices();
        PersistMesh();
    }
}