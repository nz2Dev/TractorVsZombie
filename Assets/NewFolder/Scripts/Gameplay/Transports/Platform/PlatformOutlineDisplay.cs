using System;

using UnityEngine;

public class PlatformOutlineDisplay : MonoBehaviour {
    
    [SerializeField] private FlatSurfaceRig outlineRig;
    [SerializeField] private LineRenderer ringRenderer;
    [SerializeField] private Gradient defaultRingGradient;
    [SerializeField] private Gradient activeRingGradient;
    [SerializeField] private SpriteRenderer bulletRenderer;
    [SerializeField] private Color defaultBulletTint;
    [SerializeField] private Color activeBulletTint;

    public Vector3 Position {
        set {
            transform.position = value;
        }
    }

    public Vector3 AimPoint {
        set {
            outlineRig.AimPoint = value;
        }
    }

    public Sprite AimIcon {
        set {
            bulletRenderer.sprite = value;
        }
    }

    public void ShowNoOutline() {
        ringRenderer.gameObject.SetActive(false);
        bulletRenderer.gameObject.SetActive(false);
    }

    public void ShowSelectedOutline() {
        ringRenderer.gameObject.SetActive(true);
        ringRenderer.colorGradient = defaultRingGradient;
        bulletRenderer.gameObject.SetActive(true);
        bulletRenderer.color = defaultBulletTint;
    }

    public void ShowActiveOutline() {
        ringRenderer.gameObject.SetActive(true);
        ringRenderer.colorGradient = activeRingGradient;
        bulletRenderer.gameObject.SetActive(true);
        bulletRenderer.color = activeBulletTint;
    }

}