using UnityEngine;

public interface IProducer {

    bool IsValid(); // todo: more clear would be IsDespawned()
    Vector3 Position { get; }
    void SpawnEntity();
    bool TryGetSpawnResult(out SpawnResult spawnResult);

}
