using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using URandom = UnityEngine.Random;

public class TrunkGenerator : TreeGenerator
{
    const float BASE_THICKNESS_MULTIPLIER = 1.5f;
    const float STUMP_INNER_RING_RADIUS = 0.8f;

    private readonly float height;
    private readonly int expectedNumSegments;
    private readonly float baseRadius;
    private readonly float stumpChance;

    private readonly Color woodColor;

    private List<BranchGenerator> branchGenerators = new List<BranchGenerator>();

    public TrunkGenerator(ProceduralTree tree, Mesh mesh) : base(tree, mesh)
    {
        numSides = tree.NumSides;

        height = tree.Height;
        expectedNumSegments = tree.NumSegments;
        baseRadius = tree.Thickness;
        stumpChance = tree.StumpChance;
        woodColor = tree.WoodColor;
    }

    private void AddSplinter(int outerStartIndex, Vector3 direction, int i)
    {
        //Calculate indices of bottom vertices
        int currentOuter = outerStartIndex + i;
        int nextOuter = (i < numSides - 1) ? currentOuter + 1 : outerStartIndex;
        int currentInner = currentOuter + numSides;
        int nextInner = nextOuter + numSides;

        //Randomize splinter height
        float height = URandom.Range(0.05f, 0.3f);
        Vector3 topOffset = height * direction;

        //Create top vertices
        int currentOuterTop = AddVertex(vertices[currentOuter] + topOffset, barkColor);
        int nextOuterTop = AddVertex(vertices[nextOuter] + topOffset, barkColor);
        int currentInnerTop = AddVertex(vertices[currentInner] + topOffset, barkColor);
        int nextInnerTop = AddVertex(vertices[nextInner] + topOffset, barkColor);

        //Top part
        AddRectangle(currentInnerTop, nextInnerTop, nextOuterTop, currentOuterTop);

        //Connect to inner and outer rings
        AddRectangle(nextInnerTop, currentInnerTop, currentInner, nextInner);
        AddRectangle(currentOuterTop, nextOuterTop, nextOuter, currentOuter);

        //Sides
        AddRectangle(currentInnerTop, currentOuterTop, currentOuter, currentInner);
        AddRectangle(nextOuterTop, nextInnerTop, nextInner, nextOuter);
    }

    /// <summary>
    /// Adds the top part of the stump to the mesh.
    /// </summary>
    /// <param name="topPos">Position of the top circle</param>
    /// <param name="radius">Radius of the top circle</param>
    /// <param name="rotation">Rotation of the top circle</param>
    private void AddStumpTop(Vector3 topPos, float radius, Quaternion rotation)
    {
        int outerStartIndex = vertices.Count - numSides;
        AssertHelper.NotNegative(outerStartIndex);

        //Add the inner ring
        AddRing(numSides, topPos, radius * STUMP_INNER_RING_RADIUS, rotation, barkColor);

        var direction = rotation * Vector3.up;
        int innerStartIndex = vertices.Count - numSides;

        for (int i = 0; i < numSides; i++) AddSplinter(outerStartIndex, direction, i);

        //Fill the top of the stump
        AddRing(numSides, topPos, radius * STUMP_INNER_RING_RADIUS, rotation, woodColor);
        int first = vertices.Count - numSides;
        for (int i = first; i < vertices.Count - 2; i++) AddTriangle(i + 2, i + 1, first);
    }

    /// <summary>
    /// Attempts to add a branch generator to the list.
    /// Branches are generated last so as not to mess up vertex order.
    /// A generator is only added if a randomly selected face doesn't face down.
    /// </summary>
    /// <param name="remainingSegments">Remaning number of segments</param>
    private void AddBranchGenerator(float segmentLength, int remainingSegments)
    {
        int ringStartIndex = vertices.Count - 2 * numSides;
        int faceIndex = Random.Range(0, numSides - 1);

        //Find the normal of the face
        int i = ringStartIndex + faceIndex;
        int j = (faceIndex < numSides - 1) ? i + 1 : ringStartIndex;

        Vector3 normal = MeshHelper.Normal(
            vertices[i + numSides],
            vertices[j + numSides],
            vertices[i]
        );

        //Only generate if not facing down
        if (normal.y > 0)
        {
            //Find the center of the face
            Vector3 l = Vector3.Lerp(vertices[i], vertices[i + numSides], 0.5f);
            Vector3 r = Vector3.Lerp(vertices[j], vertices[j + numSides], 0.5f);
            Vector3 center = Vector3.Lerp(l, r, 0.5f);
            float radius = (l - r).magnitude / 2;

            var generator = new BranchGenerator(
                tree, mesh, this, center, normal, radius, segmentLength, remainingSegments
            );
            branchGenerators.Add(generator);
        }
    }

    public override void GenerateMesh()
    {
        //Bottom of the tree
        float bottomRadius = baseRadius * BASE_THICKNESS_MULTIPLIER;
        AddRing(numSides, Vector3.zero, bottomRadius, Quaternion.identity, barkColor);
        for (int i = 0; i < numSides - 2; i++) AddTriangle(0, i + 1, i + 2);

        //Determine whether this tree is a stump
        bool isStump = stumpChance > 0 && URandom.value <= stumpChance;

        //Calculate actual height, which is lower than expected in case of a stump
        int numSegments = isStump ? URandom.Range(2, expectedNumSegments / 2) : expectedNumSegments;

        float segmentLength = height / expectedNumSegments;

        //Save calculated parameters because conifers need them
        var positions = new Vector3[numSegments + 1];
        var rotations = new Quaternion[numSegments];
        var radiuses = new float[numSegments];

        Vector3 pos = Vector3.zero;
        for (int i = 0; i < numSegments; i++)
        {
            // Randomizing the angle
            float xRotation = (URandom.value - 0.5f) * twisting;
            float zRotation = (URandom.value - 0.5f) * twisting;
            var rotation = Quaternion.Euler(xRotation, 0f, zRotation);

            var shift = new Vector3(URandom.value - 0.5f, 0, URandom.value - 0.5f);
            pos += rotation * Vector3.up * segmentLength + shift;

            positions[i] = pos;
            rotations[i] = rotation;
            radiuses[i] = Mathf.Lerp(baseRadius, 0, (float)i / expectedNumSegments);
        }

        for (int i = 0; i < numSegments; i++)
        {
            AddRing(numSides, positions[i], radiuses[i], rotations[i], barkColor);
            ConnectRings(numSides);

            //Attempt to sprout a branch
            if (foliageType == FoliageStyle.Round || foliageType == FoliageStyle.None)
            {
                float trunkPosition = (float)i / expectedNumSegments;
                if (trunkPosition >= 0.33 && trunkPosition <= 0.9)
                {
                    if (URandom.value < 0.5f) AddBranchGenerator(segmentLength, numSegments - i);
                }
            }
        }

        if (isStump) AddStumpTop(pos, radiuses[numSegments - 1], rotations[numSegments - 1]);
        else
        {
            var rotation = rotations[numSegments - 1];
            var capPos = positions[numSegments - 1] + rotation * Vector3.up * segmentLength;
            positions[numSegments] = capPos;
            AddCap(numSides, capPos, barkColor);
            AddFoliage(1, capPos, positions, rotations, radiuses);
        }

        foreach (var g in branchGenerators) g.GenerateMesh();

        PersistMesh();
    }
}