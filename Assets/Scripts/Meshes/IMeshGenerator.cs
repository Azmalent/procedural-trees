using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tree_Generator.Assets.Scripts
{
    public interface IMeshGenerator
    {
        void GenerateMesh();
        void PersistMesh();
    }
}