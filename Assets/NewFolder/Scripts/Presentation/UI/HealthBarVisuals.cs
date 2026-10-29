using System;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class HealthBarVisuals : MonoBehaviour {
    
    [SerializeField] private RadialSlider radialSlider;
    [SerializeField] TextMeshProUGUI text;

    public void SetBar(int health, int maxHealth) {
        text.text = $"[{health} / {maxHealth}]";
        if (radialSlider != null) {
            var healthNoralized = (float) health / Mathf.Max(maxHealth, 1);
            radialSlider.Value = healthNoralized;
        }
    }

}