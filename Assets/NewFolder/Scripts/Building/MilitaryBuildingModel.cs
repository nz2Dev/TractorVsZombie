using UnityEngine;

public class MilitaryBuildingModel {
    
    public int Id { get; }
    public MilitaryBuildingConfig Config { get; }
    
    public int CombatId { get; set; }
    public int CommanderId { get; set; }
    public Vector3 Position { get; set; }
    public bool Alie { get; set; }
    public float NextSpawnTime { get; set; }
    public bool Destroyed { get; set; }

    public MilitaryBuildingModel(int id, MilitaryBuildingConfig config) {
        Id = id;
        Config = config;
    }
}
