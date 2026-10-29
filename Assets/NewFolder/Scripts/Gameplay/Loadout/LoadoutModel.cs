using UnityEngine;

public class LoadoutModel {
    
    public int Id { get; }
    
    public LoadoutModel(int id) {
        Id = id;
    }

    public FactionId HostFactionId { get; set; }
    public Vector3 Position { get; set; }
    public HealthId HealthId { get; set; }
    public HitboxId HitboxId { get; set; }
    public int WeaponId { get; set; }
    public Vector3 WeaponLocalOffset { get; set; }
    
}
