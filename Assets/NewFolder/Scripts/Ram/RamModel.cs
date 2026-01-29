using UnityEngine;

public class RamModel {

    private readonly RamConfig config;

    public RamModel(int id, int combatId, Vector3 position, RamConfig config) {
        Id = id;
        CombatId = combatId;
        Position = position;
        Damage = config.damage;
        this.config = config;
    }

    public int Id { get; private set; }
    public int CombatId { get; private set; }
    public Vector3 Position { get; set; }
    public int Damage { get; set; }

    public float Radius => config.radius;
    public AudioClip[] ImpactSFX => config.impactSFX;
}