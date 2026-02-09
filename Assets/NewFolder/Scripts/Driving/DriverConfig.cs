using UnityEngine;

[CreateAssetMenu(fileName = "DriverConfig", menuName = "DriverConfig", order = 0)]
public class DriverConfig : ScriptableObject {
    public float powerAccelerationSpeed;
    public float speedCeilingForSteering;
    public float speedKFactor;
    public float minStterAmount;
    public float maxSteerDegrees;
    public float maxEngineTorque;
    public float maxBrakesTorque;
}