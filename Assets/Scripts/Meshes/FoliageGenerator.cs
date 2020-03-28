using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

using URandom = UnityEngine.Random;

public abstract class FoliageGenerator : MeshGenerator
{
    protected Color color;
    
    public FoliageGenerator(ProceduralTree tree, Mesh mesh) : base(mesh) 
    {
        int numColors = tree.FoliageColors.Length;
        if (numColors > 0)
        {
            int colorIndex = URandom.Range(0, numColors);
            color = tree.FoliageColors[colorIndex];
        }
        else color = Palette.GREEN;
    }
}