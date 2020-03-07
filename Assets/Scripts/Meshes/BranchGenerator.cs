using System;
using System.Collections.Generic;
using Tree_Generator.Assets.Scripts;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

using URandom = UnityEngine.Random;

public class BranchGenerator : MonoBehaviour, IMeshGenerator
{
    private readonly Mesh mesh;

    private readonly List<Vector3> vertices;
    private readonly List<int> triangles;

    private readonly int numSegments;
    private readonly int numSides;
    private readonly float baseRadius;
    private readonly float twisting;

    public BranchGenerator(ProceduralTree tree, Mesh mesh)
    { 
        this.mesh = mesh;

        numSegments = tree.Height;
        numSides = tree.Roundness;
        baseRadius = tree.BaseRadius;
        
        twisting = tree.Twisting;

        vertices = new List<Vector3>();
        triangles = new List<int>();
    }

    private void AddRing(Vector3 centerPos, float radius, Quaternion rotation)
    {
        double angle = 2*Math.PI / numSides; 
        for (int i = 0; i < numSides; i++)
        {
            float x = (float) Math.Cos(angle * i);
            float z = (float) Math.Sin(angle * i);
            var offset = radius * new Vector3(x, 0, z);
            var pos = centerPos + rotation * offset;
            vertices.Add(pos);
        }
    }

    private void AddBranchCap(Vector3 position)
    {
        int startIndex = vertices.Count - numSides;
        Assert.IsTrue(startIndex >= 0);

        vertices.Add(position);
        for (int i = startIndex; i < vertices.Count-2; i++)
        {
            AddTriangle(i, vertices.Count-1, i+1);
        }
        AddTriangle(vertices.Count-2, vertices.Count-1, startIndex);
    }

    /// <summary>
    /// Adds a triangle to the list.
    /// </summary>
    private void AddTriangle(int a, int b, int c)
    {
        Assert.IsFalse(a < 0);
        Assert.IsFalse(b < 0);
        Assert.IsFalse(c < 0);

        triangles.Add(a);
        triangles.Add(b);
        triangles.Add(c);
    }

    /// <summary>
    /// <para> One method to rule them all       </para>
    /// <para> One method to find them           </para>
    /// <para> One method to bring them all      </para>
    /// <para> And with triangles bind them      </para>
    /// </summary>
    private void ConnectRings()
    {
        int startIndex = vertices.Count - 2*numSides;
        Assert.IsTrue(startIndex >= 0);

        for (int i = 0; i <= numSides-1; i++)
        {
            int cur = startIndex + i;
            int next = (i < numSides-1) ? cur+1 : startIndex;
            AddTriangle(cur+numSides, next, cur);
            AddTriangle(cur+numSides, next+numSides, next);
        }
    }

    public void GenerateMesh()
    {
        var rotation = Quaternion.identity;
        Vector3 pos = Vector3.zero;

        //Bottom of the tree
        AddRing(pos, baseRadius * 1.5f, rotation);
        for (int i = 0; i < numSides-2; i++) AddTriangle(0, i+1, i+2);

        for (int i = 0; i < numSegments; i++)
        {
            float radius = Mathf.Lerp(baseRadius, 0, (float) i / numSegments);

            // Randomize the branch angle and extend the branch, with random offsets
            float xRotation = (URandom.value - 0.5f) * twisting;
            float zRotation = (URandom.value - 0.5f) * twisting;
            rotation *= Quaternion.Euler(xRotation, 0f, zRotation);

            float xShift = URandom.value - 0.5f;
            float zShift = URandom.value - 0.5f;
            pos += rotation * Vector3.up + new Vector3(xShift, 0, zShift);

            AddRing(pos, radius, rotation);
            ConnectRings();
        }

        pos += rotation * Vector3.up;
        AddBranchCap(pos);
        FinishMesh();
    }

    public void FinishMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        MeshUtility.Optimize(mesh);
    }
}