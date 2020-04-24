using System;
using UnityEngine;
using UnityEngine.Assertions;

public class FlatLeavesGenerator : FoliageGenerator
{
    private Quaternion rotation;

    public FlatLeavesGenerator(ProceduralTree tree, Mesh mesh, Quaternion rotation) : base(tree, mesh)
    {
        this.rotation = rotation;
    }

    private void AddTwoSidedTriangle(int a, int b, int c)
    {
        Assert.IsFalse(a < 0);
        Assert.IsFalse(b < 0);
        Assert.IsFalse(c < 0);

        triangles.Add(a);
        triangles.Add(b);
        triangles.Add(c);

        triangles.Add(c);
        triangles.Add(b);
        triangles.Add(a);
    }

    protected void AddTwoSidedRectangle(int a, int b, int c, int d)
    {
        AssertHelper.NotNegative(a, b, c);

        triangles.Add(a);
        triangles.Add(b);
        triangles.Add(c);

        triangles.Add(a);
        triangles.Add(c);
        triangles.Add(d);

        triangles.Add(c);
        triangles.Add(b);
        triangles.Add(a);

        triangles.Add(d);
        triangles.Add(c);
        triangles.Add(a);
    }

    private void GenerateLeaf(Quaternion rotation)
    {
        float x = 2.5f;
        float y = -0.5f;
        float z = 0.5f;

        int startIndex = vertices.Count;
        AddVertex(Vector3.zero, color);
        AddVertex(rotation * new Vector3(x, y, -z), color);
        AddVertex(rotation * new Vector3(x, y, z), color);
        AddVertex(rotation * new Vector3(2 * x, 2.5f * y, z), color);
        AddVertex(rotation * new Vector3(2 * x, 2.5f * y, -z), color);
        AddVertex(rotation * new Vector3(3 * x, 6f * y, 0), color);

        AddTwoSidedTriangle(startIndex, startIndex + 1, startIndex + 2);
        AddTwoSidedRectangle(startIndex + 1, startIndex + 2, startIndex + 3, startIndex + 4);
        AddTwoSidedTriangle(startIndex + 3, startIndex + 4, startIndex + 5);
    }

    public override void GenerateMesh()
    {
        int numLeaves = 5;
        float angle = 360f / numLeaves;
        Debug.Log(angle);
        for (int i = 0; i < numLeaves; i++)
        {
            var q = Quaternion.Euler(0, angle * i, 0);
            GenerateLeaf(q);
        }

        PersistMesh();
    }
}