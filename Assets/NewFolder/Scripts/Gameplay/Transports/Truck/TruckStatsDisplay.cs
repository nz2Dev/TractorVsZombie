using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class TruckStatsDisplay : MonoBehaviour {
    
    [SerializeField] private CanvasRig canvasRig;
    [SerializeField] private HealthWidget healthWidget;
    [SerializeField] private GameObject selectionBadge;
    [SerializeField] private Image projectileIcon;
    
    public Vector3 WorldPosition {
        set {
            transform.position = value;
        }
    }

    public CanvasRig CanvasRig => canvasRig;

    public void SetHealthStat(int health, int maxHealth) {
        healthWidget.SetBar(health, maxHealth);
    }

    public void SetSelected(bool selected) {
        if (selectionBadge != null)
            selectionBadge.SetActive(selected);
    }

}
