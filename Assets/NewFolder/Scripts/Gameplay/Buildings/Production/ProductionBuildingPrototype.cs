
using UnityEngine;

public struct ProductionBuildingPrototype {
    public int uniqueId;
    public Vector3 position;
    public Quaternion rotation;
    public GameObject visualsPrefab;
    public StructurePrototype structurePrototype;
    public HealthPrototype healthPrototype;
    public FactionPrototype factionPrototype;
    public HitboxPrototype hitboxPrototype;
    public SpawnerPrototype spawnerPrototype;

    public ProductionBuildingPrototype(int uniqueId, Vector3 position, Quaternion rotation, GameObject visualsPrefab, StructurePrototype structurePrototype, HealthPrototype healthPrototype, FactionPrototype factionPrototype, HitboxPrototype hitboxPrototype, SpawnerPrototype spawnerPrototype) {
        this.uniqueId = uniqueId;
        this.position = position;
        this.rotation = rotation;
        this.visualsPrefab = visualsPrefab;
        this.structurePrototype = structurePrototype;
        this.healthPrototype = healthPrototype;
        this.factionPrototype = factionPrototype;
        this.hitboxPrototype = hitboxPrototype;
        this.spawnerPrototype = spawnerPrototype;
    }
}
