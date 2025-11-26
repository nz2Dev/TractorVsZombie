using System;
using System.Collections.Generic;

using UnityEngine;

public class RewardController {
    
    private readonly RewardView view;
    private readonly RewardsMediator rewardsMediator;
    private readonly InfantryController infantryController;
    private readonly ArmorController armorController;

    private int idCounter;
    private readonly Dictionary<int, RewardModel> registry = new ();
    private readonly List<int> rewardsSpatialIdBuffer = new (10);
    private readonly List<RewardState> rewardsStatesBuffer = new(10);

    public RewardController(RewardView view, RewardsMediator rewardsMediator, InfantryController infantryController, ArmorController armorController) {
        this.view = view;
        this.rewardsMediator = rewardsMediator;
        this.infantryController = infantryController;
        this.armorController = armorController;
    }

    public void Update() {
        DiscoverRewards();
    }

    public IReadOnlyList<RewardState> CollectRewards(Vector3 position, float radius) {
        rewardsStatesBuffer.Clear();
        if (rewardsMediator.CollectRewardsPoints(position, radius, rewardsSpatialIdBuffer)) {
            foreach (var spatialId in rewardsSpatialIdBuffer) {
                var reward = registry[spatialId];
                rewardsStatesBuffer.Add(GetRewardState(reward));
                DeleteReward(spatialId);
            }
        }
        return rewardsStatesBuffer;
    }

    private RewardState GetRewardState(RewardModel model) {
        return new RewardState {
            Position = model.Position,
            RewardType = model.RewardType,
            WeaponConfig = model.WeaponConfig
        };
    }

    private void DiscoverRewards() {
        foreach (var infantry in infantryController.DiedInfantry) {
            SpawnPointReward(infantry);
        }
        infantryController.ClearDiedRegistry();
        
        foreach (var diedArmor in armorController.DiedArmor) {
            SpawnWeaponReward(diedArmor);
        }
        armorController.ClearDiedRegistry();
    }

    private void SpawnPointReward(InfantryModel diedInfantry) {
        var nextId = ++idCounter;
        var reward = new RewardModel { Id = nextId, Position = diedInfantry.Position, RewardType = RewardType.Points, WeaponConfig = null };
        reward.SpatialId = rewardsMediator.AddRewardPoint(reward.Position, 2f);
        registry[reward.SpatialId] = reward;
        view.SpawnPointReward(reward.Id, reward.Position);
    }

    private void SpawnWeaponReward(ArmorModel diedArmor) {
        var nextId = ++idCounter;
        var reward = new RewardModel { Id = nextId, Position = diedArmor.Position, RewardType = RewardType.Weapon, WeaponConfig = diedArmor.WeaponConfig };
        reward.SpatialId = rewardsMediator.AddRewardPoint(reward.Position, 2f);
        registry[reward.SpatialId] = reward;
        view.SpawnReward(reward.Id, reward.Position, reward.WeaponConfig.visualsPrefab.gameObject);
    }

    private void DeleteReward(int spatialId) {
        var reward = registry[spatialId];
        registry.Remove(spatialId);
        view.DespawnReward(reward.Id);
    }

}