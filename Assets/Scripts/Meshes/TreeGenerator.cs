using System;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class TreeGenerator : MeshGenerator
{
    const float BASE_THICKNESS_MULTIPLIER = 1.5f;

    protected readonly ProceduralTree tree;
    protected int numSides;
    protected readonly float twisting;

    protected readonly Color barkColor;
    protected readonly FoliageStyle foliageType;

    public TreeGenerator(ProceduralTree tree, Mesh mesh, TreeGenerator parent = null) : base(mesh, parent)
    {
        this.tree = tree;

        twisting = tree.Twisting;

        barkColor = tree.BarkColor;
        foliageType = tree.FoliageStyle;
    }


    /// <summary>
    /// Adds foliage to the end of the branch as a new GameObject.
    /// </summary>
    /// <param name="rotation">Rotation of the foliage</param>
    /// <param name="pos">Position of the foliage</param>
    protected void AddFoliage(float scale, Vector3 pos, Quaternion rotation)
    {
        AddFoliage(scale, pos, null, new[] { rotation }, null);
    }

    protected void AddFoliage(float scale, Vector3 pos, Vector3[] positions, Quaternion[] rotations, float[] radiuses)
    {
        if (foliageType == FoliageStyle.None) return;


        var leaves = new GameObject("Foliage");
        leaves.transform.parent = tree.gameObject.transform;
        leaves.transform.localPosition = Vector3.zero;

        //Conifer leaves have the same position as trunk and no rotation
        if (foliageType != FoliageStyle.Coniferous)
        {
            leaves.transform.localPosition = pos;
            leaves.transform.rotation = rotations[rotations.Length - 1];
        }

        var filter = leaves.AddComponent<MeshFilter>();
        var renderer = leaves.AddComponent<MeshRenderer>();
        renderer.materials = tree.FoliageMaterials;

        FoliageGenerator generator;
        switch (foliageType)
        {
            case FoliageStyle.Round:
                generator = new RoundLeavesGenerator(tree, filter.mesh, scale);
                break;

            case FoliageStyle.Flat:
                generator = new FlatLeavesGenerator(tree, filter.mesh, scale);
                break;

            case FoliageStyle.Coniferous:
                generator = new ConiferousLeavesGenerator(tree, filter.mesh, scale, positions, rotations, radiuses);
                break;

            default:
                throw new ArgumentException("Unsupported foliage type");
        }

        generator.GenerateMesh();
    }
}