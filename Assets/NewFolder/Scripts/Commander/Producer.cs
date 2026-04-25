using System;

[Serializable]
public enum ProducerType {
    ProductionSpace,
    ProductionBuilding
}

[Serializable]
public struct ProducerReference {
    public int producerUniqueId;
    public ProducerType type;
}

public interface IProducer {

    bool IsValid();
    bool TryGetSpawnResult(out SpawnResult spawnResult);

}