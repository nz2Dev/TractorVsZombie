using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;
using UnityEngine.XR;

public class FlowFieldsGrid : MonoBehaviour {
    
    [SerializeField] private int size;
    [SerializeField] private bool updateBlockersInEditor;
    [SerializeField] private Vector2Int[] blockedCells;
    [Range(0.1f, 1f)][SerializeField] private float scale = 1;

    private Vector3 anchor;

    private void OnValidate() {
        var sizeOffset = size * 0.5f;
        anchor = transform.position + new Vector3(-sizeOffset * scale, 0, -sizeOffset * scale);
    }

    [ContextMenu("Regenerate blockers")]
    private void GenerateBlockers() {
        var colliders = Object.FindObjectsOfType<Collider>();
        var cellsSet = new HashSet<Vector2Int>(size * size);

        foreach (var collider in colliders) {
            if (collider.gameObject.layer == 11 /* walls */) {
                FindBlockerRaycasts(collider, cellsSet);
            }
        }

        blockedCells = cellsSet.ToArray();
    }

    private void FindBlockerRaycasts(Collider collider, ISet<Vector2Int> collection) {
        var center = collider.bounds.center;
        var extens = collider.bounds.extents;
        var doubleUp = new Vector3(0, collider.bounds.size.y * 2, 0);

        Vector3 bottomLeft = center + new Vector3(-extens.x, 0, -extens.z);
        Vector3 topRight = center + new Vector3(+extens.x, 0, +extens.z);

        var gridStart = ConvertToGrid(bottomLeft);
        var gridEnd = ConvertToGrid(topRight);
        var rowsSpan = gridEnd.x - gridStart.x;
        var columnSpan = gridEnd.y - gridStart.y;
        for (int row = 0; row <= rowsSpan; row++) {
            for (int column = 0; column <= columnSpan; column++) {
                var gridLocation = gridStart + new Vector2Int(row, column);
                var gridWorld = ConvertToWorld(gridLocation, atCenter: true);

                var gridRay = new Ray(gridWorld + doubleUp, Vector3.down);
                var raycasted = collider.Raycast(gridRay, out var _, maxDistance: float.MaxValue);
                
                if (raycasted) {
                    collection.Add(gridLocation);
                }
            }
        }
    }

    public void UpdateCells(FlowFields flowFields) {
        flowFields.SetGrid(size);
        foreach (var blocked in blockedCells) {
            flowFields.SetCellBlocked(blocked.x, blocked.y, true);
        }
    }

    public Vector2Int ConvertToGrid(Vector3 worldPosition) {
        var localPosition = worldPosition - anchor;
        localPosition /= scale;
        return new Vector2Int(Mathf.FloorToInt(localPosition.x), Mathf.FloorToInt(localPosition.z));
    }

    public Vector3 ConvertToWorld(Vector2Int gridPosition, bool atCenter = false) {
        var localPosition = new Vector3(gridPosition.x, 0, gridPosition.y);
        if (atCenter) {
            var halfScale = scale * 0.5f;
            localPosition.x += halfScale;
            localPosition.z += halfScale;
        }
        localPosition *= scale;
        return anchor + localPosition;
    }

    private void OnDrawGizmos() {
        if (updateBlockersInEditor) {
            GenerateBlockers();
        }

        Handles.color = Color.white;
        Handles.DrawPolyLine(
            anchor,
            anchor + new Vector3(size * scale, 0, 0),
            anchor + new Vector3(size * scale, 0, size * scale),
            anchor + new Vector3(0, 0, size * scale),
            anchor
        );
        
        Handles.color = Color.red;
        foreach (var blockedCell in blockedCells) {
            var worldPos = ConvertToWorld(blockedCell, atCenter: true);
            Handles.RectangleHandleCap(0, worldPos, Quaternion.LookRotation(Vector3.up), scale * 0.5f, EventType.Repaint);
        }
    }

}