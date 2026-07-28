using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

public class FlowFieldDebugWindow : EditorWindow {

    private FlowFieldSpaceSource spaceSource;
    private FlowFieldObstacleSource[] obstacleSources;

    private FlowFieldSpace space;
    private HashSet<Vector2Int> obstacleCells = new();

    private int selectedIndex;
    private string[] flowFieldNames = new string[0];
    
    [MenuItem("Tools/FlowField Debuger")]
    private static void ShowWindow() {
        var window = GetWindow<FlowFieldDebugWindow>();
        window.titleContent = new GUIContent("FlowField Debug");
        window.Show();
    }

    private void OnBecameVisible() {
        SceneView.duringSceneGui += OnSceneGUI;
        spaceSource = FindFirstObjectByType<FlowFieldSpaceSource>();
        space = spaceSource == null ? new FlowFieldSpace(10, 1) : spaceSource.Space;

        obstacleSources = FindObjectsByType<FlowFieldObstacleSource>(FindObjectsSortMode.None);
        obstacleCells.Clear();
        foreach (var obstacleSource in obstacleSources) {
            var obstacle = obstacleSource.CreateObstacle();
            foreach (var cell in obstacle.CalculateCells(space)) {
                obstacleCells.Add(cell);
            }
        }

        if (Application.isPlaying) {
            flowFieldNames = FlowFieldSystem.Instance.Handles.Select((handle, index) => "flow field " + index).ToArray();
        }
    }

    private void OnBecameInvisible() {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    void OnGUI() {
        GUILayout.Label("obstacle sources: " + obstacleSources.Length);
        GUILayout.Label("space source found: " + (spaceSource == null ? "no" : "yes"));
        GUILayout.Label("obstacle cells: " + obstacleCells.Count);

        if (Application.isPlaying) {
            GUILayout.Label("Flow Fields Count: " + FlowFieldSystem.Instance.Handles.Count);
            selectedIndex = EditorGUILayout.Popup("Select Flow Field", selectedIndex, flowFieldNames);
            
            if (selectedIndex >= 0 && selectedIndex < flowFieldNames.Length) {
                EditorGUILayout.LabelField("Selected:", flowFieldNames[selectedIndex]);
            }
        }
    }

    private void OnSceneGUI(SceneView sceneView) {
        if (!sceneView.drawGizmos)
            return;

        if (!Application.isPlaying) {
            DrawEditTimeSceneGUI();
        } else {
            DrawRuntimeSceneGUI();
        }
    }

    private void DrawEditTimeSceneGUI() {
        Handles.color = Color.white;
        Handles.DrawPolyLine(
            space.Anchor,
            space.Anchor + new Vector3(space.Size * space.Scale, 0, 0),
            space.Anchor + new Vector3(space.Size * space.Scale, 0, space.Size * space.Scale),
            space.Anchor + new Vector3(0, 0, space.Size * space.Scale),
            space.Anchor
        );

        Handles.color = Color.red;
        foreach (var blockedCell in obstacleCells) {
            var worldPos = space.ConvertToWorld(blockedCell, atCenter: true);
            Handles.RectangleHandleCap(0, worldPos, Quaternion.LookRotation(Vector3.up), space.Scale * 0.5f, EventType.Repaint);
        }
    }

    private void DrawRuntimeSceneGUI() {
        var system = FlowFieldSystem.Instance;
        if (system.Handles.Count == 0)
            return;

        DrawGrid(new Vector3(-space.Size * space.Scale * 0.5f, 0, -space.Size * space.Scale * 0.5f), space.Size, space.Size, space.Scale, 3);

        var selectedHandle = system.Handles[selectedIndex];
        var flowField = selectedHandle.flowField;
        for (int x = 0; x < flowField.Size; x++) {
            for (int y = 0; y < flowField.Size; y++) {
                var worldPos = space.ConvertToWorld(new Vector2Int(x, y), atCenter: true);
                var flowVector = system.GetFlowVector(selectedHandle, worldPos);
                if (!flowField[x, y].IsBlocked()) {
                    if (flowField[x, y].HasFlag(CellFlags.HasLineOfSight)) {
                        Handles.color = Color.yellow;
                    } else {
                        Handles.color = Color.white;
                    }
                    DrawArrow(worldPos, flowVector, space.Scale * 0.9f, space.Scale * 0.3f);
                }
            }
        }
    }

    public static void DrawGrid(Vector3 origin, int width, int height, float cellSize, float lineThickness) {
        // Vertical lines
        for (int x = 0; x <= width; x++) {
            Vector3 start = origin + new Vector3(x * cellSize, 0f, 0f);
            Vector3 end = origin + new Vector3(x * cellSize, 0f, height * cellSize);

            Handles.DrawAAPolyLine(
                lineThickness,
                start,
                end
            );
        }

        // Horizontal lines
        for (int y = 0; y <= height; y++) {
            Vector3 start = origin + new Vector3(0f, 0f, y * cellSize);
            Vector3 end = origin + new Vector3(width * cellSize, 0f, y * cellSize);

            Handles.DrawAAPolyLine(
                lineThickness,
                start,
                end
            );
        }
    }

    private void DrawCrosshair(Vector3 worldPos, float size) {
        var halfSize = size * 0.5f;
        Handles.DrawLine(worldPos - halfSize * Vector3.left, worldPos + Vector3.right * halfSize, 2);
        Handles.DrawLine(worldPos - halfSize * Vector3.forward, worldPos + Vector3.back * halfSize, 2);
    }

    private static void DrawArrow(Vector3 start, Vector3 direction, float length, float headLength) {
        start = start - (direction * length * 0.5f);
        Vector3 end = start + direction * length;
        
        Handles.DrawLine(start, end, thickness: 2);

        var lookRotation = Quaternion.LookRotation(direction);

        Vector3 right = lookRotation *
                        Quaternion.Euler(0, 180 - 25, 0) *
                        Vector3.forward;

        Vector3 left = lookRotation *
                    Quaternion.Euler(0, 180 + 25, 0) *
                    Vector3.forward;

        Handles.DrawLine(end, end + right * headLength, thickness: 2);
        Handles.DrawLine(end, end + left * headLength, thickness: 2);
    }

}