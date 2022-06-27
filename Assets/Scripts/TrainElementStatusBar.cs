using UnityEngine;

public class TrainElementStatusBar : MonoBehaviour {
    
    private TrainElement _trainElement;
    private HealthBar _healthBar;

    private void Awake() {
        _trainElement = GetComponentInParent<TrainElement>();
        _healthBar = GetComponentInChildren<HealthBar>();
    }

    private void Start() {
        var trainHealth = _trainElement.GetComponent<TrainHealth>();
        if (trainHealth != null) {
            _healthBar.AttachHealth(trainHealth);    
        }
    }
}