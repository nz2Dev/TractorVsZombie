using System;
using UnityEngine;

public class Health : MonoBehaviour {
    [SerializeField] private int max = 100;
    [SerializeField] private int initial = 100;
    [SerializeField] private bool godMod = false;
    
    private int _value;

    public int Value => _value;
    public int Max => max;
    public float Amount => (float) _value / max;

    public bool IsZero => _value <= 0;
    
    public event Action<Health> OnHealthChanged;

    private void Awake() {
        _value = Mathf.Min(Mathf.Clamp(initial, 0, max), max);
    }

    public void TakeDamage(int damage) {
        if (godMod) {
            Debug.LogWarning($"Disperse damage {damage} in godMode");
            return;
        }
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