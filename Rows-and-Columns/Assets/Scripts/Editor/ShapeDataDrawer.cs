using UnityEngine;
using UnityEditor;
using System.CodeDom;

// Custom editor for ShapeData objects that works with multiple selected objects
[CustomEditor(typeof(ShapeData), false)]
[CanEditMultipleObjects]
[System.Serializable]
public class ShapeDataDrawer : Editor
{
    // Convenience property to access the target as ShapeData
    private ShapeData ShapeDataInstance => target as ShapeData;

    // Main inspector GUI drawing method
    public override void OnInspectorGUI()
    {
        // Update the serialized object representation
        serializedObject.Update();

        // Draw the clear board button
        ClearBoardButton();
        EditorGUILayout.Space();

        // Draw the columns/rows input fields
        DrawColumnsInputFields();
        EditorGUILayout.Space();

        // Only draw the board table if the board exists and has valid dimensions
        if (ShapeDataInstance.board != null && ShapeDataInstance.columns > 0 && ShapeDataInstance.rows > 0)
        {
            DrawBoardTable();
        }

        // Apply any modifications made to the serialized object
        serializedObject.ApplyModifiedProperties();

        // Mark the object as dirty if any GUI changes occurred
        if (GUI.changed)
        {
            EditorUtility.SetDirty(ShapeDataInstance);
        }
    }

    // Draws a button that clears the board
    private void ClearBoardButton()
    {
        if (GUILayout.Button("Clear Board"))
        {
            ShapeDataInstance.Clear();
        }
    }

    // Draws input fields for columns and rows, recreates board when dimensions change
    private void DrawColumnsInputFields()
    {
        // Store current values for comparison
        var columnsTemp = ShapeDataInstance.columns;
        var rowsTemp = ShapeDataInstance.rows;

        // Draw the dimension input fields
        ShapeDataInstance.columns = EditorGUILayout.IntField("Columns", ShapeDataInstance.columns);
        ShapeDataInstance.rows = EditorGUILayout.IntField("Rows", ShapeDataInstance.rows);

        // If dimensions changed and are valid, create a new board
        if ((ShapeDataInstance.columns != columnsTemp) || (ShapeDataInstance.rows != rowsTemp) &&
            ShapeDataInstance.columns > 0 && ShapeDataInstance.rows > 0)
        {
            ShapeDataInstance.CreateNewBoard();
        }
    }

    // Draws an interactive table representing the shape board
    private void DrawBoardTable()
    {
        // Define styles for the table and its elements
        var tableStyle = new GUIStyle("box");
        tableStyle.padding = new RectOffset(10, 10, 10, 10);
        tableStyle.margin.left = 32;

        var headerColumnStyle = new GUIStyle();
        headerColumnStyle.fixedWidth = 65;
        headerColumnStyle.alignment = TextAnchor.MiddleCenter;

        var rowStyle = new GUIStyle();
        rowStyle.fixedHeight = 25;
        rowStyle.alignment = TextAnchor.MiddleCenter;

        var dataFieldStyle = new GUIStyle(EditorStyles.miniButtonMid);
        dataFieldStyle.normal.background = Texture2D.grayTexture;
        dataFieldStyle.onNormal.background = Texture2D.whiteTexture;

        // Draw each cell in the board as a toggle
        for (var row = 0; row < ShapeDataInstance.rows; row++)
        {
            EditorGUILayout.BeginHorizontal(headerColumnStyle);

            for (var col = 0; col < ShapeDataInstance.columns; col++)
            {
                EditorGUILayout.BeginHorizontal(rowStyle);
                // Draw toggle for each cell and update the board data
                var data = EditorGUILayout.Toggle(ShapeDataInstance.board[row].column[col], dataFieldStyle);
                ShapeDataInstance.board[row].column[col] = data;
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}