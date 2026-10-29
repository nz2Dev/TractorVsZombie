using System.Collections.Generic;

using UnityEngine;

public class RewardSpawnListSource : MonoBehaviour {
    [SerializeField] private List<RewardSpawnSource> rewardSpawns = new();
    
    public RewardSpawnPrototype[] GetPrototypes() {
        var rewardSpawnPrototypes = new RewardSpawnPrototype[rewardSpawns.Count];
        for (int i = 0; i < rewardSpawns.Count; i++) {
            rewardSpawnPrototypes[i] = rewardSpawns[i].GetPrototype();
        }
        return rewardSpawnPrototypes;
    }
}