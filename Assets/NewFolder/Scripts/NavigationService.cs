using System;

using UnityEngine;

public class NavigationService {

    private FlowFields flowFields;

    public NavigationService() {
        flowFields = new FlowFields();
    }

    public void SetupFlowField(int sizeBounds, int density, object obstacles) {
        flowFields.SetGrid(sizeBounds);
    }

    public Vector3 GetFlowVector(Vector3 worldSpacePosition) {
        return Vector3.zero;
    }

    public void SetGoal(Vector3 worldSpacePosition) {
    }
}