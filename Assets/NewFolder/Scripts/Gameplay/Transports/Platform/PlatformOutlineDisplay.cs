using System;

using UnityEngine;

public class PlatformOutlineDisplay : MonoBehaviour {
    
    [SerializeField] private FlatSurfaceRig outlineRig;
    [SerializeField] private LineRenderer ringRenderer;

    private bool visibility;
    private bool selected;

    public Vector3 Position {
        set {
            transform.position = value;
        }
    }

    public FlatSurfaceRig Rig => outlineRig;

    public void SetOutlineVisibility(bool visibility) {
        this.visibility = visibility;
        ApplyRingIsActiveState();
    }

    internal void SetSelected(bool selected) {
        this.selected = selected;
        ApplyRingIsActiveState();
    }

    private void ApplyRingIsActiveState() {
        ringRenderer.gameObject.SetActive(visibility && selected);
    }
}