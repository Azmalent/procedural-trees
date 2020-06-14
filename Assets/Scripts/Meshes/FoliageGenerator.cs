using UnityEngine;
using URandom = UnityEngine.Random;

public abstract class FoliageGenerator : MeshGenerator
{
    const float MIN_SIZE = 0.5f;

    protected readonly Color color;
    protected readonly float width;
    protected readonly float height;

    public FoliageGenerator(ProceduralTree tree, Mesh mesh, float scale) : base(mesh)
    {
        if (tree.FoliageStyle == FoliageStyle.None) return;

        var colors = tree.FoliageColors;
        int numColors = colors.Length;

        if (numColors > 0)
        {
            int colorIndex = URandom.Range(0, numColors);
            color = colors[colorIndex];
        }
        else color = Palette.GREEN;

        width = tree.FoliageWidth + Random.Range(-1, 1) * tree.FoliageWidthVariance;
        width = Mathf.Max(MIN_SIZE, width * scale);

        height = tree.FoliageHeight + Random.Range(-1, 1) * tree.FoliageHeightVariance;
        height = Mathf.Max(MIN_SIZE, height * scale);
    }

    /// <summary>
    /// Finds the vertex in the middle of a specified segment. 
    /// If this vertex does not exist, it is added. 
    /// </summary>
    /// <param name="aIndex">Index of first vertex</param>
    /// <param name="bIndex">Index of second vertex</param>
    /// <returns>Index of the vertex between vertices A and B</returns>
    protected int FindOrCreateMiddle(int aIndex, int bIndex, bool normalized = false)
    {
        Vector3 a = vertices[aIndex];
        Vector3 b = vertices[bIndex];
        Vector3 middle = Vector3.Lerp(a, b, 0.5f);
        if (normalized) middle = middle.normalized;

        int index = vertices.IndexOf(middle);
        if (index > -1) return index;

        return AddVertex(middle, color);
    }
}