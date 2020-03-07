using System.Collections.Generic;
using UnityEngine;

namespace Tree_Generator.Assets.Scripts
{
    public class LeavesGenerator : MonoBehaviour, IMeshGenerator
    {
        const float MAX_SHIFT_MAGNITUDE = 0.4f;

        private ProceduralTree tree;
        private Mesh mesh;

        private Vector3[] vertices;
        private int[] triangles;

        public LeavesGenerator(Mesh mesh)
        { 
            this.mesh = mesh;
        }

        public void GenerateMesh()
        {
            var icosahedron = PrimitiveFactory.CreateIcosahedron();

            for(int i = 0; i < icosahedron.Vertices.Length; i++)
            {
                var oldVertex = icosahedron.Vertices[i];
                float magnitude = (Random.value * 2 - 1) * MAX_SHIFT_MAGNITUDE;
                var shift = Random.insideUnitSphere * magnitude;
                
                icosahedron.Vertices[i] = oldVertex + shift;
            }

            mesh.vertices = icosahedron.Vertices;
            mesh.triangles = icosahedron.Triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            FinishMesh();
        }

        public void FinishMesh()
        {
            
        }
    }
}