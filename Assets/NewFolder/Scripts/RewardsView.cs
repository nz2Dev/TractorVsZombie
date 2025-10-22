using System.Collections.Generic;

using UnityEngine;

public class RewardsView {
    
    private readonly Dictionary<int, GameObject> rewardVisualsRegistry = new ();
    
    public RewardsView() {
    }

    public void SpawnReward(int id, Vector3 position, GameObject rewardVisualsPrefab) {
        var visuals = GameObject.Instantiate(rewardVisualsPrefab, position, Quaternion.identity);
        rewardVisualsRegistry[id] = visuals;
    }

    public void DespawnReward(int id) {
        var visuals = rewardVisualsRegistry[id];
        Object.Destroy(visuals);
        rewardVisualsRegistry.Remove(id);
    }

}