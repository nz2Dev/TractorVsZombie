using System.Collections.Generic;

using UnityEngine;

public class RewardsView {
    
    private readonly GameObject rewardVisualsPrefab;
    private readonly Dictionary<int, GameObject> rewardVisuals = new ();
    
    public RewardsView(GameObject rewardVisualsPrefab) {
        this.rewardVisualsPrefab = rewardVisualsPrefab;
    }

    public void SpawnReward(int id, Vector3 position) {
        var visuals = GameObject.Instantiate(rewardVisualsPrefab, position, Quaternion.identity);
        rewardVisuals[id] = visuals;
    }

    public void DespawnReward(int id) {
        var visuals = rewardVisuals[id];
        Object.Destroy(visuals);
        rewardVisuals.Remove(id);
    }

}