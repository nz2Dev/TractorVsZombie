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
            BrokenArmorVisualsPrefab = model.BrokenArmorVisualsPrefab,
            WeaponConfig = model.WeaponConfig,
            WeaponOffset = model.WeaponOffset
        };
    }

    public void SpawnPointReward(Vector3 position) {
        var nextId = ++idCounter;
        var reward = new RewardModel { Id = nextId, Position = position, RewardType = RewardType.Points };
        registry[reward.Id] = reward;
        view.SpawnPointReward(reward.Id, reward.Position);
    }

    public void SpawnBrokenArmorReward(Vector3 position, GameObject brokenArmorVisualsPrefab, WeaponConfig weaponConfig, Vector3 weaponVisualsOffset) {
        var nextId = ++idCounter;
        var reward = new RewardModel { Id = nextId, Position = position, 
            RewardType = RewardType.BrokenArmor,
            BrokenArmorVisualsPrefab = brokenArmorVisualsPrefab, 
            WeaponConfig = weaponConfig,
            WeaponOffset = weaponVisualsOffset
        };
        registry[reward.Id] = reward;
        view.SpawnBrokenArmorReward(reward.Id, reward.Position, brokenArmorVisualsPrefab, weaponConfig.visualsPrefab.gameObject, weaponVisualsOffset);
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