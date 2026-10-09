using System;
using System.Collections.Generic;

using UnityEngine;

public class RewardView {

    private readonly Dictionary<int, GameObject> visualsRegistry = new ();

    public RewardView() {
    }

    internal void SpawnReward(int id, Vector3 position, Quaternion rotation, GameObject visualsPrefab) {
        var visuals = GameObject.Instantiate(visualsPrefab, position, rotation);
        visualsRegistry[id] = visuals;
    }

    public void DespawnReward(int id) {
        var visuals = visualsRegistry[id];
        GameObject.Destroy(visuals);
        visualsRegistry.Remove(id);
    }

}