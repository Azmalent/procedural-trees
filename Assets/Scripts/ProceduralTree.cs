#define ENABLE_PROFILER

using UnityEngine;
using UnityEngine.Profiling;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralTree : MonoBehaviour
{
    public const int MIN_ROUNDNESS = 4, MAX_ROUNDNESS = 8;
    public const int MIN_SEGMENTS = 3, MAX_SEGMENTS = 8;
    public const float MIN_THICKNESS = 0.25f, MAX_THICKNESS = 4f;
    public const float MIN_HEIGHT = 5f, MAX_HEIGHT = 25f;
    public const float MIN_FOLIAGE_SIZE = 1f, MAX_FOLIAGE_SIZE = 10f;
    public const float MAX_FOLIAGE_VARIANCE = 5f;

    private const int GIZMO_LINES_PER_SIDE = 3;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private TrunkGenerator generator;

    [Header("Trunk Options")]
    /// <summary>
    /// The material assigned to the trunk.
    /// </summary>
    public Material[] TrunkMaterials;


    /// <summary>
    /// The height of the tree.
    /// </summary>
    [Range(MIN_HEIGHT, MAX_HEIGHT)]
    public float Height = 10f;

    /// <summary>
    /// Radius at the base of the tree.
    /// </summary>
    [Range(MIN_THICKNESS, MAX_THICKNESS)]
    public float Thickness = 1f;

    /// <summary>
    /// Low values make branches straighter.
    /// </summary>
    [Range(0f, 40f)]
    public float Twisting = 8;

    /// <summary>
    /// The chance a generated tree will be broken.
    /// </summary>
    [Range(0f, 1f)]
    public float StumpChance = 0.1f;

    /// <summary>
    /// The colot of the bark.
    /// </summary>
    public Color BarkColor = Palette.BARK_BROWN;

    /// <summary>
    /// The color of the inside of the tree, visible when a stump is generated.
    /// </summary>
    public Color WoodColor = Palette.WOOD_BROWN;

    [Header("Foliage Options")]
    public Material[] FoliageMaterials;

    public FoliageStyle FoliageStyle = FoliageStyle.Round;

    [HideInInspector]
    public Color[] FoliageColors = new Color[] { Palette.GREEN };

    [HideInInspector, Range(MIN_FOLIAGE_SIZE, MAX_FOLIAGE_SIZE)]
    public float FoliageHeight = 3f;

    [HideInInspector, Range(0, MAX_FOLIAGE_VARIANCE)]
    public float FoliageHeightVariance = 1f;

    [HideInInspector, Range(MIN_FOLIAGE_SIZE, MAX_FOLIAGE_SIZE)]
    public float FoliageWidth = 3f;

    [HideInInspector, Range(0, MAX_FOLIAGE_VARIANCE)]
    public float FoliageWidthVariance = 3f;

    public int NumSides
    {
        get
        {
            float t = Mathf.InverseLerp(MIN_THICKNESS, MAX_THICKNESS, Thickness);
            return (int)Mathf.Floor(Mathf.Lerp(MIN_ROUNDNESS, MAX_ROUNDNESS, t));
        }
    }

    public int NumSegments
    {
        get
        {
            float t = Mathf.InverseLerp(MIN_HEIGHT, MAX_HEIGHT, Height);
            return (int)Mathf.Floor(Mathf.Lerp(MIN_SEGMENTS, MAX_SEGMENTS, t));
        }
    }

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.materials = TrunkMaterials;

        generator = new TrunkGenerator(this, meshFilter.mesh);
    }

    void Start()
    {
        Profiler.BeginSample("Tree Generation", gameObject);
        generator.GenerateMesh();
        Profiler.EndSample();

        var meshCollider = GetComponent<MeshCollider>();
        if (meshCollider != null) meshCollider.sharedMesh = meshFilter.mesh;
    }

    void OnDrawGizmos()
    {
        if (meshFilter != null) return;

        Gizmos.color = BarkColor;

        Vector3 top = transform.position + Height * Vector3.up;
        int numLines = NumSides * GIZMO_LINES_PER_SIDE;
        float angle = 2 * Mathf.PI / numLines;

        for (int i = 0; i < numLines; i++)
        {
            float x = (float)Mathf.Cos(angle * i);
            float z = (float)Mathf.Sin(angle * i);
            var offset = Thickness * new Vector3(x, 0, z);
            Gizmos.DrawLine(transform.position + offset, top);
        }
    }
}
