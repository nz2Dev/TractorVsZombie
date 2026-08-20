using UnityEngine;

namespace Compatibility {

    internal class CombatAgent : IPositionSource {

        public int Id { get; }
        public bool Alie { get; }
        public CombatAgentConfig Config { get; }
        public float Height { get; }
        public ProximityId ProximityId { get; set; }
        public RaycastId HitboxId { get; set; }

        public Vector3 Position { get; set; }
        public int Health { get; set; }

        public int ReceivedDamage { get; set; }
        public Vector3 DamageSourcePosition { get; set; }
        public bool DamageByProjectile { get; set; }
        public bool DamageByExplosion { get; set; }
        public ExplosionData ExplosionData { get; set; }
        public bool DamageByPunch { get; set; }
        public bool Exploded { get; set; }

        public CombatOutputInfo Output { get; set; }

        public CombatAgent(int id, bool alie, CombatAgentConfig config, float height) {
            Id = id;
            Alie = alie;
            Config = config;
            Height = height;
        }

        internal void ClearEvents() {
            ReceivedDamage = 0;
            DamageByProjectile = false;
            DamageByExplosion = false;
            DamageByPunch = false;
        }
    }
}
