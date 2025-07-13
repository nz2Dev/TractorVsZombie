using System;

using UnityEditor;

using UnityEngine;

public class FlowFieldsDebugger : MonoBehaviour {

    [SerializeField] private FlowFieldsGrid grid;
    [SerializeField] private bool updateGoalInEditor = true;
    [SerializeField] private bool costOrFields = true;

    private Vector2Int goal;
    private FlowFields flowFields = new FlowFields();

    private void UpdateFields() {
        flowFields = new FlowFields();
        grid.UpdateCells(flowFields);
        flowFields.ComputeCosts(goal);
        flowFields.ComputeFlow();
    }

    private void OnDrawGizmos() {
        if (updateGoalInEditor) {
            goal = grid.ConvertToGrid(transform.position);
            UpdateFields();
        }

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
                var pointArrow = new Vector3(flowVector.x, 0, flowVector.y).normalized * 0.5f;
                Handles.DrawLine(worldPos, worldPos + pointArrow, thickness: 2);
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