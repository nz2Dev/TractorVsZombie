using UnityEngine;
using UnityEditor;
using System;

public class FlowFieldVisualizerWindow : EditorWindow {

    private const float SidebarWidth = 250f;
    private const int MaxGridSize = 32;

    [SerializeField] private int gridSize = 10;
    [SerializeField] private Vector2Int goal;

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
        GUILayout.BeginArea(new Rect(viewWidth * 0.5f - minViewSize * 0.5f, viewHeight * 0.5f - minViewSize * 0.5f, minViewSize, minViewSize));
        DrawGrid(minViewSize);
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
    }
}