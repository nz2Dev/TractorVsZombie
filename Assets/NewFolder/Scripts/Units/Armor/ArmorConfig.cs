using UnityEngine;

[CreateAssetMenu(fileName = "ArmorConfig", menuName = "ArmorConfig", order = 0)]
public class ArmorConfig : ScriptableObject {
    
    public CombatAgentConfig combatConfig;
    public VehicleDrivingConfig drivingConfig;
    public WeaponConfig weaponConfig;
    public LoadoutConfig loadoutConfig;

}