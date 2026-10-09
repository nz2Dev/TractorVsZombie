using UnityEngine;

namespace Combat {
    public struct DamageResult {
        public DamageType damageType;
        public Vector3 damageSource;
        public bool damageWasFatal;
        public int damage;
    }
}
