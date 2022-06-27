using UnityEngine;

public class TrainElementStatusBar : MonoBehaviour {
    
    private FaceToFaceConstraint _transformConstraint;
    private TrainElement _trainElement;
    private HealthBar _healthBar;
    private Canvas _canvas;

    private void Awake() {
        _trainElement = GetComponentInParent<TrainElement>();
        _healthBar = GetComponentInChildren<HealthBar>();

        _transformConstraint = GetComponent<FaceToFaceConstraint>();
        _canvas = GetComponent<Canvas>();
    }

    private void Start() {
        var trainHealth = _trainElement.GetComponent<TrainHealth>();
        if (trainHealth != null) {
            _healthBar.AttachHealth(trainHealth);    
        }

        var main = Camera.main;
        if (main != null) {
            _canvas.worldCamera = main;
            _transformConstraint.faceObject = main.transform;   
        }
    }
}