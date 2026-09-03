using System.Collections.Generic;

public class EnemyModel {
    
    public EnemyModel(InfantryAIConfig infantryAIConfig) {
        InfantryAIConfig = infantryAIConfig;
    }

    public InfantryAIConfig InfantryAIConfig { get; }

    public bool ChasingCenter { get; set; }
    public int MainGoalFlowFieldId { get; set; }
}