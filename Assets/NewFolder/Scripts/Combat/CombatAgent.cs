using UnityEngine;

internal class CombatAgent : IPositionSource, IMetadata {
    
    public int Id { get; }
    public bool Alie { get; }
    public int MaxHealth { get; }
    public float Height { get; }

    public Vector3 Position { get; set; }
    public int Health { get; set; }

    public int ReceivedDamage { get; set; }
    public Vector3 DamageSourcePosition { get; set; }
    public bool DamageByProjectile { get; set; }
    public bool DamageByExplosion { get; set; }
    public ExplosionData ExplosionData { get; set; }
    public bool DamageByPunch { get; set; }

    public CombatOutputInfo Output { get; set; }

    public CombatAgent(int id, bool alie, int maxHealth, float height) {
        Id = id;
        Alie = alie;
        MaxHealth = maxHealth;
        Height = height;
    }

    internal void ClearEvents() {
        ReceivedDamage = 0;
        DamageByProjectile = false;
        DamageByExplosion = false;
        DamageByPunch = false;
    }
}