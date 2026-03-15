using UnityEngine;

[CreateAssetMenu(fileName = "SquadAIConfig", menuName = "SquadAIConfig", order = 0)]
public class SquadAIConfig : ScriptableObject {
    [Range(0, 1)] public float formationBlendFactor = 0.3f;
    [Range(0, 1)] public float coheseSpeedAdjustFactor = 0.4f;
    [Range(0, 1)] public float coheseSpeedAdjustMinClamped = 0.5f;
}