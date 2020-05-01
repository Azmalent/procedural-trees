using UnityEngine;
using UnityEngine.Assertions;
using URandom = UnityEngine.Random;

public abstract class FoliageGenerator : MeshGenerator
{
    const float MIN_SIZE = 0.5f;

    protected Color color;
    protected float width;
    protected float height;

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

        width = tree.FoliageWidth + Random.Range(-1, 1) * tree.FoliageWidthVariance;
        width = Mathf.Max(MIN_SIZE, width) * tree.Scale;

        height = tree.FoliageHeight + Random.Range(-1, 1) * tree.FoliageHeightVariance;
        height = Mathf.Max(MIN_SIZE, height) * tree.Scale;
    }
}