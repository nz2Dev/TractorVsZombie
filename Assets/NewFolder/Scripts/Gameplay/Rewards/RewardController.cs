using System;
using System.Collections.Generic;

using UnityEngine;

public class RewardController {
    
    private readonly RewardView view;

    private int idCounter;
    private readonly Dictionary<int, RewardModel> registry = new ();
    private SpatialLookup<RewardModel> spatialLookup = new(2048);
    private List<int> rewardIdBuffer = new (128);

    public RewardController(RewardView view) {
        this.view = view;
    }

    public void Update() {
        UpdateSpatialLookup();
    }

    internal void Destroy() {
        spatialLookup.Dispose();
    }

    public IReadOnlyList<int> CheckRewards(Vector3 position, float radius) {
        var rewardsInRange = spatialLookup.QueryRange(position, radius);
        rewardIdBuffer.Clear();
        foreach (var reward in rewardsInRange) {
            rewardIdBuffer.Add(reward.Id);
        }
        return rewardIdBuffer;
    }

    public void CollectReward(int rewardId) {
        DeleteReward(registry[rewardId]);
    }

    public RewardState ReadState(int rewardId) {
        var model = registry[rewardId];
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