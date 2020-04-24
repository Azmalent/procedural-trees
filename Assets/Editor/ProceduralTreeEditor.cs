using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ProceduralTree)), CanEditMultipleObjects]
public class ProceduralTreeEditor : Editor
{
    ProceduralTree tree;

    private void OnEnable()
    {
        tree = target as ProceduralTree;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Trunk Settings", EditorStyles.boldLabel);
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Foliage Parameters", EditorStyles.boldLabel);
        DrawFoliageOptions();
    }

    private void DrawFoliageOptions()
    {
        tree.FoliageStyle = (ProceduralFoliageStyle)
            EditorGUILayout.EnumPopup("Style", tree.FoliageStyle);

        if (tree.FoliageStyle == ProceduralFoliageStyle.None) return;

        //TODO: display color array somehow

        tree.FoliageWidth = EditorGUILayout.Slider("Width", tree.FoliageWidth,
            ProceduralTree.MIN_FOLIAGE_SIZE, ProceduralTree.MAX_FOLIAGE_SIZE);
        tree.FoliageHeight = EditorGUILayout.Slider("Height", tree.FoliageHeight,
            ProceduralTree.MIN_FOLIAGE_SIZE, ProceduralTree.MAX_FOLIAGE_SIZE);

        switch (tree.FoliageStyle)
        {
            //TODO: style-specific settings
        }
    }
}