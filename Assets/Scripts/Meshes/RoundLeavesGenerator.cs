using System.Collections.Generic;
using UnityEngine;

public class RoundLeavesGenerator : FoliageGenerator
{
    //Golden ratio
    const float PHI = 1.61803399f;

    private float width;
    private float height;
    private Quaternion rotation;

    public RoundLeavesGenerator(ProceduralTree tree, Mesh mesh, Quaternion rotation) : base(tree, mesh)
    {
        width = tree.FoliageWidth;
        height = tree.FoliageHeight;

        this.rotation = rotation;
    }

    /// <summary>
    /// Generates a basic icosahedron.
    /// </summary>
    /// <param name="rotation">Icosahedron's rotation</param>
    /// <param name="width">Icosahedron's width</param>
    /// <param name="height">Icosahedron's height</param>
    private void GenerateIcosahedron(Quaternion rotation, float width, float height)
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

        Subdivide();
        AdjustRadius();
    }

    /// <summary>
    /// Finds the vertex in the middle of a specified segment. 
    /// If this vertex does not exist, it is added. 
    /// </summary>
    /// <param name="aIndex">Index of first vertex</param>
    /// <param name="bIndex">Index of second vertex</param>
    /// <returns>Index of the vertex between vertices A and B</returns>
    private int FindOrCreateMiddle(int aIndex, int bIndex)
    {
        Vector3 a = vertices[aIndex];
        Vector3 b = vertices[bIndex];
        Vector3 middle = Vector3.Lerp(a, b, 0.5f).normalized;

        int index = vertices.IndexOf(middle);
        if (index > -1) return index;

        return AddVertex(middle, color);
    }

    /// <summary>
    /// Subdivides every triangle into four new ones.
    /// </summary>
    private void Subdivide()
    {
        int n = triangles.Count;
        for (int i = 0; i < n; i += 3)
        {
            int a = FindOrCreateMiddle(triangles[i], triangles[i + 1]);
            int b = FindOrCreateMiddle(triangles[i + 1], triangles[i + 2]);
            int c = FindOrCreateMiddle(triangles[i + 2], triangles[i]);

            AddTriangle(c, triangles[i], a);
            AddTriangle(a, triangles[i + 1], b);
            AddTriangle(b, triangles[i + 2], c);
            AddTriangle(a, b, c);
        }

        triangles.RemoveRange(0, n);
    }

    /// <summary>
    /// Adjusts the radius of the vertices of the icosahedron.
    /// </summary>
    private void AdjustRadius()
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            Vector3 v = vertices[i];
            float radius = Mathf.Lerp(width, height, Mathf.Sin(v.y / v.magnitude));
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
            float minDistance = float.MaxValue;

            Vector3 v = vertices[i];
            foreach (int j in FindNeighbors(i))
            {
                Vector3 neighbor = vertices[j];
                float distance = Vector3.Distance(v, neighbor);
                if (distance < minDistance) minDistance = distance;
            }

            float magnitude = Random.value * (minDistance / 3);
            Vector3 displacement = Random.insideUnitSphere * magnitude;

            vertices[i] = v + displacement;
        }
    }

    public override void GenerateMesh()
    {
        GenerateIcosahedron(rotation * Random.rotation, width, height);
        DisplaceVertices();
        PersistMesh();
    }
}