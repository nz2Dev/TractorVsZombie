using System;

[Serializable]
public enum ProducerType {
    Space,
    Structure
}

[Serializable]
public struct ProducerHandle {
    public int producerId;
    public ProducerType type;
}

public interface IProducer {

    bool IsValid();
    bool TryGetSpawnResult(out SpawnResult spawnResult);

}