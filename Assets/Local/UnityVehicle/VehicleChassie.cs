using System;

using UnityEngine;

[Serializable]
public struct WheelAxis {
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
}

[Serializable]
public struct WheelAxisConfig {
    public float depth;
    public float width;
    [Inline][Local] public WheelCollider wheelPrototype;
}

public class VehicleChassie : MonoBehaviour {

    [Header("Builder")]
    [SerializeField] internal WheelAxisConfig frontAxisConfig;
    [SerializeField] internal WheelAxisConfig rearAxisConfig;
    [Header("Runtime")]
    [SerializeField] internal WheelAxis frontAxis;
    [SerializeField] internal WheelAxis rearAxis;

#if UNITY_EDITOR
    private void OnValidate() {
        if (frontAxisConfig.wheelPrototype != null) frontAxisConfig.wheelPrototype.gameObject.SetActive(false);
        if (rearAxisConfig.wheelPrototype != null) rearAxisConfig.wheelPrototype.gameObject.SetActive(false);
    }
#endif

    private void Awake() {
        frontAxis = BuildAxis(frontAxisConfig);
        rearAxis = BuildAxis(rearAxisConfig);
    }

    private WheelAxis BuildAxis(WheelAxisConfig axisConfig) {
        var wheelRadius = axisConfig.wheelPrototype.radius;
        var leftWheel = Instantiate(axisConfig.wheelPrototype, transform, false);
        leftWheel.transform.localPosition = new Vector3(-0.5f * axisConfig.width, wheelRadius, axisConfig.depth);
        leftWheel.gameObject.SetActive(true);
        
        var rightWheel = Instantiate(axisConfig.wheelPrototype, transform, false);
        rightWheel.transform.localPosition = new Vector3(0.5f * axisConfig.width, wheelRadius, axisConfig.depth);
        rightWheel.gameObject.SetActive(true);
        return new WheelAxis { leftWheel = leftWheel, rightWheel = rightWheel };
    }

    public void SetFrontAxisTorque(float torque) {
        frontAxis.leftWheel.motorTorque = torque;
        frontAxis.rightWheel.motorTorque = torque;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        DrawAxisBoundary(frontAxisConfig);
        DrawAxisBoundary(rearAxisConfig);
    }

    private void DrawAxisBoundary(WheelAxisConfig axisConfig) {
        var wheelRadius = axisConfig.wheelPrototype == null ? 0.1f : axisConfig.wheelPrototype.radius;
        var wheelDiameter = wheelRadius + wheelRadius;
        Gizmos.DrawWireCube(transform.position + new Vector3(0, wheelRadius, axisConfig.depth), new Vector3(axisConfig.width, wheelDiameter, wheelDiameter));
    }
#endif
}