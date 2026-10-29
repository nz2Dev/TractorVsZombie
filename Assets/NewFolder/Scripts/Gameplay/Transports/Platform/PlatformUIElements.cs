using UnityEngine;

[RequireComponent(typeof(WorldSpaceUI))]
public class PlatformUIElements : MonoBehaviour {
    
    [SerializeField] private HealthBarVisuals healthBar;

    private WorldSpaceUI holder;

    public WorldSpaceUI Holder => holder;

    private void Awake() {
        holder = GetComponent<WorldSpaceUI>();
    }

    public void UpdatePosition(Vector3 position) {
        transform.position = position;
    }

    public void HideHealthBar() {
        healthBar.gameObject.SetActive(false);
    }

    public void SetHealthBar(int health, int maxHealth) {
        healthBar.gameObject.SetActive(true);
        healthBar.SetBar(health, maxHealth);
    }

}