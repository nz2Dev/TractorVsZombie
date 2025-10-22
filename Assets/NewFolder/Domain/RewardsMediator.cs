using System.Collections.Generic;

using UnityEngine;

public enum RewardType {
    Points,
    TurelWeapon,
}

public struct RewardState {
    public int id;
    public Vector3 position;
    public RewardType rewardType;
    public GameObject rewardVisuals;
    public RewardConfigs configs;
}

public struct RewardConfigs {
    public TurelConfig turelConfig;
}

public class RewardsMediator {

    private class Reward {
        public int id;
        public SphereCollider marker;
        public int spawnFrame;
        public RewardType type;
        public GameObject visualsPrefab;
        public RewardConfigs configs;
    }
    
    private readonly int rewardLayer;
    private readonly LayerMask rewardsMask;

    private int rewardIdCounter;
    private readonly List<Reward> rewards = new ();
    private readonly Dictionary<Collider, Reward> colliderToReward = new ();
    private readonly Collider[] overlapBuffer = new Collider[512];

    private readonly List<RewardState> rewardSpawnedEvents = new();
    private readonly List<RewardState> rewardRemovedEvents = new();

    public RewardsMediator(int markersLayer) {
        this.rewardLayer = markersLayer;
        rewardsMask = 1 << markersLayer;
    }

    public List<RewardState> RewardAddedEvents => rewardSpawnedEvents;
    public List<RewardState> RewardRemovedEvents => rewardRemovedEvents;

    public void AddReward(Vector3 position, float radius, RewardType rewardType, GameObject visualsPrefab, RewardConfigs configs) {
        int nextRewardId = rewardIdCounter++;
        
        var marker = CreateRewardMarker(nextRewardId, position, radius);
        var reward = new Reward {
            id = nextRewardId, 
            marker = marker, 
            spawnFrame = Time.frameCount,
            type = rewardType,
            visualsPrefab = visualsPrefab,
            configs = configs,
        };
        
        rewards.Add(reward);
        colliderToReward[marker] = reward;
        
        rewardSpawnedEvents.Add(GetRewardState(reward));
    }

    public bool CollectRewards(Vector3 position, float radius, List<RewardState> rewards) {
        var rewardsCount = Physics.OverlapSphereNonAlloc(position, radius, overlapBuffer, rewardsMask);
        
        rewards.Clear();
        for (int i = 0; i < rewardsCount; i++) {
            var overlappedCollider = overlapBuffer[i];
            if (colliderToReward.TryGetValue(overlappedCollider, out var reward) && reward.spawnFrame != Time.frameCount) {
                rewards.Add(GetRewardState(reward));
                RemoveReward(reward);
            }
        }
        
        return rewards.Count > 0;
    }

    private void RemoveReward(Reward reward) {
        colliderToReward.Remove(reward.marker);
        Object.Destroy(reward.marker.gameObject);
        rewards.Remove(reward);
        rewardRemovedEvents.Add(GetRewardState(reward));
    }

    public void ClearEvents() {
        rewardSpawnedEvents.Clear();
        rewardRemovedEvents.Clear();
    }

    private RewardState GetRewardState(Reward reward) {
        return new RewardState {
            id = reward.id,
            position = reward.marker.transform.position,
            rewardType = reward.type,
            rewardVisuals = reward.visualsPrefab,
            configs = reward.configs,
        };
    }

    private SphereCollider CreateRewardMarker(int nextRewardId, Vector3 position, float radius) {
        var gameObject = new GameObject("reward " + nextRewardId, typeof(SphereCollider));
        gameObject.layer = rewardLayer;
        gameObject.transform.position = position;
        var collider = gameObject.GetComponent<SphereCollider>();
        collider.isTrigger = true;
        collider.radius = radius;
        return collider;
    }

}