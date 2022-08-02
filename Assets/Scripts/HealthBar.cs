using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
    [SerializeField] private Image fillImage;
    [SerializeField] private Health health;

    private void Awake() {
        if (health != null) {
            AttachHealth(health);
        }
    }

    private void OnDestroy() {
        DetachCurrentHealth();
    }

    public void AttachHealth(Health newHealth) {
        DetachCurrentHealth();
        if (newHealth == null) {
            return;
        }
        
        health = newHealth;
        health.OnHealthChanged += OnHealthChanged;
    }

    private void DetachCurrentHealth() {
        if (health != null) {
            health.OnHealthChanged -= OnHealthChanged;
        }
    }

    private void OnHealthChanged(Health changedHealth) {
        fillImage.fillAmount = changedHealth.Amount;
    }
}