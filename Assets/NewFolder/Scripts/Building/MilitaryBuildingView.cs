using System;
using System.Collections.Generic;

using UnityEngine;

public class MilitaryBuildingView {

    private readonly Dictionary<int, GameObject> registry = new();

    internal void AddVisuals(int id, Vector3 position, Quaternion rotation, GameObject visualsPrefab) {
        var visuals = GameObject.Instantiate(visualsPrefab, position, rotation);
        registry[id] = visuals;
    }

    internal void RemoveVisuals(int id) {
        registry.Remove(id, out var visuals);
        GameObject.Destroy(visuals);
    }
    
}