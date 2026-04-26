using System;
using System.Collections.Generic;

using UnityEngine;

public class RewardController {
    
    private readonly RewardView view;

    private int idCounter;
    private readonly Dictionary<int, RewardModel> registry = new ();
    private SpatialLookup<RewardModel> spatialLookup = new(2048);
    private List<RewardState> rewardStateBuffer = new (128);

    public RewardController(RewardView view) {
        this.view = view;
    }

    public void Update() {
        UpdateSpatialLookup();
    }

    internal void Destroy() {
        spatialLookup.Dispose();
    }

    public IReadOnlyList<RewardState> CollectRewards(Vector3 position, float radius) {
        var rewardsInRange = spatialLookup.QueryRange(position, radius);
        rewardStateBuffer.Clear();
        foreach (var reward in rewardsInRange) {
            rewardStateBuffer.Add(GetRewardState(reward));
            DeleteReward(reward);
        }
        return rewardStateBuffer;
    }

    private RewardState GetRewardState(RewardModel model) {
        return new RewardState {
            Position = model.Position,
            RewardType = model.RewardType,
            LoadoutConfig = model.LoadoutConfig
        };
    }

    public void SpawnPointReward(Vector3 position) {
        var nextId = ++idCounter;
        var reward = new RewardModel { Id = nextId, Position = position, RewardType = RewardType.Points };
        registry[reward.Id] = reward;
        view.SpawnPointReward(reward.Id, reward.Position);
    }

    public void SpawnLoadoutReward(Vector3 position, LoadoutConfig loadout) {
        var nextId = ++idCounter;
        var reward = new RewardModel { Id = nextId, Position = position, 
            RewardType = RewardType.Loadout,
            LoadoutConfig = loadout
        };
        registry[reward.Id] = reward;
        view.SpawnLoadoutReward(reward.Id, reward.Position, loadout.brokenVisualsPrefab, loadout.weaponVisualsPrefab.gameObject, loadout.weaponLocalOffset);
    }

    private void DeleteReward(RewardModel reward) {
        registry.Remove(reward.Id);
        view.DespawnReward(reward.Id);
    }

    private void UpdateSpatialLookup() {
        spatialLookup.Reset();
        foreach (var reward in registry.Values) {
            spatialLookup.Add(reward);
        }
        spatialLookup.Fixate();
        if (spatialLookup.SourceCount > 0) {
            spatialLookup.ScheduleBuild().Complete();
        }
    }

}