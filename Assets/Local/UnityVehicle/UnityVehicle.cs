using UnityEngine;

public class UnityVehicle : MonoBehaviour {

    [SerializeField] private float maxTroque; // example field, part of another module component in future
    [Space]
    [SerializeField] private VehicleChassie chassie;

    public void Gas(float input) {
        chassie.SetFrontAxisTorque(input * maxTroque);
    }
}