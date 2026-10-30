
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
            rewardController.CollectReward(rewardId);            
            if (rewardState.payload.type == RewardType.Loadout) {
                loaderController.Place(rewardState.payload.loadoutPrototype);
            } else if (rewardState.payload.type == RewardType.Platform) {
                assemblingController.AddPlatform(rewardState.position, config.startOrEndCouplingOfRewards);
            }
        }
    }
}
