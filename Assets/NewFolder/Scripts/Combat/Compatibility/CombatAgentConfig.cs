using System;

namespace Compatibility  {
    [Serializable]
    public struct CombatAgentConfig {
        public int maxHealth;
        public ContactSurface surface;
    }

    [Serializable]
    public enum ContactSurface {
        None,
        Metal,
        Soft
    }
}
