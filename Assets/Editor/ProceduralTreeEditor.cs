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
        DrawDefaultInspector();
        DrawFoliageOptions();
    }

    private bool colorsExpanded = true;
    private void DrawFoliageColorList()
    {
        var colors = serializedObject.FindProperty("FoliageColors");
        colorsExpanded = EditorGUILayout.Foldout(colorsExpanded, "Possible Colors");

        if (colorsExpanded)
        {
            EditorGUI.indentLevel++;

            //TODO: use a slider
            EditorGUILayout.PropertyField(colors.FindPropertyRelative("Array.size"));
            for (int i = 0; i < colors.arraySize; i++)
            {
                EditorGUILayout.PropertyField(colors.GetArrayElementAtIndex(i));
            }

            EditorGUI.indentLevel--;
        }
    }

    private void DrawFoliageOptions()
    {
        serializedObject.Update();

        if (tree.FoliageStyle == FoliageStyle.None) return;

        DrawFoliageColorList();
        //TODO: better editor
        EditorGUILayout.PropertyField(serializedObject.FindProperty("FoliageWidth"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("FoliageWidthVariance"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("FoliageHeight"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("FoliageHeightVariance"));

        switch (tree.FoliageStyle)
        {
            //TODO: style-specific settings
        }

        serializedObject.ApplyModifiedProperties();
    }
}