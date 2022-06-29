using System;
using UnityEngine;

public class Health : MonoBehaviour {
    [SerializeField] private int max = 100;
    
    private int _value;

    public int Value => _value;
    public int Max => max;
    public float Amount => (float)_value / max;

    public bool IsZero => _value <= 0;
    
    public event Action<Health> OnHealthChanged;

    private void Start() {
        _value = max;
    }

    public void TakeDamage(int damage) {
        if (damage < 0) {
            Debug.LogError($"Damage can't be negative, was {damage}");
            return;
        }

        if (_value <= 0) {
            Debug.LogWarning($"{name}'s health <= 0 while taking a damage");
            return;
        }

        _value -= damage;
        _value = Mathf.Clamp(_value, 0, Max);
        
        OnHealthChanged?.Invoke(this);
    }
}