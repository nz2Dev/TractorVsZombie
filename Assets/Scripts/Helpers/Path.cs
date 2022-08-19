using UnityEngine;
// ReSharper disable Unity.InefficientPropertyAccess

public class Path : MonoBehaviour {

    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    public bool FindClosestNormalPoint(Vector3 point, out Vector3 globalPoint) {
        var ap = point - pointA.position;
        var ab = pointB.position - pointA.position;
        ab.Normalize();
        var normalPointOnSegment = ab * Vector3.Dot(ap, ab);
        globalPoint = pointA.position + normalPointOnSegment;
        return true;
    }
    
    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(pointA.position, pointB.position);
    }
}