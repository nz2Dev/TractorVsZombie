using System;

using UnityEditor;

using UnityEngine;

public class FlowFieldsDebugger : MonoBehaviour {

    [SerializeField] private Vector2Int goal;
    [SerializeField] private FlowFieldsGrid grid;
    [SerializeField] private bool costOrFields = true;

    private FlowFields flowFields = new FlowFields();

    [ContextMenu("Update Fields")]
    private void UpdateFields() {
        flowFields = new FlowFields();
        grid.UpdateCells(flowFields);
        flowFields.ComputeCosts(goal);
        flowFields.ComputeFlow();
    }

    private void OnDrawGizmos() {
        if (costOrFields) {
            DrawCosts();
        } else {
            DrawFields();
        }
    }

    private void DrawFields() {
        for (int row = 0; row < flowFields.Size; row++) {
            for (int column = 0; column < flowFields.Size; column++) {
                var worldPos = grid.ConvertToWorld(new Vector2Int(row, column), atCenter: true);
                var flowVector = flowFields.GetFlowVector(row, column);
                Handles.DrawLine(worldPos, worldPos + new Vector3(flowVector.x * 0.5f, 0, flowVector.y * 0.5f), thickness: 2);
            }
        }
    }

    private void DrawCosts() {
        for (int row = 0; row < flowFields.Size; row++) {
            for (int column = 0; column < flowFields.Size; column++) {
                var worldPos = grid.ConvertToWorld(new Vector2Int(row, column), atCenter: true);
                var integratedCost = flowFields.GetIntegratedCost(row, column);
                Handles.Label(worldPos, $"{integratedCost}");
            }
        }
    }
}