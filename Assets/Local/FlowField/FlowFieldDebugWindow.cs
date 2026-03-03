using UnityEngine;
using UnityEditor;

public class FlowFieldDebugWindow : EditorWindow {
    
    [MenuItem("Tools/FlowField Debuger")]
    private static void ShowWindow() {
        var window = GetWindow<FlowFieldDebugWindow>();
        window.titleContent = new GUIContent("FlowField Debug");
        window.Show();
    }

    // void OnGUI() {
    //     var surface = target as FlowFieldSpaceSource;
    //     var space = surface.Space;
        
    //     Handles.color = Color.white;
    //     Handles.DrawPolyLine(
    //         space.Anchor,
    //         space.Anchor + new Vector3(space.Size * space.Scale, 0, 0),
    //         space.Anchor + new Vector3(space.Size * space.Scale, 0, space.Size * space.Scale),
    //         space.Anchor + new Vector3(0, 0, space.Size * space.Scale),
    //         space.Anchor
    //     );

    //     if (surface.DisplayBlockers) {
    //         DrawBlockers(surface);
    //     }
    // }

    // private void DrawBlockers(FlowFieldSpaceSource surface) {
    //     Handles.color = Color.red;
    //     foreach (var blockedCell in surface.BlockedCells) {
    //         var worldPos = surface.GetWorldPosition(blockedCell.x, blockedCell.y, atCenter: true);
    //         Handles.RectangleHandleCap(0, worldPos, Quaternion.LookRotation(Vector3.up), surface.Space.Scale * 0.5f, EventType.Repaint);
    //     }
    // }
}