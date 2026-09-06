
using System;

public struct SpawnerPrototype {
    
    public SpawnerConfig config;
    public SpawnSpot spawnSpot;
    public SpawnVariant spawnVariant;

    public SpawnerPrototype(SpawnerConfig config, SpawnSpot spawnSpot, SpawnVariant spawnVariant) {
        this.config = config;
        this.spawnSpot = spawnSpot;
        this.spawnVariant = spawnVariant;
    }
}