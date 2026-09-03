using UnityEngine;

public class InfantryAIModel {
    
    public int InfantryId { get; }
    public InfantryAIConfig Config { get; }

    public InfantryAIModel(InfantryAIConfig config, int infantryId) {
        Config = config;
        InfantryId = infantryId;
    }
}