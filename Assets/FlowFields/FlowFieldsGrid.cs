using UnityEditor;

using UnityEngine;
using UnityEngine.XR;

public class FlowFieldsGrid : MonoBehaviour {
    
    [SerializeField] private int size;
    [SerializeField] private Vector2Int[] blockedCells;
    [Range(0.1f, 1f)][SerializeField] private float scale = 1;

    private Vector3 anchor;

    private void OnValidate() {
        var sizeOffset = size * 0.5f;
        anchor = transform.position + new Vector3(-sizeOffset * scale, 0, -sizeOffset * scale);
    }

    public void UpdateCells(FlowFields flowFields) {
        flowFields.SetGrid(size);
        foreach (var blocked in blockedCells) {
            var gridLocation = ConvertToGrid(new Vector3(blocked.x, 0, blocked.y));
            flowFields.SetCellBlocked(gridLocation.x, gridLocation.y, true);
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
            var worldPos = new Vector3(blockedCell.x + scale * 0.5f, 0, blockedCell.y + scale * 0.5f);
            Handles.RectangleHandleCap(0, worldPos, Quaternion.LookRotation(Vector3.up), scale * 0.5f, EventType.Repaint);
        }
    }

}