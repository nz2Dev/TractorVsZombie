using UnityEngine;

public class WaveSource : MonoBehaviour {
    
    [SerializeField] private WaveConfig waveConfig;
    [SerializeField] private SpawnPointSource spawnPointSource;

    public WavePrototype GetPrototype() {
        return new WavePrototype {
            waveConfig = waveConfig,
            waveSpawnPoint = spawnPointSource.Provide(),
        };
    }

}