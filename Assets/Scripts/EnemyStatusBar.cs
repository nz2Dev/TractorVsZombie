using UnityEngine;

public class EnemyStatusBar : MonoBehaviour {

    private void Start() {
        var enemy = GetComponentInParent<Enemy>();
        var healthBar = GetComponentInChildren<HealthBar>();

        var transformConstraint = GetComponent<FaceToFaceConstraint>();
        var canvas = GetComponent<Canvas>();
        
        var trainHealth = enemy.GetComponent<Health>();
        if (trainHealth != null) {
            healthBar.AttachHealth(trainHealth);    
        }

        var main = Camera.main;
        if (main != null) {
            canvas.worldCamera = main;
            transformConstraint.faceObject = main.transform;   
        }
    }
    
}