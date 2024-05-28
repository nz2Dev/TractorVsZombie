using UnityEngine;
// ReSharper disable Unity.InefficientPropertyAccess

public class Path : MonoBehaviour {

    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    public Vector3 FindClosestNormalPoint(Vector3 point) {
        var ap = point - pointA.position;
        var ab = pointB.position - pointA.position;
        ab.Normalize();
        var normalPointOnSegment = ab * Vector3.Dot(ap, ab);
        return pointA.position + normalPointOnSegment;
    }
    
    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(pointA.position, pointB.position);
    }
}