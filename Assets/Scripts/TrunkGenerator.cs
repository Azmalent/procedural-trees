using System;
using System.Collections.Generic;
using System.Linq;
using Tree_Generator.Assets.Scripts;
using UnityEngine;

public class TrunkGenerator : AbstractMeshGenerator
{
    private const int NUM_SIDES = 8;

    private const int RADIUS = 2;
    private const int HEIGHT = 10;

    private Mesh mesh;

    public TrunkGenerator(Mesh mesh)
    {
        this.mesh = mesh;
    }

    public override void Generate()
    {
        var vertices = new List<Vector3>();
        double angle = 2 * Math.PI / NUM_SIDES; 
        for (int i = 0; i < NUM_SIDES; i++)
        {
            float x = RADIUS * (float) Math.Cos(angle * i);
            float z = RADIUS * (float) Math.Sin(angle * i);
            vertices.Add(new Vector3(x, 0, z));
            vertices.Add(new Vector3(x, HEIGHT, z));
        }

        var triangles = new List<int>();
        for (int i = 0; i < NUM_SIDES - 2; i++)
        {
            triangles.Add(0);
            triangles.Add((i+1) * 2);
            triangles.Add((i+2) * 2);

            triangles.Add(1 + (i+2) * 2);
            triangles.Add(1 + (i+1) * 2);
            triangles.Add(1);
        }

        for (int i = 0; i < NUM_SIDES*2; i += 2)
        {
            triangles.Add(i);
            triangles.Add((i+1) % (NUM_SIDES*2));
            triangles.Add((i+2) % (NUM_SIDES*2));

            triangles.Add((i+3) % (NUM_SIDES*2));
            triangles.Add((i+2) % (NUM_SIDES*2));
            triangles.Add((i+1) % (NUM_SIDES*2));
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.colors = vertices.Select(_ => Color.yellow).ToArray();

        mesh.RecalculateNormals();
    }
}