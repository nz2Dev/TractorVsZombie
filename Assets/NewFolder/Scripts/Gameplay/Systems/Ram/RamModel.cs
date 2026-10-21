using System.Collections.Generic;
using UnityEngine;

public class RamModel {
    
    public RamModel(int id, RamConfig config, FactionId holderFactionId, VehicleId vehicleId, float baseLinearDrag) {
        Id = id; 
        Config = config; 
        HolderFactionId = holderFactionId; 
        HolderVehicleId = vehicleId; 
        BaseLinearDrag = baseLinearDrag;
    }

    public int Id { get; }
    public RamConfig Config { get; }
    public FactionId HolderFactionId { get; }
    public VehicleId HolderVehicleId { get; }
    
    public float BaseLinearDrag { get; }
    public float TemporalLinearDrag { get; set; }
    public int ReceivedContacts { get; set; }

    public List<HitboxId> InContact { get; } = new();
    public List<HitboxId> ReceiveContactBuffer = new();
    public List<HitboxId> LostContactBuffer = new();
    
}
