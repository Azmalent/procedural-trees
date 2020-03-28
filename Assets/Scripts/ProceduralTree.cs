using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralTree : MonoBehaviour 
{
    public enum TrunkType
    {
        Smooth, Notched
    }

	private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
	private MeshGenerator generator;

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

    [Range(1f, 10f)]
    public float FoliageSize = 3f;

    public Color BarkColor = Palette.BARK_BROWN;

    public Color WoodColor = Palette.WOOD_BROWN;

    public Color[] FoliageColors = new Color[] { Palette.GREEN };

    public FoliageType FoliageStyle = FoliageType.Round;

	private void Awake() 
	{
		meshFilter = GetComponent<MeshFilter>();
		meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = new Material(Resources.Load<Shader>("Shaders/VertexColor"));

		generator = new BranchGenerator(this, meshFilter.mesh);
	}

	void Start () 
	{
        generator.GenerateMesh();
	}

	void Update () 
	{
		
	}
}
