using UnityEngine;
using UnityEngine.Animations;

public class TruckStatsDisplay : MonoBehaviour {
    
    [SerializeField] private FlatRotation displayRotator;
    [SerializeField] private RollConstraint canvasConstraint;
    [SerializeField] private HealthBarVisuals healthBar;
    
    public Vector3 WorldPosition {
        set {
            transform.position = value;
        }
    }

    public Camera LookCamera {
        set {
            canvasConstraint.Target = value.transform;
        }
    }

    public Vector3 UnitForward {
        set {
            displayRotator.ForwardInput = value;
        }
    }

    public void SetHealthStat(int health, int maxHealth) {
        healthBar.SetBar(health, maxHealth);
    }

}