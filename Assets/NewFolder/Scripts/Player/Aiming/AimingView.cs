using UnityEngine;

public class AimingView {
    
    private AimVisuals aimVisuals;

    private readonly CameraManager cameraManager;

    public AimingView(CameraManager cameraManager) {
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

    internal Ray GetCameraRay(Vector2 screenPoint) {
        return cameraManager.GetActiveCamera().ScreenPointToRay(screenPoint);
    }

    internal void UpdateAim(TopDownAimInput aimInput) {
        aimVisuals.Transform(aimInput);
    }

    internal void HideAim() {
        aimVisuals.HideSelf();
    }    

}