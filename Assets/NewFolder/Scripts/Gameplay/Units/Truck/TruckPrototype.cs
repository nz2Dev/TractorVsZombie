
using UnityEngine;

public struct TruckPrototype {
    public Vector3 position;
    public Quaternion rotation;
    public TruckConfig config;
    public RamEffectPrototype ramPrototype;
    public UnityVehicle vehiclePrefab;
    public TruckVisuals visualsPrefab;
    public AudioClip engineLoopSFX;
    public HealthPrototype healthPrototype;
    public FactionPrototype factionPrototype;
    public HitboxPrototype hitboxPrototype;
    public WorldSpaceUI worldSpaceUIPrefab;
}
