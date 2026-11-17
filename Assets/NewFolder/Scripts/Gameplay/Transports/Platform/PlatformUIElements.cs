using System;

using TMPro;

using UnityEngine;

[RequireComponent(typeof(WorldSpaceUI))]
public class PlatformUIElements : MonoBehaviour {
    
    [SerializeField] private HealthWidget healthWidget;
    [SerializeField] private TextMeshProUGUI durabilityLabel;

    private WorldSpaceUI holder;

    public WorldSpaceUI Holder => holder;

    private void Awake() {
        holder = GetComponent<WorldSpaceUI>();
    }

    public void UpdatePosition(Vector3 position) {
        transform.position = position;
    }

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