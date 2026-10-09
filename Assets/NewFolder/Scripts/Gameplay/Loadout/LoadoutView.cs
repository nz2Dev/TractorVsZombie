using System.Collections.Generic;
using UnityEngine;

public class LoadoutView {
    
    private readonly Dictionary<int, GameObject> shellVisualsRegistry = new ();

    public void AddLoadout(int id, Vector3 position, GameObject shellVisualsPrefab) {
        if (shellVisualsPrefab == null) return;
        var visuals = GameObject.Instantiate(shellVisualsPrefab, position, Quaternion.identity);
        shellVisualsRegistry[id] = visuals;
    }

    public void UpdateTransforms(int id, Vector3 position, Quaternion rotation) {
        if (shellVisualsRegistry.TryGetValue(id, out var visuals)) {
            visuals.transform.SetPositionAndRotation(position, rotation);
        }
    }

    public void RemoveLoadout(int id) {
        if (shellVisualsRegistry.TryGetValue(id, out var visuals)) {
            GameObject.Destroy(visuals);
            shellVisualsRegistry.Remove(id);
        }
    }
}
