using UnityEditor;

using UnityEngine;

public class FlowFieldsGridBlocker : MonoBehaviour {

    [SerializeField] private FlowFieldsGrid grid;
    
    private void OnValidate() {
        
    }

    void OnDrawGizmos() {
        var collider = GetComponent<BoxCollider>();
        var center = collider.bounds.center;
        var extens = collider.bounds.extents;

        Vector3 bottomLeft = center + new Vector3(-extens.x, 0, -extens.z);
        Vector3 topLeft = center + new Vector3(-extens.x, 0, +extens.z);
        Vector3 topRight = center + new Vector3(+extens.x, 0, +extens.z);
        Vector3 bottomRight = center + new Vector3(+extens.x, 0, -extens.z);
        Handles.DrawPolyLine(
            bottomLeft,
            topLeft,
            topRight,
            bottomRight,
            bottomLeft
        );

        if (grid == null)
            return;

        var gridStart = grid.ConvertToGrid(bottomLeft);
        var gridEnd = grid.ConvertToGrid(topRight);
        var rowsSpan = gridEnd.x - gridStart.x;
        var columnSpan = gridEnd.y - gridStart.y;
        for (int row = 0; row <= rowsSpan; row++) {
            for (int column = 0; column <= columnSpan; column++) {
                var gridLocation = gridStart + new Vector2Int(row, column);
                var gridWorld = grid.ConvertToWorld(gridLocation, atCenter: true);

                var gridRay = new Ray(gridWorld + Vector3.up * 10, Vector3.down);
                var raycasted = collider.Raycast(gridRay, out var _, maxDistance: float.MaxValue);
                
                Handles.color = raycasted ? Color.red : Color.white;
                Handles.DrawWireDisc(gridWorld, Vector3.up, 0.5f);
            }
        }
    }

}