using System;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class TreeGenerator : MeshGenerator
{
    const float BASE_THICKNESS_MULTIPLIER = 1.5f;

    protected readonly ProceduralTree tree;
    protected readonly int numSides;
    protected readonly float twisting;

    protected readonly Color barkColor;
    protected readonly ProceduralFoliageStyle foliageType;

    public TreeGenerator(ProceduralTree tree, Mesh mesh, TreeGenerator parent = null) : base(mesh, parent)
    {
        this.tree = tree;
        numSides = tree.Roundness;

        twisting = tree.Twisting;

        barkColor = tree.BarkColor;
        foliageType = tree.FoliageStyle;
    }

    /// <summary>
    /// Adds a ring of vertices to the mesh.
    /// </summary>
    /// <param name="centerPos">Position of the center point.</param>
    /// <param name="radius">Radius of the ring.</param>
    /// <param name="rotation">Rotation of the ring.</param>
    /// <param name="color">Color of the vertices.</param>
    protected void AddRing(Vector3 centerPos, float radius, Quaternion rotation, Color color)
    {
        double angle = 2 * Math.PI / numSides;
        for (int i = 0; i < numSides; i++)
        {
            float x = (float)Math.Cos(angle * i);
            float z = (float)Math.Sin(angle * i);
            var offset = radius * new Vector3(x, 0, z);
            var pos = centerPos + rotation * offset;
            AddVertex(pos, color);
        }
    }

    /// <summary>
    /// Caps the branch with a new vertex, connecting it to the last added ring.
    /// </summary>
    /// <param name="pos">The position of the new vertex.</param>
    protected void AddBranchCap(Vector3 pos, Color color)
    {
        int startIndex = vertices.Count - numSides;
        Assert.IsTrue(startIndex >= 0);

        AddVertex(pos, color);
        for (int i = startIndex; i < vertices.Count - 2; i++) AddTriangle(i, vertices.Count - 1, i + 1);

        AddTriangle(vertices.Count - 2, vertices.Count - 1, startIndex);
    }

    /// <summary>
    /// Connects two last added rings with triangles.
    /// </summary>
    protected void ConnectRings()
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
    /// Adds foliage to the end of the branch as a new GameObject.
    /// </summary>
    /// <param name="rotation">Rotation of the foliage</param>
    /// <param name="pos">Position of the foliage</param>
    protected void AddFoliage(Quaternion rotation, Vector3 pos)
    {
        if (foliageType == ProceduralFoliageStyle.None) return;

        var leaves = new GameObject("Foliage");
        leaves.transform.parent = tree.gameObject.transform;
        leaves.transform.localPosition = pos;
        leaves.transform.rotation = rotation;

        var filter = leaves.AddComponent<MeshFilter>();
        var renderer = leaves.AddComponent<MeshRenderer>();
        renderer.material = tree.FoliageMaterial;

        FoliageGenerator generator;
        switch (foliageType)
        {
            case ProceduralFoliageStyle.Round:
                generator = new RoundLeavesGenerator(tree, filter.mesh);
                break;

            case ProceduralFoliageStyle.Flat:
                generator = new FlatLeavesGenerator(tree, filter.mesh);
                break;

            default:
                throw new ArgumentException("Unsupported foliage type");
        }

        generator.GenerateMesh();
    }
}