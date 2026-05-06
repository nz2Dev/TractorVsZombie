using UnityEngine;
using UnityEngine.Playables;

public class LevelPrototypeSource : MonoBehaviour {
    
    [SerializeField] private PlayableDirector entranceCutscene;
    
    public LevelPrototype Get() {
        var enemySource = GameObject.FindFirstObjectByType<EnemyPrototypeSource>(FindObjectsInactive.Include);
        var playerSource = GameObject.FindFirstObjectByType<PlayerPrototypeSource>(FindObjectsInactive.Include);
        var headquarterSource = GameObject.FindFirstObjectByType<HeadquarterBuildingSource>(FindObjectsInactive.Include);
        return new LevelPrototype {
            entranceCutscene = entranceCutscene,
            enemyPrototype = enemySource.Get(),
            playerPrototype = playerSource.Get(),
            headquarterBuildingPrototype = headquarterSource.GetPrototype(),
        };
    }
}