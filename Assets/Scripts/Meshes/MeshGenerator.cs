using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public abstract class MeshGenerator
{
    protected readonly Mesh mesh;

    protected List<Vector3> vertices;
    protected List<int> triangles;
    protected List<Color> colors;

    protected MeshGenerator(Mesh mesh, MeshGenerator parent = null)
    {
        this.mesh = mesh;

        if (parent == null)
        {
            vertices = new List<Vector3>();
            triangles = new List<int>();
            colors = new List<Color>();
        }
        else
        {
            vertices = parent.vertices;
            triangles = parent.triangles;
            colors = parent.colors;
        }
    }

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

    public abstract void GenerateMesh();

    protected void PersistMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.colors = colors.ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        MeshUtility.Optimize(mesh);
    }
}