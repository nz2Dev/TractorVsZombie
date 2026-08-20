using System;

using Combat;

namespace Compatibility  {
    [Serializable]
    public struct CombatAgentConfig {
        public int maxHealth;
        public ContactSurface surface;
    }
}
