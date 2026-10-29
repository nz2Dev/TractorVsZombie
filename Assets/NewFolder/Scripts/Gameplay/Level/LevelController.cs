using UnityEngine;

public class LevelController {
    
    private readonly LevelView view;
    private readonly PlayerController playerController;
    private readonly EnemyController enemyController;
    private readonly HeadquarterBuildingController headquarterBuildingController;
    private readonly RewardSpawnController rewardSpawnController;

    private LevelModel model;
    private LevelPrototype prototype;

    public LevelController(LevelView view, PlayerController playerController, EnemyController enemyController, HeadquarterBuildingController headquarterBuildingController, RewardSpawnController rewardSpawnController) {
        this.view = view;
        this.playerController = playerController;
        this.enemyController = enemyController;
        this.headquarterBuildingController = headquarterBuildingController;
        this.rewardSpawnController = rewardSpawnController;
    }

    public void Init(LevelPrototype levelPrototype) {
        this.prototype = levelPrototype;
        model = new LevelModel();
        
        headquarterBuildingController.Create(levelPrototype.headquarterBuildingPrototype); //?
        view.ShowEnteringCutscene(levelPrototype.entranceCutscene, levelPrototype.startPosition);
        model.InCutscene = true;
        view.ShowPlayerUI();
    }

    public void Update() {
        if (model.InCutscene && view.CutsceneFinished) {
            model.InCutscene = false;
            OnLevelLoaded();
        }

        if (model.LevelStarted && (headquarterBuildingController.IsDestroyed() || !playerController.HasUnits)) {
            view.ShowGameOverUI();
        }
    }

    private void OnLevelLoaded() {
        model.LevelStarted = true;
        playerController.Setup(prototype.playerPrototype);
        enemyController.Setup(prototype.enemyPrototype);
        rewardSpawnController.Setup(prototype.rewardSpawns);
    }

}
