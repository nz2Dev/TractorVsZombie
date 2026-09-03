public interface IProducer {

    bool IsValid(); // todo: more clear would be IsDespawned()
    void SpawnEntity();
    bool TryGetSpawnResult(out SpawnResult spawnResult);

}
