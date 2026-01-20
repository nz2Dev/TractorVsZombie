using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;

[ExecuteInEditMode]
public class FlowFieldsSurface : MonoBehaviour {
    
    private const float DefaultScale = 1;

    [SerializeField] private int size;
    [SerializeField] private LayerMask blockersMask;
    [SerializeField] private Vector2Int[] blockedCells = new Vector2Int[0];
    [Space]
    [SerializeField] private bool bakeInRealTime;
    [SerializeField] private bool displayBlockers = true;

    private FlowFieldsSpace space;

    public int Size => size;
    public FlowFieldsSpace Space => space;
    public Vector2Int[] BlockedCells => blockedCells;
    public bool BakeInRealTime => bakeInRealTime;
    public bool DisplayBlockers => displayBlockers;

    private void OnValidate() {
        DefineSpace();
    }

    private void Awake() {
        DefineSpace();
    }

    private void DefineSpace() {
        space = new FlowFieldsSpace(size, DefaultScale);
    }

    public void SetSize(int size) {
        this.size = size;
        DefineSpace();
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