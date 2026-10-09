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
            position = model.Position,
            payload = model.Payload
        };
    }

    public int Create(RewardPrototype prototype, Vector3 position = default, Quaternion rotation = default) {
        var nextId = ++idCounter;
        var model = new RewardModel(nextId, position == default ? prototype.position : position, prototype.payload);
        registry[nextId] = model;
        view.SpawnReward(nextId, model.Position, rotation, prototype.visualsPrefab);
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