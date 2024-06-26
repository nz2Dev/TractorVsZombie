using UnityEngine;

public interface ISteering {

    Color Color { get; }
    string Source { get; }
    float Weight { get; }

    Vector3 CalculateSteeringForce(Vehicle vehicle);
#if UNITY_EDITOR
    void DrawGizmosSelected(Vehicle vehicle);
#endif

}

[RequireComponent(typeof(Vehicle))]
public class SteeringApplier : MonoBehaviour {
    private Vehicle _vehicle;
    private ISteering[] _steerings;

    private void Awake() {
        _vehicle = GetComponent<Vehicle>();
        _steerings = GetComponents<ISteering>();
    }

    private void Update() {
        foreach (var steering in _steerings) {
            _vehicle.ApplyForce(steering.CalculateSteeringForce(_vehicle) * steering.Weight, steering.Source, steering.Color);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        foreach (var steering in _steerings) {
            steering.DrawGizmosSelected(_vehicle);
        }
    }
#endif
}