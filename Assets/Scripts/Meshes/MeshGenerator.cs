﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class MeshGenerator
{
    protected readonly Mesh mesh;

    protected readonly List<Vector3> vertices = new List<Vector3>();
    protected readonly List<int> triangles = new List<int>();
    protected readonly List<Color> colors = new List<Color>();

    protected MeshGenerator(Mesh mesh)
    {
        this.mesh = mesh;
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