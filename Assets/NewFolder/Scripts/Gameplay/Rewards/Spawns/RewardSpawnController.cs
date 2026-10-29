using System.Collections.Generic;

using UnityEngine;

public class RewardSpawnController {

    private readonly RewardController rewardController;
    private readonly PlayerController playerController;

    private List<RewardSpawnPrototype> spawns = new ();
    private float startTime;

    public RewardSpawnController(RewardController rewardController, PlayerController playerController) {
        this.rewardController = rewardController;
        this.playerController = playerController;
    }

    public void Setup(RewardSpawnPrototype[] prototypes) {
        spawns.AddRange(prototypes);
        startTime = Time.time;
    }

    public void Update() {
        for (int i = spawns.Count - 1; i >= 0; i--) {
            if (!ShouldSpawn(spawns[i]))
                continue;

            rewardController.Create(spawns[i].reward, rotation: spawns[i].rotation);
            spawns.RemoveAt(i);
        }
    }

    private bool ShouldSpawn(RewardSpawnPrototype spawn) {
        if (spawn.rule == RewardSpawnRule.DelayAfterLevelStart)
            return Time.time - startTime >= Mathf.Max(0, spawn.delaySeconds);

        float distance = Mathf.Max(0, spawn.playerDistance);
        return playerController.HasPosition &&
               (playerController.Position - spawn.reward.position).sqrMagnitude <= distance * distance;
    }
}
