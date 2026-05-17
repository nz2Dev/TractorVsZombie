using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
[RequireComponent(typeof(VehicleBody))]
[RequireComponent(typeof(VehicleChassie))]
public class UnityVehicle : MonoBehaviour {

    [SerializeField] private float maxTorque; // example field, part of another module component in future
    
    private VehicleBody body;
    private VehicleChassie chassie;

    private void Awake() {
        body = GetComponent<VehicleBody>();
        chassie = GetComponent<VehicleChassie>();
    }

    public void Gas(float input) {
        chassie.SetAxisTorque(WheelAxisName.Front, input * maxTorque);
        chassie.SetAxisTorque(WheelAxisName.Rear, input * maxTorque);
    }

}