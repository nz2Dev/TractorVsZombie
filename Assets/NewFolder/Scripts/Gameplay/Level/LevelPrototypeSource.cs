using UnityEngine;
using UnityEngine.Playables;

public class LevelPrototypeSource : MonoBehaviour {
    
    [SerializeField] private PlayableDirector entranceCutscene;
    [SerializeField] private Transform cutscenePlatformContainers;
    [Space]
    [SerializeField] private PlayerPrototypeSource playerPrototypeSource;
    [SerializeField] private EnemyPrototypeSource enemyPrototypeSource;
    [SerializeField] private HeadquarterBuildingSource headquarterBuildingSource;
    [SerializeField] private Transform startTransform;

    [ContextMenu("Find In Scene")]
    public void FindInScene() {
        playerPrototypeSource = GameObject.FindFirstObjectByType<PlayerPrototypeSource>(FindObjectsInactive.Include);
        enemyPrototypeSource = GameObject.FindFirstObjectByType<EnemyPrototypeSource>(FindObjectsInactive.Include);
        headquarterBuildingSource = GameObject.FindFirstObjectByType<HeadquarterBuildingSource>(FindObjectsInactive.Include);
    }

    [ContextMenu("Mirror Player Prototype")]
    public void MirrorPlayerPrototype() {
        while (cutscenePlatformContainers.childCount > 0)
            DestroyImmediate(cutscenePlatformContainers.GetChild(0).gameObject);
        
        for (int i = 0; i < playerPrototypeSource.LoadoutsLength; i++) {
            var platformVisuals = Instantiate(playerPrototypeSource.PlatformPrototype.visualsPrefab, cutscenePlatformContainers);
            platformVisuals.transform.localPosition = (i + 1) * 6 * -cutscenePlatformContainers.transform.forward;
            var loadoutVisuals = Instantiate(playerPrototypeSource.GetLoadoutPrototype(i).shellVisualsPrefab, platformVisuals.transform);
            loadoutVisuals.transform.localPosition = playerPrototypeSource.PlatformPrototype.loadoutOffset;
        }
    }
    
    public LevelPrototype Get() {
        return new LevelPrototype (
            entranceCutscene: entranceCutscene,
            enemyPrototype: enemyPrototypeSource.Get(),
            playerPrototype: playerPrototypeSource.Get(),
            headquarterBuildingPrototype: headquarterBuildingSource.GetPrototype(),
            startPosition: startTransform.position
        );
    }
}