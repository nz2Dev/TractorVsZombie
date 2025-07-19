using System.Collections.Generic;

using UnityEngine;

public class CrowdView {

    private Dictionary<int, GameObject> unitVisuals = new Dictionary<int, GameObject>();

    private readonly GameObject visualsPrefab;

    public CrowdView(GameObject visualsPrefab = null) {
        this.visualsPrefab = visualsPrefab;
    }

    public void AddUnit(int id, Vector3 position) {
        if (unitVisuals.ContainsKey(id))
            return;

        GameObject visual;
        if (visualsPrefab != null) {
            visual = Object.Instantiate(visualsPrefab, position, Quaternion.identity);
        } else {
            visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            visual.transform.position = position;
        }
        
        unitVisuals[id] = visual;
    }

    public void UpdateUnitPositionAndRotation(int id, Vector3 position, Quaternion rotation) {
        if (unitVisuals.TryGetValue(id, out var visual)) {
            visual.transform.SetPositionAndRotation(position, rotation);
        }
    }

}