using UnityEngine;

public class HeadquarterBuilding {

    public int CombatId { get; set; }
    public int ObstacleId { get; set; }
    public Vector3 Position { get; set; }
    public bool Destroyed { get; set; }
    public HeadquarterBuildingConfig Config { get; }

    public HeadquarterBuilding(HeadquarterBuildingConfig config) {
        Config = config;
    }
    
}