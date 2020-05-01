using UnityEngine;

public static class Bezier
{
    public static Vector3 Quadratic(float t, Vector3 a, Vector3 b, Vector3 c)
    {
        float v = (1 - t);

        Vector3 p = v * v * a;
        p += 2 * v * t * b;
        p += t * t * c;

        return p;
    }
}