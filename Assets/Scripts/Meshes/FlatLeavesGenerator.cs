using UnityEngine;

public class FlatLeavesGenerator : FoliageGenerator
{
    private const float MIN_NOTCH_DEPTH = 0.25f;
    private const float MAX_NOTCH_DEPTH = 0.5f;
    private const float NOTCH_WIDTH = 0.2f;

    public FlatLeavesGenerator(ProceduralTree tree, Mesh mesh, float scale) : base(tree, mesh, scale)
    {
    }

    private void AddTwoSidedTriangle(int a, int b, int c)
    {
        triangles.Add(a);
        triangles.Add(b);
        triangles.Add(c);

        triangles.Add(c);
        triangles.Add(b);
        triangles.Add(a);
    }

    private void AddTwoSidedRectangle(int a, int b, int c, int d)
    {
        AssertHelper.NotNegative(a, b, c, d);
        AddTwoSidedTriangle(a, b, c);
        AddTwoSidedTriangle(a, c, d);
    }

    private void AddTwoSidedNotchedRectangle(int a, int b, int c, int d)
    {
        //Calculate the position of the notch on the rectangle
        float notchCenterPoint = Random.Range(NOTCH_WIDTH, 1 - NOTCH_WIDTH);

        var notchCenterPos = Vector3.Lerp(vertices[a], vertices[d], notchCenterPoint);
        var notchTopPos = Vector3.Lerp(vertices[a], vertices[d], notchCenterPoint - NOTCH_WIDTH / 2);
        var notchBottomPos = Vector3.Lerp(vertices[a], vertices[d], notchCenterPoint + NOTCH_WIDTH / 2);

        //Select random depth for the notch
        float depth = Random.Range(MIN_NOTCH_DEPTH, MAX_NOTCH_DEPTH);

        var oppositePos = Vector3.Lerp(vertices[b], vertices[c], notchCenterPoint);
        var notchEndPos = Vector3.Lerp(notchCenterPos, oppositePos, depth);
        notchEndPos += (vertices[d] - vertices[a]) * Random.Range(-NOTCH_WIDTH, NOTCH_WIDTH);

        int notchTop = AddVertex(notchTopPos, color);
        int notchBottom = AddVertex(notchBottomPos, color);
        int notchInner = AddVertex(notchEndPos, color);

        AddTwoSidedTriangle(a, notchInner, notchTop);
        AddTwoSidedTriangle(a, b, notchInner);
        AddTwoSidedTriangle(b, c, notchInner);
        AddTwoSidedTriangle(c, d, notchInner);
        AddTwoSidedTriangle(d, notchBottom, notchInner);
    }

    private void AddRectangleSegment(int a, int b, int c, int d)
    {
        float notchChance = 0.3f;

        bool leftNotch = Random.value < notchChance;
        bool rightNotch = Random.value < notchChance;

        if (!leftNotch && !rightNotch) AddTwoSidedRectangle(a, b, c, d);
        else if (leftNotch && !rightNotch) AddTwoSidedNotchedRectangle(a, b, c, d);
        else if (!leftNotch && rightNotch) AddTwoSidedNotchedRectangle(c, d, a, b);
        else
        {
            var abMiddle = FindOrCreateMiddle(a, b);
            var cdMiddle = FindOrCreateMiddle(c, d);

            AddTwoSidedNotchedRectangle(a, abMiddle, cdMiddle, d);
            AddTwoSidedNotchedRectangle(c, cdMiddle, abMiddle, b);
        }
    }

    private void GenerateLeaf(Quaternion rotation)
    {
        float length = 15f;
        int segmentCount = 5;   //TODO

        var start = Vector3.zero;
        var end = rotation * (Vector3.right * length);
        var topPoint = start + (end - start) / 2 + Vector3.up * height;

        int midpoint = segmentCount / 2;

        int startIndex = AddVertex(start, color);
        int previousLeftIndex = 0, previousRightIndex = 0;

        for (int i = 0; i < segmentCount - 1; i++)
        {
            float t = (i + 1) / (float)segmentCount;
            float segmentWidth = Mathf.Lerp(width, 0, Mathf.Abs(t - 0.5f) * 2);

            var center = Bezier.Quadratic(t, start, topPoint, end);
            var left = center + rotation * (Vector3.back * segmentWidth / 2);
            var right = center + rotation * (Vector3.forward * segmentWidth / 2);

            int leftIndex = AddVertex(left, color);
            int rightIndex = AddVertex(right, color);

            if (i == 0) AddTwoSidedTriangle(leftIndex, rightIndex, startIndex);
            else AddRectangleSegment(previousLeftIndex, previousRightIndex, rightIndex, leftIndex);

            previousLeftIndex = leftIndex;
            previousRightIndex = rightIndex;
        }

        int endIndex = AddVertex(end, color);
        AddTwoSidedTriangle(previousLeftIndex, previousRightIndex, endIndex);
    }

    public override void GenerateMesh()
    {
        int numLeaves = 5; //TODO
        float angle = 360f / numLeaves;
        for (int i = 0; i < numLeaves; i++)
        {
            var q = Quaternion.Euler(0, angle * i, 0);
            GenerateLeaf(q);
        }

        PersistMesh();
    }
}