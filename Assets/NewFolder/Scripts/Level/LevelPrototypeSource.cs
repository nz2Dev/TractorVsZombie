using UnityEngine;
using UnityEngine.Playables;

public class LevelPrototypeSource : MonoBehaviour {
    
    [SerializeField] private PlayableDirector entranceCutscene;
    [SerializeField] private PlayerPrototypeSource playerPrototypeSource;
    [SerializeField] private EnemyPrototypeSource enemyPrototypeSource;
    [SerializeField] private HeadquarterBuildingSource headquarterBuildingSource;

    [ContextMenu("Find In Scene")]
    public void FindInScene() {
        playerPrototypeSource = GameObject.FindFirstObjectByType<PlayerPrototypeSource>(FindObjectsInactive.Include);
        enemyPrototypeSource = GameObject.FindFirstObjectByType<EnemyPrototypeSource>(FindObjectsInactive.Include);
        headquarterBuildingSource = GameObject.FindFirstObjectByType<HeadquarterBuildingSource>(FindObjectsInactive.Include);
    }
    
    public LevelPrototype Get() {
        return new LevelPrototype {
            entranceCutscene = entranceCutscene,
            enemyPrototype = enemyPrototypeSource.Get(),
            playerPrototype = playerPrototypeSource.Get(),
            headquarterBuildingPrototype = headquarterBuildingSource.GetPrototype(),
        };
    }
}