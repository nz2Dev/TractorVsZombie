using UnityEngine;

[CreateAssetMenu(fileName = "CommanderConfig", menuName = "CommanderConfig", order = 0)]
public class CommanderConfig : ScriptableObject {
    
    [Inline] public SquadAIConfig squadAIConfig;
    
}