using UnityEngine;

[CreateAssetMenu(fileName = "SquadAIConfig", menuName = "SquadAIConfig", order = 0)]
public class SquadAIConfig : ScriptableObject {
    [Range(0, 1)] public float formationBlendFactor = 0.3f;
    public CohesionConfig cohesionConfig;
}