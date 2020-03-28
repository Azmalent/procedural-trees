using System;
using UnityEngine;
using UnityEngine.Assertions;

using URandom = UnityEngine.Random;

public class BranchGenerator : MeshGenerator
{
    //TODO: XML comments
    //TODO: refactoring
    //TODO: magic numbers to constants

    const float BASE_THICKNESS_MULTIPLIER = 1.5f;

    private readonly ProceduralTree tree;
    private readonly int expectedHeight;
    private readonly int numSides;
    private readonly float baseRadius;
    private readonly float twisting;
    private readonly float stumpChance;

    private readonly Color barkColor;
    private readonly Color woodColor;
    private readonly FoliageType foliageType;

    public BranchGenerator(ProceduralTree tree, Mesh mesh) : base(mesh)
    { 
        this.tree = tree;
        expectedHeight = tree.Height;
        numSides = tree.Roundness;
        baseRadius = tree.Thickness;
        
        twisting = tree.Twisting;

        stumpChance = tree.StumpChance;

        barkColor = tree.BarkColor;
        woodColor = tree.WoodColor;   
        foliageType = tree.FoliageStyle;
    }

    /// <summary>
    /// Adds a ring of vertices to the mesh.
    /// </summary>
    /// <param name="centerPos">Position of the center point.</param>
    /// <param name="radius">Radius of the ring.</param>
    /// <param name="rotation">Rotation of the ring.</param>
    /// <param name="color">Color of the vertices.</param>
    private void AddRing(Vector3 centerPos, float radius, Quaternion rotation, Color color)
    {
        double angle = 2*Math.PI / numSides; 
        for (int i = 0; i < numSides; i++)
        {
            float x = (float) Math.Cos(angle * i);
            float z = (float) Math.Sin(angle * i);
            var offset = radius * new Vector3(x, 0, z);
            var pos = centerPos + rotation * offset;
            AddVertex(pos, color);
        }
    }

    /// <summary>
    /// Caps the branch with a new vertex, connecting it to the last added ring.
    /// </summary>
    /// <param name="pos">The position of the new vertex.</param>
    private void AddBranchCap(Vector3 pos, Color color)
    {
        int startIndex = vertices.Count - numSides;
        Assert.IsTrue(startIndex >= 0);

        AddVertex(pos, color);
        for (int i = startIndex; i < vertices.Count-2; i++) AddTriangle(i, vertices.Count-1, i+1);

        AddTriangle(vertices.Count-2, vertices.Count-1, startIndex);
    }

    /// <summary>
    /// Connects two last added rings with triangles.
    /// </summary>
    private void ConnectRings()
    {
        int startIndex = vertices.Count - 2*numSides;
        Assert.IsTrue(startIndex >= 0);

        for (int i = 0; i < numSides; i++)
        {
            int cur = startIndex + i;
            int next = (i < numSides-1) ? cur+1 : startIndex;
            AddRectangle(cur+numSides, next+numSides, next, cur);
        }
    }

    private void AddSplinter(int outerStartIndex, Vector3 direction, int i)
    {
        //Calculate indices of bottom vertices
        int currentOuter = outerStartIndex + i;
        int nextOuter = (i < numSides-1) ? currentOuter+1 : outerStartIndex;
        int currentInner = currentOuter + numSides;
        int nextInner = nextOuter + numSides;

        //Randomize splinter height
        float height = URandom.Range(0.05f, 0.3f);
        Vector3 topOffset = height * direction;

        //Create top vertices
        int currentOuterTop = AddVertex(vertices[currentOuter] + topOffset, barkColor);
        int nextOuterTop    = AddVertex(vertices[nextOuter]    + topOffset, barkColor);        
        int currentInnerTop = AddVertex(vertices[currentInner] + topOffset, barkColor);
        int nextInnerTop    = AddVertex(vertices[nextInner]    + topOffset, barkColor);
        
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
        AddRing(topPos, radius * 0.8f, rotation, barkColor);

        var direction = rotation * Vector3.up;
        int innerStartIndex = vertices.Count - numSides;

        for (int i = 0; i < numSides; i++) AddSplinter(outerStartIndex, direction, i);

        //Fill the top of the stump
        AddRing(topPos, radius * 0.8f, rotation, woodColor);
        int first = vertices.Count - numSides;
        for (int i = first; i < vertices.Count-2; i++) AddTriangle(i+2, i+1, first);
    }

    /// <summary>
    /// Adds foliage to the end of the branch as a new GameObject.
    /// </summary>
    /// <param name="rotation">Rotation of the foliage</param>
    /// <param name="pos">Position of the foliage</param>
    private void AddFoliage(Quaternion rotation, Vector3 pos)
    {
        if (foliageType == FoliageType.None) return;
        var leaves = new GameObject();
        leaves.transform.parent = tree.gameObject.transform;
        leaves.transform.localPosition = pos;

        var filter = leaves.AddComponent<MeshFilter>();
        var renderer = leaves.AddComponent<MeshRenderer>();
        renderer.material = new Material(Resources.Load<Shader>("Shaders/VertexColor"));

        FoliageGenerator generator;
        switch (foliageType)
        {
            case FoliageType.Round:
                generator = new RoundLeavesGenerator(tree, filter.mesh);
                break;

            case FoliageType.Flat:
                generator = new FlatLeavesGenerator(tree, filter.mesh);
                break;

            default: 
                throw new ArgumentException("Unsupported foliage type");
        }

        generator.GenerateMesh();
    }

    public override void GenerateMesh()
    {
        float radius = baseRadius;
        var rotation = Quaternion.identity;
        Vector3 pos = Vector3.zero;

        //Bottom of the tree
        AddRing(pos, baseRadius * BASE_THICKNESS_MULTIPLIER, rotation, barkColor);
        for (int i = 0; i < numSides-2; i++) AddTriangle(0, i+1, i+2);

        //Determine whether this tree is a stump
        bool isStump = stumpChance > 0 && URandom.value <= stumpChance; 

        //Calculate actual height, which is lower than expected in case of a stump
        int actualHeight = isStump ? URandom.Range(2, expectedHeight/2) : expectedHeight;
        
        for (int i = 0; i < actualHeight; i++)
        {
            radius = Mathf.Lerp(baseRadius, 0, (float) i / expectedHeight);

            // Randomizing the branch angle
            float xRotation = (URandom.value - 0.5f) * twisting;
            float zRotation = (URandom.value - 0.5f) * twisting;
            rotation *= Quaternion.Euler(xRotation, 0f, zRotation);

            float xShift = URandom.value - 0.5f;
            float zShift = URandom.value - 0.5f;
            pos += rotation * Vector3.up + new Vector3(xShift, 0, zShift);

            AddRing(pos, radius, rotation, barkColor);
            ConnectRings();
        }

        if (isStump) AddStumpTop(pos, radius, rotation);
        else 
        {
            var capPos = pos + rotation * Vector3.up;
            AddBranchCap(capPos, barkColor);
            AddFoliage(rotation, capPos);
        }

        PersistMesh();
    }

   
}