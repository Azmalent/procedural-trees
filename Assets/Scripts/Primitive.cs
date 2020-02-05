using UnityEngine;

namespace Tree_Generator.Assets.Scripts
{
    public struct Primitive
    {
        public readonly Vector3[] Vertices;
        public readonly int[] Triangles;

        public Primitive(Vector3[] vertices, int[] triangles)
        {
            Vertices = vertices;
            Triangles = triangles;
        }
    }
}