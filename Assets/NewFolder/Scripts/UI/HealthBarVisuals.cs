using TMPro;

using UnityEngine;

public class HealthBarVisuals : MonoBehaviour {
    
    [SerializeField] TextMeshProUGUI text;

    public void SetBar(int health, int maxHealth) {
        text.text = $"[{health} / {maxHealth}]";
    }

}