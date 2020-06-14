using UnityEngine;

public class DemoTerrain : MonoBehaviour
{
    void Start()
    {
        foreach (Transform child in transform)
        {
            Vector3 pos = child.position;
            pos.y = Terrain.activeTerrain.SampleHeight(pos);
            child.position = pos;
        }
    }
}
