using System.Collections.Generic;

public class EnemyModel {
    
    public EnemyModel(InfantryAIConfig infantryAIConfig) {
        InfantryAIConfig = infantryAIConfig;
    }

    public InfantryAIConfig InfantryAIConfig { get; }

    public bool ChasingCenter { get; set; }
    public List<IProducer> Producers { get; } = new ();
    public int MainGoalFlowFieldId { get; set; }
}