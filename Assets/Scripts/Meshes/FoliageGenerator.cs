using UnityEngine;
using UnityEngine.Assertions;
using URandom = UnityEngine.Random;

public abstract class FoliageGenerator : MeshGenerator
{
    protected Color color;

    public FoliageGenerator(ProceduralTree tree, Mesh mesh) : base(mesh)
    {
        if (tree.FoliageStyle == ProceduralFoliageStyle.None) return;

        var colors = tree.FoliageColors;
        int numColors = colors.Length;

        if (numColors > 0)
        {
            int colorIndex = URandom.Range(0, numColors);
            color = colors[colorIndex];
        }
        else color = Palette.GREEN;
    }
}