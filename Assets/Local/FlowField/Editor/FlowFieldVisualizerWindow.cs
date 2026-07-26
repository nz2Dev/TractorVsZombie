using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public class FlowFieldVisualizerWindow : EditorWindow {

    private const float SidebarWidth = 300f;
    private const int MaxGridSize = 32;

    [SerializeField] private int gridSize = 10;
    [SerializeField] private Vector2Int goal;
    [SerializeField] private byte[] costsPaint = new byte[MaxGridSize * MaxGridSize];
    [SerializeField] private int inputType = 2;
    [SerializeField] private bool showCosts;
    [SerializeField] private bool enableLineOfSight;

    private static string[] GRID_INPUT_TYPES = new string[] { "Wall Edit", "Goal Edit", "Inspection" };
    private bool? wallBrushPlacing;
    private Vector2Int? selectedCell;

    private FlowField field;
    private int maxIntegrationCost;

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
        maxIntegrationCost = GetMaxIntegrationCost();
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

        int oldSize = gridSize;
        gridSize = EditorGUILayout.IntSlider("Size", gridSize, 2, MaxGridSize);
        if (oldSize != gridSize) {
            goal = new Vector2Int {
                x = Mathf.Clamp(goal.x, 0, gridSize - 1),
                y = Mathf.Clamp(goal.y, 0, gridSize - 1),
            };
            OnStructureChanged();
        }
        
        var oldLOS = enableLineOfSight;
        enableLineOfSight = EditorGUILayout.Toggle("Line Of Sight Pass", enableLineOfSight);
        FlowFieldIntegrator.losEnabled = enableLineOfSight;
        if (oldLOS != enableLineOfSight) {
            OnConfigChanged();
        }

        GUILayout.Space(12);
        DrawSectionHeader("Display");
        var oldShowCosts = showCosts;
        showCosts = EditorGUILayout.Toggle("Show Costs", showCosts);
        if (oldShowCosts != showCosts) {
            OnDisplayConfigChanged();
        }

        GUILayout.Space(12);
        DrawSectionHeader("Input");
        inputType = GUILayout.Toolbar(inputType, GRID_INPUT_TYPES);

        if (inputType == 2) {
            DrawCellInspector();
        }
    }

    private void OnDisplayConfigChanged() {
        Repaint();
    }

    private void OnConfigChanged() {
        Rerun();
        Repaint();
    }

    private void OnStructureChanged() {
        Rebuild();
        Rerun();
        Repaint();
    }

    private void DrawSectionHeader(string label) {
        using (new GUIColorScope(new Color(0.6f, 0.8f, 1f))) {
            GUILayout.Label(label, new GUIStyle(EditorStyles.boldLabel) { fontSize = 12} );
            GUILayout.Space(2);
        }
    }

    private void DrawRectBorder(Rect rect, Color color, float thickness) {
        Handles.color = color;
        Handles.DrawAAPolyLine(thickness,
            new Vector3(rect.x, rect.y),
            new Vector3(rect.xMax - thickness, rect.y),
            new Vector3(rect.xMax - thickness, rect.yMax - thickness),
            new Vector3(rect.x, rect.yMax - thickness),
            new Vector3(rect.x, rect.y));
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
        for (int x = 0; x < gridSize; x++) {
            for (int y = 0; y < gridSize; y++) {
                DrawCell(new Vector2Int(x, y), new Rect(x * cellViewSize, gridRect.height - ((y + 1) * cellViewSize), cellViewSize, cellViewSize), cellViewSize);
            }
        }
        Handles.EndGUI();

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
    }

    private void DrawCell(Vector2Int cell, Rect cellRect, float cellViewSize) {
        var cellDisplay = GetCellDisplaay(cell);
        if (cell == selectedCell) {
            cellDisplay.borderColor = Color.yellow;
        }

        EditorGUI.DrawRect(cellRect, cellDisplay.backgroundColor);

        if (cellDisplay.borderColor.HasValue) {
            DrawRectBorder(cellRect, cellDisplay.borderColor.Value, 4f);
        }

        if (!string.IsNullOrEmpty(cellDisplay.label)) {
            using (new GUIColorScope(cellDisplay.labelColor)) {
                GUI.Label(cellRect, cellDisplay.label, new(EditorStyles.miniLabel) {
                    fontSize = Mathf.Clamp(Mathf.RoundToInt(cellViewSize * 0.32f), 8, 14),
                    alignment = TextAnchor.MiddleCenter,
                });
            }
        }
        
        if (cellDisplay.icon.HasValue) {
            if (cellDisplay.icon.Value == Icon.Arrow) {
                DrawArrow(cellRect, cellDisplay.iconDirection2D, cellDisplay.iconColor);
            } else if (cellDisplay.icon.Value == Icon.Crosshair) {
                DrawCrosshair(cellRect, cellDisplay.iconColor);
            }
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

        var bestCost = field[cell.x, cell.y].integratedCost;
        EditorGUILayout.LabelField("Integrated Cost", bestCost.ToString());

        var flags = field[cell.x, cell.y].flags;
        EditorGUILayout.LabelField("Flags", flags.ToString());

        var flowCell = field[cell.x, cell.y];
        EditorGUILayout.LabelField("Flow Dir", flowCell.flowVector.ToString());
        EditorGUILayout.LabelField("Has LOS (Flow)", flowCell.HasFlag(CellFlags.HasLineOfSight).ToString());

        EditorGUI.indentLevel--;
    }

    private void DrawCrosshair(Rect rect, Color color) {
        var center = rect.center;
        float size = Mathf.Min(rect.width, rect.height) * 0.18f;
        Handles.color = color;
        Handles.DrawAAPolyLine(2f, new Vector3(center.x - size, center.y), new Vector3(center.x + size, center.y));
        Handles.DrawAAPolyLine(2f, new Vector3(center.x, center.y - size), new Vector3(center.x, center.y + size));
    }

    private void DrawArrow(Rect rect, Vector2Int dirOffset, Color color) {
        var center = rect.center;
        float size = Mathf.Min(rect.width, rect.height) * 0.38f;

        var forward = new Vector2(dirOffset.x, -dirOffset.y).normalized;
        var right = new Vector2(forward.y, -forward.x);

        var start = center - forward * (size * 0.5f);
        var end = center + forward * (size * 0.6f);

        Handles.color = color;
        Handles.DrawAAPolyLine(2.5f, start, end);

        float headSize = size * 0.35f;
        Handles.DrawAAPolyLine(2.5f, end, (Vector3)(end - forward * headSize + right * headSize));
        Handles.DrawAAPolyLine(2.5f, end, (Vector3)(end - forward * headSize - right * headSize));
    }

    private struct CellDisplay {
        public string label;
        public Color labelColor;
        public Color backgroundColor;
        public Color? borderColor;
        public Icon? icon;
        public Color iconColor;
        public Vector2Int iconDirection2D;
    }

    private enum Icon {
        Crosshair,
        Arrow
    }

    private CellDisplay GetCellDisplaay(Vector2Int cell) {
        var cellData = field[cell.x, cell.y];
        if (cellData.cost == Cell.WallCost) {
            return new CellDisplay {
                label = "W",
                labelColor = new Color(0.5f, 0.5f, 0.5f),
                backgroundColor = new Color(0.15f, 0.15f, 0.15f),
            };
        }
        else if (cell == goal) {
            return new CellDisplay {
                label = "G",
                labelColor = Color.green,
                backgroundColor = Color.black,
                borderColor = new Color(0.2f, 1f, 0.3f, 1f)
            };
        } else if (cellData.cost == Cell.DefaultCost) {
            return new CellDisplay {
                label = showCosts ? cellData.integratedCost.ToString("F1") : null,
                labelColor = Color.white,
                backgroundColor = GetIntegrationColor(cellData.integratedCost),
                icon = showCosts ? null : (cellData.HasFlag(CellFlags.HasLineOfSight) ? Icon.Crosshair : Icon.Arrow),
                iconDirection2D = cellData.flowVector,
                iconColor = Color.white,
                borderColor = cellData.HasFlag(CellFlags.WaveFrontBlocked) ? Color.red : null,
            };
        } else {
            return new CellDisplay {
                backgroundColor = Color.gray
            };
        }
    }

    private int GetMaxIntegrationCost() {
        var max = 0;
        for (int y = 0; y < gridSize; y++) {
            for (int x = 0; x < gridSize; x++) {
                var c = field[x, y].integratedCost;
                if (c > max)
                    max = c;
            }
        }
        return max;
    }

    private Color GetIntegrationColor(int integratedCost) {
        float t = maxIntegrationCost > 0 ? (float) integratedCost / maxIntegrationCost : 0f;
        return Color.Lerp(new Color(0.0f, 0.55f, 0.8f), new Color(0.25f, 0.0f, 0.45f), t);
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
        if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) {
            if (TryGetCellUnderMouse(gridRect, cellViewSize, out var cell)) {
                selectedCell = cell;
                OnDisplayConfigChanged();
            }
        }
    }

    private void HandleGoalEditing(Rect gridRect, float cellViewSize) {
        Event e = Event.current;
        if (e.type == EventType.MouseDown || e.type == EventType.MouseDrag) {
            if (TryGetCellUnderMouse(gridRect, cellViewSize, out var cell)) {
                goal = cell;
                OnConfigChanged();
            }
        }
    }

    private void HandleWallEditing(Rect gridRect, float cellViewSize) {
        Event e = Event.current;
        if (e.type == EventType.MouseDown && (e.button == 0 || e.button == 1)) {
            if (TryGetCellUnderMouse(gridRect, cellViewSize, out var cell)) {
                wallBrushPlacing = e.button == 0;
                ApplyWallBrush(cell);
                e.Use();
                OnStructureChanged();
            }
        }
        else if (e.type == EventType.MouseDrag && wallBrushPlacing.HasValue) {
            if (TryGetCellUnderMouse(gridRect, cellViewSize, out var cell)) {
                ApplyWallBrush(cell);
                e.Use();
                OnStructureChanged();
            }
        }
        else if (e.type == EventType.MouseUp) {
            wallBrushPlacing = null;
        }
    }

    private bool TryGetCellUnderMouse(Rect gridRect, float cellViewSize, out Vector2Int cell) {
        var mousePosition = Event.current.mousePosition;
        // Debug.Log($"grid {gridRect} event mouse {Event.current.mousePosition} local mouse {mousePosition}");
        int gridColumn = Mathf.FloorToInt(mousePosition.x / cellViewSize);
        int gridRow = Mathf.FloorToInt((gridRect.height - mousePosition.y) / cellViewSize);
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

    private struct GUIColorScope : IDisposable {
        private readonly Color previous;
        public GUIColorScope(Color color) {
            previous = GUI.contentColor;
            GUI.contentColor = color;
        }
        public void Dispose() => GUI.contentColor = previous;
    }
}