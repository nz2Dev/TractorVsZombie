using System;

using UnityEngine;

public class TruckOutlineDisplay : MonoBehaviour {
    
    [SerializeField] private FlatSurfaceRig surfaceRig;
    [SerializeField] private LineRenderer ringRenderer;

    private bool hasAim;
    private bool hasSelection;

    public Vector3 Position {
        set {
            transform.position = value;
        }
    }

    public Vector3 AimPoint {
        set {
            surfaceRig.AimPoint = value;
        }
    }

    public bool AimVisibility {
        set {
            hasAim = value;
            ApplyRingIsActive();
        }
    }

    public bool Selection {
        set {
            hasSelection = value;
            ApplyRingIsActive();
        }
    }

    private void ApplyRingIsActive() {
        ringRenderer.gameObject.SetActive(hasAim && hasSelection);
    }
}