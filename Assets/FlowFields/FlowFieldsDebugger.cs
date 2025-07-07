using System;

using Codice.Client.Common.GameUI;

using UnityEditor;

using UnityEngine;

public class FlowFieldsDebugger : MonoBehaviour {

    [SerializeField] private int size;
    [SerializeField] private Vector2Int goal;
    [SerializeField] private bool costOrFields = true; 

    private FlowFields flowFields = new FlowFields();

    [ContextMenu("Update Fields")]
    private void UpdateFields() {
        flowFields = new FlowFields();
        flowFields.SetGrid(size);
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
                var flowVector = flowFields.GetFlowVector(row, column);
                var worldPos = new Vector3(row + 0.5f, 0, column + 0.5f);
                Handles.DrawLine(worldPos, worldPos + new Vector3(flowVector.x * 0.5f, 0, flowVector.y * 0.5f), thickness: 2);
            }
        }
    }

    private void DrawCosts() {
        for (int row = 0; row < flowFields.Size; row++) {
            for (int column = 0; column < flowFields.Size; column++) {
                var integratedCost = flowFields.GetIntegratedCost(row, column);
                var worldPos = new Vector3(row + 0.5f, 0, column + 0.5f);
                Handles.Label(worldPos, $"{integratedCost}");
            }
        }
    }
}