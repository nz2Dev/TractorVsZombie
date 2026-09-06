
using System;

public struct SpawnerPrototype {
    
    public SpawnConfig initConfig;
    public SpawnSpot initSpawnSpot;
    public SpawnVariant initSpawnVariant;

    public SpawnerPrototype(SpawnConfig config, SpawnSpot spawnSpot, SpawnVariant spawnVariant) {
        this.initConfig = config;
        this.initSpawnSpot = spawnSpot;
        this.initSpawnVariant = spawnVariant;
    }
}