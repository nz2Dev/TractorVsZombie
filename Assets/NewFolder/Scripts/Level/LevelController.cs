using UnityEngine;

public class LevelController {
    
    private readonly LevelView view;
    private readonly PlayerController playerController;
    private readonly EnemyController enemyController;
    private readonly HeadquarterBuildingController headquarterBuildingController;

    private LevelModel model;

    public LevelController(LevelView view, PlayerController playerController, EnemyController enemyController, HeadquarterBuildingController headquarterBuildingController) {
        this.view = view;
        this.playerController = playerController;
        this.enemyController = enemyController;
        this.headquarterBuildingController = headquarterBuildingController;
    }

    public void Init(LevelPrototype levelPrototype) {
        headquarterBuildingController.Create(levelPrototype.headquarterBuildingPrototype); //?
        model = new LevelModel();
        model.PlayerPrototype = levelPrototype.playerPrototype;
        model.EnemyPrototype = levelPrototype.enemyPrototype;
        // view.ShowEnteringCutscene(levelPrototype.entranceCutscene);
        // model.InCutscene = true;
        OnLevelLoaded();
    }

    public void Update() {
        if (model.InCutscene && view.CutsceneFinished) {
            model.InCutscene = false;
            OnLevelLoaded();    
        }
    }

    private void OnLevelLoaded() {
        playerController.Setup(model.PlayerPrototype);
        enemyController.Setup(model.EnemyPrototype);
    }

}