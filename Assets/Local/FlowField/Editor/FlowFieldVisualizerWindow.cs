using UnityEngine;
using UnityEditor;
using System;

public class FlowFieldVisualizerWindow : EditorWindow {

    private const float SidebarWidth = 250f;
    private const int MaxGridSize = 32;

    [SerializeField] private int gridSize = 10;
    [SerializeField] private Vector2Int goal;
    [SerializeField] private byte[] costsPaint = new byte[MaxGridSize * MaxGridSize];
    [SerializeField] private int editingType = 0;

    private static string[] GRID_EDITING_TYPES = new string[] { "Walls", "Goal" };
    private bool? wallBrushPlacing;


    [MenuItem("Tools/FlowField/Visualizer")]
    private static void ShowWindow() {
        var window = GetWindow<FlowFieldVisualizerWindow>();
        window.titleContent = new GUIContent("FlowFieldVisualizerWindow");
        window.Show();
    }

    private void OnGUI() {
        EditorGUI.DrawRect(new Rect(0, 0, SidebarWidth, position.height), new Color(0.18f, 0.18f, 0.18f));
        EditorGUI.DrawRect(new Rect(SidebarWidth - 1f, 0, 1f, position.height), new Color(0.12f, 0.12f, 0.12f));

        GUILayout.BeginArea(new Rect(0, 0, SidebarWidth, position.height));
        DrawSidebar();
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(SidebarWidth, 0, position.width - SidebarWidth, position.height));
        DrawGridArea(position.width - SidebarWidth, position.height);
        GUILayout.EndArea();
    }

    private void DrawSidebar() {
        GUILayout.Space(12);
        DrawSectionHeader("Grid Size");

        bool sizeChanged = false;
        int newSize = EditorGUILayout.IntSlider("Size", gridSize, 2, MaxGridSize);
        if (newSize != gridSize) {
            sizeChanged = true;
            gridSize = newSize;
        }

        GUILayout.Space(12);
        editingType = GUILayout.Toolbar(editingType, GRID_EDITING_TYPES);

        if (sizeChanged) {
            goal.x = Mathf.Clamp(goal.x, 0, gridSize - 1);
            goal.y = Mathf.Clamp(goal.y, 0, gridSize - 1);
        }
    }

    private void DrawSectionHeader(string label) {
        var style = new GUIStyle(EditorStyles.boldLabel) { fontSize = 12 };
        style.normal.textColor = new Color(0.6f, 0.8f, 1f);
        GUILayout.Label(label, style);
        GUILayout.Space(2);
    }

    private void DrawGridArea(float viewWidth, float viewHeight) {
        EditorGUI.DrawRect(new Rect(0, 0, viewWidth, viewHeight), new Color(0.1f, 0.1f, 0.1f));

        var minViewSize = Mathf.Min(viewWidth, viewHeight) - 20;
        var gridRect = new Rect(viewWidth * 0.5f - minViewSize * 0.5f, viewHeight * 0.5f - minViewSize * 0.5f, minViewSize, minViewSize);
        GUILayout.BeginArea(gridRect);
        DrawGrid(minViewSize);
        OnGridInput(gridRect, minViewSize);
        GUILayout.EndArea();
    }

    private void DrawGrid(float viewSize) {
        EditorGUI.DrawRect(new Rect(0, 0, viewSize, viewSize), new Color(0.2f, 0.2f, 0.2f));
        var cellViewSize = viewSize / Mathf.Max(gridSize, 1);

        Handles.BeginGUI();
        // Grid lines
        Handles.color = new Color(1f, 1f, 1f, 0.12f);
        for (int x = 0; x <= gridSize; x++) {
            float px = x * cellViewSize;
            Handles.DrawLine(new Vector3(px, 0), new Vector3(px, viewSize));
        }
        for (int y = 0; y <= gridSize; y++) {
            float py = y * cellViewSize;
            Handles.DrawLine(new Vector3(0, py), new Vector3(viewSize, py));
        }
        Handles.EndGUI();

        Handles.BeginGUI();
        for (int x = 0; x < gridSize; x++) {
            for (int y = 0; y < gridSize; y++) {
                DrawCell(new Vector2Int(x, y), new Rect(x * cellViewSize, y * cellViewSize, cellViewSize, cellViewSize), cellViewSize);
            }
        }
        Handles.EndGUI();
    }

    private void DrawCell(Vector2Int cell, Rect cellRect, float cellViewSize) {
        var isWall = costsPaint[cell.y * MaxGridSize + cell.x] == Cell.WallCost;
        
        string labelText = "";
        Color labelColor = Color.white;
        int labelFontSize = Mathf.Clamp(Mathf.RoundToInt(cellViewSize * 0.32f), 8, 14);

        if (isWall) {
            labelText = "W";
            labelColor = new Color(0.5f, 0.5f, 0.5f);
        }

        if (!string.IsNullOrEmpty(labelText)) {
            var labelStyle = new GUIStyle(EditorStyles.miniLabel) {
                alignment = TextAnchor.MiddleCenter,
                fontSize = labelFontSize
            };
            labelStyle.normal.textColor = labelColor;
            GUI.Label(cellRect, labelText, labelStyle);
        }
    }

    private void OnGridInput(Rect gridRect, float gridViewSize) {
        var cellViewSize = gridViewSize / Mathf.Max(gridSize, 1);
        if (editingType == 0) {
            HandleWallEditing(gridRect, cellViewSize);
        }
    }

    private void HandleWallEditing(Rect gridRect, float cellViewSize) {
        Event e = Event.current;
        if (e.type == EventType.MouseDown && (e.button == 0 || e.button == 1)) {
            if (TryGetCellUnderMouse(gridRect, cellViewSize, e.mousePosition, out var cell)) {
                wallBrushPlacing = e.button == 0;
                ApplyWallBrush(cell);
                e.Use();
            }
        } else if (e.type == EventType.MouseDrag && wallBrushPlacing.HasValue) {
            if (TryGetCellUnderMouse(gridRect, cellViewSize, e.mousePosition, out var cell)) {
                ApplyWallBrush(cell);
                e.Use();
            }
        } else if (e.type == EventType.MouseUp) {
            wallBrushPlacing = null;
        }
    }

    private bool TryGetCellUnderMouse(Rect gridRect, float cellViewSize, Vector2 mousePosition, out Vector2Int cell) {
        int gridColumn = Mathf.FloorToInt((mousePosition.x - gridRect.x) / cellViewSize);
        int gridRow = Mathf.FloorToInt((mousePosition.y - gridRect.y) / cellViewSize);
        gridColumn = Mathf.Clamp(gridColumn, 0, gridSize - 1);
        gridRow = Mathf.Clamp(gridRow, 0, gridSize - 1);
        cell = new Vector2Int(gridColumn, gridRow);
        return true;
    }

    private void ApplyWallBrush(Vector2Int cell) {
        bool placing = wallBrushPlacing.GetValueOrDefault(true);
        byte current = costsPaint[cell.y * MaxGridSize + cell.x];

        if (placing && current != Cell.WallCost) {
            costsPaint[cell.y * MaxGridSize + cell.x] = (byte) Cell.WallCost;

        } else if (!placing && current == Cell.WallCost) {
            costsPaint[cell.y * MaxGridSize + cell.x] = (byte) Cell.DefaultCost;
        }
    }
}