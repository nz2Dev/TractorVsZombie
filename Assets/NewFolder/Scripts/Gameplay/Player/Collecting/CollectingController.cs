
using UnityEngine;

public class CollectingController {

    private readonly RewardController rewardController;
    private readonly AssemblingController assemblingController;
    private readonly LoaderController loaderController;
    private CollectingConfig config;

    public CollectingController(RewardController rewardController, AssemblingController assemblingController, LoaderController loaderController) {
        this.rewardController = rewardController;
        this.assemblingController = assemblingController;
        this.loaderController = loaderController;
    }

    public void Init(CollectingPrototype prototype) {
        this.config = prototype.config;
    }
    
    public void Update() {
        CollectRewards();
    }

    private void CollectRewards() {
        var position = assemblingController.HeadPosition;
        var rewardsInRadius = rewardController.CheckRewards(position, config.radius);
        foreach (var rewardId in rewardsInRadius) {
            var rewardState = rewardController.ReadState(rewardId);
            if (rewardState.payload.type == RewardType.Loadout) {
                if (loaderController.TryPlace(rewardState.payload.loadoutPrototype)) {
                    rewardController.CollectReward(rewardId);
                }
            } else if (rewardState.payload.type == RewardType.Platform) {
                assemblingController.AddPlatform(rewardState.position, config.startOrEndCouplingOfRewards);
                rewardController.CollectReward(rewardId);
            } else {
                rewardController.CollectReward(rewardId);
            }
        }
    }

}
