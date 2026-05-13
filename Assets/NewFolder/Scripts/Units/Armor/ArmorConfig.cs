using UnityEngine;

[CreateAssetMenu(fileName = "ArmorConfig", menuName = "ArmorConfig", order = 0)]
public class ArmorConfig : ScriptableObject {
    public CombatAgentConfig combatConfig;
    // Driving parameters (maxEngineTorque, maxSteerDegrees, etc.) are configured
    // directly on the VehiclePhysics prefab via its [Header("Driving")] fields.
}