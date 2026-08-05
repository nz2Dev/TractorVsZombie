using System;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

public class PlayerView {

    internal readonly UIDocument uiDocument;
    private readonly CameraManager cameraManager;
    private AimVisuals aimVisuals;

    public PlayerView(UIDocument uiDocument, CameraManager cameraManager) {
        this.uiDocument = uiDocument;
        this.cameraManager = cameraManager;
    }

    internal void SetAimVisuals(AimVisuals aimVisualsPrefab) {
        aimVisuals = GameObject.Instantiate(aimVisualsPrefab);
        aimVisuals.HideSelf();
    }

    internal void ShowAim(TopDownAimInput aimInput) {
        aimVisuals.ShowSelf();
        aimVisuals.Transform(aimInput);
    }

    internal void UpdateAim(TopDownAimInput aimInput) {
        aimVisuals.Transform(aimInput);
    }

    internal void HideAim() {
        aimVisuals.HideSelf();
    }    

    internal void UpdateFollowCamera(Vector3 position) {
        cameraManager.UpdateTopDownFollowPosition(position);
    }
    
}