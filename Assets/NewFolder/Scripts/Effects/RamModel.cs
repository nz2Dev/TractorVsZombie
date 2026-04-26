using UnityEngine;

public class RamModel {

    public RamConfig Config { get; }
    public int Id { get; private set; }
    public int CombatId { get; private set; }

    public RamModel(int id, int combatId, RamConfig config) {
        Id = id;
        CombatId = combatId;
        Config = config;
    }
    
    public Vector3 Position { get; set; }

}