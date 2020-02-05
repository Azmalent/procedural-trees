using System;
using UnityEngine;

namespace Tree_Generator.Assets.Scripts
{
    public static class PrimitiveFactory
    {
        public static Primitive CreateIcosahedron()
        {
            const float X = 0.525731112119133606f;
            const float Z = 0.850650808352039932f;
            
            var vertices = new Vector3[] {
                new Vector3(-X, 0, Z), 
                new Vector3(X, 0, Z), 
                new Vector3(-X, 0, -Z), 
                new Vector3(X, 0, -Z),
                new Vector3(0, Z, X), 
                new Vector3(0, Z, -X), 
                new Vector3(0, -Z, X), 
                new Vector3(0, -Z, -X),
                new Vector3(Z, X, 0), 
                new Vector3(-Z, X, 0), 
                new Vector3(Z, -X, 0), 
                new Vector3(-Z, -X, 0)
            };
            
            var triangles = new int[] {
                0,4,1,
                0,9,4,
                9,5,4,
                4,5,8,
                4,8,1,
                8,10,1,
                8,3,10,
                5,3,8,
                5,2,3,
                2,7,3,
                7,10,3,
                7,6,10,
                7,11,6,
                11,0,6,
                0,1,6,
                6,1,10,
                9,0,11,
                9,11,2,
                9,2,5,
                7,2,11
            };

            return new Primitive(vertices, triangles);
        }
    }
}