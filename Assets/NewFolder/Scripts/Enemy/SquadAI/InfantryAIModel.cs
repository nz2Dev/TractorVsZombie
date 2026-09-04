using UnityEngine;

public class InfantryAIModel {
    
    public int InfantryId { get; }
    public InfantryAIConfig Config { get; }
    public FormationId FormationId { get; }

    public InfantryAIModel(InfantryAIConfig config, int infantryId, FormationId formation) {
        Config = config;
        InfantryId = infantryId;
        FormationId = formation;
    }
}