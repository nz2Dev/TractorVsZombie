using UnityEngine;

namespace Combat {
    [CreateAssetMenu(fileName = "CombatConfig", menuName = "CombatConfig", order = 0)]
    public class CombatConfig : ScriptableObject {
        public int maxHelath;
        public ContactSurface surface;
    }
}
