using UnityEngine;

public class HeadquarterStatsDisplay : MonoBehaviour {
    
    [SerializeField] private HealthWidget healthWidget;
    [SerializeField] private StationarCanvasRig canvasRig;

    public StationarCanvasRig CanvasRig => canvasRig;

    public void SetHealthState(int health, int maxHealth) {
        healthWidget.SetBar(health, maxHealth);
    }

}