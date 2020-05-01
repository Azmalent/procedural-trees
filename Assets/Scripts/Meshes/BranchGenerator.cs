using System;
using UnityEngine;
using UnityEngine.Assertions;

using URandom = UnityEngine.Random;

public class BranchGenerator : TreeGenerator
{
    private readonly Vector3 position;
    private readonly float initialRadius;
    private readonly Quaternion initialRotation;

    public BranchGenerator(ProceduralTree tree, Mesh mesh, TrunkGenerator parent,
        Vector3 position, Vector3 direction, float radius) : base(tree, mesh, parent)
    {
        this.position = position;
        initialRadius = radius;

        initialRotation = Quaternion.FromToRotation(Vector3.up, direction);
        Debug.Log(initialRotation);
    }

    public override void GenerateMesh()
    {
        int length = 10; //TODO

        float radius = initialRadius;
        Vector3 pos = position;
        var rotation = initialRotation;

        AddRing(pos, radius, rotation, barkColor);

        for (int i = 0; i < length; i++)
        {
            float t = (float)i / length;
            radius = Mathf.Lerp(initialRadius, 0.2f * initialRadius, t);

            // Randomizing the branch angle
            float xRotation = (URandom.value - 0.5f) * twisting;
            float zRotation = (URandom.value - 0.5f) * twisting;
            var randomRotation = Quaternion.Euler(xRotation, 0f, zRotation);
            rotation = Quaternion.Lerp(initialRotation, randomRotation, t);

            var direction = Vector3.Lerp(initialRotation * Vector3.up, Vector3.up, t);
            var shift = new Vector3(URandom.value - 0.5f, 0, URandom.value - 0.5f);
            pos += (direction + shift) * tree.Scale;

            AddRing(pos, radius, rotation, barkColor);
            ConnectRings();
        }

        var capPos = pos + Vector3.up * tree.Scale;
        AddBranchCap(capPos, barkColor);
        AddFoliage(rotation, capPos);

        PersistMesh();
    }
}