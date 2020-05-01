using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using URandom = UnityEngine.Random;

public class TrunkGenerator : TreeGenerator
{
    const float BASE_THICKNESS_MULTIPLIER = 1.5f;
    const float STUMP_INNER_RING_RADIUS = 0.8f;

    private readonly int expectedHeight;
    private readonly float baseRadius;
    private readonly float stumpChance;

    private readonly Color woodColor;

    private List<BranchGenerator> branchGenerators = new List<BranchGenerator>();

    public TrunkGenerator(ProceduralTree tree, Mesh mesh) : base(tree, mesh)
    {
        expectedHeight = tree.Height;
        baseRadius = tree.Thickness * tree.Scale;
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
        float height = URandom.Range(0.05f, 0.3f) * tree.Scale;
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
        AddRing(topPos, radius * STUMP_INNER_RING_RADIUS, rotation, barkColor);

        var direction = rotation * Vector3.up;
        int innerStartIndex = vertices.Count - numSides;

        for (int i = 0; i < numSides; i++) AddSplinter(outerStartIndex, direction, i);

        //Fill the top of the stump
        AddRing(topPos, radius * STUMP_INNER_RING_RADIUS, rotation, woodColor);
        int first = vertices.Count - numSides;
        for (int i = first; i < vertices.Count - 2; i++) AddTriangle(i + 2, i + 1, first);
    }

    /// <summary>
    /// Attempts to add a branch generator to the list.
    /// Branches are generated last so as not to mess up vertex order.
    /// A generator is only added if a randomly selected face doesn't face down.
    /// </summary>
    /// <param name="position">The position of the branch</param>
    /// <param name="baseRotation">The rotation</param>
    private void AddBranchGenerator()
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

            var generator = new BranchGenerator(tree, mesh, this, center, normal, radius);
            branchGenerators.Add(generator);
        }
    }

    public override void GenerateMesh()
    {
        float radius = baseRadius;
        var rotation = Quaternion.identity;
        Vector3 pos = Vector3.zero;

        //Bottom of the tree
        AddRing(pos, baseRadius * BASE_THICKNESS_MULTIPLIER, rotation, barkColor);
        for (int i = 0; i < numSides - 2; i++) AddTriangle(0, i + 1, i + 2);

        //Determine whether this tree is a stump
        bool isStump = stumpChance > 0 && URandom.value <= stumpChance;

        //Calculate actual height, which is lower than expected in case of a stump
        int actualHeight = isStump ? URandom.Range(2, expectedHeight / 2) : expectedHeight;

        for (int i = 0; i < actualHeight; i++)
        {
            radius = Mathf.Lerp(baseRadius, 0, (float)i / expectedHeight);

            // Randomizing the branch angle
            float xRotation = (URandom.value - 0.5f) * twisting;
            float zRotation = (URandom.value - 0.5f) * twisting;
            rotation = Quaternion.Euler(xRotation, 0f, zRotation);

            var shift = new Vector3(URandom.value - 0.5f, 0, URandom.value - 0.5f);
            pos += (rotation * Vector3.up + shift) * tree.Scale;

            AddRing(pos, radius, rotation, barkColor);
            ConnectRings();

            //Branches will only attempt to b
            float trunkPosition = (float)i / expectedHeight;
            if (trunkPosition >= 0.5 && trunkPosition <= 0.8)
            {
                if (URandom.value < 0.5f) AddBranchGenerator();
            }
        }

        if (isStump) AddStumpTop(pos, radius, rotation);
        else
        {
            var capPos = pos + rotation * Vector3.up;
            AddBranchCap(capPos, barkColor);
            AddFoliage(rotation, capPos);
        }

        foreach (var g in branchGenerators) g.GenerateMesh();

        PersistMesh();
    }
}