using System.Runtime.InteropServices;
using Cinemachine;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class CinemachineCircleTargetGroup : MonoBehaviour, ICinemachineTargetGroup {

    [SerializeField] private float radius;
    [SerializeField] private float height;

    private Bounds _boundingBox;
    private BoundingSphere _boundsingSphere;

    public Transform Transform => transform;

    public Bounds BoundingBox => _boundingBox;

    public BoundingSphere Sphere => _boundsingSphere;

    public bool IsEmpty => _boundingBox == null;

    private void Update() {
        _boundingBox = new Bounds(transform.position, new Vector3(radius * 2, height * 2, radius * 2));
        _boundsingSphere = new BoundingSphere(transform.position, radius);
    }

    public void GetViewSpaceAngularBounds(Matrix4x4 observer, out Vector2 minAngles, out Vector2 maxAngles, out Vector2 zRange) {
        var observerForward = observer.MultiplyVector(Vector3.forward);
        var observerForwardOnDisc = Vector3.ProjectOnPlane(observerForward, Transform.up).normalized;

        var localPosition = observer.inverse.MultiplyPoint3x4(transform.position);
        var discFarmost = transform.position + observerForwardOnDisc * radius + Vector3.up * height;
        var localFarmost = observer.inverse.MultiplyPoint3x4(discFarmost);
        var angle = Vector3.Angle(localPosition.normalized, localFarmost.normalized);
        var verticalDistance = localPosition.z * Mathf.Tan(angle * Mathf.Deg2Rad);

        var viewPortPosition = localPosition / localPosition.z;
        var viewPortSize = new Vector3(radius / localPosition.z, verticalDistance / localPosition.z, 0);
        // var viewPortSize = new Vector3(radius / localPosition.z, radius / localPosition.z, 0);

        var b = new Bounds(viewPortPosition, viewPortSize * 2);

        var pMin = b.min;
        var pMax = b.max;
        zRange = new Vector2(localPosition.z - radius, localPosition.z + radius);
        minAngles = new Vector2(
            Vector3.SignedAngle(Vector3.forward, new Vector3(0, pMin.y, 1), Vector3.left),
            Vector3.SignedAngle(Vector3.forward, new Vector3(pMin.x, 0, 1), Vector3.up));
        maxAngles = new Vector2(
            Vector3.SignedAngle(Vector3.forward, new Vector3(0, pMax.y, 1), Vector3.left),
            Vector3.SignedAngle(Vector3.forward, new Vector3(pMax.x, 0, 1), Vector3.up));
    }

    public Bounds GetViewSpaceBoundingBox(Matrix4x4 observer) {
        var observerForward = observer.MultiplyVector(Vector3.forward);
        var observerForwardOnDisc = Vector3.ProjectOnPlane(observerForward, transform.up).normalized;

        var localPosition = observer.inverse.MultiplyPoint3x4(transform.position);
        var discFarmost = transform.position + observerForwardOnDisc * radius + Vector3.up * height;
        var localFarmost = observer.inverse.MultiplyPoint3x4(discFarmost);
        var angle = Vector3.Angle(localPosition.normalized, localFarmost.normalized);
        var verticalDistance = localPosition.z * Mathf.Tan(angle * Mathf.Deg2Rad);

        return new Bounds(
            localPosition,
            new Vector3(
                 radius * 2f,
                 verticalDistance * 2f,
                 radius * 2f
            )
        // radius * 2 * Vector3.one
        );
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        Handles.DrawWireDisc(transform.position, transform.up, radius);
    }
#endif
}