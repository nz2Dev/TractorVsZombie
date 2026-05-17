using UnityEngine;

[RequireComponent(typeof(VehicleBody))]
[RequireComponent(typeof(VehicleChassie))]
public class UnityVehicle : MonoBehaviour {

    [SerializeField] private float maxTroque; // example field, part of another module component in future
    
    private VehicleBody body;
    private VehicleChassie chassie;

    private void Awake() {
        body = GetComponent<VehicleBody>();
        chassie = GetComponent<VehicleChassie>();
    }

    public void Gas(float input) {
        chassie.SetFrontAxisTorque(input * maxTroque);
    }
}