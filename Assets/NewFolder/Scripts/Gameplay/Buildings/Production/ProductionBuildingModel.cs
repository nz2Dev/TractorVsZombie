
using UnityEngine;

public class ProductionBuildingModel {
    
    public ProductionBuildingModel(int id) {
        Id = id;
    }

    public int Id { get; }
    public HealthId HealthId { get; set; }
    public FactionId FactionId { get; set; }
    public int PathfindingObstacleId { get; set; }
    public StructureId StructureId { get; set; }
    public HitboxId HitboxId { get; set; }
    public SpawnerId SpawnerId { get; set; }

    public bool Destroyed { get; set; }

}
