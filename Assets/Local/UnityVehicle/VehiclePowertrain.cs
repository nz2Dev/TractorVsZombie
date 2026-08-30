using System;

using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class VehiclePowertrain : MonoBehaviour {
    
    [SerializeField] private float maxEngineTorque = 1000;
    [SerializeField] private float maxBrakesTorque = 1000;

    [SerializeField, HideInInspector] private Rigidbody physics;

    internal float MotorTorque { get; private set; }
    internal float BrakesTorque { get; private set; }

#if UNITY_EDITOR
    private void OnValidate() {
        if (physics == null)
            physics = GetComponent<Rigidbody>();
    }
#endif

    private void Awake() {
        if (physics == null)
            throw new InvalidOperationException();
    }

    public void SetGas(float gas) {
        MotorTorque = gas * maxEngineTorque;
    }

    public void SetBrakes(float brakes) {
        BrakesTorque = brakes * maxBrakesTorque;
    }

}