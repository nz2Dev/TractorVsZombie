using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;

public struct CircleBlocker {
    public Vector3 position;
    public int radius;
}

[ExecuteInEditMode]
public class FlowFieldSurface : MonoBehaviour {
    
    private const float DefaultScale = 1;

    [SerializeField] private int size;
    [SerializeField] private LayerMask blockersMask;
    [SerializeField] private Vector2Int[] blockedCells = new Vector2Int[0];
    [Space]
    [SerializeField] private bool bakeInRealTime;
    [SerializeField] private bool displayBlockers = true;

    private FlowFieldSpace space;
    private int blockerIdCounter;
    private Dictionary<int, CircleBlocker> dynamicBlockers = new();
    private List<Vector2Int> dynamicBlockedCells = new();
    private List<Vector2Int> allBlockedCells = new();

    public int Size => size;
    public FlowFieldSpace Space => space;
    public IReadOnlyList<Vector2Int> BlockedCells => allBlockedCells;
    public bool BakeInRealTime => bakeInRealTime;
    public bool DisplayBlockers => displayBlockers;

    private void OnValidate() {
        DefineSpace();
    }

    private void Awake() {
        dynamicBlockers = new();
        dynamicBlockedCells = new();
        allBlockedCells = new();
        DefineSpace();
        CacheAllBlockedCells();
    }

    private void DefineSpace() {
        space = new FlowFieldSpace(size, DefaultScale);
    }

    private void CacheAllBlockedCells() {
        allBlockedCells.Clear();
        allBlockedCells.AddRange(blockedCells);
        allBlockedCells.AddRange(dynamicBlockedCells);
    }

    public void SetSize(int size) {
        this.size = size;
        DefineSpace();
    }

    public int AddBlockerShape(Vector3 center, int radius) {
        var blockerId = ++blockerIdCounter;
        dynamicBlockers[blockerId] = new CircleBlocker {
            position = center,
            radius = radius
        };
        return blockerId;
    }

    public void RemoveBlockerShape(int shapeId) {
        dynamicBlockers.Remove(shapeId);
    }

    public void BakeDynamicBlockers() {
        dynamicBlockedCells.Clear();

        foreach (var shape in dynamicBlockers.Values) {
            var centerGrid = GetGridPositionClamped(shape.position);
            var radius = shape.radius;
            for (int x = -radius; x <= radius; x++) {
                for (int y = -radius; y <= radius; y++) {
                    var offset = new Vector2Int(x, y);
                    if (offset.sqrMagnitude <= radius * radius) {
                        dynamicBlockedCells.Add(centerGrid + offset);
                    }
                }
            }    
        }

        CacheAllBlockedCells();
    }

    public void BakeBlockers() {
        var colliders = FindObjectsByType<Collider>(FindObjectsSortMode.None);
        BakeWithBlockers(colliders);
    }

    public void BakeWithBlockers(IEnumerable<Collider> colliders) {
        if (colliders == null)
            return;

        var cellsSet = new HashSet<Vector2Int>(size * size);

        foreach (var collider in colliders) {
            if (DoesMaskContainsLayer(blockersMask, collider.gameObject.layer)) {
                CellRaycaster.ColliderCast(collider, space, cellsSet);
            }
        }

        blockedCells = cellsSet.ToArray();
    }

    public Vector3 GetWorldPosition(int x, int y, bool atCenter = true) {
        return space.ConvertToWorld(new Vector2Int(x, y), atCenter);
    }

    public Vector2Int GetGridPositionClamped(Vector3 worldPosition) {
        return space.ConvertToGridClampled(worldPosition);
    }

    private static bool DoesMaskContainsLayer(LayerMask layerMask, int layer) {
        return (layerMask.value & (1 << layer)) != 0;
    }

}