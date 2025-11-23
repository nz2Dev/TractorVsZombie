using System;
using System.Collections.Generic;

public class RewardController {
    
    private readonly RewardView view;
    private readonly RewardsMediator rewardsMediator;
    private readonly PlayerController playerController;
    private readonly InfantryController infantryController;
    private readonly ArmorController armorController;

    private int idCounter;
    private Dictionary<int, RewardModel> registry = new ();

    public RewardController(RewardView view, RewardsMediator rewardsMediator, PlayerController playerController, InfantryController infantryController, ArmorController armorController) {
        this.view = view;
        this.rewardsMediator = rewardsMediator;
        this.playerController = playerController;
        this.infantryController = infantryController;
        this.armorController = armorController;
    }

    public void Update() {
        DiscoverRewards();
        CollectRewards();
    }

    private void DiscoverRewards() {
        foreach (var infantry in infantryController.DiedInfantry) {
            SpawnPointReward(infantry);
        }
        infantryController.ClearDiedRegistry();
        
        foreach (var diedArmor in armorController.DiedArmor) {
            SpawnWeaponReward(diedArmor);
        }
        infantryController.ClearDiedRegistry();
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

    private List<int> rewardsBuffer = new (10);

    private void CollectRewards() {
        var playerPosition = playerController.GetPlayerPosition();
        if (rewardsMediator.CollectRewardsPoints(playerPosition, 0.5f, rewardsBuffer)) {
            foreach (var spatialId in rewardsBuffer) {
                var reward = registry[spatialId];
                if (reward.RewardType == RewardType.Weapon) {
                    playerController.SpawnHostWithWeapon(reward.Position, reward.WeaponConfig);
                }
                DeleteReward(spatialId);
            }
        }
    }

    private void DeleteReward(int spatialId) {
        var reward = registry[spatialId];
        registry.Remove(spatialId);
        view.DespawnReward(reward.Id);
    }

}