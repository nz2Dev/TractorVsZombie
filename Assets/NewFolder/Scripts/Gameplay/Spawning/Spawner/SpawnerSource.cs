using UnityEngine;
public class SpawnerSource : MonoBehaviour {
    
    [SerializeField] private SpawnerConfig configSource;
    [Inline, SerializeField] private SpawnPrototypeSource spawnPrototypeSource;

    public SpawnerPrototype Get() {
        return new SpawnerPrototype(
            config: configSource,
            spawnPrototype: spawnPrototypeSource.Get()
        );
    }
}