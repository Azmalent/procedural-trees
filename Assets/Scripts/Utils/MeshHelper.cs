using UnityEngine;

public static class MeshHelper
{
    /// <summary>
    /// Returns the index of the triangle that consists of three specified points.
    /// </summary>
    public static Vector3 Normal(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 ab = b - a;
        Vector3 ac = c - a;
        return Vector3.Cross(ab, ac).normalized;
    }
}