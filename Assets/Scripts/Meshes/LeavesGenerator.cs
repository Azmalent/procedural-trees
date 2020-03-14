using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Tree_Generator.Assets.Scripts
{
    public class LeavesGenerator : MonoBehaviour, IMeshGenerator
    {
        const float MAX_SHIFT_MAGNITUDE = 0.4f;

        private ProceduralTree tree;
        private Mesh mesh;

        private List<Vector3> vertices;
        private List<int> triangles;
        private List<Color> colors;

        public LeavesGenerator(Mesh mesh)
        { 
            this.mesh = mesh;

            vertices = new List<Vector3>();
            triangles = new List<int>();
        }


        /// <summary>
        /// Adds a triangle to the list.
        /// </summary>
        private void AddTriangle(int a, int b, int c)
        {
            Assert.IsFalse(a < 0);
            Assert.IsFalse(b < 0);
            Assert.IsFalse(c < 0);

            triangles.Add(a);
            triangles.Add(b);
            triangles.Add(c);
        }

        private void GenerateIcosahedron(float radius)
        {    
            // create 12 vertices of a icosahedron
            float t = (1f + Mathf.Sqrt(5f)) / 2f;
    
            vertices.Add(new Vector3(-1f,  t,  0f).normalized * radius);
            vertices.Add(new Vector3( 1f,  t,  0f).normalized * radius);
            vertices.Add(new Vector3(-1f, -t,  0f).normalized * radius);
            vertices.Add(new Vector3( 1f, -t,  0f).normalized * radius);
    
            vertices.Add(new Vector3( 0f, -1f,  t).normalized * radius);
            vertices.Add(new Vector3( 0f,  1f,  t).normalized * radius);
            vertices.Add(new Vector3( 0f, -1f, -t).normalized * radius);
            vertices.Add(new Vector3( 0f,  1f, -t).normalized * radius);
    
            vertices.Add(new Vector3( t,  0f, -1f).normalized * radius);
            vertices.Add(new Vector3( t,  0f,  1f).normalized * radius);
            vertices.Add(new Vector3(-t,  0f, -1f).normalized * radius);
            vertices.Add(new Vector3(-t,  0f,  1f).normalized * radius);
    
            // 5 faces around point 0
            AddTriangle(0, 11, 5);
            AddTriangle(0, 5, 1);
            AddTriangle(0, 1, 7);
            AddTriangle(0, 7, 10);
            AddTriangle(0, 10, 11);
    
            // 5 adjacent faces 
            AddTriangle(1, 5, 9);
            AddTriangle(5, 11, 4);
            AddTriangle(11, 10, 2);
            AddTriangle(10, 7, 6);
            AddTriangle(7, 1, 8);
    
            // 5 faces around point 3
            AddTriangle(3, 9, 4);
            AddTriangle(3, 4, 2);
            AddTriangle(3, 2, 6);
            AddTriangle(3, 6, 8);
            AddTriangle(3, 8, 9);
    
            // 5 adjacent faces 
            AddTriangle(4, 9, 5);
            AddTriangle(2, 4, 11);
            AddTriangle(6, 2, 10);
            AddTriangle(8, 6, 7);
            AddTriangle(9, 8, 1);
        }

        public void GenerateMesh()
        {
            GenerateIcosahedron(1f);

            for(int i = 0; i < vertices.Count; i++)
            {
                var oldVertex = vertices[i];
                float magnitude = (Random.value * 2 - 1) * MAX_SHIFT_MAGNITUDE;
                var shift = Random.insideUnitSphere * magnitude;
                
                vertices[i] = oldVertex + shift;
            }

            PersistMesh();
        }

        public void PersistMesh()
        {
            mesh.Clear();

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            MeshUtility.Optimize(mesh);
        }
    }
}