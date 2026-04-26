using UnityEngine;

public class LoadoutModel {
    
    public int Id { get; }
    public LoadoutConfig Config { get; }    
    
    public LoadoutModel(int id, LoadoutConfig config) {
        Id = id;
        Config = config;
    }

    public int WeaponId { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 WeaponLocalOffset { get; set; }
    
}
