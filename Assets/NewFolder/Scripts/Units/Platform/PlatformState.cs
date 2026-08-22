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

    public PlatformState(Vector3 position, CombatId combatId, CombatState combatState, int weaponId, WeaponState weaponState, int vehiclePhysicsId, int platformId) {
        this.position = position;
        this.combatId = combatId;
        this.combatState = combatState;
        this.weaponId = weaponId;
        this.weaponState = weaponState;
        this.vehiclePhysicsId = vehiclePhysicsId;
        this.platformId = platformId;
    }
}