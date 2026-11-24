using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class TruckStatsDisplay : MonoBehaviour {
    
    [SerializeField] private CanvasRig canvasRig;
    [SerializeField] private HealthWidget healthWidget;
    [SerializeField] private Image bulletImage;
    
    public Vector3 WorldPosition {
        set {
            transform.position = value;
        }
    }

    public Transform AimTransform {
        set {
            canvasRig.AimTransform = value;
        }
    }

    public Vector3 ForwardInput {
        set {
            canvasRig.ForwardInput = value;
        }
    }

    public bool BulletIconDisplayed {
        set {
            bulletImage.gameObject.SetActive(value);
        }
    }

    public Sprite BulletIcon {
        set {
            bulletImage.sprite = value;
        }
    }

    public void ShowHealthBar(int health, int maxHealth) {
        healthWidget.gameObject.SetActive(true);
        healthWidget.SetBar(health, maxHealth);
    }

    public void HideHealthBar() {
        healthWidget.gameObject.SetActive(false);
    }

}
