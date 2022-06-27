using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
    
    private TrainHealth _health;
    private Image _healthBarImage;

    private void Awake() {
        _healthBarImage = GetComponent<Image>();
    }

    private void OnDestroy() {
        DetachCurrentHealth();
    }

    public void AttachHealth(TrainHealth newHealth) {
        DetachCurrentHealth();
        if (newHealth == null) {
            return;
        }
        
        _health = newHealth;
        _health.OnHealthChanged += OnHealthChanged;
    }

    private void DetachCurrentHealth() {
        if (_health != null) {
            _health.OnHealthChanged -= OnHealthChanged;
        }
    }

    private void OnHealthChanged(TrainHealth changedHealth) {
        _healthBarImage.fillAmount = changedHealth.HealthAmount;
    }
}