using System.Collections.Generic;
using UnityEngine;

public class LoadoutView {
    
    private readonly Dictionary<int, LoadoutVisuals> visualsRegistry = new ();

    public void AddLoadout(int id, Vector3 position, LoadoutVisuals visualsPrefab) {
        var visuals = GameObject.Instantiate(visualsPrefab, position, Quaternion.identity);
        visualsRegistry[id] = visuals;
    }

    public void UpdateTransforms(int id, Vector3 position, Quaternion rotation) {
        visualsRegistry[id].transform.SetPositionAndRotation(position, rotation);
    }

    public void ShowTakeHit(int id) {
        visualsRegistry[id].TriggerHitFlash();
    }

    public void RemoveLoadout(int id) {
        visualsRegistry.Remove(id, out var visuals);
        GameObject.Destroy(visuals.gameObject);
    }
    
}
