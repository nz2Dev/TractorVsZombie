using UnityEngine;

public class FlowFieldSpace {

    private readonly int size;
    private readonly float scale;
    
    private readonly Vector3 anchor;

    public int Size => size;
    public float Scale => scale;
    public Vector3 Anchor => anchor;

    public FlowFieldSpace(int gridSize, float scale) {
        this.scale = scale;
        this.size = gridSize;
        var sizeOffset = size * 0.5f;
        anchor = new Vector3(-sizeOffset * scale, 0, -sizeOffset * scale);
    }

    public Vector2Int ConvertToGrid(Vector3 worldPosition) {
        var localPosition = worldPosition - anchor;
        localPosition /= scale;
        return new Vector2Int(Mathf.FloorToInt(localPosition.x), Mathf.FloorToInt(localPosition.z));
    }

    public Vector2Int ConvertToGridClampled(Vector3 worldPosition) {
        var gridPosition = ConvertToGrid(worldPosition);
        gridPosition.x = Mathf.Clamp(gridPosition.x, 0, size - 1);
        gridPosition.y = Mathf.Clamp(gridPosition.y, 0, size - 1);
        return gridPosition;
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
}