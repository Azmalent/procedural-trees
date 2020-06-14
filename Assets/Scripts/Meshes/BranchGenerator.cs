using System;
using UnityEngine;
using UnityEngine.Assertions;

using URandom = UnityEngine.Random;

public class BranchGenerator : TreeGenerator
{
    private readonly Vector3 position;
    private readonly float initialRadius;
    private readonly Quaternion initialRotation;

    private readonly float segmentLength;
    private readonly int numSegments;

    private readonly float foliageScale;

    public BranchGenerator(ProceduralTree tree, Mesh mesh, TrunkGenerator parent,
        Vector3 position, Vector3 direction, float radius, float segmentLength, int numSegments) : base(tree, mesh, parent)
    {
        this.position = position;
        initialRadius = radius;

        initialRotation = Quaternion.FromToRotation(Vector3.up, direction);

        float t = Mathf.InverseLerp(
            ProceduralTree.MIN_THICKNESS, ProceduralTree.MAX_THICKNESS, initialRadius
        );
        numSides = (int)Mathf.Floor(
            Mathf.Lerp(ProceduralTree.MIN_ROUNDNESS, ProceduralTree.MAX_ROUNDNESS, t)
        );

        this.segmentLength = segmentLength;
        this.numSegments = numSegments;

        this.foliageScale = initialRadius / tree.Thickness;
    }

    public override void GenerateMesh()
    {
        float radius = initialRadius;
        Vector3 pos = position;
        var rotation = initialRotation;

        AddRing(numSides, pos, radius, rotation, barkColor);

        for (int i = 0; i < numSegments; i++)
        {
            float t = (float)(i + 1) / numSegments;
            radius = Mathf.Lerp(initialRadius, 0.2f * initialRadius, t);

            // Randomizing the branch angle
            float xRotation = (URandom.value - 0.5f) * twisting;
            float zRotation = (URandom.value - 0.5f) * twisting;
            var randomRotation = Quaternion.Euler(xRotation, 0f, zRotation);
            rotation = Quaternion.Lerp(initialRotation, randomRotation, t);

            var direction = Vector3.Lerp(initialRotation * Vector3.up, Vector3.up, t - 0.5f);
            var shift = new Vector3(URandom.value - 0.5f, 0, URandom.value - 0.5f);
            pos += direction * segmentLength + shift;

            AddRing(numSides, pos, radius, rotation, barkColor);
            ConnectRings(numSides);
        }

        var capPos = pos + Vector3.up;
        AddCap(numSides, capPos, barkColor);
        AddFoliage(foliageScale, capPos, rotation);

        PersistMesh();
    }
}