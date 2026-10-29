using UnityEngine;
using UnityEngine.Playables;

public struct LevelPrototype {
    public PlayableDirector entranceCutscene;
    public HeadquarterBuildingPrototype headquarterBuildingPrototype;
    public EnemyPrototype enemyPrototype;
    public PlayerPrototype playerPrototype;
    public Vector3 startPosition;
    public RewardSpawnPrototype[] rewardSpawns;

    public LevelPrototype(PlayableDirector entranceCutscene, HeadquarterBuildingPrototype headquarterBuildingPrototype, EnemyPrototype enemyPrototype, PlayerPrototype playerPrototype, Vector3 startPosition, RewardSpawnPrototype[] rewardSpawns) {
        this.entranceCutscene = entranceCutscene;
        this.headquarterBuildingPrototype = headquarterBuildingPrototype;
        this.enemyPrototype = enemyPrototype;
        this.playerPrototype = playerPrototype;
        this.startPosition = startPosition;
        this.rewardSpawns = rewardSpawns;
    }
}
