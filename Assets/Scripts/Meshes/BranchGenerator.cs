using System;
using System.Collections.Generic;
using Tree_Generator.Assets.Scripts;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

using URandom = UnityEngine.Random;

public class BranchGenerator : MeshGenerator
{
    //TODO: XML comments
    //TODO: refactoring
    //TODO: magic numbers to constants

    const float BASE_THICKNESS_MULTIPLIER = 1.5f;

    private readonly int expectedHeight;
    private readonly int numSides;
    private readonly float baseRadius;
    private readonly float twisting;
    private readonly float stumpChance;
    private readonly Color barkColor;
    private readonly Color woodColor;

    public BranchGenerator(ProceduralTree tree, Mesh mesh) : base(mesh)
    { 
        expectedHeight = tree.Height;
        numSides = tree.Roundness;
        baseRadius = tree.Thickness;
        
        twisting = tree.Twisting;

        stumpChance = tree.StumpChance;

        barkColor = tree.BarkColor;
        woodColor = tree.WoodColor;   
    }

    private void AddVertex(Vector3 pos, Color color)
    {
        vertices.Add(pos);
        colors.Add(color);
    }

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

    private void AddStumpTop(Vector3 bottomPos, float radius, Quaternion rotation)
    {
        int outerStartIndex = vertices.Count - numSides;
        Assert.IsTrue(outerStartIndex >= 0);

        //Fill the top of the stump
        AddRing(bottomPos, radius * 0.8f, rotation, woodColor);
        int first = vertices.Count - numSides;
        for (int i = first; i < vertices.Count-2; i++) AddTriangle(i+2, i+1, first);

        var direction = rotation * Vector3.up;
        int innerStartIndex = vertices.Count - numSides;

        for (int i = 0; i < numSides; i++)
        {
            int cur = outerStartIndex + i;
            int next = (i < numSides-1) ? cur+1 : outerStartIndex;

            float height = URandom.Range(0.05f, 0.3f);

            int lastIndex = vertices.Count;

            AddVertex(vertices[cur] + height * direction, barkColor);
            AddVertex(vertices[next] + height * direction, barkColor);
            AddVertex(vertices[cur + numSides] + height * direction, barkColor);
            AddVertex(vertices[next + numSides] + height * direction, barkColor);

            //Top
            AddRectangle(lastIndex+3, lastIndex+1, lastIndex, lastIndex+2);
            
            //Connect to inner ring
            AddRectangle(lastIndex+3, lastIndex+2, cur + numSides, next + numSides);

            //Connect to outer ring
            AddRectangle(lastIndex, lastIndex+1, next, cur);
        }
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

        int actualHeight = isStump ? URandom.Range(1, expectedHeight/2) : expectedHeight;
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
            AddBranchCap(pos + rotation * Vector3.up, barkColor);
            //TODO: generate leaves
        }

        PersistMesh();
    }
}