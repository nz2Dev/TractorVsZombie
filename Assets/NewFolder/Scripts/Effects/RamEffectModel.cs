using UnityEngine;

public class RamEffectModel {

    public RamEffectConfig Config { get; }
    public int Id { get; private set; }
    public int CombatId { get; private set; }

    public RamEffectModel(int id, int combatId, RamEffectConfig config) {
        Id = id;
        CombatId = combatId;
        Config = config;
    }
    
    public Vector3 Position { get; set; }

}