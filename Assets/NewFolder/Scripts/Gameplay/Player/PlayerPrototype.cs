using UnityEngine;

public struct PlayerPrototype {
    public PlayerConfig config;
    public LoadoutPrototype[] initialLoadouts;
    public int initialPlatformCount;
    public TruckPrototype initialTruckPrototype;
    public AimingPrototype aimingPrototype;
    public AssemblingPrototype assemblingPrototype;
    public CollectingPrototype collectingPrototype;
}
