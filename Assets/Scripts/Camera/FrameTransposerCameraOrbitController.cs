using UnityEngine;

public class FrameTransposerCameraOrbitController : MonoBehaviour, ICameraOrbitController {
    [SerializeField] private bool inverseVerticalOrbit = true;
    [SerializeField][Range(-180, 180)] private float initialOrbitHorizontally = 0;
    [SerializeField][Range(0, 89)] private float initialOrbitVertically = 75f;

    private float _horizontalOrbit;
    private float _verticalOrbit;

    private void Awake() {
        ResetOrbitRotation();
    }

    void ICameraOrbitController.StartOrbiting() {
        // do nothing
    }

    void ICameraOrbitController.OrbitDelta(float horizontalDegree, float verticalDegree) {
        _horizontalOrbit += horizontalDegree;
        _verticalOrbit += inverseVerticalOrbit ? -verticalDegree : verticalDegree;
        UpdateOrbitRotation();
    }

    void ICameraOrbitController.StopOrbiting() {
        // do nothing
    }    

    [ContextMenu("Reset Orbit Rotation")]
    private void ResetOrbitRotation() {
        _horizontalOrbit = initialOrbitHorizontally;
        _verticalOrbit = initialOrbitVertically;
        UpdateOrbitRotation();
    }

    [ContextMenu("Update Orbit Rotation")]
    private void UpdateOrbitRotation() {
        transform.rotation = Quaternion.Euler(_verticalOrbit, _horizontalOrbit, 0);
    }
}