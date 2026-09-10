public struct SpawnSetup {
    
    public SpawnConfig config;
    public SpawnSpot spot;
    public SpawnVariant variant;

    public SpawnSetup(SpawnConfig config, SpawnSpot spot, SpawnVariant variant) {
        this.config = config;
        this.spot = spot;
        this.variant = variant;
    }
}