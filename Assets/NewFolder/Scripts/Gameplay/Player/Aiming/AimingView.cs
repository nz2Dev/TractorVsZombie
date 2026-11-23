using UnityEngine;

public class AimingView {
    
    private AimVisuals aimVisuals;
    private bool isAimVisible;

    private readonly CameraManager cameraManager;

    public AimingView(CameraManager cameraManager) {
        this.cameraManager = cameraManager;
    }

    internal void SetAimVisuals(AimVisuals aimVisualsPrefab) {
        aimVisuals = GameObject.Instantiate(aimVisualsPrefab);
        aimVisuals.HideSelf();
        isAimVisible = false;
    }

    internal void SetAimVisible(bool visible) {
        if (isAimVisible == visible)
            return;
        isAimVisible = visible;
        if (visible)
            aimVisuals.ShowSelf();
        else
            aimVisuals.HideSelf();
    }

    internal Ray GetCameraRay(Vector2 screenPoint) {
        return cameraManager.GetActiveCamera().ScreenPointToRay(screenPoint);
    }

    internal void UpdateAim(TopDownAimInput aimInput) {
        aimVisuals.Transform(aimInput);
    }

}
