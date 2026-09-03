using System.Collections.Generic;

using UnityEngine;

public class FlowFieldHandle {
    internal FlowField flowField;
    internal Vector3 goal;
    internal bool computeIsDirty;

    public Vector3 Goal => goal;
}