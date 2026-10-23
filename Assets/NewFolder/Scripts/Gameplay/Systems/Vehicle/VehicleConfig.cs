using UnityEngine;

[CreateAssetMenu(fileName = "VehicleConfig", menuName = "VehicleConfig", order = 0)]
public class VehicleConfig : ScriptableObject {
    public float maxTemporalLinearDrag;
    public float temporalLinearDragRecoverySpeed;
}
