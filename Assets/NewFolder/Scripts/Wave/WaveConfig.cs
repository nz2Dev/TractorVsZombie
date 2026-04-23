using UnityEngine;

[CreateAssetMenu(fileName = "WaveConfig", menuName = "WaveConfig", order = 0)]
public class WaveConfig : ScriptableObject {
    public int spawnInterval;
    public int initialQueue;
    public SpawnType spawnType;
    public SpawnConfig spawnConfig;
    public SpawnShape spawnShape;
}