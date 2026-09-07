using System;

[Serializable]
public struct SpawnerSource {
    
    public SpawnConfig initConfig;
    [Inline] public SpawnSpotSource initSpotSource;
    public SpawnVariantSource initVariantSource;

    public SpawnerPrototype Get() {
        return new SpawnerPrototype ( initSetup: new SpawnSetup {
            config = initConfig,
            spot = initSpotSource == null ? default : initSpotSource.Get(),
            variant = initVariantSource.Get()
        });
    }
}