using UnityEngine;

public struct CombatOutputInfo {
    public bool wasExploded;
    public bool wasProjectiled;
    public bool wasPunched;
    public int damageTaken;
    public Vector3 damageSourcePosition;
    public bool damageWasFatal;
}
