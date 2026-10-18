using System.Collections.Generic;


using UnityEngine;

public class RamEffectModel {

    public RamEffectModel(int id, RamEffectConfig config, FactionId holderFactionId, VehicleId vehicleId, float baseLinearDrag) {
        Id = id;
        Config = config;
        HolderFactionId = holderFactionId;
        HolderVehicleId = vehicleId;
        BaseLinearDrag = baseLinearDrag;
    }

    public int Id { get; }
    public RamEffectConfig Config { get; }
    public FactionId HolderFactionId { get; }
    public VehicleId HolderVehicleId { get; }
    public float BaseLinearDrag { get; }
    public float TemporalLinearDrag { get; set; }
    
    public Vector3 Position { get; set; }
    public List<HitboxId> InContact { get; } = new ();
    public List<HitboxId> ReceiveContactBuffer = new ();
    public List<HitboxId> LostContactBuffer = new ();

}
