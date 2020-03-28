using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

using URandom = UnityEngine.Random;

public class RoundLeavesGenerator : FoliageGenerator
{
    const float MAX_DISPLACEMENT_MAGNITUDE = 0.4f;

    private float radius;
    
    public RoundLeavesGenerator(ProceduralTree tree, Mesh mesh) : base(tree, mesh) 
    {
        radius = tree.FoliageSize;
    }

    private void GenerateIcosahedron(float radius)
    {    
        // create 12 vertices of a icosahedron
        float t = (1f + Mathf.Sqrt(5f)) / 2f;

        AddVertex(new Vector3(-1f,  t,  0f).normalized * radius, color);
        AddVertex(new Vector3( 1f,  t,  0f).normalized * radius, color);
        AddVertex(new Vector3(-1f, -t,  0f).normalized * radius, color);
        AddVertex(new Vector3( 1f, -t,  0f).normalized * radius, color);

        AddVertex(new Vector3( 0f, -1f,  t).normalized * radius, color);
        AddVertex(new Vector3( 0f,  1f,  t).normalized * radius, color);
        AddVertex(new Vector3( 0f, -1f, -t).normalized * radius, color);
        AddVertex(new Vector3( 0f,  1f, -t).normalized * radius, color);

        AddVertex(new Vector3( t,  0f, -1f).normalized * radius, color);
        AddVertex(new Vector3( t,  0f,  1f).normalized * radius, color);
        AddVertex(new Vector3(-t,  0f, -1f).normalized * radius, color);
        AddVertex(new Vector3(-t,  0f,  1f).normalized * radius, color);

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

    private void DisplaceVertices()
    {
        for(int i = 0; i < vertices.Count; i++)
        {
            Vector3 v = vertices[i];
            float magnitude = Random.value * MAX_DISPLACEMENT_MAGNITUDE;
            Vector3 displacement = Random.insideUnitSphere * magnitude;
            
            vertices[i] = v + displacement;
        }
    }

    public override void GenerateMesh()
    {
        GenerateIcosahedron(radius);
        DisplaceVertices();
        PersistMesh();
    }
}