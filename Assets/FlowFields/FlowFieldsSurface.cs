using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;

[ExecuteInEditMode]
public class FlowFieldsSurface : MonoBehaviour {
    
    private const float DefaultScale = 1;

    [SerializeField] private int size;
    [SerializeField] private Vector2Int[] blockedCells;
    [Space]
    [SerializeField] private bool bakeInRealTime;
    [SerializeField] private bool displayBlockers = true;
    [SerializeField] private bool displayComputations = true;
    [SerializeField] private bool costOrFieldsDisplay = true;
    [SerializeField] private bool updateGoalInEditor = true;

    private FlowFieldsSpace space;
    private FlowFields flowFields;
    private Vector3 goal;

    public int Size => flowFields.Size;
    public FlowFieldsSpace Space => space;
    public Vector2Int[] BlockedCells => blockedCells;
    public bool BakeInRealTime => bakeInRealTime;
    public bool DisplayBlockers => displayBlockers;
    public bool DisplayComputations => displayComputations;
    public bool CostOrFieldsDisplay => costOrFieldsDisplay;
    public bool UpdateGoalInEditor => updateGoalInEditor;

    private void OnValidate() {
        DefineSpace();
        UpdateFields();
    }

    private void Awake() {
        DefineSpace();
        UpdateFields();
    }

    private void DefineSpace() {
        space = new FlowFieldsSpace(size, DefaultScale);
    }

    public void BakeBlockers() {
        var colliders = FindObjectsByType<Collider>(FindObjectsSortMode.None);
        var cellsSet = new HashSet<Vector2Int>(size * size);

        foreach (var collider in colliders) {
            if (collider.gameObject.layer == 11 /* walls */) {
                CellRaycaster.ColliderCast(collider, space, cellsSet);
            }
        }

        blockedCells = cellsSet.ToArray();
        UpdateFields();
        UpdateGoal();
    }

    public void SetGoal(Vector3 goalPosition) {
        goal = goalPosition;
        UpdateGoal();
    }

    public Vector3 GetGridPosition(int x, int y, bool atCenter = true) {
        return space.ConvertToWorld(new Vector2Int(x, y), atCenter);
    }

    public int GetIntegratedCost(int x, int y) {
        return flowFields.GetIntegratedCost(x, y);
    }

    public Vector3 GetFlowVector(int x, int y) {
        var gridVector = flowFields.GetFlowVector(x, y);
        return new Vector3(gridVector.x, 0, gridVector.y).normalized;
    }

    private void UpdateFields() {
        flowFields = new FlowFields();
        flowFields.SetGrid(size);

        foreach (var blocked in blockedCells) {
            flowFields.SetCellBlocked(blocked.x, blocked.y, true);
        }
    }

    private void UpdateGoal() {
        var goalLocation = space.ConvertToGrid(goal);
        flowFields.ComputeCosts(goalLocation);
        flowFields.ComputeFlow();
    }

}