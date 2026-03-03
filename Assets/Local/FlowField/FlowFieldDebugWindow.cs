using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Text;

public class FlowFieldDebugWindow : EditorWindow {

    private FlowFieldSpaceSource spaceSource;
    private FlowFieldObstacleSource[] obstacleSources;

    private FlowFieldSpace space;
    private HashSet<Vector2Int> obstacleCells = new();
    
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
    }

    private void OnBecameInvisible() {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    void OnGUI() {
        GUILayout.Label("obstacle sources: " + obstacleSources.Length);
        GUILayout.Label("space source found: " + (spaceSource == null ? "no" : "yes"));
        GUILayout.Label("obstacle cells: " + obstacleCells.Count);
    }

    private void OnSceneGUI(SceneView sceneView) {
        if (!sceneView.drawGizmos)
            return;

        if (!Application.isPlaying) {
            DrawEditTimeSceneGUI();
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

}