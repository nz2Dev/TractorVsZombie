using System.Collections.Generic;

using UnityEngine;

public class UnitView {

    private Dictionary<int, GameObject> unitVisuals = new Dictionary<int, GameObject>();

    private readonly GameObject visualsPrefab;

    public UnitView(GameObject visualsPrefab = null) {
        this.visualsPrefab = visualsPrefab;
    }

    public void AddUnit(int id, Vector3 position) {
        if (unitVisuals.ContainsKey(id))
            return;

        GameObject visual;
        if (visualsPrefab != null) {
            visual = Object.Instantiate(visualsPrefab, position, Quaternion.identity);
            var renderer = visual.GetComponentInChildren<Renderer>();
            renderer.material = new Material(renderer.sharedMaterial);
        } else {
            visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Object.Destroy(visual.GetComponent<SphereCollider>());
            visual.transform.position = position;
        }
        
        unitVisuals[id] = visual;
    }

    public void UpdateUnitPositionAndRotation(int id, Vector3 position, Quaternion rotation) {
        if (unitVisuals.TryGetValue(id, out var visual)) {
            visual.transform.SetPositionAndRotation(position, rotation);
        }
    }

    public void ShowDirectFrontAttack(int unitId) {
        if (unitVisuals.TryGetValue(unitId, out var visuals)) {
            var animation = visuals.GetComponent<Animation>();
            animation.Play("Attack Animation");
            animation.PlayQueued("Walk Animation");
        }
    }

    public void RemoveUnit(int id) {
        if (unitVisuals.TryGetValue(id, out var visual)) {
            Object.Destroy(visual);
            unitVisuals.Remove(id);
        }
    }

}