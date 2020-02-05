using UnityEngine;

namespace Tree_Generator.Assets.Scripts
{
    public class LeavesGenerator : AbstractMeshGenerator
    {
        const float MAX_SHIFT_MAGNITUDE = 0.4f;

        private Mesh mesh;
        public LeavesGenerator(Mesh mesh)
        {
            this.mesh = mesh;
        }

        public override void Generate()
        {
            var icosahedron = PrimitiveFactory.CreateIcosahedron();

            var rng = new System.Random();
            for(int i = 0; i < icosahedron.Vertices.Length; i++)
            {
                var oldVertex = icosahedron.Vertices[i];
                float magnitude = ((float) rng.NextDouble() * 2 - 1) * MAX_SHIFT_MAGNITUDE;
                var shift = Random.insideUnitSphere * magnitude;
                
                icosahedron.Vertices[i] = oldVertex + shift;
            }

            mesh.vertices = icosahedron.Vertices;
            mesh.triangles = icosahedron.Triangles;
            mesh.RecalculateNormals();
        }
    }
}