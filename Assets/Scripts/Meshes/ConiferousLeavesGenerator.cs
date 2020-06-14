using UnityEngine;
using UnityEngine.Assertions;

public class ConiferousLeavesGenerator : FoliageGenerator
{
    private Vector3[] positions;
    private Quaternion[] rotations;
    private float[] radiuses;

    public ConiferousLeavesGenerator(ProceduralTree tree, Mesh mesh, float scale, Vector3[] positions, Quaternion[] rotations, float[] radiuses) : base(tree, mesh, scale)
    {
        this.positions = positions;
        this.rotations = rotations;
        this.radiuses = radiuses;
    }

    void ShiftRingVertices(int numSides)
    {
        int startIndex = vertices.Count - numSides;
        for (int i = 0; i < numSides; i++)
        {
            int topIndex = startIndex + i;
            int bottomIndex = topIndex - numSides;
            var shift = (vertices[bottomIndex] - vertices[topIndex]) * 0.1f;
            vertices[bottomIndex] += shift;

            if (i % 2 == 0) vertices[bottomIndex] += shift;
        }
    }

    private void AddLayer(int numSides, Vector3 pos,
        Quaternion previousRotation, Quaternion rotation,
        float foliageRadius, float trunkRadius,
        bool small)
    {
        Assert.AreEqual(numSides % 2, 0);
        float smallRadius = trunkRadius + foliageRadius / (small ? 4 : 2);
        float bigRadius = trunkRadius + foliageRadius;

        AddRing(numSides, pos, smallRadius, previousRotation, color);
        ConnectRings(numSides);
        ShiftRingVertices(numSides);

        AddRing(numSides, pos, bigRadius, rotation, color);
        ConnectRings(numSides);
    }

    public override void GenerateMesh()
    {
        int NUM_SIDES = 6;
        float SMALL_FIRST_LAYER_CHANCE = 0.5f;

        int startIndex = 2;

        float r = radiuses[startIndex] + width;
        bool smallFirstLayer = Random.value < SMALL_FIRST_LAYER_CHANCE;
        if (smallFirstLayer) r -= width / 2;

        Quaternion previousRotation = rotations[startIndex];
        AddRing(NUM_SIDES, positions[startIndex], r, previousRotation, color);
        for (int i = 0; i < NUM_SIDES - 2; i++) AddTriangle(0, i + 1, i + 2);

        int numLayers = radiuses.Length;
        for (int i = startIndex + 1; i < numLayers; i++)
        {
            float foliageRadius = Mathf.Lerp(width, 0, (i - startIndex - 1) / (float)numLayers);
            float radius = radiuses[i];
            Vector3 pos = positions[i];
            Quaternion rotation = rotations[i];

            bool isSmall = i == (startIndex + 1) && smallFirstLayer;
            AddLayer(NUM_SIDES, pos, previousRotation, rotation, foliageRadius, radius, isSmall);
            previousRotation = rotation;
        }

        AddCap(NUM_SIDES, positions[positions.Length - 1], color);

        PersistMesh();
    }
}