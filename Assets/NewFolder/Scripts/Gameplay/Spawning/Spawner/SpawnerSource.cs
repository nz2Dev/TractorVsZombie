using UnityEngine;
public class SpawnerSource : MonoBehaviour {
    
    [SerializeField] private SpawnerConfig configSource;
    [Inline, SerializeField] private SpawnSource spawnSource;

    public SpawnerPrototype Get() {
        return new SpawnerPrototype(
            config: configSource,
            spawnPrototype: spawnSource.Get()
        );
    }
}