
using System;

using UnityEngine;

public class CollectingController {

    private readonly RewardController rewardController;

    private Vector3 position;
    private CollectingConfig config;

    public event Action<Vector3, LoadoutPrototype> OnLoadoutCollected;

    public CollectingController(RewardController rewardController) {
        this.rewardController = rewardController;
    }

    public void Init(CollectingPrototype prototype) {
        this.config = prototype.config;
    }
    
    public void SetPosition(Vector3 collectPosition) {
        this.position = collectPosition;
    }

    public void Update() {
        CollectRewards();
    }

    private void CollectRewards() {
        var collectedRewardStates = rewardController.CollectRewards(position, config.radius);
        foreach (var rewardState in collectedRewardStates) {
            if (rewardState.payload.type == RewardType.Loadout) {
                OnLoadoutCollected?.Invoke(rewardState.position, rewardState.payload.loadoutPrototype);
            }
        }
    }
}