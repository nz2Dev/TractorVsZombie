using UnityEngine;

public class WaveSource : MonoBehaviour {
    
    [SerializeField] private WaveConfig waveConfig;
    [SerializeField] private SpawnPointSource spawnPointSource;
    [SerializeField] private SpawnShape spawnShapePrefab;

    public WavePrototype GetPrototype() {
        return new WavePrototype {
            waveConfig = waveConfig,
            waveSpawnPointA = spawnPointSource.Provide(),
            waveSpawnShapeA = spawnShapePrefab,
        };
    }

}