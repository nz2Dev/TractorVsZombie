using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;

public class FlowFieldsSystem : MonoBehaviour {
    
    [SerializeField] private int size;
    [Range(0.1f, 1f)][SerializeField] private float scale = 1;
    [Space]
    [SerializeField] private bool updateBlockersInEditor;
    [SerializeField] private bool drawBlockers = true;
    [SerializeField] private Vector2Int[] blockedCells;
    [Space]
    [SerializeField] private bool drawComputation = true;
    [SerializeField] private bool costOrFields = true;
    [SerializeField] private bool updateGoalInEditor = true;
    [SerializeField] private Transform goalTransform;

    private FlowFieldsSpace space;
    private FlowFields flowFields = new FlowFields();

    private void OnValidate() {
        space = new FlowFieldsSpace(size, scale);
    }

    [ContextMenu("Regenerate blockers")]
    private void GenerateBlockers() {
        var colliders = Object.FindObjectsOfType<Collider>();
        var cellsSet = new HashSet<Vector2Int>(size * size);

        foreach (var collider in colliders) {
            if (collider.gameObject.layer == 11 /* walls */) {
                CellRaycaster.ColliderCast(collider, space, cellsSet);
            }
        }

        blockedCells = cellsSet.ToArray();
    }

    private void UpdateFields() {
        flowFields = new FlowFields();
        flowFields.SetGrid(size);
        foreach (var blocked in blockedCells) {
            flowFields.SetCellBlocked(blocked.x, blocked.y, true);
        }
        
        var goalGrid = space.ConvertToGrid(goalTransform == null ? Vector3.zero : goalTransform.position);
        flowFields.ComputeCosts(goalGrid);
        flowFields.ComputeFlow();
    }

    private void OnDrawGizmos() {
        if (updateBlockersInEditor) {
            GenerateBlockers();
        }

        if (updateGoalInEditor || updateBlockersInEditor) {
            UpdateFields();
        }

        Handles.color = Color.white;
        Handles.DrawPolyLine(
            space.Anchor,
            space.Anchor + new Vector3(space.Size * space.Scale, 0, 0),
            space.Anchor + new Vector3(space.Size * space.Scale, 0, space.Size * space.Scale),
            space.Anchor + new Vector3(0, 0, space.Size * space.Scale),
            space.Anchor
        );

        if (drawBlockers) {
            DrawBlockers();
        }

        if (drawComputation) {
            if (costOrFields) {
                DrawCosts();
            } else {
                DrawFields();
            }
        }
    }

    private void DrawBlockers() {
        Handles.color = Color.red;
        foreach (var blockedCell in blockedCells) {
            var worldPos = space.ConvertToWorld(blockedCell, atCenter: true);
            Handles.RectangleHandleCap(0, worldPos, Quaternion.LookRotation(Vector3.up), scale * 0.5f, EventType.Repaint);
        }
    }

    private void DrawFields() {
        Handles.color = Color.white;
        for (int row = 0; row < flowFields.Size; row++) {
            for (int column = 0; column < flowFields.Size; column++) {
                var worldPos = space.ConvertToWorld(new Vector2Int(row, column), atCenter: true);
                var flowVector = flowFields.GetFlowVector(row, column);
                var pointArrow = new Vector3(flowVector.x, 0, flowVector.y).normalized * 0.5f;
                Handles.DrawLine(worldPos, worldPos + pointArrow, thickness: 2);
            }
        }
    }

    private void DrawCosts() {
        Handles.color = Color.white;
        for (int row = 0; row < flowFields.Size; row++) {
            for (int column = 0; column < flowFields.Size; column++) {
                var worldPos = space.ConvertToWorld(new Vector2Int(row, column), atCenter: true);
                var integratedCost = flowFields.GetIntegratedCost(row, column);
                Handles.Label(worldPos, $"{integratedCost}");
            }
        }
    }

}