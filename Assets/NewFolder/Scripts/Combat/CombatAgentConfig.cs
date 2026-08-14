using System;

[Serializable]
public enum ContactSurface {
    None,
    Metal,
    Soft
}

[Serializable]
public struct CombatAgentConfig {
    public int maxHealth;
    public ContactSurface surface;
}