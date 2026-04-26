using UnityEngine;

public struct CombatOutputInfo {
    public bool wasExploded;
    public float explosionRadius;
    public float explosionForce;
    public bool wasProjectiled;
    public bool wasPunched;
    public int damageTaken;
    public Vector3 damageSourcePosition;
    public bool damageWasFatal;
}
