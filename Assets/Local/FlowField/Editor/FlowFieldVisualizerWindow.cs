using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public class FlowFieldVisualizerWindow : EditorWindow {

    private const float SidebarWidth = 250f;
    private const int MaxGridSize = 32;

    [SerializeField] private int gridSize = 10;
    [SerializeField] private Vector2Int goal;
    [SerializeField] private byte[] costsPaint = new byte[MaxGridSize * MaxGridSize];
    [SerializeField] private int inputType = 2;
    [SerializeField] private bool showCosts;

    private static string[] GRID_INPUT_TYPES = new string[] { "Wall Edit", "Goal Edit", "Inspection" };
    private bool? wallBrushPlacing;
    private Vector2Int? selectedCell;

    private FlowField field;

    [MenuItem("Tools/FlowField/Visualizer")]
    private static void ShowWindow() {
        var window = GetWindow<FlowFieldVisualizerWindow>();
        window.titleContent = new GUIContent("FlowFieldVisualizerWindow");
        window.Show();
    }

    private void OnEnable() {
        Rebuild();
        Rerun();
    }

    private void Rebuild() {
        var blockedCells = new List<Vector2Int>();
        for (int x = 0; x < gridSize; x++) {
            for (int y = 0; y < gridSize; y++) {
                if (costsPaint[y * MaxGridSize + x] == (byte) Cell.WallCost) {
                    blockedCells.Add(new Vector2Int(x, y));
                }
            }
        }
        field = new FlowField(gridSize, blockedCells);
    }

    private void Rerun() {
        FlowFieldIntegrator.Integrate(field, goal);
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
        if (sizeChanged) {
            goal = new Vector2Int {
                x = Mathf.Clamp(goal.x, 0, gridSize - 1),
                y = Mathf.Clamp(goal.y, 0, gridSize - 1),
            };
            Rebuild();
        }

        GUILayout.Space(12);
        DrawSectionHeader("Display");
        var oldShowCosts = showCosts;
        showCosts = GUILayout.Toggle(showCosts, " show costs?");
        if (oldShowCosts != showCosts)
            Repaint();

        GUILayout.Space(12);
        DrawSectionHeader("Input");
        inputType = GUILayout.Toolbar(inputType, GRID_INPUT_TYPES);

        if (inputType == 2) {
            DrawCellInspector();
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
        DrawGrid(gridRect, minViewSize);
        OnGridInput(gridRect, minViewSize);
        GUILayout.EndArea();
    }

    private void DrawGrid(Rect gridRect, float viewSize) {
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
                DrawCell(new Vector2Int(x, y), new Rect(x * cellViewSize, gridRect.height - ((y + 1) * cellViewSize), cellViewSize, cellViewSize), cellViewSize);
            }
        }
        Handles.EndGUI();
    }

    private void DrawCell(Vector2Int cell, Rect cellRect, float cellViewSize) {
        string labelText = "";
        Color labelColor = Color.white;
        int labelFontSize = Mathf.Clamp(Mathf.RoundToInt(cellViewSize * 0.32f), 8, 14);

        var isWall = field[cell.x, cell.y].cost == Cell.WallCost;
        if (isWall) {
            labelText = "W";
            labelColor = new Color(0.5f, 0.5f, 0.5f);
        }
        var isGoal = goal == cell;
        if (isGoal) {
            labelText = "G";
            labelColor = Color.green;
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

    private void DrawCellInspector() {
        if (!selectedCell.HasValue) {
            EditorGUILayout.HelpBox("Click a grid cell to inspect.", MessageType.Info);
            return;
        }

        var cell = selectedCell.Value;
        if (cell.x < 0 || cell.x >= gridSize || cell.y < 0 || cell.y >= gridSize) {
            selectedCell = null;
            return;
        }

        EditorGUI.indentLevel++;

        EditorGUILayout.LabelField("Coords", $"({cell.x}, {cell.y})");

        int costVal = field[cell.x, cell.y].cost;
        EditorGUILayout.LabelField("Cost", costVal == Cell.WallCost ? "Wall (255)" : costVal.ToString());

        // var bestCost = tile.Integration[cell.x, cell.y].BestCost;
        // EditorGUILayout.LabelField("Best Cost", bestCost == IntegrationField.Unreachable ? "Unreachable" : bestCost.ToString());

        // var flags = tile.Integration[cell.x, cell.y].Flags;
        // EditorGUILayout.LabelField("Flags", flags.ToString());

        // var flowCell = tile.Flow[cell.x, cell.y];
        // EditorGUILayout.LabelField("Flow Dir", flowCell.Direction.ToString());
        // EditorGUILayout.LabelField("Has LOS (Flow)", flowCell.HasLineOfSight.ToString());

        EditorGUI.indentLevel--;
    }

    private void OnGridInput(Rect gridRect, float gridViewSize) {
        var cellViewSize = gridViewSize / Mathf.Max(gridSize, 1);
        if (inputType == 0) {
            HandleWallEditing(gridRect, cellViewSize);
        }
        else if (inputType == 1) {
            HandleGoalEditing(gridRect, cellViewSize);
        }
        else if (inputType == 2) {
            HandleCellInspect(gridRect, cellViewSize);
        }
    }

    private void HandleCellInspect(Rect gridRect, float cellViewSize) {
        if (Event.current.type == EventType.MouseDown) {
            if (TryGetCellUnderMouse(gridRect, cellViewSize, Event.current.mousePosition, out var cell)) {
                selectedCell = cell;
                Repaint();
            }
        }
    }

    private void HandleGoalEditing(Rect gridRect, float cellViewSize) {
        Event e = Event.current;
        if (e.type == EventType.MouseDown || e.type == EventType.MouseDrag) {
            if (TryGetCellUnderMouse(gridRect, cellViewSize, e.mousePosition, out var cell)) {
                goal = cell;
                Repaint();
                Rerun();
            }
        }
    }

    private void HandleWallEditing(Rect gridRect, float cellViewSize) {
        Event e = Event.current;
        if (e.type == EventType.MouseDown && (e.button == 0 || e.button == 1)) {
            if (TryGetCellUnderMouse(gridRect, cellViewSize, e.mousePosition, out var cell)) {
                wallBrushPlacing = e.button == 0;
                ApplyWallBrush(cell);
                e.Use();
                Rebuild();
                Repaint();
            }
        }
        else if (e.type == EventType.MouseDrag && wallBrushPlacing.HasValue) {
            if (TryGetCellUnderMouse(gridRect, cellViewSize, e.mousePosition, out var cell)) {
                ApplyWallBrush(cell);
                e.Use();
                Rebuild();
                Repaint();
            }
        }
        else if (e.type == EventType.MouseUp) {
            wallBrushPlacing = null;
        }
    }

    private bool TryGetCellUnderMouse(Rect gridRect, float cellViewSize, Vector2 mousePosition, out Vector2Int cell) {
        int gridColumn = Mathf.FloorToInt((mousePosition.x - gridRect.x) / cellViewSize);
        int gridRow = Mathf.FloorToInt((gridRect.height - (mousePosition.y - gridRect.y)) / cellViewSize);
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

        }
        else if (!placing && current == Cell.WallCost) {
            costsPaint[cell.y * MaxGridSize + cell.x] = (byte) Cell.DefaultCost;
        }
    }
}