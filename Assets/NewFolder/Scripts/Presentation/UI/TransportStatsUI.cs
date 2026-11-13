using UnityEngine;
using UnityEngine.Animations;

public class TransportStatsUI : MonoBehaviour {
    
    [SerializeField] private RollConstraint canvasConstraint;
    [SerializeField] private HealthBarVisuals healthBar;
    
    public Camera TargetCamera {
        set {
            canvasConstraint.Target = value.transform;
        }
    }

    public Vector3 WorldPosition {
        set {
            transform.position = value;
        }
    }

    public Vector3 UnitForward {
        set {
            var forwardFlat = Vector3.ProjectOnPlane(value, Vector3.up);
            transform.rotation = Quaternion.LookRotation(forwardFlat, Vector3.up);
        }
    }

    public void SetHealthStat(int health, int maxHealth) {
        healthBar.SetBar(health, maxHealth);
    }

}