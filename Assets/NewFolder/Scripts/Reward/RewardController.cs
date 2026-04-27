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
            LoadoutPrototype = model.LoadoutPrototype
        };
    }

    public int SpawnReward(RewardPrototype prototype) {
        var nextId = ++idCounter;
        var model = new RewardModel(nextId, prototype.position, prototype.type, prototype.loadoutPrototype);
        registry[nextId] = model;
        view.SpawnReward(nextId, prototype.position, prototype.visualsPrefab);
        return nextId;
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