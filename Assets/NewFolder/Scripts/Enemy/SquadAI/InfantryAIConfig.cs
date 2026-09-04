using UnityEngine;

[CreateAssetMenu(fileName = "InfantryAIConfig", menuName = "InfantryAIConfig", order = 0)]
public class InfantryAIConfig : ScriptableObject {
    [Range(0, 1)] public float formationBlendFactor = 0.3f;
    [Range(0, 100)] public float targetAgroCostRange = 10;
}