using UnityEngine;

[CreateAssetMenu(fileName = "SpawnerConfig", menuName = "SpawnerConfig", order = 0)]
public class SpawnerConfig : ScriptableObject {
    public int spawnInterval;
    public int initialQueue;
}