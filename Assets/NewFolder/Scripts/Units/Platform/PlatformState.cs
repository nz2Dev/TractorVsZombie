using Combat;

using UnityEngine;

public struct PlatformState {
    public Vector3 position;
    public CombatId combatId;
    public CombatState combatState;
    public int weaponId;
    public WeaponState weaponState;
    public int vehiclePhysicsId;
    public int platformId;
}