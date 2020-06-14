using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class MeshGenerator
{
    protected readonly Mesh mesh;

    protected List<Vector3> vertices;
    protected List<int> triangles;
    protected List<Color> colors;
    protected List<Vector2> uvs;

    protected MeshGenerator(Mesh mesh, MeshGenerator parent = null)
    {
        this.mesh = mesh;

        if (parent == null)
        {
            vertices = new List<Vector3>();
            triangles = new List<int>();
            colors = new List<Color>();
            uvs = new List<Vector2>();
        }
        else
        {
            vertices = parent.vertices;
            triangles = parent.triangles;
            colors = parent.colors;
            uvs = parent.uvs;
        }
    }

    /// <summary>
    /// Adds a vertex with a specified color.
    /// </summary>
    /// <param name="pos">Position of the vertex.</param>
    /// <param name="color">Color of the vertex.</param>
    /// <returns>The index of the newly added vertex.</returns>
    protected int AddVertex(Vector3 pos, Color color)
    {
        vertices.Add(pos);
        colors.Add(color);

        return vertices.Count - 1;
    }

    /// <summary>
    /// Adds a triangle to the list.
    /// </summary>
    protected void AddTriangle(int a, int b, int c)
    {
        AssertHelper.NotNegative(a, b, c);

        triangles.Add(a);
        triangles.Add(b);
        triangles.Add(c);
    }

    /// <summary>
    /// Adds a rectangle consisting of two triangles to the list.
    /// </summary>
    protected void AddRectangle(int a, int b, int c, int d)
    {
        AssertHelper.NotNegative(a, b, c, d);

        triangles.Add(a);
        triangles.Add(b);
        triangles.Add(c);

        triangles.Add(c);
        triangles.Add(d);
        triangles.Add(a);
    }

    /// <summary>
    /// Adds a ring of vertices to the mesh.
    /// </summary>
    /// <param name="centerPos">Position of the center point.</param>
    /// <param name="radius">Radius of the ring.</param>
    /// <param name="rotation">Rotation of the ring.</param>
    /// <param name="color">Color of the vertices.</param>
    protected void AddRing(int numSides, Vector3 centerPos, float radius, Quaternion rotation, Color color)
    {
        float angle = 2 * Mathf.PI / numSides;
        for (int i = 0; i < numSides; i++)
        {
            float x = (float)Mathf.Cos(angle * i);
            float z = (float)Mathf.Sin(angle * i);
            var offset = radius * new Vector3(x, 0, z);
            var pos = centerPos + rotation * offset;
            AddVertex(pos, color);
        }
    }

    /// <summary>
    /// Connects two last added rings with triangles.
    /// </summary>
    protected void ConnectRings(int numSides)
    {
        int startIndex = vertices.Count - 2 * numSides;
        Assert.IsTrue(startIndex >= 0);

        for (int i = 0; i < numSides; i++)
        {
            int cur = startIndex + i;
            int next = (i < numSides - 1) ? cur + 1 : startIndex;
            AddRectangle(cur + numSides, next + numSides, next, cur);
        }
    }

    /// <summary>
    /// Adds a new vertex, connected it to the last added ring.
    /// </summary>
    /// <param name="pos">The position of the new vertex.</param>
    protected void AddCap(int numSides, Vector3 pos, Color color)
    {
        int startIndex = vertices.Count - numSides;
        Assert.IsTrue(startIndex >= 0);

        AddVertex(pos, color);
        for (int i = startIndex; i < vertices.Count - 2; i++) AddTriangle(i, vertices.Count - 1, i + 1);

        AddTriangle(vertices.Count - 2, vertices.Count - 1, startIndex);
    }

    public abstract void GenerateMesh();

    protected void PersistMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.colors = colors.ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();
    }
}