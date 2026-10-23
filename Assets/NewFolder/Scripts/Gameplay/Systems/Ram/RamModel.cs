using System.Collections.Generic;
using UnityEngine;

public class RamModel {
    
    public RamModel(RamId id, RamConfig config, FactionId holderFactionId, VehicleId vehicleId) {
        Id = id; 
        Config = config; 
        HolderFactionId = holderFactionId; 
        HolderVehicleId = vehicleId; 
    }

    public RamId Id { get; }
    public RamConfig Config { get; }
    public FactionId HolderFactionId { get; }
    public VehicleId HolderVehicleId { get; }
    
    public int ReceivedContacts { get; set; }

    public List<HitboxId> InContact { get; } = new();
    public List<HitboxId> ReceiveContactBuffer = new();
    public List<HitboxId> LostContactBuffer = new();
    
}
