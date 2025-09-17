using System.Collections.Generic;

using UnityEngine;

public class UnitView {

    private Dictionary<int, GameObject> unitVisuals = new Dictionary<int, GameObject>();

    private readonly GameObject visualsPrefab;
    private List<int> postponedDeletions = new ();

    public UnitView(GameObject visualsPrefab = null) {
        this.visualsPrefab = visualsPrefab;
    }

    public void UpdateView() {
        for (int i = 0; i < postponedDeletions.Count; i++) {
            var unitId = postponedDeletions[i];
            var visuals = unitVisuals[unitId];
            if (!visuals.GetComponent<Animation>().isPlaying) {
                Object.Destroy(visuals);
                unitVisuals.Remove(unitId);
                postponedDeletions.RemoveAt(i);
                i--;
            }
        }
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

    public void ShowFinalBlow(int unitId, Vector3 damageSourcePosition) {
        if (unitVisuals.TryGetValue(unitId, out var visuls)) {
            var shootVector = visuls.transform.position - damageSourcePosition;
            visuls.transform.rotation = Quaternion.LookRotation(-shootVector.normalized, Vector3.up);
            var animation = visuls.GetComponent<Animation>();
            animation.Play("Death Animation", PlayMode.StopAll);
        }
    }

    public void RemoveUnit(int id) {
        if (unitVisuals.TryGetValue(id, out var visual)) {
            var animation = visual.GetComponent<Animation>();
            bool isDeathPlaying = animation.IsPlaying("Death Animation");
            if (isDeathPlaying) {
                postponedDeletions.Add(id);
            } else {
                Object.Destroy(visual);
                unitVisuals.Remove(id);
            }
        }
    }

}