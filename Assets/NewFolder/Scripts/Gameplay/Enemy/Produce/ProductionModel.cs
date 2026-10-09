using System.Collections.Generic;

public class ProductionModel {
    public List<ProducerHandle> ProducerHandles { get; } = new ();
    public List<int> ProducedInfantries { get; } = new ();
    public List<int> ProducedArmors { get; } = new ();
    public int TargetFlowFieldId { get; set; }
}