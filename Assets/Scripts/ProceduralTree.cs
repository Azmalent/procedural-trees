using Tree_Generator.Assets.Scripts;
using UnityEngine;

public class ProceduralTree : MonoBehaviour 
{
	private Mesh mesh;
	private AbstractMeshGenerator generator;

	private void Awake() 
	{
		mesh = GetComponent<MeshFilter>().mesh;
		generator = new LeavesGenerator(mesh);
	}

	void Start () 
	{
		mesh.Clear();
		generator.Generate();
		mesh.RecalculateNormals();
	}

	void Update () 
	{
		
	}
}
