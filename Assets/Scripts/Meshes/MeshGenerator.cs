using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Tree_Generator.Assets.Scripts
{
    public abstract class MeshGenerator : MonoBehaviour
    {
        protected readonly Mesh mesh;

        protected readonly List<Vector3> vertices = new List<Vector3>();
        protected readonly List<int> triangles = new List<int>();
        protected readonly List<Color> colors = new List<Color>();

        protected MeshGenerator(Mesh mesh)
        {
            this.mesh = mesh;
        }

        /// <summary>
        /// Adds a triangle to the list.
        /// </summary>
        protected void AddTriangle(int a, int b, int c)
        {
            Assert.IsFalse(a < 0);
            Assert.IsFalse(b < 0);
            Assert.IsFalse(c < 0);

            triangles.Add(a);
            triangles.Add(b);
            triangles.Add(c);
        }

        protected void AddRectangle(int a, int b, int c, int d)
        {
            Assert.IsFalse(a < 0);
            Assert.IsFalse(b < 0);
            Assert.IsFalse(c < 0);

            triangles.Add(a);
            triangles.Add(b);
            triangles.Add(c);

            triangles.Add(a);
            triangles.Add(c);
            triangles.Add(d);
        }

        public abstract void GenerateMesh();

        protected void PersistMesh()
        {
            mesh.Clear();

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.colors = colors.ToArray();

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            MeshUtility.Optimize(mesh);
        }
    }
}