using System;

using UnityEngine;

[Serializable]
public class RewardSpawnSource {

    public RewardSource reward;
    public RewardSpawnRule rule;
    [Min(0)] public float delaySeconds;
    [Min(0)] public float playerDistance = 10;

    public RewardSpawnPrototype GetPrototype() {
        return new RewardSpawnPrototype {
            reward = reward.GetPrototype(),
            rotation = reward.transform.rotation,
            rule = rule,
            delaySeconds = delaySeconds,
            playerDistance = playerDistance,
        };
    }
}
