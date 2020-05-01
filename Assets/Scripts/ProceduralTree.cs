using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralTree : MonoBehaviour
{
    public const float MIN_FOLIAGE_SIZE = 1f;
    public const float MAX_FOLIAGE_SIZE = 10f;
    public const float MAX_FOLIAGE_VARIANCE = 5f;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshGenerator generator;

    [Range(0.1f, 10)]
    public float Scale;

    public float Units(float n)
    {
        return n * Scale;
    }

    [Header("Trunk Options")]
    /// <summary>
    /// The material assigned to the trunk.
    /// </summary>
    public Material TrunkMaterial;

    /// <summary>
    /// The amount of segments of the tree.
    /// </summary>
    [Range(5, 25)]
    public int Height = 10;

    /// <summary>
    /// The amount of sides of the tree.
    /// </summary>
    [Range(4, 12)]
    public int Roundness = 8;

    /// <summary>
    /// Radius at the base of the tree.
    /// </summary>
    [Range(0.25f, 4f)]
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
    public Material FoliageMaterial;

    public ProceduralFoliageStyle FoliageStyle = ProceduralFoliageStyle.Round;

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

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = TrunkMaterial;

        generator = new TrunkGenerator(this, meshFilter.mesh);
    }

    void Start()
    {
        generator.GenerateMesh();
    }
}
