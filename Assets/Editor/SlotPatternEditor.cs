using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SlotPatternSO))]
public class SlotPatternEditor : Editor
{
    private const float TOGGLE_SIZE = 20f;
    private const float PADDING = 4f;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw everything EXCEPT cells
        DrawPropertiesExcluding(serializedObject, "cells");

        SerializedProperty rowsProp = serializedObject.FindProperty("rows");
        SerializedProperty colsProp = serializedObject.FindProperty("columns");
        SerializedProperty cellsProp = serializedObject.FindProperty("cells");

        int rows = rowsProp.intValue;
        int cols = colsProp.intValue;
        int expectedSize = rows * cols;

        if (cellsProp.arraySize != expectedSize)
        {
            cellsProp.arraySize = expectedSize;
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Pattern Matrix", EditorStyles.boldLabel);

        DrawGrid(cellsProp, rows, cols);

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawGrid(SerializedProperty cells, int rows, int cols)
    {
        for (int r = 0; r < rows; r++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int c = 0; c < cols; c++)
            {
                int index = r * cols + c;
                SerializedProperty cell = cells.GetArrayElementAtIndex(index);

                cell.boolValue = GUILayout.Toggle(
                    cell.boolValue,
                    GUIContent.none,
                    GUILayout.Width(TOGGLE_SIZE),
                    GUILayout.Height(TOGGLE_SIZE)
                );
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(PADDING);
        }
    }
}
