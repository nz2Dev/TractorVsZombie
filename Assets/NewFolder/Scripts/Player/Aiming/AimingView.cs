using UnityEngine;

public class AimingView {
    
    private AimVisuals aimVisuals;

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

}