using System.Collections.Generic;

using Combat;

using UnityEngine;

public class RamEffectModel {

    public RamEffectModel(int id, RamEffectConfig config, CombatId combatId, int vehicleId, bool holderIsAlie, float baseLinearDrag) {
        Id = id;
        Config = config;
        HolderCombatId = combatId;
        HolderVehicleId = vehicleId;
        HolderIsAlie = holderIsAlie;
        BaseLinearDrag = baseLinearDrag;
    }

    public int Id { get; }
    public RamEffectConfig Config { get; }
    public CombatId HolderCombatId { get; }
    public int HolderVehicleId { get; }
    public bool HolderIsAlie { get; }
    public float BaseLinearDrag { get; }
    public float TemporalLinearDrag { get; set; }
    
    public Vector3 Position { get; set; }
    public List<RaycastId> InContact { get; } = new ();
    public List<RaycastId> ReceiveContactBuffer = new ();
    public List<RaycastId> LostContactBuffer = new ();

}
