using UnityEngine;

public class ProductionBuildingModel {
    
    public int Id { get; }
    public ProductionBuildingConfig Config { get; }
    
    public int CombatId { get; set; }
    public int CommanderId { get; set; }
    public Vector3 Position { get; set; }
    public bool Alie { get; set; }
    public float NextSpawnTime { get; set; }
    public bool Destroyed { get; set; }

    public ProductionBuildingModel(int id, ProductionBuildingConfig config) {
        Id = id;
        Config = config;
    }
}
