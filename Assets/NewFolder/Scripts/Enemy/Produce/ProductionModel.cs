using System.Collections.Generic;

public class ProductionModel {
    public List<IProducer> Producers { get; } = new ();
    public List<int> ProducedInfantries { get; } = new ();
    public List<int> ProducedArmors { get; } = new ();
}