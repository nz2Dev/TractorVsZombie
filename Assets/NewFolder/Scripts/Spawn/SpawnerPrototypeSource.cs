using UnityEngine;

public class SpawnerPrototypeSource : MonoBehaviour {
    
    [SerializeField] private SpawnerConfig configSource;
    [Inline, SerializeField] private SpawnPrototypeSource spawnPrototypeSource;

    public SpawnerPrototype Get() {
        return new SpawnerPrototype(
            config: configSource,
            spawnPrototype: spawnPrototypeSource.Get()
        );
    }
}