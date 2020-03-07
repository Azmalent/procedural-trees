using Tree_Generator.Assets.Scripts;
using UnityEditor;
using UnityEngine;

public class ProceduralTree : MonoBehaviour 
{
	private MeshFilter meshFilter;
	private BranchGenerator generator;

    /// <summary>
    /// The amount of segments of the tree.
    /// </summary>
    [Range(0, 20)]
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
    public float BaseRadius = 1f;
    
    /// <summary>
    /// Radius at the base of the tree.
    /// </summary>
    [Range(0f, 40f)]
    public float Twisting = 8;

	private void Awake() 
	{
		meshFilter = GetComponent<MeshFilter>();
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
