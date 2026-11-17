using UnityEngine;

public class ArmorStatsDisplay : MonoBehaviour {
    
    [SerializeField] private CanvasRig canvasRig;
    [SerializeField] private HealthWidget healthWidget;

    public Vector3 WorldPosition {
        set {
            transform.position = value;
        }
    }

    public CanvasRig CanvasRig => canvasRig;

    public void SetHealthState(int health, int maxHealth) {
        healthWidget.SetBar(health, maxHealth);
    }

}