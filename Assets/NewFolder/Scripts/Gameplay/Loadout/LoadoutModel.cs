using UnityEngine;

public class LoadoutModel {
    
    public int Id { get; }
    public LoadoutModel(int id) {
        Id = id;
    }

    public int WeaponId { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 WeaponLocalOffset { get; set; }
    
}
