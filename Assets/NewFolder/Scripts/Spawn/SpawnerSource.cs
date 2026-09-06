using System;

[Serializable]
public struct SpawnerSource {
    
    public SpawnConfig initConfig;
    [Inline] public SpawnSpotSource initSpotSource;
    public SpawnVariantSource initVariantSource;

    public SpawnerPrototype Get() {
        return new SpawnerPrototype {
            initConfig = initConfig,
            initSpawnSpot = initSpotSource == null ? default : initSpotSource.Get(),
            initSpawnVariant = initVariantSource.Get()
        };
    }
}