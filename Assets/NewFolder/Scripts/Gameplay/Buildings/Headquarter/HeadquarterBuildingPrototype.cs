
using UnityEngine;

public struct HeadquarterBuildingPrototype {
    
    public Vector3 position;
    public Quaternion rotation;
    public HealthPrototype healthPrototype;
    public FactionPrototype factionPrototype;
    public HitboxPrototype hitboxPrototype;
    public StructurePrototype structurePrototype;
    public HeadquarterBuildingVisuals visualsPrefab;
    public WorldSpaceUI worldSpaceUIPrefab;

    public HeadquarterBuildingPrototype(Vector3 position, Quaternion rotation,
        HealthPrototype healthPrototype, FactionPrototype factionPrototype, HitboxPrototype hitboxPrototype, StructurePrototype structurePrototype, HeadquarterBuildingVisuals visualsPrefab, WorldSpaceUI worldSpaceUIPrefab) {
        this.position = position;
        this.rotation = rotation;
        this.healthPrototype = healthPrototype;
        this.factionPrototype = factionPrototype;
        this.hitboxPrototype = hitboxPrototype;
        this.structurePrototype = structurePrototype;
        this.visualsPrefab = visualsPrefab;
        this.worldSpaceUIPrefab = worldSpaceUIPrefab;
    }
}
