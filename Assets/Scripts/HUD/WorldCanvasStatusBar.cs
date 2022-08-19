using UnityEngine;

public class WorldCanvasStatusBar : MonoBehaviour {
    
    private FaceToFaceConstraint _constraint;
    private Canvas _canvas;

    private void Awake() {
        _constraint = GetComponent<FaceToFaceConstraint>();
        _canvas = GetComponent<Canvas>();
    }

    private void Start() {
        var camera = Camera.main;
        if (camera != null) {
            _canvas.worldCamera = camera;
            _constraint.faceObject = camera.transform;   
        }
    }
}