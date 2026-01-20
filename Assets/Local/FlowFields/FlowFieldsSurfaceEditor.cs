using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FlowFieldsSurface))]
public class FlowFieldsSurfaceEditor : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
    }

    private void OnEnable() {
        EditorApplication.update += OnEditorUpdate;
    }

    private void OnDisable() {
        EditorApplication.update -= OnEditorUpdate;
    }

    private void OnEditorUpdate() {
        var surface = target as FlowFieldsSurface;
        if (surface == null)
            return;

        if (surface.BakeInRealTime) {
            surface.BakeBlockers();
        }
    }

    void OnSceneGUI() {
        var surface = target as FlowFieldsSurface;
        var space = surface.Space;
        
        Handles.color = Color.white;
        Handles.DrawPolyLine(
            space.Anchor,
            space.Anchor + new Vector3(space.Size * space.Scale, 0, 0),
            space.Anchor + new Vector3(space.Size * space.Scale, 0, space.Size * space.Scale),
            space.Anchor + new Vector3(0, 0, space.Size * space.Scale),
            space.Anchor
        );

        if (surface.DisplayBlockers) {
            DrawBlockers(surface);
        }
    }

    private void DrawBlockers(FlowFieldsSurface surface) {
        Handles.color = Color.red;
        foreach (var blockedCell in surface.BlockedCells) {
            var worldPos = surface.GetWorldPosition(blockedCell.x, blockedCell.y, atCenter: true);
            Handles.RectangleHandleCap(0, worldPos, Quaternion.LookRotation(Vector3.up), surface.Space.Scale * 0.5f, EventType.Repaint);
        }
    }

    /*
    private void DrawFields(FlowFieldsSurface surface) {
        Handles.color = Color.white;
        for (int row = 0; row < surface.Size; row++) {
            for (int column = 0; column < surface.Size; column++) {
                var worldPos = surface.GetGridPosition(row, column, atCenter: true);
                var flowVector = surface.GetFlowVector(row, column) * 0.5f;
                Handles.DrawLine(worldPos, worldPos + flowVector, thickness: 2);
            }
        }
    }
    */

}