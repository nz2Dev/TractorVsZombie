using System;

using TMPro;

using UnityEngine;

public class PlatformStatsDisplay : MonoBehaviour {

    [SerializeField] private CanvasRig canvasRig;    
    [SerializeField] private HealthWidget healthWidget;
    [SerializeField] private TextMeshProUGUI durabilityLabel;

    public Vector3 WorldPosition {
        set {
            transform.position = value;
        }
    }

    public CanvasRig CanvasRig => canvasRig;

    public void HideHealthBar() {
        healthWidget.gameObject.SetActive(false);
    }

    public void SetHealthBar(int health, int maxHealth) {
        healthWidget.gameObject.SetActive(true);
        healthWidget.SetBar(health, maxHealth);
    }

    internal void SetDurabilityBar(int durability, int maxDurability) {
        durabilityLabel.text = $"[{durability} out of {maxDurability}]";
    }
}