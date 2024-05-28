using System.Linq;
using UnityEngine;

public static class DirectionSteerAlgorithm {
    public static Vector3 FollowDirection(this Vehicle vehicle, Vector3 anchor, Vector3 direction, float lookaheadDistance) {
        var anchorToPosition = vehicle.Position - anchor;
        var vehiclePositionOnTurnDirection = Vector3.Project(anchorToPosition, direction);
        var futurePosition = anchor + vehiclePositionOnTurnDirection + direction * lookaheadDistance;
        return vehicle.Seek(futurePosition);
    }
}

public class DirectionSteering : MonoBehaviour, ISteering {
    [SerializeField] private float lookaheadDistance = 1;
    public Vector3 turnDirection;
    public Vector3 turnAnchor;

    public Color Color => Color.blue;
    public string Source => "Direction";
    public float Weight => 1f;

    public void SetDirection(Vector3 direction, Vector3 anchor) {
        turnDirection = direction;
        turnAnchor = anchor;
    }

    public Vector3 CalculateSteeringForce(Vehicle vehicle) {
        return vehicle.FollowDirection(turnAnchor, turnDirection, lookaheadDistance);
    }

    public void OnDrawGizmosSelected(Vehicle vehicle) {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(turnAnchor, 0.1f);
        Gizmos.DrawRay(turnAnchor, turnDirection);
    }
}