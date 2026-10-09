using Combat;

using UnityEngine;

public struct ArmorState {
    public Vector3 position;
    public CombatId combatId;
    public bool combatIsAlie;
    public int weaponId;
    public WeaponState weaponState;
    public int vehiclePhysicsId;
}