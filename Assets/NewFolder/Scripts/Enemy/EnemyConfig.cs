using UnityEngine;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "EnemyConfig", order = 0)]
public class EnemyConfig : ScriptableObject {
    public int maxInfantryCount;
    public SquadAIConfig squadAIConfig;
    public int maxArmorCount;
}